//---------------------------------------------------------------------
// <copyright file="IsOfAndCastController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.IsOfAndCast;

public class IsOfAndCastController : ODataController
{
    private static DefaultDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/People")]
    public IActionResult GetPeople()
    {
        return Ok(_dataSource.People);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})")]
    public IActionResult GetPerson([FromQuery] int key)
    {
        var result = _dataSource.People?.FirstOrDefault(p => p.PersonID == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})/Addresses")]
    public IActionResult GetPersonAddresses([FromQuery] int key)
    {
        var result = _dataSource.People?.FirstOrDefault(p => p.PersonID == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result?.Addresses);
    }

    [HttpPost("odata/isofandcasttests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = DefaultDataSource.CreateInstance();

        return Ok();
    }
}
