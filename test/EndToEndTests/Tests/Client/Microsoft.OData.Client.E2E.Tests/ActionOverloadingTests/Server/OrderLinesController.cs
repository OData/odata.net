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
            var products = ActionOverloadingDataSource.OrderLines;
            return Ok(products);
        }

        [EnableQuery]
        public IActionResult Get([FromRoute] int keyOrderId, [FromRoute] int keyProductId)
        {
            var orderLine = ActionOverloadingDataSource.OrderLines?.FirstOrDefault(a => a.OrderId == keyOrderId && a.ProductId == keyProductId);
            return Ok(orderLine);
        }

        [HttpPost("odata/OrderLines({keyOrderId},{keyProductId})/RetrieveProduct")]
        public IActionResult RetrieveProduct([FromODataUri] int keyOrderId, [FromODataUri] int keyProductId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var productId = ActionOverloadingDataSource.OrderLines?.FirstOrDefault(a => a.OrderId == keyOrderId && a.ProductId == keyProductId)?.ProductId;

            return Ok(productId);
        }
    }
}
