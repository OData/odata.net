//-----------------------------------------------------------------------------
// <copyright file="PeopleController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server
{
    public class PeopleController : ODataController
    {
        public PeopleController() { }

        [EnableQuery]
        [HttpGet("odata/People")]
        public IActionResult Get()
        {
            var people = CommonEndToEndDataSource.People;

            return Ok(people);
        }

        [EnableQuery]
        [HttpGet("odata/People({key})")]
        public IActionResult Get(int key)
        {
            var person = CommonEndToEndDataSource.People.FirstOrDefault(a => a.PersonId == key);

            if (person == null)
            {
                return NotFound();
            }

            return Ok(person);
        }

        [HttpPost("odata/Default.UpdatePersonInfo")]
        public IActionResult UpdatePersonInfo()
        {
            var person = CommonEndToEndDataSource.People.SingleOrDefault(a => a.PersonId == -10);
            if (person == null)
            {
                return NotFound();
            }

            person.Name += "[UpdataPersonName]";

            return Ok();
        }

        [HttpPost("odata/People({key})/Default.UpdatePersonInfo")]
        public IActionResult UpdatePersonInfo([FromODataUri] int key)
        {
            var person = CommonEndToEndDataSource.People.SingleOrDefault(x => x.PersonId == key);

            if (person == null)
            {
                return NotFound();
            }

            person.Name += "[UpdataPersonName]";

            return Ok();
        }

        [HttpPost("odata/People({key})/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Person/Default.UpdatePersonInfo")]
        public IActionResult UpdateEmployeeInfo([FromODataUri] int key)
        {
            var person = CommonEndToEndDataSource.People.SingleOrDefault(a => a.PersonId == key);

            if (person == null)
            {
                return NotFound();
            }

            person.Name += "[UpdataPersonName]";

            return Ok();
        }

        [HttpPost("odata/People({key})/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee/Default.UpdatePersonInfo")]
        public IActionResult UpdateEmployeeeInfo([FromODataUri] int key)
        {
            var person = CommonEndToEndDataSource.People.SingleOrDefault(a => a.PersonId == key);

            if (person == null)
            {
                return NotFound();
            }

            person.Name += "[UpdataPersonName]";

            return Ok();
        }

        [HttpPost("odata/People({key})/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee/Default.IncreaseEmployeeSalary")]
        public IActionResult IncreaseEmployeeSalary([FromODataUri] int key, ODataActionParameters parameters)
        {
            var person = CommonEndToEndDataSource.People.First(a => a.PersonId == key);
            if (person is SpecialEmployee specialEmployee && parameters == null)
            {
                specialEmployee.Salary += 1;
                return Ok(specialEmployee.Salary);
            }
            else
            {
                var employee = (Employee)CommonEndToEndDataSource.People.First(a => a.PersonId == key);
                employee.Salary += (int)parameters["n"];

                return Ok(true);
            }
        }

        [HttpPost("odata/People/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee/Default.IncreaseSalaries")]
        public IActionResult IncreaseSalaries(ODataActionParameters parameters)
        {
            var employees = CommonEndToEndDataSource.People.OfType<Employee>();
            foreach (var employee in employees)
            {
                employee.Salary += (int)parameters["n"];
            }

            return Ok();
        }

        [HttpPost("odata/People/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.SpecialEmployee/Default.IncreaseSalaries")]
        public IActionResult IncreaseSpecialEmployeesSalaries(ODataActionParameters parameters)
        {
            var employees = CommonEndToEndDataSource.People.OfType<SpecialEmployee>();
            foreach (var employee in employees)
            {
                employee.Salary += (int)parameters["n"];
            }

            return Ok();
        }

        [HttpPost("odata/People({key})/Default.Sack")]
        public IActionResult Sack([FromODataUri] int key)
        {
            return Ok();
        }
    }
}
