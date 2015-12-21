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
using OsmSharp.Routing.Network.Data;
using OsmSharp.Routing.Profiles;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharpDataProcessor.Commands.Processors.RouterDbs
{
    /// <summary>
    /// A routerdb processor.
    /// </summary>
    public class RouterDbProcessorOptimizer : ProcessorBase, IRouterDbSource
    {
        /// <summary>
        /// Creates a new processor.
        /// </summary>
        public RouterDbProcessorOptimizer()
        {

        }

        private Func<RouterDb> _getSourceDb;
        private Dictionary<string, object> _previousMeta;

        /// <summary>
        /// Collapses the given list of processors by adding this one to it.
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
        { // a source is always ready.
            get { return true; }
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
        { // a source cannot be executed.
            get { return false; }
        }

        /// <summary>
        /// Gets the get router db function.
        /// </summary>
        /// <returns></returns>
        public Func<RouterDb> GetRouterDb()
        {
            return () =>
            {
                var db = _getSourceDb();

                // remove vertices with only two neighbours when possible.
                var optimizer = new OsmSharp.Routing.Algorithms.Networks.NetworkOptimizer(
                    db.Network, (EdgeData edgeData1, bool inverted1,
                        EdgeData edgeData2, bool inverted2, out EdgeData mergedEdgeData, out bool mergedInverted) =>
                    {
                        mergedEdgeData = new EdgeData()
                        {
                            Distance = edgeData1.Distance + edgeData2.Distance,
                            MetaId = edgeData2.MetaId,
                            Profile = edgeData2.Profile
                        };
                        mergedInverted = inverted2;
                        if (edgeData1.MetaId != edgeData2.MetaId)
                        { // different meta data, do not merge.
                            return false;
                        }
                        if (inverted1 != inverted2)
                        { // directions are the same.
                            return OsmSharp.Routing.Osm.HighwayComparer.CompareOpposite(db.EdgeProfiles,
                                edgeData1.Profile, edgeData2.Profile);
                        }
                        return OsmSharp.Routing.Osm.HighwayComparer.Compare(db.EdgeProfiles,
                            edgeData1.Profile, edgeData2.Profile);
                    });
                optimizer.Run();

                // remove edges of length zero.
                var removeZero = new OsmSharp.Routing.Algorithms.Networks.ZeroLengthLinksOptimizer(
                    db.Network, (data) =>
                    {
                        var profile = db.EdgeProfiles.Get(data.Profile);
                        return OsmSharp.Routing.Osm.Vehicles.Vehicle.Car.CanTraverse(profile);
                    });
                removeZero.Run();

                // compress the network after removing vertices and edges.
                db.Network.Compress();

                // copy meta.
                if(_previousMeta != null)
                {
                    foreach(var meta in _previousMeta)
                    {
                        this.Meta[meta.Key] = meta.Value;
                    }
                }

                return db;
            };
        }
    }
}