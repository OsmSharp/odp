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

using OsmSharp.Geo.Geometries;
using OsmSharp.Routing.Osm.Vehicles;
using OsmSharp.Routing.Profiles;
using OsmSharpDataProcessor.Commands.Processors;
using Reminiscence.IO;
using System.IO;

namespace OsmSharpDataProcessor.Commands
{
    /// <summary>
    /// The graph-write command.
    /// </summary>
    public class CommandCreateRouterDb : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSwitch()
        {
            return new string[] { "--create-routerdb" };
        }

        /// <summary>
        /// Gets or sets the vehicles.
        /// </summary>
        public OsmSharp.Routing.Osm.Vehicles.Vehicle[] Vehicles { get; set; }

        /// <summary>
        /// Gets or sets the contraction profiles.
        /// </summary>
        public OsmSharp.Routing.Profiles.Profile[] ContractionProfiles { get; set; }

        /// <summary>
        /// Gets or sets the memory-map file.
        /// </summary>
        public string MemoryMapFile { get; set; }

        /// <summary>
        /// Gets or sets the all core flag.
        /// </summary>
        public bool AllCore { get; set; }

        /// <summary>
        /// Gets or sets the keep way id's flag.
        /// </summary>
        public bool KeepWayIds { get; set; }

        /// <summary>
        /// Gets or sets the poly file.
        /// </summary>
        public string Poly { get; set; }

        /// <summary>
        /// Parse the command arguments for the command.
        /// </summary>
        public override int Parse(string[] args, int idx, out Command command)
        {
            var commandWriteGraph = new CommandCreateRouterDb();

            // check next argument.
            if (args.Length < idx)
            {
                throw new CommandLineParserException("None", "Invalid arguments for --create-routerdb!");
            }

            // set default vehicle to car.
            commandWriteGraph.Vehicles = new OsmSharp.Routing.Osm.Vehicles.Vehicle[] 
            {
                Vehicle.Car
            };
            commandWriteGraph.AllCore = false;

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
                        case "vehicles":
                            string[] vehicleValues;
                            if (CommandParser.SplitValuesArray(keyValue[1].ToLower(), out vehicleValues))
                            { // split the values array.
                                var vehicles = new System.Collections.Generic.List<Vehicle>(vehicleValues.Length);
                                for (int i = 0; i < vehicleValues.Length; i++)
                                {
                                    Vehicle vehicle;
                                    if (!Vehicle.TryGetByUniqueName(vehicleValues[i], out vehicle))
                                    {
                                        if (vehicleValues[i] == "all")
                                        { // all vehicles.
                                            vehicles.Add(Vehicle.Bicycle);
                                            vehicles.Add(Vehicle.BigTruck);
                                            vehicles.Add(Vehicle.Bus);
                                            vehicles.Add(Vehicle.Car);
                                            vehicles.Add(Vehicle.Moped);
                                            vehicles.Add(Vehicle.MotorCycle);
                                            vehicles.Add(Vehicle.Pedestrian);
                                            vehicles.Add(Vehicle.SmallTruck);
                                        }
                                        else if (vehicleValues[i] == "motorvehicle" ||
                                            vehicleValues[i] == "motorvehicles")
                                        { // all motor vehicles.
                                            vehicles.Add(Vehicle.BigTruck);
                                            vehicles.Add(Vehicle.Bus);
                                            vehicles.Add(Vehicle.Car);
                                            vehicles.Add(Vehicle.MotorCycle);
                                            vehicles.Add(Vehicle.SmallTruck);
                                        }
                                        else
                                        {
                                            throw new CommandLineParserException("--create-routerdb",
                                                string.Format("Invalid parameter value for command --create-routerdb: Vehicle profile '{0}' not found.",
                                                    vehicleValues[i]));
                                        }
                                    }
                                    else
                                    {
                                        vehicles.Add(vehicle);
                                    }
                                }
                                commandWriteGraph.Vehicles = vehicles.ToArray();
                            }
                            break;
                        case "contract":
                            string[] contractionProfileValues;
                            if (CommandParser.SplitValuesArray(keyValue[1].ToLower(), out contractionProfileValues))
                            { // split the values array.
                                var profiles = new Profile[contractionProfileValues.Length];
                                for (int i = 0; i < contractionProfileValues.Length; i++)
                                {
                                    Profile profile;
                                    if (!Profile.TryGet(contractionProfileValues[i], out profile))
                                    {
                                        throw new CommandLineParserException("--create-routerdb",
                                            string.Format("Invalid parameter value for command --write-graph: Profile '{0}' not found.",
                                                contractionProfileValues[i]));
                                    }
                                    profiles[i] = profile;
                                }
                                commandWriteGraph.ContractionProfiles = profiles;
                            }
                            break;
                        case "map":
                            commandWriteGraph.MemoryMapFile = keyValue[1];
                            break;
                        case "poly":
                            commandWriteGraph.Poly = keyValue[1];
                            break;
                        case "allcore":
                            if (!string.IsNullOrWhiteSpace(keyValue[1]) &&
                                 (keyValue[1].ToLowerInvariant() == "yes" ||
                                  keyValue[1].ToLowerInvariant() == "true"))
                            {
                                commandWriteGraph.AllCore = true;
                            }
                            break;
                        case "keepwayids":
                            if (!string.IsNullOrWhiteSpace(keyValue[1]) &&
                                 (keyValue[1].ToLowerInvariant() == "yes" ||
                                  keyValue[1].ToLowerInvariant() == "true"))
                            {
                                commandWriteGraph.KeepWayIds = true;
                            }
                            break;
                        default:
                            // the command splitting succeed but one of the arguments is unknown.
                            throw new CommandLineParserException("--create-routerdb",
                                string.Format("Invalid parameter for command --create-routerdb: {0} not recognized.", keyValue[0]));
                    }
                }
                else
                { // the command splitting failed and this is not a switch.
                    throw new CommandLineParserException("--create-routerdb", "Invalid parameter for command --create-routerdb.");
                }

                idx++; // increase the index.
            }

            // everything ok, take the next argument as the filename.
            command = commandWriteGraph;
            return idx - startIdx;
        }

        /// <summary>
        /// Creates the stream processor associated with this command.
        /// </summary>
        /// <returns></returns>
        public override ProcessorBase CreateProcessor()
        {
            try
            {
                LineairRing ring = null;
                if(!string.IsNullOrWhiteSpace(this.Poly))
                { // poly file stream.
                    var polyFile = new FileInfo(this.Poly);
                    if(!polyFile.Exists)
                    {
                        throw new FileNotFoundException("Poly file not found.", polyFile.FullName);
                    }
                    using (var polyFileStream = polyFile.OpenRead())
                    {
                        var poly = OsmSharp.Geo.Streams.Poly.PolyFileConverter.ReadPolygon(polyFileStream);
                        if (!(poly.Geometry is LineairRing) &&
                           !(poly.Geometry is Polygon))
                        { // oeps, no ring or polygon found.
                            throw new System.Exception("Could not find a valid polygon to filter on based on poly file.");
                        }
                        if (poly.Geometry is LineairRing)
                        {
                            ring = poly.Geometry as LineairRing;
                        }
                        else
                        {
                            var polygon = poly.Geometry as Polygon;
                            ring = polygon.Ring;
                        }
                    }
                }

                // create memory mappped stream if option is there.
                MemoryMap map = null;
                if (!string.IsNullOrWhiteSpace(this.MemoryMapFile))
                {
                    map = new MemoryMapStream((new FileInfo(this.MemoryMapFile)).Open(FileMode.Create));
                    return new ProcessorCreateRouterDb(map, this.Vehicles, this.ContractionProfiles, this.AllCore, this.KeepWayIds, ring);
                }
                return new ProcessorCreateRouterDb(this.Vehicles, this.ContractionProfiles, this.AllCore, this.KeepWayIds, ring);
            }
            catch
            {
                throw new InvalidCommandException("Invalid command: " + this.ToString());
            }
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        public override string ToString()
        {
            var vehicles = string.Empty;
            if(this.Vehicles != null &&
               this.Vehicles.Length > 0)
            {
                vehicles = this.Vehicles[0].UniqueName;
                for(var i = 1; i < this.Vehicles.Length; i++)
                {
                    vehicles += "," + this.Vehicles[i].UniqueName;
                }
            }
            var profiles = string.Empty;
            if (this.ContractionProfiles != null &&
               this.ContractionProfiles.Length > 0)
            {
                profiles = this.ContractionProfiles[0].Name;
                for (var i = 1; i < this.ContractionProfiles.Length; i++)
                {
                    profiles += "," + this.ContractionProfiles[i].Name;
                }
            }
            var result = "--create-routerdb";
            if (!string.IsNullOrEmpty(vehicles))
            {
                result += " vehicles=" + vehicles;
            }
            if (!string.IsNullOrEmpty(profiles))
            {
                result += " contract=" + profiles;
            }
            return result;
        }
    }

    /// <summary>
    /// Graph type.
    /// </summary>
    public enum GraphType
    {
        /// <summary>
        /// Regular graph definition.
        /// </summary>
        Regular,
        /// <summary>
        /// Contracted graph definition.
        /// </summary>
        Contracted
    }

    /// <summary>
    /// Format type.
    /// </summary>
    public enum FormatType
    {
        /// <summary>
        /// The flat-file format.
        /// </summary>
        Flat,
        /// <summary>
        /// The tiled format.
        /// </summary>
        Tiled,
        /// <summary>
        /// The mobile format.
        /// </summary>
        Mobile
    }
}