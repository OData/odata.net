//---------------------------------------------------------------------
// <copyright file="UriPrimitiveTypeParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    public class UriPrimitiveTypeParserTests
    {
        [Fact]
        public void InvalidDateTimeOffsetShouldReturnFalse()
        {
            object output;
            Assert.False(this.TryParseUriStringToPrimitiveType("Ct >dvDTrz", EdmCoreModel.Instance.GetDateTimeOffset(true), out output));
        }

        [Fact]
        public void InvalidDateShouldReturnFalse()
        {
            object output;
            Assert.False(this.TryParseUriStringToPrimitiveType("-1000-00-01", EdmCoreModel.Instance.GetDateOnly(true), out output));
        }

        [Fact]
        public void InvalidTimeOfDayShouldReturnFalse()
        {
            object output;
            var list = new string[]
            {
                "1:5:20,0",
                "-20:3:40.900",
                "24:14:40.090"
            };
            foreach (var s in list)
            {
                Assert.False(this.TryParseUriStringToPrimitiveType(s, EdmCoreModel.Instance.GetTimeOnly(true), out output));
            }
        }

        [Fact]
        public void TryUriStringToPrimitiveWithValidDurationLiteralShouldReturnValidTimeSpan()
        {
            object output;
            Assert.True(this.TryParseUriStringToPrimitiveType("duration'P1D'", EdmCoreModel.Instance.GetDuration(false /*isNullable*/), out output));
            Assert.Equal(output, new TimeSpan(1, 0, 0, 0));
        }

        [Fact]
        public void TryUriStringToPrimitiveWithInvalidDurationLiteralShouldReturnFalse()
        {
            object output;
            Assert.False(this.TryParseUriStringToPrimitiveType("duration'P1Y'", EdmCoreModel.Instance.GetDuration(false /*isNullable*/), out output));
        }

        [Fact]
        public void TryUriStringToPrimitiveWithOverflowingDurationLiteralShouldReturnFalse()
        {
            object output;
            Assert.False(this.TryParseUriStringToPrimitiveType("duration'P999999999D'", EdmCoreModel.Instance.GetDuration(false /*isNullable*/), out output));
        }

        [Theory]
        [InlineData("'validstring'", true)]
        [InlineData("'string with spaces'", true)]
        [InlineData("''", true)]
        [InlineData("'escaped''quote'", true)]
        [InlineData("invalidstring", false)]
        [InlineData("'unclosed", false)]
        public void TryUriStringToPrimitiveWithStringShouldHandleQuotedValues(string input, bool expectedResult)
        {
            object output;
            Assert.Equal(expectedResult, this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetString(false), out output));
            if (expectedResult)
            {
                Assert.NotNull(output);
            }
        }

        [Theory]
        [InlineData("true", true, true)]
        [InlineData("false", false, true)]
        [InlineData("True", true, false)]
        [InlineData("FALSE", false, false)]
        [InlineData("1", true, true)]
        [InlineData("0", false, true)]
        [InlineData("invalid", false, false)]
        public void TryUriStringToPrimitiveWithBooleanShouldHandleFormatException(string input, bool expectedValue, bool expectedResult)
        {
            object output;
            Assert.Equal(expectedResult, this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetBoolean(false), out output));
            if (expectedResult)
            {
                Assert.Equal(expectedValue, output);
            }
        }

        [Theory]
        [InlineData("0", (byte)0)]
        [InlineData("255", (byte)255)]
        [InlineData("128", (byte)128)]
        public void TryUriStringToPrimitiveWithValidByteShouldReturnTrue(string input, byte expectedValue)
        {
            object output;
            Assert.True(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetByte(false), out output));
            Assert.Equal(expectedValue, output);
        }

        [Theory]
        [InlineData("256")]  // Overflow
        [InlineData("-1")]   // Overflow (negative)
        [InlineData("abc")]  // Format exception
        [InlineData("12.5")] // Format exception
        public void TryUriStringToPrimitiveWithInvalidByteShouldReturnFalse(string input)
        {
            object output;
            Assert.False(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetByte(false), out output));
        }

        [Theory]
        [InlineData("-128", (sbyte)-128)]
        [InlineData("127", (sbyte)127)]
        [InlineData("0", (sbyte)0)]
        public void TryUriStringToPrimitiveWithValidSByteShouldReturnTrue(string input, sbyte expectedValue)
        {
            object output;
            Assert.True(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetSByte(false), out output));
            Assert.Equal(expectedValue, output);
        }

        [Theory]
        [InlineData("128")]  // Overflow
        [InlineData("-129")] // Overflow
        [InlineData("xyz")]  // Format exception
        public void TryUriStringToPrimitiveWithInvalidSByteShouldReturnFalse(string input)
        {
            object output;
            Assert.False(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetSByte(false), out output));
        }

        [Theory]
        [InlineData("-32768", (short)-32768)]
        [InlineData("32767", (short)32767)]
        [InlineData("0", (short)0)]
        public void TryUriStringToPrimitiveWithValidInt16ShouldReturnTrue(string input, short expectedValue)
        {
            object output;
            Assert.True(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetInt16(false), out output));
            Assert.Equal(expectedValue, output);
        }

        [Theory]
        [InlineData("32768")]  // Overflow
        [InlineData("-32769")] // Overflow
        [InlineData("invalid")] // Format exception
        public void TryUriStringToPrimitiveWithInvalidInt16ShouldReturnFalse(string input)
        {
            object output;
            Assert.False(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetInt16(false), out output));
        }

        [Theory]
        [InlineData("-2147483648", -2147483648)]
        [InlineData("2147483647", 2147483647)]
        [InlineData("0", 0)]
        [InlineData("12345", 12345)]
        public void TryUriStringToPrimitiveWithValidInt32ShouldReturnTrue(string input, int expectedValue)
        {
            object output;
            Assert.True(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetInt32(false), out output));
            Assert.Equal(expectedValue, output);
        }

        [Theory]
        [InlineData("2147483648")]  // Overflow
        [InlineData("-2147483649")] // Overflow
        [InlineData("notanumber")]  // Format exception
        public void TryUriStringToPrimitiveWithInvalidInt32ShouldReturnFalse(string input)
        {
            object output;
            Assert.False(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetInt32(false), out output));
        }

        [Theory]
        [InlineData("9223372036854775807", 9223372036854775807L)]
        [InlineData("-9223372036854775808", -9223372036854775808L)]
        [InlineData("0", 0L)]
        [InlineData("123L", 123L)]
        [InlineData("456l", 456L)]
        public void TryUriStringToPrimitiveWithValidInt64ShouldReturnTrue(string input, long expectedValue)
        {
            object output;
            Assert.True(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetInt64(false), out output));
            Assert.Equal(expectedValue, output);
        }

        [Theory]
        [InlineData("9223372036854775808")]  // Overflow
        [InlineData("-9223372036854775809")] // Overflow
        [InlineData("abc123")]  // Format exception
        public void TryUriStringToPrimitiveWithInvalidInt64ShouldReturnFalse(string input)
        {
            object output;
            Assert.False(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetInt64(false), out output));
        }

        [Theory]
        [InlineData("3.14f", 3.14f)]
        [InlineData("-1.5f", -1.5f)]
        [InlineData("0f", 0f)]
        [InlineData("INF", float.PositiveInfinity)]
        [InlineData("-INF", float.NegativeInfinity)]
        [InlineData("NaN", float.NaN)]
        public void TryUriStringToPrimitiveWithValidSingleShouldReturnTrue(string input, float expectedValue)
        {
            object output;
            Assert.True(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetSingle(false), out output));
            if (float.IsNaN(expectedValue))
            {
                Assert.True(float.IsNaN((float)output));
            }
            else
            {
                Assert.Equal(expectedValue, output);
            }
        }

        [Theory]
        [InlineData("invalid")]
        public void TryUriStringToPrimitiveWithInvalidSingleShouldReturnFalse(string input)
        {
            object output;
            Assert.False(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetSingle(false), out output));
        }

        [Theory]
        [InlineData("3.14", 3.14)]
        [InlineData("-1.5", -1.5)]
        [InlineData("0.0", 0.0)]
        [InlineData("1.23d", 1.23)]
        [InlineData("INF", double.PositiveInfinity)]
        [InlineData("-INF", double.NegativeInfinity)]
        [InlineData("NaN", double.NaN)]
        public void TryUriStringToPrimitiveWithValidDoubleShouldReturnTrue(string input, double expectedValue)
        {
            object output;
            Assert.True(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetDouble(false), out output));
            if (double.IsNaN(expectedValue))
            {
                Assert.True(double.IsNaN((double)output));
            }
            else
            {
                Assert.Equal(expectedValue, output);
            }
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("notadouble")]
        public void TryUriStringToPrimitiveWithInvalidDoubleShouldReturnFalse(string input)
        {
            object output;
            Assert.False(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetDouble(false), out output));
        }

        [Fact]
        public void TryUriStringToPrimitiveWithValidDecimalShouldReturnTrue()
        {
            object output;
            Assert.True(this.TryParseUriStringToPrimitiveType("123.45M", EdmCoreModel.Instance.GetDecimal(false), out output));
            Assert.Equal(123.45M, output);

            Assert.True(this.TryParseUriStringToPrimitiveType("1.23e2", EdmCoreModel.Instance.GetDecimal(false), out output));
            Assert.Equal(123M, output);
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("1e309")]  // Overflow
        public void TryUriStringToPrimitiveWithInvalidDecimalShouldReturnFalse(string input)
        {
            object output;
            Assert.False(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetDecimal(false), out output));
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        [InlineData("12345678-1234-1234-1234-123456789012")]
        [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff")]
        public void TryUriStringToPrimitiveWithValidGuidShouldReturnTrue(string input)
        {
            object output;
            Assert.True(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetGuid(false), out output));
            Assert.Equal(Guid.Parse(input), output);
        }

        [Theory]
        [InlineData("invalid-guid")]
        [InlineData("12345")]
        [InlineData("zzzzzzzz-zzzz-zzzz-zzzz-zzzzzzzzzzzz")]
        public void TryUriStringToPrimitiveWithInvalidGuidShouldReturnFalse(string input)
        {
            object output;
            Assert.False(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetGuid(false), out output));
        }

        [Theory]
        [InlineData("binary'VGVzdA=='")]
        [InlineData("binary'AQID'")]
        [InlineData("binary''")]
        public void TryUriStringToPrimitiveWithValidBinaryShouldReturnTrue(string input)
        {
            object output;
            Assert.True(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetBinary(false), out output));
            Assert.NotNull(output);
        }

        [Theory]
        [InlineData("binary'+w=='")]   // standard base64 with +
        [InlineData("binary'/w=='")]   // standard base64 with /
        [InlineData("binary'+++/'")]   // standard base64 with both + and /
        public void TryUriStringToPrimitiveWithValidBinaryContainingPlusAndSlashShouldReturnTrue(string input)
        {
            object output;
            Assert.True(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetBinary(false), out output));
            Assert.NotNull(output);
        }

        [Theory]
        [InlineData("binary'notbase64!@#$'")]
        [InlineData("invalid")]
        [InlineData("VGVzdA==")]  // Missing prefix
        public void TryUriStringToPrimitiveWithInvalidBinaryShouldReturnFalse(string input)
        {
            object output;
            Assert.False(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetBinary(false), out output));
        }

        [Theory]
        [InlineData("2024-01-15")]
        [InlineData("2000-12-31")]
        [InlineData("1970-01-01")]
        public void TryUriStringToPrimitiveWithValidDateShouldReturnTrue(string input)
        {
            object output;
            Assert.True(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetDateOnly(false), out output));
            Assert.Equal(DateOnly.Parse(input), output);
        }

        [Theory]
        [InlineData("12:30:45.123")]
        [InlineData("23:59:59.999")]
        [InlineData("00:00:00")]
        public void TryUriStringToPrimitiveWithValidTimeOfDayShouldReturnTrue(string input)
        {
            object output;
            Assert.True(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetTimeOnly(false), out output));
            Assert.Equal(TimeOnly.Parse(input), output);
        }

        [Theory]
        [InlineData("2024-01-15T12:30:00Z")]
        [InlineData("2000-12-31T23:59:59+00:00")]
        public void TryUriStringToPrimitiveWithValidDateTimeOffsetShouldReturnTrue(string input)
        {
            object output;
            Assert.True(this.TryParseUriStringToPrimitiveType(input, EdmCoreModel.Instance.GetDateTimeOffset(false), out output));
            Assert.NotNull(output);
        }

        [Fact]
        public void TryUriStringToPrimitiveWithNullableShouldHandleNullKeyword()
        {
            object output;
            Assert.True(this.TryParseUriStringToPrimitiveType("null", EdmCoreModel.Instance.GetInt32(true), out output));
            Assert.Null(output);

            Assert.True(this.TryParseUriStringToPrimitiveType("null", EdmCoreModel.Instance.GetString(true), out output));
            Assert.Null(output);

            Assert.True(this.TryParseUriStringToPrimitiveType("null", EdmCoreModel.Instance.GetGuid(true), out output));
            Assert.Null(output);
        }

        [Fact]
        public void TryUriStringToPrimitiveWithNullForNonNullableTypeShouldReturnFalse()
        {
            object output;
            Assert.False(this.TryParseUriStringToPrimitiveType("null", EdmCoreModel.Instance.GetInt32(false), out output));
        }

        [Fact]
        public void TryUriStringToPrimitiveWithUrlSafeBinaryLiteralShouldReturnByteArray()
        {
            object output;
            Assert.True(this.TryParseUriStringToPrimitiveType("binary'AAEA_w=='", EdmCoreModel.Instance.GetBinary(false), out output));
            Assert.Equal(new byte[] { 0x00, 0x01, 0x00, 0xff }, (byte[])output);
        }

        #region Private Methods

        private bool TryParseUriStringToPrimitiveType(string text, IEdmTypeReference targetType, out object targetValue)
        {
            UriLiteralParsingException exception;

            return UriPrimitiveTypeParser.TryParseUriStringToType(text.AsSpan(), targetType, out targetValue, out exception);
        }

        #endregion

    }
}
