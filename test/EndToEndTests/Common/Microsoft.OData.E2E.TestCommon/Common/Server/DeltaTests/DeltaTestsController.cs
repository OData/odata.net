//-----------------------------------------------------------------------------
// <copyright file="DeltaTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.DeltaTests;

public class DeltaTestsController : ODataController
{
    private static DefaultDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/Customers({key})")]
    public IActionResult GetCustomer([FromRoute] int key)
    {
        var customer = _dataSource.Customers?.FirstOrDefault(e => e.PersonID == key);
        if (customer == null)
        {
            return NotFound();
        }
        return Ok(customer);
    }

    [EnableQuery]
    [HttpGet("odata/Customers({customerKey})/Orders({orderKey})")]
    public IActionResult GetOrder([FromRoute] int customerKey, [FromRoute] int orderKey)
    {
        var order = _dataSource.Customers?.FirstOrDefault(e => e.PersonID == customerKey)?.Orders?.FirstOrDefault(o => o.OrderID == orderKey);
        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPatch("odata/Customers({key})")]
    public IActionResult Patch([FromRoute] int key, [FromBody] Delta<Customer> delta)
    {
        var customer = _dataSource.Customers?.FirstOrDefault(e => e.PersonID == key);
        if (customer == null)
        {
            return NotFound();
        }

        if (delta.TryGetPropertyValue("Orders", out var orders))
        {
            var orderDeltaSet = orders as DeltaSet<Order>;
            if (orderDeltaSet != null)
            {
                foreach (Delta<Order> deltaUnit in orderDeltaSet)
                {
                    if (deltaUnit.Kind == DeltaItemKind.Resource)
                    {
                        var id = deltaUnit.GetInstance().OrderID;
                        var order = customer.Orders.FirstOrDefault(o => o.OrderID == id);
                        if (order != null)
                        {
                            deltaUnit.Patch(order);
                        }
                        else
                        {
                            customer.Orders.Add(deltaUnit.GetInstance());
                        }
                    }
                    else if (deltaUnit.Kind == DeltaItemKind.DeletedResource)
                    {
                        var id = deltaUnit.GetInstance().OrderID;
                        var order = customer.Orders.FirstOrDefault(o => o.OrderID == id);
                        if (order != null)
                        {
                            customer.Orders.Remove(order);
                        }
                    }
                }
            }
        }

        return Ok();
    }

    [HttpPost("odata/deltatests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = DefaultDataSource.CreateInstance();

        return Ok();
    }
}
