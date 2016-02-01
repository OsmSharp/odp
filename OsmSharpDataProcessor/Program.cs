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

using OsmSharpDataProcessor.Processors;
using System;
using System.Collections.Generic;

namespace OsmSharpDataProcessor
{
    internal class Program
    {
        /// <summary>
        /// The main entry point of the application.
        /// </summary>
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

            // create processors.
            var processors = new List<ProcessorBase>();
            for (int i = 0; i < commands.Length; i++)
            {
                processors.Add(commands[i].CreateProcessor());
            }

            // collapse processors.
            int p = 0;
            while (p < processors.Count)
            {
                var consumed = processors[p].Collapse(processors, p);
                p = p + consumed + 1;
            }

            // check if all current processors can be executed.
            for (var i = 0; i < processors.Count; i++)
            {
                if (!processors[i].CanExecute)
                {
                    throw new Exception("Collapsing processors end in a non-executable processor, invalid processor sequence.");
                }
            }

            var performanceinfo = new PerformanceInfoConsumer("odp", 5000);
            performanceinfo.Start();
            for (var i = 0; i < processors.Count; i++)
            {
                processors[i].Execute();
            }
            performanceinfo.Stop();
        }
    }
}