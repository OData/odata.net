//-----------------------------------------------------------------------------
// <copyright file="OpenTypesServiceClientTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Microsoft.OData.Client.E2E.Tests.ClientTests.Server
{
    public class OpenTypesServiceClientTestsController : ODataController
    {
        [EnableQuery]
        [HttpGet("odata/Row")]
        public IActionResult Get()
        {
            var row = OpenTypesServiceClientTestsDataSource.Row;
            return Ok(row);
        }

        [HttpPatch("odata/Row({key})")]
        public IActionResult Patch([FromODataUri]Guid key, Row row)
        {
            var rowToUpdate = OpenTypesServiceClientTestsDataSource.Row.SingleOrDefault(a => a.Id == key);
            rowToUpdate.Id = row.Id;
            return Ok();
        }

        [HttpPost("odata/Row")]
        public IActionResult Post(Row row)
        {
            OpenTypesServiceClientTestsDataSource.Row.Add(row);
            return Created(row);
        }


        [EnableQuery]
        [HttpGet("odata/RowIndex")]
        public IActionResult RowIndex()
        {
            var rowIndex = OpenTypesServiceClientTestsDataSource.RowIndex;
            return Ok(rowIndex);
        }
    }
}
