//-----------------------------------------------------------------------------
// <copyright file="HttpClientController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.TransportLayerTests.Server
{
    public class HttpClientController : ODataController
    {
        private static CommonEndToEndDataSource _dataSource;

        [EnableQuery]
        [HttpGet("odata/Products")]
        public IActionResult Get()
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
        [HttpGet("odata/Computers")]
        public IActionResult GetComputer()
        {
            var computers = _dataSource.Computers;

            return Ok(computers);
        }

        [EnableQuery]
        [HttpGet("odata/Login")]
        public IActionResult GetLogins()
        {
            var logins = _dataSource.Logins;

            return Ok(logins);
        }

        [HttpPost("odata/Orders")]
        public IActionResult PostOrder([FromBody] Order order)
        {
            _dataSource.Orders.Add(order);
            return Created(order);
        }

        [HttpPost("odata/Products")]
        public IActionResult PostProducts([FromBody] Product product)
        {
            _dataSource.Products.Add(product);
            return Created(product);
        }

        [HttpPatch("odata/Computers({key})")]
        public IActionResult GetComputer([FromRoute] int key)
        {
            var computer = _dataSource.Computers.SingleOrDefault(a => a.ComputerId == key);

            if (computer == null)
            {
                return NotFound();
            }

            return Ok(computer);
        }

        [HttpGet("odata/Products({key})/Description")]
        public IActionResult GetProductDescription([FromRoute] int key)
        {
            var product = _dataSource.Products.SingleOrDefault(a => a.ProductId == key);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product.Description);
        }

        [HttpGet("odata/Orders({key})")]
        public IActionResult GetOrder([FromRoute] int key)
        {
            var order = _dataSource.Orders.SingleOrDefault(a => a.OrderId == key);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpGet("odata/Products({key})/RelatedProducts")]
        public IActionResult GetRelatedProducts([FromRoute] int key)
        {
            var product = _dataSource.Products.SingleOrDefault(a => a.ProductId == key);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product.RelatedProducts);
        }

        [HttpPatch("odata/Products({key})")]
        public IActionResult PatchProducts([FromRoute] int key, [FromBody] Delta<Product> delta)
        {
            var product = _dataSource.Products.SingleOrDefault(a => a.ProductId == key);

            if (product == null)
            {
                return NotFound();
            }

            var updatedProduct = delta.Patch(product);

            return Updated(updatedProduct);
        }

        [HttpPatch("odata/CustomerInfos({key})")]
        public IActionResult PatchCustomerInfos([FromRoute] int key, [FromBody] Delta<CustomerInfo> delta)
        {
            var customerInfo = _dataSource.CustomerInfos.SingleOrDefault(a => a.CustomerInfoId == key);

            if (customerInfo == null)
            {
                return NotFound();
            }

            var updatedCustomerInfo = delta.Patch(customerInfo);

            return Updated(updatedCustomerInfo);
        }

        [HttpPatch("odata/Customers({key})")]
        public IActionResult PatchCustomers([FromRoute] int key, [FromBody] Delta<Customer> delta)
        {
            var customer = _dataSource.Customers.SingleOrDefault(a => a.CustomerId == key);

            if (customer == null)
            {
                return NotFound();
            }

            var updatedCustomer = delta.Patch(customer);

            return Updated(updatedCustomer);
        }

        [HttpDelete("odata/Customers({key})")]
        public IActionResult DeleteCustomer([FromRoute] int key)
        {
            var customer = _dataSource.Customers?.SingleOrDefault(o => o.CustomerId == key);

            if (customer == null)
            {
                return NotFound();
            }

            _dataSource.Customers.Remove(customer);

            return NoContent();
        }

        [HttpPut("odata/Products({key})")]
        public IActionResult PutProduct([FromRoute] int key, [FromBody] Product product)
        {
            var existProduct = _dataSource.Products?.SingleOrDefault(o => o.ProductId == key);

            if (existProduct == null)
            {
                return NotFound();
            }

            _dataSource.Products.Remove(existProduct);
            existProduct.Description = product.Description;

            _dataSource.Products.Add(existProduct);

            return Ok(existProduct);
        }

        [HttpDelete("odata/Computers({key})")]
        public IActionResult DeleteComputer([FromRoute] int key)
        {
            var computer = _dataSource.Computers?.SingleOrDefault(o => o.ComputerId == key);

            if (computer == null)
            {
                return NotFound();
            }

            _dataSource.Computers.Remove(computer);

            return NoContent();
        }

        [HttpPost("odata/httpclient/Default.ResetDataSource")]
        public IActionResult ResetDataSource()
        {
            _dataSource = CommonEndToEndDataSource.CreateInstance();

            return Ok();
        }
    }
}
