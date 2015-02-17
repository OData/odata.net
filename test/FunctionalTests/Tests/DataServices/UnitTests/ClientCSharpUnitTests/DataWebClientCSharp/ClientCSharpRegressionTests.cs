//---------------------------------------------------------------------
// <copyright file="ClientCSharpRegressionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Xml.Linq;
    using AstoriaUnitTests.ClientExtensions;
    using AstoriaUnitTests.DataWebClientCSharp;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using CustomDataClient = AstoriaUnitTests.Stubs.CustomDataClient;
    using DSClient = Microsoft.OData.Client;

    [TestClass]
    public class ClientCSharpRegressionTests
    {
        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        // RFC 3986
        // pct-encoded   = "%" HEXDIG HEXDIG
        // reserved      = gen-delims / sub-delims
        // gen-delims    = ":" / "/" / "?" / "#" / "[" / "]" / "@"
        // sub-delims    = "!" / "$" / "&" / "'" / "(" / ")"
        //                 / "*" / "+" / "," / ";" / "="

        static char[] restrictedChars = new char[] {  '%', ':', '/', '?', '#', 
                    '[', ']', '@', '=', '$', 
                    '&', ';', '(', ')', '*', 
                    '+', ',', '\'', '"', ' ',
                    '!', '\t', '\r', '\n' };

        [TestMethod]
        public void ClientLinqUriInjectionTest()
        {
            // LINQ Uri Security Injection Hack
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.StartService();

                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot);
                ctx.EnableAtom = true;
                ctx.Format.UseAtom();
                var userInput = "Customer 0' or true";
                var q = from c in ctx.CreateQuery<Customer>("Customers")
                        where c.Name == userInput
                        select c;

                var custs = q.ToList();
                Assert.AreEqual(0, custs.Count);
            }
        }

        [TestMethod]
        public void DeletePostTunnelingShouldWork()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.StartService();

                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot);
                ctx.EnableAtom = true;
                ctx.Format.UseAtom();
                ctx.Configurations.RequestPipeline.OnMessageCreating = (args)=> new HttpWebRequestMessage(args);
                    
                ctx.UsePostTunneling = true;
                var q = from c in ctx.CreateQuery<Product>("Products")
                        select c;

                var custs = q.ToList();

                ctx.DeleteObject(custs[0]);
                //var asyncResult = ctx.BeginSaveChanges(a =>
                //    {
                //        ctx.EndSaveChanges(a);
                //    }, ctx);

                ctx.SaveChanges();
         
                //asyncResult.AsyncWaitHandle.WaitOne();
            }
        }

        [TestMethod]
        public void ClientLinqUriWriterReservedCharacterTest()
        {
            // LINQ Uri reserved characters
            var operation = new string[] { "eq", "contains", "key", "tolower", "orderby" };

            DataServiceContext ctx = new DataServiceContext(new Uri("http://localhost/test.svc"));

            TestUtil.RunCombinations(restrictedChars, operation, (r, op) =>
            {
                string testValue = r + "value";

                IQueryable q = null;
                switch (op)
                {
                    case "eq": q = ctx.CreateQuery<Customer>("Customers").Where(c => c.Name == testValue); break;
                    case "contains": q = ctx.CreateQuery<Customer>("Customers").Where(c => c.Name.Contains(testValue)); break;
                    case "key": q = ctx.CreateQuery<northwindClient.Customers>("Customers").Where(c => c.CustomerID == testValue); break;
                    case "tolower": q = ctx.CreateQuery<Customer>("Customers").Where(c => c.Name.ToLower() == testValue.ToLower()); break;
                    case "orderby": q = ctx.CreateQuery<Customer>("Customers").OrderBy(c => testValue); break;
                }

                // q.ToString() unescapes some parts of the uri
                string uri = ((DataServiceRequest)q).RequestUri.OriginalString;
                Trace.WriteLine(uri);

                string baseline = Uri.EscapeDataString(testValue);
                TestUtil.AssertContains(uri, baseline);
                if (testValue != baseline)
                {
                    TestUtil.AssertContainsFalse(uri, testValue);
                }
                else
                {
                    // some characters are not escaped by Uri.EscapeDataString
                    TestUtil.AssertContains("'!()*", r.ToString());
                }
            });
        }

        [TestMethod]
        public void ClientLinqUriWriterTypeInjectionTests()
        {
            // Injection hack in type resolving system
            DataServiceContext ctx = new DataServiceContext(new Uri("http://localhost/test.svc"));
            ctx.ResolveName = (type) =>
                {
                    return "%:/?#[]@=$&;()*+,'\" !";
                };

            var q = ctx.CreateQuery<Customer>("Customers").Where(c => ((CustomerWithBirthday)c) is CustomerWithBirthday);
            string uri = ((DataServiceRequest)q).RequestUri.OriginalString;
            // check that we do not escape type strings:
            TestUtil.AssertContains(uri, "$filter=isof(cast('%:/?#[]@=$&;()*+,'\" !'), '%:/?#[]@=$&;()*+,'\" !')");
        }

        public class TestEntity1
        {
            public string ID { get; set; }
        }

        public class TestContext1
        {
            public IQueryable<TestEntity1> Entities
            {
                get
                {
                    return restrictedChars.AsQueryable().Select(c => new TestEntity1() { ID = "ID_" + c + "_ID" });
                }
            }
        }

        [TestMethod]
        public void EnsureClientAttachToGenerateSameIdentityAsServer()
        {
            // Ensure AttachTo on the client generates the same identity as the server
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(TestContext1);
                web.StartService();

                DataServiceContext ctx = new DataServiceContext(web.ServiceRoot);
                ctx.EnableAtom = true;
                ctx.Format.UseAtom();

                List<TestEntity1> entities = ctx.CreateQuery<TestEntity1>("Entities").ToList();

                foreach (TestEntity1 e in entities)
                {
                    EntityDescriptor originalDescriptor = ctx.GetEntityDescriptor(e);
                    ctx.Detach(e);
                    ctx.AttachTo("Entities", e);
                    EntityDescriptor newDescriptor = ctx.GetEntityDescriptor(e);

                    Assert.AreEqual(originalDescriptor.EditLink, newDescriptor.EditLink);
                    Assert.AreEqual(originalDescriptor.Identity, newDescriptor.Identity);
                }
            }
        }

        public class TestEntity2
        {
            public string ID { get; set; }
            public string Value { get; set; }
        }

        public class TestContext2 : IUpdatable
        {
            List<TestEntity2> entities = new List<TestEntity2>();

            public IQueryable<TestEntity2> Entities
            {
                get { return entities.AsQueryable(); }
            }

            #region IUpdatable Members

            public object CreateResource(string containerName, string fullTypeName)
            {
                TestEntity2 newEntity = new TestEntity2() { ID = this.entities.Count.ToString() };
                this.entities.Add(newEntity);
                return newEntity;
            }

            public object GetResource(IQueryable query, string fullTypeName)
            {
                if (typeof(Order).IsAssignableFrom(query.ElementType))
                {
                    return new Order();
                }
                else
                {
                    return null;
                }
            }

            public object ResetResource(object resource)
            {
                throw new NotImplementedException();
            }

            public void SetValue(object targetResource, string propertyName, object propertyValue)
            {
                TestEntity2 entity = targetResource as TestEntity2;
                switch (propertyName)
                {
                    case "ID": break;
                    case "Value": entity.Value = propertyValue.ToString(); break;
                }
            }

            public object GetValue(object targetResource, string propertyName)
            {
                throw new NotImplementedException();
            }

            public void SetReference(object targetResource, string propertyName, object propertyValue)
            {
                throw new NotImplementedException();
            }

            public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
            {
                throw new NotImplementedException();
            }

            public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
            {
                throw new NotImplementedException();
            }

            public void DeleteResource(object targetResource)
            {
                throw new NotImplementedException();
            }

            public void SaveChanges()
            {
            }

            public object ResolveResource(object resource)
            {
                return resource;
            }

            public void ClearChanges()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        [TestMethod]
        public void SendingEntityWithNullKey()
        {
            using (TestWebRequest service = TestWebRequest.CreateForInProcessWcf())
            {
                service.DataServiceType = typeof(TestContext2);
                service.StartService();

                var ctx = new DataServiceContext(service.ServiceRoot);
                ctx.EnableAtom = true;
                ctx.Format.UseAtom();
                TestEntity2 newEntity = new TestEntity2() { ID = null, Value = "Something" };

                ctx.AddObject("Entities", newEntity);
                ctx.SaveChanges();

                Assert.AreEqual(newEntity.ID, "0");
            }
        }

        [TestMethod]
        public void ClientServerPropertyTypeMismatchShouldThrow()
        {
            // Mismatch between the client property type and server type throws an exception
            #region payload
            string responsePayload =
@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<feed xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
<title type=""text"">Entities</title>
<updated>2010-01-27T18:06:26Z</updated>
<link rel=""self"" title=""Entities"" href=""Entities"" />
<entry>
    <id>http://localhost:62614/TheTest/Entities(16584)</id>
    <title type=""text""></title>
    <updated>2009-09-30T01:44:35Z</updated>
    <author>
        <name/>
    </author>
    <link rel=""edit"" title=""Order"" href=""Orders(16584)"" />
    <category term=""#AstoriaUnitTests.TestNamespace.A.MyEntity"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
    <content type=""application/xml"">
      <m:properties>
        <d:ID m:type=""Edm.Int32"">16584</d:ID>
        <d:Number m:type=""Edm.Double"">100.50</d:Number>
      </m:properties>
    </content>
</entry>
<entry>
    <id>http://localhost:62614/TheTest/Entities(165846)</id>
    <title type=""text""></title>
    <updated>2009-09-30T01:44:35Z</updated>
    <author>
        <name/>
    </author>
    <link rel=""edit"" title=""Order"" href=""Orders(16586)"" />
    <category term=""#AstoriaUnitTests.TestNamespace.B.MyEntity"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
    <content type=""application/xml"">
      <m:properties>
        <d:ID m:type=""Edm.Int32"">16584</d:ID>
        <d:Number m:type=""Edm.Double"">100.50</d:Number>
      </m:properties>
    </content>
</entry>
</feed>";
            #endregion payload

            var ctx = CreateContextWithTransportLayer(responsePayload);
            ctx.EnableAtom = true;

            try
            {
                var entities = ctx.Execute<TestType3>(new Uri("/Entities", UriKind.Relative)).ToList();
                Assert.Fail("expecting an exception, but no exception was encountered");
            }
            catch (InvalidOperationException e)
            {
                // expecting an argument exception, and no assert.
                Assert.AreEqual(ODataLibResourceUtil.GetString("ValidationUtils_IncorrectTypeKind", "Edm.Double", "Complex", "Primitive"), e.Message);
            }
        }

        public class TestType3
        {
            public int ID { get; set; }
            public SafeDouble Number { get; set; }
        }

        public class SafeDouble
        {
            public double Value { get; set; }
            public bool IsNull { get; set; }

            public SafeDouble(double d)
            {
                this.Value = d;
                this.IsNull = false;
            }

            public SafeDouble(double? d)
            {
                this.IsNull = d.HasValue;
                if (d.HasValue)
                {
                    this.Value = d.Value;
                }
            }
        }

        [TestMethod]
        public void ALinqKeyPropertyPredicate()
        {
            // Client Linq : Incorrect Filter expression generated when filtering Reference property of the same type
            var ctx = new northwindClient.northwindContext(new Uri("http://localhost/TheTest"));
            TestUtil.RunCombinations(new[] { 
                new { query = ctx.Employees.Where(e => e.Employees2.EmployeeID == 2), uri = "Employees?$filter=Employees2/EmployeeID eq 2" },
                new { query = ctx.Employees.Where(e => e.EmployeeID == 2), uri = "Employees(2)" },
                new { query = from c in ctx.Employees where c.EmployeeID == 2 from e in c.Employees1 where e.Employees2.EmployeeID == 1 select e, uri = "Employees(2)/Employees1?$filter=Employees2/EmployeeID eq 1" },
                new { query = from c in ctx.Employees where c.EmployeeID == 2 from e in c.Employees1 where e.EmployeeID == 1 select e, uri = "Employees(2)/Employees1(1)" }
            }, value =>
            {
                string actual = value.query.ToString();
                Assert.AreEqual("http://localhost/TheTest/" + value.uri, actual);
            });
        }

        [TestMethod]
        public void SelectManyInvalidProjectionWithSelfReferencingType_CSharp()
        {
            // Projecting entity that is not in scope with self-referencing type (CSharp)
            // All of these queries should fail with the same NotSupportedException because they are
            // attempting to project with an iterator that is outside of the current scope.
            // Other tests in AstoriaUnitTests.Tests.LinqTests cover the cases where the types are different.

            // The VB version of this test is in ClientRegressionTests.SelectManyInvalidProjectionWithSelfReferencingType_VB
            // The queries are the same, but VB handles SelectMany with a slightly different expression tree, so make sure we can handle both

            // Note that in order to produce the kind of expression tree that caused the bug, the query must reference
            // the inner query iterator. Since the purpose of this test is to explicitly *not* use that iterator in the
            // projection, that means the query needs to have some other clause that reference the iterator
            // The queries use orderby and where to achieve this, but there is no additional significance to which operator is used in a given query.

            DataServiceContext context = new DataServiceContext(new Uri("http://invalidhost"));
            var baseQuery = context.CreateQuery<SelfReferenceTypedEntity<int, int>>("EntitySet");

            IQueryable[] testQueries = new IQueryable[] {

                from outer in baseQuery
                where outer.ID == 1
                from inner in outer.Collection
                orderby inner.ID
                select outer,

                from outer in baseQuery
                where outer.ID == 1
                from inner in outer.Collection
                where inner.ID == 2
                select outer.Member,

                from outer in baseQuery
                where outer.ID == 1
                from inner in outer.Collection
                orderby inner.ID
                select outer.Reference,

                from outer in baseQuery
                where outer.ID == 1
                from inner in outer.Collection
                where inner.ID == 2
                select outer.Reference.Member,

                from outer in baseQuery
                where outer.ID == 1
                from inner in outer.Collection
                orderby inner.ID
                select new { outer },

                from outer in baseQuery
                where outer.ID == 1
                from inner in outer.Collection
                where inner.ID == 2
                select new { outer.Member },

                from outer in baseQuery
                where outer.ID == 1
                from inner in outer.Collection
                orderby inner.ID
                select new { outer.Reference },

                from outer in baseQuery
                where outer.ID == 1
                from inner in outer.Collection
                where inner.ID == 2
                select new { outer.Reference.Member },

                from outer in baseQuery
                where outer.ID == 1
                from inner in outer.Collection
                orderby inner.ID
                select new { outer.Member, outer.ID, Nested = outer.Reference.Reference.Member },

                from outer in baseQuery
                where outer.ID == 1
                from firstInner in outer.Collection
                where firstInner.ID == 1
                from secondInner in firstInner.DSC
                orderby secondInner.ID
                select new { firstInner },

                from outer in baseQuery
                where outer.ID == 1
                from firstInner in outer.Collection
                where firstInner.ID == 1
                from secondInner in firstInner.DSC
                where secondInner.ID == 2
                select new { firstInner },

                from outer in baseQuery
                where outer.ID == 1
                from firstInner in outer.Collection
                where firstInner.ID == 1
                from secondInner in firstInner.DSC
                orderby secondInner.ID
                select new { firstInner.Member, secondInner },
            };

            TestUtil.RunCombinations(testQueries, (testQuery) =>
                {
                    try
                    {
                        testQuery.GetEnumerator();
                        Assert.Fail("Expected NotSupportedException for query " + testQuery);
                    }
                    catch (NotSupportedException nse)
                    {
                        Assert.AreEqual(DataServicesClientResourceUtil.GetString("ALinq_CanOnlyProjectTheLeaf"), nse.Message);
                    }
                });
        }

        [TestMethod]
        public void RequestDataFailedToCreateWhenSaveChangeOptionsContinueOnErrorSet()
        {
            // Assert when SaveChangeOptions.ContinueOnError is set and part of the request data failed to create
            TestUtil.RunCombinations(new SaveChangesOptions[] { SaveChangesOptions.None, SaveChangesOptions.ContinueOnError },
            (options) =>
            {
                DataServiceContext context = new DataServiceContext(new Uri("http://localhost/TheTest.svc"), ODataProtocolVersion.V4);
                context.AddAndUpdateResponsePreference = DataServiceResponsePreference.NoContent;

                Customer c = new Customer() { ID = 1, Name = "Foo" };
                context.AttachTo("Customers", c);
                MemoryStream stream = new MemoryStream(new byte[] { 0, 1, 2, 3 });
                context.SetSaveStream(c, stream, true, new DataServiceRequestArgs() { ContentType = "image/bmp" });

                DataServiceRequestException ex = TestUtil.RunCatching<DataServiceRequestException>(() => { context.SaveChanges(options); });
                TestUtil.AssertExceptionExpected(ex, true);
                InvalidOperationException innerEx = ex.InnerException as InvalidOperationException;
                Assert.IsNotNull(innerEx, "Expected invalid operation exception in the inner exception");
                Assert.AreEqual(DataServicesClientResourceUtil.GetString("Context_SetSaveStreamWithoutEditMediaLink"), innerEx.Message);
            });
        }

        [TestMethod]
        public void ALinqShouldThrowWhenConstantExpressionInconvertableToString()
        {
            // Alinq translator should throw better exception when constant expression are not convertable to string, but outer expression is also incorrect.
            var ctx = new DataServiceContext(new Uri("http://localhost/service.svc"));
            ctx.EnableAtom = true;
            ctx.Format.UseAtom();
            var q = from c in ctx.CreateQuery<Customer>("Customers")
                    where c.GetType() == typeof(Customer)
                    select c;

            NotSupportedException ex = TestUtil.RunCatching(() => q.FirstOrDefault()) as NotSupportedException;
            Assert.IsNotNull(ex);
            // the expression .GetType() == EpmCustomer is not supported is a better message than "Cannot convert EpmCustomer to string"
            TestUtil.AssertContains(ex.Message, "GetType()");
            TestUtil.AssertContains(ex.Message, "is not supported");

            q = from c in ctx.CreateQuery<Customer>("Customers")
                where c.Address == new Address()
                select c;

            NotSupportedException ex1 = TestUtil.RunCatching(() => q.FirstOrDefault()) as NotSupportedException;
            Assert.IsNotNull(ex1);
            Assert.AreEqual(DataServicesClientResourceUtil.GetString("ALinq_CouldNotConvert", "AstoriaUnitTests.Stubs.Address"), ex1.Message);
        }

        [HasStream]
        internal class CustomerWithStream : Customer
        {
        }

        [TestMethod]
        public void ClientReadEtagOnlyIfStatusCode201()
        {
            // Client reads the etag response header in POST MR, only if the status code is 201 (Created)
            TestUtil.RunCombinations((IEnumerable<SaveChangesMode>)Enum.GetValues(typeof(SaveChangesMode)), UnitTestsUtil.BooleanValues, (mode, closeStream) =>
            {
                var requestMessage = new ODataTestMessage();
                Func<IODataRequestMessage> getRequestMessage = () => requestMessage;

                int i = 0;
                Func<IODataResponseMessage> getResponseMessage = () =>
                {
                    var responseMessage = new ODataTestMessage();
                    if (i == 1)
                    {
                        // Verify the second request was a PATCH request to the MLE
                        Assert.AreEqual(requestMessage.Method, "PATCH", "The second request must be a PATCH request");
                        Assert.AreEqual(requestMessage.Url, DataServiceContextWithCustomTransportLayer.DummyUri.AbsoluteUri + "/location/Customers(1)", "The request uri of the second request must be the value of the location header");
                        Assert.AreEqual(requestMessage.GetHeader("If-Match"), "W/\"randometag\"", "the If-Match header must be specified in the second request");
                        responseMessage.StatusCode = 200;
                    }
                    else if (i == 0)
                    {
                        // Verify the first request was a POST request
                        Assert.AreEqual(requestMessage.Method, "POST", "The first request must be a POST request");
                        Assert.AreEqual(requestMessage.Url, DataServiceContextWithCustomTransportLayer.DummyUri.AbsoluteUri + "/Customers", "The first request must be a POST to customers");

                        responseMessage.SetHeader("Location", DataServiceContextWithCustomTransportLayer.DummyUri.AbsoluteUri + "/location/Customers(1)");
                        responseMessage.SetHeader("OData-EntityId", "http://idservice/identity");
                        responseMessage.SetHeader("ETag", "W/\"randometag\"");
                        responseMessage.StatusCode = 200;
                    }

                    i++;
                    return responseMessage;
                };

                var context = new DataServiceContextWithCustomTransportLayer(ODataProtocolVersion.V4, getRequestMessage, getResponseMessage);
                context.AddAndUpdateResponsePreference = DataServiceResponsePreference.NoContent;

                CustomerWithStream c = new CustomerWithStream() {ID = 1, Name = "Foo"};
                context.AddObject("Customers", c);
                MemoryStream stream = new MemoryStream(new byte[] {0, 1, 2, 3});
                context.SetSaveStream(c, stream, closeStream, new DataServiceRequestArgs() {ContentType = "image/bmp"});

                DataServiceContextTestUtil.SaveChanges(context, SaveChangesOptions.None, mode);
                Assert.AreEqual(i, 2, "Only 2 request should have been made");
            });
        }

        #region Assert when SetSaveStream on multiple objects

        [HasStream]
        public class TestEntityType4
        {
            public int ID { get; set; }
            public String Description { get; set; }
            public List<TestEntityType5> Entities { get; set; }

            public TestEntityType4()
            {
                Entities = new List<TestEntityType5>();
            }
        }

        [HasStream]
        public class TestEntityType5
        {
            public int ID { get; set; }
            public String Description { get; set; }
            public TestEntityType4 Entity { get; set; }
        }

        public class TestEntityType6 : DSPResource
        {
            public TestEntityType6(Microsoft.OData.Service.Providers.ResourceType type)
                : base(type)
            {
                this.SetValue("Entities", new List<DSPResource>());
            }
        }

        [TestMethod]
        public void SetSaveStreamOnMultipleObjects()
        {
            // Assert when SetSaveStream on multiple objects
            // Service Definition:
            DSPMetadata metadata = new DSPMetadata("ClientCSharpRegressionTests", "AstoriaUnitTests.Tests");
            var entityType1 = metadata.AddEntityType("TestEntityType4", typeof(TestEntityType6), null, false);
            var entityType2 = metadata.AddEntityType("TestEntityType5", null, null, false);
            entityType1.IsMediaLinkEntry = true;
            entityType2.IsMediaLinkEntry = true;
            metadata.AddKeyProperty(entityType1, "ID", typeof(int));
            metadata.AddKeyProperty(entityType2, "ID", typeof(int));
            metadata.AddPrimitiveProperty(entityType1, "Description", typeof(string));
            metadata.AddPrimitiveProperty(entityType2, "Description", typeof(string));
            var set1 = metadata.AddResourceSet("EntitySet1", entityType1);
            var set2 = metadata.AddResourceSet("EntitySet2", entityType2);
            metadata.AddResourceSetReferenceProperty(entityType1, "Entities", set2, entityType2);
            metadata.AddResourceReferenceProperty(entityType2, "Entity", set1, entityType1);

            DSPServiceDefinition service = new DSPServiceDefinition()
            {
                Metadata = metadata,
                Writable = true,
                SupportMediaResource = true,
                MediaResourceStorage = new DSPMediaResourceStorage()
            };

            DSPContext data = new DSPContext();
            service.CreateDataSource = (m) => { return data; };

            using (TestWebRequest request = service.CreateForInProcessWcf())
            {
                request.StartService();
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                context.AddAndUpdateResponsePreference = DataServiceResponsePreference.NoContent;

                TestEntityType4 entity1 = new TestEntityType4() { ID = 0 };
                TestEntityType5 entity2 = new TestEntityType5() { ID = 1 };

                context.AddObject("EntitySet1", entity1);
                context.AddRelatedObject(entity1, "Entities", entity2);

                MemoryStream stream1 = new MemoryStream(new byte[] { 0, 1, 2, 3 });
                context.SetSaveStream(entity1, stream1, true, new DataServiceRequestArgs() { ContentType = "image/bmp", Slug = "0" });

                MemoryStream stream2 = new MemoryStream(new byte[] { 0, 1, 2, 3 });
                context.SetSaveStream(entity2, stream2, true, new DataServiceRequestArgs() { ContentType = "image/bmp", Slug = "1" });

                context.SaveChanges();

                Assert.AreEqual(2, context.Entities.Count);
                Assert.AreEqual(1, context.Links.Count);
            }
        }

        #endregion

        public class TestService4 : DataService<CustomDataContext>
        {
            public static int ChangeCounter = 0;

            public static void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
                config.SetEntitySetAccessRule("Products", EntitySetRights.All);
                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            }

            [ChangeInterceptor("Products")]
            public void ProductChanged(Product p, UpdateOperations o)
            {
                ChangeCounter++;
            }
        }

        [TestMethod]
        public void SaveChangeOptionsContinueOnErrorShouldWork()
        {
            // Make sure SaveChangeOptions.ContinueOnError is working as we expected, and when we don't continue on error, we fail as expected
            using (TestWebRequest service = TestWebRequest.CreateForInProcessWcf())
            {
                service.ServiceType = typeof(TestService4);
                service.StartService();

                TestUtil.RunCombinations(
                    // Dimension1: all save change option values
                    (IEnumerable<SaveChangesOptions>)Enum.GetValues(typeof(SaveChangesOptions)),
                    (IEnumerable<SaveChangesMode>)Enum.GetValues(typeof(SaveChangesMode)),
                    UnitTestsUtil.BooleanValues,
                    (option, mode, clientSideError) =>
                    {

                        DataServiceContext ctx = new DataServiceContext(service.ServiceRoot);
                        ctx.EnableAtom = true;
                        ctx.Format.UseAtom();

                        ctx.AddObject("Products", new CustomDataClient.Product() { ID = 1000 });
                        if (clientSideError)
                        {
                            // to generate a client side error, we attach an entity and set its MLE
                            // if we support this scenario in the future, replace this with anything that throws in the CreateNextRequest() path
                            CustomDataClient.Product p = new CustomDataClient.Product() { ID = 0 };
                            ctx.AttachTo("Products", p);
                            ctx.SetSaveStream(p, new MemoryStream(new byte[] { 0 }), true, new DataServiceRequestArgs() { ContentType = "image/bmp" });
                        }
                        else
                        {
                            ctx.AddObject("Orders", new CustomDataClient.Order() { ID = 1010 });
                        }
                        ctx.AddObject("Products", new CustomDataClient.Product() { ID = 1001 });

                        TestService4.ChangeCounter = 0;
                        Exception ex = TestUtil.RunCatching(() => DataServiceContextTestUtil.SaveChanges(ctx, option, mode));
                        Assert.IsNotNull(ex);

                        if (clientSideError)
                        {
                            // assert that the error is actually on the client side:
                            while (ex.InnerException != null)
                            {
                                ex = ex.InnerException;
                            }

                            // batch and MLE = not supported, rest = IOE because no edit link
                            Assert.AreEqual(ex.GetType(), (option == SaveChangesOptions.BatchWithSingleChangeset || option == SaveChangesOptions.BatchWithIndependentOperations) ? typeof(NotSupportedException) : typeof(InvalidOperationException));
                        }

                        int expectedCallCount;
                        if (option == SaveChangesOptions.BatchWithSingleChangeset) expectedCallCount = clientSideError ? 0 : 1;    // batch always fails as a whole
                        else if (option == SaveChangesOptions.BatchWithIndependentOperations) expectedCallCount = clientSideError ? 0 : 2;    // Independent  batch always fails as a whole
                        else if (option == SaveChangesOptions.PostOnlySetProperties) expectedCallCount = 0;
                        else if (option == SaveChangesOptions.ContinueOnError || (mode != SaveChangesMode.Synchronous && !clientSideError)) expectedCallCount = 2;  // V2 behavior: async code path always continue on server-error
                        else expectedCallCount = 1;

                        Assert.AreEqual(expectedCallCount, TestService4.ChangeCounter);
                    });
            }
        }

        public class EntityTypeWithVaryingSizePaylod
        {
            public int ID { get; set; }
            public byte[] Bin { get; set; }
        }

        [TestMethod]
        public void AsyncWithLargePayload()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                // 100 iterations takes roughly 30 seconds on my VM.
                for (int i = 0; i < 100; i++)
                {
                    AsyncWithVaryingSizePayload_Inner(request);
                }
            }
        }

        private void AsyncWithVaryingSizePayload_Inner(TestWebRequest request)
        {
            TypedCustomDataContext<EntityTypeWithVaryingSizePaylod>.ClearData();
            TypedCustomDataContext<EntityTypeWithVaryingSizePaylod>.ClearHandlers();
            TypedCustomDataContext<EntityTypeWithVaryingSizePaylod>.ClearValues();
            TypedCustomDataContext<EntityTypeWithVaryingSizePaylod>.CreateChangeScope();

            request.DataServiceType = typeof(TypedCustomDataContext<EntityTypeWithVaryingSizePaylod>);
            request.StartService();

            DataServiceContext ctx = new DataServiceContext(request.ServiceRoot);
            ctx.EnableAtom = true;
            ctx.Format.UseAtom();
            
            Random rand = new Random(DateTime.Now.Millisecond);
            int entityCount = 20;
            for (int id = 1; id <= entityCount; id++)
            {
                EntityTypeWithVaryingSizePaylod entity = new EntityTypeWithVaryingSizePaylod()
                {
                    ID = id,
                    Bin = new byte[rand.Next(0, 100000)],
                };

                rand.NextBytes(entity.Bin);
                ctx.AddObject("Values", entity);
            }

            System.Threading.AutoResetEvent waitHandle = new System.Threading.AutoResetEvent(false);
            Exception error = null;
            ctx.BeginSaveChanges(
                SaveChangesOptions.None,
                (ar) =>
                {
                    try
                    {
                        ctx.EndSaveChanges(ar);
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                },
                null);

            waitHandle.WaitOne();
            if (error != null)
            {
                throw error;
            }

            Assert.AreEqual(entityCount, TypedCustomDataContext<EntityTypeWithVaryingSizePaylod>.CurrentValues.Count());
            Assert.AreEqual(entityCount, ctx.Entities.Count());
            foreach (EntityDescriptor descriptor in ctx.Entities)
            {
                EntityTypeWithVaryingSizePaylod clientEntity = (EntityTypeWithVaryingSizePaylod)descriptor.Entity;
                EntityTypeWithVaryingSizePaylod serverEntity = TypedCustomDataContext<EntityTypeWithVaryingSizePaylod>.CurrentValues.Single(e => e.ID == clientEntity.ID);
                Assert.IsTrue(TestUtil.CompareStream(new MemoryStream(clientEntity.Bin), new MemoryStream(serverEntity.Bin)));
            }
        }

        [TestMethod]
        public void EmptyFeedWithCollectionQueryShouldNotLoopInfinite()
        {
#region payload
            string payload =
@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<feed xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
<title type=""text"">Entities</title>
<updated>2010-01-27T18:06:26Z</updated>
<link rel=""self"" title=""Entities"" href=""Entities"" />
</feed>";
#endregion payload
            // Empty feed with collection query resulting infinite loop
            var requestMessage = new ODataTestMessage();
            var responseMessage = new ODataTestMessage();
            responseMessage.StatusCode = 200;
            responseMessage.SetHeader("Content-Type", "application/atom+xml");
            responseMessage.SetHeader("Content-ID", "1");
            responseMessage.WriteToStream(payload);
            var ctx = new DataServiceContextWithCustomTransportLayer(requestMessage, responseMessage);
            ctx.EnableAtom = true;
            ctx.Format.UseAtom();

            int moveCount = 0;
            foreach (var pList in ctx.CreateQuery<ICollection<Product>>("Entities"))
            {
                Assert.AreEqual(0, pList.Count);
                Assert.AreEqual(0, moveCount++);    // only move once
            }
        }

        [TestMethod]
        public void Assert_TypeResolver_Not_Invoked_On_LoadProperty_Complex()
        {
            string payload =
@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<m:value m:type=""Address"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
<StreetAddress>98007</StreetAddress>
<City>Bellevue</City>
<State>98007</State>
<CountryRegion>USA</CountryRegion>
<PostalCode>Bellevue</PostalCode>
</m:value>";
            var responseMessage = new ODataTestMessage();
            responseMessage.StatusCode = 200;
            responseMessage.SetHeader("Content-Type", UnitTestsUtil.MimeApplicationXml);
            responseMessage.SetHeader("OData-Version", "4.0;");
            responseMessage.WriteToStream(payload);

            // Regression test for "Type resolver not invoked for complex types when calling LoadProperty"
            // This unit test ensures that this behavior doesn't break in a future release.
            var ctx = new DataServiceContextWithCustomTransportLayer(new ODataTestMessage(), responseMessage);
            ctx.EnableAtom = true;
            ctx.Format.UseAtom();

            bool wasResolveTypeCalled = false;
            ctx.ResolveType = (typeName) =>
            {
                wasResolveTypeCalled = true;
                return typeName == "Address" ? typeof(Address) : null;
            };

            Customer entity = new Customer() { ID = 1 };
            ctx.AttachTo("Customers", entity);
            ctx.LoadProperty(entity, "Address");

            Assert.IsTrue(wasResolveTypeCalled, "ResolveType should be called");
        }

        [TestMethod]
        public void Assert_TypeResolver_Not_Invoked_On_BeginLoadProperty_Complex()
        {
            // Regression test for "Type resolver not invoked for complex types when calling LoadProperty"
            // This unit test ensures that this behavior doesn't break in a future release.
            string payload =
@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<m:value m:type=""Address"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
<StreetAddress>98007</StreetAddress>
<City>Bellevue</City>
<State>98007</State>
<CountryRegion>USA</CountryRegion>
<PostalCode>Bellevue</PostalCode>
</m:value>";
            var responseMessage = new ODataTestMessage();
            responseMessage.StatusCode = 200;
            responseMessage.SetHeader("Content-Type", UnitTestsUtil.MimeApplicationXml);
            responseMessage.SetHeader("OData-Version", "4.0;");
            responseMessage.WriteToStream(payload);

            // Regression test for "Type resolver not invoked for complex types when calling LoadProperty"
            // This unit test ensures that this behavior doesn't break in a future release.
            var ctx = new DataServiceContextWithCustomTransportLayer(new ODataTestMessage(), responseMessage);
            ctx.EnableAtom = true;
            ctx.Format.UseAtom();

            bool wasResolveTypeCalled = false;
            ctx.ResolveType = (typeName) =>
            {
                wasResolveTypeCalled = true;
                return typeName == "Address" ? typeof(Address) : null;
            };

            AutoResetEvent waitHandle = new AutoResetEvent(false);

            Customer entity = new Customer() { ID = 1 };
            ctx.AttachTo("Customers", entity);
            ctx.BeginLoadProperty(
                entity,
                "Address",
                (loadPropertyResult) =>
                {
                    ctx.EndLoadProperty(loadPropertyResult);
                    waitHandle.Set();
                }, null);

            waitHandle.WaitOne();

            Assert.IsTrue(wasResolveTypeCalled, "ResolveType should be called");
        }

        public class TestService5 : DataService<CustomDataContext>
        {
            public static void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            }

            [ChangeInterceptor("Orders")]
            public void OrderChanged(Order o, UpdateOperations operation)
            {
                o.ID = 10;
            }
        }

        [TestMethod]
        public void ClientShouldCheckIdOrEditLinks()
        {
            // Client does not check IDs or Edit links from the server against IDs it is already tracking
            using (TestWebRequest service = TestWebRequest.CreateForInProcessWcf())
            {
                service.ServiceType = typeof(TestService5);
                service.StartService();

                TestUtil.RunCombinations(UnitTestsUtil.BooleanValues, (useAttach) =>
                    {
                        var ctx = new DataServiceContext(service.ServiceRoot);
                        ctx.EnableAtom = true;
                        ctx.Format.UseAtom();

                        var o1 = new Order() { ID = 10 };
                        if (useAttach)
                        {
                            ctx.AttachTo("Orders", o1);
                        }
                        else
                        {
                            ctx.AddObject("Orders", o1);
                        }

                        var o2 = new Order() { ID = 11 };
                        ctx.AddObject("Orders", o2);

                        Exception ex = TestUtil.RunCatching(() => ctx.SaveChanges());
                        Assert.AreEqual(DataServicesClientResourceUtil.GetString("Context_DifferentEntityAlreadyContained"), ex.Message);
                    });
            }
        }

        [TestMethod]
        public void FailuresWithEntityInitializationWithTypecastProjections()
        {
            // This tests a group of queries that initialize an entity as part of the projection of a subquery, where type conversions are involved
            //
            // The test cases cover:
            //      (1) Putting the type conversion at various places in the expression used to initialize the entity being created by the projection
            //      (2) Projecting various levels of the entity, i.e. entity.Property, entity.ReferenceMember.Property, entity.ReferenceMember.ReferenceMember.Property, etc.
            //
            // The baselineQuery is expected to be identical to the test query, but with the type conversions removed. The goal for this test is not to test those particular queries
            // but to make sure that the queries with the conversions behave the same as the queries without the conversions.

            // Initialization of an entity as part of the projection of a subquery, with type conversions in the projected property accessors
            var testCases = new TestCase[]
            {
                new TestCase
                {
                    // Cast: Top-level entity in subquery
                    // Projected Properties: Top-level entity in subquery
                    testQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    ID = (f as Person).ID,
                                    Name = (f as Person).Name,
                                    BestFriend = (f as Person).BestFriend,
                                    Address = (f as Person).Address
                                })
                        }),

                    baselineQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    ID = f.ID,
                                    Name = f.Name,
                                    BestFriend = f.BestFriend,
                                    Address = f.Address
                                })
                        }),
                },
                new TestCase
                {
                    // Cast: Top-level entity in subequery
                    // Projected Properties: Reference property one level down from top-level entity
                    testQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    ID = (f as Person).BestFriend.ID,
                                    Name = (f as Person).BestFriend.Name,
                                    BestFriend = (f as Person).BestFriend.BestFriend,
                                    Address = (f as Person).BestFriend.Address
                                })
                        }),

                    baselineQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    ID = f.BestFriend.ID,
                                    Name = f.BestFriend.Name,
                                    BestFriend = f.BestFriend.BestFriend,
                                    Address = f.BestFriend.Address
                                })
                        }),
                },
                new TestCase
                {
                    // Cast: Various levels from top-level entity, not the same on all projected properties
                    // Projected Properties: Reference property one level down from top-level entity
                    testQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    ID = f.BestFriend.ID,
                                    Name = (f as Person).BestFriend.Name,
                                    BestFriend = (f.BestFriend as Person).BestFriend,
                                    Address = (f.BestFriend as Person).Address
                                })
                        }),

                    baselineQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    ID = f.BestFriend.ID,
                                    Name = f.BestFriend.Name,
                                    BestFriend = f.BestFriend.BestFriend,
                                    Address = f.BestFriend.Address
                                })
                        }),
                },
                new TestCase
                {
                    // Cast: Top-level entity in subequery
                    // Projected Properties: Reference property three levels down from top-level entity
                    testQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    ID = (f as Person).BestFriend.BestFriend.BestFriend.ID,
                                    Name = (f as Person).BestFriend.BestFriend.BestFriend.Name,
                                    BestFriend = (f as Person).BestFriend.BestFriend.BestFriend.BestFriend,
                                    Address = (f as Person).BestFriend.BestFriend.BestFriend.Address
                                })
                        }),

                    baselineQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    ID = f.BestFriend.BestFriend.BestFriend.ID,
                                    Name = f.BestFriend.BestFriend.BestFriend.Name,
                                    BestFriend = f.BestFriend.BestFriend.BestFriend.BestFriend,
                                    Address = f.BestFriend.BestFriend.BestFriend.Address
                                })
                        }),
                },
                new TestCase
                {
                    // Cast: Reference property two levels down from top-level entity in subequery
                    // Projected Properties: Reference property three levels down from top-level entity
                    testQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    // leaving ID with no casting because when all had casting this was working even before the bug fix
                                    // but with no casting on the first property, it was failing
                                    ID = f.BestFriend.BestFriend.BestFriend.ID,
                                    Name = (f.BestFriend.BestFriend as Person).BestFriend.Name,
                                    BestFriend = (f.BestFriend.BestFriend as Person).BestFriend.BestFriend,
                                    Address = (f.BestFriend.BestFriend as Person).BestFriend.Address
                                })
                        }),

                    baselineQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    ID = f.BestFriend.BestFriend.BestFriend.ID,
                                    Name = f.BestFriend.BestFriend.BestFriend.Name,
                                    BestFriend = f.BestFriend.BestFriend.BestFriend.BestFriend,
                                    Address = f.BestFriend.BestFriend.BestFriend.Address
                                })
                        }),
                },
                new TestCase
                {
                    // Cast: Reference property two levels down from top-level entity in subequery and also reference property three levels down
                    // Projected Properties: Reference property three levels down from top-level entity
                    testQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    // leaving ID with no casting because when all had casting this was working even before the bug fix
                                    // but with no casting on the first property, it was failing
                                    ID = f.BestFriend.BestFriend.BestFriend.ID,
                                    Name = ((f.BestFriend.BestFriend as Person).BestFriend as Person).Name,
                                    BestFriend = ((f.BestFriend.BestFriend as Person).BestFriend as Person).BestFriend,
                                    Address = ((f.BestFriend.BestFriend as Person).BestFriend as Person).Address

                                })
                        }),

                    baselineQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    ID = f.BestFriend.BestFriend.BestFriend.ID,
                                    Name = f.BestFriend.BestFriend.BestFriend.Name,
                                    BestFriend = f.BestFriend.BestFriend.BestFriend.BestFriend,
                                    Address = f.BestFriend.BestFriend.BestFriend.Address
                                })
                        }),
                },
                new TestCase
                {
                    // Cast: Top-level entity in subquery
                    // Projected Properties: For outer entity, properties are on top-level entity in subquery
                    //                       For nested entity they are on the reference property one level down
                    testQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    ID = (f as Person).ID,
                                    Name = (f as Person).Name,
                                    BestFriend = new Person()
                                    {
                                        ID = (f as Person).BestFriend.ID,
                                        Name = (f as Person).BestFriend.Name,
                                        BestFriend = (f as Person).BestFriend.BestFriend,
                                        Address = (f as Person).BestFriend.Address
                                    }
                                })
                        }),

                    baselineQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    ID = f.ID,
                                    Name = f.Name,
                                    BestFriend = new Person()
                                    {
                                        ID = f.BestFriend.ID,
                                        Name = f.BestFriend.Name,
                                        BestFriend = f.BestFriend.BestFriend,
                                        Address = f.BestFriend.Address
                                    }
                                })
                        }),
                },
                new TestCase
                {
                    // Cast: Top-level entity in subquery
                    // Projected Properties: For outer entity, properties are on reference property two levels down
                    //                       For nested entity they are on the reference property three levels down
                    testQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    ID = (f as Person).BestFriend.BestFriend.ID,
                                    Name = (f as Person).BestFriend.BestFriend.Name,
                                    BestFriend = new Person()
                                    {
                                        ID = (f as Person).BestFriend.BestFriend.BestFriend.ID,
                                        Name = (f as Person).BestFriend.BestFriend.BestFriend.Name,
                                        BestFriend = (f as Person).BestFriend.BestFriend.BestFriend.BestFriend,
                                        Address = (f as Person).BestFriend.BestFriend.BestFriend.Address
                                    }
                                })
                        }),

                    baselineQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    ID = f.BestFriend.BestFriend.ID,
                                    Name = f.BestFriend.BestFriend.Name,
                                    BestFriend = new Person()
                                    {
                                        ID = f.BestFriend.BestFriend.BestFriend.ID,
                                        Name = f.BestFriend.BestFriend.BestFriend.Name,
                                        BestFriend = f.BestFriend.BestFriend.BestFriend.BestFriend,
                                        Address = f.BestFriend.BestFriend.BestFriend.Address
                                    }
                                })
                        }),
                },
                new TestCase
                {
                    // Cast: Reference property one level down from top-level entity in subquery
                    // Projected Properties: Reference property one level down from top-level entity
                    testQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    BestFriend = new Person()
                                    {
                                        ID = (f.BestFriend as Person).ID,
                                        Name = (f.BestFriend as Person).Name,
                                        BestFriend = (f.BestFriend as Person).BestFriend,
                                        Address = (f.BestFriend as Person).Address
                                    }
                                })
                        }),

                    baselineQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    BestFriend = new Person()
                                    {
                                        ID = f.BestFriend.ID,
                                        Name = f.BestFriend.Name,
                                        BestFriend = f.BestFriend.BestFriend,
                                        Address = f.BestFriend.Address
                                    }
                                })
                        }),
                },
                new TestCase
                {
                    // Cast: Reference property two levels down from top-level entity in subquery
                    // Projected Properties: Reference property three levels down from top-level entity
                    testQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    BestFriend = new Person()
                                    {
                                        // leaving ID with no casting because when all had casting this was working even before the bug fix
                                        // but with no casting on the first property, it was failing
                                        ID = f.BestFriend.BestFriend.BestFriend.ID,
                                        Name = (f.BestFriend.BestFriend as Person).BestFriend.Name,
                                        BestFriend = (f.BestFriend.BestFriend as Person).BestFriend.BestFriend,
                                        Address = (f.BestFriend.BestFriend as Person).BestFriend.Address
                                    }
                                })
                        }),

                    baselineQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    BestFriend = new Person()
                                    {
                                        ID = f.BestFriend.BestFriend.BestFriend.ID,
                                        Name = f.BestFriend.BestFriend.BestFriend.Name,
                                        BestFriend = f.BestFriend.BestFriend.BestFriend.BestFriend,
                                        Address = f.BestFriend.BestFriend.BestFriend.Address
                                    }
                                })
                        }),
                },
                new TestCase
                {
                    // Cast: Reference property two levels down from top-level entity in subequery and also reference property three levels down
                    // Projected Properties: Reference property three levels down from top-level entity
                    testQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    BestFriend = new Person()
                                    {
                                        // leaving ID with no casting because when all had casting this was working even before the bug fix
                                        // but with no casting on the first property, it was failing
                                        ID = f.BestFriend.BestFriend.BestFriend.ID,
                                        Name = ((f.BestFriend.BestFriend as Person).BestFriend as Person).Name,
                                        BestFriend = ((f.BestFriend.BestFriend as Person).BestFriend as Person).BestFriend,
                                        Address = ((f.BestFriend.BestFriend as Person).BestFriend as Person).Address
                                    }
                                })
                        }),

                    baselineQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    BestFriend = new Person()
                                    {
                                        ID = f.BestFriend.BestFriend.BestFriend.ID,
                                        Name = f.BestFriend.BestFriend.BestFriend.Name,
                                        BestFriend = f.BestFriend.BestFriend.BestFriend.BestFriend,
                                        Address = f.BestFriend.BestFriend.BestFriend.Address
                                    }
                                })
                        }),
                },
                new TestCase
                {
                    // Cast: Various levels from top-level entity, not the same on all projected properties
                    // Projected Properties: Reference property three levels down from top-level entity
                    testQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    BestFriend = new Person()
                                    {
                                        ID = (((f as Person).BestFriend.BestFriend as Person).BestFriend as Person).ID,
                                        Name = (f as Person).BestFriend.BestFriend.BestFriend.Name,
                                        BestFriend = f.BestFriend.BestFriend.BestFriend.BestFriend,
                                        Address = (f.BestFriend.BestFriend.BestFriend as Person).Address
                                    }
                                })
                        }),

                    baselineQuery = (people) => people.
                        Select(c => new ProjectionWrapper
                        {
                            ProjectedFriends = c.Friends.Select(f =>
                                new Person
                                {
                                    BestFriend = new Person()
                                    {
                                        ID = f.BestFriend.BestFriend.BestFriend.ID,
                                        Name = f.BestFriend.BestFriend.BestFriend.Name,
                                        BestFriend = f.BestFriend.BestFriend.BestFriend.BestFriend,
                                        Address = f.BestFriend.BestFriend.BestFriend.Address
                                    }
                                })
                        }),
                },
            };

            TestUtil.RunCombinations(testCases, (testCase) =>
                {
                    string friend5Entry = ProjectionTests.AnyEntry("f5", GetPersonProperties(5), ProjectionTests.Link(true, "BestFriend", String.Empty));
                    string friend4Entry = ProjectionTests.AnyEntry("f4", GetPersonProperties(4), ProjectionTests.Link(true, "BestFriend", friend5Entry));
                    string friend3Entry = ProjectionTests.AnyEntry("f3", GetPersonProperties(3), ProjectionTests.Link(true, "BestFriend", friend4Entry));
                    string friend2Entry = ProjectionTests.AnyEntry("f2", GetPersonProperties(2), ProjectionTests.Link(true, "BestFriend", friend3Entry));
                    string friend1Entry = ProjectionTests.AnyEntry("f1", GetPersonProperties(1), ProjectionTests.Link(true, "BestFriend", friend2Entry));

                    string feedStart = AtomParserTests.FeedStart;

                    string friendsXml =
                        feedStart +
                        friend1Entry +
                        friend2Entry +
                        "</feed>";

                    string person0Entry = ProjectionTests.AnyEntry("f1", GetPersonProperties(1), ProjectionTests.Link(false, "Friends", friendsXml));

                    string xml =
                        feedStart +
                        person0Entry +
                        "</feed>";

                    // Create the queries against two different contexts to ensure they really work properly and we don't accidentally get the benefit of identity resolution
                    DataServiceContext testContext = new DataServiceContext(new Uri("http://localhost"));
                    testContext.EnableAtom = true;
                    IQueryable<ProjectionWrapper> testQuery = testCase.testQuery(testContext.CreateQuery<Person>("People"));

                    DataServiceContext baselineContext = new DataServiceContext(new Uri("http://localhost"));
                    baselineContext.EnableAtom = true;
                    IQueryable<ProjectionWrapper> baselineQuery = testCase.baselineQuery(baselineContext.CreateQuery<Person>("People"));

                    // Verify that the queries did produce the same URIs
                    Assert.AreEqual(baselineQuery.ToString(), testQuery.ToString(), "Expected the queries to produce the same URI");

                    // Execute the query and get the materialized object
                    ProjectionWrapper testProjectionResult = ProjectionTests.CreateTestMaterializeAtom(xml, testContext, testQuery).Single();
                    ProjectionWrapper baselineProjectionResult = ProjectionTests.CreateTestMaterializeAtom(xml, baselineContext, baselineQuery).Single();

                    // Since we should be using different contexts, make sure we really got two different instances.
                    Assert.AreNotSame(baselineProjectionResult, testProjectionResult, "Expected the objects to come from different contexts and be different instances.");

                    // The query is just producing a wrapper type, so we only really care about validating the one projected property
                    IEnumerable<Person> baselineFriends = baselineProjectionResult.ProjectedFriends;
                    IEnumerable<Person> testFriends = testProjectionResult.ProjectedFriends;

                    Assert.AreNotEqual(0, baselineFriends.Count(), "Baseline query returned no results");
                    Assert.AreEqual(baselineFriends.Count(), testFriends.Count(), "Test query produced wrong number of results in Friends");

                    foreach (Person baselineFriend in baselineFriends)
                    {
                        Person testFriend = testFriends.Where(o => o.ID == baselineFriend.ID).SingleOrDefault();
                        Assert.IsNotNull(testFriend, "Test Friends does not contain friend with ID {0}", baselineFriend.ID);
                        VerifyPerson(baselineFriend, testFriend);
                    }
                });
        }

        private static void VerifyPerson(Person expectedPerson, Person actualPerson)
        {
            Assert.AreNotSame(expectedPerson, actualPerson, "Expected the objects to be different instances.");

            Assert.AreEqual(expectedPerson.ID, actualPerson.ID, "Wrong value for Person.ID");
            Assert.AreEqual(expectedPerson.Name, actualPerson.Name, "Wrong value for Person.Name");

            Address expectedAddress = expectedPerson.Address;
            Address actualAddress = actualPerson.Address;

            if (expectedAddress == null)
            {
                Assert.IsNull(actualAddress);
            }
            else
            {
                Assert.AreEqual(expectedAddress.City, actualAddress.City, "Wrong value for Address.City");
                Assert.AreEqual(expectedAddress.PostalCode, actualAddress.PostalCode, "Wrong value for Address.PostalCode");
                Assert.AreEqual(expectedAddress.State, actualAddress.State, "Wrong value for Address.State");
                Assert.AreEqual(expectedAddress.StreetAddress, actualAddress.StreetAddress, "Wrong value for Address.StreetAddress");
            }

            if (expectedPerson.BestFriend == null)
            {
                Assert.IsNull(actualPerson.BestFriend);
            }
            else
            {
                Assert.IsNotNull(actualPerson.BestFriend, "Expected a non-null value for BestFriend");
                VerifyPerson(expectedPerson.BestFriend, actualPerson.BestFriend);
            }
        }

        private static string GetPersonProperties(int id)
        {
            return String.Format("<d:ID>{0}</d:ID><d:Name>Person{0}</d:Name><d:Address><d:City>City{0}</d:City><d:StreetAddress>StreetAddress{0}</d:StreetAddress></d:Address>", id);
        }

        public class Person
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public Address Address { get; set; }
            public Person BestFriend { get; set; }
            public List<Person> Friends { get; set; }
        }

        private class TestCase
        {
            public Func<IQueryable<Person>, IQueryable<ProjectionWrapper>> testQuery;
            public Func<IQueryable<Person>, IQueryable<ProjectionWrapper>> baselineQuery;
        }

        public class ProjectionWrapper
        {
            public IEnumerable<Person> ProjectedFriends;
        }

        [TestMethod]
        public void MaterializationOfEntitesIntoComplexTypeShouldNotBeAllowed()
        {
            // Materialization of entities into complex type should not be allowed
            var versionV3 = new ODataProtocolVersion[] { ODataProtocolVersion.V4 };

            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.StartService();

                // Top Level Type Mismatch: Client - NonEntity, Server - Entity - Feed Case
                HelperMethod<CustomerNonEntity>(
                    request.ServiceRoot,
                    "/Customers",
                    DataServicesClientResourceUtil.GetString("AtomMaterializer_InvalidNonEntityType", typeof(CustomerNonEntity)));

                // Top Level Type Mismatch: Client - NonEntity, Server - Entity - Single Entity case
                HelperMethod<CustomerNonEntity>(
                    request.ServiceRoot,
                    "/Customers(1)",
                    DataServicesClientResourceUtil.GetString("AtomMaterializer_InvalidNonEntityType", typeof(CustomerNonEntity)));

                // Top Level Type Mismatch: Client - Entity, Server - NonEntity
                HelperMethod<AddressEntity>(
                    request.ServiceRoot,
                    "/Customers(0)/Address",
                    DataServicesClientResourceUtil.GetString("AtomMaterializer_InvalidEntityType", typeof(AddressEntity)));

                // The reason for doing $select here is if we do not do $select, Orders will should up as unexpanded link, and the client already validates that the property
                // must be a collection for unexpanded collection links.

                // Navigation Reference Property: Client - NonEntity, Server - Entity
                HelperMethod<CustomerEntity>(
                    request.ServiceRoot,
                    "/Customers?$select=ID,Name,BestFriend&$expand=BestFriend",
                    ODataLibResourceUtil.GetString("ValidationUtils_NavigationPropertyExpected", "BestFriend", "AstoriaUnitTests.Tests.ClientCSharpRegressionTests_CustomerEntity", "Structural"));

                // Navigation Reference Property: Client - Entity, Server - NonEntity
                HelperMethod<CustomerEntity>(
                    request.ServiceRoot,
                    "/Customers?$select=ID,Name,Address",
                    ODataLibResourceUtil.GetString("ODataAtomPropertyAndValueDeserializer_NavigationPropertyInProperties", "Address", "AstoriaUnitTests.Tests.ClientCSharpRegressionTests_CustomerEntity"));

                // Navigation Collection Property: Client - NonEntity, Server - Entity
                HelperMethod<CustomerEntity>(
                    request.ServiceRoot,
                    "/Customers?$select=ID,Name,Orders&$expand=Orders",
                    ODataLibResourceUtil.GetString("ValidationUtils_NavigationPropertyExpected", "Orders", "AstoriaUnitTests.Tests.ClientCSharpRegressionTests_CustomerEntity", "Structural"));
            }

            // Navigation Collection Property: Client - Entity, Server - NonEntity
            DSPMetadata metadata = new DSPMetadata("TypeMismatchTests", "AstoriaUnitTests.Tests");
            var entityType = metadata.AddEntityType("CustomerEntity", null, null, false);
            metadata.AddKeyProperty(entityType, "ID", typeof(int));
            metadata.AddPrimitiveProperty(entityType, "Name", typeof(string));
            var complexType = metadata.AddComplexType("OrderNonEntity", null, null, false);
            metadata.AddPrimitiveProperty(complexType, "DollarAmount", typeof(int));
            metadata.AddCollectionProperty(entityType, "OrdersNavProperty", complexType);
            var resourceSet = metadata.AddResourceSet("Customers", entityType);
            metadata.SetReadOnly();

            DSPContext context = new DSPContext();
            DSPResource entity1 = new DSPResource(entityType);
            entity1.SetValue("ID", 1);
            entity1.SetValue("Name", "Foo");
            DSPResource complex1 = new DSPResource(complexType);
            complex1.SetValue("DollarAmount", 100);
            entity1.SetValue("OrdersNavProperty", new List<DSPResource> { complex1 });
            context.GetResourceSetEntities("Customers").Add(entity1);

            DSPServiceDefinition service = new DSPServiceDefinition()
            {
                Metadata = metadata,
                CreateDataSource = (m) => context,
            };

            using (TestWebRequest request = service.CreateForInProcessWcf())
            {
                request.StartService();

                // for V1/V2, the MDSV is too low for the response, since the response contains collection properties. hence testing this only for V3.
                HelperMethod<CustomerEntity>(
                    request.ServiceRoot,
                    "/Customers",
                    versionV3,
                    ODataLibResourceUtil.GetString("ODataAtomPropertyAndValueDeserializer_NavigationPropertyInProperties", "OrdersNavProperty", "AstoriaUnitTests.Tests.ClientCSharpRegressionTests_CustomerEntity"));
            }
        }

        private static void HelperMethod<T>(Uri serviceUri, string requestUri, string expectedErrorMessage)
        {
            HelperMethod<T>(serviceUri, requestUri, new ODataProtocolVersion[] { ODataProtocolVersion.V4 }, expectedErrorMessage);
        }

        private static void HelperMethod<T>(Uri serviceUri, string requestUri, ODataProtocolVersion[] versions, string expectedErrorMessage)
        {
            TestUtil.RunCombinations(versions, (maxProtocolVersion) =>
                {
                    DataServiceContext context = new DataServiceContext(serviceUri, maxProtocolVersion);
                    context.EnableAtom = true;
                    context.Format.UseAtom();

                    context.IgnoreMissingProperties = true;
                    bool innerException = true;
                    try
                    {
                        // Since we now pass the model to ODataLib, for top level cases, we fail before
                        // we start to enumerate. For inner payload errors, we fail during enumeration and hence
                        // we need to check for the message directly.
                        var result = context.Execute<T>(new Uri(requestUri, UriKind.RelativeOrAbsolute));
                        innerException = false;
                        result.FirstOrDefault();
                        if (expectedErrorMessage != null)
                        {
                            Assert.Fail("Error expected, but no error was thrown for query '{0}'", requestUri);
                        }
                    }
                    catch (InvalidOperationException e)
                    {
                        if (expectedErrorMessage == null)
                        {
                            Assert.Fail(String.Format("No Error expected, but error was thrown for uri '{0}' - '{1}'", requestUri, e.Message));
                        }

                        Assert.AreEqual(expectedErrorMessage, innerException ? e.InnerException.Message : e.Message, "the error message did not match");
                    }
                });
        }

        public class CustomerNonEntity
        {
            public string Name { get; set; }
        }

        public class OrderNonEntity
        {
            public double DollarAmount { get; set; }
        }

        [EntityType]
        public class OrderEntity
        {
            public double DollarAmount { get; set; }
        }

        [EntityType()]
        public class CustomerEntity
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public CustomerNonEntity BestFriend { get; set; }
            public List<OrderNonEntity> Orders { get; set; }
            public List<OrderEntity> OrdersNavProperty { get; set; }
            public AddressEntity Address { get; set; }
        }

        [EntityType()]
        public class AddressEntity
        {
            public string City { get; set; }
            public string PostalCode { get; set; }
            public string State { get; set; }
            public string StreetAddress { get; set; }
        }

        [TestMethod]
        public void RequestVersionShouldNotBumpForDeletion()
        {
            // Ensures that the request version is not bumped unnecessarily for delete operations
            IEnumerable<object> entities = new List<object> {new AstoriaUnitTests.DataWebClientCSharp.Collection.EntityWithDictionary {ID = 1, Collection = new Dictionary<string, string> {{"property1", "value1"}, {"property2", "value2"}}}, // non collection
                new Entity<ICollection<int>, int>(new int[] {100, 101, 200}), // collection
                new Customer {ID = 1, Name = "Customer"} // EPM
            };

            TestUtil.RunCombinations(entities, (entity) =>
            {
                Func<IODataRequestMessage> getRequestMessage = () => new ODataTestMessage();
                Func<IODataResponseMessage> getResponseMessage = () =>
                {
                    var responseMessage = new ODataTestMessage();
                    responseMessage.StatusCode = 201;
                    responseMessage.SetHeader("Content-Length", "0");
                    responseMessage.SetHeader("Location", DataServiceContextWithCustomTransportLayer.DummyUri.AbsoluteUri);
                    responseMessage.SetHeader("OData-EntityId", DataServiceContextWithCustomTransportLayer.DummyUri.AbsoluteUri);
                    return responseMessage;
                };

                var ctx = new DataServiceContextWithCustomTransportLayer(ODataProtocolVersion.V4, getRequestMessage, getResponseMessage);
                ctx.EnableAtom = true;
                ctx.Format.UseAtom();

                ctx.AddObject("Entities", entity);
                ctx.SaveChanges();

                ctx.SendingRequest2 += (sender, args) =>
                {
                    Assert.IsTrue(args.RequestMessage.GetHeader("OData-Version").StartsWith("4.0"));
                };

                ctx.DeleteObject(entity);
                ctx.SaveChanges();
            });
        }

        [TestMethod]
        public void VerifyUriAppliedToDescriptorWhenLinkFoldingUsed()
        {
            // Verify URIs are properly applied to descriptors when link folding is used
            TestUtil.RunCombinations(
                (DataServiceResponsePreference[])Enum.GetValues(typeof(DataServiceResponsePreference)),
                new ODataProtocolVersion[] { ODataProtocolVersion.V4 },
                new SaveChangesOptions[] { SaveChangesOptions.None, SaveChangesOptions.BatchWithSingleChangeset },
                (responsePreference, protocolVersion, saveChangesOption) =>
                {
                    if (responsePreference != DataServiceResponsePreference.None && protocolVersion < ODataProtocolVersion.V4)
                    {
                        // Response preference only applies to V3 and higher
                        return;
                    }

                    using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                    {
                        request.DataServiceType = typeof(CustomDataContext);
                        request.StartService();

                        // Verify the correct URIs are used whether they come from the payload or the headers
                        DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, protocolVersion);
                        ctx.EnableAtom = true;
                        ctx.Format.UseAtom();

                        ctx.AddAndUpdateResponsePreference = responsePreference;

                        Order newOrder1 = new Order();
                        newOrder1.ID = 1000;
                        ctx.AddObject("Orders", newOrder1);

                        // Attach some existing order details so we can create links to them from the new order
                        OrderDetail existingOrderDetail1 = new OrderDetail();
                        existingOrderDetail1.OrderID = 1;
                        existingOrderDetail1.ProductID = 1;
                        ctx.AttachTo("OrderDetails", existingOrderDetail1);

                        OrderDetail existingOrderDetail2 = new OrderDetail();
                        existingOrderDetail2.OrderID = 2;
                        existingOrderDetail2.ProductID = 2;
                        ctx.AttachTo("OrderDetails", existingOrderDetail2);

                        // Adding links where the targets are in the unchanged state will cause the links to be folded into the POST for newOrder1, instead of sent separately.
                        // Particularly in the case of batch, this means that the client has to be able to handle not seeing a response for this change.
                        // In Dev11:86477 we were trying to apply the headers from the response to the link descriptor instead of the correct entity descriptor,
                        // because we forgot to skip over the link descriptor when processing the changes.
                        ctx.AddLink(newOrder1, "OrderDetails", existingOrderDetail1);
                        ctx.AddLink(newOrder1, "OrderDetails", existingOrderDetail2);

                        // Need a second object that will show up in the batch after the position where the folded link would have been,
                        // so we can verify we're really skipping the link properly.
                        Order newOrder2 = new Order();
                        newOrder2.ID = 1001;
                        ctx.AddObject("Orders", newOrder2);

                        EntityDescriptor newOrder1Descriptor = ctx.GetEntityDescriptor(newOrder1);
                        Assert.IsNull(newOrder1Descriptor.EditLink, "EditLink should be null before first new order is saved.");
                        Assert.IsNull(newOrder1Descriptor.Identity, "Identity should be null before first new order is saved.");
                        Assert.AreEqual(EntityStates.Added, newOrder1Descriptor.State, "Expected first new order to be in the Added state before SaveChanges.");

                        EntityDescriptor newOrder2Descriptor = ctx.GetEntityDescriptor(newOrder2);
                        Assert.IsNull(newOrder2Descriptor.EditLink, "EditLink should be null before second new order is saved.");
                        Assert.IsNull(newOrder2Descriptor.Identity, "Identity should be null before second new order is saved.");
                        Assert.AreEqual(EntityStates.Added, newOrder2Descriptor.State, "Expected second new order to be in the Added state before SaveChanges.");

                        LinkDescriptor linkDescriptor = ctx.GetLinkDescriptor(newOrder1, "OrderDetails", existingOrderDetail1);
                        Assert.AreEqual(EntityStates.Added, linkDescriptor.State, "Expected first link to be in the Added state before SaveChanges.");
                        linkDescriptor = ctx.GetLinkDescriptor(newOrder1, "OrderDetails", existingOrderDetail2);
                        Assert.AreEqual(EntityStates.Added, linkDescriptor.State, "Expected second link to be in the Added state before SaveChanges.");

                        // The bug in Dev11:86477 only applies to batch, but testing this for non-batch too to make sure it continues to work as well.
                        ctx.SaveChanges(saveChangesOption);

                        string orderIdUri = request.ServiceRoot + "/Orders({0})";

                        newOrder1Descriptor = ctx.GetEntityDescriptor(newOrder1);
                        string id = String.Format(orderIdUri, newOrder1.ID);

                        Assert.AreEqual(id, newOrder1Descriptor.EditLink.AbsoluteUri, "Wrong value for EditLink on first new order after SaveChanges.");
                        Assert.AreEqual(id, newOrder1Descriptor.Identity.AbsoluteUri, "Wrong value for Identity on first new order after SaveChanges.");
                        Assert.AreEqual(EntityStates.Unchanged, newOrder1Descriptor.State, "Expected first new order to be in the Unchanged state after SaveChanges.");

                        newOrder2Descriptor = ctx.GetEntityDescriptor(newOrder2);
                        string id2 = String.Format(orderIdUri, newOrder2.ID);
                        Assert.AreEqual(id2, newOrder2Descriptor.EditLink.AbsoluteUri, "Wrong value for EditLink on second new order after SaveChanges.");
                        Assert.AreEqual(id2, newOrder2Descriptor.Identity.AbsoluteUri, "Wrong value for Identity on second new order after SaveChanges.");
                        Assert.AreEqual(EntityStates.Unchanged, newOrder2Descriptor.State, "Expected second new order to be in the Unchanged state after SaveChanges.");

                        linkDescriptor = ctx.GetLinkDescriptor(newOrder1, "OrderDetails", existingOrderDetail1);
                        Assert.AreEqual(EntityStates.Unchanged, linkDescriptor.State, "Expected first link to be in the Unchanged state after SaveChanges.");
                        linkDescriptor = ctx.GetLinkDescriptor(newOrder1, "OrderDetails", existingOrderDetail2);
                        Assert.AreEqual(EntityStates.Unchanged, linkDescriptor.State, "Expected second link to be in the Unchanged state after SaveChanges.");
                    }
                });
        }

        #region Client does not write any type information for primitive collection properties

        public class TestEntity7
        {
            public int ID { get; set; }
            public List<Boolean?> CollectionNBoolean { get; set; }
            public List<Boolean> CollectionBoolean { get; set; }
            public List<Byte?> CollectionNByte { get; set; }
            public List<Byte> CollectionByte { get; set; }
            public List<Byte[]> CollectionByteArray { get; set; }
            public List<Char?> CollectionNChar { get; set; }
            public List<Char> CollectionChar { get; set; }
            public List<Char[]> CollectionCharArray { get; set; }
            public List<Decimal?> CollectionNDecimal { get; set; }
            public List<Decimal> CollectionDecimal { get; set; }
            public List<Double?> CollectionNDouble { get; set; }
            public List<Double> CollectionDouble { get; set; }
            public List<Guid?> CollectionNGuid { get; set; }
            public List<Guid> CollectionGuid { get; set; }
            public List<Int16?> CollectionNInt16 { get; set; }
            public List<Int16> CollectionInt16 { get; set; }
            public List<Int32?> CollectionNInt32 { get; set; }
            public List<Int32> CollectionInt32 { get; set; }
            public List<Int64?> CollectionNInt64 { get; set; }
            public List<Int64> CollectionInt64 { get; set; }
            public List<Single?> CollectionNSingle { get; set; }
            public List<Single> CollectionSingle { get; set; }
            public List<String> CollectionString { get; set; }
            public List<SByte?> CollectionNSByte { get; set; }
            public List<SByte> CollectionSByte { get; set; }
            public List<Type> CollectionType { get; set; }
            public List<Uri> CollectionUri { get; set; }
            public List<System.Xml.Linq.XDocument> CollectionXDocument { get; set; }
            public List<System.Xml.Linq.XElement> CollectionXElement { get; set; }
            public List<DateTimeOffset?> CollectionNDateTimeOffset { get; set; }
            public List<DateTimeOffset> CollectionDateTimeOffset { get; set; }
            public List<TimeSpan?> CollectionNTimeSpan { get; set; }
            public List<TimeSpan> CollectionTimeSpan { get; set; }
            
            // Not Supported
            //public List<UInt16?> CollectionNUInt16 { get; set; }
            //public List<UInt16> CollectionUInt16 { get; set; }
            //public List<UInt32?> CollectionNUInt32 { get; set; }
            //public List<UInt32> CollectionUInt32 { get; set; }
            //public List<UInt64?> CollectionNUInt64 { get; set; }
            //public List<UInt64> CollectionUInt64 { get; set; }
        }

        [TestMethod]
        public void ClientShouldWriteTypeInfoForPrimitiveCollectionProperties()
        {
            // Client does not write any type information for primitive collection properties
            var requestMessage = new ODataTestMessage();
            var responseMessage = new ODataTestMessage { StatusCode = 201 };
            responseMessage.SetHeader("Content-Length", "0");
            responseMessage.SetHeader("Location", DataServiceContextWithCustomTransportLayer.DummyUri.AbsoluteUri);
            responseMessage.SetHeader("OData-EntityId", DataServiceContextWithCustomTransportLayer.DummyUri.AbsoluteUri);

            DataServiceContextWithCustomTransportLayer ctx = new DataServiceContextWithCustomTransportLayer(
                ODataProtocolVersion.V4,
                requestMessage,
                responseMessage);

            ctx.EnableAtom = true;
            ctx.Format.UseAtom();

            TestEntity7 entity = new TestEntity7()
                {
                    ID = 1,
                    CollectionBoolean = new List<bool>(),
                    CollectionByte = new List<byte>(),
                    CollectionByteArray = new List<byte[]>(),
                    CollectionChar = new List<char>(),
                    CollectionCharArray = new List<char[]>(),
                    CollectionDecimal = new List<decimal>(),
                    CollectionDouble = new List<double>(),
                    CollectionGuid = new List<Guid>(),
                    CollectionInt16 = new List<short>(),
                    CollectionInt32 = new List<int>(),
                    CollectionInt64 = new List<long>(),
                    CollectionNBoolean = new List<bool?>(),
                    CollectionNByte = new List<byte?>(),
                    CollectionNChar = new List<char?>(),
                    CollectionNDecimal = new List<decimal?>(),
                    CollectionNDouble = new List<double?>(),
                    CollectionNGuid = new List<Guid?>(),
                    CollectionNInt16 = new List<short?>(),
                    CollectionNInt32 = new List<int?>(),
                    CollectionNInt64 = new List<long?>(),
                    CollectionNSByte = new List<sbyte?>(),
                    CollectionNSingle = new List<float?>(),
                    CollectionSByte = new List<sbyte>(),
                    CollectionSingle = new List<float>(),
                    CollectionString = new List<string>(),
                    CollectionType = new List<Type>(),
                    CollectionUri = new List<Uri>(),
                    CollectionXDocument = new List<XDocument>(),
                    CollectionXElement = new List<XElement>(),
                    CollectionDateTimeOffset = new List<DateTimeOffset>(),
                    CollectionNDateTimeOffset = new List<DateTimeOffset?>(),
                    CollectionTimeSpan = new List<TimeSpan>(),
                    CollectionNTimeSpan = new List<TimeSpan?>(),

                    // Not Supported
                    //CollectionNUInt16 = new List<ushort?>(),
                    //CollectionNUInt32 = new List<uint?>(),
                    //CollectionNUInt64 = new List<ulong?>(),
                    //CollectionUInt16 = new List<ushort>(),
                    //CollectionUInt32 = new List<uint>(),
                    //CollectionUInt64 = new List<ulong>(),
                };

            ctx.AddObject("Entities", entity);
            ctx.SaveChanges();

            ctx.SendingRequest2 += (sender, args) =>
            {
                Assert.IsTrue(args.RequestMessage.GetHeader("OData-Version").StartsWith("1.0"));
            };

            // Verify the type name is written on the wire
            requestMessage.MemoryStream.Position = 0;
            XDocument doc = XDocument.Load(requestMessage.MemoryStream);
            XElement properties = doc.Root.Element(UnitTestsUtil.AtomNamespace + "content").Element(UnitTestsUtil.MetadataNamespace + "properties");

            List<KeyValuePair<XName, string>> prop = new List<KeyValuePair<XName, string>>()
            {
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionBoolean", "Collection(Boolean)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionByte", "Collection(Byte)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionByteArray", "Collection(Binary)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionChar", "Collection(String)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionCharArray", "Collection(String)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionDecimal", "Collection(Decimal)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionDouble", "Collection(Double)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionGuid", "Collection(Guid)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionInt16", "Collection(Int16)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionInt32", "Collection(Int32)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionInt64", "Collection(Int64)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionNBoolean", "Collection(Boolean)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionNByte", "Collection(Byte)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionNChar", "Collection(String)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionNDecimal", "Collection(Decimal)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionNDouble", "Collection(Double)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionNGuid", "Collection(Guid)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionNInt16", "Collection(Int16)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionNInt32", "Collection(Int32)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionNInt64", "Collection(Int64)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionNSByte", "Collection(SByte)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionNSingle", "Collection(Single)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionSByte", "Collection(SByte)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionSingle", "Collection(Single)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionString", "Collection(String)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionType", "Collection(String)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionUri", "Collection(String)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionXDocument", "Collection(String)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionXElement", "Collection(String)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionDateTimeOffset", "Collection(DateTimeOffset)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionNDateTimeOffset", "Collection(DateTimeOffset)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionTimeSpan", "Collection(Duration)"),
                new KeyValuePair<XName, string>(UnitTestsUtil.DataNamespace + "CollectionNTimeSpan", "Collection(Duration)"),
            };

            foreach (var p in prop)
            {
                Assert.AreEqual('#' + p.Value, properties.Element(p.Key).Attribute(UnitTestsUtil.MetadataNamespace + "type").Value);
            }
        }

        #endregion

        #region DeleteLinkAsyncExtraLinkDescriptorTest

        [TestMethod]
        public void DeleteLinkAsyncExtraLinkDescriptorTest()
        {
            // Setting a reference link to null asynchronously sometimes returns same operation response twice
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.StartService();

                var waitHandle = new ManualResetEvent(false);
                Exception exc = null;

                // Takes about 10 secs to run 4000 iterations on a VM in a typical dev box.
                DeleteLinkAsync(request.ServiceRoot, 0, 4000, waitHandle, e => { exc = e; waitHandle.Set(); });

                WaitHandle.WaitAll(new WaitHandle[] { waitHandle }, TimeSpan.FromMinutes(3));
                if (exc != null)
                {
                    Assert.Fail(exc.ToString());
                }
            }
        }

        private void DeleteLinkAsync(Uri serviceUri, int iteration, int maxIterations, EventWaitHandle handle, Action<Exception> onFail)
        {
            if (iteration > maxIterations)
            {
                handle.Set();
                return;
            }
            var context = new DataServiceContext(serviceUri);
            var order = new Order() { ID = 0 };
            context.AttachTo("Orders", order);
            context.SetLink(order, "Customer", null);
            context.BeginSaveChanges(
            result =>
            {
                try
                {
                    var ctx = (DataServiceContext)result.AsyncState;
                    var operationResponses = ctx.EndSaveChanges(result).ToList();
                    Assert.AreEqual(1, operationResponses.Count, string.Format("Number of operation responses was wrong for iteration {0} of {1}", iteration, maxIterations));
                }
                catch (Exception e)
                {
                    onFail(e);
                }
                DeleteLinkAsync(serviceUri, iteration + 1, maxIterations, handle, onFail);
            }, context);
        }

        #endregion

        [TestMethod]
        public void HiddenNullReferenceException()
        {
            // Cancellation of LoadAsync requests should not cause a null reference exception.
            // The context does not need to return usable results. It is created with a dummy URI so
            // that there is actually time to cancel the request. It should not return promptly.
            var context = new Microsoft.OData.Client.DataServiceContext(new Uri("http://123.32.52.1"));
            context.IgnoreMissingProperties = true;
            Uri uri = new Uri("foo", UriKind.Relative);

            IAsyncResult result = context.BeginExecute<string>(uri, (_) => { }, null);
            context.CancelRequest(result);

            Exception expectedException = TestUtil.RunCatching(() => { context.EndExecute<string>(result); });

            Assert.IsNotNull(expectedException);
            Assert.IsTrue(expectedException is InvalidOperationException);
            string expectedMessage = DataServicesClientResourceUtil.GetString("Context_OperationCanceled");
            Assert.AreEqual(expectedMessage, expectedException.Message);
        }

        [TestMethod]
        public void TestMultipleKeysOfKeyAttribute()
        {
            Microsoft.OData.Client.DataServiceContext context = new Microsoft.OData.Client.DataServiceContext(new Uri("http://DoesNotExist/nope.svc"));
            TestType7 instance = new TestType7 { FkID = 11, TypeID = 22 };
            context.AttachTo("Set", instance);
            Microsoft.OData.Client.EntityDescriptor entity = context.Entities.Single();
            StringAssert.Contains(entity.Identity.AbsoluteUri, "FkID=11", "didn't get the 1st key value");
            StringAssert.Contains(entity.Identity.AbsoluteUri, "TypeID=22", "didn't get the 2nd key value");
        }

        [Microsoft.OData.Client.Key("FkID", "TypeID")]
        public class TestType7
        {
            public int FkID { get; set; }
            public int TypeID { get; set; }
        }

        #region ClientTypeCacheError_LoadProperties
        public class ClientTypeCacheError_NonEntityType
        {
            public string Name { get; set; }
            public ClientTypeCacheError_EntityType NavProp { get; set; }
            public string Name2 { get; set; }
        }

        public class ClientTypeCacheError_EntityType
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public ClientTypeCacheError_NonEntityType NavProp { get; set; }
        }

        [TestMethod]
        public void ClientTypeCacheError_LoadProperties()
        {
            // ensure that with an error thrown, the type cache gets deleted, and that when you try again, the same error gets thrown
            for (int i = 0; i < 2; ++i)
            {
                DataServiceContext ctx = new DataServiceContext(new Uri("http://localhost"), ODataProtocolVersion.V4);
                ctx.AddObject("Entities", new ClientTypeCacheError_EntityType());

                Exception ex = TestUtil.RunCatching(() => ctx.SaveChanges());
                Assert.IsNotNull(ex);
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }

                Assert.AreEqual(DataServicesClientResourceUtil.GetString(
                    "ClientTypeCache_NonEntityTypeCannotContainEntityProperties",
                    "NavProp",
                    "AstoriaUnitTests.Tests.ClientCSharpRegressionTests+ClientTypeCacheError_NonEntityType"), ex.Message);
            }
        }

        #endregion

        [TestMethod]
        public void NullCheckAgainstNonEntityProperty()
        {
            // Client LINQ: NullReferenceException preceded by an Assert: atomProperty.Entry != null -- otherwise a primitive property / complex type is being rewritte with a null check; this is only supported for entities and collection
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.StartService();

                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot);
                ctx.EnableAtom = true;
                ctx.Format.UseAtom();

                // Repos: when the IIF statement returns an entity type when the null check is false and the null check is against a non-entity property.
                var q = from c in ctx.CreateQuery<Customer>("Customers")
                        select new Customer { Name = c.Name, Orders = c.Name == null ? null : c.Orders };

                var customers = q.ToList();
                Assert.AreEqual(3, customers.Count);
            }
        }

        #region The setter for OperationResponse.Error property will cause Debug.Assert when the value passed is null.

        [TestMethod]
        public void OperationResponseErrorShouldAcceptNull()
        {
            // The setter for OperationResponse.Error property will cause Debug.Assert when the value passed is null.
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.StartService();

                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot);
                ctx.EnableAtom = true;
                ctx.Format.UseAtom();

                Order newOrder1 = new Order();
                newOrder1.ID = 2332;
                ctx.AddObject("Orders", newOrder1);

                DataServiceResponse response = ctx.SaveChanges();

                response.Single().Error = null;
            }

        }

        #endregion

        [TestMethod]
        public void TestLoadDataServiceCollectionFromLoadProperty()
        {
            // NRE when trying to load DataServiceCollection from the result of LoadProperty.
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.StartService();

                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot);
                ctx.EnableAtom = true;
                ctx.Format.UseAtom();

                var customer = ctx.CreateQuery<Customer>("Customers").FirstOrDefault();
                var result = ctx.LoadProperty<Order>(customer, "Orders", null);

                DataServiceCollection<Order> orders = new DataServiceCollection<Order>(result, TrackingMode.None);
                Assert.AreEqual(customer.Orders.Count, orders.Count);
            }
        }

        public class MyAddress
        {
            public string StreetAddress { get; set; }
            public string StreetAddress2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string PostalCode { get; set; }
        }

        public class MyCustomer
        {
            public int ID { get; set; }
            public MyAddress Address { get; set; }
        }

        public class MyNonEntity
        {
            public MyCustomer Customer { get; set; }
            public MyAddress Address { get; set; }
        }

        public class MyNonEntityCustomer
        {
            public string Name { get; set; }
        }

        public class MyOrder
        {
            public int ID { get; set; }
            public MyNonEntityCustomer Customer { get; set; }
        }

        [TestMethod]
        public void EnumerateProjectionIntoComplexTypeShouldThrow()
        {
            // Client Linq : InvalidOperationException when enumerating results of a projection into a complex type
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.StartService();

                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot);
                ctx.EnableAtom = true;
                ctx.Format.UseAtom();

                var testCases = new[]
                {
                    // Projecting a non-entity customer to an entity customer
                    new
                    {
                        Query = (IQueryable<object>)from o in ctx.CreateQuery<MyOrder>("Orders") select new Order { ID = o.ID, Customer = new Customer { Name = o.Customer.Name }},
                        ExceptionMessage = "Initializing instances of the entity type AstoriaUnitTests.Stubs.Customer with the expression o.Customer.Name is not supported.",
                    },

                    // The following verifies:
                    // For entities, we do not allow projecting properties of their complex properties so that it's complex properties will always be safe for roundtrip.
                    // For non-entities, we allow projection of properties of their complex properties.

                    // Entity inside ananymous type
                    new
                    {
                        Query = (IQueryable<object>)from c in ctx.CreateQuery<Customer>("Customers") select new { Customer = new Customer { ID = c.ID, Address = new Address { StreetAddress = c.Address.StreetAddress }}},
                        ExceptionMessage = "Initializing instances of the entity type AstoriaUnitTests.Stubs.Customer with the expression new Address() {StreetAddress = c.Address.StreetAddress} is not supported.",
                    },
                    new
                    {
                        Query = (IQueryable<object>)from c in ctx.CreateQuery<Customer>("Customers") select new { Customer = new Customer { ID = c.ID, Address = new Address { StreetAddress = c.Address.StreetAddress, City = c.Address.City, State = c.Address.State, PostalCode = c.Address.PostalCode, }}},
                        ExceptionMessage = "Initializing instances of the entity type AstoriaUnitTests.Stubs.Customer with the expression new Address() {StreetAddress = c.Address.StreetAddress, City = c.Address.City, State = c.Address.State, PostalCode = c.Address.PostalCode} is not supported.",
                    },
                    new
                    {
                        Query = (IQueryable<object>)from c in ctx.CreateQuery<Customer>("Customers") select new { Customer = new Customer { ID = c.ID, Address = c.Address }},
                        ExceptionMessage = default(string),
                    },

                    // Entity inside non-entity type
                    new
                    {
                        Query = (IQueryable<object>)from c in ctx.CreateQuery<Customer>("Customers") select new MyNonEntity { Customer = new MyCustomer { ID = c.ID, Address = new MyAddress { StreetAddress = c.Address.StreetAddress }}},
                        ExceptionMessage = "Initializing instances of the entity type AstoriaUnitTests.Tests.ClientCSharpRegressionTests+MyCustomer with the expression new MyAddress() {StreetAddress = c.Address.StreetAddress} is not supported.",
                    },
                    new
                    {
                        Query = (IQueryable<object>)from c in ctx.CreateQuery<Customer>("Customers") select new MyNonEntity { Customer = new MyCustomer { ID = c.ID, Address = new MyAddress { StreetAddress = c.Address.StreetAddress, City = c.Address.City, State = c.Address.State, PostalCode = c.Address.PostalCode, }}},
                        ExceptionMessage = "Initializing instances of the entity type AstoriaUnitTests.Tests.ClientCSharpRegressionTests+MyCustomer with the expression new MyAddress() {StreetAddress = c.Address.StreetAddress, City = c.Address.City, State = c.Address.State, PostalCode = c.Address.PostalCode} is not supported.",
                    },

                    // Same entity type
                    new
                    {
                        Query = (IQueryable<object>)from c in ctx.CreateQuery<Customer>("Customers") select new Customer { ID = c.ID, Address = new Address { StreetAddress = c.Address.StreetAddress }},
                        ExceptionMessage = "Initializing instances of the entity type AstoriaUnitTests.Stubs.Customer with the expression new Address() {StreetAddress = c.Address.StreetAddress} is not supported.",
                    },
                    new
                    {
                        Query = (IQueryable<object>)from c in ctx.CreateQuery<Customer>("Customers") select new Customer { ID = c.ID, Address = new Address { StreetAddress = c.Address.StreetAddress, City = c.Address.City, State = c.Address.State, PostalCode = c.Address.PostalCode, }},
                        ExceptionMessage = "Initializing instances of the entity type AstoriaUnitTests.Stubs.Customer with the expression new Address() {StreetAddress = c.Address.StreetAddress, City = c.Address.City, State = c.Address.State, PostalCode = c.Address.PostalCode} is not supported.",
                    },
                    new
                    {
                        Query = (IQueryable<object>)from c in ctx.CreateQuery<Customer>("Customers") select new Customer { ID = c.ID, Address = c.Address },
                        ExceptionMessage = default(string),
                    },

                    // Different entity type
                    new
                    {
                        Query = (IQueryable<object>)from c in ctx.CreateQuery<Customer>("Customers") select new MyCustomer { ID = c.ID, Address = new MyAddress { StreetAddress = c.Address.StreetAddress }},
                        ExceptionMessage = "Initializing instances of the entity type AstoriaUnitTests.Tests.ClientCSharpRegressionTests+MyCustomer with the expression new MyAddress() {StreetAddress = c.Address.StreetAddress} is not supported.",
                    },
                    new
                    {
                        Query = (IQueryable<object>)from c in ctx.CreateQuery<Customer>("Customers") select new MyCustomer { ID = c.ID, Address = new MyAddress { StreetAddress = c.Address.StreetAddress, City = c.Address.City, State = c.Address.State, PostalCode = c.Address.PostalCode }},
                        ExceptionMessage = "Initializing instances of the entity type AstoriaUnitTests.Tests.ClientCSharpRegressionTests+MyCustomer with the expression new MyAddress() {StreetAddress = c.Address.StreetAddress, City = c.Address.City, State = c.Address.State, PostalCode = c.Address.PostalCode} is not supported.",
                    },

                    // Non-entity type
                    new
                    {
                        Query = (IQueryable<object>)from c in ctx.CreateQuery<Customer>("Customers") select new MyNonEntity { Address = new MyAddress { StreetAddress = c.Address.StreetAddress }},
                        ExceptionMessage = default(string),
                    },
                    new
                    {
                        Query = (IQueryable<object>)from c in ctx.CreateQuery<Customer>("Customers") select new MyNonEntity { Address = new MyAddress { StreetAddress = c.Address.StreetAddress, City = c.Address.City, State = c.Address.State, PostalCode = c.Address.PostalCode }},
                        ExceptionMessage = default(string),
                    },

                    // Ananymous type
                    new
                    {
                        Query = (IQueryable<object>)from c in ctx.CreateQuery<Customer>("Customers") select new { ID = c.ID, Address = c.Address },
                        ExceptionMessage = default(string),
                    },
                    new
                    {
                        Query = (IQueryable<object>)from c in ctx.CreateQuery<Customer>("Customers") select new { ID = c.ID, Address = new { StreetAddress = c.Address.StreetAddress }},
                        ExceptionMessage = default(string),
                    },
                    new
                    {
                        Query = (IQueryable<object>)from c in ctx.CreateQuery<Customer>("Customers") select new  { ID = c.ID, Address = new { StreetAddress = c.Address.StreetAddress, City = c.Address.City, State = c.Address.State, PostalCode = c.Address.PostalCode }},
                        ExceptionMessage = default(string),
                    },
                };

                TestUtil.RunCombinations(testCases, test =>
                {
                    try
                    {
                        foreach (var o in test.Query)
                        {
                            var obj = o;
                            var customerProperty = obj.GetType().GetProperty("Customer");
                            if (customerProperty != null)
                            {
                                obj = customerProperty.GetValue(obj, null);
                                if (obj == null)
                                {
                                    obj = o;
                                }
                            }

                            var addressProperty = obj.GetType().GetProperty("Address");
                            Assert.IsNotNull(addressProperty.GetValue(obj, null));
                            ctx.Detach(obj);
                        }

                        Assert.IsNull(test.ExceptionMessage);
                    }
                    catch (NotSupportedException e)
                    {
                        Assert.AreEqual(test.ExceptionMessage, e.Message);
                    }
                });
            }
        }

        [TestMethod]
        public void LoadNavigationPropertyToSetNotAssignableFromListOfT()
        {
            // Assert followed by an ArgumentException when loading a navigation property to a set whose type is not assignable from List<T>
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.StartService();

                Type[] testCollectionTypes = new Type[] { typeof(List<Order>), typeof(ICollection<Order>), typeof(MyCollection), typeof(MyList) };
                TestUtil.RunCombinations(testCollectionTypes, (testCollectionType) =>
                    {
                        var testMethodInfo = this.GetType().GetMethod("TestLoadProperty", BindingFlags.Static | BindingFlags.NonPublic);
                        var testMethod = testMethodInfo.MakeGenericMethod(testCollectionType);
                        testMethod.Invoke(null, new object[] { request.ServiceRoot });
                    });
            }
        }

        private static void TestLoadProperty<T>(Uri uri) where T : ICollection<Order>
        {
            DataServiceContext ctx = new DataServiceContext(uri);
            ctx.EnableAtom = true;
            ctx.Format.UseAtom();
            ctx.IgnoreMissingProperties = true;

            var customer = ctx.CreateQuery<Customer<T>>("Customers").FirstOrDefault();

            // Verify the collection is not initialized by the test class
            Assert.IsNull(customer.Orders, "Expected Orders property to be null before LoadProperty is called.");

            var result = ctx.LoadProperty(customer, "Orders");

            // Verify the collection has been initialized and is not empty
            Assert.IsNotNull(customer.Orders, "customer.Orders should have been initialized by LoadProperty.");
            Assert.IsTrue(customer.Orders.Count > 0, "No Orders were loaded by LoadProperty.");

            // Verify the collection is the right type
            Type expectedCollectionType = typeof(T).IsInterface ? typeof(List<Order>) : typeof(T);
            Assert.AreEqual(expectedCollectionType, customer.Orders.GetType(), "customer.Orders is not the expected Type.");
        }


        public class Customer<T> where T : ICollection<Order>
        {
            public int ID { get; set; }
            public T Orders { get; set; }
        }

        public class MyCollection : ICollection<Order>
        {
            private List<Order> orders = new List<Order>();

            public void Add(Order item)
            {
                this.orders.Add(item);
            }

            public void Clear()
            {
                this.orders.Clear();
            }

            public bool Contains(Order item)
            {
                return this.orders.Contains(item);
            }

            public void CopyTo(Order[] array, int arrayIndex)
            {
                this.orders.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return this.orders.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(Order item)
            {
                return this.orders.Remove(item);
            }

            public IEnumerator<Order> GetEnumerator()
            {
                return this.orders.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)this.orders).GetEnumerator();
            }
        }

        public class MyList : List<Order>
        {
        }

        [TestMethod]
        public void ShouldThrowIfSaveChangesResponseNotFollowODataProtocol()
        {
            // Assert in client during save changes if the response from the server does not follow OData protocol.
            var testCases = new[]
            {
                new
                {
                    EntryCount = 1,
                    ValidTopLevelFeed = true,
                    // okay to take the breaking change
                    ExpectedInnerException = new InvalidOperationException(ODataLibResourceUtil.GetString("ODataAtomEntryAndFeedDeserializer_EntryElementWrongName", "feed", "http://www.w3.org/2005/Atom"))
                },
                new
                {
                    EntryCount = 0,
                    ValidTopLevelFeed = true,
                    ExpectedInnerException = new InvalidOperationException(ODataLibResourceUtil.GetString("ODataAtomEntryAndFeedDeserializer_EntryElementWrongName", "feed", "http://www.w3.org/2005/Atom"))
                },
                new
                {
                    EntryCount = 2,
                    ValidTopLevelFeed = true,
                    ExpectedInnerException = new InvalidOperationException(ODataLibResourceUtil.GetString("ODataAtomEntryAndFeedDeserializer_EntryElementWrongName", "feed", "http://www.w3.org/2005/Atom"))
                },
                new
                {
                    EntryCount = 1,
                    ValidTopLevelFeed = false,
                    ExpectedInnerException = new InvalidOperationException(ODataLibResourceUtil.GetString("ODataAtomEntryAndFeedDeserializer_EntryElementWrongName", "feed", ""))
                }
            };

            TestUtil.RunCombinations(testCases, (testCase) =>
            {
                var requestMessage = new ODataTestMessage();
                var responseMessage = new ODataTestMessage();
                DataServiceContextWithCustomTransportLayer ctx = new DataServiceContextWithCustomTransportLayer(requestMessage, responseMessage);
                ctx.EnableAtom = true;

                DataServiceRequestException ex = TestUtil.RunCatching<DataServiceRequestException>(() => RunTestCase(ctx, responseMessage, testCase.EntryCount, testCase.ValidTopLevelFeed));

                bool expectedException = testCase.ExpectedInnerException != null;
                TestUtil.AssertExceptionExpected(ex, expectedException);

                if (expectedException)
                {
                    InvalidOperationException innerEx = ex.InnerException as InvalidOperationException;
                    Assert.IsNotNull(innerEx, "Expected invalid operation exception in the inner exception");
                    Assert.AreEqual(testCase.ExpectedInnerException.Message, innerEx.Message);
                }
            });
        }

        private static void RunTestCase(DataServiceContext ctx, ODataTestMessage responseMessage, int entryCount, bool validTopLevelFeed)
        {
            responseMessage.StatusCode = 200;
            responseMessage.SetHeader("Content-Type", "application/atom+xml;charset=utf-8");
            string payloadHeader =
@"<?xml version=""1.0"" encoding=""utf-8""?>";

            StringBuilder entryPayloadBuilder = new StringBuilder();
            for (int i = 1; i <= entryCount; i++)
            {
                entryPayloadBuilder.Append(GetEntry(i));
            }

            string feedPayloadFormat =
@"<feed{0}>
<title type=""text"">Entities</title>
<updated>2010-01-27T18:06:26Z</updated>
{1}
</feed>";

            string feedPayload = string.Format(
                feedPayloadFormat,
                validTopLevelFeed ? @" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom""" : String.Empty,
                entryPayloadBuilder.ToString());

            responseMessage.WriteToStream(payloadHeader + feedPayload);

            var order = new Order() { ID = 1 };
            ctx.AttachTo("Orders", order);
            ctx.UpdateObject(order);

            Assert.AreEqual(0, order.DollarAmount, "order.DollarAmount should be 0 before SaveChanges.");
            ctx.SaveChanges();
            Assert.AreEqual(1, order.DollarAmount, "order.DollarAmount should have been updated on the Order.");
        }

        private static string GetEntry(int id)
        {
            string entryXml =
@"<entry xml:base=""http://sparradev2:25484/TheTest/"" xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns:georss=""http://www.georss.org/georss"" xmlns:gml=""http://www.opengis.net/gml"">
<id>http://sparradev2:25484/TheTest/Orders({0})</id>
<category term=""#AstoriaUnitTests.Stubs.Order"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
<link rel=""edit"" title=""Order"" href=""Orders({0})"" />
<link rel=""http://docs.oasis-open.org/odata/ns/related/Customer"" type=""application/atom+xml;type=entry"" title=""Customer"" href=""Orders({0})/Customer"" />
<link rel=""http://docs.oasis-open.org/odata/ns/related/OrderDetails"" type=""application/atom+xml;type=feed"" title=""OrderDetails"" href=""Orders({0})/OrderDetails"" />
<title />
<updated>2011-11-04T22:15:32Z</updated>
<author><name /></author>
<content type=""application/xml"">
  <m:properties>
    <d:ID m:type=""Edm.Int32"">{0}</d:ID>
    <d:DollarAmount m:type=""Edm.Double"">{0}</d:DollarAmount>
    <d:CurrencyAmount m:type=""AstoriaUnitTests.Stubs.CurrencyAmount"" m:null=""true"" />
  </m:properties>
</content>
</entry>";

            return string.Format(entryXml, id);
        }

        [TestMethod]
        public void ClientShouldMaterializeEmptyInlineAsEmptyCollection()
        {
            // [Client-ODataLib-Integration] Client reads empty m:inline as empty feed, server doesn't.
            //// Client should materialize the empty m:inline as empty collection
            string payloadHeader = @"<?xml version=""1.0"" encoding=""utf-8""?>";

                string payload = @"<entry " + TestUtil.CommonPayloadNamespaces + @">
                            <id>" + DataServiceContextWithCustomTransportLayer.DummyUri + @"/Customers(1)</id>
                            <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='#AstoriaUnitTests.Stubs.Customer'/>
                                <link rel='edit' title='Customer' href='Customers(1)' />
                                <link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)'><adsm:inline /></link>
                                <content type='application/xml'>
                                    <adsm:properties>
                                        <ads:ID adsm:type='Edm.Int32'>42</ads:ID>
                                    </adsm:properties>
                            </content>
                            </entry>";

                string feedPayloadFormat =
@"<feed{0}>
<title type=""text"">Customers</title>
<updated>2010-01-27T18:06:26Z</updated>
{1}
</feed>";
            string feedPayload = string.Format(
                    feedPayloadFormat,
                    @" xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'",
                    payload);

            var ctx = CreateContextWithTransportLayer(payloadHeader + feedPayload);
            ctx.EnableAtom = true;
            var customer = ctx.CreateQuery<Customer>("Customers").FirstOrDefault();

            Assert.IsNotNull(customer, "Failed, customer object should not be null");
            Assert.AreEqual(0, customer.Orders.Count, "Failed to verify the Orders collection");
        }

        [TestMethod]
        public void AstoriaClientShouldFailIfEditMediaLinkIsPresent()
        {
            // [Client-ODataLib-Integration] Astoria client doesn't fail if edit-media link is present on otherwise non-MLE entity
            //// We'll need to maintain the existing client behavior under the client knob.
            // We took a breaking change in v4 for this.
            string payloadHeader = @"<?xml version=""1.0"" encoding=""utf-8""?>";

            string payload = @"<entry " + TestUtil.CommonPayloadNamespaces + @">
                        <id>" + DataServiceContextWithCustomTransportLayer.DummyUri + @"/Orders(1)</id>
                        <link rel='edit-media' title='Orders' href='Orders(1)/$value' />
                        <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='#AstoriaUnitTests.Stubs.Order'/>
                            <content type='application/xml'>
                                <adsm:properties>
                                    <ads:ID adsm:type='Edm.Int32'>99</ads:ID>
                                </adsm:properties>
                        </content>
                        </entry>";

            string feedPayloadFormat =
@"<feed{0}>
<title type=""text"">Orders</title>
<updated>2010-01-27T18:06:26Z</updated>
{1}
</feed>";
            string feedPayload = string.Format(
                feedPayloadFormat,
                @" xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'",
                payload);

            var ctx = CreateContextWithTransportLayer(payloadHeader + feedPayload);
            ctx.EnableAtom = true;

            try
            {
                ctx.CreateQuery<Order>("Orders").FirstOrDefault();
                Assert.Fail("Expected exception, but none occured");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ODataLibResourceUtil.GetString("ODataAtomReader_MediaLinkEntryMismatch"), ex.Message, "Exception happened, but the message was not as expected.");
            }
        }

        public class TestEntity
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        [TestMethod]
        public void EpmKeepInContentShouldNotReadAtom()
        {
            // AstoriaClient-ODL-Integration - when KIC, do not read property from mapped element
            // In V3, we are making a change to ONLY apply the out of content elements when the EPM KeepInContent is FALSE
            // Otherwise, if EPM specified KIC=true, then we should only apply the in content value, even if it doesn't exist and one exists outside of content.
            var requestMessage = new ODataTestMessage();
            var responseMessage = new ODataTestMessage();
            responseMessage.StatusCode = 200;
            responseMessage.SetHeader("Content-Type", UnitTestsUtil.AtomFormat);
            responseMessage.SetHeader("Content-ID", "1");

            string payload =
@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<feed xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
<title type=""text"">Entities</title>
<updated>2010-01-27T18:06:26Z</updated>
<link rel=""self"" title=""Entities"" href=""Entities"" />
<entry>
    <id>http://localhost:62614/TheTest/Entities(16584)</id>
    <title type=""text""></title>
    <updated>2009-09-30T01:44:35Z</updated>
    <author>
        <name>Foo</name>
    </author>
    <link rel=""edit"" title=""Order"" href=""Orders(16584)"" />
    <category term=""#TestEntity"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
    <content type=""application/xml"">
        <m:properties>
            <d:ID>0</d:ID>
        </m:properties>
    </content>
</entry>";

            responseMessage.WriteToStream(payload);
            DataServiceContextWithCustomTransportLayer ctx = new DataServiceContextWithCustomTransportLayer(requestMessage, responseMessage);
            ctx.EnableAtom = true;
            var entity = ctx.CreateQuery<TestEntity>("Entities").FirstOrDefault();
            Assert.IsNull(entity.Name);
        }

        #region [Client-ODataLib-Integration] Complex value without any properties is read as null in client

        [TestMethod]
        public void EmptyComplexValueShouldNotReadAsNull()
        {
            // [Client-ODataLib-Integration] Complex value without any properties is read as null in client
            string payload =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<feed xmlns='http://www.w3.org/2005/Atom'>
    <title type='text'>Orders</title>
    <updated>2010-01-27T18:06:26Z</updated>
    <entry " + TestUtil.CommonPayloadNamespaces + @">
        <id>" + DataServiceContextWithCustomTransportLayer.DummyUri + @"/Customers(1)</id>
        <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='#AstoriaUnitTests.Stubs.Customer'/>
        <content type='application/xml'>
            <adsm:properties>
                <ads:ID adsm:type='Edm.Int32'>100</ads:ID>
                <ads:Address adsm:type='#AstoriaUnitTests.Stubs.Address'>foo</ads:Address>
            </adsm:properties>
        </content>
    </entry>
</feed>";

            var ctx = CreateContextWithTransportLayer(payload);
            ctx.EnableAtom = true;
            var customer = ctx.CreateQuery<Customer>("Customers").First();

            // Updated: verify customer.Address is not null and contains default values that get set in the Customer constructor
            Assert.IsNotNull(customer.Address, "customer.Address should not be null");

            Customer expectedCustomer = new Customer();
            Address expectedAddress = expectedCustomer.Address;
            Address actualAddress = customer.Address;

            Assert.AreEqual(expectedAddress.City, customer.Address.City, "Customer.Address.City has the wrong value, expected the default from the Customer constructor.");
            Assert.AreEqual(expectedAddress.PostalCode, actualAddress.PostalCode, "Customer.Address.PostalCode has the wrong value, expected the default from the Customer constructor.");
            Assert.AreEqual(expectedAddress.State, actualAddress.State, "Customer.Address.State has the wrong value, expected the default from the Customer constructor.");
            Assert.AreEqual(expectedAddress.StreetAddress, actualAddress.StreetAddress, "Customer.Address.StreetAddress has the wrong value, expected the default from the Customer constructor.");
        }

        #endregion

        [TestMethod]
        public void AstoriaClientShouldNotThrowIfFirstXmlBaseAttributeNotAbsolute()
        {
            // [Client-ODataLib-Integration] Astoria client throws if the first xml base attribute specified is not an absolute one
            //// We decided to allow this and use the BaseUri from the settings even in default ODL.
            //// For client this is going to be a relaxing change which is perfectly fine.

            string payloadHeader = @"<?xml version=""1.0"" encoding=""utf-8""?>";

            string payload = @"<entry xml:base='/baserelative/' " + TestUtil.CommonPayloadNamespaces + @">
                        <id>" + DataServiceContextWithCustomTransportLayer.DummyUri + @"/Orders(1)</id>
                        <link title='Orders' href='Orders(1)/$value' />
                        <link rel='edit' href='Orders(1)' />
                        <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='#AstoriaUnitTests.Stubs.Order'/>
                            <content type='application/xml'>
                                <adsm:properties>
                                    <ads:ID adsm:type='Edm.Int32'>1</ads:ID>
                                </adsm:properties>
                        </content>
                        </entry>";

            string feedPayloadFormat =
@"<feed{0}>
<title type=""text"">Orders</title>
<updated>2010-01-27T18:06:26Z</updated>
{1}
</feed>";
            string feedPayload = string.Format(
                feedPayloadFormat,
                @" xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'",
                payload);

            var ctx = CreateContextWithTransportLayer(payloadHeader + feedPayload);
            ctx.EnableAtom = true;

            Exception expectedException = TestUtil.RunCatching(() => { ctx.CreateQuery<Order>("Orders").FirstOrDefault(); });

            Assert.IsNull(expectedException);
            Uri expectedEditLink = new Uri(new Uri(DataServiceContextWithCustomTransportLayer.DummyUri, "/baserelative/"), "Orders(1)");
            Assert.AreEqual(expectedEditLink, ctx.Entities[0].EditLink, "identity must match");
        }

        #region Astoria client does not require navigation properties to be part of the model

        public class EntityA
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        [TestMethod]
        public void AstoriaClientShouldRequireNavigationPropertiesBePartOfModel()
        {
            // [Client-ODataLib-Integration] Astoria client does not require navigation properties to be part of the model
            // Resolution : Fixed
            // Expected behavior :
            // Undeclared primitive/complex/collection property, 
            // - If IgnoreMissingProperties is off (default) - fail 
            // - If IgnoreMissingProperties is on - don't read such property (don't even parse it).
            // Undeclared deferred navigation property, 
            // - Regardless of settings always read it and populate LinkInfos
            // Undeclared expanded navigation property, 
            // - If IgnoreMissingProperties is off (default) - fail 
            // - If IgnoreMissingProperties in on - read the link and populate LinkInfos, but ignore the expanded content (don't even parse it).
            // Undeclared association link, 
            // - Regardless of settings always read it and populate LinkInfos
            // Undeclared stream property,
            // - Regardless of settings always read it and populate LinkInfos

            const string atomPayloadTemplate =
@"<feed xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'>
  <m:count>3</m:count>
  <title type='text'>ASet</title>
  <id>http://host/ASet</id>
  <updated>2010-09-01T23:36:00Z</updated>
  <link rel='self' title='ASet' href='ASet' />
  <entry>
    <id>http://host/ASet(1)</id>
    <title type='text'></title>
    <updated>2010-09-01T23:36:00Z</updated>
    <author>
      <name />
    </author>
    {0}
  </entry>
</feed>";

            var tests = new[]
                {
                    new UndeclaredPropertyTestCase()
                    {
                        Type = "UndeclaredProperty",
                        IgnoreMissingProperties = false,
                        Payload = @"    
          <content type='application/xml' >
             <m:properties>
                <d:Test m:type='Edm.Int32'>1</d:Test>
             </m:properties>
          </content>",
                      ExpectFailure = true
                    },

                    new UndeclaredPropertyTestCase()
                    {
                        Type = "UndeclaredProperty2",
                        IgnoreMissingProperties = true,
                        Payload = @"    
          <content type='application/xml' >
             <m:properties>
                <d:Test m:type='Edm.Int32'>1</d:Test>
             </m:properties>
          </content>",
                     ExpectFailure = false
                    },

                    new  UndeclaredPropertyTestCase()
                    {
                        Type = "UndeclaredDeferredNavigationLink",
                        IgnoreMissingProperties = false,
                        Payload = @"
    <link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=entry'/>",
                        LinkCount = 1,
                        ExpectFailure = false
                    },

                    new  UndeclaredPropertyTestCase()
                    {
                        Type = "UndeclaredDeferredNavigationLink2",
                        IgnoreMissingProperties = true,
                        Payload = @"
    <link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=entry'/>
    <link rel='http://docs.oasis-open.org/odata/ns/related/NavProp2' href='http://odata.org/link2' type='application/atom+xml;type=feed'/>",
                        LinkCount = 2,
                        ExpectFailure = false
                    },

                    new  UndeclaredPropertyTestCase()
                    {
                        Type = "UndeclaredExpandedNavigationLink",
                        IgnoreMissingProperties = false,
                        Payload = @"
      <link rel='http://docs.oasis-open.org/odata/ns/related/B' type='application/atom+xml;type=entry' title='B' href='Customer(1)/B'>
      <m:inline>
        <entry>
          <id>http://host/BSet(2)</id>
          <content type='application/xml' >
             <m:properties>
                <d:ID m:type='Edm.Int32'>2</d:ID>
                <d:Prop1 m:type='Edm.Int32'>5</d:Prop1>
             </m:properties>
          </content>
        </entry>
      </m:inline>
      <m:inline/>
      </link>",
                        ExpectFailure = true
                    },

                    new  UndeclaredPropertyTestCase()
                    {
                        Type = "UndeclaredExpandedNavigationLink2",
                        IgnoreMissingProperties = true,
                        Payload = @"
      <link rel='http://docs.oasis-open.org/odata/ns/related/B' type='application/atom+xml;type=entry' title='B' href='Customer(1)/B'>
      <m:inline>
        <entry>
          <id>http://host/BSet(2)</id>
          <content type='application/xml' >
             <m:properties>
                <d:ID m:type='Edm.Int32'>2</d:ID>
                <d:Prop1 m:type='Edm.Int32'>5</d:Prop1>
             </m:properties>
          </content>
        </entry>
      </m:inline>
      <m:inline/>
      </link>",
                        LinkCount = 1,
                        ExpectFailure = false
                    },

                    new  UndeclaredPropertyTestCase()
                    {
                        Type = "UndeclaredStream",
                        IgnoreMissingProperties = false,
                        Payload = @"
    <link rel='http://docs.oasis-open.org/odata/ns/mediaresource/MediaResourceStream' href='http://odata.org/readlink' type='mime/type'/>",
                        LinkCount = 1,
                        ExpectFailure = false
                    },

                    new  UndeclaredPropertyTestCase()
                    {
                        Type = "UndeclaredStream2",
                        IgnoreMissingProperties = true,
                        Payload = @"
    <link rel='http://docs.oasis-open.org/odata/ns/edit-media/NamedStream' href='http://odata.org/editlink' type='mime/type'/>
    <link rel='http://docs.oasis-open.org/odata/ns/mediaresource/MediaResourceStream' href='http://odata.org/readlink' type='mime/type'/>",
                        LinkCount = 2,
                        ExpectFailure = false
                    },
                };

            TestUtil.RunCombinations(tests, (test) =>
            {
                var ctx = CreateContextWithTransportLayer(string.Format(atomPayloadTemplate, test.Payload), ODataProtocolVersion.V4);
                ctx.EnableAtom = true;
                ctx.IgnoreMissingProperties = test.IgnoreMissingProperties;

                EntityA entity = null;
                Exception ex = TestUtil.RunCatching(() => entity = ctx.CreateQuery<EntityA>("ASet").FirstOrDefault());

                if (test.ExpectFailure)
                {
                    Assert.IsNotNull(ex, test.Type + " expects an exception but got none.");
                }
                else
                {
                    Assert.IsNull(ex, test.Type + " expects to pass but got an exception: " + ex);

                    if (!test.Type.Contains("Property"))
                    {
                        EntityDescriptor descriptor = ctx.Entities.First();
                        int actualCount = 0;

                        if (test.Type.Contains("Link"))
                        {
                            actualCount = descriptor.LinkInfos.Count;
                        }
                        else
                        {
                            actualCount = descriptor.StreamDescriptors.Count;
                        }

                        Assert.AreEqual(test.LinkCount, actualCount, "Number of Link populated is incorrect for " + test.Type);
                    }
                }
            });
        }

        private class UndeclaredPropertyTestCase
        {
            public UndeclaredPropertyTestCase()
            {
            }

            public string Type { get; set; }
            public bool IgnoreMissingProperties { get; set; }
            public string Payload { get; set; }
            public int LinkCount { get; set; }
            public bool ExpectFailure { get; set; }
        }

        #endregion

        [TestMethod]
        public void V3ODataSpecCompliance_ATOMNamespaceXmlParsing()
        {
            // Validate in V3 that parsing fails in V3 when atom elements use atom term
            string payloadHeader = @"<?xml version=""1.0"" encoding=""utf-8""?>";

                string feedPayload = @"<feed 
  xmlns:d='http://docs.oasis-open.org/odata/ns/data' 
  xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' 
  xmlns:atom='http://www.w3.org/2005/Atom'>
  <title type=""text"">Products</title>
  <updated>2010-01-27T18:06:26Z</updated>
  <entry xml:base=""" + DataServiceContextWithCustomTransportLayer.DummyUri + @""">
    <id>" + DataServiceContextWithCustomTransportLayer.DummyUri + @"/Products(1)</id>
    <link title='Product' href=""Products(1)/$value"" />
    <category atom:scheme=""http://docs.oasis-open.org/odata/ns/scheme"" atom:term=""AstoriaUnitTests.Stubs.Product"" />
    <content type=""application/xml"">
      <m:properties>
        <d:ID>1</d:ID>
        <d:ProductName>My Product</d:ProductName>
        <d:Discontinued>false</d:Discontinued>
      </m:properties>
    </content>
  </entry>
</feed>";

            var ctx = CreateContextWithTransportLayer(payloadHeader + feedPayload);

            Exception expectedException = TestUtil.RunCatching(() => { ctx.CreateQuery<Product>("Products").FirstOrDefault(); });
            ValidateErrorMessageWithParameters(expectedException.InnerException, typeof(InvalidOperationException), "AtomMaterializer_InvalidResponsePayload", "http://docs.oasis-open.org/odata/ns/data");

            ctx = CreateContextWithTransportLayer(payloadHeader + feedPayload, ODataProtocolVersion.V4);
            expectedException = TestUtil.RunCatching(() => { ctx.CreateQuery<Product>("Products").FirstOrDefault(); });
            ValidateErrorMessageWithParameters(expectedException.InnerException, typeof(InvalidOperationException), "AtomMaterializer_InvalidResponsePayload", "http://docs.oasis-open.org/odata/ns/data");
        }

        [TestMethod]
        public void V3ODataSpecCompliance_AtomNamespaceParsing()
        {
            // Validate in V3 that parsing ignores comments
            string payloadHeader = @"<?xml version=""1.0"" encoding=""utf-8""?>";

            string feedPayload = @"<feed 
  xmlns:d='http://docs.oasis-open.org/odata/ns/data' 
  xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' 
  xmlns='http://www.w3.org/2005/Atom'>
  <title type=""text"">Products</title>
  <updated>2010-01-27T18:06:26Z</updated>
  <entry xml:base=""" + DataServiceContextWithCustomTransportLayer.DummyUri + @""">
    <id>" + DataServiceContextWithCustomTransportLayer.DummyUri + @"/Products(1)</id>
    <link title='Product' href=""Products(1)/$value"" />
    <category scheme=""http://docs.oasis-open.org/odata/ns/scheme"" term=""AstoriaUnitTests.Stubs.Product""/>
    <content type=""application/xml"">
      <m:properties>
        <d:ID>1</d:ID>
        <d:ProductName><?xml-stylesheet type='type' href='uri' ?><?comment?>My Product</d:ProductName>
        <d:Discontinued>false</d:Discontinued>
      </m:properties>
    </content>
  </entry>
</feed>";

            string payload = payloadHeader + feedPayload;
            var ctx = CreateContextWithTransportLayer(payload);
            ctx.EnableAtom = true;
            var result = ctx.CreateQuery<Product>("Products").FirstOrDefault();

            // Validate no exception
            Assert.AreEqual(result.ProductName, "My Product");

            ctx = CreateContextWithTransportLayer(payload, ODataProtocolVersion.V4);
            ctx.EnableAtom = true;
            result = ctx.CreateQuery<Product>("Products").FirstOrDefault();

            // Validate no exception
            Assert.AreEqual(result.ProductName, "My Product");
        }

        [TestMethod]
        public void V3ODataSpecCompliance_AtomContent()
        {
            // Validate in V3 that invalid ATOM content is ignored
            string payloadHeader = @"<?xml version=""1.0"" encoding=""utf-8""?>";

                string feedPayload = @"<feed 
  xmlns:d='http://docs.oasis-open.org/odata/ns/data' 
  xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' 
  xmlns='http://www.w3.org/2005/Atom'>
  <title type=""text"">Products</title>
  <updated>2010-01-27T18:06:26Z</updated>
  <entry xml:base=""" + DataServiceContextWithCustomTransportLayer.DummyUri + @""">
    <id>" + DataServiceContextWithCustomTransportLayer.DummyUri + @"/Products(1)</id>
    <link title='Product' href=""Products(1)/$value"" />
    <category scheme=""http://docs.oasis-open.org/odata/ns/scheme"" term=""AstoriaUnitTests.Stubs.Product""/>
    <content type=""application/pdf"">
      <m:properties>
        <d:ID>1</d:ID>
        <d:ProductName>My Product</d:ProductName>
        <d:Discontinued>false</d:Discontinued>
      </m:properties>
    </content>
  </entry>
</feed>";

            // No exception, values are not parsed
            string payload = payloadHeader + feedPayload;
            var ctx = CreateContextWithTransportLayer(payload);
            ctx.EnableAtom = true;

            var expectedException = TestUtil.RunCatching(() => ctx.CreateQuery<Product>("Products").FirstOrDefault());
            ValidateODataErrorMessageWithParameters(expectedException, typeof(InvalidOperationException), "ODataAtomEntryAndFeedDeserializer_ContentWithWrongType", "application/pdf");
        }

        [TestMethod]
        public void V3ODataSpecCompliance_Duplicates()
        {
            // Validate in V3 that the client allows duplicates and doesn't fail
            // Took a breaking change in v4 that this fails
            string payloadHeader = @"<?xml version=""1.0"" encoding=""utf-8""?>";

            // Duplicate properties
            var feedPayload = @"<feed 
  xmlns:d='http://docs.oasis-open.org/odata/ns/data' 
  xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' 
  xmlns='http://www.w3.org/2005/Atom'>
  <title type=""text"">Products</title>
  <updated>2010-01-27T18:06:26Z</updated>
  <entry xml:base=""" + DataServiceContextWithCustomTransportLayer.DummyUri + @""">
    <id>" + DataServiceContextWithCustomTransportLayer.DummyUri + @"/Products(1)</id>
    <link title='Product' href=""Products(1)/$value"" />
    <category scheme=""http://docs.oasis-open.org/odata/ns/scheme"" term=""AstoriaUnitTests.Stubs.Product""/>
    <content type=""application/xml"">
      <m:properties>
        <d:ID>1</d:ID>
        <d:ProductName>My Product1</d:ProductName>
        <d:Discontinued>false</d:Discontinued>
      </m:properties>
      <m:properties>
        <d:ID>2</d:ID>
        <d:ProductName>My Product2</d:ProductName>
        <d:Discontinued>false</d:Discontinued>
      </m:properties>
    </content>
  </entry>
</feed>";

            string payload = payloadHeader + feedPayload;
            var ctx = CreateContextWithTransportLayer(payload);
            ctx.EnableAtom = true;
            Exception expectedException = TestUtil.RunCatching(() => { ctx.CreateQuery<Product>("Products").FirstOrDefault(); });
            ValidateODataErrorMessageWithParameters(expectedException, typeof(InvalidOperationException), "ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://docs.oasis-open.org/odata/ns/metadata", "properties");

            // Duplicate contents
            feedPayload = @"<feed 
  xmlns:d='http://docs.oasis-open.org/odata/ns/data' 
  xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' 
  xmlns='http://www.w3.org/2005/Atom'>
  <title type=""text"">Products</title>
  <updated>2010-01-27T18:06:26Z</updated>
  <entry xml:base=""" + DataServiceContextWithCustomTransportLayer.DummyUri + @""">
    <id>" + DataServiceContextWithCustomTransportLayer.DummyUri + @"/Products(1)</id>
    <link title='Product' href=""Products(1)/$value"" />
    <category scheme=""http://docs.oasis-open.org/odata/ns/scheme"" term=""AstoriaUnitTests.Stubs.Product""/>
    <content type=""application/xml"">
      <m:properties>
        <d:ID>1</d:ID>
        <d:ProductName>My Product</d:ProductName>
        <d:Discontinued>false</d:Discontinued>
      </m:properties>
    </content>
    <content type=""application/xml"">
      <m:properties>
        <d:ID>2</d:ID>
        <d:ProductName>My Product2</d:ProductName>
        <d:Discontinued>false</d:Discontinued>
      </m:properties>
    </content>
  </entry>
</feed>";

            payload = payloadHeader + feedPayload;
            ctx = CreateContextWithTransportLayer(payload);
            ctx.EnableAtom = true;

            expectedException = TestUtil.RunCatching(() => { ctx.CreateQuery<Product>("Products").FirstOrDefault(); });
            ValidateODataErrorMessageWithParameters(expectedException, typeof(InvalidOperationException), "ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://docs.oasis-open.org/odata/ns/metadata", "properties");
        }

        [TestMethod]
        public void ClientShouldIgnoreResponseWithInStreamError()
        {
            // If server sends a response to a PUT/PATCH that contains an in-stream error, the client should not ignore it
            string payload = @"
<entry xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'>
   <id>http://localhost:3000/northwind.svc/Order(1)</id>
    <category term='#AstoriaUnitTests.Stubs.Order' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
    <content type='application/xml'>
     <m:properties>
      <d:ID m:type='Edm.Int32'>1</d:ID>
      <m:error xmlns:m='http://docs.oasis-open.org/odata/ns/metadata'><m:message xml:lang='en-US'>broken</m:message></m:error>";

            var requestMessage = new ODataTestMessage();
            var responseMessage = new ODataTestMessage();
            responseMessage.StatusCode = 200;
            responseMessage.SetHeader("Content-Type", UnitTestsUtil.AtomFormat);
            responseMessage.SetHeader("Preference-Applied", "return=representation");
            responseMessage.SetHeader("OData-Version", "4.0;");
            responseMessage.WriteToStream(payload);

            var ctx = new DataServiceContextWithCustomTransportLayer(ODataProtocolVersion.V4, requestMessage, responseMessage);
            ctx.EnableAtom = true;
            ctx.Format.UseAtom();

            ctx.AddAndUpdateResponsePreference = DataServiceResponsePreference.IncludeContent;

            var order = new Order() { ID = 1 };
            ctx.AttachTo("Orders", order);
            ctx.UpdateObject(order);

            Exception expectedException = TestUtil.RunCatching(() => { ctx.SaveChanges(); });

            ValidateErrorMessageWithParameters(expectedException.InnerException, typeof(DataServiceClientException), "Deserialize_ServerException", "broken");
        }

        [TestMethod]
        public void ClientShouldFailIfPrimitiveTypeNotMatch()
        {
            // Making sure in projection, if the primitive type do not match, client fails gracefully
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.StartService();

                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot);
                ctx.EnableAtom = true;
                ctx.Format.UseAtom();

                ctx.ResolveType = ((name) =>
                {
                    Debug.Assert(name.Contains("Customer"), "name.Contains(\"Customer\")");
                    return typeof(NarrowCustomer);
                });

                var q = from c in ctx.CreateQuery<Customer>("Customers")
                        select new Customer()
                        {
                            ID = c.ID,
                            GuidValue = c.GuidValue,
                        };

                // NOTE: As part of changing the layering between the client and ODL to be a more straight-forward
                // parse-then-materialize, the client had to start converting values that did not match the client
                // types. This is primarily because the server model might not match, but also includes the case
                // that this test was written for. So this is no longer an error test.
                var ex = TestUtil.RunCatching(() => q.ToList());
                Assert.IsNull(ex);
            }
        }

        private class NarrowCustomer
        {
            public short ID { get; set; }
            public string GuidValue { get; set; }
        }

        [TestMethod]
        public void AstoriaClientShouldBeMoreStrictReadingPrimitivesThanServer()
        {
            // [Client-ODataLib-Integration] Astoria client is more strict when reading primitive values from XML than Astoria server
            string payloadHeader = @"<?xml version=""1.0"" encoding=""utf-8""?>";

            string feedPayload = @"<atom:feed 
  xmlns:d='http://docs.oasis-open.org/odata/ns/data' 
  xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' 
  xmlns:atom='http://www.w3.org/2005/Atom'>
  <atom:title type=""text"">Products</atom:title>
  <atom:updated>2010-01-27T18:06:26Z</atom:updated>
  <atom:entry xml:base=""" + DataServiceContextWithCustomTransportLayer.DummyUri + @""">
    <atom:id>" + DataServiceContextWithCustomTransportLayer.DummyUri + @"/Products(1)</atom:id>
    <atom:link atom:title='Product' atom:href=""Products(1)/$value"" />
    <atom:category scheme=""http://docs.oasis-open.org/odata/ns/scheme"" term=""AstoriaUnitTests.Stubs.Product"" />
    <atom:content type=""application/xml"">
      <m:properties>
        <d:ID><?xml-stylesheet type='type' href='uri' ?>1</d:ID>
        <d:ProductName><?comment?><![CDATA[cdata]]>My Product</d:ProductName>
        <d:Discontinued>false</d:Discontinued>
      </m:properties>
    </atom:content>
  </atom:entry>
</atom:feed>";

            var ctx = CreateContextWithTransportLayer(payloadHeader + feedPayload);
            ctx.EnableAtom = true;
            var result = ctx.CreateQuery<Product>("Products").FirstOrDefault();

            Assert.AreEqual(result.ProductName, "cdataMy Product");
            Assert.AreEqual(result.ID, 1);
        }

        [TestMethod]
        public void ClientShouldOverwriteDefaultMessageQuotasLimits()
        {
            // Client should overwrite ODL default limits for messageQuotas
            DataServiceContext context = CreateContextWithTransportLayer(null, ODataProtocolVersion.V4);

            for (int i = 0; i < 1001; i++)
            {
                context.AddObject("Customer", new CustomDataClient.Customer() { ID = i, Name = ("foo" + i) });
            }                
            Exception ex = TestUtil.RunCatching(() => context.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset));
            Assert.AreNotEqual("The current change set contains too many operations. Only change sets with a maximum number of '1000' operations are allowed.", ex.Message, "Client failed to override ODL default value for maxOperationsPerChangeset");

            List<DataServiceRequest> batchRequests = new List<DataServiceRequest>();
            for (int i = 0; i < 101; i++)
            {
                batchRequests.Add(new DataServiceRequest<CustomDataClient.Customer>(new Uri(DataServiceContextWithCustomTransportLayer.DummyUri + "/foo" + i)));
            }
            ex = TestUtil.RunCatching(() => context.ExecuteBatch(batchRequests.ToArray()));
            if (ex != null)
            {
                Assert.AreNotEqual("The current batch message contains too many parts. Only batch messages with a maximum number of '100' query operations and change sets are allowed.", ex.Message, "Client failed to override ODL default value for maxPartsPerBatch");
            }
        }

        [TestMethod]
        public void ShouldThrowIfUsingJsonLightWithoutModel()
        {
            // InvalidOperationException if client user try to use jsonlight but doesn't provide IEdmModel and the model is used by odatalib
            // Specifying $format in the url causes client to throw InvalidOperationException
            using (OpenWebDataServiceHelper.MaxProtocolVersion.Restore())
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                OpenWebDataServiceHelper.MaxProtocolVersion.Value = ODataProtocolVersion.V4;
                request.DataServiceType = typeof(CustomDataContext);
                request.StartService();

                var context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);

                // Atom is the default, but setting it explicitly to ensure we are testing the right scenario
                context.EnableAtom = true;
                context.Format.UseAtom();
                Exception ex = TestUtil.RunCatching(() => context.Execute<Customer>(new Uri(request.ServiceRoot + "/Customers?$format=json")));
                Assert.IsNotNull(ex, "Expected an exception, but none thrown");
                Assert.IsInstanceOfType(ex, typeof(InvalidOperationException));
                Assert.IsNotNull(ex.InnerException, "Expected an exception, but none thrown");
                Assert.AreEqual(DataServicesClientResourceUtil.GetString("DataServiceClientFormat_ValidServiceModelRequiredForJson"), ex.InnerException.Message);
                Assert.IsInstanceOfType(ex.InnerException, typeof(InvalidOperationException));
            }
        }

        [TestMethod]
        public void ChangeAcceptHeaderToJsonInSendingRequestWithoutModel()
        {
            using (OpenWebDataServiceHelper.MaxProtocolVersion.Restore())
            using (var request = TestWebRequest.CreateForInProcessWcf())
            {
                OpenWebDataServiceHelper.MaxProtocolVersion.Value = ODataProtocolVersion.V4;
                request.DataServiceType = typeof(CustomDataContext);
                request.StartService();

                var context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);

                // Atom is the default, but setting it explicitly to ensure we are testing the right scenario
                context.EnableAtom = true;
                context.Format.UseAtom();
                context.SendingRequest2 += (c, args) => args.RequestMessage.SetHeader("Accept", "application/json");
                var ex = TestUtil.RunCatching(() => context.Execute<Customer>(new Uri(request.ServiceRoot + "/Customers")));
                Assert.IsNotNull(ex, "Expected an exception, but none thrown");
                Assert.IsInstanceOfType(ex, typeof(InvalidOperationException));
                Assert.IsNotNull(ex.InnerException, "Expected an exception, but none thrown");
                Assert.AreEqual(DataServicesClientResourceUtil.GetString("DataServiceClientFormat_ValidServiceModelRequiredForJson"), ex.InnerException.Message);
                Assert.IsInstanceOfType(ex.InnerException, typeof(InvalidOperationException));
            }
        }

        private static void ValidateErrorMessageWithParameters(Exception exc, Type exceptionType, string messageName, params string[] parameters)
        {
            string expectedMessage = DataServicesClientResourceUtil.GetString(messageName, parameters);
            ValidateErrorMessage(exc, exceptionType, expectedMessage);
        }

        private static void ValidateODataErrorMessageWithParameters(Exception exc, Type exceptionType, string messageName, params string[] parameters)
        {
            string expectedMessage = ODataLibResourceUtil.GetString(messageName, parameters);
            ValidateErrorMessage(exc, exceptionType, expectedMessage);
        }

        private static void ValidateErrorMessage(Exception exc, Type exceptionType, string errorMessage)
        {
            Assert.IsNotNull(exc, "expected exception, but none was thrown");
            Assert.AreEqual(exceptionType, exc.GetType(), "the exception is not of the expected type");
            Assert.AreEqual(errorMessage, exc.Message);
        }

        private static DataServiceContext CreateContextWithTransportLayer(string payload, ODataProtocolVersion protocolVersion = ODataProtocolVersion.V4)
        {
            var requestMessage = new ODataTestMessage();
            var responseMessage = new ODataTestMessage();
            responseMessage.SetHeader("Content-Type", UnitTestsUtil.AtomFormat);
            if (payload != null)
            {
                responseMessage.StatusCode = 200;
                responseMessage.WriteToStream(payload);
            }
            else
            {
                responseMessage.StatusCode = 204;
            }

            return new DataServiceContextWithCustomTransportLayer(protocolVersion, requestMessage, responseMessage);
        }
    }
}

namespace V3ODataSpecComplianceChangeSet
{
    using System;

    public class Entity
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
    }
}
