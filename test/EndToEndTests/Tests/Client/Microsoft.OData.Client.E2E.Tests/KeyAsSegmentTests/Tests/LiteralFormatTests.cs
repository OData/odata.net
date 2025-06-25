//---------------------------------------------------------------------
// <copyright file="LiteralFormatTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Server;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;
using Microsoft.OData.Edm;
using Xunit;
using Customer = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Customer;
using Login = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Login;

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
            services.ConfigureControllers(typeof(LiteralFormatTestsController), typeof(MetadataController));

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
    [InlineData("$var1", "Login?$filter=Username eq '%24var1'")]
    [InlineData("$$", "Login?$filter=Username eq '%24%24'")]
    [InlineData("$$$", "Login?$filter=Username eq '%24%24%24'")]
    [InlineData("$$$$", "Login?$filter=Username eq '%24%24%24%24'")]
    [InlineData("$", "Login?$filter=Username eq '%24'")]
    [InlineData("$orderby", "Login?$filter=Username eq '%24orderby'")]
    [InlineData("$filter", "Login?$filter=Username eq '%24filter'")]
    [InlineData("$format", "Login?$filter=Username eq '%24format'")]
    [InlineData("$top", "Login?$filter=Username eq '%24top'")]
    [InlineData("$count", "Login?$filter=Username eq '%24count'")]
    [InlineData("$expand", "Login?$filter=Username eq '%24expand'")]
    [InlineData("$select", "Login?$filter=Username eq '%24select'")]
    public void PrimaryKeyValueBeginsWithDollarSign(string dollarSignKeyValue, string expectedQuery)
    {
        ResetDefaultDataSource();

        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        _context.MergeOption = MergeOption.PreserveChanges;

        var customer = _context.Customers.Take(1).Single();

        var newLogin = new Login { Username = dollarSignKeyValue };

        _context.AddToLogin(newLogin);
        _context.SetLink(newLogin, "Customer", customer);
        _context.AddLink(customer, "Logins", newLogin);
        _context.SaveChanges();

        // Act & Assert
        var loginQuery = _context.CreateQuery<Login>("Login").Where(l => l.Username == dollarSignKeyValue);
        var logins = loginQuery.ToArray();

        Assert.EndsWith(expectedQuery, loginQuery.ToString());
        Assert.True(newLogin == loginQuery.Single(), "Query result does not equal newly added login with key " + dollarSignKeyValue);
        Assert.Equal(dollarSignKeyValue, logins[0].Username);

        var customerQuery = _context.Execute<Customer>(new Uri(_baseUri + "Login/" + dollarSignKeyValue + "/Customer")).ToArray();
        Assert.True(customer == customerQuery.Single(), "Execute query result does not equal associated customer");
    }

    [Theory]
    [InlineData(" /", "Login?$filter=Username eq ' %2F'")]
    [InlineData("/ ", "Login?$filter=Username eq '%2F '")]
    [InlineData("var1/baz", "Login?$filter=Username eq 'var1%2Fbaz'")]
    [InlineData("//var1", "Login?$filter=Username eq '%2F%2Fvar1'")]
    [InlineData("var1//", "Login?$filter=Username eq 'var1%2F%2F'")]
    public void PrimaryKeyValueContainsForwardSlash(string keyValue, string expectedQuery)
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        _context.MergeOption = MergeOption.PreserveChanges;

        var customer = _context.Customers.Take(1).Single();

        var newLogin = new Login { Username = keyValue };

        // Act
        _context.AddToLogin(newLogin);
        _context.SetLink(newLogin, "Customer", customer);
        _context.AddLink(customer, "Logins", newLogin);
        _context.SaveChanges();

        var loginQuery = _context.CreateQuery<Login>("Login").Where(l => l.Username == keyValue);
        var logins = loginQuery.ToArray();

        // Assert
        Assert.EndsWith(expectedQuery, loginQuery.ToString());
        Assert.Single(logins);
        Assert.Equal(keyValue, logins[0].Username);

        ResetDefaultDataSource();
    }

    [Theory]
    [InlineData("var1 baz", "Login?$filter=Username eq 'var1 baz'")]
    [InlineData("  var1", "Login?$filter=Username eq '  var1'")]
    public void PrimaryKeyValueContainsWhitespace(string keyValue, string expectedQuery)
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        _context.MergeOption = MergeOption.PreserveChanges;

        var customer = _context.Customers.Take(1).Single();

        var newLogin = new Login { Username = keyValue };

        // Act
        _context.AddToLogin(newLogin);
        _context.SetLink(newLogin, "Customer", customer);
        _context.AddLink(customer, "Logins", newLogin);
        _context.SaveChanges();

        var loginQuery = _context.CreateQuery<Login>("Login").Where(l => l.Username == keyValue);
        var logins = loginQuery.ToArray();

        // Assert
        Assert.EndsWith(expectedQuery, loginQuery.ToString());
        Assert.True(newLogin == loginQuery.Single(), "Query result does not equal newly added login with key " + keyValue);

        var customerQuery = _context.Execute<Customer>(new Uri(_baseUri + "Login/" + keyValue + "/Customer")).ToArray();
        Assert.True(customer == customerQuery.Single(), "Execute query result does not equal associated customer");

        ResetDefaultDataSource();
    }

    #region Private methods

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "keyasssegmentliteralformattests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
