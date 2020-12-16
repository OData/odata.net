//---------------------------------------------------------------------
// <copyright file="DataServiceQueryProviderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Microsoft.OData.Client.Tests
{
    /// <summary>
    /// Tests to verify that LINQ expressions can be translated to URIs.
    /// </summary>
    public class DataServiceQueryProviderTests
    {
        private readonly DataServiceContext dsc;
 
        public DataServiceQueryProviderTests()
        {
            dsc = new DataServiceContext(new Uri("http://root"), ODataProtocolVersion.V4);
        }

        #region String.Contains tests

        [Fact]
        public void TranslatesStringContainsToContainsFunction()
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

        #endregion

        #region Enumerable.Contains tests

        [Fact]
        public void TranslatesEnumerableContainsToInOperator()
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

        [Fact]
        public void ThrowsForEnumerableContainsWithEmptyCollection()
        {
            // Arrange
            var sut = new DataServiceQueryProvider(dsc);
            var products = dsc.CreateQuery<Product>("Products")
                .Where(product => Enumerable.Empty<string>().Contains(product.Name));

            // Act
            var exception = Assert.ThrowsAny<InvalidOperationException>(() => sut.Translate(products.Expression));

            // Assert
            Assert.Equal(Strings.ALinq_ContainsNotValidOnEmptyCollection, exception.Message);
        }

        [Fact]
        public void TranslatesEnumerableContainsWithSingleItem()
        {
            // Arrange
            var sut = new DataServiceQueryProvider(dsc);
            var products = dsc.CreateQuery<Product>("Products")
                .Where(product => new[] { "Pancake mix" }.Contains(product.Name));

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=Name in ('Pancake mix')", queryComponents.Uri.ToString());
        }

        [Fact]
        public void TranslatesEnumerableContainsWithOtherClauses()
        {
            // Arrange
            var sut = new DataServiceQueryProvider(dsc);
            var products = dsc.CreateQuery<Product>("Products")
                .Where(product => new[] { "Milk", "Flour" }.Contains(product.Name) && product.Price < 5);

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=Name in ('Milk','Flour') and Price lt 5", queryComponents.Uri.ToString());
        }

        [Fact]
        public void EnumerableContainsOnCollectionValuedPropertiesWithConstantIsNotSupported()
        {
            // Arrange
            var sut = new DataServiceQueryProvider(dsc);
            var products = dsc.CreateQuery<Product>("Products")
                .Where(product => product.Comments.Contains("buy"));

            // Act
            var exception = Assert.ThrowsAny<NotSupportedException>(() => sut.Translate(products.Expression));

            // Assert
            Assert.Equal("The method 'Contains' is not supported.", exception.Message);
        }

        [Fact]
        public void EnumerableContainsOnCollectionValuedPropertiesWithMemberAccessIsNotSupported()
        {
            // Arrange
            var sut = new DataServiceQueryProvider(dsc);
            var products = dsc.CreateQuery<Product>("Products")
                .Where(product => product.Comments.Contains(product.Name));

            // Act
            var exception = Assert.ThrowsAny<NotSupportedException>(() => sut.Translate(products.Expression));

            // Assert
            Assert.Equal("The method 'Contains' is not supported.", exception.Message);
        }

        #endregion

        [EntityType]
        [Key(nameof(Id))]
        private class Product
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public decimal Price { get; set; }

            public IEnumerable<string> Comments { get; set; }
        }
    }
}
