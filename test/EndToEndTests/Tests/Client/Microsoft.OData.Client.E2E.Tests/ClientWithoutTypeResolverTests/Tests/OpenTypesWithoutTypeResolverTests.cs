﻿//-----------------------------------------------------------------------------
// <copyright file="OpenTypesWithoutTypeResolverTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.Tests.ClientWithoutTypeResolverTests.Server;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.OpenTypes.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.OpenTypes;
using Xunit;
using ClientOpenTypesModel = Microsoft.OData.E2E.TestCommon.Common.Client.OpenTypes;

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
                    .AddRouteComponents("odata", OpenTypesEdmModel.GetEdmModel(), new DefaultODataBatchHandler()));
            }
        }

        public OpenTypesWithoutTypeResolverTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _context = new Container(_baseUri);
            _context.HttpClientFactory = HttpClientFactory;
            ResetOpenTypesDataSource();

            _context.AddObject("Rows", row1);
            _context.AddObject("Rows", row2);

            typeNameResolver = _context.ResolveName;

            _context.ResolveName = typeNameResolver;

            var rowIndex = new ClientOpenTypesModel.RowIndex
            {
                Id = TestRowIndexId,
                Rows = new DataServiceCollection<ClientOpenTypesModel.IndexedRow>(_context),
                DynamicProperties = new Dictionary<string, object>()
                {
                    { "IndexComments", "This is a test"}
                }
            };

            _context.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
        }

        #region Test Data
        private ClientOpenTypesModel.Row row1 = new ClientOpenTypesModel.Row
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
                { "OpenInt16", short.MaxValue },
                { "OpenInt64", long.MaxValue },
                { "OpenString","hello world" },
                { "OpenTime", TimeSpan.MaxValue }
            }
        };

        private ClientOpenTypesModel.Row row2 = new ClientOpenTypesModel.Row
        {
            Id = Guid.NewGuid(),
            DynamicProperties = new Dictionary<string, object>
            {
                { "OpenBoolean", null },
                { "OpenDateTimeOffset", null },
                { "OpenDecimal", null },
                { "OpenDouble", null },
                { "OpenFloat", null },
                { "OpenGuid", null },
                { "OpenInt16", null },
                { "OpenInt64", null },
                { "OpenString",null },
                { "OpenTime", null }
            }
        };

        private ClientOpenTypesModel.IndexedRow row3 = new ClientOpenTypesModel.IndexedRow
        {
            Id = Guid.NewGuid(),
            DynamicProperties = new Dictionary<string, object>()
            {
                { "OpenDouble", double.NaN }
            }
        };
        #endregion

        private const int TestRowIndexId = 999;
        private Func<Type, string> typeNameResolver;

        [Fact]
        public async Task ExpandQuery_OnOpenType_WorksCorrectly()
        {
            var query = _context.CreateQuery<ClientOpenTypesModel.RowIndex>("RowIndices").Expand(i => i.Rows);
            Assert.Equal("http://localhost/odata/RowIndices?$expand=Rows", query.ToString());

            var results = await query.ExecuteAsync();

            //Get a result with expanded row values;
            var res = results.Single(a => a.Id == -9);
            Assert.Single(res.Rows);
        }

        [Fact]
        public void SelectQuery_OnOpenTypes_WorksCorrectly()
        {
            var query = _context.CreateQuery<ClientOpenTypesModel.Row>("Rows").Select(r => new { r.Id });
            var results = query.ToList();

            Assert.Equal(12, results.Count);
            Assert.Equal("432f0da9-806e-4a2f-b708-dbd1c57a1c21", results.First().Id.ToString());
        }

        [Fact]
        public async Task DerivedTypesQuery_OnOpenTypes_WorksCorrectly()
        {
            var query = _context.Rows.OfType<ClientOpenTypesModel.IndexedRow>();
            var results = (await ((DataServiceQuery<ClientOpenTypesModel.IndexedRow>)query).ExecuteAsync()).ToArray();

            // Ensure all results are of the correct type
            foreach (var result in results)
            {
                Assert.IsType<ClientOpenTypesModel.IndexedRow>(result);
            }
        }

        [Fact]
        public async Task BaseTypeQuery_Returns_ADerivedTypeObject()
        {
            var derivedQuery = _context.Rows.OfType<ClientOpenTypesModel.IndexedRow>().Take(1);
            var indexedRow = (await ((DataServiceQuery<ClientOpenTypesModel.IndexedRow>)derivedQuery).ExecuteAsync()).Single();

            var baseQuery = (await _context.ExecuteAsync<ClientOpenTypesModel.Row>(new Uri(_baseUri.OriginalString + "Rows(" + indexedRow.Id.ToString() + ")"))).ToList();
            var row = baseQuery.Single();

            Assert.True(row is ClientOpenTypesModel.IndexedRow);
        }

        [Fact]
        public async Task Updating_OpenProperties_UpdatesSuccessfully()
        {
            // Restore the type name resolver since we will be writing open complex properties.
            _context.ResolveName = typeNameResolver;

            // Act - Fetch the row and update its dynamic properties
            var updateRow = (await ((DataServiceQuery<ClientOpenTypesModel.Row>)_context.Rows.Where(r => r.Id == row2.Id)).ExecuteAsync()).Single();
            var openBooleanBeforeUpdate = updateRow.DynamicProperties.SingleOrDefault(a => a.Key.Equals("OpenBoolean"));
            Assert.Null(openBooleanBeforeUpdate.Value);

            // Update dynamic properties
            updateRow.DynamicProperties = new Dictionary<string, object>()
            {
                {
                    "OpenComplex", new ContactDetails
                    {
                        Byte = byte.MinValue,
                        Contacted = DateTimeOffset.Now,
                        Double = double.MaxValue,
                        FirstContacted = new byte[] { byte.MaxValue, byte.MinValue, 0 },
                        GUID = Guid.NewGuid(),
                        LastContacted = DateTimeOffset.Now,
                        PreferedContactTime = TimeSpan.FromMilliseconds(1234D),
                        Short = short.MinValue
                    }
                },
                { "OpenBoolean", true },
                { "OpenInt64", long.MaxValue },
                { "OpenDecimal", decimal.Zero },
                { "OpenString", string.Empty }
            };

            _context.UpdateObject(updateRow);
            _context.SaveChanges();

            // Act - Fetch the row again to verify updates
            var updatedRow = (await ((DataServiceQuery<ClientOpenTypesModel.Row>)_context.Rows.Where(r => r.Id == row2.Id)).ExecuteAsync()).Single();

            // Assert - Verify each dynamic property was updated correctly
            var openComplexAfterUpdate = updatedRow.DynamicProperties.SingleOrDefault(a => a.Key.Equals("OpenComplex"));
            var complexValue = Assert.IsType<ContactDetails>(openComplexAfterUpdate.Value);
            Assert.Equal(byte.MinValue, complexValue.Byte);
            Assert.Equal(double.MaxValue, complexValue.Double);
            Assert.Equal(new byte[] { byte.MaxValue, byte.MinValue, 0 }, complexValue.FirstContacted);
            Assert.NotEqual(Guid.Empty, complexValue.GUID);
            Assert.Equal(TimeSpan.FromMilliseconds(1234D), complexValue.PreferedContactTime);
            Assert.Equal(short.MinValue, complexValue.Short);

            var openBooleanAfterUpdate = updatedRow.DynamicProperties.SingleOrDefault(a => a.Key.Equals("OpenBoolean"));
            Assert.Equal(true, openBooleanAfterUpdate.Value);

            var openInt64AfterUpdate = updatedRow.DynamicProperties.SingleOrDefault(a => a.Key.Equals("OpenInt64"));
            Assert.Equal(long.MaxValue, openInt64AfterUpdate.Value);

            var openDecimalAfterUpdate = updatedRow.DynamicProperties.SingleOrDefault(a => a.Key.Equals("OpenDecimal"));
            Assert.Equal(decimal.Zero, openDecimalAfterUpdate.Value);

            var openStringAfterUpdate = updatedRow.DynamicProperties.SingleOrDefault(a => a.Key.Equals("OpenString"));
            Assert.Equal(string.Empty, openStringAfterUpdate.Value);
        }

        private void ResetOpenTypesDataSource()
        {
            var actionUri = new Uri(_baseUri + "opentypeswithouttyperesolver/Default.ResetOpenTypesDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}
