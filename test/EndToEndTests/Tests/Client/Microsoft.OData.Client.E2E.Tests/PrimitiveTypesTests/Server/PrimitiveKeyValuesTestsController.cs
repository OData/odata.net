//---------------------------------------------------------------------
// <copyright file="PrimitiveTypesTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
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
    [HttpGet("odata/EdmBinarySet/{key}")]
    public IActionResult GetEdmBinary([FromODataUri] byte[] key)
    {
        var result = _dataSource.EdmBinaries?.SingleOrDefault(a => a.Id.SequenceEqual(key));
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmBooleanSet")]
    public IActionResult GetEdmBooleans()
    {
        var result = _dataSource.EdmBooleans;
        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmBooleanSet/{key}")]
    public IActionResult GetEdmBoolean([FromODataUri] bool key)
    {
        var result = _dataSource.EdmBooleans?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmBooleanSet({key})")]
    public IActionResult GetEdmBooleanWithParentheses([FromODataUri] bool key)
    {
        var result = _dataSource.EdmBooleans?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmInt16Set")]
    public IActionResult GetEdmInt16s()
    {
        var result = _dataSource.EdmInt16s;
        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmInt16Set/{key}")]
    public IActionResult GetEdmInt16([FromODataUri] short key)
    {
        var result = _dataSource.EdmInt16s?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmInt16Set({key})")]
    public IActionResult GetEdmInt16WithParenthese([FromODataUri] short key)
    {
        var result = _dataSource.EdmInt16s?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmInt32Set")]
    public IActionResult GetEdmInt32s()
    {
        var result = _dataSource.EdmInt32s;
        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmInt32Set/{key}")]
    public IActionResult GetEdmInt32([FromODataUri] int key)
    {
        var result = _dataSource.EdmInt32s?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmInt32Set({key})")]
    public IActionResult GetEdmInt32WithParentheses([FromODataUri] int key)
    {
        var result = _dataSource.EdmInt32s?.SingleOrDefault(a => a.Id == key);
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

    [EnableQuery]
    [HttpGet("odata/EdmInt64Set/$count")]
    public IActionResult GetEdmInt64sCount()
    {
        var result = _dataSource.EdmInt64s;
        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmInt64Set/{key}")]
    public IActionResult GetEdmInt64([FromODataUri] long key)
    {
        var result = _dataSource.EdmInt64s?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmInt64Set({key})")]
    public IActionResult GetEdmInt64WithParentheses([FromODataUri] long key)
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

    [EnableQuery]
    [HttpGet("odata/EdmSingleSet/$count")]
    public IActionResult GetEdmSinglesCount()
    {
        var result = _dataSource.EdmSingles;
        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmSingleSet/{key}")]
    public IActionResult GetEdmSingle([FromODataUri] float key)
    {
        var result = _dataSource.EdmSingles?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmSingleSet({key})")]
    public IActionResult GetEdmSingleWithParentheses([FromODataUri] float key)
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
    [HttpGet("odata/EdmDoubleSet/{key}")]
    public IActionResult GetEdmDouble([FromODataUri] double key)
    {
        var result = _dataSource.EdmDoubles?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmDoubleSet({key})")]
    public IActionResult GetEdmDoubleWithParentheses([FromODataUri] double key)
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
    public IActionResult GetEdmDecimals()
    {
        var result = _dataSource.EdmDecimals;
        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmDecimalSet/{key}")]
    public IActionResult GetEdmDecimal([FromODataUri] decimal key)
    {
        var result = _dataSource.EdmDecimals?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
    [EnableQuery]
    [HttpGet("odata/EdmDecimalSet({key})")]
    public IActionResult GetEdmDecimalWithParentheses([FromODataUri] decimal key)
    {
        var result = _dataSource.EdmDecimals?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmDateTimeOffsetSet")]
    public IActionResult GetEdmDateTimeOffsets()
    {
        var result = _dataSource.EdmDateTimeOffsets;
        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmDateTimeOffsetSet/{key}")]
    public IActionResult GetEdmDateTimeOffset([FromODataUri] DateTimeOffset key)
    {
        var result = _dataSource.EdmDateTimeOffsets?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmDateTimeOffsetSet({key})")]
    public IActionResult GetEdmDateTimeOffsetWithParentheses([FromODataUri] DateTimeOffset key)
    {
        var result = _dataSource.EdmDateTimeOffsets?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmTimeSet")]
    public IActionResult GetEdmTimes()
    {
        var result = _dataSource.EdmTimes;
        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmTimeSet/{key}")]
    public IActionResult GetEdmTime([FromODataUri] TimeSpan key)
    {
        var result = _dataSource.EdmTimes?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmTimeSet({key})")]
    public IActionResult GetEdmTimeWithParentheses([FromODataUri] TimeSpan key)
    {
        var result = _dataSource.EdmTimes?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [EnableQuery(PageSize = 2)]
    [HttpGet("odata/EdmStringSet")]
    public IActionResult GetEdmStrings()
    {
        var result = _dataSource.EdmStrings;
        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmStringSet/{key}")]
    public IActionResult GetEdmString([FromODataUri] string key)
    {
        var result = _dataSource.EdmStrings?.SingleOrDefault(a => a.Id == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/EdmStringSet({key})")]
    public IActionResult GetEdmStringWithParentheses([FromODataUri] string key)
    {
        var result = _dataSource.EdmStrings?.SingleOrDefault(a => a.Id == key);
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
