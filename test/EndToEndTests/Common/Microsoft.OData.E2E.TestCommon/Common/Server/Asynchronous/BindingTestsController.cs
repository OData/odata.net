//---------------------------------------------------------------------
// <copyright file="BindingTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.Asynchronous;

public class BindingTestsController : ODataController
{
    private static CommonEndToEndDataSource _dataSource;

    [EnableQuery(MaxAnyAllExpressionDepth = 3)]
    [HttpGet("odata/Customers")]
    public IActionResult GetCustomers()
    {
        return Ok(_dataSource.Customers);
    }

    [EnableQuery]
    [HttpGet("odata/Customers({key})/Orders")]
    public IActionResult GetCustomerOrders([FromODataUri] int key)
    {
        var customer = _dataSource.Customers?.SingleOrDefault(c => c.CustomerId == key);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer.Orders);
    }

    [HttpPost("odata/Customers")]
    public IActionResult AddCustomer([FromBody] Customer customer)
    {
        _dataSource.Customers?.Add(customer);
        return Created(customer);
    }

    [HttpPatch("odata/Customers({key})")]
    public IActionResult PatchCustomer([FromODataUri] int key, [FromBody] Delta<Customer> delta)
    {
        var customer = _dataSource.Customers?.FirstOrDefault(c => c.CustomerId == key);
        if (customer == null)
        {
            return NotFound();
        }

        var updated = delta.Patch(customer);

        var preferHeader = Request.Headers["Prefer"];
        if(preferHeader.ToString().Contains("return=minimal", StringComparison.OrdinalIgnoreCase))
        {
            return Updated(updated);
        }

        return Ok(updated);
    }

    [HttpDelete("odata/Customers({key})")]
    public IActionResult DeleteCustomer([FromODataUri] int key)
    {
        var person = _dataSource.Customers?.FirstOrDefault(c => c.CustomerId == key);
        if (person == null)
        {
            return NotFound();
        }

        _dataSource.Customers?.Remove(person);
        return NoContent();
    }

    [HttpPost("odata/bindingtests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = CommonEndToEndDataSource.CreateInstance();
        return Ok();
    }
}
