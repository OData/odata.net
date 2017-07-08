//---------------------------------------------------------------------
// <copyright file="WriterNavigationLinkTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.Json.TextAnnotations;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Atom;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Json;
    using Microsoft.Test.Taupo.OData.Writer.Tests.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
    /// <summary>
    /// Tests for writing expanded links with the OData writer.
    /// </summary>
    // [TestClass, TestCase]
    public class WriterNavigationLinkTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test writing of navigation links in response.")]
        public void NavigationLinksInResponse()
        {
            EdmModel model = new EdmModel();

            var customerType = new EdmEntityType("TestModel", "Customer");
            model.AddElement(customerType);

            var orderType = new EdmEntityType("TestModel", "Order");
            model.AddElement(orderType);

            var ordersNavProp = customerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "Orders", Target = orderType, TargetMultiplicity = EdmMultiplicity.Many });
            var bestFriendNavProp = customerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "BestFriend", Target = customerType, TargetMultiplicity = EdmMultiplicity.One });

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            model.AddElement(container);

            var customerSet = container.AddEntitySet("Customers", customerType);
            var orderSet = container.AddEntitySet("Order", orderType);
            customerSet.AddNavigationTarget(ordersNavProp, orderSet);
            customerSet.AddNavigationTarget(bestFriendNavProp, customerSet);

            ODataResource expandedCustomerInstance = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
            expandedCustomerInstance.TypeName = "TestModel.Customer";
            ODataResource expandedOrderInstance = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
            expandedOrderInstance.TypeName = "TestModel.Order";

            var testCases = new[]
            {
                // Collection deferred link
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] { new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/collection") } },
                    PropertyName = "Orders",
                    Xml = "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/collection") + " />",
                    JsonLight = "{\"" + JsonLightUtils.GetPropertyAnnotationName("Orders", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/collection\"}"
                },
                // Singleton deferred link
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] { new ODataNestedResourceInfo() { IsCollection = false, Name = "BestFriend", Url = new Uri("http://odata.org/singleton") } },
                    PropertyName = "BestFriend",
                    Xml = "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("BestFriend", "application/atom+xml;type=entry", "BestFriend", "http://odata.org/singleton") + " />",
                    JsonLight = "{\"" + JsonLightUtils.GetPropertyAnnotationName("BestFriend", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/singleton\"}"
                },
                // Deferred link with IsCollection set to null - should fail in ATOM only
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] { new ODataNestedResourceInfo() { IsCollection = null, Name = "Orders", Url = new Uri("http://odata.org/collection") } },
                    PropertyName = "Orders",
                    JsonLight = "{\"" + JsonLightUtils.GetPropertyAnnotationName("Orders", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/collection\"}",
                    EdmExpectedExceptionCallback = (tc, m) => null
                },
                // Deferred link with Url set to null - should work only in JSON Light
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] { new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = null } },
                    PropertyName = "Orders",
                    JsonLight = "{}",
                    EdmExpectedExceptionCallback = (tc, m) => tc.Format != ODataFormat.Json ? ODataExpectedExceptions.ODataException("WriterValidationUtils_NavigationLinkMustSpecifyUrl", "Orders") : null
                },
                // Entity reference link in response should fail
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = null, Name = "Orders", Url = new Uri("http://odata.org/collection") },
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/entityrefernce") },
                        null
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataWriterCore_EntityReferenceLinkInResponse")
                },
                // Single expanded entry in a singleton
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = false, Name = "BestFriend", Url = new Uri("http://odata.org/nav") },
                        expandedCustomerInstance,
                        null,
                        null
                    },
                    PropertyName = "BestFriend",
                    Xml = "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("BestFriend", "application/atom+xml;type=entry", "BestFriend", "http://odata.org/nav") + ">" +
                            "<inline xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                GetDefaultEntryXmlAsString("TestModel.Customer") +
                            "</inline>" +
                        "</link>",
                    JsonLight = "{" + JsonLightWriterUtils.CombineProperties(
                            "\"" + JsonLightUtils.GetPropertyAnnotationName("BestFriend", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/nav\"",
                            "\"BestFriend\":{" + JsonLightWriterUtils.CombineProperties(
                                "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryId + "\"",
                                "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataReadLinkAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\"")
                            + "}")
                        + "}"
                },
                // Single expanded entry in a collection - should fail
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        expandedOrderInstance,
                        null,
                        null
                    },
                    EdmExpectedExceptionCallback = (tc, m) => ODataExpectedExceptions.ODataException("WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceContent", "http://odata.org/nav")
                },
                // Two expanded entries in a singleton
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = false, Name = "BestFriend", Url = new Uri("http://odata.org/nav") },
                        expandedCustomerInstance,
                        null,
                        expandedCustomerInstance,
                        null,
                        null
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataWriterCore_MultipleItemsInNestedResourceInfoWithContent")
                },
                // Expanded feed in a collection
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                        null,
                        null
                    },
                    PropertyName = "Orders",
                    Xml = "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/nav") + ">" +
                            "<inline xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                GetDefaultFeedXmlAsString() +
                            "</inline>" +
                        "</link>",
                    JsonLight = "{" + JsonLightWriterUtils.CombineProperties(
                            "\"" + JsonLightUtils.GetPropertyAnnotationName("Orders", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/nav\"",
                            "\"Orders\":[]")
                        + "}"
                },
                // Expanded feed in a singleton
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = false, Name = "BestFriend", Url = new Uri("http://odata.org/nav") },
                        ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                        null,
                        null
                    },
                    EdmExpectedExceptionCallback = (tc, m) => ODataExpectedExceptions.ODataException("WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetContent", "http://odata.org/nav")
                },
                // Two expanded feeds in a singleton
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                        null,
                        ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                        null,
                        null
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataWriterCore_MultipleItemsInNestedResourceInfoWithContent")
                },
            };

            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testDescriptors =
                testCases.SelectMany(testCase =>
                    {
                        PayloadWriterTestDescriptor<ODataItem> withoutModel = testCase.ToEdmTestDescriptor(this.Settings, null, null, null);
                        var skipTestConfiguration = withoutModel.SkipTestConfiguration;
                        withoutModel.SkipTestConfiguration = tc => tc.Format == ODataFormat.Json || (skipTestConfiguration == null ? false : skipTestConfiguration(tc));

                        return new[]
                        {
                            withoutModel,
                            testCase.ToEdmTestDescriptor(this.Settings, model, customerSet, customerType)
                        };
                    });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataEdmPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test writing of navigation links in request.")]
        public void NavigationLinksInRequest()
        {
            EdmModel model = new EdmModel();

            var customerType = new EdmEntityType("TestModel", "Customer");
            model.AddElement(customerType);

            var orderType = new EdmEntityType("TestModel", "Order");
            model.AddElement(orderType);

            var ordersNavProp = customerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "Orders", Target = orderType, TargetMultiplicity = EdmMultiplicity.Many });
            var bestFriendNavProp = customerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "BestFriend", Target = customerType, TargetMultiplicity = EdmMultiplicity.One });

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            model.AddElement(container);

            var customerSet = container.AddEntitySet("Customers", customerType);
            var orderSet = container.AddEntitySet("Order", orderType);
            customerSet.AddNavigationTarget(ordersNavProp, orderSet);
            customerSet.AddNavigationTarget(bestFriendNavProp, customerSet);

            ODataResource expandedOrderInstance = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
            expandedOrderInstance.TypeName = "TestModel.Order";

            ODataResource expandedBestFriendInstance = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
            expandedBestFriendInstance.TypeName = "TestModel.Customer";

            var testCases = new[]
            {
                // Empty navigation link should fail - there must be something in it - collection
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        null
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataWriterCore_DeferredLinkInRequest")
                },
                // Empty navigation link should fail - there must be something in it - singleton
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = false, Name = "BestFriend", Url = new Uri("http://odata.org/nav") },
                        null
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataWriterCore_DeferredLinkInRequest")
                },
                // Entity reference link with IsCollection = null, should fail always in request
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = null, Name = "BestFriend", Url = new Uri("http://odata.org/nav") },
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/singleton") },
                        null
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_NestedResourceInfoMustSpecifyIsCollection", "BestFriend")
                },
                // Single entity reference link in a singleton
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = false, Name = "BestFriend", Url = new Uri("http://odata.org/nav") },
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/singleton") },
                        null
                    },
                    PropertyName = "BestFriend",
                    Xml = "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("BestFriend", "application/atom+xml;type=entry", "BestFriend", "http://odata.org/singleton") + " />",
                    JsonLight =  "{\"BestFriend@" + JsonLightConstants.ODataBindAnnotationName + "\":\"http://odata.org/singleton\"}",
                },
                // Single expanded entry in a singleton
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = false, Name = "BestFriend", Url = new Uri("http://odata.org/nav") },
                        expandedBestFriendInstance,
                        null,
                        null
                    },
                    PropertyName = "BestFriend",
                    Xml = "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("BestFriend", "application/atom+xml;type=entry", "BestFriend", "http://odata.org/nav") + ">" +
                            "<inline xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                GetDefaultEntryXmlAsString("TestModel.Customer") +
                            "</inline>" +
                        "</link>",
                    JsonLight =  "{\"BestFriend\":{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryId + "\",\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataReadLinkAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\"}}",
                },
                // Single expanded feed in a singleton
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = false, Name = "BestFriend", Url = new Uri("http://odata.org/nav") },
                        ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                        null,
                        null
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetContent", "http://odata.org/nav")
                },
                // Multiple expanded entries in a singleton
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = false, Name = "BestFriend", Url = new Uri("http://odata.org/nav") },
                        expandedBestFriendInstance,
                        null,
                        expandedBestFriendInstance,
                        null,
                        null
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataWriterCore_MultipleItemsInNestedResourceInfoWithContent")
                },
                // Expanded entry and entity reference link in a singleton
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = false, Name = "BestFriend", Url = new Uri("http://odata.org/nav") },
                        expandedBestFriendInstance,
                        null,
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/singleton") },
                        null
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataWriterCore_MultipleItemsInNestedResourceInfoWithContent")
                },
                // Expanded entry and entity reference link in a singleton
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = false, Name = "BestFriend", Url = new Uri("http://odata.org/nav") },
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/singleton") },
                        expandedBestFriendInstance,
                        null,
                        null
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataWriterCore_MultipleItemsInNestedResourceInfoWithContent")
                },
                // Multiple entity reference links in a singleton
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = false, Name = "BestFriend", Url = new Uri("http://odata.org/nav") },
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/singleton") },
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/singleton") },
                        null
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataWriterCore_MultipleItemsInNestedResourceInfoWithContent")
                },

                // Single entity reference link in a collection
                // TODO: Is it OK to have type=feed in this case - the URL will point to a singleton
                //   We basically tolerate "entry" type in collection properties, but only for backward compat reasons
                //   otherwise we seem to prefer the "feed" type in this case.
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/collection") },
                        null
                    },
                    PropertyName = "Orders",
                    Xml = "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/collection") + " />",
                    JsonLight =  "{\"Orders@" + JsonLightConstants.ODataBindAnnotationName + "\":[\"http://odata.org/collection\"]}",
                },
                // Two entity reference links in a collection
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/collection") },
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/collection2") },
                        null
                    },
                    PropertyName = "Orders",
                    Xml = "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/collection") + " />" +
                        "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/collection2") + " />",
                    JsonLight =  "{\"Orders@" + JsonLightConstants.ODataBindAnnotationName + "\":[\"http://odata.org/collection\",\"http://odata.org/collection2\"]}",
                },
                // Single expanded feed with no entries in a collection
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                        null,
                        null
                    },
                    PropertyName = "Orders",
                    Xml = "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/nav") + ">" +
                            "<inline xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                GetDefaultFeedXmlAsString() +
                            "</inline>" +
                        "</link>",
                    JsonLight = "{\"Orders\":[]}",
                },
                // Single expanded feed with one entry in a collection
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                        expandedOrderInstance,
                        null,
                        null,
                        null
                    },
                    PropertyName = "Orders",
                    Xml = "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/nav") + ">" +
                            "<inline xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                GetDefaultResourceSetStartXmlAsString() +
                                    GetDefaultEntryXmlAsString("TestModel.Order") +
                                "</feed>" +
                            "</inline>" +
                        "</link>",
                    JsonLight =  "{\"Orders\":[{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryId + "\",\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataReadLinkAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\"}]}",
                },
                // Single expanded feed with two entries in a collection
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                        expandedOrderInstance,
                        null,
                        expandedOrderInstance,
                        null,
                        null,
                        null
                    },
                    PropertyName = "Orders",
                    Xml = "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/nav") + ">" +
                            "<inline xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                GetDefaultResourceSetStartXmlAsString() +
                                    GetDefaultEntryXmlAsString("TestModel.Order") +
                                    GetDefaultEntryXmlAsString("TestModel.Order") +
                                "</feed>" +
                            "</inline>" +
                        "</link>",
                   JsonLight =  "{\"Orders\":[" +
                        "{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryId + "\",\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataReadLinkAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\"}," +
                        "{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryId + "\",\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataReadLinkAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\"}" +
                        "]}",
                },
                // Single expanded feed with no entries and with entity reference link in a collection
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                        null,
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/collection") },
                        null
                    },
                    PropertyName = "Orders",
                    Xml = "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/nav") + ">" +
                            "<inline xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                GetDefaultFeedXmlAsString() +
                            "</inline>" +
                        "</link>" +
                        "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/collection") + " />",
                    // JSON Light requires the entity reference links to come before the feed(s)
                    SkipTestConfiguration = tc => tc.Format == ODataFormat.Json,
                },
                // Single expanded feed with one entry and with entity reference link in a collection
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                            new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/collection") },
                            ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                                expandedOrderInstance,
                                null,
                            null,
                        null
                    },
                    PropertyName = "Orders",
                    Xml =
                        "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/collection") + " />" +
                        "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/nav") + ">" +
                            "<inline xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                GetDefaultResourceSetStartXmlAsString() +
                                    GetDefaultEntryXmlAsString("TestModel.Order") +
                                "</feed>" +
                            "</inline>" +
                        "</link>",
                    JsonLight =  "{" +
                        "\"Orders@" + JsonLightConstants.ODataBindAnnotationName + "\":[\"http://odata.org/collection\"]," +
                        "\"Orders\":[{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryId + "\",\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataReadLinkAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\"}]" +
                        "}",
                },
                // Two expanded feeds with one entry and with entity reference link in a collection
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                            ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                                expandedOrderInstance,
                                null,
                            null,
                            new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/collection") },
                            ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                                expandedOrderInstance,
                                null,
                            null,
                        null
                    },
                    PropertyName = "Orders",
                    Xml =
                        "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/nav") + ">" +
                            "<inline xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                GetDefaultResourceSetStartXmlAsString() +
                                    GetDefaultEntryXmlAsString("TestModel.Order") +
                                "</feed>" +
                            "</inline>" +
                        "</link>" +
                        "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/collection") + " />" +
                        "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/nav") + ">" +
                            "<inline xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                GetDefaultResourceSetStartXmlAsString() +
                                    GetDefaultEntryXmlAsString("TestModel.Order") +
                                "</feed>" +
                            "</inline>" +
                        "</link>",
                   // NOTE: JSON Light does not support mix and match of feeds and entity reference links.
                    SkipTestConfiguration = tc => tc.Format == ODataFormat.Json
                },
                // An entity reference link and two expanded feeds with one entry and in a collection
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                            new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/collection") },
                            ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                                expandedOrderInstance,
                                null,
                            null,
                            ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                                expandedOrderInstance,
                                null,
                            null,
                        null
                    },
                    PropertyName = "Orders",
                    Xml =
                        "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/collection") + " />" +
                        "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/nav") + ">" +
                            "<inline xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                GetDefaultResourceSetStartXmlAsString() +
                                    GetDefaultEntryXmlAsString("TestModel.Order") +
                                "</feed>" +
                            "</inline>" +
                        "</link>" +
                        "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/nav") + ">" +
                            "<inline xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                GetDefaultResourceSetStartXmlAsString() +
                                    GetDefaultEntryXmlAsString("TestModel.Order") +
                                "</feed>" +
                            "</inline>" +
                        "</link>",
                   JsonLight =  "{" +
                        "\"Orders@" + JsonLightConstants.ODataBindAnnotationName + "\":[\"http://odata.org/collection\"]," +
                        "\"Orders\":[" +
                                "{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryId + "\",\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataReadLinkAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\"}," +
                                "{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryId + "\",\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataReadLinkAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\"}" +
                            "]" +
                        "}",
                },
                // Single expanded feed with two entries surrounded by entity reference links in a collection
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/collection") },
                        ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                        expandedOrderInstance,
                        null,
                        expandedOrderInstance,
                        null,
                        null,
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/collection") },
                        null
                    },
                    PropertyName = "Orders",
                    Xml =
                        "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/collection") + " />" +
                        "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/nav") + ">" +
                            "<inline xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                GetDefaultResourceSetStartXmlAsString() +
                                    GetDefaultEntryXmlAsString("TestModel.Order") +
                                    GetDefaultEntryXmlAsString("TestModel.Order") +
                                "</feed>" +
                            "</inline>" +
                        "</link>" +
                        "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/collection") + " />",
                   // NOTE: JSON Light does not support mix and match of feeds and entity reference links.
                    SkipTestConfiguration = tc => tc.Format == ODataFormat.Json
                },
                // Single expanded feed with two entries surrounded preceeded by two entity reference links in a collection
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/collection") },
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/collection") },
                        ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                        expandedOrderInstance,
                        null,
                        expandedOrderInstance,
                        null,
                        null,
                        null
                    },
                    PropertyName = "Orders",
                    Xml =
                        "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/collection") + " />" +
                        "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/collection") + " />" +
                        "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/nav") + ">" +
                            "<inline xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                GetDefaultResourceSetStartXmlAsString() +
                                    GetDefaultEntryXmlAsString("TestModel.Order") +
                                    GetDefaultEntryXmlAsString("TestModel.Order") +
                                "</feed>" +
                            "</inline>" +
                        "</link>",
                   JsonLight =  "{" +
                        "\"Orders@" + JsonLightConstants.ODataBindAnnotationName + "\":[\"http://odata.org/collection\",\"http://odata.org/collection\"]," +
                        "\"Orders\":[" +
                                "{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryId + "\",\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataReadLinkAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\"}," +
                                "{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryId + "\",\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataReadLinkAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\"}" +
                            "]" +
                        "}",
                },
                // Single expanded entry in a collection - should fail
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        ObjectModelUtils.CreateDefaultEntryWithAtomMetadata(),
                        null,
                        null
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceContent", "http://odata.org/nav")
                },
                // Expanded entry after an entity reference link in a collection - should fail
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/collection") },
                        ObjectModelUtils.CreateDefaultEntryWithAtomMetadata(),
                        null,
                        null
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceContent", "http://odata.org/nav")
                },
            };

            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testDescriptors =
                testCases.SelectMany(testCase =>
                {
                    PayloadWriterTestDescriptor<ODataItem> withoutModel = testCase.ToEdmTestDescriptor(this.Settings);
                    var skipTestConfiguration = withoutModel.SkipTestConfiguration;
                    withoutModel.SkipTestConfiguration = tc => tc.Format == ODataFormat.Json || (skipTestConfiguration == null ? false : skipTestConfiguration(tc));

                    return new[]
                        {
                            withoutModel,
                            testCase.ToEdmTestDescriptor(this.Settings, model, customerSet, customerType)
                        };
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataEdmPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test that we protect the caller by failing on deeply nested expanded entries")]
        public void NavigationLinkDepthTests()
        {
            EdmModel model = new EdmModel();

            var customerType = new EdmEntityType("TestModel", "Customer");
            model.AddElement(customerType);

            var orderType = new EdmEntityType("TestModel", "Order");
            model.AddElement(orderType);

            var ordersNavProp = customerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "Orders", Target = orderType, TargetMultiplicity = EdmMultiplicity.Many });
            var singletonOrderNavProp = orderType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "SingletonOrder", Target = orderType, TargetMultiplicity = EdmMultiplicity.One });

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            model.AddElement(container);

            var customerSet = container.AddEntitySet("Customers", customerType);
            var orderSet = container.AddEntitySet("Order", orderType);
            customerSet.AddNavigationTarget(ordersNavProp, orderSet);
            orderSet.AddNavigationTarget(singletonOrderNavProp, orderSet);

            ODataResource expandedOrderInstance = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
            expandedOrderInstance.TypeName = "TestModel.Order";

            // Note: we configure the entry depth limit to be 3 in RunCombinations below
            var testCases = new[]
            {
                // 4 nested entries (the outer navigation link gets wrapped in an entry as well), should fail
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                        expandedOrderInstance,
                        new ODataNestedResourceInfo() { IsCollection = false, Name = "SingletonOrder", Url = new Uri("http://odata.org/nav") },
                        expandedOrderInstance,
                        new ODataNestedResourceInfo() { IsCollection = false, Name = "SingletonOrder", Url = new Uri("http://odata.org/nav") },
                        expandedOrderInstance,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_MaxDepthOfNestedEntriesExceeded", "3")
                },
                // 3 nested entries (the outer navigation link gets wrapped in an entry as well), should succeed.
                new NavigationLinkTestCase
                {
                    Items = new ODataItem[] {
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                        expandedOrderInstance,
                        new ODataNestedResourceInfo() { IsCollection = false, Name = "SingletonOrder", Url = new Uri("http://odata.org/nav") },
                        expandedOrderInstance,
                        null,
                        null,
                        null,
                        null,
                        null,
                    },
                    PropertyName = "Orders",
                    Xml = "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("Orders", "application/atom+xml;type=feed", "Orders", "http://odata.org/nav") + ">" +
                            "<inline xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                GetDefaultResourceSetStartXmlAsString() +
                                    GetDefaultEntryStartXmlAsString("TestModel.Order") +
                                      "<link " + TestAtomUtils.GetExpectedAtomNavigationLinkAttributesAsString("SingletonOrder", "application/atom+xml;type=entry", "SingletonOrder", "http://odata.org/nav") + ">" +
                                        "<inline xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                          GetDefaultEntryXmlAsString("TestModel.Order") +
                                        "</inline>" +
                                      "</link>" +
                                    GetDefaultEntryEndXmlAsString() +
                                "</feed>" +
                            "</inline>" +
                        "</link>",
                    JsonLight = "{" + JsonLightWriterUtils.CombineProperties(
                            "\"" + JsonLightUtils.GetPropertyAnnotationName("Orders", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/nav\"",
                            "\"Orders\":[" +
                                "{" + JsonLightWriterUtils.CombineProperties(
                                    "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryId + "\"",
                                    "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataReadLinkAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\"",
                                    "\"" + JsonLightUtils.GetPropertyAnnotationName("SingletonOrder", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/nav\"",
                                    "\"SingletonOrder\":{" + JsonLightWriterUtils.CombineProperties(
                                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryId + "\"",
                                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataReadLinkAnnotationName + "\":\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\"") +
                                    "}" +
                                "}" +
                            "]")) +
                        "}"
                },
            };

            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testDescriptors = testCases.SelectMany(testCase =>
                {
                    PayloadWriterTestDescriptor<ODataItem> withoutModel = testCase.ToEdmTestDescriptor(this.Settings, null, null, null);
                    var skipTestConfiguration = withoutModel.SkipTestConfiguration;
                    withoutModel.SkipTestConfiguration = tc => tc.Format == ODataFormat.Json || (skipTestConfiguration == null ? false : skipTestConfiguration(tc));

                    return new[]
                        {
                            withoutModel,
                            testCase.ToEdmTestDescriptor(this.Settings, model, customerSet, customerType)
                        };
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.MessageQuotas.MaxNestingDepth = 3;
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    TestWriterUtils.WriteAndVerifyODataEdmPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test that we cannot write a navigation link with incorrect multiplicity.")]
        public void NavigationLinkMultiplicityTests()
        {
            ODataResource cityEntry = ObjectModelUtils.CreateDefaultEntry("TestModel.CityType");

            // CityHall is a nav prop with multiplicity '*' of type 'TestModel.OfficeType'
            ODataNestedResourceInfo cityHallLinkIsCollectionNull = ObjectModelUtils.CreateDefaultCollectionLink("CityHall", /*isCollection*/ null);
            ODataNestedResourceInfo cityHallLinkIsCollectionTrue = ObjectModelUtils.CreateDefaultCollectionLink("CityHall", /*isCollection*/ true);
            ODataNestedResourceInfo cityHallLinkIsCollectionFalse = ObjectModelUtils.CreateDefaultCollectionLink("CityHall", /*isCollection*/ false);

            // PoliceStation is a nav prop with multiplicity '1' of type 'TestModel.OfficeType'
            ODataNestedResourceInfo policeStationLinkIsCollectionNull = ObjectModelUtils.CreateDefaultCollectionLink("PoliceStation", /*isCollection*/ null);
            ODataNestedResourceInfo policeStationLinkIsCollectionTrue = ObjectModelUtils.CreateDefaultCollectionLink("PoliceStation", /*isCollection*/ true);
            ODataNestedResourceInfo policeStationLinkIsCollectionFalse = ObjectModelUtils.CreateDefaultCollectionLink("PoliceStation", /*isCollection*/ false);

            ExpectedException expandedFeedLinkWithEntryMetadataError = ODataExpectedExceptions.ODataException("WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceMetadata", "http://odata.org/link");
            ExpectedException expandedEntryLinkWithFeedMetadataError = ODataExpectedExceptions.ODataException("WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetMetadata", "http://odata.org/link");
            ExpectedException deferredLinkInRequestError = ODataExpectedExceptions.ODataException("ODataWriterCore_DeferredLinkInRequest");

            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            var citySet = model.FindEntityContainer("DefaultContainer").FindEntitySet("Cities");
            var cityType = (IEdmEntityType)model.FindType("TestModel.CityType");

            var testCases = new NavigationLinkMultiplicityTestCase[]
            {
                new NavigationLinkMultiplicityTestCase
                {
                    // Deferred link, IsCollection == false, singleton nav prop; should not fail.
                    Items = new ODataItem[] {cityEntry, policeStationLinkIsCollectionFalse},
                    ExpectedError = tc => tc.IsRequest ? deferredLinkInRequestError : null,
                },
                new NavigationLinkMultiplicityTestCase
                {
                    // Deferred link, IsCollection == true, singleton nav prop; should fail.
                    Items = new ODataItem[] {cityEntry, policeStationLinkIsCollectionTrue},
                    ExpectedError =
                        tc => tc.IsRequest ? deferredLinkInRequestError : expandedFeedLinkWithEntryMetadataError,
                    Model = model,
                },
                new NavigationLinkMultiplicityTestCase
                {
                    // Deferred link, IsCollection == false, collection nav prop; should fail.
                    Items = new ODataItem[] {cityEntry, cityHallLinkIsCollectionFalse},
                    ExpectedError =
                        tc => tc.IsRequest ? deferredLinkInRequestError : expandedEntryLinkWithFeedMetadataError,
                    Model = model
                },
                new NavigationLinkMultiplicityTestCase
                {
                    // Deferred link, IsCollection == true, collection nav prop; should not fail.
                    Items = new ODataItem[] {cityEntry, cityHallLinkIsCollectionTrue},
                    ExpectedError = tc => tc.IsRequest ? deferredLinkInRequestError : null,
                    Model = model
                },
                new NavigationLinkMultiplicityTestCase
                {
                    // Deferred link, IsCollection == null, singleton nav prop; should not fail.
                    Items = new ODataItem[] {cityEntry, policeStationLinkIsCollectionNull},
                    ExpectedError = tc => tc.IsRequest ? deferredLinkInRequestError : null,
                    Model = model,
                },
                new NavigationLinkMultiplicityTestCase
                {
                    // Deferred link, IsCollection == null, collection nav prop; should not fail.
                    Items = new ODataItem[] {cityEntry, cityHallLinkIsCollectionNull},
                    ExpectedError = tc => tc.IsRequest ? deferredLinkInRequestError : null,
                    Model = model,
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations,
                (testCase, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    bool isJsonLight = testConfiguration.Format == ODataFormat.Json;
                    if (testCase.Model == null && isJsonLight) return;
                    using (var memoryStream = new TestStream())
                    using (var messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert, null, testCase.Model))
                    {
                        ODataWriter writer = messageWriter.CreateODataWriter(
                            /*isFeed*/ false,
                            isJsonLight ? citySet : null,
                            isJsonLight ? cityType : null);
                        TestExceptionUtils.ExpectedException(
                            this.Assert,
                            () => TestWriterUtils.WritePayload(messageWriter, writer, true, testCase.Items),
                            testCase.ExpectedError == null ? null : testCase.ExpectedError(testConfiguration),
                            this.ExceptionVerifier);
                    }
                });
        }

        internal sealed class NavigationLinkTestCase
        {
            public ODataItem[] Items { get; set; }
            public string PropertyName { get; set; }
            public string Xml { get; set; }
            public string JsonLight { get; set; }
            public ExpectedException ExpectedException { get; set; }
            public Func<WriterTestConfiguration, EdmModel, ExpectedException> EdmExpectedExceptionCallback { get; set; }
            public Func<Microsoft.Test.Taupo.OData.Common.TestConfiguration, bool> SkipTestConfiguration { get; set; }

            public PayloadWriterTestDescriptor<ODataItem> ToEdmTestDescriptor(PayloadWriterTestDescriptor.Settings settings, EdmModel model = null, EdmEntitySet entitySet = null, EdmEntityType entityType = null)
            {
                ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                entry.TypeName = entityType == null ? null : entityType.FullName();

                return new PayloadWriterTestDescriptor<ODataItem>(
                    settings,
                    new ODataItem[] { entry }.Concat(this.Items).ToArray(),
                    testConfiguration =>
                    {
                        ExpectedException expectedException = this.ExpectedException;
                        if (this.EdmExpectedExceptionCallback != null)
                        {
                            expectedException = this.EdmExpectedExceptionCallback(testConfiguration, model);
                        }

                        if (testConfiguration.Format == ODataFormat.Json)
                        {
                            return new JsonWriterTestExpectedResults(settings.ExpectedResultSettings)
                            {
                                FragmentExtractor = result => new JsonObject().AddProperties(result.RemoveAllAnnotations(true).Object().GetPropertyAnnotationsAndProperty(this.PropertyName)),
                                Json = this.JsonLight,
                                ExpectedException2 = expectedException
                            };
                        }
                        else
                        {
                            settings.Assert.Fail("Unsupported format {0}", testConfiguration.Format);
                            return null;
                        }
                    })
                {
                    SkipTestConfiguration = this.SkipTestConfiguration,
                    Model = model,
                    PayloadEdmElementContainer = entitySet,
                    PayloadEdmElementType = entityType,
                };
            }
        }

        private sealed class NavigationLinkMultiplicityTestCase
        {
            public ODataItem[] Items { get; set; }
            public IEdmModel Model { get; set; }
            public Func<WriterTestConfiguration, ExpectedException> ExpectedError { get; set; }
        }

        private static string GetDefaultEntryXmlAsString(string typeName = null)
        {
            return GetDefaultEntryStartXmlAsString(typeName) +
                GetDefaultEntryEndXmlAsString();
        }

        private static string GetDefaultEntryStartXmlAsString(string typeName = null)
        {
            return "<entry xmlns=\"" + TestAtomConstants.AtomNamespace + "\">" +
                    "<id>" + ObjectModelUtils.DefaultEntryId + "</id>" +
                    (typeName == null ? string.Empty : ("<category term='" + typeName + "' scheme='http://docs.oasis-open.org/odata/ns/scheme' />")) +
                    "<link rel=\"self\" href=\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\" />";
        }

        private static string GetDefaultEntryEndXmlAsString()
        {
            return "<title />" +
                    "<updated>" + ObjectModelUtils.DefaultEntryUpdated + "</updated>" +
                    "<author><name /></author>" +
                    "<content type=\"application/xml\" />" +
                "</entry>";
        }

        private static string GetDefaultFeedXmlAsString()
        {
            return GetDefaultResourceSetStartXmlAsString() +
                    "<author><name /></author>" +
                "</feed>";
        }

        private static string GetDefaultResourceSetStartXmlAsString()
        {
            return "<feed xmlns=\"" + TestAtomConstants.AtomNamespace + "\">" +
                    "<id>" + ObjectModelUtils.DefaultFeedId + "</id>" +
                    "<title />" +
                    "<updated>" + ObjectModelUtils.DefaultFeedUpdated + "</updated>";
        }
    }
}
