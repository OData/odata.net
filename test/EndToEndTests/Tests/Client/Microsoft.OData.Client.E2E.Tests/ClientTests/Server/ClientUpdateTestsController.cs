//-----------------------------------------------------------------------------
// <copyright file="ClientUpdateTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.ClientTests.Server
{
    public class ClientUpdateTestsController : ODataController
    {
        private static CommonEndToEndDataSource _dataSource;

        [EnableQuery]
        [HttpGet("odata/Customers")]
        public IActionResult GetCustomers()
        {
            var customers = _dataSource.Customers;

            return Ok(customers);
        }

        [EnableQuery]
        [HttpGet("odata/Customers/$count")]
        public IActionResult GetCustomersCount()
        {
            var customers = _dataSource.Customers;

            return Ok(customers);
        }

        [EnableQuery]
        [HttpGet("odata/Products")]
        public IActionResult Get()
        {
            var products = _dataSource.Products;

            return Ok(products);
        }

        [EnableQuery]
        [HttpGet("odata/Products({key})")]
        public IActionResult Get(int key)
        {
            var product = _dataSource.Products?.FirstOrDefault(a => a.ProductId == key);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPatch("odata/Customers({key})")]
        public IActionResult Patch([FromODataUri] int key, [FromBody] Delta<Customer> customer)
        {
            var existingCustomer = _dataSource.Customers.FirstOrDefault(c => c.CustomerId == key);
            
            if (existingCustomer == null)
            {
                return NotFound();
            }

            customer.Patch(existingCustomer);

            return Updated(existingCustomer);
        }

        [HttpPatch("odata/Products({key})")]
        public IActionResult PatchProducts([FromODataUri] int key, [FromBody] Delta<Product> product)
        {
            var existingProduct = _dataSource.Products.FirstOrDefault(c => c.ProductId == key);

            if (existingProduct == null)
            {
                return NotFound();
            }

            product.Patch(existingProduct);

            return Updated(existingProduct);
        }

        [HttpPost("odata/clientupdate/Default.ResetDataSource")]
        public IActionResult ResetDataSource()
        {
            _dataSource = CommonEndToEndDataSource.CreateInstance();

            return Ok();
        }
    }
}
