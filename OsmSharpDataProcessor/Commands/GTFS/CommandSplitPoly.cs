// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
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

using GTFS;
using GTFS.Entities;
using OsmSharp.Geo.Geometries;
using OsmSharpDataProcessor.Processors;
using OsmSharpDataProcessor.Processors.GTFS;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharpDataProcessor.Commands.GTFS
{
    /// <summary>
    /// Split poly.
    /// </summary>
    class CommandSplitPoly : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSwitch()
        {
            return new string[] { "--split-polygon" };
        }

        /// <summary>
        /// The polygon file.
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
                throw new CommandLineParserException("None", "Invalid file name for --split-polygon command!");
            }

            // everything ok, take the next argument as the filename.
            command = new CommandSplitPoly()
            {
                File = args[idx]
            };
            return 1;
        }

        private HashSet<string> _routes;

        /// <summary>
        /// Returns the processor that corresponds to this filter.
        /// </summary>
        /// <returns></returns>
        public override ProcessorBase CreateProcessor()
        {
            // poly file stream.
            using (var polyFileStream = new StreamReader((new FileInfo(this.File)).OpenRead()))
            {
                var geoJson = polyFileStream.ReadToEnd();
                var poly = OsmSharp.Geo.Streams.GeoJson.GeoJsonConverter.ToFeature(geoJson);
                if (!(poly.Geometry is LineairRing) &&
                   !(poly.Geometry is Polygon))
                { // oeps, no ring or polygon found.
                    throw new System.Exception("Could not find a valid polygon to filter on based on poly file.");
                }
                LineairRing ring = null;
                if (poly.Geometry is LineairRing)
                {
                    ring = poly.Geometry as LineairRing;
                }
                else
                {
                    var polygon = poly.Geometry as Polygon;
                    ring = polygon.Ring;
                }

                _routes = null;
                Func<IGTFSFeed, Route, bool> includeRoute = (f, r) =>
                {
                    if (_routes == null)
                    {
                        _routes = f.GetRoutesFor((s) =>
                        {
                            return ring.Contains(new OsmSharp.Math.Geo.GeoCoordinate(s.Latitude, s.Longitude));
                        });
                    }
                    return _routes.Contains(r.Id);
                };
                return new ProcessorSplitRoutes(includeRoute);
            }
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("--bounding-polygon file={0}",
                this.File);
        }
    }
}
