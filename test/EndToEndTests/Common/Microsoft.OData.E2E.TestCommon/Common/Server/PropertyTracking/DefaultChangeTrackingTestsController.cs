//-----------------------------------------------------------------------------
// <copyright file="DefaultChangeTrackingTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.PropertyTrackingTests;

public class DefaultChangeTrackingTestsController : ODataController
{
    private static DefaultDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/People")]
    public IActionResult GetPeople()
    {
        var people = _dataSource.People;
        return Ok(people);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})")]
    public IActionResult GetPerson([FromRoute] int key)
    {
        var person = _dataSource.People?.SingleOrDefault(a => a.PersonID == key);
        if (person == null)
        {
            return NotFound();
        }

        return Ok(person);
    }

    [EnableQuery]
    [HttpGet("odata/Boss")]
    public IActionResult GetBoss()
    {
        var boss = _dataSource.Boss;
        return Ok(boss);
    }

    [EnableQuery]
    [HttpGet("odata/Accounts")]
    public IActionResult GetAccounts()
    {
        var accounts = _dataSource.Accounts;
        return Ok(accounts);
    }

    [EnableQuery]
    [HttpGet("odata/Orders")]
    public IActionResult GetOrders()
    {
        var orders = _dataSource.Orders;
        return Ok(orders);
    }

    [EnableQuery]
    [HttpGet("odata/Orders({key})")]
    public IActionResult GetOrder([FromRoute] int key)
    {
        var order = _dataSource.Orders?.SingleOrDefault(a => a.OrderID == key);
        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [EnableQuery]
    [HttpGet("odata/OrderDetails")]
    public IActionResult GetOrderDetails()
    {
        var orderDetails = _dataSource.OrderDetails;
        return Ok(orderDetails);
    }

    [EnableQuery]
    [HttpGet("odata/Accounts({key})/MyGiftCard")]
    public IActionResult GetAccountMyGiftCard([FromRoute] int key)
    {
        var account = _dataSource.Accounts?.SingleOrDefault(a => a.AccountID == key);
        if (account == null)
        {
            return NotFound();
        }

        return Ok(account.MyGiftCard);
    }

    [HttpPost("odata/People")]
    public IActionResult AddPerson([FromBody] Person person)
    {
        _dataSource.People?.Add(person);
        return Created(person);
    }

    [HttpPost("odata/Orders")]
    public IActionResult AddOrder([FromBody] Order order)
    {
        _dataSource.Orders?.Add(order);
        return Created(order);
    }

    [HttpPost("odata/Accounts")]
    public IActionResult AddAccount([FromBody] Account account)
    {
        _dataSource.Accounts?.Add(account);
        return Created(account);
    }

    [HttpPut("odata/Orders({key})")]
    public IActionResult UpdateOrder([FromRoute] int key, [FromBody] Order order)
    {
        var orderToUpdate = _dataSource.Orders?.SingleOrDefault(a => a.OrderID == key);
        if (orderToUpdate == null)
        {
            return NotFound();
        }

        orderToUpdate.LoggedInEmployee = order.LoggedInEmployee ?? orderToUpdate.LoggedInEmployee;
        orderToUpdate.CustomerForOrder = order.CustomerForOrder ?? orderToUpdate.CustomerForOrder;
        orderToUpdate.ShipDate = order.ShipDate;
        orderToUpdate.ShipTime = order.ShipTime;
        orderToUpdate.InfoFromCustomer = order.InfoFromCustomer ?? orderToUpdate.InfoFromCustomer;
        orderToUpdate.OrderDetails = order.OrderDetails ?? orderToUpdate.OrderDetails;
        orderToUpdate.ShelfLife = order.ShelfLife ?? orderToUpdate.ShelfLife;
        orderToUpdate.OrderDate = order.OrderDate;
        orderToUpdate.OrderShelfLifes = order.OrderShelfLifes ?? orderToUpdate.OrderShelfLifes;
        orderToUpdate.UpdatedTime = DateTime.UtcNow;

        return Updated(orderToUpdate);
    }

    [HttpPatch("odata/OrderDetails(OrderID={orderID},ProductID={productID})")]
    public IActionResult PatchOrderDetail([FromODataUri] int orderID, [FromODataUri] int productID, [FromBody] Delta<OrderDetail> delta)
    {
        var orderDetail = _dataSource.OrderDetails?.SingleOrDefault(a => a.OrderID == orderID && a.ProductID == productID);
        if (orderDetail == null)
        {
            return NotFound();
        }

        var updatedOrderDetail = delta.Patch(orderDetail);
        return Updated(updatedOrderDetail);
    }

    [HttpPatch("odata/Orders({key})")]
    public IActionResult PatchOrder([FromRoute] int key, [FromBody] Delta<Order> delta)
    {
        var order = _dataSource.Orders?.SingleOrDefault(a => a.OrderID == key);
        if (order == null)
        {
            return NotFound();
        }

        var updatedOrder = delta.Patch(order);
        return Updated(updatedOrder);
    }

    [HttpPatch("odata/Boss/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer")]
    public IActionResult PatchBossCustomer([FromBody] Delta<Customer> delta)
    {
        var boss = _dataSource.Boss as Customer;
        if (boss == null)
        {
            return NotFound();
        }

        var updatedBoss = delta.Patch(boss);
        return Updated(updatedBoss);
    }

    [HttpPatch("odata/Accounts({key})/MyGiftCard")]
    public IActionResult PatchAccountGiftCard([FromRoute] int key, [FromBody] Delta<GiftCard> delta)
    {
        var account = _dataSource.Accounts?.SingleOrDefault(a => a.AccountID == key);
        if (account == null)
        {
            return NotFound();
        }

        if(account.MyGiftCard == null)
        {
            account.MyGiftCard = new GiftCard();
        }

        var updatedGiftCard = delta.Patch(account.MyGiftCard);
        return Updated(updatedGiftCard);
    }

    [HttpPatch("odata/People({key})/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer")]
    public IActionResult PatchPerson([FromRoute] int key, [FromBody] Delta<Customer> delta)
    {
        var person = _dataSource.People?.SingleOrDefault(a => a.PersonID == key) as Customer;
        if (person == null)
        {
            return NotFound();
        }

        var updatedCustomer = delta.Patch(person);
        return Updated(updatedCustomer);
    }

    [HttpPost("odata/defaultchangetrackingtests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = DefaultDataSource.CreateInstance();

        return Ok();
    }
}
