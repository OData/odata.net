//-----------------------------------------------------------------------------
// <copyright file="ActionOverloadingTestsController.cs" company=".NET Foundation">
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
    public class ActionOverloadingTestsController : ODataController
    {
        private static CommonEndToEndDataSource _dataSource;

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
            var person = _dataSource.People.FirstOrDefault(a => a.PersonId == key);

            if (person == null)
            {
                return NotFound();
            }

            return Ok(person);
        }

        [HttpPost("odata/Default.UpdatePersonInfo")]
        public IActionResult UpdatePersonInfo()
        {
            var person = _dataSource.People.SingleOrDefault(a => a.PersonId == -10);

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
            var person = _dataSource.People.SingleOrDefault(x => x.PersonId == key);

            if (person == null)
            {
                return NotFound();
            }

            person.Name += "[UpdataPersonName]";

            return Ok();
        }

        [HttpPost("odata/People({key})/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Person/Default.UpdatePersonInfo")]
        public IActionResult UpdateInfo([FromODataUri] int key)
        {
            var person = _dataSource.People.SingleOrDefault(a => a.PersonId == key);

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
            var person = _dataSource.People.SingleOrDefault(a => a.PersonId == key);

            if (person == null)
            {
                return NotFound();
            }

            person.Name += "[UpdataPersonName]";

            return Ok();
        }

        [HttpPost("odata/People({key})/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.SpecialEmployee/Default.UpdatePersonInfo")]
        public IActionResult UpdateSpecialEmployeeeInfo([FromODataUri] int key)
        {
            var person = _dataSource.People.SingleOrDefault(a => a.PersonId == key);

            if (person == null)
            {
                return NotFound();
            }

            person.Name += "[UpdataPersonName]";

            return Ok();
        }

        [HttpPost("odata/People({key})/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Contractor/Default.UpdatePersonInfo")]
        public IActionResult UpdateContractorInfo([FromODataUri] int key)
        {
            var person = _dataSource.People.SingleOrDefault(a => a.PersonId == key);

            if (person == null)
            {
                return NotFound();
            }

            person.Name += "[UpdataPersonName]";

            return Ok();
        }

        [HttpPost("odata/People({key})/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.SpecialEmployee/Default.IncreaseEmployeeSalary")]
        public IActionResult IncreaseSpecialEmployeeSalary([FromODataUri] int key, ODataActionParameters parameters)
        {
            var person = _dataSource.People.First(a => a.PersonId == key);

            if (person is SpecialEmployee specialEmployee && parameters == null)
            {
                specialEmployee.Salary += 1;
                return Ok(specialEmployee.Salary);
            }
            else
            {
                var employee = (Employee)_dataSource.People.First(a => a.PersonId == key);
                employee.Salary += (int)parameters["n"];

                return Ok(true);
            }
        }

        [HttpPost("odata/People({key})/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee/Default.IncreaseEmployeeSalary")]
        public IActionResult IncreaseEmployeeSalary([FromODataUri] int key, ODataActionParameters parameters)
        {
            var person = _dataSource.People.First(a => a.PersonId == key);

            if (person is SpecialEmployee specialEmployee && parameters == null)
            {
                specialEmployee.Salary += 1;
                return Ok(specialEmployee.Salary);
            }
            else
            {
                var employee = (Employee)_dataSource.People.First(a => a.PersonId == key);
                employee.Salary += (int)parameters["n"];

                return Ok(true);
            }
        }

        [HttpPost("odata/People/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee/Default.IncreaseSalaries")]
        public IActionResult IncreaseSalaries(ODataActionParameters parameters)
        {
            var employees = _dataSource.People.OfType<Employee>();

            foreach (var employee in employees)
            {
                employee.Salary += (int)parameters["n"];
            }

            return Ok();
        }

        [HttpPost("odata/People/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.SpecialEmployee/Default.IncreaseSalaries")]
        public IActionResult IncreaseSpecialEmployeesSalaries(ODataActionParameters parameters)
        {
            var employees = _dataSource.People.OfType<SpecialEmployee>();

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

        [EnableQuery]
        [HttpGet("odata/OrderLines")]
        public IActionResult GetOrderLines()
        {
            var orderLines = _dataSource.OrderLines;

            return Ok(orderLines);
        }

        [EnableQuery]
        [HttpGet("odata/OrderLines(OrderId={keyOrderId},ProductId={keyProductId})")]
        public IActionResult Get([FromRoute] int keyOrderId, [FromRoute] int keyProductId)
        {
            var orderLine = _dataSource.OrderLines?.FirstOrDefault(a => a.OrderId == keyOrderId && a.ProductId == keyProductId);

            if (orderLine == null)
            {
                return NotFound();
            }

            return Ok(orderLine);
        }

        [HttpPost("odata/OrderLines(OrderId={keyOrderId},ProductId={keyProductId})/Default.RetrieveProduct")]
        public IActionResult RetrieveProduct([FromODataUri] int keyOrderId, [FromODataUri] int keyProductId)
        {
            var orderLine = _dataSource.OrderLines?.FirstOrDefault(a => a.OrderId == keyOrderId && a.ProductId == keyProductId);

            if (orderLine == null)
            {
                return NotFound();
            }

            var productId = orderLine.ProductId;

            return Ok(productId);
        }

        [EnableQuery]
        [HttpGet("odata/Products")]
        public IActionResult GetProducts()
        {
            var products = _dataSource.Products;
            return Ok(products);
        }

        [EnableQuery]
        [HttpGet("odata/Products({key})")]
        public IActionResult GetProduct(int key)
        {
            var product = _dataSource.Products.FirstOrDefault(a => a.ProductId == key);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost("odata/RetrieveProduct")]
        public IActionResult RetrieveProduct()
        {
            var product = _dataSource.Products.FirstOrDefault(a => a.ProductId == -9);

            if (product == null)
            {
                return NotFound();
            }

            var productId = product.ProductId;

            return Ok(productId);
        }

        [HttpPost("odata/Products({key})/Default.RetrieveProduct")]
        public IActionResult RetrieveProduct([FromODataUri] int key)
        {
            var product = _dataSource.Products.FirstOrDefault(a => a.ProductId == key);

            if (product == null)
            {
                return NotFound();
            }

            var productId = product.ProductId;

            return Ok(productId);
        }

        [HttpPost("odata/actionoverloading/Default.ResetDataSource")]
        public IActionResult ResetDataSource()
        {
            _dataSource = CommonEndToEndDataSource.CreateInstance();

            return Ok();
        }
    }
}
