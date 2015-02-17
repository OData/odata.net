//---------------------------------------------------------------------
// <copyright file="ReorderingJsonReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.OData.TDD.Tests.Reader.JsonLight
{
    using System.IO;
    using Microsoft.OData.Core.Json;
    using Microsoft.OData.Core.JsonLight;
    using FluentAssertions;

    [TestClass]
    public class ReorderingJsonReaderTests
    {
        [TestMethod]
        public void TypeShouldBeMovedToTop()
        {
            var json = @"
            { 
                ""@odata.editlink"":""RelativeUrl"",
                ""@foo.bla"": 4,
                ""@odata.type"": ""SomeEntityType""
            }";

            var reader = CreateReorderingReaderPositionedOnFirstProperty(json);
            reader.ReadPropertyName().Should().Be("@odata.type");
            reader.ReadPrimitiveValue().Should().Be("SomeEntityType");
        }

        [TestMethod]
        public void ETagShouldBeMovedToTop()
        {
            var json = @"
            { 
                ""@odata.editlink"":""RelativeUrl"",
                ""@foo.bla"": 4,
                ""@odata.etag"": ""etag-val""
            }";

            var reader = CreateReorderingReaderPositionedOnFirstProperty(json);
            reader.ReadPropertyName().Should().Be("@odata.etag");
            reader.ReadPrimitiveValue().Should().Be("etag-val");
        }

        [TestMethod]
        public void IdShouldBeMovedToTop()
        {
            var json = @"
            { 
                ""@odata.editlink"":""RelativeUrl"",
                ""@foo.bla"": 4,
                ""@odata.id"": 42
            }";

            var reader = CreateReorderingReaderPositionedOnFirstProperty(json);
            reader.ReadPropertyName().Should().Be("@odata.id");
            reader.ReadPrimitiveValue().Should().Be(42);
        }

        [TestMethod]
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
            reader.ReadPropertyName().Should().Be("@odata.type");
            reader.ReadPrimitiveValue().Should().Be("SomeEntityType");

            // Per the protocol, odata.id and odata.etag can be in either order relative to each other,
            // but we'll (arbitarily) lock down "id" before "etag" for our reordering reader.
            reader.ReadPropertyName().Should().Be("@odata.id");
            reader.ReadPrimitiveValue().Should().Be(42);
            reader.ReadPropertyName().Should().Be("@odata.etag");
            reader.ReadPrimitiveValue().Should().Be("etag-val");
        }

        /// <summary>
        /// Creates a new <see cref="ReorderingJsonReader"/> and advances it to the first property node
        /// </summary>
        /// <param name="json">The json string to potentially reorder and read.</param>
        /// <returns>The created json reader.</returns>
        private static ReorderingJsonReader CreateReorderingReaderPositionedOnFirstProperty(string json)
        {
            var reader = new ReorderingJsonReader(new StringReader(json), maxInnerErrorDepth: 0, isIeee754Comaptible: true);

            reader.NodeType.Should().Be(JsonNodeType.None);
            reader.Read();
            reader.NodeType.Should().Be(JsonNodeType.StartObject);
            reader.Read();
            reader.NodeType.Should().Be(JsonNodeType.Property);
            return reader;
        }
    }
}
