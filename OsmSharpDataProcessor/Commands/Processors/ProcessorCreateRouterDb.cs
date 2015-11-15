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

using OsmSharp.Osm.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.Osm;
using OsmSharp.Routing.Osm.Vehicles;
using OsmSharp.Routing.Profiles;
using Reminiscence.IO;
using System;
using System.Collections.Generic;

namespace OsmSharpDataProcessor.Commands.Processors
{
    /// <summary>
    /// A processor to create a router db.
    /// </summary>
    class ProcessorCreateRouterDb : ProcessorBase
    {
        private readonly Vehicle[] _vehicles;
        private readonly Profile[] _contractionProfiles;
        private readonly MemoryMap _map;

        /// <summary>
        /// Creates a new processor.
        /// </summary>
        public ProcessorCreateRouterDb(Vehicle[] vehicles, 
            Profile[] contractionProfiles)
        {
            _vehicles = vehicles;
            _contractionProfiles = contractionProfiles;
            _map = null;
        }

        /// <summary>
        /// Creates a new processor.
        /// </summary>
        public ProcessorCreateRouterDb(MemoryMap map, Vehicle[] vehicles,
            Profile[] contractionProfiles)
        {
            _vehicles = vehicles;
            _contractionProfiles = contractionProfiles;
            _map = map;
        }

        private OsmStreamSource _source;

        /// <summary>
        /// Collapses the given list of processors by adding this one to it.
        /// </summary>
        public override void Collapse(List<ProcessorBase> processors)
        {
            if (processors == null || processors.Count == 0) { throw new ArgumentOutOfRangeException(); }

            if (processors.Count > 1)
            { // cannot read from multiple streams.
                throw new Exception("Cannot register multiple processors as a source of the create router db processor.");
            }
            if (processors[0] is ProcessorSource)
            { // ok there is a source, keep it around for execution.
                _source = (processors[0] as ProcessorSource).Source;
            }
            processors.RemoveAt(0);
            processors.Add(this);
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
                var routerDb = new RouterDb();
                routerDb.LoadOsmData(_source, _vehicles);

                if(_contractionProfiles != null)
                {
                    for(var i = 0; i < _contractionProfiles.Length; i++)
                    {
                        routerDb.AddContracted(_contractionProfiles[i]);
                    }
                }
                return routerDb;
            };
        }
    }
}
