//---------------------------------------------------------------------
// <copyright file="AnyAllFilterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Xml.Linq;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ocs = AstoriaUnitTests.ObjectContextStubs;
    #endregion

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
    [TestClass]
    public class AnyAllFilterTests
    {
        public class MVComplexType
        {
            public string Name { get; set; }
            public List<int> Numbers { get; set; }
        }

        public class EntityForQuery
        {
            public int ID { get; set; }
            public List<int> CollectionOfInt { get; set; }
            public List<string> CollectionOfString { get; set; }
            public List<MVComplexType> CollectionOfComplexType { get; set; }
        }
        [Ignore] // Remove Atom
        // [TestMethod]
        public void FilterCollectionWithAnyAll()
        {

            using (OpenWebDataServiceHelper.AcceptAnyAllRequests.Restore())
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            using (TestUtil.RestoreStaticValueOnDispose(typeof(TypedCustomDataContext<EntityForQuery>), "PreserveChanges"))
            {
                OpenWebDataServiceHelper.AcceptAnyAllRequests.Value = true;
                request.Accept = UnitTestsUtil.AtomFormat;
                request.DataServiceType = typeof(TypedCustomDataContext<EntityForQuery>);

                TypedCustomDataContext<EntityForQuery>.ClearHandlers();
                TypedCustomDataContext<EntityForQuery>.ClearValues();
                TypedCustomDataContext<EntityForQuery>.PreserveChanges = true;
                TypedCustomDataContext<EntityForQuery>.ValuesRequested += (sender, args) =>
                {
                    TypedCustomDataContext<EntityForQuery> typedContext = (TypedCustomDataContext<EntityForQuery>)sender;
                    typedContext.SetValues(new EntityForQuery[] { 
                                                        new EntityForQuery {
                                                            ID = 0,
                                                            CollectionOfInt = new List<int>() {1,2,3},
                                                            CollectionOfString = new List<string>() {"a", "b", "c"},
                                                            CollectionOfComplexType = new List<MVComplexType>()
                                                        },
                                                        new EntityForQuery {
                                                            ID = 1,
                                                            CollectionOfInt = new List<int>() {4,5,6},
                                                            CollectionOfString = new List<string>() {"d", "e", "f"},
                                                            CollectionOfComplexType = new List<MVComplexType>()
                                                        },
                                                        new EntityForQuery {
                                                            ID = 2,
                                                            CollectionOfInt = new List<int>() {7,8,9},
                                                            CollectionOfString = new List<string>() {"x", "y", "z"},
                                                            CollectionOfComplexType = new List<MVComplexType>() { new MVComplexType { Name = "mvcomplextype", Numbers = new List<int>() {100, 101}} }
                                                        },
                                                });
                };

                request.StartService();

                string[] requestStrings = new string[]
                                        {
                                            "/Values?$filter=CollectionOfInt/any() and (ID eq 2)",
                                            "/Values?$filter=CollectionOfString/any(s: s eq 'y')", 
                                            "/Values?$filter=CollectionOfInt/any(i: cast(i, 'Edm.Int64') eq 7)", 
                                            "/Values?$filter=CollectionOfInt/all(i: (i ge 7) and (i le 9))",
                                            "/Values?$filter=CollectionOfInt/all(i: isof(i, 'Edm.Int32') and ($it/ID eq 2))",
                                            "/Values?$filter=CollectionOfComplexType/any(ct: (ct/Name eq 'mvcomplextype') and ct/Numbers/all(n: (n ge 100) and (ct/Name eq 'mvcomplextype')))"
                                        };

                foreach (var uri in requestStrings)
                {
                    request.RequestUriString = uri;
                    request.ForceVerboseErrors = true;
                    Exception e = TestUtil.RunCatching(request.SendRequest);
                    Assert.IsNull(e, "Not expecting exception.");

                    var xdoc = request.GetResponseStreamAsXDocument();
                    UnitTestsUtil.VerifyXPaths(xdoc,
                                                "count(//atom:entry)=1",
                                                "boolean(//atom:entry/atom:id[contains(.,'Values(2)')])");
                }
            }
        }
        [Ignore] // Remove Atom
        // [TestMethod]
        public void FilterNavigationWithAnyAll()
        {
            ocs.PopulateData.EntityConnection = null;
            using (OpenWebDataServiceHelper.AcceptAnyAllRequests.Restore())
            using (System.Data.EntityClient.EntityConnection connection = ocs.PopulateData.CreateTableAndPopulateData())
            {
                OpenWebDataServiceHelper.AcceptAnyAllRequests.Value = true;
                Type[] types = new Type[] { typeof(ocs.CustomObjectContext), typeof(CustomDataContext), typeof(CustomRowBasedContext) };

                var testCases = new[]
                {
                    new 
                    {
                        filters = new string[]
                        {
                            "Orders/any() eq false",
                            "Orders/any() and ID eq 100",
                            "Orders/any(o: o/ID eq 500)",
                            "Orders/any(o: o/ID eq 100 and o/$(Customer)/ID ne 0)",
                            "Orders/any(o: isof(o/$(Customer),'$(CustomerWithBirthDayType)') and o/$(Customer)/ID ne 1)",
                            "Orders/all(o: o/$(Customer)/ID eq 1 and $it/ID ne 1)",
                            "Orders/all(o: o/ID eq 100)",
                            "Orders/all(o: o/ID eq $it/ID)",
                            "Orders/all(o: o/$(Customer)/Orders/any() eq false)",
                            "Orders/all(o: $it/ID eq 1 and o/$(Customer)/Orders/any(o1: o1/ID eq 500))",
                            "Orders/all(o: isof(o/$(Customer),'$(CustomerWithBirthDayType)') and o/$(Customer)/ID ne 1)",
                        },
                        xpaths = new string[]
                        {
                            "count(//atom:entry)=0"
                        }
                    },

                    new 
                    {
                        filters = new string[]
                        {
                            "Orders/any()",
                            "Orders/any() and ID lt 100",
                            "ID lt 100 and Orders/any()",
                            "Orders/all(o: o/$(Customer)/Orders/any())",
                        },
                        xpaths = new string[]
                        {
                            "count(//atom:entry)=3"
                        }
                    },

                    new 
                    {
                        filters = new string[]
                        {
                            "Orders/any() and (ID eq 1)",
                            "Orders/any() and isof('$(CustomerWithBirthDayType)')",
                            "Orders/any(o: $it/ID eq 1)",
                            "Orders/any(o: o/$(Customer)/ID eq 1)",
                            "Orders/any(o: isof(o/$(Customer), '$(CustomerWithBirthDayType)'))",

                            "Orders/all(o: $it/ID eq 1)",
                            "Orders/all(o: o/$(Customer)/ID eq 1)",
                            "Orders/all(o: isof(o/$(Customer), '$(CustomerWithBirthDayType)'))",

                            // using the same range variable name in multiple not-nested predicates
                            "Orders/any(o: o/$(Customer)/ID eq 1) and Orders/all(o: o/$(Customer)/ID eq 1)",

                            // nested queries
                            "Orders/any(o: o/$(Customer)/Orders/any(o1: o1/$(Customer)/Orders/all(o2: o2/$(Customer)/ID eq 1)) or $it/ID eq 1)",
                            "Orders/all(o: $it/ID eq 1 and o/$(Customer)/Orders/all(o1: o1/$(Customer)/Orders/any(o2: o2/$(Customer)/ID eq 1)))",

                            // keywords
                            "Orders/any(event: event/$(Customer)/ID eq 1)",
                            "Orders/any(while: while/$(Customer)/ID eq 1)",
                        },
                        xpaths = new string[]
                        {
                            "count(//atom:entry)=1",
                            "boolean(//atom:entry/atom:category[contains(@term,'CustomerWithBirthday')])"
                        }
                    }
                };

                TestUtil.RunCombinations(types, (type) =>
                    {
                        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                        {
                            request.DataServiceType = type;
                            request.StartService();

                            TestUtil.RunCombinations(testCases, (testCase) =>
                                {
                                    foreach (var str in testCase.filters)
                                    {
                                        string filter = UnitTestsUtil.ProcessStringVariables(str, (variable) =>
                                         {
                                             if (type == typeof(ocs.CustomObjectContext))
                                             {
                                                 switch (variable)
                                                 {
                                                     case "Customer":
                                                         return "Customers";
                                                     case "CustomerWithBirthDayType":
                                                         return "AstoriaUnitTests.ObjectContextStubs.Types.CustomerWithBirthday";
                                                 }
                                             }
                                             else if (type == typeof(CustomDataContext))
                                             {
                                                 switch (variable)
                                                 {
                                                     case "CustomerWithBirthDayType":
                                                         return "AstoriaUnitTests.Stubs.CustomerWithBirthday";
                                                 }
                                             }
                                             else if (type == typeof(CustomRowBasedContext))
                                             {
                                                 switch (variable)
                                                 {
                                                     case "CustomerWithBirthDayType":
                                                         return "AstoriaUnitTests.Stubs.CustomerWithBirthday";
                                                 }
                                             }

                                             return variable;
                                         });

                                        request.RequestUriString = "/Customers?$format=atom&$filter=" + filter;
                                        request.SendRequest();
                                        var response = request.GetResponseStreamAsXDocument();
                                        UnitTestsUtil.VerifyXPaths(response, testCase.xpaths);
                                    }
                                });
                        }
                    });
            }
        }
        [Ignore] // Remove Atom
        // [TestMethod]
        public void FilterNavigationWithAnyAll_TypeCasts()
        {
            using (OpenWebDataServiceHelper.AcceptAnyAllRequests.Restore())
            {
                OpenWebDataServiceHelper.AcceptAnyAllRequests.Value = true;
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.StartService();

                    string[] filters = new string[]
                            {
                                "Orders/all(o: o/Customer/AstoriaUnitTests.Stubs.CustomerWithBirthday/Birthday gt 1911-04-22T15:20:45.907Z)",
                                "Orders/all(o: isof(o/Customer, 'AstoriaUnitTests.Stubs.CustomerWithBirthday') and cast(o/Customer, 'AstoriaUnitTests.Stubs.CustomerWithBirthday')/Birthday gt 1911-04-22T15:20:45.907Z)",
                                "Orders/all(o: isof(o/Customer, 'AstoriaUnitTests.Stubs.CustomerWithBirthday') and cast(o/Customer, 'AstoriaUnitTests.Stubs.CustomerWithBirthday')/Orders/any())",
                                "isof(Orders/any(),'Edm.Boolean') and ID eq 1",
                                "isof(Orders/any(o: $it/ID eq 2),'Edm.Boolean') and isof(Orders/all(o: $it/ID eq 2),'Edm.Boolean') and ID eq 1",

                            };

                    foreach (var filter in filters)
                    {
                        request.RequestUriString = "/Customers?$format=atom&$filter=" + filter;
                        Exception e = TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNull(e, "Not expecting exception.");

                        var xdoc = request.GetResponseStreamAsXDocument();
                        XElement customerWithBirthday = xdoc.Root.Elements(XName.Get("{http://www.w3.org/2005/Atom}entry")).Single();
                        XElement typeName = customerWithBirthday.Elements(XName.Get("{http://www.w3.org/2005/Atom}category")).Single();
                        Assert.IsTrue(typeName.Attribute("term").Value.EndsWith("CustomerWithBirthday"), "typeName.Attribute(\"term\").Value.EndsWith(\"CustomerWithBirthday\")");
                    }
                }
            }
        }

        [TestMethod]
        public void QueryTestFilterErrorCases()
        {
            // variable starting with $it
            TestFilterError("Orders/any(o: $its/ID eq 1)", "Could not find a property named '$its' on type 'AstoriaUnitTests.Stubs.Customer'.");

            // range variable outside 'any()'
            TestFilterError("Orders/any(o: $it/ID eq 1) and o/ID eq 1", "Could not find a property named 'o' on type 'AstoriaUnitTests.Stubs.Customer'.");

            // range variable outside 'any()'
            TestFilterError("o/ID eq 1 and Orders/any(o: $it/ID eq 1)", "Could not find a property named 'o' on type 'AstoriaUnitTests.Stubs.Customer'.");

            // 'all' without a predicate
            TestFilterError("Orders/all()", "The method 'all' without a supplied predicate is not supported.");

            // missing range variable identifier
            TestFilterError("Orders/all(: o/ID)", "An identifier was expected at position 11.");

            // missing range variable identifier
            TestFilterError("Orders/any($it/ID eq 2)", "The range variable '$it' has already been declared.");

            // missing range variable identifier
            TestFilterError("Orders/any(2 eq ID)", "An identifier was expected at position 11.");

            // missing range variable identifier
            TestFilterError("Orders/any(true)", "An identifier was expected at position 11.");

            // missing colon after range variable identifier
            TestFilterError("Orders/any(o(true)", "Syntax error at position 13 in 'Orders/any(o(true)'.");

            // missing colon after range variable identifier
            TestFilterError("Orders/any(o o/ID)", "Syntax error at position 14 in 'Orders/any(o o/ID)'.");

            // predicate must be boolean
            TestFilterError("Orders/all(o: o/ID)", "The Any/All query expression must evaluate to a single boolean value.");

            // undefined range variable.
            TestFilterError("Orders/all(o: A/Customer/Orders/any())", "Could not find a property named 'A' on type 'AstoriaUnitTests.Stubs.Customer'.");

            // re-defining in range variable.
            TestFilterError("Orders/all(o: o/Customer/Orders/any(o: o/ID eq 100))", "The range variable 'o' has already been declared.");
        }

        private static void TestFilterError(string filter, string expectedMessage)
        {
            using (OpenWebDataServiceHelper.AcceptAnyAllRequests.Restore())
            {
                OpenWebDataServiceHelper.ForceVerboseErrors = true;
                OpenWebDataServiceHelper.AcceptAnyAllRequests.Value = true;
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.StartService();

                    request.RequestUriString = "/Customers?$filter=" + filter;
                    Exception e = TestUtil.RunCatching(request.SendRequest);
                    Assert.IsNotNull(e, "Expecting exception.");
                    Assert.IsNotNull(e.InnerException, "Expecting exception.");

                    Assert.AreEqual(expectedMessage, e.InnerException.Message, "testCase.message == e.InnerException.Message");
                }
            }
        }

        public class CustomDataContextWithQueryInterception : OpenWebDataService<CustomDataContext>
        {
            [QueryInterceptor("Orders")]
            public Expression<Func<Order, bool>> NoOrdersForCustomer0()
            {
                return (o) => o.Customer.ID != 0;
            }
        }
        [Ignore] // Remove Atom
        // [TestMethod]
        public void TestAnyAllWithQueryInterceptor()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(CustomDataContextWithQueryInterception);
                request.StartService();

                var testCases = new[]
                {
                    new{
                        filter = "Orders/any()",
                        xpaths = new string[]{
                            "count(//atom:entry)=2",
                            "/atom:feed/atom:entry[atom:id='http://host/Customers(1)']",
                            "/atom:feed/atom:entry[atom:id='http://host/Customers(2)']",
                        }
                    },
                    new{
                        filter = "BestFriend/Orders/any()",
                        xpaths = new string[]{
                            "count(//atom:entry)=1",
                            "/atom:feed/atom:entry[atom:id='http://host/Customers(2)']",
                        }
                    },
                    new{
                        // use the same name for the parameter for the any() and QueryInterceptor
                        filter = "Orders/any(o: o/Customer/ID eq 1)",
                        xpaths = new string[]{
                            "count(//atom:entry)=1",
                            "/atom:feed/atom:entry[atom:id='http://host/Customers(1)']",
                        }
                    },
                    new{
                        filter = "Orders/any(o: isof(o/Customer, 'AstoriaUnitTests.Stubs.CustomerWithBirthday'))",
                        xpaths = new string[]{
                            "count(//atom:entry)=1",
                            "/atom:feed/atom:entry[atom:id='http://host/Customers(1)']",
                        }
                    },
                    new{
                        filter = "Orders/any(o: o/Customer/ID eq $it/ID)",
                        xpaths = new string[]{
                            "count(//atom:entry)=2",
                            "/atom:feed/atom:entry[atom:id='http://host/Customers(1)']",
                            "/atom:feed/atom:entry[atom:id='http://host/Customers(2)']",
                        }
                    },
                    new{
                        // since QueryInterceptor filters Orders for Customer 0, all() returns true even if predicate says it has to be 1.
                        filter = "Orders/all(o: o/Customer/ID eq 1)",
                        xpaths = new string[]{
                            "count(//atom:entry)=2",
                            "/atom:feed/atom:entry[atom:id='http://host/Customers(0)']",
                            "/atom:feed/atom:entry[atom:id='http://host/Customers(1)']",
                        }
                    },
                };

                TestUtil.RunCombinations(testCases, (testCase) =>
                         {
                             request.RequestUriString = "/Customers?$format=atom&$filter=" + testCase.filter;
                             request.SendRequest();
                             var response = request.GetResponseStreamAsXDocument();
                             UnitTestsUtil.VerifyXPaths(response, testCase.xpaths);
                         });
            }
        }

        [TestMethod]
        public void AnyAllVersioningTests()
        {
            using (OpenWebDataServiceHelper.AcceptAnyAllRequests.Restore())
            {
                OpenWebDataServiceHelper.ForceVerboseErrors = true;
                TestUtil.RunCombinations(UnitTestsUtil.BooleanValues, (acceptAnyAllRequests) =>
                {
                    OpenWebDataServiceHelper.AcceptAnyAllRequests.Value = acceptAnyAllRequests;
                    using (TestUtil.MetadataCacheCleaner())
                    using (TestWebRequest request =TestWebRequest.CreateForInProcess())
                    {
                        request.DataServiceType = typeof(CustomDataContext);

                        request.StartService();
                        request.RequestUriString = "/Customers?$filter=Orders/any()";
                        TestUtil.RunCombinations(ServiceVersion.ValidVersions,(requestVersion) =>
                        {
                            if (requestVersion != null)
                            {
                                request.RequestVersion = requestVersion.ToString();
                            }
                            else
                            {
                                request.RequestVersion = null;
                            }

                            Exception e = TestUtil.RunCatching(request.SendRequest);

                            if (!acceptAnyAllRequests)
                            {
                                Assert.IsNotNull(e);
                                Assert.IsInstanceOfType(e.InnerException, typeof(DataServiceException));
                                Assert.AreEqual(400, ((DataServiceException)(e.InnerException)).StatusCode);
                                Assert.AreEqual(DataServicesResourceUtil.GetString("RequestQueryParser_DisallowMemberAccessForResourceSetReference", "any", 7), e.InnerException.Message);
                            }
                            else
                            {
                                Assert.IsNull(e);
                            }
                        });
                    }
                });
            }
        }
    }
}