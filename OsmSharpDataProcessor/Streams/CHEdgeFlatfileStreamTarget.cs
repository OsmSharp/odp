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
using OsmSharp.Routing;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.PreProcessing.Ordering;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.CH.Serialization;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Interpreter;
using System.IO;

namespace OsmSharpDataProcessor.Streams
{
    /// <summary>
    /// A stream target that writes a flatfile.
    /// </summary>
    public class CHEdgeFlatfileStreamTarget : OsmSharp.Routing.Osm.Streams.Graphs.CHEdgeGraphOsmStreamTarget
    {
        /// <summary>
        /// Holds the output stream.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// Creates a new live edge flat file.
        /// </summary>
        /// <param name="tagsIndex"></param>
        /// <param name="vehicle"></param>
        /// <param name="stream"></param>
        public CHEdgeFlatfileStreamTarget(Stream stream, ITagsCollectionIndex tagsIndex, Vehicle vehicle)
            : base(new DynamicGraphRouterDataSource<CHEdgeData>(tagsIndex), new OsmRoutingInterpreter(), tagsIndex, vehicle)
        {
            _stream = stream;
        }

        /// <summary>
        /// Called after all data has been read.
        /// </summary>
        public override void OnAfterPull()
        {
            base.OnAfterPull();

            var serializer = new CHEdgeFlatfileSerializer();
            serializer.Serialize(_stream, this.DynamicGraph as DynamicGraphRouterDataSource<CHEdgeData>, new TagsCollection());
            _stream.Flush();
        }

        /// <summary>
        /// Creates the target.
        /// </summary>
        /// <returns></returns>
        public static CHEdgeFlatfileStreamTarget CreateTarget(Stream stream, Vehicle vehicle)
        {
            return new CHEdgeFlatfileStreamTarget(stream, new TagsTableCollectionIndex(), vehicle);
        }


        /// <summary>
        /// Returns the preprocessor for this stream.
        /// </summary>
        /// <returns></returns>
        public override OsmSharp.Routing.Graph.PreProcessor.IPreProcessor GetPreprocessor()
        {
            var witnessCalculator = new DykstraWitnessCalculator();
            var edgeDifference = new EdgeDifference(
                this.DynamicGraph, witnessCalculator);
            return new CHPreProcessorWrapper(new CHPreProcessor(this.DynamicGraph, edgeDifference, witnessCalculator));
        }

        /// <summary>
        /// TODO: remove this after issue: https://github.com/OsmSharp/OsmSharp/issues/112
        /// </summary>
        private class CHPreProcessorWrapper : OsmSharp.Routing.Graph.PreProcessor.IPreProcessor
        {
            private CHPreProcessor _preProcessor;

            public CHPreProcessorWrapper(CHPreProcessor preProcessor)
            {
                _preProcessor = preProcessor;
            }

            public void Start()
            {
                _preProcessor.Start();
            }
        }
    }


}
