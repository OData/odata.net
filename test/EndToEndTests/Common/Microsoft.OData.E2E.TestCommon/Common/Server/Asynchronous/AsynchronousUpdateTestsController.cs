//---------------------------------------------------------------------
// <copyright file="AsynchronousUpdateTestsController.cs" company="Microsoft">
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

public class AsynchronousUpdateTestsController : ODataController
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
    [HttpGet("odata/Orders({key})/Customer")]
    public IActionResult GetOrderCustomer([FromODataUri] int key)
    {
        var order = _dataSource.Orders?.SingleOrDefault(c => c.OrderId == key);
        if (order == null)
        {
            return NotFound();
        }

        return Ok(order.Customer);
    }

    [EnableQuery(MaxAnyAllExpressionDepth = 3)]
    [HttpGet("odata/People")]
    public IActionResult GetPeople()
    {
        return Ok(_dataSource.People);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee/Title")]
    public IActionResult GetPersonTitle([FromODataUri]int key)
    {
        var employee = _dataSource.People?.OfType<Employee>().FirstOrDefault(c => c.PersonId == key);
        if (employee == null)
        {
            return NotFound();
        }

        return Ok(employee.Title);
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

    [HttpPut("odata/Orders({key})/Customer/$ref")]
    public IActionResult UpdateCompanyVipCustomerRef([FromODataUri] int key, [FromBody] Uri customerUri)
    {
        var order = _dataSource.Orders?.FirstOrDefault(c => c.OrderId == key);
        if (order == null)
        {
            return NotFound();
        }

        // Extract the ID from the URI
        var lastSegment = customerUri.Segments.Last();
        int startIndex = lastSegment.IndexOf('(') + 1;
        int endIndex = lastSegment.IndexOf(')') - 1;
        var customerId = int.Parse(Uri.UnescapeDataString(lastSegment.Substring(startIndex, endIndex - startIndex + 1)));

        // Find the vipCustomer by ID
        var customer = _dataSource.Customers?.SingleOrDefault(d => d.CustomerId == customerId);
        if (customer == null)
        {
            return NotFound();
        }

        order.Customer = customer;

        return NoContent();
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

    [HttpDelete("odata/Orders({key})/Customer/$ref")]
    public IActionResult RemoveCustomerRefFromOrder([FromODataUri] int key)
    {
        // Remove the customer reference from the order
        var order = _dataSource.Orders?.SingleOrDefault(c => c.OrderId == key);
        if (order == null)
        {
            return NotFound();
        }

        order.Customer = null;
        return Ok(order);
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

    #region Functions and Actions

    [HttpGet("odata/GetCustomerCount")]
    public IActionResult GetCustomerCount()
    {
        return Ok(_dataSource.Customers?.Count);
    }

    [HttpPost("odata/People({key})/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee/Sack")]
    public IActionResult Sack([FromODataUri] int key)
    {
        var employee = _dataSource.People?.OfType<Employee>().FirstOrDefault(c => c.PersonId == key);
        if (employee == null)
        {
            return NotFound();
        }

        employee.Salary = 0;
        employee.Title += "[Sacked]";

        return Ok();
    }

    #endregion

    [HttpPost("odata/asynchronousupdatetests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = CommonEndToEndDataSource.CreateInstance();
        return Ok();
    }
}
