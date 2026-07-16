//---------------------------------------------------------------------
// <copyright file="NestedCountFilterDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.E2E.Tests.NestedCountFilterTests;

// Products with Rating > 3 are "well-reviewed": wirelessMouse (5,4) and monitorStand (4).
// usbCable (2,3) and mousepad (1) have no ratings above 3.
//
// A "qualifying order" contains at least one item whose product is well-reviewed.
//   Alice : 4 qualifying orders (all wirelessMouse)          -> 4 gt 2, qualifies
//   Bob   : 0 qualifying orders (all usbCable/mousepad)      -> 0 gt 2, does not qualify
//   Carol : 3 qualifying orders (all monitorStand)           -> 3 gt 2, qualifies
//   David : 2 qualifying orders (wirelessMouse/monitorStand) -> 2 gt 2, does not qualify (boundary)
//
// Orders 1 and 7 carry a premium-priced item (Price > 500), all others do not.
public static class NestedCountFilterDataSource
{
    public static readonly IReadOnlyList<StoreProduct> Products;
    public static readonly IReadOnlyList<StoreCustomer> Customers;

    static NestedCountFilterDataSource()
    {
        var wirelessMouse = new StoreProduct
        {
            Id = 1, Name = "Wireless Mouse",
            Reviews = new List<StoreReview>
            {
                new StoreReview { Id = 1, Rating = 5 },
                new StoreReview { Id = 2, Rating = 4 },
            }
        };

        var usbCable = new StoreProduct
        {
            Id = 2, Name = "USB Cable",
            Reviews = new List<StoreReview>
            {
                new StoreReview { Id = 3, Rating = 2 },
                new StoreReview { Id = 4, Rating = 3 },
            }
        };

        var monitorStand = new StoreProduct
        {
            Id = 3, Name = "Monitor Stand",
            Reviews = new List<StoreReview>
            {
                new StoreReview { Id = 5, Rating = 4 },
            }
        };

        var mousepad = new StoreProduct
        {
            Id = 4, Name = "Mousepad",
            Reviews = new List<StoreReview>
            {
                new StoreReview { Id = 6, Rating = 1 },
            }
        };

        Products = new List<StoreProduct> { wirelessMouse, usbCable, monitorStand, mousepad };

        // Alice: 4 orders, all wirelessMouse (well-reviewed). Order 1 has a premium item (Price > 500).
        var alice = new StoreCustomer
        {
            Id = 1, Name = "Alice",
            Orders = new List<StoreOrder>
            {
                new StoreOrder { Id = 1, Items = new List<StoreOrderItem> { new StoreOrderItem { Id =  1, Price =  649.99m, Product = wirelessMouse } } },
                new StoreOrder { Id = 2, Items = new List<StoreOrderItem> { new StoreOrderItem { Id =  2, Price =   89.99m, Product = wirelessMouse } } },
                new StoreOrder { Id = 3, Items = new List<StoreOrderItem> { new StoreOrderItem { Id =  3, Price =   45.99m, Product = wirelessMouse } } },
                new StoreOrder { Id = 4, Items = new List<StoreOrderItem> { new StoreOrderItem { Id =  4, Price =   29.99m, Product = wirelessMouse } } },
            }
        };

        // Bob: 2 orders, all usbCable/mousepad (not well-reviewed).
        var bob = new StoreCustomer
        {
            Id = 2, Name = "Bob",
            Orders = new List<StoreOrder>
            {
                new StoreOrder { Id = 5, Items = new List<StoreOrderItem> { new StoreOrderItem { Id =  5, Price =  19.99m, Product = usbCable } } },
                new StoreOrder { Id = 6, Items = new List<StoreOrderItem> { new StoreOrderItem { Id =  6, Price =   9.99m, Product = mousepad } } },
            }
        };

        // Carol: 3 orders, all monitorStand (well-reviewed). Order 7 has a premium item (Price > 500).
        var carol = new StoreCustomer
        {
            Id = 3, Name = "Carol",
            Orders = new List<StoreOrder>
            {
                new StoreOrder { Id =  7, Items = new List<StoreOrderItem> { new StoreOrderItem { Id =  7, Price = 1249.99m, Product = monitorStand } } },
                new StoreOrder { Id =  8, Items = new List<StoreOrderItem> { new StoreOrderItem { Id =  8, Price =   79.99m, Product = monitorStand } } },
                new StoreOrder { Id =  9, Items = new List<StoreOrderItem> { new StoreOrderItem { Id =  9, Price =   49.99m, Product = monitorStand } } },
            }
        };

        // David: 2 orders with well-reviewed products — exactly at the boundary (2 gt 2 is false).
        var david = new StoreCustomer
        {
            Id = 4, Name = "David",
            Orders = new List<StoreOrder>
            {
                new StoreOrder { Id = 10, Items = new List<StoreOrderItem> { new StoreOrderItem { Id = 10, Price = 199.99m, Product = wirelessMouse } } },
                new StoreOrder { Id = 11, Items = new List<StoreOrderItem> { new StoreOrderItem { Id = 11, Price = 179.99m, Product = monitorStand } } },
            }
        };

        Customers = new List<StoreCustomer> { alice, bob, carol, david };
    }
}
