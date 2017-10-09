//---------------------------------------------------------------------
// <copyright file="EnumerationTypeQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.EnumerationTypeTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OClient = Microsoft.OData.Client;

    /// <summary>
    /// Send query and verify the results from the service implemented using ODataLib and EDMLib.
    /// </summary>
    [TestClass]
    public class EnumerationTypeQueryTests : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        private static string NameSpacePrefix = "Microsoft.Test.OData.Services.ODataWCFService.";

        public EnumerationTypeQueryTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {

        }

        [TestMethod]
        public void QueryEntitySet()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Products", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        List<ODataResource> entries = new List<ODataResource>();
                        var reader = messageReader.CreateODataResourceSetReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                ODataResource entry = reader.Item as ODataResource;
                                Assert.IsNotNull(entry);
                                entries.Add(entry);
                            }
                            else if (reader.State == ODataReaderState.ResourceSetEnd)
                            {
                                Assert.IsNotNull(reader.Item as ODataResourceSet);
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                        Assert.AreEqual(5, entries.Count);

                        ODataEnumValue skinColor = (ODataEnumValue)entries[1].Properties.Single(p => p.Name == "SkinColor").Value;
                        ODataEnumValue userAccess = (ODataEnumValue)entries[1].Properties.Single(p => p.Name == "UserAccess").Value;
                        Assert.AreEqual("Blue", skinColor.Value);
                        Assert.AreEqual("ReadWrite", userAccess.Value);
                    }
                }
            }
        }

        [TestMethod]
        public void QuerySpecificEntity()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Products(6)", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        List<ODataResource> entries = new List<ODataResource>();
                        var reader = messageReader.CreateODataResourceReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                ODataResource entry = reader.Item as ODataResource;
                                Assert.IsNotNull(entry);
                                entries.Add(entry);
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                        Assert.AreEqual(1, entries.Count);

                        ODataEnumValue skinColor = (ODataEnumValue)entries[0].Properties.Single(p => p.Name == "SkinColor").Value;
                        ODataEnumValue userAccess = (ODataEnumValue)entries[0].Properties.Single(p => p.Name == "UserAccess").Value;
                        Assert.AreEqual("Blue", skinColor.Value);
                        Assert.AreEqual("ReadWrite", userAccess.Value);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryEnumProperty()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Products(5)/SkinColor", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                if (mimeType == MimeTypes.ApplicationAtomXml)
                {
                    requestMessage.SetHeader("Accept", "text/html, application/xhtml+xml, */*");
                }

                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        ODataProperty skinColorProperty = messageReader.ReadProperty();
                        ODataEnumValue enumValue = skinColorProperty.Value as ODataEnumValue;
                        Assert.AreEqual("Red", enumValue.Value);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryEnumPropertyValue()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Products(5)/SkinColor/$value", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "*/*");
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
            {
                var skinColorPropertyValue = messageReader.ReadValue(EdmCoreModel.Instance.GetString(false));
                Assert.AreEqual("Red", skinColorPropertyValue);
            }
        }

        [TestMethod]
        public void InvokeActionWithEnumParameterAndReturnType()
        {
            var writerSettings = new ODataMessageWriterSettings();
            writerSettings.BaseUri = ServiceBaseUri;
            var readerSettings = new ODataMessageReaderSettings();
            readerSettings.BaseUri = ServiceBaseUri;

            foreach (var mimeType in mimeTypes)
            {
                if (mimeType == MimeTypes.ApplicationAtomXml)
                {
                    continue;
                }

                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Products(5)/Microsoft.Test.OData.Services.ODataWCFService.AddAccessRight"));

                requestMessage.SetHeader("Content-Type", mimeType);
                requestMessage.SetHeader("Accept", "*/*");
                requestMessage.Method = "POST";
                ODataEnumValue accessRight = new ODataEnumValue("Read,Execute", NameSpacePrefix + "AccessLevel");
                using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings, Model))
                {
                    var odataWriter = messageWriter.CreateODataParameterWriter((IEdmOperation)null);
                    odataWriter.WriteStart();
                    odataWriter.WriteValue("accessRight", accessRight);
                    odataWriter.WriteEnd();
                }

                // send the http request
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var userAccessPropertyValue = messageReader.ReadProperty();
                        ODataEnumValue enumValue = userAccessPropertyValue.Value as ODataEnumValue;
                        Assert.AreEqual("Read, Execute", enumValue.Value);
                    }
                }
            }
        }

        [TestMethod]
        public void CallUnboundFunctionWhichReturnsEnumValue()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "GetDefaultColor()", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                if (mimeType == MimeTypes.ApplicationAtomXml)
                {
                    requestMessage.SetHeader("Accept", "text/html, application/xhtml+xml, */*");
                }

                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        ODataProperty colorProperty = messageReader.ReadProperty();
                        ODataEnumValue enumValue = colorProperty.Value as ODataEnumValue;
                        Assert.AreEqual("Red", enumValue.Value);
                    }
                }
            }

        }

        [TestMethod]
        public void QueryEntitiesFilterByEnumProperty()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Products?$filter=UserAccess has Microsoft.Test.OData.Services.ODataWCFService.AccessLevel'Read'", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        List<ODataResource> entries = new List<ODataResource>();
                        var reader = messageReader.CreateODataResourceSetReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                ODataResource entry = reader.Item as ODataResource;
                                Assert.IsNotNull(entry);
                                entries.Add(entry);
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                        Assert.AreEqual(3, entries.Count);

                        Assert.AreEqual(6, entries[0].Properties.Single(p => p.Name == "ProductID").Value);
                        Assert.AreEqual(7, entries[1].Properties.Single(p => p.Name == "ProductID").Value);
                        Assert.AreEqual(9, entries[2].Properties.Single(p => p.Name == "ProductID").Value);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryEntitiesOrderByEnumProperty()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Products?$orderby=SkinColor", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        List<ODataResource> entries = new List<ODataResource>();
                        var reader = messageReader.CreateODataResourceSetReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                ODataResource entry = reader.Item as ODataResource;
                                Assert.IsNotNull(entry);
                                entries.Add(entry);
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                        Assert.AreEqual(5, entries.Count);

                        Assert.AreEqual(5, entries[0].Properties.Single(p => p.Name == "ProductID").Value);
                        Assert.AreEqual(7, entries[1].Properties.Single(p => p.Name == "ProductID").Value);
                        Assert.AreEqual(8, entries[2].Properties.Single(p => p.Name == "ProductID").Value);
                        Assert.AreEqual(9, entries[3].Properties.Single(p => p.Name == "ProductID").Value);
                        Assert.AreEqual(6, entries[4].Properties.Single(p => p.Name == "ProductID").Value);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryEntitiesSelectEnumProperty()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Products?$select=SkinColor,UserAccess", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        List<ODataResource> entries = new List<ODataResource>();
                        var reader = messageReader.CreateODataResourceSetReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                ODataResource entry = reader.Item as ODataResource;
                                Assert.IsNotNull(entry);
                                entries.Add(entry);
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                        Assert.AreEqual(5, entries.Count);

                        Assert.AreEqual(false, entries[0].Properties.Any(p => p.Name != "SkinColor" && p.Name != "UserAccess"));

                    }
                }
            }
        }

        #region client operations

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [TestMethod]
        public void QueryEntitySetFromODataClient()
        {
            for (int i = 1; i < 2; i++)
            {
                if (i == 0)
                {
                    //TestClientContext.Format.UseAtom();
                }
                else
                {
                    TestClientContext.Format.UseJson(Model);
                }

                var queryable = TestClientContext.Products;
                Assert.IsTrue(queryable.RequestUri.OriginalString.EndsWith("/Products", StringComparison.Ordinal));

                List<Product> result = queryable.ToList();
                Assert.AreEqual(5, result.Count);

                Assert.AreEqual(Color.Blue, result[1].SkinColor);
                Assert.AreEqual(AccessLevel.ReadWrite, result[1].UserAccess);
            }
        }

        [TestMethod]
        public void QueryEntityFromODataClient()
        {
            for (int i = 1; i < 2; i++)
            {
                if (i == 0)
                {
                    //TestClientContext.Format.UseAtom();
                }
                else
                {
                    TestClientContext.Format.UseJson(Model);
                }

                var queryable = TestClientContext.Products.Where(p => p.ProductID == 8) as OClient.DataServiceQuery<Product>;
                Assert.IsTrue(queryable.RequestUri.OriginalString.EndsWith("/Products(8)", StringComparison.Ordinal));

                List<Product> result = queryable.ToList();
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(Color.Red, result[0].SkinColor);
                Assert.AreEqual(AccessLevel.Execute, result[0].UserAccess);
            }
        }

        [TestMethod]
        public void QueryEnumPropertyFromODataClient()
        {
            for (int i = 1; i < 2; i++)
            {
                if (i == 0)
                {
                    //TestClientContext.Format.UseAtom();
                }
                else
                {
                    TestClientContext.Format.UseJson(Model);
                }

                var userAccess = TestClientContext.Execute<AccessLevel>(new Uri(ServiceBaseUri.AbsoluteUri + "/Products(8)/UserAccess"));
                List<AccessLevel> enumResult = userAccess.ToList();
                Assert.AreEqual(1, enumResult.Count);
                Assert.AreEqual(AccessLevel.Execute, enumResult[0]);
            }
        }

        [TestMethod]
        public void QueryEnumCollectionPropertyFromODataClient()
        {
            for (int i = 1; i < 2; i++)
            {
                if (i == 0)
                {
                    //TestClientContext.Format.UseAtom();
                }
                else
                {
                    TestClientContext.Format.UseJson(Model);
                }

                var resultTmp = TestClientContext.Execute<List<Color>>(new Uri(ServiceBaseUri.AbsoluteUri + "/Products(5)/CoverColors"));
                List<List<Color>> enumResult = resultTmp.ToList();
                Assert.AreEqual(1, enumResult.Count);
                Assert.AreEqual(Color.Green, enumResult[0][0]);
                Assert.AreEqual(Color.Blue, enumResult[0][1]);
                Assert.AreEqual(Color.Blue, enumResult[0][2]);
            }
        }

        [TestMethod]
        public void QueryEntitiesWithQueryOptionsFromODataClient()
        {
            // MinimalMetadata: UseJson() + no $select in request uri
            TestClientContext.Format.UseJson(Model);
            var queryable = TestClientContext.CreateQuery<Product>("Products")
                .AddQueryOption("$filter", "SkinColor eq Microsoft.Test.OData.Services.ODataWCFService.Color'Red'");

            Assert.IsTrue(queryable.RequestUri.OriginalString.EndsWith("/Products?$filter=SkinColor eq Microsoft.Test.OData.Services.ODataWCFService.Color'Red'", StringComparison.Ordinal));
            List<Product> result = queryable.ToList<Product>();
            Assert.IsTrue(result.All(s => s.SkinColor == Color.Red));
            //Assert.IsTrue(result.Any(s => s.UserAccess == AccessLevel.None));
            //Assert.IsTrue(result.Any(s => s.UserAccess == AccessLevel.Execute));
            //Assert.IsTrue(result.Any(s => s.UserAccess == AccessLevel.Read));
            Assert.AreEqual(result.Select(s => s.ProductID).Distinct().Count(), result.Count);

            // FullMetadata: UseJson() + have $select in request uri
            TestClientContext.Format.UseJson(Model);
            queryable = TestClientContext.CreateQuery<Product>("Products")
                .AddQueryOption("$filter", "SkinColor eq Microsoft.Test.OData.Services.ODataWCFService.Color'Red'")
                .AddQueryOption("$select", "SkinColor,ProductID,Name");

            Assert.IsTrue(queryable.RequestUri.OriginalString.EndsWith("/Products?$filter=SkinColor eq Microsoft.Test.OData.Services.ODataWCFService.Color'Red'&$select=SkinColor,ProductID,Name", StringComparison.Ordinal));
            result = queryable.ToList<Product>();
            Assert.IsTrue(result.All(s => s.SkinColor == Color.Red));
            //Assert.IsTrue(result.Any(s => s.UserAccess == AccessLevel.None));
            //Assert.IsTrue(result.Any(s => s.UserAccess == AccessLevel.Execute));
            //Assert.IsTrue(result.Any(s => s.UserAccess == AccessLevel.Read));
            Assert.AreEqual(result.Select(s => s.ProductID).Distinct().Count(), result.Count);

            // Atom
            queryable = TestClientContext.CreateQuery<Product>("Products")
                .AddQueryOption("$filter", "SkinColor eq Microsoft.Test.OData.Services.ODataWCFService.Color'Red'");

            Assert.IsTrue(queryable.RequestUri.OriginalString.EndsWith("/Products?$filter=SkinColor eq Microsoft.Test.OData.Services.ODataWCFService.Color'Red'", StringComparison.Ordinal));
            result = queryable.ToList<Product>();
            Assert.IsTrue(result.All(s => s.SkinColor == Color.Red));
            Assert.IsTrue(result.Any(s => s.UserAccess == AccessLevel.Execute));
            Assert.IsTrue(result.Any(s => s.UserAccess == AccessLevel.Read));
            Assert.AreEqual(result.Select(s => s.ProductID).Distinct().Count(), result.Count);
        }

        [TestMethod]
        public void QueryEntitiesWithFilterFromODataClient()
        {
            var products = TestClientContext.Products.ToList();

            //$filter with Binary Operation
            var queryable = TestClientContext.Products.Where(p => p.SkinColor == Color.Red).Where(p => AccessLevel.Read < p.UserAccess) as OClient.DataServiceQuery<Product>;
            List<Product> result = queryable.ToList();
            Assert.IsTrue(queryable.RequestUri.OriginalString.EndsWith(
                "/Products?$filter=SkinColor eq Microsoft.Test.OData.Services.ODataWCFService.Color'Red' and Microsoft.Test.OData.Services.ODataWCFService.AccessLevel'Read' lt UserAccess"
                , StringComparison.Ordinal));
            Assert.IsTrue(result.All(s => s.SkinColor == Color.Red));
            Assert.IsTrue(result.All(s => s.UserAccess > AccessLevel.Read));
            Assert.AreEqual(products.Where(p => p.SkinColor == Color.Red && p.UserAccess > AccessLevel.Read).Count(), result.Count);

            queryable = TestClientContext.Products.Where(p => AccessLevel.Read == (AccessLevel)p.UserAccess) as OClient.DataServiceQuery<Product>;
            result = queryable.ToList();
            Assert.IsTrue(queryable.RequestUri.OriginalString.EndsWith(
                "/Products?$filter=Microsoft.Test.OData.Services.ODataWCFService.AccessLevel'Read' eq UserAccess",
                StringComparison.Ordinal));
            Assert.IsTrue(result.All(s => s.UserAccess == AccessLevel.Read));
            Assert.AreEqual(products.Where(p => p.UserAccess == AccessLevel.Read).Count(), result.Count);

            //$filter with has
            queryable = TestClientContext.Products.Where(p => p.SkinColor.Value.HasFlag(Color.Red) || AccessLevel.Read.HasFlag(p.UserAccess.Value)) as OClient.DataServiceQuery<Product>;
            result = queryable.ToList();
            Assert.IsTrue(queryable.RequestUri.OriginalString.EndsWith(
                "/Products?$filter=SkinColor has Microsoft.Test.OData.Services.ODataWCFService.Color'Red' or Microsoft.Test.OData.Services.ODataWCFService.AccessLevel'Read' has UserAccess"
                , StringComparison.Ordinal));
            Assert.IsTrue(result.All(s => s.SkinColor.Value.HasFlag(Color.Red) || s.UserAccess.Value == AccessLevel.Read || s.UserAccess.Value == AccessLevel.None));
            Assert.AreEqual(products.Where(p => p.SkinColor.Value.HasFlag(Color.Red) || AccessLevel.Read.HasFlag(p.UserAccess)).Count(), result.Count);

            //$filter with Built-in Query Functions
            //isof
            queryable = TestClientContext.Products.Where(p => p.UserAccess is AccessLevel) as OClient.DataServiceQuery<Product>;
            result = queryable.ToList();
            Assert.IsTrue(queryable.RequestUri.OriginalString.EndsWith(
                "/Products?$filter=isof(UserAccess, 'Microsoft.Test.OData.Services.ODataWCFService.AccessLevel')"
                , StringComparison.Ordinal));
            Assert.AreEqual(products.Count(), result.Count);

            //Any
            queryable = TestClientContext.Products.Where(p => p.CoverColors.Any(c => c == Color.Blue)).Where(p => p.CoverColors.Any(c => c.HasFlag(Color.Blue))) as OClient.DataServiceQuery<Product>;
            result = queryable.ToList();
            Assert.IsTrue(queryable.RequestUri.OriginalString.EndsWith(
                "/Products?$filter=CoverColors/any(c:c eq Microsoft.Test.OData.Services.ODataWCFService.Color'Blue') and CoverColors/any(c:c has Microsoft.Test.OData.Services.ODataWCFService.Color'Blue')"
                , StringComparison.Ordinal));
            Assert.AreEqual(products.Where(p => p.CoverColors.Any(c => c == Color.Blue)).Count(), result.Count);

            //All
            queryable = TestClientContext.Products.Where(p => p.CoverColors.All(c => c == Color.Blue)).Where(p => p.CoverColors.All(c => c.HasFlag(Color.Blue))) as OClient.DataServiceQuery<Product>;
            result = queryable.ToList();
            Assert.IsTrue(queryable.RequestUri.OriginalString.EndsWith(
                "/Products?$filter=CoverColors/all(c:c eq Microsoft.Test.OData.Services.ODataWCFService.Color'Blue') and CoverColors/all(c:c has Microsoft.Test.OData.Services.ODataWCFService.Color'Blue')"
                , StringComparison.Ordinal));
            Assert.AreEqual(products.Where(p => p.CoverColors.All(c => c == Color.Blue)).Count(), result.Count);
        }

        [TestMethod]
        public void QueryEntitiesWithOrderByFromODataClient()
        {
            var products = TestClientContext.Products.ToList();

            var queryable = TestClientContext.Products.OrderBy(p => p.UserAccess) as OClient.DataServiceQuery<Product>;
            var result = queryable.ToList();
            Assert.IsTrue(queryable.RequestUri.OriginalString.EndsWith("/Products?$orderby=UserAccess", StringComparison.Ordinal));
            for (int i = 0; i < result.Count; i++)
            {
                var orderedProducts = products.OrderBy(p => p.UserAccess);
                Assert.AreEqual(orderedProducts.ElementAt(i).ProductID, result[i].ProductID);
            }

            queryable = TestClientContext.Products.OrderByDescending(p => p.UserAccess) as OClient.DataServiceQuery<Product>;
            result = queryable.ToList();
            Assert.IsTrue(queryable.RequestUri.OriginalString.EndsWith("/Products?$orderby=UserAccess desc", StringComparison.Ordinal));
            for (int i = 0; i < result.Count; i++)
            {
                var orderedProducts = products.OrderByDescending(p => p.UserAccess);
                Assert.AreEqual(orderedProducts.ElementAt(i).ProductID, result[i].ProductID);
            }
        }

        [TestMethod]
        public void QueryEntitiesWithSelectFromODataClient()
        {
            var products = TestClientContext.Products.ToList();

            var queryable = TestClientContext.Products.Where(p => p.ProductID == 5).Select(p => p.UserAccess) as OClient.DataServiceQuery<AccessLevel?>;
            var result = queryable.Single();
            Assert.IsTrue(queryable.RequestUri.OriginalString.EndsWith("/Products(5)/UserAccess", StringComparison.Ordinal));
            Assert.AreEqual(products.Where(p => p.ProductID == 5).Single().UserAccess, result);

            var queryable2 = TestClientContext.Products
                .Select(p => new Product { UserAccess = p.UserAccess, CoverColors = p.CoverColors, SkinColor = p.SkinColor, ProductID = p.ProductID })
                as OClient.DataServiceQuery<Product>;
            var result2 = queryable2.ToList();
            Assert.IsTrue(queryable2.RequestUri.OriginalString.EndsWith("/Products?$select=UserAccess,CoverColors,SkinColor,ProductID", StringComparison.Ordinal));
            foreach (var prod in result2)
            {
                var expectedProd = products.Where(p => p.ProductID == prod.ProductID).Single();

                Assert.AreEqual(expectedProd.UserAccess, prod.UserAccess);
                Assert.AreEqual(expectedProd.SkinColor, prod.SkinColor);
                Assert.AreEqual(expectedProd.CoverColors.Count, prod.CoverColors.Count);
            }
        }
#endif

        #endregion
    }
}
