//---------------------------------------------------------------------
// <copyright file="JsonPathTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Json;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Json
{
    public class JsonPathTests
    {
        [Theory]
        [InlineData(true, "$['a']['b'][2][4]['c']")]
        [InlineData(false, "$.a.b[2][4].c")]
        public void JsonPathReturnsCorrectPath(bool isBracketNotation, string expected)
        {
            JsonPath jsonPath = new JsonPath(null, isBracketNotation);

            jsonPath.Push("a");
            jsonPath.Push("b");
            jsonPath.Push(2);
            jsonPath.Push(4);
            jsonPath.Push("c");

            Assert.Equal(expected, jsonPath.Path);
        }

        [Theory]
        [InlineData(true, "(myjson)$['a']['b'][2][4]['c']")]
        [InlineData(false, "(myjson)$.a.b[2][4].c")]
        public void JsonPathWorksWithSourceSetting(bool isBracketNotation, string expected)
        {
            string source = "myjson";
            JsonPath jsonPath = new JsonPath(source, isBracketNotation);

            jsonPath.Push("a");
            jsonPath.Push("b");
            jsonPath.Push(2);
            jsonPath.Push(4);
            jsonPath.Push("c");

            Assert.Equal(expected, jsonPath.Path);
        }

        [Fact]
        public void JsonPathWorksWithIterator()
        {
            JsonPath jsonPath = new JsonPath();

            jsonPath.Push("a");
            jsonPath.Push("b");
            Assert.Equal("$.a.b", jsonPath.Path);

            jsonPath.Pop();
            jsonPath.Push(2);
            jsonPath.Push(4);
            jsonPath.Push("c");

            Assert.Equal("$.a[2][4].c", jsonPath.Path);
            Assert.Equal("$.a[2][4].c", jsonPath.Path);
        }
    }
}