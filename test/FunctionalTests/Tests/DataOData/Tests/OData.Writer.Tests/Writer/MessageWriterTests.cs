//---------------------------------------------------------------------
// <copyright file="MessageWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
#if !SILVERLIGHT
    using System.Threading.Tasks;
#endif
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
    /// <summary>
    /// Tests for the <see cref="ODataMessageWriter" /> class to write different payloads.
    /// </summary>
    [TestClass, TestCase]
    public class MessageWriterTests : ODataWriterTestCase
    {
        private static readonly Uri baseUri = new Uri("http://www.odata.org");
        private static readonly Uri ServiceDocumentUri = new Uri("http://www.odata.org");

        private class WriterActionForPayloadKind
        {
            public ODataPayloadKind PayloadKind { get; set; }
            public Action<ODataMessageWriterTestWrapper> WriterAction { get; set; }
        }

        private static readonly IEnumerable<WriterActionForPayloadKind> writerActionsForPayloadKinds =
            new WriterActionForPayloadKind[]
            {
                new WriterActionForPayloadKind { PayloadKind = ODataPayloadKind.Unsupported, WriterAction = messageWriter => { throw new InvalidOperationException("Must not get here for unsupported kinds."); } },
                new WriterActionForPayloadKind { PayloadKind = ODataPayloadKind.Resource, WriterAction = messageWriter =>
                    { ODataWriter writer = messageWriter.CreateODataResourceWriter(); writer.WriteStart(ObjectModelUtils.CreateDefaultEntry("TestModel.CityType")); writer.WriteEnd(); writer.Flush(); } },
                new WriterActionForPayloadKind { PayloadKind = ODataPayloadKind.ResourceSet, WriterAction = messageWriter =>
                    { ODataWriter writer = messageWriter.CreateODataResourceSetWriter(); writer.WriteStart(ObjectModelUtils.CreateDefaultFeed()); writer.WriteEnd(); writer.Flush(); } },
                new WriterActionForPayloadKind { PayloadKind = ODataPayloadKind.Collection, WriterAction = messageWriter =>
                    { ODataCollectionWriter writer = messageWriter.CreateODataCollectionWriter(); writer.WriteStart(new ODataCollectionStart { Name = "collection" }); writer.WriteEnd(); writer.Flush(); } },
                new WriterActionForPayloadKind { PayloadKind = ODataPayloadKind.Batch, WriterAction = messageWriter =>
                    { ODataBatchWriterTestWrapper writer = messageWriter.CreateODataBatchWriter(); writer.WriteStartBatch(); writer.WriteStartChangeset(); writer.WriteEndChangeset(); writer.WriteEndBatch(); writer.Flush(); } },
                new WriterActionForPayloadKind { PayloadKind = ODataPayloadKind.EntityReferenceLink, WriterAction = messageWriter => messageWriter.WriteEntityReferenceLink(ObjectModelUtils.CreateDefaultEntityReferenceLink()) },
                new WriterActionForPayloadKind { PayloadKind = ODataPayloadKind.EntityReferenceLinks, WriterAction = messageWriter => messageWriter.WriteEntityReferenceLinks(ObjectModelUtils.CreateDefaultEntityReferenceLinks()) },
                new WriterActionForPayloadKind { PayloadKind = ODataPayloadKind.Error, WriterAction = messageWriter => { ODataAnnotatedError annotatedError = ObjectModelUtils.CreateDefaultError(); messageWriter.WriteError(annotatedError.Error, annotatedError.IncludeDebugInformation); } },
                new WriterActionForPayloadKind { PayloadKind = ODataPayloadKind.MetadataDocument, WriterAction = messageWriter => messageWriter.WriteMetadataDocument() },
                new WriterActionForPayloadKind { PayloadKind = ODataPayloadKind.Property, WriterAction = messageWriter => messageWriter.WriteProperty(ObjectModelUtils.CreateDefaultPrimitiveProperties()[1]) },
                new WriterActionForPayloadKind { PayloadKind = ODataPayloadKind.ServiceDocument, WriterAction = messageWriter => messageWriter.WriteServiceDocument(ObjectModelUtils.CreateDefaultWorkspace()) },
                new WriterActionForPayloadKind { PayloadKind = ODataPayloadKind.Value, WriterAction = messageWriter => messageWriter.WriteValue("foo") },
                new WriterActionForPayloadKind { PayloadKind = ODataPayloadKind.Parameter, WriterAction = messageWriter => { ODataParameterWriter writer = messageWriter.CreateODataParameterWriter(null); writer.WriteStart(); writer.WriteValue("p1", "foo"); writer.WriteEnd(); writer.Flush(); }},
            };

        [TestMethod, Variation(Description = "Verifies correct behavior of constructor of ODataMessageWriter in regard to argument validation.")]
        public void MessageWriterConstructorTest()
        {
            var nullMessageActions = new Action[]
            {
                new Action(() => new ODataMessageWriter((IODataRequestMessage)null)),
                new Action(() => new ODataMessageWriter((IODataRequestMessage)null, new ODataMessageWriterSettings())),
                new Action(() => new ODataMessageWriter((IODataRequestMessage)null, new ODataMessageWriterSettings(), null)),
                new Action(() => new ODataMessageWriter((IODataResponseMessage)null)),
                new Action(() => new ODataMessageWriter((IODataResponseMessage)null, new ODataMessageWriterSettings())),
                new Action(() => new ODataMessageWriter((IODataResponseMessage)null, new ODataMessageWriterSettings(), null)),
            };

            this.CombinatorialEngineProvider.RunCombinations(
                nullMessageActions,
                (nullMessageAction) =>
                {
                    this.Assert.ThrowsException<ArgumentException>(nullMessageAction, "Should have failed.");
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test that the message writer can only be used once.")]
        public void UseWriterOnceTests()
        {
            Uri baseUri = new Uri("http://www.odata.org/");

            ODataError error = new ODataError();
            ODataEntityReferenceLink link = new ODataEntityReferenceLink { Url = new Uri("http://www.odata.org") };
            ODataEntityReferenceLinks links = new ODataEntityReferenceLinks
            {
                Links = new ODataEntityReferenceLink[] { link, link, link },
            };

            ODataServiceDocument serviceDocument = ObjectModelUtils.CreateDefaultWorkspace();
            ODataProperty property = new ODataProperty() { Name = "FirstName", Value = "Bill" };
            object rawValue = 42;

            // TODO: add WriteMetadataDocumentAsync() here once implemented.
            UseWriterOnceTestCase[] messageWriterOperations = new UseWriterOnceTestCase[]
            {
                new UseWriterOnceTestCase { WriterMethod = (messageWriter) => messageWriter.CreateODataCollectionWriter() },
                new UseWriterOnceTestCase { WriterMethod = (messageWriter) => messageWriter.CreateODataResourceWriter() },
                new UseWriterOnceTestCase { WriterMethod = (messageWriter) => messageWriter.CreateODataResourceWriter() },
                new UseWriterOnceTestCase { WriterMethod = (messageWriter) => messageWriter.WriteError(error, false), IsWriteError = true },
                new UseWriterOnceTestCase { WriterMethod = (messageWriter) => messageWriter.WriteEntityReferenceLink(link) },
                new UseWriterOnceTestCase { WriterMethod = (messageWriter) => messageWriter.WriteEntityReferenceLinks(links) },
                new UseWriterOnceTestCase { WriterMethod = (messageWriter) => messageWriter.WriteProperty(property) },
                new UseWriterOnceTestCase { WriterMethod = (messageWriter) => messageWriter.WriteServiceDocument(serviceDocument) },
                new UseWriterOnceTestCase { WriterMethod = (messageWriter) => messageWriter.WriteValue(rawValue), IsWriteValue = true },
            };

            var testCases = messageWriterOperations.Combinations(2);

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                (testCase, testConfiguration) =>
                {
                    ODataMessageWriterSettings settingsWithBaseUri = testConfiguration.MessageWriterSettings.Clone();
                    settingsWithBaseUri.BaseUri = baseUri;
                    settingsWithBaseUri.SetServiceDocumentUri(ServiceDocumentUri);

                    using (var memoryStream = new TestStream())
                    {
                        TestMessage testMessage;
                        using (ODataMessageWriterTestWrapper messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert, out testMessage, settingsWithBaseUri))
                        {
                            testCase[0].WriterMethod(messageWriter);

                            string expectedException = "The ODataMessageWriter has already been used to write a message payload. An ODataMessageWriter can only be used once to write a payload for a given message.";
                            if (testCase[0].IsWriteValue && testCase[1].IsWriteError)
                            {
                                expectedException = "The WriteError method or the WriteErrorAsync method on ODataMessageWriter cannot be called after the WriteValue method or the WriteValueAsync method is called. In OData, writing an in-stream error for raw values is not supported.";
                            }
                            else if (!testCase[0].IsWriteError && testCase[1].IsWriteError)
                            {
                                expectedException = null;
                            }

                            TestExceptionUtils.ExpectedException<ODataException>(this.Assert, () => testCase[1].WriterMethod(messageWriter), expectedException);
                        }
                    }
                });
        }

        private class UseWriterOnceTestCase
        {
            public bool IsWriteError { get; set; }
            public bool IsWriteValue { get; set; }
            public Action<ODataMessageWriterTestWrapper> WriterMethod { get; set; }
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test that an OData entry writer can only be used to write entries and an ODataFeedWriter only for feeds.")]
        public void ODataFeedAndEntryWriterIncorrectPayload()
        {
            var testCases = new[]
                {
                    new
                    {
                        ODataWriterFunc = (Func<ODataMessageWriterTestWrapper, ODataWriter>)(messageWriter => messageWriter.CreateODataResourceWriter()),
                        WriteAction = (Action<ODataWriter>)(writer => writer.WriteStart(ObjectModelUtils.CreateDefaultFeed())),
                        ExpectedErrorMessage = "Cannot write a top-level feed with a writer that was created to write a top-level entry."
                    },
                    new
                    {
                        ODataWriterFunc = (Func<ODataMessageWriterTestWrapper, ODataWriter>)(messageWriter => messageWriter.CreateODataResourceSetWriter()),
                        WriteAction = (Action<ODataWriter>)(writer => writer.WriteStart(ObjectModelUtils.CreateDefaultEntry())),
                        ExpectedErrorMessage = "Cannot write a top-level entry with a writer that was created to write a top-level feed."
                    },
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testCase, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    using (var memoryStream = new TestStream())
                    {
                        TestMessage testMessage;
                        using (ODataMessageWriterTestWrapper messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert, out testMessage))
                        {
                            ODataWriter writer = testCase.ODataWriterFunc(messageWriter);
                            TestExceptionUtils.ExpectedException<ODataException>(
                                this.Assert,
                                () => testCase.WriteAction(writer),
                                testCase.ExpectedErrorMessage);
                        }
                    }
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Tests the correct error behavior for invalid calls after SetHeadersForPayload was called.")]
        public void SetHeadersForPayloadAndInvalidCallsErrorTest()
        {
            string errorMessageTemplate = "The payload kind '{0}' used in the last call to ODataUtils.SetHeadersForPayload is incompatible with the payload being written which is of kind '{1}'.";

            this.CombinatorialEngineProvider.RunCombinations(
                writerActionsForPayloadKinds,
                this.WriterTestConfigurationProvider.AllFormatsConfigurationsWithIndent,
                (testCase, testConfiguration) =>
                {
                    ODataMessageWriterSettings settingsWithBaseUri = testConfiguration.MessageWriterSettings.Clone();
                    settingsWithBaseUri.BaseUri = baseUri;

                    ODataPayloadKind payloadKind = testCase.PayloadKind;

                    if (!payloadKind.IsSupported(testConfiguration))
                    {
                        return;
                    }

                    foreach (var writerAction in writerActionsForPayloadKinds)
                    {
                        if (writerAction.PayloadKind == ODataPayloadKind.Unsupported || !payloadKind.IsSupported(testConfiguration))
                        {
                            return;
                        }

                        // Continue to ensure we run all test cases for a given payload kind.
                        if (payloadKind == writerAction.PayloadKind)
                        {
                            continue;
                        }

                        using (var memoryStream = new TestStream())
                        {
                            TestMessage testMessage;
                            using (ODataMessageWriterTestWrapper messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert, out testMessage, settingsWithBaseUri))
                            {
                                // expecting an argument exception for unsupported kinds; no exception otherwise.
                                TestExceptionUtils.ExpectedException<ArgumentException>(
                                    this.Assert,
                                    // this will call ODataMessage.SetHeader for valid payload kinds.
                                    () => messageWriter.SetHeadersForPayload(payloadKind),
                                    payloadKind == ODataPayloadKind.Unsupported
                                        ? "Cannot set message headers for the invalid payload kind 'Unsupported'.\r\nParameter name: payloadKind" : null,
                                    null);

                                // nothing more to do for unsupported payload kinds
                                if (payloadKind == ODataPayloadKind.Unsupported)
                                {
                                    return;
                                }

                                string errorMessage = string.Format(errorMessageTemplate, payloadKind, writerAction.PayloadKind);

                                // we expect an exception if we try to write any other payload kinds
                                TestExceptionUtils.ExpectedException<ODataException>(
                                    this.Assert,
                                    () => writerAction.WriterAction(messageWriter),
                                    errorMessage,
                                    null);
                            }
                        }
                    }
                });
        }

        [TestMethod, Variation(Description = "Tests the correct error behavior of SetHeadersForPayload.")]
        public void SetHeadersForPayloadTest()
        {
            string multipleTypesWithQualityValues = "application/json;q=0.7, application/xml;q=0.2";

            IEnumerable<SetHeadersForPayloadTestCase> testCases = new SetHeadersForPayloadTestCase[]
                {
                    #region Invalid payload kind
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Unsupported,
                        ExpectedException = ODataExpectedExceptions.ArgumentException("ODataMessageWriter_CannotSetHeadersWithInvalidPayloadKind", "Unsupported"),
                    },
                    #endregion Invalid payload kind

                    #region No accept headers
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Batch,
                        ExpectedFormatFunc = tc => ODataFormat.Batch,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.BinaryValue,
                        ExpectedFormatFunc = tc => ODataFormat.RawValue,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Collection,
                        ExpectedFormatFunc = tc => ODataFormat.Json,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.EntityReferenceLink,
                        ExpectedFormatFunc = tc => ODataFormat.Json,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.EntityReferenceLinks,
                        ExpectedFormatFunc = tc => ODataFormat.Json,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Resource,
                        ExpectedFormatFunc = tc => ODataFormat.Json,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Error,
                        ExpectedFormatFunc = tc => ODataFormat.Json,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.ResourceSet,
                        ExpectedFormatFunc = tc => ODataFormat.Json,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.MetadataDocument,
                        ExpectedFormatFunc = tc => ODataFormat.Metadata,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Parameter,
                        ExpectedFormatFunc = tc => ODataFormat.Json,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Property,
                        ExpectedFormatFunc = tc => ODataFormat.Json,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.ServiceDocument,
                        ExpectedFormatFunc = tc => ODataFormat.Json,
                    },
                    #endregion No accept headers

                    #region With accept headers
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Batch,
                        AcceptHeader = "multipart/mixed, a/b, c/d",
                        ExpectedFormatFunc = tc => ODataFormat.Batch,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.BinaryValue,
                        AcceptHeader = "application/octet-stream, a/b, c/d",
                        ExpectedFormatFunc = tc => ODataFormat.RawValue,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Collection,
                        AcceptHeader = multipleTypesWithQualityValues,
                        ExpectedFormatFunc = tc => ODataFormat.Json,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.EntityReferenceLink,
                        AcceptHeader = multipleTypesWithQualityValues,
                        ExpectedFormatFunc = tc => ODataFormat.Json,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.EntityReferenceLinks,
                        AcceptHeader = multipleTypesWithQualityValues,
                        ExpectedFormatFunc = tc => ODataFormat.Json,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Resource,
                        AcceptHeader = multipleTypesWithQualityValues,
                        ExpectedFormatFunc = tc => ODataFormat.Json,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Error,
                        AcceptHeader = multipleTypesWithQualityValues,
                        ExpectedFormatFunc = tc => ODataFormat.Json,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.ResourceSet,
                        AcceptHeader = multipleTypesWithQualityValues,
                        ExpectedFormatFunc = tc => ODataFormat.Json,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.MetadataDocument,
                        AcceptHeader = multipleTypesWithQualityValues,
                        ExpectedFormatFunc = tc => ODataFormat.Metadata,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Parameter,
                        AcceptHeader = multipleTypesWithQualityValues,
                        ExpectedFormatFunc = tc => ODataFormat.Json,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Property,
                        AcceptHeader = multipleTypesWithQualityValues,
                        ExpectedFormatFunc = tc => ODataFormat.Json,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.ServiceDocument,
                        AcceptHeader = multipleTypesWithQualityValues,
                        ExpectedFormatFunc = tc => ODataFormat.Json,
                    },
                    #endregion With accept headers

                    #region With accept charsets
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Value,
                        AcceptHeader = "text/*",
                        AcceptCharSets = null,
                        ExpectedContentType = "text/plain;charset=utf-8",
                        ExpectedFormatFunc = tc => ODataFormat.RawValue,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Value,
                        AcceptHeader = "text/*",
                        AcceptCharSets = "utf-8",
                        ExpectedContentType = "text/plain;charset=utf-8",
                        ExpectedFormatFunc = tc => ODataFormat.RawValue,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Value,
                        AcceptHeader = "text/*",
                        AcceptCharSets = "utf-8",
                        ExpectedContentType = "text/plain;charset=utf-8",
                        ExpectedFormatFunc = tc => ODataFormat.RawValue,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Value,
                        AcceptHeader = "text/*",
                        AcceptCharSets = "utf-8,iso-8859-1",
                        ExpectedContentType = "text/plain;charset=utf-8",
                        ExpectedFormatFunc = tc => ODataFormat.RawValue,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Value,
                        AcceptHeader = "text/*",
                        AcceptCharSets = "iso-8859-1,utf-8",
                        ExpectedContentType = "text/plain;charset=iso-8859-1",
                        ExpectedFormatFunc = tc => ODataFormat.RawValue,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Value,
                        AcceptHeader = "text/*",
                        AcceptCharSets = "iso-8859-1,utf-8",
                        ExpectedContentType = "text/plain;charset=iso-8859-1",
                        ExpectedFormatFunc = tc => ODataFormat.RawValue,
                    },
                    new SetHeadersForPayloadTestCase
                    {
                        PayloadKind = ODataPayloadKind.Value,
                        AcceptHeader = "text/*",
                        // unsupported encoding name; return the default encoding
                        AcceptCharSets = "abc-pqr",
                        ExpectedContentType = "text/plain;charset=utf-8",
                        ExpectedFormatFunc = tc => ODataFormat.RawValue,
                    },
                    #endregion With accept charsets
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurations,
                (testCase, testConfiguration) =>
                {
                    ODataPayloadKind payloadKind = testCase.PayloadKind;

                    TestExceptionUtils.ExpectedException(
                        this.Assert,
                        () =>
                        {
                            ODataMessageWriterSettings writerSettings = testConfiguration.MessageWriterSettings;
                            if (testCase.AcceptHeader != null)
                            {
                                writerSettings = writerSettings.Clone();
                                writerSettings.SetContentType(testCase.AcceptHeader, testCase.AcceptCharSets);
                            }

                            using (var memoryStream = new TestStream())
                            {
                                TestMessage testMessage;
                                using (ODataMessageWriterTestWrapper messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert, out testMessage, writerSettings))
                                {
                                    ODataFormat format = messageWriter.SetHeadersForPayload(testCase.PayloadKind);
                                    this.Assert.AreEqual(testCase.ExpectedFormatFunc(testConfiguration), format, "Formats don't match.");
                                }

                                // validate that the test message has the proper content type header set
                                if (testCase.ExpectedContentType != null)
                                {
                                    this.Assert.AreEqual(testCase.ExpectedContentType, testMessage.GetHeader(ODataConstants.ContentTypeHeader), "Content types don't match.");
                                }
                            }
                        },
                        testCase.ExpectedException,
                        this.ExceptionVerifier);
                });
        }

        // These tests are disabled on Silverlight and Phone because they use private reflection to validate the test result.
#if !SILVERLIGHT && !WINDOWS_PHONE
        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Tests the correct behavior of a message writer when the data service version is specified as message header.")]
        public void SetVersionInMessageHeaderTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            IEnumerable<KeyValuePair<string, ODataVersion>> validMappings = ODataVersionUtils.AllSupportedVersions.Select(
                version => new KeyValuePair<string, ODataVersion>(ODataUtils.ODataVersionToString(version), version));

            string[] suffixes = new string[]
            {
                string.Empty,       // no user-agent string
                ";",                // empty user-agent string
                ";UserAgentString", // random user-agent string
                ";NetFx"            // user-agend string used by Astoria
            };

            // Build combinations of all valid versions with a set of interesting user agent strings
            IEnumerable<VersionMessageHeaderTestCase> testCases = validMappings.SelectMany(
                mapping => suffixes.Select(
                    suffix => new VersionMessageHeaderTestCase { VersionHeaderValue = mapping.Key + suffix, ExpectedVersion = mapping.Value }));

            // Add the 'null' header case (same as no header) => default version since no version on the settings
            testCases = testCases.ConcatSingle(new VersionMessageHeaderTestCase { VersionHeaderValue = null, ExpectedVersion = ODataVersionUtils.DefaultVersion });

            // Add some error combinations
            string[] invalidVersionHeaders = { "randomstring", "V1.0", "1.5", "randomstring;1.0", "1", ";UserAgentString" };
            IEnumerable<VersionMessageHeaderTestCase> errorTestCases = invalidVersionHeaders.Select(invalidHeader =>
                new VersionMessageHeaderTestCase
                {
                    VersionHeaderValue = invalidHeader,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataUtils_UnsupportedVersionHeader", invalidHeader)
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testCases.Concat(errorTestCases),
                writerActionsForPayloadKinds,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testCase, writerInvocation, testConfiguration) =>
                {
                    ODataPayloadKind payloadKind = writerInvocation.PayloadKind;
                    if (!payloadKind.IsSupported(testConfiguration))
                    {
                        return;
                    }

                    // Clone the test configuration and make sure no version is set
                    WriterTestConfiguration clonedTestConfiguration = testConfiguration.Clone();
                    clonedTestConfiguration.MessageWriterSettings.Version = null;
                    clonedTestConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    using (var memoryStream = new TestStream())
                    {
                        TestMessage testMessage;
                        using (ODataMessageWriterTestWrapper messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, clonedTestConfiguration, this.Assert, out testMessage, null, model))
                        {
                            // Set the version header in the test message
                            testMessage.SetHeader(ODataConstants.ODataVersionHeader, testCase.VersionHeaderValue);

                            TestExceptionUtils.ExpectedException(
                                this.Assert,
                                () => writerInvocation.WriterAction(messageWriter),
                                testCase.ExpectedException,
                                this.ExceptionVerifier);

                            if (testCase.ExpectedException != null)
                            {
                                // nothing more to do if we expected an exception
                                return;
                            }

                            // Check that the version was properly set
                            ODataMessageWriterSettings actualSettings = messageWriter.PrivateSettings;
                            this.Assert.IsNotNull(actualSettings.Version, "Version on actual writer settings must not be null!");
                            this.Assert.AreEqual(testCase.ExpectedVersion, actualSettings.Version, "Versions do not match!");

                            if (testCase.VersionHeaderValue == null)
                            {
                                this.Assert.AreEqual(ODataUtils.ODataVersionToString(ODataVersionUtils.DefaultVersion), testMessage.GetHeader(ODataConstants.ODataVersionHeader), "Default version should be append to hearders!");
                            }
                        }
                    }
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Tests the correct behavior of a message writer when the data service version is specified as message header as well as in the writer settings.")]
        public void SetVersionInMessageHeaderAndSettingsTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            IEnumerable<string> validVersionHeaders = ODataVersionUtils.AllSupportedVersions.Select(version => ODataUtils.ODataVersionToString(version));

            string[] suffixes = new string[]
            {
                string.Empty,       // no user-agent string
                ";",                // empty user-agent string
                ";UserAgentString"
            };

            // Build combinations of all valid versions with a set of interesting user agent strings
            IEnumerable<VersionMessageHeaderTestCase> testCases = validVersionHeaders.SelectMany(
                validHeader => suffixes.Select(
                    suffix => new VersionMessageHeaderTestCase { VersionHeaderValue = validHeader + suffix }));

            // Add some invalid header strings; won't matter because we won't look at them
            string[] invalidVersionHeaders = { null, "randomstring", "V1.0", "1.5", "randomstring;1.0", "4.0", "1", ";UserAgentString" };
            IEnumerable<VersionMessageHeaderTestCase> errorTestCases = invalidVersionHeaders.Select(invalidHeader =>
                new VersionMessageHeaderTestCase { VersionHeaderValue = invalidHeader });


            this.CombinatorialEngineProvider.RunCombinations(
                testCases.Concat(errorTestCases),
                writerActionsForPayloadKinds,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testCase, writerInvocation, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    ODataPayloadKind payloadKind = writerInvocation.PayloadKind;
                    if (!payloadKind.IsSupported(testConfiguration))
                    {
                        return;
                    }

                    // NOTE: we don't clone the message writer settings of the test configuation; just ensure that they have a version
                    this.Assert.IsNotNull(testConfiguration.MessageWriterSettings.Version, "Version must exist on the message writer settings.");

                    using (var memoryStream = new TestStream())
                    {
                        TestMessage testMessage;
                        using (ODataMessageWriterTestWrapper messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert, out testMessage, null, model))
                        {
                            // Set the version header in the test message; this should have no effect independently of the value of the header.
                            // A version is specified on the settings and that should win.
                            testMessage.SetHeader(ODataConstants.ODataVersionHeader, testCase.VersionHeaderValue);
                            this.Assert.IsNull(testCase.ExpectedException, "No exception should be expected.");

                            writerInvocation.WriterAction(messageWriter);

                            // Check that the version was properly set (in the writer settings and in the message header)
                            ODataVersion? expectedVersion = testConfiguration.MessageWriterSettings.Version.Value;
                            this.Assert.IsNotNull(expectedVersion, "Version on actual writer settings must not be null!");

                            string versionFromHeader = testMessage.GetHeader(ODataConstants.ODataVersionHeader);
                            this.Assert.AreEqual(
                                ODataUtils.ODataVersionToString(expectedVersion.Value), versionFromHeader,
                                "The version message header should have been updated.");

                            ODataMessageWriterSettings actualSettings = messageWriter.PrivateSettings;
                            this.Assert.AreEqual(expectedVersion, actualSettings.Version, "Versions do not match!");
                        }
                    }
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Tests the correct behavior of a message writer when the content type is specified as message header.")]
        public void SetContentTypeInMessageHeaderTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            var testDescriptors = writerActionsForPayloadKinds.SelectMany(
                writerAction => CreateContentTypeHeaderTestCases(writerAction.PayloadKind).Select(
                    testCase =>
                        new
                        {
                            WriterInvocation = writerAction,
                            TestCase = testCase
                        }));
            // ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.Where(td => false),
                this.WriterTestConfigurationProvider.AllFormatsConfigurations.Where(tc => false),
                (testDescriptor, testConfiguration) =>
                {
                    ODataPayloadKind payloadKind = testDescriptor.WriterInvocation.PayloadKind;
                    if (!payloadKind.IsSupported(testConfiguration))
                    {
                        return;
                    }

                    // Clone the test configuration and make sure no content type is set on the message writer settings
                    ODataMessageWriterSettings settings = testConfiguration.MessageWriterSettings;
                    ODataMessageWriterSettings clonedSettings = new ODataMessageWriterSettings()
                    {
                        // NOTE intentionally not copying acceptable media types, acceptable char sets, format and useFormat
                        BaseUri = settings.BaseUri,
                        EnableCharactersCheck = settings.EnableCharactersCheck,
                        EnableMessageStreamDisposal = settings.EnableMessageStreamDisposal,
                        MessageQuotas = new ODataMessageQuotas(settings.MessageQuotas),
                        Version = settings.Version
                    };
                    clonedSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    WriterTestConfiguration clonedTestConfiguration = new WriterTestConfiguration(
                        testConfiguration.Format,
                        clonedSettings,
                        testConfiguration.IsRequest,
                        testConfiguration.Synchronous);

                    using (var memoryStream = new TestStream())
                    {
                        TestMessage testMessage;
                        using (ODataMessageWriterTestWrapper messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, clonedTestConfiguration, this.Assert, out testMessage, null, model))
                        {
                            // Set the content type header in the test message
                            testMessage.SetHeader(ODataConstants.ContentTypeHeader, testDescriptor.TestCase.ContentTypeHeaderValue);

                            Func<WriterTestConfiguration, ExpectedException> expectedExceptionFunc = testDescriptor.TestCase.ExpectedException;
                            ExpectedException expectedException = expectedExceptionFunc == null ? null : expectedExceptionFunc(testConfiguration);

                            TestExceptionUtils.ExpectedException(
                                this.Assert,
                                () => testDescriptor.WriterInvocation.WriterAction(messageWriter),
                                expectedException,
                                this.ExceptionVerifier);

                            if (expectedException != null)
                            {
                                // nothing more to do if we expected an exception
                                return;
                            }

                            // Check that the format and encoding was properly set
                            ODataFormat actualFormat = messageWriter.PrivateFormat;
                            Encoding actualEncoding = messageWriter.PrivateEncoding;

                            string actualEncodingName = actualEncoding != null ? actualEncoding.WebName : null;
                            string expectedEncodingName = testDescriptor.TestCase.ExpectedEncoding != null ? testDescriptor.TestCase.ExpectedEncoding.WebName : null;

                            this.Assert.AreEqual(expectedEncodingName, actualEncodingName, "Encodings do not match!");
                            this.Assert.AreEqual(testDescriptor.TestCase.ExpectedFormat, actualFormat, "Formats do not match!");

                            // For batch payloads we need to validate the boundary string as well.
                            if (payloadKind == ODataPayloadKind.Batch)
                            {
                                string batchBoundary = messageWriter.PrivateBatchBoundary;
                                this.Assert.IsNotNull(batchBoundary, "Boundary string must not be null.");
                                this.Assert.IsTrue(testDescriptor.TestCase.ContentTypeHeaderValue.Contains("boundary=" + batchBoundary), "Boundary strings don't match.");
                            }
                        }
                    }
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Tests the correct behavior of a message writer when the content type is specified as message header as well as in the writer settings.")]
        public void SetContentTypeInMessageHeaderAndSettingsTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            var testDescriptors = writerActionsForPayloadKinds.SelectMany(
                writerAction => CreateContentTypeHeaderTestCases(writerAction.PayloadKind).Select(
                    testCase =>
                        new
                        {
                            WriterInvocation = writerAction,
                            TestCase = testCase
                        }));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    ODataPayloadKind payloadKind = testDescriptor.WriterInvocation.PayloadKind;
                    if (!payloadKind.IsSupported(testConfiguration))
                    {
                        return;
                    }

                    // NOTE: ensure that the writter settings have a format set - independently of the content type header
                    //       the format of the settings should win.
                    WriterTestConfiguration clonedTestConfiguration = testConfiguration.Clone();
                    clonedTestConfiguration.MessageWriterSettings.SetContentType(testConfiguration.Format);
                    clonedTestConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    using (var memoryStream = new TestStream())
                    {
                        TestMessage testMessage;
                        using (ODataMessageWriterTestWrapper messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, clonedTestConfiguration, this.Assert, out testMessage, null, model))
                        {
                            // Set the content type header in the test message; should not have any effect.
                            testMessage.SetHeader(ODataConstants.ContentTypeHeader, testDescriptor.TestCase.ContentTypeHeaderValue);

                            TestExceptionUtils.ExpectedException(
                                this.Assert,
                                () => testDescriptor.WriterInvocation.WriterAction(messageWriter),
                                /*expectedException*/ null,
                                this.ExceptionVerifier);

                            ODataFormat expectedFormat = TestMediaTypeUtils.GetDefaultFormat(payloadKind, clonedTestConfiguration.Format);

                            // Check that the content type header has been set correctly.
                            string contentTypeHeader = testMessage.GetHeader(ODataConstants.ContentTypeHeader);
                            string defaultContentType = TestMediaTypeUtils.GetDefaultContentType(payloadKind, expectedFormat);
                            if (payloadKind == ODataPayloadKind.Batch)
                            {
                                this.Assert.IsTrue(contentTypeHeader.StartsWith(defaultContentType), "Batch content type header does not start with expected prefix.");
                            }
                            else
                            {
                                this.Assert.AreEqual(defaultContentType, contentTypeHeader, "Content type header was not properly set.");
                            }

                            // Check that the format and encoding was properly set
                            ODataFormat actualFormat = messageWriter.PrivateFormat;
                            Encoding actualEncoding = messageWriter.PrivateEncoding;
                            this.Assert.AreEqual(expectedFormat, actualFormat, "Formats do not match!");
                            this.Assert.AreEqual(TestMediaTypeUtils.GetDefaultEncoding(payloadKind, actualFormat), actualEncoding, "Encodings do not match!");
                        }
                    }
                });
        }

        // TODO: add tests that verify the correct writer behavior if the data service version is specified in the headers and ODataUtils.SetHeadersForPayload is used
        // TODO: add tests that verify the correct writer behavior if the content type is specified in the headers and ODataUtils.SetHeadersForPayload is used

        private static readonly ContentTypeMessageHeaderTestCase AppJsonLightTestCase = new ContentTypeMessageHeaderTestCase
        {
            ContentTypeHeaderValue = "application/json",
            ExpectedEncoding = TestMediaTypeUtils.EncodingUtf8NoPreamble,
            ExpectedFormat = ODataFormat.Json
        };

        //private static readonly ContentTypeMessageHeaderTestCase AppXmlTestCase = new ContentTypeMessageHeaderTestCase
        //{
        //    ContentTypeHeaderValue = "application/xml",
        //    ExpectedEncoding = null,
        //    ExpectedFormat = ODataFormat.Atom
        //};

        //private static readonly ContentTypeMessageHeaderTestCase TextXmlTestCase = new ContentTypeMessageHeaderTestCase
        //{
        //    ContentTypeHeaderValue = "text/xml",
        //    ExpectedEncoding = null,
        //    ExpectedFormat = ODataFormat.Atom
        //};

        private static IEnumerable<ContentTypeMessageHeaderTestCase> CreateContentTypeHeaderTestCases(ODataPayloadKind payloadKind)
        {

            IEnumerable<ContentTypeMessageHeaderTestCase> testCases;
            switch (payloadKind)
            {
                case ODataPayloadKind.ResourceSet:
                case ODataPayloadKind.EntityReferenceLinks:
                    testCases = new ContentTypeMessageHeaderTestCase[]
                    {
                        AppJsonLightTestCase,
                    };

                    break;
                case ODataPayloadKind.Resource:
                    testCases = new ContentTypeMessageHeaderTestCase[]
                    {
                        AppJsonLightTestCase,
                    };

                    break;
                case ODataPayloadKind.Property:
                    testCases = new ContentTypeMessageHeaderTestCase[]
                    {
                        //AppXmlTestCase,
                        //TextXmlTestCase,
                        AppJsonLightTestCase,
                    };

                    break;
                case ODataPayloadKind.EntityReferenceLink:
                    testCases = new ContentTypeMessageHeaderTestCase[]
                    {
                        //AppXmlTestCase,
                        //TextXmlTestCase,
                        AppJsonLightTestCase,
                    };

                    break;
                case ODataPayloadKind.Value:
                    testCases = new ContentTypeMessageHeaderTestCase[]
                    {
                        new ContentTypeMessageHeaderTestCase
                        {
                            ContentTypeHeaderValue = "text/plain",
                            ExpectedEncoding = TestMediaTypeUtils.Latin1Encoding,
                            ExpectedFormat = ODataFormat.RawValue
                        },
                    };

                    break;
                case ODataPayloadKind.BinaryValue:
                    testCases = new ContentTypeMessageHeaderTestCase[]
                    {
                        new ContentTypeMessageHeaderTestCase
                        {
                            ContentTypeHeaderValue = "application/octet-stream",
                            ExpectedEncoding = null,
                            ExpectedFormat = ODataFormat.RawValue
                        },
                    };

                    break;
                case ODataPayloadKind.Collection:
                    testCases = new ContentTypeMessageHeaderTestCase[]
                    {
                        //AppXmlTestCase,
                        //TextXmlTestCase,
                        AppJsonLightTestCase,
                    };

                    break;
                case ODataPayloadKind.ServiceDocument:
                    testCases = new ContentTypeMessageHeaderTestCase[]
                    {
                        AppJsonLightTestCase,
                    };

                    break;
                case ODataPayloadKind.MetadataDocument:
                    testCases = new ContentTypeMessageHeaderTestCase[]
                    {
                        new ContentTypeMessageHeaderTestCase
                        {
                            ContentTypeHeaderValue = "application/xml",
                            ExpectedEncoding = null,
                            ExpectedFormat = ODataFormat.Metadata
                        },
                    };

                    break;
                case ODataPayloadKind.Error:
                    testCases = new ContentTypeMessageHeaderTestCase[]
                    {
                        // AppXmlTestCase,
                        AppJsonLightTestCase,
                    };

                    break;
                case ODataPayloadKind.Batch:
                    testCases = new ContentTypeMessageHeaderTestCase[]
                    {
                        new ContentTypeMessageHeaderTestCase
                        {
                            ContentTypeHeaderValue = "multipart/mixed;boundary=MyBoundaryString",
                            ExpectedEncoding = null,
                            ExpectedFormat = ODataFormat.Batch
                        },
                    };
                    break;
                case ODataPayloadKind.Unsupported:
                default:
                    return Enumerable.Empty<ContentTypeMessageHeaderTestCase>();
            }

            // Add some other parameters that should not affect us
            string[] additionalParameters = new string[] { ";", ";a=b", ";a=b;c=d", ";a=b;q=0.5" };
            var extraParameterTestCases = testCases.SelectMany(
                testCase => additionalParameters.Select(
                    additionalParameter => new ContentTypeMessageHeaderTestCase(testCase)
                    {
                        ContentTypeHeaderValue = testCase.ContentTypeHeaderValue + additionalParameter
                    }));

            // Add some test cases that change the encoding
            var customEncodings = new[]
                {
                    new { CharSet = ";charset=iso-8859-1", Encoding = TestMediaTypeUtils.Latin1Encoding },
#if !SILVERLIGHT && !WINDOWS_PHONE   // Encoding not supported on these platforms
                    new { CharSet = ";charset=ibm850", Encoding = TestMediaTypeUtils.Ibm850Encoding}
#endif
                };
            var encodingTestCases = testCases.SelectMany(
                testCase => customEncodings.Select(
                    customEncoding => new ContentTypeMessageHeaderTestCase(testCase)
                    {
                        ContentTypeHeaderValue = testCase.ContentTypeHeaderValue + customEncoding.CharSet,
                        ExpectedEncoding = customEncoding.Encoding
                    }));

            // Add some error test cases
            var errorTestCases = new ContentTypeMessageHeaderTestCase[]
            {
                new ContentTypeMessageHeaderTestCase
                {
                    ContentTypeHeaderValue = "abc/pqr",
                    ExpectedException = tc => ODataExpectedExceptions.ODataContentTypeException(
                        "MediaTypeUtils_CannotDetermineFormatFromContentType",
                        TestMediaTypeUtils.GetSupportedMediaTypes(payloadKind, /*includeAppJson*/true),
                        "abc/pqr"),
                },
            };

            return testCases.Concat(extraParameterTestCases).Concat(encodingTestCases).Concat(errorTestCases);
        }
#endif

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verifies that the writer gracefully fails if the message returns a null stream.")]
        public void NullStreamMessageTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            this.CombinatorialEngineProvider.RunCombinations(
                writerActionsForPayloadKinds,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations,
                (writerInvocation, testConfiguration) =>
                {
                    if (!writerInvocation.PayloadKind.IsSupported(testConfiguration))
                    {
                        return;
                    }

                    ODataMessageWriter messageWriter;
                    if (testConfiguration.IsRequest)
                    {
                        messageWriter = new ODataMessageWriter(new NullStreamRequestMessage(), testConfiguration.MessageWriterSettings, model);
                    }
                    else
                    {
                        messageWriter = new ODataMessageWriter(new NullStreamResponseMessage(), testConfiguration.MessageWriterSettings, model);
                    }

                    using (ODataMessageWriterTestWrapper messageWriterWrapper = new ODataMessageWriterTestWrapper(messageWriter, testConfiguration))
                    {
                        this.Assert.ExpectedException(
                            () => TestExceptionUtils.UnwrapAggregateException(() => writerInvocation.WriterAction(messageWriterWrapper), this.Assert),
                            testConfiguration.IsRequest
                                ? ODataExpectedExceptions.ODataException("ODataRequestMessage_MessageStreamIsNull")
                                : ODataExpectedExceptions.ODataException("ODataResponseMessage_MessageStreamIsNull"),
                            this.ExceptionVerifier);
                    }
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verifies no BOM is written out on default writer.")]
        public void NoByteOrderMarkByDefaultTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            this.CombinatorialEngineProvider.RunCombinations(
                writerActionsForPayloadKinds.Where(writerAction =>
                    writerAction.PayloadKind != ODataPayloadKind.Unsupported &&
                    writerAction.PayloadKind != ODataPayloadKind.Batch &&
                    writerAction.PayloadKind != ODataPayloadKind.Error &&
                    writerAction.PayloadKind != ODataPayloadKind.ServiceDocument &&
                    writerAction.PayloadKind != ODataPayloadKind.Value),
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testCase, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    if (!testCase.PayloadKind.IsSupported(testConfiguration))
                    {
                        return;
                    }

                    using (var memoryStream = new TestStream())
                    {
                        TestMessage testMessage;
                        byte[] payload;
                        using (ODataMessageWriterTestWrapper messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert, out testMessage, null, model))
                        {
                            testCase.WriterAction(messageWriter);
                            payload = new byte[testMessage.TestStream.Length];
                            testMessage.TestStream.Seek(0, SeekOrigin.Begin);
                            this.Assert.IsTrue(testMessage.TestStream.Length >= 2, "payload is not big enough");

                            testMessage.TestStream.Read(payload, 0, 2 /* 2 bytes only are needed for validation */);
                        }

                        this.Assert.AreEqual(60, payload[0], "BOM might be present which is not expected");
                        this.Assert.AreEqual(63, payload[1], "BOM might be present which is not expected");
                    }
                });
        }

        [TestMethod, Variation(Description = "Verifies that IODataRequestMessage and IODataResponseMessage continue to have settable properties.")]
        public void PublicSetOnProperties()
        {
            var requestMessage = new TestRequestMessage(new MemoryStream());
            var exception = TestExceptionUtils.RunCatching(() => requestMessage.Url = new Uri("http://www.odata.org"));
            ExceptionUtilities.Assert(exception == null, "No exception was expected. Exception was thrown {0}", exception);

            var responseMessage = new TestResponseMessage(new MemoryStream());
            exception = TestExceptionUtils.RunCatching(() => responseMessage.StatusCode = 5);
            ExceptionUtilities.Assert(exception == null, "No exception was expected. Exception was thrown {0}", exception);
        }

        // These tests are disabled on Silverlight and Phone because they only run on async configuration 
#if !SILVERLIGHT && !WINDOWS_PHONE
        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verifies that the writer gracefully fails if the message returns a null stream task.")]
        public void NullStreamTaskMessageTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                writerActionsForPayloadKinds,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => !tc.Synchronous),
                (writerInvocation, testConfiguration) =>
                {
                    if (!writerInvocation.PayloadKind.IsSupported(testConfiguration))
                    {
                        return;
                    }

                    ODataMessageWriter messageWriter;
                    if (testConfiguration.IsRequest)
                    {
                        messageWriter = new ODataMessageWriter(new NullStreamTaskRequestMessage(), testConfiguration.MessageWriterSettings);
                    }
                    else
                    {
                        messageWriter = new ODataMessageWriter(new NullStreamTaskResponseMessage(), testConfiguration.MessageWriterSettings);
                    }

                    using (ODataMessageWriterTestWrapper messageWriterWrapper = new ODataMessageWriterTestWrapper(messageWriter, testConfiguration))
                    {
                        this.Assert.ExpectedException(
                            () => TestExceptionUtils.UnwrapAggregateException(() => writerInvocation.WriterAction(messageWriterWrapper), this.Assert),
                            testConfiguration.IsRequest
                                ? ODataExpectedExceptions.ODataException("ODataRequestMessage_StreamTaskIsNull")
                                : ODataExpectedExceptions.ODataException("ODataResponseMessage_StreamTaskIsNull"),
                            this.ExceptionVerifier);
                    }
                });
        }
#endif

        private class NullStreamRequestMessage :
#if SILVERLIGHT
 IODataRequestMessage
#else
 IODataRequestMessageAsync
#endif
        {
#if !SILVERLIGHT
            public Task<Stream> GetStreamAsync() { return TestTaskUtils.GetCompletedTask<Stream>(null); }
#endif
            public IEnumerable<KeyValuePair<string, string>> Headers { get { throw new TaupoInvalidOperationException("Headers should not be accessed on the message."); } }
            public Uri Url
            {
                get { throw new TaupoInvalidOperationException("Url should not be accessed on the message."); }
                set { throw new TaupoInvalidOperationException("Url should not be accessed on the message."); }
            }
            public string Method
            {
                get { throw new TaupoInvalidOperationException("Method should not be accessed on the message."); }
                set { throw new TaupoInvalidOperationException("Method should not be accessed on the message."); }
            }
            public string GetHeader(string headerName) { throw new TaupoInvalidOperationException("GetHeaders should not be accessed on the message."); }
            public void SetHeader(string headerName, string headerValue) { }
            public Stream GetStream() { return null; }
        }

        private class NullStreamResponseMessage :
#if SILVERLIGHT
 IODataResponseMessage
#else
 IODataResponseMessageAsync
#endif
        {
#if !SILVERLIGHT
            public Task<Stream> GetStreamAsync() { return TestTaskUtils.GetCompletedTask<Stream>(null); }
#endif
            public IEnumerable<KeyValuePair<string, string>> Headers { get { throw new TaupoInvalidOperationException("Headers should not be accessed on the message."); } }
            public int StatusCode
            {
                get { throw new TaupoInvalidOperationException("Status code should not be accessed on the message."); }
                set { throw new TaupoInvalidOperationException("Status code should not be accessed on the message."); }
            }

            public string GetHeader(string headerName)
            {
                if (headerName == "Preference-Applied")
                {
                    return null;
                }

                throw new TaupoInvalidOperationException("GetHeaders should not be accessed on the message.");
            }

            public void SetHeader(string headerName, string headerValue) { }
            public Stream GetStream() { return null; }
        }

        private class NullStreamTaskRequestMessage :
#if SILVERLIGHT
 IODataRequestMessage
#else
 IODataRequestMessageAsync
#endif
        {
#if !SILVERLIGHT
            public Task<Stream> GetStreamAsync() { return null; }
#endif
            public IEnumerable<KeyValuePair<string, string>> Headers { get { throw new TaupoInvalidOperationException("Headers should not be accessed on the message."); } }
            public Uri Url
            {
                get { throw new TaupoInvalidOperationException("Url should not be accessed on the message."); }
                set { throw new TaupoInvalidOperationException("Url should not be accessed on the message."); }
            }
            public string Method
            {
                get { throw new TaupoInvalidOperationException("Method should not be accessed on the message."); }
                set { throw new TaupoInvalidOperationException("Method should not be accessed on the message."); }
            }
            public string GetHeader(string headerName) { throw new TaupoInvalidOperationException("GetHeaders should not be accessed on the message."); }
            public void SetHeader(string headerName, string headerValue) { }
            public Stream GetStream() { throw new TaupoInvalidOperationException("GetStream should not be accessed on the message."); }
        }

        private class NullStreamTaskResponseMessage :
#if SILVERLIGHT
 IODataResponseMessage
#else
 IODataResponseMessageAsync
#endif
        {
#if !SILVERLIGHT
            public Task<Stream> GetStreamAsync() { return null; }
#endif
            public IEnumerable<KeyValuePair<string, string>> Headers { get { throw new TaupoInvalidOperationException("Headers should not be accessed on the message."); } }
            public int StatusCode
            {
                get { throw new TaupoInvalidOperationException("Status code should not be accessed on the message."); }
                set { throw new TaupoInvalidOperationException("Status code should not be accessed on the message."); }
            }
            public string GetHeader(string headerName)
            {
                if (headerName == "Preference-Applied")
                {
                    return null;
                }

                throw new TaupoInvalidOperationException("GetHeaders should not be accessed on the message.");
            }
            public void SetHeader(string headerName, string headerValue) { }
            public Stream GetStream() { throw new TaupoInvalidOperationException("GetStream should not be accessed on the message."); }
        }

        private sealed class SetHeadersForPayloadTestCase
        {
            private string expectedContentType = null;
            public ODataPayloadKind PayloadKind { get; set; }
            public string AcceptHeader { get; set; }
            public string AcceptCharSets { get; set; }
            public string ExpectedContentType
            {
                get { return this.expectedContentType; }

                set
                {
                    // iso-8859 is not available on Phone and Silverlight
#if !SILVERLIGHT && !WINDOWS_PHONE
                    this.expectedContentType = value;
#else
                    this.expectedContentType= value.Replace("iso-8859-1", "utf-8");
#endif
                }
            }
            public ExpectedException ExpectedException { get; set; }
            public Func<WriterTestConfiguration, ODataFormat> ExpectedFormatFunc { get; set; }
        }

        private sealed class VersionMessageHeaderTestCase
        {
            public string VersionHeaderValue { get; set; }
            public ODataVersion ExpectedVersion { get; set; }
            public ExpectedException ExpectedException { get; set; }
        }

        private sealed class ContentTypeMessageHeaderTestCase
        {
            public ContentTypeMessageHeaderTestCase()
            {
            }

            public ContentTypeMessageHeaderTestCase(ContentTypeMessageHeaderTestCase other)
            {
                this.ContentTypeHeaderValue = other.ContentTypeHeaderValue;
                this.ExpectedException = other.ExpectedException;
                this.ExpectedFormat = other.ExpectedFormat;
                this.ExpectedEncoding = other.ExpectedEncoding;
            }

            public string ContentTypeHeaderValue { get; set; }
            public ODataFormat ExpectedFormat { get; set; }
            public Encoding ExpectedEncoding { get; set; }
            public Func<WriterTestConfiguration, ExpectedException> ExpectedException { get; set; }
        }
    }
}
