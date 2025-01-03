//---------------------------------------------------------------------
// <copyright file="SingletonQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.TestCommon.Common;
using Microsoft.OData.Client.E2E.TestCommon.Helpers;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;
using Microsoft.OData.Client.E2E.Tests.SingletonTests.Server;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.SingletonTests.Tests;

public class SingletonQueryTests : EndToEndTestBase<SingletonQueryTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(SingletonTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
        }
    }

    public SingletonQueryTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
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

        _model = DefaultEdmModel.GetEdmModel();
        ResetDefaultDataSource();
    }

    public static IEnumerable<object[]> MimeTypesData
    {
        get
        {
            yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata };
            yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata };
            yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata };
        }
    }

    #region Query singleton entity

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task QuerySingleton(string mimeType)
    {
        // Arrange & Act
        var entries = await this.TestsHelper.QueryResourceEntriesAsync("VipCustomer", mimeType);
        var entry = entries.SingleOrDefault(e => e != null && e.TypeName.EndsWith("Customer"));

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.NotNull(entry);

            var personIDProperty = entry.Properties.SingleOrDefault(p => p.Name == "PersonID") as ODataProperty;
            var firstNameProperty = entry.Properties.SingleOrDefault(p => p.Name == "FirstName") as ODataProperty;
            var lastNameProperty = entry.Properties.SingleOrDefault(p => p.Name == "LastName") as ODataProperty;
            var cityProperty = entry.Properties.SingleOrDefault(p => p.Name == "City") as ODataProperty;
            var emailsProperty = entry.Properties.SingleOrDefault(p => p.Name == "Emails") as ODataProperty;
            Assert.NotNull(personIDProperty);
            Assert.NotNull(firstNameProperty);
            Assert.NotNull(lastNameProperty);
            Assert.NotNull(cityProperty);

            Assert.Equal(1, personIDProperty.Value);
            Assert.Equal("Bob", firstNameProperty.Value);
            Assert.Equal("Cat", lastNameProperty.Value);
            Assert.Equal("London", cityProperty.Value);
        }
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task QuerySingletonWhichIsOpenType(string mimeType)
    {
        // Arrange & Act
        var entries = await this.TestsHelper.QueryResourceEntriesAsync("Company", mimeType);
        var entry = entries.SingleOrDefault(e => e != null && e.TypeName.EndsWith("Company"));

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.NotNull(entry);

            var companyIDProperty = entry.Properties.SingleOrDefault(p => p.Name == "CompanyID") as ODataProperty;
            var companyCategoryProperty = entry.Properties.SingleOrDefault(p => p.Name == "CompanyCategory") as ODataProperty;
            var nameProperty = entry.Properties.SingleOrDefault(p => p.Name == "Name") as ODataProperty;
            Assert.NotNull(companyIDProperty);
            Assert.NotNull(companyCategoryProperty);
            Assert.NotNull(nameProperty);

            Assert.Equal(0, companyIDProperty.Value);
            Assert.Equal("IT", companyCategoryProperty.Value.ToString());
            Assert.Equal("MS", nameProperty.Value);
        }
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task QueryDerivedSingletonWithTypeCast(string mimeType)
    {
        // Arrange & Act
        var entries = await this.TestsHelper.QueryResourceEntriesAsync("Boss/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.Customer", mimeType);
        var entry = entries.SingleOrDefault(e => e != null && e.TypeName.EndsWith("Customer"));

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.NotNull(entry);

            var personIDProperty = entry.Properties.SingleOrDefault(p => p.Name == "PersonID") as ODataProperty;
            var firstNameProperty = entry.Properties.SingleOrDefault(p => p.Name == "FirstName") as ODataProperty;
            var lastNameProperty = entry.Properties.SingleOrDefault(p => p.Name == "LastName") as ODataProperty;
            var cityProperty = entry.Properties.SingleOrDefault(p => p.Name == "City") as ODataProperty;
            Assert.NotNull(personIDProperty);
            Assert.NotNull(firstNameProperty);
            Assert.NotNull(lastNameProperty);
            Assert.NotNull(cityProperty);

            Assert.Equal(2, personIDProperty.Value);
            Assert.Equal("Jill", firstNameProperty.Value);
            Assert.Equal("Jones", lastNameProperty.Value);
            Assert.Equal("Sydney", cityProperty.Value);
        }
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task QueryDerivedSingletonWithoutTypeCast(string mimeType)
    {
        // Arrange & Act
        var entries = await this.TestsHelper.QueryResourceEntriesAsync("Boss", mimeType);
        var entry = entries.SingleOrDefault(e => e != null && e.TypeName.EndsWith("Customer"));

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.NotNull(entry);

            var personIDProperty = entry.Properties.SingleOrDefault(p => p.Name == "PersonID") as ODataProperty;
            var firstNameProperty = entry.Properties.SingleOrDefault(p => p.Name == "FirstName") as ODataProperty;
            var cityProperty = entry.Properties.SingleOrDefault(p => p.Name == "City") as ODataProperty;
            Assert.NotNull(personIDProperty);
            Assert.NotNull(firstNameProperty);
            Assert.NotNull(cityProperty);

            Assert.Equal(2, personIDProperty.Value);
            Assert.Equal("Jill", firstNameProperty.Value);
            Assert.Equal("Sydney", cityProperty.Value);
        }
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task QuerySingletonWithExpand(string mimeType)
    {
        // Arrange & Act
        var resources = await this.TestsHelper.QueryResourceEntriesAsync("VipCustomer?$expand=Orders", mimeType);
        var entries = resources.Where(r => r != null && r.Id != null).ToList();

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            var orders = entries.FindAll(e => e.Id.AbsoluteUri.Contains("Orders"));
            Assert.Equal(2, orders.Count);

            var customer = entries.SingleOrDefault(e => e.Id.AbsoluteUri.Contains("VipCustomer"));
            Assert.NotNull(customer);

            var personIDProperty = customer.Properties.SingleOrDefault(p => p.Name == "PersonID") as ODataProperty;
            var firstNameProperty = customer.Properties.SingleOrDefault(p => p.Name == "FirstName") as ODataProperty;
            var cityProperty = customer.Properties.SingleOrDefault(p => p.Name == "City") as ODataProperty;
            Assert.NotNull(personIDProperty);
            Assert.NotNull(firstNameProperty);
            Assert.NotNull(cityProperty);

            Assert.Equal(1, personIDProperty.Value);
            Assert.Equal("Bob", firstNameProperty.Value);
            Assert.Equal("London", cityProperty.Value);
        }
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task QuerySingletonWithSelect(string mimeType)
    {
        // Arrange & Act
        var entries = await this.TestsHelper.QueryResourceEntriesAsync("VipCustomer?$select=PersonID,FirstName", mimeType);
        var customer = entries.SingleOrDefault(e => e != null && e.TypeName.EndsWith("Customer"));

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.NotNull(customer);
            Assert.Equal(2, customer.Properties.Count());
        }
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task QueryDerivedSingletonWithTypeCastAndSelect(string mimeType)
    {
        // Arrange & Act
        var entries = await this.TestsHelper.QueryResourceEntriesAsync("Boss/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.Customer?$select=City", mimeType);
        var customer = entries.SingleOrDefault(e => e != null && e.TypeName.EndsWith("Customer"));

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.NotNull(customer);
            Assert.Single(customer.Properties);
            var cityProperty = customer.Properties.SingleOrDefault(p => p.Name == "City") as ODataProperty;
            Assert.NotNull(cityProperty);
            Assert.Equal("Sydney", cityProperty.Value);
        }
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task QuerySingletonWithSelectUnderExpand(string mimeType)
    {
        // Arrange & Act
        var resources = await this.TestsHelper.QueryResourceEntriesAsync("VipCustomer?$expand=Orders($select=OrderID,OrderDate)", mimeType);
        var entries = resources.Where(r => r != null && r.Id != null).ToList();

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            var orders = entries.FindAll(e => e != null && e.Id.AbsoluteUri.Contains("Orders"));
            Assert.Equal(2, orders.Count);

            foreach (var order in orders)
            {
                Assert.Equal(2, order.Properties.Count());
                Assert.Contains(order.Properties, p => p.Name == "OrderID");
                Assert.Contains(order.Properties, p => p.Name == "OrderDate");
            }

            var customer = entries.SingleOrDefault(e => e != null && e.Id.AbsoluteUri.Contains("VipCustomer"));
            Assert.NotNull(customer);

            var personIDProperty = customer.Properties.SingleOrDefault(p => p.Name == "PersonID") as ODataProperty;
            var firstNameProperty = customer.Properties.SingleOrDefault(p => p.Name == "FirstName") as ODataProperty;
            var cityProperty = customer.Properties.SingleOrDefault(p => p.Name == "City") as ODataProperty;
            Assert.NotNull(personIDProperty);
            Assert.NotNull(firstNameProperty);
            Assert.NotNull(cityProperty);

            Assert.Equal(1, personIDProperty.Value);
            Assert.Equal("Bob", firstNameProperty.Value);
            Assert.Equal("London", cityProperty.Value);
        }
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task QuerySingletonWithMiscQueryOptions(string mimeType)
    {
        // Arrange & Act
        var resources = await this.TestsHelper.QueryResourceEntriesAsync("VipCustomer?$select=FirstName,HomeAddress&$expand=Orders", mimeType);
        var entries = resources.Where(r => r != null && r.Id != null).ToList();

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            var orders = entries.FindAll(e => e.Id.AbsoluteUri.Contains("Orders"));
            Assert.Equal(2, orders.Count);

            var customer = entries.SingleOrDefault(e => e.Id.AbsoluteUri.Contains("VipCustomer"));
            Assert.NotNull(customer);
            Assert.Single(customer.Properties);
            
            var firstNameProperty = customer.Properties.SingleOrDefault(p => p.Name == "FirstName") as ODataProperty;
            Assert.NotNull(firstNameProperty);
            Assert.Equal("Bob", firstNameProperty.Value);

            var homeAddress = resources.SingleOrDefault(r => r != null && r.TypeName.EndsWith("Address"));
            Assert.NotNull(homeAddress);
        }
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task SelectDerivedPropertyWithoutTypeCastShouldFail(string mimeType)
    {
        await this.BadRequestOrNotFoundAsync("Boss?$select=City", mimeType, /* Bad Request */ 400);
    }

    #endregion

    #region Query singleton property

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task QuerySingletonProperty(string mimeType)
    {
        // Arrange & Act
        var property = await this.TestsHelper.QueryPropertyAsync("VipCustomer/PersonID", mimeType);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.NotNull(property);
            Assert.Equal(1, property.Value);
        }
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task QuerySingletonPropertyUnderComplexProperty(string mimeType)
    {
        // Arrange & Act
        var property = await this.TestsHelper.QueryPropertyAsync("VipCustomer/HomeAddress/City", mimeType);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.NotNull(property);
            Assert.Equal("London", property.Value);
        }
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task QuerySingletonEnumProperty(string mimeType)
    {
        // Arrange & Act
        var property = await this.TestsHelper.QueryPropertyAsync("Company/CompanyCategory", mimeType);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.NotNull(property);
            Assert.Equal("IT", property.Value.ToString());
        }
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task QuerySingletonNavigationProperty(string mimeType)
    {
        // Arrange & Act
        var entries = await this.TestsHelper.QueryResourceSetsAsync("VipCustomer/Orders", mimeType);
        var entry = entries.FirstOrDefault();

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.NotNull(entry);
            var orderIdProperty = entry.Properties.Single(p => p.Name == "OrderID") as ODataProperty;
            Assert.NotNull(orderIdProperty);
            Assert.Equal(7, orderIdProperty.Value);
        }
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task QuerySingletonPropertyUnderNavigationProperty(string mimeType)
    {
        // Arrange & Act
        var property = await this.TestsHelper.QueryPropertyAsync("VipCustomer/Orders(8)/OrderDate", mimeType);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.NotNull(property);
            Assert.Equal(new DateTimeOffset(2011, 3, 4, 16, 3, 57, TimeSpan.FromHours(-8)), property.Value);
        }
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task QueryDerivedSingletonPropertyWithTypeCast(string mimeType)
    {
        // Arrange & Act
        var property = await this.TestsHelper.QueryPropertyAsync("Boss/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.Customer/City", mimeType);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.NotNull(property);
            Assert.Equal("Sydney", property.Value);
        }
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task QuerySingletonNavigationPropertyWithFilter(string mimeType)
    {
        // Arrange & Act
        var entries = await this.TestsHelper.QueryResourceSetsAsync("VipCustomer/Orders?$filter=OrderID eq 8", mimeType);
        var entry = entries.FirstOrDefault();

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.NotNull(entry);
            var orderDateProperty = entry.Properties.Single(p => p.Name == "OrderDate") as ODataProperty;
            Assert.NotNull(orderDateProperty);
            Assert.Equal(new DateTimeOffset(2011, 3, 4, 16, 3, 57, TimeSpan.FromHours(-8)), orderDateProperty.Value);
        }
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task QueryDerivedPropertyWithoutTypeCastShouldFail(string mimeType)
    {
        // Arrange & Act & Assert
        await this.BadRequestOrNotFoundAsync("Boss/City", mimeType, /* Not Found */ 404);
    }

    #endregion

    #region Private methods

    private async Task BadRequestOrNotFoundAsync(string requestUri, string mimeType, int errorCode)
    {
        ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };
        var requestUrl = new Uri(_baseUri.AbsoluteUri + requestUri, UriKind.Absolute);

        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);
        if (mimeType == MimeTypes.ApplicationAtomXml)
        {
            requestMessage.SetHeader("Accept", "text/html, application/xhtml+xml, */*");
        }

        var responseMessage = await requestMessage.GetResponseAsync();

        Assert.Equal(errorCode, responseMessage.StatusCode);
    }

    private ODataMessageReaderTestsHelper TestsHelper
    {
        get
        {
            return new ODataMessageReaderTestsHelper(_baseUri, _model, Client);
        }
    }

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "singletontests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
