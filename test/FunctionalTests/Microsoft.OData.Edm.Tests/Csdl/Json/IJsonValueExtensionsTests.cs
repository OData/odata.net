//---------------------------------------------------------------------
// <copyright file="IJsonValueExtensionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Csdl.Json;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Json
{
    public class IJsonValueExtensionsTests
    {
        private IJsonPath _jsonPath = new JsonPath();

        [Fact]
        public void ReadPrimitiveStringWorks()
        {
            IJsonValue jsonValue = new JsonPrimitiveValue("abc");
            string value = jsonValue.ReadPrimitiveString(_jsonPath);
            Assert.Equal("abc", value);
        }

        [Fact]
        public void ReadPrimitiveStringThrows()
        {
            IJsonValue jsonValue = new JsonPrimitiveValue(5.5);
            Action test = () => jsonValue.ReadPrimitiveString(_jsonPath);
            Exception exception = Assert.Throws<Exception>(test);
            Assert.Equal(Strings.JsonReader_CannotReadValueAsType(5.5, "$", "String"), exception.Message);
        }

        [Fact]
        public void ReadPrimitiveBooleanWorks()
        {
            IJsonValue jsonValue = new JsonPrimitiveValue(true);
            bool? value = jsonValue.ReadPrimitiveBoolean(_jsonPath);
            Assert.NotNull(value);
            Assert.True(value.Value);
        }

        [Fact]
        public void ReadPrimitiveBooleanThrows()
        {
            IJsonValue jsonValue = new JsonPrimitiveValue("abc");
            Action test = () => jsonValue.ReadPrimitiveBoolean(_jsonPath);
            Exception exception = Assert.Throws<Exception>(test);
            Assert.Equal(Strings.JsonReader_CannotReadValueAsType("abc", "$", "Boolean"), exception.Message);
        }

        [Fact]
        public void ReadPrimitiveIntegerWorks()
        {
            IJsonValue jsonValue = new JsonPrimitiveValue(42);
            int? value = jsonValue.ReadPrimitiveInteger(_jsonPath);
            Assert.NotNull(value);
            Assert.Equal(42, value.Value);
        }

        [Fact]
        public void ReadPrimitiveIntegerThrows()
        {
            IJsonValue jsonValue = new JsonPrimitiveValue("abc");
            Action test = () => jsonValue.ReadPrimitiveInteger(_jsonPath);
            Exception exception = Assert.Throws<Exception>(test);
            Assert.Equal(Strings.JsonReader_CannotReadValueAsType("abc", "$", "Integer"), exception.Message);
        }
    }
}