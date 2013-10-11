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
using System.ComponentModel;
using System.Linq;
using System.Text;
using OsmSharpDataProcessor.CommandLine;
using OsmSharp.Osm.Data.Streams;

namespace OsmSharpDataProcessor
{
    internal class Program
    {
        /// <summary>
        /// The main entry point of the application.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            OsmSharp.Logging.Log.RegisterConsoleListener();

            // parse commands first.
            Command[] commands = CommandParser.ParseCommands(args);

            // convert commands into data processors.
            if (commands == null || commands.Length < 2)
            {
                throw new Exception("Please specifiy a valid data processing command!");
            }

            // start from the final command, that should be a target.
            object processor = commands[commands.Length - 1].CreateProcessor();
            if (!(processor is OsmStreamTarget))
            {
                throw new InvalidCommandException(
                    string.Format("Last argument {0} does not present a data processing target!",
                                  commands[commands.Length - 1].ToString()));
            }
            // target is defined.
            var target = (processor as OsmStreamTarget);

            // get the second to last argument.
            processor = commands[commands.Length - 2].CreateProcessor();
            if (!(processor is OsmStreamSource))
            {
                throw new InvalidCommandException(
                    string.Format("Second last argument {0} does not present a data processing source or filter!",
                                  commands[commands.Length - 2].ToString()));
            }

            // two options.
            if (processor is OsmStreamFilter)
            {
                // there should be more filters or sources.
                var filter = (processor as OsmStreamFilter);
                target.RegisterSource(filter);

                int commandIdx = commands.Length - 3;
                while (commandIdx >= 0)
                {
                    processor = commands[commandIdx].CreateProcessor();

                    // check source/filter.
                    if (!(processor is OsmStreamSource))
                    {
                        throw new InvalidCommandException(
                            string.Format(
                                "Second last argument {0} does not present a data processing source or filter!",
                                commands[commands.Length - 2].ToString()));
                    }

                    if (processor is OsmStreamFilter)
                    {
                        // another filter!
                        var newFilter = (processor as OsmStreamFilter);
                        filter.RegisterSource(newFilter);
                        filter = newFilter;
                    }
                    else if (processor is OsmStreamSource)
                    {
                        // everything should end here!
                        var source = (processor as OsmStreamSource);
                        filter.RegisterSource(source);

                        if (commandIdx > 0)
                        {
                            throw new InvalidCommandException(
                                string.Format("Wrong order in filter/source specification!"));
                        }
                    }

                    // move to next command.
                    commandIdx--;
                }
            }
            else if (processor is OsmStreamSource)
            {
                // everything should end here!
                var source = (processor as OsmStreamSource);
                target.RegisterSource(source);

                if (commands.Length > 2)
                {
                    throw new InvalidCommandException(
                        string.Format("Wrong order in filter/source specification!"));
                }
            }

            // execute the command by pulling the data to the target.
            target.Pull();
        }
    }
}