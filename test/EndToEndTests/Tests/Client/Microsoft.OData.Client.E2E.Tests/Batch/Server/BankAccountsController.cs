using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Client.E2E.Tests.Batch.Server
{
    internal class BankAccountsController : ODataController
    {
        // POST: odata/Banks
        [EnableQuery]
        [HttpPost("odata/BankAccounts")]
        public IActionResult Post([FromBody] BankAccount bankAccount)
        {
            if (bankAccount == null)
            {
                return BadRequest();
            }
            BanksController._dataSource.BankAccounts.Add(bankAccount);
            return Created(bankAccount);
        }

        // PUT: /odata/$1/Bank
        [EnableQuery]
        [HttpPut("odata/BankAccounts({id})/Bank/$ref")]
        public IActionResult PutBankAccountBank([FromODataUri] int id, [FromBody] Bank bank)
        {
            if (bank == null)
            {
                return BadRequest();
            }
            BankAccount bankAccount = BanksController._dataSource.BankAccounts.First();
            bankAccount.Bank = bank;
            return Updated(bankAccount);
        }
    }
}
