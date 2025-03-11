//-----------------------------------------------------------------------------
// <copyright file="TransportLayerErrorController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.TransportLayerTests.Server
{
    public class TransportLayerErrorController : ODataController
    {
        private static CommonEndToEndDataSource _dataSource;

        [EnableQuery]
        [HttpGet("odata/Products")]
        public IActionResult Get()
        {
            var products = _dataSource.Products;

            return Ok(products);
        }

        [EnableQuery]
        [HttpGet("odata/Customers")]
        public IActionResult GetCustomers()
        {
            var customers = _dataSource.Customers;

            return Ok(customers);
        }

        [EnableQuery]
        [HttpGet("odata/MessageAttachments")]
        public IActionResult GetMessageAttachments()
        {
            var products = _dataSource.MessageAttachments;

            return Ok(products);
        }

        [EnableQuery]
        [HttpGet("odata/MessageAttachments({key})")]
        public IActionResult GetMessageAttachment(Guid key)
        {
            var messageAttachment = _dataSource.MessageAttachments.FirstOrDefault(a => a.AttachmentId == key);

            if (messageAttachment == null)
            {
                return NotFound();
            }

            return Ok(messageAttachment);
        }

        [HttpPost("odata/transportlayererror/Default.ResetDataSource")]
        public IActionResult ResetDataSource()
        {
            _dataSource = CommonEndToEndDataSource.CreateInstance();

            return Ok();
        }
    }
}
