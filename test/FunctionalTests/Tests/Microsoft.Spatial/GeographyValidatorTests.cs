//---------------------------------------------------------------------
// <copyright file="GeographyValidatorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.Data.Spatial;
using Microsoft.Spatial;
using DataSpatialUnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataSpatialUnitTests.Tests
{
    [TestClass]
    public class GeographyValidatorTests
    {
        private static readonly CoordinateSystem NonDefaultGeographicCoords = CoordinateSystem.Geography(1234);

        [TestMethod]
        public void ValidateLatitude()
        {
            GeographyPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeographicCoords);
            v.BeginGeography(SpatialType.Point);

            var ex = SpatialTestUtils.RunCatching<FormatException>(() => v.BeginFigure(new GeographyPosition(-91, 0, 0, 0)));
            Assert.IsNotNull(ex);
            Assert.AreEqual(Strings.Validator_InvalidLatitudeCoordinate(-91), ex.Message);

            v.Reset();
            v.SetCoordinateSystem(NonDefaultGeographicCoords);
            v.BeginGeography(SpatialType.Point);

            ex = SpatialTestUtils.RunCatching<FormatException>(() => v.BeginFigure(new GeographyPosition(91, 0, 0, 0)));
            Assert.IsNotNull(ex);
            Assert.AreEqual(Strings.Validator_InvalidLatitudeCoordinate(91), ex.Message);
        }

        [TestMethod]
        public void ValidateLongitude()
        {
            GeographyPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeographicCoords);
            v.BeginGeography(SpatialType.Point);

            var ex = SpatialTestUtils.RunCatching<FormatException>(() => v.BeginFigure(new GeographyPosition(0, -15070, 0, 0)));
            Assert.IsNotNull(ex);
            Assert.AreEqual(Strings.Validator_InvalidLongitudeCoordinate(-15070), ex.Message);

            v.Reset();
            v.SetCoordinateSystem(NonDefaultGeographicCoords);
            v.BeginGeography(SpatialType.Point);

            ex = SpatialTestUtils.RunCatching<FormatException>(() => v.BeginFigure(new GeographyPosition(0, 15070, 0, 0)));
            Assert.IsNotNull(ex);
            Assert.AreEqual(Strings.Validator_InvalidLongitudeCoordinate(15070), ex.Message);
        }

        [TestMethod]
        public void ValidatePolygonRing_LessThanFour()
        {
            GeographyPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeographicCoords);
            v.BeginGeography(SpatialType.Polygon);
            v.BeginFigure(new GeographyPosition(10, 20, 30, 40));
            v.LineTo(new GeographyPosition(20, 30, 40, 50));
            v.LineTo(new GeographyPosition(20, 30, 40, 50));
            var ex = SpatialTestUtils.RunCatching<FormatException>(v.EndFigure);
            Assert.IsNotNull(ex);
            Assert.AreEqual(Strings.Validator_InvalidPolygonPoints, ex.Message);
        }

        [TestMethod]
        public void ValidatePolygonRing_NotARing()
        {
            GeographyPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeographicCoords);
            v.BeginGeography(SpatialType.Polygon);
            v.BeginFigure(new GeographyPosition(10, 20, 30, 40));
            v.LineTo(new GeographyPosition(20, 30, 40, 50));
            v.LineTo(new GeographyPosition(20, 40, 40, 50));
            v.LineTo(new GeographyPosition(20, 50, 40, 50));
            var ex = SpatialTestUtils.RunCatching<FormatException>(v.EndFigure);
            Assert.IsNotNull(ex);
            Assert.AreEqual(Strings.Validator_InvalidPolygonPoints, ex.Message);
        }

        [TestMethod]
        public void ValidatePolygonRing()
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

        [TestMethod]
        public void ValidatePolygonRing_Longitude()
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

        [TestMethod]
        public void ValidateFullGlobe()
        {
            GeographyPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeographicCoords);
            v.BeginGeography(SpatialType.FullGlobe);
            v.EndGeography();
        }
    }
}