//---------------------------------------------------------------------
// <copyright file="EnumerationTypeTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;

namespace Microsoft.OData.Client.E2E.Tests.EnumerationTypeTests.Server;

public class EnumerationTypeTestsController : ODataController
{
    private static DefaultDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/Products")]
    public IActionResult GetProducts()
    {
        var products = _dataSource.Products;
        return Ok(products);
    }

    [EnableQuery]
    [HttpGet("odata/Products({key})")]
    public IActionResult GetProduct([FromODataUri] int key)
    {
        var result = _dataSource.Products?.FirstOrDefault(p => p.ProductID == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [EnableQuery]
    [HttpGet("odata/Products({key})/SkinColor")]
    public IActionResult GetProductSkinColor([FromODataUri] int key)
    {
        var result = _dataSource.Products?.FirstOrDefault(p => p.ProductID == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result.SkinColor);
    }

    [EnableQuery]
    [HttpGet("odata/Products({key})/SkinColor/$value")]
    public IActionResult GetProductSkinColorValue([FromODataUri] int key)
    {
        var result = _dataSource.Products?.FirstOrDefault(p => p.ProductID == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result.SkinColor);
    }

    [EnableQuery]
    [HttpGet("odata/Products({key})/UserAccess")]
    public IActionResult GetProductUserAccess([FromODataUri] int key)
    {
        var result = _dataSource.Products?.FirstOrDefault(p => p.ProductID == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result.UserAccess);
    }

    [EnableQuery]
    [HttpGet("odata/Products({key})/CoverColors")]
    public IActionResult GetProductCoverColors([FromODataUri] int key)
    {
        var result = _dataSource.Products?.FirstOrDefault(p => p.ProductID == key);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result.CoverColors);
    }

    [EnableQuery]
    [HttpGet("odata/GetDefaultColor()")]
    public IActionResult GetDefaultColor()
    {
        return Ok(Color.Red);
    }

    [EnableQuery]
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

    [HttpPost("odata/enumerationtypetests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = DefaultDataSource.CreateInstance();

        return Ok();
    }
}
