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
        #region ReadAsJsonValue
        [Fact]
        public void ReadAsJsonValueThrowsForNullJsonReader()
        {
            IJsonReader jsonReader = null;
            Action test = () => jsonReader.ReadAsJsonValue();
            Assert.Throws<ArgumentNullException>("jsonReader", test);
        }

        [Theory]
        [InlineData("{}", "Object")] // empty object
        [InlineData("{ \"a\": 1 }", "Object")]
        [InlineData("[]", "Array")] // empty array
        [InlineData("[1, 2]", "Array")]
        [InlineData("42", "Primitive")]
        [InlineData("\"abc42\"", "Primitive")]
        [InlineData("6.9", "Primitive")]
        [InlineData("true", "Primitive")]
        public void ReadAsJsonValueWorksForJsonValue(string json, string expected)
        {
            JsonValueKind expectedKind = (JsonValueKind)Enum.Parse(typeof(JsonValueKind), expected);

            IJsonValue jsonValue = Read(json, r => r.ReadAsJsonValue());

            Assert.NotNull(jsonValue);
            Assert.Equal(expectedKind, jsonValue.ValueKind);
        }
        #endregion

        #region ReadAsPrimitive

        [Fact]
        public void ReadAsPrimitiveThrowsForNullJsonReader()
        {
            IJsonReader jsonReader = null;
            Action test = () => jsonReader.ReadAsPrimitive();
            Assert.Throws<ArgumentNullException>("jsonReader", test);
        }

        [Fact]
        public void ReadAsPrimitiveThrowsForNonPrimitiveValue()
        {
            Action test = () => Read("{ object }", r => r.ReadAsPrimitive());

            Exception exception = Assert.Throws<Exception>(test);
            Assert.Equal(Strings.JsonReader_UnexpectedJsonNodeKind(JsonNodeKind.StartObject, JsonNodeKind.PrimitiveValue), exception.Message);
        }

        [Theory]
        [InlineData("\"abc\"", typeof(string))]
        [InlineData(42, typeof(int))]
        [InlineData(6.9, typeof(decimal))]
        [InlineData("true", typeof(bool))]
        public void ReadAsPrimitiveWorksForNonNullValue(string value, Type expected)
        {
            JsonPrimitiveValue primitive = Read(value, r => r.ReadAsPrimitive());

            Assert.NotNull(primitive);
            Assert.NotNull(primitive.Value);
            Assert.Equal(expected, primitive.Value.GetType());
        }

        [Fact]
        public void ReadAsPrimitiveWorksForNullValue()
        {
            JsonPrimitiveValue primitive = Read("null", r => r.ReadAsPrimitive());

            Assert.NotNull(primitive);
            Assert.Null(primitive.Value);
        }
        #endregion

        #region ReadAsObject
        [Fact]
        public void ReadAsObjectThrowsForNullJsonReader()
        {
            IJsonReader jsonReader = null;
            Action test = () => jsonReader.ReadAsObject();
            Assert.Throws<ArgumentNullException>("jsonReader", test);
        }

        [Fact]
        public void ReadAsObjectThrowsForNonJsonObjectValue()
        {
            string json = " [ anything ]";

            Action test = () => Read(json, r => r.ReadAsObject());

            Exception exception = Assert.Throws<Exception>(test);
            Assert.Equal(Strings.JsonReader_UnexpectedJsonNodeKind(JsonNodeKind.StartArray, JsonNodeKind.StartObject),
                exception.Message);
        }

        [Fact]
        public void ReadAsObjectWorksForNormalObjectJson()
        {
            string json = @" {
  ""name"":""John"",
  ""age"": [],
  ""city"": {},
  ""other"": null
}";

            JsonObjectValue objValue = Read(json, r => r.ReadAsObject());

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
        #endregion

        #region ReadAsArrary
        [Fact]
        public void ReadAsArrayThrowsForNullJsonReader()
        {
            IJsonReader jsonReader = null;
            Action test = () => jsonReader.ReadAsArray();
            Assert.Throws<ArgumentNullException>("jsonReader", test);
        }

        [Fact]
        public void ReadAsArrayThrowsForNonObjectValue()
        {
            string json = " { anything }";

            Action test = () => Read(json, r => r.ReadAsArray());

            Exception exception = Assert.Throws<Exception>(test);
            Assert.Equal(Strings.JsonReader_UnexpectedJsonNodeKind(JsonNodeKind.StartObject, JsonNodeKind.StartArray),
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
]"; // intentially add line, whitespace into the string to verify Json reader can process them correctly.

            JsonArrayValue arrayValue = Read(json, r => r.ReadAsArray());

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
        #endregion

        private static T Read<T>(string json, Func<IJsonReader, T> reader) where T : IJsonValue
        {
            using (TextReader txtReader = new StringReader(json))
            {
                IJsonReader jsonReader = new JsonReader(txtReader);
                return reader(jsonReader);
            }
        }
    }
}