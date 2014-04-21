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
    /// Base class for all possible commands.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Gets the switches for this command.
        /// </summary>
        /// <returns></returns>
        public abstract string[] GetSwitch();

        /// <summary>
        /// Creates a new processor that corresponds with the action in this command.
        /// </summary>
        /// <returns></returns>
        public abstract object CreateProcessor();

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        /// <remarks>Forces all implementations of Command to implement a description.</remarks>
        /// <returns></returns>
        public abstract override string ToString();

        /// <summary>
        /// Parses string arguments into an actual command object.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="idx"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public abstract int Parse(string[] args, int idx, out Command command);

        /// <summary>
        /// Returns true if the given switch points to this command.
        /// </summary>
        /// <param name="switchString"></param>
        /// <returns></returns>
        public virtual bool HasSwitch(string switchString)
        {
            return this.GetSwitch().Contains(switchString);
        }
    }
}
