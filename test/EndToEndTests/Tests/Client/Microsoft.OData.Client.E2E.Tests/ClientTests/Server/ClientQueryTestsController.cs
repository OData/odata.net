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
        private static CommonEndToEndDataSource _dataSource;

        [EnableQuery]
        [HttpGet("odata/People")]
        public IActionResult Get()
        {
            var people = _dataSource.People;

            return Ok(people);
        }

        [EnableQuery]
        [HttpGet("odata/People/$count")]
        public IActionResult GetPeopleCount()
        {
            var people = _dataSource.People;

            return Ok(people);
        }

        [EnableQuery]
        [HttpGet("odata/People({key})")]
        public IActionResult Get(int key)
        {
            var person = _dataSource.People?.FirstOrDefault(a => a.PersonId == key);

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
            var logins = _dataSource.Logins;

            return Ok(logins);
        }

        [EnableQuery]
        [HttpGet("odata/Logins({key})")]
        public IActionResult GetLogin(string key)
        {
            var login = _dataSource.Logins?.FirstOrDefault(a => a.Username == key);
            
            if (login == null)
            {
                return NotFound();
            }

            return Ok(login);
        }

        [EnableQuery]
        [HttpGet("odata/Logins({key})/SentMessages(fromUsername={fromUsername},messageId={messageId})")]
        public IActionResult GetLoginSentMessages([FromODataUri] string key, [FromODataUri] string fromUsername, [FromODataUri] int messageId)
        {
            var login = _dataSource.Logins?.SingleOrDefault(a => a.Username == key);

            if (login == null)
            {
                return NotFound();
            }

            Message sentMessage = login.SentMessages.SingleOrDefault(a => a.FromUsername == fromUsername && a.MessageId == messageId);

            return Ok(sentMessage);
        }

        [EnableQuery]
        [HttpGet("odata/Logins({key})/SentMessages(messageId={messageId})")]
        public IActionResult GetLoginSentMessage([FromODataUri] string key, [FromODataUri] int messageId)
        {
            return BadRequest("Key Mismatch");
        }

        [EnableQuery]
        [HttpGet("odata/Logins({key})/SentMessages({messageId})")]
        public IActionResult GetLoginSentMessageWithKeyMismatch([FromODataUri] string key, [FromODataUri] int messageId)
        {
            return BadRequest("Key Mismatch");
        }

        [EnableQuery]
        [HttpGet("odata/Cars")]
        public IActionResult GetCars()
        {
            var cars = _dataSource.Cars;

            return Ok(cars);
        }

        [EnableQuery]
        [HttpGet("odata/Cars({key})")]
        public IActionResult GetCar(int key)
        {
            var car = _dataSource.Cars?.FirstOrDefault(a => a.VIN == key);
            
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
            var customers = _dataSource.Customers;

            return Ok(customers);
        }

        [EnableQuery]
        [HttpGet("odata/Customers/$count")]
        public IActionResult GetCustomersCount()
        {
            var customers = _dataSource.Customers;

            return Ok(customers);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})/Orders")]
        public IActionResult GetCustomerOrders(int key)
        {
            var customer = _dataSource.Customers.SingleOrDefault(a => a.CustomerId == key);
            
            if (customer == null)
            {
                return NotFound();
            }

            var customerOrders = customer.Orders;

            return Ok(customerOrders);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})/Orders/$count")]
        public IActionResult GetCustomerOrdersCount(int key)
        {
            var customer = _dataSource.Customers.SingleOrDefault(a => a.CustomerId == key);

            if (customer == null)
            {
                return NotFound();
            }

            var customerOrders = customer.Orders;

            return Ok(customerOrders);
        }

        [HttpPost("odata/clientquery/Default.ResetDataSource")]
        public IActionResult ResetDataSource()
        {
            _dataSource = CommonEndToEndDataSource.CreateInstance();

            return Ok();
        }
    }
}
