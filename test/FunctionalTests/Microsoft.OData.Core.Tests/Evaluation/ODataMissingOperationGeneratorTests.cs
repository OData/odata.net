//---------------------------------------------------------------------
// <copyright file="ODataMissingOperationGeneratorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.Evaluation;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.Evaluation
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

        private ODataEntry entry;
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

            this.odataAction = new ODataAction {Metadata = new Uri("http://temp.org/$metadata#Fake.FakeAction")};
            this.odataFunction = new ODataFunction {Metadata = new Uri("http://temp.org/$metadata#Fake.FakeFunction")};

            this.entry = ReaderUtils.CreateNewEntry();
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
            this.entry.Actions.Single().ShouldHave().AllProperties().EqualTo(this.odataAction);
            this.entry.Functions.ToList().Count.Should().Be(1);
            this.entry.Functions.Single().ShouldHave().AllProperties().EqualTo(this.odataFunction);
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
            this.entry.Actions.Single().ShouldHave().AllProperties().EqualTo(this.odataAction);
        }

        [Fact]
        public void SelectedFunctionShouldBeGenerated()
        {
            this.AddMissingOperationsForAll(SelectedPropertiesNode.Create(this.functionEdmMetadata.Name));
            
            this.entry.Actions.Should().BeEmpty();

            this.entry.Functions.Should().HaveCount(1);
            this.entry.Functions.Single().ShouldHave().AllProperties().EqualTo(this.odataFunction);
        }

        [Fact]
        public void SelectedFunctionWithoutContainerQualifierShouldNotBeGeneratedForOpenType()
        {
            AddMissingOperations(this.entry, this.entityType, SelectedPropertiesNode.Create(this.functionEdmMetadata.Name), this.model, type => this.allOperations, entry => new NoOpEntityMetadataBuilder(entry), e => true);

            this.entry.Actions.Should().BeEmpty();
            this.entry.Functions.Should().BeEmpty();
        }

        private void AddMissingOperationsForAll(SelectedPropertiesNode selectedProperties)
        {
            AddMissingOperations(this.entry, this.entityType, selectedProperties, this.model, type => this.allOperations, entry => new NoOpEntityMetadataBuilder(entry), e => false);
        }

        private static void AddMissingOperations(ODataEntry entry, IEdmEntityType entityType, SelectedPropertiesNode selectedProperties, IEdmModel model, Func<IEdmType, IEdmOperation[]> getOperations, Func<ODataEntry, ODataEntityMetadataBuilder> getEntityMetadataBuilder = null, Func<IEdmEntityType, bool> typeIsOpen = null)
        {
            var metadataContext = new TestMetadataContext 
            {
                GetModelFunc = () => model,
                GetMetadataDocumentUriFunc = () => new Uri("http://temp.org/$metadata"),
                GetServiceBaseUriFunc = () => new Uri("http://temp.org/"),
                GetBindableOperationsForTypeFunc = getOperations,
                GetEntityMetadataBuilderFunc = getEntityMetadataBuilder,
                OperationsBoundToEntityTypeMustBeContainerQualifiedFunc = typeIsOpen,
            };

            var entryContext = ODataEntryMetadataContext.Create(entry, new TestFeedAndEntryTypeContext(), /*serializationInfo*/null, entityType, metadataContext, selectedProperties);
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
        public Func<ODataEntry, ODataEntityMetadataBuilder> GetEntityMetadataBuilderFunc { get; set; }
        public Func<IEdmEntityType, bool> OperationsBoundToEntityTypeMustBeContainerQualifiedFunc { get; set; }

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

        public ODataEntityMetadataBuilder GetEntityMetadataBuilderForReader(IODataJsonLightReaderEntryState entryState, bool? useKeyAsSegment)
        {
            if (this.GetEntityMetadataBuilderFunc != null)
            {
                return this.GetEntityMetadataBuilderFunc(entryState.Entry);
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

        public bool OperationsBoundToEntityTypeMustBeContainerQualified(IEdmEntityType entityType)
        {
            if (this.OperationsBoundToEntityTypeMustBeContainerQualifiedFunc != null)
            {
                return this.OperationsBoundToEntityTypeMustBeContainerQualifiedFunc(entityType);
            }

            throw new NotImplementedException();
        }

        public ODataUri ODataUri { get; set; }
    }
}