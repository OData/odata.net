//---------------------------------------------------------------------
// <copyright file="BinaryValueEncodingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Json;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Roundtrip.Json
{
    public class BinaryValueEncodingTests
    {
        private static readonly LiteralFormatter DefaultLiteralFormatter = LiteralFormatter.ForKeys(false);
        private static readonly LiteralParser DefaultLiteralParser = LiteralParser.ForKeys(false);
        private static readonly LiteralFormatter KeyAsSegmentsLiteralFormatter = LiteralFormatter.ForKeys(true);
        private static readonly LiteralParser KeyAsSegmentsLiteralParser = LiteralParser.ForKeys(true);

        [Fact]
        public void UriPayloadEncodingShouldMatchForBinaryLiteralWithoutEqualPadding()
        {
            var byteArray = new byte[] { 1, 2, 3 };
            string encodedByteArr = Convert.ToBase64String(byteArray, 0, byteArray.Length);
            Assert.Equal("AQID", encodedByteArr);
            VerifyBinaryEncoding(byteArray, encodedByteArr);
        }

        [Fact]
        public void UriAndPayloadEncodingShouldMatchForBinaryLiteralWithEqualPadding()
        {
            var byteArray = new byte[] { 1, 2, 3, 4 };
            string encodedByteArr = Convert.ToBase64String(byteArray, 0, byteArray.Length);
            Assert.Equal("AQIDBA==", encodedByteArr);
            VerifyBinaryEncoding(byteArray, encodedByteArr);
        }

        private static void VerifyBinaryEncoding(byte[] byteArray, string encodedByteArr)
        {
            // Writing binary literal
            var builder = new StringBuilder();
            var writer = new JsonWriter(new StringWriter(builder), isIeee754Compatible: true);
            writer.WritePrimitiveValue(byteArray);

            // Json literals is surrounded with quotes, so we need to add quotes to the encoded string. 
            Assert.Equal("\"" + encodedByteArr + "\"", builder.ToString());

            string defaultFormattedByteArray = DefaultLiteralFormatter.Format(byteArray);
            string keyAsSegmentFormattedByteArray = KeyAsSegmentsLiteralFormatter.Format(byteArray);

            // Non key segment is surrounded with binary prefix and escaped. 
            Assert.Equal("binary\'" + Uri.EscapeDataString(encodedByteArr) + "\'", defaultFormattedByteArray);

            // Key Segments are the same as the escaped encoded string.
            Assert.Equal(Uri.EscapeDataString(encodedByteArr), keyAsSegmentFormattedByteArray);

            // Parsing binary literal
            var jsonReader = new JsonReader(new StringReader(builder.ToString()), isIeee754Compatible: true);
            jsonReader.Read();
            Assert.Equal(encodedByteArr, jsonReader.GetValue());

            object defaultParsedByteArray;
            Assert.True(DefaultLiteralParser.TryParseLiteral(byteArray.GetType(), Uri.UnescapeDataString(defaultFormattedByteArray), out defaultParsedByteArray));
            Assert.Equal((byte[])defaultParsedByteArray, byteArray);

            object keyAsSegmentParsedByteArray;
            Assert.True(KeyAsSegmentsLiteralParser.TryParseLiteral(byteArray.GetType(), Uri.UnescapeDataString(keyAsSegmentFormattedByteArray), out keyAsSegmentParsedByteArray));
            Assert.Equal((byte[])keyAsSegmentParsedByteArray, byteArray);
        }

        [Fact]
        public void LiteralFormatterFormatDateOnlyLiteral()
        {
            DateOnly dateOnly = new DateOnly(2024, 10, 1);
            string defaultFormattedDateOnly = DefaultLiteralFormatter.Format(dateOnly);
            string keyAsSegmentFormattedDateOnly = KeyAsSegmentsLiteralFormatter.Format(dateOnly);

            Assert.Equal("2024-10-01", defaultFormattedDateOnly);
            Assert.Equal(defaultFormattedDateOnly, keyAsSegmentFormattedDateOnly);
        }

        [Fact]
        public void LiteralFormatterFormatTimeOnlyLiteral()
        {
            TimeOnly timeOnly = new TimeOnly(4, 10, 1, 9);
            string defaultFormattedTimeOnly = DefaultLiteralFormatter.Format(timeOnly);
            string keyAsSegmentFormattedTimeOnly = KeyAsSegmentsLiteralFormatter.Format(timeOnly);

            Assert.Equal("04%3A10%3A01.0090000", defaultFormattedTimeOnly);
            Assert.Equal(defaultFormattedTimeOnly, keyAsSegmentFormattedTimeOnly);
        }
    }
}

