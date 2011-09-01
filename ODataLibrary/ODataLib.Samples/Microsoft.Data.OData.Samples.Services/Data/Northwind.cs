//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Services;

namespace Microsoft.Data.OData.Samples.Services.Data
{
    public class NorthwindData
    {
        /// <summary>
        /// This method creates the initial data to populate the  OData OM types.
        /// </summary>
        public NorthwindData()
        {
            _orders = new List<Order>(){
                new Order() { OrderId = 1, OrderDate = new DateTime(2001, 01, 01), RequiredDate = new DateTime(1991, 12, 01), ShipAddress = new Address() { City = "OrderCity", PostalCode = "98001", Street = "First" } },
                new Order() { OrderId = 2, OrderDate = new DateTime(2002, 02, 02), RequiredDate = new DateTime(1992, 12, 02), ShipAddress = new Address() { City = "OrderCity", PostalCode = "98002", Street = "Second" } },
                new Order() { OrderId = 3, OrderDate = new DateTime(2003, 03, 03), RequiredDate = new DateTime(1993, 12, 03), ShipAddress = new Address() { City = "OrderCity", PostalCode = "98003", Street = "Third" } },
                new Order() { OrderId = 4, OrderDate = new DateTime(2004, 04, 04), RequiredDate = new DateTime(1994, 12, 04), ShipAddress = new Address() { City = "OrderCity", PostalCode = "98004", Street = "Fourth" } },
                new Order() { OrderId = 5, OrderDate = new DateTime(2005, 05, 05), RequiredDate = new DateTime(1995, 12, 05), ShipAddress = new Address() { City = "OrderCity", PostalCode = "98005", Street = "Fifth" } },
                new Order() { OrderId = 6, OrderDate = new DateTime(2006, 06, 06), RequiredDate = new DateTime(1996, 12, 06), ShipAddress = new Address() { City = "OrderCity", PostalCode = "98006", Street = "Sixth" } },
            };
            _customers = new List<Customer>()
            {
                new Customer() { 
                    CustomerID = "01", 
                    CompanyName = "FirstTech", 
                    ContactName = "Able", 
                    ContactTitle = "President", 
                    ModifiedDate = DateTime.Today, 
                    Address = new Address() { City = "CustomerTown", PostalCode = "98001", Street = "First" },
                },
                new Customer() { 
                    CustomerID = "02", 
                    CompanyName = "SecondTech", 
                    ContactName = "Bob", 
                    ContactTitle = "Vice-President", 
                    ModifiedDate = DateTime.Today, 
                    Address = new Address() { City = "CustomerTown", PostalCode = "98002", Street = "Second" },
                },
                new Customer() { CustomerID = "03", 
                    CompanyName = "ThirdTech", 
                    ContactName = "Charley", 
                    ContactTitle = "General Manager", 
                    ModifiedDate = DateTime.Today, 
                    Address = new Address() { City = "CustomerTown", PostalCode = "98003", Street = "Third" },
                },
                new Customer() { CustomerID = "04", 
                    CompanyName = "FourthTech", 
                    ContactName = "Dan", 
                    ContactTitle = "Director", 
                    ModifiedDate = DateTime.Today, 
                    Address = new Address() { City = "CustomerTown", PostalCode = "98004", Street = "Fourth" },
                },
            };
            _employees = new List<Employee>()
            {
                new Employee() { EmployeeID = 1, EmployeeName = "Aaron", Address = new Address() { City = "Toronto", PostalCode = "11111", Street = "FirstBlvd" }},
                new Employee() { EmployeeID = 2, EmployeeName = "Brandy", Address = new Address() { City = "Toronto", PostalCode = "22222", Street = "SecondBlvd" }},
            };
            _customers[0].Orders = new List<Order>() { _orders[0], _orders[1], _orders[3], _orders[4], _orders[5], _orders[2] };
            _customers[0].Employee = _employees[0];
            _customers[1].Employee = _employees[1];
            _employees[0].Customers = new List<Customer>() { _customers[0] };
            _employees[1].Customers = new List<Customer>() { _customers[1] };
            _orders[0].Customer = _customers[0];
            _orders[1].Customer = _customers[0];
            _orders[2].Customer = _customers[0];
            _orders[3].Customer = _customers[0];
            _orders[4].Customer = _customers[0];
            _orders[5].Customer = _customers[0];
        }

        /// <summary>
        /// The list of customers used in the samples
        /// </summary>
        private List<Customer> _customers;

        /// <summary>
        /// The list of orders used in the samples
        /// </summary>
        private List<Order> _orders;

        /// <summary>
        /// The list of employees used in the samples
        /// </summary>
        private List<Employee> _employees;

        public IQueryable<Employee> Employees
        {
            get
            {
                return _employees.AsQueryable<Employee>();
            }
        }

        public IQueryable<Order> Orders
        {
            get
            {
                return _orders.AsQueryable<Order>();
            }
        }

        public IQueryable<Customer> Customers
        {
            get
            {
                return _customers.AsQueryable<Customer>();
            }
        }
    }
}
