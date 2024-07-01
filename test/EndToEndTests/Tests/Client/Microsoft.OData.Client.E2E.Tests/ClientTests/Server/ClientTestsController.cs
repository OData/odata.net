//-----------------------------------------------------------------------------
// <copyright file="ClientTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server;

namespace Microsoft.OData.Client.E2E.Tests.ClientTests.Server
{
    public class ClientTestsController : ODataController
    {
        [EnableQuery]
        [HttpGet("odata/People")]
        public IActionResult Get()
        {
            var people = DefaultDataSource.People;
            return Ok(people);
        }

        [EnableQuery]
        [HttpGet("odata/People({key})")]
        public IActionResult Get([FromRoute] int key)
        {
            var person = DefaultDataSource.People?.FirstOrDefault(a => a.PersonID == key);
            return Ok(person);
        }

        [HttpDelete("odata/People({key})/Parent")]
        public IActionResult DeleteParent(int key)
        {
            var person = DefaultDataSource.People.SingleOrDefault(a => a.PersonID == key);

            if (person == null)
            {
                return NotFound();
            }

            DefaultDataSource.People.Remove(person);

            return NoContent();
        }

        [EnableQuery]
        [HttpGet("odata/Products")]
        public IActionResult GetProducts()
        {
            var products = DefaultDataSource.Products;
            return Ok(products);
        }

        [EnableQuery]
        [HttpGet("odata/Orders")]
        public IActionResult GetOrders()
        {
            var orders = DefaultDataSource.Orders;
            return Ok(orders);
        }

        [EnableQuery]
        [HttpGet("odata/Customers")]
        public IActionResult GetCustomers()
        {
            var customers = DefaultDataSource.Customers;
            return Ok(customers);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})")]
        public IActionResult GetCustomer(int key)
        {
            var customer = DefaultDataSource.Customers.SingleOrDefault(a=>a.PersonID == key);
            return Ok(customer);
        }

        [EnableQuery]
        [HttpGet("odata/ProductDetails")]
        public IActionResult GetProductDetails()
        {
            var productDetails = DefaultDataSource.ProductDetails;
            return Ok(productDetails);
        }

        [EnableQuery]
        [HttpGet("odata/OrderDetails")]
        public IActionResult GetOrderDetails()
        {
            var orderDetails = DefaultDataSource.OrderDetails;
            return Ok(orderDetails);
        }

        [EnableQuery]
        [HttpGet("odata/OrderDetails(OrderID={OrderID},ProductID={ProductID})")]
        public IActionResult GetOrderDetail([FromODataUri]int OrderID, [FromODataUri]int ProductID)
        {
            var orderDetail = DefaultDataSource.OrderDetails.SingleOrDefault(a=>a.OrderID == OrderID && a.ProductID == ProductID);
            return Ok(orderDetail);
        }
    }
}
