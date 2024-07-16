//-----------------------------------------------------------------------------
// <copyright file="ClientOpenTypeUpdateTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.ClientTests.Server;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.OpenTypes.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.OpenTypes;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.ClientTests.Tests
{
    public class ClientOpenTypeUpdateTests : EndToEndTestBase<ClientOpenTypeUpdateTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(OpenTypesTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", OpenTypesEdmModel.GetEdmModel()));
            }
        }
        public ClientOpenTypeUpdateTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };

            ResetOpenTypesDataSource();
        }

        [Fact]
        public async Task UpdatingOpenTypeWithUndeclaredProperties_AddsDynamicProperty_ToDynamicPropertiesDict()
        {
            // Arrange
            _context.MergeOption = MergeOption.PreserveChanges;
            _context.Configurations.RequestPipeline.OnEntryStarting(ea => EntryStarting(ea));

            // Retrieve the row by its Id
            var rowId = Guid.Parse("814d505b-6b6a-45a0-9de0-153b16149d56");
            var row = (await ((DataServiceQuery<Common.Clients.OpenTypes.Row>)_context.Rows.Where(r => r.Id == rowId)).ExecuteAsync()).First();

            // Verify that initially there is only one dynamic property
            Assert.Single(row.DynamicProperties);

            // Act
            _context.UpdateObject(row);
            await _context.SaveChangesAsync();

            // Retrieve the updated row
            var updatedRow = (await ((DataServiceQuery<Common.Clients.OpenTypes.Row>)_context.Rows.Where(r => r.Id == rowId)).ExecuteAsync()).First();

            // Assert
            // Verify that the dynamic properties now contain two items
            Assert.Equal(2, updatedRow.DynamicProperties.Count);

            // Verify the added dynamic property is present and has the correct value
            Assert.True(updatedRow.DynamicProperties.TryGetValue("dynamicPropertyKey", out object keyValue));
            Assert.Equal("dynamicPropertyValue", keyValue);
        }

        [Fact]
        public async Task AddOpenTypeWithUndeclaredProperties_DoesNotThrowException()
        {
            var rowId = Guid.NewGuid();
            var row = Common.Clients.OpenTypes.Row.CreateRow(rowId);

            _context.Configurations.RequestPipeline.OnEntryStarting(ea => EntryStarting(ea));

            _context.AddObject("Rows", row);
            await _context.SaveChangesAsync();

            // Get the created row;
            var createdRow = (await ((DataServiceQuery<Common.Clients.OpenTypes.Row>)_context.Rows.Where(r => r.Id == rowId)).ExecuteAsync()).First(); ;

            Assert.Equal(rowId, createdRow.Id);
            Assert.Single(createdRow.DynamicProperties);

            // Verify the added dynamic property is present and has the correct value
            Assert.True(createdRow.DynamicProperties.TryGetValue("dynamicPropertyKey", out object keyValue));
            Assert.Equal("dynamicPropertyValue", keyValue);
        }

        private void EntryStarting(WritingEntryArgs ea)
        {
            var odataProps = ea.Entry.Properties as List<ODataProperty>;

            var entityState = _context.Entities.First(e => e.Entity == ea.Entity).State;

            // Send up an undeclared property on an Open Type.
            if (entityState == EntityStates.Modified || entityState == EntityStates.Added)
            {
                if (ea.Entity.GetType() == typeof(Common.Clients.OpenTypes.Row))
                {
                    // In practice, the data from this undeclared property would probably be stored in a transient property of the partial companion class to the client proxy.
                    var undeclaredOdataProperty = new ODataProperty() { Name = "dynamicPropertyKey", Value = "dynamicPropertyValue" };
                    odataProps.Add(undeclaredOdataProperty);
                }
            }
        }

        private void ResetOpenTypesDataSource()
        {
            var actionUri = new Uri(_baseUri + "clientopentypeupdate/Default.ResetOpenTypesDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}
