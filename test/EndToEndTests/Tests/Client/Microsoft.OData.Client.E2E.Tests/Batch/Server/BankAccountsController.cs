using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.Batch.Server
{
    internal class BankAccountsController : ODataController
    {
        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(Backend.DataSource.BankAccounts);
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            var bankAccount = Backend.DataSource.BankAccounts.FirstOrDefault(b => b.Id == key);
            if (bankAccount == null)
            {
                return NotFound();
            }

            return Ok(bankAccount);
        }

        // POST: odata/Banks
        [EnableQuery]
        public IActionResult Post([FromBody] BankAccount bankAccount)
        {
            if (bankAccount == null)
            {
                return BadRequest();
            }

            Backend.DataSource.BankAccounts.Add(bankAccount);
            return Created(bankAccount);
        }

        // PUT: odata/BankAccounts({key})/Bank/$ref
        [AcceptVerbs("POST", "PUT")]
        public IActionResult CreateRefToBank(int key, [FromBody] Uri link)
        {
            if (link == null)
            {
                return BadRequest();
            }

            if (!Backend.TryGetKeyFromUri(link, out int relatedKey))
            {
                return NotFound();
            }

            var bankAccount = Backend.DataSource.BankAccounts.FirstOrDefault(b => b.Id == key);
            var bank = Backend.DataSource.Banks.FirstOrDefault(b => b.Id == relatedKey);
            bankAccount.Bank = bank;

            return Updated(bankAccount);
        }

        // POST: odata/BankAccounts({key})/Bank
        public IActionResult PostToBank(int key, [FromBody] Bank bank)
        {
            if (bank == null)
            {
                return BadRequest();
            }

            var bankAccount = Backend.DataSource.BankAccounts.FirstOrDefault(b => b.Id == key);

            if (bankAccount == null)
            {
                return NotFound();
            }

            bankAccount.Bank = bank;
            Backend.DataSource.Banks.Add(bank);

            return Created(bank);
        }

        // DELETE: odata/BankAccounts({key})/Bank/$ref
        public IActionResult DeleteRefToBank(int key)
        {
            var bankAccount = Backend.DataSource.BankAccounts.FirstOrDefault(b => b.Id == key);
            if (bankAccount == null)
            {
                return NotFound();
            }

            bankAccount.Bank = null;
            return NoContent();
        }
    }
}
