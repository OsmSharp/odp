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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp.Routing.Transit.Data;
using System.IO;

namespace OsmSharpDataProcessor.Commands.Processors.TransitDbs
{
    /// <summary>
    /// A processor to read a transitdb.
    /// </summary>
    public class ProcessorRead : ProcessorBase, ITransitDbSource
    {
        private readonly string _file;

        /// <summary>
        /// Creates a new processor read.
        /// </summary>
        /// <param name="file"></param>
        public ProcessorRead(string file)
        {
            _file = file;
        }

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

        /// <summary>
        /// Collapses this processor read.
        /// </summary>
        /// <param name="processors"></param>
        public override void Collapse(List<ProcessorBase> processors)
        {
            processors.Add(this);
        }

        /// <summary>
        /// Executes the tasks or commands in this processor.
        /// </summary>
        public override void Execute()
        { // a source cannot be executed.
            throw new InvalidOperationException("This processor cannot be executed, check CanExecute before calling this method.");
        }

        /// <summary>
        /// Gets the transit db.
        /// </summary>
        /// <returns></returns>
        public Func<TransitDb> GetTransitDb()
        {
            return () =>
            {
                return TransitDb.Deserialize(
                    File.OpenRead(_file));
            };
        }
    }
}
