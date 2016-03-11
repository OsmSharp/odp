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

using OsmSharpDataProcessor.Processors;
using OsmSharp.Routing.Profiles;
using System.IO;

namespace OsmSharpDataProcessor.Commands.RouterDbs
{
    /// <summary>
    /// A router db contraction command.
    /// </summary>
    public class CommandContract : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSwitch()
        {
            return new string[] { "--contract" };
        }

        /// <summary>
        /// The profile to contract for.
        /// </summary>
        public string[] Profiles { get; set; }

        /// <summary>
        /// Parse the command arguments for the read-pbf command.
        /// </summary>
        public override int Parse(string[] args, int idx, out Command command)
        {
            // check next argument.
            if (args.Length < idx)
            {
                throw new CommandLineParserException("None", "Invalid file name for contract command!");
            }

            // everything ok, take the next argument as the filename.
            command = new RouterDbs.CommandContract()
            {
                Profiles = args[idx].Split(',')
            };
            return 1;
        }

        /// <summary>
        /// Creates the processor that corresponds to this command.
        /// </summary>
        public override ProcessorBase CreateProcessor()
        {
            var profiles = new System.Collections.Generic.List<OsmSharp.Routing.Profiles.Profile>();
            foreach (var profileName in this.Profiles)
            {
                OsmSharp.Routing.Profiles.Profile profile;
                if (!OsmSharp.Routing.Profiles.Profile.TryGet(profileName, out profile))
                {
                    throw new CommandLineParserException("--contract",
                        string.Format("Invalid parameter value for command --contract: Profile '{0}' not found.",
                            this.Profiles));
                }
                profiles.Add(profile);
            }

            return new Processors.RouterDbs.RouterDbProcessorContract(profiles);
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("--contract {0}", this.Profiles);
        }
    }
}