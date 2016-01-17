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

namespace OsmSharpDataProcessor.Commands.Processors.GTFS
{
    /// <summary>
    /// A processor to create transit db.
    /// </summary>
    public class ProcessorCreateTransitDb : ProcessorBase, TransitDbs.ITransitDbSource
    {
        /// <summary>
        /// Creates a new processor.
        /// </summary>
        public ProcessorCreateTransitDb()
        {

        }

        private Func<GTFSFeed> _getFeed;

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
            if (processors[0] is IGTFSSource)
            { // ok there is a source, keep it around for execution.
                _getFeed = (processors[0] as IGTFSSource).GetFeed();
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
        /// Gets the get transit db function.
        /// </summary>
        /// <returns></returns>
        public Func<TransitDb> GetTransitDb()
        {
            return () =>
            {
                var feed = this._getFeed();

                var transitDb = new TransitDb();
                transitDb.LoadFrom(feed);

                transitDb.SortConnections(DefaultSorting.DepartureTime, null);

                return transitDb;
            };
        }
    }
}
