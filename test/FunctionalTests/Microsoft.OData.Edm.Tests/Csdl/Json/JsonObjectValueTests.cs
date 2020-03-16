//---------------------------------------------------------------------
// <copyright file="JsonObjectValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Json;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Json
{
    public class JsonObjectValueTests
    {
        [Fact]
        public void ValueKindPropertyReturnsJObject()
        {
            JsonObjectValue primitiveValue = new JsonObjectValue();

            Assert.Equal(JsonValueKind.JObject, primitiveValue.ValueKind);
        }
    }
}