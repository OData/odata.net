//---------------------------------------------------------------------
// <copyright file="BatchReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Common.Batch;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using PayloadTestDescriptor = Microsoft.Test.Taupo.OData.Common.PayloadTestDescriptor;
    using ODataUri = Microsoft.Test.Taupo.Astoria.Contracts.OData.ODataUri;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of various batch payloads in both success and error scenarios.
    /// </summary>
    [TestClass, TestCase]
    public class BatchReaderTests : ODataReaderTestCase
    {
        [InjectDependency(IsRequired = true)]
        public IModelGenerator modelGenerator { get; set; }

        [InjectDependency(IsRequired = true)]
        public PayloadReaderTestDescriptor.Settings PayloadReaderSettings { get; set; }

        [InjectDependency(IsRequired = true)]
        public IODataRequestManager RequestManager { get; set; }

        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "A batch reader test which puts together all the pieces")]
        public void BatchReaderPayloadTest()
        {
            var model = new EdmModel();

            IEnumerable<PayloadTestDescriptor> batchRequestDescriptors = 
                TestBatches.CreateBatchRequestTestDescriptors(this.RequestManager, model, /*withTypeNames*/ true);
            IEnumerable<PayloadReaderTestDescriptor> requestTestDescriptors = 
                batchRequestDescriptors.Select(bd => new PayloadReaderTestDescriptor(this.PayloadReaderSettings)
            {
                PayloadDescriptor = bd,
                SkipTestConfiguration = tc => !tc.IsRequest || (tc.Format != ODataFormat.Json)  
            });

            IEnumerable<PayloadTestDescriptor> batchResponseDescriptors =
                TestBatches.CreateBatchResponseTestDescriptors(this.RequestManager, model, /*withTypeNames*/ true);
            IEnumerable<PayloadReaderTestDescriptor> responseTestDescriptors =
                batchResponseDescriptors.Select(bd => new PayloadReaderTestDescriptor(this.PayloadReaderSettings)
                {
                    PayloadDescriptor = bd,
                    SkipTestConfiguration = tc => tc.IsRequest || (tc.Format != ODataFormat.Json)
                });

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = requestTestDescriptors.Concat(responseTestDescriptors);
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.DefaultFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Testing the ODataBatchReaderStream.ProcessPartHeader method.")]
        public void BatchReaderInvalidOperationHeadersTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = 
                this.CreateBatchRequestWithInvalidRequestLineTestDescriptors()
                .Concat(this.CreateBatchResponseWithInvalidResponseLineTestDescriptors());

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.DefaultFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    // The payloads in this test are not true ODataPayloadElements, so disable payload
                    // transformation, which breaks on them.
                    testDescriptor.ApplyPayloadTransformations = false;
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Tests different max sizes for change sets and the expected error behavior.")]
        public void BatchReaderChangeSetSizeTests()
        {
            var testCases = new[]
            {
                new
                {
                    MaxOperationsPerChangeset = (int?)null,
                    ChangeSetCount = 0,
                    ChangeSetSizes = new int[0],
                    ExpectedException = (ExpectedException)null
                },
                new
                {
                    MaxOperationsPerChangeset = (int?)null,
                    ChangeSetCount = 1,
                    ChangeSetSizes = new int[] { 1 },
                    ExpectedException = (ExpectedException)null
                },
                new
                {
                    MaxOperationsPerChangeset = (int?)null,
                    ChangeSetCount = 1,
                    ChangeSetSizes = new int[] { 3 },
                    ExpectedException = (ExpectedException)null
                },
                new
                {
                    MaxOperationsPerChangeset = (int?)null,
                    ChangeSetCount = 2,
                    ChangeSetSizes = new int[] { 0, 0 },
                    ExpectedException = (ExpectedException)null
                },
                new
                {
                    MaxOperationsPerChangeset = (int?)null,
                    ChangeSetCount = 2,
                    ChangeSetSizes = new int[] { 1, 2 },
                    ExpectedException = (ExpectedException)null
                },
                new
                {
                    MaxOperationsPerChangeset = (int?)1,
                    ChangeSetCount = 1,
                    ChangeSetSizes = new int[] { 2 },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReader_MaxChangeSetSizeExceeded", "1")
                },
                new
                {
                    MaxOperationsPerChangeset = (int?)0,
                    ChangeSetCount = 1,
                    ChangeSetSizes = new int[] { 1 },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReader_MaxChangeSetSizeExceeded", "0")
                },
                new
                {
                    MaxOperationsPerChangeset = (int?)2,
                    ChangeSetCount = 1,
                    ChangeSetSizes = new int[] { 2 },
                    ExpectedException = (ExpectedException)null
                },
                new
                {
                    MaxOperationsPerChangeset = (int?)0,
                    ChangeSetCount = 0,
                    ChangeSetSizes = new int[0],
                    ExpectedException = (ExpectedException)null
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.ReaderTestConfigurationProvider.DefaultFormatConfigurations,
                (testCase, testConfig) =>
                {
                    PayloadTestDescriptor ptd = TestBatches.CreateDefaultChangeSetBatch(
                        this.RequestManager,
                        testCase.ChangeSetCount, 
                        testCase.ChangeSetSizes,
                        testConfig.IsRequest);
                    PayloadReaderTestDescriptor testDescriptor = new PayloadReaderTestDescriptor(this.PayloadReaderSettings)
                    {
                        DebugDescription = "Batch with " + testCase.ChangeSetCount + " changesets of varying sizes.",
                        PayloadDescriptor = ptd,
                        ExpectedException = testCase.ExpectedException,
                    };

                    testConfig = ModifyBatchTestConfig(testConfig, /*maxPartsPerBatch*/ null, testCase.MaxOperationsPerChangeset);
                    testDescriptor.RunTest(testConfig);
                });
        }

        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Tests different max sizes for batches and the expected error behavior.")]
        public void BatchReaderBatchSizeTests()
        {
            var testCases = new[]
            {
                new
                {
                    MaxPartsPerBatch = (int?)null,
                    QueryCount = 0,
                    ExpectedException = (ExpectedException)null
                },
                new
                {
                    MaxPartsPerBatch = (int?)null,
                    QueryCount = 1,
                    ExpectedException = (ExpectedException)null
                },
                new
                {
                    MaxPartsPerBatch = (int?)null,
                    QueryCount = 4,
                    ExpectedException = (ExpectedException)null
                },
                new
                {
                    MaxPartsPerBatch = (int?)0,
                    QueryCount = 0,
                    ExpectedException = (ExpectedException)null
                },
                new
                {
                    MaxPartsPerBatch = (int?)1,
                    QueryCount = 1,
                    ExpectedException = (ExpectedException)null
                },
                new
                {
                    MaxPartsPerBatch = (int?)2,
                    QueryCount = 2,
                    ExpectedException = (ExpectedException)null
                },
                new
                {
                    MaxPartsPerBatch = (int?)5,
                    QueryCount = 0,
                    ExpectedException = (ExpectedException)null
                },
                new
                {
                    MaxPartsPerBatch = (int?)5,
                    QueryCount = 1,
                    ExpectedException = (ExpectedException)null
                },
                new
                {
                    MaxPartsPerBatch = (int?)5,
                    QueryCount = 2,
                    ExpectedException = (ExpectedException)null
                },
                new
                {
                    MaxPartsPerBatch = (int?)0,
                    QueryCount = 1,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReader_MaxBatchSizeExceeded", "0")
                },
                new
                {
                    MaxPartsPerBatch = (int?)1,
                    QueryCount = 2,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReader_MaxBatchSizeExceeded", "1")
                },
                new
                {
                    MaxPartsPerBatch = (int?)2,
                    QueryCount = 5,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReader_MaxBatchSizeExceeded", "2")

                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.ReaderTestConfigurationProvider.DefaultFormatConfigurations,
                (testCase, testConfig) =>
                {
                    PayloadTestDescriptor ptd = TestBatches.CreateDefaultQueryBatch(
                        this.RequestManager,
                        testCase.QueryCount,
                        testConfig.IsRequest);
                    PayloadReaderTestDescriptor testDescriptor = new PayloadReaderTestDescriptor(this.PayloadReaderSettings)
                    {
                        DebugDescription = "Batch with " + testCase.QueryCount + " queries.",
                        PayloadDescriptor = ptd,
                        ExpectedException = testCase.ExpectedException,
                    };

                    testConfig = ModifyBatchTestConfig(testConfig, testCase.MaxPartsPerBatch, /*maxOperationsPerChangeset*/ null);
                    testDescriptor.RunTest(testConfig);
                });
        }

        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Tests that for each operation in a batch payload a message is created.")]
        public void BatchReaderMessageForEachOperationTests()
        {
            IEnumerable<BatchMessageForEachOperationTestDescriptor> testDescriptors =
                this.CreateMessageForEachOperationTestDescriptors();
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.DefaultFormatConfigurations,
                (testDescriptor, testConfig) =>
                {
                    testDescriptor.RunTest(testConfig);
                });
        }

        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Tests valid and invalid boundary strings and the expected reader behavior.")]
        public void BatchReaderBoundaryStringTests()
        {
            var testCases = new[]
            {
                new
                {
                    DebugDescription = "Quoted boundary of '@'; should work.",
                    Boundary = "\"@\"",
                    ExpectedException = (ExpectedException)null
                },
                new
                {
                    DebugDescription = "Quoted boundary of two whitespaces; should work.",
                    Boundary = "\"  \"",
                    ExpectedException = (ExpectedException)null
                },
                new
                {
                    DebugDescription = "Quoted boundary of escaped double quote; should work.",
                    Boundary = "\"\\\"\"",
                    ExpectedException = (ExpectedException)null
                },
                new
                {
                    DebugDescription = "Single quote as boundary; should fail.",
                    Boundary = "\"",
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_ClosingQuoteNotFound", "Content-Type", "multipart/mixed; boundary=\"", "27"),
                },
                new
                {
                    DebugDescription = "Boundary with starting quote but no ending quote; should fail.",
                    Boundary = "\"foo",
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_ClosingQuoteNotFound", "Content-Type", "multipart/mixed; boundary=\"foo", "30"),
                },
                new
                {
                    DebugDescription = "Unquoted boundary with whitespace character; should fail.",
                    Boundary = "foo baz",
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeRequiresSemicolonBeforeParameter", "multipart/mixed; boundary=foo baz"),
                },
                new
                {
                    DebugDescription = "Unquoted boundary with '@' character; should fail.",
                    Boundary = "foo@bar",
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeRequiresSemicolonBeforeParameter", "multipart/mixed; boundary=foo@bar"),
                },
                new
                {
                    DebugDescription = "Unquoted boundary with control character (ASCII 17); should fail.",
                    Boundary = "foo" + (char)17 + "bar",
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeRequiresSemicolonBeforeParameter", "multipart/mixed; boundary=foo" + (char)17 + "bar"),
                },
                new
                {
                    DebugDescription = "Unquoted boundary with non-OCTET character (ASCII >127); should fail.",
                    Boundary = "foo" + (char)228 + "bar",
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeRequiresSemicolonBeforeParameter", "multipart/mixed; boundary=foo" + (char)228 + "bar"),
                },
                new
                {
                    DebugDescription = "Unquoted boundary with DEL control character (ASCII 127); should fail.",
                    Boundary = "foo" + (char)127 + "bar",
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeRequiresSemicolonBeforeParameter", "multipart/mixed; boundary=foo" + (char)127 + "bar"),
                },
                new
                {
                    DebugDescription = "Quoted boundary with single quote in it; should fail.",
                    Boundary = "\"\"\"",
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeRequiresSemicolonBeforeParameter", "multipart/mixed; boundary=\"\"\""),
                },
                new
                {
                    DebugDescription = "Quoted boundary with single control character (ASCII 17) in it; should fail.",
                    Boundary = "\"" + (char)17 + "\"",
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_InvalidCharacterInQuotedParameterValue", "Content-Type", "multipart/mixed; boundary=\"" + (char)17 + "\"", "27", new string((char)17, 1)),
                },
                new
                {
                    DebugDescription = "Quoted boundary with DEL control character (ASCII 127) in it; should fail.",
                    Boundary = "\"foo" + (char)127 + "bar\"",
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_InvalidCharacterInQuotedParameterValue", "Content-Type", "multipart/mixed; boundary=\"foo" + (char)127 + "bar\"", "30", new string((char)127, 1)),
                },
                new
                {
                    DebugDescription = "Quoted boundary with non-control characters outside quotes; should fail.",
                    Boundary = "\"foo\"foo",
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeRequiresSemicolonBeforeParameter", "multipart/mixed; boundary=\"foo\"foo"),
                },
                new
                {
                    DebugDescription = "Quoted boundary with control character outside quotes; should fail.",
                    Boundary = "\"foo\"" + (char)127,
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeRequiresSemicolonBeforeParameter", "multipart/mixed; boundary=\"foo\"" + (char)127),
                },
            };

            // These characters must be quoted for a valid boundary string
            char[] specialCharacters = new[] { '(', ')', '<', '>', '@', ',', ';', ':', '\\', '/', '[', ']', '?', '=' };

            Func<char, ExpectedException> getExpectedException = 
                (c) =>
                {
                    switch(c)
                    {
                        case '(' :
                        case ')' :
                        case '<' :
                        case '>' :
                        case '@' :
                        case ':' :
                        case '[' :
                        case ']' :
                        case '?' :
                        case '=' :
                        case '/' :
                            return ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeRequiresSemicolonBeforeParameter", "multipart/mixed; boundary=" + c);
                        case ',' :
                        case ';' :
                            return ODataExpectedExceptions.ODataException("ValidationUtils_InvalidBatchBoundaryDelimiterLength", string.Empty, "70");
                        case '\\' :
                            return ODataExpectedExceptions.ODataContentTypeException("HttpUtils_EscapeCharWithoutQuotes", "Content-Type", "multipart/mixed; boundary=\\", "26", "\\");
                    }

                    throw new TaupoInvalidOperationException("Unknown character '" + c + "'");
                };

            testCases = testCases.Concat(
                specialCharacters.Select(
                   c => new
                   {
                       DebugDescription = "Unquoted special character '" + c + "' should fail.",
                       Boundary = c.ToString(),
                       ExpectedException = getExpectedException(c),
                   })).ToArray();

            testCases = testCases.Concat(
                new[]
                {
                    new
                    {
                        DebugDescription = "Quoted boundary of 'special' characters; should work.",
                        Boundary = "\"" + string.Join(string.Empty, specialCharacters) + "\"",
                        ExpectedException = (ExpectedException)null
                    },
                }).ToArray();

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                new bool[] { true, false },
                this.ReaderTestConfigurationProvider.DefaultFormatConfigurations,
                (testCase, asBatchBoundary, testConfig) =>
                {
                    PayloadReaderTestDescriptor testDescriptor;
                    if (asBatchBoundary)
                    {
                        PayloadTestDescriptor ptd = TestBatches.CreateDefaultQueryBatch(
                            this.RequestManager,
                            /*queryCount*/ 1,
                            testConfig.IsRequest,
                            /*batchBoundary*/ testCase.Boundary);
                        testDescriptor = new PayloadReaderTestDescriptor(this.PayloadReaderSettings)
                        {
                            DebugDescription = "[Batch boundary] " + testCase.DebugDescription,
                            PayloadDescriptor = ptd,
                            ExpectedException = testCase.ExpectedException,
                        };
                    }
                    else
                    {
                        PayloadTestDescriptor ptd = TestBatches.CreateDefaultChangeSetBatch(
                            this.RequestManager,
                            /*changeSetCount*/ 1,
                            /*changeSetSizes*/ new int[] { 1 },
                            testConfig.IsRequest,
                            /*changeSetBoundary*/ testCase.Boundary);
                        testDescriptor = new PayloadReaderTestDescriptor(this.PayloadReaderSettings)
                        {
                            DebugDescription = "[Changeset boundary] " + testCase.DebugDescription,
                            PayloadDescriptor = ptd,
                            ExpectedException = testCase.ExpectedException,
                        };
                    }

                    testDescriptor.RunTest(testConfig);
                });
        }

        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Tests that we do not regress, where the batch reader will loop infinitely if the stream ends immediately before an operation request or response line is expected.")]
        public void BatchReaderShouldNotLoopInfinitelyIfStreamEndsUnexpectedly()
        {
            const string truncatedBatchPayload = @"--batch_862fb28e-dc50-4af1-aad5-9608647761d1
Content-Type: multipart/mixed; boundary=changeset_18c8af6c-659f-4c95-bfd5-9d7bf1ee579b

--changeset_18c8af6c-659f-4c95-bfd5-9d7bf1ee579b
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-Id: 1
";
            using (var messageReader = new ODataMessageReader(new BatchReaderTestMessage(truncatedBatchPayload)))
            {
                ODataBatchReader batchReader = messageReader.CreateODataBatchReader();
                bool hitExpectedException = false;

                while (batchReader.State != ODataBatchReaderState.Exception && batchReader.Read())
                {
                    switch (batchReader.State)
                    {
                        case ODataBatchReaderState.Operation:
                            // In the original bug, this would cause an infinite loop.
                            try
                            {
                                batchReader.CreateOperationRequestMessage();
                            }
                            catch (ODataException exception)
                            {
                                hitExpectedException = exception.Message == "Encountered an unexpected end of input while reading the batch payload.";
                            }
                            break;
                    }
                }

                Assert.IsTrue(hitExpectedException, "Expected to fail with exception message about end of input being reached.");
            }
        }

        [TestMethod, TestCategory("Reader.Batch"), Variation(Description = "Tests exception for reading batch payload without Content-ID in ChangeSet head.")]
        public void BatchReaderNonContentIdTest()
        {
            const string truncatedBatchPayload = @"--batch_862fb28e-dc50-4af1-aad5-9608647761d1
Content-Type: multipart/mixed; boundary=changeset_18c8af6c-659f-4c95-bfd5-9d7bf1ee579b

--changeset_18c8af6c-659f-4c95-bfd5-9d7bf1ee579b
Content-Type: application/http
Content-Transfer-Encoding: binary

POST http://example.org/Products HTTP/1.1

--changeset_18c8af6c-659f-4c95-bfd5-9d7bf1ee579b--
--batch_862fb28e-dc50-4af1-aad5-9608647761d1--
";
            using (var messageReader = new ODataMessageReader(new BatchReaderTestMessage(truncatedBatchPayload)))
            {
                ODataBatchReader batchReader = messageReader.CreateODataBatchReader();
                bool hitExpectedException = false;

                while (batchReader.State != ODataBatchReaderState.Exception && batchReader.Read())
                {
                    switch (batchReader.State)
                    {
                        case ODataBatchReaderState.Operation:
                            try
                            {
                                batchReader.CreateOperationRequestMessage();
                            }
                            catch (ODataException exception)
                            {
                                hitExpectedException = exception.Message == "The header with name 'Content-ID' was not present in the header collection of the batch operation.";
                            }
                            break;
                    }
                }

                Assert.IsTrue(hitExpectedException, "Expected to fail with exception message about content ID not found for Changeset.");
            }
        }

        private class BatchReaderTestMessage : IODataRequestMessage
        {
            private readonly Stream stream;

            public BatchReaderTestMessage(string messageBody)
            {
                this.stream = new MemoryStream(Encoding.UTF8.GetBytes(messageBody));
            }

            public IEnumerable<KeyValuePair<string, string>> Headers
            {
                get { throw new NotImplementedException(); }
            }

            public Uri Url
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public string Method
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public string GetHeader(string headerName)
            {
                if (headerName == "Content-Type")
                {
                    return "multipart/mixed; boundary=batch_862fb28e-dc50-4af1-aad5-9608647761d1";
                }

                return null;
            }

            public void SetHeader(string headerName, string headerValue)
            {
                
            }

            public Stream GetStream()
            {
                return this.stream;
            }
        }

        private static ReaderTestConfiguration ModifyBatchTestConfig(ReaderTestConfiguration testConfig, int? maxPartsPerBatch, int? maxOperationsPerChangeset)
        {
            ODataMessageReaderSettings newSettings = null;

            if (maxPartsPerBatch.HasValue || maxOperationsPerChangeset.HasValue)
            {
                newSettings = testConfig.MessageReaderSettings.Clone();

                if (maxPartsPerBatch.HasValue)
                {
                    newSettings.MessageQuotas.MaxPartsPerBatch = maxPartsPerBatch.Value;
                }

                if (maxOperationsPerChangeset.HasValue)
                {
                    newSettings.MessageQuotas.MaxOperationsPerChangeset = maxOperationsPerChangeset.Value;
                }
            }

            return newSettings == null
                ? testConfig
                : new ReaderTestConfiguration(testConfig.Format, newSettings, testConfig.IsRequest, testConfig.Synchronous, testConfig.Version);
        }

        /// <summary>
        /// Creates several PayloadTestDescriptors containing batch requests with invalid request lines.
        /// </summary>
        /// <returns>The created payload test descriptors with invalid request lines.</returns>
        private IEnumerable<PayloadReaderTestDescriptor> CreateBatchRequestWithInvalidRequestLineTestDescriptors()
        {
            var testCases = new[]
            {
                new
                {
                    DebugDescription = "Read a part header with only 1 segment in the request line; this will fail.",
                    RequestLine = "No_spaces_in_the_request_line",
                    ExpectedBatchExeption = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_InvalidRequestLine", "No_spaces_in_the_request_line"),
                    ExpectedChangesetException = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_InvalidRequestLine", "No_spaces_in_the_request_line"),
                },
                new
                {
                    DebugDescription = "Read a part header with only 2 segments in the request line; this will fail.",
                    RequestLine = "Only_one_space in_the_request_line",
                    ExpectedBatchExeption = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_InvalidRequestLine", "Only_one_space in_the_request_line"),
                    ExpectedChangesetException = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_InvalidRequestLine", "Only_one_space in_the_request_line"),
                },
                new
                {
                    DebugDescription = "Read a part header with an incorrect HTTP method in the request line; this will fail.",
                    RequestLine = "FOO http://odata.org/get1 HTTP/1.1",
                    ExpectedBatchExeption = ODataExpectedExceptions.ODataException("HttpUtils_InvalidHttpMethodString", "FOO"),
                    ExpectedChangesetException = ODataExpectedExceptions.ODataException("HttpUtils_InvalidHttpMethodString", "FOO"),
                },
                new
                {
                    DebugDescription = "Read a part header with an invalid (relative) URL in the request line; this will fail.",
                    RequestLine = "GET http odata.org get1 HTTP/1.1",
                    ExpectedBatchExeption = ODataExpectedExceptions.ODataException("ODataBatchUtils_RelativeUriUsedWithoutBaseUriSpecified", "http odata.org get1"),
                    ExpectedChangesetException = ODataExpectedExceptions.ODataException("ODataBatch_InvalidHttpMethodForChangeSetRequest", "GET")
                },
                new
                {
                    DebugDescription = "Read a part header with an invalid (relative) URL in the request line; this will fail.",
                    RequestLine = "GET $foo HTTP/1.1",
                    ExpectedBatchExeption = ODataExpectedExceptions.ODataException("ODataBatchUtils_RelativeUriStartingWithDollarUsedWithoutBaseUriSpecified", "$foo"),
                    ExpectedChangesetException = ODataExpectedExceptions.ODataException("ODataBatch_InvalidHttpMethodForChangeSetRequest", "GET")
                },
                new
                {
                    DebugDescription = "Read a part header with an invalid HTTP version in the request line; this will fail.",
                    RequestLine = "GET http odata.org get1",
                    ExpectedBatchExeption = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_InvalidHttpVersionSpecified", "get1", "HTTP/1.1"),
                    ExpectedChangesetException = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_InvalidHttpVersionSpecified", "get1", "HTTP/1.1"),
                },
                new
                {
                    DebugDescription = "Read a part header with no HTTP version in the request line; this will fail.",
                    RequestLine = "GET http://odata.org/get1",
                    ExpectedBatchExeption = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_InvalidRequestLine", "GET http://odata.org/get1"),
                    ExpectedChangesetException = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_InvalidRequestLine", "GET http://odata.org/get1"),
                },
                new
                {
                    DebugDescription = "Read a part header with incorrect HTTP method 'GET' in the request line of a changeset operation; this will fail.",
                    RequestLine = "GET http://odata.org/get1 HTTP/1.1",
                    ExpectedBatchExeption = (ExpectedException)null,
                    ExpectedChangesetException = ODataExpectedExceptions.ODataException("ODataBatch_InvalidHttpMethodForChangeSetRequest", "GET")
                }
            };

            string[] invalidHttpMethods = new string[] 
            {
                TestHttpUtils.HttpMethodPost,
                TestHttpUtils.HttpMethodPut,
                TestHttpUtils.HttpMethodDelete,
                TestHttpUtils.HttpMethodPatch,
            };

            testCases = testCases.Concat(invalidHttpMethods.Select(
                httpMethod =>
                    new
                    {
                        DebugDescription = "Read a part header with incorrect HTTP method '" + httpMethod + "' in the request line of a batch operation; this should work.",
                        RequestLine = httpMethod + " http://odata.org/get1 HTTP/1.1",
                        ExpectedBatchExeption = (ExpectedException)null,
                        ExpectedChangesetException = (ExpectedException)null
                    }
                )).ToArray();

            var testCasesWithNulls = testCases.SelectMany(tc => new bool[] { true, false }.Select(inChangeset =>
            {
                if (inChangeset && tc.ExpectedChangesetException == null ||
                    !inChangeset && tc.ExpectedBatchExeption == null)
                {
                    return null;
                }

                var deleteOperation = new RequestWithMutableRequestLine()
                {
                    Verb = HttpVerb.Delete,
                    Uri = new Uri("http://www.odata.org/service.svc"),
                    RequestLine = tc.RequestLine,
                };

                IMimePart part = inChangeset
                    ? (IMimePart)BatchPayloadBuilder.RequestChangeset("changeset_boundary", null, deleteOperation)
                    : (IMimePart)deleteOperation;

                var testDescriptor = new PayloadReaderTestDescriptor(this.PayloadReaderSettings)
                {
                    DebugDescription = tc.DebugDescription,
                    PayloadDescriptor = new PayloadTestDescriptor()
                    {
                        PayloadElement = PayloadBuilder.BatchRequestPayload(part)
                            .AddAnnotation(new BatchBoundaryAnnotation("bb_singleoperation"))
                    },
                    ExpectedException = inChangeset ? tc.ExpectedChangesetException : tc.ExpectedBatchExeption,
                    SkipTestConfiguration = testConfig => !testConfig.IsRequest
                };

                testDescriptor.ExpectedResultNormalizers.Clear();
                return testDescriptor;
            }));

            return testCasesWithNulls.Where(tc => tc != null);
        }

        /// <summary>
        /// Creates several PayloadTestDescriptors containing batch response with invalid response lines.
        /// </summary>
        /// <returns>The created payload test descriptors with invalid response lines.</returns>
        private IEnumerable<PayloadReaderTestDescriptor> CreateBatchResponseWithInvalidResponseLineTestDescriptors()
        {          
            var testCases = new[]
            {
                new
                {
                    DebugDescription = "Read a part header with only 1 segment in the response line; this will fail.",
                    ResponseLine = "No_spaces_in_the_response_line",
                    ExpectedExeption = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_InvalidResponseLine", "No_spaces_in_the_response_line"),
                },
                new
                {
                    DebugDescription = "Read a part header with only 2 segments in the response line; this will fail.",
                    ResponseLine = "Only_one_space in_the_response_line",
                    ExpectedExeption = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_InvalidResponseLine", "Only_one_space in_the_response_line"),
                },
                new
                {
                    DebugDescription = "Read a part header with an invalid HTTP version in the response line; this will fail.",
                    ResponseLine = "HTTP/2.2 200 Ok",
                    ExpectedExeption = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_InvalidHttpVersionSpecified", "HTTP/2.2", "HTTP/1.1"),
                },
                new
                {
                    DebugDescription = "Read a part header with an incorrect status code in the response line; this will fail.",
                    ResponseLine = "HTTP/1.1 NaN Ok",
                    ExpectedExeption = ODataExpectedExceptions.ODataException("ODataBatchReaderStream_NonIntegerHttpStatusCode", "NaN"),
                },
            };

            return testCases.Select(tc =>
            {
                // Operation with no payload
                var operationResponse = new ResponseWithMutableStatusLine()
                {
                    StatusCode = HttpStatusCode.OK,
                    StatusLine = tc.ResponseLine,
                };

                var testDescriptor = new PayloadReaderTestDescriptor(this.PayloadReaderSettings)
                {
                    DebugDescription = tc.DebugDescription,
                    PayloadDescriptor = new PayloadTestDescriptor()
                    {
                        PayloadElement = PayloadBuilder.BatchResponsePayload(operationResponse)
                            .AddAnnotation(new BatchBoundaryAnnotation("bb_singleoperation"))
                    },
                    ExpectedException = tc.ExpectedExeption,
                    SkipTestConfiguration = testConfig => testConfig.IsRequest
                };

                testDescriptor.ExpectedResultNormalizers.Clear();
                return testDescriptor;
            });

        }

        /// <summary>
        /// Creates several test descriptors for testing that not creating operation messages for operations really fails.
        /// </summary>
        /// <returns>The created test descriptors.</returns>
        private IEnumerable<BatchMessageForEachOperationTestDescriptor> CreateMessageForEachOperationTestDescriptors()
        {
            var edmModel = new EdmModel().Fixup();
            var emptyPayload = new PayloadTestDescriptor()
            {
                PayloadEdmModel = edmModel
            };

            var root = ODataUriBuilder.Root(new Uri("http://www.odata.org/service.svc"));
            var rootUri = new ODataUri(new ODataUriSegment[] { root });

            ODataResponse responseOperation = emptyPayload.InResponseOperation(200, this.RequestManager);
            ODataRequest requestOperation = emptyPayload.InRequestOperation(HttpVerb.Get, rootUri, this.RequestManager);

            BatchRequestChangeset emptyRequestChangeset = BatchUtils.GetRequestChangeset(new IMimePart[0], this.RequestManager);
            BatchResponseChangeset emptyResponseChangeset = BatchUtils.GetResponseChangeset(new IMimePart[0], this.RequestManager);
            BatchRequestChangeset requestChangeset = BatchUtils.GetRequestChangeset(new IMimePart[] { requestOperation }, this.RequestManager);
            BatchResponseChangeset responseChangeset = BatchUtils.GetResponseChangeset(new IMimePart[] { responseOperation }, this.RequestManager);

            return new BatchMessageForEachOperationTestDescriptor[]
            {
                // Empty batch payloads
                new BatchMessageForEachOperationTestDescriptor(this.PayloadReaderSettings)
                {
                    DebugDescription = "Empty payload without operations in request; should not fail.",
                    PayloadElement = PayloadBuilder.BatchRequestPayload()
                        .AddAnnotation(new BatchBoundaryAnnotation("request_emptybatch")),
                    SkipTestConfiguration = testConfig => !testConfig.IsRequest
                },
                new BatchMessageForEachOperationTestDescriptor(this.PayloadReaderSettings)
                {
                    DebugDescription = "Empty payload without operations in response; should not fail.",
                    PayloadElement = PayloadBuilder.BatchResponsePayload()
                        .AddAnnotation(new BatchBoundaryAnnotation("response_emptybatch")),
                    SkipTestConfiguration = testConfig => testConfig.IsRequest
                },

                // Single query operation payloads
                new BatchMessageForEachOperationTestDescriptor(this.PayloadReaderSettings)
                {
                    DebugDescription = "Single query operation in request; should fail.",
                    PayloadElement = PayloadBuilder.BatchRequestPayload(requestOperation)
                        .AddAnnotation(new BatchBoundaryAnnotation("request_singleoperation")),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReader_NoMessageWasCreatedForOperation"),
                    SkipTestConfiguration = testConfig => !testConfig.IsRequest
                },
                new BatchMessageForEachOperationTestDescriptor(this.PayloadReaderSettings)
                {
                    DebugDescription = "Empty payload without operations in response; should fail.",
                    PayloadElement = PayloadBuilder.BatchResponsePayload(responseOperation)
                        .AddAnnotation(new BatchBoundaryAnnotation("response_singleoperation")),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReader_NoMessageWasCreatedForOperation"),
                    SkipTestConfiguration = testConfig => testConfig.IsRequest
                },
            
                // Empty changeset payloads
                new BatchMessageForEachOperationTestDescriptor(this.PayloadReaderSettings)
                {
                    DebugDescription = "Empty changeset in request; should not fail.",
                    PayloadElement = PayloadBuilder.BatchRequestPayload(emptyRequestChangeset)
                        .AddAnnotation(new BatchBoundaryAnnotation("request_emptychangeset")),
                    SkipTestConfiguration = testConfig => !testConfig.IsRequest
                },
                new BatchMessageForEachOperationTestDescriptor(this.PayloadReaderSettings)
                {
                    DebugDescription = "Empty changeset in response; should not fail.",
                    PayloadElement = PayloadBuilder.BatchResponsePayload(responseChangeset)
                        .AddAnnotation(new BatchBoundaryAnnotation("response_changeset")),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReader_NoMessageWasCreatedForOperation"),
                    SkipTestConfiguration = testConfig => testConfig.IsRequest
                },
            
                // Changeset with query operation payloads
                new BatchMessageForEachOperationTestDescriptor(this.PayloadReaderSettings)
                {
                    DebugDescription = "Empty changeset in request; should not fail.",
                    PayloadElement = PayloadBuilder.BatchRequestPayload(requestChangeset)
                        .AddAnnotation(new BatchBoundaryAnnotation("request_changeset")),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchReader_NoMessageWasCreatedForOperation"),
                    SkipTestConfiguration = testConfig => !testConfig.IsRequest
                },
                new BatchMessageForEachOperationTestDescriptor(this.PayloadReaderSettings)
                {
                    DebugDescription = "Empty changeset in response; should not fail.",
                    PayloadElement = PayloadBuilder.BatchResponsePayload(emptyResponseChangeset)
                        .AddAnnotation(new BatchBoundaryAnnotation("response_emptychangeset")),
                    SkipTestConfiguration = testConfig => testConfig.IsRequest
                },
            };
        }

        /// <summary>
        /// Test descriptor for testing that we fail if we don't create a message for each operation
        /// in a batch or changeset.
        /// </summary>
        private sealed class BatchMessageForEachOperationTestDescriptor : PayloadReaderTestDescriptor
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="settings">Settings class to use.</param>
            public BatchMessageForEachOperationTestDescriptor(Settings settings)
                : base(settings)
            {
            }

            /// <summary>
            /// Called to get the expected result of the test.
            /// </summary>
            /// <param name="testConfiguration">The test configuration to use.</param>
            /// <returns>The expected result.</returns>
            protected override ReaderTestExpectedResult GetExpectedResult(ReaderTestConfiguration testConfiguration)
            {
                BatchReaderTestExpectedResult batchReaderExpectedResult = (BatchReaderTestExpectedResult)base.GetExpectedResult(testConfiguration);
                return new BatchMessageForEachOperationTestExpectedResults(this.settings.BatchExpectedResultSettings, batchReaderExpectedResult);
            }
        }

        /// <summary>
        /// Batch Reader test expected result to ensure that CreateOperationRequestMessage/CreateOperationResponseMessage
        /// is called for each operation in the batch.
        /// </summary>
        private sealed class BatchMessageForEachOperationTestExpectedResults : BatchReaderTestExpectedResult
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="settings">Settings class to use.</param>
            public BatchMessageForEachOperationTestExpectedResults(BatchReaderTestExpectedResultSettings settings, BatchReaderTestExpectedResult batchExpectedResult)
                : base(settings)
            {
                this.PayloadModel = batchExpectedResult.PayloadModel;
                this.ExpectedBatchPayload = batchExpectedResult.ExpectedBatchPayload;
                this.ExpectedException = batchExpectedResult.ExpectedException;
            }

            /// <summary>
            /// Verifies that the result of the test (the message reader) is what the test expected.
            /// </summary>
            /// <param name="messageReader">The message reader which is the result of the test. This method should use it to read the results
            /// of the parsing and verify those.</param>
            /// <param name="payloadKind">The payload kind specified in the test descriptor.</param>
            /// <param name="testConfiguration">The test configuration to use.</param>
            public override void VerifyResult(ODataMessageReaderTestWrapper messageReader, ODataPayloadKind payloadKind, ReaderTestConfiguration testConfiguration)
            {
                // We are not verifying anything here other than reading the result in a non-compliant
                // way by not creating operation messages when we encounter an operation in a batch/changeset.
                ODataBatchReaderTestWrapper batchReader = messageReader.CreateODataBatchReader();
                this.settings.Assert.AreEqual(ODataBatchReaderState.Initial, batchReader.State, "Reader states don't match.");

                while (batchReader.Read())
                {
                    // Just keep reading through the whole batch.
                    // The batch reader should fail if no operation message is created for an operation.
                }
            }
        }

        /// <summary>
        /// Response which allows the status line to be modified
        /// </summary>
        private sealed class ResponseWithMutableStatusLine : HttpResponseData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ResponseWithMutableStatusLine"/> class.
            /// </summary>
            public ResponseWithMutableStatusLine()
                : base()
            {
            }

            /// <summary>
            /// Gets or sets the status line.
            /// </summary>
            /// <value>
            /// The status line.
            /// </value>
            public string StatusLine { get; set; }

            /// <summary>
            /// Returns the first line of the message
            /// </summary>
            /// <returns>
            /// The first line of the message
            /// </returns>
            public override string GetFirstLine()
            {
                return this.StatusLine;
            }
        }

        /// <summary>
        /// REquest which allows the request line to be modified
        /// </summary>
        private sealed class RequestWithMutableRequestLine : HttpRequestData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RequestWithMutableRequestLine"/> class.
            /// </summary>
            public RequestWithMutableRequestLine()
                : base()
            {
            }

            /// <summary>
            /// Gets or sets the request line.
            /// </summary>
            /// <value>
            /// The request line.
            /// </value>
            public string RequestLine { get; set; }

            /// <summary>
            /// Returns the first line of the message
            /// </summary>
            /// <returns>
            /// The first line of the message
            /// </returns>
            public override string GetFirstLine()
            {
                return this.RequestLine;
            }
        }
    }
}
