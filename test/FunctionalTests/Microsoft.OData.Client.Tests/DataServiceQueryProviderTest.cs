//---------------------------------------------------------------------
// <copyright file="DataServiceQueryProviderTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using Xunit;

namespace Microsoft.OData.Client.Tests
{
    /// <summary>
    /// Tests to verify that LINQ expressions can be translated to URIs.
    /// </summary>
    public class DataServiceQueryProviderTest
    {
        private DataServiceContext dsc;
 
        public DataServiceQueryProviderTest()
        {
            dsc = new DataServiceContext(new Uri("http://root"), ODataProtocolVersion.V4);
        }

        [Fact]
        public void StringContainsTranslatesToContainsFunction()
        {
            // Arrange
            var sut = new DataServiceQueryProvider(dsc);
            var search = "Strawberry";
            var products = dsc.CreateQuery<Product>("Products")
                .Where(product => product.Name.Contains(search));

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=contains(Name,'Strawberry')", queryComponents.Uri.ToString());
        }


        [Fact]
        public void EnumerableContainsTranslatesToInOperator()
        {
            // Arrange
            var sut = new DataServiceQueryProvider(dsc);
            var productNames = new[] { "Milk", "Cheese", "Donut" };
            var products = dsc.CreateQuery<Product>("Products")
                .Where(product => productNames.Contains(product.Name));

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=Name in ('Milk','Cheese','Donut')", queryComponents.Uri.ToString());
        }

        [EntityType]
        [Key(nameof(Id))]
        private class Product
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}
