//-----------------------------------------------------------------------------
// <copyright file="OrderLinesController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server
{
    public class OrderLinesController : ODataController
    {
        public OrderLinesController()
        {
        }

        [EnableQuery]
        public IActionResult Get()
        {
            var orderLines = CommonEndToEndDataSource.OrderLines;

            if (orderLines == null)
            {
                return NotFound();
            }

            return Ok(orderLines);
        }

        [EnableQuery]
        public IActionResult Get([FromRoute] int keyOrderId, [FromRoute] int keyProductId)
        {
            var orderLine = CommonEndToEndDataSource.OrderLines?.FirstOrDefault(a => a.OrderId == keyOrderId && a.ProductId == keyProductId);

            if (orderLine == null)
            {
                return NotFound();
            }

            return Ok(orderLine);
        }

        [HttpPost("odata/OrderLines({keyOrderId},{keyProductId})/RetrieveProduct")]
        public IActionResult RetrieveProduct([FromODataUri] int keyOrderId, [FromODataUri] int keyProductId)
        {
            var orderLine = CommonEndToEndDataSource.OrderLines?.FirstOrDefault(a => a.OrderId == keyOrderId && a.ProductId == keyProductId);

            if (orderLine == null)
            {
                return NotFound();
            }

            var productId = orderLine.ProductId;

            return Ok(productId);
        }
    }
}
