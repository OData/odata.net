//---------------------------------------------------------------------
// <copyright file="JsonFormatTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataFormatTests
    {
        [Fact]
        public void JsonFormatToStringShouldReturnJson()
        {
            Assert.Equal("Json", ODataFormat.Json.ToString());
        }
    }
}
