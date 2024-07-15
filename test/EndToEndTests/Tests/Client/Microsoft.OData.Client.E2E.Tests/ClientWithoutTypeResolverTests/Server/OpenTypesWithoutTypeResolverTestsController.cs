//-----------------------------------------------------------------------------
// <copyright file="OpenTypesWithoutTypeResolverTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.OpenTypes;

namespace Microsoft.OData.Client.E2E.Tests.ClientWithoutTypeResolverTests.Server
{
    public class OpenTypesWithoutTypeResolverTestsController : ODataController
    {
        [EnableQuery]
        [HttpGet("odata/RowIndex")]
        public IActionResult RowIndex()
        {
            var rowIndex = OpenTypesServiceDataSource.RowIndex;
            return Ok(rowIndex);
        }

        [EnableQuery]
        [HttpGet("odata/Row")]
        public IActionResult Row()
        {
            var row = OpenTypesServiceDataSource.Row;
            return Ok(row);
        }

        [HttpPost("odata/Row")]
        public IActionResult Post([FromBody] Row row)
        {
            OpenTypesServiceDataSource.Row.Add(row);
            return Created(row);
        }

        [HttpGet("odata/Row/Microsoft.OData.Client.E2E.Tests.Common.Server.OpenTypes.IndexedRow")]
        public IActionResult Get()
        {
            return Ok(OpenTypesServiceDataSource.Row.OfType<IndexedRow>().ToList());
        }
    }
}
