//---------------------------------------------------------------------
// <copyright file="NavigationLinkInResponseReaderJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of navigation links in responses (deferred and expanded) in JSON Light.
    /// </summary>
    [TestClass, TestCase]
    public class NavigationLinksInResponseReaderJsonLightTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the reading of deferred links on entry payloads.")]
        public void DeferredLinkTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            IEnumerable<NavigationLinkTestCase> testCases = new[]
            {
                new NavigationLinkTestCase
                {
                    DebugDescription = "Just navigation link",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink1\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("CityHall", "http://odata.org/deferredlink1")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Just association link",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink1\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("CityHall", null, "http://odata.org/deferredlink1")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Both links",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink1\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink2\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("CityHall", "http://odata.org/deferredlink1", "http://odata.org/deferredlink2")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Invalid annotation on navigation link - just that annotation",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataTypeAnnotationName) + "\":\"TestModel.OfficeType\"",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_UnexpectedDeferredLinkPropertyAnnotation", "CityHall", JsonLightConstants.ODataTypeAnnotationName)
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Invalid annotation on navigation link - with navigation link",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink1\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataTypeAnnotationName) + "\":\"TestModel.OfficeType\"",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_UnexpectedDeferredLinkPropertyAnnotation", "CityHall", JsonLightConstants.ODataTypeAnnotationName)
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Invalid annotation on navigation link - with association link",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink1\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataTypeAnnotationName) + "\":\"TestModel.OfficeType\"",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_UnexpectedDeferredLinkPropertyAnnotation", "CityHall", JsonLightConstants.ODataTypeAnnotationName)
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Navigation property with just custom annotation",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", "custom.type") + "\":\"TestModel.OfficeType\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("CityHall", null, null)
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Both links with custom annotation",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink1\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", "custom.type") + "\":\"TestModel.OfficeType\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink2\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("CityHall", "http://odata.org/deferredlink1", "http://odata.org/deferredlink2")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Both links on singleton property",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink1\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink2\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("PoliceStation", "http://odata.org/deferredlink1", "http://odata.org/deferredlink2")
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases.Select(testCase => testCase.ToTestDescriptor(this.Settings, model)),
                // Deferred links are response only
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the reading of expanded entry navigation links on entry payloads.")]
        public void ExpandedEntryNavigationLinkTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            IEnumerable<NavigationLinkTestCase> testCases = new[]
            {
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with no annotations.",
                    Json = "\"PoliceStation\":{ \"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().ExpandedNavigationProperty("PoliceStation", PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with navigation link annotation.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"PoliceStation\":{ \"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1) }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with association link annotation.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"PoliceStation\":{ \"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", 
                        new ExpandedLink() { ExpandedElement = PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1) },
                        new DeferredLink() { UriString = "http://odata.org/navlink1" }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with invalid annotation.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataTypeAnnotationName) + "\":\"TestModel.OfficeType\"," +
                        "\"PoliceStation\":{ \"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_UnexpectedExpandedSingletonNavigationLinkPropertyAnnotation", "PoliceStation", JsonLightConstants.ODataTypeAnnotationName)
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with navigation and association link annotations.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink2\"," +
                        "\"PoliceStation\":{ \"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", 
                        new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1) },
                        new DeferredLink() { UriString = "http://odata.org/navlink2" }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with only custom annotation.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation") + "\":\"value\"," +
                        "\"PoliceStation\":{ \"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().ExpandedNavigationProperty("PoliceStation", PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with navigation link and custom annotations.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation") + "\":\"value\"," +
                        "\"PoliceStation\":{ \"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1) }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with navigation, association link and custom annotations.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation") + "\":null," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink2\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation2") + "\":42," +
                        "\"PoliceStation\":{ \"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", 
                        new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1) },
                        new DeferredLink() { UriString = "http://odata.org/navlink2" }))
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases.Select(testCase => testCase.ToTestDescriptor(this.Settings, model)),
                // Expanded entry links are response only
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the reading of expanded null entry navigation links on entry payloads.")]
        public void ExpandedNullEntryNavigationLinkTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            IEnumerable<NavigationLinkTestCase> testCases = new[]
            {
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded null entry with no annotations.",
                    Json = "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = null, ExpandedElement = PayloadBuilder.NullEntity() }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded null entry with navigation link annotation.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.NullEntity() }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded null entry with association link annotation.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", 
                        new ExpandedLink() { ExpandedElement = PayloadBuilder.NullEntity() },
                        new DeferredLink() { UriString = "http://odata.org/navlink1" }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded null entry with invalid annotation.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataTypeAnnotationName) + "\":\"TestModel.OfficeType\"," +
                        "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_UnexpectedExpandedSingletonNavigationLinkPropertyAnnotation", "PoliceStation", JsonLightConstants.ODataTypeAnnotationName)
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded null entry with navigation and association link annotations.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink2\"," +
                        "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", 
                        new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.NullEntity() },
                        new DeferredLink() { UriString = "http://odata.org/navlink2" }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded null entry with only custom annotation.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation") + "\":\"value\"," +
                        "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity().ExpandedNavigationProperty("PoliceStation", PayloadBuilder.NullEntity())
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded null entry with navigation link and custom annotations.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation") + "\":\"value\"," +
                        "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.NullEntity() }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded null entry with navigation, association link and custom annotations.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation") + "\":null," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink2\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation2") + "\":42," +
                        "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", 
                        new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.NullEntity() },
                        new DeferredLink() { UriString = "http://odata.org/navlink2" }))
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases.Select(testCase => testCase.ToTestDescriptor(this.Settings, model)),
                // Expanded entry links are response only
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the reading of expanded feed navigation links on entry payloads.")]
        public void ExpandedFeedNavigationLinkTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            IEnumerable<NavigationLinkTestCase> testCases = new[]
            {
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with no annotations.",
                    Json = "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().ExpandedNavigationProperty("CityHall", PayloadBuilder.EntitySet())
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with navigation link annotation.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.EntitySet() }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with association link annotation.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", 
                        new ExpandedLink() { ExpandedElement = PayloadBuilder.EntitySet() },
                        new DeferredLink() { UriString = "http://odata.org/navlink1" }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with next link annotation before the property.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNextLinkAnnotationName) + "\":\"http://odata.org/nextlink\"," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", 
                        new ExpandedLink() { UriString = null, ExpandedElement = PayloadBuilder.EntitySet().NextLink("http://odata.org/nextlink") }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with next link annotation after the property.",
                    Json = 
                        "\"CityHall\": []," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNextLinkAnnotationName) + "\":\"http://odata.org/nextlink\"",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", 
                        new ExpandedLink() { UriString = null, ExpandedElement = PayloadBuilder.EntitySet().NextLink("http://odata.org/nextlink") }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with entry and with next link annotation after the property.",
                    Json = 
                        "\"CityHall\": [ { \"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }]," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNextLinkAnnotationName) + "\":\"http://odata.org/nextlink\"",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", 
                        new ExpandedLink() { UriString = null, ExpandedElement = PayloadBuilder.EntitySet().Append(PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1)).NextLink("http://odata.org/nextlink") }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with navigation link annotation after the property - invalid.",
                    Json = 
                        "\"CityHall\": []," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink\"",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_UnexpectedPropertyAnnotationAfterExpandedFeed", JsonLightConstants.ODataNavigationLinkUrlAnnotationName, "CityHall")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with duplicate next link annotation before and after the property - invalid.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNextLinkAnnotationName) + "\":\"http://odata.org/nextlink\"," +
                        "\"CityHall\": []," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNextLinkAnnotationName) + "\":\"http://odata.org/nextlink2\"",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_DuplicateExpandedFeedAnnotation", JsonLightConstants.ODataNextLinkAnnotationName, "CityHall")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with invalid annotation.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataTypeAnnotationName) + "\":\"TestModel.OfficeType\"," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_UnexpectedExpandedCollectionNavigationLinkPropertyAnnotation", "CityHall", JsonLightConstants.ODataTypeAnnotationName)
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with navigation, association and next link annotations.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink2\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNextLinkAnnotationName) + "\":\"http://odata.org/nextlink\"," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", 
                        new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.EntitySet().NextLink("http://odata.org/nextlink") },
                        new DeferredLink() { UriString = "http://odata.org/navlink2" }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with only custom annotation.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", "custom.annotation") + "\":\"value\"," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().ExpandedNavigationProperty("CityHall", PayloadBuilder.EntitySet())
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with navigation link and custom annotations.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", "custom.annotation") + "\":\"value\"," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.EntitySet() }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with navigation, association link and custom annotations.",
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNextLinkAnnotationName) + "\":\"http://odata.org/nextlink\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", "custom.annotation") + "\":null," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink2\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", "custom.annotation2") + "\":42," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", 
                        new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.EntitySet().NextLink("http://odata.org/nextlink") },
                        new DeferredLink() { UriString = "http://odata.org/navlink2" }))
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases.Select(testCase => testCase.ToTestDescriptor(this.Settings, model)),
                // Expanded feed links are response only
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the reading of invalid annotation values on navigation links on entry payloads.")]
        public void RelativeNavigationAndAssociationLinkTests()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            IEnumerable<NavigationLinkTestCase> testCases = new[]
            {
                new NavigationLinkTestCase
                {
                    DebugDescription = "Relative navigation link - deferred link",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"yyy\"",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new DeferredLink() { UriString = "http://odata.org/test/yyy"})),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Relative navigation link - expanded entry link",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"yyy\"," +
                        "\"PoliceStation\":{ \"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = "http://odata.org/test/yyy", ExpandedElement = PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1) })),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Relative navigation link - expanded null entry link",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"yyy\"," + 
                        "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = "http://odata.org/test/yyy", ExpandedElement = PayloadBuilder.NullEntity() })),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Relative navigation link - expanded feed link",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"yyy\"," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new ExpandedLink() { UriString = "http://odata.org/test/yyy", ExpandedElement = PayloadBuilder.EntitySet() })),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Relative association link - deferred link",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"yyy\"",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new DeferredLink() { UriString = "http://odata.org/test/Cities(1)/CityHall"})),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Relative association link - expanded entry link",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"yyy\"," +
                        "\"PoliceStation\":{ \"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = "http://odata.org/test/Cities(1)/PoliceStation", ExpandedElement = PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1) })),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Relative association link - expanded null entry link",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("PoliceStation", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"yyy\"," + 
                        "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = "http://odata.org/test/Cities(1)/PoliceStation", ExpandedElement = PayloadBuilder.NullEntity() })),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Relative association link - expanded feed link",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"yyy\"," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new ExpandedLink() { UriString = "http://odata.org/test/Cities(1)/CityHall", ExpandedElement = PayloadBuilder.EntitySet() })),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Relative next link - deferred link",
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("CityHall", JsonLightConstants.ODataNextLinkAnnotationName) + "\":\"yyy\"",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_UnexpectedDeferredLinkPropertyAnnotation", "CityHall", "odata.nextLink"),
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases.Select(testCase => testCase.ToTestDescriptor(this.Settings, model)),
                // Expanded feed links are response only
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the reading of invalid annotation values on navigation links on entry payloads.")]
        public void InvalidNavigationLinkAnnotationValueTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            IEnumerable<InvalidNavigationLinkAnnotationValueTestCase> testCases = new[]
            {
                new InvalidNavigationLinkAnnotationValueTestCase
                {
                    DebugDescription = "Invalid navigation link - null",
                    Json = propertyName => "\"" + JsonLightUtils.GetPropertyAnnotationName(propertyName, JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":null",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightReaderUtils_AnnotationWithNullValue", JsonLightConstants.ODataNavigationLinkUrlAnnotationName)
                },
                new InvalidNavigationLinkAnnotationValueTestCase
                {
                    DebugDescription = "Invalid navigation link - number",
                    Json = propertyName => "\"" + JsonLightUtils.GetPropertyAnnotationName(propertyName, JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":42",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_CannotReadPropertyValueAsString", "42", JsonLightConstants.ODataNavigationLinkUrlAnnotationName)
                },
                new InvalidNavigationLinkAnnotationValueTestCase
                {
                    DebugDescription = "Invalid navigation link - array",
                    Json = propertyName => "\"" + JsonLightUtils.GetPropertyAnnotationName(propertyName, JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":[]",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartArray")
                },
                new InvalidNavigationLinkAnnotationValueTestCase
                {
                    DebugDescription = "Invalid association link - null",
                    Json = propertyName => "\"" + JsonLightUtils.GetPropertyAnnotationName(propertyName, JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":null",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightReaderUtils_AnnotationWithNullValue", JsonLightConstants.ODataAssociationLinkUrlAnnotationName)
                },
                new InvalidNavigationLinkAnnotationValueTestCase
                {
                    DebugDescription = "Invalid association link - boolean",
                    Json = propertyName => "\"" + JsonLightUtils.GetPropertyAnnotationName(propertyName, JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":true",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_CannotReadPropertyValueAsString", "True", JsonLightConstants.ODataAssociationLinkUrlAnnotationName)
                },
                new InvalidNavigationLinkAnnotationValueTestCase
                {
                    DebugDescription = "Invalid association link - array",
                    Json = propertyName => "\"" + JsonLightUtils.GetPropertyAnnotationName(propertyName, JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":[]",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartArray")
                },
                new InvalidNavigationLinkAnnotationValueTestCase
                {
                    DebugDescription = "Invalid next link - null",
                    Json = propertyName => "\"" + JsonLightUtils.GetPropertyAnnotationName(propertyName, JsonLightConstants.ODataNextLinkAnnotationName) + "\":null",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightReaderUtils_AnnotationWithNullValue", JsonLightConstants.ODataNextLinkAnnotationName)
                },
                new InvalidNavigationLinkAnnotationValueTestCase
                {
                    DebugDescription = "Invalid next link - number",
                    Json = propertyName => "\"" + JsonLightUtils.GetPropertyAnnotationName(propertyName, JsonLightConstants.ODataNextLinkAnnotationName) + "\":42",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_CannotReadPropertyValueAsString", "42", JsonLightConstants.ODataNextLinkAnnotationName)
                },
                new InvalidNavigationLinkAnnotationValueTestCase
                {
                    DebugDescription = "Invalid next link - array",
                    Json = propertyName => "\"" + JsonLightUtils.GetPropertyAnnotationName(propertyName, JsonLightConstants.ODataNextLinkAnnotationName) + "\":[]",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartArray")
                },
            };

            this.RunInvalidNavigationLinkAnnotationValueTests(model, testCases);
        }

        private sealed class InvalidNavigationLinkAnnotationValueTestCase
        {
            public string DebugDescription { get; set; }
            public Func<string, string> Json { get; set; }
            public ExpectedException ExpectedException { get; set; }
            public bool ExpandedFeedOnly { get; set; }
        }

        private void RunInvalidNavigationLinkAnnotationValueTests(IEdmModel model, IEnumerable<InvalidNavigationLinkAnnotationValueTestCase> testCases)
        {
            IEnumerable<NavigationLinkTestCase> linkTestCases = testCases.SelectMany(testCase =>
            {
                NavigationLinkTestCase deferredLink = new NavigationLinkTestCase
                {
                    DebugDescription = testCase.DebugDescription + " - deferred link",
                    Json = testCase.Json("CityHall"),
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new DeferredLink() { UriString = "http://odata.org/test/yyy"})),
                    ExpectedException = testCase.ExpectedException
                };
                NavigationLinkTestCase expandedEntryLink = new NavigationLinkTestCase
                {
                    DebugDescription = testCase.DebugDescription + " - expanded entry link",
                    Json =
                        testCase.Json("PoliceStation") + "," +
                        "\"PoliceStation\":{ \"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = "http://odata.org/test/yyy", ExpandedElement = PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1) })),
                    ExpectedException = testCase.ExpectedException
                };
                NavigationLinkTestCase expandedNullEntryLink = new NavigationLinkTestCase
                {
                    DebugDescription = testCase.DebugDescription + " - expanded null entry link",
                    Json =
                        testCase.Json("PoliceStation") + "," +
                        "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = "http://odata.org/test/yyy", ExpandedElement = PayloadBuilder.NullEntity() })),
                    ExpectedException = testCase.ExpectedException
                };
                NavigationLinkTestCase expandedFeedLink = new NavigationLinkTestCase
                {
                    DebugDescription = testCase.DebugDescription + " - expanded feed link",
                    Json =
                        testCase.Json("CityHall") + "," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new ExpandedLink() { UriString = "http://odata.org/test/yyy", ExpandedElement = PayloadBuilder.EntitySet() })),
                    ExpectedException = testCase.ExpectedException
                };

                if (testCase.ExpandedFeedOnly)
                {
                    return new[] { expandedFeedLink };
                }
                else
                {
                    return new[] { deferredLink, expandedEntryLink, expandedNullEntryLink, expandedFeedLink };
                }
            });

            this.CombinatorialEngineProvider.RunCombinations(
                linkTestCases.Select(testCase => testCase.ToTestDescriptor(this.Settings, model)),
                // Expanded or deferred links are response only
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    var testConfigClone = new ReaderTestConfiguration(testConfiguration);
                    testConfigClone.MessageReaderSettings.BaseUri = null;

                    testDescriptor.RunTest(testConfigClone);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the reading of expanded links with invalid values on entry payloads.")]
        public void ExpandedNavigationLinkInvalidValueTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            IEnumerable<NavigationLinkTestCase> testCases = new[]
            {
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with array value",
                    Json = "\"PoliceStation\": []",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_CannotReadSingletonNavigationPropertyValue", "StartArray", "PoliceStation")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with number value",
                    Json = "\"PoliceStation\": 42",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_CannotReadNavigationPropertyValue", "PoliceStation")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with object value",
                    Json = "\"CityHall\": {}",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_CannotReadCollectionNavigationPropertyValue", "StartObject", "CityHall")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with null value",
                    Json = "\"CityHall\": null",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_CannotReadCollectionNavigationPropertyValue", "PrimitiveValue", "CityHall")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with boolean value",
                    Json = "\"CityHall\": true",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_CannotReadNavigationPropertyValue", "CityHall")
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases.Select(testCase => testCase.ToTestDescriptor(this.Settings, model)),
                // Expanded links are response only
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        private sealed class NavigationLinkTestCase
        {
            public string DebugDescription { get; set; }
            public string Json { get; set; }
            public EntityInstance ExpectedEntity { get; set; }
            public ExpectedException ExpectedException { get; set; }

            public PayloadReaderTestDescriptor ToTestDescriptor(PayloadReaderTestDescriptor.Settings settings, IEdmModel model)
            {
                EntityInstance entity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1)
                    .ExpectedEntityType(model.FindType("TestModel.CityType"), model.EntityContainer.FindEntitySet("Cities"))
                    .JsonRepresentation(
                        "{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.CityType\"," +
                            "\"Id\":1," +
                            this.Json +
                        "}");
                foreach (PropertyInstance property in this.ExpectedEntity.Properties)
                {
                    entity.Add(property.DeepCopy());
                }

                return new PayloadReaderTestDescriptor(settings)
                {
                    DebugDescription = this.DebugDescription,
                    PayloadElement = entity,
                    PayloadEdmModel = model,
                    ExpectedException = this.ExpectedException
                };
            }
        }
    }
}
