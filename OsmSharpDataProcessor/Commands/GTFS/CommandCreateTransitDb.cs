using OsmSharpDataProcessor.Commands.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsmSharpDataProcessor.Commands.GTFS
{
    /// <summary>
    /// A command to create a transit db.
    /// </summary>
    public class CommandCreateTransitDb : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSwitch()
        {
            return new string[] { "--create-transitdb" };
        }

        /// <summary>
        /// Parse the command arguments for the command.
        /// </summary>
        public override int Parse(string[] args, int idx, out Command command)
        {
            command = new CommandCreateTransitDb();
            return 0;
        }

        /// <summary>
        /// Creates the stream processor associated with this command.
        /// </summary>
        /// <returns></returns>
        public override ProcessorBase CreateProcessor()
        {
            return new Processors.GTFS.ProcessorCreateTransitDb();
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        public override string ToString()
        {
            return string.Format("--create-transitdb");
        }
    }
}
