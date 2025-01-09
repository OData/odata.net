//---------------------------------------------------------------------
// <copyright file="SingletonTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;

namespace Microsoft.OData.Client.E2E.Tests.SingletonTests.Server;

public class SingletonClientTestsController : ODataController
{
    private static DefaultDataSource _dataSource;


    #region odata/VipCustomer

    [EnableQuery]
    [HttpGet("odata/VipCustomer")]
    public IActionResult GetVipCustomer()
    {
        var result = _dataSource.VipCustomer;

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/VipCustomer/PersonID")]
    public IActionResult GetVipCustomerPersonID()
    {
        var result = _dataSource.VipCustomer;

        return Ok(result?.PersonID);
    }

    [EnableQuery]
    [HttpGet("odata/VipCustomer/Orders")]
    public IActionResult GetVipCustomerOrders()
    {
        var result = _dataSource.VipCustomer;

        return Ok(result?.Orders);
    }

    [EnableQuery]
    [HttpGet("odata/VipCustomer/Orders({key})/OrderDate")]
    public IActionResult GetVipCustomerOrderOrderDate([FromRoute] int key)
    {
        var result = _dataSource.VipCustomer?.Orders?.SingleOrDefault(a => a.OrderID == key);

        return Ok(result?.OrderDate);
    }

    [EnableQuery]
    [HttpGet("odata/VipCustomer/HomeAddress")]
    public IActionResult GetVipCustomerHomeAddress()
    {
        var result = _dataSource.VipCustomer;

        return Ok(result?.HomeAddress);
    }

    [EnableQuery]
    [HttpGet("odata/VipCustomer/HomeAddress/City")]
    public IActionResult GetVipCustomerHomeAddressCity()
    {
        var result = _dataSource.VipCustomer;

        return Ok(result?.HomeAddress?.City);
    }

    [HttpPatch("odata/VipCustomer")]
    public IActionResult UpdateVipCustomer([FromBody] Delta<Customer> delta)
    {
        var customer = _dataSource.VipCustomer;
        if (customer == null)
        {
            return NotFound();
        }

        var updatedResult = delta.Patch(customer);
        return Updated(updatedResult);
    }

    #endregion

    #region odata/Company

    [EnableQuery]
    [HttpGet("odata/Company")]
    public IActionResult GetCompany()
    {
        var result = _dataSource.Company;

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/Company/Name")]
    public IActionResult GetCompanyName()
    {
        var result = _dataSource.Company;

        return Ok(result?.Name);
    }

    [EnableQuery]
    [HttpGet("odata/Company/CompanyCategory")]
    public IActionResult GetCompanyCompanyCategory()
    {
        var result = _dataSource.Company;

        return Ok(result?.CompanyCategory);
    }

    [EnableQuery]
    [HttpGet("odata/Company/VipCustomer")]
    public IActionResult GetCompanyVipCustomer()
    {
        var result = _dataSource.Company;

        return Ok(result?.VipCustomer);
    }

    [EnableQuery]
    [HttpGet("odata/Company/Departments")]
    public IActionResult GetCompanyDepartments()
    {
        var result = _dataSource.Company;

        return Ok(result?.Departments);
    }

    [EnableQuery]
    [HttpGet("odata/Company/Revenue")]
    public IActionResult GetRevenue()
    {
        var result = _dataSource.Company;

        return Ok(result?.Revenue);
    }

    [EnableQuery]
    [HttpGet("odata/Company/CoreDepartment")]
    public IActionResult GetCompanyCoreDepartment()
    {
        var result = _dataSource.Company;

        return Ok(result?.CoreDepartment);
    }

    [EnableQuery]
    [HttpGet("odata/Company/Address/City")]
    public IActionResult GetCompanyAddressCity()
    {
        var result = _dataSource.Company;

        return Ok(result?.Address?.City);
    }

    [EnableQuery]
    [HttpGet("odata/Company/Default.GetEmployeesCount")]
    public IActionResult GetEmployeesCount()
    {
        var result = _dataSource.Company;

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result.Employees?.Count);
    }

    [EnableQuery]
    [HttpPost("odata/Company/Default.IncreaseRevenue")]
    public IActionResult IncreaseRevenue([FromODataBody] int IncreaseValue)
    {
        var result = _dataSource.Company;

        if (result == null)
        {
            return NotFound();
        }

        result.Revenue += IncreaseValue;

        return Ok(result.Revenue);
    }

    [HttpPost("odata/Company/Departments/$ref")]
    public IActionResult AddDepartmentRefToCompany([FromBody] Uri departmentUri)
    {
        if (departmentUri == null)
        {
            return BadRequest();
        }

        // Extract the department ID from the URI
        var lastSegment = departmentUri.Segments.Last();
        var departmentId = int.Parse(Regex.Match(lastSegment, @"\d+").Value);

        // Find the department by ID
        var department = _dataSource.Departments?.SingleOrDefault(d => d.DepartmentID == departmentId);
        if (department == null)
        {
            return NotFound();
        }

        // Add the department reference to the company
        var company = _dataSource.Company;
        if (company == null)
        {
            return NotFound();
        }

        company.Departments ??= [];
        company.Departments.Add(department);

        return Ok(department);
    }

    [HttpPut("odata/company")]
    public IActionResult UpdateCompany([FromBody] Company company)
    {
        var companyToUpdate = _dataSource.Company;
        if (companyToUpdate == null)
        {
            return NotFound();
        }

        companyToUpdate.CompanyID = company.CompanyID == 0 ? companyToUpdate.CompanyID : company.CompanyID;
        companyToUpdate.Address = company.Address ?? companyToUpdate.Address;
        companyToUpdate.CompanyCategory = company.CompanyCategory;
        companyToUpdate.Name = company.Name ?? companyToUpdate.Name;
        companyToUpdate.Employees = company.Employees ?? companyToUpdate.Employees;
        companyToUpdate.Revenue = company.Revenue;
        companyToUpdate.CoreDepartment = company.CoreDepartment ?? companyToUpdate.CoreDepartment;
        companyToUpdate.VipCustomer = company.VipCustomer ?? companyToUpdate.VipCustomer;
        companyToUpdate.Departments = company.Departments ?? companyToUpdate.Departments;

        return Updated(companyToUpdate);
    }

    [HttpPatch("odata/company")]
    public IActionResult PatchCompany([FromBody] Delta<Company> delta)
    {
        var company = _dataSource.Company;
        if (company == null)
        {
            return NotFound();
        }

        var updatedResult = delta.Patch(company);
        return Updated(updatedResult);
    }

    [HttpPut("odata/Company/CoreDepartment/$ref")]
    public IActionResult UpdateCompanyCoreDepartmentRef([FromBody] Uri departmentUri)
    {
        if (departmentUri == null)
        {
            return BadRequest();
        }

        // Extract the department ID from the URI
        var lastSegment = departmentUri.Segments.Last();
        var departmentId = int.Parse(Regex.Match(lastSegment, @"\d+").Value);

        // Find the department by ID
        var department = _dataSource.Departments?.SingleOrDefault(d => d.DepartmentID == departmentId);
        if (department == null)
        {
            return NotFound();
        }

        // Update the core department reference in the company
        var company = _dataSource.Company;
        if (company == null)
        {
            return NotFound();
        }

        company.CoreDepartment = department;

        return NoContent();
    }

    [HttpPut("odata/Company/VipCustomer/$ref")]
    public IActionResult UpdateCompanyVipCustomerRef([FromBody] Uri vipCustomerUri)
    {
        var vipCustomer = _dataSource.VipCustomer;
        if (vipCustomer == null)
        {
            return NotFound();
        }

        // Update the vipCustomer reference in the company
        var company = _dataSource.Company;
        if (company == null)
        {
            return NotFound();
        }

        company.VipCustomer = vipCustomer;

        return NoContent();
    }

    [HttpDelete("odata/Company/Departments/$ref")]
    public IActionResult DeleteDepartmentRefFromCompany([FromODataUri] string id)
    {
        id ??= Request.Query["$id"].ToString();

        var uriId = new Uri(id);

        // Extract the department ID from the URI
        var lastSegment = uriId.Segments.Last();
        var departmentId = int.Parse(Regex.Match(lastSegment, @"\d+").Value);

        // Find the department by ID
        var department = _dataSource.Departments?.SingleOrDefault(d => d.DepartmentID == departmentId);
        if (department == null)
        {
            return NotFound();
        }

        // Remove the department reference from the company
        var company = _dataSource.Company;
        if (company == null)
        {
            return NotFound();
        }

        company.Departments?.Remove(department);

        return NoContent();
    }

    [HttpDelete("odata/Company/VipCustomer/$ref")]
    public IActionResult DeleteVipCustomerRefFromCompany()
    {
        // Remove the VipCustomer reference from the company
        var company = _dataSource.Company;
        if (company == null)
        {
            return NotFound();
        }

        company.VipCustomer = null;

        return NoContent();
    }

    #endregion

    #region odata/Boss

    [EnableQuery]
    [HttpGet("odata/Boss")]
    public IActionResult GetBoss()
    {
        var result = _dataSource.Boss;

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/Boss/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.Customer")]
    public IActionResult GetBossFromDerivedType()
    {
        var result = _dataSource.Boss;

        if (result is not Customer customer)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [EnableQuery]
    [HttpGet("odata/Boss/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.Customer/City")]
    public IActionResult GetBossCityFromDerivedType()
    {
        var result = _dataSource.Boss;

        if (result is not Customer customer)
        {
            return NotFound();
        }

        return Ok(customer.City);
    }

    #endregion

    #region odata/Departments

    [EnableQuery]
    [HttpGet("odata/Departments")]
    public IActionResult GetDepartments()
    {
        var result = _dataSource.Departments;

        return Ok(result);
    }

    [HttpPut("odata/Departments({key})/Company/$ref")]
    public IActionResult UpdateDepartmentCompanyRef([FromRoute] int key, [FromBody] Uri companyUri)
    {
        if (companyUri == null)
        {
            return BadRequest();
        }

        // Find the company by ID
        var company = _dataSource.Company;
        if (company == null)
        {
            return NotFound();
        }

        // Find the department by ID
        var department = _dataSource.Departments?.SingleOrDefault(d => d.DepartmentID == key);
        if (department == null)
        {
            return NotFound();
        }

        // Update the company reference in the department
        department.Company = company;

        return NoContent();
    }

    [HttpPost("odata/Departments")]
    public IActionResult CreateDepartment([FromBody] Department department)
    {
        if (department == null)
        {
            return BadRequest();
        }

        _dataSource.Departments?.Add(department);
        return Created(department);
    }

    #endregion

    #region odata/PublicCompany

    [EnableQuery]
    [HttpGet("odata/PublicCompany")]
    public IActionResult GetPublicCompany()
    {
        var result = _dataSource.PublicCompany;

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/PublicCompany/Name")]
    public IActionResult GetPublicCompanyName()
    {
        var result = _dataSource.PublicCompany;

        return Ok(result?.Name);
    }

    [EnableQuery]
    [HttpGet("odata/PublicCompany/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.PublicCompany/StockExchange")]
    public IActionResult GetPublicCompanyStockExchange()
    {
        var result = _dataSource.PublicCompany;

        return Ok(result?.StockExchange);
    }

    [EnableQuery]
    [HttpGet("odata/PublicCompany/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.PublicCompany/Assets")]
    public IActionResult GetPublicCompanyAssets()
    {
        var result = _dataSource.PublicCompany;

        return Ok(result?.Assets);
    }

    [EnableQuery]
    [HttpGet("odata/PublicCompany/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.PublicCompany/Club")]
    public IActionResult GetPublicCompanyClub()
    {
        var result = _dataSource.PublicCompany;

        return Ok(result?.Club);
    }

    [EnableQuery]
    [HttpGet("odata/PublicCompany/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.PublicCompany/LabourUnion")]
    public IActionResult GetPublicCompanyLabourUnion()
    {
        var result = _dataSource.PublicCompany;

        return Ok(result?.LabourUnion);
    }

    [HttpPost("odata/PublicCompany/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.PublicCompany/Assets")]
    public IActionResult AddAssetToPublicCompanyAssets([FromBody] Asset asset)
    {
        var result = _dataSource.PublicCompany;
        if (result == null)
        {
            return NotFound();
        }

        result.Assets ??= [];
        result.Assets.Add(asset);

        return Created(asset);
    }

    [HttpPut("odata/PublicCompany/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.PublicCompany")]
    public IActionResult UpdatePublicCompany([FromBody] PublicCompany company)
    {
        var companyToUpdate = _dataSource.PublicCompany;

        if (companyToUpdate == null)
        {
            return NotFound();
        }

        companyToUpdate.CompanyID = company.CompanyID == 0 ? companyToUpdate.CompanyID : company.CompanyID;
        companyToUpdate.Address = company.Address ?? companyToUpdate.Address;
        companyToUpdate.CompanyCategory = company.CompanyCategory;
        companyToUpdate.Name = company.Name ?? companyToUpdate.Name;
        companyToUpdate.Employees = company.Employees ?? companyToUpdate.Employees;
        companyToUpdate.Revenue = company.Revenue;
        companyToUpdate.CoreDepartment = company.CoreDepartment ?? companyToUpdate.CoreDepartment;
        companyToUpdate.VipCustomer = company.VipCustomer ?? companyToUpdate.VipCustomer;
        companyToUpdate.Departments = company.Departments ?? companyToUpdate.Departments;
        companyToUpdate.StockExchange = company.StockExchange ?? companyToUpdate.StockExchange;
        companyToUpdate.Assets = company.Assets ?? companyToUpdate.Assets;
        companyToUpdate.Club = company.Club ?? companyToUpdate.Club;
        companyToUpdate.LabourUnion = company.LabourUnion ?? companyToUpdate.LabourUnion;

        return Updated(companyToUpdate);
    }

    [HttpPatch("odata/PublicCompany/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.PublicCompany/Club")]
    public IActionResult PatchPublicCompanyClub([FromBody] Delta<Club> delta)
    {
        var club = _dataSource.PublicCompany?.Club;
        if (club == null)
        {
            return NotFound();
        }

        var updatedResult = delta.Patch(club);
        return Updated(updatedResult);
    }

    [HttpPatch("odata/LabourUnion")]
    public IActionResult PatchPublicCompanyLabourUnion([FromBody] Delta<LabourUnion> delta)
    {
        var labourUnion = _dataSource.LabourUnion;
        if (labourUnion == null)
        {
            return NotFound();
        }

        var updatedResult = delta.Patch(labourUnion);
        return Updated(updatedResult);
    }

    [HttpDelete("odata/PublicCompany/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.PublicCompany/Assets({key})")]
    public IActionResult RemoveAssetFromPublicCompanyAssets([FromRoute] int key)
    {
        var result = _dataSource.PublicCompany;
        if (result == null)
        {
            return NotFound();
        }

        var asset = result.Assets?.SingleOrDefault(a => a.AssetID == key);
        if (asset == null)
        {
            return NotFound();
        }

        result.Assets?.Remove(asset);

        return NoContent();
    }

    #endregion

    [HttpPost("odata/singletonclienttests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = DefaultDataSource.CreateInstance();

        return Ok();
    }
}
