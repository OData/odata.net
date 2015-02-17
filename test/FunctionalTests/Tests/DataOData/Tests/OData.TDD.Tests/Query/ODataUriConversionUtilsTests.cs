//---------------------------------------------------------------------
// <copyright file="ODataUriConversionUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Query
{
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataUriConvesionUtilsTests
    {
        [TestMethod]
        public void DoubleLiteralShouldNotHaveDecimalPointForScientificNotation()
        {
            PrimitiveLiteral(1e+100).Should().Be("1E+100");
            PrimitiveLiteral(-1e+100).Should().Be("-1E+100");
            PrimitiveLiteral(1e-100).Should().Be("1E-100");
            PrimitiveLiteral(-1e-100).Should().Be("-1E-100");
        }

        [TestMethod]
        public void DoubleLiteralShouldNotHaveDecimalPointOrTypeMarkerForInfinity()
        {
            PrimitiveLiteral(double.PositiveInfinity).Should().Be("INF");
            PrimitiveLiteral(double.NegativeInfinity).Should().Be("-INF");
        }

        [TestMethod]
        public void DoubleLiteralShouldNotHaveDecimalPointOrTypeMarkerForNaN()
        {
            PrimitiveLiteral(double.NaN).Should().Be("NaN");
        }

        [TestMethod]
        public void DoubleLiteralShouldHaveExactlyOneDecimalPointForRealNumber()
        {
            PrimitiveLiteral(12.345).Should().Be("12.345");
            PrimitiveLiteral(-12.345).Should().Be("-12.345");
        }

        [TestMethod]
        public void DoubleLiteralShouldHaveDecimalPointForWholeNumber()
        {
            PrimitiveLiteral(100D).Should().Be("100.0");
            PrimitiveLiteral(-100D).Should().Be("-100.0");
        }

        private static string PrimitiveLiteral(object value)
        {
            return ODataUriConversionUtils.ConvertToUriPrimitiveLiteral(value, ODataVersion.V4);
        }
    }
}