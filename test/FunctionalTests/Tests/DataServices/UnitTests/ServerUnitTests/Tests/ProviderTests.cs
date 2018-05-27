//---------------------------------------------------------------------
// <copyright file="ProviderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.ServiceModel.Web;
    using System.Xml;
    using System.Xml.Linq;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using Microsoft.OData.Edm;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using DSP = Microsoft.OData.Service.Providers;
    using ocs = AstoriaUnitTests.ObjectContextStubs;
    using Microsoft.OData.Client;

    [TestModule]
    public partial class UnitTestModule : AstoriaTestModule
    {
        /// <summary>This is a test class for data provider functionality.</summary>
        [TestClass, TestCase]
        public class ProviderTests : AstoriaTestCase
        {
            /// <summary>
            /// Verifies that a reflection-based provider can handle types
            /// with no namespaces.
            /// </summary>
            [TestMethod, Variation]
            public void ProviderReflectionEmptyNamespacesTest()
            {
                const string modelText = "E1 = entitytype { ID string key; }; ES1: E1;";
                AdHocModel model = AdHocModel.ModelFromText(modelText);
                ModuleBuilder module = TestUtil.CreateModuleBuilder("ProviderReflectionEmptyNamespacesTest", true);
                TypeBuilder typeBuilder = model.GeneratePocoModel(module);
                string assemblyPath = TestUtil.SaveModule(module);
                Assembly assembly = Assembly.LoadFile(assemblyPath);
                Type dataType = assembly.GetType(typeBuilder.FullName, true);
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = dataType;
                    request.RequestUriString = "/$metadata";
                    request.SendRequest();
                    XmlDocument document = request.GetResponseStreamAsXmlDocument();
                    Trace.WriteLine(document.OuterXml);
                }
            }

            /// <summary>
            /// Verifies that a refection-based provider can handle navigation properties
            /// with types that are in a different namespace than the declaring type.
            /// </summary>
            [TestMethod, Variation]
            public void ProviderReflectionNavigationBetweenEntitiesInDifferenNamespacesTest()
            {
                const string modelText = @"
                                            NS1.E1 = entitytype { ID string key; }; ES1: E1;
                                            NS2.E2 = entitytype { ID string key; }; ES2: E2;
                                            NS1.AT1 = associationtype { end1 NS1.E1 1; end2 NS2.E2 *; };
                                            NS1.E1 = entitytype { navigation NP1 NS1.AT1 end1 NS2.E2; };
                                            NS2.E2 = entitytype { navigation NP2 NS1.AT1 end2 NS1.E1; };
                                        ";
                string populationModel = @"ES1 : [ { ID : '1' } ];
                                           ES2 : [ { ID : '2' } ];  ";
                AdHocModel model = AdHocModel.ModelFromText(modelText);
                ModuleBuilder module = TestUtil.CreateModuleBuilder("ProviderReflectionDifferentNamespacesTest", true);
                TypeBuilder typeBuilder = model.GeneratePocoModel(module, populationModel);
                string assemblyPath = TestUtil.SaveModule(module);
                Assembly assembly = Assembly.LoadFile(assemblyPath);
                Type dataType = assembly.GetType(typeBuilder.FullName, true);
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = dataType;
                    request.RequestUriString = "/$metadata";
                    request.SendRequest();
                    XDocument document = request.GetResponseStreamAsXDocument();
                    XNamespace nsConceptual = TestXmlConstants.EdmOasisNamespace;
                    var navigationElements = document.Descendants(nsConceptual + "NavigationProperty");
                    Assert.AreEqual(2, navigationElements.Count(), "Wrong number of navigation properties");
                    Assert.IsNotNull(navigationElements.SingleOrDefault(x => x.Attribute("Name").Value == "NP1" && x.Parent.Attribute("Name").Value == "E1"), "No Navigation property was found on E1");
                    Assert.IsNotNull(navigationElements.SingleOrDefault(x => x.Attribute("Name").Value == "NP2" && x.Parent.Attribute("Name").Value == "E2"), "No Navigation property was found on E2");
                    Trace.WriteLine(document.ToString());
                }
            }

            private static void ValidateGetDerivedTypes(DSP.IDataServiceMetadataProvider provider)
            {
                foreach (DSP.ResourceType type in provider.Types)
                {
                    DSP.ResourceType[] fromProvider = provider.GetDerivedTypes(type).OrderBy(t => t.FullName, StringComparer.InvariantCulture).ToArray();
                    DSP.ResourceType[] fromTest = GetDerivedTypes(type, provider.Types).OrderBy(t => t.FullName, StringComparer.InvariantCulture).ToArray();

                    Assert.AreEqual(fromTest.Length, fromProvider.Length);
                    for (int idx = 0; idx < fromProvider.Length; idx++)
                    {
                        Assert.AreEqual(fromTest[idx].FullName, fromProvider[idx].FullName);
                    }

                    Assert.AreEqual(fromTest.Length > 0, provider.HasDerivedTypes(type));
                }
            }

            private static IEnumerable<DSP.ResourceType> GetDerivedTypes(DSP.ResourceType resourceType, IEnumerable<DSP.ResourceType> types)
            {
                foreach (DSP.ResourceType type in types)
                {
                    if (type.BaseType != null && IsDerivedType(resourceType, type))
                    {
                        yield return type;
                    }
                }
            }

            private static bool IsDerivedType(DSP.ResourceType baseType, DSP.ResourceType derivedType)
            {
                while (derivedType.BaseType != null)
                {
                    if (derivedType.BaseType == baseType)
                    {
                        return true;
                    }
                    derivedType = derivedType.BaseType;
                }

                return false;
            }

            [TestMethod, Variation("Tests IDSP.GetDerivedTypes() and IDSP.HasDerivedType()")]
            public void BaseProviderGetDerivedTypesTest()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("ServiceModelData", ServiceModelData.Values));

                TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                {
                    ServiceModelData modelData = (ServiceModelData)values["ServiceModelData"];
                    object service = Activator.CreateInstance(typeof(OpenWebDataService<>).MakeGenericType(modelData.ServiceModelType));
                    service.GetType().GetMethod("AttachHost", BindingFlags.Public | BindingFlags.Instance).Invoke(service, new object[] { new TestServiceHost2() });
                    object context = ServiceModelData.InitializeAndGetContext(service);
                    object provider = ServiceModelData.CreateProvider(context, service);

                    ValidateGetDerivedTypes(provider as DSP.IDataServiceMetadataProvider);
                });
            }

            [TestMethod, Variation("Make sure DataServiceConfiguration.RegisterKnownType() cannot be called for IDSP")]
            public void IDSPRegisterKnownType()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.ServiceType = typeof(BadIDSPService);
                    request.RequestUriString = "/$metadata";
                    request.HttpMethod = "GET";
                    Exception e = TestUtil.RunCatching(request.SendRequest);
                    
                    TestUtil.AssertExceptionExpected(e, true);
                    Assert.IsInstanceOfType(e, typeof(InvalidOperationException));
                    string errorMsg = "Adding types through RegisterKnownType() is not supported for providers instantiated by the user.";
                    Assert.AreEqual(errorMsg, e.Message);
                }
            }

            private class BadIDSPService : DataService<CustomRowBasedContext>, IServiceProvider
            {
                public static void InitializeService(DataServiceConfiguration configuration)
                {
                    configuration.RegisterKnownType(typeof(RowComplexType));
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

            [TestMethod, Variation("IDSP: Service operations defined on the subclass of DataService<T> is not supported")]
            public void ServiceOpFromDataServiceNotSupportedOnIDSP()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.ServiceType = typeof(IDSPServiceWithServiceOperations);
                    request.RequestUriString = "/$metadata";
                    request.HttpMethod = "GET";
                    Exception e = TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(e, false);

                    string response = request.GetResponseStreamAsText();
                    Stream stream = IOUtil.CreateStream(response);
                    IEdmModel metadata = MetadataUtil.IsValidMetadata(stream, null);

                    Assert.AreEqual(0, metadata.EntityContainer.FindOperationImports("ServiceOperation1").Count());
                    Assert.AreEqual(0, metadata.EntityContainer.FindOperationImports("ServiceOperation2").Count());
                }
            }

            private class IDSPServiceWithServiceOperations : OpenWebDataService<CustomRowBasedContext>
            {
                [WebInvoke]
                public void ServiceOperation1() { }

                [WebGet]
                public int ServiceOperation2(short param)
                {
                    return param;
                }
            }

            private void MakeRequestForIDSPIntercepterTest(string entitySetName, string uri, string httpMethod, string payload, string contentType, string ifMatchEtag)
            {
                using (TestUtil.RestoreStaticValueOnDispose(typeof(IDSPServiceWithIntercepters), "QueryIntercepterInvokeCount"))
                using (TestUtil.RestoreStaticValueOnDispose(typeof(IDSPServiceWithIntercepters), "ChangeIntercepterInvokeCount"))
                using (TestUtil.RestoreStaticValueOnDispose(typeof(IDSPServiceWithIntercepters), "ChangeIntercepterAction"))
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    IDSPServiceWithIntercepters.QueryIntercepterInvokeCount = new Dictionary<string, int>();
                    IDSPServiceWithIntercepters.ChangeIntercepterInvokeCount = new Dictionary<string, int>();
                    IDSPServiceWithIntercepters.ChangeIntercepterAction = new Dictionary<string, UpdateOperations>();

                    request.ServiceType = typeof(IDSPServiceWithIntercepters);
                    request.RequestUriString = uri;
                    request.HttpMethod = httpMethod;
                    if (!string.IsNullOrEmpty(payload))
                    {
                        request.RequestContentType = contentType;
                        request.SetRequestStreamAsText(payload);
                    }

                    request.IfMatch = ifMatchEtag;

                    Exception e = TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(e, false);

                    if (string.IsNullOrEmpty(entitySetName))
                    {
                        Assert.AreEqual(0, IDSPServiceWithIntercepters.QueryIntercepterInvokeCount.Count);
                        Assert.AreEqual(0, IDSPServiceWithIntercepters.ChangeIntercepterInvokeCount.Count);
                        Assert.AreEqual(0, IDSPServiceWithIntercepters.ChangeIntercepterAction.Count);
                    }
                    else
                    {
                        if (httpMethod.ToUpper() == "GET")
                        {
                            XDocument response = request.GetResponseStreamAsXDocument();
                            int count = response.Descendants(UnitTestsUtil.AtomNamespace + "entry").Count();
                            Assert.AreEqual(count, IDSPServiceWithIntercepters.QueryIntercepterInvokeCount[entitySetName]);
                            Assert.AreEqual(0, IDSPServiceWithIntercepters.ChangeIntercepterInvokeCount.Count);
                            Assert.AreEqual(0, IDSPServiceWithIntercepters.ChangeIntercepterAction.Count);
                        }
                        else
                        {
                            if (httpMethod.ToUpper() == "POST")
                            {
                                Assert.AreEqual(0, IDSPServiceWithIntercepters.QueryIntercepterInvokeCount.Count);
                            }
                            else
                            {
                                Assert.AreEqual(1, IDSPServiceWithIntercepters.QueryIntercepterInvokeCount[entitySetName]);
                            }

                            Assert.AreEqual(1, IDSPServiceWithIntercepters.ChangeIntercepterInvokeCount[entitySetName]);
                            switch(httpMethod.ToUpper())
                            {
                                case "POST":
                                    Assert.AreEqual(UpdateOperations.Add, IDSPServiceWithIntercepters.ChangeIntercepterAction[entitySetName]);
                                    break;

                                case "PUT":
                                case "PATCH":
                                    Assert.AreEqual(UpdateOperations.Change, IDSPServiceWithIntercepters.ChangeIntercepterAction[entitySetName]);
                                    break;

                                case "DELETE":
                                    Assert.AreEqual(UpdateOperations.Delete, IDSPServiceWithIntercepters.ChangeIntercepterAction[entitySetName]);
                                    break;

                                default:
                                    Assert.Fail("Unexpected http method");
                                    break;
                            }
                        }
                    }
                }
            }

            private string GetEtag(Type serviceType, Type contextType, string uri)
            {
                using (TestUtil.RestoreStaticValueOnDispose(typeof(IDSPServiceWithIntercepters), "QueryIntercepterInvokeCount"))
                using (TestUtil.RestoreStaticValueOnDispose(typeof(IDSPServiceWithIntercepters), "ChangeIntercepterInvokeCount"))
                using (TestUtil.RestoreStaticValueOnDispose(typeof(IDSPServiceWithIntercepters), "ChangeIntercepterAction"))
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    IDSPServiceWithIntercepters.QueryIntercepterInvokeCount = new Dictionary<string, int>();
                    IDSPServiceWithIntercepters.ChangeIntercepterInvokeCount = new Dictionary<string, int>();
                    IDSPServiceWithIntercepters.ChangeIntercepterAction = new Dictionary<string, UpdateOperations>();

                    if (contextType != null)
                    {
                        request.DataServiceType = contextType;
                    }
                    else
                    {
                        request.ServiceType = serviceType;
                    }

                    request.RequestUriString = uri;
                    request.HttpMethod = "GET";

                    request.SendRequest();
                    return request.ResponseETag;
                }
            }

            // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
            [Ignore] // Remove Atom
            // [TestMethod, Variation("Test QueryIntercepters defined on the subclass of DataService<T>")]
            public void IDSPQueryIntercepterTest()
            {
                MakeRequestForIDSPIntercepterTest(null, "/$metadata", "GET", null, null, null);

                KeyValuePair<string, string>[] EntitySets = new KeyValuePair<string,string>[]
                {
                    new KeyValuePair<string, string>("Customers", "Customers?$format=atom"),
                    new KeyValuePair<string, string>("Orders", "Orders?$format=atom"),
                    new KeyValuePair<string, string>("Products", "Products?$format=atom"),
                    new KeyValuePair<string, string>("Regions", "Regions?$format=atom"),
                    new KeyValuePair<string, string>("OrderDetails", "OrderDetails?$format=atom"),
                    
                    new KeyValuePair<string, string>(null, "GetAllCustomersQueryable?$format=atom"),
                    new KeyValuePair<string, string>(null, "GetCustomerByIdQueryable?id=0&$format=atom"),
                    new KeyValuePair<string, string>(null, "GetAllCustomersEnumerable?$format=atom"),
                    new KeyValuePair<string, string>(null, "GetCustomerByIdDirectValue?id=0&$format=atom"),
                    
                    new KeyValuePair<string, string>(null, "GetAllOrdersQueryable?$format=atom"),
                    new KeyValuePair<string, string>(null, "GetOrderByIdQueryable?id=0&$format=atom"),
                    new KeyValuePair<string, string>(null, "GetAllOrdersEnumerable?$format=atom"),
                    new KeyValuePair<string, string>(null, "GetOrderByIdDirectValue?id=0&$format=atom"),

                    new KeyValuePair<string, string>("Orders", "GetCustomerByIdQueryable/Orders?id=0&$format=atom"),
                    new KeyValuePair<string, string>("Customers", "GetOrderByIdQueryable/Customer?id=0&$format=atom"),
                };
                
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("EntitySet", EntitySets));

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    KeyValuePair<string, string> pair = (KeyValuePair<string, string>)values["EntitySet"];
                    MakeRequestForIDSPIntercepterTest(pair.Key, "/" + pair.Value, "GET", null, null, null);
                });
            }

            [TestMethod, Variation("Test ChangeIntercepters defined on the subclass of DataService<T> for POST operation")]
            public void IDSPChangeIntercepterTestForPost()
            {
                string uri = "/Customers";
                string jsonPayLoad = "{" +
                                        "@odata.type: \"" + CustomRowBasedContext.CustomerFullName + "\"," +
                                        "Name: \"Foo\"," +
                                        "ID: 125," +
                                        "Address : " +
                                        "{" +
                                            "@odata.type: \"" + CustomRowBasedContext.AddressFullName + "\"," +
                                            "StreetAddress: \"Street Number, Street Address\"," +
                                            "City: \"Redmond\"," +
                                            "State: \"WA\"," +
                                            "PostalCode: \"98052\"" +
                                        "}" +
                                     "}";

                using (CustomRowBasedContext.CreateChangeScope())
                {
                    MakeRequestForIDSPIntercepterTest("Customers", uri, "POST", jsonPayLoad, UnitTestsUtil.JsonLightMimeType, null);
                }
            }

            [TestMethod, Variation("Test ChangeIntercepters defined on the subclass of DataService<T> for PUT/PATCH operation")]
            public void IDSPChangeIntercepterTestForPutMerge()
            {
                string jsonPayLoad = "{" +
                        "@odata.type: \"" + CustomRowBasedContext.CustomerFullName + "\"," +
                        "Name: \"Foo\"," +
                        "ID: 125," +
                        "Address : " +
                        "{" +
                            "@odata.type: \"" + CustomRowBasedContext.AddressFullName + "\"," +
                            "StreetAddress: \"Street Number, Street Address\"," +
                            "City: \"Redmond\"," +
                            "State: \"WA\"," +
                            "PostalCode: \"98052\"" +
                        "}" +
                     "}";

                string[] methods = new string[]
                {
                    "PUT",
                    "PATCH"
                };

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("HttpMethod", methods));

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    string method = (string)values["HttpMethod"];
                    string entityUri = "/Customers(0)";

                    using (CustomRowBasedContext.CreateChangeScope())
                    {
                        string etag = GetEtag(typeof(IDSPServiceWithIntercepters), null, entityUri);
                        MakeRequestForIDSPIntercepterTest("Customers", entityUri, method, jsonPayLoad, UnitTestsUtil.JsonLightMimeType, etag);
                    }
                });
            }

            [TestMethod, Variation("Test ChangeIntercepters defined on the subclass of DataService<T> for DELETE operation")]
            public void IDSPChangeIntercepterTestForDelete()
            {
                string[] EntitySets = new string[]
                {
                    "Customers",
                    "Orders",
                    "Products",
                    "Regions",
                    //"OrderDetails"
                };

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("EntitySet", EntitySets));

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    string entitySet = (string)values["EntitySet"];
                    string entityUri = "/" + entitySet + (entitySet == "OrderDetails" ? "(OrderID=0,ProductID=0)" : "(0)");

                    using (CustomRowBasedContext.CreateChangeScope())
                    {
                        string etag = GetEtag(typeof(IDSPServiceWithIntercepters), null, entityUri);
                        MakeRequestForIDSPIntercepterTest(entitySet, entityUri, "DELETE", null, null, etag);
                    }
                });
            }

            private class IDSPServiceWithIntercepters : OpenWebDataService<CustomRowBasedContext>
            {
                #region QueryIntercepters

                public static Dictionary<string, int> QueryIntercepterInvokeCount = null;
                public static Dictionary<string, int> ChangeIntercepterInvokeCount = null;
                public static Dictionary<string, UpdateOperations> ChangeIntercepterAction = null;

                private static bool ValidateEntity(RowEntityType entity, string expectedTypeName, string entitySetName, UpdateOperations? action)
                {
                    IServiceProvider isp = CustomRowBasedContext.GetInstance() as IServiceProvider;
                    DSP.IDataServiceQueryProvider provider = (DSP.IDataServiceQueryProvider)isp.GetService(typeof(DSP.IDataServiceQueryProvider));

                    DSP.ResourceType resourceType = provider.GetResourceType(entity);
                    bool derivedFromExpectedType = false;
                    while(resourceType != null)
                    {
                        if(resourceType.FullName == expectedTypeName)
                        {
                            derivedFromExpectedType = true;
                            break;
                        }

                        resourceType = resourceType.BaseType;
                    }

                    Assert.IsTrue(derivedFromExpectedType, string.Format("Entity instance is of type '{0}' which is not derived from '{1}'", entity.TypeName, expectedTypeName));

                    if (expectedTypeName == CustomRowBasedContext.OrderDetailsFullName)
                    {
                        Assert.AreEqual(typeof(RowEntityType), entity.GetType());
                    }
                    else
                    {
                        Assert.AreEqual(typeof(RowEntityTypeWithIDAsKey), entity.GetType());
                    }

                    if (action == null)
                    {
                        if (QueryIntercepterInvokeCount.ContainsKey(entitySetName))
                        {
                            QueryIntercepterInvokeCount[entitySetName]++;
                        }
                        else
                        {
                            QueryIntercepterInvokeCount[entitySetName] = 1;
                        }
                    }
                    else
                    {
                        if (ChangeIntercepterInvokeCount.ContainsKey(entitySetName))
                        {
                            ChangeIntercepterInvokeCount[entitySetName]++;
                        }
                        else
                        {
                            ChangeIntercepterInvokeCount[entitySetName] = 1;
                        }

                        ChangeIntercepterAction[entitySetName] = action.Value;
                    }

                    return true;
                }

                [QueryInterceptor("Customers")]
                public Expression<Func<RowEntityTypeWithIDAsKey, bool>> FilterCustomers()
                {
                    return (c) => ValidateEntity((RowEntityType)c, CustomRowBasedContext.CustomerFullName, "Customers", null);
                }

                [QueryInterceptor("Orders")]
                public Expression<Func<RowEntityTypeWithIDAsKey, bool>> FilterOrders()
                {
                    return (o) => ValidateEntity((RowEntityType)o, CustomRowBasedContext.OrderFullName, "Orders", null);
                }

                [QueryInterceptor("Products")]
                public Expression<Func<RowEntityTypeWithIDAsKey, bool>> FilterProducts()
                {
                    return (p) => ValidateEntity((RowEntityType)p, CustomRowBasedContext.ProductFullName, "Products", null);
                }

                [QueryInterceptor("Regions")]
                public Expression<Func<RowEntityTypeWithIDAsKey, bool>> FilterRegions()
                {
                    return (r) => ValidateEntity((RowEntityType)r, CustomRowBasedContext.RegionFullName, "Regions", null);
                }

                [QueryInterceptor("OrderDetails")]
                public Expression<Func<RowEntityType, bool>> FilterOrderDetails()
                {
                    return (od) => ValidateEntity(od, CustomRowBasedContext.OrderDetailsFullName, "OrderDetails", null);
                }

                #endregion QueryIntercepters

                #region ChangeIntercepters

                [ChangeInterceptor("Customers")]
                public void CheckCustomerUpdated(RowEntityTypeWithIDAsKey c, UpdateOperations action)
                {
                    ValidateEntity(c, CustomRowBasedContext.CustomerFullName, "Customers", action);
                }

                [ChangeInterceptor("Orders")]
                public void CheckOrderUpdated(RowEntityTypeWithIDAsKey o, UpdateOperations action)
                {
                    ValidateEntity(o, CustomRowBasedContext.OrderFullName, "Orders", action);
                }

                [ChangeInterceptor("Products")]
                public void CheckProductUpdated(RowEntityTypeWithIDAsKey p, UpdateOperations action)
                {
                    ValidateEntity(p, CustomRowBasedContext.ProductFullName, "Products", action);
                }

                [ChangeInterceptor("Regions")]
                public void CheckRegionUpdated(RowEntityTypeWithIDAsKey r, UpdateOperations action)
                {
                    ValidateEntity(r, CustomRowBasedContext.RegionFullName, "Regions", action);
                }

                [ChangeInterceptor("OrderDetails")]
                public void CheckOrderDetailUpdated(RowEntityType od, UpdateOperations action)
                {
                    ValidateEntity(od, CustomRowBasedContext.OrderDetailsFullName, "OrderDetails", action);
                }

                #endregion ChangeIntercepters
            }

            [TestMethod, Variation("Making sure our MEST context returns valid metadata.")]
            public void MestMetadataTest()
            {
                string targetPath = Path.Combine(TestUtil.GeneratedFilesLocation, MethodInfo.GetCurrentMethod().Name);
                IOUtil.EnsureEmptyDirectoryExists(targetPath);

                using (CustomRowBasedContext.CreateChangeScope())
                using (TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.MimeApplicationXml, "/$metadata", typeof(CustomRowBasedContext), null, "GET"))
                {
                    TestUtil.ClearMetadataCache();
                    string file = Path.Combine(targetPath, "MestMetadata.xml");
                    MetadataUtil.IsValidMetadata(request.GetResponseStream(), file);
                }
            }

            [TestMethod, Variation("Making sure we catch cases where the provider is returning wrong association sets")]
            public void ResourceAssociationSetTest()
            {
                string targetPath = Path.Combine(TestUtil.GeneratedFilesLocation, MethodInfo.GetCurrentMethod().Name);
                IOUtil.EnsureEmptyDirectoryExists(targetPath);

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("TestCases", new[] { 1, 2 }));

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    ResourceAssociationSetTestContext.TestCase = (int)values["TestCases"];

                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.DataServiceType = typeof(ResourceAssociationSetTestContext);
                        request.RequestUriString = "/$metadata";
                        request.HttpMethod = "GET";

                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception, ResourceAssociationSetTestContext.ExpectFailure);

                        if (exception != null)
                        {
                            Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidOperationException));
                            TestUtil.AssertContains(exception.InnerException.Message, ResourceAssociationSetTestContext.ErrorMessageShouldContain);
                        }
                        else
                        {
                            string file = Path.Combine(targetPath, "TestCase_" + ResourceAssociationSetTestContext.TestCase + "_metadata.xml");
                            MetadataUtil.IsValidMetadata(request.GetResponseStream(), file);
                        }
                    }
                });
            }

            public class ResourceAssociationSetTestContext : IServiceProvider
            {
                private DSP.IDataServiceMetadataProvider provider;
                public ResourceAssociationSetTestContext()
                {
                    DSP.ResourceType customerType = new DSP.ResourceType(
                        typeof(ClrBackingType),
                        DSP.ResourceTypeKind.EntityType,
                        null,
                        typeof(ClrBackingType).Namespace,
                        "Customer",
                        false);

                    DSP.ResourceType orderType = new DSP.ResourceType(
                        typeof(ClrBackingType),
                        DSP.ResourceTypeKind.EntityType,
                        null,
                        typeof(ClrBackingType).Namespace,
                        "Order",
                        false);

                    customerType.AddProperty(new DSP.ResourceProperty("ID", DSP.ResourcePropertyKind.Key | DSP.ResourcePropertyKind.Primitive, DSP.ResourceType.GetPrimitiveResourceType(typeof(int))));
                    orderType.AddProperty(new DSP.ResourceProperty("ID", DSP.ResourcePropertyKind.Key | DSP.ResourcePropertyKind.Primitive, DSP.ResourceType.GetPrimitiveResourceType(typeof(int))));
                    DSP.ResourceProperty customerOrders = new DSP.ResourceProperty("Orders", DSP.ResourcePropertyKind.ResourceSetReference, orderType);
                    customerType.AddProperty(customerOrders);
                    DSP.ResourceProperty orderCustomer = new DSP.ResourceProperty("Customer", DSP.ResourcePropertyKind.ResourceReference, customerType);
                    orderType.AddProperty(orderCustomer);
                    customerOrders.CanReflectOnInstanceTypeProperty = false;
                    orderCustomer.CanReflectOnInstanceTypeProperty = false;

                    DSP.ResourceSet customerSet = new Microsoft.OData.Service.Providers.ResourceSet("Customers", customerType);
                    DSP.ResourceSet orderSet = new Microsoft.OData.Service.Providers.ResourceSet("Orders", orderType);
                    DSP.ResourceSet goodCustomerSet = new Microsoft.OData.Service.Providers.ResourceSet("GoodCustomers", customerType);
                    DSP.ResourceSet goodOrderSet = new Microsoft.OData.Service.Providers.ResourceSet("GoodOrders", orderType);

                    List<DSP.ResourceSet> containers = new List<DSP.ResourceSet>()
                    {
                        customerSet,
                        orderSet,
                        goodCustomerSet,
                        goodOrderSet
                    };

                    List<DSP.ResourceType> types = new List<DSP.ResourceType>()
                    {
                        customerType,
                        orderType
                    };

                    List<DSP.ServiceOperation> operations = new List<DSP.ServiceOperation>(0);

                    List<DSP.ResourceAssociationSet> associationSets = null;

                    switch(TestCase)
                    {
                        case 1:
                            // We define 1 association set, the service should not throw
                            associationSets = new List<Microsoft.OData.Service.Providers.ResourceAssociationSet>()
                            {
                                new DSP.ResourceAssociationSet(
                                    "Customers_Orders",
                                    new DSP.ResourceAssociationSetEnd(customerSet, customerType, customerOrders),
                                    new DSP.ResourceAssociationSetEnd(orderSet, orderType, orderCustomer))
                            };
                            ExpectFailure = false;
                            ErrorMessageShouldContain = null;
                            break;

                        case 2:
                            // We define both association sets, the servie should not throw
                            associationSets = new List<Microsoft.OData.Service.Providers.ResourceAssociationSet>()
                            {
                                new DSP.ResourceAssociationSet(
                                    "Customers_Orders",
                                    new DSP.ResourceAssociationSetEnd(customerSet, customerType, customerOrders),
                                    new DSP.ResourceAssociationSetEnd(orderSet, orderType, orderCustomer)),
                                new DSP.ResourceAssociationSet(
                                    "GoodCustomers_GoodOrders",
                                    new DSP.ResourceAssociationSetEnd(goodCustomerSet, customerType, customerOrders),
                                    new DSP.ResourceAssociationSetEnd(goodOrderSet, orderType, orderCustomer))
                            };
                            ExpectFailure = false;
                            ErrorMessageShouldContain = null;
                            break;

                        default:
                            Assert.Fail("shouldn't be here");
                            break;
                    }

                    this.provider = new CustomDataServiceProvider(containers, types, operations, associationSets, this);
                }

                internal static int TestCase;
                internal static bool ExpectFailure;
                internal static string ErrorMessageShouldContain;

                IQueryable<ClrBackingType> Data
                {
                    get { return new ClrBackingType[0].AsQueryable(); }
                }

                #region IServiceProvider Members

                public object GetService(Type serviceType)
                {
                    if (serviceType == typeof(DSP.IDataServiceMetadataProvider) ||
                        serviceType == typeof(DSP.IDataServiceQueryProvider))
                    {
                        return this.provider;
                    }

                    return null;
                }

                #endregion

                public class ClrBackingType
                {
                    public int ID { get; set; }
                }
            }

            [TestMethod, Variation("Tests DataServiceConfiguration.EnableAccess()")]
            public void ConfigurationEnableAccessTest()
            {
                string targetPath = Path.Combine(TestUtil.GeneratedFilesLocation, MethodInfo.GetCurrentMethod().Name);
                IOUtil.EnsureEmptyDirectoryExists(targetPath);

                var VisibleOpenTypes = new List<List<string>>()
                {
                    null,
                    new List<string>() { "AstoriaUnitTests.Stubs.Address" },
                    new List<string>() { "*" }
                };

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("VisibleOpenTypes", VisibleOpenTypes));

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    using (OpenWebDataServiceHelper.EnableAccess.Restore())
                    {
                        OpenWebDataServiceHelper.EnableAccess.Value = (List<string>)values["VisibleOpenTypes"];
                        TestUtil.ClearConfiguration();

                        using (TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.MimeApplicationXml, "/$metadata", typeof(CustomRowBasedOpenTypesContext), null, "GET"))
                        {
                            string file = Path.Combine(targetPath, "Metadata.xml");
                            MetadataUtil.IsValidMetadata(request.GetResponseStream(), file);
                        }

                        using (TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.MimeApplicationXml, "/$metadata", typeof(CustomRowBasedOpenTypesContext), null, "GET"))
                        {
                            XDocument metadata = request.GetResponseStreamAsXDocument();

                            if (OpenWebDataServiceHelper.EnableAccess.Value == null)
                            {
                                var entityTypes = metadata.Descendants(UnitTestsUtil.EdmOasisNamespace + "EntityType").Select(e => e.Attribute("Name").Value);
                                Assert.IsTrue(entityTypes.Contains("Customer"), "Customer type should be made visible through the visible set.");
                                Assert.IsTrue(entityTypes.Contains("CustomerWithBirthday"), "CustomerWithBirthday type should be made visible through the visible set.");
                                Assert.IsTrue(entityTypes.Contains("Order"), "Order type should be made visible through the visible set.");
                                var complexTypes = metadata.Descendants(UnitTestsUtil.EdmOasisNamespace + "ComplexType").Select(e => e.Attribute("Name").Value);
                                Assert.IsFalse(complexTypes.Contains("Address"), "Address type should NOT be visible because it's unreachable via non-open properties.");
                                Assert.IsFalse(complexTypes.Contains("UnusedType"), "UnusedType type should NOT be visible because it's unreachable via non-open properties.");
                            }
                            else if (OpenWebDataServiceHelper.EnableAccess.Value[0] == "AstoriaUnitTests.Stubs.Address")
                            {
                                var entityTypes = metadata.Descendants(UnitTestsUtil.EdmOasisNamespace + "EntityType").Select(e => e.Attribute("Name").Value);
                                Assert.IsTrue(entityTypes.Contains("Customer"), "Customer type should be made visible through the visible set.");
                                Assert.IsTrue(entityTypes.Contains("CustomerWithBirthday"), "CustomerWithBirthday type should be made visible through the visible set.");
                                Assert.IsTrue(entityTypes.Contains("Order"), "Order type should be made visible through the visible set.");
                                var complexTypes = metadata.Descendants(UnitTestsUtil.EdmOasisNamespace + "ComplexType").Select(e => e.Attribute("Name").Value);
                                Assert.IsTrue(complexTypes.Contains("Address"), "Address type should be marked visible via DataServiceConfiguration.EnableAccess().");
                                Assert.IsFalse(complexTypes.Contains("UnusedType"), "UnusedType type should NOT be visible because it's unreachable via non-open properties.");
                            }
                            else if (OpenWebDataServiceHelper.EnableAccess.Value[0] == "*")
                            {
                                var entityTypes = metadata.Descendants(UnitTestsUtil.EdmOasisNamespace + "EntityType").Select(e => e.Attribute("Name").Value);
                                Assert.IsTrue(entityTypes.Contains("Customer"), "Customer type should be made visible through the visible set.");
                                Assert.IsTrue(entityTypes.Contains("CustomerWithBirthday"), "CustomerWithBirthday type should be made visible through the visible set.");
                                Assert.IsTrue(entityTypes.Contains("Order"), "Order type should be made visible through the visible set.");
                                var complexTypes = metadata.Descendants(UnitTestsUtil.EdmOasisNamespace + "ComplexType").Select(e => e.Attribute("Name").Value);
                                Assert.IsTrue(complexTypes.Contains("Address"), "Address type should be marked visible via DataServiceConfiguration.EnableAccess().");
                                Assert.IsTrue(complexTypes.Contains("UnusedType"), "UnusedType type should be marked visible via DataServiceConfiguration.EnableAccess().");
                            }
                        }
                    }
                });
            }

            public class ContextWithISP : IServiceProvider
            {
                public object GetService(Type serviceType)
                {
                    return this;
                }
            }

            public class ContextImplementingEveryInterface : IExpandProvider, DSP.IDataServiceStreamProvider, DSP.IDataServiceUpdateProvider, DSP.IDataServicePagingProvider
            {
                public ContextImplementingEveryInterface() { }

                #region IExpandProvider Members

                IEnumerable IExpandProvider.ApplyExpansions(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths)
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IConcurrencyProvider Members

                void DSP.IDataServiceUpdateProvider.SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IUpdatable Members

                object IUpdatable.CreateResource(string containerName, string fullTypeName)
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.GetResource(IQueryable query, string fullTypeName)
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.ResetResource(object resource)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.SetValue(object targetResource, string propertyName, object propertyValue)
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.GetValue(object targetResource, string propertyName)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.SetReference(object targetResource, string propertyName, object propertyValue)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.DeleteResource(object targetResource)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.SaveChanges()
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.ResolveResource(object resource)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.ClearChanges()
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IDataServiceStreamProvider Members

                int Microsoft.OData.Service.Providers.IDataServiceStreamProvider.StreamBufferSize
                {
                    get { throw new NotImplementedException(); }
                }

                Stream Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                Stream Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                void Microsoft.OData.Service.Providers.IDataServiceStreamProvider.DeleteStream(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                string Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetStreamContentType(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                Uri Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                string Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetStreamETag(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                string Microsoft.OData.Service.Providers.IDataServiceStreamProvider.ResolveType(string entitySetName, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IDataServicePagingProvider Members

                public object[] GetContinuationToken(IEnumerator enumerator)
                {
                    throw new NotImplementedException();
                }

                public void SetContinuationToken(IQueryable query, Microsoft.OData.Service.Providers.ResourceType resourceType, object[] continuationToken)
                {
                    throw new NotImplementedException();
                }

                #endregion
            }

            public class ContextImplementingJustIDSMP : DSP.IDataServiceMetadataProvider
            {
                #region IDataServiceMetadataProvider Members

                public string ContainerNamespace
                {
                    get { throw new NotImplementedException(); }
                }

                public string ContainerName
                {
                    get { throw new NotImplementedException(); }
                }

                public IEnumerable<Microsoft.OData.Service.Providers.ResourceSet> ResourceSets
                {
                    get { throw new NotImplementedException(); }
                }

                public IEnumerable<Microsoft.OData.Service.Providers.ResourceType> Types
                {
                    get { throw new NotImplementedException(); }
                }

                public IEnumerable<Microsoft.OData.Service.Providers.ServiceOperation> ServiceOperations
                {
                    get { throw new NotImplementedException(); }
                }

                public bool TryResolveResourceSet(string name, out Microsoft.OData.Service.Providers.ResourceSet resourceSet)
                {
                    throw new NotImplementedException();
                }

                public Microsoft.OData.Service.Providers.ResourceAssociationSet GetResourceAssociationSet(Microsoft.OData.Service.Providers.ResourceSet resourceSet, Microsoft.OData.Service.Providers.ResourceType resourceType, Microsoft.OData.Service.Providers.ResourceProperty resourceProperty)
                {
                    throw new NotImplementedException();
                }

                public bool TryResolveResourceType(string name, out Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                public IEnumerable<Microsoft.OData.Service.Providers.ResourceType> GetDerivedTypes(Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                public bool HasDerivedTypes(Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                public bool TryResolveServiceOperation(string name, out Microsoft.OData.Service.Providers.ServiceOperation serviceOperation)
                {
                    throw new NotImplementedException();
                }

                #endregion
            }

            public class ContextImplementingIDSPAndEverInterface : ContextImplementingEveryInterface, DSP.IDataServiceMetadataProvider, DSP.IDataServiceQueryProvider
            {
                #region IDataServiceMetadataProvider Members

                public string ContainerNamespace
                {
                    get { throw new NotImplementedException(); }
                }

                public string ContainerName
                {
                    get { throw new NotImplementedException(); }
                }

                public IEnumerable<Microsoft.OData.Service.Providers.ResourceSet> ResourceSets
                {
                    get { throw new NotImplementedException(); }
                }

                public IEnumerable<Microsoft.OData.Service.Providers.ResourceType> Types
                {
                    get { throw new NotImplementedException(); }
                }

                public IEnumerable<Microsoft.OData.Service.Providers.ServiceOperation> ServiceOperations
                {
                    get { throw new NotImplementedException(); }
                }

                public bool TryResolveResourceSet(string name, out Microsoft.OData.Service.Providers.ResourceSet resourceSet)
                {
                    throw new NotImplementedException();
                }

                public Microsoft.OData.Service.Providers.ResourceAssociationSet GetResourceAssociationSet(Microsoft.OData.Service.Providers.ResourceSet resourceSet, Microsoft.OData.Service.Providers.ResourceType resourceType, Microsoft.OData.Service.Providers.ResourceProperty resourceProperty)
                {
                    throw new NotImplementedException();
                }

                public bool TryResolveResourceType(string name, out Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                public IEnumerable<Microsoft.OData.Service.Providers.ResourceType> GetDerivedTypes(Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                public bool HasDerivedTypes(Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                public bool TryResolveServiceOperation(string name, out Microsoft.OData.Service.Providers.ServiceOperation serviceOperation)
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IDataServiceQueryProvider Members

                public object CurrentDataSource { get; set; }

                public bool IsNullPropagationRequired
                {
                    get { throw new NotImplementedException(); }
                }

                public IQueryable GetQueryRootForResourceSet(Microsoft.OData.Service.Providers.ResourceSet resourceSet)
                {
                    throw new NotImplementedException();
                }

                public Microsoft.OData.Service.Providers.ResourceType GetResourceType(object target)
                {
                    throw new NotImplementedException();
                }

                public object GetPropertyValue(object target, Microsoft.OData.Service.Providers.ResourceProperty resourceProperty)
                {
                    throw new NotImplementedException();
                }

                public object GetOpenPropertyValue(object target, string propertyName)
                {
                    throw new NotImplementedException();
                }

                public IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target)
                {
                    throw new NotImplementedException();
                }

                public object InvokeServiceOperation(Microsoft.OData.Service.Providers.ServiceOperation serviceOperation, object[] parameters)
                {
                    throw new NotImplementedException();
                }

                #endregion
            }

            private class DataServiceWithISPforV1ProviderWithUpdatable<T> : DataService<T>, IServiceProvider, IExpandProvider, DSP.IDataServiceStreamProvider, IUpdatable, DSP.IDataServicePagingProvider
            {
                public static void InitializeService(DataServiceConfiguration configuration)
                {
                    configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                }

                #region IServiceProvider Members

                object IServiceProvider.GetService(Type serviceType)
                {
                    if (serviceType == typeof(DSP.IDataServiceMetadataProvider) ||
                        serviceType == typeof(DSP.IDataServiceQueryProvider) ||
                        serviceType == typeof(DSP.IDataServiceUpdateProvider2) ||
                        serviceType == typeof(DSP.IDataServiceUpdateProvider) ||
                        serviceType == typeof(DSP.IDataServiceProviderBehavior) ||
                        serviceType == typeof(DSP.IDataServiceEntityFrameworkProvider) ||
                        serviceType == typeof(DSP.IDataServiceActionProvider))
                    {
                        return null;
                    }

                    return this;
                }

                #endregion

                #region IExpandProvider Members

                IEnumerable IExpandProvider.ApplyExpansions(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths)
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IUpdatable Members

                object IUpdatable.CreateResource(string containerName, string fullTypeName)
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.GetResource(IQueryable query, string fullTypeName)
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.ResetResource(object resource)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.SetValue(object targetResource, string propertyName, object propertyValue)
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.GetValue(object targetResource, string propertyName)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.SetReference(object targetResource, string propertyName, object propertyValue)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.DeleteResource(object targetResource)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.SaveChanges()
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.ResolveResource(object resource)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.ClearChanges()
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IDataServiceStreamProvider Members

                int Microsoft.OData.Service.Providers.IDataServiceStreamProvider.StreamBufferSize
                {
                    get { throw new NotImplementedException(); }
                }

                Stream Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                Stream Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                void Microsoft.OData.Service.Providers.IDataServiceStreamProvider.DeleteStream(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                string Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetStreamContentType(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                Uri Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                string Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetStreamETag(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                string Microsoft.OData.Service.Providers.IDataServiceStreamProvider.ResolveType(string entitySetName, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IDataServicePagingProvider Members

                public object[] GetContinuationToken(IEnumerator enumerator)
                {
                    throw new NotImplementedException();
                }

                public void SetContinuationToken(IQueryable query, Microsoft.OData.Service.Providers.ResourceType resourceType, object[] continuationToken)
                {
                    throw new NotImplementedException();
                }

                #endregion
            }

            private class DataServiceWithISPforV1Provider<T> : DataService<T>, IServiceProvider, IExpandProvider, DSP.IDataServiceStreamProvider, DSP.IDataServiceUpdateProvider, DSP.IDataServicePagingProvider
            {
                public static void InitializeService(DataServiceConfiguration configuration)
                {
                    configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                }

                #region IServiceProvider Members

                object IServiceProvider.GetService(Type serviceType)
                {
                    if (serviceType == typeof(DSP.IDataServiceMetadataProvider) ||
                        serviceType == typeof(DSP.IDataServiceQueryProvider) ||
                        serviceType == typeof(DSP.IDataServiceUpdateProvider2) ||
                        serviceType == typeof(DSP.IDataServiceProviderBehavior) ||
                        serviceType == typeof(DSP.IDataServiceEntityFrameworkProvider) ||
                        serviceType == typeof(DSP.IDataServiceActionProvider))
                    {
                        return null;
                    }

                    return this;
                }

                #endregion

                #region IExpandProvider Members

                IEnumerable IExpandProvider.ApplyExpansions(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths)
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IConcurrencyProvider Members

                void DSP.IDataServiceUpdateProvider.SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IUpdatable Members

                object IUpdatable.CreateResource(string containerName, string fullTypeName)
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.GetResource(IQueryable query, string fullTypeName)
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.ResetResource(object resource)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.SetValue(object targetResource, string propertyName, object propertyValue)
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.GetValue(object targetResource, string propertyName)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.SetReference(object targetResource, string propertyName, object propertyValue)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.DeleteResource(object targetResource)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.SaveChanges()
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.ResolveResource(object resource)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.ClearChanges()
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IDataServiceStreamProvider Members

                int Microsoft.OData.Service.Providers.IDataServiceStreamProvider.StreamBufferSize
                {
                    get { throw new NotImplementedException(); }
                }

                Stream Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                Stream Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                void Microsoft.OData.Service.Providers.IDataServiceStreamProvider.DeleteStream(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                string Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetStreamContentType(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                Uri Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                string Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetStreamETag(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                string Microsoft.OData.Service.Providers.IDataServiceStreamProvider.ResolveType(string entitySetName, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IDataServicePagingProvider Members

                public object[] GetContinuationToken(IEnumerator enumerator)
                {
                    throw new NotImplementedException();
                }

                public void SetContinuationToken(IQueryable query, Microsoft.OData.Service.Providers.ResourceType resourceType, object[] continuationToken)
                {
                    throw new NotImplementedException();
                }

                #endregion
            }

            private class DataServiceWithISPForIDSPSettings
            {
                public static bool ProvideIDataServiceQueryProvider = true;
            }

            private class DataServiceWithISPforIDSP<T> : 
                DataService<T>, 
                IServiceProvider, 
                IExpandProvider, 
                DSP.IDataServiceStreamProvider, 
                DSP.IDataServiceUpdateProvider, 
                DSP.IDataServicePagingProvider
            {
                public static void InitializeService(DataServiceConfiguration configuration)
                {
                    configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                }

                #region IServiceProvider Members

                public object GetService(Type serviceType)
                {
                    if (serviceType == typeof(DSP.IDataServiceQueryProvider) && !DataServiceWithISPForIDSPSettings.ProvideIDataServiceQueryProvider)
                    {
                        return null;
                    }

                    if(serviceType == typeof(DSP.IDataServiceMetadataProvider) ||
                       serviceType == typeof(DSP.IDataServiceQueryProvider))
                    {
                        return new IDSPwithISP();
                    }

                    if (serviceType == typeof(DSP.IDataServiceUpdateProvider2) ||
                        serviceType == typeof(DSP.IDataServiceProviderBehavior) ||
                        serviceType == typeof(DSP.IDataServiceEntityFrameworkProvider))
                    {
                        return null;
                    }

                    return this;
                }

                #endregion

                #region IExpandProvider Members

                IEnumerable IExpandProvider.ApplyExpansions(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths)
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IConcurrencyProvider Members

                void DSP.IDataServiceUpdateProvider.SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IUpdatable Members

                object IUpdatable.CreateResource(string containerName, string fullTypeName)
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.GetResource(IQueryable query, string fullTypeName)
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.ResetResource(object resource)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.SetValue(object targetResource, string propertyName, object propertyValue)
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.GetValue(object targetResource, string propertyName)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.SetReference(object targetResource, string propertyName, object propertyValue)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.DeleteResource(object targetResource)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.SaveChanges()
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.ResolveResource(object resource)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.ClearChanges()
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IDataServiceStreamProvider Members

                int Microsoft.OData.Service.Providers.IDataServiceStreamProvider.StreamBufferSize
                {
                    get { throw new NotImplementedException(); }
                }

                Stream Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                Stream Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                void Microsoft.OData.Service.Providers.IDataServiceStreamProvider.DeleteStream(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                string Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetStreamContentType(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                Uri Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                string Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetStreamETag(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                string Microsoft.OData.Service.Providers.IDataServiceStreamProvider.ResolveType(string entitySetName, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IDataServicePagingProvider Members

                public object[] GetContinuationToken(IEnumerator enumerator)
                {
                    throw new NotImplementedException();
                }

                public void SetContinuationToken(IQueryable query, Microsoft.OData.Service.Providers.ResourceType resourceType, object[] continuationToken)
                {
                    throw new NotImplementedException();
                }

                #endregion
            }

            private class DataServiceForIDSPwithISP<T> : DataService<T>, IServiceProvider
            {
                #region IServiceProvider Members

                object IServiceProvider.GetService(Type serviceType)
                {
                    if(serviceType == typeof(DSP.IDataServiceMetadataProvider) ||
                       serviceType == typeof(DSP.IDataServiceQueryProvider))
                    {
                        return new IDSPwithISP();
                    }

                    return null;
                }

                #endregion
            }

            private class DataServiceForIDSPImplementingEveryInterfaces<T> : DataService<T>, IServiceProvider
            {
                #region IServiceProvider Members

                object IServiceProvider.GetService(Type serviceType)
                {
                    if (serviceType == typeof(DSP.IDataServiceMetadataProvider) ||
                        serviceType == typeof(DSP.IDataServiceQueryProvider))
                    {
                        return new IDSPImplementingEveryInterfaces();
                    }

                    return null;
                }

                #endregion
            }

            private class IDSPwithISP : IServiceProvider, DSP.IDataServiceMetadataProvider, DSP.IDataServiceQueryProvider
            {
                #region IDataServiceProvider Members

                object Microsoft.OData.Service.Providers.IDataServiceQueryProvider.CurrentDataSource
                {
                    get { return new ContextWithISP(); }
                    set
                    {
                        throw new NotSupportedException();
                    }
                }

                bool Microsoft.OData.Service.Providers.IDataServiceQueryProvider.IsNullPropagationRequired
                {
                    get { throw new NotImplementedException(); }
                }

                string Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.ContainerNamespace
                {
                    get { throw new NotImplementedException(); }
                }

                string Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.ContainerName
                {
                    get { throw new NotImplementedException(); }
                }

                IEnumerable<Microsoft.OData.Service.Providers.ResourceSet> Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.ResourceSets
                {
                    get { throw new NotImplementedException(); }
                }

                IEnumerable<Microsoft.OData.Service.Providers.ResourceType> Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.Types
                {
                    get { throw new NotImplementedException(); }
                }

                IEnumerable<Microsoft.OData.Service.Providers.ServiceOperation> Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.ServiceOperations
                {
                    get { throw new NotImplementedException(); }
                }

                bool Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.TryResolveResourceSet(string name, out Microsoft.OData.Service.Providers.ResourceSet resourceSet)
                {
                    throw new NotImplementedException();
                }

                Microsoft.OData.Service.Providers.ResourceAssociationSet Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.GetResourceAssociationSet(Microsoft.OData.Service.Providers.ResourceSet resourceSet, Microsoft.OData.Service.Providers.ResourceType resourceType, Microsoft.OData.Service.Providers.ResourceProperty resourceProperty)
                {
                    throw new NotImplementedException();
                }

                IQueryable Microsoft.OData.Service.Providers.IDataServiceQueryProvider.GetQueryRootForResourceSet(Microsoft.OData.Service.Providers.ResourceSet resourceSet)
                {
                    throw new NotImplementedException();
                }

                bool Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.TryResolveResourceType(string name, out Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                Microsoft.OData.Service.Providers.ResourceType Microsoft.OData.Service.Providers.IDataServiceQueryProvider.GetResourceType(object target)
                {
                    throw new NotImplementedException();
                }

                IEnumerable<Microsoft.OData.Service.Providers.ResourceType> Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.GetDerivedTypes(Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                bool Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.HasDerivedTypes(Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                bool Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.TryResolveServiceOperation(string name, out Microsoft.OData.Service.Providers.ServiceOperation serviceOperation)
                {
                    throw new NotImplementedException();
                }

                object Microsoft.OData.Service.Providers.IDataServiceQueryProvider.GetPropertyValue(object target, Microsoft.OData.Service.Providers.ResourceProperty resourceProperty)
                {
                    throw new NotImplementedException();
                }

                object Microsoft.OData.Service.Providers.IDataServiceQueryProvider.GetOpenPropertyValue(object target, string propertyName)
                {
                    throw new NotImplementedException();
                }

                IEnumerable<KeyValuePair<string, object>> Microsoft.OData.Service.Providers.IDataServiceQueryProvider.GetOpenPropertyValues(object target)
                {
                    throw new NotImplementedException();
                }

                object Microsoft.OData.Service.Providers.IDataServiceQueryProvider.InvokeServiceOperation(Microsoft.OData.Service.Providers.ServiceOperation serviceOperation, object[] parameters)
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IServiceProvider Members

                object IServiceProvider.GetService(Type serviceType)
                {
                    if (serviceType == typeof(DSP.IDataServiceProviderBehavior) ||
                        serviceType == typeof(DSP.IDataServiceEntityFrameworkProvider))
                    {
                        return null;
                    }

                    return this;
                }

                #endregion
            }

            public class IDSPImplementingEveryInterfaces : DSP.IDataServiceMetadataProvider, DSP.IDataServiceQueryProvider, IExpandProvider, DSP.IDataServiceStreamProvider, DSP.IDataServiceUpdateProvider, DSP.IDataServicePagingProvider
            {
                public IDSPImplementingEveryInterfaces() { }

                #region IExpandProvider Members

                IEnumerable IExpandProvider.ApplyExpansions(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths)
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IConcurrencyProvider Members

                void DSP.IDataServiceUpdateProvider.SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IUpdatable Members

                object IUpdatable.CreateResource(string containerName, string fullTypeName)
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.GetResource(IQueryable query, string fullTypeName)
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.ResetResource(object resource)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.SetValue(object targetResource, string propertyName, object propertyValue)
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.GetValue(object targetResource, string propertyName)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.SetReference(object targetResource, string propertyName, object propertyValue)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.DeleteResource(object targetResource)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.SaveChanges()
                {
                    throw new NotImplementedException();
                }

                object IUpdatable.ResolveResource(object resource)
                {
                    throw new NotImplementedException();
                }

                void IUpdatable.ClearChanges()
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IDataServiceProvider Members

                object Microsoft.OData.Service.Providers.IDataServiceQueryProvider.CurrentDataSource
                {
                    get { return new ContextWithISP(); }
                    set
                    {
                        throw new NotSupportedException();
                    }
                }

                bool Microsoft.OData.Service.Providers.IDataServiceQueryProvider.IsNullPropagationRequired
                {
                    get { throw new NotImplementedException(); }
                }

                string Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.ContainerNamespace
                {
                    get { throw new NotImplementedException(); }
                }

                string Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.ContainerName
                {
                    get { throw new NotImplementedException(); }
                }

                IEnumerable<Microsoft.OData.Service.Providers.ResourceSet> Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.ResourceSets
                {
                    get { throw new NotImplementedException(); }
                }

                IEnumerable<Microsoft.OData.Service.Providers.ResourceType> Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.Types
                {
                    get { throw new NotImplementedException(); }
                }

                IEnumerable<Microsoft.OData.Service.Providers.ServiceOperation> Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.ServiceOperations
                {
                    get { throw new NotImplementedException(); }
                }

                bool Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.TryResolveResourceSet(string name, out Microsoft.OData.Service.Providers.ResourceSet resourceSet)
                {
                    throw new NotImplementedException();
                }

                Microsoft.OData.Service.Providers.ResourceAssociationSet Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.GetResourceAssociationSet(Microsoft.OData.Service.Providers.ResourceSet resourceSet, Microsoft.OData.Service.Providers.ResourceType resourceType, Microsoft.OData.Service.Providers.ResourceProperty resourceProperty)
                {
                    throw new NotImplementedException();
                }

                IQueryable Microsoft.OData.Service.Providers.IDataServiceQueryProvider.GetQueryRootForResourceSet(Microsoft.OData.Service.Providers.ResourceSet resourceSet)
                {
                    throw new NotImplementedException();
                }

                bool Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.TryResolveResourceType(string name, out Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                Microsoft.OData.Service.Providers.ResourceType Microsoft.OData.Service.Providers.IDataServiceQueryProvider.GetResourceType(object target)
                {
                    throw new NotImplementedException();
                }

                IEnumerable<Microsoft.OData.Service.Providers.ResourceType> Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.GetDerivedTypes(Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                bool Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.HasDerivedTypes(Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                bool Microsoft.OData.Service.Providers.IDataServiceMetadataProvider.TryResolveServiceOperation(string name, out Microsoft.OData.Service.Providers.ServiceOperation serviceOperation)
                {
                    throw new NotImplementedException();
                }

                object Microsoft.OData.Service.Providers.IDataServiceQueryProvider.GetPropertyValue(object target, Microsoft.OData.Service.Providers.ResourceProperty resourceProperty)
                {
                    throw new NotImplementedException();
                }

                object Microsoft.OData.Service.Providers.IDataServiceQueryProvider.GetOpenPropertyValue(object target, string propertyName)
                {
                    throw new NotImplementedException();
                }

                IEnumerable<KeyValuePair<string, object>> Microsoft.OData.Service.Providers.IDataServiceQueryProvider.GetOpenPropertyValues(object target)
                {
                    throw new NotImplementedException();
                }

                object Microsoft.OData.Service.Providers.IDataServiceQueryProvider.InvokeServiceOperation(Microsoft.OData.Service.Providers.ServiceOperation serviceOperation, object[] parameters)
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IDataServiceStreamProvider Members

                int Microsoft.OData.Service.Providers.IDataServiceStreamProvider.StreamBufferSize
                {
                    get { throw new NotImplementedException(); }
                }

                Stream Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                Stream Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                void Microsoft.OData.Service.Providers.IDataServiceStreamProvider.DeleteStream(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                string Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetStreamContentType(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                Uri Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                string Microsoft.OData.Service.Providers.IDataServiceStreamProvider.GetStreamETag(object entity, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                string Microsoft.OData.Service.Providers.IDataServiceStreamProvider.ResolveType(string entitySetName, DataServiceOperationContext operationContext)
                {
                    throw new NotImplementedException();
                }

                #endregion

                #region IDataServicePagingProvider Members

                public object[] GetContinuationToken(IEnumerator enumerator)
                {
                    throw new NotImplementedException();
                }

                public void SetContinuationToken(IQueryable query, Microsoft.OData.Service.Providers.ResourceType resourceType, object[] continuationToken)
                {
                    throw new NotImplementedException();
                }

                #endregion
            }

            private class DataServiceWithoutISP<T> : DataService<T>
            {
            }

            private class CustomProviderBehavior : DSP.IDataServiceProviderBehavior
            {
                public DSP.ProviderBehavior ProviderBehavior
                {
                    get { return new DSP.ProviderBehavior(DSP.ProviderQueryBehaviorKind.CustomProviderQueryBehavior); }
                }
            }

            private object GetService(Type interfaceType, object dataService, bool treatAsInternal)
            {
                PropertyInfo providerWrapperProperty = typeof(DataService<>).Assembly.GetType("Microsoft.OData.Service.IDataService").GetProperty("Provider");
                object providerWrapperInstance = providerWrapperProperty.GetValue(dataService, null);

                Type dataServiceWrapperType = typeof(DataService<>).Assembly.GetType("Microsoft.OData.Service.Providers.DataServiceProviderWrapper");

                if (!treatAsInternal)
                {
                    PropertyInfo providerBehaviorPropertyInfo = dataServiceWrapperType.GetProperty("ProviderBehavior", BindingFlags.Public | BindingFlags.Instance);
                    providerBehaviorPropertyInfo.GetSetMethod().Invoke(providerWrapperInstance, new object[] { new CustomProviderBehavior() });
                }

                MethodInfo getServiceMethod = dataServiceWrapperType.GetMethod("GetService", BindingFlags.NonPublic | BindingFlags.Instance);
                getServiceMethod = getServiceMethod.MakeGenericMethod(interfaceType);
                return getServiceMethod.Invoke(providerWrapperInstance, new object[0]);
            }

            private static Type[] KnownInterfaceTypes = new Type[]
            {
                typeof(IExpandProvider),
                typeof(DSP.IDataServiceUpdateProvider),
                typeof(DSP.IDataServiceStreamProvider),
                typeof(IUpdatable),
                typeof(DSP.IDataServicePagingProvider)
            };

            private void TurnOffPipelineAssertCheck(object service)
            {
                
                object pipeline = service.GetType().GetProperty("ProcessingPipeline").GetValue(service, null);
                FieldInfo skipAssertField = pipeline.GetType().GetField("SkipDebugAssert", BindingFlags.NonPublic | BindingFlags.Instance);
                if (skipAssertField != null)
                {
                    skipAssertField.SetValue(pipeline, true);
                }
            }

            [TestMethod, Variation("Test the algorithm to resolve interfaces")]
            public void GetInterfaceTestForIDSPProvider()
            {
                using (TestUtil.MetadataCacheCleaner())
                {
                    // For IDSP, if the service implements ISP, we try and resolve the interface there
                    {
                        object service = Activator.CreateInstance(typeof(DataServiceWithISPforIDSP<>).MakeGenericType(typeof(ContextWithISP)));
                        object context = ServiceModelData.InitializeAndGetContext(service);
                        object provider = ServiceModelData.CreateProvider(context, service);
                        Assert.IsTrue(provider.GetType() == typeof(IDSPwithISP), "The provider should be IDSP.");
                        TurnOffPipelineAssertCheck(service);
                        foreach (Type interfaceType in KnownInterfaceTypes)
                        {
                            object instance = GetService(interfaceType, service, false);
                            Assert.AreEqual(service, instance, "ISP on the service should resolve the interface " + interfaceType.ToString());
                        }
                    }

                    // For IDSP, if the service does NOT resolve the interface, we don't look for it on the provider's ISP
                    {
                        object service = Activator.CreateInstance(typeof(DataServiceForIDSPwithISP<>).MakeGenericType(typeof(ContextWithISP)));
                        object context = ServiceModelData.InitializeAndGetContext(service);
                        object provider = ServiceModelData.CreateProvider(context, service);
                        Assert.IsTrue(provider.GetType() == typeof(IDSPwithISP), "The provider should be IDSP.");
                        TurnOffPipelineAssertCheck(service);
                        foreach (Type interfaceType in KnownInterfaceTypes)
                        {
                            object instance = GetService(interfaceType, service, false);
                            Assert.AreNotEqual(provider, instance, "For IDSP, if the service does NOT resolve the interface, we don't look for it on the provider's ISP.");
                        }
                    }

                    // For IDSP, if the service does NOT resolve the interface, we don't look for it on the provider
                    {
                        object service = Activator.CreateInstance(typeof(DataServiceForIDSPImplementingEveryInterfaces<>).MakeGenericType(typeof(ContextWithISP)));
                        object context = ServiceModelData.InitializeAndGetContext(service);
                        object provider = ServiceModelData.CreateProvider(context, service);
                        Assert.IsTrue(provider.GetType() == typeof(IDSPImplementingEveryInterfaces), "The provider should be IDSP.");
                        TurnOffPipelineAssertCheck(service);
                        foreach (Type interfaceType in KnownInterfaceTypes)
                        {
                            object instance = GetService(interfaceType, service, false);
                            Assert.AreNotEqual(provider, instance, "For IDSP, if the service does NOT resolve the interface, we don't look for it on the provider.");
                        }
                    }

                    // For IDSP, if the service does NOT implement the IDSMP/IDSQP, we try resolve the interface on the T of the service
                    {
                        object service = Activator.CreateInstance(typeof(DataServiceWithoutISP<>).MakeGenericType(typeof(ContextImplementingIDSPAndEverInterface)));
                        object context = ServiceModelData.InitializeAndGetContext(service);
                        TurnOffPipelineAssertCheck(service);
                        foreach (Type interfaceType in KnownInterfaceTypes)
                        {
                            // Skip IUpdatable as the context implements IDataServiceUpdateProvider
                            if (interfaceType == typeof(IUpdatable))
                            {
                                continue;
                            }

                            object instance = GetService(interfaceType, service, false);
                            Assert.AreEqual(context, instance, "For IDSP, if the service does NOT resolve the interface, we look for it on the context.");
                        }
                    }
                }
            }

            [TestMethod, Variation("Test the algorithm to resolve interfaces")]
            public void GetInterfaceTestForReflectionProvider()
            {
                using (TestUtil.MetadataCacheCleaner())
                {
                    // For ReflectionServiceProvider, if the service implements ISP, we try and resolve the interface there - This one is for IUpdatable.
                    // Basically for this test, the service implements ISP and the context doesn't implement any interface directly on it.
                    {
                        object service = Activator.CreateInstance(typeof(DataServiceWithISPforV1ProviderWithUpdatable<>).MakeGenericType(typeof(ContextImplementingEveryInterface)));
                        service.GetType().GetMethod("AttachHost", BindingFlags.Public | BindingFlags.Instance).Invoke(service, new object[] { new TestServiceHost2() });
                        object context = ServiceModelData.InitializeAndGetContext(service);
                        object provider = ServiceModelData.CreateProvider(context, service);
                        Type reflectionServiceProvider = typeof(DataService<>).Assembly.GetType("Microsoft.OData.Service.Providers.ReflectionDataServiceProvider");
                        Assert.IsTrue(provider.GetType() == reflectionServiceProvider, "The context should be ReflectionDataServiceProvider.");
                        TurnOffPipelineAssertCheck(service);
                        foreach (Type interfaceType in KnownInterfaceTypes)
                        {
                            object instance = GetService(interfaceType, service, true);
                            if (interfaceType == typeof(DSP.IDataServiceUpdateProvider))
                            {
                                Assert.AreNotEqual(service, instance, "ISP on the service should return null for IDataServiceUpdateProvider" + interfaceType.ToString());
                                Assert.AreEqual(typeof(ContextImplementingEveryInterface), instance.GetType(), "This should fallback on the context for IDataServiceUpdateProvider" + interfaceType.ToString());
                            }
                            else
                            {
                                Assert.AreEqual(service, instance, "ISP on the service should resolve the interface " + interfaceType.ToString());
                            }
                        }
                    }

                    // For ReflectionServiceProvider, if the service implements ISP, we try and resolve the interface there - This one is for IDataServiceUpdateProvider
                    {
                        object service = Activator.CreateInstance(typeof(DataServiceWithISPforV1Provider<>).MakeGenericType(typeof(ContextImplementingEveryInterface)));
                        service.GetType().GetMethod("AttachHost", BindingFlags.Public | BindingFlags.Instance).Invoke(service, new object[] { new TestServiceHost2() });
                        object context = ServiceModelData.InitializeAndGetContext(service);
                        object provider = ServiceModelData.CreateProvider(context, service);
                        Type reflectionServiceProvider = typeof(DataService<>).Assembly.GetType("Microsoft.OData.Service.Providers.ReflectionDataServiceProvider");
                        Assert.IsTrue(provider.GetType() == reflectionServiceProvider, "The context should be ReflectionServiceProvider.");
                        TurnOffPipelineAssertCheck(service);
                        foreach (Type interfaceType in KnownInterfaceTypes)
                        {
                            object instance = GetService(interfaceType, service, true);
                            Assert.AreEqual(service, instance, "ISP on the service should resolve the interface " + interfaceType.ToString());
                        }
                    }

                    // If the service doesn't implement ISP, we try to resolve the interface on T
                    {
                        object service = Activator.CreateInstance(typeof(DataServiceWithoutISP<>).MakeGenericType(typeof(ContextImplementingEveryInterface)));
                        service.GetType().GetMethod("AttachHost", BindingFlags.Public | BindingFlags.Instance).Invoke(service, new object[] { new TestServiceHost2() });
                        object context = ServiceModelData.InitializeAndGetContext(service);
                        object provider = ServiceModelData.CreateProvider(context, service);
                        Type reflectionServiceProvider = typeof(DataService<>).Assembly.GetType("Microsoft.OData.Service.Providers.ReflectionDataServiceProvider");
                        Assert.IsTrue(provider.GetType() == reflectionServiceProvider, "The context should be ReflectionServiceProvider.");
                        TurnOffPipelineAssertCheck(service);
                        foreach (Type interfaceType in KnownInterfaceTypes)
                        {
                            object instance = GetService(interfaceType, service, true);
                            Assert.AreEqual(context, instance, "Should resolve the interface " + interfaceType.ToString() + " at the context T.");
                        }
                    }

                    // If the service doesn't implement ISP and we can't find it on T, make sure we don't try to call ISP over T
                    {
                        object service = Activator.CreateInstance(typeof(DataServiceWithoutISP<>).MakeGenericType(typeof(ContextWithISP)));
                        service.GetType().GetMethod("AttachHost", BindingFlags.Public | BindingFlags.Instance).Invoke(service, new object[] { new TestServiceHost2() });
                        object context = ServiceModelData.InitializeAndGetContext(service);
                        object provider = ServiceModelData.CreateProvider(context, service);
                        Type reflectionServiceProvider = typeof(DataService<>).Assembly.GetType("Microsoft.OData.Service.Providers.ReflectionDataServiceProvider");
                        Assert.IsTrue(provider.GetType() == reflectionServiceProvider, "The context should be ReflectionServiceProvider.");
                        TurnOffPipelineAssertCheck(service);
                        foreach (Type interfaceType in KnownInterfaceTypes)
                        {
                            object instance = GetService(interfaceType, service, true);
                            Assert.AreNotEqual(context, instance, "Should NOT resolve the interface " + interfaceType.ToString() + " through IServiceProvider on context T.");
                        }
                    }
                }
            }

            [TestMethod, Variation("Test the algorithm to resolve interfaces")]
            public void GetInterfaceTestForObjectContextProvider()
            {
                using (TestUtil.MetadataCacheCleaner())
                {
                    // For ObjectContextServiceProvider, we make sure IDataServiceStreamProvider is the only supported external provider
                    ServiceModelData.Northwind.EnsureDependenciesAvailable();
                    object service = Activator.CreateInstance(typeof(DataServiceWithISPforV1Provider<>).MakeGenericType(typeof(NorthwindModel.NorthwindContext)));
                    service.GetType().GetMethod("AttachHost", BindingFlags.Public | BindingFlags.Instance).Invoke(service, new object[] { new TestServiceHost2() });
                    object context = ServiceModelData.InitializeAndGetContext(service);
                    object provider = ServiceModelData.CreateProvider(context, service);
                    Type objectContextServiceProviderType = typeof(DataService<>).Assembly.GetType("Microsoft.OData.Service.Providers.EntityFrameworkDataServiceProvider");
                    Assert.IsTrue(provider.GetType() == objectContextServiceProviderType, "The context should be EntityFrameworkDataServiceProvider.");
                    TurnOffPipelineAssertCheck(service);
                    foreach (Type interfaceType in KnownInterfaceTypes)
                    {
                        object instance = GetService(interfaceType, service, true);
                        Assert.AreEqual(service, instance, "ISP on the service should resolve all interfaces.");
                    }
                }
            }

            [TestMethod, Variation("Test the algorithm to resolve interfaces - failure cases")]
            public void GetInterfaceTestFailures()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(DataServiceWithISPForIDSPSettings)))
                {
                    // If the data service's ISP returns IDSMP, it also must provider IDSQP
                    try
                    {
                        DataServiceWithISPForIDSPSettings.ProvideIDataServiceQueryProvider = false;
                        object service = Activator.CreateInstance(typeof(DataServiceWithISPforIDSP<>).MakeGenericType(typeof(ContextWithISP)));
                        object context = ServiceModelData.InitializeAndGetContext(service);
                        Assert.Fail("DataService with ISP returning IDSMP but not IDSQP should fail to create.");
                    }
                    catch (TargetInvocationException ex)
                    {
                        Assert.IsNotNull(ex.InnerException as InvalidOperationException, "Should have thrown InvalidOperationException.");
                    }

                    // If the data service does provide IDSMP, but the data source instance does, it also needs to provide IDSQP
                    try
                    {
                        object service = Activator.CreateInstance(typeof(DataServiceWithoutISP<>).MakeGenericType(typeof(ContextImplementingJustIDSMP)));
                        object context = ServiceModelData.InitializeAndGetContext(service);
                        Assert.Fail("If the data source provides IDSMP it also must provide IDSQP.");
                    }
                    catch (TargetInvocationException ex)
                    {
                        Assert.IsNotNull(ex.InnerException as InvalidOperationException, "Should have thrown InvalidOperationException.");
                    }
                }
            }

            #region EnumeratorDisposeDataContext
            public class EnumeratorDisposeDataContext
            {
                public class OwnOrder
                {
                    public int ID { get; set; }
                }

                [ETag("GuidValue")]
                public class OwnCustomer
                {
                    public int ID { get; set; }
                    public string Name { get; set; }
                    public OwnCustomer BestFriend { get; set; }
                    public Guid GuidValue { get; set; }
                    public IEnumerable<OwnOrder> Orders
                    {
                        get
                        {
                            return new OwnEnumerable<OwnOrder>(this.InternalOrders);
                        }
                    }

                    internal List<OwnOrder> InternalOrders { get; set; }

                    public OwnCustomer()
                    {
                        this.InternalOrders = new List<OwnOrder>();
                    }
                }

                public static int OpenedCount = 0;

                private class OwnEnumerator<T> : IEnumerator<T>
                {
                    private IEnumerator<T> inner;
                    public OwnEnumerator(IEnumerator<T> inner)
                    {
                        EnumeratorDisposeDataContext.OpenedCount++;
                        this.inner = inner;
                    }
                    public T Current
                    {
                        get { return this.inner.Current; }
                    }
                    public void Dispose()
                    {
                        Assert.IsTrue(EnumeratorDisposeDataContext.OpenedCount > 0);
                        EnumeratorDisposeDataContext.OpenedCount--;
                        this.inner.Dispose();
                    }
                    object IEnumerator.Current
                    {
                        get { return this.inner.Current; }
                    }
                    public bool MoveNext()
                    {
                        return this.inner.MoveNext();
                    }
                    public void Reset()
                    {
                        this.inner.Reset();
                    }
                }

                internal class OwnEnumerable<T> : IEnumerable<T>
                {
                    private IEnumerable<T> inner;
                    public OwnEnumerable(IEnumerable<T> inner)
                    {
                        this.inner = inner;
                    }

                    public IEnumerator<T> GetEnumerator()
                    {
                        return new OwnEnumerator<T>(this.inner.GetEnumerator());
                    }

                    IEnumerator IEnumerable.GetEnumerator()
                    {
                        return this.GetEnumerator();
                    }
                }

                private class OwnQueryable<T> : OwnEnumerable<T>, IOrderedQueryable<T>
                {
                    private IQueryable<T> inner;
                    public OwnQueryable(IQueryable<T> inner) : base(inner)
                    {
                        this.inner = inner;
                    }

                    public Type ElementType
                    {
                        get { return this.inner.ElementType; }
                    }

                    public Expression Expression
                    {
                        get { return this.inner.Expression; }
                    }

                    public IQueryProvider Provider
                    {
                        get { return new OwnQueryProvider(this.inner.Provider); }
                    }
                }

                private class OwnQueryProvider : IQueryProvider
                {
                    private IQueryProvider inner;
                    public OwnQueryProvider(IQueryProvider inner)
                    {
                        this.inner = inner;
                    }
                    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
                    {
                        return new OwnQueryable<TElement>(this.inner.CreateQuery<TElement>(expression));
                    }

                    private Type GetIQueryableOfT(Type t)
                    {
                        Type q = null;
                        while (t != null)
                        {
                            foreach (Type i in t.GetInterfaces())
                            {
                                if (i.GetGenericTypeDefinition() == typeof(IQueryable<>))
                                {
                                    return i;
                                }
                                q = GetIQueryableOfT(i);
                                if (q != null)
                                {
                                    return q;
                                }
                            }
                            t = t.BaseType;
                        }
                        return null;
                    }

                    public IQueryable CreateQuery(Expression expression)
                    {
                        IQueryable res = this.inner.CreateQuery(expression);
                        Type q = GetIQueryableOfT(res.GetType());
                        Type telement = q.GetGenericArguments()[0];
                        return (IQueryable)Activator.CreateInstance(typeof(OwnQueryable<>).MakeGenericType(telement), res);
                    }

                    public TResult Execute<TResult>(Expression expression)
                    {
                        return this.inner.Execute<TResult>(expression);
                    }

                    public object Execute(Expression expression)
                    {
                        return this.inner.Execute(expression);
                    }
                }

                public IQueryable<OwnCustomer> Customers
                {
                    get
                    {
                        return new OwnQueryable<OwnCustomer>(_customers.AsQueryable());
                    }
                }

                public IQueryable<OwnOrder> Orders
                {
                    get
                    {
                        return new OwnQueryable<OwnOrder>(_orders.AsQueryable());
                    }
                }

                private static List<OwnCustomer> _customers;
                private static List<OwnOrder> _orders;

                static EnumeratorDisposeDataContext()
                {
                    _customers = new List<OwnCustomer>();
                    _customers.Add(new OwnCustomer() { ID = 0, Name = "Customer 0", GuidValue = new Guid() });
                    _customers.Add(new OwnCustomer() { ID = 1, Name = "Customer 1", GuidValue = new Guid() });
                    _customers[1].BestFriend = _customers[0];

                    _orders = new List<OwnOrder>();
                    _orders.Add(new OwnOrder() { ID = 0 });
                    _orders.Add(new OwnOrder() { ID = 1 });

                    _customers[1].InternalOrders.AddRange(_orders);
                }

                public static void VerifyEnumeratorDisposed(string uri, string format)
                {
                    VerifyEnumeratorDisposed(uri, format, null, null);
                }

                public static void VerifyEnumeratorDisposed(string uri, string format, string ifMatch, string ifNoneMatch)
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                    {
                        request.DataServiceType = typeof(EnumeratorDisposeDataContext);

                        EnumeratorDisposeDataContext.OpenedCount = 0;
                        request.Accept = format;
                        request.RequestUriString = uri;
                        request.IfMatch = ifMatch;
                        request.IfNoneMatch = ifNoneMatch;
                        TestUtil.RunCatching(request.SendRequest);
                        request.GetResponseStreamAsText();
                        Assert.IsTrue(EnumeratorDisposeDataContext.OpenedCount == 0, "The enumerator was not disposed after the request.");
                    }
                }
            }
            #endregion
            // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
            // [TestMethod, Variation("Test that we correctly Dispose query results.")]
            public void EnumeratorDisposeAfterQuery()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                TestUtil.RunCombinatorialEngineFail(engine, values =>
                {
                    string format = (string)values["Format"];
                    // Normal query
                    using (TestUtil.MetadataCacheCleaner())
                    {
                        // Simple
                        EnumeratorDisposeDataContext.VerifyEnumeratorDisposed("/Customers", format);
                        // Empty result (204)
                        EnumeratorDisposeDataContext.VerifyEnumeratorDisposed("/Customers(0)/BestFriend", format);
                        // Existing result
                        EnumeratorDisposeDataContext.VerifyEnumeratorDisposed("/Customers(1)/BestFriend", format);
                        // Non-existing result - error
                        EnumeratorDisposeDataContext.VerifyEnumeratorDisposed("/Customers(2)/BestFriend", format);
                        // IfNoneMatch=* - always return 304
                        EnumeratorDisposeDataContext.VerifyEnumeratorDisposed("/Customers(1)/BestFriend", format, null, "*");
                        // Error - no enumerator created
                        EnumeratorDisposeDataContext.VerifyEnumeratorDisposed("/Customers", format, null, "*");
                        // Nested enumerations
                        EnumeratorDisposeDataContext.VerifyEnumeratorDisposed("/Customers?$expand=Orders", format);
                        // Nested enumerations with nav. property traversal
                        EnumeratorDisposeDataContext.VerifyEnumeratorDisposed("/Customers(1)/Orders", format);
                        // Nested enumerations with empty list
                        EnumeratorDisposeDataContext.VerifyEnumeratorDisposed("/Customers(0)/Orders", format);
                        // Links on empty collection
                        EnumeratorDisposeDataContext.VerifyEnumeratorDisposed("/Customers(0)/Orders$ref", "*/*");
                        // Links on non-empty collection
                        EnumeratorDisposeDataContext.VerifyEnumeratorDisposed("/Customers(1)/Orders/$ref", "*/*");
                        // Single link
                        EnumeratorDisposeDataContext.VerifyEnumeratorDisposed("/Customers(1)/BestFriend/$ref", "*/*");
                        // Value
                        EnumeratorDisposeDataContext.VerifyEnumeratorDisposed("/Customers(1)/Name/$value", "*/*");
                        EnumeratorDisposeDataContext.VerifyEnumeratorDisposed("/Customers(1)/BestFriend/Name/$value", "*/*");
                        EnumeratorDisposeDataContext.VerifyEnumeratorDisposed("/Customers(1)/Orders(0)/ID/$value", "*/*");
                    }

                    // Force usage of BasicExpandProvider
                    using (TestUtil.MetadataCacheCleaner())
                    using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                    {
                        OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) =>
                        {
                            config.SetEntitySetPageSize("Customers", 1);
                        };
                        EnumeratorDisposeDataContext.VerifyEnumeratorDisposed("/Customers?$expand=Orders", format);
                    }
                });
            }

            public class IDSPNullTestContext : DSP.IDataServiceMetadataProvider, DSP.IDataServiceQueryProvider
            {
                public static List<DSP.ResourceSet> ResourceSetsOverride;
                public static List<DSP.ResourceType> TypesOverride;
                public static List<DSP.ServiceOperation> ServiceOperationsOverride;
                public static string ContainerNamespaceOverride;
                public static string ContainerNameOverride;
                public static object CurrentDataSourceOverride;

                #region IDataServiceProvider Members

                public object CurrentDataSource
                {
                    get
                    {
                        if (CurrentDataSourceOverride != null)
                        {
                            return CurrentDataSourceOverride;
                        }
                        else
                        {
                            return this;
                        }
                    }
                    set
                    {
                        throw new NotSupportedException();
                    }
                }

                public bool IsNullPropagationRequired
                {
                    get { throw new NotImplementedException(); }
                }

                public string ContainerNamespace
                {
                    get { return ContainerNamespaceOverride; ; }
                }

                public string ContainerName
                {
                    get { return ContainerNameOverride; }
                }

                public IEnumerable<Microsoft.OData.Service.Providers.ResourceSet> ResourceSets
                {
                    get { return ResourceSetsOverride; }
                }

                public IEnumerable<Microsoft.OData.Service.Providers.ResourceType> Types
                {
                    get { return TypesOverride; }
                }

                public IEnumerable<Microsoft.OData.Service.Providers.ServiceOperation> ServiceOperations
                {
                    get { return ServiceOperationsOverride; }
                }

                public void DisposeDataSource()
                {
                    return;
                }

                public bool TryResolveResourceSet(string name, out Microsoft.OData.Service.Providers.ResourceSet resourceSet)
                {
                    throw new NotImplementedException();
                }

                public Microsoft.OData.Service.Providers.ResourceAssociationSet GetResourceAssociationSet(Microsoft.OData.Service.Providers.ResourceSet resourceSet, Microsoft.OData.Service.Providers.ResourceType resourceType, Microsoft.OData.Service.Providers.ResourceProperty resourceProperty)
                {
                    throw new NotImplementedException();
                }

                public IQueryable GetQueryRootForResourceSet(Microsoft.OData.Service.Providers.ResourceSet resourceSet)
                {
                    throw new NotImplementedException();
                }

                public bool TryResolveResourceType(string name, out Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                public Microsoft.OData.Service.Providers.ResourceType GetResourceType(object target)
                {
                    throw new NotImplementedException();
                }

                public IEnumerable<Microsoft.OData.Service.Providers.ResourceType> GetDerivedTypes(Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    return null;
                }

                public bool HasDerivedTypes(Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    return true;
                }

                public bool TryResolveServiceOperation(string name, out Microsoft.OData.Service.Providers.ServiceOperation serviceOperation)
                {
                    throw new NotImplementedException();
                }

                public object GetPropertyValue(object target, Microsoft.OData.Service.Providers.ResourceProperty resourceProperty)
                {
                    throw new NotImplementedException();
                }

                public object GetOpenPropertyValue(object target, string propertyName)
                {
                    throw new NotImplementedException();
                }

                public IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target)
                {
                    throw new NotImplementedException();
                }

                public object InvokeServiceOperation(Microsoft.OData.Service.Providers.ServiceOperation serviceOperation, object[] parameters)
                {
                    throw new NotImplementedException();
                }

                #endregion
            }

            [TestMethod, Variation("Make sure we can handle nulls from IDSP correctly")]
            public void IDSPNullTest()
            {
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(IDSPNullTestContext)))
                {
                    OpenWebDataServiceHelper.GetServiceCustomizer.Value = type =>
                        {
                            if (type == typeof(DSP.IDataServiceMetadataProvider) ||
                                type == typeof(DSP.IDataServiceQueryProvider))
                            {
                                return new IDSPNullTestContext();
                            }

                            return null;
                        };

                    // Make sure the data source is assignable to T in DataService<T>.
                    IDSPNullTestContext.CurrentDataSourceOverride = new object();
                    UnitTestsUtil.VerifyInvalidRequest(null, "/$metadata", typeof(IDSPNullTestContext), null, "GET", 500, "IDataServiceQueryProvider.CurrentDataSource must return an object of type that is assignable to T in DataService<T>.", null);
                    IDSPNullTestContext.CurrentDataSourceOverride = null;

                    // Make sure we validate IDSP.ContainerNamespace
                    IDSPNullTestContext.ContainerNameOverride = "ContainerName";
                    IDSPNullTestContext.ContainerNamespaceOverride = null;
                    UnitTestsUtil.VerifyInvalidRequest(null, "/$metadata", typeof(IDSPNullTestContext), null, "GET", 500, "The value returned by IDataServiceMetadataProvider.ContainerNamespace must not be null or empty.", null);
                    IDSPNullTestContext.ContainerNamespaceOverride = string.Empty;
                    UnitTestsUtil.VerifyInvalidRequest(null, "/$metadata", typeof(IDSPNullTestContext), null, "GET", 500, "The value returned by IDataServiceMetadataProvider.ContainerNamespace must not be null or empty.", null);

                    // Make sure we validate IDSP.ContainerName
                    IDSPNullTestContext.ContainerNamespaceOverride = "ContainerNamespace";
                    IDSPNullTestContext.ContainerNameOverride = null;
                    UnitTestsUtil.VerifyInvalidRequest(null, "/$metadata", typeof(IDSPNullTestContext), null, "GET", 500, "The value returned by IDataServiceMetadataProvider.ContainerName must not be null or empty.", null);
                    IDSPNullTestContext.ContainerNameOverride = null;
                    UnitTestsUtil.VerifyInvalidRequest(null, "/$metadata", typeof(IDSPNullTestContext), null, "GET", 500, "The value returned by IDataServiceMetadataProvider.ContainerName must not be null or empty.", null);

                    // This makes sure that we don't hit null ref exception when IDSP.ResourceSets, IDSP.Types and IDSP.ServiceOperations return null.
                    IDSPNullTestContext.ContainerNamespaceOverride = "ContainerNamespace";
                    IDSPNullTestContext.ContainerNameOverride = "ContainerName";
                    IDSPNullTestContext.ResourceSetsOverride = null;
                    IDSPNullTestContext.TypesOverride = null;
                    IDSPNullTestContext.ServiceOperationsOverride = null;
                    TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.MimeApplicationXml, "/$metadata", typeof(IDSPNullTestContext), null, "GET");

                    // This makes sure that we don't hit null ref exception when IDSP.GetDerivedTypes() returns null.
                    DSP.ResourceType customerType = new DSP.ResourceType(typeof(Customer), DSP.ResourceTypeKind.EntityType, null, null, "Customer", false);
                    DSP.ResourceProperty id = new DSP.ResourceProperty("ID", DSP.ResourcePropertyKind.Key | DSP.ResourcePropertyKind.Primitive, DSP.ResourceType.GetPrimitiveResourceType(typeof(int)));
                    customerType.AddProperty(id);
                    DSP.ResourceSet customerSet = new DSP.ResourceSet("Customers", customerType);
                    customerSet.SetReadOnly();
                    IDSPNullTestContext.ResourceSetsOverride = new List<DSP.ResourceSet>() { customerSet };
                    IDSPNullTestContext.TypesOverride = new List<DSP.ResourceType>() { customerType };
                    request = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.MimeApplicationXml, "/$metadata", typeof(IDSPNullTestContext), null, "GET");
                }
            }

            public class IDSPCustomDataSource : DSP.IDataServiceMetadataProvider, DSP.IDataServiceQueryProvider
            {
                public static List<DSP.ResourceSet> ResourceSetsOverride;
                public static List<DSP.ResourceType> TypesOverride;
                public static List<DSP.ServiceOperation> ServiceOperationsOverride;
                public static string ContainerNamespaceOverride;
                public static string ContainerNameOverride;
                public static object CurrentDataSourceOverride = null;

                #region IDataServiceProvider Members

                public object CurrentDataSource
                {
                    get
                    {
                        return CurrentDataSourceOverride;
                    }
                    set
                    {
                        CurrentDataSourceOverride = value;
                    }
                }

                public bool IsNullPropagationRequired
                {
                    get { throw new NotImplementedException(); }
                }

                public string ContainerNamespace
                {
                    get { return ContainerNamespaceOverride; ; }
                }

                public string ContainerName
                {
                    get { return ContainerNameOverride; }
                }

                public IEnumerable<Microsoft.OData.Service.Providers.ResourceSet> ResourceSets
                {
                    get { return ResourceSetsOverride; }
                }

                public IEnumerable<Microsoft.OData.Service.Providers.ResourceType> Types
                {
                    get { return TypesOverride; }
                }

                public IEnumerable<Microsoft.OData.Service.Providers.ServiceOperation> ServiceOperations
                {
                    get { return ServiceOperationsOverride; }
                }

                public void DisposeDataSource()
                {
                    return;
                }

                public bool TryResolveResourceSet(string name, out Microsoft.OData.Service.Providers.ResourceSet resourceSet)
                {
                    throw new NotImplementedException();
                }

                public Microsoft.OData.Service.Providers.ResourceAssociationSet GetResourceAssociationSet(Microsoft.OData.Service.Providers.ResourceSet resourceSet, Microsoft.OData.Service.Providers.ResourceType resourceType, Microsoft.OData.Service.Providers.ResourceProperty resourceProperty)
                {
                    throw new NotImplementedException();
                }

                public IQueryable GetQueryRootForResourceSet(Microsoft.OData.Service.Providers.ResourceSet resourceSet)
                {
                    throw new NotImplementedException();
                }

                public bool TryResolveResourceType(string name, out Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                public Microsoft.OData.Service.Providers.ResourceType GetResourceType(object target)
                {
                    throw new NotImplementedException();
                }

                public IEnumerable<Microsoft.OData.Service.Providers.ResourceType> GetDerivedTypes(Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    return null;
                }

                public bool HasDerivedTypes(Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                public bool TryResolveServiceOperation(string name, out Microsoft.OData.Service.Providers.ServiceOperation serviceOperation)
                {
                    throw new NotImplementedException();
                }

                public object GetPropertyValue(object target, Microsoft.OData.Service.Providers.ResourceProperty resourceProperty)
                {
                    throw new NotImplementedException();
                }

                public object GetOpenPropertyValue(object target, string propertyName)
                {
                    throw new NotImplementedException();
                }

                public IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target)
                {
                    throw new NotImplementedException();
                }

                public object InvokeServiceOperation(Microsoft.OData.Service.Providers.ServiceOperation serviceOperation, object[] parameters)
                {
                    throw new NotImplementedException();
                }

                #endregion
            }

            [TestMethod, Variation("Make sure we set the CurrentDataSource property, if it returns null")]
            public void IDSPCurrentDataSource()
            {
                IDSPCustomDataSource providerInstance = new IDSPCustomDataSource();
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(IDSPNullTestContext)))
                {
                    OpenWebDataServiceHelper.GetServiceCustomizer.Value = type =>
                    {
                        if (type == typeof(DSP.IDataServiceMetadataProvider) ||
                            type == typeof(DSP.IDataServiceQueryProvider))
                        {
                            return providerInstance;
                        }

                        return null;
                    };

                    UnitTestsUtil.VerifyInvalidRequest(null, "/$metadata", typeof(IDSPCustomDataSource), null, "GET", 500, "The value returned by IDataServiceMetadataProvider.ContainerName must not be null or empty.", null);
                    Assert.IsTrue(IDSPCustomDataSource.CurrentDataSourceOverride != providerInstance, "CurrentDataSource property has been set for custom providers");
                }
            }

            internal class ServiceWithProviders : DataService<object>, DSP.IDataServiceMetadataProvider, DSP.IDataServiceQueryProvider, IServiceProvider
            {
                public static bool ImplementsMetadataProvider;
                public static bool ImplementsQueryProvider;

                public object GetService(Type serviceType)
                {
                    if (ImplementsMetadataProvider && serviceType == typeof(DSP.IDataServiceMetadataProvider))
                    {
                        return this;
                    }
                    else if (ImplementsQueryProvider && serviceType == typeof(DSP.IDataServiceQueryProvider))
                    {
                        return this;
                    }

                    return null;
                }

                #region IDataServiceMetadataProvider

                string DSP.IDataServiceMetadataProvider.ContainerNamespace
                {
                    get { throw new NotImplementedException(); }
                }

                string DSP.IDataServiceMetadataProvider.ContainerName
                {
                    get { throw new NotImplementedException(); }
                }

                System.Collections.Generic.IEnumerable<DSP.ResourceSet> DSP.IDataServiceMetadataProvider.ResourceSets
                {
                    get { throw new NotImplementedException(); }
                }

                System.Collections.Generic.IEnumerable<Microsoft.OData.Service.Providers.ResourceType> DSP.IDataServiceMetadataProvider.Types
                {
                    get { throw new NotImplementedException(); }
                }

                System.Collections.Generic.IEnumerable<Microsoft.OData.Service.Providers.ServiceOperation> DSP.IDataServiceMetadataProvider.ServiceOperations
                {
                    get { throw new NotImplementedException(); }
                }

                bool DSP.IDataServiceMetadataProvider.TryResolveResourceSet(string name, out DSP.ResourceSet resourceSet)
                {
                    throw new NotImplementedException();
                }

                Microsoft.OData.Service.Providers.ResourceAssociationSet DSP.IDataServiceMetadataProvider.GetResourceAssociationSet(DSP.ResourceSet resourceSet, Microsoft.OData.Service.Providers.ResourceType resourceType, Microsoft.OData.Service.Providers.ResourceProperty resourceProperty)
                {
                    throw new NotImplementedException();
                }

                bool DSP.IDataServiceMetadataProvider.TryResolveResourceType(string name, out Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                System.Collections.Generic.IEnumerable<Microsoft.OData.Service.Providers.ResourceType> DSP.IDataServiceMetadataProvider.GetDerivedTypes(Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                bool DSP.IDataServiceMetadataProvider.HasDerivedTypes(Microsoft.OData.Service.Providers.ResourceType resourceType)
                {
                    throw new NotImplementedException();
                }

                bool DSP.IDataServiceMetadataProvider.TryResolveServiceOperation(string name, out Microsoft.OData.Service.Providers.ServiceOperation serviceOperation)
                {
                    throw new NotImplementedException();
                }

                #endregion IDataServiceMetadataProvider

                #region IDataServiceQueryProvider

                object DSP.IDataServiceQueryProvider.CurrentDataSource
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

                bool DSP.IDataServiceQueryProvider.IsNullPropagationRequired
                {
                    get { throw new NotImplementedException(); }
                }

                IQueryable DSP.IDataServiceQueryProvider.GetQueryRootForResourceSet(DSP.ResourceSet resourceSet)
                {
                    throw new NotImplementedException();
                }

                Microsoft.OData.Service.Providers.ResourceType DSP.IDataServiceQueryProvider.GetResourceType(object target)
                {
                    throw new NotImplementedException();
                }

                object DSP.IDataServiceQueryProvider.GetPropertyValue(object target, Microsoft.OData.Service.Providers.ResourceProperty resourceProperty)
                {
                    throw new NotImplementedException();
                }

                object DSP.IDataServiceQueryProvider.GetOpenPropertyValue(object target, string propertyName)
                {
                    throw new NotImplementedException();
                }

                System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>> DSP.IDataServiceQueryProvider.GetOpenPropertyValues(object target)
                {
                    throw new NotImplementedException();
                }

                object DSP.IDataServiceQueryProvider.InvokeServiceOperation(Microsoft.OData.Service.Providers.ServiceOperation serviceOperation, object[] parameters)
                {
                    throw new NotImplementedException();
                }

                #endregion IDataServiceQueryProvider
            }

            [TestMethod, Variation("Metadata provider and query provider load error.")]
            public void MetadataAndQueryProviderLoadErrorTest()
            {
                var testCases = new []
                {
                    new
                    {
                        ImplementsMetadataProvider = true,
                        ImplementsQueryProvider = false,
                        ErrorMsg = "For custom providers, if GetService returns non-null for IDataServiceMetadataProvider, it must not return null for IDataServiceQueryProvider.",
                    },
                };

                TestUtil.RunCombinations(testCases, testCase =>
                {
                    using (TestUtil.RestoreStaticMembersOnDispose(typeof(ServiceWithProviders)))
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        ServiceWithProviders.ImplementsMetadataProvider = testCase.ImplementsMetadataProvider;
                        ServiceWithProviders.ImplementsQueryProvider = testCase.ImplementsQueryProvider;

                        request.DataServiceType = typeof(ServiceWithProviders);
                        request.RequestUriString = "/$metadata";
                        request.HttpMethod = "GET";
                        Exception e = TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNotNull(e);
                        Assert.AreEqual(testCase.ErrorMsg, e.Message);
                        Assert.AreEqual(typeof(InvalidOperationException), e.GetType());
                    }
                });
            }
        }
    }
}
