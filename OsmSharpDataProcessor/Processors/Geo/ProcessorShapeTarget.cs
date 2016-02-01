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

using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using OsmSharp.Geo.Features;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharpDataProcessor.Processors.Geo
{
    /// <summary>
    /// A processor target to write a shapefile.
    /// </summary>
    public class ProcessorShapeTarget : ProcessorBase, IGeoTarget
    {
        private readonly string _path;

        /// <summary>
        /// Creates a new geojson target.
        /// </summary>
        public ProcessorShapeTarget(string path)
        {
            _path = path;
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

        /// <summary>
        /// Executes this target.
        /// </summary>
        public override void Execute()
        {
            var features = this.GetSourceFeatures();

            OsmSharp.Logging.Log.TraceEvent("Processor - Shapefile Writer", OsmSharp.Logging.TraceEventType.Information,
                "Writing Shapefile to {0}...", _path);

            var header = ShapefileDataWriter.GetHeader(features[0], features.Count);
            var shapeWriter = new ShapefileDataWriter(_path + ".shp", new GeometryFactory())
            {
                Header = header
            };
            shapeWriter.Write(features);
        }
    }
}