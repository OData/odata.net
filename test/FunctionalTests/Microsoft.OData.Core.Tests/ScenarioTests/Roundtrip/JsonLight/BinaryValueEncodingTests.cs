//---------------------------------------------------------------------
// <copyright file="BinaryValueEncodingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.Evaluation;
using Microsoft.OData.Core.Json;
using Microsoft.OData.Core.UriParser.Parsers;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Roundtrip.JsonLight
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
            var writer = new JsonWriter(new StringWriter(builder), false /*indent*/, ODataFormat.Json, isIeee754Compatible: true);
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
            var jsonReader = new JsonReader(new StringReader(builder.ToString()), ODataFormat.Json, isIeee754Compatible:true);
            jsonReader.Read();
            Assert.Equal(encodedByteArr, jsonReader.Value);

            object defaultParsedByteArray;
            DefaultLiteralParser.TryParseLiteral(byteArray.GetType(),Uri.UnescapeDataString(defaultFormattedByteArray), out defaultParsedByteArray).Should().BeTrue();
            Assert.Equal((byte[])defaultParsedByteArray, byteArray);

            object keyAsSegmentParsedByteArray;
            KeyAsSegmentsLiteralParser.TryParseLiteral(byteArray.GetType(), Uri.UnescapeDataString(keyAsSegmentFormattedByteArray), out keyAsSegmentParsedByteArray).Should().BeTrue();
            Assert.Equal((byte[])keyAsSegmentParsedByteArray, byteArray);
        }
    }
}

 