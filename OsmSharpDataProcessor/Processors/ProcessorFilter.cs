// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using OsmSharp.Osm.Streams;
using System;
using System.Collections.Generic;

namespace OsmSharpDataProcessor.Processors
{
    /// <summary>
    /// A processor created by a filter command.
    /// </summary>
    public class ProcessorFilter : ProcessorSource
    {
        /// <summary>
        /// Holds the stream filter.
        /// </summary>
        private OsmStreamFilter _filter;

        /// <summary>
        /// Holds the ready status flag.
        /// </summary>
        private bool _isReady = false;

        /// <summary>
        /// Creates a new processor filter.
        /// </summary>
        /// <param name="filter"></param>
        public ProcessorFilter(OsmStreamFilter filter)
            :base(filter)
        {
            _filter = filter;
        }

        /// <summary>
        /// Collapses the given list of processors by adding this one to it.
        /// </summary>
        public override int Collapse(List<ProcessorBase> processors, int i)
        {
            if (processors == null) { throw new ArgumentNullException("processors"); }
            if (processors.Count == 0) { throw new ArgumentOutOfRangeException("processors", "There has to be at least on processor there to collapse this filter."); }
            if (processors[processors.Count - 1] == null) { throw new ArgumentOutOfRangeException("processors", "The last processor in the processors list is null."); }
            if (i < 1) { throw new ArgumentOutOfRangeException("i"); }

            // take the last processor and collapse.
            if (processors[i - 1] is ProcessorSource)
            { // ok, processor is a source.
                var source = processors[i - 1] as ProcessorSource;
                if (!source.IsReady) { throw new InvalidOperationException("Last processor before filter is a source but it is not ready."); }
                processors.RemoveAt(i - 1);

                _filter.RegisterSource(source.Source);
                _isReady = true;
                return -1;
            }
            throw new InvalidOperationException("Last processor before filter is not a source.");
        }

        /// <summary>
        /// Returns true if this processor is ready.
        /// </summary>
        public override bool IsReady
        {
            get { return _isReady; }
        }

        /// <summary>
        /// Executes the tasks or commands in this processor.
        /// </summary>
        public override void Execute()
        { // a filter cannot be executed.
            throw new InvalidOperationException("This processor cannot be executed, check CanExecute before calling this method.");
        }

        /// <summary>
        /// Returns true if this processor can be executed.
        /// </summary>
        public override bool CanExecute
        { // a filter cannot be executed.
            get { return false; }
        }
    }
}
