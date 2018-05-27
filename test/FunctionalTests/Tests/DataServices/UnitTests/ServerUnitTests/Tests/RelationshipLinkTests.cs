//---------------------------------------------------------------------
// <copyright file="RelationshipLinkTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region usings
    using System;
    using System.Linq;
    using System.Data.Test.Astoria;
    using System.Net;
    using System.Xml;
    using AstoriaUnitTests.Stubs;
    using Microsoft.OData.Client;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ocs = AstoriaUnitTests.ObjectContextStubs;
    using System.Xml.Linq;
    using Microsoft.OData.Service;

    #endregion

    public class RelationshipLinksService : DataService<CustomDataContext>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            config.DataServiceBehavior.IncludeAssociationLinksInResponse = true;
        }
    }

    [TestModule]
    public partial class UnitTestModule : AstoriaTestModule
    {
        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
        [Ignore] // Remove Atom
        // [TestClass]
        public class RelationshipLinkTests : AstoriaTestCase
        {
            [TestMethod]
            public void CheckRelationshipLinks()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.ServiceType = typeof(RelationshipLinksService);
                    request.StartService();

                    var requests = new string[]{
                                                "/Customers",
                                                "/Customers?$expand=Orders",
                                                "/Customers?$expand=BestFriend",
                                                "/Customers?$expand=Orders,BestFriend",
                                                "/Customers(0)",
                                                "/Customers(1)?$expand=Orders",
                                                "/Customers(1)?$expand=BestFriend",
                                                "/Customers(1)?$expand=Orders,BestFriend",
                                                "/Customers(1)?$expand=Orders($expand=OrderDetails),BestFriend",
                                                "/Customers(1)?$expand=BestFriend($expand=Orders)",
                                                "/Customers(1)/BestFriend",
                                                "/Customers(1)/BestFriend?$expand=BestFriend",
                                                "/Customers(1)/BestFriend?$expand=Orders",
                                                "/Customers(1)/BestFriend?$expand=BestFriend($expand=Orders($expand=OrderDetails))",
                                                "/Customers(1)/BestFriend?$expand=Orders($expand=OrderDetails)",
                                                "/Orders?$expand=OrderDetails,Customer,Customer($expand=BestFriend)",
                                                "/Customers?$select=Name",
                                                "/Customers?$select=Name,Orders",
                                                "/Customers?$select=Name,BestFriend",
                                                "/Customers?$select=Name,Orders&$expand=Orders",
                                                "/Customers?$select=Name,Orders&$expand=BestFriend",
                                                "/Customers(1)?$select=Name",
                                                "/Customers(1)?$select=Name,Orders",
                                                "/Customers(1)?$select=Name,BestFriend",
                                                "/Customers(1)?$select=Name,Orders&$expand=Orders",
                                                "/Customers(1)?$select=Name,Orders&$expand=BestFriend",
                                                "/Customers(1)?$select=Name&$expand=Orders($select=Customer)",
                                                "/Customers(1)?$select=Name&$expand=Orders($select=Customer,OrderDetails)",
                                               };


                    TestUtil.RunCombinations(UnitTestsUtil.ResponseFormats, requests, (format, requestUri) =>
                    {
                        XDocument response = UnitTestsUtil.GetResponseAsAtomXLinq(request, requestUri, format);
                        Assert.AreEqual("4.0;", request.ResponseVersion);

                        var entries = response.Descendants(UnitTestsUtil.AtomNamespace + "entry");
                        foreach(var entry in entries)
                        {
                            var links = entry.Elements(UnitTestsUtil.AtomNamespace + "link");
                            
                            // for each entry there must be only 1 edit link"
                            var editLinks = (from link in links 
                                           where link.Attribute("rel").Value == "edit"
                                           select link.Attribute("href").Value).ToList();
                            
                            Assert.IsTrue(editLinks.Count == 1, "for each entry there must be only 1 edit link");
                            var editLink = editLinks[0];

                            var navigationLinks = (from link in links
                                                   where link.Attribute("rel").Value.StartsWith(UnitTestsUtil.NavigationLinkNamespace.NamespaceName)
                                                   select link).ToDictionary((l) => { return l.Attribute("title").Value; });
                            
                            var relationshipLinks = (from link in links
                                                     where link.Attribute("rel").Value.StartsWith(UnitTestsUtil.RelationshipLinkNamespace.NamespaceName)
                                                     select link).ToDictionary((l) => { return l.Attribute("title").Value; });

                            // for projections, we should only write out property links if it's selected
                            // TODO: fix server relationship links test.
                            // TODO: this is broken because select doesn't contain what it used to... need
                            // TODO: rework this, but its not high priority because its a server test.
                            //int index = requestUri.IndexOf("$select");
                            //if (index > 0)
                            //{
                            //    string selectClause = requestUri.Substring(index + 8);
                            //    index = selectClause.IndexOf("&");
                            //    if (index > 0)
                            //    {
                            //        selectClause = selectClause.Substring(0, index);
                            //    }

                            //    var selectedProperties = selectClause.Split(',');
                            //    string entryType = entry.Elements(UnitTestsUtil.AtomNamespace + "category").Single().Attribute("term").Value;
                            //    if (entryType.Equals("AstoriaUnitTests.Stubs.Customer", StringComparison.Ordinal))
                            //    {
                            //        Assert.AreEqual(selectedProperties.Intersect(new string[] { "Orders", "BestFriend" }, StringComparer.Ordinal).Count() * 2, relationshipLinks.Count);
                            //    }
                            //    else if (entryType.Equals("AstoriaUnitTests.Stubs.Order", StringComparison.Ordinal))
                            //    {
                            //        if (selectClause.Contains("Orders/"))
                            //        {
                            //            Assert.AreEqual(selectedProperties.Intersect(new string[] { "Orders/Customer", "Orders/OrderDetails" }, StringComparer.Ordinal).Count(), relationshipLinks.Count);
                            //        }
                            //    }
                            //}

                            if (format != UnitTestsUtil.JsonLightMimeType)
                            {
                                // json format does not write out nav links if the 1-1 property is null, atom does.
                                Assert.AreEqual(navigationLinks.Count, relationshipLinks.Count, "there should be equal number of relationship and navigation links");
                            }

                            foreach (var navigationLink in navigationLinks.Values)
                            {
                                VerifyNavigationLink(navigationLink, editLink);
                                XElement relationshipLink = null;
                                string propertyName = navigationLink.Attribute("title").Value;
                                if (relationshipLinks.TryGetValue(propertyName, out relationshipLink))
                                {
                                    VerifyRelationshipLink(relationshipLink, editLink);
                                }
                                else
                                {
                                    Assert.Fail("Relationship link is missing for navigation property: " + propertyName);
                                }
                            }
                        }
                    });
               };
           }

            [TestMethod]
            public void CheckRelationshipLinkForDeleteOperationForEntityCollection()
            {
                // CreateChangeScope will make sure that changes are preserved after every SendRequest.
                // By default the data source is created for every request
                using (UnitTestsUtil.CreateChangeScope(typeof(CustomDataContext)))
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                    {
                        request.ServiceType = typeof(RelationshipLinksService);

                        request.RequestUriString = "/Customers(1)/Orders/$ref";
                        request.HttpMethod = "GET";
                        request.SendRequest();
                        var response1 = request.GetResponseStreamAsText();

                        request.RequestUriString = "/Customers(1)/Orders/$ref?$id=Orders(1)";
                        request.HttpMethod = "DELETE";
                        request.SendRequest();

                        var response = request.GetResponseStreamAsText();
                        Assert.IsTrue(response != null);
                        Assert.IsTrue(request.ResponseStatusCode == 204);

                        request.RequestUriString = "/Customers(1)/Orders/$ref";
                        request.HttpMethod = "GET";
                        request.SendRequest();
                        var response2 = request.GetResponseStreamAsText();

                        Assert.IsTrue(response1 != response2);
                    }
                }
            }

            [TestMethod]
            public void CheckRelationshipLinkForDeleteOperationForSingleEntity()
            {
                // CreateChangeScope will make sure that changes are preserved after every SendRequest.
                // By default the data source is created for every request
                using (UnitTestsUtil.CreateChangeScope(typeof(CustomDataContext)))
                {
                    using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                    {
                        request.ServiceType = typeof(RelationshipLinksService);

                        request.RequestUriString = "/Customers(1)/BestFriend/$ref";
                        request.HttpMethod = "GET";
                        request.SendRequest();
                        var response1 = request.GetResponseStreamAsText();

                        request.RequestUriString = "/Customers(1)/BestFriend/$ref";
                        request.HttpMethod = "DELETE";
                        request.SendRequest();
                        var response = request.GetResponseStreamAsText();
                        Assert.IsTrue(response != null);
                        Assert.IsTrue(request.ResponseStatusCode == 204);

                        request.RequestUriString = "/Customers(1)/BestFriend/$ref";
                        request.HttpMethod = "GET";
                        try
                        {
                            request.SendRequest();
                        }
                        catch (WebException)
                        {
                            Assert.IsTrue(request.ResponseStatusCode == 404);
                        }
                        var response2 = request.GetResponseStreamAsText();

                        Assert.IsTrue(response1 != response2);
                    }
                }
            }

            [TestMethod]
            public void DeleteOperationOnEntityReferenceShouldNotExpectAnyQeuryParmeter()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.ServiceType = typeof(RelationshipLinksService);

                    request.RequestUriString = "/Customers(1)/BestFriend/$ref?$id=1234";
                    request.HttpMethod = "DELETE";

                    try
                    {
                        request.SendRequest();
                    }
                    catch (WebException ex)
                    {
                        Assert.IsTrue(ex.Status == WebExceptionStatus.ProtocolError);
                    }

                    var response = request.GetResponseStreamAsText();
                    Assert.IsTrue(response != null);
                    Assert.IsTrue(response.Contains("$id or any other query option must not be specified for DELETE operation on a single entity reference."));
                    Assert.IsTrue(request.ResponseStatusCode == 400);
                }
            }

            [TestMethod]
            public void DeleteOpeationOnEntityReferenceShouldNotExpectMoreThanOneQueryParameters()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.ServiceType = typeof(RelationshipLinksService);

                    request.RequestUriString = "/Customers(1)/Orders/$ref?$id=Orders(1)&f00=bar";
                    request.HttpMethod = "DELETE";

                    try
                    {
                        request.SendRequest();
                    }
                    catch (WebException ex)
                    {
                        Assert.IsTrue(ex.Status == WebExceptionStatus.ProtocolError);
                    }

                    var response = request.GetResponseStreamAsText();
                    Assert.IsTrue(response != null);
                    Assert.IsTrue(response.Contains("The URI must refer to a single resource from an entity set or from a property referring to a set of resources."));
                    Assert.IsTrue(request.ResponseStatusCode == 400);
                }
            }

            [TestMethod]
            public void DeleteOpeationOnEntityReferencesRequireIdQueryParameter()
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    request.ServiceType = typeof(RelationshipLinksService);

                    request.RequestUriString = "/Customers(1)/Orders/$ref?f00=bar";
                    request.HttpMethod = "DELETE";

                    try
                    {
                        request.SendRequest();
                    }
                    catch (WebException ex)
                    {
                        Assert.IsTrue(ex.Status == WebExceptionStatus.ProtocolError);
                    }

                    var response = request.GetResponseStreamAsText();
                    Assert.IsTrue(response != null);
                    Assert.IsTrue(response.Contains("$id query option must be specified for DELETE operation on a collection of entity references."));
                    Assert.IsTrue(request.ResponseStatusCode == 400);
                }
            }

           private void VerifyRelationshipLink(XElement relationshipLink, string editLink)
           {
               string rel = relationshipLink.Attribute("rel").Value;
               string title = relationshipLink.Attribute("title").Value;
               string type = relationshipLink.Attribute("type").Value;
               string href = relationshipLink.Attribute("href").Value;
               Assert.AreEqual(UnitTestsUtil.RelationshipLinkNamespace.NamespaceName+ title, rel, "rel attribute on relationship link is incorrect");
               Assert.AreEqual("application/xml", type, "type attribute on relationship link must be application/xml");
               Assert.AreEqual(editLink + "/" + title + "/$ref", href, "href attribute on relationship link is incorrect");
           }

           private void VerifyNavigationLink(XElement navigationLink, string editLink)
           {
               string rel = navigationLink.Attribute("rel").Value;
               string title = navigationLink.Attribute("title").Value;
               string href = navigationLink.Attribute("href").Value;
               Assert.AreEqual(UnitTestsUtil.NavigationLinkNamespace.NamespaceName + title, rel, "rel attribute on navigation link is incorrect");
               Assert.AreEqual(editLink + "/" + title, href, "href attribute on navigation link is incorrect");
           }
        }
    }
}
