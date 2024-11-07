//-----------------------------------------------------------------------------
// <copyright file="ClientMultipleKeysEnumKeyTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.MultipleKeys;

namespace Microsoft.OData.Client.E2E.Tests.ClientTests.Server
{
    public class ClientMultipleKeysEnumKeyTestsController : ODataController
    {
        private static MultipleKeysEnumKeyDataSource _dataSource;

        [EnableQuery]
        [HttpGet("odata/Employees")]
        public IActionResult Get()
        {
            var employees = _dataSource.Employees;

            return Ok(employees);
        }

        [EnableQuery]
        [HttpGet("odata/Employees/$count")]
        public IActionResult GetEmployeesCount()
        {
            var employees = _dataSource.Employees;

            return Ok(employees);
        }

        [EnableQuery]
        [HttpGet("odata/Employees(employeeNumber={employeeNumber},employeeType={employeeType})")]
        public IActionResult Get([FromODataUri] int employeeNumber, [FromODataUri] EmployeeType employeeType)
        {
            var employee = _dataSource.Employees?.FirstOrDefault(a => a.EmployeeNumber == employeeNumber && a.EmployeeType == employeeType);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpPost("odata/clientmultiplekeysenumkey/Default.ResetDefaultDataSource")]
        public IActionResult ResetDefaultDataSource()
        {
            _dataSource = MultipleKeysEnumKeyDataSource.CreateInstance();

            return Ok();
        }
    }
}
