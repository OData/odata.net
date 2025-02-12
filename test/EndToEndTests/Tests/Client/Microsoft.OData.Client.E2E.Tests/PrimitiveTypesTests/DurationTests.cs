//---------------------------------------------------------------------
// <copyright file="DurationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.PrimitiveTypes;
using Microsoft.OData.Edm;
using System.Collections.ObjectModel;
using System.Xml;
using Xunit;
using ClientDefaultModel = Microsoft.OData.E2E.TestCommon.Common.Client.Default;

namespace Microsoft.OData.Client.E2E.Tests.PrimitiveTypesTests;

public class DurationTests : EndToEndTestBase<DurationTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(PrimitiveTypesTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
        }
    }

    public DurationTests(TestWebApplicationFactory<TestsStartup> fixture)
        : base(fixture)
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

        _model = DefaultEdmModel.GetEdmModel();
        ResetDefaultDataSource();
    }

    #region Insert and update property value

    [Fact]
    public async Task InsertAndUpdatePropertyValueTest()
    {
        // Query with filter
        var timespan = new TimeSpan(new Random().Next());
        var orderQueryable = _context.Orders.Where(c => c.ShelfLife == timespan) as DataServiceQuery<ClientDefaultModel.Order>;
        Assert.NotNull(orderQueryable);
        Assert.EndsWith($"/Orders?$filter=ShelfLife eq duration'{XmlConvert.ToString(timespan)}'", orderQueryable.RequestUri.OriginalString, StringComparison.Ordinal);
        Assert.Empty(orderQueryable.ToList());

        int orderID = new Random().Next();

        // Create an entity
        var order = new ClientDefaultModel.Order()
        {
            OrderID = orderID,
            OrderDate = new DateTimeOffset(new DateTime(2011, 3, 4, 16, 3, 57)),
            ShelfLife = timespan,
            OrderShelfLifes = new ObservableCollection<TimeSpan>() { timespan }
        };
        _context.AddToOrders(order);
        await _context.SaveChangesAsync();

        // Query and verify
        var result = orderQueryable.ToList();
        Assert.Single(result);
        Assert.Equal(orderID, result[0].OrderID);

        // Update the Duration properties
        timespan = new TimeSpan(new Random().Next());
        order.ShelfLife = timespan;
        order.OrderShelfLifes = new ObservableCollection<TimeSpan>() { timespan };
        _context.UpdateObject(order);
        await _context.SaveChangesAsync(SaveChangesOptions.ReplaceOnUpdate);

        // Query Duration property
        var shelfLifeQueryable = _context.Orders.Where(c => c.OrderID == orderID).Select(c => new { c.ShelfLife }).FirstOrDefault();
        Assert.NotNull(shelfLifeQueryable);
        Assert.NotNull(shelfLifeQueryable.ShelfLife);
        Assert.Equal(timespan, shelfLifeQueryable.ShelfLife);

        // Query collection of Duration property
        var orderShelfLifesQueryable = (from c in _context.Orders
                                        where c.OrderID == orderID
                                        select new { c.OrderShelfLifes }).FirstOrDefault();

        Assert.NotNull(orderShelfLifesQueryable);
        Assert.NotNull(orderShelfLifesQueryable.OrderShelfLifes);
        Assert.Single(orderShelfLifesQueryable.OrderShelfLifes);
        Assert.Equal(timespan, orderShelfLifesQueryable.OrderShelfLifes[0]);

        // Delete entity and validate
        _context.DeleteObject(order);
        await _context.SaveChangesAsync(SaveChangesOptions.ReplaceOnUpdate);

        var queryable = _context.Execute<Order>(new Uri($"Orders()?$filter=ShelfLife eq duration'{XmlConvert.ToString(timespan)}'", UriKind.Relative));
        Assert.Empty(queryable);
    }

    #endregion

    #region Private methods

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "primitivetypes/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
