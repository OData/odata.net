//---------------------------------------------------------------------
// <copyright file="SecurityTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces

    [Microsoft.Test.ModuleCore.TestModule]
    public partial class UnitTestModule : System.Data.Test.Astoria.AstoriaTestModule
    {
        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
        /// <summary>This is a test class for security features.</summary>
        [TestClass, Microsoft.Test.ModuleCore.TestCase]
        public class SecurityTest : System.Data.Test.Astoria.AstoriaTestCase
        {
            /// <summary>
            /// Verifies that query callbacks are made on $filter and $orderby properties.
            /// </summary>
            [Ignore] // Remove Atom
            // [TestMethod]
            public void SecurityCallbacksFilterTest()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("option", "$filter,$orderby".Split(',')));
                TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                {
                    string option = (string)values["option"];
                    int callCount = 0;
                    using (AstoriaUnitTests.Tests.UnitTestModule.AuthorizationTest.InitializationCallbackManager.RegisterStatic((s, e) =>
                        {
                            e.Configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
                        }))
                    using (StaticCallbackManager<AstoriaUnitTests.Tests.UnitTestModule.AuthorizationTest.ComposeQueryEventArgs>.RegisterStatic((s, e) =>
                        {
                            System.Linq.Expressions.Expression<Func<Customer, bool>> notZero =
                                c => c.ID != 0;
                            e.Filter = notZero;
                            callCount++;
                        }))
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.ServiceType = typeof(AstoriaUnitTests.Tests.UnitTestModule.AuthorizationTest.WebDataServiceA);
                        request.RequestUriString = "/Customers?" +
                            ((option == "$filter") ? "$filter=BestFriend/ID%20gt%200" : "$orderby=BestFriend/ID%20desc");
                        request.Accept = "application/atom+xml,application/xml";
                        request.SendRequest();
                        var document = request.GetResponseStreamAsXmlDocument();
                        Assert.AreEqual(2, callCount, "Callback is called twice (once for URI, once for best friend.");

                        // Customer with ID #2 has best friend with ID #1 and thus it's returned.
                        TestUtil.AssertSelectSingleElement(document, "/atom:feed/atom:entry/atom:id[text()='http://host/Customers(2)']");

                        if (option == "$filter")
                        {
                            // Customer #0 is not returned because of the filter (on the segment), and
                            // customer #1 is not returned because of the filter (on the navigation property).
                            TestUtil.AssertSelectSingleElement(document, "/atom:feed[0 = count(//atom:id[text()='http://host/Customers(0)'])]");
                            TestUtil.AssertSelectSingleElement(document, "/atom:feed[0 = count(//atom:id[text()='http://host/Customers(1)'])]");
                        }
                        else
                        {
                            // Customer #0 is not returned because of the filter (on the segment).
                            TestUtil.AssertSelectSingleElement(document, "/atom:feed[0 = count(//atom:id[text()='http://host/Customers(0)'])]");
                            TestUtil.AssertSelectSingleElement(document, "/atom:feed[1 = count(//atom:id[text()='http://host/Customers(1)'])]");
                        }
                    }
                });
            }

            /// <summary>
            /// Verifies that query callbacks are made on $filter and $orderby properties.
            /// </summary>
            [Ignore] // Remove Atom
            // [TestMethod]
            public void SecurityCallbacksFilterEdmTest()
            {
                ServiceModelData.Northwind.EnsureDependenciesAvailable();
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("option", "$filter,$orderby".Split(',')));
                TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                {
                    string option = (string)values["option"];
                    int callCount = 0;
                    using (AstoriaUnitTests.Tests.UnitTestModule.AuthorizationTest.InitializationCallbackManager.RegisterStatic((s, e) =>
                    {
                        e.Configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
                    }))
                    using (StaticCallbackManager<AstoriaUnitTests.Tests.UnitTestModule.AuthorizationTest.ComposeQueryEventArgs>.RegisterStatic((s, e) =>
                    {
                        System.Linq.Expressions.Expression<Func<NorthwindModel.Customers, bool>> notAnatr =
                            c => c.CustomerID != "ANATR";
                        e.Filter = notAnatr;
                        callCount++;
                    }))
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        // In Northwind, the two first customers are 'ALFKI' and 'ANATR'.
                        // 'ALFKI' has these orders: 10643, 10692, 10702, 10835, 10952, 11011.
                        // 'ANATR' has these orders: 10308, 10625, 10759, 10926.
                        request.ServiceType = typeof(AstoriaUnitTests.Tests.UnitTestModule.AuthorizationTest.WebDataServiceEdmCustomerCallback);
                        request.RequestUriString = "/Orders?" +
                            ((option == "$filter") ?
                             "$filter=startswith(Customers/CustomerID, 'A')" :
                             "$filter=startswith(Customers/CustomerID,%20'A')%20or%20Customers%20eq%20null&$orderby=Customers/CustomerID");
                        request.Accept = "application/atom+xml,application/xml";
                        request.SendRequest();
                        var document = request.GetResponseStreamAsXmlDocument();
                        if (option == "$filter")
                        {
                            Assert.AreEqual(1, callCount, "Callback is called a single time (for the navigation property.");

                            // The orders for 'ANATR' should not be returned
                            TestUtil.AssertSelectSingleElement(document, "/atom:feed[0 = count(atom:entry/atom:id[text()='http://host/Orders(10308)'])]");
                            TestUtil.AssertSelectSingleElement(document, "/atom:feed[0 = count(atom:entry/atom:id[text()='http://host/Orders(10625)'])]");

                            // ALFKI is OK.
                            TestUtil.AssertSelectSingleElement(document, "/atom:feed[1 = count(atom:entry/atom:id[text()='http://host/Orders(10643)'])]");
                        }
                        else
                        {
                            Assert.AreEqual(3, callCount, "Callback is called a three tims (for the navigation property)");

                            // ANATR will come before ALFKI
                            System.Xml.XmlElement alfkiOrder =
                                TestUtil.AssertSelectSingleElement(document, "/atom:feed/atom:entry[atom:id/text()='http://host/Orders(10643)']");
                            System.Xml.XmlElement anatrOrder =
                                TestUtil.AssertSelectSingleElement(document, "/atom:feed/atom:entry[atom:id/text()='http://host/Orders(10308)']");
                            bool found = false;
                            System.Xml.XmlNode node = alfkiOrder;
                            while (node != null)
                            {
                                if (node == anatrOrder)
                                {
                                    found = true;
                                    break;
                                }
                                else
                                {
                                    node = node.PreviousSibling;
                                }
                            }
                            Assert.IsTrue(found, "ANATR orders sort after ALFKI");
                        }
                    }
                });
            }
            [Ignore] // Remove Atom
            // [TestMethod]
            public void SecurityMaxResultsTest()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("SerializationFormatData", SerializationFormatData.StructuredValues),
                    new Dimension("Expand", new bool[] { true, false }),
                    new Dimension("LinksOnly", new bool[] { true, false }),
                    new Dimension("OrderCount", new int[] { 0, 1, 2, 3 }),
                    new Dimension("CustomerCount", new int[] { 0, 1, 2, 3 }),
                    new Dimension("MaxResultsPerCollection", new int[] { Int32.MaxValue, 0, 1, 2, 3 }));
                TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                {
                    SerializationFormatData format = (SerializationFormatData)values["SerializationFormatData"];
                    bool expand = (bool)values["Expand"];
                    int customerCount = (int)values["CustomerCount"];
                    int orderCount = (int)values["OrderCount"];
                    bool linksOnly = (bool)values["LinksOnly"];
                    int maxResultsPerCollection = (int)values["MaxResultsPerCollection"];

                    // $ref only makes sense if we want to bring down the related items.
                    if (linksOnly && !expand)
                    {
                        return;
                    }

                    TestUtil.ClearConfiguration();
                    using (var request = TestWebRequest.CreateForInProcess())
                    {
                        request.TestArguments = new Hashtable();
                        request.TestArguments[CustomDataContext.CustomerCountArgument] = customerCount;
                        request.TestArguments[CustomDataContext.OrderCountArgument] = orderCount;
                        request.ServiceType = typeof(AuthorizationTest.WebDataServiceA);
                        request.Accept = format.MimeTypes[0];
                        request.RequestUriString = expand ?
                            (linksOnly ? "/Customers(0)/Orders/$ref" : "/Customers?$expand=Orders") :
                            "/Customers";
                        if (linksOnly && format == SerializationFormatData.Atom)
                        {
                            request.Accept = "*/*";
                        }

                        using (AuthorizationTest.InitializationCallbackManager.RegisterStatic((sender, args) =>
                        {
                            args.Configuration.MaxResultsPerCollection = maxResultsPerCollection;
                            args.Configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
                        }))
                        {
                            Exception exception = TestUtil.RunCatching(delegate()
                            {
                                request.SendRequest();
                                string response = request.GetResponseStreamAsText();
                                if (response.Contains("<m:error") || response.Contains("\"error\":")) throw new Exception(response);
                            });

                            TestUtil.AssertExceptionExpected(exception,
                                customerCount > maxResultsPerCollection && !linksOnly,
                                expand && orderCount > maxResultsPerCollection && customerCount > 0);
                        }
                    }
                });
            }

            // [TestMethod] Removed due to local IIS server instance racing on build machine.
            public void SecurityPartialTrustTest()
            {
                // Repro: Astoria not working in partial trust with the EF
                string savedPath = LocalWebServerHelper.FileTargetPath;
                LocalWebServerHelper.Cleanup();
                try
                {
                    string[] trustLevels = new string[] { null, "High", "Medium" };
                    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                        new Dimension("trustLevel", trustLevels));
                    LocalWebServerHelper.FileTargetPath = Path.Combine(Path.GetTempPath(), "SecurityPartialTrustTest");
                    IOUtil.EnsureEmptyDirectoryExists(LocalWebServerHelper.FileTargetPath);
                    LocalWebServerHelper.StartWebServer();
                    TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                    {
                        string trustLevel = (string)values["trustLevel"];

                        if (trustLevel == "Medium" || trustLevel == "High")
                        {
                            Trace.WriteLine(
                                "'Medium'/'High' trust level cannot be tested reliably in unit tests, " +
                                "because it may run into problems depending on the local file system " +
                                "permissions and/or the local web server (as far as can be told). " +
                                "You can still use the generated files to prop them to IIS, set up an application " +
                                "and test there.");
                            return;
                        }

                        // Setup partial trust service.
                        SetupPartialTrustService(trustLevel);
                        string address, text = null;
                        Exception exception;
                        address = "http://localhost:" + LocalWebServerHelper.LocalPortNumber + "/service.svc/ASet?$expand=NavB";
                        Trace.WriteLine("Requesting " + address);
                        exception = TestUtil.RunCatching(delegate() { text = new WebClient().DownloadString(address); });
                        WriteExceptionOrText(exception, text);

                        // Note: the second argument should be 'false' even for medium trust.
                        TestUtil.AssertExceptionExpected(exception, false);

                        // Invoke something that would normally fail.
                        address = "http://localhost:" + LocalWebServerHelper.LocalPortNumber + "/service.svc/Q";
                        text = null;
                        Trace.WriteLine("Requesting " + address);
                        exception = TestUtil.RunCatching(delegate() { text = new WebClient().DownloadString(address); });
                        WriteExceptionOrText(exception, text);
                        TestUtil.AssertExceptionExpected(exception,
                            trustLevel == "Medium" && !text.Contains("error") // The error may come in the body
                            );
                    });
                }
                finally
                {
                    LocalWebServerHelper.DisposeProcess();
                    LocalWebServerHelper.Cleanup();
                    LocalWebServerHelper.FileTargetPath = savedPath;
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod]
            public void FriendlyFeedsInMediumTrust()
            {
                string savedPath = LocalWebServerHelper.FileTargetPath;
                LocalWebServerHelper.Cleanup();
                try
                {
                    string[] trustLevels = new string[] { null, "High", "Medium" };
                    LocalWebServerHelper.FileTargetPath = Path.Combine(Path.GetTempPath(), "SecurityPartialTrustTest");
                    IOUtil.EnsureEmptyDirectoryExists(LocalWebServerHelper.FileTargetPath);
                    LocalWebServerHelper.StartWebServer();
                    TestUtil.RunCombinations(trustLevels, (trustLevel) =>
                    {
                        // Setup partial trust service.
                        SetupPartialTrustService(trustLevel);
                        string address, text = null;
                        Exception exception;
                        address = "http://localhost:" + LocalWebServerHelper.LocalPortNumber + "/service.svc/$metadata";
                        Trace.WriteLine("Requesting " + address);
                        exception = TestUtil.RunCatching(delegate() { text = new WebClient().DownloadString(address); });
                        WriteExceptionOrText(exception, text);
                        TestUtil.AssertExceptionExpected(exception, false);
                    });
                }
                finally
                {
                    LocalWebServerHelper.DisposeProcess();
                    LocalWebServerHelper.Cleanup();
                    LocalWebServerHelper.FileTargetPath = savedPath;
                }
            }

            private static void SetupPartialTrustService(string trustLevel)
            {
                string binLocation = Path.Combine(LocalWebServerHelper.FileTargetPath, "bin");
                SetupPartialTrustServiceConfig(trustLevel);
                SetupPartialTrustServiceFile();
                Directory.CreateDirectory(binLocation);
                LocalWebServerHelper.CopyProductBinaries(binLocation);
            }

            private static void SetupPartialTrustServiceFile()
            {
                string servicePath = Path.Combine(LocalWebServerHelper.FileTargetPath, "service.svc");
                string restrictedFilePath = Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), @"config\machine.config");
                string escapedRestrictedFilePath = restrictedFilePath.Replace(@"\", @"\\");
                string serviceContents =
                    "<%@ ServiceHost Language=\"C#\" Debug=\"true\" Factory=\"Microsoft.OData.Service.DataServiceHostFactory\" Service=\"AstoriaTest.TheDataService\" %>\r\n" +
                    "namespace AstoriaTest\r\n" +
                    "{\r\n" +
                    "    using System;\r\n" +
                    "    using System.Collections.Generic;\r\n" +
                    "    using System.Linq;\r\n" +
                    "    using System.Linq.Expressions;\r\n" +
                    "    using Microsoft.OData.Service;\r\n" +
                    "    \r\n" +
                    "    public class A { public int ID { get; set; } public string Name { get; set; } public B NavB { get; set; } }\r\n" +
                    "    public class B { public int ID { get; set; } }\r\n" +
                    "    \r\n" +
                    "    public class TheContext \r\n" +
                    "    { \r\n" +
                    "        private List<A> a = new List<A>();\r\n" +
                    "        private List<B> b = new List<B>();\r\n" +
                    "        \r\n" +
                    "        public TheContext ()\r\n" +
                    "        {\r\n" +
                    "            this.b.Add(new B() { ID = 1 });\r\n" +
                    "            this.a.Add(new A() { ID = 1, Name = \"A1\", NavB = this.b[0] });\r\n" +
                    "        }\r\n" +
                    "        \r\n" +
                    "        public IQueryable<A> ASet { get { return this.a.AsQueryable(); } }\r\n" +
                    "        public IQueryable<B> BSet { get { return this.b.AsQueryable(); } }\r\n" +
                    "    }\r\n" +
                    "    \r\n" +
                    "    [System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]\r\n" +
                    "    public class TheDataService : Microsoft.OData.Service.DataService<TheContext>\r\n" +
                    "    {\r\n" +
                    "        [QueryInterceptor(\"BSet\")]\r\n" +
                    "        public Expression<Func<B, bool>> F() { return b => true; }\r\n" +
                    "\r\n" +
                    "        [System.ServiceModel.Web.WebGet]\r\n" +
                    "        public IQueryable<string> Q() {\r\n" +
                    "          return from s in new string[] { \"" + escapedRestrictedFilePath + "\" }.AsQueryable()\r\n" +
                    "                 select System.IO.File.ReadAllText(s);\r\n" +
                    "        }\r\n" +
                    "\r\n" +
                    "        public static void InitializeService(Microsoft.OData.Service.IDataServiceConfiguration configuration)\r\n" +
                    "        {\r\n" +
                    "            configuration.SetEntitySetAccessRule(\"*\", Microsoft.OData.Service.EntitySetRights.All);\r\n" +
                    "            configuration.SetServiceOperationAccessRule(\"*\", Microsoft.OData.Service.ServiceOperationRights.All);\r\n" +
                    "            configuration.UseVerboseErrors = true;\r\n" +
                    "        }\r\n" +
                    "    }\r\n" +
                    "}\r\n";
                File.WriteAllText(servicePath, serviceContents);
            }

            private static void SetupPartialTrustServiceConfig(string trustLevel)
            {
                string webConfigPath = Path.Combine(LocalWebServerHelper.FileTargetPath, "web.config");
                string webConfigContents =
                    "<?xml version='1.0'?>\r\n" +
                    "<configuration>\r\n" +
                    " <system.web>\r\n" +
                    ((trustLevel == null) ? "" : "  <trust level=\"" + trustLevel + "\" />\r\n") +
                    LocalWebServerHelper.WebConfigCompilationFragment +
                    " </system.web>\r\n" +
                    LocalWebServerHelper.WebConfigCodeDomFragment +
                    "</configuration>\r\n";
                File.WriteAllText(webConfigPath, webConfigContents);
            }

            private static void WriteExceptionOrText(Exception exception, string text)
            {
                if (exception == null)
                {
                    Trace.WriteLine("Text: " + text);
                }
                else
                {
                    if (exception is WebException)
                    {
                        Trace.WriteLine(exception);
                        Stream stream = ((WebException)exception).Response.GetResponseStream();
                        if (stream != null)
                        {
                            using (StreamReader reader = new StreamReader(((WebException)exception).Response.GetResponseStream()))
                            {
                                Trace.WriteLine(reader.ReadToEnd());
                            }
                        }
                        else
                        {
                            Trace.WriteLine("No response stream on WebException.");
                        }
                    }
                    else
                    {
                        Trace.WriteLine("Expecting a WebException, but found: " + exception);
                    }
                }
            }

            /// <summary>Verifies that all stack-consuming features are properly guarded.</summary>
            [Ignore] // Remove Atom
            // [TestMethod]
            public void SecurityStackOverflowTest()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Feature", StackConsumingFeatureData.Values),
                    new Dimension("UseCollections", new bool [] { false, true }));
                TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                {
                    StackConsumingFeatureData feature = (StackConsumingFeatureData)values["Feature"];
                    WebServerLocation location = WebServerLocation.InProcess;
                    bool useCollections = (bool)values["UseCollections"];

                    using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
                    {
                        if (useCollections)
                        {
                            // Collection switch only does a collection version of the test for serializers
                            if (feature.Feature != StackConsumingFeature.AtomSerializer &&
                                feature.Feature != StackConsumingFeature.JsonSerializer &&
                                feature.Feature != StackConsumingFeature.XmlSerializer)
                            {
                                return;
                            }
                            request.RequestVersion = "4.0";
                            request.RequestMaxVersion = "4.0";
                        }

                        feature.SetupOverflowRequest(request, useCollections);
                        Exception exception = TestUtil.RunCatching(request.SendRequest);

                        TestUtil.AssertExceptionExpected(exception, true);
                        TestUtil.AssertExceptionStatusCode(exception, feature.ExpectedStatusCode, "");
                        if (location == WebServerLocation.InProcess)
                        {
                            TestUtil.AssertContains(request.GetResponseStreamAsText(), feature.ErrorMessageKey);
                        }
                    }
                });
            }

            internal enum StackConsumingFeature
            {
                AtomDeserializer,
                JsonDeserializer,
                JsonReader,
                RequestQueryParser,
                RequestUriProcessor,
                AtomSerializer,
                JsonSerializer,
                XmlSerializer,
            }

            internal class StackConsumingFeatureData
            {
                private static StackConsumingFeatureData[] values;
                private readonly StackConsumingFeature feature;

                private StackConsumingFeatureData(StackConsumingFeature feature)
                {
                    this.feature = feature;
                }

                public static StackConsumingFeatureData[] Values
                {
                    get
                    {
                        if (values == null)
                        {
                            values = TestUtil.SelectEnumValues(
                                (StackConsumingFeature c) => new StackConsumingFeatureData((StackConsumingFeature)c))
                                .ToArray();
                        }

                        return values;
                    }
                }

                public StackConsumingFeature Feature
                {
                    [DebuggerStepThrough]
                    get { return this.feature; }
                }

                public string ErrorMessageKey
                {
                    get
                    {
                        if (this.feature == StackConsumingFeature.RequestUriProcessor)
                        {
                            return "egment";
                        }
                        else if (this.feature == StackConsumingFeature.JsonReader)
                        {
                            // JSON reader is not recursive with ODL, so the request should fail cause the payload is wrong (with a generic error message).
                            return "An error occurred while processing this request.";
                        }
                        else
                        {
                            return "ecursion";
                        }
                    }
                }

                public int ExpectedStatusCode
                {
                    [DebuggerStepThrough]
                    get { return 400; }
                }

                public override string ToString()
                {
                    return this.Feature.ToString();
                }

                public void SetupOverflowRequest(TestWebRequest request, bool useCollections)
                {
                    TestUtil.CheckArgumentNotNull(request, "request");
                    StringBuilder builder;
                    int entryId = 100;
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/Customers";
                    switch (this.feature)
                    {
                        case StackConsumingFeature.AtomDeserializer:
                            // ATOM deserializer shares limits with JSON deserialized.
                            int atomDeserializerDepth = 105;
                            request.HttpMethod = "POST";
                            request.RequestContentType = SerializationFormatData.Atom.MimeTypes[0];
                            builder = new StringBuilder();
                            string entryTail = "xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                                AtomUpdatePayloadBuilder.GetCategoryXml(typeof(Customer).FullName);
                                // "<category term=\"" + typeof(Customer).FullName + "\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />";
                            for (int i = 0; i < atomDeserializerDepth; i++)
                            {
                                if (i != 0)
                                {
                                    builder.Append("<adsm:inline>");
                                }
                                if (i == 1)
                                {
                                    entryTail = "> " + AtomUpdatePayloadBuilder.GetCategoryXml(typeof(Customer).FullName);
                                }

                                builder.Append("<entry " + entryTail);
                                builder.Append("<content type=\"application/xml\"><adsm:properties>");
                                builder.Append("<ads:ID>" + (entryId++).ToString() + "</ads:ID></adsm:properties></content>");
                                builder.Append("<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' title='BestFriend'>");
                            }

                            for (int i = 0; i < atomDeserializerDepth; i++)
                            {
                                if (i != 0)
                                {
                                    builder.Append("</adsm:inline>");
                                }
                                builder.Append("</link></entry>");
                            }

                            request.SetRequestStreamAsText(builder.ToString());
                            break;
                        case StackConsumingFeature.JsonDeserializer:
                            // JSON deserializer survives 1000 nested objects but not 2000 on Vista Ultimate x86 chk under debugger. We hard-code to 100.
                            int jsonDeserializerDepth = 200;
                            request.HttpMethod = "POST";
                            request.RequestContentType = UnitTestsUtil.JsonLightMimeType;
                            builder = new StringBuilder();
                            for (int i = 0; i < jsonDeserializerDepth; i++)
                            {
                                builder.Append("{ @odata.type: \"" + typeof(Customer).FullName + "\" , ID: " + (entryId++).ToString() + ", BestFriend : ");
                            }
                            // builder.Append("null"); // null not supported
                            builder.Append(" { ID : 110000 } ");
                            builder.Append('}', jsonDeserializerDepth);
                            request.SetRequestStreamAsText(builder.ToString());
                            break;
                        case StackConsumingFeature.JsonReader:
                            // JSON reader is not recursive in ODL, so we pick a large depth here to verify that it doesn't break.
                            int jsonReaderDepth = 3000;
                            request.HttpMethod = "POST";
                            request.RequestContentType = UnitTestsUtil.JsonLightMimeType;
                            request.SetRequestStreamAsText(new string('[', jsonReaderDepth) + " abc ");
                            break;
                        case StackConsumingFeature.RequestQueryParser:
                            // Query parser survives 1000 nested expressions but not 5000 on Vista Ultimate x86 chk under the debugger. We hard-code to 100 nested expressions.
                            int queryParserDepth = 115;
                            request.RequestUriString += "?$filter=" + new string('(', queryParserDepth) + "true" + new string(')', queryParserDepth);
                            break;
                        case StackConsumingFeature.RequestUriProcessor:
                            // The Uri constructor already restricts to 65520 characters.
                            // Request processor is hard-coded to reject more than 100 segments of querying.
                            int processorDepth = 101;
                            builder = new StringBuilder(request.RequestUriString + "(1)");
                            for (int i = 0; i < processorDepth; i++)
                            {
                                builder.Append("/a");
                            }

                            request.RequestUriString = builder.ToString();
                            break;
                        case StackConsumingFeature.JsonSerializer:
                            request.DataServiceType = useCollections ? typeof(StackOverflowCollectionCustomDataContext) : typeof(StackOverflowCustomDataContext);
                            request.Accept = AstoriaUnitTests.Data.SerializationFormatData.JsonLight.MimeTypes[0];
                            break;
                        case StackConsumingFeature.AtomSerializer:
                            request.DataServiceType = useCollections ? typeof(StackOverflowCollectionCustomDataContext) : typeof(StackOverflowCustomDataContext);
                            request.Accept = AstoriaUnitTests.Data.SerializationFormatData.Atom.MimeTypes[0];
                            break;
                        case StackConsumingFeature.XmlSerializer:
                            request.DataServiceType = useCollections ? typeof(StackOverflowCollectionCustomDataContext) : typeof(StackOverflowCustomDataContext);
                            request.RequestUriString = "/Customers(1)/Address";
                            break;
                        default:
                            Debug.Fail("Unknown feature.");
                            break;
                    }
                }
            }

            public class StackOverflowAddress
            {
                public StackOverflowAddress Child { get; set; }
                public string Value { get; set; }
            }

            public class StackOverflowCustomer
            {
                public int ID { get; set; }
                public StackOverflowAddress Address { get; set; }
            }

            public class StackOverflowCustomDataContext
            {
                public IQueryable<StackOverflowCustomer> Customers
                {
                    get
                    {
                        const int AddressCount = 2000;
                        StackOverflowCustomer customer = new StackOverflowCustomer();
                        customer.ID = 1;
                        StackOverflowAddress address = new StackOverflowAddress();
                        customer.Address = address;
                        for (int i = 0; i < AddressCount; i++)
                        {
                            address.Child = new StackOverflowAddress();
                            address = address.Child;
                        }
                        return (new StackOverflowCustomer[] { customer }).AsQueryable();
                    }
                }
            }

            public class StackOverflowCollectionAddress
            {
                public IList<StackOverflowCollectionAddress> Children { get; set; }
                public string Value { get; set; }
            }

            public class StackOverflowCollectionCustomer
            {
                public int ID { get; set; }
                public StackOverflowCollectionAddress Address { get; set; }
            }

            public class StackOverflowCollectionCustomDataContext
            {
                public IQueryable<StackOverflowCollectionCustomer> Customers
                {
                    get
                    {
                        const int AddressCount = 2000;
                        StackOverflowCollectionCustomer customer = new StackOverflowCollectionCustomer();
                        customer.ID = 1;
                        StackOverflowCollectionAddress address = new StackOverflowCollectionAddress();
                        customer.Address = address;
                        for (int i = 0; i < AddressCount; i++)
                        {
                            address.Children = new List<StackOverflowCollectionAddress> { new StackOverflowCollectionAddress() };
                            address = address.Children[0];
                        }
                        return (new StackOverflowCollectionCustomer[] { customer }).AsQueryable();
                    }
                }
            }
        }
    }
}
