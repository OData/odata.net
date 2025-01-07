//---------------------------------------------------------------------
// <copyright file="ODataUriConversionUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.OData.Tests.Query
{
    public class ODataUriConvesionUtilsTests
    {
        [Theory]
        [InlineData(1e+100, "1E+100")]
        [InlineData(-1e+100, "-1E+100")]
        [InlineData(1e-100, "1E-100")]
        [InlineData(-1e-100, "-1E-100")]
        [InlineData(double.PositiveInfinity, "INF")]
        [InlineData(double.NegativeInfinity, "-INF")]
        [InlineData(double.NaN, "NaN")]
        [InlineData(12.345, "12.345")]
        [InlineData(-12.345, "-12.345")]
        [InlineData(100D, "100.0")] // DoubleLiteralShouldHaveDecimalPointForWholeNumber
        [InlineData(-100D, "-100.0")] // DoubleLiteralShouldHaveDecimalPointForWholeNumber
        public void ConvertToUriPrimitiveLiteralUsingPrimitiveValueWorks(object value, string expect)
        {
            string actual = ODataUriConversionUtils.ConvertToUriPrimitiveLiteral(value, ODataVersion.V4);
            Assert.Equal(expect, actual);
        }
    }
}