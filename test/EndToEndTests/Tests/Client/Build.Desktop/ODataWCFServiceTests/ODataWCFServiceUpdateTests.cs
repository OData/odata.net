//---------------------------------------------------------------------
// <copyright file="ODataWCFServiceUpdateTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ODataWCFServiceTests
{
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Tests.Client.Common;
    using System;
    using System.Linq;
    using System.Collections.ObjectModel;
    using Xunit;

    /// <summary>
    /// CUD tests for the ODL service.
    /// </summary>
    public class ODataWCFServiceUpdateTests : ODataWCFServiceTestsBase<Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.InMemoryEntities>
    {
        private static string NameSpacePrefix = "Microsoft.Test.OData.Services.ODataWCFService.";

        public ODataWCFServiceUpdateTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {

        }

        /// <summary>
        /// Insert and delete a simple entity.
        /// </summary>
        [Fact]
        public void UpsertEntityInstance()
        {
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

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Orders(101)"));
            requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson);
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson);
            requestMessage.Method = "PUT";
            using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter(orderSet, orderType);
                odataWriter.WriteStart(orderEntry);
                odataWriter.WriteEnd();
            }

            var responseMessage = requestMessage.GetResponse();

            // verify the insert
            Assert.Equal(201, responseMessage.StatusCode);
            ODataResource entry = this.QueryEntityItem("Orders(101)") as ODataResource;
            Assert.Equal(101, entry.Properties.Single(p => p.Name == "OrderID").Value);

            // delete the entry
            var deleteRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Orders(101)"));
            deleteRequestMessage.Method = "DELETE";
            var deleteResponseMessage = deleteRequestMessage.GetResponse();

            // verify the delete
            Assert.Equal(204, deleteResponseMessage.StatusCode);
            ODataResource deletedEntry = this.QueryEntityItem("Orders(101)", 204) as ODataResource;
            Assert.Null(deletedEntry);
        }

        /// <summary>
        /// Insert and delete a simple entity.
        /// </summary>
        [Fact]
        public void InsertDeleteEntityInstance()
        {
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

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Orders"));
            requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata);
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata);
            requestMessage.Method = "POST";
            using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter(orderSet, orderType);
                odataWriter.WriteStart(orderEntry);
                odataWriter.WriteEnd();
            }

            var responseMessage = requestMessage.GetResponse();

            // verify the insert
            Assert.Equal(201, responseMessage.StatusCode);
            ODataResource entry = this.QueryEntityItem("Orders(101)") as ODataResource;
            Assert.Equal(101, entry.Properties.Single(p => p.Name == "OrderID").Value);

            // delete the entry
            var deleteRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Orders(101)"));
            deleteRequestMessage.Method = "DELETE";
            var deleteResponseMessage = deleteRequestMessage.GetResponse();

            // verify the delete
            Assert.Equal(204, deleteResponseMessage.StatusCode);
            ODataResource deletedEntry = this.QueryEntityItem("Orders(101)", 204) as ODataResource;
            Assert.Null(deletedEntry);
        }

        /// <summary>
        /// Update a simple entity.
        /// </summary>
        [Fact]
        public void UpdateEntityInstanceProperty()
        {
            // query an entry
            ODataResource customerEntry = this.QueryEntityItem("Customers(1)") as ODataResource;
            Assert.Equal("London", customerEntry.Properties.Single(p => p.Name == "City").Value);

            // send a request to update an entry property
            customerEntry = new ODataResource() { TypeName = NameSpacePrefix + "Customer" };
            customerEntry.Properties = new[] { new ODataProperty { Name = "City", Value = "Seattle" } };

            var settings = new ODataMessageWriterSettings();
            settings.BaseUri = ServiceBaseUri;

            var customerType = Model.FindDeclaredType(NameSpacePrefix + "Customer") as IEdmEntityType;
            var customerSet = Model.EntityContainer.FindEntitySet("Customers");

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Customers(1)"));
            requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson);
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson);
            requestMessage.Method = "PATCH";

            using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter(customerSet, customerType);
                odataWriter.WriteStart(customerEntry);
                odataWriter.WriteEnd();
            }

            var responseMessage = requestMessage.GetResponse();

            // verify the update
            Assert.Equal(204, responseMessage.StatusCode);
            ODataResource updatedCustomer = this.QueryEntityItem("Customers(1)") as ODataResource;
            Assert.Equal("Seattle", updatedCustomer.Properties.Single(p => p.Name == "City").Value);
        }

        private ODataItem QueryEntityItem(string uri, int expectedStatusCode = 200)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var queryRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + uri, UriKind.Absolute));
            queryRequestMessage.SetHeader("Accept", MimeTypes.ApplicationJsonLight);
            var queryResponseMessage = queryRequestMessage.GetResponse();
            Assert.Equal(expectedStatusCode, queryResponseMessage.StatusCode);

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

                    Assert.Equal(ODataReaderState.Completed, reader.State);
                }
            }

            return item;
        }
    }
}