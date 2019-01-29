//---------------------------------------------------------------------
// <copyright file="JsonReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using FluentAssertions;
using Microsoft.OData.Buffers;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
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

        [Theory]
        [InlineData("\"abc\u6211xyz\"")]
        [InlineData("\"abcxyz\u6211\"")]
        [InlineData("\"\u6211abcxyz\"")]
        public void EscapeStringShouldBeReadAsString(string value)
        {
            // Arrange & Act
            object actual = this.CreateJsonLightReader(value).ReadPrimitiveValue();

            // Assert
            Assert.IsType<string>(actual);
        }

        private JsonReader CreateJsonLightReader(string jsonValue)
        {
            JsonReader reader = new JsonReader(new StringReader(String.Format("{{ \"data\" : {0} }}", jsonValue)), isIeee754Compatible: false);
            reader.Read();
            reader.ReadStartObject();
            reader.ReadPropertyName();
            reader.NodeType.Should().Be(JsonNodeType.PrimitiveValue);

            return reader;
        }

        [Fact]
        public void ShouldUseArrayPoolIfSet()
        {
            // Arrange
            TestArrayPool pool = new TestArrayPool();
            Assert.Equal(0, pool.RentCount); // guard
            Assert.Equal(0, pool.ReturnCount); // guard
            IJsonReader reader = new JsonReader(new StringReader("[]"), isIeee754Compatible: false)
            {
                ArrayPool = pool
            };

            // Act
            while (reader.Read())
            { }

            // Assert
            Assert.Equal(JsonNodeType.EndOfInput, reader.NodeType);
            Assert.Equal(1, pool.RentCount);
            Assert.Equal(1, pool.ReturnCount);
        }

        public class TestArrayPool : ICharArrayPool
        {
            public int RentCount { get; set; }

            public int ReturnCount { get; set; }

            public char[] Rent(int minSize)
            {
                RentCount++;
                return new char[minSize];
            }

            public void Return(char[] array)
            {
                ReturnCount++;
            }
        }
    }
}
