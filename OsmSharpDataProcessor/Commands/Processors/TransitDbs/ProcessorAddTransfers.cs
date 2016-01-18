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
using OsmSharp.Routing.Profiles;
using OsmSharp.Routing.Transit.Data;
using OsmSharp.Collections.Tags;

namespace OsmSharpDataProcessor.Commands.Processors.TransitDbs
{
    /// <summary>
    /// A processor to add transfers.
    /// </summary>
    public class ProcessorAddTransfers : ProcessorBase, ITransitDbSource
    {
        private readonly Profile _profile;
        private readonly float _seconds;

        /// <summary>
        /// Creates a processor to add transfers.
        /// </summary>
        public ProcessorAddTransfers(Profile profile, float seconds)
        {
            _profile = profile;
            _seconds = seconds;
        }

        private Func<TransitDb> _getTransitDb;

        /// <summary>
        /// Can never execute on it's own.
        /// </summary>
        public override bool CanExecute
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Is always ready.
        /// </summary>
        public override bool IsReady
        {
            get
            {
                return true;
            }
        }

        public override void Execute()
        {

        }

        /// <summary>
        /// Collapses this processor if possible.
        /// </summary>
        public override void Collapse(List<ProcessorBase> processors)
        {
            if (processors == null) { throw new ArgumentNullException("processors"); }
            if (processors.Count == 0) { throw new ArgumentOutOfRangeException("processors", "There has to be at least on processor there to collapse this target."); }
            if (processors[processors.Count - 1] == null) { throw new ArgumentOutOfRangeException("processors", "The last processor in the processors list is null."); }

            // take the last processor and collapse.
            if (processors[processors.Count - 1] is ITransitDbSource)
            { // ok, processor is a source.
                var source = processors[processors.Count - 1] as ITransitDbSource;
                processors.RemoveAt(processors.Count - 1);
                _getTransitDb = source.GetTransitDb();
                processors.Add(this);
                return;
            }
            throw new InvalidOperationException("Last processor before filter is not a source.");
        }

        /// <summary>
        /// Gets the function to get the transit db.
        /// </summary>
        /// <returns></returns>
        public Func<TransitDb> GetTransitDb()
        {
            return () =>
            {
                var db = _getTransitDb();

                db.AddTransfersDb(_profile, new TagsCollection(Tag.Create("highway", "residential")), _seconds);

                return db;
            };
        }
    }
}
