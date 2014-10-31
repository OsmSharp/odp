using NUnit.Framework;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm;
using OsmSharp.Osm.Streams;
using OsmSharp.Osm.Data.Memory;
using OsmSharp.Osm.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Streams.Complete;
using OsmSharpDataProcessor.NTS.Osm;
using GeoAPI.Geometries;
using NetTopologySuite.Features;

namespace OsmSharpDataProcessor.Unittest.NTS.Osm
{
    /// <summary>
    /// Geometry interpreter stream tests.
    /// </summary>
    [TestFixture]
    public class GeometryInterpreterStreamSourceTests
    {
        /// <summary>
        /// Tests way area is yes.
        /// </summary>
        [Test]
        public void TestWayAreaIsYes()
        {
            Node node1 = new Node();
            node1.Id = 1;
            node1.Latitude = 0;
            node1.Longitude = 0;
            Node node2 = new Node();
            node2.Id = 2;
            node2.Latitude = 1;
            node2.Longitude = 0;
            Node node3 = new Node();
            node3.Id = 3;
            node3.Latitude = 0;
            node3.Longitude = 1;

            Way way = new Way();
            way.Id = 1;
            way.Nodes = new List<long>();
            way.Nodes.Add(1);
            way.Nodes.Add(2);
            way.Nodes.Add(3);
            way.Nodes.Add(1);
            way.Tags = new TagsCollection();
            way.Tags.Add("area", "yes");

            var source = new List<OsmGeo>();
            source.Add(node1);
            source.Add(node2);
            source.Add(node3);
            source.Add(way);

            // the use of natural=water implies an area-type.
            var interpreter = new SimpleGeometryInterpreter();
            var completeStreamSource = new OsmSimpleCompleteStreamSource(source.ToOsmStreamSource());

            // use the stream to interpret.
            var features = new List<Feature>(new FeatureInterpreterStreamSource(completeStreamSource, interpreter));

            Assert.AreEqual(1, features.Count);
        }
    }
}