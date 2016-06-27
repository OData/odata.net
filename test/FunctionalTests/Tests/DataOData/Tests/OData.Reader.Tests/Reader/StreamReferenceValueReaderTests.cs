//---------------------------------------------------------------------
// <copyright file="StreamReferenceValueReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of various complex value payloads.
    /// </summary>
    [TestClass, TestCase]
    public class StreamReferenceValueReaderTests : ODataReaderTestCase
    {
        private PayloadReaderTestDescriptor.Settings jsonLightSettings;

        [InjectDependency]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings JsonLightSettings
        {
            get { return this.jsonLightSettings; }
            set { this.jsonLightSettings = value; this.jsonLightSettings.ExpectedResultSettings.ObjectModelToPayloadElementConverter = new JsonLightObjectModelToPayloadElementConverter(); }
        }

        public static IEnumerable<PayloadReaderTestDescriptor> CreateStreamPropertyMetadataTestDescriptors(PayloadReaderTestDescriptor.Settings settings)
        {
            // Get the standard stream property payloads
            IEnumerable<PayloadReaderTestDescriptor> streamPropertyTestDescriptors = PayloadReaderTestDescriptorGenerator.CreateStreamReferenceValueTestDescriptors(settings, withMetadata: true);

            // Add some reader specific payloads
            EdmModel model = new EdmModel().Fixup();

            streamPropertyTestDescriptors = streamPropertyTestDescriptors.Concat(new PayloadReaderTestDescriptor[]
            {
                // Just read link - valid for readers
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.StreamProperty("StreamProperty", readLink: "http://odata.org/mr"),
                    PayloadEdmModel = model,
                },
                // Just content type
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.StreamProperty("StreamProperty", contentType: "mime/type"),
                    PayloadEdmModel = model,
                    // Doesn't work for ATOM as ATOM needs the self link to put the content type on
                    SkipTestConfiguration = tc => false
                },
                // Read link and content type
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.StreamProperty("StreamProperty", readLink: "http://odata.org/mr", contentType: "mime/type"),
                    PayloadEdmModel = model,
                },
                // Just edit link - valid for readers
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.StreamProperty("StreamProperty", editLink: "http://odata.org/mr"),
                    PayloadEdmModel = model,
                },
                // Just etag
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.StreamProperty("StreamProperty", etag: "etag"),
                    PayloadEdmModel = model,
                    // Doesn't work for ATOM as ATOM needs the edit link to put the etag on
                    SkipTestConfiguration = tc => false
                },
                // Just edit link and etag - valid for readers
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.StreamProperty("StreamProperty", editLink: "http://odata.org/mr", etag: "etag"),
                    PayloadEdmModel = model,
                },
                // All properties
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.StreamProperty("StreamProperty", readLink: "http://odata.org/mrread", contentType: "mime/type", editLink: "http://odata.org/mr", etag: "etag"),
                    PayloadEdmModel = model,
                },

                // TODO: Add more tests around values for the stream properties. Readers won't validate anything, so empty values are acceptable
            });

            return streamPropertyTestDescriptors;
        }

        [TestMethod, TestCategory("Reader.Streams"), Variation(Description = "Verifies correct reading of stream properties (stream reference values) with fully specified metadata in JSON Light.")]
        public void StreamPropertyWithMetadataJsonLightTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors
                = CreateStreamPropertyMetadataTestDescriptors(this.JsonLightSettings).SelectMany(td => this.PayloadGenerator.GenerateReaderPayloads(td));

            // NOTE: manual JSON tests and error tests are part of the JSON specific StreamPropertyWithMetadataJsonTests test case
            // NOTE: manual ATOM tests and error tests are part of the ATOM specific StreamPropertyWithMetadataAtomTests test case
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                // No stream properties in requests or <V3 payloads
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Streams"), Variation(Description = "Verifies correct reading of stream properties (stream reference values) with regard to UndeclaredPropertyBehaviorKinds in JSON Light.")]
        public void UndeclaredPropertyBehaviorKindStreamPropertyJsonLightTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                new[] { false, true },
                throwOnUndeclaredPropertyForNonOpenType =>
                {
                    var testDescriptors = CreateUndeclaredPropertyBehaviorKindStreamPropertyTestDescriptors(throwOnUndeclaredPropertyForNonOpenType, this.JsonLightSettings);
                    this.CombinatorialEngineProvider.RunCombinations(
                        testDescriptors,
                        this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                        (testDescriptor, testConfiguration) =>
                        {
                            testConfiguration = new ReaderTestConfiguration(testConfiguration);
                            if (!throwOnUndeclaredPropertyForNonOpenType)
                            {
                                testConfiguration.MessageReaderSettings.Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
                            }

                            testDescriptor.RunTest(testConfiguration);
                        });
                });
        }

        private static IEnumerable<PayloadReaderTestDescriptor> CreateUndeclaredPropertyBehaviorKindStreamPropertyTestDescriptors(bool throwOnUndeclaredPropertyForNonOpenType, PayloadReaderTestDescriptor.Settings settings)
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                // Undeclared stream property with read-link only.
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("UndeclaredProperty", "http://odata.org/readlink"),
                    PayloadEdmModel = model,
                    ExpectedException = throwOnUndeclaredPropertyForNonOpenType
                                            ? ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", "UndeclaredProperty", "TestModel.CityType")
                                            : null
                },
                // Undeclared stream property with edit-link only.
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("UndeclaredProperty", null, "http://odata.org/editlink"),
                    PayloadEdmModel = model,
                    ExpectedException = throwOnUndeclaredPropertyForNonOpenType
                                            ? ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", "UndeclaredProperty", "TestModel.CityType")
                                            : null
                },
                // Undeclared stream property with all properties.
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("UndeclaredProperty", "http://odata.org/readlink", "http://odata.org/editlink", "stream/content", "stream:etag"),
                    PayloadEdmModel = model,
                    ExpectedException = throwOnUndeclaredPropertyForNonOpenType
                                            ? ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", "UndeclaredProperty", "TestModel.CityType")
                                            : null
                },
            };

            return testDescriptors;
        }
    }
}
