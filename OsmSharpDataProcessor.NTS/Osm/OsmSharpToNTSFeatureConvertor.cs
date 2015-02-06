using GeoAPI.Geometries;
using NetTopologySuite.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsmSharpDataProcessor.NTS.Osm
{
    /// <summary>
    /// Holds conversion methods to convert OsmSharp geometries to NTS geometries.
    /// </summary>
    public static class OsmSharpToNTSFeatureConvertor
    {
        /// <summary>
        /// Converts the given OsmSharp feature collection into a list of NTS feature collection.
        /// </summary>
        /// <param name="featureCollection"></param>
        /// <returns></returns>
        public static List<Feature> Convert(OsmSharp.Geo.Features.FeatureCollection featureCollection)
        {
            var features = new List<Feature>(featureCollection.Count);
            foreach (var feature in featureCollection)
            {
                features.Add(OsmSharpToNTSFeatureConvertor.Convert(feature));
            }
            return features;
        }

        /// <summary>
        /// Converts the given OsmSharp feature into an NTS feature.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static Feature Convert(OsmSharp.Geo.Features.Feature feature)
        {
            if (feature == null) { throw new ArgumentNullException("feature"); }

            var geometryFactory = new NetTopologySuite.Geometries.GeometryFactory();
            if(feature.Geometry is OsmSharp.Geo.Geometries.Polygon)
            { // a polygon.
                var polygon = (feature.Geometry as OsmSharp.Geo.Geometries.Polygon);
                var holes = polygon.Holes.Select((hole) => {
                    return (ILinearRing)geometryFactory.CreateLinearRing(hole.Coordinates.Select((coordinate) => {
                        return new Coordinate(coordinate.Longitude, coordinate.Latitude);
                    }).ToArray());
                }).ToArray();
                var shell = geometryFactory.CreateLinearRing(polygon.Ring.Coordinates.Select((coordinate) => {
                    return new Coordinate(coordinate.Longitude, coordinate.Latitude);
                }).ToArray());
                return new Feature(geometryFactory.CreatePolygon(shell, holes),
                    OsmSharpToNTSFeatureConvertor.Convert(feature.Attributes));
            }
            else if (feature.Geometry is OsmSharp.Geo.Geometries.LineairRing)
            { // a lineair ring.
                var lineairRing = (feature.Geometry as OsmSharp.Geo.Geometries.LineairRing);
                var coordinates = lineairRing.Coordinates.Select((coordinate) => {
                    return new Coordinate(coordinate.Longitude, coordinate.Latitude);
                });
                return new Feature(geometryFactory.CreateLinearRing(coordinates.ToArray()),
                    OsmSharpToNTSFeatureConvertor.Convert(feature.Attributes));
            }
            else if (feature.Geometry is OsmSharp.Geo.Geometries.LineString)
            { // a line string.
                var lineString = (feature.Geometry as OsmSharp.Geo.Geometries.LineString);
                var coordinates = lineString.Coordinates.Select((coordinate) =>
                {
                    return new Coordinate(coordinate.Longitude, coordinate.Latitude);
                });
                return new Feature(geometryFactory.CreateLineString(coordinates.ToArray()),
                    OsmSharpToNTSFeatureConvertor.Convert(feature.Attributes));
            }
            else if (feature.Geometry is OsmSharp.Geo.Geometries.Point)
            { // a point.
                var point = (feature.Geometry as OsmSharp.Geo.Geometries.Point);
                return new Feature(geometryFactory.CreatePoint(new Coordinate(point.Coordinate.Longitude, point.Coordinate.Latitude)),
                    OsmSharpToNTSFeatureConvertor.Convert(feature.Attributes));
            }
            else if (feature.Geometry is OsmSharp.Geo.Geometries.MultiLineString)
            { // a multi line string.
                throw new NotSupportedException("A MultiLineString is not supported.");
            }
            else if (feature.Geometry is OsmSharp.Geo.Geometries.MultiPoint)
            { // a multi point.
                throw new NotSupportedException("A MultiPoint is not supported.");
            }
            else if (feature.Geometry is OsmSharp.Geo.Geometries.MultiPolygon)
            { // a multi polygon.
                throw new NotSupportedException("A MultiPolygon is not supported.");
            }
            throw new ArgumentOutOfRangeException("Geometry not recognized: {0}", feature.ToString());
        }

        /// <summary>
        /// Converts the attributes tables.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static IAttributesTable Convert(OsmSharp.Geo.Attributes.GeometryAttributeCollection attributes)
        {
            var attributesTable = new AttributesTable();
            foreach(var attribute in attributes)
            {
                attributesTable.AddAttribute(attribute.Key, attribute.Value);
            }
            return attributesTable;
        }
    }
}
