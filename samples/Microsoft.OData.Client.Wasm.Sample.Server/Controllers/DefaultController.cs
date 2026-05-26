//---------------------------------------------------------------------
// <copyright file="DefaultController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.Wasm.Sample.Server.Data;

namespace Microsoft.OData.Client.Wasm.Sample.Server.Controllers;

public class DefaultController : ODataController
{
    [HttpPost("odata/ResetDataSource")]
    public IActionResult ResetDataSource()
    {
        DataSource.ResetDataSource();

        return Ok();
    }
}
