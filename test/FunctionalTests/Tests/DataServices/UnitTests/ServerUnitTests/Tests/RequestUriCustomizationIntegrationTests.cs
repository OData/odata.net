//---------------------------------------------------------------------
// <copyright file="RequestUriCustomizationIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Linq;
    using System.ServiceModel;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RequestUriCustomizationIntegratiotnTests
    {
        [Ignore] // Remove Atom
        [TestCategory("Partition2")]
        [TestMethod]
        public void GetSmokeTestWhereRequestAndServiceUriAreCompletelyReplaced()
        {
            Run(request =>
            {
                request.RequestUriString = "/WhoCaresWhatIPutHere/Something/SomethingElse/TotallyIgnored";
                request.RequestHeaders["MyCustomRequestUri"] = "http://myservicebase/Customers(0)";
                request.RequestHeaders["MyCustomServiceUri"] = "http://myservicebase/";
                request.Accept = "application/atom+xml,application/xml";

                GetResponseAndVerify(request, 200, "atom:entry[@xml:base='http://myservicebase/']/atom:link[@rel='edit' and @href='Customers(0)']");
            });
        }

        [Ignore] // Remove Atom
        [TestCategory("Partition2")]
        [TestMethod]
        public void GetSmokeTestWhereRequestUriPathIsReversed()
        {
            Run(request =>
            {
                ((InProcessWebRequest)request).CustomServiceBaseUri = "http://host/" + Reverse("path1/path2");
                request.RequestUriString = "/" + Reverse("Customers(0)");
                request.RequestHeaders["ReverseUriPaths"] = "true";
                request.Accept = "application/atom+xml,application/xml";
                GetResponseAndVerify(request, 200, "atom:entry[@xml:base='http://host/path1/path2/']/atom:link[@rel='edit' and @href='Customers(0)']");
            });
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void ChangingQueryOptionErrorTest()
        {
            // Regression coverage for: Query Options ignored when modifying request URL.
            Run(request =>
            {
                request.RequestUriString = "/Orders(0)";
                request.RequestHeaders["MyCustomRequestUri"] = "http://myservicebase/Customers(0)?$top=1";
                request.RequestHeaders["MyCustomServiceUri"] = "http://myservicebase/";
                TestUtil.RunCatching(request.SendRequest);

                Assert.AreEqual(500, request.ResponseStatusCode);

                var responsePayload = request.GetResponseStreamAsText();
                Assert.IsTrue(responsePayload.Contains("The query string of the request URI cannot be modified in the OnStartProcessingRequest method."));
            });
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void BatchSmokeTestWhereRequestAndServiceUriAreCompletelyReplaced()
        {
            Run(request =>
                {
                    Action<TestWebRequest> configureBatchRequest = r =>
                                                   {
                                                       r.RequestUriString = "NotBatchAtAll";
                                                       r.RequestHeaders["MyCustomRequestUri"] = "http://myservicebase/$batch";
                                                       r.RequestHeaders["MyCustomServiceUri"] = "http://myservicebase/";
                                                   };
                    Action<TestWebRequest> configureInnerRequest = r =>
                                                   {
                                                       r.RequestUriString = "SomethingThatWillNotBeUsedAtAll";
                                                       r.RequestHeaders["MyCustomRequestUri"] = "http://myservicebase/Customers(0)";
                                                   };
                    RunBatchTest(request, configureBatchRequest, configureInnerRequest, "atom:entry[@xml:base='http://myservicebase/']/atom:link[@rel='edit' and @href='Customers(0)']");
                });
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void BatchSmokeTestWhereRequestAndServiceUriAreReversed()
        {
            Run(request =>
                {
                    ((InProcessWebRequest)request).CustomServiceBaseUri = "http://host/" + Reverse("path1/path2");
                    Action<TestWebRequest> configureBatchRequest = r =>
                                                   {
                                                       r.RequestUriString = "/hctab$";
                                                       r.RequestHeaders["ReverseUriPaths"] = "true";
                                                   };
                    Action<TestWebRequest> configureInnerRequest = r =>
                                                   {
                                                       r.RequestUriString = Reverse("Customers(0)");
                                                       r.RequestHeaders["ReverseUriPaths"] = "true";
                                                   };
                    RunBatchTest(request, configureBatchRequest, configureInnerRequest, "atom:entry[@xml:base='http://host/path1/path2/']/atom:link[@rel='edit' and @href='Customers(0)']");
                });
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void BatchSmokeTestWhereRequestUriIsNotRelativeToService()
        {
            Run(request =>
                {
                    request.HttpMethod = "POST";

                    request.RequestUriString = "NotBatchAtAll";
                    request.RequestHeaders["MyCustomRequestUri"] = "http://myservicebase/path1/path2/$batch";
                    request.RequestHeaders["MyCustomServiceUri"] = "http://myservicebase/path1/path2/";

                    var batch = new BatchWebRequest();
                    var innerRequest = new InMemoryWebRequest();

                    innerRequest.RequestUriString = "SomethingThatWillNotBeUsedAtAll";
                    innerRequest.RequestHeaders["MyCustomRequestUri"] = "http://myservicebase/path2/Customers(0)";

                    batch.Parts.Add(innerRequest);
                    batch.SetContentTypeAndRequestStream(request);

                    TestUtil.RunCatching(request.SendRequest);
                    Assert.AreEqual(202, request.ResponseStatusCode);
                    batch.ParseResponseFromRequest(request, true);
                    Assert.AreEqual(400, innerRequest.ResponseStatusCode);
                    var innerPayload = innerRequest.GetResponseStreamAsXDocument();
                    UnitTestsUtil.VerifyXPathExists(innerPayload, "adsm:error/adsm:message[text()=\"The URI 'http://myservicebase/path2/Customers(0)' is not valid because it is not based on 'http://myservicebase/path1/path2/'.\"]");
                });
        }

        private static void RunBatchTest(TestWebRequest request, Action<TestWebRequest> configureBatchRequest, Action<TestWebRequest> configureInnerRequest, string innerRequestXPath)
        {
            request.HttpMethod = "POST";

            var batch = new BatchWebRequest();
            var innerRequest = new InMemoryWebRequest();
            innerRequest.Accept = "application/atom+xml,application/xml";

            configureBatchRequest(request);
            configureInnerRequest(innerRequest);

            batch.Parts.Add(innerRequest);
            batch.SetContentTypeAndRequestStream(request);
            TestUtil.RunCatching(request.SendRequest);
            Assert.AreEqual(202, request.ResponseStatusCode);
            batch.ParseResponseFromRequest(request, true);
            Assert.AreEqual(200, innerRequest.ResponseStatusCode);
            var innerPayload = innerRequest.GetResponseStreamAsXDocument();
            UnitTestsUtil.VerifyXPathExists(innerPayload, innerRequestXPath);
        }

        private static void GetResponseAndVerify(TestWebRequest request, int expectedStatusCode, string xpath)
        {
            TestUtil.RunCatching(request.SendRequest);

            Assert.AreEqual(expectedStatusCode, request.ResponseStatusCode);

            var responsePayload = request.GetResponseStreamAsXDocument();
            UnitTestsUtil.VerifyXPaths(responsePayload, new[] {xpath});
        }

        private static void Run(Action<TestWebRequest> runTest)
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            using (TestUtil.RestoreStaticValueOnDispose(typeof(BaseTestWebRequest), "HostInterfaceType"))
            {
                BaseTestWebRequest.HostInterfaceType = typeof(IDataServiceHost2);
                request.DataServiceType = typeof(RequestUriCustomizationService);
                runTest(request);
            }
        }

        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class RequestUriCustomizationService : DataService<CustomDataContext>
        {
            public static void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.UseVerboseErrors = true;
                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            }

            protected override void OnStartProcessingRequest(ProcessRequestArgs args)
            {
                if (args.OperationContext.RequestHeaders["ReverseUriPaths"] == "true")
                {
                    var relativeRequestUri = args.ServiceUri.MakeRelativeUri(args.RequestUri);

                    if (!args.IsBatchOperation)
                    {
                        args.ServiceUri = ReversePath(args.ServiceUri);
                    }

                    args.RequestUri = new Uri(args.ServiceUri, ReversePath(relativeRequestUri));
                }
                else
                {
                    if (!args.IsBatchOperation)
                    {
                        args.ServiceUri = new Uri(args.OperationContext.RequestHeaders["MyCustomServiceUri"]);
                    }

                    args.RequestUri = new Uri(args.OperationContext.RequestHeaders["MyCustomRequestUri"]);
                }
            }
        }

        private static Uri ReversePath(Uri toReverse)
        {
            string pathValue;
            if(toReverse.IsAbsoluteUri)
            {
                pathValue = toReverse.AbsolutePath;
            }
            else
            {
                pathValue = toReverse.OriginalString;
            }

            pathValue = Reverse(pathValue);
            if (!toReverse.IsAbsoluteUri)
            {
                return new Uri(pathValue, UriKind.Relative);
            }

            UriBuilder builder = new UriBuilder(toReverse.Scheme, toReverse.Host, toReverse.Port, pathValue);
            return builder.Uri;
        }

        private static string Reverse(string toReverse)
        {
            return new string(toReverse.Reverse().ToArray());
        }
    }
}