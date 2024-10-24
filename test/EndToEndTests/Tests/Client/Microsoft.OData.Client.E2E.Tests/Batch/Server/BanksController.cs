using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.Batch.Server
{
    public class BanksController : ODataController
    {
        private static CommonEndToEndDataSource _dataSource = CommonEndToEndDataSource.CreateInstance();

        [EnableQuery]
        [HttpGet("odata/Banks")]
        public IActionResult Get()
        {
            var banks = _dataSource.Banks; 
            return Ok(banks);
        }

        // POST: odata/Banks
        [EnableQuery]
        [HttpPost("odata/Banks")]
        public IActionResult Post([FromBody] Bank bankd)
        {
            if (bankd == null)
            {
                return BadRequest();
            }
            _dataSource.Banks.Add(bankd);
            return Created(bankd);
        }

        // POST: /odata/$1/BankAccounts
        [HttpPost]
        [Route("odata/Banks({id})/BankAccounts")]
        public IActionResult PostBankAccount([FromODataUri] int id , [FromBody] BankAccount bankAccount)
        {
            if (bankAccount == null)
            {
                return BadRequest();
            }

            var bank = _dataSource.Banks.FirstOrDefault(b => b.Id == id);
            if (bank == null)
            {
                return NotFound();
            }

            bankAccount.Bank = bank;
            return Created(bankAccount);
        }
    }
}
