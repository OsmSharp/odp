﻿// OsmSharp - OpenStreetMap (OSM) SDK
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
using System.Threading.Tasks;

namespace OsmSharpDataProcessor.Processors.RouterDbs
{
    /// <summary>
    /// A routerdb processor source.
    /// </summary>
    public class RouterDbProcessorContract : ProcessorBase, IRouterDbSource
    {
        private readonly List<Profile> _profiles;

        /// <summary>
        /// Creates a new processor contract.
        /// </summary>
        public RouterDbProcessorContract(List<Profile> profiles)
        {
            _profiles = profiles;
        }

        private Func<RouterDb> _getSourceDb;

        /// <summary>
        /// Collapses the given list of processors by adding this one to it.
        /// </summary>
        public override int Collapse(List<ProcessorBase> processors, int i)
        {
            if (processors == null || processors.Count == 0) { throw new ArgumentOutOfRangeException(); }
            if (i < 1) { throw new ArgumentOutOfRangeException("i"); }
            
            if (processors[i - 1] is IRouterDbSource)
            { // ok there is a source, keep it around for execution.
                _getSourceDb = (processors[i - 1] as IRouterDbSource).GetRouterDb();
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
        /// Gets the get router db function.
        /// </summary>
        /// <returns></returns>
        public Func<RouterDb> GetRouterDb()
        {
            return () =>
            {
                var db = _getSourceDb();

                // execute all tasks.
                var tasks = new List<Task>(_profiles.Count);
                foreach(var profile in _profiles)
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        db.AddContracted(profile);
                    }));
                }
                Task.WaitAll(tasks.ToArray());
                
                return db;
            };
        }
    }
}