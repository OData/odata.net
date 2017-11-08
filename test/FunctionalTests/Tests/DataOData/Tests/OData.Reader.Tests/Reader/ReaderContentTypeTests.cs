//---------------------------------------------------------------------
// <copyright file="ReaderContentTypeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.Platforms;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests parsing and usage of content types in the message reader.
    /// </summary>
    [TestClass, TestCase]
    public class ReaderContentTypeTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        private const string ApplicationJson = "application/json";
        private const string ApplicationJsonODataLight = "application/json;odata.metadata=minimal";
        private const string ApplicationJsonODataLightStreaming = "application/json;odata.metadata=minimal;odata.streaming=true";
        private const string ApplicationJsonODataLightNonStreaming = "application/json;odata.metadata=minimal;odata.streaming=false";

        [TestMethod, TestCategory("Reader.ContentType"), Variation(Description = "Verifies that we properly parse the content type header.")]
        public void ContentTypeHeaderParsingTest()
        {
            IEnumerable<ContentTypeTestCase> testCases = new ContentTypeTestCase[]
            {
                #region RawValue test cases
                new ContentTypeTestCase
                {
                    // only reading a raw value will succeed
                    ContentType = "text/plain",
                    ExpectedFormat = ODataFormat.RawValue,
                    ShouldSucceedForPayloadKind = pk =>
                        pk == ODataPayloadKind.Value
                        || pk == ODataPayloadKind.BinaryValue,
                },
                new ContentTypeTestCase
                {
                    // only reading a raw value or binary value will succeed; raw values can be read as binary values when the content type is application/octet-stream
                    ContentType = "application/octet-stream",
                    ExpectedFormat = ODataFormat.RawValue,
                    ShouldSucceedForPayloadKind = pk =>
                        pk == ODataPayloadKind.Value
                        || pk == ODataPayloadKind.BinaryValue,
                },
                new ContentTypeTestCase
                {
                    // only raw value / binary value will succeed
                    ContentType = "multipart/mixed",
                    ExpectedFormat = ODataFormat.RawValue,
                    ShouldSucceedForPayloadKind = pk => false,
                },
                new ContentTypeTestCase
                {
                    // Test for: MimeType allows 0x7F character, but ContentType parsing doesn't
                    ContentType = "application/"+0x7F,
                    ExpectedFormat = ODataFormat.RawValue,
                    ShouldSucceedForPayloadKind = pk => false,
                },

                #endregion RawValue test cases

                #region JSON Lite test cases
                new ContentTypeTestCase
                {
                    // only batch and raw value will fail (batch payload kind tested separately in BatchContentTypeHeaderParsingTest)
                    ContentType = ApplicationJsonODataLight,
                    ExpectedFormat = ODataFormat.Json,
                    ShouldSucceedForPayloadKind = pk =>
                        pk != ODataPayloadKind.Value
                        && pk != ODataPayloadKind.BinaryValue,
                },
                new ContentTypeTestCase
                {
                    // only batch and raw value will fail (batch payload kind tested separately in BatchContentTypeHeaderParsingTest)
                    ContentType = ApplicationJsonODataLightStreaming,
                    ExpectedFormat = ODataFormat.Json,
                    ShouldSucceedForPayloadKind = pk =>
                        pk != ODataPayloadKind.Value
                        && pk != ODataPayloadKind.BinaryValue,
                },
                new ContentTypeTestCase
                {
                    // only batch and raw value will fail (batch payload kind tested separately in BatchContentTypeHeaderParsingTest)
                    ContentType = ApplicationJsonODataLightNonStreaming,
                    ExpectedFormat = ODataFormat.Json,
                    ShouldSucceedForPayloadKind = pk =>
                        pk != ODataPayloadKind.Value
                        && pk != ODataPayloadKind.BinaryValue,
                },
                #endregion JSON Lite test cases

                #region Error test cases
                new ContentTypeTestCase
                {
                    // unsupported content type; everything will fail
                    ContentType = "application/foo",
                    ShouldSucceedForPayloadKind = pk => false,
                },
                new ContentTypeTestCase
                {
                    // unsupported content type with parameters; everything will fail
                    ContentType = "abc/pqr;a=b;c=d",
                    ShouldSucceedForPayloadKind = pk => false,
                },
                new ContentTypeTestCase
                {
                    // "image/jpeg" is not supported, even for raw values.
                    ContentType = "image/jpeg",
                    ShouldSucceedForPayloadKind = pk => false,
                },
                #endregion Error test cases

                #region Content Type is null or empty
                new ContentTypeTestCase
                {
                    // null content type and zero content length should be default to Json if the payload kind is not binary value or value.
                    ContentType = null,
                    ContentLength = 0,
                    ExpectedFormat = ODataFormat.Json,
                    ShouldSucceedForPayloadKind = pk => true,
                    ShouldIgnoreTest = pk => pk == ODataPayloadKind.BinaryValue || pk == ODataPayloadKind.Value
                },
                new ContentTypeTestCase
                {
                    // null content type and zero content length should be default to RawValue if the payload kind is binary value or value.
                    ContentType = null,
                    ContentLength = 0,
                    ExpectedFormat = ODataFormat.RawValue,
                    ShouldSucceedForPayloadKind = pk => true,
                    ShouldIgnoreTest = pk => pk != ODataPayloadKind.Value
                },
                #endregion
            };

            string[] parameters = new string[]
            {
                "foo=bar",
                "foo1=bar1;foo2=bar2"
            };

            testCases = testCases.Concat(testCases.Where(tc => tc.ContentType != null).SelectMany(tc => parameters.Select(p => new ContentTypeTestCase(tc) { ContentType = tc.ContentType + ";" + p })));

            int oDataPayloadKindCount = EnumExtensionMethods.GetValues<ODataPayloadKind>().Length;
            this.Assert.AreEqual(oDataPayloadKindCount, TestReaderUtils.ODataPayloadKinds.Length, "The number of payload kind have changed, please update this test.");

            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            // We don't support batch payloads here; we test those separately in BatchContentTypeHeaderParsingTest
            IEnumerable<ODataPayloadKind> payloadKinds =
                TestReaderUtils.ODataPayloadKinds.Where(k => k != ODataPayloadKind.Batch && k != ODataPayloadKind.Unsupported);
            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                payloadKinds,
                this.ReaderTestConfigurationProvider.AllFormatConfigurations,
                (testCase, payloadKind, testConfiguration) =>
                {
                    testConfiguration = new ReaderTestConfiguration(testConfiguration);
                    if (IgnoreTestCase(payloadKind, testConfiguration))
                    {
                        return;
                    }

                    if (testCase.ShouldIgnoreTest != null && testCase.ShouldIgnoreTest(payloadKind))
                    {
                        return;
                    }

                    string supportedMediaTypes;

                    if (payloadKind == ODataPayloadKind.Value || payloadKind == ODataPayloadKind.BinaryValue)
                    {
                        supportedMediaTypes = TestMediaTypeUtils.GetSupportedMediaTypes(ODataPayloadKind.Value) + ", " + TestMediaTypeUtils.GetSupportedMediaTypes(ODataPayloadKind.BinaryValue);
                    }
                    else
                    {
                        supportedMediaTypes = TestMediaTypeUtils.GetSupportedMediaTypes(payloadKind, /*includeAppJson*/true);
                    }

                    ExpectedException expectedException = testCase.ExpectedException == null
                         ? testCase.ShouldSucceedForPayloadKind != null && testCase.ShouldSucceedForPayloadKind(payloadKind)
                             ? null
                             : ODataExpectedExceptions.ODataContentTypeException("MediaTypeUtils_CannotDetermineFormatFromContentType", supportedMediaTypes, testCase.ContentType ?? "")
                         : testCase.ExpectedException;

                    // Make sure to run success test cases only in configurations that will work.
                    if (expectedException == null
                        && testConfiguration.Format != null
                        && testCase.ExpectedFormat != testConfiguration.Format)
                    {
                        return;
                    }

                    ODataPayloadElement payloadElement = CreatePayloadElement(model, payloadKind, testConfiguration);

                    // When we write a value with a content type different than 'text/plain', we will read it as binary.
                    // Likewise, when we write a binary value with a 'text/plain' content type, we will read it as a string.
                    Func<ReaderTestConfiguration, ODataPayloadElement> expectedResultElementFunc = null;
                    if (payloadKind == ODataPayloadKind.Value && testCase.ContentType != null && !testCase.ContentType.StartsWith("text/plain"))
                    {
                        expectedResultElementFunc = (testConfig) => ConvertToBinaryPayloadElement(payloadElement);
                    }
                    else if (payloadKind == ODataPayloadKind.BinaryValue && testCase.ContentType != null && testCase.ContentType.StartsWith("text/plain"))
                    {
                        expectedResultElementFunc = (testConfig) => ConvertToStringPayloadElement(payloadElement);
                    }

                    ODataFormat expectedFormat = testCase.ExpectedFormat;

                    ReaderContentTypeTestDescriptor testDescriptor = new ReaderContentTypeTestDescriptor(this.Settings)
                    {
                        PayloadElement = payloadElement,
                        ExpectedResultPayloadElement = expectedResultElementFunc,
                        PayloadEdmModel = model,
                        ExpectedFormat = expectedFormat,
                        ContentType = testCase.ContentType,
                        ExpectedException = expectedException
                    };

                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.ContentType"), Variation(Description = "Verifies that we properly fail when parsing invalid content type headers.")]
        public void ContentTypeHeaderParsingErrorTest()
        {
            var testCases = new ContentTypeTestCase[]
            {
                new ContentTypeTestCase
                {
                    ContentType = null,
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("ODataMessageReader_NoneOrEmptyContentTypeHeader")
                },
                new ContentTypeTestCase
                {
                    ContentType = string.Empty,
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("ODataMessageReader_NoneOrEmptyContentTypeHeader")
                },
                new ContentTypeTestCase
                {
                    ContentType = ";foo=bar",
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeRequiresSlash", ";foo=bar")
                },
                new ContentTypeTestCase
                {
                    ContentType = "application/*",
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("ODataMessageReader_WildcardInContentType", "application/*")
                },
                new ContentTypeTestCase
                {
                    ContentType = "*/*",
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("ODataMessageReader_WildcardInContentType", "*/*")
                },
                new ContentTypeTestCase
                {
                    ContentType = "application/json, application/xml",
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("MediaTypeUtils_NoOrMoreThanOneContentTypeSpecified", "application/json, application/xml")
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.ReaderTestConfigurationProvider.AllFormatConfigurations.Where(tc => tc.Format != ODataFormat.Json),
                (testCase, testConfiguration) =>
                {
                    // create a message reader and call GetFormat; this should fail with the expected error message
                    TestMessage testMessage = TestReaderUtils.CreateInputMessageFromStream(new TestStream(), testConfiguration);
                    testMessage.SetHeader(Microsoft.OData.ODataConstants.ContentTypeHeader, testCase.ContentType);
                    testMessage.SetHeader(Microsoft.OData.ODataConstants.ContentLengthHeader, testCase.ContentLength.ToString());

                    TestExceptionUtils.ExpectedException(
                        this.Assert,
                        () =>
                        {
                            using (ODataMessageReaderTestWrapper messageReader = TestReaderUtils.CreateMessageReader(testMessage, null, testConfiguration))
                            {
                                ODataFormat actualFormat = messageReader.DetectPayloadKind().Single().Format;
                            }
                        },
                        testCase.ExpectedException,
                        this.ExceptionVerifier);
                });
        }

        [TestMethod, TestCategory("Reader.ContentType"), Variation(Description = "Verifies that we properly parse batch content type headers.")]
        public void BatchContentTypeHeaderParsingTest()
        {
            IEnumerable<ContentTypeTestCase> testCases = new ContentTypeTestCase[]
            {
                // correct batch content type
                new ContentTypeTestCase
                {
                    ContentType = "multipart/mixed;boundary=--aa_bb_cc--",
                    ExpectedFormat = ODataFormat.Batch,
                },

                // correct batch content type -- JSON with parameters
                new ContentTypeTestCase
                {
                    ContentType = ApplicationJsonODataLight,
                    ExpectedFormat = ODataFormat.Json
                },

                // correct batch content type -- JSON
                new ContentTypeTestCase
                {
                    ContentType = ApplicationJson,
                    ExpectedFormat = ODataFormat.Json
                },

                // missing batch boundary
                new ContentTypeTestCase
                {
                    ContentType = "multipart/mixed",
                    ExpectedFormat = ODataFormat.Batch,
                    ExpectedException = ODataExpectedExceptions.ODataException("MediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads", "multipart/mixed", "boundary")
                },

                // multiple batch boundary parameters
                new ContentTypeTestCase
                {
                    ContentType = "multipart/mixed;boundary=boundary1;boundary=boundary2",
                    ExpectedFormat = ODataFormat.Batch,
                    ExpectedException = ODataExpectedExceptions.ODataException("MediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads", "multipart/mixed;boundary=boundary1;boundary=boundary2", "boundary")
                },

                // invalid batch content types
                new ContentTypeTestCase
                {
                    ContentType = "multipart/bar",
                    ExpectedFormat = ODataFormat.Batch,
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("MediaTypeUtils_CannotDetermineFormatFromContentType", TestMediaTypeUtils.GetSupportedMediaTypes(ODataPayloadKind.Batch), "multipart/bar")
                },
                new ContentTypeTestCase
                {
                    ContentType = "foo/mixed",
                    ExpectedFormat = ODataFormat.Batch,
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("MediaTypeUtils_CannotDetermineFormatFromContentType", TestMediaTypeUtils.GetSupportedMediaTypes(ODataPayloadKind.Batch), "foo/mixed")
                },
                new ContentTypeTestCase
                {
                    ContentType = "abc/pqr",
                    ExpectedFormat = ODataFormat.Batch,
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("MediaTypeUtils_CannotDetermineFormatFromContentType", TestMediaTypeUtils.GetSupportedMediaTypes(ODataPayloadKind.Batch), "abc/pqr")
                }
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.ReaderTestConfigurationProvider.DefaultFormatConfigurations,
                (testCase, testConfiguration) =>
                {
                    // create a message reader and call GetFormat; this should fail with the expected error message
                    TestMessage testMessage = TestReaderUtils.CreateInputMessageFromStream(new TestStream(), testConfiguration);
                    testMessage.SetHeader(Microsoft.OData.ODataConstants.ContentTypeHeader, testCase.ContentType);

                    TestExceptionUtils.ExpectedException(
                        this.Assert,
                        () =>
                        {
                            using (ODataMessageReaderTestWrapper messageReader = TestReaderUtils.CreateMessageReader(testMessage, null, testConfiguration))
                            {
                                messageReader.CreateODataBatchReader();
                                ODataFormat actualFormat = ODataUtils.GetReadFormat(messageReader.MessageReader);
                                this.Assert.AreEqual(testCase.ExpectedFormat, actualFormat, "Formats don't match.");
                            }
                        },
                        testCase.ExpectedException,
                        this.ExceptionVerifier);
                });
        }

        [TestMethod, TestCategory("Reader.ContentType"), Variation(Description = "Verifies that 'application/json' as a content type is interpretted correctly based on the version.")]
        public void ContentTypeHeaderAppJsonVersioningTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            this.CombinatorialEngineProvider.RunCombinations(
                new[] { ApplicationJson, ApplicationJsonODataLight },
                new[] { ODataPayloadKind.Resource, ODataPayloadKind.ResourceSet },
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (contentType, payloadKind, testConfiguration) =>
                {
                    ODataVersion version = testConfiguration.Version;

                    if (IgnoreTestConfiguration(contentType, testConfiguration))
                    {
                        return;
                    }

                    if (IgnoreTestCase(payloadKind, testConfiguration))
                    {
                        return;
                    }

                    ExpectedException expectedException = null;

                    ODataPayloadElement payloadElement = CreatePayloadElement(model, payloadKind, testConfiguration);
                    ReaderContentTypeTestDescriptor testDescriptor = new ReaderContentTypeTestDescriptor(this.Settings)
                    {
                        PayloadElement = payloadElement,
                        PayloadEdmModel = model,
                        ExpectedFormat = ComputeExpectedFormat(testConfiguration, contentType),
                        ContentType = contentType,
                        ExpectedException = expectedException,
                    };

                    testDescriptor.RunTest(testConfiguration);
                });
        }

        // TODO: add encoding tests

        /// <summary>
        /// Ignore some test configurations based on the content type since we need to serialize the payloads
        /// with a supported format for that content type.
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="testConfiguration"></param>
        /// <returns></returns>
        private static bool IgnoreTestConfiguration(string contentType, ReaderTestConfiguration testConfiguration)
        {
            if (testConfiguration.Format != ODataFormat.Json)
            {
                string formatName = testConfiguration.Format == null ? "null" : testConfiguration.Format.GetType().Name;
                throw new NotSupportedException("Unsupported format " + formatName + " for method IgnoreTestConfiguration.");
            }

            return false;
        }

        private static bool IgnoreTestCase(ODataPayloadKind payloadKind, ReaderTestConfiguration testConfiguration)
        {
            // TODO: enable unsupported payload kinds when implemented
            if (payloadKind == ODataPayloadKind.MetadataDocument)
            {
                return true;
            }

            // ignore some payloads in requests since they are not allowed
            if (testConfiguration.IsRequest)
            {
                if (payloadKind == ODataPayloadKind.ServiceDocument || payloadKind == ODataPayloadKind.Error || payloadKind == ODataPayloadKind.EntityReferenceLinks)
                {
                    // not allowed in requests
                    return true;
                }
            }
            else
            {
                if (payloadKind == ODataPayloadKind.Parameter)
                {
                    // not allowed in responses
                    return true;
                }
            }

            ODataFormat format = testConfiguration.Format;

            // ignore some payloads in some formats
            switch (payloadKind)
            {
                case ODataPayloadKind.ResourceSet:
                    return format == null;
                case ODataPayloadKind.Resource:
                    return format == null;
                case ODataPayloadKind.Property:
                    return format == null;
                case ODataPayloadKind.EntityReferenceLink:
                    return format == null;
                case ODataPayloadKind.EntityReferenceLinks:
                    return format == null;
                case ODataPayloadKind.Value:
                    return format != null;
                case ODataPayloadKind.BinaryValue:
                    return format != null;
                case ODataPayloadKind.Collection:
                    return format == null;
                case ODataPayloadKind.ServiceDocument:
                    return format == null;
                case ODataPayloadKind.MetadataDocument:
                    return format != null;
                case ODataPayloadKind.Error:
                    return format == null;
                case ODataPayloadKind.Batch:
                    return format != null;
                case ODataPayloadKind.Parameter:
                    return format != ODataFormat.Json;
                case ODataPayloadKind.Unsupported:  // fall through
                default:
                    return true;
            }
        }

        private static ODataPayloadElement CreatePayloadElement(IEdmModel model, ODataPayloadKind payloadKind, ReaderTestConfiguration testConfig)
        {
            IEdmEntitySet citySet = model.EntityContainer.FindEntitySet("Cities");
            IEdmEntityType cityType = model.EntityTypes().Single(e => e.Name == "CityType");
            IEdmProperty cityNameProperty = cityType.Properties().Single(e => e.Name == "Name");
            IEdmNavigationProperty policeStationNavProp = cityType.NavigationProperties().Single(e => e.Name == "PoliceStation");
            IEdmOperationImport primitiveCollectionResultOperation = model.EntityContainer.FindOperationImports("PrimitiveCollectionResultOperation").Single();
            IEdmOperationImport serviceOp1 = model.EntityContainer.FindOperationImports("ServiceOperation1").Single();

            bool isRequest = testConfig.IsRequest;
            bool isJsonLightRequest = isRequest && testConfig.Format == ODataFormat.Json;
            switch (payloadKind)
            {
                case ODataPayloadKind.ResourceSet:
                    {
                        return PayloadBuilder.EntitySet().WithTypeAnnotation(cityType).ExpectedEntityType(cityType, citySet);
                    }
                case ODataPayloadKind.Resource:
                    {
                        return PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).WithTypeAnnotation(cityType).ExpectedEntityType(cityType, citySet);
                    }
                case ODataPayloadKind.Property:
                    return PayloadBuilder.PrimitiveProperty(isJsonLightRequest ? string.Empty : null, "SomeCityValue").ExpectedProperty(cityType, "Name");
                case ODataPayloadKind.EntityReferenceLink:
                    return PayloadBuilder.DeferredLink("http://odata.org/entityreferencelink").ExpectedNavigationProperty(citySet, cityType, "PoliceStation");

                case ODataPayloadKind.EntityReferenceLinks:
                    return PayloadBuilder.LinkCollection().Item(PayloadBuilder.DeferredLink("http://odata.org/entityreferencelink")).ExpectedNavigationProperty((EdmEntitySet)citySet, (EdmEntityType)cityType, "CityHall");

                case ODataPayloadKind.Value:
                    return PayloadBuilder.PrimitiveValue("PrimitiveValue");
                case ODataPayloadKind.BinaryValue:
                    return PayloadBuilder.PrimitiveValue(new byte[] { 0, 0, 1, 1 });
                case ODataPayloadKind.Collection:
                    return PayloadBuilder.PrimitiveCollection().CollectionName(null).ExpectedFunctionImport((EdmOperationImport)primitiveCollectionResultOperation);

                case ODataPayloadKind.ServiceDocument:
                    Debug.Assert(!isRequest, "Not supported in requests.");
                    return new ServiceDocumentInstance().Workspace(PayloadBuilder.Workspace());

                case ODataPayloadKind.MetadataDocument:
                    Debug.Assert(!isRequest, "Not supported in requests.");
                    throw new NotImplementedException();
                case ODataPayloadKind.Error:
                    Debug.Assert(!isRequest, "Not supported in requests.");
                    return PayloadBuilder.Error("ErrorCode");

                case ODataPayloadKind.Parameter:
                    // build parameter payload based on model definition
                    var parameterPayload = new ComplexInstance(null, false);
                    ODataPayloadElement a = PayloadBuilder.PrimitiveValue(123).WithTypeAnnotation(EdmCoreModel.Instance.GetInt32(false));
                    ODataPayloadElement b = PayloadBuilder.PrimitiveValue("stringvalue").WithTypeAnnotation(EdmCoreModel.Instance.GetString(false));
                    PrimitiveProperty parametera = new PrimitiveProperty("a", "Edm.Integer", ((PrimitiveValue)a).ClrValue);
                    PrimitiveProperty parameterb = new PrimitiveProperty("b", "Edm.String", ((PrimitiveValue)b).ClrValue);
                    parameterPayload.Add(parametera);
                    parameterPayload.Add(parameterb);
                    parameterPayload.ExpectedFunctionImport((EdmOperationImport)serviceOp1);
                    return parameterPayload;

                case ODataPayloadKind.Unsupported:  // fall through
                default:
                    throw new NotSupportedException();
            }
        }

        private static ODataPayloadElement ConvertToBinaryPayloadElement(ODataPayloadElement stringPayload)
        {
            Debug.Assert(stringPayload != null && stringPayload.ElementType == ODataPayloadElementType.PrimitiveValue, "Expected non-null, primitive value as payload");

            PrimitiveValue primitiveValue = (PrimitiveValue)stringPayload;
            Debug.Assert(primitiveValue.ClrValue.GetType() == typeof(string), "Only expect string primitive values.");

            string stringValue = (string)primitiveValue.ClrValue;

            // very simple conversion assuming all chars are in the range of byte
            byte[] bytes = new byte[stringValue.Length];
            for (int i = 0; i < stringValue.Length; ++i)
            {
                bytes[i] = (byte)stringValue[i];
            }

            return PayloadBuilder.PrimitiveValue(bytes);
        }

        private static ODataPayloadElement ConvertToStringPayloadElement(ODataPayloadElement binaryPayload)
        {
            Debug.Assert(binaryPayload != null && binaryPayload.ElementType == ODataPayloadElementType.PrimitiveValue, "Expected non-null, primitive value as payload");

            PrimitiveValue primitiveValue = (PrimitiveValue)binaryPayload;
            Debug.Assert(primitiveValue.ClrValue.GetType() == typeof(byte[]), "Only expect byte[] primitive values.");

            byte[] bytes = (byte[])primitiveValue.ClrValue;

            return PayloadBuilder.PrimitiveValue(System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length));
        }

        private static ODataFormat ComputeExpectedFormat(ReaderTestConfiguration testConfiguration, string contentType)
        {
            switch (contentType)
            {
                case ApplicationJson:
                case ApplicationJsonODataLight:
                    return ODataFormat.Json;
                default:
                    return null;
            }
        }

        private sealed class ContentTypeTestCase
        {
            public ContentTypeTestCase()
            {
                this.ContentLength = 100;
            }

            public ContentTypeTestCase(ContentTypeTestCase other)
            {
                this.ContentType = other.ContentType;
                this.ExpectedException = other.ExpectedException;
                this.ShouldSucceedForPayloadKind = other.ShouldSucceedForPayloadKind;
                this.ExpectedFormat = other.ExpectedFormat;
                this.ContentLength = other.ContentLength;
            }

            public string ContentType { get; set; }
            public ODataFormat ExpectedFormat { get; set; }
            public ExpectedException ExpectedException { get; set; }
            public Func<ODataPayloadKind, bool> ShouldSucceedForPayloadKind { get; set; }
            public int ContentLength { get; set; }
            public Func<ODataPayloadKind, bool> ShouldIgnoreTest { get; set; }
        }
    }
}
