//---------------------------------------------------------------------
// <copyright file="PrimitiveTypesTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.PrimitiveKeys;

namespace Microsoft.OData.Client.E2E.Tests.PrimitiveTypesTests.Server;

public class PrimitiveKeyValuesTestsController : ODataController
{
    private static PrimitiveKeyValuesDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/EdmBinarySet")]
    public IActionResult GetEdmBinaries()
    {
        var result = _dataSource.EdmBinaries;
        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmBinarySet({key})")]
    public IActionResult GetEdmBinary([FromRoute] byte[] key)
    {
        var result = _dataSource.EdmBinaries?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery(PageSize = 2)]
    [HttpGet("odata/EdmInt64Set")]
    public IActionResult GetEdmInt64s()
    {
        var result = _dataSource.EdmInt64s;
        return Ok(result);
    }

    [HttpGet("odata/EdmInt64Set/$count")]
    public IActionResult GetEdmInt64sCount()
    {
        var result = _dataSource.EdmInt64s;
        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmInt64Set({key})")]
    public IActionResult GetEdmInt64([FromRoute] long key)
    {
        var result = _dataSource.EdmInt64s?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery(PageSize = 2)]
    [HttpGet("odata/EdmSingleSet")]
    public IActionResult GetEdmSingles()
    {
        var result = _dataSource.EdmSingles;
        return Ok(result);
    }

    [HttpGet("odata/EdmSingleSet/$count")]
    public IActionResult GetEdmSinglesCount()
    {
        var result = _dataSource.EdmSingles;
        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmSingleSet({key})")]
    public IActionResult GetEdmSingle([FromRoute] float key)
    {
        var result = _dataSource.EdmSingles?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery(PageSize = 2)]
    [HttpGet("odata/EdmDoubleSet")]
    public IActionResult GetEdmDoubles()
    {
        var result = _dataSource.EdmDoubles;
        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmDoubleSet({key})")]
    public IActionResult GetEdmDouble([FromRoute] double key)
    {
        var result = _dataSource.EdmDoubles?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery(PageSize = 2)]
    [HttpGet("odata/EdmDecimalSet")]
    public IActionResult EdmDecimals()
    {
        var result = _dataSource.EdmDecimals;
        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmDecimalSet({key})")]
    public IActionResult EdmDecimal([FromRoute] decimal key)
    {
        var result = _dataSource.EdmDecimals?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("odata/primitivekeyvalues/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = PrimitiveKeyValuesDataSource.CreateInstance();

        return Ok();
    }
}
