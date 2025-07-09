//---------------------------------------------------------------------
// <copyright file="InstanceAnnotationTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.Annotations;

public class InstanceAnnotationTestsController : ODataController
{
    private static DefaultDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/Boss")]
    public IActionResult GetBoss()
    {
        return Ok(_dataSource.Boss);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})")]
    public IActionResult GetPerson(int key)
    {
        var person = _dataSource.People?.FirstOrDefault(p => p.PersonID == key);
        if (person == null)
        {
            return NotFound();
        }

        return Ok(person);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})/HomeAddress")]
    public IActionResult GetHomeAddress(int key)
    {
        var person = _dataSource.People?.FirstOrDefault(p => p.PersonID == key);
        if (person == null)
        {
            return NotFound();
        }

        var homeAddress = person.HomeAddress as HomeAddress;
        return Ok(homeAddress);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})/HomeAddress/City")]
    public IActionResult GetHomeAddressCity(int key)
    {
        var person = _dataSource.People?.FirstOrDefault(p => p.PersonID == key);
        if (person == null)
        {
            return NotFound();
        }

        var homeAddress = person.HomeAddress as HomeAddress;
        return Ok(homeAddress?.City);
    }

    [HttpPost("odata/annotationstests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = DefaultDataSource.CreateInstance();

        return Ok();
    }
}
