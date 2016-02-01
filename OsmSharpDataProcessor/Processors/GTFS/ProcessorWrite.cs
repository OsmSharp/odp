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
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharpDataProcessor.Processors.GTFS
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcessorWrite : ProcessorBase, IGTFSTarget
    {
        /// <summary>
        /// Holds the path.
        /// </summary>
        private string _path;

        /// <summary>
        /// Creates a new write feed processor.
        /// </summary>
        public ProcessorWrite(string path)
        {
            _path = path;
        }

        /// <summary>
        /// Collapses the list of processors by trying to collapse this one.
        /// </summary>
        public override int Collapse(List<ProcessorBase> processors, int i)
        {
            if (processors == null) { throw new ArgumentNullException("processors"); }
            if (processors.Count == 0) { throw new ArgumentOutOfRangeException("processors", "There has to be at least on processor there to collapse this target."); }
            if (processors[processors.Count - 1] == null) { throw new ArgumentOutOfRangeException("processors", "The last processor in the processors list is null."); }
            if (i < 1) { throw new ArgumentOutOfRangeException("i"); }

            // take the last processor and collapse.
            if (processors[i - 1] is IGTFSSource)
            { // ok, processor is a source.
                var source = processors[i - 1] as IGTFSSource;
                processors.RemoveAt(i - 1);

                this.GetSourceFeed = source.GetFeed();
                return -1;
            }
            throw new InvalidOperationException("Last processor before filter is not a source.");
        }

        /// <summary>
        /// Gets or sets the source for this writer.
        /// </summary>
        public Func<IGTFSFeed> GetSourceFeed { get; set; }

        /// <summary>
        /// Returns true if this writer is ready.
        /// </summary>
        public override bool IsReady
        {
            get { return this.GetSourceFeed != null; }
        }

        /// <summary>
        /// Executes this processor.
        /// </summary>
        public override void Execute()
        {
            // read feed.
            var feed = GetSourceFeed();

            // create directory if needed.
            var directory = new DirectoryInfo(_path);
            if (!directory.Exists)
            {
                directory.Create();
            }

            OsmSharp.Logging.Log.TraceEvent("Processor - Write", OsmSharp.Logging.TraceEventType.Information,
                "Writing GTFS to {0}...", _path);

            // write feed.
            var feedWriter = new GTFSWriter<IGTFSFeed>();
            feedWriter.Write(feed, new GTFSDirectoryTarget(directory));
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
