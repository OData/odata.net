﻿//---------------------------------------------------------------------
// <copyright file="ODataWCFServiceQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ODataWCFServiceTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Client;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Xunit;
    using HttpWebRequestMessage = Common.HttpWebRequestMessage;

    /// <summary>
    /// Send query and verify the results from the service implemented using ODataLib and EDMLib.
    /// </summary>
    public class ODataWCFServiceQueryTests : ODataWCFServiceTestsBase<InMemoryEntities>, IDisposable
    {
        public ODataWCFServiceQueryTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {

        }

        private static string NameSpacePrefix = "Microsoft.Test.OData.Services.ODataWCFService.";

        [Fact]
        public void QueryServiceDocument()
        {
            string[] types = new string[]
            {
                "text/html, application/xhtml+xml, */*",
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
            };

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in types)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri, UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.Equal(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        ODataServiceDocument workSpace = messageReader.ReadServiceDocument();

                        Assert.NotNull(workSpace.EntitySets.Single(c => c.Name == "Customers"));
                        Assert.NotNull(workSpace.Singletons.Single(c => c.Name == "VipCustomer"));
                        Assert.NotNull(workSpace.Singletons.Single(c => c.Name == "Company"));
                    }
                }
            }
        }

        [Fact]
        public void QueryEntitySet()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Customers", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.Equal(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataResourceSetReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                ODataResource entry = reader.Item as ODataResource;
                                if (entry != null && entry.TypeName.EndsWith("Customer"))
                                {
                                    Assert.NotNull(Assert.IsType<ODataProperty>(entry.Properties.Single(p => p.Name == "PersonID")).Value);
                                }
                            }
                            else if (reader.State == ODataReaderState.ResourceSetEnd)
                            {
                                Assert.NotNull(reader.Item as ODataResourceSet);
                            }
                        }

                        Assert.Equal(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [Fact]
        public void QueryEntityInstance()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Customers(1)", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.Equal(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataResourceReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                ODataResource entry = reader.Item as ODataResource;
                                if (entry != null && entry.TypeName.EndsWith("Customer"))
                                {
                                    Assert.Equal(1, Assert.IsType<ODataProperty>(entry.Properties.Single(p => p.Name == "PersonID")).Value);
                                }
                            }
                        }

                        Assert.Equal(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [Fact]
        public void QueryEntityProperty()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Customers(1)/PersonID", UriKind.Absolute));

                requestMessage.SetHeader("Accept", mimeType);
                if (mimeType == MimeTypes.ApplicationAtomXml)
                {
                    requestMessage.SetHeader("Accept", "text/html, application/xhtml+xml, */*");
                }

                var responseMessage = requestMessage.GetResponse();
                Assert.Equal(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataProperty property = null;
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        property = messageReader.ReadProperty();
                    }

                    Assert.Equal(1, property.Value);
                }
            }
        }

        [Fact]
        public void QueryDollarValue()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "People(1)/Home/$value", UriKind.Absolute));

            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);
            object property = null;
            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
            {
                property = messageReader.ReadValue(new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true));
            }

            Assert.Equal(@"{""type"":""Point"",""coordinates"":[23.1,32.1],""crs"":{""type"":""name"",""properties"":{""name"":""EPSG:4326""}}}", property);
        }

        [Fact]
        public void QueryEntityNavigation()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Customers(1)/Orders", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.Equal(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataResourceSetReader();
                        ODataResource entry = null;

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                entry = reader.Item as ODataResource;
                            }
                            else if (reader.State == ODataReaderState.ResourceSetEnd)
                            {
                                Assert.NotNull(reader.Item as ODataResourceSet);
                            }
                        }

                        Assert.Equal(8, Assert.IsType<ODataProperty>(entry.Properties.Single(p => p.Name == "OrderID")).Value);
                        Assert.Equal(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [Fact]
        public void QueryEntityNavigationWithImplicitKeys()
        {
            // test Uri's
            List<string> testCases = new List<string>
            {
                "Products(5)/Details(3)",
                "Products(5)/Details(ProductDetailID=3)",
                "Products(5)/Details(ProductID=5,ProductDetailID=3)",
                "ProductDetails(ProductID=5,ProductDetailID=2)/Reviews(ReviewTitle='So so',RevisionID=1)",
                "ProductDetails(ProductID=5,ProductDetailID=2)/Reviews(ProductDetailID=2,ReviewTitle='So so',RevisionID=1)",
                "ProductDetails(ProductID=5,ProductDetailID=2)/Reviews(ProductDetailID=2,RevisionID=1,ProductID=5,ReviewTitle='So so')",
            };

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var testCase in testCases)
            {
                foreach (var mimeType in mimeTypes)
                {
                    var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + testCase, UriKind.Absolute));
                    requestMessage.SetHeader("Accept", mimeType);
                    var responseMessage = requestMessage.GetResponse();
                    Assert.Equal(200, responseMessage.StatusCode);

                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        ODataResource entry = null;
                        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                        {
                            var reader = messageReader.CreateODataResourceReader();

                            while (reader.Read())
                            {
                                if (reader.State == ODataReaderState.ResourceEnd)
                                {
                                    entry = reader.Item as ODataResource;
                                }
                            }

                            Assert.Equal(ODataReaderState.Completed, reader.State);
                        }

                        Assert.True(entry.Id.OriginalString.EndsWith("ProductDetails(ProductID=5,ProductDetailID=3)") ||
                                        entry.Id.OriginalString.EndsWith("ProductReviews(ProductID=5,ProductDetailID=2,ReviewTitle='So%20so',RevisionID=1)"));
                    }
                }
            }
        }

        [Fact]
        public void QueryEntityPropertyValue()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Customers(1)/FirstName/$value", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "*/*");

            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
            {
                var propertyValue = messageReader.ReadValue(EdmCoreModel.Instance.GetString(false));
                Assert.Equal("Bob", propertyValue);
            }
        }

        [Fact]
        public void QueryCount()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Customers/$count", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "*/*");

            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
            {
                var count = messageReader.ReadValue(EdmCoreModel.Instance.GetInt32(false));
                Assert.Equal(2, count);
            }
        }

        [Fact]
        public void QueryWithFilter()
        {
            // test Uri and expected resulting PersonID
            Dictionary<string, int> testCases = new Dictionary<string, int>()
            {
                {"Customers?$filter=PersonID eq 1", 1},
                {"Customers?$filter=PersonID le 1", 1},
                {"Customers?$filter=PersonID gt 1", 2},
                {"Customers?$filter=LastName eq 'Jones'", 2},
                {"Customers?$filter=PersonID ge 1 and FirstName eq 'Jill'", 2},
                {"Customers?$filter=1 add PersonID eq 2", 1},
                {"Customers?$filter=HomeAddress eq null", 2},//null
                {"Customers?$filter=Home ne null", 2},//Spatial type ne null
            };

            this.SendRequestAndVerifyResponse(testCases);
        }

        [Fact]
        public void QueryWithParameterAliasInFilter()
        {
            // test Uri and expected resulting PersonID
            Dictionary<string, int> testCases = new Dictionary<string, int>()
            {
                {"Customers?$filter=FirstName eq @name&@name='Bob'", 1},
                {"Customers?$filter=FirstName eq @name and LastName ne @name&@name='Bob'", 1},
                {"Customers?$filter=FirstName eq @name and PersonID eq @id&@name='Bob'&@id=1", 1},
                {"Customers?$filter=contains(FirstName,@name)&@name='Bo'", 1},
                {"Customers?$filter=PersonID eq @id&@id=1L", 1},//ConvertNode
                {"Customers?$filter=HomeAddress eq @home", 2},//parameter alias value is not provided.
            };
            this.SendRequestAndVerifyResponse(testCases);
        }

        private void SendRequestAndVerifyResponse(Dictionary<string, int> testCases)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var testCase in testCases)
            {
                foreach (var mimeType in mimeTypes)
                {
                    var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + testCase.Key, UriKind.Absolute));
                    requestMessage.SetHeader("Accept", mimeType);
                    var responseMessage = requestMessage.GetResponse();
                    Assert.Equal(200, responseMessage.StatusCode);
                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        ODataResource entry = null;
                        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                        {
                            var reader = messageReader.CreateODataResourceSetReader();

                            while (reader.Read())
                            {
                                if (reader.State == ODataReaderState.ResourceEnd)
                                {
                                    entry = reader.Item as ODataResource;
                                }
                                else if (reader.State == ODataReaderState.ResourceSetEnd)
                                {
                                    Assert.NotNull(reader.Item as ODataResourceSet);
                                }
                            }
                        }

                        Assert.NotNull(entry);
                        //String.Format("Mime:{0},URL:{1}", mimeType, testCase.Key)
                        Assert.Equal(testCase.Value, Assert.IsType<ODataProperty>(entry.Properties.Single(p => p.Name == "PersonID")).Value);
                    }
                }
            }
        }

        [Fact]
        public void QueryWithExpand()
        {
            Dictionary<string, bool> testCases = new Dictionary<string, bool>()
            {
                { "Customers(1)?$expand=Orders", false /*single property selected*/ },
                { "Orders(8)?$expand=CustomerForOrder", false /*single property selected*/ },
                { "Orders(8)?$select=OrderID&$expand=CustomerForOrder($select=PersonID)", true /*single property selected*/ }
            };

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var testCase in testCases)
            {
                foreach (var mimeType in mimeTypes)
                {
                    var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + testCase.Key, UriKind.Absolute));
                    requestMessage.SetHeader("Accept", mimeType);
                    var responseMessage = requestMessage.GetResponse();
                    Assert.Equal(200, responseMessage.StatusCode);

                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                        {
                            List<ODataResource> entries = new List<ODataResource>();
                            List<ODataNestedResourceInfo> navigationLinks = new List<ODataNestedResourceInfo>();

                            var reader = messageReader.CreateODataResourceReader();
                            while (reader.Read())
                            {
                                if (reader.State == ODataReaderState.ResourceEnd)
                                {
                                    var entry = reader.Item as ODataResource;
                                    if (entry != null)
                                    {
                                        entries.Add(entry);
                                    }
                                }
                                else if (reader.State == ODataReaderState.NestedResourceInfoEnd)
                                {
                                    navigationLinks.Add(reader.Item as ODataNestedResourceInfo);
                                }
                            }

                            Assert.Equal(ODataReaderState.Completed, reader.State);

                            Assert.True(navigationLinks.Count > 0);

                            var order = entries.FirstOrDefault(e => e != null && e.Id != null && e.Id.AbsoluteUri.Contains("Orders"));
                            Assert.NotNull(order);
                            Assert.Equal(8, Assert.IsType<ODataProperty>(order.Properties.Single(p => p.Name == "OrderID")).Value);

                            var customer = entries.FirstOrDefault(e => e.Id != null && e.Id.AbsoluteUri.Contains("Customers"));
                            Assert.NotNull(customer);
                            Assert.Equal(1, Assert.IsType<ODataProperty>(customer.Properties.Single(p => p.Name == "PersonID")).Value);

                            if (testCase.Value /*single property selected*/)
                            {
                                Assert.Single(order.Properties);
                                Assert.Single(customer.Properties);
                            }
                            else
                            {
                                Assert.True(order.Properties.Count() > 1);
                                Assert.True(customer.Properties.Count() > 1);
                            }
                        }
                    }
                }
            }
        }

        [Fact]
        public void QueryWithSelect()
        {
            Dictionary<string, int> testCases = new Dictionary<string, int>()
            {
                { "Customers?$select=PersonID,FirstName", 2 },
                { "Customers?$select=*", 10 },
            };

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var testCase in testCases)
            {
                foreach (var mimeType in mimeTypes)
                {
                    var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + testCase.Key, UriKind.Absolute));
                    requestMessage.SetHeader("Accept", mimeType);
                    var responseMessage = requestMessage.GetResponse();
                    Assert.Equal(200, responseMessage.StatusCode);

                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                        {
                            var reader = messageReader.CreateODataResourceSetReader();
                            int count = 0;
                            while (reader.Read())
                            {
                                if (reader.State == ODataReaderState.ResourceEnd)
                                {
                                    ODataResource entry = reader.Item as ODataResource;
                                    if (entry != null && entry.TypeName.EndsWith("Customer"))
                                    {
                                        Assert.NotNull(Assert.IsType<ODataProperty>(entry.Properties.Single(p => p.Name == "PersonID")).Value);
                                        Assert.NotNull(Assert.IsType<ODataProperty>(entry.Properties.Single(p => p.Name == "FirstName")).Value);
                                        Assert.Equal(testCase.Value, entry.Properties.Count());
                                        count++;
                                    }
                                }
                                else if (reader.State == ODataReaderState.ResourceSetEnd)
                                {
                                    Assert.NotNull(reader.Item as ODataResourceSet);
                                }
                            }

                            Assert.Equal(ODataReaderState.Completed, reader.State);
                            Assert.True(count > 0, "No entry is returned.");
                        }
                    }
                }
            }
        }

        [Fact]
        public void QueryWithOrderBy()
        {
            Dictionary<string, int> testCases = new Dictionary<string, int>()
            {
                {"Customers?$orderby=PersonID", -1},//-1 means the current PersonID is smaller than the next one.
                {"Customers?$orderby=PersonID mul @factor&@factor=-1", 1},//-1 means the current PersonID is bigger than the next one.
                {"Customers?$orderby=PersonID mul @factor desc&@factor=-1", -1},
            };

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var testCase in testCases)
            {
                foreach (var mimeType in mimeTypes)
                {
                    var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + testCase.Key, UriKind.Absolute));
                    requestMessage.SetHeader("Accept", mimeType);
                    var responseMessage = requestMessage.GetResponse();
                    Assert.Equal(200, responseMessage.StatusCode);

                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        ODataResource entry1 = null;
                        ODataResource entry2 = null;
                        ODataResourceSet feed = null;
                        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                        {
                            var reader = messageReader.CreateODataResourceSetReader();

                            int depth = 0;
                            while (reader.Read())
                            {
                                switch (reader.State)
                                {
                                    case ODataReaderState.ResourceSetStart:
                                    case ODataReaderState.ResourceStart:
                                    case ODataReaderState.NestedResourceInfoStart:
                                        depth++;
                                        break;
                                    case ODataReaderState.ResourceSetEnd:
                                        if (depth == 1)
                                        {
                                            feed = reader.Item as ODataResourceSet;
                                        }
                                        depth--;
                                        break;

                                    case ODataReaderState.ResourceEnd:
                                        if (depth == 2)
                                        {
                                            if (null == entry1)
                                            {
                                                entry1 = reader.Item as ODataResource;
                                            }
                                            else
                                            {
                                                entry2 = reader.Item as ODataResource;
                                            }
                                        }
                                        depth--;
                                        break;
                                    case ODataReaderState.NestedResourceInfoEnd:
                                        depth--;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        Assert.NotNull(feed);

                        Assert.Equal(testCase.Value,
                            String.Compare(
                            Assert.IsType<ODataProperty>(entry1.Properties.Single(p => p.Name == "PersonID")).Value.ToString(),
                            Assert.IsType<ODataProperty>(entry2.Properties.Single(p => p.Name == "PersonID")).Value.ToString()));

                    }
                }
            }
        }

        [Fact]
        public void QueryWithFormat()
        {
#if (NETCOREAPP1_0 || NETCOREAPP2_0)
            Dictionary<string, string> testCases = new Dictionary<string, string>()
            {
                {"Customers?$format=application/json", "application/json"},
                {"Customers?$format=application/json;odata.metadata=full", "application/json; odata.metadata=full"},
                {"Customers?$format=json", "application/json"},
            };
#else
            Dictionary<string, string> testCases = new Dictionary<string, string>()
            {
                {"Customers?$format=application/json", "application/json"},
                {"Customers?$format=application/json;odata.metadata=full", "application/json;odata.metadata=full"},
                {"Customers?$format=json", "application/json"},
            };
#endif

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var testCase in testCases)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + testCase.Key, UriKind.Absolute));
                var responseMessage = requestMessage.GetResponse();
                Assert.Equal(200, responseMessage.StatusCode);

                string contentType = responseMessage.Headers.FirstOrDefault(x => x.Key.Equals("Content-Type")).Value;
                Assert.True(contentType.StartsWith(testCase.Value),
                    string.Format("contentType is '{0}', when the expected string starts with '{1}'", contentType, testCase.Value));

                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    var reader = messageReader.CreateODataResourceSetReader();

                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.ResourceEnd)
                        {
                            ODataResource entry = reader.Item as ODataResource;
                            if (entry != null && entry.TypeName.EndsWith("Customer"))
                            {
                                Assert.NotNull(Assert.IsType<ODataProperty>(entry.Properties.Single(p => p.Name == "PersonID")).Value);
                            }
                        }
                    }

                    Assert.Equal(ODataReaderState.Completed, reader.State);
                }
            }
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [Fact]
        public void QueryPropertyWithNullValueFromODataClient()
        {
            TestClientContext.Format.UseJson(Model);

            var middleName = TestClientContext.Execute<string>(new Uri(ServiceBaseUri.AbsoluteUri + "/People(5)/MiddleName"));
            List<string> enumResult = middleName.ToList();
            Assert.Empty(enumResult);
        }

        [Fact]
        public void QueryPropertyValueWhichIsNullFromODataClient()
        {
            TestClientContext.Format.UseJson(Model);

            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((Microsoft.OData.Client.HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader("Accept", "text/plain");
            var middleName = TestClientContext.Execute<string>(new Uri(ServiceBaseUri.AbsoluteUri + "/People(5)/MiddleName/$value"));
            List<string> enumResult = middleName.ToList();
            Assert.Empty(enumResult);
        }
#endif

        [Fact]
        public void QueryDelta()
        {
            Uri deltaLink = null;

            #region Request DeltaLink
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Orders?$expand=OrderDetails", UriKind.Absolute));
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata);
            requestMessage.SetHeader("Prefer", "odata.track-changes");
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
            {
                var reader = messageReader.CreateODataResourceSetReader();

                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource entry = reader.Item as ODataResource;
                        if (entry != null && entry.TypeName.EndsWith("Order"))
                        {
                            Assert.NotNull(Assert.IsType<ODataProperty>(entry.Properties.Single(p => p.Name == "OrderID")).Value);
                        }
                    }
                    else if (reader.State == ODataReaderState.ResourceSetEnd)
                    {
                        var feed = reader.Item as ODataResourceSet;
                        Assert.NotNull(feed);
                        deltaLink = feed.DeltaLink;
                    }
                }
                Assert.Equal(ODataReaderState.Completed, reader.State);
            }
            #endregion

            #region Add a new Order
            // create entry and insert
            var orderEntry = new ODataResource() { TypeName = NameSpacePrefix + "Order" };
            var orderP1 = new ODataProperty { Name = "OrderID", Value = 101 };
            var orderp2 = new ODataProperty { Name = "OrderDate", Value = new DateTimeOffset(new DateTime(2013, 8, 29, 14, 11, 57)) };
            var orderp3 = new ODataProperty
            {
                Name = "OrderShelfLifes",
                Value = new ODataCollectionValue() { TypeName = "Collection(Edm.Duration)", Items = new Collection<object> { new TimeSpan(1) } }
            };
            orderEntry.Properties = new[] { orderP1, orderp2, orderp3 };

            var settings = new ODataMessageWriterSettings();
            settings.BaseUri = ServiceBaseUri;

            var orderType = Model.FindDeclaredType(NameSpacePrefix + "Order") as IEdmEntityType;
            var orderSet = Model.EntityContainer.FindEntitySet("Orders");

            requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Orders"));
            requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata);
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata);
            requestMessage.Method = "POST";
            using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter(orderSet, orderType);
                odataWriter.WriteStart(orderEntry);
                odataWriter.WriteEnd();
            }
            requestMessage.GetResponse();
            #endregion

            #region Delete a Order
            requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Orders(8)"));
            requestMessage.Method = "DELETE";
            requestMessage.GetResponse();
            #endregion

            #region Delete a Order Detail link
            requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "OrderDetails(OrderID=7,ProductID=6)"));
            requestMessage.Method = "DELETE";
            requestMessage.GetResponse();
            #endregion

            #region Add a Order Detail entry and link
            #endregion

            #region Using DeltaLink
            requestMessage = new HttpWebRequestMessage(deltaLink);
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata);
            requestMessage.SetHeader("Prefer", "odata.track-changes");
            responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);
            #endregion
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
