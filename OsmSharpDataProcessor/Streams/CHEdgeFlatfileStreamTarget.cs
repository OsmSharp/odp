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

using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.IO.MemoryMappedFiles;
using OsmSharp.Routing.CH.Preprocessing;
using OsmSharp.Routing.CH.Serialization;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Vehicles;
using System.IO;

namespace OsmSharpDataProcessor.Streams
{
    /// <summary>
    /// A stream target that writes a flatfile.
    /// </summary>
    public class CHEdgeFlatfileStreamTarget : OsmSharp.Routing.Osm.Streams.CHEdgeGraphOsmStreamTarget
    {
        private readonly Stream _stream = null;
        private readonly MemoryMappedStream _memoryMappedStream = null;

        /// <summary>
        /// Creates a new stream target.
        /// </summary>
        public CHEdgeFlatfileStreamTarget(Stream stream, ITagsIndex tagsIndex, Vehicle vehicle)
            : base(new RouterDataSource<CHEdgeData>(new DirectedGraph<CHEdgeData>(), tagsIndex), new OsmRoutingInterpreter(), tagsIndex, vehicle)
        {
            _stream = stream;
        }

        /// <summary>
        /// Creates a new stream target.
        /// </summary>
        public CHEdgeFlatfileStreamTarget(Stream stream, ITagsIndex tagsIndex, Vehicle vehicle, MemoryMappedStream memoryMappedStream)
            : base(new RouterDataSource<CHEdgeData>(new DirectedGraph<CHEdgeData>(memoryMappedStream, 1000, 
                CHEdgeData.MapFromDelegate, CHEdgeData.MapToDelegate, CHEdgeData.SizeUints), tagsIndex), new OsmRoutingInterpreter(), tagsIndex, vehicle)
        {
            _stream = stream;
            _memoryMappedStream = memoryMappedStream;
        }

        /// <summary>
        /// Called after all data has been read.
        /// </summary>
        public override void OnAfterPull()
        {
            base.OnAfterPull();

            RouterDataSource<CHEdgeData> copy = null;
            if(_memoryMappedStream == null)
            {
                copy = new RouterDataSource<CHEdgeData>(new DirectedGraph<CHEdgeData>(),
                    (this.Graph as RouterDataSource<CHEdgeData>).TagsIndex);
            }
            else
            {
                copy = new RouterDataSource<CHEdgeData>(new DirectedGraph<CHEdgeData>(_memoryMappedStream, this.Graph.VertexCount, 
                    CHEdgeData.MapFromDelegate, CHEdgeData.MapToDelegate, CHEdgeData.SizeUints), 
                        (this.Graph as RouterDataSource<CHEdgeData>).TagsIndex);
            }
            copy.CopyFrom(this.Graph);
            copy.Trim();
            copy.Compress();

            var serializer = new CHEdgeSerializer();
            serializer.Serialize(_stream, copy as RouterDataSource<CHEdgeData>, new TagsCollection());
            _stream.Flush();
        }

        /// <summary>
        /// Creates the target.
        /// </summary>
        /// <returns></returns>
        public static CHEdgeFlatfileStreamTarget CreateTarget(Stream stream, Vehicle vehicle, MemoryMappedStream memoryMappedStream)
        {
            if(memoryMappedStream == null)
            {
                return new CHEdgeFlatfileStreamTarget(stream, new TagsIndex(new MemoryMappedStream()), vehicle);
            }
            return new CHEdgeFlatfileStreamTarget(stream, new TagsIndex(memoryMappedStream), vehicle, memoryMappedStream);
        }
    }
}
