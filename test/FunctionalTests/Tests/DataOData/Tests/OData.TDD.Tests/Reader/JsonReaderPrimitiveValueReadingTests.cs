//---------------------------------------------------------------------
// <copyright file="JsonReaderPrimitiveValueReadingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Reader
{
    using System;
    using System.IO;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JsonReaderPrimitiveValueReadingTests
    {
        [TestMethod]
        public void DottedNumberShouldBeReadAsDecimal()
        {
            this.CreateJsonLightReader("42.0").ReadPrimitiveValue().Should().BeOfType<Decimal>();
        }

        [TestMethod]
        public void NonDottedNumberShouldBeReadAsInt()
        {
            this.CreateJsonLightReader("42").ReadPrimitiveValue().Should().BeOfType<Int32>();
        }

        [TestMethod]
        public void TrueShouldBeReadAsBoolean()
        {
            this.CreateJsonLightReader("true").ReadPrimitiveValue().Should().BeOfType<Boolean>();
        }

        [TestMethod]
        public void FalseShouldBeReadAsBoolean()
        {
            this.CreateJsonLightReader("false").ReadPrimitiveValue().Should().BeOfType<Boolean>();
        }

        [TestMethod]
        public void NullShouldBeReadAsNull()
        {
            this.CreateJsonLightReader("null").ReadPrimitiveValue().Should().BeNull();
        }

        [TestMethod]
        public void QuotedNumberShouldBeReadAsString()
        {
            this.CreateJsonLightReader("\"42\"").ReadPrimitiveValue().Should().BeOfType<String>();
        }

        [TestMethod]
        public void QuotedISO8601DateTimeShouldBeReadAsString()
        {
            this.CreateJsonLightReader("\"2012-08-14T19:39Z\"").ReadPrimitiveValue().Should().BeOfType<String>();
        }

        [TestMethod]
        public void QuotedNullShouldBeReadAsString()
        {
            this.CreateJsonLightReader("\"null\"").ReadPrimitiveValue().Should().BeOfType<String>();
        }

        [TestMethod]
        public void QuotedBooleanValueShouldBeReadAsString()
        {
            this.CreateJsonLightReader("\"true\"").ReadPrimitiveValue().Should().BeOfType<String>();
        }

        [TestMethod]
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
