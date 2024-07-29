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
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;

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
            var products = CommonEndToEndDataSource.Products;
            return Ok(products);
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            var product = CommonEndToEndDataSource.Products.FirstOrDefault(a => a.ProductId == key);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost("odata/RetrieveProduct")]
        public IActionResult RetrieveProduct()
        {
            var product = CommonEndToEndDataSource.Products.FirstOrDefault(a => a.ProductId == -9);

            if (product == null)
            { 
                return NotFound();
            }

            var productId = product.ProductId;

            return Ok(productId);
        }

        [HttpPost("odata/Products({key})/RetrieveProduct")]
        public IActionResult RetrieveProduct([FromODataUri] int key)
        {
            var product = CommonEndToEndDataSource.Products.FirstOrDefault(a => a.ProductId == key);

            if (product == null)
            {
                return NotFound();
            }

            var productId = product.ProductId;

            return Ok(productId);
        }
    }
}
