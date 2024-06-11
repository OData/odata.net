//-----------------------------------------------------------------------------
// <copyright file="ProductsController.cs" company=".NET Foundation">
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
    public class ProductsController : ODataController
    {
        public ProductsController()
        {
        }

        [EnableQuery]
        public IActionResult Get()
        {
            var products = ActionOverloadingDataSource.Products;
            return Ok(products);
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            var product = ActionOverloadingDataSource.Products?.FirstOrDefault(a => a.ProductId == key);
            return Ok(product);
        }

        [HttpPost("odata/RetrieveProduct")]
        public IActionResult RetrieveProduct()
        {
            var productId = ActionOverloadingDataSource.Products?.FirstOrDefault(a => a.ProductId == -10)?.ProductId;
            return Ok(productId);
        }

        [HttpPost("odata/Products({key})/RetrieveProduct")]
        public IActionResult RetrieveProduct([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var productId = ActionOverloadingDataSource.Products?.FirstOrDefault(a=>a.ProductId == key)?.ProductId;

            return Ok(productId);
        }
    }
}
