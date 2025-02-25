//---------------------------------------------------------------------
// <copyright file="PrimitiveTypesTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.PrimitiveTypes;

public class PrimitiveTypesTestsController : ODataController
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
        var customer = _dataSource.Customers?.SingleOrDefault(a => a.PersonID == key);

        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [EnableQuery]
    [HttpGet("odata/Customers({key})/TimeBetweenLastTwoOrders")]
    public IActionResult GetTimeBetweenLastTwoOrdersFromCustomer([FromRoute] int key)
    {
        var customer = _dataSource.Customers?.SingleOrDefault(a => a.PersonID == key);

        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer.TimeBetweenLastTwoOrders);
    }

    [EnableQuery]
    [HttpGet("odata/Customers({key})/TimeBetweenLastTwoOrders/$value")]
    public IActionResult GetTimeBetweenLastTwoOrdersValueFromCustomer([FromRoute] int key)
    {
        var customer = _dataSource.Customers?.SingleOrDefault(a => a.PersonID == key);

        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer.TimeBetweenLastTwoOrders);
    }

    [EnableQuery]
    [HttpGet("odata/Orders")]
    public IActionResult Orders()
    {
        var orders = _dataSource.Orders;

        return Ok(orders);
    }

    [EnableQuery]
    [HttpGet("odata/Orders()")]
    public IActionResult GetOrders()
    {
        var orders = _dataSource.Orders;

        return Ok(orders);
    }

    [EnableQuery]
    [HttpGet("odata/Orders/$count")]
    public IActionResult GetOrdersCount()
    {
        var count = _dataSource.Orders?.Count ?? 0;
        return Ok(count);
    }

    [EnableQuery]
    [HttpGet("odata/Orders({key})")]
    public IActionResult GetOrder([FromRoute] int key)
    {
        var order = _dataSource.Orders?.SingleOrDefault(o => o.OrderID == key);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [EnableQuery]
    [HttpGet("odata/Orders({key})/CustomerForOrder")]
    public IActionResult GetCustomerForOrderFromOrder([FromRoute] int key)
    {
        var order = _dataSource.Orders?.SingleOrDefault(o => o.OrderID == key);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order.CustomerForOrder);
    }

    [HttpPost("odata/Orders")]
    public IActionResult PostOrder([FromBody] Order order)
    {
        _dataSource.Orders.Add(order);

        return Created(order);
    }

    [HttpPut("odata/Orders({key})")]
    public IActionResult PutOrder([FromRoute] int key, [FromBody] Order order)
    {
        var existsOrder = _dataSource.Orders?.SingleOrDefault(o => o.OrderID == key);

        if (existsOrder == null)
        {
            return NotFound();
        }

        // Remove the current order from the data source
        _dataSource.Orders.Remove(existsOrder);

        existsOrder.ShelfLife = order.ShelfLife;
        existsOrder.OrderShelfLifes = order.OrderShelfLifes;

        // Add the updated order to the data source
        _dataSource.Orders.Add(existsOrder);

        return Ok(existsOrder);
    }

    [HttpPatch("odata/Orders({key})")]
    public IActionResult PatchOrder([FromRoute] int key, [FromBody] Delta<Order> delta)
    {
        var order = _dataSource.Orders?.SingleOrDefault(o => o.OrderID == key);

        if (order == null)
        {
            return NotFound();
        }

        var updatedOrder = delta.Patch(order);

        return Ok(updatedOrder);
    }

    [HttpDelete("odata/Orders({key})")]
    public IActionResult PatchOrder([FromRoute] int key)
    {
        var order = _dataSource.Orders?.SingleOrDefault(o => o.OrderID == key);

        if (order == null)
        {
            return NotFound();
        }

        _dataSource.Orders.Remove(order);

        return NoContent();
    }

    [HttpPost("odata/primitivetypes/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = DefaultDataSource.CreateInstance();

        return Ok();
    }
}
