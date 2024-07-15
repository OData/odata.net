//-----------------------------------------------------------------------------
// <copyright file="ClientQueryTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.ClientTests.Server
{
    public class ClientQueryTestsController : ODataController
    {
        [EnableQuery]
        [HttpGet("odata/People")]
        public IActionResult Get()
        {
            var people = CommonEndToEndDataSource.People;
            return Ok(people);
        }

        [EnableQuery]
        [HttpGet("odata/People({key})")]
        public IActionResult Get(int key)
        {
            var person = CommonEndToEndDataSource.People?.FirstOrDefault(a => a.PersonId == key);
            return Ok(person);
        }

        [EnableQuery]
        [HttpGet("odata/Logins")]
        public IActionResult GetLogins()
        {
            var logins = CommonEndToEndDataSource.Logins;
            return Ok(logins);
        }

        [EnableQuery]
        [HttpGet("odata/Logins({key})")]
        public IActionResult GetLogin(string key)
        {
            var person = CommonEndToEndDataSource.Logins?.FirstOrDefault(a => a.Username == key);
            return Ok(person);
        }

        [EnableQuery]
        [HttpGet("odata/Logins({key})/SentMessages(FromUsername={FromUsername},MessageId={MessageId})")]
        public IActionResult GetLoginSentMessages([FromODataUri]string key, [FromODataUri] string FromUsername, [FromODataUri] int MessageId)
        {
            var sentMessage = CommonEndToEndDataSource.Logins?.SingleOrDefault(a => a.Username == key).SentMessages.SingleOrDefault(a=>a.FromUsername == FromUsername && a.MessageId == MessageId);
            return Ok(sentMessage);
        }

        [EnableQuery]
        [HttpGet("odata/Cars")]
        public IActionResult GetCars()
        {
            var cars = CommonEndToEndDataSource.Cars;
            return Ok(cars);
        }

        [EnableQuery]
        [HttpGet("odata/Cars({key})")]
        public IActionResult GetCar(int key)
        {
            var car = CommonEndToEndDataSource.Cars?.FirstOrDefault(a => a.VIN == key);
            return Ok(car);
        }

        [EnableQuery]
        [HttpGet("odata/Customers")]
        public IActionResult GetCustomers()
        {
            var customers = CommonEndToEndDataSource.Customers;
            return Ok(customers);
        }

        [EnableQuery]
        [HttpGet("odata/Customers/$count")]
        public IActionResult GetCustomersCount()
        {
            var customers = CommonEndToEndDataSource.Customers;
            return Ok(customers);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})/Orders")]
        public IActionResult GetCustomersByKey(int key)
        {
            var customerOrders = CommonEndToEndDataSource.Customers.SingleOrDefault(a=>a.CustomerId == key).Orders;
            return Ok(customerOrders);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})/Orders/$count")]
        public IActionResult GetCustomersByKeyCount(int key)
        {
            var customerOrders = CommonEndToEndDataSource.Customers.SingleOrDefault(a => a.CustomerId == key).Orders;
            return Ok(customerOrders);
        }
    }
}
