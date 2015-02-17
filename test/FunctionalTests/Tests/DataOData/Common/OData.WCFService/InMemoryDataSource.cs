//---------------------------------------------------------------------
// <copyright file="InMemoryDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Spatial;

    /// <summary>
    /// The class implements an in memory data source that provides IQueryables of model types
    /// </summary>
    public class InMemoryDataSource
    {
        private static IList<Person> people;
        private static IList<Order> orders;
        private static IList<OrderDetail> orderdetails;
        private static IList<Product> products;


        static InMemoryDataSource()
        {
            EnsureDataSource();
        }

        /// <summary>
        /// Returns an IQueryable of type Customer.
        /// </summary>
        public IQueryable<Person> People
        {
            get
            {
                return people.AsQueryable();
            }
        }

        /// <summary>
        /// Returns an IQueryable of type Customer.
        /// </summary>
        public IQueryable<Customer> Customers
        {
            get
            {
                return people.OfType<Customer>().AsQueryable();
            }
        }

        /// <summary>
        /// Returns an IQueryable of type Employee.
        /// </summary>
        public IQueryable<Employee> Employees
        {
            get
            {
                return people.OfType<Employee>().AsQueryable();
            }
        }

        /// <summary>
        /// Returns an IQueryable of type Product.
        /// </summary>
        public IQueryable<Product> Products
        {
            get
            {
                return products.AsQueryable();
            }
        }

        /// <summary>
        /// Returns an IQueryable of type Order.
        /// </summary>
        public IQueryable<Order> Orders
        {
            get
            {
                return orders.AsQueryable();
            }
        }

        /// <summary>
        /// Returns an IQueryable of type OrderDetail.
        /// </summary>
        public IQueryable<OrderDetail> OrderDetails
        {
            get
            {
                return orderdetails.AsQueryable();
            }
        }

        /// <summary>
        /// Populates the data source.
        /// </summary>
        private static void EnsureDataSource()
        {
            if (people == null)
            {
                people = new List<Person>()
                {
                    new Customer()
                    {
                         FirstName = "Bob",
                         LastName = "Cat",
                         Numbers = new Collection<string>{"111-111-1111"},
                         PersonID = 1,
                         Birthday = new DateTime(1957, 4, 3),
                         City = "London",
                         Home = GeographyPoint.Create(32.1, 23.1)
                    },
                    new Customer()
                    {
                         FirstName = "Jill",
                         LastName = "Jones",
                         Numbers = new Collection<string>{},
                         PersonID = 2,
                         Birthday = new DateTime(1983, 1, 15),
                         City = "Sydney",
                         Home = GeographyPoint.Create(15.0, 161.8)
                    },
                    new Employee()
                    {
                         FirstName = "Jacob",
                         LastName = "Zip",
                         Numbers = new Collection<string>{"333-333-3333"},
                         PersonID = 3,
                         DateHired = new DateTime(2010, 12, 13),
                         Home = GeographyPoint.Create(15.0, 161.8),
                         Office = GeographyPoint.Create(15.0, 162)
                    },
                    new Employee()
                    {
                         FirstName = "Elmo",
                         LastName = "Rogers",
                         Numbers = new Collection<string>{"444-444-4444", "555-555-5555", "666-666-6666"},
                         PersonID = 4,
                         DateHired = new DateTime(2008, 3, 27),
                         Home = GeographyPoint.Create(-15.0, -61.8),
                         Office = GeographyPoint.Create(-15.0, -62)
                    }
                };
                
                products = new List<Product>()
                {
                    new Product()
                    {
                         Name = "Cheetos",
                         ProductID = 5,
                         QuantityInStock = 100,
                         QuantityPerUnit = "100g Bag",
                         UnitPrice = 3.24f, 
                         Discontinued = true
                    },
                    new Product()
                    {
                         Name = "Mushrooms",
                         ProductID = 6,
                         QuantityInStock = 100,
                         QuantityPerUnit = "Pound",
                         UnitPrice = 3.24f,
                         Discontinued = false
                    }
                };

                orders = new List<Order>()
                {
                    new Order()
                    {
                        OrderID = 7,
                        CustomerForOrder = people.OfType<Customer>().ElementAt(1),
                        CustomerID = people.OfType<Customer>().ElementAt(1).PersonID,
                        LoggedInEmployee = people.OfType<Employee>().ElementAt(0),
                        EmployeeID = people.OfType<Employee>().ElementAt(0).PersonID,
                        OrderDate = new DateTime(2011, 5, 29, 14, 21, 12)
                    },
                    new Order()
                    {
                        OrderID = 8,
                        CustomerForOrder = people.OfType<Customer>().ElementAt(0),
                        CustomerID = people.OfType<Customer>().ElementAt(0).PersonID,
                        LoggedInEmployee = people.OfType<Employee>().ElementAt(1),
                        EmployeeID = people.OfType<Employee>().ElementAt(1).PersonID,
                        OrderDate = new DateTime(2011, 3, 4, 16, 3, 57)
                    }
                };

                orderdetails = new List<OrderDetail>()
                {
                    new OrderDetail()
                    {
                        OrderID = orders[0].OrderID,
                        AssociatedOrder = orders[0],
                        ProductID = products[0].ProductID,
                        ProductOrdered = products[0],
                        Quantity = 50,
                        UnitPrice = products[0].UnitPrice
                    },
                    new OrderDetail()
                    {
                        OrderID = orders[0].OrderID,
                        AssociatedOrder = orders[0],
                        ProductID = products[1].ProductID,
                        ProductOrdered = products[1],
                        Quantity = 2,
                        UnitPrice = products[1].UnitPrice
                    },
                    new OrderDetail()
                    {
                        OrderID = orders[1].OrderID,
                        AssociatedOrder = orders[1],
                        ProductID = products[1].ProductID,
                        ProductOrdered = products[1],
                        Quantity = 5,
                        UnitPrice = products[1].UnitPrice
                    }
                };
            }
        }
    }    
}
