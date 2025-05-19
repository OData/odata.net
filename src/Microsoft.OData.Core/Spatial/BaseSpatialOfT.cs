//---------------------------------------------------------------------
// <copyright file="SpatialBaseOfT.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using NetTopologySuite;
using NtsCoordinate = NetTopologySuite.Geometries.Coordinate;
using NtsCoordinateZ = NetTopologySuite.Geometries.CoordinateZ;
using NtsCoordinateZM = NetTopologySuite.Geometries.CoordinateZM;
using NtsGeometry = NetTopologySuite.Geometries.Geometry;
using NtsGeometryCollection = NetTopologySuite.Geometries.GeometryCollection;
using NtsGeometryFactory = NetTopologySuite.Geometries.GeometryFactory;
using NtsLinearRing = NetTopologySuite.Geometries.LinearRing;
using NtsLineString = NetTopologySuite.Geometries.LineString;
using NtsMultiLineString = NetTopologySuite.Geometries.MultiLineString;
using NtsMultiPoint = NetTopologySuite.Geometries.MultiPoint;
using NtsMultiPolygon = NetTopologySuite.Geometries.MultiPolygon;
using NtsPoint = NetTopologySuite.Geometries.Point;
using NtsPolygon = NetTopologySuite.Geometries.Polygon;

namespace Microsoft.OData.Spatial
{
#pragma warning disable RS0016 // Add public types and members to the declared API
    public abstract class BaseSpatialOfT<T> where T : NtsGeometry
    {
        public T Geometry { get; }

        public int Srid => Geometry.SRID;

        protected BaseSpatialOfT(T geometry)
        {
            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry), "Geometry cannot be null.");
            }

            Geometry = geometry;
        }
    }

    public class Geography : BaseSpatialOfT<NtsGeometry>
    {
        public Geography(NtsGeometry geometry) : base(geometry) { }

        public static implicit operator NtsGeometry(Geography geography) => geography.Geometry;

        public override string ToString() => Geometry.ToString();

        public override bool Equals(object obj)
        {
            if (obj is Geography other)
            {
                return Geometry.EqualsExact(other.Geometry);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Geometry.GetHashCode();
        }
    }

    public class GeographyPoint : Geography
    {
        private readonly NtsPoint point;

        public GeographyPoint(NtsPoint point) : base(point)
        {
            this.point = point;
        }

        public double Latitude => point.Y;
        public double Longitude => point.X;
        public double? Elevation => double.IsNaN(point.Z) ? null : point.Z;
        public double? Measure => double.IsNaN(point.M) ? null : point.M;

        public static implicit operator NtsPoint(GeographyPoint geographyPoint) => geographyPoint.point;
        public static implicit operator NtsGeometry(GeographyPoint geographyPoint) => geographyPoint.point;
    }

    public class GeographyLineString : Geography
    {
        private readonly NtsLineString lineString;

        public GeographyLineString(NtsLineString lineString) : base(lineString)
        {
            this.lineString = lineString;
        }

        public static implicit operator NtsLineString(GeographyLineString geographyLineString) => geographyLineString.lineString;
        public static implicit operator NtsGeometry(GeographyLineString geographyLineString) => geographyLineString.lineString;
    }

    public class GeographyPolygon : Geography
    {
        private readonly NtsPolygon polygon;

        public GeographyPolygon(NtsPolygon polygon) : base(polygon)
        {
            this.polygon = polygon;
        }

        public static implicit operator NtsPolygon(GeographyPolygon geographyPolygon) => geographyPolygon.polygon;
        public static implicit operator NtsGeometry(GeographyPolygon geographyPolygon) => geographyPolygon.polygon;
    }

    public class GeographyMultiPoint : Geography
    {
        private readonly NtsMultiPoint multiPoint;

        public GeographyMultiPoint(NtsMultiPoint multiPoint) : base(multiPoint)
        {
            this.multiPoint = multiPoint;
        }

        public static implicit operator NtsMultiPoint(GeographyMultiPoint geographyMultiPoint) => geographyMultiPoint.multiPoint;
        public static implicit operator NtsGeometry(GeographyMultiPoint geographyMultiPoint) => geographyMultiPoint.multiPoint;
    }

    public class GeographyMultiLineString : Geography
    {
        private readonly NtsMultiLineString multiLineString;

        public GeographyMultiLineString(NtsMultiLineString multiLineString) : base(multiLineString)
        {
            this.multiLineString = multiLineString;
        }

        public static implicit operator NtsMultiLineString(GeographyMultiLineString geographyMultiLineString) => geographyMultiLineString.multiLineString;
        public static implicit operator NtsGeometry(GeographyMultiLineString geographyMultiLineString) => geographyMultiLineString.multiLineString;
    }

    public class GeographyMultiPolygon : Geography
    {
        private readonly NtsMultiPolygon multiPolygon;

        public GeographyMultiPolygon(NtsMultiPolygon multiPolygon) : base(multiPolygon)
        {
            this.multiPolygon = multiPolygon;
        }

        public static implicit operator NtsMultiPolygon(GeographyMultiPolygon geographyMultiPolygon) => geographyMultiPolygon.multiPolygon;
        public static implicit operator NtsGeometry(GeographyMultiPolygon geographyMultiPolygon) => geographyMultiPolygon.multiPolygon;
    }

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public class GeographyCollection : Geography
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        private readonly NtsGeometryCollection collection;

        public GeographyCollection(NtsGeometryCollection collection) : base(collection)
        {
            this.collection = collection;
        }

        public static implicit operator NtsGeometryCollection(GeographyCollection geographyCollection) => geographyCollection.collection;
        public static implicit operator NtsGeometry(GeographyCollection geographyCollection) => geographyCollection.collection;
    }

    public class Geometry : BaseSpatialOfT<NtsGeometry>
    {
        public Geometry(NtsGeometry geometry) : base(geometry) { }

        public static implicit operator NtsGeometry(Geometry geometry) => geometry.Geometry;

        public override bool Equals(object obj)
        {
            if (obj is Geometry other)
            {
                return Geometry.EqualsExact(other.Geometry);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Geometry.GetHashCode();
        }
    }

    public class GeometryPoint : Geometry
    {
        private readonly NtsPoint point;

        public GeometryPoint(NtsPoint point) : base(point)
        {
            this.point = point;
        }

        public double Latitude => point.Y;
        public double Longitude => point.X;
        public double? Elevation => double.IsNaN(point.Z) ? null : point.Z;
        public double? Measure => double.IsNaN(point.M) ? null : point.M;

        public static implicit operator NtsPoint(GeometryPoint geometryPoint) => geometryPoint.point;
        public static implicit operator NtsGeometry(GeometryPoint geometryPoint) => geometryPoint.point;
    }

    public class GeometryLineString : Geometry
    {
        private readonly NtsLineString lineString;

        public GeometryLineString(NtsLineString lineString) : base(lineString)
        {
            this.lineString = lineString;
        }

        public static implicit operator NtsLineString(GeometryLineString geometryLineString) => geometryLineString.lineString;
        public static implicit operator NtsGeometry(GeometryLineString geometryLineString) => geometryLineString.lineString;
    }

    public class GeometryPolygon : Geometry
    {
        private readonly NtsPolygon polygon;

        public GeometryPolygon(NtsPolygon polygon) : base(polygon)
        {
            this.polygon = polygon;
        }

        public static implicit operator NtsPolygon(GeometryPolygon geometryPolygon) => geometryPolygon.polygon;
        public static implicit operator NtsGeometry(GeometryPolygon geometryPolygon) => geometryPolygon.polygon;
    }

    public class GeometryMultiPoint : Geometry
    {
        private readonly NtsMultiPoint multiPoint;

        public GeometryMultiPoint(NtsMultiPoint multiPoint) : base(multiPoint)
        {
            this.multiPoint = multiPoint;
        }

        public static implicit operator NtsMultiPoint(GeometryMultiPoint geometryMultiPoint) => geometryMultiPoint.multiPoint;
        public static implicit operator NtsGeometry(GeometryMultiPoint geometryMultiPoint) => geometryMultiPoint.multiPoint;
    }

    public class GeometryMultiLineString : Geometry
    {
        private readonly NtsMultiLineString multiLineString;

        public GeometryMultiLineString(NtsMultiLineString multiLineString) : base(multiLineString)
        {
            this.multiLineString = multiLineString;
        }

        public static implicit operator NtsMultiLineString(GeometryMultiLineString geometryMultiLineString) => geometryMultiLineString.multiLineString;
        public static implicit operator NtsGeometry(GeometryMultiLineString geometryMultiLineString) => geometryMultiLineString.multiLineString;
    }

    public class GeometryMultiPolygon : Geometry
    {
        private readonly NtsMultiPolygon multiPolygon;

        public GeometryMultiPolygon(NtsMultiPolygon multiPolygon) : base(multiPolygon)
        {
            this.multiPolygon = multiPolygon;
        }

        public static implicit operator NtsMultiPolygon(GeometryMultiPolygon geometryMultiPolygon) => geometryMultiPolygon.multiPolygon;
        public static implicit operator NtsGeometry(GeometryMultiPolygon geometryMultiPolygon) => geometryMultiPolygon.multiPolygon;
    }

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public class GeometryCollection : Geometry
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        private readonly NtsGeometryCollection collection;

        public GeometryCollection(NtsGeometryCollection collection) : base(collection)
        {
            this.collection = collection;
        }

        public static implicit operator NtsGeometryCollection(GeometryCollection geometryCollection) => geometryCollection.collection;
        public static implicit operator NtsGeometry(GeometryCollection geometryCollection) => geometryCollection.collection;
    }

    /// <summary>
    /// Supports a set of factory methods for creating instances of <see cref="Geography"/> types.
    /// </summary>
    public class GeographyFactory
    {
        private readonly NtsGeometryFactory ntsGeometryFactory;

        private GeographyFactory(NtsGeometryFactory ntsGeometryFactory)
        {
            this.ntsGeometryFactory = ntsGeometryFactory;
        }

        public static GeographyFactory Default => new GeographyFactory(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));

        public static GeographyFactory Create(int srid)
        {
            return new GeographyFactory(NtsGeometryServices.Instance.CreateGeometryFactory(srid: srid));
        }

        /// <summary>
        /// Creates a <see cref="GeographyPoint"/> with the specified longitude and latitude.
        /// </summary>
        /// <param name="longitude">The longitude of the point.</param>
        /// <param name="latitude">The latitude of the point.</param>
        /// <returns>A <see cref="GeographyPoint"/> object.</returns>
        public GeographyPoint CreatePoint(double longitude, double latitude)
        {
            return CreatePoint(new NtsCoordinate(longitude, latitude));
        }

        /// <summary>
        /// Creates a <see cref="GeographyPoint"/> with the specified longitude, latitude, and elevation.
        /// </summary>
        /// <param name="longitude">The longitude of the point.</param>
        /// <param name="latitude">The latitude of the point.</param>
        /// <param name="elevation">The elevation of the point.</param>
        /// <returns>A <see cref="GeographyPoint"/> object.</returns>
        public GeographyPoint CreatePoint(double longitude, double latitude, double elevation)
        {
            return CreatePoint(new NtsCoordinateZ(longitude, latitude, elevation));
        }

        /// <summary>
        /// Creates a <see cref="GeographyPoint"/> with the specified longitude, latitude, elevation, and measure.
        /// </summary>
        /// <param name="longitude">The longitude of the point.</param>
        /// <param name="latitude">The latitude of the point.</param>
        /// <param name="elevation">The elevation of the point.</param>
        /// <param name="measure">The measure of the point.</param>
        /// <returns>A <see cref="GeographyPoint"/> object.</returns>
        public GeographyPoint CreatePoint(double longitude, double latitude, double elevation, double measure)
        {
            return CreatePoint(new NtsCoordinateZM(longitude, latitude, elevation, measure));
        }

        /// <summary>
        /// Creates a <see cref="GeographyPoint"/> from a <see cref="NtsCoordinate"/> object.
        /// </summary>
        /// <param name="coordinate">The coordinate to create the point from.</param>
        /// <returns>A <see cref="GeographyPoint"/> object.</returns>
        public GeographyPoint CreatePoint(NtsCoordinate coordinate)
        {
            return new GeographyPoint(ntsGeometryFactory.CreatePoint(coordinate));
        }


        public GeographyLineString CreateLineString(params NtsCoordinate[] coordinates)
        {
            return new GeographyLineString(ntsGeometryFactory.CreateLineString(coordinates));
        }

        /// <summary>
        /// Constructs a <see cref="GeographyPolygon"/> with the given exterior boundary.
        /// </summary>
        /// <param name="coordinates">The outer boundary of the new <see cref="GeographyPolygon"/>.</param>
        /// <returns>A <see cref="GeographyPolygon"/> object.</returns>
        public GeographyPolygon CreatePolygon(params NtsCoordinate[] coordinates)
        {
            return new GeographyPolygon(ntsGeometryFactory.CreatePolygon(coordinates));
        }

        public GeographyPolygon CreatePolygon(NtsCoordinate[] shell, NtsCoordinate[][] holes)
        {
            var shellRing = ntsGeometryFactory.CreateLinearRing(shell);
            var holeRings = new NtsLinearRing[holes.Length];
            for (int i = 0; i < holes.Length; i++)
            {
                holeRings[i] = ntsGeometryFactory.CreateLinearRing(holes[i]);
            }

            return new GeographyPolygon(ntsGeometryFactory.CreatePolygon(shellRing, holeRings));
        }

        // TODO: Determine if we should support overload that accepts LinearRing directly
        public GeographyPolygon CreatePolygon(NtsLinearRing shell, params NtsLinearRing[] holes)
        {
            return new GeographyPolygon(ntsGeometryFactory.CreatePolygon(shell, holes));
        }

        public GeographyMultiPoint CreateMultiPoint(params NtsPoint[] points)
        {
            return new GeographyMultiPoint(ntsGeometryFactory.CreateMultiPoint(points));
        }

        public GeographyMultiLineString CreateMultiLineString(params NtsLineString[] lineStrings)
        {
            return new GeographyMultiLineString(ntsGeometryFactory.CreateMultiLineString(lineStrings));
        }

        public GeographyMultiPolygon CreateMultiPolygon(params NtsPolygon[] polygons)
        {
            return new GeographyMultiPolygon(ntsGeometryFactory.CreateMultiPolygon(polygons));
        }

        public GeographyCollection CreateGeographyCollection()
        {
            return new GeographyCollection(ntsGeometryFactory.CreateGeometryCollection());
        }

        public GeographyCollection CreateGeographyCollection(params NtsGeometry[] geometries)
        {
            return new GeographyCollection(ntsGeometryFactory.CreateGeometryCollection(geometries));
        }

        // TODO: Add support for other empty geography types if needed
    }

    /// <summary>
    /// Supports a set of factory methods for creating instances of <see cref="Geometry"/> types.
    /// </summary>
    public class GeometryFactory
    {
        private readonly NtsGeometryFactory ntsGeometryFactory;

        private GeometryFactory(NtsGeometryFactory ntsGeometryFactory)
        {
            this.ntsGeometryFactory = ntsGeometryFactory;
        }

        public static GeometryFactory Default => new GeometryFactory(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 0));

        public static GeometryFactory Create(int srid)
        {
            return new GeometryFactory(NtsGeometryServices.Instance.CreateGeometryFactory(srid: srid));
        }

        public GeometryPoint CreatePoint(double longitude, double latitude)
        {
            return CreatePoint(new NtsCoordinate(longitude, latitude));
        }

        public GeometryPoint CreatePoint(double longitude, double latitude, double elevation)
        {
            return CreatePoint(new NtsCoordinateZ(longitude, latitude, elevation));
        }

        public GeometryPoint CreatePoint(double longitude, double latitude, double elevation, double measure)
        {
            return CreatePoint(new NtsCoordinateZM(longitude, latitude, elevation, measure));
        }

        public GeometryPoint CreatePoint(NtsCoordinate coordinate)
        {
            return new GeometryPoint(ntsGeometryFactory.CreatePoint(coordinate));
        }

        public GeometryLineString CreateLineString(params NtsCoordinate[] coordinates)
        {
            return new GeometryLineString(ntsGeometryFactory.CreateLineString(coordinates));
        }

        public GeometryPolygon CreatePolygon(params NtsCoordinate[] coordinates)
        {
            return new GeometryPolygon(ntsGeometryFactory.CreatePolygon(coordinates));
        }

        public GeometryPolygon CreatePolygon(NtsCoordinate[] shell, NtsCoordinate[][] holes)
        {
            var shellRing = ntsGeometryFactory.CreateLinearRing(shell);
            var holeRings = new NtsLinearRing[holes.Length];
            for (int i = 0; i < holes.Length; i++)
            {
                holeRings[i] = ntsGeometryFactory.CreateLinearRing(holes[i]);
            }

            return new GeometryPolygon(ntsGeometryFactory.CreatePolygon(shellRing, holeRings));
        }

        // TODO: Determine if we should support overload that accepts LinearRing directly
        public GeometryPolygon CreatePolygon(NtsLinearRing shell, params NtsLinearRing[] holes)
        {
            return new GeometryPolygon(ntsGeometryFactory.CreatePolygon(shell, holes));
        }

        public GeometryMultiPoint CreateMultiPoint(params NtsPoint[] points)
        {
            return new GeometryMultiPoint(ntsGeometryFactory.CreateMultiPoint(points));
        }

        public GeometryMultiLineString CreateMultiLineString(params NtsLineString[] lineStrings)
        {
            return new GeometryMultiLineString(ntsGeometryFactory.CreateMultiLineString(lineStrings));
        }

        public GeometryMultiPolygon CreateMultiPolygon(params NtsPolygon[] polygons)
        {
            return new GeometryMultiPolygon(ntsGeometryFactory.CreateMultiPolygon(polygons));
        }

        public GeometryCollection CreateGeometryCollection()
        {
            return new GeometryCollection(ntsGeometryFactory.CreateGeometryCollection());
        }

        public GeometryCollection CreateGeometryCollection(params NtsGeometry[] geometries)
        {
            return new GeometryCollection(ntsGeometryFactory.CreateGeometryCollection(geometries));
        }

        // TODO: Add support for other empty geometry types if needed
    }

    internal static class SpatialExtensions
    {
        /// <summary>
        /// Converts a <see cref="NtsGeometry"/> to a <see cref="Geography"/>.
        /// </summary>
        /// <param name="coordinate">The NetTopologySuite geometry instance to convert.</param>
        /// <returns>A new <see cref="Geography"/> instance.</returns>
        public static Geography ToGeography(this NtsGeometry geometry)
        {
            Debug.Assert(geometry != null, "Geometry cannot be null when converting to Geography.");

            return new Geography(geometry);
        }
        /// <summary>
        /// Converts a <see cref="NtsGeometry"/> to a <see cref="Geometry"/>.
        /// </summary>
        /// <param name="coordinate">The NetTopologySuite geometry instance to convert.</param>
        /// <returns>A new <see cref="Geometry"/> instance.</returns>
        public static Geometry ToGeometry(this NtsGeometry geometry)
        {
            Debug.Assert(geometry != null, "Geometry cannot be null when converting to Geography.");

            return new Geometry(geometry);
        }
    }

#pragma warning restore RS0016 // Add public types and members to the declared API

}
