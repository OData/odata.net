//---------------------------------------------------------------------
// <copyright file="JsonReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
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
            Assert.IsType<Decimal>(this.CreateJsonLightReader("42.0").ReadPrimitiveValue());
        }

        [Fact]
        public void NonDottedNumberShouldBeReadAsInt()
        {
            Assert.IsType<Int32>(this.CreateJsonLightReader("42").ReadPrimitiveValue());
        }

        [Fact]
        public void TrueShouldBeReadAsBoolean()
        {
            Assert.IsType<Boolean>(this.CreateJsonLightReader("true").ReadPrimitiveValue());
        }

        [Fact]
        public void FalseShouldBeReadAsBoolean()
        {
            Assert.IsType<Boolean>(this.CreateJsonLightReader("false").ReadPrimitiveValue());
        }

        [Fact]
        public void NullShouldBeReadAsNull()
        {
            Assert.Null(this.CreateJsonLightReader("null").ReadPrimitiveValue());
        }

        [Fact]
        public void QuotedNumberShouldBeReadAsString()
        {
            Assert.IsType<String>(this.CreateJsonLightReader("\"42\"").ReadPrimitiveValue());
        }

        [Fact]
        public void QuotedISO8601DateTimeShouldBeReadAsString()
        {
            Assert.IsType<String>(this.CreateJsonLightReader("\"2012-08-14T19:39Z\"").ReadPrimitiveValue());
        }

        [Fact]
        public void QuotedNullShouldBeReadAsString()
        {
            Assert.IsType<String>(this.CreateJsonLightReader("\"null\"").ReadPrimitiveValue());
        }

        [Fact]
        public void QuotedBooleanValueShouldBeReadAsString()
        {
            Assert.IsType<String>(this.CreateJsonLightReader("\"true\"").ReadPrimitiveValue());
        }

        [Fact]
        public void QuotedAspNetDateTimeValueShouldBeReadAsStringInJsonLight()
        {
            Assert.IsType<String>(this.CreateJsonLightReader("\"\\/Date(628318530718)\\/\"").ReadPrimitiveValue());
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
            Assert.Equal(JsonNodeType.PrimitiveValue, reader.NodeType);

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
