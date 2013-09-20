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
using OsmSharpDataProcessor.CommandLine;

namespace OsmSharpDataProcessor.Unittest
{
    /// <summary>
    /// Tests for command line parsing.
    /// </summary>
    [TestFixture]
    public class CommandParserTests
    {
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
            args = new string[] { "-rx", "somefile.osm" };

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
            args = new string[] { "-wx", "somefile.osm" };

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
            var args = new string[] { "--read-xml", "somefile.osm", "-wx", "someotherfile.osm" };

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
    }
}
