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

using OsmSharpDataProcessor.Commands.Processors;
using OsmSharp.Routing.Profiles;
using System.IO;

namespace OsmSharpDataProcessor.Commands.RouterDbs
{
    /// <summary>
    /// A router db optimization command.
    /// </summary>
    public class CommandOptimize : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSwitch()
        {
            return new string[] { "--optimize" };
        }

        /// <summary>
        /// Parse the command arguments for the read-pbf command.
        /// </summary>
        public override int Parse(string[] args, int idx, out Command command)
        {
            // check next argument.
            if (args.Length < idx)
            {
                throw new CommandLineParserException("None", "Invalid file name for contract command!");
            }

            // everything ok, take the next argument as the filename.
            command = new RouterDbs.CommandOptimize(); ;
            return 0;
        }

        /// <summary>
        /// Creates the processor that corresponds to this command.
        /// </summary>
        public override ProcessorBase CreateProcessor()
        {
            return new Processors.RouterDbs.RouterDbProcessorOptimizer();
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("--optimize");
        }
    }
}