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

using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.IO.MemoryMappedFiles;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Serialization;
using OsmSharp.Routing.Osm.Interpreter;
using System.IO;

namespace OsmSharpDataProcessor.Streams
{
    /// <summary>
    /// A stream target that writes a flatfile.
    /// </summary>
    public class LiveEdgeFlatfileStreamTarget : OsmSharp.Routing.Osm.Streams.GraphOsmStreamTarget
    {
        /// <summary>
        /// Holds the output stream.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// Creates a new live edge flat file.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="tagsIndex"></param>
        public LiveEdgeFlatfileStreamTarget(Stream stream, ITagsIndex tagsIndex)
            : base(new RouterDataSource<Edge>(new Graph<Edge>(), tagsIndex), new OsmRoutingInterpreter(), tagsIndex)
        {
            _stream = stream;
        }

        /// <summary>
        /// Called after all data has been read.
        /// </summary>
        public override void OnAfterPull()
        {
            base.OnAfterPull();

            var serializer = new RoutingDataSourceSerializer();
            serializer.Serialize(_stream, this.Graph as RouterDataSource<Edge>, new TagsCollection());
            _stream.Flush();
        }

        /// <summary>
        /// Creates the target.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static LiveEdgeFlatfileStreamTarget CreateTarget(Stream stream)
        {
            return new LiveEdgeFlatfileStreamTarget(stream, new TagsIndex(new MemoryMappedStream()));
        }
    }
}
