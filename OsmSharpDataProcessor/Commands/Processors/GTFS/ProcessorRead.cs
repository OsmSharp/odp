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

namespace OsmSharpDataProcessor.Commands.Processors.GTFS
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcessorRead : ProcessorBase, IGTFSSource
    {
        /// <summary>
        /// Holds the path.
        /// </summary>
        private string _path;

        /// <summary>
        /// Creates a new feed source.
        /// </summary>
        public ProcessorRead(string path)
        {
            _path = path;
        }

        /// <summary>
        /// Collapses the processors if possible.
        /// </summary>
        public override void Collapse(List<ProcessorBase> processors)
        {
            processors.Add(this);
        }

        /// <summary>
        /// Returns true if this reader is ready.
        /// </summary>
        public override bool IsReady
        {
            get { return true; }
        }

        /// <summary>
        /// Executes this processor.
        /// </summary>
        public override void Execute()
        {
            throw new InvalidOperationException("Cannot execute processor, check CanExecute.");
        }

        /// <summary>
        /// Returns true if this processor can be executed.
        /// </summary>
        public override bool CanExecute
        {
            get { return false; ; }
        }

        /// <summary>
        /// Returns the function to get the GTFS feed.
        /// </summary>
        /// <returns></returns>
        Func<GTFSFeed> IGTFSSource.GetFeed()
        {
            return () =>
            {
                // create the reader.
                var reader = new GTFSReader<GTFSFeed>(false);

                // execute the reader.
                return reader.Read(new GTFSDirectorySource(new DirectoryInfo(_path)));
            };
        }
    }
}
