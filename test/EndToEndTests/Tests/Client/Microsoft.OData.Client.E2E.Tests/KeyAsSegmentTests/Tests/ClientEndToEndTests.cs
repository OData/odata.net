//---------------------------------------------------------------------
// <copyright file="ClientEndToEndTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;
using Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Server;
using Microsoft.OData.Edm;
using Xunit;
using Employee = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Employee;
using Person = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Person;
using SpecialEmployee = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.SpecialEmployee;
using DiscontinuedProduct = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.DiscontinuedProduct;
using ProductPhoto = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.ProductPhoto;
using Customer = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Customer;
using Order = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Order;

namespace Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Tests;

public class ClientEndToEndTests : EndToEndTestBase<ClientEndToEndTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(KeyAsSegmentTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel()));
        }
    }

    public ClientEndToEndTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
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

        _model = CommonEndToEndEdmModel.GetEdmModel();
        ResetDefaultDataSource();
    }

    [Fact]
    public void GetSingleEntity()
    {
        // Arrange & Act
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

        var queryByKey = _context.CreateQuery<Person>("People").ByKey(-10);
        var personByKey = queryByKey.GetValue();

        var query = _context.CreateQuery<Person>("People").Where(p => p.PersonId == -10) as DataServiceQuery<Person>;

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
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

        var queryByKey = _context.People.ByKey(-10);
        var personByKey = queryByKey.GetValue();

        var query = _context.People.Where(p => p.PersonId == -10);

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
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

        var query =
            from p in _context.People
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
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

        var query =
            from p in _context.People
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
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

        var query = (
            from p in _context.People.OfType<Employee>()
            where p.PersonId == -6
            select p) as DataServiceQuery<Employee>;

        // Act
        var employee = query?.FirstOrDefault();

        // Assert
        Assert.IsType<Employee>(employee);
        Assert.EndsWith("odata/People/$/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee?$filter=PersonId eq -6", query?.ToString());
        Assert.NotNull(employee);
        Assert.Equal("vnqfkvpolnxvurgxpfbfquqrqxqxknjykkuapsqcmbeuslhkqufultvr", employee.Name);
        Assert.Equal(2147483647, employee.Salary);
    }

    [Fact]
    public void MultipleNavigationAndOfTypeInQuery()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

        var query = (
            from product in _context.Products.OfType<DiscontinuedProduct>()
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
            "odata/Products/$/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct/-9/RelatedProducts/$/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct/-9/Photos?$filter=PhotoId eq -4 and ProductId eq -4",
            query?.ToString());

        Assert.NotNull(photoResult);
        Assert.Equal(-4, photoResult.PhotoId);
        Assert.Equal(-4, photoResult.ProductId);
    }

    [Fact]
    public void AttachTo()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

        var person = new Person { PersonId = -10 };

        // Act
        _context.AttachTo("Person", person);

        // Assert
        Assert.Single(_context.Entities);
        Assert.Equal(-10, (_context.Entities[0].Entity as Person)?.PersonId);
    }

    [Fact]
    public void AttachToLoadProperty()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

        var person = new Person { PersonId = -10 };

        // Act
        _context.AttachTo("People", person);
        _context.LoadProperty(person, "PersonMetadata");

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
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

        var response = _context.Customers.Expand(c => c.Orders).Execute() as QueryOperationResponse<Customer>;
        DataServiceQueryContinuation<Customer>? customerContinuation = null;

        do
        {
            if (customerContinuation != null)
            {
                response = _context.Execute<Customer>(customerContinuation);
            }

            foreach (var customer in response)
            {
                DataServiceQueryContinuation<Order> orderContinuation = response.GetContinuation(customer.Orders);

                while (orderContinuation != null)
                {
                    var ordersResponse = _context.LoadProperty(customer, "Orders", orderContinuation);
                    orderContinuation = ordersResponse.GetContinuation();
                }
            }

        } while ((customerContinuation = response.GetContinuation()) != null);
    }

    #region Private methods

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "keyassegmenttests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
