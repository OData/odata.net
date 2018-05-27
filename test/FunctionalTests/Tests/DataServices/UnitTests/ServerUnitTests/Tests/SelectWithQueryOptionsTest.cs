//---------------------------------------------------------------------
// <copyright file="SelectWithQueryOptionsTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region usings
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Objects;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
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
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Microsoft.OData.Client;
    #endregion

    [TestModule]
    public partial class UnitTestModule : AstoriaTestModule
    {
        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
        [Ignore] // Remove Atom
        // [TestClass, TestCase]
        public class SelectTestWithQueryOptions : AstoriaTestCase
        {
            public class SelectDescription
            {
                public SelectDescription(string select, params string[] verificationXPaths)
                {
                    this.Select = select;
                    this.VerificationXPaths = verificationXPaths;
                }

                public string Select { get; set; }
                public string[] VerificationXPaths { get; set; }
            }

            private static SelectDescription[] CustomerSelects = new SelectDescription[] {
                // Top simple
                new SelectDescription("Name",
                    "*[count(/atom:feed/atom:entry/atom:content/adsm:properties[count(ads:Name)=0])=0]",
                    "*[count(/atom:feed/atom:entry/atom:link[@rel!='edit'])=0]",
                    "*[count(/atom:feed/atom:entry/atom:content/adsm:properties[count(*[local-name()!='Name'])!=0])=0]"),
                // Top complex
                new SelectDescription("Address",
                    "*[count(/atom:feed/atom:entry/atom:content/adsm:properties[count(ads:Address)=0])=0] | *[count(/atom:feed/atom:entry[count(atom:link[@title='Address'])=0])=0]",
                    "*[count(/atom:feed/atom:entry/atom:content/adsm:properties[count(*[local-name()!='Address'])!=0])=0]"),
                // Top navigation
                new SelectDescription("Orders",
                    "*[count(/atom:feed/atom:entry[count(atom:link[@title='Orders'])=0])=0]",
                    "*[count(/atom:feed/atom:entry/atom:content/adsm:properties[count(*)!=0])=0]"),
                // Nav simple
                new SelectDescription("ID&$expand=Orders($select=ID)",
                    "*[count(/atom:feed/atom:entry/atom:link[@title='Orders']//atom:entry/atom:content/adsm:properties[count(ads:ID)=0])=0]",
                    "*[count(/atom:feed/atom:entry/atom:link[@title='Orders']//atom:entry/atom:content/adsm:properties[count(*[local-name()!='ID'])!=0])=0]"),
                // Nav complex
                new SelectDescription("ID&$expand=BestFriend($select=Address)",
                    "*[count(/atom:feed/atom:entry/atom:content/adsm:properties[count(ads:ID)=0])=0]"),
                // Top all, nav all
                new SelectDescription("*,Orders&$expand=Orders",
                    "*[count(/atom:feed/atom:entry/atom:content/adsm:properties[count(ads:ID)=0])=0]",
                    "*[count(/atom:feed/atom:entry/atom:content/adsm:properties[count(ads:Name)=0])=0]"),
                // Nav all, deep
                new SelectDescription("*&$expand=BestFriend($select=*;$expand=BestFriend($expand=Orders($select=*)))",
                    "*[count(/atom:feed/atom:entry/atom:content/adsm:properties[count(ads:ID)=0])=0]")
            };

            private void VerifyEntryIDsAndXPaths(string requestUri, TestWebRequest request, int[] expectedIDs, params string[] xpaths)
            {
                var response = UnitTestsUtil.GetResponseAsAtomXLinq(request, requestUri);
                List<string> ids = response.Root.Elements(UnitTestsUtil.AtomNamespace + "entry").Elements(UnitTestsUtil.AtomNamespace + "id")
                                        .Select(e => (string)e).ToList();
                Assert.AreEqual(expectedIDs.Length, ids.Count, "The number of returned entries doesn't match.");
                for (int i = 0; i < expectedIDs.Length; i++)
                {
                    if (!ids[i].Contains("(" + expectedIDs[i].ToString() + ")"))
                    {
                        Assert.Fail("Entries not reported correctly, \r\nexpected: " + expectedIDs.Select(n => n.ToString()).Concatenate(", ") +
                            "\r\nactual: " + ids.Concatenate(", ") +
                            "\r\n" + response.ToString());
                    }
                }

                UnitTestsUtil.VerifyXPathExists(response, xpaths);
            }

            #region $orderby
            public class OrderBySetting
            {
                public OrderBySetting(string orderby, params int[] orderedIDs)
                {
                    this.OrderBy = orderby;
                    this.OrderedIDs = orderedIDs;
                }

                public string OrderBy { get; set; }
                public int[] OrderedIDs { get; set; }
                public override string ToString()
                {
                    return "$orderby=" + this.OrderBy;
                }
            }

            [TestMethod, Variation]
            public void Projections_OrderBy()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                using (TestUtil.MetadataCacheCleaner())
                using (ocs.PopulateData.CreateTableAndPopulateData())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        request.DataServiceType = (Type)values["DataServiceType"];
                        request.Accept = (string)values["Format"];
                        TestUtil.RunCombinatorialEngineFail(CombinatorialEngine.FromDimensions(
                            new Dimension("OrderBy", new OrderBySetting[] {
                                new OrderBySetting("ID", 0, 1, 2),
                                new OrderBySetting("Name", 0, 1, 2),
                                new OrderBySetting("length(Name) sub ID", 2, 1, 0),
                                new OrderBySetting("BestFriend/ID", 0, 1, 2),
                                new OrderBySetting("BestFriend/Name,2 sub BestFriend/BestFriend/ID", 0, 1, 2),
                                new OrderBySetting("2 sub BestFriend/BestFriend/ID desc", 2, 0, 1)
                            }),
                            new Dimension("Select", CustomerSelects)), values2 =>
                        {
                            OrderBySetting orderby = (OrderBySetting)values2["OrderBy"];
                            SelectDescription select = (SelectDescription)values2["Select"];

                            VerifyEntryIDsAndXPaths(
                                "/Customers?$select=" + select.Select + "&$orderby=" + orderby.OrderBy, 
                                request, 
                                orderby.OrderedIDs, 
                                select.VerificationXPaths);
                        });

                        VerifyEntryIDsAndXPaths("/Customers(1)/Orders?$select=DollarAmount&$orderby=ID", request, new int[] { 1, 101 });
                    });
                }
            }
            #endregion

            #region $filter
            public class FilterSetting
            {
                public FilterSetting(string filter, params int[] filteredIDs)
                {
                    this.Filter = filter;
                    this.FilteredIDs = filteredIDs;
                }

                public string Filter { get; set; }
                public int[] FilteredIDs { get; set; }
                public override string ToString()
                {
                    return "$filter=" + this.Filter;
                }
            }

            [TestMethod, Variation]
            public void Projections_Filter()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                using (TestUtil.MetadataCacheCleaner())
                using (ocs.PopulateData.CreateTableAndPopulateData())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        request.DataServiceType = (Type)values["DataServiceType"];
                        request.Accept = (string)values["Format"];
                        TestUtil.RunCombinatorialEngineFail(CombinatorialEngine.FromDimensions(
                            new Dimension("Filter", new FilterSetting[] {
                                new FilterSetting("ID eq 1", 1),
                                new FilterSetting("ID gt 0", 1, 2),
                                new FilterSetting("length(Name) sub ID gt 9", 0),
                                new FilterSetting("BestFriend/ID eq 1", 2),
                                new FilterSetting("2 sub BestFriend/BestFriend/ID gt 0", 2)
                            }),
                            new Dimension("Select", CustomerSelects)), values2 =>
                        {
                            FilterSetting filter = (FilterSetting)values2["Filter"];
                            SelectDescription select = (SelectDescription)values2["Select"];

                            VerifyEntryIDsAndXPaths(
                                "/Customers?$select=" + select.Select + "&$filter=" + filter.Filter, 
                                request, 
                                filter.FilteredIDs,
                                select.VerificationXPaths);
                        });

                        VerifyEntryIDsAndXPaths("/Customers(1)/Orders?$select=DollarAmount&$filter=ID gt 1", request, new int[] { 101 });
                        VerifyEntryIDsAndXPaths(
                            "/Customers?$select=BestFriend&$expand=BestFriend($select=ID)&$filter=ID gt 0&$orderby=BestFriend/ID desc", 
                            request, new int[] { 2, 1 });
                    });
                }
            }
            #endregion

            #region $top and $skip
            public class TopSkipSetting
            {
                public TopSkipSetting(string top, string skip, params int[] ids)
                {
                    this.Top = top;
                    this.Skip = skip;
                    this.IDs = ids;
                }

                public TopSkipSetting(string top, string skip, string orderby, params int[] ids)
                {
                    this.Top = top;
                    this.Skip = skip;
                    this.OrderBy = orderby;
                    this.IDs = ids;
                }

                public string Top { get; set; }
                public string Skip { get; set; }
                public string OrderBy { get; set; }
                public int[] IDs { get; set; }
                public override string ToString()
                {
                    return (this.Top != null ? "$top=" + this.Top : "") + "&" + (this.Skip != null ? "$skip=" + this.Skip : "") +
                        "&" + (this.OrderBy != null ? "$orderby" + this.OrderBy : "");
                }
            }

            [TestMethod, Variation]
            public void Projections_TopSkip()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("DataServiceType", new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) }),
                    new Dimension("Format", UnitTestsUtil.ResponseFormats));

                using (TestUtil.MetadataCacheCleaner())
                using (ocs.PopulateData.CreateTableAndPopulateData())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        request.DataServiceType = (Type)values["DataServiceType"];
                        request.Accept = (string)values["Format"];
                        TestUtil.RunCombinatorialEngineFail(CombinatorialEngine.FromDimensions(
                            new Dimension("TopSkip", new TopSkipSetting[] {
                                new TopSkipSetting("2", null, 0, 1),
                                new TopSkipSetting(null, "1", 1, 2),
                                new TopSkipSetting(null, "3"),
                                new TopSkipSetting("1", "1", 1),
                                new TopSkipSetting("1", null, "ID desc", 2),
                                new TopSkipSetting(null, "1", "ID desc", 1, 0),
                                new TopSkipSetting("1", "2", "ID desc", 0)
                            }),
                            new Dimension("Select", CustomerSelects)), values2 =>
                        {
                            TopSkipSetting topskip = (TopSkipSetting)values2["TopSkip"];
                            SelectDescription select = (SelectDescription)values2["Select"];

                            VerifyEntryIDsAndXPaths(
                                "/Customers?$select=" + select.Select + 
                                    (topskip.Top != null ? "&$top=" + topskip.Top : "") + 
                                    (topskip.Skip != null ? "&$skip=" + topskip.Skip : "") +
                                    (topskip.OrderBy != null ? "&$orderby=" + topskip.OrderBy : ""), 
                                request, 
                                topskip.IDs,
                                select.VerificationXPaths);
                        });

                        VerifyEntryIDsAndXPaths("/Customers(1)/Orders?$select=ID&$top=1&$skip=1&$orderby=ID", request, new int[] { 101 });
                    });
                }
            }
            #endregion

            #region Batch
            [TestMethod, Variation]
            public void Projections_Batch()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (ocs.PopulateData.CreateTableAndPopulateData())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    TestUtil.RunCombinations(
                        new Type[] { typeof(CustomDataContext), typeof(ocs.CustomObjectContext), typeof(CustomRowBasedContext), typeof(CustomRowBasedOpenTypesContext) },
                        CustomerSelects.Variations(2),
                        (dataServiceType, selects) =>
                    {
                        request.DataServiceType = dataServiceType;

                        BatchWebRequest batchRequest = new BatchWebRequest();
                        foreach (var select in selects)
                        {
                            InMemoryWebRequest part = new InMemoryWebRequest();
                            part.Accept = "application/atom+xml,application/xml";
                            part.RequestUriString = "/Customers?$select=" + select.Select;
                            batchRequest.Parts.Add(part);
                        }

                        batchRequest.SendRequest(request);

                        for (int i = 0; i < selects.Length; i++)
                        {
                            UnitTestsUtil.VerifyXPathExists(UnitTestsUtil.GetResponseAsAtomXLinq(batchRequest.Parts[i]), selects[i].VerificationXPaths);
                        }
                    });
                }
            }
            #endregion
        }
    }
}