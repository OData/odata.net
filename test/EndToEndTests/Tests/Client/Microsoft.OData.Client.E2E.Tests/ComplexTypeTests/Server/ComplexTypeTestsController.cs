//-----------------------------------------------------------------------------
// <copyright file="ComplexTypeTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.OpenTypes;

namespace Microsoft.OData.Client.E2E.Tests.ComplexTypeTests.Server
{
    public class ComplexTypeTestsController : ODataController
    {
        public ComplexTypeTestsController() { }

        [EnableQuery]
        [HttpGet("odata/People")]
        public IActionResult Get()
        {
            var people = DefaultDataSource.People;

            return Ok(people);
        }

        [EnableQuery]
        [HttpGet("odata/People({key})")]
        public IActionResult Get(int key)
        {
            var person = DefaultDataSource.People.FirstOrDefault(a => a.PersonID == key);

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
            var person = DefaultDataSource.People.FirstOrDefault(a => a.PersonID == key);

            if (person == null)
            {
                return NotFound();
            }

            var personAddress = person.HomeAddress;

            return Ok(personAddress);
        }

        [EnableQuery]
        [HttpGet("odata/People({key})/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.HomeAddress/FamilyName")]
        public IActionResult GetFamilyName(int key)
        {
            var person = DefaultDataSource.People.FirstOrDefault(a => a.PersonID == key);

            if (person == null)
            {
                return NotFound();
            }

            var familyName = person.Addresses.OfType<HomeAddress>().FirstOrDefault().FamilyName;

            return Ok(familyName);
        }

        [HttpPatch("odata/People({key})")]
        public IActionResult PatchPerson([FromRoute] int key, [FromBody] Delta<Person> delta)
        {
            var person = DefaultDataSource.People.SingleOrDefault(a => a.PersonID == key);

            if (person == null)
            {
                return NotFound();
            }

            var updatedPerson = delta.Patch(person);

            return Updated(updatedPerson);
        }

        [HttpPost("odata/People")]
        public IActionResult Post([FromBody] Person person)
        {
            DefaultDataSource.People.Add(person);

            return Created(person);
        }

        [HttpGet("odata/Accounts({key})")]
        public IActionResult GetAccounts([FromRoute] int key)
        {
            var account = DefaultDataSource.Accounts.SingleOrDefault(a=>a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        [HttpGet("odata/Accounts({key})/AccountInfo/MiddleName")]
        public IActionResult GetAccountInfoMiddleName([FromRoute] int key)
        {
            var account = DefaultDataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

            if (account == null)
            {
                return NotFound();
            }

            var middleName = account.AccountInfo.OpenProperties.FirstOrDefault(a=>a.Key == "MiddleName").Value;

            return Ok(middleName);
        }
    }
}
