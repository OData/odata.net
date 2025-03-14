//---------------------------------------------------------------------
// <copyright file="UrlModifyingTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.UrlModifying;

public class UrlModifyingTestsController : ODataController
{
    private static DefaultDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/People")]
    public IActionResult GetPeople()
    {
        return Ok(_dataSource.People);
    }

    [EnableQuery]
    [HttpGet("odata/Customers")]
    public IActionResult GetCustomers()
    {
        return Ok(_dataSource.Customers);
    }

    [HttpPost("odata/urlmodifyingtests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = DefaultDataSource.CreateInstance();

        return Ok();
    }
}
