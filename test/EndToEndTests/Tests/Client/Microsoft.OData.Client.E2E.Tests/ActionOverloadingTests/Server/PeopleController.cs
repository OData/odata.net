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

namespace Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server
{
    public class PeopleController : ODataController
    {
        public PeopleController() { }

        [EnableQuery]
        public IActionResult Get()
        {
            var people = ActionOverloadingDataSource.People;
            return Ok(people);
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            var person = ActionOverloadingDataSource.People?.FirstOrDefault(a => a.PersonId == key);
            return Ok(person);
        }

        [HttpPost("odata/Default.UpdatePersonInfo")]
        public IActionResult UpdatePersonInfo()
        {
            ActionOverloadingDataSource.People.First().Name += "[UpdataPersonName]";
            return Ok();
        }

        [HttpPost("odata/People({key})/Default.UpdatePersonInfo")]
        public IActionResult UpdatePersonInfo([FromODataUri] int key)
        {
            ActionOverloadingDataSource.People.First(a => a.PersonId == key).Name += "[UpdataPersonName]";
            return Ok();
        }

        [HttpPost("odata/People({key})/Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.Person/Default.UpdatePersonInfo")]
        public IActionResult UpdateEmployeeInfo([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            ActionOverloadingDataSource.People.First(a => a.PersonId == key).Name += "[UpdataPersonName]";

            return Ok();
        }

        [HttpPost("odata/People({key})/Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.Employee/Default.UpdatePersonInfo")]
        public IActionResult UpdateEmployeeeInfo([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            ActionOverloadingDataSource.People.First(a => a.PersonId == key).Name += "[UpdataPersonName]";

            return Ok();
        }

        [HttpPost("odata/People({key})/Default.IncreaseEmployeeSalary")]
        public IActionResult IncreaseEmployeeSalary([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var specialEmployee = (SpecialEmployee)ActionOverloadingDataSource.People.First(a => a.PersonId == key);
            specialEmployee.Salary+= 1;

            return Ok(specialEmployee.Salary);
        }

        [HttpPost("odata/People({key})/Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.Employee/Default.IncreaseEmployeeSalary")]
        public IActionResult IncreaseEmployeeSalary([FromODataUri] int key, ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var employee = (Employee)ActionOverloadingDataSource.People.First(a => a.PersonId == key);
            employee.Salary += (int)parameters["n"];

            return Ok(true);
        }

        [HttpPost("odata/People/Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.Employee/Default.IncreaseSalaries")]
        public IActionResult IncreaseSalaries(ODataActionParameters parameters)
        {
            var employees = ActionOverloadingDataSource.People.OfType<Employee>();
            foreach (var employee in employees)
            {
                employee.Salary+= (int)parameters["n"];
            }

            return Ok();
        }

        [HttpPost("odata/People/Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.SpecialEmployee/Default.IncreaseSalaries")]
        public IActionResult IncreaseSpecialEmployeesSalaries(ODataActionParameters parameters)
        {
            var employees = ActionOverloadingDataSource.People.OfType<SpecialEmployee>();
            foreach (var employee in employees)
            {
                employee.Salary += (int)parameters["n"];
            }

            return Ok();
        }
    }
}
