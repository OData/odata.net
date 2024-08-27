//-----------------------------------------------------------------------------
// <copyright file="ContainmentTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;

namespace Microsoft.OData.Client.E2E.Tests.ContainmentTest.Server
{
    public class ContainmentTestsController : ODataController
    {
        private static DefaultDataSource _dataSource;

        [EnableQuery]
        [HttpGet("odata/Accounts")]
        public IActionResult GetAccounts()
        {
            var accounts = _dataSource.Accounts;

            return Ok(accounts);
        }

        [EnableQuery]
        [HttpGet("odata/Accounts({key})")]
        public IActionResult GetAccount([FromRoute] int key)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        [EnableQuery]
        [HttpGet("odata/Accounts({key})/MyGiftCard")]
        public IActionResult GetAccountMyGiftCard([FromRoute] int key)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var myGiftCard = account.MyGiftCard;

            return Ok(myGiftCard);
        }

        [EnableQuery]
        [HttpGet("odata/Accounts({key})/MyPaymentInstruments")]
        public IActionResult GetAccountMyPaymentInstruments([FromRoute] int key)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var myPaymentInstruments = account.MyPaymentInstruments;

            return Ok(myPaymentInstruments);
        }

        [EnableQuery]
        [HttpGet("odata/Accounts({key})/MyPaymentInstruments/$count")]
        public IActionResult GetAccountMyPaymentInstrumentsCount([FromRoute] int key)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var myPaymentInstruments = account.MyPaymentInstruments;

            return Ok(myPaymentInstruments);
        }

        [EnableQuery]
        [HttpGet("odata/Accounts({key})/MyPaymentInstruments({paymentInstrumentId})")]
        public IActionResult GetAccountMyPaymentInstrument([FromRoute] int key, [FromRoute] int paymentInstrumentId)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var myPaymentInstruments = account.MyPaymentInstruments;
            PaymentInstrument paymentInstrument = null;

            if (myPaymentInstruments != null)
            {
                paymentInstrument = myPaymentInstruments.SingleOrDefault(a=>a.PaymentInstrumentID == paymentInstrumentId);

                if (paymentInstrument == null)
                {
                    return NotFound();
                }
            }

            return Ok(paymentInstrument);
        }

        [EnableQuery]
        [HttpGet("odata/Accounts({key})/MyPaymentInstruments({paymentInstrumentId})/PaymentInstrumentID")]
        public IActionResult GetAccountMyPaymentInstrumentID([FromRoute] int key, [FromRoute] int paymentInstrumentId)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var myPaymentInstruments = account.MyPaymentInstruments;
            PaymentInstrument paymentInstrument = null;

            if (myPaymentInstruments != null)
            {
                paymentInstrument = myPaymentInstruments.SingleOrDefault(a => a.PaymentInstrumentID == paymentInstrumentId);

                if (paymentInstrument == null)
                {
                    return NotFound();
                }
            }

            return Ok(paymentInstrument.PaymentInstrumentID);
        }

        [EnableQuery]
        [HttpGet("odata/Accounts({key})/MyPaymentInstruments({paymentInstrumentId})/BillingStatements({billingStatementId})")]
        public IActionResult GetMyPaymentInstrumentBillingStatement([FromRoute] int key, [FromRoute] int paymentInstrumentId, [FromRoute] int billingStatementId)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var myPaymentInstruments = account.MyPaymentInstruments;
            Statement billingStatement = null;

            if (myPaymentInstruments != null)
            {
                var paymentInstrument = myPaymentInstruments.SingleOrDefault(a => a.PaymentInstrumentID == paymentInstrumentId);

                if (paymentInstrument == null)
                {
                    return NotFound();
                }

                billingStatement = paymentInstrument.BillingStatements.SingleOrDefault(a => a.StatementID == billingStatementId);
            }

            return Ok(billingStatement);
        }

        [EnableQuery]
        [HttpGet("odata/Accounts({key})/MyPaymentInstruments({paymentInstrumentId})/BillingStatements")]
        public IActionResult GetMyPaymentInstrumentBillingStatementS([FromRoute] int key, [FromRoute] int paymentInstrumentId)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var myPaymentInstruments = account.MyPaymentInstruments;
            List<Statement> billingStatements = [];

            if (myPaymentInstruments != null)
            {
                var paymentInstrument = myPaymentInstruments.SingleOrDefault(a => a.PaymentInstrumentID == paymentInstrumentId);

                if (paymentInstrument == null)
                {
                    return NotFound();
                }

                billingStatements = paymentInstrument.BillingStatements;
            }

            return Ok(billingStatements);
        }


        [EnableQuery]
        [HttpGet("odata/Accounts({key})/MyPaymentInstruments({paymentInstrumentId})/BackupStoredPI")]
        public IActionResult GetMyPaymentInstrumentBackupStoredPI([FromRoute] int key, [FromRoute] int paymentInstrumentId)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var myPaymentInstruments = account.MyPaymentInstruments;
            StoredPI backupStoredPI = null;

            if (myPaymentInstruments != null)
            {
                var paymentInstrument = myPaymentInstruments.SingleOrDefault(a => a.PaymentInstrumentID == paymentInstrumentId);

                if (paymentInstrument == null)
                {
                    return NotFound();
                }

                backupStoredPI = paymentInstrument.BackupStoredPI;
            }

            return Ok(backupStoredPI);
        }

        [EnableQuery]
        [HttpGet("odata/Accounts({key})/MyPaymentInstruments({paymentInstrumentId})/TheStoredPI")]
        public IActionResult GetMyPaymentInstrumentTheStoredPI([FromRoute] int key, [FromRoute] int paymentInstrumentId)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var myPaymentInstruments = account.MyPaymentInstruments;
            StoredPI theStoredPI = null;

            if (myPaymentInstruments != null)
            {
                var paymentInstrument = myPaymentInstruments.SingleOrDefault(a => a.PaymentInstrumentID == paymentInstrumentId);

                if (paymentInstrument == null)
                {
                    return NotFound();
                }

                theStoredPI = paymentInstrument.TheStoredPI;
            }

            return Ok(theStoredPI);
        }

        [EnableQuery]
        [HttpGet("odata/Accounts({key})/MyPaymentInstruments({paymentInstrumentId})/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.CreditCardPI")]
        public IActionResult GetMyPaymentInstrumentCreditCardPI([FromRoute] int key, [FromRoute] int paymentInstrumentId)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var myPaymentInstruments = account.MyPaymentInstruments;
            CreditCardPI creditCardPI = null;

            if (myPaymentInstruments != null)
            {
                var paymentInstrument = myPaymentInstruments.SingleOrDefault(a => a.PaymentInstrumentID == paymentInstrumentId);

                if (paymentInstrument == null)
                {
                    return NotFound();
                }

                creditCardPI = paymentInstrument as CreditCardPI;
            }

            return Ok(creditCardPI);
        }

        [EnableQuery]
        [HttpGet("odata/Accounts({key})/MyPaymentInstruments/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.CreditCardPI")]
        public IActionResult GetMyPaymentInstrumentsCreditCardPI([FromRoute] int key)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            List<CreditCardPI> creditCardPIs = [];

            if (creditCardPIs != null)
            {
                creditCardPIs = account.MyPaymentInstruments.OfType<CreditCardPI>().ToList();
            }

            return Ok(creditCardPIs);
        }

        [EnableQuery]
        [HttpGet("odata/Accounts({key})/MyPaymentInstruments({paymentInstrumentId})/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.CreditCardPI/CreditRecords")]
        public IActionResult GetMyPaymentInstrumentsCreditCardPICreditRecords([FromRoute] int key, [FromRoute] int paymentInstrumentId)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            List<CreditRecord> creditRecords = [];

            if (creditRecords != null)
            {
                var paymentInstrument = account.MyPaymentInstruments.SingleOrDefault(a => a.PaymentInstrumentID == paymentInstrumentId);

                if (paymentInstrument == null)
                {
                    return NotFound();
                }

                creditRecords = (paymentInstrument as CreditCardPI).CreditRecords;
            }

            return Ok(creditRecords);
        }

        [EnableQuery]
        [HttpGet("odata/Accounts({key})/MyGiftCard/Default.GetActualAmount(bonusRate={bonusRate})")]
        public IActionResult GetActualAmount([FromRoute] int key, [FromRoute] double bonusRate)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var amount = account.MyGiftCard.Amount * (1 + bonusRate);

            return Ok(amount);
        }

        [EnableQuery]
        [HttpPost("odata/Accounts")]
        public IActionResult GetAccounts([FromBody] Account account)
        {
            _dataSource.Accounts.Add(account);

            return Created(account);
        }

        [EnableQuery]
        [HttpPost("odata/Accounts({key})/MyPaymentInstruments")]
        public IActionResult PostAccountMyPaymentInstruments([FromRoute] int key, [FromBody] PaymentInstrument paymentInstrument)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            account.MyPaymentInstruments ??= [];

            account.MyPaymentInstruments.Add(paymentInstrument);

            return Created(paymentInstrument);
        }

        [EnableQuery]
        [HttpPatch("odata/Accounts({key})/MyGiftCard")]
        public IActionResult UpdateAccountMyGiftCard([FromRoute] int key, [FromBody] GiftCard giftCard)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            if (account.MyGiftCard != null)
            {
                account.MyGiftCard.GiftCardID = giftCard.GiftCardID;
                account.MyGiftCard.GiftCardNO = giftCard.GiftCardNO;
                account.MyGiftCard.ExperationDate = giftCard.ExperationDate;
                account.MyGiftCard.Amount = giftCard.Amount;
            }
            else
            {
                account.MyGiftCard = giftCard;
            }

            return Updated(account.MyGiftCard);
        }

        [HttpDelete("odata/Accounts({key})/MyPaymentInstruments({paymentInstrumentId})")]
        public IActionResult DeleteAccountMyPaymentInstrument([FromRoute] int key, [FromRoute] int paymentInstrumentId)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var myPaymentInstruments = account.MyPaymentInstruments;

            if (myPaymentInstruments != null)
            {
                var paymentInstrument = myPaymentInstruments.SingleOrDefault(a => a.PaymentInstrumentID == paymentInstrumentId);

                if (paymentInstrument == null)
                {
                    return NotFound();
                }

                myPaymentInstruments.Remove(paymentInstrument);
            }

            return NoContent();
        }

        [EnableQuery]
        [HttpGet("odata/Accounts({key})/Default.GetDefaultPI()")]
        public IActionResult GetDefaultPI([FromRoute] int key)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            if (account.MyPaymentInstruments != null && account.MyPaymentInstruments.Count > 0)
            {
                PaymentInstrument pi = account.MyPaymentInstruments[0];

                return Ok(pi);
            }

            return null;
        }

        [HttpPost("odata/Accounts({key})/Default.RefreshDefaultPI")]
        public IActionResult RefreshDefaultPI([FromRoute] int key, ODataActionParameters parameters)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            if (account.MyPaymentInstruments != null && account.MyPaymentInstruments.Count > 0)
            {
                PaymentInstrument pi = account.MyPaymentInstruments[0];
                pi.CreatedDate = (DateTimeOffset)parameters["newDate"];

                return Ok(pi);
            }

            return null;
        }

        [EnableQuery]
        [HttpPatch("odata/Accounts({key})/MyPaymentInstruments({paymentInstrumentId})")]
        public IActionResult UpdateAccountMyPaymentInstrument([FromRoute] int key, [FromRoute] int paymentInstrumentId, [FromBody] PaymentInstrument updatedPaymentInstrument)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var existingPaymentInstrument = account.MyPaymentInstruments?.SingleOrDefault(pi => pi.PaymentInstrumentID == paymentInstrumentId);

            if (existingPaymentInstrument == null)
            {
                account.MyPaymentInstruments ??= new List<PaymentInstrument>();
                account.MyPaymentInstruments.Add(updatedPaymentInstrument);
            }
            else
            {
                existingPaymentInstrument.FriendlyName = updatedPaymentInstrument.FriendlyName;
                existingPaymentInstrument.CreatedDate = updatedPaymentInstrument.CreatedDate;
            }

            var result = account.MyPaymentInstruments.SingleOrDefault(pi => pi.PaymentInstrumentID == paymentInstrumentId);

            return Updated(result);
        }


        [HttpPost("odata/containmenttests/Default.ResetDefaultDataSource")]
        public IActionResult ResetDefaultDataSource()
        {
            _dataSource = DefaultDataSource.CreateInstance();

            return Ok();
        }
    }
}
