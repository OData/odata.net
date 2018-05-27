//---------------------------------------------------------------------
// <copyright file="SelectTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region usings
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using providers = Microsoft.OData.Service.Providers;
    using ocs = AstoriaUnitTests.ObjectContextStubs;

    #endregion

    #region Extensions
    public static class Extensions
    {
        public static IEnumerable<T[]> Combinations<T>(this IList<T> source)
        {
            for (int length = 0; length <= source.Count; length++)
            {
                foreach (var result in Combinations(source, length))
                {
                    yield return result;
                }
            }
        }

        public static IEnumerable<T[]> Combinations<T>(this IList<T> source, int length)
        {
            int[] stack = new int[length];
            for (int i = 0; i < length; i++) stack[i] = i;

            T[] selected = new T[length];
            int total = source.Count;
            while (true)
            {
                for (int i = 0; i < length; i++)
                {
                    selected[i] = source[stack[i]];
                }
                yield return selected.ToArray();
                int ti;
                for (ti = length - 1; ti >= 0; ti--)
                {
                    stack[ti]++;
                    bool found = true;
                    for (int j = ti; j < length; j++)
                    {
                        stack[j] = stack[ti] + (j - ti);
                        if (stack[j] >= total)
                        {
                            found = false;
                            break;
                        }
                    }
                    if (found)
                    {
                        break;
                    }
                }
                if (ti < 0) break;
            }
        }

        public static IEnumerable<T[]> Variations<T>(this IList<T> source, int length)
        {
            List<int> stack = new List<int>();
            int xxx = 1;

            int position = 0;
            T[] selected = new T[length];
            int start = 0;
            switch (xxx)
            {
                case 1:
                    for (int i = start; i < source.Count; i++)
                    {
                        if (stack.Contains(i)) continue;
                        selected[position] = source[i];
                        if (position == (length - 1))
                        {
                            yield return selected.ToArray();
                        }
                        else
                        {
                            position++;
                            stack.Insert(0, i);
                            start = 0;
                            goto case 1;
                        }
                    }
                    position--;
                    if (position >= 0)
                    {
                        start = stack[0]; stack.RemoveAt(0);
                        start++;
                        goto case 1;
                    }
                    break;
            }
        }

        public static IEnumerable<T[]> Permutations<T>(this IList<T> source)
        {
            return source.Variations(source.Count);
        }
    }
    #endregion

    [TestModule]
    public partial class UnitTestModule : AstoriaTestModule
    {
        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
        [Ignore] // Remove Atom
        // [TestClass, TestCase]
        public class SelectTest : AstoriaTestCase
        {
            [TestMethod, Variation]
            public void Projections_Parser_SelectOptionOccurence()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    VerifyQueryValid(request, "/Customers");
                    VerifyQueryValid(request, "/Customers?$select=");
                    VerifyQueryInvalid(request, "/Customers?$select");
                    VerifyQueryInvalid(request, "/Customers?$select=ID&$select=ID");

                    VerifyQueryInvalid(request, "/Customers(1)/ID?$select=ID", UnitTestsUtil.MimeApplicationXml, 400);
                    VerifyQueryInvalid(request, "/Customers(1)/Address?$select=City", UnitTestsUtil.MimeApplicationXml, 400);
                    VerifyQueryInvalid(request, "/Customers(1)/Orders/$ref?$select=ID", UnitTestsUtil.AtomFormat, 400);
                    VerifyQueryInvalid(request, "/Customers(1)/Orders(1)/OrderDetails/$ref?$select=ID", UnitTestsUtil.AtomFormat, 400);
                }
            }

            [TestMethod, Variation]
            public void Projections_Parser_BasicParsing()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = typeof(CustomDataContext);

                    VerifyQueryValid(request, "/Customers?$select=");
                    VerifyQueryValid(request, "/Customers?$select=  ");
                    VerifyQueryValid(request, "/Customers?$select=ID");
                    VerifyQueryValid(request, "/Customers?$select=ID,Name");
                    VerifyQueryValid(request, "/Customers?$select=ID,ID");
                    VerifyQueryValid(request, "/Customers?$select=Orders");
                    VerifyQueryValid(request, "/Customers?$expand=Orders&$select=Orders");
                    VerifyQueryValid(request, "/Customers?$select=BestFriend");
                    VerifyQueryValid(request, "/Customers?$expand=BestFriend&$select=BestFriend");
                    VerifyQueryValid(request, "/Customers?$select=Address");
                    VerifyQueryValid(request, "/Customers?$select=*");
                    VerifyQueryValid(request, "/Customers?$select=  ID  , Name ");
                    VerifyQueryValid(request, "/Customers?$select=%20%20ID%20,%20Name");
                    VerifyQueryValid(request, "/Customers?$select=ID,");

                    VerifyQueryInvalid(request, "/Customers?$select=,");
                    VerifyQueryInvalid(request, "/Customers?$select=  ,  ");
                    VerifyQueryInvalid(request, "/Customers?$select=ID,,Name");
                    VerifyQueryInvalid(request, "/Customers?$select=/ID");
                    VerifyQueryInvalid(request, "/Customers?$select=/");
                    VerifyQueryInvalid(request, "/Customers?$select=*/ID");
                    VerifyQueryInvalid(request, "/Customers?$select=Orders/");
                    VerifyQueryInvalid(request, "/Customers?$select=ID/");
                }
            }

            [TestMethod, Variation]
            public void Projections_Parser_NavigationPropertyTraversal()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = typeof(CustomDataContext);

                    VerifyQueryValid(request, "/Customers?$expand=Orders($select=ID)&$select=Orders");
                    VerifyQueryValid(request, "/Customers?$expand=Orders($select=*)&$select=Orders");
                    VerifyQueryValid(request, "/Customers?$expand=BestFriend($expand=BestFriend($expand=BestFriend($select=Name)))&$select=BestFriend");

                    VerifyQueryInvalid(request, "/Customers?$select=NonExistant");
                    VerifyQueryInvalid(request, "/Customers?$select=Name/Name");
                    VerifyQueryInvalid(request, "/Customers?$select=Address/*");
                    VerifyQueryInvalid(request, "/Customers?$select=Orders&$expand=Orders($select=ID");
                    VerifyQueryInvalid(request, "/Customers?$select=Orders($select=ID)&$expand=Orders");
                }
            }

            [TestMethod, Variation]
            public void Projections_Parser_NavigationPropertyTraversalOnOpenTypes()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.DataServiceType = typeof(CustomRowBasedOpenTypesContext);

                    VerifyQueryValid(request, "/Customers?$select=ID");  // This is not an open property
                    VerifyQueryValid(request, "/Customers?$select=*");
                    VerifyQueryValid(request, "/Customers?$select=Name"); // This is an open property

                    VerifyQueryInvalid(request, "/Customers?$select=ID/ID");
                }
            }

            private void VerifyQueryValid(TestWebRequest request, string requestUri)
            {
                request.RequestUriString = requestUri;
                request.SendRequest();
            }

            private void VerifyQueryInvalid(TestWebRequest request, string requestUri)
            {
                VerifyQueryInvalid(request, requestUri, 400);
            }

            private void VerifyQueryInvalid(TestWebRequest request, string requestUri, int statusCode)
            {
                VerifyQueryInvalid(request, requestUri, UnitTestsUtil.AtomFormat, statusCode);
            }

            private void VerifyQueryInvalid(TestWebRequest request, string requestUri, string format, int statusCode)
            {
                request.RequestUriString = requestUri;
                request.Accept = format;
                Exception exception = TestUtil.RunCatching(request.SendRequest);
                TestUtil.AssertExceptionStatusCode(exception, statusCode, "Expected invalid query.");
            }

            #region Old IExpandProvider
            public class OldExpandProvider : IExpandProvider
            {
                public enum Modifications
                {
                    None,
                    AddOrders,
                    RemoveOrders
                };
                public static Modifications[] AllModifications = new Modifications[] { Modifications.None, Modifications.AddOrders, Modifications.RemoveOrders };
                public static Modifications Modification = Modifications.None;

                public static bool ExpandedResultUsed = false;

                public IEnumerable ApplyExpansions(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths)
                {
                    return OldExpandProvider.ApplyExpansionsImpl(queryable, expandPaths);
                }

                public static IEnumerable ApplyExpansionsImpl(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths)
                {
                    switch (Modification)
                    {
                        case Modifications.AddOrders:
                            ExpandSegmentCollection c = new ExpandSegmentCollection();
                            c.Add(new ExpandSegment("Orders", null));
                            expandPaths.Add(c);
                            break;

                        case Modifications.RemoveOrders:
                            expandPaths.First().RemoveAt(0);
                            break;

                        default:
                            break;
                    }

                    return new CustomEnumerable(queryable);
                }

                public class CustomEnumerable : IEnumerable
                {
                    private IEnumerable source;
                    public CustomEnumerable(IEnumerable source) { this.source = source; }

                    public IEnumerator GetEnumerator()
                    {
                        return new CustomEnumerator(this.source.GetEnumerator());
                    }
                }

                public class CustomEnumerator : IEnumerator, IExpandedResult
                {
                    private IEnumerator source;
                    public CustomEnumerator(IEnumerator source) { this.source = source; }

                    public object Current
                    {
                        get { return source.Current; }
                    }

                    public bool MoveNext()
                    {
                        return source.MoveNext();
                    }

                    public void Reset()
                    {
                        source.Reset();
                    }

                    public object ExpandedElement
                    {
                        get { return this.Current; }
                    }

                    public object GetExpandedPropertyValue(string name)
                    {
                        OldExpandProvider.ExpandedResultUsed = true;
                        PropertyInfo pi = this.Current.GetType().GetProperty(name);
                        if (pi == null && this.Current is RowComplexType)
                        {
                            RowComplexType r = this.Current as RowComplexType;
                            if (r.Properties.ContainsKey(name))
                            {
                                return r.Properties[name];
                            }
                            else
                            {
                                return null;
                            }
                        }
                        return pi == null ? null : pi.GetValue(this.Current, null);
                    }
                }
            }


            public class OldExpandCustomDataContext : CustomDataContext, IExpandProvider
            {
                public IEnumerable ApplyExpansions(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths)
                {
                    return OldExpandProvider.ApplyExpansionsImpl(queryable, expandPaths);
                }
            }

            public class OldExpandCustomObjectContext : ocs.CustomObjectContext, IExpandProvider
            {
                public IEnumerable ApplyExpansions(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths)
                {
                    return OldExpandProvider.ApplyExpansionsImpl(queryable, expandPaths);
                }
            }

            [TestMethod, Variation]
            public void Projections_OldExpandProvider()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("ServiceType", new Type[] { 
                        typeof(OpenWebDataService<CustomRowBasedContext>),
                        typeof(OldExpandCustomDataContext),
                        typeof(OldExpandCustomObjectContext)}),
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                using (TestUtil.MetadataCacheCleaner())
                using (ocs.PopulateData.CreateTableAndPopulateData())
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
                using (TestUtil.RestoreStaticValueOnDispose(typeof(OldExpandProvider), "Modification"))
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                    {
                        OpenWebDataServiceHelper.GetServiceCustomizer.Value = type =>
                        {
                            if (type == typeof(IExpandProvider)) return new OldExpandProvider();
                            else return null;
                        };

                        request.DataServiceType = (Type)values["ServiceType"];
                        string format = (string)values["Format"];

                        // Verify that projections will fail with old provider
                        VerifyQueryInvalid(request, "/Customers?$select=ID", 500);

                        // Verify that no changes will give us orders
                        OldExpandProvider.Modification = OldExpandProvider.Modifications.None;
                        OldExpandProvider.ExpandedResultUsed = false;
                        var response = UnitTestsUtil.GetResponseAsAtom(request, "/Customers?$expand=Orders", format);
                        UnitTestsUtil.VerifyXPathExists(response, "//atom:entry//atom:entry[contains(atom:id,'Orders')]");
                        Assert.IsTrue(OldExpandProvider.ExpandedResultUsed, "The IExpandedResult on enumerator should have been used.");

                        // Verify that we can add more expansions
                        OldExpandProvider.Modification = OldExpandProvider.Modifications.AddOrders;
                        OldExpandProvider.ExpandedResultUsed = false;
                        response = UnitTestsUtil.GetResponseAsAtom(request, "/Customers?$expand=BestFriend", format);
                        UnitTestsUtil.VerifyXPathExists(response, "//atom:entry//atom:entry[contains(atom:id,'Orders')]");
                        Assert.IsTrue(OldExpandProvider.ExpandedResultUsed, "The IExpandedResult on enumerator should have been used.");

                        // Verify that we can remove expansions
                        OldExpandProvider.Modification = OldExpandProvider.Modifications.RemoveOrders;
                        OldExpandProvider.ExpandedResultUsed = false;
                        response = UnitTestsUtil.GetResponseAsAtom(request, "/Customers?$expand=Orders", format);
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry//atom:entry[contains(atom:id,'Orders')]");
                        Assert.IsTrue(OldExpandProvider.ExpandedResultUsed, "The IExpandedResult on enumerator should have been used.");

                        OldExpandProvider.ExpandedResultUsed = false;
                        UnitTestsUtil.VerifyInvalidRequest(null, "/Customers?$select=ID", request.ServiceType, format, "GET", 500, DataServicesResourceUtil.GetString("DataService_Projections_ProjectionsWithOldExpandProvider"));

                        #region Test Expand With Top Level Paging with custom IExpandProvider
                        OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) =>
                            {
                                config.SetEntitySetPageSize("Customers", 1);
                            };

                        TestUtil.ClearConfiguration();

                        // Expands are not allowed with custom IExpandProvider with paging enabled for top level entity sets
                        UnitTestsUtil.VerifyInvalidRequest(null, "/Customers?$expand=Orders", request.ServiceType, format, "GET", 500, DataServicesResourceUtil.GetString("DataService_SDP_TopLevelPagedResultWithOldExpandProvider"));
                        OpenWebDataServiceHelper.PageSizeCustomizer.Value = null;
                        TestUtil.ClearConfiguration();
                        #endregion
                    });
                }
            }
            #endregion

            [TestMethod, Variation]
            public void Projections_InternalIProjectionProvider()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }));

                using (TestUtil.MetadataCacheCleaner())
                using (TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
                using (ocs.PopulateData.CreateTableAndPopulateData())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    OpenWebDataServiceHelper.GetServiceCustomizer.Value = (type) =>
                        {
                            Assert.AreNotEqual("IProjectionProvider", type.Name, "We should never ask for internal interface.");
                            return null;
                        };

                    TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                    {
                        request.DataServiceType = (Type)values["DataServiceType"];
                        request.Accept = "application/atom+xml,application/xml";

                        UnitTestsUtil.GetResponseAsAtom(request, "/Customers?$select=ID");
                        UnitTestsUtil.GetResponseAsAtom(request, "/Customers?$expand=Orders");
                    });
                }
            }

            #region BigObjectDataSource

            public class BigObject
            {
                public int ID { get; set; }

                public int IntProperty1 { get; set; }
                public int IntProperty2 { get; set; }
                public int IntProperty3 { get; set; }
                public int IntProperty4 { get; set; }
                public int IntProperty5 { get; set; }
                public int IntProperty6 { get; set; }
                public int IntProperty7 { get; set; }
                public int IntProperty8 { get; set; }
                public int IntProperty9 { get; set; }
                public int IntProperty10 { get; set; }

                public string StringProperty1 { get; set; }
                public string StringProperty2 { get; set; }
                public string StringProperty3 { get; set; }
                public string StringProperty4 { get; set; }
                public string StringProperty5 { get; set; }
                public string StringProperty6 { get; set; }
                public string StringProperty7 { get; set; }
                public string StringProperty8 { get; set; }
                public string StringProperty9 { get; set; }
                public string StringProperty10 { get; set; }

                public BigObject Another { get; set; }
            }

            public class BigObjectDataSource
            {
                private static List<BigObject> _bigObjects;

                static BigObjectDataSource()
                {
                    _bigObjects = new List<BigObject>();

                    _bigObjects.Add(new BigObject()
                    {
                        ID = 1,
                        IntProperty1 = 1,
                        IntProperty2 = 2,
                        IntProperty3 = 3,
                        IntProperty4 = 4,
                        IntProperty5 = 5,
                        IntProperty6 = 6,
                        IntProperty7 = 7,
                        IntProperty8 = 8,
                        IntProperty9 = 9,
                        IntProperty10 = 10,
                        StringProperty1 = "one",
                        StringProperty2 = "two",
                        StringProperty3 = "three",
                        StringProperty4 = "four",
                        StringProperty5 = "five",
                        StringProperty6 = "six",
                        StringProperty7 = "seven",
                        StringProperty8 = "eight",
                        StringProperty9 = "nine",
                        StringProperty10 = "ten",
                    });
                    _bigObjects.Add(new BigObject()
                    {
                        ID = 2,
                        IntProperty1 = 21,
                        IntProperty2 = 22,
                        IntProperty3 = 23,
                        IntProperty4 = 24,
                        IntProperty5 = 25,
                        IntProperty6 = 26,
                        IntProperty7 = 27,
                        IntProperty8 = 28,
                        IntProperty9 = 29,
                        IntProperty10 = 30,
                        StringProperty1 = "second one",
                        StringProperty2 = "second two",
                        StringProperty3 = "second three",
                        StringProperty4 = "second four",
                        StringProperty5 = "second five",
                        StringProperty6 = "second six",
                        StringProperty7 = "second seven",
                        StringProperty8 = "second eight",
                        StringProperty9 = "second nine",
                        StringProperty10 = "second ten",
                    });
                    _bigObjects[0].Another = _bigObjects[1];
                }

                public IQueryable<BigObject> BigObjects
                {
                    get { return _bigObjects.AsQueryable(); }
                }
            }

            #endregion

            #region ExpressionTreeTests
#if DEBUG
            [TestMethod, Variation, Conditional("DEBUG")]
            public void Projections_ProjectedWrapperCreation()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("NumberOfPropertiesToProject", new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20 }),
                    new Dimension("UseExpansion", new bool[] { false, true }));

                TestUtil.RunCombinatorialEngineFail(engine, values =>
                {
                    int numberOfPropertiesToProject = (int)values["NumberOfPropertiesToProject"];
                    bool useExpansion = (bool)values["UseExpansion"];
                    StringBuilder sb = new StringBuilder();
                    if (useExpansion)
                    {
                        sb.Append("/BigObjects?$select=Another&$expand=Another($select=ID)");
                    }
                    else
                    {
                        sb.Append("/BigObjects?$select=ID");
                    }

                    for (int i = 0; i < numberOfPropertiesToProject && i < 10; i++)
                    {
                        if (useExpansion)
                        {
                            sb.AppendFormat(",Another($select=IntProperty{0})", i + 1);
                        }
                        else
                        {
                            sb.AppendFormat(",IntProperty{0}", i + 1);
                        }
                    }

                    for (int i = 0; i + 10 < numberOfPropertiesToProject && i < 10; i++)
                    {
                        if (useExpansion)
                        {
                            sb.AppendFormat(",Another($select=StringProperty{0})", i + 1);
                        }
                        else
                        {
                            sb.AppendFormat(",StringProperty{0}", i + 1);
                        }
                    }

                    var e = ExpressionTreeTestUtils.CreateRequestAndGetExpressionTreeXml(typeof(BigObjectDataSource), sb.ToString());
                    string expectedType;
                    string projectedWrapperType;
                    if (numberOfPropertiesToProject <= 7) // + 1 for ID
                    {
                        projectedWrapperType = string.Format("ProjectedWrapper{0}", numberOfPropertiesToProject + 1);
                    }
                    else
                    {
                        projectedWrapperType = "ProjectedWrapperMany";
                    }

                    if (!useExpansion)
                    {
                        expectedType = string.Format("IQueryable[{0}]", projectedWrapperType);
                    }
                    else
                    {
                        expectedType = string.Format("IQueryable[ExpandedWrapper[ProjectedWrapper1, {0}]]", projectedWrapperType);
                    }
                    UnitTestsUtil.VerifyXPathExists(e, "/node()[@type='" + expectedType + "']");
                });
            }

            [TestMethod, Variation, Conditional("DEBUG")]
            public void Projections_SimpleProjectionExpressions()
            {
                var e = ExpressionTreeTestUtils.CreateRequestAndGetExpressionTreeXml(
                    typeof(CustomDataContext),
                    "/Customers?$select=Name");
                UnitTestsUtil.VerifyXPathExists(e, "/node()[@type='IQueryable[ProjectedWrapper3]']"); // We projected Name, ID is Key and Guid is ETag -> 3 properties
                // Verify that we correctly project the PropertyNameList
                UnitTestsUtil.VerifyXPathExists(e,
                    "//MemberInit[@type='ProjectedWrapper3']/MemberAssignment[@member='PropertyNameList' and Constant='Name,ID,GuidValue']");
                // Verify that we correctly project the three type names
                UnitTestsUtil.VerifyXPathResultCount(e, 1,
                    "//MemberInit[@type='ProjectedWrapper3']/MemberAssignment[@member='ResourceTypeName'][Constant='AstoriaUnitTests.Stubs.Customer']",
                    "//MemberInit[@type='ProjectedWrapper3']/MemberAssignment[@member='ResourceTypeName'][Constant='AstoriaUnitTests.Stubs.CustomerWithBirthday']",
                    "//MemberInit[@type='ProjectedWrapper3']/MemberAssignment[@member='ResourceTypeName'][Constant='AstoriaUnitTests.Stubs.CustomerWithoutProperties']");
                // Verify that we project ID->0, Name->1, GuidValue->2
                UnitTestsUtil.VerifyXPathResultCount(e, 3,
                    "//MemberInit[@type='ProjectedWrapper3']/MemberAssignment[@member='ProjectedProperty0']//MemberAccess[Member='Name']",
                    "//MemberInit[@type='ProjectedWrapper3']/MemberAssignment[@member='ProjectedProperty1']//MemberAccess[Member='ID']",
                    "//MemberInit[@type='ProjectedWrapper3']/MemberAssignment[@member='ProjectedProperty2']//MemberAccess[Member='GuidValue']");
            }

            [TestMethod, Variation, Conditional("DEBUG")]
            public void Projections_TestProjectionCastsInLinqToObjects()
            {
                var e = ExpressionTreeTestUtils.CreateRequestAndGetExpressionTreeXml(typeof(CustomDataContext), "/Customers?$select=ID,Name");
                // Verify that for reflection provider we do cast the projected properties to Object, otherwise Linq to Objects will produce invalid IL
                UnitTestsUtil.VerifyXPathResultCount(e, 3,
                    "//MemberInit[@type='ProjectedWrapper3']/MemberAssignment[@member='ProjectedProperty0']/Convert[@type='Object']",
                    "//MemberInit[@type='ProjectedWrapper3']/MemberAssignment[@member='ProjectedProperty1']/Convert[@type='Object']",
                    "//MemberInit[@type='ProjectedWrapper3']/MemberAssignment[@member='ProjectedProperty2']/Convert[@type='Object']");
                // Verify that when we access the Customer object we always cast it to a specific type
                UnitTestsUtil.VerifyXPathDoesntExist(e,
                    "//MemberExpression//Parameter[@type='Customer' and " +
                        "parent::TypeAs/@type!='CustomerWithoutProperties' and " +
                        "parent::TypeAs/@type!='CustomerWithBirthday' and " +
                        "parent::TypeAs/@type!='Customer']");
            }

            [TestMethod, Variation, Conditional("DEBUG")]
            public void Projections_TestNullProjectionInLinqToObjects()
            {
                var e = ExpressionTreeTestUtils.CreateRequestAndGetExpressionTreeXml(typeof(CustomDataContext), "/Customers?$expand=BestFriend($select=Name)");
                // Verify that we insert a p.BestFriend == null ? null : new ProjectedWrapper into the expanded wrapper property.
                // Linq to Objects has NullPropagationRequired = true, so it needs to perform the null check before it starts projecting properties.
                
                // And verify it doesn't contain the ResourceTypeName hack (as it's not needed in this case)
                UnitTestsUtil.VerifyXPathDoesntExist(e,
                    "//MemberInit[contains(@type,'ExpandedWrapper')]/MemberAssignment[@member='ProjectedProperty0']/" +
                        "Conditional/Test/Equal/Left/MemberAccess[Member='BestFriend' and MemberExpression/Parameter]", 
                    "//MemberInit[contains(@type,'ExpandedWrapper')]/MemberAssignment[@member='ProjectedProperty0']/" +
                        "Conditional/True[Constant='null' and contains(Constant/@type, 'ProjectedWrapper')]",
                    "//MemberInit[contains(@type,'ExpandedWrapper')]/MemberAssignment[@member='ProjectedProperty0']//" +
                        "MemberInit[contains(@type,'ProjectedWrapper')]/MemberAssignment[@member='ResourceTypeName']/" +
                        "Conditional/True[Constant/@type='String' and string-length(Constant)=0]",
                    "//MemberInit[contains(@type,'ExpandedWrapper')]/MemberAssignment[@member='ProjectedProperty0']//" +
                        "MemberInit[contains(@type,'ProjectedWrapper')]/MemberAssignment[@member='ResourceTypeName']/" +
                        "Conditional/False[Constant/@type='String' and string-length(Constant)!=0]",
                    "//MemberInit[contains(@type,'ExpandedWrapper')]/MemberAssignment[@member='ProjectedProperty0']//" +
                        "MemberInit[contains(@type,'ProjectedWrapper')]/MemberAssignment[@member='ResourceTypeName']/" +
                        "Conditional/Test/Equal/Left/MemberAccess[Member='BestFriend' and MemberExpression/Parameter]",
                    "//MemberInit[contains(@type,'ExpandedWrapper')]/MemberAssignment[@member='ProjectedProperty0']//" +
                        "MemberInit[contains(@type,'ProjectedWrapper')]/MemberAssignment[@member='ResourceTypeName']/" +
                        "Conditional/Test/Equal/Right[Constant='null' and Constant/@type='Customer']",
                    "//MemberInit[contains(@type,'ExpandedWrapper')]/MemberAssignment[@member='ProjectedProperty0']/" +
                        "Conditional/Test/Equal/Right[Constant='null' and Constant/@type='Customer']");
            }

            [TestMethod, Variation, Conditional("DEBUG")]
            public void Projections_TestNullProjectionInLinqToEntities()
            {
                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    var e = ExpressionTreeTestUtils.CreateRequestAndGetExpressionTreeXml(typeof(ocs.CustomObjectContext), "/Customers?$select=BestFriend&$expand=BestFriend($select=Name)");
                    // Verify that we insert a p.BestFriend == null ? "" : "Customer" into the ResourceTypeName property of the ProjectedWrapper.
                    // Linq to Entities has NullPropagationRequires = false, so it can't project the null directly, instead it uses
                    //   the trick with empty type name.
                    UnitTestsUtil.VerifyXPathExists(e,
                        "//MemberInit[contains(@type,'ExpandedWrapper')]/MemberAssignment[@member='ProjectedProperty0']//" +
                            "MemberInit[contains(@type,'ProjectedWrapper')]/MemberAssignment[@member='ResourceTypeName']/" +
                            "Conditional/True[Constant/@type='String' and string-length(Constant)=0]",
                        "//MemberInit[contains(@type,'ExpandedWrapper')]/MemberAssignment[@member='ProjectedProperty0']//" +
                            "MemberInit[contains(@type,'ProjectedWrapper')]/MemberAssignment[@member='ResourceTypeName']/" +
                            "Conditional/False[Constant/@type='String' and string-length(Constant)!=0]",
                        "//MemberInit[contains(@type,'ExpandedWrapper')]/MemberAssignment[@member='ProjectedProperty0']//" +
                            "MemberInit[contains(@type,'ProjectedWrapper')]/MemberAssignment[@member='ResourceTypeName']/" +
                            "Conditional/Test/Equal/Left/MemberAccess[Member='BestFriend' and MemberExpression/Parameter]",
                        "//MemberInit[contains(@type,'ExpandedWrapper')]/MemberAssignment[@member='ProjectedProperty0']//" +
                            "MemberInit[contains(@type,'ProjectedWrapper')]/MemberAssignment[@member='ResourceTypeName']/" +
                            "Conditional/Test/Equal/Right[Constant='null' and Constant/@type='Customer']");
                    // And verify it doesn't contain the projection of null directly, as EF won't be able to process it
                    UnitTestsUtil.VerifyXPathDoesntExist(e,
                        "//MemberInit[contains(@type,'ExpandedWrapper')]/MemberAssignment[@member='ProjectedProperty0']/" +
                            "Conditional/True[Constant='null' and contains(Constant/@type, 'ProjectedWrapper')]",
                        "//MemberInit[contains(@type,'ExpandedWrapper')]/MemberAssignment[@member='ProjectedProperty0']/" +
                            "Conditional/Test/Equal/Left/MemberAccess[Member='BestFriend' and MemberExpression/Parameter]",
                        "//MemberInit[contains(@type,'ExpandedWrapper')]/MemberAssignment[@member='ProjectedProperty0']/" +
                            "Conditional/Test/Equal/Right[Constant='null' and Constant/@type='Customer']");
                }
            }

            [TestMethod, Variation, Conditional("DEBUG")]
            public void Projections_TestProjectionCastsInLinqToEntities()
            {
                using (ocs.PopulateData.CreateTableAndPopulateData())
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    var e = ExpressionTreeTestUtils.CreateRequestAndGetExpressionTreeXml(typeof(ocs.CustomObjectContext), "/Customers?$select=ID,Name");
                    // Verify that for EF provider we do NOT cast the projected properties to Object, since Linq to Entities
                    //   might fail as it supports only casts for simple types
                    UnitTestsUtil.VerifyXPathDoesntExist(e,
                        "//MemberInit[@type='ProjectedWrapper4']/MemberAssignment[@member='ProjectedProperty0']//Convert[@type='Object']",
                        "//MemberInit[@type='ProjectedWrapper4']/MemberAssignment[@member='ProjectedProperty1']//Convert[@type='Object']",
                        "//MemberInit[@type='ProjectedWrapper4']/MemberAssignment[@member='ProjectedProperty2']//Convert[@type='Object']",
                        "//MemberInit[@type='ProjectedWrapper4']/MemberAssignment[@member='ProjectedProperty3']//Convert[@type='Object']");
                    // Verify that when we access the Customer object we always cast it to a specific type
                    UnitTestsUtil.VerifyXPathDoesntExist(e,
                        "//MemberExpression//Parameter[@type='Customer' and " +
                            "parent::TypeAs/@type!='CustomerWithBirthday' and " +
                            "parent::TypeAs/@type!='Customer']");
                }
            }

            [TestMethod, Variation, Conditional("DEBUG")]
            public void Projections_TestProjectionOrderBy()
            {
                var e = ExpressionTreeTestUtils.CreateRequestAndGetExpressionTreeXml(typeof(CustomDataContext), "/Customers?$select=ID&$orderby=ID");
                // Verify that there's an OrderBy before Select (new shape of the query as it uses projections)
                UnitTestsUtil.VerifyXPathExists(e, "/Call[Method='Select']/Arguments/Call[Method='OrderBy']");
                // Verify that the OrderBy lamba is accessing the ID property and nothing else
                UnitTestsUtil.VerifyXPathExists(e, "/Call//Call[Method='OrderBy']/Arguments/*[2]//MemberAccess[MemberExpression/Parameter/@type='Customer' and Member='ID']");
                UnitTestsUtil.VerifyXPathDoesntExist(e, "/Call//Call[Method='OrderBy']/Arguments/*[2]//MemberAccess[MemberExpression/Parameter/@type='Customer' and Member!='ID']");
            }

            [TestMethod, Variation, Conditional("DEBUG")]
            public void Projections_SimpleIDSPProjections()
            {
                using (TestUtil.MetadataCacheCleaner())
                {
                    // Verify simple projection works
                    var doc = ExpressionTreeTestUtils.CreateRequestAndGetExpressionTreeXml(typeof(CustomRowBasedContext), "/Customers?$select=Name");
                    UnitTestsUtil.VerifyXPathExists(doc,
                        "//MemberAssignment[@member='ProjectedProperty0']//Call[Method/@type='DataServiceProviderMethods' and Method='GetValue' and Arguments/Constant='Name']");
                }
            }

            [TestMethod, Variation, Conditional("DEBUG")]
            public void Projections_SimpleIDSPExpansion()
            {
                using (TestUtil.MetadataCacheCleaner())
                {
                    // Verify that expansions are projected through ExpandedWrappers (we don't want to assume expand on demand for IDSPs)
                    var doc = ExpressionTreeTestUtils.CreateRequestAndGetExpressionTreeXml(typeof(CustomRowBasedContext), "/Customers?$expand=Orders");
                    UnitTestsUtil.VerifyXPathExists(doc,
                        "//MemberInit[contains(@type, 'ExpandedWrapper') and MemberAssignment[@member='Description']/Constant='Orders']");
                }
            }

            [TestMethod, Variation, Conditional("DEBUG")]
            public void Projections_SimpleOpenTypeProjetions()
            {
                using (TestUtil.MetadataCacheCleaner())
                {
                    var doc = ExpressionTreeTestUtils.CreateRequestAndGetExpressionTreeXml(typeof(CustomRowBasedOpenTypesContext), "/Customers?$select=Name");
                    UnitTestsUtil.VerifyXPathExists(doc,
                        "//MemberAssignment[@member='ProjectedProperty0']//Call[Method/@type='OpenTypeMethods' and Method='GetValue' and Arguments/Constant='Name']");
                }
            }
#endif

            #endregion

            [TestMethod, Variation]
            public void Projections_TopLevelKeyProperty()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataServiceType = (Type)values["DataServiceType"];
                        string format = (string)values["Format"];
                        string typePrefix = dataServiceType == typeof(ocs.CustomObjectContext) ?
                            "AstoriaUnitTests.ObjectContextStubs.Types" : "AstoriaUnitTests.Stubs";

                        var response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$select=ID", format);

                        VerifyETags(response);
                        // Verify total entry count
                        UnitTestsUtil.VerifyXPathResultCount(response, 3, "/atom:feed/atom:entry");
                        // Verify IDs
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed/atom:entry[contains(atom:id, 'Customers(0)')]",
                            "/atom:feed/atom:entry[contains(atom:id, 'Customers(1)')]",
                            "/atom:feed/atom:entry[contains(atom:id, 'Customers(2)')]");
                        // Verify types
                        UnitTestsUtil.VerifyXPathExists(response,
                            string.Format("//atom:entry[contains(atom:id, 'Customers(0)') and atom:category/@term='#{0}.Customer']", typePrefix),
                            string.Format("//atom:entry[contains(atom:id, 'Customers(1)') and atom:category/@term='#{0}.CustomerWithBirthday']", typePrefix),
                            string.Format("//atom:entry[contains(atom:id, 'Customers(2)') and atom:category/@term='#{0}.Customer']", typePrefix));
                        // Verify ID values
                        UnitTestsUtil.VerifyXPathExists(response,
                            "//atom:entry[contains(atom:id, 'Customers(0)') and atom:content/adsm:properties/ads:ID='0']",
                            "//atom:entry[contains(atom:id, 'Customers(1)') and atom:content/adsm:properties/ads:ID='1']",
                            "//atom:entry[contains(atom:id, 'Customers(2)') and atom:content/adsm:properties/ads:ID='2']");
                        // Verify entries have etags
                        if (dataServiceType != typeof(CustomRowBasedOpenTypesContext))
                        {
                            UnitTestsUtil.VerifyXPathResultCount(response, 3, "//atom:entry/@adsm:etag");
                        }
                        // Verify no other properties than ID are present
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//adsm:properties/*[local-name()!='ID']");
                        // Verify no links are present either
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry/atom:link[@rel!='edit']");
                    });
                }
            }

            [TestMethod, Variation]
            public void Projections_TopLevelPrimitiveProperty()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataServiceType = (Type)values["DataServiceType"];
                        string format = (string)values["Format"];

                        var response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$select=Name", format);
                        VerifyETags(response);
                        // Verify customer names
                        UnitTestsUtil.VerifyXPathExists(response,
                            "//ads:Name[node()='Customer 0']",
                            "//ads:Name[node()='Customer 1']",
                            "//ads:Name[node()='Customer 2']");
                        // Verify nothing else is present
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//adsm:properties/*[local-name()!='Name']");
                        // Verify no links are present either
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry/atom:link[@rel!='edit']");
                    });
                }
            }

            [TestMethod, Variation]
            public void Projections_TopLevelComplexProperty()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", new string[] { UnitTestsUtil.AtomFormat }));

                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataServiceType = (Type)values["DataServiceType"];
                        string format = (string)values["Format"];
                        string typePrefix = dataServiceType == typeof(ocs.CustomObjectContext) ?
                            "AstoriaUnitTests.ObjectContextStubs.Types" : "AstoriaUnitTests.Stubs";

                        var response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$select=Address", format);
                        VerifyETags(response);
                        // Verify total entry count
                        UnitTestsUtil.VerifyXPathResultCount(response, 3, "/atom:feed/atom:entry");
                        // Verify IDs
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed/atom:entry[contains(atom:id, 'Customers(0)')]",
                            "/atom:feed/atom:entry[contains(atom:id, 'Customers(1)')]",
                            "/atom:feed/atom:entry[contains(atom:id, 'Customers(2)')]");
                        // Verify types
                        UnitTestsUtil.VerifyXPathExists(response,
                            string.Format("//atom:entry[contains(atom:id, 'Customers(0)') and atom:category/@term='#{0}.Customer']", typePrefix),
                            string.Format("//atom:entry[contains(atom:id, 'Customers(1)') and atom:category/@term='#{0}.CustomerWithBirthday']", typePrefix),
                            string.Format("//atom:entry[contains(atom:id, 'Customers(2)') and atom:category/@term='#{0}.Customer']", typePrefix));
                        // Verify ID values
                        UnitTestsUtil.VerifyXPathExists(response,
                            "//atom:entry[contains(atom:id, 'Customers(0)') and atom:content/adsm:properties/ads:Address[ads:State='WA']]",
                            "//atom:entry[contains(atom:id, 'Customers(1)') and atom:content/adsm:properties/ads:Address[ads:State='WA']]",
                            "//atom:entry[contains(atom:id, 'Customers(2)') and atom:content/adsm:properties/ads:Address[ads:State='WA']]");
                        // Verify entries have etags
                        if (dataServiceType != typeof(CustomRowBasedOpenTypesContext))
                        {
                            UnitTestsUtil.VerifyXPathResultCount(response, 3, "//atom:entry/@adsm:etag");
                        }
                        // Verify no other properties than ID are present
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//adsm:properties/*[local-name()!='Address']");
                        // Verify no links are present either
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry/atom:link[@rel!='edit']");
                    });
                }
            }

            [TestMethod, Variation]
            public void Projections_TopLevelNavigationSetProperty()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataServiceType = (Type)values["DataServiceType"];
                        string format = (string)values["Format"];

                        var response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$select=Orders", format);
                        VerifyETags(response);
                        // Verify no content properties are present
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry/atom:content/adsm:properties/*");
                        // Verify the link to Orders exists
                        UnitTestsUtil.VerifyXPathExists(response, "//atom:entry/atom:link[@title='Orders']");
                        //Verify that the link to Orders is not expanded, since we only did a select to it
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry/atom:link[@title='Orders']/*");
                        // Verify no links are present either
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry/atom:link[@rel!='edit' and @title!='Orders']");
                    });
                }
            }

            [TestMethod, Variation]
            public void Projections_TopLevelNavigationReferenceProperty()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataServiceType = (Type)values["DataServiceType"];
                        string format = (string)values["Format"];
                        string uri = (string)values["Uri"];

                        var response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$select=BestFriend", format);

                        VerifyETags(response);
                        // Verify no content properties are present
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry/atom:content/adsm:properties/*");
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry/atom:link[@title='Orders']");
                        // Verify the link to Orders exists
                        UnitTestsUtil.VerifyXPathExists(response, "//atom:entry/atom:link[@title='BestFriend']");
                        //Verify that the link to Orders is not expanded, since we only did a select to it
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry/atom:link[@title='BestFriend']/*");
                        // Verify no links are present either
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry/atom:link[@rel!='edit' and @title!='BestFriend']");
                    });
                }
            }

            [TestMethod, Variation]
            public void Projections_SelectAllNonNavigationProperties1()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", new string [] { UnitTestsUtil.AtomFormat }),
                    new Dimension("Uri", new string[] { "/Customers?$select=*&$expand=Orders,BestFriend" }));

                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataServiceType = (Type)values["DataServiceType"];
                        string format = (string)values["Format"];
                        string uri = (string)values["Uri"];

                        var response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, uri, format);
                        VerifyETags(response);
                        // Verify both ID and Name are peresent as well as the link to Orders
                        UnitTestsUtil.VerifyXPathResultCount(response, 11,
                        "//atom:entry/atom:content/adsm:properties/ads:ID");

                        UnitTestsUtil.VerifyXPathResultCount(response, 5,
                        "//atom:entry/atom:content/adsm:properties/ads:Name");

                        UnitTestsUtil.VerifyXPathResultCount(response, 5,
                        "//atom:entry/atom:link[@title='Orders']");

                        //Verify that the link are unexpanded
                        UnitTestsUtil.VerifyXPathExists(response, "//atom:entry/atom:link/*");
                    });
                }
            }

            [TestMethod, Variation]
            public void Projections_SelectAllNonNavigationProperties2()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", new string[] { UnitTestsUtil.AtomFormat }),
                    new Dimension("Uri", new string[] { "/Customers?$select=*" }));

                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataServiceType = (Type)values["DataServiceType"];
                        string format = (string)values["Format"];
                        string uri = (string)values["Uri"];

                        var response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, uri, format);
                        VerifyETags(response);
                        // Verify both ID and Name are peresent as well as the link to Orders
                        UnitTestsUtil.VerifyXPathResultCount(response, 3,
                        "//atom:entry/atom:content/adsm:properties/ads:ID");

                        UnitTestsUtil.VerifyXPathResultCount(response, 3,
                        "//atom:entry/atom:content/adsm:properties/ads:Name");

                        UnitTestsUtil.VerifyXPathResultCount(response, 3,
                        "//atom:entry/atom:link[@title='Orders']");

                        //Verify that the link are unexpanded
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry/atom:link/*");
                    });
                }
            }

            [TestMethod, Variation]
            public void Projections_SimplePropertyFromNavigationSetProperty()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataServiceType = (Type)values["DataServiceType"];
                        string format = (string)values["Format"];

                        var response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$select=Orders&$expand=Orders($select=ID)", format);
                        VerifyETags(response);
                        // Verify no content properties are present on the Customer
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "/atom:feed/atom:entry/atom:content/adsm:properties/*");
                        // Verify no links are present either on the Customer
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "/atom:feed/atom:entry/atom:link[@rel!='edit' and @title!='Orders']");
                        // Verify that orders are inlined
                        UnitTestsUtil.VerifyXPathResultCount(response, 3, "/atom:feed/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed");
                        // Verify that each customer has 2 orders
                        UnitTestsUtil.VerifyXPathResultCount(response, 3, "/atom:feed/atom:entry/atom:link[@title='Orders'][count(adsm:inline/atom:feed/atom:entry)=2]");
                        // Verify that orders have their ID property
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry//atom:entry[count(atom:content/adsm:properties/ads:ID)=0]");
                        // Verify that orders don't have anything else than ID filled
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry//atom:entry//adsm:properties/*[local-name()!='ID']");
                        // Verify that orders don't have any links
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry//atom:entry/atom:link[@rel!='edit']");
                    });
                }
            }

            [TestMethod, Variation]
            public void Projections_SimplePropertyFromNavigationReferenceProperty()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", new string[] { UnitTestsUtil.AtomFormat }));

                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataServiceType = (Type)values["DataServiceType"];
                        string format = (string)values["Format"];

                        var response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$select=BestFriend&$expand=BestFriend($select=ID)", format);

                        VerifyETags(response);
                        // Verify that the first customer has the BestFriend inlined but empty (as it's null)
                        UnitTestsUtil.VerifyXPathExists(response,
                            format == UnitTestsUtil.AtomFormat ?
                                "/atom:feed/atom:entry[contains(atom:id,'Customers(0)')]/atom:link[@title='BestFriend']/adsm:inline[count(*)=0]" :
                                "/atom:feed/atom:entry[contains(atom:id,'Customers(0)')]/atom:content/adsm:properties[ads:BestFriend/@adsm:null='true']");

                        // Verify no content properties are present on the Customer
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "/atom:feed/atom:entry/atom:content/adsm:properties/*");
                        // Verify no links are present either on the Customer, except BestFriend
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "/atom:feed/atom:entry/atom:link[@rel!='edit' and @title!='BestFriend']");
                        // Verify that BestFriend are inlined
                        UnitTestsUtil.VerifyXPathResultCount(response, 2, "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline/atom:entry");
                        // Verify that BestFriend have their ID property
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry//atom:entry[count(atom:content/adsm:properties/ads:ID)=0]");
                        // Verify that BestFriend don't have anything else than ID filled
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry//atom:entry//adsm:properties/*[local-name()!='ID']");
                        // Verify that BestFriend don't have any links
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry//atom:entry/atom:link[@rel!='edit']");
                    });
                }
            }

            [TestMethod, Variation]
            public void Projections_ComplexPropertyFromNavigationReferenceProperty()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", new string[] { UnitTestsUtil.AtomFormat }));

                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataServiceType = (Type)values["DataServiceType"];
                        string format = (string)values["Format"];

                        var response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$select=BestFriend&$expand=BestFriend($select=Address)", format);

                        // Verify that the first customer has the BestFriend inlined but empty (as it's null)
                        UnitTestsUtil.VerifyXPathExists(response,
                            format == UnitTestsUtil.AtomFormat ?
                                "/atom:feed/atom:entry[contains(atom:id,'Customers(0)')]/atom:link[@title='BestFriend']/adsm:inline[count(*)=0]" :
                                "/atom:feed/atom:entry[contains(atom:id,'Customers(0)')]/atom:content/adsm:properties[ads:BestFriend/@adsm:null='true']");

                        VerifyETags(response);
                        // Verify no content properties are present on the Customer
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "/atom:feed/atom:entry/atom:content/adsm:properties/*");
                        // Verify no links are present either on the Customer, except BestFriend
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "/atom:feed/atom:entry/atom:link[@rel!='edit' and @title!='BestFriend']");
                        // Verify that BestFriend are inlined
                        UnitTestsUtil.VerifyXPathResultCount(response, 2, "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline/atom:entry");
                        // Verify that BestFriend have their ID property
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry//atom:entry[count(atom:content/adsm:properties/ads:Address)=0]");
                        // Verify that BestFriend don't have anything else than ID filled
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry//atom:entry//adsm:properties/*[local-name()!='Address']");
                        // Verify that BestFriend don't have any links
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry//atom:entry/atom:link[@rel!='edit']");
                    });
                }
            }

            [TestMethod, Variation]
            public void Projections_AllPropertiesFromNavigationSetProperty()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", UnitTestsUtil.ResponseFormats),
                    new Dimension("Uri", new string[] { "/Customers?$select=Orders&$expand=Orders", "/Customers?$select=Orders&$expand=Orders($select=*)" }));

                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataServiceType = (Type)values["DataServiceType"];
                        string format = (string)values["Format"];
                        string uri = (string)values["Uri"];

                        var response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, uri, format);

                        VerifyETags(response);
                        // Now orders should have DollarAmount as well
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry//atom:entry[count(.//ads:DollarAmount)=0]");
                        // Verify no content properties are present on the Customer
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "/atom:feed/atom:entry/atom:content/adsm:properties/*");
                        // Verify no links are present either on the Customer,except Orders
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "/atom:feed/atom:entry/atom:link[@rel!='edit' and @title!='Orders']");
                        // Verify that orders are inlined
                        UnitTestsUtil.VerifyXPathResultCount(response, 3, "/atom:feed/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed");
                        // Verify that each customer has 2 orders
                        UnitTestsUtil.VerifyXPathResultCount(response, 3, "/atom:feed/atom:entry/atom:link[@title='Orders'][count(adsm:inline/atom:feed/atom:entry)=2]");
                        // Verify that orders have their ID property
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry//atom:entry[count(atom:content/adsm:properties/ads:ID)=0]");
                    });
                }
            }

            [TestMethod, Variation]
            public void Projections_AllPropertiesFromNavigationReferenceProperty()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", new string[] { UnitTestsUtil.AtomFormat }),
                    new Dimension("Uri", new string[] { "/Customers?$select=BestFriend&$expand=BestFriend", "/Customers?$select=BestFriend&$expand=BestFriend($select=*)" }));

                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataServiceType = (Type)values["DataServiceType"];
                        string format = (string)values["Format"];
                        string uri = (string)values["Uri"];

                        var response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, uri, format);

                        VerifyETags(response);
                        // Verify no content properties are present on the Customer
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "/atom:feed/atom:entry/atom:content/adsm:properties/*");
                        // Verify no links are present either on the Customer,except BestFriend
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "/atom:feed/atom:entry/atom:link[@rel!='edit' and @title!='BestFriend']");
                        // Verify that BestFriend are inlined
                        UnitTestsUtil.VerifyXPathResultCount(response, 2, "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline/atom:entry");
                        // Verify that customers have best friend property (first one does not have, hence count = 
                        UnitTestsUtil.VerifyXPathResultCount(response, 2, "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline/atom:entry/atom:content/adsm:properties");
                    });
                }
            }

            [TestMethod, Variation]
            public void Projections_MultiLevelSelect()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", new string[] { UnitTestsUtil.AtomFormat }));

                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataServiceType = (Type)values["DataServiceType"];
                        string format = (string)values["Format"];

                        var response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$select=Name,Orders&$expand=Orders($select=ID)", format);

                        // Now customer should have Name
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "/atom:feed/atom:entry[count(.//ads:Name)=0]");
                        if (dataServiceType != typeof(CustomRowBasedOpenTypesContext))
                        {
                            // But no GuidValue for example
                            UnitTestsUtil.VerifyXPathDoesntExist(response, "/atom:feed/atom:entry/atom:content//ads:GuidValue");
                        }

                        VerifyETags(response);
                        // Customers should have no other properties other than Name
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "/atom:feed/atom:entry/adsm:properties/*[local-name()!='Name']");
                        // Verify no links are present either on the Customer, except orders
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "/atom:feed/atom:entry/atom:link[@rel!='edit' and @title!='Orders']");
                        // Verify that orders are inlined
                        UnitTestsUtil.VerifyXPathResultCount(response, 3, "/atom:feed/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed");
                        // Verify that each customer has 2 orders
                        UnitTestsUtil.VerifyXPathResultCount(response, 3, "/atom:feed/atom:entry/atom:link[@title='Orders'][count(adsm:inline/atom:feed/atom:entry)=2]");
                        // Verify that orders have their ID property
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry//atom:entry[count(atom:content/adsm:properties/ads:ID)=0]");
                        // Verify that orders don't have anything else than ID filled
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry//atom:entry//adsm:properties/*[local-name()!='ID']");
                        // Verify that orders don't have any links
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry//atom:entry/atom:link[@rel!='edit']");
                    });
                }
            }

            [TestMethod, Variation]
            public void Projections_ExpandedProjections()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(CustomRowBasedContext) }),
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataServiceType = (Type)values["DataServiceType"];
                        string format = (string)values["Format"];

                        var response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$select=Orders&$expand=Orders($expand=OrderDetails)", format);

                        VerifyETags(response);
                        // Verify no content properties are present on the Customer
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "/atom:feed/atom:entry/atom:content/adsm:properties/*");
                        // Verify no links are present either on the Customer,except Orders
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "/atom:feed/atom:entry/atom:link[@rel!='edit' and @title!='Orders']");
                        // Verify that orders are inlined
                        UnitTestsUtil.VerifyXPathResultCount(response, 3, "/atom:feed/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed");
                        // Verify that each customer has 2 orders
                        UnitTestsUtil.VerifyXPathResultCount(response, 3, "/atom:feed/atom:entry/atom:link[@title='Orders'][count(adsm:inline/atom:feed/atom:entry)=2]");
                        // Verify that both orders/Orderdetails should have all properties expanded
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry//atom:entry[count(atom:content/adsm:properties/*)=0]");
                        // Verify that orderdetails are expanded
                        UnitTestsUtil.VerifyXPathResultCount(response, 6, "/atom:feed/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:link[@title='OrderDetails']/adsm:inline/atom:feed");
                        
                    });
                }
            }

            [TestMethod, Variation]
            public void Projections_ManyProperties()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Format", UnitTestsUtil.ResponseFormats),
                    new Dimension("ProjectedPropertyCount", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }),
                    new Dimension("PropertyType", new string[] { "Int", "String" }));

                using (TestUtil.MetadataCacheCleaner())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        string format = (string)values["Format"];
                        int projectedPropertyCount = (int)values["ProjectedPropertyCount"];
                        string propertyType = (string)values["PropertyType"];

                        StringBuilder query = new StringBuilder();
                        query.Append("/BigObjects?$select=");
                        for (int i = 0; i < projectedPropertyCount; i++)
                        {
                            if (i > 0)
                            {
                                query.Append(",");
                            }
                            query.AppendFormat("{0}Property{1}", propertyType, i + 1);
                        }

                        var response = UnitTestsUtil.GetResponseAsAtom(typeof(BigObjectDataSource), query.ToString(), format);

                        // Verify that the projected properties are included as well
                        for (int i = 0; i < projectedPropertyCount; i++)
                        {
                            UnitTestsUtil.VerifyXPathResultCount(response, 2,
                                string.Format("//atom:entry//ads:{0}Property{1}", propertyType, i + 1));
                        }
                    });
                }
            }

            [TestMethod, Variation]
            public void Projections_Misc()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataServiceType = (Type)values["DataServiceType"];
                        string format = (string)values["Format"];

                        var response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$select=*,Orders&$expand=Orders($select=ID)", format);

                        VerifyETags(response);
                        // Now customer should have Name, ID and GuidValue
                        UnitTestsUtil.VerifyXPathResultCount(response, 3,
                            "/atom:feed/atom:entry/atom:content//ads:Name",
                            "/atom:feed/atom:entry/atom:content//ads:ID");
                        if (dataServiceType != typeof(CustomRowBasedOpenTypesContext))
                        {
                            UnitTestsUtil.VerifyXPathResultCount(response, 3,
                                "/atom:feed/atom:entry/atom:content//ads:GuidValue");
                        }

                        response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$select=BestFriend&$expand=BestFriend($select=ID)" + "", format);

                        VerifyETags(response);
                        // Verify that the first customer has the BestFriend inlined but empty (as it's null)
                        UnitTestsUtil.VerifyXPathExists(response,
                            format == UnitTestsUtil.AtomFormat ?
                                "/atom:feed/atom:entry[contains(atom:id,'Customers(0)')]/atom:link[@title='BestFriend']/adsm:inline[count(*)=0]" :
                                "/atom:feed/atom:entry[contains(atom:id,'Customers(0)')]/atom:content/adsm:properties[ads:BestFriend/@adsm:null='true']");

                        // Verify that the first customer still has the the BestFriend inlines but empty
                        // and that the second customer has the BestFriend filled, but its BestFriend is inlined and empty
                        response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$select=BestFriend&$expand=BestFriend($select=Name;$expand=BestFriend($select=Name))", format);
                        VerifyETags(response); 
                        UnitTestsUtil.VerifyXPathExists(response,
                            format == UnitTestsUtil.AtomFormat ?
                                "/atom:feed/atom:entry[contains(atom:id,'Customers(0)')]/atom:link[@title='BestFriend']/adsm:inline[count(*)=0]" :
                                "/atom:feed/atom:entry[contains(atom:id,'Customers(0)')]/atom:content/adsm:properties[ads:BestFriend/@adsm:null='true']",
                            format == UnitTestsUtil.AtomFormat ?
                                "/atom:feed/atom:entry[contains(atom:id,'Customers(1)')]/atom:link[@title='BestFriend']/adsm:inline//atom:link[@title='BestFriend']/adsm:inline[count(*)=0]" :
                                "/atom:feed/atom:entry[contains(atom:id,'Customers(1)')]/atom:link[@title='BestFriend']/adsm:inline//atom:content/adsm:properties[ads:BestFriend/@adsm:null='true']"
                                );

                        // Verify that navigation over single value nav.property into another one which can have multiple values
                        //   works and that we can project on each level.
                        response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$select=BestFriend&$expand=BestFriend($select=Name;$expand=Orders($select=ID))", format);
                        VerifyETags(response);
                        // Verify that the Customers(0) doesn't have any orders (As its BestFriend = null) and that
                        //   Customers(1) does have some orders.
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed/atom:entry[contains(atom:id, 'Customers(0)') and count(.//atom:entry[contains(atom:id, 'Orders')])=0]",
                            "/atom:feed/atom:entry[contains(atom:id, 'Customers(1)') and count(.//atom:entry[contains(atom:id, 'Orders')])!=0]");

                        // Verify that expansion over the null nav. property works as well (just a sanity check that projections didn't break expand)
                        response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$expand=BestFriend($expand=Orders)", format);
                        VerifyETags(response);
                        // Verify that the Customers(0) doesn't have any orders (As its BestFriend = null) and that
                        //   Customers(1) does have some orders.
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed/atom:entry[contains(atom:id, 'Customers(0)') and count(.//atom:entry[contains(atom:id, 'Orders')])=0]",
                            "/atom:feed/atom:entry[contains(atom:id, 'Customers(1)') and count(.//atom:entry[contains(atom:id, 'Orders')])!=0]");

                        // Verify that expansions are correctly ommited when * is used
                        response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$expand=Orders&$select=*", format);
                        VerifyETags(response);
                        // Verify that Customer does have properties
                        //   but no Orders are inlined
                        UnitTestsUtil.VerifyXPathResultCount(response, 3,
                            "/atom:feed/atom:entry/atom:content/adsm:properties[count(*)>0]",
                            "/atom:feed/atom:entry[count(atom:link[@title='Orders']/adsm:inline)>0]");

                        // Verify that expansions are correctly ommited when * is used on sublevel
                        response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$expand=BestFriend($select=*;$expand=Orders)&$select=BestFriend", format);
                        VerifyETags(response);
                        // Verify that BestFriend does have properties
                        //   but no Orders are inlined
                        UnitTestsUtil.VerifyXPathResultCount(response, 2,
                            "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline/atom:entry/atom:content/adsm:properties[count(*)>0]",
                            "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline/atom:entry[count(atom:link[@title='Orders']/adsm:inline)>0]");

                        response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$select=*,Orders&$expand=Orders($select=ID)", format);
                        VerifyETags(response);
                        // Now customer should have Name, ID and GuidValue
                        UnitTestsUtil.VerifyXPathResultCount(response, 3,
                            "/atom:feed/atom:entry/atom:content//ads:Name",
                            "/atom:feed/atom:entry/atom:content//ads:ID");
                        if (dataServiceType != typeof(CustomRowBasedOpenTypesContext))
                        {
                            UnitTestsUtil.VerifyXPathResultCount(response, 3,
                                "/atom:feed/atom:entry/atom:content//ads:GuidValue");
                        }

                        // Verify that the first customer still has the the BestFriend inlines but empty
                        // and that the second customer has the BestFriend filled, but its BestFriend is inlined and empty
                        response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$select=BestFriend&$expand=BestFriend($select=Name;$expand=BestFriend($select=Name))", format);
                        VerifyETags(response); 
                        UnitTestsUtil.VerifyXPathExists(response,
                            format == UnitTestsUtil.AtomFormat ?
                                "/atom:feed/atom:entry[contains(atom:id,'Customers(0)')]/atom:link[@title='BestFriend']/adsm:inline[count(*)=0]" :
                                "/atom:feed/atom:entry[contains(atom:id,'Customers(0)')]/atom:content/adsm:properties[ads:BestFriend/@adsm:null='true']",
                            format == UnitTestsUtil.AtomFormat ?
                                "/atom:feed/atom:entry[contains(atom:id,'Customers(1)')]/atom:link[@title='BestFriend']/adsm:inline//atom:link[@title='BestFriend']/adsm:inline[count(*)=0]" :
                                "/atom:feed/atom:entry[contains(atom:id,'Customers(1)')]/atom:link[@title='BestFriend']/adsm:inline//atom:content/adsm:properties[ads:BestFriend/@adsm:null='true']"
                                );

                        // Verify that navigation over single value nav.property into another one which can have multiple values
                        //   works and that we can project on each level.
                        response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$select=BestFriend&$expand=BestFriend($select=Name;$expand=Orders($select=ID))", format);
                        VerifyETags(response);
                        // Verify that the Customers(0) doesn't have any orders (As its BestFriend = null) and that
                        //   Customers(1) does have some orders.
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed/atom:entry[contains(atom:id, 'Customers(0)') and count(.//atom:entry[contains(atom:id, 'Orders')])=0]",
                            "/atom:feed/atom:entry[contains(atom:id, 'Customers(1)') and count(.//atom:entry[contains(atom:id, 'Orders')])!=0]");

                        // Verify that expansion over the null nav. property works as well (just a sanity check that projections didn't break expand)
                        response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$expand=BestFriend($expand=Orders)", format);
                        VerifyETags(response);
                        // Verify that the Customers(0) doesn't have any orders (As its BestFriend = null) and that
                        //   Customers(1) does have some orders.
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed/atom:entry[contains(atom:id, 'Customers(0)') and count(.//atom:entry[contains(atom:id, 'Orders')])=0]",
                            "/atom:feed/atom:entry[contains(atom:id, 'Customers(1)') and count(.//atom:entry[contains(atom:id, 'Orders')])!=0]");

                        // Verify that expansions are correctly ommited when * is used
                        response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$expand=Orders&$select=*", format);
                        VerifyETags(response);
                        // Verify that Customer does have properties
                        //   but no Orders are inlined
                        UnitTestsUtil.VerifyXPathResultCount(response, 3,
                            "/atom:feed/atom:entry/atom:content/adsm:properties[count(*)>0]",
                            "/atom:feed/atom:entry[count(atom:link[@title='Orders']/adsm:inline)>0]");

                        // Verify that expansions are correctly ommited when * is used on sublevel
                        response = UnitTestsUtil.GetResponseAsAtom(dataServiceType, "/Customers?$expand=BestFriend($select=*;$expand=Orders)&$select=BestFriend", format);
                        VerifyETags(response);
                        // Verify that BestFriend does have properties
                        //   but no Orders are inlined
                        UnitTestsUtil.VerifyXPathResultCount(response, 2,
                            "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline/atom:entry/atom:content/adsm:properties[count(*)>0]",
                            "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline/atom:entry[count(atom:link[@title='Orders']/adsm:inline)>0]");
                    });
                }
            }

            #region PropertiesOrder
            [TestMethod, Variation]
            public void Projections_DeclaredPropertiesOrder()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext) }));

                using (TestUtil.MetadataCacheCleaner())
                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataServiceType = (Type)values["DataServiceType"];

                        using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                        {
                            request.DataServiceType = dataServiceType;
                            VerifyAtomProjectedPropertiesOrder(request, "/Customers?$select=Name");
                            VerifyAtomProjectedPropertiesOrder(request, "/Customers?$select=Name,ID");
                            VerifyAtomProjectedPropertiesOrder(request, "/Customers?$select=ID,Name");
                            VerifyAtomProjectedPropertiesOrder(request, "/Customers?$select=Name,GuidValue,ID");
                            VerifyAtomProjectedPropertiesOrder(request, "/Customers?$select=GuidValue,ID,Name");
                            VerifyAtomProjectedPropertiesOrder(request, "/Customers?$select=Orders&$expand=Orders($select=ID,DollarAmount)");
                            VerifyAtomProjectedPropertiesOrder(request, "/Customers?$select=Name,ID,Orders&$expand=Orders($select=ID,DollarAmount)");
                        }
                    });
                }
            }

            [TestMethod, Variation]
            public void Projections_OpenPropertiesOrder()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (CustomRowBasedOpenTypesContext.CreateChangeScope())
                using (TestUtil.RestoreStaticValueOnDispose(typeof(CustomRowBasedOpenTypesContext), "CustomizeCustomers"))
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    CustomRowBasedOpenTypesContext.CustomizeCustomers = customers =>
                        {
                            RowComplexType customer = customers.Where(c => (int)c.Properties["ID"] == 0).FirstOrDefault();
                            customer.Properties["OpenProperty0"] = "zero";
                            customer.Properties["OpenProperty1"] = "one";
                            customer.Properties["OpenProperty2"] = "two";
                        };

                    request.DataServiceType = typeof(CustomRowBasedOpenTypesContext);
                    string[] openProperties = new string[] { "Name", "OpenProperty0", "OpenProperty1", "OpenProperty2", "NonExistant" };
                    VerifyAtomProjectedPropertiesOrder(request, "/Customers?$select=Name,ID", openProperties);
                    VerifyAtomProjectedPropertiesOrder(request, "/Customers?$select=ID,Name", openProperties);
                    VerifyAtomProjectedPropertiesOrder(request, "/Customers?$select=OpenProperty0,OpenProperty1,OpenProperty2", openProperties);
                    VerifyAtomProjectedPropertiesOrder(request, "/Customers?$select=OpenProperty2,OpenProperty1,OpenProperty0", openProperties);
                    VerifyAtomProjectedPropertiesOrder(request, "/Customers?$select=OpenProperty2,OpenProperty1,Name", openProperties);
                    VerifyAtomProjectedPropertiesOrder(request, "/Customers?$select=NonExistant,ID,OpenProperty1,Name", openProperties);
                }
            }

            /// <summary>Verifies that the projected properties are listed in the same order as they are in a query without projections.</summary>
            /// <param name="request">The request to use.</param>
            /// <param name="requestUri">Request URI with projections to run (and compare to the same request without projections)</param>
            private void VerifyAtomProjectedPropertiesOrder(TestWebRequest request, string requestUri)
            {
                VerifyAtomProjectedPropertiesOrder(request, requestUri, null);
            }

            /// <summary>Verifies that the projected properties are listed in the same order as they are in a query without projections.
            /// Also verifies that open properties are in alhpabetical order and that they are after all declared properties.</summary>
            /// <param name="request">The request to use.</param>
            /// <param name="requestUri">Request URI with projections to run (and compare to the same request without projections)</param>
            /// <param name="openProperties">List of names of open properties in the projected result. Can be null.</param>
            private void VerifyAtomProjectedPropertiesOrder(TestWebRequest request, string requestUri, IEnumerable<string> openProperties)
            {
                string requestUriNoProjections = RemoveProjectionsFromRequestUri(requestUri);

                XmlDocument projected = UnitTestsUtil.GetResponseAsAtom(request, requestUri, UnitTestsUtil.AtomFormat);
                XmlDocument all = UnitTestsUtil.GetResponseAsAtom(request, requestUriNoProjections, UnitTestsUtil.AtomFormat);
                HashSet<string> openPropertiesSet = new HashSet<string>(openProperties ?? new List<string>());

                XmlNodeList projectedEntries = projected.SelectNodes("//atom:entry", TestUtil.TestNamespaceManager);
                XmlNodeList allEntries = all.SelectNodes("//atom:entry", TestUtil.TestNamespaceManager);
                for (int entryIndex = 0; entryIndex < projectedEntries.Count; entryIndex++)
                {
                    XmlNodeList projectedProperties = projectedEntries[entryIndex].SelectNodes("atom:content/adsm:properties/*", TestUtil.TestNamespaceManager);
                    XmlNodeList allProperties = allEntries[entryIndex].SelectNodes("atom:content/adsm:properties/*", TestUtil.TestNamespaceManager);

                    string description = projectedEntries[entryIndex].SelectNodes("ancestor-or-self::*").ToEnumerable().
                            Select(n => (n.LocalName == "entry") ?
                                ("entry[id='" + ((XmlElement)n).SelectSingleNode("atom:id", TestUtil.TestNamespaceManager).InnerText) + "']" :
                                n.LocalName).
                            Concatenate("/");

                    VerifyProjectedPropertiesOrder(
                        projectedProperties.ToEnumerable<XmlElement>().Select(e => e.LocalName),
                        allProperties.ToEnumerable<XmlElement>().Select(e => e.LocalName),
                        openPropertiesSet,
                        description);
                }
            }

            /// <summary>Verifies that the order of property names in projected is the same as they appear in all.</summary>
            /// <param name="projectedProperties">Names of projected properties in the order they appeared on the wire.</param>
            /// <param name="allProperties">Names of all properties as they appear on the wire.</param>
            /// <param name="openProperties">Set of names of properties which are open.</param>
            /// <param name="description">A description used for logging to identify which object's properties where compared.</param>
            private void VerifyProjectedPropertiesOrder(
                IEnumerable<string> projectedProperties,
                IEnumerable<string> allProperties,
                HashSet<string> openProperties,
                string description)
            {
                IEnumerator<string> projectedEnumerator = projectedProperties.Where(name => !openProperties.Contains(name)).GetEnumerator();
                IEnumerator<string> allEnumerator = allProperties.GetEnumerator();
                string failureString = null;
                string declaredPropertyFailure = "The order of declared projected properties doesn't match the order of all properties.";

                // Verify order of declared properties
                bool projectedAvailable = projectedEnumerator.MoveNext();
                bool allAvailable = allEnumerator.MoveNext();
                while (projectedAvailable)
                {
                    if (!allAvailable)
                    {
                        failureString = declaredPropertyFailure;
                        goto Failure;
                    }
                    while (!String.Equals(projectedEnumerator.Current, allEnumerator.Current, StringComparison.Ordinal))
                    {
                        allAvailable = allEnumerator.MoveNext();
                        if (!allAvailable)
                        {
                            failureString = declaredPropertyFailure;
                            goto Failure;
                        }
                    }
                    projectedAvailable = projectedEnumerator.MoveNext();
                }

                // Verify order of open properties
                string previousOpenPropertyName = null;
                foreach (string openPropertyName in projectedProperties.Where(name => openProperties.Contains(name)))
                {
                    if (previousOpenPropertyName != null &&
                        string.Compare(previousOpenPropertyName, openPropertyName, StringComparison.Ordinal) > 0)
                    {
                        failureString = "The order of projected open properties is not alphabetical.";
                        goto Failure;
                    }
                }

                // Verify that open properties are after all declared properties
                bool foundOpenProperty = false;
                foreach (string propertyName in projectedProperties)
                {
                    if (openProperties.Contains(propertyName))
                    {
                        foundOpenProperty = true;
                    }
                    else if (foundOpenProperty)
                    {
                        failureString = "Found declared property after an open property.";
                        goto Failure;
                    }
                }

                return;

            Failure:
                Trace.WriteLine("Comparing properties in: " + description);
                Trace.WriteLine("Projected properties: " + projectedProperties.Concatenate(", "));
                Trace.WriteLine("All properties: " + allProperties.Concatenate(", "));
                Trace.WriteLine("Open properties: " + openProperties.Concatenate(", "));
                Assert.Fail(failureString);
            }

            /// <summary>Removes the $select option from the specified query.</summary>
            /// <param name="requestUri">The query to process.</param>
            /// <returns>The same query but with the $select removed (or untouched if no $select was present).</returns>
            private static string RemoveProjectionsFromRequestUri(string requestUri)
            {
                int start = requestUri.IndexOf("$select");
                if (start == -1) return requestUri;
                int end = requestUri.IndexOf('&', start);
                string allRequest = requestUri.Substring(0, start) +
                    ((end == -1) ? "" : requestUri.Substring(end + 1));
                allRequest = allRequest.TrimEnd('&', '?');
                return allRequest;
            }
            #endregion

            [TestMethod, Variation]
            public void Projections_ProjectNullValues()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                TestUtil.RunCombinatorialEngineFail(engine, values =>
                {
                    string format = (string)values["Format"];

                    // Try it with Linq to Objects and a null string
                    using (TestUtil.RestoreStaticValueOnDispose(typeof(CustomDataContext), "ModifyDefaultData"))
                    {
                        CustomDataContext.ModifyDefaultData = ctx =>
                            {
                                ctx.InternalCustomersList.Add(new Customer()
                                {
                                    ID = 42,
                                    Name = null,
                                    Address = null
                                });
                            };
                        var response = UnitTestsUtil.GetResponseAsAtom(typeof(CustomDataContext), "/Customers(42)?select=Name,Address", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:entry/atom:content/adsm:properties/ads:Name[@adsm:null='true']",
                            "/atom:entry/atom:content/adsm:properties/ads:Address[@adsm:null='true']");
                    }

                    // And now try it with open types
                    using (TestUtil.RestoreStaticValueOnDispose(typeof(CustomRowBasedOpenTypesContext), "CustomizeCustomers"))
                    using (TestUtil.MetadataCacheCleaner())
                    {
                        CustomRowBasedOpenTypesContext.CustomizeCustomers = customers =>
                        {
                            RowComplexType customer = customers.Where(c => (int)c.Properties["ID"] == 0).FirstOrDefault();
                            customer.Properties["NullProperty"] = null;
                        };

                        var response = UnitTestsUtil.GetResponseAsAtom(typeof(CustomRowBasedOpenTypesContext), "/Customers(0)?$select=NullProperty", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:entry/atom:content/adsm:properties/ads:NullProperty[@adsm:null='true']");
                    }
                });
            }

            [TestMethod, Variation]
            public void Projections_ProjectionsOverPropertyNavigation()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                using (TestUtil.MetadataCacheCleaner())
                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type dataServiceType = (Type)values["DataServiceType"];
                        string format = (string)values["Format"];
                        string typePrefix = dataServiceType == typeof(ocs.CustomObjectContext) ?
                            "AstoriaUnitTests.ObjectContextStubs.Types" : "AstoriaUnitTests.Stubs";
                        using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                        {
                            request.ForceVerboseErrors = true;
                            request.DataServiceType = dataServiceType;

                            // Try on the nav.property which has the same name as the resource set it points to
                            var response = UnitTestsUtil.GetResponseAsAtom(request, "/Customers(0)/Orders?$select=ID", format);
                            // Verify that we recieve 2 results
                            UnitTestsUtil.VerifyXPathResultCount(response, 2, "/atom:feed/atom:entry");
                            // Verify that they are indeed Orders
                            UnitTestsUtil.VerifyXPathResultCount(response, 2,
                                string.Format("/atom:feed/atom:entry/atom:category[@term='#{0}.Order']", typePrefix));
                            // Verify that ID is projected
                            UnitTestsUtil.VerifyXPathResultCount(response, 2,
                                "/atom:feed/atom:entry/atom:content/adsm:properties/ads:ID");
                            // Verify that nothing else is projected
                            UnitTestsUtil.VerifyXPathDoesntExist(response,
                                "/atom:feed/atom:entry/atom:content/adsm:properties/*[local-name()!='ID']",
                                "/atom:feed/atom:entry/atom:link[@rel!='edit']");

                            // Try on a nav.property with a different name than it points to
                            response = UnitTestsUtil.GetResponseAsAtom(request, "/Customers(1)/BestFriend?$select=Name", format);
                            // Verify that we get the right one
                            UnitTestsUtil.VerifyXPathExists(response,
                                "/atom:entry/atom:content/adsm:properties[ads:Name='Customer 0']");

                            // Try projections over nav. property with single null value
                            UnitTestsUtil.VerifyNoContentResponse(request, "/Customers(0)/BestFriend?$select=Name", format);
                        }
                    });
                }
            }

            [TestMethod, Variation]
            public void Projections_OpenTypeWithPropertyDeclaredOnDerivedType()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                using (TestUtil.MetadataCacheCleaner())
                using (CustomRowBasedOpenTypesContext.CreateChangeScope())
                using (TestUtil.RestoreStaticValueOnDispose(typeof(CustomRowBasedOpenTypesContext), "CustomizeMetadata"))
                using (TestUtil.RestoreStaticValueOnDispose(typeof(CustomRowBasedOpenTypesContext), "CustomizeCustomers"))
                {
                    CustomRowBasedOpenTypesContext.CustomizeMetadata = (resourceSets, types, operations, associationSets) =>
                    {
                        providers.ResourceType customer = types.Where(t => t.Name == "Customer").First();
                        providers.ResourceType customerWithPhone = new providers.ResourceType(
                            customer.InstanceType,
                            providers.ResourceTypeKind.EntityType,
                            customer,
                            "AstoriaUnitTests.Stubs",
                            "CustomerWithPhone",
                            false);
                        customerWithPhone.IsOpenType = true;
                        customerWithPhone.CanReflectOnInstanceType = false;

                        providers.ResourceProperty phoneProperty = new providers.ResourceProperty(
                            "Phone", providers.ResourcePropertyKind.Primitive,
                            providers.ResourceType.GetPrimitiveResourceType(typeof(string)));
                        phoneProperty.CanReflectOnInstanceTypeProperty = false;
                        customerWithPhone.AddProperty(phoneProperty);

                        types.Add(customerWithPhone);

                        providers.ResourceType customerWithBirthday = types.Where(t => t.Name == "CustomerWithBirthday").First();
                        providers.ResourceType customerWithRealBirthday = new providers.ResourceType(
                            customer.InstanceType,
                            providers.ResourceTypeKind.EntityType,
                            customerWithBirthday,
                            "AstoriaUnitTests.Stubs",
                            "CustomerWithRealBirthday",
                            false);
                        customerWithRealBirthday.IsOpenType = true;
                        customerWithRealBirthday.CanReflectOnInstanceType = false;

                        providers.ResourceProperty birthdayProperty = new providers.ResourceProperty(
                            "Birthday", providers.ResourcePropertyKind.Primitive,
                            providers.ResourceType.GetPrimitiveResourceType(typeof(DateTime)));
                        birthdayProperty.CanReflectOnInstanceTypeProperty = false;
                        customerWithRealBirthday.AddProperty(birthdayProperty);

                        types.Add(customerWithRealBirthday);
                    };

                    CustomRowBasedOpenTypesContext.CustomizeCustomers = (customers) =>
                    {
                        RowComplexType customerWithPhone = new RowComplexType("AstoriaUnitTests.Stubs.CustomerWithPhone");
                        customerWithPhone.Properties["ID"] = 100;
                        customerWithPhone.Properties["GuidValue"] = System.Guid.NewGuid();
                        customerWithPhone.Properties["Name"] = "The one with phone.";
                        customerWithPhone.Properties["Phone"] = "1234567890";
                        customers.Add(customerWithPhone);

                        RowComplexType customerWithRealBirthday = new RowComplexType("AstoriaUnitTests.Stubs.CustomerWithRealBirthday");
                        customerWithRealBirthday.Properties["ID"] = 101;
                        customerWithRealBirthday.Properties["GuidValue"] = System.Guid.NewGuid();
                        customerWithRealBirthday.Properties["Name"] = "The one with real birthday.";
                        customerWithRealBirthday.Properties["Birthday"] = DateTime.Today.AddYears(-30);
                        customers.Add(customerWithRealBirthday);

                        customers.Where(c => (int)c.Properties["ID"] == 0).First().Properties["Phone"] = "0987654321";
                    };

                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        string format = (string)values["Format"];

                        XmlDocument response;
                        using (TestUtil.MetadataCacheCleaner())
                        using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                        {
                            request.DataServiceType = typeof(CustomRowBasedOpenTypesContext);
                            request.RequestMaxVersion = "4.0;";
                            response = UnitTestsUtil.GetResponseAsAtom(request, "/Customers?$select=Phone,ID", format);
                        }

                        // Verify the property gets correctly projected when it was declared on the instance
                        UnitTestsUtil.VerifyXPathExists(response, "//atom:entry[.//ads:ID='100' and .//ads:Phone='1234567890']");
                        // Verify the property gets correctly projected when it is open on the instance
                        UnitTestsUtil.VerifyXPathExists(response, "//atom:entry[.//ads:ID='0' and .//ads:Phone='0987654321']");
                        // Verify the property gets projected as null when it is not present on the instance
                        UnitTestsUtil.VerifyXPathExists(response, "//atom:entry[.//ads:ID='1' and .//ads:Phone/@adsm:null='true']");
                    });
                }
            }

            #region Inheritance
            public class InheritanceContext
            {
                private static List<BaseEntity> entities;

                static InheritanceContext()
                {
                    entities = new List<BaseEntity>();

                    BaseEntity e0 = new BaseEntity();
                    e0.ID = 0;
                    e0.Name = "Base entity 0";
                    entities.Add(e0);

                    DerivedEntity e100 = new DerivedEntity();
                    e100.ID = 100;
                    e100.Name = "Derived entity 0";
                    e100.DerivedName = "Derived name 0";
                    e100.ComplexProperty = null;
                    entities.Add(e100);

                    BaseEntity e1 = new BaseEntity();
                    e1.ID = 1;
                    e1.Name = "Base entity 1";
                    entities.Add(e1);

                    DerivedEntity e101 = new DerivedEntity();
                    e101.ID = 101;
                    e101.Name = "Derived entity 1";
                    e101.DerivedName = "Derived name 1";
                    e101.ComplexProperty = new ComplexType() { Comment = "Complex type comment" };
                    entities.Add(e101);

                    // Leave everything empty on e0
                    // Leave everything empty on e100

                    e1.SingleBaseEntity = e0;
                    e1.BaseEntities.Add(e0);
                    e1.SingleDerivedEntity = e101;
                    e1.DerivedEntities.Add(e100);

                    e101.SingleBaseEntity = e0;
                    e101.BaseEntities.Add(e0);
                    e101.BaseEntities.Add(e100);
                    e101.SingleDerivedEntity = e100;
                    e101.DerivedEntities.Add(e100);
                    e101.DerivedEntities.Add(e101);
                }

                public class BaseEntity
                {
                    public int ID { get; set; }
                    public string Name { get; set; }

                    public List<BaseEntity> BaseEntities { get; private set;  }
                    public BaseEntity SingleBaseEntity { get; set; }
                    public List<DerivedEntity> DerivedEntities { get; private set; }
                    public DerivedEntity SingleDerivedEntity { get; set; }

                    public BaseEntity()
                    {
                        this.BaseEntities = new List<BaseEntity>();
                        this.DerivedEntities = new List<DerivedEntity>();
                    }
                }

                public class DerivedEntity : BaseEntity
                {
                    public string DerivedName { get; set; }
                    public ComplexType ComplexProperty { get; set; }
                }

                public class ComplexType
                {
                    public string Comment { get; set; }
                }

                public IQueryable<BaseEntity> Entities
                {
                    get
                    {
                        return entities.AsQueryable();
                    }
                }
            }

            public class InheritanceService : OpenWebDataService<InheritanceContext>
            {
                [WebGet]
                public IQueryable<InheritanceContext.DerivedEntity> GetDerivedEntities()
                {
                    return this.CurrentDataSource.Entities.Where(e => e is InheritanceContext.DerivedEntity).Cast<InheritanceContext.DerivedEntity>().AsQueryable();
                }
            }

            [TestMethod()]
            public void Projections_DerivedProperties()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                using (TestUtil.MetadataCacheCleaner())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(InheritanceService);

                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        request.Accept = (string)values["Format"];

                        // Try accessing derived property over a navigation - collection
                        var response = UnitTestsUtil.GetResponseAsAtomXLinq(request, "/Entities?$select=DerivedEntities&$expand=DerivedEntities($select=DerivedName)");
                        // There should be 3 DerivedName properties in the response
                        // 1 with expanded 100 has one, 101 with expanded 100 has second and 101 with expanded 101 has third
                        UnitTestsUtil.VerifyXPathResultCount(response, 3, "//ads:DerivedName");
                        // Single entity with expansion
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request, "/Entities?$select=SingleDerivedEntity&$expand=SingleDerivedEntity($select=DerivedName)");
                        // There should be 2 DerivedName properties in the response
                        // 1 with expanded 101 has one, 101 with expanded 100 has second.
                        UnitTestsUtil.VerifyXPathResultCount(response, 2, "//ads:DerivedName");

                        // Single entity with property navigation
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request, "/Entities(1)/SingleDerivedEntity?$select=DerivedName");
                        UnitTestsUtil.VerifyXPathResultCount(response, 1, "//ads:DerivedName");

                        // Try accessing derived property over a service operation - collection
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request, "/GetDerivedEntities?$select=DerivedName");
                        // There should be 2 DerivedName properties in the response
                        // 100 has one and 101 has the second
                        UnitTestsUtil.VerifyXPathResultCount(response, 2, "//ads:DerivedName");
                        // Single entity - this fails as we don't allow $select on SO returning single entity - no reason to test here

                        // Try accessing derived complex property over a navigation - collection
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request, "/Entities?$select=DerivedEntities&$expand=DerivedEntities($select=ComplexProperty)");
                        // There should be 3 ComplexProperty properties and one ComplexProperty/Comment (the others are null)
                        UnitTestsUtil.VerifyXPathResultCount(response, 3, "//ads:ComplexProperty");
                        UnitTestsUtil.VerifyXPathResultCount(response, 1, "//ads:ComplexProperty/ads:Comment");

                        // Try accessing derived complex property over a navigation - sinle
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request, "/Entities(1)/SingleDerivedEntity?$select=ComplexProperty");
                        // There should be one ComplexProperty/Comment
                        UnitTestsUtil.VerifyXPathResultCount(response, 1, "//ads:ComplexProperty/ads:Comment");

                        // Try accessing derived complex property over a service operation - collection
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request, "/GetDerivedEntities?$select=ComplexProperty");
                        // There should be 2 ComplexProperty properties and one ComplexProperty/Comment (the other is null)
                        UnitTestsUtil.VerifyXPathResultCount(response, 2, "//ads:ComplexProperty");
                        UnitTestsUtil.VerifyXPathResultCount(response, 1, "//ads:ComplexProperty/ads:Comment");
                    });

                    // Verify that asking for the derived property on the entity set itself fails
                    VerifyQueryInvalid(request, "/Entities?$select=DerivedName");
                    // It should also fail asking for it on navigation property of the base type
                    VerifyQueryInvalid(request, "/Entities?$select=SingleBaseEntity?$expand=SingleBaseEntity($select=DerivedName)");
                    // And on a nav collection property of the base type
                    VerifyQueryInvalid(request, "/Entities?$select=BaseEntities?$expand=BaseEntities($select=DerivedName)");
                }
            }
            #endregion

            [TestMethod, Variation]
            public void Projections_SingleResult()
            {
                // Note - Linq To Objects will not work in these cases
                // V1 had this bug that we don't check for nulls on the root query, we can't fix it in V2
                //   without breaking Linq to SQL. So there's no point testing that it will fail.
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                using (TestUtil.MetadataCacheCleaner())
                using (ocs.PopulateData.CreateTableAndPopulateData())
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        request.DataServiceType = (Type)values["DataServiceType"];
                        string format = (string)values["Format"];

                        UnitTestsUtil.VerifyNoContentResponse(request,
                            "/Customers(0)/BestFriend?$expand=BestFriend($select=Name)&$select=Name,BestFriend", format);

                        // Verify that non-null nav. property works (Customer 2 has a BestFriend)
                        var response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Customers(2)/BestFriend?$select=Name", format);
                        UnitTestsUtil.VerifyXPathResultCount(response, 1,
                            "/atom:entry//ads:Name");

                        // Verify that null nav. property returns empty response (Customer 0 doesn't have a BestFriend)
                        UnitTestsUtil.VerifyNoContentResponse(request,
                            "/Customers(0)/BestFriend?$select=Name", format);

                        // Verify that non-null nav. property and expand works
                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Customers(2)/BestFriend?$expand=Orders", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:entry/atom:link[@title='Orders']//atom:feed");

                        // And the same over null is empty
                        UnitTestsUtil.VerifyNoContentResponse(request,
                            "/Customers(0)/BestFriend?$expand=Orders", format);

                        // Verify that non-null nav. property works with both expand and select
                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Customers(2)/BestFriend?$expand=BestFriend($select=Name)&$select=Name,BestFriend", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:entry//ads:Name",
                            "/atom:entry/atom:link[@title='BestFriend']//ads:Name");

                        // And the same over null is empty
                        UnitTestsUtil.VerifyNoContentResponse(request,
                            "/Customers(0)/BestFriend?$expand=BestFriend($select=Name)&$select=Name,BestFriend", format);
                    });
                }
            }

            [TestMethod, Variation]
            public void Projections_NorthwindEdm()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                TestUtil.RunCombinatorialEngineFail(engine, values =>
                {
                    string format = (string)values["Format"];
                    using (TestUtil.MetadataCacheCleaner())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                    {
                        request.DataServiceType = ServiceModelData.Northwind.ServiceModelType;
                        XmlDocument response;

                        response = UnitTestsUtil.GetResponseAsAtom(request, "/Customers('ALFKI')?$select=Region", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:entry/atom:content/adsm:properties/ads:Region[@adsm:null='true']");

                        response = UnitTestsUtil.GetResponseAsAtom(request, "/Employees(2)/Employees1?$select=LastName", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:LastName");

                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Customers?$select=CustomerID,CompanyName,ContactName,ContactTitle,Address,City," +
                            "Region,PostalCode,Phone,Fax",
                            format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:CustomerID",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:CompanyName",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:ContactName",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:ContactTitle",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:Address",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:City",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:Region",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:PostalCode",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:Phone",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:Fax");

                        // This verifies that we build acceptable query when navigationg over a nav. property to a single resource
                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Orders?$select=ShipName,Customers&$expand=Customers($select=CustomerID,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Phone,Fax;)",
                            format);

                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Invoices?$select=ShipName",
                            format);

                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Categories?$orderby=CategoryID&$select=CategoryID,CategoryName,Description,Picture",
                            format);

                        // This verifies that we access the correct projected property from OrderBy
                        //   when projected multiple levels of ProjectedWrapperMany. If not, it fails
                        //   to convert values from one type to another (as each orderby clause has a differnt CLR type).
                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Invoices?$orderby=CustomerName,Salesperson,OrderID,ShipperName,ProductID,ProductName," +
                            "UnitPrice,Quantity,Discount&$select=ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode," +
                            "CustomerID,CustomerName,Address,City,Region,PostalCode,Salesperson," +
                            "OrderID,OrderDate,RequiredDate,ShippedDate,ShipperName,ProductID,ProductName,UnitPrice," +
                            "Quantity,Discount,ExtendedPrice,Freight",
                            format);

                        // This verifies that we can sort using value type property in some non-trivial expression
                        //   with projections on. The expression has to project the proeprty as nullable but then the
                        //   add operator needs true int, so we need to ask for its value
                        response = UnitTestsUtil.GetResponseAsAtom(request, "/Orders?$orderby=OrderID add 3&$select=ShipCity", format);
                        // And then try the same without the complex expression - just in case
                        response = UnitTestsUtil.GetResponseAsAtom(request, "/Orders?$orderby=OrderID&$select=ShipCity", format);
                        // And now try the same with a nav. property traversal
                        response = UnitTestsUtil.GetResponseAsAtom(request, "/Orders?$orderby=Shippers/ShipperID&$select=ShipCity", format);
                        response = UnitTestsUtil.GetResponseAsAtom(request, "/Orders?$orderby=Shippers/ShipperID add 3&$select=ShipCity", format);
                    }

                    using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                    using (TestUtil.MetadataCacheCleaner())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                    {
                        request.DataServiceType = ServiceModelData.Northwind.ServiceModelType;
                        OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) =>
                        {
                            config.SetEntitySetPageSize("Customers", 3);
                            config.SetEntitySetPageSize("Orders", 3);
                        };

                        var response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Customers?$top=3&$expand=Orders($select=OrderID,OrderDate)&$select=CustomerID,ContactName,Orders",
                            format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed/atom:entry/atom:link[@title='Orders']//atom:link[@rel='next']");
                    }
                });
            }

            public class NorthwindSqlWithInterceptorsService<T> : OpenWebDataService<T>
            {
                [QueryInterceptor("Orders")]
                public Expression<Func<AstoriaUnitTests.Stubs.Sql.Orders, bool>> QueryInterceptor_Orders()
                {
                    return (o) => o.OrderID >= 10625;
                }
            }

            [TestMethod, Variation]
            public void Projections_NorthwindSql()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                TestUtil.RunCombinatorialEngineFail(engine, values =>
                {
                    string format = (string)values["Format"];

                    using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                    {
                        request.DataServiceType = ServiceModelData.SqlNorthwindData.ServiceModelType;
                        XmlDocument response;

                        response = UnitTestsUtil.GetResponseAsAtom(request, "/Customers('ALFKI')?$select=Region", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:entry/atom:content/adsm:properties/ads:Region[@adsm:null='true']");

                        response = UnitTestsUtil.GetResponseAsAtom(request, "/Employees(2)/Employee?$select=LastName", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:LastName");

                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Customers?$select=CustomerID,CompanyName,ContactName,ContactTitle,Address,City," +
                            "Region,PostalCode,Phone,Fax",
                            format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:CustomerID",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:CompanyName",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:ContactName",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:ContactTitle",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:Address",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:City",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:Region",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:PostalCode",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:Phone",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:Fax");

                        // This verifies that we build acceptable query when navigationg over a nav. property to a single resource
                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Orders?$select=ShipName,Customers&$expand=Customers($select=CustomerID,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Phone,Fax)",
                            format);

                        // Verify that expand without projections over null nav property works
                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/CustomerCustomerDemo?$expand=Customers($expand=CustomerCustomerDemo)",
                            format);

                        // Verify that projections work even when URI path property traversal is used
                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Customers('ALFKI')/Orders(10643)/OrderDetails(OrderID=10643,ProductID=28)?" +
                                "$expand=Products($select=ProductID;$expand=Categories($select=CategoryID),Suppliers($select=CompanyName))&" +
                                "$select=Products," +
                                    "Quantity,UnitPrice",
                            UnitTestsUtil.AtomFormat);

                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Categories?$orderby=CategoryID&$select=CategoryID,CategoryName,Description,Picture",
                            format);

                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Orders?$orderby=OrderID add 3&$select=ShipCity",
                            format);

                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Orders?$orderby=OrderID&$select=ShipCity",
                            format);

                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Orders?$orderby=Shippers/ShipperID&$select=ShipCity",
                            format);

                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Orders?$orderby=Shippers/ShipperID add 3&$select=ShipCity",
                            format);

                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Customers?$top=3&$expand=Orders($select=OrderID,OrderDate)&$select=CustomerID,Orders,ContactName", 
                            format);
                    }

                    using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                    {
                        request.DataServiceType = ServiceModelData.SqlNorthwindData.ServiceModelType;
                        OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) =>
                            {
                                config.SetEntitySetPageSize("Customers", 3);
                                config.SetEntitySetPageSize("Orders", 3);
                                config.SetEntitySetPageSize("CustomerCustomerDemo", 3);
                            };
                        XmlDocument response;

                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Customers?$top=3&$expand=Orders($select=OrderID,OrderDate)&$select=CustomerID,Orders,ContactName",
                            format);

                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/CustomerCustomerDemo?$expand=Customers($expand=CustomerCustomerDemo)", format);
                        UnitTestsUtil.VerifyXPathExists(response, "/atom:feed[count(atom:entry)=0]");

                        response = UnitTestsUtil.GetResponseAsAtom(request,
                            "/Customers?$expand=Orders", format);

                        /*
                        DEVNOTE: This query had to be disabled since it's causing an assert in Linq to SQL that will not be fixed (#804789). 
                        I am leaving the query commented so that we know that it was intentionally disabled rather than missed. 
                         
                         Hits the known problem with $expand and $select in Linq to SQL where we try to project constant null
                           and the query can't be translated because of it.
                         VerifyQueryInvalid(request,
                             "/CustomerCustomerDemo?$expand=Customers/CustomerCustomerDemo&" +
                                 "$select=CustomerTypeID,Customers/CustomerID,Customers/CustomerCustomerDemo/CustomerTypeID",
                             format, 500);
                         */
                    }

                    using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                    {
                        request.DataServiceType = typeof(NorthwindSqlWithInterceptorsService<>).MakeGenericType(ServiceModelData.SqlNorthwindData.ServiceModelType);
                        XmlDocument response;
                        
                        // Filtered expand should work (V1 scenario which worked)
                        response = UnitTestsUtil.GetResponseAsAtom(request, "/Shippers?$expand=Orders", format);
                        UnitTestsUtil.VerifyXPathExists(response, "/atom:feed//atom:link[@title='Orders']//atom:feed");
                        // Try it with projections as well
                        response = UnitTestsUtil.GetResponseAsAtom(request, "/Shippers?$expand=Orders($select=OrderID)&$select=ShipperID,Orders", format);
                        response = UnitTestsUtil.GetResponseAsAtom(request, "/Shippers?$expand=Orders($select=OrderID)&$select=*,Orders", format);
                        response = UnitTestsUtil.GetResponseAsAtom(request, "/Shippers?$expand=Orders&$select=ShipperID,Orders", format);

                        // We don't use null check here (V1 bug) which actually makes Linq to SQL work (even though it breaks Linq to Objects)
                        response = UnitTestsUtil.GetResponseAsAtom(request, "/Orders(10625)/Shippers?$expand=Orders", format);
                        UnitTestsUtil.VerifyXPathExists(response, "/atom:entry/atom:link[@title='Orders']//atom:feed");
                        // same with projections
                        response = UnitTestsUtil.GetResponseAsAtom(request, "/Orders(10625)/Shippers?$expand=Orders($select=OrderID)&$select=ShipperID,Orders", format);
                        UnitTestsUtil.VerifyXPathExists(response, "/atom:entry/atom:link[@title='Orders']//atom:feed");
                    }
                });
            }

            [TestMethod, Variation]
            public void Paging_RootLevelWithProjections()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Format", UnitTestsUtil.ResponseFormats),
                    new Dimension("ServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("OrderBy", new string[] { "", "&$orderby=ID", "&$orderby=Name", "&$orderby=Name desc", "&$orderby=ID sub length(Name)" }),
                    new Dimension("Expand", new bool[] { true, false }));

                using (TestUtil.MetadataCacheCleaner())
                using (ocs.PopulateData.CreateTableAndPopulateData())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        string format = (string)values["Format"];
                        string orderby = (string)values["OrderBy"];
                        bool expand = (bool)values["Expand"];
                        request.DataServiceType = (Type)values["ServiceType"];
                        OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) =>
                            {
                                config.SetEntitySetPageSize("Customers", 2);
                            };

                        // Simple top level and complex top level projections + navigation collection/all
                        string query = (expand ? "/Customers?$select=ID,Address,Orders&$expand=Orders" : "/Customers?$select=ID,Address") + orderby;
                        var response = UnitTestsUtil.GetResponseAsAtomXLinq(request, query, format);
                        // Verify only 2 customers were returned
                        UnitTestsUtil.VerifyXPathResultCount(response, 2, "/atom:feed/atom:entry");
                        // Get the next link and a response to it
                        string nextLink = UnitTestsUtil.GetXPathValue(response, "/atom:feed/atom:link[@rel='next']/@href");
                        Assert.IsNotNull(nextLink, "There should be a next link.");
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request, nextLink, format);
                        // Verify only 1 customer was returned
                        UnitTestsUtil.VerifyXPathResultCount(response, 1, "/atom:feed/atom:entry");
                        // Verify the ID is present, no other properties are and there are no links
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:ID",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:Address",
                            "/atom:feed/atom:entry/atom:content/adsm:properties[count(./*[local-name()!='ID' and local-name()!='Address'])=0]",
                            expand ?
                                "/atom:feed/atom:entry[count(./atom:link[@rel!='edit' and @title!='Orders'])=0]" :
                                "/atom:feed/atom:entry[count(./atom:link[@rel!='edit'])=0]");

                        // * on top level projection + navigation reference/all
                        query = (expand ? "/Customers?$select=*,BestFriend&$expand=BestFriend" : "/Customers?$select=*") + orderby;
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request, query, format);
                        // Verify only 2 customers were returned
                        UnitTestsUtil.VerifyXPathResultCount(response, 2, "/atom:feed/atom:entry");
                        // Get the next link and a response to it
                        nextLink = UnitTestsUtil.GetXPathValue(response, "/atom:feed/atom:link[@rel='next']/@href");
                        Assert.IsNotNull(nextLink, "There should be a next link.");
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request, nextLink, format);
                        // Verify only 1 customer was returned
                        UnitTestsUtil.VerifyXPathResultCount(response, 1, "/atom:feed/atom:entry");
                        // Verify the ID and Name are present (all properties should be in fact)
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:ID",
                            "/atom:feed/atom:entry/atom:content/adsm:properties/ads:Name",
                            expand ? "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline | /atom:feed/atom:entry/atom:content//ads:BestFriend[@adsm:null]" : "/");
                    });
                }
            }

            [TestMethod, Variation]
            public void Paging_ExpansionsAndProjections()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Format", UnitTestsUtil.ResponseFormats),
                    new Dimension("ServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }));

                using (TestUtil.MetadataCacheCleaner())
                using (ocs.PopulateData.CreateTableAndPopulateData())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        string format = (string)values["Format"];
                        request.DataServiceType = (Type)values["ServiceType"];
                        OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) =>
                        {
                            config.SetEntitySetPageSize("Customers", 1);
                            config.SetEntitySetPageSize("Orders", 1);
                        };

                        // Simple collection navigation
                        var response = UnitTestsUtil.GetResponseAsAtomXLinq(request, "/Customers?$select=ID&$expand=Orders($select=ID)", format);
                        // Verify that only 1 Order is reported back (and 1 Customer)
                        UnitTestsUtil.VerifyXPathResultCount(response, 1, "/atom:feed/atom:entry//atom:entry");

                        // Get the next link for Orders and a response to it
                        string nextLink = UnitTestsUtil.GetXPathValue(response, "/atom:feed/atom:entry//atom:feed/atom:link[@rel='next']/@href");
                        Assert.IsNotNull(nextLink, "There should be a next link.");
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request, nextLink, format);
                        // Verify only 1 order was returned
                        UnitTestsUtil.VerifyXPathResultCount(response, 1, "/atom:feed/atom:entry");
                        // Verify the ID is present, no other properties are and there are no links
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed/atom:entry//ads:ID",
                            "/atom:feed/atom:entry//adsm:properties[count(./*[local-name()!='ID'])=0]",
                            "/atom:feed/atom:entry[count(./atom:link[@rel!='edit'])=0]");

                        // Verify that we can correctly expand single value nav. properties with null values
                        //   and then apply OrderBy on its nav. property.
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request, "/Customers?$expand=BestFriend($expand=Orders)", format);
                        // Verify that first customer has no orders (as BestFriend is null)
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry[contains(atom:id, 'Orders')]");
                        nextLink = UnitTestsUtil.GetXPathValue(response, "/atom:feed/atom:link[@rel='next']/@href");
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request, nextLink, format);
                        // Verify that second customer does have some orders
                        UnitTestsUtil.VerifyXPathExists(response, "//atom:entry[contains(atom:id, 'Orders')]");

                        // Verify that we can correctly expand single value nav. properties with null values using projections
                        //   and then apply OrderBy on its nav. property with projections as well
                        // Simple reference navigation + deep navigation
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request,
                            "/Customers?$expand=BestFriend($select=Name;$expand=Orders($select=ID))",
                            format);
                        // Verify that first customer has no orders (as BestFriend is null)
                        UnitTestsUtil.VerifyXPathDoesntExist(response, "//atom:entry[contains(atom:id, 'Orders')]");
                        nextLink = UnitTestsUtil.GetXPathValue(response, "/atom:feed/atom:link[@rel='next']/@href");
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request, nextLink, format);
                        // Verify that second customer does have some orders
                        UnitTestsUtil.VerifyXPathExists(response, "//atom:entry[contains(atom:id, 'Orders')]");

                        // * projection on collection navigation
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request,
                            "/Customers?$expand=Orders($select=*)",
                            format);
                        nextLink = UnitTestsUtil.GetXPathValue(response, "/atom:feed/atom:link[@rel='next']/@href");
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request, nextLink, format);
                        // Verify that second customer does have some orders and some properties on them
                        UnitTestsUtil.VerifyXPathExists(response, 
                            "//atom:entry[contains(atom:id, 'Orders')]/atom:content/adsm:properties/ads:ID");

                        // projection all on collection navigation
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request,
                            "/Customers?$expand=Orders&$select=Orders",
                            format);
                        nextLink = UnitTestsUtil.GetXPathValue(response, "/atom:feed/atom:link[@rel='next']/@href");
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request, nextLink, format);
                        // Verify that second customer does have some orders and some properties on them
                        UnitTestsUtil.VerifyXPathExists(response,
                            "//atom:entry[contains(atom:id, 'Orders')]/atom:content/adsm:properties/ads:ID");

                        // * projection on reference navigation
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request,
                            "/Customers?$expand=BestFriend($select=*;$expand=Orders($select=ID))",
                            format);
                        nextLink = UnitTestsUtil.GetXPathValue(response, "/atom:feed/atom:link[@rel='next']/@href");
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request, nextLink, format);
                        // Verify that second customer does have BestFriend and some properties on it
                        UnitTestsUtil.VerifyXPathExists(response,
                            "//atom:entry//atom:entry[contains(atom:id, 'Customer')]/atom:content/adsm:properties[count(./*)>1]");

                        // projection all on reference navigation
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request,
                            "/Customers?$expand=BestFriend&$select=BestFriend",
                            format);
                        nextLink = UnitTestsUtil.GetXPathValue(response, "/atom:feed/atom:link[@rel='next']/@href");
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request, nextLink, format);
                        // Verify that second customer does have BestFriend and some properties on it
                        UnitTestsUtil.VerifyXPathExists(response,
                            "//atom:entry//atom:entry[contains(atom:id, 'Customer')]/atom:content/adsm:properties[count(./*)>1]");

                        // complex projection on reference navigation
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request,
                            "/Customers?$expand=BestFriend($select=Address)",
                            format);
                        nextLink = UnitTestsUtil.GetXPathValue(response, "/atom:feed/atom:link[@rel='next']/@href");
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request, nextLink, format);
                        // Verify that second customer does have BestFriend and has Address property
                        UnitTestsUtil.VerifyXPathExists(response,
                            "//atom:entry//atom:entry[contains(atom:id, 'Customer')]/atom:content/adsm:properties/ads:Address");

                        // Verify that we can correctly expand nav. property with top applied.
                        response = UnitTestsUtil.GetResponseAsAtomXLinq(request,
                            "/Customers?$top=3&$expand=Orders($select=ID)&$select=ID",
                            format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed/atom:entry/atom:link[@title='Orders']//atom:link[@rel='next']");
                    });
                }
            }

            [TestMethod, Variation]
            public void Paging_ExpansionsAndProjectionsNextLink()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) =>
                    {
                        config.SetEntitySetPageSize("Orders", 1);
                    };
                    request.DataServiceType = typeof(CustomDataContext);

                    // First level (that is Orders only)
                    // One property
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$select=Orders&$expand=Orders($select=ID)",
                        "ID", "");
                    // Implicit all properties
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$select=Orders&$expand=Orders",
                        "", "");
                    // No projections
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$expand=Orders",
                        "", "");
                    // Explicit all properties
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$select=Orders&$expand=Orders($select=*)",
                        "*", "");
                    // One property with parent property
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$select=Orders,ID&$expand=Orders($select=ID)",
                        "ID", "");
                    // Multiple properties
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$expand=Orders($select=ID,DollarAmount)",
                        "ID,DollarAmount", "");

                    // Second level (that is Orders and OrderDetails)
                    // Parent == the property with paging (Orders)
                    // Child == the second level property (OrderDetails)
                    // One property
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$expand=Orders($expand=OrderDetails($select=OrderID))",
                        "", "OrderDetails($select=OrderID)");
                    // Implicit all properties
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$expand=Orders($expand=OrderDetails)",
                        "", "OrderDetails");
                    // No projections
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$expand=Orders($expand=OrderDetails)",
                        "", "OrderDetails");
                    // Explicit all properties
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$expand=Orders($expand=OrderDetails($select=*))",
                        "", "OrderDetails($select=*)");
                    // One property with parent property
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$expand=Orders($expand=OrderDetails($select=OrderID);$select=ID)",
                        "ID,OrderDetails", "OrderDetails($select=OrderID)");
                    // Multiple properties
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?&$expand=Orders($expand=OrderDetails($select=OrderID,ProductID))",
                        "", "OrderDetails($select=OrderID,ProductID)");

                    // Second level advanced
                    // Parent == the property with paging (Orders)
                    // Child == the second level property (OrderDetails)
                    // Implicit subtree
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$select=Orders&$expand=Orders($expand=OrderDetails)",
                        "", "OrderDetails");
                    // Using * to remove expansion
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$select=Orders&$expand=Orders($select=*;$expand=OrderDetails)",
                        "*", "OrderDetails");
                    // Explicit all properties on parent and explicit all on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$select=Orders&$expand=Orders($select=*;$expand=OrderDetails($select=*))",
                        "*", "OrderDetails($select=*)");
                    // Explicit all properties on parent and implicit all on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$select=Orders&$expand=Orders($select=*;$expand=OrderDetails)",
                        "*", "OrderDetails");
                    // Explicit all properties on parent and one property on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$select=Orders&$expand=Orders($select=*;$expand=OrderDetails($select=Quantity))",
                        "*", "OrderDetails($select=Quantity)");
                    // Multiple properties on parent and all explicit on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$expand=Orders($select=ID,DollarAmount;$expand=OrderDetails($select=*))&$select=Orders",
                        "ID,DollarAmount,OrderDetails", "OrderDetails($select=*)");
                    // Multiple properties on parent and all implicit on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$expand=Orders($select=ID,DollarAmount;$expand=OrderDetails)&$select=Orders",
                        "ID,DollarAmount,OrderDetails", "OrderDetails");
                    // Multiple properties on parent and one property on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$expand=Orders($select=ID,DollarAmount;$expand=OrderDetails($select=OrderID))&$select=Orders",
                        "ID,DollarAmount,OrderDetails", "OrderDetails($select=OrderID)");

                    // Second level with grandparent
                    // Grandparent == the root of the query which is not tested in the result (Customers)
                    // Parent == the property with paging (Orders)
                    // Child == the second level property (OrderDetails)
                    // One property on grandparent, one property on parent and one property on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$expand=Orders,Orders($select=ID;$expand=OrderDetails($select=OrderID))&$select=ID,Orders",
                        "ID,OrderDetails", "OrderDetails($select=OrderID)");
                    // One property on grandparent, one property on parent and all explicit properties on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$expand=Orders,Orders($select=ID;$expand=OrderDetails($select=*))&$select=ID,Orders",
                        "ID,OrderDetails", "OrderDetails($select=*)");
                    // One property on grandparent, all explicit properties on parent and all explicit properties on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$expand=Orders,Orders($select=*;$expand=OrderDetails($select=*))&$select=ID,Orders",
                        "*", "OrderDetails($select=*)");
                    // One property on grandparent, all explicit properties on parent and one property on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$expand=Orders,Orders($select=*;$expand=OrderDetails($select=OrderID))&$select=ID,Orders",
                        "*", "OrderDetails($select=OrderID)");
                    // All explicit properties on grandparent (just one test case as this has no effect on the parent and child)
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Customers?$expand=Orders($select=*;$expand=OrderDetails($select=*))&$select=*,Orders",
                        "*", "OrderDetails($select=*)");
                }

                // For third and fourth level we need to use different data model which actually has such deep relationships
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) =>
                    {
                        config.SetEntitySetPageSize("Teams", 1);
                    };
                    request.DataServiceType = typeof(ReadOnlyTestContext);

                    // Third and fourth level
                    // Parent == the property with the paging (Teams in this case)
                    // Child == The fourth level property Teams/Players/Team/Players in this case
                    // One property on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Leagues?$select=Teams&$expand=Teams($expand=Players($expand=Team($expand=Players($select=ID))))",
                        "", "Players($expand=Team($expand=Players($select=ID)))");
                    // Implicit all properties on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Leagues?$select=Teams&$expand=Teams($expand=Players($expand=Team($expand=Players)))",
                        "", "Players($expand=Team($expand=Players))");
                    // Explicit all properties on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Leagues?$select=Teams&$expand=Teams($expand=Players($expand=Team($expand=Players($select=*))))",
                        "", "Players($expand=Team($expand=Players($select=*)))");

                    // Explicit all properties on parent, one property on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Leagues?$select=Teams&$expand=Teams($select=*;$expand=Players($expand=Team($expand=Players($select=ID))))",
                        "*", "Players($expand=Team($expand=Players($select=ID)))");
                    // Explicit all properties on parent, explicit all properties on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Leagues?$select=Teams&$expand=Teams($select=*;$expand=Players($expand=Team($expand=Players($select=*))))",
                        "*", "Players($expand=Team($expand=Players($select=*)))");
                    // Explicit all properties on parent, implicit all properties on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Leagues?$select=Teams&$expand=Teams($select=*;$expand=Players($expand=Team($expand=Players)))",
                        "*", "Players($expand=Team($expand=Players))");

                    // One property on parent, one property on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Leagues?$select=Teams&$expand=Teams($select=TeamID;$expand=Players($expand=Team($expand=Players($select=ID))))",
                        "TeamID,Players", "Players($expand=Team($expand=Players($select=ID)))");
                    // One property on parent, explicit all properties on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Leagues?$select=Teams&$expand=Teams($select=TeamID;$expand=Players($expand=Team($expand=Players($select=*))))",
                        "TeamID,Players", "Players($expand=Team($expand=Players($select=*)))");
                    // One property on parent, implicit all properties on child
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Leagues?$select=Teams&$expand=Teams($select=TeamID;$expand=Players($expand=Team($expand=Players)))",
                        "TeamID,Players", "Players($expand=Team($expand=Players))");

                    // Implicit all properties on parent
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Leagues?$select=Teams&$expand=Teams($expand=Players($expand=Team($expand=Players)))",
                        "", "Players($expand=Team($expand=Players))");
                    // Implicit all properties on second level
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Leagues?$select=Teams&$expand=Teams($expand=Players($expand=Team($expand=Players)))",
                        "", "Players($expand=Team($expand=Players))");
                    // Implicit all properties on third level
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Leagues?$select=Teams&$expand=Teams($select=Players;$expand=Players($select=Team;$expand=Team($expand=Players)))",
                        "Players", "Players($select=Team;$expand=Team($expand=Players))");

                    // Ensure that * is projected when implicit projections were used on child levels
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Leagues?$select=Teams&$expand=Teams($select=*;$expand=Players($select=ID,Team;$expand=Team($expand=Players)))",
                        "*", "Players($select=ID,Team;$expand=Team($expand=Players))");
                    // And the same one level deeper
                    VerifyInlinedNextLinkProjectionsAndExpansions(request,
                        "/Leagues?$select=Teams&$expand=Teams($expand=Players($select=*;$expand=Team($select=TeamID,Players;$expand=Players)))",
                        "", "Players($select=*;$expand=Team($select=TeamID,Players;$expand=Players))");
                }
            }

            private void VerifyInlinedNextLinkProjectionsAndExpansions(TestWebRequest request, string query, string projections, string expansions)
            {
                var response = UnitTestsUtil.GetResponseAsAtom(request, query, UnitTestsUtil.AtomFormat);
                string nextLink = UnitTestsUtil.GetXPathValue(response, "/atom:feed/atom:entry/atom:link/adsm:inline/atom:feed/atom:link[@rel='next']/@href");

                VerifyPathsInQueryOption(nextLink, "select", projections);
                VerifyPathsInQueryOption(nextLink, "expand", expansions);
            }

            private void VerifyPathsInQueryOption(string uri, string queryOption, string expectedPaths)
            {
                Match match = Regex.Match(uri, @"[\&\?]+\$" + queryOption + @"=([^\&]*).*");
                string queryOptionValue = match.Groups[1].Value;

                string[] pathsActual = queryOptionValue.Split(',');
                string[] pathsExpected = expectedPaths.Split(',');
                List<string> pathsExpectedNotFound = new List<string>(pathsExpected);

                foreach (string path in pathsActual)
                {
                    if (pathsExpected.Contains(path))
                    {
                        pathsExpectedNotFound.Remove(path);
                    }
                    else
                    {
                        Assert.Fail(string.Format("Unexpected path '{0}' found in ${1} in uri {2}.",
                            path, queryOption, uri));
                    }
                }

                foreach (string path in pathsExpectedNotFound)
                {
                    Assert.Fail(string.Format("Expected path '{0}' not found in ${1} in uri {2}.",
                        path, queryOption, uri));
                }
            }

            #region Projections_Interceptors
            public abstract class InterceptorsServiceType
            {
                public abstract Type DataSourceType { get;}
                public abstract Type FirstCustomerFilteredServiceType { get; }
                public abstract Type AllButFirstCustomerFilteredServiceType { get; }
                public abstract Type FirstOrderFilteredServiceType { get; }
                public abstract Type FirstCustomerFilteredComplexServiceType { get; }
            }

            public class CustomDataContextInterceptorServiceType : InterceptorsServiceType
            {
                public class FirstCustomerFiltered : OpenWebDataService<CustomDataContext>
                {
                    [QueryInterceptor("Customers")]
                    public Expression<Func<Customer, bool>> CustomerFilter()
                    {
                        return (p) => p.ID == 0;
                    }
                }

                public class AllButFirstCustomerFiltered : OpenWebDataService<CustomDataContext>
                {
                    [QueryInterceptor("Customers")]
                    public Expression<Func<Customer, bool>> CustomerFilter()
                    {
                        return (p) => p.ID != 0;
                    }
                }

                public class FirstOrderFiltered : OpenWebDataService<CustomDataContext>
                {
                    [QueryInterceptor("Orders")]
                    public Expression<Func<Order, bool>> OrderFilter()
                    {
                        return (p) => p.ID == 0;
                    }
                }

                public class FirstCustomerFilteredComplex : OpenWebDataService<CustomDataContext>
                {
                    static private bool CustomerFilterHelper(Customer c)
                    {
                        return c.ID == 0;
                    }

                    [QueryInterceptor("Customers")]
                    public Expression<Func<Customer, bool>> CustomerFilter()
                    {
                        return (p) => CustomerFilterHelper(p);
                    }
                }

                public override Type DataSourceType { get { return typeof(CustomDataContext); } }
                public override Type FirstCustomerFilteredServiceType { get { return typeof(FirstCustomerFiltered); } }
                public override Type AllButFirstCustomerFilteredServiceType { get { return typeof(AllButFirstCustomerFiltered); } }
                public override Type FirstOrderFilteredServiceType { get { return typeof(FirstOrderFiltered); } }
                public override Type FirstCustomerFilteredComplexServiceType { get { return typeof(FirstCustomerFilteredComplex); } }
            }

            public class CustomRowBasedContextInterceptorServiceType : InterceptorsServiceType
            {
                public class FirstCustomerFiltered : OpenWebDataService<CustomRowBasedContext>
                {
                    [QueryInterceptor("Customers")]
                    public Expression<Func<RowEntityTypeWithIDAsKey, bool>> CustomerFilter()
                    {
                        return (p) => p.ID == 0;
                    }
                }

                public class AllButFirstCustomerFiltered : OpenWebDataService<CustomRowBasedContext>
                {
                    [QueryInterceptor("Customers")]
                    public Expression<Func<RowEntityTypeWithIDAsKey, bool>> CustomerFilter()
                    {
                        return (p) => p.ID != 0;
                    }
                }

                public class FirstOrderFiltered : OpenWebDataService<CustomRowBasedContext>
                {
                    [QueryInterceptor("Orders")]
                    public Expression<Func<RowEntityTypeWithIDAsKey, bool>> OrderFilter()
                    {
                        return (p) => p.ID == 0;
                    }
                }

                public class FirstCustomerFilteredComplex : OpenWebDataService<CustomRowBasedContext>
                {
                    static private bool CustomerFilterHelper(RowEntityTypeWithIDAsKey c)
                    {
                        return c.ID == 0;
                    }

                    [QueryInterceptor("Customers")]
                    public Expression<Func<RowEntityTypeWithIDAsKey, bool>> CustomerFilter()
                    {
                        return (p) => CustomerFilterHelper(p);
                    }
                }

                public override Type DataSourceType { get { return typeof(CustomRowBasedContext); } }
                public override Type FirstCustomerFilteredServiceType { get { return typeof(FirstCustomerFiltered); } }
                public override Type AllButFirstCustomerFilteredServiceType { get { return typeof(AllButFirstCustomerFiltered); } }
                public override Type FirstOrderFilteredServiceType { get { return typeof(FirstOrderFiltered); } }
                public override Type FirstCustomerFilteredComplexServiceType { get { return typeof(FirstCustomerFilteredComplex); } }
            }

            public class CustomObjectContextInterceptorServiceType : InterceptorsServiceType
            {
                public class FirstCustomerFiltered : OpenWebDataService<ocs.CustomObjectContext>
                {
                    [QueryInterceptor("Customers")]
                    public Expression<Func<ocs.Hidden.Customer, bool>> CustomerFilter()
                    {
                        return (p) => p.ID == 0;
                    }
                }

                public class AllButFirstCustomerFiltered : OpenWebDataService<ocs.CustomObjectContext>
                {
                    [QueryInterceptor("Customers")]
                    public Expression<Func<ocs.Hidden.Customer, bool>> CustomerFilter()
                    {
                        return (p) => p.ID != 0;
                    }
                }

                public class FirstOrderFiltered : OpenWebDataService<ocs.CustomObjectContext>
                {
                    [QueryInterceptor("Orders")]
                    public Expression<Func<ocs.Hidden.Order, bool>> OrderFilter()
                    {
                        return (p) => p.ID == 0;
                    }
                }

                public class FirstCustomerFilteredComplex : OpenWebDataService<ocs.CustomObjectContext>
                {
                    static private bool CustomerFilterHelper(ocs.Hidden.Customer c)
                    {
                        return c.ID == 0;
                    }

                    [QueryInterceptor("Customers")]
                    public Expression<Func<ocs.Hidden.Customer, bool>> CustomerFilter()
                    {
                        return (p) => CustomerFilterHelper(p);
                    }
                }

                public override Type DataSourceType { get { return typeof(ocs.CustomObjectContext); } }
                public override Type FirstCustomerFilteredServiceType { get { return typeof(FirstCustomerFiltered); } }
                public override Type AllButFirstCustomerFilteredServiceType { get { return typeof(AllButFirstCustomerFiltered); } }
                public override Type FirstOrderFilteredServiceType { get { return typeof(FirstOrderFiltered); } }
                public override Type FirstCustomerFilteredComplexServiceType { get { return typeof(FirstCustomerFilteredComplex); } }
            }
            #endregion

            [TestMethod, Variation]
            public void Projections_Interceptors()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("ServiceType", new InterceptorsServiceType[] {
                        new CustomDataContextInterceptorServiceType(),
                        new CustomRowBasedContextInterceptorServiceType(),
                        new CustomObjectContextInterceptorServiceType()
                    }),
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                using (TestUtil.MetadataCacheCleaner())
                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        string format = (string)values["Format"];
                        InterceptorsServiceType serviceType = (InterceptorsServiceType)values["ServiceType"];

                        var response = UnitTestsUtil.GetResponseAsAtom(serviceType.FirstCustomerFilteredServiceType,
                            "/Customers?$select=Name", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed[count(atom:entry)=1]",
                            "/atom:feed/atom:entry[.//ads:Name='Customer 0']");
                        response = UnitTestsUtil.GetResponseAsAtom(serviceType.AllButFirstCustomerFilteredServiceType,
                            "/Customers?$select=Name", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed[count(atom:entry)=2]",
                            "/atom:feed/atom:entry[.//ads:Name='Customer 1']",
                            "/atom:feed/atom:entry[.//ads:Name='Customer 2']");

                        response = UnitTestsUtil.GetResponseAsAtom(serviceType.FirstCustomerFilteredServiceType,
                            "/Customers?$expand=BestFriend($select=ID),Orders($select=ID)&$select=BestFriend,Orders", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            format == UnitTestsUtil.AtomFormat ?
                                "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline[count(*)=0]" :
                                "/atom:feed/atom:entry//ads:BestFriend[@adsm:null='true']",
                            "/atom:feed/atom:entry/atom:link[@title='Orders']/adsm:inline[count(.//ads:ID)=2]");

                        response = UnitTestsUtil.GetResponseAsAtom(serviceType.FirstCustomerFilteredServiceType,
                            "/Customers?$expand=BestFriend($expand=BestFriend)", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            format == UnitTestsUtil.AtomFormat ?
                                "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline[count(*)=0]" :
                                "/atom:feed/atom:entry//ads:BestFriend[@adsm:null='true']");

                        response = UnitTestsUtil.GetResponseAsAtom(serviceType.FirstCustomerFilteredServiceType,
                            "/Customers?$expand=BestFriend($expand=BestFriend)&$orderby=BestFriend/ID", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            format == UnitTestsUtil.AtomFormat ?
                                "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline[count(*)=0]" :
                                "/atom:feed/atom:entry//ads:BestFriend[@adsm:null='true']");

                        response = UnitTestsUtil.GetResponseAsAtom(serviceType.FirstCustomerFilteredServiceType,
                            "/Customers?$expand=BestFriend($expand=BestFriend($select=ID))&$select=BestFriend&$orderby=BestFriend/ID", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            format == UnitTestsUtil.AtomFormat ?
                                "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline[count(*)=0]" :
                                "/atom:feed/atom:entry//ads:BestFriend[@adsm:null='true']");

                        response = UnitTestsUtil.GetResponseAsAtom(serviceType.FirstCustomerFilteredServiceType,
                            "/Customers?$expand=BestFriend($select=ID;$expand=BestFriend)&$orderby=BestFriend/BestFriend/ID&$select=BestFriend", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            format == UnitTestsUtil.AtomFormat ?
                                "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline[count(*)=0]" :
                                "/atom:feed/atom:entry//ads:BestFriend[@adsm:null='true']");

                        response = UnitTestsUtil.GetResponseAsAtom(serviceType.AllButFirstCustomerFilteredServiceType,
                            "/Customers?$expand=BestFriend($select=ID;$expand=BestFriend)&$orderby=BestFriend/BestFriend/ID&$select=BestFriend", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            format == UnitTestsUtil.AtomFormat ?
                                "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline[count(*)=0]" :
                                "/atom:feed/atom:entry//ads:BestFriend[@adsm:null='true']");

                        response = UnitTestsUtil.GetResponseAsAtom(serviceType.FirstCustomerFilteredServiceType,
                            "/Customers?$expand=BestFriend($expand=BestFriend($select=ID))&$orderby=BestFriend/BestFriend/ID", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            format == UnitTestsUtil.AtomFormat ?
                                "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline[count(*)=0]" :
                                "/atom:feed/atom:entry//ads:BestFriend[@adsm:null='true']");

                        response = UnitTestsUtil.GetResponseAsAtom(serviceType.AllButFirstCustomerFilteredServiceType,
                            "/Customers?$expand=BestFriend($expand=BestFriend)&$orderby=BestFriend/BestFriend/ID", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            format == UnitTestsUtil.AtomFormat ?
                                "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline[count(*)=0]" :
                                "/atom:feed/atom:entry//ads:BestFriend[@adsm:null='true']");

                        if (serviceType.DataSourceType != typeof(ocs.CustomObjectContext))
                        {
                            response = UnitTestsUtil.GetResponseAsAtom(serviceType.FirstCustomerFilteredComplexServiceType,
                                "/Customers?$expand=BestFriend($expand=BestFriend($select=ID))&$select=BestFriend&$orderby=BestFriend/ID", format);
                            UnitTestsUtil.VerifyXPathExists(response,
                                format == UnitTestsUtil.AtomFormat ?
                                    "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline[count(*)=0]" :
                                    "/atom:feed/atom:entry//ads:BestFriend[@adsm:null='true']");
                        }

                        response = UnitTestsUtil.GetResponseAsAtom(serviceType.AllButFirstCustomerFilteredServiceType,
                            "/Customers?$expand=BestFriend($expand=BestFriend)&$orderby=BestFriend/ID add 3", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            format == UnitTestsUtil.AtomFormat ?
                                "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline[count(*)=0]" :
                                "/atom:feed/atom:entry//ads:BestFriend[@adsm:null='true']");

                        response = UnitTestsUtil.GetResponseAsAtom(serviceType.AllButFirstCustomerFilteredServiceType,
                            "/Customers?$expand=BestFriend($expand=BestFriend($select=ID))&$select=BestFriend&$orderby=BestFriend/ID add 3", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            format == UnitTestsUtil.AtomFormat ?
                                "/atom:feed/atom:entry/atom:link[@title='BestFriend']/adsm:inline[count(*)=0]" :
                                "/atom:feed/atom:entry//ads:BestFriend[@adsm:null='true']");

                        response = UnitTestsUtil.GetResponseAsAtom(serviceType.FirstOrderFilteredServiceType,
                            "/Customers?$expand=BestFriend($expand=Orders)&$orderby=BestFriend/ID", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed[count(./atom:entry/atom:link[@title='Orders' and count(*)!=0])=0]",
                            "/atom:feed[count(.//atom:entry[contains(atom:id, 'Orders(0)')])=1]");

                        response = UnitTestsUtil.GetResponseAsAtom(serviceType.FirstOrderFilteredServiceType,
                            "/Customers?$expand=BestFriend($expand=Orders($select=ID))&$orderby=BestFriend/ID&$select=BestFriend", format);
                        UnitTestsUtil.VerifyXPathExists(response,
                            "/atom:feed[count(./atom:entry/atom:link[@title='Orders' and count(*)!=0])=0]",
                            "/atom:feed[count(.//atom:entry[contains(atom:id, 'Orders(0)')])=1]");

                        using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                        {
                            OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) =>
                            {
                                config.SetEntitySetPageSize("Customers", 2);
                            };

                            response = UnitTestsUtil.GetResponseAsAtom(serviceType.FirstCustomerFilteredServiceType,
                                "/Customers?$select=Name, BestFriend&$expand=BestFriend($expand=BestFriend($select=Name))", format);
                            UnitTestsUtil.VerifyXPathResultCount(response, 1,
                                "/atom:feed/atom:entry");

                            if (serviceType.DataSourceType != typeof(ocs.CustomObjectContext))
                            {
                                response = UnitTestsUtil.GetResponseAsAtom(serviceType.FirstCustomerFilteredComplexServiceType,
                                    "/Customers?$select=Name, BestFriend&$expand=BestFriend($expand=BestFriend($select=Name))", format);
                                UnitTestsUtil.VerifyXPathResultCount(response, 1,
                                    "/atom:feed/atom:entry");
                            }
                        }

                        using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                        {
                            OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) =>
                            {
                                config.SetEntitySetPageSize("Orders", 2);
                            };

                            response = UnitTestsUtil.GetResponseAsAtom(serviceType.FirstCustomerFilteredServiceType,
                                "/Customers?$select=Name,Orders&$expand=Orders($select=ID)", format);
                            if (serviceType.DataSourceType != typeof(ocs.CustomObjectContext))
                            {
                                response = UnitTestsUtil.GetResponseAsAtom(serviceType.FirstCustomerFilteredComplexServiceType,
                                    "/Customers?$select=Name,Orders&$expand=Orders($select=ID)", format);
                            }
                        }
                    });
                }
            }

            #region Projections_ServiceOperations
            public class Projections_ServiceOperations_CustomDataContextService : OpenWebDataService<CustomDataContext>
            {
                [WebGet]
                public IQueryable<Customer> GetCustomerByCity(string city)
                {
                    return this.CurrentDataSource.Customers.Where(c => c.Address.City == city);
                }
            }

            public class Projections_ServiceOperations_CustomerObjectContextService : OpenWebDataService<ocs.CustomObjectContext>
            {
                [WebGet]
                public IQueryable<ocs.Hidden.Customer> GetCustomerByCity(string city)
                {
                    return this.CurrentDataSource.Customers;
                }
            }

            [TestMethod, Variation]
            public void Projections_ServiceOperations()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Format", UnitTestsUtil.ResponseFormats),
                    new Dimension("ServiceType", new Type[] { 
                        typeof(Projections_ServiceOperations_CustomDataContextService), 
                        typeof(Projections_ServiceOperations_CustomerObjectContextService), 
                        typeof(CustomRowBasedContext), 
                        typeof(CustomRowBasedOpenTypesContext) }));

                using (ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        string format = (string)values["Format"];
                        using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                        using (TestUtil.MetadataCacheCleaner())
                        {
                            request.DataServiceType = (Type)values["ServiceType"];

                            XmlDocument response;
                            // Verify that simple projections on top of SO work
                            response = UnitTestsUtil.GetResponseAsAtom(request, 
                                "/GetCustomerByCity?city='Redmond'&$select=Name", 
                                format);
                            UnitTestsUtil.VerifyXPathResultCount(response, 3,
                                "//atom:entry//ads:Name",
                                "//atom:entry[count(//adsm:properties/*[local-name()!='Name'])=0]");

                            // Verify that expansions with projections work on top of SO
                            response = UnitTestsUtil.GetResponseAsAtom(request,
                                "/GetCustomerByCity?city='Redmond'&$select=Name,Orders&$expand=Orders($select=ID)",
                                format);
                            UnitTestsUtil.VerifyXPathResultCount(response, 3,
                                "/atom:feed/atom:entry/atom:content/adsm:properties/ads:Name",
                                "/atom:feed/atom:entry[count(./atom:content/adsm:properties/*[local-name()!='Name'])=0]");
                            UnitTestsUtil.VerifyXPathResultCount(response, 6,
                                "/atom:feed/atom:entry/atom:link[@title='Orders']//atom:entry//ads:ID[1]",
                                "/atom:feed/atom:entry/atom:link[@title='Orders']//atom:entry[count(./atom:content/adsm:properties/*[local-name()!='ID'])=0]");
                        }

                        using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                        using (TestUtil.MetadataCacheCleaner())
                        using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                        {
                            request.DataServiceType = (Type)values["ServiceType"];
                            OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) =>
                            {
                                config.SetEntitySetPageSize("Customers", 1);
                                config.SetEntitySetPageSize("Orders", 1);
                            };

                            XmlDocument response;
                            // Verify that projections work even when SDP is on and over SO
                            response = UnitTestsUtil.GetResponseAsAtom(request,
                                "/GetCustomerByCity?city='Redmond'&$select=Name",
                                format);
                            UnitTestsUtil.VerifyXPathResultCount(response, 1,
                                "//atom:entry//ads:Name");

                            // Verify that both expansions and projections work with SDP on the root and expanded level
                            response = UnitTestsUtil.GetResponseAsAtom(request,
                                "/GetCustomerByCity?city='Redmond'&$select=Name,Orders&$expand=Orders($select=ID)",
                                format);
                            UnitTestsUtil.VerifyXPathResultCount(response, 1,
                                "/atom:feed/atom:entry",
                                "/atom:feed/atom:entry/atom:link[@title='Orders']//atom:entry");
                        }
                    });
                }
            }

            #endregion

            private void VerifyETags(XmlDocument document)
            {
                UnitTestsUtil.VerifyXPaths(document, "count(/atom:feed/atom:entry/@adsm:etag)!=0");
            }

            public class CustomDataContextWithExpand : CustomDataContext, IExpandProvider
            {
                public IEnumerable ApplyExpansions(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths)
                {
                    Debug.Assert(queryable.ElementType == typeof(Customer), "Only works for customer");

                    ExpandSegment orderSegment = GetExpandSegment(expandPaths, "Orders");
                    ExpandSegment bestFriendSegment = GetExpandSegment(expandPaths, "BestFriend");

                    // Do the actual expansion.
                    if (orderSegment != null || bestFriendSegment != null)
                    {
                        return new ExpandedCustomerEnumerable(
                            from c in this.Customers
                            select new ExpandedCustomer()
                            {
                                Customer = c,
                                Orders = orderSegment != null ? ((orderSegment.MaxResultsExpected != Int32.MaxValue) ? c.Orders.Take(orderSegment.MaxResultsExpected): c.Orders) : null,
                                BestFriend = bestFriendSegment != null ? c.BestFriend : null
                            }
                            );
                    }

                    throw new Exception("unexpected path for test.");
                }

                private ExpandSegment GetExpandSegment(ICollection<ExpandSegmentCollection> expandPaths, string propertyName)
                {
                    foreach (ExpandSegmentCollection esc in expandPaths)
                        foreach (ExpandSegment es in esc)
                        {
                            if (es.Name == propertyName)
                            {
                                return es;
                            }
                        }

                    return null;
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

            [TestMethod, Variation("Verify that the nested expands with paging works when there is a custom IExpandProvider")]
            public void NestedPagingWithCustomIExpandProvider()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) =>
                    {
                        config.SetEntitySetPageSize("Orders", 1);
                    };

                    request.DataServiceType = typeof(CustomDataContextWithExpand);
                    request.Accept = UnitTestsUtil.AtomFormat;

                    // Nested Paging with expands is allowed with custom IExpandProvider
                    var response = UnitTestsUtil.GetResponseAsAtom(request, "/Customers?$expand=Orders");
                    UnitTestsUtil.VerifyXPathExists(response,
                        "//atom:entry[contains(atom:id, 'Orders')]", //Orders are expanded
                        "/atom:feed/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed[count(atom:entry) = 1]" //There must be only one orders expanded
                        );
                }
            }

            [TestMethod, Variation("Verify that ETag null values are correctly serialized even when projections are applied.")]
            public void Projections_NullETagValue()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                using (TestUtil.RestoreStaticMembersOnDispose(typeof(ocs.PopulateData)))
                {
                    ocs.PopulateData.CustomersCustomizer = (customers, context) =>
                    {
                        customers[0].Concurrency = null;
                    };

                    using (TestUtil.MetadataCacheCleaner())
                    using (ocs.PopulateData.CreateTableAndPopulateData())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                        {
                            request.Accept = (string)values["Format"];
                            request.DataServiceType = typeof(ocs.CustomObjectContext);

                            // Select single property
                            var response = UnitTestsUtil.GetResponseAsAtom(request, "/Customers?$select=Name");
                            VerifyETags(response);
                            // Select all properties
                            response = UnitTestsUtil.GetResponseAsAtom(request, "/Customers?$select=*");
                            VerifyETags(response);
                            // No projections
                            response = UnitTestsUtil.GetResponseAsAtom(request, "/Customers");
                            VerifyETags(response);
                            // Expand and projection
                            response = UnitTestsUtil.GetResponseAsAtom(request, "/Customers?$select=*,BestFriend&$expand=BestFriend($select=Name)");
                            VerifyETags(response);
                        });
                    }
                }
            }
        }
    }
}
