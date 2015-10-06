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

using OsmSharp.Geo.Geometries;
using OsmSharpDataProcessor.Commands.Processors;
using System.IO;

namespace OsmSharpDataProcessor.Commands
{
    /// <summary>
    /// A poly filter command.
    /// </summary>
    public class CommandFilterPoly : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSwitch()
        {
            return new string[] { "--bp", "--bounding-polygon" };
        }

        /// <summary>
        /// The style file.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Parses the command line arguments for the filter command.
        /// </summary>
        /// <returns></returns>
        public override int Parse(string[] args, int idx, out Command command)
        {
            var commandFilterPoly = new CommandFilterPoly();
            // check next argument.
            if (args.Length < idx)
            {
                throw new CommandLineParserException("None", "Invalid arguments for --bounding-polygon!");
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
                            commandFilterPoly.File = keyValue[1];
                            break;
                        default:
                            // the command splitting succeed but one of the arguments is unknown.
                            throw new CommandLineParserException("--bounding-polygon",
                                string.Format("Invalid parameter for command --bounding-polygon: {0} not recognized.", keyValue[0]));

                    }
                }
                else
                { // the command splitting failed and this is not a switch.
                    throw new CommandLineParserException("--bounding-polygon", "Invalid parameter for command --bounding-polygon.");
                }

                idx++; // increase the index.
            }
            command = commandFilterPoly;
            return 1;
        }

        /// <summary>
        /// Returns the processor that corresponds to this filter.
        /// </summary>
        /// <returns></returns>
        public override ProcessorBase CreateProcessor()
        {
            // poly file stream.
            using(var polyFileStream = (new FileInfo(this.File)).OpenRead())
            {
                var poly = OsmSharp.Geo.Streams.Poly.PolyFileConverter.ReadPolygon(polyFileStream);
                if(!(poly.Geometry is LineairRing) &&
                   !(poly.Geometry is Polygon))
                { // oeps, no ring or polygon found.
                    throw new System.Exception("Could not find a valid polygon to filter on based on poly file.");
                }
                LineairRing ring = null;
                if (poly.Geometry is LineairRing)
                {
                    ring = poly.Geometry as LineairRing;
                }
                else
                {
                    var polygon = poly.Geometry as Polygon;
                    ring = polygon.Ring;
                }
                return new ProcessorFilter(new OsmSharp.Osm.Streams.Filters.OsmStreamFilterPoly(ring));
            }
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("--bounding-polygon file={0}",
                this.File);
        }
    }
}