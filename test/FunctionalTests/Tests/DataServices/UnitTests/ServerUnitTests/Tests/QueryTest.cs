//---------------------------------------------------------------------
// <copyright file="QueryTest.cs" company="Microsoft">
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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.ServiceModel.Web;
    using System.Xml;
    using System.Xml.Linq;
    using AstoriaUnitTests;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.OData.Edm;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ocs = AstoriaUnitTests.ObjectContextStubs;
    using provider = Microsoft.OData.Service.Providers;
    using Microsoft.OData.Client;

    #endregion Namespaces

    [TestModule]
    public partial class UnitTestModule : AstoriaTestModule
    {
        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
        /// <summary>This is a test class for querying functionality.</summary>
        [TestClass, TestCase]
        public class QueryTest : AstoriaTestCase
        {
            /// <summary>Checks the constructor of $expand segments.</summary>
            [TestMethod, Variation]
            public void ExpandSegmentCtor()
            {
                // Null name.
                Exception exception = TestUtil.RunCatching(delegate () { new ExpandSegment(null, null); });
                TestUtil.AssertExceptionExpected(exception, true);

                // Name with null filter is OK.
                new ExpandSegment("foo", null);

                // Name with incorrect filter is not OK.
                Expression[] incorrectFilters = new Expression[]
                {
                    Expression.Constant(true),
                    Expression.Lambda<Func<bool>>(Expression.Constant(true)),
                    Expression.Lambda<Func<Order, int>>(Expression.Constant(1), Expression.Parameter(typeof(Order), "o")),
                };
                foreach (Expression f in incorrectFilters)
                {
                    exception = TestUtil.RunCatching(delegate () { new ExpandSegment("foo", f); });
                    TestUtil.AssertExceptionExpected(exception, true);
                }

                // Name with correct filter is OK.
                new ExpandSegment("foo",
                    Expression.Lambda<Func<Order, bool>>(Expression.Constant(true), Expression.Parameter(typeof(Order), "o")));
            }

            /// <summary>Check the PathHasFilter method of the ExpandSegment type.</summary>
            [TestMethod]
            public void ExpandSegmentPathHasFilter()
            {
                // Null argument.
                Exception exception = TestUtil.RunCatching(delegate () { ExpandSegment.PathHasFilter(null); });
                TestUtil.AssertExceptionExpected(exception, true);

                // Single segment with no filter.
                Assert.IsFalse(ExpandSegment.PathHasFilter(new ExpandSegment[0]));

                // Single segment with no filter.
                Assert.IsFalse(ExpandSegment.PathHasFilter(new ExpandSegment[] { new ExpandSegment("foo", null) }));

                // Single segment with filter.
                Expression<Func<Order, bool>> filter = (Order o) => true;
                Assert.IsTrue(ExpandSegment.PathHasFilter(new ExpandSegment[] { new ExpandSegment("foo", filter) }));

                // Multiple segments with filter.
                Assert.IsTrue(ExpandSegment.PathHasFilter(
                    new ExpandSegment[] { new ExpandSegment("foo", null), new ExpandSegment("foo", filter) }));
            }

            /// <summary>Checks that arguments can take whitespace before and after with no problems.</summary>
            [Ignore] // Remove Atom
            // [TestMethod]
            public void QueryTestBasicArguments()
            {
                // DEVNOTE(pqian):
                // We use a dictionary to store literals like "1" in the filter, the two "1"s are actually different instances
                // of ConstantExpression so this is OK. However, if Expression.Constant caches the first expression then
                // queries like these will fail. The place to look is RequestQueryParser.CreateLiteral()
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/Customers?$filter%20=1%20eq%201";
                    request.SendRequest();

                    // Specify the argument twice.
                    request.RequestUriString = "/Customers?$filter%20%20=true&$filter%20=1%20eq%201";
                    Exception exception = TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(exception, true);
                }
            }

            /// <summary>Checks that skipping can be invoked correctly.</summary>
            [TestMethod, Variation]
            public void QueryTestBasicSkipTop()
            {
                // Test matrix:
                // - Source of data.
                // - Number of items to skip.
                // - Whether whitespace is present.
                // - Whether order has been specified.
                // - Integer format.
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("HasOrder", new object[] { true, false }),
                    new Dimension("HasWhitespace", new object[] { true, false }),
                    new Dimension("FormatString", FormatStringData.Values),
                    new Dimension("UseTop", new object[] { true, false }),
                    new Dimension("SkipCount", new object[] { -1, 0, 1, 1000 }));
                TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable values)
                {
                    bool hasOrder = (bool)values["HasOrder"];
                    bool hasWhitespace = (bool)values["HasWhitespace"];
                    FormatStringData formatString = (FormatStringData)values["FormatString"];
                    int skipCount = (int)values["SkipCount"];
                    bool useTop = (bool)values["UseTop"];

                    if (!formatString.IsApplicableToType(typeof(Int32)))
                    {
                        return;
                    }

                    ServiceModelData model = ServiceModelData.AnyValue;
                    string keyword = (useTop) ? "top" : "skip";
                    string skipCountString = skipCount.ToString(formatString.FormatSpecifier, CultureInfo.InvariantCulture);
                    string ws = (hasWhitespace) ? " " : "";

                    using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcess))
                    {
                        request.DataServiceType = model.ServiceModelType;
                        request.RequestMaxVersion = "4.0;";

                        foreach (string containerName in model.ContainerNames)
                        {
                            var property = model.GetValidContainerProperties(containerName).First();
                            request.RequestUriString = "/" + containerName + "?$" + keyword + "=" + ws + skipCountString + ws;
                            if (hasOrder)
                            {
                                request.RequestUriString += "&$orderby=" + property.Name;
                            }

                            Trace.WriteLine("Running query: [" + request.RequestUriString + "]");
                            Exception exception = TestUtil.RunCatching(delegate () { request.SendRequest(); });
                            TestUtil.AssertExceptionExpected(exception,
                                formatString != FormatStringData.General &&
                                formatString != FormatStringData.Decimal &&
                                formatString != FormatStringData.Hexadecimal,
                                formatString == FormatStringData.Hexadecimal && skipCount != 0 && skipCount != 1,
                                hasOrder && !TypeData.IsSortable(property.Type.Definition));
                        }
                    }
                });
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void QueryTestDeepExpand()
            {
                using (TestWebRequest request = TestWebRequest.CreateForLocal())
                {
                    XmlDocument document;

                    request.Accept = UnitTestsUtil.AtomFormat;
                    request.DataServiceType = typeof(NorthwindModel.NorthwindContext);

                    // Shallow expand from top-level containers.
                    request.RequestUriString = "/Categories(1)?$expand=Products";
                    request.SendRequest();
                    document = request.GetResponseStreamAsXmlDocument();
                    TestUtil.AssertSelectNodes(document, "/atom:entry/atom:link[@title='Products']/*/atom:feed/atom:entry");

                    request.RequestUriString = "/Categories?$expand=Products&$top=3";
                    request.SendRequest();
                    document = request.GetResponseStreamAsXmlDocument();
                    TestUtil.AssertSelectNodes(document, "/atom:feed/atom:entry/atom:link[@title='Products']/*/atom:feed/atom:entry");

                    // Deep expand from top-level containers.
                    request.RequestUriString = "/Categories(1)?$expand=Products($expand=Suppliers)";
                    request.SendRequest();
                    document = request.GetResponseStreamAsXmlDocument();
                    TestUtil.AssertSelectNodes(document, "/atom:entry/atom:link[@title='Products']/*/atom:feed/atom:entry");
                    TestUtil.AssertSelectNodes(document, "/atom:entry/atom:link[@title='Products']/*/atom:feed/atom:entry/atom:link[@title='Suppliers']/*/atom:entry");

                    request.RequestUriString = "/Categories?$top=2&$expand=Products($expand=Suppliers)";
                    request.SendRequest();
                    document = request.GetResponseStreamAsXmlDocument();
                    TestUtil.AssertSelectNodes(document, "/atom:feed/atom:entry/atom:link[@title='Products']/*/atom:feed/atom:entry");
                    TestUtil.AssertSelectNodes(document, "/atom:feed/atom:entry/atom:link[@title='Products']/*/atom:feed/atom:entry/atom:link[@title='Suppliers']/*/atom:entry");

                    // Deep expand from collections.
                    request.RequestUriString = "/Categories(1)/Products(1)?$expand=Suppliers($expand=Products)";
                    request.SendRequest();
                    document = request.GetResponseStreamAsXmlDocument();
                    TestUtil.AssertSelectNodes(document, "/atom:entry/atom:link[@title='Suppliers']/*/atom:entry");
                    TestUtil.AssertSelectNodes(document, "/atom:entry/atom:link[@title='Suppliers']/*/atom:entry/atom:link[@title='Products']/*/atom:feed/atom:entry");

                    request.RequestUriString = "/Categories(1)/Products?$top=2&$expand=Suppliers($expand=Products)";
                    request.SendRequest();
                    document = request.GetResponseStreamAsXmlDocument();
                    TestUtil.AssertSelectNodesBySegment(document, "/atom:feed/atom:entry/atom:link[@title='Suppliers']/*/atom:entry");
                    TestUtil.AssertSelectNodesBySegment(document, "/atom:feed/atom:entry/atom:link[@title='Suppliers']/*/atom:entry/atom:link[@title='Products']/*/atom:feed/atom:entry");

                    request.RequestUriString = "/Categories(1)/Products?$filter=ProductID%20eq%201&$expand=Suppliers($expand=Products)";
                    request.SendRequest();
                    document = request.GetResponseStreamAsXmlDocument();
                    TestUtil.AssertSelectNodesBySegment(document, "/atom:feed/atom:entry/atom:link[@title='Suppliers']/*/atom:entry");
                    TestUtil.AssertSelectNodesBySegment(document, "/atom:feed/atom:entry/atom:link[@title='Suppliers']/*/atom:entry/atom:link[@title='Products']/*/atom:feed/atom:entry");

                    // Deep expand with order-by.
                    request.RequestUriString = "/Categories(1)/Products?$orderby=UnitPrice%20desc&$expand=Suppliers($expand=Products)";
                    request.SendRequest();
                    document = request.GetResponseStreamAsXmlDocument();
                    TestUtil.AssertSelectNodes(document, "/atom:feed/atom:entry/atom:link[@title='Suppliers']/*/atom:entry");
                    TestUtil.AssertSelectNodes(document, "/atom:feed/atom:entry/atom:link[@title='Suppliers']/*/atom:entry/atom:link[@title='Products']/*/atom:feed/atom:entry");
                    decimal lastUnitPrice = decimal.MaxValue;
                    XmlNodeList nodes = document.SelectNodes("/atom:feed/atom:entry/atom:content/ads:UnitPrice", TestUtil.TestNamespaceManager);
                    foreach (XmlNode node in nodes)
                    {
                        decimal value = decimal.Parse(node.InnerText, CultureInfo.InvariantCulture);
                        AstoriaTestLog.IsTrue(value <= lastUnitPrice, "Values are sorted in descending UnitPrice");
                        lastUnitPrice = value;
                    }
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void QueryTestBasicExpand()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("ServiceModel", ServiceModelData.Values),
                new Dimension("SerializationFormatData", SerializationFormatData.StructuredValues));
                TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable values)
                {
                    ServiceModelData model = (ServiceModelData)values["ServiceModel"];
                    SerializationFormatData format = (SerializationFormatData)values["SerializationFormatData"];

                    if (!model.IsValid || model.HasOpenTypes)
                    {
                        return;
                    }

                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.DataServiceType = model.ServiceModelType;
                        request.RequestMaxVersion = "4.0;";
                        foreach (string containerName in model.ContainerNames)
                        {
                            string uri = "/" + containerName + "?$top=5&$expand";
                            string separator = "=";
                            string typeName = model.GetContainerRootTypeName(containerName);

                            // Ignore open types
                            foreach (var property in model.GetModelProperties(typeName).OfType<IEdmNavigationProperty>())
                            {
                                uri += separator + property.Name;
                                separator = ",";
                            }

                            request.Accept = format.MimeTypes[0];
                            request.RequestUriString = uri;
                            request.SendRequest();

                            XmlDocument document = null;
                            string xpath = null;

                            if (format == SerializationFormatData.Atom)
                            {
                                document = new XmlDocument(TestUtil.TestNameTable);
                                document.Load(request.GetResponseStream());

                                xpath = "/atom:feed/atom:entry/atom:link[@rel='related' and @title='Products' and 0=count(*)]";
                            }

                            if (format == SerializationFormatData.Atom)
                            {
                                XmlNodeList list = document.SelectNodes(xpath, TestUtil.TestNamespaceManager);
                                Assert.AreEqual(0, list.Count, "There are zero immediate DeferredContent elements.");
                            }
                        }
                    }
                });
            }

            [Ignore] // TODO : Need to Fix Astoria Service
            // [TestMethod, Variation]
            public void QueryTestExpandOpenResource()
            {
                string[] requestStrings = new string[]
                {
                    "/Customers?$expand=Orders",
                    "/Customers?$expand=BestFriend"
                };

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Uris", requestStrings));

                TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable values)
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.DataServiceType = typeof(CustomRowBasedOpenTypesContext);

                        // CustomDataContextWithExpand will remove orders here.
                        request.RequestUriString = (string)values["Uris"];
                        Exception e = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(e, false);
                    }
                });
            }

            [Ignore] // TODO : Need to Fix Astoria Service
            // [TestMethod, Variation]
            public void QueryTestOpenResourceProperty()
            {
                string[] requestStrings = new string[]
                {
                    "/Customers(1)/Orders",
                    "/Customers(1)/BestFriend"
                };

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Uris", requestStrings));

                TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable values)
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.DataServiceType = typeof(CustomRowBasedOpenTypesContext);

                        // CustomDataContextWithExpand will remove orders here.
                        request.RequestUriString = (string)values["Uris"];
                        Exception e = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(e, false);
                    }
                });
            }

            [Ignore] // TODO : Need to Fix Astoria Service
            // [TestMethod, Variation]
            public void QueryTestFilterOpenResourceProperty()
            {
                string[] requestStrings = new string[]
                {
                    "/Customers?$filter=Orders/DollarAmount%20eq%201",
                    "/Customers?$filter=BestFriend/Name%20eq%20null"
                };

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Uris", requestStrings));

                TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable values)
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.DataServiceType = typeof(CustomRowBasedOpenTypesContext);
                        request.RequestUriString = (string)values["Uris"];
                        Exception e = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(e, request.RequestUriString.Contains("Orders"));
                    }
                });
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void QueryTestExpandCustom()
            {
                foreach (SerializationFormatData d in SerializationFormatData.StructuredValues)
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        string resultText;
                        request.DataServiceType = typeof(CustomDataContextWithExpand);

                        // CustomDataContextWithExpand will remove orders here.
                        request.RequestUriString = "/Customers?$expand=Orders";
                        request.SendRequest();
                        resultText = request.GetResponseStreamAsText();
                        TestUtil.AssertContainsFalse(resultText, "/Orders(0)");

                        request.ServiceType = typeof(CustomDataServiceWithExpand);
                        request.RequestUriString = "/Customers?$expand=Orders,BestFriend";

                        request.Accept = d.MimeTypes[0];

                        if (d.Name == "JsonLight")
                        {
                            request.Accept = UnitTestsUtil.JsonLightMimeTypeFullMetadata;
                        }

                        request.Accept += ",*/*";

                        request.SendRequest();
                        resultText = request.GetResponseStreamAsText();
                        TestUtil.AssertContains(resultText, "/Orders(1)");
                        TestUtil.AssertContainsFalse(resultText, "/Orders(0)");
                    }
                }
            }

            [Ignore] // TODO : Need to Fix Astoria Service
            // [TestMethod, Variation]
            public void QueryTestFilterWithCastAndTypeIsForIDSP()
            {
                string[] requestStrings = new string[]
                {
                    "/Customers?$filter=isof('AstoriaUnitTests.Stubs.CustomerWithBirthday') and cast('AstoriaUnitTests.Stubs.CustomerWithBirthday') ne null",
                    "/Customers?$filter=cast(Address, 'AstoriaUnitTests.Stubs.Address') ne null",
                    "/Customers?$filter=cast(Address, 'AstoriaUnitTests.Stubs.Address')/StreetAddress ne null",
                    "/Customers?$filter=length(substring(cast(Address, 'AstoriaUnitTests.Stubs.Address')/StreetAddress, 1)) add 5 eq 9",
                    "/Customers?$filter=isof('AstoriaUnitTests.Stubs.CustomerWithBirthday')",
                    "/Customers?$filter=isof('AstoriaUnitTests.Stubs.CustomerWithBirthday') eq isof('AstoriaUnitTests.Stubs.Customer')",
                };

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Uris", requestStrings),
                    new Dimension("ServiceType", new Type[] { typeof(ocs.CustomObjectContext), typeof(CustomDataContext), typeof(CustomRowBasedOpenTypesContext), typeof(CustomRowBasedContext) }));

                ocs.PopulateData.EntityConnection = null;
                using (System.Data.EntityClient.EntityConnection connection = ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable values)
                    {
                        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                        {
                            request.DataServiceType = (Type)values["ServiceType"];
                            request.RequestUriString = (string)values["Uris"];
                            if (request.DataServiceType == typeof(ocs.CustomObjectContext))
                            {
                                if (request.RequestUriString.Contains("Address"))
                                {
                                    return;
                                }

                                request.RequestUriString = request.RequestUriString.Replace("AstoriaUnitTests.Stubs", "AstoriaUnitTests.ObjectContextStubs.Types");
                            }

                            Exception e = TestUtil.RunCatching(request.SendRequest);
                            Assert.IsNull(e, "Not expecting exception.");
                        }
                    });
                }
            }

            [TestMethod, Variation]
            public void QueryTestExpandCustomized()
            {
                Type[] dataContextTypes = new Type[]
                {
                    typeof(CustomDataContext),
                    typeof(NorthwindModel.NorthwindContext),
                };

                ServiceModelData.Northwind.EnsureDependenciesAvailable();
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("dataContextType", dataContextTypes),
                    new Dimension("maxExpandCount", new int[] { 0, 1, -1 }),
                    new Dimension("maxExpandDepth", new int[] { 0, 1, -1 }));
                TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable values)
                {
                    TestUtil.ClearConfiguration();
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        int maxExpandCount = (int)values["maxExpandCount"];
                        int maxExpandDepth = (int)values["maxExpandDepth"];
                        Type dataContextType = (Type)values["dataContextType"];
                        string primitiveProperty = (dataContextType == typeof(CustomDataContext)) ? "Name" : "CompanyName";
                        StaticCallbackManager<InitializeServiceArgs>.ClearInvoked();
                        StaticCallbackManager<ApplyingExpansionsArgs>.ClearInvoked();
                        StaticCallbackManager<InitializeServiceArgs>.EventInvoked += (s1, a1) =>
                        {
                            a1.Config.MaxExpandCount = maxExpandCount;
                            a1.Config.MaxExpandDepth = maxExpandDepth;
                        };

                        request.ServiceType = typeof(TypedDataService<>).MakeGenericType(dataContextType);
                        request.RequestUriString = "/Customers";
                        Trace.WriteLine("Requesting: " + request.RequestUriString);
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception,
                            maxExpandCount < 0,
                            maxExpandDepth < 0);
                        if (exception != null) return;
                        string responseText = request.GetResponseStreamAsText();
                        Trace.WriteLine(responseText);
                        TestUtil.AssertContainsFalse(responseText, "Orders(");

                        request.RequestUriString = "/Customers?$expand=Orders";
                        Trace.WriteLine("Requesting: " + request.RequestUriString);
                        exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception,
                            maxExpandDepth == 0,
                            maxExpandCount == 0);
                        Trace.WriteLine(request.GetResponseStreamAsText());

                        // Assume coalescing.
                        request.RequestUriString = "/Customers?$expand=Orders,Orders";
                        Trace.WriteLine("Requesting: " + request.RequestUriString);
                        exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception,
                            maxExpandDepth == 0,
                            maxExpandCount == 0);
                        Trace.WriteLine(request.GetResponseStreamAsText());
                    }
                });
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void QueryTestExpandReflection()
            {
                ServiceModelData.Northwind.EnsureDependenciesAvailable();
                foreach (SerializationFormatData d in SerializationFormatData.StructuredValues)
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        string resultText;
                        request.ServiceType = typeof(CustomDataServiceWithoutExpand);
                        request.RequestUriString = "/Customers?$expand=Orders,BestFriend";

                        request.Accept = d.MimeTypes[0];

                        if (d.Name == "JsonLight")
                        {
                            request.Accept = UnitTestsUtil.JsonLightMimeTypeFullMetadata;
                        }
                        else
                        {
                            request.Accept = "application/atom+xml,application/xml";
                        }

                        request.Accept += ",*/*";

                        request.SendRequest();
                        resultText = request.GetResponseStreamAsText();
                        TestUtil.AssertContains(resultText, "/Orders(1)");
                        TestUtil.AssertContainsFalse(resultText, "/Orders(0)");

                        request.RequestUriString = "/Customers?$expand=Orders,BestFriend($expand=BestFriend)";

                        request.Accept = d.MimeTypes[0];

                        if (d.Name == "JsonLight")
                        {
                            request.Accept = UnitTestsUtil.JsonLightMimeTypeFullMetadata;
                        }
                        else
                        {
                            request.Accept = "application/atom+xml,application/xml";
                        }

                        request.Accept += ",*/*";

                        request.SendRequest();
                        resultText = request.GetResponseStreamAsText();
                        TestUtil.AssertContains(resultText, "/Orders(1)");
                        TestUtil.AssertContainsFalse(resultText, "/Orders(0)");

                        // Basic filtering, without $expand.
                        request.ServiceType = typeof(NorthwindServiceWithFilters);
                        request.RequestUriString = "/Customers('ALFKI')";
                        request.Accept = "application/atom+xml,application/xml";
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception, true);
                        TestUtil.AssertExceptionStatusCode(exception, 404, "Filtered customer should be 404.");

                        // ALFKI is in Germany and has orders, but we shouldn't be able to find it.
                        // select * from orders where ShipCity = 'Berlin';
                        request.RequestUriString = "/Orders?$filter=ShipCity%20eq%20'Berlin'&$expand=Customers";
                        request.SendRequest();
                        resultText = request.GetResponseStreamAsText();
                        TestUtil.AssertContains(resultText, "Orders(10702)"); // An Order referencing ALFKI ...
                        TestUtil.AssertContainsFalse(resultText, "ALFKI"); // ... but no ALFKI.
                    }
                }
            }

            [TestMethod, Variation]
            public void QueryTestExpandObjectContextMultiple()
            {
                bool[] allowedValues = new bool[] { true, false };
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                using (TestUtil.RestoreStaticValueOnDispose(typeof(NorthwindServiceWithFilters), "AllowBuchanan"))
                {
                    // EmployeeID   LastName    ReportsTo 
                    // 1            Davolio     2 
                    // 2            Fuller  
                    // 3            Leverling   2 
                    // 4            Peacock     2 
                    // 5            Buchanan    2 
                    // 6            Suyama      5 
                    // 7            King        5 
                    // 8            Callahan    2 
                    // 9            Dodsworth   5 
                    //
                    // There is a rule that Buchanan is never returned.
                    //
                    request.ServiceType = typeof(NorthwindServiceWithFilters);

                    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                        new Dimension("AllowBuchanan", allowedValues),
                        new Dimension("Atom", allowedValues));
                    TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable values)
                    {
                        bool allowed = (bool)values["AllowBuchanan"];
                        bool atom = (bool)values["Atom"];
                        request.Accept = (atom) ? "*/*" : UnitTestsUtil.JsonLightMimeType;

                        NorthwindServiceWithFilters.AllowBuchanan = allowed;
                        request.RequestUriString = "/Employees(5)";
                        AssertResourceFound(request, allowed);

                        request.RequestUriString = "/Employees(9)/Employees2";
                        AssertResourceFound(request, allowed);

                        request.RequestUriString = "/Employees(9)?$expand=Employees2";
                        // AssertResourceContains(request, "null=\"true\"", !allowed);   // null for expanded entry
                        AssertResourceContains(request, "Buchanan", allowed);   // No Buchanan

                        request.RequestUriString = "/Employees(9)?$expand=Employees2($expand=Employees2)";
                        AssertResourceContains(request, "Buchanan", allowed);   // No Buchanan
                        AssertResourceContains(request, "Fuller", allowed);     // No Fuller (shouldn't traverse Buchanan)

                        request.RequestUriString = "/Employees(1)?$expand=Employees2";
                        AssertResourceContains(request, "Fuller", true);

                        request.RequestUriString = "/Employees(9)?$expand=Employees1($expand=Employees1),Employees2($expand=Employees2)";
                        AssertResourceContains(request, "Buchanan", allowed);   // No Buchanan

                        request.RequestUriString = "/Employees(2)?$expand=Employees1";
                        AssertResourceContains(request, "Buchanan", allowed);   // No Buchanan

                        request.RequestUriString = "/Employees(2)?$expand=Employees1($expand=Employees1)";
                        AssertResourceContains(request, "King", allowed);   // No King, shouldn't traverse Buchanan
                    });
                }
            }

            public static void AssertResourceContains(TestWebRequest request, string text, bool expected)
            {
                Debug.WriteLine("Sending request for " + request.RequestUriString);
                request.SendRequest();
                string resultText = request.GetResponseStreamAsText();
                if (expected)
                {
                    TestUtil.AssertContains(resultText, text);
                }
                else
                {
                    TestUtil.AssertContainsFalse(resultText, text);
                }
            }

            public static void AssertResourceFound(TestWebRequest request, bool shouldBeFound)
            {
                Exception exception = TestUtil.RunCatching(request.SendRequest);
                TestUtil.AssertExceptionExpected(exception, !shouldBeFound);
                if (!shouldBeFound)
                {
                    TestUtil.AssertExceptionStatusCode(exception, 404, "Expecting 404 for " + request.RequestUriString);
                }
            }

            public class CustomDataContextWithExpand : CustomDataContext, IExpandProvider
            {
                public IEnumerable ApplyExpansions(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths)
                {
                    // For QueryTestExpandCustom.
                    if (expandPaths.Count == 1 && expandPaths.First().Count == 1 && expandPaths.First().First().Name == "Orders")
                    {
                        expandPaths.Clear();
                        return queryable;
                    }

                    // Do the actual expansion.
                    if (expandPaths.Count == 2)
                    {
                        return new ExpandedCustomerEnumerable(
                            from c in this.Customers
                            select new ExpandedCustomer()
                            {
                                Customer = c,
                                Orders = c.Orders.Where((o) => o.ID != 0),
                                BestFriend = (c.BestFriend != null && c.BestFriend.ID == 1) ? c.BestFriend : null,
                            }
                            );
                    }

                    throw new Exception("unexpected path for test.");
                }

                internal class ExpandedCustomerEnumerable : IEnumerable<Customer>
                {
                    private IEnumerable<ExpandedCustomer> source;

                    public ExpandedCustomerEnumerable(IEnumerable<ExpandedCustomer> source)
                    {
                        this.source = source;
                    }

                    IEnumerator IEnumerable.GetEnumerator()
                    {
                        return GetEnumerator();
                    }

                    public IEnumerator<Customer> GetEnumerator()
                    {
                        return new PropertyEnumerator() { e = this.source.GetEnumerator() };
                    }

                    internal class PropertyEnumerator : IEnumerator<Customer>, IExpandedResult
                    {
                        public IEnumerator<ExpandedCustomer> e;

                        public object ExpandedElement
                        {
                            get { return this.Current; }
                        }

                        public object GetExpandedPropertyValue(string name)
                        {
                            if (name == "Orders")
                            {
                                return this.e.Current.Orders;
                            }
                            else if (name == "BestFriend")
                            {
                                return this.e.Current.BestFriend;
                            }
                            else if (name == "$skiptoken")
                            {
                                return null;
                            }
                            else
                            {
                                throw new Exception("unexpected request for " + name);
                            }
                        }

                        public Customer Current
                        {
                            get { return e.Current.Customer; }
                        }

                        public bool MoveNext()
                        {
                            return e.MoveNext();
                        }

                        public void Reset()
                        {
                            throw new NotImplementedException();
                        }

                        #region IDisposable Members

                        public void Dispose()
                        {
                            e.Dispose();
                        }

                        #endregion

                        #region IEnumerator Members

                        object IEnumerator.Current
                        {
                            get { return Current; }
                        }

                        #endregion
                    }
                }

                internal class ExpandedCustomer
                {
                    public Customer Customer { get; set; }
                    public IEnumerable<Order> Orders { get; set; }
                    public Customer BestFriend { get; set; }
                }
            }

            public class NorthwindServiceWithFilters : OpenWebDataService<NorthwindModel.NorthwindContext>
            {
                public static bool AllowBuchanan = false;

                [QueryInterceptor("Employees")]
                public Expression<Func<NorthwindModel.Employees, bool>> EmployeeNotBuchanan()
                {
                    // Buchanan is a 'middle manager'.
                    if (AllowBuchanan)
                    {
                        return (c) => true;
                    }
                    else
                    {
                        return (c) => c.LastName != "Buchanan";
                    }
                }

                [QueryInterceptor("Customers")]
                public Expression<Func<NorthwindModel.Customers, bool>> CustomerNotAlfki()
                {
                    return (c) => c.CustomerID != "ALFKI";
                }

                [QueryInterceptor("Region")]
                public Expression<Func<NorthwindModel.Region, bool>> RegionNorthern()
                {
                    return (r) => r.RegionDescription == "Northern";
                }
            }

            public class CustomDataServiceWithoutExpand : OpenWebDataService<CustomDataContext>
            {
                [QueryInterceptor("Orders")]
                public Expression<Func<Order, bool>> FilterOrdersZeroOut()
                {
                    return (o) => o.ID != 0;
                }

                [QueryInterceptor("Orders")]
                public Expression<Func<Order, bool>> FilterOrdersOneHundredOut()
                {
                    return (o) => o.ID != 100;
                }
            }

            public class CustomDataServiceWithExpand : OpenWebDataService<CustomDataContextWithExpand>
            {
                [QueryInterceptor("Orders")]
                public Expression<Func<Order, bool>> FilterOrdersZeroOut()
                {
                    return (o) => o.ID != 0;
                }

                [QueryInterceptor("Orders")]
                public Expression<Func<Order, bool>> FilterOrdersOneHundredOut()
                {
                    return (o) => o.ID != 100;
                }
            }

            [TestMethod, Variation]
            public void QueryTestExpandDeep()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                using (OpenWebDataServiceHelper.MaxExpandDepth.Restore())
                {
                    TestUtil.ClearConfiguration();
                    OpenWebDataServiceHelper.MaxExpandDepth.Value = 8;
                    request.DataServiceType = ServiceModelData.Northwind.ServiceModelType;
                    request.RequestUriString = "/Employees?$expand=Employees2" +
                        TestUtil.StringRepeat("/Employees2", 8);
                    Exception exception = TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(exception, true);
                    TestUtil.AssertExceptionStatusCode(exception, 400, "Bad request expected for too-deep $expand.");
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void QueryTestFilterComplexType()
            {
                // Repro for Protocol: Internal Server Error using filter with a Complex Type.
                const string modelText = "Ns.CT1 = complexreftype { CP1 string; }; Ns.ET1 = entitytype { ID string key; CT Ns.CT1; }; ES1: ET1;";
                AdHocModel model = AdHocModel.ModelFromText(modelText);
                model.CreateDatabase();
                try
                {
                    Assembly assembly = model.GenerateModelsAndAssembly("QueryTestFilterComplexType", false /* isReflectionBased */);
                    Type contextType = TestUtil.LoadDerivedTypeFromAssembly(assembly, typeof(System.Data.Objects.ObjectContext));
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.DataServiceType = contextType;
                        request.Accept = "application/atom+xml,application/xml";
                        request.RequestUriString = "/ES1?$filter=CT%20ne%20null";
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception, true);
                        TestUtil.AssertExceptionStatusCode(exception, 501, "Status code for unsupported query is 501.");
                    }
                }
                finally
                {
                    model.DropDatabase();
                }
            }

            public class BoolGuidEntity
            {
                public int ID { get; set; }
                public bool NonNullBool { get; set; }
                public bool? NullBool { get; set; }
                public Guid NonNullGuid { get; set; }
                public Guid? NullGuid { get; set; }
            }

            public class BoolGuidDataContext
            {
                private static BoolGuidEntity[] _data = new BoolGuidEntity[] {
                                    new BoolGuidEntity { ID = 1, NonNullBool = true, NullBool = false, NonNullGuid = new Guid("12345678-1234-1234-1234-1334567890AB"), NullGuid = null },
                                    new BoolGuidEntity { ID = 2, NonNullBool = false, NullBool = null, NonNullGuid = new Guid("12345678-1234-1234-1234-1234567890AB"), NullGuid = new Guid("12345678-1234-1234-1234-1134567890AB") }
                                    };

                public IQueryable<BoolGuidEntity> Data
                {
                    get
                    {
                        return _data.AsQueryable();
                    }
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void QueryTestFilterWithBoolGuid()
            {
                var testData = new[] { new { Input = "/Data?$filter=NonNullBool gt false", Count = 1, Index = 1 },
                                        new { Input = "/Data?$filter=NonNullBool le false", Count = 1, Index = 2 },
                                        new { Input = "/Data?$filter=NonNullBool ge false", Count = 2, Index = 0 },
                                        new { Input = "/Data?$filter=NonNullBool lt true", Count = 1, Index = 2 },
                                        new { Input = "/Data?$filter=NonNullBool gt NullBool", Count = 2, Index = 0 },
                                        new { Input = "/Data?$filter=NullBool le true", Count = 2, Index = 0 },
                                        new { Input = "/Data?$filter=NullBool ge true", Count = 0, Index = 0 },
                                        new { Input = "/Data?$filter=NullBool eq null", Count = 1, Index = 2 },
                                        new { Input = "/Data?$filter=NonNullGuid gt 12345678-1234-1234-1234-0034567890AB", Count = 2, Index = 0 },
                                        new { Input = "/Data?$filter=NonNullGuid le 12345678-1234-1234-1234-1234567890AB", Count = 1, Index = 2 },
                                        new { Input = "/Data?$filter=NonNullGuid ge 00000000-0000-0000-0000-000000000000", Count = 2, Index = 0 },
                                        new { Input = "/Data?$filter=NonNullGuid lt 12345678-1234-1234-1234-1235567890AB", Count = 1, Index = 2 },
                                        new { Input = "/Data?$filter=NullGuid gt NonNullGuid", Count = 0, Index = 0 },
                                        new { Input = "/Data?$filter=NullGuid le NonNullGuid", Count = 2, Index = 0 },
                                        new { Input = "/Data?$filter=NullGuid ge 00000000-0000-0000-0000-000000000000", Count = 1, Index = 2 },
                                        new { Input = "/Data?$filter=NullGuid lt 00000000-0000-0000-0000-000000000000", Count = 1, Index = 1 },
                                        };

                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    foreach (var testDataValue in testData)
                    {
                        request.DataServiceType = typeof(BoolGuidDataContext);
                        request.RequestUriString = testDataValue.Input;
                        request.Accept = "application/atom+xml,application/xml";
                        request.SendRequest();
                        XmlDocument doc = request.GetResponseStreamAsXmlDocument();
                        XmlNodeList lst = doc.SelectNodes("/atom:feed/atom:entry/atom:content/adsm:properties/ads:ID", TestUtil.TestNamespaceManager);
                        Assert.AreEqual(testDataValue.Count, lst.Count);
                        if (testDataValue.Count == 1)
                        {
                            Assert.AreEqual(testDataValue.Index.ToString(), lst[0].InnerText);
                        }
                    }
                }
            }

            [TestMethod, Variation]
            public void QueryTestOrderByComparable()
            {
                // URI Parser Integration: OrderByClause API does not allow ordering by collections, while Astoria server does so long as the value is IComparable
                // Due to integration with the ODL Uri Parser and the general undefined nature of ordering by complex, navigation, or collection properties
                // we decided to stop supporting what that this test was covering, and fail instead.
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(CustomerWithOrderedAddressContext);
                    request.RequestUriString = "/Customers?$orderby=Address";
                    Exception ex = TestUtil.RunCatching(request.SendRequest);
                    Assert.IsNotNull(ex);
                    Assert.AreEqual(400, request.ResponseStatusCode);
                    Assert.AreEqual(ODataLibResourceUtil.GetString("MetadataBinder_OrderByExpressionNotSingleValue"), ex.InnerException.Message);
                }
            }

            #region Types for QueryTestOrderByComparable.

            public class ComparableAddress : IComparable
            {
                public string City { get; set; }
                public string Street { get; set; }

                public int CompareTo(object obj)
                {
                    ComparableAddress other = (ComparableAddress)obj;
                    int result = StringComparer.Ordinal.Compare(this.City, other.City);
                    if (result == 0)
                    {
                        result = StringComparer.Ordinal.Compare(this.Street, other.Street);
                    }
                    return result;
                }
            }

            public class CustomerWithOrderedAddress
            {
                public int ID { get; set; }
                public ComparableAddress Address { get; set; }
            }

            public class CustomerWithOrderedAddressContext
            {
                public IQueryable<CustomerWithOrderedAddress> Customers
                {
                    get
                    {
                        return new List<CustomerWithOrderedAddress>()
                        {
                            new CustomerWithOrderedAddress()
                            {
                                ID = 1, Address = new ComparableAddress() { City = "Boston", Street = "1st" },
                            },
                            new CustomerWithOrderedAddress()
                            {
                                ID = 2, Address = new ComparableAddress() { City = "Atlanta", Street = "1st" },
                            },
                            new CustomerWithOrderedAddress()
                            {
                                ID = 3, Address = new ComparableAddress() { City = "Boston", Street = "2nd" },
                            },
                        }.AsQueryable();
                    }
                }
            }

            #endregion Types for QueryTestOrderByComparable.

            [TestMethod, Variation]
            public void QueryTestNull()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = ServiceModelData.AnyValue.ServiceModelType;
                    Exception exception = TestUtil.RunCatching(request.SendRequest);
                    TestUtil.AssertExceptionExpected(exception, false);
                }
            }

            /// <summary>Checks that filtering can be invoked correctly.</summary>
            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void QueryTestBasicFilter()
            {
                // Very simple test at this level: simply check that we can get an entity by its ID.
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("ServiceModel", ServiceModelData.Values));
                TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable values)
                {
                    ServiceModelData model = (ServiceModelData)values["ServiceModel"];
                    if (!model.IsValid)
                    {
                        return;
                    }

                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.DataServiceType = model.ServiceModelType;

                        foreach (string containerName in model.ContainerNames)
                        {
                            string typeName = model.GetContainerRootTypeName(containerName);

                            // Get the first entity.
                            request.RequestUriString = "/" + containerName + "?$top=1";
                            request.Accept = "application/atom+xml,application/xml";
                            request.SendRequest();

                            XmlDocument document = new XmlDocument(TestUtil.TestNameTable);
                            document.Load(request.GetResponseStream());

                            Trace.WriteLine("Initial response: " + document.InnerXml);

                            XmlNode element = document.SelectSingleNode("//atom:entry[atom:category/@term='#" + typeName + "']", TestUtil.TestNamespaceManager);
                            if (element == null)
                            {
                                Trace.WriteLine("No elements in container " + containerName);
                                continue;
                            }

                            // Get the keys to the element, and build a filter that returns just this one.
                            var keyProperties = model.GetKeyProperties(typeName).ToList();
                            var keyValues = new Dictionary<IEdmProperty, string>(keyProperties.Count);
                            foreach (var property in keyProperties)
                            {
                                XmlElement propertyElement = TestUtil.AssertSelectSingleElement(
                                    element,
                                    "//atom:entry[atom:category/@term='#" + typeName + "']/atom:content/adsm:properties/ads:" + property.Name);
                                keyValues[property] = propertyElement.InnerText;
                            }

                            request.RequestUriString = "/" + containerName + "?$filter=";
                            request.Accept = "application/atom+xml,application/xml";
                            bool first = true;
                            bool useSmallCase = false;
                            foreach (KeyValuePair<IEdmProperty, string> keyValue in keyValues)
                            {
                                if (first)
                                {
                                    first = false;
                                }
                                else
                                {
                                    request.RequestUriString += " and ";
                                }

                                TypeData data = TypeData.FindForType(((IEdmPrimitiveType)keyValue.Key.Type.Definition).FullName());
                                object value = data.ValueFromXmlText(keyValue.Value, UnitTestsUtil.AtomFormat);
                                string keyValueText = TypeData.FormatForKey(value, useSmallCase, false);
                                useSmallCase = !useSmallCase;
                                request.RequestUriString += keyValue.Key.Name + " eq " + keyValueText;
                            }

                            // Re-request the entity, this time with a filter.
                            Trace.WriteLine("Requesting [" + request.RequestUriString + "]");
                            request.SendRequest();
                        }
                    }
                });
            }

            /// <summary>Checks that ordering can be invoked correctly.</summary>
            [TestMethod, Variation]
            public void QueryTestBasicOrderBy()
            {
                // Test matrix:
                //
                // - Type of data to sort.
                // - Names of properties.
                // - Kind of property (primitive, navigation-to-one, navigation-to-many).
                // - Ascending/descending, for each parameter.
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Isolated", new object[] { true, false }),
                    new Dimension("Ascending", new string[] { "asc", "desc", "", "not" }),
                    new Dimension("Whitespace", new string[] { "", " " }),
                    new Dimension("ServiceModel", new ServiceModelData[] { ServiceModelData.ValidValues[0], ServiceModelData.ValidValues[1] }));
                TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable values)
                {
                    ServiceModelData model = (ServiceModelData)values["ServiceModel"];
                    bool isolated = (bool)values["Isolated"];
                    string ascending = (string)values["Ascending"];
                    string whitespace = (string)values["Whitespace"];

                    using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcess))
                    {
                        request.DataServiceType = model.ServiceModelType;

                        foreach (string containerName in model.ContainerNames)
                        {
                            // Cut down on the number of containers tested.
                            if (containerName != "Customers" &&
                                containerName != "Customer" &&
                                containerName != "Orders" &&
                                containerName != "Products" &&
                                containerName != "Region" &&
                                containerName != "Category_Sales_for_1997" &&
                                containerName != "Order_Details")
                            {
                                Trace.WriteLine("Skipping container " + containerName);
                                continue;
                            }

                            string typeName = model.GetContainerRootTypeName(containerName);

                            if (isolated)
                            {
                                foreach (IEdmProperty property in model.GetModelProperties(typeName))
                                {
                                    if ((((IEdmSchemaType)property.DeclaringType).FullName() == typeof(NorthwindModel.Categories).FullName && property.Name == "Description") ||
                                        (((IEdmSchemaType)property.DeclaringType).FullName() == typeof(NorthwindModel.CustomerDemographics).FullName && property.Name == "CustomerDesc") ||
                                        (((IEdmSchemaType)property.DeclaringType).FullName() == typeof(NorthwindModel.Employees).FullName && property.Name == "Notes") ||
                                        (((IEdmSchemaType)property.DeclaringType).FullName() == typeof(NorthwindModel.Suppliers).FullName && property.Name == "HomePage")
                                        )
                                    {   // text, ntext, and image data types cannot be compared or sorted
                                        continue;
                                    }

                                    request.RequestUriString = "/" + containerName + "?$orderby=" + whitespace + property.Name + " " + ascending + whitespace;
                                    request.RequestUriString += "&$top=3";

                                    bool sortable = TypeData.IsSortable(property.Type.Definition);
                                    bool exceptionExpected = !sortable || ascending == "not";
                                    Trace.WriteLine("Requesting: " + request.RequestUriString);
                                    Exception exception = TestUtil.RunCatching(request.SendRequest);
                                    TestUtil.AssertExceptionExpected(exception, exceptionExpected);
                                }
                            }
                            else
                            {
                                request.RequestUriString = "/" + containerName;
                                bool first = true;
                                bool exceptionExpected = false;
                                foreach (var property in model.GetModelProperties(typeName))
                                {
                                    if (first)
                                    {
                                        request.RequestUriString += "?$orderby=";
                                        first = false;
                                    }
                                    else
                                    {
                                        request.RequestUriString += ",";
                                    }

                                    exceptionExpected = exceptionExpected || !TypeData.IsSortable(property.Type.Definition) || ascending == "not";
                                    string ascendingWhitespace = ((ascending == "") ? "" : " ");
                                    request.RequestUriString += whitespace + property.Name + ascendingWhitespace + ascending + whitespace;
                                }

                                request.RequestUriString += "&$top=3";

                                Trace.WriteLine("Requesting: " + request.RequestUriString);
                                Exception exception = TestUtil.RunCatching(request.SendRequest);
                                TestUtil.AssertExceptionExpected(exception, exceptionExpected);
                            }

                            // TODO: when support is available for 'rehydrating' graph, verify order of results.
                        }
                    }
                });
            }

            /// <summary>Verifies that targets that aren't resource sets are rejected.</summary>
            [TestMethod, Variation]
            public void QueryTestNotApplicable()
            {
                // Repro: support $filter on /ResourceSet(key) and /ComplextType URIs

                // The 'fail:' or 'pass:' prefix applies to $filter usage; everything
                // else is expected to fail.
                string[] targetUris = new string[]
                {
                    "fail:/",
                    "fail:/$metadata",
                    "fail:/Customers(1)/Name",
                    "fail:/Customers(1)/Name/$value",
                    "pass:/Customers(1)/Address",
                    "pass:/Customers(1)/BestFriend"
                };

                string[] options = new string[] { "$filter=true", "$orderby=1", "$top=1", "$skip=1" };
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("TargetUri", targetUris),
                    new Dimension("Option", options));
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable values)
                    {
                        string targetUri = (string)values["TargetUri"];
                        string option = (string)values["Option"];
                        bool expected = targetUri.StartsWith("fail:") || !option.StartsWith("$filter");
                        targetUri = targetUri.Substring("fail:".Length);
                        request.RequestUriString = targetUri + "?" + option;
                        Trace.WriteLine("Requesting <" + request.RequestUriString + ">...");
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception, expected);
                    });
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void QueryLinksUri()
            {
                var urisToVerify = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>("/Customers(1)/BestFriend/$ref",
                        new string[] { String.Format("/{0}/odata.id[text() = 'http://host/Customers(0)']", JsonValidator.ObjectString),
                                       "/adsm:ref[@id = 'http://host/Customers(0)']" }),
                    new KeyValuePair<string, string[]>("/Customers(2)/Orders(2)/$ref",
                        new string[] { String.Format("/{0}/odata.id[text() = 'http://host/Orders(2)']", JsonValidator.ObjectString),
                                       "/adsm:ref[@id = 'http://host/Orders(2)']" }),
                    new KeyValuePair<string, string[]>("/Customers(1)/Orders/$ref",
                        new string[] { String.Format("/Object/value/{0}[{1}/odata.id = 'http://host/Orders(1)' and {1}/odata.id = 'http://host/Orders(101)']", JsonValidator.ArrayString, JsonValidator.ObjectString),
                                       "/atom:feed[adsm:ref[@id = 'http://host/Orders(1)'] and adsm:ref[@id = 'http://host/Orders(101)']]" })
                };

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                   new Dimension("ResponseFormat", new string[] { UnitTestsUtil.JsonLightMimeType, UnitTestsUtil.AtomFormat }),
                   new Dimension("ContextType", new Type[]
                    {
                        typeof(CustomDataContext),
                        typeof(ocs.CustomObjectContext),
                        typeof(CustomRowBasedOpenTypesContext)
                    })
               );

                ocs.PopulateData.EntityConnection = null;
                using (System.Data.EntityClient.EntityConnection connection = ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                    {
                        string responseFormat = (string)values["ResponseFormat"];
                        Type contextType = (Type)values["ContextType"];

                        foreach (KeyValuePair<string, string[]> uriAndXPaths in urisToVerify)
                        {
                            string xPath = (responseFormat == UnitTestsUtil.JsonLightMimeType) ?
                                uriAndXPaths.Value[0] : uriAndXPaths.Value[1];

                            string uri = uriAndXPaths.Key;

                            if (contextType == typeof(CustomRowBasedOpenTypesContext))
                            {
                                uri = UnitTestsUtil.AddParenthesisForCollections(uri);
                            }

                            UnitTestsUtil.VerifyPayload(uri, contextType, responseFormat, null, new string[] { xPath });
                        }
                    });
                }
            }

            #region Row Count Query Test

            [TestMethod, Variation]
            public void QueryTestRowCountSegment()
            {
                TestUtil.ClearMetadataCache();

                ServiceModelData.Northwind.EnsureDependenciesAvailable();
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("RequestUri", new string[] {
                            "830 /Orders/$count",
                            "830 /Orders/$count?$expand=Order_Details",
                            "829 /Orders/$count?$skip=1",
                            "10 /Orders/$count?$top=10",
                            "6 /Orders/$count?$filter=ShipCity%20eq%20'Berlin'",
                            "10 /Orders/$count?$filter=ShipRegion%20eq%20null&$top=10",
                            "200 /Orders/$count?$top=200&$skip=10&$filter=ShipRegion%20eq%20null"
                        }));

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        string[] temp = ((String)values["RequestUri"]).Split(' ');
                        string requestUri = temp[1];
                        string expectedResponse = temp[0];
                        string responseFormat = UnitTestsUtil.MimeTextPlain;
                        request.DataServiceType = typeof(NorthwindModel.NorthwindContext);
                        request.Accept = responseFormat;
                        request.RequestUriString = requestUri;
                        request.SendRequest();
                        string responseText = request.GetResponseStreamAsText();
                        Assert.AreEqual(expectedResponse, responseText);
                    }
                });
            }

            [TestMethod, Variation]
            public void QueryTestRowCountSegmentMime()
            {
                TestUtil.ClearMetadataCache();

                ServiceModelData.Northwind.EnsureDependenciesAvailable();
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("RequestUri", new string[] {
                            "/Orders/$count",
                            "/Customers('ALFKI')/Orders/$count"}),
                    new Dimension("Accept", new string[] {
                            UnitTestsUtil.AtomFormat,
                            UnitTestsUtil.JsonLightMimeType,
                            UnitTestsUtil.MimeApplicationXml,
                            UnitTestsUtil.MimeTextPlain,
                            "*/*",
                            String.Empty}));

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        string accept = (String)values["Accept"];
                        request.DataServiceType = typeof(NorthwindModel.NorthwindContext);
                        request.Accept = accept;
                        request.RequestUriString = (String)values["RequestUri"];
                        Exception ex = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(ex, accept != String.Empty && accept != UnitTestsUtil.MimeTextPlain && accept != "*/*");
                        if (ex != null)
                        {
                            TestUtil.AssertExceptionStatusCode(ex, 415, "Unknown Mime Type Exception Status Code Should Be 406");
                        }
                    }
                });
            }

            [TestMethod, Variation]
            public void QueryTestRowCountSegmentFailureModes()
            {
                TestUtil.ClearMetadataCache();

                ServiceModelData.Northwind.EnsureDependenciesAvailable();
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("RequestUri", new string[] {
                            "404 /$count",                                          // $count on root
                            "404 /Customers('ALFKI')/CompanyName/$count",           // $count on primitive type
                            "404 /Customers('ALFKI')/CompanyName/$value/$count",    // $count on primitive value
                            "400 /Customers('ALFKI')/$ref/$count",                  // $count on link
                            "404 /Customers('ALFKI')/$count",                       // $count on singleton
                            "404 /Orders(10643)/Customers/$count",                  // $count on singleton
                            "404 /Orders/$count/$value"                             // segments after $count
                        }));

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        string[] input = ((String)values["RequestUri"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        string requestUri = input[1];
                        int exceptionCode = Int32.Parse(input[0]);

                        request.DataServiceType = typeof(NorthwindModel.NorthwindContext);
                        request.Accept = UnitTestsUtil.MimeTextPlain;
                        request.RequestUriString = requestUri;

                        Exception ex = TestUtil.RunCatching(request.SendRequest);
                        string res = request.GetResponseStreamAsText();
                        Assert.IsNotNull(ex);
                        TestUtil.AssertExceptionStatusCode(ex, exceptionCode, "Incorrect status code received");
                    }
                });
            }

            [TestMethod, Variation]
            public void QueryTestRowCountOpenTypes()
            {
                TestUtil.ClearMetadataCache();

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("RequestUri", new String[] {
                            "/Customers(1)/Address/$count",
                            "/Orders(1)/DollarAmount/$count"
                        }),

                    new Dimension("ContextType", new Type[]
                        {
                            typeof(CustomRowBasedOpenTypesContext)
                        }));

                ocs.PopulateData.EntityConnection = null;
                using (System.Data.EntityClient.EntityConnection connection = ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                    {
                        Type contextType = (Type)values["ContextType"];
                        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                        {
                            String requestUri = (String)values["RequestUri"];
                            request.DataServiceType = contextType;
                            request.RequestUriString = requestUri;
                            Exception ex = TestUtil.RunCatching(request.SendRequest);

                            string response = request.GetResponseStreamAsText();
                        }
                    });
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void QueryTestRowCountProviders()
            {
                TestUtil.ClearMetadataCache();

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("RequestUri", new String[] {
                        "/Customers?$count=true",
                        "/Customers/$count"}),
                    //"/Customers?$top=2&$skip=1&count=true",
                    //"/Customers/$count?$top=2&$skip=1"}),
                    new Dimension("ContextType", new Type[]
                        {
                            typeof(CustomDataContext),
                            typeof(ocs.CustomObjectContext),
                            typeof( CustomRowBasedContext),
                            typeof( CustomRowBasedOpenTypesContext)
                        }));

                ocs.PopulateData.EntityConnection = null;
                using (System.Data.EntityClient.EntityConnection connection = ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                    {
                        Type contextType = (Type)values["ContextType"];
                        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                        {
                            String requestUri = (String)values["RequestUri"];
                            request.DataServiceType = contextType;
                            request.RequestUriString = requestUri;
                            request.Accept = requestUri.Contains("/$count") ? UnitTestsUtil.MimeTextPlain : UnitTestsUtil.AtomFormat;
                            request.SendRequest();

                            var response = request.GetResponseStreamAsText();

                            if (requestUri.Contains("count="))
                            {
                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml(response);
                                TestUtil.AssertSelectNodes(doc, "//adsm:count['3'=text()]");
                            }
                            else
                            {
                                Assert.AreEqual(response, "3");
                            }
                        }
                    });
                }
            }

            // [TestMethod, Variation]
            public void QueryTestRowCountValueRequestFormat()
            {
                TestUtil.ClearMetadataCache();

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Accept", new string[] {
                            UnitTestsUtil.AtomFormat,
                            UnitTestsUtil.JsonLightMimeType,
                            UnitTestsUtil.MimeTextPlain,
                            ""
                    }));

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        string acceptValue = (String)values["Accept"];
                        request.DataServiceType = typeof(CustomDataContext);
                        request.RequestUriString = "/Customers(1)/Orders/$ref?$count=value";
                        request.Accept = acceptValue;
                        Exception ex = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(ex, acceptValue.Contains("json") || acceptValue.Contains("xml"));
                    }
                });
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void QueryTestRowCountLinkEnd()
            {
                TestUtil.ClearMetadataCache();
                ServiceModelData.Northwind.EnsureDependenciesAvailable();

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Format", new string[]
                    {
                        UnitTestsUtil.AtomFormat,
                        UnitTestsUtil.JsonLightMimeType
                    }));

                TestUtil.RunCombinatorialEngineFail(engine, values =>
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        string format = (String)values["Format"];

                        request.DataServiceType = typeof(NorthwindModel.NorthwindContext);
                        request.RequestUriString = "/Customers('ALFKI')/Orders/$ref?$count=true";
                        request.Accept = format;
                        request.SendRequest();

                        if (format == UnitTestsUtil.JsonLightMimeType)
                        {
                            XmlDocument xdoc = JsonValidator.ConvertToXmlDocument(request.GetResponseStream());
                            UnitTestsUtil.VerifyXPaths(xdoc, "Object/odata.count[text() = '6']");
                        }
                        else
                        {
                            TestUtil.AssertSelectNodes(
                                request.GetResponseStreamAsXmlDocument(),
                                "//adsm:count[text() = '6']");
                        }
                    }
                });
            }

            [TestMethod, Variation]
            public void QueryTestRowCountFailureModes()
            {
                TestUtil.ClearMetadataCache();

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("RequestUri", new string[] {
                        "/Orders/$count?$count=true",  // count with $count
                        "/$count=true",           // $count on root
                        "?$count=true",           // $count on root
                        "/Customers?$count=foo",      // Invalid $count option
                        "/Customers(1)?$count=true"    // Count on non-collection entity
                    }));

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        string requestUri = (String)values["RequestUri"];
                        request.DataServiceType = typeof(CustomDataContext);
                        request.RequestUriString = requestUri;
                        request.Accept = requestUri.Contains("/$count") ? UnitTestsUtil.MimeTextPlain : UnitTestsUtil.AtomFormat;
                        Exception ex = TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNotNull(ex);
                        Assert.IsTrue(ex.InnerException is DataServiceException);
                    }
                });
            }

            [TestMethod, Variation]
            public void QueryTestRowCountPostOp()
            {
                TestUtil.ClearMetadataCache();

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("RequestUri", new string[] {
                        "/Customers?$count=true",
                        "/Customers/$count"
                    }));

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.DataServiceType = typeof(CustomDataContext);
                        request.RequestUriString = (String)values["RequestUri"];
                        request.HttpMethod = "POST";
                        request.RequestContentType = SerializationFormatData.JsonLight.MimeTypes[0];
                        request.SetRequestStreamAsText("{ \"ID\" : 3 }");
                        Exception ex = TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNotNull(ex);
                        Assert.IsTrue(ex.InnerException is DataServiceException);
                        TestUtil.AssertExceptionStatusCode(ex.InnerException, 400, "POST to row count uri did not throw");
                    }
                });
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void QueryTestRowCountFiltered()
            {
                TestUtil.ClearMetadataCache();


                ServiceModelData.Northwind.EnsureDependenciesAvailable();
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("RequestUri", new string[] {
                        "/Orders?$filter=ShipCity%20eq%20'Berlin'&$count=true&$top=10&skip=1" ,
                        "/Orders?$filter=ShipCity%20eq%20'Berlin'&$count=true&$top=1",
                        "/Orders?$filter=ShipCity%20eq%20'Berlin'&$count=true&$top=5&$expand=Shippers,Customers",
                        "/Orders?$filter=ShipCity%20eq%20'Berlin'&$top=1"
                    }),
                new Dimension("ResponseFormat", new string[] { UnitTestsUtil.AtomFormat, UnitTestsUtil.JsonLightMimeType }));

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        string requestUri = (String)values["RequestUri"];
                        string responseFormat = (String)values["ResponseFormat"];
                        request.DataServiceType = typeof(NorthwindModel.NorthwindContext);
                        request.Accept = responseFormat;
                        request.RequestUriString = requestUri;
                        request.SendRequest();
                        string responseText = request.GetResponseStreamAsText();

                        if (requestUri.Contains("$count=true"))
                        {
                            if (responseFormat == UnitTestsUtil.JsonLightMimeType)
                            {
                                TestUtil.AssertContains(responseText, "\"@odata.count\":6");
                            }
                            else
                            {
                                TestUtil.AssertSelectNodes(
                                    XmlUtil.XmlDocumentFromString(responseText),
                                    "//adsm:count[text() = '6']");
                            }
                        }
                        else
                        {
                            TestUtil.AssertContainsFalse(responseText, "count");
                        }
                    }
                });
            }

            [TestMethod]
            public void QueryTestRowCountRootService()
            {
                TestUtil.ClearMetadataCache();

                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "?$count=true";
                    Exception ex = TestUtil.RunCatching(request.SendRequest);

                    Assert.IsNotNull(ex);
                    Assert.IsTrue(ex.InnerException is DataServiceException);
                    Assert.AreEqual(((DataServiceException)ex.InnerException).StatusCode, 400);
                    TestUtil.AssertContains(ex.InnerException.Message, DataServicesResourceUtil.GetString("RequestQueryProcessor_QueryNoOptionsApplicable"));
                }
            }

            [TestMethod]
            public void QueryTestRowCountInJson()
            {
                TestUtil.ClearMetadataCache();
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("RequestUri", new string[] {
                        "/Customers?$count=true",
                        "/Customers?$top=1&$count=true",
                        "/Customers?$filter=false&$count=true",
                    })
                );

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.DataServiceType = typeof(CustomDataContext);
                        request.RequestUriString = (string)values["RequestUri"];

                        // verify that in json light, the count element comes 2nd after odata.metadata
                        request.Accept = UnitTestsUtil.JsonLightMimeType;
                        request.SendRequest();

                        var response = request.GetResponseStreamAsXmlDocument(UnitTestsUtil.JsonLightMimeType);
                        UnitTestsUtil.VerifyXPaths(response, "/Object/odata.count[count(preceding-sibling::*)=1]");
                    }
                });
            }

            class OnStartProcessingService<T> : DataService<T>
            {
                protected override void OnStartProcessingRequest(ProcessRequestArgs args)
                {
                    if (OnStartProcessingServiceHelper.Validator != null)
                    {
                        OnStartProcessingServiceHelper.Validator(args);
                    }

                    base.OnStartProcessingRequest(args);
                }
            }

            class OnStartProcessingServiceHelper
            {
                public static Action<ProcessRequestArgs> Validator;
                public static int CurrentItemIndex;
            }

            [Ignore] // Remove Atom
            // [TestMethod]
            public void QueryTestOnStartProcessingRequest()
            {
                TestUtil.ClearMetadataCache();

                using (TestUtil.RestoreStaticMembersOnDispose(typeof(OnStartProcessingServiceHelper)))
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(OnStartProcessingService<CustomRowBasedOpenTypesContext>);
                    OnStartProcessingServiceHelper.Validator = (args) =>
                        {
                            switch (OnStartProcessingServiceHelper.CurrentItemIndex)
                            {
                                case 0:
                                    Assert.IsTrue(args.OperationContext.AbsoluteRequestUri.AbsoluteUri.Contains("$batch"));
                                    break;

                                case 1:
                                    Assert.IsTrue(args.OperationContext.AbsoluteRequestUri.AbsoluteUri.Contains("$count=true"));
                                    break;

                                case 2:
                                    Assert.IsTrue(args.OperationContext.AbsoluteRequestUri.AbsoluteUri.Contains("$count"));
                                    break;

                                default:
                                    Assert.Fail("Not expecting a 4th call.");
                                    break;
                            }

                            OnStartProcessingServiceHelper.CurrentItemIndex++;
                        };
                    request.RequestUriString = "/$batch";
                    request.RequestContentType = "multipart/mixed; boundary=batch_1954f60c-6ed2-4959-89bf-95d727a36507";
                    request.HttpMethod = "POST";
                    string requestContent = @"
--batch_1954f60c-6ed2-4959-89bf-95d727a36507
Content-Type: application/http
Content-Transfer-Encoding: binary

GET http://host/Customers?$count=true HTTP/1.1
Host: localhost
OData-Version: 2.0

--batch_1954f60c-6ed2-4959-89bf-95d727a36507
Content-Type: application/http
Content-Transfer-Encoding: binary

GET http://host/Customers/$count HTTP/1.1
Host: localhost
OData-Version: 4.0

--batch_1954f60c-6ed2-4959-89bf-95d727a36507--
";
                    request.SetRequestStreamAsText(requestContent);
                    request.SendRequest();
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod]
            public void QueryTestRowCountBatching()
            {
                TestUtil.ClearMetadataCache();

                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/$batch";
                    request.RequestContentType = "multipart/mixed; boundary=batch_1954f60c-6ed2-4959-89bf-95d727a36507";
                    request.HttpMethod = "POST";
                    string requestContent = @"
--batch_1954f60c-6ed2-4959-89bf-95d727a36507
Accept: application/atom+xml,application/xml
Content-Type: application/http
Content-Transfer-Encoding: binary

GET http://host/Customers?$count=true HTTP/1.1
Host: localhost
Accept: application/atom+xml,application/xml
OData-Version: 4.0

--batch_1954f60c-6ed2-4959-89bf-95d727a36507
Accept: application/atom+xml,application/xml
Content-Type: application/http
Content-Transfer-Encoding: binary

GET http://host/Customers/$count HTTP/1.1
Host: localhost
Accept: application/atom+xml,application/xml
OData-Version: 4.0

--batch_1954f60c-6ed2-4959-89bf-95d727a36507--
";
                    // reset to 4.0: should pass
                    request.SetRequestStreamAsText(requestContent);
                    request.SendRequest();
                    var s = request.GetResponseStreamAsText();
                    TestUtil.AssertContains(s, "count>3</");
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void QueryTestRowCountServiceOp()
            {
                TestUtil.ClearMetadataCache();

                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.ServiceType = typeof(RowCountServiceOpService);
                    request.Accept = "application/atom+xml,application/xml";
                    request.RequestUriString = "/Orders?$count=true";
                    request.SendRequest();
                    string responseText = request.GetResponseStreamAsText();
                    TestUtil.AssertSelectNodes(
                        XmlUtil.XmlDocumentFromString(responseText),
                        "//adsm:count[text() = '3']");

                    request.RequestUriString = "/SOP_EmptyQueryable?$count=true";
                    request.Accept = "application/atom+xml,application/xml";
                    request.SendRequest();
                    responseText = request.GetResponseStreamAsText();
                    TestUtil.AssertSelectNodes(
                        XmlUtil.XmlDocumentFromString(responseText),
                        "//adsm:count[text() = '0']");

                    request.RequestUriString = "/SOP_MVQueryable?$count=true";
                    request.Accept = "application/atom+xml,application/xml";
                    request.SendRequest();
                    responseText = request.GetResponseStreamAsText();
                    TestUtil.AssertSelectNodes(
                        XmlUtil.XmlDocumentFromString(responseText),
                        "//adsm:count[text() = '3']");

                    request.RequestUriString = "/SOP_SVQueryable?$count=true";
                    request.Accept = "application/atom+xml,application/xml";
                    Exception ex = TestUtil.RunCatching(request.SendRequest);
                    responseText = request.GetResponseStreamAsText();
                    Assert.IsNotNull(ex);
                    TestUtil.AssertContains(ex.InnerException.Message, DataServicesResourceUtil.GetString("RequestQueryProcessor_QuerySetOptionsNotApplicable"));

                    request.RequestUriString = "/Orders/$count";
                    request.Accept = UnitTestsUtil.MimeTextPlain;
                    request.SendRequest();
                    responseText = request.GetResponseStreamAsText();
                    Assert.AreEqual("3", responseText);

                    request.RequestUriString = "/SOP_MVQueryable/$count";
                    request.SendRequest();
                    responseText = request.GetResponseStreamAsText();
                    Assert.AreEqual("3", responseText);

                    request.RequestUriString = "/SOP_SVQueryable/$count";
                    ex = TestUtil.RunCatching(request.SendRequest);
                    Assert.IsNotNull(ex);
                    responseText = request.GetResponseStreamAsText();
                    TestUtil.AssertContains(ex.InnerException.Message, "request URI is not valid");

                    request.RequestUriString = "/SOP_EmptyQueryable/$count";
                    request.SendRequest();
                    responseText = request.GetResponseStreamAsText();
                    Assert.AreEqual("0", responseText);
                }
            }

            public class RowCountServiceOpService : DataService<CustomDataContext>
            {
                // This method is called only once to initialize service-wide policies.
                public static void InitializeService(DataServiceConfiguration config)
                {
                    config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                    config.SetEntitySetAccessRule("*", EntitySetRights.All);
                    config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                }

                [System.ServiceModel.Web.WebGet]
                public IQueryable<Customer> SOP_EmptyQueryable()
                {
                    return (new Customer[0]).AsQueryable();
                }

                [System.ServiceModel.Web.WebGet]
                public IQueryable<Customer> SOP_MVQueryable()
                {
                    return (new Customer[3]).AsQueryable();
                }

                [System.ServiceModel.Web.WebGet]
                [SingleResult]
                public IQueryable<Customer> SOP_SVQueryable()
                {
                    return (new Customer[1]).AsQueryable();
                }

                [QueryInterceptor("Orders")]
                public Expression<Func<Order, bool>> FilterOrders()
                {
                    return o => o.ID < 100;
                }
            }

            #endregion
        }

        public class Product
        {
            public int ID { get; set; }
        }

        public class Context
        {
            public IQueryable<Product> Products
            {
                get
                {
                    return (new List<Product>() { new Product() { ID = 0 } }).AsQueryable();
                }
            }
        }

        /// <summary>This is a test class for querying functionality.</summary>
        [TestClass, TestCase]
        public class ServerDrivenPaging : AstoriaTestCase
        {
            private void PageSizeCustomizer(DataServiceConfiguration config, Type modelType)
            {
                if (modelType == typeof(NorthwindModel.NorthwindContext) || modelType == typeof(AstoriaUnitTests.Stubs.Sql.SqlNorthwindDataContext))
                {
                    config.SetEntitySetPageSize("*", 2);
                    config.SetEntitySetPageSize("Customers", 10);
                }
                else if (modelType == typeof(DataTypesContext.EdmDataTypesContext) || modelType == typeof(DataTypesContext.SqlDataTypesContext))
                {
                    config.SetEntitySetPageSize("*", 1);
                }
                else
                {
                    config.SetEntitySetPageSize("Customers", 2);
                    config.SetEntitySetPageSize("Orders", 1);
                }
            }

            private void NoPageSizeCustomizer(DataServiceConfiguration config, Type modelType)
            {
            }

            [TestMethod, Variation]
            public void ShouldReturn400WhenEntitySetHasNoPagingLimitSet()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(Context);

                    request.RequestUriString = "/Products?$skiptoken=0";
                    Exception ex = TestUtil.RunCatching(request.SendRequest);
                    Assert.AreEqual(typeof(DataServiceException), ex.InnerException.GetType());
                    DataServiceException dse = ex.InnerException as DataServiceException;
                    Assert.AreEqual(400, dse.StatusCode);
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void BasicCustomAndOpenTypes()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = this.PageSizeCustomizer;
                    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                        new Dimension("ServiceModel", new ServiceModelData[] {
                                                                ServiceModelData.CustomData,
                                                                ServiceModelData.CustomRowBased,
                                                                ServiceModelData.CustomRowBasedOpenType
                                                                }),
                        new Dimension("SerializationFormatData", SerializationFormatData.StructuredValues));

                    TestUtil.RunCombinatorialEngineFail(
                        engine,
                        delegate (Hashtable values)
                        {
                            ServiceModelData model = (ServiceModelData)values["ServiceModel"];
                            SerializationFormatData format = (SerializationFormatData)values["SerializationFormatData"];

                            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                            {
                                request.DataServiceType = model.ServiceModelType;

                                var querycountpairs = new[] { new { query = "/Customers", count = 1, result = "http://host/Customers?$skiptoken=1", idcount = 2 },
                                                              new { query = "/Customers?$orderby=Address/City,ID asc", count = 1 , result = "http://host/Customers?$orderby=Address/City,ID%20asc&$skiptoken='Redmond',1" , idcount = 2},
                                                              new { query = "/Orders", count = 1 , result = "http://host/Orders?$skiptoken=0" , idcount = 1}
                                                            };

                                foreach (var querycount in querycountpairs)
                                {
                                    string uri = querycount.query;
                                    request.Accept = format.MimeTypes[0];
                                    request.RequestUriString = uri;
                                    request.SendRequest();

                                    XmlDocument document;
                                    string xpath;
                                    if (format == SerializationFormatData.JsonLight)
                                    {
                                        // No validation for JSON light
                                        return;
                                    }
                                    else
                                    {
                                        Assert.IsTrue(format == SerializationFormatData.Atom, "expecting Atom");
                                        document = new XmlDocument(TestUtil.TestNameTable);
                                        document.Load(request.GetResponseStream());

                                        xpath = "/atom:feed/atom:link[@rel='next']";
                                    }

                                    XmlNodeList list = document.SelectNodes(xpath, TestUtil.TestNamespaceManager);
                                    Assert.AreEqual(querycount.count, list.Count, "Expected number of next elements did not match the actual number of next elements");

                                    // Verify that the next link is correct
                                    Assert.AreEqual(list[0].Attributes["href"].Value, querycount.result);

                                    // Verify count
                                    list = document.SelectNodes("/atom:feed/atom:entry", TestUtil.TestNamespaceManager);
                                    Assert.AreEqual(list.Count, querycount.idcount);
                                }
                            }
                        });
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void ExpandEFAndLinq2Sql()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = this.PageSizeCustomizer;

                    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                        new Dimension("ServiceModel", new ServiceModelData[] {
                            ServiceModelData.Northwind }),
                        // Linq to SQL disabled due to problems with null checks and orderbys
                        //, ServiceModelData.SqlNorthwindData }),
                        new Dimension("SerializationFormatData", SerializationFormatData.StructuredValues));

                    TestUtil.RunCombinatorialEngineFail(
                        engine,
                        delegate (Hashtable values)
                        {
                            ServiceModelData model = (ServiceModelData)values["ServiceModel"];
                            SerializationFormatData format = (SerializationFormatData)values["SerializationFormatData"];

                            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                            {
                                request.DataServiceType = model.ServiceModelType;

                                var querycountpairs = new[] { new { query = "/Employees?$expand=Orders($expand=Customers),Territories&$orderby=EmployeeID add 1", count = 5 },
                                                              new { query = "/Employees(1)?$expand=Orders($expand=Customers),Territories", count = 2 },
                                                              new { query = "/Employees?$expand=Orders($expand=Customers),Territories&$orderby=EmployeeID add 1", count = 5 },
                                                              new { query = "/Customers?$expand=Orders,CustomerDemographics&$orderby=substring(CompanyName, 1)", count = 11 },
                                                              new { query = "/Customers?$expand=Orders,CustomerDemographics&$orderby=substring(CompanyName, 1), CustomerID asc&$skiptoken='a maison d''Asie','LAMAI'", count = 10 },
                                                              new { query = "/Customers?$expand=Orders&$top=10", count = 10 }, // 10 orders and no customer 
                                                              new { query = "/Customers?$expand=Orders&$top=11", count = 11 }, // 10 orders and 1 customer
                                                         };
                                int i = 0;
                                foreach (var querycount in querycountpairs)
                                {
                                    string uri = querycount.query;

                                    if (model.ServiceModelType == typeof(AstoriaUnitTests.Stubs.Sql.SqlNorthwindDataContext))
                                    {
                                        if (i == 0 || i == 1 || i == 2)
                                        {
                                            uri = uri.Replace("Territories", "EmployeeTerritories");
                                        }
                                        else
                                            if (i == 4 || i == 5)
                                        {
                                            uri = uri.Replace("CustomerDemographics", "CustomerCustomerDemo");
                                        }
                                    }

                                    request.Accept = format.MimeTypes[0];
                                    request.RequestUriString = uri;
                                    request.SendRequest();

                                    XmlDocument document;
                                    string xpath;
                                    if (format == SerializationFormatData.JsonLight)
                                    {
                                        // No validation for JSON light
                                        return;
                                    }
                                    else
                                    {
                                        Assert.IsTrue(format == SerializationFormatData.Atom, "expecting Atom");
                                        document = new XmlDocument(TestUtil.TestNameTable);
                                        document.Load(request.GetResponseStream());

                                        xpath = "descendant::atom:link[@rel='next']";
                                    }

                                    XmlNodeList list = document.SelectNodes(xpath, TestUtil.TestNamespaceManager);
                                    Assert.AreEqual(querycount.count, list.Count, "Expected number of next elements did not match the actual number of next elements");
                                    i++;
                                }
                            }
                        });
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void ExpandCustomAndOpenTypes()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = this.PageSizeCustomizer;
                    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                        new Dimension("ServiceModel", new ServiceModelData[] {
                                                                ServiceModelData.CustomData,
                                                                ServiceModelData.CustomRowBased,
                                                                ServiceModelData.CustomRowBasedOpenType,
                                                                }),
                        new Dimension("SerializationFormatData", SerializationFormatData.StructuredValues));

                    TestUtil.RunCombinatorialEngineFail(
                        engine,
                        delegate (Hashtable values)
                        {
                            ServiceModelData model = (ServiceModelData)values["ServiceModel"];
                            SerializationFormatData format = (SerializationFormatData)values["SerializationFormatData"];

                            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                            {
                                request.DataServiceType = model.ServiceModelType;

                                var querycountpairs = new[] {
                                                              new { query = "/Customers?$expand=Orders&$orderby=nonexistent", count = 3 },
                                                              new { query = "/Customers?$expand=Orders/OrderDetails&$orderby=ID add 1", count = 3 },
                                                              new { query = "/Orders?$expand=OrderDetails&$orderby=DollarAmount", count = 1 }
                                                            };
                                int i = 0;
                                foreach (var querycount in querycountpairs)
                                {
                                    string uri = querycount.query;
                                    request.Accept = format.MimeTypes[0];

                                    if (i == 1)
                                    {
                                        uri = uri.Replace("/OrderDetails", "");
                                    }
                                    else
                                        if (i == 2)
                                    {
                                        if (model.ServiceModelType == typeof(CustomRowBasedOpenTypesContext))
                                        {
                                            i++;
                                            continue;
                                        }
                                    }


                                    request.RequestUriString = uri;

                                    Exception e = TestUtil.RunCatching(request.SendRequest);

                                    if (i == 0)
                                    {
                                        if (model.ServiceModelType != typeof(CustomRowBasedOpenTypesContext))
                                        {
                                            Assert.IsTrue(e != null, "Expecting exception.");
                                            i++;
                                            continue;
                                        }
                                    }

                                    Assert.IsTrue(e == null, "Not expecting exception.");
                                    XmlDocument document;
                                    string xpath;
                                    if (format == SerializationFormatData.JsonLight)
                                    {
                                        // No validation for JSON light
                                        return;
                                    }
                                    else
                                    {
                                        Assert.IsTrue(format == SerializationFormatData.Atom, "expecting Atom");
                                        document = new XmlDocument(TestUtil.TestNameTable);
                                        document.Load(request.GetResponseStream());

                                        xpath = "descendant::atom:link[@rel='next']";
                                    }

                                    XmlNodeList list = document.SelectNodes(xpath, TestUtil.TestNamespaceManager);
                                    Assert.AreEqual(querycount.count, list.Count, "Expected number of next elements did not match the actual number of next elements");
                                    i++;
                                }
                            }
                        });
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void LinksEFAndLinq2Sql()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = this.PageSizeCustomizer;

                    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                        new Dimension("ServiceModel", new ServiceModelData[] { ServiceModelData.Northwind, ServiceModelData.SqlNorthwindData }),
                        new Dimension("SerializationFormatData", SerializationFormatData.StructuredValues));

                    TestUtil.RunCombinatorialEngineFail(
                        engine,
                        delegate (Hashtable values)
                        {
                            ServiceModelData model = (ServiceModelData)values["ServiceModel"];
                            SerializationFormatData format = (SerializationFormatData)values["SerializationFormatData"];

                            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                            {
                                request.DataServiceType = model.ServiceModelType;

                                var querycountpairs = new[] { new { query = "/Employees(1)/Orders/$ref", count = 1, result = "http://host/Employees(1)/Orders/$ref?$skiptoken=10270" },
                                                              new { query = "/Employees(1)/Orders/$ref?$orderby=OrderID add 1,OrderID asc", count = 1, result = "http://host/Employees(1)/Orders/$ref?$orderby=OrderID%20add%201,OrderID%20asc&$skiptoken=10271,10270" },
                                                              new { query = "/Customers('ALFKI')/Orders(10643)/Order_Details/$ref", count = 1, result = "http://host/Customers('ALFKI')/Orders(10643)/Order_Details/$ref?$skiptoken=10643,39" }
                                                        };

                                foreach (var querycount in querycountpairs)
                                {
                                    string uri = querycount.query;
                                    if (model.ServiceModelType == typeof(AstoriaUnitTests.Stubs.Sql.SqlNorthwindDataContext))
                                    {
                                        uri = uri.Replace("Order_Details", "OrderDetails");
                                    }

                                    request.Accept = format.MimeTypes[0];
                                    request.RequestUriString = uri;
                                    request.SendRequest();

                                    XmlDocument document;
                                    string xpath;
                                    if (format == SerializationFormatData.JsonLight)
                                    {
                                        // No validation for JSON light
                                        return;
                                    }
                                    else
                                    {
                                        Assert.IsTrue(format == SerializationFormatData.Atom, "expecting Atom");
                                        document = new XmlDocument(TestUtil.TestNameTable);
                                        document.Load(request.GetResponseStream());

                                        xpath = "descendant::dw:next";
                                    }

                                    XmlNodeList list = document.SelectNodes(xpath, TestUtil.TestNamespaceManager);
                                    Assert.AreEqual(querycount.count, list.Count, "Expected number of next elements did not match the actual number of next elements");
                                    Assert.AreEqual(XmlNodeType.Text, list[0].ChildNodes[0].NodeType, "Expected text node type");

                                    string expected = querycount.result;
                                    if (model.ServiceModelType == typeof(AstoriaUnitTests.Stubs.Sql.SqlNorthwindDataContext))
                                    {
                                        expected = expected.Replace("Order_Details", "OrderDetails");
                                    }
                                    Assert.AreEqual(expected, list[0].ChildNodes[0].Value, "Incorrect value for next link detected");
                                }
                            }
                        });
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void LinksCustomAndOpenTypes()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = this.PageSizeCustomizer;
                    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                        new Dimension("ServiceModel", new ServiceModelData[] {
                                                                ServiceModelData.CustomData,
                                                                ServiceModelData.CustomRowBased,
                                                                ServiceModelData.CustomRowBasedOpenType
                                                                }),
                        new Dimension("SerializationFormatData", SerializationFormatData.StructuredValues));

                    TestUtil.RunCombinatorialEngineFail(
                        engine,
                        delegate (Hashtable values)
                        {
                            ServiceModelData model = (ServiceModelData)values["ServiceModel"];
                            SerializationFormatData format = (SerializationFormatData)values["SerializationFormatData"];

                            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                            {
                                request.DataServiceType = model.ServiceModelType;

                                var querycountpairs = new[] { new { query = "/Customers(1)/Orders()/$ref", count = 1 } };

                                foreach (var querycount in querycountpairs)
                                {
                                    string uri = querycount.query;
                                    request.Accept = format.MimeTypes[0];
                                    request.RequestUriString = uri;
                                    request.SendRequest();

                                    XmlDocument document;
                                    string xpath;
                                    if (format == SerializationFormatData.JsonLight)
                                    {
                                        // No validation for JSON light
                                        return;
                                    }
                                    else
                                    {
                                        Assert.IsTrue(format == SerializationFormatData.Atom, "expecting Atom");
                                        document = new XmlDocument(TestUtil.TestNameTable);
                                        document.Load(request.GetResponseStream());

                                        xpath = "descendant::dw:next";
                                    }

                                    XmlNodeList list = document.SelectNodes(xpath, TestUtil.TestNamespaceManager);
                                    Assert.AreEqual(querycount.count, list.Count, "Expected number of next elements did not match the actual number of next elements");
                                }
                            }
                        });
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod]
            public void QueryRowCountWithSDP()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = this.PageSizeCustomizer;

                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.DataServiceType = ServiceModelData.Northwind.ServiceModelType;
                        var querycountpairs = new[] {
                            new { uri = "/Customers/$count", count = "91" },
                            new { uri = "/Customers/$count?$top=20", count = "20" },
                            new { uri = "/Customers/$count?$skip=90", count = "1" },
                            new { uri = "/Orders/$count", count="830" },
                            new { uri = "/Customers('ALFKI')/Orders/$count", count="6" },
                            new { uri = "/Customers?$count=true", count = "91" },
                            new { uri = "/Customers?$count=true&$top=20", count = "91" },
                            new { uri = "/Customers?$count=true&$skiptoken='BOTTM'", count="91" }
                        };

                        foreach (var q in querycountpairs)
                        {
                            request.RequestUriString = q.uri;
                            if (!q.uri.Contains("/$count"))
                            {
                                request.Accept = "application/atom+xml,application/xml";
                            }

                            request.SendRequest();

                            string response = request.GetResponseStreamAsText();

                            if (q.uri.Contains("/$count"))
                            {
                                Assert.AreEqual(q.count, response);
                            }
                            else
                            {
                                TestUtil.AssertSelectNodes(
                                    XmlUtil.XmlDocumentFromString(response),
                                    "//adsm:count[text() = '" + q.count + "']");
                            }
                        }
                    }
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void IgnoreMaxIntPageSize()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, t) => { config.SetEntitySetPageSize("*", Int32.MaxValue); };

                    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                        new Dimension("ServiceModel", new ServiceModelData[] { ServiceModelData.CustomRowBasedOpenType }),
                        new Dimension("SerializationFormatData", SerializationFormatData.StructuredValues));

                    TestUtil.RunCombinatorialEngineFail(
                        engine,
                        delegate (Hashtable values)
                        {
                            ServiceModelData model = (ServiceModelData)values["ServiceModel"];
                            SerializationFormatData format = (SerializationFormatData)values["SerializationFormatData"];

                            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                            {
                                request.DataServiceType = model.ServiceModelType;
                                request.Accept = format.MimeTypes[0];
                                request.RequestUriString = "/Customers?$expand=Orders";
                                request.SendRequest();

                                XmlDocument document;
                                string xpath;
                                if (format == SerializationFormatData.JsonLight)
                                {
                                    // No validation for JSON light
                                    return;
                                }
                                else
                                {
                                    Assert.IsTrue(format == SerializationFormatData.Atom, "expecting Atom");
                                    document = new XmlDocument(TestUtil.TestNameTable);
                                    document.Load(request.GetResponseStream());

                                    xpath = "/atom:feed/atom:link[@rel='next']";
                                }

                                XmlNodeList list = document.SelectNodes(xpath, TestUtil.TestNamespaceManager);
                                Assert.AreEqual(0, list.Count, "Not expecting a next link.");
                            }
                        });
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void BasicEFAndLinq2Sql()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = this.PageSizeCustomizer;

                    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                        new Dimension("ServiceModel", new ServiceModelData[] { ServiceModelData.Northwind, ServiceModelData.SqlNorthwindData }),
                        new Dimension("SerializationFormatData", SerializationFormatData.StructuredValues));

                    TestUtil.RunCombinatorialEngineFail(
                        engine,
                        delegate (Hashtable values)
                        {
                            ServiceModelData model = (ServiceModelData)values["ServiceModel"];
                            SerializationFormatData format = (SerializationFormatData)values["SerializationFormatData"];

                            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                            {
                                request.DataServiceType = model.ServiceModelType;

                                var querycountpairs = new[] { new { query = "/Customers?$top=20&$skip=5", count = 1, result = "http://host/Customers?$top=10&$skiptoken='COMMI'", idcount = 10 },
                                                              new { query = "/Customers", count = 1, result = "http://host/Customers?$skiptoken='BOTTM'", idcount = 10 },
                                                              new { query = "/Orders", count = 1, result = "http://host/Orders?$skiptoken=10249", idcount = 2 },
                                                              new { query = "/Orders?$orderby=OrderID add 1 desc, OrderID asc", count = 1, result = "http://host/Orders?$orderby=OrderID%20add%201%20desc,%20OrderID%20asc&$skiptoken=11077,11076", idcount = 2 },
                                                            };

                                foreach (var querycount in querycountpairs)
                                {
                                    string uri = querycount.query;
                                    request.Accept = format.MimeTypes[0];
                                    request.RequestUriString = uri;
                                    request.SendRequest();

                                    XmlDocument document;
                                    string xpath;
                                    if (format == SerializationFormatData.JsonLight)
                                    {
                                        // No validation for JSON light
                                        return;
                                    }
                                    else
                                    {
                                        Assert.IsTrue(format == SerializationFormatData.Atom, "expecting Atom");
                                        document = new XmlDocument(TestUtil.TestNameTable);
                                        document.Load(request.GetResponseStream());

                                        xpath = "/atom:feed/atom:link[@rel='next']";
                                    }

                                    XmlNodeList list = document.SelectNodes(xpath, TestUtil.TestNamespaceManager);
                                    Assert.AreEqual(querycount.count, list.Count, "Expected number of next elements did not match the actual number of next elements");

                                    // Verify that the next link is correct
                                    Assert.AreEqual(list[0].Attributes["href"].Value, querycount.result);

                                    // Verify count
                                    list = document.SelectNodes("/atom:feed/atom:entry", TestUtil.TestNamespaceManager);
                                    Assert.AreEqual(list.Count, querycount.idcount);
                                }
                            }
                        });
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void FullTraversalEFAndLinq2Sql()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = this.PageSizeCustomizer;

                    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                        new Dimension("ServiceModel", new ServiceModelData[] { ServiceModelData.Northwind, ServiceModelData.SqlNorthwindData }),
                        new Dimension("SerializationFormatData", SerializationFormatData.StructuredValues));

                    TestUtil.RunCombinatorialEngineFail(
                        engine,
                        delegate (Hashtable values)
                        {
                            ServiceModelData model = (ServiceModelData)values["ServiceModel"];
                            SerializationFormatData format = (SerializationFormatData)values["SerializationFormatData"];

                            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                            {
                                request.DataServiceType = model.ServiceModelType;

                                var querycountpairs = new[] { new { query = "/Customers?$orderby=CompanyName", expectedcount = 91 },
                                                              new { query = "/Orders(11077)/Order_Details?$orderby=OrderID add 1 desc,OrderID asc,ProductID asc", expectedcount = 25 },
                                                              new { query = "/Invoices?$orderby=Region&$top=30", expectedcount = 30 },
                                                              new { query = "/Invoices?$orderby=Region desc&$top=15", expectedcount = 15 },
                                                            };

                                foreach (var querycount in querycountpairs)
                                {
                                    int count = 0;
                                    bool needToStop = false;
                                    string uri = querycount.query;

                                    if (model.ServiceModelType == typeof(AstoriaUnitTests.Stubs.Sql.SqlNorthwindDataContext))
                                    {
                                        if (uri.Contains("Invoices"))
                                        {
                                            continue;
                                        }
                                        uri = uri.Replace("Order_Details", "OrderDetails");
                                    }

                                    while (needToStop == false)
                                    {
                                        request.Accept = format.MimeTypes[0];
                                        request.RequestUriString = uri;
                                        request.SendRequest();

                                        XmlDocument document;
                                        string xpathForNextLink;
                                        string xpathForNodeCount;
                                        if (format == SerializationFormatData.JsonLight)
                                        {
                                            document = JsonValidator.ConvertToXmlDocument(request.GetResponseStream());
                                            xpathForNextLink = "Object/odata.nextLink";
                                            xpathForNodeCount = "Object/value/Array/Object/" + (querycount.query.Contains("/Customers") ? "CustomerID" : "OrderID");
                                        }
                                        else
                                        {
                                            Assert.IsTrue(format == SerializationFormatData.Atom, "expecting Atom");
                                            document = new XmlDocument(TestUtil.TestNameTable);
                                            document.Load(request.GetResponseStream());
                                            xpathForNextLink = "/atom:feed/atom:link[@rel='next']";
                                            xpathForNodeCount = "/atom:feed/atom:entry";
                                        }

                                        XmlNodeList list = document.SelectNodes(xpathForNextLink, TestUtil.TestNamespaceManager);
                                        if (list.Count == 1)
                                        {
                                            uri = GetNextLink(list[0], format, request.ServiceRoot);
                                            uri = uri.Replace(request.BaseUri, "/");
                                        }
                                        else
                                        {
                                            needToStop = true;
                                        }

                                        list = document.SelectNodes(xpathForNodeCount, TestUtil.TestNamespaceManager);
                                        count += list.Count;
                                    }

                                    Assert.AreEqual(querycount.expectedcount, count);
                                }
                            }
                        });
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void TestQueryResultOrdering()
            {
                using (TestUtil.MetadataCacheCleaner())
                {
                    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                        new Dimension("SerializationFormatData", SerializationFormatData.StructuredValues));

                    TestUtil.RunCombinatorialEngineFail(
                        engine,
                        delegate (Hashtable values)
                        {
                            ServiceModelData model = ServiceModelData.Northwind;
                            SerializationFormatData format = (SerializationFormatData)values["SerializationFormatData"];

                            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                            {
                                request.DataServiceType = model.ServiceModelType;

                                var querycountpairs = new[] { new { query = "/Orders?$expand=Customers&$orderby=OrderID" } };

                                foreach (var querycount in querycountpairs)
                                {
                                    string uri = querycount.query;
                                    request.Accept = format.MimeTypes[0];
                                    request.RequestUriString = uri;
                                    request.SendRequest();

                                    XmlDocument document;
                                    string xpathForNextLink;
                                    string xpathForNodeCount;
                                    if (format == SerializationFormatData.JsonLight)
                                    {
                                        document = JsonValidator.ConvertToXmlDocument(request.GetResponseStream());
                                        xpathForNextLink = "Object/odata.nextLink";
                                        xpathForNodeCount = "/Object/value/Array/Object/OrderID";
                                    }
                                    else
                                    {
                                        Assert.IsTrue(format == SerializationFormatData.Atom, "expecting Atom");
                                        document = new XmlDocument(TestUtil.TestNameTable);
                                        document.Load(request.GetResponseStream());
                                        xpathForNextLink = "/atom:feed/atom:link[@rel='next']";
                                        xpathForNodeCount = "/atom:feed/atom:entry/atom:content/adsm:properties/ads:OrderID";
                                    }

                                    XmlNodeList list = document.SelectNodes(xpathForNextLink, TestUtil.TestNamespaceManager);
                                    Assert.AreEqual(0, list.Count);
                                    list = document.SelectNodes(xpathForNodeCount, TestUtil.TestNamespaceManager);
                                    Assert.AreEqual(830, list.Count);
                                    int currentOrderID;
                                    int lastOrderID = Int32.MinValue;
                                    for (int i = 0; i < 830; i++)
                                    {
                                        currentOrderID = Int32.Parse(list[i].InnerText);
                                        Assert.IsTrue(currentOrderID > lastOrderID, "Ordering constraint violated");
                                        lastOrderID = currentOrderID;
                                    }
                                }
                            }
                        });
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void TestPagingQueryResultCount()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = this.PageSizeCustomizer;

                    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                        new Dimension("ServiceModel", new ServiceModelData[] { ServiceModelData.Northwind }),
                        new Dimension("SerializationFormatData", SerializationFormatData.StructuredValues));

                    TestUtil.RunCombinatorialEngineFail(
                        engine,
                        delegate (Hashtable values)
                        {
                            ServiceModelData model = (ServiceModelData)values["ServiceModel"];
                            SerializationFormatData format = (SerializationFormatData)values["SerializationFormatData"];

                            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                            {
                                request.DataServiceType = model.ServiceModelType;

                                var querycountpairs = new[] { new { query = "/Alphabetical_list_of_products", expectedcount = 69, lookupNode = "ProductID" },
                                                              new { query = "/Invoices?$filter=CustomerID eq 'SAVEA'", expectedcount = 116 , lookupNode = "CustomerID" },
                                                              new { query = "/Products_by_Category", expectedcount = 69 , lookupNode = "ProductName" },
                                                            };

                                foreach (var querycount in querycountpairs)
                                {
                                    int count = 0;
                                    bool needToStop = false;
                                    string uri = querycount.query;

                                    while (needToStop == false)
                                    {
                                        request.Accept = format.MimeTypes[0];
                                        request.RequestUriString = uri;
                                        request.SendRequest();

                                        XmlDocument document;
                                        string xpathForNextLink;
                                        string xpathForNodeCount;
                                        if (format == SerializationFormatData.JsonLight)
                                        {
                                            document = JsonValidator.ConvertToXmlDocument(request.GetResponseStream());
                                            xpathForNextLink = "Object/odata.nextLink";
                                            xpathForNodeCount = "Object/value/Array/Object/" + querycount.lookupNode;
                                        }
                                        else
                                        {
                                            Assert.IsTrue(format == SerializationFormatData.Atom, "expecting Atom");
                                            document = new XmlDocument(TestUtil.TestNameTable);
                                            document.Load(request.GetResponseStream());
                                            xpathForNextLink = "/atom:feed/atom:link[@rel='next']";
                                            xpathForNodeCount = "/atom:feed/atom:entry";
                                        }

                                        XmlNodeList list = document.SelectNodes(xpathForNextLink, TestUtil.TestNamespaceManager);
                                        if (list.Count == 1)
                                        {
                                            uri = GetNextLink(list[0], format, request.ServiceRoot);
                                            uri = uri.Replace(request.BaseUri, "/");
                                        }
                                        else
                                        {
                                            needToStop = true;
                                        }

                                        list = document.SelectNodes(xpathForNodeCount, TestUtil.TestNamespaceManager);
                                        count += list.Count;
                                    }

                                    Assert.AreEqual(count, querycount.expectedcount);
                                }
                            }
                        });
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void Linq2SqlBooleanOrdering()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = this.PageSizeCustomizer;

                    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                        new Dimension("ServiceModel", new ServiceModelData[] { ServiceModelData.SqlNorthwindData }),
                        new Dimension("SerializationFormatData", SerializationFormatData.StructuredValues));

                    TestUtil.RunCombinatorialEngineFail(
                        engine,
                        delegate (Hashtable values)
                        {
                            ServiceModelData model = (ServiceModelData)values["ServiceModel"];
                            SerializationFormatData format = (SerializationFormatData)values["SerializationFormatData"];

                            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                            {
                                request.DataServiceType = model.ServiceModelType;

                                var querycountpairs = new[] { new { query = "/Products?$orderby=Discontinued,ProductID asc", expectedcount = 77, lookupNode = "ProductID" },
                                                            };

                                foreach (var querycount in querycountpairs)
                                {
                                    int count = 0;
                                    bool needToStop = false;
                                    string uri = querycount.query;

                                    while (needToStop == false)
                                    {
                                        request.Accept = format.MimeTypes[0];
                                        request.RequestUriString = uri;
                                        request.SendRequest();

                                        XmlDocument document;
                                        string xpathForNextLink;
                                        string xpathForNodeCount;

                                        if (format == SerializationFormatData.JsonLight)
                                        {
                                            document = JsonValidator.ConvertToXmlDocument(request.GetResponseStream());
                                            xpathForNextLink = "Object/odata.nextLink";
                                            xpathForNodeCount = "Object/value/Array/Object/" + querycount.lookupNode;
                                        }
                                        else
                                        {
                                            Assert.IsTrue(format == SerializationFormatData.Atom, "expecting Atom");
                                            document = new XmlDocument(TestUtil.TestNameTable);
                                            document.Load(request.GetResponseStream());
                                            xpathForNextLink = "/atom:feed/atom:link[@rel='next']";
                                            xpathForNodeCount = "/atom:feed/atom:entry";
                                        }

                                        XmlNodeList list = document.SelectNodes(xpathForNextLink, TestUtil.TestNamespaceManager);
                                        if (list.Count == 1)
                                        {
                                            uri = GetNextLink(list[0], format, request.ServiceRoot);

                                            // veify that boolean in $skiptoken is serialized into true or false, not 1 or 0.
                                            Assert.IsTrue(uri.StartsWith(request.BaseUri + "Products?$orderby=Discontinued,ProductID%20asc&$skiptoken=false", StringComparison.Ordinal)
                                                || uri.StartsWith(request.BaseUri + "Products?$orderby=Discontinued,ProductID%20asc&$skiptoken=true", StringComparison.Ordinal));
                                            uri = uri.Replace(request.BaseUri, "/");
                                        }
                                        else
                                        {
                                            needToStop = true;
                                        }

                                        list = document.SelectNodes(xpathForNodeCount, TestUtil.TestNamespaceManager);
                                        count += list.Count;
                                    }

                                    Assert.AreEqual(count, querycount.expectedcount);
                                }
                            }
                        });
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void VersioningUsingEF()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = this.PageSizeCustomizer;

                    ServiceModelData model = ServiceModelData.Northwind;
                    SerializationFormatData format = SerializationFormatData.Atom;

                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.DataServiceType = model.ServiceModelType;

                        var querycountpairs = new[] { new { query = "/Customers" , count = 1},
                                                      new { query = "/Orders", count = 1 },
                                                      new { query = "/Customers('ALFKI')", count = 0 },
                                                      new { query = "/Customers('ALFKI')/Orders", count = 1 },
                                                      new { query = "/Customers?$top=9", count = 0 },
                                                      new { query = "/Customers?$top=10", count = 0 },
                                                      new { query = "/Customers?$top=11", count = 1 },
                                                      new { query = "/Customers?$expand=Orders", count = 11 },
                                                      new { query = "/Customers?$expand=Orders&$top=9", count = 9 },
                                                      new { query = "/Customers?$skip=10&$filter=City eq 'Seattle'", count = 0 },
                                                      new { query = "/Customers?$orderby=CompanyName,CustomerID asc&$skiptoken='Around the Horn','AROUT'", count = 1 },
                                                };

                        // Positive tests
                        foreach (var querycount in querycountpairs)
                        {
                            string uri = querycount.query;
                            request.Accept = format.MimeTypes[0];
                            request.RequestUriString = uri;
                            request.RequestVersion = "4.0";
                            request.RequestMaxVersion = "4.0;";
                            request.SendRequest();

                            Assert.AreEqual(request.ResponseVersion, "4.0;");

                            XmlDocument document = new XmlDocument(TestUtil.TestNameTable);
                            document.Load(request.GetResponseStream());
                            XmlNodeList list = document.SelectNodes("descendant::atom:link[@rel='next']", TestUtil.TestNamespaceManager);
                            Assert.AreEqual(querycount.count, list.Count, "Expected number of next elements did not match the actual number of next elements");
                        }
                    }
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod]
            public void VerifyServiceDocumentEndPoint()
            {
                string[] entitySetNames = new string[] { "Customers", "EmptySet", "Orders", "Regions", "Products", "OrderDetails" };

                // Making sure nothing in the service document format is changed from the previous release
                using (var request = TestWebRequest.CreateForInProcess())
                {
                    request.RequestUriString = "/";
                    request.DataServiceType = typeof(CustomDataContext);
                    request.Accept = "application/atom+xml,application/xml";
                    request.SendRequest();

                    // verify atom service document format
                    var actualResponse = request.GetResponseStreamAsText();

                    string atomEntitySetElements = null;
                    foreach (string setName in entitySetNames)
                    {
                        atomEntitySetElements += GetWorkspaceElementInServiceDocument(setName);
                    }

                    var expectedResponse =
                        "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                        "<service xml:base=\"http://host/\" xmlns=\"http://www.w3.org/2007/app\" xmlns:atom=\"http://www.w3.org/2005/Atom\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" m:context=\"http://host/$metadata\">" +
                          "<workspace>" +
                            "<atom:title type=\"text\">Default</atom:title>" +
                                atomEntitySetElements +
                          "</workspace>" +
                        "</service>";

                    Assert.AreEqual(expectedResponse, actualResponse, "The atom service  document format is not as expected");

                    request.Accept = UnitTestsUtil.JsonLightMimeType;
                    request.SendRequest();

                    string jsonEntitySetNames = null;
                    string seperator = String.Empty;
                    foreach (string setName in entitySetNames)
                    {
                        jsonEntitySetNames += seperator;
                        jsonEntitySetNames += String.Format(@"{{""name"":""{0}"",""kind"":""EntitySet"",""url"":""{0}""}}", setName);
                        seperator = ",";
                    }

                    actualResponse = request.GetResponseStreamAsText();
                    actualResponse = System.Data.Test.Astoria.Util.JsonPrimitiveTypesUtil.FilterJson(actualResponse);
                    expectedResponse = "{\"@odata.context\":\"http://host/$metadata\",\"value\":[" + jsonEntitySetNames + "]}";
                    Assert.AreEqual(expectedResponse, actualResponse, "The json service  document format is not as expected");
                }
            }

            private static string GetWorkspaceElementInServiceDocument(string entitySetName)
            {
                return String.Format("<collection href=\"{0}\"><atom:title type=\"text\">{0}</atom:title></collection>", entitySetName);
            }

            class ServiceOperationTestService : OpenWebDataService<CustomDataContext>
            {
                [WebGet]
                public IQueryable<AstoriaUnitTests.Stubs.Customer> CustomersByCity(string city)
                {
                    return this.CurrentDataSource.Customers.Where(c => c.Address.City == city);
                }

                [WebInvoke(Method = "POST")]
                public IQueryable<AstoriaUnitTests.Stubs.Customer> GetCustomersByID(int id)
                {
                    return this.CurrentDataSource.Customers.Where(c => c.ID == id);
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void ServiceOperationsCustom()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = this.PageSizeCustomizer;

                    var queryresults = new[] {
                        new { query = "/CustomersByCity?city='Redmond'", result = "http://host/CustomersByCity?city='Redmond'&$skiptoken=1", nextlinkcount = 1 },
                        new { query = "/CustomersByCity(1)/Orders?city='Redmond'", result = "http://host/CustomersByCity(1)/Orders?city='Redmond'&$skiptoken=1" , nextlinkcount = 1},
                        new { query = "/CustomersByCity()?city='Redmond'&$top=3&$expand=Orders", result = "http://host/CustomersByCity?city='Redmond'&$expand=Orders&$top=1&$skiptoken=1" , nextlinkcount = 3},
                        };

                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.ServiceType = typeof(ServiceOperationTestService);

                        foreach (var queryresult in queryresults)
                        {
                            request.RequestUriString = queryresult.query;
                            request.Accept = "application/atom+xml,application/xml";
                            request.SendRequest();
                            XmlDocument document = new XmlDocument(TestUtil.TestNameTable);
                            document.Load(request.GetResponseStream());
                            XmlNodeList list = document.SelectNodes("/atom:feed/atom:link[@rel='next']", TestUtil.TestNamespaceManager);
                            Assert.AreEqual(1, list.Count, "Expected number of next elements did not match the actual number of next elements");
                            Assert.AreEqual(queryresult.result, list[0].Attributes["href"].Value, "Node type is not text");
                            list = document.SelectNodes("descendant::atom:link[@rel='next']", TestUtil.TestNamespaceManager);
                            Assert.AreEqual(queryresult.nextlinkcount, list.Count, "Expected number of next elements did not match the actual number of next elements");
                        }
                    }
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void VersioningWithPost()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = this.PageSizeCustomizer;

                    var queryresults = new[] {
                        new { query = "/Customers", hascontent = true, nextlinkcount = 0 },
                        new { query = "/GetCustomersByID()?id=1&$expand=Orders", hascontent = false, nextlinkcount = 1 },
                        };

                    string postedContent = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                                            "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                                              "<title type=\"text\" />" +
                                              "<updated>2009-06-11T23:51:06Z</updated>" +
                                              "<author>" +
                                              "<name />" +
                                              "</author>" +
                                              "<category term=\"#AstoriaUnitTests.Stubs.CustomerWithBirthday\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                                              "<content type=\"application/xml\">" +
                                              "<adsm:properties>" +
                                              "<ads:GuidValue adsm:type=\"Edm.Guid\">efab9bfa-38e4-4e1d-a6cd-27d328fadda5</ads:GuidValue>" +
                                              "<ads:ID adsm:type=\"Edm.Int32\">5</ads:ID>" +
                                              "<ads:Name>Customer 5</ads:Name>" +
                                              "<ads:Birthday adsm:type=\"Edm.DateTime\">1980-06-11T00:00:00-07:00</ads:Birthday>" +
                                              "<ads:Address adsm:type=\"#AstoriaUnitTests.Stubs.Address\">" +
                                              "<ads:StreetAddress>Line1</ads:StreetAddress>" +
                                              "<ads:City>Redmond</ads:City>" +
                                              "<ads:State>WA</ads:State>" +
                                              "<ads:PostalCode>98052</ads:PostalCode>" +
                                              "</ads:Address>" +
                                              "</adsm:properties>" +
                                              "</content>" +
                                            "</entry>";

                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.ServiceType = typeof(ServiceOperationTestService);

                        foreach (var queryresult in queryresults)
                        {
                            request.RequestUriString = queryresult.query;
                            request.HttpMethod = "POST";
                            request.Accept = "application/atom+xml,application/xml";
                            request.RequestContentType = "application/atom+xml";

                            if (queryresult.hascontent)
                            {
                                request.RequestContentLength = postedContent.Length;
                                request.RequestStream = new MemoryStream();
                                StreamWriter writer = new StreamWriter(request.RequestStream);
                                writer.Write(postedContent);
                                writer.Flush();
                            }
                            else
                            {
                                request.RequestContentLength = 0;
                            }

                            request.SendRequest();
                            XmlDocument document = new XmlDocument(TestUtil.TestNameTable);
                            Assert.AreEqual("4.0;", request.ResponseVersion);
                            document.Load(request.GetResponseStream());
                            XmlNodeList list = document.SelectNodes("/atom:feed/atom:link[@rel='next']", TestUtil.TestNamespaceManager);
                            Assert.AreEqual(0, list.Count, "Expected number of next elements did not match the actual number of next elements");
                            list = document.SelectNodes("descendant::atom:link[@rel='next']", TestUtil.TestNamespaceManager);
                            Assert.AreEqual(queryresult.nextlinkcount, list.Count, "Expected number of next elements did not match the actual number of next elements");
                        }
                    }
                }
            }

            public class OldExpandCustomDataContext : CustomDataContext, IExpandProvider
            {
                #region IExpandProvider Members

                public IEnumerable ApplyExpansions(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths)
                {
                    return queryable;
                }

                #endregion
            }

            public class OldExpandCustomObjectContext : ocs.CustomObjectContext, IExpandProvider
            {
                #region IExpandProvider Members

                public IEnumerable ApplyExpansions(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths)
                {
                    return queryable;
                }

                #endregion
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void InvalidExpandQuery()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataSourceType", new Type[] {
                        typeof(OldExpandCustomDataContext),
                        typeof(OldExpandCustomObjectContext) }));

                using (TestUtil.MetadataCacheCleaner())
                using (ocs.PopulateData.CreateTableAndPopulateData())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataSourceType = (Type)values["DataSourceType"];
                        OpenWebDataServiceHelper.PageSizeCustomizer.Value = this.PageSizeCustomizer;

                        using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                        {
                            request.ServiceType = typeof(OpenWebDataService<object>).GetGenericTypeDefinition().MakeGenericType(new Type[] { dataSourceType });

                            request.RequestUriString = "/Customers?$expand=Orders";
                            Exception exception = TestUtil.RunCatching(request.SendRequest);
                            TestUtil.AssertExceptionStatusCode(exception, 500, "Expected invalid query.");
                        }
                    });
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void SkipTokenAppliedToSetsWithoutPageSize()
            {
                // regression test: Server ignores $skiptoken in query string if preceded by $orderby option and the set doesnt have page size limits.
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataSourceType", new Type[] {
                        typeof(OldExpandCustomDataContext),
                        typeof(OldExpandCustomObjectContext) }));

                using (TestUtil.MetadataCacheCleaner())
                using (ocs.PopulateData.CreateTableAndPopulateData())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataSourceType = (Type)values["DataSourceType"];
                        OpenWebDataServiceHelper.PageSizeCustomizer.Value = this.NoPageSizeCustomizer;

                        using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                        {
                            request.ServiceType = typeof(OpenWebDataService<object>).GetGenericTypeDefinition().MakeGenericType(new Type[] { dataSourceType });

                            request.RequestUriString = "/Customers?$skiptoken=12";
                            Exception exception = TestUtil.RunCatching(request.SendRequest);
                            TestUtil.AssertExceptionStatusCode(exception, 400, "Expected invalid query.");

                            Exception dataServiceException = request.CreateExceptionFromError(XElement.Parse(request.ErrorResponseContent));
                            Assert.AreEqual(dataServiceException.Message, DataServicesResourceUtil.GetString("RequestQueryProcessor_SkipTokenSupportedOnPagedSets"));
                        }
                    });
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void CustomPagingWithExpand()
            {
                CustomRowBasedContext.ClearData();

                var querycountpairs = new[] {
                                new { query = "/Customers?$select=ID,Orders", count = 3 /* 3 customers with no orders */, maxcount = 1 },
                                new { query = "/Customers(0)?$select=ID,Orders", count = 1 /* 1 customers with no orders */, maxcount = 1 },
                                new { query = "/Customers(0)?$expand=Orders", count = 1 * (2 + 1) /* 1 customer with 2 orders */, maxcount = 1 },
                                new { query = "/Customers(0)?$expand=Orders&?select=ID,Orders/ID", count = 1 * (2 + 1) /* 1 customer with 2 orders. */, maxcount = 1 },
                                new { query = "/Customers?$expand=Orders", count = 3 * (2 + 1) /* 3 customers with 2 orders each. */, maxcount = 2 },
                                new { query = "/Customers?$expand=Orders&?select=ID,Orders/ID", count = 3 * (2 + 1) /* 3 customers with 2 orders each. */, maxcount = 2 },
                                };
                try
                {
                    using (TestUtil.MetadataCacheCleaner())
                    using (TestUtil.RestoreStaticValueOnDispose(typeof(CustomRowBasedContext), "PreserveChanges"))
                    using (TestUtil.RestoreStaticValueOnDispose(typeof(CountManager), "MaxCount"))
                    using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                    {
                        CustomRowBasedContext.PreserveChanges = true;
                        TestUtil.RunCombinations(
                            new Type[] { typeof(CustomObjectContextWithPaging), typeof(CustomReflectionContextWithPaging) },
                            SerializationFormatData.StructuredValues,
                            querycountpairs,
                            (dataSourceType, format, item) =>
                            {
                                OpenWebDataServiceHelper.PageSizeCustomizer.Value = this.PageSizeCustomizer;
                                CountManager.MaxCount = item.maxcount;
                                int totalObjects = 0;
                                int expectedObjects = item.count;

                                Queue<string> requestQueue = new Queue<string>();
                                requestQueue.Enqueue(item.query);

                                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                                {
                                    request.ServiceType = typeof(OpenWebDataService<>).MakeGenericType(new Type[] { dataSourceType });
                                    request.Accept = format.MimeTypes[0];

                                    while (requestQueue.Count > 0)
                                    {
                                        request.RequestUriString = requestQueue.Dequeue();

                                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                                        TestUtil.AssertExceptionExpected(exception, false);

                                        XmlDocument document;
                                        string xpathForTotalEntries;
                                        string xpathForNextLink;
                                        if (format == SerializationFormatData.JsonLight)
                                        {
                                            document = JsonValidator.ConvertToXmlDocument(request.GetResponseStream());
                                            xpathForTotalEntries = "//Object/ID";
                                            string ordersNextLink = XmlConvert.EncodeName("Orders@odata.nextLink");
                                            xpathForNextLink = "//Object/odata.nextLink | //" + ordersNextLink;
                                        }
                                        else
                                        {
                                            document = new XmlDocument(TestUtil.TestNameTable);
                                            document.Load(request.GetResponseStream());
                                            xpathForTotalEntries = "//atom:entry";
                                            xpathForNextLink = "//atom:feed/atom:link[@rel='next']";
                                        }

                                        XmlNodeList entries = document.SelectNodes(xpathForTotalEntries, TestUtil.TestNamespaceManager);
                                        Assert.IsTrue(CountManager.MaxCount >= entries.Count, "Count of entries did not match expected.");
                                        totalObjects += entries.Count;

                                        XmlNodeList list = document.SelectNodes(xpathForNextLink, TestUtil.TestNamespaceManager);
                                        if (list.Count > 0)
                                        {
                                            for (int i = 0; i < list.Count; i++)
                                            {
                                                string nextLink = GetNextLink(list[i], format, request.ServiceRoot);
                                                requestQueue.Enqueue(nextLink.Replace(request.BaseUri, "/"));
                                            }
                                        }
                                    }

                                    Assert.AreEqual(expectedObjects, totalObjects, "Total object count did not match expected count.");
                                }
                            });
                    }
                }
                finally
                {
                    CustomRowBasedContext.ClearData();
                }
            }

            /// <summary>
            /// Server driven paging doesn't work when the skip token value should contain NULL
            /// </summary>
            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void PagingWithNullSkipTokens()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                using (var connection = DataTypesContext.DataTypesContext.CreateTableAndPopulateData())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = this.PageSizeCustomizer;

                    CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                        new Dimension("ContextType", new Type[] {
                            typeof(DataTypesContext.SqlDataTypesContext),
                            typeof(DataTypesContext.EdmDataTypesContext) }),
                        new Dimension("SerializationFormatData", SerializationFormatData.StructuredValues));

                    TestUtil.RunCombinatorialEngineFail(
                        engine,
                        delegate (Hashtable values)
                        {
                            Type contextType = (Type)values["ContextType"];
                            SerializationFormatData format = (SerializationFormatData)values["SerializationFormatData"];

                            var querycountpairs = new[] {
                                new { query = "/DataTypes?$orderby=ID", expectedcount = 3 },
                                new { query = "/DataTypes?$orderby=ID desc", expectedcount = 3 },
                                new { query = "/DataTypes?$orderby=String", expectedcount = 3 },
                                new { query = "/DataTypes?$orderby=String desc", expectedcount = 3 },
                                new { query = "/DataTypes?$orderby=Integer", expectedcount = 3 },
                                new { query = "/DataTypes?$orderby=Integer desc", expectedcount = 3 },
                                new { query = "/DataTypes?$orderby=Float", expectedcount = 3 },
                                new { query = "/DataTypes?$orderby=Float desc", expectedcount = 3 },
                                new { query = "/DataTypes?$orderby=Decimal", expectedcount = 3 },
                                new { query = "/DataTypes?$orderby=Decimal desc", expectedcount = 3 },
                                new { query = "/DataTypes?$orderby=Boolean", expectedcount = 3 },
                                new { query = "/DataTypes?$orderby=Boolean desc", expectedcount = 3 },
                                new { query = "/DataTypes?$orderby=Date", expectedcount = 3 },
                                new { query = "/DataTypes?$orderby=Date desc", expectedcount = 3 },
                                new { query = "/DataTypes?$orderby=UniqueId", expectedcount = 3 },
                                new { query = "/DataTypes?$orderby=UniqueId desc", expectedcount = 3 },
                            };

                            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                            {
                                request.DataServiceType = contextType;

                                TestUtil.RunCombinations(querycountpairs, (querycount) =>
                                {
                                    int count = 0;
                                    bool needToStop = false;
                                    string uri = querycount.query;

                                    while (!needToStop)
                                    {
                                        request.Accept = format.MimeTypes[0];
                                        request.RequestUriString = uri;
                                        request.SendRequest();

                                        XmlDocument document;
                                        string xpathForNextLink;
                                        string xpathForNodeCount;
                                        if (format == SerializationFormatData.JsonLight)
                                        {
                                            document = JsonValidator.ConvertToXmlDocument(request.GetResponseStream());
                                            xpathForNextLink = "/Object/odata.nextLink";
                                            xpathForNodeCount = "//Object/ID";
                                        }
                                        else
                                        {
                                            Assert.IsTrue(format == SerializationFormatData.Atom, "expecting Atom");
                                            document = new XmlDocument(TestUtil.TestNameTable);
                                            document.Load(request.GetResponseStream());
                                            xpathForNextLink = "/atom:feed/atom:link[@rel='next']";
                                            xpathForNodeCount = "/atom:feed/atom:entry";
                                        }

                                        XmlNodeList list = document.SelectNodes(xpathForNextLink, TestUtil.TestNamespaceManager);
                                        if (list.Count == 1)
                                        {
                                            uri = GetNextLink(list[0], format, request.ServiceRoot);
                                            uri = uri.Replace(request.BaseUri, "/");
                                        }
                                        else
                                        {
                                            needToStop = true;
                                        }

                                        list = document.SelectNodes(xpathForNodeCount, TestUtil.TestNamespaceManager);
                                        count += list.Count;
                                    }
                                    Assert.AreEqual(querycount.expectedcount, count);
                                });
                            }
                        });
                }
            }

            /// <summary>
            /// Regression test: Custom IDataServicePagingProvider does not update the $top value in the next link uri correctly
            /// </summary>
            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void CustomPagingWithTopQueryParameter()
            {
                CustomRowBasedContext.ClearData();

                try
                {
                    using (TestUtil.MetadataCacheCleaner())
                    using (TestUtil.RestoreStaticValueOnDispose(typeof(CustomRowBasedContext), "PreserveChanges"))
                    using (TestUtil.RestoreStaticValueOnDispose(typeof(CountManager), "MaxCount"))
                    {
                        CustomRowBasedContext.PreserveChanges = true;
                        var querycountpairs = new[] {
                                new
                                {
                                    query = "/Customers?$top=3&$skip=1",
                                    maxcount = 1,
                                    result = "http://host/Customers?$top=2&$skiptoken=1"
                                },
                                new
                                {
                                    query = "/Customers?$top=3",
                                    maxcount = 2,
                                    result = "http://host/Customers?$top=1&$skiptoken=1"
                                },
                                // TODO: this test needs to be fixed
                                //new
                                //{
                                //    query = "/Customers?$top=3&$skiptoken=-21&$filter=true or%20false&$skip=1&$orderby='foo'+asc&$count=true&$expand=BestFriend&$select=Name",
                                //    maxcount = 1,
                                //    result = "http://host/Customers?$filter=true%20or%20false&$expand=BestFriend&$orderby='foo'%20asc&$count=true&$select=Name&$top=2&$skiptoken=1"
                                //},
                                new
                                {
                                    query = "/Customers?$top=2",
                                    maxcount = 2,
                                    result = (string)null,
                                },
                            };

                        TestUtil.RunCombinations(
                            new Type[] { typeof(CustomObjectContextWithPaging), typeof(CustomReflectionContextWithPaging) },
                            SerializationFormatData.StructuredValues,
                            querycountpairs,
                            (dataSourceType, format, item) =>
                            {
                                CountManager.MaxCount = item.maxcount;
                                int totalObjects = 0;

                                Queue<string> requestQueue = new Queue<string>();
                                requestQueue.Enqueue(item.query);

                                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                                {
                                    request.ServiceType = typeof(OpenWebDataService<>).MakeGenericType(new Type[] { dataSourceType });
                                    request.Accept = format.MimeTypes[0];

                                    request.RequestUriString = requestQueue.Dequeue();

                                    Exception exception = TestUtil.RunCatching(request.SendRequest);
                                    TestUtil.AssertExceptionExpected(exception, false);

                                    XmlDocument document;
                                    string xpathForTotalEntries;
                                    string xpathForNextLink;
                                    if (format == SerializationFormatData.JsonLight)
                                    {
                                        document = JsonValidator.ConvertToXmlDocument(request.GetResponseStream());
                                        xpathForTotalEntries = "//Object/Name";
                                        string ordersNextLink = XmlConvert.EncodeName("Orders@odata.nextLink");
                                        xpathForNextLink = "//Object/odata.nextLink | //" + ordersNextLink;
                                    }
                                    else
                                    {
                                        document = new XmlDocument(TestUtil.TestNameTable);
                                        document.Load(request.GetResponseStream());
                                        xpathForTotalEntries = "//atom:entry";
                                        xpathForNextLink = "//atom:feed/atom:link[@rel='next']";
                                    }

                                    XmlNodeList entries = document.SelectNodes(xpathForTotalEntries, TestUtil.TestNamespaceManager);
                                    Assert.IsTrue(CountManager.MaxCount >= entries.Count, "Count of entries did not match expected.");
                                    totalObjects += entries.Count;

                                    Assert.AreEqual(item.maxcount, totalObjects, "Total object count did not match expected count.");

                                    XmlNodeList list = document.SelectNodes(xpathForNextLink, TestUtil.TestNamespaceManager);
                                    if (item.result == null)
                                    {
                                        Assert.AreEqual(0, list.Count, "Should not have found a next link");
                                    }
                                    else
                                    {
                                        Assert.AreEqual(1, list.Count, "Should have found exactly 1 next link");
                                        string nextLink = GetNextLink(list[0], format, request.ServiceRoot);
                                        Assert.AreEqual(item.result, nextLink, "Next link is not as expected");
                                    }
                                }
                            });
                    }
                }
                finally
                {
                    CustomRowBasedContext.ClearData();
                }
            }

            public class StringData
            {
                public String ID { get; set; }
            }

            public class GuidData
            {
                public Guid ID { get; set; }
            }

            public class DoubleData
            {
                public double ID { get; set; }
            }

            public static StringData[] Strings = new StringData[] {
            new StringData { ID =  "64SSXvlMs0zf8rVIcxS4aGXoVEuib3r7XUnfYKc2sRmY"},
            new StringData { ID =  "bZbkSh1n3OiC9E"},
            new StringData { ID =  "EUAC67Qnj4iAO8YXiQfDYBIbny8LQCN3JBhJe40"},
            new StringData { ID =  "FFgIFGSnXQGW0pBAsxgmNTgER8hf9UODzpuJVZ2e4"},
            new StringData { ID =  "jn6ggg24hgxysBbwHiJe"},
            new StringData { ID =  "ZDIjeu4TK3nq9yHXkmDpoaQpuDbL8KAYVDsOimO4FE479"},
            new StringData { ID =  "sYWZIaxDES31PwYy4WNp7TKYbWYo"},
            new StringData { ID =  "87s4IXQVu18cR3uyNE9S7Zji9b4ySjh"},
            new StringData { ID =  "x3bnLl7EAylC0yqkxtyRpFUABz3VghDJUQVv6pT40kgLZU"},
            new StringData { ID =  "KDbDmSUSrdydTAHMa8dwZ8ZkcQYUUOnqQtLnN"},
            new StringData { ID =  "UlFOODj2HBrdhc"},
            new StringData { ID =  "nyRoDyZEP"},
            new StringData { ID =  "qTXfjp5zW5g"},
            };

            public static GuidData[] Guids = new GuidData[] {
            new GuidData { ID = System.Guid.NewGuid() },
            new GuidData { ID = System.Guid.NewGuid() },
            new GuidData { ID = System.Guid.NewGuid() },
            new GuidData { ID = System.Guid.NewGuid() },
            new GuidData { ID = System.Guid.NewGuid() },
            new GuidData { ID = System.Guid.NewGuid() },
            new GuidData { ID = System.Guid.NewGuid() },
            new GuidData { ID = System.Guid.NewGuid() },
            new GuidData { ID = System.Guid.NewGuid() },
            new GuidData { ID = System.Guid.NewGuid() },
            new GuidData { ID = System.Guid.NewGuid() },
            };

            public static DoubleData[] Doubles = new DoubleData[] {
            new DoubleData { ID =  Double.MaxValue},
            new DoubleData { ID =  Double.MinValue},
            new DoubleData { ID =  Double.NaN},
            new DoubleData { ID =  0},
            new DoubleData { ID =  12345678901},
            new DoubleData { ID =  -1.0e12},
            new DoubleData { ID =  1.0e12},
            new DoubleData { ID =  1234},
            new DoubleData { ID =  1234.5678},
            new DoubleData { ID =  Double.NegativeInfinity},
            new DoubleData { ID =  Double.PositiveInfinity},
            };

            public class StringContext
            {
                public IQueryable<StringData> Data
                {
                    get
                    {
                        return Strings.AsQueryable();
                    }
                }
            }

            public class GuidContext
            {
                public IQueryable<GuidData> Data
                {
                    get
                    {
                        return Guids.AsQueryable();
                    }
                }
            }

            public class DoubleContext
            {
                public IQueryable<DoubleData> Data
                {
                    get
                    {
                        return Doubles.AsQueryable();
                    }
                }
            }
            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void CustomPagingWithDifferentIdType()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataSourceType", new Type[] {
                        typeof(StringContext),
                        typeof(GuidContext),
                        typeof(DoubleContext)})
                        );

                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        OpenWebDataServiceHelper.PageSizeCustomizer.Value = this.AllTo2PageSizeCustomizer;

                        using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                        {
                            Type dataSourceType = (Type)values["DataSourceType"];
                            request.ServiceType = typeof(OpenWebDataService<>).MakeGenericType(dataSourceType);
                            request.RequestUriString = "/Data";
                            request.Accept = "application/atom+xml,application/xml";

                            List<string> actual = new List<string>();

                            while (true)
                            {
                                request.SendRequest();

                                XmlDocument document = new XmlDocument(TestUtil.TestNameTable);
                                document.Load(request.GetResponseStream());

                                string xpath = "/atom:feed/atom:entry/atom:content/adsm:properties/ads:ID";
                                XmlNodeList list = document.SelectNodes(xpath, TestUtil.TestNamespaceManager);

                                foreach (String value in GetInnerText(list))
                                {
                                    actual.Add(value);
                                }

                                string xpathForNextLink = "/atom:feed/atom:link[@rel='next']";
                                list = document.SelectNodes(xpathForNextLink, TestUtil.TestNamespaceManager);
                                if (list.Count > 0)
                                {
                                    request.RequestUriString = list[0].Attributes["href"].Value.Replace(request.BaseUri, "/");
                                }
                                else
                                {
                                    break;
                                }
                            }

                            IEnumerable<string> expected;

                            if (dataSourceType == typeof(StringContext))
                            {
                                expected = Strings.OrderBy(s => s.ID).Select(s => s.ID);
                            }
                            else
                                if (dataSourceType == typeof(GuidContext))
                            {
                                expected = Guids.OrderBy(s => s.ID).Select(s => s.ID.ToString());
                            }
                            else
                            {
                                expected = Doubles.OrderBy(d => d.ID).Select(d => XmlConvert.ToString(d.ID));
                            }

                            Assert.IsTrue(CompareEnums(actual, expected));
                        }
                    });
                }
            }

            internal class ResourceSetMetadataKeyOrderEnabler
            {
                public Action<provider.ResourceSet> EnableOnResourceSet { get; set; }
                public bool MetadataKeyOrderEnabled { get; set; }
            }

            private static string[] orderingMethodNames = new string[] { "OrderBy", "OrderByDescending", "ThenBy", "ThenByDescending" };

            [TestMethod, Variation]
            public void ImplicitOrderByPropertiesSorting_CustomProvider()
            {
                Type[] keyPropertyTypes = new Type[] { typeof(int), typeof(string), typeof(DateTime), typeof(double), typeof(Guid) };
                string[] keyPropertyNames = new string[] { "AKey", "CKey", "BKey", "bKey", "aKey" };
                var metadataOrderingEnablers = new ResourceSetMetadataKeyOrderEnabler[] {
                    new ResourceSetMetadataKeyOrderEnabler{  // Default behavior -> false
                        EnableOnResourceSet = (rs) => {},
                        MetadataKeyOrderEnabled = false
                    },
                    new ResourceSetMetadataKeyOrderEnabler{  // Set the public property to false -> false
                        EnableOnResourceSet = (rs) => { rs.UseMetadataKeyOrder = false; },
                        MetadataKeyOrderEnabled = false
                    },
                    new ResourceSetMetadataKeyOrderEnabler{  // Set the public property to true -> true
                        EnableOnResourceSet = (rs) => { rs.UseMetadataKeyOrder = true; },
                        MetadataKeyOrderEnabled = true
                    },
                    new ResourceSetMetadataKeyOrderEnabler{  // Set custom state to a boolean -> false
                        EnableOnResourceSet = (rs) => { rs.CustomState = true; },
                        MetadataKeyOrderEnabled = false
                    },
                    new ResourceSetMetadataKeyOrderEnabler{  // Set custom state to wrong typed dictionary -> false
                        EnableOnResourceSet = (rs) => { var d = new Dictionary<string, bool>(); d["UseMetadataKeyOrder"] = true; rs.CustomState = d; },
                        MetadataKeyOrderEnabled = false
                    },
                    new ResourceSetMetadataKeyOrderEnabler{  // Set custom state to the right dictionary, but empty -> false
                        EnableOnResourceSet = (rs) => { var d = new Dictionary<string, object>(); rs.CustomState = d; },
                        MetadataKeyOrderEnabled = false
                    },
                    new ResourceSetMetadataKeyOrderEnabler{  // Set custom state to dictionary with integer value -> false
                        EnableOnResourceSet = (rs) => { var d = new Dictionary<string, object>(); d["UseMetadataKeyOrder"] = 42; rs.CustomState = d; },
                        MetadataKeyOrderEnabled = false
                    },
                    new ResourceSetMetadataKeyOrderEnabler{  // Set custom state to dictionary with wrong casing in key -> false
                        EnableOnResourceSet = (rs) => { var d = new Dictionary<string, object>(); d["useMetadataKeyorder"] = true; rs.CustomState = d; },
                        MetadataKeyOrderEnabled = false
                    },
                    new ResourceSetMetadataKeyOrderEnabler{  // Set custom state to dictionary with false value -> false
                        EnableOnResourceSet = (rs) => { var d = new Dictionary<string, object>(); d["UseMetadataKeyOrder"] = false; rs.CustomState = d; },
                        MetadataKeyOrderEnabled = false
                    },
                    new ResourceSetMetadataKeyOrderEnabler{  // Set custom state to dictionary with true value -> true
                        EnableOnResourceSet = (rs) => { var d = new Dictionary<string, object>(); d["UseMetadataKeyOrder"] = true; rs.CustomState = d; },
                        MetadataKeyOrderEnabled = true
                    },
                    new ResourceSetMetadataKeyOrderEnabler{  // Set custom state to dictionary with true value (and something else) -> true
                        EnableOnResourceSet = (rs) => { var d = new Dictionary<string, object>();
                            d["UseMetadataKeyOrder"] = true; d["MyCustomValue"] = 42;
                            rs.CustomState = d; },
                        MetadataKeyOrderEnabled = true
                    },
                    new ResourceSetMetadataKeyOrderEnabler{  // Set custom state to dictionary with true value and set the public property to false explicitely -> true (QFE way wins)
                        EnableOnResourceSet = (rs) => { var d = new Dictionary<string, object>();
                            d["UseMetadataKeyOrder"] = true; rs.CustomState = d;
                            rs.UseMetadataKeyOrder = false; },
                        MetadataKeyOrderEnabled = true
                    },
                    new ResourceSetMetadataKeyOrderEnabler{  // Set custom state to dictionary with false value and public property to true -> true (the dictionary is ignored)
                        EnableOnResourceSet = (rs) => { var d = new Dictionary<string, object>();
                            d["UseMetadataKeyOrder"] = false; rs.CustomState = d;
                            rs.UseMetadataKeyOrder = true; },
                        MetadataKeyOrderEnabled = true
                    },
                };

                TestUtil.RunCombinations(
                    new int[] { 1, 2, 5 },
                    new string[] { "SDP", "$top=1", "$skip=1", "expand" },
                    UnitTestsUtil.BooleanValues,
                    metadataOrderingEnablers,
                    (keyPropertyCount, wayToForceImplicitOrdering, behaviorUseMetadataKeyOrder, metadataOrderingEnabler) =>
                    {
                        DSPMetadata metadata = new DSPMetadata("Test", "TestNS");
                        var baseEntityType = metadata.AddEntityType("BaseEntityType", null, null, false);
                        provider.ResourceProperty[] keyProperties = new provider.ResourceProperty[keyPropertyCount];
                        for (int i = 0; i < keyPropertyCount; i++)
                        {
                            keyProperties[i] = metadata.AddKeyProperty(baseEntityType, keyPropertyNames[i], keyPropertyTypes[i]);
                        }
                        metadata.AddPrimitiveProperty(baseEntityType, "Name", typeof(string));
                        var derivedEntityType = metadata.AddEntityType("DerivedEntityType", null, baseEntityType, false);
                        var parentEntityType = metadata.AddEntityType("ParentEntityType", null, null, false);
                        metadata.AddKeyProperty(parentEntityType, "ID", typeof(int));
                        var resourceSet = metadata.AddResourceSet("Entities", baseEntityType);
                        var otherResourceSet = metadata.AddResourceSet("OtherEntities", baseEntityType);
                        var parentResourceSet = metadata.AddResourceSet("ParentEntities", parentEntityType);
                        metadata.AddResourceSetReferenceProperty(parentEntityType, "RelatedEntities", resourceSet, derivedEntityType);
                        metadataOrderingEnabler.EnableOnResourceSet(resourceSet);
                        metadata.SetReadOnly();

                        DSPServiceDefinition service = new DSPServiceDefinition()
                        {
                            Metadata = metadata,
                            DataSource = new DSPContext(),
                            ServiceConstructionCallback = ExpressionTreeTestUtils.RegisterOnService
                        };
                        service.DataServiceBehavior.UseMetadataKeyOrderForBuiltInProviders = behaviorUseMetadataKeyOrder;
                        if (wayToForceImplicitOrdering == "SDP" || wayToForceImplicitOrdering == "expand")
                        {
                            service.PageSizeCustomizer = (config, type) =>
                            {
                                config.SetEntitySetPageSize(resourceSet.Name, 2);
                                config.SetEntitySetPageSize(otherResourceSet.Name, 2);
                            };
                        }

                        if (wayToForceImplicitOrdering == "expand")
                        {
                            VerifyKeyPropertyOrdering(service, "ParentEntities", "$expand=RelatedEntities", keyProperties.Select(p => p.Name),
                                metadataOrderingEnabler.MetadataKeyOrderEnabled, "//Call[Method='Select']/Arguments/*[2]//MemberAssignment");
                        }
                        else
                        {
                            // Verify the resource set which is potentially marked to use different key ordering
                            VerifyKeyPropertyOrdering(service, "Entities", wayToForceImplicitOrdering, keyProperties.Select(p => p.Name), metadataOrderingEnabler.MetadataKeyOrderEnabled, "");
                            // Verify a resource set with the same base type that it's not marked with different key ordering
                            VerifyKeyPropertyOrdering(service, "OtherEntities", wayToForceImplicitOrdering, keyProperties.Select(p => p.Name), false, "");
                        }
                    });
            }

            [TestMethod, Variation]
            public void ImplicitOrderByPropertiesSorting_BuiltinProviders()
            {
                string[] keyPropertyNames = new string[] { "bKey", "AKey" };

                using (CompoundKeyContext.CompoundKeyContext.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinations(
                        new Type[] {
                            typeof(CompoundKeyContext.EdmCompoundKeyContext.EdmCompoundKeyContext),
                            typeof(CompoundKeyContext.ReflectionCompoundKeyContext.ReflectionCompoundKeyContext) },
                        UnitTestsUtil.BooleanValues,
                        UnitTestsUtil.BooleanValues,
                        (contextType, useExpand, useMetadataKeyOrder) =>
                        {
                            OpenWebDataServiceDefinition service = new OpenWebDataServiceDefinition()
                            {
                                DataServiceType = contextType,
                                PageSizeCustomizer = (config, type) =>
                                    {
                                        config.SetEntitySetPageSize("Orders", 2);
                                    },
                                ServiceConstructionCallback = ExpressionTreeTestUtils.RegisterOnService
                            };
                            service.DataServiceBehavior.UseMetadataKeyOrderForBuiltInProviders = useMetadataKeyOrder;

                            if (useExpand)
                            {
                                VerifyKeyPropertyOrdering(service, "Customers", "$expand=Orders", keyPropertyNames, useMetadataKeyOrder,
                                    "//Call[Method='Select']/Arguments/*[2]//MemberAssignment");
                            }
                            else
                            {
                                VerifyKeyPropertyOrdering(service, "Orders", "", keyPropertyNames, useMetadataKeyOrder, "");
                            }
                        });
                }
            }

            private void VerifyKeyPropertyOrdering(TestServiceDefinition service, string entitySetName, string wayToForceImplicitOrdering,
                IEnumerable<string> keyPropertyNames, bool useMetadataKeyOrder, string xpathToStartAt)
            {
                using (TestWebRequest request = service.CreateForInProcess())
                {
                    string requestUri = "/" + entitySetName;
                    if (wayToForceImplicitOrdering.StartsWith("$"))
                    {
                        requestUri += "?" + wayToForceImplicitOrdering;
                    }

                    request.RequestUriString = requestUri;
                    request.SendRequest();
                }
                var expectedKeyPropertiesOrder = keyPropertyNames.ToList();
                if (!useMetadataKeyOrder)
                {
                    expectedKeyPropertiesOrder.Sort(StringComparer.OrdinalIgnoreCase);
                }

                var expressionTree = ExpressionTreeTestUtils.GetLastExpressionTreeXml();
                var orderingCall = expressionTree.SelectSingleNode(xpathToStartAt + "//Call[Method='OrderBy' or Method='OrderByDescending' or Method='ThenBy' or Method='ThenByDescending']") as XmlElement;
                var orderingProperties = this.GetOrderByProperties(orderingCall);
                orderingProperties.Skip(orderingProperties.Count - expectedKeyPropertiesOrder.Count); // Skip additional orderby, since those or at the begining

                Assert.AreEqual(string.Join(",", expectedKeyPropertiesOrder), string.Join(",", orderingProperties),
                    "The order of key properties in the ordering expressions is wrong.");
            }

            private List<string> GetOrderByProperties(XmlElement callElement)
            {
                Assert.AreEqual("Call", callElement.Name, "Call element is required.");

                List<string> properties = new List<string>();

                while (callElement != null)
                {
                    if (orderingMethodNames.Contains(callElement.ChildNodes.OfType<XmlElement>().First(e => e.Name == "Method").InnerText))
                    {
                        var arguments = callElement.ChildNodes.OfType<XmlElement>().First(e => e.Name == "Arguments");
                        callElement = arguments.ChildNodes.OfType<XmlElement>().First();
                        var lambda = arguments.ChildNodes.OfType<XmlElement>().Skip(1).First();
                        var resourcePropertyConstant = lambda.SelectSingleNode(".//Constant[@type='ResourceProperty']") as XmlElement;
                        if (resourcePropertyConstant != null)
                        {
                            properties.Insert(0, resourcePropertyConstant.InnerText);
                        }
                        else
                        {
                            var memberAccessMember = lambda.SelectSingleNode(".//Body/MemberAccess/Member") as XmlElement;
                            if (memberAccessMember != null)
                            {
                                properties.Insert(0, memberAccessMember.InnerText);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }

                    if (callElement.Name != "Call")
                    {
                        break;
                    }
                }

                return properties;
            }

            private IEnumerable<string> GetInnerText(XmlNodeList list)
            {
                foreach (XmlNode node in list)
                {
                    yield return node.InnerText;
                }
            }

            private void AllTo2PageSizeCustomizer(DataServiceConfiguration config, Type modelType)
            {
                config.SetEntitySetPageSize("*", 2);
            }

            private bool CompareEnums<T>(IEnumerable<T> left, IEnumerable<T> right)
            {
                IEnumerator<T> leftEnum = left.GetEnumerator();
                IEnumerator<T> rightEnum = right.GetEnumerator();

                while (leftEnum.MoveNext())
                {
                    if (!rightEnum.MoveNext())
                        return false;

                    if (!leftEnum.Current.Equals(rightEnum.Current))
                        return false;
                }

                return !rightEnum.MoveNext();
            }

            private string GetNextLink(XmlNode node, SerializationFormatData format, Uri baseUri)
            {
                string nextLink = format == SerializationFormatData.JsonLight ? node.ChildNodes[0].Value : node.Attributes["href"].Value;
                if (format == SerializationFormatData.JsonLight)
                {
                    nextLink = new Uri(baseUri, nextLink).AbsoluteUri;
                }

                return nextLink;
            }
        }
    }
}
