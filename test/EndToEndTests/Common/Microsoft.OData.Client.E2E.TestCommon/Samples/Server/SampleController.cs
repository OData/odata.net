//-----------------------------------------------------------------------------
// <copyright file="SampleController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Microsoft.OData.Client.E2E.TestCommon.Samples.Server
{
    public class SampleController : ODataController
    {
        public SampleController()
        {
        }

        [HttpGet("sample/Products")]
        public IActionResult Products()
        {
            var products = SampleDataSource.Products;
            return Ok(products);
        }
    }
}
