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

using OsmSharpDataProcessor.Commands;
using System.Collections.Generic;

namespace OsmSharpDataProcessor
{
    /// <summary>
    /// Command loader class: loads all local commands and plugins.
    /// </summary>
    public class CommandLoader
    {
        /// <summary>
        /// Loads all commands, local and plugins.
        /// </summary>
        /// <returns></returns>
        public static List<Command> LoadCommands()
        {
            var commands = new List<Command>();
            commands.Add(new CommandFilterBoundingBox());
            commands.Add(new CommandMerge());
            commands.Add(new CommandFilterSort());
            commands.Add(new CommandFilterProgress());
            commands.Add(new CommandFilterStyle());
            commands.Add(new CommandReadPBF());
            commands.Add(new CommandReadXml());
            commands.Add(new CommandCreateRouterDb());
            commands.Add(new CommandWriteScene());
            commands.Add(new CommandWriteXml());
            commands.Add(new CommandWritePBF());
            commands.Add(new CommandFilterPoly());
            commands.Add(new CommandFilterGeoJson());
            commands.Add(new Commands.RouterDbs.CommandRead());
            commands.Add(new Commands.RouterDbs.CommandWrite());
            commands.Add(new Commands.RouterDbs.CommandContract());
            commands.Add(new Commands.RouterDbs.CommandWriteContracted());
            commands.Add(new Commands.RouterDbs.CommandMergeContracted());
            commands.Add(new Commands.RouterDbs.CommandWriteShape());
            commands.Add(new Commands.RouterDbs.CommandOptimize());
            commands.Add(new Commands.GTFS.CommandRead());
            commands.Add(new Commands.GTFS.CommandWrite());
            commands.Add(new Commands.GTFS.CommandCreateTransitDb());
            commands.Add(new Commands.CommandCreateMultimodalDb());
            commands.Add(new Commands.TransitDbs.CommandWrite());
            commands.Add(new Commands.TransitDbs.CommandRead());
            commands.Add(new Commands.TransitDbs.CommandAddTransfers());
            commands.Add(new Commands.MultimodalDbs.CommandWrite());
            commands.Add(new Commands.MultimodalDbs.CommandAddLinks());

            return commands;
        }
    }
}
