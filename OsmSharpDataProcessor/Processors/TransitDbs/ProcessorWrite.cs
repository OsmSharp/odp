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
using GTFS.IO;
using OsmSharp.Routing.Transit.Data;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharpDataProcessor.Processors.TransitDbs
{
    /// <summary>
    /// A write transitdb processor.
    /// </summary>
    public class ProcessorWrite : ProcessorBase
    {
        /// <summary>
        /// Holds the file.
        /// </summary>
        private string _file;

        /// <summary>
        /// Creates a new write transitdb processor.
        /// </summary>
        public ProcessorWrite(string file)
        {
            _file = file;
        }

        private Func<TransitDb> _getTransitDb;

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
            if (processors[i - 1] is ITransitDbSource)
            { // ok, processor is a source.
                var source = processors[i - 1] as ITransitDbSource;
                processors.RemoveAt(i - 1);
                _getTransitDb = source.GetTransitDb();
                return -1;
            }
            throw new InvalidOperationException("Last processor before filter is not a source.");
        }
        
        /// <summary>
        /// Returns true if this writer is ready.
        /// </summary>
        public override bool IsReady
        {
            get { return _getTransitDb != null; }
        }

        /// <summary>
        /// Executes this processor.
        /// </summary>
        public override void Execute()
        {
            var transitDb = _getTransitDb();

            if(transitDb.ConnectionSorting == null)
            {
                throw new Exception("Transitdb connections need to be sorted before serialization.");
            }

            var fileInfo = new FileInfo(_file);
            OsmSharp.Logging.Log.TraceEvent("Processor - Read", OsmSharp.Logging.TraceEventType.Information,
                "Writing to {0}...", fileInfo.Name);

            transitDb.Serialize(File.OpenWrite(_file));
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