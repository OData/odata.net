//---------------------------------------------------------------------
// <copyright file="IJsonReaderExtensionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.OData.Edm.Csdl.Json;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Json
{
    public class IJsonReaderExtensionsTests
    {
        [Theory]
        [InlineData("\"abc\"", typeof(string))]
        [InlineData(42, typeof(int))]
        [InlineData(6.9, typeof(decimal))]
        [InlineData("true", typeof(bool))]
        public void ReadAsPrimitiveWorksForNonNullValue(string value, Type expected)
        {
            JsonPrimitiveValue primitive = ReadAsJsonPrimitive(value);

            Assert.NotNull(primitive);
            Assert.NotNull(primitive.Value);
            Assert.Equal(expected, primitive.Value.GetType());
        }

        [Fact]
        public void ReadAsPrimitiveWorksForNullValue()
        {
            JsonPrimitiveValue primitive = ReadAsJsonPrimitive("null");

            Assert.NotNull(primitive);
            Assert.Null(primitive.Value);
        }

        [Fact]
        public void ReadAsPrimitiveThrowsForNonPrimitiveValue()
        {
            Action test = () => ReadAsJsonPrimitive("{ object }");

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(test);
            Assert.Equal(Strings.CsdlJsonReader_UnexpectedJsonNodeType(JsonNodeType.StartObject, JsonNodeType.PrimitiveValue),
                exception.Message);
        }

        [Fact]
        public void ReadAsObjectWorksForNormalJson()
        {
            string json = @" {
  ""name"":""John"",
  ""age"": [],
  ""city"": {},
  ""other"": null
}";

            JsonObjectValue objValue = ReadAsJsonObejct(json);

            Assert.NotNull(objValue);

            Assert.Equal(4, objValue.Count);

            JsonPrimitiveValue name = Assert.IsType<JsonPrimitiveValue>(objValue["name"]);
            Assert.Equal("John", name.Value);

            JsonArrayValue age = Assert.IsType<JsonArrayValue>(objValue["age"]);
            Assert.Empty(age);

            JsonObjectValue city = Assert.IsType<JsonObjectValue>(objValue["city"]);
            Assert.Empty(city);

            JsonPrimitiveValue other = Assert.IsType<JsonPrimitiveValue>(objValue["other"]);
            Assert.Null(other.Value);
        }

        [Fact]
        public void ReadAsObjectThrowsForNonObjectValue()
        {
            string json = " [ anything ]";

            Action test = () => ReadAsJsonObejct(json);

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(test);
            Assert.Equal(Strings.CsdlJsonReader_UnexpectedJsonNodeType(JsonNodeType.StartArray, JsonNodeType.StartObject),
                exception.Message);
        }

        [Fact]
        public void ReadAsArrayWorksForNormalJson()
        {
            string json = @"[
  null,
  ""abc"" ,
  {
    ""name"":""John"",
    ""age"":31
  },

  []
]";

            JsonArrayValue arrayValue = ReadAsJsonArray(json);

            Assert.NotNull(arrayValue);
            Assert.Equal(4, arrayValue.Count);

            // #1 is null
            JsonPrimitiveValue primitive1 = Assert.IsType<JsonPrimitiveValue>(arrayValue[0]);
            Assert.Null(primitive1.Value);

            // #2 is string
            JsonPrimitiveValue primitive2 = Assert.IsType<JsonPrimitiveValue>(arrayValue[1]);
            Assert.NotNull(primitive2.Value);
            Assert.Equal("abc", primitive2.Value);

            // #3 is object
            JsonObjectValue objValue = Assert.IsType<JsonObjectValue>(arrayValue[2]);
            Assert.Equal(2, objValue.Count);

            // #4 is empty array
            JsonArrayValue subArray = Assert.IsType<JsonArrayValue>(arrayValue[3]);
            Assert.Empty(subArray);
        }

        [Fact]
        public void ReadAsArrayThrowsForNonObjectValue()
        {
            string json = " { anything }";

            Action test = () => ReadAsJsonArray(json);

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(test);
            Assert.Equal(Strings.CsdlJsonReader_UnexpectedJsonNodeType(JsonNodeType.StartObject, JsonNodeType.StartArray),
                exception.Message);
        }

        #region Helper methods

        private static JsonPrimitiveValue ReadAsJsonPrimitive(string value)
        {
            string json = "{" + "\"data\":" + value + "}";
            using (TextReader txtReader = new StringReader(json))
            {
                IJsonReader jsonReader = new JsonReader(txtReader);
                jsonReader.Read(); // root
                jsonReader.Read(); // {
                jsonReader.Read(); // data
                return jsonReader.ReadAsPrimitive();
            }
        }

        private static JsonObjectValue ReadAsJsonObejct(string json)
        {
            using (TextReader txtReader = new StringReader(json))
            {
                IJsonReader jsonReader = new JsonReader(txtReader);
                return jsonReader.ReadAsObject();
            }
        }

        private static JsonArrayValue ReadAsJsonArray(string json)
        {
            using (TextReader txtReader = new StringReader(json))
            {
                IJsonReader jsonReader = new JsonReader(txtReader);
                return jsonReader.ReadAsArray();
            }
        }
        #endregion
    }
}