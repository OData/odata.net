//---------------------------------------------------------------------
// <copyright file="JsonLightFormatTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataFormatTests
    {
        [Fact]
        public void JsonLightFormatToStringShouldReturnJsonLight()
        {
            ODataFormat.Json.ToString().Should().Be("JsonLight");
        }
    }
}
