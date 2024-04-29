//---------------------------------------------------------------------
// <copyright file="JsonMinimalMetadataLevelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class JsonMinimalMetadataLevelTests
    {
        private readonly JsonMinimalMetadataLevel testSubject = new JsonMinimalMetadataLevel();

        [Fact]
        public void MinimalMetadataLevelShouldReturnMinimalMetadataTypeOracleWhenKnobIsSet()
        {
            Assert.IsType<JsonMinimalMetadataTypeNameOracle>(testSubject.GetTypeNameOracle());
        }

        [Fact]
        public void MinimalMetadataLevelShouldReturnNullForCreateEntityMetadataBuilder()
        {
            Assert.Null(testSubject.CreateResourceMetadataBuilder(
                new ODataResource(),
                /*typeContext*/ null,
                /*serializationInfo*/ null,
                /*actualEntityType*/ null,
                new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree),
                /*isResponse*/ true,
                /*keyAsSegment*/ false,
                /*requestUri*/ null,
                 /*settings*/null));
        }

        [Fact]
        public void InjectMetadataBuilderShouldNotSetBuilderOnEntry()
        {
            var entry = new ODataResource();
            var builder = new TestEntityMetadataBuilder(entry);
            testSubject.InjectMetadataBuilder(entry, builder);
            Assert.IsType<NoOpResourceMetadataBuilder>(entry.MetadataBuilder);
        }

        [Fact]
        public void InjectMetadataBuilderShouldNotSetBuilderOnEntryMediaResource()
        {
            var entry = new ODataResource();
            var builder = new TestEntityMetadataBuilder(entry);
            entry.MediaResource = new ODataStreamReferenceValue();
            testSubject.InjectMetadataBuilder(entry, builder);
            Assert.Null(entry.MediaResource.GetMetadataBuilder());
        }

        [Fact]
        public void InjectMetadataBuilderShouldNotSetBuilderOnEntryNamedStreamProperties()
        {
            var entry = new ODataResource();
            var builder = new TestEntityMetadataBuilder(entry);
            var stream1 = new ODataStreamReferenceValue();
            var stream2 = new ODataStreamReferenceValue();
            entry.Properties = new[]
                {
                    new ODataProperty {Name = "Stream1", Value = stream1},
                    new ODataProperty {Name = "Stream2", Value = stream2}
                };
            testSubject.InjectMetadataBuilder(entry, builder);
            Assert.Null(stream1.GetMetadataBuilder());
            Assert.Null(stream2.GetMetadataBuilder());
        }

        [Fact]
        public void InjectMetadataBuilderShouldNotSetBuilderOnEntryActions()
        {
            var entry = new ODataResource();
            var builder = new TestEntityMetadataBuilder(entry);
            var action1 = new ODataAction { Metadata = new Uri("http://service/$metadata#action1", UriKind.Absolute) };
            var action2 = new ODataAction { Metadata = new Uri("http://service/$metadata#action2", UriKind.Absolute) };

            entry.AddAction(action1);
            entry.AddAction(action2);

            testSubject.InjectMetadataBuilder(entry, builder);
            Assert.Null(action1.GetMetadataBuilder());
            Assert.Null(action2.GetMetadataBuilder());
        }

        [Fact]
        public void InjectMetadataBuilderShouldNotSetBuilderOnEntryFunctions()
        {
            var entry = new ODataResource();
            var builder = new TestEntityMetadataBuilder(entry);
            var function1 = new ODataFunction { Metadata = new Uri("http://service/$metadata#function1", UriKind.Absolute) };
            var function2 = new ODataFunction { Metadata = new Uri("http://service/$metadata#function2", UriKind.Absolute) };

            entry.AddFunction(function1);
            entry.AddFunction(function2);

            testSubject.InjectMetadataBuilder(entry, builder);
            Assert.Null(function1.GetMetadataBuilder());
            Assert.Null(function2.GetMetadataBuilder());
        }
    }
}
