//-----------------------------------------------------------------------------
// <copyright file="BatchRequestTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server;

namespace Microsoft.OData.Client.E2E.Tests.BatchRequestTests.Server
{
    public class BatchRequestTestsController : ODataController
    {

        [EnableQuery]
        [HttpGet("odata/Accounts")]
        public IActionResult Get()
        {
            var accounts = DefaultDataSource.Accounts;
            return Ok(accounts);
        }

        [HttpPost("odata/Accounts")]
        public IActionResult Post([FromBody] Account account)
        {
            DefaultDataSource.Accounts.Add(account);
            return Created(account);
        }

        [HttpGet("odata/Accounts({accountId})/MyPaymentInstruments")]
        public IActionResult GetMyPaymentInstruments([FromODataUri] int accountId)
        {
            var paymentInstruments = DefaultDataSource.Accounts.FirstOrDefault(a => a.AccountID == accountId).MyPaymentInstruments;

            if (!paymentInstruments.Any())
            {
                return NotFound();
            }

            return Ok(paymentInstruments);
        }

        [EnableQuery]
        [HttpGet("Accounts({accountId})/MyPaymentInstruments({paymentInstrumentId})/BillingStatements({billingStatementId})")]
        public IActionResult GetBillingStatement([FromODataUri] int accountId, [FromODataUri] int paymentInstrumentId, [FromODataUri] int billingStatementId)
        {
            var result = DefaultDataSource.Accounts
                .FirstOrDefault(a => a.AccountID == accountId).MyPaymentInstruments
                .FirstOrDefault(b => b.PaymentInstrumentID == paymentInstrumentId).BillingStatements
                .FirstOrDefault(bs => bs.StatementID == billingStatementId);
            return Ok(result);
        }

        [HttpPost("odata/Accounts({accountId})/MyPaymentInstruments")]
        public async Task<IActionResult> PostMyPaymentInstrument([FromODataUri] int accountId, [FromBody] List<PaymentInstrument> paymentInstrument)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = DefaultDataSource.Accounts.SingleOrDefault(a => a.AccountID == accountId);
            if (account == null)
            {
                return NotFound();
            }

            foreach (var c in paymentInstrument)
            {
                account.MyPaymentInstruments.Add(c);
            }

            return Created(account);
        }
    }
}
