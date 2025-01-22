//---------------------------------------------------------------------
// <copyright file="EnumerationTypeUpdateTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;

namespace Microsoft.OData.Client.E2E.Tests.EnumerationTypeTests.Server;

public class EnumerationTypeUpdateTestsController : ODataController
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
    [HttpGet("odata/GetDefaultColor()")]
    public IActionResult GetDefaultColor()
    {
        return Ok(Color.Red);
    }

    [HttpPost("odata/Products")]
    public IActionResult AddProduct([FromBody] Product product)
    {
        _dataSource.Products?.Add(product);
        return Created(product);
    }

    [HttpPatch("odata/Products({key})")]
    public IActionResult UpdateProduct([FromODataUri] int key, [FromBody] Delta<Product> delta)
    {
        var originalProduct = _dataSource.Products?.FirstOrDefault(p => p.ProductID == key);
        if (originalProduct == null)
        {
            return NotFound();
        }

        delta.Patch(originalProduct);
        return Updated(originalProduct);
    }
 
    [HttpDelete("odata/Products({key})")]
    public IActionResult DeleteProduct([FromODataUri] int key)
    {
        var product = _dataSource.Products?.FirstOrDefault(p => p.ProductID == key);
        if (product == null)
        {
            return NotFound();
        }

        _dataSource.Products?.Remove(product);
        return NoContent();
    }

    [HttpPost("odata/enumerationtypeupdatetests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = DefaultDataSource.CreateInstance();

        return Ok();
    }
}
