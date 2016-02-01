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

using OsmSharp;
using NetTopologySuite.Features;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using OsmSharp.Math.Geo;
using GeoAPI.Geometries;

namespace OsmSharpDataProcessor.Processors.Geo
{
    /// <summary>
    /// A processor target to write geojson.
    /// </summary>
    public class ProcessorGeoJsonTarget : ProcessorBase, IGeoTarget
    {
        private readonly string _file;

        /// <summary>
        /// Creates a new geojson target.
        /// </summary>
        public ProcessorGeoJsonTarget(string file)
        {
            _file = file;
        }

        /// <summary>
        /// Returns true if this target can be executed.
        /// </summary>
        public override bool CanExecute
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the function to get the source features.
        /// </summary>
        public Func<IList<IFeature>> GetSourceFeatures
        {
            get;
            set;
        }

        /// <summary>
        /// Returns true if this processor is read.
        /// </summary>
        public override bool IsReady
        {
            get
            {
                return this.GetSourceFeatures != null;
            }
        }

        /// <summary>
        /// Collapses this processor.
        /// </summary>
        public override int Collapse(List<ProcessorBase> processors, int i)
        {
            if (processors == null) { throw new ArgumentNullException("processors"); }
            if (processors.Count == 0) { throw new ArgumentOutOfRangeException("processors", "There has to be at least on processor there to collapse this target."); }
            if (processors[processors.Count - 1] == null) { throw new ArgumentOutOfRangeException("processors", "The last processor in the processors list is null."); }
            if (i < 1) { throw new ArgumentOutOfRangeException("i"); }

            // take the last processor and collapse.
            if (processors[i - 1] is IGeoSource)
            { // ok, processor is a source.
                var source = processors[i - 1] as IGeoSource;
                processors.RemoveAt(i - 1);

                this.GetSourceFeatures = source.GetFeatures;
                return -1;
            }
            throw new InvalidOperationException("Last processor before filter is not a source.");
        }

        private static OsmSharp.Geo.Features.Feature Convert(IFeature feature)
        {
            var attributes = ProcessorGeoJsonTarget.Convert(feature.Attributes);
            if (feature.Geometry is NetTopologySuite.Geometries.Point)
            {
                var point = (feature.Geometry as NetTopologySuite.Geometries.Point);
                return new OsmSharp.Geo.Features.Feature(new OsmSharp.Geo.Geometries.Point(
                    new OsmSharp.Math.Geo.GeoCoordinate(point.Coordinate.Y, point.Coordinate.X)),
                        attributes);
            }
            else if (feature.Geometry is NetTopologySuite.Geometries.LineString)
            {
                var lineString = (feature.Geometry as NetTopologySuite.Geometries.LineString);
                var coordinates = Convert(lineString.Coordinates);
                return new OsmSharp.Geo.Features.Feature(
                    new OsmSharp.Geo.Geometries.LineString(
                        coordinates), attributes);
            }
            else if (feature.Geometry is NetTopologySuite.Geometries.LinearRing)
            {
                var lineString = (feature.Geometry as NetTopologySuite.Geometries.LinearRing);
                var coordinates = Convert(lineString.Coordinates);
                return new OsmSharp.Geo.Features.Feature(
                    new OsmSharp.Geo.Geometries.LineairRing(
                        coordinates), attributes);
            }
            throw new Exception("Unknown geometry type.");
        }

        private static OsmSharp.Geo.Attributes.SimpleGeometryAttributeCollection Convert(IAttributesTable attributes)
        {
            var converted = new OsmSharp.Geo.Attributes.SimpleGeometryAttributeCollection();
            var names = attributes.GetNames();
            var values = attributes.GetValues();
            for (var i = 0; i < attributes.Count; i++)
            {
                converted.Add(names[i], values[i].ToInvariantString());
            }
            return converted;
        }

        private static GeoCoordinate[] Convert(Coordinate[] coordinates)
        {
            var converted = new GeoCoordinate[coordinates.Length];
            for (var i = 0; i < coordinates.Length; i++)
            {
                converted[i] = new GeoCoordinate(
                    (float)coordinates[i].Y, (float)coordinates[i].X);
            }
            return converted;
        }

        /// <summary>
        /// Executes this target.
        /// </summary>
        public override void Execute()
        {
            var features = this.GetSourceFeatures();
            
            OsmSharp.Logging.Log.TraceEvent("Processor - GeoJson Writer", OsmSharp.Logging.TraceEventType.Information,
                "Writing GeoJson to {0}...", _file);

            var osmsharpFeatures = features.Select<IFeature,
                OsmSharp.Geo.Features.Feature>((f) =>
                {
                    return ProcessorGeoJsonTarget.Convert(f);
                });

            using (var outputStream = File.OpenWrite(_file))
            {
                using (var textWriter = new StreamWriter(outputStream))
                {
                    OsmSharp.Geo.Streams.GeoJson.GeoJsonConverter.Write(
                        new OsmSharp.IO.Json.JsonTextWriter(textWriter),
                            osmsharpFeatures);
                }
            }
        }
    }
}