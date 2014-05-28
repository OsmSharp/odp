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
        /// Converts the given OsmSharp geometry collection into a list of NTS geometry collection.
        /// </summary>
        /// <param name="geometryCollection"></param>
        /// <returns></returns>
        public static List<Feature> Convert(OsmSharp.Geo.Geometries.GeometryCollection geometryCollection)
        {
            var geometries = new List<Feature>(geometryCollection.Count);
            foreach(var geometry in geometryCollection)
            {
                geometries.Add(OsmSharpToNTSFeatureConvertor.Convert(geometry));
            }
            return geometries;
        }

        /// <summary>
        /// Converts the given OsmSharp geometry into an NTS feature.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static Feature Convert(OsmSharp.Geo.Geometries.Geometry geometry)
        {
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

            var geometryFactory = new NetTopologySuite.Geometries.GeometryFactory();
            if(geometry is OsmSharp.Geo.Geometries.Polygon)
            { // a polygon.
                var polygon = (geometry as OsmSharp.Geo.Geometries.Polygon);
                var holes = polygon.Holes.Select((hole) => {
                    return (ILinearRing)geometryFactory.CreateLinearRing(hole.Coordinates.Select((coordinate) => {
                        return new Coordinate(coordinate.Longitude, coordinate.Latitude);
                    }).ToArray());
                }).ToArray();
                var shell = geometryFactory.CreateLinearRing(polygon.Ring.Coordinates.Select((coordinate) => {
                    return new Coordinate(coordinate.Longitude, coordinate.Latitude);
                }).ToArray());
                return new Feature(geometryFactory.CreatePolygon(shell, holes),
                    OsmSharpToNTSFeatureConvertor.Convert(geometry.Attributes));
            }
            else if (geometry is OsmSharp.Geo.Geometries.LineairRing)
            { // a lineair ring.
                var lineairRing = (geometry as OsmSharp.Geo.Geometries.LineairRing);
                var coordinates = lineairRing.Coordinates.Select((coordinate) => {
                    return new Coordinate(coordinate.Longitude, coordinate.Latitude);
                });
                return new Feature(geometryFactory.CreateLinearRing(coordinates.ToArray()),
                    OsmSharpToNTSFeatureConvertor.Convert(geometry.Attributes));
            }
            else if (geometry is OsmSharp.Geo.Geometries.LineString)
            { // a line string.
                var lineString = (geometry as OsmSharp.Geo.Geometries.LineString);
                var coordinates = lineString.Coordinates.Select((coordinate) =>
                {
                    return new Coordinate(coordinate.Longitude, coordinate.Latitude);
                });
                return new Feature(geometryFactory.CreateLineString(coordinates.ToArray()),
                    OsmSharpToNTSFeatureConvertor.Convert(geometry.Attributes));
            }
            else if (geometry is OsmSharp.Geo.Geometries.Point)
            { // a point.
                var point = (geometry as OsmSharp.Geo.Geometries.Point);
                return new Feature(geometryFactory.CreatePoint(new Coordinate(point.Coordinate.Longitude, point.Coordinate.Latitude)),
                    OsmSharpToNTSFeatureConvertor.Convert(geometry.Attributes));
            }
            else if (geometry is OsmSharp.Geo.Geometries.MultiLineString)
            { // a multi line string.
                throw new NotSupportedException("A MultiLineString is not supported.");
            }
            else if (geometry is OsmSharp.Geo.Geometries.MultiPoint)
            { // a multi point.
                throw new NotSupportedException("A MultiPoint is not supported.");
            }
            else if (geometry is OsmSharp.Geo.Geometries.MultiPolygon)
            { // a multi polygon.
                throw new NotSupportedException("A MultiPolygon is not supported.");
            }
            throw new ArgumentOutOfRangeException("Geometry not recognized: {0}", geometry.ToString());
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
