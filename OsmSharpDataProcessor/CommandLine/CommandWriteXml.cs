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
using OsmSharp.Osm.Xml.Streams;

namespace OsmSharpDataProcessor.CommandLine
{
    /// <summary>
    /// The write-xml command.
    /// </summary>
    public class CommandWriteXml : Command
    {
        /// <summary>
        /// The file to write to.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Parse the command arguments for the write-xml command.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="idx"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static int Parse(string[] args, int idx, out Command command)
        {
            // check next argument.
            if (args.Length < idx)
            {
                throw new CommandLineParserException("None", "Invalid file name for write-xml command!");
            }

            // everything ok, take the next argument as the filename.
            command = new CommandWriteXml()
                          {
                              File = args[idx]
                          };
            return 1;
        }

        /// <summary>
        /// Creates a processor that corresponds to this command.
        /// </summary>
        /// <returns></returns>
        public override object CreateProcessor()
        {
            return new XmlOsmStreamTarget(this.File);
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("--write-xml {0}", this.File);
        }
    }
}
