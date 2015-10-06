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
            commands.Add(new CommandWriteGraph());
            commands.Add(new CommandWriteScene());
            commands.Add(new CommandWriteXml());
            commands.Add(new CommandWritePBF());
            commands.Add(new CommandFilterPoly());

            // use reflection to load others.
            var executingAssembly = Assembly.GetExecutingAssembly();
            var executingLocation = new FileInfo(executingAssembly.Location).DirectoryName;
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach(var file in new DirectoryInfo(executingLocation).GetFiles("*.dll"))
            {
                try
                {
                    if (!loadedAssemblies.Any(x => x.Location == file.FullName))
                    { // assembly not loaded yet.
                        var assembly = Assembly.LoadFrom(file.FullName);
                        commands.AddRange(from t in assembly.GetTypes()
                                          where t.IsSubclassOf(typeof(Command))
                                          select (Command)Activator.CreateInstance(t));
                    }
                }
                catch (ReflectionTypeLoadException ex)
                { // retrow load exception.
                    StringBuilder builder = new StringBuilder();
                    if(ex.LoaderExceptions != null)
                    {
                        foreach(var loadedEx in ex.LoaderExceptions)
                        {
                            builder.Append(loadedEx.ToString());
                            builder.Append(Environment.NewLine);
                        }
                    }
                    OsmSharp.Logging.Log.TraceEvent("CommandLoaded", OsmSharp.Logging.TraceEventType.Warning, "Failed to load Assembly: {0} with exceptions {1}",
                        file, builder.ToString());
                }
            }

            return commands;
        }
    }
}
