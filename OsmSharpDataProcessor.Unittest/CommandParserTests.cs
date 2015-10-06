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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharpDataProcessor.Commands;

namespace OsmSharpDataProcessor.Unittest
{
    /// <summary>
    /// Tests for command line parsing.
    /// </summary>
    [TestFixture]
    public class CommandParserTests
    {
        /// <summary>
        /// Tests the split key-value parser.
        /// </summary>
        [Test]
        public void SplitValuesArray()
        {
            string valuesArray = "1,1";
            string[] values;
            Assert.IsTrue(CommandParser.SplitValuesArray(valuesArray, out values));
            Assert.AreEqual(2, values.Length);
            Assert.AreEqual("1", values[0]);
            Assert.AreEqual("1", values[1]);

            valuesArray = "1";
            Assert.IsTrue(CommandParser.SplitValuesArray(valuesArray, out values));
            Assert.AreEqual(1, values.Length);
            Assert.AreEqual("1", values[0]);
        }

        /// <summary>
        /// Tests the split key-value parser.
        /// </summary>
        [Test]
        public void TestSplitKeyValue()
        {
            string keyValueString = "key=value";
            string[] keyValue;
            Assert.IsTrue(CommandParser.SplitKeyValue(keyValueString, out keyValue));
            Assert.AreEqual("key", keyValue[0]);
            Assert.AreEqual("value", keyValue[1]);

            keyValueString = "key=value=anothervalue";
            Assert.IsFalse(CommandParser.SplitKeyValue(keyValueString, out keyValue));

            keyValueString = "key=";
            Assert.IsFalse(CommandParser.SplitKeyValue(keyValueString, out keyValue));

            keyValueString = "=value";
            Assert.IsFalse(CommandParser.SplitKeyValue(keyValueString, out keyValue));

            keyValueString = "onlyvalueorkey";
            Assert.IsFalse(CommandParser.SplitKeyValue(keyValueString, out keyValue));

            keyValueString = "1=2";
            Assert.IsTrue(CommandParser.SplitKeyValue(keyValueString, out keyValue));
            Assert.AreEqual("1", keyValue[0]);
            Assert.AreEqual("2", keyValue[1]);
        }

        /// <summary>
        /// Tests the read-xml command.
        /// </summary>
        [Test]
        public void TestReadXml()
        {
            // define some args.
            var args = new string[]{"--read-xml", "somefile.osm"};

            // parse.
            Command[] commands = CommandParser.ParseCommands(args);

            // verify.
            Assert.NotNull(commands);
            Assert.AreEqual(1, commands.Length);
            Assert.IsTrue(commands[0] is CommandReadXml);
            Assert.AreEqual("somefile.osm", (commands[0] as CommandReadXml).File);

            // define short args.
            args = new string[] { "--rx", "somefile.osm" };

            // parse.
            commands = CommandParser.ParseCommands(args);

            // verify.
            Assert.NotNull(commands);
            Assert.AreEqual(1, commands.Length);
            Assert.IsTrue(commands[0] is CommandReadXml);
            Assert.AreEqual("somefile.osm", (commands[0] as CommandReadXml).File);
        }

        /// <summary>
        /// Tests the write-xml command.
        /// </summary>
        [Test]
        public void TestWriteXml()
        {
            // define some args.
            var args = new string[] { "--write-xml", "somefile.osm" };

            // parse.
            Command[] commands = CommandParser.ParseCommands(args);

            // verify.
            Assert.NotNull(commands);
            Assert.AreEqual(1, commands.Length);
            Assert.IsTrue(commands[0] is CommandWriteXml);
            Assert.AreEqual("somefile.osm", (commands[0] as CommandWriteXml).File);

            // define short args.
            args = new string[] { "--wx", "somefile.osm" };

            // parse.
            commands = CommandParser.ParseCommands(args);

            // verify.
            Assert.NotNull(commands);
            Assert.AreEqual(1, commands.Length);
            Assert.IsTrue(commands[0] is CommandWriteXml);
            Assert.AreEqual("somefile.osm", (commands[0] as CommandWriteXml).File);
        }

        /// <summary>
        /// Tests a real read-write xml command.
        /// </summary>
        [Test]
        public void TestReadWriteXml()
        {
            // define some args.
            var args = new string[] { "--read-xml", "somefile.osm", "--wx", "someotherfile.osm" };

            // parse.
            Command[] commands = CommandParser.ParseCommands(args);

            // verify.
            Assert.NotNull(commands);
            Assert.AreEqual(2, commands.Length);
            Assert.IsTrue(commands[0] is CommandReadXml);
            Assert.AreEqual("somefile.osm", (commands[0] as CommandReadXml).File);
            Assert.IsTrue(commands[1] is CommandWriteXml);
            Assert.AreEqual("someotherfile.osm", (commands[1] as CommandWriteXml).File);
        }

        /// <summary>
        /// Tests a real read-write pbf command.
        /// </summary>
        [Test]
        public void TestReadWritePBF()
        {
            // define some args.
            var args = new string[] { "--read-pbf", "somefile.osm.pbf", "--wb", "someotherfile.osm.pbf" };

            // parse.
            Command[] commands = CommandParser.ParseCommands(args);

            // verify.
            Assert.NotNull(commands);
            Assert.AreEqual(2, commands.Length);
            Assert.IsTrue(commands[0] is CommandReadPBF);
            Assert.AreEqual("somefile.osm.pbf", (commands[0] as CommandReadPBF).File);
            Assert.IsTrue(commands[1] is CommandWritePBF);
            Assert.AreEqual("someotherfile.osm.pbf", (commands[1] as CommandWritePBF).File);

            // define some args.
            args = new string[] { "--rb", "somefile.osm.pbf", "--write-pbf", "someotherfile.osm.pbf" };

            // parse.
            commands = CommandParser.ParseCommands(args);

            // verify.
            Assert.NotNull(commands);
            Assert.AreEqual(2, commands.Length);
            Assert.IsTrue(commands[0] is CommandReadPBF);
            Assert.AreEqual("somefile.osm.pbf", (commands[0] as CommandReadPBF).File);
            Assert.IsTrue(commands[1] is CommandWritePBF);
            Assert.AreEqual("someotherfile.osm.pbf", (commands[1] as CommandWritePBF).File);
        }

        /// <summary>
        /// Tests a write scene command.
        /// </summary>
        [Test]
        public void TestWriteScene()
        {
            // define some args.
            var args = new string[] { "--rx", "somefile.osm", "--write-scene", "scene=scene.out", "css=scene.mapcss", "cutoffs=5,11,13,15,18" };

            // parse.
            Command[] commands = CommandParser.ParseCommands(args);

            // verify.
            Assert.NotNull(commands);
            Assert.AreEqual(2, commands.Length);
            Assert.IsTrue(commands[0] is CommandReadXml);
            Assert.AreEqual("somefile.osm", (commands[0] as CommandReadXml).File);
            Assert.IsTrue(commands[1] is CommandWriteScene);
            Assert.AreEqual("scene.mapcss", (commands[1] as CommandWriteScene).MapCSS);
            Assert.AreEqual("scene.out", (commands[1] as CommandWriteScene).SceneFile);
            Assert.IsNotNull((commands[1] as CommandWriteScene).ZoomLevelCutoffs);
            Assert.AreEqual(5, (commands[1] as CommandWriteScene).ZoomLevelCutoffs.Length);
            Assert.AreEqual(5, (commands[1] as CommandWriteScene).ZoomLevelCutoffs[0]);
            Assert.AreEqual(11, (commands[1] as CommandWriteScene).ZoomLevelCutoffs[1]);
            Assert.AreEqual(13, (commands[1] as CommandWriteScene).ZoomLevelCutoffs[2]);
            Assert.AreEqual(15, (commands[1] as CommandWriteScene).ZoomLevelCutoffs[3]);
            Assert.AreEqual(18, (commands[1] as CommandWriteScene).ZoomLevelCutoffs[4]);
        }

        /// <summary>
        /// Tests a write graph command.
        /// </summary>
        [Test]
        public void TestWriteGraph()
        {
            // define some args.
            var args = new string[] { "--rx", "somefile.osm", "--write-graph", "graph=graph.out", "type=contracted" };

            // parse.
            var commands = CommandParser.ParseCommands(args);

            // verify.
            Assert.NotNull(commands);
            Assert.AreEqual(2, commands.Length);
            Assert.IsTrue(commands[0] is CommandReadXml);
            Assert.AreEqual("somefile.osm", (commands[0] as CommandReadXml).File);
            Assert.IsTrue(commands[1] is CommandWriteGraph);
            Assert.AreEqual("graph.out", (commands[1] as CommandWriteGraph).GraphFile);
            Assert.AreEqual(GraphType.Contracted, (commands[1] as CommandWriteGraph).GraphType);
        }
    }
}