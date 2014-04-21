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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharpDataProcessor.Commands
{
    /// <summary>
    /// A command line parser exception: thrown when commands cannot be parsed properly.
    /// </summary>
    public class CommandLineParserException : Exception
    {
        /// <summary>
        /// Holds the command.
        /// </summary>
        private readonly string _command;

        /// <summary>
        /// Creates a new command line parser exception.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="message"></param>
        public CommandLineParserException(string command, string message)
            :base(message)
        {
            _command = command;
        }
    }
}
