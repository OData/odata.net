//---------------------------------------------------------------------
// <copyright file="FeedAndEntryMaterializerAdapterUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Materialization;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FeedAndEntryMaterializerAdapterUnitTests
    {
        [TestMethod]
        public void ValidateShortIntegrationFeedReading()
        {
            var initialFeed = new ODataFeed() {Id = new Uri("http://services.odata.org/OData/OData.svc/Products")};
            
            var productItem = new ODataEntry() {Id = new Uri("http://services.odata.org/OData/OData.svc/Products(0)")};
            productItem.Properties = new ODataProperty[] {new ODataProperty() {Name = "Id", Value = 0}};

            var categoryNavigationLink = new ODataNavigationLink() {Name = "Category"};

            var categoryItem = new ODataEntry() { Id = new Uri("http://services.odata.org/OData/OData.svc/Categories(0)") };
            categoryItem.Properties = new ODataProperty[] { new ODataProperty() { Name = "Id", Value = 0 } };

            var productsNavigationLink = new ODataNavigationLink() { Name = "Products" };

            var supplierNavigationLink = new ODataNavigationLink() { Name = "Supplier" };

            var testODataReader = new TestODataReader()
            {
               new TestODataReaderItem(ODataReaderState.FeedStart, initialFeed),
               new TestODataReaderItem(ODataReaderState.EntryStart, productItem),
               new TestODataReaderItem(ODataReaderState.NavigationLinkStart, categoryNavigationLink),
               new TestODataReaderItem(ODataReaderState.EntryStart, categoryItem),
               new TestODataReaderItem(ODataReaderState.NavigationLinkStart, productsNavigationLink),
               new TestODataReaderItem(ODataReaderState.NavigationLinkEnd, productsNavigationLink),
               new TestODataReaderItem(ODataReaderState.EntryEnd, categoryItem),
               new TestODataReaderItem(ODataReaderState.NavigationLinkEnd, categoryNavigationLink),
               new TestODataReaderItem(ODataReaderState.NavigationLinkStart, supplierNavigationLink),
               new TestODataReaderItem(ODataReaderState.NavigationLinkEnd, supplierNavigationLink),
               new TestODataReaderItem(ODataReaderState.EntryEnd, productItem),
               new TestODataReaderItem(ODataReaderState.FeedEnd, initialFeed),
            };

            ClientEdmModel clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);

            var responsePipeline = new DataServiceClientResponsePipelineConfiguration(new DataServiceContext());
            var odataReaderWrapper = ODataReaderWrapper.CreateForTest(testODataReader, responsePipeline);
            FeedAndEntryMaterializerAdapter reader = new FeedAndEntryMaterializerAdapter(ODataFormat.Json, odataReaderWrapper, clientEdmModel, MergeOption.OverwriteChanges);

            int readCounter = 0;

            while (reader.Read())
            {
                readCounter++;
            }

            readCounter.Should().Be(2);
        }
    }   
}
