//---------------------------------------------------------------------
// <copyright file="AsynchronousQueryTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.Asynchronous;

public class AsynchronousQueryTestsController : ODataController
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

    [EnableQuery]
    [HttpGet("odata/People/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee")]
    public IActionResult GetPeopleOfTypeEmployee()
    {
        var employees = _dataSource.People?.OfType<Employee>().ToList();
        return Ok(employees);
    }

    [EnableQuery]
    [HttpGet("odata/Computers")]
    public IActionResult GetComputers()
    {
        return Ok(_dataSource.Computers);
    }

    [EnableQuery]
    [HttpGet("odata/Drivers")]
    public IActionResult GetDrivers()
    {
        return Ok(_dataSource.Drivers);
    }

    [EnableQuery]
    [HttpGet("odata/ComputerDetails")]
    public IActionResult GetComputerDetails()
    {
        return Ok(_dataSource.ComputerDetails);
    }

    [HttpPost("odata/asynchronousquerytests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = CommonEndToEndDataSource.CreateInstance();
        return Ok();
    }
}
