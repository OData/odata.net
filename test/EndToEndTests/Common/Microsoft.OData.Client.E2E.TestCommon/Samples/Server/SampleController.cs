//-----------------------------------------------------------------------------
// <copyright file="SampleController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Microsoft.OData.Client.E2E.TestCommon.Samples.Server
{
    public class SampleController : ODataController
    {
        public SampleController()
        {
        }

        [HttpGet("actionresult/Products")]
        public IActionResult Products()
        {
            var products = SampleDataSource.Products;
            return Ok(products);
        }

        [HttpGet("actionresult/GetProduct/{key}")]
        public IActionResult ReturnProduct(int key)
        {
            var product = SampleDataSource.Products.FirstOrDefault(a => a.ProductId == key);
            return Ok(product);
        }

        [HttpPost("actionresult/RetrieveProduct")]
        public int RetrieveProduct()
        {
            var productId = SampleDataSource.Products.First().ProductId;
            return productId;
        }

        [HttpPost("actionresult/RetrieveProductWithOrderLine()")]
        public int RetrieveProductWithOrderLine(ODataActionParameters parameters)
        {
            OrderLine orderLine = (OrderLine)parameters["orderLine"];

            var productId = orderLine.ProductId;

            return productId;
        }
    }
}
