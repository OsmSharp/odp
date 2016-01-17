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

using GTFS;
using System;
using System.Collections.Generic;
using OsmSharp.Routing.Transit.GTFS;
using OsmSharp.Routing.Transit.Data;
using OsmSharpDataProcessor.Commands.Processors.RouterDbs;
using OsmSharp.Routing;
using OsmSharpDataProcessor.Commands.Processors.TransitDbs;

namespace OsmSharpDataProcessor.Commands.Processors.GTFS
{
    /// <summary>
    /// A processor to create transit db.
    /// </summary>
    public class ProcessorCreateMultimodalDb : ProcessorBase, MultimodalDbs.IMultimodalDbSource
    {
        /// <summary>
        /// Creates a new processor.
        /// </summary>
        public ProcessorCreateMultimodalDb()
        {

        }

        private Func<TransitDb> _getTransitDb;
        private Func<RouterDb> _getRouterDb;

        /// <summary>
        /// Collapses the given list of processors by adding this one to it.
        /// </summary>
        public override void Collapse(List<ProcessorBase> processors)
        {
            if (processors == null || processors.Count == 0) { throw new ArgumentOutOfRangeException(); }
            
            // ok combine the transit db and the router db into one multimodal db.
            if (processors[0] is ITransitDbSource &&
                processors[1] is IRouterDbSource)
            {
                _getTransitDb = (processors[0] as ITransitDbSource).GetTransitDb();
                _getRouterDb = (processors[1] as IRouterDbSource).GetRouterDb();
            }
            else if(processors[1] is ITransitDbSource &&
                processors[0] is IRouterDbSource)
            {
                _getTransitDb = (processors[1] as ITransitDbSource).GetTransitDb();
                _getRouterDb = (processors[0] as IRouterDbSource).GetRouterDb();
            }
            else
            {
                throw new Exception("Creating a multimodal requires a transit db an a router db source.");
            }
            processors.RemoveAt(processors.Count - 1);
            processors.RemoveAt(processors.Count - 1);
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
        /// Gets the get transit db function.
        /// </summary>
        /// <returns></returns>
        public Func<MultimodalDb> GetMultimodalDb()
        {
            return () =>
            {
                var transitDb = this._getTransitDb();
                var routerDb = this._getRouterDb();

                return new MultimodalDb(routerDb, transitDb);
            };
        }
    }
}
