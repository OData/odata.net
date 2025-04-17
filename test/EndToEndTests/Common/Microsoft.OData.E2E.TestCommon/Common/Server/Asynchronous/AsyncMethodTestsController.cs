//---------------------------------------------------------------------
// <copyright file="AsyncMethodTestsController.cs" company="Microsoft">
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

public class AsyncMethodTestsController : ODataController
{
    private static CommonEndToEndDataSource _dataSource;

    [EnableQuery(MaxAnyAllExpressionDepth = 3, PageSize = 2)]
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

    [EnableQuery(PageSize = 2)]
    [HttpGet("odata/Customers/{key}/Orders")]
    public IActionResult GetCustomerOrders2([FromODataUri] int key)
    {
        var customer = _dataSource.Customers?.SingleOrDefault(c => c.CustomerId == key);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer.Orders);
    }

    [EnableQuery(MaxAnyAllExpressionDepth = 3)]
    [HttpGet("odata/People")]
    public IActionResult GetPeople()
    {
        return Ok(_dataSource.People);
    }

    [EnableQuery]
    [HttpGet("odata/People/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee")]
    public IActionResult GetPeopleOfTypeEmployee()
    {
        var employees = _dataSource.People?.OfType<Employee>().ToList();
        return Ok(employees);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.SpecialEmployee/Car")]
    public IActionResult GetSpecialEmployeeCar([FromODataUri] int key)
    {
        var employee = _dataSource.People?.OfType<SpecialEmployee>().SingleOrDefault(s => s.PersonId == key);
        if (employee == null)
        {
            return NotFound();
        }

        return Ok(employee.Car);
    }

    [EnableQuery]
    [HttpGet("odata/Drivers")]
    public IActionResult GetDrivers()
    {
        return Ok(_dataSource.Drivers);
    }

    [HttpPost("odata/People")]
    public IActionResult AddPerson([FromBody] Person person)
    {
        _dataSource.People?.Add(person);
        return Created(person);
    }

    [HttpPost("odata/Customers")]
    public IActionResult AddCustomer([FromBody] Customer customer)
    {
        _dataSource.Customers?.Add(customer);
        return Created(customer);
    }

    [HttpPost("odata/Orders")]
    public IActionResult AddOrder([FromBody] Order order)
    {
        _dataSource.Orders?.Add(order);
        return Created(order);
    }

    [HttpPost("odata/Customers({key})/Orders/$ref")]
    public IActionResult AddOrderRefToCustomer([FromODataUri] int key, [FromBody] Uri orderUri)
    {
        if (orderUri == null)
        {
            return BadRequest();
        }

        // Extract the ID from the URI
        var lastSegment = orderUri.Segments.Last();
        int startIndex = lastSegment.IndexOf('(') + 1;
        int endIndex = lastSegment.IndexOf(')') - 1;
        var orderId = int.Parse(Uri.UnescapeDataString(lastSegment.Substring(startIndex, endIndex - startIndex + 1)));

        // Find the order by ID
        var order = _dataSource.Orders?.SingleOrDefault(d => d.OrderId == orderId);
        if (order == null)
        {
            return NotFound();
        }

        var customer = _dataSource.Customers?.SingleOrDefault(c => c.CustomerId == key);
        if (customer == null)
        {
            return NotFound();
        }

        customer.Orders ??= new List<Order>();
        customer.Orders.Add(order);

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
        return Updated(updated);
    }

    [HttpPatch("odata/Orders({key})")]
    public IActionResult PatchOrder([FromODataUri] int key, [FromBody] Delta<Order> delta)
    {
        var order = _dataSource.Orders?.FirstOrDefault(c => c.OrderId == key);
        if (order == null)
        {
            return NotFound();
        }

        var updated = delta.Patch(order);
        return Updated(updated);
    }

    [HttpPatch("odata/People({key})")]
    public IActionResult PatchPerson([FromODataUri] int key, [FromBody] Delta<Person> delta)
    {
        var person = _dataSource.People?.FirstOrDefault(c => c.PersonId == key);
        if (person == null)
        {
            return NotFound();
        }

        var updated = delta.Patch(person);
        return Updated(updated);
    }

    [HttpDelete("odata/People({key})")]
    public IActionResult DeletePeople([FromODataUri] int key)
    {
        var person = _dataSource.People?.FirstOrDefault(c => c.PersonId == key);
        if (person == null)
        {
            return NotFound();
        }

        _dataSource.People?.Remove(person);
        return NoContent();
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

    #region Functions and Actions

    [HttpGet("odata/GetCustomerCount")]
    public IActionResult GetCustomerCount()
    {
        return Ok(_dataSource.Customers?.Count);
    }

    [HttpPost("odata/People/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee/Default.IncreaseSalaries")]
    public IActionResult IncreaseSalaries(ODataActionParameters parameters)
    {
        var employees = _dataSource.People?.OfType<Employee>();

        foreach (var employee in employees)
        {
            employee.Salary += (int)parameters["n"];
        }

        return Ok();
    }

    #endregion

    [HttpPost("odata/asyncmethodtests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = CommonEndToEndDataSource.CreateInstance();
        return Ok();
    }
}
