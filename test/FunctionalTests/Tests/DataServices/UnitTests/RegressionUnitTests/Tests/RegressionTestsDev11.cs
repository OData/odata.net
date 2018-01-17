//---------------------------------------------------------------------
// <copyright file="RegressionTestsDev11.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Objects;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Configuration;
    using Microsoft.OData.Service.Providers;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using System.Xml.Linq;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NorthwindModel;
    using ocs = AstoriaUnitTests.ObjectContextStubs;
    using Providers = Microsoft.OData.Service.Providers;
    using Microsoft.OData.Client;

    #endregion Namespaces

    [TestModule]
    public partial class RegressionUnitTestModule : AstoriaTestModule
    {
        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/876
        /// <summary>This is a test class for adding regression tests.</summary>
        [TestClass, TestCase]
        public class RegressionTestDev11 : AstoriaTestCase
        {
            [ClassInitialize()]
            public static void PerClassSetup(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext context)
            {
                AstoriaTestProperties.Host = Host.Cassini;
            }

            #region Service Operations

            [Ignore] // Remove Atom
            // [TestMethod, Variation("Service Operation rights is not checked, service operation always invoke even if there are errors in query syntax. Also Exception message should be more user friendly.")]
            public void ServiceOpInvokeAndRights()
            {
                string queryPortionNotEmpty = DataServicesResourceUtil.GetString("RequestUriProcessor_SegmentDoesNotSupportKeyPredicates");
                string queryOptionNotEmpty = DataServicesResourceUtil.GetString("RequestQueryProcessor_QueryNoOptionsApplicable");
                string queryOptionNoSet = DataServicesResourceUtil.GetString("RequestQueryProcessor_QuerySetOptionsNotApplicable");
                string forbidden = DataServicesResourceUtil.GetString("RequestUriProcessor_Forbidden");

                // Dimension 1: operation rights
                var serviceOperationRights = new ServiceOperationRights[]
                {
                    ServiceOperationRights.All,
                    ServiceOperationRights.AllRead,
                    ServiceOperationRights.ReadMultiple,
                    ServiceOperationRights.ReadSingle,
                    ServiceOperationRights.OverrideEntitySetRights | ServiceOperationRights.ReadMultiple,
                    ServiceOperationRights.OverrideEntitySetRights | ServiceOperationRights.ReadSingle
                };

                // Dimension 2: result kind (skip void)
                var providers = new Providers.ServiceOperationResultKind[]
                {
                    Providers.ServiceOperationResultKind.DirectValue,
                    Providers.ServiceOperationResultKind.QueryWithMultipleResults,
                    Providers.ServiceOperationResultKind.QueryWithSingleResult,
                    Providers.ServiceOperationResultKind.Enumeration
                };

                // Dimension 3: uri string
                var requestUris = new string[]
                {
                    "/GetEntity?id=1",
                    "/GetEntity?id=1&$filter=ID eq 1&$select=ID",
                    "/GetEntity?id=1&$top=1", "/GetEntity()?id=1",
                    "/GetEntity(1)?id=1", "/GetEntity(1)/ID?id=1"
                };

                // Dimension 4: entity set rights
                var entitySetRights = new EntitySetRights[]
                {
                    EntitySetRights.All,
                    EntitySetRights.AllRead,
                    EntitySetRights.AllWrite,
                    EntitySetRights.None,
                    EntitySetRights.ReadMultiple,
                    EntitySetRights.ReadSingle
                };

                TestUtil.RunCombinations(serviceOperationRights, providers, requestUris, entitySetRights, (sopRights, resultKind, uri, esRights) =>
                    {
                        DSPDataService serviceInstance = null;

                        DSPMetadata m = new DSPMetadata("testContainer", "AstoriaUnitTests.Tests");
                        var entityType = m.AddEntityType("TestEntity", null, null, false);
                        m.AddKeyProperty(entityType, "ID", typeof(int));
                        var entitySet = m.AddResourceSet("Entities", entityType);

                        bool callBackInvoked = false;
                        Func<object[], object> sopCallBack = (args) =>
                        {
                            callBackInvoked = true;
                            var res = new DSPResource(entityType);
                            res.SetValue("ID", 1);
                            if (resultKind == Providers.ServiceOperationResultKind.DirectValue)
                            {
                                return res;
                            }
                            else if (resultKind == Providers.ServiceOperationResultKind.Enumeration)
                            {
                                return new DSPResource[] { res }.AsEnumerable();
                            }
                            else
                            {
                                return new DSPResource[] { res }.AsQueryable();
                            }
                        };

                        m.AddServiceOperation("GetEntity", resultKind, entityType, entitySet, "GET",
                            new Providers.ServiceOperationParameter[] { new Providers.ServiceOperationParameter("id", Providers.ResourceType.GetPrimitiveResourceType(typeof(int))) });

                        DSPServiceDefinition service = new DSPServiceDefinition()
                        {
                            Metadata = m,
                            CreateDataSource = (metadata) =>
                            {
                                return new DSPContext() { ServiceOperations = new Dictionary<string, Func<object[], object>>() { { "GetEntity", sopCallBack } } };
                            },
                            ServiceConstructionCallback = (instance) => { serviceInstance = (DSPDataService)instance; },
                            ServiceOperationAccessRule = new Dictionary<string, ServiceOperationRights>() { { "GetEntity", sopRights } },
                            EntitySetAccessRule = new Dictionary<string, EntitySetRights>() { { "Entities", esRights } }
                        };

                        using (TestWebRequest request = service.CreateForInProcessWcf())
                        {
                            request.RequestUriString = uri;
                            Exception ex = TestUtil.RunCatching(request.SendRequest);

                            string expectedMessage = null;      // expected message
                            HttpStatusCode expectedStatusCode = HttpStatusCode.Accepted;

                            if (esRights == EntitySetRights.None)
                            {
                                // this should be 500 error
                                expectedStatusCode = HttpStatusCode.BadRequest;
                                expectedMessage = "The operation 'GetEntity' has the resource set 'Entities' that is not visible. The operation 'GetEntity' should be made hidden or the resource set 'Entities' should be made visible.";
                            }
                            else if ((resultKind == Providers.ServiceOperationResultKind.DirectValue || resultKind == Providers.ServiceOperationResultKind.QueryWithSingleResult))
                            {
                                // Single Result SOP: fail with any query, no read single rights
                                if (uri.Contains("(1)"))
                                {
                                    expectedStatusCode = HttpStatusCode.BadRequest;
                                    expectedMessage = string.Format(queryPortionNotEmpty, "GetEntity");
                                }
                                else if (uri.Contains("$top"))
                                {
                                    expectedStatusCode = HttpStatusCode.BadRequest;
                                    expectedMessage = resultKind == Providers.ServiceOperationResultKind.QueryWithSingleResult ? queryOptionNoSet : queryOptionNotEmpty;
                                }
                                else if (uri.Contains("$") && resultKind != Providers.ServiceOperationResultKind.QueryWithSingleResult)
                                {
                                    expectedStatusCode = HttpStatusCode.BadRequest;
                                    expectedMessage = queryOptionNotEmpty;
                                }
                                else if (!HasRights(sopRights, esRights, single: true))
                                {
                                    expectedStatusCode = HttpStatusCode.Forbidden;
                                    expectedMessage = forbidden;
                                }
                            }
                            else if (resultKind == Providers.ServiceOperationResultKind.Enumeration)
                            {
                                // IEnumerable result sop : fail if no Multiple Rights or any filter portion
                                if (uri.Contains("(1)"))
                                {
                                    expectedStatusCode = HttpStatusCode.BadRequest;
                                    expectedMessage = "The request URI is not valid. The segment 'GetEntity' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.";
                                }
                                else if (uri.Contains("$"))
                                {
                                    expectedStatusCode = HttpStatusCode.BadRequest;
                                    expectedMessage = queryOptionNotEmpty;
                                }
                                else if (!HasRights(sopRights, esRights, single: false))
                                {
                                    expectedStatusCode = HttpStatusCode.Forbidden;
                                    expectedMessage = forbidden;
                                }
                            }
                            else
                            {
                                // multiple result sop / selection by key
                                if (!HasRights(sopRights, esRights, single: uri.Contains("(1)")))
                                {
                                    expectedStatusCode = HttpStatusCode.Forbidden;
                                    expectedMessage = forbidden;
                                }
                            }

                            if (expectedStatusCode != HttpStatusCode.Accepted)
                            {
                                UnitTestsUtil.VerifyWebExceptionXML(ex, expectedStatusCode, expectedMessage);
                                Assert.IsFalse(callBackInvoked, "Service operation should not be invoked when there is an error");
                            }
                            else
                            {
                                TestUtil.AssertExceptionExpected(ex, false);
                                Assert.IsTrue(callBackInvoked, "Service operation should be invoked when there is not an error");
                            }
                        }
                    }
                );
            }

            private bool HasRights(ServiceOperationRights sorSet, EntitySetRights esrSet, bool single)
            {
                if (sorSet.HasFlag(ServiceOperationRights.OverrideEntitySetRights))
                {
                    return sorSet.HasFlag(single ? ServiceOperationRights.ReadSingle : ServiceOperationRights.ReadMultiple);
                }
                else
                {
                    return esrSet.HasFlag(single ? EntitySetRights.ReadSingle : EntitySetRights.ReadMultiple);
                }
            }

            #endregion

            #region Edm.Boolean should be returned in error messages, not System.Boolean

            [Ignore] // Remove Atom
            // [TestMethod, Variation("Edm.Boolean should be returned in error messages, not System.Boolean")]
            public void ErrorMessageShouldReturnEdmBooleanInsteadOfSystemBoolean()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = AstoriaUnitTests.Data.ServiceModelData.CustomData.ServiceModelType;
                    request.RequestUriString = "/Customers?$filter=ID";

                    Exception ex = TestUtil.RunCatching(request.SendRequest);
                    UnitTestsUtil.VerifyWebExceptionXML(ex, HttpStatusCode.BadRequest, ODataLibResourceUtil.GetString("MetadataBinder_FilterExpressionNotSingleValue"));
                }
            }
            #endregion

            #region Default ODataBatchReaderStreamBuffer Length

            [Ignore] // Remove Atom
            // [TestMethod]
            public void TestDefaultBatchReaderBufferLength()
            {
                Type batchStreamType = typeof(Microsoft.OData.ODataBatchReader).Assembly.GetType("Microsoft.OData.Core.ODataBatchReaderStreamBuffer");
                FieldInfo defaultBufferSizeField = batchStreamType.GetField("BufferLength", BindingFlags.NonPublic | BindingFlags.Static);
                Assert.IsNotNull(defaultBufferSizeField);
                int defaultBufferSize = (int)defaultBufferSizeField.GetValue(null);

                string batchContentBegin = @"--batch_09c35e58-cefd-401a-97e3-4de640036aa6
Content-Type: multipart/mixed; boundary=changeset_6e069cca-b4f0-4e89-b37b-f42278d05fa3

--changeset_6e069cca-b4f0-4e89-b37b-f42278d05fa3
Content-Type: application/http
Content-Transfer-Encoding:binary

POST http://host/Customers HTTP/1.1
Accept: application/json
Content-Type: application/json
Content-ID: 1

{ Name: """;
                string batchContentEnd = "\" }\r\n\r\n--changeset_6e069cca-b4f0-4e89-b37b-f42278d05fa3--\r\n--batch_09c35e58-cefd-401a-97e3-4de640036aa6--";

                using (TestWebRequest r = TestWebRequest.CreateForInProcessWcf())
                {
                    r.DataServiceType = typeof(CustomDataContext);
                    r.RequestUriString = "/$batch";
                    r.HttpMethod = "POST";
                    r.RequestContentType = "multipart/mixed; boundary=batch_09c35e58-cefd-401a-97e3-4de640036aa6";

                    for (int i = defaultBufferSize - batchContentBegin.Length - 100; i < defaultBufferSize + 100; ++i)
                    {
                        // create dynamic payload
                        using (MemoryStream ms = new MemoryStream())
                        {
                            StreamWriter w = new StreamWriter(ms);
                            w.Write(batchContentBegin);

                            for (int j = 0; j < i; ++j)
                            {
                                w.Write("o");
                            }

                            w.Write(batchContentEnd);
                            w.Flush();

                            ms.Seek(0, SeekOrigin.Begin);

                            r.RequestStream = ms;
                            r.SendRequest();
                        }

                        var s = r.GetResponseStreamAsText();

                        if (s.Contains("An internal read request was too small."))
                        {
                            Assert.Fail("Batch stream buffer reader error: An internal read request was too small. Payload length is " + i);
                        }
                    }
                }
            }
            #endregion

            #region Linq Call to Public Apis
            [TestMethod, Variation, Conditional("DEBUG")]
            public void VerifyLinqCallsToPublicMethods()
            {
                using (TestUtil.MetadataCacheCleaner())
                {
                    // verify that we use the AreByteArraysEqual method in this call
                    var doc = ExpressionTreeTestUtils.CreateRequestAndGetExpressionTreeXml(typeof(Context), "/Values?$filter=BinaryValue eq binary'AAAB'");
                    UnitTestsUtil.VerifyXPathResultCount(doc, 1, "//Method[@type='DataServiceProviderMethods' and text()='AreByteArraysEqual']");

                    // verify that we use the AreByteArraysNotEqual method in this call
                    doc = ExpressionTreeTestUtils.CreateRequestAndGetExpressionTreeXml(typeof(Context), "/Values?$filter=BinaryValue ne binary'AAAB'");
                    UnitTestsUtil.VerifyXPathResultCount(doc, 1, "//Method[@type='DataServiceProviderMethods' and text()='AreByteArraysNotEqual']");
                }
            }

            [TestMethod]
            public void ApiTests()
            {
                byte[] value0 = new byte[] { 0x00 };
                byte[] value0_2 = new byte[] { 0x00 };
                byte[] value1 = new byte[] { 0x01 };
                byte[] value4 = new byte[] { 0x01, 0x00 };

                Tuple<bool, byte[], byte[]>[] callValues = new Tuple<bool, byte[], byte[]>[]
                {
                    new Tuple<bool, byte[], byte[]>(true, null, null),      // both null
                    new Tuple<bool, byte[], byte[]>(false, value0, null),   // right null
                    new Tuple<bool, byte[], byte[]>(false, null, value0),   // left null
                    new Tuple<bool, byte[], byte[]>(false, value0, value4), // diff by length
                    new Tuple<bool, byte[], byte[]>(false, value0, value1), // diff by value
                    new Tuple<bool, byte[], byte[]>(true, value0, value0),  // equal - by reference
                    new Tuple<bool, byte[], byte[]>(true, value0, value0_2),  // equal - by value
                };
                TestUtil.RunCombinations(callValues,
                    (call) =>
                    {
                        Assert.AreEqual(call.Item1, DataServiceProviderMethods.AreByteArraysEqual(call.Item2, call.Item3), "AreByteArraysEqual returned {0} with parameters {1}, {2}", !call.Item1, call.Item2, call.Item3);
                        Assert.AreEqual(!call.Item1, DataServiceProviderMethods.AreByteArraysNotEqual(call.Item2, call.Item3), "AreByteArraysNotEqual returned {0} with parameters {1}, {2}", call.Item1, call.Item2, call.Item3);
                    });
            }

            public class Context
            {
                private static List<BinaryType> data;

                static Context()
                {
                    data = new List<BinaryType>();
                    data.Add(new BinaryType() { ID = 1, BinaryValue = new Byte[] { 0x00, 0x00, 0x01 } });
                    data.Add(new BinaryType() { ID = 1, BinaryValue = new Byte[] { 0x00, 0x01, 0x00 } });
                    data.Add(new BinaryType() { ID = 1, BinaryValue = new Byte[] { 0x01, 0x00, 0x00 } });
                    data.Add(new BinaryType() { ID = 1, BinaryValue = new Byte[] { 0x01, 0x02, 0x03 } });
                }

                public IQueryable<BinaryType> Values
                {
                    get { return data.AsQueryable(); }
                }
            }

            public class BinaryType
            {
                public int ID { get; set; }
                public byte[] BinaryValue { get; set; }
            }
            #endregion

            #region DSP Resource

            public class BugResource : DSPResource
            {
                public BugResource() : base() { }
                public BugResource(Providers.ResourceType resourceType) : base(resourceType) { }

                public override object GetValue(string propertyName)
                {
                    object value = base.GetValue(propertyName);

                    if (value == null)
                    {
                        Providers.ResourceProperty resourceProperty = this.GetResourceProperty(propertyName);
                        if (resourceProperty != null && resourceProperty.Kind == ResourcePropertyKind.ResourceSetReference)
                        {
                            // lazy create the list
                            value = new List<DSPResource>();
                            //Type listType = typeof(List<>).MakeGenericType(resourceProperty.ResourceType.InstanceType);
                            //value = Activator.CreateInstance(listType);
                            this.SetRawValue(propertyName, value);
                        }
                    }

                    return value;
                }
            }

            [TestMethod, Variation("Deserialize 'results' collections")]
            public void ComplexTypeKeepInContent_IDSP()
            {
                DSPMetadata metadata = new DSPMetadata("ContainerName", "NamespaceName");
                Providers.ResourceType parentEntity = metadata.AddEntityType("ParentType", typeof(BugResource), null, false);
                metadata.AddKeyProperty(parentEntity, "ID", typeof(string));

                Providers.ResourceType childEntity = metadata.AddEntityType("ChildType", typeof(BugResource), null, false);
                metadata.AddKeyProperty(childEntity, "ID", typeof(string));

                ResourceSet parentSet = metadata.AddResourceSet("ParentSet", parentEntity);
                ResourceSet childSet = metadata.AddResourceSet("ChildSet", childEntity);
                metadata.AddResourceSetReferenceProperty(parentEntity, "Children", childSet, childEntity);

                DSPServiceDefinition service = new DSPServiceDefinition();
                service.Metadata = metadata;
                service.EnableAccess = new List<string>() { "*" };
                service.Writable = true;

                DSPContext context = new DSPContext();
                DSPResource child1 = new DSPResource(childEntity);
                child1.SetValue("ID", "1");
                DSPResource child2 = new DSPResource(childEntity);
                child2.SetValue("ID", "2");
                context.GetResourceSetEntities("ChildSet").Add(child1);
                context.GetResourceSetEntities("ChildSet").Add(child2);

                service.CreateDataSource = (m) => { return context; };

                string regularArray = @"
{
    ""ID"": ""3002"",
    ""Children"":
        [
            {
                ""ID"": ""1""
            },
            {
                ""ID"": ""2""
            }
        ]
}
";
                string badNameArray = regularArray.Replace("[", "foo").Replace("]", "]}");
                Tuple<bool, string>[] payloads = new Tuple<bool, string>[]{
                    new Tuple<bool, string>(false, regularArray),
                    new Tuple<bool, string>(true, badNameArray),
                };
                TestUtil.RunCombinations(payloads,
                    (payload) =>
                    {
                        using (TestWebRequest request = service.CreateForInProcess())
                        {
                            request.RequestUriString = "/ParentSet";
                            request.Accept = UnitTestsUtil.JsonLightMimeType;
                            request.HttpMethod = "POST";
                            request.RequestContentType = UnitTestsUtil.JsonLightMimeType;
                            request.RequestStream = new MemoryStream();

                            StreamWriter writer = new StreamWriter(request.RequestStream);
                            writer.Write(payload.Item2);
                            writer.Flush();
                            request.RequestStream.Seek(0, SeekOrigin.Begin);
                            bool expectException = payload.Item1;
                            try
                            {
                                request.SendRequest();
                                Assert.IsFalse(expectException, "didn't get the expected exception");
                            }
                            catch (Exception)
                            {
                                if (!expectException)
                                {
                                    throw;
                                }
                            }
                            string responseText = request.GetResponseStreamAsText();
                        }
                    });
            }
            #endregion

            #region InvalidRequestVersionErrorMsg
            [Ignore] // Remove Atom
            // [TestMethod]
            public void InvalidRequestVersionErrorMsg()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    string versionRequested = "d;4.0";
                    request.RequestMaxVersion = versionRequested;
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/Customers(0)";
                    request.HttpMethod = "GET";

                    try
                    {
                        request.SendRequest();
                        Assert.Fail();
                    }
                    catch (WebException ex)
                    {
                        HttpStatusCode statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                        string errorPayload = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                        string idealErrorMsg = DataServicesResourceUtil.GetString("DataService_VersionCannotBeParsed", versionRequested, "OData-MaxVersion");

                        Assert.AreEqual(HttpStatusCode.BadRequest, statusCode, "a 400 error should occur");
                        Assert.IsTrue(errorPayload.Contains(idealErrorMsg), "making sure we are getting the right error message with the right header name");

                    }
                }

                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    string versionRequested = "d;4.0";
                    request.RequestVersion = versionRequested;
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/Customers(0)";
                    request.HttpMethod = "GET";

                    try
                    {
                        request.SendRequest();
                        Assert.Fail();
                    }
                    catch (WebException ex)
                    {
                        HttpStatusCode statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                        string errorPayload = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                        string idealErrorMsg = DataServicesResourceUtil.GetString("DataService_VersionCannotBeParsed", versionRequested, "OData-Version");

                        Assert.AreEqual(HttpStatusCode.BadRequest, statusCode, "a 400 error should occur");
                        Assert.IsTrue(errorPayload.Contains(idealErrorMsg), "making sure we are getting the right error message with the right header name");

                    }
                }
            }
            #endregion

            #region Batch insert assert

            [HasStream]
            public class EntityType
            {
                public int ID { get; set; }
                public string Description { get; set; }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation("Assertion when inserting a short payload in batch")]
            public void InsertShortPayloadInBatchShouldWork()
            {
                using (TestUtil.RestoreStaticValueOnDispose(typeof(BaseStreamProvider2), "DefaultBufferSize"))
                {
                    BaseStreamProvider2.DefaultBufferSize = 8;

                    // Service Definition:
                    DSPMetadata metadata = new DSPMetadata("RegressionTestsDev11", "AstoriaUnitTests.Tests");
                    var entityType1 = metadata.AddEntityType("TestEntityType", null, null, false);
                    entityType1.IsMediaLinkEntry = true;
                    metadata.AddKeyProperty(entityType1, "ID", typeof(int));
                    metadata.AddPrimitiveProperty(entityType1, "Description", typeof(string));
                    var set1 = metadata.AddResourceSet("Entities", entityType1);

                    DSPServiceDefinition service = new DSPServiceDefinition()
                    {
                        Metadata = metadata,
                        Writable = true,
                        SupportMediaResource = true,
                        MediaResourceStorage = new DSPMediaResourceStorage()
                    };

                    DSPContext data = new DSPContext();
                    service.CreateDataSource = (m) => { return data; };

                    #region Payload

                    string payload = @"--batch_f2fde10b-7ad2-4be0-9542-adfc4349a473
Content-Type: multipart/mixed; boundary=changeset_73bdba39-9b8f-450b-80fa-6c199eaabb7f

--changeset_73bdba39-9b8f-450b-80fa-6c199eaabb7f
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 0

POST {0}/Entities HTTP/1.1
Accept: application/atom+xml
Content-Type: application/octet-stream
Slug: 0

wooo
--changeset_73bdba39-9b8f-450b-80fa-6c199eaabb7f
Content-Type: ApplICAtioN/hTtp
Content-Transfer-Encoding: binary
Content-ID: 1

PATCH $0 HTTP/1.1
Accept: APPLicATiON/json;odata.metadata=minimal
Content-Type: APPLicATiON/json;odata.metadata=minimal

{{ Description : ""Photo"" }}
--changeset_73bdba39-9b8f-450b-80fa-6c199eaabb7f--
--batch_f2fde10b-7ad2-4be0-9542-adfc4349a473--";

                    #endregion

                    using (TestWebRequest r = service.CreateForInProcessWcf())
                    {
                        r.StartService();
                        r.RequestUriString = "/$batch";
                        r.HttpMethod = "POST";
                        r.RequestContentType = "multipart/mixed; boundary=batch_f2fde10b-7ad2-4be0-9542-adfc4349a473";

                        payload = String.Format(payload, r.ServiceRoot);
                        r.SetRequestStreamAsText(payload);

                        r.SendRequest();
                        var s = r.GetResponseStreamAsText();

                        TestUtil.AssertContains(s, "HTTP/1.1 201 Created");
                        TestUtil.AssertContains(s, "HTTP/1.1 204 No Content");
                    }
                }

            }
            #endregion

            #region Verify that setting reference property when FK is part of the PK in EF works.

            [Ignore] // Remove Atom
            // [TestMethod, Variation("Verify that setting reference property when FK is part of the PK in EF works.")]
            public void SettingReferencePropertyWhenFKIsPartOfPKShouldWork()
            {
                Type providerType = typeof(EFFK.CustomObjectContextPOCO);
                using (UnitTestsUtil.CreateChangeScope(typeof(EFFK.CustomObjectContextPOCO)))
                {
                    #region AtomPayload and XPaths
                    string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                        "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                            AtomUpdatePayloadBuilder.GetCategoryXml(typeof(OrderDetail).FullName) +
                            "<content type=\"application/xml\"><adsm:properties>" +
                                "<ads:ProductID>125</ads:ProductID>" +
                                "<ads:OrderID>1</ads:OrderID>" +
                            "</adsm:properties></content>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Order' title='Order' type='application/atom+xml;type=entry' href='Orders(1)' />" +
                        "</entry>";

                    var atomXPaths1 = new KeyValuePair<string, string[]>(
                        "/OrderDetails",
                        new string[] { "/atom:entry[atom:category/@term='#AstoriaUnitTests.Stubs.OrderDetail' and atom:id='http://host/OrderDetails(OrderID=1,ProductID=125)' and atom:content/adsm:properties[ads:OrderID='1' and ads:ProductID='125']]" });

                    var atomXPaths2 = new KeyValuePair<string, string[]>(
                        "/OrderDetails(OrderID=1,ProductID=125)/Order",
                        new string[] { "/atom:entry[atom:category/@term='#AstoriaUnitTests.Stubs.Order' and atom:id='http://host/Orders(1)' and atom:content/adsm:properties/ads:ID='1']" });
                    #endregion

                    //TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
                    UnitTestsUtil.CustomProviderRequest(providerType, "/OrderDetails", UnitTestsUtil.AtomFormat, atomPayload, new KeyValuePair<string, string[]>[] { atomXPaths1, atomXPaths2 }, "POST", false /*verifyETagReturned*/);
                }
            }
            #endregion

            #region For EF, when FK is part of the PK, setting just the reference property does not work
            [TestMethod, Variation("For EF, when FK is part of the PK, setting just the reference property does not work")]
            public void SettingJustReferencePropertyWhenFKIsPartOfPKShouldFail()
            {
                string jsonOfficePayload = "{ ID: 1234, OfficeNumber: 1240, FloorNumber: 1, BuildingName: 'Building 18' }";
                string jsonWorkerPayload = "{ ID: 0, FirstName: 'Pratik', LastName: 'Patel', Office: {@odata.readLink:'/Offices(1234)'} }";

                TestUtil.RunCombinations(
                    new Type[] { typeof(EFFK.CustomObjectContextPOCO), typeof(EFFK.CustomObjectContextPOCOProxy) },
                    contextType =>
                    {
                        using (UnitTestsUtil.CreateChangeScope(contextType))
                        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                        {
                            // Insert an office first
                            UnitTestsUtil.SendRequestAndVerifyXPath(
                                UnitTestsUtil.ConvertPayload(contextType, jsonOfficePayload),
                                UnitTestsUtil.ConvertUri(contextType, "/Offices"),
                                null,
                                contextType,
                                UnitTestsUtil.JsonLightMimeType,
                                "POST");

                            // Insert a worker now (do not set the FK, but set the reference property only)
                            UnitTestsUtil.SendRequestAndVerifyXPath(
                                UnitTestsUtil.ConvertPayload(contextType, jsonWorkerPayload),
                                UnitTestsUtil.ConvertUri(contextType, "/Workers"),
                                null,
                                contextType,
                                UnitTestsUtil.JsonLightMimeType,
                                "POST");
                        }
                    });
            }
            #endregion

            #region In PUT requests to EF provider, we fire change interceptors with entities which do not have the new value
            [TestMethod, Variation("In PUT requests to EF provider, we fire change interceptors with entities which do not have the new value")]
            public void FireChangeInterceptorsInPutToEFShouldWork()
            {
                TestUtil.RunCombinations(new Type[] { typeof(EFFK.CustomObjectContextPOCO), typeof(EFFK.CustomObjectContextPOCOProxy) },
                    contextType =>
                    {
                        using (UnitTestsUtil.CreateChangeScope(contextType))
                        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                        {
                            request.DataServiceType = typeof(ObjectContext<>).MakeGenericType(contextType);
                            request.RequestUriString = "/CustomObjectContext.Orders(0)";
                            request.HttpMethod = "PUT";
                            request.SetRequestStreamAsText("{ DollarAmount: 1111.11 }");
                            request.RequestContentType = UnitTestsUtil.JsonLightMimeType;

                            request.SendRequest();
                        }
                    });
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation("In PUT requests to EF provider, we fire change interceptors with entities which do not have the new value")]
            public void FireChangeInterceptorsInPutToEFShouldWork_Batch()
            {
                TestUtil.RunCombinations(
                    new Type[] { typeof(EFFK.CustomObjectContextPOCO), typeof(EFFK.CustomObjectContextPOCOProxy) },
                    new bool[] { true, false },
                    (contextType, onlyPUTRequest) =>
                    {
                        using (UnitTestsUtil.CreateChangeScope(contextType))
                        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                        {
                            request.DataServiceType = typeof(ObjectContext<>).MakeGenericType(contextType);
                            BatchWebRequest batchRequest = new BatchWebRequest();

                            if (onlyPUTRequest)
                            {
                                InMemoryWebRequest r = new InMemoryWebRequest();
                                r.RequestUriString = "/CustomObjectContext.Orders(0)";
                                r.HttpMethod = "PUT";
                                r.SetRequestStreamAsText("{ DollarAmount: 1111.11 }");
                                r.RequestContentType = UnitTestsUtil.JsonLightMimeType;

                                var changeSet = new BatchWebRequest.Changeset();
                                changeSet.Parts.Add(r);
                                batchRequest.Changesets.Add(changeSet);
                            }
                            else
                            {
                                InMemoryWebRequest postRequest = new InMemoryWebRequest();
                                postRequest.RequestUriString = "/CustomObjectContext.Orders";
                                postRequest.HttpMethod = "POST";
                                postRequest.SetRequestStreamAsText("{ ID: 1111,  DollarAmount: 9999.99 }");
                                postRequest.RequestContentType = UnitTestsUtil.JsonLightMimeType;
                                postRequest.Accept = UnitTestsUtil.JsonLightMimeType;
                                postRequest.RequestHeaders.Add("Content-ID", "1");

                                InMemoryWebRequest putRequest = new InMemoryWebRequest();
                                putRequest.RequestUriString = "/$1";
                                putRequest.HttpMethod = "PUT";
                                putRequest.SetRequestStreamAsText("{ DollarAmount: 1111.11 }");
                                putRequest.RequestContentType = UnitTestsUtil.JsonLightMimeType;

                                var changeSet = new BatchWebRequest.Changeset();
                                changeSet.Parts.Add(postRequest);
                                changeSet.Parts.Add(putRequest);
                                batchRequest.Changesets.Add(changeSet);
                            }

                            batchRequest.SendRequest(request);
                        }
                    });
            }

            public class ObjectContext<T> : OpenWebDataService<T> where T : System.Data.Objects.ObjectContext
            {
                [ChangeInterceptor("CustomObjectContext.Orders")]
                public void OrderChangeInterceptor(EFFK.Order order, UpdateOperations operation)
                {
                    Assert.IsTrue(this.CurrentDataSource.ObjectStateManager.GetObjectStateEntry(order) != null, "the order instance must be tracked by the context");

                    if (operation == UpdateOperations.Change)
                    {
                        // the entity must contain all the changes that are sent in the request
                        Assert.IsTrue(order.DollarAmount == 1111.11, "Make sure the dollar amount is changed");
                    }
                    else if (operation == UpdateOperations.Add)
                    {
                        Assert.IsTrue(order.DollarAmount == 9999.99, "For POST request in batch tests, verify that the value is correct");
                    }
                }
            }
            #endregion

            #region In batch case, if both If-Match and If-None-Match is specified, we fire an assert and then ignore the If-None-Match header
            [Ignore] // Remove Atom
            // [TestMethod, Variation("In batch case, if both If-Match and If-None-Match is specified, we fire an assert and then ignore the If-None-Match header")]
            public void IgnoreIfNoneMatchHeaderWhenIfMatchAndIfNonMatchIsSpecified()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                using (CustomDataContext.CreateChangeScope())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    string etag = null;
                    {
                        // Make a in-memory GET request to get the etag
                        InMemoryWebRequest getRequest = new InMemoryWebRequest();
                        getRequest.RequestUriString = "/Customers(1)";
                        getRequest.Accept = UnitTestsUtil.AtomFormat;
                        BatchWebRequest batchRequest = new BatchWebRequest();
                        batchRequest.Parts.Add(getRequest);
                        batchRequest.SendRequest(request);
                        Assert.IsTrue(UnitTestsUtil.IsSuccessStatusCode(getRequest.ResponseStatusCode), "The request should succeed");
                        etag = getRequest.ResponseETag;
                        Assert.IsTrue(!String.IsNullOrEmpty(etag), "etag must be returned");
                    }

                    {
                        // Make another in memory GET request where both If-Match and If-None-Match headers are set
                        InMemoryWebRequest getRequest = new InMemoryWebRequest();
                        getRequest.RequestUriString = "/Customers(1)";
                        getRequest.Accept = UnitTestsUtil.AtomFormat;
                        getRequest.IfMatch = etag;
                        getRequest.IfNoneMatch = etag;
                        BatchWebRequest batchRequest = new BatchWebRequest();
                        batchRequest.Parts.Add(getRequest);
                        batchRequest.SendRequest(request);
                        Assert.AreEqual(getRequest.ResponseStatusCode, 200, "The request should succeed");
                    }
                }
            }
            #endregion

            #region Since we are calling the EF API directly, we need to catch the exceptions internally and throw DataServiceException
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Since we are calling the EF API directly, we need to catch the exceptions internally and throw DataServiceException")]
            public void ShouldThrowDataServiceExceptionInsteadOfInternalExceptions()
            {
                TestUtil.RunCombinations(
                    UnitTestsUtil.BooleanValues,
                    isBatchRequest =>
                    {
                        using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                        using (UnitTestsUtil.CreateChangeScope(typeof(ocs.CustomObjectContext)))
                        using (OpenWebDataServiceHelper.EnableTypeConversion.Restore())
                        {
                            request.ServiceType = typeof(OpenWebDataService<ocs.CustomObjectContext>);
                            OpenWebDataServiceHelper.EnableTypeConversion.Value = false;
                            HttpStatusCode statusCode = (HttpStatusCode)(-1);
                            string errorPayload = null;

                            if (isBatchRequest)
                            {
                                BatchWebRequest batchRequest = new BatchWebRequest();
                                var changeSet = new BatchWebRequest.Changeset();

                                InMemoryWebRequest mergeRequest = new InMemoryWebRequest();
                                mergeRequest.HttpMethod = "PATCH";
                                mergeRequest.RequestUriString = "Orders(1)";
                                mergeRequest.RequestContentType = UnitTestsUtil.JsonLightMimeType;
                                mergeRequest.SetRequestStreamAsText("{ DollarAmount: \"1111\" }");
                                changeSet.Parts.Add(mergeRequest);
                                batchRequest.Changesets.Add(changeSet);

                                batchRequest.SendRequest(request);

                                statusCode = (HttpStatusCode)mergeRequest.ResponseStatusCode;
                                errorPayload = mergeRequest.GetResponseStreamAsText();
                            }
                            else
                            {
                                request.HttpMethod = "PATCH";
                                request.RequestUriString = "/Orders(1)";
                                request.RequestContentType = UnitTestsUtil.JsonLightMimeType;
                                request.SetRequestStreamAsText("{ DollarAmount: \"1111\" }");

                                try
                                {
                                    request.SendRequest();
                                }
                                catch (WebException ex)
                                {
                                    statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                                    errorPayload = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                                }
                            }

                            Assert.AreEqual(statusCode, HttpStatusCode.BadRequest, "making sure we get bad request when property type is not set correctly");
                            Assert.IsTrue(errorPayload.Contains(DataServicesResourceUtil.GetString("BadRequest_ErrorInSettingPropertyValue", "DollarAmount")), "making sure we are getting the right error message");
                        }
                    });
            }
            #endregion

            #region In ReflectionServiceProvider, we are loading more metadata than we need to
            [TestMethod, Variation("In ReflectionServiceProvider, we are loading more metadata than we need to")]
            public void LoadMoreMetadataInReflectionServiceProvider()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(TestContext);
                    request.HttpMethod = "GET";
                    request.RequestUriString = "/$metadata";
                    request.Accept = UnitTestsUtil.MimeApplicationXml;
                    request.SendRequest();
                    var responsePayload = request.GetResponseStreamAsXDocument();
                    UnitTestsUtil.VerifyXPathResultCount(responsePayload, 2, "//csdl:EntityType");
                }
            }

            public class TestContext
            {
                public IQueryable<TestDerivedType1> Set { get; set; }
            }

            public class TestBaseType
            {
                public int ID { get; set; }
            }

            public class TestDerivedType1 : TestBaseType
            {
            }

            // The etag property name is invalid. Hence loading the metadata should fail.
            // But ideally we should not have loaded this metadata, since this type does
            // not fall under the type hierarchy, since the set refers to the peer type
            [ETag("Name")]
            public class TestDerivedType2 : TestBaseType
            {
            }

            #endregion

            #region Ignore IncludeAssociationLinksInResponse knob value if the MPV is set to less than 4.0

            [Ignore] // Remove Atom
            // [TestMethod, Variation("Ignore IncludeAssociationLinksInResponse knob value if the MPV is set to less than 4.0")]
            public void IgnoreIncludeAssociationLinksInResponseIfMPVLessThan40()
            {
                // Not including V1, since CustomDataContext has EPM mappings
                TestUtil.RunCombinations(new ODataProtocolVersion[] { ODataProtocolVersion.V4 },
                    (maxProtocolVersion) =>
                {
                    using (TestUtil.MetadataCacheCleaner())
                    using (OpenWebDataServiceHelper.MaxProtocolVersion.Restore())
                    using (OpenWebDataServiceHelper.IncludeRelationshipLinksInResponse.Restore())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        OpenWebDataServiceHelper.MaxProtocolVersion.Value = maxProtocolVersion;
                        OpenWebDataServiceHelper.IncludeRelationshipLinksInResponse.Value = true;
                        request.DataServiceType = typeof(CustomDataContext);
                        request.HttpMethod = "GET";
                        request.RequestUriString = "/Customers";
                        request.Accept = UnitTestsUtil.AtomFormat;

                        request.SendRequest();
                        string expectedResponseVersion = (maxProtocolVersion < ODataProtocolVersion.V4) ? "1.0;" : "4.0;";
                        Assert.AreEqual(request.ResponseVersion, expectedResponseVersion, "If the protocol version is set to v3, only then IncludeRelationshipsInPayload knob should come into effect");
                    }
                });
            }
            #endregion

            #region Prefer: Server writes 'Preference-Applied' header on response to POST/Invoke-based service operations, even though it is not honored

            public class TestEntity1
            {
                public int ID { get; set; }
            }

            public class TestContext1
            {
                public IQueryable<TestEntity1> Entity
                {
                    get
                    {
                        return (new TestEntity1[] { new TestEntity1() { ID = 1 } }).AsQueryable();
                    }
                }
            }

            public class TestService1 : OpenWebDataService<TestContext1>
            {
                [WebInvoke]
                public TestEntity1 PostEntity()
                {
                    return this.CurrentDataSource.Entity.First();
                }

                [WebGet]
                public TestEntity1 GetEntity()
                {
                    return this.CurrentDataSource.Entity.First();
                }
            }

            [TestMethod, Variation("Prefer: Server writes 'Preference-Applied' header on response to POST/Invoke-based service operations, even though it is not honored")]
            public void PreferenceAppliedOnPostServiceOps()
            {
                string[] methods = new[] { "GET", "POST" };
                Version[] versions = new Version[] { null, new Version(4, 0) };
                ODataProtocolVersion[] protocolVersions = new[] { ODataProtocolVersion.V4 };

                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    TestUtil.RunCombinations(methods, versions, versions, protocolVersions, (method, dsv, mdsv, mpv) =>
                    {
                        using (TestUtil.MetadataCacheCleaner())
                        using (OpenWebDataServiceHelper.MaxProtocolVersion.Restore())
                        {
                            OpenWebDataServiceHelper.MaxProtocolVersion.Value = mpv;

                            request.ServiceType = typeof(TestService1);
                            request.HttpMethod = method;
                            request.RequestUriString = method == "POST" ? "/PostEntity" : "/GetEntity";
                            request.Accept = "application/atom+xml,application/xml";
                            request.RequestContentLength = 0;
                            request.RequestVersion = dsv == null ? null : dsv.ToString();
                            request.RequestMaxVersion = mdsv == null ? null : mdsv.ToString();
                            request.RequestHeaders["Prefer"] = "return=minimal";
                            Exception e = TestUtil.RunCatching(request.SendRequest);
                            if (e == null)
                            {
                                XDocument response = request.GetResponseStreamAsXDocument();
                                Assert.IsNotNull(response, "Content expected but received none.");
                                Assert.IsFalse(request.ResponseHeaders.Keys.Contains("Preference-Applied"), "Prefer header should not be applied on POST Service Op.");
                            }
                        }
                    });
                }
            }

            #endregion

            #region Json serializer not handling entities that implement IEnumerable

            [TestMethod, Variation("Json serializer not handling entities that implement IEnumerable")]
            public void JsonSerializerNotHandlingEntitiesThatImplementIEnumerable()
            {
                DSPServiceDefinition service = GetService1();
                var jsonXPaths = new string[] {
                    String.Format("{0}[odata.context='http://host/$metadata#Customers']", JsonValidator.ObjectString),
                    String.Format("{1}/value/{0}/{1}[ID=1 and Name='Customer 1']", JsonValidator.ArrayString, JsonValidator.ObjectString),
                    String.Format("{1}/value/{0}/{1}/Orders/{0}/{1}[ID=1 and DollarAmount='1000']", JsonValidator.ArrayString, JsonValidator.ObjectString),
                    String.Format("{1}/value/{0}/{1}/Orders/{0}/{1}[ID=100 and DollarAmount='555']", JsonValidator.ArrayString, JsonValidator.ObjectString),
                };

                using (TestWebRequest request = service.CreateForInProcess())
                {
                    request.RequestUriString = "/Customers?$expand=Orders";
                    request.Accept = UnitTestsUtil.JsonLightMimeType;
                    request.HttpMethod = "GET";
                    request.SendRequest();

                    UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), UnitTestsUtil.JsonLightMimeType, jsonXPaths);
                }
            }

            internal DSPServiceDefinition GetService1()
            {
                DSPMetadata metadata = new DSPMetadata("containerName", "namespaceName");

                Providers.ResourceType customerType = metadata.AddEntityType("Customer", typeof(TestType), null, false);
                Providers.ResourceType orderType = metadata.AddEntityType("Order", typeof(TestType), null, false);

                ResourceSet customerSet = metadata.AddResourceSet("Customers", customerType);
                ResourceSet orderSet = metadata.AddResourceSet("Orders", orderType);

                metadata.AddKeyProperty(customerType, "ID", typeof(int));
                metadata.AddPrimitiveProperty(customerType, "Name", typeof(string));
                metadata.AddResourceSetReferenceProperty(customerType, "Orders", orderSet, orderType);

                metadata.AddKeyProperty(orderType, "ID", typeof(int));
                metadata.AddPrimitiveProperty(orderType, "DollarAmount", typeof(decimal));
                metadata.AddResourceReferenceProperty(orderType, "Customer", customerSet, customerType);

                metadata.SetReadOnly();

                DSPContext context = new DSPContext();

                TestType customer = new TestType(customerType);
                customer.Add("ID", 1);
                customer.Add("Name", "Customer 1");

                TestType order1 = new TestType(orderType);
                order1.Add("ID", 1);
                order1.Add("DollarAmount", 1000M);
                order1.Add("Customer", customer);

                TestType order2 = new TestType(orderType);
                order2.Add("ID", 100);
                order2.Add("DollarAmount", 555M);
                order2.Add("Customer", customer);

                customer.Add("Orders", new List<TestType>() { order1, order2 });

                var customers = context.GetResourceSetEntities("Customers");
                customers.Add(customer);

                var orders = context.GetResourceSetEntities("Orders");
                orders.Add(order1);
                orders.Add(order2);

                DSPServiceDefinition service = new DSPServiceDefinition();
                service.Metadata = metadata;
                service.CreateDataSource = (m) => { return context; };

                return service;
            }

            public class TestType : DSPResource, IDictionary<string, object>
            {
                private new Dictionary<string, object> properties = new Dictionary<string, object>();

                public TestType(Providers.ResourceType resourceType)
                    : base(resourceType)
                {
                }

                public override object GetValue(string propertyName)
                {
                    return properties[propertyName];
                }

                #region IDictionary Methods
                public void Add(string key, object value)
                {
                    this.properties.Add(key, value);
                }

                public bool ContainsKey(string key)
                {
                    return this.properties.ContainsKey(key);
                }

                public ICollection<string> Keys
                {
                    get { return this.properties.Keys; }
                }

                public bool Remove(string key)
                {
                    throw new NotImplementedException();
                }

                public bool TryGetValue(string key, out object value)
                {
                    return this.properties.TryGetValue(key, out value);
                }

                public ICollection<object> Values
                {
                    get { throw new NotImplementedException(); }
                }

                public object this[string key]
                {
                    get
                    {
                        throw new NotImplementedException();
                    }
                    set
                    {
                        throw new NotImplementedException();
                    }
                }

                public void Add(KeyValuePair<string, object> item)
                {
                    throw new NotImplementedException();
                }

                public void Clear()
                {
                    throw new NotImplementedException();
                }

                public bool Contains(KeyValuePair<string, object> item)
                {
                    throw new NotImplementedException();
                }

                public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
                {
                    throw new NotImplementedException();
                }

                public int Count
                {
                    get { throw new NotImplementedException(); }
                }

                public bool IsReadOnly
                {
                    get { throw new NotImplementedException(); }
                }

                public bool Remove(KeyValuePair<string, object> item)
                {
                    throw new NotImplementedException();
                }

                public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
                {
                    throw new NotImplementedException();
                }

                System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                {
                    return this.properties.GetEnumerator();
                }
                #endregion
            }

            #endregion

            #region JSON deserializer should fail on stream properties in requests.

            [Ignore] // Remove Atom
            // [TestMethod, Variation("JSON deserializer should fail on stream properties in requests.")]
            public void JsonInsertPayloadWithNamedStream()
            {
                DSPMetadata metadata = new DSPMetadata("TestContainer", "RegressionTestsDev11");
                Providers.ResourceType entityType = metadata.AddEntityType("EntityWithNamedStream", null, null, false);
                metadata.AddKeyProperty(entityType, "ID", typeof(int));
                Providers.ResourceProperty stream1 = new Providers.ResourceProperty("Stream1", Providers.ResourcePropertyKind.Stream, Providers.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)));
                entityType.AddProperty(stream1);
                metadata.AddResourceSet("Entities", entityType);

                DSPServiceDefinition service = new DSPServiceDefinition()
                {
                    Writable = true,
                    Metadata = metadata,
                    MediaResourceStorage = new DSPMediaResourceStorage(),
                    SupportNamedStream = true,
                    ForceVerboseErrors = true,
                    CreateDataSource = (m) => { return new AstoriaUnitTests.Stubs.DataServiceProvider.DSPContext(); }
                };

                string payload = "{ \"Stream1\": { \"@odata.mediaEditLink\":\"http://pqianvm2:33369/TheTest/Entities(0)/Stream1\" }, \"ID\":0}";

                using (TestWebRequest request = service.CreateForInProcessWcf())
                {
                    request.RequestUriString = "/Entities";
                    request.HttpMethod = "POST";
                    request.RequestContentType = UnitTestsUtil.JsonLightMimeType;
                    request.SetRequestStreamAsText(payload);

                    WebException e = (WebException)TestUtil.RunCatching(request.SendRequest);

                    HttpStatusCode statusCode = ((HttpWebResponse)e.Response).StatusCode;
                    string errorPayload = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                    string expectedErrorMsg = ODataLibResourceUtil.GetString("ODataJsonLightResourceDeserializer_StreamPropertyWithValue", "Stream1");
                    Assert.AreEqual(HttpStatusCode.BadRequest, statusCode, "Should generate a 400 error since stream properties are not allowed in requests.");
                    Assert.IsTrue(errorPayload.Contains(expectedErrorMsg), "Error messages don't match.");
                }
            }
            #endregion

            #region Hidden Navigation Properties causes type segment to be appended at the end of canonical uris

            private static DSPMetadata GetModel()
            {
                DSPMetadata metadata = new DSPMetadata("TestContainer", "AstoriaUnitTests.Tests");

                var addressType = metadata.AddEntityType("Address", null, null, false);
                metadata.AddKeyProperty(addressType, "ID", typeof(int));
                metadata.AddPrimitiveProperty(addressType, "Street", typeof(string));
                metadata.AddPrimitiveProperty(addressType, "City", typeof(string));
                metadata.AddPrimitiveProperty(addressType, "State", typeof(string));
                metadata.AddPrimitiveProperty(addressType, "ZipCode", typeof(string));

                var peopleType = metadata.AddEntityType("People", null, null, false);
                metadata.AddKeyProperty(peopleType, "ID", typeof(int));
                metadata.AddPrimitiveProperty(peopleType, "Name", typeof(string));

                var customerType = metadata.AddEntityType("Customer", null, peopleType, false);
                var customerAddressProperty = metadata.AddResourceReferenceProperty(customerType, "Address", addressType);

                var peopleSet = metadata.AddResourceSet("People", peopleType);
                var addressSet = metadata.AddResourceSet("Addresses", addressType);

                metadata.AddResourceAssociationSet(new Microsoft.OData.Service.Providers.ResourceAssociationSet(
                    "Customer_Address",
                    new ResourceAssociationSetEnd(peopleSet, customerType, customerAddressProperty),
                    new ResourceAssociationSetEnd(addressSet, addressType, null)));

                metadata.SetReadOnly();
                return metadata;
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation("Hidden Navigation Properties causes type segment to be appended at the end of canonical uris")]
            public void HiddenNavigationPropertiesAppendTypeSegmentToCanonicalUri()
            {
                var metadata = GetModel();
                var peopleType = metadata.GetResourceType("People");

                var context = new DSPContext();
                DSPResource people1 = new DSPResource(peopleType);
                people1.SetValue("ID", 1);
                people1.SetValue("Name", "Foo");
                context.GetResourceSetEntities("People").Add(people1);

                TestUtil.RunCombinations(ServiceVersion.ValidVersions, (mpv) =>
                {
                    var service = new DSPServiceDefinition() { Metadata = metadata, CreateDataSource = (m) => { return context; } };
                    service.EntitySetAccessRule = new Dictionary<string, EntitySetRights>() { { "People", EntitySetRights.AllRead } };
                    using (OpenWebDataServiceHelper.MaxProtocolVersion.Restore())
                    using (TestWebRequest request = service.CreateForInProcess())
                    {
                        request.StartService();
                        if (mpv != null) service.DataServiceBehavior.MaxProtocolVersion = mpv.ToProtocolVersion();
                        TestUtil.RunCombinations(UnitTestsUtil.ResponseFormats, ServiceVersion.ValidVersions, (format, version) =>
                        {
                            request.HttpMethod = "GET";
                            request.RequestUriString = "/People";
                            request.Accept = format;
                            if (version != null) request.RequestMaxVersion = version.ToString();
                            request.SendRequest();
                            UnitTestsUtil.VerifyXPaths(UnitTestsUtil.GetResponseAsAtom(request, new JsonToAtomUtil(metadata)),
                                new string[] { "//atom:entry/atom:link[@rel='edit' and substring(@href, string-length(@href) - string-length('People(1)'))]" });
                        });
                    }
                });
            }
            #endregion

            #region JsonSerializer throws an instream error when serializing an entity with no nav properties and IncludeAssociationLinksInResponse is turned on

            public class TestEntity2
            {
                public int ID { get; set; }
                public string Name { get; set; }
            }

            [TestMethod, Variation("JsonSerializer throws an instream error when serializing an entity with no nav properties and IncludeAssociationLinksInResponse is turned on")]
            public void JsonSerializerShouldNotThrowWhenSerializingEntityWithoutNavProperties()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                using (OpenWebDataServiceHelper.IncludeRelationshipLinksInResponse.Restore())
                using (TestUtil.RestoreStaticValueOnDispose(typeof(TypedCustomDataContext<TestEntity2>), "PreserveChanges"))
                {
                    TypedCustomDataContext<TestEntity2>.ClearHandlers();
                    TypedCustomDataContext<TestEntity2>.ClearValues();
                    TypedCustomDataContext<TestEntity2>.PreserveChanges = true;

                    TypedCustomDataContext<TestEntity2>.ValuesRequested += (sender, args) =>
                    {
                        TypedCustomDataContext<TestEntity2> typedContext = (TypedCustomDataContext<TestEntity2>)sender;
                        typedContext.SetValues(new TestEntity2[] { new TestEntity2 { ID = 1, Name = "Foo" } });
                    };

                    OpenWebDataServiceHelper.IncludeRelationshipLinksInResponse.Value = true;
                    request.DataServiceType = typeof(TypedCustomDataContext<TestEntity2>);
                    request.RequestUriString = "/Values";
                    request.HttpMethod = "GET";
                    request.Accept = UnitTestsUtil.JsonLightMimeType;
                    request.SendRequest();

                    Assert.AreEqual("4.0;", request.ResponseVersion, "Response version should be 4.0");
                }
            }

            #endregion

            #region Ensure we error gracefully on service operations returning complex types that are then used in property access

            public class TestDataService : DataService<CustomDataContext>
            {
                public static void InitializeService(DataServiceConfiguration config)
                {
                    config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
                    config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                    config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                }

                // DirectValue
                [WebGet]
                [SingleResult]
                public Address FirstAddress()
                {
                    return this.CurrentDataSource.Customers.Where(c => c.ID == 1).First().Address;
                }

                // DirectValue
                [WebInvoke]
                [SingleResult]
                public Address FirstAddressI()
                {
                    return this.CurrentDataSource.Customers.Where(c => c.ID == 1).First().Address;
                }

                // QueryWithSingleResult
                [WebGet]
                [SingleResult]
                public IQueryable<Address> FirstAddressQ()
                {
                    return this.CurrentDataSource.Customers.Where(c => c.ID == 1).Select<Customer, Address>(c => c.Address);
                }

                // QueryWithSingleResult
                [WebInvoke]
                [SingleResult]
                public IQueryable<Address> FirstAddressQI()
                {
                    return this.CurrentDataSource.Customers.Where(c => c.ID == 1).Select<Customer, Address>(c => c.Address);
                }

                // Enumerable
                [WebGet]
                public IEnumerable<Address> AllAddress()
                {
                    return this.CurrentDataSource.Customers.Select<Customer, Address>(c => c.Address);
                }

                // Enumerable
                [WebInvoke]
                public IEnumerable<Address> AllAddressI()
                {
                    return this.CurrentDataSource.Customers.Select<Customer, Address>(c => c.Address);
                }

            }

            [TestMethod, Variation("Ensure we error gracefully on service operations returning complex types that are then used in property access")]
            public void ShouldFailOnServiceOperationsReturningComplexTypeUsedInPropertyAccess()
            {
                var serviceOps = new[]
                {
                    new {Operation = "GET", ServiceOpName="FirstAddress"},
                    new {Operation = "POST", ServiceOpName="FirstAddressI"},
                    new {Operation = "GET", ServiceOpName="FirstAddressQ"},
                    new {Operation = "POST", ServiceOpName="FirstAddressQI"},
                    new {Operation = "GET", ServiceOpName="AllAddress"},
                    new {Operation = "POST", ServiceOpName="AllAddressI"},
                };

                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    TestUtil.RunCombinations(serviceOps, test =>
                    {
                        request.ServiceType = typeof(TestDataService);
                        request.RequestUriString = "/" + test.ServiceOpName + "/City";
                        request.HttpMethod = test.Operation;
                        request.Accept = UnitTestsUtil.JsonLightMimeType;
                        Exception e = TestUtil.RunCatching(request.SendRequest);

                        int expectedStatusCode = 404;
                        string expectedError;
                        if (test.Operation == "POST")
                        {
                            expectedStatusCode = 400;
                            expectedError = ODataLibResourceUtil.GetString("RequestUriProcessor_MustBeLeafSegment", test.ServiceOpName);
                        }
                        else if (test.ServiceOpName == "AllAddress" || test.ServiceOpName == "AllAddressI")
                        {
                            expectedStatusCode = 400;
                            expectedError = ODataLibResourceUtil.GetString("RequestUriProcessor_MustBeLeafSegment", test.ServiceOpName);
                        }
                        else
                        {
                            expectedError = DataServicesResourceUtil.GetString("RequestUriProcessor_IEnumerableServiceOperationsCannotBeFurtherComposed", test.ServiceOpName);
                        }
                        Assert.IsTrue(e.InnerException.Message.Contains(expectedError));
                        string responseBody = request.GetResponseStreamAsText();
                        Assert.IsTrue(responseBody.Contains(expectedError));
                        Assert.AreEqual(expectedStatusCode, request.ResponseStatusCode, "Response Status Code should be 404");
                    });
                }
            }

            #endregion

            #region ValueAfterCollectionOfPrimitives

            public class Person
            {
                public int ID { get; set; }
                public string Name { get; set; }
            }

            public class TestDataContext1
            {
                public IQueryable<int> GetIntList()
                {
                    return new List<int>() { 1, 2, 3, 4, 5 }.AsQueryable();
                }

                public IQueryable<Person> GetPeople()
                {
                    return new List<Person>() { new Person { ID = 1, Name = "Bob" }, new Person { ID = 2, Name = "Jill" } }.AsQueryable();
                }
            }

            public class TestDataService1 : DataService<TestDataContext1>
            {
                public static void InitializeService(DataServiceConfiguration config)
                {
                    config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
                    config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                    config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                }

                [WebGet]
                public IQueryable<int> ServiceOp_IQueryableInt()
                {
                    return this.CurrentDataSource.GetIntList();
                }

                [WebGet]
                public IQueryable<Person> ServiceOp_IQueryablePeople()
                {
                    return this.CurrentDataSource.GetPeople();
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod]
            public void ValueAfterCollectionOfPrimitives()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = typeof(TestDataService1);
                    request.RequestUriString = "/ServiceOp_IQueryableInt/$value";
                    request.HttpMethod = "GET";
                    try
                    {
                        request.SendRequest();
                        Assert.Fail();
                    }
                    catch (WebException ex)
                    {
                        HttpStatusCode statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                        string errorPayload = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                        string idealErrorMsg = ODataLibResourceUtil.GetString("PathParser_CannotUseValueOnCollection");

                        Assert.AreEqual(HttpStatusCode.BadRequest, statusCode, "Should generate a 400 error since $value can't come after a QueryWithMultipleResults returning primitives");
                        Assert.IsTrue(errorPayload.Contains(idealErrorMsg), "making sure we are getting the right error message");
                    }
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod]
            public void ValueAfterCollectionOfObjects()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = typeof(TestDataService1);
                    request.RequestUriString = "/ServiceOp_IQueryablePeople/$value";
                    request.HttpMethod = "GET";
                    try
                    {
                        request.SendRequest();
                        Assert.Fail();
                    }
                    catch (WebException ex)
                    {
                        HttpStatusCode statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                        string errorPayload = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                        string idealErrorMsg = ODataLibResourceUtil.GetString("PathParser_CannotUseValueOnCollection");

                        Assert.AreEqual(HttpStatusCode.BadRequest, statusCode,
                            "Should generate a 400 error since $value can't come after a QueryWithMultipleResults returning objects");
                        Assert.IsTrue(errorPayload.Contains(idealErrorMsg),
                            "making sure we are getting the right error message");
                    }
                }
            }

            #endregion

            #region VoidServiceOperationUnchangedResponse

            public class EmptyContext { }

            public class TestDataService4 : DataService<EmptyContext>
            {
                public const string ERROR_INT = "Test data service exception (int)";
                public const string ERROR_VOID = "Test data service exception (void)";

                public static void InitializeService(DataServiceConfiguration config)
                {
                    config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
                    config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                    config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                }

                [WebGet]
                public int TestMethod_Int()
                {
                    throw new DataServiceException(400, "code1", ERROR_INT, null, null);
                }

                [WebGet]
                public void TestMethod_Void()
                {
                    throw new DataServiceException(400, "code1", ERROR_VOID, null, null);
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod]
            public void VoidServiceOperationUnchangedResponse()
            {
                var tests = new[] { new { Function = "/TestMethod_Int", Error = TestDataService4.ERROR_INT },
                    new { Function = "/TestMethod_Int", Error = TestDataService4.ERROR_INT } };
                TestUtil.RunCombinations(tests, serviceCallInfo =>
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                    {
                        request.DataServiceType = typeof(TestDataService4);
                        request.RequestUriString = serviceCallInfo.Function;
                        request.HttpMethod = "GET";
                        try
                        {
                            request.SendRequest();
                            Assert.Fail();
                        }
                        catch (WebException ex)
                        {
                            HttpStatusCode statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                            string errorPayload = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                            string idealErrorMsg = serviceCallInfo.Error;

                            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode, "The test methods should generate a 400 error,");
                            Assert.IsTrue(errorPayload.Contains(idealErrorMsg), "making sure we are getting the right error message");
                        }
                    }
                });
            }

            #endregion

            #region Allow custom hosts in WCF scenarios
            [Ignore] // Remove Atom
            // [TestMethod, Description("Allow custom hosts in WCF scenarios")]
            public void AllowCustomHostsWcfScenarios()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.ServiceType = typeof(MyService);
                    // set some dummy values for the http request
                    request.RequestUriString = "/Foo";
                    request.HttpMethod = "GET";
                    request.Accept = "application/atom+xml,application/xml";

                    // set the actual request via the host
                    MyService.Host.RequestHttpMethod = "GET";
                    MyService.Host.RequestPathInfo = "/Customers";
                    MyService.Host.RequestAccept = "application/atom+xml,application/xml";

                    request.SendRequest();
                    UnitTestsUtil.VerifyXPaths(request.GetResponseStreamAsXDocument(), new string[] {
                        "/atom:feed/atom:entry[atom:id='http://host/Customers(0)']" });
                }
            }

            [ServiceContract]
            [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
            [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
            public class MyService
            {
                internal static TestServiceHost2 Host = new TestServiceHost2();

                [OperationContract]
                [WebInvoke(UriTemplate = "*", Method = "*")]
                public Message ProcessRequestForMessage(Stream messageBody)
                {
                    DataServiceWrapper dataService = new DataServiceWrapper();
                    dataService.AttachHost(Host);
                    return dataService.ProcessRequestForMessage(messageBody);
                }

                private class DataServiceWrapper : DataService<CustomDataContext>
                {
                    public static void InitializeService(DataServiceConfiguration config)
                    {
                        config.SetEntitySetAccessRule("*", EntitySetRights.All);
                        config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                    }
                }
            }
            #endregion

            #region WrongDefaultEncodingForValue
            [TestMethod]
            public void WrongDefaultEncodingForValue()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/Customers(1)/Name/$value";
                    request.HttpMethod = "GET";

                    VerifyResponseCharset(request, null, true);
                    VerifyResponseCharset(request, "", true);
                    VerifyResponseCharset(request, "*", true);
                    VerifyResponseCharset(request, "iso-8859-1", false);
                    VerifyResponseCharset(request, "utf-16", false);
                }
            }

            private static void VerifyResponseCharset(TestWebRequest request, string requestAcceptCharset, bool expectUtf8)
            {
                if (requestAcceptCharset != null)
                {
                    request.RequestHeaders["Accept-Charset"] = requestAcceptCharset;
                }
                else
                {
                    request.RequestHeaders.Remove("Accept-Charset");
                }

                request.SendRequest();

                string expectedContentType = String.Format("text/plain;charset={0}", expectUtf8 ? "utf-8" : requestAcceptCharset);
                Assert.AreEqual(expectedContentType, request.ResponseHeaders["Content-Type"]);
            }
            #endregion

            #region SkipTokenEscape

            public class SkipTokenEscapeService : DataService<CustomDataContext>
            {
                public static void InitializeService(DataServiceConfiguration config)
                {
                    config.SetEntitySetPageSize("Customers", 1);
                    config.SetEntitySetAccessRule("*", EntitySetRights.All);
                    config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                    config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                }

                [WebGet]
                public IQueryable<Customer> GetCustomers(String parameter)
                {
                    return this.CurrentDataSource.Customers;
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod]
            public void SkipTokenEscape_NonStringLiteral()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    String parameterExpr = Uri.EscapeDataString("10E+6");
                    String filterExpr = String.Format("$filter={0}%20eq%20{0}", parameterExpr);
                    String orderExpr = String.Format("$orderby={0}%20eq%20{0}", parameterExpr);

                    request.DataServiceType = typeof(SkipTokenEscapeService);
                    request.RequestUriString = "/GetCustomers()?parameter='" + parameterExpr + "'&" + filterExpr + "&" + orderExpr;
                    request.HttpMethod = "GET";
                    request.Accept = "application/atom+xml,application/xml";
                    request.SendRequest();

                    var responseBody = request.GetResponseStreamAsXmlDocument();
                    UnitTestsUtil.VerifyXPaths(responseBody,
                        String.Format("atom:feed/atom:link[@rel = 'next' and contains(@href, \"{0}\") and contains(@href,\"{1}\") and contains(@href,\"{2}\")]", parameterExpr, filterExpr, orderExpr));

                    String nextLinkUri = responseBody.SelectSingleNode("atom:feed/atom:link[@rel = 'next']", TestUtil.TestNamespaceManager).Attributes["href"].Value;
                    request.RequestUriString = nextLinkUri.Replace(request.BaseUri, "/");
                    request.SendRequest();
                }
            }

            #endregion

            #region Should display correct type string for generic type

            public class TestEntity6<T>
            {
                public class Entity<T2>
                {
                    public int ID { get; set; }
                }
            }

            public class TestContext6
            {
                public IQueryable<TestEntity6<int>.Entity<double>> Entities
                {
                    get
                    {
                        return new TestEntity6<int>.Entity<double>[]
                        {
                            new TestEntity6<int>.Entity<double>(){ ID = 0}
                        }.AsQueryable();
                    }
                }
            }

            public class TestEntity7<T>
            {
                public int ID { get; set; }
                public class ComplexType
                {
                    public string Data { get; set; }
                }
                public ComplexType Property { get; set; }
            }

            public class TestContext7
            {
                public IQueryable<TestEntity7<int>> Entities
                {
                    get
                    {
                        return new TestEntity7<int>[]
                        {
                            new TestEntity7<int>(){ ID = 0, Property = new TestEntity7<int>.ComplexType() { Data = "ABCD" } }
                        }.AsQueryable();
                    }
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod]
            public void ShouldDisplayCorrectTypeStringForGenericEntityType()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(TestContext6);
                    request.HttpMethod = "GET";
                    request.Accept = "application/atom+xml,application/xml";
                    request.RequestUriString = "/Entities";
                    request.SendRequest();

                    string response = request.GetResponseStreamAsText();
                    TestUtil.AssertContains(response, "TestEntity6`1[T]_Entity`1[System.Int32 System.Double]");
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod]
            public void ShouldDisplayCorrectTypeStringForGenericComplexType()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(TestContext7);
                    request.HttpMethod = "GET";
                    request.Accept = "application/atom+xml,application/xml";
                    request.RequestUriString = "/Entities";
                    request.SendRequest();

                    string response = request.GetResponseStreamAsText();
                    TestUtil.AssertContains(response, "TestEntity7`1[T]_ComplexType[System.Int32]");
                }
            }

            #endregion

            #region Querying the complex type property with a loop returned invalid response version

            public class TestEntityType3
            {
                public int ID { get; set; }
                public TestComplexType4 ComplexType1 { get; set; }
            }

            public class TestComplexType4
            {
                public TestComplexType5 ComplexType2 { get; set; }
                public IEnumerable<string> Tags { get; set; }
            }

            public class TestComplexType5
            {
                public TestComplexType4 ComplexType1 { get; set; }
                public string Foo { get; set; }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Description("Querying the complex type property with a loop returned invalid response version")]
            public void ComplexTypePropertyWithLoopShouldReturnValidResponseVersion()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    TypedCustomDataContext<TestEntityType3>.CreateChangeScope();
                    TypedCustomDataContext<TestEntityType3>.ValuesRequested += (object sender, EventArgs e) =>
                    {
                        TypedCustomDataContext<TestEntityType3> context = (TypedCustomDataContext<TestEntityType3>)sender;
                        var complexType1 = new TestComplexType4();
                        complexType1.ComplexType2 = new TestComplexType5()
                        {
                            ComplexType1 = new TestComplexType4() { Tags = new string[] { "x", "y" }, ComplexType2 = null },
                            Foo = "bar"
                        };
                        complexType1.Tags = new string[] { "Foo", "Bar" };

                        context.SetValues(new TestEntityType3[] {
                            new TestEntityType3()
                            {
                                ID = 1,
                                ComplexType1 = complexType1
                            }
                        });
                    };

                    request.HttpMethod = "GET";
                    request.DataServiceType = typeof(TypedCustomDataContext<TestEntityType3>);
                    request.Accept = UnitTestsUtil.MimeApplicationXml;
                    request.RequestUriString = "/Values(1)/ComplexType1/ComplexType2";
                    request.SendRequest();
                    Assert.AreEqual(request.ResponseStatusCode, 200, "Request must succeed");
                    Assert.AreEqual(request.ResponseVersion, "4.0;", "Response version must be 4.0 since it contains a complex type with collection");
                }
            }
            #endregion

            #region Expanding reference property fails if entity's instance type derives from IEnumerable

            [TestMethod, Variation("Expanding reference property fails if entity's instance type derives from IEnumerable")]
            public void ExpandingReferencePropertyShouldWorkIfEntityInstanceTypeDerivesFromIEnumerable()
            {
                DSPServiceDefinition service = GetService1();
                var jsonXPaths = new string[]
                {
                    String.Format("{0}[odata.context='http://host/$metadata#Orders']", JsonValidator.ObjectString),
                    String.Format("{1}/value/{0}/{1}[Customer[ID=1 and Name='Customer 1'] and ID=1 and DollarAmount='1000']", JsonValidator.ArrayString, JsonValidator.ObjectString),
                    String.Format("{1}/value/{0}/{1}[Customer[ID=1 and Name='Customer 1'] and ID=100 and DollarAmount='555']", JsonValidator.ArrayString, JsonValidator.ObjectString),
                };

                using (TestWebRequest request = service.CreateForInProcess())
                {
                    request.RequestUriString = "/Orders?$expand=Customer";
                    request.Accept = UnitTestsUtil.JsonLightMimeType;
                    request.HttpMethod = "GET";
                    request.SendRequest();

                    UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), UnitTestsUtil.JsonLightMimeType, jsonXPaths);
                }
            }
            #endregion

            #region Tests that a 404 is returned from a request when an ActionProvider's CreateInvokable returns null.

            private class TestActionProvider : DSPActionProvider
            {
                public override IDataServiceInvokable CreateInvokable(DataServiceOperationContext operationContext, ServiceAction serviceAction, object[] parameterTokens)
                {
                    return null;
                }
            }

            private class TestAction
            {
                private ObjectContext objectContext;

                public TestAction(ObjectContext objectContext)
                {
                    this.objectContext = objectContext;
                }

                [DSPActionAttribute(Providers.OperationParameterBindingKind.Sometimes, ReturnSet = null, ParameterTypeNames = new string[] { "NorthwindModel.Customers" })]
                public void ClearAddress(Customers c)
                {
                    ObjectStateEntry ose;
                    if (!this.objectContext.ObjectStateManager.TryGetObjectStateEntry(c, out ose))
                    {
                        this.objectContext.Attach(c);
                    }

                    c.Address = "";
                    this.objectContext.ApplyCurrentValues("Customers", c);
                }
            }

            public class TestDataService3 : NorthwindTempDbServiceBase<NorthwindContext>, IServiceProvider
            {
                public static new void InitializeService(DataServiceConfiguration config)
                {
                    config.SetEntitySetAccessRule("*", EntitySetRights.All);
                    config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                    config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);

                    config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                    config.DataServiceBehavior.AcceptProjectionRequests = true;
                    config.UseVerboseErrors = true;
                }

                object IServiceProvider.GetService(Type serviceType)
                {
                    if (serviceType == typeof(IDataServiceActionProvider))
                    {
                        var actionProvider = new TestActionProvider();
                        var actionInstance = new TestAction((ObjectContext)this.CurrentDataSource);

                        foreach (MethodInfo method in typeof(TestAction).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(m => m.GetCustomAttributes(typeof(DSPActionAttribute), false).Any()))
                        {
                            actionProvider.AddAction(method, actionInstance);
                        }

                        return actionProvider;
                    }

                    return null;
                }
            }

            [TestMethod, Variation("Tests that a 404 is returned from a request when an ActionProvider's CreateInvokable returns null.")]
            public void ShouldReturn404WhenActionProviderCreateInvokableReturnsNull()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = typeof(TestDataService3);
                    request.HttpMethod = "POST";
                    request.RequestUriString = "/Customers('ALFKI')/ClearAddress";
                    request.Accept = UnitTestsUtil.JsonLightMimeType;
                    request.RequestContentLength = 0;

                    try
                    {
                        request.SendRequest();
                        Assert.Fail();
                    }
                    catch (WebException ex)
                    {
                        HttpStatusCode statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                        string errorPayload = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                        string idealErrorMsg = DataServicesResourceUtil.GetString("RequestUriProcessor_ResourceNotFound", "ClearAddress");

                        Assert.AreEqual(HttpStatusCode.NotFound, statusCode, "Should generate a 404 error since the Action Provider gave null for CreateInvokable on ClearAddress.");
                        Assert.IsTrue(errorPayload.Contains(idealErrorMsg), "making sure we are getting the right error message");
                    }
                }
            }

            #endregion

            #region Updating primitive or complex open property with prefer=return=representation fails if entity's backing type implements IEnumerable

            [TestMethod, Variation("Updating primitive or complex open property with prefer=return=representation fails if entity's backing type implements IEnumerable")]
            public void UpdatingPrimitiveOrComplexOpenPropertyShouldWork()
            {
                var testCases = new[]
                {
                    new // Test that a dynamic primitive string property can be updated without issue
                    {
                        UriString = "/DynamicStr",
                        Content = "{ \"value\":\"New Dynamic String\" }",
                        ExceptionVerifier = new Action<Exception>((Exception ex) => Assert.IsNull(ex, "No exception is expected")),
                        CodeVerifier = new Action<int>((int code) => Assert.AreEqual(code, 200)),
                        ResponseVerifier = new Action<string>((string response) => Assert.IsTrue(response.Contains("New Dynamic String"))),
                    },
                    new // Test that a dynamic primitive int property can be updated without issue
                    {
                        UriString = "/DynamicInt",
                        Content = "{ \"value\":42999 }",
                        ExceptionVerifier = new Action<Exception>((Exception ex) => Assert.IsNull(ex, "No exception is expected")),
                        CodeVerifier = new Action<int>((int code) => Assert.AreEqual(code, 200)),
                        ResponseVerifier = new Action<string>((string response) => Assert.IsTrue(response.Contains("42999"))),
                    },
                    //new // Test that a dynamic complex property can be updated without issue
                    //{
                    //    UriString = "/DynamicComplex",
                    //    Content = "{\"value\": {\"@odata.type\":\"#testNamespace.Address\",\"Street\":\"148th Ave\",\"Zip\":98052}}",
                    //    ExceptionVerifier = new Action<Exception>((Exception ex) => Assert.IsNull(ex, "No exception is expected")),
                    //    CodeVerifier = new Action<int>((int code) => Assert.AreEqual(code, 200)),
                    //    ResponseVerifier = new Action<string>((string response) => Assert.IsTrue(response.Contains("98052"))),
                    //},
                    ////new // Test that a dynamic IEnumerable complex property can be updated without issue
                    ////{
                    ////    UriString = "/DynamicComplexEnumerable",
                    ////    Content = "{\"value\":{\"@odata.type\":\"#testNamespace.EnumerableComplex\",\"StringProperty\":\"String Value\"}}",
                    ////    ExceptionVerifier = new Action<Exception>((Exception ex) => Assert.IsNull(ex, "No exception is expected")),
                    ////    CodeVerifier = new Action<int>((int code) => Assert.AreEqual(code, 200)),
                    ////    ResponseVerifier = new Action<string>((string response) => Assert.IsTrue(response.Contains("String Value"))),
                    ////},
                };

                DSPServiceDefinition service = GetService2();

                using (TestWebRequest request = service.CreateForInProcessWcf())
                {
                    request.StartService();
                    TestUtil.RunCombinations(testCases, new string[] { "PUT", "PATCH", "PATCH" }, UnitTestsUtil.BooleanValues, (testCase, verb, uriProp) =>
                    {
                        request.RequestUriString = "/Customers(1)" + (uriProp ? testCase.UriString : "");
                        request.Accept = UnitTestsUtil.JsonLightMimeType;
                        request.RequestContentType = UnitTestsUtil.JsonLightMimeType;
                        request.RequestHeaders["Prefer"] = "return=representation";
                        request.SetRequestStreamAsText(testCase.Content);
                        request.RequestContentLength = testCase.Content.Length;
                        request.HttpMethod = verb;

                        Exception ex = TestUtil.RunCatching(() => request.SendRequest());
                        int code = request.ResponseStatusCode;
                        string response = request.GetResponseStreamAsText();

                        testCase.ExceptionVerifier(ex);
                        testCase.CodeVerifier(code);
                        testCase.ResponseVerifier(response);
                    });
                }
            }

            internal DSPServiceDefinition GetService2()
            {
                DSPMetadata metadata = new DSPMetadata("containerName", "testNamespace");

                Providers.ResourceType customerType = metadata.AddEntityType("Customer", typeof(TestType1), null, false);
                customerType.IsOpenType = true;

                ResourceSet customerSet = metadata.AddResourceSet("Customers", customerType);

                metadata.AddKeyProperty(customerType, "ID", typeof(int));
                metadata.AddPrimitiveProperty(customerType, "Name", typeof(string));

                Providers.ResourceType addressType = metadata.AddComplexType("Address", null, null, false);
                metadata.AddPrimitiveProperty(addressType, "Street", typeof(string));
                metadata.AddPrimitiveProperty(addressType, "Zip", typeof(int));

                Providers.ResourceType enumerableComplexType = metadata.AddComplexType("EnumerableComplex", typeof(TestType1), null, false);
                metadata.AddPrimitiveProperty(enumerableComplexType, "StringProperty", typeof(string));

                metadata.SetReadOnly();

                DSPContext context = new DSPContext();

                TestType1 customer = new TestType1(customerType);
                customer.Add("ID", 1);
                customer.Add("Name", "Customer 1");

                var customers = context.GetResourceSetEntities("Customers");
                customers.Add(customer);

                DSPServiceDefinition service = new DSPServiceDefinition();
                service.Metadata = metadata;
                service.CreateDataSource = (m) => { return context; };
                service.Writable = true;
                service.ForceVerboseErrors = true;

                return service;
            }

            /// <summary>
            /// Trivial subclass of DSPResource that implements IDictionary with a mapping to the properties list (which contains declared and open properties).
            /// </summary>
            public class TestType1 : DSPResource, IDictionary<string, object>
            {
                public TestType1(Providers.ResourceType resourceType)
                    : base(resourceType)
                {
                }

                #region IDictionary Methods
                public void Add(string key, object value)
                {
                    this.properties.Add(key, value);
                }

                public bool ContainsKey(string key)
                {
                    return this.properties.ContainsKey(key);
                }

                public ICollection<string> Keys
                {
                    get { return this.properties.Keys; }
                }

                public bool Remove(string key)
                {
                    throw new NotImplementedException();
                }

                public bool TryGetValue(string key, out object value)
                {
                    return this.properties.TryGetValue(key, out value);
                }

                public ICollection<object> Values
                {
                    get { throw new NotImplementedException(); }
                }

                public object this[string key]
                {
                    get
                    {
                        throw new NotImplementedException();
                    }
                    set
                    {
                        throw new NotImplementedException();
                    }
                }

                public void Add(KeyValuePair<string, object> item)
                {
                    throw new NotImplementedException();
                }

                public void Clear()
                {
                    throw new NotImplementedException();
                }

                public bool Contains(KeyValuePair<string, object> item)
                {
                    throw new NotImplementedException();
                }

                public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
                {
                    throw new NotImplementedException();
                }

                public int Count
                {
                    get { throw new NotImplementedException(); }
                }

                public bool IsReadOnly
                {
                    get { throw new NotImplementedException(); }
                }

                public bool Remove(KeyValuePair<string, object> item)
                {
                    throw new NotImplementedException();
                }

                public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
                {
                    throw new NotImplementedException();
                }

                System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                {
                    return this.properties.GetEnumerator();
                }
                #endregion
            }

            #endregion

            #region Deleting nullable value type properties should work
            [TestMethod, Description("Deleting nullable value type properties should work")]
            public void DeletingNullableValueTypePropertiesShouldWork()
            {
                EventHandler handler = new EventHandler((sender, e) =>
                {
                    var context = (TypedCustomDataContext<AllTypes>)sender;
                    var theValue = new AllTypes();

                    theValue.ID = 1;
                    theValue.NullableInt16Type = 15;

                    context.SetValues(new object[] { theValue });
                });

                TypedCustomDataContext<AllTypes>.ValuesRequested += handler;

                try
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        TypedCustomDataContext<AllTypes>.CreateChangeScope();
                        request.DataServiceType = typeof(TypedCustomDataContext<AllTypes>);
                        request.HttpMethod = "DELETE";
                        request.RequestUriString = "/Values(1)/NullableInt16Type/$value";
                        request.SendRequest();
                        Assert.AreEqual(request.ResponseStatusCode, 204, "Request must succeed");

                        request.HttpMethod = "GET";
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        Assert.AreEqual(request.ResponseStatusCode, 404, "Request must fail, since the value is null");
                    }
                }
                finally
                {
                    TypedCustomDataContext<AllTypes>.ValuesRequested += handler;
                }
            }
            #endregion

            #region SingleResult attribute shouldn't affect nav props

            public class TestService2 : OpenWebDataService<CustomDataContext>
            {
                [WebGet]
                [SingleResult]
                public IQueryable<Customer> FirstCustomer()
                {
                    return this.CurrentDataSource.Customers.Where(c => c.ID == 1);
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod]
            public void SingleResultAttributeShouldNotAffectNavProps()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.ServiceType = typeof(TestService2);
                    request.RequestUriString = "/FirstCustomer/Orders?$top=1";
                    request.Accept = "application/atom+xml,application/xml";
                    // navigation should succeed
                    request.SendRequest();
                    request.GetResponseStreamAsXDocument();

                    request.RequestUriString = "/FirstCustomer?$top=1";
                    // Direct composition should fail
                    Exception ex = TestUtil.RunCatching(request.SendRequest);
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                    }

                    Assert.AreEqual(DataServicesResourceUtil.GetString("RequestQueryProcessor_QuerySetOptionsNotApplicable"), ex.Message);
                }
            }

            #endregion

            #region Abstract complex types and types in the System namespace cause $metadata to blow off (was: $metadata small breaking change after Edmlib integration on Reflection Service on Array Type)

            [TestMethod]
            public void TestMetadataContainingAbstractComplexTypeInSystemNamespaces()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(TestContext3);
                    request.HttpMethod = "GET";
                    request.RequestUriString = "/$metadata";
                    request.Accept = UnitTestsUtil.MimeApplicationXml;
                    request.SendRequest();
                    var response = request.GetResponseStreamAsXDocument();
                    var test = response.Descendants(UnitTestsUtil.EdmOasisNamespace + "ComplexType");
                    Assert.IsFalse(
                        response.Descendants(UnitTestsUtil.EdmOasisNamespace + "ComplexType").Where(e => e.Attribute("Abstract") != null).Any(),
                        "Complex type must not have 'Abstract' attribute.");
                }
            }

            public class TestEntity3
            {
                public int ID { get; set; }
                public TestComplexType3 ComplexProperty { get; set; }
            }

            public abstract class TestComplexType3
            {
                public string Property { get; set; }
            }

            public class TestContext3
            {
                public IQueryable<TestEntity3> Entities { get; set; }
            }

            #endregion

            #region [Astoria-ODataLib-Integration] In-stream errors due to XmlExceptions are written out backwards (error before partial valid payload)

            [Ignore] // Remove Atom
            // [TestMethod, Variation("Astoria-ODataLib-Integration] In-stream errors due to XmlExceptions are written out backwards (error before partial valid payload")]
            public void InStreamErrorsDueToXmlExceptionsWrittenOutBackwards()
            {
                DSPMetadata metadata = new DSPMetadata("TestContainer", "TestNamespace");
                Providers.ResourceType myType = metadata.AddEntityType("MyType", null, null, false);
                metadata.AddKeyProperty(myType, "ID", typeof(int));
                metadata.AddPrimitiveProperty(myType, "Property Name With Space", typeof(string));
                metadata.AddResourceSet("MySet", myType);

                DSPResource resource1 = new DSPResource(myType, new[] { new KeyValuePair<string, object>("ID", 111), new KeyValuePair<string, object>("Property Name With Space", "&nbsp;") });
                DSPContext context = new DSPContext();
                context.GetResourceSetEntities("MySet").Add(resource1);

                DSPServiceDefinition service = new DSPServiceDefinition() { Metadata = metadata, CreateDataSource = m => context };
                service.ForceVerboseErrors = true;
                using (TestWebRequest request = service.CreateForInProcessWcf())
                {
                    request.HttpMethod = "GET";
                    request.Accept = "application/atom+xml,application/xml";
                    request.RequestUriString = "/MySet(111)";

                    request.SendRequest();
                    string response = request.GetResponseStreamAsText();
                    using (var reader = XmlReader.Create(new StringReader(response)))
                    {
                        string atomNs = UnitTestsUtil.AtomNamespace.NamespaceName;
                        string metadataNs = UnitTestsUtil.MetadataNamespace.NamespaceName;
                        string dataNs = UnitTestsUtil.DataNamespace.NamespaceName;

                        // Expected result (check points):
                        // <?xml version="1.0" encoding="utf-8"?>
                        // <m:error xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">
                        //   <m:innererror>
                        //     <m:message>Invalid name character in 'Resolved Reason'. The ' ' character, hexadecimal value 0x20, cannot be included in a name.</m:message>
                        //   </m:innererror>
                        // </m:error>

                        Assert.IsTrue(reader.Read());
                        Assert.AreEqual(reader.NodeType, XmlNodeType.XmlDeclaration);
                        Assert.AreEqual(XmlNodeType.Element, reader.MoveToContent());

                        reader.ReadToFollowing("error", metadataNs);
                        Assert.AreEqual("error", reader.LocalName);
                        Assert.AreEqual(metadataNs, reader.NamespaceURI);
                        Assert.IsTrue(reader.ReadToDescendant("innererror", metadataNs));
                        Assert.IsTrue(reader.ReadToDescendant("message", metadataNs));
                        var message = reader.ReadElementContentAsString();
                        Assert.AreEqual(
                            "Invalid name character in 'Property Name With Space'. The ' ' character, hexadecimal value 0x20, cannot be included in a name.",
                            message);
                    }
                }
            }

            #endregion

            #region IDSQP.GetResourceType being called with instance of EnumerableWrapper

            [TestMethod, Variation("IDSQP.GetResourceType being called with instance of EnumerableWrapper")]
            public void ExpandReferenceNavigationPropertyDefinedOnDerivedTypeWhenNavigationPropertyTypeIsIEnumerable()
            {
                DSPServiceDefinition service = GetService();
                var jsonXPaths = new string[] {
                    String.Format("{0}[odata.context='http://host/$metadata#People']", JsonValidator.ObjectString),
                    String.Format("{1}/value/{0}/{1}[Id=1]", JsonValidator.ArrayString, JsonValidator.ObjectString ),
                    String.Format("{1}/value/{0}/{1}[odata.type='#namespaceName.Employee' and Photo[Id=1] and Id=2]" , JsonValidator.ArrayString, JsonValidator.ObjectString)
                };

                using (TestWebRequest request = service.CreateForInProcess())
                {
                    request.RequestUriString = "/People?$expand=namespaceName.Employee/Photo";
                    request.Accept = UnitTestsUtil.JsonLightMimeType;
                    request.HttpMethod = "GET";
                    request.SendRequest();

                    UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), UnitTestsUtil.JsonLightMimeType, jsonXPaths);
                }
            }

            internal DSPServiceDefinition GetService()
            {
                DSPMetadata metadata = new DSPMetadata("containerName", "namespaceName");

                Providers.ResourceType photoType = metadata.AddEntityType("Photo", null, null, false);
                Providers.ResourceType personType = metadata.AddEntityType("Person", null, null, false);
                Providers.ResourceType emploeeType = metadata.AddEntityType("Employee", null, personType, false);

                ResourceSet photoSet = metadata.AddResourceSet("Photos", photoType);
                ResourceSet peopleSet = metadata.AddResourceSet("People", personType);

                metadata.AddKeyProperty(photoType, "Id", typeof(int));
                metadata.AddKeyProperty(personType, "Id", typeof(int));
                // To repro the bug Navigation reference property should be defined on the derived type
                metadata.AddResourceReferenceProperty(emploeeType, "Photo", photoSet, photoType);

                metadata.SetReadOnly();

                DSPContext context = new DSPContext();

                var photo = new Resource(photoType);
                photo.SetValue("Id", 1);

                var person = new Resource(personType);
                person.SetValue("Id", 1);

                var emploee = new Resource(emploeeType);
                emploee.SetValue("Id", 2);
                emploee.SetValue("Photo", photo);

                var photos = context.GetResourceSetEntities("Photos");
                photos.Add(photo);

                var people = context.GetResourceSetEntities("People");
                people.Add(person);
                people.Add(emploee);

                DSPServiceDefinition service = new DSPServiceDefinition();
                service.Metadata = metadata;
                service.CreateDataSource = (m) => { return context; };

                return service;
            }

            /// <summary>
            /// To repro the resource type should implement IEnumerable
            /// </summary>
            public class Resource : DSPResource, IEnumerable<KeyValuePair<string, object>>
            {
                public Resource(Providers.ResourceType resourceType)
                    : base(resourceType)
                {
                }

                public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
                {
                    return this.Properties.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }
            }

            #endregion

            #region Errors when sending null values for non-nullable value-type server-generated columns on EF
            [TestMethod, Variation("Errors when sending null values for non-nullable value-type server-generated columns on EF")]
            public void SendNullValueForNonNullableValueTypeOnEF()
            {
                foreach (var contextType in new[] { typeof(AstoriaUnitTests.ObjectContextStubs.CustomObjectContext), typeof(EFFK.CustomObjectContextPOCO) })
                {
                    string requestUri = "/Customers";
                    string requestPayload = @"{ ""@odata.type"": ""AstoriaUnitTests.ObjectContextStubs.Types.Customer"", ""ID"": 123456, ""EditTimeStamp"": null }";

                    using (UnitTestsUtil.CreateChangeScope(contextType))
                    using (var request = TestWebRequest.CreateForInProcess())
                    {
                        UnitTestsUtil.SendRequestAndVerifyXPath(
                                    UnitTestsUtil.ConvertPayload(contextType, requestPayload),
                                    UnitTestsUtil.ConvertUri(contextType, requestUri),
                                    null,
                                    contextType,
                                    UnitTestsUtil.JsonLightMimeType,
                                    "POST",
                                    verifyETag: contextType == typeof(AstoriaUnitTests.ObjectContextStubs.CustomObjectContext));
                    }
                }
            }
            #endregion

            #region Invalid Content Id reference in batch produces null reference exception
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Invalid Content Id reference in batch produces null reference exception")]
            public void InvalidContentIdReferenceInBatchShouldProduceNullReferenceException()
            {
                string batchPayload =
@"--batch_09c35e58-cefd-401a-97e3-4de640036aa6
content-type: application/http
content-transfer-encoding: binary

GET http://host/$1 HTTP/1.1
Accept: application/atom+xml

--batch_09c35e58-cefd-401a-97e3-4de640036aa6--";

                using (TestWebRequest r = TestWebRequest.CreateForInProcess())
                {
                    r.DataServiceType = typeof(CustomDataContext);
                    r.RequestUriString = "/$batch";
                    r.HttpMethod = "POST";
                    r.RequestContentType = "multipart/mixed; boundary=batch_09c35e58-cefd-401a-97e3-4de640036aa6";
                    r.SetRequestStreamAsText(batchPayload);
                    r.SendRequest();

                    string responsePayload = r.GetResponseStreamAsText();
                    Assert.IsTrue(responsePayload.Contains("HTTP/1.1 404 Not Found"), "The status code was not as expected");
                    Assert.IsTrue(responsePayload.Contains(DataServicesResourceUtil.GetString("RequestUriProcessor_ResourceNotFound", "$1")), "The error message did not match");
                }
            }
            #endregion

            #region Edm Version
            [TestMethod, Variation("Fix for: EDM version does not get a bump when the first Function Import is a service operation and there are actions later in the list")]
            public void EdmVersionShouldBe40IfThereIsAnAction()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = typeof(DataService);
                    request.StartService();
                    request.RequestUriString = "/$metadata";
                    var exception = TestUtil.RunCatching(request.SendRequest);
                    var xml = request.GetResponseStreamAsXDocument();
                    Assert.IsNull(exception);
                    UnitTestsUtil.VerifyXPathExists(xml, "//csdl:Schema");
                    Assert.AreEqual("4.0;", request.ResponseVersion, "Version of metadata should be 4.0; if there is an action");
                    Assert.AreEqual(200, request.ResponseStatusCode, "Status code of OK metadata should be 200");
                }
            }

            private class ActionProvider : DSPActionProvider
            {
                public override IDataServiceInvokable CreateInvokable(DataServiceOperationContext operationContext, ServiceAction serviceAction, object[] parameterTokens)
                {
                    return null;
                }
            }

            private class Actions
            {
                private ObjectContext objectContext;

                public Actions(ObjectContext objectContext)
                {
                    this.objectContext = objectContext;
                }

                [DSPActionAttribute(Providers.OperationParameterBindingKind.Sometimes, ReturnSet = null, ParameterTypeNames = new string[] { "NorthwindModel.Customers" })]
                public void ClearAddress(Customers c)
                {
                    ObjectStateEntry ose;
                    if (!this.objectContext.ObjectStateManager.TryGetObjectStateEntry(c, out ose))
                    {
                        this.objectContext.Attach(c);
                    }

                    c.Address = "";
                    this.objectContext.ApplyCurrentValues("Customers", c);
                }
            }

            public class DataService : NorthwindTempDbServiceBase<NorthwindContext>, IServiceProvider
            {
                public static new void InitializeService(DataServiceConfiguration config)
                {
                    config.SetEntitySetAccessRule("*", EntitySetRights.All);
                    config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                    config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);

                    config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                    config.DataServiceBehavior.AcceptProjectionRequests = true;
                    config.UseVerboseErrors = true;
                }

                [WebGet]
                public Customers GetImportantCustomers()
                {
                    return new Customers() { CustomerID = "765123", CompanyName = "StubHub" };
                }

                object IServiceProvider.GetService(Type serviceType)
                {
                    if (serviceType == typeof(IDataServiceActionProvider))
                    {
                        var actionProvider = new ActionProvider();
                        var actionInstance = new Actions(this.CurrentDataSource);

                        foreach (MethodInfo method in typeof(Actions).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(m => m.GetCustomAttributes(typeof(DSPActionAttribute), false).Any()))
                        {
                            actionProvider.AddAction(method, actionInstance);
                        }

                        return actionProvider;
                    }

                    return null;
                }
            }

            #endregion

            #region Replace Function

            private void RunReplaceFunctionTest(bool? apivalue, bool? configValue, int i)
            {
                using (OpenWebDataServiceHelper.AcceptReplaceFunctionInQuery.Restore())
                {
                    using (var request = (LocalWebRequest)TestWebRequest.CreateForLocal())
                    {
                        // Clear the configuration cache so that InitializeService can be called for every combination
                        TestUtil.ClearConfiguration();

                        // making sure it generates a new service for every combination
                        request.TestArguments = new Hashtable();
                        request.TestArguments["foo" + i] = "bar" + i;
                        if (configValue != null)
                        {
                            var configFeaturesSection = new DataServicesFeaturesSection() { ReplaceFunction = new DataServicesReplaceFunctionFeature() { Enable = configValue.Value } };

                            request.AddToConfig(configFeaturesSection);
                        }

                        // Set the configuration api value
                        if (apivalue != null)
                        {
                            var initializeServiceCodeOptions = new DataServicesFeaturesSection() { ReplaceFunction = new DataServicesReplaceFunctionFeature() { Enable = apivalue.Value } };

                            request.AddToInitializeService(initializeServiceCodeOptions);
                        }

                        request.DataServiceType = typeof(CustomDataContext);
                        request.RequestUriString = "/Customers?$orderby=replace(Name, 'c', Name)";
                        request.HttpMethod = "GET";

                        Exception exception = TestUtil.RunCatching(request.SendRequest);

                        // Figure out whether exception should be thrown or not: config settings wins over the api
                        bool replaceAllowed = false;
                        if (configValue != null)
                        {
                            replaceAllowed = configValue.Value;
                        }
                        else if (apivalue != null)
                        {
                            replaceAllowed = apivalue.Value;
                        }

                        string responseStream = request.GetResponseStreamAsText();
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("====================");
                        sb.AppendLine("Config Setting : " + (configValue.HasValue ? configValue.Value.ToString() : "NULL"));
                        sb.AppendLine("API Setting : " + (apivalue.HasValue ? apivalue.ToString() : "NULL"));
                        sb.AppendLine("Exception details: " + ((exception == null) ? "NULL" : exception.ToString()));
                        sb.AppendLine("Response stream: " + responseStream);
                        sb.AppendLine("Request Headers: " + GetHeadersInStringFormat(request.RequestHeaders));
                        sb.AppendLine("Response Headers: " + GetHeadersInStringFormat(request.ResponseHeaders));
                        sb.AppendLine("Request Version: " + ((request.RequestVersion == null) ? "NULL" : request.RequestVersion.ToString()));
                        sb.AppendLine("Response Version: " + ((request.ResponseVersion == null) ? "NULL" : request.ResponseVersion.ToString()));
                        sb.AppendLine("====================");
                        Assert.AreEqual(replaceAllowed, exception == null, "if replace is allowed, no exception should be thrown. More info: " + sb.ToString());

                        if (exception != null)
                        {
                            Assert.AreEqual(400, request.ResponseStatusCode, "In case of error, it should be bad request");
                            Assert.IsTrue(responseStream.Contains(DataServicesResourceUtil.GetString("RequestQueryParser_UnknownFunction", "replace", "0")));
                        }
                    }
                }
            }

            public static string GetHeadersInStringFormat(Dictionary<string, string> headers)
            {
                StringBuilder sb = new StringBuilder();

                if (headers != null)
                {
                    foreach (string header in headers.Keys)
                    {
                        sb.AppendLine(header + ": " + headers[header]);
                    }
                }
                return sb.ToString();
            }
            #endregion

            #region ServerShouldGenerateAbsoluteNextLinkForJsonLightNoMetadata
            [TestMethod]
            public void ServerShouldGenerateAbsoluteNextLinkForJsonLightNoMetadata()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.ServiceType = typeof(SkipTokenEscapeService);
                    request.RequestUriString = "/GetCustomers()?parameter='p'";
                    request.Accept = UnitTestsUtil.JsonLightMimeTypeNoMetadata;
                    request.SendRequest();
                    string response = request.GetResponseStreamAsText();
                    TestUtil.AssertContains(response, "\"@odata.nextLink\":\"http://host/GetCustomers?parameter='p'&$skiptoken=0\"");

                    request.Accept = UnitTestsUtil.JsonLightMimeType;
                    request.SendRequest();
                    response = request.GetResponseStreamAsText();
                    TestUtil.AssertContains(response, "\"@odata.nextLink\":\"GetCustomers?parameter='p'&$skiptoken=0\"");

                    request.Accept = UnitTestsUtil.JsonLightMimeTypeFullMetadata;
                    request.SendRequest();
                    response = request.GetResponseStreamAsText();
                    TestUtil.AssertContains(response, "\"@odata.nextLink\":\"GetCustomers?parameter='p'&$skiptoken=0\"");
                }
            }
            #endregion

            #region Makes sure Invalid accept header throws 415
            [TestMethod, Description("Makes sure Invalid accept header throws 415")]
            public void InvalidAcceptHeaderShouldThrow415()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.RequestUriString = "/Customers";
                    request.DataServiceType = typeof(CustomDataContext);

                    string[] headerAcceptCases = new string[]
                    {
                        "application",
                        "application/",
                        "application]",
                        "application/atom+xml;foo",
                        "application/atom+xml foo=bar",
                        @"application/atom+xml;foo=""bar",
                        "application/atom+xml;;",
                        @"application/atom+xml;foo=unquotedEscape""Char",
                        @"application/atom+xml;foo=""quotedWithMissingEndQuote",
                        @"application/atom+xml;foo=""quotedWithEscapeCharAtTheEnd\",
                        "application/atom+xml; q=1.1",
                        "application/atom+xml; q=2.1",
                        "application/atom+xml; q]1001"
                    };

                    for (int i = 0; i < headerAcceptCases.Length; i++)
                    {
                        request.Accept = headerAcceptCases[i];
                        var ex = TestUtil.RunCatching(request.SendRequest);
                        Assert.AreEqual(415, request.ResponseStatusCode, "Expecting 415 as the status code");
                    }

                    string[] headerAcceptCharsetCases = new string[]
                    {
                        "iso-8859-5 unicode-1-1; q=1",
                    };

                    request.Accept = "application/atom+xml";

                    for (int i = 0; i < headerAcceptCharsetCases.Length; i++)
                    {
                        request.AcceptCharset = headerAcceptCharsetCases[i];
                        var ex = TestUtil.RunCatching(request.SendRequest);
                        Assert.AreEqual(415, request.ResponseStatusCode, "Expecting 415 as the status code");
                    }
                }
            }
            #endregion

            #region [Regression,Security] Infinite loop in 'prefer' header parsing when the value contains a separator character
            [Ignore] // Remove Atom
            // [TestMethod, Variation("[Regression,Security] Infinite loop in 'prefer' header parsing when the value contains a separator character")]
            public void PreferHeaderWithSeperator()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())

                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.MaxProtocolVersion.Restore())
                {
                    OpenWebDataServiceHelper.MaxProtocolVersion.Value = ODataProtocolVersion.V4;

                    request.DataServiceType = typeof(CustomDataContext);
                    request.HttpMethod = "POST";
                    request.Accept = "application/atom+xml,application/xml";
                    request.RequestUriString = "/Products";
                    request.RequestVersion = "4.0";
                    request.RequestMaxVersion = "4.0";
                    request.RequestHeaders["Prefer"] = "foo;bar";
                    request.RequestContentType = "application/json";
                    request.SetRequestStreamAsText("{ \"ID\": 1131562 }");

                    request.SendRequestAndCheckResponse();
                    Assert.IsFalse(request.ResponseHeaders.ContainsKey("PreferenceApplied"));
                }
            }
            #endregion

            #region MicrosoftDataServicesRequestUri
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Specifying MicrosoftDataServicesRequestUri in OnStartProcessingRequest does not get picked up")]
            public void MicrosoftDataServicesRequestUriShouldWorkInOnStartProcessingRequest()
            {
                var rootUri = new Uri("http://abcpqr/SomeRandomService/WcfDataService.svc/");
                var xmlBaseUri = String.Format("xml:base=\"{0}\"", rootUri.AbsoluteUri);

                using (var request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = typeof(OnStartProcessingRequestMethod);
                    OnStartProcessingRequestMethod.RootUri = rootUri;

                    OnStartProcessingRequestMethod.RequestUri = rootUri;
                    request.HttpMethod = "GET";
                    request.Accept = "application/atom+xml,application/xml";
                    request.RequestUriString = "/Something"; // This does not matter since the MicrosoftDataServicesRequestUri will get used.
                    request.SendRequest();
                    var response = request.GetResponseStreamAsText();
                    Assert.IsTrue(response.Contains(xmlBaseUri), "Document must contain the root uri as specified in the MicrosoftDataServicesRootUri");

                    OnStartProcessingRequestMethod.RequestUri = new Uri("http://abcpqr/SomeRandomService/WcfDataService.svc/Customers");
                    request.SendRequest();
                    response = request.GetResponseStreamAsText();
                    Assert.IsTrue(response.Contains(xmlBaseUri), "Document must contain the root uri as specified in the MicrosoftDataServicesRootUri");
                }
            }

            public class OnStartProcessingRequestMethod : DataService<CustomDataContext>
            {
                internal static Uri RequestUri;
                internal static Uri RootUri;

                public static void InitializeService(DataServiceConfiguration config)
                {
                    config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
                    config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                    config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                }

                protected override void OnStartProcessingRequest(ProcessRequestArgs args)
                {
                    OperationContext.Current.IncomingMessageProperties["MicrosoftDataServicesRootUri"] = RootUri;
                    OperationContext.Current.IncomingMessageProperties["MicrosoftDataServicesRequestUri"] = RequestUri;
                }
            }

            #endregion

            #region ServiceDocumentShouldntThrowWithNoMetadata
            [TestMethod]
            public void ServiceDocumentShouldntThrowWithNoMetadata()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.ServiceType = typeof(SkipTokenEscapeService);
                    request.RequestUriString = "/";
                    request.Accept = UnitTestsUtil.JsonLightMimeTypeNoMetadata;
                    request.SendRequest();
                    string response = request.GetResponseStreamAsText();
                    TestUtil.AssertContains(response, "\"name\":\"Customers\",\"kind\":\"EntitySet\",\"url\":\"http://host/Customers\"");

                    request.Accept = UnitTestsUtil.JsonLightMimeType;
                    request.SendRequest();
                    response = request.GetResponseStreamAsText();
                    TestUtil.AssertContains(response, "\"name\":\"Customers\",\"kind\":\"EntitySet\",\"url\":\"Customers\"");

                    request.Accept = UnitTestsUtil.JsonLightMimeTypeFullMetadata;
                    request.SendRequest();
                    response = request.GetResponseStreamAsText();
                    TestUtil.AssertContains(response, "\"name\":\"Customers\",\"kind\":\"EntitySet\",\"url\":\"Customers\"");
                }
            }
            #endregion

            #region KeysInMetadataShouldBeAlphabetized
            [TestMethod]
            public void KeysInMetadataShouldBeAlphabetized()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(CustomRowBasedContext);
                    request.RequestUriString = "/$metadata";
                    request.SendRequest();
                    var response = request.GetResponseStreamAsText();

                    const string expected = @"<EntityType Name=""OrderDetail""><Key><PropertyRef Name=""OrderID"" /><PropertyRef Name=""ProductID"" /></Key>";
                    Assert.IsTrue(response.Contains(expected));
                }
            }
            #endregion

            #region PatchRequestForMaxProtocolVersionV3ShouldReturnSuccess
            /// <summary>
            /// Creates a PATCH request to a service with given Protocol Version
            /// and returns status code for the response.
            /// </summary>
            internal void ValidateResponseForHttpRequest(string httpVerb, ODataProtocolVersion version, Action<TestWebRequest> validateAction)
            {
                // Note: Remember to put this statement since the MaxProtocolVersion is going to be changed
                // otherwise the service won't pickup new values.
                TestUtil.ClearMetadataCache();
                // Just using empty payload since this is just to verify the return code for PATCH request
                string Content = "{}";
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                using (OpenWebDataServiceHelper.MaxProtocolVersion.Restore())
                {
                    OpenWebDataServiceHelper.MaxProtocolVersion.Value = version;
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/Products(1)";
                    request.HttpMethod = httpVerb;
                    request.Accept = UnitTestsUtil.JsonLightMimeType;
                    request.RequestContentType = UnitTestsUtil.JsonLightMimeType;
                    request.SetRequestStreamAsText(Content);
                    request.RequestContentLength = Content.Length;

                    WebException e = (WebException)TestUtil.RunCatching(request.SendRequest);

                    validateAction(request);
                }
            }

            [TestMethod]
            public void PatchRequestForMaxProtocolVersionV3ShouldReturnSuccess()
            {
                Action<TestWebRequest> validate = (request) =>
                    {
                        Assert.AreEqual(204, request.ResponseStatusCode);
                    };

                ValidateResponseForHttpRequest("PATCH", ODataProtocolVersion.V4, validate);
            }
            #endregion

            #region InvalidHttpVerbShouldRetur501NotImplemented
            [TestMethod]
            public void InvalidHttpVerbShouldRetur501NotImplemented()
            {
                Action<TestWebRequest> validate = (request) =>
                    {
                        Assert.AreEqual(501, request.ResponseStatusCode);
                    };

                ValidateResponseForHttpRequest("FAKEHTTPVERB", ODataProtocolVersion.V4, validate);
            }
            #endregion

            #region Setting InstanceContextMode to Single on DataService results in cached query results being returned for subsequent queries
            /// <summary>
            /// A Service with "Singleton" service mode, i.e. only one instance of this service is
            /// created for all requests.
            /// Note that by default the "ConcurrencyMode" attribute of ServiceBehavior is "Single",
            /// so this service is "Single instance and "single-threaded"
            /// </summary>
            [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single)]
            private class DataServiceWrapper : DataService<CustomDataContext>
            {
                public static void InitializeService(DataServiceConfiguration config)
                {
                    config.SetEntitySetAccessRule("*", EntitySetRights.All);
                    config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                }
            }

            /// <summary>
            /// Setting InstanceContextMode to Single on DataService results in cached query results being returned for subsequent queries
            /// </summary>
            // [TestMethod]
            public void ServiceWithSingleInstanceContextModeShouldReturnCorrectResults()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.ServiceType = typeof(DataServiceWrapper);
                    request.StartService();

                    request.RequestUriString = "/Products";
                    request.Accept = "application/atom+xml,application/xml";
                    Exception ex = TestUtil.RunCatching(request.SendRequest);

                    var res = request.GetResponseStreamAsXmlDocument();
                    Assert.AreEqual(200, request.ResponseStatusCode, "Response Stream: " + res.ToString());

                    // Subsequent requests should be reutrn correct results
                    request.RequestUriString = "/Products(0)";
                    request.Accept = "application/atom+xml,application/xml";
                    ex = TestUtil.RunCatching(request.SendRequest) as WebException;

                    res = request.GetResponseStreamAsXmlDocument();
                    Assert.AreEqual(200, request.ResponseStatusCode, "Response stream: " + res.ToString());
                }
            }

            public static ManualResetEvent[] allDone = new ManualResetEvent[2];

            class ObjectState
            {
                public string Query;
                public int Index;
            }

            /// <summary>
            /// Setting InstanceContextMode to Single on DataService results in cached query results being returned for subsequent queries
            /// This test sends multiple requests before getting first response.
            /// Please note that in this case the only the InstanceContextMode property was tested with "Single" value
            /// There is ConcurrencyMode which by default is "Single" and got tested in this particular test.
            /// For "Multiple" ConcurrencyMode, the code throws exception. That is by design.
            /// </summary>
            [Ignore] // Remove Atom
            // [TestMethod]
            public void ServiceWithSingleInstanceContextModeAndSingleConcurrencyMode_SimultaneousRequestsShouldBeProcessedSequentially()
            {
                using (TestWebRequest request1 = TestWebRequest.CreateForInProcessWcf())
                {
                    request1.ServiceType = typeof(DataServiceWrapper);
                    request1.StartService();

                    for (int i = 1; i <= 2; i++)
                    {
                        string query = (i / 2 == 0) ? "/Products(" + i + ")" : "/Customers(" + i + ")";

                        var r1 = WebRequest.Create(request1.BaseUri + query + "?$format=atom");

                        allDone[i - 1] = new ManualResetEvent(false);

                        ObjectState currentRequestState = new ObjectState() { Query = query, Index = i - 1 };

                        ThreadPool.QueueUserWorkItem(new WaitCallback(state => r1.BeginGetResponse(ar =>
                        {
                            ObjectState q1 = (ObjectState)ar.AsyncState;
                            var response1 = r1.EndGetResponse(ar);
                            var payload1 = (new StreamReader(response1.GetResponseStream())).ReadToEnd();
                            Assert.IsTrue(payload1.Contains(query));
                            allDone[q1.Index].Set();
                        }, currentRequestState)));
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        allDone[i].WaitOne();
                    }
                }
            }
            #endregion

            #region ReflectionServiceProvider does not work if 'T' in DataService<T> is an interface

            [KeyAttribute("OrderId")]
            public partial class MyOrder
            {
                public int OrderId { get; set; }
                public string Customer { get; set; }
                public IList<MyItem> Items { get; set; }
            }

            [Key("Product")]
            public partial class MyItem
            {
                public string Product { get; set; }
                public int Quantity { get; set; }
            }

            public interface IDummyInterface
            {
                void DummyMethod();
            }

            public partial class MyOrderItemData : IDummyInterface
            {
                #region Populate Service Data

                private static IList<MyOrder> _orders;
                private static IList<MyItem> _items;

                static MyOrderItemData()
                {
                    _orders = new MyOrder[]
                {
                    new MyOrder() {OrderId = 0, Customer = "Peter", Items = new List<MyItem>()},
                    new MyOrder() {OrderId = 1, Customer = "Ana", Items = new List<MyItem>()}
                };
                    _items = new MyItem[]
                {
                    new MyItem() {Product = "Chai", Quantity = 10},
                    new MyItem() {Product = "Chang", Quantity = 25},
                    new MyItem() {Product = "Aniseed Syrup", Quantity = 5},
                    new MyItem() {Product = "Chef Anton's Cajun Seasoning", Quantity = 30}
                };
                    _orders[0].Items.Add(_items[0]);
                    _orders[0].Items.Add(_items[1]);
                    _orders[1].Items.Add(_items[2]);
                    _orders[1].Items.Add(_items[3]);
                }

                #endregion

                public void DummyMethod()
                {
                    // Do nothing
                }

                public IQueryable<MyOrder> Orders
                {
                    get { return _orders.AsQueryable<MyOrder>(); }
                }

                public IQueryable<MyItem> Items
                {
                    get { return _items.AsQueryable<MyItem>(); }
                }
            }

            [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
            public class ReflectionProviderServiceWithInterface : DataService<IDummyInterface>
            {
                // This method is called only once to initialize service-wide policies.
                public static void InitializeService(DataServiceConfiguration config)
                {
                    config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
                    config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                    config.UseVerboseErrors = true;
                }

                protected override IDummyInterface CreateDataSource()
                {
                    return new MyOrderItemData();
                }
            }

            /// <summary>
            /// ReflectionServiceProvider does not work if 'T' in DataService<T> is an interface
            /// Note that this would create a Reflection Provider Service (ReflectionProviderServiceWithInterface) by taking interface (IDummyInterface) as a parameter
            /// and in CreateDataSource, it would return the concrete implementation
            /// The test makes sure that it returns 200 (OK) status code
            /// </summary>
            [Ignore] // Remove Atom
            // [TestMethod]
            public void ReflectionServiceProviderShouldWorkIfGenericParameterInDataServiceIsAnInterface()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.ServiceType = typeof(ReflectionProviderServiceWithInterface);
                    request.StartService();

                    request.RequestUriString = "/Orders";
                    request.Accept = "application/atom+xml,application/xml";
                    request.SendRequest();

                    var res = request.GetResponseStreamAsXmlDocument();
                    Assert.AreEqual(200, request.ResponseStatusCode, "Response Stream: " + res.ToString());

                    // Subsequent requests should be reutrn correct results
                    request.RequestUriString = "/Items";
                    request.SendRequest();

                    res = request.GetResponseStreamAsXmlDocument();
                    Assert.AreEqual(200, request.ResponseStatusCode, "Response stream: " + res.ToString());
                }
            }
            #endregion
        }
    }
}
