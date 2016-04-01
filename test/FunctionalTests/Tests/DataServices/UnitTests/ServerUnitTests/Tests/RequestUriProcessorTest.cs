//---------------------------------------------------------------------
// <copyright file="RequestUriProcessorTest.cs" company="Microsoft">
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
    using System.Net;
    using System.Reflection;
    using System.Xml;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NorthwindModel;

    #endregion Namespaces

    /// <summary>
    /// This is a test class for RequestUriProcessor and is intended to contain all RequestUriProcessor Unit Tests.
    /// </summary>
    [TestClass]
    public class RequestUriProcessorTest
    {
        //[TestMethod]
        //public void RemoteFoo()
        //{
        //    // TODO: enable local IIS7 detection.
        //    ServiceModelData.Northwind.EnsureDependenciesAvailable();
        //    using (TestWebRequest request = TestWebRequest.CreateForRemote())
        //    {
        //        request.DataServiceType = typeof(NorthwindContext);
        //        request.RequestUriString = "/Customers('A&B')";
        //        Exception exception = TestUtil.RunCatching(request.SendRequest);
        //        TestUtil.AssertExceptionExpected(exception, true);
        //        TestUtil.AssertExceptionStatusCode(exception, 404, "/Customers('A&B') should return a 404 result");
        //    }
        //}

        [TestMethod]
        public void RequestUriCaseInsensitive()
        {
            // Repro: Path to the .svc file shoud not be case sensitive.
            WebServerLocation[] locations = new WebServerLocation[]
            {
                WebServerLocation.InProcessWcf,
                WebServerLocation.Local
            };
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("location", locations));
            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
            {
                WebServerLocation location = (WebServerLocation)values["location"];
                using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/Customers";
                    request.FullRequestUriString = request.FullRequestUriString
                        .Replace(".svc", ".SvC")
                        .Replace("Test", "test");
                    request.SendRequest();
                    string response = request.GetResponseStreamAsText();
                    Trace.WriteLine(response);
                }
            });
        }

        [TestMethod]
        public void RequestUriProcessorKeySpecialCharsTest()
        {
            ServiceModelData.Northwind.EnsureDependenciesAvailable();

            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("WebServerLocation", TestWebRequest.LocalWebServerLocations));
            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
            {
                WebServerLocation location = (WebServerLocation)values["WebServerLocation"];
                using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
                {
                    Exception exception;
                    request.DataServiceType = typeof(NorthwindContext);

                    int expectedStatusCode;
                    if (location == WebServerLocation.InProcess)
                    {
                        expectedStatusCode = 404;
                    }
                    else
                    {
                        // See http://support.microsoft.com/kb/820129 for more information,
                        // including how to apply the changes.
                        // HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\HTTP\Parameters
                        int keyValue;
                        using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Services\HTTP\Parameters"))
                        {
                            keyValue = (int)key.GetValue("AllowRestrictedChars", 0);
                        }

                        expectedStatusCode = (keyValue == 0) ? 400 : 404;
                    }

                    // Check that control characters are accepted.
                    request.RequestUriString = "/Customers('\x003')";
                    exception = TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionStatusCode(exception, expectedStatusCode, "404 expected for not-found entity with control character in key");

                    // Check that other special characters are accepted.
                    if (location == WebServerLocation.InProcess || location == WebServerLocation.InProcessWcf)
                    {
                        expectedStatusCode = 404;
                    }
                    else
                    {
                        // NOTE: this would work on IIS, but Cassini doesn't use this registry key.
                        // See http://support.microsoft.com/kb/932552 for more information,
                        // including how to apply the changes.
                        // HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\ASP.NET
                        //int keyValue;
                        //using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\ASP.NET"))
                        //{
                        //    keyValue = (int)key.GetValue("VerificationCompatibility", 0);
                        //}

                        //expectedStatusCode = (keyValue == 0) ? 400 : 404;
                        expectedStatusCode = 400;
                    }

                    request.RequestUriString = "/Customers('.:')";
                    exception = TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionStatusCode(exception, expectedStatusCode, "404 expected for not-found entity with special characters in key");
                }
            });
        }

        /// <summary>This test verifies that special real keywords (Infinity, -Infinity, NaN) are covered.</summary>
        [TestMethod]
        public void RequestUriProcessorKeySpecialRealTest()
        {
            double[] doubleValues = new double[] { double.PositiveInfinity, double.NegativeInfinity, double.NaN };
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("doubleValue", doubleValues));
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(TypedCustomDataContext<TypedEntity<double, string>>);
                TypedCustomDataContext<TypedEntity<double, string>>.ClearHandlers();
                TypedCustomDataContext<TypedEntity<double, string>>.ClearValues();
                TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                {
                    double doubleValue = (double)values["doubleValue"];
                    TypedCustomDataContext<TypedEntity<double, string>>.ValuesRequested += (sender, args) =>
                    {
                        TypedCustomDataContext<TypedEntity<double, string>> s = (TypedCustomDataContext<TypedEntity<double, string>>)sender;
                        TypedEntity<double, string> entity = new TypedEntity<double, string>();
                        entity.ID = doubleValue;
                        s.SetValues(new object[] { entity });
                    };
                    try
                    {
                        request.RequestUriString = "/Values";
                        request.Accept = "application/atom+xml,application/xml";
                        request.SendRequest();
                        XmlDocument document = request.GetResponseStreamAsXmlDocument();
                        XmlElement element = TestUtil.AssertSelectSingleElement(document, "/atom:feed/atom:entry/atom:id");

                        Trace.WriteLine("Found ID: " + element.InnerText);
                        request.FullRequestUriString = element.InnerText;
                        Exception exception = TestUtil.RunCatching(request.SendRequest);

                        // One NaN value won't match another except throug the use of the .IsNaN
                        // method. It's probably OK to not support this.
                        TestUtil.AssertExceptionExpected(exception, double.IsNaN(doubleValue));

                        string responseText = request.GetResponseStreamAsText();
                        Trace.WriteLine(responseText);
                    }
                    finally
                    {
                        TypedCustomDataContext<TypedEntity<double, string>>.ClearHandlers();
                        TypedCustomDataContext<TypedEntity<double, string>>.ClearValues();
                    }
                });
            }
        }

        [TestMethod]
        public void RequestUriProcessorEmptySegments()
        {
            // This test reproes: extra / are not ignored, http://host/service//$metadata
            string[] uris = new string[]
            {
                "/",
                "/$metadata",
                "//$metadata",
                "//",
                "///",
                "//Values//",
            };
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(TypedCustomDataContext<TypedEntity<int, string>>);
                foreach (string uri in uris)
                {
                    Trace.WriteLine("Requesting " + uri);
                    request.RequestUriString = uri;
                    request.Accept = "application/atom+xml,application/xml";
                    request.SendRequest();
                    var doc = request.GetResponseStreamAsXmlDocument();
                    if (uri == "/$metadata")
                    {
                        TestUtil.AssertSelectNodes(doc, "/edmx:Edmx/edmx:DataServices[0 = count(@adsm:DataServiceVersion)]");
                    }
                }
            }
        }

        [TestMethod]
        public void RequestUriProcessorEnumerateSegmentsBasicTest()
        {
            VerifyInvokeEnumerateSegments("/abc", "abc");
            VerifyInvokeEnumerateSegments("/abc/pqr", "abc", "pqr");
            VerifyInvokeEnumerateSegments("/abc/pqr?", "abc", "pqr");
            VerifyInvokeEnumerateSegments("/abc/pqr?q=1", "abc", "pqr");
            VerifyInvokeEnumerateSegments("/abc/pqr%5a", "abc", "pqr\x5a");
            VerifyInvokeEnumerateSegments("/abc/pqr%5A", "abc", "pqr\x5a");
            VerifyInvokeEnumerateSegments("/abc/doc.htm#abc", "abc", "doc.htm");
            VerifyInvokeEnumerateSegments("/abc/doc.htm/", "abc", "doc.htm");
            VerifyInvokeEnumerateSegments("/abc/doc.htm//", "abc", "doc.htm");
            VerifyInvokeEnumerateSegments("/");
            VerifyInvokeEnumerateSegments("/#");
            VerifyInvokeEnumerateSegments("/?#");
            VerifyInvokeEnumerateSegments("/abc(123)", "abc(123)");
            VerifyInvokeEnumerateSegments("/abc!123", "abc!123");
            VerifyInvokeEnumerateSegments("/(", "(");
            VerifyInvokeEnumerateSegments("/%55", "\x55");
        }

        [TestMethod]
        public void RequestUriProcessorNullNavigationTest()
        {
            string[] uris = new string[]
            {
                "/Customer(100)/BestFriend/Name",
                "/Customer(100)/Orders",
                "/Customer(100)/Orders(1)",
                "/Customer(100)/Orders(1)/DollarAmount",
                // "/Customer(100)/BestFriend/BestFriend/Name",
            };

            using (CustomDataContext.CreateChangeScope())
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(CustomDataContext);

                var c = new CustomDataContext();
                c.InternalCustomersList.Add(new Customer() { ID = 100, BestFriend = null, Orders = null });
                c.SaveChanges();

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(new Dimension("URI", uris));
                TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                {
                    string uri = (string)values["URI"];
                    request.RequestUriString = "/Customer(100)/BestFriend/Name";
                    Exception exception = TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionStatusCode(exception, 404, "404 expected for " + uri);
                });
            }
        }

        [TestMethod]
        public void RequestUriProcessorNotFoundErrorTest()
        {
            string[] requestValues = new string[]
            {
                // Traversing a property.
                "/Customers(1)/Name/More",

                // Non-existent property
                "/Customers(1)/SomeName",

                // Traversing a $value.
                "/Customers(1)/Name/$value/More",

                "/Customers%n0",
            };

            foreach (string responseFormat in UnitTestsUtil.ResponseFormats)
            {
                using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcess))
                {
                    request.Accept = responseFormat;
                    request.DataServiceType = typeof(CustomDataContext);
                    foreach (string requestValue in requestValues)
                    {
                        request.RequestUriString = requestValue;
                        VerifyResourceNotFoundError(request);
                    }
                }
            }
        }

        [TestMethod]
        public void RequestUriProcessorDollarValueMustComeAfterMleError()
        {
            string[] requestValues = new string[]
            {
                // $value after a non-MLE entity should fail.
                "/Customers(1)/$value",
            };

            foreach (string responseFormat in UnitTestsUtil.ResponseFormats)
            {
                using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcess))
                {
                    request.Accept = responseFormat;
                    request.DataServiceType = typeof(CustomDataContext);
                    foreach (string requestValue in requestValues)
                    {
                        request.RequestUriString = requestValue;
                        Exception e = TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNotNull(e);
                        Assert.AreEqual(400, request.ResponseStatusCode);
                        Assert.AreEqual(typeof(DataServiceException), e.InnerException.GetType());
                        Assert.AreEqual("The URI 'http://host/Customers(1)/$value' is not valid. The segment before '$value' must be a Media Link Entry or a primitive property.", e.InnerException.Message);
                    }
                }
            }
        }

        [TestMethod]
        public void RequestUriProcessExtraParensTest()
        {
            // Repro for: Need to add support for () to denote collections to URI syntax
            CompareIds("/Customers", "/Customers()");
            CompareIds("/Customers(1)/Orders", "/Customers(1)/Orders()");
            CompareIds("/Customers(2)/BestFriend/BestFriend/Orders", "/Customers(2)/BestFriend/BestFriend/Orders()");
            CompareIds("/Customers?$expand=Orders", "/Customers()?$expand=Orders");
        }

        private static void CompareIds(string uri, string uri1)
        {
            string xpath = "/atom:feed/atom:id";
            using (CustomDataContext.CreateChangeScope())
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.RequestUriString = uri;
                request.Accept = "application/atom+xml,application/xml";
                request.SendRequest();
                XmlDocument document = request.GetResponseStreamAsXmlDocument();
                string id = document.SelectSingleNode(xpath, TestUtil.TestNamespaceManager).InnerText;

                request.DataServiceType = typeof(CustomDataContext);
                request.RequestUriString = uri1;
                request.SendRequest();
                document = request.GetResponseStreamAsXmlDocument();
                string id1 = document.SelectSingleNode(xpath, TestUtil.TestNamespaceManager).InnerText;

                Assert.AreEqual(id, id1);
                Assert.IsFalse(id.Contains("()"));
            }
        }

        [TestMethod]
        public void RequestUriProcessorSyntaxErrorTest()
        {
            string[] requestValues = new string[]
            {
                // Wrong number of keys.
                "/Customers(1,2)",

                //// Wrong key type.
                "/Customers(a)",
                // "/Customers(1).", // NOTE: trailing period not significant to System.Uri
                "/Customers(1.0)",

                // Wrong paren and quote format.
                "/Customers(1(",
                "/Customers(1",
                "/Customers('1",
                "/Customers('1')",

                // Traversing through non-single element.
                "/Customers/Customer",

                // Wrong place to put key information.
                "/Customers(1)/Name(1)",
                "/Customers(1)/Name/$value(1)",

                // Starting with .Net 4.5, Uri.UnescapeDataString does not throw an exception on invalid sequence. It just does not unescape something
                // that it does not understand. The url below fails with 404 segment not found instead of 400. Hence disabling this scenario.
                // Malformed Uri
                // "/Customers%FF",
            };

            foreach (string responseFormat in UnitTestsUtil.ResponseFormats)
            {
                using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcess))
                {
                    request.Accept = responseFormat;
                    request.DataServiceType = typeof(CustomDataContext);
                    foreach (string requestValue in requestValues)
                    {
                        request.RequestUriString = requestValue;
                        VerifyRequestSyntaxError(request);
                    }
                }
            }
        }

        [TestMethod]
        public void RequestUriProcessorMetadataTest()
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("UriString", new string[] { "", "/", "/$metadata" }));
            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
            {
                string s = (string)values["UriString"];
                using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcess))
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.Accept = "application/atom+xml,application/xml";
                    request.RequestUriString = s;
                    request.SendRequest();
                    string expectedContentType = "application/xml";
                    Assert.AreEqual(expectedContentType, TestUtil.GetMediaType(request.ResponseContentType));
                }
            });
        }

        [TestMethod]
        public void RequestUriProcessUnquotedTest()
        {
            string[] invalidUris = new string[]
            {
                "/Territories(1)",
                "/Customers(1)",
                "/Orders('1')",
            };
            ServiceModelData.Northwind.EnsureDependenciesAvailable();
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("uri", invalidUris));
            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(NorthwindContext);
                    request.RequestUriString = (string)values["uri"];
                    Exception exception = TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(exception, true);
                    TestUtil.AssertExceptionStatusCode(exception, 400, "Bad Request expected for query portion errors.");
                }
            });
        }

        [TestMethod]
        public void RequestUriResourceKeySimpleTest()
        {
            UnitTestsUtil.VerifyPayload("/Customers(1)", typeof(CustomDataContext), null,
                new string[] {
                    "/cdc:Customer",
                    "/cdc:Customer/cdc:ID[text()='1']",
                    "count(//cdc:Customer)=1"},
                new string[] {
                    JsonValidator.GetJsonTypeXPath(typeof(CustomerWithBirthday), false /*isArray*/),
                    String.Format("/{0}/ID[text()='1']", JsonValidator.ObjectString),
                    String.Format("count(//{0})=1", JsonValidator.ObjectString)},
                new string[0]);
        }

        [TestMethod]
        public void RequestUriResourcePropertyTest()
        {
            UnitTestsUtil.VerifyInvalidUri("/Customers(1000)/BestFriend", typeof(CustomDataContext));

            UnitTestsUtil.VerifyPayload("/Customers(2)/BestFriend", typeof(CustomDataContext), null,
                new string[] {"/cdc:BestFriend",
                              "/cdc:BestFriend/web3s:ID[text()='1']"},
                new string[] { String.Format("/{0}", JsonValidator.ObjectString),
                               String.Format("/{0}/ID[text()='1']", JsonValidator.ObjectString) },
                new string[0]);

            ServiceModelData.Northwind.EnsureDependenciesAvailable();
            UnitTestsUtil.VerifyPayload("/Territories", typeof(NorthwindContext), null,
                new string[] { "//nc:Territories/web3s:ID" },
                new string[] { String.Format("/{0}/{1}/TerritoryID[text()='01581']", JsonValidator.ArrayString, JsonValidator.ObjectString),
                               String.Format("count(//{0})>10", JsonValidator.ObjectString) },
                new string[0]);
        }

        [TestMethod]
        public void RequestUriResourceSetPropertyTest()
        {
            Func<Hashtable, XmlDocument, bool> testCallBack = (values, document) =>
             {
                 string responseFormat = (string)values["ResponseFormat"];
                 string xPath;

                 if (responseFormat == UnitTestsUtil.JsonLightMimeType)
                 {
                     xPath = String.Format("/{0}/{1}/ID", JsonValidator.ArrayString, JsonValidator.ObjectString);
                 }
                 else
                 {
                     Assert.IsTrue(responseFormat == UnitTestsUtil.AtomFormat, "unexpected format");
                     xPath = "/atom:feed/atom:entry/atom:content/adsm:properties/ads:ID";
                 }

                 XmlElement order = (XmlElement)document.SelectSingleNode(xPath, TestUtil.TestNamespaceManager);

                 using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcess))
                 {
                     request.RequestUriString = "/Customers(0)/Orders(" + order.InnerText + ")";
                     request.Accept = responseFormat;
                     request.DataServiceType = typeof(CustomDataContext);
                     request.SendRequest();

                     UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), responseFormat,
                         new string[] { "/cdc:Order" },
                         new string[] { JsonValidator.GetJsonTypeXPath(typeof(Order), false /*isArray*/) },
                         new string[0]);

                     return true;
                 }
             };

            UnitTestsUtil.VerifyPayload("/Customers(0)/Orders", typeof(CustomDataContext), testCallBack,
                new string[] { "/cdc:Orders",
                               "/cdc:Orders/cdc:Order" },
                new string[] { String.Format("/{0}", JsonValidator.ArrayString),
                               JsonValidator.GetJsonTypeXPath(typeof(Order), true /*isArray*/) },
                new string[0]);

            // TODO: When this is fixed, we should uncomment this test cases
            //UnitTestsUtil.VerifyInvalidUri("/Customers!1000/Orders", typeof(CustomDataContext));
            UnitTestsUtil.VerifyInvalidUri("/Customers(1)/Orders(10000)", typeof(CustomDataContext));
        }

        [TestMethod]
        public void RequestUriComplexPropertyTest()
        {
            UnitTestsUtil.VerifyPayload("/Customers(0)/Address", typeof(CustomDataContext), null,
                new string[] { "/cdc:Address" },
                new string[] { String.Format("/{0}/Address/StreetAddress[text()='Line1']", JsonValidator.ObjectString),
                               String.Format("/{0}/Address/City[text()='Redmond']", JsonValidator.ObjectString) },
                new string[0]);

            UnitTestsUtil.VerifyPayload("/Customers(0)/Address/StreetAddress", typeof(CustomDataContext), null,
                new string[] { "/cdc:Line1" },
                new string[] { String.Format("/{0}/StreetAddress[text()='Line1']", JsonValidator.ObjectString) },
                new string[0]);
        }

        [TestMethod]
        public void RequestUriResourceKeyTest()
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("TypeData", TypeData.Values),
                new Dimension("UseSmallCasing", new bool[] { true, false }),
                new Dimension("UseDoublePostfix", new bool[] { true, false }));

            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
            {
                TypeData typeData = (TypeData)values["TypeData"];
                if (!typeData.IsTypeSupportedAsKey)
                {
                    return;
                }

                // TODO: when System.Uri handles '/' correctly, re-enable.
                if (typeData.ClrType == typeof(System.Xml.Linq.XElement))
                {
                    return;
                }

                Type entityType = typeof(TypedEntity<,>).MakeGenericType(typeData.ClrType, typeof(int));
                CustomDataContextSetup setup = new CustomDataContextSetup(entityType);
                object idValue = typeData.NonNullValue;
                if (idValue is byte[])
                {
                    // idValue = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
                    return;
                }

                bool useSmallCasing = (bool)values["UseSmallCasing"];
                bool useDoublePostFix = (bool)values["UseDoublePostfix"];

                if (!(idValue is double) && useDoublePostFix)
                {
                    return;
                }

                string valueAsString = TypeData.FormatForKey(idValue, useSmallCasing, useDoublePostFix);
                using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcess))
                {
                    Trace.WriteLine("Running with value: [" + valueAsString + "] for [" + typeData.ToString() + "]");
                    setup.Id = idValue;
                    setup.MemberValue = 1;
                    request.DataServiceType = setup.DataServiceType;
                    request.Accept = "application/atom+xml,application/xml";
                    request.RequestUriString = "/Values(" + valueAsString + ")";

                    Trace.WriteLine("RequestUriString: " + request.RequestUriString);
                    request.SendRequest();
                    request.GetResponseStreamAsXmlDocument();
                }
                setup.Cleanup();
            });
        }

        [TestMethod]
        public void RequestUriSpecialNumbersTest()
        {
            TypedCustomDataContext<AllTypes>.ClearHandlers();
            TypedCustomDataContext<AllTypes>.ClearValues();
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(TypedCustomDataContext<AllTypes>);
                string[] uris = new string[]
                {
                    "/Values(1)",
                    "/Values(-1)",
                };
                foreach (string uri in uris)
                {
                    Trace.WriteLine("Requesting URI " + uri);
                    request.RequestUriString = uri;
                    Exception exception = TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(exception, true);
                    TestUtil.AssertExceptionStatusCode(
                        exception,
                        404,
                        "Correctly parsed (but missing) entites should return 404.");
                }
            }
        }

        [TestMethod]
        public void RequestUriNamedKeyTest()
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("TypeData", TypeData.Values),
                new Dimension("UseSmallCasing", new bool[] { true, false }),
                new Dimension("UseDoublePostfix", new bool[] { true, false }));

            bool syntaxErrorTested = false;
            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
            {
                TypeData typeData = (TypeData)values["TypeData"];
                if (!typeData.IsTypeSupportedAsKey)
                {
                    return;
                }

                // TODO: when System.Uri handles '/' correctly, re-enable.
                if (typeData.ClrType == typeof(System.Xml.Linq.XElement))
                {
                    return;
                }

                Type entityType = typeof(DoubleKeyTypedEntity<,,>).MakeGenericType(typeData.ClrType, typeData.ClrType, typeof(int));
                CustomDataContextSetup setup = new CustomDataContextSetup(entityType);
                object idValue = typeData.NonNullValue;
                if (idValue is byte[])
                {
                    // idValue = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
                    return;
                }

                bool useSmallCasing = (bool)values["UseSmallCasing"];
                bool useDoublePostFix = (bool)values["UseDoublePostfix"];

                if (!(idValue is double) && useDoublePostFix)
                {
                    return;
                }

                string valueAsString = TypeData.FormatForKey(idValue, useSmallCasing, useDoublePostFix);

                TestUtil.ClearMetadataCache();
                using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcess))
                {
                    Trace.WriteLine("Running with value: [" + valueAsString + "] for [" + typeData.ToString() + "]");
                    setup.Id = idValue;
                    setup.SecondId = idValue;
                    setup.MemberValue = 1;

                    request.DataServiceType = setup.DataServiceType;
                    request.Accept = "application/atom+xml,application/xml";
                    request.RequestUriString = "/Values(FirstKey=" + valueAsString + ",SecondKey=" + valueAsString + ")";
                    request.SendRequest();
                    string response = request.GetResponseStreamAsText();
                    TestUtil.AssertContains(response, "/Values(FirstKey=");
                    TestUtil.AssertContains(response, ",SecondKey=");
                    if (!syntaxErrorTested)
                    {
                        syntaxErrorTested = true;
                        VerifyRequestSyntaxError(request,
                            "/Values(" + valueAsString + "," + valueAsString + ")");
                        VerifyRequestSyntaxError(request,
                            "/Values(SecondKey == " + valueAsString + " , FirstKey = " + valueAsString + " )");
                        VerifyRequestSyntaxError(request,
                            "/Values(ASecondKey = " + valueAsString + " , FirstKey = " + valueAsString + " )");
                        VerifyRequestSyntaxError(request,
                            "/Values(SecondKey = " + valueAsString + ")");
                        VerifyRequestSyntaxError(request,
                            "/Values(SecondKey = " + valueAsString + ",,FirstKey=" + valueAsString + ")");
                        VerifyRequestSyntaxError(request,
                            "/Values(SecondKey)");
                        VerifyRequestSyntaxError(request,
                            "/Values(SecondKey=,FirstKey=)");
                        VerifyRequestSyntaxError(request,
                            "/Values(SecondKey = " + valueAsString + ",FirstKey=" + valueAsString +
                            ",ThirdKey=" + valueAsString + ")");
                    }
                }
                setup.Cleanup();
            });
        }

        [TestMethod]
        public void RequestUriProjectPropertyTest()
        {
            UnitTestsUtil.VerifyPayload("/Customers(1)/Name", typeof(CustomDataContext), null,
                new string[] { "/cdc:Name",
                               "/cdc:Name[text()='Customer 1']" },
                new string[] { String.Format("/{0}/Name[text()='Customer 1']", JsonValidator.ObjectString) },
                new string[0]);
        }

        [TestMethod]
        public void RequestUriLinksReferenceTest()
        {
            UnitTestsUtil.VerifyPayload("/Customers(1)/BestFriend/$ref", typeof(CustomDataContext), UnitTestsUtil.MimeApplicationXml, null,
                new string[] {
                    "/adsm:ref[@id='http://host/Customers(0)']",
                    "count(//adsm:ref)=1"});

            UnitTestsUtil.VerifyPayload("/Customers(1)/BestFriend/$ref", typeof(CustomDataContext), UnitTestsUtil.JsonLightMimeType, null,
                new string[] {
                    String.Format("/{0}/odata.id[text()='http://host/Customers(0)']", JsonValidator.ObjectString),
                    String.Format("count(//{0}/odata.id)=1", JsonValidator.ObjectString)});
        }

        [TestMethod]
        public void RequestUriLinksCollectionTest()
        {
            UnitTestsUtil.VerifyPayload("/Customers(2)/Orders/$ref", typeof(CustomDataContext), UnitTestsUtil.AtomFormat, null,
                new string[] {"/atom:feed/adsm:ref[@id='http://host/Orders(2)']",
                              "/atom:feed/adsm:ref[@id='http://host/Orders(102)']",
                              "count(/atom:feed/adsm:ref)=2" });

            UnitTestsUtil.VerifyPayload("/Customers(2)/Orders/$ref", typeof(CustomDataContext), UnitTestsUtil.JsonLightMimeType, null,
                new string[] { String.Format("/{1}/value/{0}/{1}/odata.id[text()='http://host/Orders(2)']", JsonValidator.ArrayString, JsonValidator.ObjectString),
                               String.Format("/{1}/value/{0}/{1}/odata.id[text()='http://host/Orders(102)']", JsonValidator.ArrayString, JsonValidator.ObjectString),
                               String.Format("count(/{1}/value/{0}/{1}//odata.id)=2", JsonValidator.ArrayString, JsonValidator.ObjectString) });
        }

        [TestMethod]
        public void EdmValidNamesNotAllowedInUri()
        {
            DSPMetadata metadata = new DSPMetadata("Test", "TestNS");
            var entityType = metadata.AddEntityType("MyType", null, null, false);
            metadata.AddKeyProperty(entityType, "ID", typeof(int));
            metadata.AddPrimitiveProperty(entityType, "Pròjè_x00A2_tÎð瑞갂థ్క_x0020_Iiلإَّ", typeof(string));
            var resourceSet = metadata.AddResourceSet("EntitySet", entityType);
            metadata.SetReadOnly();

            DSPServiceDefinition service = new DSPServiceDefinition()
            {
                Metadata = metadata,
                Writable = true
            };

            DSPContext data = new DSPContext();
            service.CreateDataSource = (m) => { return data; };

            using (TestWebRequest request = service.CreateForInProcessWcf())
            {
                request.StartService();
                DataServiceContext context = new DataServiceContext(request.ServiceRoot);
                //context.EnableAtom = true;
                //context.Format.UseAtom();

                string value = "value of Pròjè_x00A2_tÎð瑞갂థ్క_x0020_Iiلإَّ";

                context.AddObject("EntitySet", new MyType() { 
                    ID = 1,
                    Pròjè_x00A2_tÎð瑞갂థ్క_x0020_Iiلإَّ = value,
                });
                context.SaveChanges();
                var result = context.Execute<MyType>(new Uri("EntitySet?$orderby=Pròjè_x00A2_tÎð瑞갂థ్క_x0020_Iiلإَّ", UriKind.Relative)).First();
                Assert.AreEqual(value, result.Pròjè_x00A2_tÎð瑞갂థ్క_x0020_Iiلإَّ, "The roundtrip value not as expected");
            }
        }

        public class MyType
        {
            public int ID { get; set; }
            public string Pròjè_x00A2_tÎð瑞갂థ్క_x0020_Iiلإَّ { get; set;}
        }

        internal static void VerifyInvokeEnumerateSegments(string path, params string[] values)
        {
            TestUtil.AssertAreIEnumerablesEqual(values, InvokeEnumerateSegments(path), path);
        }

        internal static IEnumerable<string> InvokeEnumerateSegments(string path)
        {
            Type type = GetRequestUriProcessorType();
            MethodInfo method = type.GetMethod("EnumerateSegments", BindingFlags.Static | BindingFlags.NonPublic);
            try
            {
                Uri baseUri = new Uri("http://host/", UriKind.Absolute);
                Uri pathAsUri = new Uri("http://host/" + path, UriKind.RelativeOrAbsolute);
                return (IEnumerable<string>)method.Invoke(null, new object[] { pathAsUri, baseUri });
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
        }

        internal static void VerifyResourceNotFoundError(TestWebRequest request)
        {
            try
            {
                request.SendRequest();
                Assert.Fail("Resource Not Found error expected for " + request.RequestUriString + ", but none thrown.");
            }
            catch (WebException exception)
            {
                DataServiceException serviceException = exception.InnerException as DataServiceException;
                if (serviceException == null)
                {
                    throw;
                }

                Assert.AreEqual(404, serviceException.StatusCode, "General URI errors give 404 - Resource Not Found results (for [" + request.RequestUriString + "])");
            }
        }

        internal static void VerifyRequestSyntaxError(TestWebRequest request, string requestUriString)
        {
            request.RequestUriString = requestUriString;
            VerifyRequestSyntaxError(request);
        }

        internal static void VerifyRequestSyntaxError(TestWebRequest request)
        {
            try
            {
                request.SendRequest();
                Assert.Fail("Syntax error expected for " + request.RequestUriString + ", but none thrown.");
            }
            catch (WebException exception)
            {
                DataServiceException serviceException = (DataServiceException)exception.InnerException;
                Assert.AreEqual(400, serviceException.StatusCode, "Syntax errors give 400 - Bad Status results (for [" + request.RequestUriString + "])");
            }
        }

        internal static bool InvokeExtractSegmentIdentifier(string path, out string identifier)
        {
            Type type = GetRequestUriProcessorType();
            MethodInfo method = type.GetMethod("ExtractSegmentIdentifier", BindingFlags.Static | BindingFlags.NonPublic);
            try
            {
                object[] parameters = new object[] { path, null };
                bool result = (bool)method.Invoke(null, parameters);
                identifier = (string)parameters[1];
                return result;
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
        }

        internal static Type GetRequestUriProcessorType()
        {
            return typeof(IDataServiceHost).Assembly.GetType("Microsoft.OData.Service.RequestUriProcessor");
        }
    }
}
