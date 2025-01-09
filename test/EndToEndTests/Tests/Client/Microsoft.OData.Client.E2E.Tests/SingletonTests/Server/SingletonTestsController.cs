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

public class SingletonTestsController : ODataController
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

    #endregion

    [HttpPost("odata/singletontests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = DefaultDataSource.CreateInstance();

        return Ok();
    }
}
