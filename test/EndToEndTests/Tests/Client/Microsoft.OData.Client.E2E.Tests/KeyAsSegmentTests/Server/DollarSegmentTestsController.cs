//---------------------------------------------------------------------
// <copyright file="DollarSegmentTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Server;

public class DollarSegmentTestsController : ODataController
{
    private static CommonEndToEndDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/People/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee")]
    public IActionResult GetPeopleOfTypeEmployee()
    {
        var people = _dataSource.People?.OfType<Employee>();
        return Ok(people);
    }

    [EnableQuery]
    [HttpGet("odata/People/{key}/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee/$/Salary")]
    public IActionResult GetEmployeeSalary([FromRoute] int key)
    {
        var employee = _dataSource.People?.OfType<Employee>().SingleOrDefault(a => a.PersonId == key);
        if (employee == null)
        {
            return NotFound();
        }
        return Ok(employee.Salary);
    }

    [EnableQuery]
    [HttpGet("odata/Products/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.DiscontinuedProduct/{key}/RelatedProducts/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.DiscontinuedProduct/{relatedKey}/Photos")]
    public IActionResult GetProductRelatedPhotos([FromRoute] int key, [FromRoute] int relatedKey)
    {
        var product = _dataSource.Products?.SingleOrDefault(a => a.ProductId == key);
        if (product == null)
        {
            return NotFound();
        }

        var relatedProduct = product.RelatedProducts?.SingleOrDefault(a => a.ProductId == relatedKey);
        if (relatedProduct == null)
        {
            return NotFound();
        }

        return Ok(relatedProduct.Photos);
    }

    [EnableQuery]
    [HttpGet("odata/Products/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.DiscontinuedProduct")]
    public IActionResult GetDiscontinuedProducts()
    {
        var discontinuedProducts = _dataSource.Products?.OfType<DiscontinuedProduct>();

        return Ok(discontinuedProducts);
    }

    [EnableQuery]
    [HttpGet("odata/Products/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.DiscontinuedProduct/$")]
    public IActionResult GetAnotherDiscontinuedProducts()
    {
        var discontinuedProducts = _dataSource.Products?.OfType<DiscontinuedProduct>();

        return Ok(discontinuedProducts);
    }

    [EnableQuery]
    [HttpGet("odata/Products/$/$/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.DiscontinuedProduct")]
    public IActionResult GetDiscontinuedProductsMultipleDollarSegments()
    {
        var discontinuedProducts = _dataSource.Products?.OfType<DiscontinuedProduct>();

        return Ok(discontinuedProducts);
    }

    [EnableQuery]
    [HttpGet("odata/Login")]
    public IActionResult GetLogin()
    {
        var login = _dataSource.Logins;
        return Ok(login);
    }

    [HttpPost("odata/People/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee/$/Default.IncreaseSalaries")]
    public IActionResult IncreaseSalaries([FromODataBody] int n)
    {
        _dataSource.People?.OfType<Employee>().ToList().ForEach(e => e.Salary += n);
        return Ok();
    }

    [HttpPost("odata/People/$/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee/$/$/Default.IncreaseSalaries")]
    public IActionResult IncreaseSalariesMultipleDollarSegments([FromODataBody] int n)
    {
        _dataSource.People?.OfType<Employee>().ToList().ForEach(e => e.Salary += n);
        return Ok();
    }

    [HttpPost("odata/People/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee/$/Default.IncreaseSalaries/$")]
    public IActionResult IncreaseSalariesDollarSegmentAtUriEnd([FromODataBody] int n)
    {
        _dataSource.People?.OfType<Employee>().ToList().ForEach(e => e.Salary += n);
        return Ok();
    }

    [HttpPost("odata/People/$/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee/{key}/$/Default.Sack")]
    public IActionResult Sack([FromODataUri] int key)
    {
        var employee = _dataSource.People?.OfType<Employee>().SingleOrDefault(a => a.PersonId == key);
        if (employee == null)
        {
            return NotFound();
        }

        _dataSource.People?.Remove(employee);
        return NoContent();
    }

    [HttpPost("odata/Login")]
    public IActionResult PostLogin([FromBody] Login login)
    {
        _dataSource.Logins?.Add(login);
        return Created(login);
    }

    [HttpPost("odata/dollarsegmenttests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = CommonEndToEndDataSource.CreateInstance();

        return Ok();
    }
}
