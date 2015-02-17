//---------------------------------------------------------------------
// <copyright file="PayloadKindDetectionAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for detecting payload kinds of ATOM payloads
    /// </summary>
    [TestClass, TestCase]
    public class PayloadKindDetectionAtomTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadKindDetectionTestDescriptor.Settings Settings { get; set; }

        [InjectDependency]
        public EntityModelSchemaToEdmModelConverter EntityModelSchemaToEdmModelConverter { get; set; }

        /// <summary>Reusable constant of an invalid local name.</summary>
        private const string invalidLocalName = "invalidLocalName";

        /// <summary>Reusable constant of an invalid namespace.</summary>
        private const string invalidNamespace = "http://odata.org/invalid";

        /// <summary>Reusable constant of an empty detection result.</summary>
        private static readonly IEnumerable<PayloadKindDetectionResult> emptyDetectionResult = Enumerable.Empty<PayloadKindDetectionResult>();

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Test the payload kind detection of ATOM payloads.")]
        public void PayloadKindDetectionAtomTest()
        {
            var testDescriptors = Enumerable.Empty<PayloadKindDetectionTestDescriptor>();

            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            IEdmTypeReference personType = model.FindType("TestModel.Person").ToTypeReference();

            string[] genericPayloadTemplates = new string[]
                {
                    "<{0} xmlns=\"{1}\" />",
                    "<!-- Comment --><?pi Ignore this?><{0} xmlns=\"{1}\" />",
                };

            {
                // atom:entry
                // NOTE: since feeds are not allowed in requests we decide based on the content type in requests (and ignore the payload)
                string[] entryContentTypes = new string[] { "application/atom+xml", "application/atom+xml;type=entry" };
                var entryPayloadTemplates = genericPayloadTemplates.ConcatSingle(
                        "<{0} xmlns=\"{1}\"><category term='TestModel.Person' scheme='http://docs.oasis-open.org/odata/ns/scheme'/></{0}>"
                    );
                var entryResult = new PayloadKindResult(ODataPayloadKind.Entry, /*expectedType*/personType, /*expectedException*/null);
                var entryInvalidResult = new PayloadKindResult(ODataPayloadKind.Entry, /*expectedType*/personType, ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_EntryElementWrongName", invalidLocalName, invalidNamespace));
                var entryInvalidLocalNameResult = new PayloadKindResult(ODataPayloadKind.Entry, /*expectedType*/personType, ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_EntryElementWrongName", invalidLocalName, TestAtomConstants.AtomNamespace));
                var entryInvalidNamespaceResult = new PayloadKindResult(ODataPayloadKind.Entry, /*expectedType*/personType, ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_EntryElementWrongName", TestAtomConstants.AtomEntryElementName, invalidNamespace));
                testDescriptors = testDescriptors.Concat(
                    entryContentTypes.SelectMany(ct =>
                        entryPayloadTemplates.SelectMany(pt =>
                            CreatePayloadKindDetectionTestCases(
                                ct,
                                pt,
                                TestAtomConstants.AtomEntryElementName,
                                TestAtomConstants.AtomNamespace,
                                model,
                                testConfig => CreateEnumerable(entryResult),
                                testConfig => testConfig.IsRequest ? CreateEnumerable(entryInvalidResult) : CreateEnumerable(),
                                testConfig => testConfig.IsRequest ? CreateEnumerable(entryInvalidLocalNameResult) : CreateEnumerable(),
                                testConfig => testConfig.IsRequest ? CreateEnumerable(entryInvalidNamespaceResult) : CreateEnumerable()))));
            }

            {
                // atom:feed
                // NOTE: since feeds are not allowed in requests we decide based on the content type in requests (and ignore the payload)
                var feedContentTypes = new string[] { "application/atom+xml", "application/atom+xml;type=feed" };
                var feedPayloadTemplates = genericPayloadTemplates.ConcatSingle(
                        "<{0} xmlns=\"{1}\"><entry><category term='TestModel.Person' scheme='http://docs.oasis-open.org/odata/ns/scheme' /></entry></{0}>"
                    );
                var feedResult = new PayloadKindResult(ODataPayloadKind.Feed, /*expectedType*/personType, /*expectedException*/null);
                var feedDetectedAsEntryResult = new PayloadKindResult(ODataPayloadKind.Entry, /*expectedType*/personType, ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_EntryElementWrongName", TestAtomConstants.AtomFeedElementName, TestAtomConstants.AtomNamespace));
                var feedDetectedAsEntryInvalidResult = new PayloadKindResult(ODataPayloadKind.Entry, /*expectedType*/personType, ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_EntryElementWrongName", invalidLocalName, invalidNamespace));
                var feedDetectedAsEntryInvalidLocalNameResult = new PayloadKindResult(ODataPayloadKind.Entry, /*expectedType*/personType, ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_EntryElementWrongName", invalidLocalName, TestAtomConstants.AtomNamespace));
                var feedDetectedAsEntryInvalidNamespaceResult = new PayloadKindResult(ODataPayloadKind.Entry, /*expectedType*/personType, ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_EntryElementWrongName", TestAtomConstants.AtomFeedElementName, invalidNamespace));
                testDescriptors = testDescriptors.Concat(
                    feedContentTypes.SelectMany(ct =>
                        feedPayloadTemplates.SelectMany(pt =>
                            CreatePayloadKindDetectionTestCases(
                                ct,
                                pt,
                                TestAtomConstants.AtomFeedElementName,
                                TestAtomConstants.AtomNamespace,
                                model,
                                testConfig => testConfig.IsRequest ? CreateEnumerable(feedDetectedAsEntryResult) : CreateEnumerable(feedResult),
                                testConfig => testConfig.IsRequest ? CreateEnumerable(feedDetectedAsEntryInvalidResult) : CreateEnumerable(),
                                testConfig => testConfig.IsRequest ? CreateEnumerable(feedDetectedAsEntryInvalidLocalNameResult) : CreateEnumerable(),
                                testConfig => testConfig.IsRequest ? CreateEnumerable(feedDetectedAsEntryInvalidNamespaceResult) : CreateEnumerable()))));
            }

            {
                // atompub:service
                var serviceDocContentTypes = new string[] { "application/xml" };
                var serviceDocPayloadTemplates = new string[] { "<{0} xmlns=\"{1}\"><workspace></workspace></{0}>" };
                var serviceDocResult = new PayloadKindResult(ODataPayloadKind.ServiceDocument, /*expectedType*/null, /*expectedException*/null);
                testDescriptors = testDescriptors.Concat(
                    serviceDocContentTypes.SelectMany(ct =>
                        serviceDocPayloadTemplates.SelectMany(pt =>
                            CreatePayloadKindDetectionTestCases(
                                ct,
                                pt,
                                TestAtomConstants.AtomPublishingServiceElementName,
                                TestAtomConstants.AtomPublishingNamespace,
                                model,
                                testConfig => testConfig.IsRequest ? CreateEnumerable() : CreateEnumerable(serviceDocResult),
                                testConfig => CreateEnumerable()))));

                // NOTE: for application/atomsvc+xml we always decide based on the content type.
                var serviceDocContentTypes2 = new string[] { "application/atomsvc+xml" };
                var serviceDocInvalidResult2 = new PayloadKindResult(ODataPayloadKind.ServiceDocument, /*expectedType*/null, ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentDeserializer_ServiceDocumentRootElementWrongNameOrNamespace", invalidLocalName, invalidNamespace));
                var serviceDocInvalidLocalNameResult2 = new PayloadKindResult(ODataPayloadKind.ServiceDocument, /*expectedType*/null, ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentDeserializer_ServiceDocumentRootElementWrongNameOrNamespace", invalidLocalName, TestAtomConstants.AtomPublishingNamespace));
                var serviceDocInvalidNamespaceResult2 = new PayloadKindResult(ODataPayloadKind.ServiceDocument, /*expectedType*/null, ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentDeserializer_ServiceDocumentRootElementWrongNameOrNamespace", TestAtomConstants.AtomPublishingServiceElementName, invalidNamespace));
                testDescriptors = testDescriptors.Concat(
                    serviceDocContentTypes2.SelectMany(ct =>
                        serviceDocPayloadTemplates.SelectMany(pt =>
                            CreatePayloadKindDetectionTestCases(
                                ct,
                                pt,
                                TestAtomConstants.AtomPublishingServiceElementName,
                                TestAtomConstants.AtomPublishingNamespace,
                                model,
                                testConfig => testConfig.IsRequest ? CreateEnumerable() : CreateEnumerable(serviceDocResult),
                                testConfig => testConfig.IsRequest ? CreateEnumerable() : CreateEnumerable(serviceDocInvalidResult2),
                                testConfig => testConfig.IsRequest ? CreateEnumerable() : CreateEnumerable(serviceDocInvalidLocalNameResult2),
                                testConfig => testConfig.IsRequest ? CreateEnumerable() : CreateEnumerable(serviceDocInvalidNamespaceResult2)))));
            }

            {
                // m:error
                var errorContentTypes = new string[] { "application/xml" };
                var errorPayloadTemplates = genericPayloadTemplates.ConcatSingle("<{0} xmlns=\"{1}\"><code>Some error code</code></{0}>");
                var errorResult = new PayloadKindResult(ODataPayloadKind.Error, /*expectedType*/null, /*expectedException*/null);
                testDescriptors = testDescriptors.Concat(
                    errorContentTypes.SelectMany(ct =>
                        errorPayloadTemplates.SelectMany(pt =>
                            CreatePayloadKindDetectionTestCases(
                                ct,
                                pt,
                                TestAtomConstants.ODataErrorElementName,
                                TestAtomConstants.ODataMetadataNamespace,
                                model,
                                testConfig => testConfig.IsRequest ? CreateEnumerable() : CreateEnumerable(errorResult),
                                testConfig => CreateEnumerable()))));
            }

            // collection and property test descriptors
            var collAndPropertyContentTypes = new string[] { "application/xml", "text/xml" };
            var collAndPropertyElementNames = new string[] { "Id", "element" };
            testDescriptors = testDescriptors.Concat(
                collAndPropertyContentTypes.SelectMany(ct =>
                    collAndPropertyElementNames.SelectMany(en =>
                        this.CreateCollectionAndPropertyTestDescriptors(model, en, ct))));

            // Add some manual test descriptors
            testDescriptors = testDescriptors.Concat(new PayloadKindDetectionTestDescriptor[]
            {
                // Non-Xml payload
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    ContentType = "application/xml",
                    PayloadString = "Some non-Xml payload",
                    ExpectedDetectionResults = testConfig => emptyDetectionResult,
                },

                // Invalid content type
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    ContentType = "application/invalid",
                    PayloadString = "Some non-Xml payload",
                    ExpectedDetectionResults = testConfig => emptyDetectionResult,
                },

                // Custom Xml payload
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    ContentType = "application/xml",
                    PayloadString = "<custom />",
                    ExpectedDetectionResults = testConfig => emptyDetectionResult,
                },
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                // NOTE: only using the configurations that disable stream disposal since we potentially read multiple
                //       times from the stream (to verify that we can read all the detected payload kinds).
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => tc.MessageReaderSettings.DisableMessageStreamDisposal),
                (testDescriptor, testConfiguration) => testDescriptor.RunTest(testConfiguration));
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Test the payload kind detection in WCF DS server mode.")]
        public void PayloadKindDetectionInServerBehaviorAtomTest()
        {
            var testDescriptors = new PayloadKindDetectionTestDescriptor[]
            {
                // Arbitrary payload string; in WCF DS server mode we should always throw.
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    ContentType = "application/atom+xml",
                    PayloadString = "<" + TestAtomConstants.AtomEntryElementName + " xmlns=\"" + TestAtomConstants.AtomNamespace + "\" />",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataMessageReader_PayloadKindDetectionInServerMode")
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) => testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(TestODataBehaviorKind.WcfDataServicesServer)));
        }

        /// <summary>
        /// Create test descriptors for collection and/or property payloads.
        /// </summary>
        /// <param name="model">The metadata model to use.</param>
        /// <param name="localName">The specified local name.</param>
        /// <param name="contentType">The specified content type.</param>
        /// <returns>The generated test descriptors.</returns>
        private IEnumerable<PayloadKindDetectionTestDescriptor> CreateCollectionAndPropertyTestDescriptors(
            IEdmModel model,
            string localName,
            string contentType)
        {
            var propertyResult = localName == "Id"
                ? new PayloadKindResult(ODataPayloadKind.Property, EdmCoreModel.Instance.GetInt32(false), /*expectedException*/ null)
                : new PayloadKindResult(ODataPayloadKind.Property, /*expectedType*/ null, /*expectedException*/ null);
            var invalidPropertyResult = new PayloadKindResult(ODataPayloadKind.Property, EdmCoreModel.Instance.GetInt32(false), ODataExpectedExceptions.ODataException("ReaderValidationUtils_CannotConvertPrimitiveValue", "", "Edm.Int32"));
            var collectionResult = new PayloadKindResult(ODataPayloadKind.Collection, /*expectedType*/ null, /*expectedException*/ null);
            var collectionAsPropertyInvalidResult = new PayloadKindResult(ODataPayloadKind.Property, EdmCoreModel.Instance.GetInt32(false), ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"));

            // empty <d:TestAtomConstants.ODataValueElementName /> element
            yield return new PayloadKindDetectionTestDescriptor(this.Settings)
            {
                ContentType = contentType,
                PayloadString = "<" + TestAtomConstants.ODataValueElementName + " xmlns=\"" + TestAtomConstants.ODataMetadataNamespace + "\" />",
                PayloadEdmModel = model,
                ReadDetectedPayloads = true,
                ExpectedDetectionResults = testConfig => CreateAtomFormatResult(model, invalidPropertyResult),
            };
            // <d:TestAtomConstants.ODataValueElementName /> element with empty content
            yield return new PayloadKindDetectionTestDescriptor(this.Settings)
            {
                ContentType = contentType,
                PayloadString = "<" + TestAtomConstants.ODataValueElementName + " xmlns=\"" + TestAtomConstants.ODataMetadataNamespace + "\"></" + TestAtomConstants.ODataValueElementName + ">",
                PayloadEdmModel = model,
                ReadDetectedPayloads = true,
                ExpectedDetectionResults = testConfig => CreateAtomFormatResult(model, invalidPropertyResult),
            };
            // <d:TestAtomConstants.ODataValueElementName /> element with text content
            yield return new PayloadKindDetectionTestDescriptor(this.Settings)
            {
                ContentType = contentType,
                PayloadString = "<" + TestAtomConstants.ODataValueElementName + " xmlns=\"" + TestAtomConstants.ODataMetadataNamespace + "\">1</" + TestAtomConstants.ODataValueElementName + ">",
                PayloadEdmModel = model,
                ReadDetectedPayloads = true,
                ExpectedDetectionResults = testConfig => CreateAtomFormatResult(model, propertyResult),
            };
            // single <m:element /> element
            yield return new PayloadKindDetectionTestDescriptor(this.Settings)
            {
                ContentType = contentType,
                PayloadString =
                    "<" + TestAtomConstants.ODataValueElementName + " xmlns=\"" + TestAtomConstants.ODataMetadataNamespace + "\">" +
                    "  <" + TestAtomConstants.ODataCollectionItemElementName + " />" +
                    "</" + TestAtomConstants.ODataValueElementName + ">",
                PayloadEdmModel = model,
                ReadDetectedPayloads = true,
                ExpectedDetectionResults = testConfig => testConfig.IsRequest
                    ? CreateAtomFormatResult(model, collectionAsPropertyInvalidResult)
                    : CreateAtomFormatResult(model, collectionAsPropertyInvalidResult, collectionResult),
            };
            // single <m:element /> element with empty content
            yield return new PayloadKindDetectionTestDescriptor(this.Settings)
            {
                ContentType = contentType,
                PayloadString = ("<" + TestAtomConstants.ODataValueElementName + " xmlns=\"" + TestAtomConstants.ODataMetadataNamespace + "\">" +
                    "  <" + TestAtomConstants.ODataCollectionItemElementName + "></" + TestAtomConstants.ODataCollectionItemElementName + ">" +
                    "</" + TestAtomConstants.ODataValueElementName + ">"),
                PayloadEdmModel = model,
                ReadDetectedPayloads = true,
                ExpectedDetectionResults = testConfig => testConfig.IsRequest
                    ? CreateAtomFormatResult(model, collectionAsPropertyInvalidResult)
                    : CreateAtomFormatResult(model, collectionAsPropertyInvalidResult, collectionResult),
            };
            // single <m:element /> element with text content
            yield return new PayloadKindDetectionTestDescriptor(this.Settings)
            {
                ContentType = contentType,
                PayloadString =
                    "<" + TestAtomConstants.ODataValueElementName + " xmlns=\"" + TestAtomConstants.ODataMetadataNamespace + "\">" +
                    "  <" + TestAtomConstants.ODataCollectionItemElementName + ">1</" + TestAtomConstants.ODataCollectionItemElementName + ">" +
                    "</" + TestAtomConstants.ODataValueElementName + ">",
                PayloadEdmModel = model,
                ReadDetectedPayloads = true,
                ExpectedDetectionResults = testConfig => testConfig.IsRequest
                    ? CreateAtomFormatResult(model, collectionAsPropertyInvalidResult)
                    : CreateAtomFormatResult(model, collectionAsPropertyInvalidResult, collectionResult),
            };
            // multiple <m:element /> elements
            yield return new PayloadKindDetectionTestDescriptor(this.Settings)
            {
                ContentType = contentType,
                PayloadString =
                    "<" + TestAtomConstants.ODataValueElementName + " xmlns=\"" + TestAtomConstants.ODataMetadataNamespace + "\">" +
                    "  <" + TestAtomConstants.ODataCollectionItemElementName + " />" +
                    "  <" + TestAtomConstants.ODataCollectionItemElementName + " />" +
                    "  <" + TestAtomConstants.ODataCollectionItemElementName + " />" +
                "</" + TestAtomConstants.ODataValueElementName + ">",
                PayloadEdmModel = model,
                ReadDetectedPayloads = true,
                ExpectedDetectionResults = testConfig => testConfig.IsRequest
                    ? CreateAtomFormatResult(model, collectionAsPropertyInvalidResult)
                    : CreateAtomFormatResult(model, collectionAsPropertyInvalidResult, collectionResult),
            };
            // <d:localname /> and <d:element />
            yield return new PayloadKindDetectionTestDescriptor(this.Settings)
            {
                ContentType = contentType,
                PayloadString =
                    "<" + TestAtomConstants.ODataValueElementName + " xmlns=\"" + TestAtomConstants.ODataMetadataNamespace + "\" xmlns:d=\"" + TestAtomConstants.ODataNamespace + "\">" +
                    "  <" + (localName != TestAtomConstants.ODataCollectionItemElementName ? "d:" : "") + localName + " />" +
                    "  <" + TestAtomConstants.ODataCollectionItemElementName + ">1</" + TestAtomConstants.ODataCollectionItemElementName + ">" +
                    "</" + TestAtomConstants.ODataValueElementName + ">",
                PayloadEdmModel = model,
                ReadDetectedPayloads = true,
                ExpectedDetectionResults = testConfig =>
                    localName == TestAtomConstants.ODataCollectionItemElementName && !testConfig.IsRequest
                        ? CreateAtomFormatResult(model, collectionAsPropertyInvalidResult, collectionResult)
                        : CreateAtomFormatResult(model, collectionAsPropertyInvalidResult),
            };
            // <d:element />, <d:TestAtomConstants.ODataValueElementName /> and <d:element />
            yield return new PayloadKindDetectionTestDescriptor(this.Settings)
            {
                ContentType = contentType,
                PayloadString =
                    "<" + TestAtomConstants.ODataValueElementName + " xmlns=\"" + TestAtomConstants.ODataMetadataNamespace + "\" xmlns:d=\"" + TestAtomConstants.ODataNamespace + "\">" +
                    "  <" + TestAtomConstants.ODataCollectionItemElementName + ">1</" + TestAtomConstants.ODataCollectionItemElementName + ">" +
                    "  <" + (localName != TestAtomConstants.ODataCollectionItemElementName ? "d:" : "") + localName + " />" +
                    "  <" + TestAtomConstants.ODataCollectionItemElementName + ">1</" + TestAtomConstants.ODataCollectionItemElementName + ">" +
                    "</" + TestAtomConstants.ODataValueElementName + ">",
                PayloadEdmModel = model,
                ReadDetectedPayloads = true,
                ExpectedDetectionResults = testConfig =>
                    localName == TestAtomConstants.ODataCollectionItemElementName && !testConfig.IsRequest
                        ? CreateAtomFormatResult(model, collectionAsPropertyInvalidResult, collectionResult)
                        : CreateAtomFormatResult(model, collectionAsPropertyInvalidResult),
            };
            // <m:element />, <m:element /> and <m:TestAtomConstants.ODataValueElementName />
            yield return new PayloadKindDetectionTestDescriptor(this.Settings)
            {
                ContentType = contentType,
                PayloadString =
                    "<" + TestAtomConstants.ODataValueElementName + " xmlns=\"" + TestAtomConstants.ODataMetadataNamespace + "\" xmlns:d=\"" + TestAtomConstants.ODataNamespace + "\">" +
                    "  <" + TestAtomConstants.ODataCollectionItemElementName + ">1</" + TestAtomConstants.ODataCollectionItemElementName + ">" +
                    "  <" + TestAtomConstants.ODataCollectionItemElementName + ">1</" + TestAtomConstants.ODataCollectionItemElementName + ">" +
                    "  <" + (localName != TestAtomConstants.ODataCollectionItemElementName ? "d:" : "") + localName + " />" +
                    "</" + TestAtomConstants.ODataValueElementName + ">",
                PayloadEdmModel = model,
                ReadDetectedPayloads = true,
                ExpectedDetectionResults = testConfig =>
                    localName == TestAtomConstants.ODataCollectionItemElementName && !testConfig.IsRequest
                        ? CreateAtomFormatResult(model, collectionAsPropertyInvalidResult, collectionResult)
                        : CreateAtomFormatResult(model, collectionAsPropertyInvalidResult),
            };
            // <d:localname m:type="Edm.Int32"/> element with m:type of Edm.Int32
            yield return new PayloadKindDetectionTestDescriptor(this.Settings)
            {
                ContentType = contentType,
                PayloadString = "<" + TestAtomConstants.ODataValueElementName + " m:type='Edm.Int32' xmlns=\"" + TestAtomConstants.ODataMetadataNamespace + "\" xmlns:m=\"" + TestAtomConstants.ODataMetadataNamespace + "\" />",
                PayloadEdmModel = model,
                ReadDetectedPayloads = true,
                ExpectedDetectionResults = testConfig => CreateAtomFormatResult(model, invalidPropertyResult),
            };
            // <d:localname m:type="TestModel.SomeComplexType"/> element with m:type of TestModel.SomeComplexType
            var invalidTypeKindPropertyResult = new PayloadKindResult(ODataPayloadKind.Property, EdmCoreModel.Instance.GetInt32(false), ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "TestModel.SomeComplexType", "Primitive", "Complex"));
            yield return new PayloadKindDetectionTestDescriptor(this.Settings)
            {
                ContentType = contentType,
                PayloadString = "<" + TestAtomConstants.ODataValueElementName + " m:type='TestModel.SomeComplexType' xmlns=\"" + TestAtomConstants.ODataMetadataNamespace + "\" xmlns:m=\"" + TestAtomConstants.ODataMetadataNamespace + "\" />",
                PayloadEdmModel = model,
                ReadDetectedPayloads = true,
                ExpectedDetectionResults = testConfig => CreateAtomFormatResult(model, invalidTypeKindPropertyResult),
            };
            // <d:localname m:type="Collection(Edm.Int32)"/> element with m:type of Collection(Edm.Int32)
            // Will be recognized as property just because it has the m:type
            var invalidCollectionTypeKindPropertyResult = new PayloadKindResult(ODataPayloadKind.Property, EdmCoreModel.Instance.GetInt32(false), ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "Collection(Edm.Int32)", "Primitive", "Collection"));
            yield return new PayloadKindDetectionTestDescriptor(this.Settings)
            {
                ContentType = contentType,
                PayloadString = "<" + TestAtomConstants.ODataValueElementName + " m:type='Collection(Edm.Int32)' xmlns=\"" + TestAtomConstants.ODataMetadataNamespace + "\" xmlns:m=\"" + TestAtomConstants.ODataMetadataNamespace + "\" />",
                PayloadEdmModel = model,
                ReadDetectedPayloads = true,
                ExpectedDetectionResults = testConfig => testConfig.IsRequest
                    ? CreateAtomFormatResult(model, invalidCollectionTypeKindPropertyResult)
                    : CreateAtomFormatResult(model, invalidCollectionTypeKindPropertyResult),
                SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
            };
            // <d:localname m:type="Collection(TestModel.SomeComplexType)"/> element with m:type of Collection(TestModel.SomeComplexType)
            // Will be recognized as property just because it has the m:type
            var invalidCollectionTypeKindPropertyResult2 = new PayloadKindResult(ODataPayloadKind.Property, EdmCoreModel.Instance.GetInt32(false), ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "Collection(TestModel.SomeComplexType)", "Primitive", "Collection"));
            yield return new PayloadKindDetectionTestDescriptor(this.Settings)
            {
                ContentType = contentType,
                PayloadString = "<" + TestAtomConstants.ODataValueElementName + " m:type='Collection(TestModel.SomeComplexType)' xmlns=\"" + TestAtomConstants.ODataMetadataNamespace + "\" xmlns:m=\"" + TestAtomConstants.ODataMetadataNamespace + "\" />",
                PayloadEdmModel = model,
                ReadDetectedPayloads = true,
                ExpectedDetectionResults = testConfig => testConfig.IsRequest
                    ? CreateAtomFormatResult(model, invalidCollectionTypeKindPropertyResult2)
                    : CreateAtomFormatResult(model, invalidCollectionTypeKindPropertyResult2),
                SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
            };
        }

        /// <summary>
        /// Creates a set of test cases based on a given namespace, local name and content type.
        /// </summary>
        /// <param name="contentType">The specified content type.</param>
        /// <param name="payloadTemplate">The string template for the payload.</param>
        /// <param name="localName">The specified local name.</param>
        /// <param name="ns">The specified namespace.</param>
        /// <param name="model">The metadata model to use.</param>
        /// <param name="validResultFunc">Func to produce the expected valid result.</param>
        /// <param name="invalidResultFunc">Func to produce the expected invalid result.</param>
        /// <param name="invalidLocalNameFunc">Func to produce the expected valid result for invalid local name; null if the same as invalidResultFunc.</param>
        /// <param name="invalidNamespaceResultFunc">Func to produce the expected valid result for invalid namespace; null if the same as invalidResultFunc.</param>
        /// <returns>The generated test descriptors.</returns>
        private IEnumerable<PayloadKindDetectionTestDescriptor> CreatePayloadKindDetectionTestCases(
            string contentType,
            string payloadTemplate,
            string localName,
            string ns,
            IEdmModel model,
            Func<ReaderTestConfiguration, IEnumerable<PayloadKindResult>> validResultFunc,
            Func<ReaderTestConfiguration, IEnumerable<PayloadKindResult>> invalidResultFunc,
            Func<ReaderTestConfiguration, IEnumerable<PayloadKindResult>> invalidLocalNameFunc = null,
            Func<ReaderTestConfiguration, IEnumerable<PayloadKindResult>> invalidNamespaceResultFunc = null)
        {
            Debug.Assert(validResultFunc != null, "validResultFunc != null");
            Debug.Assert(invalidResultFunc != null, "invalidResultFunc != null");

            // Correct namespace and local name
            string payloadString = string.Format(payloadTemplate, localName, ns);
            yield return new PayloadKindDetectionTestDescriptor(this.Settings)
            {
                DebugDescription = payloadString + " payload",
                ContentType = contentType,
                PayloadString = payloadString,
                PayloadEdmModel = model,
                ReadDetectedPayloads = true,
                ExpectedDetectionResults = testConfig => CreateAtomFormatResult(model, validResultFunc(testConfig))
            };
            // Correct namespace and incorrect local name
            payloadString = string.Format(payloadTemplate, invalidLocalName, ns);
            yield return new PayloadKindDetectionTestDescriptor(this.Settings)
            {
                DebugDescription = payloadString + " payload",
                ContentType = contentType,
                PayloadString = payloadString,
                PayloadEdmModel = model,
                ReadDetectedPayloads = true,
                ExpectedDetectionResults = testConfig => CreateAtomFormatResult(model, invalidLocalNameFunc == null ? invalidResultFunc(testConfig) : invalidLocalNameFunc(testConfig)),
            };
            // Incorrect namespace and correct local name
            payloadString = string.Format(payloadTemplate, localName, invalidNamespace);
            yield return new PayloadKindDetectionTestDescriptor(this.Settings)
            {
                DebugDescription = payloadString + " payload",
                ContentType = contentType,
                PayloadString = payloadString,
                PayloadEdmModel = model,
                ReadDetectedPayloads = true,
                ExpectedDetectionResults = testConfig => CreateAtomFormatResult(model, invalidNamespaceResultFunc == null ? invalidResultFunc(testConfig) : invalidNamespaceResultFunc(testConfig)),
            };
            // Incorrect namespace and incorrect local name
            payloadString = string.Format(payloadTemplate, invalidLocalName, invalidNamespace);
            yield return new PayloadKindDetectionTestDescriptor(this.Settings)
            {
                DebugDescription = payloadString + " payload",
                ContentType = contentType,
                PayloadString = payloadString,
                PayloadEdmModel = model,
                ReadDetectedPayloads = true,
                ExpectedDetectionResults = testConfig => CreateAtomFormatResult(model, invalidResultFunc(testConfig)),
            };
        }

        /// <summary>
        /// Creates the detection result for the specified payload kinds in the ATOM format.
        /// </summary>
        /// <param name="model">The metadata model to use.</param>
        /// <param name="payloadKinds">The detected payload kind results.</param>
        /// <returns>The detection result for the specified payload kinds in the ATOM format.</returns>
        private static IEnumerable<PayloadKindDetectionResult> CreateAtomFormatResult(
            IEdmModel model,
            params PayloadKindResult[] payloadKinds)
        {
            return CreateAtomFormatResult(model, (IEnumerable<PayloadKindResult>)payloadKinds);
        }

        /// <summary>
        /// Creates the detection result for the specified payload kinds in the ATOM format.
        /// </summary>
        /// <param name="model">The metadata model to use.</param>
        /// <param name="payloadKindResults">The detected payload kind results.</param>
        /// <returns>The detection result for the specified payload kinds in the ATOM format.</returns>
        private static IEnumerable<PayloadKindDetectionResult> CreateAtomFormatResult(
            IEdmModel model,
            IEnumerable<PayloadKindResult> payloadKindResults)
        {
            return payloadKindResults.Select(pkr => new PayloadKindDetectionResult(pkr.PayloadKind, ODataFormat.Atom)
                {
                    Model = model,
                    ExpectedType = pkr.ExpectedType,
                    ExpectedException = pkr.ExpectedException,
                });
        }

        /// <summary>
        /// Helper method to create an enumerable of items.
        /// </summary>
        /// <param name="items">The items to create the enumerable for or null if no items are available.</param>
        /// <returns>An enumerable of the <paramref name="items"/>.</returns>
        private static IEnumerable<PayloadKindResult> CreateEnumerable(params PayloadKindResult[] items)
        {
            return items ?? Enumerable.Empty<PayloadKindResult>();
        }

        private sealed class PayloadKindDetectionTestCase
        {
            public string DebugDescription { get; set; }
            public string PayloadString { get; set; }
            public Func<ReaderTestConfiguration, IEnumerable<PayloadKindDetectionResult>> ExpectedDetectionResults { get; set; }
        }

        private sealed class PayloadKindResult
        {
            public PayloadKindResult(ODataPayloadKind payloadKind, IEdmTypeReference expectedType, ExpectedException expectedException)
            {
                this.PayloadKind = payloadKind;
                this.ExpectedType = expectedType;
                this.ExpectedException = expectedException;
            }

            public ODataPayloadKind PayloadKind { get; set; }
            public IEdmTypeReference ExpectedType { get; set; }
            public ExpectedException ExpectedException { get; set; }
        }
    }
}
