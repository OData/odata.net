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
    using Microsoft.OData;
    using Xunit;

    public class FeedAndEntryMaterializerAdapterUnitTests
    {
        [Fact]
        public void ValidateShortIntegrationFeedReading()
        {
            var initialFeed = new ODataResourceSet() {Id = new Uri("http://services.odata.org/OData/OData.svc/Products")};
            
            var productItem = new ODataResource() {Id = new Uri("http://services.odata.org/OData/OData.svc/Products(0)")};
            productItem.Properties = new ODataProperty[] {new ODataProperty() {Name = "Id", Value = 0}};

            var categoryNavigationLink = new ODataNestedResourceInfo() {Name = "Category"};

            var categoryItem = new ODataResource() { Id = new Uri("http://services.odata.org/OData/OData.svc/Categories(0)") };
            categoryItem.Properties = new ODataProperty[] { new ODataProperty() { Name = "Id", Value = 0 } };

            var productsNavigationLink = new ODataNestedResourceInfo() { Name = "Products" };

            var supplierNavigationLink = new ODataNestedResourceInfo() { Name = "Supplier" };

            var testODataReader = new TestODataReader()
            {
               new TestODataReaderItem(ODataReaderState.ResourceSetStart, initialFeed),
               new TestODataReaderItem(ODataReaderState.ResourceStart, productItem),
               new TestODataReaderItem(ODataReaderState.NestedResourceInfoStart, categoryNavigationLink),
               new TestODataReaderItem(ODataReaderState.ResourceStart, categoryItem),
               new TestODataReaderItem(ODataReaderState.NestedResourceInfoStart, productsNavigationLink),
               new TestODataReaderItem(ODataReaderState.NestedResourceInfoEnd, productsNavigationLink),
               new TestODataReaderItem(ODataReaderState.ResourceEnd, categoryItem),
               new TestODataReaderItem(ODataReaderState.NestedResourceInfoEnd, categoryNavigationLink),
               new TestODataReaderItem(ODataReaderState.NestedResourceInfoStart, supplierNavigationLink),
               new TestODataReaderItem(ODataReaderState.NestedResourceInfoEnd, supplierNavigationLink),
               new TestODataReaderItem(ODataReaderState.ResourceEnd, productItem),
               new TestODataReaderItem(ODataReaderState.ResourceSetEnd, initialFeed),
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

            readCounter.Should().Be(1);
        }
    }   
}
