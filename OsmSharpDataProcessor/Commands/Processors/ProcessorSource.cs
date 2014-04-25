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

namespace OsmSharpDataProcessor.Commands.Processors
{
    /// <summary>
    /// Represents a processor that encapsulates a source or a task that depends on at least a filter/target to be executed.
    /// </summary>
    public class ProcessorSource : ProcessorBase
    {
        /// <summary>
        /// Holds the osm stream source.
        /// </summary>
        private OsmStreamSource _source;

        /// <summary>
        /// Creates a new processor source.
        /// </summary>
        /// <param name="source"></param>
        public ProcessorSource(OsmStreamSource source)
        {
            _source = source;
        }

        /// <summary>
        /// Collapses the given list of processors by adding this one to it.
        /// </summary>
        /// <param name="processors"></param>
        public override void Collapse(List<ProcessorBase> processors)
        {
            // just add to the list.
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
        /// Returns the source from this processor source.
        /// </summary>
        public virtual OsmStreamSource Source
        {
            get
            {
                return _source;
            }
        }
    }
}