//---------------------------------------------------------------------
// <copyright file="WriterAssociationLinkTests.cs" company="Microsoft">
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
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.Json.TextAnnotations;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Writer.Tests;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
    /// <summary>
    /// Tests for writing association links with the OData writer.
    /// </summary>
    // [TestClass, TestCase]
    public class WriterAssociationLinkTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [Ignore] // Remove Atom
        // Navigation link cannot be used in request
        [TestMethod, Variation(Description = "Validates the payloads for various association links.")]
        public void AssociationLinkTest()
        {
            string associationLinkName1 = "AssociationLinkOne";
            string linkUrl1 = "http://odata.org/associationlink";
            Uri linkUrlUri1 = new Uri(linkUrl1);
            string associationLinkName2 = "AssociationLinkTwo";
            string linkUrl2 = "http://odata.org/associationlink2";
            Uri linkUrlUri2 = new Uri(linkUrl2);

            EdmModel model = new EdmModel();

            var edmEntityTypeOrderType = new EdmEntityType("TestModel", "OrderType");
            model.AddElement(edmEntityTypeOrderType);

            var edmEntityTypeCustomerType = new EdmEntityType("TestModel", "CustomerType");
            var edmNavigationPropertyAssociationLinkOne = edmEntityTypeCustomerType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo { Name = associationLinkName1, Target = edmEntityTypeOrderType, TargetMultiplicity = EdmMultiplicity.One });
            var edmNavigationPropertyAssociationLinkTwo = edmEntityTypeCustomerType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo { Name = associationLinkName2, Target = edmEntityTypeOrderType, TargetMultiplicity = EdmMultiplicity.Many });
            model.AddElement(edmEntityTypeCustomerType);

            var container = new EdmEntityContainer("TestModel", "Default");
            model.AddElement(container);

            var customerSet = container.AddEntitySet("Customers", edmEntityTypeCustomerType);
            var orderSet = container.AddEntitySet("Orders", edmEntityTypeOrderType);
            customerSet.AddNavigationTarget(edmNavigationPropertyAssociationLinkOne, orderSet);
            customerSet.AddNavigationTarget(edmNavigationPropertyAssociationLinkTwo, orderSet);

            var testCases = new[]
            {
                new {
                    NavigationLink = ObjectModelUtils.CreateDefaultNavigationLink(associationLinkName1, linkUrlUri1),
                    Atom = BuildXmlAssociationLink(associationLinkName1, "application/xml", linkUrl1),
                    JsonLight = (string)null,
                },
                new {
                    NavigationLink = ObjectModelUtils.CreateDefaultNavigationLink(associationLinkName2, linkUrlUri2),
                    Atom = BuildXmlAssociationLink(associationLinkName2, "application/xml", linkUrl2),
                    JsonLight = (string)null
                },
            };

            var testCasesWithMultipleLinks = testCases.Variations()
                .Select(tcs =>
                new
                {
                    NavigationLinks = tcs.Select(tc => tc.NavigationLink),
                    Atom = string.Concat(tcs.Select(tc => tc.Atom)),
                    JsonLight = string.Join(",", tcs.Where(tc => tc.JsonLight != null).Select(tc => tc.JsonLight))
                });

            var testDescriptors = testCasesWithMultipleLinks.Select(testCase =>
            {
                ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                entry.TypeName = "TestModel.CustomerType";
                List<ODataItem> items = new ODataItem[] { entry }.ToList();
                foreach (var navLink in testCase.NavigationLinks)
                {
                    items.Add(navLink);
                    items.Add(null);
                }

                return new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    items,
                    (testConfiguration) =>
                    {
                        var firstAssocLink = testCase.NavigationLinks == null ? null : testCase.NavigationLinks.FirstOrDefault();
                        if (testConfiguration.Format == ODataFormat.Json)
                        {
                            return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                Json = string.Join(
                                    "$(NL)",
                                    "{",
                                    testCase.JsonLight,
                                    "}"),
                                FragmentExtractor = (result) =>
                                {
                                    var associationLinks = result.Object().GetAnnotationsWithName("@" + JsonLightConstants.ODataAssociationLinkUrlAnnotationName).ToList();
                                    var jsonResult = new JsonObject();
                                    associationLinks.ForEach(l =>
                                    {
                                        // NOTE we remove all annoatations here and in particular the text annotations to be able to easily compare
                                        //      against the expected results. This however means that we do not distinguish between the indented and non-indented case here.
                                        l.RemoveAllAnnotations(true);
                                        jsonResult.Add(l);
                                    });
                                    return jsonResult;
                                },
                            };
                        }
                        else
                        {
                            this.Settings.Assert.Fail("Unknown format '{0}'.", testConfiguration.Format);
                            return null;
                        }
                    })
                {
                    Model = model,
                    PayloadEdmElementContainer = customerSet
                };
            });

            // With and without model
            testDescriptors = testDescriptors.SelectMany(td =>
                new[]
                {
                    td,
                    new PayloadWriterTestDescriptor<ODataItem>(td)
                    {
                        Model = null,
                        PayloadEdmElementContainer = null
                    }
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.EntryPayloads),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    if (testDescriptor.Model == null && testConfiguration.Format == ODataFormat.Json)
                    {
                        return;
                    }

                    if (testDescriptor.IsGeneratedPayload && testDescriptor.Model != null)
                    {
                        return;
                    }

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Veifies correct writing for association links specified directly on the nav. link.")]
        public void AssociationLinkOnNavigationLinkTest()
        {
            EdmModel model = new EdmModel();

            var container = new EdmEntityContainer("TestModel", "Default");
            model.AddElement(container);

            var edmEntityTypeOrderType = new EdmEntityType("TestModel", "OrderType");
            model.AddElement(edmEntityTypeOrderType);

            var edmEntityTypeCustomerType = new EdmEntityType("TestModel", "CustomerType");
            var edmNavigationPropertyAssociationLinkOne = edmEntityTypeCustomerType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo { Name = "NavProp1", Target = edmEntityTypeOrderType, TargetMultiplicity = EdmMultiplicity.One });
            var edmNavigationPropertyAssociationLinkTwo = edmEntityTypeCustomerType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo { Name = "NavProp2", Target = edmEntityTypeOrderType, TargetMultiplicity = EdmMultiplicity.Many });
            model.AddElement(edmEntityTypeCustomerType);

            var customerSet = container.AddEntitySet("Customers", edmEntityTypeCustomerType);
            var orderSet = container.AddEntitySet("Orders", edmEntityTypeOrderType);
            customerSet.AddNavigationTarget(edmNavigationPropertyAssociationLinkOne, orderSet);
            customerSet.AddNavigationTarget(edmNavigationPropertyAssociationLinkTwo, orderSet);

            var testCases = new[]
            {
                // Both nav link URL and association link URL
                new {
                    NavigationLink = new ODataNestedResourceInfo() { Name = "NavProp1", IsCollection = false, Url = new Uri("http://odata.org/navlink"), AssociationLinkUrl = new Uri("http://odata.org/assoclink") },
                    PropertyName = "NavProp1",
                    Atom = BuildXmlNavigationLink("NavProp1", "application/atom+xml;type=entry", "http://odata.org/navlink"),
                    JsonLight =
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("NavProp1", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("NavProp1", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/assoclink\""
                },
                // Just nav link URL
                new {
                    NavigationLink = new ODataNestedResourceInfo() { Name = "NavProp1", IsCollection = false, Url = new Uri("http://odata.org/navlink"), AssociationLinkUrl = null },
                    PropertyName = "NavProp1",
                    Atom = BuildXmlNavigationLink("NavProp1", "application/atom+xml;type=entry", "http://odata.org/navlink"),
                    JsonLight = "\"" + JsonLightUtils.GetPropertyAnnotationName("NavProp1", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navlink\""
                },
                // Just association link URL
                new {
                    NavigationLink = new ODataNestedResourceInfo() { Name = "NavProp1", IsCollection = false, Url = null, AssociationLinkUrl = new Uri("http://odata.org/assoclink") },
                    PropertyName = "NavProp1",
                    Atom = (string)null,
                    JsonLight = "\"" + JsonLightUtils.GetPropertyAnnotationName("NavProp1", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/assoclink\""
                },
                // Navigation link with both URLs null
                new {
                    NavigationLink = new ODataNestedResourceInfo() { Name = "NavProp1", IsCollection = false, Url = null, AssociationLinkUrl = null },
                    PropertyName = "NavProp1",
                    Atom = (string)null,
                    JsonLight = string.Empty
                }
            };

            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testDescriptors = testCases.Select(testCase =>
                {
                    ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                    entry.TypeName = "TestModel.CustomerType";
                    return new PayloadWriterTestDescriptor<ODataItem>(
                        this.Settings,
                        new ODataItem[] { entry, testCase.NavigationLink, null },
                        (testConfiguration) =>
                        {
                            if (testConfiguration.Format == ODataFormat.Json)
                            {
                                return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                                {
                                    Json = string.Join(
                                        "$(NL)",
                                        "{",
                                        testCase.JsonLight,
                                        "}"),
                                    FragmentExtractor = (result) =>
                                    {
                                        var links = result.Object().GetPropertyAnnotations(testCase.PropertyName).ToList();
                                        var jsonResult = new JsonObject();
                                        links.ForEach(l =>
                                        {
                                            // NOTE we remove all annoatations here and in particular the text annotations to be able to easily compare
                                            //      against the expected results. This however means that we do not distinguish between the indented and non-indented case here.
                                            l.RemoveAllAnnotations(true);
                                            jsonResult.Add(l);
                                        });
                                        return jsonResult;
                                    }
                                };
                            }
                            else
                            {
                                this.Settings.Assert.Fail("Unknown format '{0}'.", testConfiguration.Format);
                                return null;
                            }
                        })
                    {
                        Model = model,
                        PayloadEdmElementContainer = customerSet
                    };
                });

            // With and without model
            testDescriptors = testDescriptors.SelectMany(td =>
                new[]
                {
                    td,
                    new PayloadWriterTestDescriptor<ODataItem>(td)
                    {
                        Model = null,
                        PayloadEdmElementContainer = null
                    }
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.EntryPayloads),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(testConfiguration => !testConfiguration.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    if (testDescriptor.Model == null && testConfiguration.Format == ODataFormat.Json)
                    {
                        return;
                    }

                    if (testDescriptor.IsGeneratedPayload && testDescriptor.Model != null)
                    {
                        return;
                    }

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies correct metadata validation for association links.")]
        public void AssociationLinkMetadataValidationTest()
        {
            EdmModel model = new EdmModel();

            var container = new EdmEntityContainer("TestModel", "Default");
            model.AddElement(container);

            var edmEntityTypeOrderType = new EdmEntityType("TestModel", "OrderType");
            model.AddElement(edmEntityTypeOrderType);

            var edmEntityTypeCustomerType = new EdmEntityType("TestModel", "CustomerType");
            edmEntityTypeCustomerType.AddKeys(new EdmStructuralProperty(edmEntityTypeCustomerType, "ID", EdmCoreModel.Instance.GetInt32(false)));
            edmEntityTypeCustomerType.AddStructuralProperty("PrimitiveProperty", EdmCoreModel.Instance.GetString(true));
            var edmNavigationPropertyAssociationLinkOne = edmEntityTypeCustomerType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo { Name = "Orders", Target = edmEntityTypeOrderType, TargetMultiplicity = EdmMultiplicity.Many });
            var edmNavigationPropertyAssociationLinkTwo = edmEntityTypeCustomerType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo { Name = "BestFriend", Target = edmEntityTypeCustomerType, TargetMultiplicity = EdmMultiplicity.One });
            model.AddElement(edmEntityTypeCustomerType);

            var edmEntityTypeOpenCustomerType = new EdmEntityType("TestModel", "OpenCustomerType", edmEntityTypeCustomerType, isAbstract: false, isOpen: true);
            model.AddElement(edmEntityTypeOpenCustomerType);

            var customerSet = container.AddEntitySet("Customers", edmEntityTypeCustomerType);
            var orderSet = container.AddEntitySet("Orders", edmEntityTypeOrderType);
            customerSet.AddNavigationTarget(edmNavigationPropertyAssociationLinkOne, orderSet);
            customerSet.AddNavigationTarget(edmNavigationPropertyAssociationLinkTwo, customerSet);

            Uri associationLinkUrl = new Uri("http://odata.org/associationlink");

            var testCases = new[]
            {
                // Valid collection
                new
                {
                    TypeName = "TestModel.CustomerType",
                    NavigationLink = ObjectModelUtils.CreateDefaultCollectionLink("Orders"),
                    ExpectedException = (ExpectedException)null,
                },
                // Valid singleton
                new
                {
                    TypeName = "TestModel.CustomerType",
                    NavigationLink = ObjectModelUtils.CreateDefaultSingletonLink("BestFriend"),
                    ExpectedException = (ExpectedException)null,
                },
                // Undeclared on closed type
                new
                {
                    TypeName = "TestModel.CustomerType",
                    NavigationLink = ObjectModelUtils.CreateDefaultCollectionLink("NonExistant"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", "NonExistant", "TestModel.CustomerType"),
                },
                // Undeclared on open type
                new
                {
                    TypeName = "TestModel.OpenCustomerType",
                    NavigationLink = ObjectModelUtils.CreateDefaultCollectionLink("NonExistant"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_OpenNavigationProperty", "NonExistant", "TestModel.OpenCustomerType"),
                },
                // Declared but of wrong kind
                new
                {
                    TypeName = "TestModel.CustomerType",
                    NavigationLink = ObjectModelUtils.CreateDefaultSingletonLink("PrimitiveProperty"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_NavigationPropertyExpected", "PrimitiveProperty", "TestModel.CustomerType", "Structural"),
                },
            };

            var testDescriptors = testCases.Select(testCase =>
                {
                    ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                    entry.TypeName = testCase.TypeName;
                    ODataNestedResourceInfo navigationLink = testCase.NavigationLink;
                    navigationLink.AssociationLinkUrl = associationLinkUrl;

                    return new PayloadWriterTestDescriptor<ODataItem>(
                        this.Settings,
                        new ODataItem[] { entry, navigationLink },
                        tc => (WriterTestExpectedResults)new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            ExpectedException2 = testCase.ExpectedException
                        })
                    {
                        Model = model,
                        PayloadEdmElementContainer = customerSet
                    };
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations
                    .Where(testConfiguration => !testConfiguration.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        private static string BuildXmlNavigationLink(string navigationLinkName, string mimeType, string navigationLinkUrl)
        {
            return "<link rel=\"" + TestAtomConstants.ODataNavigationPropertiesRelatedLinkRelationPrefix + navigationLinkName + "\" "
                        + "type=\"" + mimeType + "\" title=\"" + navigationLinkName + "\" href=\"" + navigationLinkUrl + "\" "
                        + "xmlns=\"" + TestAtomConstants.AtomNamespace + "\" />";
        }

        private static string BuildXmlAssociationLink(string associationLinkName, string mimeType, string associationLinkUrl)
        {
            return "<link rel=\"" + TestAtomConstants.ODataNavigationPropertiesAssociationLinkRelationPrefix + associationLinkName + "\" "
                        + "type=\"" + mimeType + "\" title=\"" + associationLinkName + "\" href=\"" + associationLinkUrl + "\" "
                        + "xmlns=\"" + TestAtomConstants.AtomNamespace + "\" />";
        }
    }
}
