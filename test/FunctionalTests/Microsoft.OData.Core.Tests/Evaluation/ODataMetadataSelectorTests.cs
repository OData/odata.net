//---------------------------------------------------------------------
// <copyright file="JsonFullMetadataLevelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Json;
using Microsoft.OData.Tests.Evaluation;
using Xunit;
using Microsoft.OData.Tests.UriParser;

namespace Microsoft.OData.Tests.Json
{
    public class ODataMetadataSelectorTests
    {
        private static readonly Uri MetadataDocumentUri;
        private readonly static JsonFullMetadataLevel fullMetadataLevel;

        private readonly static JsonMinimalMetadataLevel minimalMetadataLevel = new JsonMinimalMetadataLevel();

        private static IEdmModel Model;
        private static readonly TestFeedAndEntryTypeContext personTypeContext;
        private static readonly TestFeedAndEntryTypeContext managerTypeContext;
        private static readonly TestFeedAndEntryTypeContext dogStreamContext;
        private static ODataResource resource;

        private const string EntitySetName = "People";
        private const string EntityTypeName = "Fully.Qualified.Namespace.Person";
        private const string DerivedEntityTypeName = "Fully.Qualified.Namespace.Manager";
        private const string DogEntitySetName = "Dog";
        private const string DogEntityTypeName = "Fully.Qualified.Namespace.Dog";



        static ODataMetadataSelectorTests()
        {
            MetadataDocumentUri = new Uri("http://submodel1/", UriKind.Absolute);

            GenerateModel();
            
            resource = new ODataResource();
            fullMetadataLevel = new JsonFullMetadataLevel(MetadataDocumentUri, Model);
            minimalMetadataLevel = new JsonMinimalMetadataLevel();

            personTypeContext = new TestFeedAndEntryTypeContext
            {
                NavigationSourceName = EntitySetName,
                NavigationSourceEntityTypeName = EntityTypeName,
                ExpectedResourceTypeName = EntityTypeName,
                IsMediaLinkEntry = false,
                IsFromCollection = false,
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet
            };

            managerTypeContext = new TestFeedAndEntryTypeContext
            {
                NavigationSourceName = EntitySetName,
                NavigationSourceEntityTypeName = DerivedEntityTypeName,
                ExpectedResourceTypeName = DerivedEntityTypeName,
                IsMediaLinkEntry = false,
                IsFromCollection = false,
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet
            };

            dogStreamContext = new TestFeedAndEntryTypeContext
            {
                NavigationSourceName = DogEntitySetName,
                NavigationSourceEntityTypeName = DogEntityTypeName,
                ExpectedResourceTypeName = DogEntityTypeName,
                IsMediaLinkEntry = false,
                IsFromCollection = false,
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet
            };
        }

        private static void GenerateModel()
        {
            Model = HardCodedTestModel.TestModel;
        }

        [Fact]
        public void OmitOperations()
        {
            var resource = new ODataResource();
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
            settings.MetadataSelector = new TestMetadataSelector();

            ODataResourceMetadataBuilder resourceMetadataBuilder = fullMetadataLevel.CreateResourceMetadataBuilder(
                resource,
                personTypeContext,
                null,
                HardCodedTestModel.GetPersonType(),
                new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree),
                /*isResponse*/ true,
                /*keyAsSegment*/ false,
                /*requestUri*/ null,
                /*settings*/ settings);

            fullMetadataLevel.InjectMetadataBuilder(resource, resourceMetadataBuilder);
            var function = new ODataFunction { Metadata = new Uri(MetadataDocumentUri, "#function1"), };
            var action = new ODataAction { Metadata = new Uri(MetadataDocumentUri, "#action2") };

            resource.AddFunction(function);
            resource.AddAction(action);

            Assert.Same(resourceMetadataBuilder, resource.MetadataBuilder);

            //metadataselector only allows for two HasHat functions to be written as metadata
            Assert.True(resource.Functions.Count() == 3);
            Assert.True(resource.Actions.Count() == 1);
        }

        [Theory]
        [InlineData("MyDog", 7)]
        [InlineData("MyFriendsDogs", 7)]
        [InlineData("DoesNotExist", 8)]
        public void OmitNavigationProperties(string navProperty, int expected)
        {
            var resource = new ODataResource();
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
            TestMetadataSelector selector = new TestMetadataSelector();
            selector.PropertyToOmit = navProperty;
            settings.MetadataSelector = selector;
            ODataResourceMetadataBuilder resourceMetadataBuilder = fullMetadataLevel.CreateResourceMetadataBuilder(
                resource,
                personTypeContext,
                null,
                HardCodedTestModel.GetPersonType(),
                new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree),
                /*isResponse*/ true,
                /*keyAsSegment*/ false,
                /*requestUri*/ null,
                /*settings*/ settings);


            fullMetadataLevel.InjectMetadataBuilder(resource, resourceMetadataBuilder);
            Assert.Same(resourceMetadataBuilder, resource.MetadataBuilder);

            int count = 0;
            while (resource.MetadataBuilder.GetNextUnprocessedNavigationLink() != null)
            {
                count++;
            };

            Assert.True(count == expected);
        }

        [Fact]
        public void ReturnSpecifiedNavigationLinkWithFullMetadata()
        {
            var resource = new ODataResource();
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();

            TestMetadataSelector selector = new TestMetadataSelector();
            selector.NavigationPropertyToReturn = new List<IEdmNavigationProperty>{(IEdmNavigationProperty)HardCodedTestModel.GetPersonType().FindProperty("MyFriendsDogs") };

            settings.MetadataSelector = selector;
            ODataResourceMetadataBuilder resourceMetadataBuilder = fullMetadataLevel.CreateResourceMetadataBuilder(
                resource,
                personTypeContext,
                null,
                HardCodedTestModel.GetPersonType(),
                new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree),
                /*isResponse*/ true,
                /*keyAsSegment*/ false,
                /*requestUri*/ null,
                /*settings*/ settings);

            fullMetadataLevel.InjectMetadataBuilder(resource, resourceMetadataBuilder);
            Assert.Same(resourceMetadataBuilder, resource.MetadataBuilder);

            int count = 0;
            while (resource.MetadataBuilder.GetNextUnprocessedNavigationLink() != null)
            {
                count++;
            };

            Assert.True(count == 1);
        }

        [Theory]
        [InlineData("NamedStream", 0)]
        [InlineData("DoesNotExist",1)]
        public void OmitStreamProperty(string streamPropertyNameToOmit, int expectedCount)
        {
            var resource = new ODataResource();
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();

            TestMetadataSelector selector = new TestMetadataSelector();
            selector.PropertyToOmit = streamPropertyNameToOmit;
            settings.MetadataSelector = selector;
            ODataResourceMetadataBuilder resourceMetadataBuilder = fullMetadataLevel.CreateResourceMetadataBuilder(
                resource,
                personTypeContext,
                null,
                HardCodedTestModel.GetDogType(),
                new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree),
                /*isResponse*/ true,
                /*keyAsSegment*/ false,
                /*requestUri*/ null,
                /*settings*/ settings);

            fullMetadataLevel.InjectMetadataBuilder(resource, resourceMetadataBuilder);
            Assert.Same(resourceMetadataBuilder, resource.MetadataBuilder);

            int count = 0;
            while (resource.MetadataBuilder.GetNextUnprocessedStreamProperty() != null)
            {
                count++;
            };

            Assert.True(count == expectedCount);
        }
    }
}
