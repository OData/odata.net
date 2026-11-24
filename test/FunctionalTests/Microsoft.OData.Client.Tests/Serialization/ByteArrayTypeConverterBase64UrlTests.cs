//---------------------------------------------------------------------
// <copyright file="ByteArrayTypeConverterBase64UrlTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Xunit;

namespace Microsoft.OData.Client.Tests.Serialization
{
    public class ByteArrayTypeConverterBase64UrlTests
    {
        [Theory]
        [InlineData("--8=", "++8=")]
        [InlineData("__8=", "//8=")]
        [InlineData("-_8=", "+/8=")]
        public void ParsesBase64Url(string value, string base64)
        {
            Assert.Equal(Convert.FromBase64String(base64), Assert.IsType<byte[]>(new ByteArrayTypeConverter().Parse(value)));
        }
    }
}
