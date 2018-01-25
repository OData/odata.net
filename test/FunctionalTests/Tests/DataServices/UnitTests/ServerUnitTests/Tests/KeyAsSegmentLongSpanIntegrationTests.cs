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

        /*
        // Issue: #623
        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentEndToEndSmokeTestInJsonLight()
        {
            RunEndToEndSmokeTestWithClient(ctx =>
                                           {
                                               ctx.ResolveName = t => t.FullName;
                                               ctx.ResolveType = n => n.Contains("Customer") ? typeof(Customer) : null;
                                               ctx.Format.UseJson(CsdlReader.Parse(XmlReader.Create(ctx.GetMetadataUri().AbsoluteUri)));
                                           });
        }
        */

        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForTopLevelEntity()
        {
            ResponseShouldContainEntry("/Customers/0", "\"Name\":\"Customer 0\"");
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForProperty()
        {
            ResponseShouldContainEntry("/Customers/0/Name", "\"value\":\"Customer 0\"");
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForTypeSegments()
        {
            ResponseShouldContainEntry("/Customers/$/AstoriaUnitTests.Stubs.Customer/0",
                "\"ID\":0,\"Name\":\"Customer 0\"");
            ResponseShouldContainEntry("/Customers/0/AstoriaUnitTests.Stubs.Customer",
                "\"ID\":0,\"Name\":\"Customer 0\"");
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForNavigation()
        {
            ResponseShouldContainEntry("/Customers/1/BestFriend",
                "\"ID\":0,\"Name\":\"Customer 0\"");
            ResponseShouldContainEntry("/Customers/0/Orders/0",
                "\"ID\":0,\"DollarAmount\":20.1,\"CurrencyAmount\":null");
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForLinks()
        {
            // Response should be single link
            ResponseShouldContainEntry("/Customers/1/BestFriend/$ref",
                "{\"@odata.context\":\"http://host/$metadata#$ref\",\"@odata.id\":\"http://host/Customers/0\"}");

            // Response should be link collection
            ResponseShouldContainEntry("/Customers/0/Orders/$ref",
                "{\"@odata.context\":\"http://host/$metadata#Collection($ref)\",\"value\":" +
                "[{\"@odata.id\":\"http://host/Orders/0\"},{\"@odata.id\":\"http://host/Orders/100\"}]}");
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForServiceOperation()
        {
            ResponseShouldContainEntry("/GetCustomers/0", "\"ID\":0,\"Name\":\"Customer 0\"");
        }

        /*
         * https://github.com/OData/odata.net/issues/839
         * There's one failed case in here. See comment.
        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForReservedNameAsKeyValue()
        {
            ResponseShouldContainEntry("/StringKeys/$$count", "StringKeys/%24%24count");
            ResponseShouldContainEntry("/StringKeys/$$ref", "StringKeys/%24%24ref");
            ResponseShouldContainEntry("/StringKeys/$$filter", "StringKeys/%24%24filter");

            // FAILED CASE:
            // Should have media type "text/plain;charset=utf-8"
            // JSON is causing response HTTP to be 415
            ResponseShouldContainEntry("/StringKeys/$count", "text/plain;charset=utf-8");

            // Response should have response code 400
            ResponseShouldHaveStatusCode("/StringKeys/$ref", 400);
            ResponseShouldHaveStatusCode("/StringKeys/$ref", 400);
        }
        */

        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForTypeNameAsKeyValue()
        {
            ResponseShouldContainEntry("/StringKeys/" + EntityTypeNameWithStringKey,
                "StringKeys/" + EntityTypeNameWithStringKey);

            ResponseShouldContainEntry("/StringKeys/$/" + EntityTypeNameWithStringKey,
                "StringKeys/" + EntityTypeNameWithStringKey);
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForActionNameAsKeyValue()
        {
            ResponseShouldContainEntry("/StringKeys/Action", "StringKeys/Action");
            ResponseShouldHaveStatusCode("/StringKeys/$/AstoriaUnitTests.Tests.Action", 405);
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForStream()
        {
            ResponseShouldContainEntry("/StringKeys/$$value", "StringKeys/%24%24value");
            ResponseShouldHaveStatusCode("/StringKeys/$value", 400);
            ResponseShouldHaveStatusCode("/StringKeys/$$value/$value", 415);
        }

        /*
         * https://github.com/OData/odata.net/issues/840
        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentUriParsingSmokeTestForKeysWithParens()
        {
            // Returns 404
            ResponseShouldContainEntry("/StringKeys/()", "StringKeys/" + Uri.EscapeDataString("()"));
            // Returns 400
            ResponseShouldContainEntry("/StringKeys/(", "StringKeys/" + Uri.EscapeDataString("("));
            // Returns 200
            ResponseShouldContainEntry("/StringKeys/)", "StringKeys/" + Uri.EscapeDataString(")"));
            // Returns 400
            ResponseShouldContainEntry("/StringKeys/pa()rens", "StringKeys/" + Uri.EscapeDataString("pa()rens"));
            // Returns 200
            ResponseShouldContainEntry("/StringKeys/parens()", "StringKeys/" + Uri.EscapeDataString("parens()"));
            // Returns 400
            ResponseShouldContainEntry("/StringKeys/parens(", "StringKeys/" + Uri.EscapeDataString("parens("));
            // Returns 400
            ResponseShouldContainEntry("/StringKeys/(parens", "StringKeys/" + Uri.EscapeDataString("(parens"));
            // Returns 404
            ResponseShouldContainEntry("/StringKeys/(parens)", "StringKeys/" + Uri.EscapeDataString("(parens)"));
            // Returns 400
            ResponseShouldContainEntry("/StringKeys/)parens(", "StringKeys/" + Uri.EscapeDataString(")parens("));
        }
        */

        /*
         * https://github.com/OData/odata.net/issues/841
        [TestCategory("Partition2")]
        [TestMethod]
        public void KeyAsSegmentMetadataSmokeTest()
        {
            ResponseShouldContainEntry("/$metadata", "some expected string");
        }
        */

        private static void ResponseShouldContainEntry(string requestUriString, string expectedString)
        {
            ResponseShouldMatchVerification(requestUriString, 200, expectedString);
        }
        
        private static void ResponseShouldMatchVerification(string requestUriString, int statusCode, string expectedString)
        {
            ResponseShouldHaveStatusCode(
                requestUriString,
                statusCode,
                request =>
                {
                    string responsePayload = request.GetResponseStreamAsText();
                    Assert.IsTrue(responsePayload.Contains(expectedString),
                         string.Format("Response did not contain expected string: {0}", expectedString));
                });
        }

        private static void ResponseShouldHaveStatusCode(string requestUriString, int statusCode, Action<TestWebRequest> verifyResponse = null)
        {
            Run(request =>
            {
                request.Accept = "application/json";
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
        
        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class KeyAsSegmentService : DataService<KeyAsSegmentContext>
        {
            public static void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
                config.UseVerboseErrors = true;

                config.DataServiceBehavior.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

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
