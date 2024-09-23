//-----------------------------------------------------------------------------
// <copyright file="EdmDateAndTimeOfDayTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Client.E2E.Tests.EdmDateAndTimeOfDayTests.Server
{
    public class EdmDateAndTimeOfDayTestsController : ODataController
    {
        private static DefaultDataSource _dataSource;

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
            var order = _dataSource.Orders.SingleOrDefault(a => a.OrderID == key);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [EnableQuery]
        [HttpGet("odata/Orders({key})/ShipDate")]
        public IActionResult GetOrderShipDate([FromRoute] int key)
        {
            var order = _dataSource.Orders.SingleOrDefault(a => a.OrderID == key);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order.ShipDate);
        }


        [EnableQuery]
        [HttpGet("odata/Orders({key})/ShipDate/$value")]
        public IActionResult GetOrderShipDateRawValue([FromRoute] int key)
        {
            var order = _dataSource.Orders.SingleOrDefault(a => a.OrderID == key);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order.ShipDate);
        }

        [EnableQuery]
        [HttpGet("odata/Orders({key})/ShipTime")]
        public IActionResult GetOrderShipTime([FromRoute] int key)
        {
            var order = _dataSource.Orders.SingleOrDefault(a => a.OrderID == key);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order.ShipTime);
        }

        [EnableQuery]
        [HttpGet("odata/Orders({key})/ShipTime/$value")]
        public IActionResult GetOrderShipTimeRawValue([FromRoute] int key)
        {
            var order = _dataSource.Orders.SingleOrDefault(a => a.OrderID == key);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order.ShipTime);
        }

        [EnableQuery]
        [HttpGet("odata/Orders({key})/Default.GetShipDate()")]
        public IActionResult GetShipDate([FromRoute] int key)
        {
            var order = _dataSource.Orders.SingleOrDefault(a => a.OrderID == key);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order.ShipDate);
        }

        [EnableQuery]
        [HttpGet("odata/Orders({key})/Default.CheckShipDate(date = {date})")]
        public IActionResult CheckShipDate([FromRoute] int key, Date date)
        {
            var order = _dataSource.Orders.SingleOrDefault(a => a.OrderID == key);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order.ShipDate == date);
        }

        [EnableQuery]
        [HttpGet("odata/Orders({key})/Default.GetShipTime")]
        public IActionResult GetShipTime([FromRoute] int key)
        {
            var order = _dataSource.Orders.SingleOrDefault(a => a.OrderID == key);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order.ShipTime);
        }


        [EnableQuery]
        [HttpGet("odata/Orders({key})/Default.CheckShipTime(time = {time})")]
        public IActionResult CheckShipTime([FromRoute] int key, TimeOfDay time)
        {
            var order = _dataSource.Orders.SingleOrDefault(a => a.OrderID == key);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order.ShipTime == time);
        }

        [EnableQuery]
        [HttpGet("odata/Calendars({key})")]
        public IActionResult GetCalendar([FromRoute] Date key)
        {
            var calendar = _dataSource.Calendars.SingleOrDefault(a => a.Day == key);

            if (calendar == null)
            {
                return NotFound();
            }

            return Ok(calendar);
        }

        [HttpPost("odata/Orders({key})/Default.ChangeShipTimeAndDate")]
        public IActionResult ChangeShipTimeAndDate([FromRoute] int key, ODataActionParameters parameters)
        {
            var order = _dataSource.Orders.SingleOrDefault(a => a.OrderID == key);

            if (order == null)
            {
                return NotFound();
            }

            order.ShipTime = (TimeOfDay)parameters["time"];
            order.ShipDate = (Date)parameters["date"];

            return Ok(order);
        }


        [HttpPatch("odata/Orders({key})")]
        public IActionResult PatchOrder([FromRoute] int key, [FromBody] Delta<Order> delta)
        {
            var order = _dataSource.Orders.SingleOrDefault(a => a.OrderID == key);

            if (order == null)
            {
                return NotFound();
            }

            var updatedOrder = delta.Patch(order);

            return Ok(updatedOrder);
        }

        [HttpPost("odata/edmdateandtimeofday/Default.ResetDefaultDataSource")]
        public IActionResult ResetDefaultDataSource()
        {
            _dataSource = DefaultDataSource.CreateInstance();

            return Ok();
        }
    }
}
