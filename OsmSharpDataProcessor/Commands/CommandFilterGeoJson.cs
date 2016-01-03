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

using OsmSharp.Geo.Geometries;
using OsmSharpDataProcessor.Commands.Processors;
using System.IO;

namespace OsmSharpDataProcessor.Commands
{
    /// <summary>
    /// A geojson filter command.
    /// </summary>
    public class CommandFilterGeoJson : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSwitch()
        {
            return new string[] { "--bg", "--bounding-geojson" };
        }

        /// <summary>
        /// The geojson file.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Parses the command line arguments for the filter command.
        /// </summary>
        /// <returns></returns>
        public override int Parse(string[] args, int idx, out Command command)
        {
            // check next argument.
            if (args.Length < idx)
            {
                throw new CommandLineParserException("None", "Invalid file name for --bounding-geojson command!");
            }

            // everything ok, take the next argument as the filename.
            command = new CommandFilterGeoJson()
            {
                File = args[idx]
            };
            return 1;
        }

        /// <summary>
        /// Returns the processor that corresponds to this filter.
        /// </summary>
        /// <returns></returns>
        public override ProcessorBase CreateProcessor()
        {
            // poly file stream.
            using(var geoJsonFileStream = new StreamReader((new FileInfo(this.File)).OpenRead()))
            {
                var features = OsmSharp.Geo.Streams.GeoJson.GeoJsonConverter.ToFeatureCollection(geoJsonFileStream.ReadToEnd());
                LineairRing ring = null;
                foreach(var feature in features)
                {
                    if(feature.Geometry is Polygon)
                    {
                        ring = (feature.Geometry as Polygon).Ring;
                        break;
                    }
                    if(feature.Geometry is LineairRing)
                    {
                        ring = (feature.Geometry as LineairRing);
                        break;
                    }
                }
                if (ring == null)
                { // oeps, no ring or polygon found.
                    throw new System.Exception("Could not find a valid polygon to filter on in geojson file.");
                }
                return new ProcessorFilter(new OsmSharp.Osm.Streams.Filters.OsmStreamFilterPoly(ring));
            }
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("--bounding-geojson file={0}",
                this.File);
        }
    }
}