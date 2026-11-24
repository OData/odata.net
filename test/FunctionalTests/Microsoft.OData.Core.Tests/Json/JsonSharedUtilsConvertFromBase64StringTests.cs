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
        [Theory]
        [InlineData("--8=", "++8=")]
        [InlineData("__8=", "//8=")]
        [InlineData("-_8=", "+/8=")]
        [InlineData("AQID", "AQID")]
        [InlineData("", "")]
        [InlineData(" \t-_8=\r\n", "+/8=")]
        public void DecodesBase64AndBase64Url(string value, string base64)
        {
            Assert.Equal(Convert.FromBase64String(base64), JsonSharedUtils.ConvertFromBase64String(value));
        }

        [Theory]
        [InlineData("-")]
        [InlineData("__8")]
        [InlineData("-_8!")]
        [InlineData("-_8===")]
        public void InvalidValuesStillThrow(string value)
        {
            Assert.Throws<FormatException>(() => JsonSharedUtils.ConvertFromBase64String(value));
        }

        [Fact]
        public void NullStillThrows()
        {
            Assert.Throws<ArgumentNullException>(() => JsonSharedUtils.ConvertFromBase64String(null));
        }
    }
}
