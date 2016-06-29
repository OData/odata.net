//---------------------------------------------------------------------
// <copyright file="KeyAsSegmentLongSpanIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service.Providers;
    using System.Data.Test.Astoria;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Xml;
    using AstoriaUnitTests.Stubs;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ResourceType = Microsoft.OData.Service.Providers.ResourceType;

    [TestClass]
    public class KeyAsSegmentLongSpanIntegrationTests
    {
        private static readonly string EntityTypeNameWithStringKey = typeof(KeyAsSegmentLongSpanIntegrationTests).FullName + "_EntityTypeWithStringKey";

        [Ignore] // Issue: #623
        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentEndToEndSmokeTestInJsonLight()
        {
            RunEndToEndSmokeTestWithClient(ctx =>
                                           {
                                               ctx.ResolveName = t => t.FullName;
                                               ctx.ResolveType = n => n.Contains("Customer") ? typeof(Customer) : null;
                                               ctx.Format.UseJson(EdmxReader.Parse(XmlReader.Create(ctx.GetMetadataUri().AbsoluteUri)));
                                           });
        }
        [Ignore] // Remove Atom
        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForTopLevelEntity()
        {
            ResponseShouldBeEntryWithEditLink("/Customers/0", "Customers/0");
        }
        [Ignore] // Remove Atom
        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForProperty()
        {
            ResponseShouldBeValueElement("/Customers/0/Name", "Customer 0");
        }
        [Ignore] // Remove Atom
        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForTypeSegments()
        {
            ResponseShouldBeEntryWithEditLink("/Customers/$/AstoriaUnitTests.Stubs.Customer/0", "Customers/0");
            ResponseShouldBeEntryWithEditLink("/Customers/0/AstoriaUnitTests.Stubs.Customer", "Customers/0");
        }
        [Ignore] // Remove Atom
        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForNavigation()
        {
            ResponseShouldBeEntryWithEditLink("/Customers/1/BestFriend", "Customers/0");
            ResponseShouldBeEntryWithEditLink("/Customers/0/Orders/0", "Orders/0");
        }
        [Ignore] // Remove Atom
        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForLinks()
        {
            ResponseShouldBeSingleLink("/Customers/1/BestFriend/$ref", "Customers/0");
            ResponseShouldBeLinkCollection("/Customers/0/Orders/$ref", "Orders/0");
        }
        [Ignore] // Remove Atom
        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForServiceOperation()
        {
            ResponseShouldBeEntryWithEditLink("/GetCustomers/0", "Customers/0");
        }
        [Ignore] // Remove Atom
        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForReservedNameAsKeyValue()
        {
            ResponseShouldBeEntryWithEditLink("/StringKeys/$$count", "StringKeys/%24%24count");
            ResponseShouldBeEntryWithEditLink("/StringKeys/$$ref", "StringKeys/%24%24ref");
            ResponseShouldBeEntryWithEditLink("/StringKeys/$$filter", "StringKeys/%24%24filter");

            ResponseShouldHaveMediaType("/StringKeys/$count", "text/plain;charset=utf-8");

            ResponseShouldHaveStatusCode("/StringKeys/$ref", 400);
            ResponseShouldHaveStatusCode("/StringKeys/$ref", 400);
        }
        [Ignore] // Remove Atom
        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForTypeNameAsKeyValue()
        {
            ResponseShouldBeEntryWithEditLink("/StringKeys/" + EntityTypeNameWithStringKey, "StringKeys/" + EntityTypeNameWithStringKey);

            ResponseShouldBeFeed("/StringKeys/$/" + EntityTypeNameWithStringKey, 200);
        }
        [Ignore] // Remove Atom
        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForActionNameAsKeyValue()
        {
            ResponseShouldBeEntryWithEditLink("/StringKeys/Action?$format=atom", "StringKeys/Action");
            ResponseShouldHaveStatusCode("/StringKeys/$/AstoriaUnitTests.Tests.Action?$format=atom", 405);
        }
        [Ignore] // Remove Atom
        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForStream()
        {
            ResponseShouldBeEntryWithEditLink("/StringKeys/$$value", "StringKeys/%24%24value");
            ResponseShouldHaveStatusCode("/StringKeys/$value", 400);
            ResponseShouldHaveMediaType("/StringKeys/$$value/$value", "application/jpeg");
        }
        [Ignore] // Remove Atom
        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForKeysWithParens()
        {
            ResponseShouldBeEntryWithEditLink("/StringKeys/()", "StringKeys/" + Uri.EscapeDataString("()"));
            ResponseShouldBeEntryWithEditLink("/StringKeys/(", "StringKeys/" + Uri.EscapeDataString("("));
            ResponseShouldBeEntryWithEditLink("/StringKeys/)", "StringKeys/" + Uri.EscapeDataString(")"));
            ResponseShouldBeEntryWithEditLink("/StringKeys/pa()rens", "StringKeys/" + Uri.EscapeDataString("pa()rens"));
            ResponseShouldBeEntryWithEditLink("/StringKeys/parens()", "StringKeys/" + Uri.EscapeDataString("parens()"));
            ResponseShouldBeEntryWithEditLink("/StringKeys/parens(", "StringKeys/" + Uri.EscapeDataString("parens("));
            ResponseShouldBeEntryWithEditLink("/StringKeys/(parens", "StringKeys/" + Uri.EscapeDataString("(parens"));
            ResponseShouldBeEntryWithEditLink("/StringKeys/(parens)", "StringKeys/" + Uri.EscapeDataString("(parens)"));
            ResponseShouldBeEntryWithEditLink("/StringKeys/)parens(", "StringKeys/" + Uri.EscapeDataString(")parens("));
        }
        [Ignore] // Remove Atom
        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentMetadataSmokeTest()
        {
            // <Annotations Target="AstoriaUnitTests.Tests.KeyAsSegmentContext">
            //   <Annotation Term="Com.Microsoft.OData.Service.Conventions.V1.UrlConventions" String="KeyAsSegment" />
            // </Annotations>
            ResponseShouldMatchXPath(
                "/$metadata",
                200,
                "//csdl:Annotations[@Target='AstoriaUnitTests.Tests.KeyAsSegmentContext']/csdl:Annotation[@Term='Com.Microsoft.OData.Service.Conventions.V1.UrlConventions' and @String='KeyAsSegment']");
        }

        private static void ResponseShouldBeEntryWithEditLink(string requestUriString, string expectedEditLink)
        {
            string xpath = "atom:entry/atom:link[@rel='edit' and @href=\"" + expectedEditLink + "\"]";
            ResponseShouldMatchXPath(requestUriString, 200, xpath);
        }

        private static void ResponseShouldBeLinkCollection(string requestUriString, string expectedLink)
        {
            // TODO: Change this as part of changing payload for EntityReference
            string xpath = "atom:feed/adsm:ref[contains(@id,\"" + expectedLink + "\")]";
            ResponseShouldMatchXPath(requestUriString, 200, xpath);
        }

        private static void ResponseShouldBeSingleLink(string requestUriString, string expectedLink)
        {
            string xpath = "adsm:ref[contains(@id,\"" + expectedLink + "\")]";
            ResponseShouldMatchXPath(requestUriString, 200, xpath);
        }

        private static void ResponseShouldBeValueElement(string requestUriString, string propertyValue)
        {
            string xpath = "adsm:value" + "[text()='" + propertyValue + "']";
            ResponseShouldMatchXPath(requestUriString, 200, xpath);
        }

        private static void ResponseShouldBeProperty(string requestUriString, string propertyName, string propertyValue)
        {
            string xpath = "ads:" + propertyName + "[text()='" + propertyValue + "']";
            ResponseShouldMatchXPath(requestUriString, 200, xpath);
        }

        private static void ResponseShouldHaveMediaType(string requestUriString, string mediaType)
        {
            ResponseShouldHaveStatusCode(
                requestUriString,
                200,
                request => Assert.AreEqual(mediaType, request.ResponseContentType));
        }

        private static void ResponseShouldBeFeed(string requestUriString, int statusCode)
        {
            ResponseShouldMatchXPath(requestUriString, statusCode, "atom:feed");
        }

        private static void ResponseShouldMatchXPath(string requestUriString, int statusCode, string xpath)
        {
            ResponseShouldHaveStatusCode(
                requestUriString,
                statusCode,
                request =>
                {
                    var responsePayload = request.GetResponseStreamAsXDocument();
                    UnitTestsUtil.VerifyXPaths(responsePayload, new[] { xpath });
                });
        }

        private static void ResponseShouldHaveStatusCode(string requestUriString, int statusCode, Action<TestWebRequest> verifyResponse = null)
        {
            Run(request =>
            {
                request.Accept = "application/atom+xml,application/xml,application/jpeg,text/plain;charset=utf-8";
                request.RequestUriString = requestUriString;
                request.RequestHeaders["DataServiceUrlConventions"] = "KeyAsSegment";

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
            {
                BaseTestWebRequest.HostInterfaceType = typeof(IDataServiceHost2);

                request.DataServiceType = typeof(KeyAsSegmentService);

                runTest(request);
            }
        }

        private static void RunEndToEndSmokeTestWithClient(Action<DataServiceContext> customize = null)
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(KeyAsSegmentService);

                request.StartService();

                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4) { UrlConventions = DataServiceUrlConventions.KeyAsSegment };
                if (customize != null)
                {
                    customize(ctx);
                }

                var customer = ctx.CreateQuery<Customer>("Customers").Where(c => c.ID == 0).Single();
                var descriptor = ctx.GetEntityDescriptor(customer);

                var navigationsLinks = descriptor.LinkInfos.Where(l => l.NavigationLink != null).ToList();
                var baseUri = request.ServiceRoot.AbsoluteUri;
                Assert.AreEqual(baseUri + "/Customers/0", descriptor.Identity.OriginalString);
                Assert.AreEqual(baseUri + "/Customers/0", descriptor.EditLink.OriginalString);
                Assert.AreEqual(baseUri + "/Customers/0/BestFriend/$ref", navigationsLinks[0].AssociationLink.OriginalString);
                Assert.AreEqual(baseUri + "/Customers/0/BestFriend", navigationsLinks[0].NavigationLink.OriginalString);
                Assert.AreEqual(baseUri + "/Customers/0/Orders/$ref", navigationsLinks[1].AssociationLink.OriginalString);
                Assert.AreEqual(baseUri + "/Customers/0/Orders", navigationsLinks[1].NavigationLink.OriginalString);
            }
        }

        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class KeyAsSegmentService : DataService<KeyAsSegmentContext>
        {
            public static void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
                config.UseVerboseErrors = true;

                config.DataServiceBehavior.UrlConventions = DataServiceUrlConventions.KeyAsSegment;

                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                config.DataServiceBehavior.IncludeAssociationLinksInResponse = true;
            }

            [WebGet]
            public IQueryable<Customer> GetCustomers()
            {
                return this.CurrentDataSource.Customers;
            }
        }

        public class KeyAsSegmentContext : CustomDataContext, IDataServiceActionProvider, IDataServiceStreamProvider
        {
            public IQueryable<EntityTypeWithStringKey> StringKeys
            {
                get
                {
                    return new List<EntityTypeWithStringKey>
                    {
                        new EntityTypeWithStringKey { ID = "$count" },
                        new EntityTypeWithStringKey { ID = "$ref" },
                        new EntityTypeWithStringKey { ID = "$filter" },
                        new EntityTypeWithStringKey { ID = "$value" },
                        new EntityTypeWithStringKey { ID = EntityTypeNameWithStringKey },
                        new EntityTypeWithStringKey { ID = "Action" },
                        new EntityTypeWithStringKey { ID = "()" },
                        new EntityTypeWithStringKey { ID = "(" },
                        new EntityTypeWithStringKey { ID = ")" },
                        new EntityTypeWithStringKey { ID = "pa()rens" },
                        new EntityTypeWithStringKey { ID = "parens()" },
                        new EntityTypeWithStringKey { ID = "parens(" },
                        new EntityTypeWithStringKey { ID = "(parens" },
                        new EntityTypeWithStringKey { ID = "(parens)" },
                        new EntityTypeWithStringKey { ID = ")parens(" },
                    }.AsQueryable();
                }
            }

            public IEnumerable<ServiceAction> GetServiceActions(DataServiceOperationContext operationContext)
            {
                return Enumerable.Empty<ServiceAction>();
            }

            public bool TryResolveServiceAction(DataServiceOperationContext operationContext, string serviceActionName, out ServiceAction serviceAction)
            {
                if (serviceActionName == "Action")
                {
                    IDataServiceMetadataProvider metadataProvider = (IDataServiceMetadataProvider)operationContext.GetService(typeof(IDataServiceMetadataProvider));
                    ResourceType resourceType;
                    metadataProvider.TryResolveResourceType(EntityTypeNameWithStringKey, out resourceType);
                    Assert.IsNotNull(resourceType);
                    serviceAction = new ServiceAction("Action", ResourceType.GetPrimitiveResourceType(typeof(string)), OperationParameterBindingKind.Always, new[] { new ServiceActionParameter("param1", ResourceType.GetEntityCollectionResourceType(resourceType)) }, null);
                    serviceAction.SetReadOnly();
                    return true;
                }

                serviceAction = null;
                return false;
            }

            public IEnumerable<ServiceAction> GetServiceActionsByBindingParameterType(DataServiceOperationContext operationContext, ResourceType bindingParameterType)
            {
                var collectionType = bindingParameterType as EntityCollectionResourceType;
                if (collectionType != null && collectionType.ItemType.Name == "EntityTypeWithStringKey")
                {
                    var serviceAction = new ServiceAction("Action", ResourceType.GetPrimitiveResourceType(typeof(string)), OperationParameterBindingKind.Always, new[] { new ServiceActionParameter("param1", bindingParameterType) }, null);
                    serviceAction.SetReadOnly();
                    yield return serviceAction;
                }
            }

            public IDataServiceInvokable CreateInvokable(DataServiceOperationContext operationContext, ServiceAction serviceAction, object[] parameterTokens)
            {
                throw new System.NotImplementedException();
            }

            public bool AdvertiseServiceAction(DataServiceOperationContext operationContext, ServiceAction serviceAction, object resourceInstance, bool resourceInstanceInFeed, ref ODataAction actionToSerialize)
            {
                return true;
            }

            public int StreamBufferSize
            {
                get { return 1024; }
            }

            public Stream GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
            {
                return new MemoryStream(new byte[] { 1, 2, 3, 4, 5 });
            }

            public Stream GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
            {
                throw new NotImplementedException();
            }

            public void DeleteStream(object entity, DataServiceOperationContext operationContext)
            {
                throw new NotImplementedException();
            }

            public string GetStreamContentType(object entity, DataServiceOperationContext operationContext)
            {
                return "application/jpeg";
            }

            public Uri GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
            {
                return null;
            }

            public string GetStreamETag(object entity, DataServiceOperationContext operationContext)
            {
                return null;
            }

            public string ResolveType(string entitySetName, DataServiceOperationContext operationContext)
            {
                throw new NotImplementedException();
            }
        }

        [HasStream]
        public class EntityTypeWithStringKey
        {
            public string ID { get; set; }
        }
    }
}
