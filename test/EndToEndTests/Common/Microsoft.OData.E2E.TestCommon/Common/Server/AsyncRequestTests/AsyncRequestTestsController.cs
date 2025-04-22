//---------------------------------------------------------------------
// <copyright file="AsyncRequestTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.AsyncRequestTests;

public class AsyncRequestTestsController : ODataController
{
    private static DefaultDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/People")]
    public IActionResult GetPeople()
    {
        return Ok(_dataSource.People);
    }

    [EnableQuery]
    [HttpGet("odata/Accounts({key})/MyPaymentInstruments")]
    public IActionResult GetAccountMyPaymentInstruments([FromRoute] int key)
    {
        var account = _dataSource.Accounts?.SingleOrDefault(a => a.AccountID == key);
        if(account == null)
        {
            return NotFound();
        }

        return Ok(account.MyPaymentInstruments);
    }

    [EnableQuery]
    [HttpGet("odata/Accounts({accountKey})/MyPaymentInstruments({piKey})/BillingStatements({bsKey})")]
    public IActionResult GetAccountMyPaymentInstrumentBillingStatement([FromRoute] int accountKey, [FromRoute] int piKey, [FromRoute] int bsKey)
    {
        var account = _dataSource.Accounts?.SingleOrDefault(a => a.AccountID == accountKey);
        if (account == null)
        {
            return NotFound();
        }

        var paymentInstrument = account.MyPaymentInstruments?.SingleOrDefault(m => m.PaymentInstrumentID == piKey);
        if (paymentInstrument == null)
        {
            return NotFound();
        }

        var billingStatement = paymentInstrument.BillingStatements?.SingleOrDefault(b => b.StatementID == bsKey);
        if (billingStatement == null)
        {
            return NotFound();
        }

        return Ok(billingStatement);
    }

    [HttpPost("odata/Accounts({key})/MyPaymentInstruments")]
    public IActionResult AddAccountMyPaymentInstruments([FromRoute] int key, [FromBody] PaymentInstrument paymentInstrument)
    {
        var account = _dataSource.Accounts?.SingleOrDefault(a => a.AccountID == key);
        if (account == null)
        {
            return NotFound();
        }

        account.MyPaymentInstruments = account.MyPaymentInstruments ?? new List<PaymentInstrument>();
        account.MyPaymentInstruments.Add(paymentInstrument);

        return Created(paymentInstrument);
    }

    [HttpPost("odata/Accounts")]
    public IActionResult AddAccount([FromBody] Account account)
    {
        _dataSource.Accounts?.Add(account);
        return Created(account);
    }

    [HttpPost("odata/asyncrequetstests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = DefaultDataSource.CreateInstance();
        return Ok();
    }
}
