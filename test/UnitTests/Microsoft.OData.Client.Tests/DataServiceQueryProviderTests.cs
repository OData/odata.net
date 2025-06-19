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
        public void TranslatesEnumerableContainsWithEnumToInOperator()
        {
            // Arrange
            var sut = new DataServiceQueryProvider(dsc);
            IEnumerable<Color> productColors = new List<Color> { Color.None, Color.Blue, Color.Green };
            var products = dsc.CreateQuery<Product>("Products")
                .Where(product => productColors.Contains(product.Color));

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=Color in ('None','Blue','Green')", queryComponents.Uri.ToString());
        }

        [Fact]
        public void TranslatesEnumerableContainsWithSpecialCharactersToInOperator()
        {
            // Arrange
            var sut = new DataServiceQueryProvider(dsc);
            var productNames = new[] { "Mac & Cheese" };
            var products = dsc.CreateQuery<Product>("Products")
                .Where(product => productNames.Contains(product.Name));

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=Name in ('Mac %26 Cheese')", queryComponents.Uri.ToString());
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
            Assert.Equal(SRResources.ALinq_ContainsNotValidOnEmptyCollection, exception.Message);
        }

        [Fact]
        public void ThrowsForEnumerableContainsWithEmptyEnumCollection()
        {
            // Arrange
            var sut = new DataServiceQueryProvider(dsc);
            var products = dsc.CreateQuery<Product>("Products")
                .Where(product => Enumerable.Empty<Color>().Contains(product.Color));

            // Act
            var exception = Assert.ThrowsAny<InvalidOperationException>(() => sut.Translate(products.Expression));

            // Assert
            Assert.Equal(SRResources.ALinq_ContainsNotValidOnEmptyCollection, exception.Message);
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
        public void TranslatesEnumerableContainsWithSingleEnumItem()
        {
            // Arrange
            var sut = new DataServiceQueryProvider(dsc);
            var products = dsc.CreateQuery<Product>("Products")
                .Where(product => new[] { Color.None }.Contains(product.Color));

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=Color in ('None')", queryComponents.Uri.ToString());
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
        public void TranslatesEnumerableNotContainsToNotInOperator()
        {
            // Arrange
            var sut = new DataServiceQueryProvider(dsc);
            var productNames = new[] { "Milk", "Cheese", "Donut" };
            var products = dsc.CreateQuery<Product>("Products")
                .Where(product => !productNames.Contains(product.Name));

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=not Name in ('Milk','Cheese','Donut')", queryComponents.Uri.ToString());
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

        #region CustomFunction tests

        [Fact]
        public void TranslatesStaticFunction()
        {
            // Arrange
            var localDsc = new DataServiceContext(new Uri("http://root"), ODataProtocolVersion.V4);
            localDsc.ResolveName = (t) => "ServiceNamespace.Product";
            var sut = new DataServiceQueryProvider(localDsc);
            var products = localDsc.CreateQuery<Product>("Products")
                .Where(product => Product.StaticFunction(product.Name));

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=ServiceNamespace.StaticFunction(parameter=$it/Name)", queryComponents.Uri.ToString());
        }

        [Fact]
        public void TranslatesInstanceFunction()
        {
            // Arrange
            var localDsc = new DataServiceContext(new Uri("http://root"), ODataProtocolVersion.V4);
            localDsc.ResolveName = (t) => "ServiceNamespace.Product";
            var sut = new DataServiceQueryProvider(localDsc);
            var products = localDsc.CreateQuery<Product>("Products")
                .Where(product => product.InstanceFunction(product.Name));

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=$it/ServiceNamespace.InstanceFunction(parameter=$it/Name)", queryComponents.Uri.ToString());
        }

        [Fact]
        public void TranslatesStaticFunctionWhenLast()
        {
            // Arrange
            var localDsc = new DataServiceContext(new Uri("http://root"), ODataProtocolVersion.V4);
            localDsc.ResolveName = (t) => "ServiceNamespace.Product";
            var sut = new DataServiceQueryProvider(localDsc);
            var products = localDsc.CreateQuery<Product>("Products")
                .Where(product => true && Product.StaticFunction(product.Name));

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=true and ServiceNamespace.StaticFunction(parameter=$it/Name)", queryComponents.Uri.ToString());
        }

        [Fact]
        public void TranslatesInstanceFunctionWhenLast()
        {
            // Arrange
            var localDsc = new DataServiceContext(new Uri("http://root"), ODataProtocolVersion.V4);
            localDsc.ResolveName = (t) => "ServiceNamespace.Product";
            var sut = new DataServiceQueryProvider(localDsc);
            var products = localDsc.CreateQuery<Product>("Products")
                .Where(product => true && product.InstanceFunction(product.Name));

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=true and $it/ServiceNamespace.InstanceFunction(parameter=$it/Name)", queryComponents.Uri.ToString());
        }

        #endregion

        #region CustomUriFunction tests

        [Fact]
        public void TranslatesInstanceUriFunction()
        {
            // Arrange - products selling more than 1000 in year 2022
            var localDsc = new DataServiceContext(new Uri("http://root"), ODataProtocolVersion.V4);
            var sut = new DataServiceQueryProvider(localDsc);
            var products = localDsc.CreateQuery<Product>("Products")
                .Where(product => product.YearSale(2022) > 1000);

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=sale($it,2022) gt 1000", queryComponents.Uri.ToString());
        }

        [Fact]
        public void TranslatesInstanceUriFunctionOfProperty()
        {
            // Arrange - products selling more than 1000 the year it was launched
            var localDsc = new DataServiceContext(new Uri("http://root"), ODataProtocolVersion.V4);
            var sut = new DataServiceQueryProvider(localDsc);
            var products = localDsc.CreateQuery<Product>("Products")
                .Where(product => product.YearSale(product.LaunchDate.Year) > 1000);

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=sale($it,year(LaunchDate)) gt 1000", queryComponents.Uri.ToString());
        }

        [Fact]
        public void TranslatesStaticUriFunction()
        {
            // Arrange - products launched 7 days ago, evaluated on server
            var localDsc = new DataServiceContext(new Uri("http://root"), ODataProtocolVersion.V4);
            var sut = new DataServiceQueryProvider(localDsc);
            var products = localDsc.CreateQuery<Product>("Products")
                .Where(product => product.LaunchDate == UriFunctions.ServerDate(UriFunctions.ServerNow() - TimeSpan.FromDays(7)));

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=LaunchDate eq date(now() sub duration'P7D')", queryComponents.Uri.ToString());
        }

        [Fact]
        public void TranslatesStaticUriFunctionCanResolve()
        {
            // Arrange - client evaluated Even()
            var localDsc = new DataServiceContext(new Uri("http://root"), ODataProtocolVersion.V4);
            var sut = new DataServiceQueryProvider(localDsc);
            var products = localDsc.CreateQuery<Product>("Products")
                .Where(product => UriFunctions.Even(2));

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=true", queryComponents.Uri.ToString());
        }

        [Fact]
        public void TranslatesStaticUriFunctionOfProperty()
        {
            // Arrange - products with Even Id
            var localDsc = new DataServiceContext(new Uri("http://root"), ODataProtocolVersion.V4);
            var sut = new DataServiceQueryProvider(localDsc);
            var products = localDsc.CreateQuery<Product>("Products")
                .Where(product => UriFunctions.Even(product.Id));

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=Even(Id)", queryComponents.Uri.ToString());
        }

        [Fact]
        public void TranslatesInstanceUriFunctionOfProperty2()
        {
            // Arrange - products with Even Id
            var localDsc = new DataServiceContext(new Uri("http://root"), ODataProtocolVersion.V4);
            var sut = new DataServiceQueryProvider(localDsc);
            var products = localDsc.CreateQuery<Product>("Products")
                .Where(product => product.Test(product.Name));

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=Test($it,Name)", queryComponents.Uri.ToString());
        }

        [Fact]
        public void TranslatesInstanceUriFunctionOfProperty3()
        {
            Product clientProduct = new Product();
            // Arrange - products with Even Id
            var localDsc = new DataServiceContext(new Uri("http://root"), ODataProtocolVersion.V4);
            var sut = new DataServiceQueryProvider(localDsc);
            var products = localDsc.CreateQuery<Product>("Products")
                .Where(product => clientProduct.Test(""));

            // Act
            var queryComponents = sut.Translate(products.Expression);

            // Assert
            Assert.Equal(@"http://root/Products?$filter=true", queryComponents.Uri.ToString());
        }

        #endregion

        [EntityType]
        [Key(nameof(Id))]
        private class Product
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public decimal Price { get; set; }

            public Color Color { get; set; }

            public Edm.Date LaunchDate { get; set; }

            public IEnumerable<string> Comments { get; set; }

            public static bool StaticFunction(string parameter) { return true; }

            public bool InstanceFunction(string parameter) { return true; }

            [UriFunction, OriginalName("sale")]
            public int YearSale(int year) => throw new NotSupportedException();

            [UriFunction(true)]
            public bool Test(string data)
            {
                return string.IsNullOrEmpty(data);
            }
        }

        private enum Color
        {
            None = 0,
            Red = 1,
            Green = 2,
            Blue = 3
        }

        private static class UriFunctions
        {
            [UriFunction, OriginalName("now")]
            public static DateTimeOffset ServerNow() => throw new NotSupportedException();

            [UriFunction, OriginalName("date")]
            public static Edm.Date ServerDate(DateTimeOffset value) => throw new NotSupportedException();

            [UriFunction(true)]
            public static bool Even(int value)
            {
                return value % 2 == 0;
            }
        }
    }
}
