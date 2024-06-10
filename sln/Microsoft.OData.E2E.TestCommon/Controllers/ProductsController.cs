//---------------------------------------------------------------------
// <copyright file="ProductsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Models;

namespace Microsoft.OData.E2E.TestCommon.Controllers
{
    public class ProductsController : ODataController
    {
        public ProductsController()
        {
        }

        [HttpGet("actionresult/RetrieveProduct")]
        public IActionResult RetrieveProduct()
        {
            var product = DataSource.Products.Where(a => a.ProductId == -10);
            return Ok(product);
        }

        [HttpGet("actionresult/RetrieveProductWithOrderLine()")]
        public IActionResult RetrieveProductWithOrderLine(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            OrderLine orderLine = (OrderLine)parameters["orderLine"];

            var productId = orderLine.ProductId;

            return Ok(productId);
        }
    }
}
