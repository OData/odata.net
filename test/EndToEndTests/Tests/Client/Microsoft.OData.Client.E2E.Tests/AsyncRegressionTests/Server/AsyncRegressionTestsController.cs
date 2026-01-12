//-----------------------------------------------------------------------------
// <copyright file="AsyncRegressionTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Microsoft.OData.Client.E2E.Tests.AsyncRegressionTests.Server;

public class AsyncRegressionTestsController : ODataController
{
    private static AsyncRegressionTestsDataSource DataSource = null!;

    private static readonly byte[] smileyPng = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAMAAAAoLQ9TAAAARVBMVEUAAAD///8AAADMzMwAAAB/f38zMzOZmZmqqqq3t7fLy8vExMTr6+vV1dXU1NTS0tK8vLzAwMDx8fG3t7eQkJDY2Nj0P+0oAAAAEHRSTlMAEBAgIDAwQEBQUFBgYGCQYv6kAAAAVElEQVQY02NgQAXGAAEEBkYmJgYGRkYGJiYmBgYGBgYGRgYGBkYGJgYGBgYGRkYGJiYmBgYGBgYGJiYmBgYGAAA7OQHGpJx+KwAAAABJRU5ErkJggg==");

    private static readonly List<MediaAsset> mediaAssets = new List<MediaAsset>
    {
        new MediaAsset { Id = 1 }
    };

    [EnableQuery]
    [HttpGet("Customers")]
    public ActionResult<IEnumerable<Customer>> GetCustomers()
    {
        return DataSource.Customers;
    }

    [EnableQuery]
    [HttpGet("Customers({key})")]
    public ActionResult<Customer> GetCustomer([FromODataUri] int key)
    {
        var customer = DataSource.Customers.FirstOrDefault(c => c.Id == key);

        if (customer == null)
        {
            return NotFound();
        }

        return customer;
    }

    [EnableQuery(PageSize = 2)]
    [HttpGet("Customers({key})/Orders")]
    public ActionResult<IEnumerable<Order>> GetOrders([FromODataUri] int key)
    {
        var customer = DataSource.Customers.FirstOrDefault(c => c.Id == key);
        if (customer == null)
        {
            return NotFound();
        }

        return customer.Orders;
    }

    [HttpGet("Customers({key})/Photo")]
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

    [HttpPost("Customers")]
    public ActionResult Post([FromBody] Customer customer)
    {
        DataSource.Customers.Add(customer);

        return Created(customer);
    }

    [HttpPatch("Customers")]
    public ActionResult Patch([FromBody] DeltaSet<Customer> deltaSet)
    {
        return Ok(deltaSet);
    }

    [HttpGet("Customers/Default.GetTopCustomer()")]
    public ActionResult<Customer> GetTopCustomer()
    {
        var customerId = Random.Shared.Next(1, DataSource.Customers.Count + 1);
        var topCustomer = DataSource.Customers.Single(d => d.Id == customerId);

        return topCustomer;
    }

    [EnableQuery(PageSize = 3)]
    [HttpGet("Orders")]
    public ActionResult<IEnumerable<Order>> GetOrders()
    {
        return DataSource.Orders;
    }

    [EnableQuery]
    [HttpGet("Orders({key})")]
    public ActionResult<Order> GetOrder([FromODataUri] int key)
    {
        var order = DataSource.Orders.FirstOrDefault(c => c.Id == key);

        if (order == null)
        {
            return NotFound();
        }

        return order;
    }

    [HttpGet("Orders/GetTop2Orders()")]
    public ActionResult<IEnumerable<Order>> GetTop2Orders()
    {
        var top2Orders = DataSource.Orders.OrderByDescending(d => d.Amount).Take(2).ToList();

        return top2Orders;
    }

    [HttpGet("Orders({key})/Customer")]
    public ActionResult<Customer> GetOrderCustomer(int key)
    {
        var order = DataSource.Orders.SingleOrDefault(c => c.Id == key);

        if (order == null)
        {
            return NotFound();
        }

        return order.Customer;
    }

    [HttpPost("Orders({key})/Default.ApplyDiscount")]
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

    [EnableQuery]
    [HttpGet("Media")]
    public ActionResult<IEnumerable<MediaAsset>> GetMedia()
    {
        return mediaAssets;
    }

    [EnableQuery]
    [HttpGet("Media({key})")]
    public ActionResult<MediaAsset> Get([FromODataUri] int key)
    {
        var mediaAsset = mediaAssets.FirstOrDefault(d => d.Id == key);
        if (mediaAsset == null)
        {
            return NotFound();
        }

        return mediaAsset;
    }

    [HttpGet("Media({key})/$value")]
    public ActionResult GetMediaStream(int key)
    {
        var mediaAsset = mediaAssets.FirstOrDefault(d => d.Id == key);
        if (mediaAsset == null)
        {
            return NotFound();
        }

        byte[] mediaContent = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        // Return the byte array as a FileContentResult
        // application/octet-stream is the default content type for binary data
        return File(mediaContent, "application/octet-stream", $"media_{key}.bin");
    }

    [HttpPost("Default.ResetDataSource")]
    public IActionResult ResetDataSource()
    {
        DataSource = AsyncRegressionTestsDataSource.CreateInstance();

        return Ok();
    }
}
