//---------------------------------------------------------------------
// <copyright file="InternalTestHookTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.IO;
    using System.Net;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces
    /// <summary>
    /// Tests for internal DataServiceContext test hooks used for header and payload verification
    /// </summary>
    [TestClass]
    public class InternalTestHookTests
    {
        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
        [Ignore] // Remove Atom
        // [TestMethod]
        public void ResponseHeadersAndStreamExceptionTest()
        {
            // Execute a query using a variety of methods (including sync, async, batch) and verify the response headers and payloads
            using (PlaybackService.OverridingPlayback.Restore())
            using (PlaybackService.ProcessRequestOverride.Restore())
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.ServiceType = typeof(PlaybackService);
                request.StartService();

                TestUtil.RunCombinations(((IEnumerable<QueryMode>)Enum.GetValues(typeof(QueryMode))), (queryMode) =>
                {
                    bool isBatchQuery = queryMode == QueryMode.BatchAsyncExecute || queryMode == QueryMode.BatchAsyncExecuteWithCallback || queryMode == QueryMode.BatchExecute;
                    PlaybackService.ProcessRequestOverride.Value = (r) => { throw new InvalidOperationException("ResponseHeadersAndStreamExceptionTest -- Bad Request."); };

                    DataServiceContext context = new DataServiceContext(new Uri(request.BaseUri));
                    HttpTestHookConsumer testHookConsumer = new HttpTestHookConsumer(context, false);

                    DataServiceQuery<Customer> query = context.CreateQuery<Customer>("Customers");
                    Exception ex = null;
                    try
                    {
                        foreach (var o in DataServiceContextTestUtil.ExecuteQuery(context, query, queryMode))
                        {
                        }
                    }
                    catch (Exception e)
                    {
                        ex = e;
                    }

                    // Verify response headers
                    Assert.AreEqual(1, testHookConsumer.ResponseHeaders.Count, "Wrong number of response headers being tracked by the test hook.");
                    Dictionary<string, string> actualResponseHeaders = testHookConsumer.ResponseHeaders[0];
                    Assert.AreEqual("InternalServerError", actualResponseHeaders["__HttpStatusCode"]);

                    // Verify response stream
                    Assert.AreEqual(1, testHookConsumer.ResponseWrappingStreams.Count, "Unexpected number of response streams tracked by the test hook.");
                    string actualResponsePayload = testHookConsumer.ResponseWrappingStreams[0].GetLoggingStreamAsString();
                    if (queryMode == QueryMode.BatchExecute)
                    {
                        Assert.AreEqual("", actualResponsePayload, "In batch the client calls the hook to get the stream but never reads from it.");
                    }
                    else
                    {
                        TestUtil.AssertContains(actualResponsePayload, "System.InvalidOperationException: ResponseHeadersAndStreamExceptionTest -- Bad Request.");
                    }

                    // Sanity check on the count of request streams, but not verifying them here. That functionality is tested more fully in another test method.
                    int expectedRequestStreamsCount = isBatchQuery ? 1 : 0;
                    Assert.AreEqual(expectedRequestStreamsCount, testHookConsumer.RequestWrappingStreams.Count, "Unexpected number of request streams.");
                });
            }
        }

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
        [Ignore] // Remove Atom
        // [TestMethod]
        public void RequestHeadersAndStreamTest()
        {
            // SaveChanges with multiple changes, using a variety of configurations (including sync, async, batch) and verify the request headers and payloads
            using (PlaybackService.InspectRequestPayload.Restore())
            using (PlaybackService.OverridingPlayback.Restore())
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.ServiceType = typeof(PlaybackService);
                request.StartService();

                Customer newCustomer1 = new Customer();
                newCustomer1.Address = new Address();
                newCustomer1.Address.City = "CustCity1";
                newCustomer1.GuidValue = new Guid();
                newCustomer1.Name = "CustName1";

                Customer newCustomer2 = new Customer();
                newCustomer2.ID = 2;
                newCustomer2.Address = new Address();
                newCustomer2.Address.City = "CustCity2";
                newCustomer2.GuidValue = new Guid();
                newCustomer2.Name = "CustName2";

                Dictionary<string, string> responseHeaders1 = new Dictionary<string, string>();
                responseHeaders1.Add("Content-ID", newCustomer1.ID.ToString());
                responseHeaders1.Add("Location", String.Format("{0}/Customers({1})", request.BaseUri, newCustomer1.ID));

                Dictionary<string, string> responseHeaders2 = new Dictionary<string, string>();
                responseHeaders2.Add("Content-ID", newCustomer2.ID.ToString());
                responseHeaders2.Add("Location", String.Format("{0}/Customers({1})", request.BaseUri, newCustomer2.ID));

                string[] nonBatchHttpResponses = new string[]
                {
                    PlaybackService.ConvertToPlaybackServicePayload(responseHeaders1, null, HttpStatusCode.Created),
                    PlaybackService.ConvertToPlaybackServicePayload(responseHeaders2, null, HttpStatusCode.Created)
                };

                string batchPayload;
                string batchHttpResponse = PlaybackService.ConvertToBatchResponsePayload(nonBatchHttpResponses, false, out batchPayload);

                var saveChangesModes = (IEnumerable<SaveChangesMode>)Enum.GetValues(typeof(SaveChangesMode));
                var saveChangesOptions = new SaveChangesOptions[] { SaveChangesOptions.None, SaveChangesOptions.BatchWithSingleChangeset };

                Dictionary<string, string> expectedRequestHeadersBatch = new Dictionary<string, string>();
                expectedRequestHeadersBatch.Add("Content-Type", "multipart/mixed; boundary=batch_");
                expectedRequestHeadersBatch.Add("Accept", "multipart/mixed");
                AddCommonRequestHeaders(expectedRequestHeadersBatch);
                expectedRequestHeadersBatch.Add("__Uri", String.Format("{0}/$batch", request.BaseUri));

                Dictionary<string, string> expectedRequestHeadersNonBatch = new Dictionary<string, string>();
                expectedRequestHeadersNonBatch.Add("Content-Type", "application/atom+xml");
                expectedRequestHeadersNonBatch.Add("Accept", "application/atom+xml,application/xml");
                AddCommonRequestHeaders(expectedRequestHeadersNonBatch);
                expectedRequestHeadersNonBatch.Add("__Uri", String.Format("{0}/Customers", request.BaseUri));

                TestUtil.RunCombinations(saveChangesModes, saveChangesOptions, (saveChangesMode, saveChangesOption) =>
                    {
                        DataServiceContext context = new DataServiceContext(new Uri(request.BaseUri));
                        //context.EnableAtom = true;
                        //context.Format.UseAtom();
                        HttpTestHookConsumer testHookConsumer = new HttpTestHookConsumer(context, false);

                        bool isBatch = saveChangesOption == SaveChangesOptions.BatchWithSingleChangeset;
                        int actualRequestCount = 0;
                        PlaybackService.InspectRequestPayload.Value = (requestStream) =>
                        {
                            // Verify request headers
                            Assert.AreEqual(1, testHookConsumer.RequestHeaders.Count, "Wrong number of request headers tracked by the test hook");
                            Dictionary<string, string> actualRequestHeaders = testHookConsumer.RequestHeaders[0];
                            Dictionary<string, string> expectedRequestHeaders = isBatch ? expectedRequestHeadersBatch : expectedRequestHeadersNonBatch;
                            VerifyHeaders(expectedRequestHeaders, actualRequestHeaders);
                            testHookConsumer.RequestHeaders.Clear();

                            // Verify request stream
                            Assert.AreEqual(1, testHookConsumer.RequestWrappingStreams.Count, "Wrong number of request streams tracked by the test hook");
                            string actualString = testHookConsumer.RequestWrappingStreams[0].GetLoggingStreamAsString();
                            StreamReader reader = new StreamReader(requestStream);
                            string expectedString = reader.ReadToEnd();
                            Assert.AreEqual(expectedString, actualString, "Request stream does not contain the expected value in the test hook.");
                            testHookConsumer.RequestWrappingStreams.Clear();

                            // Set the response payload here because a single SaveChanges call can produce multiple requests that
                            // require different responses, when not using batching.
                            string httpResponse = isBatch ? batchHttpResponse : nonBatchHttpResponses[actualRequestCount];
                            PlaybackService.OverridingPlayback.Value = httpResponse;
                            actualRequestCount++;
                        };

                        // Add multiple objects to ensure that multiple streams in a single API call will still get passed to the test hook
                        context.AddObject("Customers", newCustomer1);
                        context.AddObject("Customers", newCustomer2);

                        // Verify no requests have been made yet
                        Assert.AreEqual(0, actualRequestCount, "No HTTP requests should have been made yet.");

                        DataServiceContextTestUtil.SaveChanges(context, saveChangesOption, saveChangesMode);

                        // Verify that the expected number of requests were made during SaveChanges
                        int expectedRequestCount = isBatch ? 1 : 2;
                        Assert.AreEqual(expectedRequestCount, actualRequestCount, "Wrong number of HTTP requests made during SaveChanges.");
                    });
            }
        }

        private static void VerifyHeaders(Dictionary<string, string> expectedHeaders, Dictionary<string, string> actualHeaders)
        {
            Assert.AreEqual(expectedHeaders.Count, actualHeaders.Count, "Wrong number of response headers.");
            foreach (string expectedHeaderKey in expectedHeaders.Keys)
            {
                Assert.IsTrue(actualHeaders.ContainsKey(expectedHeaderKey), "Headers do not contain the header {0}.", expectedHeaderKey);
                // Verify value of all headers except Date, because it will change with each response
                if (expectedHeaderKey == "Content-Type")
                {
                    // For batch, content type contains a GUID that can change on each request or response, so just verify it starts with the expected value
                    Assert.IsTrue(actualHeaders[expectedHeaderKey].StartsWith(expectedHeaders[expectedHeaderKey]), "Header Content-Type does not start with the expected value.");
                }
                else if (expectedHeaderKey != "Date")
                {
                    Assert.AreEqual(expectedHeaders[expectedHeaderKey], actualHeaders[expectedHeaderKey], "Header {0} is not the expected value.", expectedHeaderKey);
                }
            }
        }

        private static void AddCommonRequestHeaders(Dictionary<string, string> headers)
        {
            headers.Add("User-Agent", "Microsoft ADO.NET Data Services");
            headers.Add("OData-Version", "4.0");
            headers.Add("OData-MaxVersion", "4.0");
            headers.Add("Accept-Charset", "UTF-8");
            headers.Add("__HttpVerb", "POST");
        }
    }
}
