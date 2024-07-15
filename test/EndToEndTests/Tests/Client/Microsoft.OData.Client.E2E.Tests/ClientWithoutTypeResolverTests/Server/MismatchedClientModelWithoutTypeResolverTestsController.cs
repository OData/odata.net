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
        [EnableQuery]
        [HttpGet("odata/Customers")]
        public IActionResult GetCustomers()
        {
            var customers = CommonEndToEndDataSource.Customers;
            return Ok(customers);
        }

        [EnableQuery]
        [HttpGet("odata/Messages")]
        public IActionResult GetMessages()
        {
            var messages = CommonEndToEndDataSource.Messages;
            return Ok(messages);
        }

        [EnableQuery]
        [HttpGet("odata/PageViews")]
        public IActionResult GetPageViews()
        {
            var pageViews = CommonEndToEndDataSource.PageViews;
            return Ok(pageViews);
        }
    }
}
