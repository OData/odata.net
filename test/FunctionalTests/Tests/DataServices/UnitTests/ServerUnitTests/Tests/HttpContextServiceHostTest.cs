//---------------------------------------------------------------------
// <copyright file="HttpContextServiceHostTest.cs" company="Microsoft">
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
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces

    /// <summary>
    /// This is a test class for HttpContextServiceHost and is intended
    /// to contain all HttpContextServiceHost Unit Tests.
    /// </summary>
    [TestClass()]
    public class HttpContextServiceHostTest
    {
        private static Type GetContentTypeUtilType()
        {
            return typeof(IDataServiceHost).Assembly.GetType(
                "Microsoft.OData.Service.ContentTypeUtil", true);
        }

        private static Type GetCommonUtilityType()
        {
            return typeof(IDataServiceHost).Assembly.GetType(
                "Microsoft.OData.Service.CommonUtil", true);
        }

        private static Type GetWebUtilType()
        {
            return typeof(IDataServiceHost).Assembly.GetType(
                "Microsoft.OData.Service.WebUtil", true);
        }

        private static IEnumerable InvokeAcceptCharsetParts(string headerValue)
        {
            Type utilityType = GetContentTypeUtilType();
            try
            {
                MethodInfo method = utilityType.GetMethod("AcceptCharsetParts", BindingFlags.NonPublic | BindingFlags.Static);
                return (IEnumerable)method.Invoke(null, new object[] { headerValue });
            }
            catch (System.Reflection.TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
        }

        private static bool InvokeTryReadVersion(string text, out KeyValuePair<Version, string> result)
        {
            Type utilityType = GetCommonUtilityType();
            result = default(KeyValuePair<Version, string>);
            try
            {
                MethodInfo method = utilityType.GetMethod("TryReadVersion", BindingFlags.NonPublic | BindingFlags.Static);
                object[] arguments = new object[] { text, null };
                bool returnValue = (bool)method.Invoke(null, arguments);
                result = (KeyValuePair<Version, string>)arguments[1];
                return returnValue;
            }
            catch (System.Reflection.TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
        }

        private static void EnumerateAcceptCharsetParts(string headerValue)
        {
            IEnumerable enumerable = InvokeAcceptCharsetParts(headerValue);
            foreach (object o in enumerable)
            {
            }
        }

        private static void ExpectWebDataServiceException(int statusCode, System.Threading.ThreadStart callback)
        {
            try
            {
                callback();
                Assert.Fail("Callback didn't throw a DataServiceException.");
            }
            catch (DataServiceException exception)
            {
                Assert.AreEqual(exception.StatusCode, statusCode);
                return;
            }
        }

        private static T GetField<T>(object o, string fieldName)
        {
            FieldInfo fieldInfo = o.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)fieldInfo.GetValue(o);
        }

        private static void VerifyAcceptCharsetParts(string headerValue, string[] charsets, int[] qualityValues)
        {
            IEnumerable e = InvokeAcceptCharsetParts(headerValue);
            IEnumerator enumerator = e.GetEnumerator();
            for (int i = 0; i < charsets.Length; i++)
            {
                Assert.IsTrue(enumerator.MoveNext());
                string charset = GetField<string>(enumerator.Current, "Charset");
                int qualityValue = GetField<int>(enumerator.Current, "Quality");
                Assert.AreEqual(charsets[i], charset);
                Assert.AreEqual(qualityValues[i], qualityValue);
            }
            Assert.IsFalse(enumerator.MoveNext(), "No more results.");
        }

        [TestMethod]
        public void AcceptCharsetPartsBlankTest()
        {
            IEnumerable e = InvokeAcceptCharsetParts(" ");
            Assert.IsFalse(e.GetEnumerator().MoveNext());
        }

        [TestMethod]
        public void AcceptCharsetPartsBlankWithSeparatorsTest()
        {
            IEnumerable e = InvokeAcceptCharsetParts(" , , ");
            Assert.IsFalse(e.GetEnumerator().MoveNext());
        }

        [TestMethod]
        public void IsValidMimeTypeTest()
        {
            TestUtil.TraceScopeForException("IsValidMimeTypeTest", delegate()
            {
                VerifyIsValidMimeType("", false);
                VerifyIsValidMimeType("a", false);
                VerifyIsValidMimeType("/", false);
                VerifyIsValidMimeType("a/", false);
                VerifyIsValidMimeType("a/c", true);
                VerifyIsValidMimeType("a /c", false);
                VerifyIsValidMimeType("a/ c", false);
                VerifyIsValidMimeType("a/c ", false);
                VerifyIsValidMimeType("a/c+xml", true);
                VerifyIsValidMimeType("a/c+xml;foo", false);
            });
        }

        [TestMethod]
        public void SelectMimeTypeTest()
        {
            TestUtil.TraceScopeForException("SelectMimeTypeTest", delegate()
            {
                VerifySelectMimeType("", new string[] { "text/plain" }, "text/plain");
                VerifySelectMimeType("*/*", new string[] { "text/plain" }, "text/plain");
                VerifySelectMimeType("text/*", new string[] { "text/plain" }, "text/plain");
                VerifySelectMimeType("text/plain", new string[] { "text/plain" }, "text/plain");
                VerifySelectMimeType("text/*,text/html", new string[] { "text/plain" }, "text/plain");
                VerifySelectMimeType("text/*,text/html,text/plain", new string[] { "text/html", "text/plain" }, "text/html");
                VerifySelectMimeType("text/*,text/html,text/plain", new string[] { "text/plain", "text/html" }, "text/plain");
                VerifySelectMimeType("text/*,text/html,text/plain;q=0.5", new string[] { "text/html", "text/plain" }, "text/html");
                VerifySelectMimeType("text/*,text/html;q=0,text/plain;q=0.5", new string[] { "text/html", "text/plain" }, "text/plain");
                VerifySelectMimeType("text/plain;q=0", new string[] { "text/plain" }, null);
            });
        }

        [TestMethod]
        public void SelectRequiredMimeTypeTest()
        {
            TestUtil.TraceScopeForException("SelectRequiredMimeTypeTest", delegate()
            {
                VerifySelectRequiredMimeType(null, "text/plain", "text/plain", "text/plain");
                VerifySelectRequiredMimeType("", "text/plain", "text/plain", "text/plain");
                VerifySelectRequiredMimeType(" ", "text/plain", "text/plain", "text/plain");
                VerifySelectRequiredMimeType("text/*", "text/plain", "text/plain", "text/plain");
                VerifySelectRequiredMimeType("text/plain", "text/plain", "text/plain", "text/plain");
                VerifySelectRequiredMimeType("text/plain;q=0.1", "text/plain", "text/plain", "text/plain");
                VerifySelectRequiredMimeType("*/*;q=0,text/plain", "text/plain", "text/plain", "text/plain");
                VerifySelectRequiredMimeType("text/*", "text/plain", "text/html", "text/html");
                VerifySelectRequiredMimeType("*/*", "text/plain", "abc/pqr", "abc/pqr");
                VerifySelectRequiredMimeTypeFails("text/html", "text/plain", "text/plain");
                VerifySelectRequiredMimeTypeFails("text/plain;q=0", "text/plain", "text/plain");

                // Some samples from the HTTP RFC.
                VerifySelectRequiredMimeType("audio/*; q=0.2, audio/basic", "audio/basic", "audio/basic", "audio/basic");
                VerifySelectRequiredMimeType("audio/*; q=0.2, audio/basic", "audio/specific", "audio/basic", "audio/basic");
                VerifySelectRequiredMimeType("audio/*; q=0.2, audio/basic", "audio/specific", "audio/specific", "audio/specific");
                VerifySelectRequiredMimeType(
                    "text/plain; q=0.5, text/html, text/x-dvi; q=0.8, text/x-c",
                    "text/html", "text/html", "text/html");
                VerifySelectRequiredMimeType(
                    "text/plain; q=0.5, text/html, text/x-dvi; q=0.8, text/x-c",
                    "something/else", "text/x-dvi", "text/x-dvi");
                VerifySelectRequiredMimeType(
                    "text/*, text/html,text/html;level=1, */*",
                    "text/html", "text/blah", "text/html");
                VerifySelectRequiredMimeType(
                    "text/*, text/html,text/html;level=1, */*",
                    "text/other", "text/blah", "text/blah");
                VerifySelectRequiredMimeType(
                    "text/*, text/html,text/html;level=1, */*",
                    "something/else", "a/b", "a/b");
                // text/html;level=1
                // text/html;q=0.7
                // text/html;level=2;q=0.4
                // text/*;q=0.3
                // */*;q=0.5
                VerifySelectRequiredMimeType(
                    "text/*;q=0.3, text/html;q=0.7, text/html;level=1,text/html;level=2;q=0.4, */*;q=0.5",
                    "text/html", "a/b", "text/html");
                VerifySelectRequiredMimeType(
                    "text/*;q=0.3, text/html;q=0.7, text/html;level=1,text/html;level=2;q=0.4, */*;q=0.5",
                    "something/else", "a/b", "a/b");
                VerifySelectRequiredMimeType(
                    "text/*;q=0.3, text/html;q=0.7, text/html;level=1,text/html;level=2;q=0.4, */*;q=0.5",
                    "text/foo", "a/b", "a/b");

                VerifySelectRequiredMimeType("application/atomsvc+xml, application/json", new string[] { "application/atomsvc+xml", "application/json" }, "application/xml", "application/atomsvc+xml");
            });
        }

        [TestMethod]
        public void AcceptCharsetPartsSingleValueTest()
        {
            VerifyAcceptCharsetParts("iso-8859-5", new string[] { "iso-8859-5" }, new int[] { 1000 });
        }

        [TestMethod]
        public void AcceptCharsetPartsSingleQualityTest()
        {
            VerifyAcceptCharsetParts("iso-8859-5;q=0", new string[] { "iso-8859-5" }, new int[] { 0 });
            VerifyAcceptCharsetParts("iso-8859-5;q=0.1", new string[] { "iso-8859-5" }, new int[] { 100 });
            VerifyAcceptCharsetParts("iso-8859-5;q=0.01", new string[] { "iso-8859-5" }, new int[] { 10 });
            VerifyAcceptCharsetParts("iso-8859-5;q=0.001", new string[] { "iso-8859-5" }, new int[] { 1 });
        }

        [TestMethod]
        public void AcceptCharsetPartsOneQualityTest()
        {
            VerifyAcceptCharsetParts("iso-8859-5;q=1", new string[] { "iso-8859-5" }, new int[] { 1000 });
            VerifyAcceptCharsetParts("iso-8859-5;q=1.", new string[] { "iso-8859-5" }, new int[] { 1000 });
            VerifyAcceptCharsetParts("iso-8859-5;q=1.0", new string[] { "iso-8859-5" }, new int[] { 1000 });
            VerifyAcceptCharsetParts("iso-8859-5;q=1.00", new string[] { "iso-8859-5" }, new int[] { 1000 });
            VerifyAcceptCharsetParts("iso-8859-5;q=1.000", new string[] { "iso-8859-5" }, new int[] { 1000 });
        }

        [TestMethod]
        public void AcceptCharsetPartsInvalidQualityTest()
        {
            ExpectWebDataServiceException(400, delegate { EnumerateAcceptCharsetParts("iso-8859-5;q=1.0000"); });
            ExpectWebDataServiceException(400, delegate { EnumerateAcceptCharsetParts("iso-8859-5;q=A"); });
            ExpectWebDataServiceException(400, delegate { EnumerateAcceptCharsetParts("iso-8859-5;A"); });
            ExpectWebDataServiceException(400, delegate { EnumerateAcceptCharsetParts("iso-8859-5;q"); });
            ExpectWebDataServiceException(400, delegate { EnumerateAcceptCharsetParts("iso-8859-5;q="); });
            ExpectWebDataServiceException(400, delegate { EnumerateAcceptCharsetParts("iso-8859-5;q=A"); });
            ExpectWebDataServiceException(400, delegate { EnumerateAcceptCharsetParts("iso-8859-5;q=10"); });
            ExpectWebDataServiceException(400, delegate { EnumerateAcceptCharsetParts("iso-8859-5;q=2"); });
            ExpectWebDataServiceException(400, delegate { EnumerateAcceptCharsetParts("iso-8859-5;q=0.A"); });
        }

        [TestMethod]
        public void AcceptCharsetPartsMultipleTest()
        {
            // VerifyAcceptCharsetParts("iso-8859-5 iso-8859-6", new string[] { "iso-8859-5" }, new int[] { 1000 });
            VerifyAcceptCharsetParts(
                "iso-8859-5,windows-1252",
                new string[] { "iso-8859-5", "windows-1252" },
                new int[] { 1000, 1000 });
            VerifyAcceptCharsetParts(
                "iso-8859-5 ,windows-1252",
                new string[] { "iso-8859-5", "windows-1252" },
                new int[] { 1000, 1000 });
            VerifyAcceptCharsetParts(
                "iso-8859-5, windows-1252",
                new string[] { "iso-8859-5", "windows-1252" },
                new int[] { 1000, 1000 });
            VerifyAcceptCharsetParts(
                "iso-8859-5;q=0, windows-1252;q=1",
                new string[] { "iso-8859-5", "windows-1252" },
                new int[] { 0, 1000 });

            VerifyAcceptCharsetParts(
                "ISO-8859-1,utf-8;q=0.7,*;q=0.7",
                new string[] { "ISO-8859-1", "utf-8", "*" },
                new int[] { 1000, 700, 700 });
        }

        [TestMethod]
        public void AcceptCharsetPartsZeroQualityTest()
        {
            VerifyAcceptCharsetParts("iso-8859-5;q=0", new string[] { "iso-8859-5" }, new int[] { 0 });
            VerifyAcceptCharsetParts("iso-8859-5;q=0 ", new string[] { "iso-8859-5" }, new int[] { 0 });
            VerifyAcceptCharsetParts("iso-8859-5;q=0.", new string[] { "iso-8859-5" }, new int[] { 0 });
            VerifyAcceptCharsetParts("iso-8859-5;q=0. ", new string[] { "iso-8859-5" }, new int[] { 0 });
            VerifyAcceptCharsetParts("iso-8859-5;q=0.0", new string[] { "iso-8859-5" }, new int[] { 0 });
            VerifyAcceptCharsetParts("iso-8859-5;q=0.00", new string[] { "iso-8859-5" }, new int[] { 0 });
            VerifyAcceptCharsetParts("iso-8859-5;q=0.000", new string[] { "iso-8859-5" }, new int[] { 0 });
        }

        [TestMethod]
        public void AcceptCharsetPartsMissingSeparatorTest()
        {
            ExpectWebDataServiceException(400, delegate { EnumerateAcceptCharsetParts("iso-8859-5 iso-8859-6"); });
        }

        [TestMethod]
        public void AcceptCharsetPartsInvalidSeparatorTest()
        {
            ExpectWebDataServiceException(400, delegate { EnumerateAcceptCharsetParts("iso-8859-5\x5iso-8859-6"); });
        }

        [TestMethod]
        public void AcceptCharsetPartsInvalidTokenTest()
        {
            ExpectWebDataServiceException(400, delegate { EnumerateAcceptCharsetParts("\x5iso-8859-6"); });
            ExpectWebDataServiceException(400, delegate { EnumerateAcceptCharsetParts("iso-8859-5,\x5iso-8859-6"); });
            ExpectWebDataServiceException(400, delegate { EnumerateAcceptCharsetParts("iso-8859-5,\x5"); });
        }

        [TestMethod]
        public void HttpContextServiceHostQueryStringTest()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.RequestUriString = "/Customers?$orderby=";
                request.SendRequest();
            }
        }

        [TestMethod]
        public void HttpContextServiceHost()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(CustomDataContext);

                string[] invalidUris = new string[]
                {
                    "/Customers?$top=1&$top=2",
                    "/Customers?$top=1&$top=",
                    // "/Customers?$top=1&$top", - System.UriTemplateHelpers.ParseQueryString drops the empty argument
                    "/Customers?$top",
                    "/Customers?$foo",
                    "/Customers?$top=1&%20$top=1",
                    "/Customers?$top=1&$top%20=1",
                };

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("uri", invalidUris));
                TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                {
                    request.RequestUriString = (string)values["uri"];
                    Exception exception = TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(exception, true);
                    TestUtil.AssertExceptionStatusCode(exception, 400, "400 error expected for invalid query options " + request.RequestUriString);
                });
            }
        }

        [TestMethod]
        public void TryReadVersionTest()
        {
            var tests = new List<TryReadVersionTestCase>()
            {
                // Success.
                new TryReadVersionTestCase("1.0;my user-agent", 1, 0, "my user-agent"),
                new TryReadVersionTestCase("1.0;1.0", 1, 0, "1.0"),
                new TryReadVersionTestCase(" 1.0 ; 1.0 ", 1, 0, "1.0"),
                new TryReadVersionTestCase(" 1.0 ", 1, 0, null),
                new TryReadVersionTestCase(" 1.0; ", 1, 0, ""),
                new TryReadVersionTestCase(" 100.5; ", 100, 5, ""),

                // Failure.
                new TryReadVersionTestCase(";my user-agent"),
                new TryReadVersionTestCase("1,000.1"),
                new TryReadVersionTestCase("one dot zero;my user-agent"),
                new TryReadVersionTestCase("1;my user-agent"),
                new TryReadVersionTestCase("1.;my user-agent"),
                new TryReadVersionTestCase("1.0.0;my user-agent"),
                new TryReadVersionTestCase("1.0.;my user-agent"),
                new TryReadVersionTestCase("1.0.;my user-agent"),
                new TryReadVersionTestCase("-1.0;"),
                new TryReadVersionTestCase("1." + UInt32.MaxValue.ToString()),
            };
            foreach (var test in tests)
            {
                KeyValuePair<Version, string> version;
                bool result = InvokeTryReadVersion(test.Text, out version);
                Assert.AreEqual(test.Result, result, "Result is as expected");
                if (result)
                {
                    Assert.AreEqual(test.Major, version.Key.Major);
                    Assert.AreEqual(test.Minor, version.Key.Minor);
                    Assert.AreEqual(test.Comment, version.Value);
                }
            }
        }

        class TryReadVersionTestCase
        {
            public TryReadVersionTestCase(string text)
            {
                this.Text = text;
                this.Result = false;
            }

            public TryReadVersionTestCase(string text, int major, int minor, string comment)
            {
                this.Text = text;
                this.Major = major;
                this.Minor = minor;
                this.Comment = comment;
                this.Result = true;
            }

            public string Text { get; set; }
            public int Major { get; set; }
            public int Minor { get; set; }
            public string Comment { get; set; }
            public bool Result { get; set; }
        }

        private static Type SystemNetLogging
        {
            [DebuggerStepThrough]
            get
            {
                Assembly assembly = typeof(System.Net.WebRequest).Assembly;
                return assembly.GetType("System.Net.Logging", true);
            }
        }

        [TestMethod]
        public void HttpContextServiceHostTunnelingTest()
        {
            // Remove Method "POST" due to local IIS server instance racing on build machine.
            // Remove XMethod "PATCH" due to local IIS server instance racing on build machine.
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("Method", new string[] { "GET", "PUT", "DELETE" }),
                new Dimension("XMethod", new string[] { "GET", "POST", "DELETE", "DELETE,DELETE" }));

            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
            {
                using (TestWebRequest request = TestWebRequest.CreateForLocal())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/Orders(100)";

                    string method = (string)values["Method"];
                    string xmethod = (string)values["XMethod"];
                    request.HttpMethod = method;
                    if (xmethod == "PATCH")
                    {
                        request.RequestContentType = UnitTestsUtil.JsonLightMimeType;
                        request.SetRequestStreamAsText(
                            "{ @odata.type:\"#AstoriaUnitTests.Stubs.Order\"" +
                            " , DollarAmount: 10 }");
                    }

                    request.RequestHeaders["X-HTTP-Method"] = xmethod;
                    Exception exception = TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(exception,
                        method != "POST",
                        xmethod != "PATCH");
                }
            });
        }

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
            [Ignore] // Remove Atom
        // [TestMethod]
        public void HttpContextServiceHostRequestNameTest()
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("WebServerLocation", new WebServerLocation[] { WebServerLocation.InProcessWcf }),
                new Dimension("LocalHostName", new string[] { "127.0.0.1" }));
            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
            {
                WebServerLocation location = (WebServerLocation)values["WebServerLocation"];
                string hostName = (string)values["LocalHostName"];
                using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/Customers(1)?$format=atom";
                    request.StartService();

                    UriBuilder builder = new UriBuilder(request.FullRequestUriString);
                    builder.Host = hostName;
                    WebClient client = new WebClient();
                    string response = client.DownloadString(builder.Uri);

                    response = response.Substring(response.IndexOf('<'));
                    XmlDocument document = new XmlDocument(TestUtil.TestNameTable);
                    document.LoadXml(response);
                    string baseUri = UnitTestsUtil.GetBaseUri(document.DocumentElement);
                    TestUtil.AssertContains(baseUri, hostName);
                }
            });
        }

        [TestMethod]
        public void HttpProcessUtilityJsonEncodingTest()
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("Test", TestUtil.CreateDictionary<string, Encoding>(
                    new string[] { UnitTestsUtil.JsonLightMimeType, UnitTestsUtil.JsonLightMimeType + ";charset=utf-16", UnitTestsUtil.JsonLightMimeType + ";charset=GB18030" },
                    new Encoding[] { Encoding.UTF8, Encoding.Unicode, Encoding.GetEncoding("GB18030") })));
            using (CustomDataContext.CreateChangeScope())
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.Accept = UnitTestsUtil.JsonLightMimeType;
                request.HttpMethod = "POST";

                int index = 100;
                TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                {
                    KeyValuePair<string, Encoding> test = (KeyValuePair<string, Encoding>)values["Test"];
                    Encoding encoding = test.Value;

                    request.RequestContentType = test.Key;
                    request.RequestStream = new MemoryStream();

                    byte[] bytes = encoding.GetBytes("{ @odata.type: 'AstoriaUnitTests.Stubs.Customer', ID : " + index++.ToString() + ", Name : \"P\\\\B \\/K\\\"n\\u00e4c\\f\\tke\\r\\n\\br\\u00f6d AB\" }");
                    request.RequestStream.Write(bytes, 0, bytes.Length);
                    request.RequestStream.Position = 0;

                    request.RequestUriString = "/Customers";
                    request.SendRequest();
                    string responseText = request.GetResponseStreamAsText();
                    Assert.IsTrue(responseText.Contains(@"P\\B /K\" + "\"" + @"n\u00e4c\f\tke\r\n\br\u00f6d AB"), "Response [" + responseText + "] contains the expected escape sequences.");
                });
            }
        }

        [TestMethod]
        public void InvalidEncodingForTextXml()
        {
            Type type = GetContentTypeUtilType();
            try
            {
                MethodInfo method = type.GetMethod("ReadMediaType", BindingFlags.NonPublic | BindingFlags.Static);
                object o = method.Invoke(null, new object[] { "text/xml" });
                Type mediaTypeType = o.GetType();
                Encoding encodingValue = (Encoding)mediaTypeType.InvokeMember("SelectEncoding", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, o, null);
                Assert.IsTrue(encodingValue == null, "encoding must be null for xml, since we expect to read from the encoding from xml header");
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
        }

        [TestMethod]
        public void EncodingFromAcceptCharsetTest()
        {
            // It takes over a minute to run all combinations.
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("EncodingData", EncodingData.Values),
                new Dimension("StringData", StringData.Values),
                new Dimension("SerializationFormatData", SerializationFormatData.StructuredValues),
                new Dimension("WebServerLocation", new object[] { WebServerLocation.InProcess }));
            engine.Mode = CombinatorialEngineMode.EveryElement;

            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable table)
            {
                EncodingData encodingData = (EncodingData)table["EncodingData"];
                StringData stringData = (StringData)table["StringData"];
                WebServerLocation location = (WebServerLocation)table["WebServerLocation"];
                SerializationFormatData format = (SerializationFormatData)table["SerializationFormatData"];

                if (encodingData.Encoding == null)
                {
                    return;
                }

                if (!EncodingHandlesString(encodingData.Encoding, "<>#&;\r\n"))
                {
                    return;
                }

                // Transliteration of ISCII characters and Unicode is possible, but round-tripping from
                // Unicode will not work because all phonetic sounds share an ISCII value, but have
                // distinct Unicode points depending on the language.
                if (encodingData.Name.StartsWith("x-iscii") && stringData.TextScript != null && stringData.TextScript.SupportsIscii)
                {
                    return;
                }

                using (CustomDataContext.CreateChangeScope())
                using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.HttpMethod = "POST";
                    request.RequestUriString = "/Customers";
                    request.RequestContentType = UnitTestsUtil.JsonLightMimeType;
                    request.SetRequestStreamAsText("{ " +
                        "@odata.type : \"AstoriaUnitTests.Stubs.Customer\" ," +
                        "ID: 100," +
                        "Name: " +
                        System.Data.Test.Astoria.Util.JsonPrimitiveTypesUtil.PrimitiveToString(stringData.Value, typeof(string)) +
                        " }");
                    request.SendRequest();

                    request.HttpMethod = "GET";
                    request.AcceptCharset = encodingData.Name;
                    request.Accept = format.MimeTypes[0];
                    request.RequestUriString = "/Customers(100)";

                    bool encoderCanHandleData = EncodingHandlesString(encodingData.Encoding, stringData.Value);
                    Trace.WriteLine("Encoding handles string: " + encoderCanHandleData);

                    Exception exception = TestUtil.RunCatching(request.SendRequest);

                    XmlDocument document = null;
                    Stream byteStream = new MemoryStream();
                    if (exception == null)
                    {
                        using (Stream stream = request.GetResponseStream())
                        {
                            IOUtil.CopyStream(stream, byteStream);
                        }

                        byteStream.Position = 0;
                        Trace.WriteLine(TestUtil.BuildHexDump(byteStream));
                        byteStream.Position = 0;

                        if (format == SerializationFormatData.Atom)
                        {
                            document = new XmlDocument(TestUtil.TestNameTable);
                            using (StreamReader reader = new StreamReader(byteStream, encodingData.Encoding))
                            {
                                document.Load(reader);
                            }

                            TestUtil.TraceXml(document);

                            XmlElement nameElement = TestUtil.AssertSelectSingleElement(document, "//ads:Name");
                            if (stringData.Value == null)
                            {
                                Assert.IsTrue(UnitTestsUtil.HasElementNullValue(nameElement,
                                    new System.Net.Mime.ContentType(request.ResponseContentType).MediaType));
                            }
                            else
                            {
                                Assert.AreEqual(stringData.Value, nameElement.InnerText);
                            }
                        }
                    }
                    else
                    {
                        TestUtil.AssertExceptionExpected(exception, !encoderCanHandleData);
                    }
                }
            });
        }

        private static bool EncodingHandlesString(Encoding encoding, string data)
        {
            Debug.Assert(encoding != null, "encoding != null");

            if (String.IsNullOrEmpty(data))
            {
                return true;
            }

            Encoder encoder = encoding.GetEncoder();
            try
            {
                encoder.Fallback = new EncoderExceptionFallback();
                char[] chars = data.ToCharArray();
                byte[] bytes = new byte[encoder.GetByteCount(chars, 0, chars.Length, true)];
            }
            catch (EncoderFallbackException)
            {
                return false;
            }

            return true;
        }

        private static string InvokeSelectRequiredMimeType(string text, string[] requiredContentType, string inexactContentType)
        {
            Type utilityType = GetContentTypeUtilType();
            try
            {
                MethodInfo method = utilityType.GetMethod("SelectRequiredMimeType", BindingFlags.NonPublic | BindingFlags.Static);
                string result = (string)method.Invoke(null, new object[] { text, requiredContentType, inexactContentType });
                return result;
            }
            catch (System.Reflection.TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
        }

        private static void VerifyIsValidMimeType(string mimeType, bool expectedResult)
        {
            Trace.WriteLine("Verifying IsValidMimeType for [" + mimeType + "]");
            Type utilityType = GetWebUtilType();
            try
            {
                MethodInfo method = utilityType.GetMethod("IsValidMimeType", BindingFlags.NonPublic | BindingFlags.Static);
                bool result = (bool)method.Invoke(null, new object[] { mimeType });
                Assert.AreEqual(expectedResult, result);
            }
            catch (System.Reflection.TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
        }

        /// <summary>Checks whether the specified text picks the correct acceptable type.</summary>
        /// <param name="text">Header text as sent by client.</param>
        /// <param name="acceptableTypes">Ordered list of acceptable values for server.</param>
        /// <param name="expectedValue">Expected result value.</param>
        private static void VerifySelectMimeType(string text, string[] acceptableTypes, string expectedValue)
        {
            Trace.WriteLine("Verifying SelectMimeType for [" + text + "]");
            Type utilityType = GetContentTypeUtilType();
            try
            {
                MethodInfo method = utilityType.GetMethod("SelectMimeType", BindingFlags.NonPublic | BindingFlags.Static);
                string result = (string)method.Invoke(null, new object[] { text, acceptableTypes });
                Assert.AreEqual(
                    expectedValue,
                    result,
                    "Verifying SelectMimeType for [" + text + " -- " + string.Join(",", acceptableTypes) + "]");
            }
            catch (System.Reflection.TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
        }

        /// <summary>Checks whether the specified text picks the correct acceptable type.</summary>
        private static void VerifySelectRequiredMimeType(string text, string requiredContentType, string inexactContentType, string expectedValue)
        {
            VerifySelectRequiredMimeType(text, new string[] { requiredContentType }, inexactContentType, expectedValue);
        }

        /// <summary>Checks whether the specified text picks the correct acceptable type.</summary>
        private static void VerifySelectRequiredMimeType(string text, string[] requiredContentType, string inexactContentType, string expectedValue)
        {
            Trace.WriteLine("Verifying VerifySelectRequiredMimeType for [" + text + "]");
            Assert.AreEqual(
                expectedValue,
                InvokeSelectRequiredMimeType(text, requiredContentType, inexactContentType),
                String.Format("Verifying SelectRequiredMimeType for [{0},{1},{2}]", text, requiredContentType, inexactContentType));
        }

        /// <summary>Checks that a selecting a required MIME type fails.</summary>
        private static void VerifySelectRequiredMimeTypeFails(string text, string requiredContentType, string inexactContentType)
        {
            Trace.WriteLine("Verifying VerifySelectRequiredMimeTypeFails for [" + text + "]");
            Exception exception = TestUtil.RunCatching(delegate()
            {
                InvokeSelectRequiredMimeType(text, new string[] { requiredContentType }, inexactContentType);
            });
            TestUtil.AssertExceptionExpected(exception, true);
        }

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
            [Ignore] // Remove Atom
        // [TestMethod]
        public void IncomingMessagePropertiesTest()
        {
            var testCases = new[]
            {
                new
                {
                    ExpectFailure = false,
                    ErrorCode = 0,
                    IncomingMessageProperties = new Dictionary<string, object>()
                    {
                        {"MicrosoftDataServicesRequestUri", new Uri("http://mappedhost/mappedService.svc/Customers", UriKind.Absolute) },
                        {"MicrosoftDataServicesRootUri", new Uri("http://mappedhost/mappedService.svc/", UriKind.Absolute) }
                    }
                },
                new
                {
                    ExpectFailure = true,
                    ErrorCode = 500,
                    IncomingMessageProperties = new Dictionary<string, object>()
                    {
                        {"MicrosoftDataServicesRequestUri", "http://mappedhost/mappedService.svc/Customers"},
                    }
                },
                new
                {
                    ExpectFailure = true,
                    ErrorCode = 500,
                    IncomingMessageProperties = new Dictionary<string, object>()
                    {
                        {"MicrosoftDataServicesRootUri", "http://mappedhost/mappedService.svc/"}
                    }
                },
                new
                {
                    ExpectFailure = true,
                    ErrorCode = 400,
                    IncomingMessageProperties = new Dictionary<string, object>()
                    {
                        {"MicrosoftDataServicesRequestUri", new Uri("http://mappedhost/mappedService.svc/Customers", UriKind.Absolute) },
                    }
                },
                new
                {
                    ExpectFailure = true,
                    ErrorCode = 400,
                    IncomingMessageProperties = new Dictionary<string, object>()
                    {
                        {"MicrosoftDataServicesRootUri", new Uri("http://mappedhost/mappedService.svc/", UriKind.Absolute) }
                    }
                }
            };

            foreach (var testCase in testCases)
            {
                using (CustomDataContext.CreateChangeScope())
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    OpenWebDataServiceHelper.ForceVerboseErrors = true;

                    request.DataServiceType = typeof(CustomDataContext);
                    request.HttpMethod = "GET";
                    request.RequestUriString = "/Customers?$format=atom";

                    ((InProcessWcfWebRequest)request).IncomingMessageProperties = testCase.IncomingMessageProperties;

                    Exception e = TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(e, testCase.ExpectFailure);
                    if (e == null)
                    {
                        XDocument response = request.GetResponseStreamAsXDocument();
                        var idElements = response.Descendants(UnitTestsUtil.AtomNamespace + "feed").Descendants(UnitTestsUtil.AtomNamespace + "id");
                        foreach (var id in idElements)
                        {
                            TestUtil.AssertContains(id.Value, ((Uri)((InProcessWcfWebRequest)request).IncomingMessageProperties["MicrosoftDataServicesRequestUri"]).AbsoluteUri);
                        }
                    }
                    else
                    {
                        TestUtil.AssertExceptionStatusCode(e, testCase.ErrorCode, null);
                    }
                }
            }
        }
    }
}
