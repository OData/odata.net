//---------------------------------------------------------------------
// <copyright file="LiteralFormatTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Server;

public class LiteralFormatTestsController : ODataController
{
    private static CommonEndToEndDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/Customers")]
    public IActionResult GetCustomers()
    {
        var customers = _dataSource.Customers;
        return Ok(customers);
    }

    [EnableQuery]
    [HttpGet("odata/Customers/{key}")]
    public IActionResult GetCustomer([FromODataUri] int key)
    {
        var customer = _dataSource.Customers?.SingleOrDefault(c => c.CustomerId == key);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [EnableQuery]
    [HttpGet("odata/Login")]
    public IActionResult GetLogin()
    {
        var login = _dataSource.Logins;
        return Ok(login);
    }

    [EnableQuery]
    [HttpGet("odata/Login/{key}/Customer")]
    public IActionResult GetLoginCustomers([FromODataUri] string key)
    {
        var login = _dataSource.Logins?.SingleOrDefault(l => l.Username == key);
        if (login == null)
        {
            return NotFound();
        }

        return Ok(login.Customer);
    }

    [HttpPost("odata/Login")]
    public IActionResult AddLogin([FromBody] Login login)
    {
        _dataSource.Logins?.Add(login);
        return Created(login);
    }

    [HttpPost("odata/Customers({key})/Logins/$ref")]
    public IActionResult AddLoginsRefToCustomer([FromODataUri] int key, [FromBody] Uri loginUri)
    {
        if (loginUri == null)
        {
            return BadRequest();
        }

        // Extract the login ID from the URI
        var lastSegment = loginUri.Segments.Last();
        int startIndex = lastSegment.IndexOf("('") + 2; // +2 to skip the opening parenthesis and the single quote
        int endIndex = lastSegment.IndexOf("')") - 1; // -1 to skip the closing single quote

        var loginUserName = Uri.UnescapeDataString(lastSegment.Substring(startIndex, endIndex - startIndex + 1));

        // Find the login by ID
        var login = _dataSource.Logins?.SingleOrDefault(d => d.Username == loginUserName);
        if (login == null)
        {
            return NotFound();
        }

        var customer = _dataSource.Customers?.SingleOrDefault(c => c.CustomerId == key);
        if (customer == null)
        {
            return NotFound();
        }

        customer.Logins?.Add(login);

        return Ok(login);
    }

    [HttpPost("odata/keyasssegmentliteralformattests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = CommonEndToEndDataSource.CreateInstance();

        return Ok();
    }
}
