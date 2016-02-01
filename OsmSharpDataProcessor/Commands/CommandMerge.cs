// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using OsmSharpDataProcessor.Streams;
using OsmSharpDataProcessor.Processors;

namespace OsmSharpDataProcessor.Commands
{
    /// <summary>
    /// A merge filter command.
    /// </summary>
    public class CommandMerge : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        public override string[] GetSwitch()
        {
            return new string[] { "--m", "--merge" };
        }

        /// <summary>
        /// Parses the command line arguments for the merge command.
        /// </summary>
        public override int Parse(string[] args, int idx, out Command command)
        {
            // everything ok, there are no arguments.
            command = new CommandMerge();
            return 0;
        }

        /// <summary>
        /// Returns the processor that corresponds to this filter.
        /// </summary>
        public override ProcessorBase CreateProcessor()
        {
            return new ProcessorMerge();
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        public override string ToString()
        {
            return string.Format("--merge");
        }
    }
}
