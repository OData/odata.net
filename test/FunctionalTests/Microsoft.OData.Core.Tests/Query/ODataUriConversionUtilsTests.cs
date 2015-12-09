//---------------------------------------------------------------------
// <copyright file="ODataUriConversionUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Xunit;

namespace Microsoft.OData.Core.Tests.Query
{
    public class ODataUriConvesionUtilsTests
    {
        [Fact]
        public void DoubleLiteralShouldNotHaveDecimalPointForScientificNotation()
        {
            PrimitiveLiteral(1e+100).Should().Be("1E+100");
            PrimitiveLiteral(-1e+100).Should().Be("-1E+100");
            PrimitiveLiteral(1e-100).Should().Be("1E-100");
            PrimitiveLiteral(-1e-100).Should().Be("-1E-100");
        }

        [Fact]
        public void DoubleLiteralShouldNotHaveDecimalPointOrTypeMarkerForInfinity()
        {
            PrimitiveLiteral(double.PositiveInfinity).Should().Be("INF");
            PrimitiveLiteral(double.NegativeInfinity).Should().Be("-INF");
        }

        [Fact]
        public void DoubleLiteralShouldNotHaveDecimalPointOrTypeMarkerForNaN()
        {
            PrimitiveLiteral(double.NaN).Should().Be("NaN");
        }

        [Fact]
        public void DoubleLiteralShouldHaveExactlyOneDecimalPointForRealNumber()
        {
            PrimitiveLiteral(12.345).Should().Be("12.345");
            PrimitiveLiteral(-12.345).Should().Be("-12.345");
        }

        [Fact]
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