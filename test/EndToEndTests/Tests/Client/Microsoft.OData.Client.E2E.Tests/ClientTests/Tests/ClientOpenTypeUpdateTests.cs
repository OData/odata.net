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
using Microsoft.OData.Client.E2E.Tests.ClientTests.Default;
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
                services.ConfigureControllers(typeof(OpenTypesServiceClientTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", OpenTypesServiceClientTestsEdmModel.GetEdmModel()));
            }
        }
        public ClientOpenTypeUpdateTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _context = new Container(_baseUri);
            _context.HttpClientFactory = HttpClientFactory;
        }

        [Fact]
        public void UpdateOpenTypeWithUndeclaredProperties()
        {
            _context.MergeOption = MergeOption.PreserveChanges;
            _context.Configurations.RequestPipeline.OnEntryStarting(ea => EntryStarting(ea));
            var row = _context.Row.Where(r => r.Id == Guid.Parse("814d505b-6b6a-45a0-9de0-153b16149d56")).First();

            // In practice, transient property data would be mutated here in the partial companion to the client proxy.

            _context.UpdateObject(row);
            _context.SaveChanges();
            // No more check, this case is to make sure that client doesn't throw exception.
        }

        [Fact]
        public void AddOpenTypeWithUndeclaredProperties()
        {
            var row = Row.CreateRow(Guid.NewGuid());

            _context.Configurations.RequestPipeline.OnEntryStarting(ea => EntryStarting(ea));

            _context.AddObject("Row", row);
            _context.SaveChanges();
            // All clear if no exception is thrown
        }

        private void EntryStarting(WritingEntryArgs ea)
        {
            var odataProps = ea.Entry.Properties as List<ODataProperty>;

            var entityState = _context.Entities.First(e => e.Entity == ea.Entity).State;

            // Send up an undeclared property on an Open Type.
            if (entityState == EntityStates.Modified || entityState == EntityStates.Added)
            {
                if (ea.Entity.GetType() == typeof(Row))
                {
                    // In practice, the data from this undeclared property would probably be stored in a transient property of the partial companion class to the client proxy.
                    var undeclaredOdataProperty = new ODataProperty() { Name = "dynamicPropertyKey", Value = "dynamicPropertyValue" };
                    odataProps.Add(undeclaredOdataProperty);
                }
            }
        }
    }
}
