//---------------------------------------------------------------------
// <copyright file="QueryOptionOnCollectionTypePropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.QueryOptionTests
{
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.OData;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Xunit;

    public class QueryOptionOnCollectionTypePropertyTests : ODataWCFServiceTestsBase<InMemoryEntities>
    {

        public QueryOptionOnCollectionTypePropertyTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        private QueryOptionTestsHelper TestsHelper
        {
            get
            {
                return new QueryOptionTestsHelper(ServiceBaseUri, Model);
            }
        }

        private const string MimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata;

        #region Test Method

        [Fact]
        public void BasicQueryOptionTest()
        {
            //$skip
            ODataProperty property = this.TestsHelper.QueryProperty("Customers(1)/Numbers?$skip=2", MimeType);
            ODataCollectionValue collectionValue = property.Value as ODataCollectionValue;
            if (collectionValue != null)
            {
                Assert.Equal(3, collectionValue.Items.Cast<object>().Count());
            }

            //$top
            property = this.TestsHelper.QueryProperty("Customers(1)/Numbers?$top=3", MimeType);
            collectionValue = property.Value as ODataCollectionValue;
            if (collectionValue != null)
            {
                Assert.Equal(3, collectionValue.Items.Cast<object>().Count());
            }

            //$orderby
            property = this.TestsHelper.QueryProperty("Customers(1)/Numbers?$orderby=$it", MimeType);
            collectionValue = property.Value as ODataCollectionValue;
            if (collectionValue != null)
            {
                ODataCollectionValue expectedValue = new ODataCollectionValue()
                {
                    TypeName = "Collection(Edm.String)",
                    Items = new [] {"012", "111-111-1111", "310", "ayz", "bca"}
                };
                ODataValueAssertEqualHelper.AssertODataValueEqual(expectedValue, collectionValue);
            }

            //$filter
            property = this.TestsHelper.QueryProperty("Customers(1)/Numbers?$filter=$it eq '012'", MimeType);
            collectionValue = property.Value as ODataCollectionValue;
            if (collectionValue != null)
            {
                ODataCollectionValue expectedValue = new ODataCollectionValue()
                {
                    TypeName = "Collection(Edm.String)",
                    Items = new[] { "012" }
                };
                ODataValueAssertEqualHelper.AssertODataValueEqual(expectedValue, collectionValue);
            }
        }

        [Fact]
        public void MiscQueryOptionTest()
        {
            ODataProperty property = this.TestsHelper.QueryProperty("Customers(1)/Numbers?$orderby=$it&$top=1", MimeType);
            ODataCollectionValue collectionValue = property.Value as ODataCollectionValue;
            if (collectionValue != null)
            {
                ODataCollectionValue expectedValue = new ODataCollectionValue()
                {
                    TypeName = "Collection(Edm.String)",
                    Items = new[] { "012" }
                };
                ODataValueAssertEqualHelper.AssertODataValueEqual(expectedValue, collectionValue);
            }

            property = this.TestsHelper.QueryProperty("Customers(1)/Numbers?$skip=1&$filter=contains($it,'a')", MimeType);
            collectionValue = property.Value as ODataCollectionValue;
            if (collectionValue != null)
            {
                ODataCollectionValue expectedValue = new ODataCollectionValue()
                {
                    TypeName = "Collection(Edm.String)",
                    Items = new[] { "ayz" }
                };
                ODataValueAssertEqualHelper.AssertODataValueEqual(expectedValue, collectionValue);
            }
        }

        [Fact]
        public void FilterOnCollectionCountTest()
        {
            var person = TestClientContext.People.Where(p => p.Emails.Count == 2) as DataServiceQuery<Person>;
            Assert.True(person.RequestUri.OriginalString.EndsWith("/People?$filter=Emails/$count eq 2"));

            var product = TestClientContext.Products.Where(p => p.CoverColors.Count == 2) as DataServiceQuery<Product>;
            Assert.True(product.RequestUri.OriginalString.EndsWith("/Products?$filter=CoverColors/$count eq 2"));

            person = TestClientContext.People.Where(p => p.Addresses.Count == 2) as DataServiceQuery<Person>;
            Assert.True(person.RequestUri.OriginalString.EndsWith("/People?$filter=Addresses/$count eq 2"));

            var custoemers = TestClientContext.Customers.Where(p => p.Orders.Count == 2) as DataServiceQuery<Customer>;
            Assert.True(custoemers.RequestUri.OriginalString.EndsWith("/Customers?$filter=Orders/$count eq 2"));
        }

        [Fact]
        public void OrderbyOnCollectionCountTest()
        {
            var person = TestClientContext.People.OrderBy(p => p.Emails.Count) as DataServiceQuery<Person>;
            Assert.True(person.RequestUri.OriginalString.EndsWith("/People?$orderby=Emails/$count"));

            person = TestClientContext.People.OrderByDescending(p => p.Emails.Count) as DataServiceQuery<Person>;
            Assert.True(person.RequestUri.OriginalString.EndsWith("/People?$orderby=Emails/$count desc"));

            var products = TestClientContext.Products.OrderBy(p => p.CoverColors.Count) as DataServiceQuery<Product>;
            Assert.True(products.RequestUri.OriginalString.EndsWith("/Products?$orderby=CoverColors/$count"));

            person = TestClientContext.People.OrderBy(p => p.Addresses.Count) as DataServiceQuery<Person>;
            Assert.True(person.RequestUri.OriginalString.EndsWith("/People?$orderby=Addresses/$count"));

            var custoemers = TestClientContext.Customers.OrderBy(p => p.Orders.Count) as DataServiceQuery<Customer>;
            Assert.True(custoemers.RequestUri.OriginalString.EndsWith("/Customers?$orderby=Orders/$count"));
        }

        #endregion
    }
}
