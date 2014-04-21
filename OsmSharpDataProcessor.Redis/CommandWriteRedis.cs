using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Data.Redis.Osm.Streams;
using OsmSharpDataProcessor.Commands;

namespace OsmSharpDataProcessor.CommandLine
{
    /// <summary>
    /// The write-redis command.
    /// </summary>
    public class CommandWriteRedis : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSwitch()
        {
            return new string[] { "-wre", "--write-redis" };
        }

        /// <summary>
        /// Creates a new write-redis command.
        /// </summary>
        public CommandWriteRedis()
        {
            this.Truncate = true;
            this.Create = true;
        }

        /// <summary>
        /// The connectionstring of the database to write to.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// The truncate flag.
        /// </summary>
        public bool Truncate { get; set; }

        /// <summary>
        /// The create flag.
        /// </summary>
        public bool Create { get; set; }

        /// <summary>
        /// Parse the command arguments for the write-xml command.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="idx"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public override int Parse(string[] args, int idx, out Command command)
        {
            // check next argument.
            if (args.Length < idx)
            {
                throw new CommandLineParserException("None", "Invalid file name for write-xml command!");
            }

            // everything ok, take the next argument as the filename.
            command = new CommandWriteRedis()
            {
                ConnectionString = args[idx]
            };
            return 1;
        }

        /// <summary>
        /// Creates a processor that corresponds to this command.
        /// </summary>
        /// <returns></returns>
        public override object CreateProcessor()
        {
            return new RedisOsmStreamTarget(this.ConnectionString);
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("--write-redis {0}", this.ConnectionString);
        }
    }
}
