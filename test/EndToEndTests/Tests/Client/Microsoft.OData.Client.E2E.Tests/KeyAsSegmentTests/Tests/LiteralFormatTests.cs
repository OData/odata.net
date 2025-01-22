//---------------------------------------------------------------------
// <copyright file="LiteralFormatTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;
using Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Server;
using Microsoft.OData.Edm;
using Xunit;
using Login = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Login;
using Customer = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Customer;

namespace Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Tests;

public class LiteralFormatTests : EndToEndTestBase<LiteralFormatTests.TestsStartup>
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

    public LiteralFormatTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
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

    [Theory]
    [InlineData("$var1")]
    [InlineData("$$")]
    [InlineData("$$$")]
    [InlineData("$$$$")]
    [InlineData("$")]
    [InlineData("$orderby")]
    [InlineData("$filter")]
    [InlineData("$format")]
    [InlineData("$top")]
    [InlineData("$count")]
    [InlineData("$expand")]
    [InlineData("$select")]
    public void PrimaryKeyValueBeginsWithDollarSign(string dollarSignKeyValue)
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        var customer = _context.Customers.Take(1).Single();

        var newLogin = new Login { Username = dollarSignKeyValue };

        _context.AddToLogin(newLogin);
        _context.SetLink(newLogin, "Customer", customer);
        _context.AddLink(customer, "Logins", newLogin);
        _context.SaveChanges();

        // Act & Assert
        var loginQuery = _context.CreateQuery<Login>("Login").Where(l => l.Username == dollarSignKeyValue).ToArray();
        Assert.True(newLogin == loginQuery.Single(), "Query result does not equal newly added login with key " + dollarSignKeyValue);
        Assert.Equal(dollarSignKeyValue, loginQuery[0].Username);

        var customerQuery = _context.Execute<Customer>(new Uri(_baseUri + "Login/" + dollarSignKeyValue + "/Customer")).ToArray();
        Assert.True(customer == customerQuery.Single(), "Execute query result does not equal associated customer");
    }

    [Theory]
    [InlineData(" /")]
    [InlineData("/ ")]
    [InlineData("var1/baz")]
    [InlineData("//var1")]
    [InlineData("var1//")]
    public void PrimaryKeyValueContainsForwardSlash(string keyValue)
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        var customer = _context.Customers.Take(1).Single();

        var newLogin = new Login { Username = keyValue };

        // Act
        _context.AddToLogin(newLogin);
        _context.SetLink(newLogin, "Customer", customer);
        _context.AddLink(customer, "Logins", newLogin);
        _context.SaveChanges();

        var loginQuery = _context.CreateQuery<Login>("Login").Where(l => l.Username == keyValue).ToArray();

        // Assert
        Assert.Single(loginQuery);
        Assert.Equal(keyValue, loginQuery[0].Username);
    }

    [Theory]
    [InlineData("var1 baz")]
    [InlineData("  var1")]
    public void PrimaryKeyValueContainsWhitespace(string keyValue)
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        var customer = _context.Customers.Take(1).Single();

        var newLogin = new Login { Username = keyValue };

        // Act
        _context.AddToLogin(newLogin);
        _context.SetLink(newLogin, "Customer", customer);
        _context.AddLink(customer, "Logins", newLogin);
        _context.SaveChanges();

        var loginQuery = _context.CreateQuery<Login>("Login").Where(l => l.Username == keyValue).ToArray();
        Assert.True(newLogin == loginQuery.Single(), "Query result does not equal newly added login with key " + keyValue);

        var customerQuery = _context.Execute<Customer>(new Uri(_baseUri + "Login/" + keyValue + "/Customer")).ToArray();
        Assert.True(customer == customerQuery.Single(), "Execute query result does not equal associated customer");
    }

    #region Private methods

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "keyassegmenttests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
