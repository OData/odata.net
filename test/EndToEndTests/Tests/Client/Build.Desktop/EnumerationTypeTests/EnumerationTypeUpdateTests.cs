//---------------------------------------------------------------------
// <copyright file="EnumerationTypeUpdateTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.EnumerationTypeTests
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
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
    public class EnumerationTypeUpdateTests : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        private static string NameSpacePrefix = "Microsoft.Test.OData.Services.ODataWCFService.";

        public EnumerationTypeUpdateTests() : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {

        }

        /// <summary>
        /// Create and delete a simple entity.
        /// </summary>
        [TestMethod]
        public void CreateDeleteEntityWithEnumProperties()
        {
            // construct the request message to create an entity
            var productEntry = new ODataResource
            {
                TypeName = NameSpacePrefix + "Product",
                Properties = new[]
                {
                    new ODataProperty { Name = "Name", Value = "MoonCake" },
                    new ODataProperty { Name = "ProductID", Value = 101 },
                    new ODataProperty { Name = "QuantityInStock", Value = 20 },
                    new ODataProperty { Name = "QuantityPerUnit", Value = "100g Bag" },
                    new ODataProperty { Name = "UnitPrice", Value = 8.0f },
                    new ODataProperty { Name = "Discontinued", Value = true },
                    new ODataProperty { Name = "SkinColor", Value = new ODataEnumValue("Green", NameSpacePrefix + "Color") },
                    new ODataProperty { 
                        Name = "CoverColors", 
                        Value = new ODataCollectionValue() 
                        { 
                            TypeName = string.Format("Collection({0}Color)", NameSpacePrefix),
                            Items = new Collection<object>() 
                            {
                                new ODataEnumValue("Green", NameSpacePrefix + "Color"),
                                new ODataEnumValue("Blue", NameSpacePrefix + "Color"),
                                new ODataEnumValue("Green", NameSpacePrefix + "Color")
                            } 
                        } 
                    },
                    new ODataProperty { Name = "UserAccess", Value = new ODataEnumValue("Read", NameSpacePrefix + "AccessLevel") }
                }
            };

            var settings = new ODataMessageWriterSettings();
            settings.BaseUri = ServiceBaseUri;
            var productType = Model.FindDeclaredType(NameSpacePrefix + "Product") as IEdmEntityType;
            var productSet = Model.EntityContainer.FindEntitySet("Products");

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Products"));
                requestMessage.SetHeader("Content-Type", mimeType);
                requestMessage.SetHeader("Accept", mimeType);
                requestMessage.Method = "POST";
                using (var messageWriter = new ODataMessageWriter(requestMessage, settings, Model))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter(productSet, productType);
                    odataWriter.WriteStart(productEntry);
                    odataWriter.WriteEnd();
                }

                // send the http request
                var responseMessage = requestMessage.GetResponse();

                // verify the create
                Assert.AreEqual(201, responseMessage.StatusCode);
                ODataResource entry = this.QueryEntityItem("Products(101)") as ODataResource;
                ODataEnumValue skinColor = entry.Properties.Single(p => p.Name == "SkinColor").Value as ODataEnumValue;
                ODataEnumValue userAccess = entry.Properties.Single(p => p.Name == "UserAccess").Value as ODataEnumValue;
                Assert.AreEqual(101, entry.Properties.Single(p => p.Name == "ProductID").Value);
                Assert.AreEqual("Green", skinColor.Value);
                Assert.AreEqual("Read", userAccess.Value);

                // delete the entry
                var deleteRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Products(101)"));
                deleteRequestMessage.Method = "DELETE";
                var deleteResponseMessage = deleteRequestMessage.GetResponse();

                // verify the delete
                Assert.AreEqual(204, deleteResponseMessage.StatusCode);
                ODataResource deletedEntry = this.QueryEntityItem("Products(101)", 204) as ODataResource;
                Assert.IsNull(deletedEntry);
            }
        }

        /// <summary>
        /// Update a simple entity.
        /// </summary>
        [TestMethod]
        public void UpdateEnumProperty()
        {
            // query an entry
            ODataResource productEntry = this.QueryEntityItem("Products(5)") as ODataResource;

            // send a request to update an entry property
            productEntry = new ODataResource()
            {
                TypeName = NameSpacePrefix + "Product",
                Properties = new[]
                { 
                    new ODataProperty { Name = "SkinColor", Value = new ODataEnumValue("Green") },
                    new ODataProperty { Name = "UserAccess", Value = new ODataEnumValue("Read") }
                }
            };

            var settings = new ODataMessageWriterSettings();
            settings.BaseUri = ServiceBaseUri;
            var productType = Model.FindDeclaredType(NameSpacePrefix + "Product") as IEdmEntityType;
            var productSet = Model.EntityContainer.FindEntitySet("Products");

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Products(5)"));
                requestMessage.SetHeader("Content-Type", mimeType);
                requestMessage.SetHeader("Accept", mimeType);
                requestMessage.Method = "PATCH";

                using (var messageWriter = new ODataMessageWriter(requestMessage, settings, Model))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter(productSet, productType);
                    odataWriter.WriteStart(productEntry);
                    odataWriter.WriteEnd();
                }

                var responseMessage = requestMessage.GetResponse();

                // verify the update
                Assert.AreEqual(204, responseMessage.StatusCode);
                ODataResource updatedProduct = this.QueryEntityItem("Products(5)") as ODataResource;
                ODataEnumValue skinColor = updatedProduct.Properties.Single(p => p.Name == "SkinColor").Value as ODataEnumValue;
                ODataEnumValue userAccess = updatedProduct.Properties.Single(p => p.Name == "UserAccess").Value as ODataEnumValue;
                Assert.AreEqual("Green", skinColor.Value);
                Assert.AreEqual("Read", userAccess.Value);
            }
        }

        #region client operations

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [TestMethod]
        public void CreateUpdateEntityFromODataClient()
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

                string tmpName = Guid.NewGuid().ToString();
                var queryable = TestClientContext.Products.AddQueryOption("$filter", string.Format("Name eq '{0}'", tmpName));

                // query and verify
                var result1 = queryable.ToList();
                Assert.AreEqual(0, result1.Count);

                // create an entity
                Product product = new Product()
                {
                    ProductID = (new Random()).Next(), 
                    Name = tmpName, 
                    SkinColor = Color.Red, 
                    Discontinued = false, 
                    QuantityInStock = 23, 
                    QuantityPerUnit = "my quantity per unit", 
                    UnitPrice = 23.01f, 
                    UserAccess = AccessLevel.ReadWrite,
                    CoverColors = new ObservableCollection<Color>()
                    {
                        Color.Red,
                        Color.Blue
                    }
                };
                TestClientContext.AddToProducts(product);
                TestClientContext.SaveChanges();

                // query and verify
                var result2 = queryable.ToList();
                Assert.AreEqual(1, result2.Count);
                Assert.AreEqual(Color.Red, result2[0].SkinColor);

                // update the Enum properties
                product.SkinColor = Color.Green;
                product.UserAccess = AccessLevel.Execute;
                TestClientContext.UpdateObject(product);
                TestClientContext.SaveChanges();

                // query and verify
                var result3 = queryable.ToList();
                Assert.AreEqual(1, result3.Count);
                Assert.AreEqual(Color.Green, result3[0].SkinColor);
            }
        }
#endif


        #endregion

        #region private methods
        private ODataItem QueryEntityItem(string uri, int expectedStatusCode = 200)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var queryRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + uri, UriKind.Absolute));
            queryRequestMessage.SetHeader("Accept", MimeTypes.ApplicationJsonLight);
            var queryResponseMessage = queryRequestMessage.GetResponse();
            Assert.AreEqual(expectedStatusCode, queryResponseMessage.StatusCode);

            ODataItem item = null;
            if (expectedStatusCode == 200)
            {
                using (var messageReader = new ODataMessageReader(queryResponseMessage, readerSettings, Model))
                {
                    var reader = messageReader.CreateODataResourceReader();
                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.ResourceEnd)
                        {
                            item = reader.Item;
                        }
                    }

                    Assert.AreEqual(ODataReaderState.Completed, reader.State);
                }
            }

            return item;
        }

        #endregion
    }
}
