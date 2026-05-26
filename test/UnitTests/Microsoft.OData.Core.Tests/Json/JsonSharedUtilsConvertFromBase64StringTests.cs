//---------------------------------------------------------------------
// <copyright file="JsonSharedUtilsConvertFromBase64StringTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class JsonSharedUtilsConvertFromBase64StringTests
    {
        [Fact]
        public void ConvertFromBase64String_StandardBase64_ReturnsExpectedBytes()
        {
            // "AAEC" is Base64 for { 0x00, 0x01, 0x02 }
            byte[] result = JsonSharedUtils.ConvertFromBase64String("AAEC");
            Assert.Equal(new byte[] { 0x00, 0x01, 0x02 }, result);
        }

        [Fact]
        public void ConvertFromBase64String_StandardBase64WithPadding_ReturnsExpectedBytes()
        {
            // "AAEA/w==" is Base64 for { 0x00, 0x01, 0x00, 0xFF }
            byte[] result = JsonSharedUtils.ConvertFromBase64String("AAEA/w==");
            Assert.Equal(new byte[] { 0x00, 0x01, 0x00, 0xFF }, result);
        }

        [Fact]
        public void ConvertFromBase64String_Base64UrlWithUnderscore_ReturnsExpectedBytes()
        {
            // "AAEA_w==" is Base64Url for { 0x00, 0x01, 0x00, 0xFF }
            byte[] result = JsonSharedUtils.ConvertFromBase64String("AAEA_w==");
            Assert.Equal(new byte[] { 0x00, 0x01, 0x00, 0xFF }, result);
        }

        [Fact]
        public void ConvertFromBase64String_Base64UrlWithHyphen_ReturnsExpectedBytes()
        {
            // '+' in standard Base64 becomes '-' in Base64Url
            // "AAEC+w==" is standard, "AAEC-w==" is Base64Url for { 0x00, 0x01, 0x02, 0xFB }
            byte[] result = JsonSharedUtils.ConvertFromBase64String("AAEC-w==");
            Assert.Equal(new byte[] { 0x00, 0x01, 0x02, 0xFB }, result);
        }

        [Fact]
        public void ConvertFromBase64String_Base64UrlWithBothSpecialChars_ReturnsExpectedBytes()
        {
            // Contains both '-' and '_' (Base64Url equivalents of '+' and '/')
            // Standard: "+/8=" -> Base64Url: "-_8="
            byte[] result = JsonSharedUtils.ConvertFromBase64String("-_8=");
            byte[] expected = Convert.FromBase64String("+/8=");
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ConvertFromBase64String_EmptyString_ReturnsEmptyArray()
        {
            byte[] result = JsonSharedUtils.ConvertFromBase64String("");
            Assert.Empty(result);
        }

        [Fact]
        public void ConvertFromBase64String_InvalidString_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => JsonSharedUtils.ConvertFromBase64String("!!!invalid!!!"));
        }

        [Fact]
        public void ConvertFromBase64String_NullValue_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => JsonSharedUtils.ConvertFromBase64String(null));
        }
    }
}
