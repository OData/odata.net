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
    public class RequestUriCustomizationIntegrationTests
    {
        [TestCategory("Partition2")]
        [TestMethod]
        public void GetSmokeTestWhereRequestAndServiceUriAreCompletelyReplaced()
        {
            Run(request =>
            {
                request.RequestUriString = "/WhoCaresWhatIPutHere/Something/SomethingElse/TotallyIgnored";
                request.RequestHeaders["MyCustomRequestUri"] = "http://myservicebase/Customers(0)";
                request.RequestHeaders["MyCustomServiceUri"] = "http://myservicebase/";
                request.Accept = "application/json";

                GetResponseAndVerify(request, 200, new string[] {
                    "http://myservicebase/$metadata#Customers/$entity",
                    "\"ID\":0,\"Name\":\"Customer 0\",\"NameAsHtml\":" +
                    "\"<html><body>Customer 0</body></html>\",\"Address\":" +
                    "{\"@odata.type\":\"#AstoriaUnitTests.Stubs.Address\",\"StreetAddress\":" +
                    "\"Line1\",\"City\":\"Redmond\",\"State\":\"WA\",\"PostalCode\":\"98052\""
                });
            });
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void GetSmokeTestWhereRequestUriPathIsReversed()
        {
            Run(request =>
            {
                ((InProcessWebRequest)request).CustomServiceBaseUri = "http://host/" + Reverse("path1/path2");
                request.RequestUriString = "/" + Reverse("Customers(0)");
                request.RequestHeaders["ReverseUriPaths"] = "true";
                request.Accept = "application/json";

                GetResponseAndVerify(request, 200, new string[] {
                    "http://host/path1/path2/$metadata#Customers/$entity",
                    "\"ID\":0,\"Name\":\"Customer 0\",\"NameAsHtml\":" +
                    "\"<html><body>Customer 0</body></html>\",\"Address\":" +
                    "{\"@odata.type\":\"#AstoriaUnitTests.Stubs.Address\",\"StreetAddress\":" +
                    "\"Line1\",\"City\":\"Redmond\",\"State\":\"WA\",\"PostalCode\":\"98052\""
                });
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

        private static void GetResponseAndVerify(TestWebRequest request, int expectedStatusCode, string[] verificationStrings)
        {
            TestUtil.RunCatching(request.SendRequest);
            Assert.AreEqual(expectedStatusCode, request.ResponseStatusCode);

            string responsePayload = request.GetResponseStreamAsText();
            foreach (string verifyString in verificationStrings)
            {
                Assert.IsTrue(responsePayload.Contains(verifyString));
            }
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