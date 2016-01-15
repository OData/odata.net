//---------------------------------------------------------------------
// <copyright file="BLOBSupportTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.EntityClient;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.XPath;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Data;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NorthwindModel;
    using ocs = AstoriaUnitTests.ObjectContextStubs;
    using Microsoft.OData.Client;
    using System.ServiceModel.Web;
    using System.Xml.Linq;
    using AstoriaUnitTests.Tests;
    #endregion Namespaces

    /// <summary>
    /// This is a test class for WebDataServiceTest and is intended
    /// to contain all WebDataServiceTest Unit Tests
    /// </summary>
    [TestModule]
    public partial class UnitTestModule : AstoriaTestModule
    {
        [TestClass()]
        public class BLOBSupportTest
        {
            private static bool HasStreamAttributeInComplexType = false;
            public static Func<Type, object> GetServiceOverride = null;
            public static Action ValidateInterceptorOverride = null;

            [ClassInitialize]
            public static void PerClassSetup(TestContext context)
            {
                BaseTestWebRequest.HostInterfaceType = typeof(IDataServiceHost2);
            }

            [ClassCleanup]
            public static void PerClassCleanup()
            {
                BaseTestWebRequest.HostInterfaceType = typeof(IDataServiceHost);
            }

            [HasStreamAttribute]
            public class MyComplexType
            {
            }

            public class BlobReflectionDataService : DataService<PhotoDataServiceContext>, IServiceProvider
            {
                public static void InitializeService(IDataServiceConfiguration configuration)
                {
                    configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
                    configuration.UseVerboseErrors = OpenWebDataServiceHelper.ForceVerboseErrors;
                    if (BLOBSupportTest.HasStreamAttributeInComplexType)
                    {
                        configuration.RegisterKnownType(typeof(MyComplexType));
                    }
                }

                public BlobReflectionDataService()
                {
                    this.ProcessingPipeline.ProcessingRequest += new EventHandler<DataServiceProcessingPipelineEventArgs>(BlobDataServicePipelineHandlers.ProcessingRequestHandler);
                    this.ProcessingPipeline.ProcessingChangeset += new EventHandler<EventArgs>(BlobDataServicePipelineHandlers.ProcessingChangesetHandler);
                    this.ProcessingPipeline.ProcessedChangeset += new EventHandler<EventArgs>(BlobDataServicePipelineHandlers.ProcessedChangesetHandler);
                    this.ProcessingPipeline.ProcessedRequest += new EventHandler<DataServiceProcessingPipelineEventArgs>(BlobDataServicePipelineHandlers.ProcessedRequestHandler);
                }

                #region IServiceProvider Members

                object IServiceProvider.GetService(Type serviceType)
                {
                    if (GetServiceOverride != null)
                    {
                        return GetServiceOverride(serviceType);
                    }

                    if (serviceType == typeof(IDataServiceStreamProvider))
                    {
                        return new DataServiceStreamProvider();
                    }

                    return null;
                }

                #endregion
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void BlobClrMetadataTest()
            {
                string targetPath = Path.Combine(TestUtil.GeneratedFilesLocation, MethodInfo.GetCurrentMethod().Name);
                IOUtil.EnsureEmptyDirectoryExists(targetPath);

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Location", new WebServerLocation[] { WebServerLocation.InProcess, WebServerLocation.InProcessWcf }),
                    new Dimension("HasStreamAttributeInComplexType", new bool[] { true, false }));

                TestUtil.RunCombinatorialEngineFail(engine, table =>
                {
                    BLOBSupportTest.HasStreamAttributeInComplexType = (bool)table["HasStreamAttributeInComplexType"];

                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                    using (TestWebRequest request = TestWebRequest.CreateForLocation((WebServerLocation)table["Location"]))
                    {
                        TestUtil.ClearConfiguration();

                        request.ServiceType = typeof(BlobReflectionDataService);
                        request.RequestUriString = "/$metadata";
                        Exception e = TestUtil.RunCatching(request.SendRequest);

                        TestUtil.AssertExceptionExpected(e, BLOBSupportTest.HasStreamAttributeInComplexType);

                        if (e != null)
                        {
                            return;
                        }
                        else
                        {
                            Assert.AreEqual(1, BlobDataServicePipelineHandlers.ProcessingRequestInvokeCount);
                            Assert.AreEqual(1, BlobDataServicePipelineHandlers.ProcessedRequestInvokeCount);
                            Assert.AreEqual(0, BlobDataServicePipelineHandlers.ProcessingChangesetInvokeCount);
                            Assert.AreEqual(0, BlobDataServicePipelineHandlers.ProcessedChangesetInvokeCount);
                        }

                        string path = Path.Combine(targetPath, "BlobClrMetadataTest" + Enum.GetName(typeof(WebServerLocation), table["Location"]) + ".xml");
                        System.Diagnostics.Trace.WriteLine("Documents saved to " + path);
                        var model = MetadataUtil.IsValidMetadata(request.GetResponseStream(), path);

                        var types = model.SchemaElements.OfType<IEdmStructuredType>().ToArray();

                        foreach (IEdmSchemaType t in types)
                        {
                            if (t.TypeKind == EdmTypeKind.Entity && ((IEdmEntityType)t).HasStream)
                            {
                                // the HasDefaultStream method takes inheritance into account
                                Assert.IsTrue(t.Name == typeof(Photo).Name || t.Name == typeof(DerivedFromPhoto).Name);
                            }
                            else
                            {
                                Assert.AreNotEqual(t.Name, typeof(Photo).Name);
                            }
                        }

                        DataServiceStreamProvider.ValidateInstantiatedInstances();
                    }
                });
            }

            [TestCategory("Partition1"), TestMethod, Variation]
            public void BlobEdmMetadataTest()
            {
                this.EnsureTestHasNoLeakedStreams();

                string[] attributeValues = new string[]
                {
                    "$default",
                    "true",
                    "",
                    "foo"
                };

                string[] typesToApplyAttribute = new string[]
                {
                    "Customers",
                    "Orders"
                    /*TODO: add complex type*/
                };

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("AttributeValues", attributeValues),
                    new Dimension("TypesToApplyAttribute", typesToApplyAttribute));

                TestUtil.RunCombinatorialEngineFail(engine, table =>
                {
                    string attributeValue = (string)table["AttributeValues"];
                    string typeToApplyAttribute = (string)table["TypesToApplyAttribute"];

                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                    using (NorthwindDefaultStreamService.SetupNorthwindWithStreamAndETag(
                        new KeyValuePair<string, string>[] { new KeyValuePair<string, string>(typeToApplyAttribute, attributeValue) },
                        null,
                        "BlobSupportTest_BlobEdmMetadataTest"))
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        // CachedConnections.RecycleDatabase(CachedConnections.BaseModelType.Northwind);

                        request.ServiceType = typeof(NorthwindDefaultStreamService);
                        request.RequestUriString = "/$metadata";

                        // Request metadata and check whether an exception was thrown.
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception, attributeValue != "true");

                        if (exception != null)
                        {
                            // If we failed to get $metadata, there are no further checks to be made.
                            return;
                        }
                        else
                        {
                            Assert.AreEqual(1, BlobDataServicePipelineHandlers.ProcessingRequestInvokeCount);
                            Assert.AreEqual(1, BlobDataServicePipelineHandlers.ProcessedRequestInvokeCount);
                            Assert.AreEqual(0, BlobDataServicePipelineHandlers.ProcessingChangesetInvokeCount);
                            Assert.AreEqual(0, BlobDataServicePipelineHandlers.ProcessedChangesetInvokeCount);
                        }

                        var metadata = MetadataUtil.IsValidMetadata(
                            request.GetResponseStream(),
                            Path.Combine(Path.Combine(TestUtil.GeneratedFilesLocation, "BlobSupportTest_BlobEdmMetadataTest"), "Northwind.WithStream.csdl.returned.xml"));

                        var types = metadata.SchemaElements.OfType<IEdmStructuredType>().ToArray();

                        foreach (IEdmSchemaType t in types)
                        {
                            if (t.TypeKind == EdmTypeKind.Entity && ((IEdmEntityType)t).HasStream)
                            {
                                Assert.IsTrue(t.Name == typeToApplyAttribute);
                            }
                            else
                            {
                                Assert.IsTrue(t.Name != typeToApplyAttribute);
                            }
                        }

                        DataServiceStreamProvider.ValidateInstantiatedInstances();
                    }
                });
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void BlobETagTest()
            {
                this.EnsureTestHasNoLeakedStreams();
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcess))
                {
                    string blobETag = "\"BlobETag123\"";
                    DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => blobETag;
                    PhotoDataServiceContext.PopulateData();
                    int id = PhotoDataServiceContext.NextItemID;
                    int rating = 5;
                    string photoPayload = GetPhotoPayload(UnitTestsUtil.AtomFormat, request.BaseUri, id, "sample photo payload", "sample photo", rating, new byte[] { 1, 2, 3, 4 });
                    string binaryContentType = DataServiceStreamProvider.GetContentType(new Photo() { ID = id });
                    BLOBSupportTest.ValidateInterceptorOverride = () =>
                    {
                        InterceptorChecker.ValidateQueryInterceptor(2);
                        InterceptorChecker.ValidateChangeInterceptor(2);
                    };
                    request.RequestHeaders["CustomRequestHeader_ItemType"] = typeof(Photo).FullName;
                    Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "POST", "/Folders(0)/Items", null, null, UnitTestsUtil.AtomFormat, binaryContentType, null, photoPayload, 201));
                }
                this.EnsureTestHasNoLeakedStreams();
            }

            // ODataLib now correctly ignores __deferred properties in WCF DS Server.
            [TestCategory("Partition1"), TestMethod, Variation]
            public void BlobObjectContextTest()
            {
                this.EnsureTestHasNoLeakedStreams();

                int bufferSize = 1024 * 64;
                if (DataServiceStreamProvider.DefaultStreamBufferSize > 0)
                {
                    bufferSize = DataServiceStreamProvider.DefaultStreamBufferSize;
                }

                byte[][] buffers = new byte[][]
                {
                    new byte[0],
                    new byte[bufferSize - 1],
                    new byte[bufferSize],
                    new byte[bufferSize + 1]
                };

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Buffers", buffers),
                    new Dimension("Format", UnitTestsUtil.ResponseFormats),
                    new Dimension("Location", new WebServerLocation[] { WebServerLocation.InProcessStreamedWcf, WebServerLocation.InProcess, WebServerLocation.InProcessWcf }));

                using (NorthwindDefaultStreamService.SetupNorthwindWithStreamAndETag(
                    new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("Customers", "true") },
                    null,
                    "BlobSupportTest_BlobProjectionTests_EFProvider"))
                {
                    string blobETag = "\"BlobETag123\"";
                    DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => blobETag;

                    TestUtil.RunCombinatorialEngineFail(engine, table =>
                    {
                        WebServerLocation location = (WebServerLocation)table["Location"];
                        bool isChunkedRequest = location == WebServerLocation.InProcessStreamedWcf;

                        using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                        using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                        using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
                        {
                            byte[] buffer = (byte[])table["Buffers"];
                            FillBuffer(buffer);
                            string format = (string)table["Format"];
                            string slug = "C" + TestUtil.Random.Next(1000, 9999).ToString();

                            string expectedID = slug;
                            NorthwindModel.Customers customer = NorthwindModel.Customers.CreateCustomers(expectedID, "");
                            string contentType = DataServiceStreamProvider.GetContentType(customer);

                            // POST Media Link Entry -- not supported
                            // When the server sees POST on the MLE, it simply tries to create the MR. IDataServiceStreamProvider.GetWriteStream() should fail because the content type is incorrect.
                            string payload = GetCustomerPayload(format, request.BaseUri, expectedID, "Microsoft", "Address", "City");
                            Assert.IsNotNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "POST", "/Customers", null, null, format, format, slug, payload, 500));

                            // POST Media Resource
                            Assert.IsNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "POST", "/Customers", null, null, format, contentType, slug, buffer, 201));
                            payload = request.GetResponseStreamAsText();
                            ValidateMediaResourceFromStorage(typeof(NorthwindModel.Customers), request, expectedID, buffer);

                            if (format == UnitTestsUtil.AtomFormat)
                            {
                                payload = Regex.Replace(payload, @"\s*<link\s+rel=""(?:(?!edit)|(?!edit-media)).*?/>\s*", "", RegexOptions.Multiline | RegexOptions.Compiled);
                            }

                            // PATCH the same response payload back to the MLE.
                            Assert.IsNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "PATCH", "/Customers('" + expectedID + "')", null, null, format, format, slug, payload, 200, true));
                            ValidateCustomersMLEFromResponse(request, format, expectedID, "Microsoft", null, null, false);

                            // GET Media Resource
                            Assert.IsNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "GET", "/Customers('" + expectedID + "')/$value", blobETag, null, null, contentType, slug, null, 200));
                            ValidateMediaResourceFromGet(typeof(NorthwindModel.Customers), request, expectedID, buffer);

                            // GET Media Link Entry
                            Assert.IsNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "GET", "/Customers('" + expectedID + "')", null, null, format, null, slug, null, 200));
                            ValidateCustomersMLEFromResponse(request, format, expectedID, "Microsoft", null, null, false);

                            // PUT Media Resource
                            FillBuffer(buffer);
                            Assert.IsNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "PUT", "/Customers('" + expectedID + "')/$value", blobETag, null, null, contentType, slug, buffer, 204));
                            Assert.AreEqual("", request.GetResponseStreamAsText());
                            ValidateMediaResourceFromStorage(typeof(NorthwindModel.Customers), request, expectedID, buffer);
                            Assert.IsNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "GET", "/Customers('" + expectedID + "')/$value", blobETag, null, null, contentType, slug, null, 200));
                            ValidateMediaResourceFromGet(typeof(NorthwindModel.Customers), request, expectedID, buffer);

                            // PATCH Media Resource -- Not supported. Expect 405.
                            Assert.IsNotNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "PATCH", "/Customers('" + expectedID + "')/$value", blobETag, null, null, contentType, slug, new byte[] { 1, 2, 3, 4 }, 405));
                            // The stream property for Customer should not be updated.
                            ValidateMediaResourceFromStorage(typeof(NorthwindModel.Customers), null, expectedID, buffer);
                            Assert.IsNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "GET", "/Customers('" + expectedID + "')/$value", blobETag, null, null, contentType, slug, null, 200));
                            ValidateMediaResourceFromGet(typeof(NorthwindModel.Customers), request, expectedID, buffer);

                            // PATCH Media Link Entry
                            payload = GetCustomerPayload(format, request.BaseUri, expectedID, "Microsoft", "Address", "City");
                            Assert.IsNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "PATCH", "/Customers('" + expectedID + "')", null, null, format, format, slug, payload, 200, true));
                            ValidateCustomersMLEFromResponse(request, format, expectedID, "Microsoft", "Address", "City", false);

                            // PUT Media Link Entry
                            payload = GetCustomerPayload(format, request.BaseUri, expectedID, "Microsoft", "One Microsoft Way", "Redmond");
                            Assert.IsNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "PUT", "/Customers('" + expectedID + "')", null, null, format, format, slug, payload, 200, true));
                            ValidateCustomersMLEFromResponse(request, format, expectedID, "Microsoft", "One Microsoft Way", "Redmond", false);

                            /* For chunked requests we need to send a "0" meaning that this is the last chunk. In this case we need to provide contentType as well */
                            // DELETE Media Resource -- not supported
                            Assert.IsNotNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "DELETE", "/Customers('" + expectedID + "')/$value", blobETag, null, null, isChunkedRequest ? "text/plain" : null, slug, isChunkedRequest ? "0" : null, 405));

                            // DELETE Media Link Entry
                            string path = DataServiceStreamProvider.GetStoragePath(customer);
                            Assert.IsTrue(File.Exists(path));
                            Assert.IsNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "DELETE", "/Customers('" + expectedID + "')", null, null, null, isChunkedRequest ? "text/plain" : null, slug, isChunkedRequest ? "0" : null, 204));
                            Assert.IsFalse(File.Exists(path));

                            Assert.AreEqual("[DataServiceStreamProvider-GetWriteStream-Dispose][DataServiceStreamProvider-GetWriteStream-StreamBufferSize-GetStreamETag-GetReadStreamUri-GetStreamContentType-Dispose][DataServiceStreamProvider-GetStreamETag-GetReadStreamUri-GetStreamContentType-Dispose][DataServiceStreamProvider-GetStreamContentType-GetReadStream-GetStreamETag-StreamBufferSize-Dispose][DataServiceStreamProvider-GetStreamETag-GetReadStreamUri-GetStreamContentType-Dispose][DataServiceStreamProvider-GetWriteStream-StreamBufferSize-GetStreamETag-Dispose][DataServiceStreamProvider-GetStreamContentType-GetReadStream-GetStreamETag-StreamBufferSize-Dispose][DataServiceStreamProvider-GetStreamContentType-GetReadStream-GetStreamETag-StreamBufferSize-Dispose][DataServiceStreamProvider-GetStreamETag-GetReadStreamUri-GetStreamContentType-Dispose][DataServiceStreamProvider-GetStreamETag-GetReadStreamUri-GetStreamContentType-Dispose][DataServiceStreamProvider-DeleteStream-Dispose]", DataServiceStreamProvider.CallOrderLog);
                        }
                    });
                }

                this.EnsureTestHasNoLeakedStreams();
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void EdmBlobWithInheritanceTest()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Format", UnitTestsUtil.ResponseFormats),
                    new Dimension("Location", new WebServerLocation[] { WebServerLocation.InProcessStreamedWcf, WebServerLocation.InProcess, WebServerLocation.InProcessWcf }));

                TestUtil.RunCombinatorialEngineFail(engine, table =>
                {
                    WebServerLocation location = (WebServerLocation)table["Location"];
                    bool isChunkedRequest = location == WebServerLocation.InProcessStreamedWcf;
                    string format = (string)table["Format"];

                    using (var conn = ocs.PopulateData.CreateTableAndPopulateData())
                    using (TestUtil.RestoreStaticValueOnDispose(typeof(OpenWebDataServiceHelper), "ForceVerboseErrors"))
                    using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
                    {
                        string expectedID = "1";

                        OpenWebDataServiceHelper.ForceVerboseErrors = true;

                        // GET an instance of a type derived from a type marked with HasStream=true.                        
                        request.DataServiceType = typeof(ocs.CustomObjectContext);
                        request.RequestUriString = "/CustomerBlobs(" + expectedID + ")";
                        request.HttpMethod = "GET";
                        request.Accept = format;

                        Exception e = TestUtil.RunCatching(request.SendRequest);

                        if (e != null)
                        {
                            System.Diagnostics.Trace.WriteLine(string.Format("Exception from SendRequest():\r\n{0}\r\nCall Stack:\r\n{1}", e.Message, e.StackTrace));
                        }

                        Assert.IsNull(e);

                        // Check that the response is an MLE.    
                        Assert.AreEqual(200, request.ResponseStatusCode);

                        const string expectedTypeName = "AstoriaUnitTests.ObjectContextStubs.Types.CustomerBlobWithBirthday";
                        string baseUriWithSlash = request.BaseUri.EndsWith("/") ? request.BaseUri : request.BaseUri + "/";
                        string expectedIdUri = baseUriWithSlash + "CustomerBlobs(" + expectedID + ")";
                        string expectedEditLink = "CustomerBlobs(" + expectedID + ")" + "/" + expectedTypeName;
                        string expectedEditMediaLink = expectedEditLink + "/$value";
                        string expectedSrcUri = ocs.CustomObjectContext.DummyReadStreamUri.OriginalString;
                        string expectedContentType = ocs.CustomObjectContext.DummyContentType;

                        string[] xPaths = null;
                        xPaths = new string[] {
                            String.Format(@"/atom:entry[atom:category/@term=""#{0}"" and
                                        atom:id=""{1}"" and
                                        atom:content[@type=""{2}"" and @src=""{3}""] and
                                        atom:link[@rel=""edit-media"" and @href =""{4}"" ] and
                                        atom:link[@rel=""edit"" and @title=""CustomerBlob"" and @href =""{5}""] and
                                        adsm:properties[ads:ID=""{6}""]
                            ]",
                            expectedTypeName,
                            expectedIdUri,
                            expectedContentType,
                            expectedSrcUri,
                            expectedEditMediaLink,
                            expectedEditLink,
                            expectedID)
                        };

                        UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), format, xPaths);
                    }
                });
            }

            // ODataLib now correctly ignores __deferred properties in WCF DS Server.
            [TestCategory("Partition2"), TestMethod, Variation]
            public void BlobReflectionContextItemTest()
            {
                Assert.AreEqual(0, DataServiceStreamProvider.UnDisposedInstances,
                    "Expecting 0 DataServiceStreamProvider.UnDisposedInstances, but found " +
                    DataServiceStreamProvider.UnDisposedInstances + " - any instances here indicate some previous test leaked them");

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Accept", UnitTestsUtil.ResponseFormats),
                    new Dimension("ContentType", UnitTestsUtil.ResponseFormats),
                    new Dimension("Location", new WebServerLocation[] { WebServerLocation.InProcess }));

                TestUtil.RunCombinatorialEngineFail(engine, table =>
                {
                    WebServerLocation location = (WebServerLocation)table["Location"];
                    bool isChunkedRequest = location == WebServerLocation.InProcessStreamedWcf;
                    string accept = (string)table["Accept"];
                    string contentType = (string)table["ContentType"];

                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                    using (PhotoDataServiceContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
                    {
                        int id = TestUtil.Random.Next(10, int.MaxValue);
                        string description = string.Format("Item {0} Description", id);
                        string name = string.Format("Item {0}", id);
                        string itemPayload = GetItemPayload(contentType, request.BaseUri, id, description, name);

                        // POST non-MLE
                        request.RequestHeaders["CustomRequestHeader_ItemType"] = typeof(Item).FullName;
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "POST", "/Items", null, null, accept, contentType, null, itemPayload, 201));

                        itemPayload = request.GetResponseStreamAsText();
                        ValidateItemOnContext(id, description, name);

                        if (accept == UnitTestsUtil.AtomFormat)
                        {
                            itemPayload = Regex.Replace(itemPayload, @"\s*<link\s+rel=""(?:(?!edit)|(?!edit-media)).*?/>\s*", "", RegexOptions.Multiline | RegexOptions.Compiled);
                        }

                        // PATCH the same response payload back to the Item.
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "PATCH", "/Items(" + id + ")", request.ResponseETag, null, accept, accept, null, itemPayload, 204));
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(" + id + ")", request.ResponseETag, null, accept, null, null, null, 200));
                        ValidateItemFromResponse(request, accept, id, description, name);

                        // GET non-MLE/$value
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(" + id + ")/$value", null, null, null, null, null, null, 400));

                        // GET non-MLE
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(" + id + ")", null, null, accept, null, null, null, 200));
                        ValidateItemFromResponse(request, accept, id, description, name);
                        string etag = request.ResponseETag;

                        // PATCH non-MLE/$value
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "PATCH", "/Items(" + id + ")/$value", etag, null, null, contentType, null, itemPayload, 400));

                        // PATCH non-MLE
                        description = description + TestUtil.Random.Next().ToString();
                        name = name + TestUtil.Random.Next().ToString();
                        itemPayload = GetItemPayload(contentType, request.BaseUri, id, description, name);

                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "PATCH", "/Items(" + id + ")", etag, null, null, contentType, null, itemPayload, 204));
                        Assert.AreEqual("", request.GetResponseStreamAsText());
                        ValidateItemOnContext(id, description, name);
                        etag = request.ResponseETag;
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(" + id + ")", etag, null, accept, null, null, null, 200));
                        ValidateItemFromResponse(request, accept, id, description, name);

                        // PUT non-MLE/$value
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(" + id + ")/$value", etag, null, null, contentType, null, itemPayload, 400));

                        // PUT non-MLE
                        description = description + TestUtil.Random.Next().ToString();
                        name = name + TestUtil.Random.Next().ToString();
                        itemPayload = GetItemPayload(contentType, request.BaseUri, id, description, name);

                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(" + id + ")", etag, null, null, contentType, null, itemPayload, 204));
                        Assert.AreEqual("", request.GetResponseStreamAsText());
                        ValidateItemOnContext(id, description, name);
                        etag = request.ResponseETag;
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(" + id + ")", etag, null, accept, null, null, null, 200));
                        ValidateItemFromResponse(request, accept, id, description, name);

                        /* For chunked requests we need to send a "0" meaning that this is the last chunk. In this case we need to provide contentType as well */
                        // DELETE non-MLE/$value
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(" + id + ")/$value", etag, null, null, isChunkedRequest ? "text/plain" : null, null, isChunkedRequest ? "0" : null, 405));

                        // DELETE Media Link Entry
                        Item i = FindItem(id);
                        Assert.IsNotNull(i);
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(" + id + ")", etag, null, null, isChunkedRequest ? "text/plain" : null, null, isChunkedRequest ? "0" : null, 204));
                        Assert.IsNull(FindItem(id));

                        Assert.AreEqual("[DataServiceStreamProvider-ResolveType-Dispose]", DataServiceStreamProvider.CallOrderLog);
                    }
                });

                this.EnsureTestHasNoLeakedStreams();
            }

            // ODataLib now correctly ignores __deferred properties in WCF DS Server.
            [TestCategory("Partition2"), TestMethod, Variation]
            public void BlobReflectionContextPhotoTest()
            {
                this.EnsureTestHasNoLeakedStreams();

                int oneK = 1024;
                int oneMB = oneK * oneK;
                int bufferSize = 64 * oneK;
                if (DataServiceStreamProvider.DefaultStreamBufferSize > 0)
                {
                    bufferSize = DataServiceStreamProvider.DefaultStreamBufferSize;
                }

                byte[] smallBuffer = new byte[200];
                byte[] largeBuffer = new byte[TestUtil.Random.Next(10 * oneMB, 20 * oneMB)];

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Accept", UnitTestsUtil.ResponseFormats),
                    new Dimension("Location", new WebServerLocation[] { WebServerLocation.InProcessStreamedWcf, WebServerLocation.InProcess, WebServerLocation.InProcessWcf }));

                TestUtil.RunCombinatorialEngineFail(engine, table =>
                {
                    WebServerLocation location = (WebServerLocation)table["Location"];
                    bool isChunkedRequest = location == WebServerLocation.InProcessStreamedWcf;

                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                    using (PhotoDataServiceContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForLocation((WebServerLocation)table["Location"]))
                    {
                        byte[] buffer = isChunkedRequest ? largeBuffer : smallBuffer;
                        FillBuffer(buffer);
                        string accept = (string)table["Accept"];

                        int id = TestUtil.Random.Next(10, int.MaxValue);
                        string description = string.Format("Photo {0} Description", id);
                        string name = string.Format("Photo {0}", id);
                        int rating = TestUtil.Random.Next();
                        string photoPayload = GetPhotoPayload(accept, request.BaseUri, id, description, name, rating, new byte[] { 1, 2, 3, 4 });

                        string slug = id.ToString();
                        string binaryContentType = DataServiceStreamProvider.GetContentType(new Photo() { ID = id });
                        string blobETag = "\"BlobETag123\"";
                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => blobETag;

                        // POST MLE -- not supported
                        // When the server sees POST on the MLE, it simply tries to create the MR. IDataServiceStreamProvider.GetWriteStream() should fail because the content type is incorrect.
                        request.RequestHeaders["CustomRequestHeader_ItemType"] = typeof(Photo).FullName;
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "POST", "/Items", null, null, accept, accept, slug, photoPayload, 500));

                        // POST MR
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "POST", "/Items", null, null, accept, binaryContentType, slug, buffer, 201));
                        ValidateMediaResourceFromStorage(typeof(Photo), request, id, buffer);

                        photoPayload = request.GetResponseStreamAsText();

                        if (accept == UnitTestsUtil.AtomFormat)
                        {
                            photoPayload = Regex.Replace(photoPayload, @"\s*<link\s+rel=""(?:(?!edit)|(?!edit-media)).*?/>\s*", "", RegexOptions.Multiline | RegexOptions.Compiled);
                        }

                        // PATCH the same response payload back to the MLE.
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "PATCH", "/Items(" + id + ")", request.ResponseETag, null, accept, accept, null, photoPayload, 204));
                        Assert.AreEqual("", request.GetResponseStreamAsText());
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(" + id + ")", request.ResponseETag, null, accept, null, null, null, 200));
                        ValidatePhotoMLEFromResponse(request, accept, typeof(Photo), id, null, null, 0, null, request.ResponseETag, blobETag, false);
                        string mleETag = request.ResponseETag;

                        // GET Media Resource
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(" + id + ")/$value", null, null, null, null, null, null, 200));
                        ValidateMediaResourceFromGet(typeof(Photo), request, id, buffer);
                        Assert.AreEqual(blobETag, request.ResponseETag);

                        // GET Media Link Entry
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(" + id + ")", mleETag, null, accept, null, null, null, 200));
                        ValidatePhotoMLEFromResponse(request, accept, typeof(Photo), id, null, null, 0, null, request.ResponseETag, blobETag, false);
                        mleETag = request.ResponseETag;

                        // PUT Media Resource
                        FillBuffer(buffer);
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(" + id + ")/$value", blobETag, null, null, binaryContentType, null, buffer, 204));
                        Assert.AreEqual("", request.GetResponseStreamAsText());
                        ValidateMediaResourceFromStorage(typeof(Photo), request, id, buffer);
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(" + id + ")/$value", request.ResponseETag, null, null, null, null, null, 200));
                        ValidateMediaResourceFromGet(typeof(Photo), request, id, buffer);

                        // PATCH Media Resource -- Not supported. Expect 405.
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "PATCH", "/Items(" + id + ")/$value", request.ResponseETag, null, null, binaryContentType, null, buffer, 405));
                        // The stream property for Customer should not be updated.
                        ValidateMediaResourceFromStorage(typeof(Photo), null, id, buffer);
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(" + id + ")/$value", request.ResponseETag, null, null, null, null, null, 200));
                        ValidateMediaResourceFromGet(typeof(Photo), request, id, buffer);

                        // Blob update modifies Photo.LastUpdated timestamp, we need to get the new etag value
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(" + id + ")", null, null, accept, null, null, null, 200));

                        // PATCH Media Link Entry
                        description = "Merged Description";
                        name = "Merged Name";
                        photoPayload = GetPhotoPayload(accept, request.BaseUri, id, description, name, 4321, new byte[] { 4, 3, 2, 1 });
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "PATCH", "/Items(" + id + ")", request.ResponseETag, null, null, accept, null, photoPayload, 204));
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(" + id + ")", request.ResponseETag, null, accept, null, null, null, 200));
                        ValidatePhotoMLEFromResponse(request, accept, typeof(Photo), id, description, name, 4321, new byte[] { 4, 3, 2, 1 }, request.ResponseETag, blobETag, false);

                        // PUT Media Link Entry
                        description = null;
                        name = null;
                        photoPayload = GetPhotoPayload(accept, request.BaseUri, id, description, name, 1234, new byte[] { 1, 2, 3, 4 });
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(" + id + ")", request.ResponseETag, null, null, accept, null, photoPayload, 204));
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(" + id + ")", request.ResponseETag, null, accept, null, null, null, 200));
                        ValidatePhotoMLEFromResponse(request, accept, typeof(Photo), id, description, name, 1234, new byte[] { 1, 2, 3, 4 }, request.ResponseETag, blobETag, false);
                        mleETag = request.ResponseETag;

                        /* For chunked requests we need to send a "0" meaning that this is the last chunk. In this case we need to provide contentType as well */
                        // DELETE Media Resource -- not supported
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(" + id + ")/$value", null, null, null, isChunkedRequest ? "text/plain" : null, null, isChunkedRequest ? "0" : null, 405));

                        // DELETE Media Link Entry
                        Photo p = FindPhoto(id);
                        Assert.IsNotNull(p);
                        string path = DataServiceStreamProvider.GetStoragePath(p);
                        Assert.IsTrue(File.Exists(path));
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(" + id + ")", mleETag, null, null, isChunkedRequest ? "text/plain" : null, null, isChunkedRequest ? "0" : null, 204));
                        Assert.IsNull(FindPhoto(id));
                        Assert.IsFalse(File.Exists(path));

                        // GET Media Link Entry for photo entity of a derived type that does not explicitly declare the [HasStreamAttribute] but should be treated as though it does.
                        DerivedFromPhoto derivedPhoto = PhotoDataServiceContext._items.Where(i => i.GetType() == typeof(DerivedFromPhoto)).First() as DerivedFromPhoto;
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(" + derivedPhoto.ID + ")", null, null, accept, null, null, null, 200));
                        ValidatePhotoMLEFromResponse(request, accept, typeof(DerivedFromPhoto), derivedPhoto.ID, derivedPhoto.Description, derivedPhoto.Name, derivedPhoto.Rating, derivedPhoto.ThumbNail, request.ResponseETag, null, false);
                        mleETag = request.ResponseETag;

                        Assert.AreEqual("[DataServiceStreamProvider-ResolveType-GetWriteStream-Dispose][DataServiceStreamProvider-ResolveType-GetWriteStream-StreamBufferSize-GetStreamETag-GetReadStreamUri-GetStreamContentType-Dispose][DataServiceStreamProvider-GetStreamETag-GetReadStreamUri-GetStreamContentType-Dispose][DataServiceStreamProvider-GetStreamContentType-GetReadStream-GetStreamETag-StreamBufferSize-Dispose][DataServiceStreamProvider-GetStreamETag-GetReadStreamUri-GetStreamContentType-Dispose][DataServiceStreamProvider-GetWriteStream-StreamBufferSize-GetStreamETag-Dispose][DataServiceStreamProvider-GetStreamContentType-GetReadStream-GetStreamETag-StreamBufferSize-Dispose][DataServiceStreamProvider-GetStreamContentType-GetReadStream-GetStreamETag-StreamBufferSize-Dispose][DataServiceStreamProvider-GetStreamETag-GetReadStreamUri-GetStreamContentType-Dispose][DataServiceStreamProvider-GetStreamETag-GetReadStreamUri-GetStreamContentType-Dispose][DataServiceStreamProvider-GetStreamETag-GetReadStreamUri-GetStreamContentType-Dispose][DataServiceStreamProvider-DeleteStream-Dispose][DataServiceStreamProvider-GetStreamETag-GetReadStreamUri-GetStreamContentType-Dispose]", DataServiceStreamProvider.CallOrderLog);
                    }
                });
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void BlobReflectionContextPermissionTest()
            {
                this.EnsureTestHasNoLeakedStreams();

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Accept", UnitTestsUtil.ResponseFormats),
                    new Dimension("Location", new WebServerLocation[] { WebServerLocation.InProcess, WebServerLocation.InProcessWcf }));

                using (TestUtil.MetadataCacheCleaner())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, table =>
                    {
                        using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                        using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                        using (TestUtil.RestoreStaticMembersOnDispose(typeof(PhotoDataService)))
                        using (PhotoDataServiceContext.CreateChangeScope())
                        using (TestWebRequest request = TestWebRequest.CreateForLocation((WebServerLocation)table["Location"]))
                        {
                            PhotoDataService.Right = EntitySetRights.AllRead;

                            byte[] buffer = new byte[20];
                            FillBuffer(buffer);
                            string accept = (string)table["Accept"];

                            int id = TestUtil.Random.Next(10, int.MaxValue);
                            string description = string.Format("Photo {0} Description", id);
                            string name = string.Format("Photo {0}", id);
                            int rating = TestUtil.Random.Next();
                            string photoPayload = GetPhotoPayload(accept, request.BaseUri, id, description, name, rating, new byte[] { 1, 2, 3, 4 });

                            string slug = id.ToString();
                            string binaryContentType = DataServiceStreamProvider.GetContentType(new Photo() { ID = id });

                            // POST MLE
                            request.RequestHeaders["CustomRequestHeader_ItemType"] = typeof(Photo).FullName;
                            Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "POST", "/Items", null, null, accept, accept, slug, photoPayload, 403));

                            // POST MR
                            Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "POST", "/Items", null, null, accept, binaryContentType, slug, buffer, 403));

                            // PATCH the same response payload back to the MLE.
                            Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "PATCH", "/Items(" + id + ")", request.ResponseETag, null, accept, accept, null, photoPayload, 403));

                            // PUT Media Resource
                            Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(" + id + ")/$value", request.ResponseETag, null, null, binaryContentType, null, buffer, 403));

                            // DELETE Media Resource -- not supported
                            Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(" + id + ")/$value", String.Empty, null, null, null, null, null, 405));

                            // DELETE Media Link Entry
                            Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(" + id + ")", String.Empty, null, null, null, null, null, 403));

                            // INSERT a new MLE
                            TestUtil.ClearConfiguration();
                            PhotoDataService.Right = EntitySetRights.WriteAppend;
                            Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "POST", "/Items", null, null, UnitTestsUtil.AtomFormat, binaryContentType, slug, buffer, 201));

                            // DELETE an existing MLE
                            TestUtil.ClearConfiguration();
                            PhotoDataService.Right = EntitySetRights.WriteDelete | EntitySetRights.ReadSingle;
                            Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(" + id + ")", request.ResponseETag, null, null, null, null, null, 204));

                            // Get an existing MLE
                            TestUtil.ClearConfiguration();
                            PhotoDataService.Right = EntitySetRights.ReadSingle;
                            Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(" + 1 + ")", null, null, UnitTestsUtil.AtomFormat, null, null, null, 200));

                            // PATCH the same response payload back to the MLE.
                            TestUtil.ClearConfiguration();
                            photoPayload = GetPhotoPayload(UnitTestsUtil.AtomFormat, request.BaseUri, 1, description, name, rating, new byte[] { 1, 2, 3, 4 });
                            PhotoDataService.Right = EntitySetRights.WriteMerge | EntitySetRights.ReadSingle;
                            Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "PATCH", "/Items(" + 1 + ")", request.ResponseETag, null, UnitTestsUtil.AtomFormat, UnitTestsUtil.AtomFormat, null, photoPayload, 204));

                            // PUT the same response payload back to the MLE.
                            TestUtil.ClearConfiguration();
                            photoPayload = GetPhotoPayload(UnitTestsUtil.AtomFormat, request.BaseUri, 1, description, name, rating, new byte[] { 1, 2, 3, 4 });
                            PhotoDataService.Right = EntitySetRights.WriteReplace | EntitySetRights.ReadSingle;
                            Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(" + 1 + ")", request.ResponseETag, null, UnitTestsUtil.AtomFormat, UnitTestsUtil.AtomFormat, null, photoPayload, 204));

                            TestUtil.ClearConfiguration();
                        }
                    });
                }
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void BlobPostBindingTest()
            {
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(PhotoDataService)))
                using (PhotoDataServiceContext.CreateChangeScope())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    byte[] buffer = new byte[20];
                    FillBuffer(buffer);
                    string accept = UnitTestsUtil.AtomFormat;

                    int id = 500;
                    string slug = id.ToString();
                    string binaryContentType = DataServiceStreamProvider.GetContentType(new Photo() { ID = id });

                    int photoCount = PhotoDataServiceContext._items.Count;
                    int folderCount = PhotoDataServiceContext._folders.Count;

                    ///////////////////////////////////////
                    // Folders/Photos

                    // POST MR
                    request.RequestHeaders["CustomRequestHeader_ItemType"] = typeof(Photo).FullName;
                    Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "POST", "/Items", null, null, accept, binaryContentType, slug, buffer, 201));
                    Assert.AreEqual(++photoCount, PhotoDataServiceContext._items.Count);
                    Assert.AreEqual(folderCount, PhotoDataServiceContext._folders.Count);
                    Assert.IsNull(PhotoDataServiceContext._folders.Single(f => f.ID == 0).Items.SingleOrDefault(i => i.ID == id));

                    // Insert Link
                    request.RequestHeaders.Clear();
                    BLOBSupportTest.ValidateInterceptorOverride = () =>
                    {
                        InterceptorChecker.ValidateQueryInterceptor(3);
                        InterceptorChecker.ValidateChangeInterceptor(1);
                    };
                    string payload = string.Format("<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id='/Items({0})' />", id);
                    Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "POST", "/Folders(0)/Items/$ref", null, null, accept, UnitTestsUtil.MimeApplicationXml, null, payload, 204));
                    Assert.AreEqual(photoCount, PhotoDataServiceContext._items.Count);
                    Assert.AreEqual(folderCount, PhotoDataServiceContext._folders.Count);
                    Assert.IsNotNull(PhotoDataServiceContext._folders.Single(f => f.ID == 0).Items.SingleOrDefault(i => i.ID == id));

                    // Delete Link
                    BLOBSupportTest.ValidateInterceptorOverride = () =>
                    {
                        InterceptorChecker.ValidateQueryInterceptor(2);
                        InterceptorChecker.ValidateChangeInterceptor(1);
                    };
                    Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Folders(0)/Items/$ref?$id=Items(500)", null, null, null, null, null, null, 204));
                    Assert.AreEqual(photoCount, PhotoDataServiceContext._items.Count);
                    Assert.AreEqual(folderCount, PhotoDataServiceContext._folders.Count);
                    Assert.IsNull(PhotoDataServiceContext._folders.Single(f => f.ID == 0).Items.SingleOrDefault(i => i.ID == id));

                    // POST MR and Bind
                    id = 501;
                    slug = id.ToString();
                    binaryContentType = DataServiceStreamProvider.GetContentType(new Photo() { ID = id });
                    request.RequestHeaders["CustomRequestHeader_ItemType"] = typeof(Photo).FullName;
                    BLOBSupportTest.ValidateInterceptorOverride = () =>
                    {
                        InterceptorChecker.ValidateQueryInterceptor(2);
                        InterceptorChecker.ValidateChangeInterceptor(2);
                    };
                    Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "POST", "/Folders(0)/Items", null, null, accept, binaryContentType, slug, buffer, 201));
                    Assert.AreEqual(++photoCount, PhotoDataServiceContext._items.Count);
                    Assert.AreEqual(folderCount, PhotoDataServiceContext._folders.Count);
                    Assert.IsNotNull(PhotoDataServiceContext._folders.Single(f => f.ID == 0).Items.SingleOrDefault(i => i.ID == id));

                    // Delete MR through collection property
                    request.RequestHeaders.Clear();
                    BLOBSupportTest.ValidateInterceptorOverride = () =>
                    {
                        InterceptorChecker.ValidateQueryInterceptor(2);
                        InterceptorChecker.ValidateChangeInterceptor(1);
                    };
                    Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Folders(0)/Items(" + id + ")", request.ResponseETag, null, null, null, null, null, 204));
                    Assert.AreEqual(--photoCount, PhotoDataServiceContext._items.Count);
                    Assert.AreEqual(folderCount, PhotoDataServiceContext._folders.Count);
                    Assert.IsNull(PhotoDataServiceContext._items.SingleOrDefault(i => i.ID == id));
                    Assert.IsNull(PhotoDataServiceContext._folders.Single(f => f.ID == 0).Items.SingleOrDefault(i => i.ID == id));

                    ///////////////////////////////////////
                    // Photos/Folders

                    // Insert Link
                    request.RequestHeaders.Clear();
                    BLOBSupportTest.ValidateInterceptorOverride = () =>
                    {
                        InterceptorChecker.ValidateQueryInterceptor(3);
                        InterceptorChecker.ValidateChangeInterceptor(1);
                    };
                    payload = "<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id='/Folders(0)' />";
                    Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "POST", "/Items(500)/RelatedFolders/$ref", null, null, accept, UnitTestsUtil.MimeApplicationXml, null, payload, 204));
                    Assert.AreEqual(photoCount, PhotoDataServiceContext._items.Count);
                    Assert.AreEqual(folderCount, PhotoDataServiceContext._folders.Count);
                    Assert.IsNotNull(PhotoDataServiceContext._items.Single(i => i.ID == 500).RelatedFolders.SingleOrDefault(f => f.ID == 0));

                    // Delete Link
                    BLOBSupportTest.ValidateInterceptorOverride = () =>
                    {
                        InterceptorChecker.ValidateQueryInterceptor(2);
                        InterceptorChecker.ValidateChangeInterceptor(1);
                    };
                    Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(500)/RelatedFolders/$ref?$id=Folders(0)", null, null, null, null, null, null, 204));
                    Assert.AreEqual(photoCount, PhotoDataServiceContext._items.Count);
                    Assert.AreEqual(folderCount, PhotoDataServiceContext._folders.Count);
                    Assert.IsNull(PhotoDataServiceContext._items.Single(i => i.ID == 500).RelatedFolders.SingleOrDefault(f => f.ID == 0));

                    // POST folder and Bind to Photo
                    payload = "{ @odata.type: \"" + typeof(Folder).FullName + "\" , Name: \"Folder333\", ID: 333 }";
                    BLOBSupportTest.ValidateInterceptorOverride = () =>
                    {
                        InterceptorChecker.ValidateQueryInterceptor(2);
                        InterceptorChecker.ValidateChangeInterceptor(2);
                    };
                    Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "POST", "/Items(500)/RelatedFolders", null, null, accept, UnitTestsUtil.JsonLightMimeType, null, payload, 201));
                    Assert.AreEqual(photoCount, PhotoDataServiceContext._items.Count);
                    Assert.AreEqual(++folderCount, PhotoDataServiceContext._folders.Count);
                    Assert.IsNotNull(PhotoDataServiceContext._items.Single(i => i.ID == 500).RelatedFolders.SingleOrDefault(f => f.ID == 333));

                    // Delete Folder from collection property
                    BLOBSupportTest.ValidateInterceptorOverride = () =>
                    {
                        InterceptorChecker.ValidateQueryInterceptor(2);
                        InterceptorChecker.ValidateChangeInterceptor(1);
                    };
                    Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(500)/RelatedFolders(333)", null, null, null, null, null, null, 204));
                    Assert.AreEqual(photoCount, PhotoDataServiceContext._items.Count);
                    Assert.AreEqual(--folderCount, PhotoDataServiceContext._folders.Count);
                    Assert.IsNull(PhotoDataServiceContext._folders.SingleOrDefault(f => f.ID == 333));
                    Assert.IsNull(PhotoDataServiceContext._items.Single(i => i.ID == 500).RelatedFolders.SingleOrDefault(f => f.ID == 333));

                    ///////////////////////////////////////
                    // Photos/Photos

                    // Insert Link
                    request.RequestHeaders.Clear();
                    BLOBSupportTest.ValidateInterceptorOverride = () =>
                    {
                        InterceptorChecker.ValidateQueryInterceptor(3);
                        InterceptorChecker.ValidateChangeInterceptor(1);
                    };
                    payload = "<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id='/Items(1)' />";
                    Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "POST", "/Items(500)/RelatedItems/$ref", null, null, accept, UnitTestsUtil.MimeApplicationXml, null, payload, 204));
                    Assert.AreEqual(photoCount, PhotoDataServiceContext._items.Count);
                    Assert.AreEqual(folderCount, PhotoDataServiceContext._folders.Count);
                    Assert.IsNotNull(PhotoDataServiceContext._items.Single(i => i.ID == 500).RelatedItems.SingleOrDefault(i => i.ID == 1));

                    // Delete Link
                    BLOBSupportTest.ValidateInterceptorOverride = () =>
                    {
                        InterceptorChecker.ValidateQueryInterceptor(2);
                        InterceptorChecker.ValidateChangeInterceptor(1);
                    };
                    Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(500)/RelatedItems/$ref?$id=Items(1)", null, null, null, null, null, null, 204));
                    Assert.AreEqual(photoCount, PhotoDataServiceContext._items.Count);
                    Assert.AreEqual(folderCount, PhotoDataServiceContext._folders.Count);
                    Assert.IsNull(PhotoDataServiceContext._items.Single(i => i.ID == 500).RelatedItems.SingleOrDefault(i => i.ID == 1));

                    // POST MR and Bind
                    id = 502;
                    slug = id.ToString();
                    binaryContentType = DataServiceStreamProvider.GetContentType(new Photo() { ID = id });
                    request.RequestHeaders["CustomRequestHeader_ItemType"] = typeof(Photo).FullName;
                    BLOBSupportTest.ValidateInterceptorOverride = () =>
                    {
                        InterceptorChecker.ValidateQueryInterceptor(2);
                        InterceptorChecker.ValidateChangeInterceptor(2);
                    };
                    Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "POST", "/Items(500)/RelatedItems", null, null, accept, binaryContentType, slug, buffer, 201));
                    Assert.AreEqual(++photoCount, PhotoDataServiceContext._items.Count);
                    Assert.AreEqual(folderCount, PhotoDataServiceContext._folders.Count);
                    Assert.IsNotNull(PhotoDataServiceContext._items.Single(i => i.ID == 500).RelatedItems.SingleOrDefault(i => i.ID == id));

                    // Delete MR through collection property
                    BLOBSupportTest.ValidateInterceptorOverride = () =>
                    {
                        InterceptorChecker.ValidateQueryInterceptor(2);
                        InterceptorChecker.ValidateChangeInterceptor(1);
                    };
                    Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(500)/RelatedItems(" + id + ")", request.ResponseETag, null, null, null, null, null, 204));
                    Assert.AreEqual(--photoCount, PhotoDataServiceContext._items.Count);
                    Assert.AreEqual(folderCount, PhotoDataServiceContext._folders.Count);
                    Assert.IsNull(PhotoDataServiceContext._items.SingleOrDefault(i => i.ID == id));
                    Assert.IsNull(PhotoDataServiceContext._items.Single(i => i.ID == 500).RelatedItems.SingleOrDefault(i => i.ID == id));
                }
            }

            [TestCategory("Partition2"), TestMethod]
            public void BlobEntryWithNoProperties()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                using (PhotoDataServiceContext.CreateChangeScope())
                using (TestWebRequest req = TestWebRequest.CreateForInProcessWcf())
                {
                    DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "\"BlobETag123\"";
                    DataServiceStreamProvider.GetReadStreamUriOverride = (entity, operationContext) => new Uri("http://localhost");
                    DataServiceStreamProvider.GetStreamContentTypeOverride = (entity, operationContext) => "image/jpeg";
                    DataServiceStreamProvider.SkipValidation = true;

                    req.DataServiceType = typeof(PhotoDataService);
                    req.StartService();

                    DataServiceContext ctx = new DataServiceContext(req.ServiceRoot);
                    ctx.EnableAtom = true;
                    ctx.Format.UseAtom();
                    var q = from p in ctx.CreateQuery<Photo>("Items") where p.ParentFolder != null select new { p.ParentFolder.ID };

                    bool foundEntry = false;
                    foreach (var v in q)
                    {
                        foundEntry = true;
                    }

                    Assert.IsTrue(foundEntry, "Must get at least one item.");
                }
            }

            // ODataLib now accepts MLE payloads without content element.
            [TestCategory("Partition2"), TestMethod, Variation]
            public void BlobUpdateMLEWithEmptyContentETagTest()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                using (PhotoDataServiceContext.CreateChangeScope())
                using (TestWebRequest req = TestWebRequest.CreateForInProcessWcf())
                {
                    DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "\"BlobETag123\"";
                    DataServiceStreamProvider.GetReadStreamUriOverride = (entity, operationContext) => new Uri("http://localhost");
                    DataServiceStreamProvider.GetStreamContentTypeOverride = (entity, operationContext) => "image/jpeg";
                    DataServiceStreamProvider.SkipValidation = true;

                    req.DataServiceType = typeof(PhotoDataService);

                    string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                       AtomUpdatePayloadBuilder.GetCategoryXml(typeof(Photo).FullName) +
                    "</entry>";

                    req.RequestUriString = "/Items(1)";
                    req.HttpMethod = "PUT";
                    req.IfMatch = "W/\"sdfght\""; // Wrong ETag
                    req.SetRequestStreamAsText(atomPayload);
                    req.RequestContentType = UnitTestsUtil.AtomFormat;
                    Exception e = TestUtil.RunCatching(req.SendRequest);
                    Assert.IsNotNull(e, "The request should have failed.");
                    Assert.AreEqual(412, req.ResponseStatusCode, "The request should have failed due to mismatch in ETags");
                }
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void BlobUpdateBindingTest()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Method", new string[] { "PUT", "PATCH" }));

                TestUtil.RunCombinatorialEngineFail(engine, table =>
                {
                    string method = (string)table["Method"];

                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(PhotoDataService)))
                    using (PhotoDataServiceContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        byte[] buffer = new byte[20];
                        FillBuffer(buffer);
                        string accept = UnitTestsUtil.AtomFormat;

                        int id = 500;
                        string slug = id.ToString();
                        string binaryContentType = DataServiceStreamProvider.GetContentType(new Photo() { ID = id });

                        int photoCount = PhotoDataServiceContext._items.Count;
                        int folderCount = PhotoDataServiceContext._folders.Count;

                        ///////////////////////////////////////
                        // Folders/Photos

                        // POST MR
                        request.RequestHeaders["CustomRequestHeader_ItemType"] = typeof(Photo).FullName;
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "POST", "/Items", null, null, accept, binaryContentType, slug, buffer, 201));
                        Assert.AreEqual(++photoCount, PhotoDataServiceContext._items.Count);
                        Assert.AreEqual(folderCount, PhotoDataServiceContext._folders.Count);
                        Assert.IsNull(PhotoDataServiceContext._folders.Single(f => f.ID == 0).Items.SingleOrDefault(i => i.ID == id));

                        // Set Link
                        request.RequestHeaders.Clear();
                        BLOBSupportTest.ValidateInterceptorOverride = () =>
                        {
                            InterceptorChecker.ValidateQueryInterceptor(3);
                            InterceptorChecker.ValidateChangeInterceptor(1);
                        };
                        string payload = string.Format("<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id='/Items({0})' />", id);
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, method, "/Folders(0)/Icon/$ref", null, null, accept, UnitTestsUtil.MimeApplicationXml, null, payload, 204));
                        Assert.AreEqual(photoCount, PhotoDataServiceContext._items.Count);
                        Assert.AreEqual(folderCount, PhotoDataServiceContext._folders.Count);
                        Assert.IsTrue(PhotoDataServiceContext._folders.Single(f => f.ID == 0).Icon.ID == id);

                        // Delete Link
                        BLOBSupportTest.ValidateInterceptorOverride = () =>
                        {
                            InterceptorChecker.ValidateQueryInterceptor(2);
                            InterceptorChecker.ValidateChangeInterceptor(1);
                        };
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Folders(0)/Icon/$ref", null, null, null, null, null, null, 204));
                        Assert.AreEqual(photoCount, PhotoDataServiceContext._items.Count);
                        Assert.AreEqual(folderCount, PhotoDataServiceContext._folders.Count);
                        Assert.IsNull(PhotoDataServiceContext._folders.Single(f => f.ID == 0).Icon);

                        ///////////////////////////////////////
                        // Photos/Folders

                        // Set Link
                        request.RequestHeaders.Clear();
                        BLOBSupportTest.ValidateInterceptorOverride = () =>
                        {
                            InterceptorChecker.ValidateQueryInterceptor(3);
                            InterceptorChecker.ValidateChangeInterceptor(1);
                        };
                        payload = "<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id='/Folders(0)' />";
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, method, "/Items(500)/ParentFolder/$ref", null, null, accept, UnitTestsUtil.MimeApplicationXml, null, payload, 204));
                        Assert.AreEqual(photoCount, PhotoDataServiceContext._items.Count);
                        Assert.AreEqual(folderCount, PhotoDataServiceContext._folders.Count);
                        Assert.IsTrue(PhotoDataServiceContext._items.Single(i => i.ID == 500).ParentFolder.ID == 0);

                        // Delete Link
                        BLOBSupportTest.ValidateInterceptorOverride = () =>
                        {
                            InterceptorChecker.ValidateQueryInterceptor(2);
                            InterceptorChecker.ValidateChangeInterceptor(1);
                        };
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(500)/ParentFolder/$ref", null, null, null, null, null, null, 204));
                        Assert.AreEqual(photoCount, PhotoDataServiceContext._items.Count);
                        Assert.AreEqual(folderCount, PhotoDataServiceContext._folders.Count);
                        Assert.IsNull(PhotoDataServiceContext._items.Single(i => i.ID == 500).ParentFolder);

                        ///////////////////////////////////////
                        // Photos/Photos

                        // Insert Link
                        request.RequestHeaders.Clear();
                        BLOBSupportTest.ValidateInterceptorOverride = () =>
                        {
                            InterceptorChecker.ValidateQueryInterceptor(3);
                            InterceptorChecker.ValidateChangeInterceptor(1);
                        };
                        payload = "<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id='/Items(1)' />";
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, method, "/Items(500)/Icon/$ref", null, null, accept, UnitTestsUtil.MimeApplicationXml, null, payload, 204));
                        Assert.AreEqual(photoCount, PhotoDataServiceContext._items.Count);
                        Assert.AreEqual(folderCount, PhotoDataServiceContext._folders.Count);
                        Assert.IsTrue(PhotoDataServiceContext._items.Single(i => i.ID == 500).Icon.ID == 1);

                        // Delete Link
                        BLOBSupportTest.ValidateInterceptorOverride = () =>
                        {
                            InterceptorChecker.ValidateQueryInterceptor(2);
                            InterceptorChecker.ValidateChangeInterceptor(1);
                        };
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(500)/Icon/$ref", null, null, null, null, null, null, 204));
                        Assert.AreEqual(photoCount, PhotoDataServiceContext._items.Count);
                        Assert.AreEqual(folderCount, PhotoDataServiceContext._folders.Count);
                        Assert.IsNull(PhotoDataServiceContext._items.Single(i => i.ID == 500).Icon);
                    }
                });
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void BlobLargeStreamTest()
            {
                this.EnsureTestHasNoLeakedStreams();

                if (!TestUtil.RunningInMinilab)
                {
                    Trace.WriteLine("IMPORTANT: Skipping because machine is not identified as a minilab machine.");
                    return;
                }

                const long oneGB = 1024 * 1024 * 1024;
                Random rand = TestUtil.Random;

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Location", new WebServerLocation[] { WebServerLocation.InProcessStreamedWcf }));

                TestUtil.RunCombinatorialEngineFail(engine, table =>
                {
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                    // Setting the size to 2GB + 1KB. 1KB is to test that we are good when passing 2GB boundary. 
                    // 4 GB adds too much to test execution time.
                    using (LargeStream stream1 = new LargeStream((byte)rand.Next(byte.MinValue, byte.MaxValue), 1024 + 2 * oneGB))
                    using (LargeStream stream2 = new LargeStream((byte)rand.Next(byte.MinValue, byte.MaxValue), 1 * oneGB))
                    using (PhotoDataServiceContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForLocation((WebServerLocation)table["Location"]))
                    {
                        DataServiceStreamProvider.SetupLargeStreamStorage();

                        string accept = UnitTestsUtil.AtomFormat;

                        int id = rand.Next(10, int.MaxValue);
                        string description = string.Format("Photo {0} Description", id);
                        string name = string.Format("Photo {0}", id);
                        int rating = rand.Next();
                        string photoPayload = GetPhotoPayload(accept, request.BaseUri, id, description, name, rating, new byte[] { 1, 2, 3, 4 });

                        string slug = id.ToString();
                        string binaryContentType = DataServiceStreamProvider.GetContentType(new Photo() { ID = id });
                        string blobETag = null;
                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => blobETag;

                        // POST MR
                        request.RequestHeaders["CustomRequestHeader_ItemType"] = typeof(Photo).FullName;
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "POST", "/Items", null, null, accept, binaryContentType, slug, stream1, 201));
                        ValidatePhotoMLEFromResponse(request, accept, typeof(Photo), id, null, null, 0, null, request.ResponseETag, blobETag, false);
                        ValidateMediaResourceFromStorage(typeof(Photo), request, id, stream1);

                        // GET Media Resource
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(" + id + ")/$value", null, null, null, null, null, null, 200));
                        ValidateMediaResourceFromGet(typeof(Photo), request, id, stream1);

                        // PUT Media Resource
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(" + id + ")/$value", null, null, null, binaryContentType, null, stream2, 204));
                        Assert.AreEqual("", request.GetResponseStreamAsText());
                        ValidateMediaResourceFromStorage(typeof(Photo), request, id, stream2);

                        // GET Media Resource
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(" + id + ")/$value", null, null, null, null, null, null, 200));
                        ValidateMediaResourceFromGet(typeof(Photo), request, id, stream2);

                        // Get the etag value before we can delete.
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(" + id + ")", null, null, null, null, null, null, 200));

                        // DELETE Media Link Entry
                        Photo p = FindPhoto(id);
                        Assert.IsNotNull(p);
                        string key = DataServiceStreamProvider.GetStoragePath(p);
                        Assert.IsTrue(DataServiceStreamProvider.LargeStreamStorage.ContainsKey(key));
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(" + id + ")", request.ResponseETag, null, null, "text/plain", null, "0", 204));
                        Assert.IsNull(FindPhoto(id));
                        Assert.IsFalse(DataServiceStreamProvider.LargeStreamStorage.ContainsKey(key));
                    }
                });

                this.EnsureTestHasNoLeakedStreams();
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void BlobEtagTest()
            {
                this.EnsureTestHasNoLeakedStreams();

                byte[] buffer = new byte[] { 1, 2, 3, 4 };
                string contentTypeP1 = DataServiceStreamProvider.GetContentType(new Photo() { ID = 1 });
                string contentTypeP100 = DataServiceStreamProvider.GetContentType(new Photo() { ID = 100 });

                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                {
                    using (PhotoDataServiceContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        // Make sure we support both strong and week etags for streams
                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "";
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", null, null, null, null, null, null, 200));
                        Assert.IsTrue(!request.ResponseHeaders.Keys.Any(k => string.Compare(k, "etag", StringComparison.OrdinalIgnoreCase) == 0), "etag header should not be there.");

                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => ",";
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", null, null, null, null, null, null, 500));

                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "\"\"";
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", null, null, null, null, null, null, 200));
                        Assert.AreEqual("\"\"", request.ResponseETag);

                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "\",\"";
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", null, null, null, null, null, null, 200));
                        Assert.AreEqual("\",\"", request.ResponseETag);

                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "W/";
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", null, null, null, null, null, null, 500));

                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "W/,";
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", null, null, null, null, null, null, 500));

                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "W/\"\"";
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", null, null, null, null, null, null, 200));
                        Assert.AreEqual("W/\"\"", request.ResponseETag);

                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "W/\",\"";
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", null, null, null, null, null, null, 200));
                        Assert.AreEqual("W/\",\"", request.ResponseETag);
                    }

                    using (PhotoDataServiceContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        // Make sure we only support weak etags for MLEs
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(1)", ",", null, null, null, null, null, 400));
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(1)", "\"\"", null, null, null, null, null, 400));
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "PATCH", "/Items(1)", "\",\"", null, null, null, null, null, 400));
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(1)", "W/", null, null, null, null, null, 400));
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)", "W/,", null, null, null, null, null, 400));
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(1)", "W/\"\"", null, null, null, null, null, 412));
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(1)", "W/\",\"", null, null, null, null, null, 412));
                    }

                    using (PhotoDataServiceContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        // If GetStreamETag() returns null or empty etag, need to make sure we don't set it on the response header
                        // Note IIS blocks empty headers on 2k8, but not on 2k3
                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => null;

                        // PUT MR
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(1)/$value", null, null, null, contentTypeP1, null, buffer, 204));
                        Assert.IsTrue(!request.ResponseHeaders.Keys.Any(k => string.Compare(k, "etag", StringComparison.OrdinalIgnoreCase) == 0), "etag header should not be there.");

                        // GET MR
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", null, null, null, null, null, null, 200));
                        Assert.IsTrue(!request.ResponseHeaders.Keys.Any(k => string.Compare(k, "etag", StringComparison.OrdinalIgnoreCase) == 0), "etag header should not be there.");

                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => string.Empty;

                        // PUT MR
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(1)/$value", null, null, null, contentTypeP1, null, buffer, 204));
                        Assert.IsTrue(!request.ResponseHeaders.Keys.Any(k => string.Compare(k, "etag", StringComparison.OrdinalIgnoreCase) == 0), "etag header should not be there.");

                        // GET MR
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", null, null, null, null, null, null, 200));
                        Assert.IsTrue(!request.ResponseHeaders.Keys.Any(k => string.Compare(k, "etag", StringComparison.OrdinalIgnoreCase) == 0), "etag header should not be there.");
                    }

                    using (PhotoDataServiceContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        // Etag not allowed for POST
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "POST", "/Items", "W/\"someetag\"", null, UnitTestsUtil.AtomFormat, contentTypeP100, "100", buffer, 400));
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "POST", "/Items", null, "W/\"someetag\"", UnitTestsUtil.AtomFormat, contentTypeP100, "100", buffer, 400));
                    }

                    using (PhotoDataServiceContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        // GET MR
                        DataServiceStreamProvider.GetStreamETagOverride = null;
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", null, null, null, null, null, null, 200));
                        Assert.IsFalse(string.IsNullOrEmpty(request.GetResponseStreamAsText()));
                        Assert.IsTrue(string.IsNullOrEmpty(request.ResponseETag), "If blob has no etag, we shouldn't see the etag for the MLE.");

                        DataServiceStreamProvider.GetStreamETagOverride = null;
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", "W/\"someetag\"", null, null, null, null, null, 912));
                        Assert.IsTrue(string.IsNullOrEmpty(request.ResponseETag));

                        DataServiceStreamProvider.GetStreamETagOverride = null;
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", null, "W/\"someetag\"", null, null, null, null, 200));
                        Assert.IsFalse(string.IsNullOrEmpty(request.GetResponseStreamAsText()));
                        Assert.IsTrue(string.IsNullOrEmpty(request.ResponseETag));

                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "InvalidETagFormat";
                        Exception e = SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", null, null, null, null, null, null, 500);
                        Assert.IsNotNull(e);
                        Assert.AreEqual(typeof(InvalidOperationException), e.InnerException.GetType());
                        Assert.AreEqual("The method 'IDataServiceStreamProvider.GetStreamETag' returned an entity tag with invalid format.", e.InnerException.Message);
                        Assert.IsTrue(string.IsNullOrEmpty(request.ResponseETag));

                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "\"BlobETag123\"";
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", null, null, null, null, null, null, 200));
                        Assert.IsFalse(string.IsNullOrEmpty(request.GetResponseStreamAsText()));
                        Assert.AreEqual("\"BlobETag123\"", request.ResponseETag, "The Blob has an etag, we expect to see it in the response header.");

                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "\"BlobETag123\"";
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", "\"BlobETag123\"", null, null, null, null, null, 200));
                        Assert.IsFalse(string.IsNullOrEmpty(request.GetResponseStreamAsText()));
                        Assert.AreEqual("\"BlobETag123\"", request.ResponseETag, "The Blob has an etag, we expect to see it in the response header.");

                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "\"BlobETag123\"";
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", "W/\"BlobETag456\"", null, null, null, null, null, 912));
                        Assert.IsTrue(string.IsNullOrEmpty(request.ResponseETag));

                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "W/\"BlobETag456\"";
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", null, "\"BlobETag123\"", null, null, null, null, 200));
                        Assert.IsFalse(string.IsNullOrEmpty(request.GetResponseStreamAsText()));
                        Assert.AreEqual("W/\"BlobETag456\"", request.ResponseETag, "The Blob has an etag, we expect to see it in the response header.");

                        DataServiceStreamProvider.ThrowDataServiceException304 = true;
                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "W/\"BlobETag456\"";
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", null, "W/\"BlobETag456\"", null, null, null, null, 304));
                        Assert.IsTrue(string.IsNullOrEmpty(request.GetResponseStreamAsText()));
                        Assert.AreEqual("W/\"BlobETag456\"", request.ResponseETag, "The Blob has an etag, we expect to see it in the response header.");

                        DataServiceStreamProvider.ThrowDataServiceException304 = false;
                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "W/\"BlobETag456\"";
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", null, "W/\"BlobETag456\"", null, null, null, null, 304));
                        Assert.IsTrue(string.IsNullOrEmpty(request.GetResponseStreamAsText()));
                        Assert.AreEqual("W/\"BlobETag456\"", request.ResponseETag, "The Blob has an etag, we expect to see it in the response header.");

                        // GET MLE
                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "\"BlobETag789\"";
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)", null, null, null, null, null, null, 200));
                        Assert.IsFalse(string.IsNullOrEmpty(request.GetResponseStreamAsText()));
                        Assert.IsFalse(string.IsNullOrEmpty(request.ResponseETag));
                        Assert.AreNotEqual("\"BlobETag789\"", request.ResponseETag);

                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => null;
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)", request.ResponseETag, null, null, null, null, null, 200));
                        Assert.IsFalse(string.IsNullOrEmpty(request.GetResponseStreamAsText()));
                        Assert.IsFalse(string.IsNullOrEmpty(request.ResponseETag));

                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)", null, request.ResponseETag, null, null, null, null, 304));
                        Assert.IsTrue(string.IsNullOrEmpty(request.GetResponseStreamAsText()));

                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)", "W/\"someetag\"", null, null, null, null, null, 412));
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)", null, "W/\"someetag\"", null, null, null, null, 200));
                        Assert.IsFalse(string.IsNullOrEmpty(request.GetResponseStreamAsText()));
                    }

                    using (PhotoDataServiceContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        // MLE
                        string payload = GetPhotoPayload(UnitTestsUtil.AtomFormat, request.BaseUri, 1, "Photo Description", "Photo Name", 1234, new byte[] { 1, 2, 3, 4 });

                        // Should fail because etag value is not specified for If-Match, make sure by removing the stream etag the MLE etag is still working
                        DataServiceStreamProvider.GetStreamETagOverride = null;
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(1)", null, null, null, UnitTestsUtil.AtomFormat, null, payload, 400));
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "PATCH", "/Items(1)", null, null, null, UnitTestsUtil.AtomFormat, null, payload, 400));

                        // If none match not supported for PUT/PATCH
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(1)", null, "W/\"someetag\"", null, UnitTestsUtil.AtomFormat, null, payload, 400));
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "PATCH", "/Items(1)", null, "W/\"someetag\"", null, UnitTestsUtil.AtomFormat, null, payload, 400));

                        // fail if match, make sure the MLEs don't check against the stream etag
                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "W/\"someetag\"";
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(1)", "W/\"someetag\"", null, null, UnitTestsUtil.AtomFormat, null, payload, 412));
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "PATCH", "/Items(1)", "W/\"someetag\"", null, null, UnitTestsUtil.AtomFormat, null, payload, 412));

                        // if match
                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "W/\"someetag\"";
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)", null, null, null, null, null, null, 200));
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(1)", request.ResponseETag, null, null, UnitTestsUtil.AtomFormat, null, payload, 204));
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "PATCH", "/Items(1)", request.ResponseETag, null, null, UnitTestsUtil.AtomFormat, null, payload, 204));
                        Assert.AreNotEqual("W/\"someetag\"", request.ResponseETag);

                        // PUT MR
                        DataServiceStreamProvider.GetStreamETagOverride = null;
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(1)/$value", null, null, null, contentTypeP1, null, buffer, 204));
                        Assert.IsTrue(string.IsNullOrEmpty(request.ResponseETag));
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(1)/$value", "W/\"someetag\"", null, null, contentTypeP1, null, buffer, 900));
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(1)/$value", null, "W/\"someetag\"", null, contentTypeP1, null, buffer, 900));

                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => "\"BlobETag789\"";
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(1)/$value", null, null, null, contentTypeP1, null, buffer, 900));
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(1)/$value", "W/\"someetag\"", null, null, contentTypeP1, null, buffer, 912));
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(1)/$value", "\"BlobETag789\"", null, null, contentTypeP1, null, buffer, 204));
                    }

                    using (PhotoDataServiceContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(1)/$value", null, null, null, null, null, null, 405 /* DELETE MR -- not allowed */));
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(1)/$value", "W/\"someetag\"", null, null, null, null, null, 405 /* DELETE MR -- not allowed */));
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(1)/$value", null, "W/\"someetag\"", null, null, null, null, 405 /* DELETE MR -- not allowed */));

                        Exception e = SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(1)", null, null, null, null, null, null, 400);
                        Assert.IsNotNull(e);
                        Assert.AreEqual(typeof(DataServiceException), e.InnerException.GetType());
                        Assert.AreEqual(string.Format("Since entity type '{0}' has one or more etag properties, If-Match HTTP header must be specified for DELETE/PUT operations on this type.", typeof(Photo).FullName), e.InnerException.Message);

                        // If none match not supported for DELETE
                        e = SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(1)", null, "W/\"someetag\"", null, null, null, null, 400);
                        Assert.IsNotNull(e);
                        Assert.AreEqual(typeof(DataServiceException), e.InnerException.GetType());
                        Assert.AreEqual("If-None-Match HTTP header cannot be specified for DELETE operations.", e.InnerException.Message);

                        // fail if match
                        Assert.IsNotNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(1)", "W/\"someetag\"", null, null, null, null, null, 412));

                        // if match
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)", null, null, null, null, null, null, 200));
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(1)", request.ResponseETag, null, null, null, null, null, 204));
                    }
                }

                this.EnsureTestHasNoLeakedStreams();
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void BlobResolveTypeNegativeTest()
            {
                byte[] buffer = new byte[] { 1, 2, 3, 4 };

                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                {
                    using (PhotoDataServiceContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        DataServiceStreamProvider.ResolveTypeOverride = (entitySetName, operationContext) => null;

                        Exception e = SendRequest(typeof(PhotoDataService), request, "POST", "/Items", null, null, null, "image/jpeg", "1234", buffer, 500);
                        Assert.IsNotNull(e);
                        Assert.AreEqual(e.InnerException.GetType(), typeof(InvalidOperationException));
                        Assert.AreEqual("The method 'IDataServiceStreamProvider.ResolveType' must return a valid resource type name.", e.InnerException.Message);

                        DataServiceStreamProvider.ResolveTypeOverride = (entitySetName, operationContext) => string.Empty;

                        e = SendRequest(typeof(PhotoDataService), request, "POST", "/Items", null, null, null, "image/jpeg", "1234", buffer, 500);
                        Assert.IsNotNull(e);
                        Assert.AreEqual(e.InnerException.GetType(), typeof(InvalidOperationException));
                        Assert.AreEqual("The method 'IDataServiceStreamProvider.ResolveType' must return a valid resource type name.", e.InnerException.Message);

                        DataServiceStreamProvider.ResolveTypeOverride = (entitySetName, operationContext) => "NoneExistingTypeName";

                        e = SendRequest(typeof(PhotoDataService), request, "POST", "/Items", null, null, null, "image/jpeg", "1234", buffer, 500);
                        Assert.IsNotNull(e);
                        Assert.AreEqual(e.InnerException.GetType(), typeof(InvalidOperationException));
                        Assert.AreEqual("The method 'IDataServiceStreamProvider.ResolveType' must return a valid resource type name.", e.InnerException.Message);
                    }
                }
            }

            [TestCategory("Partition1"), TestMethod, Variation]
            [Ignore]
                public void BlobBatchingTest()
            {
                this.EnsureTestHasNoLeakedStreams();

                string[] entitySets = new string[]
                {
                    "Items",
                    "Customers",
                    "CustomersWithPrefer"
                };

                string expectedPhotoDescription = "Batch photo description";
                string expectedPhotoName = "Batch photo name";
                int expectedPhotoRating = 3;
                byte[] expectedPhotoThumbNail = new byte[] { 1, 2, 3, 4 };

                string expectedCompanyName = "Microsoft Corp.";
                string expectedAddress = "One Microsoft Way";
                string expectedCity = "Redmond";

                string batchRequestsDirectory = Path.Combine(TestUtil.ServerUnitTestSamples, @"tests\BatchRequests");

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("EntitySets", entitySets),
                    new Dimension("Locations", new[] { WebServerLocation.InProcess }));

                TestUtil.RunCombinatorialEngineFail(engine, table =>
                {
                    string specifiedEntitySet = (string)table["EntitySets"];
                    string entitySet = specifiedEntitySet.StartsWith("Customers") ? "Customers" : specifiedEntitySet;
                    Type entityType = entitySet == "Items" ? typeof(Photo) : typeof(NorthwindModel.Customers);

                    // MR etag
                    string blobETag = "\"BlobETag123\"";
                    DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => blobETag;

                    // ID and ContentType
                    object expectedID = 123456;
                    string blobContentType = DataServiceStreamProvider.GetContentType(new Photo() { ID = (int)expectedID });
                    if (specifiedEntitySet == "Customers")
                    {
                        expectedID = "C" + "4321";
                        blobContentType = DataServiceStreamProvider.GetContentType(NorthwindModel.Customers.CreateCustomers((string)expectedID, expectedCompanyName));
                    }
                    else
                        if (specifiedEntitySet == "CustomersWithPrefer")
                        {
                            expectedID = "C" + "4322";
                            blobContentType = DataServiceStreamProvider.GetContentType(NorthwindModel.Customers.CreateCustomers((string)expectedID, expectedCompanyName));
                        }

                    // Slug header
                    string slug = expectedID.ToString();

                    WebServerLocation serverLocation = (WebServerLocation)table["Locations"];

                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                    using (NorthwindDefaultStreamService.SetupNorthwindWithStreamAndETag(
                        new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("Customers", "true") },
                        null,
                        "BlobSupportTest_BlobBatchingTest"))
                    using (PhotoDataServiceContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForLocation(serverLocation))
                    {
                        // payload to update
                        string payload = string.Empty;
                        if (entityType == typeof(Photo))
                        {
                            payload = GetPhotoPayload(UnitTestsUtil.AtomFormat, request.BaseUri, (int)expectedID, expectedPhotoDescription, expectedPhotoName, expectedPhotoRating, expectedPhotoThumbNail);
                        }
                        else if (entityType == typeof(NorthwindModel.Customers))
                        {
                            payload = GetCustomerPayload(UnitTestsUtil.AtomFormat, request.BaseUri, (string)expectedID, expectedCompanyName, expectedAddress, expectedCity);
                        }

                        // Blob content
                        byte[] buffer;
                        if (serverLocation == WebServerLocation.InProcessStreamedWcf)
                        {
                            buffer = new byte[1024 * 1024];
                        }
                        else
                        {
                            buffer = new byte[20];
                        }

                        Batch_FillBuffer(buffer, (byte)'1');

                        ///////////////////////////////////////////////////
                        // Testing Post and update in the same changeset
                        MemoryStream stream = new MemoryStream();
                        StreamWriter writer = new StreamWriter(stream);

                        Batch_BeginBatchMultiPart(writer);
                        Batch_BeginChangeSet(writer, "2");
                        Batch_PostMR(entitySet, entityType, blobContentType, slug, writer, buffer);

                        Batch_BeginChangeSet(writer, "3");
                        // This is to verify that 1.0 and 2.0 client works still behave the same.
                        Batch_UpdateMLE("PATCH", payload, writer, "4.0");

                        Batch_BeginChangeSet(writer, "4");
                        Batch_UpdateMLE("PUT", payload, writer, requestContent: (specifiedEntitySet == "CustomersWithPrefer"));

                        Batch_BeginChangeSet(writer, "5");
                        Batch_UpdateMLE("PATCH", payload, writer, requestContent: (specifiedEntitySet == "CustomersWithPrefer"));

                        Batch_BeginChangeSet(writer, "6");
                        Batch_PutMR(blobContentType, buffer, writer);
                        Batch_EndChangeSet(writer);

                        // GET MR -- If-None-Match -- should return 304
                        DataServiceStreamProvider.ThrowDataServiceException304 = true;
                        Batch_Begin(writer);
                        Batch_GetMR(entitySet, expectedID, blobETag, writer);

                        // GET MR -- If-None-Match -- should return 304
                        DataServiceStreamProvider.ThrowDataServiceException304 = false;
                        Batch_Begin(writer);
                        Batch_GetMR(entitySet, expectedID, blobETag, writer);

                        Batch_Begin(writer);
                        Batch_GetMR(entitySet, expectedID, null, writer);

                        Batch_Begin(writer);
                        Batch_GetMLE(entitySet, expectedID, writer);
                        Batch_End(writer);

                        if (entitySet == "Items")
                        {
                            request.ServiceType = typeof(PhotoDataService);
                        }
                        else if (entitySet == "Customers")
                        {
                            request.ServiceType = typeof(NorthwindDefaultStreamService);
                        }

                        request.HttpMethod = "POST";
                        request.RequestUriString = "/$batch";
                        request.RequestContentType = "multipart/mixed;boundary=boundary1";
                        request.RequestStream = stream;

                        AstoriaTestLog.WriteLineIgnore("#####################################################");
                        AstoriaTestLog.WriteLineIgnore("+++++++++++++++++++ BATCH REQUEST +++++++++++++++++++");
                        if (buffer.Length < 100)
                        {
                            stream.Position = 0;
                            AstoriaTestLog.WriteLineIgnore((new StreamReader(stream)).ReadToEnd());
                        }
                        else
                        {
                            AstoriaTestLog.WriteLineIgnore("Payload is too large to log...");
                        }

                        AstoriaTestLog.WriteLineIgnore("------------------- BATCH REQUEST -------------------");
                        AstoriaTestLog.WriteLineIgnore("#####################################################");

                        stream.Position = 0;
                        request.SendRequest();

                        string baseUriWithSlash = request.BaseUri.EndsWith("/") ? request.BaseUri : request.BaseUri + "/";
                        string key = expectedID.GetType() == typeof(string) ? "'" + expectedID.ToString() + "'" : expectedID.ToString();
                        string expectedLocation = "Location: " + baseUriWithSlash + entitySet + "(" + key + ")";
                        string response = request.GetResponseStreamAsText();

                        AstoriaTestLog.WriteLineIgnore("#####################################################");
                        AstoriaTestLog.WriteLineIgnore("+++++++++++++++++++ BATCH RESPONSE ++++++++++++++++++");
                        if (buffer.Length < 100)
                        {
                            AstoriaTestLog.WriteLineIgnore(response);
                        }
                        else
                        {
                            AstoriaTestLog.WriteLineIgnore("Payload is too large to log...");
                        }

                        AstoriaTestLog.WriteLineIgnore("------------------- BATCH RESPONSE ------------------");
                        AstoriaTestLog.WriteLineIgnore("#####################################################");

                        TestUtil.AssertContains(response, expectedLocation);

                        if (entitySet == "Items")
                        {
                            ValidateMediaResourceFromStorage(typeof(Photo), null, expectedID, buffer);
                            ValidatePhotoMLEOnContext((int)expectedID, expectedPhotoDescription, expectedPhotoName, expectedPhotoRating, expectedPhotoThumbNail);
                        }
                        else
                        {
                            ValidateMediaResourceFromStorage(typeof(NorthwindModel.Customers), null, expectedID, buffer);
                        }

                        response = BatchTestUtil.PrepareResponseForFileCompare(new StringReader(response), request.BaseUri, "http://host/");

                        // Strip an extra \r\n from the response inserted by preparation.
                        Assert.AreEqual('\r', response[response.Length - 2], "response[response.Length - 2]");
                        Assert.AreEqual('\n', response[response.Length - 1], "response[response.Length - 1]");
                        response = response.Substring(0, response.Length - 2);

                        string responseBaselineFileName = Path.Combine(batchRequestsDirectory, "Blob" + specifiedEntitySet + (serverLocation == WebServerLocation.InProcessStreamedWcf ? "Streamed" : "") + "Response.txt");
                        BatchTestUtil.CompareBatchResponse(responseBaselineFileName, response);

                        Assert.AreEqual(1, BlobDataServicePipelineHandlers.ProcessingRequestInvokeCount);
                        Assert.AreEqual(1, BlobDataServicePipelineHandlers.ProcessingChangesetInvokeCount);
                        Assert.AreEqual(1, BlobDataServicePipelineHandlers.ProcessedChangesetInvokeCount);
                        Assert.AreEqual(1, BlobDataServicePipelineHandlers.ProcessedRequestInvokeCount);

                        InterceptorChecker.ValidateQueryInterceptor(5);
                        InterceptorChecker.ValidateChangeInterceptor(5);
                        DataServiceStreamProvider.ValidateInstantiatedInstances();
                    }
                });
            }

            private void Batch_GetMLE(string entitySet, object expectedID, StreamWriter writer)
            {
                writer.WriteLine("GET {0}({1}) HTTP/1.1", entitySet, expectedID.GetType() == typeof(string) ? "'" + expectedID + "'" : expectedID);
                writer.WriteLine();
                writer.Flush();
            }

            private void Batch_GetMR(string entitySet, object expectedID, string blobETag, StreamWriter writer)
            {
                writer.WriteLine("GET {0}({1})/$value HTTP/1.1", entitySet, expectedID.GetType() == typeof(string) ? "'" + expectedID + "'" : expectedID);
                Batch_SetCustomRequestHeaders(writer);
                if (!string.IsNullOrEmpty(blobETag))
                {
                    writer.WriteLine("If-None-Match: {0}", blobETag);
                }

                writer.WriteLine();
                writer.Flush();
            }

            private void Batch_PutMR(string contentType, byte[] buffer, StreamWriter writer)
            {
                writer.WriteLine("PUT $2/$value HTTP/1.1");
                writer.WriteLine("Content-Type: {0}", contentType);
                Batch_SetCustomRequestHeaders(writer);
                writer.WriteLine();
                writer.Flush();

                // binary stream to update
                Batch_FillBuffer(buffer, (byte)'2');
                writer.BaseStream.Write(buffer, 0, buffer.Length);
                writer.WriteLine();
                writer.Flush();
            }

            private void Batch_FillBuffer(byte[] buffer, byte content)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = content;
                }
            }

            private void Batch_UpdateMLE(string method, string payload, StreamWriter writer, string dataServiceMaxVersion = null, bool requestContent = false)
            {
                writer.WriteLine("{0} $2 HTTP/1.1", method);
                writer.WriteLine("Content-Type: {0}", UnitTestsUtil.AtomFormat);
                if (requestContent)
                {
                    writer.WriteLine("Prefer: return=representation");
                }
                if (dataServiceMaxVersion != null)
                {
                    writer.WriteLine("OData-MaxVersion: {0}", dataServiceMaxVersion);
                }
                Batch_SetCustomRequestHeaders(writer);
                writer.WriteLine();
                writer.WriteLine(payload);
                writer.Flush();
            }

            private void Batch_PostMR(string entitySet, Type entityType, string contentType, string slug, StreamWriter writer, byte[] buffer)
            {
                // headers
                writer.WriteLine("POST {0} HTTP/1.1", entitySet);
                writer.WriteLine("Content-Type: {0}", contentType);
                writer.WriteLine("CustomRequestHeader_ItemType: {0}", entityType.FullName);
                writer.WriteLine("Slug: {0}", slug);
                Batch_SetCustomRequestHeaders(writer);
                writer.WriteLine();
                writer.Flush();

                // binary content
                writer.BaseStream.Write(buffer, 0, buffer.Length);
                writer.WriteLine();
                writer.Flush();
            }

            private void Batch_BeginBatchMultiPart(StreamWriter writer)
            {
                writer.WriteLine("--boundary1");
                writer.WriteLine("Content-Type: multipart/mixed; boundary=cs");
                writer.WriteLine();
                writer.Flush();
            }

            private void Batch_Begin(StreamWriter writer)
            {
                writer.WriteLine("--boundary1");
                writer.WriteLine("Content-Type: application/http");
                writer.WriteLine("Content-Transfer-Encoding: binary");
                writer.WriteLine();
                writer.Flush();
            }

            private void Batch_End(StreamWriter writer)
            {
                writer.Write("--boundary1--");
                writer.Flush();
            }

            private void Batch_BeginChangeSet(StreamWriter writer, string contentId)
            {
                writer.WriteLine("--cs");
                writer.WriteLine("Content-Type: application/http");
                writer.WriteLine("Content-Transfer-Encoding: binary");
                writer.WriteLine("Content-ID: {0}", contentId);
                writer.WriteLine();
                writer.Flush();
            }

            private void Batch_EndChangeSet(StreamWriter writer)
            {
                writer.WriteLine("--cs--");
                writer.Flush();
            }

            [TestCategory("Partition1"), TestMethod, Variation]
            public void BlobAcceptContentTypeHeaderTest()
            {
                this.EnsureTestHasNoLeakedStreams();

                byte[] buffer = new byte[] { 1, 2, 3, 4 };
                string contentTypeP123 = DataServiceStreamProvider.GetContentType(new Photo() { ID = 123 });
                string contentTypeP1 = DataServiceStreamProvider.GetContentType(new Photo() { ID = 1 });

                string[] accepts = new string[]
                {
                    UnitTestsUtil.AtomFormat,
                    contentTypeP123,
                    contentTypeP1,
                    "image/*",
                    "*/*",
                    ""
                };

                string[] contentTypes = new string[]
                {
                    UnitTestsUtil.AtomFormat,
                    contentTypeP123,
                    contentTypeP1,
                    "some/type"
                };

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Accepts", accepts),
                    new Dimension("ContentTypes", contentTypes),
                    new Dimension("StreamBufferSize", new int[] { 0, -1, 4000 }));

                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                {
                    string blobETag = "\"BlobETag123\"";
                    DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => blobETag;

                    TestUtil.RunCombinatorialEngineFail(engine, table =>
                    {
                        string accept = (string)table["Accepts"];
                        string contentType = (string)table["ContentTypes"];
                        DataServiceStreamProvider.DefaultStreamBufferSize = (int)table["StreamBufferSize"];

                        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                        {
                            Exception e;
                            bool fail;

                            // POST Media Resource
                            using (PhotoDataServiceContext.CreateChangeScope())
                            {
                                bool fail415 = accept == contentTypeP123 || accept == contentTypeP1 || accept == "image/*";
                                bool fail500 = contentType != contentTypeP123;
                                request.RequestHeaders["CustomRequestHeader_ItemType"] = typeof(Photo).FullName;
                                e = SendRequest(typeof(PhotoDataService), request, "POST", "/Items", null, null, accept, contentType, "123", buffer, fail415 ? 415 : fail500 ? 500 : 201);
                                TestUtil.AssertExceptionExpected(e, fail415, fail500);
                            }

                            // GET Media Resource
                            using (PhotoDataServiceContext.CreateChangeScope())
                            {
                                fail = (accept == UnitTestsUtil.AtomFormat || accept == contentTypeP123);
                                e = SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", blobETag, null, accept, contentType, null, null, fail ? 415 : 200);
                                TestUtil.AssertExceptionExpected(e, fail);
                            }

                            // GET Media Link Entry
                            using (PhotoDataServiceContext.CreateChangeScope())
                            {
                                fail = (accept == contentTypeP123 || accept == contentTypeP1 || accept == "image/*");
                                e = SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)", null, null, accept, contentType, null, null, fail ? 415 : 200);
                                TestUtil.AssertExceptionExpected(e, fail);
                            }

                            // PUT Media Resource
                            using (PhotoDataServiceContext.CreateChangeScope())
                            {
                                fail = contentType != contentTypeP1;
                                e = SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(1)/$value", blobETag, null, accept, contentType, null, buffer, fail ? 500 : 204);
                                TestUtil.AssertExceptionExpected(e, fail);
                            }

                            // MERGE Media Resource -- not supported
                            //using (PhotoDataServiceContext.CreateChangeScope())
                            //{
                            //    fail = contentType != contentTypeP1;
                            //    e = SendRequest(typeof(PhotoDataService), request, "MERGE", "/Items(1)/$value", blobETag, null, accept, contentType, null, buffer, fail ? 500 : 204);
                            //    TestUtil.AssertExceptionExpected(e, fail);
                            //}

                            string payload = null;
                            if (contentType == UnitTestsUtil.AtomFormat)
                            {
                                payload = GetPhotoPayload(UnitTestsUtil.AtomFormat, request.BaseUri, 1, null, null, 0, null);
                            }

                            // MERGE Media Link Entry
                            using (PhotoDataServiceContext.CreateChangeScope())
                            {
                                Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)", null, null, null, null, null, null, 200));
                                fail = contentType != UnitTestsUtil.AtomFormat;
                                e = SendRequest(typeof(PhotoDataService), request, "PATCH", "/Items(1)", request.ResponseETag, null, accept, contentType, null, payload, fail ? 415 : 204);
                                TestUtil.AssertExceptionExpected(e, fail);
                            }

                            // PUT Media Link Entry
                            using (PhotoDataServiceContext.CreateChangeScope())
                            {
                                Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)", null, null, null, null, null, null, 200));
                                fail = contentType != UnitTestsUtil.AtomFormat;
                                e = SendRequest(typeof(PhotoDataService), request, "PUT", "/Items(1)", request.ResponseETag, null, accept, contentType, null, payload, fail ? 415 : 204);
                                TestUtil.AssertExceptionExpected(e, fail);
                            }

                            // DELETE Media Link Entry
                            using (PhotoDataServiceContext.CreateChangeScope())
                            {
                                Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)", null, null, null, null, null, null, 200));
                                Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "DELETE", "/Items(1)", request.ResponseETag, null, accept, contentType, null, null, 204));
                            }
                        }
                    });
                }
            }

            [TestCategory("Partition2"), TestMethod, Variation("Verify that no query options can be applied to Media Resources + invalid queries for Blobs and Projections")]
            public void BlobStreamQueryOptionTests()
            {
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                using (PhotoDataServiceContext.CreateChangeScope())
                {
                    request.RequestMaxVersion = "4.0;";
                    request.RequestVersion = "4.0;";

                    //negative test cases
                    foreach (string queryString in new string[] { 
                            "/Items(1)/$value?$select=Name",
                            "/Items(1)/$value?$expand=ParentFolder",
                            "/Items(1)/$value?$filter=ID eq 1",
                            "/Items(1)/$value?$orderby=Name",
                            "/Items(1)/$value?$count=true",
                            "/Items(1)/$value?$skip=1",
                            "/Items(1)/$value?$top=1",

                            "/Items(1)?$select=$value", 
                            "/Items(1)?$select=$count", 
                            "/Items(1)?$select=Icon/$value", 
                            "/Items(1)?$select=Icon/$count", 
                            "/Items(1)?$select=*/$value", 

                            "/Items(1)?$expand=$value", 
                            "/Items(1)?$expand=$count", 
                            //"/Items(1)?$expand=Name",
                            "/Items(1)?$expand=Icon/$value", 
                            "/Items(1)?$expand=Icon/$count", 
                            //"/Items(1)?$expand=Icon/Name", 
                    })
                    {
                        Exception e = SendRequest(typeof(PhotoDataService), request, "GET", queryString, null, null, null, null, null, null, 400);
                        Assert.IsInstanceOfType(e.InnerException, typeof(DataServiceException));
                    }

                    foreach (string requestString in new string[]
                    {
                        "/Items?$select=Name",
                        "/Items?$expand=Icon",
                        "/Items?$filter=ID eq 0",
                        "/Items?$orderby=Name",
                        "/Items?$skiptoken=ID",
                        "/Items?$top=1",
                        "/Items?$count=true",
                        "/Items/$count",

                        "/Items/$value?$select=Name",
                        "/Items/$value?$expand=Icon",
                        "/Items/$value?$filter=ID eq 0",
                        "/Items/$value?$orderby=Name",
                        "/Items/$value?$skiptoken=ID",
                        "/Items/$value?$top=1",
                        "/Items/$value?$count=true",
                        "/Items/$value/$count",
                    })
                    {
                        Exception ex = SendRequest(typeof(PhotoDataService), request, "POST", requestString, null, null, null, UnitTestsUtil.AtomFormat, null, null, 400);
                        Assert.IsInstanceOfType(ex.InnerException, typeof(DataServiceException));
                    }

                    foreach (string verb in new string[] { "PUT", "PATCH" })
                    {
                        foreach (string requestString in new string[]
                        {
                            "/Items(1)?$select=Name",
                            "/Items(1)?$expand=Icon",
                            "/Items(1)?$filter=ID eq 0",
                            "/Items(1)?$orderby=Name",
                            "/Items(1)?$skiptoken=ID",
                            "/Items(1)?$top=1",
                            "/Items(1)?$count=true",

                            "/Items(1)/$value?$select=Name",
                            "/Items(1)/$value?$expand=Icon",
                            "/Items(1)/$value?$filter=ID eq 0",
                            "/Items(1)/$value?$orderby=Name",
                            "/Items(1)/$value?$skiptoken=ID",
                            "/Items(1)/$value?$top=1",
                            "/Items(1)/$value?$count=true",
                        })
                        {
                            Exception ex = SendRequest(typeof(PhotoDataService), request, verb, requestString, null, null, null, UnitTestsUtil.AtomFormat, null, "some content", 400);
                            Assert.IsInstanceOfType(ex.InnerException, typeof(DataServiceException));
                        }
                    }

                    foreach (string requestString in new string[]
                    {
                        "/Items(1)?$select=Name",
                        "/Items(1)?$expand=Icon",
                        //"/Items(1)?$filter=ID eq 0",
                        "/Items(1)?$orderby=Name",
                        "/Items(1)?$skiptoken=ID",
                        "/Items(1)?$top=1",
                        "/Items(1)?$count=true",

                        "/Items(1)/$value?$select=Name",
                        "/Items(1)/$value?$expand=Icon",
                        "/Items(1)/$value?$filter=ID eq 0",
                        "/Items(1)/$value?$orderby=Name",
                        "/Items(1)/$value?$skiptoken=ID",
                        "/Items(1)/$value?$top=1",
                        "/Items(1)/$value?$count=true",
                    })
                    {
                        Exception ex = SendRequest(typeof(PhotoDataService), request, "DELETE", requestString, null, null, null, UnitTestsUtil.AtomFormat, null, "some content", 400);
                        Assert.IsInstanceOfType(ex.InnerException, typeof(DataServiceException));
                    }
                }
            }

            [TestCategory("Partition2"), TestMethod, Variation("Negative test cases for  'IDataServiceStreamProvider.GetStreamContentType'")]
            public void GetStreamContentTypeTest()
            {
                this.EnsureTestHasNoLeakedStreams();
                var testCases = new[] {
                    new {
                        GetStreamContentTypeOverride = (Func<object, DataServiceOperationContext, string>)
                            delegate(object entity, DataServiceOperationContext operationContext) { 
                                return null;
                            },
                        ExpectedHTTPError = 500, 
                        ExpectedExceptionType = typeof(InvalidOperationException),
                        ExceptionMsgRegex = "^The method 'IDataServiceStreamProvider.GetStreamContentType' must not return a null or empty string.$"
                    },

                    new {
                        GetStreamContentTypeOverride = (Func<object, DataServiceOperationContext, string>)
                            delegate(object entity, DataServiceOperationContext operationContext) { 
                                return string.Empty;
                            },
                        ExpectedHTTPError = 500, 
                        ExpectedExceptionType = typeof(InvalidOperationException),
                        ExceptionMsgRegex = "^The method 'IDataServiceStreamProvider.GetStreamContentType' must not return a null or empty string.$"
                    },
                    new {
                        GetStreamContentTypeOverride = (Func<object, DataServiceOperationContext, string>)
                            delegate(object entity, DataServiceOperationContext operationContext) { 
                                return "a";
                            },
                        ExpectedHTTPError = 400, 
                        ExpectedExceptionType = typeof(DataServiceException),
                        ExceptionMsgRegex = "^Media type is unspecified.$"
                    },
                    new {
                        GetStreamContentTypeOverride = (Func<object, DataServiceOperationContext, string>)
                            delegate(object entity, DataServiceOperationContext operationContext) { 
                                return "image/";
                            },
                        ExpectedHTTPError = 400, 
                        ExpectedExceptionType = typeof(DataServiceException),
                        ExceptionMsgRegex = "^Media type requires a subtype definition.$"
                    },
                    new {
                        GetStreamContentTypeOverride = (Func<object, DataServiceOperationContext, string>)
                            delegate(object entity, DataServiceOperationContext operationContext) { 
                                return "a/a";
                            },
                        ExpectedHTTPError = 415, 
                        ExpectedExceptionType = typeof(DataServiceException),
                        ExceptionMsgRegex = "^Unsupported media type requested.$"
                    }
                };

                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                {
                    TestUtil.RunCombinations(testCases, (testCase) =>
                    {
                        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                        {
                            using (PhotoDataServiceContext.CreateChangeScope())
                            {
                                DataServiceStreamProvider.GetStreamContentTypeOverride = testCase.GetStreamContentTypeOverride;
                                Exception e = SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)/$value", null, null, "image/png", UnitTestsUtil.AtomFormat, null, null, testCase.ExpectedHTTPError);
                                TestUtil.AssertExceptionExpected(e, true);
                                Assert.IsNotNull(e.InnerException, "e.InnerException");
                                Assert.AreEqual(testCase.ExpectedExceptionType, e.InnerException.GetType());
                                Assert.IsTrue(Regex.IsMatch(e.InnerException.Message, testCase.ExceptionMsgRegex));
                            }
                        }
                    });
                }
            }

            [TestCategory("Partition2"), TestMethod, Variation("Test cases for  'IDataServiceStreamProvider.GetReadStreamUri'")]
            public void GetReadStreamUriTest()
            {
                var testCases = new[] {
                    new {
                        GetReadStreamUriOverride = (Func<object, DataServiceOperationContext, Uri>)
                            delegate(object entity, DataServiceOperationContext operationContext) { 
                                return new Uri("some/relative/uri", UriKind.Relative);
                            },
                        ExpectedStatusCode = 500, 
                        ExpectedExceptionType = typeof(InvalidOperationException),
                        ExceptionMsgRegex = "^The method 'IDataServiceStreamProvider.GetReadStreamUri' must return an absolute Uri or null.$"
                    },

                    new {
                        GetReadStreamUriOverride = (Func<object, DataServiceOperationContext, Uri>)
                            delegate(object entity, DataServiceOperationContext operationContext) { 
                                return new Uri("http://some/absolute/uri", UriKind.Absolute);
                            },
                        ExpectedStatusCode = 200, 
                        ExpectedExceptionType = default(Type),
                        ExceptionMsgRegex = default(string)
                    },
                    new {
                        GetReadStreamUriOverride = (Func<object, DataServiceOperationContext, Uri>)
                            delegate(object entity, DataServiceOperationContext operationContext) { 
                                return null;
                            },
                        ExpectedStatusCode = 200,
                        ExpectedExceptionType = default(Type),
                        ExceptionMsgRegex = default(string)
                    },
                };

                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                {
                    foreach (string format in UnitTestsUtil.ResponseFormats)
                    {
                        foreach (var testCase in testCases)
                        {
                            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                            {
                                using (PhotoDataServiceContext.CreateChangeScope())
                                {
                                    DataServiceStreamProvider.GetReadStreamUriOverride = testCase.GetReadStreamUriOverride;
                                    Exception e = SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)", null, null, format, null, null, null, testCase.ExpectedStatusCode);
                                    TestUtil.AssertExceptionExpected(e, testCase.ExpectedExceptionType != null);
                                    if (e != null)
                                    {
                                        Assert.IsNotNull(e.InnerException, "e.InnerException");
                                        Assert.AreEqual(testCase.ExpectedExceptionType, e.InnerException.GetType());
                                        Assert.IsTrue(Regex.IsMatch(e.InnerException.Message, testCase.ExceptionMsgRegex));
                                    }
                                    else
                                    {
                                        string expectedReadStreamUri = testCase.GetReadStreamUriOverride(null, null) == null ? null : testCase.GetReadStreamUriOverride(null, null).OriginalString;
                                        if (string.IsNullOrEmpty(expectedReadStreamUri))
                                        {
                                            expectedReadStreamUri = request.RequestUriString;
                                            if (expectedReadStreamUri.StartsWith("/"))
                                            {
                                                expectedReadStreamUri = expectedReadStreamUri.Substring(1);
                                            }

                                            if (!expectedReadStreamUri.EndsWith("/"))
                                            {
                                                expectedReadStreamUri += "/";
                                            }

                                            expectedReadStreamUri += "AstoriaUnitTests.Stubs.Photo/$value";
                                        }

                                        string xpath;

                                        xpath = "/atom:entry/atom:content[@src='" + expectedReadStreamUri + "']";

                                        UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), format, xpath);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            [TestCategory("Partition2"), TestMethod, Variation("Verify that we can call GetQueryStringValue(headerName) correctly in an IDataServiceStreamProvider method.")]
            public void BlobQueryStringHeaderTest()
            {
                this.EnsureTestHasNoLeakedStreams();

                byte[] buffer = new byte[] { 1, 2, 3, 4 };
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        // POST Media Resource
                        using (PhotoDataServiceContext.CreateChangeScope())
                        {
                            Exception e = SendRequest(typeof(PhotoDataService), request, "POST", "/Items?Query-String-Header-Force-Error=yes", null, null, UnitTestsUtil.AtomFormat, UnitTestsUtil.AtomFormat, "slug", buffer, 418);
                            Assert.IsTrue(e.InnerException.Message.Contains("User code threw a Query-String-Header-Force-Error exception."));
                        }
                    }
                }
            }

            [TestCategory("Partition2"), TestMethod, Variation("We do not do JSONP for non json $value response")]
            public void BlobCallbackQueryOptionTest()
            {
                this.EnsureTestHasNoLeakedStreams();
                using (NorthwindDefaultStreamService.SetupNorthwindWithStreamAndETag(
                    new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("Customers", "true") },
                    null,
                    "BlobSupportTest_BlobProjectionTests_EFProvider"))
                {
                    string blobETag = "\"BlobETag123\"";
                    DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) => blobETag;

                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                    using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcessWcf))
                    {
                        string format = UnitTestsUtil.JsonLightMimeType;
                        string slug = "C" + TestUtil.Random.Next(1000, 9999).ToString();
                        string requestBodyString = "some string";
                        byte[] buffer = Encoding.ASCII.GetBytes(requestBodyString);
                        string expectedID = slug;

                        NorthwindModel.Customers customer = NorthwindModel.Customers.CreateCustomers(expectedID, "");
                        string contentType = DataServiceStreamProvider.GetContentType(customer);

                        // POST Media Resource
                        Assert.IsNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "POST", "/Customers", null, null, format, contentType, slug, buffer, 201));
                        string payload = request.GetResponseStreamAsText();

                        // PATCH the same response payload back to the MLE.
                        // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
                        // This Assert passes but hits a debug assert and it needs to be fixed.
                        // Assert.IsNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "MERGE", "/Customers('" + expectedID + "')", null, null, format, format, slug, payload, 200, true));

                        Assert.IsNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "GET", "/Customers('" + expectedID + "')/$value?$callback=foo", blobETag, null, null, null, slug, null, 200));
                        Assert.AreEqual(contentType, request.ResponseHeaders["Content-Type"]);

                        // We do not wrap the blob response with the callback value.
                        Assert.AreEqual(requestBodyString, request.GetResponseStreamAsText());
                    }
                }

                this.EnsureTestHasNoLeakedStreams();
            }

            #region Cross Feature - Blobs, service ops, concurrency, RowCount

            [TestCategory("Partition2"), TestMethod, Variation("Cross Feature - Blobs, service ops, concurrency, RowCount, query options")]
            public void Blob_ServiceOp_Concurrency_RowCount_QueryOptions()
            {
                var TestCases = new[]
                {
                    //
                    // No query options
                    //
                    new
                    {
                        RequestUri = "/Customers",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },
                    new
                    {
                        RequestUri = "/Customers(0)",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },                    
                    new
                    {
                        RequestUri = "/GetAllCustomersQueryable",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },
                    new
                    {                    
                        RequestUri = "/GetCustomerByIdQueryable?id=0",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },
                    new
                    {
                        RequestUri = "/GetAllCustomersEnumerable",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },
                    new
                    {
                        RequestUri = "/GetCustomerByIdDirectValue?id=0",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },

                    //
                    // Row Count
                    //
                    new
                    {
                        RequestUri = "/Customers/$count?$select=Name&$top=1000&$skip=0&$orderby=NameAsHtml&$filter=Address/City eq 'Redmond'&$expand=Orders",
                        Accept = new string[] { UnitTestsUtil.MimeTextPlain },
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },
                    new
                    {
                        RequestUri = "/Customers(0)/$count?$select=Name&$top=1000&$skip=0&$orderby=NameAsHtml&$filter=Address/City eq 'Redmond'&$expand=Orders",
                        Accept = new string[] { UnitTestsUtil.MimeTextPlain },
                        ErrorMessage = "The request URI is not valid. $count cannot be applied to the segment 'Customers' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.",
                        StatusCode = 404,
                    },
                    new
                    {
                        RequestUri = "/GetAllCustomersQueryable/$count?$select=Name&$top=1000&$skip=0&$orderby=NameAsHtml&$filter=Address/City eq 'Redmond'&$expand=Orders",
                        Accept = new string[] { UnitTestsUtil.MimeTextPlain },
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },
                    new
                    {
                        RequestUri = "/GetCustomerByIdQueryable/$count?id=0&$select=Name&$top=1000&$skip=0&$orderby=NameAsHtml&$filter=Address/City eq 'Redmond'&$expand=Orders",
                        Accept = new string[] { UnitTestsUtil.MimeTextPlain },
                        ErrorMessage = "The request URI is not valid. $count cannot be applied to the segment 'GetCustomerByIdQueryable' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.",
                        StatusCode = 404,
                    },
                    new
                    {
                        RequestUri = "/GetAllCustomersEnumerable/$count",
                        Accept = new string[] { UnitTestsUtil.MimeTextPlain },
                        ErrorMessage = ODataLibResourceUtil.GetString("RequestUriProcessor_MustBeLeafSegment", "GetAllCustomersEnumerable"),
                        StatusCode = 400,
                    },
                    new
                    {
                        RequestUri = "/GetAllCustomersEnumerable/$count?$select=Name&$top=1000&$skip=0&$orderby=NameAsHtml&$filter=Address/City eq 'Redmond'&$expand=Orders",
                        Accept = new string[] { UnitTestsUtil.MimeTextPlain },
                        ErrorMessage = ODataLibResourceUtil.GetString("RequestUriProcessor_MustBeLeafSegment", "GetAllCustomersEnumerable"),
                        StatusCode = 400,
                    },
                    new
                    {
                        RequestUri = "/GetCustomerByIdDirectValue/$count?id=0",
                        Accept = new string[] { UnitTestsUtil.MimeTextPlain },
                        ErrorMessage = "The request URI is not valid. $count cannot be applied to the segment 'GetCustomerByIdDirectValue' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.",
                        StatusCode = 404,
                    },
                    new
                    {
                        RequestUri = "/Customers?$count=true&$select=Name&$top=1000&$skip=0&$orderby=NameAsHtml&$filter=Address/City eq 'Redmond'&$expand=Orders",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },
                    new
                    {
                        RequestUri = "/Customers(0)?$count=true&$select=Name&$top=1000&$skip=0&$orderby=NameAsHtml&$filter=Address/City eq 'Redmond'&$expand=Orders",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = DataServicesResourceUtil.GetString("RequestQueryProcessor_QuerySetOptionsNotApplicable"),
                        StatusCode = 400,
                    },
                    new
                    {
                        RequestUri = "/GetAllCustomersQueryable?$count=true&$select=Name&$top=1000&$skip=0&$orderby=NameAsHtml&$filter=Address/City eq 'Redmond'&$expand=Orders",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },
                    new
                    {
                        RequestUri = "/GetCustomerByIdQueryable?id=0&$count=true&$select=Name&$top=1000&$skip=0&$orderby=NameAsHtml&$filter=Address/City eq 'Redmond'&$expand=Orders",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = DataServicesResourceUtil.GetString("RequestQueryProcessor_QuerySetOptionsNotApplicable"),
                        StatusCode = 400,
                    },
                    new
                    {
                        RequestUri = "/GetAllCustomersEnumerable?$count=true&$select=Name&$top=1000&$skip=0&$orderby=NameAsHtml&$filter=Address/City eq 'Redmond'&$expand=Orders",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = DataServicesResourceUtil.GetString("RequestQueryProcessor_QueryNoOptionsApplicable"),
                        StatusCode = 400,
                    },
                    new
                    {
                        RequestUri = "/GetCustomerByIdDirectValue?id=0&$count=true&$select=Name&$top=1000&$skip=0&$orderby=NameAsHtml&$filter=Address/City eq 'Redmond'&$expand=Orders",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = DataServicesResourceUtil.GetString("RequestQueryProcessor_QueryNoOptionsApplicable"),
                        StatusCode = 400,
                    },

                    //
                    // Media Resources
                    //
                    new
                    {
                        RequestUri = "/Customers/$value",
                        Accept = new string[] { "CustomType/CustomSubType" },
                        ErrorMessage = ODataLibResourceUtil.GetString("PathParser_CannotUseValueOnCollection"),
                        StatusCode = 400,
                    },
                    new
                    {
                        RequestUri = "/Customers(0)/$value",
                        Accept = new string[] { "CustomType/CustomSubType" },
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },
                    new
                    {
                        RequestUri = "/GetAllCustomersQueryable/$value",
                        Accept = new string[] { "CustomType/CustomSubType" },
                        ErrorMessage = ODataLibResourceUtil.GetString("PathParser_CannotUseValueOnCollection"),
                        StatusCode = 400,
                    },
                    new
                    {
                        RequestUri = "/GetCustomerByIdQueryable/$value?id=0",
                        Accept = new string[] { "CustomType/CustomSubType" },
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },
                    new
                    {
                        RequestUri = "/GetAllCustomersEnumerable/$value",
                        Accept = new string[] { "CustomType/CustomSubType" },
                        ErrorMessage = ODataLibResourceUtil.GetString("RequestUriProcessor_MustBeLeafSegment", "GetAllCustomersEnumerable"),
                        StatusCode = 400,
                    },
                    new
                    {
                        RequestUri = "/GetCustomerByIdDirectValue/$value?id=0",
                        Accept = new string[] { "CustomType/CustomSubType" },
                        ErrorMessage = DataServicesResourceUtil.GetString("RequestUriProcessor_IEnumerableServiceOperationsCannotBeFurtherComposed", "GetCustomerByIdDirectValue"),
                        StatusCode = 404,
                    },

                    //
                    // Projection
                    //
                    new
                    {
                        RequestUri = "/Customers?$select=Name",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },
                    new
                    {
                        RequestUri = "/Customers(0)?$select=Name",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },
                    new
                    {
                        RequestUri = "/GetAllCustomersQueryable?$select=Name",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },
                    new
                    {
                        RequestUri = "/GetCustomerByIdQueryable?id=0&$select=Name",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },
                    new
                    {
                        RequestUri = "/GetAllCustomersEnumerable?$select=Name",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = DataServicesResourceUtil.GetString("RequestQueryProcessor_QueryNoOptionsApplicable"),
                        StatusCode = 400,
                    },
                    new
                    {
                        RequestUri = "/GetCustomerByIdDirectValue?id=0&$select=Name",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = DataServicesResourceUtil.GetString("RequestQueryProcessor_QueryNoOptionsApplicable"),
                        StatusCode = 400,
                    },

                    //
                    // Mix of query options
                    //
                    new
                    {
                        RequestUri = "/Customers?$select=Name&$top=1&$skip=1&$orderby=NameAsHtml&$filter=Address/City eq 'Redmond'",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },
                    new
                    {
                        RequestUri = "/Customers(0)?$select=Name&$top=1&$skip=1&$orderby=NameAsHtml&$filter=Address/City eq 'Redmond'",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = DataServicesResourceUtil.GetString("RequestQueryProcessor_QuerySetOptionsNotApplicable"),
                        StatusCode = 400,
                    },
                    new
                    {
                        RequestUri = "/Customers(0)/Address?$filter=City eq 'Redmond'",
                        Accept = new string[] { UnitTestsUtil.MimeApplicationXml },
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },
                    new
                    {
                        RequestUri = "/GetAllCustomersQueryable?$select=Name&$top=1&$skip=1&$orderby=NameAsHtml&$filter=Address/City eq 'Redmond'",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },
                    new
                    {
                        RequestUri = "/GetCustomerByIdQueryable?id=0&$select=Name&$top=1&$skip=1&$orderby=NameAsHtml&$filter=Address/City eq 'Redmond'",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = DataServicesResourceUtil.GetString("RequestQueryProcessor_QuerySetOptionsNotApplicable"),
                        StatusCode = 400,
                    },
                    new
                    {
                        RequestUri = "/GetCustomerByIdQueryable/Address?id=0&$filter=City eq 'Redmond'",
                        Accept = new string[] { UnitTestsUtil.MimeApplicationXml },
                        ErrorMessage = default(string),
                        StatusCode = 200,
                    },
                    new
                    {
                        RequestUri = "/GetAllCustomersEnumerable?$select=Name&$top=1&$skip=1&$orderby=NameAsHtml&$filter=Address/City eq 'Redmond'",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = DataServicesResourceUtil.GetString("RequestQueryProcessor_QueryNoOptionsApplicable"),
                        StatusCode = 400,
                    },
                    new
                    {
                        RequestUri = "/GetCustomerByIdDirectValue?id=0&$select=Name&$top=1&$skip=1&$orderby=NameAsHtml&$filter=Address/City eq 'Redmond'",
                        Accept = UnitTestsUtil.ResponseFormats,
                        ErrorMessage = DataServicesResourceUtil.GetString("RequestQueryProcessor_QueryNoOptionsApplicable"),
                        StatusCode = 400,
                    },
                };

                List<int> testCaseIdx = new List<int>();
                int idx = 0;
                for (; idx < TestCases.Length; idx++)
                {
                    testCaseIdx.Add(idx);
                }

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new[] { typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("IsBlobService", new[] { true, false }),
                    new Dimension("TestCaseIdx", testCaseIdx));

                TestUtil.RunCombinatorialEngineFail(engine, table =>
                {
                    var testCase = TestCases[(int)table["TestCaseIdx"]];

                    foreach (string accept in testCase.Accept)
                    {
                        using (TestUtil.MetadataCacheCleaner())
                        using (TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
                        using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                        using (CustomRowBasedContext.CreateChangeScope())
                        using (CustomRowBasedOpenTypesContext.CreateChangeScope())
                        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                        {
                            request.DataServiceType = (Type)table["DataServiceType"];

                            if (request.DataServiceType == typeof(CustomRowBasedContext))
                            {
                                DataServiceStreamProvider.GetStreamETagOverride = (entity, context) => "\"" + ((RowEntityTypeWithIDAsKey)entity).Properties["Name"] + "\"";
                            }
                            else
                            {
                                DataServiceStreamProvider.GetStreamETagOverride = (entity, context) => "\"" + ((RowComplexType)entity).Properties["Name"] + "\"";
                            }

                            OpenWebDataServiceHelper.EnableBlobServer.Value = (bool)table["IsBlobService"];
                            OpenWebDataServiceHelper.GetServiceCustomizer.Value = (type) =>
                            {
                                if (type == typeof(IDataServiceStreamProvider))
                                {
                                    return new DataServiceStreamProvider();
                                }

                                return null;
                            };

                            request.HttpMethod = "GET";
                            request.RequestUriString = testCase.RequestUri;
                            request.Accept = accept;
                            SetCustomRequestHeaders(request);
                            Exception e = TestUtil.RunCatching(request.SendRequest);
                            TestUtil.AssertExceptionExpected(e, testCase.ErrorMessage != null, testCase.RequestUri.Contains("/$value") && !OpenWebDataServiceHelper.EnableBlobServer);
                            if (e == null)
                            {
                                if (accept == UnitTestsUtil.MimeTextPlain)
                                {
                                    string responseText = request.GetResponseStreamAsText();
                                    int count;
                                    int.TryParse(responseText, out count);
                                    if (request.DataServiceType == typeof(CustomRowBasedContext))
                                    {
                                        Assert.AreEqual(CustomRowBasedContext.customers.Count, count);
                                    }
                                    else
                                    {
                                        Assert.AreEqual(CustomRowBasedOpenTypesContext.customers.Count, count);
                                    }
                                }
                                else
                                {
                                    List<string> xpaths = new List<string>();
                                    if (request.RequestUriString.Contains("$count=true"))
                                    {
                                        int count;
                                        if (request.DataServiceType == typeof(CustomRowBasedOpenTypesContext))
                                        {
                                            count = CustomRowBasedOpenTypesContext.customers.Count;
                                        }
                                        else
                                        {
                                            count = CustomRowBasedContext.customers.Count;
                                        }

                                        xpaths.Add("boolean(/atom:feed[adsm:count=" + count + "])");
                                    }

                                    // verify entity (or MLE) header etag
                                    if (request.ResponseETag != null && !request.RequestUriString.Contains("/$value"))
                                    {
                                        string expectedETag;
                                        if (request.DataServiceType == typeof(CustomRowBasedContext))
                                        {
                                            expectedETag = CustomRowBasedContext.customers.Single(c => c.ID == 0).Properties["GuidValue"].ToString();
                                        }
                                        else
                                        {
                                            expectedETag = CustomRowBasedOpenTypesContext.customers.Single(c => (int)c.Properties["ID"] == 0).Properties["GuidValue"].ToString();
                                        }

                                        Assert.AreEqual("W/\"" + expectedETag + "\"", request.ResponseETag);
                                    }

                                    // verify entity (or MLE) etag in payload
                                    if (request.ResponseETag != null)
                                    {
                                        // for single entry
                                        string etag = request.ResponseETag.Substring(8, request.ResponseETag.Length - 8 - 2);
                                        xpaths.Add("boolean(/atom:entry[contains(substring-before(substring-after(@adsm:etag, 'W/\"'), '\"'), '" + etag + "')])");
                                    }
                                    else
                                    {
                                        // for feed
                                        //TODO: Server not generating eTag correctly
                                        //xpaths.Add("not(//atom:entry[atom:link/@rel='edit' and not(@adsm:etag)])");
                                        if (OpenWebDataServiceHelper.EnableBlobServer)
                                        {
                                            xpaths.Add("not(//atom:entry[atom:link/@rel='edit' and not(contains(substring-before(substring-after(@adsm:etag, 'W/\"'), '\"'), adsm:properties/ads:GuidValue))])");
                                        }
                                        else
                                        {
                                            xpaths.Add("not(//atom:entry[atom:link/@rel='edit' and not(contains(substring-before(substring-after(@adsm:etag, 'W/\"'), '\"'), atom:content/adsm:properties/ads:GuidValue))])");
                                        }
                                    }

                                    // verify MR header etag
                                    if (testCase.RequestUri.Contains("/$value"))
                                    {
                                        Assert.IsNotNull(request.ResponseETag);
                                        Assert.AreEqual("\"Customer 0\"", request.ResponseETag);
                                    }

                                    // verify MR etag in payload
                                    if (OpenWebDataServiceHelper.EnableBlobServer)
                                    {
                                        xpaths.Add("not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])");
                                        xpaths.Add("not(//atom:entry/atom:link[@rel='edit-media' and not(contains(../adsm:properties/ads:Name, substring-before(substring-after(@adsm:etag, '\"'), '\"')))])");
                                    }

                                    if (accept == UnitTestsUtil.AtomFormat)
                                    {
                                        XmlDocument atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                                        UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, xpaths.ToArray());
                                    }
                                }
                            }
                            else
                            {
                                if (testCase.RequestUri.Contains("/$value") && !OpenWebDataServiceHelper.EnableBlobServer && testCase.StatusCode == 200)
                                {
                                    string errMsg = DataServicesResourceUtil.GetString("BadRequest_InvalidUriForMediaResource", new Uri(new Uri(request.BaseUri), request.RequestUriString).AbsoluteUri);
                                    UnitTestsUtil.VerifyTestException(e, 400, errMsg);
                                }
                                else
                                {
                                    UnitTestsUtil.VerifyTestException(e, testCase.StatusCode, testCase.ErrorMessage);
                                }
                            }
                        }
                    }
                });
            }

            #endregion

            #region Cross Feature - Blobs and Projections

            [TestCategory("Partition2"), TestMethod, Variation("Tests Projection on MLEs - Reflection provider")]
            public void BlobProjectionTests_ReflectionProvider()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Location", new[] { WebServerLocation.InProcess, WebServerLocation.InProcessWcf, WebServerLocation.InProcessStreamedWcf }),
                    new Dimension("Accept", UnitTestsUtil.ResponseFormats));

                TestUtil.RunCombinatorialEngineFail(engine, table =>
                {
                    string accept = (string)table["Accept"];

                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                    using (TestWebRequest request = TestWebRequest.CreateForLocation((WebServerLocation)table["Location"]))
                    using (PhotoDataServiceContext.CreateChangeScope())
                    {
                        request.RequestMaxVersion = "4.0;";
                        request.RequestVersion = "4.0;";

                        //
                        // Make sure when there's no projected properties, we omit the <m:properties /> node
                        // This should hold true for both MLE and non-MLE entities.
                        //
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Folders(0)?$select=Icon", null, null, accept, null, null, null, 200));
                        XmlDocument atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathDoesntExist(
                            atomResponse,
                            "/atom:entry/atom:content/@src",
                            "/atom:entry/adsm:properties",
                            "/atom:entry/atom:content/adsm:properties");

                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(0)?$select=Icon", null, null, accept, null, null, null, 200));
                        atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathDoesntExist(
                            atomResponse,
                            "/atom:entry/atom:content/@src",
                            "/atom:entry/adsm:properties",
                            "/atom:entry/atom:content/adsm:properties");

                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)?$select=Icon", null, null, accept, null, null, null, 200));
                        atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathExists(
                            atomResponse,
                            "/atom:entry/atom:content/@src");
                        UnitTestsUtil.VerifyXPathDoesntExist(
                            atomResponse,
                            "/atom:entry/adsm:properties",
                            "/atom:entry/atom:content/adsm:properties");

                        //
                        // Project 1 property
                        //
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)?$select=Name", null, null, accept, null, null, null, 200));
                        atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathExists(
                            atomResponse,
                            "/atom:entry/adsm:properties/ads:Name",
                            "/atom:entry/atom:content/@src");
                        UnitTestsUtil.VerifyXPathDoesntExist(
                            atomResponse,
                            "/atom:entry/adsm:properties/*[local-name()!='Name']");

                        //
                        // Project 2 primitive properties
                        //
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)?$select=Name,Description", null, null, accept, null, null, null, 200));
                        atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathExists(
                            atomResponse,
                            "/atom:entry/adsm:properties/ads:Name",
                            "/atom:entry/adsm:properties/ads:Description",
                            "/atom:entry/atom:content/@src");
                        UnitTestsUtil.VerifyXPathDoesntExist(
                            atomResponse,
                            "/atom:entry/adsm:properties/*[local-name()!='Name' and local-name()!='Description']");

                        //
                        // Explicitly project all properties
                        //
                        foreach (string queryString in new string[] { 
                            "/Items(1)?$select=*,Icon", 
                            "/Items(1)?$select=Name,*", 
                            "/Items(1)?$select=*,*"})
                        {
                            Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", queryString, null, null, accept, null, null, null, 200));
                            atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                            UnitTestsUtil.VerifyXPathExists(
                                atomResponse,
                                "/atom:entry/adsm:properties[count(*)=5]",
                                "/atom:entry/atom:content/@src");
                        }

                        // No QueryInterceptor for Service Operation
                        BLOBSupportTest.ValidateInterceptorOverride = () =>
                        {
                            InterceptorChecker.ValidateQueryInterceptor(0);
                            InterceptorChecker.ValidateChangeInterceptor(0);
                        };

                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/GetPhoto", null, null, accept, null, null, null, 200));
                        atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathExists(
                            atomResponse,
                            "/atom:entry/adsm:properties[count(*)=5]",
                            "/atom:entry/atom:content/@src");

                        BLOBSupportTest.ValidateInterceptorOverride = () =>
                        {
                            InterceptorChecker.ValidateQueryInterceptor(2);
                            InterceptorChecker.ValidateChangeInterceptor(0);
                        };

                        // 
                        // Implicitly project all properties of the expanded entity
                        // 
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(0)?$select=Icon&$expand=Icon", null, null, accept, null, null, null, 200));
                        atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathDoesntExist(
                            atomResponse,
                            "/atom:entry/atom:content/@src",
                            "/atom:entry/adsm:properties",
                            "/atom:entry/atom:content/adsm:properties");
                        UnitTestsUtil.VerifyXPathExists(
                            atomResponse,
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/atom:content/@src",
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/adsm:properties[count(*)=5]");

                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)?$select=Icon&$expand=Icon", null, null, accept, null, null, null, 200));
                        atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathDoesntExist(
                            atomResponse,
                            "/atom:entry/adsm:properties",
                            "/atom:entry/atom:content/adsm:properties");
                        UnitTestsUtil.VerifyXPathExists(
                            atomResponse,
                            "/atom:entry/atom:content/@src",
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/atom:content/@src",
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/adsm:properties[count(*)=5]");

                        // 
                        // Explicitly project all properties of the expanded entity
                        // 
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(0)?$select=Icon&$expand=Icon($select=*)", null, null, accept, null, null, null, 200));
                        atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathDoesntExist(
                            atomResponse,
                            "/atom:entry/atom:content/@src",
                            "/atom:entry/adsm:properties",
                            "/atom:entry/atom:content/adsm:properties");
                        UnitTestsUtil.VerifyXPathExists(
                            atomResponse,
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/atom:content/@src",
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/adsm:properties[count(*)=5]");

                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items(1)?$select=Icon&$expand=Icon($select=*)", null, null, accept, null, null, null, 200));
                        atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathDoesntExist(
                            atomResponse,
                            "/atom:entry/adsm:properties",
                            "/atom:entry/atom:content/adsm:properties");
                        UnitTestsUtil.VerifyXPathExists(
                            atomResponse,
                            "/atom:entry/atom:content/@src",
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/atom:content/@src",
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/adsm:properties[count(*)=5]");

                        //
                        // Project 1 property, expand an MLE and implicitly/explicitly project all properties  of the expanded MLE 
                        //
                        foreach (string queryString in new string[] { "/Items(1)?$select=Name,Icon&$expand=Icon", "/Items(1)?$select=Name,Icon&$expand=Icon($select=*)" })
                        {
                            Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", queryString, null, null, accept, null, null, null, 200));
                            atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                            UnitTestsUtil.VerifyXPathExists(
                                atomResponse,
                                "/atom:entry/adsm:properties/ads:Name",
                                "/atom:entry/atom:content/@src",
                                "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/atom:content/@src",
                                "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/adsm:properties[count(*)=5]");
                            UnitTestsUtil.VerifyXPathDoesntExist(
                                atomResponse,
                                "/atom:entry/adsm:properties/*[local-name()!='Name']");
                        }

                        //
                        // Project 1 property in the expanded MLE
                        //

                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Folders(0)?$select=Name,Items&$expand=Items($select=Name)", null, null, accept, null, null, null, 200));
                        atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathExists(
                            atomResponse,
                            "/atom:entry/atom:content/adsm:properties/ads:Name",
                            "/atom:entry/atom:link[@title='Items']/adsm:inline/atom:feed[count(atom:entry/atom:content/adsm:properties/ads:Name)=1]",
                            "/atom:entry/atom:link[@title='Items']/adsm:inline/atom:feed[count(atom:entry/adsm:properties/ads:Name)=2]",
                            "/atom:entry/atom:link[@title='Items']/adsm:inline/atom:feed[count(atom:entry/atom:content/@src)=2]");
                        UnitTestsUtil.VerifyXPathDoesntExist(
                            atomResponse,
                            "/atom:entry/atom:content/@src",
                            "/atom:entry/adsm:properties",
                            "/atom:entry/atom:content/adsm:properties/*[local-name()!='Name']",
                            "/atom:entry/atom:link[@title='Items']/adsm:inline/atom:feed/atom:entry/adsm:properties/*[local-name()!='Name']",
                            "/atom:entry/atom:link[@title='Items']/adsm:inline/atom:feed/atom:entry/atom:content/adsm:properties/*[local-name()!='Name']");

                        //
                        // Project nav property in the expanded MLE
                        //
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Folders(0)?$select=Name&$expand=Items($select=ParentFolder)", null, null, accept, null, null, null, 200));
                        atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathExists(
                            atomResponse,
                            "/atom:entry/atom:content/adsm:properties/ads:Name",
                            "/atom:entry/atom:link[@title='Items']/adsm:inline/atom:feed[count(atom:entry/atom:content/@src)=2]");
                        UnitTestsUtil.VerifyXPathDoesntExist(
                            atomResponse,
                            "/atom:entry/atom:content/@src",
                            "/atom:entry/adsm:properties",
                            "/atom:entry/atom:content/adsm:properties/*[local-name()!='Name']",
                            "/atom:entry/atom:link[@title='Items']/adsm:inline/atom:feed/atom:entry/adsm:properties",
                            "/atom:entry/atom:link[@title='Items']/adsm:inline/atom:feed/atom:entry/atom:content/adsm:properties");

                        //
                        // V1 expand
                        //
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Folders(0)?$expand=Items", null, null, accept, null, null, null, 200));
                        atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathExists(
                            atomResponse,
                            "/atom:entry/atom:content/adsm:properties",
                            "/atom:entry/atom:link[@title='Items']/adsm:inline/atom:feed[count(atom:entry/atom:content/adsm:properties[count(*)=3])=1]",
                            "/atom:entry/atom:link[@title='Items']/adsm:inline/atom:feed[count(atom:entry/adsm:properties[count(*)=5])=1]",
                            "/atom:entry/atom:link[@title='Items']/adsm:inline/atom:feed[count(atom:entry/adsm:properties[count(*)=6])=1]",
                            "/atom:entry/atom:link[@title='Items']/adsm:inline/atom:feed[count(atom:entry/atom:content/@src)=2]");
                        UnitTestsUtil.VerifyXPathDoesntExist(
                            atomResponse,
                            "/atom:entry/atom:content/@src",
                            "/atom:entry/adsm:properties");

                        BLOBSupportTest.ValidateInterceptorOverride = () =>
                        {
                            InterceptorChecker.ValidateQueryInterceptor(4);
                            InterceptorChecker.ValidateChangeInterceptor(0);
                        };

                        //
                        // multilevel expand and project, only project 1 property per level
                        //
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Folders(0)?$select=Name&$expand=Icon($select=Name;$expand=ParentFolder($select=Name;$expand=Items($select=Name)))", null, null, accept, null, null, null, 200));
                        atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathExists(
                            atomResponse,
                            "/atom:entry/atom:content/adsm:properties/ads:Name",
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/atom:content/@src",
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/adsm:properties/ads:Name",
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/atom:link[@title='ParentFolder']/adsm:inline/atom:entry/atom:content/adsm:properties/ads:Name",
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/atom:link[@title='ParentFolder']/adsm:inline/atom:entry/atom:link[@title='Items']/adsm:inline/atom:feed[count(atom:entry/atom:content/@src)=2]",
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/atom:link[@title='ParentFolder']/adsm:inline/atom:entry/atom:link[@title='Items']/adsm:inline/atom:feed[count(atom:entry/adsm:properties/ads:Name)=2]",
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/atom:link[@title='ParentFolder']/adsm:inline/atom:entry/atom:link[@title='Items']/adsm:inline/atom:feed[count(atom:entry/atom:content/adsm:properties/ads:Name)=1]");
                        UnitTestsUtil.VerifyXPathDoesntExist(
                            atomResponse,
                            "/atom:entry/atom:content/@src",
                            "/atom:entry/adsm:properties",
                            "/atom:entry/atom:content/adsm:properties/*[local-name()!='Name']",
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/adsm:properties/*[local-name()!='Name']",
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/atom:link[@title='ParentFolder']/adsm:inline/atom:entry/atom:content/@src",
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/atom:link[@title='ParentFolder']/adsm:inline/atom:entry/atom:content/adsm:properties/*[local-name()!='Name']",
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/atom:link[@title='ParentFolder']/adsm:inline/atom:entry/atom:link[@title='Items']/adsm:inline/atom:feed/atom:entry/atom:content/adsm:properties/*[local-name()!='Name']");

                        //
                        // multilevel expand and project, only project 0 property per level
                        //
                        Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Folders(0)?$expand=Icon($expand=ParentFolder($expand=Items($select=Icon)))", null, null, accept, null, null, null, 200));
                        atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathExists(
                            atomResponse,
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/atom:content/@src",
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/atom:link[@title='ParentFolder']/adsm:inline/atom:entry/atom:link[@title='Items']/adsm:inline/atom:feed[count(atom:entry/atom:content/@src)=2]");
                        UnitTestsUtil.VerifyXPathDoesntExist(
                            atomResponse,
                            "/atom:entry/atom:content/@src",
                            "/atom:entry/adsm:properties",
                            //"/atom:entry/atom:content/adsm:properties"
                            //"/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/adsm:properties");
                            "/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/atom:link[@title='ParentFolder']/adsm:inline/atom:entry/atom:content/@src");
                        //"/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/atom:link[@title='ParentFolder']/adsm:inline/atom:entry/atom:content/adsm:properties",
                        //"/atom:entry/atom:link[@title='Icon']/adsm:inline/atom:entry/atom:link[@title='ParentFolder']/adsm:inline/atom:entry/atom:link[@title='Items']/adsm:inline/atom:feed/atom:entry/adsm:properties");

                        BLOBSupportTest.ValidateInterceptorOverride = () =>
                        {
                            InterceptorChecker.ValidateQueryInterceptor(5);
                            InterceptorChecker.ValidateChangeInterceptor(0);
                        };
                    }
                });
            }

            // Ignore this as it is for atom, and bound to nw db.
            [Ignore] 
            [TestCategory("Partition2"), TestMethod, Variation("Tests Projection on MLEs - EF provider")]
            public void BlobProjectionTests_EFProvider()
            {
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (NorthwindDefaultStreamService.SetupNorthwindWithStreamAndETag(
                    new KeyValuePair<string, string>[] {
                        new KeyValuePair<string, string>("Customers", "true"),
                        new KeyValuePair<string, string>("Orders", "true")
                    },
                    new KeyValuePair<string[], string>[] {
                        new KeyValuePair<string[], string>(new string[] {"Customers", "Phone"}, "Fixed"),
                        new KeyValuePair<string[], string>(new string[] {"Orders", "Freight"}, "Fixed")
                    },
                    "BlobSupportTest_BlobProjectionTests_EFProvider"))
                {
                    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                        new Dimension("Location", new[] { WebServerLocation.InProcess, WebServerLocation.InProcessWcf, WebServerLocation.InProcessStreamedWcf }),
                        new Dimension("Accept", UnitTestsUtil.ResponseFormats));

                    TestUtil.RunCombinatorialEngineFail(engine, table =>
                    {
                        string accept = (string)table["Accept"];

                        using (TestWebRequest request = TestWebRequest.CreateForLocation((WebServerLocation)table["Location"]))
                        {
                            // Customers, Orders are MLE, Order_Details is non-MLE
                            var testCases = new[] {
                                // Make sure when there's no projected properties, we omit the <m:properties /> node
                                // This should hold true for both MLE and non-MLE entities.
                                new {
                                    QueryStrings = new string[] {
                                        "/Order_Details(OrderID=10355,ProductID=1)?$expand=Orders",
                                        "/Orders(10355)?$expand=Order_Details",
                                    },
                                    XPathExprs = new string[] {
                                        // Order_Details(OrderID=10643,ProductID=28) is not MLE and must not have @src while Orders(10643) must have @src because it is an MLE 
                                        "not(/atom:entry[contains(atom:id, 'Order_Details(OrderID=10355,ProductID=1)')]/atom:content/@src) or boolean(/atom:entry[contains(atom:id, 'Orders(10355)')]/atom:content/@src)",
                                        // "boolean(/atom:entry/atom:content/adsm:properties)",

                                        "not(//atom:entry/atom:link[@rel='edit-media' and not(contains(../atom:link[@rel='edit']/@href, substring-before(substring-after(@adsm:etag, '\"'), '\"')))])",
                                        "count(//atom:entry[atom:link[@rel='edit' and @title='Order_Details'] and @adsm:etag]) = 0",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")] and contains(@adsm:etag, '030-0074321')]) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')] and @adsm:etag =  'W/\"29.4600\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')] and @adsm:etag =  'W/\"1.2100\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')]])"
                                    },
                                    ExpectedQueryInterceptorCalls = 2

                                },
                                // Project 1 property
                                new {
                                    QueryStrings = new string[] {
                                        "/Customers('ALFKI')?$select=City"
                                    },
                                    XPathExprs = new string[] {
                                        "boolean(/atom:entry/adsm:properties/ads:City)",
                                        "boolean(/atom:entry/atom:content/@src)",
                                        "not(/atom:entry/adsm:properties/*[local-name()!='City'])",

                                        "not(//atom:entry/atom:link[@rel='edit-media' and not(contains(../atom:link[@rel='edit']/@href, substring-before(substring-after(@adsm:etag, '\"'), '\"')))])",
                                        "count(//atom:entry[atom:link[@rel='edit' and @title='Order_Details'] and @adsm:etag]) = 0",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")] and contains(@adsm:etag, '030-0074321')]) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')] and @adsm:etag =  'W/\"29.4600\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')] and @adsm:etag =  'W/\"1.2100\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')]])"
                                    },
                                    ExpectedQueryInterceptorCalls = 1
                                },
                                // Project 2 primitive properties
                                new {
                                    QueryStrings = new string[] {
                                        "/Customers('ALFKI')?$select=City,CompanyName",
                                        "/Customers('ALFKI')?$select=City,*"
                                    },
                                    XPathExprs = new string[] {
                                        "boolean(/atom:entry/adsm:properties/ads:City)",
                                        "boolean(/atom:entry/adsm:properties/ads:CompanyName)",
                                        "boolean(/atom:entry/atom:content/@src)",
                                        "count(/atom:entry/adsm:properties/*) >= 2",

                                        "not(//atom:entry/atom:link[@rel='edit-media' and not(contains(../atom:link[@rel='edit']/@href, substring-before(substring-after(@adsm:etag, '\"'), '\"')))])",
                                        "count(//atom:entry[atom:link[@rel='edit' and @title='Order_Details'] and @adsm:etag]) = 0",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")] and contains(@adsm:etag, '030-0074321')]) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')] and @adsm:etag =  'W/\"29.4600\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')] and @adsm:etag =  'W/\"1.2100\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')]])"
                                    },
                                    ExpectedQueryInterceptorCalls = 1
                                },
                                // Explicitly project all properties
                                new {
                                    QueryStrings = new string[] {
                                        "/Customers('ALFKI')?$select=*,*",
                                        "/Customers?$filter=City eq 'Berlin'&$select=*",
                                    },
                                    XPathExprs = new string[] {
                                        "not((/atom:entry | /atom:feed/atom:entry)/atom:content/adsm:properties)",
                                        "count((/atom:entry | /atom:feed/atom:entry)/adsm:properties/*) = 10",
                                        "boolean((/atom:entry | /atom:feed/atom:entry)/atom:content/@src)",

                                        "not(//atom:entry/atom:link[@rel='edit-media' and not(contains(../atom:link[@rel='edit']/@href, substring-before(substring-after(@adsm:etag, '\"'), '\"')))])",
                                        "count(//atom:entry[atom:link[@rel='edit' and @title='Order_Details'] and @adsm:etag]) = 0",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")] and contains(@adsm:etag, '030-0074321')]) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')] and @adsm:etag =  'W/\"29.4600\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')] and @adsm:etag =  'W/\"1.2100\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')]])"
                                    },
                                    ExpectedQueryInterceptorCalls = 1
                                },
                                // Implicitly/Explicitly project all properties of the expanded entity
                                new {
                                    QueryStrings = new string[] {
                                        "/Order_Details(OrderID=10643,ProductID=28)?$expand=Orders",  /*Implicit projection */
                                        "/Order_Details(OrderID=10643,ProductID=28)?$expand=Orders($select=*)" /*Explicit projection */
                                    },
                                    XPathExprs = new string[] {
                                        "not(/atom:entry/atom:content/@src)",
                                        "not(/atom:entry/adsm:properties)",
                                        "boolean(/atom:entry/atom:content/adsm:properties)",
                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:content/@src)",
                                        "count(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/adsm:properties/*) = 10",

                                        "not(//atom:entry/atom:link[@rel='edit-media' and not(contains(../atom:link[@rel='edit']/@href, substring-before(substring-after(@adsm:etag, '\"'), '\"')))])",
                                        "count(//atom:entry[atom:link[@rel='edit' and @title='Order_Details'] and @adsm:etag]) = 0",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")] and contains(@adsm:etag, '030-0074321')]) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')] and @adsm:etag =  'W/\"29.4600\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')] and @adsm:etag =  'W/\"1.2100\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')]])"
                                    },
                                    ExpectedQueryInterceptorCalls = 2
                                },
                                new {
                                    QueryStrings = new string[] {
                                        "/Customers('ALFKI')?$expand=Orders",        /*Implicit projection */
                                        "/Customers('ALFKI')?$expand=Orders($select=*)",      /*Explicit projection */
                                    },
                                    XPathExprs = new string[] {
                                        "boolean(/atom:entry/adsm:properties)",
                                        "not(/atom:entry/atom:content/adsm:properties)",
                                        "boolean(/atom:entry/atom:content/@src)",
                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content)",
                                        "not(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content[not(@src)])",
                                        "not(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry[count(adsm:properties/*) != 10])",

                                        "not(//atom:entry/atom:link[@rel='edit-media' and not(contains(../atom:link[@rel='edit']/@href, substring-before(substring-after(@adsm:etag, '\"'), '\"')))])",
                                        "count(//atom:entry[atom:link[@rel='edit' and @title='Order_Details'] and @adsm:etag]) = 0",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")] and contains(@adsm:etag, '030-0074321')]) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')] and @adsm:etag =  'W/\"29.4600\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')] and @adsm:etag =  'W/\"1.2100\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')]])"
                                    },
                                    ExpectedQueryInterceptorCalls = 2
                                },
                                // Project 1 property, expand an MLE and implicitly/explicitly project all properties  of the expanded MLE 
                                new {
                                    QueryStrings = new string[] {
                                        "/Customers('ALFKI')?$select=*&$expand=Orders", 
                                        "/Customers('ALFKI')?$select=City&$expand=Orders",
                                        "/Customers('ALFKI')?$select=City&$expand=Orders($select=*)"
                                    },
                                    XPathExprs = new string[] {
                                        "boolean(/atom:entry/adsm:properties/ads:City)",
                                        "count(/atom:entry/adsm:properties/*) >= 1",
                                        "not(/atom:entry/atom:content/adsm:properties)",
                                        "boolean(/atom:entry/atom:content/@src)",
                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content)",
                                        "not(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content[not(@src)])",
                                        "not(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry[count(adsm:properties/*) != 10])",

                                        "not(//atom:entry/atom:link[@rel='edit-media' and not(contains(../atom:link[@rel='edit']/@href, substring-before(substring-after(@adsm:etag, '\"'), '\"')))])",
                                        "count(//atom:entry[atom:link[@rel='edit' and @title='Order_Details'] and @adsm:etag]) = 0",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")] and contains(@adsm:etag, '030-0074321')]) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')] and @adsm:etag =  'W/\"29.4600\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')] and @adsm:etag =  'W/\"1.2100\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')]])"
                                    },
                                    ExpectedQueryInterceptorCalls = 2
                                },
                                // Project 1 property in the expanded MLE
                                new {
                                    QueryStrings = new string[] {
                                        "/Order_Details(OrderID=10643,ProductID=28)?$select=Quantity&$expand=Orders($select=ShipCity)"
                                    },
                                    XPathExprs = new string[] {
                                        "not(/atom:entry/adsm:properties)",
                                        "not(/atom:entry/adsm:content/@src)",
                                        "boolean(/atom:entry/atom:content/adsm:properties/ads:Quantity)",
                                        "count(/atom:entry/atom:content/adsm:properties/*) = 1",
            
                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:content)",
                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/adsm:properties)",
                                        "not(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:content/*)",
                                        "not(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:content[not(@src)])",
                                        "not(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/adsm:properties[not(ads:ShipCity)])",
                                        "not(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/adsm:properties[count(*) != 1])",

                                        "not(//atom:entry/atom:link[@rel='edit-media' and not(contains(../atom:link[@rel='edit']/@href, substring-before(substring-after(@adsm:etag, '\"'), '\"')))])",
                                        "count(//atom:entry[atom:link[@rel='edit' and @title='Order_Details'] and @adsm:etag]) = 0",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")] and contains(@adsm:etag, '030-0074321')]) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')] and @adsm:etag =  'W/\"29.4600\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')] and @adsm:etag =  'W/\"1.2100\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')]])"
                                    },
                                    ExpectedQueryInterceptorCalls = 2
                                },
                                // Project nav property in the expanded MLE
                                new {
                                    QueryStrings = new string[] {
                                        "/Order_Details(OrderID=10643,ProductID=28)?$select=Quantity&$expand=Orders($expand=Customers)"
                                    },
                                    XPathExprs = new string[] {
                                        "not(/atom:entry/adsm:properties)",
                                        "not(/atom:entry/adsm:content/@src)",
                                        "boolean(/atom:entry/atom:content/adsm:properties/ads:Quantity)",
                                        "count(/atom:entry/atom:content/adsm:properties/*) = 1",

                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:content)",
                                        "not(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:content/*)",
                                        "not(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/adsm:content[not(@src)])",
                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/adsm:properties/*)",

                                        "not(//atom:entry/atom:link[@rel='edit-media' and not(contains(../atom:link[@rel='edit']/@href, substring-before(substring-after(@adsm:etag, '\"'), '\"')))])",
                                        "count(//atom:entry[atom:link[@rel='edit' and @title='Order_Details'] and @adsm:etag]) = 0",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")] and contains(@adsm:etag, '030-0074321')]) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')] and @adsm:etag =  'W/\"29.4600\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')] and @adsm:etag =  'W/\"1.2100\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')]])"
                                    },
                                    ExpectedQueryInterceptorCalls = 3
                                },
                                // V1 expand
                                new {
                                    QueryStrings = new string[] {
                                        "/Order_Details(OrderID=10643,ProductID=28)?$expand=Orders"
                                    },
                                    XPathExprs = new string[] {
                                        "not(/atom:entry/adsm:properties)",
                                        "not(/atom:entry/adsm:content/@src)",
                                        "count(/atom:entry/atom:content/adsm:properties/*) = 5",

                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:content)",
                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/adsm:properties)",
                                        "not(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:content/*)",
                                        "not(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:content[not(@src)])",
                                        "not(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/adsm:properties[not(ads:ShipCity)])",
                                        "not(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/adsm:properties[count(*) != 10])",

                                        "not(//atom:entry/atom:link[@rel='edit-media' and not(contains(../atom:link[@rel='edit']/@href, substring-before(substring-after(@adsm:etag, '\"'), '\"')))])",
                                        "count(//atom:entry[atom:link[@rel='edit' and @title='Order_Details'] and @adsm:etag]) = 0",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")] and contains(@adsm:etag, '030-0074321')]) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')] and @adsm:etag =  'W/\"29.4600\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')] and @adsm:etag =  'W/\"1.2100\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')]])"
                                    },
                                    ExpectedQueryInterceptorCalls = 2
                                },
                                // multilevel expand and project, only project 1 property per level
                                new {
                                    QueryStrings = new string[] {
                                        "/Order_Details(OrderID=10643,ProductID=28)?$select=Quantity&$expand=Orders($expand=Customers($select=CompanyName;$expand=Orders($select=ShipCity)))"
                                    },
                                    XPathExprs = new string[] {
                                        "not(/atom:entry/adsm:properties)",
                                        "not(/atom:entry/adsm:content/@src)",
                                        "boolean(/atom:entry/atom:content/adsm:properties/ads:Quantity)",
                                        "count(/atom:entry/atom:content/adsm:properties/*) = 1",

                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:content)",
                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/adsm:properties)",
                                        "not    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:content/*)",
                                        "not    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:content[not(@src)])",
                                        "not    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/adsm:properties[not(ads:ShipCity)])",
                                        "boolean    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/adsm:properties[count(*) != 1])",

                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:link[@title='Customers']/adsm:inline/atom:entry/atom:content)",
                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:link[@title='Customers']/adsm:inline/atom:entry/adsm:properties)",
                                        "not    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:link[@title='Customers']/adsm:inline/atom:entry/atom:content/*)",
                                        "not    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:link[@title='Customers']/adsm:inline/atom:entry/atom:content[not(@src)])",
                                        "not    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:link[@title='Customers']/adsm:inline/atom:entry/adsm:properties[not(ads:CompanyName)])",
                                        "boolean    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:link[@title='Customers']/adsm:inline/atom:entry/adsm:properties[count(*) != 1])",

                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:link[@title='Customers']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content)",
                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:link[@title='Customers']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/adsm:properties)",
                                        "not    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:link[@title='Customers']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content/*)",
                                        "not    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:link[@title='Customers']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content[not(@src)])",
                                        "not    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:link[@title='Customers']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/adsm:properties[not(ads:ShipCity)])",
                                        "boolean    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:link[@title='Customers']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/adsm:properties[count(*) != 1])",

                                        "not(//atom:entry/atom:link[@rel='edit-media' and not(contains(../atom:link[@rel='edit']/@href, substring-before(substring-after(@adsm:etag, '\"'), '\"')))])",
                                        "count(//atom:entry[atom:link[@rel='edit' and @title='Order_Details'] and @adsm:etag]) = 0",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")] and contains(@adsm:etag, '030-0074321')]) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')] and @adsm:etag =  'W/\"29.4600\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')] and @adsm:etag =  'W/\"1.2100\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')]])"
                                    },
                                    ExpectedQueryInterceptorCalls = 4
                                },
                                // multilevel expand and project, only project 0 property per level
                                new {
                                    QueryStrings = new string[] {
                                        "/Order_Details(OrderID=10643,ProductID=28)?$expand=Orders($expand=Customers($select=CustomerDemographics))"
                                    },
                                    XPathExprs = new string[] {
                                        "not(/atom:entry/adsm:properties)",
                                        "not(/atom:entry/adsm:content/@src)",
                                        "count(/atom:entry/atom:content/adsm:properties/*) > 0",

                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:content)",
                                        "boolean    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/adsm:properties)",
                                        "not    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:content/*)",
                                        "not    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:content[not(@src)])",

                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:link[@title='Customers']/adsm:inline/atom:entry/atom:content)",
                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:link[@title='Customers']/adsm:inline/atom:entry/adsm:properties)",
                                        "not    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:link[@title='Customers']/adsm:inline/atom:entry/atom:content/*)",
                                        "not    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:link[@title='Customers']/adsm:inline/atom:entry/atom:content[not(@src)])",

                                        "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:link[@title='Customers']/adsm:inline/atom:entry/atom:link[@title='CustomerDemographics'])",
                                        "not(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:entry/atom:link[@title='Customers']/adsm:inline/atom:entry/atom:link[@title='CustomerDemographics']/*)",

                                        "not(//atom:entry/atom:link[@rel='edit-media' and not(contains(../atom:link[@rel='edit']/@href, substring-before(substring-after(@adsm:etag, '\"'), '\"')))])",
                                        "count(//atom:entry[atom:link[@rel='edit' and @title='Order_Details'] and @adsm:etag]) = 0",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")] and contains(@adsm:etag, '030-0074321')]) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')] and @adsm:etag =  'W/\"29.4600\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')] and @adsm:etag =  'W/\"1.2100\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')]])"
                                    },
                                    ExpectedQueryInterceptorCalls = 3
                                },
                                new {
                                    QueryStrings = new string[] {
                                        "/Customers?$top=5&$select=CompanyName&$skip=10",
                                    },
                                    XPathExprs = new string[] {
                                        "not(/atom:feed/atom:entry/atom:content/adsm:properties)",
                                        "count(/atom:feed/atom:entry/atom:content[@src]) = 5",
                                        "count(/atom:feed/atom:entry/adsm:properties/ads:CompanyName) = 5",

                                        "not(//atom:entry/atom:link[@rel='edit-media' and not(contains(../atom:link[@rel='edit']/@href, substring-before(substring-after(@adsm:etag, '\"'), '\"')))])",
                                        "count(//atom:entry[atom:link[@rel='edit' and @title='Order_Details'] and @adsm:etag]) = 0",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")] and contains(@adsm:etag, '030-0074321')]) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')] and @adsm:etag =  'W/\"29.4600\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')] and @adsm:etag =  'W/\"1.2100\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')]])"
                                    },
                                    ExpectedQueryInterceptorCalls = 1
                                },
                                new {
                                    QueryStrings = new string[] {
                                        "/Customers?$select=CompanyName,City&$filter=CustomerID eq 'CACTU'",
                                    },
                                    XPathExprs = new string[] {
                                        "not(/atom:feed/atom:entry/atom:content/adsm:properties)",
                                        "count(/atom:feed/atom:entry/atom:content[@src]) = 1",
                                        "count(/atom:feed/atom:entry/adsm:properties/*) = 2",
                                        "boolean(/atom:feed/atom:entry/adsm:properties/ads:CompanyName)",
                                        "boolean(/atom:feed/atom:entry/adsm:properties/ads:City)",

                                        "not(//atom:entry/atom:link[@rel='edit-media' and not(contains(../atom:link[@rel='edit']/@href, substring-before(substring-after(@adsm:etag, '\"'), '\"')))])",
                                        "count(//atom:entry[atom:link[@rel='edit' and @title='Order_Details'] and @adsm:etag]) = 0",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")] and contains(@adsm:etag, '030-0074321')]) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')] and @adsm:etag =  'W/\"29.4600\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')] and @adsm:etag =  'W/\"1.2100\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')]])"
                                    },
                                    ExpectedQueryInterceptorCalls = 1
                                },
                                new {
                                    QueryStrings = new string[] {
                                        "/Order_Details?$filter=Orders/ShipCity eq 'Berlin'&$select=OrderID"
                                    },
                                    XPathExprs = new string[] {
                                        "not(/atom:feed/atom:entry/atom:content/@src)",
                                        "not(/atom:feed/atom:entry/adsm:properties)",
                                        "boolean(/atom:feed/atom:entry/atom:content)",
                                        "not(/atom:feed/atom:entry/atom:content/adsm:properties[not(ads:OrderID)])",
                                        "not(/atom:feed/atom:entry/atom:content/adsm:properties[count(*) != 1])",

                                        "not(//atom:entry/atom:link[@rel='edit-media' and not(contains(../atom:link[@rel='edit']/@href, substring-before(substring-after(@adsm:etag, '\"'), '\"')))])",
                                        "count(//atom:entry[atom:link[@rel='edit' and @title='Order_Details'] and @adsm:etag]) = 0",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")] and contains(@adsm:etag, '030-0074321')]) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, \"Customers('ALFKI')\")]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')] and @adsm:etag =  'W/\"29.4600\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(10643)')]])",
                                        "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')] and @adsm:etag =  'W/\"1.2100\"']) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders(11011)')]])"
                                    },
                                    ExpectedQueryInterceptorCalls = 2
                                },
                            };

                            DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) =>
                            {

                                string entityName = entity.GetType().Name;
                                string entityId = entity is NorthwindModel.Customers ? "'" + ((NorthwindModel.Customers)entity).CustomerID + "'" :
                                                  entity is NorthwindModel.Orders ? ((NorthwindModel.Orders)entity).OrderID.ToString() :
                                                  string.Empty;

                                return string.Format("W/\"{0}({1})\"", entityName, entityId);
                            };

                            foreach (var testCase in testCases)
                            {
                                foreach (string queryString in testCase.QueryStrings)
                                {
                                    BLOBSupportTest.ValidateInterceptorOverride = () =>
                                    {
                                        InterceptorChecker.ValidateQueryInterceptor(testCase.ExpectedQueryInterceptorCalls);
                                        InterceptorChecker.ValidateChangeInterceptor(0);
                                    };

                                    Assert.IsNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "GET", queryString, null, null, accept, null, null, null, 200));

                                    string expectedHeaderETagContent = queryString.Contains("$expand=") ? null :
                                                                       queryString.StartsWith("/Customers(") ? "'030-0074321'" :
                                                                       queryString.StartsWith("/Orders(") ? "29.4600" :
                                                                       null;

                                    string expectedHeaderETag = expectedHeaderETagContent != null ? string.Format("W/\"{0}\"", expectedHeaderETagContent) : null;

                                    Assert.AreEqual(expectedHeaderETag, request.ResponseETag);

                                    XmlDocument atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                                    UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, testCase.XPathExprs);
                                }
                            }
                        }
                    });
                }
            }


            [TestCategory("Partition2"), TestMethod, Variation("Tests Projection on MLEs Client Queries- EF provider")]
            public void BlobProjectionTestsClientQueries_EFProvider()
            {
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (NorthwindDefaultStreamService.SetupNorthwindWithStreamAndETag(
                    new KeyValuePair<string, string>[] {
                        new KeyValuePair<string, string>("Customers", "true"),
                        new KeyValuePair<string, string>("Orders", "true")
                    },
                    new KeyValuePair<string[], string>[] {
                        new KeyValuePair<string[], string>(new string[] {"Customers", "Phone"}, "Fixed"),
                        new KeyValuePair<string[], string>(new string[] {"Orders", "Freight"}, "Fixed")
                    },
                    "BlobSupportTest_BlobProjectionTestsClientQueries_EFProvider"))
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.ServiceType = typeof(NorthwindDefaultStreamService);
                    request.StartService();

                    DataServiceContext ctx = new DataServiceContext(request.ServiceRoot);
                    ctx.EnableAtom = true;
                    ctx.Format.UseAtom();

                    ctx.SendingRequest2 += new EventHandler<SendingRequest2EventArgs>(ctx_SendingRequest);

                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                    {
                        BLOBSupportTest.ValidateInterceptorOverride = () =>
                        {
                            InterceptorChecker.ValidateQueryInterceptor(2);
                            InterceptorChecker.ValidateChangeInterceptor(0);
                        };

                        // "/Customers('AROUT')?$select=Orders/*&$expand=Orders"
                        var q1 = from e in ctx.CreateQuery<northwindClient.Customers>("Customers")
                                 where e.CustomerID == "AROUT"
                                 select new
                                 {
                                     Orders = e.Orders
                                 };

                        var anonEntityFromCustomers = q1.FirstOrDefault();
                        Assert.IsNotNull(anonEntityFromCustomers);
                        Assert.IsInstanceOfType(anonEntityFromCustomers.Orders.First(), typeof(northwindClient.Orders));
                    }

                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                    {
                        BLOBSupportTest.ValidateInterceptorOverride = () =>
                        {
                            InterceptorChecker.ValidateQueryInterceptor(4);
                            InterceptorChecker.ValidateChangeInterceptor(0);
                        };

                        // "/Order_Details(OrderID=10285,ProductID=1)?$select=Quantity,Orders/Customers/CompanyName,Orders/Customers/Orders/ShipCity&$expand=Orders/Customers/Orders"
                        var q2 = from e in ctx.CreateQuery<northwindClient.Order_Details>("Order_Details")
                                 where e.OrderID == 10285 && e.ProductID == 1
                                 select new
                                 {
                                     Quantity = e.Quantity,
                                     CompanyName = e.Orders.Customers.CompanyName,
                                     ShipCities = from o in e.Orders.Customers.Orders
                                                  select o.ShipCity
                                 };

                        var anonEntity = q2.FirstOrDefault();
                        Assert.IsNotNull(anonEntity);
                        Assert.AreEqual(45, anonEntity.Quantity);
                        Assert.AreEqual("QUICK-Stop", anonEntity.CompanyName);
                        Assert.AreEqual(1, anonEntity.ShipCities.Count());
                        Assert.AreEqual("Cunewalde", anonEntity.ShipCities.Single());
                    }
                }
            }

            #endregion

            #region Cross Feature - Projections and Expand on MLEs

            [TestCategory("Partition2"), TestMethod, Variation("Tests Projection and Expand on MLEs - IDSP provider")]
            public void BlobProjectionTests_IDSP()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Location", new[] { WebServerLocation.InProcess, WebServerLocation.InProcessWcf, WebServerLocation.InProcessStreamedWcf }),
                    new Dimension("Accept", UnitTestsUtil.ResponseFormats));

                TestUtil.RunCombinatorialEngineFail(engine, table =>
                {
                    string accept = (string)table["Accept"];

                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
                    using (CustomRowBasedContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForLocation((WebServerLocation)table["Location"]))
                    {
                        OpenWebDataServiceHelper.EnableBlobServer.Value = true;

                        // Customers are MLEs, Orders are non-MLEs
                        var testCases = new[] {
                            // Make sure when there's no projected properties, we omit the <m:properties /> node
                            // This should hold true for both MLE and non-MLE entities.
                            new {
                                QueryStrings = new string[] {
                                    "/Customers(0)?$select=Orders",
                                    "/GetCustomerByIdQueryable?id=0&$select=Orders",
                                    "/Orders(0)?$select=OrderDetails",
                                    "/GetOrderByIdQueryable?id=0&$select=OrderDetails",
                                },
                                XPathExprs = new string[] {
                                    // Orders(0) is not MLE and must not have @src while Customers(0) must have @src because it is an MLE 
                                    "not(/atom:entry[contains(atom:id, 'Orders(0)')]/atom:content/@src) or boolean(/atom:entry[contains(atom:id, 'Customers(0)')]/atom:content/@src)",
                                    "not(/atom:entry/adsm:properties)",
                                    "not(/atom:entry/atom:content/adsm:properties)",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                }
                            },
                            // Project 1 property
                            new {
                                QueryStrings = new string[] {
                                    "/Customers(0)?$select=Name",
                                    "/GetCustomerByIdQueryable?id=0&$select=Name",
                                },
                                XPathExprs = new string[] {
                                    "boolean(/atom:entry/adsm:properties/ads:Name)",
                                    "boolean(/atom:entry/atom:content/@src)",
                                    "not(/atom:entry/adsm:properties/*[local-name()!='Name'])",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            // Project 2 primitive properties
                            new {
                                QueryStrings = new string[] {
                                    "/Customers(0)?$select=Name,Address",
                                    "/GetCustomerByIdQueryable?id=0&$select=Name,Address",
                                },
                                XPathExprs = new string[] {
                                    "boolean(/atom:entry/adsm:properties/ads:Name)",
                                    "boolean(/atom:entry/adsm:properties/ads:Address)",
                                    "boolean(/atom:entry/atom:content/@src)",
                                    "count(/atom:entry/adsm:properties/*) = 2",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            // Explicitly project all properties
                            new {
                                QueryStrings = new string[] {
                                    "/Customers(0)?$select=*,Orders", 
                                    "/Customers(0)?$select=Address,*", 
                                    "/Customers(0)?$select=*,*",
                                    "/Customers?$filter=ID eq 0&$select=*",
                                    
                                    "/GetCustomerByIdQueryable?id=0&$select=*,Orders", 
                                    "/GetCustomerByIdQueryable?id=0&$select=Address,*", 
                                    "/GetCustomerByIdQueryable?id=0&$select=*,*",
                                    "/GetCustomerByIdQueryable?id=0&$filter=ID eq 0&$select=*",
                                },
                                XPathExprs = new string[] {
                                    "not((/atom:entry | /atom:feed/atom:entry)/atom:content/adsm:properties)",
                                    "count((/atom:entry | /atom:feed/atom:entry)/adsm:properties/*) = 5",
                                    "boolean((/atom:entry | /atom:feed/atom:entry)/atom:content/@src)",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            // Implicitly/Explicitly project all properties of the expanded entity
                            new {
                                QueryStrings = new string[] {
                                    "/Orders(0)?$select=Customer&$expand=Customer",  /*Implicit projection */
                                    "/Orders(0)?$select=Customer&$expand=Customer($select=*)", /*Explicit projection */
                                    "/GetOrderByIdQueryable?id=0&$select=Customer&$expand=Customer",  /*Implicit projection */
                                    "/GetOrderByIdQueryable?id=0&$select=Customer&$expand=Customer($select=*)" /*Explicit projection */

                                },
                                XPathExprs = new string[] {
                                    "not(/atom:entry/atom:content/@src)",
                                    "not(/atom:entry/adsm:properties)",
                                    "not(/atom:entry/atom:content/adsm:properties)",
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/@src)",
                                    "count(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties/*) = 5",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            // Project 1 property, expand an MLE and implicitly/explicitly project all properties  of the expanded MLE 
                            new {
                                QueryStrings = new string[] {
                                    "/Orders(0)?$select=ID,Customer&$expand=Customer",
                                    "/Orders(0)?$select=ID,Customer&$expand=Customer($select=*)",
                                    "/GetOrderByIdQueryable?id=0&$select=ID,Customer&$expand=Customer",
                                    "/GetOrderByIdQueryable?id=0&$select=ID,Customer&$expand=Customer($select=*)",
                                },
                                XPathExprs = new string[] {
                                    "boolean(/atom:entry/atom:content/adsm:properties/ads:ID)",
                                    "count(/atom:entry/atom:content/adsm:properties/*) = 1",
                                    "not(/atom:entry/adsm:properties)",
                                    "not(/atom:entry/atom:content/@src)",
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/@src)",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry[count(adsm:properties/*) != 5])",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            // Project 1 property in the expanded MLE
                            new {
                                QueryStrings = new string[] {
                                    "/Orders(0)?$select=ID,Customer&$expand=Customer($select=Name)",
                                    "/GetOrderByIdQueryable?id=0&$select=ID,Customer&$expand=Customer($select=Name)"
                                },
                                XPathExprs = new string[] {
                                    "not(/atom:entry/adsm:properties)",
                                    "not(/atom:entry/adsm:content/@src)",
                                    "boolean(/atom:entry/atom:content/adsm:properties/ads:ID)",
                                    "count(/atom:entry/atom:content/adsm:properties/*) = 1",
        
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content)",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/*)",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content[not(@src)])",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties[not(ads:Name)])",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties[count(*) != 1])",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            // Project nav property in the expanded MLE
                            new {
                                QueryStrings = new string[] {
                                    "/Orders(0)?$select=ID,Customer&$expand=Customer($expand=Orders)",
                                    "/GetOrderByIdQueryable?id=0&$select=ID,Customer&$expand=Customer($expand=Orders)"},
                                XPathExprs = new string[] {
                                    "not(/atom:entry/adsm:properties)",
                                    "not(/atom:entry/adsm:content/@src)",
                                    "boolean(/atom:entry/atom:content/adsm:properties/ads:ID)",
                                    "count(/atom:entry/atom:content/adsm:properties/*) = 1",

                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content)",
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders'])",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/*)",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:content[not(@src)])",
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties/*)",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            // V1 expand
                            new {
                                QueryStrings = new string[] {
                                    "/Orders(0)?$expand=Customer",
                                    "/GetOrderByIdQueryable?id=0&$expand=Customer"
                                },
                                XPathExprs = new string[] {
                                    "not(/atom:entry/adsm:properties)",
                                    "not(/atom:entry/adsm:content/@src)",
                                    "count(/atom:entry/atom:content/adsm:properties/*) = 3",

                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content)",
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties)",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/*)",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content[not(@src)])",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties[count(*) != 5])",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            // multilevel expand and project, only project 1 property per level
                            new {
                                QueryStrings = new string[] {
                                    "/Orders(0)?$select=ID&$expand=Customer($select=Name),Customer($expand=Orders($select=ID)),Customer($expand=Orders($expand=OrderDetails($select=Quantity)))",
                                    "/GetOrderByIdQueryable?id=0&$select=ID&$expand=Customer($select=Name),Customer($expand=Orders($select=ID)),Customer($expand=Orders($expand=OrderDetails($select=Quantity)))",
                                },
                                XPathExprs = new string[] {
                                    "not(/atom:entry/adsm:properties)",
                                    "not(/atom:entry/adsm:content/@src)",
                                    "boolean(/atom:entry/atom:content/adsm:properties/ads:ID)",
                                    "count(/atom:entry/atom:content/adsm:properties/*) = 1",

                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content)",
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/*)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content[not(@src)])",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties[not(ads:Name)])",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties[count(*) != 1])",

                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/adsm:properties)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content/@src)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content/adsm:properties[not(ads:ID)])",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content/adsm:properties[count(*) != 1])",

                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:link[@title='OrderDetails']/adsm:inline/atom:feed/atom:entry/atom:content/@src)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:link[@title='OrderDetails']/adsm:inline/atom:feed/atom:entry/adsm:properties)",
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:link[@title='OrderDetails']/adsm:inline/atom:feed/atom:entry/atom:content/adsm:properties/ads:Quantity)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:link[@title='OrderDetails']/adsm:inline/atom:feed/atom:entry/atom:content/adsm:properties[count(*) != 1])",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            // multilevel expand and project, only project 0 property per level
                            new {
                                QueryStrings = new string[] {
                                    "/Orders(0)?$expand=Customer($expand=Orders($expand=OrderDetails))",
                                    "/GetOrderByIdQueryable?id=0&$expand=Customer($expand=Orders($expand=OrderDetails))"
                                },
                                XPathExprs = new string[] {
                                    "not(/atom:entry/adsm:properties)",
                                    "not(/atom:entry/adsm:content/@src)",
                                    "count(/atom:entry/atom:content/adsm:properties/*) != 0",

                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content)",
                                    "boolean    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/*)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content[not(@src)])",

                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/adsm:properties)",
                                    "boolean    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content/*)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content/@src)",

                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:link[@title='OrderDetails'])",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:link[@title='OrderDetails']/adsm:inline/atom:entry/atom:content/*)",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            new {
                                QueryStrings = new string[] {
                                    "/Customers?$top=1&$select=Name&$skip=1",
                                    "/GetAllCustomersQueryable?$top=1&$select=Name&$skip=1",
                                },
                                XPathExprs = new string[] {
                                    "not(/atom:feed/atom:entry/atom:content/adsm:properties)",
                                    "count(/atom:feed/atom:entry/atom:content[@src]) = 1",
                                    "count(/atom:feed/atom:entry/adsm:properties/ads:Name) = 1",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            new {
                                QueryStrings = new string[] {
                                    "/Customers?$select=Name,Address&$filter=ID eq 0",
                                    "/GetAllCustomersQueryable?$select=Name,Address&$filter=ID eq 0",
                                },
                                XPathExprs = new string[] {
                                    "not(/atom:feed/atom:entry/atom:content/adsm:properties)",
                                    "count(/atom:feed/atom:entry/atom:content[@src]) = 1",
                                    "count(/atom:feed/atom:entry/adsm:properties/*) = 2",
                                    "boolean(/atom:feed/atom:entry/adsm:properties/ads:Name)",
                                    "boolean(/atom:feed/atom:entry/adsm:properties/ads:Address)",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            new {
                                QueryStrings = new string[] {
                                    "/Orders?$filter=Customer/ID eq 0&$select=ID",
                                    "/GetAllOrdersQueryable?$filter=Customer/ID eq 0&$select=ID"
                                },
                                XPathExprs = new string[] {
                                    "not    (/atom:feed/atom:entry/atom:content/@src)",
                                    "not    (/atom:feed/atom:entry/adsm:properties)",
                                    "boolean(/atom:feed/atom:entry/atom:content/adsm:properties/ads:ID)",
                                    "not    (/atom:feed/atom:entry/atom:content/adsm:properties[count(*) != 1])",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                        };

                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) =>
                        {

                            string entityName = entity.GetType().Name;
                            string entityId = entity is NorthwindModel.Customers ? "'" + ((NorthwindModel.Customers)entity).CustomerID + "'" :
                                              entity is NorthwindModel.Orders ? ((NorthwindModel.Orders)entity).OrderID.ToString() :
                                              string.Empty;

                            return string.Format("W/\"{0}({1})\"", entityName, entityId);
                        };

                        OpenWebDataServiceHelper.ProcessingRequest.Value = new EventHandler<DataServiceProcessingPipelineEventArgs>(BlobDataServicePipelineHandlers.ProcessingRequestHandler);
                        OpenWebDataServiceHelper.ProcessingChangeset.Value = new EventHandler<EventArgs>(BlobDataServicePipelineHandlers.ProcessingChangesetHandler);
                        OpenWebDataServiceHelper.ProcessedChangeset.Value = new EventHandler<EventArgs>(BlobDataServicePipelineHandlers.ProcessedChangesetHandler);
                        OpenWebDataServiceHelper.ProcessedRequest.Value = new EventHandler<DataServiceProcessingPipelineEventArgs>(BlobDataServicePipelineHandlers.ProcessedRequestHandler);
                        OpenWebDataServiceHelper.GetServiceCustomizer.Value = (type) =>
                        {
                            if (type == typeof(IDataServiceStreamProvider))
                            {
                                return new DataServiceStreamProvider();
                            }

                            return null;
                        };

                        foreach (var testCase in testCases)
                        {
                            foreach (string queryString in testCase.QueryStrings)
                            {
                                BLOBSupportTest.ValidateInterceptorOverride = () =>
                                {
                                    InterceptorChecker.ValidateQueryInterceptor(0);
                                    InterceptorChecker.ValidateChangeInterceptor(0);
                                };

                                Assert.IsNull(SendRequest(typeof(CustomRowBasedContext), request, "GET", queryString, null, null, accept, null, null, null, 200));
                                XmlDocument atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                                UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, testCase.XPathExprs);
                            }
                        }
                    }
                });
            }

            [TestCategory("Partition2"), TestMethod, Variation("Tests Projection and Expand on MLEs - Open Type provider")]
            public void BlobProjectionTests_OpenTypeProvider()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Location", new[] { WebServerLocation.InProcess, WebServerLocation.InProcessWcf, WebServerLocation.InProcessStreamedWcf }),
                    new Dimension("Accept", UnitTestsUtil.ResponseFormats));

                TestUtil.RunCombinatorialEngineFail(engine, table =>
                {
                    string accept = (string)table["Accept"];

                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
                    using (CustomRowBasedOpenTypesContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForLocation((WebServerLocation)table["Location"]))
                    {
                        OpenWebDataServiceHelper.EnableBlobServer.Value = true;

                        // Customers are MLEs, Orders are non-MLEs
                        var testCases = new[] {
                            // Make sure when there's no projected properties, we omit the <m:properties /> node
                            // This should hold true for both MLE and non-MLE entities.
                            new {
                                QueryStrings = new string[] {
                                    "/Customers(0)?$select=Orders",
                                    "/GetCustomerByIdQueryable?id=0&$select=Orders",
                                    "/Orders(0)?$select=Customer",
                                    "/GetOrderByIdQueryable?id=0&$select=Customer",
                                },
                                XPathExprs = new string[] {
                                    // Orders(0) is not MLE and must not have @src while Customers(0) must have @src because it is an MLE 
                                    "not(/atom:entry[contains(atom:id, 'Orders(0)')]/atom:content/@src) or boolean(/atom:entry[contains(atom:id, 'Customers(0)')]/atom:content/@src)",
                                    "not(/atom:entry/adsm:properties)",
                                    "not(/atom:entry/atom:content/adsm:properties)",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                }
                            },
                            // Project 1 property
                            new {
                                QueryStrings = new string[] {
                                    "/Customers(0)?$select=Name",
                                    "/GetCustomerByIdQueryable?id=0&$select=Name",
                                },
                                XPathExprs = new string[] {
                                    "boolean(/atom:entry/adsm:properties/ads:Name)",
                                    "boolean(/atom:entry/atom:content/@src)",
                                    "not(/atom:entry/adsm:properties/*[local-name()!='Name'])",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            // Project 2 primitive properties
                            new {
                                QueryStrings = new string[] {
                                    "/Customers(0)?$select=Name,Address",
                                    "/GetCustomerByIdQueryable?id=0&$select=Name,Address",
                                },
                                XPathExprs = new string[] {
                                    "boolean(/atom:entry/adsm:properties/ads:Name)",
                                    "boolean(/atom:entry/adsm:properties/ads:Address)",
                                    "boolean(/atom:entry/atom:content/@src)",
                                    "count(/atom:entry/adsm:properties/*) = 2",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            // Explicitly project all properties
                            new {
                                QueryStrings = new string[] {
                                    "/Customers(0)?$select=*,Orders", 
                                    "/Customers(0)?$select=Address,*", 
                                    "/Customers(0)?$select=*,*",
                                    "/Customers?$filter=ID eq 0&$select=*",
                                    
                                    "/GetCustomerByIdQueryable?id=0&$select=*,Orders", 
                                    "/GetCustomerByIdQueryable?id=0&$select=Address,*", 
                                    "/GetCustomerByIdQueryable?id=0&$select=*,*",
                                    "/GetCustomerByIdQueryable?id=0&$filter=ID eq 0&$select=*",
                                },
                                XPathExprs = new string[] {
                                    "not((/atom:entry | /atom:feed/atom:entry)/atom:content/adsm:properties)",
                                    "count((/atom:entry | /atom:feed/atom:entry)/adsm:properties/*) = 4",
                                    "boolean((/atom:entry | /atom:feed/atom:entry)/atom:content/@src)",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            // Implicitly/Explicitly project all properties of the expanded entity
                            new {
                                QueryStrings = new string[] {
                                    "/Orders(0)?$select=Customer&$expand=Customer",  /*Implicit projection */
                                    "/Orders(0)?$select=Customer&$expand=Customer($select=*)", /*Explicit projection */
                                    "/GetOrderByIdQueryable?id=0&$select=Customer&$expand=Customer",  /*Implicit projection */
                                    "/GetOrderByIdQueryable?id=0&$select=Customer&$expand=Customer($select=*)", /*Explicit projection */
                                },
                                XPathExprs = new string[] {
                                    "not(/atom:entry/atom:content/@src)",
                                    "not(/atom:entry/adsm:properties)",
                                    "not(/atom:entry/atom:content/adsm:properties)",
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/@src)",
                                    "count(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties/*) = 4",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            // Project 1 property, expand an MLE and implicitly/explicitly project all properties  of the expanded MLE 
                            new {
                                QueryStrings = new string[] {
                                    "/Orders(0)?$select=ID,Customer&$expand=Customer",
                                    "/Orders(0)?$select=ID,Customer&$expand=Customer($select=*)",
                                    "/GetOrderByIdQueryable?id=0&$select=ID,Customer&$expand=Customer",
                                    "/GetOrderByIdQueryable?id=0&$select=ID,Customer&$expand=Customer($select=*)",
                                },
                                XPathExprs = new string[] {
                                    "boolean(/atom:entry/atom:content/adsm:properties/ads:ID)",
                                    "count(/atom:entry/atom:content/adsm:properties/*) = 1",
                                    "not(/atom:entry/adsm:properties)",
                                    "not(/atom:entry/atom:content/@src)",
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/@src)",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry[count(adsm:properties/*) != 4])",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag])= count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            // Project 1 property in the expanded MLE
                            new {
                                QueryStrings = new string[] {
                                    "/Orders(0)?$select=ID&$expand=Customer($select=Name)",
                                    "/GetOrderByIdQueryable?id=0&$select=ID,Customer&$expand=Customer($select=Name)"
                                },
                                XPathExprs = new string[] {
                                    "not(/atom:entry/adsm:properties)",
                                    "not(/atom:entry/adsm:content/@src)",
                                    "boolean(/atom:entry/atom:content/adsm:properties/ads:ID)",
                                    "count(/atom:entry/atom:content/adsm:properties/*) = 1",
        
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content)",
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties)",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/*)",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content[not(@src)])",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties[not(ads:Name)])",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties[count(*) != 1])",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            // Project nav property in the expanded MLE
                            new {
                                QueryStrings = new string[] {
                                    "/Orders(0)?$select=ID,Customer&$expand=Customer($expand=Orders)",
                                    "/GetOrderByIdQueryable?id=0&$select=ID,Customer&$expand=Customer($expand=Orders)",
                                },
                                XPathExprs = new string[] {
                                    "not(/atom:entry/adsm:properties)",
                                    "not(/atom:entry/adsm:content/@src)",
                                    "boolean(/atom:entry/atom:content/adsm:properties/ads:ID)",
                                    "count(/atom:entry/atom:content/adsm:properties/*) = 1",

                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content)",
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders'])",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/*)",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:content[not(@src)])",
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties/*)",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            // V1 expand
                            new {
                                QueryStrings = new string[] {
                                    "/Orders(0)?$expand=Customer",
                                    "/GetOrderByIdQueryable?id=0&$expand=Customer"
                                },
                                XPathExprs = new string[] {
                                    "not(/atom:entry/adsm:properties)",
                                    "not(/atom:entry/adsm:content/@src)",
                                    "count(/atom:entry/atom:content/adsm:properties/*) = 2",

                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content)",
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties)",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/*)",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content[not(@src)])",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties[count(*) != 4])",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            // multilevel expand and project, only project 1 property per level
                            new {
                                QueryStrings = new string[] {
                                    "/Orders(0)?$select=ID,Customer&$expand=Customer($select=Name;$expand=Orders($select=ID;$expand=Customer($select=ID)))",
                                    "/GetOrderByIdQueryable?id=0&$select=ID&$expand=Customer($select=Name;$expand=Orders($select=ID;$expand=Customer($select=ID)))"
                                },
                                XPathExprs = new string[] {
                                    "not(/atom:entry/adsm:properties)",
                                    "not(/atom:entry/adsm:content/@src)",
                                    "boolean(/atom:entry/atom:content/adsm:properties/ads:ID)",
                                    "count(/atom:entry/atom:content/adsm:properties/*) = 1",

                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content)",
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/*)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content[not(@src)])",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties[not(ads:Name)])",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties[count(*) != 1])",

                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/adsm:properties)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content/@src)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content/adsm:properties[not(ads:ID)])",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content/adsm:properties[count(*) != 1])",

                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/@src)",
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties)",
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties/ads:ID)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/*)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties[count(*) != 1])",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            // multilevel expand and project, only project 0 property per level
                            new {
                                QueryStrings = new string[] {
                                    "/Orders(0)?$select=Customer&$expand=Customer($select=Orders;$expand=Orders($expand=Customer))",
                                    "/GetOrderByIdQueryable?id=0&$select=Customer&$expand=Customer($select=Orders;$expand=Orders($expand=Customer))",
                                },
                                XPathExprs = new string[] {
                                    "not(/atom:entry/adsm:properties)",
                                    "not(/atom:entry/adsm:content/@src)",
                                    "count(/atom:entry/atom:content/adsm:properties/*) = 0",

                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content)",
                                    "not(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/*)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content[not(@src)])",

                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/adsm:properties)",
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content/*)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content/@src)",

                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:link[@title='Customer'])",
                                    "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/*)",
                                    "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content[not(@src)])",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            new {
                                QueryStrings = new string[] {
                                    "/Customers?$top=1&$select=Name&$skip=1",
                                    "/GetAllCustomersQueryable?$top=1&$select=Name&$skip=1",
                                },
                                XPathExprs = new string[] {
                                    "not(/atom:feed/atom:entry/atom:content/adsm:properties)",
                                    "count(/atom:feed/atom:entry/atom:content[@src]) = 1",
                                    "count(/atom:feed/atom:entry/adsm:properties/ads:Name) = 1",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            new {
                                QueryStrings = new string[] {
                                    "/Customers?$select=Name,Address&$filter=ID eq 0",
                                    "/GetAllCustomersQueryable?$select=Name,Address&$filter=ID eq 0",
                                },
                                XPathExprs = new string[] {
                                    "not(/atom:feed/atom:entry/atom:content/adsm:properties)",
                                    "count(/atom:feed/atom:entry/atom:content[@src]) = 1",
                                    "count(/atom:feed/atom:entry/adsm:properties/*) = 2",
                                    "boolean(/atom:feed/atom:entry/adsm:properties/ads:Name)",
                                    "boolean(/atom:feed/atom:entry/adsm:properties/ads:Address)",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                            new {
                                QueryStrings = new string[] {
                                    "/Orders?$filter=Customer/ID eq 0&$select=ID",
                                    "/GetAllOrdersQueryable?$filter=Customer/ID eq 0&$select=ID"
                                },
                                XPathExprs = new string[] {
                                    "not    (/atom:feed/atom:entry/atom:content/@src)",
                                    "not    (/atom:feed/atom:entry/adsm:properties)",
                                    "boolean(/atom:feed/atom:entry/atom:content/adsm:properties/ads:ID)",
                                    "not    (/atom:feed/atom:entry/atom:content/adsm:properties[count(*) != 1])",

                                    "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag)])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @title='OrderDetails'] and @adsm:etag]) = 0",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"] and @adsm:etag]) = count(//atom:entry[atom:link[@rel='edit' and @href=\"Customers(0)\"]])",
                                    "count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)'] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and @href='Orders(0)']])"
                                },
                            },
                        };

                        DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) =>
                        {

                            string entityName = entity.GetType().Name;
                            string entityId = entity is NorthwindModel.Customers ? "'" + ((NorthwindModel.Customers)entity).CustomerID + "'" :
                                              entity is NorthwindModel.Orders ? ((NorthwindModel.Orders)entity).OrderID.ToString() :
                                              string.Empty;

                            return string.Format("W/\"{0}({1})\"", entityName, entityId);
                        };

                        OpenWebDataServiceHelper.ProcessingRequest.Value = new EventHandler<DataServiceProcessingPipelineEventArgs>(BlobDataServicePipelineHandlers.ProcessingRequestHandler);
                        OpenWebDataServiceHelper.ProcessingChangeset.Value = new EventHandler<EventArgs>(BlobDataServicePipelineHandlers.ProcessingChangesetHandler);
                        OpenWebDataServiceHelper.ProcessedChangeset.Value = new EventHandler<EventArgs>(BlobDataServicePipelineHandlers.ProcessedChangesetHandler);
                        OpenWebDataServiceHelper.ProcessedRequest.Value = new EventHandler<DataServiceProcessingPipelineEventArgs>(BlobDataServicePipelineHandlers.ProcessedRequestHandler);
                        OpenWebDataServiceHelper.GetServiceCustomizer.Value = (type) =>
                        {
                            if (type == typeof(IDataServiceStreamProvider))
                            {
                                return new DataServiceStreamProvider();
                            }

                            return null;
                        };

                        foreach (var testCase in testCases)
                        {
                            foreach (string queryString in testCase.QueryStrings)
                            {
                                BLOBSupportTest.ValidateInterceptorOverride = () =>
                                {
                                    InterceptorChecker.ValidateQueryInterceptor(0);
                                    InterceptorChecker.ValidateChangeInterceptor(0);
                                };

                                Assert.IsNull(SendRequest(typeof(CustomRowBasedOpenTypesContext), request, "GET", queryString, null, null, accept, null, null, null, 200));
                                XmlDocument atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                                UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, testCase.XPathExprs);
                            }
                        }
                    }
                });
            }

            #endregion

            #region Cross Feature - Server Blobs and $orderby

            [TestCategory("Partition1"), TestMethod, Variation("Tests orderby on MLEs, positive cases - EF provider")]
            public void BlobAndOrderby_EFProvider()
            {
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (NorthwindDefaultStreamService.SetupNorthwindWithStreamAndETag(
                    new KeyValuePair<string, string>[] {
                        new KeyValuePair<string, string>("Orders", "true")
                    },
                    new KeyValuePair<string[], string>[] {
                        new KeyValuePair<string[], string>(new string[] {"Orders", "Freight"}, "Fixed")
                    },
                    "BlobSupportTest_BlobAndOrderby_EFProvider"))
                {
                    foreach (WebServerLocation location in new WebServerLocation[] { 
                        WebServerLocation.InProcess, 
                        WebServerLocation.InProcessWcf, 
                        WebServerLocation.InProcessStreamedWcf})
                    {
                        using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
                        {
                            BLOBSupportTest.ValidateInterceptorOverride = () => { };
                            Assert.IsNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "GET", "/Orders?$expand=Customers", null, null, UnitTestsUtil.AtomFormat, null, null, null, 200));
                            XmlDocument ordersAsAtom = UnitTestsUtil.GetResponseAsAtom(request);

                            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                            new Dimension("Accept", UnitTestsUtil.ResponseFormats),
                            new Dimension("QueryString", new string[] { 
                                "/Orders?$orderby=OrderID desc&$top=10",
                                "/Orders?$orderby=Freight,OrderID&$top=10",
                                "/Orders?$orderby=OrderID&$top=10", 
                                "/Orders?$orderby=OrderID&$select=ShipCity&$top=10",
                                "/Orders?$orderby=Customers/CustomerID,OrderID&$top=10",
                                "/Orders?$orderby=Customers/CustomerID,OrderID&$expand=Customers($select=*)&$top=10",
                                "/Orders?$orderby=Customers/CustomerID,OrderID&$expand=Customers($select=CustomerID)&$top=10",
                                "/Orders?$orderby=Customers/CustomerID,OrderID&$expand=Customers($select=CompanyName)&$top=10",
                                "/Orders?$orderby=OrderID desc&$skip=" + (int.MaxValue - 1).ToString(),
                                "/Orders?$orderby=OrderID desc&$top=10",
                            }));

                            TestUtil.RunCombinatorialEngineFail(engine, table =>
                            {
                                DataServiceStreamProvider.SkipValidation = true;
                                string queryString = (string)table["QueryString"];
                                string accept = (string)table["Accept"];

                                SendRequest(typeof(NorthwindDefaultStreamService), request, "GET", queryString, null, null, accept, null, null, null, 200);
                                XmlDocument atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                                Assert.AreEqual("4.0;", request.ResponseHeaders["OData-Version"]);
                                Assert.IsNull(request.ResponseETag);

                                UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, new string[] { 
                                    "boolean(/atom:feed)",
                                    "not(/atom:feed/atom:entry[not(@adsm:etag)])" });

                                VerifyOrder(queryString, ordersAsAtom, atomResponse);
                            });
                        }
                    }
                }
            }

            [TestCategory("Partition2"), TestMethod, Variation("Tests orderby on MLEs, positive cases - reflection provider")]
            public void BlobAndOrderby_ReflectionProvider()
            {
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                using (PhotoDataServiceContext.CreateChangeScope())
                {

                    foreach (WebServerLocation location in new WebServerLocation[] { 
                        WebServerLocation.InProcess, 
                        WebServerLocation.InProcessWcf, 
                        WebServerLocation.InProcessStreamedWcf })
                    {
                        using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
                        {
                            BLOBSupportTest.ValidateInterceptorOverride = () => { };
                            Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items?$expand=ParentFolder", null, null, UnitTestsUtil.AtomFormat, null, null, null, 200));
                            XmlDocument itemsAsAtom = UnitTestsUtil.GetResponseAsAtom(request);

                            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                            new Dimension("Accept", UnitTestsUtil.ResponseFormats),
                            new Dimension("QueryString", new string[] {
                                "/Items?$orderby=ID desc",
                                "/Items?$orderby=Name,ID",
                                "/Items?$orderby=Name,ID&$select=Name",
                                "/Items?$orderby=Name,ID&$select=Description",
                                "/Items?$orderby=ParentFolder/ID desc,ID",
                                "/Items?$orderby=ParentFolder/ID desc,ID&&$expand=ParentFolder($select=*)",
                                "/Items?$orderby=ParentFolder/ID desc,Name desc&$expand=ParentFolder($select=ID)",
                                "/Items?$orderby=ParentFolder/ID desc,Name&$expand=ParentFolder($select=Name)",
                                "/Items?$orderby=ID desc&$skip=" + (int.MaxValue - 1).ToString(),
                                "/Items?$orderby=ID desc&$top=10&$filter=ParentFolder/ID eq 0",
                            }));

                            TestUtil.RunCombinatorialEngineFail(engine, table =>
                            {
                                DataServiceStreamProvider.SkipValidation = true;
                                string queryString = (string)table["QueryString"];
                                string accept = (string)table["Accept"];

                                SendRequest(typeof(PhotoDataService), request, "GET", queryString, null, null, accept, null, null, null, 200);
                                XmlDocument atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                                Assert.AreEqual("4.0;", request.ResponseHeaders["OData-Version"]);
                                Assert.IsNull(request.ResponseETag);

                                UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, new string[] { 
                                    "boolean(/atom:feed)",
                                    "not(/atom:feed/atom:entry[not(@adsm:etag)])" });

                                VerifyOrder(queryString, itemsAsAtom, atomResponse);
                            });
                        }
                    }
                }
            }

            [TestCategory("Partition2"), TestMethod, Variation("Tests orderby on MLEs on IDSP and OpenTypes provider")]
            public void BlobAndOrderBy_IDSPAndOpenTypesProvider()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Location", new[] { WebServerLocation.InProcess, WebServerLocation.InProcessWcf, WebServerLocation.InProcessStreamedWcf }),
                    new Dimension("ContextType", new Type[] { 
                        typeof(CustomRowBasedContext),
                        typeof(CustomRowBasedOpenTypesContext)
                    }),
                    new Dimension("Accept", UnitTestsUtil.ResponseFormats),
                    new Dimension("QueryString", new string[] { 
                        "/Customers?$orderby=Name",
                        "/Customers?$orderby=ID desc",
                        "/Customers?$orderby=Name desc&$select=Name",
                        "/Customers?$orderby=ID desc&$select=Name",
                        "/Customers?$orderby=BestFriend/ID",
                        "/Customers?$orderby=BestFriend/ID&$expand=BestFriend",
                        "/Customers?$orderby=BestFriend/ID desc&$expand=BestFriend&select=BestFriend/*",
                        "/Customers?$orderby=BestFriend/ID&$expand=BestFriend&select=BestFriend/ID",
                        "/Customers?$orderby=BestFriend/ID&$expand=BestFriend&select=BestFriend/Name",
                    }));

                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
                {
                    OpenWebDataServiceHelper.EnableBlobServer.Value = true;

                    OpenWebDataServiceHelper.ProcessingRequest.Value = new EventHandler<DataServiceProcessingPipelineEventArgs>(BlobDataServicePipelineHandlers.ProcessingRequestHandler);
                    OpenWebDataServiceHelper.ProcessingChangeset.Value = new EventHandler<EventArgs>(BlobDataServicePipelineHandlers.ProcessingChangesetHandler);
                    OpenWebDataServiceHelper.ProcessedChangeset.Value = new EventHandler<EventArgs>(BlobDataServicePipelineHandlers.ProcessedChangesetHandler);
                    OpenWebDataServiceHelper.ProcessedRequest.Value = new EventHandler<DataServiceProcessingPipelineEventArgs>(BlobDataServicePipelineHandlers.ProcessedRequestHandler);
                    OpenWebDataServiceHelper.GetServiceCustomizer.Value = (type) =>
                    {
                        if (type == typeof(IDataServiceStreamProvider))
                        {
                            return new DataServiceStreamProvider();
                        }

                        return null;
                    };
                    BLOBSupportTest.ValidateInterceptorOverride = () => { };

                    TestUtil.RunCombinatorialEngineFail(engine, table =>
                    {
                        using (CustomRowBasedContext.CreateChangeScope())
                        using (TestWebRequest request = TestWebRequest.CreateForLocation((WebServerLocation)table["Location"]))
                        {
                            string queryString = (string)table["QueryString"];
                            string accept = (string)table["Accept"];
                            Type contextType = (Type)table["ContextType"];
                            request.RequestMaxVersion = "4.0";

                            Assert.IsNull(SendRequest(contextType, request, "GET", "/Customers?$expand=BestFriend", null, null, UnitTestsUtil.AtomFormat, null, null, null, 200));
                            XmlDocument customersAsAtom = UnitTestsUtil.GetResponseAsAtom(request);

                            SendRequest(contextType, request, "GET", queryString, null, null, accept, null, null, null, 200);
                            XmlDocument atomResponse = UnitTestsUtil.GetResponseAsAtom(request);

                            Assert.AreEqual("4.0;", request.ResponseHeaders["OData-Version"]);
                            Assert.IsNull(request.ResponseETag);

                            UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, new string[] { 
                                    "boolean(/atom:feed)",
                                    "not(/atom:feed/atom:entry[not(@adsm:etag)])" });

                            VerifyOrder(queryString, customersAsAtom, atomResponse);
                        }
                    });
                }
            }

            public void VerifyOrder(string queryString, XmlDocument baseline, XmlDocument response)
            {
                XPathExpression sortingExpression = GetBaselineSortingXPathExpr(queryString);
                XPathNodeIterator baselineIter = baseline.CreateNavigator().Select(sortingExpression);
                XPathNodeIterator responseIter = response.CreateNavigator().Select("/atom:feed/atom:entry", TestUtil.TestNamespaceManager);
                while (responseIter.MoveNext())
                {
                    string responseAtomId = responseIter.Current.SelectSingleNode("atom:id/text()", TestUtil.TestNamespaceManager).Value;

                    while (true)
                    {
                        if (!baselineIter.MoveNext())
                        {
                            Assert.Fail("Elements in the baseline have different order than elements in the response.");
                        }

                        string baselineAtomId = baselineIter.Current.SelectSingleNode("atom:id/text()", TestUtil.TestNamespaceManager).Value;
                        if (baselineAtomId == responseAtomId)
                        {
                            break;
                        }
                    }
                }

                Assert.IsFalse(responseIter.MoveNext(), "Not all nodes have been matched");
            }

            private XPathExpression GetBaselineSortingXPathExpr(string queryString)
            {
                string orderBy = ((queryString.Split('?')[1]).Split('&')).Where(s => s.ToLower().StartsWith("$orderby")).FirstOrDefault();
                Assert.IsNotNull(orderBy, "$orderby query option could not be found");
                orderBy = orderBy.Split('=')[1];
                XPathExpression selectSortedExpr = XPathExpression.Compile("/atom:feed/atom:entry", TestUtil.TestNamespaceManager);

                foreach (string singleOrderBy in orderBy.Split(','))
                {
                    AddSingleSort(selectSortedExpr, singleOrderBy);
                }

                return selectSortedExpr;
            }

            private void AddSingleSort(XPathExpression selectExpr, string orderBy)
            {
                string[] numberSorting = new string[] {
                    "Freight",
                    "OrderID",
                    "ID"
                };

                XmlSortOrder sortOrder = XmlSortOrder.Ascending;

                int spacePos = orderBy.IndexOf(' ');
                if (spacePos >= 0)
                {
                    if (orderBy.EndsWith("desc"))
                    {
                        sortOrder = XmlSortOrder.Descending;
                    }

                    orderBy = orderBy.Substring(0, spacePos);
                }

                StringBuilder sortingExprBuilder = new StringBuilder();
                string[] segments = orderBy.Split('/');
                for (int i = 0; i < segments.Length - 1; i++)
                {
                    sortingExprBuilder.Append("atom:link[@title='").Append(segments[i]).Append("']/adsm:inline/atom:entry/");
                }

                sortingExprBuilder.Append("atom:category/following-sibling::*[local-name()!='link']/descendant::ads:").Append(segments[segments.Length - 1]).Append("[parent::adsm:properties]");

                XmlDataType sortingDataType = numberSorting.Count(s => s == segments[segments.Length - 1]) > 0 ? XmlDataType.Number : XmlDataType.Text;

                selectExpr.AddSort(
                    XPathExpression.Compile(sortingExprBuilder.ToString(), TestUtil.TestNamespaceManager),
                    sortOrder,
                    XmlCaseOrder.None,
                    null,
                    sortingDataType);
            }

            [TestCategory("Partition2"), TestMethod, Variation("Tests $orderby on MLEs, negative test cases")]
            public void BlobAndOrderby_NegativeCases()
            {
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    OpenWebDataServiceHelper.EnableBlobServer.Value = true;

                    OpenWebDataServiceHelper.ProcessingRequest.Value = new EventHandler<DataServiceProcessingPipelineEventArgs>(BlobDataServicePipelineHandlers.ProcessingRequestHandler);
                    OpenWebDataServiceHelper.ProcessingChangeset.Value = new EventHandler<EventArgs>(BlobDataServicePipelineHandlers.ProcessingChangesetHandler);
                    OpenWebDataServiceHelper.ProcessedChangeset.Value = new EventHandler<EventArgs>(BlobDataServicePipelineHandlers.ProcessedChangesetHandler);
                    OpenWebDataServiceHelper.ProcessedRequest.Value = new EventHandler<DataServiceProcessingPipelineEventArgs>(BlobDataServicePipelineHandlers.ProcessedRequestHandler);
                    OpenWebDataServiceHelper.GetServiceCustomizer.Value = (type) =>
                    {
                        if (type == typeof(IDataServiceStreamProvider))
                        {
                            return new DataServiceStreamProvider();
                        }

                        return null;
                    };
                    BLOBSupportTest.ValidateInterceptorOverride = () => { };

                    foreach (var testCase in new[] {
                        new { QueryString = "/Customers?$orderby=$value", Method = "GET" },
                        new { QueryString = "/Customers?$orderby=dummyproperty", Method = "GET" },
                        new { QueryString = "/Customers?$orderby=Icon", Method = "GET" },
                        new { QueryString = "/Customers?$orderby=RelatedItems", Method = "GET" },
                        new { QueryString = "/Customers?$orderby=RelatedItems/ID", Method = "GET" },
                        new { QueryString = "/Customers?$orderby=Name", Method = "POST" },
                        new { QueryString = "/Customers(0)?$orderby=Name", Method = "PUT" },
                        new { QueryString = "/Customers(0)?$orderby=Name", Method = "PATCH" },
                        new { QueryString = "/Customers(0)?$orderby=Name", Method = "DELETE" },
                    })
                    {
                        string body = testCase.Method != "GET" && testCase.Method != "DELETE" ? "Some content" : null;
                        Exception e = SendRequest(typeof(CustomRowBasedContext), request, testCase.Method, testCase.QueryString, null, null, null, null, null, body, 400);
                        Assert.AreEqual("4.0;", request.ResponseHeaders["OData-Version"]);
                        Assert.IsNull(request.ResponseETag);
                    }
                }
            }

            #endregion

            #region Cross feature Server Blobs and $top and $skip

            // Test Cases for EF, IDSP, OpenTypes provider
            private KeyValuePair<string[], int>[] GetTopSkipTestCases(int customersCount)
            {
                return new KeyValuePair<string[], int>[] {
                        new KeyValuePair<string[], int>(
                            new string[]{
                                "/Customers?$top=0",
                                "/Customers?$top=0&skip=0",
                                "/Customers?$top=0&skip=" + customersCount.ToString(),
                                "/Customers?$top=0&skip=" + (customersCount + 10).ToString(),
                                "/Customers?$skip=" + customersCount.ToString() + "&$top=10",
                                "/Customers?$skip=" + (customersCount + 10).ToString() + "&$top=10",
                                "/Customers?$skip=" + int.MaxValue.ToString(),
                                "/Customers?$top=" + int.MaxValue.ToString() + "&$skip=" + int.MaxValue.ToString()
                            }, 0),
                        new KeyValuePair<string[], int>(
                            new string[]{
                                "/Customers?$skip=0",
                                "/Customers?$top=" + customersCount.ToString(),
                                "/Customers?$top=" + (customersCount + 10).ToString(),
                                "/Customers?$top=" + int.MaxValue.ToString(),
                                "/Customers?$skip=0&$top=" + customersCount.ToString(),
                                "/Customers?$skip=0&$top=" + (customersCount + 10).ToString(),
                                "/Customers?$skip=0&$top=" + int.MaxValue.ToString(),
                            }, customersCount),

                        new KeyValuePair<string[], int>(
                            new string[]{
                                "/Customers?$top=1"
                            }, 1),

                        new KeyValuePair<string[], int>(
                            new string[]{
                                "/Customers?$top=" + ((int)(customersCount / 3)).ToString()
                            }, (int)(customersCount / 3)),

                        new KeyValuePair<string[], int>(
                            new string[]{
                                "/Customers?$skip=1"
                            }, customersCount - 1),

                        new KeyValuePair<string[], int>(
                            new string[]{
                                "/Customers?$skip=" + ((int)(customersCount / 3)).ToString()
                            }, customersCount - (int)(customersCount / 3)),

                        new KeyValuePair<string[], int>(
                            new string[]{
                                "/Customers?$skip=" + (customersCount - 1).ToString() + "&$top=" + (customersCount >> 1).ToString()
                            }, 1),
                        };
            }

            [TestCategory("Partition2"), TestMethod, Variation("Tests top and skip on MLEs - EF provider")]
            public void BlobAndTopSkip_EFProvider()
            {
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (NorthwindDefaultStreamService.SetupNorthwindWithStreamAndETag(
                    new KeyValuePair<string, string>[] {
                        new KeyValuePair<string, string>("Customers", "true")
                    },
                    new KeyValuePair<string[], string>[] {
                        new KeyValuePair<string[], string>(new string[] {"Customers", "Phone"}, "Fixed")
                    },
                    "BlobSupportTest_BlobAndTopSkip_EFProvider"))
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    Assert.IsNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "GET", "/Customers", null, null, UnitTestsUtil.AtomFormat, null, null, null, 200));
                    XmlDocument customersAsAtom = UnitTestsUtil.GetResponseAsAtom(request);

                    int customersCount = customersAsAtom.SelectNodes("/atom:feed/atom:entry[atom:link[@rel = 'edit' and @title = 'Customers']]", TestUtil.TestNamespaceManager).Count;
                    Assert.IsTrue(customersCount >= 0);

                    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                        new Dimension("TestCases", GetTopSkipTestCases(customersCount)));

                    TestUtil.RunCombinatorialEngineFail(engine, table =>
                    {
                        string[] queryStrings = ((KeyValuePair<string[], int>)table["TestCases"]).Key;
                        int expectedCount = ((KeyValuePair<string[], int>)table["TestCases"]).Value;

                        foreach (string queryString in queryStrings)
                        {
                            Assert.IsNull(SendRequest(typeof(NorthwindDefaultStreamService), request, "GET", queryString, null, null, "application/atom+xml,application/xml", null, null, null, 200));

                            Assert.AreEqual("4.0;", request.ResponseVersion);
                            Assert.IsNull(request.ResponseETag);

                            XmlDocument atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                            // Verify we really received AtomPub feed
                            // Verify embedded all atom entries have etags
                            UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, new string[] { 
                                "boolean(/atom:feed)",
                                "not(/atom:feed/atom:entry[not(@adsm:etag)])",
                            });

                            // Verify Etag values. This is equivalent of:
                            // "not(/atom:feed/atom:entry[not(contains(@adsm:etag, adsm:properties/ads:Phone))])",
                            // The issue is that etag is UrlEncoded but the property isn't. It's not to do such a 
                            // conversion in a single XPath query.
                            foreach (XmlNode entryNode in atomResponse.SelectNodes("/atom:feed/atom:entry", TestUtil.TestNamespaceManager))
                            {
                                string eTag = entryNode.Attributes[0].Value;
                                string phone = "W/\"'" + Uri.EscapeDataString(entryNode.SelectSingleNode("adsm:properties/ads:Phone", TestUtil.TestNamespaceManager).InnerText) + "'\"";
                                Assert.AreEqual(eTag, phone, "etag-phone");
                            }

                            UnitTestsUtil.VerifyXPathResultCount(atomResponse, expectedCount, new string[] { "/atom:feed/atom:entry[atom:link[@rel = 'edit' and @title = 'Customers']]" });

                            ValidateResponseNodes(queryString, customersAsAtom, atomResponse);
                        }
                    });
                }
            }

            [TestCategory("Partition2"), TestMethod, Variation("Tests top and skip on MLEs - IDSP and OpenTypes provider")]
            public void BlobAndTopSkip_IDSPAndOpenTypesProvider()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Location", new[] { WebServerLocation.InProcess, WebServerLocation.InProcessWcf, WebServerLocation.InProcessStreamedWcf }),
                    new Dimension("ContextType", new Type[] { 
                        typeof(CustomRowBasedContext),
                        typeof(CustomRowBasedOpenTypesContext)
                    }),
                    new Dimension("Accept", UnitTestsUtil.ResponseFormats));

                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
                {
                    OpenWebDataServiceHelper.EnableBlobServer.Value = true;

                    OpenWebDataServiceHelper.ProcessingRequest.Value = new EventHandler<DataServiceProcessingPipelineEventArgs>(BlobDataServicePipelineHandlers.ProcessingRequestHandler);
                    OpenWebDataServiceHelper.ProcessingChangeset.Value = new EventHandler<EventArgs>(BlobDataServicePipelineHandlers.ProcessingChangesetHandler);
                    OpenWebDataServiceHelper.ProcessedChangeset.Value = new EventHandler<EventArgs>(BlobDataServicePipelineHandlers.ProcessedChangesetHandler);
                    OpenWebDataServiceHelper.ProcessedRequest.Value = new EventHandler<DataServiceProcessingPipelineEventArgs>(BlobDataServicePipelineHandlers.ProcessedRequestHandler);
                    OpenWebDataServiceHelper.GetServiceCustomizer.Value = (type) =>
                    {
                        if (type == typeof(IDataServiceStreamProvider))
                        {
                            return new DataServiceStreamProvider();
                        }

                        return null;
                    };
                    BLOBSupportTest.ValidateInterceptorOverride = () => { };

                    TestUtil.RunCombinatorialEngineFail(engine, table =>
                    {
                        using (CustomRowBasedContext.CreateChangeScope())
                        using (TestWebRequest request = TestWebRequest.CreateForLocation((WebServerLocation)table["Location"]))
                        {
                            string accept = (string)table["Accept"];
                            Type contextType = (Type)table["ContextType"];
                            request.RequestMaxVersion = "4.0";

                            Assert.IsNull(SendRequest(contextType, request, "GET", "/Customers", null, null, UnitTestsUtil.AtomFormat, null, null, null, 200));
                            XmlDocument customersAsAtom = UnitTestsUtil.GetResponseAsAtom(request);

                            int customersCount = customersAsAtom.SelectNodes("/atom:feed/atom:entry", TestUtil.TestNamespaceManager).Count;
                            Assert.IsTrue(customersCount >= 0);

                            foreach (KeyValuePair<string[], int> testCase in GetTopSkipTestCases(customersCount))
                            {
                                foreach (string queryString in testCase.Key)
                                {
                                    Assert.IsNull(SendRequest(contextType, request, "GET", queryString, null, null, accept, null, null, null, 200));

                                    Assert.AreEqual("4.0;", request.ResponseHeaders["OData-Version"]);
                                    Assert.IsNull(request.ResponseETag);

                                    XmlDocument atomResponse = UnitTestsUtil.GetResponseAsAtom(request);

                                    UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, new string[] { 
                                    "boolean(/atom:feed)",
                                    "not(/atom:feed/atom:entry[not(@adsm:etag)])" });

                                    UnitTestsUtil.VerifyXPathResultCount(atomResponse, testCase.Value /*expected count*/, new string[] { "/atom:feed/atom:entry" });

                                    ValidateResponseNodes(queryString, customersAsAtom, atomResponse);
                                }
                            }
                        }
                    });
                }
            }

            [TestCategory("Partition2"), TestMethod, Variation("Tests orderby on MLEs, positive cases - reflection provider")]
            public void BlobAndTopSkip_ReflectionProvider()
            {
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                using (PhotoDataServiceContext.CreateChangeScope())
                {
                    foreach (WebServerLocation location in new WebServerLocation[] { 
                        WebServerLocation.InProcess,
                        WebServerLocation.InProcessWcf, 
                        WebServerLocation.InProcessStreamedWcf })
                    {
                        using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
                        {
                            BLOBSupportTest.ValidateInterceptorOverride = () => { };
                            Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", "/Items", null, null, UnitTestsUtil.AtomFormat, null, null, null, 200));
                            XmlDocument itemsAsAtom = UnitTestsUtil.GetResponseAsAtom(request);

                            int itemsCount = itemsAsAtom.SelectNodes("/atom:feed/atom:entry", TestUtil.TestNamespaceManager).Count;
                            Assert.IsTrue(itemsCount >= 0);

                            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                            new Dimension("Accept", UnitTestsUtil.ResponseFormats),
                            new Dimension("TestCases", new KeyValuePair<string[], int>[] {
                                new KeyValuePair<string[], int>(
                                    new string[]{
                                        "/Items?$top=0",
                                        "/Items?$top=0&skip=0",
                                        "/Items?$top=0&skip=" + itemsCount.ToString(),
                                        "/Items?$top=0&skip=" + (itemsCount + 10).ToString(),
                                        "/Items?$skip=" + itemsCount.ToString() + "&$top=10",
                                        "/Items?$skip=" + (itemsCount + 10).ToString() + "&$top=10",
                                        "/Items?$skip=" + int.MaxValue.ToString(),
                                        "/Items?$top=" + int.MaxValue.ToString() + "&$skip=" + int.MaxValue.ToString()
                                    }, 0),
                                new KeyValuePair<string[], int>(
                                    new string[]{
                                        "/Items?$skip=0",
                                        "/Items?$top=" + itemsCount.ToString(),
                                        "/Items?$top=" + (itemsCount + 10).ToString(),
                                        "/Items?$top=" + int.MaxValue.ToString(),
                                        "/Items?$skip=0&$top=" + itemsCount.ToString(),
                                        "/Items?$skip=0&$top=" + (itemsCount + 10).ToString(),
                                        "/Items?$skip=0&$top=" + int.MaxValue.ToString(),
                                    }, itemsCount),

                                new KeyValuePair<string[], int>(
                                    new string[]{
                                        "/Items?$top=1"
                                    }, 1),

                                new KeyValuePair<string[], int>(
                                    new string[]{
                                        "/Items?$top=" + ((int)(itemsCount / 3)).ToString()
                                    }, (int)(itemsCount / 3)),

                                new KeyValuePair<string[], int>(
                                    new string[]{
                                        "/Items?$skip=1"
                                    }, itemsCount - 1),

                                new KeyValuePair<string[], int>(
                                    new string[]{
                                        "/Items?$skip=" + ((int)(itemsCount / 3)).ToString()
                                    }, itemsCount - (int)(itemsCount / 3)),

                                new KeyValuePair<string[], int>(
                                    new string[]{
                                        "/Items?$skip=" + (itemsCount - 1).ToString() + "&$top=" + (itemsCount >> 1).ToString()
                                    }, 1),
                            }));

                            TestUtil.RunCombinatorialEngineFail(engine, table =>
                            {
                                DataServiceStreamProvider.SkipValidation = true;
                                string accept = (string)table["Accept"];
                                KeyValuePair<string[], int> testCase = (KeyValuePair<string[], int>)table["TestCases"];

                                foreach (string queryString in testCase.Key)
                                {
                                    Assert.IsNull(SendRequest(typeof(PhotoDataService), request, "GET", queryString, null, null, accept, null, null, null, 200));

                                    XmlDocument atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                                    Assert.AreEqual("4.0;", request.ResponseHeaders["OData-Version"]);

                                    Assert.IsNull(request.ResponseETag);

                                    UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, new string[] { 
                                    "boolean(/atom:feed)",
                                    "not(/atom:feed/atom:entry[not(@adsm:etag)])" });

                                    UnitTestsUtil.VerifyXPathResultCount(atomResponse, testCase.Value /*expected count*/, new string[] { "/atom:feed/atom:entry" });

                                    ValidateResponseNodes(queryString, itemsAsAtom, atomResponse);
                                }
                            });
                        }
                    }
                }
            }

            private void ValidateResponseNodes(string queryString, XmlDocument customersAsAtom, XmlDocument atomResponse)
            {
                int? skip = null, top = null;
                int position = -1;

                position = queryString.IndexOf("?");
                Assert.IsTrue(position >= 0, "Missing query options");
                string[] queryOptions = queryString.Substring(position + 1).Split('&');
                foreach (string s in queryOptions)
                {
                    if (s.StartsWith("$skip"))
                    {
                        skip = int.Parse(s.Split('=')[1]);
                    }
                    else if (s.StartsWith("$top"))
                    {
                        top = int.Parse(s.Split('=')[1]);
                    }
                }

                if (skip == null)
                {
                    skip = 0;

                    if (top == null)
                    {
                        Assert.Fail("Neither $skip nor $top query option found.");
                    }
                }

                string xpath = string.Format("/atom:feed/atom:entry[position() > {0} and position() <= {1}]", skip, top != null ? skip + top : int.MaxValue);
                XmlNodeList atomCustomersNodeList = customersAsAtom.SelectNodes(xpath, TestUtil.TestNamespaceManager);

                XmlNodeList responseCustomersNodeList = atomResponse.SelectNodes("/atom:feed/atom:entry", TestUtil.TestNamespaceManager);

                Assert.AreEqual(atomCustomersNodeList.Count, responseCustomersNodeList.Count);

                for (int nodeIdx = 0; nodeIdx < atomCustomersNodeList.Count; nodeIdx++)
                {
                    string atomCustomersId = atomCustomersNodeList[nodeIdx].SelectSingleNode("atom:id", TestUtil.TestNamespaceManager).InnerText;
                    string responseCustomersId = responseCustomersNodeList[nodeIdx].SelectSingleNode("atom:id", TestUtil.TestNamespaceManager).InnerText;
                    Assert.AreEqual(atomCustomersId, responseCustomersId, "IDs don't match.");
                }
            }

            [TestCategory("Partition2"), TestMethod, Variation("Tests Top and Skip on MLEs, negative cases")]
            public void BlobTopSkipNegative()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Format", UnitTestsUtil.ResponseFormats),
                    new Dimension("QueryString", new string[] { 
                        "/Customers?$top=-3",
                        "/Customers?$skip=-1",
                        "/Customers?$top=-4&$skip=5",
                        "/Customers?$top=5&$skip=-2",
                        "/Customers?$top=abc",
                        "/Customers?$skip=xyz",
                        "/Customers?$top=1.3",
                        "/Customers?$skip=2.5",
                    }));

                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (NorthwindDefaultStreamService.SetupNorthwindWithStreamAndETag(
                    new KeyValuePair<string, string>[] {
                        new KeyValuePair<string, string>("Customers", "true")
                    },
                    new KeyValuePair<string[], string>[] {
                        new KeyValuePair<string[], string>(new string[] {"Customers", "Phone"}, "Fixed")
                    },
                    "BlobSupportTest_BlobAndTopSkip_EFProvider"))
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    TestUtil.RunCombinatorialEngineFail(engine, table =>
                    {
                        string format = (string)table["Format"];
                        string queryString = (string)table["QueryString"];

                        // Note: 500 is not the desired behavior. See: $top and $skip not checked for negative values
                        // The current behavior is baselined here.
                        int expectedStatusCode = queryString.Contains("=-") ? 500 : 400;
                        Exception e = SendRequest(typeof(NorthwindDefaultStreamService), request, "GET", queryString, null, null, format, null, null, null, expectedStatusCode);
                        Assert.IsNotNull(e);
                        if (expectedStatusCode == 500)
                        {
                            Assert.IsInstanceOfType(e.InnerException, typeof(ArgumentException));
                            Assert.IsTrue(e.InnerException.Message.Contains("must have a non-negative value.\r\nParameter name:"));
                        }
                        else
                        {
                            Assert.IsInstanceOfType(e.InnerException, typeof(DataServiceException));
                            Assert.IsTrue(e.InnerException.Message.StartsWith("Incorrect format for $"));
                        }
                        Assert.AreEqual("4.0;", request.ResponseHeaders["OData-Version"]);
                        Assert.IsNull(request.ResponseETag);
                    });
            }

            #endregion

            #region Blob Client Insert Tests

            [TestCategory("Partition2"), TestMethod, Variation("Blob client insert tests -- in memory provider")]
            public void BlobClientInsertTests()
            {
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(PhotoDataService)))
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                using (PhotoDataServiceContext.CreateChangeScope())
                {
                    PhotoDataService.EnablePipelineVerification = false;
                    request.Accept = UnitTestsUtil.AtomFormat;
                    request.ServiceType = typeof(PhotoDataService);
                    request.StartService();

                    DataServiceContext ctx = new DataServiceContext(new Uri(request.BaseUri));
                    ctx.EnableAtom = true;
                    ctx.Format.UseAtom();
                    ctx.ResolveName = type =>
                    {
                        if (type == typeof(Item))
                        {
                            return typeof(Item).FullName;
                        }
                        else if (type == typeof(Photo))
                        {
                            return typeof(Photo).FullName;
                        }

                        return null;
                    };
                    ctx.SendingRequest2 += new EventHandler<SendingRequest2EventArgs>(ctx_SendingRequest);

                    Dictionary<int, string> myStreamETagLookup = new Dictionary<int, string>();

                    Func<int, string> MyGetStreamETag = (id) =>
                    {
                        if (!myStreamETagLookup.ContainsKey(id))
                        {
                            myStreamETagLookup[id] = "Some Etag" + id.ToString();
                        }

                        return "\"" + myStreamETagLookup[id] + "\"";
                    };

                    Func<int, string> MyGetWriteStreamETag = (id) =>
                    {
                        if (myStreamETagLookup.ContainsKey(id))
                        {
                            myStreamETagLookup[id] += ".";
                        }
                        else
                        {
                            return MyGetStreamETag(id);
                        }

                        return "\"" + myStreamETagLookup[id] + "\"";
                    };

                    DataServiceStreamProvider.GetStreamETagOverride = (entity, operationContext) =>
                    {
                        return MyGetStreamETag(((Item)entity).ID);
                    };

                    DataServiceStreamProvider.GetPostWriteStreamETagOverride = (entity, operationContext) =>
                    {
                        return MyGetWriteStreamETag(((Item)entity).ID);
                    };

                    Item p1 = ctx.Execute<Photo>(new Uri("/Items(1)", UriKind.Relative)).Single();
                    Item p2 = ctx.Execute<Photo>(new Uri("/Items(2)", UriKind.Relative)).Single();
                    Item i0 = ctx.Execute<Item>(new Uri("/Items(0)", UriKind.Relative)).Single();

                    string p1ETag = ctx.GetEntityDescriptor(p1).ETag;
                    string p1StreamETag = ctx.GetEntityDescriptor(p1).StreamETag;
                    string p2ETag = ctx.GetEntityDescriptor(p2).ETag;
                    string p2StreamETag = ctx.GetEntityDescriptor(p2).StreamETag;
                    string i0ETag = ctx.GetEntityDescriptor(i0).ETag;

                    byte[] buffer = new byte[20];
                    FillBuffer(buffer);

                    ///////////////
                    // Photo 1
                    DataServiceRequestArgs arg1 = new DataServiceRequestArgs() { ContentType = DataServiceStreamProvider.GetContentType(p1) };
                    ctx.SetSaveStream(p1, new MemoryStream(buffer), true, arg1);

                    ///////////////
                    // Photo 2
                    DataServiceRequestArgs arg2 = new DataServiceRequestArgs() { ContentType = DataServiceStreamProvider.GetContentType(p2) };
                    ctx.SetSaveStream(p2, new MemoryStream(buffer), true, arg2);
                    p2.Description = "Photo2 Updated Description";
                    ctx.UpdateObject(p2);

                    ///////////////
                    // Photo 333
                    Photo p = new Photo() { ID = 333, Name = "Photo 333", Rating = 1, ThumbNail = new byte[] { 1, 2, 3 } };
                    ctx.AddObject("Items", p);
                    DataServiceRequestArgs args = new DataServiceRequestArgs()
                    {
                        ContentType = DataServiceStreamProvider.GetContentType(p),
                        Slug = "333"
                    };
                    args.Headers["CustomRequestHeader_ItemType"] = typeof(Photo).FullName;
                    ctx.SetSaveStream(p, new MemoryStream(buffer), true, args);

                    ///////////////
                    // Item 0
                    i0.Description = "Item0 Updated Description";
                    ctx.UpdateObject(i0);

                    DataServiceResponse response = ctx.SaveChanges();

                    ///////////////
                    // Validation
                    Assert.AreEqual(6, response.Count());

                    EntityDescriptor descriptor = ctx.GetEntityDescriptor(p1);
                    Assert.IsTrue(descriptor.ETag.StartsWith("W/\""));
                    Assert.IsTrue(descriptor.StreamETag.StartsWith("\"") && descriptor.StreamETag.EndsWith("\""));
                    Assert.AreEqual(p1ETag, descriptor.ETag);
                    Assert.AreNotEqual(descriptor.StreamETag, descriptor.ETag);
                    Assert.AreNotEqual(p1StreamETag, descriptor.StreamETag);
                    Assert.IsNotNull(descriptor.EditStreamUri);
                    Assert.IsNotNull(descriptor.ReadStreamUri);

                    descriptor = ctx.GetEntityDescriptor(p2);
                    Assert.IsTrue(descriptor.ETag.StartsWith("W/\""));
                    Assert.IsTrue(descriptor.StreamETag.StartsWith("\"") && descriptor.StreamETag.EndsWith("\""));
                    Assert.AreNotEqual(p2ETag, descriptor.ETag);
                    Assert.AreNotEqual(descriptor.StreamETag, descriptor.ETag);
                    Assert.AreNotEqual(p2StreamETag, descriptor.StreamETag);
                    Assert.IsNotNull(descriptor.EditStreamUri);
                    Assert.IsNotNull(descriptor.ReadStreamUri);

                    descriptor = ctx.GetEntityDescriptor(p);
                    Assert.IsTrue(descriptor.ETag.StartsWith("W/\""));
                    Assert.IsTrue(descriptor.StreamETag.StartsWith("\"") && descriptor.StreamETag.EndsWith("\""));
                    Assert.IsNotNull(descriptor.EditStreamUri);
                    Assert.IsNotNull(descriptor.ReadStreamUri);

                    descriptor = ctx.GetEntityDescriptor(i0);
                    Assert.IsTrue(descriptor.ETag.StartsWith("W/\""));
                    Assert.IsNull(descriptor.StreamETag);
                    Assert.IsNull(descriptor.EditStreamUri);
                    Assert.IsNull(descriptor.ReadStreamUri);
                }
            }

            #endregion

            #region Cross Feature BlobServer and SDP Client

            [TestCategory("Partition2"), TestMethod, Variation("HasStream attribute mismatch in server/client types")]
            public void BlobServerClientMismatchHasStreamTest()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("ServerContextTypes", new[] { typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("EnableBlobServer", new[] { true, false }),
                    new Dimension("ClientType", new[] { typeof(MyCustomer), typeof(MyMLECustomer) }));

                TestUtil.RunCombinatorialEngineFail(engine, table =>
                {
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
                    using (CustomRowBasedOpenTypesContext.CreateChangeScope())
                    using (CustomRowBasedContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcessStreamedWcf())
                    {
                        OpenWebDataServiceHelper.EnableBlobServer.Value = (bool)table["EnableBlobServer"];
                        OpenWebDataServiceHelper.GetServiceCustomizer.Value = (type) =>
                        {
                            if (type == typeof(IDataServiceStreamProvider))
                            {
                                return new DataServiceStreamProvider();
                            }

                            return null;
                        };
                        OpenWebDataServiceHelper.MaxProtocolVersion.Value = ODataProtocolVersion.V4;
                        OpenWebDataServiceHelper.ForceVerboseErrors = true;

                        request.Accept = UnitTestsUtil.AtomFormat;
                        request.DataServiceType = (Type)table["ServerContextTypes"];
                        request.StartService();

                        DataServiceContext ctx = new DataServiceContext(new Uri(request.BaseUri));
                        ctx.EnableAtom = true;
                        ctx.Format.UseAtom();
                        ctx.IgnoreMissingProperties = true;
                        ctx.SendingRequest2 += new EventHandler<SendingRequest2EventArgs>(ctx_SendingRequest);
                        ctx.ResolveName = (type) =>
                        {
                            if (type == typeof(Address))
                            {
                                return typeof(Address).FullName;
                            }
                            else if (type == typeof(MyCustomer) || type == typeof(MyMLECustomer))
                            {
                                return typeof(Customer).FullName;
                            }
                            else if (type == typeof(MyOrder))
                            {
                                return typeof(Order).FullName;
                            }

                            return null;
                        };

                        MyCustomer c1 = ctx.Execute<MyCustomer>(new Uri("/Customers(0)", UriKind.Relative)).Single();
                        string etag = ctx.GetEntityDescriptor(c1).ETag;
                        // Clear type info and MLE info from server
                        ctx.Detach(c1);

                        Type clientType = (Type)table["ClientType"];
                        MyCustomer c = (MyCustomer)Activator.CreateInstance(clientType);
                        c.ID = 0;
                        c.Name = "bob";
                        ctx.AttachTo("Customers", c, etag);
                        ctx.UpdateObject(c);
                        Exception e = TestUtil.RunCatching(() => ctx.SaveChanges());
                        TestUtil.AssertExceptionExpected(
                            e,
                            OpenWebDataServiceHelper.EnableBlobServer && clientType == typeof(MyCustomer),
                            !OpenWebDataServiceHelper.EnableBlobServer && clientType == typeof(MyMLECustomer));
                        if (e != null)
                        {
                            Assert.IsInstanceOfType(e.InnerException, typeof(DataServiceClientException));
                            Assert.AreEqual(400, ((DataServiceClientException)e.InnerException).StatusCode);
                            string expectedMsg = String.Empty;
                            if (OpenWebDataServiceHelper.EnableBlobServer && clientType == typeof(MyCustomer))
                            {
                                expectedMsg = String.Format(ODataLibResourceUtil.GetString("ValidationUtils_EntryWithoutMediaResourceAndMLEType"), CustomRowBasedContext.CustomerFullName);
                            }
                            else if (!OpenWebDataServiceHelper.EnableBlobServer && clientType == typeof(MyMLECustomer))
                            {
                                expectedMsg = String.Format(ODataLibResourceUtil.GetString("ValidationUtils_EntryWithMediaResourceAndNonMLEType"), CustomRowBasedContext.CustomerFullName);
                            }
                            else
                            {
                                Assert.Fail("Shouldn't be here...");
                            }

                            expectedMsg = expectedMsg.Replace("<", "&lt;").Replace(">", "&gt;");
                            TestUtil.AssertContains(((DataServiceClientException)e.InnerException).Message, expectedMsg);
                        }
                    }
                });
            }

            private class ContinuationList<T> : List<T>
            {
                public DataServiceQueryContinuation<T> Continuation { get; set; }
            }

            private class MyCustomer
            {
                public int ID { get; set; }
                public Address Address { get; set; }
                public string Name { get; set; }
                public ContinuationList<MyOrder> Orders { get; set; }
                public MyCustomer BestFriend { get; set; }
                public MyCustomer()
                {
                    this.Address = new Address();
                    this.Address.StreetAddress = "Line1";
                    this.Address.City = "Redmond";
                    this.Address.State = "WA";
                    this.Address.PostalCode = "98052";
                    this.Orders = new ContinuationList<MyOrder>();
                }
            }

            [HasStream]
            private class MyMLECustomer : MyCustomer { }

            private class MyOrder
            {
                public int ID { get; set; }
                public double DollarAmount { get; set; }
                public MyCustomer Customer { get; set; }
            }

            [TestCategory("Partition2"), TestMethod, Variation("Blob Server X SDP client")]
            public void BlobServerXSDPClientTest()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("ServerContextTypes", new[] { typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }));

                TestUtil.RunCombinatorialEngineFail(engine, table =>
                {
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(BLOBSupportTest)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceStreamProvider)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
                    using (CustomRowBasedOpenTypesContext.CreateChangeScope())
                    using (CustomRowBasedContext.CreateChangeScope())
                    using (TestUtil.MetadataCacheCleaner())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                    {
                        int pageSize = 1;

                        OpenWebDataServiceHelper.EnableBlobServer.Value = true;
                        OpenWebDataServiceHelper.GetServiceCustomizer.Value = (type) =>
                        {
                            if (type == typeof(IDataServiceStreamProvider))
                            {
                                return new DataServiceStreamProvider();
                            }

                            return null;
                        };
                        OpenWebDataServiceHelper.MaxProtocolVersion.Value = ODataProtocolVersion.V4;
                        OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) => config.SetEntitySetPageSize("*", pageSize);

                        request.Accept = UnitTestsUtil.AtomFormat;
                        request.DataServiceType = (Type)table["ServerContextTypes"];
                        request.StartService();

                        DataServiceContext ctx = new DataServiceContext(new Uri(request.BaseUri));
                        ctx.EnableAtom = true;
                        ctx.Format.UseAtom();
                        ctx.IgnoreMissingProperties = true;
                        ctx.SendingRequest2 += new EventHandler<SendingRequest2EventArgs>(ctx_SendingRequest);
                        if (request.DataServiceType == typeof(CustomRowBasedContext))
                        {
                            ctx.ResolveName = null;
                        }
                        else
                        {
                            ctx.ResolveName = (type) =>
                            {
                                if (type == typeof(Address))
                                {
                                    return typeof(Address).FullName;
                                }

                                return null;
                            };
                        }

                        var dsq = ctx.CreateQuery<MyMLECustomer>("Customers").Expand("Orders").Execute() as QueryOperationResponse<MyMLECustomer>;

                        int totalCustomerCount = 0;
                        DataServiceQueryContinuation<MyMLECustomer> continuation1 = null;
                        do
                        {
                            int perPageCustomerCount = 0;
                            foreach (MyMLECustomer c in dsq)
                            {
                                Assert.AreEqual(c.Name, "Customer " + c.ID);
                                c.Name = c.Name + " Updated";
                                ctx.UpdateObject(c);
                                if (request.DataServiceType == typeof(CustomRowBasedContext))
                                {
                                    Assert.AreEqual(CustomRowBasedContext.customers.Single(sc => sc.ID == c.ID).TypeName, ctx.GetEntityDescriptor(c).ServerTypeName);
                                }
                                else
                                {
                                    Assert.AreEqual(CustomRowBasedOpenTypesContext.customers.Single(sc => (int)sc.Properties["ID"] == c.ID).TypeName, ctx.GetEntityDescriptor(c).ServerTypeName);
                                }

                                Assert.IsNotNull(ctx.GetEntityDescriptor(c).EditStreamUri);

                                perPageCustomerCount++;

                                int populatedOrderCount = c.Orders.Count;
                                Assert.IsTrue(c.Orders.Count <= pageSize);
                                while (c.Orders.Continuation != null)
                                {
                                    ctx.LoadProperty(c, "Orders", c.Orders.Continuation);
                                    Assert.IsTrue(c.Orders.Count - populatedOrderCount <= pageSize);
                                    populatedOrderCount = c.Orders.Count;
                                }

                                if (request.DataServiceType == typeof(CustomRowBasedContext))
                                {
                                    Assert.AreEqual(((IEnumerable<RowEntityTypeWithIDAsKey>)CustomRowBasedContext.customers.Single(sc => sc.ID == c.ID).Properties["Orders"]).Count(), c.Orders.Count);
                                }
                                else
                                {
                                    Assert.AreEqual(((IEnumerable<RowComplexType>)CustomRowBasedOpenTypesContext.customers.Single(sc => (int)sc.Properties["ID"] == c.ID).Properties["Orders"]).Count(), c.Orders.Count);
                                }

                                foreach (MyOrder o in c.Orders)
                                {
                                    o.DollarAmount = 9999;
                                    ctx.UpdateObject(o);

                                    if (request.DataServiceType == typeof(CustomRowBasedContext))
                                    {
                                        Assert.AreEqual(CustomRowBasedContext.orders.Single(so => so.ID == o.ID).TypeName, ctx.GetEntityDescriptor(o).ServerTypeName);
                                    }
                                    else
                                    {
                                        Assert.AreEqual(CustomRowBasedOpenTypesContext.orders.Single(so => (int)so.Properties["ID"] == o.ID).TypeName, ctx.GetEntityDescriptor(o).ServerTypeName);
                                    }

                                    Assert.IsNull(ctx.GetEntityDescriptor(o).EditStreamUri);
                                }
                            }

                            Assert.IsTrue(perPageCustomerCount <= pageSize);
                            totalCustomerCount += perPageCustomerCount;
                            perPageCustomerCount = 0;
                        } while ((continuation1 = dsq.GetContinuation()) != null && (dsq = ctx.Execute<MyMLECustomer>(continuation1)) != null);

                        if (request.DataServiceType == typeof(CustomRowBasedContext))
                        {
                            Assert.AreEqual(CustomRowBasedContext.customers.Count, totalCustomerCount);
                        }
                        else
                        {
                            Assert.AreEqual(CustomRowBasedOpenTypesContext.customers.Count, totalCustomerCount);
                        }

                        ctx.SaveChanges();

                        //
                        // Verify Customers
                        //
                        DataServiceCollection<MyCustomer> customers = new DataServiceCollection<MyCustomer>(ctx.Execute<MyCustomer>(new Uri("/Customers", UriKind.Relative)), TrackingMode.None);
                        while (customers.Continuation != null)
                        {
                            customers.Load(ctx.Execute<MyCustomer>(customers.Continuation));
                        }

                        foreach (MyCustomer c in customers)
                        {
                            Assert.AreEqual(c.Name, "Customer " + c.ID + " Updated");
                            totalCustomerCount--;
                        }

                        Assert.AreEqual(0, totalCustomerCount);

                        //
                        // Verify Orders
                        //
                        DataServiceCollection<MyOrder> orders = new DataServiceCollection<MyOrder>(ctx.Execute<MyOrder>(new Uri("/Orders", UriKind.Relative)), TrackingMode.None);
                        while (orders.Continuation != null)
                        {
                            orders.Load(ctx.Execute<MyOrder>(orders.Continuation));
                        }

                        foreach (MyOrder o in orders)
                        {
                            Assert.AreEqual(9999, o.DollarAmount);
                        }

                        if (request.DataServiceType == typeof(CustomRowBasedContext))
                        {
                            Assert.AreEqual(CustomRowBasedContext.orders.Count, orders.Count);
                        }
                        else
                        {
                            Assert.AreEqual(CustomRowBasedOpenTypesContext.orders.Count, orders.Count);
                        }
                    }
                });
            }

            void ctx_SendingRequest(object sender, SendingRequest2EventArgs e)
            {
                e.RequestMessage.SetHeader("CustomRequestHeader1", "CustomRequestHeaderValue1");
                e.RequestMessage.SetHeader("CustomRequestHeader2", "CustomRequestHeaderValue2");
            }

            #endregion

            private bool IsDataServiceOfT(Type type)
            {
                while (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(DataService<>))
                {
                    type = type.BaseType;
                    if (type == null)
                    {
                        return false;
                    }
                }

                return true;
            }

            private Exception SendRequest(Type serviceType, TestWebRequest request, string requestMethod, string requestUri, string ifMatch, string ifNoneMatch, string requestAccept, string requestContentType, string slug, object requestBody, int expectedStatusCode, bool includePreference = false)
            {
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(InterceptorChecker)))
                {
                    if (IsDataServiceOfT(serviceType))
                    {
                        request.ServiceType = serviceType;
                    }
                    else
                    {
                        request.DataServiceType = serviceType;
                    }

                    request.RequestUriString = requestUri;
                    request.HttpMethod = requestMethod;
                    request.Accept = requestAccept;
                    request.RequestContentType = requestContentType;
                    request.IfMatch = ifMatch;
                    request.IfNoneMatch = ifNoneMatch;

                    if (includePreference)
                    {
                        request.RequestHeaders["Prefer"] = "return=representation";
                    }

                    if (slug != null)
                    {
                        request.RequestHeaders["Slug"] = slug;
                    }

                    // custom headers
                    SetCustomRequestHeaders(request);

                    if (requestBody is byte[])
                    {
                        Stream requestStream = new MemoryStream((byte[])requestBody);
                        request.RequestStream = requestStream;
                    }
                    else if (requestBody is string)
                    {
                        request.SetRequestStreamAsText((string)requestBody);
                    }
                    else if (requestBody is Stream)
                    {
                        request.RequestStream = (Stream)requestBody;
                    }

                    Assert.AreEqual(0, InterceptorChecker.ChangeInterceptorInvokeCount, "InterceptorChecker.ChangeInterceptorInvokeCount is zero");
                    Assert.AreEqual(0, InterceptorChecker.QueryInterceptorInvokeCount, "InterceptorChecker.QueryInterceptorInvokeCount is zero");
                    this.EnsureTestHasNoLeakedStreams();
                    Assert.AreEqual(0, DataServiceStreamProvider.InstantiatedCount, "DataServiceStreamProvider.InstantiatedCount is zero");

                    Exception e = TestUtil.RunCatching(request.SendRequest);

                    if (e != null)
                    {
                        System.Diagnostics.Trace.WriteLine(string.Format("Exception from SendRequest():\r\n{0}\r\nCall Stack:\r\n{1}", e.Message, e.StackTrace));
                    }
                    else
                    {
                        // The Stream Provider throws 304, even though it's not treated as a failure, it goes through the exception path on the server...
                        // Some events will not get fired. We are skipping the asserts.
                        if (request.ResponseStatusCode != (int)HttpStatusCode.NotModified)
                        {
                            Assert.AreEqual(1, BlobDataServicePipelineHandlers.ProcessingRequestInvokeCount);
                            Assert.AreEqual(1, BlobDataServicePipelineHandlers.ProcessedRequestInvokeCount);
                            Assert.AreEqual("1", request.ResponseHeaders["ProcessingRequestInvokeCount"]);
                            Assert.AreEqual("1", request.ResponseHeaders["ProcessedRequestInvokeCount"]);
                            if (request.HttpMethod == "GET")
                            {
                                Assert.AreEqual(0, BlobDataServicePipelineHandlers.ProcessingChangesetInvokeCount);
                                Assert.AreEqual(0, BlobDataServicePipelineHandlers.ProcessedChangesetInvokeCount);
                            }
                            else
                            {
                                Assert.AreEqual(1, BlobDataServicePipelineHandlers.ProcessingChangesetInvokeCount);
                                Assert.AreEqual(1, BlobDataServicePipelineHandlers.ProcessedChangesetInvokeCount);
                            }
                        }
                    }

                    if (request.RequestStream != null)
                    {
                        request.RequestStream.Dispose();
                        request.RequestStream = null;
                    }

                    request.RequestContentLength = -1;

                    if (expectedStatusCode > -1)
                    {
                        Assert.AreEqual(expectedStatusCode, request.ResponseStatusCode);
                    }

                    DataServiceStreamProvider.ValidateInstantiatedInstances();

                    if (e == null)
                    {
                        if (BLOBSupportTest.ValidateInterceptorOverride != null)
                        {
                            BLOBSupportTest.ValidateInterceptorOverride();
                        }
                        else
                        {
                            InterceptorChecker.ValidateQueryInterceptor(1);
                            if (requestMethod != "GET")
                            {
                                InterceptorChecker.ValidateChangeInterceptor(1);
                            }
                            else
                            {
                                InterceptorChecker.ValidateChangeInterceptor(0);
                            }
                        }
                    }

                    return e;
                }
            }

            private Item FindItem(int id)
            {
                foreach (Item i in PhotoDataServiceContext._items)
                {
                    if (i.ID == id)
                    {
                        return i;
                    }
                }

                return null;
            }

            private Photo FindPhoto(int photoID)
            {
                Item i = FindItem(photoID);
                if (i != null)
                {
                    return (Photo)i;
                }

                return null;
            }

            private string GetItemPayload(string format, string baseUri, int id, string description, string name)
            {
                string uri = baseUri + "Items(" + id + ")";
                string editLink = "Items(" + id + ")";

                if (format == UnitTestsUtil.AtomFormat)
                {
                    return String.Format(
@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<entry xml:base=""{0}"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
  <id>{1}</id>
  <link rel=""edit"" title=""Item"" href=""{2}"" />
  <category term=""#{3}"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
  <content type=""application/xml"">
    <m:properties>
      <d:ID>{4}</d:ID>
      <d:Description>{5}</d:Description>
      <d:Name>{6}</d:Name>
    </m:properties>
  </content>
</entry>",
                        baseUri,
                        uri,
                        editLink,
                        typeof(Item).FullName,
                        id,
                        description,
                        name);
                }
                else
                {
                    throw new NotSupportedException("Unsupported format: " + format);
                }
            }

            private string GetPhotoPayload(string format, string baseUri, int expectedPhotoID, string description, string name, int rating, byte[] thumbNail)
            {
                string mleUri = baseUri + "Items(" + expectedPhotoID + ")";
                string editLink = "Items(" + expectedPhotoID + ")";

                string editMediaLink = editLink + "/$value";
                string src = editMediaLink;
                Uri readStreamUri = DataServiceStreamProvider.GetReadStreamUri(new Photo() { ID = expectedPhotoID });
                if (readStreamUri != null)
                {
                    src = readStreamUri.OriginalString;
                }

                string contentType = DataServiceStreamProvider.GetContentType(new Photo() { ID = expectedPhotoID });


                if (format == UnitTestsUtil.AtomFormat)
                {
                    return String.Format(
@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<entry xml:base=""{0}"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
  <id>{1}</id>
  <link rel=""edit"" title=""Photo"" href=""{2}"" />
  <link rel=""edit-media"" title=""Photo"" href=""{3}"" />
  <category term=""#{4}"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
  <content type=""{5}"" src=""{6}"" />
  <m:properties xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"">
    <d:ID>{7}</d:ID>
    <d:Description>{8}</d:Description>
    <d:Name>{9}</d:Name>
    <d:Rating>{10}</d:Rating>
    <d:ThumbNail>{11}</d:ThumbNail>
  </m:properties>
</entry>",
                        baseUri,
                        mleUri,
                        editLink,
                        editMediaLink,
                        typeof(Photo).FullName,
                        contentType,
                        src,
                        expectedPhotoID,
                        description,
                        name,
                        rating,
                        thumbNail == null ? null : Convert.ToBase64String(thumbNail));
                }
                else
                {
                    throw new NotSupportedException("Unsupported format: " + format);
                }
            }

            private string GetCustomerPayload(string format, string baseUri, string customerID, string companyName, string address, string city)
            {
                string mleUri = baseUri + "Customers('" + customerID + "')";
                string editLink = "Customers('" + customerID + "')";

                string editMediaLink = editLink + "/$value";
                string src = editMediaLink;
                NorthwindModel.Customers customer = NorthwindModel.Customers.CreateCustomers(customerID, companyName);
                Uri readStreamUri = DataServiceStreamProvider.GetReadStreamUri(customer);
                if (readStreamUri != null)
                {
                    src = readStreamUri.OriginalString;
                }

                string contentType = DataServiceStreamProvider.GetContentType(customer);

                if (format == UnitTestsUtil.AtomFormat)
                {
                    return String.Format(
@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<entry xml:base=""{0}"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
  <id>{1}</id>
  <link rel=""edit"" title=""Customers"" href=""{2}"" />
  <link rel=""edit-media"" title=""Customers"" href=""{3}"" />
  <category term=""#{4}"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
  <content type=""{5}"" src=""{6}"" />
  <m:properties xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"">
    <d:CustomerID>{7}</d:CustomerID>
    <d:CompanyName>{8}</d:CompanyName>
    <d:Address>{9}</d:Address>
    <d:City>{10}</d:City>
  </m:properties>
</entry>",
                        baseUri,
                        mleUri,
                        editLink,
                        editMediaLink,
                        typeof(NorthwindModel.Customers).FullName,
                        contentType,
                        src,
                        customerID,
                        companyName,
                        address,
                        city);
                }
                else
                {
                    throw new NotSupportedException("Unsupported format: " + format);
                }
            }

            private void ValidateMediaResourceFromGet(Type entityType, TestWebRequest request, object expectedID, object expectedContent)
            {
                Assert.IsTrue(DataServiceStreamProvider.ValidateCustomResponseHeaders(request));

                object entity;
                if (entityType == typeof(Photo))
                {
                    entity = new Photo() { ID = (int)expectedID };
                }
                else if (entityType == typeof(NorthwindModel.Customers))
                {
                    entity = NorthwindModel.Customers.CreateCustomers((string)expectedID, "");
                }
                else
                {
                    throw new NotSupportedException("Unsupported entity type: " + entityType.FullName);
                }

                Assert.AreEqual(request.ResponseContentType, DataServiceStreamProvider.GetContentType(entity));

                Stream expectedStream = null;
                if (expectedContent is byte[])
                {
                    expectedStream = new MemoryStream((byte[])expectedContent);
                }
                else if (expectedContent is Stream)
                {
                    expectedStream = (Stream)expectedContent;
                    ((LargeStream)expectedStream).ReOpen();
                }

                Assert.IsNotNull(expectedStream);

                using (expectedStream)
                using (Stream resultStream = request.GetResponseStream())
                {
                    Assert.IsTrue(TestUtil.CompareStream(expectedStream, resultStream));
                }

                Assert.AreEqual(0, DataServiceStreamProvider.UnDisposedInstances);
            }

            private void ValidateMediaResourceFromStorage(Type entityType, TestWebRequest request, object expectedID, object expectedContent)
            {
                if (request != null)
                {
                    Assert.IsTrue(DataServiceStreamProvider.ValidateCustomResponseHeaders(request));
                }

                object entity;
                if (entityType == typeof(Photo))
                {
                    entity = new Photo() { ID = (int)expectedID };
                }
                else if (entityType == typeof(NorthwindModel.Customers))
                {
                    entity = NorthwindModel.Customers.CreateCustomers((string)expectedID, "");
                }
                else
                {
                    throw new NotSupportedException("Unsupported entity type: " + entityType.FullName);
                }

                Stream expectedStream = null;
                if (expectedContent is byte[])
                {
                    expectedStream = new MemoryStream((byte[])expectedContent);
                }
                else if (expectedContent is Stream)
                {
                    expectedStream = (Stream)expectedContent;
                    ((LargeStream)expectedStream).ReOpen();
                }

                Assert.IsNotNull(expectedStream);

                Stream storedStream = null;
                if (DataServiceStreamProvider.ProviderStorageMode == DataServiceStreamProvider.StorageMode.Disk)
                {
                    storedStream = File.OpenRead(DataServiceStreamProvider.GetStoragePath(entity));
                }
                else if (DataServiceStreamProvider.ProviderStorageMode == DataServiceStreamProvider.StorageMode.LargeStream)
                {
                    storedStream = DataServiceStreamProvider.GetLargeStream(entity);
                    ((LargeStream)storedStream).ReOpen();
                }

                Assert.IsNotNull(storedStream);

                using (expectedStream)
                using (storedStream)
                {
                    if (DataServiceStreamProvider.ProviderStorageMode == DataServiceStreamProvider.StorageMode.Disk)
                    {
                        Assert.IsTrue(TestUtil.CompareStream(expectedStream, storedStream));
                    }
                    else if (DataServiceStreamProvider.ProviderStorageMode == DataServiceStreamProvider.StorageMode.LargeStream)
                    {
                        Assert.IsTrue(((LargeStream)expectedStream).Compare((LargeStream)storedStream));
                    }
                }

                this.EnsureTestHasNoLeakedStreams();
            }

            private void ValidateItemOnContext(int expectedID, string expectedDescription, string expectedName)
            {
                Item i = FindItem(expectedID);
                Assert.IsNotNull(i);

                Assert.AreEqual(expectedDescription, i.Description);
                Assert.AreEqual(expectedName, i.Name);
                Assert.AreEqual(0, DataServiceStreamProvider.UnDisposedInstances, "Expecting 0 DataServiceStreamProvider.UnDisposedInstances, but found " + DataServiceStreamProvider.UnDisposedInstances);
            }

            private void ValidateItemFromResponse(TestWebRequest request, string responseFormat, int expectedID, string expectedDescription, string expectedName)
            {
                string baseUriWithSlash = request.BaseUri.EndsWith("/") ? request.BaseUri : request.BaseUri + "/";
                string expectedUri = baseUriWithSlash + "Items(" + expectedID + ")";
                string expectedEditLink = "Items(" + expectedID + ")";

                string[] atomXPaths = new string[] {
                    String.Format(@"/atom:entry[atom:category/@term='#{0}' and
                                              atom:id='{1}' and
                                              atom:link[@rel='edit' and @href ='{2}'] and
                                              atom:content[@type='application/xml' and
                                                           adsm:properties[ads:ID='{3}' and
                                                                           ads:Description='{4}' and
                                                                           ads:Name='{5}']
                                                          ]
                                   ]",
                    typeof(Item).FullName,
                    expectedUri,
                    expectedEditLink,
                    expectedID,
                    expectedDescription,
                    expectedName)
                };


                string[] xPaths = atomXPaths;

                UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), responseFormat, xPaths);
                Assert.AreEqual(0, DataServiceStreamProvider.UnDisposedInstances);
            }

            private void ValidatePhotoMLEOnContext(int expectedPhotoID, string expectedPhotoDescription, string expectedPhotoName, int expectedPhotoRating, byte[] expectedThumbNail)
            {
                Photo p = FindPhoto(expectedPhotoID);
                Assert.IsNotNull(p);

                Assert.AreEqual(expectedPhotoDescription, p.Description);
                Assert.AreEqual(expectedPhotoName, p.Name);
                Assert.AreEqual(expectedPhotoRating, p.Rating);
                Assert.AreEqual(expectedThumbNail.Length, p.ThumbNail.Length);
                for (int i = 0; i < expectedThumbNail.Length; i++)
                {
                    Assert.AreEqual(expectedThumbNail[i], p.ThumbNail[i]);
                }

                Assert.AreEqual(0, DataServiceStreamProvider.UnDisposedInstances);
            }

            private void ValidatePhotoMLEFromResponse(
                TestWebRequest request,
                string responseFormat,
                Type expectedType,
                int expectedPhotoID,
                string expectedPhotoDescription,
                string expectedPhotoName,
                int expectedPhotoRating,
                byte[] expectedThumbNail,
                string expectedMleETag,
                string expectedMediaResourceETag,
                bool validateCustomResponseHeaders,
                bool editLinkHasTypeSegment = true)
            {
                if (validateCustomResponseHeaders)
                {
                    Assert.IsTrue(DataServiceStreamProvider.ValidateCustomResponseHeaders(request));
                }

                string typeSegmentIfRequired = editLinkHasTypeSegment ? "/" + expectedType.FullName : "";

                string baseUriWithSlash = request.BaseUri.EndsWith("/") ? request.BaseUri : request.BaseUri + "/";
                string expectedIdUri = baseUriWithSlash + "Items(" + expectedPhotoID + ")";
                string expectedEditLink = "Items(" + expectedPhotoID + ")" + typeSegmentIfRequired;

                string expectedEditMediaLink = expectedEditLink + "/$value";
                string expectedSrcUri = expectedEditMediaLink;
                Uri readStreamUri = DataServiceStreamProvider.GetReadStreamUri(new Photo() { ID = expectedPhotoID });
                if (readStreamUri != null)
                {
                    expectedSrcUri = readStreamUri.OriginalString;
                }

                string expectedContentType = DataServiceStreamProvider.GetContentType(new Photo() { ID = expectedPhotoID });

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("/atom:entry[atom:category/@term='#{0}' and\r\n", expectedType.FullName);
                sb.AppendFormat("          atom:id='{0}' and\r\n", expectedIdUri);
                //if (!string.IsNullOrEmpty(expectedMleETag))
                //{
                //    sb.AppendFormat("          @adsm:etag='{0}' and\r\n", expectedMleETag);
                //}
                sb.AppendFormat("          atom:content[@type='{0}' and @src='{1}'] and\r\n", expectedContentType, expectedSrcUri);
                //if (string.IsNullOrEmpty(expectedMediaResourceETag))
                //{
                sb.AppendFormat("          atom:link[@rel='edit-media' and @href ='{0}' ] and\r\n", expectedEditMediaLink);
                //}
                //else
                //{
                //    sb.AppendFormat("          atom:link[@rel='edit-media' and @href ='{0}' @adsm:etag='{1}'] and\r\n", expectedEditMediaLink, expectedMediaResourceETag);
                //}
                sb.AppendFormat("          atom:link[@rel='edit' and @href ='{0}'] and\r\n", expectedEditLink);
                sb.AppendFormat("          adsm:properties[ads:ID='{0}' and\r\n", expectedPhotoID);
                sb.AppendFormat("                         ads:Description='{0}' and\r\n", expectedPhotoDescription);
                sb.AppendFormat("                         ads:Name='{0}' and\r\n", expectedPhotoName);
                sb.AppendFormat("                         ads:Rating='{0}' and\r\n", expectedPhotoRating);
                sb.AppendFormat("                         ads:ThumbNail='{0}']\r\n", expectedThumbNail == null ? "" : Convert.ToBase64String(expectedThumbNail));
                sb.AppendFormat("          ]\r\n");
                sb.AppendFormat("");
                string[] xPaths = new string[] { sb.ToString() };

                MemoryStream response = new MemoryStream();
                TestUtil.CopyStream(request.GetResponseStream(), response);
                response.Position = 0;
                string responsePayloadText = (new StreamReader(response)).ReadToEnd();
                response.Position = 0;
                UnitTestsUtil.VerifyXPaths(response, responseFormat, xPaths);
                Assert.AreEqual(0, DataServiceStreamProvider.UnDisposedInstances);

                if (!string.IsNullOrEmpty(expectedMleETag))
                {
                    expectedMleETag = expectedMleETag.Replace("\"", "&quot;");
                    Assert.IsTrue(Regex.Match(responsePayloadText, "<entry\\s+[^>]*?m:etag=\"" + Regex.Escape(expectedMleETag) + "\"").Success);
                }

                if (!string.IsNullOrEmpty(expectedMediaResourceETag))
                {
                    expectedMediaResourceETag = expectedMediaResourceETag.Replace("\"", "&quot;");
                    Assert.IsTrue(Regex.Match(responsePayloadText, "<link\\s+[^>]*?m:etag=\"" + Regex.Escape(expectedMediaResourceETag) + "\"").Success);
                }
            }

            private void ValidateCustomersMLEFromResponse(TestWebRequest request, string responseFormat, string customerID, string companyName, string address, string city, bool validateCustomResponseHeaders)
            {
                if (validateCustomResponseHeaders)
                {
                    Assert.IsTrue(DataServiceStreamProvider.ValidateCustomResponseHeaders(request));
                }

                string baseUriWithSlash = request.BaseUri.EndsWith("/") ? request.BaseUri : request.BaseUri + "/";
                string expectedMLEUri = baseUriWithSlash + "Customers('" + customerID + "')";
                string expectedEditLink = "Customers('" + customerID + "')";

                string expectedEditMediaLink = expectedEditLink + "/$value";
                string expectedSrcUri = expectedEditMediaLink;
                NorthwindModel.Customers customer = NorthwindModel.Customers.CreateCustomers(customerID, companyName);
                Uri readStreamUri = DataServiceStreamProvider.GetReadStreamUri(customer);
                if (readStreamUri != null)
                {
                    expectedSrcUri = readStreamUri.OriginalString;
                }

                string expectedContentType = DataServiceStreamProvider.GetContentType(customer);

                string[] atomXPaths = new string[] {
                    String.Format(@"/atom:entry[atom:category/@term=""#{0}"" and
                                              atom:id=""{1}"" and
                                              atom:content[@type=""{2}"" and @src=""{3}""] and
                                              atom:link[@rel=""edit-media"" and @title=""Customers"" and @href =""{4}"" ] and
                                              atom:link[@rel=""edit"" and @title=""Customers"" and @href =""{5}""] and
                                              adsm:properties[ads:CustomerID=""{6}"" and
                                                              ads:CompanyName=""{7}"" and
                                                              ads:Address=""{8}"" and
                                                              ads:City=""{9}""]
                                   ]",
                    typeof(NorthwindModel.Customers).FullName,
                    expectedMLEUri,
                    expectedContentType,
                    expectedSrcUri,
                    expectedEditMediaLink,
                    expectedEditLink,
                    customerID,
                    companyName,
                    address,
                    city)
                };

                string[] xPaths = atomXPaths;

                UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), responseFormat, xPaths);
                Assert.AreEqual(0, DataServiceStreamProvider.UnDisposedInstances);
            }

            private void SetCustomRequestHeaders(TestWebRequest request)
            {
                request.RequestHeaders["CustomRequestHeader1"] = "CustomRequestHeaderValue1";
                request.RequestHeaders["CustomRequestHeader2"] = "CustomRequestHeaderValue2";
            }

            private void Batch_SetCustomRequestHeaders(StreamWriter writer)
            {
                writer.WriteLine("CustomRequestHeader1: CustomRequestHeaderValue1");
                writer.WriteLine("CustomRequestHeader2: CustomRequestHeaderValue2");
                writer.Flush();
            }

            internal static void FillBuffer(byte[] buffer)
            {
                if (buffer.Length > 0)
                {
                    Random rand = TestUtil.Random;
                    rand.NextBytes(buffer);
                }
            }

            private void EnsureTestHasNoLeakedStreams()
            {
                string description = DataServiceStreamProvider.ConsumeTraceDescription();
                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(10));
                Assert.AreEqual(0, DataServiceStreamProvider.UnDisposedInstances,
                    "Expecting 0 DataServiceStreamProvider.UnDisposedInstances, but found " +
                    DataServiceStreamProvider.UnDisposedInstances + " - any instances here indicate some previous test leaked them\r\n" +
                    description);
            }
        }

    }
}
