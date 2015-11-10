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

using OsmSharp.Osm.Streams;
using OsmSharp.Osm.Streams.Complete;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Routing;
using OsmSharpDataProcessor.Commands;
using OsmSharpDataProcessor.Commands.Processors;
using OsmSharpDataProcessor.Streams;
using System;
using System.Collections.Generic;

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
            // enable logging.
            OsmSharp.Logging.Log.Enable();
            OsmSharp.Logging.Log.RegisterListener(new global::OsmSharp.WinForms.UI.Logging.ConsoleTraceListener());

            // register OsmSharp vehicles.
            OsmSharp.Routing.Osm.Vehicles.Vehicle.RegisterVehicles();

            // parse commands first.
            var commands = CommandParser.ParseCommands(args);

            // convert commands into data processors.
            if (commands == null)
            {
                throw new Exception("Please specifiy a valid data processing command!");
            }

            var collapsedCommands = new List<ProcessorBase>();
            for(int idx = 0; idx < commands.Length; idx++)
            {
                var processor = commands[idx].CreateProcessor();
                processor.Collapse(collapsedCommands);
            }

            if(collapsedCommands.Count > 1)
            { // there is more than one command left.
                throw new Exception("Command list could not be interpreted. Make sure you have the correct source/filter/target combinations.");
            }

            if(collapsedCommands[0].CanExecute)
            { // execute the last remaining fully collapsed command.
                collapsedCommands[0].Execute();
            }
        }
    }
}