//---------------------------------------------------------------------
// <copyright file="CreateODataWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using Microsoft.OData.Service;
    using System.Net;
    using AstoriaUnitTests.Stubs;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using test = System.Data.Test.Astoria;
    using BindingFlags = System.Reflection.BindingFlags;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
    [TestClass]
    public class CreateODataWriterTests
    {
        [Ignore] // Remove Atom
        // [TestMethod]
        public void CreateODataWriterDelegateTest()
        {
            int createODataWriterDelegateCount = 0;
            var testInfo = new[] {
                    new { Query = "/Customers?$format=atom", CustomerCount = 3, NavLinkCount = 6, OrderCount = 0 },
                    new { Query = "/Customers?$format=atom&$expand=BestFriend", CustomerCount = 5, NavLinkCount = 10, OrderCount = 0 },
                    new { Query = "/Customers?$format=atom&$expand=Orders", CustomerCount = 3, NavLinkCount = 18, OrderCount = 6 },
                    new { Query = "/Customers(1)?$format=atom", CustomerCount = 1, NavLinkCount = 2, OrderCount = 0 },
                    new { Query = "/Customers(1)?$format=atom&$expand=Orders", CustomerCount = 1, NavLinkCount = 6, OrderCount = 2 },
                    new { Query = "/Customers(1)/Orders?$format=atom", CustomerCount = 0, NavLinkCount = 4, OrderCount = 2 },
                };

            using (OpenWebDataServiceHelper.CreateODataWriterDelegate.Restore())
            using (MyODataWriter.WriteEntryStart.Restore())
            using (MyODataWriter.WriteLinkStart.Restore())
            using (var request = TestWebRequest.CreateForInProcess())
            {
                createODataWriterDelegateCount = 0;
                request.HttpMethod = "GET";
                request.DataServiceType = typeof(CustomDataContext);
                OpenWebDataServiceHelper.CreateODataWriterDelegate.Value = (odataWriter) =>
                {
                    createODataWriterDelegateCount++;
                    return new MyODataWriter(odataWriter);
                };

                test.TestUtil.RunCombinations(testInfo, UnitTestsUtil.BooleanValues, (ti, batchMode) =>
                {
                    createODataWriterDelegateCount = 0;
                    int CustomerCount = 0;
                    int NavigationLink = 0;
                    int OrderCount = 0;

                    MyODataWriter.WriteEntryStart.Value = (args) =>
                        {
                            if (args.Entry != null)
                            {
                                if (args.Entry.TypeName.Contains("Customer"))
                                {
                                    Assert.IsTrue(typeof(Customer).IsAssignableFrom(args.Instance.GetType()), "Make sure the instance is customer or customerwithbirthday");
                                    CustomerCount++;
                                }
                                else if (args.Entry.TypeName.Contains("Order"))
                                {
                                    Assert.IsTrue(typeof(Order).IsAssignableFrom(args.Instance.GetType()), "Make sure the instance is order");
                                    OrderCount++;
                                }
                            }

                            return false;
                        };

                    MyODataWriter.WriteLinkStart.Value = (args) =>
                        {
                             NavigationLink++;
                             return false;
                        };

                    if (!batchMode)
                    {
                        request.RequestUriString = ti.Query;
                        request.SendRequest();
                        Assert.AreEqual(1, createODataWriterDelegateCount, "NavigationCount should match");
                    }
                    else
                    {
                        BatchWebRequest batchRequest = new BatchWebRequest();
                        InMemoryWebRequest getRequest = new InMemoryWebRequest();
                        getRequest.RequestUriString = ti.Query;
                        batchRequest.Parts.Add(getRequest);
                        batchRequest.SendRequest(request);
                        Assert.AreEqual(batchRequest.Parts.Count, createODataWriterDelegateCount, "NavigationCount should match");
                    }

                    Assert.AreEqual(ti.CustomerCount, CustomerCount, "CustomerCount should match");
                    Assert.AreEqual(ti.OrderCount, OrderCount, "OrderCount should match");
                    Assert.AreEqual(ti.NavLinkCount, NavigationLink, "NavigationCount should match");
                });
            }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void CreateODataWriterInlinecountTest()
        {
            foreach (string acceptType in new string[] { "application/atom+xml", "application/json;odata.metadata=minimal" })
            {
                // ATOM the value is written to the Feed prior to Start,JSON the value is written after start
                // but before end
                using (OpenWebDataServiceHelper.CreateODataWriterDelegate.Restore())
                using (MyODataWriter.WriteEntryStart.Restore())
                using (MyODataWriter.WriteLinkStart.Restore())
                using (MyODataWriter.WriteFeedStart.Restore())
                using (MyODataWriter.WriteEndDelegate.Restore())
                using (var request = TestWebRequest.CreateForInProcess())
                {
                    request.HttpMethod = "GET";
                    request.DataServiceType = typeof(CustomDataContext);
                    OpenWebDataServiceHelper.CreateODataWriterDelegate.Value = (odataWriter) => new MyODataWriter(odataWriter);

                    var writeArgsStack = new Stack<object>();
                    long? actualInlineCountValueAtWriteEnd = null;
                    MyODataWriter.WriteLinkStart.Value = (args) =>
                                                         {
                                                             writeArgsStack.Push(args);
                                                             return false;
                                                         };

                    MyODataWriter.WriteEntryStart.Value = (args) =>
                                                          {
                                                              writeArgsStack.Push(args);
                                                              return false;
                                                          };

                    MyODataWriter.WriteFeedStart.Value = (args) =>
                                                         {
                                                             writeArgsStack.Push(args);
                                                             if (args.Feed != null)
                                                             {
                                                                 args.Feed.Count = -2;
                                                             }

                                                             return false;
                                                         };

                    MyODataWriter.WriteEndDelegate.Value = () =>
                                                           {
                                                               var currentArgs = writeArgsStack.Pop();

                                                               var feedArgs = currentArgs as DataServiceODataWriterFeedArgs;
                                                               if (feedArgs != null)
                                                               {
                                                                   actualInlineCountValueAtWriteEnd = feedArgs.Feed.Count;

                                                               }

                                                               return false;
                                                           };

                    request.Accept = acceptType;
                    request.RequestUriString = "/Orders?$inlinecount=allpages";
                    request.SendRequest();
                    if (acceptType == "application/atom+xml")
                    {
                        var document = request.GetResponseStreamAsXDocument();
                        var element = document.Elements().ToArray()[0].Elements().Where(e => e.Name.LocalName == "count").Single();
                        Assert.AreEqual(element.Value, "-2");
                    }
                    else if (acceptType == "application/json;odata.metadata=minimal")
                    {
                        var responsePayload = request.GetResponseStreamAsText();
                        Assert.IsTrue(responsePayload.Contains("count\":-2"));
                    }
                    else 
                    {
                        // Behavior is odd
                        var responsePayload = request.GetResponseStreamAsText();
                        Assert.IsTrue(responsePayload.Contains("__count\":-2"));
                    }
                }
            }
        }

        [TestMethod]
        public void CreateODataWriterDataServerExceptionSurfacingTest()
        {
            // ATOM the value is written to the Feed prior to Start,JSON the value is written after start
            // but before end
            using (OpenWebDataServiceHelper.CreateODataWriterDelegate.Restore())
            using (MyODataWriter.WriteEntryStart.Restore())
            using (var request = TestWebRequest.CreateForInProcess())
            {
                request.HttpMethod = "GET";
                request.DataServiceType = typeof(CustomDataContext);
                OpenWebDataServiceHelper.CreateODataWriterDelegate.Value = (odataWriter) => new MyODataWriter(odataWriter);

                MyODataWriter.WriteEntryStart.Value = (args) => { throw new DataServiceException(509, "Should see this message in error"); };

                request.RequestUriString = "/Orders";

                try
                {
                    request.SendRequest();
                }
                catch (WebException)
                {
                    Assert.AreEqual(509, request.ResponseStatusCode);
                    Assert.IsTrue(request.GetResponseStreamAsText().Contains("Should see this message in error"));
                }
            }
        }

        [TestMethod]
        public void CreateODataWriterOtherExceptionTest()
        {
            // ATOM the value is written to the Feed prior to Start,JSON the value is written after start
            // but before end
            using (OpenWebDataServiceHelper.CreateODataWriterDelegate.Restore())
            using (MyODataWriter.WriteFeedStart.Restore())
            using (var request = TestWebRequest.CreateForInProcess())
            {
                request.HttpMethod = "GET";
                request.DataServiceType = typeof(CustomDataContext);
                OpenWebDataServiceHelper.CreateODataWriterDelegate.Value = (odataWriter) => new MyODataWriter(odataWriter);

                MyODataWriter.WriteFeedStart.Value = (args) => { throw new InvalidCastException("Cast exception"); };

                request.RequestUriString = "/Orders";

                try
                {
                    request.SendRequest();
                }
                catch (WebException)
                {
                    Assert.AreEqual(500, request.ResponseStatusCode);
                    Assert.IsTrue(request.GetResponseStreamAsText().Contains("Cast exception"));
                }
            }
        }

        [TestMethod]
        public void CreateODataWriterExceptionTestOnLink()
        {
            // ATOM the value is written to the Feed prior to Start,JSON the value is written after start
            // but before end
            using (OpenWebDataServiceHelper.CreateODataWriterDelegate.Restore())
            using (MyODataWriter.WriteLinkStart.Restore())
            using (var request = TestWebRequest.CreateForInProcess())
            {
                request.HttpMethod = "GET";
                request.DataServiceType = typeof(CustomDataContext);
                OpenWebDataServiceHelper.CreateODataWriterDelegate.Value = (odataWriter) => new MyODataWriter(odataWriter);

                MyODataWriter.WriteLinkStart.Value = (args) => { throw new ODataException("Cast exception"); };

                request.RequestUriString = "/Customers?$expand=Orders";

                try
                {
                    request.SendRequest();
                }
                catch (WebException)
                {
                    Assert.AreEqual(500, request.ResponseStatusCode);
                    Assert.IsTrue(request.GetResponseStreamAsText().Contains("Cast exception"));
                }
            }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void CreateODataWriterDelegateTestForOpenProvider()
        {
            var testInfo = new[] {
                    new { Query = "/Customers?$format=atom", CustomerCount = 3, NavLinkCount = 3, OrderCount = 0 },
                    new { Query = "/Customers?$format=atom&$expand=Orders", CustomerCount = 3, NavLinkCount = 3, OrderCount = 6 },
                    new { Query = "/Customers(1)?$format=atom", CustomerCount = 1, NavLinkCount = 1, OrderCount = 0 },
                    new { Query = "/Customers(1)?$format=atom&$expand=Orders", CustomerCount = 1, NavLinkCount = 1, OrderCount = 2 },
                    new { Query = "/Customers(1)/Orders?$format=atom", CustomerCount = 0, NavLinkCount = 0, OrderCount = 2 },
                };

            using (OpenWebDataServiceHelper.CreateODataWriterDelegate.Restore())
            using (MyODataWriter.WriteEntryStart.Restore())
            using (MyODataWriter.WriteLinkStart.Restore())
            using (var request = TestWebRequest.CreateForInProcess())
            {
                request.HttpMethod = "GET";
                request.DataServiceType = typeof(CustomRowBasedOpenTypesContext);
                OpenWebDataServiceHelper.CreateODataWriterDelegate.Value = (odataWriter) =>
                {
                    return new MyODataWriter(odataWriter);
                };

                test.TestUtil.RunCombinations(testInfo, UnitTestsUtil.BooleanValues, (ti, batchMode) =>
                {
                    int CustomerCount = 0;
                    int NavigationLink = 0;
                    int OrderCount = 0;

                    MyODataWriter.WriteEntryStart.Value = (args) =>
                    {
                        Assert.IsTrue(args.Instance.GetType() == typeof(RowComplexType), "Making sure the right provider type is exposed");
                        var instance = (RowComplexType)args.Instance;
                        if (args.Entry.TypeName.Contains("Customer"))
                        {
                            Assert.IsTrue(instance.TypeName.Contains("Customer"), "Make sure the instance is customer or customerwithbirthday");
                            CustomerCount++;
                        }
                        else if (args.Entry.TypeName.Contains("Order"))
                        {
                            Assert.IsTrue(instance.TypeName.Contains("Order"), "Make sure the instance is order");
                            OrderCount++;
                        }

                        return false;
                    };

                    MyODataWriter.WriteLinkStart.Value = (args) =>
                    {
                        if (args.NavigationLink.Name == "Orders")
                        {
                            Assert.IsTrue(args.NavigationLink.IsCollection.Value, "orders must be collection");
                            NavigationLink++;
                        }

                        return false;
                    };

                    if (!batchMode)
                    {
                        request.RequestUriString = ti.Query;
                        request.SendRequest();
                    }
                    else
                    {
                        BatchWebRequest batchRequest = new BatchWebRequest();
                        InMemoryWebRequest getRequest = new InMemoryWebRequest();
                        getRequest.RequestUriString = ti.Query;
                        batchRequest.Parts.Add(getRequest);
                        batchRequest.SendRequest(request);
                    }

                    Assert.AreEqual(CustomerCount, ti.CustomerCount, "CustomerCount should match");
                    Assert.AreEqual(OrderCount, ti.OrderCount, "OrderCount should match");
                    Assert.AreEqual(NavigationLink, ti.NavLinkCount, "NavigationCount should match");
                });
            }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void ChangingFeedCollectionValueForTopLevel()
        {
            using (OpenWebDataServiceHelper.CreateODataWriterDelegate.Restore())
            using (MyODataWriter.WriteEntryStart.Restore())
            using (MyODataWriter.WriteFeedStart.Restore())
            using (MyODataWriter.WriteLinkStart.Restore())
            using (MyODataWriter.WriteEndDelegate.Restore())
            using (var request = TestWebRequest.CreateForInProcess())
            {
                MyODataWriter testODataWriter = null;
                request.HttpMethod = "GET";
                request.DataServiceType = typeof(CustomDataContext);
                OpenWebDataServiceHelper.CreateODataWriterDelegate.Value = (odataWriter) =>
                {
                    testODataWriter = new MyODataWriter(odataWriter);
                    return testODataWriter;
                };

                int CustomerCount = 0;
                int insideIgnoreCustomer = 0;

                MyODataWriter.WriteEntryStart.Value = (args) =>
                {
                    if (args.Entry.TypeName.Contains("Customer"))
                    {
                        // Only write the first instance of the customer on the wire
                        CustomerCount++;
                        if (CustomerCount != 1)
                        {
                            insideIgnoreCustomer = 1;
                            return true;
                        }
                    }

                    return false;
                };

                MyODataWriter.WriteFeedStart.Value = (args) =>
                    {
                        if (args.Feed.Id.OriginalString.Contains("Customers"))
                        {
                            testODataWriter.CallBaseWriteStart(new ODataResourceSet() { Id = args.Feed.Id, Count = 88 });
                            return true;
                        }

                        return false;
                    };

                MyODataWriter.WriteLinkStart.Value = (args) =>
                    {
                        if (insideIgnoreCustomer > 0)
                        {
                            insideIgnoreCustomer++;
                            return true;
                        }

                        return false;
                    };

                MyODataWriter.WriteEndDelegate.Value = () =>
                {
                    if (insideIgnoreCustomer > 0)
                    {
                        insideIgnoreCustomer--;
                        return true;
                    }

                    return false;
                };

                request.RequestUriString = "/Customers?$format=atom&$inlineCount=allpages";
                request.SendRequest();
                var response = request.GetResponseStreamAsXDocument();

                // verify that the count is 100
                // verify that there is one customer written on the wire
                UnitTestsUtil.VerifyXPaths(response, new string[] { "/atom:feed[adsm:count=88]" });
                UnitTestsUtil.VerifyXPathResultCount(response, 1, "/atom:feed/atom:entry");
            }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void DataServiceOdataWriterWriteEndForEntryTest()
        {
            using (OpenWebDataServiceHelper.CreateODataWriterDelegate.Restore())
            using (MyODataWriter.WriteEntryStart.Restore())
            using (MyODataWriter.WriteLinkStart.Restore())
            using (MyODataWriter.WriteEntryEnd.Restore())
            using (MyODataWriter.WriteLinkEnd.Restore())
            using (var request = TestWebRequest.CreateForInProcess())
            {
                MyODataWriter testODataWriter = null;
                request.HttpMethod = "GET";
                request.DataServiceType = typeof(CustomDataContext);
                Stack<ODataItem> items = new Stack<ODataItem>();
                int customersCount = 0;
                int customersStartedAndNotEnded = 0;
                int linksCount = 0;
                int linksStartedAndNotEnded = 0;
                OpenWebDataServiceHelper.CreateODataWriterDelegate.Value = (odataWriter) =>
                {
                    testODataWriter = new MyODataWriter(odataWriter);
                    return testODataWriter;
                };

                MyODataWriter.WriteEntryStart.Value = (args) =>
                {
                    items.Push(args.Entry);
                    customersCount++;
                    customersStartedAndNotEnded++;
                    return false;
                };

                MyODataWriter.WriteLinkStart.Value = (args) =>
                {
                    items.Push(args.NavigationLink);
                    linksCount++;
                    linksStartedAndNotEnded++;
                    return false;
                };

                MyODataWriter.WriteEntryEnd.Value = (args) =>
                {
                    customersStartedAndNotEnded--;
                    Assert.AreSame(args.Entry, items.Pop());
                    return false;
                };

                MyODataWriter.WriteLinkEnd.Value = (args) =>
                {
                    linksStartedAndNotEnded--;
                    Assert.AreSame(args.NavigationLink, items.Pop());
                    return false;
                };

                request.RequestUriString = "/Customers(1)?$format=atom";
                request.SendRequest();
                Assert.AreEqual(customersStartedAndNotEnded, 0);
                Assert.AreEqual(linksStartedAndNotEnded, 0);
                var response = request.GetResponseStreamAsXDocument();
                UnitTestsUtil.VerifyXPathResultCount(response, customersCount, "/atom:entry");
                UnitTestsUtil.VerifyXPathResultCount(response, linksCount, "/atom:entry/atom:link[@title='BestFriend' or @title='Orders']");
            }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void DataServiceOdataWriterWriteEndForLinksTest()
        {
            using (OpenWebDataServiceHelper.CreateODataWriterDelegate.Restore())
            using (MyODataWriter.WriteLinkStart.Restore())
            using (MyODataWriter.WriteLinkEnd.Restore())
            using (var request = TestWebRequest.CreateForInProcess())
            {
                MyODataWriter testODataWriter = null;
                request.HttpMethod = "GET";
                request.DataServiceType = typeof(CustomDataContext);
                Stack<ODataItem> items = new Stack<ODataItem>();
                int linksCount = 0;
                int linksStartedAndNotEnded = 0;
                OpenWebDataServiceHelper.CreateODataWriterDelegate.Value = (odataWriter) =>
                {
                    testODataWriter = new MyODataWriter(odataWriter);
                    return testODataWriter;
                };

                MyODataWriter.WriteLinkStart.Value = (args) =>
                {
                    items.Push(args.NavigationLink);
                    linksCount++;
                    linksStartedAndNotEnded++;
                    return false;
                };

                MyODataWriter.WriteLinkEnd.Value = (args) =>
                {
                    linksStartedAndNotEnded--;
                    Assert.AreSame(args.NavigationLink, items.Pop());
                    return false;
                };

                request.RequestUriString = "/Customers(1)/Orders/$ref?$format=atom";
                request.SendRequest();
                Assert.AreEqual(linksStartedAndNotEnded, 0);
                var response = request.GetResponseStreamAsXDocument();
                UnitTestsUtil.VerifyXPathResultCount(response, linksCount, "/atom:links/atom:uri");
            }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void DataServiceOdataWriterWriteEndForFeedTest()
        {
            using (OpenWebDataServiceHelper.CreateODataWriterDelegate.Restore())
            using (MyODataWriter.WriteFeedStart.Restore())
            using (MyODataWriter.WriteEntryStart.Restore())
            using (MyODataWriter.WriteLinkStart.Restore())
            using (MyODataWriter.WriteFeedEnd.Restore())
            using (MyODataWriter.WriteEntryEnd.Restore())
            using (MyODataWriter.WriteLinkEnd.Restore())
            using (var request = TestWebRequest.CreateForInProcess())
            {
                MyODataWriter testODataWriter = null;
                request.HttpMethod = "GET";
                request.DataServiceType = typeof(CustomDataContext);
                Stack<ODataItem> items = new Stack<ODataItem>();
                int customersCount = 0;
                int customersStartedAndNotEnded = 0;
                int linksCount = 0;
                int linksStartedAndNotEnded = 0;
                bool feedStarted = false, feedEnded = false;
                OpenWebDataServiceHelper.CreateODataWriterDelegate.Value = (odataWriter) =>
                {
                    testODataWriter = new MyODataWriter(odataWriter);
                    return testODataWriter;
                };

                MyODataWriter.WriteFeedStart.Value = (args) =>
                {
                    items.Push(args.Feed);
                    feedStarted = true;
                    return false;
                };

                MyODataWriter.WriteEntryStart.Value = (args) =>
                {
                    items.Push(args.Entry);
                    customersCount++;
                    customersStartedAndNotEnded++;
                    return false;
                };

                MyODataWriter.WriteLinkStart.Value = (args) =>
                {
                    items.Push(args.NavigationLink);
                    linksCount++;
                    linksStartedAndNotEnded++;
                    return false;
                };

                MyODataWriter.WriteFeedEnd.Value = (args) =>
                {
                    feedEnded = true;
                    Assert.AreSame(args.Feed, items.Pop());
                    return false;
                };

                MyODataWriter.WriteEntryEnd.Value = (args) =>
                {
                    customersStartedAndNotEnded--;
                    Assert.AreSame(args.Entry, items.Pop());
                    return false;
                };

                MyODataWriter.WriteLinkEnd.Value = (args) =>
                {
                    linksStartedAndNotEnded--;
                    Assert.AreSame(args.NavigationLink, items.Pop());
                    return false;
                };

                request.RequestUriString = "/Customers?$format=atom";
                request.SendRequest();
                Assert.IsTrue(feedStarted);
                Assert.IsTrue(feedEnded);
                Assert.AreEqual(customersStartedAndNotEnded, 0);
                Assert.AreEqual(linksStartedAndNotEnded, 0);
                var response = request.GetResponseStreamAsXDocument();
                UnitTestsUtil.VerifyXPathResultCount(response, customersCount, "/atom:feed/atom:entry");
                UnitTestsUtil.VerifyXPathResultCount(response, linksCount, "/atom:feed/atom:entry/atom:link[@title='BestFriend' or @title='Orders']");
            }
        }

        private class MyODataWriter : DataServiceODataWriter
        {
            private static test.Restorable<Func<DataServiceODataWriterFeedArgs, bool>> writeFeedStart = new test.Restorable<Func<DataServiceODataWriterFeedArgs, bool>>();
            public static test.Restorable<Func<DataServiceODataWriterFeedArgs, bool>> WriteFeedStart { get { return writeFeedStart; } }

            private static test.Restorable<Func<DataServiceODataWriterEntryArgs, bool>> writeEntryStart = new test.Restorable<Func<DataServiceODataWriterEntryArgs, bool>>();
            public static test.Restorable<Func<DataServiceODataWriterEntryArgs, bool>> WriteEntryStart { get { return writeEntryStart; } }

            private static test.Restorable<Func<DataServiceODataWriterNestedResourceInfoArgs, bool>> writeLinkStart = new test.Restorable<Func<DataServiceODataWriterNestedResourceInfoArgs, bool>>();
            public static test.Restorable<Func<DataServiceODataWriterNestedResourceInfoArgs, bool>> WriteLinkStart { get { return writeLinkStart; } }

            private static test.Restorable<Func<DataServiceODataWriterFeedArgs, bool>> writeFeedEnd = new test.Restorable<Func<DataServiceODataWriterFeedArgs, bool>>();
            public static test.Restorable<Func<DataServiceODataWriterFeedArgs, bool>> WriteFeedEnd { get { return writeFeedEnd; } }

            private static test.Restorable<Func<DataServiceODataWriterEntryArgs, bool>> writeEntryEnd = new test.Restorable<Func<DataServiceODataWriterEntryArgs, bool>>();
            public static test.Restorable<Func<DataServiceODataWriterEntryArgs, bool>> WriteEntryEnd { get { return writeEntryEnd; } }

            private static test.Restorable<Func<DataServiceODataWriterNestedResourceInfoArgs, bool>> writeLinkEnd = new test.Restorable<Func<DataServiceODataWriterNestedResourceInfoArgs, bool>>();
            public static test.Restorable<Func<DataServiceODataWriterNestedResourceInfoArgs, bool>> WriteLinkEnd { get { return writeLinkEnd; } }

            private static test.Restorable<Func<bool>> writeEnd = new test.Restorable<Func<bool>>();
            public static test.Restorable<Func<bool>> WriteEndDelegate { get { return writeEnd; } }

            private readonly ODataWriter odataWriter;

            public MyODataWriter(ODataWriter odataWriter) : base(odataWriter)
            {
                this.odataWriter = odataWriter;
            }

            public override void WriteStart(DataServiceODataWriterEntryArgs args)
            {
                if (WriteEntryStart.Value != null)
                {
                    if (WriteEntryStart.Value(args)) return;
                }

                base.WriteStart(args);
            }

            public override void WriteStart(DataServiceODataWriterFeedArgs args)
            {
                if (WriteFeedStart.Value != null)
                {
                    if (WriteFeedStart.Value(args)) return;
                }

                base.WriteStart(args);
            }

            public override void WriteStart(DataServiceODataWriterNestedResourceInfoArgs args)
            {
                if (WriteLinkStart.Value != null)
                {
                    if (WriteLinkStart.Value(args)) return;
                }

                base.WriteStart(args);
            }

            public override void WriteEnd()
            {
                if (WriteEndDelegate.Value != null)
                {
                    if (WriteEndDelegate.Value()) return;
                }

                base.WriteEnd();
            }

            public override void WriteEnd(DataServiceODataWriterFeedArgs args)
            {
                if (WriteFeedEnd.Value != null)
                {
                    if (WriteFeedEnd.Value(args)) return;
                }

                base.WriteEnd(args);
            }

            public override void WriteEnd(DataServiceODataWriterEntryArgs args)
            {
                if (WriteEntryEnd.Value != null)
                {
                    if (WriteEntryEnd.Value(args)) return;
                }

                base.WriteEnd(args);
            }

            public override void WriteEnd(DataServiceODataWriterNestedResourceInfoArgs args)
            {
                if (WriteLinkEnd.Value != null)
                {
                    if (WriteLinkEnd.Value(args)) return;
                }

                base.WriteEnd(args);
            }

            public void CallBaseWriteStart(ODataResourceSet feed)
            {
                this.odataWriter.WriteStart(feed);
            }

            public void CallBaseWriteStart(ODataResource entry)
            {
                this.odataWriter.WriteStart(entry);
            }

            public void CallBaseWriteStart(ODataNestedResourceInfo navigationLink)
            {
                this.odataWriter.WriteStart(navigationLink);
            }
        }
    }
}
