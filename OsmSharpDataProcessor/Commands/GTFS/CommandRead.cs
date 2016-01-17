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

using OsmSharpDataProcessor.Commands.Processors;

namespace OsmSharpDataProcessor.Commands.GTFS
{
    /// <summary>
    /// A GTFS read command.
    /// </summary>
    public class CommandRead : Command
    {
        /// <summary>
        /// Gets the switches for this command.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSwitch()
        {
            return new string[] { "--read-gtfs" };
        }

        /// <summary>
        /// Gets or sets the path to read from.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Parse the command arguments for the read-gtfs command.
        /// </summary>
        /// <returns></returns>
        public override int Parse(string[] args, int idx, out Command command)
        {
            // check next argument.
            if (args.Length < idx)
            {
                throw new CommandLineParserException("None", "Invalid file name for read-gtfs command!");
            }

            // everything ok, take the next argument as the filename.
            command = new CommandRead()
            {
                Path = args[idx]
            };
            return 1;
        }

        /// <summary>
        /// Creates the processor that corresponds to this command.
        /// </summary>
        /// <returns></returns>
        public override ProcessorBase CreateProcessor()
        {
            return new Processors.GTFS.ProcessorRead(this.Path);
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("--read-gtfs {0}", this.Path);
        }
    }
}