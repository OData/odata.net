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

            if (person == null)
            {
                return NotFound();
            }

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
            var login = CommonEndToEndDataSource.Logins?.FirstOrDefault(a => a.Username == key);
            
            if (login == null)
            {
                return NotFound();
            }

            return Ok(login);
        }

        [EnableQuery]
        [HttpGet("odata/Logins({key})/SentMessages(FromUsername={FromUsername},MessageId={MessageId})")]
        public IActionResult GetLoginSentMessages([FromODataUri] string key, [FromODataUri] string FromUsername, [FromODataUri] int MessageId)
        {
            var login = CommonEndToEndDataSource.Logins?.SingleOrDefault(a => a.Username == key);

            if (login == null)
            {
                return NotFound();
            }

            Message sentMessage = login.SentMessages.SingleOrDefault(a => a.FromUsername == FromUsername && a.MessageId == MessageId);

            return Ok(sentMessage);
        }

        [EnableQuery]
        [HttpGet("odata/Logins({key})/SentMessages(MessageId={MessageId})")]
        public IActionResult GetLoginSentMessage([FromODataUri] string key, [FromODataUri] int MessageId)
        {
            return BadRequest("Key Mismatch");
        }

        [EnableQuery]
        [HttpGet("odata/Logins({key})/SentMessages({MessageId})")]
        public IActionResult GetLoginSentMessageWithKeyMismatch([FromODataUri] string key, [FromODataUri] int MessageId)
        {
            return BadRequest("Key Mismatch");
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
            
            if (car == null)
            {
                return NotFound();
            }

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
            var customer = CommonEndToEndDataSource.Customers.SingleOrDefault(a => a.CustomerId == key);
            
            if (customer == null)
            {
                return NotFound();
            }

            var customerOrders = customer.Orders;

            return Ok(customerOrders);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})/Orders/$count")]
        public IActionResult GetCustomersByKeyCount(int key)
        {
            var customer = CommonEndToEndDataSource.Customers.SingleOrDefault(a => a.CustomerId == key);

            if (customer == null)
            {
                return NotFound();
            }

            var customerOrders = customer.Orders;

            return Ok(customerOrders);
        }
    }
}
