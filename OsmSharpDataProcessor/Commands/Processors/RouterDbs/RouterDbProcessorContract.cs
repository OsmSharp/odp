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

using OsmSharp.Routing;
using OsmSharp.Routing.Profiles;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharpDataProcessor.Commands.Processors.RouterDbs
{
    /// <summary>
    /// A routerdb processor source.
    /// </summary>
    public class RouterDbProcessorContract : ProcessorBase
    {
        private readonly Profile _profile;

        /// <summary>
        /// Creates a new processor contract.
        /// </summary>
        public RouterDbProcessorContract(Profile profile)
        {
            _profile = profile;
        }

        private Func<RouterDb> _getSourceDb;

        /// <summary>
        /// Collapses the given list of processors by adding this one to it.
        /// </summary>
        public override void Collapse(List<ProcessorBase> processors)
        {
            if (processors == null || processors.Count == 0) { throw new ArgumentOutOfRangeException(); }

            if (processors.Count > 1)
            { // cannot merge or write multiple router db's.
                throw new Exception("Cannot register multiple processors.");
            }
            if (processors[0] is RouterDbProcessorSource)
            { // ok there is a source, keep it around for execution.
                _getSourceDb = (processors[0] as RouterDbProcessorSource).GetRouterDb();
            }
            if (processors[0] is ProcessorCreateRouterDb)
            { // ok there is a source, keep it around for execution.
                _getSourceDb = (processors[0] as ProcessorCreateRouterDb).GetRouterDb();
            }
            if (processors[0] is RouterDbProcessorContract)
            { // ok there is a source, keep it around for execution.
                _getSourceDb = (processors[0] as RouterDbProcessorContract).GetRouterDb();
            }
            processors.RemoveAt(0);
            processors.Add(this);
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
        /// Gets the get router db function.
        /// </summary>
        /// <returns></returns>
        public Func<RouterDb> GetRouterDb()
        {
            return () =>
            {
                var db = _getSourceDb();
                db.AddContracted(_profile);
                return db;
            };
        }
    }
}