// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using OsmSharp.Collections.Tags;
using OsmSharp.Routing;
using OsmSharp.Routing.Osm.Vehicles;
using System;
using System.Collections.Generic;

namespace OsmSharpDataProcessor.Commands.Processors.RouterDbs.Shape
{
    /// <summary>
    /// A router db target to write a routable shapefile.
    /// </summary>
    public class RouterDbProcessorTargetShape : ProcessorBase, IRouterDbSource
    {
        private readonly string _shapefile;

        /// <summary>
        /// Creates a new processor.
        /// </summary>
        public RouterDbProcessorTargetShape(string shapefile)
        {
            _shapefile = shapefile;
        }

        private Func<RouterDb> _getSourceDb;
        private Dictionary<string, object> _previousMeta;

        /// <summary>
        /// Collapse this processor.
        /// </summary>
        public override void Collapse(List<ProcessorBase> processors)
        {
            if (processors == null || processors.Count == 0) { throw new ArgumentOutOfRangeException(); }

            if (processors.Count > 1)
            { // cannot merge or write multiple router db's.
                throw new Exception("Cannot register multiple processors.");
            }
            if (processors[0] is IRouterDbSource)
            { // ok there is a source, keep it around for execution.
                _getSourceDb = (processors[0] as IRouterDbSource).GetRouterDb();
                _previousMeta = processors[0].Meta;
            }
            processors.RemoveAt(0);
            processors.Add(this);
        }

        /// <summary>
        /// Returns true if this processor is ready.
        /// </summary>
        public override bool IsReady
        { // a source is always ready
            get { return _getSourceDb != null; }
        }

        /// <summary>
        /// Executes the tasks or commands in this processor.
        /// </summary>
        public override void Execute()
        {
            this.GetRouterDb()();
        }

        /// <summary>
        /// Returns true if this processor can be executed.
        /// </summary>
        public override bool CanExecute
        { // a target can be executed.
            get { return true; }
        }

        /// <summary>
        /// Gets the router db.
        /// </summary>
        /// <returns></returns>
        public Func<RouterDb> GetRouterDb()
        {
            return () =>
            {
                var db = _getSourceDb();

                if (_previousMeta.ContainsKey("all_core") &&
                   _previousMeta.ContainsKey("node_id_map"))
                { // there is a full node map, write the shapefile using the original node id's as id's.
                    var nodeIdMap = (IDictionary<long, uint>)_previousMeta["node_id_map"];
                    var vertexIdMap = new Dictionary<uint, long>();
                    foreach (var pair in nodeIdMap)
                    {
                        vertexIdMap.Add(pair.Value, pair.Key);
                    }
                    WriteShape(_shapefile, db, vertexIdMap);
                }
                else
                { // just write the shape file using the router db id's.
                    WriteShape(_shapefile, db, null);
                }

                return db;
            };
        }

        /// <summary>
        /// Writes a routable shapefile.
        /// </summary>
        private static void WriteShape(string shapefile, RouterDb routerDb, IDictionary<uint, long> nodeIdMap)
        {
            OsmSharp.Logging.Log.TraceEvent("Processor - Routerdb Shape Writer", OsmSharp.Logging.TraceEventType.Information, "Converting to geometries...");
            var features = new List<NetTopologySuite.Features.IFeature>();
            for (uint edgeId = 0; edgeId < routerDb.Network.EdgeCount; edgeId++)
            {
                var edge = routerDb.Network.GetEdge(edgeId);

                var vertexLocation = routerDb.Network.GeometricGraph.GetVertex(edge.From);
                var coordinates = new List<Coordinate>();
                coordinates.Add(new Coordinate(vertexLocation.Longitude, vertexLocation.Latitude));
                var shape = edge.Shape;
                if (shape != null)
                {
                    var shapeEnumerable = shape.GetEnumerator();
                    shapeEnumerable.Reset();
                    while (shapeEnumerable.MoveNext())
                    {
                        coordinates.Add(new Coordinate(shapeEnumerable.Current.Longitude,
                            shapeEnumerable.Current.Latitude));
                    }
                }
                vertexLocation = routerDb.Network.GeometricGraph.GetVertex(edge.To);
                coordinates.Add(new Coordinate(vertexLocation.Longitude, vertexLocation.Latitude));
                var geometry = new LineString(coordinates.ToArray());

                var length = 0.0;
                for (var i = 0; i < coordinates.Count - 1; i++)
                {
                    var coordinate1 = new OsmSharp.Math.Geo.GeoCoordinate(coordinates[i + 0].Y, coordinates[i + 0].X);
                    var coordinate2 = new OsmSharp.Math.Geo.GeoCoordinate(coordinates[i + 1].Y, coordinates[i + 1].X);
                    length += coordinate1.DistanceReal(coordinate2).Value;
                }

                var tags = new OsmSharp.Collections.Tags.TagsCollection(routerDb.EdgeProfiles.Get(edge.Data.Profile));
                tags.AddOrReplace(routerDb.EdgeMeta.Get(edge.Data.MetaId));

                var attributes = new AttributesTable();
                AddTo("highway", attributes, tags);

                var oneway = Vehicle.Car.IsOneWay(tags);
                if (oneway == null)
                {
                    attributes.AddAttribute("oneway", string.Empty);
                }
                else if (oneway.Value)
                {
                    attributes.AddAttribute("oneway", "F");
                }
                else
                {
                    attributes.AddAttribute("oneway", "B");
                }

                var pedestrian = Vehicle.Pedestrian.CanTraverse(tags);
                attributes.AddAttribute("byfoot", pedestrian);
                var bicycle = Vehicle.Bicycle.CanTraverse(tags);
                attributes.AddAttribute("bybicycle", bicycle);
                var car = Vehicle.Car.CanTraverse(tags);
                attributes.AddAttribute("bycar", car);

                var speed = Vehicle.Car.ProbableSpeed(tags);
                attributes.AddAttribute("speed", (int)System.Math.Round(speed.Value, 0));
                attributes.AddAttribute("length", System.Math.Round(length, 3));

                string lanesString;
                var lanes = 1;
                var lanesVerified = false;
                if (tags.TryGetValue("lanes", out lanesString))
                {
                    lanesVerified = true;
                    if (!int.TryParse(lanesString, out lanes))
                    {
                        lanes = 1;
                        lanesVerified = false;
                    }
                }
                attributes.AddAttribute("lanes", lanes);
                attributes.AddAttribute("lanes_ve", lanesVerified);

                string maxSpeedString;
                var maxSpeed = (int)Vehicle.Car.MaxSpeedAllowed(tags).Value;
                var maxSpeedVerified = false;
                if (tags.TryGetValue("maxspeed", out maxSpeedString))
                {
                    maxSpeedVerified = true;
                    if (!int.TryParse(maxSpeedString, out maxSpeed))
                    {
                        maxSpeedVerified = false;
                        maxSpeed = (int)Vehicle.Car.MaxSpeedAllowed(tags).Value;
                    }
                }
                attributes.AddAttribute("maxspeed", maxSpeed);
                attributes.AddAttribute("maxspeed_ve", maxSpeedVerified);

                AddTo("name", attributes, tags);
                AddTo("way_id", attributes, tags);
                AddTo("tunnel", attributes, tags);
                AddTo("bridge", attributes, tags);
                long startid;
                if (nodeIdMap == null)
                { // use the vertex id's.
                    startid = edge.From;
                }
                else
                { // use the node id's.
                    if (nodeIdMap.TryGetValue(edge.From, out startid))
                    {
                        attributes.AddAttribute("startid", startid);
                    }
                    else
                    {
                        throw new Exception("One of the vertices has no node-id.");
                    }
                }
                long endid;
                if (nodeIdMap == null)
                { // use the vertex id's.
                    endid = edge.To;
                }
                else
                { // use the node id's.
                    if (nodeIdMap.TryGetValue(edge.To, out endid))
                    {
                        attributes.AddAttribute("endid", endid);
                    }
                    else
                    {
                        throw new Exception("One of the vertices has no node-id.");
                    }
                }
                attributes.AddAttribute("linkid", edge.Id);
                FunctionalRoadClass frc;
                FormOfWay fow;
                if (!TryMatching(tags, out frc, out fow))
                {
                    frc = FunctionalRoadClass.Frc7;
                    fow = FormOfWay.Undefined;
                }
                attributes.AddAttribute("frc", (int)frc);
                attributes.AddAttribute("fow", fow.ToString().ToLowerInvariant());

                features.Add(new Feature(geometry, attributes));
            }

            OsmSharp.Logging.Log.TraceEvent("Processor - Routerdb Shape Writer", OsmSharp.Logging.TraceEventType.Information, "Writing data...");
            var header = ShapefileDataWriter.GetHeader(features[0], features.Count);
            var shapeWriter = new ShapefileDataWriter(shapefile + ".shp", new GeometryFactory())
            {
                Header = header
            };
            shapeWriter.Write(features);
        }

        /// <summary>
        /// Adds the value of the given key to the attributes table if found, otherwise adds empty string.
        /// </summary>
        private static void AddTo(string name, AttributesTable table, TagsCollection tags)
        {
            AddTo(name, table, tags, string.Empty);
        }

        /// <summary>
        /// Adds the value of the given key to the attributes table if found, otherwise adds a given default value.
        /// </summary>
        private static void AddTo(string name, AttributesTable table, TagsCollection tags,
            string defaultValue)
        {
            var value = string.Empty;
            if (!tags.TryGetValue(name, out value))
            {
                value = defaultValue;
            }
            table.AddAttribute(name, value);
        }

        /// <summary>
        /// Tries to match the given tags and figure out a corresponding frc and fow.
        /// </summary>
        /// <returns>False if no matching was found.</returns>
        private static bool TryMatching(TagsCollectionBase tags, out FunctionalRoadClass frc, out FormOfWay fow)
        {
            frc = FunctionalRoadClass.Frc7;
            fow = FormOfWay.Undefined;
            string highway;
            if (tags.TryGetValue("highway", out highway))
            {
                switch (highway)
                { // check there reference values against OSM: http://wiki.openstreetmap.org/wiki/Highway
                    case "motorway":
                    case "trunk":
                        frc = FunctionalRoadClass.Frc0;
                        break;
                    case "primary":
                    case "primary_link":
                        frc = FunctionalRoadClass.Frc1;
                        break;
                    case "secondary":
                    case "secondary_link":
                        frc = FunctionalRoadClass.Frc2;
                        break;
                    case "tertiary":
                    case "tertiary_link":
                        frc = FunctionalRoadClass.Frc3;
                        break;
                    case "road":
                    case "road_link":
                    case "unclassified":
                    case "residential":
                        frc = FunctionalRoadClass.Frc4;
                        break;
                    case "living_street":
                        frc = FunctionalRoadClass.Frc5;
                        break;
                    default:
                        frc = FunctionalRoadClass.Frc7;
                        break;
                }
                switch (highway)
                { // check there reference values against OSM: http://wiki.openstreetmap.org/wiki/Highway
                    case "motorway":
                    case "trunk":
                        fow = FormOfWay.Motorway;
                        break;
                    case "primary":
                    case "primary_link":
                        fow = FormOfWay.MultipleCarriageWay;
                        break;
                    case "secondary":
                    case "secondary_link":
                    case "tertiary":
                    case "tertiary_link":
                        fow = FormOfWay.SingleCarriageWay;
                        break;
                    default:
                        fow = FormOfWay.SingleCarriageWay;
                        break;
                }
                return true; // should never fail on a highway tag.
            }
            return false;
        }
    }
}