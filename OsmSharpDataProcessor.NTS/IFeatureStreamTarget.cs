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
    /// Represents a geometry target accepting geometry objects for processing.
    /// </summary>
    public interface IFeatureStreamTarget
    {
        /// <summary>
        /// Intializes this target.
        /// </summary>
        /// <rremarks>Has to be called before starting to add objects.</rremarks>
        void Initialize();

        /// <summary>
        /// Register a source.
        /// </summary>
        /// <param name="source"></param>
        void RegisterSource(IFeatureStreamSource source);

        /// <summary>
        /// Adds a new feature.
        /// </summary>
        /// <param name="feature"></param>
        void AddFeature(Feature feature);

        /// <summary>
        /// Closes this target.
        /// </summary>
        /// <remarks>Closes any open connections, file locks or anything related to this target.</remarks>
        void Close();
    }
}
