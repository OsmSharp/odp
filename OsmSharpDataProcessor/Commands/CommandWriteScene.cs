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

using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain;
using OsmSharp.UI.Map.Styles.Streams;
using OsmSharp.UI.Renderer.Scene;
using OsmSharpDataProcessor.Processors;
using System.IO;
using System.Linq;

namespace OsmSharpDataProcessor.Commands
{
    /// <summary>
    /// The scene-write command.
    /// </summary>
    public class CommandWriteScene : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSwitch()
        {
            return new string[] { "--ws", "--write-scene" };
        }

        /// <summary>
        /// Gets or sets the MapCSS file.
        /// </summary>
        public string MapCSS { get; set; }

        /// <summary>
        /// Gets or sets the scene output file.
        /// </summary>
        public string SceneFile { get; set; }

        /// <summary>
        /// Gets or sets the zoom level cutoffs.
        /// </summary>
        public float[] ZoomLevelCutoffs { get; set; }

        /// <summary>
        /// Parse the command arguments for the write-xml command.
        /// </summary>
        public override int Parse(string[] args, int idx, out Command command)
        {
            CommandWriteScene commandWriteScene = new CommandWriteScene();
            // check next argument.
            if (args.Length < idx)
            {
                throw new CommandLineParserException("None", "Invalid arguments for --write-scene!");
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
                        case "scene":
                            commandWriteScene.SceneFile = keyValue[1];
                            break;
                        case "css":
                            commandWriteScene.MapCSS = keyValue[1];
                            break;
                        case "cutoffs":
                            string[] values;
                            if (CommandParser.SplitValuesArray(keyValue[1], out values))
                            { // split the values array.
                                float[] cutoffs = new float[values.Length];
                                for (int valueIdx = 0; valueIdx < values.Length; valueIdx++)
                                {
                                    float value;
                                    if (!float.TryParse(values[valueIdx], System.Globalization.NumberStyles.Any,
                                        System.Globalization.CultureInfo.InvariantCulture, out value))
                                    {
                                        throw new CommandLineParserException("--write-scene",
                                            string.Format("Invalid parameter value for command --write-scene parameter {0}: {1} not recognized.", keyValue[0], values[valueIdx]));
                                    }
                                    cutoffs[valueIdx] = value;
                                }
                                commandWriteScene.ZoomLevelCutoffs = cutoffs;
                            }
                            break;
                        default:
                            // the command splitting succeed but one of the arguments is unknown.
                            throw new CommandLineParserException("--write-scene",
                                string.Format("Invalid parameter for command --write-scene: {0} not recognized.", keyValue[0]));

                    }
                }
                else
                { // the command splitting failed and this is not a switch.
                    throw new CommandLineParserException("--write-scene", "Invalid parameter for command --write-scene.");
                }

                idx++; // increase the index.
            }

            // check command consistency.
            if (commandWriteScene.ZoomLevelCutoffs == null ||
                    commandWriteScene.ZoomLevelCutoffs.Length == 0)
            { // assign some defaults instead of complaining.
                commandWriteScene.ZoomLevelCutoffs = new float[] { 16, 13, 10, 7, 4 };
            }

            // everything ok, take the next argument as the filename.
            command = commandWriteScene;
            return idx - startIdx;
        }

        /// <summary>
        /// Creates the stream processor associated with this command.
        /// </summary>
        /// <returns></returns>
        public override ProcessorBase CreateProcessor()
        {
            // mapCSS stream.
            Stream mapCSSStream = (new FileInfo(this.MapCSS)).OpenRead();
            MapCSSFile mapCSSFile = MapCSSFile.FromStream(mapCSSStream);

            // scene stream.
            Stream sceneStream = (new FileInfo(this.SceneFile)).Open(FileMode.OpenOrCreate);

            // create web mercator.
            IProjection projection = new WebMercator();

            // create scene.
            Scene2D scene = new Scene2D(projection, this.ZoomLevelCutoffs.ToList());

            return new ProcessorCompleteTarget(new StyleOsmStreamSceneStreamTarget(
                new MapCSSInterpreter(mapCSSFile, new MapCSSDictionaryImageSource()),
                scene,
                projection,
                sceneStream));
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.ZoomLevelCutoffs != null && this.ZoomLevelCutoffs.Length > 0)
            {
                string cutoffs = "";
                for (int idx = 0; idx < this.ZoomLevelCutoffs.Length; idx++)
                {
                    cutoffs = cutoffs + this.ZoomLevelCutoffs[idx].ToString(System.Globalization.CultureInfo.InvariantCulture) + ",";
                }
                cutoffs = cutoffs.Substring(0, cutoffs.Length - 1);
                return string.Format("--write-scene css={0} scene={1} cutoffs={2}",
                    this.MapCSS, this.SceneFile, cutoffs);
            }
            return string.Format("--write-scene css={0} scene={1}",
                this.MapCSS, this.SceneFile);
        }
    }
}