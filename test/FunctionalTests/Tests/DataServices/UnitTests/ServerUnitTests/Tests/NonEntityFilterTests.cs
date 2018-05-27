//---------------------------------------------------------------------
// <copyright file="NonEntityFilterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Data.Test.Astoria;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
    [Ignore] // Remove Atom
    // [TestClass]
    public class NonEntityFilterTests
    {
        [TestCategory("Partition1")]
        [TestMethod]
        public void FilterOnDeclaredComplexPropertyShouldNotBeApplied()
        {
            ResponseShouldMatchXPath("/Customers(0)/Address?$filter=false", false, 200, "adsm:value");
        }

        [TestCategory("Partition1")]
        [TestMethod]
        public void FilterOnDynamicPropertyShouldNotBeApplied()
        {
            ResponseShouldMatchXPath("/Customers(0)/Something?$filter=false", true, 200, "adsm:value[@adsm:null='true']");
        }

        [TestCategory("Partition1")]
        [TestMethod]
        public void FilterOnDynamicComplexShouldNotBeApplied()
        {
            ResponseShouldMatchXPath("/Customers(0)/Address?$filter=Street ne '123 fake st'", true, 200, "adsm:value");
        }

        [TestCategory("Partition1")]
        [TestMethod]
        public void FilterOnDeclaredComplexPropertyShouldBeIgnored()
        {
            ResponseShouldMatchXPath("/Customers(0)/Address?$filter=Should Be Ignored", false, 200, "adsm:value");
        }

        [TestCategory("Partition1")]
        [TestMethod]
        public void FilterOnDynamicPropertyShouldBeIgnored()
        {
            ResponseShouldMatchXPath("/Customers(0)/Something?$filter=Should Be Ignored", true, 200, "adsm:value[@adsm:null='true']");
        }

        [TestCategory("Partition1")]
        [TestMethod]
        public void FilterOnDynamicComplexShouldBeIgnored()
        {
            ResponseShouldMatchXPath("/Customers(0)/Address?$filter=Should Be Ignored", true, 200, "adsm:value");
        }


        private static void ResponseShouldMatchXPath(string requestUriString, bool openType, int statusCode, string xpath)
        {
            ResponseShouldHaveStatusCode(requestUriString, openType, statusCode, request =>
                                                                       {
                                                                           var responsePayload = request.GetResponseStreamAsXDocument();
                                                                           UnitTestsUtil.VerifyXPaths(responsePayload, new[] {xpath});
                                                                       });
        }

        private static void ResponseShouldHaveStatusCode(string requestUriString, bool openType, int statusCode, Action<TestWebRequest> verifyResponse = null)
        {
            Run(openType, request =>
            {
                    request.Accept = "application/atom+xml,application/xml";
                    request.RequestUriString = requestUriString;

                    TestUtil.RunCatching(request.SendRequest);

                    Assert.AreEqual(statusCode, request.ResponseStatusCode);

                    if (verifyResponse != null)
                    {
                        verifyResponse(request);
                    }
                });
        }

        private static void Run(bool openType, Action<TestWebRequest> runTest)
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                if (openType)
                {
                    request.DataServiceType = typeof(CustomRowBasedOpenTypesContext);
                }
                else
                {
                    request.DataServiceType = typeof(CustomRowBasedContext);    
                }
                
                runTest(request);
            }
        }
    }
}