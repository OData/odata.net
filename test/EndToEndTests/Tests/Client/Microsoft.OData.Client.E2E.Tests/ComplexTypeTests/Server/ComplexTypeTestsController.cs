//-----------------------------------------------------------------------------
// <copyright file="ComplexTypeTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;

namespace Microsoft.OData.Client.E2E.Tests.ComplexTypeTests.Server
{
    public class ComplexTypeTestsController : ODataController
    {
        private static DefaultDataSource _dataSource;

        [EnableQuery]
        [HttpGet("odata/People")]
        public IActionResult Get()
        {
            var people = _dataSource.People;

            return Ok(people);
        }

        [EnableQuery]
        [HttpGet("odata/People({key})")]
        public IActionResult Get(int key)
        {
            var person = _dataSource.People.FirstOrDefault(a => a.PersonID == key);

            if (person == null)
            {
                return NotFound();
            }

            return Ok(person);
        }

        [EnableQuery]
        [HttpGet("odata/People({key})/HomeAddress")]
        public IActionResult GetPersonHomeAddress(int key)
        {
            var person = _dataSource.People.FirstOrDefault(a => a.PersonID == key);

            if (person == null)
            {
                return NotFound();
            }

            var personAddress = person.HomeAddress;

            return Ok(personAddress);
        }

        [EnableQuery]
        [HttpGet("odata/People({key})/HomeAddress/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.HomeAddress/FamilyName")]
        public IActionResult GetFamilyName(int key)
        {
            var person = _dataSource.People.FirstOrDefault(a => a.PersonID == key);

            if (person == null)
            {
                return NotFound();
            }

            var familyName = (person.HomeAddress as HomeAddress).FamilyName;

            return Ok(familyName);
        }

        [HttpPatch("odata/People({key})")]
        public IActionResult PatchPerson([FromRoute] int key, [FromBody] Delta<Person> delta)
        {
            var person = _dataSource.People.SingleOrDefault(a => a.PersonID == key);

            if (person == null)
            {
                return NotFound();
            }

            var updatedPerson = delta.Patch(person);

            return Updated(updatedPerson);
        }

        [HttpPut("odata/People({key})/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.Customer")]
        public IActionResult PatchCustomerHomeAddress([FromRoute] int key, [FromBody] Person person)
        {
            var customer = _dataSource.People.SingleOrDefault(a => a.PersonID == key);

            if (customer == null)
            {
                return NotFound();
            }

            var newCustomerAddress = customer.HomeAddress as HomeAddress;
            var originalCustomerAddress = person.HomeAddress as HomeAddress;
            newCustomerAddress.FamilyName = originalCustomerAddress.FamilyName;
            newCustomerAddress.City = originalCustomerAddress.City;

            return Updated(newCustomerAddress);
        }

        [HttpDelete("odata/People({key})")]
        public IActionResult DeletePerson(int key)
        {
            var person = _dataSource.People.SingleOrDefault(a => a.PersonID == key);

            if (person == null)
            {
                return NotFound();
            }

            _dataSource.People.Remove(person);

            return NoContent();
        }

        [HttpPost("odata/People")]
        public IActionResult Post([FromBody] Person person)
        {
            _dataSource.People.Add(person);

            return Created(person);
        }

        [HttpPost("odata/People({key})")]
        public IActionResult PostPerson(int key, [FromBody] Person person)
        {
            _dataSource.People.Add(person);

            return Created(person);
        }

        [EnableQuery]
        [HttpGet("odata/Accounts")]
        public IActionResult GetAccounts()
        {
            var accounts = _dataSource.Accounts;

            return Ok(accounts);
        }

        [HttpGet("odata/Accounts({key})")]
        public IActionResult GetAccount([FromRoute] int key)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a=>a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        [EnableQuery]
        [HttpGet("odata/Accounts({key})/AccountInfo")]
        public IActionResult GetAccountInfo([FromRoute] int key)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var accountInfo = account.AccountInfo;

            return Ok(accountInfo);
        }

        [HttpGet("odata/Accounts({key})/AccountInfo/MiddleName")]
        public IActionResult GetAccountInfoMiddleName([FromRoute] int key)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var middleName = account.AccountInfo.DynamicProperties.FirstOrDefault(a=>a.Key == "MiddleName").Value;

            return Ok(middleName);
        }

        [HttpPost("odata/Accounts")]
        public IActionResult PostAccount([FromBody] Account account)
        {
            _dataSource.Accounts.Add(account);

            return Created(account);
        }

        [HttpPatch("odata/Accounts({key})")]
        public IActionResult PatchAccount([FromRoute] int key, [FromBody] Delta<Account> delta)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var updatedAccount = delta.Patch(account);

            return Updated(updatedAccount);
        }

        [HttpPatch("odata/Accounts({key})/AccountInfo")]
        public IActionResult PatchAccountInfo([FromRoute] int key, [FromBody] Delta<AccountInfo> delta)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var updatedAccountInfo = delta.Patch(account.AccountInfo);

            return Updated(updatedAccountInfo);
        }

        [HttpPut("odata/Accounts({key})")]
        public IActionResult UpdateAccount([FromRoute] int key, [FromBody] Account account)
        {
            var originalCustomer = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (originalCustomer == null)
            {
                return NotFound();
            }

            var updatedAccountInfo = originalCustomer.AccountInfo;
            updatedAccountInfo.FirstName = account.AccountInfo.FirstName;
            updatedAccountInfo.DynamicProperties = account.AccountInfo.DynamicProperties;

            return Updated(updatedAccountInfo);
        }

        [HttpDelete("odata/Accounts({key})")]
        public IActionResult DeleteAccount(int key)
        {
            var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            _dataSource.Accounts.Remove(account);

            return NoContent();
        }

        [HttpGet("odata/People({key})/Default.GetHomeAddress")]
        public IActionResult GetHomeAddress([FromODataUri] int key)
        {
            var person = _dataSource.People.SingleOrDefault(x => x.PersonID == key);

            if (person == null)
            {
                return NotFound();
            }

            var homeAddress = person.HomeAddress;

            return Ok(homeAddress);
        }

        [HttpGet("odata/Accounts({key})/Default.GetAccountInfo")]
        public IActionResult GetAccountInfoFunc([FromODataUri] int key)
        {
            var account = _dataSource.Accounts.SingleOrDefault(x => x.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var accountInfo = account.AccountInfo;

            return Ok(accountInfo);
        }

        [HttpPost("odata/complextype/Default.ResetDefaultDataSource")]
        public IActionResult ResetDefaultDataSource()
        {
            _dataSource = DefaultDataSource.CreateInstance();

            return Ok();
        }
    }
}
