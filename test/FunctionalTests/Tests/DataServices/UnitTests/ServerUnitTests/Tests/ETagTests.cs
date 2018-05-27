//---------------------------------------------------------------------
// <copyright file="ETagTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Test.Astoria;
    using System.IO;
    using System.Net;
    using AstoriaUnitTests.Stubs;
    using Microsoft.OData.Client;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ocs = AstoriaUnitTests.ObjectContextStubs;
    using Microsoft.OData.Service;
    using System.ServiceModel.Web;
    
    [TestModule]
    public partial class UnitTestModule : AstoriaTestModule
    {
        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
        /// <summary>This is a test class for etags test</summary>
        [TestClass, TestCase]
        public class ETagTests : AstoriaTestCase
        {
            #region GET tests
            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void GetTopLevelEntitySet()
            {
                string[] jsonXPaths = new string[] {
                    String.Format("count({0}/{1}/{2}/{3}/odata.etag)=3", JsonValidator.ObjectString, JsonValidator.ValueString.ToLowerInvariant(), JsonValidator.ArrayString, JsonValidator.ObjectString)
                };

                string[] atomXPaths = new string[] {
                    "count(/atom:feed/atom:entry/@adsm:etag)=3"
                };

                GetResponseFromReflectionBasedProvider("/Customers", UnitTestsUtil.JsonLightMimeTypeFullMetadata, jsonXPaths);
                GetResponseFromReflectionBasedProvider("/Customers", UnitTestsUtil.AtomFormat, atomXPaths);
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void CannotPassETagInGetTopLevelEntitySet()
            {
                var ifMatch = new KeyValuePair<string, string>("If-Match", "W/\"foo\"");
                var ifNoneMatch = new KeyValuePair<string, string>("If-None-Match", "W/\"foo\"");

                VerifyInvalidRequestForAllProviders("/Customers", 400, ifMatch);

                VerifyInvalidRequestForAllProviders("/Customers", 400, ifNoneMatch);

                VerifyInvalidRequestForAllProviders("/Customers", 400, ifMatch, ifNoneMatch);
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void GetSingleEntityFromTopLevelEntitySet()
            {
                string[] jsonXPaths = new string[] {
                    String.Format("/{0}[odata.id='http://host/Customers(0)' and odata.type='#{1}']", JsonValidator.ObjectString, typeof(Customer).FullName)
                };

                string[] atomXPaths = new string[] {
                    "/atom:entry[atom:category/@term='#" + typeof(CustomerWithBirthday).FullName + "' and atom:id='http://host/Customers(1)']"
                };

                GetResponseWithETagFromReflectionBasedProvider("/Customers(0)", UnitTestsUtil.JsonLightMimeTypeFullMetadata, jsonXPaths, true/*verifyETagScenarios*/);
                GetResponseWithETagFromReflectionBasedProvider("/Customers(1)", UnitTestsUtil.AtomFormat, atomXPaths, true/*verifyETagScenarios*/);
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void GetComplexProperty()
            {
                string[] jsonXPaths = new string[] {
                    String.Format("/{0}[odata.type='#{1}']", JsonValidator.ObjectString, typeof(Address).FullName)
                };

                string[] atomXPaths = new string[] {
                    "/adsm:value[@adsm:type='#" + typeof(Address).FullName + "']"
                };

                GetResponseWithETagFromReflectionBasedProvider("/Customers(0)/Address", UnitTestsUtil.JsonLightMimeTypeFullMetadata, jsonXPaths, true/*verifyETagScenarios*/);
                GetResponseWithETagFromReflectionBasedProvider("/Customers(1)/Address", UnitTestsUtil.MimeApplicationXml, atomXPaths, true/*verifyETagScenarios*/);
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void GetPrimitiveProperty()
            {
                string[] jsonXPaths = new string[] {
                    String.Format("/{0}[value='Customer 0']", JsonValidator.ObjectString)
                };

                string[] atomXPaths = new string[] {
                    "/adsm:value[text()='Customer 1']"
                };

                GetResponseWithETagFromReflectionBasedProvider("/Customers(0)/Name", UnitTestsUtil.JsonLightMimeTypeFullMetadata, jsonXPaths, true/*verifyETagScenarios*/);
                GetResponseWithETagFromReflectionBasedProvider("/Customers(1)/Name", UnitTestsUtil.MimeApplicationXml, atomXPaths, true/*verifyETagScenarios*/);
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void GetResourceFromReferenceNavigationProperty()
            {
                string[] jsonXPaths = new string[] {
                    String.Format("/{0}[odata.id='http://host/Customers(0)' and odata.type='#{1}']", JsonValidator.ObjectString, typeof(Customer).FullName)
                };

                string[] atomXPaths = new string[] {
                    "/atom:entry[atom:category/@term='#" + typeof(Customer).FullName + "' and atom:id='http://host/Customers(0)']"
                };

                GetResponseWithETagFromReflectionBasedProvider("/Customers(1)/BestFriend", UnitTestsUtil.JsonLightMimeTypeFullMetadata, jsonXPaths, true/*verifyETagScenarios*/);
                GetResponseWithETagFromReflectionBasedProvider("/Customers(1)/BestFriend", UnitTestsUtil.AtomFormat, atomXPaths, true/*verifyETagScenarios*/);
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void GetNullValuedResourceFromReferenceNavigationProperty()
            {
                TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.JsonLightMimeType, "/Customers(0)/BestFriend", typeof(CustomDataContext), null, "GET");
                Assert.IsTrue(request.ResponseStatusCode == 204);
                VerifyEmptyStream(request.GetResponseStream());
                request = UnitTestsUtil.GetTestWebRequestInstance(UnitTestsUtil.AtomFormat, "/Customers(0)/BestFriend", typeof(CustomDataContext), null, "GET");
                Assert.IsTrue(request.ResponseStatusCode == 204);
                VerifyEmptyStream(request.GetResponseStream());
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void GetSingleEntityFromTopLevelEntitySetWithoutETagProperties()
            {
                string[] jsonXPaths = new string[] {
                    String.Format("/{0}[odata.id='http://host/Orders(0)' and odata.type='#{1}']", JsonValidator.ObjectString, typeof(Order).FullName)
                };

                string[] atomXPaths = new string[] {
                    "/atom:entry[atom:category/@term='#" + typeof(Order).FullName + "' and atom:id='http://host/Orders(0)']"
                };

                GetResponseWithETagFromReflectionBasedProvider("/Orders(0)", UnitTestsUtil.JsonLightMimeTypeFullMetadata, jsonXPaths, false/*verifyETagScenarios*/);
                GetResponseWithETagFromReflectionBasedProvider("/Orders(0)", UnitTestsUtil.AtomFormat, atomXPaths, false/*verifyETagScenarios*/);
            }

            [TestMethod, Variation]
            public void GetNullEntityWithAnyValueForETag()
            {
                GetResponseWithETagFromReflectionBasedProvider("/Customers(0)/BestFriend", UnitTestsUtil.JsonLightMimeType, new string[0], false, true);
            }

            public class NotModifiedHeadersBug : DataService<CustomDataContext>
            {
                public static void InitializeService(DataServiceConfiguration config)
                {
                    config.SetEntitySetAccessRule("*", EntitySetRights.All);
                    config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                }

                protected override void OnStartProcessingRequest(ProcessRequestArgs args)
                {
                    base.OnStartProcessingRequest(args);
                    if (args.RequestUri.AbsoluteUri.EndsWith("Customers(1)/Name"))
                    {
                        args.OperationContext.ResponseHeaders.Add("MyCustomHeader", "MyCustomValue");
                    }
                }
            }

            [Ignore] // Remove Atom
            // [TestMethod]
            public void Validate304HasNoHeaders()
            {
                var serviceUri = new Uri("http://" + Environment.MachineName + ":" + NetworkUtil.GetRandomPortNumber() + "/" + System.Guid.NewGuid() + "/Service/");
                using (WebServiceHost host = new WebServiceHost(typeof(NotModifiedHeadersBug), serviceUri))
                {
                    host.Open();
                    try
                    {
                        HttpWebRequest setCustomerNameToNullRequest = CreateRequest(serviceUri, "Customers(1)/Name/$value", "DELETE", UnitTestsUtil.AtomFormat, Tuple.Create<string, string>("If-Match", "*"));
                      
                        HttpWebResponse setCustomerNameToNullResponse = (HttpWebResponse)setCustomerNameToNullRequest.GetResponse();
                        Assert.AreEqual(HttpStatusCode.NoContent, setCustomerNameToNullResponse.StatusCode);
                        string etag = setCustomerNameToNullResponse.Headers["ETag"];
                        HttpWebRequest getCustomerNameRequest = CreateRequest(serviceUri, "Customers(1)/Name", "GET", UnitTestsUtil.AtomFormat, Tuple.Create<string, string>("If-None-Match", "*"));
                        try
                        {
                            getCustomerNameRequest.GetResponse();
                            throw new InvalidOperationException("Should have failed with 304");
                        }
                        catch (WebException exc)
                        {
                            var response = ((HttpWebResponse)exc.Response);
                            Assert.AreEqual(HttpStatusCode.NotModified, response.StatusCode);
                            Assert.AreEqual(6, response.Headers.Count);
                            Assert.AreEqual("0", response.Headers["Content-Length"]);
                            Assert.AreEqual("nosniff", response.Headers["X-Content-Type-Options"]);
                            Assert.AreEqual("Microsoft-HTTPAPI/2.0", response.Headers["Server"]);
                            Assert.AreEqual("MyCustomValue", response.Headers["MyCustomHeader"]);
                            Assert.IsTrue(response.Headers["ETag"] != null);
                            Assert.IsTrue(response.Headers["Date"] != null);
                        }
                    }
                    finally
                    {
                        host.Close();
                    }
                }
            }

            [TestMethod, Variation]
            public void GetNullPrimitiveValueWithAnyValueForETag()
            {
                using (CustomDataContext.CreateChangeScope())
                {
                    Type contextType = typeof(CustomDataContext);

                    var ifMatchHeader = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("If-Match", "*") };

                    // Set the name property to null
                    UnitTestsUtil.SendRequestAndVerifyXPath(null, "/Customers(1)/Name/$value", null, contextType, UnitTestsUtil.JsonLightMimeType, "DELETE", ifMatchHeader, false);

                    // Both should work fine since the etag is of Customers(1)
                    UnitTestsUtil.SendRequestAndVerifyXPath(null, "/Customers(1)/Name", null, contextType, UnitTestsUtil.JsonLightMimeType, "GET", ifMatchHeader, true);
                    VerifyStatusCode("/Customers(1)/Name", UnitTestsUtil.JsonLightMimeType, "*", contextType, (int)HttpStatusCode.NotModified);

                    // This should return 404
                    UnitTestsUtil.VerifyInvalidRequest(null, "/Customers(1)/Name/$value", contextType, null, "GET", (int)HttpStatusCode.NotFound, ifMatchHeader);
                    VerifyStatusCode("/Customers(1)/Name/$value", UnitTestsUtil.MimeTextPlain, "*", contextType, (int)HttpStatusCode.NotModified);
                }
            }

            [TestMethod, Variation]
            public void DeleteUsingAnyValueForIfMatch()
            {
                using (CustomDataContext.CreateChangeScope())
                {
                    Type contextType = typeof(CustomDataContext);
                    var ifMatchHeader = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("If-Match", "*") };

                    // Directly deleting a top level entity
                    UnitTestsUtil.SendRequestAndVerifyXPath(null, "/Customers(0)", null, contextType, null, "DELETE", ifMatchHeader, false);

                    // Deleting a deep entity
                    UnitTestsUtil.SendRequestAndVerifyXPath(null, "/Customers(2)/BestFriend", null, contextType, null, "DELETE", ifMatchHeader, false);
                }
            }

            #endregion // GET tests

            #region POST tests
            [TestMethod, Variation]
            public void ETag_ErrorOnPostToTopLevelEntityWithIfMatchHeader()
            {
                var headerValues = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("If-Match", "W/\"sdhdfksdjsdf\"") };
                string jsonPayload = " { ID: 112 }";
                VerifyInvalidRequest(typeof(CustomDataContext), UnitTestsUtil.JsonLightMimeType, "/Orders", 400, headerValues, "POST", jsonPayload);
            }

            [TestMethod, Variation]
            public void ETag_ErrorOnPostToTopLevelEntityWithIfNoneMatchHeader()
            {
                var headerValues = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("If-None-Match", "W/\"sdhdfksdjsdf\"") };
                string jsonPayload = " { ID: 112 }";
                VerifyInvalidRequest(typeof(CustomDataContext), UnitTestsUtil.JsonLightMimeType, "/Orders", 400, headerValues, "POST", jsonPayload);
            }

            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void ETag_VerifyETagInPayloadInTopLevelPost()
            {
                string jsonPayload = "{ " +
                                         "@odata.type : \"AstoriaUnitTests.Stubs.Customer\" ," +
                                         "Name: \"Bar\"," +
                                         "ID: 125" +
                                     "}";

                string[] jsonXPath = new string[] { String.Format("count(/{0}/odata.etag)=1", JsonValidator.ObjectString) };

                string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                       AtomUpdatePayloadBuilder.GetCategoryXml(typeof(Customer).FullName) +
                       "<content type=\"application/xml\">" +
                         "<adsm:properties>" +
                           "<ads:Name>Foo</ads:Name>" +
                           "<ads:ID>125</ads:ID>" +
                         "</adsm:properties>" +
                       "</content>" +
                   "</entry>";

                string[] atomXPaths = new string[] { "count(/atom:entry/@adsm:etag)=1" };
                using (CustomDataContext.CreateChangeScope())
                {
                    GetResponse("/Customers", UnitTestsUtil.JsonLightMimeType, typeof(CustomDataContext), jsonXPath, null, "POST", jsonPayload);
                }

                using (CustomDataContext.CreateChangeScope())
                {
                    GetResponse("/Customers", UnitTestsUtil.AtomFormat, typeof(CustomDataContext), atomXPaths, null, "POST", atomPayload);
                }
            }

            #endregion //POST tests

            #region DELETE Tests

            [TestMethod, Variation]
            public void ETag_ErrorOnDeleteWithIfNoneMatchHeader()
            {
                var headerValues = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("If-None-Match", "W/\"sdhdfksdjsdf\"") };
                VerifyInvalidRequest(typeof(CustomDataContext), UnitTestsUtil.JsonLightMimeType, "/Customers(1)", 400, headerValues, "DELETE", null);
                VerifyInvalidRequest(typeof(CustomDataContext), UnitTestsUtil.AtomFormat, "/Customers(1)/BestFriend", 400, headerValues, "DELETE", null);
                VerifyInvalidRequest(typeof(CustomDataContext), UnitTestsUtil.JsonLightMimeType, "/Customers(1)/Orders(1)", 400, headerValues, "DELETE", null);
                VerifyInvalidRequest(typeof(CustomDataContext), UnitTestsUtil.AtomFormat, "/Customers(1)/Name/$value", 400, headerValues, "DELETE", null);
            }

            #endregion //DELETE Tests

            #region PUT/PATCH Tests
            // [Astoria-ODataLib-Integration] WCF DS Server doesn't check ETags if an ATOM payload entry has no content and no links (and it's a V1 entry)
            // We decided to break the old behavior and always check the ETag.
            [Ignore] // Remove Atom
            // [TestMethod, Variation]
            public void ETag_PUTPATCHResourceWithNoContent()
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("Type", new Type[] { typeof(Customer) }),
                    new Dimension("Method", new string[] { "PUT", "PATCH" }),
                    new Dimension("ProcessResponse", new bool[] { true, false }));

                using (CustomDataContext.CreateChangeScope())
                using (TestUtil.RestoreStaticValueOnDispose(typeof(BaseTestWebRequest), "HostInterfaceType"))
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    BaseTestWebRequest.HostInterfaceType = typeof(Microsoft.OData.Service.IDataServiceHost2);

                    TestUtil.RunCombinatorialEngineFail(engine, values =>
                    {
                        Type type = (Type)values["Type"];
                        string method = (string)values["Method"];
                        bool processResponse = (bool)values["ProcessResponse"];

                        string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                        "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                           AtomUpdatePayloadBuilder.GetCategoryXml(type.FullName) +
                        "</entry>";

                        request.RequestUriString = "/Customers(0)";
                        request.HttpMethod = method;
                        request.IfMatch = "W/\"sdfght\""; // Wrong ETag
                        request.RequestVersion = "4.0;";
                        if (processResponse)
                        {
                            request.RequestHeaders["Prefer"] = "return=representation";
                        }
                        else
                        {
                            request.RequestHeaders["Prefer"] = "return=minimal";
                        }

                        request.SetRequestStreamAsText(atomPayload);
                        request.RequestContentType = UnitTestsUtil.AtomFormat;
                        Exception e = TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNotNull(e, "The request should have failed.");
                        Assert.AreEqual(412, request.ResponseStatusCode, "The request should have failed due to mismatch in ETags");
                    });
                }
            }
            #endregion

            private static HttpWebRequest CreateRequest(Uri serviceUri, string url, string method, string contentType, params Tuple<string, string>[] headers)
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(serviceUri.AbsoluteUri + url);
                request.Method = method;
                request.ContentType = contentType;

                foreach (var header in headers)
                {
                    request.Headers[header.Item1] = header.Item2;
                }

                return request;
            }

            private void GetResponseFromReflectionBasedProvider(string uri, string responseFormat, string[] xPathsToVerify)
            {
                CustomDataContext.ClearData();
                CustomDataContext.PreserveChanges = true;
                Type contextType = typeof(CustomDataContext);
                GetResponse(uri, responseFormat, contextType, xPathsToVerify, null);
                CustomDataContext.PreserveChanges = false;
            }

            private void GetResponseWithETagFromReflectionBasedProvider(string uri, string responseFormat, string[] xPathsToVerify, bool verifyETagScenarios)
            {
                GetResponseWithETagFromReflectionBasedProvider(uri, responseFormat, xPathsToVerify, verifyETagScenarios, false /*nullValueExpected*/);
            }

            private void GetResponseWithETagFromReflectionBasedProvider(string uri, string responseFormat, string[] xPathsToVerify, bool verifyETagScenarios, bool nullValueExpected)
            {
                CustomDataContext.ClearData();
                using (CustomDataContext.CreateChangeScope())
                {
                    Type contextType = typeof(CustomDataContext);
                    string etag = null;

                    if (verifyETagScenarios)
                    {
                        etag = GetETagFromResponse(contextType, uri, responseFormat);
                        Assert.IsTrue(!String.IsNullOrEmpty(etag), "!String.IsNullOrEmpty(etag)");

                        var ifMatch = new KeyValuePair<string, string>("If-Match", etag);
                        GetResponse(uri, responseFormat, contextType, xPathsToVerify, new KeyValuePair<string, string>[] { ifMatch });

                        VerifyStatusCode(uri, responseFormat, etag, contextType, (int)HttpStatusCode.NotModified);

                        ifMatch = new KeyValuePair<string, string>("If-Match", "W/\"sdfsdffweljrwerjwekr\"");
                        VerifyInvalidRequest(contextType, responseFormat, uri, (int)HttpStatusCode.PreconditionFailed, new KeyValuePair<string, string>[] { ifMatch });

                        var ifNoneMatch = new KeyValuePair<string, string>("If-None-Match", "W/\"sdfsdffweljrwerjwekr\"");
                        GetResponse(uri, responseFormat, typeof(CustomDataContext), xPathsToVerify, new KeyValuePair<string, string>[] { ifNoneMatch });


                        // for non-null values, * should behave as if there was no if-match header.
                        // for null values, it should return 412
                        ifMatch = new KeyValuePair<string, string>("If-Match", "*");
                        GetResponse(uri, responseFormat, contextType, xPathsToVerify, new KeyValuePair<string, string>[] { ifMatch });

                        ifNoneMatch = new KeyValuePair<string, string>("If-None-Match", "*");
                        VerifyStatusCode(uri, responseFormat, "*", contextType, (int)HttpStatusCode.NotModified);
                    }
                    else
                    {
                        // check and make sure the etag header is null
                        etag = GetETagFromResponse(contextType, uri, responseFormat);
                        Assert.IsTrue(String.IsNullOrEmpty(etag), "String.IsNullOrEmpty(etag)");

                        int errorCode = (int)HttpStatusCode.BadRequest;
                        if (nullValueExpected)
                        {
                            errorCode = (int)HttpStatusCode.PreconditionFailed;
                        }

                        var ifMatch = new KeyValuePair<string, string>("If-Match", "W/\"" + new Guid().ToString() + "\"");
                        VerifyInvalidRequest(contextType, responseFormat, uri, errorCode, new KeyValuePair<string, string>[] { ifMatch });

                        var ifNoneMatch = new KeyValuePair<string, string>("If-None-Match", "W/\"" + new Guid().ToString() + "\"");
                        GetResponse(uri, responseFormat, typeof(CustomDataContext), xPathsToVerify, new KeyValuePair<string, string>[] { ifNoneMatch });

                        if (nullValueExpected)
                        {
                            ifMatch = new KeyValuePair<string, string>("If-Match", "*");
                            VerifyInvalidRequest(contextType, responseFormat, uri, (int)HttpStatusCode.PreconditionFailed, new KeyValuePair<string, string>[] { ifMatch });

                            VerifyStatusCode(uri, responseFormat, "*", contextType, (int)HttpStatusCode.NoContent);
                        }
                    }
                }
            }

            private void GetResponse(string uri, string responseFormat, Type contextType, string[] xPathsToVerify, KeyValuePair<string, string>[] headerValues)
            {
                GetResponse(uri, responseFormat, contextType, xPathsToVerify, headerValues, "GET", null);
            }

            private void GetResponse(string uri, string responseFormat, Type contextType, string[] xPathsToVerify, KeyValuePair<string, string>[] headerValues, string httpMethodName, string requestPayload)
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = contextType;
                    request.RequestUriString = uri;
                    request.Accept = responseFormat;
                    request.HttpMethod = httpMethodName;
                    UnitTestsUtil.SetHeaderValues(request, headerValues);
                    if (requestPayload != null)
                    {
                        request.RequestContentType = responseFormat;
                        request.RequestStream = new MemoryStream();
                        StreamWriter writer = new StreamWriter(request.RequestStream);
                        writer.Write(requestPayload);
                        writer.Flush();
                    }
                    request.SendRequest();
                    Stream responseStream = request.GetResponseStream();
                    if (xPathsToVerify != null)
                    {
                        UnitTestsUtil.VerifyXPaths(responseStream, responseFormat, xPathsToVerify);
                    }
                }
            }

            private static void VerifyInvalidRequestForAllProviders(string uri, int errorCode, params KeyValuePair<string, string>[] headerValues)
            {
                // Very simple test at this level: simply check that we can get an entity by its ID.
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("ContentFormat", UnitTestsUtil.ResponseFormats),
                    new Dimension("ContextType", new Type[] 
                    { 
                        typeof(CustomDataContext), 
                        typeof(ocs.CustomObjectContext),
                        typeof(CustomRowBasedContext),
                        typeof(CustomRowBasedOpenTypesContext)
                    }));

                ocs.PopulateData.EntityConnection = null;
                using (System.Data.EntityClient.EntityConnection connection = ocs.PopulateData.CreateTableAndPopulateData())
                {
                    TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
                                                                    {
                                                                        Type contextType = (Type) values["ContextType"];
                                                                        string contentFormat = (string) values["ContentFormat"];

                                                                        VerifyInvalidRequest(contextType, contentFormat, uri, errorCode, headerValues);
                                                                    });
                }
            }

            private static void VerifyInvalidRequest(Type contextType, string contentFormat, string uri, int errorCode, KeyValuePair<string, string>[] headerValues)
            {
                VerifyInvalidRequest(contextType, contentFormat, uri, errorCode, headerValues, "GET", null);
            }

            private static void VerifyInvalidRequest(Type contextType, string contentFormat, string uri, int errorCode, KeyValuePair<string, string>[] headerValues, string httpMethodName, string requestPayload)
            {
                UnitTestsUtil.VerifyInvalidRequest(requestPayload, uri, contextType, contentFormat, httpMethodName, errorCode, headerValues);
            }

            private static TestWebRequest GetTestWebRequestInstance(string responseFormat, string uri, Type contextType, KeyValuePair<string, string>[] headerValues, string httpMethodName)
            {
                return UnitTestsUtil.GetTestWebRequestInstance(responseFormat, uri, contextType, headerValues, httpMethodName);
            }

            private static void VerifyEmptyStream(Stream stream)
            {
                string streamContent = new StreamReader(stream).ReadToEnd();
                Assert.IsTrue(streamContent.Length == 0, "streamContent.Length == 0");
            }

            private static string GetETagFromResponse(Type contextType, string uri, string responseFormat)
            {
                return UnitTestsUtil.GetETagFromResponse(contextType, uri, responseFormat);
            }

            private static void VerifyStatusCode(string uri, string responseFormat, string etag, Type contextType, int statusCode)
            {
                var ifNoneMatch = new KeyValuePair<string, string>("If-None-Match", etag);
                TestWebRequest request = GetTestWebRequestInstance(responseFormat, uri, contextType, new KeyValuePair<string, string>[] { ifNoneMatch }, "GET");
                Assert.IsTrue(request.ResponseStatusCode == statusCode, "Since the etag should match, the status code should be 312");
                VerifyEmptyStream(request.GetResponseStream());
                request.Dispose();
            }
        }
    }
}
