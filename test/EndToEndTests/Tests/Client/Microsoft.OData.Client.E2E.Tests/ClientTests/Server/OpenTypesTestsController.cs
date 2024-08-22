//-----------------------------------------------------------------------------
// <copyright file="OpenTypesTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.OpenTypes;

namespace Microsoft.OData.Client.E2E.Tests.ClientTests.Server
{
    public class OpenTypesTestsController : ODataController
    {
        private static OpenTypesDataSource _dataSource;

        [EnableQuery]
        [HttpGet("odata/Rows")]
        public IActionResult Get()
        {
            var row = _dataSource.Rows;

            return Ok(row);
        }

        [HttpPatch("odata/Rows({key})")]
        public IActionResult Patch([FromODataUri] Guid key, Delta<Row> row)
        {
            var rowToUpdate = _dataSource.Rows.SingleOrDefault(a => a.Id == key);

            if (rowToUpdate == null)
            {
                return NotFound();
            }

            var updatedRow = row.Patch(rowToUpdate);

            return Updated(updatedRow);
        }

        [HttpPost("odata/Rows")]
        public IActionResult Post([FromBody] Row row)
        {
            _dataSource.Rows.Add(row);

            return Created(row);
        }


        [EnableQuery]
        [HttpGet("odata/RowIndices")]
        public IActionResult RowIndex()
        {
            var rowIndex = _dataSource.RowIndices;

            return Ok(rowIndex);
        }

        [HttpPost("odata/clientopentypeupdate/Default.ResetOpenTypesDataSource")]
        public IActionResult ResetOpenTypesDataSource()
        {
            _dataSource = OpenTypesDataSource.CreateInstance();

            return Ok();
        }
    }
}
