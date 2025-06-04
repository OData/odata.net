//---------------------------------------------------------------------
// <copyright file="ODataSpatialTypeUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Spatial;
using Coordinate = NetTopologySuite.Geometries.Coordinate;

namespace Microsoft.OData.Tests.ScenarioTests.Writer.Json
{
    /// <summary>
    /// Common OData Spatial Type Utility class.
    /// </summary>
    public class ODataSpatialTypeUtil
    {
        #region Geography & Geometry type values

        /// <summary>Default Geography type values.</summary>
        public static readonly Geography GeographyValue;
        public static readonly GeographyPoint GeographyPointValue;
        public static readonly GeographyLineString GeographyLineStringValue;
        public static readonly GeographyPolygon GeographyPolygonValue;
        public static readonly GeographyCollection GeographyCollectionValue;
        public static readonly GeographyMultiPoint GeographyMultiPointValue;
        public static readonly GeographyMultiLineString GeographyMultiLineStringValue;
        public static readonly GeographyMultiPolygon GeographyMultiPolygonValue;

        /// <summary>Default Geometry type values.</summary>
        public static readonly Geometry GeometryValue;
        public static readonly GeometryPoint GeometryPointValue;
        public static readonly GeometryLineString GeometryLineStringValue;
        public static readonly GeometryPolygon GeometryPolygonValue;
        public static readonly GeometryCollection GeometryCollectionValue;
        public static readonly GeometryMultiPoint GeometryMultiPointValue;
        public static readonly GeometryMultiLineString GeometryMultiLineStringValue;
        public static readonly GeometryMultiPolygon GeometryMultiPolygonValue;

        static ODataSpatialTypeUtil()
        {
            // Geometry type values.
            GeometryValue = GeometryFactory.Default.CreatePoint(new Coordinate(-10.0, 32.0));
            GeometryPointValue = GeometryFactory.Default.CreatePoint(new Coordinate(-11.0, 33.1));
            GeometryLineStringValue = GeometryFactory.Default.CreateLineString([new Coordinate(-11.5, 33.1), new Coordinate(35.97, -11)]);
            GeometryPolygonValue = GeometryFactory.Default.CreatePolygon(
                [new Coordinate(33.1, -13.6), new Coordinate(35.97, -11.15), new Coordinate(11.45, 87.75), new Coordinate(33.1, -13.6)],
                [[new Coordinate(-11, 35.97), new Coordinate(-11.15, 36.97), new Coordinate(23.18, 45.23), new Coordinate(-11, 35.97)]]);
            GeometryCollectionValue = GeometryFactory.Default.CreateGeometryCollection([
                GeometryFactory.Default.CreatePoint(new Coordinate(-12.0, -19.99))]);
            GeometryMultiPointValue = GeometryFactory.Default.CreateMultiPoint([
                GeometryFactory.Default.CreatePoint(new Coordinate(11.2, 10.2)),
                GeometryFactory.Default.CreatePoint(new Coordinate(11.6, 11.9))]);
            GeometryMultiLineStringValue = GeometryFactory.Default.CreateMultiLineString([
                GeometryFactory.Default.CreateLineString([new Coordinate(11.2, 10.2), new Coordinate(11.6, 11.9)]),
                GeometryFactory.Default.CreateLineString([new Coordinate(17.2, 16.2), new Coordinate(19.6, 18.9)])]);
            GeometryMultiPolygonValue = GeometryFactory.Default.CreateMultiPolygon([
                GeometryFactory.Default.CreatePolygon(
                    [new Coordinate(11.2, 10.2), new Coordinate(11.6, 11.9), new Coordinate(87.75, 11.45), new Coordinate(11.2, 10.2)],
                    [[new Coordinate(17.2, 16.2), new Coordinate(19.6, 18.9), new Coordinate(87.75, 11.45), new Coordinate(17.2, 16.2)]])]);


            // Geography type values.
            GeographyValue = GeographyFactory.Default.CreatePoint(new Coordinate(-100.0, 32.0));
            GeographyPointValue = GeographyFactory.Default.CreatePoint(new Coordinate(-110.0, 33.1));
            GeographyLineStringValue = GeographyFactory.Default.CreateLineString([new Coordinate(-110.0, 33.1), new Coordinate(-110.0, 35.97)]);
            GeographyPolygonValue = GeographyFactory.Default.CreatePolygon(
                [new Coordinate(-110.0, 33.1), new Coordinate(-110.15, 35.97), new Coordinate(87.75, 11.45), new Coordinate(-110.0, 33.1)]);
            GeographyCollectionValue = GeographyFactory.Default.CreateGeographyCollection([
                GeographyFactory.Default.CreatePoint(new Coordinate(-12.0, -19.99))]);
            GeographyMultiPointValue = GeographyFactory.Default.CreateMultiPoint([
                GeographyFactory.Default.CreatePoint(new Coordinate(11.2, 10.2)),
                GeographyFactory.Default.CreatePoint(new Coordinate(11.6, 11.9))]);
            GeographyMultiLineStringValue = GeographyFactory.Default.CreateMultiLineString([
                GeographyFactory.Default.CreateLineString([new Coordinate(11.2, 10.2), new Coordinate(11.6, 11.9)]),
                GeographyFactory.Default.CreateLineString([new Coordinate(17.2, 16.2), new Coordinate(19.6, 18.9)])]);
            GeographyMultiPolygonValue = GeographyFactory.Default.CreateMultiPolygon([
                GeographyFactory.Default.CreatePolygon(
                    [new Coordinate(11.2, 10.2), new Coordinate(11.6, 11.9), new Coordinate(87.75, 11.45), new Coordinate(11.2, 10.2)],
                    [[new Coordinate(17.2, 16.2), new Coordinate(19.6, 18.9), new Coordinate(87.75, 11.45), new Coordinate(17.2, 16.2)]])
            ]);
        }

        #endregion
    }
}
