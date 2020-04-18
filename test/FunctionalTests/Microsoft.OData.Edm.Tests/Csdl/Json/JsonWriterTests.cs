//---------------------------------------------------------------------
// <copyright file="JsonWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using Microsoft.OData.Edm.Csdl.Json;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Json
{
    public class JsonWriterTests
    {
        [Fact]
        public void CanWritePrimitiveValueString()
        {
            string json = Write(w => w.WriteValue("88"));
            Assert.Equal("\"88\"", json);
        }

        [Theory]
        [InlineData(true, "true")]
        [InlineData(false, "false")]
        public void CanWritePrimitiveValueBoolean(bool value, string expected)
        {
            string json = Write(w => w.WriteValue(value));
            Assert.Equal(expected, json);
        }

        [Fact]
        public void CanWritePrimitiveValueInt32()
        {
            string json = Write(w => w.WriteValue(88));
            Assert.Equal("88", json);
        }

        [Theory]
        [InlineData(true, "\"88.08\"")]
        [InlineData(false, "88.08")]
        public void CanWritePrimitiveValueDecimal(bool isIeee754Compatible, string expected)
        {
            JsonWriterOptions options = new JsonWriterOptions
            {
                IsIeee754Compatible = isIeee754Compatible
            };

            string json = Write(options, w => w.WriteValue(88.08m));
            Assert.Equal(expected, json);
        }

        [Theory]
        [InlineData(true, "\"9223372036854775807\"")]
        [InlineData(false, "9223372036854775807")]
        public void CanWritePrimitiveValueLong(bool isIeee754Compatible, string expected)
        {
            JsonWriterOptions options = new JsonWriterOptions
            {
                IsIeee754Compatible = isIeee754Compatible
            };

            string json = Write(options, w => w.WriteValue(long.MaxValue));
            Assert.Equal(expected, json);
        }

        [Fact]
        public void CanWritePrimitiveValueDouble()
        {
            string json = Write(w => w.WriteValue((double)88.08));
            Assert.Equal("88.08", json);
        }

        [Fact]
        public void CanWriteJsonObject()
        {
            string json = Write(w =>
            {
                w.StartObjectScope();
                    w.WritePropertyName("category");
                    w.WriteValue("reference");
                    w.WritePropertyName("title");
                    w.WriteNull();
                w.EndObjectScope();
            });

            string expected = @"{
  ""category"": ""reference"",
  ""title"": null
}";

            Assert.Equal(expected, json);
        }

        [Fact]
        public void CanWriteJsonArray()
        {
            string json = Write(w =>
            {
                w.StartArrayScope();
                    w.WriteValue(true);
                    w.StartObjectScope();
                        w.WritePropertyName("category");
                        w.WriteValue("reference");
                        w.WritePropertyName("title");
                        w.WriteNull();
                    w.EndObjectScope();
                    w.WriteNull();
                w.EndArrayScope();
            });

            string expected = @"[
  true,
  {
    ""category"": ""reference"",
    ""title"": null
  },
  null
]";
            Assert.Equal(expected, json);
        }

        internal static string Write(Action<JsonWriter> action)
        {
            return Write(JsonWriterOptions.Default, action);
        }

        internal static string Write(JsonWriterOptions options, Action<JsonWriter> action)
        {
            if (options == null)
            {
                options = JsonWriterOptions.Default;
            }

            StringBuilder sb = new StringBuilder();
            JsonWriter jsonWriter = new JsonWriter(new StringWriter(sb), options);
            action(jsonWriter);
            return sb.ToString();
        }
    }
}
