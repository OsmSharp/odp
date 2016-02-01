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

namespace OsmSharpDataProcessor.Commands.GTFS
{
    /// <summary>
    /// A command to create a transit db.
    /// </summary>
    public class CommandCreateTransitDb : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSwitch()
        {
            return new string[] { "--create-transitdb" };
        }

        /// <summary>
        /// Parse the command arguments for the command.
        /// </summary>
        public override int Parse(string[] args, int idx, out Command command)
        {
            command = new CommandCreateTransitDb();
            return 0;
        }

        /// <summary>
        /// Creates the stream processor associated with this command.
        /// </summary>
        /// <returns></returns>
        public override ProcessorBase CreateProcessor()
        {
            return new Processors.GTFS.ProcessorCreateTransitDb();
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        public override string ToString()
        {
            return string.Format("--create-transitdb");
        }
    }
}