//---------------------------------------------------------------------
// <copyright file="WriterInputValidationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
    /// <summary>
    /// Tests to verify writer correctly validates input
    /// </summary>
    // [TestClass, TestCase]
    public class WriterInputValidationTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");

        private static readonly ODataResourceSerializationInfo SerializationInfo = new ODataResourceSerializationInfo()
        {
            NavigationSourceEntityTypeName = "Null",
            NavigationSourceName = "MySet",
            ExpectedTypeName = "Null"
        };

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Tests that either no or an absolute base Uri is specified.")]
        public void BaseUriValidationTest()
        {
            string relativeUriString = "abc/pqr/";
            Uri absoluteUri = new Uri("http://odata.org");
            Uri relativeUri = absoluteUri.MakeRelativeUri(new Uri(absoluteUri, relativeUriString));

            string expectedError = "The base URI '" + relativeUriString + "' specified in ODataMessageWriterSettings.BaseUri is invalid; it must either be null or an absolute URI.";

            ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
            var testDescriptors = new []
                {
                    new
                    {
                        BaseUri = relativeUri,
                        TestDescriptor = new PayloadWriterTestDescriptor<ODataItem>(this.Settings, entry, testConfiguration =>
                            new WriterTestExpectedResults(this.Settings.ExpectedResultSettings) { ExpectedODataExceptionMessage = expectedError })
                    }
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    // clone the test configuration and set an invalid base Uri
                    ODataMessageWriterSettings settings = testConfiguration.MessageWriterSettings.Clone();
                    settings.BaseUri = testDescriptor.BaseUri;

                    WriterTestConfiguration config =
                        new WriterTestConfiguration(testConfiguration.Format, settings, testConfiguration.IsRequest, testConfiguration.Synchronous);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor.TestDescriptor, config, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Tests that entry is correctly validated.")]
        public void EntryValidationTest()
        {
            var testCases = new[] {
                new { // type name must not be empty
                    InvalidateEntry = new Action<ODataResource>(entry => entry.TypeName = string.Empty),
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_TypeNameMustNotBeEmpty")
                },
            };

            // Convert test cases to test descriptions
            var testDescriptors = testCases.Select(testCase =>
                {
                    ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                    testCase.InvalidateEntry(entry);
                    return new PayloadWriterTestDescriptor<ODataItem>(this.Settings, entry, testConfiguration =>
                        new WriterTestExpectedResults(this.Settings.ExpectedResultSettings) { ExpectedException2 = testCase.ExpectedException });
                });

            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.EntryPayloads),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => false),
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Tests that properties are correctly validated.")]
        public void PropertyValidationTest()
        {
            var testCases = new[] {
                new { // null property is not valid
                    Property = (ODataProperty)null,
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_PropertyMustNotBeNull")
                },
                new { // null property name is not valid
                    Property = new ODataProperty() { Name = null },
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_PropertiesMustHaveNonEmptyName")
                },
                new { // empty property name is not valid
                    Property = new ODataProperty() { Name = string.Empty },
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_PropertiesMustHaveNonEmptyName")
                },
            };

            var testDescriptors = testCases.Select(testCase =>
                {
                    return new PayloadWriterTestDescriptor<ODataItem>(
                        this.Settings,
                        new ODataResource() { Properties = new ODataProperty[] { testCase.Property } },
                        testConfiguration => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings) { ExpectedException2 = testCase.ExpectedException });
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.PropertyPayloads),
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        private sealed class NavigationLinkValidationTestCase
        {
            public Action<ODataNestedResourceInfo> InvalidateLink { get; set; }
            public Func<ODataNestedResourceInfo, ExpectedException> ExpectedException { get; set; }
            public Func<Microsoft.Test.Taupo.OData.Common.TestConfiguration, bool> SkipTestConfiguration { get; set; }
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Tests that navigation links are correctly validated.")]
        public void NavigationLinkValidationTest()
        {
            var testCases = new[] {
                new NavigationLinkValidationTestCase { // null link name is not valid
                    InvalidateLink = link => link.Name = null,
                    ExpectedException = link => ODataExpectedExceptions.ODataException("ValidationUtils_LinkMustSpecifyName"),
                },
                new NavigationLinkValidationTestCase { // empty link name is not valid
                    InvalidateLink = link => link.Name = string.Empty,
                    ExpectedException = link => ODataExpectedExceptions.ODataException("ValidationUtils_LinkMustSpecifyName"),
                },
                new NavigationLinkValidationTestCase { // null link Url is not valid
                    InvalidateLink = link => link.Url = null,
                    ExpectedException = link => ODataExpectedExceptions.ODataException("WriterValidationUtils_NavigationLinkMustSpecifyUrl", link.Name),
                    SkipTestConfiguration = (testConfiguration) => testConfiguration.Format == ODataFormat.Json || testConfiguration.IsRequest
                },
            };

            var testDescriptors = testCases.Select(testCase =>
            {
                ODataNestedResourceInfo link = ObjectModelUtils.CreateDefaultCollectionLink();
                testCase.InvalidateLink(link);
                return new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    link,
                    testConfiguration => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings) { ExpectedException2 = testCase.ExpectedException(link) })
                {
                    SkipTestConfiguration = testCase.SkipTestConfiguration
                };
            });

            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.NavigationLinkOnlyPayloads),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => false),
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor.DeferredLinksToEntityReferenceLinksInRequest(testConfiguration), testConfiguration, this.Assert, this.Logger);
                });
        }

        private sealed class NamedStreamValidationTestCase
        {
            public ODataProperty StreamProperty { get; set; }
            public ExpectedException ExpectedException { get; set; }
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Tests that named streams are correctly validated.")]
        public void NamedStreamValidationTest()
        {
            var testCases = new NamedStreamValidationTestCase[] {
                // null property is not valid
                new NamedStreamValidationTestCase
                {
                    StreamProperty = (ODataProperty)null,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_EnumerableContainsANullItem", "ODataResource.Properties"),
                },
                // null property name is not valid
                new NamedStreamValidationTestCase
                {
                    StreamProperty = new ODataProperty() { Name = null, Value = new ODataStreamReferenceValue() },
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_PropertiesMustHaveNonEmptyName"),
                },
                // empty property name is not valid
                new NamedStreamValidationTestCase
                {
                    StreamProperty = new ODataProperty() { Name = string.Empty, Value = new ODataStreamReferenceValue() },
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_PropertiesMustHaveNonEmptyName"),
                },
                // ReadLink and EditLink cannot be both null
                new NamedStreamValidationTestCase
                {
                    StreamProperty = new ODataProperty() { Name = "Stream1", Value = new ODataStreamReferenceValue() },
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_StreamReferenceValueMustHaveEditLinkOrReadLink"),
                },
                // empty content type is not valid
                new NamedStreamValidationTestCase
                {
                    StreamProperty = new ODataProperty() { Name = "Stream1", Value = new ODataStreamReferenceValue() { ContentType = string.Empty } },
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_StreamReferenceValueEmptyContentType"),
                },
                // etag without an edit link is invalid
                new NamedStreamValidationTestCase
                {
                    StreamProperty = new ODataProperty() { Name = "Stream1", Value = new ODataStreamReferenceValue() { ReadLink = new Uri("someUri", UriKind.RelativeOrAbsolute), ETag = "\"etagValue\"" }},
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_StreamReferenceValueMustHaveEditLinkToHaveETag"),
                },
                // relative read link without base uri is invalid
                new NamedStreamValidationTestCase
                {
                    StreamProperty = new ODataProperty() { Name = "Stream1", Value = new ODataStreamReferenceValue() { ReadLink = new Uri("someUri", UriKind.RelativeOrAbsolute) }},
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataWriter_RelativeUriUsedWithoutBaseUriSpecified", "someUri"),
                },
                // relative edit link without base uri is invalid
                new NamedStreamValidationTestCase
                {
                    StreamProperty = new ODataProperty() { Name = "Stream1", Value = new ODataStreamReferenceValue() { EditLink = new Uri("someUri", UriKind.RelativeOrAbsolute) }},
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataWriter_RelativeUriUsedWithoutBaseUriSpecified", "someUri"),
                },
            };

            var testDescriptors = testCases.Select(testCase =>
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    new ODataResource() { Properties = new ODataProperty[] { testCase.StreamProperty } },
                    testConfiguration => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings) { ExpectedException2 = testCase.ExpectedException }));

            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.NamedStreamPayloads),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => false),
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Tests that etag values are correctly validated.")]
        public void ETagValidationTest()
        {
            string[] etagValues = new string[]
            {
                 "\"",
                "W/\"",
                "W/",
                "etagValue",
                "etagValue\"",
                "\"etagValue",
                "etag\"Value",
                "\"etagV\"alue\"",
                "W/etagValue",
                "W/etagValue\"",
                "W/\"etagValue",
                "W/etag\"Value",
                "W/\"etagV\"alue\""
            };

            var testDescriptors = etagValues.SelectMany(etagValue =>
            {
                var stream1 = new ODataStreamReferenceValue() { ReadLink = new Uri("http://foo/", UriKind.RelativeOrAbsolute), EditLink = new Uri("http://foo/", UriKind.RelativeOrAbsolute), ContentType = "customType/customSubtype", ETag = etagValue };

                WriterTestDescriptor.WriterTestExpectedResultCallback formatSelector =
                    testConfiguration => (WriterTestExpectedResults)new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings);

                return new[]
                {
                    // entity etag
                    new PayloadWriterTestDescriptor<ODataItem>(
                        this.Settings,
                        new ODataResource() { ETag = etagValue, Properties = ObjectModelUtils.CreateDefaultPrimitiveProperties(), SerializationInfo = SerializationInfo },
                        formatSelector),
                    // default stream etag
                    new PayloadWriterTestDescriptor<ODataItem>(
                        this.Settings,
                        new ODataResource() { MediaResource = stream1, Properties = ObjectModelUtils.CreateDefaultPrimitiveProperties(), SerializationInfo = SerializationInfo },
                        formatSelector),
                    // named stream etag
                    new PayloadWriterTestDescriptor<ODataItem>(
                        this.Settings,
                        new ODataResource() { Properties = new ODataProperty[] { new ODataProperty() { Name = "Stream1", Value = stream1 }}, SerializationInfo = SerializationInfo},
                        formatSelector)
                        {
                            // No stream properties in requests
                            SkipTestConfiguration = tc => tc.IsRequest
                        },
                };
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Tests that complex values are correctly validated.")]
        public void ComplexValueValidationTest()
        {
            var testCases = new[] {
                new { // empty type name is not valid
                    ComplexValue = new ODataComplexValue() { TypeName = "" },
                    ExpectedExceptionMessage = "An empty type name was found; the name of a type cannot be an empty string."
                },
            };

            var testDescriptors = testCases.Select(testCase =>
            {
                return new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    new ODataResource() { Properties = new ODataProperty[] { new ODataProperty() { Name = "ComplexProperty", Value = testCase.ComplexValue } } },
                    testConfiguration => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings) { ExpectedODataExceptionMessage = testCase.ExpectedExceptionMessage });
            });

            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.ValuePayloads),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => false),
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Tests that collections are correctly validated.")]
        public void CollectionValidationTest()
        {
            var testCases = new[] {
                new { // empty type name is not valid
                    Collection = new ODataCollectionValue() { TypeName = string.Empty },
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_TypeNameMustNotBeEmpty"),
                },
            };

            var testDescriptors = testCases.Select(testCase =>
            {
                return new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    new ODataResource() { Properties = new ODataProperty[] { new ODataProperty() { Name = "CollectionProperty", Value = testCase.Collection } } },
                    testConfiguration => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings) { ExpectedException2 = testCase.ExpectedException });
            });

            //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.ValuePayloads),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => false),
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Tests that default streams are correctly validated.")]
        public void DefaultStreamValidationTest()
        {
            var testCases = new[] {
                new { // empty content type is invalid
                    InvalidateDefaultStream = new Action<ODataStreamReferenceValue>(mediaResource => mediaResource.ContentType = string.Empty),
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_StreamReferenceValueEmptyContentType"),
                },
                new { // null read link and non-empty content type is invalid
                    InvalidateDefaultStream = new Action<ODataStreamReferenceValue>(mediaResource => { mediaResource.ReadLink = null; mediaResource.ContentType = "mime/type"; }),
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_DefaultStreamWithContentTypeWithoutReadLink"),
                },
                new { // non-null read link and null content type is invalid
                    InvalidateDefaultStream = new Action<ODataStreamReferenceValue>(mediaResource => { mediaResource.ReadLink = new Uri("http://odata.org"); mediaResource.ContentType = null; }),
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_DefaultStreamWithReadLinkWithoutContentType"),
                },
                new { // etag without an edit link is invalid
                    InvalidateDefaultStream = new Action<ODataStreamReferenceValue>(mediaResource => { mediaResource.ETag = "someetag"; mediaResource.EditLink = null; }),
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_StreamReferenceValueMustHaveEditLinkToHaveETag"),
                },
            };

            var testDescriptors = testCases.Select(testCase =>
            {
                ODataStreamReferenceValue mediaResource = ObjectModelUtils.CreateDefaultStream();
                testCase.InvalidateDefaultStream(mediaResource);
                ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                entry.MediaResource = mediaResource;
                return new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    entry,
                    testConfiguration => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings) { ExpectedException2 = testCase.ExpectedException });
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.EntryPayloads),
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Tests that entity reference link is correctly validated.")]
        public void EntityReferenceLinkValidationTest()
        {
            var testCases = new[] {
                new {
                    InvalidateEntityReferenceLink = new Action<ODataEntityReferenceLink>(entityReferenceLink => entityReferenceLink.Url = null),
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_EntityReferenceLinkUrlMustNotBeNull"),
                },
            };

            var testDescriptors = testCases.SelectMany(testCase =>
            {
                ODataEntityReferenceLink entityReferenceLink = ObjectModelUtils.CreateDefaultEntityReferenceLink();
                testCase.InvalidateEntityReferenceLink(entityReferenceLink);
                ODataEntityReferenceLinks entityReferenceLinks = ObjectModelUtils.CreateDefaultEntityReferenceLinks();
                testCase.InvalidateEntityReferenceLink(entityReferenceLinks.Links.First());

                return new PayloadWriterTestDescriptor[]
                {
                    // Top level entity reference link
                    new PayloadWriterTestDescriptor<ODataEntityReferenceLink>(
                        this.Settings,
                        entityReferenceLink,
                        testConfiguration => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings) { ExpectedException2 = testCase.ExpectedException }),
                    // Entity reference link in entity reference links
                    new PayloadWriterTestDescriptor<ODataEntityReferenceLinks>(
                        this.Settings,
                        entityReferenceLinks,
                        testConfiguration => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings) {
                            // Top-level EntityReferenceLinks payload write requests are not allowed.                      
                            ExpectedException2 = testConfiguration.IsRequest ?
                                ODataExpectedExceptions.ODataException("ODataMessageWriter_EntityReferenceLinksInRequestNotAllowed") : testCase.ExpectedException
                        }),
                    new PayloadWriterTestDescriptor<ODataItem>(
                        this.Settings,
                        new ODataItem[] { ObjectModelUtils.CreateDefaultEntry(), ObjectModelUtils.CreateDefaultSingletonLink(), entityReferenceLink, null, null },
                        testConfiguration => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings) {
                            ExpectedException2 = testConfiguration.IsRequest ? testCase.ExpectedException : ODataExpectedExceptions.ODataException("ODataWriterCore_EntityReferenceLinkInResponse")
                        }),
                };
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    PayloadWriterTestDescriptor<ODataEntityReferenceLink> entityReferenceLinkTestDescriptor = testDescriptor as PayloadWriterTestDescriptor<ODataEntityReferenceLink>;
                    if (entityReferenceLinkTestDescriptor != null)
                    {
                        TestWriterUtils.WriteAndVerifyTopLevelContent(
                            entityReferenceLinkTestDescriptor,
                            testConfiguration,
                            (messageWriter) => messageWriter.WriteEntityReferenceLink(entityReferenceLinkTestDescriptor.PayloadItems.Single()),
                            this.Assert,
                            baselineLogger: this.Logger);
                        return;
                    }

                    PayloadWriterTestDescriptor<ODataEntityReferenceLinks> entityReferenceLinksTestDescriptor = testDescriptor as PayloadWriterTestDescriptor<ODataEntityReferenceLinks>;
                    if (entityReferenceLinksTestDescriptor != null)
                    {
                        TestWriterUtils.WriteAndVerifyTopLevelContent(
                            entityReferenceLinksTestDescriptor,
                            testConfiguration,
                            (messageWriter) => messageWriter.WriteEntityReferenceLinks(entityReferenceLinksTestDescriptor.PayloadItems.Single()),
                            this.Assert,
                            baselineLogger: this.Logger);
                        return;
                    }

                    TestWriterUtils.WriteAndVerifyODataPayload((PayloadWriterTestDescriptor<ODataItem>)testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Tests that entity reference links are correctly validated.")]
        public void EntityReferenceLinksValidationTest()
        {
            var testCases = new[] {
                new {
                    InvalidateEntityReferenceLinks = new Action<ODataEntityReferenceLinks>(entityReferenceLinks => entityReferenceLinks.Links = new ODataEntityReferenceLink[] { null }),
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_EntityReferenceLinksLinkMustNotBeNull"),
                },
            };

            var testDescriptors = testCases.Select(testCase =>
            {
                ODataEntityReferenceLinks entityReferenceLinks = ObjectModelUtils.CreateDefaultEntityReferenceLinks();
                testCase.InvalidateEntityReferenceLinks(entityReferenceLinks);
                return new PayloadWriterTestDescriptor<ODataEntityReferenceLinks>(
                    this.Settings,
                    entityReferenceLinks,
                    testConfiguration => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        // Top-level EntityReferenceLinks payload write requests are not allowed.
                        ExpectedException2 = testConfiguration.IsRequest ?
                                    ODataExpectedExceptions.ODataException("ODataMessageWriter_EntityReferenceLinksInRequestNotAllowed") : testCase.ExpectedException
                    });
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyTopLevelContent(
                        testDescriptor,
                        testConfiguration,
                        (messageWriter) => messageWriter.WriteEntityReferenceLinks(testDescriptor.PayloadItems.Single()),
                        this.Assert,
                        baselineLogger: this.Logger);
                    return;
                });
        }
    }
}
