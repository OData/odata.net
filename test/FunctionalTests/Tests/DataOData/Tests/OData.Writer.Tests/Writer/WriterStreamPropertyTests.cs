//---------------------------------------------------------------------
// <copyright file="WriterStreamPropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Json.TextAnnotations;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Writer.Tests;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
    /// <summary>
    /// Tests for writing entries with named stream properties with the OData writer.
    /// </summary>
    [TestClass, TestCase]
    public class WriterStreamPropertyTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://www.odata.org/");

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Validates the payloads for various stream properties.")]
        public void WriterStreamPropertiesTests()
        {
            Uri baseUri = new Uri("http://www.odata.org/", UriKind.Absolute);
            Uri relativeReadLinkUri = new Uri("readlink", UriKind.RelativeOrAbsolute);
            Uri relativeEditLinkUri = new Uri("editlink", UriKind.RelativeOrAbsolute);
            Uri absoluteReadLinkUri = new Uri(baseUri, relativeReadLinkUri.OriginalString);
            Uri absoluteEditLinkUri = new Uri(baseUri, relativeEditLinkUri.OriginalString);

            string contentType = "application/binary";
            string etag = "\"myetagvalue\"";
            string streamPropertyName = "stream1";

            var namedStreamProperties = new[]
            {
                // with only read link
                new ODataProperty { Name = streamPropertyName, Value = new ODataStreamReferenceValue { ReadLink = relativeReadLinkUri }},
                new ODataProperty { Name = streamPropertyName, Value = new ODataStreamReferenceValue { ReadLink = relativeReadLinkUri, ContentType = contentType}},
                new ODataProperty { Name = streamPropertyName, Value = new ODataStreamReferenceValue { ReadLink = absoluteReadLinkUri }},
                new ODataProperty { Name = streamPropertyName, Value = new ODataStreamReferenceValue { ReadLink = absoluteReadLinkUri, ContentType = contentType}},
                // with only edit link
                new ODataProperty { Name = streamPropertyName, Value = new ODataStreamReferenceValue { EditLink = relativeEditLinkUri }},
                new ODataProperty { Name = streamPropertyName, Value = new ODataStreamReferenceValue { EditLink = relativeEditLinkUri, ContentType = contentType }},
                new ODataProperty { Name = streamPropertyName, Value = new ODataStreamReferenceValue { EditLink = relativeEditLinkUri, ContentType = contentType, ETag = etag }},
                new ODataProperty { Name = streamPropertyName, Value = new ODataStreamReferenceValue { EditLink = absoluteEditLinkUri }},
                new ODataProperty { Name = streamPropertyName, Value = new ODataStreamReferenceValue { EditLink = absoluteEditLinkUri, ContentType = contentType }},
                new ODataProperty { Name = streamPropertyName, Value = new ODataStreamReferenceValue { EditLink = absoluteEditLinkUri, ContentType = contentType, ETag = etag }},
                // with both edit and read link
                new ODataProperty { Name = streamPropertyName, Value = new ODataStreamReferenceValue { ReadLink = relativeReadLinkUri, EditLink = relativeEditLinkUri }},
                new ODataProperty { Name = streamPropertyName, Value = new ODataStreamReferenceValue { ReadLink = relativeReadLinkUri, EditLink = relativeEditLinkUri, ContentType = contentType}},
                new ODataProperty { Name = streamPropertyName, Value = new ODataStreamReferenceValue { ReadLink = relativeReadLinkUri, EditLink = relativeEditLinkUri, ContentType = contentType, ETag = etag }},
                new ODataProperty { Name = streamPropertyName, Value = new ODataStreamReferenceValue { ReadLink = absoluteReadLinkUri, EditLink = relativeEditLinkUri }},
                new ODataProperty { Name = streamPropertyName, Value = new ODataStreamReferenceValue { ReadLink = absoluteReadLinkUri, EditLink = relativeEditLinkUri, ContentType = contentType}},
                new ODataProperty { Name = streamPropertyName, Value = new ODataStreamReferenceValue { ReadLink = absoluteReadLinkUri, EditLink = relativeEditLinkUri, ContentType = contentType, ETag = etag }},
            };

            var testCases = namedStreamProperties.Select(property =>
            {
                var propertyName = property.Name;
                var streamReferenceValue = (ODataStreamReferenceValue)property.Value;
                return new StreamPropertyTestCase
                {
                    NamedStreamProperty = property,
                    GetExpectedAtomPayload = (testConfiguration) =>
                        {
                            return
                                (streamReferenceValue.ReadLink == null
                                    ? string.Empty
                                    : (
                                        "<link rel=\"http://docs.oasis-open.org/odata/ns/mediaresource/" + property.Name + "\" " +
                                        (streamReferenceValue.ContentType == null ? string.Empty : "type=\"" + streamReferenceValue.ContentType + "\" ") +
                                        "title=\"" + property.Name + "\" " +
                                        "href=\"" + (((ODataStreamReferenceValue)property.Value).ReadLink.IsAbsoluteUri ? absoluteReadLinkUri.OriginalString : relativeReadLinkUri.OriginalString) + "\" " +
                                        "xmlns=\"" + TestAtomConstants.AtomNamespace + "\" />")) +

                                (streamReferenceValue.EditLink == null
                                    ? string.Empty
                                    : (
                                        "<link rel=\"http://docs.oasis-open.org/odata/ns/edit-media/" + property.Name + "\" " +
                                        (streamReferenceValue.ContentType == null ? string.Empty : "type=\"" + streamReferenceValue.ContentType + "\" ") +
                                        "title=\"" + property.Name + "\" " +
                                        "href=\"" + (((ODataStreamReferenceValue)property.Value).EditLink.IsAbsoluteUri ? absoluteEditLinkUri.OriginalString : relativeEditLinkUri.OriginalString) + "\" " +
                                        (streamReferenceValue.ETag == null ? string.Empty : "m:etag=\"" + streamReferenceValue.ETag.Replace("\"", "&quot;") + "\" xmlns:m=\"" + TestAtomConstants.ODataMetadataNamespace + "\" ") +
                                        "xmlns=\"" + TestAtomConstants.AtomNamespace + "\" />"));
                        },
                    GetExpectedJsonLightPayload = (testConfiguration) =>
                        {
                            return JsonLightWriterUtils.CombineProperties(
                                (streamReferenceValue.EditLink == null ? string.Empty : ("\"" + JsonLightUtils.GetPropertyAnnotationName(propertyName, JsonLightConstants.ODataMediaEditLinkAnnotationName) + "\":\"" + absoluteEditLinkUri.OriginalString + "\"")),
                                (streamReferenceValue.ReadLink == null ? string.Empty : ("\"" + JsonLightUtils.GetPropertyAnnotationName(propertyName, JsonLightConstants.ODataMediaReadLinkAnnotationName) + "\":\"" + absoluteReadLinkUri.OriginalString + "\"")),
                                (streamReferenceValue.ContentType == null ? string.Empty : ("\"" + JsonLightUtils.GetPropertyAnnotationName(propertyName, JsonLightConstants.ODataMediaContentTypeAnnotationName) + "\":\"" + streamReferenceValue.ContentType + "\"")),
                                (streamReferenceValue.ETag == null ? string.Empty : ("\"" + JsonLightUtils.GetPropertyAnnotationName(propertyName, JsonLightConstants.ODataMediaETagAnnotationName) + "\":\"" + streamReferenceValue.ETag.Replace("\"", "\\\"") + "\"")));
                        },
                };
            });

            var testDescriptors = testCases.SelectMany(testCase =>
                {
                    EdmModel model = new EdmModel();

                    EdmEntityType edmEntityType = new EdmEntityType("TestModel", "StreamPropertyEntityType");
                    EdmStructuralProperty edmStructuralProperty = edmEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false);
                    edmEntityType.AddKeys(new IEdmStructuralProperty[] { edmStructuralProperty });
                    model.AddElement(edmEntityType);

                    EdmEntityContainer edmEntityContainer = new EdmEntityContainer("TestModel", "DefaultContainer");
                    model.AddElement(edmEntityContainer);

                    EdmEntitySet edmEntitySet = new EdmEntitySet(edmEntityContainer, "StreamPropertyEntitySet", edmEntityType);
                    edmEntityContainer.AddElement(edmEntitySet);

                    ODataResource entry = new ODataResource()
                    {
                        Id = ObjectModelUtils.DefaultEntryId,
                        ReadLink = ObjectModelUtils.DefaultEntryReadLink,
                        TypeName = edmEntityType.FullName()
                    };

                    var streamReference = (ODataStreamReferenceValue)testCase.NamedStreamProperty.Value;
                    bool needBaseUri = (streamReference.ReadLink != null && !streamReference.ReadLink.IsAbsoluteUri) || (streamReference.EditLink != null && !streamReference.EditLink.IsAbsoluteUri);
                    entry.Properties = new ODataProperty[] { testCase.NamedStreamProperty };

                    var resultDescriptor = new PayloadWriterTestDescriptor<ODataItem>(
                        this.Settings,
                        entry,
                        (testConfiguration) =>
                        {
                            if (testConfiguration.Format == ODataFormat.Json)
                            {
                                return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                                {
                                    Json = string.Join(
                                        "$(NL)",
                                        "{",
                                        testCase.GetExpectedJsonLightPayload(testConfiguration),
                                        "}"),
                                    FragmentExtractor = result => result.RemoveAllAnnotations(true),
                                };
                            }
                            else
                            {
                                throw new NotSupportedException("Unsupported ODataFormat found: " + testConfiguration.Format.ToString());
                            }
                        })
                    {
                        Model = model,
                        PayloadEdmElementContainer = edmEntityContainer,
                        PayloadEdmElementType = edmEntityType,
                    };

                    var resultTestCases = new List<StreamPropertyTestDescriptor>();
                    if (needBaseUri)
                    {
                        resultTestCases.Add(new StreamPropertyTestDescriptor { BaseUri = baseUri, TestDescriptor = resultDescriptor });
                    }
                    else
                    {
                        resultTestCases.Add(new StreamPropertyTestDescriptor { BaseUri = null, TestDescriptor = resultDescriptor });
                        resultTestCases.Add(new StreamPropertyTestDescriptor { BaseUri = baseUri, TestDescriptor = resultDescriptor });
                        resultTestCases.Add(new StreamPropertyTestDescriptor { BaseUri = new Uri("http://mybaseuri/", UriKind.Absolute), TestDescriptor = resultDescriptor });
                    }

                    return resultTestCases;
                });

            var testDescriptorBaseUriPairSet = testDescriptors.SelectMany(descriptor =>
                WriterPayloads.NamedStreamPayloads(descriptor.TestDescriptor).Select(namedStreamPayload =>
                     new Tuple<PayloadWriterTestDescriptor<ODataItem>, Uri>(namedStreamPayload, descriptor.BaseUri)));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptorBaseUriPairSet,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptorBaseUriPair, testConfiguration) =>
                {
                    var testDescriptor = testDescriptorBaseUriPair.Item1;

                    if (testDescriptor.IsGeneratedPayload && testConfiguration.Format == ODataFormat.Json)
                    {
                        return;
                    }

                    ODataMessageWriterSettings settings = testConfiguration.MessageWriterSettings.Clone();
                    settings.BaseUri = testDescriptorBaseUriPair.Item2;
                    settings.SetServiceDocumentUri(ServiceDocumentUri);

                    WriterTestConfiguration config =
                        new WriterTestConfiguration(testConfiguration.Format, settings, testConfiguration.IsRequest, testConfiguration.Synchronous);

                    if (testConfiguration.IsRequest)
                    {
                        ODataResource payloadEntry = (ODataResource)testDescriptor.PayloadItems[0];
                        ODataProperty firstStreamProperty = payloadEntry.Properties.Where(p => p.Value is ODataStreamReferenceValue).FirstOrDefault();
                        this.Assert.IsNotNull(firstStreamProperty, "firstStreamProperty != null");

                        testDescriptor = new PayloadWriterTestDescriptor<ODataItem>(testDescriptor)
                        {
                            ExpectedResultCallback = tc => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                ExpectedException2 = ODataExpectedExceptions.ODataException("WriterValidationUtils_StreamPropertyInRequest", firstStreamProperty.Name)
                            }
                        };
                    }

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, config, this.Assert, this.Logger);
                });
        }

        private class StreamPropertyTestDescriptor
        {
            public Uri BaseUri { get; set; }
            public PayloadWriterTestDescriptor<ODataItem> TestDescriptor { get; set; }
        }

        private class StreamPropertyTestCase
        {
            public ODataProperty NamedStreamProperty { get; set; }
            public Func<WriterTestConfiguration, string> GetExpectedAtomPayload { get; set; }
            public Func<WriterTestConfiguration, string> GetExpectedJsonLightPayload { get; set; }
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Negative test cases for named streams.")]
        public void StreamPropertiesNegativeTests()
        {
            EdmModel model = new EdmModel();

            EdmComplexType edmComplexType = new EdmComplexType("TestModel", "MyComplexType");
            edmComplexType.AddStructuralProperty("Stream1", EdmCoreModel.Instance.GetStream(false));
            model.AddElement(edmComplexType);

            EdmEntityType edmEntityType = new EdmEntityType("TestModel", "EntityTypeForStreams");
            edmEntityType.AddStructuralProperty("Complex", new EdmComplexTypeReference(edmComplexType, false));
            edmEntityType.AddStructuralProperty("Collection", EdmCoreModel.GetCollection(new EdmComplexTypeReference(edmComplexType, false)));
            edmEntityType.AddStructuralProperty("Int32Collection", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false)));
            edmEntityType.AddStructuralProperty("NamedStreamCollection", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetStream(false)));
            model.AddElement(edmEntityType);

            var edmEntityContainer = new EdmEntityContainer("TestModel", "DefaultContainer");
            model.AddElement(edmEntityContainer);

            var entitySet = edmEntityContainer.AddEntitySet("EntitySetForStreams", edmEntityType);

            var testCases = new[] {
                // Note that negative test cases to validate the content of an ODataStreamReferenceValue are in WriteInputValidationTests.cs.
                // TODO: We need to add these test cases for writing top level properties and metadata as well.
                new { // named stream properties are not allowed on complex types
                    NamedStreamProperty = new ODataProperty()
                    {
                        Name = "Complex",
                        Value = new ODataComplexValue()
                        {
                            TypeName = "TestModel.MyComplexType",
                            Properties = new[]
                            {
                                new ODataProperty()
                                {
                                    Name = "Stream1",
                                    Value = new ODataStreamReferenceValue()
                                    {
                                        EditLink = new Uri("someUri", UriKind.RelativeOrAbsolute)
                                    }
                                }
                            }
                        }
                    },
                    ExpectedExceptionWithoutModel = ODataExpectedExceptions.ODataException("ODataWriter_StreamPropertiesMustBePropertiesOfODataResource"),
                    ExpectedExceptionWithModel = ODataExpectedExceptions.ODataException("ODataWriter_StreamPropertiesMustBePropertiesOfODataResource"),
                },
                new { // named stream properties are not allowed on complex collection types
                    NamedStreamProperty = new ODataProperty()
                    {
                        Name = "Collection",
                        Value = new ODataCollectionValue()
                        {
                            TypeName = EntityModelUtils.GetCollectionTypeName("TestModel.MyComplexType"),
                            Items = new[]
                            {
                                new ODataComplexValue()
                                {
                                    TypeName = "TestModel.MyComplexType",
                                    Properties = new[]
                                    {
                                        new ODataProperty()
                                        {
                                            Name = "Stream1",
                                            Value = new ODataStreamReferenceValue()
                                            {
                                                EditLink = new Uri("someUri", UriKind.RelativeOrAbsolute)
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    ExpectedExceptionWithoutModel = ODataExpectedExceptions.ODataException("ODataWriter_StreamPropertiesMustBePropertiesOfODataResource"),
                    ExpectedExceptionWithModel = ODataExpectedExceptions.ODataException("ODataWriter_StreamPropertiesMustBePropertiesOfODataResource"),
                },
                // TODO: Add the following case for the top-level collection writer as well.
                new { // named stream collection properties are not allowed.
                    NamedStreamProperty = new ODataProperty()
                    {
                        Name = "Int32Collection",
                        Value = new ODataCollectionValue()
                        {
                            TypeName = EntityModelUtils.GetCollectionTypeName("Edm.Int32"),
                            Items = new[]
                            {
                                new ODataStreamReferenceValue()
                                {
                                    EditLink = new Uri("someUri", UriKind.RelativeOrAbsolute)
                                }
                            }
                        }
                    },
                    ExpectedExceptionWithoutModel = ODataExpectedExceptions.ODataException("ValidationUtils_StreamReferenceValuesNotSupportedInCollections"),
                    ExpectedExceptionWithModel = ODataExpectedExceptions.ODataException("ValidationUtils_StreamReferenceValuesNotSupportedInCollections"),
                },
                new { // named stream collection properties are not allowed - with valid type.
                    NamedStreamProperty = new ODataProperty()
                    {
                        Name = "NamedStreamCollection",
                        Value = new ODataCollectionValue()
                        {
                            TypeName = EntityModelUtils.GetCollectionTypeName("Edm.Stream"),
                            Items = new[]
                            {
                                new ODataStreamReferenceValue()
                                {
                                    EditLink = new Uri("someUri", UriKind.RelativeOrAbsolute)
                                }
                            }
                        }
                    },
                    ExpectedExceptionWithoutModel = ODataExpectedExceptions.ODataException("ValidationUtils_StreamReferenceValuesNotSupportedInCollections"),
                    ExpectedExceptionWithModel = ODataExpectedExceptions.ODataException("EdmLibraryExtensions_CollectionItemCanBeOnlyPrimitiveEnumComplex"),
                },
            };

            var testDescriptors = testCases.SelectMany(testCase =>
            {
                ODataResource entry = new ODataResource()
                {
                    TypeName = "TestModel.EntityTypeForStreams",
                    Properties = new ODataProperty[] { testCase.NamedStreamProperty },
                    SerializationInfo = new ODataResourceSerializationInfo()
                    {
                        NavigationSourceEntityTypeName = "TestModel.EntityTypeForStreams",
                        ExpectedTypeName = "TestModel.EntityTypeForStreams",
                        NavigationSourceName = "MySet"
                    }
                };
                return new[]
                {
                    new PayloadWriterTestDescriptor<ODataItem>(
                        this.Settings,
                        entry,
                        testConfiguration => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings) { ExpectedException2 = testCase.ExpectedExceptionWithoutModel })
                        {
                            Model = null,
                        },
                    new PayloadWriterTestDescriptor<ODataItem>(
                        this.Settings,
                        entry,
                        testConfiguration => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings) { ExpectedException2 = testCase.ExpectedExceptionWithModel })
                        {
                            Model = model,
                            PayloadEdmElementContainer = entitySet,
                            PayloadEdmElementType = edmEntityType,
                        },
                };
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    if (testDescriptor.Model == null && testConfiguration.Format == ODataFormat.Json)
                    {
                        return;
                    }

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }
    }
}
