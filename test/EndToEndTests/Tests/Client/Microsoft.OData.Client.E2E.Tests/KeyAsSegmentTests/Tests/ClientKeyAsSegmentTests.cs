//---------------------------------------------------------------------
// <copyright file="ClientUrlConventionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Server;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd;
using Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;
using Xunit;
using Customer = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Customer;
using DiscontinuedProduct = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.DiscontinuedProduct;
using Employee = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Employee;
using Order = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Order;
using OrderLine = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.OrderLine;
using Person = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Person;
using ProductPhoto = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.ProductPhoto;
using SpecialEmployee = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.SpecialEmployee;

namespace Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Tests;

public class ClientKeyAsSegmentTests : EndToEndTestBase<ClientKeyAsSegmentTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(KeyAsSegmentTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel()));
        }
    }

    public ClientKeyAsSegmentTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");

        _context = new Container(_baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        ResetDefaultDataSource();
    }

    [Fact]
    public void ClientChangesUrlConventionsBetweenQueries()
    {
        // Arrange & Act & Assert
        var contextWrapper = CreateWrappedContext();

        var query = contextWrapper.CreateQuery<Customer>("Customers").OrderBy(c => c.CustomerId).ToList();
        Assert.Equal(10, query.Count);

        contextWrapper.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var queryWithDefaultKeys = contextWrapper.CreateQuery<Customer>("Customers").OrderBy(c => c.CustomerId).ToList();
        Assert.Equal(10, queryWithDefaultKeys.Count);
        Assert.Equal(query, queryWithDefaultKeys);

        contextWrapper.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        query = contextWrapper.CreateQuery<Customer>("Customers").OrderBy(c => c.CustomerId).ToList();
        Assert.Equal(10, query.Count);
        Assert.Equal(query, queryWithDefaultKeys);
    }

    [Fact]
    public void GetSingleEntityWithKeyAsSegment()
    {
        // Arrange & Act
        var contextWrapper = CreateWrappedContext();

        var queryByKey = contextWrapper.CreateQuery<Person>("People").ByKey(-10);
        var personByKey = queryByKey.GetValue();

        var query = contextWrapper.CreateQuery<Person>("People").Where(p => p.PersonId == -10) as DataServiceQuery<Person>;

        // Assert
        Assert.EndsWith("/odata/People/-10", queryByKey.RequestUri.OriginalString);
        Assert.EndsWith("/odata/People?$filter=PersonId eq -10", query?.RequestUri.OriginalString);
        Assert.NotNull(personByKey);
        Assert.Equal(personByKey, query?.SingleOrDefault());

        Assert.Equal("ぺソぞ弌タァ匚タぽひハ欲ぴほ匚せまたバボチマ匚ぁゾソチぁЯそぁミя暦畚ボ歹ひЯほダチそЯせぽゼポЯチａた歹たをタマせをせ匚ミタひぜ畚暦グクひほそたグせяチ匚ｦぺぁ", personByKey.Name);
        Assert.Equal(4091, (personByKey as Employee)?.Salary);
        Assert.Equal(-37730565, (personByKey as SpecialEmployee)?.Bonus);
    }

    [Fact]
    public void LinqQueryWithKeyUsingMethodSyntax()
    {
        // Arrange & Act
        var contextWrapper = CreateWrappedContext();

        var queryByKey = contextWrapper.People.ByKey(-10);
        var personByKey = queryByKey.GetValue();

        var query = contextWrapper.People.Where(p => p.PersonId == -10);

        // Assert
        Assert.EndsWith("/odata/People/-10", queryByKey.RequestUri.OriginalString);
        Assert.EndsWith("/odata/People?$filter=PersonId eq -10", query?.ToString());
        Assert.NotNull(personByKey);
        Assert.Equal(personByKey, query?.SingleOrDefault());

        Assert.Equal("ぺソぞ弌タァ匚タぽひハ欲ぴほ匚せまたバボチマ匚ぁゾソチぁЯそぁミя暦畚ボ歹ひЯほダチそЯせぽゼポЯチａた歹たをタマせをせ匚ミタひぜ畚暦グクひほそたグせяチ匚ｦぺぁ", personByKey.Name);
        Assert.Equal(4091, (personByKey as Employee)?.Salary);
        Assert.Equal(-37730565, (personByKey as SpecialEmployee)?.Bonus);
    }

    [Fact]
    public void LinqQueryWithNullStringInKey()
    {
        // Arrange
        var contextWrapper = CreateWrappedContext();

        var query =
            from p in contextWrapper.People
            where p.Name == "\0te\0st\0"
            select p;

        // Act
        var person = query.SingleOrDefault();

        // Assert
        Assert.Contains("/odata/People?$filter=Name eq '%00te%00st%00'", query.ToString());
        Assert.Null(person);
    }

    [Fact]
    public void LinqQueryWithKey()
    {
        // Arrange
        var contextWrapper = CreateWrappedContext();

        var query =
            from p in contextWrapper.People
            where p.PersonId == -10
            select p;

        // Act
        var person = query.SingleOrDefault();

        // Assert
        Assert.Contains("/odata/People?$filter=PersonId eq -10", query.ToString());
        Assert.NotNull(person);

        Assert.Equal("ぺソぞ弌タァ匚タぽひハ欲ぴほ匚せまたバボチマ匚ぁゾソチぁЯそぁミя暦畚ボ歹ひЯほダチそЯせぽゼポЯチａた歹たをタマせをせ匚ミタひぜ畚暦グクひほそたグせяチ匚ｦぺぁ", person.Name);
        Assert.Equal(4091, (person as Employee)?.Salary);
        Assert.Equal(-37730565, (person as SpecialEmployee)?.Bonus);
    }

    [Fact]
    public void LinqQueryWithKeyAndOfType()
    {
        // Arrange
        var contextWrapper = CreateWrappedContext();

        var query = (
            from p in contextWrapper.People.OfType<Employee>()
            where p.PersonId == -6
            select p) as DataServiceQuery<Employee>;

        // Act
        var employee = query?.FirstOrDefault();

        // Assert
        Assert.IsType<Employee>(employee);
        Assert.EndsWith("odata/People/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee?$filter=PersonId eq -6", query?.ToString());
        Assert.NotNull(employee);
        Assert.Equal("vnqfkvpolnxvurgxpfbfquqrqxqxknjykkuapsqcmbeuslhkqufultvr", employee.Name);
        Assert.Equal(2147483647, employee.Salary);
    }

    [Fact]
    public void MultipleNavigationAndOfTypeInQuery()
    {
        // Arrange
        var contextWrapper = CreateWrappedContext();

        var query = (
            from product in contextWrapper.Products.OfType<DiscontinuedProduct>()
            from related in product.RelatedProducts.OfType<DiscontinuedProduct>()
            from photo in related.Photos
            where
                product.ProductId == -9 &&
                related.ProductId == -9 &&
                photo.PhotoId == -4 &&
                photo.ProductId == -4

            select photo) as IQueryable<ProductPhoto>;

        // Act
        var photoResult = query?.SingleOrDefault();

        // Assert
        Assert.EndsWith(
            "odata/Products/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.DiscontinuedProduct/-9/RelatedProducts/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.DiscontinuedProduct/-9/Photos?$filter=PhotoId eq -4 and ProductId eq -4",
            query?.ToString());

        Assert.NotNull(photoResult);
        Assert.Equal(-4, photoResult.PhotoId);
        Assert.Equal(-4, photoResult.ProductId);
    }

    [Fact]
    public void MultipleNavigationAndOfTypeInQuery_VerifyQuery()
    {
        // Arrange
        var contextWrapper = CreateWrappedContext();
        contextWrapper.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

        // Act
        var query = (
            from product in contextWrapper.Products.OfType<DiscontinuedProduct>()
            from related in product.RelatedProducts.OfType<DiscontinuedProduct>()
            from photo in related.Photos
            where
                product.ProductId == -9 &&
                related.ProductId == -9 &&
                photo.PhotoId == -4 &&
                photo.ProductId == -4

            select photo) as IQueryable<ProductPhoto>;

        // Assert
        Assert.EndsWith(
            "/Products/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.DiscontinuedProduct(-9)/RelatedProducts/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.DiscontinuedProduct(-9)/Photos?$filter=PhotoId eq -4 and ProductId eq -4",
            query?.ToString());
    }

    [Fact]
    public void AttachToWithKeyAsSegment()
    {
        // Arrange
        var contextWrapper = CreateWrappedContext();

        var person = new Person { PersonId = -10 };

        // Act
        contextWrapper.AttachTo("Person", person);

        // Assert
        Assert.Single(contextWrapper.Entities);
        Assert.Equal(-10, (contextWrapper.Entities[0].Entity as Person)?.PersonId);
    }

    [Fact]
    public void AttachToAndLoadPropertyWithKeyAsSegment()
    {
        // Arrange
        var contextWrapper = CreateWrappedContext();

        var person = new Person { PersonId = -10 };

        // Act
        contextWrapper.AttachTo("People", person);
        contextWrapper.LoadProperty(person, "PersonMetadata");

        // Assert
        Assert.Equal(3, person.PersonMetadata.Count);
        Assert.Equal("ysjrkvxlmdiddnrpxvnizyqvsfurnvhiugqyukiyedbrzgpqlevdfeqainzoauyqvzkx", person.PersonMetadata.Where(p => p.PersonMetadataId == -7).Single().PropertyValue);
        Assert.Equal("ァ亜ぽﾈソぽひァミａ弌ゾダソポぼタ黑歹九ぁんЯﾝёゼミァ弌タ九ｦぞチポポЯぺｚたダゾゾﾝミポチａタマぴ欲яﾈタЯ亜まａあ", person.PersonMetadata.Where(p => p.PersonMetadataId == -9).Single().PropertyValue);
        Assert.Equal("lazcbjlydpauujlvßgszchoxhycaryzbmkuskiqfxyiu", person.PersonMetadata.Where(p => p.PersonMetadataId == -10).Single().PropertyValue);
    }

    [Fact]
    public void LoadPropertyWithNextLink()
    {
        // Arrange
        var contextWrapper = CreateWrappedContext();

        var response = contextWrapper.Customers.Expand(c => c.Orders).Execute() as QueryOperationResponse<Customer>;
        DataServiceQueryContinuation<Customer>? customerContinuation = null;

        do
        {
            if (customerContinuation != null)
            {
                response = contextWrapper.Execute(customerContinuation);
            }

            foreach (var customer in response)
            {
                DataServiceQueryContinuation<Order> orderContinuation = response.GetContinuation(customer.Orders);

                while (orderContinuation != null)
                {
                    var ordersResponse = contextWrapper.LoadProperty(customer, "Orders", orderContinuation);
                    orderContinuation = ordersResponse.GetContinuation();
                }
            }

        } while ((customerContinuation = response.GetContinuation()) != null);
    }


    [Fact]
    public void ClientWithKeyAsSegmentSendsRequestsToServerWithoutKeyAsSegment()
    {
        // Arrange
        var contextWrapper = CreateWrappedContext();

        // Act
        var queryable = contextWrapper.Orders.ByKey(0);
        var exception = Record.Exception(() => queryable.GetValue());

        // Assert
        Assert.EndsWith("/Orders/0", queryable.RequestUri.OriginalString);
        Assert.NotNull(exception.InnerException);
        Assert.IsType<DataServiceClientException>(exception.InnerException);
    }

    [Fact]
    public void UrlKeyDelimiter_WhereAndByKey_SingleKey()
    {
        // Arrange
        var contextParentheses = CreateWrappedContext();
        contextParentheses.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

        var contextSlash = CreateWrappedContext();
        contextSlash.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

        // Act & Assert
        var whereParentheses = contextParentheses.Customers.Where(c => c.CustomerId == 1) as DataServiceQuery<Customer>;
        var whereSlash = contextSlash.Customers.Where(c => c.CustomerId == 1) as DataServiceQuery<Customer>;
        Assert.EndsWith("/Customers?$filter=CustomerId eq 1", whereParentheses?.RequestUri.OriginalString);
        Assert.EndsWith("/Customers?$filter=CustomerId eq 1", whereSlash?.RequestUri.OriginalString);
        Assert.Equal(whereParentheses?.RequestUri.OriginalString, whereSlash?.RequestUri.OriginalString);

        // BYKEY: Parentheses delimiter should use (key)
        var byKeyParentheses = contextParentheses.Customers.ByKey(1);
        Assert.EndsWith("/Customers(1)", byKeyParentheses.RequestUri.OriginalString);

        // BYKEY: Slash delimiter should use /key
        var byKeySlash = contextSlash.Customers.ByKey(1);
        Assert.EndsWith("/Customers/1", byKeySlash.RequestUri.OriginalString);
    }

    [Fact]
    public void UrlKeyDelimiter_WhereAndByKey_CompositeKey()
    {
        // Arrange
        var contextParentheses = CreateWrappedContext();
        contextParentheses.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

        var contextSlash = CreateWrappedContext();
        contextSlash.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

        // Act & Assert
        // WHERE: Should always generate $filter regardless of delimiter
        var whereParentheses = contextParentheses.CreateQuery<OrderLine>("OrderLines").Where(ol => ol.OrderId == 1 && ol.ProductId == 2) as DataServiceQuery<OrderLine>;
        var whereSlash = contextSlash.CreateQuery<OrderLine>("OrderLines").Where(ol => ol.OrderId == 1 && ol.ProductId == 2) as DataServiceQuery<OrderLine>;
        Assert.EndsWith("/OrderLines?$filter=OrderId eq 1 and ProductId eq 2", whereParentheses?.RequestUri.OriginalString);
        Assert.EndsWith("/OrderLines?$filter=OrderId eq 1 and ProductId eq 2", whereSlash?.RequestUri.OriginalString);
        Assert.Equal(whereParentheses?.RequestUri.OriginalString, whereSlash?.RequestUri.OriginalString);

        // BYKEY: Parentheses delimiter should use (key1=val1,key2=val2)
        var byKeyParentheses = contextParentheses.CreateQuery<OrderLine>("OrderLines").ByKey(new Dictionary<string, object>
        {
            { "OrderId", 1 },
            { "ProductId", 2 }
        });
        Assert.EndsWith("/OrderLines(OrderId=1,ProductId=2)", byKeyParentheses.RequestUri.OriginalString);

        // BYKEY: Slash delimiter should use /key1=val1,key2=val2
        var byKeySlash = contextSlash.CreateQuery<OrderLine>("OrderLines").ByKey(new Dictionary<string, object>
        {
            { "OrderId", 1 },
            { "ProductId", 2 }
        });
        Assert.EndsWith("/OrderLines/OrderId=1,ProductId=2", byKeySlash.RequestUri.OriginalString);
    }

    #region Private methods

    private Container CreateWrappedContext()
    {
        var context = new Container(_baseUri)
        {
            HttpClientFactory = HttpClientFactory,
            UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash
        };

        return context;
    }

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "keyassegmenttests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
