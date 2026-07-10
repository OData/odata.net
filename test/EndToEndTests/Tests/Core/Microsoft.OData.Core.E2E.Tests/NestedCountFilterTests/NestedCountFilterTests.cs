//---------------------------------------------------------------------
// <copyright file="NestedCountFilterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.Core.E2E.Tests.NestedCountFilterTests;

/// <summary>
/// End-to-end tests that verify nested <c>$count($filter=...)</c> expressions are parsed and
/// executed correctly through the full OData pipeline (URI parsing → LINQ translation → HTTP response).
///
/// The domain is a simple e-commerce store:
///
///   Customer
///     └─ Orders        (collection navigation)
///          └─ Items    (collection navigation)
///               ├─ Price    (decimal scalar)
///               └─ Product  (single-valued navigation)
///                    └─ Reviews  (collection navigation)
///                         └─ Rating  (int scalar)
/// </summary>
public class NestedCountFilterTests : EndToEndTestBase<NestedCountFilterTests.TestsStartup>
{
    private readonly Uri _baseUri;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            var modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<StoreCustomer>("StoreCustomers");
            modelBuilder.EntitySet<StoreOrder>("StoreOrders");
            modelBuilder.EntitySet<StoreProduct>("StoreProducts");
            modelBuilder.EntityType<StoreOrderItem>();
            modelBuilder.EntityType<StoreReview>();

            services.ConfigureControllers(typeof(StoreCustomersController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", modelBuilder.GetEdmModel()));
        }
    }

    public NestedCountFilterTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        _baseUri = new Uri(Client.BaseAddress!, "odata/");
    }

    /// <summary>
    /// The primary scenario: three levels of nested <c>$count($filter=...)</c>.
    ///
    /// Find customers who have placed more than two orders that each contain at least one item
    /// whose product has at least one highly-rated review (Rating > 3):
    ///
    ///   Orders/$count($filter=            ← count orders where…
    ///     Items/$count($filter=            ←   count items where…
    ///       Product/Reviews/$count($filter=
    ///         Rating gt 3                  ←     review is highly rated
    ///       ) gt 0                         ←   …item has at least one such review
    ///     ) gt 0                           ← …order has at least one such item
    ///   ) gt 2                             ← customer has more than 2 such orders
    ///
    /// From the test data set:
    ///   Alice  – 4 qualifying orders (Wireless Mouse: ★5, ★4)  →  4 gt 2  ✓
    ///   Bob    – 0 qualifying orders (USB Cable/Mousepad, no rating > 3)  →  0 gt 2  ✗
    ///   Carol  – 3 qualifying orders (Monitor Stand: ★4)  →  3 gt 2  ✓
    ///   David  – 2 qualifying orders (boundary: equal is not greater than)  →  2 gt 2  ✗
    ///
    /// Expected: 2 customers (Alice and Carol).
    /// </summary>
    [Fact]
    public async Task CustomersWithMoreThanTwoOrdersContainingWellReviewedItemsAreReturned()
    {
        // Arrange
        //
        //   StoreCustomers?$filter=
        //     Orders/$count($filter=
        //       Items/$count($filter=
        //         Product/Reviews/$count($filter=Rating gt 3) gt 0
        //       ) gt 0
        //     ) gt 2
        //
        const string filter =
            "Orders/$count($filter=" +
                "Items/$count($filter=" +
                    "Product/Reviews/$count($filter=Rating gt 3) gt 0" +
                ") gt 0" +
            ") gt 2";

        // Act
        var count = await GetCustomerCountAsync(filter);

        // Assert
        Assert.Equal(2, count);
    }

    /// <summary>
    /// Verifies that a two-level nested filter correctly identifies customers who have placed
    /// at least one high-value order (an order containing at least one item priced above $500).
    ///
    ///   Orders/$count($filter=
    ///     Items/$count($filter=
    ///       Price gt 500
    ///     ) gt 0
    ///   ) gt 0
    ///
    /// Expected: 2 customers (Alice — Order 1 at $649.99, Carol — Order 7 at $1,249.99).
    /// </summary>
    [Fact]
    public async Task CustomersWithAtLeastOneHighValueOrderAreReturned()
    {
        const string filter =
            "Orders/$count($filter=" +
                "Items/$count($filter=Price gt 500) gt 0" +
            ") gt 0";

        var count = await GetCustomerCountAsync(filter);

        Assert.Equal(2, count);
    }

    /// <summary>
    /// Sanity check: when the inner filter is always true (every order has at least one item),
    /// all customers are returned.
    ///
    ///   Orders/$count($filter=Items/$count gt 0) gt 0
    ///
    /// Expected: all 4 customers.
    /// </summary>
    [Fact]
    public async Task AllCustomersReturnedWhenEveryOrderHasItems()
    {
        const string filter = "Orders/$count($filter=Items/$count gt 0) gt 0";

        var count = await GetCustomerCountAsync(filter);

        Assert.Equal(4, count);
    }

    /// <summary>
    /// Boundary check: when the inner filter is never satisfied (no item costs more than $10,000),
    /// no customers are returned.
    ///
    ///   Orders/$count($filter=Items/$count($filter=Price gt 10000) gt 0) gt 0
    ///
    /// Expected: 0 customers.
    /// </summary>
    [Fact]
    public async Task NoCustomersReturnedWhenNoItemExceedsThreshold()
    {
        const string filter =
            "Orders/$count($filter=" +
                "Items/$count($filter=Price gt 10000) gt 0" +
            ") gt 0";

        var count = await GetCustomerCountAsync(filter);

        Assert.Equal(0, count);
    }

    // -------------------------------------------------------------------------
    // Helper method
    // -------------------------------------------------------------------------

    private async Task<int> GetCustomerCountAsync(string filter)
    {
        var url = new Uri(_baseUri, $"StoreCustomers?$filter={Uri.EscapeDataString(filter)}");
        var response = await Client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("value").GetArrayLength();
    }
}
