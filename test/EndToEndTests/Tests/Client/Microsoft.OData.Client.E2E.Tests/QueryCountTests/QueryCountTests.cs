//-----------------------------------------------------------------------------
// <copyright file="QueryCountTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;
using Microsoft.OData.E2E.TestCommon.Common.Server.QueryCount;
using Microsoft.OData.Edm;
using Xunit;
using Computer = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Computer;
using Customer = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Customer;
using Order = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Order;

namespace Microsoft.OData.Client.E2E.Tests.QueryCountTests;

public class QueryCountTests : EndToEndTestBase<QueryCountTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(QueryCountTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel()));
        }
    }

    public QueryCountTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
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

    #region Positive tests

    /// <summary>
    /// IncludeCount Test
    /// </summary>
    [Fact]
    public void CountLinqTest()
    {
        // Arrange & Act
        var query = _context.Customers.IncludeCount();

        // Assert
        Assert.Contains("$count=true", query.ToString());

        var response = query.Execute() as QueryOperationResponse<Customer>;
        Assert.NotNull(response);
        Assert.Equal(10, response.Count);
    }

    /// <summary>
    /// IncludeCount(true) Test
    /// </summary>
    [Fact]
    public void CountWithBoolTrueParamLinqTest()
    {
        // Arrange & Act
        var query = _context.Customers.IncludeCount(true);

        // Assert
        Assert.Contains("$count=true", query.ToString());

        var response = query.Execute() as QueryOperationResponse<Customer>;
        Assert.NotNull(response);
        Assert.Equal(10, response.Count);
    }

    /// <summary>
    /// Query Entity Set  With Server Driven Paging
    /// </summary>
    [Fact]
    public void CountLinqTestWithServerDrivenPaging()
    {
        // Arrange & Act
        var query = _context.Computers.IncludeCount();

        // Assert
        Assert.Contains("$count=true", query.ToString());

        var response = query.Execute() as QueryOperationResponse<Computer>;
        Assert.NotNull(response);
        Assert.Equal(10, response.Count);
    }

    /// <summary>
    /// Normal $count request
    /// </summary>
    [Theory]
    [InlineData("Computers?$count=true")]
    [InlineData("Computers?$expand=ComputerDetail&$count=true")]
    [InlineData("Computers?$top=5&$count=true")]
    [InlineData("Computers?$count=true&$skip=5")]
    [InlineData("Computers?$skip=5&$count=true&$top=10")]
    public void CountUriTest(string queryText)
    {
        // Arrange & Act
        var response = _context.Execute<Computer>(new Uri(_baseUri.AbsoluteUri + queryText)) as QueryOperationResponse<Computer>;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(10, response.Count);
    }

    /// <summary>
    /// Normal $count request With Server Driven Paging
    /// </summary>
    [Theory]
    [InlineData("Customers?$count=true")]
    [InlineData("Customers?$Count=true")]
    [InlineData("Customers?$COUNT=true")]
    [InlineData("Customers?$expand=Orders&$count=true")]
    [InlineData("Customers?$top=5&$count=true")]
    [InlineData("Customers?$count=true&$skip=5")]
    [InlineData("Customers?$skip=5&$count=true&$top=10")]
    public void CountUriTestWithServerDrivenPaging(string queryText)
    {
        // Arrange & Act
        var response = _context.Execute<Customer>(new Uri(_baseUri.AbsoluteUri + queryText)) as QueryOperationResponse<Customer>;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(10, response.Count);

        var orders = _context.Execute<Order>(new Uri(_baseUri.AbsoluteUri + "Customers(-10)/Orders?$count=true")) as QueryOperationResponse<Order>;
        Assert.NotNull(orders);
        Assert.Equal(3, orders.Count);
    }

    #endregion

    #region Negative tests

    [Theory]
    [InlineData("Computers?$count", "'$count' cannot be empty.")]
    [InlineData("Computers/$count=true", "NotFound")]
    [InlineData("Computers/$count?$count=true", "The query specified in the URI is not valid. The requested resource is not a collection.")]
    [InlineData("Computers?$count=True", "'True' is not a valid count option.")]
    [InlineData("Computers?$count=true&$count=true", "'true,true' is not a valid count option.")]
    [InlineData("Computers?$count=invalidValue", "'invalidValue' is not a valid count option.")]
    [InlineData("Computers?$count='true'", "''true'' is not a valid count option.")]
    [InlineData("Computers?$count=true/$count", "'true/$count' is not a valid count option.")]
    [InlineData("Computers(-10)?$count=true", "The query specified in the URI is not valid. The requested resource is not a collection.")]
    [InlineData("Computers(-10)/ComputerDetail/Manufacturer?$count=true", "The query specified in the URI is not valid. The requested resource is not a collection.")]
    public async Task CountUriInvalidTest(string queryText, string errorString)
    {
        // Arrange & Act
        var task = _context.ExecuteAsync<Computer>(new Uri(_baseUri.AbsoluteUri + queryText));

        var exception = await Assert.ThrowsAsync<DataServiceQueryException>(async () => await task);

        // Assert
        Assert.NotNull(exception.InnerException);
        Assert.IsType<DataServiceClientException>(exception.InnerException);
        Assert.Contains(errorString, exception.InnerException.Message);
    }

    /// <summary>
    /// Invalid getting count when $count=false
    /// </summary>
    [Theory]
    [InlineData("Customers")]
    [InlineData("Customers?$count=false")]
    public void GetTotalCountInvalidTest(string queryText)
    {
        // Arrange & Act
        var response = _context.Execute<Customer>(new Uri(_baseUri.AbsoluteUri + queryText)) as QueryOperationResponse<Customer>;
        Assert.NotNull(response);

        var exception = Assert.Throws<InvalidOperationException>(() => response.Count);

        // Assert
        Assert.NotNull(exception);
        Assert.Contains("Count value is not part of the response stream", exception.Message);
    }

    /// <summary>
    /// Invalid getting count when IncludeCount(false)
    /// </summary>
    [Fact]
    public void CountWithBoolFalseParamLinqTest()
    {
        // Arrange & Act
        var query = _context.Customers.IncludeCount(false);

        var response = query.Execute() as QueryOperationResponse<Customer>;
        Assert.NotNull(response);

        var exception = Assert.Throws<InvalidOperationException>(() => response.Count);

        // Assert
        Assert.Contains("Count value is not part of the response stream", exception.Message);
    }

    #endregion

    #region Private

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "querycounttests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
