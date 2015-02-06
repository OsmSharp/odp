using GeoAPI.Geometries;
using NetTopologySuite.Features;
using OsmSharp.Osm;
using OsmSharp.Osm.Geo.Interpreter;
using OsmSharp.Osm.Streams.Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsmSharpDataProcessor.NTS.Osm
{
    /// <summary>
    /// A stream that reads OSM-data and outputs NTS geometries based on a geometry interpreter.
    /// </summary>
    public class FeatureInterpreterStreamSource : IFeatureStreamSource
    {
        /// <summary>
        /// Holds the complete source.
        /// </summary>
        private readonly OsmCompleteStreamSource _completeSource;

        /// <summary>
        /// Holds the geometry interpreter.
        /// </summary>
        private readonly FeatureInterpreter _featureInterpreter;

        /// <summary>
        /// Creates a geometry interpreter stream source.
        /// </summary>
        /// <param name="completeSource"></param>
        /// <param name="featureInterpreter"></param>
        public FeatureInterpreterStreamSource(OsmCompleteStreamSource completeSource, FeatureInterpreter featureInterpreter)
        {
            _featureInterpreter = featureInterpreter;
            _completeSource = completeSource;

            _nextFeatures = new List<Feature>();
        }

        /// <summary>
        /// Initializes this stream source.
        /// </summary>
        public void Initialize()
        {
            _completeSource.Initialize();
        }

        /// <summary>
        /// Holds the list of next features.
        /// </summary>
        private List<Feature> _nextFeatures;

        /// <summary>
        /// Holds the current feature.
        /// </summary>
        private Feature _current;

        /// <summary>
        /// Returns the current feature.
        /// </summary>
        public Feature Current
        {
            get { return _current; }
        }

        /// <summary>
        /// Moves this source to the next feature.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            // first take from the next features list.
            if(_nextFeatures.Count > 0)
            { // there is one left!
                _current = _nextFeatures[_nextFeatures.Count - 1];
                _nextFeatures.RemoveAt(_nextFeatures.Count - 1);
                return true;
            }

            // move to the next feature.
            while(_completeSource.MoveNext())
            {
                // interpret the geometries.
                var features = _featureInterpreter.Interpret(_completeSource.Current());
                if(features != null && features.Count > 0)
                { // there are geometries found!
                    _nextFeatures = OsmSharpToNTSFeatureConvertor.Convert(features);
                    return this.MoveNext();
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        /// <returns></returns>
        public bool CanReset()
        {
            return _completeSource.CanReset;
        }

        /// <summary>
        /// Closes this source.
        /// </summary>
        public void Close()
        {

        }

        /// <summary>
        /// Returns true if this source has bounds.
        /// </summary>
        public bool HasBounds
        {
            get { return false; }
        }

        /// <summary>
        /// Returns the bounds of the data in this source.
        /// </summary>
        /// <returns></returns>
        public Envelope GetBounds()
        {
            throw new InvalidOperationException("No bounds available, check HasBounds");
        }

        /// <summary>
        /// Diposes all resources associated with this source.
        /// </summary>
        public void Dispose()
        {
            _completeSource.Dispose();
        }

        /// <summary>
        /// Returns the current geometry.
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        /// <summary>
        /// Resets this source.
        /// </summary>
        public void Reset()
        {
            _completeSource.Reset();
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Feature> GetEnumerator()
        {
            return this;
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
    }
}