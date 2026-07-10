//---------------------------------------------------------------------
// <copyright file="NestedCountFilterController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Microsoft.OData.Core.E2E.Tests.NestedCountFilterTests;

public class StoreCustomersController : ODataController
{
    [EnableQuery]
    [HttpGet("odata/StoreCustomers")]
    public IActionResult Get()
    {
        return Ok(NestedCountFilterDataSource.Customers);
    }
}
