//---------------------------------------------------------------------
// <copyright file="ServiceOperationsTest.cs" company="Microsoft">
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
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.ServiceModel.Web;
    using System.Xml;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using BindingFlags = System.Reflection.BindingFlags;

    #endregion Namespaces

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
    /// <summary>This is a test class for service operations.</summary>
    [TestClass()]
    public class ServiceOperationsTest
    {
        /// <summary>Checks that methods with parameters can be invoked correctly.</summary>
        [TestMethod]
        public void ServiceOperationsParameterInvoke()
        {
            // Test matrix:
            //
            // - Type of parameters.
            // - Count of parameters defined.
            // - For each parameter, whether it's present zero, one or many times.
            // - Whether the parameter value is of the correct type, and whether it's a well-formed value.
            int id = 0;
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("TypeData", TypeData.Values),
                new Dimension("UseSmallCasing", new bool[] { true, false }),
                new Dimension("UseDoublePostfix", new bool[] { true, false }),
                new Dimension("ParameterCount", new object[] { 1, 3 }));

            Dictionary<KeyValuePair<TypeData, int>, TypeBuilder> typeBuilders = new Dictionary<KeyValuePair<TypeData, int>, TypeBuilder>();
            ModuleBuilder module = TestUtil.CreateModuleBuilder("ServiceOpModule");
            TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable engineValues)
            {
                TypeData typeData = (TypeData)engineValues["TypeData"];
                int parameterCount = (int)engineValues["ParameterCount"];
                bool useSmallCasing = (bool)engineValues["UseSmallCasing"];
                bool useDoublePostFix = (bool)engineValues["UseDoublePostfix"];

                // Unsupported types are rejected before invocation - there's a separate test for these cases.
                if (!typeData.IsTypeSupported || useSmallCasing == false || useDoublePostFix == true)
                {
                    return;
                }

                typeBuilders[new KeyValuePair<TypeData, int>(typeData, parameterCount)] =
                    CreateServiceTypeForParameterTesting(module, id++, "ServiceOp", typeData.ClrType, parameterCount);
            });

            TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable engineValues)
            {
                TypeData typeData = (TypeData)engineValues["TypeData"];
                int parameterCount = (int)engineValues["ParameterCount"];
                bool useSmallCasing = (bool)engineValues["UseSmallCasing"];
                bool useDoublePostFix = (bool)engineValues["UseDoublePostfix"];

                // Unsupported types are rejected before invocation - there's a separate test for these cases.
                if (!typeData.IsTypeSupported)
                {
                    return;
                }

                TestUtil.ClearMetadataCache();
                using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcess))
                {
                    request.ServiceType = typeBuilders[new KeyValuePair<TypeData, int>(typeData, parameterCount)].CreateType();

                    List<Dimension> requestDimensions = new List<Dimension>(parameterCount * 2);
                    for (int i = 0; i < parameterCount; i++)
                    {
                        // Turns out that which parameter is valid/multiply specified never matters in practice.
                        // Shrinking the matrix a bit.
                        if (i == 0)
                        {
                            requestDimensions.Add(new Dimension("Parameter" + i + "ValueCount", new object[] { 0, 1, 2 }));
                            requestDimensions.Add(new Dimension("Parameter" + i + "IsValid", new object[] { true, false }));
                        }
                        else
                        {
                            requestDimensions.Add(new Dimension("Parameter" + i + "ValueCount", new object[] { 1 }));
                            requestDimensions.Add(new Dimension("Parameter" + i + "IsValid", new object[] { true }));
                        }
                    }

                    CombinatorialEngine requestEngine = CombinatorialEngine.FromDimensions(requestDimensions.ToArray());
                    TestUtil.RunCombinatorialEngineFail(requestEngine, delegate (Hashtable requestValues)
                    {
                        bool isParameterMissing = false;
                        bool parameterTypeCanBeNull = typeData.ClrType.IsClass || Nullable.GetUnderlyingType(typeData.ClrType) != null;
                        bool areParametersValid = true;
                        bool parameterWritten = false;
                        object[] expectedValues = new object[parameterCount];

                        // Compose on the request URI for every parameter and track flags.
                        string requestUri = "/ServiceOp";
                        object expectedValue = typeData.NonNullValue;

                        if (!(expectedValue is double) && useDoublePostFix)
                        {
                            return;
                        }

                        for (int i = 0; i < parameterCount; i++)
                        {
                            int valueCount = (int)requestValues["Parameter" + i + "ValueCount"];
                            bool isValid = (bool)requestValues["Parameter" + i + "IsValid"];

                            if (valueCount == 0 && !parameterTypeCanBeNull)
                            {
                                isParameterMissing = true;
                            }

                            if ((!isValid && valueCount > 0) || (valueCount > 1))
                            {
                                areParametersValid = false;
                            }

                            expectedValues[i] = (valueCount > 0) ? expectedValue : null;
                            for (int j = 0; j < valueCount; j++)
                            {
                                if (parameterWritten)
                                {
                                    requestUri += "&";
                                }
                                else
                                {
                                    requestUri += "?";
                                    parameterWritten = true;
                                }

                                requestUri += "p" + i + "=";
                                requestUri += TypeData.FormatForKey(expectedValue, useSmallCasing, useDoublePostFix);
                                if (!isValid)
                                {
                                    requestUri += "blahblah";
                                }
                            }
                        }

                        request.RequestUriString = requestUri;

                        Exception exceptionThrown = null;
                        try
                        {
                            CustomDataContext.SetLastParameters(null);
                            Trace.WriteLine("Sending request for " + request.RequestUriString);
                            request.SendRequest();

                            object[] receivedValues = CustomDataContext.LastParameters;
                            for (int i = 0; i < receivedValues.Length; i++)
                            {
                                if (typeData.ClrType.IsArray)
                                {
                                    TestUtil.AssertAreIEnumerablesEqual(expectedValues[i] as IEnumerable, receivedValues[i] as IEnumerable);
                                }
                                else if
                                    (typeData.ClrType == typeof(System.Xml.Linq.XElement) &&
                                     expectedValues[i] != null)
                                {
                                    Assert.AreEqual(expectedValues[i].ToString(), receivedValues[i].ToString(), "Received value #" + i + " matches what was sent.");
                                }
                                else
                                {
                                    Assert.AreEqual(expectedValues[i], receivedValues[i], "Received value #" + i + " matches what was sent.");
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            exceptionThrown = exception;
                        }

                        TestUtil.AssertExceptionExpected(exceptionThrown, isParameterMissing, !areParametersValid);
                    });
                }
            });
        }

        /// <summary>
        /// Checks that metadata can be generate for service operation 
        /// parameters.
        /// </summary>
        [TestMethod]
        public void ServiceOperationsParameterMetadataBasic()
        {
            // Test Matrix
            // - With [WebGet]/[WebInvoke]/nothing set.
            // - Type of parameter / no parameter.
            Type[] webAttributes = new Type[]
            {
                typeof(WebGetAttribute),
                typeof(WebInvokeAttribute),
                null
            };

            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("WebAttribute", webAttributes),
                new Dimension("ParameterType", TypeData.Values));

            int assemblyCount = 0;
            TestUtil.RunCombinatorialEngineFail(engine, (table) =>
            {
                Type resultTypeKind = typeof(IQueryable);
                Type elementType = typeof(Customer);
                Type webAttribute = (Type)table["WebAttribute"];
                TypeData parameterType = (TypeData)table["ParameterType"];
                Type baseTypeForContext = typeof(CustomDataContext);
                string methodName = "Action";

                // V4 would return metadata with "Nullable" attribute on function parameters.
                // EdmItemCollection's constructor would throw if the parameter is non-nullable
                if (parameterType.DefaultContentType != null)
                {
                    return;
                }

                // Create an assembly for this type combination.
                assemblyCount++;
                Type serviceType = CreateServiceType(
                    assemblyCount,
                    resultTypeKind,
                    elementType,
                    false /* isAbstract */,
                    false /* singleResult */,
                    webAttribute,
                    baseTypeForContext,
                    methodName,
                    parameterType.ClrType);
                CheckValidMetadata(serviceType, webAttribute,
                    webAttribute != null && parameterType != null && !parameterType.IsTypeSupported);
            });
        }

        /// <summary>Checks that invalid MIME types are rejected.</summary>
        [TestMethod]
        public void ServiceOperationsInvalidMimeType()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.ServiceType = typeof(InvalidMimeService);
                request.RequestUriString = "/$metadata";
                Exception exception = TestUtil.RunCatching(request.SendRequest);
                TestUtil.AssertExceptionExpected(exception, true);
                TestUtil.AssertExceptionStatusCode(exception, 500,
                    "Invalid MIME specification results in 500 status code.");
            }
        }

        [MimeType("Op", "text/html;level=1")]
        class InvalidMimeService : OpenWebDataService<CustomDataContext>
        {
            [SingleResult]
            [WebGet]
            public IQueryable<string> Op()
            {
                return new string[] { "howdy" }.AsQueryable();
            }
        }

        /// <summary>Checks that metadata can be generated for service operations.</summary>
        [Ignore] // Remove Atom
        // [TestMethod]
        public void ServiceOperationsMetadataBasic()
        {
            // Test Matrix
            // - Service operation result type: IQueryable<T>, IEnumerable<T>, T, void.
            // - T being a primitive type, a complex type, a resource type, a "new" complex type.
            // - With/without [SingleResult] set.
            // - With [WebGet]/[WebInvoke]/nothing set.
            // - With same name / different name from entity set.
            Type[] resultTypeKinds = new Type[]
            {
                typeof(IQueryable),
                typeof(IEnumerable),
                typeof(object),
                //typeof(void),
            };
            Type[] elementTypes = new Type[]
            {
                typeof(string),
                typeof(Customer),
            };
            Type[] webAttributes = new Type[]
            {
                typeof(WebGetAttribute),
                typeof(WebInvokeAttribute),
                null
            };
            object[] booleanValues = new object[] { true, false };

            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("ResultTypeKind", resultTypeKinds),
                new Dimension("NameOverrides", booleanValues),
                new Dimension("IsAbstract", booleanValues),
                new Dimension("ElementType", elementTypes),
                new Dimension("SingleResult", booleanValues),
                new Dimension("WebAttribute", webAttributes),
                new Dimension("EdmBased", booleanValues));

            bool nullAttributeNotOverridesTested = false;
            bool nullAttributeIncorrectSuffixTested = false;
            bool abstractTested = false;
            bool nameOverridesTested = false;
            int assemblyCount = 0;
            TestUtil.RunCombinatorialEngineFail(engine, (table) =>
            {
                Type resultTypeKind = (Type)table["ResultTypeKind"];
                Type elementType = (Type)table["ElementType"];
                bool nameOverrides = (bool)table["NameOverrides"];
                bool isAbstract = (bool)table["IsAbstract"];
                bool singleResult = (bool)table["SingleResult"];
                Type webAttribute = (Type)table["WebAttribute"];
                bool edmBased = (bool)table["EdmBased"];

                #region Uninteresting cases.

                // Void Functions are now invalid.
                if (resultTypeKind == typeof(void) && webAttribute == typeof(WebGetAttribute))
                {
                    return;
                }

                // Abstract class won't allow instantiation.
                if (isAbstract)
                {
                    if (abstractTested)
                    {
                        return;
                    }

                    abstractTested = true;
                }

                if (webAttribute == null && !nameOverrides)
                {
                    if (nullAttributeNotOverridesTested)
                    {
                        return;
                    }
                    nullAttributeNotOverridesTested = true;
                }

                if (webAttribute != null && nameOverrides)
                {
                    if (nameOverridesTested)
                    {
                        return;
                    }
                    nameOverridesTested = true;
                }

                if (singleResult == false && resultTypeKind == typeof(object))
                {
                    // This is effectively one of IEnumerable or IQueryable.
                    return;
                }

                #endregion Uninteresting cases.

                Type baseTypeForContext = (edmBased) ? typeof(NorthwindModel.NorthwindContext) : typeof(CustomDataContext);

                // For Northwind, we use a different Entity type.
                bool isMultiple =
                    (resultTypeKind == typeof(IQueryable) && !singleResult) ||
                    resultTypeKind == typeof(IEnumerable);
                bool isEntity = elementType == typeof(Customer);
                elementType = (isEntity && edmBased) ? typeof(NorthwindModel.Customers) : elementType;

                string methodName = (nameOverrides) ? "Customers" : "Action";

                // Create an assembly for this type combination.
                assemblyCount++;
                Type serviceType = CreateServiceType(
                    assemblyCount,          // id
                    resultTypeKind,         // resultTypeKind
                    elementType,            // elementType
                    isAbstract,             // isAbstract
                    singleResult,           // singleResult
                    webAttribute,           // webAttribute
                    baseTypeForContext,     // baseTypeForContext
                    methodName,             // methodName
                    null);                  // parameterType

                {
                    MethodInfo method = serviceType.GetMethod(methodName);
                    Trace.WriteLine("The method name signature is '" + method.ToString() + "'.");
                }

                ServiceModelData.Northwind.EnsureDependenciesAvailable();
                TestUtil.ClearMetadataCache();
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.ServiceType = serviceType;

                    // Check that generate metadata is valid.
                    bool valid = CheckValidMetadata(request, webAttribute,
                        isAbstract,
                        (webAttribute == null && nameOverrides == false && false), // change to true when URI processor is implemented
                                                                                   /* These are 'schedule' rather than technical limitations. */
                        nameOverrides && webAttribute != null,
                        webAttribute != null && singleResult && resultTypeKind == typeof(IEnumerable));

                    // If metadata is invalid, we won't be able to get results either.
                    if (!valid)
                    {
                        return;
                    }

                    // One-off enumeration for appending something after service operation:
                    // no suffix; suffixed by correct property, suffixed by incorrect property.
                    const string NoSuffix = "NoSuffix";
                    const string CorrectPropertySuffix = "CorrectPropertySuffix";
                    const string IncorrectPropertySuffix = "IncorrectPropertySuffix";

                    CombinatorialEngine requestEngine = CombinatorialEngine.FromDimensions(
                        new Dimension("Suffix", new object[] { NoSuffix, CorrectPropertySuffix, IncorrectPropertySuffix }),
                        new Dimension("Method", new object[] { "GET", "POST" }),
                        new Dimension("Format", UnitTestsUtil.ResponseFormats));
                    TestUtil.RunCombinatorialEngineFail(requestEngine, (requestValues) =>
                    {
                        string suffix = (string)requestValues["Suffix"];
                        string method = (string)requestValues["Method"];
                        string responseFormat = (string)requestValues["Format"];

                        // Uninteresting cases.
                        if (webAttribute == null && suffix != NoSuffix)
                        {
                            if (nullAttributeIncorrectSuffixTested)
                            {
                                return;
                            }
                            nullAttributeIncorrectSuffixTested = true;
                        }

                        request.Accept = (responseFormat == UnitTestsUtil.AtomFormat) ? "application/atom+xml,application/xml" : responseFormat;
                        request.RequestUriString = "/" + methodName;
                        request.HttpMethod = method;

                        if (suffix != NoSuffix)
                        {
                            if (isMultiple)
                            {
                                request.RequestUriString += (edmBased) ? "!'ALFKI'" : "!1";
                            }

                            if (elementType == typeof(string))
                            {
                                if (suffix == CorrectPropertySuffix)
                                {
                                    return;
                                }
                                else
                                {
                                    request.RequestUriString += "/Foo";
                                }
                            }
                            else
                            {
                                request.RequestUriString += (edmBased) ? "/ContactName" : "/Name";
                                if (suffix == IncorrectPropertySuffix)
                                {
                                    request.RequestUriString += "Foo";
                                }
                            }
                        }

                        Stream stream = null;
                        Exception exceptionThrown = TestUtil.RunCatching(delegate ()
                        {
                            Trace.WriteLine("RequestUriString: " + request.RequestUriString);
                            request.SendRequest();
                            stream = request.GetResponseStream();
                        });

                        // IQueryables actually return multiple results, so
                        // a single-result query will always fail downstream
                        // (it's an internal error in this case).
                        TestUtil.AssertExceptionExpected(exceptionThrown,
                            (webAttribute == null && !nameOverrides),
                            (webAttribute != null && suffix != NoSuffix && resultTypeKind == typeof(void)),
                            (webAttribute != null && suffix == IncorrectPropertySuffix),
                            (webAttribute != null && suffix == CorrectPropertySuffix && resultTypeKind == typeof(IEnumerable)),
                            (webAttribute != null && suffix == CorrectPropertySuffix && resultTypeKind == typeof(object)),
                            (webAttribute != null && suffix == CorrectPropertySuffix && method == "POST"),
                            (webAttribute == null && suffix != NoSuffix),
                            (webAttribute == typeof(WebGetAttribute) && method != "GET"),
                            (webAttribute == typeof(WebInvokeAttribute) && method != "POST"),
                            (webAttribute == null && method == "POST"),
                            (webAttribute != null && !singleResult && suffix != NoSuffix));

                        if (stream != null)
                        {
                            if (resultTypeKind == typeof(void) && webAttribute != null)
                            {
                                TestUtil.AssertIsEmpty(stream);
                            }
                            else if (suffix == NoSuffix && webAttribute != null)
                            {
                                string web3sPrefix = edmBased ? "nc" : "cdc";
                                string elementName = (elementType == typeof(string) && webAttribute != null) ? "String" :
                                    (edmBased ? "Customers" : "Customer");
                                string[] web3sXPaths = new string[] {
                                    "/" + web3sPrefix + ":" + methodName,
                                    "/" + web3sPrefix + ":" + methodName + "/" + web3sPrefix + ":" + elementName,
                                };

                                string jsonElementName = (elementType == typeof(string) && webAttribute != null) ? "System.String" :
                                    (edmBased ? typeof(NorthwindModel.Customers).FullName : typeof(Customer).FullName);
                                string[] atomXPaths, jsonXPaths;
                                if (elementType == typeof(string) && webAttribute != null)
                                {
                                    if (singleResult)
                                    {
                                        atomXPaths = new string[] { "/adsm:value", "/adsm:value[count(adsm:Element) = 0]" };
                                        jsonXPaths = new string[] { "/Object" };
                                    }
                                    else
                                    {
                                        atomXPaths = new string[] { "/adsm:value/adsm:element" };
                                        jsonXPaths = new string[] {
                                            String.Format("/{0}/{1}/{2}/{0}[count(@*) = 0]",
                                                 JsonValidator.ObjectString, JsonValidator.ResultsString, JsonValidator.ArrayString )
                                        };
                                    }
                                }
                                else
                                {
                                    if (singleResult)
                                    {
                                        atomXPaths = new string[] {
                                            "/atom:entry[atom:category/@term='#" + jsonElementName + "']"
                                        };
                                        jsonXPaths = new string[] {
                                            String.Format("/{0}/__metadata[type='{1}']",
                                                JsonValidator.ObjectString, jsonElementName)
                                        };
                                    }
                                    else
                                    {
                                        atomXPaths = new string[] {
                                            "/atom:feed/atom:entry[atom:category/@term='#" + jsonElementName + "']"
                                        };
                                        jsonXPaths = new string[] {
                                            String.Format("/{1}/{3}/{0}/{1}/__metadata[type='{2}']",
                                                JsonValidator.ArrayString, JsonValidator.ObjectString, jsonElementName, JsonValidator.ResultsString)
                                        };
                                    }
                                }

                                UnitTestsUtil.VerifyXPaths(stream, request.ResponseContentType,
                                    web3sXPaths, jsonXPaths, atomXPaths);
                            }
                        }
                    });
                }
            });
        }

        /// <summary>Checks that parameters support extra whitespace.</summary>
        [Ignore] // Remove Atom
        // [TestMethod]
        public void ServiceOperationsSyntax()
        {
            TestUtil.TraceScopeForException("ServiceOperationsSyntax", delegate ()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.RequestUriString = "/ManyCustomers?name%20=%20'the+name'+&id=12+";
                    request.ServiceType = typeof(ServiceOperationsSyntaxService);
                    request.SendRequest();

                    request.RequestUriString = "/ManyCustomers?name=color&id=123";
                    request.ServiceType = typeof(ServiceOperationsSyntaxService);
                    Exception exception = TestUtil.RunCatching(delegate () { request.SendRequest(); });
                    TestUtil.AssertExceptionExpected(exception, true);
                }
            });
        }

        /// <summary>Checks that service operation supports derived type identifier segments in the URI.</summary>
        [Ignore] // Remove Atom
        // [TestMethod]
        public void ServiceOperations_WithTypeIdentifier()
        {
            TestUtil.TraceScopeForException("ServiceOperations_WithTypeIdentifier", delegate ()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.RequestUriString = "/CustomersWithDerivedTypeIdentiferSegment()/AstoriaUnitTests.Stubs.CustomerWithBirthday/";
                    request.ServiceType = typeof(ServiceOperationsSyntaxService);
                    request.SendRequest();
                }
            });
        }

        /// <summary>Checks that service operation with a derived type identifier segment in the URI returns only instances of the type..</summary>
        [Ignore] // Remove Atom
        // [TestMethod]
        public void ServiceOperations_WithTypeIdentifier_ReturnsAllResults_1()
        {
            TestUtil.TraceScopeForException("ServiceOperations_WithTypeIdentifier_ReturnsAllResults", delegate ()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.RequestUriString = "/CustomersWithDerivedTypeIdentiferSegment()/AstoriaUnitTests.Stubs.CustomerWithBirthday/";
                    request.ServiceType = typeof(ServiceOperationsSyntaxService);
                    request.SendRequest();

                    DataServiceContext clientContext = new DataServiceContext(new Uri(request.BaseUri), Microsoft.OData.Client.ODataProtocolVersion.V4);
                    //clientContext.EnableAtom = true;
                    //clientContext.Format.UseAtom();
                    clientContext.Timeout = 6000;
                    var customersWithBirthday = clientContext.CreateQuery<Customer>("CustomersWithDerivedTypeIdentiferSegment").OfType<CustomerWithBirthday>().ToList();

                    Assert.IsTrue(customersWithBirthday.All(instance => instance is CustomerWithBirthday), "Expected only instances of CustomerWithBirthday in results");
                    Assert.AreEqual(1, customersWithBirthday.Count, "Expected only one row of data for this query");
                }
            });
        }

        /// <summary>Checks that service operation with a derived type identifier segment in the URI allows one to project derived type properites</summary>
        [Ignore] // Remove Atom
        // [TestMethod]
        public void ServiceOperations_WithTypeIdentifier_ReturnsAllResults_2()
        {
            TestUtil.TraceScopeForException("ServiceOperations_WithTypeIdentifier_ReturnsAllResults", delegate ()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.RequestUriString = "/CustomersWithDerivedTypeIdentiferSegment()/AstoriaUnitTests.Stubs.CustomerWithBirthday/";
                    request.ServiceType = typeof(ServiceOperationsSyntaxService);
                    request.SendRequest();

                    DataServiceContext clientContext = new DataServiceContext(new Uri(request.BaseUri), Microsoft.OData.Client.ODataProtocolVersion.V4);
                    //clientContext.EnableAtom = true;
                    //clientContext.Format.UseAtom();
                    clientContext.Timeout = 6000;
                    var customersWithBirthday = (from customerWithBirthday in clientContext.CreateQuery<Customer>("CustomersWithDerivedTypeIdentiferSegment").OfType<CustomerWithBirthday>()
                                                 select new CustomerWithBirthday()
                                                 {
                                                     ID = customerWithBirthday.ID,
                                                     Birthday = customerWithBirthday.Birthday
                                                 }).ToList();

                    Assert.IsTrue(customersWithBirthday.All(instance => instance is CustomerWithBirthday), "Expected only instances of CustomerWithBirthday in results");
                    Assert.AreEqual(1, customersWithBirthday.LongCount(), "Expected only one row of data for this query");
                }
            });
        }

        /// <summary>Checks that service operations work with enumerations that are not IQueryable.</summary>
        [Ignore] // Remove Atom
        // [TestMethod]
        public void ServiceOperationsPlainEnumerableTest()
        {
            TestUtil.TraceScopeForException("ServiceOperationsPlainEnumerableTest", delegate ()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.ServiceType = typeof(ServiceOperationsSyntaxService);
                    request.RequestUriString = "/TheEnumerableCustomer";
                    Exception exception = TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(exception, false);
                }
            });
        }
        [Ignore] // Remove Atom
        // [TestMethod]
        public void InStreamPagingErrorForIEnumerableServiceOperation()
        {
            using (TestUtil.MetadataCacheCleaner())
            using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, foo) => { config.SetEntitySetPageSize("*", 1); };
                request.ServiceType = typeof(ServiceOperationsSyntaxService);

                // Verify in-stream error when page limit gets reached.
                request.RequestUriString = "/TheEnumerableCustomerMulti";
                Exception exception = TestUtil.RunCatching(request.SendRequest);
                TestUtil.AssertExceptionExpected(exception, false);
                String response = request.GetResponseStreamAsText();
                Assert.IsTrue(response.Contains("The response exceeds the maximum 1 results per collection."));

                // Verify that there is no next link.
                request.RequestUriString = "/TheEnumerableCustomer";
                request.Accept = "application/atom+xml,application/xml";
                exception = TestUtil.RunCatching(request.SendRequest);
                TestUtil.AssertExceptionExpected(exception, false);
                XmlDocument document = new XmlDocument(TestUtil.TestNameTable);
                document.Load(request.GetResponseStream());
                XmlNodeList list = document.SelectNodes("/atom:feed/atom:link[@rel='next']", TestUtil.TestNamespaceManager);
                Assert.AreEqual(0, list.Count, "Not expecting any next link.");

                // Verify that expands & skip tokens are not allowed.
                foreach (var option in new[] { "$expand=Orders", "$skiptoken=1" })
                {
                    request.RequestUriString = "/TheEnumerableCustomer?" + option;
                    exception = TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(exception, true);
                }
            }
        }

        /// <summary>Enumerable and direct values returning service operations should be allowed to composed further.</summary>
        [TestMethod]
        public void ServiceOperationsNoCompositionTest()
        {
            TestUtil.TraceScopeForException("ServiceOperationsNoCompositionTest", delegate ()
            {
                string[] options = new string[]
                {
                    "$expand=Name",
                    "$filter=true",
                    "$skip=1", "$top=1",
                    "$orderby=Name",
                    "$count=true",
                    "$select=ID",
                    "$skiptoken=1",
                    "/$count"
                };
                string[] uriList = new string[]
                {
                    "EnumerableCustomer",
                    "TheEnumerableCustomer",
                    "TheEnumerableCustomerReturningQueryableInstance",
                    "TheEnumerableCustomerMulti",
                    "GetSingleCustomer"
                };

                foreach (string uri in uriList)
                {
                    foreach (string option in options)
                    {
                        int expectedErrorCode = 400;
                        string uriAppend = "?";
                        string errorString = DataServicesResourceUtil.GetString("RequestQueryProcessor_QueryNoOptionsApplicable");

                        if (!option.StartsWith("$", StringComparison.Ordinal))
                        {
                            uriAppend = String.Empty;
                            expectedErrorCode = 404;
                            if (uri == "GetSingleCustomer")
                            {
                                errorString = ODataLibResourceUtil.GetString("RequestUriProcessor_CountNotSupported", uri, "$count");
                            }
                            else if (option == "/$count")
                            {
                                expectedErrorCode = 400;
                                errorString = ODataLibResourceUtil.GetString("RequestUriProcessor_MustBeLeafSegment", uri);
                            }
                            else
                            {
                                errorString = DataServicesResourceUtil.GetString("RequestUriProcessor_IEnumerableServiceOperationsCannotBeFurtherComposed", uri);
                            }
                        }

                        string u = "/" + uri + uriAppend + option;
                        UnitTestsUtil.VerifyInvalidRequest(null, u, typeof(ServiceOperationsSyntaxService), UnitTestsUtil.AtomFormat, "GET", expectedErrorCode, errorString);
                    }
                }
            });
        }

        [TestMethod]
        public void ServiceOperationForCustomProvidersMetadataBasic()
        {
            foreach (Type contextType in new Type[] { typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) })
            {
                TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(null, "/$metadata", typeof(OpenWebDataService<>).MakeGenericType(contextType), null, "GET");
                XmlDocument document = request.GetResponseStreamAsXmlDocument();

                // Validate the metadata
                MemoryStream stream = new MemoryStream();
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(document.OuterXml);
                    writer.Flush();
                    stream.Position = 0;
                    MetadataUtil.IsValidMetadata(stream, null);
                }

                // Validate the xpaths
                string[] xpaths = new string[] {
                    "//csdl:Function[@Name='IntServiceOperation' and csdl:ReturnType[@Type='Edm.Int32']]",
                    "//csdl:FunctionImport[@Name='IntServiceOperation']",
                    "//csdl:Action[@Name='InsertCustomer' and csdl:ReturnType[@Type='AstoriaUnitTests.Stubs.Customer'] and " +
                        "csdl:Parameter[@Name='id' and @Type='Edm.Int32'] and csdl:Parameter[@Name='name' and @Type='Edm.String']]",
                    "//csdl:ActionImport[@Name='InsertCustomer' and @EntitySet='Customers']",
                    "//csdl:Function[@Name='GetCustomerByCity' and csdl:ReturnType[@Type='Collection(AstoriaUnitTests.Stubs.Customer)'] and " +
                        "csdl:Parameter[@Name='city' and @Type='Edm.String']]",
                    "//csdl:FunctionImport[@Name='GetCustomerByCity' and @EntitySet='Customers']"};

                UnitTestsUtil.VerifyXPaths(document, xpaths);
            }
        }
        [Ignore] // Remove Atom
        // [TestMethod, Description("Assert were getting fired when If-Match or If-None-Match header was specified for singleton service operations")]
        public void ServiceOperationsETagNotAllowed()
        {
            string[] uriList = new string[]
                {
                    "/GetSingleCustomerWithId?id=1", "/GetSingleQueryableCustomer?id=1"
                };

            foreach (string uri in uriList)
                foreach (string format in new string[] { UnitTestsUtil.AtomFormat, UnitTestsUtil.JsonLightMimeType })
                    foreach (var etagHeader in new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("If-Match", "W/\"foo\""),
                    new KeyValuePair<string, string>("If-None-Match", "W/\"bar\"") })
                    {
                        UnitTestsUtil.SendRequestAndVerifyXPath(
                            null /*payload*/,
                            uri,
                            null,
                            typeof(ServiceOperationsSyntaxService),
                            format,
                            "GET",
                            new KeyValuePair<string, string>[] { etagHeader },
                            false /*verifyETag*/);
                    }
        }
        [Ignore] // Remove Atom
        // [TestMethod]
        public void ServiceOperationReturningNullShouldThrow404()
        {
            // Invalid uri should return 404
            UnitTestsUtil.VerifyInvalidRequest(null, "/ManyCustomers(1100)?name='foo'&id=1", typeof(ServiceOperationsSyntaxService), UnitTestsUtil.AtomFormat, "GET", 404, DataServicesResourceUtil.GetString("RequestUriProcessor_ResourceNotFound", "ManyCustomers"));

            // Service operation returning null should throw 404
            UnitTestsUtil.VerifyInvalidRequest(null, "/GetSingleCustomerWithId?id=11", typeof(ServiceOperationsSyntaxService), UnitTestsUtil.AtomFormat, "GET", 404, DataServicesResourceUtil.GetString("RequestUriProcessor_ResourceNotFound", "GetSingleCustomerWithId"));

            // Service operation returning null should throw 404
            UnitTestsUtil.VerifyInvalidRequest(null, "/GetSingleQueryableCustomer?id=11", typeof(ServiceOperationsSyntaxService), UnitTestsUtil.AtomFormat, "GET", 404, DataServicesResourceUtil.GetString("RequestUriProcessor_ResourceNotFound", "GetSingleQueryableCustomer"));
        }
        [Ignore] // Remove Atom
        // [TestMethod]
        public void ServiceOperationSerializationWithSingleEntity()
        {
            var atomXPaths = new string[]
            {
                String.Format("/atom:entry[atom:category/@term='#{0}' and atom:id='http://host/Customers(1000)' and atom:content/adsm:properties[ads:Name='Foo' and ads:ID=1000 and ads:Address[ads:StreetAddress='Line1' and ads:City='Redmond' and ads:PostalCode='98052']]]",
                    typeof(Customer).FullName)
            };


            var jsonLiteXPaths = new string[]
            {
                String.Format("/{0}[odata.context='http://host/$metadata#Customers/$entity' and ID=1000 and Name='Foo' and Address/StreetAddress='Line1' and Address/City='Redmond' and Address/State='WA' and Address/PostalCode='98052']",
                    JsonValidator.ObjectString)
            };

            TestServiceOperationSerialization("/GetSingleCustomer", atomXPaths, jsonLiteXPaths);
        }
        [Ignore] // Remove Atom
        // [TestMethod]
        public void ServiceOperationSerializationWithCollectionOfEntities()
        {
            var atomXPaths = new string[]
            {
                String.Format("/atom:feed/atom:entry[atom:category/@term='#{0}' and atom:id='http://host/Customers(1001)' and atom:content/adsm:properties[ads:Name='name2' and ads:ID=1001 and ads:Address[ads:StreetAddress='Line1' and ads:City='Redmond' and ads:PostalCode='98052']]]",
                    typeof(Customer).FullName)
            };

            var jsonLiteXPaths = new string[]
            {
                String.Format("//{0}[odata.context='http://host/$metadata#Customers' and value/{1}/{2}[ID=1001 and Name='name2' and Address/StreetAddress='Line1' and Address/City='Redmond' and Address/State='WA' and Address/PostalCode='98052']]",
                    JsonValidator.ObjectString,
                    JsonValidator.ArrayString,
                    JsonValidator.ObjectString)
            };

            TestServiceOperationSerialization("/TheEnumerableCustomerMulti", atomXPaths, jsonLiteXPaths);
        }
        [Ignore] // Remove Atom
        // [TestMethod]
        public void ServiceOperationSerializationWithSingleComplexType()
        {
            string serviceOpName = "GetSingleComplexType";

            var atomXPaths = new string[]
            {
                String.Format("/adsm:value[ads:StreetAddress='One Microsoft Way' and ads:City='Redmond' and ads:PostalCode='98052']")
            };

            var jsonLiteXPaths = new string[]
            {
                String.Format("/{0}[odata.context='http://host/$metadata#AstoriaUnitTests.Stubs.Address' and StreetAddress='One Microsoft Way' and City='Redmond' and State='WA' and PostalCode='98052']",
                    JsonValidator.ObjectString)
            };

            TestServiceOperationSerialization(string.Format("/{0}", serviceOpName), atomXPaths, jsonLiteXPaths);
        }
        [Ignore] // Remove Atom
        // [TestMethod]
        public void ServiceOperationSerializationWithCollectionOfComplexTypes()
        {
            string serviceOpName = "GetComplexTypeCollection";

            var atomXPaths = new string[]
            {
                String.Format("/adsm:value/adsm:element[ads:StreetAddress='One Microsoft Way' and ads:City='Redmond' and ads:PostalCode='98052']")
            };

            var jsonLiteXPaths = new string[]
            {
                String.Format("//{0}[odata.context='http://host/$metadata#Collection(AstoriaUnitTests.Stubs.Address)' and value/{1}/{2}[StreetAddress='One Microsoft Way' and City='Redmond' and State='WA' and PostalCode='98052']]",
                    JsonValidator.ObjectString,
                    JsonValidator.ArrayString,
                    JsonValidator.ObjectString)
            };

            TestServiceOperationSerialization(string.Format("/{0}", serviceOpName), atomXPaths, jsonLiteXPaths);
        }
        [Ignore] // Remove Atom
        // [TestMethod]
        public void ServiceOperationSerializationWithSinglePrimitive()
        {
            string serviceOpName = "GetSinglePrimitive";

            var atomXPaths = new string[]
            {
                String.Format("/adsm:value[.='23']")
            };

            var jsonLiteXPaths = new string[]
            {
                String.Format("/{0}[odata.context='http://host/$metadata#Edm.Int32' and value='23']",
                    JsonValidator.ObjectString,
                    serviceOpName)
            };

            TestServiceOperationSerialization(string.Format("/{0}", serviceOpName), atomXPaths, jsonLiteXPaths);
        }
        [Ignore] // Remove Atom
        // [TestMethod]
        public void ServiceOperationSerializationWithCollectionOfPrimitives()
        {
            string serviceOpName = "GetPrimitiveCollection";

            var atomXPaths = new string[]
            {
                String.Format("/adsm:value[adsm:element='String2']")
            };

            var jsonLiteXPaths = new string[]
            {
                String.Format("//{0}[odata.context='http://host/$metadata#Collection(Edm.String)' and value/{1}[{2}='String2']]",
                    JsonValidator.ObjectString,
                    JsonValidator.ArrayString,
                    JsonValidator.ObjectString)
            };

            TestServiceOperationSerialization(string.Format("/{0}", serviceOpName), atomXPaths, jsonLiteXPaths);
        }

        private static void TestServiceOperationSerialization(string uri, string[] atomXPaths, string[] jsonLiteXPaths)
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                Type providerType = typeof(ServiceOperationsSyntaxService);
                request.ServiceType = providerType;
                request.StartService();

                SendAndVerifyResults(atomXPaths, uri, UnitTestsUtil.AtomFormat);
                SendAndVerifyResults(jsonLiteXPaths, uri, UnitTestsUtil.JsonLightMimeType);
            }
        }

        private static void SendAndVerifyResults(string[] xPaths, string uri, string responseFormat)
        {
            UnitTestsUtil.SendRequestAndVerifyXPath(null, uri, new KeyValuePair<string, string[]>[] { new KeyValuePair<string, string[]>(uri, xPaths) }, typeof(ServiceOperationsSyntaxService), responseFormat, "GET");
        }

        public class ServiceOperationsSyntaxService : OpenWebDataService<CustomDataContext>
        {
            [WebGet]
            public IEnumerable<Customer> EnumerableCustomer()
            {
                return new List<Customer>() { new Customer() { Name = "name", ID = 1000 } };
            }

            [WebGet]
            public IEnumerable<Customer> TheEnumerableCustomer()
            {
                return new Customer[] { new Customer() { Name = "name", ID = 1000 } };
            }

            [WebGet]
            public IEnumerable<Customer> TheEnumerableCustomerReturningQueryableInstance()
            {
                return new Customer[] { new Customer() { Name = "name", ID = 1000 } }.AsQueryable();
            }

            [WebGet]
            public IEnumerable<Customer> TheEnumerableCustomerMulti()
            {
                return new Customer[] { new Customer() { Name = "name", ID = 1000 }, new Customer() { Name = "name2", ID = 1001 } };
            }

            [WebGet]
            [SingleResult]
            public IQueryable<Customer> SingleCustomer(string name, int id)
            {
                return new List<Customer>() { new Customer() { Name = name, ID = 1000 } }.AsQueryable();
            }

            [WebGet]
            public Customer GetSingleCustomer()
            {
                return new Customer() { Name = "Foo", ID = 1000 };
            }

            [WebGet]
            public Customer GetSingleCustomerWithId(int id)
            {
                return this.CurrentDataSource.Customers.Where(c => c.ID == id).FirstOrDefault();
            }

            [WebGet]
            [SingleResult]
            public IQueryable<Customer> GetSingleQueryableCustomer(int id)
            {
                return this.CurrentDataSource.Customers.Where(c => c.ID == id);
            }

            [WebGet]
            public IQueryable<Customer> ManyCustomers(string name, int id)
            {
                return new List<Customer>() { new Customer() { Name = name, ID = 1000 } }.AsQueryable();
            }

            [WebGet]
            public IQueryable<Customer> CustomersWithDerivedTypeIdentiferSegment()
            {
                return new List<Customer>() {
                    new Customer() { Name = "Phani", ID = 1000 } ,
                    new CustomerWithBirthday() { Name = "Raju", ID = 1001 }
                    }.AsQueryable();
            }

            [WebGet]
            public IEnumerable<Address> GetComplexTypeCollection()
            {
                return new List<Address>()
                {
                    new Address() { City = "Redmond", PostalCode = "98052", State = "WA", StreetAddress = "One Microsoft Way" },
                    new Address() { City = "Bellevue", PostalCode = "98004", State = "WA", StreetAddress = "100 Bellevue Way" }
                };
            }

            [WebGet]
            public Address GetSingleComplexType()
            {
                return new Address() { City = "Redmond", PostalCode = "98052", State = "WA", StreetAddress = "One Microsoft Way" };
            }

            [WebGet]
            public IEnumerable<string> GetPrimitiveCollection()
            {
                return new List<string>() { "String1", "String2" };
            }

            [WebGet]
            public int GetSinglePrimitive()
            {
                return 23;
            }
        }

        private static bool CheckValidMetadata(TestWebRequest request, Type webAttributeType, params bool[] exceptionExpected)
        {
            Exception exception = TestUtil.RunCatching(delegate ()
            {
                request.RequestUriString = "/$metadata";
                request.SendRequest();
                Stream stream = TestUtil.EnsureStreamWithSeek(request.GetResponseStream());
                // Trace.WriteLine(new StreamReader(stream).ReadToEnd());
                stream.Position = 0;

                var metadata = MetadataUtil.IsValidMetadata(request.GetResponseStream(), null);
                if (webAttributeType != null)
                {
                    Assert.IsTrue(webAttributeType == typeof(WebGetAttribute) ||
                        webAttributeType == typeof(WebInvokeAttribute), "Only Get and Invoke are valid values for the attribute");

                    IEdmEntityContainer entityContainer = metadata.EntityContainer;
                    if (entityContainer != null)
                    {
                        IList<IEdmOperationImport> operationImports = (IList<IEdmOperationImport>)entityContainer.OperationImports().ToList();
                        Assert.IsTrue(operationImports.Count == 1, "there must be exactly one function import");
                    }
                }
                Assert.IsTrue(metadata != null);
            });
            TestUtil.AssertExceptionExpected(exception, exceptionExpected);
            return exception == null;
        }

        private static void CheckValidMetadata(Type serviceType, Type webAttribute, bool exceptionExpected)
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.ServiceType = serviceType;
                CheckValidMetadata(request, webAttribute, exceptionExpected);
            }
        }

        public static Type CreateServiceType(
            int id,
            Type resultTypeKind,
            Type elementType,
            bool isAbstract,
            bool singleResult,
            Type webAttribute,
            Type baseTypeForContext,
            string methodName,
            params Type[] parameterType)
        {
            MethodAttributes methodAttributes = MethodAttributes.Public |
                ((isAbstract) ? MethodAttributes.Abstract | MethodAttributes.Virtual : MethodAttributes.Final);

            Type returnType;
            if (resultTypeKind == typeof(void))
            {
                returnType = typeof(void);
            }
            else if (resultTypeKind == typeof(IQueryable))
            {
                returnType = typeof(IQueryable<>).MakeGenericType(elementType);
            }
            else if (resultTypeKind == typeof(IEnumerable))
            {
                returnType = typeof(IEnumerable<>).MakeGenericType(elementType);
            }
            else
            {
                Debug.Assert(resultTypeKind == typeof(object), "resultTypeKind == typeof(object)");
                returnType = elementType;
            }

            Type baseTypeForService;
            TypeBuilder dataServiceBuilder;
            CreateDataServiceTypeBuilder(id, isAbstract, baseTypeForContext, "DynamicService", out baseTypeForService, out dataServiceBuilder);
            MethodBuilder methodBuilder = dataServiceBuilder.DefineMethod(methodName, methodAttributes);
            methodBuilder.SetReturnType(returnType);

            if (parameterType != null && parameterType.Length != 0)
            {
                methodBuilder.SetParameters(parameterType);
                for (int i = 1; i <= parameterType.Length; i++)
                {
                    methodBuilder.DefineParameter(i, ParameterAttributes.In, "param" + i);
                }
            }

            if (!isAbstract)
            {
                string typeNamePart;
                string typeKindPart;
                string cardinalityPart;

                if (resultTypeKind == typeof(void))
                {
                    typeKindPart = "Void";
                }
                else if (resultTypeKind == typeof(IQueryable))
                {
                    typeKindPart = "Queryable";
                }
                else if (resultTypeKind == typeof(IEnumerable))
                {
                    typeKindPart = "Enumerable";
                }
                else
                {
                    Debug.Assert(resultTypeKind == typeof(object), "resultTypeKind == typeof(object)");
                    typeKindPart = "";
                }

                if (resultTypeKind == typeof(void))
                {
                    typeNamePart = "";
                }
                else if (elementType == typeof(string))
                {
                    typeNamePart = "String";
                }
                else
                {
                    typeNamePart = "Customer";
                }

                cardinalityPart = (singleResult) ? "Single" : "Multiple";

                string baseMethodName = typeNamePart + typeKindPart + cardinalityPart + "Method";
                MethodInfo baseMethod = baseTypeForContext.GetMethod(baseMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (baseMethod == null)
                {
                    throw new InvalidOperationException("Unable to find method '" + baseMethodName + "' on type '" + baseTypeForContext + "'");
                }

                Trace.WriteLine("Base method used for action is '" + baseMethod.ToString() + "'.");
                MethodInfo getCurrentDataSource = baseTypeForService.GetMethod("get_CurrentDataSource", BindingFlags.Instance | BindingFlags.NonPublic);
                ILGenerator generator = methodBuilder.GetILGenerator();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Call, getCurrentDataSource);
                if (parameterType != null && parameterType.Length != 0)
                {
                    for (byte x = 0; x < parameterType.Length; x++)
                    {
                        generator.Emit(OpCodes.Ldarg_S, x);
                    }
                }
                generator.Emit(OpCodes.Call, baseMethod);
                generator.Emit(OpCodes.Ret);
            }

            if (singleResult)
            {
                CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(
                    typeof(SingleResultAttribute).GetConstructor(Type.EmptyTypes), TestUtil.EmptyObjectArray);
                methodBuilder.SetCustomAttribute(attributeBuilder);
            }

            if (webAttribute != null)
            {
                CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(
                    webAttribute.GetConstructor(Type.EmptyTypes), TestUtil.EmptyObjectArray);
                methodBuilder.SetCustomAttribute(attributeBuilder);
            }

            Type serviceType = dataServiceBuilder.CreateType();
            return serviceType;
        }

        private static void CreateDataServiceTypeBuilder(int id, bool isAbstract, Type baseTypeForContext, string baseName, out Type baseTypeForService, out TypeBuilder dataServiceBuilder)
        {
            ModuleBuilder module = TestUtil.CreateModuleBuilder(baseName + id);
            CreateDataServiceTypeBuilder(module, id, isAbstract, baseTypeForContext, baseName, out baseTypeForService, out dataServiceBuilder);
        }

        private static void CreateDataServiceTypeBuilder(ModuleBuilder module, int id, bool isAbstract, Type baseTypeForContext, string baseName, out Type baseTypeForService, out TypeBuilder dataServiceBuilder)
        {
            const TypeAttributes dataContextAttributes = TypeAttributes.Class | TypeAttributes.Public;
            baseTypeForService = typeof(OpenWebDataService<>).MakeGenericType(baseTypeForContext);
            dataServiceBuilder = module.DefineType(
                "Service" + id,
                dataContextAttributes | ((isAbstract) ? TypeAttributes.Abstract : default(TypeAttributes)),
                baseTypeForService);
        }

        internal static TypeBuilder CreateServiceTypeForParameterTesting(ModuleBuilder module, int id, string methodName, Type parameterType, int parameterCount)
        {
            return CreateServiceTypeForParameterTesting(module, id, methodName, parameterType, parameterCount, typeof(WebGetAttribute));
        }

        internal static TypeBuilder CreateServiceTypeForParameterTesting(ModuleBuilder module, int id, string methodName, Type parameterType, int parameterCount, Type attribute)
        {
            Type baseTypeForService;
            TypeBuilder dataServiceBuilder;
            Type baseTypeForContext = typeof(CustomDataContext);
            CreateDataServiceTypeBuilder(module, id, false, baseTypeForContext, "ServiceForParamTesting", out baseTypeForService, out dataServiceBuilder);

            Type[] parameterTypes = new Type[parameterCount];
            for (int i = 0; i < parameterCount; i++)
            {
                parameterTypes[i] = parameterType;
            }

            MethodBuilder methodBuilder = dataServiceBuilder.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.Final, typeof(void), parameterTypes);
            methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(attribute.GetConstructor(Type.EmptyTypes), new object[0]));

            for (int i = 0; i < parameterCount; i++)
            {
                methodBuilder.DefineParameter(i + 1, ParameterAttributes.In, "p" + i);
            }

            BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            ILGenerator generator = methodBuilder.GetILGenerator();
            MethodInfo baseMethod = baseTypeForContext.GetMethod("SetLastParameters", flags);
            LocalBuilder arrayBuilder = generator.DeclareLocal(typeof(object[]));
            arrayBuilder.SetLocalSymInfo("paramsArray");

            // Create the array.
            generator.Emit(OpCodes.Ldc_I4, parameterCount);
            generator.Emit(OpCodes.Newarr, typeof(object));
            generator.Emit(OpCodes.Stloc, arrayBuilder);

            // Populate the array.
            for (int i = 0; i < parameterCount; i++)
            {
                generator.Emit(OpCodes.Ldloc, arrayBuilder);
                generator.Emit(OpCodes.Ldc_I4, i);
                generator.Emit(OpCodes.Ldarg, i + 1);

                if (!parameterType.IsClass) // && Nullable.GetUnderlyingType(parameterType) == null)
                {
                    // Box first before storing the item.
                    generator.Emit(OpCodes.Box, parameterType);
                }

                generator.Emit(OpCodes.Stelem_Ref);
            }

            // Call the base method to store the items.
            generator.Emit(OpCodes.Ldloc, arrayBuilder);
            generator.Emit(OpCodes.Call, baseMethod);

            generator.Emit(OpCodes.Ret);

            return dataServiceBuilder;
        }
    }
}
