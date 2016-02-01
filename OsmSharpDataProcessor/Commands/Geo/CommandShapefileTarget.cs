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

using OsmSharpDataProcessor.Processors;
using System.IO;

namespace OsmSharpDataProcessor.Commands.Geo
{
    /// <summary>
    /// A command to write a shapefile.
    /// </summary>
    class CommandShapefileTarget : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSwitch()
        {
            return new string[] { "--write-shape" };
        }

        /// <summary>
        /// The file to path to.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Parse the command arguments for the write-shape command.
        /// </summary>
        public override int Parse(string[] args, int idx, out Command command)
        {
            // check next argument.
            if (args.Length < idx)
            {
                throw new CommandLineParserException("None", "Invalid file name for write-shape command!");
            }

            // everything ok, take the next argument as the filename.
            command = new CommandShapefileTarget()
            {
                Path = args[idx]
            };
            return 1;
        }

        /// <summary>
        /// Creates a processor that corresponds to this command.
        /// </summary>
        /// <returns></returns>
        public override ProcessorBase CreateProcessor()
        {
            return new Processors.Geo.ProcessorShapeTarget(
                this.Path);
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("--write-shape {0}", this.Path);
        }
    }
}
