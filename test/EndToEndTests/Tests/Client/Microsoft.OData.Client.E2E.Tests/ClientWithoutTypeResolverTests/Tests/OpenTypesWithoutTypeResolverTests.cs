//-----------------------------------------------------------------------------
// <copyright file="OpenTypesWithoutTypeResolverTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.ClientWithoutTypeResolverTests.Server;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.OpenTypes.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.OpenTypes;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.ClientWithoutTypeResolverTests.Tests
{
    public class OpenTypesWithoutTypeResolverTests : EndToEndTestBase<OpenTypesWithoutTypeResolverTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(OpenTypesWithoutTypeResolverTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.EnableQueryFeatures()
                    .AddRouteComponents("odata", OpenTypesServiceEdmModel.GetEdmModel(), new DefaultODataBatchHandler()));
            }
        }

        public OpenTypesWithoutTypeResolverTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _context = new Container(_baseUri);
            _context.HttpClientFactory = HttpClientFactory;

            _context.ResolveName = this.typeNameResolver;

            _context.AddObject("Row", row1);
            _context.AddObject("Row", row2);

            var rowIndex = new Common.Clients.OpenTypes.RowIndex
            {
                Id = TestRowIndexId,
                Rows = new DataServiceCollection<Common.Clients.OpenTypes.IndexedRow>(_context),
                DynamicProperties = new Dictionary<string, object>()
                {
                    { "IndexComments", "This is a test"}
                }
            };

           // rowIndex.Rows.Add(row3);
          //  _context.AddObject("RowIndex", rowIndex);

            _context.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
        }

        private Common.Clients.OpenTypes.Row row1 = new Common.Clients.OpenTypes.Row
        {
            Id = Guid.NewGuid(),
            DynamicProperties = new Dictionary<string, object>
            {
                { "OpenBoolean", true },
                { "OpenDateTimeOffset", DateTimeOffset.Now },
                { "OpenDecimal", decimal.MinusOne },
                { "OpenDouble", double.NaN },
                { "OpenFloat", float.PositiveInfinity },
                { "OpenGuid", Guid.NewGuid() },
                { "OpenInt16", Int16.MaxValue },
                { "OpenInt64", Int64.MaxValue },
                { "OpenString", "hello world" },
                { "OpenTime", TimeSpan.MaxValue }
            }
        };

        private Common.Clients.OpenTypes.Row row2 = new Common.Clients.OpenTypes.Row
        {
            Id = Guid.NewGuid(),
            DynamicProperties = new Dictionary<string, object>
            {
                { "OpenBoolean", null },
                { "OpenDateTimeOffset", null },
                { "OpenDecimal", null },
                { "OpenDouble", null},
                { "OpenFloat", null },
                { "OpenGuid", null },
                { "OpenInt16", null },
                { "OpenInt64", null },
                { "OpenString", null },
                { "OpenTime", null }
            }
        };

        private Common.Clients.OpenTypes.IndexedRow row3 = new Common.Clients.OpenTypes.IndexedRow
        {
            Id = Guid.NewGuid(),
            DynamicProperties = new Dictionary<string, object>()
            {
                { "OpenDouble", double.NaN }
            }
        };

        private const int TestRowIndexId = 999;
        private Func<Type, string> typeNameResolver;

        [Fact, TestPriority(3)]
        public void ExpandQuery()
        {
            var query = _context.CreateQuery<Common.Clients.OpenTypes.RowIndex>("RowIndex").Expand(i => i.Rows);
            var results = query.Execute();
        }

        [Fact, TestPriority(4)]
        public void ProjectionQuery()
        {
            var query = _context.CreateQuery<Common.Clients.OpenTypes.Row>("Row").Select(r => new { r.Id, r.DynamicProperties.Where(a=>a.Key.Equals("")) });
            var results = query.ToList();
        }
    }
}
