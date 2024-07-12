//-----------------------------------------------------------------------------
// <copyright file="CollectionNullableFacetTestController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server;

namespace Microsoft.OData.Client.E2E.Tests.CollectionTests.Server
{
    public class CollectionNullableFacetTestController : ODataController
    {
        [EnableQuery]
        [HttpGet("odata/Customers")]
        public IActionResult GetCustomers()
        {
            var rowIndex = DefaultDataSource.Customers;
            return Ok(rowIndex);
        }
    }
}
