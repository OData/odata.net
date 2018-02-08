//---------------------------------------------------------------------
// <copyright file="ModifyHeadersScenarioTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Linq;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Text.RegularExpressions;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Tests.Server;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/875
    /// <summary>
    /// Tests to ensure that service authors can overwrite header values in our public hooks and the changes are persisted.
    /// </summary>
    [TestClass]
    public class ModifyHeadersScenarioTests
    {
        private Version V4 = new Version(4, 0);
        private static readonly Type[] Services = new Type[] {typeof(ModifyHeaderOnStartProcessingRequestTestService), typeof(ModifyHeadersInProcessingRequestEventTestService)};

        /// <summary>
        /// Tests that both the ProcessingRequest event and OnStartProcessingRequest can overrwite the accept header
        /// with a header and the server uses the new value.
        /// </summary>
        [TestMethod, TestCategory("Partition1")]
        public void CanSetOverrideAcceptHeaderWithHeader()
        {
            Action<TestWebRequest> configureRequest = (TestWebRequest request) =>
            {
                request.RequestUriString = "/Customers(1)/Address";
                request.Accept = UnitTestsUtil.MimeApplicationXml;
                request.RequestHeaders["Override-Accept"] = UnitTestsUtil.JsonLightMimeType;
            };
            Action<TestWebRequest> validateResponse = (TestWebRequest request) =>
                Assert.IsTrue(request.ResponseContentType.ToLower().Contains(UnitTestsUtil.JsonLightMimeType.ToLower()));

            RunTest(configureRequest, validateResponse);
        }

        /// <summary>
        /// Tests that both the ProcessingRequest event and OnStartProcessingRequest can overrwite the accept header
        /// with a query item and the server uses the new value.
        /// </summary>
        [TestMethod, TestCategory("Partition1")]
        public void CanSetOverrideAcceptHeaderWithQueryItem()
        {
            Action<TestWebRequest> configureRequest = (TestWebRequest request) =>
            {
                request.RequestUriString = "/Customers(1)/Address?Override-Accept=" + UnitTestsUtil.JsonLightMimeType;
                request.Accept = UnitTestsUtil.MimeApplicationXml;
            };
            Action<TestWebRequest> validateResponse = (TestWebRequest request) => 
                Assert.IsTrue(request.ResponseContentType.ToLower().Contains(UnitTestsUtil.JsonLightMimeType.ToLower()));

            RunTest(configureRequest, validateResponse);
        }

        /// <summary>
        /// Tests that both the ProcessingRequest event and OnStartProcessingRequest can overrwite the MaxDataServiceVersion header 
        /// with a custom header and the server uses the new value.
        /// </summary>
        [TestMethod, TestCategory("Partition1")]
        public void CanSetOverrideMaxDataServiceVersionHeaderWithHeader()
        {
            Action<TestWebRequest> configureRequest = (TestWebRequest request) =>
            {
                request.RequestUriString = "/Customers(1)/Address";
                request.Accept = UnitTestsUtil.JsonLightMimeType;
                request.RequestMaxVersion = "4.0";
                request.RequestHeaders["Override-MaxDataServiceVersion"] = "4.0";
            };
            Action<TestWebRequest> validateResponse = (TestWebRequest request) =>
                Assert.IsTrue(request.ResponseContentType.ToLower().Contains(UnitTestsUtil.JsonLightMimeType.ToLower()));

            RunTest(configureRequest, validateResponse);
        }

        /// <summary>
        /// Tests that both the ProcessingRequest event and OnStartProcessingRequest can overrwite the MaxDataServiceVersion header
        /// with a query item and the server uses the new value.
        /// </summary>
        [TestMethod, TestCategory("Partition1")]
        public void CanSetOverrideMaxDataServiceVersionHeaderWithQueryString()
        {
            Action<TestWebRequest> configureRequest = (TestWebRequest request) =>
            {
                request.RequestUriString = "/Customers(1)/Address?Override-MaxDataServiceVersion=4.0";
                request.Accept = UnitTestsUtil.JsonLightMimeType;
                request.RequestMaxVersion = "4.0";
            };
            Action<TestWebRequest> validateResponse = (TestWebRequest request) =>
                Assert.IsTrue(request.ResponseContentType.ToLower().Contains(UnitTestsUtil.JsonLightMimeType.ToLower()));

            RunTest(configureRequest, validateResponse);
        }

        /// <summary>
        /// Tests that both the ProcessingRequest event and OnStartProcessingRequest can overrwite the X-Http-Method header 
        /// with a custom header and the server uses the new value.
        /// </summary>
        [TestMethod, TestCategory("Partition1")]
        public void CanOverloadPostTunnelingWithHeader()
        {
            Action<TestWebRequest> configureRequest = (TestWebRequest request) =>
            {
                request.RequestUriString = "/Customers(1)";
                request.HttpMethod = "POST";
                request.Accept = UnitTestsUtil.JsonLightMimeType;
                request.RequestContentLength = 0;
                request.IfMatch = "*";
                request.RequestHeaders["Custom-Post-Tunneling"] = "DELETE";
            };
            Action<TestWebRequest> validateResponse = (TestWebRequest request) =>
                Assert.AreEqual(204, request.ResponseStatusCode);

            RunTest(configureRequest, validateResponse);
        }

        /// <summary>
        /// Tests that both the ProcessingRequest event and OnStartProcessingRequest can overrwite the X-Http-Method header 
        /// with a query item and the server uses the new value.
        /// </summary>
        [TestMethod, TestCategory("Partition1")]
        public void CanOverloadPostTunnelingWithQueryItem()
        {
            Action<TestWebRequest> configureRequest = (TestWebRequest request) =>
            {
                request.RequestUriString = "/Customers(1)?Custom-Post-Tunneling=DELETE";
                request.HttpMethod = "POST";
                request.Accept = UnitTestsUtil.JsonLightMimeType;
                request.RequestContentLength = 0;
                request.IfMatch = "*";
            };
            Action<TestWebRequest> validateResponse = (TestWebRequest request) =>
                Assert.AreEqual(204, request.ResponseStatusCode);

            RunTest(configureRequest, validateResponse);
        }

        /// <summary>
        /// Tests that if ProcessingRequest event or OnStartProcessingRequest throws, the server will throw with that error status code 
        /// and message.
        /// </summary>
        [TestMethod, TestCategory("Partition1")]
        public void UserCodeForcedErrorIsThrown()
        {
            Action<TestWebRequest> configureRequest = (TestWebRequest request) =>
            {
                request.RequestUriString = "/Customers(1)/Address?$format=json";
                request.RequestHeaders["Force-Error"] = "yes";
            };
            Action<TestWebRequest> validateResponse = (TestWebRequest request) =>
            {
                var responseBody = request.GetResponseStreamAsText();
                Assert.IsTrue(responseBody.Contains("\"error\":") && responseBody.Contains("teapot"));
                Assert.AreEqual(418, request.ResponseStatusCode);
            };

            RunTest(configureRequest, validateResponse);
        }

        /// <summary>
        /// Tests that if ProcessingRequest event or OnStartProcessingRequest throws and we have bad version header combinations,
        /// that we will get the right format anyway.
        /// </summary>
        [TestMethod, TestCategory("Partition1")]
        public void UserCodeForcedErrorAndIfVersionHeadersDontMakeSenseWeGetTheRequestedContentTypeAnyway()
        {
            Action<TestWebRequest> configureRequest = (TestWebRequest request) =>
            {
                request.Accept = UnitTestsUtil.JsonLightMimeType;
                request.RequestUriString = "/Customers(1)/Address";
                request.RequestHeaders["Force-Error"] = "yes";
                request.RequestMaxVersion = "4.0";
            };
            Action<TestWebRequest> validateResponse = (TestWebRequest request) =>
            {
                var responseBody = request.GetResponseStreamAsText();
                Assert.IsTrue(responseBody.Contains("error") && responseBody.Contains("teapot"));
                Assert.AreEqual(418, request.ResponseStatusCode);
            };

            RunTest(configureRequest, validateResponse);
        }

        /// <summary>
        /// Tests that both the ProcessingRequest event and OnStartProcessingRequest can overrwite the header values of various types
        /// with a query item and the server uses the new value. Also test multiple query string headers in Uri.
        /// </summary>
        [Ignore] // Remove Atom
        // [TestMethod, TestCategory("Partition1")]
        public void SetOverrideVariousTypeHeaderValueWithQueryString()
        {
            Action<TestWebRequest> configureRequest = (TestWebRequest request) =>
            {
                request.RequestUriString = "/Customers(1)?Override-Accept=" + Uri.EscapeDataString("application/*,application/atom+xml;") + "&Override-Date=" + Uri.EscapeDataString("Wed, 01 Aug 2012 23:23:21 GMT") + "&Override-MyHeader=200";
                request.Accept = UnitTestsUtil.JsonLightMimeType;
            };
            Action<TestWebRequest> validateResponse = (TestWebRequest request) =>
            {
                Assert.AreEqual(200, request.ResponseStatusCode);
                Assert.IsTrue(request.ResponseContentType.Contains("application/atom+xml;"));
                // Date is a system header and our service will override the user custom value in response
                Assert.AreNotEqual("Wed, 01 Aug 2012 23:23:21 GMT", request.ResponseHeaders["Date"]);

                // The following response headers are written in ProcessingRequest/OnStartProcessingRequest
                Assert.AreEqual("Wed, 01 Aug 2012 23:23:21 GMT", request.ResponseHeaders["MyDate"]);
                Assert.AreEqual("200", request.ResponseHeaders["MyHeader"]);
            };

            RunTest(configureRequest, validateResponse);
        }

        /// <summary>
        /// Tests that both the ProcessingRequest event and OnStartProcessingRequest can overrwite the headers with a query item and the server uses the new value
        /// if there are service operation parameters in the Uri.
        /// </summary>
        [TestMethod, TestCategory("Partition1")]
        public void ServiceOperationParameterWithQueryStringHeader()
        {
            List<string> uriStrings = new List<string>()
            {
                "/GetCustomer?Override-Accept="+ UnitTestsUtil.JsonLightMimeType + "&param1='blahblah'&param2=200" + "&Override-MyHeader=myheadervalue",
                "/GetCustomer?param1='blahblah'&Override-Accept="+ UnitTestsUtil.JsonLightMimeType + "&Override-MyHeader=myheadervalue" + "&param2=200",
            };

            foreach (string uriString in uriStrings)
            {
                Action<TestWebRequest> configureRequest = (TestWebRequest request) =>
                {
                    request.RequestUriString = uriString;
                    request.Accept = UnitTestsUtil.AtomFormat;
                };
                Action<TestWebRequest> validateResponse = (TestWebRequest request) =>
                {
                    Assert.AreEqual(200, request.ResponseStatusCode);
                    Assert.IsTrue(request.ResponseContentType.ToLower().Contains(UnitTestsUtil.JsonLightMimeType.ToLower()));

                    // The following response header is written in ProcessingRequest/OnStartProcessingRequest
                    Assert.AreEqual("myheadervalue", request.ResponseHeaders["MyHeader"]);
                };

                RunTest(configureRequest, validateResponse);
            }
        }

        /// <summary>
        /// Tests GetQueryStringValue(headerName) behaviors with edge case headerName values.
        /// </summary>
        [Ignore] // Remove Atom
        // [TestMethod, TestCategory("Partition1")]
        public void GetQueryStringItemEdgeTest()
        {
            using (TestWebRequest webRequest = TestWebRequest.CreateForInProcessWcf())
            {
                webRequest.HttpMethod = "GET";
                webRequest.DataServiceType = typeof(ModifyHeaderOnStartProcessingRequestTestService);
                webRequest.Accept = UnitTestsUtil.AtomFormat;

                // query string header name is not case sensitive for GetQueryStringValue
                webRequest.RequestUriString = "/Customers(1)?TestCaseSensitive=true" + "&lowercaseheader=lowercaseheadervalue" + "&UPPERCASEHEADER=UPPERCASEHEADERVALUE";
                TestUtil.RunCatching(webRequest.SendRequest);
                Assert.AreEqual(200, webRequest.ResponseStatusCode);
                // The following response headers are written in ProcessingRequest/OnStartProcessingRequest
                Assert.AreEqual("lowercaseheadervalue", webRequest.ResponseHeaders["lowercaseheader"]);
                Assert.AreEqual("UPPERCASEHEADERVALUE", webRequest.ResponseHeaders["UPPERCASEHEADER"]);

                // GetQueryStringValue(headerName) returns null if the headerName does not exist in Uri
                webRequest.RequestUriString = "/Customers(1)?TestNonExisting=true" + "&MyHeader=MyHeaderValue";
                TestUtil.RunCatching(webRequest.SendRequest);
                Assert.AreEqual(200, webRequest.ResponseStatusCode);

                // GetQueryStringValue(headerName) returns null if the headerName is null
                webRequest.RequestUriString = "/Customers(1)?TestNullKey=true" + "&=MyHeaderValue";
                TestUtil.RunCatching(webRequest.SendRequest);
                Assert.AreEqual(200, webRequest.ResponseStatusCode);

                // GetQueryStringValue(headerName) returns "" if the header value is not specified
                webRequest.RequestUriString = "/Customers(1)?TestNullValue=true" + "&MyHeader=";
                TestUtil.RunCatching(webRequest.SendRequest);
                Assert.AreEqual(200, webRequest.ResponseStatusCode);

                // duplicate query string header
                webRequest.RequestUriString = "/Customers(1)?TestDuplicateHeader=true&MyHeader=v1" + "&MyHeader=v2";
                TestUtil.RunCatching(webRequest.SendRequest);
                Assert.AreEqual(400, webRequest.ResponseStatusCode);
            }
        }

        /// <summary>
        /// Tests that both the ProcessingRequest event and OnStartProcessingRequest can write invalid MaxDataServiceVersion header
        /// with a query item and the server uses the new value.
        /// </summary>
        [TestMethod, TestCategory("Partition1")]
        public void SetOverrideBadMaxDataServiceVersionHeaderWithQueryString()
        {
            Action<TestWebRequest> configureRequest = (TestWebRequest request) =>
            {
                request.RequestUriString = "/Customers(1)/Address?Override-MaxDataServiceVersion=4.a;";
                request.Accept = UnitTestsUtil.JsonLightMimeType;
                request.RequestMaxVersion = "4.0";
            };
            Action<TestWebRequest> validateResponse = (TestWebRequest request) =>
            {
                Assert.AreEqual(400, request.ResponseStatusCode);
                Assert.IsTrue(request.GetResponseStreamAsText().Contains("Request version '4.a;' specified for header 'OData-MaxVersion' is not valid"));
            };

            RunTest(configureRequest, validateResponse);
        }


        /// <summary>
        /// Tests that both the ProcessingRequest event and OnStartProcessingRequest can overrwite ETag and prefer headers 
        /// with a custom header and the server uses the new value.
        /// </summary>
        [TestMethod, TestCategory("Partition1")]
        public void CanOverloadUpdateRequestWithHeader()
        {
            foreach (var serviceType in Services)
            {
                using (CustomDataContext.CreateChangeScope())
                {
                    using (TestWebRequest webRequest = TestWebRequest.CreateForInProcessWcf())
                    {
                        // get an existing customer before sending PUT requests
                        webRequest.HttpMethod = "GET";
                        webRequest.ServiceType = serviceType;
                        webRequest.RequestUriString = "/Customers(1)";
                        webRequest.Accept = UnitTestsUtil.JsonLightMimeType;
                        TestUtil.RunCatching(webRequest.SendRequest);
                        Assert.IsTrue(webRequest.ResponseContentType.ToLower().Contains(UnitTestsUtil.JsonLightMimeType.ToLower()));

                        // all the subsequent requests are PUT requests with specified Accept and Content-Type headers
                        webRequest.HttpMethod = "PUT";
                        webRequest.Accept = UnitTestsUtil.JsonLightMimeType;
                        webRequest.RequestContentType = "application/json";

                        // update the customer and send the correct ETag in query string header - should succeed
                        webRequest.RequestUriString = "/Customers(1)/Name?Override-If-Match=" + webRequest.ResponseETag;
                        webRequest.IfMatch = "SomeBadValue";
                        webRequest.SetRequestStreamAsText("{\"value\": \"Name1\"}");
                        TestUtil.RunCatching(webRequest.SendRequest);
                        Assert.AreEqual(204, webRequest.ResponseStatusCode);
                        string eTag = webRequest.ResponseETag;

                        // update the customer and send ETag with reversed guid in query string header - should fail
                        // Length of a GUID is 36
                        string correctGuid = webRequest.ResponseETag.Substring(webRequest.ResponseETag.IndexOf('"') + 1, 36);
                        string reversedGuid = string.Concat(correctGuid.ToArray().Reverse());
                        webRequest.RequestUriString = "/Customers(1)/Name?Override-If-Match=" + webRequest.ResponseETag.Replace(correctGuid, reversedGuid);
                        webRequest.IfMatch = eTag;
                        webRequest.SetRequestStreamAsText("{\"value\": \"Name2\"}");
                        TestUtil.RunCatching(webRequest.SendRequest);
                        Assert.AreEqual(412, webRequest.ResponseStatusCode);

                        // update the customer and send prefer header return=representation in query string - expect 200
                        webRequest.RequestUriString = "/Customers(1)/Name?Override-Prefer=return=representation";
                        webRequest.IfMatch = eTag;
                        webRequest.SetRequestStreamAsText("{\"value\": \"Name3\"}");
                        TestUtil.RunCatching(webRequest.SendRequest);
                        Assert.AreEqual(200, webRequest.ResponseStatusCode);
                        Assert.IsTrue(webRequest.GetResponseStreamAsText().Contains("TheTest/$metadata#Customers(1)/Name\",\"value\":\"Name3\""));
                    }
                }
            }
        }

        /// <summary>
        /// Tests that server does not honor Content-Type, Content-Length header values from query string.
        /// </summary>
        [Ignore] // Remove Atom
        // [TestMethod, TestCategory("Partition1")]
        public void OverloadUpdateRequestWithContentTypeContentLengthHeader()
        {
            using (CustomDataContext.CreateChangeScope())
            {
                using (TestWebRequest webRequest = TestWebRequest.CreateForInProcessWcf())
                {
                    // get an existing customer before the PUT requests
                    webRequest.HttpMethod = "GET";
                    webRequest.ServiceType = typeof(ModifyHeaderOnStartProcessingRequestTestService);
                    webRequest.RequestUriString = "/Customers(1)";
                    webRequest.Accept = UnitTestsUtil.JsonLightMimeType;
                    TestUtil.RunCatching(webRequest.SendRequest);
                    Assert.IsTrue(webRequest.ResponseContentType.ToLower().Contains(UnitTestsUtil.JsonLightMimeType.ToLower()));

                    // all the subsequent requests are PUT requests with specified Accept If-Match, and Content-Type headers
                    webRequest.HttpMethod = "PUT";
                    webRequest.Accept = UnitTestsUtil.JsonLightMimeType;
                    webRequest.IfMatch = "*";
                    webRequest.RequestContentType = "application/json";

                    // override Content-Type header
                    webRequest.RequestUriString = "/Customers(1)/Name?Override-Content-Type=application/xml";
                    webRequest.SetRequestStreamAsText("{\"value\": \"Name1\"}");
                    TestUtil.RunCatching(webRequest.SendRequest);
                    Assert.AreEqual(400, webRequest.ResponseStatusCode);
                    // The following response header is written in ProcessingRequest/OnStartProcessingRequest
                    Assert.IsTrue(webRequest.GetResponseStreamAsText().Contains("Data at the root level is invalid."));

                    // override Content-Length header
                    webRequest.RequestUriString = "/Customers(1)/Name?Override-Content-Length=10000";
                    webRequest.SetRequestStreamAsText("{\"value\": \"Name1\"}");
                    TestUtil.RunCatching(webRequest.SendRequest);
                    Assert.AreEqual(204, webRequest.ResponseStatusCode);
                    // The following response header is written in ProcessingRequest/OnStartProcessingRequest
                    Assert.AreEqual("10000", webRequest.ResponseHeaders["Request-Content-Length"]);
                }
            }
        }

        /// <summary>
        /// Verify that the OnStartProcessingRequest Method can overwrite the accept header in a batch operation by 
        /// using a custom header.
        /// 
        /// The also is a regression test: Batch Operations with Json Light as the Content-Type do not bump the response DSV to 4.0.
        /// </summary>
        [TestMethod, TestCategory("Partition1")]
        public void OnStartProcessingRequestCanOverrideAcceptHeaderInBatchOperationWithHeader()
        {
            StringBuilder batchQueryOperation = new StringBuilder();
            batchQueryOperation.AppendLine("GET Customers(1)/Address HTTP/1.1");
            batchQueryOperation.AppendLine("Host: host");
            batchQueryOperation.AppendLine("Accept: " + UnitTestsUtil.MimeApplicationXml);
            batchQueryOperation.AppendLine("Override-Accept: " + UnitTestsUtil.JsonLightMimeType);

            var test = new SimpleBatchTestCase
            {
                RequestPayload = new BatchInfo(new BatchQuery(new Operation(batchQueryOperation.ToString()))),
                ExpectedResponsePayloadContains = new[] { "OData-Version: 4.0", "Content-Type: application/json;odata.metadata=minimal" },
                ResponseStatusCode = 202, 
                ResponseETag = default(string), 
                ResponseVersion = V4, 
                RequestDataServiceVersion = V4, 
                RequestMaxDataServiceVersion = V4,
            };

            RunBatchTest(test, typeof(ModifyHeaderOnStartProcessingRequestTestService));
        }

        /// <summary>
        /// Verify that the OnStartProcessingRequest Method can overwrite the accept header in a batch operation by 
        /// using a query item.
        /// </summary>
        [TestMethod, TestCategory("Partition1")]
        public void OnStartProcessingRequestCanOverrideAcceptHeaderInBatchOperationWithQueryItem()
        {
            StringBuilder batchQueryOperation = new StringBuilder();
            batchQueryOperation.AppendLine("GET Customers(1)/Address?Override-Accept=" + UnitTestsUtil.JsonLightMimeType + " HTTP/1.1");
            batchQueryOperation.AppendLine("Host: host");
            batchQueryOperation.AppendLine("Accept: " + UnitTestsUtil.MimeApplicationXml);

            var test = new SimpleBatchTestCase
            {
                RequestPayload = new BatchInfo(new BatchQuery(new Operation(batchQueryOperation.ToString()))),
                ExpectedResponsePayloadContains = new[] { "Content-Type: application/json;odata.metadata=minimal" },
                ResponseStatusCode = 202, 
                ResponseETag = default(string), 
                ResponseVersion = V4, 
                RequestDataServiceVersion = V4, 
                RequestMaxDataServiceVersion = V4,
            };

            RunBatchTest(test, typeof(ModifyHeaderOnStartProcessingRequestTestService));
        }

        /// <summary>
        /// Verify behaviors of overriding accept header of $batch request and its inner request in OnStartProcessingRequest/ProcessingRequest   
        /// </summary>
        [Ignore] // Remove Atom
        // [TestMethod, TestCategory("Partition1")]
        public void CanOverrideAcceptHeaderToBatchRequestWithQueryItem()
        {
            StringBuilder batchQueryOperation = new StringBuilder();
            batchQueryOperation.AppendLine("GET Customers(1)/Address?Override-Accept=" + UnitTestsUtil.JsonLightMimeType + " HTTP/1.1");
            batchQueryOperation.AppendLine("Host: host");
            batchQueryOperation.AppendLine("Accept: " + UnitTestsUtil.MimeApplicationXml);

            var test = new SimpleBatchTestCase
            {
                RequestPayload = new BatchInfo(new BatchQuery(new Operation(batchQueryOperation.ToString()))),
                ResponseStatusCode = 202,
                ResponseETag = default(string),
                ResponseVersion = V4,
                RequestDataServiceVersion = V4,
                RequestMaxDataServiceVersion = V4,
            };
            foreach (var serviceType in Services)
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.HttpMethod = "POST";
                    request.RequestUriString = "/$batch?Override-Batch-Accept=" + UnitTestsUtil.JsonLightMimeType;
                    request.DataServiceType = serviceType;
                    request.Accept = UnitTestsUtil.MimeMultipartMixed;
                    request.RequestVersion = test.RequestDataServiceVersion.ToString();
                    request.RequestMaxVersion = test.RequestMaxDataServiceVersion.ToString();
                    request.ForceVerboseErrors = true;
                    if (test.RequestPayload == null)
                    {
                        request.RequestContentLength = 0;
                    }
                    else
                    {
                        const string boundary = "batch-set";
                        request.RequestContentType = String.Format("{0}; boundary={1}", UnitTestsUtil.MimeMultipartMixed, boundary);
                        request.SetRequestStreamAsText(BatchRequestWritingUtils.GetBatchText(test.RequestPayload, boundary));
                    }

                    TestUtil.RunCatching(request.SendRequest);

                    // expect 202 as $batch request does not honor query string Accept header
                    Assert.AreEqual(test.ResponseStatusCode, request.ResponseStatusCode);
                    // The following response header is written in ProcessingRequest/OnStartProcessingRequest
                    Assert.AreEqual(UnitTestsUtil.JsonLightMimeType, request.ResponseHeaders["Override-Batch-Accept"]);

                    string response = request.GetResponseStreamAsText();
                    if (serviceType == typeof(ModifyHeaderOnStartProcessingRequestTestService))
                    {
                        Assert.IsTrue(response.Contains("Content-Type: application/json;odata.metadata=minimal;"));
                    }
                    else
                    {
                        // ProcessingRequest which sets the Aceept header is not called for inner requests
                        Assert.IsTrue(response.Contains("Content-Type: application/xml;charset=utf-8"));
                    }
                }
            }
        }

        // We don't fire Processing / Processed Request for a Batched query/operation, thus no tests trying those scenarios
        /// <summary>
        /// Verify that $format takes precidence over an user-set accept header values.
        /// </summary>
        [TestMethod, TestCategory("Partition1")]
        public void DollarFormatOverrideAcceptHeaderBeingSetInOnStartProcessingRequest()
        {
            StringBuilder batchQueryOperation = new StringBuilder();
            batchQueryOperation.AppendLine("GET Customers(1)/Address?$format=json HTTP/1.1");
            batchQueryOperation.AppendLine("Host: host");
            batchQueryOperation.AppendLine("Accept: " + UnitTestsUtil.MimeApplicationXml);
            batchQueryOperation.AppendLine("Override-Accept: something/var1");

            var test = new SimpleBatchTestCase
            {
                RequestPayload = new BatchInfo(new BatchQuery(new Operation(batchQueryOperation.ToString()))),
                ExpectedResponsePayloadContains = new[] { "Content-Type: application/json;odata.metadata=minimal" },
                ResponseStatusCode = 202,
                ResponseETag = default(string),
                ResponseVersion = V4,
                RequestDataServiceVersion = V4,
                RequestMaxDataServiceVersion = V4,
            };

            RunBatchTest(test, typeof(ModifyHeaderOnStartProcessingRequestTestService));
        }

        /// <summary>
        /// Verify that $format will fail when used in a batch uri
        /// </summary>
        [Ignore] // Remove Atom
        // [TestMethod, TestCategory("Partition1")]
        public void DollarFormatShouldFailOnTopLevelBatch()
        {
            StringBuilder batchQueryOperation = new StringBuilder();
            batchQueryOperation.AppendLine("GET Customers(1)/Address HTTP/1.1");
            batchQueryOperation.AppendLine("Host: host");

            var test = new SimpleBatchTestCase
            {
                RequestPayload = new BatchInfo(new BatchQuery(new Operation(batchQueryOperation.ToString()))), 
                ExpectedResponsePayloadContains = new[] {"batch"}, 
                ResponseStatusCode = 400,
                ResponseETag = default(string), 
                ResponseVersion = V4, 
                RequestDataServiceVersion = V4,
                RequestMaxDataServiceVersion = V4,
            };
            RunBatchTest(test, typeof(ModifyHeaderOnStartProcessingRequestTestService), "/$batch?$format=multipart/mixed", () => DataServicesResourceUtil.GetString("RequestQueryProcessor_FormatNotApplicable"));
        }

        private static void RunBatchTest(SimpleBatchTestCase testCase, Type serviceType)
        {
            RunBatchTest(testCase, serviceType, "/$batch", () => null);
        }

        /// <summary>
        /// Runs a batch test. For the purposes of exact baselining, this will strip off the guids from the boundary markers
        /// in the batch response. Also replaces the string BASE_URI with the service's base URI in any expected response text,
        /// so metadata in the response can be properly compared. Ignore the action-specific fields on the SimpleBatchTestCase.
        /// </summary>
        /// <param name="testCase">Description of the test.</param>
        /// <param name="serviceType">The DataServiceType to use.</param>
        private static void RunBatchTest(SimpleBatchTestCase testCase, Type serviceType, string requestUriString, Func<string> exceptionMessageFunc)
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.HttpMethod = "POST";
                request.RequestUriString = requestUriString;
                request.DataServiceType = serviceType;
                request.Accept = UnitTestsUtil.MimeMultipartMixed;
                request.RequestVersion = testCase.RequestDataServiceVersion.ToString();
                request.RequestMaxVersion = testCase.RequestMaxDataServiceVersion.ToString();
                request.ForceVerboseErrors = true;
                if (testCase.RequestPayload == null)
                {
                    request.RequestContentLength = 0;
                }
                else
                {
                    const string boundary = "batch-set";
                    request.RequestContentType = String.Format("{0}; boundary={1}", UnitTestsUtil.MimeMultipartMixed, boundary);
                    request.SetRequestStreamAsText(BatchRequestWritingUtils.GetBatchText(testCase.RequestPayload, boundary));
                }

                Exception e = TestUtil.RunCatching(request.SendRequest);
                string response = request.GetResponseStreamAsText();

                // Top level status code / response version / etag comparisons
                Assert.AreEqual(testCase.ResponseStatusCode, request.ResponseStatusCode);
                Assert.AreEqual(testCase.ResponseVersion.ToString(), request.ResponseVersion);
                Assert.AreEqual(testCase.ResponseETag, request.ResponseETag);

                // strip off the guid from the boundries and etag for comparison purposes
                Regex regex1 = new Regex(@"batchresponse_\w{8}-\w{4}-\w{4}-\w{4}-\w{12}");
                response = regex1.Replace(response, "batchresponse");
                Regex regex2 = new Regex(@"changesetresponse_\w{8}-\w{4}-\w{4}-\w{4}-\w{12}");
                response = regex2.Replace(response, "changesetresponse");
                Regex regex3 = new Regex(@"ETag: W/""\w{8}-\w{4}-\w{4}-\w{4}-\w{12}""");
                response = regex3.Replace(response, "ETag");

                var expectedMessage = exceptionMessageFunc();
                if (expectedMessage != null)
                {
                    Assert.IsNotNull(expectedMessage, "Expected an exception");
                    Assert.IsTrue(response.Contains(expectedMessage));
                }
                else
                {
                    // Make exact comparision if given that, or otherwise do a series of str contains
                    if (testCase.ExpectedResponsePayloadExact != null)
                    {
                        Assert.AreEqual(testCase.ExpectedResponsePayloadExact.Replace("BASE_URI", request.BaseUri + "/"), response);
                    }
                    else
                    {
                        foreach (string str in testCase.ExpectedResponsePayloadContains)
                        {
                            Assert.IsTrue(response.Contains(str.Replace("BASE_URI", request.BaseUri + "/")), String.Format("The response:\r\n{0}\r\nDoes not contain the string:\r\n{1}.", response, str));
                        }
                    }

                    Assert.IsNull(e, "No exception expected but received one.");
                }
            }
        }

        /// <summary>
        /// Runs a non-batch test over the the Services defined.
        /// </summary>
        /// <param name="configureRequest">Action to configure the request.</param>
        /// <param name="validateResponse">Action to validate the test results.</param>
        public static void RunTest(Action<TestWebRequest> configureRequest, Action<TestWebRequest> validateResponse)
        {
            foreach (var serviceType in Services)
            {
                using (TestWebRequest webRequest = TestWebRequest.CreateForInProcessWcf())
                {
                    // Default settings
                    webRequest.HttpMethod = "GET";
                    webRequest.DataServiceType = serviceType;

                    // Apply test's settings
                    configureRequest(webRequest);

                    TestUtil.RunCatching(webRequest.SendRequest);
                    validateResponse(webRequest);
                }
            }
        }

        /// <summary>
        /// Method used by test service to overwrite certain headers with custom header values, specified either as
        /// request headers or query string items.
        /// </summary>
        /// <param name="operationContext">OperationContext for the current operation.</param>
        public static void OverwriteHeaders(DataServiceOperationContext operationContext)
        {
            var overrideAcceptValue = operationContext.GetQueryStringValue("Override-Accept");
            overrideAcceptValue = overrideAcceptValue ?? operationContext.RequestHeaders.Get("Override-Accept"); 
            if (overrideAcceptValue != null)
            {
                operationContext.RequestHeaders.Set("Accept", overrideAcceptValue);
            }

            var overrideBatchAcceptValue = operationContext.GetQueryStringValue("Override-Batch-Accept");
            if (overrideBatchAcceptValue != null)
            {
                operationContext.RequestHeaders.Set("Accept", overrideBatchAcceptValue);

                // write a new header in the response so we can verify the request headers were overridden
                operationContext.ResponseHeaders.Set("Override-Batch-Accept", operationContext.RequestHeaders["Accept"]);
            }

            var customHeaderValue = operationContext.GetQueryStringValue("CustomHeader");
            customHeaderValue = customHeaderValue ?? operationContext.RequestHeaders.Get("CustomHeader");
            if (customHeaderValue != null)
            {
                operationContext.ResponseHeaders.Set("CustomHeader", customHeaderValue);
            }

            var overrideMaxDataServiceVersion = operationContext.GetQueryStringValue("Override-MaxDataServiceVersion");
            overrideMaxDataServiceVersion = overrideMaxDataServiceVersion ?? operationContext.RequestHeaders.Get("Override-MaxDataServiceVersion");
            if (overrideMaxDataServiceVersion != null)
            {
                operationContext.RequestHeaders.Set("OData-MaxVersion", overrideMaxDataServiceVersion);
            }

            var customPostTunneling = operationContext.GetQueryStringValue("Custom-Post-Tunneling");
            customPostTunneling = customPostTunneling ?? operationContext.RequestHeaders.Get("Custom-Post-Tunneling");
            if (customPostTunneling != null)
            {
                operationContext.RequestHeaders.Set("X-Http-Method", customPostTunneling);
            }

            var forceErrorValue = operationContext.GetQueryStringValue("Force-Error");
            forceErrorValue = forceErrorValue ?? operationContext.RequestHeaders.Get("Force-Error");
            if (forceErrorValue == "yes")
            {
                throw new DataServiceException(418, "User code threw a teapot exception.");
            }
            var overrideIfMatch = operationContext.GetQueryStringValue("Override-If-Match");
            if (overrideIfMatch != null)
            {
                operationContext.RequestHeaders.Set("If-Match", overrideIfMatch);
            }

            var overridePrefer = operationContext.GetQueryStringValue("Override-Prefer");
            if (overridePrefer != null)
            {
                operationContext.RequestHeaders.Set("Prefer", overridePrefer);
            }

            var overrideMyHeader = operationContext.GetQueryStringValue("Override-MyHeader");
            if (overrideMyHeader != null)
            {
                operationContext.RequestHeaders.Set("MyHeader", overrideMyHeader);

                // write a new header in the response so we can verify the request headers were overridden
                operationContext.ResponseHeaders.Set("MyHeader", overrideMyHeader);
            }

            var overrideDate = operationContext.GetQueryStringValue("Override-Date");
            if (overrideDate != null)
            {
                operationContext.RequestHeaders.Set("Date", overrideDate);
                operationContext.ResponseHeaders.Set("Date", overrideDate);

                // write a new header in the response so we can verify the request headers were overridden
                operationContext.ResponseHeaders.Set("MyDate", overrideDate);
            }

            // Override headers for edge tests
            var testCaseSensitive = operationContext.GetQueryStringValue("TestCaseSensitive");
            if (testCaseSensitive != null)
            {
                var lowercaseheader = operationContext.GetQueryStringValue("LowerCaseHeader");
                operationContext.ResponseHeaders.Set("lowercaseheader", lowercaseheader);
                var uppercaseheader = operationContext.GetQueryStringValue("uppercaseheader");
                operationContext.ResponseHeaders.Set("uppercaseheader", uppercaseheader);
            }

            var testNonExisting = operationContext.GetQueryStringValue("NonExistingHeader");
            if (testNonExisting != null)
            {
                throw new DataServiceException(418, "User code threw a testNonExisting exception.");
            }

            var testNullKey = operationContext.GetQueryStringValue("TestNullKey");
            if (testNullKey != null)
            {
                if (operationContext.GetQueryStringValue("") != null)
                {
                    throw new DataServiceException(418, "User code threw a testNullKey exception.");
                }
            }

            var testNullValue = operationContext.GetQueryStringValue("TestNullValue");
            if (testNullValue != null)
            {
                if (operationContext.GetQueryStringValue("MyHeader") != string.Empty)
                {
                    throw new DataServiceException(418, "User code threw a testNullValue exception.");
                }
            }

            var overrideContentType = operationContext.GetQueryStringValue("Override-Content-Type");
            if (overrideContentType != null)
            {
                operationContext.RequestHeaders.Set("Content-Type", overrideContentType);
            }

            var overrideContentLength = operationContext.GetQueryStringValue("Override-Content-Length");
            if (overrideContentLength != null)
            {
                operationContext.RequestHeaders.Set("Content-Length", overrideContentLength);
                operationContext.ResponseHeaders.Set("Request-Content-Length", overrideContentLength);
            }

            var testDuplicateHeader = operationContext.GetQueryStringValue("TestDuplicateHeader");
            if (testDuplicateHeader != null)
            {
                operationContext.GetQueryStringValue("MyHeader");
            }
        }
    }

    /// <summary>
    /// Test service that has custom headers which will override normal headers in OnStartProcessingRequest.
    /// </summary>
    public class ModifyHeaderOnStartProcessingRequestTestService : DataService<CustomDataContext>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
            config.UseVerboseErrors = true;
            config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
        }

        protected override void OnStartProcessingRequest(ProcessRequestArgs args)
        {
            base.OnStartProcessingRequest(args);
            Assert.IsNotNull(args.OperationContext.ResponseHeaders);

            ModifyHeadersScenarioTests.OverwriteHeaders(args.OperationContext);
        }

        [WebGet()]
        public IQueryable<Customer> GetCustomer(string param1, int param2)
        {
            return this.CurrentDataSource.Customers;
        }
    }

    /// <summary>
    /// Test service that has custom headers which will override normal headers in the ProcessingRequest event.
    /// </summary>
    public class ModifyHeadersInProcessingRequestEventTestService : DataService<CustomDataContext>
    {
        public ModifyHeadersInProcessingRequestEventTestService()
        {
            ProcessingPipeline.ProcessingRequest += ProcessingRequestListener;
        }

        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
            config.UseVerboseErrors = true;
            config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
        }

        protected static void ProcessingRequestListener(object caller, DataServiceProcessingPipelineEventArgs args)
        {
            ModifyHeadersScenarioTests.OverwriteHeaders(args.OperationContext);
        }

        [WebGet()]
        public IQueryable<Customer> GetCustomer(string param1, int param2)
        {
            return this.CurrentDataSource.Customers;
        }
    }

    /// <summary>
    /// Test Service that listens on ProcessedRequest and can override headers at that time.
    /// </summary>
    public class ModifyHeadersInProcessedRequestEventTestService : DataService<CustomDataContext>
    {
        public ModifyHeadersInProcessedRequestEventTestService()
        {
            ProcessingPipeline.ProcessedRequest += ProcessedRequestListener;
        }

        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
            config.UseVerboseErrors = true;
            config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
        }

        protected static void ProcessedRequestListener(object caller, DataServiceProcessingPipelineEventArgs args)
        {
            ModifyHeadersScenarioTests.OverwriteHeaders(args.OperationContext);
        }
    }
}