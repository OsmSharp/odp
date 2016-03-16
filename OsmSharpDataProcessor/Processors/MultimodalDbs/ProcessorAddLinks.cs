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

using OsmSharp.Routing;
using OsmSharp.Routing.Profiles;
using OsmSharp.Routing.Transit.Data;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharpDataProcessor.Processors.MultimodalDbs
{
    /// <summary>
    /// A routerdb processor source.
    /// </summary>
    public class ProcessorAddLink : ProcessorBase, IMultimodalDbSource
    {
        private readonly Profile _profile;

        /// <summary>
        /// Creates a new processor contract.
        /// </summary>
        public ProcessorAddLink(Profile profile)
        {
            _profile = profile;
        }

        private Func<MultimodalDb> _getMultimodalDb;
        
        /// <summary>
        /// Collapses the list of processors by trying to collapse this one.
        /// </summary>
        public override int Collapse(List<ProcessorBase> processors, int i)
        {
            if (processors == null || processors.Count == 0) { throw new ArgumentOutOfRangeException("processors"); }
            if (i < 1) { throw new ArgumentOutOfRangeException("i"); }
            
            if (processors[i - 1] is IMultimodalDbSource)
            { // ok there is a source, keep it around for execution.
                _getMultimodalDb = (processors[i - 1] as IMultimodalDbSource).GetMultimodalDb();
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
        /// Gets the get multimodal db function.
        /// </summary>
        /// <returns></returns>
        public Func<MultimodalDb> GetMultimodalDb()
        {
            return () =>
            {
                var db = _getMultimodalDb();
                
                OsmSharp.Logging.Log.TraceEvent("Processor - Add Links", OsmSharp.Logging.TraceEventType.Information,
                    "Adding stop links - for {0} ...", _profile.Name);
                db.AddStopLinksDb(_profile, maxRouterPoints: 4);
                return db;
            };
        }
    }
}