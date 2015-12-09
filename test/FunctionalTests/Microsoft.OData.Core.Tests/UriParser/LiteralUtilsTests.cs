//---------------------------------------------------------------------
// <copyright file="LiteralUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Core.UriParser;
using Microsoft.Spatial;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser
{
    public class LiteralUtilsTests
    {
        [Fact]
        public void GeometryWorksWithTwoDimensions()
        {
            GeometryPoint geometryPoint = LiteralUtils.ParseGeometry("POINT(10 30)") as GeometryPoint;
            geometryPoint.X.ShouldBe(10.0);
            geometryPoint.Y.ShouldBe(30.0);
        }

        [Fact]
        public void GeographyWorksWithTwoDimensions()
        {
            GeographyPoint geographyPoint = LiteralUtils.ParseGeography("POINT(10 30)") as GeographyPoint;
            geographyPoint.Latitude.ShouldBe(30.0);
            geographyPoint.Longitude.ShouldBe(10.0);
        }

        [Fact]
        public void GeometryWorksWithMoreThanTwoDimensions()
        {
            GeometryPoint geometryPoint = LiteralUtils.ParseGeometry("POINT(10 30 40)") as GeometryPoint;
            geometryPoint.X.ShouldBe(10.0);
            geometryPoint.Y.ShouldBe(30.0);
            geometryPoint.Z.ShouldBe(40.0);
        }

        [Fact]
        public void GeographyWorksWithMoreThanTwoDimensions()
        {
            GeographyPoint geographyPoint = LiteralUtils.ParseGeography("POINT(10 30 40)") as GeographyPoint;
            geographyPoint.Latitude.ShouldBe(30.0);
            geographyPoint.Longitude.ShouldBe(10.0);
            geographyPoint.Z.ShouldBe(40.0);
        }
    }
}
