//---------------------------------------------------------------------
// <copyright file="JsonLightNavigationLinkWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.JsonLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Writer;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;

    /// <summary>
    /// Tests for writing navigation link payloads in JSON Lite format.
    /// </summary>
    [TestClass, TestCase]
    public class JsonLightNavigationLinkWriterTests : ODataWriterTestCase
    {
        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestExpectedResults.Settings ExpectedResultSettings { get; set; }

        [TestMethod, Variation(Description = "Error case for writing entity reference links after a feed in JSON Lite navigation links in requests.")]
        public void WriteEntityReferenceLinkAfterFeedJsonLightErrorTest()
        {
            EdmModel model = new EdmModel();

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            model.AddElement(container);

            var customerType = new EdmEntityType("TestModel", "Customer");
            var orderType = new EdmEntityType("TestModel", "Order");
            var orderNavProp = customerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "Orders", Target = orderType, TargetMultiplicity = EdmMultiplicity.Many });
            var bestFriendNavProp = customerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "BestFriend", Target = customerType, TargetMultiplicity = EdmMultiplicity.One });
            var customerSet = container.AddEntitySet("Customers", customerType);
            var orderSet = container.AddEntitySet("Order", orderType);
            customerSet.AddNavigationTarget(orderNavProp, orderSet);
            customerSet.AddNavigationTarget(bestFriendNavProp, customerSet);
            model.AddElement(customerType);
            model.AddElement(orderType);

            ODataResource expandedOrderInstance = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
            expandedOrderInstance.TypeName = "TestModel.Order";

            var testCases = new WriterNavigationLinkTests.NavigationLinkTestCase[]
            {
                // Entity reference link after empty feed in collection navigation link
                new WriterNavigationLinkTests.NavigationLinkTestCase
                {
                    Items = new ODataItem[] { 
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                        null,
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/singleton") },
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightWriter_EntityReferenceLinkAfterResourceSetInRequest")
                },
                // Entity reference link after non-empty feed in collection navigation link
                new WriterNavigationLinkTests.NavigationLinkTestCase
                {
                    Items = new ODataItem[] { 
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                            expandedOrderInstance,
                            null,
                        null,
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/singleton") },
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightWriter_EntityReferenceLinkAfterResourceSetInRequest")
                },
                // Entity reference link before and after empty feed in collection navigation link
                new WriterNavigationLinkTests.NavigationLinkTestCase
                {
                    Items = new ODataItem[] { 
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/singleton") },
                        ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                        null,
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/singleton") },
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightWriter_EntityReferenceLinkAfterResourceSetInRequest")
                },
                // Entity reference link after two empty feeds in collection navigation link
                new WriterNavigationLinkTests.NavigationLinkTestCase
                {
                    Items = new ODataItem[] { 
                        new ODataNestedResourceInfo() { IsCollection = true, Name = "Orders", Url = new Uri("http://odata.org/nav") },
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/singleton") },
                        ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                        null,
                        ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),
                        null,
                        new ODataEntityReferenceLink() { Url = new Uri("http://odata.org/singleton") },
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightWriter_EntityReferenceLinkAfterResourceSetInRequest")
                },
            };

            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testDescriptors =
                testCases.Select(testCase => testCase.ToEdmTestDescriptor(this.Settings, model, customerSet, customerType));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    TestWriterUtils.WriteAndVerifyODataEdmPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }
    }
}
