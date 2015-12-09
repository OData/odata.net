//---------------------------------------------------------------------
// <copyright file="SpatialValidatorImplementationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.Data.Spatial;
using Microsoft.Spatial.Tests;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class SpatialValidatorImplementationTests
    {
        private static readonly CoordinateSystem NonDefaultGeographicCoords = CoordinateSystem.Geography(1234);
        private static readonly CoordinateSystem NonDefaultGeometricCoords = CoordinateSystem.Geometry(1234);

        [Fact]
        public void ValidateLatitude_Geography()
        {
            GeographyPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeographicCoords);
            v.BeginGeography(SpatialType.Point);

            var ex = SpatialTestUtils.RunCatching<FormatException>(() => v.BeginFigure(new GeographyPosition(-91, 0, 0, 0)));
            Assert.NotNull(ex);
            Assert.Equal(Strings.Validator_InvalidLatitudeCoordinate(-91), ex.Message);

            v.Reset();
            v.SetCoordinateSystem(NonDefaultGeographicCoords);
            v.BeginGeography(SpatialType.Point);

            ex = SpatialTestUtils.RunCatching<FormatException>(() => v.BeginFigure(new GeographyPosition(91, 0, 0, 0)));
            Assert.NotNull(ex);
            Assert.Equal(Strings.Validator_InvalidLatitudeCoordinate(91), ex.Message);
        }

        [Fact]
        public void ValidateLongitude_Geography()
        {
            GeographyPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeographicCoords);
            v.BeginGeography(SpatialType.Point);

            var ex = SpatialTestUtils.RunCatching<FormatException>(() => v.BeginFigure(new GeographyPosition(0, -15070, 0, 0)));
            Assert.NotNull(ex);
            Assert.Equal(Strings.Validator_InvalidLongitudeCoordinate(-15070), ex.Message);

            v.Reset();
            v.SetCoordinateSystem(NonDefaultGeographicCoords);
            v.BeginGeography(SpatialType.Point);

            ex = SpatialTestUtils.RunCatching<FormatException>(() => v.BeginFigure(new GeographyPosition(0, 15070, 0, 0)));
            Assert.NotNull(ex);
            Assert.Equal(Strings.Validator_InvalidLongitudeCoordinate(15070), ex.Message);
        }

        [Fact]
        public void ValidatePolygonRing_LessThanFour_Geography()
        {
            GeographyPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeographicCoords);
            v.BeginGeography(SpatialType.Polygon);
            v.BeginFigure(new GeographyPosition(10, 20, 30, 40));
            v.LineTo(new GeographyPosition(20, 30, 40, 50));
            v.LineTo(new GeographyPosition(20, 30, 40, 50));
            var ex = SpatialTestUtils.RunCatching<FormatException>(v.EndFigure);
            Assert.NotNull(ex);
            Assert.Equal(Strings.Validator_InvalidPolygonPoints, ex.Message);
        }

        [Fact]
        public void ValidatePolygonRing_NotARing_Geography()
        {
            GeographyPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeographicCoords);
            v.BeginGeography(SpatialType.Polygon);
            v.BeginFigure(new GeographyPosition(10, 20, 30, 40));
            v.LineTo(new GeographyPosition(20, 30, 40, 50));
            v.LineTo(new GeographyPosition(20, 40, 40, 50));
            v.LineTo(new GeographyPosition(20, 50, 40, 50));
            var ex = SpatialTestUtils.RunCatching<FormatException>(v.EndFigure);
            Assert.NotNull(ex);
            Assert.Equal(Strings.Validator_InvalidPolygonPoints, ex.Message);
        }

        [Fact]
        public void ValidatePolygonRing_Geography()
        {
            GeographyPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeographicCoords);
            v.BeginGeography(SpatialType.Polygon);
            v.BeginFigure(new GeographyPosition(10, 20, 30, 40));
            v.LineTo(new GeographyPosition(20, 30, 40, 50));
            v.LineTo(new GeographyPosition(20, 40, 40, 50));
            v.LineTo(new GeographyPosition(10, 20, 30, 40));
            v.EndFigure();
            v.EndGeography();
        }

        [Fact]
        public void ValidatePolygonRing_Longitude_Geography()
        {
            GeographyPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeographicCoords);
            v.BeginGeography(SpatialType.Polygon);
            v.BeginFigure(new GeographyPosition(10, 20, 30, 40));
            v.LineTo(new GeographyPosition(20, 30, 40, 50));
            v.LineTo(new GeographyPosition(20, 40, 40, 50));
            v.LineTo(new GeographyPosition(10, 20 + 360, 30, 40));
            v.EndFigure();
            v.EndGeography();
        }

        [Fact]
        public void ValidateFullGlobe_Geography()
        {
            GeographyPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeographicCoords);
            v.BeginGeography(SpatialType.FullGlobe);
            v.EndGeography();
        }

        [Fact]
        public void ValidateNoFullGlobe_Geometry()
        {
            GeometryPipeline v = new SpatialValidatorImplementation();
            var ex = SpatialTestUtils.RunCatching<FormatException>(() => v.BeginGeometry(SpatialType.FullGlobe));
            Assert.NotNull(ex);
            Assert.Equal(Strings.Validator_InvalidType(SpatialType.FullGlobe), ex.Message);
        }

        [Fact]
        public void ValidatePolygonRing_LessThanFour_Geometry()
        {
            GeometryPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeometricCoords);
            v.BeginGeometry(SpatialType.Polygon);
            v.BeginFigure(new GeometryPosition(10, 20, 30, 40));
            v.LineTo(new GeometryPosition(20, 30, 40, 50));
            v.LineTo(new GeometryPosition(20, 30, 40, 50));
            var ex = SpatialTestUtils.RunCatching<FormatException>(v.EndFigure);
            Assert.NotNull(ex);
            Assert.Equal(Strings.Validator_InvalidPolygonPoints, ex.Message);
        }

        [Fact]
        public void ValidatePolygonRing_NotARing_Geometry()
        {
            GeometryPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeometricCoords);
            v.BeginGeometry(SpatialType.Polygon);
            v.BeginFigure(new GeometryPosition(10, 20, 30, 40));
            v.LineTo(new GeometryPosition(20, 30, 40, 50));
            v.LineTo(new GeometryPosition(20, 40, 40, 50));
            v.LineTo(new GeometryPosition(20, 50, 40, 50));
            var ex = SpatialTestUtils.RunCatching<FormatException>(v.EndFigure);
            Assert.NotNull(ex);
            Assert.Equal(Strings.Validator_InvalidPolygonPoints, ex.Message);
        }

        [Fact]
        public void ValidatePolygonRing_Geometry()
        {
            GeometryPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeometricCoords);
            v.BeginGeometry(SpatialType.Polygon);
            v.BeginFigure(new GeometryPosition(10, 20, 30, 40));
            v.LineTo(new GeometryPosition(20, 30, 40, 50));
            v.LineTo(new GeometryPosition(20, 40, 40, 50));
            v.LineTo(new GeometryPosition(10, 20, 30, 40));
            v.EndFigure();
            v.EndGeometry();
        }

        [Fact]
        public void ValidateFullGlobe_Geometry()
        {
            GeometryPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeometricCoords);

            var ex = SpatialTestUtils.RunCatching<FormatException>(() => v.BeginGeometry(SpatialType.FullGlobe));
            Assert.NotNull(ex);
            Assert.Equal(Strings.Validator_InvalidType(SpatialType.FullGlobe), ex.Message);
        }
    }
}