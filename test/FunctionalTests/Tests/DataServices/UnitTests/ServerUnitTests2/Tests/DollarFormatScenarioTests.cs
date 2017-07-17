//---------------------------------------------------------------------
// <copyright file="DollarFormatScenarioTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/875
    /// <summary>
    /// This is a test class for $format and is intended to contain all $format scenario tests.
    /// </summary>
    [TestClass]
    public class DollarFormatScenarioTests
    {
        private const string applicationXml = "application/xml;charset=utf-8";
        private const string applicationAtomXml = "application/atom+xml;type=feed;charset=utf-8";
        private const string applicationJsonLight = "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8";

        [Ignore]
        // [TestCategory("Partition2"), TestMethod]
        public void DollarFormatSmokeTest()
        {
            const string expectedJsonLight = @"{""@odata.context"":""BASE_URI$metadata#Customers(1)/Address"",""StreetAddress"":""Line1"",""City"":""Redmond"",""State"":""WA"",""PostalCode"":""98052""}";

            // $format should override accept header and give us json light since MDSV is 3
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.Accept = "application/xml";
                request.RequestMaxVersion = "4.0";
                request.RequestVersion = "4.0";
                request.RequestUriString = "/Customers(1)/Address?$format=json";

                request.SendRequest();
                var actualText = request.GetResponseStreamAsText();

                Assert.AreEqual(200, request.ResponseStatusCode);
                Assert.AreEqual(expectedJsonLight.Replace("BASE_URI", request.BaseUri), actualText);
            }
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod]
        public void DollarFormatQueryTest()
        {
            var testCases = new List<DollarFormatTestCase>()
            {
                // $metadata tests
                new DollarFormatTestCase()
                {
                    UriString = "/$metadata?$format=atom",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/$metadata?$format=json",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/$metadata?$format=xml",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 200
                },

                // query keyword tests: covers $format with expand, select, top, skip, orderby, filter, any
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$expand=Orders&$select=ID,Name,Orders&$top=3&$skip=1&$orderby=ID&$filter=Orders/any(p:p/ID%20ne%200)&$format=atom",
                    ExpectedContentType = applicationAtomXml,
                    ExpectedStatusCode = 200
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$expand=Orders&$select=ID,Name,Orders&$top=3&$skip=1&$orderby=ID&$filter=Orders/any(p:p/ID%20ne%200)&$format=json",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 200
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$expand=Orders&$select=ID,Name,Orders&$top=3&$skip=1&$orderby=ID&$filter=Orders/any(p:p/ID%20ne%200)&$format=json",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 200
                },
               new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$expand=Orders&$select=ID,Name,Orders&$top=3&$skip=1&$orderby=ID&$filter=Orders/any(p:p/ID%20ne%200)&$format=xml",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },

                // query keyword tests : put $format in the middle of uri, covers $format with filter, all, inlinecount
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$format=atom&$filter=Orders/all(p:p/ID%20ge%200)&inlinecount=allpages",
                    ExpectedContentType = applicationAtomXml,
                    ExpectedStatusCode = 200
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$format=json&$filter=Orders/all(p:p/ID%20ge%200)&inlinecount=allpages",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 200
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$format=json&$filter=Orders/all(p:p/ID%20ge%200)&inlinecount=allpages",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 200
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$format=xml&$filter=Orders/all(p:p/ID%20ge%200)&inlinecount=allpages",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },

                // $format with $ref
                new DollarFormatTestCase()
                {
                    UriString = "/Customers(1)/Orders/$ref?$format=application/xml",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers(1)/Orders/$ref?$format=json",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 200
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers(1)/Orders/$ref?$format=json",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 200
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers(1)/Orders/$ref?$format=atom",
                    ExpectedContentType = applicationAtomXml,
                    ExpectedStatusCode = 200
                },

                //parser IEEE754Compatible format parameters
                new DollarFormatTestCase()
                {
                    UriString = "/Customers(1)/ID?$format=application/json;IEEE754Compatible=true",
                    ExpectedContentType = "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=true;charset=utf-8",
                    ExpectedStatusCode = 200
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers(1)/ID?$format=application/json;IEEE754Compatible=false",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 200
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers(1)/ID?$format=application/json;IEEE754Compatible=unrecognized",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 415
                },
            };

            this.RunQueryTest(testCases);
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod]
        public void DollarFormatQueryErrorTest()
        {
            var testCases = new List<DollarFormatTestCase>()
            {
                // $format with $count
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/$count?$format=atom",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/$count?$format=json",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/$count?$format=json",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/$count?$format=xml",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },

                // $format with $value
                new DollarFormatTestCase()
                {
                    UriString = "/Customers(1)/Name/$value?$format=atom",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers(1)/Name/$value?$format=json",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers(1)/Name/$value?$format=json",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers(1)/Name/$value?$format=xml",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },
            };

            this.RunQueryTest(testCases);
        }

        [TestCategory("Partition2"), TestMethod]
        public void EmptyDollarFormatValue()
        {
            var testCases = new List<DollarFormatTestCase>()
            {
                // No uri parsing error occurs if there are other query options
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$format=&$expand=Orders&$select=ID,Name,Orders",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 200
                },

                // Accept header is over-written by empty $format option
                new DollarFormatTestCase()
                {
                    UriString = "/Customers(1)/ID?$format=",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 200
                },
            };

            using (TestWebRequest webRequest = TestWebRequest.CreateForInProcessWcf())
            {
                webRequest.HttpMethod = "GET";
                webRequest.DataServiceType = typeof(DollarFormatTestService);

                foreach (var testCase in testCases)
                {
                    webRequest.RequestUriString = testCase.UriString;
                    webRequest.Accept = "application/atom+xml,application/xml"; // Set Accept header to be different from expected

                    TestUtil.RunCatching(webRequest.SendRequest);
                    Assert.AreEqual(testCase.ExpectedStatusCode, webRequest.ResponseStatusCode);
                    Assert.AreEqual(testCase.ExpectedContentType, webRequest.ResponseHeaders["Content-Type"]);
                }
            }
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod]
        public void DollarFormatMIMETypeTest()
        {
            var testCases = new List<DollarFormatTestCase>()
            {
                // use MIME types as format options, also test them with combination with other query keywords
                new DollarFormatTestCase()
                {
                    UriString = "/Customers(1)/ID?$format=application/xml",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 200
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$expand=Orders&$select=ID,Name,Orders&$top=3&$skip=1&$orderby=ID&$filter=Orders/any(p:p/ID%20ne%200)&$format=application/atom%2Bxml",
                    ExpectedContentType = applicationAtomXml,
                    ExpectedStatusCode = 200
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$filter=Orders/all(p:p/ID%20ge%200)&inlinecount=allpages&$format=application/json",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 200
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$format=application/json;odata.metadata=minimal;&$expand=Orders&$select=ID,Name,Orders&$top=3&$skip=1&$orderby=ID&$filter=Orders/any(p:p/ID%20ne%200)",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 200
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$format=application/json;odata.metadata=minimal;&$filter=Orders/all(p:p/ID%20ge%200)&inlinecount=allpages",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 200
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$format=*/*;&$filter=Orders/all(p:p/ID%20ge%200)&inlinecount=allpages",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 200
                },

                // $ref, $count, $value with MIME type format options
                new DollarFormatTestCase()
                {
                    UriString = "/Customers(1)/Orders/$ref?$format=application/atom%2Bxml",
                    ExpectedContentType = applicationAtomXml,
                    ExpectedStatusCode = 200
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers(1)/Orders/$ref?$format=application/json;charset=utf-8",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 200
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/$count?$format=application/xml;",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/$count?$format=application/json",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers(1)/Name/$value?$format=application/atom%2Bxml",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers(1)/Name/$value?$format=application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8",
                    ExpectedContentType = applicationJsonLight,
                    ExpectedStatusCode = 415
                },
            };

            this.RunQueryTest(testCases);
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod]
        public void DollarFormatInvalidFormatTest()
        {
            // Get 415 responses for invalid requests with $format
            var testCases = new List<DollarFormatTestCase>()
            {
                // jsonlight was removed as a special value, so we want to make sure it fails.
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$format=jsonlight",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },

                // $format option is case sentitive
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$format=Atom",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },

                // invalid $format options
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$format=blah",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$format=atomm",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$format=atom;",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$format=atom;json",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$format=atom/xml",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$format=*",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers(1)/ID?$format=application/xml;text/plain;",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },
                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$format=application/atom%2Bxml;application/xml",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },

                new DollarFormatTestCase()
                {
                    UriString = "/Customers/?$format=text/plain",
                    ExpectedContentType = applicationXml,
                    ExpectedStatusCode = 415
                },
            };

            this.RunQueryTest(testCases);
        }

        [TestCategory("Partition2"), TestMethod]
        public void SetDollarFormatInBuildingRequest()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(DollarFormatTestService);
                web.StartService();

                DataServiceContext ctx = new DataServiceContext(web.ServiceRoot, ODataProtocolVersion.V4);
                List<string> options = new List<string>()
                {
                   // "atom",  It will be enabled by finishing
                   // "json",  enable the json case by involving proper EDM model in DataServiceContext.
                    "xml",
                };
                string option = string.Empty;
                ctx.BuildingRequest += (sender, arg) => arg.RequestUri = new Uri(arg.RequestUri.AbsoluteUri + "?$format=" + option);

                foreach (string s in options)
                {
                    try
                    {
                        option = s;
                        ctx.Execute<Customer>(new Uri(web.ServiceRoot + "/Customers"));
                        ctx.CreateQuery<Customer>("Customers");
                        //  Assert.IsTrue(option == "json");
                    }
                    catch (DataServiceQueryException e)
                    {
                        if (option == "xml")
                        {
                            TestUtil.AssertContains(e.InnerException.Message, "A supported MIME type could not be found that matches the acceptable MIME types for the request.");
                            Assert.AreEqual(415, e.Response.StatusCode);
                        }
                        else
                        {
                            // Assert.AreEqual("atom", option);
                            // Assert.AreEqual(DataServicesClientResourceUtil.GetString("DataServiceClientFormat_ValidServiceModelRequiredForAtom"), e.InnerException.Message);
                        }
                    }
                }
            }
        }

        [TestCategory("Partition2"), TestMethod]
        public void SetDollarFormatInAddQueryOption()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(DollarFormatTestService);
                web.StartService();
                DataServiceContext ctx = new DataServiceContext(web.ServiceRoot, ODataProtocolVersion.V4);
                List<string> options = new List<string>()
                {
                    "atom",
                    "json",
                    "jsonlight",
                    "xml",
                };

                foreach (string option in options)
                {
                    try
                    {
                        ctx.CreateQuery<Customer>("Customers").AddQueryOption("$format", option).Execute();
                    }
                    catch (NotSupportedException e)
                    {
                        Assert.AreEqual(DataServicesClientResourceUtil.GetString("ALinq_FormatQueryOptionNotSupported"), e.Message);
                    }
                }
            }
        }

        private void RunQueryTest(List<DollarFormatTestCase> testCases, string RequestMaxVersion = "4.0;")
        {
            using (TestWebRequest webRequest = TestWebRequest.CreateForInProcessWcf())
            {
                webRequest.HttpMethod = "GET";
                webRequest.DataServiceType = typeof(DollarFormatTestService);

                foreach (var testCase in testCases)
                {
                    webRequest.RequestUriString = testCase.UriString;
                    webRequest.RequestMaxVersion = RequestMaxVersion;

                    TestUtil.RunCatching(webRequest.SendRequest);
                    Assert.AreEqual(testCase.ExpectedStatusCode, webRequest.ResponseStatusCode);
                    Assert.AreEqual(testCase.ExpectedContentType, webRequest.ResponseHeaders["Content-Type"]);
                }
            }
        }

        /// <summary>
        /// Test service to be used in $format tests.
        /// </summary>
        public class DollarFormatTestService : DataService<CustomDataContext>
        {
            public static void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
                config.UseVerboseErrors = true;
                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            }
        }

        private class DollarFormatTestCase
        {
            public string UriString { get; set; }
            public string ExpectedContentType { get; set; }
            public int ExpectedStatusCode { get; set; }
        }
    }
}
