// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
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
using OsmSharp.Collections;
using OsmSharp.Collections.Tags;
using OsmSharp.Routing;
using OsmSharp.Routing.Osm.Vehicles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsmSharpDataProcessor.Commands.Processors.RouterDbs.Shape
{
    class FeaturesList : IList<IFeature>
    {
        private readonly RouterDb _routerDb;
        private readonly IDictionary<uint, long> _nodeIdMap;
        private readonly bool _forPedestrians;
        private readonly bool _forBicycle;

        /// <summary>
        /// Creates a new features list.
        /// </summary>
        public FeaturesList(RouterDb routerDb, IDictionary<uint, long> nodeIdMap)
        {
            _routerDb = routerDb;
            _nodeIdMap = nodeIdMap;
            
            _forPedestrians = routerDb.Supports(Vehicle.Pedestrian.Fastest());
            _forBicycle = routerDb.Supports(Vehicle.Bicycle.Fastest());
        }

        public IFeature this[int index]
        {
            get
            {
                return this.BuildFeature(index);
            }
            set
            {
                throw new NotSupportedException("List is reaonly.");
            }
        }

        public int Count
        {
            get
            {
                return (int)_routerDb.Network.EdgeCount;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public void Add(IFeature item)
        {
            throw new NotSupportedException("List is reaonly.");
        }

        public void Clear()
        {
            throw new NotSupportedException("List is reaonly.");
        }

        public bool Contains(IFeature item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IFeature[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IFeature> GetEnumerator()
        {
            return new Enumerator(this);
        }

        private class Enumerator : IEnumerator<IFeature>
        {
            private readonly FeaturesList _list;

            public Enumerator(FeaturesList list)
            {
                _list = list;
            }

            private int _current = -1;

            public IFeature Current
            {
                get
                {
                    return _list[_current];
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return _list[_current];
                }
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                _current++;
                return _current < _list.Count;
            }

            public void Reset()
            {
                _current = -1;
            }
        }

        public int IndexOf(IFeature item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, IFeature item)
        {
            throw new NotSupportedException("List is reaonly.");
        }

        public bool Remove(IFeature item)
        {
            throw new NotSupportedException("List is reaonly.");
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException("List is reaonly.");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private IFeature BuildFeature(int index)
        {
            var edge = _routerDb.Network.GetEdge((uint)index);

            var vertexLocation = _routerDb.Network.GeometricGraph.GetVertex(edge.From);
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
            vertexLocation = _routerDb.Network.GeometricGraph.GetVertex(edge.To);
            coordinates.Add(new Coordinate(vertexLocation.Longitude, vertexLocation.Latitude));
            var geometry = new LineString(coordinates.ToArray());

            var length = 0.0;
            for (var i = 0; i < coordinates.Count - 1; i++)
            {
                var coordinate1 = new OsmSharp.Math.Geo.GeoCoordinate(coordinates[i + 0].Y, coordinates[i + 0].X);
                var coordinate2 = new OsmSharp.Math.Geo.GeoCoordinate(coordinates[i + 1].Y, coordinates[i + 1].X);
                length += coordinate1.DistanceReal(coordinate2).Value;
            }

            var tags = new OsmSharp.Collections.Tags.TagsCollection(_routerDb.EdgeProfiles.Get(edge.Data.Profile));
            tags.AddOrReplace(_routerDb.EdgeMeta.Get(edge.Data.MetaId));

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

            if (_forPedestrians)
            {
                var pedestrian = Vehicle.Pedestrian.CanTraverse(tags);
                attributes.AddAttribute("byfoot", pedestrian);
            }
            if (_forBicycle)
            {
                var bicycle = Vehicle.Bicycle.CanTraverse(tags);
                attributes.AddAttribute("bybicycle", bicycle);
            }
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
            if (tags.ContainsKey("way_id"))
            {
                AddTo("way_id", attributes, tags);
            }
            AddTo("tunnel", attributes, tags);
            AddTo("bridge", attributes, tags);
            long startid;
            if (_nodeIdMap == null)
            { // use the vertex id's.
                startid = edge.From;
                attributes.AddAttribute("startid", startid);
            }
            else
            { // use the node id's.
                if (_nodeIdMap.TryGetValue(edge.From, out startid))
                {
                    attributes.AddAttribute("startid", startid);
                }
                else
                {
                    throw new Exception("One of the vertices has no node-id.");
                }
            }
            long endid;
            if (_nodeIdMap == null)
            { // use the vertex id's.
                endid = edge.To;
                attributes.AddAttribute("endid", endid);
            }
            else
            { // use the node id's.
                if (_nodeIdMap.TryGetValue(edge.To, out endid))
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

            return new Feature(geometry, attributes);
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
                    case "motorway_link":
                    case "trunk":
                    case "trunk_link":
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
                    case "motorway_link":
                    case "trunk":
                    case "trunk_link":
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