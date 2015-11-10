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

using OsmSharp.Routing;
using OsmSharp.Routing.Algorithms.Search;
using OsmSharp.Routing.Osm.Vehicles;
using OsmSharp.Routing.Profiles;
using Reminiscence.IO;
using System.IO;

namespace OsmSharpDataProcessor.Streams
{
    /// <summary>
    /// A stream target that writes a flatfile.
    /// </summary>
    public class RouterDbSerializerStreamTarget : OsmSharp.Routing.Osm.Streams.RouterDbStreamTarget
    {
        private readonly Stream _stream = null;
        private readonly RouterDb _db = null;
        private readonly Profile[] _contractionProfiles;

        /// <summary>
        /// Creates a new stream target.
        /// </summary>
        public RouterDbSerializerStreamTarget(Stream stream, Vehicle[] vehicles)
            : this(stream, new RouterDb(), vehicles, new Profile[0])
        {

        }

        /// <summary>
        /// Creates a new stream target.
        /// </summary>
        public RouterDbSerializerStreamTarget(Stream stream, Vehicle[] vehicles, MemoryMap map)
            : this(stream, new RouterDb(map), vehicles, new Profile[0])
        {

        }

        /// <summary>
        /// Creates a new stream target.
        /// </summary>
        public RouterDbSerializerStreamTarget(Stream stream, Vehicle[] vehicles, Profile[] contractionProfiles)
            : this(stream, new RouterDb(), vehicles, contractionProfiles)
        {

        }

        /// <summary>
        /// Creates a new stream target.
        /// </summary>
        public RouterDbSerializerStreamTarget(Stream stream, Vehicle[] vehicles, Profile[] contractionProfiles, MemoryMap map)
            : this(stream, new RouterDb(map), vehicles, contractionProfiles)
        {

        }

        /// <summary>
        /// Creates a new stream target.
        /// </summary>
        private RouterDbSerializerStreamTarget(Stream stream, RouterDb db, Vehicle[] vehicles, Profile[] contractionProfiles)
            : base(db, vehicles)
        {
            _stream = stream;
            _db = db;

            _contractionProfiles = contractionProfiles;
        }

        /// <summary>
        /// Called after all data has been read.
        /// </summary>
        public override void OnAfterPull()
        {
            base.OnAfterPull();

            // sort the network.
            _db.Network.Sort();

            // add contracted network.
            for (var i = 0; i < _contractionProfiles.Length; i++)
            {
                _db.AddContracted(_contractionProfiles[i]);
            }

            // write to given stream.
            _db.Serialize(_stream);
        }
    }
}
