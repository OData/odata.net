//-----------------------------------------------------------------------------
// <copyright file="AsyncRegressionTestsDataSource.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.Client.E2E.Tests.AsyncRegressionTests.Server;

public class AsyncRegressionTestsDataSource
{

    private static List<Customer> customers = null!;
    private static List<Order> orders = null!;

    public static AsyncRegressionTestsDataSource CreateInstance()
    {
        return new AsyncRegressionTestsDataSource();
    }

    public AsyncRegressionTestsDataSource()
    {
        ResetDataSource();
        Initialize();
    }

    public List<Customer> Customers => customers;

    public List<Order> Orders => orders;

    /// <summary>
    /// Populate the data source.
    /// </summary>
    public void Initialize()
    {
        var customer1 = new Customer { Id = 1, Name = "Sue" };
        var customer2 = new Customer { Id = 2, Name = "Bob" };

        customers = new List<Customer> { customer1, customer2 };

        orders = new List<Order>
        {
            new Order { Id = 1, Amount = 190, Customer = customer1 },
            new Order { Id = 2, Amount = 230, Customer = customer2 },
            new Order { Id = 3, Amount = 160, Customer = customer1 },
            new Order { Id = 4, Amount = 310, Customer = customer1 },
            new Order { Id = 5, Amount = 290, Customer = customer2 }
        };

        customer1.Orders.AddRange(orders.Where(o => o.Customer == customer1));
        customer2.Orders.AddRange(orders.Where(o => o.Customer == customer2));
    }

    /// <summary>
    /// Resets the data source
    /// </summary>
    public void ResetDataSource()
    {
        customers?.Clear();
        orders?.Clear();
    }
}
