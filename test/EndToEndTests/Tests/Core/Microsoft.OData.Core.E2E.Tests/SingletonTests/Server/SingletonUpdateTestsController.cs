//---------------------------------------------------------------------
// <copyright file="SingletonTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;

namespace Microsoft.OData.Core.E2E.Tests.SingletonTests.Server;

public class SingletonUpdateTestsController : ODataController
{
    private static DefaultDataSource _dataSource;


    #region odata/VipCustomer

    [EnableQuery]
    [HttpGet("odata/VipCustomer")]
    public IActionResult GetVipCustomer()
    {
        var result = _dataSource.VipCustomer;

        return Ok(result);
    }

    [HttpPatch("odata/VipCustomer")]
    public IActionResult UpdateVipCustomer([FromBody] Delta<Customer> delta)
    {
        var customer = _dataSource.VipCustomer;
        if (customer == null)
        {
            return NotFound();
        }

        var updatedResult = delta.Patch(customer);
        return Updated(updatedResult);
    }

    #endregion

    [HttpPost("odata/singletonupdatetests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = DefaultDataSource.CreateInstance();

        return Ok();
    }
}
