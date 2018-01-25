//---------------------------------------------------------------------
// <copyright file="BatchTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using AstoriaUnitTests.Stubs;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces

    [TestModule]
    public partial class UnitTestModule : AstoriaTestModule
    {
        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/875
        /// <summary>This is a test class for etags test</summary>
        [TestClass, TestCase]
        public class BatchTests
        {
            /// <summary>Tests whether the configuration options are honored.</summary>
            [Ignore] // Remove Atom
            // [TestCategory("Partition2"), TestMethod, Variation]
            public void ConfigurationBatchTest()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("MaxBatchCount", new int[] { -1, 0, 1, 2, 10 }),
                    new Dimension("MaxChangeSetCount", new int[] { -1, 0, 1, 2, 10 }),
                    new Dimension("BatchCount", new int[] { 0, 1, 2 }),
                    new Dimension("ChangeSetCount", new int[] { 0, 1, 2 })
                    );
                TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                {
                    int maxBatchCount = (int)values["MaxBatchCount"];
                    int maxChangeSetCount = (int)values["MaxChangeSetCount"];
                    int batchCount = (int)values["BatchCount"];
                    int changeSetCount = (int)values["ChangeSetCount"];
                    TestUtil.ClearConfiguration();
                    using (CustomDataContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    using (StaticCallbackManager<InitializeServiceArgs>.RegisterStatic((sender, args) =>
                        {
                            args.Config.SetEntitySetAccessRule("*", EntitySetRights.All);
                            args.Config.MaxBatchCount = maxBatchCount;
                            args.Config.MaxChangesetCount = maxChangeSetCount;
                            args.Config.UseVerboseErrors = true;
                        }))
                    {
                        request.ServiceType = typeof(TypedDataService<CustomDataContext>);
                        request.RequestUriString = "/$batch";
                        request.HttpMethod = "POST";
                        request.Accept = "*/*";
                        string boundary = "boundary";
                        request.RequestContentType = String.Format("{0}; boundary={1}", UnitTestsUtil.MimeMultipartMixed, boundary);

                        int customerId = 1000;
                        int contentId = 0;
                        StringBuilder payload = new StringBuilder();
                        for (int i = 0; i < batchCount; i++)
                        {
                            StringBuilder batchElement = new StringBuilder();
                            if (i % 2 == 0)
                            {
                                string changesetBoundary = "cs";
                                for (int j = 0; j < changeSetCount; j++)
                                {
                                    StringBuilder changeSetElement = new StringBuilder();
                                    changeSetElement.AppendLine("<entry " + TestUtil.CommonPayloadNamespaces + ">");
                                    changeSetElement.AppendLine(AtomUpdatePayloadBuilder.GetCategoryXml("AstoriaUnitTests.Stubs.Customer"));
                                    changeSetElement.AppendLine(" <content type='application/xml'><adsm:properties>");
                                    changeSetElement.AppendLine("  <ads:Name>A New Customer</ads:Name>");
                                    changeSetElement.AppendLine("  <ads:ID>" + customerId++ + "</ads:ID>");
                                    changeSetElement.AppendLine(" </adsm:properties></content></entry>");

                                    int length = changeSetElement.Length;
                                    changeSetElement.Insert(0,
                                        "--" + changesetBoundary + "\r\n" +
                                        "Content-Type: application/http\r\n" +
                                        "Content-Transfer-Encoding: binary\r\n" +
                                        "Content-ID: " + (++contentId).ToString() + "\r\n" +
                                        "\r\n" +
                                        "POST /Customers HTTP/1.1\r\n" +
                                        "Content-Type: application/atom+xml;type=entry\r\n" +
                                        "Content-Length: " + length + "\r\n" +
                                        "\r\n");
                                    batchElement.Append(changeSetElement.ToString());
                                }

                                batchElement.AppendLine("--" + changesetBoundary + "--");
                                int batchLength = batchElement.Length;
                                batchElement.Insert(0,
                                    "--" + boundary + "\r\n" +
                                    "Content-Type: multipart/mixed; boundary=" + changesetBoundary + "\r\n" +
                                    "Content-Length: " + batchLength + "\r\n" +
                                    "\r\n");
                            }
                            else
                            {
                                // Do a GET request.
                                batchElement.AppendLine("--" + boundary);
                                batchElement.AppendLine("Content-Type: application/http");
                                batchElement.AppendLine("Content-Transfer-Encoding: binary");
                                batchElement.AppendLine();
                                batchElement.AppendLine("GET /Customers HTTP/1.1");
                                batchElement.AppendLine("Content-Length: 0");
                                batchElement.AppendLine();
                            }

                            payload.Append(batchElement.ToString());
                        }

                        payload.AppendLine("--" + boundary + "--");

                        string payloadText = payload.ToString();
                        Trace.WriteLine("Payload text:");
                        Trace.WriteLine(payloadText);
                        request.SetRequestStreamAsText(payloadText);

                        // Build a payload.
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception,
                            maxBatchCount < 0,
                            maxChangeSetCount < 0);
                        if (exception == null)
                        {
                            string text = request.GetResponseStreamAsText();
                            if (maxBatchCount < batchCount ||
                                (batchCount > 0 && maxChangeSetCount < changeSetCount))
                            {
                                TestUtil.AssertContains(text, "error");
                            }
                            else
                            {
                                TestUtil.AssertContainsFalse(text, "error");
                            }
                        }
                    }
                });
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition2"), TestMethod, Variation]
            public void TestExceedMaxBatchCount()
            {
                // Astoria Server: Assert on Batch Request after configuring MaxBatchCount
                TestUtil.ClearConfiguration();
                try
                {
                    using (CustomDataContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    using (StaticCallbackManager<InitializeServiceArgs>.RegisterStatic((sender, args) =>
                        {
                            args.Config.SetEntitySetAccessRule("*", EntitySetRights.All);
                            args.Config.MaxBatchCount = 1;
                            args.Config.UseVerboseErrors = true;
                        }))
                    {
                        request.ServiceType = typeof(TypedDataService<CustomDataContext>);
                        request.HttpMethod = "POST";
                        request.RequestUriString = "/$batch";
                        request.RequestContentType = "multipart/mixed;boundary=boundary1";
                        request.SetRequestStreamAsText(
                            "--boundary1\r\n" +
                            "Content-Type: multipart/mixed; boundary=cs\r\n" +
                            "\r\n" +
                            "--cs\r\n" +
                            "Content-Type: application/http\r\n" +
                            "Content-Transfer-Encoding: binary\r\n" +
                            "\r\n" +
                            "POST /Customers HTTP/1.1\r\n" +
                            "Content-Type: application/atom+xml\r\n" +
                            "Accept: application/atom+xml\r\n" +
                            "Content-ID : 2\r\n" +
                            "\r\n" +
                            "<entry xmlns:ads='http://docs.oasis-open.org/odata/ns/data' xmlns:adsm='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom' adsm:type='#AstoriaUnitTests.Stubs.Customer'>\r\n" +
                            "<content type='application/xml'>\r\n" +
                              "<adsm:properties>\r\n" +
                                "<ads:ID ads:type='Edm.Int32'>10</ads:ID>\r\n" +
                              "</adsm:properties>\r\n" +
                            "</content></entry>\r\n" +
                            "--cs--\r\n" +
                            "--boundary1\r\n" +
                            "Content-Type: multipart/mixed; boundary=cs\r\n" +
                            "\r\n" +
                            "--cs\r\n" +
                            "Content-Type: application/http\r\n" +
                            "Content-Transfer-Encoding: binary\r\n" +
                            "\r\n" +
                            "POST /Customers HTTP/1.1\r\n" +
                            "Content-Type: application/atom+xml\r\n" +
                            "Accept: application/atom+xml\r\n" +
                            "Content-ID : 2\r\n" +
                            "\r\n" +
                            "<entry xmlns:ads='http://docs.oasis-open.org/odata/ns/data' xmlns:adsm='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom' adsm:type='#AstoriaUnitTests.Stubs.Customer'>\r\n" +
                            "<content type='application/xml'>\r\n" +
                              "<adsm:properties>\r\n" +
                                "<ads:ID ads:type='Edm.Int32'>10</ads:ID>\r\n" +
                              "</adsm:properties>\r\n" +
                            "</content></entry>\r\n" +
                            "--cs--\r\n" +
                            "--boundary1--");
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception, false);
                        string response = request.GetResponseStreamAsText();
                        TestUtil.AssertContains(response, "exceeds");
                    }
                }
                finally
                {
                    TestUtil.ClearMetadataCache();
                }
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void AllowModificationWithOutChangeset()
            {
                string batchRequestsDirectory = Path.Combine(TestUtil.ServerUnitTestSamples, @"tests\BatchRequests");
                using (OpenWebDataServiceHelper.EntitySetAccessRule.Restore())
                using (OpenWebDataServiceHelper.ServiceOperationAccessRule.Restore())
                {
                    CustomRowBasedContext.PreserveChanges = false;
                    TestUtil.RunCombinations(
                        new string[] { "4.0" },
                        (dataServiceVersion) =>
                        {
                            Type contextType = typeof(CustomRowBasedContext);
                            string requestContent = File.ReadAllText(Path.Combine(batchRequestsDirectory, "ModificationOutsideChangesetbatch" + ".txt"));

                            string responseFileName = "ModificationOutsideChangesetresponse.txt";

                            responseFileName = Path.Combine(batchRequestsDirectory, responseFileName);
                            using (ChangeScope.GetChangeScope(contextType))
                            {
                                string actualResponse = BatchTestUtil.GetResponse(requestContent, contextType, WebServerLocation.InProcess, dataServiceVersion);
                                BatchTestUtil.CompareBatchResponse(responseFileName, actualResponse);
                            }
                        });
                }
            }

            [Ignore] // Remove Atom
            // [TestCategory("Partition2"), TestMethod, Variation]
            public void UriCompositionRulesChangedOnServer()
            {
                string batchRequestsDirectory = Path.Combine(TestUtil.ServerUnitTestSamples, @"tests\BatchRequests");
                using (OpenWebDataServiceHelper.EntitySetAccessRule.Restore())
                using (OpenWebDataServiceHelper.ServiceOperationAccessRule.Restore())
                {
                    CustomRowBasedContext.PreserveChanges = false;
                    TestUtil.RunCombinations(
                        new string[] {"4.0" },
                        UnitTestsUtil.BooleanValues,
                        (dataServiceVersion, insertForwardSlashCharacter) =>
                        {
                            Type contextType = typeof(CustomRowBasedContext);
                            string requestContent = File.ReadAllText(Path.Combine(batchRequestsDirectory, "UriCompositionReproRequest.txt"));
                            if (insertForwardSlashCharacter)
                            {
                                requestContent = requestContent.Replace("{InsertForwardSlash}", "/");
                            }
                            else
                            {
                                requestContent = requestContent.Replace("{InsertForwardSlash}", string.Empty);
                            }

                            string responseFileName = "UriCompositionSucessResponse.txt";
                            if (insertForwardSlashCharacter)
                            {
                                // if the DSV is set to v3 and the post and bind uris start with a forward slash, 
                                // the absolute uris produced by the RequestUriProcessor will be based at a different location.
                                responseFileName = "UriCompositionErrorResponse.txt";
                            }

                            responseFileName = Path.Combine(batchRequestsDirectory, responseFileName);
                            using (ChangeScope.GetChangeScope(contextType))
                            {
                                string actualResponse = BatchTestUtil.GetResponse(requestContent, contextType, WebServerLocation.InProcess, dataServiceVersion);
                                BatchTestUtil.CompareBatchResponse(responseFileName, actualResponse);
                            }
                        });
                }
            }

            private sealed class BatchContentTypeTestCase
            {
                public string ContentType { get; set; }
                public string PayloadBatchBoundary { get; set; }
                public int ExpectedErrorStatusCode { get; set; }
                public string ExpectedClientErrorMessage { get; set; }
                public override string ToString()
                {
                    return this.ContentType;
                }
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition2"), TestMethod, Variation]
            public void BatchContentTypeTest()
            {
                var testCases = new BatchContentTypeTestCase[]
                {
                    // Completely wrong content type
                    new BatchContentTypeTestCase
                    {
                        ContentType = "text/plain",
                        ExpectedErrorStatusCode = 400,
                        ExpectedClientErrorMessage = DataServicesClientResourceUtil.GetString("Batch_ExpectedContentType", "text/plain")
                    },
                    // Just type is correct, subtype is wrong
                    new BatchContentTypeTestCase
                    {
                        ContentType = "multipart/text",
                        ExpectedErrorStatusCode = 400,
                        ExpectedClientErrorMessage = DataServicesClientResourceUtil.GetString("Batch_ExpectedContentType", "multipart/text")
                    },
                    // No boundary - still wrong
                    new BatchContentTypeTestCase
                    {
                        ContentType = "multipart/mixed",
                        ExpectedErrorStatusCode = 400,
                        ExpectedClientErrorMessage = ODataLibResourceUtil.GetString("MediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads", "multipart/mixed", "boundary")
                    },
                    // Some other parameter but no boundary
                    new BatchContentTypeTestCase
                    {
                        ContentType = "multipart/mixed;param=value",
                        ExpectedErrorStatusCode = 400,
                        ExpectedClientErrorMessage = ODataLibResourceUtil.GetString("MediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads", "multipart/mixed;param=value", "boundary")
                    },
                    // Empty boundary - fails
                    new BatchContentTypeTestCase
                    {
                        ContentType = "multipart/mixed;boundary=",
                        ExpectedErrorStatusCode = 400,
                        ExpectedClientErrorMessage = ODataLibResourceUtil.GetString("ValidationUtils_InvalidBatchBoundaryDelimiterLength", string.Empty, "70")
                    },
                    new BatchContentTypeTestCase
                    {
                        ContentType = "multipart/mixed;boundary=;param=value",
                        ExpectedErrorStatusCode = 400,
                        ExpectedClientErrorMessage = ODataLibResourceUtil.GetString("ValidationUtils_InvalidBatchBoundaryDelimiterLength", string.Empty, "70")
                    },
                    // Two boundary parameters - wrong
                    new BatchContentTypeTestCase
                    {
                        ContentType = "multipart/mixed;boundary=one;boundary=two",
                        ExpectedErrorStatusCode = 400,
                        ExpectedClientErrorMessage = ODataLibResourceUtil.GetString("MediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads", "multipart/mixed;boundary=one;boundary=two", "boundary")
                    },
                    // Valid simple boundary
                    new BatchContentTypeTestCase
                    {
                        ContentType = "multipart/mixed;boundary=batchboundary",
                        PayloadBatchBoundary = "batchboundary"
                    },
                    // Valid simple boundary - mimetype using different casing
                    new BatchContentTypeTestCase
                    {
                        ContentType = "MultiPart/mIxed;boundary=batchboundary",
                        PayloadBatchBoundary = "batchboundary"
                    },
                    // Valid simple boundary - boundary parameter name different casing
                    new BatchContentTypeTestCase
                    {
                        ContentType = "multipart/mixed;BounDary=batchboundary",
                        PayloadBatchBoundary = "batchboundary"
                    },
                };

                OpenWebDataServiceDefinition serverService = new OpenWebDataServiceDefinition()
                {
                    DataServiceType = typeof(CustomDataContext)
                };

                PlaybackServiceDefinition clientService = new PlaybackServiceDefinition();

                TestUtil.RunCombinations(
                    testCases,
                    (testCase) =>
                    {
                        using (TestWebRequest request = serverService.CreateForInProcess())
                        {
                            request.RequestContentType = testCase.ContentType;
                            request.SetRequestStreamAsText(string.Format(
                                "--{0}\r\n" +
                                "Content-Type: multipart/mixed; boundary=changesetresponse_00000001-0000-0000-0000-000000000000\r\n\r\n" +
                                "--changesetresponse_00000001-0000-0000-0000-000000000000\r\n" +
                                "--changesetresponse_00000001-0000-0000-0000-000000000000--\r\n" +
                                "--{0}--\r\n", testCase.PayloadBatchBoundary));
                            request.RequestUriString = "/$batch";
                            request.HttpMethod = "POST";

                            Exception exception = TestUtil.RunCatching(request.SendRequest);

                            int actualStatusCode = 0;
                            if (exception != null)
                            {
                                actualStatusCode = request.ResponseStatusCode;
                            }
                            else
                            {
                                Assert.AreEqual(202, request.ResponseStatusCode, "Wrong response code for no-exception request.");
                                BatchWebRequest batchResponse = BatchWebRequest.FromResponse(InMemoryWebRequest.FromResponse(request));
                                if (batchResponse.Parts.Count > 0)
                                {
                                    actualStatusCode = batchResponse.Parts[0].ResponseStatusCode;
                                    if (actualStatusCode == 200) actualStatusCode = 0;
                                }
                            }

                            Assert.AreEqual(testCase.ExpectedErrorStatusCode, actualStatusCode, "Wrong status code.");
                        }

                        using (TestWebRequest request = clientService.CreateForInProcessWcf())
                        {
                            request.StartService();

                            clientService.ProcessRequestOverride = clientRequest =>
                            {
                                var clientResponse = new InMemoryWebRequest();
                                clientResponse.SetResponseStatusCode(202);
                                clientResponse.ResponseHeaders["Content-Type"] = testCase.ContentType;
                                clientResponse.SetResponseStreamAsText(string.Format(
                                    "--{0}\r\n" +
                                    "Content-Type: application/http\r\n" +
                                    "Content-Transfer-Encoding: binary\r\n" +
                                    "\r\n" +
                                    "200 OK\r\n" +
                                    "<feed xmlns='http://www.w3.org/2005/Atom'/>\r\n" +
                                    "--{0}--\r\n",
                                    testCase.PayloadBatchBoundary));
                                return clientResponse;
                            };

                            DataServiceContext ctx = new DataServiceContext(request.ServiceRoot);
                            Exception exception = TestUtil.RunCatching(() => ctx.ExecuteBatch(ctx.CreateQuery<Customer>("/Customers")));

                            if (exception != null)
                            {
                                exception = ((DataServiceRequestException)exception).InnerException;
                                Assert.AreEqual(testCase.ExpectedClientErrorMessage, exception.Message, "Unexpected error message.");
                            }
                            else
                            {
                                Assert.IsNull(testCase.ExpectedClientErrorMessage, "Expected exception, but none was thrown.");
                            }
                        }
                    });
            }

            [TestCategory("Partition2"), TestMethod, Variation("Make sure you can send more than 100 request (default ODL limits) in batch")]
            public void SendMoreThan100RequestsInBatch()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    BatchWebRequest batchRequest = new BatchWebRequest();

                    for (int i = 0; i < 101; i++)
                    {
                        InMemoryWebRequest getRequest = new InMemoryWebRequest();
                        getRequest.RequestUriString = "Customers(1)";
                        batchRequest.Parts.Add(getRequest);
                    }

                    batchRequest.SendRequest(request);
                    Assert.IsFalse(batchRequest.Parts.Any(p => p.ResponseStatusCode != 200), "All the requests should succeed");
                }
            }
        }
    }
}
