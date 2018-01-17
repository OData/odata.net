//---------------------------------------------------------------------
// <copyright file="ODataMissingOperationGeneratorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using Microsoft.OData.JsonLight;
using Xunit;

namespace Microsoft.OData.Tests.Evaluation
{
    public class ODataMissingOperationGeneratorTests
    {
        private readonly EdmModel model;
        private readonly EdmEntityContainer container;
        private readonly EdmAction actionEdmMetadata;
        private readonly EdmFunction functionEdmMetadata;
        private readonly EdmOperation[] allOperations;
        private readonly ODataAction odataAction;
        private readonly ODataFunction odataFunction;

        private ODataResource entry;
        private EdmEntityType entityType;

        public ODataMissingOperationGeneratorTests()
        {
            this.model = new EdmModel();
            this.container = new EdmEntityContainer("Fake", "Container");
            this.functionEdmMetadata = new EdmFunction("Fake", "FakeFunction", EdmCoreModel.Instance.GetInt32(false), true /*isBound*/, null, true /*isComposable*/);
            this.actionEdmMetadata = new EdmAction("Fake", "FakeAction", EdmCoreModel.Instance.GetInt32(false), true/*isBound*/, null /*entitySetPath*/);
            this.model.AddElement(this.container);
            this.model.AddElement(this.actionEdmMetadata);
            this.model.AddElement(this.functionEdmMetadata);

            this.allOperations = new EdmOperation[] { this.actionEdmMetadata, this.functionEdmMetadata };

            this.odataAction = new ODataAction { Metadata = new Uri("http://temp.org/$metadata#Fake.FakeAction") };
            this.odataFunction = new ODataFunction { Metadata = new Uri("http://temp.org/$metadata#Fake.FakeFunction") };

            this.entry = ReaderUtils.CreateNewResource();
            this.entityType = new EdmEntityType("TestNamespace", "EntityType");
        }

        [Fact]
        public void NoOperationsShouldBeGeneratedIfModelHasNone()
        {
            AddMissingOperations(this.entry, this.entityType, SelectedPropertiesNode.EntireSubtree, this.model, type => new IEdmOperation[0], null, e => false);
            entry.Actions.Should().BeEmpty();
            entry.Functions.Should().BeEmpty();
        }

        [Fact]
        public void NoOperationsShouldBeGeneratedIfAllArePresent()
        {
            this.entry.AddAction(this.odataAction);
            this.entry.AddFunction(this.odataFunction);
            AddMissingOperations(this.entry, this.entityType, SelectedPropertiesNode.EntireSubtree, this.model, type => this.allOperations, null, e => false);
            this.entry.Actions.ToList().Count.Should().Be(1);
#if !NETCOREAPP1_0
            this.entry.Actions.Single().ShouldHave().AllProperties().EqualTo(this.odataAction);
#endif
            this.entry.Functions.ToList().Count.Should().Be(1);
#if !NETCOREAPP1_0
            this.entry.Functions.Single().ShouldHave().AllProperties().EqualTo(this.odataFunction);
#endif
        }

        [Fact]
        public void NoOperationsShouldBeGeneratedIfNoneAreSelected()
        {
            AddMissingOperations(this.entry, this.entityType, SelectedPropertiesNode.Create(string.Empty), this.model, type => this.allOperations, null, e => false);
            entry.Actions.Should().BeEmpty();
            entry.Functions.Should().BeEmpty();
        }

        [Fact]
        public void SelectedActionShouldBeGenerated()
        {
            this.AddMissingOperationsForAll(SelectedPropertiesNode.Create(this.actionEdmMetadata.Name));

            this.entry.Functions.Should().BeEmpty();

            this.entry.Actions.Should().HaveCount(1);
#if !NETCOREAPP1_0
            this.entry.Actions.Single().ShouldHave().AllProperties().EqualTo(this.odataAction);
#endif
        }

        [Fact]
        public void SelectedFunctionShouldBeGenerated()
        {
            this.AddMissingOperationsForAll(SelectedPropertiesNode.Create(this.functionEdmMetadata.Name));

            this.entry.Actions.Should().BeEmpty();

            this.entry.Functions.Should().HaveCount(1);
#if !NETCOREAPP1_0
            this.entry.Functions.Single().ShouldHave().AllProperties().EqualTo(this.odataFunction);
#endif
        }

        [Fact]
        public void SelectedFunctionWithoutContainerQualifierShouldNotBeGeneratedForOpenType()
        {
            AddMissingOperations(this.entry, this.entityType, SelectedPropertiesNode.Create(this.functionEdmMetadata.Name), this.model, type => this.allOperations, entry => new NoOpResourceMetadataBuilder(entry), e => true);

            this.entry.Actions.Should().BeEmpty();
            this.entry.Functions.Should().BeEmpty();
        }

        private void AddMissingOperationsForAll(SelectedPropertiesNode selectedProperties)
        {
            AddMissingOperations(this.entry, this.entityType, selectedProperties, this.model, type => this.allOperations, entry => new NoOpResourceMetadataBuilder(entry), e => false);
        }

        private static void AddMissingOperations(ODataResource entry, IEdmEntityType entityType, SelectedPropertiesNode selectedProperties, IEdmModel model, Func<IEdmType, IEdmOperation[]> getOperations, Func<ODataResourceBase, ODataResourceMetadataBuilder> getEntityMetadataBuilder = null, Func<IEdmStructuredType, bool> typeIsOpen = null)
        {
            var metadataContext = new TestMetadataContext
            {
                GetModelFunc = () => model,
                GetMetadataDocumentUriFunc = () => new Uri("http://temp.org/$metadata"),
                GetServiceBaseUriFunc = () => new Uri("http://temp.org/"),
                GetBindableOperationsForTypeFunc = getOperations,
                GetEntityMetadataBuilderFunc = getEntityMetadataBuilder,
                OperationsBoundToStructuredTypeMustBeContainerQualifiedFunc = typeIsOpen,
            };

            var entryContext = ODataResourceMetadataContext.Create(entry, new TestFeedAndEntryTypeContext(), /*serializationInfo*/null, entityType, metadataContext, selectedProperties);
            var generator = new ODataMissingOperationGenerator(entryContext, metadataContext);
            List<ODataAction> actions = generator.GetComputedActions().ToList();
            List<ODataFunction> functions = generator.GetComputedFunctions().ToList();
            actions.ForEach(entry.AddAction);
            functions.ForEach(entry.AddFunction);
        }
    }

    internal class TestMetadataContext : IODataMetadataContext
    {
        public Func<IEdmModel> GetModelFunc { get; set; }
        public Func<Uri> GetMetadataDocumentUriFunc { get; set; }
        public Func<Uri> GetServiceBaseUriFunc { get; set; }
        public Func<IEdmType, IEdmOperation[]> GetBindableOperationsForTypeFunc { get; set; }
        public Func<ODataResourceBase, ODataResourceMetadataBuilder> GetEntityMetadataBuilderFunc { get; set; }
        public Func<IEdmStructuredType, bool> OperationsBoundToStructuredTypeMustBeContainerQualifiedFunc { get; set; }

        public IEdmModel Model
        {
            get
            {
                if (this.GetModelFunc != null)
                {
                    return this.GetModelFunc();
                }

                return null;
            }
        }

        public Uri MetadataDocumentUri
        {
            get
            {
                if (this.GetMetadataDocumentUriFunc != null)
                {
                    return this.GetMetadataDocumentUriFunc();
                }

                throw new NotImplementedException();
            }
        }

        public Uri ServiceBaseUri
        {
            get
            {
                if (this.GetServiceBaseUriFunc != null)
                {
                    return this.GetServiceBaseUriFunc();
                }

                throw new NotImplementedException();
            }
        }

        public ODataResourceMetadataBuilder GetResourceMetadataBuilderForReader(IODataJsonLightReaderResourceState entryState, bool useKeyAsSegment)
        {
            if (this.GetEntityMetadataBuilderFunc != null)
            {
                return this.GetEntityMetadataBuilderFunc(entryState.Resource);
            }

            throw new NotImplementedException();
        }

        public IEdmOperation[] GetBindableOperationsForType(IEdmType bindingType)
        {
            if (this.GetBindableOperationsForTypeFunc != null)
            {
                return this.GetBindableOperationsForTypeFunc(bindingType);
            }

            throw new NotImplementedException();
        }

        public bool OperationsBoundToStructuredTypeMustBeContainerQualified(IEdmStructuredType entityType)
        {
            if (this.OperationsBoundToStructuredTypeMustBeContainerQualifiedFunc != null)
            {
                return this.OperationsBoundToStructuredTypeMustBeContainerQualifiedFunc(entityType);
            }

            throw new NotImplementedException();
        }

        public ODataUri ODataUri { get; set; }
    }
}