//-----------------------------------------------------------------------------
// <copyright file="OpenTypesWithoutTypeResolverTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.OpenTypes;

namespace Microsoft.OData.Client.E2E.Tests.ClientWithoutTypeResolverTests.Server
{
    public class OpenTypesWithoutTypeResolverTestsController : ODataController
    {
        private static OpenTypesDataSource _dataSource;

        [EnableQuery]
        [HttpGet("odata/RowIndices")]
        public IActionResult RowIndices()
        {
            var rowIndex = _dataSource.RowIndices;
            return Ok(rowIndex);
        }

        [EnableQuery]
        [HttpGet("odata/Rows")]
        public IActionResult GetRows()
        {
            var row = _dataSource.Rows;
            return Ok(row);
        }

        [EnableQuery]
        [HttpGet("odata/Rows({key})")]
        public IActionResult GetRow([FromRoute] Guid key)
        {
            var row = _dataSource.Rows.SingleOrDefault(a=>a.Id == key);

            if (row == null)
            {
                return NotFound();
            }

            return Ok(row);
        }

        [HttpPost("odata/Rows")]
        public IActionResult Post([FromBody] Row row)
        {
            _dataSource.Rows.Add(row);
            return Created(row);
        }

        [EnableQuery]
        [HttpGet("odata/Rows/Microsoft.OData.Client.E2E.Tests.Common.Server.OpenTypes.IndexedRow")]
        public IActionResult GetIndexedRows()
        {
            return Ok(_dataSource.Rows.OfType<IndexedRow>().ToList());
        }

        [HttpPatch("odata/Rows({key})")]
        public IActionResult PatchRow([FromRoute]Guid key, [FromBody] Delta<Row> delta)
        {
            var r = _dataSource.Rows.SingleOrDefault(a => a.Id == key);

            if (r == null)
            {
                return NotFound();
            }

            delta.Patch(r);

            return Updated(r);
        }

        [HttpPost("odata/opentypeswithouttyperesolver/Default.ResetOpenTypesDataSource")]
        public IActionResult ResetOpenTypesDataSource()
        {
            _dataSource = OpenTypesDataSource.CreateInstance();

            return Ok();
        }
    }
}
