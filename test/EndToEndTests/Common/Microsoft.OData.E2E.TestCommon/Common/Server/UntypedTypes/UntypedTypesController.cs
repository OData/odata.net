//-----------------------------------------------------------------------------
// <copyright file="UntypedTypesController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.UntypedTypes;

public class UntypedTypesController : ODataController
{
    private static UntypedTypesDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/Customers")]
    public IActionResult GetCustomers()
    {
        return Ok(_dataSource.Customers);
    }

    [EnableQuery]
    [HttpGet("odata/Customers({key})")]
    public IActionResult GetCustomer([FromODataUri] int key)
    {
        var res = _dataSource.Customers?.FirstOrDefault(a => a.Id == key);
        if (res == null)
        {
            return NotFound();
        }

        return Ok(res);
    }

    [EnableQuery]
    [HttpGet("odata/Customers({key})/UntypedProperty")]
    public IActionResult GetCustomersUntypedProperty([FromODataUri] int key)
    {
        var res = _dataSource.Customers?.FirstOrDefault(a => a.Id == key);
        if (res == null)
        {
            return NotFound();
        }

        return Ok(res.UntypedProperty);
    }

    [EnableQuery]
    [HttpGet("odata/Customers({key})/UntypedList")]
    public IActionResult GetCustomersUntypedList([FromODataUri] int key)
    {
        var res = _dataSource.Customers?.FirstOrDefault(a => a.Id == key);
        if (res == null)
        {
            return NotFound();
        }

        return Ok(res.UntypedList);
    }

    [HttpPost("odata/Customers")]
    public IActionResult CreateCustomer([FromBody] Customer customer)
    {
        _dataSource.Customers?.Add(customer);
        return Created(customer);
    }

    [HttpPatch("odata/Customers({key})")]
    public IActionResult UpdateAccount([FromODataUri] int key, [FromBody] Delta<Customer> delta)
    {
        var existing = _dataSource.Customers?.FirstOrDefault(a => a.Id == key);
        if (existing == null)
        {
            return NotFound();
        }

        var updated = delta.Patch(existing);
        return Updated(updated);
    }

    [HttpPost("odata/untypedtypes/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = UntypedTypesDataSource.CreateInstance();

        return Ok();
    }
}
