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

namespace OsmSharpDataProcessor.Processors.RouterDbs.Shape
{
    /// <summary>
    /// The functional road class (FRC) of a line is a road classification based on the importance of the road.
    /// </summary>
    public enum FunctionalRoadClass
    {
        /// <summary>
        /// Main road
        /// </summary>
        Frc0 = 0,
        /// <summary>
        /// First class road
        /// </summary>
        Frc1 = 1,
        /// <summary>
        /// Second class road
        /// </summary>
        Frc2 = 2,
        /// <summary>
        /// Third class road
        /// </summary>
        Frc3 = 3,
        /// <summary>
        /// Fourth class road
        /// </summary>
        Frc4 = 4,
        /// <summary>
        /// Fifth class road
        /// </summary>
        Frc5 = 5,
        /// <summary>
        /// Sixth class road
        /// </summary>
        Frc6 = 6,
        /// <summary>
        /// Other class road
        /// </summary>
        Frc7 = 7
    }
}