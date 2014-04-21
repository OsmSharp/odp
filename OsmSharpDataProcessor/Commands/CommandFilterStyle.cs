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
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.UI.Map.Styles.Streams;
using System.IO;
using OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain;
using OsmSharp.UI.Map.Styles.MapCSS;

namespace OsmSharpDataProcessor.Commands
{
    /// <summary>
    /// A style filter command.
    /// </summary>
    public class CommandFilterStyle : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSwitch()
        {
            return new string[] { "-fs", "--filter-style" };
        }

        /// <summary>
        /// Gets or sets the style type.
        /// </summary>
        public StyleType StyleType { get; set; }

        /// <summary>
        /// The style file.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Parses the command line arguments for the sort command.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="idx"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public override int Parse(string[] args, int idx, out Command command)
        {
            CommandFilterStyle commandFilterStyle = new CommandFilterStyle();
            // check next argument.
            if (args.Length < idx)
            {
                throw new CommandLineParserException("None", "Invalid arguments for --write-scene!");
            }

            // parse arguments and keep parsing until the next switch.
            int startIdx = idx;
            while (args.Length > idx &&
                !CommandParser.IsSwitch(args[idx]))
            {
                string[] keyValue;
                if (CommandParser.SplitKeyValue(args[idx], out keyValue))
                { // the command splitting succeeded.
                    keyValue[0] = CommandParser.RemoveQuotes(keyValue[0]);
                    keyValue[1] = CommandParser.RemoveQuotes(keyValue[1]);
                    switch (keyValue[0].ToLower())
                    {
                        case "file":
                            commandFilterStyle.File = keyValue[1];
                            break;
                        case "type":
                            string typeValue = keyValue[1].ToLower();
                            switch (typeValue)
                            {
                                case "mapcss":
                                    commandFilterStyle.StyleType = StyleType.MapCSS;
                                    break;
                            }
                            break;
                        default:
                            // the command splitting succeed but one of the arguments is unknown.
                            throw new CommandLineParserException("--filter-style",
                                string.Format("Invalid parameter for command --filter-style: {0} not recognized.", keyValue[0]));

                    }
                }
                else
                { // the command splitting failed and this is not a switch.
                    throw new CommandLineParserException("--write-scene", "Invalid parameter for command --write-scene.");
                }

                idx++; // increase the index.
            }
            command = commandFilterStyle;
            return 2;
        }

        /// <summary>
        /// Returns the processor that corresponds to this filter.
        /// </summary>
        /// <returns></returns>
        public override object CreateProcessor()
        {
            // mapCSS stream.
            Stream mapCSSStream = (new FileInfo(this.File)).OpenRead();
            MapCSSFile mapCSSFile = MapCSSFile.FromStream(mapCSSStream);

            return new StyleOsmStreamFilter(
                new MapCSSInterpreter(mapCSSFile, new MapCSSDictionaryImageSource()));
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("--filter-style type={0} file={1}",
                this.StyleType.ToString().ToLower(), this.File);
        }
    }

    /// <summary>
    /// Scene type.
    /// </summary>
    public enum StyleType
    {
        /// <summary>
        /// A MapCSS style.
        /// </summary>
        MapCSS
    }
}