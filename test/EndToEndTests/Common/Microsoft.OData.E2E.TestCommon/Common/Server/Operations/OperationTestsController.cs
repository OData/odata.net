//-----------------------------------------------------------------------------
// <copyright file="OperationTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.Operations;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.OperationTests;

public class OperationTestsController : ODataController
{
    private static OperationsDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/Orders")]
    public ActionResult GetOrders()
    {
        var orders = _dataSource.Orders;
        return Ok(orders);
    }

    [EnableQuery]
    [HttpGet("odata/Orders({key})")]
    public ActionResult GetOrder([FromODataUri] int key)
    {
        var order = _dataSource.Orders?.SingleOrDefault(o => o.OrderID == key);
        return Ok(order);
    }

    [EnableQuery]
    [HttpGet("odata/Customers")]
    public ActionResult GetCustomers()
    {
        var customers = _dataSource.Customers;
        return Ok(customers);
    }

    [EnableQuery]
    [HttpGet("odata/Customers({key})")]
    public ActionResult GetCustomer([FromODataUri] int key)
    {
        var customer = _dataSource.Customers?.SingleOrDefault(c => c.CustomerID == key);
        return Ok(customer);
    }

    [EnableQuery]
    [HttpGet("odata/Customers/GetCustomersForAddresses(addresses={addresses})")]
    public IEnumerable<Customer>? GetCustomersForAddresses([FromODataUri] IEnumerable<Address> addresses)
    {
        var customers = _dataSource.Customers?.Where(c => c.Address != null && addresses.Contains(c.Address));
        return customers;
    }

    [EnableQuery]
    [HttpGet("odata/Customers/GetCustomerForAddress(address={address})")]
    public Customer? GetCustomerForAddress([FromODataUri] Address address)
    {
        var customer = _dataSource.Customers?.SingleOrDefault(c => c.Address != null && c.Address.Equals(address));
        return customer;
    }

    [EnableQuery]
    [HttpGet("odata/Customers({key})/GetOrdersFromCustomerByNotes(notes={notes})")]
    public IEnumerable<Order>? GetOrdersFromCustomerByNotes([FromODataUri] int key, [FromODataUri] IEnumerable<string> notes)
    {
        var customer = _dataSource.Customers?.SingleOrDefault(c => c.CustomerID == key);
        var orders = customer?.Orders.Where(order => order.Notes.Intersect(notes).Any());
        return orders;
    }

    [EnableQuery]
    [HttpGet("odata/Orders/GetOrdersByNote(note={note})")]
    public ActionResult GetOrdersByNote([FromODataUri] string note)
    {
        var orders = _dataSource.Orders?.Where(order => order.Notes.Contains(note));
        return Ok(orders);
    }

    [HttpGet("odata/Customers/GetCustomersByOrders(orders={orders})")]
    public IEnumerable<Customer>? GetCustomersByOrders([FromODataUri] IEnumerable<Order> orders)
    {
        var customers = _dataSource.Customers?.Where(customer => customer.Orders.Any(o => orders.Any(o2 => o2.OrderID == o.OrderID)));
        return customers;
    }

    [EnableQuery]
    [HttpGet("odata/Customers/GetCustomerByOrder(order={order})")]
    public Customer? GetCustomerByOrder([FromODataUri] Order order)
    {
        var customer = _dataSource.Customers?.SingleOrDefault(customer => customer.Orders.Any(o => o.OrderID == order.OrderID));
        return customer;
    }

    [EnableQuery]
    [HttpGet("odata/Orders/GetOrderByNote(notes={notes})")]
    public Order? GetOrderByNote([FromODataUri] IEnumerable<string> notes)
    {
        var order = _dataSource.Orders?.SingleOrDefault(order => order.Notes.Intersect(notes).Count() == order.Notes.Count
               && order.Notes.Count == notes.Count());
        return order;
    }

    [EnableQuery]
    [HttpGet("odata/Customers({key})/VerifyCustomerAddress(address={address})")]
    public Customer? VerifyCustomerAddress([FromODataUri] int key, [FromODataUri] Address address)
    {
        var customer = _dataSource.Customers?.SingleOrDefault(c => c.CustomerID == key);
        return address != null && address.Equals(customer?.Address) ? customer : null;
    }

    [EnableQuery]
    [HttpGet("odata/Customers({key})/VerifyCustomerByOrder(order={order})")]
    public Customer? VerifyCustomerByOrder([FromODataUri] int key, [FromODataUri] Order order)
    {
        var customer = _dataSource.Customers?.SingleOrDefault(c => c.CustomerID == key);
        return customer != null && customer.Orders.Any(o => o.OrderID == order.OrderID) ? customer : null;
    }

    [HttpPost("odata/operationtests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = OperationsDataSource.CreateInstance();

        return Ok();
    }
}
