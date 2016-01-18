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

using System;
using System.Collections.Generic;
using OsmSharp.Routing.Transit.Data;

namespace OsmSharpDataProcessor.Commands.Processors.TransitDbs
{
    /// <summary>
    /// A processor created by a merged filter command.
    /// </summary>
    public class ProcessorMerge : ProcessorBase, ITransitDbSource
    {
        private readonly List<ITransitDbSource> _sources;

        /// <summary>
        /// Creates a new processor merge.
        /// </summary>
        public ProcessorMerge()
        {
            _sources = new List<ITransitDbSource>();
        }

        /// <summary>
        /// Can never execute on it's own.
        /// </summary>
        public override bool CanExecute
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Is always ready.
        /// </summary>
        public override bool IsReady
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Collapses this processor read.
        /// </summary>
        public override void Collapse(List<ProcessorBase> processors)
        {

        }

        /// <summary>
        /// Executes the tasks or commands in this processor.
        /// </summary>
        public override void Execute()
        { // a source cannot be executed.
            throw new InvalidOperationException("This processor cannot be executed, check CanExecute before calling this method.");
        }

        /// <summary>
        /// Adds a source.
        /// </summary>
        public void Add(ITransitDbSource transitDbSource)
        {
            _sources.Add(transitDbSource);
        }

        /// <summary>
        /// Get the function to get the transit db.
        /// </summary>
        public Func<TransitDb> GetTransitDb()
        {
            return () =>
            {
                var transitDb = new TransitDb();

                OsmSharp.Logging.Log.TraceEvent("Processor - Merge", OsmSharp.Logging.TraceEventType.Information,
                    "Reading sources...");
                foreach (var source in _sources)
                {
                    var db = source.GetTransitDb()();
                    transitDb.CopyFrom(db);
                }

                OsmSharp.Logging.Log.TraceEvent("Processor - Merge", OsmSharp.Logging.TraceEventType.Information, 
                    "Sorting connections...");
                transitDb.SortConnections(DefaultSorting.DepartureTime, null);

                return transitDb;
            };
        }
    }
}
