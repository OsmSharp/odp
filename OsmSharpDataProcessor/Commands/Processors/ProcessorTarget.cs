// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharpDataProcessor.Commands.Processors
{
    /// <summary>
    /// Represents a processor that encapsulates a target.
    /// </summary>
    public class ProcessorTarget : ProcessorBase
    {
        /// <summary>
        /// Holds the stream target.
        /// </summary>
        private OsmStreamTarget _target;

        /// <summary>
        /// Holds the ready flag.
        /// </summary>
        private bool _isReady;

        /// <summary>
        /// Creates a new processor target.
        /// </summary>
        /// <param name="target"></param>
        public ProcessorTarget(OsmStreamTarget target)
        {
            _isReady = false;
            _target = target;
        }

        /// <summary>
        /// Collapses the given list of processors by adding this one to it.
        /// </summary>
        /// <param name="processors"></param>
        public override void Collapse(List<ProcessorBase> processors)
        {
            if (processors == null) { throw new ArgumentNullException("processors"); }
            if (processors.Count == 0) { throw new ArgumentOutOfRangeException("processors", "There has to be at least on processor there to collapse this target."); }
            if (processors[processors.Count - 1] == null) { throw new ArgumentOutOfRangeException("processors", "The last processor in the processors list is null."); }

            // take the last processor and collapse.
            if (processors[processors.Count - 1] is ProcessorSource)
            { // ok, processor is a source.
                var source = processors[processors.Count - 1] as ProcessorSource;
                if (!source.IsReady) { throw new InvalidOperationException("Last processor before filter is a source but it is not ready."); }
                processors.RemoveAt(processors.Count - 1);

                _target.RegisterSource(source.Source);
                _isReady = true;
                processors.Add(this);
                return;
            }
            throw new InvalidOperationException("Last processor before filter is not a source.");
        }

        /// <summary>
        /// Returns true if this processor is ready.
        /// </summary>
        public override bool IsReady
        {
            get { return _isReady; }
        }

        /// <summary>
        /// Executes the tasks or commands in this processor.
        /// </summary>
        public override void Execute()
        {
            if (!this.CanExecute) { throw new InvalidOperationException("Cannot execute processor target when it is not ready yet!"); }

            _target.Pull();
            _target.Close();
        }

        /// <summary>
        /// Returns true if this processor can be executed.
        /// </summary>
        public override bool CanExecute
        {
            get { return this.IsReady; }
        }
    }
}