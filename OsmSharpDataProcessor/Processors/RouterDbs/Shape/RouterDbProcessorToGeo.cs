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

using OsmSharp.Routing;
using OsmSharp.Routing.Osm.Vehicles;
using OsmSharpDataProcessor.Processors.Geo;
using System;
using System.Collections.Generic;
using NetTopologySuite.Features;

namespace OsmSharpDataProcessor.Processors.RouterDbs.Shape
{
    /// <summary>
    /// A router db target to write a routable shapefile.
    /// </summary>
    public class RouterDbProcessorToGeo : ProcessorBase, IGeoSource
    {
        /// <summary>
        /// Creates a new processor.
        /// </summary>
        public RouterDbProcessorToGeo()
        {

        }

        private Func<RouterDb> _getSourceDb;
        private Dictionary<string, object> _previousMeta;

        /// <summary>
        /// Collapse this processor.
        /// </summary>
        public override int Collapse(List<ProcessorBase> processors, int i)
        {
            if (processors == null || processors.Count == 0) { throw new ArgumentOutOfRangeException(); }
            if (i < 1) { throw new ArgumentOutOfRangeException("i"); }
            
            if (processors[i - 1] is IRouterDbSource)
            { // ok there is a source, keep it around for execution.
                _getSourceDb = (processors[i - 1] as IRouterDbSource).GetRouterDb();
                _previousMeta = processors[i - 1].Meta;
            }
            processors.RemoveAt(i - 1);
            return -1;
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
        { // a source cannot be executed.
            throw new InvalidOperationException("This processor cannot be executed, check CanExecute before calling this method.");
        }

        /// <summary>
        /// Returns true if this processor can be executed.
        /// </summary>
        public override bool CanExecute
        { // a target can be executed.
            get { return false; }
        }

        /// <summary>
        /// Gets the function to get the features.
        /// </summary>
        public Func<IList<IFeature>> GetFeatures
        {
            get
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
                        return WriteShape(db, vertexIdMap);
                    }
                    else
                    { // just write the shape file using the router db id's.
                        return WriteShape(db, null);
                    }
                };
            }
        }

        /// <summary>
        /// Writes a routable shapefile.
        /// </summary>
        private static FeaturesList WriteShape(RouterDb routerDb, IDictionary<uint, long> nodeIdMap)
        {
            OsmSharp.Logging.Log.TraceEvent("Processor - To geo", OsmSharp.Logging.TraceEventType.Information, 
                "Converting to geometries...");
            
            if(!routerDb.Supports(Vehicle.Car.Fastest()))
            {
                throw new Exception("Cannot write convert to geometries without the default 'car' profile being supported.");
            }

            return new FeaturesList(routerDb, nodeIdMap);
        }
    }
}