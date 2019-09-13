//---------------------------------------------------------------------
// <copyright file="LiteralUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.UriParser;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Tests.UriParser
{
    public class LiteralUtilsTests
    {
        [Fact]
        public void GeometryWorksWithTwoDimensions()
        {
            GeometryPoint geometryPoint = LiteralUtils.ParseGeometry("POINT(10 30)") as GeometryPoint;

            Assert.Equal(10.0, geometryPoint.X);
            Assert.Equal(30.0, geometryPoint.Y);
            Assert.Null(geometryPoint.Z);
            Assert.Null(geometryPoint.M);
        }

        [Fact]
        public void GeographyWorksWithTwoDimensions()
        {
            GeographyPoint geographyPoint = LiteralUtils.ParseGeography("POINT(10 30)") as GeographyPoint;

            Assert.Equal(30.0, geographyPoint.Latitude);
            Assert.Equal(10.0, geographyPoint.Longitude);
            Assert.Null(geographyPoint.Z);
            Assert.Null(geographyPoint.M);
        }

        [Fact]
        public void GeometryWorksWithMoreThanTwoDimensions()
        {
            GeometryPoint geometryPoint = LiteralUtils.ParseGeometry("POINT(10 30 40)") as GeometryPoint;

            Assert.Equal(10.0, geometryPoint.X);
            Assert.Equal(30.0, geometryPoint.Y);
            Assert.NotNull(geometryPoint.Z);
            Assert.Equal(40.0, geometryPoint.Z.Value);
            Assert.Null(geometryPoint.M);
        }

        [Fact]
        public void GeographyWorksWithMoreThanTwoDimensions()
        {
            GeographyPoint geographyPoint = LiteralUtils.ParseGeography("POINT(10 30 40)") as GeographyPoint;

            Assert.Equal(30.0, geographyPoint.Latitude);
            Assert.Equal(10.0, geographyPoint.Longitude);
            Assert.NotNull(geographyPoint.Z);
            Assert.Equal(40.0, geographyPoint.Z.Value);
            Assert.Null(geographyPoint.M);
        }
    }
}
