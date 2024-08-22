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
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;

namespace Microsoft.OData.Client.E2E.Tests.ClientTests.Server
{
    public class ClientTestsController : ODataController
    {
        private static DefaultDataSource _dataSource;

        [EnableQuery]
        [HttpGet("odata/People")]
        public IActionResult Get()
        {
            var people = _dataSource.People;

            return Ok(people);
        }

        [EnableQuery]
        [HttpGet("odata/People({key})")]
        public IActionResult Get([FromRoute] int key)
        {
            var person = _dataSource.People?.FirstOrDefault(a => a.PersonID == key);

            if (person == null)
            {
                return NotFound();
            }

            return Ok(person);
        }

        [HttpDelete("odata/People({key})")]
        public IActionResult DeletePerson(int key)
        {
            var person = _dataSource.People.SingleOrDefault(a => a.PersonID == key);

            if (person == null)
            {
                return NotFound();
            }

            _dataSource.People.Remove(person);

            return NoContent();
        }

        [HttpDelete("odata/People({key})/Parent")]
        public IActionResult DeleteParent(int key)
        {
            var person = _dataSource.People.SingleOrDefault(a => a.PersonID == key);

            if (person == null)
            {
                return NotFound();
            }

            _dataSource.People.Remove(person);

            return NoContent();
        }

        [EnableQuery]
        [HttpGet("odata/Products")]
        public IActionResult GetProducts()
        {
            var products = _dataSource.Products;

            return Ok(products);
        }

        [EnableQuery]
        [HttpGet("odata/Orders")]
        public IActionResult GetOrders()
        {
            var orders = _dataSource.Orders;

            return Ok(orders);
        }

        [EnableQuery]
        [HttpGet("odata/Customers")]
        public IActionResult GetCustomers()
        {
            var customers = _dataSource.Customers;

            return Ok(customers);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})")]
        public IActionResult GetCustomer(int key)
        {
            var customer = _dataSource.Customers.SingleOrDefault(a => a.PersonID == key);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [EnableQuery]
        [HttpGet("odata/ProductDetails")]
        public IActionResult GetProductDetails()
        {
            var productDetails = _dataSource.ProductDetails;

            return Ok(productDetails);
        }

        [EnableQuery]
        [HttpGet("odata/OrderDetails")]
        public IActionResult GetOrderDetails()
        {
            var orderDetails = _dataSource.OrderDetails;

            return Ok(orderDetails);
        }

        [EnableQuery]
        [HttpGet("odata/OrderDetails(orderId={orderId},productId={productId})")]
        public IActionResult GetOrderDetail([FromODataUri] int orderId, [FromODataUri] int productId)
        {
            var orderDetail = _dataSource.OrderDetails.SingleOrDefault(a => a.OrderID == orderId && a.ProductID == productId);
            
            if (orderDetail == null)
            {
                return NotFound();
            }

            return Ok(orderDetail);
        }

        [HttpPost("odata/clienttests/Default.ResetDefaultDataSource")]
        public IActionResult ResetDefaultDataSource()
        {
            _dataSource = DefaultDataSource.CreateInstance();

            return Ok();
        }
    }
}
