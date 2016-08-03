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
    using System.Data.Test.Astoria;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using AstoriaUnitTests.Stubs;
    using Microsoft.OData.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces
    /// <summary>
    /// Tests for internal DataServiceContext test hooks used for header and payload verification
    /// </summary>
    [TestClass]
    public class InternalTestHookTests
    {
        [TestMethod]
        public void ResponseHeadersAndStreamTest()
        {
            // Execute a query using a variety of methods (including sync, async, batch) and verify the response headers and payloads
            using (PlaybackService.OverridingPlayback.Restore())
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.ServiceType = typeof(PlaybackService);
                request.StartService();

                string nonbatchPayload =
@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<entry xml:base=""http://sparradevvm1:34866/TheTest/"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" m:etag=""W/&quot;3d9e3978-e6a7-4742-a44e-ef5a43d18d5f&quot;"" xmlns=""http://www.w3.org/2005/Atom"">
  <id>http://sparradevvm1:34866/TheTest/Customers(1)</id>
  <title type=""text""></title>
  <updated>2010-10-12T18:07:17Z</updated>
  <author>
    <name />
  </author>
  <link rel=""edit"" title=""Customer"" href=""Customers(1)"" />
  <link rel=""http://docs.oasis-open.org/odata/ns/related/BestFriend"" type=""application/atom+xml;type=entry"" title=""BestFriend"" href=""Customers(1)/BestFriend"" />
  <link rel=""http://docs.oasis-open.org/odata/ns/related/Orders"" type=""application/atom+xml;type=feed"" title=""Orders"" href=""Customers(1)/Orders"" />
  <category term=""#AstoriaUnitTests.Stubs.CustomerWithBirthday"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
  <content type=""application/xml"">
    <m:properties>
      <d:GuidValue m:type=""Edm.Guid"">3d9e3978-e6a7-4742-a44e-ef5a43d18d5f</d:GuidValue>
      <d:ID m:type=""Edm.Int32"">1</d:ID>
      <d:Name>Customer 1</d:Name>
      <d:NameAsHtml>&lt;html&gt;&lt;body&gt;Customer 1&lt;/body&gt;&lt;/html&gt;</d:NameAsHtml>
      <d:Birthday m:type=""Edm.DateTimeOffset"">1980-10-12T00:00:00-07:00</d:Birthday>
      <d:Address m:type=""AstoriaUnitTests.Stubs.Address"">
        <d:StreetAddress>Line1</d:StreetAddress>
        <d:City>Redmond</d:City>
        <d:State>WA</d:State>
        <d:PostalCode>98052</d:PostalCode>
      </d:Address>
    </m:properties>
  </content>
</entry>
";
                string nonbatchHttpResponse = PlaybackService.ConvertToPlaybackServicePayload(null, nonbatchPayload);
                string batchPayload;
                string batchHttpResponse = PlaybackService.ConvertToBatchResponsePayload(new string[] { nonbatchHttpResponse }, true, out batchPayload);

                Dictionary<string, string> nonbatchHeaders = new Dictionary<string, string>();
                nonbatchHeaders.Add("Content-Type", "application/atom+xml");
                nonbatchHeaders.Add("__HttpStatusCode", "OK");
                nonbatchHeaders.Add("Content-Length", nonbatchPayload.Length.ToString());
                nonbatchHeaders.Add("Server", "Microsoft-HTTPAPI/2.0");
                nonbatchHeaders.Add("Date", null);

                Dictionary<string, string> batchHeaders = new Dictionary<string, string>();
                batchHeaders.Add("Content-Type", "multipart/mixed; boundary=batchresponse_e9b231d9-72ab-46ea-9613-c7e8f5ece46b");
                batchHeaders.Add("__HttpStatusCode", "Accepted");
                batchHeaders.Add("Content-Length", batchPayload.Length.ToString());
                batchHeaders.Add("Server", "Microsoft-HTTPAPI/2.0");
                batchHeaders.Add("Date", null);

                TestUtil.RunCombinations(((IEnumerable<QueryMode>)Enum.GetValues(typeof(QueryMode))), (queryMode) =>
                    {
                        Dictionary<string, string> expectedResponseHeaders;
                        string expectedResponsePayload;
                        bool isBatchQuery = queryMode == QueryMode.BatchAsyncExecute || queryMode == QueryMode.BatchAsyncExecuteWithCallback || queryMode == QueryMode.BatchExecute;
                        if (isBatchQuery)
                        {
                            PlaybackService.OverridingPlayback.Value = batchHttpResponse;
                            expectedResponseHeaders = batchHeaders;
                            expectedResponsePayload = batchPayload;
                        }
                        else
                        {
                            PlaybackService.OverridingPlayback.Value = nonbatchHttpResponse;
                            expectedResponseHeaders = nonbatchHeaders;
                            expectedResponsePayload = nonbatchPayload;
                        }

                        DataServiceContext context = new DataServiceContext(new Uri(request.BaseUri));
                        context.EnableAtom = true;
                        HttpTestHookConsumer testHookConsumer = new HttpTestHookConsumer(context, false);

                        DataServiceQuery<Customer> query = context.CreateQuery<Customer>("Customers");
                        foreach (var o in DataServiceContextTestUtil.ExecuteQuery(context, query, queryMode))
                        {
                        }

                        // Verify response headers
                        Assert.AreEqual(1, testHookConsumer.ResponseHeaders.Count, "Wrong number of response headers being tracked by the test hook.");
                        Dictionary<string, string> actualResponseHeaders = testHookConsumer.ResponseHeaders[0];
                        VerifyHeaders(expectedResponseHeaders, actualResponseHeaders);

                        // Verify response stream
                        Assert.AreEqual(1, testHookConsumer.ResponseWrappingStreams.Count, "Unexpected number of response streams tracked by the test hook.");
                        string actualResponsePayload = testHookConsumer.ResponseWrappingStreams[0].GetLoggingStreamAsString();
                        Assert.AreEqual(expectedResponsePayload, actualResponsePayload, "Response payload was not the value that was expected");

                        // Sanity check on the count of request streams, but not verifying them here. That functionality is tested more fully in another test method.
                        int expectedRequestStreamsCount = isBatchQuery ? 1 : 0;
                        Assert.AreEqual(expectedRequestStreamsCount, testHookConsumer.RequestWrappingStreams.Count, "Unexpected number of request streams.");
                    });
            }
        }

        [TestMethod]
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

        [TestMethod]
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
                        context.EnableAtom = true;
                        context.Format.UseAtom();
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
            Version assemblyVersion = typeof(HeaderCollection).GetAssembly().GetName().Version;
            headers.Add("User-Agent", string.Format(CultureInfo.InvariantCulture, "Microsoft.OData.Client/{0}.{1}.{2}", assemblyVersion.Major, assemblyVersion.Minor, assemblyVersion.Build));
            headers.Add("OData-Version", "4.0");
            headers.Add("OData-MaxVersion", "4.0");
            headers.Add("Accept-Charset", "UTF-8");
            headers.Add("__HttpVerb", "POST");
        }
    }
}
