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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsmSharpDataProcessor.Commands.Processors.GTFS
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcessorWrite : ProcessorBase
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

        private Func<GTFSFeed> _getFeed;

        /// <summary>
        /// Collapses this processor if possible.
        /// </summary>
        public override void Collapse(List<ProcessorBase> processors)
        {
            if (processors == null) { throw new ArgumentNullException("processors"); }
            if (processors.Count == 0) { throw new ArgumentOutOfRangeException("processors", "There has to be at least on processor there to collapse this target."); }
            if (processors[processors.Count - 1] == null) { throw new ArgumentOutOfRangeException("processors", "The last processor in the processors list is null."); }

            // take the last processor and collapse.
            if (processors[processors.Count - 1] is IGTFSSource)
            { // ok, processor is a source.
                var source = processors[processors.Count - 1] as IGTFSSource;
                processors.RemoveAt(processors.Count - 1);

                this.Source = source;
                processors.Add(this);
                return;
            }
            throw new InvalidOperationException("Last processor before filter is not a source.");
        }

        /// <summary>
        /// Gets or sets the source for this writer.
        /// </summary>
        public IGTFSSource Source { get; set; }

        /// <summary>
        /// Returns true if this writer is ready.
        /// </summary>
        public override bool IsReady
        {
            get { return this.Source != null; }
        }

        /// <summary>
        /// Executes this processor.
        /// </summary>
        public override void Execute()
        {
            // read feed.
            var feed = Source.GetFeed()();

            // create directory if needed.
            var directory = new DirectoryInfo(_path);
            if (!directory.Exists)
            {
                directory.Create();
            }

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
