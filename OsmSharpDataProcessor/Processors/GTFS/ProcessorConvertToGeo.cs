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
using System;
using System.Collections.Generic;
using NetTopologySuite.Features;

namespace OsmSharpDataProcessor.Processors.GTFS
{
    /// <summary>
    /// A processor to convert the GTFS to geometries.
    /// </summary>
    public class ProcessorConvertToGeo : ProcessorBase, Geo.IGeoSource
    {
        private readonly bool _includeStops = true;
        private readonly bool _includeTrips = true;

        /// <summary>
        /// Creates a new processor.
        /// </summary>
        public ProcessorConvertToGeo(bool includeStops = true, bool includeTrips = true)
        {
            _includeStops = includeStops;
            _includeTrips = includeTrips;
        }

        private Func<IGTFSFeed> _getFeed;

        /// <summary>
        /// Collapses the list of processors by trying to collapse this one.
        /// </summary>
        public override int Collapse(List<ProcessorBase> processors, int i)
        {
            if (processors == null || processors.Count == 0) { throw new ArgumentOutOfRangeException("processors"); }
            if (i < 1) { throw new ArgumentOutOfRangeException("i"); }

            if (processors[i - 1] is IGTFSSource)
            { // ok there is a source, keep it around for execution.
                _getFeed = (processors[i - 1] as IGTFSSource).GetFeed();
            }
            processors.RemoveAt(i - 1);
            return -1;
        }

        /// <summary>
        /// Returns true if this processor is ready.
        /// </summary>
        public override bool IsReady
        { // a source is always ready.
            get { return true; }
        }

        /// <summary>
        /// Executes the tasks or commands in this processor.
        /// </summary>
        public override void Execute()
        { // a source cannot be executed.
            throw new InvalidOperationException("This processor cannot be executed, check CanExecute before calling this method.");
        }

        /// <summary>
        /// Returns true if this processor can be executed.
        /// </summary>
        public override bool CanExecute
        { // a source cannot be executed.
            get { return false; }
        }

        /// <summary>
        /// Gets the function to get all features for this source.
        /// </summary>
        public Func<IList<IFeature>> GetFeatures
        {
            get
            {
                return () =>
                {
                    var feed = _getFeed();

                    return new FeaturesList(feed, _includeStops, _includeTrips);
                };
            }
        }
    }
}