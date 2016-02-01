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
using System.Collections;
using GTFS;
using OsmSharp.Math.Geo.Simple;
using GTFS.Entities;
using OsmSharp.Math.Geo;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using GeoAPI.Geometries;

namespace OsmSharpDataProcessor.Processors.GTFS
{
    /// <summary>
    /// A feature list as a wrapper around a feed.
    /// </summary>
    class FeaturesList : IList<IFeature>
    {
        private readonly IGTFSFeed _feed;
        private readonly bool _includeStops;
        private readonly bool _includeTrips;

        /// <summary>
        /// Creates a feature list for the given feed.
        /// </summary>
        public FeaturesList(IGTFSFeed feed, bool includeStops, bool includeTrips)
        {
            _feed = feed;
            _includeStops = includeStops;
            _includeTrips = includeTrips;

            _stops = new Dictionary<string, Coordinate>(_feed.Stops.Count);
            _stopTimes = new Dictionary<string, List<StopTime>>();
        }

        private Dictionary<string, Coordinate> _stops;
        private Dictionary<string, List<StopTime>> _stopTimes;

        /// <summary>
        /// Gets the feature at the given index.
        /// </summary>
        public IFeature this[int index]
        {
            get
            {
                if(_stops.Count == 0)
                {
                    foreach(var stop in _feed.Stops)
                    {
                        _stops[stop.Id] = new Coordinate(stop.Longitude,
                            stop.Latitude);
                    }

                    foreach(var stopTime in _feed.StopTimes)
                    {
                        List<StopTime> stopTimes;
                        if(!_stopTimes.TryGetValue(stopTime.TripId, out stopTimes))
                        {
                            stopTimes = new List<StopTime>();
                            _stopTimes.Add(stopTime.TripId, stopTimes);
                        }
                        stopTimes.Add(stopTime);
                    }
                }

                if(_includeStops && index < _feed.Stops.Count)
                {
                    var stop = _feed.Stops.Get(index);
                    return new Feature(new Point(new Coordinate(
                        stop.Longitude, stop.Latitude)), new AttributesTable());
                }

                var tripIndex = index;
                if(_includeStops)
                {
                    tripIndex += _feed.Stops.Count;
                }

                var trip = _feed.Trips.Get(tripIndex);

                List<StopTime> tripStopTimes;
                _stopTimes.TryGetValue(trip.Id, out tripStopTimes);
                tripStopTimes.Sort((x, y) => x.StopSequence.CompareTo(y.StopSequence));
                var coordinates = new Coordinate[tripStopTimes.Count];
                for(var i = 0; i < coordinates.Length; i++)
                {
                    var stop = _stops[tripStopTimes[i].StopId];
                    coordinates[i] = new Coordinate(
                        stop);
                }
                return new Feature(new LineString(coordinates),
                    new AttributesTable());
            }
            set
            {
                throw new InvalidOperationException("List is readonly.");
            }
        }

        /// <summary>
        /// Returns the number of features in this list.
        /// </summary>
        public int Count
        {
            get
            {
                var count = 0;
                if(_includeStops)
                {
                    count += _feed.Stops.Count;
                }
                if(_includeTrips)
                {
                    count += _feed.Trips.Count;
                }
                return count;
            }
        }

        /// <summary>
        /// Returns true if this list is readonly/
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Adds a new item.
        /// </summary>
        /// <param name="item"></param>
        public void Add(IFeature item)
        {
            throw new InvalidOperationException("List is readonly.");
        }

        /// <summary>
        /// Clears all items.
        /// </summary>
        public void Clear()
        {
            throw new InvalidOperationException("List is readonly.");
        }
        
        public bool Contains(IFeature item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IFeature[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(IFeature item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, IFeature item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IFeature item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IFeature> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        private class Enumerator : IEnumerator<IFeature>
        {
            private readonly FeaturesList _list;

            public Enumerator(FeaturesList list)
            {
                _list = list;
            }

            private int _current = -1;

            public IFeature Current
            {
                get
                {
                    return _list[_current];
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return _list[_current];
                }
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                _current++;
                return _current < _list.Count;
            }

            public void Reset()
            {
                _current = -1;
            }
        }
    }
}