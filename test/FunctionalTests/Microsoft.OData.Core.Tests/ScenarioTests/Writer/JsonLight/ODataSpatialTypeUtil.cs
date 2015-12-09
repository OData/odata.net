//---------------------------------------------------------------------
// <copyright file="ODataSpatialTypeUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.Spatial;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Writer.JsonLight
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
            GeometryValue = GeometryFactory.Point(32.0, -10.0).Build();
            GeometryPointValue = GeometryFactory.Point(33.1, -11.0).Build();
            GeometryLineStringValue = GeometryFactory.LineString(33.1, -11.5).LineTo(35.97, -11).Build();
            GeometryPolygonValue = GeometryFactory.Polygon().Ring(33.1, -13.6).LineTo(35.97, -11.15).LineTo(11.45, 87.75).Ring(35.97, -11).LineTo(36.97, -11.15).LineTo(45.23, 23.18).Build();
            GeometryCollectionValue = GeometryFactory.Collection().Point(-19.99, -12.0).Build();
            GeometryMultiPointValue = GeometryFactory.MultiPoint().Point(10.2, 11.2).Point(11.9, 11.6).Build();
            GeometryMultiLineStringValue = GeometryFactory.MultiLineString().LineString(10.2, 11.2).LineTo(11.9, 11.6).LineString(16.2, 17.2).LineTo(18.9, 19.6).Build();
            GeometryMultiPolygonValue = GeometryFactory.MultiPolygon().Polygon().Ring(10.2, 11.2).LineTo(11.9, 11.6).LineTo(11.45, 87.75).Ring(16.2, 17.2).LineTo(18.9, 19.6).LineTo(11.45, 87.75).Build();

            // Geography type values.
            GeographyValue = GeographyFactory.Point(32.0, -100.0).Build();
            GeographyPointValue = GeographyFactory.Point(33.1, -110.0).Build();
            GeographyLineStringValue = GeographyFactory.LineString(33.1, -110.0).LineTo(35.97, -110).Build();
            GeographyPolygonValue = GeographyFactory.Polygon().Ring(33.1, -110.0).LineTo(35.97, -110.15).LineTo(11.45, 87.75).Ring(35.97, -110).LineTo(36.97, -110.15).LineTo(45.23, 23.18).Build();
            GeographyCollectionValue = GeographyFactory.Collection().Point(-19.99, -12.0).Build();
            GeographyMultiPointValue = GeographyFactory.MultiPoint().Point(10.2, 11.2).Point(11.9, 11.6).Build();
            GeographyMultiLineStringValue = GeographyFactory.MultiLineString().LineString(10.2, 11.2).LineTo(11.9, 11.6).LineString(16.2, 17.2).LineTo(18.9, 19.6).Build();
            GeographyMultiPolygonValue = GeographyFactory.MultiPolygon().Polygon().Ring(10.2, 11.2).LineTo(11.9, 11.6).LineTo(11.45, 87.75).Ring(16.2, 17.2).LineTo(18.9, 19.6).LineTo(11.45, 87.75).Build();
        }

        #endregion
    }
}
