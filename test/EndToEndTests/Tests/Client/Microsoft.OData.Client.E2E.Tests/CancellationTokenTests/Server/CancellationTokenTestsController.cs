//-----------------------------------------------------------------------------
// <copyright file="CancellationTokenTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;
using System.Text.RegularExpressions;

namespace Microsoft.OData.Client.E2E.Tests.CancellationTokenTests.Server;

public class CancellationTokenTestsController : ODataController
{
    private static CommonEndToEndDataSource _dataSource;

    [EnableQuery(PageSize = 5)]
    [HttpGet("odata/Customers")]
    public IActionResult GetCustomers(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Ok(_dataSource.Customers);
    }

    [EnableQuery]
    [HttpGet("odata/Customers({key})")]
    public IActionResult GetCustomer([FromODataUri] int key, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var customer = _dataSource.Customers?.FirstOrDefault(c => c.CustomerId == key);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPost("odata/Customers")]
    public IActionResult AddCustomer([FromBody] Customer customer, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _dataSource.Customers?.Add(customer);
        return Created(customer);
    }

    [HttpPost("odata/Customers({key})/Default.ChangeCustomerAuditInfo")]
    public IActionResult ChangeCustomerAuditInfo([FromODataUri] int key, [FromBody] AuditInfo auditInfo, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var customer = _dataSource.Customers?.FirstOrDefault(c => c.CustomerId == key);
        if (customer == null)
        {
            return NotFound();
        }

        customer.Auditing = auditInfo;
        return Created(auditInfo);
    }

    [HttpPost("odata/Customers({key})/Orders/$ref")]
    public IActionResult AddOrderRefToCustomer([FromODataUri] int key, [FromBody] Uri orderUri, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (orderUri == null)
        {
            return BadRequest();
        }

        // Extract the order ID from the URI
        var lastSegment = orderUri.Segments.Last();
        var orderId = int.Parse(Regex.Match(lastSegment, @"\d+").Value);

        // Find the order by ID
        var order = _dataSource.Orders?.SingleOrDefault(d => d.OrderId == orderId);
        if (order == null)
        {
            return NotFound();
        }

        // Add the order reference to the customer
        var customer = _dataSource.Customers?.SingleOrDefault(c => c.CustomerId == key);
        if (customer == null)
        {
            return NotFound();
        }

        customer.Orders ??= [];
        customer.Orders.Add(order);

        return Ok(customer);
    }

    [HttpPost("odata/Orders")]
    public IActionResult AddOrder([FromBody] Order order, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _dataSource.Orders?.Add(order);
        return Created(order);
    }

    [HttpPost("odata/Cars")]
    public async Task<IActionResult> AddCar([FromBody] Car car, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _dataSource.Cars?.Add(car);
        return Created(car);
    }

    [HttpPut("odata/Cars({key})/Photo")]
    public IActionResult UpdateCarPhoto([FromODataUri] int key, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var car = _dataSource.Cars?.FirstOrDefault(c => c.VIN == key);
        if (car == null)
        {
            return NotFound();
        }

        var photo = Request.Body;
        car.Photo = photo;
        return Ok();
    }

    [HttpPost("odata/cancellationtokentests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = CommonEndToEndDataSource.CreateInstance();

        return Ok();
    }
}
