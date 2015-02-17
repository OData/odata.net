//---------------------------------------------------------------------
// <copyright file="GeometryValidatorTests.cs" company="Microsoft">
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
    public class GeometryValidatorTests
    {
        private static readonly CoordinateSystem NonDefaultGeometricCoords = CoordinateSystem.Geometry(1234);

        [TestMethod]
        public void ValidateNoFullGlobe()
        {
            GeometryPipeline v = new SpatialValidatorImplementation();
            var ex = SpatialTestUtils.RunCatching<FormatException>(() => v.BeginGeometry(SpatialType.FullGlobe));
            Assert.IsNotNull(ex);
            Assert.AreEqual(Strings.Validator_InvalidType(SpatialType.FullGlobe), ex.Message);
        }

        [TestMethod]
        public void ValidatePolygonRing_LessThanFour()
        {
            GeometryPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeometricCoords);
            v.BeginGeometry(SpatialType.Polygon);
            v.BeginFigure(new GeometryPosition(10, 20, 30, 40));
            v.LineTo(new GeometryPosition(20, 30, 40, 50));
            v.LineTo(new GeometryPosition(20, 30, 40, 50));
            var ex = SpatialTestUtils.RunCatching<FormatException>(v.EndFigure);
            Assert.IsNotNull(ex);
            Assert.AreEqual(Strings.Validator_InvalidPolygonPoints, ex.Message);
        }

        [TestMethod]
        public void ValidatePolygonRing_NotARing()
        {
            GeometryPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeometricCoords);
            v.BeginGeometry(SpatialType.Polygon);
            v.BeginFigure(new GeometryPosition(10, 20, 30, 40));
            v.LineTo(new GeometryPosition(20, 30, 40, 50));
            v.LineTo(new GeometryPosition(20, 40, 40, 50));
            v.LineTo(new GeometryPosition(20, 50, 40, 50));
            var ex = SpatialTestUtils.RunCatching<FormatException>(v.EndFigure);
            Assert.IsNotNull(ex);
            Assert.AreEqual(Strings.Validator_InvalidPolygonPoints, ex.Message);
        }

        [TestMethod]
        public void ValidatePolygonRing()
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

        [TestMethod]
        public void ValidateFullGlobe()
        {
            GeometryPipeline v = new SpatialValidatorImplementation();
            v.SetCoordinateSystem(NonDefaultGeometricCoords);

            var ex = SpatialTestUtils.RunCatching<FormatException>(() => v.BeginGeometry(SpatialType.FullGlobe));
            Assert.IsNotNull(ex);
            Assert.AreEqual(Strings.Validator_InvalidType(SpatialType.FullGlobe), ex.Message);
        }
    }
}