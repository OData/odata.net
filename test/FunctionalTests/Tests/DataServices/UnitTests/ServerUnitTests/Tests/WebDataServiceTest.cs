//---------------------------------------------------------------------
// <copyright file="WebDataServiceTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Data.Objects.DataClasses;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Xml;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using EdmError = Microsoft.OData.Edm.Validation.EdmError;

    #endregion Namespaces

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
    /// <summary>
    /// This is a test class for WebDataServiceTest and is intended
    /// to contain all WebDataServiceTest Unit Tests
    /// </summary>
    [TestClass()]
    public class WebDataServiceTest
    {
        /// <summary>A helper method for DataService.ctor().</summary>
        public DataService<T> WebDataServiceConstructorTestHelper<T>()
        {
            return new DataService<T>();
        }

        /// <summary>A helper method for DataService.ctor(host).</summary>
        public void WebDataServiceConstructorTestHelper<T>(IDataServiceHost host)
        {
            DataService<T> target = new DataService<T>();
            target.AttachHost(host);
        }

        /// <summary>
        /// A test for DataService.ctor() outside of an Http request.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WebDataServiceConstructorNoContextTest()
        {
            var target = WebDataServiceConstructorTestHelper<string>();
            target.ProcessRequest();
        }

        /// <summary>
        /// A test for DataService.ctor(host) for an ObjectContext type.
        /// </summary>
        [TestMethod()]
        public void WebDataServiceConstructorObjectContextTest()
        {
            WebDataServiceConstructorTestHelper<NorthwindModel.NorthwindContext>(new TestServiceHost());
        }

        /// <summary>
        /// A test for DataService.ctor(host) for a plain CLR type.
        /// </summary>
        [TestMethod()]
        public void WebDataServiceConstructorReflectionTest()
        {
            WebDataServiceConstructorTestHelper<string>(new TestServiceHost());
        }

        /// <summary>
        /// A test for DataService.ctor(host) with a null host.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WebDataServiceConstructorNullHostTest()
        {
            DataService<string> target = new DataService<string>();
            target.AttachHost(null);
        }

        /// <summary>
        /// A test for DataService.ctor(host) with a custom host.
        /// </summary>
        [TestMethod]
        public void WebDataServiceConstructorCustomTest()
        {
            TestServiceHost host = new TestServiceHost();
            DataService<string> target = new DataService<string>();
            target.AttachHost(host);
            object operationContext = target.GetType().GetField("operationContext", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(target);
            object actualHost = operationContext.GetType().GetField("hostInterface", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(operationContext);
            Assert.AreSame(host, actualHost);
        }
        [Ignore] // Remove Atom
        // [TestMethod]
        public void WebDataServiceCsdlMimeTypes()
        {
            object[] targets = new object[]
            {
                // BuiltInTypeKind.EdmFunction, 
                // BuiltInTypeKind.ComplexType,
                // BuiltInTypeKind.EntityContainer, 
                // BuiltInTypeKind.EntityType,
                BuiltInTypeKind.EdmProperty, 
            };

            // Ensure the NorthwindModel service is ready to be used.
            Trace.WriteLine("NorthwindModel IsValid: " + ServiceModelData.Northwind.IsValid);

            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("Target", targets),
                new Dimension("MimeType", new object[] { "", "text/html" }));
            int schemaCounter = 0;
            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
            {
                BuiltInTypeKind target = (BuiltInTypeKind)values["Target"];
                string mimeType = (string)values["MimeType"];

                // Create a copy of Northwind model files.
                schemaCounter++;
                const bool overwriteTrue = true;
                string sourceFileName = Path.Combine(TestUtil.NorthwindMetadataDirectory, "Northwind.csdl");
                string destFileName = sourceFileName + schemaCounter + ".csdl";
                File.Copy(sourceFileName, destFileName, overwriteTrue);
                File.SetAttributes(destFileName, FileAttributes.Normal);
                
                // Add an attribute at specific values.
                string csdlFileName = destFileName;
                XmlDocument document = new XmlDocument();
                document.Load(csdlFileName);
                XmlElement element = GetElementForKind(document, target);
                element.SetAttribute("MimeType", "http://docs.oasis-open.org/odata/ns/metadata", mimeType);
                document.Save(csdlFileName);

                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    // Get the generated CSDL.
                    string connectionString = NorthwindModel.NorthwindContext.ContextConnectionString;
                    try
                    {
                        string northwindConnectionString = connectionString;
                        northwindConnectionString = northwindConnectionString
                            .Replace("Northwind.csdl", "Northwind.csdl" + schemaCounter + ".csdl");
                        NorthwindModel.NorthwindContext.ContextConnectionString = northwindConnectionString;

                        request.DataServiceType = typeof(NorthwindModel.NorthwindContext);
                        request.RequestUriString = "/$metadata";
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception,
                            target != BuiltInTypeKind.EdmProperty,
                            mimeType == "");

                        if (exception != null)
                        {
                            return;
                        }

                        XmlDocument metadataResult = new XmlDocument(TestUtil.TestNameTable);
                        metadataResult.Load(request.GetResponseStream());
                        Trace.WriteLine(metadataResult.OuterXml);

                        // Get a value using a primitive serializer.
                        request.RequestUriString = "/Customers?$format=atom&$top=1";
                        request.SendRequest();
                        XmlDocument customerDocument = SerializationFormatData.Atom.LoadXmlDocumentFromStream(request.GetResponseStream());
                        XmlElement customerId = TestUtil.AssertSelectSingleElement(customerDocument, "/atom:feed/atom:entry/atom:id");
                        request.FullRequestUriString = customerId.InnerText + "/CompanyName/$value";
                        request.SendRequest();
                        Assert.AreEqual(mimeType, TestUtil.GetMediaType(request.ResponseContentType));
                    }
                    finally
                    {
                        NorthwindModel.NorthwindContext.ContextConnectionString = connectionString;
                    }
                }
            });
        }
        [Ignore] // Remove Atom
        // [TestMethod]
        public void WebDataServiceWcfEntryPoint()
        {
            Type serviceType = typeof(OpenWebDataService<>).MakeGenericType(typeof(CustomDataContext));
            DataServiceHost host1 = UnitTestsUtil.StartHostProcess(serviceType, "http://localhost:{port}/TheTest", (host) => {
                ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(IRequestHandler), new WebHttpBinding(), "");
            });
            
            try
            {
                System.Net.WebRequest request = System.Net.WebRequest.Create(host1.BaseAddresses[0].ToString());
                System.Net.WebResponse response = request.GetResponse();
                Trace.WriteLine(new StreamReader(response.GetResponseStream()).ReadToEnd());
            }
            finally
            {
                host1.Close();
            }
        }

        [TestMethod]
        public void WebDataServiceDanglingNavigationProperty()
        {
            // Verifies that navigation properties are appropriately rejected if they have
            // no AssociationSet for any one EntitySet.
            const string modelText =
                "Ns.ET1 = entitytype { ID1 int key; };" +
                "Ns.ET2 = entitytype { ID2 int key; };" +
                "AT1 = associationtype { end1 Ns.ET1 1; end2 Ns.ET2 *;};" +
                "ET1 = entitytype { navigation NP1 AT1 end1 Ns.ET2; };" +   
                "ESa : Ns.ET1; ESb : Ns.ET2; ESc : Ns.ET1;" +               // Entity sets
                "ASab : AT1 { end1 ESa; end2 ESb; };";                      // Association set

            AdHocModel model = AdHocModel.ModelFromText(modelText);
            Assembly assembly = model.GenerateModelsAndAssembly("DanglingNavpropModel", false /* isReflectionProviderBased */);
            Type type = TestUtil.LoadDerivedTypeFromAssembly(assembly, typeof(System.Data.Objects.ObjectContext));
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = type;
                request.RequestUriString = "/$metadata";
                Exception exception = TestUtil.RunCatching(request.SendRequest);
                TestUtil.AssertExceptionExpected(exception, true);
                TestUtil.AssertExceptionStatusCode(exception, 500, "500 expected for dangling property.");
            }
        }
        [Ignore] // Remove Atom
        // [TestMethod]
        public void WebDataServiceDocumentTest()
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("Location", new object[] { WebServerLocation.InProcess, WebServerLocation.InProcessWcf }),
                new Dimension("ServiceModelData", ServiceModelData.Values),
                new Dimension("Accept", new string[]
                    {
                        "application/atomsvc+xml",
                        "application/atomsvc+xml;q=0.8",
                        "application/xml",
                        "application/xml;q=0.5"
                    }));
            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
            {
                string accept = (string)values["Accept"];
                WebServerLocation location = (WebServerLocation)values["Location"];
                ServiceModelData model = (ServiceModelData)values["ServiceModelData"];
                if (!model.IsValid)
                {
                    return;
                }

                using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
                {
                    request.Accept = accept;
                    request.DataServiceType = model.ServiceModelType;
                    request.RequestUriString = "/";
                    request.SendRequest();

                    XmlDocument document = request.GetResponseStreamAsXmlDocument();
                    string responseType = TestUtil.GetMediaType(request.ResponseContentType);
                    if (accept.Contains("application/atomsvc+xml"))
                    {
                        Assert.AreEqual("application/atomsvc+xml", responseType);
                    }
                    else
                    {
                        Assert.AreEqual("application/xml", responseType);
                    }

                    Trace.WriteLine(document.OuterXml);
                    CheckServiceDocument(document);
                }
            });
        }
        [Ignore] // Remove Atom
        // [TestMethod]
        public void WebDataServiceDocumentJsonLightTest()
        {
            // Smoke test to verify that JSON Light service document can be written through the server. Detailed format-specific tests are in ODL.
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("Location", new object[] { WebServerLocation.InProcess, WebServerLocation.InProcessWcf }),
                new Dimension("Accept", new string[]
                    {
                        "application/json",
                        "application/json;odata.metadata=minimal"
                    }));
            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
            {
                string accept = (string)values["Accept"];
                WebServerLocation location = (WebServerLocation)values["Location"];
                ServiceModelData model = ServiceModelData.CustomData;

                Assert.IsTrue(model.IsValid);

                using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
                {
                    request.Accept = accept;
                    request.DataServiceType = model.ServiceModelType;
                    request.RequestUriString = "/";
                    request.SendRequest();

                    string response = request.GetResponseStreamAsText();
                    string responseType = TestUtil.GetMediaType(request.ResponseContentType);
                    Assert.AreEqual("application/json;odata.metadata=minimal", responseType);

                    Trace.WriteLine(response);

                    // Customers is defined in ServiceModelData.
                    // Smoke test: Confirm that it's written out as the name of a resource collection somewhere in the service document.
                    Assert.IsTrue(response.Contains("{\"name\":\"Customers\""), "Expected to find \"Customers\" resource collection formatted as JSON Light");
                }
            });
        }

        /// <summary>Checks basic syntax on an APP Service Document.</summary>
        /// <param name="document">Document to check.</param>
        private void CheckServiceDocument(XmlDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            TestUtil.AssertSelectSingleElement(document, "/app:service");
            TestUtil.AssertSelectSingleElement(document, "/app:service/app:workspace");
            TestUtil.AssertSelectSingleElement(document, "/app:service/app:workspace/atom:title");
            XmlNodeList list = TestUtil.AssertSelectNodes(document, "/app:service/app:workspace/app:collection");
            foreach (XmlElement element in list)
            {
                TestUtil.AssertSelectNodes(element, "@href");
                TestUtil.AssertSelectNodes(element, "atom:title");
            }
        }

        [TestMethod]
        public void WebDataServiceBaseTypeContainer()
        {
            AdHocEntityType customersBase = new AdHocEntityType("CustomerBase");
            AdHocEntityType customersDerived = new AdHocEntityType(customersBase) { Name = "CustomerDerived" };
            AdHocEntityType referencingType = new AdHocEntityType("ReferencingType");
            AdHocEntitySet customerSet = new AdHocEntitySet() { Name = "Customers", Type = customersBase };
            AdHocAssociationType associationType = new AdHocAssociationType()
            {
                Name = "ReferenceToDerivedCustomerType",
                Ends = new List<AdHocAssociationTypeEnd>()
                {
                    new AdHocAssociationTypeEnd() { Multiplicity = "1", RoleName = "Reference", Type = referencingType },
                    new AdHocAssociationTypeEnd() { Multiplicity = "*", RoleName = "Customer", Type = customersDerived },
                }
            };
            
            AdHocEntitySet referencingSet = new AdHocEntitySet()
            {
                Name = "ReferenceHolder",
                Type = referencingType
            };
            
            AdHocContainer container = new AdHocContainer()
            {
                AssociationSets = new List<AdHocAssociationSet>()
                {
                    new AdHocAssociationSet()
                    {
                        Name = "ReferenceToDerivedCustomer",
                        Type = associationType,
                        Ends = new List<AdHocAssociationSetEnd>()
                        {
                            new AdHocAssociationSetEnd() { EndType = associationType.Ends[0], EntitySet = referencingSet },
                            new AdHocAssociationSetEnd() { EndType = associationType.Ends[1], EntitySet = customerSet }
                        },
                    }
                },
                EntitySets = new List<AdHocEntitySet>() { customerSet, referencingSet },
                ExtraEntityTypes = new List<AdHocEntityType>() { customersDerived },
            };
            associationType.AddNavigationProperties();

            AdHocModel model = new AdHocModel(container) { ConceptualNs = TestXmlConstants.EdmV1Namespace };
            Assembly assembly = model.GenerateModelsAndAssembly("WebDataServiceBaseTypeContainer", false /* isReflectionProviderBased */);
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = TestUtil.LoadDerivedTypeFromAssembly(assembly, typeof(System.Data.Objects.ObjectContext));
                request.RequestUriString = "/$metadata";
                request.SendRequest();
                using (var s = new StreamReader(request.GetResponseStream())) Trace.WriteLine(s.ReadToEnd());
            }

            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(ContextWithBaseType);
                request.RequestUriString = "/$metadata";
                request.SendRequest();
                XmlDocument document = request.GetResponseStreamAsXmlDocument();
                UnitTestsUtil.VerifyXPaths(document,
                    "//csdl:Schema/csdl:EntityType[@Name='WebDataServiceTest_SimpleBaseType']",
                    "//csdl:Schema/csdl:EntityType[@Name='WebDataServiceTest_ReferencingType']",
                    "//csdl:Schema/csdl:EntityType[@Name='WebDataServiceTest_DerivedType' and @BaseType='AstoriaUnitTests.Tests.WebDataServiceTest_SimpleBaseType']",
                    "//csdl:Schema/csdl:EntityContainer/csdl:EntitySet[@Name='B']",
                    "//csdl:Schema/csdl:EntityContainer/csdl:EntitySet[@Name='R']/csdl:NavigationPropertyBinding[@Path='DerivedReference' and @Target='B']");
            }
        }

        public class SimpleBaseType { public int ID { get; set; } }
        public class DerivedType : SimpleBaseType { public string Name { get; set; } }
        public class ReferencingType { public int ID { get; set; } public DerivedType DerivedReference { get; set; } }
        public class ContextWithBaseType { public IQueryable<SimpleBaseType> B { get; set; } public IQueryable<ReferencingType> R { get; set; } }

        [TestMethod]
        public void WebDataServiceMetadataCache()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                bool called = false;
                using (AstoriaUnitTests.Tests.UnitTestModule.AuthorizationTest.InitializationCallbackManager.RegisterStatic(
                    delegate(object sender, AstoriaUnitTests.Tests.UnitTestModule.AuthorizationTest.InitializationCallbackEventArgs args)
                {
                    args.Configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
                    called = true;
                }))
                {
                    TestUtil.ClearConfiguration();
                    request.ServiceType = typeof(AstoriaUnitTests.Tests.UnitTestModule.AuthorizationTest.WebDataServiceA);
                    request.RequestUriString = "/Customers";
                    request.SendRequest();

                    Assert.IsTrue(called, "Initialization callback called.");

                    called = false;
                    request.SendRequest();
                    Assert.IsFalse(called, "Initialization callback not called - presumably configuration was cached.");
                }
            }
        }

        [TestMethod]
        public void WebDataServiceReflectionNoLinqToSql()
        {
            // Verifies that no Linq to SQL-specific types appear in metadata.
            // Incorrect namespace always being defined as System.Data.Linq when you generated a ClientClassGeneration for a LinqToWorkspace uri
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(TypedCustomDataContext<AllTypes>);
                request.RequestUriString = "/$metadata";
                request.SendRequest();
                string text = request.GetResponseStreamAsText();
                TestUtil.AssertContainsFalse(text, "System.Data");
            }
        }

        [TestMethod]
        public void WebDataServiceReflectionMimeTypes()
        {
            // Request metadata for an attributed data context.
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.RequestUriString = "/$metadata";
                request.SendRequest();

                XmlDocument document = request.GetResponseStreamAsXmlDocument();

                // Verify that mime types are included in the payload.
                XmlNodeList list = document.SelectNodes("//@adsm:MimeType", TestUtil.TestNamespaceManager);
                Assert.IsTrue(list.Count > 0, "MimeType attributes present in attributed custom data context.");
            }
        }
        [Ignore] // Remove Atom
        // [TestMethod]
        public void ProcessGetTest()
        {
            foreach (WebServerLocation location in new WebServerLocation[] { WebServerLocation.InProcess, WebServerLocation.InProcessWcf })
            {
                using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/$metadata";
                    request.SendRequest();
                    Stream resultStream = request.GetResponseStream();
                    TextReader reader = new StreamReader(resultStream);
                    string resultText = reader.ReadToEnd();
                    Assert.IsTrue(resultText.Length > 0);
                    Assert.IsTrue(resultText.Contains("Customers"));
                    request.Dispose();
                }
            }
        }
        [Ignore] // Remove Atom
        // [TestMethod]
        public void SerializeResponseBodyAcceptTypeTest()
        {
            // Verifies that the correct Content-Type is selected at serialization time.
            TestServiceHost host = new TestServiceHost();
            host.RequestPathInfo = "/Customers";
            DataService<CustomDataContext> context = new OpenWebDataService<CustomDataContext>();

            const string AtomXml = "application/atom+xml";
            const string AtomXmlFeed = "application/atom+xml;type=feed";
            const string charsetUtf8 = ";charset=utf-8";
            const string JsonLight = "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false";

            var testCases = new[]
                {
                    // Success cases for all versions
                    new { AcceptHeaderString = "application/atom+xml, application/json", Expectation = JsonLight + charsetUtf8 },
                    new { AcceptHeaderString = "application/atom+xml, application/json;odata.metadata=minimal", Expectation = JsonLight + charsetUtf8 },
                    new { AcceptHeaderString = "application/json, application/atom+xml", Expectation = JsonLight + charsetUtf8 },
                    new { AcceptHeaderString = (string) null, Expectation = JsonLight + charsetUtf8 },
                    new { AcceptHeaderString = "", Expectation = JsonLight + charsetUtf8 },
                    new { AcceptHeaderString = " ", Expectation = JsonLight + charsetUtf8 },
                    new { AcceptHeaderString = "*/*", Expectation = JsonLight + charsetUtf8 },
                    new { AcceptHeaderString = AtomXml, Expectation = AtomXmlFeed + charsetUtf8 },
                    new { AcceptHeaderString = "application/json", Expectation = JsonLight + charsetUtf8 },
                    new { AcceptHeaderString = "application/xml,application/json;q=0.8", Expectation = JsonLight + charsetUtf8 },
                    new { AcceptHeaderString = AtomXml + ",application/json;q=0.8", Expectation = AtomXmlFeed + charsetUtf8 },
                    new { AcceptHeaderString = "application/json," + AtomXml + ";q=0.8", Expectation = JsonLight + charsetUtf8 },
                    new { AcceptHeaderString = "text/xml,*/*", Expectation = JsonLight + charsetUtf8 },
                    new { AcceptHeaderString = "application/xml,*/*", Expectation = JsonLight + charsetUtf8 },

                    // Error cases for all versions
                    new { AcceptHeaderString = "text/xml", Expectation = (string) null },
                    new { AcceptHeaderString = "application/xml", Expectation = (string) null },
                    new { AcceptHeaderString = "application/json;foo=bar", Expectation = (string) null },
                    new { AcceptHeaderString = "application/json;odata.metadata=bla", Expectation = (string) null },
                    new { AcceptHeaderString = "application/json;foo=bar", Expectation = (string) null },
                    
                    // Cases where V2 and V3 expected behaviors differ
                    new { AcceptHeaderString = "application/json", Expectation = JsonLight + charsetUtf8 },
                    new { AcceptHeaderString = "application/json;odata.metadata=minimal", Expectation = JsonLight + charsetUtf8 },
                    new { AcceptHeaderString = "application/json, application/json;odata.metadata=minimal", Expectation = JsonLight + charsetUtf8 },
                };

            TestUtil.RunCombinations(
                testCases,
                new string[] { "4.0" },
                (testCase, requestMaxVersion) =>
                {
                    host.ClearResponse();
                    context.AttachHost(host);
                    string acceptTypes = testCase.AcceptHeaderString;
                    string result = testCase.Expectation;

                    Exception exception = TestUtil.RunCatching(delegate()
                    {
                        Trace.WriteLine("Running query with accept type: " + acceptTypes);
                        host.RequestAccept = acceptTypes;
                        host.RequestMaxVersion = requestMaxVersion;
                        context.ProcessRequest();
                    });
                    TestUtil.AssertExceptionExpected(exception, result == null);
                    if (result != null)
                    {
                        Assert.AreEqual(result, host.ResponseContentType);
                    }
                });
        }

        [TestMethod]
        public void SerializeResponseBodyAcceptTypeWithCharsetTest()
        {
            // Verifies that the correct handling of 'charset' parameters in accept header media types.
            TestServiceHost host = new TestServiceHost();
            host.RequestPathInfo = "/Customers(0)/Name/$value";
            DataService<CustomDataContext> context = new OpenWebDataService<CustomDataContext>();

            const string TextPlain = "text/plain";
            var tests = TestUtil.CreateDictionary<string>(
                TextPlain, TextPlain,
                // valid charset of accept header should not be used for conneg
                TextPlain + ";charset=UTF-8", TextPlain,
                // invalid charset of accept header should not be used for conneg
                TextPlain + ";charset=aabbcc", TextPlain,
                // non-charset parameter should be used for matching
                TextPlain + ";some=value", null,
                // charset and non-charset parameters; non-charset one should be used for matching
                TextPlain + ";charset=utf-8;some=value", null
                );

            TestUtil.RunCombinations(tests, (test) =>
            {
                host.ClearResponse();
                context.AttachHost(host);
                string acceptTypes = test.Key;
                string result = test.Value;
                Exception exception = TestUtil.RunCatching(delegate()
                {
                    Trace.WriteLine("Running query with accept type: " + acceptTypes);
                    host.RequestAccept = acceptTypes;
                    context.ProcessRequest();
                });
                TestUtil.AssertExceptionExpected(exception, result == null);
                if (result != null)
                {
                    Assert.AreEqual(result, TestUtil.GetMediaType(host.ResponseContentType));
                }
            });
        }

        /// <summary>
        /// A test for DataService.ctor(host) for an ObjectContext type.
        /// </summary>
        [TestMethod()]
        public void GetMetadataObjectContext()
        {
            UnitTestsUtil.LoadMetadataFromDataServiceType(typeof(NorthwindModel.NorthwindContext), 
                Path.Combine(TestUtil.GeneratedFilesLocation, "MetadataObjectContext.csdl"));
        }

        [TestMethod()]
        public void GetMetadataForUnitTestProviders()
        {
            foreach (Type providerType in UnitTestsUtil.ProviderTypes)
                using (UnitTestsUtil.CreateChangeScope(providerType))
                {
                    UnitTestsUtil.LoadMetadataFromDataServiceType(providerType,
                        Path.Combine(TestUtil.GeneratedFilesLocation, "Metadata" + providerType.Name + ".csdl"));
                }
        }

        [TestMethod()]
        public void InvalidMetadataTestCases()
        {
            foreach (Type type in GetInvalidTypes())
            {
                // Create the custom data context
                using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcess))
                {
                    request.DataServiceType = typeof(TypedCustomDataContext<>).MakeGenericType(type);
                    request.RequestUriString = "/$metadata";
                    Exception exception = TestUtil.RunCatching(delegate()
                    {
                        request.SendRequest();
                        request.GetResponseStream();
                    });
                    System.Data.Test.Astoria.AstoriaTestLog.IsNotNull(exception,
                        String.Format("The invalid type case should not throw an exception: '{0}'", type.Name));
                }
            }
        }

        [TestMethod()]
        public void InvalidMetadataForEntityHierarchy()
        {
            // Create the custom data context
            using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcess))
            {
                request.DataServiceType = typeof(BadMetadataProvider);
                request.RequestUriString = "/$metadata";
                Exception exception = TestUtil.RunCatching(delegate()
                {
                    request.SendRequest();
                    request.GetResponseStream();
                });
                Assert.IsInstanceOfType(exception, typeof(InvalidOperationException));
                Assert.AreEqual(true, exception.Message.Contains("IgnoreProperties"));
            }
        }

        [TestMethod()]
        public void KeyPropertiesWithAttributesHaveHighestPriority()
        {
            Stream responseStream = UnitTestsUtil.GetResponseStream(WebServerLocation.InProcess, null, "/$metadata", typeof(TypedCustomDataContext<MultiplePropertiesSatisfyingKeyCriteria>));
            XmlDocument document = new XmlDocument(TestUtil.TestNameTable);
            document.Load(responseStream);
            XmlNode node = document.SelectSingleNode("//csdl:Key[csdl:PropertyRef/@Name='ActualID' and csdl:PropertyRef/@Name='ActualID1']", TestUtil.TestNamespaceManager);
            Assert.IsTrue(node != null, "Property with attribute should get the highest priority");
        }

        [TestMethod()]
        public void KeyPropertiesWithAttributesHaveHighestPriority1()
        {
            Stream responseStream = UnitTestsUtil.GetResponseStream(WebServerLocation.InProcess, null, "/$metadata", typeof(TypedCustomDataContext<MultiplePropertiesSatisfyingKeyCriteria1>));
            XmlDocument document = new XmlDocument(TestUtil.TestNameTable);
            document.Load(responseStream);

            XmlNode node = document.SelectSingleNode("//csdl:Key/csdl:PropertyRef[@Name='ActualID']", TestUtil.TestNamespaceManager);
            Assert.IsTrue(node != null, "Property with attribute should get the highest priority");
        }

        private static XmlElement GetElementForKind(XmlDocument document, BuiltInTypeKind kind)
        {
            Debug.Assert(document != null, "document != null");
            string xpath;
            switch (kind)
            {
                case BuiltInTypeKind.EdmProperty:
                    xpath = "/csdl1:Schema/csdl1:EntityType[@Name='Customers']/csdl1:Property[@Name='CompanyName']";
                    break;
                case BuiltInTypeKind.EntityContainer:
                    xpath = "/csdl1:Schema/csdl1:EntityContainer";
                    break;
                case BuiltInTypeKind.EntityType:
                    xpath = "/csdl1:Schema/csdl1:EntityType[@Name='Customers']";
                    break;
                case BuiltInTypeKind.EdmFunction:
                default:
                    throw new NotSupportedException("Built in type kind not supported: " + kind);
            }
            return (XmlElement)document.SelectSingleNode(xpath, TestUtil.TestNamespaceManager);
        }

        private static IEnumerable<Type> GetInvalidTypes()
        {
            foreach (Type type in typeof(InvalidBaseType_DuplicatePropertyName).Assembly.GetTypes())
            {
                if (type.IsPublic && type.Name.StartsWith("Invalid", StringComparison.Ordinal))
                {
                    yield return type;
                }
            }
        }

        public static IEnumerable<Type> GetObjectContextType(Assembly assembly)
        {
            // if there is no object context type in the assembly, just return an empty collection
            foreach (Type type in assembly.GetTypes())
            {
                // Only ObjectType types doesn't have a EdmTypeAttribute
                if (type.IsDefined(typeof(EdmTypeAttribute), false))
                {
                    continue;
                }

                yield return type;
            }
        }
    }
}
