//---------------------------------------------------------------------
// <copyright file="UrlModifyingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.UrlModifying;
using Xunit;
using Customer = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Customer;
using Person = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Person;

namespace Microsoft.OData.Client.E2E.Tests.UrlModifyingTests;

public class UrlModifyingTests : EndToEndTestBase<UrlModifyingTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(UrlModifyingTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
            {
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel(), new UrlModifyingBatchHandler());
                opt.RouteOptions.EnableNonParenthesisForEmptyParameterFunction = true;
            });
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<UrlModifyingMiddleware>();
            base.Configure(app, env);
        }
    }

    public UrlModifyingTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
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
    public async Task Test_ModifyQueryOptions_AppendTopQueryOption()
    {
        // This test verifies that the query options are modified to include $top=3.

        // Arrange
        var context = this.CreateWrappedContext();
        var personQuery = context.CreateQuery<Person>("People");

        // Request is modified to include $top=3

        // Act
        var top3People = (await personQuery.ExecuteAsync()).ToList();

        // Assert
        Assert.NotNull(top3People);
        Assert.Equal(3, top3People.Count);
        Assert.All(top3People, p => Assert.NotNull(p));
    }

    [Fact]
    public async Task Test_ModifyRequestUri()
    {
        // This test verifies that the request URI is remapped correctly.

        // Arrange
        var context = this.CreateWrappedContext();
        var customQuery = context.CreateQuery<Customer>("RemapPath");
        //Path should remap to the customers set

        // Act
        var retrievedCustomers = (await customQuery.ExecuteAsync()).ToList();

        // Assert
        Assert.NotNull(retrievedCustomers);
        Assert.Equal(2, retrievedCustomers.Count);
        Assert.Equal("Bob Cat", $"{retrievedCustomers[0].FirstName} {retrievedCustomers[0].LastName}");
        Assert.Equal("Jill Jones", $"{retrievedCustomers[1].FirstName} {retrievedCustomers[1].LastName}");
    }

    [Fact]
    public async Task Test_ModifyBaseUri()
    {
        // This test verifies that the base URI is remapped correctly.

        // Arrange
        var context = this.CreateWrappedContext();
        var customQuery = context.CreateQuery<Customer>("RemapBase");
        //Path should remap to the customers set

        // Act
        var retrievedPersons = await customQuery.ExecuteAsync();

        // Assert
        Assert.NotNull(retrievedPersons);
        Assert.All(retrievedPersons, p =>
        {
            var descriptor = context.GetEntityDescriptor(p);
            Assert.IsType<Customer>(p);
            Assert.Equal($"http://potato/odata/Customers({p.PersonID})", descriptor.EditLink.AbsoluteUri);
        });
    }

    [Fact]
    public async Task Test_ModifyBaseAndPathUrlsSeparately()
    {
        // This test verifies that the base and path are remapped separately.

        // Arrange
        var context = this.CreateWrappedContext();
        DataServiceQuery customQuery = context.CreateQuery<Customer>("RemapBaseAndPathSeparately");
        //Path should remap to the customers set

        // Act
        var retrievedPersons = (IEnumerable<Customer>)await customQuery.ExecuteAsync();

        Assert.NotNull(retrievedPersons);
        Assert.All(retrievedPersons, p =>
        {
            var descriptor = context.GetEntityDescriptor(p);
            Assert.IsType<Customer>(p);
            Assert.Equal($"http://potato/odata/Customers({p.PersonID})", descriptor.EditLink.AbsoluteUri);
        });
    }

    [Fact]
    public async Task Test_WhenBasesDoNotMatch_Fails()
    {
        // This test verifies that an exception is thrown when the bases don't match.

        // Arrange
        var context = this.CreateWrappedContext();

        DataServiceQuery customQuery = context.CreateQuery<Customer>("BasesDontMatchFail");
        //Path should remap to the customers set

        // Act
        var exception = await Assert.ThrowsAsync<DataServiceQueryException>(async() => await customQuery.ExecuteAsync());

        // Assert
        Assert.NotNull(exception.InnerException);
        Assert.Equal("NotFound", exception.InnerException.Message);
    }

    [Fact]
    public async Task Test_ModifyBatchRequests()
    {
        // This test verifies that batch requests are handled correctly.

        // Arrange
        var context = this.CreateWrappedContext();
        //Setup queries
        var batchRequest = new DataServiceRequest[] {
            context.CreateQuery<Customer>("BatchRequest1"), // BatchRequest1 query customers
            context.CreateQuery<Person>("BatchRequest2"), // BatchRequest2 query people
        };

        // Act
        var response = await context.ExecuteBatchAsync(SaveChangesOptions.BatchWithSingleChangeset, batchRequest);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsBatchResponse);

        var batchResponse = response.ToList();
        foreach (var item in batchResponse)
        {
            Assert.True(item.StatusCode == 200);
        }
    }

    #region Private methods

    private Container CreateWrappedContext()
    {
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        return _context;
    }

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "urlmodifyingtests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
