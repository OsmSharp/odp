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

using OsmSharpDataProcessor.Processors;
using OsmSharp.Routing.Profiles;
using System.IO;

namespace OsmSharpDataProcessor.Commands.MultimodalDbs
{
    /// <summary>
    /// A router db contraction command.
    /// </summary>
    public class CommandAddLinks : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSwitch()
        {
            return new string[] { "--add-links" };
        }

        /// <summary>
        /// The profile to add links for.
        /// </summary>
        public string Profile { get; set; }

        /// <summary>
        /// Parse the command arguments for the read-pbf command.
        /// </summary>
        public override int Parse(string[] args, int idx, out Command command)
        {
            // everything ok, take the next argument as the filename.
            command = new MultimodalDbs.CommandAddLinks()
            {
                Profile = args[idx]
            };
            return 1;
        }

        /// <summary>
        /// Creates the processor that corresponds to this command.
        /// </summary>
        public override ProcessorBase CreateProcessor()
        {
            OsmSharp.Routing.Profiles.Profile profile;
            if (!OsmSharp.Routing.Profiles.Profile.TryGet(this.Profile, out profile))
            {
                throw new CommandLineParserException("--add-links",
                    string.Format("Invalid parameter value for command --add-links: Profile '{0}' not found.",
                        this.Profile));
            }

            return new Processors.MultimodalDbs.ProcessorAddLink(
                profile);
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("--add-links {0}", this.Profile);
        }
    }
}