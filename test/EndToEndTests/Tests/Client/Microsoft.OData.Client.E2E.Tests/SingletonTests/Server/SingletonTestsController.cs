//---------------------------------------------------------------------
// <copyright file="SingletonTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;

namespace Microsoft.OData.Client.E2E.Tests.SingletonTests.Server;

public class SingletonTestsController : ODataController
{
    private static DefaultDataSource _dataSource;

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

    [EnableQuery]
    [HttpGet("odata/Company")]
    public IActionResult GetCompany()
    {
        var result = _dataSource.Company;

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/Company/CompanyCategory")]
    public IActionResult GetCompanyCompanyCategory()
    {
        var result = _dataSource.Company;

        return Ok(result?.CompanyCategory);
    }

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

    [HttpPost("odata/singletontests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = DefaultDataSource.CreateInstance();

        return Ok();
    }
}
