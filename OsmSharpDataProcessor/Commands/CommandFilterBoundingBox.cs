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
using System.Globalization;
using System.Linq;
using System.Text;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Streams.Filters;
using OsmSharpDataProcessor.Commands.Processors;

namespace OsmSharpDataProcessor.Commands
{
    /// <summary>
    /// A filter command filtering a boundingbox of data.
    /// </summary>
    public class CommandFilterBoundingBox : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSwitch()
        {
            return new string[] { "--bb", "--bounding-box" };
        }

        /// <summary>
        /// The right bound.
        /// </summary>
        public float Top { get; set; }

        /// <summary>
        /// The top bound.
        /// </summary>
        public float Bottom { get; set; }

        /// <summary>
        /// The left bound.
        /// </summary>
        public float Left { get; set; }

        /// <summary>
        /// The right bound.
        /// </summary>
        public float Right { get; set; }

        /// <summary>
        /// Parse the command arguments for the bounding-box command.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="idx"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public override int Parse(string[] args, int idx, out Command command)
        {
            // check next argument.
            if (args.Length < idx + 3)
            {
                throw new CommandLineParserException("None", "Invalid bounding-box command!");
            }

            bool topOk = false, bottomOk = false, leftOk = false, rightOk = false;
            float top = 0, bottom = 0, left = 0, right = 0;
            for (int currentArg = idx; currentArg < idx + 4; currentArg++)
            {
                string[] argSplit = args[currentArg].Split('=');

                if (argSplit.Length != 2 ||
                    argSplit[0] == null ||
                    argSplit[1] == null)
                {
                    throw new CommandLineParserException(args[currentArg],
                                                         "Invalid boundary condition for boundingbox command!");
                }

                argSplit[0] = argSplit[0].ToLower();
                argSplit[0] = CommandParser.RemoveQuotes(argSplit[0]);
                argSplit[1] = CommandParser.RemoveQuotes(argSplit[1]);
                if (argSplit[0] == "top")
                {
                    if (
                        !float.TryParse(argSplit[1], NumberStyles.Float,
                                        System.Globalization.CultureInfo.InvariantCulture, out top))
                    {
                        throw new CommandLineParserException(args[currentArg],
                                                             "Invalid boundary condition for boundingbox command!");
                    }
                    topOk = true;
                }
                else if (argSplit[0] == "left")
                {
                    if (
                        !float.TryParse(argSplit[1], NumberStyles.Float,
                                        System.Globalization.CultureInfo.InvariantCulture, out left))
                    {
                        throw new CommandLineParserException(args[currentArg],
                                                             "Invalid boundary condition for boundingbox command!");
                    }
                    leftOk = true;
                }
                else if (argSplit[0] == "bottom")
                {
                    if (
                        !float.TryParse(argSplit[1], NumberStyles.Float,
                                        System.Globalization.CultureInfo.InvariantCulture, out bottom))
                    {
                        throw new CommandLineParserException(args[currentArg],
                                                             "Invalid boundary condition for boundingbox command!");
                    }
                    bottomOk = true;
                }
                else if (argSplit[0] == "right")
                {
                    if (
                        !float.TryParse(argSplit[1], NumberStyles.Float,
                                        System.Globalization.CultureInfo.InvariantCulture, out right))
                    {
                        throw new CommandLineParserException(args[currentArg],
                                                             "Invalid boundary condition for boundingbox command!");
                    }
                    rightOk = true;
                }
                else
                {
                    throw new CommandLineParserException(args[currentArg],
                                                         "Invalid boundary condition for boundingbox command!");
                }
            }

            if (!(bottomOk && topOk && leftOk && rightOk))
            {
                throw new CommandLineParserException("None",
                                                     "Invalid bounding-box command, at least one of the boundaries is missing!");
            }

            // everything ok, take the next argument as the filename.
            command = new CommandFilterBoundingBox()
                          {
                              Top = top,
                              Bottom = bottom,
                              Left = left,
                              Right = right
                          };
            return 4;
        }

        /// <summary>
        /// Creates the processor that belongs to this data.
        /// </summary>
        /// <returns></returns>
        public override ProcessorBase CreateProcessor()
        {
            return new ProcessorFilter(new OsmStreamFilterBoundingBox(
                new GeoCoordinateBox(
                    new GeoCoordinate(this.Top, this.Left),
                    new GeoCoordinate(this.Bottom, this.Right))));
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("--bounding-box top={0} left={1} right={2} bottom={3}",
                                 this.Top.ToString(System.Globalization.CultureInfo.InvariantCulture),
                                 this.Left.ToString(System.Globalization.CultureInfo.InvariantCulture),
                                 this.Right.ToString(System.Globalization.CultureInfo.InvariantCulture),
                                 this.Bottom.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
