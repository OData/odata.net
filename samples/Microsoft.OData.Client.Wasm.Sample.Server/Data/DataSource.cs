//---------------------------------------------------------------------
// <copyright file="DataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Client.Wasm.Sample.Server.Models;

namespace Microsoft.OData.Client.Wasm.Sample.Server.Data;

internal class DataSource
{
    private static List<Customer> customers = null!;
    private static List<Order> orders = null!;

    static DataSource()
    {
        Initialize();
    }

    private static void Initialize()
    {
        var customer1 = new Customer { Id = 1, Name = "Sue" };
        var customer2 = new Customer { Id = 2, Name = "Bob" };

        customers = new List<Customer> { customer1, customer2 };

        var order1 = new Order { Id = 1, Amount = 190, Customer = customer1 };
        var order2 = new Order { Id = 2, Amount = 230, Customer = customer2 };
        var order3 = new Order { Id = 3, Amount = 160, Customer = customer1 };
        var order4 = new Order { Id = 4, Amount = 310, Customer = customer1 };
        var order5 = new Order { Id = 5, Amount = 290, Customer = customer2 };

        orders = new List<Order>
        {
            order1,
            order2,
            order3,
            order4,
            order5
        };

        customer1.Orders = new List<Order> { order1, order3, order4 };
        customer2.Orders = new List<Order> { order2, order5 };
    }

    public static List<Customer> Customers => customers;

    public static List<Order> Orders => orders;

    public static void ResetDataSource()
    {
        Initialize();
    }
}
