//---------------------------------------------------------------------
// <copyright file="QueryOptionTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.QueryOptions
{
    public class QueryOptionTestsController : ODataController
    {
        private static DefaultDataSource _dataSource;

        [EnableQuery]
        [HttpGet("odata/Products")]
        public IActionResult GetProducts()
        {
            var products = _dataSource.Products;

            return Ok(products);
        }

        [EnableQuery]
        [HttpGet("odata/Products({key})")]
        public IActionResult GetProduct([FromRoute] int key)
        {
            var product = _dataSource.Products?.SingleOrDefault(a => a.ProductID == key);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [EnableQuery]
        [HttpGet("odata/People")]
        public IActionResult GetPeople()
        {
            var people = _dataSource.People;

            return Ok(people);
        }

        [EnableQuery]
        [HttpGet("odata/People({key})")]
        public IActionResult GetPerson([FromRoute] int key)
        {
            var person = _dataSource.People?.SingleOrDefault(a => a.PersonID == key);

            if (person == null)
            {
                return NotFound();
            }

            return Ok(person);
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
        public IActionResult GetCustomer([FromRoute] int key)
        {
            var customer = _dataSource.Customers?.SingleOrDefault(a => a.PersonID == key);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})/Numbers")]
        public IActionResult GetNumbersFromCustomer([FromRoute] int key)
        {
            var customer = _dataSource.Customers?.SingleOrDefault(a => a.PersonID == key);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer.Numbers);
        }

        [EnableQuery]
        [HttpGet("odata/ProductDetails")]
        public IActionResult GetProductDetails()
        {
            var productDetails = _dataSource.ProductDetails;

            return Ok(productDetails);
        }

        [HttpPost("odata/queryoption/Default.ResetDefaultDataSource")]
        public IActionResult ResetDefaultDataSource()
        {
            _dataSource = DefaultDataSource.CreateInstance();

            return Ok();
        }
    }
}
