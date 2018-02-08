//---------------------------------------------------------------------
// <copyright file="RegressionTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.EntityClient;
    using Microsoft.OData.Service;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.Data.Test.Astoria.Util;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using edm = System.Data.Metadata.Edm;
    using ocs = AstoriaUnitTests.ObjectContextStubs;
    using p = Microsoft.OData.Service.Providers;

    #endregion Namespaces

    [TestModule]
    public partial class RegressionUnitTestModule : AstoriaTestModule
    {
        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/876
        /// <summary>This is a test class for adding regression tests.</summary>
        [DeploymentItem("Workspaces", "Workspaces")]
        [TestClass, TestCase]
        public class RegressionTest : AstoriaTestCase
        {
            [ClassInitialize()]
            public static void PerClassSetup(TestContext context)
            {
                AstoriaTestProperties.Host = Host.Cassini;
            }

            [TestMethod, Variation]
            public void PostEntityToEmptyDerivedType()
            {
                using (CustomDataContext.CreateChangeScope())
                {
                    const string payLoad = "{" +
                                           "@odata.type : \"#AstoriaUnitTests.Stubs.CustomerWithoutProperties\"," +
                                           "Name: \"Bar\"," +
                                           "ID: 125" +
                                           "}";

                    UnitTestsUtil.DoInserts(payLoad, "/Customers",
                        new string[] { String.Format("/{0}[ID=125 and Name='Bar' and odata.type='#{1}']",
                                JsonValidator.ObjectString,
                                typeof(CustomerWithoutProperties).FullName) },
                        typeof(CustomDataContext), UnitTestsUtil.JsonLightMimeType);
                }
            }

            [TestMethod, Variation("PUT with Absolute and relative payload")]
            public void PutWithAbsoluteAndRelativePayload()
            {
                WebServerLocation location = WebServerLocation.InProcessWcf;
                using (CustomDataContext.CreateChangeScope())
                using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
                {
                    string responseFormat = UnitTestsUtil.JsonLightMimeType;

                    Trace.WriteLine("Getting existing customer.../Customers(0)");
                    request.DataServiceType = typeof(CustomDataContext);
                    request.TestArguments = new Hashtable();
                    request.TestArguments[CustomDataContext.PreserveChangesArgument] = true;
                    request.HttpMethod = "GET";
                    request.RequestContentType = responseFormat;
                    request.RequestUriString = "/Customers(0)";
                    request.SendRequest();
                    string etag0 = request.ResponseETag;
                    Trace.WriteLine(String.Format("Etag value for Customers(0): '{0}'", etag0));

                    Trace.WriteLine("Getting existing customer.../Customers(1)");
                    request.RequestUriString = "/Customers(1)";
                    request.SendRequest();
                    string etag1 = request.ResponseETag;
                    Trace.WriteLine(String.Format("Etag value for Customers(1): '{0}'", etag1));

                    string newPayload =
                         "{" +
                            "@odata.type:\"#" + typeof(Customer).FullName + "\"," +
                            "Name: \"Foo1\"" +
                         "}";

                    request.HttpMethod = "PUT";
                    request.RequestUriString = "/Customers(0)";
                    request.SetRequestStreamAsText(newPayload);
                    request.IfMatch = etag0;
                    request.SendRequest();
                    Trace.WriteLine(request.GetResponseStreamAsText());

                    newPayload = "{" +
                                    "@odata.type:\"#AstoriaUnitTests.Stubs.CustomerWithBirthday\"," +
                                    "Name: \"Foo\"" +
                                 "}";

                    request.RequestUriString = "/Customers(1)";
                    request.SetRequestStreamAsText(newPayload);
                    request.IfMatch = etag1;
                    request.SendRequest();
                    Trace.WriteLine(request.GetResponseStreamAsText());

                    Trace.WriteLine("Getting new customer /Customers(1)...");
                    request.HttpMethod = "GET";
                    //
                    // After the PUT, we must reset content length for GET
                    // Otherwise it's a protocol violation
                    //
                    request.RequestContentLength = 0;
                    request.IfMatch = null;
                    request.RequestUriString = "/Customers(1)";
                    request.RequestStream = null;
                    request.Accept = responseFormat;
                    request.SendRequest();
                    using (Stream stream = request.GetResponseStream())
                    {
                        UnitTestsUtil.VerifyXPaths(stream, responseFormat,
                            new string[] { String.Format("/{0}[ID=1 and Name='Foo']",
                                                    JsonValidator.ObjectString)
                                         });
                    }

                    Trace.WriteLine("Getting new customer /Customers(0)...");
                    request.RequestUriString = "/Customers(0)";
                    request.RequestStream = null;
                    request.SendRequest();
                    using (Stream stream = request.GetResponseStream())
                    {
                        UnitTestsUtil.VerifyXPaths(stream,
                            responseFormat,
                            new string[] {
                                String.Format("/{0}[ID=0 and Name='Foo1']",
                                    JsonValidator.ObjectString,
                                    typeof(Customer).FullName)});
                    }
                }
            }

            // ODataLib now correctly ignores __deferred properties in WCF DS Server.
            [Ignore] // Remove Atom
            // [TestMethod, Variation("PATCH with the same payload as returned by the GET method")]
            public void PatchPayloadReturnedByGet_ReflectionProvider()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("ResponseFormat", UnitTestsUtil.ResponseFormats),
                    new Dimension("ProviderType", UnitTestsUtil.ProviderTypes)
                );

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    string responseFormat = (string)values["ResponseFormat"];
                    Type contextType = (Type)values["ProviderType"];

                    using (UnitTestsUtil.CreateChangeScope(contextType))
                    using (TestUtil.RestoreStaticValueOnDispose(typeof(BaseTestWebRequest), "HostInterfaceType"))
                    {
                        BaseTestWebRequest.HostInterfaceType = typeof(Microsoft.OData.Service.IDataServiceHost2);
                        string uri = UnitTestsUtil.ConvertUri(contextType, "/Customers(1)");
                        TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(responseFormat, uri, contextType, null, "GET");
                        string etag = request.ResponseETag;
                        string payload = System.Data.Test.Astoria.Util.JsonPrimitiveTypesUtil.FilterJson(request.GetResponseStreamAsText());

                        TestUtil.AssertContains(payload, "Customer 1");
                        TestUtil.AssertContainsFalse(payload, "NewName");
                        payload = payload.Replace("Customer 1", "NewName");

                        var headerValues = new KeyValuePair<string, string>[]
                        {
                            new KeyValuePair<string, string>("If-Match", etag),
                            new KeyValuePair<string, string>("Prefer", "return=representation")
                        };
                        request = UnitTestsUtil.GetTestWebRequestInstance(responseFormat, uri, contextType, headerValues, "PATCH", payload);
                        payload = request.GetResponseStreamAsText();

                        TestUtil.AssertContainsFalse(payload, "Customer 1");
                        TestUtil.AssertContains(payload, "NewName");
                    }
                });
            }

            // ODataLib now correctly ignores __deferred properties in WCF DS Server.
            [Ignore] // Remove Atom
            // [TestMethod, Variation("PUT with the same payload as returned by the GET method")]
            public void PutPayloadReturnedByGet_EdmProvider()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("HttpMethod", new string[] { "PUT", "PATCH" }),
                    new Dimension("ResponseFormat", UnitTestsUtil.ResponseFormats),
                    new Dimension("WebServerLocation", new object[] { WebServerLocation.InProcess })
                );

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    WebServerLocation location = (WebServerLocation)values["WebServerLocation"];
                    string responseFormat = (string)values["ResponseFormat"];
                    string httpMethodName = (string)values["HttpMethod"];

                    if (responseFormat == UnitTestsUtil.AtomFormat && httpMethodName == "PUT")
                    {
                        // PUT atom payloads are not round-trippable since we try and attach the relationships
                        // which is not allowed in PUT payloads.
                        return;
                    }

                    Type contextType = typeof(ocs.CustomObjectContext);

                    ocs.PopulateData.EntityConnection = null;
                    using (EntityConnection connection = ocs.PopulateData.CreateTableAndPopulateData())
                    {
                        TestWebRequest webRequest = UnitTestsUtil.GetTestWebRequestInstance(responseFormat, "/Customers(1)", contextType, null, "GET");
                        Stream getResponseStream = webRequest.GetResponseStream();
                        string payload = System.Data.Test.Astoria.Util.JsonPrimitiveTypesUtil.FilterJson((new StreamReader(getResponseStream)).ReadToEnd());

                        Assert.IsTrue(payload.Contains("Customer 1"), "making sure this value is there");
                        Assert.IsFalse(payload.Contains("NewName"), "make sure this value is not there");
                        payload = payload.Replace("Customer 1", "NewName");

                        var headers = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("If-Match", webRequest.ResponseETag) };
                        webRequest = UnitTestsUtil.GetTestWebRequestInstance(responseFormat, "/Customers(1)", contextType, headers, "PATCH", payload);
                        getResponseStream = UnitTestsUtil.GetResponseStream(location, responseFormat, "/Customers(1)", contextType);
                        payload = new StreamReader(getResponseStream).ReadToEnd();

                        TestUtil.AssertContainsFalse(payload, "Customer 1");
                        TestUtil.AssertContains(payload, "NewName");
                    }
                });
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("xml base + href not forming proper uri's")]
            public void XmlBaseHRefShouldFormProperUri()
            {
                string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                     "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                        AtomUpdatePayloadBuilder.GetCategoryXml(UnitTestsUtil.CustomerTypeName) +
                         "<content type=\"application/xml\"><adsm:properties>" +
                             "<ads:Name>Foo</ads:Name>" +
                             "<ads:ID>125</ads:ID>" +
                             "<ads:Address>" +
                                 "<ads:StreetAddress>Street Number, Street Address</ads:StreetAddress>" +
                                 "<ads:City>Redmond</ads:City>" +
                                 "<ads:PostalCode>98052</ads:PostalCode>" +
                             "</ads:Address>" +
                         "</adsm:properties></content>" +
                     "</entry>";

                string[] atomXPaths = new string[] {
                    String.Format("/atom:entry[atom:category[@term='#{0}'] and atom:id='http://host/Customers(125)' and atom:content/adsm:properties[ads:Name='Foo' and ads:ID=125 and ads:Address[ads:StreetAddress='Street Number, Street Address' and ads:City='Redmond' and ads:PostalCode='98052']]]",
                                  UnitTestsUtil.CustomerTypeName)};

                ocs.PopulateData.EntityConnection = null;
                using (EntityConnection connection = ocs.PopulateData.CreateTableAndPopulateData())
                {
                    UnitTestsUtil.DoInserts(
                        atomPayload, "/Customers/", atomXPaths,
                        typeof(ocs.CustomObjectContext), UnitTestsUtil.AtomFormat);
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Assert when an invalid uri is specified for binding in Json")]
            //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
            public void ShouldThrowWhenBindingInvalidUriInJson()
            {
                //#region jsonPayload and XPaths
                //string jsonPayload = "{" +
                //                    "odata.type : \"AstoriaUnitTests.Stubs.Customer\"," +
                //                    "Name: \"Bar\"," +
                //                    "ID: 125," +
                //                    "BestFriend: { " +
                //                        //"odata.type: \"AstoriaUnitTests.Stubs.BestFriend\"," +
                //                        "odata.readlink:\"/Orders(1)\"" +
                //                    "}" +
                //                 "}";
                //#endregion

                #region AtomPayload and XPaths
                string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<entry xml:base=\"/\" xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns='http://www.w3.org/2005/Atom'>" +
                        AtomUpdatePayloadBuilder.GetCategoryXml(typeof(Customer).FullName) +
                        "<content type=\"application/xml\"><adsm:properties>" +
                            "<ads:Name>Bar</ads:Name>" +
                            "<ads:ID>125</ads:ID>" +
                        "</adsm:properties></content>" +
                        "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' title='Orders' href='Customers(1)' />" +
                    "</entry>";
                #endregion

                using (CustomDataContext.CreateChangeScope())
                using (ocs.CustomObjectContext.CreateChangeScope())
                {
                    // UnitTestsUtil.VerifyInvalidRequest(jsonPayload, "/Customers", typeof(CustomDataContext), UnitTestsUtil.JsonLightMimeType, "POST", 400, DataServicesResourceUtil.GetString("BadRequest_ErrorInSettingPropertyValue", "BestFriend"));
                    UnitTestsUtil.VerifyInvalidRequest(atomPayload, "/Customers", typeof(CustomDataContext), UnitTestsUtil.AtomFormat, "POST", 400, DataServicesResourceUtil.GetString("BadRequest_ErrorInSettingPropertyValue", "Orders"));
                    //UnitTestsUtil.VerifyInvalidRequest(UnitTestsUtil.ConvertReflectionProviderTypesToEdmProviderTypes(jsonPayload), "/Customers", typeof(ocs.CustomObjectContext), UnitTestsUtil.JsonLightMimeType, "POST", 400, DataServicesResourceUtil.GetString("BadRequest_ErrorInSettingPropertyValue", "BestFriend"));
                    UnitTestsUtil.VerifyInvalidRequest(UnitTestsUtil.ConvertReflectionProviderTypesToEdmProviderTypes(atomPayload), "/Customers", typeof(ocs.CustomObjectContext), UnitTestsUtil.AtomFormat, "POST", 400, DataServicesResourceUtil.GetString("BadRequest_ErrorInSettingPropertyValue", "Orders"));
                }
            }

            [TestMethod, Variation("Verifying if we are not trimming white spaces for etags")]
            public void ShouldNotTrimWhitespaceFromEtag()
            {
                ocs.PopulateData.EntityConnection = null;
                using (EntityConnection connection = ocs.PopulateData.CreateTableAndPopulateData())
                {
                    string etag = UnitTestsUtil.GetETagFromResponse(typeof(ocs.CustomObjectContext), "/Customers(0)", UnitTestsUtil.MimeAny);
                    Assert.IsTrue(Uri.UnescapeDataString(etag).StartsWith("W/\"'0    '"), "whitespaces should not stripped off for etag values");
                }
            }

            [TestMethod, Variation("Etag cannot be specified if expand query string is specified")]
            public void CannotSpecifyEtagIfExpandQueryPresent()
            {
                var ifMatch = new KeyValuePair<string, string>("If-Match", "W/\"dfsdhfsdfdsf\"");
                var ifNoneMatch = new KeyValuePair<string, string>("If-None-Match", "W/\"dfsdhfsdfdsf\""); ;
                UnitTestsUtil.VerifyInvalidRequestForVariousProviders(null, "/Customers(1)?$expand=Orders", UnitTestsUtil.MimeAny, "GET", 400, ifMatch);
                UnitTestsUtil.VerifyInvalidRequestForVariousProviders(null, "/Customers(1)?$expand=Orders", UnitTestsUtil.MimeAny, "GET", 400, ifNoneMatch);
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation("Should throw if the link type attribute has invalid value")]
            public void ShouldThrowIfLinkTypeAttributeIsInvalid()
            {
                string customerFullName = typeof(Customer).FullName;
                string orderFullName = typeof(Order).FullName;

                string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<entry xml:base=\"/\" xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://www.w3.org/2005/Atom\" adsm:type=\"#AstoriaUnitTests.Stubs.Customer\">" +
                        "<content type=\"application/xml\"><adsm:properties>" +
                            "<ads:Name>Bar</ads:Name>" +
                            "<ads:ID>125</ads:ID>" +
                        "</adsm:properties></content>" +
                        "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' title='BestFriend' type='application/atom+xml;type=feed' href='Customers(1)' />" +
                    "</entry>";

                UnitTestsUtil.VerifyInvalidRequest(atomPayload, "/Customers", typeof(CustomDataContext), UnitTestsUtil.AtomFormat, "POST", 400);

                atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                        AtomUpdatePayloadBuilder.GetCategoryXml(customerFullName) +
                        "<content type=\"application/xml\"><adsm:properties>" +
                            "<ads:Name>Foo</ads:Name>" +
                            "<ads:ID>125</ads:ID>" +
                        "</adsm:properties></content>" +
                        "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' title='BestFriend'>" +
                            "<adsm:inline>" +
                                "<entry adsm:type='" + customerFullName + "'>" +
                                    "<content type=\"application/xml\"><adsm:properties>" +
                                        "<ads:Name>FooBestFriend</ads:Name>" +
                                        "<ads:ID>126</ads:ID>" +
                                    "</adsm:properties></content>" +
                                    "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' title='Orders' type='application/atom+xml;type=entry'>" +
                                        "<adsm:inline>" +
                                            "<feed>" +
                                                "<entry>" +
                                                    AtomUpdatePayloadBuilder.GetCategoryXml(orderFullName) +
                                                    "<content type=\"application/xml\"><adsm:properties>" +
                                                        "<ads:ID>151</ads:ID>" +
                                                        "<ads:DollarAmount adsm:type='Edm.Double'>1500.00</ads:DollarAmount>" +
                                                    "</adsm:properties></content>" +
                                                "</entry>" +
                                                "<entry>" +
                                                    AtomUpdatePayloadBuilder.GetCategoryXml(orderFullName) +
                                                    "<content type=\"application/xml\"><adsm:properties>" +
                                                        "<ads:ID>152</ads:ID>" +
                                                        "<ads:DollarAmount adsm:type='Edm.Double'>500.00</ads:DollarAmount>" +
                                                    "</adsm:properties></content>" +
                                                "</entry>" +
                                                "<entry>" +
                                                    AtomUpdatePayloadBuilder.GetCategoryXml(orderFullName) +
                                                    "<content type=\"application/xml\"><adsm:properties>" +
                                                        "<ads:ID>153</ads:ID>" +
                                                        "<ads:DollarAmount adsm:type='Edm.Double'>0.00</ads:DollarAmount>" +
                                                    "</adsm:properties></content>" +
                                                "</entry>" +
                                                "<entry>" +
                                                    AtomUpdatePayloadBuilder.GetCategoryXml(orderFullName) +
                                                    "<id>/Orders(101)</id>" +
                                                "</entry>" +
                                            "</feed>" +
                                        "</adsm:inline>" +
                                    "</link>" +
                                "</entry>" +
                            "</adsm:inline>" +
                        "</link>" +
                    "</entry>";

                UnitTestsUtil.VerifyInvalidRequest(atomPayload, "/Customers", typeof(CustomDataContext), UnitTestsUtil.AtomFormat, "POST", 400);
            }

            [TestMethod, Variation("Interceptors should be applied to navigation properties for CLR providers.")]
            public void InterceptorsShouldApplyToNavigationPropertyForClrProviders()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.ServiceType = typeof(TestService);
                    request.RequestUriString = "/Customers(1)/Orders";
                    request.SendRequest();
                    string text = request.GetResponseStreamAsText();
                    TestUtil.AssertContainsFalse(text, "Orders(101)");
                }
            }

            public class TestService : OpenWebDataService<CustomDataContext>
            {
                [QueryInterceptor("Orders")]
                public Expression<Func<Order, bool>> FilterOrders()
                {
                    return o => o.ID < 100;
                }
            }

            [TestMethod, Variation("Allowing base types in the Query Interceptors - Won't Fix")]
            public void AllowBaseTypesInQueryInterceptors()
            {
                const string modelText =
                    "Ns.ET1 = entitytype { PK int key; };" +
                    "Ns.ET2 = entitytype : Ns.ET1 { Name string; };" +
                    "ES1 : Ns.ET2;";
                AdHocModel model = AdHocModel.ModelFromText(modelText);
                model.CreateDatabase();
                try
                {
                    Assembly assembly = model.GenerateModelsAndAssembly("Test", false);
                    Type objectContext = TestUtil.LoadDerivedTypeFromAssembly(assembly, typeof(System.Data.Objects.ObjectContext));
                    ModuleBuilder moduleBuilder = TestUtil.CreateModuleBuilder("TestModule", false);
                    TypeBuilder typeBuilder = moduleBuilder.DefineType(
                        "TestService",                                             // name
                        TypeAttributes.Class | TypeAttributes.Public,                   // attributes
                        typeof(OpenWebDataService<>).MakeGenericType(objectContext));   // parent
                    ReflectionUtility.DefineCreateDataSourceForEdm(typeBuilder, objectContext, model.DefaultEntityConnectionString);

                    Type baseEntityType = assembly.GetType("Ns.ET1", true);
                    Type returnType = typeof(Expression<>).MakeGenericType(
                        typeof(Func<,>).MakeGenericType(baseEntityType, typeof(bool)));
                    MethodBuilder builder = typeBuilder.DefineMethod(
                        "FilterET1",                // name
                        MethodAttributes.Public,    // attributes
                        returnType,                 // returnType
                        System.Type.EmptyTypes);    // parameters
                    // [QueryInterceptorAttribute("ES1")]
                    builder.SetCustomAttribute(
                        new CustomAttributeBuilder(
                            typeof(QueryInterceptorAttribute).GetConstructors().Single(),
                            new object[] { "ES1" }));
                    ReflectionUtility.GenerateThrowNotImplementedException(builder.GetILGenerator());

                    Type serviceType = typeBuilder.CreateType();
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.ServiceType = serviceType;
                        request.RequestUriString = "/ES1(1)";
                        request.Accept = UnitTestsUtil.MimeApplicationXml;
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception, true);
                        // This varies by web request type.
                        if (exception is WebException)
                        {
                            Assert.AreEqual(exception.GetType(), typeof(WebException));
                            Assert.AreEqual(exception.InnerException.GetType(), typeof(InvalidOperationException));
                        }
                        else
                        {
                            Assert.AreEqual(exception.GetType(), typeof(InvalidOperationException));
                        }
                        // The NotImplementedException is never thrown because the query interceptor is rejected.
                    }
                }
                finally
                {
                    model.DropDatabase();
                }
            }

            [TestMethod, Variation("Discovering types in EdmProvider, via the RegisterType mechanism")]
            public void UseRegisterTypeToDiscoverTypesInEdmProvider()
            {
                // checking this behaviour for edm provider
                string schemaFileLocation = Path.Combine(TestUtil.ServerUnitTestSamples, @"stubs\RegressionTest");
                string schemaFile1 = Path.Combine(schemaFileLocation, "Schema_1.csdl");
                string schemaFile2 = Path.Combine(schemaFileLocation, "Schema_2.csdl");
                string connectionString =
                    String.Format(@"provider=System.Data.SqlClient;metadata={0};provider connection string='server=tcp:markash420,1432;database=Northwind;uid=DataWorks;pwd=fakepwd;persist security info=true;connect timeout=60;MultipleActiveResultSets=true;'",
                        String.Format("{0}|{1}", schemaFile1, schemaFile2));
                string sourceFile = Path.Combine(schemaFileLocation, "Test_PartialClass.cs");

                // Generate assembly using the edm csdl file
                string assemblyLocation1 = TestUtil.GenerateAssembly(schemaFile1, false /*isReflectionServiceProvider*/, connectionString, new string[0], new string[0], new string[] { sourceFile });
                string assemblyLocation2 = TestUtil.GenerateAssembly(schemaFile2, false /*isReflectionServiceProvider*/, null, new string[] { schemaFile1 }, new string[] { assemblyLocation1 }, new string[0]);
                Assembly assembly1 = Assembly.LoadFrom(assemblyLocation1);
                Assembly assembly2 = Assembly.LoadFrom(assemblyLocation2);

                IEdmModel model;
                foreach (Type objectContextType in WebDataServiceTest.GetObjectContextType(assembly1))
                {
                    objectContextType.InvokeMember("RegisterType", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { assembly2.GetType("TestDerived.BugEntityDerived") });
                    model = UnitTestsUtil.LoadMetadataFromDataServiceType(objectContextType, null);
                    Assert.IsTrue(model.FindType("TestDerived.BugEntityDerived") != null, "Metadata for the type in another assembly was loaded");
                }

                // checking this behavior for reflection provider
                string sourceFile1 = Path.Combine(schemaFileLocation, "Test_ReflectionProvider_1.cs");
                string sourceFile2 = Path.Combine(schemaFileLocation, "Test_ReflectionProvider_2.cs");
                assemblyLocation1 = Path.Combine(TestUtil.GeneratedFilesLocation, "Test_ReflectionProvider_1.dll");
                assemblyLocation2 = Path.Combine(TestUtil.GeneratedFilesLocation, "Test_ReflectionProvider_2.dll");
                TestUtil.GenerateAssembly(new string[] { sourceFile1 }, assemblyLocation1, new string[0]);
                TestUtil.GenerateAssembly(new string[] { sourceFile2 }, assemblyLocation2, new string[] { assemblyLocation1 });
                assembly1 = Assembly.LoadFrom(assemblyLocation1);
                assembly2 = Assembly.LoadFrom(assemblyLocation2);

                Type serviceType = assembly1.GetType("TestReflectionProvider.TestDataContext");
                serviceType.InvokeMember("RegisterType", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { assembly2.GetType("TestReflectionProvider.TestEntityDerivedRP") });
                model = UnitTestsUtil.LoadMetadataFromDataServiceType(serviceType, null);
                Assert.IsTrue(model.FindType("TestReflectionProvider.TestEntityDerivedRP") != null, "Metadata for the type in another assembly was loaded in reflection provider");
            }

            [TestMethod, Variation("Unbind in Many-to-Many cases")]
            public void TestUnbindInManyToManyCases()
            {
                ocs.PopulateData.EntityConnection = null;
                using (EntityConnection connection = ocs.PopulateData.CreateTableAndPopulateDataForManyToMany())
                {
                    string jsonPayload = "{ Id: 999, " +
                                            "Name: \"Foo\"," +
                                            "Homes: [" +
                                                "{ Id: 900," +
                                                   "Address: \"foo, wa, 98052\"" +
                                                "}" +
                                            "]" +
                                         "}";

                    UnitTestsUtil.DoInserts(jsonPayload, "/Persons", new string[0], typeof(ocs.CustomManyToManyContainer), UnitTestsUtil.JsonLightMimeType);

                    var uriAndXPathsToVerify = new KeyValuePair<string, string[]>[] {
                        new KeyValuePair<string, string[]>("/Persons(999)/Homes", new string[] { String.Format("count(//{0})=1", JsonValidator.ObjectString) })};

                    UnitTestsUtil.SendRequestAndVerifyXPath(null, "/Persons(999)/Homes/$ref?$id=Homes(900)", uriAndXPathsToVerify, typeof(ocs.CustomManyToManyContainer), UnitTestsUtil.JsonLightMimeType, "DELETE", null, false);
                }
            }

            //[TestMethod, Variation("Open types - unable to find the right parent entity for the given uri")]
            //public void ShouldFindRightParentEntityForOpenTypes()
            //{
            //    CustomRowBasedOpenTypesContext.ClearData();
            //    CustomRowBasedOpenTypesContext.PreserveChanges = true;

            //    try
            //    {
            //        string payload = "{ value: 'Foo' }";
            //        var uriAndXPathToVerify = new KeyValuePair<string, string[]>[] {
            //        new KeyValuePair<string, string[]>("/Customers(1)/Address/StreetAddress",
            //            new string[] { String.Format("{0}[value='Foo']", JsonValidator.ObjectString) }) };

            //        // Get the etag for Customer(1)
            //        string etag = UnitTestsUtil.GetETagFromResponse(typeof(CustomRowBasedOpenTypesContext), "/Customers(1)", UnitTestsUtil.JsonLightMimeType);

            //        var headers = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("If-Match", etag) };
            //        UnitTestsUtil.SendRequestAndVerifyXPath(payload, "/Customers(1)/Address/StreetAddress", uriAndXPathToVerify, typeof(CustomRowBasedOpenTypesContext), UnitTestsUtil.JsonLightMimeType, "PUT", headers, true);
            //    }
            //    finally
            //    {
            //        CustomRowBasedOpenTypesContext.PreserveChanges = false;
            //    }
            //}

            [TestMethod, Variation("$metadata should return edmx metadata document")]
            public void ShouldReturnEdmxMetadataDocument()
            {
                var uriAndXPathsToVerify = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>("/$metadata",
                        new string[] { "/edmx:Edmx[@Version='4.0' and edmx:DataServices/csdl:Schema[@Namespace='AstoriaUnitTests.Stubs']]" }) };
                UnitTestsUtil.SendRequestAndVerifyXPath(null, "/$metadata", uriAndXPathsToVerify, typeof(CustomDataContext), UnitTestsUtil.MimeApplicationXml, "GET", null, false);
            }

            public class TestService1 : DataService<ocs.CustomObjectContext>
            {
                [ChangeInterceptor("Orders")]
                public void OnNewOrder(ocs.Hidden.Order order, UpdateOperations operation)
                {
                    if (operation == UpdateOperations.Add)
                    {
                        order.CustomersReference.EntityKey = new System.Data.EntityKey("CustomObjectContext.Customers", "ID", 0);
                    }
                }

                public static void InitializeService(DataServiceConfiguration configuration)
                {
                    configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
                    configuration.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                    configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                    configuration.UseVerboseErrors = OpenWebDataServiceHelper.ForceVerboseErrors;
                }
            }

            [TestMethod, Variation("setting relationship via entity key on reference property doesn't work")]
            public void SettingRelationShipOnReferencePropertyViaEntityKeyShouldWork()
            {
                string jsonPayload = "{ ID: 45, DollarAmount: 4444 }";

                var xpaths = new string[] { String.Format("/{0}[ID=45 and DollarAmount=4444 and Customers[ID=0]]", JsonValidator.ObjectString) };

                ocs.PopulateData.EntityConnection = null;
                using (EntityConnection connection = ocs.PopulateData.CreateTableAndPopulateData())
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.ServiceType = typeof(TestService1);
                        request.RequestUriString = "/Orders";
                        request.Accept = UnitTestsUtil.JsonMimeType;
                        request.HttpMethod = "POST";
                        request.RequestStream = new MemoryStream();
                        request.RequestContentType = UnitTestsUtil.JsonMimeType;
                        StreamWriter writer = new StreamWriter(request.RequestStream);
                        writer.Write(jsonPayload);
                        writer.Flush();
                        request.SendRequest();

                        // Verify that customer got binded to the new order
                        request.RequestUriString = "/Orders(45)?$expand=Customers";
                        request.RequestStream = null;
                        request.HttpMethod = "GET";
                        request.SendRequest();
                        UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), UnitTestsUtil.JsonLightMimeType, xpaths);
                    }
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("making sure we discard the previous changes in case of error in batching")]
            public void BatchErrorShouldDiscardPreviousChanges()
            {
                string batchRequestFilePath = Path.Combine(TestUtil.ServerUnitTestSamples, @"tests\BatchRequests\TestBatchError.txt");
                string batchRequest = File.ReadAllText(batchRequestFilePath);

                using (CustomDataContext.CreateChangeScope())
                {
                    BatchTestUtil.GetResponse(batchRequest, typeof(CustomDataContext), WebServerLocation.InProcess);

                    // The changes in the same changeset before the invalid operation must not have been persisted
                    UnitTestsUtil.VerifyInvalidUri("/Customers(123456)", typeof(CustomDataContext));

                    // The changes in the same changeset after the invalid operation must not have been persisted
                    var uriAndXPathsToVerify = new KeyValuePair<string, string[]>[] {
                        new KeyValuePair<string, string[]>("/Orders(1)", new string[] { String.Format("/{0}[ID=1]", JsonValidator.ObjectString) })};
                    UnitTestsUtil.SendRequestAndVerifyXPath(null, "/Orders(1)", uriAndXPathsToVerify, typeof(CustomDataContext), UnitTestsUtil.JsonLightMimeType, "GET", null, false);

                    // The changes from the second batch must have been made
                    uriAndXPathsToVerify = new KeyValuePair<string, string[]>[] {
                        new KeyValuePair<string, string[]>("/Orders(0)", new string[] { String.Format("/{0}[DollarAmount=0]", JsonValidator.ObjectString) })};
                    UnitTestsUtil.SendRequestAndVerifyXPath(null, "/Orders(0)", uriAndXPathsToVerify, typeof(CustomDataContext), UnitTestsUtil.JsonLightMimeType, "GET", null, false);
                }
            }

            //[TestMethod, Variation("Protocol: exception thrown in XML with filter=null")]
            //public void ShouldThrowInXmlWithFilterNull()
            //{
            //    ServiceModelData.Northwind.EnsureDependenciesAvailable();
            //    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
            //        new Dimension("ServiceModelData", ServiceModelData.ValidValues));
            //    TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable values)
            //    {
            //        ServiceModelData model = (ServiceModelData)values["ServiceModelData"];
            //        if (!model.ContainerNames.Contains("Customers"))
            //        {
            //            return;
            //        }

            //        string typeName = model.GetContainerRootTypeName("Customers");
            //        string customerName = model.GetModelProperties(typeName).Any(p => p.Name == "CompanyName") ? "CompanyName" : "Name";
            //        string[] uris = new string[]
            //        {
            //            "/Customers?$filter=null",
            //            "/Customers?$filter=cast(null, 'Edm.Boolean')",
            //            "/Customers?$filter=cast(null, 'Edm.Int32') add 10 lt length(" + customerName + ")",
            //        };
            //        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            //        {
            //            request.DataServiceType = model.ServiceModelType;
            //            request.ForceVerboseErrors = true;

            //            foreach (string uri in uris)
            //            {
            //                request.RequestUriString = uri;
            //                request.SendRequest();
            //            }
            //        }
            //    });
            //}

            [TestMethod, Variation("Entity sets can be something that derives from IQueryable")]
            public void EntitySetCanDeriveFromIQueryable()
            {
                Stream responseStream = UnitTestsUtil.GetResponseStream(WebServerLocation.InProcess, UnitTestsUtil.MimeApplicationXml, "/$metadata", typeof(TestContext2));
                UnitTestsUtil.VerifyXPaths(responseStream, UnitTestsUtil.MimeApplicationXml, new string[] { "//csdl:Schema[csdl:EntityContainer/csdl:EntitySet[@Name='Values'] and csdl:EntityType[@Name='RegressionUnitTestModule_RegressionTest_TestEntity2']]" });
            }

            #region "Entity sets can be something that derives from IQueryable" specific classes
            public class TestContext2
            {
                public TestEntitySet2 Values
                {
                    get;
                    set;
                }
            }

            public class TestEntity2
            {
                public int ID { get; set; }
                public string Name { get; set; }
            }

            public class TestEntitySet2 : IQueryable<TestEntity2>
            {
                #region IEnumerable<TestEntity2> Members

                public IEnumerator<TestEntity2> GetEnumerator()
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IEnumerable Members

                IEnumerator IEnumerable.GetEnumerator()
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IQueryable Members

                public Type ElementType
                {
                    get { throw new NotImplementedException(); }
                }

                public Expression Expression
                {
                    get { throw new NotImplementedException(); }
                }

                public IQueryProvider Provider
                {
                    get { throw new NotImplementedException(); }
                }

                #endregion
            }
            #endregion
            [Ignore] // Remove Atom
            // [TestMethod, Variation("negative case fires an assert -setting null value to top level resource")]
            public void InvalidRequestIfSettingTopLevelResourceToNullValue()
            {
                string jsonPayload = "null";
                string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<entry xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" adsm:null=\"true\" xmlns=\"http://www.w3.org/2005/Atom\" />";

                UnitTestsUtil.VerifyInvalidRequest(jsonPayload, "/Customers(1)", typeof(CustomDataContext), UnitTestsUtil.JsonLightMimeType, "PUT", 400);
                UnitTestsUtil.VerifyInvalidRequest(atomPayload, "/Customers(1)", typeof(CustomDataContext), UnitTestsUtil.AtomFormat, "PUT", 400);
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Throw BadGateway error on CUD operations on service uri")]
            public void ShouldThrowOnCudOperationsOnServiceUri()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("ResponseFormat", UnitTestsUtil.ResponseFormats),
                    new Dimension("WebServerLocation", new object[] { WebServerLocation.InProcess, WebServerLocation.InProcessWcf, WebServerLocation.Local }),
                    new Dimension("Method", new object[] { "PUT", "POST", "DELETE", "GET" })
                );

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    WebServerLocation location = (WebServerLocation)values["WebServerLocation"];
                    string responseFormat = (string)values["ResponseFormat"];
                    if (responseFormat == UnitTestsUtil.AtomFormat)
                    {
                        responseFormat = UnitTestsUtil.MimeApplicationXml;
                    }

                    string methodName = (string)values["Method"];

                    int expectedErrorCode = 405; // Method Not Allowed
                    if (methodName == "GET")
                    {
                        return;
                    }

                    Type contextType = typeof(CustomDataContext);
                    UnitTestsUtil.VerifyInvalidRequest(null, "/", contextType, responseFormat, methodName, expectedErrorCode);
                });
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Assert fired for properties of type IList while serializing")]
            public void TestIListPropertiesWhenSerializing()
            {
                TypedCustomDataContext<TestEntity3>.ValuesRequested += (x, y) =>
                {
                    ((TypedCustomDataContext<TestEntity3>)x).SetValues(new object[] {
                        new TestEntity3 { ID = 1, UntypedThings = new ArrayList { "a", "b" } } });
                };

                TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(
                    UnitTestsUtil.MimeApplicationXml,
                    "/$metadata",
                    typeof(TypedCustomDataContext<TestEntity3>),
                    null,
                    "GET");

                Trace.WriteLine(request.GetResponseStreamAsXmlDocument().InnerXml);

                request = UnitTestsUtil.GetTestWebRequestInstance(
                    UnitTestsUtil.AtomFormat,
                    "/Values",
                    typeof(TypedCustomDataContext<TestEntity3>),
                    null,
                    "GET");

                Trace.WriteLine(request.GetResponseStreamAsXmlDocument().InnerXml);
            }

            [IgnoreProperties("UntypedThings")]
            public class TestEntity3
            {
                public int ID { get; set; }
                public IList UntypedThings { get; set; }
            }

            // ODataLib was fixed and reports missing type name as an annotation.
            [Ignore] // Remove Atom
            // [TestMethod, Variation("type information must be required for any type that takes part in inheritance")]
            public void ShouldRequireTypeInformationInInheritance()
            {
                // This test also verifies that not able to load metadata for public nested derived types.
                TypedCustomDataContext<TestEntity4>.PreserveChanges = true;
                TypedCustomDataContext<TestEntity4>.ValuesRequested += (x, y) =>
                {
                    ((TypedCustomDataContext<TestEntity4>)x).SetValues(new object[] {
                        new TestEntity4 { ID = 1, X = 5 },
                        new TestDerivedEntity4 { ID = 2, X = 7, Y = 8 }});
                };

                Type contextType = typeof(TypedCustomDataContext<TestEntity4>);
                string payload = "{ value : 11 }";
                UnitTestsUtil.VerifyInvalidRequest(payload, "/Values(1)", contextType, UnitTestsUtil.JsonLightMimeType, "PUT", 400);
                UnitTestsUtil.VerifyInvalidRequest(payload, "/Values(2)", contextType, UnitTestsUtil.JsonLightMimeType, "PUT", 400);

                payload =
                    "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                        "<content type=\"application/xml\">" +
                            "<ads:X>125</ads:X>" +
                        "</content>" +
                    "</entry>";

                UnitTestsUtil.VerifyInvalidRequest(payload, "/Values(1)", contextType, UnitTestsUtil.AtomFormat, "PUT", 400);
                UnitTestsUtil.VerifyInvalidRequest(payload, "/Values(2)", contextType, UnitTestsUtil.AtomFormat, "PUT", 400);
                TypedCustomDataContext<TestEntity4>.PreserveChanges = false;
            }

            public class TestEntity4
            {
                public int ID { get; set; }
                public int X { get; set; }
            }

            public class TestDerivedEntity4 : TestEntity4
            {
                public int Y { get; set; }
            }

            [TestMethod, Variation("throw entity set properties and type properties for which we cannot load metadata")]
            public void ShouldThrowWhenCannotLoadMetadata()
            {
                UnitTestsUtil.VerifyInvalidRequest(null, "/$metadata", typeof(TypedCustomDataContext<TestEntity5>), UnitTestsUtil.MimeApplicationXml, "GET", 500);
                UnitTestsUtil.VerifyInvalidRequest(null, "/$metadata", typeof(TestDataContext5), UnitTestsUtil.MimeApplicationXml, "GET", 500);
            }

            public class TestEntity5
            {
                public int ID { get; set; }
                public uint DollarAmount { get; set; }
            }

            public class TestInvalidEntity5
            {
                public int ID1 { get; set; }
                public uint DollarAmount { get; set; }
            }

            public class TestDataContext5
            {
                public IQueryable<TestInvalidEntity5> InvalidProperty { get; set; }
            }

            [TestMethod, Variation("Ignoring key properties doesn't throw an exception")]
            public void ShouldThrowWhenIgnoringKeyProperties()
            {
                UnitTestsUtil.VerifyInvalidRequest(null, "/$metadata", typeof(TypedCustomDataContext<TestEntity6>), UnitTestsUtil.MimeApplicationXml, "GET", 500);
            }

            [IgnoreProperties("ID")]
            public class TestEntity6
            {
                public int ID { get; set; }
                public string Name { get; set; }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("allowing raw binary values in keys for datatypes other than byte[] and Binary")]
            public void AllowRawBinaryInKeysIfDataTypeNotBinary()
            {
                DateTime dateTime = DateTime.Now;
                Guid guid = new Guid();
                KeyValuePair<string, Type>[] variousKeyInfos = new KeyValuePair<string, Type>[] {
                    new KeyValuePair<string, Type>("1", typeof(int)),
                    new KeyValuePair<string, Type>("'abcde'", typeof(string)),
                    new KeyValuePair<string, Type>(String.Format("{0}", XmlConvert.ToString(TypeData.ConvertDateTimeToDateTimeOffset(dateTime))), typeof(DateTime)),
                    new KeyValuePair<string, Type>(String.Format("{0}", guid), typeof(Guid)),
                    new KeyValuePair<string, Type>("1500.345", typeof(decimal))
                };

                foreach (KeyValuePair<string, Type> keyInfo in variousKeyInfos)
                {
                    byte[] keyValues = System.Text.Encoding.UTF8.GetBytes(keyInfo.Key);
                    string uri = String.Format("/Values({0})", TypeData.FormatForKey(keyValues, true, false));

                    // Set up the data context
                    Type valueType = keyInfo.Value;
                    Type entityType = typeof(TypedEntity<,>).MakeGenericType(valueType, typeof(int));

                    CustomDataContextSetup dataContextSetup = new CustomDataContextSetup(entityType);
                    dataContextSetup.Id = TypeData.ValueFromXmlText(keyInfo.Key, keyInfo.Value, true /*keySyntax*/, UnitTestsUtil.AtomFormat);
                    dataContextSetup.MemberValue = 1;

                    Trace.WriteLine("Requesting " + uri);
                    TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.AtomFormat, uri, dataContextSetup.DataServiceType, null, "GET");
                    XmlDocument document = request.GetResponseStreamAsXmlDocument();

                    // In .NET 4.5, EscapeDataString will escape single quotes. However, our payload will still continue to leave them unescaped.
                    string key = Uri.EscapeDataString(keyInfo.Key).Replace("%27", "'");
                    UnitTestsUtil.VerifyXPaths(document, new string[] { String.Format("/atom:entry[atom:id=\"http://host/Values({0})\"]", key) });
                }
            }

            [TestMethod, Variation("Service operations returning void should return 204 (no content)")]
            public void ShouldReturn204ByVoidServiceOperation()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.RequestUriString = "/VoidServiceOperation";
                    request.ServiceType = typeof(VoidServiceOperationService);
                    request.SendRequest();
                    Assert.IsTrue(request.ResponseStatusCode == 204, "Void service operation should return 204 - no content");
                }
            }

            public class VoidServiceOperationService : OpenWebDataService<CustomDataContext>
            {
                [WebGet]
                public void VoidServiceOperation()
                {
                    // does nothing
                }
            }

            [TestMethod, Variation("Invalid content type tests")]
            public void InvalidContentTypeTests()
            {
                var uriTargetingEntities = new string[] {
                    String.Format("{0},{1}", "/Customers", "POST"),
                    String.Format("{0},{1}", "/Orders(1)", "PUT"),
                    String.Format("{0},{1}", "/Customers(1)/BestFriend", "PUT"),
                    String.Format("{0},{1}", "/Customers(1)/Orders", "POST") };

                var invalidMimeForEntityUri = new string[] {
                    UnitTestsUtil.MimeTextXml,
                    UnitTestsUtil.MimeApplicationXml,
                    UnitTestsUtil.MimeTextPlain,
                    UnitTestsUtil.MimeMultipartMixed,
                    UnitTestsUtil.MimeApplicationOctetStream };

                var uriTargetingNonEntities = new string[] {
                    String.Format("{0},{1}", "/Customers(1)/Address", "PUT"),
                    String.Format("{0},{1}", "/Orders(1)/DollarAmount", "PUT"),
                    String.Format("{0},{1}", "/Customers(1)/Address/City", "PUT"),
                    String.Format("{0},{1}", "/Customers(1)/Name/$value", "PUT") };

                var invalidMimeForNonEntityResources = new string[] {
                    UnitTestsUtil.MimeAny,
                    UnitTestsUtil.AtomFormat,
                    UnitTestsUtil.MimeAny,
                    UnitTestsUtil.MimeMultipartMixed,
                    UnitTestsUtil.MimeApplicationOctetStream };

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("ContextType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext) }),
                    new Dimension("UriAndMimeType", new object[] {
                        new KeyValuePair<string[], string[]>(uriTargetingEntities, invalidMimeForEntityUri),
                        new KeyValuePair<string[], string[]>(uriTargetingNonEntities, invalidMimeForNonEntityResources)
                    }));

                ocs.PopulateData.EntityConnection = null;
                using (EntityConnection connection = ocs.PopulateData.CreateTableAndPopulateData())
                using (CustomDataContext.CreateChangeScope())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                    {
                        Type contextType = (Type)values["ContextType"];
                        var uriAndMimeType = (KeyValuePair<string[], string[]>)values["UriAndMimeType"];
                        TestUtil.RunCombinations(
                            uriAndMimeType.Key,
                            uriAndMimeType.Value,
                            (uriAndMethodInfo, mimeType) =>
                            {
                                string[] info = uriAndMethodInfo.Split(',');
                                UnitTestsUtil.VerifyInvalidRequest(null, info[0], contextType, mimeType, info[1], 415);
                            });
                    });
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Make sure the uri in the deferred elements are valid")]
            public void UriInDeferredElementsShouldBeValid()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("ResponseFormat", UnitTestsUtil.ResponseFormats),
                    new Dimension("WebServerLocation", new object[] { WebServerLocation.InProcess }),
                    new Dimension("ContextType", new Type[]
                    {
                        typeof(CustomDataContext),
                        typeof(ocs.CustomObjectContext),
                        typeof(CustomRowBasedOpenTypesContext)
                    }),
                    new Dimension("Uri", new string[] { "/Customers(1)", "/Customers(1)?$expand=BestFriend($expand=Orders)" })
                );

                ocs.PopulateData.EntityConnection = null;
                using (EntityConnection connection = ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                    {
                        string responseFormat = (string)values["ResponseFormat"];
                        Type contextType = (Type)values["ContextType"];
                        string testUri = (string)values["Uri"];

                        TestWebRequest request = null;
                        Exception e = TestUtil.RunCatching(() =>
                        {
                            request = UnitTestsUtil.GetTestWebRequestInstance(
                                responseFormat,
                                testUri,
                                contextType,
                                null,
                                "GET");
                        });

                        XmlDocument document = request.GetResponseStreamAsXmlDocument(responseFormat);

                        XmlNodeList nodeList = document.SelectNodes("//atom:link[@href]", TestUtil.TestNamespaceManager);
                        string[] uriInPayload = new string[nodeList.Count];
                        for (int i = 0; i < nodeList.Count; i++)
                        {
                            uriInPayload[i] = UnitTestsUtil.GetBaseUri(document.DocumentElement) + nodeList[i].Attributes["href"].Value;
                        }

                        for (int i = 0; i < uriInPayload.Length; i++)
                        {
                            string uri = uriInPayload[i];
                            if (uri.StartsWith("http://host"))
                            {
                                uri = uri.Substring("http://host".Length);
                            }

                            // Just make sure everything works. No need to verify any xpaths.
                            request = UnitTestsUtil.GetTestWebRequestInstance(
                                responseFormat,
                                testUri,
                                contextType,
                                null,
                                "GET");
                            request.GetResponseStream();
                        }
                    });
                }
            }

            [TestMethod, Variation("Updating a key value property directly or setting it to null should throw bad request")]
            public void ShouldThrowWhenUpdatingKeyPropertyOrSettingToNull()
            {
                UnitTestsUtil.VerifyInvalidRequest("{ value: 2 }", "/Orders(2)/ID", typeof(CustomDataContext), UnitTestsUtil.JsonLightMimeType, "PATCH", 400);
                UnitTestsUtil.VerifyInvalidRequest(null, "/Orders(2)/ID/$value", typeof(CustomDataContext), null, "PATCH", 400);
                UnitTestsUtil.VerifyInvalidRequest(null, "/Orders(2)/ID/$value", typeof(CustomDataContext), null, "DELETE", 400);
            }

            // EdmItemCollection cannot read Duration
            [Ignore]
            // [TestMethod, Variation("Updating $value uri should work")]
            public void UpdatingValueUriShouldWork()
            {
                TypedCustomAllTypesDataContext.ClearHandlers();
                TypedCustomAllTypesDataContext.ClearValues();
                TypedCustomAllTypesDataContext.PreserveChanges = true;

                // Get the metadata for the type
                var model = UnitTestsUtil.LoadMetadataFromDataServiceType(typeof(TypedCustomAllTypesDataContext), null);
                var entityType = model.FindType(typeof(AllTypes).FullName) as IEdmEntityType;

                // create entity with all default values and insert it
                AllTypes emptyAllTypes = new AllTypes();
                emptyAllTypes.ID = 1;
                string payload = UnitTestsUtil.GetPayload(emptyAllTypes, entityType, UnitTestsUtil.AtomFormat);
                UnitTestsUtil.SendRequestAndVerifyXPath(payload, "/Values", new KeyValuePair<string, string[]>[0], typeof(TypedCustomAllTypesDataContext), UnitTestsUtil.AtomFormat, "POST", null, false);

                Dictionary<Type, TypeData> typeToDataMap = new Dictionary<Type, TypeData>();
                foreach (TypeData typeData in TypeData.Values)
                {
                    typeToDataMap.Add(typeData.ClrType, typeData);
                }

                // build a dictionary for the type to properties
                foreach (PropertyInfo property in typeof(AllTypes).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (property.Name == "ID")
                    {
                        continue;
                    }

                    Type propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    TypeData typeData = typeToDataMap[propertyType];
                    string contentType = typeData.DefaultContentType;
                    string uri = "/Values(1)/" + property.Name + "/$value";
                    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                        new Dimension("SampleValue", typeData.SampleValues));
                    TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable values)
                    {
                        object value = values["SampleValue"];
                        if (value == null)
                        {
                            return;
                        }

                        payload = TypeData.XmlValueFromObject(value);
                        Trace.WriteLine("Sending payload: " + payload);
                        string mimeType = (propertyType != typeof(byte[]) && propertyType != typeof(System.Data.Linq.Binary)) ? UnitTestsUtil.MimeTextPlain + "; charset=UTF-8" : UnitTestsUtil.MimeApplicationOctetStream;
                        UnitTestsUtil.GetTestWebRequestInstance(mimeType, uri, typeof(TypedCustomAllTypesDataContext), null, "PUT", payload);
                        TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(mimeType, uri, typeof(TypedCustomAllTypesDataContext), null, "GET");
                        string response = request.GetResponseStreamAsText();
                        Assert.IsTrue(response == payload, "they should match");
                    });
                }
            }

            [TestMethod, Variation("Error Contracts: UseVerBoseErrors Broken , Error Response Returns Full Stack Trace")]
            public void UseVerboseErrorsShouldWork()
            {
                TestUtil.ClearMetadataCache();
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    ServiceModelData.Northwind.EnsureDependenciesAvailable();
                    request.ForceVerboseErrors = false;
                    request.DataServiceType = typeof(NorthwindModel.NorthwindContext);
                    request.RequestUriString = "/Customers";
                    request.HttpMethod = "POST";
                    request.RequestContentType = SerializationFormatData.JsonLight.MimeTypes[0];
                    request.SetRequestStreamAsText("{ \"CustomerID\" : \"ALFKI\", \"CompanyName\" : \"foo\" }");
                    Exception exception = TestUtil.RunCatching(request.SendRequest);
                    string response = request.GetResponseStreamAsText();
                    TestUtil.AssertContainsFalse(response, "System.");
                }
            }

            [TestMethod, Variation("Types with empty namespace returns csdl with schema namespace as empty")]
            public void SchemaNamespaceShouldBeEmptyForTypesWithEmptyNamespace()
            {
                TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(
                    UnitTestsUtil.MimeApplicationXml,
                    "/$metadata",
                    typeof(TypedCustomDataContext<NoNamespaceType>),
                    null,
                    "GET");

                XmlDocument document = request.GetResponseStreamAsXmlDocument();
                UnitTestsUtil.VerifyXPaths(document,
                    new string[] {
                        "/edmx:Edmx/edmx:DataServices/csdl:Schema[@Namespace='AstoriaUnitTests']",
                        "count(/edmx:Edmx/edmx:DataServices/csdl:Schema)=1",
                        "//csdl:EntityType[@Name='NoNamespaceType']",
                        "//csdl:EntityType[@Name='TestDerivedType']"
                    });
            }

            [TestMethod, Variation("Returns the list of entity set names for the given service")]
            public void VerifyReturnListOfEntitySetNames()
            {
                string[] jsonXPaths = new string[] {
                    "count(/Object/value/Array/Object/name)=6",
                    "/Object/value/Array/Object/name[text()='Customers']",
                    "/Object/value/Array/Object/name[text()='Orders']",
                    "/Object/value/Array/Object/name[text()='Regions']",
                    "/Object/value/Array/Object/name[text()='Products']",
                    "/Object/value/Array/Object/name[text()='OrderDetails']"
                };

                UnitTestsUtil.VerifyPayload("", typeof(CustomDataContext), UnitTestsUtil.JsonLightMimeType, null, jsonXPaths);
            }

            [TestMethod, Variation("Make sure that we check for invalid etags and that checks works when we have multiple etag properties")]
            public void CheckForInvalidEtagAndMultipleEtagPropertiesShouldWork()
            {
                TypedCustomDataContext<TestEntity7>.ValuesRequested += (x, y) =>
                {
                    ((TypedCustomDataContext<TestEntity7>)x).SetValues(new object[] {
                        new TestEntity7 { ID = 1, ETagProperty1 = 5, ETagProperty2 = "Foo" } });
                };

                Type contextType = typeof(TypedCustomDataContext<TestEntity7>);
                string etagValue = UnitTestsUtil.GetETagFromResponse(contextType, "/Values(1)", UnitTestsUtil.JsonLightMimeType);
                var headers = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("If-None-Match", etagValue) };
                TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.JsonLightMimeType, "/Values(1)", contextType, headers, "GET");
                Assert.AreEqual<int>(request.ResponseStatusCode, (int)HttpStatusCode.NotModified, "this should work");
            }

            [ETag("ETagProperty1", "ETagProperty2")]
            public class TestEntity7
            {
                public int ID { get; set; }
                public int ETagProperty1 { get; set; }
                public string ETagProperty2 { get; set; }
            }

            [TestMethod, Variation("Making sure etag checks on server works when one of the property values is null")]
            public void EtagShouldWorkInServerWhenOneOfPropertyValuesIsNull()
            {
                Type contextType = typeof(ocs.CustomObjectContext);
                ocs.PopulateData.EntityConnection = null;
                using (EntityConnection connection = ocs.PopulateData.CreateTableAndPopulateData())
                {
                    string etag = UnitTestsUtil.GetETagFromResponse(contextType, "/Customers(1)", UnitTestsUtil.JsonLightMimeType);
                    var headers = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("If-Match", etag) };
                    UnitTestsUtil.SendRequestAndVerifyXPath(null, "/Customers(1)/Concurrency/$value", null, contextType, UnitTestsUtil.JsonLightMimeType, "DELETE", headers, true);
                    etag = UnitTestsUtil.GetETagFromResponse(contextType, "/Customers(1)", UnitTestsUtil.JsonLightMimeType);
                    headers = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("If-Match", etag) };

                    string payload = "{ @odata.type: '" + UnitTestsUtil.CustomerWithBirthdayTypeName + "' , Concurrency: '123' }";
                    UnitTestsUtil.SendRequestAndVerifyXPath(payload, "/Customers(1)", null, contextType, UnitTestsUtil.JsonLightMimeType, "PATCH", headers, true);
                }
            }

            // [TestMethod, Variation("If the double number has no period in it, json reader fails to read this")]
            // Todo: Fix places where we've lost JsonVerbose coverage to add JsonLight
            [Ignore]
            public void JsonShouldReadDoubleNumberWithoutPeriod()
            {
                TypedCustomDataContext<BugBugEntity>.ValuesRequested += (x, y) =>
                {
                    ((TypedCustomDataContext<BugBugEntity>)x).SetValues(new object[] {
                        new BugBugEntity { ID = 7E-06 } });
                };

                Type contextType = typeof(TypedCustomDataContext<BugBugEntity>);
                TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.JsonLightMimeType, "/Values(7E-06)", contextType, null, "GET");
                string payload = request.GetResponseStreamAsText();
                // json light fails here without a model... need to fix the test infrastructure to provide that model.
                UnitTestsUtil.SendRequestAndVerifyXPath(payload, "/Values(7E-06)", null, contextType, UnitTestsUtil.JsonLightMimeType, "PATCH", null, false);
            }

            // [TestMethod, Variation("If the double number range is greater than int32 range, but fits within the double scale, json reader fails to read this")]
            [Ignore]
            // Todo:Fix places where we've lost JsonVerbose coverage to add JsonLight
            public void JsonShouldReadDoubleNumberGreaterThanInt32Range()
            {
                TypedCustomDataContext<BugBugEntity>.ValuesRequested += (x, y) =>
                {
                    ((TypedCustomDataContext<BugBugEntity>)x).SetValues(new object[] {
                        new BugBugEntity { ID = 9000000000 } });
                };

                Type contextType = typeof(TypedCustomDataContext<BugBugEntity>);
                TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.JsonLightMimeType, "/Values(9000000000)", contextType, null, "GET");
                string payload = request.GetResponseStreamAsText();
                // json light fails here without a model... need to fix the test infrastructure to provide that model.
                UnitTestsUtil.SendRequestAndVerifyXPath(payload, "/Values(9000000000)", null, contextType, UnitTestsUtil.JsonLightMimeType, "PATCH", null, false);
            }

            public class BugBugEntity
            {
                public Double ID { get; set; }
                public string Foo { get; set; }
            }

            [TestMethod, Variation("Testing Processing Request")]
            public void TestProcessingRequest()
            {
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
                {
                    foreach (string uri in new string[] {
                    "/Customers",
                    "/Customers?$top=5",
                    "/Customers(1)?foo=bar and xyz=123" })
                    {
                        OpenWebDataServiceHelper.ProcessRequestDelegate.Value = delegate (ProcessRequestArgs args)
                        {
                            Assert.IsFalse(args.IsBatchOperation, "Must be false for single operation requests");
                            Assert.IsTrue(args.RequestUri.OriginalString.EndsWith(uri), "the uri must match");
                        };

                        UnitTestsUtil.GetResponseStream(WebServerLocation.InProcess, UnitTestsUtil.JsonLightMimeType, uri, typeof(CustomDataContext));
                    }
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Make sure the max object count is honoured")]
            public void VerifyMaxObjectCount()
            {
                string customerFullName = typeof(Customer).FullName;
                string orderFullName = typeof(Order).FullName;

                #region Jsonpayload and xpath
                string jsonPayload = "{" +
                                    "@odata.type : \"AstoriaUnitTests.Stubs.Customer\" ," +
                                    "Name: \"Foo\"," +
                                    "ID: 125," +
                                    "Address: { StreetAddress: 'StreetAddress', State: 'WA' }," +
                                    "BestFriend: { " +
                                        "@odata.type : \"AstoriaUnitTests.Stubs.Customer\", " +
                                        "Name: \"FooBestFriend\"," +
                                        "ID: 126," +
                                        "Orders : [ " +
                                            "{" +
                                                "@odata.type: \"AstoriaUnitTests.Stubs.Order\", " +
                                                "ID: 151," +
                                                "DollarAmount: 1500.00" +
                                            "}," +
                                            "{" +
                                                "@odata.type: \"AstoriaUnitTests.Stubs.Order\"," +
                                                "ID: 152," +
                                                "DollarAmount: 500.00" +
                                            "}," +
                                            "{" +
                                                "@odata.type: \"AstoriaUnitTests.Stubs.Order\" ," +
                                                "\"ID\": 153," +
                                                "\"DollarAmount\": 0.00" +
                                            "}," +
                                            "{" +
                                                "@odata.type: \"AstoriaUnitTests.Stubs.Order\"" +
                                            "}" +
                                        "]" +
                                    "}" +
                                 "}";

                #endregion

                #region atompayload and xpath
                string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<entry xml:base=\"/\" xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://www.w3.org/2005/Atom\">" +
                        AtomUpdatePayloadBuilder.GetCategoryXml(customerFullName) +
                        "<content type=\"application/xml\"><adsm:properties>" +
                            "<ads:Name>Foo</ads:Name>" +
                            "<ads:ID>125</ads:ID>" +
                            "<ads:Address>" +
                                "<ads:StreetAddress>StreetAddress</ads:StreetAddress>" +
                                "<ads:State>WA</ads:State>" +
                            "</ads:Address>" +
                        "</adsm:properties></content>" +
                        "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' title='BestFriend'>" +
                             "<adsm:inline>" +
                                 "<entry>" +
                                   AtomUpdatePayloadBuilder.GetCategoryXml(customerFullName) +
                                    "<content type=\"application/xml\"><adsm:properties>" +
                                        "<ads:Name>FooBestFriend</ads:Name>" +
                                        "<ads:ID>126</ads:ID>" +
                                    "</adsm:properties></content>" +
                                    "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' title='Orders'>" +
                                        "<adsm:inline>" +
                                            "<feed>" +
                                                "<entry>" +
                                                    "<content type=\"application/xml\"><adsm:properties>" +
                                                        "<ads:ID>151</ads:ID>" +
                                                        "<ads:DollarAmount adsm:type='Edm.Double'>1500.00</ads:DollarAmount>" +
                                                    "</adsm:properties></content>" +
                                                "</entry>" +
                                                "<entry>" +
                                                    "<content type=\"application/xml\"><adsm:properties>" +
                                                        "<ads:ID>152</ads:ID>" +
                                                        "<ads:DollarAmount adsm:type='Edm.Double'>500.00</ads:DollarAmount>" +
                                                    "</adsm:properties></content>" +
                                                "</entry>" +
                                                "<entry>" +
                                                    "<content type=\"application/xml\"><adsm:properties>" +
                                                        "<ads:ID>153</ads:ID>" +
                                                        "<ads:DollarAmount adsm:type='Edm.Double'>0.00</ads:DollarAmount>" +
                                                    "</adsm:properties></content>" +
                                                "</entry>" +
                                            "</feed>" +
                                        "</adsm:inline>" +
                                    "</link>" +
                                    "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' type='application/atom+xml;type=feed' href='Orders(101)' />" +
                                "</entry>" +
                            "</adsm:inline>" +
                        "</link>" +
                    "</entry>";

                #endregion

                using (OpenWebDataServiceHelper.MaxObjectCountOnInsert.Restore())
                using (TestUtil.MetadataCacheCleaner())
                {
                    OpenWebDataServiceHelper.MaxObjectCountOnInsert.Value = 6;
                    TestUtil.ClearMetadataCache();
                    UnitTestsUtil.VerifyInvalidRequest(jsonPayload, "/Customers", typeof(CustomDataContext), UnitTestsUtil.JsonLightMimeType, "POST", (int)HttpStatusCode.RequestEntityTooLarge);
                    UnitTestsUtil.VerifyInvalidRequest(atomPayload, "/Customers", typeof(CustomDataContext), UnitTestsUtil.AtomFormat, "POST", (int)HttpStatusCode.RequestEntityTooLarge);
                }
            }

            [TestMethod, Variation("Make sure DataServiceException is surfaced when thrown from QueryInterceptors")]
            public void DataServiceExceptionShouldBeSurfacedFromQueryInterceptorException()
            {
                string[] requestUriStrings = new string[]
                {
                    "/Orders", /*code path: CreateFirstSegment() -> ComposeResourceContainer()*/
                    "/Customers(1)/Orders", /*code path: CreateSegments() -> ComposeResourceContainer()*/
                    "/Customers?$filter=Orders/any()", /*code path: ParseMemberAccess() -> ComposeQueryInterceptors()*/
                    "/Customers(1)?$expand=Orders" /*code path: CheckExpandPaths() -> ComposeQueryInterceptors()*/
                };

                Type[] serviceTypes = new Type[]
                {
                    typeof(QueryInterceptorDataServiceException),
                    typeof(QueryInterceptorApplicationException)
                };

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("RequestUriString", requestUriStrings),
                    new Dimension("ServiceType", serviceTypes));

                TestUtil.RunCombinatorialEngineFail(engine, (table) =>
                {
                    using (OpenWebDataServiceHelper.MaxProtocolVersion.Restore())
                    using (OpenWebDataServiceHelper.AcceptAnyAllRequests.Restore())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        OpenWebDataServiceHelper.MaxProtocolVersion.Value = ODataProtocolVersion.V4;
                        OpenWebDataServiceHelper.AcceptAnyAllRequests.Value = true;

                        request.RequestUriString = (string)table["RequestUriString"];
                        request.ServiceType = (Type)table["ServiceType"];
                        request.HttpMethod = "GET";

                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNotNull(exception);

                        if (request.ServiceType == typeof(QueryInterceptorDataServiceException))
                        {
                            DataServiceException dse = exception.InnerException as DataServiceException;
                            Assert.IsNotNull(dse);
                            Assert.AreEqual(403, dse.StatusCode);
                            TargetInvocationException tie = dse.InnerException as TargetInvocationException;
                            Assert.IsNotNull(tie);
                        }
                        else
                        {
                            TargetInvocationException tie = exception.InnerException as TargetInvocationException;
                            Assert.IsNotNull(tie);
                            ApplicationException ae = tie.InnerException as ApplicationException;
                            Assert.IsNotNull(ae);
                        }
                    }
                });
            }

            public class QueryInterceptorDataServiceException : OpenWebDataService<CustomDataContext>
            {
                [QueryInterceptor("Orders")]
                public Expression<Func<Order, bool>> FilterOrders()
                {
                    throw (new DataServiceException(403, "My DataServiceException"));
                    //return o => o.ID < 100;
                }
            }

            public class QueryInterceptorApplicationException : OpenWebDataService<CustomDataContext>
            {
                [QueryInterceptor("Orders")]
                public Expression<Func<Order, bool>> FilterOrders()
                {
                    throw (new ApplicationException("My ApplicationException"));
                    //return o => o.ID < 100;
                }
            }

            [TestMethod, Variation("Make sure service operation rights are checked correctly if it's the only segment")]
            public void VerifyServiceOperationRightsIfIsOnlySegment()
            {
                ServiceOperationRights[] serviceOperationRights = new ServiceOperationRights[4] {
                    ServiceOperationRights.OverrideEntitySetRights | ServiceOperationRights.None,
                    ServiceOperationRights.OverrideEntitySetRights | ServiceOperationRights.ReadSingle,
                    ServiceOperationRights.OverrideEntitySetRights | ServiceOperationRights.ReadMultiple,
                    ServiceOperationRights.OverrideEntitySetRights | ServiceOperationRights.All,
                };
                //for(int i = 0; i < (int)(ServiceOperationRights.All | ServiceOperationRights.OverrideEntitySetRights); ++i) {
                //    serviceOperationRights[i] = (ServiceOperationRights)i;
                //}

                string[] serviceOperationNames = new string[]
                {
                    "SOpVoid",
                    "SOpDirectValue",
                    "SOpEnumerable",
                    "SOpSingleQueryable",
                    "SOpMultipleQueryable"
                };

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("ServiceOperationRights", serviceOperationRights),
                    new Dimension("ServiceOperationName", serviceOperationNames));

                TestUtil.RunCombinatorialEngineFail(engine, (table) =>
                {
                    ServiceOperationRights rights = (ServiceOperationRights)table["ServiceOperationRights"];
                    string methodName = (string)table["ServiceOperationName"];

                    using (MyServiceOperationDS.SetServiceOperationAccessRule(methodName, rights))
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.ServiceType = typeof(MyServiceOperationDS);
                        request.RequestUriString = "/" + methodName;
                        request.HttpMethod = "GET";

                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception,
                            //rights == ServiceOperationRights.None,
                            //rights == ServiceOperationRights.ReadSingle && (methodName == "SOpEnumerable" || methodName == "SOpMultipleQueryable"),
                            //rights == ServiceOperationRights.ReadMultiple && (methodName == "SOpDirectValue" || methodName == "SOpSingleQueryable"),
                            rights == (ServiceOperationRights.OverrideEntitySetRights | ServiceOperationRights.None),
                            rights == (ServiceOperationRights.OverrideEntitySetRights | ServiceOperationRights.ReadSingle) && (methodName == "SOpEnumerable" || methodName == "SOpMultipleQueryable"),
                            rights == (ServiceOperationRights.OverrideEntitySetRights | ServiceOperationRights.ReadMultiple) && (methodName == "SOpDirectValue" || methodName == "SOpSingleQueryable")
                        );

                        if (rights == (ServiceOperationRights.OverrideEntitySetRights | ServiceOperationRights.None))
                        {
                            Assert.AreEqual(404, request.ResponseStatusCode);
                        }

                        if (rights == (ServiceOperationRights.OverrideEntitySetRights | ServiceOperationRights.ReadSingle) && (methodName == "SOpEnumerable" || methodName == "SOpMultipleQueryable"))
                        {
                            Assert.AreEqual(403, request.ResponseStatusCode);
                        }

                        if (rights == (ServiceOperationRights.OverrideEntitySetRights | ServiceOperationRights.ReadMultiple) && (methodName == "SOpDirectValue" || methodName == "SOpSingleQueryable"))
                        {
                            Assert.AreEqual(403, request.ResponseStatusCode);
                        }
                    }

                    TestUtil.ClearConfiguration();
                });
            }

            [TestMethod, Variation("Null reference exception in CheckVersion() when POSTing to a void service op")]
            public void ShouldNotThrowInCheckVersionWhenPostVoidServiceOperation()
            {
                string[] serviceOperationNames = new string[]
                {
                    "SOpVoidPost",
                    "SOpDirectValuePost",
                    "SOpEnumerablePost",
                    "SOpSingleQueryablePost",
                    "SOpMultipleQueryablePost"
                };

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("ServiceOperationName", serviceOperationNames));

                TestUtil.RunCombinatorialEngineFail(engine, (table) =>
                {
                    ServiceOperationRights rights = ServiceOperationRights.All;
                    string methodName = (string)table["ServiceOperationName"];

                    using (MyServiceOperationDS.SetServiceOperationAccessRule(methodName, rights))
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.ServiceType = typeof(MyServiceOperationDS);
                        request.RequestUriString = "/" + methodName;
                        request.HttpMethod = "POST";
                        request.SetRequestStreamAsText(string.Empty);
                        request.RequestVersion = "4.0;";
                        request.SendRequest();
                    }

                    TestUtil.ClearConfiguration();
                });
            }

            public class MyServiceOperationDS : DataService<CustomDataContext>
            {
                // This method is called only once to initialize service-wide policies.
                public static void InitializeService(DataServiceConfiguration config)
                {
                    config.SetEntitySetAccessRule("*", EntitySetRights.All);
                    foreach (string sop in serviceOperationRightsDictionary.Keys)
                    {
                        config.SetServiceOperationAccessRule(sop, serviceOperationRightsDictionary[sop]);
                    }
                    config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                }

                public static IDisposable SetServiceOperationAccessRule(string name, ServiceOperationRights rights)
                {
                    serviceOperationRightsDictionary[name] = rights;
                    return new MyServiceOperationClearRights();
                }

                [WebGet]
                public void SOpVoid()
                {
                    return;
                }

                [WebGet]
                public string SOpDirectValue()
                {
                    return "Value";
                }

                [WebGet]
                public IEnumerable<string> SOpEnumerable()
                {
                    return new string[] { "Value" };
                }

                [WebGet]
                [SingleResult]
                public IQueryable<string> SOpSingleQueryable()
                {
                    return (new string[] { "Value" }).AsQueryable();
                }

                [WebGet]
                public IQueryable<string> SOpMultipleQueryable()
                {
                    return (new string[] { "Value1", "Value2" }).AsQueryable();
                }

                [WebInvoke]
                public void SOpVoidPost()
                {
                    return;
                }

                [WebInvoke]
                public string SOpDirectValuePost()
                {
                    return "Value";
                }

                [WebInvoke]
                public IEnumerable<string> SOpEnumerablePost()
                {
                    return new string[] { "Value" };
                }

                [WebInvoke]
                [SingleResult]
                public IQueryable<string> SOpSingleQueryablePost()
                {
                    return (new string[] { "Value" }).AsQueryable();
                }

                [WebInvoke]
                public IQueryable<string> SOpMultipleQueryablePost()
                {
                    return (new string[] { "Value1", "Value2" }).AsQueryable();
                }

                private static Dictionary<string, ServiceOperationRights> serviceOperationRightsDictionary = new Dictionary<string, ServiceOperationRights>();

                class MyServiceOperationClearRights : IDisposable
                {
                    void IDisposable.Dispose()
                    {
                        serviceOperationRightsDictionary.Clear();
                    }
                }
            }

            [TestMethod, Variation("mismatch quotes causes server to stop responding")]
            public void SecurityBug()
            {
                foreach (string jsonPayload in new string[] { "'", "\"", "[", "{" })
                {
                    UnitTestsUtil.VerifyInvalidRequestForVariousProviders(jsonPayload, "/Customers", UnitTestsUtil.JsonLightMimeType, "POST", 400);
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("filter on custom providers on strongly typed properties not working")]
            public void FilterOnCustomProvidersOnStrongTypePropertiesShouldWork()
            {
                UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.AtomFormat, "/Customers?$filter=ID%20eq%201", typeof(CustomRowBasedContext), null, "GET");
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("check whether the conversion is not happening when the typeConversion is set to false")]
            public void ConversionShouldNotHappenWhenTypeConversionSetToFale()
            {
                foreach (TypeData typeData in TypeData.Values)
                {
                    // Ignore types which are not supported
                    if (!typeData.IsTypeSupported)
                    {
                        continue;
                    }

                    Type clrType = Nullable.GetUnderlyingType(typeData.ClrType) ?? typeData.ClrType;
                    foreach (object sampleValue in typeData.SampleValues)
                    {
                        // For atom case, there are 2 scenarios to test when EnableTypeConversion is set to false.
                        // When the type name is written on the wire, we will do the conversion to the type mentioned on the wire
                        // Otherwise, we will just pass the type as string. We need to test both the scenarios.

                        foreach (bool withPayloadType in new bool[] { true, false })
                        {
                            string edmTypeName = withPayloadType ? typeData.GetEdmTypeName() : null;
                            string propertyValue = sampleValue == null ? null : TypeData.XmlValueFromObject(sampleValue);
                            TestDataContext2.ExpectedType = withPayloadType ? clrType : typeof(string);
                            if (withPayloadType && clrType == typeof(DateTime))
                            {
                                TestDataContext2.ExpectedType = typeof(DateTimeOffset);
                            }
                            string atomNamespace = TestUtil.TestNamespaceManager.LookupNamespace("atom");
                            string dataWebNamespace = TestUtil.TestNamespaceManager.LookupNamespace("ads");
                            string dataWebMetadataNamespace = TestUtil.TestNamespaceManager.LookupNamespace("adsm");

                            // Try it for entity POST
                            StringWriter writer = new StringWriter();
                            using (XmlWriter xmlWriter = UnitTestsUtil.GetXmlWriter(writer))
                            {
                                UnitTestsUtil.WriteEntryElement(xmlWriter);
                                xmlWriter.WriteStartElement("content", atomNamespace);
                                xmlWriter.WriteAttributeString("type", "application/xml");
                                xmlWriter.WriteStartElement("properties", dataWebMetadataNamespace);
                                UnitTestsUtil.WritePropertyValue(xmlWriter, "ID", propertyValue, edmTypeName);
                                UnitTestsUtil.WritePropertyValue(xmlWriter, "Name", propertyValue, edmTypeName);
                                UnitTestsUtil.WritePropertyValue(xmlWriter, "Description", propertyValue, edmTypeName);
                                xmlWriter.WriteStartElement("Address", dataWebNamespace);
                                xmlWriter.WriteAttributeString("type", dataWebMetadataNamespace, typeof(Address).FullName);
                                UnitTestsUtil.WritePropertyValue(xmlWriter, "StreetAddress", propertyValue, edmTypeName);
                                xmlWriter.WriteEndElement(); // end of address
                                xmlWriter.WriteEndElement(); // end of properties
                                xmlWriter.WriteEndElement(); // end of content
                                xmlWriter.WriteEndElement(); // end of entry
                            }

                            string atomPayload = writer.ToString();
                            UnitTestsUtil.SendRequestAndVerifyXPath(atomPayload, "/Values", new KeyValuePair<string, string[]>[0], typeof(TestDataContext2), UnitTestsUtil.AtomFormat, "POST", null, false);

                            // Try it by updating the property value directly - Name
                            writer = new StringWriter();
                            using (XmlWriter xmlWriter = UnitTestsUtil.GetXmlWriter(writer))
                            {
                                xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='utf-8'");
                                UnitTestsUtil.WritePropertyValue(xmlWriter, "Name", propertyValue, edmTypeName);
                            }

                            atomPayload = writer.ToString();
                            UnitTestsUtil.SendRequestAndVerifyXPath(atomPayload, "/Values(1)/Name", new KeyValuePair<string, string[]>[0], typeof(TestDataContext2), UnitTestsUtil.MimeTextXml, "PUT", null, false);

                            // Try it by updating the property value directly - Description
                            writer = new StringWriter();
                            using (XmlWriter xmlWriter = UnitTestsUtil.GetXmlWriter(writer))
                            {
                                xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='utf-8'");
                                UnitTestsUtil.WritePropertyValue(xmlWriter, "Description", propertyValue, edmTypeName);
                            }

                            atomPayload = writer.ToString();
                            UnitTestsUtil.SendRequestAndVerifyXPath(atomPayload, "/Values(1)/Description", new KeyValuePair<string, string[]>[0], typeof(TestDataContext2), UnitTestsUtil.MimeTextXml, "PUT", null, false);

                            // Try it by updating the property value directly - Address/StreetAddress
                            writer = new StringWriter();
                            using (XmlWriter xmlWriter = UnitTestsUtil.GetXmlWriter(writer))
                            {
                                xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='utf-8'");
                                UnitTestsUtil.WritePropertyValue(xmlWriter, "StreetAddress", propertyValue, edmTypeName);
                            }

                            atomPayload = writer.ToString();
                            UnitTestsUtil.SendRequestAndVerifyXPath(atomPayload, "/Values(1)/Address/StreetAddress", new KeyValuePair<string, string[]>[0], typeof(TestDataContext2), UnitTestsUtil.MimeTextXml, "PUT", null, false);
                        }
                    }
                }
            }

            public class TestDataContext2 : Microsoft.OData.Service.Providers.IDataServiceUpdateProvider, IServiceProvider
            {
                public static Type ExpectedType = null;

                public TestDataContext2()
                {
                    provider = PopulateMetadata(this);
                }

                private static p.IDataServiceMetadataProvider PopulateMetadata(object dataSourceInstance)
                {
                    List<p.ResourceType> types = new List<p.ResourceType>(2);

                    p.ResourceType person = new p.ResourceType(
                        typeof(RowEntityTypeWithIDAsKey),
                        p.ResourceTypeKind.EntityType,
                        null, /*baseType*/
                        "AstoriaUnitTests.Stubs", /*namespaceName*/
                        "Person",
                        false /*isAbstract*/);
                    person.IsOpenType = true;

                    p.ResourceProperty idProperty = new p.ResourceProperty(
                        "ID",
                        p.ResourcePropertyKind.Key | p.ResourcePropertyKind.Primitive,
                        p.ResourceType.GetPrimitiveResourceType(typeof(int)));
                    p.ResourceProperty descriptionProperty = new p.ResourceProperty(
                        "Description",
                        p.ResourcePropertyKind.Primitive,
                        p.ResourceType.GetPrimitiveResourceType(typeof(double?)));
                    descriptionProperty.CanReflectOnInstanceTypeProperty = false;

                    p.ResourceType address = new p.ResourceType(
                        typeof(RowComplexType),
                        p.ResourceTypeKind.ComplexType,
                        null, /*baseType*/
                        "AstoriaUnitTests.Stubs", /*namespaceName*/
                        "Address",
                        false /*isAbstract*/);

                    p.ResourceProperty streetAddressProperty = new p.ResourceProperty("StreetAddress", p.ResourcePropertyKind.Primitive, p.ResourceType.GetPrimitiveResourceType(typeof(string)));
                    streetAddressProperty.CanReflectOnInstanceTypeProperty = false;
                    address.AddProperty(streetAddressProperty);

                    p.ResourceProperty addressProperty = new p.ResourceProperty("Address", p.ResourcePropertyKind.ComplexType, address);
                    addressProperty.CanReflectOnInstanceTypeProperty = false;

                    person.AddProperty(idProperty);
                    person.AddProperty(descriptionProperty);
                    person.AddProperty(addressProperty);

                    types.Add(address);
                    types.Add(person);

                    List<p.ResourceSet> containers = new List<p.ResourceSet>(1);
                    containers.Add(new p.ResourceSet("Values", person));

                    return new CustomDataServiceProvider(containers, types, new List<p.ServiceOperation>(), new List<p.ResourceAssociationSet>(), dataSourceInstance);
                }

                public static void InitializeService(DataServiceConfiguration configuration)
                {
                    configuration.EnableTypeConversion = false;
                }

                public IQueryable<RowEntityTypeWithIDAsKey> Values
                {
                    get
                    {
                        var instance = new AstoriaUnitTests.Stubs.RowEntityTypeWithIDAsKey("AstoriaUnitTests.Stubs.Person") { ID = 1 };
                        instance.Properties["Address"] = new AstoriaUnitTests.Stubs.RowComplexType("AstoriaUnitTests.Stubs.Address");
                        return new RowEntityTypeWithIDAsKey[] { instance }.AsQueryable();
                    }
                }

                #region IUpdatable Members

                public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
                {
                    throw new NotImplementedException();
                }

                public void ClearChanges()
                {
                    throw new NotImplementedException();
                }

                public object CreateResource(string containerName, string fullTypeName)
                {
                    if (fullTypeName == "AstoriaUnitTests.Stubs.Person")
                    {
                        return new AstoriaUnitTests.Stubs.RowEntityTypeWithIDAsKey(fullTypeName);
                    }
                    else if (fullTypeName == "AstoriaUnitTests.Stubs.Address")
                    {
                        return new AstoriaUnitTests.Stubs.RowComplexType(fullTypeName);
                    }

                    throw new NotSupportedException(String.Format("UnexpectedType: '{0}'", fullTypeName));
                }

                public void DeleteResource(object targetResource)
                {
                    throw new NotImplementedException();
                }

                public object GetResource(IQueryable query, string fullTypeName)
                {
                    foreach (var value in query)
                    {
                        return value;
                    }

                    return null;
                }

                public object GetValue(object targetResource, string propertyName)
                {
                    RowEntityTypeWithIDAsKey entityInstance = targetResource as RowEntityTypeWithIDAsKey;
                    if (entityInstance != null)
                    {
                        return entityInstance.Properties[propertyName];
                    }

                    throw new NotImplementedException();
                }

                public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
                {
                    throw new NotImplementedException();
                }

                public object ResetResource(object resource)
                {
                    throw new NotImplementedException();
                }

                public object ResolveResource(object resource)
                {
                    return resource;
                }

                public void SaveChanges()
                {
                    // do nothing
                }

                public void SetReference(object targetResource, string propertyName, object propertyValue)
                {
                    throw new NotImplementedException();
                }

                public void SetValue(object targetResource, string propertyName, object propertyValue)
                {
                    if (propertyName == "Address")
                    {
                        Assert.IsTrue(propertyValue.GetType() == typeof(RowComplexType), "Address property must be of RowComplexType type");
                        return;
                    }

                    Type actualType = null;
                    if (propertyValue == null)
                    {
                        // since if the value is null, we can't deduce the type. Hence don't check.
                        return;
                    }
                    else
                    {
                        actualType = propertyValue.GetType();
                    }

                    // Since we write edm types on the wire, we are not sure what types we convert to when we pass it
                    // to the provider. So we pick the default edm type mapping.
                    if (ExpectedType == typeof(System.Data.Linq.Binary))
                    {
                        ExpectedType = typeof(byte[]);
                    }
                    else if (ExpectedType == typeof(System.Xml.Linq.XElement))
                    {
                        ExpectedType = typeof(string);
                    }

                    // We need to make sure that the type comes as it from the astoria service without the conversion
                    if (ExpectedType == actualType)
                    {
                        return;
                    }

                    throw new Exception(String.Format("Invalid property value specified. Expected: '{0}', Actual: '{1}'", ExpectedType.FullName, actualType.FullName));
                }

                public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IServiceProvider Members

                private Microsoft.OData.Service.Providers.IDataServiceMetadataProvider provider;

                public object GetService(Type serviceType)
                {
                    if (serviceType == typeof(Microsoft.OData.Service.Providers.IDataServiceMetadataProvider) ||
                        serviceType == typeof(Microsoft.OData.Service.Providers.IDataServiceQueryProvider))
                    {
                        return provider;
                    }
                    else
                        if (serviceType == typeof(Microsoft.OData.Service.Providers.IDataServiceUpdateProvider))
                    {
                        return this;
                    }

                    return null;
                }

                #endregion

                private static string GetTypeName(object instance, out bool collection)
                {
                    collection = false;
                    RowComplexType complexType = instance as RowComplexType;
                    if (complexType != null)
                    {
                        return complexType.TypeName;
                    }

                    return null;
                }
            }

            public class TestDataService8 : DataService<CustomDataContext>
            {
                public static void InitializeService(DataServiceConfiguration configuration)
                {
                    configuration.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                    configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                }

                [WebGet]
                public IEnumerable<TestType8> MyServiceOperation()
                {
                    return new List<TestType8>();
                }

                public class TestType8
                {
                    public string ID { get; set; }
                    public string Name { get; set; }
                }
            }

            [TestMethod, Variation("JSON payload should terminate properly")]
            public void JsonPayloadShouldTerminateProperly_1()
            {
                string jsonPayload = "{" +
                                        "@odata.type: \"" + typeof(Customer).FullName + "\" ," +
                                        "Name: \"Foo\"," +
                                        "ID: 125," +
                                        "Address : " +
                                        "{" +
                                            "@odata.type: \"" + typeof(Address).FullName + "\"," +
                                            "StreetAddress: \"Street Number, Street Address\"," +
                                            "City: \"Redmond\"," +
                                            "State: \"WA\"," +
                                            "PostalCode: \"98052\"" +
                                        "}" +
                                     "}Some_Var1_That_Should_Cause_An_Exception";

                UnitTestsUtil.VerifyInvalidRequestForVariousProviders(jsonPayload, "/Customers", UnitTestsUtil.JsonLightMimeType, "POST", 400);
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("xmlns on innererror tag is incorrectly declared")]
            public void XmlNsOnInnerErrorTagShouldBeCorrect()
            {
                using (TestUtil.RestoreStaticValueOnDispose(typeof(OpenWebDataServiceHelper), "ForceVerboseErrors"))
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    // force vorbose so we can get a print out of the error in xml
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/Customers";
                    request.Accept = UnitTestsUtil.AtomFormat;
                    OpenWebDataServiceHelper.ForceVerboseErrors = true;
                    request.TestArguments = new Hashtable();

                    // The following test arguments, when passed into CustomerDataContext, will
                    // throw an exception, and the details of the exception can be read by getting
                    // the response stream - see CustomDataContext.cs
                    request.TestArguments[CustomDataContext.CustomerCountArgument] = 10;
                    request.TestArguments[CustomDataContext.ExceptionTypeArgument] = typeof(FormatException);
                    request.TestArguments[CustomDataContext.ExceptionAtEndArgument] = true;

                    TestUtil.RunCatching(request.SendRequest);

                    // Read the response stream
                    MemoryStream response = new MemoryStream();
                    TestUtil.CopyStream(request.GetResponseStream(), response);
                    response.Position = 0;
                    byte[] buffer = response.GetBuffer();
                    string text = System.Text.Encoding.UTF8.GetString(buffer, 0, (int)response.Length);

                    // make sure the faulty namespace declaration is gone...
                    TestUtil.AssertContainsFalse(text, "xmlns=\"xmlns\"");

                    // make sure innererror is under m: namespace (metadata)
                    TestUtil.AssertContains(text, "<m:innererror>");
                }
            }

            [TestMethod, Variation("Json Deserializer doesn't converting the values when EnableTypeConversion is true")]
            public void JsonDeserializerShouldConvertValuesWhenEnableTypeConversionIsTrue()
            {
                string payload = "{ ID: 1234.56, Name: 'foo' }";
                UnitTestsUtil.VerifyInvalidRequest(payload, "/Data", typeof(TestDataContext9), UnitTestsUtil.JsonLightMimeTypeIeee754Compatible, "POST", 400);
            }

            public class TestDataContext9 : IServiceProvider, Microsoft.OData.Service.Providers.IDataServiceUpdateProvider
            {
                private p.IDataServiceMetadataProvider provider;
                public TestDataContext9()
                {
                    p.ResourceType resourceType = new p.ResourceType(
                        typeof(TestType9),
                        p.ResourceTypeKind.EntityType,
                        null,
                        typeof(TestType9).Namespace,
                        typeof(TestType9).Name,
                        false);

                    resourceType.AddProperty(new p.ResourceProperty("ID", p.ResourcePropertyKind.Key | p.ResourcePropertyKind.Primitive, p.ResourceType.GetPrimitiveResourceType(typeof(decimal))));
                    resourceType.AddProperty(new p.ResourceProperty("Name", p.ResourcePropertyKind.Primitive, p.ResourceType.GetPrimitiveResourceType(typeof(string))));

                    List<p.ResourceSet> containers = new List<p.ResourceSet>() {
                        new p.ResourceSet("Data", resourceType) };
                    List<p.ResourceType> types = new List<p.ResourceType>() { resourceType };
                    List<p.ServiceOperation> operations = new List<p.ServiceOperation>(0);
                    List<p.ResourceAssociationSet> associationSets = new List<Microsoft.OData.Service.Providers.ResourceAssociationSet>(0);

                    this.provider = new CustomDataServiceProvider(containers, types, operations, associationSets, this);
                }

                IQueryable<TestType9> Data
                {
                    get { return new TestType9[0].AsQueryable(); }
                }

                #region IServiceProvider Members

                public object GetService(Type serviceType)
                {
                    if (serviceType == typeof(p.IDataServiceMetadataProvider) ||
                        serviceType == typeof(p.IDataServiceQueryProvider))
                    {
                        return this.provider;
                    }
                    else if (serviceType == typeof(Microsoft.OData.Service.Providers.IDataServiceUpdateProvider))
                    {
                        return this;
                    }

                    return null;
                }

                #endregion

                public class TestType9
                {
                    public decimal ID { get; set; }
                    public string Name { get; set; }
                }

                #region IUpdatable Members

                public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
                {
                    throw new NotImplementedException();
                }

                public void ClearChanges()
                {
                    throw new NotImplementedException();
                }

                public object CreateResource(string containerName, string fullTypeName)
                {
                    return new TestType9();
                }

                public void DeleteResource(object targetResource)
                {
                    throw new NotImplementedException();
                }

                public object GetResource(IQueryable query, string fullTypeName)
                {
                    throw new NotImplementedException();
                }

                public object GetValue(object targetResource, string propertyName)
                {
                    throw new NotImplementedException();
                }

                public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
                {
                    throw new NotImplementedException();
                }

                public object ResetResource(object resource)
                {
                    throw new NotImplementedException();
                }

                public object ResolveResource(object resource)
                {
                    throw new NotImplementedException();
                }

                public void SaveChanges()
                {
                    throw new NotImplementedException();
                }

                public void SetReference(object targetResource, string propertyName, object propertyValue)
                {
                    throw new NotImplementedException();
                }

                public void SetValue(object targetResource, string propertyName, object propertyValue)
                {
                    targetResource.GetType().GetProperty(propertyName).SetValue(targetResource, propertyValue, null);
                }

                public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
                {
                    throw new NotImplementedException();
                }

                #endregion
            }

            [TestMethod, Variation("no abstract attribute showing up in csdl")]
            public void AbstractAttributeShouldShowInCsdl()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.Accept = UnitTestsUtil.MimeApplicationXml;
                    request.ServiceType = typeof(TestDataService10);
                    request.RequestUriString = "/$metadata";
                    request.HttpMethod = "GET";
                    request.SendRequest();
                    XmlDocument document = request.GetResponseStreamAsXmlDocument();
                    UnitTestsUtil.VerifyXPaths(document, "//csdl:EntityType[@Name='BaseCustomer' and @Abstract='true']");
                }
            }

            public class TestDataService10 : DataService<TestDataContext10>, IServiceProvider
            {
                public static void InitializeService(IDataServiceConfiguration configuration)
                {
                    configuration.SetEntitySetAccessRule("*", EntitySetRights.ReadSingle);
                    configuration.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                }

                #region IServiceProvider Members

                public p.IDataServiceMetadataProvider provider;
                public object GetService(Type serviceType)
                {
                    if (serviceType == typeof(p.IDataServiceMetadataProvider) ||
                        serviceType == typeof(p.IDataServiceQueryProvider))
                    {
                        if (provider == null)
                        {
                            p.ResourceType baseCustomerType = new p.ResourceType(typeof(CustomerRow), p.ResourceTypeKind.EntityType, null, "Foo", "BaseCustomer", true);
                            p.ResourceType customerType = new p.ResourceType(typeof(CustomerRow), p.ResourceTypeKind.EntityType, baseCustomerType, "Foo", "Customer", false);
                            p.ResourceProperty keyProperty = new p.ResourceProperty("Id", p.ResourcePropertyKind.Primitive | p.ResourcePropertyKind.Key, p.ResourceType.GetPrimitiveResourceType(typeof(string)));
                            baseCustomerType.AddProperty(keyProperty);

                            List<p.ResourceType> types = new List<p.ResourceType>() { baseCustomerType, customerType };

                            List<p.ResourceSet> containers = new List<p.ResourceSet>() {
                                new p.ResourceSet("Customers", customerType) };

                            List<p.ResourceAssociationSet> associationSets = new List<Microsoft.OData.Service.Providers.ResourceAssociationSet>(0);

                            provider = new CustomDataServiceProvider(containers, types, new List<p.ServiceOperation>(), associationSets, new TestDataContext10());
                        }

                        return provider;
                    }

                    return null;
                }

                #endregion
            }

            public class TestDataContext10
            {
                public IQueryable<Customer> Customers
                {
                    get { return new List<Customer>().AsQueryable(); }
                }
            }

            [TestMethod, Variation("Making sure POST on service operations work if IUpdatable is not implemented")]
            public void PostOnServiceOperationsShouldWorkWithoutIUpdatable_Part1()
            {
                TestDataContext11.customers.Clear();
                try
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.HttpMethod = "POST";
                        request.RequestUriString = "/InsertCustomer?id=1&name='foo'";
                        request.ServiceType = typeof(TestDataService11<TestDataContext11>);
                        request.SendRequest();
                    }

                    Assert.IsTrue(
                        TestDataContext11.customers.Count == 1 &&
                        TestDataContext11.customers[0].ID == 1 &&
                        TestDataContext11.customers[0].Name == "foo",
                        "expecting one customer to be inserted with correct data");
                }
                finally
                {
                    TestDataContext11.customers.Clear();
                }
            }

            public class TestDataContext11
            {
                public static IList<TestEntityType11> customers = new List<TestEntityType11>();
                public TestDataContext11()
                {
                    customers = new List<TestEntityType11>();
                }

                public IQueryable<TestEntityType11> Customers
                {
                    get { return customers.AsQueryable(); }
                }

                public void InsertCustomer(int id, string name)
                {
                    TestEntityType11 customer = new TestEntityType11();
                    customer.ID = id;
                    customer.Name = name;
                    TestDataContext11.customers.Add(customer);
                }
            }

            [TestMethod, Variation("Making sure POST on service operations work if IUpdatable is implemented and SaveChanges is called")]
            public void PostOnServiceOperationsShouldWorkWithIUpdatable_Part2()
            {
                TestDataContext12.customers.Clear();
                try
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.HttpMethod = "POST";
                        request.RequestUriString = "/InsertCustomer?id=1&name='foo'";
                        request.ServiceType = typeof(TestDataService11<TestDataContext12>);
                        request.SendRequest();
                    }

                    Assert.IsTrue(
                        TestDataContext12.customers.Count == 1 &&
                        TestDataContext12.customers[0].ID == 1 &&
                        TestDataContext12.customers[0].Name == "foo",
                        "expecting one customer to be inserted with correct data");
                }
                finally
                {
                    TestDataContext12.customers.Clear();
                }
            }

            public class TestDataContext12 : IUpdatable
            {
                public static IList<TestEntityType11> customers = new List<TestEntityType11>();
                private List<TestEntityType11> customerToBeAdded = new List<TestEntityType11>();
                public TestDataContext12()
                {
                    customers = new List<TestEntityType11>();
                }

                public IQueryable<TestEntityType11> Customers
                {
                    get { return customers.AsQueryable(); }
                }

                public void InsertCustomer(int id, string name)
                {
                    TestEntityType11 customer = new TestEntityType11();
                    customer.ID = id;
                    customer.Name = name;
                    this.customerToBeAdded.Add(customer);
                }

                #region IUpdatable Members

                public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
                {
                    throw new NotImplementedException();
                }

                public void ClearChanges()
                {
                    throw new NotImplementedException();
                }

                public object CreateResource(string containerName, string fullTypeName)
                {
                    throw new NotImplementedException();
                }

                public void DeleteResource(object targetResource)
                {
                    throw new NotImplementedException();
                }

                public object GetResource(IQueryable query, string fullTypeName)
                {
                    throw new NotImplementedException();
                }

                public object GetValue(object targetResource, string propertyName)
                {
                    throw new NotImplementedException();
                }

                public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
                {
                    throw new NotImplementedException();
                }

                public object ResetResource(object resource)
                {
                    throw new NotImplementedException();
                }

                public object ResolveResource(object resource)
                {
                    throw new NotImplementedException();
                }

                public void SaveChanges()
                {
                    foreach (var customer in this.customerToBeAdded)
                    {
                        customers.Add(customer);
                    }
                }

                public void SetReference(object targetResource, string propertyName, object propertyValue)
                {
                    throw new NotImplementedException();
                }

                public void SetValue(object targetResource, string propertyName, object propertyValue)
                {
                    throw new NotImplementedException();
                }

                #endregion
            }

            public class TestEntityType11
            {
                public int ID { get; set; }
                public string Name { get; set; }
            }

            public class TestDataService11<T> : DataService<T>
            {
                public static void InitializeService(IDataServiceConfiguration configuration)
                {
                    configuration.SetEntitySetAccessRule("Customers", EntitySetRights.ReadSingle);
                    configuration.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                }

                [WebInvoke]
                public void InsertCustomer(int id, string name)
                {
                    typeof(T).GetMethod("InsertCustomer").Invoke(this.CurrentDataSource, new object[] { id, name });
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Small number seen as zero")]
            public void SmallNumberShouldNotBeTreatedAsZero()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/Customers?$filter=4.67E-16f div -3.4E38f eq 0.0f";
                    request.HttpMethod = "GET";
                    request.Accept = "application/atom+xml";
                    request.SendRequest();
                    String s = request.GetResponseStreamAsText();
                    TestUtil.AssertContainsFalse(s, "entry");

                    request.RequestUriString = "/Customers?$filter=4.67E-16f div -3.4E38f lt 0.0f";
                    request.Accept = "application/atom+xml";
                    request.SendRequest();
                    String s2 = request.GetResponseStreamAsText();
                    TestUtil.AssertContains(s2, "entry");
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod]
            public void ALinqExceptionInSelectCast()
            {
                ServiceModelData.Northwind.EnsureDependenciesAvailable();
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = ServiceModelData.Northwind.ServiceModelType;
                    request.StartService();

                    Microsoft.OData.Client.DataServiceContext ctx = new northwindClient.northwindContext(new Uri(request.BaseUri));
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();
                    ctx.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

                    var q1 = ctx.CreateQuery<northwindClient.Customers>("Customers").Select(c => c as northwindClient.Customers);

                    foreach (northwindClient.Customers c in q1)
                    {
                        Assert.IsNotNull(c);
                    }

                    var q2 = ctx.CreateQuery<northwindClient.Customers>("Customers").Select(c => (northwindClient.Customers)c);

                    foreach (northwindClient.Customers c in q2)
                    {
                        Assert.IsNotNull(c);
                    }

                    var q3 = from c in ctx.CreateQuery<northwindClient.Customers>("Customers")
                             where c.CustomerID == "ALFKI"
                             select c.ContactName as string;

                    foreach (string name in q3)
                    {
                        Assert.IsNotNull(name);
                    }

                    var q4 = from c in ctx.CreateQuery<northwindClient.Customers>("Customers")
                             where c.CustomerID == "ALFKI"
                             select (string)c.ContactName;

                    foreach (string name in q4)
                    {
                        Assert.IsNotNull(name);
                    }

                    var q5 = (from c in ctx.CreateQuery<northwindClient.Customers>("Customers")
                              where c.CustomerID == "ALFKI"
                              select c).SelectMany(c => c.Orders as Collection<northwindClient.Orders>);

                    foreach (northwindClient.Orders o in q5)
                    {
                        Assert.IsNotNull(o);
                    }

                    var q6 = (from c in ctx.CreateQuery<northwindClient.Customers>("Customers")
                              where c.CustomerID == "ALFKI"
                              select c).SelectMany(c => (Collection<northwindClient.Orders>)c.Orders);

                    foreach (northwindClient.Orders o in q6)
                    {
                        Assert.IsNotNull(o);
                    }
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Invoking Service Operation which is defined somewhere else other than data service in custom provider")]
            public void TestServiceOperationDefinedInCustomProvider()
            {
                string uri = "/IntServiceOperation";
                var xPaths = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>(uri, new string[] { "/adsm:value[text()='5']" }) };

                UnitTestsUtil.CustomProviderRequest(typeof(CustomRowBasedContext), uri, UnitTestsUtil.AtomFormat, null, xPaths, "GET", false);
            }

            [TestMethod, Variation("ReflectionServiceProvider.GetValue() should not throw if the value returned is null")]
            public void ReflectionServiceProviderShouldNotThrowIfReturnNull()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(TestDataContext13);
                    request.RequestUriString = "/Customers(1)/Name/$value";
                    request.HttpMethod = "PUT";
                    request.IfMatch = "W/\"null\"";
                    request.RequestContentType = UnitTestsUtil.MimeTextPlain;
                    request.SetRequestStreamAsText("name");

                    request.SendRequest();
                    Assert.AreEqual(204, request.ResponseStatusCode);
                    Assert.AreEqual("W/\"'name'\"", request.ResponseETag);
                    Assert.AreEqual("name", TestDataContext13.customers[0].Name);
                }
            }

            public class TestDataContext13 : IUpdatable
            {
                public static IList<TestEntityType13> customers = new List<TestEntityType13>();
                public TestDataContext13()
                {
                    customers = new List<TestEntityType13>();
                    TestEntityType13 customer = new TestEntityType13();
                    customer.ID = 1;
                    customer.Name = null;
                    customers.Add(customer);
                }

                public IQueryable<TestEntityType13> Customers
                {
                    get { return customers.AsQueryable(); }
                }

                #region IUpdatable Members

                public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
                {
                    throw new NotImplementedException();
                }

                public void ClearChanges()
                {
                    throw new NotImplementedException();
                }

                public object CreateResource(string containerName, string fullTypeName)
                {
                    throw new NotImplementedException();
                }

                public void DeleteResource(object targetResource)
                {
                    throw new NotImplementedException();
                }

                public object GetResource(IQueryable query, string fullTypeName)
                {
                    object resource = null;

                    foreach (object r in query)
                    {
                        if (resource != null)
                        {
                            throw new ArgumentException(String.Format("Invalid Uri specified. The query '{0}' must refer to a single resource", query.ToString()));
                        }

                        resource = r;
                    }

                    return resource;
                }

                public object GetValue(object targetResource, string propertyName)
                {
                    PropertyInfo propertyInfo = targetResource.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    return propertyInfo.GetValue(targetResource, null);
                }

                public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
                {
                    throw new NotImplementedException();
                }

                public object ResetResource(object resource)
                {
                    throw new NotImplementedException();
                }

                public object ResolveResource(object resource)
                {
                    return resource;
                }

                public void SaveChanges()
                {
                }

                public void SetReference(object targetResource, string propertyName, object propertyValue)
                {
                    throw new NotImplementedException();
                }

                public void SetValue(object targetResource, string propertyName, object propertyValue)
                {
                    PropertyInfo propertyInfo = targetResource.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    propertyInfo.SetValue(targetResource, propertyValue, null);
                }

                #endregion
            }

            [ETag("Name")]
            public class TestEntityType13
            {
                public int ID { get; set; }
                public string Name { get; set; }
            }

            [TestMethod, Variation("PUT should always check etag values if the type has etag properties.")]
            public void PutShouldCheckEtagIfTypeHasEtagProperties()
            {
                using (CustomDataContext.CreateChangeScope())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/Customers(1)/Name/$value";
                    request.HttpMethod = "PUT";
                    request.RequestContentType = UnitTestsUtil.MimeTextPlain;
                    request.SetRequestStreamAsText("new name");

                    Exception e = TestUtil.RunCatching(request.SendRequest);
                    Assert.AreEqual(400, request.ResponseStatusCode);
                    Assert.IsNotNull(e);
                }
            }

            // [TestMethod, Variation("Incorrect token error response should have correct serialization format")]
            [Ignore]
            public void IncorrectTokenErrorResponseShouldHaveCorrectSerializationFormat()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(new Dimension("SerializationFormatData", SerializationFormatData.StructuredValues));

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    using (TestWebRequest request = TestWebRequest.CreateForLocal())
                    {
                        SerializationFormatData format = (SerializationFormatData)values["SerializationFormatData"];

                        request.DataServiceType = typeof(CustomDataContext);
                        request.RequestUriString = "/Customers?$ttop=1";
                        request.Accept = format.MimeTypes[0];
                        Exception e = TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNotNull(e, "Must have caused an error because of bad query parameter");
                        Assert.AreEqual(400, request.ResponseStatusCode);
                        string expectedFormat = format == SerializationFormatData.JsonLight ? UnitTestsUtil.JsonLightMimeType : UnitTestsUtil.MimeApplicationXml;
                        expectedFormat = string.Format("{0};", expectedFormat);
                        Assert.IsTrue(request.ResponseContentType.StartsWith(expectedFormat, StringComparison.OrdinalIgnoreCase), string.Format("Expected: '{0}' Actual: {1}", expectedFormat, request.ResponseContentType));
                        String result = request.GetResponseStreamAsText();
                        Assert.IsTrue(result.Contains("system-reserved"), "Must received error with reserved keyword error");
                    }
                });
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Row Count query string should allow heading white spaces")]
            public void RowCountQueryStringShouldAllowHeadingWhitespaces()
            {
                TestUtil.ClearMetadataCache();

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("RequestUri", new string[] {
                            "/Customers?$count=%20true",
                            "/Customers?$count=%20%20true"
                    }));

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.DataServiceType = typeof(CustomDataContext);
                        request.Accept = UnitTestsUtil.AtomFormat;
                        request.RequestUriString = values["RequestUri"].ToString();
                        request.SendRequest();
                        string s = request.GetResponseStreamAsText();
                        TestUtil.AssertSelectNodes(
                            XmlUtil.XmlDocumentFromString(s),
                            "//adsm:count[text() = '3']");
                    }
                });
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Cannot invoke Service Op in Batch")]
            public void CannotInvokeServiceOpInBatch()
            {
                TestUtil.ClearMetadataCache();

                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.ServiceType = typeof(ServiceOpAndChangeInterceptorService);
                    request.RequestUriString = "/$batch";
                    request.RequestContentType = "multipart/mixed; boundary=batch_1954f60c-6ed2-4959-89bf-95d727a36507";
                    request.HttpMethod = "POST";
                    string requestContent = @"
--batch_1954f60c-6ed2-4959-89bf-95d727a36507
Content-Type: multipart/mixed; boundary=changeset_ac0127bc-56fc-438f-a7e9-b2752fa3c1dc

--changeset_ac0127bc-56fc-438f-a7e9-b2752fa3c1dc
Content-Type: application/http
Content-Transfer-Encoding: binary

POST http://host/SOP_InvokeQueryable HTTP/1.1

--changeset_ac0127bc-56fc-438f-a7e9-b2752fa3c1dc--
--batch_1954f60c-6ed2-4959-89bf-95d727a36507
Content-Type: application/http
Content-Transfer-Encoding: binary

GET http://host/SOP_GetQueryable HTTP/1.1
Content-ID: 2
Content-Length: 0

--batch_1954f60c-6ed2-4959-89bf-95d727a36507--
";
                    request.SetRequestStreamAsText(requestContent);
                    request.SendRequest();
                    request.GetResponseStreamAsText();
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("ChangeInterceptor not fired on DELETE $ref")]
            public void ChangeInterceptorShouldBeFiredOnDeleteRef()
            {
                var testCases = new[] {
                    new {
                        InvokeInterceptorsOnLinkDelete = false,
                        ExpectedCustomersInterceptorCount = 0,
                        ExpectedOrdersInterceptorCount = 0
                    },
                    new {
                        InvokeInterceptorsOnLinkDelete = true,
                        ExpectedCustomersInterceptorCount = 1,
                        ExpectedOrdersInterceptorCount = 0
                    }
                };

                foreach (var testCase in testCases)
                {
                    TestUtil.ClearConfiguration();

                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.ServiceType = typeof(ServiceOpAndChangeInterceptorService);

                        ServiceOpAndChangeInterceptorService.InvokeInterceptorsOnLinkDelete = testCase.InvokeInterceptorsOnLinkDelete;
                        Assert.AreEqual(0, GetNumberOfChangeInterceptorCalls(request, "SOP_CustomersInterceptorCount"));
                        Assert.AreEqual(0, GetNumberOfChangeInterceptorCalls(request, "SOP_OrdersInterceptorCount"));

                        request.RequestUriString = "/Customers(0)/Orders/$ref?$id=Orders(0)";
                        request.HttpMethod = "DELETE";
                        request.Accept = UnitTestsUtil.AtomFormat;
                        request.SendRequest();

                        Assert.AreEqual(testCase.ExpectedCustomersInterceptorCount, GetNumberOfChangeInterceptorCalls(request, "SOP_CustomersInterceptorCount"));
                        Assert.AreEqual(testCase.ExpectedOrdersInterceptorCount, GetNumberOfChangeInterceptorCalls(request, "SOP_OrdersInterceptorCount"));
                    }
                }
            }

            private int GetNumberOfChangeInterceptorCalls(TestWebRequest request, string serviceOpName)
            {
                request.HttpMethod = "GET";
                request.Accept = UnitTestsUtil.MimeApplicationXml;
                request.RequestUriString = "/" + serviceOpName;
                request.SendRequest();
                using (XmlReader xr = XmlReader.Create(request.GetResponseStream()))
                {
                    XDocument xdoc = XDocument.Load(xr);
                    XNamespace ns = "http://docs.oasis-open.org/odata/ns/data";
                    XNamespace nsm = "http://docs.oasis-open.org/odata/ns/metadata";
                    return (int)xdoc.Element(nsm + "value").Element(nsm + "element");
                }
            }

            public class ServiceOpAndChangeInterceptorService : DataService<CustomDataContext>
            {
                static Customer[] _custs = new Customer[3] {
                    new Customer() { ID=0, Name="Customer 0" },
                    new Customer() { ID=1, Name="Customer 1" },
                    new Customer() { ID=2, Name="Customer 2" }};

                private static int custInterceptorCount = 0;
                private static int ordersInterceptorCount = 0;
                public static bool InvokeInterceptorsOnLinkDelete = true;

                // This method is called only once to initialize service-wide policies.
                public static void InitializeService(DataServiceConfiguration config)
                {
                    // Validate the default value for InvokeInterceptorsOnLinkDelete setting - should be true
                    Assert.AreEqual(true, config.DataServiceBehavior.InvokeInterceptorsOnLinkDelete);

                    config.SetEntitySetAccessRule("*", EntitySetRights.All);
                    config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                    config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                    config.DataServiceBehavior.InvokeInterceptorsOnLinkDelete = InvokeInterceptorsOnLinkDelete;
                }

                [WebInvoke(Method = "POST")]
                public IQueryable<Customer> SOP_InvokeQueryable()
                {
                    return (_custs).AsQueryable();
                }

                [WebGet]
                public IQueryable<Customer> SOP_GetQueryable()
                {
                    return (_custs).AsQueryable();
                }

                [WebGet]
                public IEnumerable<int> SOP_CustomersInterceptorCount()
                {
                    return new int[] { custInterceptorCount };
                }

                [WebGet]
                public IEnumerable<int> SOP_OrdersInterceptorCount()
                {
                    return new int[] { ordersInterceptorCount };
                }

                [ChangeInterceptor("Orders")]
                public void OrdersChanged(Order order, UpdateOperations operation)
                {
                    ordersInterceptorCount++;
                }

                [ChangeInterceptor("Customers")]
                public void CustomersChanged(Customer customer, UpdateOperations operation)
                {
                    custInterceptorCount++;
                }
            }

            [TestMethod, Variation("OData-Version is sent as 4.0 in all non-GET error cases")]
            public void NonGetErrorResponseVersionShouldBe40()
            {
                using (CustomDataContext.CreateChangeScope())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/Customers(5)";
                    request.HttpMethod = "DELETE";

                    Exception e = TestUtil.RunCatching(request.SendRequest);
                    Assert.AreEqual(404, request.ResponseStatusCode);
                    Assert.IsNotNull(e);
                    Assert.AreEqual(request.ResponseVersion, "4.0;", "response version must be 4.0");
                }
            }

            [TestMethod, Variation("In Batch Payload, for binary data, \r\n is read as part of the data")]
            public void NewlineShouldReadAsPartOfBinaryDataInBatchPayload()
            {
                TypedCustomDataContext<AllTypes>.ClearValues();
                TypedCustomDataContext<AllTypes>.PreserveChanges = true;
                EventHandler handler = new EventHandler((sender, e) =>
                {
                    var context = (TypedCustomDataContext<AllTypes>)sender;
                    var theValue = new AllTypes();

                    theValue.ID = 1;
                    theValue.BinaryType = new byte[] { 1, 2, 3, 4 };

                    context.SetValues(new object[] { theValue });
                });

                TypedCustomDataContext<AllTypes>.ValuesRequested += handler;

                try
                {
                    string fileName = Path.Combine(TestUtil.ServerUnitTestSamples, @"tests\BatchRequests\BinaryDataBatchRequest.txt");
                    string batchRequest = File.ReadAllText(fileName);
                    string[] segments = batchRequest.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    string boundary = segments[0].Substring(2);
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.DataServiceType = typeof(TypedCustomDataContext<AllTypes>);
                        request.RequestUriString = "/$batch";
                        request.Accept = UnitTestsUtil.MimeMultipartMixed;
                        request.HttpMethod = "POST";
                        request.RequestContentType = String.Format("{0}; boundary={1}", UnitTestsUtil.MimeMultipartMixed, boundary);
                        request.RequestStream = IOUtil.CreateStream(batchRequest);
                        request.SendRequest();
                    }

                    TypedCustomDataContext<AllTypes> context = new TypedCustomDataContext<AllTypes>();
                    AllTypes type = context.Values.ToArray()[0];
                    byte[] binaryData = type.BinaryType;

                    MemoryStream stream = new MemoryStream();
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write("Some binary data");
                    writer.Flush();

                    long length = stream.Position;
                    byte[] expectedData = new byte[length];
                    stream.Position = 0;
                    int bytesRead = stream.Read(expectedData, 0, expectedData.Length);
                    Assert.IsTrue(bytesRead == length, "All data is read");
                    Assert.AreEqual(length, binaryData.Length, "The length must be the same");
                    for (int i = 0; i < length; i++)
                    {
                        Assert.AreEqual(binaryData[i], expectedData[i], "data should match");
                    }
                }
                finally
                {
                    TypedCustomDataContext<AllTypes>.ValuesRequested -= handler;
                }

                TypedCustomDataContext<AllTypes>.PreserveChanges = false;
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("POST in batch operation returns incorrect Location")]
            public void PostInBatchOperationsShouldReturnCorrectLocation()
            {
                TestUtil.ClearMetadataCache();

                using (TestUtil.RestoreStaticValueOnDispose(typeof(BaseTestWebRequest), "HostInterfaceType"))
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    ServiceModelData.Northwind.EnsureDependenciesAvailable();
                    BaseTestWebRequest.HostInterfaceType = typeof(IDataServiceHost2);
                    request.DataServiceType = typeof(NorthwindModel.NorthwindContext);
                    request.RequestUriString = "/$batch";
                    request.RequestContentType = "multipart/mixed; boundary=bb";
                    request.HttpMethod = "POST";
                    string requestContent = @"
--bb
Content-Type: multipart/mixed; boundary=bc

--bc
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

POST http://host/Products HTTP/1.1
Host: localhost
Accept: application/atom+xml
Content-Type: application/atom+xml;type=entry

<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<entry xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
  <title />
  <author>
    <name />
  </author>
  <updated>2009-02-28T00:40:21.9191941Z</updated>
  <id />
  <content type=""application/xml"">
    <m:properties>
      <d:Discontinued m:type=""Edm.Boolean"">false</d:Discontinued>
      <d:ProductID m:type=""Edm.Int32"">0</d:ProductID>
      <d:ProductName>Product 0</d:ProductName>
      <d:QuantityPerUnit m:null=""true"" />
      <d:ReorderLevel m:type=""Edm.Int16"" m:null=""true"" />
      <d:UnitPrice m:type=""Edm.Decimal"">3</d:UnitPrice>
      <d:UnitsInStock m:type=""Edm.Int16"">1</d:UnitsInStock>
      <d:UnitsOnOrder m:type=""Edm.Int16"" m:null=""true"" />
    </m:properties>
  </content>
</entry>

--bc
Content-Type: application/http
Content-Transfer-Encoding:binary
Content-ID: 2

PUT /$1 HTTP/1.1
Host: localhost
Accept: application/atom+xml
Content-Type: application/atom+xml;type=entry

<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<entry xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
  <title />
  <author>
    <name />
  </author>
  <updated>2009-02-28T00:40:21.9191941Z</updated>
  <id />
  <content type=""application/xml"">
    <m:properties>
      <d:Discontinued m:type=""Edm.Boolean"">false</d:Discontinued>
      <d:ProductID m:type=""Edm.Int32"">0</d:ProductID>
      <d:ProductName>Product 0</d:ProductName>
      <d:QuantityPerUnit m:null=""true"" />
      <d:ReorderLevel m:type=""Edm.Int16"" m:null=""true"" />
      <d:UnitPrice m:type=""Edm.Decimal"">3.5</d:UnitPrice>
      <d:UnitsInStock m:type=""Edm.Int16"">10</d:UnitsInStock>
      <d:UnitsOnOrder m:type=""Edm.Int16"" m:null=""true"" />
    </m:properties>
  </content>
</entry>

--bc--
--bb--";

                    request.SetRequestStreamAsText(requestContent);
                    request.SendRequest();
                    string s = request.GetResponseStreamAsText();

                    string dsv;
                    Assert.IsTrue(request.ResponseHeaders.TryGetValue("OData-Version", out dsv));
                    Assert.AreEqual(dsv, "4.0");

                    TestUtil.AssertContainsFalse(s, "Location: http://host/Products(0)");
                    TestUtil.AssertContains(s, "<d:UnitPrice m:type=\"Decimal\">3.5</d:UnitPrice>");
                }
            }

            [TestMethod, Variation("Stack Overflow while loading metadata, when using generic type")]
            public void UsingGenericTypeWhileLoadingMetadataShouldNotStackOverflow()
            {
                UnitTestsUtil.GetResponseStream(WebServerLocation.InProcess, UnitTestsUtil.MimeAny, "/$metadata", typeof(TypedCustomDataContext<TestDerived>));
            }

            [Key("Id")]
            public abstract class TestBase
            {
                public int Id { get; set; }
            }

            public class TestIntermediate<T> : TestBase
            {
            }

            public class TestDerived : TestIntermediate<TestDerived>
            {
            }

            [TestMethod, Variation("Making sure InitializeService(DataServiceConfiguration) method is invoked")]
            public void InitializeServiceOfDataServiceConfigurationShouldBeInvoked()
            {
                UnitTestsUtil.GetResponseStream(WebServerLocation.InProcess, UnitTestsUtil.MimeAny, "/Customers", typeof(TestContext14));
                Assert.IsTrue(TestContext14.MethodCalled, "InitializeService Method must be called");
            }

            public class TestContext14
            {
                internal static bool MethodCalled;
                public static void InitializeService(DataServiceConfiguration conf)
                {
                    MethodCalled = true;
                }

                public IQueryable<TestEntity14> Customers
                {
                    get { return new List<TestEntity14>().AsQueryable(); }
                }

                public class TestEntity14
                {
                    public int ID { get; set; }
                    public string Name { get; set; }
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("POST followed by PUT in a single changeset within a batch throws an exception with EF provider")]
            public void PostFollowedByPutInSingleChangesetShouldThrow()
            {
                Type contextType = typeof(ocs.CustomObjectContext);
                string fileName = Path.Combine(TestUtil.ServerUnitTestSamples, @"tests\BatchRequests\PostFollowedByPutBatchRequest.txt");
                string batchRequest = File.ReadAllText(fileName);
                string[] segments = batchRequest.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                string boundary = segments[0].Substring(2);
                batchRequest = UnitTestsUtil.ConvertReflectionProviderTypesToEdmProviderTypes(batchRequest);

                ocs.PopulateData.EntityConnection = null;
                using (EntityConnection connection = ocs.PopulateData.CreateTableAndPopulateData())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = contextType;
                    request.RequestUriString = "/$batch";
                    request.Accept = UnitTestsUtil.MimeMultipartMixed;
                    request.HttpMethod = "POST";
                    request.RequestContentType = String.Format("{0}; boundary={1}", UnitTestsUtil.MimeMultipartMixed, boundary);
                    request.RequestStream = IOUtil.CreateStream(batchRequest);
                    request.SendRequest();

                    // Verify that the request went through
                    ocs.CustomObjectContext context = new ocs.CustomObjectContext(connection);
                    ocs.Hidden.Customer customer = context.Customers.Where(c => c.ID == 123456).ToList().Single();
                    Assert.IsTrue(customer.Name == "FooBazInPut", "The second PUT should succeed");

                    customer.BestFriendReference.Load();
                    Assert.IsTrue(customer.BestFriend.ID == 0, "The best friend must be linked");

                    customer.Orders.Load();
                    Assert.IsTrue(customer.Orders.Single().ID == 0, "The orders link must be present");
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("In $ref request, we used to force uri element in the metadata namespace, while in GET $ref requests, we wrote uri element in data namespace")]
            public void ForceUriElementInVariousNamespaces()
            {
                string atomPayload = "<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id='/Orders(0)' />";
                var atomUriAndXPaths = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>(
                        "/Customers(1)/Orders(0)/$ref",
                        new string[] { "/adsm:ref[@id='http://host/Orders(0)']" }),
                    new KeyValuePair<string, string[]>(
                        "/Customers(1)/Orders(0)",
                        new string[] { "/atom:entry[atom:category/@term='#" + typeof(Order).FullName +
                            "' and atom:id='http://host/Orders(0)' and atom:content/adsm:properties[ads:ID='0' and ads:DollarAmount='20.1']]" })
                };

                UnitTestsUtil.DoInsertsForVariousProviders("/Customers(1)/Orders/$ref", UnitTestsUtil.AtomFormat, atomPayload, atomUriAndXPaths, false /*verifyETag*/);
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("No etag in response payload when one does $expand")]
            public void VerifyEtagInExpandPayload()
            {
                string requestUri = "/Customers(0)?$expand=Orders";
                string atomXPath = "/atom:entry[atom:id='http://host/Customers(0)' and @adsm:etag]";
                string jsonXPath = String.Format("count(/{0}/etag)=0",
                    JsonValidator.ObjectString);

                Stream response = UnitTestsUtil.GetResponseStream(WebServerLocation.InProcess, UnitTestsUtil.AtomFormat, requestUri, typeof(CustomDataContext));
                UnitTestsUtil.VerifyXPaths(response, UnitTestsUtil.AtomFormat, atomXPath);

                response = UnitTestsUtil.GetResponseStream(WebServerLocation.InProcess, UnitTestsUtil.JsonLightMimeType, requestUri, typeof(CustomDataContext));
                UnitTestsUtil.VerifyXPaths(response, UnitTestsUtil.JsonLightMimeType, jsonXPath);
            }

            [TestMethod, Variation("$count requests doesn't throw on if-match or if-none-match headers")]
            public void CountRequestShouldThrowOnIfMatchOrIfNoneMatchHeaders()
            {
                string requestUri = "/Customers/$count";

                KeyValuePair<string, string> ifMatchHeader = new KeyValuePair<string, string>("If-Match", "W/\"some etag\"");
                KeyValuePair<string, string> ifNoneMatchHeader = new KeyValuePair<string, string>("If-None-Match", "W/\"var1 etag\"");

                UnitTestsUtil.VerifyInvalidRequestForVariousProviders(null, requestUri, UnitTestsUtil.MimeTextPlain, "GET", 400, ifMatchHeader);
                UnitTestsUtil.VerifyInvalidRequestForVariousProviders(null, requestUri, UnitTestsUtil.MimeTextPlain, "GET", 400, ifNoneMatchHeader);

                string etag = UnitTestsUtil.GetETagFromResponse(typeof(CustomDataContext), requestUri, UnitTestsUtil.MimeTextPlain);
                Assert.IsTrue(String.IsNullOrEmpty(etag), "No etag must be specified in response headers for $count requests");
            }

            [TestMethod, Variation("OnStartProcessingRequest getting called after service operation is fired")]
            public void OnStartProcessingRequestShouldBeCalledAfterFiringServiceOperation()
            {
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
                {
                    OpenWebDataServiceHelper.ProcessRequestDelegate.Value = (ProcessRequestArgs args) =>
                        {
                            TestDataService15.onStartProcessRequestMethodCalled = true;
                        };

                    UnitTestsUtil.GetResponseStream(WebServerLocation.InProcess, UnitTestsUtil.MimeAny, "/GetCustomers", typeof(TestDataService15));
                }
            }

            public class TestDataService15 : OpenWebDataService<CustomDataContext>
            {
                public static bool onStartProcessRequestMethodCalled = false;

                [WebGet]
                public IQueryable<Customer> GetCustomers()
                {
                    Assert.IsTrue(onStartProcessRequestMethodCalled, "OnStartProcessRequest method should be called before invoking service operation");
                    return new List<Customer>().AsQueryable();
                }
            }

            [TestMethod, Variation("Specifying invalid value for etag returns 500, instead of 412")]
            public void ShouldReturn500ForInvalidEtagValue()
            {
                string payload = "{ ID: 1, Concurrency: 1}";
                Type[] contextTypes = new Type[] {
                    typeof(TypedCustomDataContext<TestEntityType16<int>>),
                    typeof(TypedCustomDataContextWithConcurrencyProvider<TestEntityType16<int>>) };

                using (ChangeScope.GetChangeScope(contextTypes[0]))
                using (ChangeScope.GetChangeScope(contextTypes[1]))
                {
                    UnitTestsUtil.SendRequestAndVerifyXPath(payload, "/Values", null, contextTypes[0], UnitTestsUtil.JsonLightMimeType, "POST", null, true);

                    var headerValues = new KeyValuePair<string, string>[] {
                        new KeyValuePair<string, string>("If-Match", "foo"), // invalid format, since etag must have this format: W/"{0}"
                        new KeyValuePair<string, string>("If-Match", "W/\"foo\""), // invalid value, since etag is a int property
                        new KeyValuePair<string, string>("If-Match", "W/\"12345678934\""), // invalid value, since etag is a int property and this value is larger than int32.maxvalue
                        new KeyValuePair<string, string>("If-Match", "W/\"#4\""), // invalid value for the int key property
                    };

                    var statusCodes = new int[] { 400, 412, 412, 412 };

                    for (int i = 0; i < headerValues.Length; i++)
                    {
                        UnitTestsUtil.VerifyInvalidRequest(payload, "/Values(1)", contextTypes[0], UnitTestsUtil.JsonLightMimeType, "PATCH", statusCodes[i], headerValues[i]);
                        UnitTestsUtil.VerifyInvalidRequest(payload, "/Values(1)", contextTypes[1], UnitTestsUtil.JsonLightMimeType, "PATCH", statusCodes[i], headerValues[i]);
                    }
                }
            }

            // [TestMethod, Variation("Making sure all the etag values are round-trippable")]
            [Ignore]
            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            public void AllEtagValuesShouldRoundtrip()
            {
                string payloadTemplate = "{ ID: 1, Concurrency: XXXX }";

                foreach (p.ResourceType primitiveType in UnitTestsUtil.GetPrimitiveResourceTypes())
                {
                    if (!UnitTestsUtil.IsKeyOrETagPrimitiveType(primitiveType))
                    {
                        // Named Streams are special primitive type that can't be used as etag
                        // Geography types are not yet supported as etags 
                        continue;
                    }

                    TypeData typeData = TypeData.Values.Where(t => t.ClrType == primitiveType.InstanceType).Single();
                    Type contextType = typeof(TypedCustomDataContextWithConcurrencyProvider<>).MakeGenericType(
                        typeof(TestEntityType16<>).MakeGenericType(typeData.ClrType));

                    using (ChangeScope.GetChangeScope(contextType))
                    {
                        bool first = true;
                        string etag = null;
                        foreach (object value in typeData.SampleValues)
                        {
                            // TODO: JsonPrimitiveTypesUtil doesn't properly translate datatime objects... need to update it for JsonLight.
                            string newPayload = payloadTemplate.Replace("XXXX", JsonPrimitiveTypesUtil.PrimitiveToString(value, typeData.ClrType));

                            // for the first time, the request will be a POST request with no If-Match header.
                            // subsequent requests will be PATCH request with If-Match headers
                            if (first)
                            {
                                etag = UnitTestsUtil.GetTestWebRequestInstance(
                                    UnitTestsUtil.JsonLightMimeType, "/Values", contextType, null, "POST", newPayload).ResponseETag;
                                first = false;
                            }
                            else
                            {
                                var headers = new KeyValuePair<string, string>[] {
                                    new KeyValuePair<string, string>("If-Match", etag) };
                                etag = UnitTestsUtil.GetTestWebRequestInstance(
                                    UnitTestsUtil.JsonLightMimeType, "/Values(1)", contextType, headers, "PATCH", newPayload).ResponseETag;
                            }
                        }
                    }
                }
            }

            [ETag("Concurrency")]
            public class TestEntityType16<T>
            {
                public int ID { get; set; }
                public T Concurrency { get; set; }
            }

            [TestMethod, Variation("Null ref in edm service with entity with rights=none and void service op with rights = all")]
            public void NullRefWithEntityRightsNoneAndVoidServiceOpRightsAll()
            {
                TestUtil.ClearMetadataCache();
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    ServiceModelData.Northwind.EnsureDependenciesAvailable();
                    request.ServiceType = typeof(TestNorthwindService);
                    request.RequestUriString = "/$metadata";
                    request.HttpMethod = "GET";
                    request.SendRequest();
                    string response = request.GetResponseStreamAsText();
                    Stream stream = IOUtil.CreateStream(response);
                    MetadataUtil.IsValidMetadata(stream, null);
                }
            }

            [System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
            public partial class TestNorthwindService : DataService<NorthwindModel.NorthwindContext>
            {
                public static void InitializeService(IDataServiceConfiguration configuration)
                {
                    configuration.SetEntitySetAccessRule("Customers", EntitySetRights.None);
                    configuration.SetServiceOperationAccessRule("ClearOperationList", ServiceOperationRights.All);

                    //Change to allow one to see verbose error messages
                    configuration.UseVerboseErrors = true;
                }

                [WebGet]
                [SingleResult]
                public int ClearOperationList()
                {
                    return 1;
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Author required on feed where there are no entries")]
            public void AtomAuthor()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.Accept = UnitTestsUtil.AtomFormat;
                    request.RequestUriString = "/EmptySet";
                    request.SendRequest();
                    UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), UnitTestsUtil.AtomFormat, "atom:feed/atom:author");

                    request.RequestUriString = "/Customers";
                    request.Accept = UnitTestsUtil.AtomFormat;
                    request.SendRequest();
                    UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), UnitTestsUtil.AtomFormat, "count(atom:feed/atom:author)=0", "atom:feed/atom:entry/atom:author");
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("[Server]Server uses ETag value from payload on PUT response")]
            public void ETagInPutResponse()
            {
                using (var conn = ocs.PopulateData.CreateTableAndPopulateData())
                {
                    var request = UnitTestsUtil.GetTestWebRequestInstance(
                        UnitTestsUtil.JsonLightMimeType, "/Customers(1)", typeof(ocs.CustomObjectContext), new KeyValuePair<string, string>[0], "GET");
                    string etag = request.ResponseETag;
                    string responseBody = request.GetResponseStreamAsText();
                    request.Dispose();

                    string body = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
                        <entry xml:base=""http://host/"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
                          <id>http://host/Customers(1)</id>
                          <category term=""#AstoriaUnitTests.ObjectContextStubs.Types.CustomerWithBirthday"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
                          <content type=""application/xml"">
                            <m:properties>
                              <d:ID m:type=""Edm.Int32"">1</d:ID>
                              <d:Name>Customer 1</d:Name>
                              <d:Concurrency xml:space=""preserve"">1    </d:Concurrency>
                              <d:EditTimeStamp m:type=""Edm.Binary"">AAAAAAAACCA=</d:EditTimeStamp>
                              <d:GuidValue m:type=""Edm.Guid"" m:null=""true"" />
                              <d:Birthday m:type=""Edm.DateTimeOffset"">2009-06-11T17:37:03.153Z</d:Birthday>
                              <d:Address m:type=""AstoriaUnitTests.ObjectContextStubs.Types.Address"">
                                <d:StreetAddress m:null=""true"" />
                                <d:City m:null=""true"" />
                                <d:State m:null=""true"" />
                                <d:PostalCode m:null=""true"" />
                              </d:Address>
                            </m:properties>
                          </content>
                        </entry>";

                    string etagAfterPut = UnitTestsUtil.GetTestWebRequestInstance(
                        UnitTestsUtil.AtomFormat, "/Customers(1)", typeof(ocs.CustomObjectContext),
                        new KeyValuePair<string, string>[]{
                            new KeyValuePair<string,string>("If-Match", etag)
                        }, "PUT", body).ResponseETag;
                    request.Dispose();

                    string etagFinal = UnitTestsUtil.GetETagFromResponse(typeof(ocs.CustomObjectContext), "/Customers(1)", UnitTestsUtil.AtomFormat);
                    Assert.AreEqual(etagAfterPut, etagFinal);
                }
            }

            [TestMethod, Variation("Service operation with access rights None should not be visible in $metadata")]
            public void ServiceOperationWithAccessRightsNoneShouldNotDisplayInMetadata()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.EntitySetAccessRule.Restore())
                using (OpenWebDataServiceHelper.ServiceOperationAccessRule.Restore())
                {
                    XDocument metadata;

                    TestUtil.ClearConfiguration();
                    OpenWebDataServiceHelper.ServiceOperationAccessRule.Value = new Dictionary<string, ServiceOperationRights>()
                    {
                        {"IntServiceOperation", ServiceOperationRights.None},
                        {"InsertCustomer", ServiceOperationRights.None | ServiceOperationRights.OverrideEntitySetRights},
                        {"*",ServiceOperationRights.All}
                    };
                    OpenWebDataServiceHelper.EntitySetAccessRule.Value = new Dictionary<string, EntitySetRights>()
                    {
                        {"Orders", EntitySetRights.None},
                        {"*", EntitySetRights.All}
                    };

                    try
                    {
                        UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.MimeApplicationXml, "/$metadata", typeof(CustomRowBasedContext), null, "GET");
                        Assert.Fail("Exception expected but not catched.");
                    }
                    catch (Exception e)
                    {
                        Assert.IsInstanceOfType(e.InnerException, typeof(InvalidOperationException));
                        string errorMsg = "The operation 'GetOrderById' has the resource set 'Orders' that is not visible. The operation 'GetOrderById' should be made hidden or the resource set 'Orders' should be made visible.";
                        Assert.AreEqual(errorMsg, e.InnerException.Message);
                    }

                    TestUtil.ClearConfiguration();
                    OpenWebDataServiceHelper.ServiceOperationAccessRule.Value.Add("GetOrderById", ServiceOperationRights.None);
                    OpenWebDataServiceHelper.ServiceOperationAccessRule.Value.Add("GetAllOrdersQueryable", ServiceOperationRights.None);
                    OpenWebDataServiceHelper.ServiceOperationAccessRule.Value.Add("GetOrderByIdQueryable", ServiceOperationRights.None);
                    OpenWebDataServiceHelper.ServiceOperationAccessRule.Value.Add("GetAllOrdersEnumerable", ServiceOperationRights.None);
                    OpenWebDataServiceHelper.ServiceOperationAccessRule.Value.Add("GetOrderByIdDirectValue", ServiceOperationRights.None);

                    using (TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.MimeApplicationXml,
                        "/$metadata", typeof(CustomRowBasedContext), null, "GET"))
                    {
                        metadata = request.GetResponseStreamAsXDocument();
                    }

                    var functions = metadata.Descendants(UnitTestsUtil.EdmOasisNamespace + "FunctionImport").Select(f => f.Attribute("Name").Value);
                    var actions = metadata.Descendants(UnitTestsUtil.EdmOasisNamespace + "ActionImport").Select(f => f.Attribute("Name").Value);
                    Assert.IsFalse(functions.Contains("IntServiceOperation"), "This SO was explicitely hidden.");
                    Assert.AreEqual(HttpStatusCode.NotFound, GetRequestStatusCode("/IntServiceOperation", typeof(CustomRowBasedContext)));
                    Assert.IsFalse(functions.Contains("InsertCustomer"), "This SO should be hidden because it's explicitely hidden.");
                    Assert.AreEqual(HttpStatusCode.NotFound, GetRequestStatusCode("/InsertCustomer", typeof(CustomRowBasedContext)));
                    Assert.IsTrue(functions.Contains("GetCustomerByCity"), "This SO should be visible as nothing is hiding it.");
                    Assert.AreEqual(HttpStatusCode.OK, GetRequestStatusCode("/GetCustomerByCity", typeof(CustomRowBasedContext)));
                    Assert.IsFalse(functions.Contains("NonExistant"), "This SO was explicitely hidden.");
                    Assert.AreEqual(HttpStatusCode.NotFound, GetRequestStatusCode("/NonExistant", typeof(CustomRowBasedContext)));
                    Assert.IsTrue(actions.Contains("DoNothingOperation"), "This SO should not be affected and should be accessible.");
                    // RequestProcessor issue - to repro enable code comment and disable existing code, set break point where bug number is. NoContent is expected.
                    // Assert.AreEqual(HttpStatusCode.NoContent, GetRequestStatusCode("/DoNothingOperation", typeof(CustomRowBasedContext)));
                    Assert.AreEqual(HttpStatusCode.MethodNotAllowed, GetRequestStatusCode("/DoNothingOperation", typeof(CustomRowBasedContext)));
                    Assert.IsFalse(functions.Contains("GetOrderById"), "This SO should be hidden because its return type is hidden.");
                    Assert.AreEqual(HttpStatusCode.NotFound, GetRequestStatusCode("/GetOrderById", typeof(CustomRowBasedContext)));
                    Assert.IsTrue(functions.Contains("GetCustomerAddress"), "This SO should not be affected and should be accessible.");
                    Assert.AreEqual(HttpStatusCode.OK, GetRequestStatusCode("/GetCustomerAddress?id=1", typeof(CustomRowBasedContext)));

                    var entitySets = metadata.Descendants(UnitTestsUtil.EdmOasisNamespace + "EntitySet").Select(e => e.Attribute("Name").Value);
                    Assert.IsFalse(entitySets.Contains("Orders"), "This set was explicitely hidden.");
                    Assert.AreEqual(HttpStatusCode.NotFound, GetRequestStatusCode("/Orders", typeof(CustomRowBasedContext)));
                    Assert.IsTrue(entitySets.Contains("Customers"), "This set should not be affected and should be accessible.");
                    Assert.AreEqual(HttpStatusCode.OK, GetRequestStatusCode("/Customers", typeof(CustomRowBasedContext)));

                    var entityTypes = metadata.Descendants(UnitTestsUtil.EdmOasisNamespace + "EntityType").Select(e => e.Attribute("Name").Value);
                    Assert.IsTrue(entityTypes.Contains("Order"), "This type is visible because MemberOrders set is visible.");
                    Assert.IsTrue(entityTypes.Contains("Customer"), "This type should not be affected and should be accessible.");
                    Assert.IsTrue(entityTypes.Contains("CustomerWithBirthday"), "This type should not be affected and should be accessible.");

                    var complexTypes = metadata.Descendants(UnitTestsUtil.EdmOasisNamespace + "ComplexType").Select(e => e.Attribute("Name").Value);
                    Assert.IsTrue(complexTypes.Contains("Address"), "This complex type is used by a service operation so it should be visible.");

                    var navigationPropertiesOnCustomer =
                        metadata.Descendants(UnitTestsUtil.EdmOasisNamespace + "EntityType").
                            Where(e => e.Attribute("Name").Value == "Customer").
                            Elements(UnitTestsUtil.EdmOasisNamespace + "NavigationProperty").
                            Select(e => e.Attribute("Name").Value);
                    Assert.IsTrue(navigationPropertiesOnCustomer.Contains("BestFriend"), "This nav property is reference back to the visible Customer, so it should be visible.");
                    Assert.IsTrue(navigationPropertiesOnCustomer.Contains("Orders"), "MemberOrders set is visible while Orders set is hidden. Order type should still be visible.");

                    ///////////////////////////////
                    TestUtil.ClearConfiguration();
                    OpenWebDataServiceHelper.EntitySetAccessRule.Value.Add("MemberOrders", EntitySetRights.None);
                    using (TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.MimeApplicationXml, "/$metadata", typeof(CustomRowBasedContext), null, "GET"))
                    {
                        metadata = request.GetResponseStreamAsXDocument();
                    }

                    entityTypes = metadata.Descendants(UnitTestsUtil.EdmOasisNamespace + "EntityType").Select(e => e.Attribute("Name").Value);
                    Assert.IsFalse(entityTypes.Contains("Order"), "This type is hidden because its sets Orders and MemberOrders are hidden.");

                    navigationPropertiesOnCustomer =
                        metadata.Descendants(UnitTestsUtil.EdmOasisNamespace + "EntityType").
                            Where(e => e.Attribute("Name").Value == "Customer").
                            Elements(UnitTestsUtil.EdmOasisNamespace + "NavigationProperty").
                            Select(e => e.Attribute("Name").Value);
                    Assert.IsFalse(navigationPropertiesOnCustomer.Contains("Orders"), "This nav property points to hidden entity set, so it should be hidden as well.");

                    // Hide the GetCustomerAddress as well and the Address type should be hidden along with it
                    TestUtil.ClearConfiguration();
                    CustomRowBasedContext.PreserveChanges = false;
                    OpenWebDataServiceHelper.ServiceOperationAccessRule.Value = new Dictionary<string, ServiceOperationRights>()
                    {
                        {"IntServiceOperation", ServiceOperationRights.None},
                        {"GetCustomerByCity", ServiceOperationRights.None},
                        {"AddressServiceOperation", ServiceOperationRights.None},
                        {"InsertCustomer", ServiceOperationRights.None | ServiceOperationRights.OverrideEntitySetRights},
                        {"GetCustomerAddress", ServiceOperationRights.None | ServiceOperationRights.OverrideEntitySetRights},
                        {"GetAllCustomersQueryable", ServiceOperationRights.None | ServiceOperationRights.OverrideEntitySetRights},
                        {"GetCustomerByIdQueryable", ServiceOperationRights.None | ServiceOperationRights.OverrideEntitySetRights},
                        {"GetAllCustomersEnumerable", ServiceOperationRights.None | ServiceOperationRights.OverrideEntitySetRights},
                        {"GetCustomerByIdDirectValue", ServiceOperationRights.None | ServiceOperationRights.OverrideEntitySetRights},
                        {"*", ServiceOperationRights.All}
                    };
                    OpenWebDataServiceHelper.EntitySetAccessRule.Value = new Dictionary<string, EntitySetRights>()
                    {
                        {"Customers", EntitySetRights.None},
                        {"MemberCustomers", EntitySetRights.None},
                        {"*",EntitySetRights.All}
                    };

                    using (TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.MimeApplicationXml,
                        "/$metadata", typeof(CustomRowBasedContext), null, "GET"))
                    {
                        metadata = request.GetResponseStreamAsXDocument();
                    }

                    complexTypes = metadata.Descendants(UnitTestsUtil.EdmOasisNamespace + "ComplexType").Select(e => e.Attribute("Name").Value);
                    Assert.IsTrue(complexTypes.Contains("Address"), "This complex type is used Headquarter complex type.");
                    Assert.AreEqual(HttpStatusCode.NotFound, GetRequestStatusCode("/GetCustomerAddress", typeof(CustomRowBasedContext)));

                    TestUtil.ClearConfiguration();
                    OpenWebDataServiceHelper.EntitySetAccessRule.Value.Add("Regions", EntitySetRights.None);
                    OpenWebDataServiceHelper.EntitySetAccessRule.Value.Add("MemberRegions", EntitySetRights.None);
                    OpenWebDataServiceHelper.ServiceOperationAccessRule.Value.Add("GetRegionByName", ServiceOperationRights.None);
                    using (TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.MimeApplicationXml,
                        "/$metadata", typeof(CustomRowBasedContext), null, "GET"))
                    {
                        metadata = request.GetResponseStreamAsXDocument();
                    }

                    complexTypes = metadata.Descendants(UnitTestsUtil.EdmOasisNamespace + "ComplexType").Select(e => e.Attribute("Name").Value);
                    Assert.IsFalse(complexTypes.Contains("Address"), "This complex type is used only by hidden entity types and hidden SO, so it should be hidden as well.");
                    Assert.AreEqual(HttpStatusCode.NotFound, GetRequestStatusCode("/GetCustomerAddress", typeof(CustomRowBasedContext)));
                }
            }

            private HttpStatusCode GetRequestStatusCode(string requestUri, Type contextType)
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = contextType;
                    request.RequestUriString = requestUri;
                    request.HttpMethod = "GET";

                    Exception exception = TestUtil.RunCatching(request.SendRequest);
                    return (HttpStatusCode)request.ResponseStatusCode;
                }
            }

            [TestMethod, Variation("Disallow NullableTypes as key properties in IDSP")]
            public void DisallowNullableTypeAsKeyProperties()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(TestDataContext16);
                    request.RequestUriString = "/$metadata";
                    request.HttpMethod = "GET";

                    Exception exception = TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(exception, true);

                    while (exception.InnerException != null && !(exception.InnerException is ArgumentException))
                    {
                        exception = exception.InnerException;
                    }

                    Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));
                    Assert.AreEqual("Key Properties cannot be of nullable type. Please make sure the type of this property is not of Nullable<> type.", exception.InnerException.Message);
                }
            }

            public class TestDataContext16 : IServiceProvider
            {
                private p.IDataServiceMetadataProvider provider;
                public TestDataContext16()
                {
                    p.ResourceType resourceType = new p.ResourceType(
                        typeof(TestType16),
                        p.ResourceTypeKind.EntityType,
                        null,
                        typeof(TestType16).Namespace,
                        typeof(TestType16).Name,
                        false);

                    resourceType.AddProperty(new p.ResourceProperty("ID", p.ResourcePropertyKind.Key | p.ResourcePropertyKind.Primitive, p.ResourceType.GetPrimitiveResourceType(typeof(int?))));
                    resourceType.AddProperty(new p.ResourceProperty("Name", p.ResourcePropertyKind.Primitive, p.ResourceType.GetPrimitiveResourceType(typeof(string))));

                    List<p.ResourceSet> containers = new List<p.ResourceSet>() {
                        new p.ResourceSet("Data", resourceType) };
                    List<p.ResourceType> types = new List<p.ResourceType>() { resourceType };
                    List<p.ServiceOperation> operations = new List<p.ServiceOperation>(0);
                    List<p.ResourceAssociationSet> associationSets = new List<Microsoft.OData.Service.Providers.ResourceAssociationSet>(0);

                    this.provider = new CustomDataServiceProvider(containers, types, operations, associationSets, this);
                }

                IQueryable<TestType16> Data
                {
                    get { return new TestType16[0].AsQueryable(); }
                }

                #region IServiceProvider Members

                public object GetService(Type serviceType)
                {
                    if (serviceType == typeof(p.IDataServiceMetadataProvider) ||
                        serviceType == typeof(p.IDataServiceQueryProvider))
                    {
                        return this.provider;
                    }

                    return null;
                }

                #endregion

                public class TestType16
                {
                    public int? ID { get; set; }
                    public string Name { get; set; }
                }
            }

            [TestMethod, Variation("IDSP: need to cache initializeservice results between requests for that service in a given app domain")]
            public void TestCacheResultsOfInitializeService()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    TestUtil.ClearMetadataCache();
                    TestService17.InitializeCount = 0;
                    request.ServiceType = typeof(TestService17);
                    request.RequestUriString = "/Customers";
                    request.SendRequest();

                    Assert.AreEqual(1, TestService17.InitializeCount);

                    request.SendRequest();
                    request.SendRequest();

                    Assert.AreEqual(1, TestService17.InitializeCount);
                }
            }

            public class TestService17 : DataService<CustomRowBasedContext>, IServiceProvider
            {
                public static int InitializeCount = 0;

                public static void InitializeService(DataServiceConfiguration configuration)
                {
                    InitializeCount++;
                    configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
                }

                #region IServiceProvider Members

                public object GetService(Type serviceType)
                {
                    IServiceProvider isp = CustomRowBasedContext.GetInstance() as IServiceProvider;
                    if (isp != null)
                    {
                        return isp.GetService(serviceType);
                    }

                    return null;
                }

                #endregion
            }

            [TestMethod, Variation("No error when specifying 2 entity sets with the same name when using IDSP")]
            public void TwoEntitySetsCanHaveSameNameWhenUsingIDSP()
            {
                var testCases = new[]
                    {
                        new {
                            NumberOfTypes = 2, NumberOfSets = 1, NumberOfServiceOperations = 1,
                            ExceptionMessage = "More than one resource type with the name 'AstoriaUnitTests.Tests.EntityTypeA' was found. Resource type names must be unique."
                        },
                        new {
                            NumberOfTypes = 1, NumberOfSets = 2, NumberOfServiceOperations = 1,
                            ExceptionMessage = "More than one entity set with the name 'Entities' was found. Entity set names must be unique."
                        },
                        new {
                            NumberOfTypes = 1, NumberOfSets = 1, NumberOfServiceOperations = 2,
                            ExceptionMessage = "More than one service operation with the name 'ServiceOp' was found. Service operation names must be unique."
                        }

                    };

                using (OpenWebDataServiceHelper.EnableAccess.Restore())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    OpenWebDataServiceHelper.EnableAccess.Value = new List<string>() { "*" };
                    request.DataServiceType = typeof(TestDataContext17);
                    request.RequestUriString = "/$metadata";
                    request.HttpMethod = "GET";

                    foreach (var testCase in testCases)
                    {
                        TestDataContext17.NumberOfTypes = testCase.NumberOfTypes;
                        TestDataContext17.NumberOfSets = testCase.NumberOfSets;
                        TestDataContext17.NumberOfServiceOperations = testCase.NumberOfServiceOperations;

                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNotNull(exception, "Exception expected but not thrown.");
                        Assert.IsInstanceOfType(exception.InnerException, typeof(DataServiceException));
                        Assert.AreEqual(exception.InnerException.Message, testCase.ExceptionMessage);
                    }
                }
            }

            public class TestDataContext17 : IServiceProvider
            {
                private p.IDataServiceMetadataProvider provider;

                public static int NumberOfTypes = 1;
                public static int NumberOfSets = 1;
                public static int NumberOfServiceOperations = 1;

                public TestDataContext17()
                {
                    p.ResourceType entityTypeA = new p.ResourceType(
                        typeof(EntityTypeA),
                        p.ResourceTypeKind.EntityType,
                        null,
                        typeof(EntityTypeA).Namespace,
                        typeof(EntityTypeA).Name,
                        false);

                    entityTypeA.AddProperty(new p.ResourceProperty("ID", p.ResourcePropertyKind.Key | p.ResourcePropertyKind.Primitive, p.ResourceType.GetPrimitiveResourceType(typeof(int))));
                    entityTypeA.AddProperty(new p.ResourceProperty("Name", p.ResourcePropertyKind.Primitive, p.ResourceType.GetPrimitiveResourceType(typeof(string))));

                    List<p.ResourceType> types = new List<p.ResourceType>();
                    for (int i = 0; i < NumberOfTypes; i++)
                    {
                        types.Add(entityTypeA);
                    }

                    List<p.ResourceSet> containers = new List<p.ResourceSet>();
                    for (int i = 0; i < NumberOfSets; i++)
                    {
                        containers.Add(new p.ResourceSet("Entities", entityTypeA));
                    };

                    List<p.ServiceOperation> serviceOperations = new List<p.ServiceOperation>();
                    for (int i = 0; i < NumberOfServiceOperations; i++)
                    {
                        serviceOperations.Add(new p.ServiceOperation("ServiceOp", p.ServiceOperationResultKind.Void,
                        null, null, "POST", new List<p.ServiceOperationParameter>(0)));
                    }


                    this.provider = new CustomDataServiceProvider(containers, types, serviceOperations, new List<p.ResourceAssociationSet>(0), this);
                }

                #region IServiceProvider Members

                public object GetService(Type serviceType)
                {
                    if (serviceType == typeof(p.IDataServiceMetadataProvider) ||
                        serviceType == typeof(p.IDataServiceQueryProvider))
                    {
                        return this.provider;
                    }

                    return null;
                }

                #endregion

                public class EntityTypeA
                {
                    public int ID { get; set; }
                    public string Name { get; set; }
                }
            }

            [TestMethod, Variation("Disallow resource types without public properties.")]
            public void EmptyComplexTypesNotSupported()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.ServiceType = typeof(TestService18);
                    request.RequestUriString = "/$metadata";
                    WebException metadataDocumentException = TestUtil.RunCatching<WebException>(() => request.SendRequest());

                    Assert.AreEqual(WebExceptionStatus.ProtocolError, metadataDocumentException.Status);
                    Assert.IsTrue(request.ErrorResponseContent.Contains(DataServicesResourceUtil.GetString("ReflectionProvider_ResourceTypeHasNoPublicallyVisibleProperties", typeof(AstoriaUnitTests.Stubs.EmptyComplexTypesNotSupported.EmptyComplexType).FullName)), "Error message is invalid");
                }
            }

            [TestMethod, Variation("ObjectContextServiceProvider fails with NullReferenceException when an entity property is not public")]
            public void NullReferenceOnNonPublicEntityProperty()
            {
                // ocs.PopulateData.EntityConnection = null;
                // using (EntityConnection connection = ocs.PopulateData.CreateTableAndPopulateData())
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                    {
                        request.ServiceType = typeof(TestService19);
                        request.RequestUriString = "/";

                        WebException serviceDocumentException = TestUtil.RunCatching<WebException>(() => request.SendRequest());

                        Assert.AreEqual(WebExceptionStatus.ProtocolError, serviceDocumentException.Status);
                        Assert.IsTrue(request.ErrorResponseContent.Contains(DataServicesResourceUtil.GetString("ObjectContext_PublicPropertyNotDefinedOnType", "Customer", "Name")), "Error message is invalid");
                    }
                }
            }

            public class TestService20 : DataService<ocs.CustomObjectContext>
            {
                public static void InitializeService(DataServiceConfiguration configuration)
                {
                    configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
                    configuration.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                    configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                }

                [WebGet]
                public IQueryable<ocs.Hidden.Customer> GetCustomers()
                {
                    List<ocs.Hidden.Customer> custs = new List<ocs.Hidden.Customer>();
                    custs.Add(new ocs.Hidden.Customer()
                    {
                        Name = "Customer 0",
                        ID = 0,
                        EditTimeStamp = new byte[] { 1, 3 }
                    });
                    custs[0].Orders.Add(new AstoriaUnitTests.ObjectContextStubs.Hidden.Order());

                    return custs.AsQueryable();
                }
            }

            public class TestService18 : DataService<AstoriaUnitTests.Stubs.EmptyComplexTypesNotSupported.EmptyComplexTypesContext>
            {
                public static void InitializeService(DataServiceConfiguration configuration)
                {
                    configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
                    configuration.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                    configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                }
            }

            public class TestService19 : DataService<AstoriaUnitTests.CompoundKeyContext.NonPublicPropertiesContext.NonPublicPropertiesContext>
            {
                public static void InitializeService(DataServiceConfiguration configuration)
                {
                    configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
                    configuration.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                    configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                }

                protected override CompoundKeyContext.NonPublicPropertiesContext.NonPublicPropertiesContext CreateDataSource()
                {
                    string connectionString = "metadata=" + TestUtil.ServerUnitTestSamples + "\\stubs\\NonPublicPropertiesContext\\;provider=System.Data.SqlClient;provider connection string=\"server=.\\sqlexpress;integrated security=true;\";";
                    return new CompoundKeyContext.NonPublicPropertiesContext.NonPublicPropertiesContext(new EntityConnection(connectionString));
                }
            }

            [HasStream]
            public class TestEntity21
            {
                public int ID { get; set; }
            }

            public class TestContext21
            {
                public IQueryable<TestEntity21> Value
                {
                    get
                    {
                        return new TestEntity21[]
                        {
                            new TestEntity21(){ID=0},
                            new TestEntity21(){ID=1}
                        }.AsQueryable();
                    }
                }

                public IQueryable<TestEntityType11> NonMediaTypeSet
                {
                    get
                    {
                        return new TestEntityType11[]
                        {
                            new TestEntityType11() {ID=0},
                            new TestEntityType11(){ID=1}
                        }.AsQueryable();
                    }
                }
            }

            [TestMethod, Variation("[BLOB][Server]Missing interface should not cause mid-stream error")]
            public void StreamWithMissingInterface()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    VerifyStreamWithMissingInterface(request, typeof(TestContext21), "/$metadata");
                    VerifyStreamWithMissingInterface(request, typeof(TestContext21), "/Value");
                }
            }

            private void VerifyStreamWithMissingInterface(TestWebRequest request, Type contextType, string requestUriString)
            {
                request.DataServiceType = contextType;
                request.RequestUriString = requestUriString;
                Exception e = TestUtil.RunCatching(request.SendRequest);
                Assert.IsNotNull(e);
                Assert.AreEqual(500, request.ResponseStatusCode);
                Assert.AreEqual(typeof(DataServiceException), e.InnerException.GetType());
                Assert.AreEqual("To support streaming, the data service must implement IServiceProvider.GetService() to return an implementation of IDataServiceStreamProvider or the data source must implement IDataServiceStreamProvider.", e.InnerException.Message);
            }

            [TestMethod, Variation("[BLOB][Server]Missing interface should not cause mid-stream error")]
            public void StreamWithMissingInterface_EF()
            {
                using (TestUtil.RestoreStaticValueOnDispose(typeof(BaseTestWebRequest), "HostInterfaceType"))
                using (TestUtil.RestoreStaticValueOnDispose(typeof(NorthwindDefaultStreamService), "GetServiceOverride"))
                using (NorthwindDefaultStreamService.SetupNorthwindWithStream(
                    new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("Customers", "true") },
                    "RegressionTest_StreamWithMissingInterface_EF"))
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    BaseTestWebRequest.HostInterfaceType = typeof(IDataServiceHost2);
                    NorthwindDefaultStreamService.GetServiceOverride = (type) => null;
                    VerifyStreamWithMissingInterface(request, typeof(NorthwindDefaultStreamService), "/$metadata");
                }
            }

            public class TestService22 : DataService<AstoriaUnitTests.Stubs.VirtualPropertiesAreSupported.VirtualPropertyContext>
            {
                public static void InitializeService(DataServiceConfiguration configuration)
                {
                    configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
                    configuration.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                    configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                }
            }


            [TestMethod, Variation("Datasource with virtual property will throw System.Security.VerificationException")]
            public void DataSourceWithVirtualProperty()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.ServiceType = typeof(TestService22);
                    request.RequestUriString = "/VirtualProperty";
                    Exception e = TestUtil.RunCatching(request.SendRequest);
                    Assert.IsNull(e);
                    Assert.AreEqual(200, request.ResponseStatusCode);
                }
            }

            public class TestService21 : DataService<TestContext21>, IServiceProvider
            {
                public static int StreamProviderInstanceCount = 0;

                public static void InitializeService(DataServiceConfiguration config)
                {
                    config.SetEntitySetAccessRule("*", EntitySetRights.All);
                }

                #region IServiceProvider Members

                public object GetService(Type serviceType)
                {
                    if (serviceType == typeof(p.IDataServiceStreamProvider))
                    {
                        StreamProviderInstanceCount++;
                        return new DataServiceStreamProvider();
                    }

                    return null;
                }

                #endregion
            }

            [TestMethod, Variation("IDSSP should not be created before DataService.provider is assigned")]
            public void StreamProviderCreatedBeforeProvider()
            {
                using (TestUtil.RestoreStaticValueOnDispose(typeof(TestService21), "StreamProviderInstanceCount"))
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    TestService21.StreamProviderInstanceCount = 0;
                    request.ServiceType = typeof(TestService21);
                    request.RequestUriString = "/";
                    request.SendRequest();

                    Assert.AreEqual(0, TestService21.StreamProviderInstanceCount);

                    request.RequestUriString = "/NonMediaTypeSet";
                    request.SendRequest();
                    Assert.AreEqual(0, TestService21.StreamProviderInstanceCount);
                }
            }

            [TestMethod, Variation("OData-Version not specified when an explicit DataServiceException is thrown from the server - side")]
            public void DataServiceExceptionThrownByServerShouldIncludeODataVersion()
            {
                TestUtil.ClearMetadataCache();
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    try
                    {
                        request.RequestUriString = "/$metadata";
                        request.ServiceType = typeof(TestService23);
                        request.SendRequest();
                        string s = request.GetResponseStreamAsText();
                        Trace.WriteLine(s);
                    }
                    catch (WebException ex)
                    {
                        HttpWebResponse webResp = (HttpWebResponse)ex.Response;
                        Assert.AreEqual(HttpStatusCode.InternalServerError, webResp.StatusCode);
                        Assert.IsNotNull(webResp.Headers["OData-Version"], "OData-Version header does not exist or has null value");
                        using (StreamReader sr = new StreamReader(webResp.GetResponseStream()))
                        {
                            string respBody = sr.ReadToEnd();
                            Assert.IsTrue(respBody.Contains("OData-Version header test"));
                        }
                    }
                }
            }

            [System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
            public class TestService23 : DataService<CustomDataContext>
            {
                public static void InitializeService(DataServiceConfiguration config)
                {
                    config.UseVerboseErrors = true;
                    throw new DataServiceException("OData-Version header test");
                }
            }

            [TestMethod, Variation("Null Reference exception when invalid content type is specified for open properties")]
            public void SpecifyInvalidContentTypeForOpenPropertiesShouldThrow()
            {
                string payload = "112.23";

                CustomRowBasedOpenTypesContext.ClearData();
                using (CustomRowBasedOpenTypesContext.CreateChangeScope())
                {
                    try
                    {
                        UnitTestsUtil.UpdateAndVerifyPrimitiveProperty(typeof(CustomRowBasedOpenTypesContext), "PUT", "/Orders(0)/DollarAmount/$value", payload, UnitTestsUtil.JsonLightMimeType, null);
                        Assert.Fail("Expected an exception, but no exception was thrown");
                    }
                    catch (Exception e)
                    {
                        DataServiceException ex = e.InnerException as DataServiceException;
                        Assert.IsTrue(ex != null && ex.StatusCode == 415, "Expecting status code 415");
                    }
                }
            }

            [TestMethod, Variation("Calling service operations returning collection of primitives/complex types threw a null reference exception when empty parenthesis was specified at the service operation segment")]
            public void ShouldNotThrowWhenAddEmptyParenthesisAfterServiceOperationReturningCollectionOfNonEntities()
            {
                using (MyServiceOperationDS.SetServiceOperationAccessRule("SOpMultipleQueryable", ServiceOperationRights.All | ServiceOperationRights.OverrideEntitySetRights))
                {
                    UnitTestsUtil.GetResponseStream(WebServerLocation.InProcess, UnitTestsUtil.MimeAny, "/SOpMultipleQueryable()", typeof(MyServiceOperationDS));
                }
            }

            public class CustomDataContextWithDispose : CustomDataContext, IDisposable
            {
                public static int DisposedCount;

                #region IDisposable Members

                void IDisposable.Dispose()
                {
                    DisposedCount++;
                }

                #endregion
            }

            [TestMethod, Variation("For WCF host, making sure IDSP.DisposeDataSource is called for operations that returns 204 or 304, i.e. updates or deletes")]
            public void IDspDisposeDataSourceShouldBeCalledForOperationsReturn204Or304()
            {
                CustomDataContext.ClearData();
                using (TestUtil.RestoreStaticValueOnDispose(typeof(CustomDataContextWithDispose), "DisposedCount"))
                using (CustomDataContext.CreateChangeScope())
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    Assert.AreEqual(0, CustomDataContextWithDispose.DisposedCount);
                    request.DataServiceType = typeof(CustomDataContextWithDispose);

                    request.HttpMethod = "GET";
                    request.RequestUriString = "/Customers(1)";
                    request.SendRequest();
                    Assert.AreEqual(200, request.ResponseStatusCode);
                    Assert.AreEqual(1, CustomDataContextWithDispose.DisposedCount);
                    Assert.IsTrue(!string.IsNullOrEmpty(request.ResponseETag), "CustomerWithBirthday should have etag properties.");

                    // GET with 304 response
                    request.HttpMethod = "GET";
                    request.RequestUriString = "/Customers(1)";
                    request.IfNoneMatch = request.ResponseETag;
                    TestUtil.RunCatching(request.SendRequest);
                    Assert.AreEqual(304, request.ResponseStatusCode);
                    Assert.IsTrue(string.IsNullOrEmpty(request.GetResponseStreamAsText()));
                    Assert.AreEqual(2, CustomDataContextWithDispose.DisposedCount);
                    Assert.IsTrue(!string.IsNullOrEmpty(request.ResponseETag), "CustomerWithBirthday should have etag properties.");

                    // PUT
                    request.HttpMethod = "PUT";
                    request.RequestUriString = "/Customers(1)/Name/$value";
                    request.IfMatch = request.ResponseETag;
                    request.IfNoneMatch = null;
                    request.RequestContentType = UnitTestsUtil.MimeTextPlain;
                    request.SetRequestStreamAsText("Bob");
                    request.SendRequest();
                    Assert.AreEqual(204, request.ResponseStatusCode);
                    // Since both IDSP and updateprovider dispose method gets called, the count increases by 2
                    Assert.AreEqual(4, CustomDataContextWithDispose.DisposedCount);
                    Assert.IsTrue(!string.IsNullOrEmpty(request.ResponseETag), "CustomerWithBirthday should have etag properties.");

                    // PATCH
                    request.HttpMethod = "PATCH";
                    request.RequestUriString = "/Customers(1)/Name/$value";
                    request.IfMatch = request.ResponseETag;
                    request.RequestContentType = UnitTestsUtil.MimeTextPlain;
                    request.SetRequestStreamAsText("Bob");
                    request.SendRequest();
                    Assert.AreEqual(204, request.ResponseStatusCode);
                    // Since both IDSP and updateprovider dispose method gets called, the count increases by 2
                    Assert.AreEqual(6, CustomDataContextWithDispose.DisposedCount);
                    Assert.IsTrue(!string.IsNullOrEmpty(request.ResponseETag), "CustomerWithBirthday should have etag properties.");

                    // DELETE
                    request.HttpMethod = "DELETE";
                    request.RequestUriString = "/Customers(1)/Name/$value";
                    request.IfMatch = request.ResponseETag;
                    request.SendRequest();
                    Assert.AreEqual(204, request.ResponseStatusCode);
                    // Since both IDSP and updateprovider dispose method gets called, the count increases by 2
                    Assert.AreEqual(8, CustomDataContextWithDispose.DisposedCount);
                    Assert.IsTrue(string.IsNullOrEmpty(request.ResponseETag));
                }
            }

            public class TypeA
            {
                public TypeA()
                {
                    this.typeName = "T.TypeA";
                }

                private string typeName;
                public string TypeName { get { return this.typeName; } }
                public int ID { get; set; }
            }

            public class TypeB
            {
                public TypeB()
                {
                    this.typeName = "T.TypeB";
                }

                private string typeName;
                public string TypeName { get { return this.typeName; } }
                public int ID { get; set; }
                public TypeA A { get; set; }
            }

            public class MyProvider : IServiceProvider
            {
                List<TypeA> a;
                List<TypeB> b;
                p.IDataServiceMetadataProvider provider;

                public MyProvider()
                {
                    a = new List<TypeA>() { new TypeA() { ID = 0 } };
                    b = new List<TypeB>() { new TypeB() { ID = 1, A = a[0] } };

                    List<p.ResourceSet> sets = new List<p.ResourceSet>();
                    List<p.ResourceType> types = new List<p.ResourceType>();
                    List<p.ResourceProperty> properties = new List<p.ResourceProperty>();

                    p.ResourceType typea = new p.ResourceType(typeof(TypeA), p.ResourceTypeKind.EntityType, null, "T", "TypeA", false);
                    typea.CanReflectOnInstanceType = false;
                    p.ResourceType typeb = new p.ResourceType(typeof(TypeB), p.ResourceTypeKind.EntityType, null, "T", "TypeB", false);
                    typeb.CanReflectOnInstanceType = false;

                    p.ResourceProperty key = new p.ResourceProperty("ID", p.ResourcePropertyKind.Primitive | p.ResourcePropertyKind.Key, p.ResourceType.GetPrimitiveResourceType(typeof(int)));
                    typea.AddProperty(key);
                    typeb.AddProperty(key);

                    p.ResourceProperty navprop = new p.ResourceProperty("A", p.ResourcePropertyKind.ResourceReference, typea);
                    typeb.AddProperty(navprop);

                    types.Add(typea);
                    types.Add(typeb);

                    properties.Add(key);
                    properties.Add(navprop);

                    p.ResourceSet seta = new p.ResourceSet("As", typea);
                    p.ResourceSet setb = new p.ResourceSet("Bset", typeb);
                    sets.Add(seta);
                    sets.Add(setb);

                    List<p.ResourceAssociationSet> associationSets = new List<p.ResourceAssociationSet>();

                    p.ResourceAssociationSet customer_BestFriend =
                        new Microsoft.OData.Service.Providers.ResourceAssociationSet(
                            "Bset_A",
                            new p.ResourceAssociationSetEnd(setb, typeb, navprop),
                            new p.ResourceAssociationSetEnd(seta, typea, null));
                    associationSets.Add(customer_BestFriend);

                    provider = new CustomDataServiceProvider(sets, types, new List<p.ServiceOperation>(), associationSets, this);
                }

                public IQueryable<TypeA> As
                {
                    get { return AstoriaUnitTests.Tests.UnitTestsUtil.GetQueryable(a.AsQueryable(), (p.IDataServiceQueryProvider)provider, "ResourceTypeName"); }
                }

                public IQueryable<TypeB> Bset
                {
                    get { return AstoriaUnitTests.Tests.UnitTestsUtil.GetQueryable(b.AsQueryable(), (p.IDataServiceQueryProvider)provider, "ResourceTypeName"); }
                }

                public object GetService(Type serviceType)
                {
                    if (serviceType == typeof(p.IDataServiceMetadataProvider) ||
                        serviceType == typeof(p.IDataServiceQueryProvider))
                    {
                        return provider;
                    }

                    return null;
                }
            }

            // [TestMethod, Variation("Server IDSP: Trying to access a property on a wrong type")]
            public void TestAs()
            {
                var response = UnitTestsUtil.GetResponseStream(WebServerLocation.InProcess, UnitTestsUtil.AtomFormat, "/Bset?$filter=ID eq A/ID", typeof(MyProvider));
            }

            public class TestDataService24 : DataService<NorthwindModel.NorthwindContext>
            {
                public static void InitializeService(IDataServiceConfiguration configuration)
                {
                    configuration.SetEntitySetAccessRule("Customers", EntitySetRights.AllWrite);
                    configuration.SetEntitySetAccessRule("Orders", EntitySetRights.AllRead);
                }

                [QueryInterceptor("Customers")]
                public Expression<Func<NorthwindModel.Customers, bool>> QueryOrders()
                {
                    throw new DataServiceException(500, "QueryInterceptor shouldn't be called!");
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Enforce EntitySetRights.")]
            public void EntitySetRightsForNestedQueries()
            {
                ServiceModelData.Northwind.EnsureDependenciesAvailable();

                UnitTestsUtil.VerifyInvalidRequest(null, "/Customers('ALKFI')/Orders", typeof(TestDataService24), UnitTestsUtil.AtomFormat, "GET", 403);
                UnitTestsUtil.VerifyInvalidRequest(null, "/Orders?$filter=Customers/ContactName eq 'ALFKI'", typeof(TestDataService24), UnitTestsUtil.AtomFormat, "GET", 403);
                UnitTestsUtil.VerifyInvalidRequest(null, "/Orders?$orderby=Customers/CustomerID", typeof(TestDataService24), UnitTestsUtil.AtomFormat, "GET", 403);
                UnitTestsUtil.VerifyInvalidRequest(null, "/Orders?$expand=Customers", typeof(TestDataService24), UnitTestsUtil.AtomFormat, "GET", 403);
            }

            public class TestDataService25 : DataService<NorthwindModel.NorthwindContext>
            {
                public static void InitializeService(IDataServiceConfiguration configuration)
                {
                    configuration.SetEntitySetAccessRule("Customers", EntitySetRights.All);
                    configuration.SetEntitySetAccessRule("Orders", EntitySetRights.None);
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Hidden Nav props do not show error during expand in IDSP/Mest")]
            public void ExpandOnHiddenNavPropShouldThrow()
            {
                ServiceModelData.Northwind.EnsureDependenciesAvailable();
                UnitTestsUtil.VerifyInvalidRequest(null, "/Customers?$expand=Orders", typeof(TestDataService25), UnitTestsUtil.AtomFormat, "GET", 400);
            }

            [ETag("ETagSingle1", "ETagSingle2", "ETagDouble1", "ETagDouble2")]
            public class FloatPointETagEntity
            {
                public int ID { get; set; }
                public Single ETagSingle1 { get; set; }
                public Single ETagSingle2 { get; set; }
                public Double ETagDouble1 { get; set; }
                public Double ETagDouble2 { get; set; }
            }

            public class FloatPointETagContext : Microsoft.OData.Service.Providers.IDataServiceUpdateProvider
            {
                static FloatPointETagEntity theEntity = null;
                public static int ConcurrencyCallCount = 0;

                public static void ResetResources()
                {
                    theEntity = new FloatPointETagEntity()
                    {
                        ID = 0,
                        ETagSingle1 = Single.NegativeInfinity,
                        ETagSingle2 = 0f,
                        ETagDouble1 = Double.NegativeInfinity,
                        ETagDouble2 = 0f
                    };
                    ConcurrencyCallCount = 0;
                }

                public IQueryable<FloatPointETagEntity> Entities
                {
                    get
                    {
                        return new FloatPointETagEntity[] { theEntity }.AsQueryable();
                    }
                }

                #region IUpdatable Members

                public object GetResource(IQueryable query, string fullTypeName)
                {
                    return "Resource";
                }

                public object ResetResource(object resource)
                {
                    return resource;
                }

                public void SetValue(object targetResource, string propertyName, object propertyValue)
                {
                    switch (propertyName)
                    {
                        case "ID": theEntity.ID = (int)propertyValue; break;
                        case "ETagSingle1": theEntity.ETagSingle1 = (Single)propertyValue; break;
                        case "ETagSingle2": theEntity.ETagSingle2 = (Single)propertyValue; break;
                        case "ETagDouble1": theEntity.ETagDouble1 = (Double)propertyValue; break;
                        case "ETagDouble2": theEntity.ETagDouble2 = (Double)propertyValue; break;
                    }
                }

                public object GetValue(object targetResource, string propertyName)
                {
                    switch (propertyName)
                    {
                        case "ID": return theEntity.ID;
                        case "ETagSingle1": return theEntity.ETagSingle1;
                        case "ETagSingle2": return theEntity.ETagSingle2;
                        case "ETagDouble1": return theEntity.ETagDouble1;
                        case "ETagDouble2": return theEntity.ETagDouble2;
                    }
                    return null;
                }

                public void SaveChanges()
                {
                }

                public object ResolveResource(object resource)
                {
                    return theEntity;
                }

                public object CreateResource(string containerName, string fullTypeName)
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

                public void ClearChanges()
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IConcurrencyProvider Members

                public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
                {
                    ConcurrencyCallCount++;
                }

                #endregion
            }

            [TestMethod, Variation("Single ETag with IConcurrencyProvider")]
            public void SingleTypeETagWithProvider()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("SingleValue1", new Single[] { Single.NegativeInfinity, Single.PositiveInfinity, Single.NaN }),
                    new Dimension("SingleValue2", new Single[] { Single.NegativeInfinity, Single.PositiveInfinity, Single.NaN }),
                    new Dimension("DoubleValue1", new Double[] { Double.NegativeInfinity, Double.PositiveInfinity, Double.NaN }),
                    new Dimension("DoubleValue2", new Double[] { Double.NegativeInfinity, Double.PositiveInfinity, Double.NaN }));

                FloatPointETagContext.ResetResources();
                TestUtil.RunCombinatorialEngineFail(engine, values =>
                {
                    var request = UnitTestsUtil.GetTestWebRequestInstance(
                        UnitTestsUtil.JsonLightMimeType, "/Entities(0)", typeof(FloatPointETagContext), new KeyValuePair<string, string>[0], "GET");

                    var etag = request.ResponseETag;

                    string payload = String.Format("{{\"ID\": 0, \"ETagSingle1\": \"{0}\", \"ETagSingle2\": \"{1}\", \"ETagDouble1\": \"{2}\", \"ETagDouble2\": \"{3}\"}}",
                        ((float)values["SingleValue1"]).ToString(CultureInfo.InvariantCulture),
                        ((float)values["SingleValue2"]).ToString(CultureInfo.InvariantCulture),
                        ((double)values["DoubleValue1"]).ToString(CultureInfo.InvariantCulture),
                        ((double)values["DoubleValue2"]).ToString(CultureInfo.InvariantCulture));

                    UnitTestsUtil.GetTestWebRequestInstance(
                        UnitTestsUtil.JsonLightMimeType, "/Entities(0)", typeof(FloatPointETagContext), new KeyValuePair<string, string>[] { new KeyValuePair<String, String>("If-Match", etag) }, "PUT",
                        payload);
                });
            }

            public class TestDataService26 : OpenWebDataService<CustomRowBasedContext>
            {
                public static int CustomersQueryInterceptorInvokeCount;
                public static int OrdersQueryInterceptorInvokeCount;
                public static bool QueryCustomersInjectsBrokenExpression;

                [QueryInterceptor("Customers")]
                public Expression<Func<RowEntityTypeWithIDAsKey, bool>> QueryCustomers()
                {
                    CustomersQueryInterceptorInvokeCount++;

                    // if the returned expression is called, we'll get a null ref exception. we make sure it's not called.
                    if (QueryCustomersInjectsBrokenExpression)
                    {
                        return c => ((RowEntityTypeWithIDAsKey)null).ID == 1;
                    }
                    else
                    {
                        return c => true;
                    }
                }

                [QueryInterceptor("Orders")]
                public Expression<Func<RowEntityTypeWithIDAsKey, bool>> QueryOrders()
                {
                    OrdersQueryInterceptorInvokeCount++;

                    // if the returned expression is called, we'll get a null ref exception. we make sure it's not called.
                    return c => ((RowEntityTypeWithIDAsKey)null).ID == 1;
                }
            }

            [TestMethod, Variation("Call order: On insert, GetQueryRoot and query interceptors are called, but no query is ever executed")]
            public void DoNotMakeUnnecessaryCallToGetQueryRoot()
            {
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(CustomDataServiceProvider)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(TestDataService26)))
                using (CustomRowBasedContext.CreateChangeScope())
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        string payload =
                            "{@odata.type: \"" + typeof(Customer).FullName + "\", Name: \"New Customer\", ID: 2000 }";

                        TestDataService26.QueryCustomersInjectsBrokenExpression = true;
                        request.ServiceType = typeof(TestDataService26);
                        request.HttpMethod = "POST";
                        request.RequestContentType = SerializationFormatData.JsonLight.MimeTypes[0] + ";charset=iso-8859-1";
                        request.RequestUriString = "/Customers";
                        request.SetRequestStreamAsText(payload);
                        request.SendRequest();
                        string responseText = request.GetResponseStreamAsText();
                        ////Trace.WriteLine(request.GetResponseStreamAsText());

                        Assert.AreEqual(0, CustomDataServiceProvider.GetQueryRootForResourceSetInvokeCount, "CustomDataServiceProvider.GetQueryRootForResourceSetInvokeCount");
                        Assert.AreEqual(0, TestDataService26.CustomersQueryInterceptorInvokeCount, "TestDataService26.CustomersQueryInterceptorInvokeCount");
                    }

                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        // One more time, with a deeper payload.
                        string payload = "{ @odata.type: \"" + typeof(Order).FullName + "\", DollarAmount: 100, ID: 2000 }";

                        TestDataService26.QueryCustomersInjectsBrokenExpression = false;
                        request.ServiceType = typeof(TestDataService26);
                        request.HttpMethod = "POST";
                        request.RequestContentType = SerializationFormatData.JsonLight.MimeTypes[0] + ";charset=iso-8859-1";
                        request.RequestUriString = "/Customers(2000)/Orders";
                        request.SetRequestStreamAsText(payload);
                        request.SendRequest();
                        string responseText = request.GetResponseStreamAsText();
                        ////Trace.WriteLine(request.GetResponseStreamAsText());

                        Assert.AreEqual(1, CustomDataServiceProvider.GetQueryRootForResourceSetInvokeCount, "CustomDataServiceProvider.GetQueryRootForResourceSetInvokeCount");
                        Assert.AreEqual(1, TestDataService26.CustomersQueryInterceptorInvokeCount, "TestDataService26.CustomersQueryInterceptorInvokeCount");
                        Assert.AreEqual(0, TestDataService26.OrdersQueryInterceptorInvokeCount, "TestDataService26.CustomersQueryInterceptorInvokeCount");
                    }
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Call order: On insert, Query interceptors called twice for PUT to link")]
            public void DoNotMakeUnnecessaryCallToQueryInterceptor()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(new Dimension("RequestType", new[] { "PUT", "PATCH" }));
                TestUtil.RunCombinatorialEngineFail(engine, (ht) =>
                {
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(CustomDataServiceProvider)))
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(TestDataService26)))
                    using (CustomRowBasedContext.CreateChangeScope())
                    {
                        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                        {
                            string payload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                                             "<adsm:ref xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\" id=\"/Customers(1)\"/>";

                            TestDataService26.QueryCustomersInjectsBrokenExpression = false;
                            request.ServiceType = typeof(TestDataService26);
                            request.HttpMethod = (string)ht["RequestType"];
                            request.RequestContentType = UnitTestsUtil.MimeApplicationXml;
                            request.RequestUriString = "/Customers(1)/BestFriend/$ref";
                            request.SetRequestStreamAsText(payload);
                            request.SendRequest();
                            Assert.AreEqual(204, request.ResponseStatusCode);
                            Assert.AreEqual(2, TestDataService26.CustomersQueryInterceptorInvokeCount, "TestDataService26.CustomersQueryInterceptorInvokeCount");
                        }
                    }
                });
            }

            [TestMethod, Variation("GET request to a specific property causes two provider queries to be enumerated/executed")]
            public void GetSpecificPropertyShouldNotCauseProviderQueriesToBeEnumerated()
            {
                using (TestUtil.RestoreStaticValueOnDispose(typeof(QueryProvider), "OnCreateQuery"))
                using (TestUtil.RestoreStaticValueOnDispose(typeof(Queryable<SimpleEntity>), "OnGetEnumerator"))
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    string[] requestUris = new string[]
                    {
                        "/Entities(1)/Name",
                        "/Entities(1)/Name/$value"
                    };

                    byte createQueryCalls = 0;
                    byte getEnumeratorCalls = 0;

                    QueryProvider.OnCreateQuery = () => { createQueryCalls++; };
                    Queryable<SimpleEntity>.OnGetEnumerator = () => { getEnumeratorCalls++; };

                    request.DataServiceType = typeof(TestDataService27);

                    foreach (string requestUri in requestUris)
                    {
                        createQueryCalls = 0;
                        getEnumeratorCalls = 0;

                        request.RequestUriString = requestUri;
                        request.HttpMethod = "GET";
                        request.SendRequest();
                        string resp = request.GetResponseStreamAsText();
                        Assert.AreEqual(1, createQueryCalls);
                        Assert.AreEqual(1, getEnumeratorCalls);
                    }
                }
            }

            public class SimpleEntity
            {
                public int ID { get; set; }
                public string Name { get; set; }
            }

            public class TestDataService27 : DataService<TestDataService27>
            {
                private static List<SimpleEntity> db = new List<SimpleEntity>
                    {
                        new SimpleEntity { ID = 1, Name = "Entity 1" },
                        new SimpleEntity { ID = 2, Name = "Entity 2" }
                    };

                public static void InitializeService(DataServiceConfiguration configuration)
                {
                    configuration.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
                }

                public IQueryable<SimpleEntity> Entities
                {
                    get
                    {
                        IQueryable<SimpleEntity> dbAsQueryable = db.AsQueryable<SimpleEntity>();
                        return new Queryable<SimpleEntity>(new QueryProvider(dbAsQueryable.Provider), dbAsQueryable);
                    }
                }
            }

            #region Dummy IQueryable and IQueryProvider

            public class Queryable<T> : IQueryable<T>, IOrderedQueryable<T>
            {
                public static Action OnGetEnumerator;
                public static Action OnElementType;
                public static Action OnExpression;
                public static Action OnProvider;

                private IQueryProvider provider;
                private IQueryable<T> queryable;

                public Queryable(IQueryProvider provider, IQueryable<T> queryable)
                {
                    this.provider = provider;
                    this.queryable = queryable;
                }

                IEnumerator<T> IEnumerable<T>.GetEnumerator()
                {
                    if (OnGetEnumerator != null)
                    {
                        OnGetEnumerator();
                    }
                    return this.queryable.GetEnumerator();
                }

                #region IEnumerable Members

                IEnumerator IEnumerable.GetEnumerator()
                {
                    if (OnGetEnumerator != null)
                    {
                        OnGetEnumerator();
                    }
                    return this.queryable.GetEnumerator();
                }

                #endregion

                #region IQueryable Members

                public Type ElementType
                {
                    get
                    {
                        if (OnElementType != null)
                        {
                            OnElementType();
                        }
                        return this.queryable.ElementType;
                    }
                }

                public Expression Expression
                {
                    get
                    {
                        if (OnExpression != null)
                        {
                            OnExpression();
                        }
                        return this.queryable.Expression;
                    }
                }

                public IQueryProvider Provider
                {
                    get
                    {
                        if (OnProvider != null)
                        {
                            OnProvider();
                        }
                        return this.provider;
                    }
                }

                #endregion
            }

            public class QueryProvider : IQueryProvider
            {
                private IQueryProvider provider;

                public static Action OnCreateQuery;
                public static Action OnExecute;

                public QueryProvider(IQueryProvider provider)
                {
                    this.provider = provider;
                }

                #region IQueryProvider Members

                public IQueryable<U> CreateQuery<U>(Expression expression)
                {
                    if (OnCreateQuery != null)
                    {
                        OnCreateQuery();
                    }

                    return (IQueryable<U>)(new Queryable<U>(this, this.provider.CreateQuery<U>(expression)));
                }

                public IQueryable CreateQuery(Expression expression)
                {
                    if (OnCreateQuery != null)
                    {
                        OnCreateQuery();
                    }

                    IQueryable queryable = this.provider.CreateQuery(expression);
                    Type queryableType = typeof(Queryable<>).MakeGenericType(queryable.ElementType);
                    return (IQueryable)Activator.CreateInstance(queryableType, queryable, this);
                }

                public U Execute<U>(Expression expression)
                {
                    if (OnExecute != null)
                    {
                        OnExecute();
                    }

                    return (U)this.provider.Execute<U>(expression);
                }

                public object Execute(Expression expression)
                {
                    if (OnExecute != null)
                    {
                        OnExecute();
                    }

                    return this.provider.Execute(expression);
                }

                #endregion
            }

            #endregion

            [TestMethod, Variation("DELETE to $ref uri doesn't fail if If-Match or If-None-Match header was specified")]
            public void DeleteToLinksWithEtagDoesNotFail()
            {
                var uriAndMethodInfo = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("/Customers(1)/Orders/$ref?$id=Orders(1)", "DELETE"), //FAILS
                    new KeyValuePair<string, string>("/Customers(1)/BestFriend/$ref", "DELETE"), //FAILS
                    new KeyValuePair<string, string>("/Customers(1)/Orders/$ref", "GET"),
                    new KeyValuePair<string, string>("/Customers(1)/BestFriend/$ref", "GET"),
                    new KeyValuePair<string, string>("/Customers(1)/Orders/$ref", "POST"),
                    new KeyValuePair<string, string>("/Customers(1)/BestFriend/$ref", "PUT"),
                    new KeyValuePair<string, string>("/Customers(1)/BestFriend/$ref", "PATCH")
                };

                var ifMatchHeader = new KeyValuePair<string, string>("If-Match", "W/\"'Foo'\"");
                var ifNoneMatchHeader = new KeyValuePair<string, string>("If-None-Match", "W/\"'Bar'\"");
                foreach (var v in uriAndMethodInfo)
                {
                    string uri = v.Key;
                    string method = v.Value;
                    UnitTestsUtil.VerifyInvalidRequestForVariousProviders(null, uri, null, method, 400, ifMatchHeader);
                    UnitTestsUtil.VerifyInvalidRequestForVariousProviders(null, uri, null, method, 400, ifNoneMatchHeader);
                }
            }

            [TestMethod, Variation("Service Initialization Errors do not include any details")]
            public void ServiceInitError()
            {
                Exception[] exceptionToThrow = new Exception[]
                {
                    new InvalidOperationException(),            // 500
                    new DataServiceException(),                 // 500
                    new DataServiceException(400, "exception")  // 400
                };

                foreach (Exception e in exceptionToThrow)
                {
                    TestService28.ThrowException = e;
                    using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                    {
                        request.ServiceType = typeof(TestService28);
                        request.RequestUriString = "/$metadata";
                        Exception ex = TestUtil.RunCatching(request.SendRequest);

                        Assert.IsNotNull(ex);
                        int status = request.ResponseStatusCode;

                        if (e is DataServiceException)
                        {
                            Assert.AreEqual(((DataServiceException)e).StatusCode, status);
                        }
                        else
                        {
                            Assert.AreEqual(500, status);
                        }
                    }
                }
            }

            [System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
            public class TestService28 : DataService<CustomDataContext>
            {
                public static Exception ThrowException = null;

                public static void InitializeService(DataServiceConfiguration configuration)
                {
                    if (ThrowException != null)
                    {
                        throw ThrowException;
                    }
                }
            }

            public class TestService29 : DataService<CustomDataContext>
            {
                public static bool SeenException;

                public static void InitializeService(DataServiceConfiguration configuration)
                {
                    try
                    {
                        configuration.EnableTypeAccess(typeof(Customer).FullName);
                    }
                    catch (ArgumentException ae)
                    {
                        TestUtil.AssertContains(ae.Message, string.Format("The given type '{0}' is not a complex type.", typeof(Customer).FullName));
                        SeenException = true;
                        throw;
                    }
                }
            }

            [TestMethod, Variation("EnableTypeAccess should only take complex type names")]
            public void EnableTypeAccessOnlyTakesComplexType()
            {
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(TestService29)))
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    Assert.IsFalse(TestService29.SeenException);
                    request.ServiceType = typeof(TestService29);
                    request.RequestUriString = "/$metadata";
                    request.HttpMethod = "GET";
                    Exception e = TestUtil.RunCatching(request.SendRequest);
                    Assert.IsNotNull(e);
                    Assert.IsTrue(TestService29.SeenException);
                }
            }

            public class ItemBase
            {
                public int ID { get; set; }
            }

            public class ItemDerived : ItemBase
            {
                public string Name { get; set; }
            }

            public class Foo
            {
                public int ID { get; set; }
                public ItemDerived Item { get; set; }
            }

            public class TestDataContext30
            {
                private static ItemDerived[] items = new ItemDerived[] { new ItemDerived { ID = 1, Name = "Waseem" }, new ItemDerived { ID = 2, Name = "Basheer" } };
                private static Foo[] foos = new Foo[] { new Foo { ID = 1, Item = items[0] }, new Foo { ID = 2, Item = items[1] } };

                public IQueryable<ItemBase> Items { get { return items.Cast<ItemBase>().AsQueryable(); } }
                public IQueryable<Foo> Foos { get { return foos.AsQueryable(); } }
            }

            public class TestService30 : OpenWebDataService<TestDataContext30>
            {
                [QueryInterceptor("Items")]
                public Expression<Func<ItemBase, bool>> ItemInterceptor()
                {
                    return t => (t.ID % 2 > 0 && t.ID < 5) || t.ID == 1;
                }
            }

            [TestMethod, Variation("Server Query Interceptor: when we compose query interceptors, RequestUriProcessor.InvokeWhere() can throw due to an illegal cast operation.")]
            public void InvokeWhereShouldNotThrowOnIllegalCastOperation()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.ServiceType = typeof(TestService30);
                    request.RequestUriString = "/Foos(1)/Item";
                    request.HttpMethod = "GET";
                    Exception e = TestUtil.RunCatching(request.SendRequest);
                    Assert.IsNull(e);
                }
            }

            /// <summary>
            /// CLR type corresponding to an entity type defined in the test below. 
            /// </summary>
            [global::System.Data.Objects.DataClasses.EdmEntityTypeAttribute(NamespaceName = "KatmaiTypesModel", Name = "KatmaiTypesTable")]
            [global::System.Runtime.Serialization.DataContractAttribute(IsReference = true)]
            [global::System.Serializable()]
            public partial class KatmaiTypesTable : global::System.Data.Objects.DataClasses.EntityObject
            {
                /// <summary>
                /// Create a new KatmaiTypesTable object.
                /// </summary>
                /// <param name="id">Initial value of ID.</param>
                public static KatmaiTypesTable CreateKatmaiTypesTable(int id)
                {
                    KatmaiTypesTable katmaiTypesTable = new KatmaiTypesTable();
                    katmaiTypesTable.ID = id;
                    return katmaiTypesTable;
                }
                /// <summary>
                /// There are no comments for Property ID in the schema.
                /// </summary>
                [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(EntityKeyProperty = true, IsNullable = false)]
                [global::System.Runtime.Serialization.DataMemberAttribute()]
                public int ID
                {
                    get
                    {
                        return this._ID;
                    }
                    set
                    {
                        this.OnIDChanging(value);
                        this.ReportPropertyChanging("ID");
                        this._ID = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                        this.ReportPropertyChanged("ID");
                        this.OnIDChanged();
                    }
                }
                private int _ID;
                partial void OnIDChanging(int value);
                partial void OnIDChanged();
                /// <summary>
                /// There are no comments for Property TimeField in the schema.
                /// </summary>
                [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
                [global::System.Runtime.Serialization.DataMemberAttribute()]
                public global::System.Nullable<global::System.TimeSpan> TimeField
                {
                    get
                    {
                        return this._TimeField;
                    }
                    set
                    {
                        this.OnTimeFieldChanging(value);
                        this.ReportPropertyChanging("TimeField");
                        this._TimeField = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                        this.ReportPropertyChanged("TimeField");
                        this.OnTimeFieldChanged();
                    }
                }
                private global::System.Nullable<global::System.TimeSpan> _TimeField;
                partial void OnTimeFieldChanging(global::System.Nullable<global::System.TimeSpan> value);
                partial void OnTimeFieldChanged();
                /// <summary>
                /// There are no comments for Property DateTimeOffsetField in the schema.
                /// </summary>
                [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
                [global::System.Runtime.Serialization.DataMemberAttribute()]
                public global::System.Nullable<global::System.DateTimeOffset> DateTimeOffsetField
                {
                    get
                    {
                        return this._DateTimeOffsetField;
                    }
                    set
                    {
                        this.OnDateTimeOffsetFieldChanging(value);
                        this.ReportPropertyChanging("DateTimeOffsetField");
                        this._DateTimeOffsetField = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                        this.ReportPropertyChanged("DateTimeOffsetField");
                        this.OnDateTimeOffsetFieldChanged();
                    }
                }
                private global::System.Nullable<global::System.DateTimeOffset> _DateTimeOffsetField;
                partial void OnDateTimeOffsetFieldChanging(global::System.Nullable<global::System.DateTimeOffset> value);
                partial void OnDateTimeOffsetFieldChanged();
            }

            [TestMethod, Variation("NullRef when using an EF model that uses DateTimeOffset")]
            public void UseDateTimeOffsetInEFModelShouldThrowNotSupported()
            {
                string modelNamespace = "KatmaiTypesModel";
                string modelTypeName = "KatmaiTypesTable";

                // Define field sets that use the EDM data types we don't support, Time and DateTimeOffset,
                // in various combinations. These correspond to data types that are new in SQL Server 2008. 
                // We don't need to worry about the other SQL Server 2008 types (Date and DateTime2) because 
                // they map to EDM DateTime, which we do support. 

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Fields", new string[][]{
                        new string[] { "Time" },
                        new string[] { "DateTimeOffset" },
                        new string[] { "Time", "DateTimeOffset" },
                        new string[] { "DateTimeOffset", "Time" }
                    })
                );

                // Run the engine, building a model with each of the field sets and then attempting to 
                // populate the ObjectContextServiceProvider's metadata with this model. 
                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    StringBuilder csdl = new StringBuilder();

                    csdl.AppendFormat(@"
      <Schema Namespace=""{0}"" Alias=""Self"" xmlns=""http://schemas.microsoft.com/ado/2006/04/edm"">
        <EntityContainer Name=""KatmaiTypesEntities"">
          <EntitySet Name=""KatmaiTypesTable"" EntityType=""{0}.{1}"" />
        </EntityContainer>
        <EntityType Name=""{1}"">
          <Key>
            <PropertyRef Name=""ID"" />
          </Key>
          <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />",
                    modelNamespace,
                    modelTypeName);

                    string[] fields = (string[])values["Fields"];
                    foreach (string field in fields)
                    {
                        csdl.AppendFormat("\n          <Property Name=\"{0}Field\" Type=\"{0}\" />", field);
                    }

                    csdl.Append(@"
        </EntityType>
      </Schema>");

                    // Load the CSDL into an EdmItemColleciton and build a MetadataWorkspace.
                    edm.EdmItemCollection edmItemCollection = new edm.EdmItemCollection(
                        new XmlReader[] { XmlReader.Create(new StringReader(csdl.ToString())) });

                    edm.MetadataWorkspace mw = new edm.MetadataWorkspace();
                    mw.RegisterItemCollection(edmItemCollection);

                    // Create a resource type corresponding to the EDM type defined in the model. 
                    p.ResourceType rt = new p.ResourceType(typeof(KatmaiTypesTable), p.ResourceTypeKind.EntityType, null, modelNamespace, modelTypeName, false);

                    Assembly serverAssembly = typeof(p.ResourceType).Assembly;

                    // Create a metadata wrapper for MetadataWorkspace
                    Type objectContextMetadataType = serverAssembly.GetType("Microsoft.OData.Service.Providers.ObjectContextMetadata");
                    ConstructorInfo objectContextMetadataConstructor = objectContextMetadataType.GetConstructor(new Type[] { typeof(edm.MetadataWorkspace) });
                    object ocMetadata = objectContextMetadataConstructor.Invoke(new object[] { mw });

                    // Get primitive resource type map
                    Type primitiveResourceTypeMappingType = serverAssembly.GetType("Microsoft.OData.Service.Providers.PrimitiveResourceTypeMap");
                    PropertyInfo primitiveResourceTypeMapPropertyInfo = primitiveResourceTypeMappingType.GetProperty("TypeMap", BindingFlags.NonPublic | BindingFlags.Static);
                    object typeMap = primitiveResourceTypeMapPropertyInfo.GetValue(null, null);

                    // Invoke the PopulateMemberMetadata method on the ObjectContextServiceProvider via reflection. This should throw a 
                    // NotSupportedException indicating the SQL Server 2008 data types are not supported.
                    Type objectContextServiceProviderType = serverAssembly.GetType("Microsoft.OData.Service.Providers.ObjectContextServiceProvider");
                    MethodInfo populateMemberMetadataMethod = objectContextServiceProviderType.GetMethod("PopulateMemberMetadata", BindingFlags.Static | BindingFlags.NonPublic);

                    Type resourceTypeCacheItemType = serverAssembly.GetType("Microsoft.OData.Service.Caching.ResourceTypeCacheItem");
                    ConstructorInfo typeCacheItemConstructor = resourceTypeCacheItemType.GetConstructor(new Type[] { typeof(p.ResourceType) });
                    object resourceTypeCacheItem = typeCacheItemConstructor.Invoke(new object[] { rt });
                    try
                    {
                        populateMemberMetadataMethod.Invoke(null, new object[] { resourceTypeCacheItem, ocMetadata, null, typeMap });
                    }
                    catch (TargetInvocationException ex)
                    {
                        // Check that the exception thrown was a NotSupportedException.
                        Assert.IsNotNull(ex.InnerException as NotSupportedException);
                    }
                });
            }

            public class TestContext31 : IUpdatable
            {
                public static Action<String> SetValueCallBack = null;

                public IQueryable<Customer> Customers
                {
                    get { return new Customer[] { }.AsQueryable(); }
                }

                public IQueryable<Order> Orders
                {
                    get { return new Order[] { }.AsQueryable(); }
                }

                public IQueryable<OrderDetail> OrderDetails
                {
                    get { return new OrderDetail[] { }.AsQueryable(); }
                }

                #region IUpdatable Members

                public object CreateResource(string containerName, string fullTypeName)
                {
                    return new Customer();
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
                    if (SetValueCallBack != null)
                    {
                        SetValueCallBack(propertyName);
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
                    if (SetValueCallBack != null)
                    {
                        SetValueCallBack(propertyName);
                    }
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

            [TestMethod, Variation("JSON payload should terminate properly")]
            public void JsonPayloadShouldTerminateProperly_2()
            {
                string jsonPayload = "{" +
                                        "@odata.type: \"" + typeof(Customer).FullName + "\" ," +
                                        "ID: 125," +
                                        "Address : " +
                                        "{" +
                                            "@odata.type: \"" + typeof(Address).FullName + "\"," +
                                            "City: \"Redmond\"," +
                                            "PostalCode: \"98052\"," +
                                            "State: \"WA\"," +
                                            "StreetAddress: \"Street Number, Street Address\"" +
                                        "}," +
                                        "Orders : [" +
                                            "{ @odata.type: \"AstoriaUnitTests.Stubs.Order\" ," +
                                                "ID: 152," +
                                                "DollarAmount: 500.00" +
                                            "}," +
                                            "{" +
                                                "@odata.type: \"AstoriaUnitTests.Stubs.Order\"" +
                                            "}," +
                                            "{ @odata.type: \"AstoriaUnitTests.Stubs.Order\" ," +
                                                "\"ID\": 153," +
                                                "\"DollarAmount\": 0.00" +
                                            "}" +
                                        "]," +
                                        "Name: \"Foo\"" +
                                     "}";

                String[] callOrder = new String[] { 
                    // primitive or complex properties goes first, in the order which they are specified
                    "ID", "Name", "City", "PostalCode", "State", "StreetAddress", "Address", 
                    // navigation properties goes last, in the order which they are specified
                    "ID", "DollarAmount", "Orders", "Orders", "ID", "DollarAmount", "Orders" };

                int currentIndex = 0;

                TestContext31.SetValueCallBack = (k) =>
                    {
                        Assert.AreEqual(callOrder[currentIndex++], k);
                        Trace.WriteLine(k);
                    };

                UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.JsonLightMimeType, "/Customers", typeof(TestContext31),
                    new KeyValuePair<string, string>[0], "POST", jsonPayload);
            }

            [TestMethod, Variation("ArgumentOutOfRangeException when service operation string argument ends with un-escaped single quote")]
            public void VerifyResponseStatusIfServiceOperationArgumentEndsWithUnEscapedSingleQuote()
            {
                UnitTestsUtil.VerifyInvalidRequest(null, "/GetCustomerByCity?city=''NYC'", typeof(CustomRowBasedContext), UnitTestsUtil.AtomFormat, "GET", 400, "Bad Request - Error in query syntax.");
                UnitTestsUtil.VerifyInvalidRequest(null, "/GetCustomerByCity?city='N'YC'", typeof(CustomRowBasedContext), UnitTestsUtil.AtomFormat, "GET", 400, "Bad Request - Error in query syntax.");
                UnitTestsUtil.VerifyInvalidRequest(null, "/GetCustomerByCity?city='N' 'YC'", typeof(CustomRowBasedContext), UnitTestsUtil.AtomFormat, "GET", 400, "Bad Request - Error in query syntax.");
                UnitTestsUtil.VerifyInvalidRequest(null, "/GetCustomerByCity?city='NYC''", typeof(CustomRowBasedContext), UnitTestsUtil.AtomFormat, "GET", 400, "Bad Request - Error in query syntax.");
                UnitTestsUtil.VerifyInvalidRequest(null, "/GetCustomerByCity?city='NYC''''", typeof(CustomRowBasedContext), UnitTestsUtil.AtomFormat, "GET", 400, "Bad Request - Error in query syntax.");
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(CustomRowBasedContext);
                    request.HttpMethod = "GET";

                    request.RequestUriString = "/GetCustomerByCity?city='NYC'";
                    request.SendRequest();
                    Assert.AreEqual(200, request.ResponseStatusCode);

                    request.RequestUriString = "/GetCustomerByCity?city='''NYC'";
                    request.SendRequest();
                    Assert.AreEqual(200, request.ResponseStatusCode);

                    request.RequestUriString = "/GetCustomerByCity?city='''N''YC'";
                    request.SendRequest();
                    Assert.AreEqual(200, request.ResponseStatusCode);

                    request.RequestUriString = "/GetCustomerByCity?city='''N''Y''C'";
                    request.SendRequest();
                    Assert.AreEqual(200, request.ResponseStatusCode);

                    request.RequestUriString = "/GetCustomerByCity?city='''N''Y''C'''";
                    request.SendRequest();
                    Assert.AreEqual(200, request.ResponseStatusCode);
                }
            }

            [TestMethod, Variation("Specifying $select to a $count query returns the wrong results")]
            public void VerifyResultCountIfSpecifySelectToCountQuery()
            {
                TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.MimeTextPlain, "/Customers/$count?$skip=1&$top=1", typeof(CustomDataContext), null, "GET");
                string count = request.GetResponseStreamAsText();

                request = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.MimeTextPlain, "/Customers/$count?$skip=1&$top=1&$select=ID", typeof(CustomDataContext), null, "GET");
                string count1 = request.GetResponseStreamAsText();

                Assert.IsTrue(count == count1, "Projections should not alter the result for $count queryies");
            }

            [TestMethod, Variation("$ref query with $count at the end is not allowed")]
            public void CountAfterRefQueryNotAllowed()
            {
                UnitTestsUtil.VerifyInvalidRequest(null, "/Customers(1)/Orders/$ref/$count?$select=ID", typeof(CustomDataContext), null, "GET", 404);
                UnitTestsUtil.VerifyInvalidRequest(null, "/Customers(1)/Orders/$ref?$select=ID", typeof(CustomDataContext), null, "GET", 400);
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("DELETE request with $select causes null reference exception")]
            public void TestDeleteRequestWithSelect()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("QueryParameter", new string[] { "$top=1", "$skip=1", "$filter=ID%20eq%201", "$select=ID", "$orderBy=Name", "$expand=Orders", "$skiptoken=1", "$count=true" }),
                    new Dimension("Provider", AstoriaUnitTests.Data.ServiceModelData.Values),
                    new Dimension("HTTPMethod", new string[] { "POST", "PUT", "PATCH", "DELETE" })
                );

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    string queryParameter = (string)values["QueryParameter"];
                    string methodName = (string)values["HTTPMethod"];
                    ServiceModelData model = (ServiceModelData)values["Provider"];
                    bool isV2Provider = (model.ServiceModelType != typeof(CustomDataContext) && model.ServiceModelType != typeof(ocs.CustomObjectContext));
                    bool isV2QueryOption = (queryParameter.StartsWith("$select") || queryParameter.StartsWith("$skiptoken") || queryParameter.StartsWith("$count"));
                    string errorString = DataServicesResourceUtil.GetString("RequestQueryProcessor_QueryNoOptionsApplicable");

                    if (!model.IsUnitTestProvider)
                    {
                        return;
                    }

                    // POST uri's - In V1, we allowd
                    if (methodName == "POST")
                    {
                        foreach (string uri in new string[] { "/Customers", "/Customers(1)/Orders", "/Customers(1)/Orders/$ref" })
                        {
                            string uriWithQueryParameter = String.Format("{0}?{1}", uri, queryParameter);

                            if (isV2QueryOption || isV2Provider)
                            {
                                UnitTestsUtil.VerifyInvalidRequest(null, uriWithQueryParameter, model.ServiceModelType, UnitTestsUtil.AtomFormat, methodName, 400, errorString);
                            }
                        }
                    }
                    else
                    {
                        var uris = new List<string>() { "/Customers(1)", "/Customers(1)/Orders(1)", "/Customers(1)/BestFriend", "/Customers(1)/BestFriend/$ref" };

                        var deleteEntityRefUri = "/Customers(1)/Orders/$ref?$id=Orders(1)";

                        if (methodName == "DELETE")
                        {
                            uris.Add(deleteEntityRefUri);
                        }
                        else
                        {
                            uris.Add("/Customers(1)/Orders(1)/$ref");
                        }

                        // PUT, PATCH DELETE uri's
                        foreach (string uri in uris)
                        {
                            string uriWithQueryParameter = uri == deleteEntityRefUri
                                ? String.Format("{0}&{1}", uri, queryParameter)
                                : String.Format("{0}?{1}", uri, queryParameter);

                            if (isV2QueryOption || isV2Provider)
                            {
                                UnitTestsUtil.VerifyInvalidRequest(null, uriWithQueryParameter, model.ServiceModelType, UnitTestsUtil.AtomFormat, methodName, 400, errorString);
                            }
                        }
                    }
                });
            }

            [TestMethod, Variation("OData-Version is sent as 2.0 in all non-GET error cases")]
            public void NonGetErrorResponseODataVersionMustBe40()
            {
                using (CustomDataContext.CreateChangeScope())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/Customers/$count";
                    request.IfMatch = "W/\"foo\"";
                    request.HttpMethod = "GET";

                    Exception e = TestUtil.RunCatching(request.SendRequest);
                    Assert.AreEqual(400, request.ResponseStatusCode);
                    Assert.IsNotNull(e);
                    Assert.AreEqual(request.ResponseVersion, "4.0;", "response version must be 4.0");
                }
            }

            public class AstoriaDataContext
            {
                public static Uri ContextUri;

                private DataServiceContext ctx;

                public AstoriaDataContext()
                {
                    this.ctx = new DataServiceContext(AstoriaDataContext.ContextUri);
                    //ctx.EnableAtom = true;
                    //this.ctx.Format.UseAtom();
                }

                public DataServiceQuery<Customer> Customers
                {
                    get
                    {
                        return this.ctx.CreateQuery<Customer>("Customers");
                    }
                }

                public DataServiceQuery<Order> Orders
                {
                    get
                    {
                        return this.ctx.CreateQuery<Order>("Orders");
                    }
                }

                public DataServiceQuery<OrderDetail> OrderDetails
                {
                    get
                    {
                        return this.ctx.CreateQuery<OrderDetail>("OrderDetail");
                    }
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void TestSettingContextUriOnDataContext()
            {
                using (CustomDataContext.CreateChangeScope())
                using (TestUtil.RestoreStaticValueOnDispose(typeof(AstoriaDataContext), "ContextUri"))
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.StartService();
                    AstoriaDataContext.ContextUri = request.ServiceRoot;
                    using (TestWebRequest requestToAstoria = TestWebRequest.CreateForInProcessWcf())
                    {
                        requestToAstoria.DataServiceType = typeof(AstoriaDataContext);
                        requestToAstoria.StartService();
                        DataServiceContext ctxToAstoria = new DataServiceContext(requestToAstoria.ServiceRoot);
                        //ctxToAstoria.EnableAtom = true;
                        //ctxToAstoria.Format.UseAtom();
                        foreach (Customer c in ctxToAstoria.CreateQuery<Customer>("Customers"))
                        {
                        }
                    }
                }
            }

            [TestMethod, Variation]
            public void ShouldThrowIfPropertyAccessSourceNotSingleValueIfRequired()
            {
                TestUtil.RunCombinations(
                    new Type[] {
                        typeof(AstoriaUnitTests.Stubs.CustomRowBasedOpenTypesContext),
                        typeof(AstoriaUnitTests.Stubs.CustomRowBasedContext),
                        typeof(AstoriaUnitTests.Stubs.CustomDataContext)},
                    new string[] {
                        "/Customers?$filter=cast(BestFriend, 'AstoriaUnitTests.Stubs.CustomerWithBirthday')/Orders/ID gt 0",
                        "/Customers?$filter=cast('AstoriaUnitTests.Stubs.CustomerWithBirthday')/Orders/ID gt 0",
                        "/Customers?$orderby=Orders/ID",
                        "/Customers?$orderby=cast('AstoriaUnitTests.Stubs.CustomerWithBirthday')/Orders/ID",
                        "/Customers?$filter=Orders/ID gt 0"},
                    (providerType, requestUri) =>
                    {
                        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                        {
                            request.DataServiceType = providerType;
                            request.RequestUriString = requestUri;
                            Exception exception = TestUtil.RunCatching(request.SendRequest);
                            Assert.IsNotNull(exception);
                            Assert.AreEqual("4.0;", request.ResponseVersion);
                            int errorLocation = requestUri.IndexOf("ID") - requestUri.IndexOf('=') - 1;
                            Assert.IsInstanceOfType(exception.InnerException, typeof(DataServiceException));
                            Assert.AreEqual(400, ((DataServiceException)(exception.InnerException)).StatusCode);

                            Assert.AreEqual(ODataLibResourceUtil.GetString("MetadataBinder_PropertyAccessSourceNotSingleValue", "ID", errorLocation), exception.InnerException.Message);
                        }
                    });
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Make sure the server can deserialize what it serializes - <m:properties> is not a required child of <atom:content> element")]
            public void ServerShouldRoundtripWhetherPropertiesPresentInContentElement()
            {
                using (CustomDataContext.CreateChangeScope())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/Customers(0)?$select=Orders";
                    request.HttpMethod = "GET";
                    request.Accept = UnitTestsUtil.AtomFormat;
                    request.SendRequest();
                    XmlDocument atom = UnitTestsUtil.GetResponseAsAtom(request);
                    string payload = atom.InnerXml;

                    string[] XPathExprs = new string[]
                    {
                        "boolean(/atom:entry[contains(atom:id, 'Customers(0)')]/atom:content)",
                        "not    (/atom:entry[contains(atom:id, 'Customers(0)')]/atom:content/@src)",
                        "not    (/atom:entry/adsm:properties)",
                        "not    (/atom:entry/atom:content/adsm:properties)"
                    };

                    UnitTestsUtil.VerifyXPathExpressionResults(atom, true, XPathExprs);

                    request.RequestUriString = "/Customers(0)";
                    request.HttpMethod = "PATCH";
                    request.RequestContentType = UnitTestsUtil.AtomFormat;
                    request.IfMatch = request.ResponseETag;
                    request.SetRequestStreamAsText(payload);
                    request.SendRequest();
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Accessing DataServiceHost.RequestHeaders/ResponseHeaders properties when host instance doesn't implement IDataServiceHost2 should throw exception")]
            public void AccessingHeaderWhenHostInstanceNotImplementIDataServiceHost2ShouldThrow()
            {
                using (TestUtil.RestoreStaticValueOnDispose(typeof(OpenWebDataServiceHelper), "ForceVerboseErrors"))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
                using (CustomDataContext.CreateChangeScope())
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    OpenWebDataServiceHelper.ForceVerboseErrors = true;
                    OpenWebDataServiceHelper.ProcessedRequest.Value += new EventHandler<DataServiceProcessingPipelineEventArgs>(AccessRequestHeaders);

                    try
                    {
                        UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.AtomFormat, "/Customers(1)", typeof(CustomDataContext), null, "GET", null, WebServerLocation.InProcess);
                        Assert.Fail("Accessing request headers should throw");
                    }
                    catch (WebException exception)
                    {
                        Assert.IsTrue(((InvalidOperationException)exception.InnerException).Message.Contains(DataServicesResourceUtil.GetString("DataServiceHost_FeatureRequiresIDataServiceHost2")));
                    }

                    try
                    {
                        OpenWebDataServiceHelper.ProcessedRequest.Value = null;
                        OpenWebDataServiceHelper.ProcessedRequest.Value += new EventHandler<DataServiceProcessingPipelineEventArgs>(AccessResponseHeaders);
                        UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.AtomFormat, "/Customers(1)", typeof(CustomDataContext), null, "GET", null, WebServerLocation.InProcess);
                        Assert.Fail("Accessing request headers should throw");
                    }
                    catch (WebException exception)
                    {
                        Assert.IsTrue(((InvalidOperationException)exception.InnerException).Message.Contains(DataServicesResourceUtil.GetString("DataServiceHost_FeatureRequiresIDataServiceHost2")));
                    }
                }
            }

            // [TestMethod, Variation("Medium trust bug for filter scenarios")]
            [Ignore]
            public void TestMediumTrustForFilterScenarios()
            {
                LocalWebServerHelper.RunInMediumTrust = true;
                try
                {
                    Guid guid = System.Guid.Empty;
                    KeyValuePair<string, string>[] uri_XPaths = new KeyValuePair<string, string>[] {
                        new KeyValuePair<string, string>("/Values?$filter=Name gt 'Customer 1'", "count(//atom:entry)=3"),
                        new KeyValuePair<string, string>("/Values?$filter=Name ge 'Customer 1'", "count(//atom:entry)=4"),
                        new KeyValuePair<string, string>("/Values?$filter=Name lt 'Customer 2'", "count(//atom:entry)=2"),
                        new KeyValuePair<string, string>("/Values?$filter=Name le 'Customer 2'", "count(//atom:entry)=3"),
                        new KeyValuePair<string, string>("/Values?$filter=GuidValue gt " + guid.ToString(), "count(//atom:entry)=5"),
                        new KeyValuePair<string, string>("/Values?$filter=GuidValue ge " + guid.ToString(), "count(//atom:entry)=5"),
                        new KeyValuePair<string, string>("/Values?$filter=GuidValue lt " + guid.ToString(), "count(//atom:entry)=0"),
                        new KeyValuePair<string, string>("/Values?$filter=GuidValue le " + guid.ToString(), "count(//atom:entry)=0"),
                        new KeyValuePair<string, string>("/Values?$filter=NullableGuidValue gt " + guid.ToString(), "count(//atom:entry)=2"),
                        new KeyValuePair<string, string>("/Values?$filter=NullableGuidValue ge " + guid.ToString(), "count(//atom:entry)=2"),
                        new KeyValuePair<string, string>("/Values?$filter=NullableGuidValue lt " + guid.ToString(), "count(//atom:entry)=3"),
                        new KeyValuePair<string, string>("/Values?$filter=NullableGuidValue le " + guid.ToString(), "count(//atom:entry)=3"),
                        new KeyValuePair<string, string>("/Values?$filter=BoolValue gt false", "count(//atom:entry)=3"),
                        new KeyValuePair<string, string>("/Values?$filter=BoolValue ge false", "count(//atom:entry)=5"),
                        new KeyValuePair<string, string>("/Values?$filter=BoolValue lt true", "count(//atom:entry)=2"),
                        new KeyValuePair<string, string>("/Values?$filter=BoolValue le false", "count(//atom:entry)=2"),
                        new KeyValuePair<string, string>("/Values?$filter=NullableBoolValue gt false", "count(//atom:entry)=2"),
                        new KeyValuePair<string, string>("/Values?$filter=NullableBoolValue ge false", "count(//atom:entry)=3"),
                        new KeyValuePair<string, string>("/Values?$filter=NullableBoolValue lt true", "count(//atom:entry)=3"),
                        new KeyValuePair<string, string>("/Values?$filter=NullableBoolValue le false", "count(//atom:entry)=3"),
                    };

                    foreach (var uri in uri_XPaths)
                    {
                        TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(
                            UnitTestsUtil.AtomFormat,
                            uri.Key,
                            typeof(TestContext32),
                            null,
                            "GET",
                            null,
                            WebServerLocation.Local);

                        UnitTestsUtil.VerifyXPaths(request.GetResponseStreamAsXmlDocument(), uri.Value);
                    }
                }
                finally
                {
                    LocalWebServerHelper.RunInMediumTrust = null;
                }
            }


            public class TestContext32
            {
                private static List<TestType32> data;

                static TestContext32()
                {
                    data = new List<TestType32>();
                    data.Add(new TestType32() { ID = 1, Name = "Customer 0", BoolValue = true, NullableBoolValue = null, GuidValue = System.Guid.NewGuid(), NullableGuidValue = null });
                    data.Add(new TestType32() { ID = 2, Name = "Customer 1", BoolValue = false, NullableBoolValue = true, GuidValue = System.Guid.NewGuid(), NullableGuidValue = System.Guid.NewGuid() });
                    data.Add(new TestType32() { ID = 3, Name = "Customer 2", BoolValue = true, NullableBoolValue = false, GuidValue = System.Guid.NewGuid(), NullableGuidValue = null });
                    data.Add(new TestType32() { ID = 4, Name = "Customer 3", BoolValue = false, NullableBoolValue = null, GuidValue = System.Guid.NewGuid(), NullableGuidValue = System.Guid.NewGuid() });
                    data.Add(new TestType32() { ID = 5, Name = "Customer 4", BoolValue = true, NullableBoolValue = true, GuidValue = System.Guid.NewGuid(), NullableGuidValue = null });
                }

                public IQueryable<TestType32> Values
                {
                    get { return data.AsQueryable(); }
                }
            }

            public class TestType32
            {
                public int ID { get; set; }
                public string Name { get; set; }
                public bool BoolValue { get; set; }
                public bool? NullableBoolValue { get; set; }
                public Guid GuidValue { get; set; }
                public Guid? NullableGuidValue { get; set; }
            }

            private static void AccessRequestHeaders(object sender, DataServiceProcessingPipelineEventArgs e)
            {
                // Try accessing the request headers and response headers and make sure they throw exception
                string contentType = e.OperationContext.RequestHeaders["Content-Type"];
                Assert.Fail("Accessing request headers should throw");
            }

            private static void AccessResponseHeaders(object sender, DataServiceProcessingPipelineEventArgs e)
            {
                // Try accessing the request headers and response headers and make sure they throw exception
                string contentType = e.OperationContext.ResponseHeaders["ETag"];
                Assert.Fail("Accessing response headers should throw");
            }

            [TestMethod]
            public void VerifySingleQuotesNotEscapedInJSon()
            {
                using (TestUtil.RestoreStaticValueOnDispose(typeof(CustomDataContext), "PreserveChanges"))
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    CustomDataContext.PreserveChanges = true;

                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/Customers";
                    request.HttpMethod = "POST";
                    request.RequestContentType = UnitTestsUtil.JsonLightMimeType;

                    request.SetRequestStreamAsText(@"{ 
                        ""@odata.type"": ""AstoriaUnitTests.Stubs.Customer"", 
                        ID : -1, Name : ""ABC\'DEF'GHI"" 
                    }");

                    request.SendRequest();

                    request.RequestUriString = "/Customers";
                    request.HttpMethod = "GET";
                    request.Accept = UnitTestsUtil.JsonLightMimeType;
                    request.SendRequest();
                    string responsePayload = request.GetResponseStreamAsText();
                    Assert.IsFalse(responsePayload.Contains("\\\'"));
                    Assert.IsTrue(responsePayload.Contains("ABC\'DEF\'GHI"));
                }
            }

            [TestMethod, Variation("Blob Client: We need to throw better error message when the client tries to SetSaveStream() on an entity that is in Deleted state.")]
            public void ShouldThrowWhenSetSaveStreamOnDeletedEntity()
            {
                DataServiceContext ctx = new DataServiceContext(new Uri("http://somehost/service.svc"));
                object entity = new Customer();
                ctx.AttachTo("MySet", entity);
                ctx.DeleteObject(entity);
                try
                {
                    ctx.SetSaveStream(entity, new MemoryStream(), true, "customType/customSubType", "slug");
                    Assert.Fail("Exception expected but received none.");
                }
                catch (DataServiceClientException e)
                {
                    string expectedMsg = DataServicesClientResourceUtil.GetString("Context_SetSaveStreamOnInvalidEntityState", "Deleted");
                    Assert.AreEqual(expectedMsg, e.Message);
                }
            }

            [TestMethod, Variation("Server resource string: RequestQueryProcessor_QueryNoOptionsApplicable should include $select as one of the query options not applicable")]
            public void RequestQueryProcessorQueryNoOptionsApplicableShouldBeCorrect()
            {
                string errorMsg = DataServicesResourceUtil.GetString("RequestQueryProcessor_QueryNoOptionsApplicable");
                TestUtil.AssertContains(errorMsg, "$select");
                TestUtil.AssertContains(errorMsg, "$expand");
                TestUtil.AssertContains(errorMsg, "$filter");
                TestUtil.AssertContains(errorMsg, "$orderby");
                TestUtil.AssertContains(errorMsg, "$count");
                TestUtil.AssertContains(errorMsg, "$skip");
                TestUtil.AssertContains(errorMsg, "$skiptoken");
                TestUtil.AssertContains(errorMsg, "$top");
            }

            #region IDSP: Returning an untyped IQueryable from GetQueryRootForResourceSet results in asserts and errors

            [TestMethod, Variation("IDSP: Returning an untyped IQueryable from GetQueryRootForResourceSet results in asserts and errors")]
            public void ShouldErrorWhenReturnUntypedIQueryableFromGetQueryRootForResourceSet()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.ServiceType = typeof(TestService33);
                    request.RequestUriString = "/MySet?$top=1";
                    request.HttpMethod = "GET";
                    Exception e = TestUtil.RunCatching(request.SendRequest);
                    Assert.IsNotNull(e, "Expecting exception but received none.");

                    string expectedMsg = DataServicesResourceUtil.GetString("DataServiceProviderWrapper_InvalidQueryRootType", TestService33.mySet.Name, typeof(IQueryable<TestEntity33>).FullName);
                    Assert.AreEqual(expectedMsg, e.InnerException.Message);
                }
            }

            private class TestQuery33 : IQueryable
            {
                public TestQuery33(IQueryProvider provider)
                {
                    this.Provider = provider;
                    this.Expression = new TestEntity33[] { }.AsQueryable().Expression;
                }

                #region IQueryable Members

                public Type ElementType
                {
                    get { return typeof(TestEntity33); }
                }

                public System.Linq.Expressions.Expression Expression
                {
                    get;
                    set;
                }

                public IQueryProvider Provider
                {
                    get;
                    private set;
                }

                #endregion

                #region IEnumerable Members

                public System.Collections.IEnumerator GetEnumerator()
                {
                    return new TestEntity33[] { }.GetEnumerator();
                }

                #endregion
            }

            private class TestQueryProvider33 : IQueryProvider
            {
                #region IQueryProvider Members

                public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
                {
                    throw new NotImplementedException();
                }

                public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
                {
                    return new TestQuery33(this);
                }

                public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
                {
                    return default(TResult);
                }

                public object Execute(System.Linq.Expressions.Expression expression)
                {
                    return null;
                }

                #endregion
            }

            private class TestEntity33
            {
                public int Id
                {
                    get;
                    set;
                }
            }

            /// <summary>
            /// IDSQP.GetQueryRootForResourceSet() implementation which returns a non-generic IQueryable that would trigger the failure
            /// </summary>
            private class TestService33 : DataService<TestContext33>, IServiceProvider, p.IDataServiceQueryProvider, p.IDataServiceMetadataProvider
            {
                internal static p.ResourceType myType;
                internal static p.ResourceSet mySet;

                static TestService33()
                {
                    myType = new p.ResourceType(typeof(TestEntity33), p.ResourceTypeKind.EntityType, null, "foo", "MyEntityType", false);
                    myType.AddProperty(new p.ResourceProperty("Id", p.ResourcePropertyKind.Key | p.ResourcePropertyKind.Primitive, p.ResourceType.GetPrimitiveResourceType(typeof(int))));
                    myType.SetReadOnly();
                    mySet = new p.ResourceSet("MySet", myType);
                    mySet.SetReadOnly();
                }

                public static void InitializeService(DataServiceConfiguration configuration)
                {
                    configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
                    configuration.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                    configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                    configuration.UseVerboseErrors = OpenWebDataServiceHelper.ForceVerboseErrors;
                }

                #region IServiceProvider Members

                public object GetService(Type serviceType)
                {
                    if (serviceType.IsAssignableFrom(this.GetType()))
                        return this;
                    return null;
                }

                #endregion

                object p.IDataServiceQueryProvider.CurrentDataSource
                {
                    get
                    {
                        return new TestContext33();
                    }
                    set
                    {
                        throw new NotImplementedException();
                    }
                }

                object p.IDataServiceQueryProvider.GetOpenPropertyValue(object target, string propertyName)
                {
                    throw new NotImplementedException();
                }

                IEnumerable<KeyValuePair<string, object>> p.IDataServiceQueryProvider.GetOpenPropertyValues(object target)
                {
                    throw new NotImplementedException();
                }

                object p.IDataServiceQueryProvider.GetPropertyValue(object target, p.ResourceProperty resourceProperty)
                {
                    throw new NotImplementedException();
                }

                IQueryable p.IDataServiceQueryProvider.GetQueryRootForResourceSet(p.ResourceSet resourceSet)
                {
                    return new TestQuery33(new TestQueryProvider33());
                }

                p.ResourceType p.IDataServiceQueryProvider.GetResourceType(object target)
                {
                    return myType;
                }

                object p.IDataServiceQueryProvider.InvokeServiceOperation(p.ServiceOperation serviceOperation, object[] parameters)
                {
                    throw new NotImplementedException();
                }

                bool p.IDataServiceQueryProvider.IsNullPropagationRequired
                {
                    get { return false; }
                }

                string p.IDataServiceMetadataProvider.ContainerName
                {
                    get { return "foo"; }
                }

                string p.IDataServiceMetadataProvider.ContainerNamespace
                {
                    get { return "bar"; }
                }

                IEnumerable<p.ResourceType> p.IDataServiceMetadataProvider.GetDerivedTypes(p.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                p.ResourceAssociationSet p.IDataServiceMetadataProvider.GetResourceAssociationSet(p.ResourceSet resourceSet, p.ResourceType resourceType, p.ResourceProperty resourceProperty)
                {
                    throw new NotImplementedException();
                }

                bool p.IDataServiceMetadataProvider.HasDerivedTypes(p.ResourceType resourceType)
                {
                    return false;
                }

                IEnumerable<p.ResourceSet> p.IDataServiceMetadataProvider.ResourceSets
                {
                    get { throw new NotImplementedException(); }
                }

                IEnumerable<p.ServiceOperation> p.IDataServiceMetadataProvider.ServiceOperations
                {
                    get { throw new NotImplementedException(); }
                }

                bool p.IDataServiceMetadataProvider.TryResolveResourceSet(string name, out p.ResourceSet resourceSet)
                {
                    resourceSet = null;
                    if (name == mySet.Name)
                    {
                        resourceSet = mySet;
                        return true;
                    }

                    return false;
                }

                bool p.IDataServiceMetadataProvider.TryResolveResourceType(string name, out p.ResourceType resourceType)
                {
                    resourceType = null;
                    if (name == myType.FullName)
                    {
                        resourceType = myType;
                        return true;
                    }

                    return false;
                }

                bool p.IDataServiceMetadataProvider.TryResolveServiceOperation(string name, out p.ServiceOperation serviceOperation)
                {
                    serviceOperation = null;
                    return false;
                }

                IEnumerable<p.ResourceType> p.IDataServiceMetadataProvider.Types
                {
                    get { throw new NotImplementedException(); }
                }
            }

            private class TestContext33
            {
            }

            #endregion

            [TestMethod, Variation("Wrong type name in an error message when non-existing property is referenced in a query")]
            public void MetadataBinderPropertyNotDeclaredShouldBeCorrect()
            {
                using (CustomRowBasedContext.CreateChangeScope())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(CustomRowBasedContext);
                    request.HttpMethod = "GET";
                    request.RequestUriString = "/Customers?$filter=foo eq 'bar'";
                    Exception e = TestUtil.RunCatching(request.SendRequest);
                    Assert.IsNotNull(e);

                    string expectedErrorMsg = ODataLibResourceUtil.GetString("MetadataBinder_PropertyNotDeclared", CustomRowBasedContext.CustomerFullName, "foo", 0);
                    Assert.AreEqual(expectedErrorMsg, e.InnerException.Message);
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("IDSP: Spaces in a property name causes entity serialization to fail and not respond, but works fine in $metadata serialization")]
            public void SpaceInPropertyNameShouldNotThrowDuringEntitySerialization()
            {
                DSPMetadata metadata = new DSPMetadata("TestContainer", "TestNamespace");
                p.ResourceType myType = metadata.AddEntityType("MyType", null, null, false);
                metadata.AddKeyProperty(myType, "ID", typeof(int));
                metadata.AddPrimitiveProperty(myType, "Property Name With Space", typeof(string));
                metadata.AddResourceSet("MySet", myType);

                DSPResource resource1 = new DSPResource(myType, new[] { new KeyValuePair<string, object>("ID", 111), new KeyValuePair<string, object>("Property Name With Space", "Property Value") });
                DSPContext context = new DSPContext();
                context.GetResourceSetEntities("MySet").Add(resource1);

                DSPServiceDefinition service = new DSPServiceDefinition() { Metadata = metadata, CreateDataSource = m => context };
                service.ForceVerboseErrors = true;
                using (TestWebRequest request = service.CreateForInProcessWcf())
                {
                    request.HttpMethod = "GET";
                    request.RequestUriString = "/$metadata";

                    request.SendRequest();
                    try
                    {
                        MetadataUtil.IsValidMetadata(request.GetResponseStream(), "");
                        Assert.Fail("Expect exception but received none.");
                    }
                    catch (Exception ex)
                    {
                        TestUtil.AssertContains(ex.Message, "InvalidName : The specified name is not allowed: 'Property Name With Space'.");
                    }

                    request.RequestUriString = "/MySet";
                    request.Accept = "application/atom+xml";
                    request.SendRequest();
                    string response = request.GetResponseStreamAsText();
                    TestUtil.AssertContains(response, "Invalid name character in 'Property Name With Space'. The ' ' character, hexadecimal value 0x20, cannot be included in a name.");

                    request.RequestUriString = "/MySet(111)/Property Name With Space";
                    request.Accept = "application/xml";
                    request.SendRequest();
                    response = request.GetResponseStreamAsText();
                    TestUtil.AssertContains(response, "m:value");
                }
            }

            #region IDSMP: Server does not respond to service-document request if a type is not marked read-only
            [Ignore] // Remove Atom
            // [TestMethod, Variation("IDSMP: Server does not respond to service-document request if a type is not marked read-only")]
            public void ServerShouldRespondToServiceDocRequestIfTypeNotMarkedReadOnly()
            {
                foreach (string accept in new[] { UnitTestsUtil.JsonLightMimeType, UnitTestsUtil.MimeApplicationXml })
                {
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        OpenWebDataServiceHelper.GetServiceCustomizer.Value = (type) =>
                        {
                            if (type == typeof(p.IDataServiceMetadataProvider) || type == typeof(p.IDataServiceQueryProvider))
                            {
                                return new TestProvider();
                            }

                            return null;
                        };

                        request.DataServiceType = typeof(object);
                        request.HttpMethod = "GET";
                        request.RequestUriString = "/"; // service document
                        request.Accept = accept;
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNotNull(exception, "Expected exception, but none was thrown");
                        string expectedError = DataServicesResourceUtil.GetString("DataServiceProviderWrapper_ResourceContainerNotReadonly", TestProvider.mySet.Name);
                        TestUtil.AssertContains(request.GetResponseStreamAsText(), expectedError);
                    }
                }
            }

            public class TestProvider : p.IDataServiceMetadataProvider, p.IDataServiceQueryProvider
            {
                private static p.ResourceType myType = new p.ResourceType(typeof(Customer), p.ResourceTypeKind.EntityType, null, "MyTypeNamespace", "Type1", false);
                internal static p.ResourceSet mySet = new p.ResourceSet("Set1", myType);

                string p.IDataServiceMetadataProvider.ContainerName
                {
                    get { return "TestContainer"; }
                }

                string p.IDataServiceMetadataProvider.ContainerNamespace
                {
                    get { return "MyNamespace"; }
                }

                IEnumerable<p.ResourceType> p.IDataServiceMetadataProvider.GetDerivedTypes(p.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                p.ResourceAssociationSet p.IDataServiceMetadataProvider.GetResourceAssociationSet(p.ResourceSet resourceSet, p.ResourceType resourceType, p.ResourceProperty resourceProperty)
                {
                    throw new NotImplementedException();
                }

                bool p.IDataServiceMetadataProvider.HasDerivedTypes(p.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                IEnumerable<p.ResourceSet> p.IDataServiceMetadataProvider.ResourceSets
                {
                    get { return new[] { mySet }; }
                }

                IEnumerable<p.ServiceOperation> p.IDataServiceMetadataProvider.ServiceOperations
                {
                    get { throw new NotImplementedException(); }
                }

                bool p.IDataServiceMetadataProvider.TryResolveResourceSet(string name, out p.ResourceSet resourceSet)
                {
                    throw new NotImplementedException();
                }

                bool p.IDataServiceMetadataProvider.TryResolveResourceType(string name, out p.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                bool p.IDataServiceMetadataProvider.TryResolveServiceOperation(string name, out p.ServiceOperation serviceOperation)
                {
                    throw new NotImplementedException();
                }

                IEnumerable<p.ResourceType> p.IDataServiceMetadataProvider.Types
                {
                    get { return new[] { myType }; }
                }

                object p.IDataServiceQueryProvider.CurrentDataSource
                {
                    get
                    {
                        return new object();
                    }
                    set
                    {
                        throw new NotImplementedException();
                    }
                }

                object p.IDataServiceQueryProvider.GetOpenPropertyValue(object target, string propertyName)
                {
                    throw new NotImplementedException();
                }

                IEnumerable<KeyValuePair<string, object>> p.IDataServiceQueryProvider.GetOpenPropertyValues(object target)
                {
                    throw new NotImplementedException();
                }

                object p.IDataServiceQueryProvider.GetPropertyValue(object target, p.ResourceProperty resourceProperty)
                {
                    throw new NotImplementedException();
                }

                IQueryable p.IDataServiceQueryProvider.GetQueryRootForResourceSet(p.ResourceSet resourceSet)
                {
                    throw new NotImplementedException();
                }

                p.ResourceType p.IDataServiceQueryProvider.GetResourceType(object target)
                {
                    throw new NotImplementedException();
                }

                object p.IDataServiceQueryProvider.InvokeServiceOperation(p.ServiceOperation serviceOperation, object[] parameters)
                {
                    throw new NotImplementedException();
                }

                bool p.IDataServiceQueryProvider.IsNullPropagationRequired
                {
                    get { throw new NotImplementedException(); }
                }
            }

            #endregion

            [TestMethod, Variation("IDSMP: ResourceAssociationSetEnd constructor causes scan of ResourceType.Properties which forces all properties to be loaded")]
            public void ResourceAssociationSetEndShouldNotScanResourceTypeProperties()
            {
                TestUtil.RunCombinations(UnitTestsUtil.BooleanValues, UnitTestsUtil.BooleanValues,
                    (includeRelationshipLinksInResponse, getMethod) =>
                    {
                        DSPMetadata metadata = new DSPMetadata("TestContainer", "TestNamespace");

                        bool loadPropertiesDeclaredOnOrderCalled = false;
                        bool loadPropertiesDeclaredOnCustomerCalled = false;
                        bool loadPropertiesDeclaredOnOrderDetailCalled = false;

                        p.ResourceType customerType = metadata.AddEntityType("Customer", null, null, false);
                        p.ResourceType orderType = metadata.AddEntityType("Order", null, null, false);
                        p.ResourceType orderDetailType = metadata.AddEntityType("OrderDetail", null, null, false);

                        p.ResourceSet customerSet = metadata.AddResourceSet("Customers", customerType);
                        p.ResourceSet orderSet = metadata.AddResourceSet("Orders", orderType);
                        p.ResourceSet orderDetailSet = metadata.AddResourceSet("OrderDetails", orderDetailType);

                        ((DSPResourceType)orderType).LoadPropertiesDeclaredOnThisTypeDelegate = () =>
                        {
                            loadPropertiesDeclaredOnOrderCalled = true;
                            p.ResourceProperty id = new p.ResourceProperty("ID", p.ResourcePropertyKind.Key | p.ResourcePropertyKind.Primitive, p.ResourceType.GetPrimitiveResourceType(typeof(int)))
                            {
                                CanReflectOnInstanceTypeProperty = false,
                                CustomState = new ResourcePropertyAnnotation()
                            };

                            p.ResourceProperty orderDetails = new p.ResourceProperty("OrderDetails", p.ResourcePropertyKind.ResourceSetReference, orderDetailType)
                            {
                                CanReflectOnInstanceTypeProperty = false,
                            };

                            orderDetails.CustomState = new ResourcePropertyAnnotation()
                            {
                                ResourceAssociationSet = () =>
                                {
                                    return new p.ResourceAssociationSet("Order_OrderDetails",
                                        new p.ResourceAssociationSetEnd(orderSet, orderType, orderDetails),
                                        new p.ResourceAssociationSetEnd(orderDetailSet, orderDetailType, null));
                                }
                            };

                            return new[] { id, orderDetails };
                        };

                        ((DSPResourceType)orderDetailType).LoadPropertiesDeclaredOnThisTypeDelegate = () =>
                        {
                            loadPropertiesDeclaredOnOrderDetailCalled = true;
                            return new[]
                    {
                        new p.ResourceProperty("ID", p.ResourcePropertyKind.Key| p.ResourcePropertyKind.Primitive, p.ResourceType.GetPrimitiveResourceType(typeof(int)))
                        {
                            CanReflectOnInstanceTypeProperty = false
                        }
                    };
                        };

                        ((DSPResourceType)customerType).LoadPropertiesDeclaredOnThisTypeDelegate = () =>
                        {
                            loadPropertiesDeclaredOnCustomerCalled = true;
                            p.ResourceProperty id = new p.ResourceProperty("ID", p.ResourcePropertyKind.Key | p.ResourcePropertyKind.Primitive, p.ResourceType.GetPrimitiveResourceType(typeof(int)))
                            {
                                CanReflectOnInstanceTypeProperty = false,
                                CustomState = new ResourcePropertyAnnotation()
                            };

                            p.ResourceProperty orders = new p.ResourceProperty("Orders", p.ResourcePropertyKind.ResourceSetReference, orderType)
                            {
                                CanReflectOnInstanceTypeProperty = false,
                            };

                            orders.CustomState = new ResourcePropertyAnnotation()
                            {
                                ResourceAssociationSet = () =>
                                {
                                    return new p.ResourceAssociationSet("Customer_Orders",
                                        new p.ResourceAssociationSetEnd(customerSet, customerType, orders),
                                        new p.ResourceAssociationSetEnd(orderSet, orderType, null));
                                }
                            };

                            return new[] { id, orders };
                        };

                        DSPResource order1 = new DSPResource(orderType, new[] { new KeyValuePair<string, object>("ID", 111) });
                        DSPResource customer1 = new DSPResource(customerType, new[] { new KeyValuePair<string, object>("ID", 11), new KeyValuePair<string, object>("Orders", order1) });
                        DSPContext context = new DSPContext();
                        context.GetResourceSetEntities("Customers").Add(customer1);
                        context.GetResourceSetEntities("Orders").Add(order1);

                        DSPServiceDefinition service = new DSPServiceDefinition() { Metadata = metadata, Writable = true, CreateDataSource = m => context };
                        service.ForceVerboseErrors = true;
                        using (TestUtil.MetadataCacheCleaner())
                        using (OpenWebDataServiceHelper.IncludeRelationshipLinksInResponse.Restore())
                        using (TestWebRequest request = service.CreateForInProcessWcf())
                        {
                            OpenWebDataServiceHelper.IncludeRelationshipLinksInResponse.Value = includeRelationshipLinksInResponse;
                            if (getMethod)
                            {
                                request.HttpMethod = "GET";
                                request.RequestUriString = "/Customers(11)";
                            }
                            else
                            {
                                request.HttpMethod = "POST";
                                request.RequestUriString = "/Customers";
                                request.SetRequestStreamAsText("{ ID : 21 }");
                                request.RequestContentType = UnitTestsUtil.JsonLightMimeType;
                            }

                            request.SendRequest();
                            Assert.IsTrue(loadPropertiesDeclaredOnCustomerCalled);
                            Assert.IsTrue(loadPropertiesDeclaredOnOrderCalled);
                            Assert.IsFalse(loadPropertiesDeclaredOnOrderDetailCalled);
                        }
                    });
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("For EF poco cases (no proxies), the update operation succeeds even if the etag specified is incorrect")]
            public void UpdateOperationShouldFailIfEtagIsIncorrect()
            {
                Type contextType = typeof(AstoriaUnitTests.EFFK.CustomObjectContextPOCO);
                string uri = UnitTestsUtil.ConvertUri(contextType, "/Customers(1)");
                using (UnitTestsUtil.CreateChangeScope(contextType))
                {
                    string etag = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.AtomFormat, uri, contextType, null, "GET").ResponseETag;

                    // Update customers(1) so that the etag changes
                    string atomPayload = UnitTestsUtil.ConvertPayload(contextType, "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                        AtomUpdatePayloadBuilder.GetCategoryXml(typeof(CustomerWithBirthday).FullName) +
                            "<content type=\"application/xml\"><adsm:properties>" +
                                "<ads:Name>Foo</ads:Name>" +
                            "</adsm:properties></content>" +
                        "</entry>");

                    var headerValues = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("If-Match", etag) };
                    UnitTestsUtil.SendRequestAndVerifyXPath(
                        atomPayload,
                        uri,
                        null,
                        contextType,
                        UnitTestsUtil.AtomFormat,
                        "PATCH",
                        requestHeaders: headerValues,
                        verifyETag: true);

                    // verify that the etag has changed
                    string etag1 = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.AtomFormat, uri, contextType, null, "GET").ResponseETag;
                    Debug.Assert(etag != etag1, "the etag value must have changed");

                    // Now try to PATCH with the old etag value. The request must fail.
                    UnitTestsUtil.VerifyInvalidRequest(atomPayload, uri, contextType, UnitTestsUtil.AtomFormat, "PATCH", 412, headerValues.ToArray());

                    // Now try to PUT with the old etag value. The request must fail.
                    UnitTestsUtil.VerifyInvalidRequest(atomPayload, uri, contextType, UnitTestsUtil.AtomFormat, "PUT", 412, headerValues.ToArray());

                    // Now try to DELETE with the old etag value. The request must fail.
                    UnitTestsUtil.VerifyInvalidRequest(atomPayload, uri, contextType, UnitTestsUtil.AtomFormat, "DELETE", 412, headerValues.ToArray());
                }
            }

            [TestMethod, Variation("DataServiceException thrown through reflection point is not propagated")]
            public void DataServiceExceptionFromReflectionPointShouldPropagate()
            {
                Assert.AreEqual(HttpStatusCode.NotImplemented, GetRequestStatusCode("/SomeEntities", typeof(TestDataService34)));
            }

            public class TestDataService34 : DataService<TestDataContext34>, IServiceProvider, p.IDataServicePagingProvider
            {
                public p.IDataServiceMetadataProvider provider;

                public TestDataService34()
                {
                    p.ResourceType entityType = new p.ResourceType(typeof(SomeEntity), p.ResourceTypeKind.EntityType, null, "Foo", "SomeEntity", false);
                    p.ResourceProperty keyProperty = new p.ResourceProperty("ID", p.ResourcePropertyKind.Primitive | p.ResourcePropertyKind.Key, p.ResourceType.GetPrimitiveResourceType(typeof(int)));
                    entityType.AddProperty(keyProperty);

                    List<p.ResourceType> types = new List<p.ResourceType>() { entityType };

                    List<p.ResourceSet> containers = new List<p.ResourceSet>() { new p.ResourceSet("SomeEntities", entityType) };

                    List<p.ResourceAssociationSet> associationSets = new List<Microsoft.OData.Service.Providers.ResourceAssociationSet>(0);

                    this.provider = new CustomDataServiceProvider(containers, types, new List<p.ServiceOperation>(), associationSets, new TestDataContext34());
                }

                public static void InitializeService(DataServiceConfiguration configuration)
                {
                    configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
                    configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                    configuration.SetEntitySetPageSize("SomeEntities", 3);
                }

                object IServiceProvider.GetService(Type serviceType)
                {
                    if (serviceType == typeof(p.IDataServiceMetadataProvider) || serviceType == typeof(p.IDataServiceQueryProvider))
                    {
                        return this.provider;
                    }
                    else if (serviceType == typeof(p.IDataServicePagingProvider))
                    {
                        return this;
                    }

                    return null;
                }

                object[] p.IDataServicePagingProvider.GetContinuationToken(IEnumerator enumerator)
                {
                    throw new DataServiceException((int)HttpStatusCode.NotImplemented, "Not implemented");
                }

                void p.IDataServicePagingProvider.SetContinuationToken(IQueryable query, p.ResourceType resourceType, object[] continuationToken)
                {
                    throw new DataServiceException((int)HttpStatusCode.NotImplemented, "Not implemented");
                }
            }

            public class TestDataContext34
            {
                List<SomeEntity> someEntities;

                public TestDataContext34()
                {
                    this.someEntities = new List<SomeEntity>();
                    for (int i = 1; i < 10; i++)
                    {
                        this.someEntities.Add(new SomeEntity { ID = i });
                    }
                }

                public IQueryable<SomeEntity> SomeEntities
                {
                    get { return this.someEntities.AsQueryable(); }
                }

                private static string GetTypeName(object instance, out bool collection)
                {
                    collection = false;
                    if (instance is SomeEntity)
                    {
                        return "Foo.SomeEntity";
                    }

                    return null;
                }
            }

            public class SomeEntity
            {
                public int ID { get; set; }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Check that the etag is cleared for named streams in the client when the provider doesn't specify an etag value.")]
            public void EtagShouldBeClearedForNamedStreamsWhenNoEtagValuePresent()
            {
                TestUtil.RunCombinations(new[] { MergeOption.AppendOnly, MergeOption.OverwriteChanges, MergeOption.PreserveChanges, MergeOption.NoTracking },
                    mergeOption =>
                    {
                        TestDataService35.InitialETag = "\"foo\"";

                        DSPMetadata metadata = new DSPMetadata("TestContainer", "TestNamespace");
                        p.ResourceType myType = metadata.AddEntityType("StreamType", null, null, false);
                        metadata.AddKeyProperty(myType, "ID", typeof(int));
                        metadata.AddPrimitiveProperty(myType, "Name", typeof(string));
                        p.ResourceProperty streamProperty = metadata.AddNamedStreamProperty(myType, "Stream");
                        metadata.AddResourceSet("StreamSet", myType);

                        // Populate the service with one entity that has one named stream
                        DSPContext context = new DSPContext();
                        DSPMediaResourceStorage mediaStorage = new DSPMediaResourceStorage();

                        DSPResource resource = new DSPResource(myType);
                        resource.SetValue("ID", 1);
                        context.GetResourceSetEntities("StreamSet").Add(resource);

                        DSPMediaResource namedStream = mediaStorage.CreateMediaResource(resource, streamProperty);
                        namedStream.ContentType = "*/*";
                        namedStream.GetWriteStream().Write(new byte[] { 0 }, 0, 1);
                        namedStream.Etag = TestDataService35.InitialETag;

                        DSPServiceDefinition service = new DSPServiceDefinition
                        {
                            Metadata = metadata,
                            CreateDataSource = m => context,
                            DataServiceType = typeof(TestDataService35),
                            SupportNamedStream = true,
                            SupportMediaResource = true,
                            Writable = true,
                            MediaResourceStorage = mediaStorage
                        };

                        using (TestWebRequest request = service.CreateForInProcessWcf())
                        {
                            request.StartService();
                            var ctx = new DataServiceContext(new Uri(request.BaseUri), ODataProtocolVersion.V4);
                            //ctx.EnableAtom = true;
                            //ctx.Format.UseAtom();

                            // Get the entity
                            var streamEntity = ctx.CreateQuery<StreamEntity>("StreamSet").Where(s => s.ID == 1).Single();
                            Assert.AreEqual(streamEntity.Stream.ETag, TestDataService35.InitialETag, "Unexpected etag value");

                            // Update the entity - the stream provider will return a null ETag for any updated stream
                            ctx.SetSaveStream(streamEntity, "Stream", new MemoryStream(new byte[] { 1 }), true, new DataServiceRequestArgs { ContentType = "*/*" });
                            ctx.MergeOption = mergeOption;
                            ctx.SaveChanges();

                            // The ETag should be null
                            Assert.IsNull(streamEntity.Stream.ETag, "Expected ETag to be null");
                        }
                    });
            }

            /// <summary>
            /// Customizes GetStreamETag behavior by implementing IDataServiceStreamProvider2 and forwards all calls to the "real" stream provider except for
            /// GetStreamETag. 
            /// </summary>
            public class TestDataService35 : DSPDataService, p.IDataServiceStreamProvider2, IDisposable
            {
                p.IDataServiceStreamProvider2 streamProvider2;

                public override object GetService(Type serviceType)
                {
                    if (serviceType == typeof(p.IDataServiceStreamProvider2) || serviceType == typeof(p.IDataServiceStreamProvider))
                    {
                        if (this.streamProvider2 == null)
                        {
                            this.streamProvider2 = base.GetService(serviceType) as p.IDataServiceStreamProvider2;
                            Assert.IsNotNull(this.streamProvider2 != null, "Missing inner stream provider");
                        }
                        return this;
                    }
                    return base.GetService(serviceType);
                }

                public static string InitialETag { get; set; }

                Stream p.IDataServiceStreamProvider2.GetReadStream(object entity, p.ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
                {
                    return this.streamProvider2.GetReadStream(entity, streamProperty, etag, checkETagForEquality, operationContext);
                }

                Stream p.IDataServiceStreamProvider2.GetWriteStream(object entity, p.ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
                {
                    return this.streamProvider2.GetWriteStream(entity, streamProperty, etag, checkETagForEquality, operationContext);
                }

                string p.IDataServiceStreamProvider2.GetStreamContentType(object entity, p.ResourceProperty streamProperty, DataServiceOperationContext operationContext)
                {
                    return this.streamProvider2.GetStreamContentType(entity, streamProperty, operationContext);
                }

                Uri p.IDataServiceStreamProvider2.GetReadStreamUri(object entity, p.ResourceProperty streamProperty, DataServiceOperationContext operationContext)
                {
                    return this.streamProvider2.GetReadStreamUri(entity, streamProperty, operationContext);
                }

                string p.IDataServiceStreamProvider2.GetStreamETag(object entity, p.ResourceProperty streamProperty, DataServiceOperationContext operationContext)
                {
                    string newETag = this.streamProvider2.GetStreamETag(entity, streamProperty, operationContext);
                    if (newETag != InitialETag)
                    {
                        return null;
                    }
                    else
                    {
                        return InitialETag;
                    }
                }

                int p.IDataServiceStreamProvider.StreamBufferSize
                {
                    get { return this.streamProvider2.StreamBufferSize; }
                }

                Stream p.IDataServiceStreamProvider.GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
                {
                    return this.streamProvider2.GetReadStream(entity, etag, checkETagForEquality, operationContext);
                }

                Stream p.IDataServiceStreamProvider.GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
                {
                    return this.streamProvider2.GetWriteStream(entity, etag, checkETagForEquality, operationContext);
                }

                void p.IDataServiceStreamProvider.DeleteStream(object entity, DataServiceOperationContext operationContext)
                {
                    this.streamProvider2.DeleteStream(entity, operationContext);
                }

                string p.IDataServiceStreamProvider.GetStreamContentType(object entity, DataServiceOperationContext operationContext)
                {
                    return this.streamProvider2.GetStreamContentType(entity, operationContext);
                }

                Uri p.IDataServiceStreamProvider.GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
                {
                    return this.streamProvider2.GetReadStreamUri(entity, operationContext);
                }

                string p.IDataServiceStreamProvider.GetStreamETag(object entity, DataServiceOperationContext operationContext)
                {
                    return this.streamProvider2.GetStreamETag(entity, operationContext);
                }

                string p.IDataServiceStreamProvider.ResolveType(string entitySetName, DataServiceOperationContext operationContext)
                {
                    return this.streamProvider2.ResolveType(entitySetName, operationContext);
                }

                void IDisposable.Dispose()
                {
                    IDisposable innerProvider = this.streamProvider2 as IDisposable;

                    if (innerProvider != null)
                    {
                        innerProvider.Dispose();
                    }
                    this.streamProvider2 = null;
                }
            }

            public class StreamEntity
            {
                public int ID { get; set; }

                public string Name { get; set; }

                public DataServiceStreamLink Stream { get; set; }
            }
        }
    }
}

public class NoNamespaceType
{
    public int ID { get; set; }
    public string Name { get; set; }
}

namespace AstoriaUnitTests
{
    public class TestDerivedType : NoNamespaceType
    {
        public string DerivedProperty { get; set; }
    }
}
