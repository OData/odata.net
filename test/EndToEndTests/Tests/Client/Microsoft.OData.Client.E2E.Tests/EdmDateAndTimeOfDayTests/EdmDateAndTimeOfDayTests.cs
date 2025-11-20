//-----------------------------------------------------------------------------
// <copyright file="EdmDateAndTimeOfDayTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.EdmDateAndTimeOfDay;
using Microsoft.OData.Edm;
using Xunit;
using ClientDefaultModel = Microsoft.OData.E2E.TestCommon.Common.Client.Default;

namespace Microsoft.OData.Client.E2E.Tests.EdmDateAndTimeOfDayTests
{
    public class EdmDateAndTimeOfDayTests : EndToEndTestBase<EdmDateAndTimeOfDayTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        private readonly IEdmModel _model;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(EdmDateAndTimeOfDayTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
            }
        }

        public EdmDateAndTimeOfDayTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");

            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };

            _model = DefaultEdmModel.GetEdmModel();
            ResetDefaultDataSource();
        }

        #region Client

        [Fact]
        public async Task SelectingAnEntitysDateOrTimeOfDayPropertiesFromODataClient_WorksCorrectly()
        {
            // Query Property
            var shipDate = await _context.Orders.ByKey(7).Select(o => o.ShipDate).GetValueAsync();
            Assert.Equal(new DateOnly(2014, 8, 31), shipDate);

            var shipTime = await _context.Orders.ByKey(7).Select(o => o.ShipTime).GetValueAsync();
            Assert.Equal(new TimeOnly(12, 40, 05, 50), shipTime);
        }

        [Fact]
        public async Task AProjectionSelectForDateAndTimeOfDayProperties_WorksCorrectly()
        {
            // Projection Select
            var projOrder = await _context.Orders.ByKey(7).Select(o => new ClientDefaultModel.Order() { ShipDate = o.ShipDate, ShipTime = o.ShipTime }).GetValueAsync();
            Assert.True(projOrder != null);
            Assert.Equal(new DateOnly(2014, 8, 31), projOrder.ShipDate);
            Assert.Equal(new TimeOnly(12, 40, 05, 50), projOrder.ShipTime);
        }

        [Fact]
        public async Task UpdatingDateAndTimeOfDayPropertiesFromODataClient_WorksCorrectly()
        {
            _context.MergeOption = MergeOption.OverwriteChanges;

            // Update Properties
            var order = await _context.Orders.ByKey(7).GetValueAsync();
            Assert.True(order != null);
            Assert.Equal(new DateOnly(2014, 8, 31), order.ShipDate);
            Assert.Equal(new TimeOnly(12, 40, 05, 50), order.ShipTime);

            order.ShipDate = new DateOnly(2014, 9, 30);
            _context.UpdateObject(order);
            await _context.SaveChangesAsync();

            var updatedOrder = await _context.Orders.ByKey(7).GetValueAsync();
            Assert.Equal(new DateOnly(2014, 9, 30), updatedOrder.ShipDate);
        }

        [Fact]
        public async Task InvokingAFunctionThatReturnsDateFromODataClient_WorksCorrectly()
        {
            // Function
            var date = await _context.Orders.ByKey(7).GetShipDate().GetValueAsync();
            Assert.Equal(new DateOnly(2014, 8, 31), date);
        }

        [Fact]
        public async Task InvokingAnActionThatTakesDateAndTimeOfDayAsParameters_WorksCorrectly()
        {
            _context.MergeOption = MergeOption.OverwriteChanges;

            var order = await _context.Orders.ByKey(7).GetValueAsync();
            Assert.Equal(new DateOnly(2014, 8, 31), order.ShipDate);

            // Action
            await _context.Orders.ByKey(7).ChangeShipTimeAndDate(DateOnly.MaxValue, TimeOnly.MaxValue).GetValueAsync();
            order = await _context.Orders.ByKey(7).GetValueAsync();
            Assert.Equal(DateOnly.MaxValue, order.ShipDate);

            Assert.Equal(TimeOnly.MaxValue, order.ShipTime);
            Assert.Equal(TimeOnly.MaxValue.ToODataString(), order.ShipTime.ToODataString());
        }

        [Fact]
        public async Task InvokingAFunctionWithDateParameterFromODataClient_WorksCorrectly()
        {
            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Orders(7)/Default.CheckShipDate(date = 2014-08-31)", UriKind.Absolute);

            var result = (await _context.ExecuteAsync<bool>(requestUrl)).Single();

            Assert.True(result);
        }

        [Fact]
        public async Task InvokingAFunctionWithATimeParameterFromODataClient_WorksCorrectly()
        {
            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Orders(7)/Default.CheckShipTime(time = 12:40:5.5)", UriKind.Absolute);

            var result = (await _context.ExecuteAsync<bool>(requestUrl)).Single();

            Assert.False(result);
        }

        [Fact]
        public void QueryingOrdersWithOrderByOnDateOnly_WorksCorrectly()
        {
            var orders = _context.Orders
                .OrderBy(o => o.ShipDate)
                .ToList();

            Assert.NotEmpty(orders);
            for (int i = 1; i < orders.Count; i++)
            {
                Assert.True(orders[i - 1].ShipDate <= orders[i].ShipDate);
            }
        }

        [Fact]
        public void QueryingOrdersWithOrderByOnTimeOnly_WorksCorrectly()
        {
            var orders = _context.Orders
                .OrderBy(o => o.ShipTime)
                .ToList();

            Assert.NotEmpty(orders);
            for (int i = 1; i < orders.Count; i++)
            {
                Assert.True(orders[i - 1].ShipTime <= orders[i].ShipTime);
            }
        }

        [Fact]
        public void QueryingOrdersWithFilterOnDateOnly_WorksCorrectly()
        {
            var targetDate = new DateOnly(2014, 8, 31);
            var orders = _context.Orders
                .Where(o => o.ShipDate == targetDate)
                .ToList();

            Assert.All(orders, o => Assert.Equal(targetDate, o.ShipDate));
        }

        [Fact]
        public void QueryingOrdersWithFilterOnTimeOnly_WorksCorrectly()
        {
            var targetTime = new TimeOnly(12, 40, 05, 50);
            var orders = _context.Orders
                .Where(o => o.ShipTime == targetTime)
                .ToList();

            Assert.All(orders, o => Assert.Equal(targetTime, o.ShipTime));
        }

        [Fact]
        public void QueryingOrdersWithTopOnDateOnly_WorksCorrectly()
        {
            var orders = _context.Orders
                .OrderBy(o => o.ShipDate)
                .Take(2)
                .ToList();

            Assert.True(orders.Count <= 2);
        }

        [Fact]
        public void QueryingOrdersWithTopOnTimeOnly_WorksCorrectly()
        {
            var orders = _context.Orders
                .OrderBy(o => o.ShipTime)
                .Take(2)
                .ToList();

            Assert.True(orders.Count <= 2);
        }

        #endregion

        private void ResetDefaultDataSource()
        {
            var actionUri = new Uri(_baseUri + "edmdateandtimeofday/Default.ResetDefaultDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}
