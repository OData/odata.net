//---------------------------------------------------------------------
// <copyright file="OperationServiceDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.OData.Edm;
using Microsoft.Test.OData.Services.ODataWCFService;
using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

namespace Microsoft.Test.OData.Services.ODataOperationService
{
    /// <summary>
    /// The class implements an in memory data source that provides IQueryables of model types
    /// </summary>
    public class OperationServiceDataSource : ODataReflectionDataSource
    {
        public OperationServiceDataSource()
        {
            this.OperationProvider = new OperationServiceOperationProvider();
        }

        #region Entity Set Resources

        public EntityCollection<Customer> Customers
        {
            get; private set;
        }

        public EntityCollection<Order> Orders
        {
            get; private set;
        }

        #endregion

        public override void Reset()
        {
            this.Customers = new EntityCollection<Customer>();
            this.Orders = new EntityCollection<Order>();
        }

        public override void Initialize()
        {
            this.Customers.AddRange(new List<Customer>
            {
                new Customer()
                {
                    ID = 1,
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
                    ID = 2,
                    FirstName = "Jill",
                    LastName = "Jones",
                    Emails = new Collection<string>(),
                },
                new Customer()
                {
                    ID = 3,
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
            });

            this.Orders.AddRange(new List<Order>()
            {
                new Order()
                {
                    ID = 0,
                    OrderDate = new DateTimeOffset(2011, 5, 29, 14, 21, 12, TimeSpan.FromHours(-8)),
                    Notes = new List<string>{"1111", "child"},
                    OrderDetails = new List<OrderDetail>{},
                    InfoFromCustomer = new InfoFromCustomer(){ CustomerMessage = "XXL" }
                },
                new Order()
                {
                    ID = 1,
                    OrderDate = new DateTimeOffset(2011, 3, 4, 16, 3, 57, TimeSpan.FromHours(-8)),
                    Notes = new List<string>(),
                    OrderDetails = new List<OrderDetail>{ new OrderDetail { Quantity = 1, UnitPrice = 1.0f }}
                },
                new Order()
                {
                    ID = 2,
                    OrderDate = new DateTimeOffset(2011, 3, 4, 16, 3, 57, TimeSpan.FromHours(-8)),
                    Notes = new List<string>{"1111", "parent"},
                    OrderDetails = new List<OrderDetail>{ new OrderDetail { Quantity = 1, UnitPrice = 1.0f }},
                    InfoFromCustomer = new InfoFromCustomer(){ CustomerMessage = "XXL" }

                },
                new Order()
                {
                    ID = 3,
                    OrderDate = new DateTimeOffset(2011, 3, 4, 16, 3, 57, TimeSpan.FromHours(-8)),
                    Notes = new List<string>{"child", "parent"},
                    OrderDetails = new List<OrderDetail>{ new OrderDetail { Quantity = 1, UnitPrice = 1.0f }},
                    InfoFromCustomer = new InfoFromCustomer(){ CustomerMessage = "XXL" }
                },
            });

            this.Customers[2].Orders.Add(this.Orders[0]);
            this.Orders[2].Customer = this.Customers[0];
            this.Customers[1].Orders.Add(this.Orders[1]);
            this.Orders[1].Customer = this.Customers[1];
        }

        protected override IEdmModel CreateModel()
        {
            return OperationServiceInMemoryModel.CreateODataServiceModel("Microsoft.Test.OData.Services.ODataOperationService");
        }
    }
}