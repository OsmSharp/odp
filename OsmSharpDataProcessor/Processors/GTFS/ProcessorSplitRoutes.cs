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

using System;
using System.Collections.Generic;
using GTFS;
using GTFS.Entities;
using GTFS.Filters;

namespace OsmSharpDataProcessor.Processors.GTFS
{
    /// <summary>
    /// A processor to split a GTFS into two new feeds: one with a selection of routes and one without.
    /// 
    /// - Stops are remove when they have no trips anymore.
    /// - Agencies, stops and calendars are duplicated over both feeds if they are shared by routes to include and not include.
    /// </summary>
    class ProcessorSplitRoutes : ProcessorBase, IGTFSMultiSource
    {
        private readonly Func<IGTFSFeed, Route, bool> _includeRoute;

        /// <summary>
        /// Creates a new processor to split a GTFS along routes.
        /// </summary>
        public ProcessorSplitRoutes(Func<IGTFSFeed, Route, bool> includeRoute)
        {
            _includeRoute = includeRoute;
        }

        private IGTFSFeed _cachedFeed;
        private Func<IGTFSFeed> _getFeed;

        /// <summary>
        /// Returns true if this processor is ready.
        /// </summary>
        public override bool IsReady
        {
            get
            {
                return _getFeed != null;
            }
        }

        /// <summary>
        /// Returns true if this processor can be executed.
        /// </summary>
        public override bool CanExecute
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the functions to get the GTFS feeds.
        /// </summary>
        /// <returns></returns>
        public Func<IGTFSFeed>[] GetFeeds()
        {
            Func<IGTFSFeed> cachedGetFeed = () =>
            {
                if (_cachedFeed == null)
                {
                    _cachedFeed = _getFeed();
                }
                return _cachedFeed;
            };

            return new Func<IGTFSFeed>[]
            {
                () =>
                {
                    var feed = cachedGetFeed();
                    return ProcessorSplitRoutes.FilterFeed(feed, 
                        (r) => _includeRoute(feed, r));
                },
                () =>
                {
                    var feed = cachedGetFeed();
                    return ProcessorSplitRoutes.FilterFeed(feed,
                        (r) => !_includeRoute(feed, r));
                }
            };
        }

        /// <summary>
        /// Collapses this processor.
        /// </summary>
        public override int Collapse(List<ProcessorBase> processors, int i)
        {
            if (processors == null || processors.Count < 3) { throw new ArgumentOutOfRangeException("processors"); }
            if (i < 0) { throw new ArgumentOutOfRangeException("i"); }
            if (processors.Count <= i + 2) { throw new ArgumentException("Cannot find two processors after split operation."); }

            var feeds = this.GetFeeds();
            
            if(processors[i - 1] is IGTFSSource &&
               processors[i + 1] is IGTFSTarget &&
               processors[i + 2] is IGTFSTarget)
            {
                _getFeed = (processors[i - 1] as IGTFSSource).GetFeed();
                var getFeeds = this.GetFeeds();
                (processors[i + 1] as IGTFSTarget).GetSourceFeed = getFeeds[0];
                (processors[i + 2] as IGTFSTarget).GetSourceFeed = getFeeds[1];
            }
            processors.RemoveAt(i - 1);
            processors.RemoveAt(i - 1);
            return 1;
        }

        /// <summary>
        /// Executes this processor.
        /// </summary>
        public override void Execute()
        {
            throw new InvalidOperationException("Cannot execute processor, check CanExecute.");
        }

        /// <summary>
        /// Filters the given feed.
        /// </summary>
        /// <returns></returns>
        private static IGTFSFeed FilterFeed(IGTFSFeed feed,
            Func<Route, bool> includeRoutes)
        {
            var filter = new GTFSFeedRoutesFilter(includeRoutes);
            return filter.Filter(feed);
        }
    }
}