//-----------------------------------------------------------------------------
// <copyright file="ClientMultipleKeysEnumKeyTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.ClientTests.Server;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.MultipleKeys;
using Microsoft.OData.Client.E2E.Tests.Common.Server.MultipleKeys;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.ClientTests.Tests
{
    public class ClientMultipleKeysEnumKeyTests : EndToEndTestBase<ClientMultipleKeysEnumKeyTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(ClientMultipleKeysEnumKeyTestsController), typeof(MetadataController));

                services.AddControllers()
                    .AddOData(opt => opt.EnableQueryFeatures().AddRouteComponents("odata", MultipleKeysEnumKeyEdmModel.GetEdmModel()));
            }
        }

        public ClientMultipleKeysEnumKeyTests(TestWebApplicationFactory<TestsStartup> fixture) 
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };

            ResetDataSource();
        }

        [Theory]
        [InlineData("Employees", 4)]
        [InlineData("Employees?$filter=EmployeeType eq 0", 2)]
        [InlineData("Employees(EmployeeNumber=1,EmployeeType=Microsoft.OData.Client.E2E.Tests.Common.Server.MultipleKeys.EmployeeType'FullTime')", 1)]
        [InlineData("Employees?$filter=EmployeeType eq Microsoft.OData.Client.E2E.Tests.Common.Server.MultipleKeys.EmployeeType'FullTime'", 2)]
        public async Task CommonQueries_ExecutesSuccessfully(string query, int expectedCount)
        {
            // Arrange
            var uri = new Uri(_baseUri.OriginalString + query);

            // Act
            var result = await _context.ExecuteAsync<Common.Clients.MultipleKeys.EmployeeWithEnumKey>(uri);

            // Assert
            Assert.Equal(expectedCount, result.ToArray().Length);
        }

        [Fact]
        public async Task UseWhereToFilterByEnumKey_WithEnumAsKey_DoNotThrowException()
        {
            // Arrange
            var query = _context.Employees
                .Where(e => e.EmployeeType == Common.Clients.MultipleKeys.EmployeeType.Contractor) as DataServiceQuery<Common.Clients.MultipleKeys.EmployeeWithEnumKey>;

            // Act
            var result = await query.ExecuteAsync();

            // Assert
            Assert.Equal("http://localhost/odata/Employees?$filter=EmployeeType eq Microsoft.OData.Client.E2E.Tests.Common.Server.MultipleKeys.EmployeeType'Contractor'", query.ToString());
            Assert.Single(result);
        }

        [Fact]
        public async Task FilterByCompositeKeys_WithEnumAsKey_DoNotThrowException()
        {
            // Arrange
            var query = _context.Employees.ByKey(
                new Dictionary<string, object>() { { "EmployeeNumber", 1 }, { "EmployeeType", Common.Clients.MultipleKeys.EmployeeType.FullTime } });

            // Act
            var result = await query.GetValueAsync();

            // Assert
            Assert.Equal("http://localhost/odata/Employees(EmployeeNumber=1,EmployeeType=Microsoft.OData.Client.E2E.Tests.Common.Server.MultipleKeys.EmployeeType'FullTime')", query.RequestUri.ToString());
            Assert.Equal(1, result.EmployeeNumber);
            Assert.Equal(Common.Clients.MultipleKeys.EmployeeType.FullTime, result.EmployeeType);
        }

        [Fact]
        public async Task DetachAndAttachEntity_WithEnumAsKey_DoNotThrowException()
        {
            // Arrange & Act
            var employeeCollection = new DataServiceCollection<Common.Clients.MultipleKeys.EmployeeWithEnumKey>(_context.Employees);

            // Get the first entity from the context
            object entity = _context.Entities.First().Entity;

            // Remove the entity from the context
            _context.Detach(entity);

            // Attach the entity back to the context
            var exception = Record.Exception(() => _context.AttachTo("Employees", entity));

            // Assert
            Assert.Null(exception);
            Assert.Equal(4, employeeCollection.Count());

            DataServiceQuery<Common.Clients.MultipleKeys.EmployeeWithEnumKey> query = _context.Employees;
            IEnumerable<Common.Clients.MultipleKeys.EmployeeWithEnumKey> employees = await query.ExecuteAsync();

            Assert.Equal(4, employees.Count());
        }

        private void ResetDataSource()
        {
            var actionUri = new Uri(_baseUri + "clientmultiplekeysenumkey/Default.ResetDefaultDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}
