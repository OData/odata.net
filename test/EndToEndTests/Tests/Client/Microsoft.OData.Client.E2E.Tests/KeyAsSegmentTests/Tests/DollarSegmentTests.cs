//---------------------------------------------------------------------
// <copyright file="DollarSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Globalization;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Server;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;
using Microsoft.OData.Edm;
using Xunit;
using DiscontinuedProduct = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.DiscontinuedProduct;
using Employee = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Employee;
using Login = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Login;

namespace Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Tests;

public class DollarSegmentTests : EndToEndTestBase<DollarSegmentTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(DollarSegmentTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel()));
        }
    }

    public DollarSegmentTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
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
    public void InsertEntityWithKeyValueSameAsNavigationPropertyName()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

        var newLogin = new Login { Username = "LastLogin", CustomerId = -289 };

        // Act
        _context.AddToLogin(newLogin);
        _context.SaveChanges();

        var loginQuery = _context.CreateQuery<Login>("Login").Where(l => l.Username == "LastLogin").ToArray();

        // Assert
        var queried = Assert.Single(loginQuery);
        Assert.True(newLogin == queried, "Query result does not equal newly added login");
        Assert.Equal(-289, queried.CustomerId);
    }

    [Fact]
    public void ClientExecuteEntitySetDerivedType()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        var requestUri = new Uri(_baseUri + "Products/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.DiscontinuedProduct");

        // Act
        var discontinuedProductQuery = _context.Execute<DiscontinuedProduct>(requestUri).ToArray();

        // Assert
        Assert.Equal(5, discontinuedProductQuery.Length);
        var discontinuedProduct = discontinuedProductQuery.Where(d => d.ProductId == -3).Single();
        Assert.Equal("ißuhmxavnmlsssssjssagmqjpchjußtkcoaldeyyduarovnxspzsskufxxfltußtxfhgjlksrn", discontinuedProduct.Description);
        Assert.Equal(-1002345821, discontinuedProduct.ReplacementProductId);
        Assert.Equal("そ歹ソボボをグ裹ぴポｦチ", discontinuedProduct.ChildConcurrencyToken);
    }

    [Fact]
    public void ClientExecuteEntitySetDerivedTypeDollarSegmentAtEnd()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        var requestUri = new Uri(_baseUri + "Products/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.DiscontinuedProduct/$");

        // Act
        var discontinuedProductQuery = _context.Execute<DiscontinuedProduct>(requestUri).ToArray();

        // Assert
        Assert.Equal(5, discontinuedProductQuery.Length);
        var discontinuedProduct = discontinuedProductQuery.Where(d => d.ProductId == -3).Single();
        Assert.Equal("ißuhmxavnmlsssssjssagmqjpchjußtkcoaldeyyduarovnxspzsskufxxfltußtxfhgjlksrn", discontinuedProduct.Description);
        Assert.Equal(-1002345821, discontinuedProduct.ReplacementProductId);
        Assert.Equal("そ歹ソボボをグ裹ぴポｦチ", discontinuedProduct.ChildConcurrencyToken);
    }

    [Fact]
    public void ClientExecuteProjectPropertyDefinedOnDerivedType()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        var requestUri = new Uri(_baseUri + "Products/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.DiscontinuedProduct?$select=Discontinued");

        // Act
        var discontinuedProductDatesQuery = _context.Execute<DiscontinuedProduct>(requestUri).ToArray();

        // Assert
        Assert.Equal(5, discontinuedProductDatesQuery.Length);
        Assert.Single(
            discontinuedProductDatesQuery,
            d => d.Discontinued.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture) == "2005-07-28T13:09:56Z");
    }

    [Fact]
    public void ClientExecuteProjectPropertyDefinedOnDerivedTypeMultipleDollarSegments()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        var requestUri = new Uri(_baseUri + "Products/$/$/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.DiscontinuedProduct?$select=Discontinued");

        // Act
        var discontinuedProductDatesQuery = _context.Execute<DiscontinuedProduct>(requestUri).ToArray();

        // Assert
        Assert.Equal(5, discontinuedProductDatesQuery.Length);
        Assert.Single(
            discontinuedProductDatesQuery,
            d => d.Discontinued.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture) == "2005-07-28T13:09:56Z");
    }

    [Fact]
    public void InvokeFeedBoundActionDefinedOnDerivedType()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        long salaryBeforeIncrementForEmployee_6 = 2147483647;

        var requestUri = new Uri(_baseUri + "People/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee/$/Default.IncreaseSalaries");

        // Act
        var response = _context.Execute(
            requestUri,
            "POST",
            new OperationParameter[]
            {
                    new BodyOperationParameter("n", 200),
            });

        // Assert
        Assert.Equal(200, response.StatusCode);
        var salaryAfterIncrement = _context.Execute<int>(new Uri(_baseUri + "People/-6/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee/$/Salary"));
        Assert.Equal(salaryBeforeIncrementForEmployee_6, salaryAfterIncrement.Single() - 200);

        ResetDefaultDataSource();
    }

    [Fact]
    public void InvokeEntryBoundActionDefinedOnDerivedType()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        var totalEmployeesBeforeSack = 7;
        var requestUri = new Uri(_baseUri + "People/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee/-10/$/Default.Sack");

        // Act
        var response = _context.Execute(
            requestUri,
            "POST",
            new OperationParameter[] { });

        // Assert
        Assert.Equal(204, response.StatusCode);

        var totalEmployeesAfterSack = _context.Execute<Employee>(new Uri(_baseUri + "People/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee")).Count();
        Assert.Equal(totalEmployeesBeforeSack, totalEmployeesAfterSack + 1);

        ResetDefaultDataSource();
    }

    [Fact]
    public void InvokeActionDefinedOnDerivedTypeMultipleDollarSegments()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        long salaryBeforeIncrementForEmployee_6 = 2147483647;
        var requestUri = new Uri(_baseUri + "People/$/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee/$/$/Default.IncreaseSalaries");

        // Act
        var response = _context.Execute(
            requestUri,
            "POST",
            new OperationParameter[]
                {
                        new BodyOperationParameter("n", -200),
                });

        // Assert
        Assert.Equal(200, response.StatusCode);
        var salaryAfterIncrement = _context.Execute<int>(new Uri(_baseUri + "People/-6/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee/$/Salary"));
        Assert.Equal(salaryBeforeIncrementForEmployee_6, salaryAfterIncrement.Single() + 200);

        ResetDefaultDataSource();
    }

    [Fact]
    public void InvokeActionDefinedOnDerivedTypeDollarSegmentAtUriEnd()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        long salaryBeforeIncrementForEmployee_6 = 2147483647;
        var requestUri = new Uri(_baseUri + "People/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee/$/Default.IncreaseSalaries/$");

        // Act
        // Actions must be leaf segments, we do not allow anything to follow them
        var response = _context.Execute(
            requestUri,
            "POST",
            new OperationParameter[]
            {
                    new BodyOperationParameter("n", -200),
            });

        // Assert
        Assert.Equal(200, response.StatusCode);
        var salaryAfterIncrement = _context.Execute<int>(new Uri(_baseUri + "People/-6/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee/$/Salary"));
        Assert.Equal(salaryBeforeIncrementForEmployee_6, salaryAfterIncrement.Single() + 200);

        ResetDefaultDataSource();
    }

    #region Private methods

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "dollarsegmenttests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
