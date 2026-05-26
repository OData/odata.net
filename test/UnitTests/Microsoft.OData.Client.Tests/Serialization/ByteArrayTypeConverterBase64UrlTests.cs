//---------------------------------------------------------------------
// <copyright file="ByteArrayTypeConverterBase64UrlTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Client;
using Xunit;

namespace Microsoft.OData.Client.Tests.Serialization
{
    /// <summary>
    /// Tests that ByteArrayTypeConverter accepts Base64Url-encoded input.
    /// </summary>
    public class ByteArrayTypeConverterBase64UrlTests
    {
        [Fact]
        public void ByteArrayTypeConverter_Parse_Base64Url_WithUnderscore()
        {
            // "AAEA_w==" is the Base64Url form of "AAEA/w==" which decodes to { 0x00, 0x01, 0x00, 0xFF }
            var converter = new ByteArrayTypeConverter();
            byte[] result = (byte[])converter.Parse("AAEA_w==");
            Assert.Equal(new byte[] { 0x00, 0x01, 0x00, 0xFF }, result);
        }

        [Fact]
        public void ByteArrayTypeConverter_Parse_Base64Url_WithHyphen()
        {
            // "AAEC-w==" is the Base64Url form of "AAEC+w==" which decodes to { 0x00, 0x01, 0x02, 0xFB }
            var converter = new ByteArrayTypeConverter();
            byte[] result = (byte[])converter.Parse("AAEC-w==");
            Assert.Equal(new byte[] { 0x00, 0x01, 0x02, 0xFB }, result);
        }

        [Fact]
        public void ByteArrayTypeConverter_Parse_StandardBase64_StillWorks()
        {
            var converter = new ByteArrayTypeConverter();
            byte[] result = (byte[])converter.Parse("AAECAw==");
            Assert.Equal(new byte[] { 0x00, 0x01, 0x02, 0x03 }, result);
        }
    }
}
