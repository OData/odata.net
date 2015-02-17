//---------------------------------------------------------------------
// <copyright file="LiteralUtilsUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Syntactic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Spatial;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    [TestClass]
    public class LiteralUtilsUnitTests
    {
        [TestMethod]
        public void GeometryWorksWithTwoDimensions()
        {
            GeometryPoint geometryPoint = LiteralUtils.ParseGeometry("POINT(10 30)") as GeometryPoint;
            geometryPoint.X.ShouldBe(10.0);
            geometryPoint.Y.ShouldBe(30.0);
        }

        [TestMethod]
        public void GeographyWorksWithTwoDimensions()
        {
            GeographyPoint geographyPoint = LiteralUtils.ParseGeography("POINT(10 30)") as GeographyPoint;
            geographyPoint.Latitude.ShouldBe(30.0);
            geographyPoint.Longitude.ShouldBe(10.0);
        }

        [TestMethod]
        public void GeometryWorksWithMoreThanTwoDimensions()
        {
            GeometryPoint geometryPoint = LiteralUtils.ParseGeometry("POINT(10 30 40)") as GeometryPoint;
            geometryPoint.X.ShouldBe(10.0);
            geometryPoint.Y.ShouldBe(30.0);
            geometryPoint.Z.ShouldBe(40.0);
        }

        [TestMethod]
        public void GeographyWorksWithMoreThanTwoDimensions()
        {
            GeographyPoint geographyPoint = LiteralUtils.ParseGeography("POINT(10 30 40)") as GeographyPoint;
            geographyPoint.Latitude.ShouldBe(30.0);
            geographyPoint.Longitude.ShouldBe(10.0);
            geographyPoint.Z.ShouldBe(40.0);
        }
    }
}
