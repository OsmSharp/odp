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

using OsmSharp.Collections.Tags.Index;
using OsmSharp.Routing;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharpDataProcessor.Commands.Processors;
using OsmSharpDataProcessor.Streams;
using System;
using System.IO;

namespace OsmSharpDataProcessor.Commands
{
    /// <summary>
    /// The graph-write command.
    /// </summary>
    public class CommandWriteGraph : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSwitch()
        {
            return new string[] { "-wgr", "--write-graph" };
        }

        /// <summary>
        /// Gets or sets the graph output file.
        /// </summary>
        public string GraphFile { get; set; }

        /// <summary>
        /// Gets or sets the graph type.
        /// </summary>
        public GraphType GraphType { get; set; }

        /// <summary>
        /// Gets or sets the graph format.
        /// </summary>
        public FormatType GraphFormat { get; set; }

        /// <summary>
        /// Parse the command arguments for the write-xml command.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="idx"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public override int Parse(string[] args, int idx, out Command command)
        {
            CommandWriteGraph commandWriteGraph = new CommandWriteGraph();
            // check next argument.
            if (args.Length < idx)
            {
                throw new CommandLineParserException("None", "Invalid arguments for --write-graph!");
            }

            // parse arguments and keep parsing until the next switch.
            int startIdx = idx;
            while (args.Length > idx &&
                !CommandParser.IsSwitch(args[idx]))
            {
                string[] keyValue;
                if (CommandParser.SplitKeyValue(args[idx], out keyValue))
                { // the command splitting succeeded.
                    keyValue[0] = CommandParser.RemoveQuotes(keyValue[0]);
                    keyValue[1] = CommandParser.RemoveQuotes(keyValue[1]);
                    switch (keyValue[0].ToLower())
                    {
                        case "graph":
                            commandWriteGraph.GraphFile = keyValue[1];
                            break;
                        case "type":
                            string typeValue = keyValue[1].ToLower();
                            switch (typeValue)
                            {
                                case "simple":
                                    commandWriteGraph.GraphType = GraphType.Simple;
                                    break;
                                case "contracted":
                                    commandWriteGraph.GraphType = GraphType.Contracted;
                                    break;
                            }
                            break;
                        case "format":
                            string formatValue = keyValue[1].ToLower();
                            switch(formatValue)
                            {
                                case "flat":
                                    commandWriteGraph.GraphFormat = FormatType.Flat;
                                    break;
                                case "tiled":
                                    commandWriteGraph.GraphFormat = FormatType.Tiled;
                                    break;
                                case "mobile":
                                    commandWriteGraph.GraphFormat = FormatType.Mobile;
                                    break;
                            }
                            break;
                        default:
                            // the command splitting succeed but one of the arguments is unknown.
                            throw new CommandLineParserException("--write-graph",
                                string.Format("Invalid parameter for command --write-graph: {0} not recognized.", keyValue[0]));
                    }
                }
                else
                { // the command splitting failed and this is not a switch.
                    throw new CommandLineParserException("--write-graph", "Invalid parameter for command --write-graph.");
                }

                idx++; // increase the index.
            }

            // everything ok, take the next argument as the filename.
            command = commandWriteGraph;
            return idx - startIdx;
        }

        /// <summary>
        /// Creates the stream processor associated with this command.
        /// </summary>
        /// <returns></returns>
        public override ProcessorBase CreateProcessor()
        {
            // create output stream.
            var graphStream = (new FileInfo(this.GraphFile)).Open(FileMode.Create);

            switch(this.GraphType)
            {
                case GraphType.Simple:
                    switch(this.GraphFormat)
                    {
                        case FormatType.Flat:
                            return new ProcessorTarget(LiveEdgeFlatfileStreamTarget.CreateTarget(graphStream));
                        case FormatType.Tiled:
                            throw new NotSupportedException("Graphtype simple and format tiled is not supported.");
                        case FormatType.Mobile:
                            throw new NotSupportedException("Graphtype simple and format mobile is not supported.");
                    }
                    break;
                case GraphType.Contracted:
                    switch (this.GraphFormat)
                    {
                        case FormatType.Flat:
                            return new ProcessorTarget(CHEdgeFlatfileStreamTarget.CreateTarget(graphStream, Vehicle.Car));
                        case FormatType.Tiled:
                            throw new NotSupportedException("Graphtype simple and format tiled is not supported.");
                        case FormatType.Mobile:
                            var tagsIndex = new TagsTableCollectionIndex();
                            var interpreter = new OsmRoutingInterpreter();
                            var graph = new DynamicGraphRouterDataSource<CHEdgeData>(tagsIndex);
                            return new ProcessorTarget(new OsmSharp.Routing.Osm.Streams.CHEdgeGraphFileStreamTarget(graphStream,graph, interpreter, tagsIndex, Vehicle.Car));
                    }
                    break;
            }
            throw new InvalidCommandException("Invalid command: " + this.ToString());
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("--write-graph graph={0} type={1} format={2}",
                this.GraphFile, this.GraphType, this.GraphFormat);
        }
    }

    /// <summary>
    /// Graph type.
    /// </summary>
    public enum GraphType
    {
        /// <summary>
        /// Simple graph definition.
        /// </summary>
        Simple,
        /// <summary>
        /// Contracted graph definition.
        /// </summary>
        Contracted
    }

    /// <summary>
    /// Format type.
    /// </summary>
    public enum FormatType
    {
        /// <summary>
        /// The flat-file format.
        /// </summary>
        Flat,
        /// <summary>
        /// The tiled format.
        /// </summary>
        Tiled,
        /// <summary>
        /// The mobile format.
        /// </summary>
        Mobile
    }
}