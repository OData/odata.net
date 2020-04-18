//---------------------------------------------------------------------
// <copyright file="JsonReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.OData.Edm.Csdl.Json;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Json
{
    public class JsonReaderTests
    {
        #region Primitive
        [Theory]
        [InlineData("\"\"")]
        [InlineData("\"abc\"")]
        [InlineData("\"   abc'',xyz  \"")]
        public void NormalStringShouldBeReadAsString(string value)
        {
            string expected = value.Trim('"');
            ReadAndTestPrimitiveJson(value, expected);
        }

        [Fact]
        public void NonDottedNumberShouldBeReadAsInt()
        {
            ReadAndTestPrimitiveJson("402", (int)402);
        }

        [Fact]
        public void DottedNumberShouldBeReadAsDecimal()
        {
            ReadAndTestPrimitiveJson("4.02", (decimal)4.02);
        }

        [Fact]
        public void ExponentNumberShouldBeReadAsDouble()
        {
            ReadAndTestPrimitiveJson("-4.02e-2", (double)-4.02e-2);
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        public void BooleanStringShouldBeReadAsBoolean(string value, bool expected)
        {
            ReadAndTestPrimitiveJson(value, expected);
        }

        [Fact]
        public void NullShouldBeReadAsNull()
        {
            ReadAndTestPrimitiveJson("null", (object)null);
        }

        [Theory]
        [InlineData("\"abc\u6211xyz\"")]
        [InlineData("\"abcxyz\u6211\"")]
        [InlineData("\"\u6211abcxyz\"")]
        public void EscapeStringShouldBeReadAsString(string value)
        {
            string expected = value.Replace("\u6211", "我").Trim('"');
            ReadAndTestPrimitiveJson(value, expected);
        }

        private static void ReadAndTestPrimitiveJson<T>(string json, T expected)
        {
            using (TextReader txtReader = new StringReader(json))
            {
                JsonReader jsonReader = new JsonReader(txtReader);

                // go to first token
                jsonReader.Read();
                Assert.Equal(JsonNodeKind.PrimitiveValue, jsonReader.NodeKind);

                object actual = jsonReader.Value;
                jsonReader.Read();
                Assert.Equal(JsonNodeKind.EndOfInput, jsonReader.NodeKind);

                if (expected == null)
                {
                    Assert.Null(actual);
                }
                else
                {
                    T actualValue = Assert.IsType<T>(actual);
                    Assert.Equal(expected, actualValue);
                }
            }
        }
        #endregion

        #region Object
        [Fact]
        public void JsonObjectShouldBeReadAsObject()
        {
            string json = @"
{
  ""category"": ""reference"",
        ""title"": null,
""price"": 8.95
}";
            using (TextReader txtReader = new StringReader(json))
            {
                JsonReader jsonReader = new JsonReader(txtReader);

                jsonReader.Read();
                jsonReader.Read();  // Consume the "{" tag.

                // #1
                string propertyName = (string)jsonReader.Value;
                Assert.Equal("category", propertyName);

                jsonReader.Read();
                string propertyValue = (string)jsonReader.Value;
                Assert.Equal("reference", propertyValue);

                // #2
                jsonReader.Read();
                propertyName = (string)jsonReader.Value;
                Assert.Equal("title", propertyName);

                jsonReader.Read();
                Assert.Null(jsonReader.Value);

                // #3
                jsonReader.Read();
                propertyName = (string)jsonReader.Value;
                Assert.Equal("price", propertyName);

                jsonReader.Read();
                decimal price = (decimal)jsonReader.Value;
                Assert.Equal(8.95m, price);

                jsonReader.Read(); // Consume the "}" tag.

                jsonReader.Read();
                Assert.Equal(JsonNodeKind.EndOfInput, jsonReader.NodeKind);
            }
        }

        [Fact]
        public void MissingCommaInJsonObjectShouldThrow()
        {
            string json = @"
{
  ""category"": ""reference""
        ""title"": null
}";
            using (TextReader txtReader = new StringReader(json))
            {
                JsonReader jsonReader = new JsonReader(txtReader);

                jsonReader.Read();
                jsonReader.Read();  // Consume the "{" tag.

                // #1
                string propertyName = (string)jsonReader.Value;
                Assert.Equal("category", propertyName);

                jsonReader.Read();
                string propertyValue = (string)jsonReader.Value;
                Assert.Equal("reference", propertyValue);

                // #2
                Action test = () => jsonReader.Read();
                Exception exception = Assert.Throws<Exception>(test);
                Assert.Equal(Strings.JsonReader_MissingComma(ScopeType.Object), exception.Message);
            }
        }
        #endregion

        #region Array
        [Fact]
        public void JsonArrayShouldBeReadAsArray()
        {
            string json = @"
[
  ""category"",
   null,               8.95,
   { ""isbn"" : ""19395-8""}
]";
            using (TextReader txtReader = new StringReader(json))
            {
                JsonReader jsonReader = new JsonReader(txtReader);

                jsonReader.Read();
                jsonReader.Read(); // Consume the "[" tag.

                // #1
                string value = (string)jsonReader.Value;
                Assert.Equal("category", value);

                // #2
                jsonReader.Read();
                Assert.Null(jsonReader.Value);

                // #3
                jsonReader.Read();
                decimal price = (decimal)jsonReader.Value;
                Assert.Equal(8.95m, price);

                // #4
                jsonReader.Read();  // Consume the "{" tag.
                jsonReader.Read();
                string propertyName = (string)jsonReader.Value;
                Assert.Equal("isbn", propertyName);

                jsonReader.Read();
                string isbnValue = (string)jsonReader.Value;
                Assert.Equal("19395-8", isbnValue);

                jsonReader.Read(); // Consume the "}" tag.
                jsonReader.Read(); // Consume the "]" tag.
                jsonReader.Read();
                Assert.Equal(JsonNodeKind.EndOfInput, jsonReader.NodeKind);
            }
        }

        [Fact]
        public void MissingCommaInJsonArrayShouldThrow()
        {
            string json = @"
[
  ""category""
   null,               8.95,
   { ""isbn"" : ""19395-8""}
]";
            using (TextReader txtReader = new StringReader(json))
            {
                JsonReader jsonReader = new JsonReader(txtReader);

                jsonReader.Read();
                jsonReader.Read(); // Consume the "[" tag.

                // #1
                string value = (string)jsonReader.Value;
                Assert.Equal("category", value);

                // #2
                Action test = () => jsonReader.Read();
                Exception exception = Assert.Throws<Exception>(test);
                Assert.Equal(Strings.JsonReader_MissingComma(ScopeType.Array), exception.Message);
            }
        }
        #endregion

        [Fact]
        public void WhiteSpaceShouldPassReading()
        {
            string json = "                ";
            using (TextReader txtReader = new StringReader(json))
            {
                JsonReader jsonReader = new JsonReader(txtReader);
                jsonReader.Read();
                Assert.Equal(JsonNodeKind.EndOfInput, jsonReader.NodeKind);
            }
        }

        [Fact]
        public void CommaAtRootShouldThrow()
        {
            string json = "         ,  ,   ";
            using (TextReader txtReader = new StringReader(json))
            {
                JsonReader jsonReader = new JsonReader(txtReader);
                Action test = () => jsonReader.Read();
                Exception exception = Assert.Throws<Exception>(test);
                Assert.Equal(Strings.JsonReader_UnexpectedComma(ScopeType.Root), exception.Message);
            }
        }
    }
}
