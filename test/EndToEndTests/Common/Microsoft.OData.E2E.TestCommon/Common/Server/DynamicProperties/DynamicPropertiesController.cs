//-----------------------------------------------------------------------------
// <copyright file="DynamicPropertiesController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.DynamicProperties;

public class DynamicPropertiesController : ODataController
{
    private static DefaultDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/Accounts")]
    public IActionResult GetAccounts()
    {
        return Ok(_dataSource.Accounts);
    }

    [EnableQuery]
    [HttpGet("odata/Accounts({key})")]
    public IActionResult GetAccount([FromODataUri] int key)
    {
        var account = _dataSource.Accounts?.FirstOrDefault(a => a.AccountID == key);
        if (account == null)
        {
            return NotFound();
        }

        return Ok(account);
    }

    [EnableQuery]
    [HttpGet("odata/Accounts({key})/AccountInfo")]
    public IActionResult GetAccountInfo([FromODataUri] int key)
    {
        var account = _dataSource.Accounts?.FirstOrDefault(a => a.AccountID == key);
        if (account == null)
        {
            return NotFound();
        }

        return Ok(account.AccountInfo);
    }

    [EnableQuery]
    [HttpGet("odata/Accounts({key})/AccountInfo/DynamicProperties")]
    public IActionResult GetDynamicProperties([FromODataUri] int key)
    {
        var account = _dataSource.Accounts?.FirstOrDefault(a => a.AccountID == key);
        if (account == null)
        {
            return NotFound();
        }

        return Ok(account.AccountInfo?.DynamicProperties);
    }

    [HttpPost("odata/Accounts")]
    public IActionResult CreateAccount([FromBody] Account account)
    {
        _dataSource.Accounts?.Add(account);
        return Created(account);
    }

    [HttpPatch("odata/Accounts({key})")]
    public IActionResult UpdateAccount([FromODataUri] int key, [FromBody] Delta<Account> delta)
    {
        var existingAccount = _dataSource.Accounts?.FirstOrDefault(a => a.AccountID == key);
        if (existingAccount == null)
        {
            return NotFound();
        }

        var updated = delta.Patch(existingAccount);
        return Updated(updated);
    }

    [HttpPost("odata/dynamicpropertiestests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = DefaultDataSource.CreateInstance();

        return Ok();
    }
}