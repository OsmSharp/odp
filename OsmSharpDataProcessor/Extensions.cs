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

using OsmSharp.Collections.Tags;
namespace OsmSharpDataProcessor
{
    /// <summary>
    /// Contains common extensions methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Copies the key and value to the target tags collection if the given key exists.
        /// </summary>
        /// <returns>True if the key exists, false otherwise.</returns>
        public static bool CopyToIfExists(this TagsCollectionBase tags, TagsCollectionBase target, string key)
        {
            return tags.CopyToIfExists(target, key, key);
        }

        /// <summary>
        /// Copies the value using the new key to the target tags collection if the given key exists.
        /// </summary>
        /// <returns>True if the key exists, false otherwise.</returns>
        public static bool CopyToIfExists(this TagsCollectionBase tags, TagsCollectionBase target, string key, string newKey)
        {
            string value;
            if (tags.TryGetValue(key, out value))
            {
                target.Add(newKey, value);
                return true;
            }
            return false;
        }
    }
}