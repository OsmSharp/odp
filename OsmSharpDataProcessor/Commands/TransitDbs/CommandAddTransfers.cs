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

using OsmSharp;
using OsmSharpDataProcessor.Processors;

namespace OsmSharpDataProcessor.Commands.TransitDbs
{
    /// <summary>
    /// A add transfers command.
    /// </summary>
    public class CommandAddTransfers : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSwitch()
        {
            return new string[] { "--add-transfers" };
        }

        /// <summary>
        /// The profile to add transfers for.
        /// </summary>
        public string Profile { get; set; }

        /// <summary>
        /// The # seconds of allowed travel.
        /// </summary>
        public float Seconds { get; set; }

        /// <summary>
        /// Parse the command arguments.
        /// </summary>
        public override int Parse(string[] args, int idx, out Command command)
        {
            var commandAddTransfers = new CommandAddTransfers();

            // check next argument.
            if (args.Length < idx)
            {
                throw new CommandLineParserException("None", "Invalid arguments for --add-transfers!");
            }

            // set default vehicle to car.
            commandAddTransfers = new CommandAddTransfers()
            {
                Seconds = 3 * 60 // default 15 mins.
            };

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
                        case "profile":
                            commandAddTransfers.Profile = keyValue[1];
                            break;
                        case "time":
                            float time;
                            if(!float.TryParse(keyValue[1], out time))
                            {
                                throw new CommandLineParserException("--add-transfers",
                                    string.Format("Invalid parameter for command --add-transfers: Could not parse value for {0}.", keyValue[0]));
                            }
                            commandAddTransfers.Seconds = time;
                            break;
                        default:
                            // the command splitting succeed but one of the arguments is unknown.
                            throw new CommandLineParserException("--add-transfers",
                                string.Format("Invalid parameter for command --add-transfers: {0} not recognized.", keyValue[0]));
                    }
                }
                else
                { // the command splitting failed and this is not a switch.
                    throw new CommandLineParserException("--add-transfers", "Invalid parameter for command --add-transfers.");
                }

                idx++; // increase the index.
            }

            // everything ok, take the next argument as the filename.
            command = commandAddTransfers;
            return idx - startIdx;
        }

        /// <summary>
        /// Creates the processor that corresponds to this command.
        /// </summary>
        public override ProcessorBase CreateProcessor()
        {
            OsmSharp.Routing.Profiles.Profile profile;
            if (!OsmSharp.Routing.Profiles.Profile.TryGet(this.Profile, out profile))
            {
                throw new CommandLineParserException("--add-transfers",
                    string.Format("Invalid parameter value for command --add-transfers: Profile '{0}' not found.",
                        this.Profile));
            }

            return new Processors.TransitDbs.ProcessorAddTransfers(
                profile, this.Seconds);
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        public override string ToString()
        {
            return string.Format("--add-transfers profile={0} time={1}", this.Profile, this.Seconds.ToInvariantString());
        }
    }
}