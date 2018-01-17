//---------------------------------------------------------------------
// <copyright file="UseDefaultNamespaceForRootElementsLongSpanIntegrationTests.cs" company="Microsoft">
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
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
    [Ignore] // Remove Atom
    // [TestClass]
    public class UseDefaultNamespaceForRootElementsLongSpanIntegrationTests
    {
        [TestCategory("Partition2")]
        [TestMethod]
        public void TopLevelErrorShouldNotHavePrefix()
        {
            ResponseShouldHaveStatusCode(
                "/Customers(-11)",
                404,
                request =>
                    {
                        var responsePayload = request.GetResponseStreamAsText();
                        Assert.IsTrue(responsePayload.Contains("xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\""));
                        Assert.IsFalse(responsePayload.Contains("d:"));
                        Assert.IsFalse(responsePayload.Contains("xmlns:d"));
                        Assert.IsFalse(responsePayload.Contains("m:"));
                        Assert.IsFalse(responsePayload.Contains("xmlns:m"));
                        Assert.IsFalse(responsePayload.Contains("atom"));
                    });
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void TopLevelPrimitivePropertyShouldNotHavePrefix()
        {
            ResponseShouldHaveDataServicesMetadataNamespaceAsDefault("/Customers(1)/GuidValue");
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void TopLevelComplexPropertyShouldNotHavePrefix()
        {
            ResponseShouldHaveDataServicesMetadataNamespaceAsDefault("/Customers(1)/Address");
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void TopLevelCollectionPropertyShouldNotHavePrefix()
        {
            ResponseShouldHaveDataServicesMetadataNamespaceAsDefault("/EntitiesWithCollectionProperties(0)/Numbers");
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void SingleValueServiceOperationShouldNotHavePrefix()
        {
            ResponseShouldHaveDataServicesMetadataNamespaceAsDefault("/GetNumber");
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void CollectionServiceOperationShouldNotHavePrefix()
        {
            ResponseShouldHaveDataServicesMetadataNamespaceAsDefault("/GetNumbers");
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void LinkCollectionShouldHaveAtomAsDefaultNamespace()
        {
            ResponseShouldHaveAtomNamespaceAsDefaultAndOthersWithNormalPrefixes("/Customers(1)/Orders/$ref", false);
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void SingleLinkShouldHaveMetadataNamesapceAsDefault()
        {
            ResponseShouldHaveDataServicesMetadataNamespaceAsDefault("/Customers(1)/Orders(1)/$ref");
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void FeedShouldHaveNormalPrefixes()
        {
            ResponseShouldHaveAtomNamespaceAsDefaultAndOthersWithNormalPrefixes("/Customers?$top=1");
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void EntryShouldHaveNormalPrefixes()
        {
            ResponseShouldHaveAtomNamespaceAsDefaultAndOthersWithNormalPrefixes("/Customers(1)");
        }

        private static void ResponseShouldHaveAtomNamespaceAsDefaultAndOthersWithNormalPrefixes(string requestUriString, bool includeDataNS = true)
        {
            ResponseShouldHaveStatusCode(
                requestUriString,
                200,
                request =>
                {
                    var responsePayload = request.GetResponseStreamAsText();
                    Assert.IsTrue(responsePayload.Contains("xmlns=\"http://www.w3.org/2005/Atom\""));

                    if (includeDataNS)
                    {
                        Assert.IsTrue(responsePayload.Contains("d:"));
                        Assert.IsTrue(responsePayload.Contains("xmlns:d"));
                    }

                    Assert.IsTrue(responsePayload.Contains("m:"));
                    Assert.IsTrue(responsePayload.Contains("xmlns:m"));
                });
        }

        private static void ResponseShouldHaveDataServicesMetadataNamespaceAsDefault(string requestUriString)
        {
            ResponseShouldHaveStatusCode(
                requestUriString,
                200,
                request =>
                {
                    var responsePayload = request.GetResponseStreamAsText();
                    Assert.IsTrue(responsePayload.Contains("xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\""));

                    // Attributes could still have m: or xmlns:m, no assertion on their absence.
                    Assert.IsFalse(responsePayload.Contains("atom"));
                });
        }

        private static void ResponseShouldHaveStatusCode(string requestUriString, int statusCode, Action<TestWebRequest> verifyResponse = null)
        {
            Run(request =>
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

        private static void Run(Action<TestWebRequest> runTest)
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            using (TestUtil.RestoreStaticValueOnDispose(typeof(BaseTestWebRequest), "HostInterfaceType"))
            using (TestServiceHost.AllowServerToSerializeException.Restore())
            {
                BaseTestWebRequest.HostInterfaceType = typeof(IDataServiceHost2);
                TestServiceHost.AllowServerToSerializeException.Value = true;

                request.DataServiceType = typeof(UseDefaultNamespaceForRootElementsService);

                runTest(request);
            }
        }

        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class UseDefaultNamespaceForRootElementsService : DataService<UseDefaultNamespaceForRootElementsContext>
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
            public int GetNumber()
            {
                return 1;
            }

            [WebGet]
            public IEnumerable<int> GetNumbers()
            {
                return new[] { 1, 2 };
            }
        }

        public class UseDefaultNamespaceForRootElementsContext : CustomDataContext
        {
            public IQueryable<EntityTypeWithCollectionProperties> EntitiesWithCollectionProperties
            {
                get
                {
                    return new List<EntityTypeWithCollectionProperties>
                            {
                                new EntityTypeWithCollectionProperties()
                            }
                            .AsQueryable();
                }
            }
        }

        public class EntityTypeWithCollectionProperties
        {
            public int ID { get; set; }

            public int[] Numbers
            {
                get { return new[] { 1, 2 }; }
            }
        }
    }
}
