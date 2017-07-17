//---------------------------------------------------------------------
// <copyright file="BatchWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.BatchWriter
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
#if !SILVERLIGHT
    using System.Threading.Tasks;
#endif
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Tests.WriterTests.BatchWriter;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common.Batch;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
    /// <summary>
    /// Tests for writing batch requests/responses with the ODataBatchWriter.
    /// </summary>
    [TestClass, TestCase]
    public class BatchWriterTests : ODataBatchWriterTestCase
    {
        [InjectDependency(IsRequired = true)]
        public BatchWriterTestDescriptor.Settings Settings { get; set; }

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestExpectedResults.Settings ExpectedResultSettings { get; set; }

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings PayloadWriterSettings { get; set; }

        [InjectDependency(IsRequired = true)]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [InjectDependency(IsRequired = true)]
        public BatchResponseToDummyRequestGenerator RequestGenerator { get; set; }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Tests writing requests with the batch writer (with dummy payloads).")]
        public void ODataBatchWriterRequestTests()
        {
            Uri baseUri = new Uri("http://services.odata.org/OData/OData.svc");

            var testCases = BatchWriterUtils.CreateBatchRequestTestCases(this.CombinatorialEngineProvider);
            IEnumerable<BatchWriterTestDescriptor> testDescriptors = testCases.Select(tc => new BatchWriterTestDescriptor(this.Settings, tc, (Dictionary<string, string>)null, baseUri));

            // Add a couple of payloads for testing parameter checking
            testDescriptors = testDescriptors.Concat(new BatchWriterTestDescriptor[]
            {
                // null method for request on top-level
                new BatchWriterTestDescriptor(
                    this.Settings,
                    new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                    {
                        BatchWriterUtils.StartBatch(),
                        BatchWriterUtils.QueryOperation(null, new Uri("http://odata.org")),
                    },
                    ODataExpectedExceptions.ArgumentNullOrEmptyException()),
                // null method for request in changeset
                new BatchWriterTestDescriptor(
                    this.Settings,
                    new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                    {
                        BatchWriterUtils.StartBatch(),
                        BatchWriterUtils.StartChangeSet(),
                        BatchWriterUtils.QueryOperation(null, new Uri("http://odata.org")),
                    },
                    ODataExpectedExceptions.ArgumentNullOrEmptyException()),
                // empty method for request on top-level
                new BatchWriterTestDescriptor(
                    this.Settings,
                    new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                    {
                        BatchWriterUtils.StartBatch(),
                        BatchWriterUtils.QueryOperation(string.Empty, new Uri("http://odata.org")),
                    },
                    ODataExpectedExceptions.ArgumentNullOrEmptyException()),
                // empty method for request in changeset
                new BatchWriterTestDescriptor(
                    this.Settings,
                    new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                    {
                        BatchWriterUtils.StartBatch(),
                        BatchWriterUtils.StartChangeSet(),
                        BatchWriterUtils.QueryOperation(string.Empty, new Uri("http://odata.org")),
                    },
                    ODataExpectedExceptions.ArgumentNullOrEmptyException()),
                // Modify method for request on top-level should work
                new BatchWriterTestDescriptor(
                    this.Settings,
                    new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                    {
                        BatchWriterUtils.StartBatch(),
                        BatchWriterUtils.QueryOperation("POST", new Uri("http://odata.org")),
                        BatchWriterUtils.EndBatch()
                    }),
                // wrong method for request in changeset
                new BatchWriterTestDescriptor(
                    this.Settings,
                    new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                    {
                        BatchWriterUtils.StartBatch(),
                        BatchWriterUtils.StartChangeSet(),
                        BatchWriterUtils.QueryOperation("GET", new Uri("http://odata.org")),
                    },
                    ODataExpectedExceptions.ODataException("ODataBatch_InvalidHttpMethodForChangeSetRequest", "GET")),
                // null uri for request on top-level
                new BatchWriterTestDescriptor(
                    this.Settings,
                    new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                    {
                        BatchWriterUtils.StartBatch(),
                        BatchWriterUtils.QueryOperation("GET", null),
                    },
                    ODataExpectedExceptions.ArgumentNullException()),
                // null uri for request in changeset
                new BatchWriterTestDescriptor(
                    this.Settings,
                    new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                    {
                        BatchWriterUtils.StartBatch(),
                        BatchWriterUtils.StartChangeSet(),
                        BatchWriterUtils.QueryOperation("POST", null),
                    },
                    ODataExpectedExceptions.ArgumentNullException()),
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent.Where(tc => tc.IsRequest),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => tc.IsRequest),
                (testDescriptor, testConfig, payloadTestConfig) =>
                {
                    testConfig = ModifyBatchTestConfig(testConfig, testDescriptor);
                    payloadTestConfig = ModifyPayloadTestConfig(payloadTestConfig, testDescriptor.BaseUri);
                    BatchWriterUtils.WriteAndVerifyBatchPayload(testDescriptor, testConfig, payloadTestConfig, this.Assert);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Tests writing responses with the batch writer (with dummy payloads).")]
        public void ODataBatchWriterResponseTests()
        {
            Uri baseUri = new Uri("http://services.odata.org/OData/OData.svc");

            var testCases = BatchWriterUtils.CreateBatchResponseTestCases(this.CombinatorialEngineProvider);
            var testDescriptors = testCases.Select(tc => new BatchWriterTestDescriptor(this.Settings, tc, (Dictionary<string, string>)null, baseUri));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent.Where(tc => !tc.IsRequest),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfig, payloadTestConfig) =>
                {
                    BatchWriterUtils.WriteAndVerifyBatchPayload(testDescriptor, testConfig, payloadTestConfig, this.Assert);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Tests correct handling of absolute and relative URIs in batches.")]
        public void ODataBatchWriterBaseUriTests()
        {
            Func<Uri, Uri, ExpectedException, BatchWriterTestDescriptor> createInvokation = (uri, baseUri, expectedException) =>
                {
                    var invocations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                    {
                        BatchWriterUtils.StartBatch(),
                        BatchWriterUtils.QueryOperation("GET", uri),
                        BatchWriterUtils.EndBatch(),
                    };
                    return expectedException == null
                        ? new BatchWriterTestDescriptor(this.Settings, invocations, (Dictionary<string, string>)null, baseUri)
                        : new BatchWriterTestDescriptor(this.Settings, invocations, expectedException, baseUri);
                };

            var testCases = new[]
                {
                    new
                    {
                        Uri = new Uri("http://odata.services.org/OData/OData.svc"),
                        BaseUri = (Uri) null,
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        Uri = new Uri("http://odata.services.org/OData/OData.svc"),
                        BaseUri = new Uri("http://odata.services.org"),
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        Uri = new Uri("/OData/OData.svc", UriKind.Relative),
                        BaseUri = new Uri("http://odata.services.org"),
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        Uri = new Uri("/OData/OData.svc", UriKind.Relative),
                        BaseUri = new Uri("http://odata.services.org"),
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        Uri = new Uri("/OData/OData.svc", UriKind.Relative),
                        BaseUri = (Uri) null,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchUtils_RelativeUriUsedWithoutBaseUriSpecified", "/OData/OData.svc")
                    },
                };

            var testDescriptors = testCases.Select(tc => createInvokation(tc.Uri, tc.BaseUri, tc.ExpectedException));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent.Where(tc => tc.IsRequest),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => tc.IsRequest),
                (testDescriptor, testConfig, payloadTestConfig) =>
                {
                    testConfig = ModifyBatchTestConfig(testConfig, testDescriptor);
                    payloadTestConfig = ModifyPayloadTestConfig(payloadTestConfig, testDescriptor.BaseUri);
                    BatchWriterUtils.WriteAndVerifyBatchPayload(testDescriptor, testConfig, payloadTestConfig, this.Assert);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Tests different combinations of Http methods for query operations and changeset requests.")]
        public void ODataBatchWriterHttpMethodTests()
        {
            Func<string, string, BatchWriterTestDescriptor> createQueryOperation = (method, expectedError) =>
            {
                var invocations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                    {
                        BatchWriterUtils.StartBatch(),
                        BatchWriterUtils.QueryOperation(method, new Uri("http://www.odata.org/")),
                        BatchWriterUtils.EndBatch(),
                    };

                return expectedError == null
                    ? new BatchWriterTestDescriptor(this.Settings, invocations, (Dictionary<string, string>)null)
                    : new BatchWriterTestDescriptor(this.Settings, invocations, expectedError);
            };

            Func<string, string, BatchWriterTestDescriptor> createChangeSetRequest = (method, expectedError) =>
            {
                var invocations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.StartBatch(),
                    BatchWriterUtils.StartChangeSet(),
                    BatchWriterUtils.ChangeSetRequest(method, new Uri("http://www.odata.org/OData/OData.svc/Products"), "1", null, "First sample payload."),
                    BatchWriterUtils.EndChangeSet(),
                    BatchWriterUtils.EndBatch(),
                };

                return expectedError == null
                    ? new BatchWriterTestDescriptor(this.Settings, invocations, (Dictionary<string, string>)null)
                    : new BatchWriterTestDescriptor(this.Settings, invocations, expectedError);
            };

            Func<string, string, BatchWriterTestDescriptor> createChangeSetRequestWithNullContentId = (method, expectedError) =>
            {
                var invocations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.StartBatch(),
                    BatchWriterUtils.StartChangeSet(),
                    BatchWriterUtils.ChangeSetRequest(method, new Uri("http://www.odata.org/OData/OData.svc/Products"), string.Empty, null, "First sample payload."),
                    BatchWriterUtils.EndChangeSet(),
                    BatchWriterUtils.EndBatch(),
                };

                return expectedError == null
                    ? new BatchWriterTestDescriptor(this.Settings, invocations, (Dictionary<string, string>)null)
                    : new BatchWriterTestDescriptor(this.Settings, invocations, expectedError);
            };

            IEnumerable<string> httpMethodValues = new string[]
            {
                Microsoft.OData.ODataConstants.MethodGet,
                Microsoft.OData.ODataConstants.MethodPatch,
                Microsoft.OData.ODataConstants.MethodPost,
                Microsoft.OData.ODataConstants.MethodPut,
            };

            // Now we allow non-query outside of changeset.
            var queryTestCases = httpMethodValues.Select(
                method => new { Method = method, ExpectedErrorMessage = (string)null });

            var changeSetOpTestCases = httpMethodValues.Select(
                method => method == "GET"
                    ? new { Method = "GET", ExpectedErrorMessage = "An invalid HTTP method 'GET' was detected for a request in a change set. Requests in change sets only support the HTTP methods 'POST', 'PUT', 'DELETE', and 'PATCH'." }
                    : new { Method = method, ExpectedErrorMessage = (string)null });

            var changeSetOpWithNoContentIdTestCases = httpMethodValues.Select(
                method => method == "GET"
                    ? new { Method = "GET", ExpectedErrorMessage = "An invalid HTTP method 'GET' was detected for a request in a change set. Requests in change sets only support the HTTP methods 'POST', 'PUT', 'DELETE', and 'PATCH'." }
                    : new { Method = method, ExpectedErrorMessage = "The header with name 'Content-ID' was not present in the header collection of the batch operation." });

            var testDescriptors = queryTestCases.Select(tc => createQueryOperation(tc.Method, tc.ExpectedErrorMessage))
                .Concat(changeSetOpTestCases.Select(tc => createChangeSetRequest(tc.Method, tc.ExpectedErrorMessage)))
                .Concat(changeSetOpWithNoContentIdTestCases.Select(tc => createChangeSetRequestWithNullContentId(tc.Method, tc.ExpectedErrorMessage)));
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent.Where(tc => tc.IsRequest),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => tc.IsRequest),
                (testDescriptor, testConfig, payloadTestConfig) =>
                {
                    testConfig = ModifyBatchTestConfig(testConfig, testDescriptor);
                    payloadTestConfig = ModifyPayloadTestConfig(payloadTestConfig, testDescriptor.BaseUri);
                    BatchWriterUtils.WriteAndVerifyBatchPayload(testDescriptor, testConfig, payloadTestConfig, this.Assert);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Tests various cross referencing links in changeset operation headers.")]
        public void ODataBatchWriterCrossReferencingLinksInHeaderTest()
        {
            Uri baseUri = new Uri("http://odata.org/base");

            // Start with test cases that have cross-referencing links in the request header and use base URIs
            Func<string, IEnumerable<CrossReferencingTestCase>> testCasesFunc = (payloadString) => new CrossReferencingTestCase[]
                {
                    new CrossReferencingTestCase
                    {
                        DebugDescription = "Valid cross-reference link in request header (reference only).",
                        BaseUri = baseUri,
                        Operations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://odata.org/1"), "1", /*headers*/ null, payloadString),
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("$1", UriKind.Relative), "2", /*headers*/ null, payloadString),
                        },
                    },
                    new CrossReferencingTestCase
                    {
                        DebugDescription = "Valid cross-reference link in request header (segement after reference).",
                        BaseUri = baseUri,
                        Operations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://odata.org/1"), "1", /*headers*/ null, payloadString),
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("$1/foo", UriKind.Relative), "2", /*headers*/ null, payloadString),
                        },
                    },
                    new CrossReferencingTestCase
                    {
                        DebugDescription = "Valid cross-reference link in request header (special name '$$$').",
                        BaseUri = baseUri,
                        Operations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://odata.org/1"), "$$$", /*headers*/ null, payloadString),
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("$$$$", UriKind.Relative), "2", /*headers*/ null, payloadString),
                        },
                    },
                    new CrossReferencingTestCase
                    {
                        DebugDescription = "Invalid cross-reference link in request header (content ID does not exist).",
                        BaseUri = baseUri,
                        Operations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://odata.org/1"), "1", /*headers*/ null, payloadString),
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("$NonExisting", UriKind.Relative), "2", /*headers*/ null, payloadString, true),
                        },
                        ExpectedException = (ExpectedException)null,
                        ExpectedExceptionNoBaseUri = ODataExpectedExceptions.ODataException("ODataBatchUtils_RelativeUriStartingWithDollarUsedWithoutBaseUriSpecified", "$NonExisting"),
                    },
                    new CrossReferencingTestCase
                    {
                        DebugDescription = "Invalid cross-reference link in request header (content ID will only appear later in the batch).",
                        BaseUri = baseUri,
                        Operations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("$2", UriKind.Relative), "1", /*headers*/ null, payloadString, true),
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://odata.org/2"), "2", /*headers*/ null, payloadString),
                        },
                        ExpectedException = (ExpectedException)null,
                        ExpectedExceptionNoBaseUri = ODataExpectedExceptions.ODataException("ODataBatchUtils_RelativeUriStartingWithDollarUsedWithoutBaseUriSpecified", "$2"),
                    },
                    new CrossReferencingTestCase
                    {
                        DebugDescription = "Invalid payload (duplicate content IDs).",
                        BaseUri = baseUri,
                        Operations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://odata.org/1"), "Duplicate", /*headers*/ null, payloadString),
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://odata.org/2"), "Duplicate", /*headers*/ null, payloadString),
                        },
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchWriter_DuplicateContentIDsNotAllowed", "Duplicate"),
                        ExpectedExceptionNoBaseUri = ODataExpectedExceptions.ODataException("ODataBatchWriter_DuplicateContentIDsNotAllowed", "Duplicate"),
                    },
                    new CrossReferencingTestCase
                    {
                        DebugDescription = "Invalid cross-reference link in request header (reference content ID in different changeset).",
                        BaseUri = baseUri,
                        Operations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://odata.org/1"), "1", /*headers*/ null, payloadString),
                        },
                        Operations2 = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("$1", UriKind.Relative), "2", /*headers*/ null, payloadString, true),
                        },
                        ExpectedExceptionNoBaseUri = ODataExpectedExceptions.ODataException("ODataBatchUtils_RelativeUriStartingWithDollarUsedWithoutBaseUriSpecified", "$1"),
                    },
                    new CrossReferencingTestCase
                    {
                        DebugDescription = "Invalid cross-reference link in request header (reference to current content ID).",
                        BaseUri = baseUri,
                        Operations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://odata.org/1"), "1", /*headers*/ null, payloadString),
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("$2", UriKind.Relative), "2", /*headers*/ null, payloadString, true),
                        },
                        ExpectedExceptionNoBaseUri = ODataExpectedExceptions.ODataException("ODataBatchUtils_RelativeUriStartingWithDollarUsedWithoutBaseUriSpecified", "$2"),
                    },
                };

            string[] payloadStrings = new string[]
            {
                "Non-empty dummy payload",
                ////string.Empty,
                null
            };

            IEnumerable<CrossReferencingTestCase> testCases = payloadStrings.SelectMany(payloadString => testCasesFunc(payloadString));

            // Now add test cases that have cross-referencing links in the request header but don't use base URIs
            testCases = testCases.Concat(testCases.Select(tc =>
                new CrossReferencingTestCase
                {
                    DebugDescription = tc.DebugDescription + " [No base Uri]",
                    BaseUri = (Uri)null,
                    Operations = tc.Operations,
                    Operations2 = tc.Operations2,
                    ExpectedException = tc.ExpectedException,
                    ExpectedExceptionNoBaseUri = tc.ExpectedExceptionNoBaseUri,
                }));

            Func<CrossReferencingTestCase, BatchWriterTestDescriptor> createChangeSetRequest = (testCase) =>
                {
                    var invocations = new List<BatchWriterTestDescriptor.InvocationAndOperationDescriptor>();
                    invocations.Add(BatchWriterUtils.StartBatch());
                    invocations.Add(BatchWriterUtils.StartChangeSet());

                    var operations = testCase.Operations;
                    for (int i = 0; i < operations.Length; ++i)
                    {
                        invocations.Add(operations[i]);
                    }

                    var operations2 = testCase.Operations2;
                    if (operations2 != null)
                    {
                        invocations.Add(BatchWriterUtils.EndChangeSet());
                        invocations.Add(BatchWriterUtils.StartChangeSet());

                        for (int i = 0; i < operations2.Length; ++i)
                        {
                            invocations.Add(operations2[i]);
                        }
                    }

                    invocations.Add(BatchWriterUtils.EndChangeSet());
                    invocations.Add(BatchWriterUtils.EndBatch());

                    var expectedException = testCase.BaseUri == null ? testCase.ExpectedExceptionNoBaseUri : testCase.ExpectedException;
                    return expectedException == null
                        ? new BatchWriterTestDescriptor(this.Settings, invocations.ToArray(), (Dictionary<string, string>)null, testCase.BaseUri)
                        : new BatchWriterTestDescriptor(this.Settings, invocations.ToArray(), expectedException);
                };

            IEnumerable<BatchWriterTestDescriptor> testDescriptors = testCases.Select(tc => createChangeSetRequest(tc));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent.Where(tc => tc.IsRequest),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => false),
                (testDescriptor, testConfig, payloadTestConfig) =>
                {
                    testConfig = ModifyBatchTestConfig(testConfig, testDescriptor);
                    payloadTestConfig = ModifyPayloadTestConfig(payloadTestConfig, testDescriptor.BaseUri);
                    BatchWriterUtils.WriteAndVerifyBatchPayload(testDescriptor, testConfig, payloadTestConfig, this.Assert);
                });
        }

        // [TestMethod, Variation(Description = "Tests various cross referencing links in changeset operation payloads.")]
        public void ODataBatchWriterCrossReferencingLinksInPayloadTest()
        {
            Uri baseUri = new Uri("http://odata.org/base");

            // the expected entry result (ATOM and JSON)
            string entryPayloadExpectedAtomResultTemplate =
                string.Join(
                "$(NL)",
                @"<entry {0}xmlns=""" + TestAtomConstants.AtomNamespace + @""" xmlns:d=""" + TestAtomConstants.ODataNamespace + @""" xmlns:m=""" + TestAtomConstants.ODataMetadataNamespace + @""" xmlns:georss=""" + TestAtomConstants.GeoRssNamespace + @""" xmlns:gml=""" + TestAtomConstants.GmlNamespace + @""">",
                @"  <id>" + ObjectModelUtils.DefaultEntryId + "</id>",
                @"  <link rel=""self"" href=""{1}"" />",
                @"  <title />",
                @"  <updated>" + ObjectModelUtils.DefaultEntryUpdated + "</updated>",
                @"  <author>",
                @"    <name />",
                @"  </author>",
                @"  <content type=""application/xml"" />",
                @"</entry>");

            string[] entryPayloadExpectedJsonResultTemplate = new string[]
            {
                "{{",
                "$(Indent)\"__metadata\":{{",
                "$(Indent)$(Indent)\"id\":\"http://www.odata.org/entryid\",\"uri\":\"{0}\"",
                "$(Indent)}}",
                "}}"
            };

            Func<Uri, string, WriterTestConfiguration, BatchWriterUtils.ODataPayload> createODataPayload = (uri, expectedUri, testConfig) =>
                {
                    Debug.Assert(!uri.IsAbsoluteUri, "Expected a relative Uri.");
                    ODataResource sampleEntry = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
                    sampleEntry.ReadLink = uri;
                    ODataItem[] entryPayload = new ODataItem[] { sampleEntry };

                    Uri testCaseBaseUri = testConfig.MessageWriterSettings.BaseUri;

                    string expectedResult = null;
                    if (testConfig.Format == ODataFormat.Json)
                    {
                        // construct the expected Uri from the payload URI
                        if (expectedUri == null)
                        {
                            // in JSON we expect an absolute URI in the payload; otherwise keep the relative one (but it will fail).
                            if (testCaseBaseUri != null)
                            {
                                expectedUri = new Uri(testCaseBaseUri, uri).AbsoluteUri;
                            }
                            else
                            {
                                expectedUri = uri.OriginalString;
                            }
                        }

                        expectedResult = JsonUtils.WrapTopLevelValue(testConfig, entryPayloadExpectedJsonResultTemplate);
                        expectedResult = string.Format(CultureInfo.InvariantCulture, expectedResult, expectedUri);
                    }

                    testConfig = SetAcceptableHeaders(testConfig);

                    return new BatchWriterUtils.ODataPayload
                    {
                        Items = new ODataItem[] { sampleEntry },
                        TestConfiguration = testConfig,
                        ExpectedResult = expectedResult,
                    };
                };

            // Start with test cases that use the cross-referencing link in the body of the payload and have a base URI
            IEnumerable<Func<WriterTestConfiguration, Uri, string, CrossReferencingTestCase>> testCaseFuncs = new Func<WriterTestConfiguration, Uri, string, CrossReferencingTestCase>[]
                {
                    (testConfig, testCaseBaseUri, payloadString) => new CrossReferencingTestCase
                    {
                        DebugDescription = "Valid cross-reference link in payload (reference only).",
                        BaseUri = testCaseBaseUri,
                        Operations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://odata.org/1"), "1", /*headers*/ null, payloadString),
                            BatchWriterUtils.ChangeSetRequest(
                                "POST",
                                new Uri("http://odata.org/2"),
                                "2",
                                GetExpectedHeadersForFormat(testConfig.Format, ODataPayloadKind.Resource),
                                createODataPayload(new Uri("$1", UriKind.Relative), "$1", testConfig)),
                        },
                    },
                    (testConfig, testCaseBaseUri, payloadString) => new CrossReferencingTestCase
                    {
                        DebugDescription = "Valid cross-reference link in payload (segement after reference).",
                        BaseUri = testCaseBaseUri,
                        Operations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://odata.org/1"), "1", /*headers*/ null, payloadString),
                            BatchWriterUtils.ChangeSetRequest(
                                "POST",
                                new Uri("http://odata.org/2"),
                                "2",
                                GetExpectedHeadersForFormat(testConfig.Format, ODataPayloadKind.Resource),
                                createODataPayload(new Uri("$1/foo", UriKind.Relative), "$1/foo", testConfig)),
                        },
                    },
                    (testConfig, testCaseBaseUri, payloadString) => new CrossReferencingTestCase
                    {
                        DebugDescription = "Valid cross-reference link in payload (special name '').",
                        BaseUri = testCaseBaseUri,
                        Operations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://odata.org/1"), "", /*headers*/ null, payloadString),
                            BatchWriterUtils.ChangeSetRequest(
                                "POST",
                                new Uri("http://odata.org/2"),
                                "2",
                                GetExpectedHeadersForFormat(testConfig.Format, ODataPayloadKind.Resource),
                                createODataPayload(new Uri("$/foo", UriKind.Relative), "$/foo", testConfig)),
                        },
                    },
                    (testConfig, testCaseBaseUri, payloadString) => new CrossReferencingTestCase
                    {
                        DebugDescription = "Valid cross-reference link in payload (special name '$$$').",
                        BaseUri = testCaseBaseUri,
                        Operations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://odata.org/1"), "$$$", /*headers*/ null, payloadString),
                            BatchWriterUtils.ChangeSetRequest(
                                "POST",
                                new Uri("http://odata.org/2"),
                                "2",
                                GetExpectedHeadersForFormat(testConfig.Format, ODataPayloadKind.Resource),
                                createODataPayload(new Uri("$$$$", UriKind.Relative), "$$$$", testConfig)),
                        },
                    },
                    (testConfig, testCaseBaseUri, payloadString) => new CrossReferencingTestCase
                    {
                        DebugDescription = "Invalid cross-reference link in payload (content ID does not exist).",
                        BaseUri = testCaseBaseUri,
                        Operations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://odata.org/1"), "1", /*headers*/ null, payloadString),
                            BatchWriterUtils.ChangeSetRequest(
                                "POST",
                                new Uri("http://odata.org/2"),
                                "2",
                                GetExpectedHeadersForFormat(testConfig.Format, ODataPayloadKind.Resource),
                                createODataPayload(new Uri("$NonExisting", UriKind.Relative), null, testConfig)),
                        },
                        ExpectedExceptionNoBaseUri = ODataExpectedExceptions.ODataException("ODataWriter_RelativeUriUsedWithoutBaseUriSpecified", "$NonExisting"),
                    },
                    (testConfig, testCaseBaseUri, payloadString) => new CrossReferencingTestCase
                    {
                        DebugDescription = "Invalid cross-reference link in payload (content ID will only appear later in the batch).",
                        BaseUri = testCaseBaseUri,
                        Operations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest(
                                "POST",
                                new Uri("http://odata.org/1"),
                                "1",
                                GetExpectedHeadersForFormat(testConfig.Format, ODataPayloadKind.Resource),
                                createODataPayload(new Uri("$2", UriKind.Relative), null, testConfig)),
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://odata.org/2"), "2", /*headers*/ null, payloadString),
                        },
                        ExpectedExceptionNoBaseUri = ODataExpectedExceptions.ODataException("ODataWriter_RelativeUriUsedWithoutBaseUriSpecified", "$2"),
                    },
                    (testConfig, testCaseBaseUri, payloadString) => new CrossReferencingTestCase
                    {
                        DebugDescription = "Invalid cross-reference link in payload (reference content ID in different changeset).",
                        BaseUri = testCaseBaseUri,
                        Operations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://odata.org/1"), "1", /*headers*/ null, payloadString),
                        },
                        Operations2 = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest(
                                "POST",
                                new Uri("http://odata.org/2"),
                                "2",
                                GetExpectedHeadersForFormat(testConfig.Format, ODataPayloadKind.Resource),
                                createODataPayload(new Uri("$1", UriKind.Relative), null, testConfig)),
                        },
                        ExpectedExceptionNoBaseUri = ODataExpectedExceptions.ODataException("ODataWriter_RelativeUriUsedWithoutBaseUriSpecified", "$1"),
                    },
                    (testConfig, testCaseBaseUri, payloadString) => new CrossReferencingTestCase
                    {
                        DebugDescription = "Invalid cross-reference link in payload (reference current content ID).",
                        BaseUri = testCaseBaseUri,
                        Operations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://odata.org/1"), "1", /*headers*/ null, payloadString),
                        },
                        Operations2 = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                        {
                            BatchWriterUtils.ChangeSetRequest(
                                "POST",
                                new Uri("http://odata.org/2"),
                                "2",
                                GetExpectedHeadersForFormat(testConfig.Format, ODataPayloadKind.Resource),
                                createODataPayload(new Uri("$2", UriKind.Relative), null, testConfig)),
                        },
                        ExpectedExceptionNoBaseUri = ODataExpectedExceptions.ODataException("ODataWriter_RelativeUriUsedWithoutBaseUriSpecified", "$2"),
                    },
                };

            Func<CrossReferencingTestCase, BatchWriterTestDescriptor> createChangeSetRequest = (testCase) =>
            {
                var invocations = new List<BatchWriterTestDescriptor.InvocationAndOperationDescriptor>();
                invocations.Add(BatchWriterUtils.StartBatch());
                invocations.Add(BatchWriterUtils.StartChangeSet());

                var operations = testCase.Operations;
                for (int i = 0; i < operations.Length; ++i)
                {
                    invocations.Add(operations[i]);
                }

                var operations2 = testCase.Operations2;
                if (operations2 != null)
                {
                    invocations.Add(BatchWriterUtils.EndChangeSet());
                    invocations.Add(BatchWriterUtils.StartChangeSet());

                    for (int i = 0; i < operations2.Length; ++i)
                    {
                        invocations.Add(operations2[i]);
                    }
                }

                invocations.Add(BatchWriterUtils.EndChangeSet());
                invocations.Add(BatchWriterUtils.EndBatch());

                Dictionary<string, string> changeSetHeaders = null;
                var expectedException = testCase.BaseUri == null ? testCase.ExpectedExceptionNoBaseUri : testCase.ExpectedException;
                return expectedException == null
                    ? new BatchWriterTestDescriptor(this.Settings, invocations.ToArray(), changeSetHeaders, testCase.BaseUri)
                    : new BatchWriterTestDescriptor(this.Settings, invocations.ToArray(), expectedException);
            };

            string[] payloadStrings = new string[]
            {
                "Non-empty dummy payload",
                string.Empty,
                null
            };

            //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testCaseFuncs,
                new Uri[] { baseUri, null },
                payloadStrings,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent.Where(tc => tc.IsRequest),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => false),
                (testCaseFunc, testBaseUri, payloadString, testConfig, payloadTestConfig) =>
                {
                    payloadTestConfig = ModifyPayloadTestConfig(payloadTestConfig, testBaseUri);
                    var testCase = testCaseFunc(payloadTestConfig, testBaseUri, payloadString);
                    BatchWriterTestDescriptor testDescriptor = createChangeSetRequest(testCase);
                    BatchWriterUtils.WriteAndVerifyBatchPayload(testDescriptor, testConfig, payloadTestConfig, this.Assert);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Tests the proper behavior of the batch writer if user code throws an exception while the batch writer is being used/active.")]
        public void ODataBatchWriterUserExceptionInRequestTests()
        {
            var testCases = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[][]
            {
                new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.UserException(),
                },

                new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.StartBatch(),
                    BatchWriterUtils.UserException(),
                },

                new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.StartBatch(),
                    BatchWriterUtils.QueryOperation("GET", new Uri("http://www.odata.org/")),
                    BatchWriterUtils.UserException(),
                },

                new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.StartBatch(),
                    BatchWriterUtils.QueryOperation("GET", new Uri("http://www.odata.org/")),
                    BatchWriterUtils.EndBatch(),
                    BatchWriterUtils.UserException(),
                },

                new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.StartBatch(),
                    BatchWriterUtils.UserException(),
                },

                new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.StartBatch(),
                    BatchWriterUtils.StartChangeSet(),
                    BatchWriterUtils.UserException(),
                },

                new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.StartBatch(),
                    BatchWriterUtils.StartChangeSet(),
                    BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://www.odata.org/OData/OData.svc/Products"), "1", null, "First sample payload."),
                    BatchWriterUtils.UserException(),
                },

                new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.StartBatch(),
                    BatchWriterUtils.StartChangeSet(),
                    BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://www.odata.org/OData/OData.svc/Products"), "1", null, "First sample payload."),
                    BatchWriterUtils.EndChangeSet(),
                    BatchWriterUtils.UserException(),
                },

                new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.StartBatch(),
                    BatchWriterUtils.StartChangeSet(),
                    BatchWriterUtils.ChangeSetRequest("POST", new Uri("http://www.odata.org/OData/OData.svc/Products"), "1", null, "First sample payload."),
                    BatchWriterUtils.EndChangeSet(),
                    BatchWriterUtils.EndBatch(),
                    BatchWriterUtils.UserException(),
                },
            };

            Exception expectedException = new Exception("User code triggered an exception.");

            var testDescriptors = testCases.Select(tc => new BatchWriterTestDescriptor(this.Settings, tc, expectedException));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent.Where(tc => tc.IsRequest),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => tc.IsRequest),
                (testDescriptor, testConfig, payloadTestConfig) =>
                {
                    testConfig = ModifyBatchTestConfig(testConfig, testDescriptor);
                    payloadTestConfig = ModifyPayloadTestConfig(payloadTestConfig, testDescriptor.BaseUri);
                    BatchWriterUtils.WriteAndVerifyBatchPayload(testDescriptor, testConfig, payloadTestConfig, this.Assert);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Tests the proper behavior of the batch writer if user code throws an exception while the batch writer is being used/active.")]
        public void ODataBatchWriterUserExceptionInResponseTests()
        {
            var testCases = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[][]
            {
                new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.UserException(),
                },

                new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.StartBatch(),
                    BatchWriterUtils.UserException(),
                },

                new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.StartBatch(),
                    BatchWriterUtils.QueryOperationResponse(200, string.Empty),
                    BatchWriterUtils.UserException(),
                },

                new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.StartBatch(),
                    BatchWriterUtils.QueryOperationResponse(200, string.Empty),
                    BatchWriterUtils.EndBatch(),
                    BatchWriterUtils.UserException(),
                },

                new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.StartBatch(),
                    BatchWriterUtils.UserException(),
                },

                new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.StartBatch(),
                    BatchWriterUtils.StartChangeSet(),
                    BatchWriterUtils.UserException(),
                },

                new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.StartBatch(),
                    BatchWriterUtils.StartChangeSet(),
                    BatchWriterUtils.ChangeSetResponse(200, "Sample payload"),
                    BatchWriterUtils.UserException(),
                },

                new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.StartBatch(),
                    BatchWriterUtils.StartChangeSet(),
                    BatchWriterUtils.ChangeSetResponse(200, "Sample payload"),
                    BatchWriterUtils.EndChangeSet(),
                    BatchWriterUtils.UserException(),
                },

                new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
                {
                    BatchWriterUtils.StartBatch(),
                    BatchWriterUtils.StartChangeSet(),
                    BatchWriterUtils.ChangeSetResponse(200, "Sample payload"),
                    BatchWriterUtils.EndChangeSet(),
                    BatchWriterUtils.EndBatch(),
                    BatchWriterUtils.UserException(),
                },
            };

            Exception expectedException = new Exception("User code triggered an exception.");

            var testDescriptors = testCases.Select(tc => new BatchWriterTestDescriptor(this.Settings, tc, expectedException));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent.Where(tc => !tc.IsRequest),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfig, payloadTestConfig) =>
                {
                    testConfig = ModifyBatchTestConfig(testConfig, testDescriptor);
                    payloadTestConfig = ModifyPayloadTestConfig(payloadTestConfig, testDescriptor.BaseUri);
                    BatchWriterUtils.WriteAndVerifyBatchPayload(testDescriptor, testConfig, payloadTestConfig, this.Assert);
                });
        }

        // [TestMethod, Variation(Description = "Tests writing of sample OData payloads.")]
        public void ODataBatchWriterODataPayloadSmokeTests()
        {
            // Create OData payloads
            ODataResource sampleEntry = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
            ODataResourceSet sampleFeed = ObjectModelUtils.CreateDefaultFeedWithAtomMetadata();
            ODataAnnotatedError sampleError = ObjectModelUtils.CreateDefaultError(true);

            ODataItem[] entryPayload = new ODataItem[] { sampleEntry };
            ODataItem[] feedPayload = new ODataItem[] { sampleFeed, sampleEntry };
            ODataAnnotatedError[] errorPayload = new ODataAnnotatedError[] { sampleError };

            // the expected entry result (JSON)
            string[] entryPayloadExpectedJsonResult = new string[]
            {
                "{",
                "$(Indent)\"__metadata\":{",
                "$(Indent)$(Indent)\"id\":\"http://www.odata.org/entryid\",\"uri\":\"http://www.odata.org/entry/readlink\"",
                "$(Indent)}",
                "}"
            };

            // create the expected feed result (JSON)
            string[] feedPayloadExpectedJsonResult = new string[]
            {
                "[",
                "$(Indent){",
                "$(Indent)$(Indent)\"__metadata\":{",
                "$(Indent)$(Indent)$(Indent)\"id\":\"http://www.odata.org/entryid\",\"uri\":\"http://www.odata.org/entry/readlink\"",
                "$(Indent)$(Indent)}",
                "$(Indent)}",
                "]"
            };

            // create the expected error result (JSON)
            string[] errorPayloadExpectedJsonResult = new string[]
            {
                "{",
                "$(Indent)\"error\":{",
                "$(Indent)$(Indent)\"code\":\"Default error code\",\"message\":{",
                "$(Indent)$(Indent)$(Indent)\"lang\":\"Default error message language.\",\"value\":\"Default error message.\"",
                "$(Indent)$(Indent)},\"innererror\":{",
                "$(Indent)$(Indent)$(Indent)\"message\":\"Default inner error.\",\"type\":\"\",\"stacktrace\":\"\"",
                "$(Indent)$(Indent)}",
                "$(Indent)}",
                "}",
            };

            // create the various query and changeset batches
            var testCases = new Func<WriterTestConfiguration, BatchTestWithDirection>[]
            {
                testConfig =>
                    {
                        // entry payload of query operation response with 200  response code
                        string expectedResult = JsonUtils.WrapTopLevelValue(testConfig, entryPayloadExpectedJsonResult);

                        testConfig = SetAcceptableHeaders(testConfig);

                        return new BatchTestWithDirection
                        {
                            Batch = BatchWriterUtils.CreateQueryResponseBatch(
                                200,
                                new BatchWriterUtils.ODataPayload() { Items = entryPayload, TestConfiguration = testConfig, ExpectedResult = expectedResult },
                                GetExpectedHeadersForFormat(testConfig.Format, ODataPayloadKind.Resource)),
                            ForRequests = false
                        };
                    },
                testConfig =>
                    {
                        // feed payload of query operation response with 200  response code
                        string expectedResult = JsonUtils.WrapTopLevelResults(testConfig, feedPayloadExpectedJsonResult);

                        testConfig = SetAcceptableHeaders(testConfig);

                        return new BatchTestWithDirection
                        {
                            Batch = BatchWriterUtils.CreateQueryResponseBatch(
                                200,
                                new BatchWriterUtils.ODataPayload() { Items = feedPayload, TestConfiguration = testConfig, ExpectedResult = expectedResult },
                                GetExpectedHeadersForFormat(testConfig.Format, ODataPayloadKind.ResourceSet)),
                            ForRequests = false
                        };
                    },
                testConfig =>
                    {
                        // error payload of query operation response with 200  response code
                        string expectedResult = string.Join("$(NL)", errorPayloadExpectedJsonResult);

                        testConfig = SetAcceptableHeaders(testConfig);

                        return new BatchTestWithDirection
                        {
                            Batch = BatchWriterUtils.CreateQueryResponseBatch(
                                200,
                                new BatchWriterUtils.ODataPayload() { Items = errorPayload, TestConfiguration = testConfig, ExpectedResult = expectedResult },
                                GetExpectedHeadersForFormat(testConfig.Format, ODataPayloadKind.Error)),
                            ForRequests = false
                        };
                    },
                testConfig =>
                    {
                        // changeset request with entry payload
                        string expectedResult = JsonUtils.WrapTopLevelValue(testConfig, entryPayloadExpectedJsonResult);

                        testConfig = SetAcceptableHeaders(testConfig);

                        return new BatchTestWithDirection
                        {
                            Batch = BatchWriterUtils.CreateChangeSetRequestBatch(
                                "POST",
                                new Uri("http://services.odata.org/OData/OData.svc/Products"),
                                new BatchWriterUtils.ODataPayload() { Items = entryPayload, TestConfiguration = testConfig, ExpectedResult = expectedResult },
                                GetExpectedHeadersForFormat(testConfig.Format, ODataPayloadKind.Resource)
                                ),
                            ForRequests = true
                        };
                    },
                testConfig =>
                    {
                        // changeset response with entry payload
                        string expectedResult = JsonUtils.WrapTopLevelValue(testConfig, entryPayloadExpectedJsonResult);

                        testConfig = SetAcceptableHeaders(testConfig);

                        return new BatchTestWithDirection
                        {
                            Batch = BatchWriterUtils.CreateChangeSetResponseBatch(
                                200,
                                new BatchWriterUtils.ODataPayload() { Items = entryPayload, TestConfiguration = testConfig, ExpectedResult = expectedResult },
                                GetExpectedHeadersForFormat(testConfig.Format, ODataPayloadKind.Resource)
                                ),
                            ForRequests = false
                        };
                    },
                testConfig =>
                    {
                        // changeset response with error payload
                        string expectedResult = string.Join("$(NL)", errorPayloadExpectedJsonResult);

                        testConfig = SetAcceptableHeaders(testConfig);

                        return new BatchTestWithDirection
                        {
                            Batch = BatchWriterUtils.CreateChangeSetResponseBatch(
                                200,
                                new BatchWriterUtils.ODataPayload() { Items = errorPayload, TestConfiguration = testConfig, ExpectedResult = expectedResult },
                                GetExpectedHeadersForFormat(testConfig.Format, ODataPayloadKind.Error)),
                            ForRequests = false
                        };
                    },
            };

            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            // Write everything to the batch (ATOM + JSON)
            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => false),
                (testCase, batchTestConfig, payloadTestConfig) =>
                {
                    if (batchTestConfig.IsRequest != payloadTestConfig.IsRequest)
                    {
                        return;
                    }

                    BatchTestWithDirection testWithDirection = testCase(payloadTestConfig);

                    if (batchTestConfig.IsRequest != testWithDirection.ForRequests)
                    {
                        return;
                    }

                    var testDescriptor = new BatchWriterTestDescriptor(this.Settings, testWithDirection.Batch, (Dictionary<string, string>)null);
                    BatchWriterUtils.WriteAndVerifyBatchPayload(testDescriptor, batchTestConfig, payloadTestConfig, this.Assert);
                });
        }

        /// <summary>
        /// Set the acceptable headers for the batch operations to make sure we pick the same encoding default as for the batch structure;
        /// since our validator does not create new readers for the operations we cannot switch encodings. This is necessary because we currently
        /// always pick UTF-8 (without BOM) as default for the batch structure but the Xml writer for writing operations in ATOM uses UTF-8 (with BOM).
        /// </summary>
        /// <param name="testConfig">The test configuration to set the headers on.</param>
        /// <returns>A cloned test configuration with the proper accept headers.</returns>
        private static WriterTestConfiguration SetAcceptableHeaders(WriterTestConfiguration testConfig)
        {
            if (testConfig.MessageWriterSettings == null)
            {
                return testConfig;
            }

            if (testConfig.Format == ODataFormat.Json)
            {
                ODataMessageWriterSettings newSettings = testConfig.MessageWriterSettings.Clone();
                string acceptableContentTypes = MimeTypes.ApplicationJson;
                newSettings.SetContentType(acceptableContentTypes, null);
                return new WriterTestConfiguration(testConfig.Format, newSettings, testConfig.IsRequest, testConfig.Synchronous);
            }

            if (testConfig.Format == null)
            {
                return testConfig;
            }

            throw new NotSupportedException("Unsupported format " + testConfig.Format.ToString() + " found.");
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Tests different max sizes for change sets and the expected error behavior.")]
        public void ODataBatchWriterChangeSetSizeTests()
        {
            Func<int, int[], int?, ExpectedException, BatchWriterTestDescriptor> createChangeSetBatch =
                (changeSetCount, operationsPerChangeSet, maxOperationsPerChangeSet, expectedException) =>
                {
                    var invocations = BatchWriterUtils.CreateDefaultChangeSetBatch(changeSetCount, operationsPerChangeSet);
                    return expectedException == null
                        ? new BatchWriterTestDescriptor(this.Settings, invocations, null, maxOperationsPerChangeSet)
                        : new BatchWriterTestDescriptor(this.Settings, invocations, null, maxOperationsPerChangeSet, expectedException);
                };

            var testCases = new[]
            {
                new
                {
                    MaxOperationsPerChangeSet = (int?)null,
                    ChangeSetCount = 0,
                    OperationsPerChangeSet = new int[0],
                     ExpectedException = (ExpectedException)null
                },
                new
                {
                    MaxOperationsPerChangeSet = (int?)null,
                    ChangeSetCount = 1,
                    OperationsPerChangeSet = new int[] { 1 },
                   ExpectedException = (ExpectedException)null
               },
                new
                {
                    MaxOperationsPerChangeSet = (int?)null,
                    ChangeSetCount = 1,
                    OperationsPerChangeSet = new int[] { 3 },
                   ExpectedException = (ExpectedException)null
               },
                new
                {
                    MaxOperationsPerChangeSet = (int?)null,
                    ChangeSetCount = 2,
                    OperationsPerChangeSet = new int[] { 0, 0 },
                   ExpectedException = (ExpectedException)null
               },
                new
                {
                    MaxOperationsPerChangeSet = (int?)null,
                    ChangeSetCount = 2,
                    OperationsPerChangeSet = new int[] { 1, 2 },
                   ExpectedException = (ExpectedException)null
               },
                new
                {
                    MaxOperationsPerChangeSet = (int?)1,
                    ChangeSetCount = 1,
                    OperationsPerChangeSet = new int[] { 2 },
                   ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchWriter_MaxChangeSetSizeExceeded", "1")
               },
                new
                {
                    MaxOperationsPerChangeSet = (int?)0,
                    ChangeSetCount = 1,
                    OperationsPerChangeSet = new int[] { 1 },
                   ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchWriter_MaxChangeSetSizeExceeded", "0")
               },
                new
                {
                    MaxOperationsPerChangeSet = (int?)2,
                    ChangeSetCount = 1,
                    OperationsPerChangeSet = new int[] { 2 },
                   ExpectedException = (ExpectedException)null
               },
                new
                {
                    MaxOperationsPerChangeSet = (int?)0,
                    ChangeSetCount = 0,
                    OperationsPerChangeSet = new int[0],
                   ExpectedException = (ExpectedException)null
               },
            };


            var testDescriptors = testCases.Select(tc => createChangeSetBatch(tc.ChangeSetCount, tc.OperationsPerChangeSet, tc.MaxOperationsPerChangeSet, tc.ExpectedException));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent.Where(tc => tc.IsRequest),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => tc.IsRequest),
                (testDescriptor, testConfig, payloadTestConfig) =>
                {
                    testConfig = ModifyBatchTestConfig(testConfig, testDescriptor);
                    payloadTestConfig = ModifyPayloadTestConfig(payloadTestConfig, testDescriptor.BaseUri);
                    BatchWriterUtils.WriteAndVerifyBatchPayload(testDescriptor, testConfig, payloadTestConfig, this.Assert);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Tests different max sizes for batches and the expected error behavior.")]
        public void ODataBatchWriterBatchSizeTests()
        {
            Func<int, int?, ExpectedException, BatchWriterTestDescriptor> createQueryBatch =
               (queryCount, maxPartsPerBatch, expectedException) =>
               {
                   var invocations = BatchWriterUtils.CreateDefaultQueryBatch(queryCount);
                   return expectedException == null
                      ? new BatchWriterTestDescriptor(this.Settings, invocations, maxPartsPerBatch, null)
                      : new BatchWriterTestDescriptor(this.Settings, invocations, maxPartsPerBatch, null, expectedException);
               };

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
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchWriter_MaxBatchSizeExceeded", "0")
                },
                new
                {
                    MaxPartsPerBatch = (int?)1,
                    QueryCount = 2,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchWriter_MaxBatchSizeExceeded", "1")
                },
                new
                {
                    MaxPartsPerBatch = (int?)2,
                    QueryCount = 5,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataBatchWriter_MaxBatchSizeExceeded", "2")
                },
            };


            var testDescriptors = testCases.Select(tc => createQueryBatch(tc.QueryCount, tc.MaxPartsPerBatch, tc.ExpectedException));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent.Where(tc => tc.IsRequest),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => tc.IsRequest),
                (testDescriptor, testConfig, payloadTestConfig) =>
                {
                    testConfig = ModifyBatchTestConfig(testConfig, testDescriptor);
                    payloadTestConfig = ModifyPayloadTestConfig(payloadTestConfig, testDescriptor.BaseUri);
                    BatchWriterUtils.WriteAndVerifyBatchPayload(testDescriptor, testConfig, payloadTestConfig, this.Assert);
                });
        }

        // [TestMethod, Variation(Description = "Tests a variety of batch payload shapes.")]
        // ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
        public void ODataBatchWithPayloadTests()
        {
            EdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            var complexvalue = PayloadBuilder.ComplexValue("TestModel.Address");
            complexvalue.PrimitiveProperty("Street", "1234 Redmond Way");
            complexvalue.PrimitiveProperty("Zip", 12345);
            complexvalue.WithTypeAnnotation(model.FindDeclaredType("TestModel.Address"));

            var payload = new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.Property("propertyName", complexvalue),
                PayloadEdmModel = model, //TODO: add clone model

            };

            var testDescriptors = new List<PayloadWriterTestDescriptor>();

            foreach (var payloadDescriptor in this.PayloadGenerator.GeneratePayloads(payload))
            {
                bool isGenerated = !object.ReferenceEquals(payloadDescriptor, payload);
                var currentPayload = payloadDescriptor.PayloadElement.DeepCopy();
                if (currentPayload is BatchResponsePayload)
                {
                    currentPayload.AddAnnotation(new ODataBatchResponseRequestAnnotation() { BatchRequest = this.RequestGenerator.GenerateRequestPayload((BatchResponsePayload)currentPayload, payloadDescriptor.PayloadModel) });
                }
                testDescriptors.Add(new BatchPayloadWriterTestDescriptor<ODataPayloadElement>(this.PayloadWriterSettings, currentPayload)
                {
                    PayloadDescriptor = payloadDescriptor,
                    IsGeneratedPayload = isGenerated,
                    ExpectedResultCallback = (tc) =>
                    {

                        return new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                        {
                            ExpectedPayload = currentPayload.DeepCopy(),
                        };
                    },
                    PayloadElement = currentPayload,
                    SkipTestConfiguration = payloadDescriptor.SkipTestConfiguration
                });
            }

            this.CombinatorialEngineProvider.RunCombinations(
               testDescriptors,
               this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent.Where(tc => tc.MessageWriterSettings.EnableMessageStreamDisposal == true),
               (testDescriptor, testConfiguration) =>
               {
                   testDescriptor.RunTest(testConfiguration, this.Logger);
               });
        }

        [TestMethod, Variation(Description = "Tests different max sizes for batches and the expected error behavior.")]
        public void ODataBatchWriterErrorTests()
        {
            var reuseMessageException = ODataExpectedExceptions.ODataException("ODataMessageWriter_WriterAlreadyUsed");
            var callStartTwiceException = ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromBatchStarted");
            var callEndTwiceException = ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromBatchCompleted");
            var nestedChangeset = ODataExpectedExceptions.ODataException("ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet");
            var endBatchBeforeEndChangesetException = ODataExpectedExceptions.ODataException("ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet");
            var streamDisposedException = ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested");
            var startChangesetWithoutBatchException = ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromStart");

            Tuple<ExpectedException, Action<WriterTestConfiguration, TestMessage>>[] cases = new Tuple<ExpectedException, Action<WriterTestConfiguration, TestMessage>>[]
            {
                new Tuple<ExpectedException, Action<WriterTestConfiguration, TestMessage>>(reuseMessageException, this.ReuseMessage),
                new Tuple<ExpectedException, Action<WriterTestConfiguration, TestMessage>>(callStartTwiceException, this.CallWriteStartTwice),
                new Tuple<ExpectedException, Action<WriterTestConfiguration, TestMessage>>(callEndTwiceException, this.CallWriteEndTwice),
                new Tuple<ExpectedException, Action<WriterTestConfiguration, TestMessage>>(nestedChangeset, this.CallWriteStartChangesetTwice),
                new Tuple<ExpectedException, Action<WriterTestConfiguration, TestMessage>>(endBatchBeforeEndChangesetException, this.CallEndBatchBeforeEndChangeset),
                new Tuple<ExpectedException, Action<WriterTestConfiguration, TestMessage>>(streamDisposedException, this.WriteWhenSetToStreamDispose),
                new Tuple<ExpectedException, Action<WriterTestConfiguration, TestMessage>>(startChangesetWithoutBatchException, this.StartChangesetWithoutStartingBatch)
            };

            this.CombinatorialEngineProvider.RunCombinations(
                cases,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent.Where(tc => tc.IsRequest && tc.Synchronous),
                (testCase, testConfig) =>
                {
                    var innerStream = new MemoryStream();
                    TestStream messageStream = new TestStream(innerStream);
                    TestWriterUtils.SetFailAsynchronousCalls(messageStream, testConfig.Synchronous);

                    TestMessage testMessage = TestWriterUtils.CreateOutputMessageFromStream(messageStream, testConfig, ODataPayloadKind.Batch, "batchboundary", new TestUrlResolver());
                    Exception exception = TestExceptionUtils.RunCatching(() =>
                    {
                        testCase.Item2(testConfig.Clone(), testMessage);
                    });

                    var match = testCase.Item1.ExpectedMessage.Verifier.IsMatch(testCase.Item1.ExpectedMessage.ResourceIdentifier, exception.Message, testCase.Item1.ExpectedMessage.Arguments);
                    ExceptionUtilities.Assert(match, "Recieved unexpected error message.");
                });
        }

        /// <summary>
        /// This method reuses the message stream to cause a failure.
        /// </summary>
        /// <param name="newTestConfig">The configuration to use</param>
        /// <param name="testMessage">The message to write to</param>
        private void ReuseMessage(WriterTestConfiguration config, TestMessage testMessage)
        {
            ExceptionUtilities.CheckArgumentNotNull(config, "config");
            ExceptionUtilities.CheckArgumentNotNull(testMessage, "testMessage");

            using (ODataMessageWriterTestWrapper messageWriterWrapper = TestWriterUtils.CreateMessageWriter(testMessage, null, config, this.Assert, null))
            {
                var batchWriter = messageWriterWrapper.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();
                var newMessage = batchWriter.CreateOperationRequestMessage("GET", new Uri("http://www.odata.org"));
                // This line should fail
                messageWriterWrapper.CreateODataResourceWriter();
            }
        }

        /// <summary>
        /// This method calls write start twice
        /// </summary>
        /// <param name="newTestConfig">The config to use</param>
        /// <param name="testMessage">The message to use</param>
        private void CallWriteStartTwice(WriterTestConfiguration config, TestMessage testMessage)
        {
            ExceptionUtilities.CheckArgumentNotNull(config, "config");
            ExceptionUtilities.CheckArgumentNotNull(testMessage, "testMessage");

            using (ODataMessageWriterTestWrapper messageWriterWrapper = TestWriterUtils.CreateMessageWriter(testMessage, null, config, this.Assert, null))
            {
                var batchWriter = messageWriterWrapper.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();
                // This line should fail
                batchWriter.WriteStartBatch();
            }
        }

        /// <summary>
        /// This method calls write end twice
        /// </summary>
        /// <param name="newTestConfig">The config to use</param>
        /// <param name="testMessage">The message to use</param>
        private void CallWriteEndTwice(WriterTestConfiguration config, TestMessage testMessage)
        {
            ExceptionUtilities.CheckArgumentNotNull(config, "config");
            ExceptionUtilities.CheckArgumentNotNull(testMessage, "testMessage");

            using (ODataMessageWriterTestWrapper messageWriterWrapper = TestWriterUtils.CreateMessageWriter(testMessage, null, config, this.Assert, null))
            {
                var batchWriter = messageWriterWrapper.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();
                batchWriter.WriteEndBatch();
                // This line should fail
                batchWriter.WriteEndBatch();
            }
        }

        /// <summary>
        /// This method calls tries to nest changesets
        /// </summary>
        /// <param name="newTestConfig">The config to use</param>
        /// <param name="testMessage">The message to use</param>
        private void CallWriteStartChangesetTwice(WriterTestConfiguration config, TestMessage testMessage)
        {
            ExceptionUtilities.CheckArgumentNotNull(config, "config");
            ExceptionUtilities.CheckArgumentNotNull(testMessage, "testMessage");

            using (ODataMessageWriterTestWrapper messageWriterWrapper = TestWriterUtils.CreateMessageWriter(testMessage, null, config, this.Assert, null))
            {
                var batchWriter = messageWriterWrapper.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();
                batchWriter.WriteStartChangeset();
                // This line should fail
                batchWriter.WriteStartChangeset();
            }
        }

        /// <summary>
        /// This method calls tries to end the batch before ending the changeset
        /// </summary>
        /// <param name="newTestConfig">The config to use</param>
        /// <param name="testMessage">The message to use</param>
        private void CallEndBatchBeforeEndChangeset(WriterTestConfiguration config, TestMessage testMessage)
        {
            ExceptionUtilities.CheckArgumentNotNull(config, "config");
            ExceptionUtilities.CheckArgumentNotNull(testMessage, "testMessage");

            using (ODataMessageWriterTestWrapper messageWriterWrapper = TestWriterUtils.CreateMessageWriter(testMessage, null, config, this.Assert, null))
            {
                var batchWriter = messageWriterWrapper.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();
                batchWriter.WriteStartChangeset();
                // This line should fail
                batchWriter.WriteEndBatch();
            }
        }

        /// <summary>
        /// This method tries to write operations with EnableMessageStreamDisposal set to true
        /// </summary>
        /// <param name="newTestConfig">The config to use</param>
        /// <param name="testMessage">The message to use</param>
        private void WriteWhenSetToStreamDispose(WriterTestConfiguration config, TestMessage testMessage)
        {
            ExceptionUtilities.CheckArgumentNotNull(config, "config");
            ExceptionUtilities.CheckArgumentNotNull(testMessage, "testMessage");
            config.MessageWriterSettings.EnableMessageStreamDisposal = false;

            using (ODataMessageWriterTestWrapper messageWriterWrapper = TestWriterUtils.CreateMessageWriter(testMessage, null, config, this.Assert, null))
            {
                var batchWriter = messageWriterWrapper.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();
                batchWriter.WriteStartChangeset();
                var opmessage1 = batchWriter.CreateOperationRequestMessage("PUT", new Uri("http://www.odata.org/Customers"));
                var messageWriterSettings = config.MessageWriterSettings.Clone();
                messageWriterSettings.SetContentType(ODataFormat.Json);
                var messageConfig = new WriterTestConfiguration(ODataFormat.Json, messageWriterSettings, config.IsRequest, config.Synchronous);

                using (ODataMessageWriterTestWrapper opWriterWrapper = TestWriterUtils.CreateMessageWriter(opmessage1.GetStream(), messageConfig, this.Assert, messageWriterSettings, null))
                {
                    var entryWriter = opWriterWrapper.CreateODataResourceWriter();
                    entryWriter.WriteStart(new ODataResource()
                    {
                        Id = new Uri("http://id"),
                        TypeName = "Entry1",
                        EditLink = new Uri("http://www.odata.org/Customers(1)"),
                        ReadLink = new Uri("http://www.odata.org/Customers(1)"),
                        Properties = new ODataProperty[]
                        {
                            new ODataProperty() { Name = "Property", Value= 5 }
                        }
                    });

                    entryWriter.WriteEnd();
                }

                // This line should fail
                var opmessage2 = batchWriter.CreateOperationRequestMessage("POST", new Uri("http://www.odata.org/Customers"));
            }
        }

        /// <summary>
        /// This method calls WriteChangesetStart before WriteBatchStart.
        /// </summary>
        /// <param name="newTestConfig">The config to use</param>
        /// <param name="testMessage">The message to use</param>
        private void StartChangesetWithoutStartingBatch(WriterTestConfiguration config, TestMessage testMessage)
        {
            ExceptionUtilities.CheckArgumentNotNull(config, "config");
            ExceptionUtilities.CheckArgumentNotNull(testMessage, "testMessage");
            config.MessageWriterSettings.EnableMessageStreamDisposal = false;

            using (ODataMessageWriterTestWrapper messageWriterWrapper = TestWriterUtils.CreateMessageWriter(testMessage, null, config, this.Assert, null))
            {
                var batchWriter = messageWriterWrapper.CreateODataBatchWriter();
                // This line should fail
                batchWriter.WriteStartChangeset();
            }
        }

        private static WriterTestConfiguration ModifyBatchTestConfig(WriterTestConfiguration testConfig, BatchWriterTestDescriptor testDescriptor)
        {
            ODataMessageWriterSettings newSettings = null;

            if (testDescriptor.BaseUri != null)
            {
                // clone the message writer settings with a new base Uri
                newSettings = testConfig.MessageWriterSettings.Clone();
                newSettings.BaseUri = testDescriptor.BaseUri;
            }

            if (testDescriptor.MaxPartsPerBatch.HasValue || testDescriptor.MaxOperationsPerChangeset.HasValue)
            {
                if (newSettings == null)
                {
                    newSettings = testConfig.MessageWriterSettings.Clone();
                }

                if (testDescriptor.MaxPartsPerBatch.HasValue)
                {
                    newSettings.MessageQuotas.MaxPartsPerBatch = testDescriptor.MaxPartsPerBatch.Value;
                }

                if (testDescriptor.MaxOperationsPerChangeset.HasValue)
                {
                    newSettings.MessageQuotas.MaxOperationsPerChangeset = testDescriptor.MaxOperationsPerChangeset.Value;
                }
            }

            return newSettings == null
                ? testConfig
                : new WriterTestConfiguration(testConfig.Format, newSettings, testConfig.IsRequest, testConfig.Synchronous);
        }

        private static WriterTestConfiguration ModifyPayloadTestConfig(WriterTestConfiguration testConfig, Uri baseUri)
        {
            ODataMessageWriterSettings newSettings = null;

            if (baseUri != null)
            {
                // clone the message writer settings with a new base Uri
                newSettings = testConfig.MessageWriterSettings.Clone();
                newSettings.BaseUri = baseUri;
            }

            return newSettings == null
                ? testConfig
                : new WriterTestConfiguration(testConfig.Format, newSettings, testConfig.IsRequest, testConfig.Synchronous);
        }

        private static readonly Dictionary<string, string> jsonPayloadHeaders = new Dictionary<string, string>()
                {
                    { "Content-Type", "application/json;charset=utf-8" },
                    { "OData-Version", null }
                };

        private static Dictionary<string, string> GetExpectedHeadersForFormat(ODataFormat format, ODataPayloadKind payloadKind)
        {
            Dictionary<string, string> expectedHeaders = null;
            switch (payloadKind)
            {
                case ODataPayloadKind.ResourceSet:
                    expectedHeaders = jsonPayloadHeaders;
                    break;
                case ODataPayloadKind.Resource:
                    expectedHeaders = jsonPayloadHeaders;
                    break;
                case ODataPayloadKind.Error:
                    expectedHeaders = jsonPayloadHeaders;
                    break;
                case ODataPayloadKind.Property:
                case ODataPayloadKind.EntityReferenceLink:
                case ODataPayloadKind.EntityReferenceLinks:
                case ODataPayloadKind.Value:
                case ODataPayloadKind.BinaryValue:
                case ODataPayloadKind.Collection:
                case ODataPayloadKind.ServiceDocument:
                case ODataPayloadKind.MetadataDocument:
                case ODataPayloadKind.Batch:
                case ODataPayloadKind.Unsupported:
                default:
                    throw new NotSupportedException("Unsupported payload kind.");
            }

            // Need to return a copy of the expected headers because the test will modify the headers
            Debug.Assert(expectedHeaders != null, "expectedHeaders != null");
            return new Dictionary<string, string>(expectedHeaders);
        }

        private sealed class ODataBatchOperationRequestMessage
        {
            private readonly object instance;

            public ODataBatchOperationRequestMessage(Stream outputStream)
            {
                Type classType = typeof(ODataAnnotatable).Assembly.GetType("Microsoft.OData.ODataBatchOperationRequestMessage");
                this.instance = Activator.CreateInstance(classType, outputStream, null);
            }

            public string GetHeader(string headerName) { return (string)ReflectionUtils.InvokeMethod(this.instance, "GetHeader", headerName); }
            public void SetHeader(string headerName, string headerValue) { ReflectionUtils.InvokeMethod(this.instance, "SetHeader", headerName, headerValue); }
#if !SILVERLIGHT
            public Task<Stream> GetStreamAsync() { return (Task<Stream>)ReflectionUtils.InvokeMethod(this.instance, "GetStreamAsync"); }
#endif
        }

        private sealed class BatchTestWithDirection
        {
            public BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] Batch { get; set; }
            public bool ForRequests { get; set; }
        }

        private sealed class CrossReferencingTestCase
        {
            internal string DebugDescription { get; set; }
            internal Uri BaseUri { get; set; }
            internal BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] Operations { get; set; }
            internal BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] Operations2 { get; set; }
            internal ExpectedException ExpectedException { get; set; }
            internal ExpectedException ExpectedExceptionNoBaseUri { get; set; }
        }
    }
}
