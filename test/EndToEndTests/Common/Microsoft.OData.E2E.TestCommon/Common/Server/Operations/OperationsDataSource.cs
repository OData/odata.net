//---------------------------------------------------------------------
// <copyright file="OperationsDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.Operations;

public class OperationsDataSource
{
    public static OperationsDataSource CreateInstance()
    {
        return new OperationsDataSource();
    }

    public OperationsDataSource()
    {
        ResetDataSource();
        Initialize();
    }

    public IList<Customer>? Customers { get; private set; }
    public IList<Order>? Orders { get; private set; }

    /// <summary>
    /// Populates the data source.
    /// </summary>
    public void Initialize()
    {
        this.Customers =
        [
            new Customer()
            {
                CustomerID = 1,
                FirstName = "Bob",
                LastName = "Cat",
                Emails = new Collection<string> { "abc@abc.com" },

                Address = new HomeAddress()
                {
                    City = "Tokyo",
                    PostalCode = "98052",
                    Street = "1 Microsoft Way",
                    FamilyName = "Cats",
                },
            },
            new Customer()
            {
                CustomerID = 2,
                FirstName = "Jill",
                LastName = "Jones",
                Emails = new Collection<string>(),
            },
            new Customer()
            {
                CustomerID = 3,
                FirstName = "Jacob",
                LastName = "Zip",
                Emails = new Collection<string> { null },
                Address = new Address()
                {
                    City = "Sydney",
                    PostalCode = "98052",
                    Street = "1 Microsoft Way",
                }
            },
        ];

        this.Orders =
        [
            new Order()
            {
                OrderID = 0,
                OrderDate = new DateTimeOffset(2011, 5, 29, 14, 21, 12, TimeSpan.FromHours(-8)),
                Notes = new List<string>{"1111", "child"},
                OrderDetails = new List<OrderDetail>{},
                InfoFromCustomer = new InfoFromCustomer(){ CustomerMessage = "XXL" }
            },
            new Order()
            {
                OrderID = 1,
                OrderDate = new DateTimeOffset(2011, 3, 4, 16, 3, 57, TimeSpan.FromHours(-8)),
                Notes = new List<string>(),
                OrderDetails = new List<OrderDetail>{ new OrderDetail { Quantity = 1, UnitPrice = 1.0f }}
            },
            new Order()
            {
                OrderID = 2,
                OrderDate = new DateTimeOffset(2011, 3, 4, 16, 3, 57, TimeSpan.FromHours(-8)),
                Notes = new List<string>{"1111", "parent"},
                OrderDetails = new List<OrderDetail>{ new OrderDetail { Quantity = 1, UnitPrice = 1.0f }},
                InfoFromCustomer = new InfoFromCustomer(){ CustomerMessage = "XXL" }

            },
            new Order()
            {
                OrderID = 3,
                OrderDate = new DateTimeOffset(2011, 3, 4, 16, 3, 57, TimeSpan.FromHours(-8)),
                Notes = new List<string>{"child", "parent"},
                OrderDetails = new List<OrderDetail>{ new OrderDetail { Quantity = 1, UnitPrice = 1.0f }},
                InfoFromCustomer = new InfoFromCustomer(){ CustomerMessage = "XXL" }
            },
        ];

        this.Customers[2].Orders.Add(this.Orders[0]);
        this.Orders[2].Customer = this.Customers[0];
        this.Customers[1].Orders.Add(this.Orders[1]);
        this.Orders[1].Customer = this.Customers[1];
    }

    /// <summary>
    /// Resets the data source
    /// </summary>
    public void ResetDataSource()
    {
        this.Customers = new List<Customer>();
        this.Orders = new List<Order>();
    }
}
