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
            
            if(!routerDb.Supports(Vehicle.Car.Fastest()))
            {
                throw new Exception("Cannot write routable shapefile without the default 'car' profile being supported.");
            }
            
            OsmSharp.Logging.Log.TraceEvent("Processor - Routerdb Shape Writer", OsmSharp.Logging.TraceEventType.Information, "Writing data...");
            var features = new FeaturesList(routerDb, nodeIdMap);
            var header = ShapefileDataWriter.GetHeader(features[0], features.Count);
            var shapeWriter = new ShapefileDataWriter(shapefile + ".shp", new GeometryFactory())
            {
                Header = header
            };
            shapeWriter.Write(features);
        }
    }
}