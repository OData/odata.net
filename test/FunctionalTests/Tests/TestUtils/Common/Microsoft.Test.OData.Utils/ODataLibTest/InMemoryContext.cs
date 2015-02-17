//---------------------------------------------------------------------
// <copyright file="InMemoryContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.ODataLibTest
{
    using Microsoft.OData.Client;
    using System;

    /// <summary>
    /// Data Service Types
    /// </summary>
    public partial class InMemoryContext : DataServiceContext
    {
        /// <summary>
        /// Initialize a new InMemoryEntities object.
        /// </summary>
        public InMemoryContext(Uri serviceRoot) :
            base(serviceRoot)
        {
            this.OnContextCreated();
        }
        partial void OnContextCreated();

        /// <summary>
        /// There are no comments for Customers in the schema.
        /// </summary>
        
        public DataServiceQuery<Customer> Customers
        {
            get
            {
                if ((this._Customers == null))
                {
                    this._Customers = base.CreateQuery<Customer>("Customers");
                }
                return this._Customers;
            }
        }
        private DataServiceQuery<Customer> _Customers;

        /// <summary>
        /// There are no comments for Employees in the schema.
        /// </summary>
        public DataServiceQuery<Employee> Employees
        {
            get
            {
                if ((this._Employees == null))
                {
                    this._Employees = base.CreateQuery<Employee>("Employees");
                }
                return this._Employees;
            }
        }
        private DataServiceQuery<Employee> _Employees;

        /// <summary>
        /// There are no comments for Order_Details in the schema.
        /// </summary>
        public DataServiceQuery<OrderDetail> OrderDetails
        {
            get
            {
                if ((this._OrderDetails == null))
                {
                    this._OrderDetails = base.CreateQuery<OrderDetail>("OrderDetails");
                }
                return this._OrderDetails;
            }
        }
        private DataServiceQuery<OrderDetail> _OrderDetails;

        /// <summary>
        /// There are no comments for Orders in the schema.
        /// </summary>
        public DataServiceQuery<Order> Orders
        {
            get
            {
                if ((this._Orders == null))
                {
                    this._Orders = base.CreateQuery<Order>("Orders");
                }
                return this._Orders;
            }
        }
        private DataServiceQuery<Order> _Orders;

        /// <summary>
        /// There are no comments for Products in the schema.
        /// </summary>
        public DataServiceQuery<Product> Products
        {
            get
            {
                if ((this._Products == null))
                {
                    this._Products = base.CreateQuery<Product>("Products");
                }
                return this._Products;
            }
        }
        private DataServiceQuery<Product> _Products;

        /// <summary>
        /// There are no comments for Customers in the schema.
        /// </summary>
        public void AddToCustomers(Customer customer)
        {
            base.AddObject("Customers", customer);
        }

        /// <summary>
        /// There are no comments for Employees in the schema.
        /// </summary>
        public void AddToEmployees(Employee employee)
        {
            base.AddObject("Employees", employee);
        }

        /// <summary>
        /// There are no comments for Order_Details in the schema.
        /// </summary>
        public void AddToOrderDetails(OrderDetail order_Detail)
        {
            base.AddObject("OrderDetails", order_Detail);
        }

        /// <summary>
        /// There are no comments for Orders in the schema.
        /// </summary>
        public void AddToOrders(Order order)
        {
            base.AddObject("Orders", order);
        }

        /// <summary>
        /// There are no comments for Products in the schema.
        /// </summary>
        public void AddToProducts(Product product)
        {
            base.AddObject("Products", product);
        }
    }
}
