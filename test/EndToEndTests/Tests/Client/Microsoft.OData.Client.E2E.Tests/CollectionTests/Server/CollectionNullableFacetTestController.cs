//-----------------------------------------------------------------------------
// <copyright file="CollectionNullableFacetTestController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;

namespace Microsoft.OData.Client.E2E.Tests.CollectionTests.Server
{
    public class CollectionNullableFacetTestController : ODataController
    {
        private static DefaultDataSource _dataSource;

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
            var customer = _dataSource.Customers.SingleOrDefault(a => a.PersonID == key);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [EnableQuery]
        [HttpPut("odata/Customers({key})")]
        public IActionResult UpdateCustomers([FromRoute] int key, [FromBody] Customer customer)
        {
            var updateCustomer = _dataSource.Customers.FirstOrDefault(a => a.PersonID == key);

            if (updateCustomer == null)
            {
                return NotFound();
            }

            updateCustomer.Numbers = customer.Numbers;
            updateCustomer.Emails = customer.Emails;

            return NoContent();
        }

        [HttpPost("odata/collectionnullable/Default.ResetDefaultDataSource")]
        public IActionResult ResetDefaultDataSource()
        {
            _dataSource = DefaultDataSource.CreateInstance();

            return Ok();
        }
    }
}
