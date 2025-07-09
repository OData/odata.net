//-----------------------------------------------------------------------------
// <copyright file="BanksController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.Batch.Server
{
    public class BanksController : ODataController
    {
        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(Backend.DataSource.Banks);
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            var bank = Backend.DataSource.Banks.FirstOrDefault(b => b.Id == key);
            if (bank == null)
            {
                return NotFound();
            }

            return Ok(bank);
        }

        // POST: odata/Banks
        [EnableQuery]
        public IActionResult Post([FromBody] Bank bank)
        {
            if (bank == null)
            {
                return BadRequest();
            }

            Backend.DataSource.Banks.Add(bank);
            return Created(bank);
        }

        // PUT: odata/Banks({key})/BankAccounts/$ref
        [AcceptVerbs("POST", "PUT")]
        public IActionResult CreateRefToBankAccounts(int key, [FromBody] Uri link)
        {
            if (link == null)
            {
                return BadRequest();
            }

            if (!Backend.TryGetKeyFromUri(link, out int relatedKey))
            {
                return NotFound();
            }

            var bank = Backend.DataSource.Banks.FirstOrDefault(b => b.Id == key);
            var bankAccount = Backend.DataSource.BankAccounts.FirstOrDefault(b => b.Id == relatedKey);
            if (bank.BankAccounts?.FirstOrDefault(d => d.Id == bankAccount.Id) != null)
            {
                return Conflict("The bank account is already associated with the bank.");
            }

            if (bank.BankAccounts == null)
            {
                bank.BankAccounts = new List<BankAccount>();
            }

            bank.BankAccounts.Add(bankAccount);

            return Updated(bankAccount);
        }

        // POST: /odata/Banks({key})/BankAccounts
        public IActionResult PostToBankAccounts([FromODataUri] int key, [FromBody] BankAccount bankAccount)
        {
            if (bankAccount == null)
            {
                return BadRequest();
            }

            var bank = Backend.DataSource.Banks.FirstOrDefault(b => b.Id == key);

            if (bank == null)
            {
                return NotFound();
            }

            bankAccount.Bank = bank;
            Backend.DataSource.BankAccounts.Add(bankAccount);

            return Created(bankAccount);
        }

        // DELETE: odata/Banks({key})/BankAccounts/$ref?$id=http://localhost/odata/BankAccounts({relatedKey})
        public IActionResult DeleteRefToBankAccounts(int key, [FromQuery(Name = "$id")] Uri link)
        {
            if (link == null)
            {
                return BadRequest();
            }

            if (!Backend.TryGetKeyFromUri(link, out int relatedKey))
            {
                return NotFound();
            }

            var bank = Backend.DataSource.Banks.FirstOrDefault(b => b.Id == key);
            var bankAccount = Backend.DataSource.BankAccounts.FirstOrDefault(b => b.Id == relatedKey);
            
            if (bank == null || bankAccount == null || bank.BankAccounts?.FirstOrDefault(d => d.Id == bankAccount.Id) == null)
            {
                return NotFound();
            }

            bank.BankAccounts.Remove(bankAccount);
            return NoContent();
        }
    }
}
