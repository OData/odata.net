//---------------------------------------------------------------------
// <copyright file="JsonReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using FluentAssertions;
using Microsoft.OData.Core.Json;
using Xunit;

namespace Microsoft.OData.Core.Tests.Json
{
    public class JsonReaderTests
    {
        [Fact]
        public void DottedNumberShouldBeReadAsDecimal()
        {
            this.CreateJsonLightReader("42.0").ReadPrimitiveValue().Should().BeOfType<Decimal>();
        }

        [Fact]
        public void NonDottedNumberShouldBeReadAsInt()
        {
            this.CreateJsonLightReader("42").ReadPrimitiveValue().Should().BeOfType<Int32>();
        }

        [Fact]
        public void TrueShouldBeReadAsBoolean()
        {
            this.CreateJsonLightReader("true").ReadPrimitiveValue().Should().BeOfType<Boolean>();
        }

        [Fact]
        public void FalseShouldBeReadAsBoolean()
        {
            this.CreateJsonLightReader("false").ReadPrimitiveValue().Should().BeOfType<Boolean>();
        }

        [Fact]
        public void NullShouldBeReadAsNull()
        {
            this.CreateJsonLightReader("null").ReadPrimitiveValue().Should().BeNull();
        }

        [Fact]
        public void QuotedNumberShouldBeReadAsString()
        {
            this.CreateJsonLightReader("\"42\"").ReadPrimitiveValue().Should().BeOfType<String>();
        }

        [Fact]
        public void QuotedISO8601DateTimeShouldBeReadAsString()
        {
            this.CreateJsonLightReader("\"2012-08-14T19:39Z\"").ReadPrimitiveValue().Should().BeOfType<String>();
        }

        [Fact]
        public void QuotedNullShouldBeReadAsString()
        {
            this.CreateJsonLightReader("\"null\"").ReadPrimitiveValue().Should().BeOfType<String>();
        }

        [Fact]
        public void QuotedBooleanValueShouldBeReadAsString()
        {
            this.CreateJsonLightReader("\"true\"").ReadPrimitiveValue().Should().BeOfType<String>();
        }

        [Fact]
        public void QuotedAspNetDateTimeValueShouldBeReadAsStringInJsonLight()
        {
            this.CreateJsonLightReader("\"\\/Date(628318530718)\\/\"").ReadPrimitiveValue().Should().BeOfType<String>();
        }

        private JsonReader CreateJsonLightReader(string jsonValue)
        {
            JsonReader reader = new JsonReader(new StringReader(String.Format("{{ \"data\" : {0} }}", jsonValue)), ODataFormat.Json, isIeee754Compatible: false);
            reader.Read();
            reader.ReadStartObject();
            reader.ReadPropertyName();
            reader.NodeType.Should().Be(JsonNodeType.PrimitiveValue);

            return reader;
        }
    }
}
