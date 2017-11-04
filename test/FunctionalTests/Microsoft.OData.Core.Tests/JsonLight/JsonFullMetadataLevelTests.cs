//---------------------------------------------------------------------
// <copyright file="JsonFullMetadataLevelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Tests.Evaluation;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.JsonLight
{
    public class JsonFullMetadataLevelTests
    {
        private static readonly Uri MetadataDocumentUri;
        private static readonly IEdmModel Model;
        private static readonly EdmModel referencedModel;
        private static readonly EdmEntitySet EntitySet;
        private static readonly EdmEntityType EntityType;

        private readonly JsonFullMetadataLevel testSubject = new JsonFullMetadataLevel(MetadataDocumentUri, Model);

        static JsonFullMetadataLevelTests()
        {
            MetadataDocumentUri = new Uri("http://host/service.svc/$metadata", UriKind.Absolute);

            referencedModel = new EdmModel();
            EdmEntityContainer defaultContainer = new EdmEntityContainer("ns", "defaultContainer_sub");
            EntityType = new EdmEntityType("ns", "EntityType");
            EntitySet = new EdmEntitySet(defaultContainer, "Set", EntityType);
            referencedModel.AddElement(defaultContainer);
            Model = TestUtils.WrapReferencedModelsToMainModel("ns", "defaultContainer", referencedModel);
        }

        [Fact]
        public void FullMetadataLevelShouldReturnFullMetadataTypeOracleWhenKnobIsSet()
        {
            testSubject.GetTypeNameOracle().Should().BeOfType<JsonFullMetadataTypeNameOracle>();
        }

        [Fact]
        public void FullMetadataLevelCreateMetadataBuilderWithoutMetadataDocumentUriShouldThrow()
        {
            var metadataLevelWithoutMetadataDocumentUri = new JsonFullMetadataLevel(/*metadataDocumentUri*/ null, Model);

            Action test = () => metadataLevelWithoutMetadataDocumentUri
                .CreateResourceMetadataBuilder(
                new ODataResource(),
                new TestFeedAndEntryTypeContext(),
                new ODataResourceSerializationInfo(),
                /*actualEntityType*/null,
                SelectedPropertiesNode.EntireSubtree,
                /*isResponse*/ true,
                /*keyAsSegment*/ false,
                /*requestUri*/ null);

            test.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataOutputContext_MetadataDocumentUriMissing);
        }

        [Fact]
        public void FullMetadataLevelShouldReturnODataConventionalEntityMetadataBuilder()
        {
            testSubject.CreateResourceMetadataBuilder(
                new ODataResource(),
                new TestFeedAndEntryTypeContext(),
                new ODataResourceSerializationInfo(),
                /*actualEntityType*/null,
                SelectedPropertiesNode.EntireSubtree,
                /*isResponse*/ true,
                /*keyAsSegment*/ false,
                /*requestUri*/ null).Should().BeAssignableTo<ODataConventionalResourceMetadataBuilder>();
        }

        [Fact]
        public void InjectMetadataBuilderShouldSetBuilderOnEntry()
        {
            var entry = new ODataResource();
            var builder = new TestEntityMetadataBuilder(entry);
            testSubject.InjectMetadataBuilder(entry, builder);
            entry.MetadataBuilder.Should().BeSameAs(builder);
        }

        [Fact]
        public void InjectMetadataBuilderShouldSetBuilderOnEntryMediaResource()
        {
            var entry = new ODataResource();
            var builder = new TestEntityMetadataBuilder(entry);
            entry.MediaResource = new ODataStreamReferenceValue();
            testSubject.InjectMetadataBuilder(entry, builder);
            entry.MediaResource.GetMetadataBuilder().Should().BeSameAs(builder);
        }

        [Fact]
        public void InjectMetadataBuilderShouldSetBuilderOnEntryNamedStreamProperties()
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
            stream1.GetMetadataBuilder().Should().BeSameAs(builder);
            stream2.GetMetadataBuilder().Should().BeSameAs(builder);
        }

        [Fact]
        public void InjectMetadataBuilderShouldSetBuilderOnEntryActions()
        {
            var entry = new ODataResource();
            var builder = new TestEntityMetadataBuilder(entry);
            var action1 = new ODataAction { Metadata = new Uri(MetadataDocumentUri, "#action1") };
            var action2 = new ODataAction { Metadata = new Uri(MetadataDocumentUri, "#action2") };

            entry.AddAction(action1);
            entry.AddAction(action2);

            testSubject.InjectMetadataBuilder(entry, builder);
            action1.GetMetadataBuilder().Should().BeSameAs(builder);
            action2.GetMetadataBuilder().Should().BeSameAs(builder);
        }

        [Fact]
        public void InjectMetadataBuilderShouldSetBuilderOnEntryFunctions()
        {
            var entry = new ODataResource();
            var builder = new TestEntityMetadataBuilder(entry);
            var function1 = new ODataFunction { Metadata = new Uri(MetadataDocumentUri, "#function1") };
            var function2 = new ODataFunction { Metadata = new Uri(MetadataDocumentUri, "#function2") };

            entry.AddFunction(function1);
            entry.AddFunction(function2);

            testSubject.InjectMetadataBuilder(entry, builder);
            function1.GetMetadataBuilder().Should().BeSameAs(builder);
            function2.GetMetadataBuilder().Should().BeSameAs(builder);
        }
    }
}
