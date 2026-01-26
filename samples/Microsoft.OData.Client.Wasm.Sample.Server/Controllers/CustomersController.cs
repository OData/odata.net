//---------------------------------------------------------------------
// <copyright file="CustomersController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.Wasm.Sample.Server.Data;
using Microsoft.OData.Client.Wasm.Sample.Server.Models;

namespace Microsoft.OData.Client.Wasm.Sample.Server.Controllers;

public class CustomersController : ODataController
{
    private static readonly byte[] smileyPng = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAMAAAAoLQ9TAAAARVBMVEUAAAD///8AAADMzMwAAAB/f38zMzOZmZmqqqq3t7fLy8vExMTr6+vV1dXU1NTS0tK8vLzAwMDx8fG3t7eQkJDY2Nj0P+0oAAAAEHRSTlMAEBAgIDAwQEBQUFBgYGCQYv6kAAAAVElEQVQY02NgQAXGAAEEBkYmJgYGRkYGJiYmBgYGBgYGRgYGBkYGJgYGBgYGRkYGJiYmBgYGBgYGJiYmBgYGAAA7OQHGpJx+KwAAAABJRU5ErkJggg==");
    
    [EnableQuery]
    public ActionResult<IEnumerable<Customer>> Get()
    {
        return DataSource.Customers;
    }

    [EnableQuery]
    public ActionResult<Customer> Get([FromODataUri] int key)
    {
        var customer = DataSource.Customers.FirstOrDefault(c => c.Id == key);

        if (customer == null)
        {
            return NotFound();
        }

        return customer;
    }

    [EnableQuery(PageSize = 2)]
    public ActionResult<IEnumerable<Order>> GetOrders([FromODataUri] int key)
    {
        var customer = DataSource.Customers.FirstOrDefault(c => c.Id == key);
        if (customer == null)
        {
            return NotFound();
        }

        return customer.Orders;
    }

    public ActionResult GetPhoto(int key)
    {
        var customer = DataSource.Customers.FirstOrDefault(d => d.Id == key);
        if (customer == null)
        {
            return NotFound();
        }

        Response.Headers.ContentDisposition = "inline; filename=customer_" + key + "_photo.png";
        return File(smileyPng, "image/png");
    }

    public ActionResult Post([FromBody] Customer customer)
    {
        DataSource.Customers.Add(customer);

        return Created(customer);
    }

    public ActionResult Patch([FromBody] DeltaSet<Customer> deltaSet)
    {
        return Ok(deltaSet);
    }

    [HttpGet]
    public ActionResult<Customer> GetTopCustomer()
    {
        var idx = Random.Shared.Next(0, DataSource.Customers.Count);
        var topCustomer = DataSource.Customers[idx];

        return topCustomer;
    }
}
