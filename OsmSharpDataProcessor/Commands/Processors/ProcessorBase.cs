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

using System.Collections.Generic;

namespace OsmSharpDataProcessor.Commands.Processors
{
    /// <summary>
    /// Base class for processor created by a command.
    /// </summary>
    public abstract class ProcessorBase
    {
        /// <summary>
        /// Collapses the given list of processors by adding this one to it.
        /// </summary>
        /// <param name="processors"></param>
        /// <remarks>This means for example taking a source and filter and converting them to a new source.</remarks>
        public abstract void Collapse(List<ProcessorBase> processors);

        /// <summary>
        /// Returns true if this processor is ready.
        /// </summary>
        /// <remarks>This means this is either a native source, a filter that has been collapsed and has a source, or... in short this processor is ready for business.</remarks>
        public abstract bool IsReady
        {
            get;
        }

        /// <summary>
        /// Executes the tasks or commands in this processor.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Returns true if this processor can be executed.
        /// </summary>
        /// <remarks>Different from IsReady. A source for example can be ready but cannot be executed by itself.</remarks>
        public abstract bool CanExecute
        {
            get;
        }
    }
}