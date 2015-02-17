//---------------------------------------------------------------------
// <copyright file="JsonNoMetadataLevelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Writer.JsonLight
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.JsonLight;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.Test.OData.TDD.Tests.Evaluation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JsonNoMetadataLevelTests
    {
        private readonly JsonNoMetadataLevel testSubject = new JsonNoMetadataLevel();

        [TestMethod]
        public void NoMetadataLevelShouldReturnNoMetadataTypeOracleWhenKnobIsSet()
        {
            testSubject.GetTypeNameOracle( /*autoComputePayloadMetadataInJson*/ true).Should().BeOfType<JsonNoMetadataTypeNameOracle>();
        }

        [TestMethod]
        public void NoMetadataLevelShouldReturnMinimalMetadataTypeOracleWhenKnobIsOff()
        {
            testSubject.GetTypeNameOracle( /*autoComputePayloadMetadataInJson*/ false).Should().BeOfType<JsonMinimalMetadataTypeNameOracle>();
        }
        
        [TestMethod]
        public void NoMetadataLevelShouldHaveContextUrlLevelNone()
        {
            testSubject.ContextUrlLevel.Should().Be(ODataContextUrlLevel.None);
        }

        [TestMethod]
        public void NoMetadataLevelShouldReturnNullMetadataBuilder()
        {
            testSubject.CreateEntityMetadataBuilder(
                new ODataEntry(), 
                /*typeContext*/ null,
                /*serializationInfo*/ null,
                /*actualEntityType*/ null,
                SelectedPropertiesNode.EntireSubtree, 
                /*isResponse*/ true,
                /*keyAsSegment*/ false,
                /*requestUri*/ null).Should().Be(ODataEntityMetadataBuilder.Null);
        }

        [TestMethod]
        public void InjectMetadataBuilderShouldSetBuilderOnEntry()
        {
            var entry = new ODataEntry();
            var builder = new TestEntityMetadataBuilder(entry);
            testSubject.InjectMetadataBuilder(entry, builder);
            entry.MetadataBuilder.Should().BeSameAs(builder);
        }

        [TestMethod]
        public void InjectMetadataBuilderShouldNotSetBuilderOnEntryMediaResource()
        {
            var entry = new ODataEntry();
            var builder = new TestEntityMetadataBuilder(entry);
            entry.MediaResource = new ODataStreamReferenceValue();
            testSubject.InjectMetadataBuilder(entry, builder);
            entry.MediaResource.GetMetadataBuilder().Should().BeNull();
        }

        [TestMethod]
        public void InjectMetadataBuilderShouldNotSetBuilderOnEntryNamedStreamProperties()
        {
            var entry = new ODataEntry();
            var builder = new TestEntityMetadataBuilder(entry);
            var stream1 = new ODataStreamReferenceValue();
            var stream2 = new ODataStreamReferenceValue();
            entry.Properties = new[]
                {
                    new ODataProperty {Name = "Stream1", Value = stream1},
                    new ODataProperty {Name = "Stream2", Value = stream2}
                };
            testSubject.InjectMetadataBuilder(entry, builder);
            stream1.GetMetadataBuilder().Should().BeNull();
            stream2.GetMetadataBuilder().Should().BeNull();
        }

        [TestMethod]
        public void InjectMetadataBuilderShouldNotSetBuilderOnEntryActions()
        {
            var entry = new ODataEntry();
            var builder = new TestEntityMetadataBuilder(entry);
            var action1 = new ODataAction { Metadata = new Uri("http://service/$metadata#action1", UriKind.Absolute) };
            var action2 = new ODataAction { Metadata = new Uri("http://service/$metadata#action2", UriKind.Absolute) };

            entry.AddAction(action1);
            entry.AddAction(action2);

            testSubject.InjectMetadataBuilder(entry, builder);
            action1.GetMetadataBuilder().Should().BeNull();
            action2.GetMetadataBuilder().Should().BeNull();
        }

        [TestMethod]
        public void InjectMetadataBuilderShouldNotSetBuilderOnEntryFunctions()
        {
            var entry = new ODataEntry();
            var builder = new TestEntityMetadataBuilder(entry);
            var function1 = new ODataFunction { Metadata = new Uri("http://service/$metadata#function1", UriKind.Absolute) };
            var function2 = new ODataFunction { Metadata = new Uri("http://service/$metadata#function2", UriKind.Absolute) };
            
            entry.AddFunction(function1);
            entry.AddFunction(function2);

            testSubject.InjectMetadataBuilder(entry, builder);
            function1.GetMetadataBuilder().Should().BeNull();
            function2.GetMetadataBuilder().Should().BeNull();
        }
    }
}
