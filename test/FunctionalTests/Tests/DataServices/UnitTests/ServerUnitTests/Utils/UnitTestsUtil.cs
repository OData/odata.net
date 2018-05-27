//---------------------------------------------------------------------
// <copyright file="UnitTestsUtil.cs" company="Microsoft">
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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.ServiceModel;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ocs = AstoriaUnitTests.ObjectContextStubs;
    using providers = Microsoft.OData.Service.Providers;

    // TODO: remove verbose json after removing it from the product
    public static class UnitTestsUtil
    {
        public static readonly XNamespace AtomNamespace = "http://www.w3.org/2005/Atom";
        public static readonly XNamespace DataNamespace = "http://docs.oasis-open.org/odata/ns/data";
        public static readonly XNamespace MetadataNamespace = "http://docs.oasis-open.org/odata/ns/metadata";

        public static readonly XNamespace EdmOasisNamespace = "http://docs.oasis-open.org/odata/ns/edm";
        public static readonly XNamespace MslV1Namespace = "urn:schemas-microsoft-com:windows:storage:mapping:CS";
        public static readonly XNamespace MslV2Namespace = "http://schemas.microsoft.com/ado/2008/09/mapping/cs";
        public static readonly XNamespace SsdlV1Namespace = "http://schemas.microsoft.com/ado/2006/04/edm/ssdl";
        public static readonly XNamespace SsdlV2Namespace = "http://schemas.microsoft.com/ado/2009/02/edm/ssdl";
        public static readonly XNamespace EdmxNamespace = "http://docs.oasis-open.org/odata/ns/edmx";
        public static readonly XNamespace TempUriNamespace = "http://tempuri.org";
        public static readonly XNamespace CurrencyNamespace = "http://www.currency.org";
        public static readonly XNamespace JsonInXmlPayloadNamespace = "http://astoriaunittests.tests/jsoninxml";
        public static readonly XNamespace NavigationLinkNamespace = "http://docs.oasis-open.org/odata/ns/related/";
        public static readonly XNamespace RelationshipLinkNamespace = "http://docs.oasis-open.org/odata/ns/relatedlinks/";
        public static readonly XNamespace SchemeNamespace = "http://docs.oasis-open.org/odata/ns/scheme";
        public static readonly XNamespace GmlNamespace = "http://www.opengis.net/gml";

        public static readonly string JsonLightMimeType = "applicatIon/jsOn;oDaTa.MeTaData=MinIMal";
        public static readonly string JsonLightMimeTypeFullMetadata = "applicatIon/jsOn;oDaTa.MeTaDaTa=FuLl";
        public static readonly string JsonLightMimeTypeNoMetadata = "applicatIon/jsOn;oDaTa.MeTaDaTa=nOnE";
        public static readonly string JsonLightMimeTypeIeee754Compatible = "applicatIon/json;odata.metadata=minimal;IEEE754Compatible=true";
        public static readonly string AtomFormat = "applicaTion/atom+xMl";
        public static readonly string MimeAny = "*/*";
        public static readonly string JsonMimeType = "application/JSON";
        public static readonly string MimeApplicationOctetStream = "applIcation/oCtet-stream";
        public static readonly string MimeApplicationXml = "applicAtion/Xml";
        public static readonly string MimeTextPlain = "tExt/plaIn";
        public static readonly string MimeMultipartMixed = "multIpart/mixEd";
        public static readonly string MimeTextXml = "texT/xMl"; // deprecated
        public static string[] ResponseFormats = new string[] { JsonLightMimeType };
        public static bool[] BooleanValues = new bool[] { false, true };

        public static string CustomerTypeName = "AstoriaUnitTests.ObjectContextStubs.Types.Customer";
        public static string CustomerWithBirthdayTypeName = "AstoriaUnitTests.ObjectContextStubs.Types.CustomerWithBirthday";
        public static string OrderTypeName = "AstoriaUnitTests.ObjectContextStubs.Types.Order";
        public static string AddressTypeName = "AstoriaUnitTests.ObjectContextStubs.Types.Address";

        public static Type[] ProviderTypes = new Type[] {
            typeof(CustomDataContext),
            typeof(EFFK.CustomObjectContextPOCOProxy),
            typeof(EFFK.CustomObjectContextPOCO),
            typeof(CustomRowBasedContext),
            typeof(CustomRowBasedOpenTypesContext)
        };

        private static XslCompiledTransform Json2AtomXslt = null;
        private static XslCompiledTransform Atom2JsonXslt = null;

        /// <summary>XmlNamespaceManager used for testing.</summary>
        private static XmlNameTable TestNameTable
        {
            [DebuggerStepThrough]
            get
            {
                return TestUtil.TestNameTable;
            }
        }

        /// <summary>
        /// Namespace manager for testing XPath expressions. The following prefixes
        /// are mapped to namespaces.
        /// </summary>
        private static XmlNamespaceManager TestNamespaceManager
        {
            get
            {
                return TestUtil.TestNamespaceManager;
            }
        }

        /// <summary>
        /// Makes the call to the server to fetch the metadata from the given type and writes
        /// to the given file location.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="schemaLocation"></param>
        /// <returns></returns>
        public static IEdmModel LoadMetadataFromDataServiceType(Type type, string schemaLocation)
        {
            using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcess))
            {
                request.DataServiceType = type;
                request.RequestUriString = "/$metadata";
                request.SendRequest();
                using (Stream resultStream = request.GetResponseStream())
                {
                    return MetadataUtil.IsValidMetadata(resultStream, schemaLocation);
                }
            }
        }

        public static void DoInserts(string payload, string uri, string[] xPaths, Type contextType, string responseFormat)
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = contextType;
                request.RequestUriString = uri;
                request.Accept = responseFormat;
                request.HttpMethod = "POST";
                request.RequestStream = IOUtil.CreateStream(payload);
                request.RequestContentType = responseFormat;
                request.SendRequest();
                if (xPaths != null)
                {
                    using (Stream resultStream = request.GetResponseStream())
                    {
                        VerifyXPaths(resultStream, responseFormat, xPaths);
                    }
                }
            }
        }

        public static XmlDocument VerifyXPaths(Stream resultStream, string responseFormat, params string[] xPaths)
        {
            XmlDocument document = null;
            string traceDocument = null;
            if (String.Equals(responseFormat, JsonLightMimeType, StringComparison.OrdinalIgnoreCase)  ||
                String.Equals(responseFormat, JsonLightMimeTypeFullMetadata, StringComparison.OrdinalIgnoreCase))
            {
                Stream stream = TestUtil.EnsureStreamWithSeek(resultStream);
                string originalText = new StreamReader(stream).ReadToEnd();
                stream.Position = 0;
                bool succeeded = false;
                try
                {
                    document = JsonValidator.ConvertToXmlDocument(stream);
                    succeeded = true; // Forego verification of response as that is beyond scope of this test case
                }
                finally
                {
                    if (!succeeded)
                    {
                        Trace.WriteLine("Original JSON payload:");
                        Trace.WriteLine(originalText);
                    }
                }
            }
            else if (String.Equals(responseFormat, AtomFormat, StringComparison.OrdinalIgnoreCase))
            {
                document = new XmlDocument(TestNameTable);
                document.Load(resultStream);

                //Trace.WriteLine(document.OuterXml);

                if (resultStream.CanSeek)
                {
                    resultStream.Position = 0;
                }

                VerifyXPaths(document, xPaths);
            }
            else if (String.Equals(responseFormat, MimeApplicationXml, StringComparison.OrdinalIgnoreCase))
            {
                resultStream = TestUtil.TraceStream(resultStream);
                traceDocument = new StreamReader(resultStream).ReadToEnd();
                document = new XmlDocument(TestNameTable);
                document.Load(new StringReader(traceDocument));
                VerifyXPaths(document, xPaths);
            }
            else
            {
                Assert.Fail("Unexpected Response format [" + responseFormat + "]");
            }

            return document;
        }

        /// <summary>
        /// Verifies that the specified XPath is present in the given document.
        /// </summary>
        /// <param name="node">Node to look in.</param>
        /// <param name="xpaths">
        /// XPath expressions. The following prefixes are mapped to namespaces:
        ///   cdc   - CustomDataContext
        ///   tcdc  - TypedCustomDataContext
        ///   nc    - NorthwindContext
        ///   web3s - Web3S:
        /// </param>
        public static void VerifyXPaths(XNode node, params string[] xpaths)
        {
            VerifyXPaths(node.CreateNavigator(TestNameTable), xpaths);
        }

        /// <summary>
        /// Verifies that the specified XPath is present in the given document.
        /// </summary>
        /// <param name="navigable">Document to look in.</param>
        /// <param name="xpaths">
        /// XPath expressions. The following prefixes are mapped to namespaces:
        ///   cdc   - CustomDataContext
        ///   tcdc  - TypedCustomDataContext
        ///   nc    - NorthwindContext
        ///   web3s - Web3S:
        /// </param>
        public static void VerifyXPaths(IXPathNavigable navigable, params string[] xpaths)
        {
            Assert.IsNotNull(navigable);
            Assert.IsNotNull(xpaths);
            foreach (string xpath in xpaths)
            {
                bool isTrue;
                if (xpath.StartsWith("count") || xpath.StartsWith("boolean"))
                {
                    XPathNavigator navigator = navigable.CreateNavigator();
                    isTrue = (bool)navigator.Evaluate(xpath, TestNamespaceManager);
                }
                else
                {
                    XPathNodeIterator iterator = navigable.CreateNavigator().Select(xpath, TestNamespaceManager);
                    isTrue = iterator.Count > 0;
                }

                if (!isTrue)
                {
                    Trace.WriteLine(navigable.CreateNavigator().OuterXml);
                    Assert.Fail("The expression " + xpath + " did not find elements. The document has just been traced.");
                }
            }
        }

        public static XmlDocument VerifyXPaths(Stream resultStream, string responseFormat, string[] web3sXPaths, string[] jsonXPaths, string[] atomXPaths)
        {
            responseFormat = TestUtil.GetMediaType(responseFormat);
            if (String.Equals(responseFormat, AtomFormat, StringComparison.OrdinalIgnoreCase) ||
                String.Equals(responseFormat, MimeApplicationXml, StringComparison.OrdinalIgnoreCase))
            {
                return VerifyXPaths(resultStream, responseFormat, atomXPaths);
            }
            else if (String.Equals(responseFormat, JsonLightMimeType, StringComparison.OrdinalIgnoreCase) ||
                String.Equals(responseFormat, JsonLightMimeTypeFullMetadata, StringComparison.OrdinalIgnoreCase))
            {
                return VerifyXPaths(resultStream, responseFormat, jsonXPaths);
            }
            else
            {
                Assert.Fail("invalid format: " + responseFormat);
                return null;
            }
        }

        /// <summary>
        /// Verifies that the specified XPath (or more) return at least one result.
        /// </summary>
        /// <param name="node">Node to look in.</param>
        /// <param name="xpaths">The xpaths to verify.</param>
        public static void VerifyXPathExists(XNode node, params string[] xpaths)
        {
            VerifyXPathExists(node.CreateNavigator(TestNameTable), xpaths);
        }

        /// <summary>
        /// Verifies that the specified XPath (or more) return at least one result.
        /// </summary>
        /// <param name="navigable">Document to look in.</param>
        /// <param name="xpaths">The xpaths to verify.</param>
        public static void VerifyXPathExists(IXPathNavigable navigable, params string[] xpaths)
        {
            if (navigable == null)
            {
                return;
            }

            foreach (string xpath in xpaths)
            {
                int count = navigable.CreateNavigator().Select(xpath, TestNamespaceManager).Count;
                if (count == 0)
                {
                    Trace.WriteLine(navigable.CreateNavigator().OuterXml);
                    Assert.Fail("Failed to find specified xpath in the document: " + xpath);
                }
            }
        }

        /// <summary>
        /// Verifies that the specified XPath (or more) is not present in the input.
        /// </summary>
        /// <param name="node">Node to look in.</param>
        /// <param name="xpaths">The xpaths to verify.</param>
        public static void VerifyXPathDoesntExist(XNode node, params string[] xpaths)
        {
            VerifyXPathDoesntExist(node.CreateNavigator(TestNameTable), xpaths);
        }

        /// <summary>
        /// Verifies that the specified XPath (or more) is not present in the input.
        /// </summary>
        /// <param name="navigable">Document to look in.</param>
        /// <param name="xpaths">The xpaths to verify.</param>
        public static void VerifyXPathDoesntExist(IXPathNavigable navigable, params string[] xpaths)
        {
            foreach (string xpath in xpaths)
            {
                int count = navigable.CreateNavigator().Select(xpath, TestNamespaceManager).Count;
                if (count != 0)
                {
                    Trace.WriteLine(navigable.CreateNavigator().OuterXml);
                    Assert.Fail("Found xpath which was not expected: " + xpath);
                }
            }
        }

        /// <summary>
        /// Verifies that the specified XPath (or more) returns the specified number of results.
        /// </summary>
        /// <param name="node">Node to look in.</param>
        /// <param name="count">The number of results each XPath should return.</param>
        /// <param name="xpaths">The xpaths to verify.</param>
        public static void VerifyXPathResultCount(XNode node, int count, params string[] xpaths)
        {
            VerifyXPathResultCount(node.CreateNavigator(TestNameTable), count, xpaths);
        }

        /// <summary>
        /// Verifies that the specified XPath (or more) returns the specified number of results.
        /// </summary>
        /// <param name="navigable">Document to look in.</param>
        /// <param name="count">The number of results each XPath should return.</param>
        /// <param name="xpaths">The xpaths to verify.</param>
        public static void VerifyXPathResultCount(IXPathNavigable navigable, int count, params string[] xpaths)
        {
            foreach (string xpath in xpaths)
            {
                int realCount = navigable.CreateNavigator().Select(xpath, TestNamespaceManager).Count;
                if (count != realCount)
                {
                    Trace.WriteLine(navigable.CreateNavigator().OuterXml);
                    Assert.AreEqual(count, realCount, "XPath didn't return expected number of results: " + xpath);
                }
            }
        }

        /// <summary>
        /// Verifies that the specified XPath (or more) evaluate to true.
        /// </summary>
        /// <param name="node">Node to look in.</param>
        /// <param name="xpaths">The xpaths to verify.</param>
        public static void VerifyXPathExpressionResults(XNode node, object expectedResult, params string[] xpaths)
        {
            VerifyXPathExpressionResults(node.CreateNavigator(TestNameTable), expectedResult, xpaths);
        }

        /// <summary>
        /// Verifies that the specified XPath (or more) evaluate to true.
        /// </summary>
        /// <param name="navigable">Document to look in.</param>
        /// <param name="xpaths">The xpaths to verify.</param>
        public static void VerifyXPathExpressionResults(IXPathNavigable navigable, object expectedResult, params string[] xpaths)
        {
            XPathNavigator nav = navigable.CreateNavigator();
            foreach (string xpath in xpaths)
            {
                object actualResult = nav.Evaluate(xpath, TestNamespaceManager);
                Assert.AreEqual(expectedResult, actualResult, "Expression: " + xpath + " evaluated to: " + actualResult.ToString() +  "\nXml:\n" + nav.OuterXml);
            }
        }

        /// <summary>
        /// Returns the value of the node returned by specified XPath
        /// </summary>
        /// <param name="node">The document to query.</param>
        /// <param name="xpath">The XPath to evaluate.</param>
        /// <returns>The value of the node returned by the XPath or null if no result was found.</returns>
        public static string GetXPathValue(XNode node, string xpath)
        {
            return GetXPathValue(node.CreateNavigator(TestNameTable), xpath);
        }

        /// <summary>
        /// Returns the value of the node returned by specified XPath
        /// </summary>
        /// <param name="navigable">The document to query.</param>
        /// <param name="xpath">The XPath to evaluate.</param>
        /// <returns>The value of the node returned by the XPath or null if no result was found.</returns>
        public static string GetXPathValue(IXPathNavigable navigable, string xpath)
        {
            var node = navigable.CreateNavigator().SelectSingleNode(xpath, TestNamespaceManager);
            return node == null ? null : node.Value;
        }

        /// <summary>Sends a request and returns the response as ATOM XML.</summary>
        /// <param name="request">The request object to use.</param>
        /// <returns>The response as ATOM XML. If the response came as JSON the method will convert the JSON to ATOM.</returns>
        public static XDocument GetResponseAsAtomXLinq(TestWebRequest request)
        {
            string format = (!string.IsNullOrEmpty(request.Accept) && request.Accept == UnitTestsUtil.JsonLightMimeType) ? UnitTestsUtil.JsonLightMimeType : UnitTestsUtil.AtomFormat;

            if (format == UnitTestsUtil.JsonLightMimeType)
            {
                return null;
            }
            else
            {
                return request.GetResponseStreamAsXDocument();
            }
        }

        /// <summary>Sends a request and returns the response as ATOM XML.</summary>
        /// <param name="request">The request object to use.</param>
        /// <param name="uri">The uri to send the request to.</param>
        /// <returns>The response as ATOM XML. If the response came as JSON the method will convert the JSON to ATOM.</returns>
        public static XDocument GetResponseAsAtomXLinq(TestWebRequest request, string uri)
        {
            return GetResponseAsAtomXLinq(request, uri, request.Accept);
        }

        /// <summary>Sends a request and returns the response as ATOM XML.</summary>
        /// <param name="request">The request object to use.</param>
        /// <param name="uri">The request URI string (relative)</param>
        /// <param name="format">The format in which to request the response (ATOM or JSON)</param>
        /// <returns>The response as ATOM XML. If the response came as JSON the method will convert the JSON to ATOM.</returns>
        public static XDocument GetResponseAsAtomXLinq(TestWebRequest request, string uri, string format)
        {
            request.Accept = format;
            request.ForceVerboseErrors = true;
            request.RequestUriString = uri;
            request.SendRequest();
            return GetResponseAsAtomXLinq(request);
        }

        /// <summary>Sends a request and returns the response as ATOM XML.</summary>
        /// <param name="dataServiceType">The type of the service to run the request against.</param>
        /// <param name="uri">The request URI string (relative)</param>
        /// <param name="format">The format in which to request the response (ATOM or JSON)</param>
        /// <returns>The response as ATOM XML. If the response came as JSON the method will convert the JSON to ATOM.</returns>
        public static XDocument GetResponseAsAtomXLinq(Type dataServiceType, string uri, string format)
        {
            using (TestUtil.MetadataCacheCleaner())
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = dataServiceType;
                return GetResponseAsAtomXLinq(request, uri, format);
            }
        }

        /// <summary>Sends a request and returns the response as ATOM XML.</summary>
        /// <param name="request">The request object to use.</param>
        /// <returns>The response as ATOM XML. If the response came as JSON the method will convert the JSON to ATOM.</returns>
        public static XmlDocument GetResponseAsAtom(TestWebRequest request, JsonToAtomUtil jsonToAtomUtil = null)
        {
            string format = TestUtil.GetMediaType(request.ResponseContentType);

            XmlDocument xml = request.GetResponseStreamAsXmlDocument(format);
            if (TestUtil.CompareMimeType(format, UnitTestsUtil.JsonLightMimeType))
            {
                return UnitTestsUtil.Json2Atom(xml, new Uri(request.ServiceRoot, request.RequestUriString).AbsoluteUri, jsonToAtomUtil);
            }
            else
            {
                return xml;
            }
        }

        /// <summary>Sends a request and returns the response as ATOM XML.</summary>
        /// <param name="request">The request object to use.</param>
        /// <param name="uri">The uri to send the request to.</param>
        /// <returns>The response as ATOM XML. If the response came as JSON the method will convert the JSON to ATOM.</returns>
        public static XmlDocument GetResponseAsAtom(TestWebRequest request, string uri)
        {
            return GetResponseAsAtom(request, uri, request.Accept);
        }

        /// <summary>Sends a request and returns the response as ATOM XML.</summary>
        /// <param name="request">The request object to use.</param>
        /// <param name="uri">The request URI string (relative)</param>
        /// <param name="format">The format in which to request the response (ATOM or JSON)</param>
        /// <returns>The response as ATOM XML. If the response came as JSON the method will convert the JSON to ATOM.</returns>
        public static XmlDocument GetResponseAsAtom(TestWebRequest request, string uri, string format)
        {
            request.Accept = format;
            request.ForceVerboseErrors = true;
            request.RequestUriString = uri;
            request.SendRequest();
            return GetResponseAsAtom(request);
        }

        /// <summary>Sends a request and returns the response as ATOM XML.</summary>
        /// <param name="dataServiceType">The type of the service to run the request against.</param>
        /// <param name="uri">The request URI string (relative)</param>
        /// <param name="format">The format in which to request the response (ATOM or JSON)</param>
        /// <returns>The response as ATOM XML. If the response came as JSON the method will convert the JSON to ATOM.</returns>
        public static XmlDocument GetResponseAsAtom(Type dataServiceType, string uri, string format)
        {
            using (TestUtil.MetadataCacheCleaner())
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = dataServiceType;
                return GetResponseAsAtom(request, uri, format);
            }
        }

        public static XmlDocument Json2Atom(XmlDocument json, string requestUri, JsonToAtomUtil jsonToAtomUtil = null)
        {
            if (Json2AtomXslt == null)
            {
                Json2AtomXslt = new XslCompiledTransform(Debugger.IsAttached);
                using (Stream s = typeof(UnitTestsUtil).Assembly.GetManifestResourceStream("AstoriaUnitTests.Tests.json2atom.xslt"))
                {
                    Json2AtomXslt.Load(XmlReader.Create(s));
                }
            }

            StringBuilder sb = new StringBuilder();

            var arguments = new XsltArgumentList();
            arguments.AddParam("requesturi", String.Empty, requestUri);
            arguments.AddParam("usemetadatautil", String.Empty, jsonToAtomUtil != null);

            if (jsonToAtomUtil != null)
            {
                arguments.AddExtensionObject("www.metadatautils.com", jsonToAtomUtil);
            }

            Json2AtomXslt.Transform(json, arguments, new StringWriter(sb));
            XmlDocument atom = new XmlDocument();
            atom.LoadXml(sb.ToString());
            return atom;
        }

        public static XDocument Json2AtomXLinq(XDocument json, bool isUploadPayload = false)
        {
            if (Json2AtomXslt == null)
            {
                Json2AtomXslt = new XslCompiledTransform();
                using (Stream s = typeof(UnitTestsUtil).Assembly.GetManifestResourceStream("AstoriaUnitTests.Tests.json2atom.xslt"))
                {
                    Json2AtomXslt.Load(XmlReader.Create(s));
                }
            }

            var arguments = new XsltArgumentList();
            arguments.AddParam("usemetadatautil", String.Empty, false);
            // produce valid upload payload from Json Strings
            arguments.AddParam("isupload", String.Empty, isUploadPayload);

            XDocument atom = new XDocument();
            using (XmlWriter writer = atom.CreateWriter())
            {
                Json2AtomXslt.Transform(json.CreateNavigator(TestUtil.TestNameTable), arguments, writer);
            }

            return atom;
        }

        public static string Atom2JsonXLinq(XDocument atom)
        {
            if (Atom2JsonXslt == null)
            {
                Atom2JsonXslt = new XslCompiledTransform();
                using (Stream s = typeof(UnitTestsUtil).Assembly.GetManifestResourceStream("AstoriaUnitTests.Tests.atom2json.xslt"))
                {
                    Atom2JsonXslt.Load(XmlReader.Create(s));
                }
            }

            StringBuilder json = new StringBuilder();
            using (StringWriter writer = new StringWriter(json))
            {
                Atom2JsonXslt.Transform(atom.CreateNavigator(TestUtil.TestNameTable), null, writer);
            }

            return json.ToString();
        }

        /// <summary>Sends a request and verifies that the response is 204 - No Content</summary>
        /// <param name="request">The request object to use.</param>
        /// <param name="uri">The request URI (relative)</param>
        /// <param name="format">The format in which to request the response (ATOM or JSON)</param>
        public static void VerifyNoContentResponse(TestWebRequest request, string uri, string format)
        {
            request.Accept = format;
            request.ForceVerboseErrors = true;
            request.RequestUriString = uri;
            request.SendRequest();
            Assert.AreEqual(204, request.ResponseStatusCode, "This request should respond with 204 - No Content.");
            using (Stream s = request.GetResponseStream())
            {
                Assert.IsTrue(s.ReadByte() == -1, "The response to this request should be empty.");
            }
        }

        /// <summary>Sends a request and verifies that the response is 204 - No Content</summary>
        /// <param name="request">The type of the service to run the request against.</param>
        /// <param name="uri">The request URI (relative)</param>
        /// <param name="format">The format in which to request the response (ATOM or JSON)</param>
        public static void VerifyNoContentResponse(Type dataServiceType, string uri, string format)
        {
            using (TestUtil.MetadataCacheCleaner())
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = dataServiceType;
                VerifyNoContentResponse(request, uri, format);
            }
        }

        public static string GetETagFromResponse(Type contextType, string uri, string responseFormat)
        {
            // */* is more generic than application/atom+xml.
            string accept = responseFormat;
            if (accept == UnitTestsUtil.AtomFormat)
            {
                accept = UnitTestsUtil.MimeAny;
            }

            TestWebRequest request = GetTestWebRequestInstance(accept, uri, contextType, null, "GET");
            string etag = request.ResponseETag;
            request.Dispose();
            return etag;
        }

        /// <summary>Returns ETag value from primitive values list.</summary>
        /// <param name="values">Enumeration of primitive values to format as ETag.</param>
        /// <returns>The ETag value including the W and quotes and everything.</returns>
        public static string GetETagFromValues(IEnumerable<object> values)
        {
            return ConcurrencyUtil.ConstructETag(values.Select(value => ConcurrencyUtil.PrimitiveToETagString(value)).ToArray());
        }

        public static TestWebRequest GetTestWebRequestInstance(string responseFormat, string uri, Type contextType, KeyValuePair<string, string>[] headerValues, string httpMethodName)
        {
            return GetTestWebRequestInstance(responseFormat, uri, contextType, headerValues, httpMethodName, null /*payload*/);
        }

        public static TestWebRequest GetTestWebRequestInstance(string responseFormat, string uri, Type contextType, KeyValuePair<string, string>[] headerValues, string httpMethodName, string payload)
        {
            return GetTestWebRequestInstance(responseFormat, uri, contextType, headerValues, httpMethodName, payload, WebServerLocation.InProcess);
        }

        public static TestWebRequest GetTestWebRequestInstance(string responseFormat, string uri, Type contextType, KeyValuePair<string, string>[] headerValues, string httpMethodName, string payload, WebServerLocation location)
        {
            TestWebRequest request = TestWebRequest.CreateForLocation(location);
            request.DataServiceType = contextType;
            request.RequestUriString = uri;
            request.Accept = responseFormat;
            request.HttpMethod = httpMethodName;
            SetHeaderValues(request, headerValues);
            if (payload != null)
            {
                request.RequestStream = new MemoryStream();
                request.RequestContentType = responseFormat;
                StreamWriter writer = new StreamWriter(request.RequestStream);
                writer.Write(payload);
                writer.Flush();
                request.RequestStream.Position = 0;
            }

            request.SendRequest();
            return request;
        }

        public static void SetHeaderValues(TestWebRequest request, IEnumerable<KeyValuePair<string, string>> headerValues)
        {
            if (headerValues != null)
            {
                foreach (KeyValuePair<string, string> header in headerValues)
                {
                    if (header.Key == "If-Match")
                    {
                        request.IfMatch = header.Value;
                    }
                    else if (header.Key == "If-None-Match")
                    {
                        request.IfNoneMatch = header.Value;
                    }
                    else if (header.Key == "OData-MaxVersion")
                    {
                        request.RequestMaxVersion = header.Value;
                    }
                    else if (header.Key == "Prefer")
                    {
                        request.RequestHeaders["Prefer"] = header.Value;
                    }
                    else
                    {
                        Assert.Fail(String.Format("Unknown header '{0}'", header.Key));
                    }
                }
            }
        }

        public static void VerifyResponseHeaders(TestWebRequest request, IEnumerable<KeyValuePair<string, string>> headerValues)
        {
            if (headerValues != null)
            {
                foreach (var h in headerValues)
                {
                    switch(h.Key)
                    {
                        case "StatusCode":
                            Assert.AreEqual(h.Value, request.ResponseStatusCode.ToString());
                            break;
                        case "ContentType":
                            Assert.IsTrue(h.Value == request.ResponseContentType || request.ResponseContentType.StartsWith(h.Value, StringComparison.OrdinalIgnoreCase), "The response content type is unexpected: {0}", request.ResponseContentType);
                            break;
                        case "Version":
                            Assert.AreEqual(h.Value, request.ResponseVersion);
                            break;
                        case "Preference-Applied":
                            Assert.AreEqual(h.Value, request.ResponseHeaders["Preference-Applied"]);
                            break;
                        default:
                            Assert.Fail("Unexpected header value: {0}", h.Key);
                            break;
                    }
                }
            }
        }

        public static bool HasElementNullValue(XmlElement element, string responseFormat)
        {
            if (String.Equals(responseFormat, JsonLightMimeType, StringComparison.OrdinalIgnoreCase))
            {
                return (element.GetAttributeNode("IsNull") != null);
            }
            else if (String.Equals(responseFormat, AtomFormat, StringComparison.OrdinalIgnoreCase))
            {
                return (element.GetAttributeNode("null", "http://docs.oasis-open.org/odata/ns/metadata") != null);
            }
            else
            {
                Assert.Fail("invalid format");
                return false;
            }
        }

        public enum SendRequestModifier
        {
            None,
            UseBatchRequest,
            UsePostTunneling
        };

        public static void SendRequestAndVerifyXPath(
            string payload,
            string uri,
            KeyValuePair<string, string[]>[] uriAndXPathsToVerify,
            Type contextType,
            string responseFormat,
            string httpMethodName,
            IEnumerable<KeyValuePair<string, string>> requestHeaders = null,
            bool verifyETag = false,
            IEnumerable<KeyValuePair<string, string>> responseHeaders = null,
            SendRequestModifier sendRequestModified = SendRequestModifier.None)
        {
            // */* is more generic than application/atom+xml.
            string accept = responseFormat;
            string contentType = responseFormat;
            if (accept == AtomFormat)
            {
                accept = "application/atom+xml,application/xml";
            }

            if (uri.Contains("$ref") && responseFormat == AtomFormat)
            {
                contentType = MimeApplicationXml;
            }
            
            TestUtil.TraceScopeForException("SendRequestAndVerifyXPath", delegate()
            {
                using (TestUtil.RestoreStaticValueOnDispose(typeof(BaseTestWebRequest), "HostInterfaceType"))
                using (TestWebRequest baseRequest = TestWebRequest.CreateForInProcess())
                {
                    baseRequest.DataServiceType = contextType;
                    baseRequest.Accept = UnitTestsUtil.MimeApplicationXml;

                    BaseTestWebRequest.HostInterfaceType = typeof(Microsoft.OData.Service.IDataServiceHost2);

                    TestWebRequest request = sendRequestModified == SendRequestModifier.UseBatchRequest ? new InMemoryWebRequest() : baseRequest;

                    request.RequestUriString = uri;
                    request.Accept = accept;
                    request.HttpMethod = httpMethodName;
                    SetHeaderValues(request, requestHeaders);
                    request.RequestStream = new MemoryStream();
                    request.RequestContentType = contentType;
                    StreamWriter writer = new StreamWriter(request.RequestStream);
                    writer.Write(payload);
                    writer.Flush();

                    if (sendRequestModified == SendRequestModifier.UseBatchRequest)
                    {
                        var batch = new BatchWebRequest();
                        var changeset = new BatchWebRequest.Changeset();
                        var part = (InMemoryWebRequest)request;

                        // Normal DELETE requests can have Content-Type (We don't fail on that), but $batch requests can't - remove it here
                        if (part.HttpMethod == "DELETE")
                        {
                            part.RequestContentType = null;
                        }

                        changeset.Parts.Add(part);
                        batch.Changesets.Add(changeset);
                        // This will send the batch and parse the response applying it to the part (which is the request variable)
                        batch.SendRequest(baseRequest);
                    }
                    else if (sendRequestModified == SendRequestModifier.UsePostTunneling &&
                        (baseRequest.HttpMethod == "PATCH" || baseRequest.HttpMethod == "PUT" || baseRequest.HttpMethod == "DELETE"))
                    {
                        baseRequest.RequestHeaders["X-HTTP-Method"] = baseRequest.HttpMethod;
                        baseRequest.HttpMethod = "POST";
                        baseRequest.SendRequest();
                    }
                    else
                    {
                        baseRequest.SendRequest();
                    }

                    VerifyResponseHeaders(request, responseHeaders);
                    if (httpMethodName == "PUT" || httpMethodName == "PATCH" || httpMethodName == "POST")
                    {
                        if (verifyETag)
                        {
                            Assert.IsTrue(
                                request.ResponseETag != null,
                                "request.ResponseETag != null, although PUT was used and verifyETag is true");
                        }
                        else
                        {
                            Assert.IsTrue(
                                request.ResponseETag == null,
                                "request.ResponseETag == null, there must be no etag in the response");
                        }
                    }

                    if (uriAndXPathsToVerify != null && uriAndXPathsToVerify.Length > 0 && httpMethodName == "POST" && !uri.Contains("$ref"))
                    {
                        using (Stream resultStream = request.GetResponseStream())
                        {
                            Debug.Assert(uriAndXPathsToVerify[0].Key == uri, "need to validate the response payload in POST");
                            VerifyXPaths(resultStream, responseFormat, uriAndXPathsToVerify[0].Value);
                        }
                    }
                    else if ((httpMethodName == "PUT" || httpMethodName == "PATCH"))
                    {
                        if (request.ResponseStatusCode == 200)
                        {
                            if (uriAndXPathsToVerify != null && uriAndXPathsToVerify.Length > 0 && uriAndXPathsToVerify[0].Key == uri)
                            {
                                using (Stream resultStream = request.GetResponseStream())
                                {
                                    VerifyXPaths(resultStream, responseFormat, uriAndXPathsToVerify[0].Value);
                                }
                            }
                        }
                        else
                        {
                            Assert.IsTrue(request.ResponseStatusCode == 204, "When GenerateResponseForUpdateOperations is set to false, the status code must be 204");
                        }
                    }
                    else if (httpMethodName == "GET")
                    {
                        if (uriAndXPathsToVerify != null)
                        {
                            Assert.IsTrue(uriAndXPathsToVerify.Length == 1, "Length must be one in GET method");
                            Assert.IsTrue(uriAndXPathsToVerify[0].Key == uri, "uri must be the same");
                            Stream resultStream = request.GetResponseStream();
                            VerifyXPaths(resultStream, responseFormat, uriAndXPathsToVerify[0].Value);
                        }

                        return;
                    }
                }

                if (uriAndXPathsToVerify != null)
                {
                    for (int i = 0; i < uriAndXPathsToVerify.Length; i++)
                    {
                        // For POST operations, skip the first uri, since it has been validated by the POST response
                        if (httpMethodName == "POST" && i == 0 && !uri.Contains("$ref"))
                        {
                            continue;
                        }

                        Trace.WriteLine("Verifying payloads for " + contextType + " with " + responseFormat);
                        string payloadUri = uriAndXPathsToVerify[i].Key;
                        string[] xpaths = uriAndXPathsToVerify[i].Value;
                        if (xpaths != null && xpaths.Length == 1 && xpaths[0] == "404")
                        {
                            VerifyInvalidUri(payloadUri, contextType);
                        }
                        else
                        {
                            VerifyPayload(payloadUri, contextType, responseFormat, null, xpaths);
                        }
                    }
                }
            });
        }

        public static void VerifyInvalidUri(string uri, Type dataServiceType)
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("ResponseFormat", ResponseFormats),
                new Dimension("WebServerLocation", new object[] { WebServerLocation.InProcess }));

            TestUtil.RunCombinatorialEngineFail(engine, (values) =>
            {
                WebServerLocation location = (WebServerLocation)values["WebServerLocation"];
                string responseFormat = (string)values["ResponseFormat"];

                if (responseFormat == AtomFormat)
                {
                    // */* will also work for complex types.
                    responseFormat = MimeAny;
                }

                try
                {
                    GetResponseStream(location, responseFormat, uri, dataServiceType);
                    Assert.Fail(String.Format("404 expected for uri '{0}'", uri));
                }
                catch (WebException exception)
                {
                    DataServiceException innerException = (DataServiceException)exception.InnerException;
                    Assert.AreEqual(404, innerException.StatusCode);
                }
            });
        }

        public static void VerifyPayload(string uri, Type dataServiceType, string responseFormat, Func<Hashtable, XmlDocument, bool> testCallback,
            string[] xPaths)
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("WebServerLocation", new object[] { WebServerLocation.InProcess }));

            Hashtable values = new Hashtable();
            while (engine.Next(values))
            {
                string accept = responseFormat;
                if (accept == AtomFormat)
                {
                    // */* will also work for complex types.
                    accept = "application/xml, application/atom+xml";
                }

                WebServerLocation location = (WebServerLocation)values["WebServerLocation"];
                Stream resultStream = GetResponseStream(location, accept, uri, dataServiceType);
                XmlDocument document = VerifyXPaths(resultStream, responseFormat, xPaths);
                if (testCallback != null && !testCallback(values, document))
                {
                    Assert.Fail("test callback failed");
                }
            }
        }

        public static void VerifyPayload(string uri, Type dataServiceType, Func<Hashtable, XmlDocument, bool> testCallback,
                                                                    string[] web3sXpaths, string[] jsonXPaths, string[] atomXPaths)
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("ResponseFormat", ResponseFormats),
                new Dimension("WebServerLocation", new object[] { WebServerLocation.InProcess }));

            Hashtable values = new Hashtable();
            while (engine.Next(values))
            {
                string responseFormat = (string)values["ResponseFormat"];
                WebServerLocation location = (WebServerLocation)values["WebServerLocation"];

                // */* will also work for complex types.
                string accept = responseFormat;
                if (String.Equals(accept, AtomFormat, StringComparison.OrdinalIgnoreCase))
                {
                    accept = "application/atom+xml,application/xml";
                }

                Stream resultStream = UnitTestsUtil.GetResponseStream(location, accept, uri, dataServiceType);
                XmlDocument document = VerifyXPaths(resultStream, responseFormat, web3sXpaths, jsonXPaths, atomXPaths);
                if (testCallback != null && !testCallback(values, document))
                {
                    Assert.Fail("test callback failed");
                }
            }
        }

        public static Stream GetResponseStream(WebServerLocation location, string accept, string uri, Type dataServiceType)
        {
            using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
            {
                request.DataServiceType = dataServiceType;
                request.RequestUriString = uri;
                request.Accept = accept;
                request.SendRequest();

                // Trace.WriteLine("ContentType: " + request.ResponseContentType);
                return request.GetResponseStream();
            }
        }

        public static void VerifyInvalidRequestForVariousProviders(string payload, string uri, string contentFormat, string httpMethodName, int expectedHttpErrorCode, params KeyValuePair<string, string>[] headerValues)
        {
            VerifyInvalidRequest(payload, uri, typeof(CustomDataContext), contentFormat, httpMethodName, expectedHttpErrorCode, headerValues);
        }

        public static void VerifyInvalidRequestForVariousProviders(string payload, string uri, string contentFormat, string httpMethodName, int expectedHttpErrorCode)
        {
            VerifyInvalidRequest(payload, uri, typeof(CustomDataContext), contentFormat, httpMethodName, expectedHttpErrorCode);
        }

        public static void VerifyInvalidRequest(string payload, string uri, Type contextType, string contentFormat, string httpMethodName, int errorCode)
        {
            VerifyInvalidRequest(payload, uri, contextType, contentFormat, httpMethodName, errorCode, null, null);
        }

        public static void VerifyInvalidRequest(string payload, string uri, Type contextType, string contentFormat, string httpMethodName, int errorCode, params KeyValuePair<string, string>[] headerValues)
        {
            VerifyInvalidRequest(payload, uri, contextType, contentFormat, httpMethodName, errorCode, null, headerValues);
        }

        public static void VerifyInvalidRequest(string payload, string uri, Type contextType, string contentFormat, string httpMethodName, int errorCode, string errorMessage, params KeyValuePair<string, string>[] headerValues)
        {
            Exception exception = TestUtil.RunCatching(delegate()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = contextType;
                    request.RequestUriString = uri;
                    request.Accept = contentFormat;
                    request.HttpMethod = httpMethodName;
                    SetHeaderValues(request, headerValues);
                    request.RequestStream = new MemoryStream();
                    request.RequestContentType = contentFormat;
                    StreamWriter writer = new StreamWriter(request.RequestStream);
                    writer.Write(payload);
                    writer.Flush();
                    request.SendRequest();
                }
            });

            VerifyTestException(exception, errorCode, errorMessage);
        }

        public static void VerifyTestException(Exception exception, int errorCode, string errorMessage)
        {
            TestUtil.AssertExceptionExpected(exception, true);
            TestUtil.AssertExceptionStatusCode(exception, errorCode, "Expecting status code: " + errorCode);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                    if (exception.Message == errorMessage)
                    {
                        return;
                    }
                }

                Assert.AreEqual(errorMessage, exception.Message);
            }
        }

        /// <summary>
        /// Use this to verify XML payload in the server exception
        /// Format is:
        /// <![CDATA[
        ///     <error>
        ///         <message>Message</message>
        ///     </error>
        /// ]]>
        /// </summary>
        public static void VerifyWebExceptionXML(Exception exception, HttpStatusCode expectedStatusCode, string expectedErrorMessage)
        {
            WebException ex = exception as WebException;
            TestUtil.AssertExceptionExpected(ex, true);

            HttpWebResponse response = ex.Response as HttpWebResponse;
            Assert.IsNotNull(response);
            Assert.AreEqual(expectedStatusCode, response.StatusCode);

            if (expectedErrorMessage != null)
            {
                Stream responseStream = response.GetResponseStream();
                XmlDocument xDoc = new XmlDocument(TestUtil.TestNameTable);
                xDoc.Load(responseStream);
                var node = xDoc.SelectSingleNode("adsm:error/adsm:message", TestUtil.TestNamespaceManager);
                Assert.IsNotNull(node);

                Assert.AreEqual(expectedErrorMessage, node.InnerText);
            }
        }

        /// <summary>
        /// Gets the base uri defined in the document that is in scope
        /// for the specified element.
        /// </summary>
        /// <param name="element">Element for which to return the xml:base uri in scope.</param>
        /// <returns>The xml:base uri in scope for the element; null if none is defined.</returns>
        /// <remarks>Note that the BaseURI on XmlElement is a different thing altogether.</remarks>
        public static string GetBaseUri(XmlElement element)
        {
            Debug.Assert(element != null, "element != null");

            while (element != null)
            {
                if (element.HasAttribute("xml:base"))
                {
                    return element.GetAttribute("xml:base");
                }
                else
                {
                    element = element.ParentNode as XmlElement;
                }
            }

            return null;
        }

        public static string GetPayload(object entity, IEdmEntityType entityType, string responseFormat)
        {
            if (responseFormat == JsonLightMimeType)
            {
                return JsonValidator.GetJsonPayload(entityType, entity);
            }
            else
            {
                Assert.IsTrue(responseFormat == AtomFormat, "expecting only atom format");
                return GetAtomPayload(entityType, entity);
            }
        }

        private static string GetAtomPayload(IEdmEntityType entityType, object value)
        {
            string atomNamespace = TestUtil.TestNamespaceManager.LookupNamespace("atom");
            string dataWebMetadataNamespace = TestUtil.TestNamespaceManager.LookupNamespace("adsm");

            StringWriter writer = new StringWriter();

            using (XmlWriter xmlWriter = GetXmlWriter(writer))
            {
                WriteEntryElement(xmlWriter);

                xmlWriter.WriteStartElement("category", atomNamespace);
                xmlWriter.WriteAttributeString("scheme", "http://docs.oasis-open.org/odata/ns/scheme");
                xmlWriter.WriteAttributeString("term", entityType.FullName());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("content", atomNamespace);
                xmlWriter.WriteAttributeString("type", "application/xml");
                xmlWriter.WriteStartElement("properties", dataWebMetadataNamespace);
                var properties = entityType.StructuralProperties().ToList();
                for (int i = 0; i < properties.Count; i++)
                {
                    IEdmProperty property = properties[i];
                    object propertyValue = value.GetType().GetProperty(properties[i].Name).GetValue(value, null);
                    WritePropertyValue(xmlWriter, property.Name, propertyValue);
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }

            return writer.ToString();
        }

        /// <summary>Given the ATOM entry element the method returns the property element for the specified property path.</summary>
        /// <param name="entry">The ATOM entry element.</param>
        /// <param name="path">Path to the property to return.</param>
        /// <returns>The XElement for that property or null if it could not be found.</returns>
        public static XElement GetPropertyPathFromAtomEntryContent(XElement entry, string path)
        {
            XElement properties = entry.Elements(UnitTestsUtil.AtomNamespace + "content").Elements(UnitTestsUtil.MetadataNamespace + "properties").FirstOrDefault();
            if (properties == null)
            {
                properties = entry.Elements(UnitTestsUtil.MetadataNamespace + "properties").FirstOrDefault();
            }
            if (properties == null) return null;
            return GetPropertyPathFromAtomEntryProperties(properties, path.Split('/'), 0);
        }

        private static XElement GetPropertyPathFromAtomEntryProperties(XElement properties, string[] path, int index)
        {
            if (properties == null || index >= path.Length) return properties;
            return GetPropertyPathFromAtomEntryProperties(properties.Element(UnitTestsUtil.DataNamespace + path[index]), path, index + 1);
        }

        public static XmlWriter GetXmlWriter(TextWriter writer)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CheckCharacters = false;
            settings.Indent = true;
            settings.NewLineHandling = NewLineHandling.Entitize;

            return XmlWriter.Create(writer, settings);
        }

        public static void ComparePropertyValue(XmlDocument document, PropertyInfo property, string responseFormat, object expectedValue)
        {
            object actualValue;
            if (responseFormat == UnitTestsUtil.JsonLightMimeType)
            {
                XmlNode node = document.SelectSingleNode(String.Format("/{0}/{1}", JsonValidator.ObjectString, property.Name));

                // if node has IsNull attribute, then the value is null
                if (node.Attributes.Count != 0 && node.Attributes["IsNull"] != null)
                {
                    actualValue = null;
                }
                else
                {
                    actualValue = JsonValidator.StringToPrimitive(node.InnerText, property.PropertyType);
                }
            }
            else
            {
                Assert.IsTrue(responseFormat == UnitTestsUtil.AtomFormat, "expecting atom format");
                string xpath = "/atom:entry/atom:content/adsm:properties/ads:" + property.Name;
                XmlNode node = TestUtil.AssertSelectSingleElement(document, xpath);

                if (node.Attributes.Count != 0 && node.Attributes["null", TestUtil.TestNamespaceManager.LookupNamespace("adsm")] != null)
                {
                    actualValue = null;
                }
                else
                {
                    actualValue = TypeData.ObjectFromXmlValue(node.InnerText, property.PropertyType);
                }
            }

            // set the property value
            if (actualValue == null && expectedValue == null)
            {
                // nothing to do;
            }
            else if (!TypeData.AreEqual(actualValue.GetType(), expectedValue, actualValue, responseFormat))
            {
                string message = String.Format("Value doesn't match for property {0}. Expected Value: {1}, Actual Value: {2}",
                    property.Name, expectedValue, actualValue);
                throw new Exception(message);
            }
        }

        public static void WriteEntryElement(XmlWriter xmlWriter)
        {
            string atomNamespace = TestUtil.TestNamespaceManager.LookupNamespace("atom");
            string dataWebNamespace = TestUtil.TestNamespaceManager.LookupNamespace("ads");
            string dataWebMetadataNamespace = TestUtil.TestNamespaceManager.LookupNamespace("adsm");

            xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='utf-8'");
            xmlWriter.WriteStartElement("entry", atomNamespace);
            xmlWriter.WriteAttributeString("xmlns", "ads", null, dataWebNamespace);
            xmlWriter.WriteAttributeString("xmlns", "adsm", null, dataWebMetadataNamespace);
        }

        public static void WritePropertyValue(XmlWriter xmlWriter, string propertyName, object propertyValue)
        {
            WritePropertyValue(xmlWriter, propertyName, propertyValue, null);
        }

        public static void WritePropertyValue(XmlWriter xmlWriter, string propertyName, object propertyValue, string typeName)
        {
            string dataWebNamespace = TestUtil.TestNamespaceManager.LookupNamespace("ads");
            string dataWebMetadataNamespace = TestUtil.TestNamespaceManager.LookupNamespace("adsm");

            xmlWriter.WriteStartElement(propertyName, dataWebNamespace);
            if (typeName != null)
            {
                xmlWriter.WriteAttributeString("type", dataWebMetadataNamespace, typeName);
            }

            if (propertyValue == null)
            {
                xmlWriter.WriteAttributeString("null", dataWebMetadataNamespace, "true");
            }
            else
            {
                string stringValue = TypeData.XmlValueFromObject(propertyValue);
                if (stringValue.Length > 0 &&
                    Char.IsWhiteSpace(stringValue, 0) &&
                    Char.IsWhiteSpace(stringValue, stringValue.Length - 1))
                {
                    xmlWriter.WriteAttributeString("xml", "space", null, "preserve");
                }
                xmlWriter.WriteValue(stringValue);
            }
            xmlWriter.WriteEndElement();
        }

        public static void UpdateAndVerifyPrimitiveProperty(Type contextType, string httpMethodName, string uri, string payload, string contentType, KeyValuePair<string, string>[] headerValues)
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = contextType;
                request.RequestUriString = uri;
                request.HttpMethod = httpMethodName;
                SetHeaderValues(request, headerValues);
                request.RequestStream = new MemoryStream();
                request.RequestContentType = contentType;
                StreamWriter writer = new StreamWriter(request.RequestStream);
                writer.Write(payload);
                writer.Flush();

                request.SendRequest();

                VerifyPrimitiveValue(WebServerLocation.InProcess, contentType, uri, contextType, payload);
            }
        }

        public static void VerifyPrimitiveValue(WebServerLocation location, string contentType, string uri, Type contextType, string expectedValue)
        {
            Stream resultStream = UnitTestsUtil.GetResponseStream(location, contentType, uri, contextType);
            if (contentType == UnitTestsUtil.MimeTextPlain)
            {
                string result = new StreamReader(resultStream).ReadToEnd();
                Assert.AreEqual<string>(result, expectedValue, "The payload doesn't match with the expected type");
            }
            else
            {
                Assert.AreEqual<string>(contentType, UnitTestsUtil.MimeApplicationOctetStream);
                byte[] actualValue = TestUtil.ReadStream(resultStream);
                Assert.AreEqual<int>(expectedValue.Length, actualValue.Length);

                for (int i = 0; i < expectedValue.Length; i++)
                {
                    Assert.AreEqual<byte>(Convert.ToByte(expectedValue[i]), actualValue[i]);
                }
            }
        }

        public static void CustomProviderRequest(Type providerType, string uri, string responseFormat, string payload, string[] xPaths, string httpMethodName, bool verifyETagReturned)
        {
            var uriAndXPathsToVerify = xPaths == null ? new KeyValuePair<string, string[]>[0] : new KeyValuePair<string, string[]>[] { new KeyValuePair<string, string[]>(uri, xPaths) };
            CustomProviderRequest(providerType, uri, responseFormat, payload, uriAndXPathsToVerify, httpMethodName, verifyETagReturned);
        }

        public static void CustomProviderRequest(
            Type providerType,
            string uri,
            string responseFormat,
            string payload,
            KeyValuePair<string, string[]>[] uriAndXPathsToVerify,
            string httpMethodName,
            bool verifyETagReturned,
            IList<KeyValuePair<string, string>> requestHeaders = null,
            IList<KeyValuePair<string, string>> responseHeaders = null,
            UnitTestsUtil.SendRequestModifier sendRequestModifier = UnitTestsUtil.SendRequestModifier.None)
        {
            using (UnitTestsUtil.CreateChangeScope(providerType))
            {
                string newUri = UnitTestsUtil.ConvertUri(providerType, uri);

                // POST must not include the ETag headers
                if ((!uri.Contains("$ref") || !verifyETagReturned) && httpMethodName != "POST")
                {
                    // Get the etag and if its present, then pass to the PUT request
                    var newFormat = newUri.Contains("$value") ? UnitTestsUtil.MimeTextPlain : responseFormat;
                    string etag = UnitTestsUtil.GetETagFromResponse(providerType, newUri, newFormat);
                    if (etag != null)
                    {
                        requestHeaders = requestHeaders ?? new List<KeyValuePair<string, string>>();
                        requestHeaders.Add(new KeyValuePair<string, string>("If-Match", etag));
                    }
                }

                UnitTestsUtil.SendRequestAndVerifyXPath(
                    UnitTestsUtil.ConvertPayload(providerType, payload),
                    newUri,
                    UnitTestsUtil.ConvertUriAndXPaths(providerType, uriAndXPathsToVerify),
                    providerType,
                    responseFormat,
                    httpMethodName,
                    requestHeaders,
                    verifyETagReturned,
                    responseHeaders,
                    sendRequestModifier);
            }
        }

        public static void DoInsertsForVariousProviders(string uri, string responseFormat, string payload, KeyValuePair<string, string[]>[] uriAndXPathsToVerify, bool verifyETag)
        {
            foreach (Type provider in UnitTestsUtil.ProviderTypes)
            {
                CustomProviderRequest(provider, uri, responseFormat, payload, uriAndXPathsToVerify, "POST", verifyETag);
            }
        }

        public static void DoInsertsForVariousProviders(string uri, string responseFormat, PayloadBuilder payloadBuilder, KeyValuePair<string, string[]>[] uriAndXPathsToVerify, bool verifyETag)
        {
            TestUtil.RunCombinations(UnitTestsUtil.ProviderTypes, (provider) =>
            {
                using (AppendTypesForOpenProperties(provider, payloadBuilder, responseFormat))
                {
                    CustomProviderRequest(provider, uri, responseFormat, PayloadGenerator.Generate(payloadBuilder, responseFormat), uriAndXPathsToVerify, "POST", verifyETag);
                }
            });
        }

        private static List<KeyValuePair<string, string[]>> typeToOpenPropertyMappings = new List<KeyValuePair<string, string[]>>()
                    {
                        new KeyValuePair<string, string[]>(typeof(Customer).FullName, new string[] { "Name", "NameAsHtml" }),
                        new KeyValuePair<string, string[]>(typeof(CustomerWithBirthday).FullName, new string[] { "Name", "NameAsHtml", "Birthday" }),
                        new KeyValuePair<string, string[]>(typeof(Address).FullName, new string[] { "StreetAddress", "City", "State", "PostalCode" }),
                        new KeyValuePair<string, string[]>(typeof(Order).FullName, new string[] { "DollarAmount" }),
                    };

        private static void UpdateOpenPropertiesProperty(PayloadBuilder payloadBuilder, List<PayloadBuilder> payloadBuilderToReset)
        {
            IEnumerable<string> openPropertyList = null;
            if (payloadBuilder.TypeName != null)
            {
                var openTypePayloadBuilder = typeToOpenPropertyMappings.FirstOrDefault(m => m.Key == payloadBuilder.TypeName);
                openPropertyList = openTypePayloadBuilder.Value;
            }
            else
            {
                var propertyList = new List<string>();
                foreach (var mapping in typeToOpenPropertyMappings)
                {
                    propertyList.AddRange(mapping.Value);
                }

                openPropertyList = propertyList;
            }

            if (openPropertyList != null)
            {
                // Set the list of open properties
                payloadBuilder.OpenProperties = openPropertyList.ToArray();
                payloadBuilderToReset.Add(payloadBuilder);

                if (payloadBuilder.Properties != null)
                {
                    foreach (var property in payloadBuilder.Properties)
                    {
                        PayloadBuilder referencePropertyPayloadBuilder = property.Value as PayloadBuilder;
                        if (referencePropertyPayloadBuilder != null)
                        {
                            UpdateOpenPropertiesProperty(referencePropertyPayloadBuilder, payloadBuilderToReset);
                            continue;
                        }

                        IEnumerable<PayloadBuilder> collectionPayloadBuilder = property.Value as IEnumerable<PayloadBuilder>;
                        if (collectionPayloadBuilder != null)
                        {
                            foreach (var element in collectionPayloadBuilder)
                            {
                                UpdateOpenPropertiesProperty(element, payloadBuilderToReset);
                            }

                            continue;
                        }
                    }
                }
            }
        }

        public static string ConvertTypeNames(string input, object context)
        {
            return ConvertTypeNames(context.GetType(), input);
        }

        public static string ConvertReflectionProviderTypesToEdmProviderTypes(string input)
        {
            if (input != null)
            {
                input = input.Replace(typeof(Customer).FullName, UnitTestsUtil.CustomerTypeName);
                input = input.Replace(typeof(CustomerWithBirthday).FullName, UnitTestsUtil.CustomerWithBirthdayTypeName);
                input = input.Replace(typeof(Order).FullName, UnitTestsUtil.OrderTypeName);
                input = input.Replace(typeof(Address).FullName, UnitTestsUtil.AddressTypeName);
            }
            return input;
        }

        public static string ConvertReflectionProviderTypesToCustomRowBasedProviderTypes(string input)
        {
            if (input != null)
            {
                input = input.Replace(typeof(Customer).FullName, CustomRowBasedContext.CustomerFullName);
                input = input.Replace(typeof(CustomerWithBirthday).FullName, CustomRowBasedContext.CustomerWithBirthdayFullName);
                input = input.Replace(typeof(Order).FullName, CustomRowBasedContext.OrderFullName);
                input = input.Replace(typeof(Address).FullName, CustomRowBasedContext.AddressFullName);
            }
            return input;
        }

        public static string ConvertTypeNames(Type providerType, string input)
        {
            if (providerType == typeof(CustomDataContext))
            {
                return input;
            }

            if (providerType == typeof(ocs.CustomObjectContext) ||
                providerType == typeof(EFFK.CustomObjectContextPOCOProxy) ||
                providerType == typeof(EFFK.CustomObjectContextPOCO))
            {
                return ConvertReflectionProviderTypesToEdmProviderTypes(input);
            }

            if (typeof(CustomRowBasedContext).IsAssignableFrom(providerType) ||
                typeof(CustomRowBasedOpenTypesContext).IsAssignableFrom(providerType))
            {
                return ConvertReflectionProviderTypesToCustomRowBasedProviderTypes(input);
            }

            Debug.Assert(false, String.Format("Invalid provider type encountered - {0} in ConvertTypeNames", providerType.FullName));
            return null;
        }

        public static string ConvertPayload(Type providerType, string payload)
        {
            payload = ConvertTypeNames(providerType, payload);

            if (providerType == typeof(EFFK.CustomObjectContextPOCO) ||
                providerType == typeof(EFFK.CustomObjectContextPOCOProxy))
            {
                return ConvertPayloadUriToEFPOCOUri(payload);
            }

            return payload;
        }

        public static string ConvertUri(Type providerType, string uri)
        {
            if (providerType == typeof(EFFK.CustomObjectContextPOCO) ||
                providerType == typeof(EFFK.CustomObjectContextPOCOProxy))
            {
                return ConvertUriForEFPOCO(uri);
            }

            return uri;
        }

        public static string ConvertXPath(Type providerType, string xPath)
        {
            if (providerType == typeof(CustomDataContext))
            {
                return xPath;
            }

            if (providerType == typeof(ocs.CustomObjectContext))
            {
                return ConvertReflectionProviderTypesToEdmProviderTypes(xPath);
            }

            if (providerType == typeof(EFFK.CustomObjectContextPOCO) ||
                providerType == typeof(EFFK.CustomObjectContextPOCOProxy))
            {
                xPath = ConvertReflectionProviderTypesToEdmProviderTypes(xPath);
                return UpdateUriInXPathForEFPOCO(xPath);
            }

            if (typeof(CustomRowBasedContext).IsAssignableFrom(providerType) ||
                typeof(CustomRowBasedOpenTypesContext).IsAssignableFrom(providerType))
            {
                return ConvertReflectionProviderTypesToCustomRowBasedProviderTypes(xPath);
            }

            Debug.Assert(false, String.Format("Invalid provider type encountered in ConvertXPaths- {0}", providerType.FullName));
            return null;
        }

        public static KeyValuePair<string, string[]>[] ConvertUriAndXPaths(Type providerType, KeyValuePair<string, string[]>[] uriAndXPaths)
        {
            var newUriAndXPaths = new KeyValuePair<string, string[]>[uriAndXPaths.Length];

            for (int i = 0; i < newUriAndXPaths.Length; i++)
            {
                var newXPaths = new string[uriAndXPaths[i].Value.Length];
                for (int j = 0; j < newXPaths.Length; j++)
                {
                    newXPaths[j] = ConvertXPath(providerType, uriAndXPaths[i].Value[j]);
                }

                newUriAndXPaths[i] = new KeyValuePair<string, string[]>(
                    ConvertUri(providerType, uriAndXPaths[i].Key),
                    newXPaths);
            }

            return newUriAndXPaths;
        }

        public static IDisposable CreateChangeScope(Type providerType)
        {
            return (IDisposable)providerType.InvokeMember("CreateChangeScope", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy, null, null, null);
        }

        private static string ConvertUriForEFPOCO(string uri)
        {
            Debug.Assert(uri.StartsWith("/", StringComparison.Ordinal), "uri must be relative");
            return uri[0] + "CustomObjectContext." + uri.Substring(1);
        }

        private static string ConvertPayloadUriToEFPOCOUri(string payload)
        {
            var entitySetNames = new string[] { "Customers", "Orders", "OrderDetails", "Workers", "Offices", "CustomerBlobs" };

            if (payload != null)
            {
                foreach (var set in entitySetNames)
                {
                    // replace uri in json payload
                    //payload = payload.Replace("'/Customers", "'/CustomObjectContext.Customers");
                    payload = payload.Replace("'/" + set, "'/CustomObjectContext." + set);
                    payload = payload.Replace("\"/" + set, "\"/CustomObjectContext." + set);

                    // replace uri in atom payload
                    //payload = payload.Replace("'Customers", "'CustomObjectContext.Customers");
                    payload = payload.Replace("'" + set, "'CustomObjectContext." + set);
                    payload = payload.Replace("\"" + set, "\"CustomObjectContext." + set);

                    // replace uri in $ref payload
                    //payload = payload.Replace(">/Customers", ">/CustomObjectContext.Customers");
                    payload = payload.Replace(">/" + set, ">/CustomObjectContext." + set);

                    // replace uri in $ref payload
                    //payload = payload.Replace(">Customers", ">CustomObjectContext.Customers");
                    payload = payload.Replace(">" + set, ">CustomObjectContext." + set);

                    // replace absolute uri in payload
                    //payload = payload.Replace("http://odata.org/Customers", "http://odata.org/CustomObjectContext.Customers");
                    payload = payload.Replace("http://odata.org/" + set, "http://odata.org/CustomObjectContext." + set);
                }

            }

            return payload;
        }

        private static string UpdateUriInXPathForEFPOCO(string xPath)
        {
            var entitySetNames = new string[] { "Customers", "Orders", "OrderDetails", "Workers", "Offices", "CustomerBlobs" };

            foreach (var set in entitySetNames)
            {
                xPath = xPath.Replace("http://host/" + set, "http://host/CustomObjectContext." + set);
            }

            xPath = ConvertPayloadUriToEFPOCOUri(xPath);
            return xPath;
        }

        public static string AddParenthesisForCollections(string uri)
        {
            // For batch requests we need to make sure the uri is not /Orders - we need to check if it has some other
            // character other than space preceding it.
            // For normal requests, if its starts with /Orders, then we don't need to do anything.
            string uriToBeReplaced = "/Orders";
            if (!uri.StartsWith(uriToBeReplaced))
            {
                int startingIndex = 0;
                int currentIndex = -1;
                // for batch these can occur multiple times
                while ((currentIndex = uri.IndexOf(uriToBeReplaced, startingIndex)) != -1)
                {
                    // If the /Orders does not appear at the start of an uri (has something preceding it other than space)
                    // and there is no further dereferencing of the /Orders uri (something like '/Orders('),
                    // then we need to insert () at the end.
                    if (currentIndex != 0 && uri[currentIndex - 1] != ' ' && (currentIndex + uriToBeReplaced.Length) < uri.Length && uri[currentIndex + uriToBeReplaced.Length] != '(')
                    {
                        uri = uri.Insert(currentIndex + uriToBeReplaced.Length, "()");
                    }

                    startingIndex = currentIndex + 1;
                }
            }

            return uri;
        }

        private static IEnumerable<providers.ResourceType> primitiveResourceTypes;

        /// <summary>Returns list of all primitive resource types.</summary>
        /// <returns>All primitive resource types.</returns>
        public static IEnumerable<providers.ResourceType> GetPrimitiveResourceTypes()
        {
            if (primitiveResourceTypes == null)
            {
                Type primitiveResourceTypeMappingType = typeof(providers.ResourceType).Assembly.GetType("Microsoft.OData.Service.Providers.PrimitiveResourceTypeMap");
                PropertyInfo primitiveResourceTypeMapPropertyInfo = primitiveResourceTypeMappingType.GetProperty("TypeMap", BindingFlags.NonPublic | BindingFlags.Static);
                object primitiveResourceTypeMap = primitiveResourceTypeMapPropertyInfo.GetValue(null, null);
                PropertyInfo allPrimitivesPropertyInfo = primitiveResourceTypeMap.GetType().GetProperty("AllPrimitives", BindingFlags.NonPublic | BindingFlags.Instance);
                primitiveResourceTypes = (providers.ResourceType[])allPrimitivesPropertyInfo.GetValue(primitiveResourceTypeMap, null);
            }

            return primitiveResourceTypes;
        }

        /// <summary>
        /// Determine whether a resource type can be used as key or etag property
        /// </summary>
        /// <param name="t">the resource type</param>
        /// <returns>True if the type can be used as key or etag property</returns>
        public static bool IsKeyOrETagPrimitiveType(providers.ResourceType t)
        {
            return t.FullName != "Edm.Stream" && !typeof(Microsoft.Spatial.Geography).IsAssignableFrom(t.InstanceType) && !typeof(Microsoft.Spatial.Geometry).IsAssignableFrom(t.InstanceType);
        }

        /// <summary>Returns true if the specified CLR type is considered to be a primitive type.</summary>
        /// <param name="t">The type to test.</param>
        /// <returns>True if the type is primitive</returns>
        public static bool IsPrimitiveType(Type t)
        {
            return GetPrimitiveResourceTypes().Any(rt => rt.InstanceType == t);
        }

        public static IQueryable<T> GetQueryable<T>(IQueryable<T> result, Microsoft.OData.Service.Providers.IDataServiceQueryProvider dsp, string typeNamePropertyName)
        {
            OpenTypeQueryProvider openTypeQueryProvider = new OpenTypeQueryProvider(result.Provider, dsp, typeNamePropertyName);
            return new OpenTypeQueryable<T>(result, openTypeQueryProvider);
        }

        #region DisposableAction
        private class DisposableAction : IDisposable
        {
            private Action action;
            public DisposableAction(Action action) { this.action = action; }
            public void Dispose()
            {
                action();
            }
        }
        #endregion

        /// <summary>Creates an instance of <see cref="IDisposable"/> which will call the specified <paramref name="action"/>
        /// when it's disposed.</summary>
        /// <param name="action">The action to call when the object is disposed.</param>
        /// <returns>An instance of an object which implements <see cref="IDisposable"/> interface.</returns>
        public static IDisposable CreateDisposableAction(Action action)
        {
            return new DisposableAction(action);
        }

        /// <summary>Processes a string input and resolves all variables in the form of $(varname).</summary>
        /// <param name="input">The string to process.</param>
        /// <param name="variableResolver">Function called to resolve variable names.</param>
        /// <returns>The processed string.</returns>
        /// <remarks>The syntax of the variables is $(varname). The varname can consist of any charact other then ). If the character ) is needed it needs to be doubled.
        /// So $(some())) is a variable reference to a variable of name some().</remarks>
        public static string ProcessStringVariables(string input, Func<string, string> variableResolver)
        {
            if (string.IsNullOrEmpty(input)) return input;
            int startIndex = 0;
            StringBuilder result = new StringBuilder();
            while (startIndex >= 0 && startIndex < input.Length)
            {
                int variableReferenceStart = input.IndexOf("$(", startIndex);
                if (variableReferenceStart >= 0)
                {
                    int variableReferenceEnd = variableReferenceStart + 2;
                    while (variableReferenceEnd < input.Length)
                    {
                        if (input[variableReferenceEnd] == ')')
                        {
                            if (variableReferenceEnd + 1 < input.Length && input[variableReferenceEnd + 1] == ')')
                            {
                                // Double )) - escape sequence
                                variableReferenceEnd += 2;
                            }
                            else
                            {
                                // Found the end
                                break;
                            }
                        }
                        else
                        {
                            variableReferenceEnd++;
                        }
                    }

                    if (variableReferenceEnd < input.Length)
                    {
                        string variableName = input.Substring(variableReferenceStart + 2, variableReferenceEnd - variableReferenceStart - 2);
                        result.Append(input.Substring(startIndex, variableReferenceStart - startIndex));
                        result.Append(variableResolver(variableName));
                        startIndex = variableReferenceEnd + 1;
                        continue;
                    }
                }

                result.Append(input.Substring(startIndex));
                break;
            }

            return result.ToString();
        }

        public static string GetModelTypeName(Type type)
        {
            Type reflectionProvider = typeof(Microsoft.OData.Service.Providers.ResourceType).Assembly.GetType("Microsoft.OData.Service.CommonUtil");

            return (string)reflectionProvider.InvokeMember("GetModelTypeName",
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod,
                null,
                null,
                new object[] { type });
        }

        public static bool IsSuccessStatusCode(int statusCode)
        {
            return (200 <= (int)statusCode && (int)statusCode < 300);
        }

        public static DataServiceHost StartHostProcess(Type serviceType, string baseAddressFormat, Action<DataServiceHost> beforeOpening)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    int localPort = NetworkUtil.GetRandomPortNumber();

                    string baseAddress = baseAddressFormat.Replace("{port}", localPort.ToString(CultureInfo.InvariantCulture));

                    //If debugger is attached use real host name instead of "localhost" to allow a proxy intercepting traffic (i.e. fiddler can be used for debugging messages) 
                    DataServiceHost host = new DataServiceHost(serviceType, new Uri[] { new Uri(baseAddress) });

                    if (beforeOpening != null)
                    {
                        beforeOpening(host);
                    }

                    host.Open();

                    // If successful, then break from the loop
                    return host;
                }
                catch (AddressAlreadyInUseException)
                {
                    // the port is in use
                }
                catch (InvalidOperationException)
                {
                    // the port is in use
                }
            }

            throw new Exception("Failed to start the host process. All the 10 attempts failed.");
        }

        public static Process StartIISExpress(string exePath, string physicalPath, ref int portNumber)
        {
            Process process = null;

            for (int i = 0; i < 10; i++)
            {
                portNumber = NetworkUtil.GetRandomPortNumber();
                string portNumberText = portNumber.ToString(CultureInfo.InvariantCulture);
                string arguments = String.Format("/port:{0} /path:\"{1}\"", portNumberText, physicalPath);

                process = Process.Start(new ProcessStartInfo() { FileName = exePath, Arguments = arguments, CreateNoWindow = true, UseShellExecute = false });

                // if the process has exited, then try again.
                if (process == null || !process.HasExited)
                {
                    return process;
                }
            }

            throw new Exception("Failed to start the Cassini server. All the 10 attempts failed");
        }

        public static IEnumerable<Microsoft.OData.Service.Providers.ResourceProperty> GetNamedStreams(this Microsoft.OData.Service.Providers.ResourceType t)
        {
            return t.Properties.Where(p => (p.Kind & providers.ResourcePropertyKind.Stream) == providers.ResourcePropertyKind.Stream);
        }

        public static IEnumerable<Microsoft.OData.Service.Providers.ResourceProperty> GetNamedStreamsDeclaredOnThisType(this Microsoft.OData.Service.Providers.ResourceType t)
        {
            return t.PropertiesDeclaredOnThisType.Where(p => (p.Kind & providers.ResourcePropertyKind.Stream) == providers.ResourcePropertyKind.Stream);
        }

        public static Exception UnwrapGeneralRequestFailureException(Exception exception)
        {
            DataServiceException dataServiceException = exception as DataServiceException;
            if (dataServiceException != null)
            {
                if (dataServiceException.Message == DataServicesResourceUtil.GetString("DataServiceException_GeneralError") &&
                    dataServiceException.InnerException != null)
                {
                    return dataServiceException.InnerException;
                }
            }

            return exception;
        }

        /// <summary>
        /// Gets the EDMX namespace for the given EDM namespace.
        /// </summary>
        /// <param name="edmNamespace">The EDM namespace to get the EDMX namespace for.</param>
        /// <returns>The EDMX namespace for the <paramref name="edmNamespace"/>.</returns>
        /// <remarks>This method preserves the Astoria behavior to map all EDM versions 
        ///  to the EDMX 1.0 namespace.</remarks>
        public static XNamespace GetEdmxNamespace(XNamespace edmNamespace)
        {
            // The server uses a fixed version "1.0" for all CSDL versions
            return UnitTestsUtil.EdmxNamespace;
        }

        public static IDisposable AppendTypesForOpenProperties(Type providerType, PayloadBuilder payloadBuilder, string format)
        {
            List<PayloadBuilder> payloadBuilderToReset = new List<PayloadBuilder>();

            if (providerType == typeof(CustomRowBasedOpenTypesContext) && String.Equals(format, JsonLightMimeType, StringComparison.OrdinalIgnoreCase))
            {
                UpdateOpenPropertiesProperty(payloadBuilder, payloadBuilderToReset);
            }

            return new PayloadBuilderForOpenTypes(payloadBuilderToReset);
        }
    }

    public static class OtherLinqExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        /// <remarks>
        /// Someday, C# will allow us to specify 'where T : System.EnumType', 
        /// meanwhile this will be relatively inefficient.
        /// </remarks>
        public static bool In<T>(this T value, T set) where T : struct
        {
            object oValue = (object)value;
            object oSet = (object)set;
            int intValue = (int)oValue;
            int intSet = (int)oSet;
            return intValue == (intSet & intValue);
        }

        public static bool HasFlag<T>(this T set, T value) where T : struct
        {
            return value.In(set);
        }

        public static bool Exists<TSource>(this IEnumerable<TSource> source)
        {
            foreach (TSource t in source)
            {
                return true;
            }

            return false;
        }

        public static bool Exists<TSource>(this IEnumerable<TSource> source, System.Linq.Expressions.Expression<Func<TSource, bool>> predicate)
        {
            Func<TSource, bool> f = predicate.Compile();
            foreach (TSource t in source)
            {
                if (f(t))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Exists<TSource>(this IQueryable<TSource> source, System.Linq.Expressions.Expression<Func<TSource, bool>> predicate)
        {
            return 0 < source.Count(predicate);
        }

        /// <summary>
        /// Extension method which turns XmlNodeList into an IEnumerable.
        /// </summary>
        /// <param name="list">The list to convert.</param>
        /// <returns>Enumerable of nodes.</returns>
        public static IEnumerable<XmlNode> ToEnumerable(this XmlNodeList list)
        {
            foreach (XmlNode node in list)
            {
                yield return node;
            }
        }

        /// <summary>
        /// Extension method which turns XmlNodeList into an IEnumerable of specified node type.
        /// </summary>
        /// <param name="list">The list to convert.</param>
        /// <returns>Enumerable of nodes. Nodes which don't match the specified type are skipped.</returns>
        public static IEnumerable<T> ToEnumerable<T>(this XmlNodeList list) where T : XmlNode
        {
            foreach (XmlNode node in list)
            {
                if (node is T)
                {
                    yield return (T)node;
                }
            }
        }

        public static string Concatenate(this IEnumerable<string> source)
        {
            return source.Concatenate("");
        }

        public static string Concatenate(this IEnumerable<string> source, string separator)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (string s in source)
            {
                if (!first)
                {
                    sb.Append(separator);
                }
                sb.Append(s);
                first = false;
            }
            return sb.ToString();
        }

        public static IEnumerable<T> FromSingle<T>(T item)
        {
            yield return item;
        }
    }

    public class PayloadBuilderForOpenTypes : IDisposable
    {
        public PayloadBuilderForOpenTypes(IEnumerable<PayloadBuilder> builders)
        {
            this.Builders = builders;
        }

        private IEnumerable<PayloadBuilder> Builders { get; set; }

        public void Dispose()
        {
            foreach (var pg in this.Builders)
            {
                pg.OpenProperties = null;
            }
        }
    }
}
