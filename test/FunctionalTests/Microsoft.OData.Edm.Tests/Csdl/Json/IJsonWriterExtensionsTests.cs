//---------------------------------------------------------------------
// <copyright file="IJsonWriterExtensionsTests.cs" company="Microsoft">
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
    public class IJsonWriterExtensionsTests
    {
        [Fact]
        public void WriteNullPropertyWorks()
        {
            string value = Write(w => w.WriteNullProperty("abc"));

            Assert.Equal(@"{
  ""abc"": null
}", value);
        }

        [Fact]
        public void WriteRequiredPropertyForStringWorks()
        {
            string value = Write(w => w.WriteRequiredProperty("abc", "123"));

            Assert.Equal(@"{
  ""abc"": ""123""
}", value);
        }

        [Fact]
        public void WriteRequiredPropertyForBooleanWorks()
        {
            string value = Write(w => w.WriteRequiredProperty("abc", false));

            Assert.Equal(@"{
  ""abc"": false
}", value);
        }

        [Fact]
        public void WriteRequiredPropertyForLongWorks()
        {
            string value = Write(w => w.WriteRequiredProperty("abc", long.MaxValue));

            Assert.Equal(@"{
  ""abc"": 9223372036854775807
}", value);
        }

        [Fact]
        public void WriteOptionalPropertyForIntegerWorks()
        {
            string value = Write(w => w.WriteOptionalProperty("abc", (int?)null));
            Assert.Equal(@"{ }", value);

            value = Write(w => w.WriteOptionalProperty("abc", (int?)42));
            Assert.Equal(@"{
  ""abc"": 42
}", value);
        }

        internal static string Write(Action<IJsonWriter> action)
        {
            StringBuilder sb = new StringBuilder();
            IJsonWriter jsonWriter = new JsonWriter(new StringWriter(sb));
            jsonWriter.StartObjectScope();
            action(jsonWriter);
            jsonWriter.EndObjectScope();
            return sb.ToString();
        }
    }
}