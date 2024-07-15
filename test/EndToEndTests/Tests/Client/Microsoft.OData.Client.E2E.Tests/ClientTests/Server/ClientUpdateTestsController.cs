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
using Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.ClientTests.Server
{
    public class ClientUpdateTestsController : ODataController
    {
        [EnableQuery]
        [HttpGet("odata/Customers")]
        public IActionResult GetCustomers()
        {
            var customers = CommonEndToEndDataSource.Customers;
            return Ok(customers);
        }

        [EnableQuery]
        [HttpGet("odata/Customers/$count")]
        public IActionResult GetCustomersCount()
        {
            var customers = CommonEndToEndDataSource.Customers;
            return Ok(customers);
        }

        [EnableQuery]
        [HttpGet("odata/Products")]
        public IActionResult Get()
        {
            var products = CommonEndToEndDataSource.Products;
            return Ok(products);
        }

        [EnableQuery]
        [HttpGet("odata/Products({key})")]
        public IActionResult Get(int key)
        {
            var product = CommonEndToEndDataSource.Products?.FirstOrDefault(a => a.ProductId == key);
            return Ok(product);
        }

        [HttpPatch("odata/Customers({key})")]
        public IActionResult Patch([FromODataUri] int key, [FromBody] Delta<Customer> customer)
        {
            var existingCustomer = CommonEndToEndDataSource.Customers.FirstOrDefault(c => c.CustomerId == key);
            if (existingCustomer == null)
            {
                return NotFound();
            }

            customer.Patch(existingCustomer);
            return Updated(existingCustomer);
        }
    }
}
