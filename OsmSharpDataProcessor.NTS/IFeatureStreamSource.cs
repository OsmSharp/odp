using GeoAPI.Geometries;
using NetTopologySuite.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsmSharpDataProcessor.NTS
{
    /// <summary>
    /// Represents a source of geometry objects.
    /// </summary>
    public interface IFeatureStreamSource : IEnumerator<Feature>, IEnumerable<Feature>
    {
        /// <summary>
        /// Intializes this source.
        /// </summary>
        /// <rremarks>Has to be called before starting read objects.</rremarks>
        void Initialize();

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Some sources cannot be reset, live feeds of objects for example.</remarks>
        bool CanReset();

        /// <summary>
        /// Closes this target.
        /// </summary>
        /// <remarks>Closes any open connections, file locks or anything related to this source.</remarks>
        void Close();

        /// <summary>
        /// Returns true if this source is bounded.
        /// </summary>
        bool HasBounds
        {
            get;
        }

        /// <summary>
        /// Returns the bounds of this source.
        /// </summary>
        /// <returns></returns>
        Envelope GetBounds();
    }
}
