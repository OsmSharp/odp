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

using OsmSharp.Routing.Transit.Data;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharpDataProcessor.Processors.MultimodalDbs
{
    /// <summary>
    /// A write multimodaldb processor.
    /// </summary>
    public class ProcessorWrite : ProcessorBase
    {
        /// <summary>
        /// Holds the file.
        /// </summary>
        private string _file;

        /// <summary>
        /// Creates a new write multimodaldb processor.
        /// </summary>
        public ProcessorWrite(string file)
        {
            _file = file;
        }

        private Func<MultimodalDb> _getMultimodalDb;

        /// <summary>
        /// Collapses this processor if possible.
        /// </summary>
        public override int Collapse(List<ProcessorBase> processors, int i)
        {
            if (processors == null) { throw new ArgumentNullException("processors"); }
            if (processors.Count == 0) { throw new ArgumentOutOfRangeException("processors", "There has to be at least on processor there to collapse this target."); }
            if (processors[processors.Count - 1] == null) { throw new ArgumentOutOfRangeException("processors", "The last processor in the processors list is null."); }
            if (i < 1) { throw new ArgumentOutOfRangeException("i"); }

            // take the last processor and collapse.
            if (processors[i - 1] is IMultimodalDbSource)
            { // ok, processor is a source.
                var source = processors[i - 1] as IMultimodalDbSource;
                processors.RemoveAt(i - 1);
                _getMultimodalDb = source.GetMultimodalDb();
                return -1;
            }
            throw new InvalidOperationException("Last processor before filter is not a source.");
        }
        
        /// <summary>
        /// Returns true if this writer is ready.
        /// </summary>
        public override bool IsReady
        {
            get { return _getMultimodalDb != null; }
        }

        /// <summary>
        /// Executes this processor.
        /// </summary>
        public override void Execute()
        {
            var multimodalDb = _getMultimodalDb();
    
            multimodalDb.Serialize(File.OpenWrite(_file));
        }

        /// <summary>
        /// Returns true if this processor can execute.
        /// </summary>
        public override bool CanExecute
        {
            get { return this.IsReady; }
        }
    }
}
