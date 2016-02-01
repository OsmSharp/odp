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

using OsmSharp;
using OsmSharp.Collections.Tags;
using OsmSharp.Geo.Geometries;
using OsmSharp.Osm.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.Algorithms.Search;
using OsmSharp.Routing.Osm;
using OsmSharp.Routing.Osm.Vehicles;
using OsmSharp.Routing.Profiles;
using Reminiscence.IO;
using System;
using System.Collections.Generic;

namespace OsmSharpDataProcessor.Processors
{
    /// <summary>
    /// A processor to create a router db.
    /// </summary>
    class ProcessorCreateRouterDb : ProcessorBase, RouterDbs.IRouterDbSource
    {
        private readonly Vehicle[] _vehicles;
        private readonly Profile[] _contractionProfiles;
        private readonly bool _allCore;
        private readonly bool _keepWayIds;
        private readonly MemoryMap _map;
        private readonly LineairRing _poly;

        /// <summary>
        /// Creates a new processor.
        /// </summary>
        public ProcessorCreateRouterDb(Vehicle[] vehicles, 
            Profile[] contractionProfiles, bool allCore, bool keepWayIds, LineairRing poly)
        {
            _vehicles = vehicles;
            _contractionProfiles = contractionProfiles;
            _allCore = allCore;
            _keepWayIds = keepWayIds;
            _map = null;
            _poly = poly;
        }

        /// <summary>
        /// Creates a new processor.
        /// </summary>
        public ProcessorCreateRouterDb(MemoryMap map, Vehicle[] vehicles,
            Profile[] contractionProfiles, bool allCore, bool keepWayIds, LineairRing poly)
        {
            _vehicles = vehicles;
            _contractionProfiles = contractionProfiles;
            _allCore = allCore;
            _map = map;
            _poly = poly;
        }

        private OsmStreamSource _source;

        /// <summary>
        /// Collapses the given list of processors by adding this one to it.
        /// </summary>
        public override int Collapse(List<ProcessorBase> processors, int i)
        {
            if (processors == null || processors.Count == 0) { throw new ArgumentOutOfRangeException("processors"); }
            if (i < 1) { throw new ArgumentOutOfRangeException("i"); }
            
            if (processors[i - 1] is ProcessorSource)
            { // ok there is a source, keep it around for execution.
                _source = (processors[i - 1] as ProcessorSource).Source;
            }
            processors.RemoveAt(i - 1);
            return -1;
        }

        /// <summary>
        /// Returns true if this processor is ready.
        /// </summary>
        public override bool IsReady
        { // a source is always ready
            get { return _source != null; }
        }

        /// <summary>
        /// Executes the tasks or commands in this processor.
        /// </summary>
        public override void Execute()
        { // this processor cannot be executed on it's own.
            throw new InvalidOperationException("This processor cannot be executed, check CanExecute before calling this method.");
        }

        /// <summary>
        /// Returns true if this processor can be executed.
        /// </summary>
        public override bool CanExecute
        { // this processor cannot be executed on it's own.
            get { return false; }
        }


        /// <summary>
        /// Gets the get router db function.
        /// </summary>
        /// <returns></returns>
        public Func<RouterDb> GetRouterDb()
        {
            return () =>
            {
                RouterDb routerDb = null;
                if(_map == null)
                {
                    routerDb = new RouterDb();
                }
                else
                {
                    routerDb = new RouterDb(_map);
                }

                // load the data.
                var target = new OsmSharp.Routing.Osm.Streams.RouterDbStreamTarget(routerDb,
                    _vehicles, _allCore);
                if (_keepWayIds)
                { // add way id's.
                    var eventsFilter = new OsmSharp.Osm.Streams.Filters.OsmStreamFilterWithEvents();
                    eventsFilter.MovedToNextEvent += EventsFilter_AddWayId;
                    eventsFilter.RegisterSource(_source);
                    target.RegisterSource(eventsFilter, false);
                }
                else
                { // use the source as-is.
                    target.RegisterSource(_source);
                }
                target.Pull();

                // sort the network.
                routerDb.Network.Sort();

                var sourceMeta = _source.GetAllMeta();
                sourceMeta.CopyToIfExists(routerDb.Meta, "poly");
                sourceMeta.CopyToIfExists(routerDb.Meta, "bbox");
                sourceMeta.CopyToIfExists(routerDb.Meta, "filename", "source_file");
                routerDb.Meta.Add("creation_date", DateTime.Now.ToInvariantString());

                if(_poly != null)
                { // override the existing poly definition.
                    routerDb.Meta.Add("poly", OsmSharp.Geo.Streams.GeoJson.GeoJsonConverter.ToGeoJson(_poly));
                }

                if(_contractionProfiles != null)
                {
                    for(var i = 0; i < _contractionProfiles.Length; i++)
                    {
                        routerDb.AddContracted(_contractionProfiles[i]);
                    }
                }

                // set processor meta.
                if(_allCore)
                {
                    this.Meta["all_core"] = true;
                    this.Meta["node_id_map"] = target.CoreNodeIdMap;
                }

                return routerDb;
            };
        }

        static OsmSharp.Osm.OsmGeo EventsFilter_AddWayId(OsmSharp.Osm.OsmGeo osmGeo, object param)
        {
            if (osmGeo.Type == OsmSharp.Osm.OsmGeoType.Way)
            {
                var tags = new TagsCollection(osmGeo.Tags);
                foreach (var tag in tags)
                {
                    if (tag.Key == "bridge")
                    {
                        continue;
                    }
                    if (tag.Key == "tunnel")
                    {
                        continue;
                    }
                    if (tag.Key == "lanes")
                    {
                        continue;
                    }
                    if (!Vehicle.Car.IsRelevant(tag.Key, tag.Value))
                    {
                        osmGeo.Tags.RemoveKeyValue(tag.Key, tag.Value);
                    }
                }

                osmGeo.Tags.Add("way_id", osmGeo.Id.ToString());
            }
            return osmGeo;
        }
    }
}
