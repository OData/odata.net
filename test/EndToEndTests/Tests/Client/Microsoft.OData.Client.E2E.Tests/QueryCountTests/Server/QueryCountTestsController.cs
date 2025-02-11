//-----------------------------------------------------------------------------
// <copyright file="QueryCountTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.QueryCountTests.Server;

public class QueryCountTestsController : ODataController
{
    private static CommonEndToEndDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/Customers")]
    public IActionResult GetCustomers()
    {
        var result = _dataSource.Customers;
        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/Customers({key})")]
    public IActionResult GetCustomer([FromODataUri] int key)
    {
        var result = _dataSource.Customers?.SingleOrDefault(a => a.CustomerId == key);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/Customers({key})/Orders")]
    public IActionResult GetCustomerOrders([FromODataUri] int key)
    {
        var result = _dataSource.Customers?.SingleOrDefault(a => a.CustomerId == key);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result?.Orders);
    }

    [EnableQuery]
    [HttpGet("odata/Computers")]
    public IActionResult GetComputers()
    {
        var result = _dataSource.Computers;
        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/Computers({key})")]
    public IActionResult GetComputer([FromODataUri] int key)
    {
        var result = _dataSource.Computers?.SingleOrDefault(a => a.ComputerId == key);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/Computers/$count")]
    public IActionResult GetComputersCount()
    {
        var result = _dataSource.Computers;
        return Ok(result?.Count);
    }

    [EnableQuery]
    [HttpGet("odata/Computers({key})/ComputerDetail/Manufacturer")]
    public IActionResult GetComputerManufacturer([FromODataUri] int key)
    {
        var result = _dataSource.Computers?.SingleOrDefault(a => a.ComputerId == key);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result?.ComputerDetail?.Manufacturer);
    }

    [HttpPost("odata/querycounttests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = CommonEndToEndDataSource.CreateInstance();

        return Ok();
    }
}

