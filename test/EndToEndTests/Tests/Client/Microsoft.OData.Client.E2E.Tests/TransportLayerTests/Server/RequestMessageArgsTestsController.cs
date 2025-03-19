//-----------------------------------------------------------------------------
// <copyright file="RequestMessageArgsTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.TransportLayerTests.Server
{
    public class RequestMessageArgsTestsController : ODataController
    {
        private static CommonEndToEndDataSource _dataSource;

        [EnableQuery]
        [HttpGet("odata/Products")]
        public IActionResult Get()
        {
            var products = _dataSource.Products;

            return Ok(products);
        }

        [HttpPatch("odata/Products({key})")]
        public IActionResult PatchProduct([FromRoute] int key, [FromBody] Delta<Product> delta)
        {
            var product = _dataSource.Products.SingleOrDefault(a => a.ProductId == key);

            if (product == null)
            {
                return NotFound();
            }

            var updatedProduct = delta.Patch(product);

            return Updated(updatedProduct);
        }

        [HttpPost("odata/requestmessageargs/Default.ResetDataSource")]
        public IActionResult ResetDataSource()
        {
            _dataSource = CommonEndToEndDataSource.CreateInstance();

            return Ok();
        }
    }
}
