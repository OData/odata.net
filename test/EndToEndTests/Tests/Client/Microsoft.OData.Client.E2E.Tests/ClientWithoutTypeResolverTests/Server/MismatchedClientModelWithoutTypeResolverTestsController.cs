//-----------------------------------------------------------------------------
// <copyright file="MismatchedClientModelWithoutTypeResolverTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.ClientWithoutTypeResolverTests.Server
{
    public class MismatchedClientModelWithoutTypeResolverTestsController : ODataController
    {
        private static CommonEndToEndDataSource _dataSource;

        [EnableQuery]
        [HttpGet("odata/Customers")]
        public IActionResult GetCustomers()
        {
            var customers = _dataSource.Customers;
            return Ok(customers);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})")]
        public IActionResult GetCustomer([FromRoute] int key)
        {
            var customer = _dataSource.Customers.SingleOrDefault(a => a.CustomerId == key);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [EnableQuery]
        [HttpGet("odata/Messages")]
        public IActionResult GetMessages()
        {
            var messages = _dataSource.Messages;
            return Ok(messages);
        }

        [EnableQuery]
        [HttpGet("odata/Messages(FromUserName={FromUserName},MessageId={MessageId})/Attachments")]
        public IActionResult GetMessageAttachments([FromRoute] string FromUserName, [FromRoute] int MessageId)
        {
            var message = _dataSource.Messages.SingleOrDefault(a=>a.FromUsername == FromUserName && a.MessageId == MessageId);

            if (message == null)
            {
                return NotFound();
            }

            return Ok(message.Attachments);
        }

        [EnableQuery]
        [HttpGet("odata/PageViews")]
        public IActionResult GetPageViews()
        {
            var pageViews = _dataSource.PageViews;
            return Ok(pageViews);
        }

        [EnableQuery]
        [HttpGet("odata/PageViews({key})")]
        public IActionResult GetPageView([FromRoute] int key)
        {
            var pageView = _dataSource.PageViews.SingleOrDefault(a => a.PageViewId == key);

            if (pageView == null)
            {
                return NotFound();
            }

            return Ok(pageView);
        }

        [HttpPost("odata/mismatchedclient/Default.ResetDataSource")]
        public IActionResult ResetDataSource()
        {
            _dataSource = CommonEndToEndDataSource.CreateInstance();

            return Ok();
        }
    }
}
