//---------------------------------------------------------------------
// <copyright file="AsynchronousDelayQueryTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.Asynchronous;

public class AsynchronousDelayQueryTestsController : ODataController
{
    private static DefaultDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/People")]
    public IActionResult GetPeople()
    {
        return Ok(_dataSource.People);
    }

    [EnableQuery]
    [HttpGet("odata/Company")]
    public IActionResult GetCompany()
    {
        return Ok(_dataSource.Company);
    }

    [HttpGet("odata/Company/Name")]
    public IActionResult GetCompanyName()
    {
        return Ok(_dataSource.Company?.Name);
    }

    [HttpGet("odata/Company/Departments")]
    public IActionResult GetCompanyDepartments()
    {
        return Ok(_dataSource.Company?.Departments);
    }

    [HttpGet("odata/Company/CoreDepartment")]
    public IActionResult GetCompanyCoreDepartment()
    {
        return Ok(_dataSource.Company?.CoreDepartment);
    }

    [EnableQuery]
    [HttpGet("odata/Company/Employees")]
    public IActionResult GetCompanyEmployees()
    {
        return Ok(_dataSource.Company?.Employees);
    }

    [EnableQuery]
    [HttpGet("odata/VipCustomer")]
    public IActionResult GetVipCustomer()
    {
        var vipCustomer = _dataSource.VipCustomer;
        return Ok(vipCustomer);
    }

    [EnableQuery]
    [HttpGet("odata/PublicCompany")]
    public IActionResult GetPublicCompany()
    {
        return Ok(_dataSource.PublicCompany);
    }

    [EnableQuery]
    [HttpGet("odata/PublicCompany/Microsoft.OData.E2E.TestCommon.Common.Server.Default.PublicCompany/StockExchange")]
    public IActionResult GetPublicCompanyStockExchange()
    {
        return Ok((_dataSource.PublicCompany as PublicCompany)?.StockExchange);
    }

    [EnableQuery]
    [HttpGet("odata/PublicCompany/Microsoft.OData.E2E.TestCommon.Common.Server.Default.PublicCompany/Assets")]
    public IActionResult GetPublicCompanyAssets()
    {
        return Ok((_dataSource.PublicCompany as PublicCompany)?.Assets);
    }

    [EnableQuery]
    [HttpGet("odata/PublicCompany/Microsoft.OData.E2E.TestCommon.Common.Server.Default.PublicCompany/LabourUnion")]
    public IActionResult GetPublicCompanyLabourUnion()
    {
        return Ok((_dataSource.PublicCompany as PublicCompany)?.LabourUnion);
    }

    [HttpGet("odata/Products")]
    public IActionResult GetProducts()
    {
        return Ok(_dataSource.Products);
    }

    [EnableQuery]
    [HttpGet("odata/GetBossEmails(start={start},count={count})")]
    public IActionResult GetAllProducts([FromODataUri] int start, [FromODataUri] int count)
    {
        var boss = _dataSource.Boss;
        var emails = new Collection<string>();
        if (boss.Emails.Count >= start + 1)
        {
            for (int i = start; i < count && i < boss.Emails.Count; i++)
            {
                emails.Add(boss.Emails[i]);
            }
        }
        return Ok(emails);
    }

    [EnableQuery]
    [HttpGet("odata/GetProductsByAccessLevel(accessLevel={accessLevel})")]
    public IActionResult GetProductsByAccessLevel([FromODataUri] AccessLevel accessLevel)
    {
        var count = _dataSource.Products?.Where(p => p.UserAccess == accessLevel).Count();
        return Ok((double)count);
    }

    [HttpGet("odata/Products({key})")]
    public IActionResult GetProduct([FromODataUri] int key)
    {
        var product = _dataSource.Products?.SingleOrDefault(p => p.ProductID == key);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpGet("odata/Products({key})/Default.GetProductDetails(count={count})")]
    public IActionResult GetProductDetails([FromODataUri] int key, [FromODataUri] int count)
    {
        var product = _dataSource.Products?.FirstOrDefault(p => p.ProductID == key);
        if (product == null)
        {
            return NotFound();
        }

        if (product.Details?.Count < count)
        {
            return Ok(product.Details);
        }

        return Ok(product.Details?.Take(count));
    }

    [EnableQuery]
    [HttpGet("odata/Accounts({key})/MyGiftCard/Default.GetActualAmount(bonusRate={bonusRate})")]
    public IActionResult GetActualAmount([FromRoute] int key, [FromRoute] double bonusRate)
    {
        var account = _dataSource.Accounts.SingleOrDefault(a => a.AccountID == key);

        if (account == null)
        {
            return NotFound();
        }

        var amount = account.MyGiftCard.Amount * (1 + bonusRate);

        return Ok(amount);
    }

    [EnableQuery]
    [HttpGet("odata/Company/Default.GetEmployeesCount()")]
    public IActionResult GetEmployeesCount()
    {
        var result = _dataSource.Company;

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result.Employees?.Count);
    }

    [HttpPost("odata/Products")]
    public IActionResult AddProduct([FromBody] Product product)
    {
        _dataSource.Products?.Add(product);
        return Created(_dataSource.Products?.Single(p => p.ProductID == product.ProductID));
    }

    [HttpPost("odata/Products/Default.Discount")]
    public IActionResult Discount([FromODataBody] int percentage)
    {
        foreach (var product in _dataSource.Products)
        {
            product.UnitPrice = product.UnitPrice * (1 - percentage / 100.0f);
        }

        return Ok(_dataSource.Products);
    }

    [HttpPost("odata/Discount")]
    public IActionResult AnotherDiscount([FromODataBody] int percentage)
    {
        foreach (var product in _dataSource.Products)
        {
            product.UnitPrice = product.UnitPrice * (1 - percentage / 100.0f);
        }

        return Ok();
    }

    [HttpPost("odata/Products({key})/Default.AddAccessRight")]
    public IActionResult AddAccessRight([FromODataUri] int key, [FromODataBody] AccessLevel accessRight)
    {
        var product = _dataSource.Products?.FirstOrDefault(p => p.ProductID == key);
        if (product == null)
        {
            return NotFound();
        }

        product.UserAccess = accessRight;
        return Ok(product.UserAccess);
    }

    [HttpPatch("odata/Company")]
    public IActionResult UpdateCompany([FromBody] Delta<Company> delta)
    {
        var company = _dataSource.Company;
        if (company == null)
        {
            return NotFound();
        }

        var updated = delta.Patch(company);
        return Updated(updated);
    }

    [HttpPatch("odata/PublicCompany/Microsoft.OData.E2E.TestCommon.Common.Server.Default.PublicCompany")]
    public IActionResult PatchPublicCompany([FromBody] Delta<PublicCompany> delta)
    {
        var company = _dataSource.PublicCompany as PublicCompany;
        if (company == null)
        {
            return NotFound();
        }

        var updatedCompany = delta.Patch(company);
        return Updated(updatedCompany);
    }

    [HttpPost("odata/PublicCompany/Microsoft.OData.E2E.TestCommon.Common.Server.Default.PublicCompany/LabourUnion/Default.ChangeLabourUnionName")]
    public IActionResult ChangeLabourUnionName([FromODataBody] string name)
    {
        var labourUnion = (_dataSource.PublicCompany as PublicCompany)?.LabourUnion;
        if (labourUnion == null)
        {
            return NotFound();
        }

        labourUnion.Name = name;
        return Ok();
    }

    [HttpPost("odata/asynchronousdelayquerytests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = DefaultDataSource.CreateInstance();
        return Ok();
    }
}
