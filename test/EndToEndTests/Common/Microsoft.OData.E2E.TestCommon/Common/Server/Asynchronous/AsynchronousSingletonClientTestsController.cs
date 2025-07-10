//---------------------------------------------------------------------
// <copyright file="AsynchronousSingletonClientTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.AsyncRequestTests;

public class AsynchronousSingletonClientTestsController : ODataController
{
    private static DefaultDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/Departments")]
    public IActionResult GetDepartments()
    {
        return Ok(_dataSource.Departments);
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
    [HttpGet("odata/Company/VipCustomer")]
    public IActionResult GetCompanyVipCustomer()
    {
        var vipCustomer = _dataSource.Company?.VipCustomer;
        return Ok(vipCustomer);
    }

    [EnableQuery]
    [HttpGet("odata/Company/VipCustomer/Company")]
    public IActionResult GetVipCustomerCompany()
    {
        var vipCustomer = _dataSource.Company?.VipCustomer;
        return Ok(vipCustomer?.Company);
    }

    [EnableQuery]
    [HttpGet("odata/Company/Address/City")]
    public IActionResult GetCompanyAddressCity()
    {
        var result = _dataSource.Company;

        return Ok(result?.Address?.City);
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
    [HttpGet("odata/PublicCompany/Name")]
    public IActionResult GetPublicCompanyName()
    {
        var result = _dataSource.PublicCompany;

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
    [HttpGet("odata/PublicCompany/Microsoft.OData.E2E.TestCommon.Common.Server.Default.PublicCompany/Club")]
    public IActionResult GetPublicCompanyClub()
    {
        var result = _dataSource.PublicCompany;

        return Ok(result?.Club);
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

    [HttpPost("odata/PublicCompany/Microsoft.OData.E2E.TestCommon.Common.Server.Default.PublicCompany/Assets")]
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

    [HttpPatch("odata/VipCustomer")]
    public IActionResult UpdateVipCustomer([FromBody] Delta<Customer> delta)
    {
        var vipCustomer = _dataSource.VipCustomer;
        if (vipCustomer == null)
        {
            return NotFound();
        }

        var updated = delta.Patch(vipCustomer);
        return Updated(updated);
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

    [HttpPatch("odata/PublicCompany/Microsoft.OData.E2E.TestCommon.Common.Server.Default.PublicCompany/Club")]
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

    [HttpDelete("odata/Company/VipCustomer/$ref")]
    public IActionResult DeleteVipCustomerRefFromCompany()
    {
        var company = _dataSource.Company;
        if (company == null)
        {
            return NotFound();
        }

        company.VipCustomer = null;

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

    [HttpDelete("odata/PublicCompany/Microsoft.OData.E2E.TestCommon.Common.Server.Default.PublicCompany/Assets({key})")]
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

    [HttpPost("odata/asynchronoussingletonclienttests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = DefaultDataSource.CreateInstance();
        return Ok();
    }
}
