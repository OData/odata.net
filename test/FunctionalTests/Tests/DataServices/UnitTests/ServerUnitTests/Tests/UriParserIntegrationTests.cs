//---------------------------------------------------------------------
// <copyright file="UriParserIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Reflection;

namespace AstoriaUnitTests.Tests
{
    using AstoriaUnitTests.Stubs;
    using Microsoft.OData.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Data.Test.Astoria;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Xml.Linq;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
    [TestClass]
    public class UriParserIntegrationTests
    {
        [Ignore] // Remove Atom
        // [TestCategory("Partition2")]
        // [TestMethod]
        public void RequestShouldFailIfExpandIsTooDeep()
        {
            string errorMessage = ODataLibResourceUtil.GetString("UriParser_ExpandDepthExceeded", 3, 2);
            ResponseShouldMatchXPath(
                "/Entities?$expand=Reference($expand=Reference($expand=Reference))",
                400,
                "//adsm:error/adsm:message[text()='" + errorMessage + "']", serviceType: typeof(UriParserIntegrationTestServiceWithLowLimits));
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition2")]
        // [TestMethod]
        public void RequestShouldNotFailIfExpandDepthIsIsExactlyAtMax()
        {
            ResponseShouldMatchXPath(
                "/Entities?$expand=Reference($expand=Reference)",
                200,
                "//atom:feed", serviceType: typeof(UriParserIntegrationTestServiceWithLowLimits));
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition2")]
        // [TestMethod]
        public void RequestShouldFailIfTooManyItemsAreExpanded()
        {
            string errorMessage = ODataLibResourceUtil.GetString("UriParser_ExpandCountExceeded", 4, 3);
            ResponseShouldMatchXPath(
                "/Entities?$expand=Reference,Reference2,Reference3,Reference4",
                400,
                "//adsm:error/adsm:message[text()='" + errorMessage + "']", serviceType: typeof(UriParserIntegrationTestServiceWithLowLimits));
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition2")]
        // [TestMethod]
        public void RequestShouldFailIfTooManyItemsAreExpandedAtMultipleLevels()
        {
            string errorMessage = ODataLibResourceUtil.GetString("UriParser_ExpandCountExceeded", 4, 3);
            ResponseShouldMatchXPath(
                "/Entities?$expand=Reference($expand=Reference2),Reference3($expand=Reference4)",
                400,
                "//adsm:error/adsm:message[text()='" + errorMessage + "']", serviceType: typeof(UriParserIntegrationTestServiceWithLowLimits));
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition2")]
        // [TestMethod]
        public void RequestShouldFailIfTooManyItemsAreExpandedEvenIfTheRootOneIsDuplicated()
        {
            string errorMessage = DataServicesResourceUtil.GetString("DataService_ExpandCountExceeded", 4, 3);
            ResponseShouldMatchXPath(
                "/Entities?$expand=Reference($expand=Reference),Reference($expand=Reference2)",
                400,
                "//adsm:error/adsm:message[text()='" + errorMessage + "']", serviceType: typeof(UriParserIntegrationTestServiceWithLowLimits));
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition2")]
        // [TestMethod]
        public void RequestShouldNotFailIfExpandCountMatchesLimit()
        {
            ResponseShouldMatchXPath(
                "/Entities?$expand=Reference($expand=Reference),Reference2",
                200,
                "//atom:feed", serviceType: typeof(UriParserIntegrationTestServiceWithLowLimits));
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition2")]
        // [TestMethod]
        public void RequestShouldFailIfExpandDepthIsHugeButNoLimitHasBeenSet()
        {
            string expand = "/Entities?";
            foreach (int i in Enumerable.Range(0,1000))
            {
                if (i < 999)
                {
                    expand += "$expand=Reference(";
                }
                else
                {
                    expand += "$expand=Reference";
                }
            }

            expand = Enumerable.Range(0, 999).Aggregate(expand, (current, i) => current + ")");

            ResponseShouldHaveStatusCode(expand, 400);
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition2")]
        // [TestMethod]
        public void RequestShouldNotFailIfExpandWidthIsHugeButNoLimitHasBeenSet()
        {
            ResponseShouldMatchXPath(
                "/Entities?$expand=Reference($expand=Reference),Reference2",
                200,
                "//atom:feed", serviceType: typeof(UriParserIntegrationTestServiceWithNoLimits));
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition2")]
        // [TestMethod]
        public void RequestShouldFailIfExpandContainsNonNavigationProperties()
        {
            ResponseShouldHaveStatusCode("/Entities?$expand=ID", 400);
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void RequestShouldFailIfCountHasKeyExpression()
        {
            ResponseShouldHaveStatusCode("/Entities/$count(ThisWillBeIgnored)", 400);
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void RequestShouldFailIfNamedStreamHasKeyExpression()
        {
            ResponseShouldHaveStatusCode("/Entities(0)/Stream(ThisWillBeIgnored)", 400);
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void RequestShouldFailIfVoidServiceOperationHasKeyExpression()
        {
            ResponseShouldHaveStatusCode("/GetNothing(ThisWillBeIgnored)", 400);
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void HiddenNavigationPropertyShouldBeTreatedSameAsNonExistentPropertyInSelect()
        {
            ResponsesShouldBeTheSame(
                "/Entities?$select=HiddenNavigation", 
                "/Entities?$select=DoesNotExist",
                400,
                p => p.Replace("DoesNotExist", "HiddenNavigation"),
                typeof (UriParserIntegrationTestServiceWithHiddenNavigation));
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void HiddenNavigationPropertyShouldBeTreatedSameAsNonExistentPropertyInExpand()
        {
            ResponsesShouldBeTheSame(
                "/Entities?$expand=HiddenNavigation",
                "/Entities?$expand=DoesNotExist",
                400,
                p => p.Replace("DoesNotExist", "HiddenNavigation"),
                typeof (UriParserIntegrationTestServiceWithHiddenNavigation));
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void HiddenNavigationPropertyShouldBeTreatedSameAsNonExistentPropertyInFilter()
        {
            ResponsesShouldBeTheSame(
                "/Entities?$filter=HiddenNavigation ne null",
                "/Entities?$filter=DoesNotExist ne null",
                400,
                p => p.Replace("DoesNotExist", "HiddenNavigation"),
                typeof (UriParserIntegrationTestServiceWithHiddenNavigation));
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void HiddenNavigationPropertyShouldBeTreatedSameAsNonExistentPropertyInOrderBy()
        {
            ResponsesShouldBeTheSame(
                "/Entities?$orderby=HiddenNavigation/ID",
                "/Entities?$orderby=DoesNotExist/ID",
                400,
                p => p.Replace("DoesNotExist", "HiddenNavigation"),
                typeof (UriParserIntegrationTestServiceWithHiddenNavigation));
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void HiddenNavigationPropertyShouldBeTreatedSameAsNonExistentPropertyInPath()
        {
            ResponsesShouldBeTheSame(
                "/Entities(0)/HiddenNavigation",
                "/Entities(0)/DoesNotExist",
                404,
                p => p.Replace("DoesNotExist", "HiddenNavigation"),
                typeof (UriParserIntegrationTestServiceWithHiddenNavigation));
        }

        [TestCategory("Partition2")]
        [TestMethod]
        public void HiddenNavigationPropertyShouldBeTreatedSameAsNonExistentPropertyInPathAfterLinks()
        {
            ResponsesShouldBeTheSame(
                "/Entities(0)/HiddenNavigation/$ref",
                "/Entities(0)/DoesNotExist/$ref",
                404,
                p => p.Replace("DoesNotExist", "HiddenNavigation"),
                typeof (UriParserIntegrationTestServiceWithHiddenNavigation));
        }
        [Ignore] // Remove Atom
        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
        // [TestCategory("Partition2")]
        // [TestMethod]
        public void XmlKeyLookupShouldWork()
        {
            var bytes = Encoding.UTF8.GetBytes("'<fake/>'");
            ResponseShouldMatchXPath(
                "/EntitiesWithXmlKeys(binary'" + Convert.ToBase64String(bytes, 0, bytes.Length) + "')", 
                200,
                "//atom:entry/atom:id[text()=\"http://host/EntitiesWithXmlKeys('%3Cfake%20%2F%3E')\"]");
        }
        [Ignore] // Remove Atom
        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
        // [TestCategory("Partition2")]
        // [TestMethod]
        public void XmlComparisonInFilterShouldNotWork()
        {
            ResponseShouldHaveStatusCode("/Entities?$filter=XmlProperty eq '<fake/>'", 400);
        }

        private static void ResponsesShouldBeTheSame(string baselineUri, string testUri, int statusCode, Func<string, string> prepareExpected, Type serviceType)
        {
            using (TestServiceHost.AllowServerToSerializeException.Restore())
            using (TestWebRequest baselineRequest = TestWebRequest.CreateForInProcess())
            using (TestWebRequest testRequest = TestWebRequest.CreateForInProcess())
            {
                TestServiceHost.AllowServerToSerializeException.Value = true;

                baselineRequest.DataServiceType = serviceType;
                testRequest.DataServiceType = serviceType;

                baselineRequest.RequestUriString = baselineUri;
                testRequest.RequestUriString = testUri;

                TestUtil.RunCatching(baselineRequest.SendRequest);
                TestUtil.RunCatching(testRequest.SendRequest);

                Assert.AreEqual(statusCode, testRequest.ResponseStatusCode);
                Assert.AreEqual(testRequest.ResponseStatusCode, baselineRequest.ResponseStatusCode);

                var baselinePayload = prepareExpected(testRequest.GetResponseStreamAsText());
                string actualPayload = baselineRequest.GetResponseStreamAsText();
                Assert.AreEqual(baselinePayload, actualPayload);
            }
        }

        private static void ResponseShouldMatchXPath(string requestUriString, int statusCode, string xpath, Type serviceType = null)
        {
            ResponseShouldHaveStatusCode(
                requestUriString,
                statusCode,
                request =>
                {
                    var responsePayload = request.GetResponseStreamAsXDocument();
                    UnitTestsUtil.VerifyXPaths(responsePayload, new[] { xpath });
                },
                serviceType);
        }

        private static void ResponseShouldHaveStatusCode(string requestUriString, int statusCode, Action<TestWebRequest> verifyResponse = null, Type serviceType = null)
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
                }, serviceType);
        }

        private static void Run(Action<TestWebRequest> runTest, Type serviceType = null)
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            using (TestUtil.RestoreStaticValueOnDispose(typeof(BaseTestWebRequest), "HostInterfaceType"))
            using (TestServiceHost.AllowServerToSerializeException.Restore())
            {
                TestServiceHost.AllowServerToSerializeException.Value = true;
                BaseTestWebRequest.HostInterfaceType = typeof(IDataServiceHost2);

                request.DataServiceType = serviceType ?? typeof(UriParserIntegrationTestServiceWithNoLimits);

                runTest(request);
            }
        }

        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class UriParserIntegrationTestServiceWithLowLimits : DataService<UriParserIntegrationContext>
        {
            public static void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
                config.UseVerboseErrors = true;
                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;

                config.MaxExpandDepth = 2;
                config.MaxExpandCount = 3;
            }
        }

        [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
        public class UriParserIntegrationTestServiceWithNoLimits : DataService<UriParserIntegrationContext>
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
            public void GetNothing()
            {
            }
        }

        public class UriParserIntegrationTestServiceWithHiddenNavigation : DataService<UriParserIntegrationContext>
        {
            public static void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("Entities", EntitySetRights.All);
                config.SetEntitySetAccessRule("HiddenEntities", EntitySetRights.None);
                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            }
        }

        public class UriParserIntegrationContext : IDataServiceStreamProvider2
        {
            public IQueryable<EntityTypeWithSelfReferencingNavigation> Entities
            {
                get
                {
                    return new List<EntityTypeWithSelfReferencingNavigation> 
                    { 
                        new EntityTypeWithSelfReferencingNavigation()
                    }.AsQueryable();
                }
            }

            public IQueryable<HiddenEntityType> HiddenEntities
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IQueryable<EntityWithXmlKey> EntitiesWithXmlKeys
            {
                get
                {
                    return new XmlValueComparingQueryable<EntityWithXmlKey>(new List<EntityWithXmlKey> 
                    { 
                        new EntityWithXmlKey { ID = XElement.Parse("<fake/>")}
                    }.AsQueryable());
                }
            }

            public int StreamBufferSize { get { return 1024; } }

            public Stream GetReadStream(object entity, string etag, bool? checkETagForEquality,
                                        DataServiceOperationContext operationContext)
            {
                return new MemoryStream();
            }

            public Stream GetWriteStream(object entity, string etag, bool? checkETagForEquality,
                                         DataServiceOperationContext operationContext)
            {
                throw new NotImplementedException();
            }

            public void DeleteStream(object entity, DataServiceOperationContext operationContext)
            {
                throw new NotImplementedException();
            }

            public string GetStreamContentType(object entity, DataServiceOperationContext operationContext)
            {
                return "text/plain";
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

            public Stream GetReadStream(object entity, Microsoft.OData.Service.Providers.ResourceProperty streamProperty, string etag, bool? checkETagForEquality,
                                        DataServiceOperationContext operationContext)
            {
                return new MemoryStream();
            }

            public Stream GetWriteStream(object entity, Microsoft.OData.Service.Providers.ResourceProperty streamProperty, string etag, bool? checkETagForEquality,
                                         DataServiceOperationContext operationContext)
            {
                throw new NotImplementedException();
            }

            public string GetStreamContentType(object entity, Microsoft.OData.Service.Providers.ResourceProperty streamProperty, DataServiceOperationContext operationContext)
            {
                return "text/plain";
            }

            public Uri GetReadStreamUri(object entity, Microsoft.OData.Service.Providers.ResourceProperty streamProperty, DataServiceOperationContext operationContext)
            {
                return null;
            }

            public string GetStreamETag(object entity, Microsoft.OData.Service.Providers.ResourceProperty streamProperty, DataServiceOperationContext operationContext)
            {
                return null;
            }
        }

        [NamedStream("Stream")]
        public class EntityTypeWithSelfReferencingNavigation
        {
            public int ID { get; set; }
            public EntityTypeWithSelfReferencingNavigation Reference { get; set; }
            public EntityTypeWithSelfReferencingNavigation Reference2 { get; set; }
            public EntityTypeWithSelfReferencingNavigation Reference3 { get; set; }
            public EntityTypeWithSelfReferencingNavigation Reference4 { get; set; }
            public HiddenEntityType HiddenNavigation { get; set; }
            public XElement XmlProperty { get; set; }
        }

        public class HiddenEntityType
        {
            public int ID { get; set; }
        }

        public class EntityWithXmlKey
        {
            public XElement ID { get; set; }
        }

        private class XmlValueComparingQueryable<T> : IOrderedQueryable<T>
        {
            private readonly IQueryable<T> realQueryable;

            public XmlValueComparingQueryable(IQueryable<T> realQueryable)
            {
                this.realQueryable = realQueryable;
                this.Provider = new XmlValueComparingQueryProvider(realQueryable.Provider);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this.realQueryable.Provider.CreateQuery<T>(new XmlValueComparingExpressionVisitor().Visit(this.Expression)).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public Expression Expression
            {
                get { return this.realQueryable.Expression; }
            }

            public Type ElementType
            {
                get { return this.realQueryable.ElementType; }
            }

            public IQueryProvider Provider { get; private set; }
        }

        private class XmlValueComparingQueryProvider : IQueryProvider
        {
            private readonly IQueryProvider realProvider;
            public XmlValueComparingQueryProvider(IQueryProvider realProvider)
            {
                this.realProvider = realProvider;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                throw new NotImplementedException();
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new XmlValueComparingQueryable<TElement>(this.realProvider.CreateQuery<TElement>(expression));
            }

            public object Execute(Expression expression)
            {
                return this.realProvider.Execute(new XmlValueComparingExpressionVisitor().Visit(expression));
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return this.realProvider.Execute<TResult>(new XmlValueComparingExpressionVisitor().Visit(expression));
            }
        }

        private class XmlValueComparingExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
        {
            private static readonly MethodInfo methodInfoForAreEqual = typeof(XmlValueComparingExpressionVisitor).GetMethod("AreEqual");

            protected override Expression VisitBinary(System.Linq.Expressions.BinaryExpression node)
            {
                if (node.NodeType == ExpressionType.Equal && node.Method == null && node.Left.Type == typeof(XElement))
                {
                    return Expression.Equal(this.Visit(node.Left), this.Visit(node.Right), node.IsLiftedToNull, methodInfoForAreEqual);
                }

                return base.Visit(node);
            }

            public static bool AreEqual(XElement x, XElement y)
            {
                return x.ToString() == y.ToString();
            }
        }
    }
}
