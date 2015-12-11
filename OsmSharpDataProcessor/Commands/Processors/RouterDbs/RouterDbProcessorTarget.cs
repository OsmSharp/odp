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
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharpDataProcessor.Commands.Processors.RouterDbs
{
    /// <summary>
    /// A routerdb stream target.
    /// </summary>
    public class RouterDbProcessorTarget : ProcessorBase, IRouterDbSource
    {
        private readonly Stream _stream; // the target stream.
        private readonly string _fileName; // the filename.

        /// <summary>
        /// Creates a new processor target.
        /// </summary>
        public RouterDbProcessorTarget(Stream stream, string fileName)
        {
            _stream = stream;
            _fileName = fileName;
        }

        private Func<RouterDb> _getSourceDb;

        /// <summary>
        /// Collapses the given list of processors by adding this one to it.
        /// </summary>
        public override void Collapse(List<ProcessorBase> processors)
        {
            if (processors == null || processors.Count == 0) { throw new ArgumentOutOfRangeException(); }

            if(processors.Count > 1)
            { // cannot merge or write multiple router db's.
                throw new Exception("Cannot register multiple processors.");
            }
            if (processors[0] is IRouterDbSource)
            { // ok there is a source, keep it around for execution.
                _getSourceDb = (processors[0] as IRouterDbSource).GetRouterDb();
            }
            processors.RemoveAt(0);
            processors.Add(this);
        }

        /// <summary>
        /// Returns true if this processor is ready.
        /// </summary>
        public override bool IsReady
        { // a source is always ready
            get { return _getSourceDb != null; }
        }

        /// <summary>
        /// Executes the tasks or commands in this processor.
        /// </summary>
        public override void Execute()
        {
            this.GetRouterDb()();
        }

        /// <summary>
        /// Returns true if this processor can be executed.
        /// </summary>
        public override bool CanExecute
        { // a target can be executed.
            get { return true; }
        }

        /// <summary>
        /// Gets the function to get the router db.
        /// </summary>
        /// <returns></returns>
        public Func<RouterDb> GetRouterDb()
        {
            return () =>
                {
                    var db = _getSourceDb();

                    // add name-tag.
                    // the name is the part of the filename before the first '.'.
                    var i = _fileName.IndexOf('.');
                    if (i > 0)
                    {
                        db.Meta.Add("name", _fileName.Substring(0, i));
                    }

                    db.Serialize(_stream, true);

                    _stream.Flush();
                    _stream.Close();

                    return db;
                };
        }
    }
}