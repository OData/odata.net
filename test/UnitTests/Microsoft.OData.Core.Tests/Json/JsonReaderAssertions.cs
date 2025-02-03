//---------------------------------------------------------------------
// <copyright file="JsonReaderAssertions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public static class JsonReaderAssertions
    {
        internal static IJsonReader ShouldBeOn(this IJsonReader jsonReader, JsonNodeType nodeType, object value)
        {
            Assert.Equal(nodeType, jsonReader.NodeType);
            Assert.Equal(value, jsonReader.GetValue());
            return jsonReader;
        }
    }
}