//---------------------------------------------------------------------
// <copyright file="BroadCoverageTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.BroadCoverageTests;

public class BroadCoverageTestsController : ODataController
{
    private static BroadCoverageTestsDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/People")]
    public IActionResult GetPeople()
    {
        return Ok(_dataSource.People);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})")]
    public IActionResult GetPerson([FromODataUri] string key)
    {
        var person = _dataSource.People?.FirstOrDefault(p => p.UserName == key);
        if (person == null)
        {
            return NotFound();
        }

        return Ok(person);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})/Trips")]
    public IActionResult GetPersonTrips([FromODataUri] string key)
    {
        var person = _dataSource.People?.FirstOrDefault(p => p.UserName == key);
        if (person == null)
        {
            return NotFound();
        }

        return Ok(person.Trips);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})/Friends")]
    public IActionResult GetPersonFriends([FromODataUri] string key)
    {
        var person = _dataSource.People?.FirstOrDefault(p => p.UserName == key);
        if (person == null)
        {
            return NotFound();
        }

        return Ok(person.Friends);
    }

    //[EnableQuery]
    //[HttpGet("odata/People({personKey})/Trips({tripKey})/PlanItems({planItemKey})/{0}Flight/Airline")]
    //public IActionResult GetPersonFlightAirline([FromODataUri] string personKey)
    //{
    //    var person = _dataSource.People?.FirstOrDefault(p => p.UserName == key);
    //    if (person == null)
    //    {
    //        return NotFound();
    //    }

    //    return Ok(person.Friends);
    //}

    [EnableQuery]
    [HttpGet("odata/Airports")]
    public IActionResult GetAirports()
    {
        return Ok(_dataSource.Airports);
    }

    [HttpPost("odata/broadcoveragetests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = BroadCoverageTestsDataSource.CreateInstance();

        return Ok();
    }
}
