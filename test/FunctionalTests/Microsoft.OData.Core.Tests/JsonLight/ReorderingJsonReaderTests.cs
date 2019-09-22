//---------------------------------------------------------------------
// <copyright file="ReorderingJsonReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using Microsoft.OData.Json;
using Microsoft.OData.JsonLight;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ReorderingJsonReaderTests
    {
        [Fact]
        public void TypeShouldBeMovedToTop()
        {
            var json = @"
            { 
                ""@odata.editlink"":""RelativeUrl"",
                ""@foo.bla"": 4,
                ""@odata.type"": ""SomeEntityType""
            }";

            var reader = CreateReorderingReaderPositionedOnFirstProperty(json);
            Assert.Equal("@odata.type", reader.ReadPropertyName());
            Assert.Equal("SomeEntityType", reader.ReadPrimitiveValue());
        }

        [Fact]
        public void ETagShouldBeMovedToTop()
        {
            var json = @"
            { 
                ""@odata.editlink"":""RelativeUrl"",
                ""@foo.bla"": 4,
                ""@odata.etag"": ""etag-val""
            }";

            var reader = CreateReorderingReaderPositionedOnFirstProperty(json);
            Assert.Equal("@odata.etag", reader.ReadPropertyName());
            Assert.Equal("etag-val", reader.ReadPrimitiveValue());
        }

        [Fact]
        public void IdShouldBeMovedToTop()
        {
            var json = @"
            { 
                ""@odata.editlink"":""RelativeUrl"",
                ""@foo.bla"": 4,
                ""@odata.id"": 42
            }";

            var reader = CreateReorderingReaderPositionedOnFirstProperty(json);
            Assert.Equal("@odata.id", reader.ReadPropertyName());
            Assert.Equal(42, reader.ReadPrimitiveValue());
        }

        [Fact]
        public void CorrectOrderShouldBeTypeThenIdThenEtag()
        {
            const string json = @"
            { 
                ""@odata.editlink"":""RelativeUrl"",
                ""@foo.bla"": 4,
                ""data"": 3.1,
                ""@odata.etag"": ""etag-val"",
                ""@odata.type"": ""SomeEntityType"",
                ""@odata.id"": 42
            }";

            var reader = CreateReorderingReaderPositionedOnFirstProperty(json);

            // Expect type name first.
            Assert.Equal("@odata.type", reader.ReadPropertyName());
            Assert.Equal("SomeEntityType", reader.ReadPrimitiveValue());

            // Per the protocol, odata.id and odata.etag can be in either order relative to each other,
            // but we'll (arbitarily) lock down "id" before "etag" for our reordering reader.
            Assert.Equal("@odata.id", reader.ReadPropertyName());
            Assert.Equal(42, reader.ReadPrimitiveValue());
            Assert.Equal("@odata.etag", reader.ReadPropertyName());
            Assert.Equal("etag-val", reader.ReadPrimitiveValue());
        }

        /// <summary>
        /// Creates a new <see cref="ReorderingJsonReader"/> and advances it to the first property node
        /// </summary>
        /// <param name="json">The json string to potentially reorder and read.</param>
        /// <returns>The created json reader.</returns>
        private static ReorderingJsonReader CreateReorderingReaderPositionedOnFirstProperty(string json)
        {
            var stringReader = new StringReader(json);
            var innerReader = new JsonReader(stringReader, isIeee754Compatible: true);
            var reader = new ReorderingJsonReader(innerReader, maxInnerErrorDepth: 0);

            Assert.Equal(JsonNodeType.None, reader.NodeType);
            reader.Read();
            Assert.Equal(JsonNodeType.StartObject, reader.NodeType);
            reader.Read();
            Assert.Equal(JsonNodeType.Property, reader.NodeType);
            return reader;
        }
    }
}
