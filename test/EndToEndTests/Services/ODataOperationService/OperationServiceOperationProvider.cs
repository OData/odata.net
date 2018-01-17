//---------------------------------------------------------------------
// <copyright file="OperationServiceOperationProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using Microsoft.OData.Edm;
using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

namespace Microsoft.Test.OData.Services.ODataOperationService
{

    public class OperationServiceOperationProvider : ODataReflectionOperationProvider
    {
        public void ResetDataSource()
        {
            var dataSource = DataSourceManager.GetCurrentDataSource<OperationServiceDataSource>();
            dataSource.Reset();
            dataSource.Initialize();
        }

        public Order PlaceOrder(Customer customer, Order order)
        {
            return order;
        }

        public IList<Order> PlaceOrders(Customer customer, IList<Order> orders)
        {
            return orders;
        }

        public Customer GetCustomerForAddress(IEnumerable<Customer> customers, Address address)
        {
            return customers.SingleOrDefault(c => c.Address != null && c.Address.Equals(address));
        }

        public Customer GetCustomerForAddresses(IEnumerable<Customer> customers, IEnumerable<Address> addresses)
        {
            return customers.SingleOrDefault(c => c.Address != null && addresses.Contains(c.Address));
        }

        public IEnumerable<Customer> GetCustomersForAddress(IEnumerable<Customer> customers, Address address)
        {
            return customers.Where(c => c.Address != null && c.Address.Equals(address));
        }

        public IEnumerable<Customer> GetCustomersForAddresses(IEnumerable<Customer> customers, IEnumerable<Address> addresses)
        {
            return customers.Where(c => c.Address != null && addresses.Contains(c.Address));
        }

        public Customer VerifyCustomerAddress(Customer customer, Address address)
        {
            return address != null && address.Equals(customer.Address) ? customer : null;
        }

        public Customer VerifyCustomerAddresses(Customer customer, IEnumerable<Address> addresses)
        {
            return addresses.Contains(customer.Address) ? customer : null;
        }

        public IEnumerable<Order> GetOrdersFromCustomerByNotes(Customer customer, IEnumerable<string> notes)
        {
            return customer.Orders.Where(order => order.Notes.Intersect(notes).Any());
        }

        public IEnumerable<Order> GetOrdersByNote(IEnumerable<Order> orders, string note)
        {
            return orders.Where(order => order.Notes.Contains(note));
        }

        public Order GetOrderByNote(IEnumerable<Order> orders, IEnumerable<string> notes)
        {
            return orders.SingleOrDefault(order => order.Notes.Intersect(notes).Count() == order.Notes.Count
                && order.Notes.Count == notes.Count());
        }

        public IEnumerable<Customer> GetCustomersByOrders(IEnumerable<Customer> customers, IEnumerable<Order> orders)
        {
            return customers.Where(customer => customer.Orders.Any(o => orders.Any(o2 => o2.ID == o.ID)));
        }

        public IEnumerable<Customer> GetCustomersByOrders(IEnumerable<Order> orders)
        {
            var customers = GetRootQuery("Customers") as IEnumerable<Customer>;
            return customers.Where(customer => customer.Orders.Any(o => orders.Any(o2 => o2.ID == o.ID)));
        }

        public Customer GetCustomerByOrder(IEnumerable<Customer> customers, Order order)
        {
            return customers.SingleOrDefault(customer => customer.Orders.Any(o => o.ID == order.ID));
        }

        public Customer GetCustomerByOrder(Order order)
        {
            var customers = GetRootQuery("Customers") as IEnumerable<Customer>;
            return customers.SingleOrDefault(customer => customer.Orders.Any(o => o.ID == order.ID));
        }

        public Customer VerifyCustomerByOrder(Customer customer, Order order)
        {
            return customer.Orders.Any(o => o.ID == order.ID) ? customer : null;
        }

        public Address GetCustomerAddress(Customer customer)
        {
            return customer.Address;
        }

        private static object GetRootQuery(string propertyName)
        {
            var dataSource = DataSourceManager.GetCurrentDataSource<OperationServiceDataSource>();
            return dataSource.GetType().GetProperty(propertyName).GetValue(dataSource, null);
        }
    }
}
