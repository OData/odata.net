//---------------------------------------------------------------------
// <copyright file="OrdersController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.Wasm.Sample.Server.Data;
using Microsoft.OData.Client.Wasm.Sample.Server.Models;

namespace Microsoft.OData.Client.Wasm.Sample.Server.Controllers;

public class OrdersController : ODataController
{
    [EnableQuery(PageSize = 3)]
    public ActionResult<IEnumerable<Order>> Get()
    {
        return DataSource.Orders;
    }

    [EnableQuery]
    public ActionResult<Order> Get([FromODataUri] int key)
    {
        var order = DataSource.Orders.FirstOrDefault(c => c.Id == key);

        if (order == null)
        {
            return NotFound();
        }

        return order;
    }

    public ActionResult<Customer> GetCustomer(int key)
    {
        var order = DataSource.Orders.SingleOrDefault(c => c.Id == key);

        if (order == null)
        {
            return NotFound();
        }

        return order.Customer;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Order>> GetTop2Orders()
    {
        var top2Orders = DataSource.Orders.OrderByDescending(d => d.Amount).Take(2).ToList();

        return top2Orders;
    }

    [HttpPost]
    public ActionResult<decimal> ApplyDiscount([FromODataUri] int key, [FromBody] ODataActionParameters parameters)
    {
        var order = DataSource.Orders.SingleOrDefault(c => c.Id == key);
        if (order == null)
        {
            return NotFound();
        }

        if (parameters == null
            || !parameters.TryGetValue("discountPercentage", out var discountPercentageObj)
            || !decimal.TryParse(discountPercentageObj.ToString(), out decimal discountPercentage))
        {
            return BadRequest("Missing parameter: discountPercentage");
        }

        var discountAmount = order.Amount * discountPercentage / 100;
        order.Amount -= discountAmount;

        return order.Amount;
    }
}
