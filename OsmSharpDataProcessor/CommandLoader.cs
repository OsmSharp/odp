using OsmSharpDataProcessor.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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
            commands.Add(new CommandFilterMerge());
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

            return commands;
        }
    }
}
