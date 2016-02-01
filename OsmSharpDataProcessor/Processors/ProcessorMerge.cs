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

using OsmSharpDataProcessor.Processors.TransitDbs;
using OsmSharpDataProcessor.Streams;
using System;
using System.Collections.Generic;

namespace OsmSharpDataProcessor.Processors
{
    /// <summary>
    /// A processor created by a merged filter command.
    /// </summary>
    public class ProcessorMerge : ProcessorSource
    {
        /// <summary>
        /// Creates a new processor filter.
        /// </summary>
        public ProcessorMerge()
            : base(new MergedOsmStreamSource())
        {

        }

        private bool _isReady = false;

        /// <summary>
        /// Collapses the given list of processors by adding this one to it.
        /// </summary>
        public override int Collapse(List<ProcessorBase> processors, int i)
        {
            if (processors == null) { throw new ArgumentNullException("processors"); }
            if (processors.Count == 0) { throw new ArgumentOutOfRangeException("processors", "There has to be at least on processor there to collapse this filter."); }
            if (i < 1) { throw new ArgumentOutOfRangeException("i"); }

            var consumed = 0;
            if (processors[i - 1] is ProcessorSource)
            { // take all processors that are sources for this merge operation.
                while (i > 0 && processors[i - 1] is ProcessorSource)
                { // ok, processor is a source.
                    var source = processors[i - 1] as ProcessorSource;
                    if (!source.IsReady) { throw new InvalidOperationException("Last processor before filter is a source but it is not ready."); }
                    processors.RemoveAt(i - 1);

                    (this.Source as MergedOsmStreamSource).RegisterSource(source.Source);
                    _isReady = true;
                    i--;
                    consumed++;
                }
            }
            else if(processors[i - 1] is ITransitDbSource)
            { // take all processor that are transit db sources for this merge operation.
                var mergeProcessor = new Processors.TransitDbs.ProcessorMerge();
                while (i > 0 && processors[i - 1] is ITransitDbSource)
                { // ok, processor is a source.
                    var source = processors[i - 1] as ITransitDbSource;
                    processors.RemoveAt(i - 1);
                    
                    i--;
                    consumed++;
                }
            }
            if(consumed == 0)
            {
                throw new Exception("No sources consumed by merge operation.");
            }
            return -consumed;
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
