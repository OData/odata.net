//---------------------------------------------------------------------
// <copyright file="NavigationLinkInResponseReaderJsonTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Json
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
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of navigation links in responses (deferred and expanded) in JSON Light.
    /// </summary>
    [TestClass, TestCase]
    public class NavigationLinksInResponseReaderJsonTests : ODataReaderTestCase
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
                    Json = "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink1\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("CityHall", "http://odata.org/deferredlink1")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Just association link",
                    Json = "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink1\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("CityHall", null, "http://odata.org/deferredlink1")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Both links",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink1\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink2\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("CityHall", "http://odata.org/deferredlink1", "http://odata.org/deferredlink2")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Type annotation on navigation link - just that annotation",
                    Json = "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataTypeAnnotationName) + "\":\"TestModel.OfficeType\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty(new NavigationPropertyInstance {Name="CityHall" }),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Type annotation on navigation link - with navigation link",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink1\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataTypeAnnotationName) + "\":\"TestModel.OfficeType\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("CityHall","http://odata.org/deferredlink1"),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Type annotation on navigation link - with association link",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink1\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataTypeAnnotationName) + "\":\"TestModel.OfficeType\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("CityHall", null, "http://odata.org/deferredlink1")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Navigation property with just custom annotation",
                    Json = "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", "custom.type") + "\":\"TestModel.OfficeType\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("CityHall", null, null)
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Both links with custom annotation",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink1\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", "custom.type") + "\":\"TestModel.OfficeType\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink2\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("CityHall", "http://odata.org/deferredlink1", "http://odata.org/deferredlink2")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Both links on singleton property",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink1\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/deferredlink2\"",
                    ExpectedEntity = PayloadBuilder.Entity().NavigationProperty("PoliceStation", "http://odata.org/deferredlink1", "http://odata.org/deferredlink2")
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases.Select(testCase => testCase.ToTestDescriptor(this.Settings, model)),
                // Deferred links are response only
                this.ReaderTestConfigurationProvider.JsonFormatConfigurations.Where(tc => !tc.IsRequest),
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
                    Json = "\"PoliceStation\":{ \"" + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().ExpandedNavigationProperty("PoliceStation", PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with navigation link annotation.",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"PoliceStation\":{ \"" + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1) }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with association link annotation.",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"PoliceStation\":{ \"" + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation",
                        new ExpandedLink() { ExpandedElement = PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1) },
                        new DeferredLink() { UriString = "http://odata.org/navlink1" }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with navigation and association link annotations.",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink2\"," +
                        "\"PoliceStation\":{ \"" + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation",
                        new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1) },
                        new DeferredLink() { UriString = "http://odata.org/navlink2" }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with only custom annotation.",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation") + "\":\"value\"," +
                        "\"PoliceStation\":{ \"" + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().ExpandedNavigationProperty("PoliceStation", PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with navigation link and custom annotations.",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation") + "\":\"value\"," +
                        "\"PoliceStation\":{ \"" + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1) }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with navigation, association link and custom annotations.",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation") + "\":null," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink2\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation2") + "\":42," +
                        "\"PoliceStation\":{ \"" + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation",
                        new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1) },
                        new DeferredLink() { UriString = "http://odata.org/navlink2" }))
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases.Select(testCase => testCase.ToTestDescriptor(this.Settings, model)),
                // Expanded entry links are response only
                this.ReaderTestConfigurationProvider.JsonFormatConfigurations.Where(tc => !tc.IsRequest),
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
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.NullEntity() }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded null entry with association link annotation.",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation",
                        new ExpandedLink() { ExpandedElement = PayloadBuilder.NullEntity() },
                        new DeferredLink() { UriString = "http://odata.org/navlink1" }))
                },
                //new NavigationLinkTestCase
                //{
                //    DebugDescription = "Expanded null entry with invalid annotation.",
                //    Json =
                //        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataTypeAnnotationName) + "\":\"TestModel.OfficeType\"," +
                //        "\"PoliceStation\": null",
                //    ExpectedEntity = PayloadBuilder.Entity(),
                //    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonResourceDeserializer_UnexpectedExpandedSingletonNavigationLinkPropertyAnnotation", "PoliceStation", JsonConstants.ODataTypeAnnotationName)
                //},
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded null entry with navigation and association link annotations.",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink2\"," +
                        "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation",
                        new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.NullEntity() },
                        new DeferredLink() { UriString = "http://odata.org/navlink2" }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded null entry with only custom annotation.",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation") + "\":\"value\"," +
                        "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity().ExpandedNavigationProperty("PoliceStation", PayloadBuilder.NullEntity())
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded null entry with navigation link and custom annotations.",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation") + "\":\"value\"," +
                        "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.NullEntity() }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded null entry with navigation, association link and custom annotations.",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation") + "\":null," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink2\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", "custom.annotation2") + "\":42," +
                        "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation",
                        new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.NullEntity() },
                        new DeferredLink() { UriString = "http://odata.org/navlink2" }))
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases.Select(testCase => testCase.ToTestDescriptor(this.Settings, model)),
                // Expanded entry links are response only
                this.ReaderTestConfigurationProvider.JsonFormatConfigurations.Where(tc => !tc.IsRequest),
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
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.EntitySet() }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with association link annotation.",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall",
                        new ExpandedLink() { ExpandedElement = PayloadBuilder.EntitySet() },
                        new DeferredLink() { UriString = "http://odata.org/navlink1" }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with next link annotation before the property.",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNextLinkAnnotationName) + "\":\"http://odata.org/nextlink\"," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall",
                        new ExpandedLink() { UriString = null, ExpandedElement = PayloadBuilder.EntitySet().NextLink("http://odata.org/nextlink") }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with next link annotation after the property.",
                    Json =
                        "\"CityHall\": []," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNextLinkAnnotationName) + "\":\"http://odata.org/nextlink\"",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall",
                        new ExpandedLink() { UriString = null, ExpandedElement = PayloadBuilder.EntitySet().NextLink("http://odata.org/nextlink") }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with entry and with next link annotation after the property.",
                    Json =
                        "\"CityHall\": [ { \"" + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }]," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNextLinkAnnotationName) + "\":\"http://odata.org/nextlink\"",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall",
                        new ExpandedLink() { UriString = null, ExpandedElement = PayloadBuilder.EntitySet().Append(PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1)).NextLink("http://odata.org/nextlink") }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with navigation link annotation after the property - invalid.",
                    Json =
                        "\"CityHall\": []," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink\"",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonResourceDeserializer_UnexpectedPropertyAnnotationAfterExpandedResourceSet", JsonConstants.ODataNavigationLinkUrlAnnotationName, "CityHall")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with duplicate next link annotation before and after the property - invalid.",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNextLinkAnnotationName) + "\":\"http://odata.org/nextlink\"," +
                        "\"CityHall\": []," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNextLinkAnnotationName) + "\":\"http://odata.org/nextlink2\"",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonResourceDeserializer_DuplicateNestedResourceSetAnnotation", JsonConstants.ODataNextLinkAnnotationName, "CityHall")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with valid annotation.",
                    Json = 
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataTypeAnnotationName) + "\":\"#Collection(TestModel.OfficeType)\"," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().ExpandedNavigationProperty("CityHall", PayloadBuilder.EntitySet())
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with navigation, association and next link annotations.",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink2\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNextLinkAnnotationName) + "\":\"http://odata.org/nextlink\"," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall",
                        new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.EntitySet().NextLink("http://odata.org/nextlink") },
                        new DeferredLink() { UriString = "http://odata.org/navlink2" }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with only custom annotation.",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", "custom.annotation") + "\":\"value\"," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().ExpandedNavigationProperty("CityHall", PayloadBuilder.EntitySet())
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with navigation link and custom annotations.",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", "custom.annotation") + "\":\"value\"," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.EntitySet() }))
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with navigation, association link and custom annotations.",
                    Json =
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNextLinkAnnotationName) + "\":\"http://odata.org/nextlink\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", "custom.annotation") + "\":null," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink1\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink2\"," +
                        "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", "custom.annotation2") + "\":42," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall",
                        new ExpandedLink() { UriString = "http://odata.org/navlink1", ExpandedElement = PayloadBuilder.EntitySet().NextLink("http://odata.org/nextlink") },
                        new DeferredLink() { UriString = "http://odata.org/navlink2" }))
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases.Select(testCase => testCase.ToTestDescriptor(this.Settings, model)),
                // Expanded feed links are response only
                this.ReaderTestConfigurationProvider.JsonFormatConfigurations.Where(tc => !tc.IsRequest),
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
                    Json = "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"yyy\"",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new DeferredLink() { UriString = "http://odata.org/test/yyy"})),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Relative navigation link - expanded entry link",
                    Json = "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"yyy\"," +
                        "\"PoliceStation\":{ \"" + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = "http://odata.org/test/yyy", ExpandedElement = PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1) })),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Relative navigation link - expanded null entry link",
                    Json = "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"yyy\"," +
                        "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = "http://odata.org/test/yyy", ExpandedElement = PayloadBuilder.NullEntity() })),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Relative navigation link - expanded feed link",
                    Json = "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"yyy\"," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new ExpandedLink() { UriString = "http://odata.org/test/yyy", ExpandedElement = PayloadBuilder.EntitySet() })),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Relative association link - deferred link",
                    Json = "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"yyy\"",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new DeferredLink() { UriString = "http://odata.org/test/Cities(1)/CityHall"})),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Relative association link - expanded entry link",
                    Json = "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"yyy\"," +
                        "\"PoliceStation\":{ \"" + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = "http://odata.org/test/Cities(1)/PoliceStation", ExpandedElement = PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1) })),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Relative association link - expanded null entry link",
                    Json = "\"" + JsonUtils.GetPropertyAnnotationName("PoliceStation", JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"yyy\"," +
                        "\"PoliceStation\": null",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("PoliceStation", new ExpandedLink() { UriString = "http://odata.org/test/Cities(1)/PoliceStation", ExpandedElement = PayloadBuilder.NullEntity() })),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Relative association link - expanded feed link",
                    Json = "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"yyy\"," +
                        "\"CityHall\": []",
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new ExpandedLink() { UriString = "http://odata.org/test/Cities(1)/CityHall", ExpandedElement = PayloadBuilder.EntitySet() })),
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Relative next link - deferred link",
                    Json = "\"" + JsonUtils.GetPropertyAnnotationName("CityHall", JsonConstants.ODataNextLinkAnnotationName) + "\":\"yyy\"",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonResourceDeserializer_UnexpectedDeferredLinkPropertyAnnotation", "CityHall", "odata.nextLink"),
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases.Select(testCase => testCase.ToTestDescriptor(this.Settings, model)),
                // Expanded feed links are response only
                this.ReaderTestConfigurationProvider.JsonFormatConfigurations.Where(tc => !tc.IsRequest),
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
                    Json = propertyName => "\"" + JsonUtils.GetPropertyAnnotationName(propertyName, JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":null",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonReaderUtils_AnnotationWithNullValue", JsonConstants.ODataNavigationLinkUrlAnnotationName)
                },
                new InvalidNavigationLinkAnnotationValueTestCase
                {
                    DebugDescription = "Invalid navigation link - number",
                    Json = propertyName => "\"" + JsonUtils.GetPropertyAnnotationName(propertyName, JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":42",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_CannotReadPropertyValueAsString", "42", JsonConstants.ODataNavigationLinkUrlAnnotationName)
                },
                new InvalidNavigationLinkAnnotationValueTestCase
                {
                    DebugDescription = "Invalid navigation link - array",
                    Json = propertyName => "\"" + JsonUtils.GetPropertyAnnotationName(propertyName, JsonConstants.ODataNavigationLinkUrlAnnotationName) + "\":[]",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartArray")
                },
                new InvalidNavigationLinkAnnotationValueTestCase
                {
                    DebugDescription = "Invalid association link - null",
                    Json = propertyName => "\"" + JsonUtils.GetPropertyAnnotationName(propertyName, JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":null",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonReaderUtils_AnnotationWithNullValue", JsonConstants.ODataAssociationLinkUrlAnnotationName)
                },
                new InvalidNavigationLinkAnnotationValueTestCase
                {
                    DebugDescription = "Invalid association link - boolean",
                    Json = propertyName => "\"" + JsonUtils.GetPropertyAnnotationName(propertyName, JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":true",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_CannotReadPropertyValueAsString", "True", JsonConstants.ODataAssociationLinkUrlAnnotationName)
                },
                new InvalidNavigationLinkAnnotationValueTestCase
                {
                    DebugDescription = "Invalid association link - array",
                    Json = propertyName => "\"" + JsonUtils.GetPropertyAnnotationName(propertyName, JsonConstants.ODataAssociationLinkUrlAnnotationName) + "\":[]",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartArray")
                },
                new InvalidNavigationLinkAnnotationValueTestCase
                {
                    DebugDescription = "Invalid next link - null",
                    Json = propertyName => "\"" + JsonUtils.GetPropertyAnnotationName(propertyName, JsonConstants.ODataNextLinkAnnotationName) + "\":null",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonReaderUtils_AnnotationWithNullValue", JsonConstants.ODataNextLinkAnnotationName)
                },
                new InvalidNavigationLinkAnnotationValueTestCase
                {
                    DebugDescription = "Invalid next link - number",
                    Json = propertyName => "\"" + JsonUtils.GetPropertyAnnotationName(propertyName, JsonConstants.ODataNextLinkAnnotationName) + "\":42",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_CannotReadPropertyValueAsString", "42", JsonConstants.ODataNextLinkAnnotationName)
                },
                new InvalidNavigationLinkAnnotationValueTestCase
                {
                    DebugDescription = "Invalid next link - array",
                    Json = propertyName => "\"" + JsonUtils.GetPropertyAnnotationName(propertyName, JsonConstants.ODataNextLinkAnnotationName) + "\":[]",
                    ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartArray")
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
                    ExpectedEntity = PayloadBuilder.Entity().Property(new NavigationPropertyInstance("CityHall", new DeferredLink() { UriString = "http://odata.org/test/yyy" })),
                    ExpectedException = testCase.ExpectedException
                };
                NavigationLinkTestCase expandedEntryLink = new NavigationLinkTestCase
                {
                    DebugDescription = testCase.DebugDescription + " - expanded entry link",
                    Json =
                        testCase.Json("PoliceStation") + "," +
                        "\"PoliceStation\":{ \"" + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + "\":\"TestModel.OfficeType\", \"Id\":1 }",
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
                this.ReaderTestConfigurationProvider.JsonFormatConfigurations.Where(tc => !tc.IsRequest),
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
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonResourceDeserializer_CannotReadSingletonNestedResource", "StartArray", "PoliceStation")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded entry with number value",
                    Json = "\"PoliceStation\": 42",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonResourceDeserializer_CannotReadNestedResource", "PoliceStation")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with object value",
                    Json = "\"CityHall\": {}",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonResourceDeserializer_CannotReadCollectionNestedResource", "StartObject", "CityHall")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with null value",
                    Json = "\"CityHall\": null",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonResourceDeserializer_CannotReadCollectionNestedResource", "PrimitiveValue", "CityHall")
                },
                new NavigationLinkTestCase
                {
                    DebugDescription = "Expanded feed with boolean value",
                    Json = "\"CityHall\": true",
                    ExpectedEntity = PayloadBuilder.Entity(),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonResourceDeserializer_CannotReadNestedResource", "CityHall")
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases.Select(testCase => testCase.ToTestDescriptor(this.Settings, model)),
                // Expanded links are response only
                this.ReaderTestConfigurationProvider.JsonFormatConfigurations.Where(tc => !tc.IsRequest),
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
                            "\"" + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                            "\"" + JsonConstants.ODataPropertyAnnotationSeparator + JsonConstants.ODataTypeAnnotationName + "\":\"TestModel.CityType\"," +
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
