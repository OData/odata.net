//---------------------------------------------------------------------
// <copyright file="BatchRequestTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;

namespace Microsoft.OData.Client.E2E.Tests.BatchRequestTests.Server;

public class BatchRequestTestsController : ODataController
{
    private static DefaultDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/Accounts")]
    public IActionResult GetAccounts()
    {
        var result = _dataSource.Accounts;

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/Accounts({key})")]
    public IActionResult GetAccount([FromRoute] int key)
    {
        var result = _dataSource.Accounts?.SingleOrDefault(a => a.AccountID == key);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/Accounts({key})/MyPaymentInstruments")]
    public IActionResult GetAccountMyPaymentInstruments([FromRoute] int key)
    {
        var result = _dataSource.Accounts?.SingleOrDefault(a => a.AccountID == key);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result.MyPaymentInstruments);
    }

    [EnableQuery]
    [HttpGet("odata/Accounts({accountKey})/MyPaymentInstruments({myPaymentInstrumentKey})/BillingStatements({billingStatementKey})")]
    public IActionResult GetAccountBillingStatement([FromRoute] int accountKey, [FromRoute] int myPaymentInstrumentKey, [FromRoute] int billingStatementKey)
    {
        var account = _dataSource.Accounts?.SingleOrDefault(a => a.AccountID == accountKey);
        var myPaymentInstrument = account?.MyPaymentInstruments?.SingleOrDefault(a => a.PaymentInstrumentID == myPaymentInstrumentKey);
        var billingStatement = myPaymentInstrument?.BillingStatements?.SingleOrDefault(a => a.StatementID == billingStatementKey);
        if (billingStatement == null)
        {
            return NotFound();
        }

        return Ok(billingStatement);
    }

    [HttpPatch("odata/Accounts({key})")]
    public IActionResult AddAccount([FromRoute] int key, [FromBody] Delta<Account> delta)
    {
        var account = _dataSource.Accounts?.SingleOrDefault(a => a.AccountID == key);
        if (account == null)
        {
            return NotFound();
        }

        var updated = delta.Patch(account);
        return Ok(updated);
    }

    [HttpPost("odata/Accounts")]
    public IActionResult AddAccount([FromBody] Account account)
    {
        _dataSource.Accounts?.Add(account);

        return Created(account);
    }

    [HttpPost("odata/Accounts({key})/MyPaymentInstruments")]
    public IActionResult GetAccountBillingStatement([FromRoute] int key, [FromBody] PaymentInstrument paymentInstrument)
    {
        var account = _dataSource.Accounts?.SingleOrDefault(a => a.AccountID == key);
        if (account == null)
        {
            return NotFound();
        }

        account.MyPaymentInstruments ??= [];
        account.MyPaymentInstruments.Add(paymentInstrument);

        return Created(paymentInstrument);
    }

    [HttpPost("odata/Accounts({accountKey})/MyPaymentInstruments({myPaymentInstrumentKey})/BillingStatements")]
    public IActionResult GetAccountBillingStatement([FromRoute] int accountKey, [FromRoute] int myPaymentInstrumentKey, [FromBody] Statement statement)
    {
        var account = _dataSource.Accounts?.SingleOrDefault(a => a.AccountID == accountKey);
        if (account == null)
        {
            return NotFound();
        }

        var paymentInstrument = account.MyPaymentInstruments?.SingleOrDefault(a => a.PaymentInstrumentID == myPaymentInstrumentKey);
        if (paymentInstrument == null)
        {
            return NotFound();
        }

        paymentInstrument.BillingStatements ??= [];
        paymentInstrument.BillingStatements.Add(statement);

        return Created(statement);
    }

    [HttpPost("odata/batchrequests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = DefaultDataSource.CreateInstance();

        return Ok();
    }
}
