//---------------------------------------------------------------------
// <copyright file="JsonArrayValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Json;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Json
{
    public class JsonArrayValueTests
    {
        [Fact]
        public void ValueKindPropertyReturnsJArray()
        {
            JsonArrayValue primitiveValue = new JsonArrayValue();

            Assert.Equal(JsonValueKind.JArray, primitiveValue.ValueKind);
        }
    }
}