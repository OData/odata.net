//---------------------------------------------------------------------
// <copyright file="PayloadKindDetectionJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for detecting payload kinds of JSON Lite payloads.
    /// </summary>
    [TestClass, TestCase] 
    public class PayloadKindDetectionJsonLightTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadKindDetectionTestDescriptor.Settings Settings { get; set; }

        /// <summary>Reusable constant of an empty detection result.</summary>
        private static readonly Func<ReaderTestConfiguration, IEnumerable<PayloadKindDetectionResult>> emptyDetectionResult = 
            testConfig => Enumerable.Empty<PayloadKindDetectionResult>();

        /// <summary>Reusable constant of an entry detection result.</summary>
        private static readonly Func<ReaderTestConfiguration, IEnumerable<PayloadKindDetectionResult>> entryDetectionResult =
            testConfig => CreateJsonLightFormatResult(ODataPayloadKind.Resource);

        /// <summary>Reusable constant of a feed detection result.</summary>
        private static readonly Func<ReaderTestConfiguration, IEnumerable<PayloadKindDetectionResult>> feedDetectionResult =
            testConfig => CreateJsonLightFormatResult(ODataPayloadKind.ResourceSet);

        /// <summary>Reusable constant of a service doc detection result.</summary>
        private static readonly Func<ReaderTestConfiguration, IEnumerable<PayloadKindDetectionResult>> serviceDocDetectionResult =
            testConfig => CreateJsonLightFormatResult(ODataPayloadKind.ServiceDocument);

        /// <summary>Reusable constant of an entity reference link detection result.</summary>
        private static readonly Func<ReaderTestConfiguration, IEnumerable<PayloadKindDetectionResult>> entityReferenceLinkDetectionResult =
            testConfig => CreateJsonLightFormatResult(ODataPayloadKind.EntityReferenceLink);

        /// <summary>Reusable constant of an entity reference links detection result.</summary>
        private static readonly Func<ReaderTestConfiguration, IEnumerable<PayloadKindDetectionResult>> entityReferenceLinksDetectionResult =
            testConfig => CreateJsonLightFormatResult(ODataPayloadKind.EntityReferenceLinks);

        /// <summary>Reusable constant of a property detection result.</summary>
        private static readonly Func<ReaderTestConfiguration, IEnumerable<PayloadKindDetectionResult>> propertyDetectionResult =
            testConfig => CreateJsonLightFormatResult(ODataPayloadKind.Property);

        private static readonly Func<ReaderTestConfiguration, IEnumerable<PayloadKindDetectionResult>> complexPropertyDetectionResult =
            testConfig => CreateJsonLightFormatResult(ODataPayloadKind.Resource, ODataPayloadKind.Property);

        /// <summary>Reusable constant of a collection (and property) detection result.</summary>
        private static readonly Func<ReaderTestConfiguration, IEnumerable<PayloadKindDetectionResult>> collectionDetectionResult =
            testConfig => CreateJsonLightFormatResult(ODataPayloadKind.Property, ODataPayloadKind.Collection);

        /// <summary>Reusable constant of a collection (and property) detection result.</summary>
        private static readonly Func<ReaderTestConfiguration, IEnumerable<PayloadKindDetectionResult>> complexCollectionDetectionResult =
            testConfig => CreateJsonLightFormatResult(ODataPayloadKind.ResourceSet, ODataPayloadKind.Property, ODataPayloadKind.Collection);

        /// <summary>Reusable constant of an error detection result.</summary>
        private static readonly Func<ReaderTestConfiguration, IEnumerable<PayloadKindDetectionResult>> errorDetectionResult =
            testConfig => CreateJsonLightFormatResult(ODataPayloadKind.Error);

        private const string metadataDocumentUri = "http://odata.org/test/$metadata";

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the payload kind detection of JSON Lite responses.")]
        public void PayloadKindDetectionResponseJsonLightTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            
            var testDescriptors = Enumerable.Empty<PayloadKindDetectionTestDescriptor>();

            var testCases = new[]
                {
                    #region Entry context URI
                    new 
                    { 
                        Description = "Simple entry",
                        ContextUri = metadataDocumentUri + "#TestModel.DefaultContainer.Persons/$entity",
                        ExpectedDetectionResults = entryDetectionResult,
                    },
                    new 
                    { 
                        Description = "Metadata document URI for an entry of a derived type from the base type of the entity set",
                        ContextUri = metadataDocumentUri + "#TestModel.DefaultContainer.Persons/TestModel.Employee/$entity",
                        ExpectedDetectionResults = entryDetectionResult,
                    },
                    new 
                    { 
                        Description = "Metadata document URI for an entry with a type cast to the base type of the entity set",
                        ContextUri = metadataDocumentUri + "#TestModel.DefaultContainer.Persons/TestModel.Person/$entity",
                        ExpectedDetectionResults = entryDetectionResult,
                    },
                    new 
                    { 
                        Description = "Metadata document URI for an entry with an invalid entity container",
                        ContextUri = metadataDocumentUri + "#TestModel.NonExistingContainer.Persons/TestModel.Employee/$entity",
                        ExpectedDetectionResults = emptyDetectionResult,
                    },
                    new 
                    { 
                        Description = "Metadata document URI for an entry with a type cast to a non-existing entity type",
                        ContextUri = metadataDocumentUri + "#TestModel.DefaultContainer.Persons/TestModel.NonExistingType/$entity",
                        ExpectedDetectionResults = emptyDetectionResult,
                    },
                    new 
                    { 
                        Description = "Metadata document URI for an entry of a type incompatible with the base type of the entity set",
                        ContextUri = metadataDocumentUri + "#TestModel.DefaultContainer.Persons/TestModel.OfficeType/$entity",
                        ExpectedDetectionResults = emptyDetectionResult,
                    },
                    new 
                    { 
                        Description = "Metadata document URI for an entry with an incorrect $entity suffix",
                        ContextUri = metadataDocumentUri + "#TestModel.DefaultContainer.Persons/@WrongElement",
                        ExpectedDetectionResults = emptyDetectionResult,
                    },
                    new 
                    { 
                        Description = "Metadata document URI for an entry with a type cast and an incorrect $entity suffix",
                        ContextUri = metadataDocumentUri + "#TestModel.DefaultContainer.Persons/TestModel.Employee/@WrongElement",
                        ExpectedDetectionResults = emptyDetectionResult,
                    },
                    new 
                    { 
                        Description = "Metadata document URI for an entry (invalid entity set)",
                        ContextUri = metadataDocumentUri + "#TestModel.DefaultContainer.WrongSet/$entity",
                        ExpectedDetectionResults = emptyDetectionResult,
                    },
                    new 
                    { 
                        Description = "Metadata document URI for an entry with type cast (invalid entity set)",
                        ContextUri = metadataDocumentUri + "#TestModel.DefaultContainer.WrongSet/TestModel.Employee/$entity",
                        ExpectedDetectionResults = emptyDetectionResult,
                    },
                    #endregion Entry context URI
                    #region Feed context URI
                    new 
                    { 
                        Description = "Metadata document URI for a feed of the same type as the entity set",
                        ContextUri = metadataDocumentUri + "#TestModel.DefaultContainer.Persons",
                        ExpectedDetectionResults = feedDetectionResult,
                    },
                    new 
                    { 
                        Description = "Metadata document URI for a feed with a type cast",
                        ContextUri = metadataDocumentUri + "#TestModel.DefaultContainer.Persons/TestModel.Employee",
                        ExpectedDetectionResults = feedDetectionResult,
                    },
                    new 
                    { 
                        Description = "Metadata document URI for a feed (invalid entity set)",
                        ContextUri = metadataDocumentUri + "#TestModel.DefaultContainer.WrongSet",
                        ExpectedDetectionResults = emptyDetectionResult,
                    },
                    new 
                    { 
                        Description = "Metadata document URI for a feed with a type cast (invalid entity set)",
                        ContextUri = metadataDocumentUri + "#TestModel.DefaultContainer.WrongSet/TestModel.Employee",
                        ExpectedDetectionResults = emptyDetectionResult,
                    },
                    new 
                    { 
                        Description = "Metadata document URI for a feed with a type cast (invalid entity set)",
                        ContextUri = metadataDocumentUri + "#TestModel.DefaultContainer.WrongSet/TestModel.Employee",
                        ExpectedDetectionResults = emptyDetectionResult,
                    },
                    #endregion Feed context URI
                    #region Service doc context URI
                    new 
                    {
                        Description = "Metadata document URI without fragment",
                        ContextUri = metadataDocumentUri,
                        ExpectedDetectionResults = serviceDocDetectionResult,
                    },
                    new 
                    {
                        Description = "Metadata document URI with empty fragment",
                        ContextUri = metadataDocumentUri + "#",
                        ExpectedDetectionResults = serviceDocDetectionResult,
                    },
                    new 
                    {
                        Description = "Metadata document URI with non-empty fragment",
                        ContextUri = metadataDocumentUri + "#SomeValue",
                        ExpectedDetectionResults = emptyDetectionResult,
                    },
                    #endregion Service doc context URI
                    #region Property context URI
                    new 
                    {
                        Description = "Metadata document URI for primitive type",
                        ContextUri = metadataDocumentUri + "#Edm.String",
                        ExpectedDetectionResults = propertyDetectionResult,
                    },
                    new 
                    {
                        Description = "Metadata document URI for complex type",
                        ContextUri = metadataDocumentUri + "#TestModel.Address",
                        ExpectedDetectionResults = complexPropertyDetectionResult,
                    },
                    new 
                    {
                        Description = "Metadata document URI for primitive collection type",
                        ContextUri = metadataDocumentUri + "#Collection(Edm.String)",
                        ExpectedDetectionResults = collectionDetectionResult,
                    },
                    new 
                    {
                        Description = "Metadata document URI for complex collection type",
                        ContextUri = metadataDocumentUri + "#Collection(TestModel.Address)",
                        ExpectedDetectionResults = complexCollectionDetectionResult,
                    },
                    #endregion Property context URI
                    #region Collection context URI
                    new 
                    {
                        Description = "Metadata document URI for primitive collection type",
                        ContextUri = metadataDocumentUri + "#Collection(Edm.String)",
                        ExpectedDetectionResults = collectionDetectionResult,
                    },
                    new 
                    {
                        Description = "Metadata document URI for complex collection type",
                        ContextUri = metadataDocumentUri + "#Collection(TestModel.Address)",
                        ExpectedDetectionResults = complexCollectionDetectionResult,
                    },
                    #endregion Collection context URI
                };

            testDescriptors = testCases.Select(testCase =>
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    DebugDescription = testCase.Description,
                    ContentType = "application/json;odata.metadata=minimal",
                    PayloadEdmModel = model,
                    PayloadString = string.Format("{{ \"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\": \"{0}\" }}", testCase.ContextUri),
                    ExpectedDetectionResults = testCase.ExpectedDetectionResults,
                });

            #region Error payload test cases
            testDescriptors = testDescriptors.Concat(new PayloadKindDetectionTestDescriptor[]
            {
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    DebugDescription = "Error payload",
                    ContentType = "application/json;odata.metadata=minimal",
                    PayloadEdmModel = model,
                    PayloadString = "{\"" + JsonLightConstants.ODataErrorPropertyName + "\":{\"code\":\"error-code\",\"message\":\"error-message\"}}",
                    ExpectedDetectionResults = errorDetectionResult,
                },
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    DebugDescription = "Error payload with custom annotations before and after",
                    ContentType = "application/json;odata.metadata=minimal",
                    PayloadEdmModel = model,
                    PayloadString = "{\"@my.custom\":42,\"" + JsonLightConstants.ODataErrorPropertyName + "\":{\"code\":\"error-code\",\"message\":\"error-message\"},\"@my.custom2\":43}",
                    ExpectedDetectionResults = errorDetectionResult,
                },
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    DebugDescription = "error property with invalid value",
                    ContentType = "application/json;odata.metadata=minimal",
                    PayloadEdmModel = model,
                    PayloadString = "{\"" + JsonLightConstants.ODataErrorPropertyName + "\":{\"a\":\"b\"}}",
                    ExpectedDetectionResults = emptyDetectionResult,
                },
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    DebugDescription = "Other property before error property ",
                    ContentType = "application/json;odata.metadata=minimal",
                    PayloadEdmModel = model,
                    PayloadString = "{\"other\":42,\"" + JsonLightConstants.ODataErrorPropertyName + "\":{\"code\":\"error-code\",\"message\":\"error-message\"}}",
                    ExpectedDetectionResults = emptyDetectionResult,
                },
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    DebugDescription = "Other property after error property ",
                    ContentType = "application/json;odata.metadata=minimal",
                    PayloadEdmModel = model,
                    PayloadString = "{\"" + JsonLightConstants.ODataErrorPropertyName + "\":{\"code\":\"error-code\",\"message\":\"error-message\"},\"other\":42}",
                    ExpectedDetectionResults = emptyDetectionResult,
                },
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    DebugDescription = "Other odata.annotation before error property ",
                    ContentType = "application/json;odata.metadata=minimal",
                    PayloadEdmModel = model,
                    PayloadString = "{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataAnnotationNamespacePrefix + "other\":42,\"" + JsonLightConstants.ODataErrorPropertyName + "\":{\"code\":\"error-code\",\"message\":\"error-message\"}}",
                    ExpectedDetectionResults = emptyDetectionResult,
                },
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    DebugDescription = "Other odata.annotation after error property ",
                    ContentType = "application/json;odata.metadata=minimal",
                    PayloadEdmModel = model,
                    PayloadString = "{\"" + JsonLightConstants.ODataErrorPropertyName + "\":{\"code\":\"error-code\",\"message\":\"error-message\"},\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataAnnotationNamespacePrefix + "other\":42}",
                    ExpectedDetectionResults = emptyDetectionResult,
                },
            });
            #endregion Error payload test cases

            #region Miscellaneous tests
            testDescriptors = testDescriptors.Concat(new PayloadKindDetectionTestDescriptor[]
            {
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    DebugDescription = "Object with multiple (not well-known) properties",
                    ContentType = "application/json;odata.metadata=minimal",
                    PayloadEdmModel = model,
                    PayloadString = "{ \"a\": 42, \"b\": 43 }",
                    ExpectedDetectionResults = emptyDetectionResult,
                },
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    DebugDescription = "Object with a single (not well-known) property",
                    ContentType = "application/json;odata.metadata=minimal",
                    PayloadEdmModel = model,
                    PayloadString = "{ \"a\": 42 }",
                    ExpectedDetectionResults = emptyDetectionResult,
                },
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    DebugDescription = "No properties (and not context URI)",
                    ContentType = "application/json;odata.metadata=minimal",
                    PayloadEdmModel = model,
                    PayloadString = "{ }",
                    ExpectedDetectionResults = emptyDetectionResult,
                },
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    DebugDescription = "Non-Json payload",
                    ContentType = "application/json;odata.metadata=minimal",
                    PayloadEdmModel = model,
                    PayloadString = "Some non-Json payload",
                    ExpectedDetectionResults = emptyDetectionResult,
                },
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    DebugDescription = "Invalid content type",
                    ContentType = "application/invalid",
                    PayloadEdmModel = model,
                    PayloadString = "{ \"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata\" }",
                    ExpectedDetectionResults = emptyDetectionResult,
                },
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    DebugDescription = "Primitive value at the top level",
                    ContentType = "application/json;odata.metadata=minimal",
                    PayloadEdmModel = model,
                    PayloadString = "42",
                    ExpectedDetectionResults = emptyDetectionResult,
                },
            });
            #endregion Miscellaneous tests

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                // We do not support payload kind detection in requests
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) => testDescriptor.RunTest(testConfiguration));
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Test the payload kind detection of JSON Lite requests.")]
        public void PayloadKindDetectionRequestJsonLightTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            var testDescriptors = new PayloadKindDetectionTestDescriptor[]
            {
                new PayloadKindDetectionTestDescriptor(this.Settings)
                {
                    DebugDescription = "Simple entry; but since in request it will fail.",
                    ContentType = "application/json;odata.metadata=minimal",
                    PayloadEdmModel = model,
                    PayloadString = string.Format("{{ \"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\": \"{0}\" }}", metadataDocumentUri + "#TestModel.DefaultContainer.Persons/$entity"),
                    ExpectedDetectionResults = emptyDetectionResult,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightInputContext_PayloadKindDetectionForRequest")
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => tc.IsRequest),
                (testDescriptor, testConfiguration) => testDescriptor.RunTest(testConfiguration));
        }
            
        /// <summary>
        /// Creates the detection result for the specified payload kinds in the JSON format.
        /// </summary>
        /// <param name="payloadKinds">The detected payload kinds.</param>
        /// <returns>The detection result for the specified payload kinds in the JSON format.</returns>
        private static IEnumerable<PayloadKindDetectionResult> CreateJsonLightFormatResult(params ODataPayloadKind[] payloadKinds)
        {
            return payloadKinds.Select(pk => new PayloadKindDetectionResult(pk, ODataFormat.Json));
        }
    }
}
