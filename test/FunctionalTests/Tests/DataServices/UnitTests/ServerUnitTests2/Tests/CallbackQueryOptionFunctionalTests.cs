//---------------------------------------------------------------------
// <copyright file="CallbackQueryOptionFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using System.IO;
    using System.Net;
    using System.ServiceModel.Web;
    using System.Text;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Tests.Server;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/875
    [TestClass]
    public class CallbackQueryOptionFunctionalTests
    {
        /// <summary>
        /// We should do JSONP if content negotiation decides to write JSON  of any kind. 
        /// </summary>
        [TestCategory("Partition2"), TestMethod]
        public void CallbackAndFormatSuccessSmokeTest()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                Dictionary<string, string> dictionary = new Dictionary<string,string>()
                {
                    {"json", "text/javascript;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8"},
                };

                foreach (var item in dictionary)
                {
                    request.RequestUriString = "/Customers(1)?$format=" + item.Key + "&$callback=foo";

                    request.SendRequest();
                    var actualText = request.GetResponseStreamAsText();
                    Assert.IsTrue(actualText.StartsWith("foo("));
                    Assert.IsTrue(actualText.EndsWith(")"));
                    Assert.AreEqual(item.Value, request.ResponseHeaders["content-type"]);
                    Assert.AreEqual(200, request.ResponseStatusCode);
                }
            }
        }

        /// <summary>
        /// We should do JSONP for various JSON query payloads. The test covers various query keywords and response payload types.
        /// </summary>
        [TestCategory("Partition2"), TestMethod]
        public void CallbackSuccessQueryTest()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                List<string> requestUris = new List<string>()
                {
                    "/Customers(1)/ID?$format=json&$callback=foo",
                    "/Customers(1)/Orders/$ref?$format=json&$callback=foo",
                    "/Customers/?$expand=Orders&$select=ID,Name,Orders&$top=3&$skip=1&$orderby=ID&$filter=Orders/any(p:p/ID%20ne%200)&$format=json&$callback=foo",
                    "/Customers/?$format=json&$callback=foo&$filter=Orders/all(p:p/ID%20ge%200)&inlinecount=allpages",
                };

                foreach (var requestUri in requestUris)
                {
                    request.RequestUriString = requestUri;

                    request.SendRequest();
                    var actualText = request.GetResponseStreamAsText();
                    Assert.IsTrue(actualText.StartsWith("foo("));
                    Assert.IsTrue(actualText.EndsWith(")"));
                    Assert.IsTrue(request.ResponseHeaders["content-type"].StartsWith("text/javascript;odata.metadata=minimal;"));
                    Assert.AreEqual(200, request.ResponseStatusCode);
                }
            }
        }

        /// <summary>
        /// We should do JSONP regardless of MDSV and DSV. 
        /// </summary>
        [TestCategory("Partition2"), TestMethod]
        public void CallbackAndFormatSuccessVersionTest()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                List<string> versions = new List<string>() {"4.0;" };

                foreach (var version in versions)
                {
                    request.RequestUriString = "/Customers(1)?$format=json&$callback=foo";
                    request.RequestVersion = version;
                    request.RequestMaxVersion = version;
                    request.SendRequest();
                    var actualText = request.GetResponseStreamAsText();
                    Assert.IsTrue(actualText.StartsWith("foo("));
                    Assert.IsTrue(actualText.EndsWith(")"));
                    Assert.AreEqual("text/javascript;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8", request.ResponseHeaders["content-type"]);
                    Assert.AreEqual(200, request.ResponseStatusCode);
                }
            }
        }

        /// <summary>
        /// Test callback option for empty, $value, $count queries.
        /// </summary>
        [TestCategory("Partition2"), TestMethod]
        public void CallbackSuccessQueryKeywordTest()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CallbackQueryOptionTestService);
                request.StartService();
                
                request.RequestUriString = "/ReturnNullServiceOperation?$callback=foo";
                request.SendRequest();
                Assert.AreEqual(204, request.ResponseStatusCode);

                request.RequestUriString = "/Customers(1)/ID/$value?$callback=foo";
                request.SendRequest();
                Assert.AreEqual(200, request.ResponseStatusCode);
                Assert.AreEqual("foo(1)", request.GetResponseStreamAsText());
                Assert.AreEqual("text/javascript;charset=utf-8", request.ResponseHeaders["content-type"]);

                request.RequestUriString = "/Customers/$count?$callback=foo";
                request.SendRequest();
                Assert.AreEqual(200, request.ResponseStatusCode);
                var actualText = request.GetResponseStreamAsText();
                Assert.IsTrue(actualText.StartsWith("foo("));
                Assert.IsTrue(actualText.EndsWith(")"));
                Assert.AreEqual("text/javascript;charset=utf-8", request.ResponseHeaders["content-type"]);
            }
        }

        /// <summary>
        /// Verify that we support callback option for inner GET request in Batch but not the batch request itself.
        /// </summary>
        [Ignore] // Remove Atom
        // [TestMethod, TestCategory("Partition1")]
        public void CallBackInBatchRequestTest()
        {
            StringBuilder batchQueryOperation = new StringBuilder(); 
            batchQueryOperation.AppendLine("GET Customers(1)/Address?$callback=foo HTTP/1.1");
            batchQueryOperation.AppendLine("Host: host");
            batchQueryOperation.AppendLine("Accept: " + UnitTestsUtil.JsonMimeType);
            batchQueryOperation.AppendLine("Override-Accept: " + UnitTestsUtil.JsonLightMimeType);

            var testCase = new SimpleBatchTestCase
            {
                RequestPayload = new BatchInfo(new BatchQuery(new Operation(batchQueryOperation.ToString()))),
                ExpectedResponsePayloadContains = new[] 
                { 
                    "Content-Type: text/javascript;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8", 
                    "foo({\"@odata.context\":",
                    "\"StreetAddress\":\"Line1\",\"City\":\"Redmond\",\"State\":\"WA\",\"PostalCode\":\"98052\"})",
                },
                ResponseStatusCode = 202,
                ResponseETag = default(string),
            };

            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.HttpMethod = "POST";
                request.RequestUriString = "/$batch";
                request.DataServiceType = typeof(CustomDataContext);
                request.Accept = UnitTestsUtil.MimeMultipartMixed;
                request.ForceVerboseErrors = true;

                const string boundary = "batch-set";
                request.RequestContentType = String.Format("{0}; boundary={1}", UnitTestsUtil.MimeMultipartMixed, boundary);
                request.SetRequestStreamAsText(BatchRequestWritingUtils.GetBatchText(testCase.RequestPayload, boundary));

                // callback in inner GET request should succeed
                request.SendRequest();
                string response = request.GetResponseStreamAsText();
                Assert.AreEqual(testCase.ResponseStatusCode, request.ResponseStatusCode);
                Assert.IsTrue(request.ResponseContentType.StartsWith("multipart/mixed; boundary=batchresponse_"));
                foreach (string str in testCase.ExpectedResponsePayloadContains)
                {
                    Assert.IsTrue(response.Contains(str), String.Format("The response:\r\n{0}\r\nDoes not contain the string:\r\n{1}.", response, str));
                }

                // callback with $batch should fail
                try
                {
                    request.RequestUriString = "/$batch?$callback=bar";
                    request.SendRequest();
                    Assert.Fail("Request should have failed because it was not a GET request.");
                }
                catch (WebException)
                {
                    Assert.IsTrue(request.GetResponseStreamAsText().Contains("$callback can only be specified on GET requests."));
                    Assert.IsTrue(request.ResponseHeaders["content-type"].StartsWith("application/xml"));
                    Assert.AreEqual(400, request.ResponseStatusCode);
                }
            }
        }

        /// <summary>
        /// We throw on $callback if we content negotiation does not result in us writing JSON.
        /// </summary>
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod]
        public void CallbackFailOnAtomXml()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                List<string> requestUriStrings = new List<string>()
                {
                    "/Customers(1)?$callback=foo",
                    "/Customers(1)/Address?$callback=foo",
                };

                foreach (string requestUriString in requestUriStrings)
                {
                    try
                    {
                        request.RequestUriString = requestUriString;
                        request.Accept = "application/atom+xml,application/xml";
                        request.SendRequest();
                        Assert.Fail("Request should have failed because our server defaults to ATOM/XML, which does not support $callback.");
                    }
                    catch (WebException)
                    {
                        var actualText = request.GetResponseStreamAsText();
                        Assert.IsFalse(actualText.StartsWith("foo("));
                        Assert.IsTrue(actualText.Contains("is not compatible with the $callback query option."));

                        Assert.IsTrue(request.ResponseHeaders["content-type"].StartsWith("application/xml"));
                        Assert.AreEqual(400, request.ResponseStatusCode);
                    }
                }
            }
        }

        /// <summary>
        /// We throw on $callback if not it's not a GET request.
        /// </summary>
        [TestCategory("Partition2"), TestMethod]
        public void CallbackFailOnCUDRequest()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.RequestUriString = "/Customers(1)?$format=json&$callback=foo";
                
                List<string> methods = new List<string>() { "POST", "PUT", "DELETE" };
                request.RequestStream = new MemoryStream(new byte[] { 1, 2, 3, });

                foreach (string method in methods)
                {
                    try
                    {
                        request.HttpMethod = method;
                        request.SendRequest();
                        Assert.Fail("Request should have failed because it was not a GET request.");
                    }
                    catch (WebException)
                    {
                        var actualText = request.GetResponseStreamAsText();
                        Assert.IsFalse(actualText.StartsWith("foo("));
                        Assert.IsTrue(actualText.Contains("$callback can only be specified on GET requests."));

                        Assert.IsTrue(request.ResponseHeaders["content-type"].StartsWith("application/json"));
                        Assert.AreEqual(400, request.ResponseStatusCode);
                    }
                }
            }
        }

        /// <summary>
        /// We throw on $callback if it's $metadata request.
        /// </summary>
        [TestCategory("Partition2"), TestMethod]
        public void CallbackFailOnMetadataRequest()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.RequestUriString = "/$metadata?$format=json&$callback=foo";
                
                try
                {
                    request.SendRequest();
                    Assert.Fail("Meatadata Request should have failed with $callback.");
                }
                catch (WebException)
                {
                    var actualText = request.GetResponseStreamAsText();
                    Assert.IsFalse(actualText.StartsWith("foo("));
                    Assert.IsTrue(actualText.Contains("The requested media type 'Metadata' is not compatible with the $callback query option."));

                    Assert.IsTrue(request.ResponseHeaders["content-type"].StartsWith("application/json"));
                    Assert.AreEqual(400, request.ResponseStatusCode);
                }
            }
        }

        /// <summary>
        /// We should not do JSONP if there is an error. They won't be able to read it through a HTML script tag anyway.
        /// </summary>
        [TestCategory("Partition2"), TestMethod]
        public void IgnoreCallbackOnErrors()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.RequestUriString = "/MissingEntitySet(1)?$format=json&$callback=foo";
                request.HttpMethod = "GET";

                try
                {
                    request.SendRequest();
                    Assert.Fail("Request should have failed because of the missign Entity Set.");
                }
                catch (WebException)
                {
                    var actualText = request.GetResponseStreamAsText();
                    Assert.IsFalse(actualText.StartsWith("foo("));
                    Assert.IsTrue(actualText.Contains("MissingEntitySet"));

                    Assert.IsTrue(request.ResponseHeaders["content-type"].StartsWith("application/json"));
                    Assert.AreEqual(404, request.ResponseStatusCode);
                }
            }
        }

        /// <summary>
        /// Test different callback string values.
        /// </summary>
        [TestCategory("Partition2"), TestMethod]
        public void CallbackOptionValueTest()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                Dictionary<string, string> testCases = new Dictionary<string, string>()
                {
                    // empty callback option
                    { "?$format=json&$callback=", string.Empty },

                    // example callback option values from JQuery and DataJS
                    { "?$callback=parent.handleJSONP_0&$format=json", "parent.handleJSONP_0" },
                    { "?$callback=jQuery18209805240577502099_1348783118115&$format=json&_=1348783118119", "jQuery18209805240577502099_1348783118115" },

                    // callback option values in special character/format
                    { "?$format=json&$callback=null", "null" },
                    { "?$callback=" + Uri.EscapeDataString("A string with characters :%*+,/.") + "&$format=json", "A string with characters :%*+,/." },
                    { "?$callback=" + "<script>$.getJSON(\"http://something.com\",function (data) {alert(data.value);});</script>" + "&$format=json", "<script>$.getJSON(\"http://something.com\",function (data) {alert(data.value);});</script>" },
                };

                foreach (var testCase in testCases)
                {
                    request.RequestUriString = "/Customers(1)" + testCase.Key;

                    request.SendRequest();
                    var actualText = request.GetResponseStreamAsText();
                    if (testCase.Value.Length == 0)
                    {
                        // We don't do JSONP for empty callback value
                        Assert.IsTrue(actualText.StartsWith(testCase.Value + "{"));
                        Assert.IsTrue(actualText.EndsWith("}"));
                        Assert.IsTrue(request.ResponseHeaders["content-type"].StartsWith("application/json;odata.metadata=minimal;"));
                    }
                    else
                    {
                        Assert.IsTrue(actualText.StartsWith(testCase.Value + "({"));
                        Assert.IsTrue(actualText.EndsWith("})"));
                        Assert.IsTrue(request.ResponseHeaders["content-type"].StartsWith("text/javascript;odata.metadata=minimal;"));
                    }

                    Assert.AreEqual(200, request.ResponseStatusCode);
                }
            }
        }

        /// <summary>
        /// Test service to be used in $callback tests.
        /// </summary>
        public class CallbackQueryOptionTestService : DataService<CustomDataContext>
        {
            public static void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
                config.UseVerboseErrors = true;
                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            }

            [WebGet]
            public void ReturnNullServiceOperation()
            {
                return;
            }
        }
    }
}
