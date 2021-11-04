//---------------------------------------------------------------------
// <copyright file="BatchWriterUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.BatchWriter
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Tests.WriterTests.BatchWriter;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using ICombinatorialEngineProvider = Microsoft.Test.OData.Utils.CombinatorialEngine.ICombinatorialEngineProvider;

    #endregion Namespaces

    /// <summary>
    /// Helper methods for creating, writing and validating batch payloads.
    /// </summary>
    internal static class BatchWriterUtils
    {
        private static readonly int[] defaultBatchSizes = new int[] { 0, 1, 5 };

        private static readonly int[] defaultChangeSetSizes = new int[] { 0, 1, 3 };

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor StartBatch()
        {
            return new BatchWriterTestDescriptor.InvocationAndOperationDescriptor
            {
                Invocation = BatchWriterTestDescriptor.WriterInvocations.WriteStartBatch,
                OperationDescriptor = null
            };
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor EndBatch()
        {
            return new BatchWriterTestDescriptor.InvocationAndOperationDescriptor
            {
                Invocation = BatchWriterTestDescriptor.WriterInvocations.WriteEndBatch,
                OperationDescriptor = null
            };
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor StartChangeSet()
        {
            return new BatchWriterTestDescriptor.InvocationAndOperationDescriptor
            {
                Invocation = BatchWriterTestDescriptor.WriterInvocations.WriteStartChangeSet,
                OperationDescriptor = null
            };
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor EndChangeSet()
        {
            return new BatchWriterTestDescriptor.InvocationAndOperationDescriptor
            {
                Invocation = BatchWriterTestDescriptor.WriterInvocations.WriteEndChangeSet,
                OperationDescriptor = null
            };
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor ChangeSetRequest(
            string method,
            Uri uri,
            string contentId = null,
            Dictionary<string, string> headers = null,
            string payload = null,
            bool ignoreCrossReferencingRule = false)
        {
            // Add the content id to the headers if it exists
            if (contentId != null)
            {
                if (headers == null)
                {
                    headers = new Dictionary<string, string>(1);
                }

                Debug.Assert(!headers.ContainsKey(HttpHeaders.ContentId), "Content-ID header already exists.");
                headers.Add(HttpHeaders.ContentId, contentId);
            }

            return new BatchWriterTestDescriptor.InvocationAndOperationDescriptor
            {
                Invocation = BatchWriterTestDescriptor.WriterInvocations.WriteChangeSetOperation,
                OperationDescriptor = new BatchWriterTestDescriptor.BatchWriterChangeSetOperationTestDescriptor
                {
                    Method = method,
                    Uri = uri,
                    IgnoreCrossReferencingRule = ignoreCrossReferencingRule,
                    Headers = headers,
                    Payload = payload,
                    ContentId = contentId
                },
            };
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor ChangeSetRequest(
            string method,
            Uri uri,
            string contentId,
            Dictionary<string, string> headers,
            ODataPayload payload)
        {
            // Add the content id to the headers if it exists
            if (contentId != null)
            {
                if (headers == null)
                {
                    headers = new Dictionary<string, string>(1);
                }

                Debug.Assert(!headers.ContainsKey(HttpHeaders.ContentId), "Content-ID header already exists.");
                headers.Add(HttpHeaders.ContentId, contentId);
            }

            return new BatchWriterTestDescriptor.InvocationAndOperationDescriptor
            {
                Invocation = BatchWriterTestDescriptor.WriterInvocations.WriteChangeSetOperation,
                OperationDescriptor = new BatchWriterTestDescriptor.BatchWriterChangeSetOperationTestDescriptor
                {
                    Method = method,
                    Uri = uri,
                    Headers = headers,
                    ODataPayload = payload,
                    ContentId = contentId
                },
            };
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor ChangeSetResponse(int statusCode, string payload, Dictionary<string, string> headers = null)
        {
            return new BatchWriterTestDescriptor.InvocationAndOperationDescriptor
            {
                Invocation = BatchWriterTestDescriptor.WriterInvocations.WriteChangeSetOperation,
                OperationDescriptor = new BatchWriterTestDescriptor.BatchWriterResponseOperationTestDescriptor
                {
                    StatusCode = statusCode,
                    Payload = payload,
                    Headers = headers
                },
            };
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor ChangeSetResponse(int statusCode, ODataPayload payload, Dictionary<string, string> headers = null)
        {
            return new BatchWriterTestDescriptor.InvocationAndOperationDescriptor
            {
                Invocation = BatchWriterTestDescriptor.WriterInvocations.WriteChangeSetOperation,
                OperationDescriptor = new BatchWriterTestDescriptor.BatchWriterResponseOperationTestDescriptor
                {
                    StatusCode = statusCode,
                    Headers = headers,
                    ODataPayload = payload,
                },
            };
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor QueryOperation(string method, Uri uri, Dictionary<string, string> headers = null)
        {
            return new BatchWriterTestDescriptor.InvocationAndOperationDescriptor
            {
                Invocation = BatchWriterTestDescriptor.WriterInvocations.WriteQueryOperation,
                OperationDescriptor = new BatchWriterTestDescriptor.BatchWriterQueryOperationTestDescriptor
                {
                    Method = method,
                    Uri = uri,
                    Headers = headers
                },
            };
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor QueryOperationResponse(int statusCode, string payload, Dictionary<string, string> headers = null)
        {
            return new BatchWriterTestDescriptor.InvocationAndOperationDescriptor
            {
                Invocation = BatchWriterTestDescriptor.WriterInvocations.WriteQueryOperation,
                OperationDescriptor = new BatchWriterTestDescriptor.BatchWriterResponseOperationTestDescriptor
                {
                    StatusCode = statusCode,
                    Payload = payload,
                    Headers = headers
                },
            };
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor QueryOperationResponse(int statusCode, ODataPayload payload, Dictionary<string, string> headers = null)
        {
            return new BatchWriterTestDescriptor.InvocationAndOperationDescriptor
            {
                Invocation = BatchWriterTestDescriptor.WriterInvocations.WriteQueryOperation,
                OperationDescriptor = new BatchWriterTestDescriptor.BatchWriterResponseOperationTestDescriptor
                {
                    StatusCode = statusCode,
                    ODataPayload = payload,
                    Headers = headers
                },
            };
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor UserException()
        {
            return new BatchWriterTestDescriptor.InvocationAndOperationDescriptor
            {
                Invocation = BatchWriterTestDescriptor.WriterInvocations.UserException,
                OperationDescriptor = null
            };
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] QueryRequests()
        {
            var operations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
            {
                // add interesting query operations to the list
                QueryOperation("GET", new Uri("http://www.odata.org/OData/OData.svc/1")),
                QueryOperation("GET", new Uri("2", UriKind.Relative)),
                QueryOperation(
                    "GET",
                    new Uri("http://www.odata.org/OData/OData.svc/3"),
                    new Dictionary<string, string> { { "Host", "www.odata.org" } }),
                QueryOperation(
                    "GET",
                    new Uri("http://www.odata.org/OData/OData.svc/4"),
                    new Dictionary<string, string> { { "Host", "www.odata.org" }, { "A", "B" }, { "C", "D" } }),
                QueryOperation("GET", new Uri("http://www.odata.org/OData/OData.svc/5")),
            };

            return operations;
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] QueryResponses()
        {
            var operations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
            {
                // add interesting query operation responses to the list
                QueryOperationResponse(200, "Sample response payload."),
                QueryOperationResponse(404, "Sample response payload 2."),
                QueryOperationResponse(
                    200,
                    "Sample response payload 3.",
                    new Dictionary<string,string> { { "CustomHeader", "Successful response" } }),
                QueryOperationResponse(
                    200,
                    "Sample response payload 4.",
                    new Dictionary<string,string> { { "A", "B" }, { "C", "D" } }),
                QueryOperationResponse(500, "Sample response payload 5."),
            };

            return operations;
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor[][] ChangeSetRequests()
        {
            // empty changeset
            BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] updateOperations1 = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
            {
            };

            // changeset with one operation, null content ID
            BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] updateOperations2 = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
            {
                ChangeSetRequest("POST", new Uri("http://www.odata.org/OData/OData.svc/Products"), null, null, "First sample payload"),
            };

            // changeset with one operation, non-null content ID
            BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] updateOperations3 = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
            {
                ChangeSetRequest("POST", new Uri("http://www.odata.org/OData/OData.svc/Products"), "1", null, "First sample payload"),
            };

            // changeset with relative links
            BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] updateOperations4 = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
            {
                ChangeSetRequest("POST", new Uri("http://www.odata.org/OData/OData.svc/Products"), "1", null, "First sample payload"),
                ChangeSetRequest("POST", new Uri("/Products(2)", UriKind.Relative), null, null, "Second sample payload"),
                ChangeSetRequest("POST", new Uri("$1/Products(2)", UriKind.Relative), null, null, "Third sample payload"),
            };

            // full changeset
            BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] updateOperations5 = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
            {
                ChangeSetRequest("POST", new Uri("http://www.odata.org/OData/OData.svc/Products"), "1", null, "First sample payload"),
                ChangeSetRequest(
                    "POST",
                    new Uri("$1/Category", UriKind.Relative),
                    "2",
                    new Dictionary<string,string>() { { "Host", "www.odata.org" }, { "Content-Length", "42" } },
                    "Second sample payload"),
                ChangeSetRequest("DELETE", new Uri("http://www.odata.org/OData/OData.svc/Products"), "3", null, "Fourth sample payload"),
                ChangeSetRequest("PUT", new Uri("http://www.odata.org/OData/OData.svc/Products"), "4", null, "Fifth sample payload"),
                ChangeSetRequest("POST", new Uri("/Products(2)", UriKind.Relative), "5", null, "Second sample payload"),
                ChangeSetRequest(
                    "POST",
                    new Uri("http://www.odata.org/OData/OData.svc/Products"),
                    "6",
                    new Dictionary<string,string>()
                    {
                        { "Host", "www.odata.org" },
                        { "Content-Length", "42" },
                        { "A", "B" },
                        { "C", "D" },
                    },
                    "Third sample payload"),
            };

            // NOTE: must not filter the operations since they may depend on each (cross-referencing) and subsetting would result in invalid payloads.
            return new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[][] { updateOperations1, updateOperations2, updateOperations3, updateOperations4, updateOperations5 };
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor[][] ChangeSetResponses()
        {
            var updateOperations = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]
            {
                ChangeSetResponse(200, "Sample response payload 1."),
                ChangeSetResponse(404, "Sample response payload 2."),
                ChangeSetResponse(200, "Sample response payload 3.", new Dictionary<string,string>() { { "Created-Date", "Today" } }),
                ChangeSetResponse(200, "Sample response payload 4.", new Dictionary<string,string>() { { "Created-Date", "Today" }, { "A", "B" }, { "C", "D" } }),
            };

            return updateOperations.Subsets(defaultChangeSetSizes).ToArray();
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor[][] CreateBatchRequestTestCases(ICombinatorialEngineProvider combinatorialEngine)
        {
            BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] queryRequests = QueryRequests();
            BatchWriterTestDescriptor.InvocationAndOperationDescriptor[][] changeSetRequests = ChangeSetRequests();

            return CreateBatchTestCases(queryRequests, changeSetRequests, combinatorialEngine);
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor[][] CreateBatchResponseTestCases(ICombinatorialEngineProvider combinatorialEngine)
        {
            BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] queryResponses = QueryResponses();
            BatchWriterTestDescriptor.InvocationAndOperationDescriptor[][] changeSetResponses = ChangeSetResponses();

            return CreateBatchTestCases(queryResponses, changeSetResponses, combinatorialEngine);
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] CreateChangeSetRequestBatch(string method, Uri uri, ODataPayload payload, Dictionary<string, string> headers = null)
        {
            return CreateChangeSetBatch(ChangeSetRequest(method, uri, null, headers, payload));
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] CreateChangeSetResponseBatch(int statusCode, ODataPayload payload, Dictionary<string, string> headers = null)
        {
            return CreateChangeSetBatch(ChangeSetResponse(statusCode, payload, headers));
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] CreateQueryResponseBatch(int statusCode, ODataPayload payload, Dictionary<string, string> headers = null)
        {
            return CreateQueryBatch(QueryOperationResponse(statusCode, payload, headers));
        }

        private static BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] CreateQueryBatch(params BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] queryOperations)
        {
            var result = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[queryOperations.Length + 2];
            result[0] = BatchWriterUtils.StartBatch();
            queryOperations.CopyTo(result, 1);
            result[queryOperations.Length + 1] = BatchWriterUtils.EndBatch();
            return result;
        }

        private static BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] CreateChangeSetBatch(params BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] changeSetRequests)
        {
            var result = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[changeSetRequests.Length + 4];
            result[0] = BatchWriterUtils.StartBatch();
            result[1] = BatchWriterUtils.StartChangeSet();
            changeSetRequests.CopyTo(result, 2);
            result[changeSetRequests.Length + 2] = BatchWriterUtils.EndChangeSet();
            result[changeSetRequests.Length + 3] = BatchWriterUtils.EndBatch();
            return result;
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] CreateDefaultChangeSetBatch(int changeSetSize)
        {
            return CreateDefaultChangeSetBatch(1, new int[] { changeSetSize });
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] CreateDefaultChangeSetBatch(int changeSetCount, int[] changeSetSizes)
        {
            Debug.Assert(changeSetCount >= 0, "batchSize >= 0");
            Debug.Assert(changeSetSizes != null, "operationsPerChangeSet != null");
            Debug.Assert(changeSetSizes.Length == changeSetCount, "Size of the batch must match the length of the change set sizes array!");

            var result = new List<BatchWriterTestDescriptor.InvocationAndOperationDescriptor>();
            result.Add(StartBatch());

            for (int i = 0; i < changeSetCount; ++i)
            {
                Debug.Assert(changeSetSizes[i] >= 0, "operationsPerChangeSet[" + i + "] >= 0");

                result.Add(StartChangeSet());

                for (int j = 0; j < changeSetSizes[i]; ++j)
                {
                    result.Add(ChangeSetRequest("POST", new Uri("http://www.odata.org/OData/OData.svc/Products"), j.ToString(), null, "Sample payload."));
                }

                result.Add(EndChangeSet());
            }

            result.Add(EndBatch());

            return result.ToArray();
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] CreateDefaultQueryBatch(int batchSize)
        {
            Debug.Assert(batchSize >= 0, "batchSize >= 0");

            int operationCount = batchSize + 2;
            var result = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[operationCount];
            result[0] = StartBatch();

            for (int i = 0; i < batchSize; ++i)
            {
                result[i + 1] = QueryOperation("GET", new Uri("http://www.odata.org/OData/OData.svc/Products"));
            }

            result[operationCount - 1] = EndBatch();

            return result;
        }

        internal static BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] CreateDefaultQueryResponseBatch(int batchSize)
        {
            Debug.Assert(batchSize >= 0, "batchSize >= 0");

            int operationCount = batchSize + 2;
            var result = new BatchWriterTestDescriptor.InvocationAndOperationDescriptor[operationCount];
            result[0] = StartBatch();

            for (int i = 0; i < batchSize; ++i)
            {
                result[i + 1] = QueryOperationResponse(200, "Sample response payload.");
            }

            result[operationCount - 1] = EndBatch();

            return result;
        }

        /// <summary>
        /// Writes the collection payload as specified in the <paramref name="testDescriptor"/>.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="flush">True if the stream should be flush before returning; otherwise false.</param>
        /// <param name="testDescriptor">The test descriptor specifying the batch to write.</param>
        /// <param name="testConfiguration">The test configuration used.</param>
        internal static void WriteBatchPayload(ODataBatchWriterTestWrapper writer, bool flush, BatchWriterTestDescriptor testDescriptor, WriterTestConfiguration testConfiguration, AssertionHandler assert)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(testDescriptor != null, "testDescriptor != null");
            Debug.Assert(testConfiguration != null, "testConfiguration != null");

            foreach (BatchWriterTestDescriptor.InvocationAndOperationDescriptor invocationAndOperationDescriptor in testDescriptor.InvocationsAndOperationDescriptors)
            {
                switch (invocationAndOperationDescriptor.Invocation)
                {
                    case BatchWriterTestDescriptor.WriterInvocations.WriteStartBatch:
                        writer.WriteStartBatch();
                        assert.IsNull(invocationAndOperationDescriptor.OperationDescriptor, "No operation descriptor expected.");
                        break;
                    case BatchWriterTestDescriptor.WriterInvocations.WriteStartChangeSet:
                        writer.WriteStartChangeset();
                        assert.IsNull(invocationAndOperationDescriptor.OperationDescriptor, "No operation descriptor expected.");
                        break;
                    case BatchWriterTestDescriptor.WriterInvocations.WriteQueryOperation:
                        if (testConfiguration.IsRequest)
                        {
                            BatchWriterTestDescriptor.BatchWriterQueryOperationTestDescriptor queryOperationDescriptor =
                                (BatchWriterTestDescriptor.BatchWriterQueryOperationTestDescriptor)invocationAndOperationDescriptor.OperationDescriptor;
                            if (queryOperationDescriptor == null)
                            {
                                // testing error cases of the state machine.
                                continue;
                            }
                            ODataBatchOperationRequestMessage queryOperationMessage = writer.CreateOperationRequestMessage(queryOperationDescriptor.Method, queryOperationDescriptor.Uri);
                            // set the headers for the operation message
                            SetOperationHeaders(queryOperationMessage, queryOperationDescriptor.Headers);
                        }
                        else
                        {
                            BatchWriterTestDescriptor.BatchWriterResponseOperationTestDescriptor responseOperationDescriptor =
                                (BatchWriterTestDescriptor.BatchWriterResponseOperationTestDescriptor)invocationAndOperationDescriptor.OperationDescriptor;
                            if (responseOperationDescriptor == null)
                            {
                                // testing error cases of the state machine
                                continue;
                            }
                            ODataBatchOperationResponseMessage queryOperationMessage = writer.CreateOperationResponseMessage(true);
                            queryOperationMessage.StatusCode = responseOperationDescriptor.StatusCode;
                            // set the headers for the operation message
                            SetOperationHeaders(queryOperationMessage, responseOperationDescriptor.Headers);
                            WritePayloadToStream(queryOperationMessage, testConfiguration, responseOperationDescriptor.Payload, responseOperationDescriptor.ODataPayload, assert);
                        }
                        break;
                    case BatchWriterTestDescriptor.WriterInvocations.WriteChangeSetOperation:
                        {
                            string payload = null;
                            ODataPayload odataPayload = null;
                            if (testConfiguration.IsRequest)
                            {
                                BatchWriterTestDescriptor.BatchWriterChangeSetOperationTestDescriptor changeSetOperationDescriptor =
                                    (BatchWriterTestDescriptor.BatchWriterChangeSetOperationTestDescriptor)invocationAndOperationDescriptor.OperationDescriptor;
                                if (changeSetOperationDescriptor == null)
                                {
                                    // testing error cases of the state machine
                                    continue;
                                }

                                payload = changeSetOperationDescriptor.Payload;
                                odataPayload = changeSetOperationDescriptor.ODataPayload;

                                ODataBatchOperationRequestMessage changeSetOperation = writer.CreateOperationRequestMessage(changeSetOperationDescriptor.Method, changeSetOperationDescriptor.Uri, changeSetOperationDescriptor.ContentId);

                                // set the headers for the operation message
                                SetOperationHeaders(changeSetOperation, changeSetOperationDescriptor.Headers);
                                WritePayloadToStream(changeSetOperation, testConfiguration, payload, odataPayload, assert);
                            }
                            else
                            {
                                BatchWriterTestDescriptor.BatchWriterResponseOperationTestDescriptor responseOperationDescriptor =
                                    (BatchWriterTestDescriptor.BatchWriterResponseOperationTestDescriptor)invocationAndOperationDescriptor.OperationDescriptor;
                                if (responseOperationDescriptor == null)
                                {
                                    // testing error cases of the state machine
                                    continue;
                                }
                                payload = responseOperationDescriptor.Payload;
                                odataPayload = responseOperationDescriptor.ODataPayload;

                                ODataBatchOperationResponseMessage changeSetOperation = writer.CreateOperationResponseMessage();
                                changeSetOperation.StatusCode = responseOperationDescriptor.StatusCode;
                                // set the headers for the operation message
                                SetOperationHeaders(changeSetOperation, responseOperationDescriptor.Headers);
                                WritePayloadToStream(changeSetOperation, testConfiguration, payload, odataPayload, assert);
                            }
                        }
                        break;
                    case BatchWriterTestDescriptor.WriterInvocations.WriteEndChangeSet:
                        writer.WriteEndChangeset();
                        assert.IsNull(invocationAndOperationDescriptor.OperationDescriptor, "No operation descriptor expected.");
                        break;
                    case BatchWriterTestDescriptor.WriterInvocations.WriteEndBatch:
                        writer.WriteEndBatch();
                        assert.IsNull(invocationAndOperationDescriptor.OperationDescriptor, "No operation descriptor expected.");
                        break;
                    case BatchWriterTestDescriptor.WriterInvocations.UserException:
                        throw new Exception("User code triggered an exception.");
                    default:
                        throw new NotSupportedException("WriterInvocation kind " + invocationAndOperationDescriptor.ToString() + "is not supported.");
                }
            }

            if (flush)
            {
                writer.Flush();
            }
        }

        /// <summary>
        /// Validate the result of writing a batch payload.
        /// </summary>
        /// <param name="stream">The (seekable) stream the payload was written to.</param>
        /// <param name="testConfiguration">The configuration of the test.</param>
        /// <param name="expectedResults">The expected result description.</param>
        /// <param name="contentTypeHeader">The content type header of the message.</param>
        /// <param name="exception">An (optional) exception if one was thrown during writing.</param>
        /// <param name="assert">The assertion handler to report errors to.</param>
        internal static void ValidateBatchResult(
            Stream stream,
            WriterTestConfiguration testConfiguration,
            WriterTestConfiguration payloadConfiguration,
            WriterTestExpectedResults expectedResults,
            string contentTypeHeader,
            Exception exception,
            AssertionHandler assert,
            IExceptionVerifier exceptionVerifier)
        {
            if (expectedResults == null)
            {
                return;
            }

            if (TestWriterUtils.ValidateReportedError(exception, expectedResults, assert, exceptionVerifier))
            {
                return;
            }

            assert.IsTrue(testConfiguration.Format == null || testConfiguration.Format == ODataFormat.Batch, "testConfiguration.Format == null || testConfiguration.Format == ODataFormat.Batch");

            // extract the batch boundary from the content type header
            string batchBoundary = GetBatchBoundary(contentTypeHeader, assert);

            stream.Seek(0, SeekOrigin.Begin);

            var batchExpectedResults = expectedResults as BatchWriterTestExpectedResults;
            ExceptionUtilities.CheckObjectNotNull(batchExpectedResults, "The expected result should be for the batch format.");

            string error;
            bool success = CompareBatchResults(batchExpectedResults, payloadConfiguration, batchBoundary, stream, assert, out error);
            assert.IsTrue(success, error);
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchWriter"/> for the specified format and the specified version and
        /// invokes the specified methods on it. It then parses the batch and the Xml/Json of the operation bodies
        /// and compares it to the expected result as specified in the descriptor.
        /// </summary>
        /// <param name="descriptor">The test descriptor to process.</param>
        /// <param name="batchConfiguration">The configuration of the batch writer.</param>
        /// <param name="payloadConfiguration">The configuration for the payload.</param>
        /// <param name="assert">The assertion handler to report errors to.</param>
        internal static void WriteAndVerifyBatchPayload(BatchWriterTestDescriptor descriptor, WriterTestConfiguration batchConfiguration, WriterTestConfiguration payloadConfiguration, AssertionHandler assert)
        {
            // serialize to a memory stream
            using (var memoryStream = new MemoryStream())
            using (var testStream = new TestStream(memoryStream, ignoreDispose: true))
            {
                TestMessage testMessage = null;
                Exception exception = TestExceptionUtils.RunCatching(() =>
                {
                    using (var messageWriter = TestWriterUtils.CreateMessageWriter(testStream, batchConfiguration, assert, out testMessage, batchConfiguration.MessageWriterSettings, null, descriptor.UrlResolver))
                    {
                        ODataBatchWriterTestWrapper writer = messageWriter.CreateODataBatchWriter();
                        BatchWriterUtils.WriteBatchPayload(writer, true, descriptor, payloadConfiguration, assert);
                        writer.Flush();
                    }
                });
                exception = TestExceptionUtils.UnwrapAggregateException(exception, assert);
                BatchWriterTestExpectedResults expectedResults = (BatchWriterTestExpectedResults)descriptor.ExpectedResultCallback(batchConfiguration);
                BatchWriterUtils.ValidateBatchResult(
                    memoryStream,
                    batchConfiguration,
                    payloadConfiguration,
                    expectedResults,
                    testMessage.GetHeader(ODataConstants.ContentTypeHeader),
                    exception,
                    assert,
                    descriptor.TestDescriptorSettings.ExpectedResultSettings.ExceptionVerifier);

                TestWriterUtils.ValidateContentType(testMessage, expectedResults, false, assert);
                TestWriterUtils.ValidateHeaders(testMessage, expectedResults.ExpectedHeaders, assert);
            }
        }


        private static void WritePayloadToStream(ODataBatchOperationResponseMessage operationResponseMessage, WriterTestConfiguration testConfiguration, string stringPayload, ODataPayload odataPayload, AssertionHandler assert)
        {
            if (stringPayload != null)
            {
                WriteStringPayloadToStream(
                    () => operationResponseMessage.GetStream(),
                    () => operationResponseMessage.GetStreamAsync(),
                    !testConfiguration.Synchronous,
                    stringPayload,
                    assert);
            }
            else if (odataPayload != null)
            {
                WriteODataPayloadToStream(() => new ODataMessageWriter(operationResponseMessage, testConfiguration.MessageWriterSettings), testConfiguration, odataPayload, assert);
            }
        }

        private static void WritePayloadToStream(ODataBatchOperationRequestMessage operationRequestMessage, WriterTestConfiguration testConfiguration, string stringPayload, ODataPayload odataPayload, AssertionHandler assert)
        {
            if (stringPayload != null)
            {
                WriteStringPayloadToStream(
                    () => operationRequestMessage.GetStream(),
                    () => operationRequestMessage.GetStreamAsync(),
                    !testConfiguration.Synchronous,
                    stringPayload, assert);
            }
            else if (odataPayload != null)
            {
                WriteODataPayloadToStream(() => new ODataMessageWriter(operationRequestMessage, testConfiguration.MessageWriterSettings), testConfiguration, odataPayload, assert);
            }
        }

        private static void WriteStringPayloadToStream(
            Func<Stream> syncStreamFunc,
            Func<Task<Stream>> asyncStreamFunc,
             bool isAsync,
            string stringPayload,
            AssertionHandler assert)
        {
            Stream stream;
            if (isAsync)
            {
                var t = asyncStreamFunc();
                t.Wait();
                stream = t.Result;
            }
            else
            {
                stream = syncStreamFunc();
            }

            using (StreamWriter streamWriter = new StreamWriter(stream))
            {
                streamWriter.WriteLine(stringPayload);
                streamWriter.Flush();
                // Calling IDisposable.Dispose  should not throw. This is to check ODataBatchOperationStream.
                streamWriter.Dispose();
                stream.Dispose();
            }
        }

        private static void WriteODataPayloadToStream(Func<ODataMessageWriter> messageWriterFunc, WriterTestConfiguration testConfiguration, ODataPayload odataPayload, AssertionHandler assert)
        {
            assert.IsTrue(testConfiguration.MessageWriterSettings.EnableMessageStreamDisposal, "The batch writer will fail if the batch operation stream doesn't get disposed after use.");
            ODataMessageWriter messageWriter = messageWriterFunc();
            using (ODataMessageWriterTestWrapper messageWriterWrapper = new ODataMessageWriterTestWrapper(messageWriter, testConfiguration))
            {
                ODataWriter writer = null;

                object firstItem = odataPayload.Items[0];
                if (!(firstItem is ODataAnnotatedError))
                {
                    writer = messageWriterWrapper.CreateODataWriter(/*isFeed*/ firstItem is ODataResourceSet);
                }

                TestWriterUtils.WritePayload(messageWriterWrapper, writer, true, odataPayload.Items);
            }
        }

        /// <summary>
        /// Compares the batch structure written by an ODataBatchWriter with the expected result.
        /// </summary>
        /// <param name="expectedResults">The expected results.</param>
        /// <param name="batchBoundary">The expected batch boundary.</param>
        /// <param name="stream">The memory stream to read the observed result from.</param>
        /// <param name="error">An error message indicating how the expected and the observed results differ.</param>
        /// <returns>True if the expected and observed results are the same; otherwise false.</returns>
        private static bool CompareBatchResults(BatchWriterTestExpectedResults expectedResults, WriterTestConfiguration testConfiguration, string batchBoundary, Stream stream, AssertionHandler assert, out string error)
        {
            assert.IsNotNull(expectedResults, "expectedResults != null");
            assert.IsNotNull(batchBoundary, "batchBoundary != null");
            assert.IsNotNull(stream, "stream != null");

            error = null;

            int operationIndex = 0;
            int changeSetIndex = 0;

            MemoryStream memoryStream = new MemoryStream();
            // Create a copy since StreamReader will Close the stream, which might not be what we want
            // NOTE: this is not strictly necessary when using the TestStream but this utility can be used with different streams as well.
            StreamUtils.CopyStream(stream, memoryStream);
            memoryStream.Position = 0;

            using (TextReader reader = new StreamReader(memoryStream))
            {
                batchBoundary = "--" + batchBoundary;
                string batchEndBoundary = batchBoundary + "--";

                string line;
                if (expectedResults.IsEmptyBatch())
                {
                    line = reader.ReadLine();
                    if (line != batchEndBoundary)
                    {
                        error = "Expected end of batch boundary but found '" + line + "'.";
                        return false;
                    }

                    return true;
                }

                // There should be nothing before the first batch boundary
                line = reader.ReadLine();
                if (line == batchEndBoundary)
                {
                    error = "Found a batch end boundary without a batch start boundary.";
                    return false;
                }

                if (line != batchBoundary)
                {
                    error = "The batch payload must start with a batch start boundary, but it doesn't, it starts with '" + line + "'.";
                    return false;
                }

                while (true)
                {
                    line = reader.ReadLine(); // Content-Type
                    if (!line.StartsWith("Content-Type:"))
                    {
                        error = "The batch operation #" + operationIndex + " doesn't specify its content type.";
                        return false;
                    }

                    string contentType = line.Substring("Content-Type:".Length).Trim();
                    if (contentType.StartsWith("multipart/mixed; "))
                    {
                        // found a change set
                        BatchWriterTestDescriptor.BatchWriterOperationTestDescriptor[] changeSetOperations = expectedResults.GetChangeSet(changeSetIndex);
                        if (changeSetOperations == null)
                        {
                            error = "The observed batch message contains more operations than the number of operations written.";
                            return false;
                        }

                        changeSetIndex++;

                        string changesetBoundary = "--" + contentType.Substring(contentType.IndexOf("; boundary=") + 11).Trim();
                        string changesetEndBoundary = changesetBoundary + "--";

                        int cOperationIndex = 0;

                        // read everything until we find an empty line
                        // TODO: we should validate the change set headers here, no?
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Trim().Length == 0) break;
                        }

                        if (changeSetOperations.Length == 0)
                        {
                            line = reader.ReadLine();
                            if (line != changesetEndBoundary)
                            {
                                error = "Expected end of changeset boundary but found '" + line + "'.";
                                return false;
                            }

                            goto ChangesetEnd;
                        }

                        line = reader.ReadLine();
                        if (line != changesetBoundary)
                        {
                            error = "Expected the first changeset boundary but found '" + line + "'.";
                            return false;
                        }

                        while (true)
                        {
                            if (changeSetOperations.Length <= cOperationIndex)
                            {
                                error = "The batch response contains more operations than the number of request operations sent.";
                                return false;
                            }

                            var operationDescriptor = (BatchWriterTestDescriptor.BatchWriterOperationTestDescriptor)changeSetOperations[cOperationIndex];
                            cOperationIndex++;

                            string changeSetOperationHeader = reader.ReadLine();
                            while (changeSetOperationHeader.Length > 0)
                            {
                                // we expect 'Content-Type', 'Content-Transfer-Encoding' and 'Content-ID' here
                                if (changeSetOperationHeader.StartsWith("Content-Type:") ||
                                    changeSetOperationHeader.StartsWith("Content-Transfer-Encoding:") ||
                                    changeSetOperationHeader.StartsWith("Content-ID:"))
                                {
                                    // TODO: validate these?
                                }
                                else
                                {
                                    // unexpected header. Fail.
                                    error = "Unexpected change set operation header '" + changeSetOperationHeader + "' found.";
                                    return false;
                                }

                                changeSetOperationHeader = reader.ReadLine();
                            }

                            bool lastOperation;
                            if (!CompareBatchOperations(reader, testConfiguration, operationDescriptor, changesetBoundary, changesetEndBoundary, out lastOperation, out error))
                            {
                                return false;
                            }

                            if (lastOperation)
                            {
                                break;
                            }
                        }

                        ChangesetEnd:
                        // TODO: this seems to indicate that the product supports in-stream errors for batch requests!
                        // We can hit another exception after the changeset.  For example if the changeset fails
                        // and after the server serializes out the error for the changeset, writes the endboundary
                        // for the changeset, and call IUpdatable.ClearChanges(), ClearChanges() can throw.
                        // When that happens we will see the second error but there won't be any boundary after it.
                        // We need to goto End when we either see the end boundary or reached the end of stream.
                        line = reader.ReadLine();
                        if (line == batchEndBoundary)
                        {
                            goto End;
                        }

                        if (line != batchBoundary)
                        {
                            error = "Expected batch boundary after the end of changeset, but found '" + line + "'.";
                            return false;
                        }
                    }
                    else
                    {
                        if (!contentType.StartsWith("application/http"))
                        {
                            error = "Detected batch operation that is neither a changeset nor a query operation.";
                            return false;
                        }

                        // TODO: validate these?
                        line = reader.ReadLine(); // Content-Transfer-Encoding
                        line = reader.ReadLine(); //

                        BatchWriterTestDescriptor.BatchWriterOperationTestDescriptor queryOperation = expectedResults.GetQueryOperation(operationIndex);
                        if (queryOperation == null)
                        {
                            error = "The batch response contains more operations than the number of request operations sent.";
                            return false;
                        }

                        operationIndex++;

                        bool lastOperation;
                        if (!CompareBatchOperations(reader, testConfiguration, queryOperation, batchBoundary, batchEndBoundary, out lastOperation, out error))
                        {
                            return false;
                        }

                        if (lastOperation)
                        {
                            goto End;
                        }
                    }
                }
                End:
                line = reader.ReadLine();
                if (line != null)
                {
                    error = "Additional data after the end of the batch found '" + line + "'.";
                }

                if (operationIndex + changeSetIndex != expectedResults.GetOperationCount())
                {
                    error = "The response didn't contain the expected number of operations.";
                    return false;
                }
            }

            return true;
        }

        private static bool CompareBatchOperations(
            TextReader reader,
            WriterTestConfiguration testConfiguration,
            BatchWriterTestDescriptor.BatchWriterOperationTestDescriptor operationDescriptor,
            string boundary,
            string endBoundary,
            out bool lastOperation,
            out string error)
        {
            lastOperation = false;
            string line;

            // validate the request/response line
            if (testConfiguration.IsRequest)
            {
                var requestOperationDescriptor = (BatchWriterTestDescriptor.BatchWriterRequestOperationTestDescriptor)operationDescriptor;
                Uri expectedRequestUri = requestOperationDescriptor.Uri;
                if (!expectedRequestUri.IsAbsoluteUri)
                {
                    if (expectedRequestUri.OriginalString.StartsWith("$") && !requestOperationDescriptor.IgnoreCrossReferencingRule)
                    {
                        // special case for cross-referencing links; don't append a base URI to them
                    }
                    else
                    {
                        // need to prepend the base Uri
                        Uri baseUri = testConfiguration.MessageWriterSettings.BaseUri;
                        if (baseUri == null)
                        {
                            // we should not get here since the product should have thrown
                            error = "Detected a relative URI for a batch operation and no base Uri was specified in the message writer settings.";
                            return false;
                        }

                        expectedRequestUri = new Uri(baseUri, expectedRequestUri);
                    }
                }
                if (!TestHttpUtils.ValidateRequestLine(reader.ReadLine(), requestOperationDescriptor.Method, expectedRequestUri, out error))
                {
                    return false;
                }
            }
            else
            {
                var responseOperationDescriptor = (BatchWriterTestDescriptor.BatchWriterResponseOperationTestDescriptor)operationDescriptor;
                if (!TestHttpUtils.ValidateResponseLine(reader.ReadLine(), TestHttpUtils.HttpVersionInBatching, responseOperationDescriptor.StatusCode, out error))
                {
                    return false;
                }
            }

            // validate the headers
            if (!TestHttpUtils.ValidateHeaders(reader, operationDescriptor.Headers, out error))
            {
                return false;
            }

            // validate the content
            StringBuilder sb = null;
            string lastLine = null;
            while ((line = reader.ReadLine()) != null)
            {
                if (line == boundary)
                {
                    break;
                }
                if (line == endBoundary)
                {
                    lastOperation = true;
                    break;
                }
                if (lastLine != null)
                {
                    if (sb == null) sb = new StringBuilder();
                    sb.AppendLine(lastLine);
                }
                lastLine = line;
            }
            // The last line must not end with a newline - the batch adds it there, but it's not actually part of the content
            if (lastLine != null)
            {
                if (lastLine == string.Empty)
                {
                    // only the newline belonging to the boundary; do nothing
                }
                else
                {
                    if (lastLine.EndsWith(Environment.NewLine))
                    {
                        lastLine = lastLine.Substring(0, lastLine.Length - Environment.NewLine.Length);
                    }
                    if (sb == null) sb = new StringBuilder();
                    sb.Append(lastLine);
                }
            }

            string observedContent = sb == null ? null : sb.ToString().Trim();
            Func<string, string> comparer = BatchWriterTestExpectedResults.GetComparer(operationDescriptor, testConfiguration);
            error = comparer(observedContent);
            return error == null;
        }

        /// <summary>
        /// Extracts the batch boundary from a content type header
        /// </summary>
        /// <param name="contentTypeHeader">The content-type string.</param>
        /// <param name="assert">The assertion handler.</param>
        /// <returns>The boundary string extracted from the content type string.</returns>
        private static string GetBatchBoundary(string contentTypeHeader, AssertionHandler assert)
        {
            assert.IsNotNull(contentTypeHeader, "Expected non-null content type.");

            const string boundaryStart = "; boundary=";
            int ix = contentTypeHeader.IndexOf(boundaryStart);
            assert.IsTrue(ix > 0, "Expected to find '" + boundaryStart + "' as part of the content type.");

            int startIx = ix + boundaryStart.Length;
            int endIx = contentTypeHeader.IndexOf(';', startIx);

            if (endIx < 0)
            {
                return contentTypeHeader.Substring(startIx);
            }
            else
            {
                return contentTypeHeader.Substring(startIx, endIx - startIx);
            }
        }

        /// <summary>
        /// Sets all the headers specified in <paramref name="headers"/> for the given <paramref name="requestMessage"/>.
        /// </summary>
        /// <param name="requestMessage">The request message to set the headers on.</param>
        /// <param name="headers">The set of headers to set.</param>
        private static void SetOperationHeaders(IODataRequestMessage requestMessage, Dictionary<string, string> headers)
        {
            if (headers != null)
            {
                foreach (var kvp in headers)
                {
                    requestMessage.SetHeader(kvp.Key, kvp.Value);
                }
            }
        }

        /// <summary>
        /// Sets all the headers specified in <paramref name="headers"/> for the given <paramref name="responseMessage"/>.
        /// </summary>
        /// <param name="responseMessage">The response message to set the headers on.</param>
        /// <param name="headers">The set of headers to set.</param>
        private static void SetOperationHeaders(IODataResponseMessage responseMessage, Dictionary<string, string> headers)
        {
            if (headers != null)
            {
                foreach (var kvp in headers)
                {
                    responseMessage.SetHeader(kvp.Key, kvp.Value);
                }
            }
        }

        /// <summary>
        /// Checks whether the <paramref name="expectedResults"/> describe an empty batch.
        /// </summary>
        /// <param name="expectedResults">The expected result structure to investigate.</param>
        /// <returns>True if the expected results describe an empty batch; otherwise false.</returns>
        private static bool IsEmptyBatch(this BatchWriterTestExpectedResults expectedResults)
        {
            Debug.Assert(expectedResults != null, "expectedResults != null");

            return expectedResults.InvocationsAndOperationDescriptors != null &&
                expectedResults.InvocationsAndOperationDescriptors.Length == 2 &&
                expectedResults.InvocationsAndOperationDescriptors[0].Invocation == BatchWriterTestDescriptor.WriterInvocations.WriteStartBatch &&
                expectedResults.InvocationsAndOperationDescriptors[1].Invocation == BatchWriterTestDescriptor.WriterInvocations.WriteEndBatch;
        }

        private static bool[][] CreateBatchConfigurations()
        {
            var batchConfigurationPermutations = Enumerable.Empty<int[]>();

            for (int i = 0; i < defaultBatchSizes.Length; ++i)
            {
                int length = defaultBatchSizes[i];
                int[] configs = new int[length];
                for (int j = 0; j < length; ++j)
                {
                    configs[j] = j;
                }

                batchConfigurationPermutations = batchConfigurationPermutations.Concat(configs.Permutations());
            }

            HashSet<bool[]> batchConfigurations = new HashSet<bool[]>(BoolArrayEqualityComparer.Default);

            foreach (int[] permutation in batchConfigurationPermutations)
            {
                int length = permutation.Length;

                // only using some counts to improve performance
                int[] queryOperationCounts = new int[] { 0, length, length / 2 };

                foreach (int queryOperationCount in queryOperationCounts)
                {
                    int changeSetCount = length - queryOperationCount;

                    bool[] batchConfiguration = new bool[length];

                    for (int i = 0; i < length; ++i)
                    {
                        if (permutation[i] < queryOperationCount)
                        {
                            batchConfiguration[i] = true;
                        }
                        else
                        {
                            batchConfiguration[i] = false;
                        }
                    }

                    // only adds the configuration if it does not already exist
                    if (!batchConfigurations.Contains(batchConfiguration))
                    {
                        batchConfigurations.Add(batchConfiguration);
                    }
                }
            }

            return batchConfigurations.ToArray();
        }

        private static BatchWriterTestDescriptor.InvocationAndOperationDescriptor[][] CreateBatchTestCases(
            BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] queryOperations,
            BatchWriterTestDescriptor.InvocationAndOperationDescriptor[][] changeSets,
            ICombinatorialEngineProvider combinatorialEngine)
        {
            bool[][] batchConfigurations = CreateBatchConfigurations();

            var batchDescriptors = new List<BatchWriterTestDescriptor.InvocationAndOperationDescriptor[]>();

            var queryOperationsCache = new Dictionary<int, BatchWriterTestDescriptor.InvocationAndOperationDescriptor[][]>();
            var changeSetsCache = new Dictionary<int, BatchWriterTestDescriptor.InvocationAndOperationDescriptor[][][]>();

            for (int i = 0; i < batchConfigurations.Length; ++i)
            {
                bool[] batchConfiguration = batchConfigurations[i];
                int length = batchConfiguration.Length;
                int queryOperationCount = batchConfiguration.Where(b => b == true).Count();
                int changeSetCount = length - queryOperationCount;

                BatchWriterTestDescriptor.InvocationAndOperationDescriptor[][] selectedQueryOperations;
                if (!queryOperationsCache.TryGetValue(queryOperationCount, out selectedQueryOperations))
                {
                    selectedQueryOperations = queryOperations.Subsets(queryOperationCount).ToArray();
                    queryOperationsCache[queryOperationCount] = selectedQueryOperations;
                }

                BatchWriterTestDescriptor.InvocationAndOperationDescriptor[][][] selectedChangeSets;
                if (!changeSetsCache.TryGetValue(changeSetCount, out selectedChangeSets))
                {
                    selectedChangeSets = changeSets.Subsets(changeSetCount).ToArray();
                    changeSetsCache[changeSetCount] = selectedChangeSets;
                }

                combinatorialEngine.RunCombinations(
                    selectedQueryOperations,
                    selectedChangeSets,
                    (queryOp, changeSet) =>
                    {
                        int queryIx = 0;
                        int changeSetIx = 0;

                        var batchDescriptor = new List<BatchWriterTestDescriptor.InvocationAndOperationDescriptor>();
                        batchDescriptor.Add(BatchWriterUtils.StartBatch());

                        for (int j = 0; j < length; ++j)
                        {
                            if (batchConfiguration[j])
                            {
                                batchDescriptor.Add(queryOp[queryIx]);
                                queryIx++;
                            }
                            else
                            {
                                batchDescriptor.Add(BatchWriterUtils.StartChangeSet());
                                batchDescriptor.AddRange(changeSet[changeSetIx]);
                                batchDescriptor.Add(BatchWriterUtils.EndChangeSet());
                                changeSetIx++;
                            }
                        }

                        batchDescriptor.Add(BatchWriterUtils.EndBatch());

                        batchDescriptors.Add(batchDescriptor.ToArray());
                    });
            }

            return batchDescriptors.ToArray();
        }

        internal sealed class ODataPayload
        {
            public object[] Items { get; set; }
            public WriterTestConfiguration TestConfiguration { get; set; }
            public string ExpectedResult { get; set; }
            public WriterTestExpectedResults WriterTestExpectedResults { get; set; }
        }

        private sealed class BoolArrayEqualityComparer : IEqualityComparer<bool[]>
        {
            internal static readonly BoolArrayEqualityComparer Default = new BoolArrayEqualityComparer();

            private BoolArrayEqualityComparer()
            {
            }

            public bool Equals(bool[] x, bool[] y)
            {
                if (x == null && y == null)
                {
                    return true;
                }
                else if (x == null || y == null)
                {
                    return false;
                }

                if (object.ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x.Length != y.Length)
                {
                    return false;
                }

                for (int i = 0; i < x.Length; ++i)
                {
                    if (x[i] != y[i]) return false;
                }

                return true;
            }

            public int GetHashCode(bool[] obj)
            {
                int hc = 0;

                for (int i = 0; i < obj.Length; ++i)
                {
                    if (obj[i])
                    {
                        hc += 1 >> i;
                    }
                    else
                    {
                        hc -= 1 >> i;
                    }
                }

                return hc;
            }
        }
    }
}
