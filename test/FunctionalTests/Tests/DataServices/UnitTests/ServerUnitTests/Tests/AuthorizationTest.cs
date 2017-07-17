//---------------------------------------------------------------------
// <copyright file="AuthorizationTest.cs" company="Microsoft">
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
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.ServiceModel.Web;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using Microsoft.OData.Edm;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Client;
    #endregion Namespaces

    /// <summary>This is a test class for authorization mechanisms in the server data service.</summary>
    [TestModule]
    public partial class UnitTestModule : AstoriaTestModule
    {
        [TestClass()]
        public class AuthorizationTest
        {
            private string RunInitializers(Type serviceType)
            {
                // Ensure that static initializers are called in the right order.
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    string result = "";
                    request.ServiceType = serviceType;
                    request.RequestUriString = "/";
                    using (InitializationCallbackManager.RegisterStatic(delegate(object o, InitializationCallbackEventArgs e)
                    {
                        result += o.ToString();
                    }))
                    {
                        request.SendRequest();
                    }

                    return result;
                }
            }

            [TestMethod, Variation]
            public void StaticInitializersNoInitTest()
            {
                Type[] types = new Type[]
                {
                    typeof(WebDataServiceNoInit0),
                    typeof(WebDataServiceNoInit1),
                    typeof(WebDataServiceNoInit2),
                    typeof(WebDataServiceNoInit3),
                    typeof(WebDataServiceNoInit4),
                };

                foreach (Type type in types)
                {
                    string result = RunInitializers(typeof(WebDataServiceA));
                    AstoriaTestLog.AreEqual("", result);
                }
            }

            [TestMethod, Variation]
            public void StaticInitializersTest()
            {
                string result;
                result = RunInitializers(typeof(WebDataServiceA));
                AstoriaTestLog.AreEqual(typeof(WebDataServiceA).ToString(), result);

                result = RunInitializers(typeof(WebDataServiceB));
                AstoriaTestLog.AreEqual(typeof(WebDataServiceB).ToString(), result);
            }

            private static string AtomCustomerPayload(int id, string name)
            {
                return AtomCustomerPayload(id, name, -1);
            }

            private static string AtomCustomerPayload(int id, string name, int orderId)
            {
                System.Xml.Linq.XNamespace metadataNs = "http://docs.oasis-open.org/odata/ns/metadata";
                System.Xml.Linq.XNamespace dataNs = "http://docs.oasis-open.org/odata/ns/data";
                System.Xml.Linq.XNamespace atomNs = "http://www.w3.org/2005/Atom";
                
                var payload = new System.Xml.Linq.XElement(
                    "wrapper",
                    new System.Xml.Linq.XAttribute("type", "application/xml"),
                    new System.Xml.Linq.XElement(
                        metadataNs + "properties",
                        new System.Xml.Linq.XElement(dataNs + "ID", id.ToString()),
                        new System.Xml.Linq.XElement(dataNs + "Name", name)));
                
                var item = new System.ServiceModel.Syndication.SyndicationItem();
                item.Categories.Add(new System.ServiceModel.Syndication.SyndicationCategory(
                    typeof(Customer).FullName, "http://docs.oasis-open.org/odata/ns/scheme", ""));
                if (orderId > -1)
                {
                    var orderElement = new System.Xml.Linq.XElement(
                        metadataNs + "inline",
                        new System.Xml.Linq.XElement(atomNs + "feed",
                            new System.Xml.Linq.XElement(atomNs + "entry",
                                new System.Xml.Linq.XElement(atomNs + "content",
                                    new System.Xml.Linq.XAttribute("type", "application/xml"),
                                    new System.Xml.Linq.XElement(metadataNs + "properties", 
                                        new System.Xml.Linq.XElement(dataNs + "ID", orderId.ToString())
                                        )
                                    )
                                )
                            )
                        );

                    var link = new System.ServiceModel.Syndication.SyndicationLink();
                    link.Title = "Orders";
                    link.RelationshipType = "http://docs.oasis-open.org/odata/ns/related/Orders";
                    link.ElementExtensions.Add(orderElement.CreateReader());
                    item.Links.Add(link);
                }

                var content = System.ServiceModel.Syndication.SyndicationContent.CreateXmlContent(payload.CreateReader());
                item.Content = content;
                
                var settings = new System.Xml.XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                
                using (StringWriter writer = new StringWriter())
                using (System.Xml.XmlWriter itemWriter = System.Xml.XmlWriter.Create(writer, settings))
                {
                    item.GetAtom10Formatter().WriteTo(itemWriter);
                    itemWriter.Flush();
                    return writer.ToString();
                }
            }

            // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void AuthorizationForMethodsTest()
            {
                List<AuthCase> cases = new List<AuthCase>()
                {
#region DELETE cases
                    ///////////////////////////////////////////////////////////
                    // DELETE CASES
                    new AuthCase("D1 - Delete Single Resource From Entity Set")
                    {
                        Rights = "Customers:RS, WD", Method = "DELETE", RequestUri = "/Customers(1)", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(!context.Customers.Exists((c)=>c.ID==1), "Resource deleted.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "DELETE callback called for Customer");
                            }
                    },
                    new AuthCase("D1.1 - Delete Single Resource From Entity Set - fail (no WD right)")
                    {
                        Rights = "Customers:WA, WR, WM, RS, RM", Method = "DELETE", RequestUri = "/Customers(1)", AccessDeniedExpected = true
                    },
                    new AuthCase("D1.2 - Delete Single Resource From Entity Set - fail (no RS right)")
                    {
                        Rights = "Customers:WA, WR, WM, WD, RM", Method = "DELETE", RequestUri = "/Customers(1)", AccessDeniedExpected = true
                    },
                    new AuthCase("D2 - Delete Single Resource From NavCollection")
                    {
                        Rights = "Customers:RS; Orders:RS, WD", Method = "DELETE", RequestUri = "/Customers(1)/Orders(1)", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(!context.Orders.Exists((o)=>o.ID==1), "Order does not exist.");
                                AstoriaTestLog.IsTrue(!context.Customers.Exists((c)=>c.Orders.Exists((o)=>o.ID==1)), "Order doesn't exist in the nav collection.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "One update fired");
                                AstoriaTestLog.IsTrue(updates.Exists((u)=>u.Action == UpdateOperations.Delete && u.Target is Order), "Order was deleted.");
                            }
                    },
                    new AuthCase("D2.1 - Delete Single Resource From NavCollection - fail (no delete)")
                    {
                        Rights = "Customers:All;Orders:RS, RM, WA, WR, WM", Method = "DELETE", RequestUri = "/Customers(1)/Orders(1)", AccessDeniedExpected = true,
                        JsonPayload = "null",
                    },
                    new AuthCase("D2.2 - Delete Single Resource From NavCollection - fail (no read single on customers)")
                    {
                        Rights = "Customers:WM, WD, WR, RM, WA;Orders:All", Method = "DELETE", RequestUri = "/Customers(1)/Orders(1)", AccessDeniedExpected = true,
                        JsonPayload = "null",
                    },
                    new AuthCase("D2.3 - Delete Single Resource From NavCollection - fail (no read single on orders)")
                    {
                        Rights = "Customers:All;Orders:WD", Method = "DELETE", RequestUri = "/Customers(1)/Orders(1)", AccessDeniedExpected = true,
                        JsonPayload = "null",
                    },
                    new AuthCase("D2.4 - Delete Single Resource From NavReference")
                    {
                        Rights = "Customers:RS,WD; Orders:RS", Method = "DELETE", RequestUri = "/Orders(1)/Customer", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(!context.Customers.Exists((c)=>c.ID==1), "Customer 1 does not exist.");
                                AstoriaTestLog.IsTrue(!context.Orders.Exists((o)=>o.ID == 1 && o.Customer != null), "Customer doesn't exist in the reference nav property.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "One update fired");
                                AstoriaTestLog.IsTrue(updates.Exists((u)=>u.Action == UpdateOperations.Delete && u.Target is Customer), "Customer was deleted.");
                            }
                    },
                    new AuthCase("D2.5 - Delete Resource From NavReference - fail (no delete on customers)")
                    {
                        Rights = "Customers:RS;Orders:All", Method = "DELETE", RequestUri = "/Orders(1)/Customer", AccessDeniedExpected = true,
                        JsonPayload = "null",
                    },
                    new AuthCase("D2.6 - Delete Resource From NavReference- fail (no read single on customers)")
                    {
                        Rights = "Customers:WM, WD, WR, RM, WA;Orders:All", Method = "DELETE", RequestUri = "/Orders(1)/Customer", AccessDeniedExpected = true,
                        JsonPayload = "null",
                    },
                    new AuthCase("D2.7 - Delete Single Resource From NavCollection - fail (no read single on orders)")
                    {
                        Rights = "Customers:All;Orders:RM,WA,WM,WR,WD", Method = "DELETE", RequestUri = "/Orders(1)/Customer", AccessDeniedExpected = true,
                        JsonPayload = "null",
                    },
                    new AuthCase("D3 - Delete Single Resource From NavReference")
                    {
                        Rights = "Customers:RS,WD", Method = "DELETE", RequestUri = "/Customers(1)/BestFriend", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(!context.Customers.Exists((o)=>o.ID==0), "Customer attached to customers(1) was deleted");
                                AstoriaTestLog.IsTrue(context.Customers.Where((c)=>c.ID == 1).First().BestFriend == null, "The relationship was unbinded - set to null.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "One update fired");
                                AstoriaTestLog.IsTrue(updates.Exists((u)=>u.Action == UpdateOperations.Delete && ((Customer)u.Target).ID == 0), "Customer 0 was deleted.");
                            }
                    },
                    new AuthCase("D3.1 - Delete Single Resource From NavReference - fail (no delete)")
                    {
                        Rights = "Customers:RS,WM,WR,RM,WA", Method = "DELETE", RequestUri = "/Customers(1)/BestFriend", AccessDeniedExpected = true,
                        JsonPayload = "null",
                    },
                    new AuthCase("D3.2 - Delete Single Resource From NavReference - fail (no read single)")
                    {
                        Rights = "Customers:RM,WM,WR,WA,WD", Method = "DELETE", RequestUri = "/Customers(1)/BestFriend", AccessDeniedExpected = true,
                        JsonPayload = "null",
                    },
                    new AuthCase("D4 - Delete on primitive - both merge or replace rights should work")
                    {
                        Rights = "Customers:RS,WM", Method = "DELETE", RequestUri = "/Customers(1)/Name/$value", Check = (context, updates) =>
                            {
                                Assert.IsNull(context.Customers.First((c)=>c.ID==1).Name, "Customer's name was nulled out.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "One update fired");
                                AstoriaTestLog.IsTrue(updates.Exists((u)=>u.Action == UpdateOperations.Change), "Customer was changed.");
                            }
                    },
                    new AuthCase("D4.1 - Delete on primitive - both merge or replace rights should work")
                    {
                        Rights = "Customers:RS,WR", Method = "DELETE", RequestUri = "/Customers(1)/Name/$value", Check = (context, updates) =>
                            {
                                Assert.IsNull(context.Customers.First((c)=>c.ID==1).Name, "Customer's name was nulled out.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "One update fired");
                                AstoriaTestLog.IsTrue(updates.Exists((u)=>u.Action == UpdateOperations.Change), "Customer was changed.");
                            }
                    },
                    new AuthCase("D4.2 - Delete on primitive - fail no update(replace or merge) rights")
                    {
                        Rights = "Customers:RS,RM,WA,WD", Method = "DELETE", RequestUri = "/Customers(1)/Name/$value",
                        AccessDeniedExpected=true
                    },
                    new AuthCase("D5 - Delete on primitive through complex - both merge or replace rights should work")
                    {
                        Rights = "Customers:RS,WM", Method = "DELETE", RequestUri = "/Customers(1)/Address/StreetAddress/$value", Check = (context, updates) =>
                            {
                                Assert.IsNull(context.Customers.First((c)=>c.ID==1).Address.StreetAddress, "Customer's Address.StreetAddress was nulled out.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "One update fired");
                                AstoriaTestLog.IsTrue(updates.Exists((u)=>u.Action == UpdateOperations.Change), "Customer was changed.");
                            }
                    },
                    new AuthCase("D5.1 - Delete on primitive through complex - both merge or replace rights should work")
                    {
                        Rights = "Customers:RS,WR", Method = "DELETE", RequestUri = "/Customers(1)/Address/StreetAddress/$value", Check = (context, updates) =>
                            {
                                Assert.IsNull(context.Customers.First((c)=>c.ID==1).Address.StreetAddress, "Customer's Address.StreetAddress was nulled out.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "One update fired");
                                AstoriaTestLog.IsTrue(updates.Exists((u)=>u.Action == UpdateOperations.Change), "Customer was changed.");
                            }
                    },
                    new AuthCase("D5.2 - Delete on primitive through complex - fail no update (replace or merge) rights")
                    {
                        Rights = "Customers:RS,RM,WA,WD", Method = "DELETE", RequestUri = "/Customers(1)/Address/StreetAddress/$value",
                        AccessDeniedExpected = true
                    },
                    new AuthCase("D6 - Unbind reference property - WriteReplace should work")
                    {
                        Rights = "Customers:RS,WR", Method = "DELETE", RequestUri = "/Customers(1)/BestFriend/$ref", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Where((c)=>c.ID == 1).First().BestFriend == null, "The relationship was unbinded - set to null.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "One update fired");
                                AstoriaTestLog.IsTrue(updates.Exists((u)=>u.Action == UpdateOperations.Change && ((Customer)u.Target).ID == 1), "Customer 1 was updated.");
                            }
                    },
                    new AuthCase("D6.1 - Unbind reference property - WriteMerge should work")
                    {
                        Rights = "Customers:RS,WM", Method = "DELETE", RequestUri = "/Customers(1)/BestFriend/$ref", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Where((c)=>c.ID == 1).First().BestFriend == null, "The relationship was unbinded - set to null.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "One update fired");
                                AstoriaTestLog.IsTrue(updates.Exists((u)=>u.Action == UpdateOperations.Change && ((Customer)u.Target).ID == 1), "Customer 1 was updated.");
                            }
                    },
                    new AuthCase("D6.1.1 - Unbind reference property - WriteReplace should work")
                    {
                        Rights = "Customers:RS;Orders:RS,WR", Method = "DELETE", RequestUri = "/Orders(1)/Customer/$ref", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Orders.Where((o)=>o.ID == 1).First().Customer == null, "The relationship was unbinded - set to null.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "One update fired");
                                AstoriaTestLog.IsTrue(updates.Exists((u)=>u.Action == UpdateOperations.Change && ((Order)u.Target).ID == 1), "Customer 1 was updated.");
                            }
                    },
                    new AuthCase("D6.1.2 - Unbind reference property - WriteMerge should work")
                    {
                        Rights = "Customers:RS;Orders:RS,WM", Method = "DELETE", RequestUri = "/Orders(1)/Customer/$ref", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Orders.Where((o)=>o.ID == 1).First().Customer == null, "The relationship was unbinded - set to null.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "One update fired");
                                AstoriaTestLog.IsTrue(updates.Exists((u)=>u.Action == UpdateOperations.Change && ((Order)u.Target).ID == 1), "Customer 1 was updated.");
                            }
                    },
                    new AuthCase("D6.2 - Unbind reference set property - WriteReplace")
                    {
                        Rights = "Customers:RS,WR; Orders:RS", Method = "DELETE", RequestUri = "/Customers(1)/Orders/$ref?$id=Orders(1)", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Where((c)=>c.ID == 1).First().Orders.Where((o) => o.ID == 1).FirstOrDefault() == null, "The relationship was unbinded - set to null.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "One update fired");
                                AstoriaTestLog.IsTrue(updates.Exists((u)=>u.Action == UpdateOperations.Change && ((Customer)u.Target).ID == 1), "Customer 1 was updated.");
                            }
                    },
                    new AuthCase("D6.3 - Unbind reference set property - WriteMerge")
                    {
                        Rights = "Customers:RS,WM; Orders:RS", Method = "DELETE", RequestUri = "/Customers(1)/Orders/$ref?$id=Orders(1)", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Where((c)=>c.ID == 1).First().Orders.Where((o) => o.ID == 1).FirstOrDefault() == null, "The relationship was unbinded - set to null.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "One update fired");
                                AstoriaTestLog.IsTrue(updates.Exists((u)=>u.Action == UpdateOperations.Change && ((Customer)u.Target).ID == 1), "Customer 1 was updated.");
                            }
                    },
                    new AuthCase("D6.4 - Unbind reference property - fail (no write merge or write replace rights on customers)")
                    {
                        Rights = "Customers:RS,RM,WD,WA", Method = "DELETE", RequestUri = "/Customers(1)/BestFriend/$ref", AccessDeniedExpected = true,
                    },
                    new AuthCase("D6.5 - Unbind reference property - fail (no read single right on customers)")
                    {
                        Rights = "Customers:RM,WA,WD,WM,WR", Method = "DELETE", RequestUri = "/Customers(1)/BestFriend/$ref", AccessDeniedExpected = true,
                    },
                    new AuthCase("D6.6 - Unbind reference set property - fail (no write merge or write replace rights on customers)")
                    {
                        Rights = "Customers:RS,RM,WA,WD;Orders:All", Method = "DELETE", RequestUri = "/Customers(1)/Orders/$ref?$id=Orders(1)", AccessDeniedExpected = true,
                    },
                    new AuthCase("D6.7 - Unbind reference set property - fail (no read single right on customers)")
                    {
                        Rights = "Customers:RM,WA,WD,WM,WR;Orders:All", Method = "DELETE", RequestUri = "/Customers(1)/Orders/$ref?$id=Orders(1)", AccessDeniedExpected = true,
                    },
                    new AuthCase("D6.8 - Unbind reference set property - fail (no read single on Orders)")
                    {
                        Rights = "Customers:All;Orders:RM,WA,WR,WM,WD", Method = "DELETE", RequestUri = "/Customers(1)/Orders/$ref?$id=Orders(1)", AccessDeniedExpected = true,
                    },
                    new AuthCase("D6.9 - Unbind reference set property - fail (no write merge or write replace rights on orders)")
                    {
                        Rights = "Orders:RS,RM,WA,WD", Method = "DELETE", RequestUri = "/Orders(1)/Customer/$ref", AccessDeniedExpected = true,
                    },
                    new AuthCase("D6.10 - Unbind reference set property - fail (no read single on Orders)")
                    {
                        Rights = "Orders:RM,WA,WR,WM,WD", Method = "DELETE", RequestUri = "/Orders(1)/Customer/$ref", AccessDeniedExpected = true,
                    },

#endregion // DELETE cases

#region POST cases
                    ///////////////////////////////////////////////////////////
                    // POST CASES
                    new AuthCase("A1 - Append to resource set")
                    {
                        Rights = "Customers:WA", Method = "POST", RequestUri = "/Customers",
                        JsonPayload = "{ \"@odata.type\": \"" + typeof(Customer).FullName + "\", ID : 1000, Name : \"abc\" }", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Exists((c)=>c.ID == 1000), "Customer was inserted.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "Update was fired.");
                                AstoriaTestLog.IsTrue(updates[0].Action == UpdateOperations.Add);
                            }
                    },
                    new AuthCase("A1.A - Append to resource set (ATOM)")
                    {
                        Rights = "Customers:WA", Method = "POST", RequestUri = "/Customers",
                        AtomPayload = AtomCustomerPayload(1000, "abc"), Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Exists((c)=>c.ID == 1000), "Customer was inserted.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "Update was fired.");
                                AstoriaTestLog.IsTrue(updates[0].Action == UpdateOperations.Add);
                            }
                    },
                    new AuthCase("A1.1 - Append to resource set - fail")
                    {
                        Rights = "Customers:RS,RM,WR,WM,WD", Method = "POST", RequestUri = "/Customers",
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\", ID : 1000, Name : \"abc\" }", AccessDeniedExpected = true
                    },
                    new AuthCase("A2 - Append to resource set, deep collection")
                    {
                        Rights = "Customers:WA;Orders:WA", Method = "POST", RequestUri = "/Customers",
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\", ID : 1000, Name : \"abc\", Orders : [ { ID : 1000 } ] }", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Exists((c)=>c.ID == 1000), "Customer was inserted.");
                                AstoriaTestLog.IsTrue(context.Customers.Where((c)=>c.ID == 1000).First().Orders.Count == 1, "Customer has new order.");
                                TestUtil.AssertEnumIn(updates.Where((u)=>u.Target is Customer).First().Action, UpdateOperations.Add);
                            }
                    },
                    new AuthCase("A2.A - Append to resource set, deep collection (ATOM)")
                    {
                        Rights = "Customers:WA;Orders:WA", Method = "POST", RequestUri = "/Customers",
                        AtomPayload = AtomCustomerPayload(1000, "abc", 1000), Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Exists((c)=>c.ID == 1000), "Customer was inserted.");
                                AstoriaTestLog.IsTrue(context.Customers.Where((c)=>c.ID == 1000).First().Orders.Count == 1, "Customer has new order.");
                                TestUtil.AssertEnumIn(updates.Where((u)=>u.Target is Customer).First().Action, UpdateOperations.Add);
                            }
                    },
                    new AuthCase("A2.1 - Append to resource set, deep collection - fail (no append rights on Orders")
                    {
                        Rights = "Customers:All;Orders:RM, RS, WD, WR, WM", Method = "POST", RequestUri = "/Customers",
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\", ID : 1000, Name : \"abc\", Orders : [ { ID : 1000 } ] }", AccessDeniedExpected = true
                    },
                    new AuthCase("A2.2 - Append to resource set, deep collection - fail (no append rights on Customers")
                    {
                        Rights = "Customers:RS,RM,WR,WM,WD;Orders:All", Method = "POST", RequestUri = "/Customers",
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\", ID : 1000, Name : \"abc\", Orders : [ { ID : 1000 } ] }", AccessDeniedExpected = true
                    },
                    new AuthCase("A3 - Append to resource set, deep, bind to collection")
                    {
                        Rights = "Customers:WA;Orders:RS", Method = "POST", RequestUri = "/Customers",
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\", ID : 1000, Name : \"abc\", Orders@odata.bind : [ \"/Orders(1)\"] }", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Exists((c)=>c.ID == 1000), "Customer was inserted.");
                                AstoriaTestLog.IsTrue(context.Customers.Where((c)=>c.ID == 1000).First().Orders.Count == 1, "Customer has new order.");
                                AstoriaTestLog.IsTrue(context.Customers.Where((c)=>c.ID == 1000).First().Orders.First().ID == 1, "Customer has existing order.");
                            }
                    },
                    new AuthCase("A3.1 - Append to resource set, deep, bind to collection - fail (can't deref order)")
                    {
                        Rights = "Customers:All;Orders:WA,WR,WM,WD,RM", Method = "POST", RequestUri = "/Customers",
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\", ID : 1000, Name : \"abc\", Orders@odata.bind : [ \"/Orders(1)\" ] }", AccessDeniedExpected = true
                    },
                    new AuthCase("A4 - Append to collection - should pass with both merge or replace rights")
                    {
                        Rights = "Customers:RS,WM;Orders:WA", Method = "POST", RequestUri = "/Customers(2)/BestFriend/Orders",
                        JsonPayload = "{ ID : 1000 }", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Orders.Exists((o)=>o.ID == 1000), "Order was inserted.");
                                AstoriaTestLog.IsTrue(updates.Count == 2, "Two updates were fired.");
                                AstoriaTestLog.IsTrue(updates.Exists((c)=>c.Action == UpdateOperations.Change && c.Target is Customer), "Customer was changed.");
                                AstoriaTestLog.IsTrue(updates.Exists((c)=>c.Action == UpdateOperations.Add && c.Target is Order), "Order was added and referenced.");
                            }
                    },
                    new AuthCase("A4.1 - Append to collection - should pass with both merge or replace rights")
                    {
                        Rights = "Customers:RS,WR;Orders:WA", Method = "POST", RequestUri = "/Customers(2)/BestFriend/Orders",
                        JsonPayload = "{ ID : 1000 }", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Orders.Exists((o)=>o.ID == 1000), "Order was inserted.");
                                AstoriaTestLog.IsTrue(updates.Count == 2, "Two updates were fired.");
                                AstoriaTestLog.IsTrue(updates.Exists((c)=>c.Action == UpdateOperations.Change && c.Target is Customer), "Customer was changed.");
                                AstoriaTestLog.IsTrue(updates.Exists((c)=>c.Action == UpdateOperations.Add && c.Target is Order), "Order was added and referenced.");
                            }
                    },
                    new AuthCase("A4.2 - Append to collection - fail (no append right on Orders")
                    {
                        Rights = "Customers:All;Orders:RS, RM, WR, WM, WD", Method = "POST", RequestUri = "/Customers(2)/BestFriend/Orders",
                        JsonPayload = "{ ID : 1000 }", AccessDeniedExpected = true
                    },
                    new AuthCase("A4.3 - Append to collection - fail (no update rights on Customers")
                    {
                        Rights = "Customers:RS,RM,WA,WD;Orders:All", Method = "POST", RequestUri = "/Customers(2)/BestFriend/Orders",
                        JsonPayload = "{ ID : 1000 }", AccessDeniedExpected = true
                    },
                    new AuthCase("A4.4 - Append to collection - fail (no read rights on Customers")
                    {
                        Rights = "Customers:RM,WA,WR,WM,WD;Orders:All", Method = "POST", RequestUri = "/Customers(2)/BestFriend/Orders",
                        JsonPayload = "{ ID : 1000 }", AccessDeniedExpected = true
                    },
                    new AuthCase("A5 - Append to resource set, deep, bind new resource to nav reference")
                    {
                        Rights = "Customers:WA", Method = "POST", RequestUri = "/Customers",
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\", ID : 1000, Name : \"abc\", BestFriend : { @odata.type: '" + typeof(Customer).FullName + "', ID: 1001, Name: 'Foo' } }", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Exists((c)=>c.ID == 1000), "Customer was inserted.");
                                AstoriaTestLog.IsTrue(context.Customers.Exists((c)=>c.ID == 1001), "Customer was inserted.");
                                AstoriaTestLog.IsTrue(context.Customers.Where((c)=>c.ID == 1000).First().BestFriend.ID == 1001, "Customer has new BestFriend.");
                                AstoriaTestLog.IsTrue(updates.Count == 2);
                                TestUtil.AssertEnumIn(updates.Where((u)=>u.Target is Customer).First().Action, UpdateOperations.Add);
                                TestUtil.AssertEnumIn(updates.Where((u)=>u.Target is Customer).Skip(1).First().Action, UpdateOperations.Add);
                            }
                    },
                    new AuthCase("A5.1 - Append to resource set, deep, bind new resource to nav reference - fail")
                    {
                        Rights = "Customers:RS,RM,WR,WM,WD", Method = "POST", RequestUri = "/Customers",
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\", ID : 1000, Name : \"abc\", BestFriend : { @odata.type: '" + typeof(Customer).FullName + "', ID: 1001, Name: 'Foo' } }",
                        AccessDeniedExpected = true
                    },
                    new AuthCase("A6 - Append to resource set, deep, bind existing resource to nav reference")
                    {
                        Rights = "Customers:WA,RS", Method = "POST", RequestUri = "/Customers",
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\", ID : 1000, Name : \"abc\", BestFriend@odata.bind : '/Customers(1)' }", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Exists((c)=>c.ID == 1000), "Customer was inserted.");
                                AstoriaTestLog.IsTrue(context.Customers.Where((c)=>c.ID == 1000).First().BestFriend.ID == 1, "Customer has new BestFriend.");
                                AstoriaTestLog.IsTrue(updates.Count == 1);
                                TestUtil.AssertEnumIn(updates.Where((u)=>u.Target is Customer).First().Action, UpdateOperations.Add);
                            }
                    },
                    new AuthCase("A6.1 - Append to resource set, deep, bind existing resource to nav reference - fail (no read right)")
                    {
                        Rights = "Customers:RM,WA,WR,WM,WD", Method = "POST", RequestUri = "/Customers",
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\", ID : 1000, Name : \"abc\", BestFriend@odata.bind : '/Customers(1)' }",
                        AccessDeniedExpected = true
                    },
                    new AuthCase("A6.2 - Append to resource set, deep, bind existing resource to nav reference - fail (no append right)")
                    {
                        Rights = "Customers:RS,RM,WR,WM,WD", Method = "POST", RequestUri = "/Customers",
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\", ID : 1000, Name : \"abc\", BestFriend@odata.bind :'/Customers(1)' }",
                        AccessDeniedExpected = true
                    },
                    new AuthCase("A7 - Bind to nav collection - both replace or merge rights should work")
                    {
                        Rights = "Customers:RS,WM; Orders:RS", Method = "POST", RequestUri = "/Customers(1)/Orders/$ref",
                        JsonPayload = "{ @odata.id : '/Orders(0)' }", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Where(c => c.ID == 1).First().Orders.Exists(o => o.ID == 0), "order got binded");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "Single update fired");
                                TestUtil.AssertEnumIn(updates.Where((u)=>u.Target is Customer).First().Action, UpdateOperations.Change);
                                AstoriaTestLog.IsTrue(updates.Where((u)=>u.Target is Order).Count() == 0);
                            }
                    },
                    new AuthCase("A7.1 - Bind to nav collection, both replace or merge rights should work")
                    {
                        Rights = "Customers:RS,WR; Orders:RS", Method = "POST", RequestUri = "/Customers(1)/Orders/$ref",
                        JsonPayload = "{ @odata.id : '/Orders(0)' }", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Where(c => c.ID == 1).First().Orders.Exists(o => o.ID == 0), "order got binded");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "Single update fired");
                                TestUtil.AssertEnumIn(updates.Where((u)=>u.Target is Customer).First().Action, UpdateOperations.Change);
                                AstoriaTestLog.IsTrue(updates.Where((u)=>u.Target is Order).Count() == 0);
                            }
                    },
                    new AuthCase("A7.2 - Bind to nav collection - fail (no update right on customers)")
                    {
                        Rights = "Customers:RS,RM,WA,WD; Orders:All", Method = "POST", RequestUri = "/Customers(1)/Orders/$ref",
                        JsonPayload = "{ @odata.id: '/Orders(0)' }", AccessDeniedExpected = true
                    },
                    new AuthCase("A7.3 - Bind to nav collection - fail (no read right on customers)")
                    {
                        Rights = "Customers:RM,WA,WR,WM,WD; Orders:All", Method = "POST", RequestUri = "/Customers(1)/Orders/$ref",
                        JsonPayload = "{ @odata.id: '/Orders(0)' }", AccessDeniedExpected = true
                    },
                    new AuthCase("A7.4 - Bind to nav collection - fail (no read right on orders)")
                    {
                        Rights = "Customers:All; Orders:RM,WA,WR,WM,WD", Method = "POST", RequestUri = "/Customers(1)/Orders/$ref",
                        JsonPayload = "{ @odata.id: '/Orders(0)' }", AccessDeniedExpected = true
                    },
#endregion // POST cases

#region PUT cases
                    ///////////////////////////////////////////////////////////
                    // PUT CASES
                    new AuthCase("U1M - Update top level entity (merge)")
                    {
                        // Why do we require a __metadata element for a scalar update? We do a refresh, which
                        // also implies read rights for an update.
                        Rights = "Customers:RS,WM", Method = "PATCH", RequestUri = "/Customers(2)", 
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\", Name: \"Changed\" }", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Exists((c)=>c.Name == "Changed"), "Customer name changed.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "Single update fired.");
                                AstoriaTestLog.IsTrue(updates[0].Action == UpdateOperations.Change, "Customer changed.");
                                AstoriaTestLog.IsTrue(updates[0].Target is Customer, "Customer changed.");
                            }
                    },
                    new AuthCase("U1M.A - Update top level entity (merge) (ATOM)")
                    {
                        Rights = "Customers:RS,WM", Method = "PATCH", RequestUri = "/Customers(2)", 
                        AtomPayload = AtomCustomerPayload(2, "Changed"), Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Exists((c)=>c.Name == "Changed"), "Customer name changed.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "Single update fired.");
                                AstoriaTestLog.IsTrue(updates[0].Action == UpdateOperations.Change, "Customer changed.");
                                AstoriaTestLog.IsTrue(updates[0].Target is Customer, "Customer changed.");
                            }
                    },
                    new AuthCase("U1R - Update top level entity (replace)")
                    {
                        Rights = "Customers:RS,WR", Method = "PUT", RequestUri = "/Customers(2)", 
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\"}", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsNull(context.Customers.Where((c)=>c.ID == 2).First().Name, "Customers 2 properties must have set to default values");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "Single update fired.");
                                AstoriaTestLog.IsTrue(updates[0].Action == UpdateOperations.Change, "change update was fired.");
                                AstoriaTestLog.IsTrue(((Customer)updates[0].Target).ID == 2, "Customer 2 got changed.");
                            }
                    },
                    new AuthCase("U1.1M - Update top level entity - fail (no merge right)")
                    {
                        Rights = "Customers:RS,RM,WA,WR,WD", Method = "PATCH", RequestUri = "/Customers(1)",
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\", Name: \"Changed\" }", AccessDeniedExpected = true
                    },
                    new AuthCase("U1.1R - Update top level entity - fail (no replace right)")
                    {
                        Rights = "Customers:RS,RM,WA,WM,WD", Method = "PUT", RequestUri = "/Customers(1)",
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\",Name: \"Changed\" }", AccessDeniedExpected = true
                    },
                    new AuthCase("U1.2M - Update top level entity - fail (no read right)")
                    {
                        Rights = "Customers:RM,WA,WR,WM,WD", Method = "PATCH", RequestUri = "/Customers(1)",
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\", Name: \"Changed\" }", AccessDeniedExpected = true
                    },
                    new AuthCase("U1.2R - Update top level entity - fail (no read right)")
                    {
                        Rights = "Customers:RM,WA,WR,WM,WD", Method = "PUT", RequestUri = "/Customers(1)",
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\", Name: \"Changed\" }", AccessDeniedExpected = true
                    },
                    new AuthCase("U2M - Update single reference to null") 
                    {
                        Rights = "Customers:RS,WM", Method = "PATCH", RequestUri = "/Customers(2)", 
                        JsonPayload = "{ @odata.type: '" + typeof(Customer).FullName + "', BestFriend: null }", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Exists((c)=>c.ID == 2 && c.BestFriend == null), "Customer BestFriend changed.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "Single update fired.");
                                AstoriaTestLog.IsTrue(updates[0].Action == UpdateOperations.Change, "Customer changed.");
                                AstoriaTestLog.IsTrue(updates[0].Target is Customer, "Customer changed.");
                            }
                    },
                    // cannot update deep relationship with PUT
                    new AuthCase("U2.1M - Update single reference to null - fail (no read single right)") 
                    {
                        Rights = "Customers:RM,WA,WR,WM,WD", Method = "PATCH", RequestUri = "/Customers(2)", 
                        JsonPayload = "{ @odata.type: '" + typeof(Customer).FullName + "', BestFriend: null }",
                        AccessDeniedExpected = true
                    },
                    new AuthCase("U2.2M - Update single reference to null - fail (no merge right)") 
                    {
                        Rights = "Customers:RS,RM,WA,WR,WD", Method = "PATCH", RequestUri = "/Customers(2)", 
                        JsonPayload = "{ @odata.type: '" + typeof(Customer).FullName + "', BestFriend: null }",
                        AccessDeniedExpected = true
                    },
                    new AuthCase("U3M - Update single reference")
                    {
                        Rights = "Customers:RS,WM", Method = "PATCH", RequestUri = "/Customers(2)", 
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\", BestFriend@odata.bind: \"/Customers(0)\" }", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Exists((c)=>c.ID == 2 && c.BestFriend.ID == 0), "Customer BestFirned changed.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "Single update fired.");
                                AstoriaTestLog.IsTrue(updates.Exists((u)=>u.Action == UpdateOperations.Change && ((Customer)u.Target).ID == 2), "Customer changed.");
                            }
                    },
                    // Updating relationships via PUT is not supported
                    new AuthCase("U3.1M - Update single reference - fail (no read single right")
                    {
                        Rights = "Customers:RM,WA,WR,WM,WD", Method = "PATCH", RequestUri = "/Customers(2)", 
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\", BestFriend@odata.bind: \"/Customers(0)\" }",
                        AccessDeniedExpected = true
                    },
                    new AuthCase("U3.2M - Update single reference - fail (no merge right")
                    {
                        Rights = "Customers:RS,RM,WA,WR,WD", Method = "PATCH", RequestUri = "/Customers(2)", 
                        JsonPayload = "{ @odata.type: \"" + typeof(Customer).FullName + "\", BestFriend@odata.bind: \"/Customers(0)\" }",
                        AccessDeniedExpected = true
                    },
                    new AuthCase("U4M - Update primitive value directly")
                    {
                        Rights = "Customers:RS,WM", Method = "PATCH", RequestUri = "/Customers(1)/Name/$value", 
                        TextPayload = "Howdy", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Exists((c)=>c.ID == 1 && c.Name == "Howdy"), "Customer name.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "Single update fired.");
                                AstoriaTestLog.IsTrue(updates[0].Action == UpdateOperations.Change, "Customer changed.");
                            }
                    },
                    new AuthCase("U4R - Update primitive value directly")
                    {
                        Rights = "Customers:RS,WR", Method = "PUT", RequestUri = "/Customers(1)/Name/$value", 
                        TextPayload = "Howdy", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Customers.Exists((c)=>c.ID == 1 && c.Name == "Howdy"), "Customer name.");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "Single update fired.");
                                AstoriaTestLog.IsTrue(updates[0].Action == UpdateOperations.Change, "Customer changed.");
                            }
                    },
                    new AuthCase("U4.1M - Update primitive value directly - fail")
                    {
                        Rights = "Customers:RS,RM,WA,WR,WD", Method = "PATCH", RequestUri = "/Customers(1)/Name/$value",
                        TextPayload = "H", AccessDeniedExpected = true
                    },
                    new AuthCase("U4.1R - Update primitive value directly - fail")
                    {
                        Rights = "Customers:RS,RM,WA,WM,WD", Method = "PUT", RequestUri = "/Customers(1)/Name/$value",
                        TextPayload = "H", AccessDeniedExpected = true
                    },
                    new AuthCase("U5M - Update Single Resource From NavCollection")
                    {
                        Rights = "Customers:RS;Orders:RS,WM", Method = "PATCH", RequestUri = "/Customers(1)/Orders(1)",
                        JsonPayload = "{ DollarAmount: 1111 }", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Orders.Exists((o)=>o.ID == 1 && o.DollarAmount == 1111), "Order property changed");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "Single update fired.");
                                AstoriaTestLog.IsTrue(updates[0].Action == UpdateOperations.Change && ((Order)updates[0].Target).ID == 1, "Order 1 got changed.");
                            }
                    },
                    new AuthCase("U5R - Update Single Resource From NavCollection")
                    {
                        Rights = "Customers:RS;Orders:RS,WR", Method = "PUT", RequestUri = "/Customers(1)/Orders(1)",
                        JsonPayload = "{ DollarAmount: 1111 }", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(context.Orders.Exists((o)=>o.ID == 1 && o.DollarAmount == 1111), "Order property changed");
                                AstoriaTestLog.IsTrue(updates.Count == 1, "Single update fired.");
                                AstoriaTestLog.IsTrue(updates[0].Action == UpdateOperations.Change && ((Order)updates[0].Target).ID == 1, "Order 1 got changed.");
                            }
                    },
                    new AuthCase("U5.1M - Update Single Resource From NavCollection - fail (no replace rights)")
                    {
                        Rights = "Customers:RS;Orders:RS,RM,WA,WR,WD", Method = "PATCH", RequestUri = "/Customers(1)/Orders(1)", AccessDeniedExpected = true,
                        JsonPayload = "null",
                    },
                    new AuthCase("U5.1R - Update Single Resource From NavCollection - fail (no replace rights)")
                    {
                        Rights = "Customers:RS;Orders:RS,RM,WA,WM,WD", Method = "PUT", RequestUri = "/Customers(1)/Orders(1)", AccessDeniedExpected = true,
                        JsonPayload = "null",
                    },
                    new AuthCase("U5.2M - Update Single Resource From NavCollection - fail (no read rights)")
                    {
                        Rights = "Customers:RS;Orders:RM,WA,WM,WR,WD", Method = "PATCH", RequestUri = "/Customers(1)/Orders(1)", AccessDeniedExpected = true,
                        JsonPayload = "null",
                    },
                    new AuthCase("U5.2R - Update Single Resource From NavCollection - fail (no read rights)")
                    {
                        Rights = "Customers:RS;Orders:RM,WA,WM,WR,WD", Method = "PUT", RequestUri = "/Customers(1)/Orders(1)", AccessDeniedExpected = true,
                        JsonPayload = "null",
                    },
                    new AuthCase("U5.3M - Update nav reference")
                    {
                        Rights = "Customers:RS,WM", Method = "PATCH", RequestUri = "/Customers(1)/BestFriend",
                        JsonPayload = "{ @odata.type: '" + typeof(Customer).FullName + "', Name: 'Foo' }", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(updates.Count == 1);
                                AstoriaTestLog.IsTrue(updates.Exists((u)=>((Customer)u.Target).ID == 0 && u.Action == UpdateOperations.Change), "Customers 1 got changed");
                            }
                    },
                    new AuthCase("U5.3R - Update nav reference")
                    {
                        Rights = "Customers:RS,WR", Method = "PUT", RequestUri = "/Customers(1)/BestFriend",
                        JsonPayload = "{ @odata.type: '" + typeof(Customer).FullName + "', Name: 'Foo' }", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(updates.Count == 1);
                                AstoriaTestLog.IsTrue(updates.Exists((u)=>((Customer)u.Target).ID == 0 && u.Action == UpdateOperations.Change), "Customers 1 got changed");
                            }
                    },
                    new AuthCase("U5.4M - Update nav reference")
                    {
                        Rights = "Customers:RS,WM;Orders:RS", Method = "PATCH", RequestUri = "/Orders(1)/Customer",
                        JsonPayload = "{ @odata.type: '" + typeof(CustomerWithBirthday).FullName + "', Name: 'Foo' }", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(updates.Count == 1);
                                AstoriaTestLog.IsTrue(updates.Exists((u)=>((Customer)u.Target).ID == 1 && u.Action == UpdateOperations.Change), "Orders 1 got changed");
                            }
                    },
                    new AuthCase("U5.4R - Update nav reference")
                    {
                        Rights = "Customers:RS,WR;Orders:RS", Method = "PUT", RequestUri = "/Orders(1)/Customer",
                        JsonPayload = "{ @odata.type: '" + typeof(CustomerWithBirthday).FullName + "', Name: 'Foo' }", Check = (context, updates) =>
                            {
                                AstoriaTestLog.IsTrue(updates.Count == 1);
                                AstoriaTestLog.IsTrue(updates.Exists((u)=>((Customer)u.Target).ID == 1 && u.Action == UpdateOperations.Change), "Orders 1 got changed");
                            }
                    },
                    new AuthCase("U6M - Unbind reference property")
                    {
                        Rights = "Customers:RS, WM", Method = "PATCH", RequestUri = "/Customers(1)/BestFriend/$ref",
                        JsonPayload = "{ @odata.id : '/Customers(2)' }", Check = (context, updates) =>
                        {
                            AstoriaTestLog.IsTrue(context.Customers.Where((c)=>c.ID == 1).First().BestFriend.ID == 2, "bind successfully done.");
                            AstoriaTestLog.IsTrue(updates.Count == 1, "Single update fired.");
                            AstoriaTestLog.IsTrue(updates[0].Action == UpdateOperations.Change, "Customer changed.");
                        }
                    },
                    new AuthCase("U6R - Unbind reference property")
                    {
                        Rights = "Customers:RS, WR", Method = "PUT", RequestUri = "/Customers(1)/BestFriend/$ref",
                        JsonPayload = "{@odata.id: '/Customers(2)' }", Check = (context, updates) =>
                        {
                            AstoriaTestLog.IsTrue(context.Customers.Where((c)=>c.ID == 1).First().BestFriend.ID == 2, "bind successfully done.");
                            AstoriaTestLog.IsTrue(updates.Count == 1, "Single update fired.");
                            AstoriaTestLog.IsTrue(updates[0].Action == UpdateOperations.Change && ((Customer)updates[0].Target).ID == 1, "Customer changed.");
                        }
                    },
                    new AuthCase("U6.1M - Unbind self reference property - fail (no merge right)")
                    {
                        Rights = "Customers:RS,RM,WA,WD,WR", Method = "PATCH", RequestUri = "/Customers(1)/BestFriend/$ref", AccessDeniedExpected = true,
                        JsonPayload = "{ @odata.id: '/Customers(2)' }"
                    },
                    new AuthCase("U6.1R - Unbind self reference property - fail (no replace right)")
                    {
                        Rights = "Customers:RS,RM,WA,WD,WM", Method = "PUT", RequestUri = "/Customers(1)/BestFriend/$ref", AccessDeniedExpected = true,
                        JsonPayload = "{ @odata.id: '/Customers(2)' }"
                    },
                    new AuthCase("U6.2M - Unbind self reference property - fail (no read single right)")
                    {
                        Rights = "Customers:RM,WA,WD,WM,WR", Method = "PATCH", RequestUri = "/Customers(1)/BestFriend/$ref", AccessDeniedExpected = true,
                        JsonPayload = "{ @odata.id: '/Customers(2)' }"
                    },
                    new AuthCase("U6.2R - Unbind self reference property - fail (no read single right)")
                    {
                        Rights = "Customers:RM,WA,WD,WM,WR", Method = "PUT", RequestUri = "/Customers(1)/BestFriend/$ref", AccessDeniedExpected = true,
                        JsonPayload = "{ @odata.id: '/Customers(2)' }"
                    },
                    new AuthCase("U6.3M - Unbind reference property")
                    {
                        Rights = "Customers:RS;Orders:RS,WM", Method = "PATCH", RequestUri = "/Orders(1)/Customer/$ref",
                        JsonPayload = "{ @odata.id: '/Customers(2)' }", Check = (context, updates) =>
                        {
                            AstoriaTestLog.IsTrue(context.Orders.Where((o)=>o.ID == 1).First().Customer.ID == 2, "bind successfully done.");
                            AstoriaTestLog.IsTrue(updates.Count == 1, "Single update fired.");
                            AstoriaTestLog.IsTrue(updates[0].Action == UpdateOperations.Change && ((Order)updates[0].Target).ID == 1, "Order changed.");
                        }
                    },
                    new AuthCase("U6.3R - Unbind reference property")
                    {
                        Rights = "Customers:RS;Orders:RS,WR", Method = "PUT", RequestUri = "/Orders(1)/Customer/$ref",
                        JsonPayload = "{ @odata.id: '/Customers(2)' }", Check = (context, updates) =>
                        {
                            AstoriaTestLog.IsTrue(context.Orders.Where((o)=>o.ID == 1).First().Customer.ID == 2, "bind successfully done.");
                            AstoriaTestLog.IsTrue(updates.Count == 1, "Single update fired.");
                            AstoriaTestLog.IsTrue(updates[0].Action == UpdateOperations.Change && ((Order)updates[0].Target).ID == 1, "Order changed.");
                        }
                    },
                    new AuthCase("U6.4M - Unbind reference property - fail (orders no merge right)")
                    {
                        Rights = "Customers:All;Orders:RS,RM,WA,WR,WD", Method = "PATCH", RequestUri = "/Orders(1)/Customer/$ref", AccessDeniedExpected = true,
                        JsonPayload = "{ @odata.id: '/Customers(2)' }"
                    },
                    new AuthCase("U6.4R - Unbind reference property - fail (orders no replace right)")
                    {
                        Rights = "Customers:All;Orders:RS,RM,WA,WM,WD", Method = "PUT", RequestUri = "/Orders(1)/Customer/$ref", AccessDeniedExpected = true,
                        JsonPayload = "{ url '/Customers(2)' }"
                    },
                    new AuthCase("U6.5M - Unbind reference property - fail (Orders no read single right)")
                    {
                        Rights = "Customers:All;Orders:RM,WA,WR,WM,WD", Method = "PATCH", RequestUri = "/Orders(1)/Customer/$ref", AccessDeniedExpected = true,
                        JsonPayload = "{ @odata.id: '/Customers(2)' }"
                    },
                    new AuthCase("U6.5R - Unbind reference property - fail (Orders no read single right)")
                    {
                        Rights = "Customers:All;Orders:RM,WA,WR,WM,WD", Method = "PUT", RequestUri = "/Orders(1)/Customer/$ref", AccessDeniedExpected = true,
                        JsonPayload = "{ @odata.id: '/Customers(2)' }"
                    },
                    new AuthCase("U6.6R - Unbind reference property - fail (Customers no read single right)")
                    {
                        Rights = "Customers:RM,WA,WD,WM,WR;Orders:All", Method = "PUT", RequestUri = "/Orders(1)/Customer/$ref", AccessDeniedExpected = true,
                        JsonPayload = "{ @odata.id: '/Customers(2)' }"
                    },
                    new AuthCase("U6.6M - Unbind reference property - fail (Customers no read single right)")
                    {
                        Rights = "Customers:RM,WA,WD,WM,WR;Orders:All", Method = "PATCH", RequestUri = "/Orders(1)/Customer/$ref", AccessDeniedExpected = true,
                        JsonPayload = "{ @odata.id: '/Customers(2)' }"
                    },

                    ///////////////////////////////////////////////////////////
                    // EXPAND CASES
                    new AuthCase("E1 - expand - fails")
                    {
                        Rights = "Customers:RS;Orders:WA", Method = "GET", RequestUri = "/Customers(0)?$expand=Orders", AccessDeniedExpected = true,
                    },
                    new AuthCase("E2 - expand - fail again")
                    {
                        Rights = "Customers:RS;Orders:RS", Method = "GET", RequestUri = "/Customers(0)?$expand=Orders", AccessDeniedExpected = true,
                    },
                    new AuthCase("E2 - expand - succeed")
                    {
                        Rights = "Customers:RS;Orders:RM", Method = "GET", RequestUri = "/Customers(0)?$expand=Orders"
                    },
#endregion // PUT cases

#region GET cases
                    ///////////////////////////////////////////////////////////
                    // GET CASES
                    new AuthCase("G1 - Get Resources From Entity Set")
                    {
                        Rights = "Customers:RM", Method = "GET", RequestUri = "/Customers"
                    },
                    new AuthCase("G2 - Get Single Resource From Entity Set")
                    {
                        Rights = "Customers:RS", Method = "GET", RequestUri = "/Customers(1)"
                    },
                    new AuthCase("G3 - Get Resources From Navigation Collection Property")
                    {
                        Rights = "Customers:RS;Orders:RM", Method = "GET", RequestUri = "/Customers(1)/Orders"
                    },
                    new AuthCase("G3-F1 - Get Resources From Navigation Collection Property - No ReadSingle on Customers")
                    {
                        Rights = "Orders:ALL", Method = "GET", RequestUri = "/Customers(1)/Orders", AccessDeniedExpected = true,
                    },
                    new AuthCase("G3-F2 - Get Resources From Navigation Collection Property - No ReadMultiple on Orders")
                    {
                        Rights = "Customers:ALL", Method = "GET", RequestUri = "/Customers(1)/Orders", AccessDeniedExpected = true,
                    },
                    new AuthCase("G4 - Get Resource From Navigation Collection Property")
                    {
                        Rights = "Customers:RS;Orders:RS", Method = "GET", RequestUri = "/Customers(1)/Orders(1)"
                    },
                    new AuthCase("G4-F1 - Get Resource From Navigation Collection Property - No ReadSingle on Customers")
                    {
                        Rights = "Orders:ALL", Method = "GET", RequestUri = "/Customers(1)/Orders(1)", AccessDeniedExpected = true,
                    },
                    new AuthCase("G4-F2 - Get Resource From Navigation Collection Property - No ReadSingle on Orders")
                    {
                        Rights = "Customers:ALL", Method = "GET", RequestUri = "/Customers(1)/Orders(1)", AccessDeniedExpected = true,
                    },
                    new AuthCase("G5 - Get Resources From Navigation Reference Property")
                    {
                        Rights = "Customers:RS;Orders:RS", Method = "GET", RequestUri = "/Orders(1)/Customer"
                    },
                    new AuthCase("G5-F1 - Get Resources From Navigation Reference Property - No ReadSingle on Customers")
                    {
                        Rights = "Orders:ALL", Method = "GET", RequestUri = "/Orders(1)/Customer", AccessDeniedExpected = true,
                    },
                    new AuthCase("G5-F2 - Get Resources From Navigation Reference Property - No ReadSingle on Orders")
                    {
                        Rights = "Customers:ALL", Method = "GET", RequestUri = "/Orders(1)/Customer", AccessDeniedExpected = true,
                    },
                    new AuthCase("G6 - Get Link Uris From Navigation Collection Property")
                    {
                        Rights = "Customers:RS;Orders:RM", Method = "GET", RequestUri = "/Customers(1)/Orders/$ref"
                    },
                    new AuthCase("G6-F1 - Get Link Uris From Navigation Collection Property - No ReadSingle on Customers")
                    {
                        Rights = "Orders:ALL", Method = "GET", RequestUri = "/Customers(1)/Orders/$ref", AccessDeniedExpected = true,
                    },
                    new AuthCase("G6-F2 - Get Link Uris From Navigation Collection Property - No ReadMultiple on Orders")
                    {
                        Rights = "Customers:ALL", Method = "GET", RequestUri = "/Customers(1)/Orders/$ref", AccessDeniedExpected = true,
                    },
                    new AuthCase("G7 - Get Link Uri From Navigation Collection Property")
                    {
                        Rights = "Customers:RS;Orders:RS", Method = "GET", RequestUri = "/Customers(1)/Orders(1)/$ref"
                    },
                    new AuthCase("G7-F1 - Get Link Uri From Navigation Collection Property - No ReadSingle on Customers")
                    {
                        Rights = "Orders:ALL", Method = "GET", RequestUri = "/Customers(1)/Orders(1)/$ref", AccessDeniedExpected = true,
                    },
                    new AuthCase("G7-F2 - Get Link Uri From Navigation Collection Property - No ReadSingle on Orders")
                    {
                        Rights = "Customers:ALL", Method = "GET", RequestUri = "/Customers(1)/Orders(1)/$ref", AccessDeniedExpected = true,
                    },
                    new AuthCase("G8 - Get Link Uri From Navigation Reference Property")
                    {
                        Rights = "Customers:RS;Orders:RS", Method = "GET", RequestUri = "/Orders(1)/Customer/$ref"
                    },
                    new AuthCase("G8-F1 - Get Link Uri From Navigation Reference Property - No ReadSingle on Customers")
                    {
                        Rights = "Orders:ALL", Method = "GET", RequestUri = "/Orders(1)/Customer/$ref", AccessDeniedExpected = true,
                    },
                    new AuthCase("G8-F2 - Get Link Uri From Navigation Reference Property - No ReadSingle on Orders")
                    {
                        Rights = "Customers:ALL", Method = "GET", RequestUri = "/Orders(1)/Customer/$ref", AccessDeniedExpected = true,
                    },
                    new AuthCase("G9 - Get $count values for Top Level Customers")
                    {
                        Rights = "Customers:RM", Method = "GET", RequestUri = "/Customers/$count"
                    },
                    new AuthCase("G9 - F1 - Get $count values for Top Level Customers")
                    {
                        Rights = "Customers:RS,WA,WR,WM,WD", Method = "GET", RequestUri = "/Customers/$count", AccessDeniedExpected = true,
                    },
                    new AuthCase("G10 - Get $count values for Navigation collection property")
                    {
                        Rights = "Customers:RS;Orders:RM", Method = "GET", RequestUri = "/Customers(1)/Orders/$count"
                    },
                    new AuthCase("G10 - F1 - Get $count values for Navigation collection property")
                    {
                        Rights = "Customers:All;Orders:RS,WA,WR,WM,WD", Method = "GET", RequestUri = "/Customers(1)/Orders/$count", AccessDeniedExpected = true
                    },
                    new AuthCase("G10 - F2- Get $count values for Navigation collection property")
                    {
                        Rights = "Customers:RM,WA,WR,WM,WD;Orders:All", Method = "GET", RequestUri = "/Customers(1)/Orders/$count", AccessDeniedExpected = true
                    },

#region GET Projections
                    new AuthCase("Projections - simple and complex top level projection, RM")
                    {
                        Rights = "Customers:RM", Method = "GET", RequestUri = "/Customers?$select=ID,Name,Address"
                    },
                    new AuthCase("Projections - * top level projection, RM")
                    {
                        Rights = "Customers:RM", Method = "GET", RequestUri = "/Customers?$select=*"
                    },
                    new AuthCase("Projections - simple and complex top level projection, RS")
                    {
                        Rights = "Customers:RS", Method = "GET", RequestUri = "/Customers?$select=ID,Name,Address", AccessDeniedExpected = true
                    },
                    new AuthCase("Projections - * top level projection, RS")
                    {
                        Rights = "Customers:RS", Method = "GET", RequestUri = "/Customers?$select=*", AccessDeniedExpected = true
                    },
                    new AuthCase("Projections - simple and complex top level projection, N")
                    {
                        Rights = "Customers:N", Method = "GET", RequestUri = "/Customers?$select=ID,Name,Address", AccessDeniedExpected = true
                    },
                    new AuthCase("Projections - * top level projection, N")
                    {
                        Rights = "Customers:N", Method = "GET", RequestUri = "/Customers?$select=*", AccessDeniedExpected = true
                    },
                    new AuthCase("Projections - reference navigation all, RM") // Fails because RM doesn't grant access to a single resource BestFriend
                    {
                        Rights = "Customers:RM", Method = "GET", RequestUri = "/Customers?$select=BestFriend&$expand=BestFriend", AccessDeniedExpected = true
                    },
                    new AuthCase("Projections - reference navigation all, RS")
                    {
                        Rights = "Customers:RS", Method = "GET", RequestUri = "/Customers(1)?$select=BestFriend&$expand=BestFriend",
                    },
                    new AuthCase("Projections - reference navigation all, N")
                    {
                        Rights = "Customers:N", Method = "GET", RequestUri = "/Customers(1)?$select=BestFriend&$expand=BestFriend", AccessDeniedExpected = true
                    },
                    new AuthCase("Projections - reference navigation simple, complex projections, RS")
                    {
                        Rights = "Customers:RS", Method = "GET", RequestUri = "/Customers(1)?$expand=BestFriend($select=ID,Address)",
                    },
                    new AuthCase("Projections - reference navigation simple, complex projections, N")
                    {
                        Rights = "Customers:N", Method = "GET", RequestUri = "/Customers(1)?$expand=BestFriend($select=ID,Address)", AccessDeniedExpected = true
                    },
                    new AuthCase("Projections - collection navigation all, RM")
                    {
                        Rights = "Customers:ALL;Orders:RM", Method = "GET", RequestUri = "/Customers?$select=Orders&$expand=Orders"
                    },
                    new AuthCase("Projections - collection navigation all, RS")
                    {
                        Rights = "Customers:ALL;Orders:RS", Method = "GET", RequestUri = "/Customers(1)?$select=Orders&$expand=Orders", AccessDeniedExpected = true
                    },
                    new AuthCase("Projections - collection navigation all, N")
                    {
                        Rights = "Customers:ALL;Orders:N", Method = "GET", RequestUri = "/Customers(1)?$select=Orders&$expand=Orders", AccessDeniedExpected = true
                    },
                    new AuthCase("Projections - collection navigation simple, RM")
                    {
                        Rights = "Customers:ALL;Orders:RM", Method = "GET", RequestUri = "/Customers?$expand=Orders($select=ID)"
                    },
                    new AuthCase("Projections - collection navigation simple, RS")
                    {
                        Rights = "Customers:ALL;Orders:RS", Method = "GET", RequestUri = "/Customers(1)?$expand=Orders($select=ID)", AccessDeniedExpected = true
                    },
                    new AuthCase("Projections - collection navigation simple, N")
                    {
                        Rights = "Customers:ALL;Orders:N", Method = "GET", RequestUri = "/Customers(1)?$expand=Orders($select=ID)", AccessDeniedExpected = true
                    },
                    new AuthCase("Projections - deep and various projections, RM")
                    {
                        Rights = "Customers:RM;Orders:RM", Method = "GET", RequestUri = "/Customers?$select=Orders&$expand=BestFriend($select=*;$expand=Orders($select=ID))", AccessDeniedExpected = true
                    },
                    new AuthCase("Projections - deep and various projections, AR")
                    {
                        Rights = "Customers:AR;Orders:AR", Method = "GET", RequestUri = "/Customers?$select=Orders&$expand=BestFriend($select=*;$expand=Orders($select=ID))"
                    },
                    new AuthCase("Projections - deep and various projections, just right")
                    {
                        Rights = "Customers:RS;Orders:RM", Method = "GET", RequestUri = "/Customers(1)?$select=Orders&$expand=BestFriend($select=*;$expand=Orders($select=ID))"
                    },
                    new AuthCase("Projections - just link, N")
                    {
                        Rights = "Customers:RM;Orders:N", Method = "GET", RequestUri = "/Customers?$select=Orders", AccessDeniedExpected = true
                    },
                    new AuthCase("Projections - just link, RM")
                    {
                        Rights = "Customers:RM;Orders:RM", Method = "GET", RequestUri = "/Customers?$select=Orders"
                    },
                    new AuthCase("Projections - just link, RS")
                    {
                        Rights = "Customers:RS;Orders:RM", Method = "GET", RequestUri = "/Orders?$select=Customer"
                    },

#endregion
#endregion
                };

                IEnumerable<AuthCase> testCases = cases;

                TestUtil.RunCombinations(testCases, (testCase) =>
                {
                    //if (!testCase.Title.StartsWith("G9")) return; // Uncomment to filter for a specific case.
                    CustomDataContext.ClearData();
                    using (TestUtil.RestoreStaticValueOnDispose(typeof(CustomDataContext), "PreserveChanges"))
                    {
                        CustomDataContext.PreserveChanges = true;
                        string etag = null;
                        using (TestUtil.MetadataCacheCleaner())
                        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                        {
                            using (InitializationCallbackManager.RegisterStatic(delegate(object sender, InitializationCallbackEventArgs args)
                            {
                                Trace.WriteLine("Initialization called on etag path...");
                                var r = new List<KeyValuePair<string, EntitySetRights>>()
                                {
                                    new KeyValuePair<string, EntitySetRights>("*", EntitySetRights.All)
                                };
                                TestUtil.ApplyResourceRights(r, args.Configuration);
                            }))
                            {
                                request.ServiceType = typeof(WebDataServiceA);
                                request.RequestUriString = testCase.RequestUri;
                                request.HttpMethod = "GET";
                                request.Accept = "*/*";
                                request.SendRequest();
                                etag = request.ResponseETag;
                            }
                        }

                        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                        {
                            request.ServiceType = typeof(WebDataServiceA);
                            List<UpdateCallback> updates = new List<UpdateCallback>();
                            using (InitializationCallbackManager.RegisterStatic(delegate(object sender, InitializationCallbackEventArgs args)
                            {
                                Trace.WriteLine("Initialization called...");
                                TestUtil.ApplyResourceRights(testCase.ResourceRights, args.Configuration);
                            }))
                            using (StaticCallbackManager<UpdatedEventArgs>.RegisterStatic(delegate(object sender, UpdatedEventArgs args)
                            {
                                Trace.WriteLine("Update called...");
                                updates.Add(new UpdateCallback() { Action = args.Action, MethodName = args.MethodName, Target = args.Target });
                            }))
                            {
                                TestUtil.ClearMetadataCache();
                                request.HttpMethod = testCase.Method;
                                request.RequestUriString = testCase.RequestUri;
                                string contentText = null;
                                string contentType = null;
                                if (testCase.JsonPayload != null)
                                {
                                    contentText = testCase.JsonPayload;
                                    contentType = UnitTestsUtil.JsonLightMimeType;
                                }

                                if (testCase.TextPayload != null)
                                {
                                    System.Diagnostics.Debug.Assert(contentText == null, "contentText == null");
                                    contentText = testCase.TextPayload;
                                    contentType = "text/plain";
                                }

                                if (testCase.AtomPayload != null)
                                {
                                    System.Diagnostics.Debug.Assert(contentText == null, "contentText == null");
                                    contentText = testCase.AtomPayload;
                                    contentType = SerializationFormatData.Atom.MimeTypes[0];
                                }

                                request.IfMatch = etag;
                                if (contentText != null)
                                {
                                    request.SetRequestStreamAsText(contentText);
                                    request.RequestContentType = contentType;
                                }

                                Exception exception = TestUtil.RunCatching(request.SendRequest);
                                TestUtil.AssertExceptionExpected(exception, testCase.AccessDeniedExpected);
                                if (testCase.Check != null)
                                {
                                    testCase.Check(new CustomDataContext(), updates);
                                }
                            }
                        }
                    }
                });
            }

            internal class UpdateCallback
            {
                internal UpdateOperations Action { get; set; }
                internal string MethodName { get; set; }
                internal object Target { get; set; }
            }

            internal class AuthCase
            {
                private string title;
                private string rights;
                private IEnumerable<KeyValuePair<string, EntitySetRights>> resourceRights;

                internal AuthCase(string title)
                {
                    this.title = title;
                }

                internal AuthCase(AuthCase other)
                {
                    this.title = other.title;
                    this.rights = other.rights;
                    this.resourceRights = other.resourceRights;
                    this.AccessDeniedExpected = other.AccessDeniedExpected;
                    this.AtomPayload = other.AtomPayload;
                    this.Check = other.Check;
                    this.Method = other.Method;
                    this.JsonPayload = other.JsonPayload;
                    this.RequestUri = other.RequestUri;
                    this.TextPayload = other.TextPayload;
                }

                /// <summary>Rights to </summary>
                internal string Rights
                {
                    get { return this.rights; }
                    set
                    {
                        this.rights = value;
                        this.resourceRights = TestUtil.ParseResourceRightsText(this.rights);
                    }
                }

                internal bool AccessDeniedExpected { get; set; }
                internal string AtomPayload { get; set; }
                internal Action<CustomDataContext, List<UpdateCallback>> Check { get; set; }
                internal string Method { get; set; }
                internal string JsonPayload { get; set; }
                internal string RequestUri { get; set; }
                internal IEnumerable<KeyValuePair<string, EntitySetRights>> ResourceRights { get { return this.resourceRights; } }
                internal string TextPayload { get; set; }
                internal string Title { get { return this.title; } set { this.title = value; } }

                public override string ToString()
                {
                    return this.Title;
                }
            }

            // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void AuthorizationCallbackForReadingTest()
            {
                const string BehaviorFilter = "BehaviorFilter";
                const string BehaviorPassThrough = "BehaviorPassThrough";
                const string BehaviorThrow = "BehaviorThrow";

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Behavior", new object[] { BehaviorPassThrough, BehaviorFilter, BehaviorThrow }),
                    new Dimension("Path", new object[] { "/Customers", "/Customers(1)", "/Customers(1)/Orders", "/Customers(2)/BestFriend" }));

                using (TestUtil.MetadataCacheCleaner())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.ServiceType = typeof(WebDataServiceA);
                    request.Accept = "application/atom+xml,application/xml";
                    TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                    {
                        string behavior = (string)values["Behavior"];
                        string path = (string)values["Path"];
                        int invocationCount = 0;
                        using (InitializationCallbackManager.RegisterStatic(delegate(object sender, InitializationCallbackEventArgs args)
                        {
                            args.Configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
                        }))
                        using (StaticCallbackManager<ComposeQueryEventArgs>.RegisterStatic(delegate(object sender, ComposeQueryEventArgs args)
                        {
                            invocationCount++;

                            if (behavior == BehaviorThrow)
                            {
                                throw new DataServiceException(403, "403");
                            }
                            else if (behavior == BehaviorFilter)
                            {
                                System.Linq.Expressions.LambdaExpression predicate = System.Linq.Expressions.Expression.Lambda(
                                    System.Linq.Expressions.Expression.Constant(false),
                                    System.Linq.Expressions.Expression.Parameter(args.ElementType, "p"));
                                args.Filter = predicate;
                            }
                        }))
                        {
                            request.RequestUriString = path;
                            Exception exception = TestUtil.RunCatching(request.SendRequestAndCheckResponse);
                            bool isSet = path.EndsWith("Customers") || path.EndsWith("Orders");
                            TestUtil.AssertExceptionExpected(exception, 
                                behavior == BehaviorThrow,
                                behavior == BehaviorFilter && !isSet);
                            if (exception == null)
                            {
                                if (path.Contains("BestFriend"))
                                {
                                    AstoriaTestLog.AreEqual(2, invocationCount, "Two invocations for BestFriend path.");
                                }
                                else
                                {
                                    AstoriaTestLog.AreEqual(1, invocationCount, "Single invocations for non-BestFriend path.");
                                }
                            }
                        }
                    });
                }
            }

            [TestMethod, Variation]
            public void AuthorizationCallbackSignaturesTest()
            {
                Type customerQueryable = typeof(IQueryable<Customer>);
                Type customerPredicate = typeof(Expression<Func<Customer, bool>>);

                List<SignatureModel> signatureModels = new List<SignatureModel>
                {
                    // 1 - Invalid container name.
                    new SignatureModel() { Attribute = "query", ContainerName = "Foo", IsValid = false },
                    new SignatureModel() { Attribute = "change", ContainerName = "Foo", IsValid = false },
                    // 2 - Invalid parameter count.
                    new SignatureModel() 
                    { 
                        Attribute = "query", ContainerName = "Customers", IsValid = false,
                        ParameterTypes = new List<Type>() { typeof(Customer) }
                    },
                    new SignatureModel() { Attribute = "change", ContainerName = "Customers", IsValid = false },

                    ///////////////////////////////////////////////////////////
                    // Reading authorization callbacks.
                    // 3 - Invalid parameter type.
                    new SignatureModel() 
                    { 
                        ContainerName = "Customers", ReturnType = customerQueryable, IsValid = false, Attribute = "query",
                        ParameterTypes = new List<Type>() { typeof(Customer) }
                    },
                    // 4 - Invalid result type.
                    new SignatureModel() 
                    { 
                        ContainerName = "Customers", ReturnType = typeof(bool), IsValid = false, Attribute = "query",
                    },
                    // 5 - Invalid result type with special error.
                    new SignatureModel() 
                    { 
                        ContainerName = "Customers", ReturnType = typeof(void), IsValid = false, Attribute = "query",
                    },
                    // 6 - Valid result type with bool?
                    new SignatureModel() 
                    { 
                        ContainerName = "Customers", ReturnType = typeof(Expression<Func<Customer, bool?>>), IsValid = true, Attribute = "query",
                    },
                    // 7 - Invalid type change for result type.
                    new SignatureModel() 
                    { 
                        ContainerName = "Customers", ReturnType = typeof(Expression<Func<object, bool>>), IsValid = false, Attribute = "query",
                    },
                    // 8 - Valid result type
                    new SignatureModel() 
                    { 
                        ContainerName = "Customers", ReturnType = customerPredicate, IsValid = true, Attribute = "query",
                    },

                    ///////////////////////////////////////////////////////////
                    // Writing authorization callbacks.
                    // 10 - Invalid parameter direction.
                    new SignatureModel() 
                    { 
                        ContainerName = "Customers", IsValid = false, Attribute = "change",
                        ParameterTypes = new List<Type>() { typeof(Customer), typeof(UpdateOperations) },
                        ParamAttributes = new List<ParameterAttributes>() { ParameterAttributes.Out, ParameterAttributes.None }
                    },
                    // 11 - Invalid parameter direction.
                    new SignatureModel() 
                    { 
                        ContainerName = "Customers", IsValid = false, Attribute = "change",
                        ParameterTypes = new List<Type>() { typeof(Customer), typeof(UpdateOperations) },
                        ParamAttributes = new List<ParameterAttributes>() { ParameterAttributes.None, ParameterAttributes.Out }
                    },
                    // 12 - Invalid parameter type.
                    new SignatureModel() 
                    { 
                        ContainerName = "Customers", IsValid = false, Attribute = "change",
                        ParameterTypes = new List<Type>() { typeof(CustomerWithBirthday), typeof(UpdateOperations) },
                    },
                    // 13 - Invalid parameter type.
                    new SignatureModel() 
                    { 
                        ContainerName = "Customers", IsValid = false, Attribute = "change",
                        ParameterTypes = new List<Type>() { typeof(Customer), typeof(int) },
                    },
                    // 14 - Invalid return type
                    new SignatureModel() 
                    { 
                        ContainerName = "Customers", ReturnType = typeof(bool), IsValid = false, Attribute = "change",
                        ParameterTypes = new List<Type>() { typeof(Customer), typeof(UpdateOperations) },
                    },
                    // 15 - Valid parameter type modification.
                    new SignatureModel() 
                    { 
                        ContainerName = "Customers", IsValid = true, Attribute = "change",
                        ParameterTypes = new List<Type>() { typeof(object), typeof(UpdateOperations) },
                    },
                };

                Assembly assembly = BuildSignatureModels(signatureModels);
                for (int i = 0; i < signatureModels.Count; i++)
                {
                    SignatureModel model = signatureModels[i];
                    System.Diagnostics.Trace.WriteLine("Testing model #" + (i + 1) + " of " + signatureModels.Count + "...");
                    using (InitializationCallbackManager.RegisterStatic(delegate(object sender, InitializationCallbackEventArgs args)
                    {
                        args.Configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
                    }))
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.ServiceType = model.ServiceType;
                        string[] queries = new string[] 
                        { 
                            "/Customers", 
                            "/Customers(1)/BestFriend", 
                            "/Customers?$expand=BestFriend", 
                            "BATCH /Customers", 
                            "BATCH /Customers(1)/BestFriend", 
                            "BATCH /Customers?$expand=BestFriend"
                        };
                        foreach (string s in queries)
                        {
                            if (s.StartsWith("BATCH "))
                            {
                                request.HttpMethod = "POST";
                                request.RequestUriString = "/$batch";
                                string requestContentType;
                                string payload = 
                                    BatchTestUtil.CreateBatchText(out requestContentType,
                                        BatchTestUtil.CreateOperationRequestText("GET", s.Substring("BATCH ".Length), "", ""));
                                request.RequestContentType = requestContentType;
                                request.SetRequestStreamAsText(payload);
                            }
                            else
                            {
                                request.HttpMethod = "GET";
                                request.RequestUriString = s;
                                request.RequestContentLength = 0;
                                request.RequestStream = null;
                            }

                            string responseText = null;
                            Exception exception = TestUtil.RunCatching(delegate()
                            {
                                request.SendRequest();
                                responseText = request.GetResponseStreamAsText();
                            });
                            TestUtil.AssertExceptionExpected(exception, !model.IsValid);
                            if (responseText != null && s.StartsWith("BATCH"))
                            {
                                if (model.IsValid)
                                {
                                    TestUtil.AssertContainsFalse(responseText, "<error");
                                }
                                else
                                {
                                    TestUtil.AssertContains(responseText, "<error");
                                }
                            }
                        }
                    }
                }
            }

            [TestMethod]
            public void ConfigurationSealed()
            {
                // Verify that the configuration is sealed after the service is initialized.
                var names = "MaxBatchCount,MaxChangesetCount,MaxExpandCount,MaxExpandDepth,MaxResultsPerCollection".Split(',');
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("PropertyName", names));
                TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                {
                    string name = (string)values["PropertyName"];
                    using (TestUtil.MetadataCacheCleaner())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.ServiceType = typeof(WebDataServiceA);
                        request.RequestUriString = "/Customers";
                        TestUtil.ClearMetadataCache();
                        IDataServiceConfiguration configuration = null;
                        using (InitializationCallbackManager.RegisterStatic((sender, args) =>
                            {
                                configuration = args.Configuration;
                                configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
                            }))
                        using (StaticCallbackManager<ComposeQueryEventArgs>.RegisterStatic((sender, args) =>
                        {
                            configuration.GetType().GetProperty(name).SetValue(configuration, 100, TestUtil.EmptyObjectArray);
                        }))
                        {
                            Exception exception = TestUtil.RunCatching(request.SendRequest);
                            TestUtil.AssertExceptionExpected(exception, true);
                        }
                    }
                });
            }

            [TestMethod]
            public void LoadInvalidMestScenario()
            {
                AdHocEntityType orderType = new AdHocEntityType("OrderType");
                AdHocEntityType customerType = new AdHocEntityType("CustomerType");
                AdHocEntitySet goodOrderSet = new AdHocEntitySet() { Name = "GoodOrders", Type = orderType };
                AdHocEntitySet badOrderSet = new AdHocEntitySet() { Name = "BadOrders", Type = orderType };
                AdHocEntitySet goodCustomerSet = new AdHocEntitySet() { Name = "GoodCustomers", Type = customerType };
                AdHocEntitySet badCustomerSet = new AdHocEntitySet() { Name = "BadCustomers", Type = customerType };
                AdHocContainer container = new AdHocContainer()
                {
                    EntitySets = new List<AdHocEntitySet>() { goodCustomerSet, badCustomerSet, goodOrderSet, badOrderSet }
                };
                container.AddCollectionAssociation(goodCustomerSet, goodOrderSet);
                container.AddCollectionAssociation(badCustomerSet, badOrderSet);

                AdHocModel model = new AdHocModel() { Containers = new List<AdHocContainer>() { container }, ConceptualNs = TestXmlConstants.EdmV1Namespace };
                Assembly assembly = model.GenerateModelsAndAssembly("LoadInvalidMestScenario", false);
                Type dataContextType = TestUtil.LoadDerivedTypeFromAssembly(assembly, typeof(System.Data.Objects.ObjectContext));

                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = dataContextType;
                    request.RequestUriString = "/$metadata";
                    Exception exception = TestUtil.RunCatching(delegate()
                    {
                        request.SendRequest();
                        using (var s = new StreamReader(request.GetResponseStream())) s.ReadToEnd();
                    });
                    TestUtil.AssertExceptionExpected(exception, false);
                }

                Type callbackContextType = typeof(WebDataServiceInitCallback<>).MakeGenericType(dataContextType);
                using (InitializationCallbackManager.RegisterStatic(delegate(object sender, InitializationCallbackEventArgs args)
                {
                    args.Configuration.SetEntitySetAccessRule("GoodOrders", EntitySetRights.AllRead);
                    args.Configuration.SetEntitySetAccessRule("BadOrders", EntitySetRights.AllWrite);
                }))
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.ServiceType = callbackContextType;
                    request.RequestUriString = "/$metadata";
                    Exception exception = TestUtil.RunCatching(delegate()
                    {
                        request.SendRequest();
                        using (var s = new StreamReader(request.GetResponseStream())) s.ReadToEnd();
                    });
                    TestUtil.AssertExceptionExpected(exception, true);
                }
            }

            private Assembly BuildSignatureModels(IEnumerable<SignatureModel> models)
            {
                ModuleBuilder module = TestUtil.CreateModuleBuilder("BuildSignaturesAssembly");
                List<TypeBuilder> types = new List<TypeBuilder>();

                Type baseType = typeof(WebDataServiceA);
                int modelIndex = 1;
                foreach (SignatureModel model in models)
                {
                    ConstructorInfo attributeConstructor = model.AttributeType.GetConstructor(new Type[] { typeof(string) });
                    string modelName = "Model" + modelIndex;
                    TypeBuilder type = module.DefineType(modelName, TypeAttributes.Class | TypeAttributes.Public, baseType);
                    types.Add(type);

                    Type returnType = model.ReturnType ?? typeof(void);
                    Type[] parameterTypes = (model.ParameterTypes == null) ? new Type[0] : model.ParameterTypes.ToArray();
                    MethodBuilder method = type.DefineMethod("M", MethodAttributes.Public, returnType, parameterTypes);
                    if (model.ParameterTypes != null)
                    {
                        for (int i = 0; i < model.ParameterTypes.Count; i++)
                        {
                            ParameterAttributes a = ParameterAttributes.None;
                            if (model.ParamAttributes != null)
                            {
                                a = model.ParamAttributes[i];
                            }

                            method.DefineParameter(i + 1, a, "p" + i);
                        }
                    }

                    if (model.ContainerNames != null)
                    {
                        foreach (string containerName in model.ContainerNames)
                        {
                            method.SetCustomAttribute(new CustomAttributeBuilder(attributeConstructor, new object[] { containerName }));
                        }
                    }

                    if (model.ReturnType != null && model.ReturnType != typeof(void))
                    {
                        if (model.IsValid)
                        {
                            //    .maxstack 4
                            //    .locals init (
                            //        [0] class [System.Core]System.Linq.Expressions.Expression`1<class [System.Core]System.Func`2<class AstoriaUnitTests.Stubs.Customer, bool>> CS$1$0000,
                            //        [1] class [System.Core]System.Linq.Expressions.ParameterExpression CS$0$0001,
                            //        [2] class [System.Core]System.Linq.Expressions.ParameterExpression[] CS$0$0002)
                            //    L_0000: nop 
                            //    L_0001: ldtoken AstoriaUnitTests.Stubs.Customer
                            //    L_0006: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
                            //    L_000b: ldstr "c"
                            //    L_0010: call class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type, string)
                            //    L_0015: stloc.1 
                            //    L_0016: ldc.i4.1 
                            //    L_0017: box bool
                            //    L_001c: ldtoken bool
                            //    L_0021: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
                            //    L_0026: call class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object, class [mscorlib]System.Type)
                            //    L_002b: ldc.i4.1 
                            //    L_002c: newarr [System.Core]System.Linq.Expressions.ParameterExpression
                            //    L_0031: stloc.2 
                            //    L_0032: ldloc.2 
                            //    L_0033: ldc.i4.0 
                            //    L_0034: ldloc.1 
                            //    L_0035: stelem.ref 
                            //    L_0036: ldloc.2 
                            //    L_0037: call class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [System.Core]System.Func`2<class AstoriaUnitTests.Stubs.Customer, bool>>(class [System.Core]System.Linq.Expressions.Expression, class [System.Core]System.Linq.Expressions.ParameterExpression[])
                            //    L_003c: stloc.0 
                            //    L_003d: br.s L_003f
                            //    L_003f: ldloc.0 
                            //    L_0040: ret
                            MethodInfo parameterMethod = typeof(Expression).GetMethod("Parameter", new Type[] { typeof(Type), typeof(string) });
                            MethodInfo constantMethod = typeof(Expression).GetMethod("Constant", new Type[] { typeof(object), typeof(Type) });
                            MethodInfo[] lambdas = typeof(Expression).GetMethods()
                                .Where((m) => m.Name == "Lambda" && m.IsGenericMethodDefinition && m.GetParameters()[1].ParameterType.IsArray)
                                .ToArray();
                            MethodInfo lambdaMethod = lambdas.Single().MakeGenericMethod(typeof(Func<Customer, bool>));
                            ILGenerator generator = method.GetILGenerator();
                            LocalBuilder l0 = generator.DeclareLocal(typeof(Expression<Func<Customer, bool>>));
                            LocalBuilder l1 = generator.DeclareLocal(typeof(ParameterExpression));
                            LocalBuilder l2 = generator.DeclareLocal(typeof(ParameterExpression[]));
                            
                            ReflectionUtility.GenerateLoadType(generator, typeof(Customer));
                            generator.Emit(OpCodes.Ldstr, "c");
                            generator.Emit(OpCodes.Call, parameterMethod);
                            generator.Emit(OpCodes.Stloc_1);
                            generator.Emit(OpCodes.Ldc_I4_1);
                            generator.Emit(OpCodes.Box, typeof(bool));
                            ReflectionUtility.GenerateLoadType(generator, typeof(bool));
                            generator.Emit(OpCodes.Call, constantMethod);
                            generator.Emit(OpCodes.Ldc_I4_1);
                            generator.Emit(OpCodes.Newarr, typeof(ParameterExpression));
                            generator.Emit(OpCodes.Stloc_2);
                            generator.Emit(OpCodes.Ldloc_2);
                            generator.Emit(OpCodes.Ldc_I4_0);
                            generator.Emit(OpCodes.Ldloc_1);
                            generator.Emit(OpCodes.Stelem_Ref);
                            generator.Emit(OpCodes.Ldloc_2);
                            generator.Emit(OpCodes.Call, lambdaMethod);
                            generator.Emit(OpCodes.Ret);
                        }
                        else
                        {
                            ReflectionUtility.GenerateReturnNull(method.GetILGenerator());
                        }
                    }
                    else
                    {
                        ReflectionUtility.GenerateReturnVoid(method.GetILGenerator());
                    }

                    modelIndex++;
                }

                modelIndex = 1;
                foreach (SignatureModel model in models)
                {
                    model.ServiceType = types[modelIndex - 1].CreateType();
                    modelIndex++;
                }

                return module.Assembly;
            }

            internal class SignatureModel
            {
                internal string ContainerName
                {
                    get
                    {
                        return this.ContainerNames[0];
                    }
                    set
                    {
                        this.ContainerNames = new List<string> { value };
                    }
                }

                internal string Attribute
                {
                    get { return (this.AttributeType == null) ? null : this.AttributeType.Name; }
                    set
                    {
                        switch (value.ToLowerInvariant())
                        {
                            case "query":
                                this.AttributeType = typeof(QueryInterceptorAttribute);
                                break;
                            case "change":
                                this.AttributeType = typeof(ChangeInterceptorAttribute);
                                break;
                            default:
                                throw new NotSupportedException();
                        }
                    }
                }

                internal Type AttributeType { get; set; }
                internal List<string> ContainerNames { get; set; }
                internal Type ReturnType { get; set; }
                internal List<ParameterAttributes> ParamAttributes { get; set; }
                internal List<Type> ParameterTypes { get; set; }
                internal bool IsValid { get; set; }
                internal Type ServiceType { get; set; }
            }

            [TestMethod, Variation]
            public void HiddenEntitySetsTest()
            {
                // Repro: $metadata  fails to show up for an Astoria Web service when authorization is used
                AstoriaUnitTests.Data.ServiceModelData.Northwind.EnsureDependenciesAvailable();
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("IsVisible", new object[] { false, true }));
                TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                {
                    TestUtil.ClearConfiguration();
                    bool isVisible = (bool)values["IsVisible"];
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        using (InitializationCallbackManager.RegisterStatic(delegate(object o, InitializationCallbackEventArgs e)
                        {
                            e.Configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
                            e.Configuration.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                            if (!isVisible)
                            {
                                e.Configuration.SetEntitySetAccessRule("Customers", EntitySetRights.None);
                            }
                        }))
                        {
                            request.ServiceType = typeof(WebDataServiceEdmCustomerServiceOperation);
                            request.RequestUriString = "/$metadata";
                            ServiceModelData.Northwind.EnsureDependenciesAvailable();
                            Exception exception = TestUtil.RunCatching(request.SendRequest);
                            TestUtil.AssertExceptionExpected(exception, !isVisible);
                            if (exception != null)
                            {
                                TestUtil.AssertExceptionStatusCode(exception, 500, "Internal Server Error expected.");
                            }
                        }
                    }
                });
            }

            [TestMethod, Variation]
            public void ContainerRightsTest()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("EdmBased", new object[] { true, false }),
                    new Dimension("SetTarget", new object[] { "Container", "*", "Foo", null }),
                    new Dimension("EntitySetRights",
                        TestUtil.ConcatObjectEnumerables(
                            new object[] { (EntitySetRights)(-1), EntitySetRights.All + 1 },
                            Enum.GetValues(typeof(EntitySetRights)))));

                TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                {
                    bool edmBased = (bool)values["EdmBased"];
                    string setTarget = (string)values["SetTarget"];
                    EntitySetRights rights = (EntitySetRights)values["EntitySetRights"];
                    bool rightsAreValid = rights >= 0 && rights <= EntitySetRights.All;

                    Type serviceType;
                    string containerName;
                    string containerTypeName;
                    string containerComplexTypeName;
                    string containerKey;
                    string referencingTypeName;
                    string referencingPropertyName;
                    
                    if (edmBased)
                    {
                        serviceType = typeof(WebDataServiceEdm);
                        containerName = "Orders";
                        containerTypeName = "NorthwindModel.Orders";
                        containerKey = "10248";
                        containerComplexTypeName = null;
                        referencingTypeName = "NorthwindModel.Customers";
                        referencingPropertyName = "Orders";

                        // Ensure there is a valid database to go query.
                        AstoriaUnitTests.Data.ServiceModelData.Northwind.EnsureDependenciesAvailable();
                    }
                    else
                    {
                        serviceType = typeof(WebDataServiceA);
                        containerName = "Orders";
                        containerTypeName = "AstoriaUnitTests.Stubs.Order";
                        containerKey = "1";
                        containerComplexTypeName = "AstoriaUnitTests.Stubs.CurrencyAmount";
                        referencingTypeName = "AstoriaUnitTests.Stubs.Customer";
                        referencingPropertyName = "Orders";
                    }

                    using (TestUtil.MetadataCacheCleaner())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.ServiceType = serviceType;
                        using (InitializationCallbackManager.RegisterStatic(delegate(object o, InitializationCallbackEventArgs e)
                        {
                            if (setTarget == "Container")
                            {
                                e.Configuration.SetEntitySetAccessRule(containerName, rights);
                            }
                            else
                            {
                                e.Configuration.SetEntitySetAccessRule(setTarget, rights);
                            }
                            
                            // Bring Customers back.
                            if (setTarget != "*")
                            {
                                e.Configuration.SetEntitySetAccessRule("Customers", EntitySetRights.All);
                            }
                        }))
                        {
                            TestUtil.ClearMetadataCache();
                            request.HttpMethod = "GET";
                            request.RequestUriString = "/" + containerName;
                            if (setTarget == "Foo" || setTarget == null || !rightsAreValid)
                            {
                                AssertInternalServerError(request, true);

                                // No need to try other URIs.
                                return;
                            }

                            bool isContainerHidden = (rights == EntitySetRights.None);
                            bool isEverythingHidden = (rights == EntitySetRights.None) && (setTarget == "*");
                            if (isContainerHidden)
                            {
                                AssertIsVisible(request, false);
                            }
                            else
                            {
                                AssertIsAuthorized(request,
                                    (0 != (rights & EntitySetRights.ReadMultiple)) &&
                                    (setTarget == "Container" || setTarget == "*" || setTarget == "Foo"));

                                request.RequestUriString = "/" + containerName + "(" + containerKey + ")";
                                AssertIsAuthorized(request,
                                    (0 != (rights & EntitySetRights.ReadSingle)) &&
                                    (setTarget == "Container" || setTarget == "*" || setTarget == "Foo"));
                            }

                            request.RequestUriString = "/$metadata";
                            request.SendRequest();

                            Stream stream = TestUtil.EnsureStreamWithSeek(request.GetResponseStream());
                            stream.Position = 0;
                            System.Diagnostics.Trace.WriteLine(new StreamReader(stream).ReadToEnd());
                            stream.Position = 0;
                            
                            var items = MetadataUtil.IsValidMetadata(stream, null);
                            var container = GetSingleContainer(items);
                            AstoriaTestLog.AreEqual(!isContainerHidden, 
                                container.FindEntitySet(containerName) != null, "EntitySet is removed from metadata when hidden.");
                            if (isEverythingHidden)
                            {
                                AstoriaTestLog.IsFalse(items.FindType(referencingTypeName) != null, "All containers removed when everything is hidden.");
                            }
                            else
                            {
                                IEdmEntityType type = items.FindType(referencingTypeName) as IEdmEntityType;
                                AstoriaTestLog.AreEqual(!isContainerHidden,
                                    type.FindProperty(referencingPropertyName) != null,
                                    "Referencing property is removed from metadata when hidden.");
                                AstoriaTestLog.AreEqual(!isContainerHidden, items.FindType(containerTypeName) != null,
                                    "Type is removed from metadata when hidden.");
                                if (containerComplexTypeName != null)
                                {
                                    AstoriaTestLog.AreEqual(!isContainerHidden, items.FindType(containerComplexTypeName) != null,
                                        "Associated complex type is removed from metadata when hidden.");
                                }
                            }
                        }
                    }
                });
            }

            [TestMethod, Variation]
            public void AccessingHiddenEntitySetShouldResultIn404()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.EntitySetAccessRule.Restore())
                {
                    OpenWebDataServiceHelper.EntitySetAccessRule.Value = new Dictionary<string, EntitySetRights>()
                    {
                        { "Customers", EntitySetRights.AllRead }
                    };

                    CustomRowBasedContext.PreserveChanges = false;

                    foreach (string reqUri in new[] { "/Regions", "/Regions(0)" })
                    {
                        RunErrorTest(reqUri, originalException =>
                        {
                            TestUtil.AssertExceptionStatusCode(originalException, 404, "Expected HTTP Error 404 - resource not found");
                            Exception dataServiceException = GetDataServiceException(originalException);

                            Assert.IsNotNull(dataServiceException, "DataServiceException exception expected but found" + originalException.ToString() + ".");
                            Assert.AreEqual("Resource not found for the segment 'Regions'.", dataServiceException.Message);
                        });
                    }
                }
            }

            [TestMethod, Variation]
            public void NavPropToHiddenEntitySetShouldLookLikeNormalMissingProperty()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.EntitySetAccessRule.Restore())
                {
                    OpenWebDataServiceHelper.EntitySetAccessRule.Value = new Dictionary<string, EntitySetRights>()
                    {
                        { "Customers", EntitySetRights.AllRead }
                    };

                    CustomRowBasedContext.PreserveChanges = false;

                    // First run a test to get the the status and and message that occurs normally
                    string errorStringFromMissingProperty = "";
                    int statusCodeFromMissingProperty = 0;

                    RunErrorTest("/Customers(0)/ThisPropDoesNotExist", originalException =>
                    {
                        statusCodeFromMissingProperty = (int)GetStatusCodeFromException(originalException);
                        Exception dataServiceException = GetDataServiceException(originalException);
                        errorStringFromMissingProperty = dataServiceException.Message;
                    });

                    // Ensure that if it's a nav prop with a hidden entity set we error in the same fashion
                    RunErrorTest("/Customers(0)/Region", originalException =>
                    {
                        TestUtil.AssertExceptionStatusCode(originalException, statusCodeFromMissingProperty, "We should have the same status code as a normal missing property.");
                        Exception dataServiceException = GetDataServiceException(originalException);

                        Assert.IsNotNull(dataServiceException, "DataServiceException exception expected but found" + originalException.ToString() + ".");
                        Assert.AreEqual(errorStringFromMissingProperty.Replace("ThisPropDoesNotExist", "Region"), dataServiceException.Message);
                    });
                }
            }

            private static void RunErrorTest(string reqUri, Action<Exception> verify)
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.ServiceType = typeof(OpenWebDataService<>).MakeGenericType(typeof(CustomRowBasedContext));
                    request.RequestUriString = reqUri;

                    Exception exception = TestUtil.RunCatching(delegate()
                    {
                        request.SendRequest();
                        using (var s = new StreamReader(request.GetResponseStream()))
                        {
                            s.ReadToEnd();
                        }
                    });

                    verify(exception);
                }
            }

            // BadRequest_InvalidPropertyNameSpecified(navigationProperty.Name, navigationProperty.DeclaringEntityType().FullName())

            [TestMethod, Variation]
            public void IDSP_VerifyRightsForResourceSet()
            {
                var testCases = new[]
                { 
                    new {
                        EntitySetRights = new Dictionary<string, EntitySetRights>()
                        {
                            { "*", EntitySetRights.AllRead}
                        },
                        ServiceOperationRights = new Dictionary<string, ServiceOperationRights>()
                        {
                            { "*", ServiceOperationRights.All}
                        },
                        ExpectedResults = new string[] 
                        { 
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityType[@Name = 'Customer'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityType[@Name = 'Customer']/edm:NavigationProperty[@Name = 'Region'])", 
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityType[@Name = 'Region'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:ComplexType[@Name = 'Address'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:ComplexType[@Name = 'Headquarter'])", 
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:EntitySet[@Name = 'Customers'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:EntitySet[@Name = 'Customers']/edm:NavigationPropertyBinding[@Path='Region' and @Target='Regions'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:EntitySet[@Name = 'Regions'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:EntitySet[@Name = 'MemberCustomers'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:EntitySet[@Name = 'MemberRegions'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:FunctionImport[@Name = 'GetRegionByName'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:FunctionImport[@Name = 'AddressServiceOperation'])",
                        },
                        Message = "All Resource Sets, Complex Types and Service Operations should be visible."
                    },
                    new {
                        EntitySetRights = new Dictionary<string, EntitySetRights>()
                        {
                            {"*", EntitySetRights.AllRead},
                            {"MemberCustomers", EntitySetRights.None},
                            {"MemberRegions", EntitySetRights.None},
                        },
                        ServiceOperationRights =new Dictionary<string, ServiceOperationRights>()
                        {
                            {"*", ServiceOperationRights.All},
                            {"GetRegionByName", ServiceOperationRights.None}
                        },
                        ExpectedResults = new string[] 
                        { 
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityType[@Name = 'Customer'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityType[@Name = 'Customer']/edm:NavigationProperty[@Name = 'Region'])", 
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityType[@Name = 'Region'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:ComplexType[@Name = 'Address'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:ComplexType[@Name = 'Headquarter'])", 
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:EntitySet[@Name = 'Customers'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:EntitySet[@Name = 'Customers']/edm:NavigationPropertyBinding[@Path='Region' and @Target='Regions'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:EntitySet[@Name = 'Regions'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:FunctionImport[@Name = 'AddressServiceOperation'])",
                        },
                        Message = "All Resource Types should be visible. 'MemberRegion' Resource Set and 'GetRegionByName' Service Operation are hidden and should not be visible."
                    },
                    new {
                        EntitySetRights = new Dictionary<string, EntitySetRights>()
                        {
                            {"*", EntitySetRights.AllRead},
                            {"Regions", EntitySetRights.None},
                            {"MemberRegions", EntitySetRights.None}
                        },
                        ServiceOperationRights =new Dictionary<string, ServiceOperationRights>()
                        {
                            { "*", ServiceOperationRights.All},
                            {"GetRegionByName", ServiceOperationRights.None}
                        },
                        ExpectedResults = new string[] 
                        { 
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityType/edm:NavigationProperty[@Name = 'Region'])", 
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityType[@Name = 'Region'])", 
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:ComplexType[@Name = 'Headquarter'])",
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:EntitySet[@Name = 'Customers']/edm:NavigationPropertyBinding[@Path='Region' and @Target='Regions'])",
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:EntitySet[@Name = 'Regions'])",
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:EntitySet[@Name = 'MemberRegions'])",
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:FunctionImport[@Name = 'GetRegionByName'])",
                        },
                        Message = "'Region' and Resource Type and 'GetRegionByName' Service Operation are hidden and should not be visible."
                    },
                    new {
                        EntitySetRights = new Dictionary<string, EntitySetRights>()
                        {
                            {"*", EntitySetRights.AllRead},
                            {"Customers", EntitySetRights.None},
                            {"MemberCustomers", EntitySetRights.None},
                            {"Regions", EntitySetRights.None},
                            {"MemberRegions", EntitySetRights.None}
                        },
                        ServiceOperationRights = new Dictionary<string, ServiceOperationRights>()
                        {
                            {"*", ServiceOperationRights.None}
                        },
                        ExpectedResults = new string[] 
                        { 
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityType[@Name = 'Customer'])",
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityType[@Name = 'Region'])", 
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:ComplexType[@Name = 'Address'])",
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:ComplexType[@Name = 'Headquarter'])",
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:EntitySet[@Name = 'Customers'])",
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:EntitySet[@Name = 'Regions'])",
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:FunctionImport[@Name = 'AddressServiceOperation'])",                           
                        },
                        Message = "'Address' Complex Type is not used by any visible Resource Type or Service Operation and should therefore be hidden."
                    },
                    new {
                        EntitySetRights = new Dictionary<string, EntitySetRights>()
                        {
                            {"*", EntitySetRights.AllRead},
                            {"Customers", EntitySetRights.None},
                            {"MemberCustomers", EntitySetRights.None},
                            {"Regions", EntitySetRights.None},
                            {"MemberRegions", EntitySetRights.None}
                        },
                        ServiceOperationRights = new Dictionary<string, ServiceOperationRights>()
                        {
                            {"*", ServiceOperationRights.None},
                            {"AddressServiceOperation", ServiceOperationRights.All}
                        },
                        ExpectedResults = new string[] 
                        { 
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityType[@Name = 'Customer'])",
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityType[@Name = 'Region'])",
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:ComplexType[@Name = 'Headquarter'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:ComplexType[@Name = 'Address'])",
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:EntitySet[@Name = 'Customers'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:FunctionImport[@Name = 'AddressServiceOperation'])",                           
                        },
                        Message = "'Address' Complex Type is used by 'AddressServiceOperation' Service Operation and should therefore be visible."
                    }, 
                    new {
                        EntitySetRights = new Dictionary<string, EntitySetRights>()
                        {
                            {"*", EntitySetRights.None},
                            {"Customers", EntitySetRights.AllRead}
                        },
                        ServiceOperationRights = new Dictionary<string, ServiceOperationRights>()
                        {
                            {"*", ServiceOperationRights.None},
                        },
                        ExpectedResults = new string[] 
                        { 
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityType[@Name = 'Customer'])",
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityType[@Name = 'Region'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:ComplexType[@Name = 'Address'])",
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:ComplexType[@Name = 'Headquarter'])",
                            "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:EntitySet[@Name = 'Customers'])",
                            "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:FunctionImport[@Name = 'AddressServiceOperation'])",                           
                        },
                        Message = "'Address' Complex Type is used by 'Customers' ResourceSet and should therefore be visible."
                    }                      
                };

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
                nsmgr.AddNamespace("edmx", "http://docs.oasis-open.org/odata/ns/edmx");
                nsmgr.AddNamespace("edm", "http://docs.oasis-open.org/odata/ns/edm");
                
                foreach (var testCase in testCases)
                {
                    CustomRowBasedContext.PreserveChanges = false;

                    using (OpenWebDataServiceHelper.EntitySetAccessRule.Restore())
                    using (OpenWebDataServiceHelper.ServiceOperationAccessRule.Restore())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        TestUtil.ClearConfiguration();
                        CustomRowBasedContext.ClearData();
                        OpenWebDataServiceHelper.EntitySetAccessRule.Value = testCase.EntitySetRights;
                        OpenWebDataServiceHelper.ServiceOperationAccessRule.Value = testCase.ServiceOperationRights;
                        request.ServiceType = typeof(OpenWebDataService<>).MakeGenericType(typeof(CustomRowBasedContext));
                        request.RequestUriString = "/$metadata";
                        request.HttpMethod = "GET";
                        request.SendRequest();

                        using (Stream stream = request.GetResponseStream())
                        {
                            XPathDocument xpathDoc = new XPathDocument(stream);
                            XPathNavigator nav = xpathDoc.CreateNavigator();

                            foreach (string xpathExpr in testCase.ExpectedResults)
                            {
                                Assert.IsTrue((bool)nav.Evaluate(xpathExpr, nsmgr), testCase.Message + " XPath: '" + xpathExpr + "'.");
                            }
                        }
                    }
                }
            }

            [TestMethod, Variation]
            public void IDSP_VerifyRightsForServiceOperation()
            {
                var testCases = new[]
                { 
                    new {
                        Permissions = new Dictionary<string, ServiceOperationRights>()
                        {
                            { "*", ServiceOperationRights.AllRead}
                        },
                        ExpectedResult = "boolean(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:FunctionImport[@Name='GetCustomerByCity'])"
                    },
                    new {
                        Permissions = new Dictionary<string, ServiceOperationRights>()
                        {
                            { "GetCustomerByCity", ServiceOperationRights.None},
                            { "*", ServiceOperationRights.All}
                        },
                        ExpectedResult = "not(/edmx:Edmx/edmx:DataServices/edm:Schema/edm:EntityContainer[@Name = 'CustomDataServiceProvider']/edm:FunctionImport[@Name='GetCustomerByCity'])"
                    }
                };

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
                nsmgr.AddNamespace("edmx", "http://docs.oasis-open.org/odata/ns/edmx");
                nsmgr.AddNamespace("edm", "http://docs.oasis-open.org/odata/ns/edm");


                foreach (var testCase in testCases)
                {
                    using (OpenWebDataServiceHelper.ServiceOperationAccessRule.Restore())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        TestUtil.ClearConfiguration();
                        OpenWebDataServiceHelper.ServiceOperationAccessRule.Value = testCase.Permissions;
                        CustomRowBasedContext.PreserveChanges = false;
                        request.ServiceType = typeof(OpenWebDataService<>).MakeGenericType(typeof(CustomRowBasedContext));
                        request.RequestUriString = "/$metadata";
                        request.HttpMethod = "GET";
                        request.SendRequest();

                        using (Stream stream = request.GetResponseStream())
                        {
                            XPathDocument xpathDoc = new XPathDocument(stream);
                            XPathNavigator nav = xpathDoc.CreateNavigator();

                            Assert.IsTrue((bool)nav.Evaluate(testCase.ExpectedResult, nsmgr));
                        }
                    }
                }
            }

            [TestMethod, Variation]
            public void IDSP_CallHiddenServiceOperation()
            {
                try
                {
                    using (OpenWebDataServiceHelper.ServiceOperationAccessRule.Restore())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        TestUtil.ClearMetadataCache();
                        OpenWebDataServiceHelper.ServiceOperationAccessRule.Value = new Dictionary<string, ServiceOperationRights>()
                                {
                                    { "GetRegionByName", ServiceOperationRights.None},
                                    { "*", ServiceOperationRights.All}
                                };

                        CustomRowBasedContext.PreserveChanges = false;
                        request.ServiceType = typeof(OpenWebDataService<>).MakeGenericType(typeof(CustomRowBasedContext));
                        request.RequestUriString = "/GetRegionByName?name='Region1'";
                        request.HttpMethod = "GET";
                        request.SendRequest();
                        Assert.Fail("Exception expected but not thrown");
                    }
                }
                catch (WebException ex)
                {
                    DataServiceException innerEx = ex.InnerException as DataServiceException;
                    if (innerEx == null || innerEx.StatusCode != 404 ||
                        innerEx.Message != "Resource not found for the segment 'GetRegionByName'.")
                    {
                        throw;
                    }
                }
            }
            
            [TestMethod, Variation]
            public void IDSP_TestServiceOperationReturingHiddenResourceType()
            {
                string[] testUris = {
                    "/GetRegionByName?name='Region1'",
                    "/$metadata"   
                };

                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    foreach (string testUri in testUris)
                    {
                        try
                        {
                            using (OpenWebDataServiceHelper.EntitySetAccessRule.Restore())
                            {
                                TestUtil.ClearConfiguration();
                                OpenWebDataServiceHelper.EntitySetAccessRule.Value = new Dictionary<string, EntitySetRights>()
                                {
                                    { "Regions", EntitySetRights.None},
                                    { "*", EntitySetRights.All}
                                };

                                CustomRowBasedContext.PreserveChanges = false;
                                request.ServiceType = typeof(OpenWebDataService<>).MakeGenericType(typeof(CustomRowBasedContext));
                                request.RequestUriString = testUri;
                                request.HttpMethod = "GET";
                                request.SendRequest();
                                Assert.Fail("Exception expected but not thrown");
                            }
                        }
                        catch (WebException ex)
                        {
                            InvalidOperationException innerEx = ex.InnerException as InvalidOperationException;
                            if (innerEx == null || innerEx.Message != "The operation 'GetRegionByName' has the resource set 'Regions' that is not visible. The operation 'GetRegionByName' should be made hidden or the resource set 'Regions' should be made visible.")
                                                                      
                            {
                                throw;
                            }
                        }
                    }
                }
            }

            #region HiddenTypesShouldNotImpactMetadataVersion

            public class TestEntityBase
            {
                public int ID { get; set; }
            }

            public class TestEntityWithCollection : TestEntityBase
            {
                public IEnumerable<int> IntCollection
                {
                    get { return new int[] { 1, 2, 3, }; }
                }
            }

            [NamedStream("Stream1")]
            public class TestEntityWithNamedStreams : TestEntityBase
            {
            }

            public class TestV1Entity : TestEntityBase
            {
            }

            public class TestService : OpenWebDataService<TestContext>
            {
                [WebGet]
                public IEnumerable<TestV1Entity> GetV1Set()
                {
                    return this.CurrentDataSource.V1Set;
                }

                [WebGet]
                public IEnumerable<TestEntityWithCollection> GetV3Set()
                {
                    return this.CurrentDataSource.V3Set;
                }
            }

            public class TestContext
            {
                public IQueryable<TestV1Entity> V1Set
                {
                    get
                    {
                        return new List<TestV1Entity>().AsQueryable();
                    }
                }

                public IQueryable<TestEntityWithCollection> V3Set
                {
                    get
                    {
                        return new List<TestEntityWithCollection>().AsQueryable();
                    }
                }

                // TestEntityWithNamedStreams is not reachable from any set and should not impact the metadata version
            }

            [TestMethod, Variation("'Collection' Server : Protocol version is expected to be 3 , even if Entity Set with collection properties is hidden on DataService")]
            public void TestHiddenTypesShouldNotImpactMetadataVersion()
            {
                TestUtil.RunCombinations(UnitTestsUtil.BooleanValues, UnitTestsUtil.BooleanValues, UnitTestsUtil.BooleanValues, UnitTestsUtil.BooleanValues,
                    (v1Set, v3Set, v1ServiceOp, v3ServiceOp) =>
                    {
                        using (TestUtil.MetadataCacheCleaner())
                        using (OpenWebDataServiceHelper.EntitySetAccessRule.Restore())
                        using (OpenWebDataServiceHelper.ServiceOperationAccessRule.Restore())
                        using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                        {
                            // If (v1Set &&  v3Set) is true, we don't need to set anything, the default of ("*", EntitySetRights.All) will be applied.
                            if (!(v1Set && v3Set))
                            {
                                OpenWebDataServiceHelper.EntitySetAccessRule.Value = new Dictionary<string, EntitySetRights>();
                                OpenWebDataServiceHelper.EntitySetAccessRule.Value.Add("V1Set", v1Set ? EntitySetRights.All : EntitySetRights.None);
                                OpenWebDataServiceHelper.EntitySetAccessRule.Value.Add("V3Set", v3Set ? EntitySetRights.All : EntitySetRights.None);
                            }

                            // If (v1ServiceOp && v3ServiceOp && v1Set && v3Set) is true, the default of ("*", ServiceOperationRights.All) will be applied.
                            if (!(v1ServiceOp && v3ServiceOp && v1Set && v3Set))
                            {
                                OpenWebDataServiceHelper.ServiceOperationAccessRule.Value = new Dictionary<string, ServiceOperationRights>();
                                OpenWebDataServiceHelper.ServiceOperationAccessRule.Value.Add("GetV1Set", v1Set && v1ServiceOp ? ServiceOperationRights.All : ServiceOperationRights.None);
                                OpenWebDataServiceHelper.ServiceOperationAccessRule.Value.Add("GetV3Set", v3Set && v3ServiceOp ? ServiceOperationRights.All : ServiceOperationRights.None);
                            }

                            request.ServiceType = typeof(TestService);
                            request.HttpMethod = "GET";
                            request.RequestUriString = "/$metadata";

                            request.SendRequest();

                            Assert.AreEqual("4.0;", request.ResponseVersion);

                            XDocument metadata = request.GetResponseStreamAsXDocument();
                            XElement edmx = metadata.Elements().First();
                            Assert.AreEqual("Edmx", edmx.Name.LocalName);

                            XElement dataServices = edmx.Elements().First();
                            Assert.AreEqual("DataServices", dataServices.Name.LocalName);

                            XElement schema = dataServices.Elements().First();
                            Assert.AreEqual("Schema", schema.Name.LocalName);

                            Assert.IsFalse(schema.Elements().Any(e => e.Name.LocalName == "EntityType" && e.Attribute("Name").Value == "UnitTestModule_AuthorizationTest_TestEntityWithNamedStreams"));
                            if (!(v1Set || v3Set))
                            {
                                Assert.IsFalse(schema.Elements().Any(e => e.Name.LocalName == "EntityType" && e.Attribute("Name").Value == "UnitTestModule_AuthorizationTest_TestEntityBase"));
                            }
                            else
                            {
                                Assert.IsNotNull(schema.Elements().Single(e => e.Name.LocalName == "EntityType" && e.Attribute("Name").Value == "UnitTestModule_AuthorizationTest_TestEntityBase"));
                            }

                            if (v3Set)
                            {
                                Assert.IsNotNull(schema.Elements().Single(e => e.Name.LocalName == "EntityType" && e.Attribute("Name").Value == "UnitTestModule_AuthorizationTest_TestEntityWithCollection"));
                                Assert.IsNotNull(schema.Elements().Single(e => e.Name.LocalName == "EntityContainer").Elements().Single(e => e.Name.LocalName == "EntitySet" && e.Attribute("Name").Value == "V3Set"));
                                if (v3ServiceOp)
                                {
                                    Assert.IsNotNull(schema.Elements().Single(e => e.Name.LocalName == "EntityContainer").Elements().Single(e => e.Name.LocalName == "FunctionImport" && e.Attribute("Name").Value == "GetV3Set"));
                                }
                                else
                                {
                                    Assert.IsFalse(schema.Elements().Single(e => e.Name.LocalName == "EntityContainer").Elements().Any(e => e.Name.LocalName == "FunctionImport" && e.Attribute("Name").Value == "GetV3Set"));
                                }
                            }
                            else
                            {
                                Assert.IsFalse(schema.Elements().Any(e => e.Name.LocalName == "EntityType" && e.Attribute("Name").Value == "UnitTestModule_AuthorizationTest_TestEntityWithCollection"));
                                Assert.IsFalse(schema.Elements().Single(e => e.Name.LocalName == "EntityContainer").Elements().Any(e => e.Name.LocalName == "EntitySet" && e.Attribute("Name").Value == "V3Set"));
                                Assert.IsFalse(schema.Elements().Single(e => e.Name.LocalName == "EntityContainer").Elements().Any(e => e.Name.LocalName == "FunctionImport" && e.Attribute("Name").Value == "GetV3Set"));
                            }

                            if (v1Set)
                            {
                                Assert.IsNotNull(schema.Elements().Single(e => e.Name.LocalName == "EntityType" && e.Attribute("Name").Value == "UnitTestModule_AuthorizationTest_TestV1Entity"));
                                Assert.IsNotNull(schema.Elements().Single(e => e.Name.LocalName == "EntityContainer").Elements().Single(e => e.Name.LocalName == "EntitySet" && e.Attribute("Name").Value == "V1Set"));
                                if (v1ServiceOp)
                                {
                                    Assert.IsNotNull(schema.Elements().Single(e => e.Name.LocalName == "EntityContainer").Elements().Single(e => e.Name.LocalName == "FunctionImport" && e.Attribute("Name").Value == "GetV1Set"));
                                }
                                else
                                {
                                    Assert.IsFalse(schema.Elements().Single(e => e.Name.LocalName == "EntityContainer").Elements().Any(e => e.Name.LocalName == "FunctionImport" && e.Attribute("Name").Value == "GetV1Set"));
                                }
                            }
                            else
                            {
                                Assert.IsFalse(schema.Elements().Any(e => e.Name.LocalName == "EntityType" && e.Attribute("Name").Value == "UnitTestModule_AuthorizationTest_TestV1Entity"));
                                Assert.IsFalse(schema.Elements().Single(e => e.Name.LocalName == "EntityContainer").Elements().Any(e => e.Name.LocalName == "EntitySet" && e.Attribute("Name").Value == "V1Set"));
                                Assert.IsFalse(schema.Elements().Single(e => e.Name.LocalName == "EntityContainer").Elements().Any(e => e.Name.LocalName == "FunctionImport" && e.Attribute("Name").Value == "GetV1Set"));
                            }
                        }
                    });
            }

            #endregion TestHiddenTypesShouldNotImpactMetadataVersion

            /// <summary>
            /// Find the "real" exception.
            /// </summary>
            private static Exception GetDataServiceException(Exception exception)
            {
                while (exception != null)
                {
                    if (exception is DataServiceException)
                    {
                        break;
                    }
                    exception = exception.InnerException;
                }

                return exception;
            }

            private static IEdmEntityContainer GetSingleContainer(IEdmModel model)
            {
                return model.EntityContainer;
            }

            private static HttpStatusCode GetStatusCodeFromRequest(TestWebRequest request)
            {
                TestUtil.CheckArgumentNotNull(request, "request");
                HttpStatusCode actualStatusCode;
                try
                {
                    request.SendRequest();
                    actualStatusCode = HttpStatusCode.OK;
                }
                catch (Exception webException)
                {
                    // Some of these will be thrown directly by us during initialization.
                    if (webException is ArgumentNullException ||
                        webException is ArgumentOutOfRangeException ||
                        webException is ArgumentException ||
                        webException is TargetInvocationException)
                    {
                        return HttpStatusCode.InternalServerError;
                    }

                    actualStatusCode = GetStatusCodeFromException(webException);
                }

                return actualStatusCode;
            }

            private void AssertInternalServerError(TestWebRequest request, bool expected)
            {
                HttpStatusCode actualStatusCode = GetStatusCodeFromRequest(request);

                if (expected)
                {
                    AstoriaTestLog.AreEqual(actualStatusCode, HttpStatusCode.InternalServerError,
                        "Checking internal server error for " + request.HttpMethod + " on URI '" + request.RequestUriString + "'");
                }
                else
                {
                    AstoriaTestLog.IsFalse(actualStatusCode == HttpStatusCode.InternalServerError,
                        "Checking internal server error for " + request.HttpMethod + " on URI '" + request.RequestUriString + "'");
                }
            }

            private void AssertIsAuthorized(TestWebRequest request, bool expected)
            {
                HttpStatusCode expectedStatusCode = expected ? HttpStatusCode.OK : HttpStatusCode.Forbidden;
                HttpStatusCode actualStatusCode = GetStatusCodeFromRequest(request);

                AstoriaTestLog.AreEqual(expectedStatusCode, actualStatusCode,
                    "Checking authorization for " + request.HttpMethod + " on URI '" + request.RequestUriString + "'");
            }

            private void AssertIsVisible(TestWebRequest request, bool expectedVisible)
            {
                Debug.Assert(request != null, "request != null");
                HttpStatusCode actualStatusCode = GetStatusCodeFromRequest(request);
                if (expectedVisible)
                {
                    AstoriaTestLog.AreEqual(actualStatusCode, HttpStatusCode.OK,
                        "Checking visibility for " + request.HttpMethod + " on URI '" + request.RequestUriString + "'");
                }
                else
                {
                    AstoriaTestLog.AreEqual(actualStatusCode, HttpStatusCode.NotFound,
                        "Checking visibility for " + request.HttpMethod + " on URI '" + request.RequestUriString + "'");
                }
            }

            private static HttpStatusCode GetStatusCodeFromException(Exception exception)
            {
                return TestUtil.GetStatusCodeFromException(exception);
            }

            #region Helper types.

            public class ComposeQueryEventArgs : EventArgs
            {
                public ComposeQueryEventArgs(Type elementType)
                {
                    this.ElementType = elementType;
                    this.Filter = Expression.Lambda(Expression.Constant(true), Expression.Parameter(elementType, "e"));
                }

                public Type ElementType { get; set; }
                public Expression Filter { get; set; }
            }

            public class InitializationCallbackEventArgs : EventArgs
            {
                public InitializationCallbackEventArgs(Type type, IDataServiceConfiguration configuration)
                {
                    this.Configuration = configuration;
                    this.Type = type;
                }

                public IDataServiceConfiguration Configuration { get; set; }
                public Type Type { get; set; }
            }

            public class UpdatedEventArgs : EventArgs
            {
                public UpdatedEventArgs(string methodName, object target, UpdateOperations action)
                {
                    this.MethodName = methodName;
                    this.Target = target;
                    this.Action = action;
                }

                public UpdateOperations Action { get; set; }
                public string MethodName { get; set; }
                public object Target { get; set; }
            }

            public class InitializationCallbackManager : StaticCallbackManager<InitializationCallbackEventArgs>
            {
                public static void NotifyInitializeService(Type type, IDataServiceConfiguration configuration)
                {
                    if (configuration == null)
                    {
                        throw new Exception();
                    }

                    FireEvent(type, new InitializationCallbackEventArgs(type, configuration));
                }
            }

            public class WebDataServiceNoInit0 : DataService<CustomDataContext>
            {
                public static void InitializeService()
                {
                    throw new Exception();
                }
            }

            public class WebDataServiceNoInit1 : DataService<CustomDataContext>
            {
                public static void InitializeServices(IDataServiceConfiguration configuration)
                {
                    throw new Exception();
                }
            }

            public class WebDataServiceNoInit2 : DataService<CustomDataContext>
            {
                public static void InitializeService(IDataServiceConfiguration configuration, object o)
                {
                    throw new Exception();
                }
            }

            public class WebDataServiceNoInit3 : DataService<CustomDataContext>
            {
                public static void InitializeService(ref IDataServiceConfiguration configuration)
                {
                    throw new Exception();
                }
            }

            public class WebDataServiceNoInit4 : DataService<CustomDataContext>
            {
                public static void InitializeService(out IDataServiceConfiguration configuration)
                {
                    throw new Exception();
                }
            }

            public class WebDataServiceInitCallback<T> : DataService<T>
            {
                public static void InitializeService(IDataServiceConfiguration configuration)
                {
                    InitializationCallbackManager.NotifyInitializeService(typeof(T), configuration);
                }
            }

            public class WebDataServiceA : DataService<CustomDataContext>
            {
                public static void InitializeService(DataServiceConfiguration configuration)
                {
                    configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                    InitializationCallbackManager.NotifyInitializeService(typeof(WebDataServiceA), configuration);
                }

                [QueryInterceptor("Customers")]
                public Expression<Func<Customer, bool>> CheckCustomers()
                {
                    ComposeQueryEventArgs args = new ComposeQueryEventArgs(typeof(Customer));
                    StaticCallbackManager<ComposeQueryEventArgs>.FireEvent(this, args);
                    return (Expression<Func<Customer, bool>>)args.Filter;
                }

                [ChangeInterceptor("Customers")]
                public void CheckCustomerUpdated(Customer c, UpdateOperations action)
                {
                    UpdatedEventArgs args = new UpdatedEventArgs("CheckCustomerUpdated", c, action);
                    StaticCallbackManager<UpdatedEventArgs>.FireEvent(this, args);
                }

                [ChangeInterceptor("Orders")]
                public void CheckOrderUpdated(object o, UpdateOperations action)
                {
                    UpdatedEventArgs args = new UpdatedEventArgs("CheckOrderUpdated", o, action);
                    StaticCallbackManager<UpdatedEventArgs>.FireEvent(this, args);
                }
            }

            public class WebDataServiceEdm : DataService<NorthwindModel.NorthwindContext>
            {
                public static void InitializeService(IDataServiceConfiguration configuration)
                {
                    InitializationCallbackManager.NotifyInitializeService(typeof(WebDataServiceEdm), configuration);
                }
            }

            public class WebDataServiceEdmCustomerCallback : WebDataServiceEdm
            {
                [QueryInterceptor("Customers")]
                public Expression<Func<NorthwindModel.Customers, bool>> CheckCustomers()
                {
                    ComposeQueryEventArgs args = new ComposeQueryEventArgs(typeof(Customer));
                    StaticCallbackManager<ComposeQueryEventArgs>.FireEvent(this, args);
                    return (Expression<Func<NorthwindModel.Customers, bool>>)args.Filter;
                }
            }

            public class WebDataServiceEdmCustomerServiceOperation : WebDataServiceEdm
            {
                [System.ServiceModel.Web.WebGet]
                public IEnumerable<NorthwindModel.Customers> CustomersOperation()
                {
                    return new NorthwindModel.Customers[0];
                }
            }

            public class WebDataServiceB : WebDataServiceA
            {
                public new static void InitializeService(DataServiceConfiguration configuration)
                {
                    configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                    InitializationCallbackManager.NotifyInitializeService(typeof(WebDataServiceB), configuration);
                }
            }

            #endregion Helper types.
        }
    }
}
