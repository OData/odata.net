//---------------------------------------------------------------------
// <copyright file="JsonPrimitiveValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Json;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Json
{
    public class JsonPrimitiveValueTests
    {
        [Fact]
        public void ValueKindPropertyReturnsJPrimitive()
        {
            JsonPrimitiveValue primitiveValue = new JsonPrimitiveValue(null);

            Assert.Equal(JsonValueKind.JPrimitive, primitiveValue.ValueKind);
        }

        [Theory]
        [InlineData(12)]
        [InlineData("string")]
        public void ValuePropertyReturnsTheSameInputValue(object value)
        {
            JsonPrimitiveValue primitiveValue = new JsonPrimitiveValue(value);

            Assert.Same(value, primitiveValue.Value);
        }
    }
}