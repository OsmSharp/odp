// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

namespace OsmSharpDataProcessor.Commands.RouterDbs
{
    /// <summary>
    /// A router db write command.
    /// </summary>
    public class CommandWrite : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        public override string[] GetSwitch()
        {
            return new string[] { "--write-routerdb" };
        }

        /// <summary>
        /// The file to write to.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Parse the command arguments for the command.
        /// </summary>
        public override int Parse(string[] args, int idx, out Command command)
        {
            // check next argument.
            if (args.Length < idx)
            {
                throw new CommandLineParserException("None", "Invalid file name for write-routerdb command!");
            }

            // everything ok, take the next argument as the filename.
            command = new RouterDbs.CommandWrite()
            {
                File = args[idx]
            };
            return 1;
        }

        /// <summary>
        /// Creates a processor that corresponds to this command.
        /// </summary>
        /// <returns></returns>
        public override ProcessorBase CreateProcessor()
        {
            var outputFile = new FileInfo(this.File);
            if (outputFile.Exists)
            {
                return new Processors.RouterDbs.RouterDbProcessorTarget(
                    outputFile.Open(FileMode.Truncate), outputFile.Name);
            }
            return new Processors.RouterDbs.RouterDbProcessorTarget(
                outputFile.Open(FileMode.Create), outputFile.Name);
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("--write-routerdb {0}", this.File);
        }
    }
}
