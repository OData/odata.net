//---------------------------------------------------------------------
// <copyright file="NamedStreamTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using providers = Microsoft.OData.Service.Providers;
    using Microsoft.OData.Client;

    #endregion Namespaces

    /// <summary>
    /// This is a test class for WebDataServiceTest and is intended
    /// to contain all WebDataServiceTest Unit Tests
    /// </summary>
    [TestModule]
    public class NamedStreamUnitTestModule : AstoriaTestModule
    {
        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/867
        [TestClass()]
        public class NamedStreamTests
        {
            private static readonly XName NamedStreamElement = UnitTestsUtil.MetadataNamespace + "NamedStream";
            private const string NameAttribute = "Name";
            private static readonly string TypeAttribute = "Type";
            private const string NullableAttribute = "Nullable";
            private static readonly string HasStreamAttributeStr = UnitTestsUtil.MetadataNamespace.NamespaceName + ":HasStream";
            //private static readonly XName HasStreamAttribute = UnitTestsUtil.EdmOasisNamespace + "HasStream";
            private static readonly XName HasStreamAttribute = "HasStream";
            private const string XmlFalseLiteral = "false";

            private static readonly bool[] BooleanValues = new[] { false, true };
            private static readonly ODataProtocolVersion[] ProtocolVersions = new[] { ODataProtocolVersion.V4 };
            private static readonly string[] DataServiceVersions = new[] { "4.0" };

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

            #region Metadata API Tests
            // See MetadataTests.cs...
            #endregion Metadata API Tests

            #region Metadata Runtime Tests

            #region IDSP Metadata Tests
            [Ignore] // Remove Atom
            // [TestMethod]
            public void NamedStreamIDSPNamedStreamsOnDerivedTypes()
            {
                DSPMetadata metadata = new DSPMetadata("NamedStreamIDSPContainer", "NamedStreamTest");
                providers.ResourceType baseWithNamedStream = metadata.AddEntityType("BaseWithNamedStream", null, null, false);
                metadata.AddKeyProperty(baseWithNamedStream, "ID", typeof(int));
                baseWithNamedStream.AddProperty(new providers.ResourceProperty("Stream1", providers.ResourcePropertyKind.Stream, providers.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream))));

                providers.ResourceType rootEntityType = metadata.AddEntityType("RootEntityType", null, baseWithNamedStream, false);
                rootEntityType.AddProperty(new providers.ResourceProperty("Stream2", providers.ResourcePropertyKind.Stream, providers.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream))));

                providers.ResourceType derivedWithBlobAndNamedStream = metadata.AddEntityType("DerivedWithBlobAndNamedStream", null, rootEntityType, false);
                derivedWithBlobAndNamedStream.IsMediaLinkEntry = true;
                var stream3Property = new providers.ResourceProperty("Stream3", providers.ResourcePropertyKind.Stream, providers.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)));
                derivedWithBlobAndNamedStream.AddProperty(stream3Property);

                providers.ResourceSet mySet = metadata.AddResourceSet("StreamedEntities", rootEntityType);

                DSPServiceDefinition service = new DSPServiceDefinition();
                service.Metadata = metadata;
                service.MediaResourceStorage = new DSPMediaResourceStorage();
                service.SupportMediaResource = true;
                service.SupportNamedStream = true;
                service.ForceVerboseErrors = true;

                DSPContext context = new DSPContext();
                DSPResource entity1 = new DSPResource(rootEntityType);
                entity1.SetValue("ID", 1);
                DSPResource entity2 = new DSPResource(derivedWithBlobAndNamedStream);
                entity2.SetValue("ID", 2);
                context.GetResourceSetEntities("StreamedEntities").Add(entity1);
                context.GetResourceSetEntities("StreamedEntities").Add(entity2);
                service.MediaResourceStorage.CreateMediaResource(entity2, null).ContentType = "CustomType/Foo";
                var mediaResourceStream3 = service.MediaResourceStorage.CreateMediaResource(entity2, stream3Property);
                mediaResourceStream3.ContentType = "CustomType/Stream3";
                mediaResourceStream3.GetWriteStream().WriteByte((int)'c');

                service.CreateDataSource = (m) => context;

                using (TestWebRequest request = service.CreateForInProcessWcf())
                {
                    request.RequestUriString = "/$metadata";
                    request.HttpMethod = "GET";
                    request.SendRequest();

                    Assert.AreEqual("4.0;", request.ResponseVersion);

                    UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), UnitTestsUtil.MimeApplicationXml,
                        "//csdl:EntityType[@Name='BaseWithNamedStream']/csdl:Property[@Name='Stream1' and @Type='Edm.Stream']",
                        "//csdl:EntityType[@Name='RootEntityType']/csdl:Property[@Name='Stream2' and @Type='Edm.Stream']",
                        "//csdl:EntityType[@Name='DerivedWithBlobAndNamedStream' and @HasStream='true' and @BaseType='NamedStreamTest.RootEntityType']/csdl:Property[@Name='Stream3' and @Type='Edm.Stream']");

                    string setUri = request.ServiceRoot.AbsoluteUri + "/StreamedEntities";
                    string[] jsonXPaths = new[] {
                        String.Format(
                            "//{0}[__metadata/type='NamedStreamTest.RootEntityType' and Stream1/__mediaresource/edit_media='{1}/Stream1' and Stream2/__mediaresource/edit_media='{1}/Stream2']", 
                            JsonValidator.ObjectString,
                            setUri + "(1)"),
                        String.Format(
                            "//{0}[__metadata/type='NamedStreamTest.DerivedWithBlobAndNamedStream' and Stream1/__mediaresource/edit_media='{1}/Stream1' and Stream2/__mediaresource/edit_media='{1}/Stream2' and Stream3/__mediaresource/edit_media='{1}/Stream3']", 
                            JsonValidator.ObjectString,
                            setUri + "(2)/NamedStreamTest.DerivedWithBlobAndNamedStream"),
                    };

                    string[] atomXPaths = new[] {
                        String.Format(
                            "//atom:entry[atom:category/@term='#{0}' and atom:link[@title='Stream1' and @href='StreamedEntities(1)/Stream1'] and atom:link[@title='Stream2' and @href='StreamedEntities(1)/Stream2']]",
                            "NamedStreamTest.RootEntityType"),
                        String.Format("//atom:entry[" +
                            "atom:category/@term='#NamedStreamTest.DerivedWithBlobAndNamedStream' and " +
                            "atom:link[@title='Stream1' and @rel='{0}/Stream1' and @href='StreamedEntities(2)/{1}/Stream1'] and " +
                            "atom:link[@title='Stream2' and @rel='{0}/Stream2' and @href='StreamedEntities(2)/{1}/Stream2'] and " +
                            "atom:link[@title='Stream3' and @type='CustomType/Stream3' and @rel='{0}/Stream3' and @href='StreamedEntities(2)/{1}/Stream3'] and " +
                            "atom:content[@type='CustomType/Foo' and @src='StreamedEntities(2)/{1}/$value']" +
                        "]",
                        "http://docs.oasis-open.org/odata/ns/edit-media", "NamedStreamTest.DerivedWithBlobAndNamedStream")
                    };

                    foreach (string format in UnitTestsUtil.ResponseFormats)
                    {
                        request.RequestUriString = "/StreamedEntities";
                        request.HttpMethod = "GET";
                        request.Accept = format;
                        request.SendRequest();
                        Assert.AreEqual("4.0;", request.ResponseVersion);
                        UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), format, null, jsonXPaths, atomXPaths);
                    }

                    request.RequestUriString = "/StreamedEntities(2)/NamedStreamTest.DerivedWithBlobAndNamedStream/Stream3";
                    request.Accept = UnitTestsUtil.MimeAny;
                    request.SendRequest();
                    Assert.AreEqual("CustomType/Stream3", request.ResponseContentType, "the content type should be as specified in the metadata");
                    Assert.AreEqual(request.GetResponseStreamAsText(), "c", "Make sure the content are the same");

                    request.RequestUriString = "/StreamedEntities(2)/Stream3";
                    request.Accept = UnitTestsUtil.MimeAny;
                    var exception = TestUtil.RunCatching<WebException>(request.SendRequest);
                    Assert.AreEqual(HttpStatusCode.NotFound, TestUtil.GetStatusCodeFromException(exception), "Expected status code of 404");
                }
            }

            [TestMethod]
            public void NamedStreamIDSPMetadataSerializationTest()
            {
                DSPMetadata metadata = new DSPMetadata("NamedStreamIDSPContainer", "NamedStreamTest");
                providers.ResourceType base1WithNamedStream = metadata.AddEntityType("Base1WithNamedStream", null, null, false);
                metadata.AddKeyProperty(base1WithNamedStream, "ID", typeof(int));
                providers.ResourceProperty stream1 = new providers.ResourceProperty("Stream1", providers.ResourcePropertyKind.Stream, providers.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)));
                base1WithNamedStream.AddProperty(stream1);
                providers.ResourceProperty stream2 = new providers.ResourceProperty("Stream2", providers.ResourcePropertyKind.Stream, providers.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)));
                base1WithNamedStream.AddProperty(stream2);

                providers.ResourceType base2WithoutNamedStream = metadata.AddEntityType("Base2WithoutNamedStream", null, base1WithNamedStream, false);

                providers.ResourceType rootEntityType = metadata.AddEntityType("RootEntityType", null, base2WithoutNamedStream, false);
                providers.ResourceProperty stream3 = new providers.ResourceProperty("Stream3", providers.ResourcePropertyKind.Stream, providers.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)));
                rootEntityType.AddProperty(stream3);
                providers.ResourceProperty stream4 = new providers.ResourceProperty("Stream4", providers.ResourcePropertyKind.Stream, providers.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)));
                rootEntityType.AddProperty(stream4);

                providers.ResourceType derived1WithBlob = metadata.AddEntityType("Derived1WithBlob", null, rootEntityType, false);
                derived1WithBlob.IsMediaLinkEntry = true;

                providers.ResourceType derived2 = metadata.AddEntityType("Derived2", null, derived1WithBlob, false);

                providers.ResourceSet mySet = metadata.AddResourceSet("StreamedEntities", rootEntityType);

                DSPServiceDefinition service = new DSPServiceDefinition();
                service.Metadata = metadata;
                service.MediaResourceStorage = new DSPMediaResourceStorage();
                service.SupportMediaResource = true;
                service.SupportNamedStream = true;
                service.ForceVerboseErrors = true;
                using (TestWebRequest request = service.CreateForInProcessWcf())
                {
                    request.RequestUriString = "/$metadata";
                    request.HttpMethod = "GET";
                    request.SendRequest();

                    Assert.AreEqual("4.0;", request.ResponseVersion);
                    XDocument edmMetadata = UnitTestsUtil.GetResponseAsAtomXLinq(request);


                    var expectedNameStreams = new Dictionary<String, String[]>();
                    expectedNameStreams.Add(base1WithNamedStream.Name, new[] { stream1.Name, stream2.Name });
                    expectedNameStreams.Add(rootEntityType.Name, new[] { stream3.Name, stream4.Name });

                    VerifyStreamsInMetadata(edmMetadata, 5, new[] { derived1WithBlob.Name }, expectedNameStreams);
                }
            }

            private void MetadataStreamProviderValidation(bool containsMediaResource, bool containsNamedStream, bool supportMediaResource, bool supportNamedStream, bool visibleStreamType, TestWebRequest request)
            {
                request.RequestUriString = "/$metadata";
                request.HttpMethod = "GET";

                WebException webException = (WebException)TestUtil.RunCatching(request.SendRequest);
                string response = request.GetResponseStreamAsText();

                if (!visibleStreamType)
                {
                    TestUtil.AssertContains(request.ResponseVersion, "4.0");

                    // expect no failure.
                    Assert.IsNull(webException);
                    TestUtil.AssertContainsFalse(response, "error");
                }
                else if (!containsMediaResource && !containsNamedStream)
                {
                    TestUtil.AssertContains(request.ResponseVersion, "4.0");

                    // expect no failure.
                    Assert.IsNull(webException);
                    TestUtil.AssertContainsFalse(response, "error");
                }
                else if (containsMediaResource && !containsNamedStream) // V2 scenario
                {
                    TestUtil.AssertContains(request.ResponseVersion, "4.0");
                    
                    // Service contains MLE, implementation of IDSSP or IDSSP2 is required.
                    if (!supportMediaResource && !supportNamedStream)
                    {
                        Assert.IsNotNull(webException);
                        TestUtil.AssertContains(response, DataServicesResourceUtil.GetString("DataServiceStreamProviderWrapper_MustImplementIDataServiceStreamProviderToSupportStreaming"));
                    }
                    else
                    {
                        Assert.IsNull(webException);
                        // MLEs are either supported through IDSSP, or when maxProtocolVersion >= V3, automatically supported through IDSSP2.
                        // expect no failure.
                        TestUtil.AssertContainsFalse(response, "error");
                    }
                }
                else // NamedStreams or NamedStream + Blob, V3 scenario
                {
                    // Service contains named streams, MaxProtocolVersion >= V3 is required.
                    if (!supportNamedStream)
                    {
                        Assert.IsNotNull(webException);
                        TestUtil.AssertContains(request.ResponseVersion, "4.0");

                        // Service contains named stream, implementation of IDSSP2 is required.
                        TestUtil.AssertContains(response, DataServicesResourceUtil.GetString("DataServiceStreamProviderWrapper_MustImplementDataServiceStreamProvider2ToSupportNamedStreams"));
                    }
                    else
                    {
                        Assert.IsNull(webException);
                        TestUtil.AssertContains(request.ResponseVersion, "4.0");

                        // Service implements IDSSP2, no error is expected.
                        TestUtil.AssertContainsFalse(response, "error");
                    }
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod]
            public void NamedStreamIDSPMetadataStreamProviderValidationTest()
            {
                TestUtil.RunCombinations(BooleanValues, BooleanValues, BooleanValues, BooleanValues, BooleanValues, ProtocolVersions,
                    (containsMediaResource, containsNamedStream, supportMediaResource, supportNamedStream, visibleStreamType, maxProtocolVersion) =>
                {
                    DSPMetadata metadata = new DSPMetadata("NamedStreamIDSPContainer", "NamedStreamTest");
                    providers.ResourceType entityType1 = metadata.AddEntityType("EntityType1", null, null, false);
                    metadata.AddKeyProperty(entityType1, "ID", typeof(int));

                    providers.ResourceType entityType2 = metadata.AddEntityType("EntityType2", null, null, false);
                    metadata.AddKeyProperty(entityType2, "ID", typeof(int));
                    if (containsMediaResource)
                    {
                        entityType2.IsMediaLinkEntry = true;
                    }

                    if (containsNamedStream)
                    {
                        entityType2.AddProperty(new providers.ResourceProperty("Stream1", providers.ResourcePropertyKind.Stream, providers.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream))));
                    }

                    metadata.AddResourceSet("NonStreamSet", entityType1);
                    metadata.AddResourceSet("StreamSet", entityType2);

                    DSPServiceDefinition service = new DSPServiceDefinition();
                    service.Metadata = metadata;
                    service.MediaResourceStorage = new DSPMediaResourceStorage();
                    service.ForceVerboseErrors = true;
                    service.SupportMediaResource = supportMediaResource;
                    service.SupportNamedStream = supportNamedStream;

                    using (OpenWebDataServiceHelper.EntitySetAccessRule.Restore())
                    using (OpenWebDataServiceHelper.MaxProtocolVersion.Restore())
                    using (TestWebRequest request = service.CreateForInProcessWcf())
                    {
                        OpenWebDataServiceHelper.EntitySetAccessRule.Value = new Dictionary<string, EntitySetRights>()
                        {
                            { "NonStreamSet", EntitySetRights.All },
                            { "StreamSet", EntitySetRights.ReadSingle },
                        };

                        if (!visibleStreamType)
                        {
                            OpenWebDataServiceHelper.EntitySetAccessRule.Value["StreamSet"] = EntitySetRights.None;
                        }

                        OpenWebDataServiceHelper.MaxProtocolVersion.Value = maxProtocolVersion;

                        this.MetadataStreamProviderValidation(containsMediaResource, containsNamedStream, supportMediaResource, supportNamedStream, visibleStreamType, request);
                    }
                });
            }

            #endregion IDSP Metadata Tests

            #region EF Metadata Tests

            [TestMethod]
            public void NamedStreamEFNamedStreamsOnDerivedTypes()
            {
                System.Data.Metadata.Edm.MetadataWorkspace.ClearCache();
                TestUtil.ClearMetadataCache();

                using (TestUtil.RestoreStaticValueOnDispose(typeof(OpenWebDataServiceHelper), "ForceVerboseErrors"))
                using (OpenWebDataServiceHelper.GetServiceCustomizer.Restore())
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    OpenWebDataServiceHelper.ForceVerboseErrors = true;
                    OpenWebDataServiceHelper.GetServiceCustomizer.Value = (type) =>
                        {
                            if (type == typeof(providers.IDataServiceStreamProvider2))
                            {
                                return new DSPStreamProvider2(new DSPMediaResourceStorage());
                            }

                            if (type == typeof(providers.IDataServiceStreamProvider))
                            {
                                return new DSPStreamProvider2(new DSPMediaResourceStorage());
                            }

                            return null;
                        };

                    request.DataServiceType = typeof(AstoriaUnitTests.ObjectContextStubs.Hidden.EFModelWithNamedStreamOnDerivedType);
                    request.RequestUriString = "/$metadata";
                    request.HttpMethod = "GET";
                    request.SendRequest();

                    Assert.AreEqual("4.0;", request.ResponseVersion);

                    UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), UnitTestsUtil.MimeApplicationXml,
                       "//csdl:EntityType[@Name='EFEntity1']/csdl:Property[@Name='Stream1' and @Type='Edm.Stream']",
                       "//csdl:EntityType[@Name='EFEntity2' and @BaseType='Model1.EFEntity1']/csdl:Property[@Name='Stream2' and @Type='Edm.Stream']");
                }
            }

            [TestMethod]
            public void NamedStreamEFMetadataSerializationTest()
            {
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(NorthwindDefaultStreamService)))
                using (NorthwindDefaultStreamService.SetupNorthwindWithStream(
                    new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("Customers", "true"), new KeyValuePair<string, string>("Orders", "true") },
                    "NamedStreamTests_EFMetadataSerialization"))
                using (TestUtil.RestoreStaticValueOnDispose(typeof(OpenWebDataServiceHelper), "ForceVerboseErrors"))
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    OpenWebDataServiceHelper.ForceVerboseErrors = true;
                    NorthwindDefaultStreamService.GetServiceOverride = type =>
                    {
                        if (type == typeof(providers.IDataServiceStreamProvider2))
                        {
                            return new DSPStreamProvider2(new DSPMediaResourceStorage());
                        }

                        if (type == typeof(providers.IDataServiceStreamProvider))
                        {
                            return new DSPStreamProvider2(new DSPMediaResourceStorage());
                        }

                        return null;
                    };

                    request.DataServiceType = NorthwindNamedStreamServiceFactory.GetNamedStreamServiceType();
                    request.RequestUriString = "/$metadata";
                    request.HttpMethod = "GET";
                    request.SendRequest();

                    Assert.AreEqual("4.0;", request.ResponseVersion);

                    XDocument edmMetadata = request.GetResponseStreamAsXDocument();

                    var expectedNameStreams = new Dictionary<String, String[]>();
                    expectedNameStreams.Add("Customers", new string[] { "Stream1", "Stream2" });
                    expectedNameStreams.Add("Order_Details", new string[] { "Stream3", "Stream4" });
                    expectedNameStreams.Add("Orders", new string[] { "Stream1" });

                    VerifyStreamsInMetadata(edmMetadata, 26, new string[] { "Customers", "Orders" }, expectedNameStreams);
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod]
            public void NamedStreamEFMetadataStreamProviderValidationTest()
            {
                TestUtil.RunCombinations(BooleanValues, BooleanValues, (containsMediaResource, containsNamedStream) =>
                {
                    KeyValuePair<string, string>[] mles = null;

                    if (containsMediaResource)
                    {
                        mles = new KeyValuePair<string, string>[]
                        {
                            new KeyValuePair<string, string>("Orders", "true")
                        };
                    }

                    using (TestUtil.RestoreStaticValueOnDispose(typeof(OpenWebDataServiceHelper), "ForceVerboseErrors"))
                    using (NorthwindDefaultStreamService.SetupNorthwindWithStream(mles, "NamedStreamTests_EFMetadataStreamProvider"))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(NorthwindDefaultStreamService)))
                    {
                        OpenWebDataServiceHelper.ForceVerboseErrors = true;

                        TestUtil.RunCombinations(ProtocolVersions, BooleanValues, BooleanValues, BooleanValues,
                            (maxProtocolVersion, supportMediaResource, supportNamedStream, visibleStreamType) =>
                        {
                            using (OpenWebDataServiceHelper.EntitySetAccessRule.Restore())
                            using (OpenWebDataServiceHelper.MaxProtocolVersion.Restore())
                            using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                            {
                                bool IDSSP2Loaded = false;
                                bool IDSSP1Loaded = false;

                                NorthwindDefaultStreamService.GetServiceOverride = type =>
                                {
                                    if (type == typeof(providers.IDataServiceStreamProvider2) && supportNamedStream)
                                    {
                                        if (maxProtocolVersion < ODataProtocolVersion.V4)
                                        {
                                            throw new InvalidOperationException("If MaxProtocolVersion < V3, GetService<IDSSP2>() must not be called.");
                                        }

                                        if (IDSSP2Loaded)
                                        {
                                            throw new InvalidOperationException("Cannot load IDSSP2 twice.");
                                        }

                                        if (IDSSP1Loaded)
                                        {
                                            throw new InvalidOperationException("If MaxProtocolVersion >= V3, GetService<IDSSP2>() must be called before GetService<IDSSP>().");
                                        }

                                        IDSSP2Loaded = true;
                                        return new DSPStreamProvider2(new DSPMediaResourceStorage());
                                    }

                                    if (type == typeof(providers.IDataServiceStreamProvider) && supportMediaResource)
                                    {
                                        if (IDSSP1Loaded)
                                        {
                                            throw new InvalidOperationException("Cannot load IDSSP twice.");
                                        }

                                        if (maxProtocolVersion >= ODataProtocolVersion.V4)
                                        {
                                            if (!IDSSP2Loaded && supportNamedStream)
                                            {
                                                throw new InvalidOperationException("If MaxProtocolVersion >= V3, GetService<IDSSP2>() must be called before GetService<IDSSP>().");
                                            }
                                        }
                                        else
                                        {
                                            if (IDSSP2Loaded)
                                            {
                                                throw new InvalidOperationException("If MaxProtocolVersion < V3, GetService<IDSSP2>() must not be called.");
                                            }
                                        }

                                        return new DSPStreamProvider(new DSPMediaResourceStorage());
                                    }

                                    return null;
                                };

                                OpenWebDataServiceHelper.EntitySetAccessRule.Value = new Dictionary<string, EntitySetRights>()
                                        {
                                            { "Customers", EntitySetRights.All },
                                            { "Orders", EntitySetRights.ReadSingle },
                                        };

                                if (!visibleStreamType)
                                {
                                    OpenWebDataServiceHelper.EntitySetAccessRule.Value["Orders"] = EntitySetRights.None;
                                    OpenWebDataServiceHelper.EntitySetAccessRule.Value["Customers"] = EntitySetRights.None;
                                    OpenWebDataServiceHelper.EntitySetAccessRule.Value["Order_Details"] = EntitySetRights.None;
                                }

                                OpenWebDataServiceHelper.MaxProtocolVersion.Value = maxProtocolVersion;
                                request.DataServiceType = containsNamedStream ? NorthwindNamedStreamServiceFactory.GetNamedStreamServiceType() : typeof(NorthwindDefaultStreamService);
                                TestUtil.ClearConfiguration();
                                this.MetadataStreamProviderValidation(containsMediaResource, containsNamedStream, supportMediaResource, supportNamedStream, visibleStreamType, request);
                            }
                        });
                    }
                });
            }

            #endregion EF Metadata Tests

            #region Reflection Metadata Tests
            
            [NamedStream("Stream2")]
            [NamedStream("Stream1")]
            public class Base1NonEntityWithNamedStream
            {
            }

            public class Base2EntityWithNoNamedStream : Base1NonEntityWithNamedStream
            {
                public int ID { get; set; }
            }

            [NamedStream("Stream1")]
            public class EntityWithConflictStream
            {
                public int ID { get; set; }
                public string Stream1 { get; set; }
            }

            [NamedStream("Stream3")]
            [NamedStream("Stream4")]
            public class RootEntityType : Base2EntityWithNoNamedStream
            {
            }

            [HasStream]
            public class Derived1WithBlob : RootEntityType
            {
            }

            public class Derived2 : Derived1WithBlob
            {
            }

            [TestMethod]
            public void NamedStreamReflectionConflictStreamProperty()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = typeof(TypedCustomDataContext<EntityWithConflictStream>);
                    request.RequestUriString = "/$metadata";
                    request.HttpMethod = "GET";
                    TestUtil.RunCatching(request.SendRequest);
                    string response = request.GetResponseStreamAsText();
                    TestUtil.AssertContains(response, DataServicesResourceUtil.GetString("ResourceType_PropertyWithSameNameAlreadyExists",
                        "Stream1", "AstoriaUnitTests.Tests.NamedStreamUnitTestModule_NamedStreamTests_EntityWithConflictStream"));
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod]
            public void NamedStreamReflectionNamedStreamOnDerivedTypes()
            {
                using (TestUtil.RestoreStaticValueOnDispose(typeof(TypedCustomDataContext<Base2EntityWithNoNamedStream>), "PreserveChanges"))
                using (TestUtil.RestoreStaticValueOnDispose(typeof(OpenWebDataServiceHelper), "ForceVerboseErrors"))
                using (OpenWebDataServiceHelper.GetServiceCustomizer.Restore())
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    OpenWebDataServiceHelper.ForceVerboseErrors = true;
                    OpenWebDataServiceHelper.GetServiceCustomizer.Value = type =>
                    {
                        if (type == typeof(providers.IDataServiceStreamProvider2))
                        {
                            return new TypedCustomStreamProvider2<Base2EntityWithNoNamedStream>(new DSPMediaResourceStorage());
                        }

                        if (type == typeof(providers.IDataServiceStreamProvider))
                        {
                            return new TypedCustomStreamProvider2<Base2EntityWithNoNamedStream>(new DSPMediaResourceStorage());
                        }

                        return null;
                    };

                    TypedCustomDataContext<Base2EntityWithNoNamedStream>.ClearHandlers();
                    TypedCustomDataContext<Base2EntityWithNoNamedStream>.ClearValues();
                    TypedCustomDataContext<Base2EntityWithNoNamedStream>.PreserveChanges = true;
                    TypedCustomDataContext<Base2EntityWithNoNamedStream>.ValuesRequested += (sender, args) =>
                    {
                        TypedCustomDataContext<Base2EntityWithNoNamedStream> typedContext = (TypedCustomDataContext<Base2EntityWithNoNamedStream>)sender;
                        typedContext.SetValues(new Base2EntityWithNoNamedStream[] { new Base2EntityWithNoNamedStream { ID = 1 }, new RootEntityType { ID = 2 } });
                    };

                    request.DataServiceType = typeof(TypedCustomDataContext<Base2EntityWithNoNamedStream>);
                    request.RequestUriString = "/$metadata";
                    request.HttpMethod = "GET";
                    request.SendRequest();

                    string base2EntityWithNoNamedStreamTypeName = "NamedStreamUnitTestModule_NamedStreamTests_Base2EntityWithNoNamedStream";
                    string rootEntityTypeName = "NamedStreamUnitTestModule_NamedStreamTests_RootEntityType";
                    string derived1WithBlobTypeName = "NamedStreamUnitTestModule_NamedStreamTests_Derived1WithBlob";
                    string typeNamespace = "AstoriaUnitTests.Tests.";

                    Assert.AreEqual("4.0;", request.ResponseVersion);
                    UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), UnitTestsUtil.MimeApplicationXml, new string[] {
                        String.Format("//csdl:EntityType[@Name='{0}' and csdl:Property[@Name='Stream1' and @Type='Edm.Stream'] and csdl:Property[@Name='Stream2' and @Type='Edm.Stream']]", base2EntityWithNoNamedStreamTypeName),
                        String.Format("//csdl:EntityType[@Name='{0}' and @BaseType='{1}' and csdl:Property[@Name='Stream3' and @Type='Edm.Stream'] and csdl:Property[@Name='Stream4' and @Type='Edm.Stream']]", rootEntityTypeName, typeNamespace + base2EntityWithNoNamedStreamTypeName),
                        String.Format("//csdl:EntityType[@Name='{0}' and @HasStream='true' and @BaseType='{1}']", derived1WithBlobTypeName, typeNamespace + rootEntityTypeName),
                    });

                    string setUri = request.ServiceRoot + "/Values";
                    string[] jsonXPaths = new string[]
                    {
                        String.Format(
                            "//{0}[__metadata/type='{1}' and Stream1/__mediaresource/edit_media='{2}/Stream1' and Stream2/__mediaresource/edit_media='{2}/Stream2']", 
                            JsonValidator.ObjectString,
                            typeNamespace + base2EntityWithNoNamedStreamTypeName,
                            setUri + "(1)"),
                        String.Format(
                            "//{0}[__metadata/type='{1}' and Stream1/__mediaresource/edit_media='{2}/{1}/Stream1' and Stream2/__mediaresource/edit_media='{2}/{1}/Stream2' and Stream3/__mediaresource/edit_media='{2}/{1}/Stream3' and Stream4/__mediaresource/edit_media='{2}/{1}/Stream4']", 
                            JsonValidator.ObjectString,
                            typeNamespace + rootEntityTypeName,
                            setUri + "(2)"),
                    };

                    string[] atomXPaths = new string[] {
                        String.Format(
                            "//atom:entry[atom:category/@term='#{0}' and atom:link[@title='Stream1' and @href='Values(1)/Stream1'] and atom:link[@title='Stream2' and @href='Values(1)/Stream2']]",
                            typeNamespace + base2EntityWithNoNamedStreamTypeName),
                        String.Format(
                            "//atom:entry[atom:category/@term='#{0}' and " +
                            "atom:link[@title='Stream1' and @rel='{1}/Stream1' and @href='Values(2)/{0}/Stream1'] and " +
                            "atom:link[@title='Stream2' and @rel='{1}/Stream2' and @href='Values(2)/{0}/Stream2'] and " +
                            "atom:link[@title='Stream3' and @rel='{1}/Stream3' and @href='Values(2)/{0}/Stream3'] and " +
                            "atom:link[@title='Stream4' and @rel='{1}/Stream4' and @href='Values(2)/{0}/Stream4']]",
                            typeNamespace + rootEntityTypeName,
                            "http://docs.oasis-open.org/odata/ns/edit-media"),
                    };

                    foreach (string format in UnitTestsUtil.ResponseFormats)
                    {
                        request.RequestUriString = "/Values";
                        request.HttpMethod = "GET";
                        request.Accept = format;
                        request.SendRequest();
                        Assert.AreEqual("4.0;", request.ResponseVersion);
                        UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), format, null, jsonXPaths, atomXPaths);
                    }
                }
            }

            [TestMethod]
            public void NamedStreamReflectionMetadataSerializationTest()
            {
                using (TestUtil.RestoreStaticValueOnDispose(typeof(OpenWebDataServiceHelper), "ForceVerboseErrors"))
                using (OpenWebDataServiceHelper.GetServiceCustomizer.Restore())
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    OpenWebDataServiceHelper.ForceVerboseErrors = true;

                    OpenWebDataServiceHelper.GetServiceCustomizer.Value = type =>
                    {
                        if (type == typeof(providers.IDataServiceStreamProvider2))
                        {
                            return new DSPStreamProvider2(new DSPMediaResourceStorage());
                        }

                        if (type == typeof(providers.IDataServiceStreamProvider))
                        {
                            return new DSPStreamProvider2(new DSPMediaResourceStorage());
                        }

                        return null;
                    };

                    request.DataServiceType = typeof(TypedCustomDataContext<RootEntityType>);
                    request.RequestUriString = "/$metadata";
                    request.HttpMethod = "GET";
                    request.SendRequest();

                    Assert.AreEqual("4.0;", request.ResponseVersion);
                    XDocument edmMetadata = UnitTestsUtil.GetResponseAsAtomXLinq(request);
                    
                    var expectedNameStreams = new Dictionary<String, String[]>();
                    expectedNameStreams.Add("NamedStreamUnitTestModule_NamedStreamTests_Base2EntityWithNoNamedStream", new string[] { "Stream1", "Stream2" });
                    expectedNameStreams.Add("NamedStreamUnitTestModule_NamedStreamTests_RootEntityType", new string[] { "Stream3", "Stream4" });
                    
                    VerifyStreamsInMetadata(edmMetadata, 4, new string[] { "NamedStreamUnitTestModule_NamedStreamTests_Derived1WithBlob" }, expectedNameStreams);
                }
            }

            private static void VerifyStreamsInMetadata(XDocument edmMetadata, int entityTypeCount,
                                                        String[] typesWithBlob,
                                                        Dictionary<String, String[]> typesWithNamedStream)
            {
                XElement edmx = edmMetadata.Element(UnitTestsUtil.EdmxNamespace + "Edmx");
                XElement dataServices = edmx.Element(UnitTestsUtil.EdmxNamespace + "DataServices");

                XElement schema;
                XElement[] entityTypes;
                schema = dataServices.Element(UnitTestsUtil.EdmOasisNamespace + "Schema");
                entityTypes = schema.Elements(UnitTestsUtil.EdmOasisNamespace + "EntityType").ToArray();

                Assert.AreEqual(entityTypeCount, entityTypes.Count());

                foreach (XElement entityType in entityTypes)
                {
                    String typeName = entityType.Attribute(NameAttribute).Value;
                    Assert.IsNotNull(typeName);

                    // verify BLOB
                    if (typesWithBlob.Contains(typeName, StringComparer.Ordinal))
                    {
                        Assert.IsNotNull(entityType.Attribute(HasStreamAttribute));
                    }
                    else
                    {
                        Assert.IsNull(entityType.Attribute(HasStreamAttribute));
                    }

                    // verify properties and NamedStream
                    String[] expectedNamedStreams;

                    bool seenNavigationProperty = false;
                    bool hasNamedStream = typesWithNamedStream.TryGetValue(typeName, out expectedNamedStreams);
                    int namedStreamsCount = 0;

                    var expectedPropertyName = UnitTestsUtil.EdmOasisNamespace;

                    foreach (var propertyNode in entityType.Elements())
                    {
                        if (propertyNode.Name == expectedPropertyName + "Property")
                        {
                            // property should come before navigation properties
                            Assert.IsFalse(seenNavigationProperty, "property should come before navigation properties");
                            string propName = propertyNode.Attributes("Name").Single().Value;
                            string propType = propertyNode.Attributes("Type").Single().Value;

                            if (propType.Equals("Edm.Stream", StringComparison.Ordinal))
                            {
                                Assert.IsTrue(hasNamedStream);
                                namedStreamsCount++;
                                Assert.IsTrue(expectedNamedStreams.Contains(propName));

                                Assert.IsTrue(
                                    propertyNode.Attribute(NullableAttribute) != null &&
                                    propertyNode.Attribute(NullableAttribute).Value == XmlFalseLiteral,
                                    "A stream property must have the Nullable=\"false\" attribute specified.");
                            }
                        }
                        else if (propertyNode.Name == expectedPropertyName + "NavigationProperty")
                        {
                            seenNavigationProperty = true;
                        }
                        else
                        {
                            Assert.AreEqual(expectedPropertyName + "Key", propertyNode.Name);
                        }
                    }

                    Assert.AreEqual(hasNamedStream ? expectedNamedStreams.Count() : 0, namedStreamsCount,
                                    "Incorrect number of named streams");
                }
            }

            public class EntityWithNoStream
            {
                public int ID { get; set; }
            }

            [HasStream]
            public class MLE
            {
                public int ID { get; set; }
            }

            [NamedStream("Stream1")]
            public class EntityWithNamedStream
            {
                public int ID { get; set; }
                public string Name { get; set; }
            }

            [HasStream]
            [NamedStream("Stream1")]
            public class MLEWithNamedStream
            {
                public int ID { get; set; }
            }
            [Ignore] // Remove Atom
            // [TestMethod]
            public void NamedStreamReflectionMetadataStreamProviderValidationTest()
            {
                TestUtil.RunCombinations(BooleanValues, BooleanValues, BooleanValues, BooleanValues, BooleanValues, ProtocolVersions,
                    (containsMediaResource, containsNamedStream, supportMediaResource, supportNamedStream, visibleStreamType, maxProtocolVersion) =>
                {
                    using (TestUtil.RestoreStaticValueOnDispose(typeof(OpenWebDataServiceHelper), "ForceVerboseErrors"))
                    using (OpenWebDataServiceHelper.EntitySetAccessRule.Restore())
                    using (OpenWebDataServiceHelper.MaxProtocolVersion.Restore())
                    using (OpenWebDataServiceHelper.GetServiceCustomizer.Restore())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                    {
                        OpenWebDataServiceHelper.ForceVerboseErrors = true;

                        bool IDSSP2Loaded = false;
                        bool IDSSP1Loaded = false;

                        OpenWebDataServiceHelper.GetServiceCustomizer.Value = type =>
                        {
                            if (type == typeof(providers.IDataServiceStreamProvider2) && supportNamedStream)
                            {
                                if (maxProtocolVersion < ODataProtocolVersion.V4)
                                {
                                    throw new InvalidOperationException("If MaxProtocolVersion < V3, GetService<IDSSP2>() must not be called.");
                                }

                                if (IDSSP2Loaded)
                                {
                                    throw new InvalidOperationException("Cannot load IDSSP2 twice.");
                                }

                                if (IDSSP1Loaded)
                                {
                                    throw new InvalidOperationException("If MaxProtocolVersion >= V3, GetService<IDSSP2>() must be called before GetService<IDSSP>().");
                                }

                                IDSSP2Loaded = true;
                                return new DSPStreamProvider2(new DSPMediaResourceStorage());
                            }

                            if (type == typeof(providers.IDataServiceStreamProvider) && supportMediaResource)
                            {
                                if (IDSSP1Loaded)
                                {
                                    throw new InvalidOperationException("Cannot load IDSSP twice.");
                                }

                                if (maxProtocolVersion >= ODataProtocolVersion.V4)
                                {
                                    if (!IDSSP2Loaded && supportNamedStream)
                                    {
                                        throw new InvalidOperationException("If MaxProtocolVersion >= V3, GetService<IDSSP2>() must be called before GetService<IDSSP>().");
                                    }
                                }
                                else
                                {
                                    if (IDSSP2Loaded)
                                    {
                                        throw new InvalidOperationException("If MaxProtocolVersion < V3, GetService<IDSSP2>() must not be called.");
                                    }
                                }

                                return new DSPStreamProvider(new DSPMediaResourceStorage());
                            }

                            return null;
                        };

                        OpenWebDataServiceHelper.EntitySetAccessRule.Value = new Dictionary<string, EntitySetRights>()
                        {
                            { "Values", EntitySetRights.ReadSingle },
                        };

                        if (!visibleStreamType)
                        {
                            OpenWebDataServiceHelper.EntitySetAccessRule.Value["Values"] = EntitySetRights.None;
                        }

                        OpenWebDataServiceHelper.MaxProtocolVersion.Value = maxProtocolVersion;
                        TestUtil.ClearConfiguration();
                        if (!containsMediaResource && !containsNamedStream)
                        {
                            request.DataServiceType = typeof(TypedCustomDataContext<EntityWithNoStream>);
                        }
                        else if (containsMediaResource && !containsNamedStream)
                        {
                            request.DataServiceType = typeof(TypedCustomDataContext<MLE>);
                        }
                        else if (!containsMediaResource && containsNamedStream)
                        {
                            request.DataServiceType = typeof(TypedCustomDataContext<EntityWithNamedStream>);
                        }
                        else
                        {
                            Assert.IsTrue(containsMediaResource && containsNamedStream);
                            request.DataServiceType = typeof(TypedCustomDataContext<MLEWithNamedStream>);
                        }

                        this.MetadataStreamProviderValidation(containsMediaResource, containsNamedStream, supportMediaResource, supportNamedStream, visibleStreamType, request);
                    }
                });
            }

            #endregion Reflection Metadata Tests

            #endregion Metadata Runtime Tests

            #region Uri Parsing Tests
            [Ignore] // Remove Atom
            // [TestMethod]
            public void NamedStreamUriNegativeTest()
            {
                DSPMetadata metadata = new DSPMetadata("NamedStreamIDSPBasicScenariosTest", "NamedStreamTests");
                providers.ResourceType entityType = metadata.AddEntityType("EntityType", null, null, false);
                metadata.AddKeyProperty(entityType, "ID", typeof(int));
                entityType.AddProperty(new providers.ResourceProperty("Stream1", providers.ResourcePropertyKind.Stream, providers.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream))));
                metadata.AddResourceSet("MySet", entityType);

                DSPServiceDefinition service = new DSPServiceDefinition();
                service.Metadata = metadata;
                service.Writable = true;
                DSPContext context = new DSPContext();

                DSPResource entity = new DSPResource(entityType);
                entity.SetValue("ID", 1);
                context.GetResourceSetEntities("MySet").Add(entity);

                service.CreateDataSource = (m) =>
                {
                    return context;
                };

                // Cannot compose after the NamedStream segment
                var requestUris = new string[]
                {
                    "/MySet(1)/Stream1/Something",
                    "/MySet(1)/Stream1/$value",
                    "/MySet(1)/Stream1/$count"
                };

                TestUtil.RunCombinations(requestUris, (requestUri) =>
                {
                    using (TestWebRequest request = service.CreateForInProcess())
                    {
                        request.HttpMethod = "GET";
                        request.RequestUriString = requestUri;
                        WebException e = (WebException)TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNotNull(e);
                        Assert.AreEqual(404, request.ResponseStatusCode);
                        Assert.IsInstanceOfType(e.InnerException, typeof(DataServiceException));
                        TestUtil.AssertContains(e.InnerException.Message, ODataLibResourceUtil.GetString("RequestUriProcessor_MustBeLeafSegment", "Stream1"));
                    }
                });

                // Query options are not allowed.
                requestUris = new string[]
                {
                    "/MySet(1)/Stream1?$select=ID",
                    "/MySet(1)/Stream1?$expand=*",
                    "/MySet(1)/Stream1?$filter=ID eq 0",
                    "/MySet(1)/Stream1?$orderby=ID",
                    "/MySet(1)/Stream1?$skiptoken=ID",
                    "/MySet(1)/Stream1?$top=1",
                    "/MySet(1)/Stream1?$skip=1",
                    "/MySet(1)/Stream1?$count=true",    
                };

                TestUtil.RunCombinations(requestUris, (requestUri) =>
                {
                    using (TestWebRequest request = service.CreateForInProcess())
                    {
                        request.HttpMethod = "GET";
                        request.RequestUriString = requestUri;
                        WebException e = (WebException)TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNotNull(e);
                        Assert.AreEqual(400, request.ResponseStatusCode);
                        Assert.IsInstanceOfType(e.InnerException, typeof(DataServiceException));
                        TestUtil.AssertContains(e.InnerException.Message, DataServicesResourceUtil.GetString("RequestQueryProcessor_QueryNoOptionsApplicable"));
                    }
                });

                // Only GET and POST are allowed on named streams, all other methods are not allowd
                var requestMethods = new string[] { "HEAD", "PATCH", "POST", "DELETE" };
                TestUtil.RunCombinations(requestMethods, (requestMethod) =>
                {
                    using (TestWebRequest request = service.CreateForInProcessWcf())
                    {
                        request.HttpMethod = requestMethod;
                        request.RequestUriString = "/MySet(1)/Stream1";
                        request.RequestContentLength = 0;
                        WebException e = (WebException)TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNotNull(e);
                        if (requestMethod == "POST" || requestMethod == "PATCH" || requestMethod == "DELETE")
                        {
                            Assert.AreEqual(405, request.ResponseStatusCode);
                            Assert.AreEqual("The remote server returned an error: (405) Method Not Allowed.", e.Message);
                            Assert.AreEqual("GET,PUT", request.ResponseHeaders["Allow"]);
                            TestUtil.AssertContains((new StreamReader(e.Response.GetResponseStream())).ReadToEnd(), DataServicesResourceUtil.GetString("RequestUriProcessor_InvalidHttpMethodForNamedStream", request.FullRequestUriString, requestMethod));
                        }
                        else
                        {
                            Assert.AreEqual(501, request.ResponseStatusCode);
                            Assert.AreEqual("The remote server returned an error: (501) Not Implemented.", e.Message);
                        }
                    }
                });
            }

            #endregion Uri Parsing Tests

            #region Main Scenario Functional Tests

            #region IDSP Main Scenario Functional Tests
            [Ignore] // Remove Atom
            // [TestMethod]
            public void NamedStreamIDSPPostEntityTest()
            {
                IDSPBasicScenariosCombinations(IDSPPostNamedStreamEntity);
            }

            private void IDSPBasicScenariosCombinations(Action<DSPServiceDefinition, providers.ResourceType, bool, string, ODataProtocolVersion, byte[], byte[]> test)
            {
                TestUtil.RunCombinations(BooleanValues, UnitTestsUtil.ResponseFormats, ProtocolVersions,
                    (isMediaLinkEntry, mimeType, maxProtocolVersion) =>
                {
                    DSPMetadata metadata = new DSPMetadata("NamedStreamIDSPBasicScenariosTest", "NamedStreamTests");
                    providers.ResourceType entityType = metadata.AddEntityType("EntityType", null, null, false);
                    entityType.IsMediaLinkEntry = isMediaLinkEntry;
                    metadata.AddKeyProperty(entityType, "ID", typeof(int));
                    metadata.AddPrimitiveProperty(entityType, "Name", typeof(string));
                    entityType.AddProperty(new providers.ResourceProperty("Stream1", providers.ResourcePropertyKind.Stream, providers.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream))));
                    entityType.AddProperty(new providers.ResourceProperty("Stream2", providers.ResourcePropertyKind.Stream, providers.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream))));
                    metadata.AddResourceSet("MySet", entityType);

                    DSPServiceDefinition service = new DSPServiceDefinition();
                    service.ForceVerboseErrors = true;
                    service.Metadata = metadata;
                    service.Writable = true;
                    service.MediaResourceStorage = new DSPMediaResourceStorage();
                    service.SupportMediaResource = true;
                    service.SupportNamedStream = true;
                    DSPContext context = new DSPContext();
                    service.CreateDataSource = (m) =>
                    {
                        return context;
                    };

                    byte[] defaultStreamBuffer = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                    byte[] stream1Buffer = new byte[] { 1, 2, 3, 4 };

                    using (OpenWebDataServiceHelper.MaxProtocolVersion.Restore())
                    {
                        OpenWebDataServiceHelper.MaxProtocolVersion.Value = maxProtocolVersion;
                        test(service, entityType, isMediaLinkEntry, mimeType, maxProtocolVersion, defaultStreamBuffer, stream1Buffer);
                    }
                });
            }

            private void IDSPPostNamedStreamEntity(
                DSPServiceDefinition service,
                providers.ResourceType entityType,
                bool isMediaLinkEntry,
                string mimeType,
                ODataProtocolVersion maxProtocolVersion,
                byte[] defaultStreamBuffer,
                byte[] stream1Buffer)
            {
                ////////////////////////////////////////////////////////////////
                // Create Entity
                ////////////////////////////////////////////////////////////////
                using (TestWebRequest request = service.CreateForInProcessWcf())
                {
                    request.HttpMethod = "POST";
                    request.RequestUriString = "/MySet";
                    request.Accept = mimeType;
                    request.RequestVersion = "4.0";
                    request.RequestMaxVersion = "4.0";

                    if (isMediaLinkEntry)
                    {
                        request.RequestContentType = "MyType1/MySubType1";
                        request.RequestStream = new MemoryStream(defaultStreamBuffer);
                        request.RequestHeaders["slug"] = "111";
                        request.RequestHeaders[DSPStreamProvider2.ResolveTypeHeaderName] = entityType.FullName;
                    }
                    else
                    {
                        DSPResource entity = new DSPResource(entityType);
                        entity.SetValue("ID", 111);
                        string payload = DSPResourceSerializer.WriteEntity(entity, mimeType == UnitTestsUtil.AtomFormat ? DSPResourceSerializerFormat.Atom : DSPResourceSerializerFormat.Json);

                        // POST entity
                        request.RequestContentType = mimeType;
                        request.SetRequestStreamAsText(payload);
                    }

                    WebException webException = (WebException)TestUtil.RunCatching(request.SendRequest);

                    if (webException == null)
                    {
                        // Validate response
                        XmlDocument atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        List<string> xpaths = new List<string>()
                            {
                                "boolean(/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/edit-media/Stream1' and @title='Stream1' and contains(@href, '/Stream1') and substring-after(@href, '/Stream1')='' and not(@adsm:etag) and not(@type)])",
                                "boolean(/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/edit-media/Stream2' and @title='Stream2' and contains(@href, '/Stream2') and substring-after(@href, '/Stream2')='' and not(@adsm:etag) and not(@type)])",
                            };

                        if (isMediaLinkEntry)
                        {
                            // media resource from local storage
                            DSPResource resource = (DSPResource)service.CurrentDataSource.GetResourceSetEntities("MySet").Single(e => ((int)((DSPResource)e).GetValue("ID")) == 111);
                            DSPMediaResource mediaResource = null;
                            
                            Assert.IsTrue(service.MediaResourceStorage.TryGetMediaResource(resource, null, out mediaResource));
                            bool isEmptyStream;
                            Assert.IsTrue(TestUtil.CompareStream(mediaResource.GetReadStream(out isEmptyStream), new MemoryStream(defaultStreamBuffer)));
                            xpaths.Add("boolean(/atom:entry/atom:link[@rel='edit-media' and contains(@href, '/$value') and substring-after(@href, '/$value')='' and @adsm:etag='" + mediaResource.Etag + "'])");
                            xpaths.Add("boolean(/atom:entry/atom:content[@type='MyType1/MySubType1' and contains(@src, '/$value') and substring-after(@src, '/$value')=''])");
                            xpaths.Add("boolean(/atom:entry/adsm:properties/ads:ID=111)");
                            xpaths.Add("boolean(/atom:entry/adsm:properties/ads:Name[@adsm:null='true'])");
                            xpaths.Add("not    (/atom:entry/adsm:properties/ads:Stream1)");
                            xpaths.Add("not    (/atom:entry/adsm:properties/ads:Stream2)");
                        }
                        else
                        {
                            xpaths.Add("not(/atom:entry/atom:link[@rel='edit-media'])");
                            xpaths.Add("boolean(/atom:entry/atom:content[@type='application/xml' and not(@src)])");
                            xpaths.Add("boolean(/atom:entry/atom:content/adsm:properties/ads:ID=111)");
                            xpaths.Add("boolean(/atom:entry/atom:content/adsm:properties/ads:Name[@adsm:null='true'])");
                            xpaths.Add("not    (/atom:entry/atom:content/adsm:properties/ads:Stream1)");
                            xpaths.Add("not    (/atom:entry/atom:content/adsm:properties/ads:Stream2)");
                        }

                        UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, xpaths.ToArray());
                    }
                    else
                    {
                        Assert.Fail("Unexpected failure in POST");
                    }
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod]
            public void NamedStreamIDSPGetAndPutDefaultStreamTest()
            {
                IDSPBasicScenariosCombinations(IDSPGetAndPutDefaultStream);
            }

            private void IDSPGetAndPutDefaultStream(
                DSPServiceDefinition service,
                providers.ResourceType entityType,
                bool isMediaLinkEntry,
                string mimeType,
                ODataProtocolVersion maxProtocolVersion,
                byte[] defaultStreamBuffer,
                byte[] stream1Buffer)
            {
                DSPResource resource = new DSPResource(entityType);
                resource.SetValue("ID", 111);
                DSPMediaResource mediaResource = null;

                // instance in the cache
                service.CurrentDataSource.GetResourceSetEntities("MySet").Add(resource);
                if (isMediaLinkEntry)
                {
                    mediaResource = service.MediaResourceStorage.CreateMediaResource(resource, null);
                    mediaResource.GetWriteStream().Write(defaultStreamBuffer, 0, defaultStreamBuffer.Length);
                    mediaResource.ContentType = "MyType1/MySubType1";
                    mediaResource.Etag = DSPMediaResource.GenerateStreamETag();
                }

                ////////////////////////////////////////////////////////////////
                // Attempt to GET the default stream
                ////////////////////////////////////////////////////////////////
                using (TestWebRequest request = service.CreateForInProcessWcf())
                {
                    request.HttpMethod = "GET";
                    request.RequestUriString = "/MySet(111)/$value";
                    request.RequestStream = null;
                    request.RequestVersion = "4.0";
                    request.RequestMaxVersion = "4.0";

                    WebException e = (WebException)TestUtil.RunCatching(request.SendRequest);
                    this.VerifyExceptionForIDSPGetAndPutDefaultStream(e, isMediaLinkEntry, request, () =>
                    {
                        Assert.AreEqual(200, request.ResponseStatusCode);
                        Assert.AreEqual(mediaResource.Etag, request.ResponseETag);
                        Assert.AreEqual(mediaResource.ContentType, request.ResponseContentType);
                        Assert.IsTrue(TestUtil.CompareStream(request.GetResponseStream(), new MemoryStream(defaultStreamBuffer)));
                    });
                }

                ////////////////////////////////////////////////////////////////
                // Attempt to PUT the default stream
                ////////////////////////////////////////////////////////////////
                using (TestWebRequest request = service.CreateForInProcessWcf())
                {
                    request.HttpMethod = "PUT";
                    request.RequestUriString = "/MySet(111)/$value";
                    request.RequestContentType = "MyType2/MySubType2";
                    request.RequestVersion = "4.0";
                    request.RequestMaxVersion = "4.0";

                    if (isMediaLinkEntry)
                    {
                        request.IfMatch = mediaResource.Etag;
                    }

                    defaultStreamBuffer = new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 };
                    request.RequestStream = new MemoryStream(defaultStreamBuffer);
                    WebException e = (WebException)TestUtil.RunCatching(request.SendRequest);
                    this.VerifyExceptionForIDSPGetAndPutDefaultStream(e, isMediaLinkEntry, request, () =>
                    {
                        Assert.AreEqual(204, request.ResponseStatusCode);
                        Assert.AreNotEqual(mediaResource.Etag, request.IfMatch);
                        Assert.AreEqual(mediaResource.Etag, request.ResponseETag);
                        Assert.AreEqual(mediaResource.ContentType, request.RequestContentType);
                        bool isEmptyStream;
                        Assert.IsTrue(TestUtil.CompareStream(mediaResource.GetReadStream(out isEmptyStream), new MemoryStream(defaultStreamBuffer)));
                    });
                }
            }

            [TestMethod]
            public void NamedStreamIDSPGetAndPutNamedStreamTest()
            {
                IDSPBasicScenariosCombinations(IDSPGetAndPutNamedStream);
            }

            private void VerifyExceptionForIDSPGetAndPutDefaultStream(WebException e, bool isMediaLinkEntry, TestWebRequest request, Action successValidator)
            {
                TestUtil.AssertExceptionExpected(e, !isMediaLinkEntry);
                if (e == null)
                {
                    successValidator();
                }
                else
                {
                    string responseText = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                    Assert.AreEqual(400, request.ResponseStatusCode);
                    TestUtil.AssertContains(responseText, DataServicesResourceUtil.GetString("BadRequest_InvalidUriForMediaResource", request.FullRequestUriString));
                }
            }

            private void IDSPGetAndPutNamedStream(
                DSPServiceDefinition service,
                providers.ResourceType entityType,
                bool isMediaLinkEntry,
                string mimeType,
                ODataProtocolVersion maxProtocolVersion,
                byte[] defaultStreamBuffer,
                byte[] stream1Buffer)
            {
                DSPResource resource = new DSPResource(entityType);
                resource.SetValue("ID", 111);
                DSPMediaResource mediaResource = null;
                service.CurrentDataSource.GetResourceSetEntities("MySet").Add(resource);

                ////////////////////////////////////////////////////////////////
                // GET null named stream
                ////////////////////////////////////////////////////////////////
                using (TestWebRequest request = service.CreateForInProcessWcf())
                {
                    request.HttpMethod = "GET";
                    request.RequestUriString = "/MySet(111)/Stream1";
                    request.RequestStream = null;
                    request.RequestVersion = "4.0";
                    request.RequestMaxVersion = "4.0";

                    WebException webException = (WebException)TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(webException, maxProtocolVersion < ODataProtocolVersion.V4);

                    if (webException == null)
                    {
                        Assert.AreEqual(204, request.ResponseStatusCode);
                        Assert.IsTrue(string.IsNullOrEmpty(request.ResponseContentType));
                        Assert.AreEqual(0, request.GetResponseStreamAsText().Length);
                    }
                    else
                    {
                        Assert.Fail("Unexpected failure in POST");
                    }
                }

                ////////////////////////////////////////////////////////////////
                // PUT named stream
                ////////////////////////////////////////////////////////////////
                using (TestWebRequest request = service.CreateForInProcessWcf())
                {
                    request.HttpMethod = "PUT";
                    request.RequestUriString = "/MySet(111)/Stream1";
                    request.RequestContentType = "MyType/MySubType";
                    request.RequestVersion = "4.0";
                    request.RequestMaxVersion = "4.0";

                    request.RequestStream = new MemoryStream(stream1Buffer);
                    WebException webException = (WebException)TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(webException, maxProtocolVersion < ODataProtocolVersion.V4);

                    if (webException == null)
                    {
                        Assert.AreEqual(204, request.ResponseStatusCode);
                        Assert.IsTrue(string.IsNullOrEmpty(request.ResponseContentType));
                        Assert.AreEqual(0, request.GetResponseStreamAsText().Length);
                        Assert.IsTrue(service.MediaResourceStorage.TryGetMediaResource(resource, entityType.GetNamedStreams().Single(s => s.Name == "Stream1"), out mediaResource));
                        bool isEmptyStream;
                        Assert.IsTrue(TestUtil.CompareStream(mediaResource.GetReadStream(out isEmptyStream), new MemoryStream(stream1Buffer)));
                        Assert.AreEqual(mediaResource.ContentType, request.RequestContentType);
                        Assert.IsNotNull(mediaResource.Etag);                        
                    }
                    else
                    {
                        Assert.Fail("Unexpected failure in POST");
                    }
                }

                ////////////////////////////////////////////////////////////////
                // GET named stream
                ////////////////////////////////////////////////////////////////
                using (TestWebRequest request = service.CreateForInProcessWcf())
                {
                    request.HttpMethod = "GET";
                    request.RequestUriString = "/MySet(111)/Stream1";
                    request.RequestVersion = "4.0";
                    request.RequestMaxVersion = "4.0";

                    WebException webException = (WebException)TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(webException, maxProtocolVersion < ODataProtocolVersion.V4);

                    if (webException == null)
                    {
                        Assert.AreEqual(200, request.ResponseStatusCode);
                        Assert.AreEqual("MyType/MySubType", request.ResponseContentType);
                        Assert.IsTrue(TestUtil.CompareStream(request.GetResponseStream(), new MemoryStream(stream1Buffer)));
                        Assert.AreEqual(mediaResource.ContentType, request.ResponseContentType);
                        Assert.AreEqual(mediaResource.Etag, request.ResponseETag);
                    }
                    else
                    {
                        Assert.Fail("Unexpected failure in POST");
                    }
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod]
            public void NamedStreamIDSPGetDeleteEntityTest()
            {
                IDSPBasicScenariosCombinations(IDSPGetDeleteEntity);
            }

            private void IDSPGetDeleteEntity(
                DSPServiceDefinition service,
                providers.ResourceType entityType,
                bool isMediaLinkEntry,
                string mimeType,
                ODataProtocolVersion maxProtocolVersion,
                byte[] defaultStreamBuffer,
                byte[] stream1Buffer)
            {
                DSPResource resource = new DSPResource(entityType);
                resource.SetValue("ID", 111);
                DSPMediaResource mediaResource = null;

                // instance in the cache
                service.CurrentDataSource.GetResourceSetEntities("MySet").Add(resource);
                if (isMediaLinkEntry)
                {
                    mediaResource = service.MediaResourceStorage.CreateMediaResource(resource, null);
                    mediaResource.GetWriteStream().Write(defaultStreamBuffer, 0, defaultStreamBuffer.Length);
                    mediaResource.ContentType = "MyType1/MySubType1";
                    mediaResource.Etag = DSPMediaResource.GenerateStreamETag();
                }

                // add Stream1 to the storage
                mediaResource = service.MediaResourceStorage.CreateMediaResource(resource, entityType.GetNamedStreams().Single(s => s.Name == "Stream1"));
                mediaResource.GetWriteStream().Write(stream1Buffer, 0, stream1Buffer.Length);
                mediaResource.ContentType = "MyType2/MySubType2";
                mediaResource.Etag = DSPMediaResource.GenerateStreamETag();

                ////////////////////////////////////////////////////////////////
                // GET entity
                ////////////////////////////////////////////////////////////////
                using (TestWebRequest request = service.CreateForInProcessWcf())
                {
                    request.HttpMethod = "GET";
                    request.RequestUriString = "/MySet(111)";
                    request.Accept = mimeType;
                    request.RequestVersion = "4.0";
                    request.RequestMaxVersion = "4.0";

                    WebException webException = (WebException)TestUtil.RunCatching(request.SendRequest);

                    if (webException == null)
                    {
                        // Validate response
                        XmlDocument atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        string[] xpaths = new string[]
                            {
                                "boolean(/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/edit-media/Stream1' and @title='Stream1' and contains(@href, '/Stream1') and substring-after(@href, '/Stream1')='' and @adsm:etag='" + mediaResource.Etag + "' and @type='" + mediaResource.ContentType + "'])",
                                "boolean(/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/edit-media/Stream2' and @title='Stream2' and contains(@href, '/Stream2') and substring-after(@href, '/Stream2')='' and not(@adsm:etag) and not(@type)])",
                            };

                        UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, xpaths);
                    }
                    else
                    {
                        Assert.Fail("Unexpected failure in GET");
                    }
                }

                ////////////////////////////////////////////////////////////////
                // DELETE entity
                ////////////////////////////////////////////////////////////////
                using (TestWebRequest request = service.CreateForInProcessWcf())
                {
                    request.HttpMethod = "DELETE";
                    request.RequestUriString = "/MySet(111)";
                    request.RequestVersion = "4.0";
                    request.RequestMaxVersion = "4.0";

                    WebException webException = (WebException)TestUtil.RunCatching(request.SendRequest);
                    if (webException == null)
                    {
                        Assert.AreEqual(204, request.ResponseStatusCode);
                        Assert.IsFalse(service.MediaResourceStorage.Content.Any(mle => mle.Key == resource));
                        Assert.IsFalse(service.CurrentDataSource.GetResourceSetEntities("MySet").Any());
                        Assert.AreEqual(0, service.MediaResourceStorage.Content.Count());
                    }
                    else
                    {
                        // No need to validate against DSV and MDSV since for delete requests, these will always be 1.0
                        Assert.Fail("Unexpected failure in DELETE");
                    }
                }
            }
            
            #endregion IDSP Main Scenario Functional Tests

            [TestMethod]
            public void NamedStreamEFGetAndPutNamedStreamTest()
            {
                EFBasicScenariosCombinations(EFGetAndPutNamedStream);
            }

            private void EFBasicScenariosCombinations(Action<DSPMediaResourceStorage, string, ODataProtocolVersion, string, string, byte[], byte[]> test)
            {
                DSPMediaResourceStorage mediaStorage = null;

                using (TestUtil.RestoreStaticMembersOnDispose(typeof(NorthwindDefaultStreamService)))
                using (NorthwindDefaultStreamService.SetupNorthwindWithStream(
                    new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("Customers", "true"), new KeyValuePair<string, string>("Orders", "true") },
                    "NamedStreamTests_EFBasicScenarios"))
                using (TestUtil.RestoreStaticValueOnDispose(typeof(OpenWebDataServiceHelper), "ForceVerboseErrors"))
                {
                    Type customerWithNamedStreamType = NorthwindNamedStreamServiceFactory.ResolveType("NorthwindModel.Customers");
                    Assert.IsNotNull(customerWithNamedStreamType);
                    Type streamProvider2 = typeof(TypedCustomStreamProvider2<>).MakeGenericType(customerWithNamedStreamType);
                    
                    OpenWebDataServiceHelper.ForceVerboseErrors = true;
                    NorthwindDefaultStreamService.GetServiceOverride = type =>
                    {
                        if (type == typeof(providers.IDataServiceStreamProvider2))
                        {
                            return Activator.CreateInstance(streamProvider2, mediaStorage);
                        }

                        if (type == typeof(providers.IDataServiceStreamProvider))
                        {
                            return Activator.CreateInstance(streamProvider2, mediaStorage);
                        }

                        return null;
                    };

                    TestUtil.RunCombinations(UnitTestsUtil.ResponseFormats, ProtocolVersions, DataServiceVersions, DataServiceVersions,
                    (mimeType, maxProtocolVersion, dsv, mdsv) =>
                    {
                        mediaStorage = new DSPMediaResourceStorage(new CustomerComparer());
                        byte[] defaultStreamBuffer = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                        byte[] stream1Buffer = new byte[] { 1, 2, 3, 4 };

                        using (OpenWebDataServiceHelper.MaxProtocolVersion.Restore())
                        {
                            TestUtil.ClearConfiguration();
                            OpenWebDataServiceHelper.MaxProtocolVersion.Value = maxProtocolVersion;
                            test(mediaStorage, mimeType, maxProtocolVersion, dsv, mdsv, defaultStreamBuffer, stream1Buffer);
                        }
                    });
                }
            }

            private class CustomerComparer : IEqualityComparer<object>
            {
                public new bool Equals(object x, object y)
                {
                    // X and Y can be streaming entities defined in runtime compiled assembly
                    // use reflection to get IDs
                    string id1, id2;
                    id1 = x.GetType().GetProperty("CustomerID").GetValue(x, null).ToString();
                    id2 = y.GetType().GetProperty("CustomerID").GetValue(y, null).ToString();

                    return id1.Equals(id2);
                }

                public int GetHashCode(object obj)
                {
                    return obj.GetType().GetProperty("CustomerID").GetValue(obj, null).GetHashCode();
                }
            }

            private void EFGetAndPutNamedStream(
                DSPMediaResourceStorage mediaStorage,
                string mimeType,
                ODataProtocolVersion maxProtocolVersion,
                string dsv,
                string mdsv,
                byte[] defaultStreamBuffer,
                byte[] stream1Buffer)
            {
                ////////////////////////////////////////////////////////////////
                // GET null named stream
                ////////////////////////////////////////////////////////////////
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.ServiceType = NorthwindNamedStreamServiceFactory.GetNamedStreamServiceType();
                    request.HttpMethod = "GET";
                    request.RequestUriString = "/Customers('ALFKI')/Stream1";
                    request.RequestStream = null;
                    request.RequestVersion = dsv;
                    request.RequestMaxVersion = mdsv;

                    WebException webException = (WebException)TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(webException, maxProtocolVersion < ODataProtocolVersion.V4);

                    if (webException == null)
                    {
                        Assert.AreEqual(204, request.ResponseStatusCode);
                        Assert.IsTrue(string.IsNullOrEmpty(request.ResponseContentType));
                        Assert.AreEqual(0, request.GetResponseStreamAsText().Length);
                    }
                    else
                    {
                        Assert.Fail("Unexpected failure in POST");
                    }
                }

                ////////////////////////////////////////////////////////////////
                // PUT named stream
                ////////////////////////////////////////////////////////////////
                DSPMediaResource mediaResource = null;

                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.ServiceType = NorthwindNamedStreamServiceFactory.GetNamedStreamServiceType();
                    request.HttpMethod = "PUT";
                    request.RequestUriString = "/Customers('ALFKI')/Stream1";
                    request.RequestContentType = "MyType/MySubType";
                    request.RequestVersion = dsv;
                    request.RequestMaxVersion = mdsv;

                    request.RequestStream = new MemoryStream(stream1Buffer);
                    WebException webException = (WebException)TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(webException, maxProtocolVersion < ODataProtocolVersion.V4);

                    if (webException == null)
                    {
                        Assert.AreEqual(204, request.ResponseStatusCode);
                        Assert.IsTrue(string.IsNullOrEmpty(request.ResponseContentType));
                        Assert.AreEqual(0, request.GetResponseStreamAsText().Length);
                        mediaResource = mediaStorage.Content.Single().Value.Single().Value;
                        Assert.IsNotNull(mediaResource);
                        bool isEmptyStream;
                        Assert.IsTrue(TestUtil.CompareStream(mediaResource.GetReadStream(out isEmptyStream), new MemoryStream(stream1Buffer)));
                        Assert.AreEqual(mediaResource.ContentType, request.RequestContentType);
                        Assert.IsNotNull(mediaResource.Etag);
                    }
                    else
                    {
                        Assert.Fail("Unexpected failure in POST");
                    }
                }

                ////////////////////////////////////////////////////////////////
                // GET named stream
                ////////////////////////////////////////////////////////////////
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(BlobDataServicePipelineHandlers)))
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.ServiceType = NorthwindNamedStreamServiceFactory.GetNamedStreamServiceType();
                    request.HttpMethod = "GET";
                    request.RequestUriString = "/Customers('ALFKI')/Stream1";
                    request.RequestVersion = dsv;
                    request.RequestMaxVersion = mdsv;

                    WebException webException = (WebException)TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(webException, maxProtocolVersion < ODataProtocolVersion.V4);

                    if (webException == null)
                    {
                        Assert.AreEqual(200, request.ResponseStatusCode);
                        Assert.AreEqual("MyType/MySubType", request.ResponseContentType);
                        Assert.IsTrue(TestUtil.CompareStream(request.GetResponseStream(), new MemoryStream(stream1Buffer)));
                        Assert.AreEqual(mediaResource.ContentType, request.ResponseContentType);
                        Assert.AreEqual(mediaResource.Etag, request.ResponseETag);
                    }
                    else
                    {
                        Assert.Fail("Unexpected failure in POST");
                    }
                }
            }

            #endregion Main Scenario Functional Tests

            #region Stream Provider API Tests
            [Ignore] // Remove Atom
            // [TestMethod]
            public void NamedStreamIDSSP2ApiTest()
            {
                TestUtil.RunCombinations(UnitTestsUtil.ResponseFormats, (accept) =>
                {
                    DSPServiceDefinition service = SetupDSPService();
                    using (TestWebRequest request = service.CreateForInProcess())
                    {
                        List<string> xpaths = new List<string>()
                        {
                            "boolean(/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/edit-media/Stream1' and contains(@href, 'MySet1(1)/Stream1') and not(@adsm:etag) and not(@type)])",
                            "boolean(/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/edit-media/Stream2' and contains(@href, 'MySet1(1)/Stream2') and not(@adsm:etag) and not(@type)])",
                            "not    (/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/mediaresource/Stream1'])",
                            "not    (/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/mediaresource/Stream2'])",
                        };

                        request.HttpMethod = "GET";
                        request.RequestUriString = "/MySet1(1)";
                        request.Accept = accept;
                        request.SendRequest();
                        XmlDocument atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, xpaths.ToArray());

                        DSPResource entity1 = (DSPResource)service.CurrentDataSource.GetResourceSetEntities("MySet1").Single(e => (int)((DSPResource)e).GetValue("ID") == 1);
                        DSPMediaResource stream1 = service.MediaResourceStorage.CreateMediaResource(entity1, entity1.ResourceType.GetNamedStreams().Single(ns => ns.Name == "Stream1"));
                        DSPMediaResource stream2 = service.MediaResourceStorage.CreateMediaResource(entity1, entity1.ResourceType.GetNamedStreams().Single(ns => ns.Name == "Stream2"));
                        stream1.ContentType = "CustomType/Stream1";

                        xpaths = new List<string>()
                        {
                            "boolean(/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/edit-media/Stream1' and contains(@href, 'MySet1(1)/Stream1') and not(@adsm:etag) and @type='CustomType/Stream1'])",
                            "boolean(/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/edit-media/Stream2' and contains(@href, 'MySet1(1)/Stream2') and not(@adsm:etag) and not(@type)])",
                            "not    (/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/mediaresource/Stream1'])",
                            "not    (/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/mediaresource/Stream2'])",
                        };

                        request.HttpMethod = "GET";
                        request.RequestUriString = "/MySet1(1)";
                        request.Accept = accept;
                        request.SendRequest();
                        atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, xpaths.ToArray());

                        stream2.Etag = DSPMediaResource.GenerateStreamETag();

                        xpaths = new List<string>()
                        {
                            "boolean(/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/edit-media/Stream1' and contains(@href, 'MySet1(1)/Stream1') and not(@adsm:etag) and @type='CustomType/Stream1'])",
                            "boolean(/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/edit-media/Stream2' and contains(@href, 'MySet1(1)/Stream2') and @adsm:etag='" + stream2.Etag + "' and not(@type)])",
                            "not    (/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/mediaresource/Stream1'])",
                            "not    (/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/mediaresource/Stream2'])",
                        };

                        request.HttpMethod = "GET";
                        request.RequestUriString = "/MySet1(1)";
                        request.Accept = accept;
                        request.SendRequest();
                        atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, xpaths.ToArray());

                        stream2.ReadStreamUri = new Uri("http://myservice/foo", UriKind.Absolute);

                        xpaths = new List<string>()
                        {
                            "boolean(/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/edit-media/Stream1' and contains(@href, 'MySet1(1)/Stream1') and not(@adsm:etag) and @type='CustomType/Stream1'])",
                            "boolean(/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/edit-media/Stream2' and contains(@href, 'MySet1(1)/Stream2') and @adsm:etag='" + stream2.Etag + "' and not(@type)])",
                            "not    (/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/mediaresource/Stream1'])",
                            "boolean(/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/mediaresource/Stream2' and @href='http://myservice/foo' and not(@adsm:etag) and not(@type)])",
                        };

                        request.HttpMethod = "GET";
                        request.RequestUriString = "/MySet1(1)";
                        request.Accept = accept;
                        request.SendRequest();
                        atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, xpaths.ToArray());

                        stream2.ContentType = "CustomType/Stream2";

                        xpaths = new List<string>()
                        {
                            "boolean(/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/edit-media/Stream1' and contains(@href, 'MySet1(1)/Stream1') and not(@adsm:etag) and @type='CustomType/Stream1'])",
                            "boolean(/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/edit-media/Stream2' and contains(@href, 'MySet1(1)/Stream2') and @adsm:etag='" + stream2.Etag + "' and @type='CustomType/Stream2'])",
                            "not    (/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/mediaresource/Stream1'])",
                            "boolean(/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/mediaresource/Stream2' and @href='http://myservice/foo' and not(@adsm:etag) and @type='CustomType/Stream2'])",
                        };

                        request.HttpMethod = "GET";
                        request.RequestUriString = "/MySet1(1)";
                        request.Accept = accept;
                        request.SendRequest();
                        atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                        UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, xpaths.ToArray());
                    }
                });
            }

            #endregion Stream Provider API Tests

            #region Cross Feature Tests

            [NamedStream("Stream1")]
            [NamedStream("Stream2")]
            public class MyEntityWithNamedStreams
            {
                public int ID { get; set; }
            }

            [TestMethod]
            public void NamedStreamPermissionTest()
            {
                using (TestUtil.RestoreStaticValueOnDispose(typeof(TypedCustomDataContext<MyEntityWithNamedStreams>), "PreserveChanges"))
                using (TestUtil.RestoreStaticValueOnDispose(typeof(OpenWebDataServiceHelper), "ForceVerboseErrors"))
                {
                    OpenWebDataServiceHelper.ForceVerboseErrors = true;
                    TypedCustomDataContext<MyEntityWithNamedStreams>.ClearHandlers();
                    TypedCustomDataContext<MyEntityWithNamedStreams>.ClearValues();
                    TypedCustomDataContext<MyEntityWithNamedStreams>.PreserveChanges = true;

                    TypedCustomDataContext<MyEntityWithNamedStreams>.ValuesRequested += (sender, args) =>
                    {
                        TypedCustomDataContext<MyEntityWithNamedStreams> typedContext = (TypedCustomDataContext<MyEntityWithNamedStreams>)sender;
                        typedContext.SetValues(new MyEntityWithNamedStreams[] { new MyEntityWithNamedStreams { ID = 1 } });
                    };

                    string[] methods = new[] { "PUT", "GET" };
                    List<EntitySetRights> rights = new List<EntitySetRights>();
                    for (EntitySetRights r = 0; r <= EntitySetRights.All; r++)
                    {
                        rights.Add(r);
                    }

                    TestUtil.RunCombinations(methods, rights, (method, right) =>
                    {
                        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                        using (OpenWebDataServiceHelper.EntitySetAccessRule.Restore())
                        using (OpenWebDataServiceHelper.GetServiceCustomizer.Restore())
                        using (TestUtil.MetadataCacheCleaner())
                        {
                            DSPMediaResourceStorage storage = new DSPMediaResourceStorage();
                            OpenWebDataServiceHelper.GetServiceCustomizer.Value = (t) =>
                            {
                                if (t == typeof(providers.IDataServiceStreamProvider2))
                                {
                                    return new TypedCustomStreamProvider2<MyEntityWithNamedStreams>(storage);
                                }

                                return null;
                            };

                            OpenWebDataServiceHelper.EntitySetAccessRule.Value = new Dictionary<string, EntitySetRights>()
                            {
                                { "Values", right }
                            };

                            request.HttpMethod = method;
                            request.RequestUriString = "/Values(1)/Stream1";
                            request.DataServiceType = typeof(TypedCustomDataContext<MyEntityWithNamedStreams>);
                            if(method == "PUT")
                            {
                                request.RequestStream = new MemoryStream(new byte[] { 0, 1, 2, 3, 4 });
                                request.RequestContentType = "CustomType/Stream1";
                            }

                            WebException e = (WebException)TestUtil.RunCatching(request.SendRequest);
                            TestUtil.AssertExceptionExpected(
                                e,
                                (right & EntitySetRights.ReadSingle) == 0,
                                method == "PUT" && ((right & EntitySetRights.ReadSingle) == 0 || (right & EntitySetRights.WriteReplace) == 0));

                            if (e == null)
                            {
                                if (method == "GET") Assert.AreEqual(204, request.ResponseStatusCode);
                                if (method == "PUT") Assert.AreEqual(204, request.ResponseStatusCode);
                            }
                            else
                            {
                                if (right == EntitySetRights.None)
                                {
                                    Assert.AreEqual(404, request.ResponseStatusCode);
                                    Assert.AreEqual("Resource not found for the segment 'Values'.", e.InnerException.Message);
                                }
                                else if ((right & EntitySetRights.ReadSingle) == 0)
                                {
                                    Assert.AreEqual(403, request.ResponseStatusCode);
                                    Assert.AreEqual("Forbidden", e.InnerException.Message);
                                }
                                else if (method == "PUT" && (right & EntitySetRights.WriteReplace) == 0)
                                {
                                    Assert.AreEqual(403, request.ResponseStatusCode);
                                    Assert.AreEqual("Forbidden", e.InnerException.Message);
                                }
                                else
                                {
                                    Assert.Fail("Exception not expected but received one: " + e.InnerException.Message);
                                }
                            }
                        }
                    });
                }
            }

            private DSPServiceDefinition SetupDSPService()
            {
                ///////////////////////////
                // Setup metadata
                DSPMetadata metadata = new DSPMetadata("NamedStreamIDSPBasicScenariosTest", "NamedStreamTests");
                providers.ResourceType entityType1 = metadata.AddEntityType("EntityType1", null, null, false);
                providers.ResourceSet mySet1 = metadata.AddResourceSet("MySet1", entityType1);

                // Add Primitive Property
                metadata.AddKeyProperty(entityType1, "ID", typeof(int));

                // Add Collection Property
                metadata.AddCollectionProperty(entityType1, "CollectionProperty", providers.ResourceType.GetPrimitiveResourceType(typeof(string)));

                // Add Complex Property
                providers.ResourceType complexType = metadata.AddComplexType("ComplexType", null, null, false);
                metadata.AddPrimitiveProperty(complexType, "Name", typeof(string));
                metadata.AddComplexProperty(entityType1, "ComplexProperty", complexType);

                // Add Navigation Property to Resource Reference
                providers.ResourceType entityType2 = metadata.AddEntityType("EntityType2", null, null, false);
                metadata.AddKeyProperty(entityType2, "ID", typeof(int));
                providers.ResourceSet mySet2 = metadata.AddResourceSet("MySet2", entityType2);
                metadata.AddResourceReferenceProperty(entityType1, "ResourceReferenceProperty", mySet2, entityType2);

                // Add Navigation Property to Resource Set Reference
                providers.ResourceType entityType3 = metadata.AddEntityType("EntityType3", null, null, false);
                metadata.AddKeyProperty(entityType3, "ID", typeof(int));
                providers.ResourceSet mySet3 = metadata.AddResourceSet("MySet3", entityType3);
                metadata.AddResourceSetReferenceProperty(entityType1, "ResourceSetReferenceProperty", mySet3, entityType3);

                // Add Named Streams
                providers.ResourceProperty stream1 = new providers.ResourceProperty("Stream1", providers.ResourcePropertyKind.Stream, providers.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)));
                entityType1.AddProperty(stream1);
                providers.ResourceProperty stream2 = new providers.ResourceProperty("Stream2", providers.ResourcePropertyKind.Stream, providers.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)));
                entityType1.AddProperty(stream2);

                // Add outter type to expand entityType1
                providers.ResourceType entityType0 = metadata.AddEntityType("EntityType0", null, null, false);
                metadata.AddResourceSet("MySet0", entityType0);
                metadata.AddKeyProperty(entityType0, "ID", typeof(int));
                providers.ResourceProperty streamA = new providers.ResourceProperty("StreamA", providers.ResourcePropertyKind.Stream, providers.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)));
                entityType0.AddProperty(streamA);
                providers.ResourceProperty streamB = new providers.ResourceProperty("StreamB", providers.ResourcePropertyKind.Stream, providers.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)));
                entityType0.AddProperty(streamB);
                metadata.AddResourceReferenceProperty(entityType0, "ResourceReferenceProperty", mySet1, entityType1);
                metadata.AddResourceSetReferenceProperty(entityType0, "ResourceSetReferenceProperty", mySet1, entityType1);

                DSPServiceDefinition service = new DSPServiceDefinition();
                service.Metadata = metadata;
                service.Writable = true;
                DSPContext context = new DSPContext();

                //////////////////////////////
                // Setup context
                for (int id = 1; id <= 2; id++)
                {
                    DSPResource entity1 = new DSPResource(entityType1);
                    context.GetResourceSetEntities("MySet1").Add(entity1);

                    entity1.SetValue("ID", id);
                    entity1.SetValue("CollectionProperty", new List<string>() { "string1" });
                    DSPResource complex1 = new DSPResource(complexType);
                    complex1.SetValue("Name", "Foo");
                    entity1.SetValue("ComplexProperty", complex1);

                    DSPResource entity2 = new DSPResource(entityType2);
                    context.GetResourceSetEntities("MySet2").Add(entity2);
                    entity2.SetValue("ID", id);
                    entity1.SetValue("ResourceReferenceProperty", entity2);

                    DSPResource entity3 = new DSPResource(entityType3);
                    context.GetResourceSetEntities("MySet3").Add(entity3);
                    entity3.SetValue("ID", id);
                    entity1.SetValue("ResourceSetReferenceProperty", new List<DSPResource>() { entity3 });

                    DSPResource entity0 = new DSPResource(entityType0);
                    entity0.SetValue("ID", id);
                    entity0.SetValue("ResourceReferenceProperty", entity1);
                    entity0.SetValue("ResourceSetReferenceProperty", context.GetResourceSetEntities("MySet1").Select(o => (DSPResource)o).ToList());
                    context.GetResourceSetEntities("MySet0").Add(entity0);
                }

                service.CreateDataSource = (m) =>
                {
                    return context;
                };

                service.SupportNamedStream = true;
                service.MediaResourceStorage = new DSPMediaResourceStorage();
                return service;
            }

            [TestMethod]
            public void NamedStreamProjectExpandNegativeTest()
            {
                DSPServiceDefinition service = SetupDSPService();
                using (TestWebRequest request = service.CreateForInProcess())
                {
                    request.HttpMethod = "GET";
                    request.RequestUriString = "/MySet1(1)?$select=Stream1/Foo";
                    WebException e = (WebException)TestUtil.RunCatching(request.SendRequest);
                    Assert.IsNotNull(e, "exception expected but received none.");
                    Assert.AreEqual(400, request.ResponseStatusCode);
                    TestUtil.AssertContains(request.GetResponseStreamAsText(), ODataLibResourceUtil.GetString("SelectBinder_MultiLevelPathInSelect"));
                }

                using (TestWebRequest request = service.CreateForInProcess())
                {
                    request.HttpMethod = "GET";
                    request.RequestUriString = "/MySet1(1)?$expand=Stream1";
                    WebException e = (WebException)TestUtil.RunCatching(request.SendRequest);
                    Assert.IsNotNull(e, "exception expected but received none.");
                    Assert.AreEqual(400, request.ResponseStatusCode);
                    TestUtil.AssertContains(request.GetResponseStreamAsText(), ODataLibResourceUtil.GetString("ExpandItemBinder_PropertyIsNotANavigationPropertyOrComplexProperty", "Stream1", "NamedStreamTests.EntityType1"));
                }
            }

            private List<string> GetXPathValidationFor1LevelProjection(bool projectAll, bool collectionProperty, bool complexProperty, bool resourceRefProperty, bool resourceSetRefProperty, bool stream1, bool stream2)
            {
                List<string> xpaths = new List<string>();
                if (!(projectAll || collectionProperty || complexProperty || resourceRefProperty || resourceSetRefProperty || stream1 || stream2))
                {
                    // No projection specified, same as if $select=*
                    projectAll = true;
                }

                if (projectAll)
                {
                    xpaths.Add("boolean((/atom:feed/atom:entry|/atom:entry)/atom:content/adsm:properties/ads:ID)");
                }
                else
                {
                    xpaths.Add("not    ((/atom:feed/atom:entry|/atom:entry)/atom:content/adsm:properties/ads:ID)");
                }

                if (projectAll || collectionProperty)
                {
                    xpaths.Add("boolean((/atom:feed/atom:entry|/atom:entry)/atom:content/adsm:properties/ads:CollectionProperty/adsm:element)");
                }
                else
                {
                    xpaths.Add("not    ((/atom:feed/atom:entry|/atom:entry)/atom:content/adsm:properties/ads:CollectionProperty)");
                }

                if (projectAll || complexProperty)
                {
                    xpaths.Add("boolean((/atom:feed/atom:entry|/atom:entry)/atom:content/adsm:properties/ads:ComplexProperty/ads:Name)");
                }
                else
                {
                    xpaths.Add("not    ((/atom:feed/atom:entry|/atom:entry)/atom:content/adsm:properties/ads:ComplexProperty)");
                }

                if (projectAll || resourceRefProperty)
                {
                    xpaths.Add("boolean((/atom:feed/atom:entry|/atom:entry)/atom:link[@title='ResourceReferenceProperty'])");
                }
                else
                {
                    xpaths.Add("not    ((/atom:feed/atom:entry|/atom:entry)/atom:link[@title='ResourceReferenceProperty'])");
                }

                if (projectAll || resourceSetRefProperty)
                {
                    xpaths.Add("boolean((/atom:feed/atom:entry|/atom:entry)/atom:link[@title='ResourceSetReferenceProperty'])");
                }
                else
                {
                    xpaths.Add("not    ((/atom:feed/atom:entry|/atom:entry)/atom:link[@title='ResourceSetReferenceProperty'])");
                }

                if (projectAll || stream1)
                {
                    xpaths.Add("boolean((/atom:feed/atom:entry|/atom:entry)/atom:link[@title='Stream1'])");
                }
                else
                {
                    xpaths.Add("not    ((/atom:feed/atom:entry|/atom:entry)/atom:link[@title='Stream1'])");
                }

                if (projectAll || stream2)
                {
                    xpaths.Add("boolean((/atom:feed/atom:entry|/atom:entry)/atom:link[@title='Stream2'])");
                }
                else
                {
                    xpaths.Add("not    ((/atom:feed/atom:entry|/atom:entry)/atom:link[@title='Stream2'])");
                }

                return xpaths;
            }
            [Ignore] // Remove Atom
            // [TestMethod]
            public void NamedStreamProjectExpandSDPTest()
            {
                DSPServiceDefinition service = SetupDSPService();
                service.PageSizeCustomizer = (config, serviceType) =>
                {
                    config.SetEntitySetPageSize("MySet0", 1);
                    config.SetEntitySetPageSize("MySet1", 1);
                    config.SetEntitySetPageSize("MySet2", 1);
                    config.SetEntitySetPageSize("MySet3", 1);
                };

                // Level 1 projection
                using (TestWebRequest request = service.CreateForInProcess())
                {
                    TestUtil.RunCombinations(UnitTestsUtil.ResponseFormats, BooleanValues, BooleanValues, BooleanValues, BooleanValues, BooleanValues, BooleanValues, BooleanValues,
                        (accept, projectAll, collectionProperty, complexProperty, resourceRefProperty, resourceSetRefProperty, stream1, stream2) =>
                        {
                            // Construct $select
                            StringBuilder select = new StringBuilder();
                            if (projectAll || collectionProperty || complexProperty || resourceRefProperty || resourceSetRefProperty || stream1 || stream2)
                            {
                                select.Append("?$select=");
                            }

                            if (projectAll) select.Append("*,");
                            if (collectionProperty) select.Append("CollectionProperty,");
                            if (complexProperty) select.Append("ComplexProperty,");
                            if (resourceRefProperty) select.Append("ResourceReferenceProperty,");
                            if (resourceSetRefProperty) select.Append("ResourceSetReferenceProperty,");
                            if (stream1) select.Append("Stream1,");
                            if (stream2) select.Append("Stream2,");
                            if (select.Length > 0) select.Remove(select.Length - 1, 1); // remove trialing comma

                            // Validations
                            List<string> xpaths = GetXPathValidationFor1LevelProjection(projectAll, collectionProperty, complexProperty, resourceRefProperty, resourceSetRefProperty, stream1, stream2);
                            QueryAndValidateProjectSDP(request, request.BaseUri + "MySet1(1)" + select.ToString(), accept, select.ToString(), xpaths);
                            QueryAndValidateProjectSDP(request, request.BaseUri + "MySet1" + select.ToString(), accept, select.ToString(), xpaths);
                        });
                }
            }

            private void QueryAndValidateProjectSDP(TestWebRequest request, string fullRequestUri, string accept, string select, List<string> xpaths)
            {
                request.HttpMethod = "GET";
                request.FullRequestUriString = fullRequestUri;
                request.Accept = accept;
                request.SendRequest();
                XmlDocument atomResponse = UnitTestsUtil.GetResponseAsAtom(request);
                if (atomResponse.SelectSingleNode("/atom:entry", TestUtil.TestNamespaceManager) != null)
                {
                    // Single entry response, validate the payload.
                    UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, xpaths.ToArray());
                }
                else
                {
                    XmlNode nextLink = atomResponse.SelectSingleNode("/atom:feed/atom:link[@rel='next']/@href", TestUtil.TestNamespaceManager);
                    if (nextLink != null)
                    {
                        // Feed response, validate the payload if the feed is not empty.
                        UnitTestsUtil.VerifyXPathExpressionResults(atomResponse, true, xpaths.ToArray());
                        fullRequestUri = nextLink.Value;
                        if (select != null)
                        {
                            TestUtil.AssertContains(fullRequestUri, select);
                        }

                        QueryAndValidateProjectSDP(request, fullRequestUri, accept, select, xpaths);
                    }
                }
            }

            [TestMethod]
            public void NamedStreamCallbackQueryOptionTest()
            {
                DSPMetadata metadata = new DSPMetadata("NamedStreamIDSPBasicScenariosTest", "NamedStreamTests");
                providers.ResourceType entityType = metadata.AddEntityType("EntityType", null, null, false);
                metadata.AddKeyProperty(entityType, "ID", typeof(int));
                metadata.AddPrimitiveProperty(entityType, "Name", typeof(string));
                entityType.AddProperty(new providers.ResourceProperty("Stream1", providers.ResourcePropertyKind.Stream, providers.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream))));
                entityType.AddProperty(new providers.ResourceProperty("Stream2", providers.ResourcePropertyKind.Stream, providers.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream))));
                metadata.AddResourceSet("MySet", entityType);
                
                DSPServiceDefinition service = new DSPServiceDefinition();
                service.ForceVerboseErrors = true;
                service.Metadata = metadata;
                service.Writable = true;
                service.MediaResourceStorage = new DSPMediaResourceStorage();
                service.SupportNamedStream = true;
                DSPContext context = new DSPContext();
                service.CreateDataSource = (m) =>
                {
                    return context;
                };
                
                byte[] stream1Buffer = new byte[] { 65, 66, 67, 68 };

                using (OpenWebDataServiceHelper.MaxProtocolVersion.Restore())
                {
                    OpenWebDataServiceHelper.MaxProtocolVersion.Value = ODataProtocolVersion.V4;
                    DSPResource resource = new DSPResource(entityType);
                    resource.SetValue("ID", 111);
                    service.CurrentDataSource.GetResourceSetEntities("MySet").Add(resource);

                    using (TestWebRequest request = service.CreateForInProcessWcf())
                    {
                        request.HttpMethod = "PUT";
                        request.RequestUriString = "/MySet(111)/Stream1";
                        request.RequestContentType = "text/plain";

                        request.RequestStream = new MemoryStream(stream1Buffer);
                        request.SendRequest();
                        Assert.AreEqual(204, request.ResponseStatusCode);
                    }

                    using (TestWebRequest request = service.CreateForInProcessWcf())
                    {
                        request.HttpMethod = "GET";
                        request.RequestUriString = "/MySet(111)/Stream1?$callback=foo";

                        request.SendRequest();
                        Assert.AreEqual(200, request.ResponseStatusCode);
                        Assert.AreEqual("text/plain", request.ResponseContentType);

                        // We do not wrap the name stream response with the callback value.
                        Assert.IsTrue(TestUtil.CompareStream(request.GetResponseStream(), new MemoryStream(stream1Buffer)));
                    }
                }
            }

            #endregion Cross Feature Tests

            [NamedStream("Stream")]
            [NamedStream("Stream")]
            public class MyEntityWithDupeNamedStreams
            {
                public int ID { get; set; }
            }

            [TestMethod]
            public void NamedStreamAttributeInvalidTest()
            {
                using (TestUtil.RestoreStaticValueOnDispose(typeof(TypedCustomDataContext<MyEntityWithDupeNamedStreams>), "PreserveChanges"))
                using (TestUtil.RestoreStaticValueOnDispose(typeof(OpenWebDataServiceHelper), "ForceVerboseErrors"))
                using (OpenWebDataServiceHelper.GetServiceCustomizer.Restore())
                using (TestUtil.MetadataCacheCleaner())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    OpenWebDataServiceHelper.ForceVerboseErrors = true;
                    TypedCustomDataContext<MyEntityWithDupeNamedStreams>.ClearHandlers();
                    TypedCustomDataContext<MyEntityWithDupeNamedStreams>.ClearValues();
                    TypedCustomDataContext<MyEntityWithDupeNamedStreams>.CreateChangeScope();
                    TypedCustomDataContext<MyEntityWithDupeNamedStreams>.ValuesRequested += (sender, e) =>
                    {
                        TypedCustomDataContext<MyEntityWithDupeNamedStreams> context = (TypedCustomDataContext<MyEntityWithDupeNamedStreams>)sender;
                        context.SetValues(new[]
                        {
                            new MyEntityWithDupeNamedStreams() { ID = 1}
                        });
                    };

                    DSPMediaResourceStorage storage = new DSPMediaResourceStorage();
                    OpenWebDataServiceHelper.GetServiceCustomizer.Value = (t) =>
                    {
                        if (t == typeof(providers.IDataServiceStreamProvider2))
                        {
                            return new TypedCustomStreamProvider2<MyEntityWithDupeNamedStreams>(storage);
                        }

                        return null;
                    };

                    string[] uris = new[]
                    {
                        "/",
                        "/$metadata",
                        "/Values",
                        "/Values(1)"
                    };

                    foreach (string uri in uris)
                    {
                        request.DataServiceType = typeof(TypedCustomDataContext<MyEntityWithDupeNamedStreams>);
                        request.HttpMethod = "GET";
                        request.RequestUriString = uri;

                        Exception e = TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNotNull(e, "Exception expected but received none.");
                        string expectedErrorMsg = DataServicesResourceUtil.GetString("ResourceType_PropertyWithSameNameAlreadyExists", "Stream", "AstoriaUnitTests.Tests.NamedStreamUnitTestModule_NamedStreamTests_MyEntityWithDupeNamedStreams");
                        Assert.IsInstanceOfType(e, typeof(InvalidOperationException));
                        Assert.AreEqual(expectedErrorMsg, e.Message);
                    }
                }
            }
        }
    }
}

namespace AstoriaUnitTests.ObjectContextStubs.Hidden
{
    using System;
    using System.Data.EntityClient;
    using System.Data.Objects.DataClasses;
    using System.Data.SqlClient;
    using System.Data.Test.Astoria;
    using System.IO;
    using System.Runtime.Serialization;
    using Microsoft.OData.Client;

    /// <summary>
    /// This is an invalid context for negative testing.  Since EF doesn't allow us to put it in the test case class,
    /// we are keeping it in the Hidden namespace so we don't polute the test namespace.
    /// </summary>
    public partial class EFModelWithNamedStreamOnDerivedType : System.Data.Objects.ObjectContext
    {
        private const string EFModelWithNamedStreamsSSDL = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Schema xmlns=""http://schemas.microsoft.com/ado/2009/02/edm/ssdl"" Namespace=""Model1.Store"" Alias=""Self"" Provider=""System.Data.SqlClient"" ProviderManifestToken=""2005"">
  <EntityContainer Name=""Model1TargetContainer"" />
</Schema>";

        private const string EFModelWithNamedStreamsCSDL = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Schema xmlns=""http://schemas.microsoft.com/ado/2008/09/edm"" xmlns:cg=""http://schemas.microsoft.com/ado/2006/04/codegeneration"" xmlns:store=""http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator"" Namespace=""Model1"" Alias=""Self"" xmlns:annotation=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
  <EntityContainer Name=""Model1Container"">
    <EntitySet Name=""EFEntity1"" EntityType=""Model1.EFEntity1"" />
  </EntityContainer>
  <EntityType Name=""EFEntity1"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Type=""Int32"" Name=""Id"" Nullable=""false"" annotation:StoreGeneratedPattern=""Identity"" />
  </EntityType>
  <EntityType Name=""EFEntity2"" BaseType=""Model1.EFEntity1"">
  </EntityType>
</Schema>";

        private const string EFModelWithNamedStreamsMSL = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Mapping xmlns=""http://schemas.microsoft.com/ado/2008/09/mapping/cs"" Space=""C-S"">
  <Alias Key=""Model"" Value=""Model1"" />
  <Alias Key=""Target"" Value=""Model1.Store"" />
  <EntityContainerMapping CdmEntityContainer=""Model1Container"" StorageEntityContainer=""Model1TargetContainer"" />
</Mapping>";

        private static EntityConnection CreateConnection()
        {
            string targetPath = Path.Combine(TestUtil.GeneratedFilesLocation, "NamedStreamEFModel1");
            IOUtil.EnsureEmptyDirectoryExists(targetPath);

            string ssdl = Path.Combine(targetPath, "Model1.ssdl");
            using (StreamWriter writer = new StreamWriter(File.OpenWrite(ssdl)))
            {
                writer.Write(EFModelWithNamedStreamsSSDL);
            }

            string csdl = Path.Combine(targetPath, "Model1.csdl");
            using (StreamWriter writer = new StreamWriter(File.OpenWrite(csdl)))
            {
                writer.Write(EFModelWithNamedStreamsCSDL);
            }

            string msl = Path.Combine(targetPath, "Model1.msl");
            using (StreamWriter writer = new StreamWriter(File.OpenWrite(msl)))
            {
                writer.Write(EFModelWithNamedStreamsMSL);
            }

            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = DataUtil.DefaultDataSource;
            sqlBuilder.IntegratedSecurity = true;
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.ConnectTimeout = 600;

            var entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Metadata = ssdl + "|" + csdl + "|" + msl;
            entityBuilder.Provider = "System.Data.SqlClient";
            entityBuilder.ProviderConnectionString = sqlBuilder.ConnectionString;

            return new EntityConnection(entityBuilder.ConnectionString);
        }

        public EFModelWithNamedStreamOnDerivedType()
            : base(CreateConnection())
        {
            this.ContextOptions.LazyLoadingEnabled = true;
        }

        public System.Data.Objects.ObjectSet<EFEntity1> EFEntity1
        {
            get { return null; }
        }
    }

    [EdmEntityTypeAttribute(NamespaceName = "Model1", Name = "EFEntity1")]
    [Serializable()]
    [DataContractAttribute(IsReference = true)]
    [KnownTypeAttribute(typeof(EFEntity2))]
    [NamedStream("Stream1")]
    public partial class EFEntity1 : System.Data.Objects.DataClasses.EntityObject
    {
        [EdmScalarPropertyAttribute(EntityKeyProperty = true, IsNullable = false)]
        [DataMemberAttribute()]
        public int Id { get; set; }
    }

    [EdmEntityTypeAttribute(NamespaceName = "Model1", Name = "EFEntity2")]
    [Serializable()]
    [DataContractAttribute(IsReference = true)]
    [NamedStream("Stream2")]
    public partial class EFEntity2 : EFEntity1 { }
}
