﻿//---------------------------------------------------------------------
// <copyright file="UpdateTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Server;

public class UpdateTestsController : ODataController
{
    private static CommonEndToEndDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/People")]
    public IActionResult GetPeople()
    {
        var people = _dataSource.People;
        return Ok(people);
    }

    [EnableQuery]
    [HttpGet("odata/People/$/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee")]
    public IActionResult GetPeopleOfTypeEmployee()
    {
        var people = _dataSource.People?.OfType<Employee>();
        return Ok(people);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})")]
    public IActionResult GetPerson([FromRoute] int key)
    {
        var person = _dataSource.People?.SingleOrDefault(a => a.PersonId == key);
        if (person == null)
        {
            return NotFound();
        }
        return Ok(person);
    }

    [EnableQuery]
    [HttpGet("odata/People/{key}")]
    public IActionResult GetPersonById([FromRoute] int key)
    {
        var person = _dataSource.People?.SingleOrDefault(a => a.PersonId == key);
        if (person == null)
        {
            return NotFound();
        }
        return Ok(person);
    }

    [EnableQuery]
    [HttpGet("odata/Orders")]
    public IActionResult GetOrders()
    {
        var orders = _dataSource.Orders;
        return Ok(orders);
    }

    [EnableQuery]
    [HttpGet("odata/Customers")]
    public IActionResult GetCustomers()
    {
        var customers = _dataSource.Customers;
        return Ok(customers);
    }

    [EnableQuery]
    [HttpGet("odata/Customers/{key}")]
    public IActionResult GetCustomer([FromODataUri] int key)
    {
        var customer = _dataSource.Customers?.SingleOrDefault(c => c.CustomerId == key);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
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

    [HttpPost("odata/Customers({key})/Orders")]
    public IActionResult AddOrderToCustomer([FromODataUri] int key, [FromBody] Order order)
    {
        var customer = _dataSource.Customers?.SingleOrDefault(c => c.CustomerId == key);
        if (customer == null)
        {
            return NotFound();
        }

        customer.Orders?.Add(order);
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

        // Add the order reference to the customer
        var customer = _dataSource.Customers?.SingleOrDefault(c => c.CustomerId == key);
        if (customer == null)
        {
            return NotFound();
        }

        customer.Orders?.Add(order);

        return Ok(customer);
    }

    [HttpPut("odata/People({key})/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.SpecialEmployee/Manager/$ref")]
    public IActionResult UpdateEmployeeManager([FromODataUri] int key, [FromBody] Uri managerUri)
    {
        if (managerUri == null)
        {
            return BadRequest();
        }

        // Extract the ID from the URI
        var lastSegment = managerUri.Segments.Last();
        int startIndex = lastSegment.IndexOf('(') + 1;
        int endIndex = lastSegment.IndexOf(')') - 1;
        var managerId = int.Parse(Uri.UnescapeDataString(lastSegment.Substring(startIndex, endIndex - startIndex + 1)));

        // Find the order by ID
        var manager = _dataSource.People?.OfType<Employee>().SingleOrDefault(d => d.PersonId == managerId);
        if (manager == null)
        {
            return NotFound();
        }

        var employee = _dataSource.People?.OfType<SpecialEmployee>().SingleOrDefault(c => c.PersonId == key);
        if (employee == null)
        {
            return NotFound();
        }

        // Update the employee manager
        employee.Manager = manager;

        return Ok(employee);
    }

    [HttpPatch("odata/People/{key}")]
    public IActionResult PatchPerson([FromRoute] int key, [FromBody] Delta<Person> patch)
    {
        var person = _dataSource.People?.SingleOrDefault(a => a.PersonId == key);
        if (person == null)
        {
            return NotFound();
        }
        patch.Patch(person);
        return Updated(person);
    }

    [HttpDelete("odata/People({key})/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.SpecialEmployee")]
    public IActionResult DeleteSpecialEmployee([FromRoute] int key)
    {
        var person = _dataSource.People?.SingleOrDefault(a => a.PersonId == key);
        if (person == null)
        {
            return NotFound();
        }

        _dataSource.People?.Remove(person);
        return NoContent();
    }

    [HttpDelete("odata/Customers({key})/Orders/$ref")]
    public IActionResult RemoveOrderRefFromCustomer([FromODataUri] int key)
    {
        string? id = Request.Query["$id"];
        var orderUri = id != null ? new Uri(id) : null;

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

        // Remove the order reference from the customer
        var customer = _dataSource.Customers?.SingleOrDefault(c => c.CustomerId == key);
        if (customer == null)
        {
            return NotFound();
        }

        customer.Orders?.Remove(order);
        return Ok(customer);
    }

    [HttpPost("odata/keyassegmentupdatetests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = CommonEndToEndDataSource.CreateInstance();

        return Ok();
    }
}
