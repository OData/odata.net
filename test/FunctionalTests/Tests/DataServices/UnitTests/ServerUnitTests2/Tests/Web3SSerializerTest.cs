//---------------------------------------------------------------------
// <copyright file="Web3SSerializerTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection.Emit;
    using System.Xml;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.StubsOtherNs;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Client;
    using System.Reflection;
    using System.Linq;
    using System.Linq.Expressions;

    #endregion Namespaces

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/875
    /// <summary>
    /// This is a test class for Web3SSerializer and is intended
    /// to contain all Web3SSerializer Unit Tests.
    /// </summary>
    [DeploymentItem("Workspaces", "Workspaces")]
    [TestClass]
    public class Web3SSerializerTest
    {
        private static readonly string JsonFormat = UnitTestsUtil.JsonLightMimeType;
        private static readonly string AtomFormat = UnitTestsUtil.AtomFormat;
        private static readonly string MimeAny = UnitTestsUtil.MimeAny;
        private static readonly string MimeApplicationOctetStream = UnitTestsUtil.MimeApplicationOctetStream;
        private static readonly string MimeApplicationXml = UnitTestsUtil.MimeApplicationXml;
        private static readonly string MimeTextPlain = UnitTestsUtil.MimeTextPlain;
        private static readonly string MimeMultipartMixed = UnitTestsUtil.MimeMultipartMixed;

        private static string[] ResponseFormats = UnitTestsUtil.ResponseFormats;
        private static Type voidType = typeof(void);

        /// <summary>
        /// Verifies that singletons and collections are returned within a wrapper.
        /// </summary>
        [TestCategory("Partition2"), TestMethod]
        public void JsonWrapperTest()
        {
            using (CustomDataContext.CreateChangeScope())
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.Accept = JsonFormat;
                request.DataServiceType = typeof(CustomDataContext);
                request.HttpMethod = "GET";

                var c = new CustomDataContext();
                c.InternalCustomersList.Add(new Customer() { ID = 1000, Name = "a */" });
                c.SaveChanges();

                string[] uris = new string[] { "/Customers", "/Customers(1000)" };
                
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Uri", uris));
                TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                {
                    string uri = (string)values["Uri"];
                    request.RequestUriString = uri;
                    request.SendRequest();

                    string response = request.GetResponseStreamAsText();
                    Assert.IsTrue(response.StartsWith("{"));
                    Assert.IsTrue(response.EndsWith("}"));
                });
            }
        }

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

        public static Stream GetResponseStream(WebServerLocation location, string responseFormat, string uri, Type dataServiceType, string httpMethodName, string payload)
        {
            using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
            {
                request.HttpMethod = httpMethodName;
                if (payload != null)
                {
                    request.SetRequestStreamAsText(payload);
                }

                request.DataServiceType = dataServiceType;
                request.RequestUriString = uri;
                request.RequestContentType = responseFormat;
                request.Accept = responseFormat;
                request.SendRequest();

                // Trace.WriteLine("ContentType: " + request.ResponseContentType);
                return request.GetResponseStream();
            }
        }

        /// <summary>
        /// Verifies that all interesting primitve types round-trip when going
        /// through the XML serializer.
        /// </summary>
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod]
        public void SerializerBasicTest()
        {
            string arr = JsonValidator.ArrayString;
            string obj = JsonValidator.ObjectString;
            string[] jsonXPaths = new string[] {
                                String.Format("/{0}", arr),
                                String.Format("/{0}/{1}", arr, obj),
                                String.Format("/{0}/{1}/{2}", arr, obj, JsonValidator.Metadata),
                                String.Format("/{0}/{1}/{2}/uri[text()='http://host/Customers(0)']", arr, obj, JsonValidator.Metadata),
                                String.Format("/{0}/{1}/{2}/type[text()='AstoriaUnitTests.Stubs.Customer']", arr, obj, JsonValidator.Metadata),
                                String.Format("/{0}/{1}/ID", arr, obj),
                                String.Format("/{0}/{1}/ID[text()='1']", arr, obj),
                                String.Format("/{0}/{1}/Name", arr, obj),
                                String.Format("/{0}/{1}/Name[text()='Customer 1']", arr, obj),
                                String.Format("/{0}/{1}/Address", arr, obj),
                                String.Format("/{0}/{1}/Address/StreetAddress[text()='Line1']", arr, obj),
                                String.Format("/{0}/{1}/Address/City[text()='Redmond']", arr, obj),
                                String.Format("/{0}/{1}/Orders/{2}/uri[text()='http://host/Customers(0)/Orders']", arr, obj, JsonValidator.Deferred),
                                String.Format("/{0}/{1}/BestFriend/{2}/uri[text()='http://host/Customers(0)/BestFriend']", arr, obj, JsonValidator.Deferred) };

            string[] atomXPaths = new string[] {
                                "/atom:feed",
                                "/atom:feed/atom:title[text()='Customers']",
                                "/atom:feed/atom:id[text()='http://host/Customers']",
                                "/atom:feed/atom:updated",
                                "/atom:feed/atom:link[@rel='self' and @href='Customers' and @title='Customers']",
                                "/atom:feed/atom:entry[atom:title='' and atom:id='http://host/Customers(0)' and atom:category/@term='#AstoriaUnitTests.Stubs.Customer']",
                                "/atom:feed/atom:entry[atom:title='' and atom:id='http://host/Customers(1)' and atom:category/@term='#AstoriaUnitTests.Stubs.CustomerWithBirthday']",
                                "/atom:feed/atom:entry/atom:author/atom:name",
                                "/atom:feed/atom:entry/atom:content/adsm:properties[ads:Name='Customer 1']",
                                "/atom:feed/atom:entry/atom:content/adsm:properties/ads:Address/ads:StreetAddress[text()='Line1']",
                                "/atom:feed/atom:entry/atom:content/adsm:properties/ads:Address/ads:City[text()='Redmond']",
                                "/atom:feed/atom:entry/atom:link[@rel='edit' and @href='Customers(0)' and @title='Customer']",
                                "/atom:feed/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/related/Orders' and @href='Customers(1)/AstoriaUnitTests.Stubs.CustomerWithBirthday/Orders' and @title='Orders' and 0=count(@adsm:inline) and @type='application/atom+xml;type=feed']",
                                "/atom:feed/atom:entry/atom:link[@rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' and @href='Customers(1)/AstoriaUnitTests.Stubs.CustomerWithBirthday/BestFriend' and @title='BestFriend' and 0=count(@adsm:inline) and @type='application/atom+xml;type=entry']"};

            VerifyPayload("/Customers", typeof(CustomDataContext), null, null, jsonXPaths, atomXPaths);
        }

        private static void VerifyPayload(string uri, Type dataServiceType, Func<Hashtable, XmlDocument, bool> testCallback,
            string[] web3sXpaths, string[] jsonXPaths, string[] atomXPaths)
        {
            UnitTestsUtil.VerifyPayload(uri, dataServiceType, testCallback, web3sXpaths, jsonXPaths, atomXPaths);
        }

        private static void VerifyPayload(string uri, Type dataServiceType, string responseFormat, Func<Hashtable, XmlDocument, bool> testCallback,
            string[] xPaths)
        {
            UnitTestsUtil.VerifyPayload(uri, dataServiceType, responseFormat, testCallback, xPaths);
        }

        private static void VerifyInvalidUri(string uri, Type dataServiceType)
        {
            UnitTestsUtil.VerifyInvalidUri(uri, dataServiceType);
        }

        private static XmlDocument VerifyXPaths(Stream resultStream, string responseFormat, string[] web3sXPaths, string[] jsonXPaths, string[] atomXPaths)
        {
            return UnitTestsUtil.VerifyXPaths(resultStream, responseFormat, web3sXPaths, jsonXPaths, atomXPaths);
        }

        private static XmlDocument VerifyXPaths(Stream resultStream, string responseFormat, string[] xPaths)
        {
            return UnitTestsUtil.VerifyXPaths(resultStream, responseFormat, xPaths);
        }

        private void VerifySingleNodePresent(XmlDocument document, string xpath, bool expected)
        {
            XmlNode node = document.SelectSingleNode(xpath, TestNamespaceManager);
            if (expected)
            {
                Assert.IsNotNull(node, "Expecting to find node at [" + xpath + "]");
            }
            else
            {
                Assert.IsNull(node, "Not expecting to find node at [" + xpath + "]");
            }
        }

        /// <summary>Ensures that links in Atom resolve to the expected resource.</summary>
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod]
        public void AtomSerializerLinks()
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("WebServerLocation", new object[] { WebServerLocation.InProcess, WebServerLocation.InProcessWcf }));
            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
            {
                WebServerLocation location = (WebServerLocation)values["WebServerLocation"];
                using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/Customers";
                    request.Accept = "application/atom+xml,application/xml";
                    request.SendRequest();

                    XmlDocument document = request.GetResponseStreamAsXmlDocument();
                    TestUtil.TraceXml(document);

                    Uri serviceBaseUri = new Uri(request.BaseUri, UriKind.RelativeOrAbsolute);

                    XmlElement element;
                    bool rooted = location != WebServerLocation.InProcess;

                    element = TestUtil.AssertSelectSingleElement(document, "/atom:feed/atom:id");
                    Uri feedId = CheckAbsoluteUri(serviceBaseUri, GetBaseUri(element), element.InnerText);

                    element = TestUtil.AssertSelectSingleElement(document, "/atom:feed/atom:link[@rel='self']");
                    Uri feedUri = CheckRelativeUri(serviceBaseUri, GetBaseUri(element), element.GetAttribute("href"));
                    Assert.AreEqual(feedId, feedUri, "URI in feed ID matches the URI in the self-link href.");

                    element = TestUtil.AssertSelectSingleElement(document, "/atom:feed/atom:entry[1]/atom:id");
                    Uri entryId = CheckAbsoluteUri(serviceBaseUri, GetBaseUri(element), element.InnerText);

                    element = TestUtil.AssertSelectSingleElement(document, "/atom:feed/atom:entry[1]/atom:link[@rel='edit']");
                    Uri entryEditHref = CheckRelativeUri(serviceBaseUri, GetBaseUri(element), element.GetAttribute("href"));
                    Assert.AreEqual(entryId, entryEditHref, "URI in entry ID matches the URI in the edit href.");

                    element = TestUtil.AssertSelectSingleElement(document, "/atom:feed/atom:entry[1]/atom:link[@rel='http://docs.oasis-open.org/odata/ns/related/Orders']");
                    CheckRelativeUri(serviceBaseUri, GetBaseUri(element), element.GetAttribute("href"));
                    CheckRelativeUri(entryEditHref, GetBaseUri(element), element.GetAttribute("href"));

                    element = TestUtil.AssertSelectSingleElement(document, "/atom:feed/atom:entry[1]/atom:link[@rel='http://docs.oasis-open.org/odata/ns/related/BestFriend']");
                    CheckRelativeUri(serviceBaseUri, GetBaseUri(element), element.GetAttribute("href"));
                    CheckRelativeUri(entryEditHref, GetBaseUri(element), element.GetAttribute("href"));
                }
            });
        }

        /// <summary>Ensures spaces are encoded as %20 rather than + in URIs.</summary>
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod]
        public void AtomSerializerUriSpaces()
        {
            string modelText = "ET1 = entitytype { Name string key; }; ES1: ET1;";
            string populationModel = "ES1 : [ { Name : '  ' } ];";
            AdHocModel model = AdHocModel.ModelFromText(modelText);
            ModuleBuilder builder = TestUtil.CreateModuleBuilder("AtomSerializerUriSpaces");
            Type contextType = model.GeneratePocoModel(builder, populationModel).CreateType();

            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = contextType;
                request.RequestUriString = "/ES1";
                request.Accept = "application/atom+xml,application/xml";
                request.SendRequest();
                
                using(StreamReader reader = new StreamReader(request.GetResponseStream()))
                {
                    string response = reader.ReadToEnd();
                    AstoriaTestLog.IsFalse(response.Contains("'+"), "No ''+' strings in payload.");
                    AstoriaTestLog.IsTrue(response.Contains("%20"), "'%20' strings in payload.");
                }
            }
        }

        /// <summary>Ensures commas between values aren't encoded in URIs.</summary>
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod]
        public void AtomSerializerUriCommas()
        {
            string modelText = "ET1 = entitytype { Name string key; OtherName string key; }; ES1: ET1;";
            string populationModel = "ES1 : [ { Name : '  ', OtherName : 'foo' } ];";
            AdHocModel model = AdHocModel.ModelFromText(modelText);
            ModuleBuilder builder = TestUtil.CreateModuleBuilder("AtomSerializerUriSpaces");
            Type contextType = model.GeneratePocoModel(builder, populationModel).CreateType();

            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = contextType;
                request.Accept = "application/atom+xml,application/xml";
                request.RequestUriString = "/ES1";
                request.SendRequest();
                string response = request.GetResponseStreamAsText();
                TestUtil.AssertContains(response, "Name='%20%20',OtherName='foo'");
            }
        }

        /// <summary>Verifies that all serializers return correct date values.</summary>
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod]
        public void GeneralSerializerDates()
        {
            // JSON and XML dates return different values.

            using (CustomDataContext.CreateChangeScope())
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(CustomDataContext);

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DateTime", new DateTime[] 
                    { 
                        DateTime.Now, 
                        DateTime.UtcNow, 
                        new DateTime(/* Ticks for one day */ 864000000000, DateTimeKind.Unspecified)
                    }),
                    new Dimension("SerializationFormatData", SerializationFormatData.StructuredValues));
                int id = 100;
                TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                {
                    DateTime birthday = (DateTime)values["DateTime"];
                    SerializationFormatData format = (SerializationFormatData)values["SerializationFormatData"];

                    id++;
                    var c = new CustomDataContext();
                    c.InternalCustomersList.Add(new CustomerWithBirthday() { ID = id, Birthday = birthday });
                    c.SaveChanges();

                    request.Accept = format.MimeTypes[0];
                    request.RequestUriString = "/Customers(" + id + ")";
                    request.SendRequest();

                    string dateText;
                    DateTime serializedDate;

                    XmlDocument document = format.LoadXmlDocumentFromStream(request.GetResponseStream());
                    if (format == SerializationFormatData.Atom)
                    {
                        XmlElement element = TestUtil.AssertSelectSingleElement(document, "/atom:entry/atom:content/adsm:properties/ads:Birthday");
                        dateText = element.InnerText;
                        serializedDate = XmlConvert.ToDateTime(dateText, XmlDateTimeSerializationMode.RoundtripKind);
                    }
                    else if (format == SerializationFormatData.JsonLight)
                    {
                        XmlElement element = TestUtil.AssertSelectSingleElement(document, "/Object/Birthday");
                        dateText = element.InnerText;
                        serializedDate = (DateTime)JsonValidator.StringToPrimitive(dateText, typeof(DateTime));
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }

                    Trace.WriteLine("Original DateTime:   [" + birthday.ToString("yyyy-MM-dd HH:mm:ss:fffffff ZZZ K") + "] - kind is " + birthday.Kind);
                    Trace.WriteLine("DateTime Text:       [" + dateText + "]");
                    Trace.WriteLine("Serialized DateTime: [" + serializedDate.ToString("yyyy-MM-dd HH:mm:ss:fffffff ZZZ K") + "]");
                    TestUtil.TraceXml(document);

                    // For JSON, DateTime should be accurate to the millisecond.
                    if (format == SerializationFormatData.JsonLight)
                    {
                        Trace.WriteLine("For JSON, DateTime should be accurate to the millisecond only.");
                        const string ComparisonFormat = "yyyy-MM-dd HH:mm:ss:fff";
                        AstoriaTestLog.AreEqual(birthday.ToString(ComparisonFormat), serializedDate.ToString(ComparisonFormat));
                    }
                    else
                    {
                        AstoriaTestLog.AreEqual(birthday, serializedDate);
                    }
                });
            }
        }

        private static readonly char[] CharsBadRequest = new char[] { ':', '.', '/' };

        /// <summary>
        /// Gets the base uri defined in the document that is in scope
        /// for the specified element.
        /// </summary>
        /// <param name="element">Element for which to return the xml:base uri in scope.</param>
        /// <returns>The xml:base uri in scope for the element; null if none is defined.</returns>
        /// <remarks>Note that the BaseURI on XmlElement is a different thing altogether.</remarks>
        private static string GetBaseUri(XmlElement element)
        {
            return UnitTestsUtil.GetBaseUri(element);
        }

        /// <summary>Verifies that cycles can be detected correctly.</summary>
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod]
        public void Web3SSerializerCycleDetectionTest()
        {
            EventHandler handler = new EventHandler((sender, e) =>
                {
                    var context = (TypedCustomDataContext<TypedEntity<int, CircularReferenceType>>)sender;
                    var theValue = new TypedEntity<int, CircularReferenceType>();

                    theValue.ID = 1;
                    theValue.Member = new CircularReferenceType();
                    theValue.Member.Data = 1;
                    theValue.Member.OtherInstance = new CircularReferenceType();
                    theValue.Member.OtherInstance.Data = 2;
                    theValue.Member.OtherInstance.OtherInstance = theValue.Member;

                    context.SetValues(new object[] { theValue });
                });

            TypedCustomDataContext<TypedEntity<int, CircularReferenceType>>.ValuesRequested += handler;
            try
            {
                string instanceName = MakeXmlName(typeof(TypedEntity<int, CircularReferenceType>));
                string fullInstanceName = MakeXmlName(typeof(TypedEntity<int, CircularReferenceType>), true /*requiresNamespace*/);
                UnitTestsUtil.VerifyInvalidRequest(null, "/Values", typeof(TypedCustomDataContext<TypedEntity<int, CircularReferenceType>>), UnitTestsUtil.JsonLightMimeType, "GET", 500);
                UnitTestsUtil.VerifyInvalidRequest(null, "/Values", typeof(TypedCustomDataContext<TypedEntity<int, CircularReferenceType>>), UnitTestsUtil.AtomFormat, "GET", 500);
            }
            finally
            {
                TypedCustomDataContext<TypedEntity<int, CircularReferenceType>>.ValuesRequested -= handler;
            }
        }

        /// <summary>
        /// Verifies that all interesting primitive types round-trip when going
        /// through the XML serializer.
        /// </summary>
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod]
        public void Web3SSerializerBasicTypesTest()
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("TypeData", TypeData.Values),
                new Dimension("ResponseFormat", ResponseFormats));

            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
            {
                TypeData typeData = (TypeData)values["TypeData"];
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    string responseFormat = (string)values["ResponseFormat"];
                    Type valueType = typeData.ClrType;
                    Type entityType = typeof(TypedEntity<,>).MakeGenericType(typeof(int), valueType);

                    CustomDataContextSetup dataContextSetup = new CustomDataContextSetup(entityType);

                    for (int i = 0; i < typeData.SampleValues.Length; i++)
                    {
                        dataContextSetup.Id = i;
                        dataContextSetup.MemberValue = typeData.SampleValues[i];

                        request.Accept = responseFormat;
                        request.DataServiceType = dataContextSetup.DataServiceType;
                        request.RequestUriString = "/Values";
                        try
                        {
                            request.SendRequest();
                            if (!typeData.IsTypeSupported)
                            {
                                Assert.Fail("Metadata loading should have failed for unsupported types: '{0}'", typeData.ClrType.FullName);
                            }
                        }
                        catch (AssertFailedException)
                        {
                            throw;
                        }
                        catch (Exception)
                        {
                            if (typeData.IsTypeSupported)
                            {
                                Assert.Fail("Metadata loading should not have failed, since this type is supported: '{0}'", typeData.ClrType.FullName);
                            }
                            else
                            {
                                return;
                            }
                        }

                        Stream resultStream = request.GetResponseStream();

                        string xmlElementName = MakeXmlName(entityType);
                        string typeName = TestUtil.GetTypeName(entityType, true /*requiresNamespace*/);

                        string[] web3sXPaths = new string[] {
                            "/tcdc:Values/tcdc:" + xmlElementName};

                        string[] jsonXPaths = new string[] {
                            String.Format("/Object/value/{0}/{1}/odata.type[text()='#{2}']", JsonValidator.ArrayString, JsonValidator.ObjectString, typeName)
                        };

                        string[] atomXPaths = new string[] {
                            "/atom:feed/atom:entry/atom:category[@term='#" + typeName + "']"
                        };

                        XmlDocument document = VerifyXPaths(resultStream, responseFormat, web3sXPaths, jsonXPaths, atomXPaths);
                        string memberXPath = null;

                        if (String.Equals(responseFormat, UnitTestsUtil.JsonLightMimeType, StringComparison.OrdinalIgnoreCase))
                        {
                            // Make sure that static properties are not serialized.
                            VerifySingleNodePresent(document, String.Format("/Object/value/{0}/{1}/StaticProperty", JsonValidator.ArrayString, JsonValidator.ObjectString), false);
                            memberXPath = String.Format("Object/value/{0}/{1}/Member", JsonValidator.ArrayString, JsonValidator.ObjectString);
                        }
                        else if (String.Equals(responseFormat, AtomFormat, StringComparison.OrdinalIgnoreCase))
                        {
                            // Make sure that static properties are not serialized.
                            VerifySingleNodePresent(document, "/atom:feed/atom:entry/atom:content/adsm:properties/ads:StaticProperty", false);
                            memberXPath = "/atom:feed/atom:entry/atom:content/adsm:properties/ads:Member";
                        }
                        else
                        {
                            Assert.Fail("invalid format");
                        }

                        // Make sure the member value has round-tripped.
                        XmlElement memberNode = (XmlElement)document.SelectSingleNode(memberXPath, TestNamespaceManager);
                        Assert.IsNotNull(memberNode, "Because " + typeData + " is supported, XPath evaluates to something: " + memberXPath);

                        object serializedValue;
                        if (HasElementNullValue(memberNode, responseFormat))
                        {
                            serializedValue = null;
                        }
                        else
                        {
                            serializedValue = typeData.ValueFromXmlText(memberNode.InnerText, false, responseFormat);
                        }

                        typeData.VerifyAreEqual(dataContextSetup.MemberValue, serializedValue, responseFormat);
                    }

                    dataContextSetup.Cleanup();
                }
            });
        }

        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod]
        public void Web3SSerializerInheritanceTest()
        {
            VerifyPayload("/Customers", typeof(CustomDataContext), null,
                new string[] {
                    "/cdc:Customers",
                    "/cdc:Customers/cdc:Customer",
                    "/cdc:Customers/cdc:Customer[@dw:TypeName='AstoriaUnitTests.Stubs.CustomerWithBirthday']",
                    "/cdc:Customers/cdc:Customer[0 = count(@dw:TypeName)]" },
                new string[] { JsonValidator.GetJsonTypeXPath(typeof(CustomerWithBirthday), true) },
                new string[0]);
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod]
        public void Web3SSerializerOtherNamespaceTest()
        {
            VerifyPayload("/Regions", typeof(CustomDataContext), null,
                new string[] {
                    "/cdc:Regions",
                    "/cdc:Regions/cdc:Region",
                    "/cdc:Regions/cdc:Region[@dw:TypeName='AstoriaUnitTests.StubsOtherNs.Region']" },
                new string[] { JsonValidator.GetJsonTypeXPath(typeof(Region), true /*isArray*/) },
                new string[0]);
        }

        private static bool HasElementEmptyValue(XmlElement element)
        {
            Debug.Assert(element != null, "element != null");
            if (!element.HasChildNodes)
            {
                return false;
            }

            XmlNode child = element.FirstChild;
            if (child.NodeType != XmlNodeType.Element)
            {
                return false;
            }

            return child.LocalName == "EmptyStream";
        }

        private static bool HasElementNullValue(XmlElement element, string responseFormat)
        {
            return UnitTestsUtil.HasElementNullValue(element, responseFormat);
        }

        private static Uri CheckAbsoluteUri(Uri serviceBaseUri, string baseUriText, string absoluteText)
        {
            if (serviceBaseUri.IsAbsoluteUri)
            {
                return CheckRelativeUri(serviceBaseUri, baseUriText, absoluteText);
            }
            else
            {
                Assert.IsTrue(
                    absoluteText.StartsWith("/"), 
                    "Absolute URI(" + absoluteText + ") with no host should always start with '/'.");
                return new Uri(absoluteText, UriKind.Relative);
            }
        }

        private static Uri CheckRelativeUri(Uri serviceBaseUri, string baseUriText, string relativeText)
        {
            Assert.IsFalse(relativeText.StartsWith("/"), "Relative parts should never start with '/'.");
            Uri resultUri;

            if (serviceBaseUri.IsAbsoluteUri)
            {
                resultUri = CreateUri(baseUriText, relativeText);
                TestUtil.AssertIsBaseOf(serviceBaseUri, resultUri);
                return resultUri;
            }
            else
            {
                return new Uri(baseUriText + relativeText, UriKind.RelativeOrAbsolute);
            }
        }

        private static Uri CreateUri(string baseUriOrNull, string relativePart)
        {
            if (String.IsNullOrEmpty(relativePart))
            {
                throw new ArgumentException("relativePart cannot be null or empty.", "relativePart");
            }

            Uri relativePartUri = new Uri(relativePart, UriKind.RelativeOrAbsolute);
            if (String.IsNullOrEmpty(baseUriOrNull))
            {
                return relativePartUri;
            }
            else
            {
                return new Uri(new Uri(baseUriOrNull, UriKind.RelativeOrAbsolute), relativePartUri);
            }
        }

        private static string MakeXmlName(Type type)
        {
            return MakeXmlName(type, false);
        }

        private static string MakeXmlName(Type type, bool requiresNamespace)
        {
            string typeName = type.Name;

            if (XmlReader.IsName(typeName))
            {
                if (requiresNamespace)
                {
                    return type.Namespace + "." + typeName;
                }
                else
                {
                    return typeName;
                }
            }
            else
            {
                string result = XmlConvert.EncodeName(typeName);
                if (requiresNamespace)
                {
                    result = XmlConvert.EncodeName(type.Namespace + ".") + result;
                }

                if (type.IsGenericType)
                {
                    bool isFirst = true;
                    result += XmlConvert.EncodeName("[");
                    foreach (Type argumentType in type.GetGenericArguments())
                    {
                        if (isFirst)
                        {
                            isFirst = false;
                        }
                        else
                        {
                            result += XmlConvert.EncodeName(" ");
                        }

                        result += MakeXmlName(argumentType, true);
                    }

                    result += XmlConvert.EncodeName("]");
                }

                return result;
            }
        }

        /// <summary>
        /// Verifies that both valid and invalid container names can be resolved correctly.
        /// </summary>
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod]
        public void Web3SSerializerServicesTest()
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("ResponseFormat", ResponseFormats),
                new Dimension("ServiceModelData", ServiceModelData.Values),
                new Dimension("WebServerLocation", new object[] { WebServerLocation.InProcess, WebServerLocation.InProcessWcf })
                );

            TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    WebServerLocation webServerLocation = (WebServerLocation)values["WebServerLocation"];
                    ServiceModelData serviceModelData = (ServiceModelData)values["ServiceModelData"];
                    string responseFormat = (string)values["ResponseFormat"];

                    if (serviceModelData.ServiceModelType == typeof(AstoriaUnitTests.Stubs.Sql.SqlNorthwindDataContext))
                    {
                        return;
                    }

                    using (TestWebRequest request = TestWebRequest.CreateForLocation(webServerLocation))
                    {
                        request.DataServiceType = serviceModelData.ServiceModelType;
                        request.Accept = responseFormat;
                        request.HttpMethod = "GET";

                        List<string> containerNames = new List<string>(serviceModelData.ContainerNames);
                        foreach (string containerName in containerNames)
                        {
                            Trace.WriteLine("Requesting " + containerName);
                            request.RequestUriString = "/" + containerName + "?$top=2";
                            request.SendRequest();

                            VerifyXPaths(request.GetResponseStream(), responseFormat, new string[0], new string[0], new string[0]);
                        }

                        // Negative testing for container names.
                        request.RequestUriString = "/CustomerNames";
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception, true);
                    }
                });
        }

        public static IQueryable CreateSingleItemQuery<TElement, TKey, TMember>(DataServiceContext context, string entitySetName, TKey key) 
            where TElement : TypedEntity<TKey, TMember>
        {
            ParameterExpression element = Expression.Parameter(typeof(TElement), "e");
            Expression<Func<TElement, bool>> predicate = Expression.Lambda<Func<TElement, bool>>(
                Expression.Equal(Expression.Property(element, "ID"), Expression.Constant(key)), element);
            return context.CreateQuery<TElement>(entitySetName).Where(predicate);
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod]
        public void CountTest()
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("IncludeCount", new bool[] { true, false }),
                new Dimension("AskBefore", new bool[] { true, false }),
                new Dimension("AskAfter", new bool[] { true, false }));
            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
            {
                bool includeCount = (bool)values["IncludeCount"];
                bool askBefore = (bool)values["AskBefore"];
                bool askAfter = (bool)values["AskAfter"];

                //using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.Accept = "application/atom+xml,application/xml";
                    request.StartService();
                    
                    DataServiceContext ctx = new DataServiceContext(request.ServiceRoot);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();
                    var q = ctx.CreateQuery<Customer>("Customers");
                    q = includeCount ? q.IncludeTotalCount() : q;

                    long beforeCount = -1;
                    long afterCount = -1;
                    long inlineCount = 0;

                    QueryOperationResponse<Customer> response = q.Execute() as QueryOperationResponse<Customer>;

                    Exception exception = TestUtil.RunCatching(() =>
                        {
                            if (askBefore) beforeCount = response.TotalCount;
                            foreach (Customer c in q)
                            {
                                inlineCount++;
                            }
                            if (askAfter) afterCount = response.TotalCount;
                        });

                    if ((askBefore || askAfter) && !includeCount)
                    {
                        Assert.IsNotNull(exception, "should throw an exception if nothing found");
                    }
                    else
                    {
                        Assert.IsNull(exception, "no exceptions thrown");
                    }

                    if (beforeCount != -1)
                    {
                        Assert.AreEqual(inlineCount, beforeCount, "Inline count matches before count");
                    }

                    if (afterCount != -1)
                    {
                        Assert.AreEqual(inlineCount, afterCount, "Inline count matches after count");
                    }

                    //CustomDataContextSetup dataContextSetup = new CustomDataContextSetup(entityType);
                    //try
                    //{
                    //    var q = ((DataServiceQuery<TypedEntity<int, int>>)dataContextSetup.CreateQuery<Customers>("Customers").Take(2)).IncludeTotalCount();
                    //    QueryOperationResponse<TypedEntity<int, int>> response = q.Execute() as QueryOperationResponse<TypedEntity<int, int>>;
                    //}
                    //finally
                    //{
                    //    dataContextSetup.Cleanup();
                    //}
                }
            });
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod]
        public void SerializerKeyTypesTest()
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("responseFormat", ResponseFormats),
                new Dimension("typeData", TypeData.Values));
            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
            {
                string responseFormat = (string)values["responseFormat"];
                TypeData typeData = (TypeData)values["typeData"];
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    Type valueType = typeData.ClrType;
                    Type entityType = typeof(TypedEntity<,>).MakeGenericType(valueType, typeof(int));
                    CustomDataContextSetup dataContextSetup = new CustomDataContextSetup(entityType);
                    try
                    {
                        for (int i = 0; i < typeData.SampleValues.Length; i++)
                        {
                            TestUtil.TraceScopeForException("Testing sample value...", delegate()
                            {
                                object propertyValue = typeData.SampleValues[i];
                                if (String.Equals(responseFormat, JsonFormat, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (typeData.ClrType == typeof(Double) && Double.IsInfinity((Double)typeData.SampleValues[i]))
                                    {
                                        propertyValue = (Double)0;
                                    }
                                    else if (typeData.ClrType == typeof(Single) && Single.IsInfinity((Single)typeData.SampleValues[i]))
                                    {
                                        propertyValue = (Single)0;
                                    }
                                }

                                dataContextSetup.Id = propertyValue;
                                dataContextSetup.MemberValue = i;

                                // If the type is not a supported key type or if the value is null, then skip this case
                                // Ignore the long string for json since test randomly fails with out of memory since we are trying
                                // to allocate 12 MB of string through interop
                                if (!typeData.IsTypeSupportedAsKey || dataContextSetup.Id == null ||
                                    (String.Equals(responseFormat, JsonFormat, StringComparison.OrdinalIgnoreCase) && dataContextSetup.Id.GetType() == typeof(string) && ((string)dataContextSetup.Id).Length > 1024 * 1024))
                                {
                                    return;
                                }

                                request.DataServiceType = dataContextSetup.DataServiceType;
                                request.Accept = responseFormat;
                                request.RequestUriString = "/Values";

                                string instanceName = MakeXmlName(entityType);

                                string[] jsonXPaths = new string[]
                                {
                                    String.Format("/Object/value/{0}", JsonValidator.ArrayString),
                                    String.Format("/Object/value//{0}/{1}", JsonValidator.ArrayString, JsonValidator.ObjectString),
                                    String.Format("/Object/value//{0}/{1}/ID", JsonValidator.ArrayString, JsonValidator.ObjectString)
                                };

                                string[] atomXPaths = new string[]
                                {
                                    "/atom:feed",
                                    "/atom:feed/atom:entry",
                                    "/atom:feed/atom:entry/atom:content/adsm:properties/ads:ID"
                                };

                                string linkPath = null;
                                string idXPath = null;
                                if (String.Equals(responseFormat, JsonFormat, StringComparison.OrdinalIgnoreCase))
                                {
                                    idXPath = jsonXPaths[2];
                                    linkPath = "Object/value/Array/Object/url";
                                }
                                else if (String.Equals(responseFormat, AtomFormat, StringComparison.OrdinalIgnoreCase))
                                {
                                    idXPath = atomXPaths[2];
                                    linkPath = "/atom:feed/atom:entry/atom:id";
                                }
                                else
                                {
                                    Assert.Fail("Unsupported mime type");
                                }

                                request.SendRequest();

                                Stream resultStream = request.GetResponseStream();
                                XmlDocument document = VerifyXPaths(resultStream, responseFormat, null, jsonXPaths, atomXPaths);
                                XmlElement idElement = TestUtil.AssertSelectSingleElement(document, idXPath);
                                string serializedValue = idElement.InnerText;

                                object idValue = dataContextSetup.Id;
                                bool keySyntax = false;

                                // Single element, easy to compare.
                                typeData.VerifyAreEqual(idValue, typeData.ValueFromXmlText(serializedValue, keySyntax, responseFormat), responseFormat);

                                Uri serviceRoot = new Uri("http://host/");
                                var ctx = new DataServiceContext(serviceRoot);
                                MethodInfo createQueryMethod = typeof(Web3SSerializerTest).GetMethod("CreateSingleItemQuery");
                                createQueryMethod = createQueryMethod.MakeGenericMethod(entityType, valueType, typeof(int));
                                IQueryable queryable = (IQueryable)createQueryMethod.Invoke(null, new object[] { ctx, "Values", propertyValue });
                                string queryUri = new Uri(queryable.ToString(), UriKind.Absolute).GetComponents(UriComponents.Path, UriFormat.Unescaped);
                                string responseUri = new Uri(TestUtil.AssertSelectSingleElement(document, linkPath).InnerText, UriKind.Absolute).GetComponents(UriComponents.Path, UriFormat.Unescaped);

                                if (valueType == typeof(double))
                                {
                                    int indexOfStartValue = responseUri.IndexOf("(") + 1;
                                    int indexOfEndValue = responseUri.IndexOf(")");
                                    string valuePortionOriginal = responseUri.Substring(indexOfStartValue, indexOfEndValue - indexOfStartValue);
                                    string valuePortion = valuePortionOriginal.Replace("D", "");
                                    bool needDecimal = true;
                                    foreach (char c in valuePortion)
                                    {
                                        if (!Char.IsDigit(c))
                                        {
                                            needDecimal = false;
                                        }
                                    }

                                    if (needDecimal)
                                    {
                                        valuePortion = valuePortion + ".0";
                                    }

                                    responseUri = responseUri.Replace(valuePortionOriginal, valuePortion);
                                }

                                Assert.AreEqual(responseUri, queryUri, "The response URI and the one generated by the data service client match.");
                            });
                        }
                    }
                    finally
                    {
                        dataContextSetup.Cleanup();
                    }
                }
            });
        }

        [TestCategory("Partition2"), TestMethod]
        public void Web3SSerializerInvalidKeyTypesTest()
        {
            foreach (TypeData typeData in TypeData.Values)
            {
                Type valueType = typeData.ClrType;
                Type entityType = typeof(TypedEntity<,>).MakeGenericType(valueType, typeof(int));

                CustomDataContextSetup dataContextSetup = new CustomDataContextSetup(entityType);

                for (int i = 0; i < typeData.SampleValues.Length; i++)
                {
                    dataContextSetup.Id = typeData.SampleValues[i];
                    dataContextSetup.MemberValue = i;

                    // If the type is not a supported key type or if the value is null, then skip this case
                    // Ignore the long string for json since test randomly fails with out of memory since we are trying
                    // to allocate 12 MB of string through interop
                    if (typeData.IsTypeSupportedAsKey)
                    {
                        continue;
                    }

                    UnitTestsUtil.VerifyInvalidRequest(null, "/Values", dataContextSetup.DataServiceType, UnitTestsUtil.AtomFormat, "GET", 500);
                }
            }
        }
        
        // The following test is commented out because we cannot currently generate
        // compound keys for reflection-based tests (we detect a single property
        // named 'Id').

        ///// <summary>
        ///// Verifies that all interesting key types round-trip when going
        ///// through the XML serializer.
        ///// </summary>
        //[TestMethod]
        //public void Web3SSerializerCompoundKeyTypesTest()
        //{
        //    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
        //        new Dimension("FirstKeyTypeData", TypeData.Values),
        //        new Dimension("SecondKeyTypeData", TypeData.Values));
        //    engine.Mode = CombinatorialEngineMode.EveryElement;

        //    Hashtable samplesTable = new Hashtable();
        //    Hashtable typesTable = new Hashtable();

        //    while (engine.Next(typesTable))
        //    {
        //        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
        //        {
        //            TypeData firstKeyTypeData = (TypeData)typesTable["FirstKeyTypeData"];
        //            TypeData secondKeyTypeData = (TypeData)typesTable["SecondKeyTypeData"];

        //            Type entityType = typeof(DoubleKeyTypedEntity<,,>).MakeGenericType(
        //                firstKeyTypeData.ClrType,
        //                secondKeyTypeData.ClrType,
        //                typeof(int));

        //            CustomDataContextSetup dataContextSetup = new CustomDataContextSetup(entityType);
        //            dataContextSetup.MemberValue = 1;

        //            CombinatorialEngine samplesEngine = CombinatorialEngine.FromDimensions(
        //                new Dimension("Id", firstKeyTypeData.SampleValues),
        //                new Dimension("SecondId", secondKeyTypeData.SampleValues));
        //            samplesEngine.Mode = CombinatorialEngineMode.EveryElement;
        //            while (samplesEngine.Next(samplesTable))
        //            {
        //                dataContextSetup.Id = samplesTable["Id"];
        //                dataContextSetup.SecondId = samplesTable["SecondId"];

        //                bool isValidModel = !firstKeyTypeData.IsTypeSupported && !secondKeyTypeData.IsTypeSupported;

        //                request.DataServiceType = dataContextSetup.DataServiceType;
        //                request.RequestUriString = "/Values";
        //                try
        //                {
        //                    request.SendRequest();
        //                    if (!isValidModel)
        //                    {
        //                        Trace.WriteLine("Key types: [" + firstKeyTypeData.ClrType + "], [" + secondKeyTypeData.ClrType + "]");
        //                        Assert.Fail("Request on model should have failed, as it's not valid.");
        //                    }
        //                }
        //                catch(InvalidOperationException)
        //                {
        //                    if (!isValidModel)
        //                    {
        //                        continue;
        //                    }
        //                    throw;
        //                }

        //                Stream resultStream = request.GetResponseStream();

        //                // Ensure it's a valid XML document.
        //                XmlDocument document = new XmlDocument(TestNameTable);
        //                document.Load(resultStream);
        //            }

        //            dataContextSetup.RemoveEventHandler();
        //        }
        //    }
        //}
    }
}
