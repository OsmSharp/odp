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
using System;
using System.IO;

namespace OsmSharpDataProcessor.Commands.RouterDbs
{
    /// <summary>
    /// A router db write contracted command.
    /// </summary>
    public class CommandWriteContracted : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        public override string[] GetSwitch()
        {
            return new string[] { "--write-contracted" };
        }

        /// <summary>
        /// The file to write to.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// The profile to write for.
        /// </summary>
        public string Profile { get; set; }

        /// <summary>
        /// Parse the command arguments for the command.
        /// </summary>
        public override int Parse(string[] args, int idx, out Command command)
        {
            var commandWriteContracted = new CommandWriteContracted();

            // check next argument.
            if (args.Length < idx)
            {
                throw new CommandLineParserException("None", "Invalid arguments for --write-contracted!");
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
                        case "profile":
                            commandWriteContracted.Profile = keyValue[1];
                            break;
                        case "file":
                            commandWriteContracted.File = keyValue[1];
                            break;
                        default:
                            // the command splitting succeed but one of the arguments is unknown.
                            throw new CommandLineParserException("--write-contracted",
                                string.Format("Invalid parameter for command --write-contracted: {0} not recognized.", keyValue[0]));
                    }
                }
                else
                { // the command splitting failed and this is not a switch.
                    throw new CommandLineParserException("--write-contracted", "Invalid parameter for command --write-contracted.");
                }

                idx++; // increase the index.
            }

            // everything ok
            command = commandWriteContracted;
            return idx - startIdx;
        }

        /// <summary>
        /// Creates a processor that corresponds to this command.
        /// </summary>
        /// <returns></returns>
        public override ProcessorBase CreateProcessor()
        {
            OsmSharp.Routing.Profiles.Profile profile;
            if(!OsmSharp.Routing.Profiles.Profile.TryGet(this.Profile, out profile))
            {
                throw new Exception(string.Format("Profile with name {0} could not be found.", this.Profile));
            }

            var outputFile = new FileInfo(this.File);
            if (outputFile.Exists)
            {
                return new Processors.RouterDbs.RouterDbProcessorTargetContracted(
                    outputFile.Open(FileMode.Truncate), outputFile.Name, profile);
            }
            return new Processors.RouterDbs.RouterDbProcessorTargetContracted(
                outputFile.Open(FileMode.Create), outputFile.Name, profile);
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("--write-contracted {0}", this.File);
        }
    }
}