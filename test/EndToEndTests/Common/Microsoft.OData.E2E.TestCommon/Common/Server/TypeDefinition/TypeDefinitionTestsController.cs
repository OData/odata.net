//---------------------------------------------------------------------
// <copyright file="TypeDefinitionTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.TypeDefinition;

public class TypeDefinitionTestsController : ODataController
{
    private static TypeDefinitionDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/People")]
    public IActionResult GetPeople()
    {
        return Ok(_dataSource.People);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})")]
    public IActionResult GetPerson([FromODataUri] int key)
    {
        var person = _dataSource.People?.FirstOrDefault(p => p.PersonId == key);
        if (person == null)
        {
            return NotFound();
        }

        return Ok(person);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})/LastName")]
    public IActionResult GetPersonLastName([FromODataUri] int key)
    {
        var person = _dataSource.People?.FirstOrDefault(p => p.PersonId == key);
        if (person == null)
        {
            return NotFound();
        }

        return Ok(person.LastName);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})/LastName/$value")]
    public string? GetPersonLastNameValue([FromODataUri] int key)
    {
        var person = _dataSource.People?.FirstOrDefault(p => p.PersonId == key);
        return person?.LastName;
    }

    [EnableQuery]
    [HttpGet("odata/People({key})/Address")]
    public IActionResult GetPersonAddress([FromODataUri] int key)
    {
        var person = _dataSource.People?.FirstOrDefault(p => p.PersonId == key);
        if (person == null)
        {
            return NotFound();
        }

        return Ok(person.Address);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})/Descriptions")]
    public IActionResult GetPersonDescriptions([FromODataUri] int key)
    {
        var person = _dataSource.People?.FirstOrDefault(p => p.PersonId == key);
        if (person == null)
        {
            return NotFound();
        }

        return Ok(person.Descriptions);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})/Height")]
    public IActionResult GetPersonHeight([FromODataUri] int key)
    {
        var person = _dataSource.People?.FirstOrDefault(p => p.PersonId == key);
        if (person == null)
        {
            return NotFound();
        }

        return Ok(person.Height?.ToString());
    }

    [EnableQuery]
    [HttpGet("odata/People({key})/Microsoft.OData.E2E.TestCommon.Common.Server.TypeDefinition.GetFullName(nickname={nickname})")]
    public IActionResult GetFullName([FromODataUri] int key, [FromODataUri] string nickname)
    {
        var person = _dataSource.People?.FirstOrDefault(p => p.PersonId == key);
        if (person == null)
        {
            return NotFound();
        }

        return Ok(person.FirstName + " (" + nickname + ") " + person.LastName);
    }

    [EnableQuery]
    [HttpGet("odata/Products")]
    public IActionResult GetProducts()
    {
        return Ok(_dataSource.Products?.AsQueryable());
    }

    [EnableQuery]
    [HttpGet("odata/Products({key})")]
    public IActionResult GetProduct([FromODataUri] int key)
    {
        var product = _dataSource.Products?.FirstOrDefault(p => p.ProductId == key);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [EnableQuery]
    [HttpGet("odata/Products({key})/ProductId")]
    public IActionResult GetProductId([FromODataUri] int key)
    {
        var product = _dataSource.Products?.FirstOrDefault(p => p.ProductId == key);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product.ProductId);
    }

    [EnableQuery]
    [HttpGet("odata/Products({key})/Quantity")]
    public IActionResult GetProductQuantity([FromODataUri] int key)
    {
        var product = _dataSource.Products?.FirstOrDefault(p => p.ProductId == key);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product.Quantity);
    }

    [EnableQuery]
    [HttpGet("odata/Products({key})/Quantity/$value")]
    public UInt32? GetProductQuantityValue([FromODataUri] int key)
    {
        var product = _dataSource.Products?.FirstOrDefault(p => p.ProductId == key);
        return product?.Quantity;
    }

    [EnableQuery]
    [HttpGet("odata/Products({key})/LifeTimeInSeconds")]
    public IActionResult GetProductLifeTimeInSeconds([FromODataUri] int key)
    {
        var product = _dataSource.Products?.FirstOrDefault(p => p.ProductId == key);
        if (product == null)
        {
            return NotFound();
        }

        return Ok((Decimal?)product.LifeTimeInSeconds);
    }

    [EnableQuery]
    [HttpGet("odata/Products({key})/Microsoft.OData.E2E.TestCommon.Common.Server.TypeDefinition.ExtendLifeTime")]
    public IActionResult GetProductLifeTimeInSecondsUsingExtendLifeTime([FromODataUri] int key)
    {
        var product = _dataSource.Products?.FirstOrDefault(p => p.ProductId == key);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product.LifeTimeInSeconds);
    }

    [EnableQuery]
    [HttpGet("odata/Products({key})/TheCombo")]
    public IActionResult GetProductTheCombo([FromODataUri] int key)
    {
        var product = _dataSource.Products?.FirstOrDefault(p => p.ProductId == key);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product.TheCombo);
    }

    [EnableQuery]
    [HttpGet("odata/Products({key})/LargeNumbers")]
    public IActionResult GetProductLargeNumbers([FromODataUri] int key)
    {
        var product = _dataSource.Products?.FirstOrDefault(p => p.ProductId == key);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product.LargeNumbers);
    }

    [EnableQuery]
    [HttpGet("odata/Products({key})/Temperature")]
    public IActionResult GetProductTemperature([FromODataUri] int key)
    {
        var product = _dataSource.Products?.FirstOrDefault(p => p.ProductId == key);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product.Temperature?.ToString());
    }

    [EnableQuery]
    [HttpPost("odata/Products({key})/Microsoft.OData.E2E.TestCommon.Common.Server.TypeDefinition.ExtendLifeTime")]
    public IActionResult ExtendLifeTime([FromODataUri] int key, [FromBody] ExtendLifeTimeParameter seconds)
    {
        var product = _dataSource.Products?.FirstOrDefault(p => p.ProductId == key);
        if (product == null)
        {
            return NotFound();
        }

        product.LifeTimeInSeconds += seconds.Seconds;
        return Ok((UInt64)product.LifeTimeInSeconds);
    }

    [HttpPost("odata/People")]
    public IActionResult AddPerson([FromBody] Person person)
    {
        _dataSource.People?.Add(person);
        return Created(person);
    }

    [HttpPost("odata/Products")]
    public IActionResult AddProduct([FromBody] Product product)
    {
        _dataSource.Products?.Add(product);
        return Created(product);
    }


    [HttpPost("odata/typedefinitiontests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = TypeDefinitionDataSource.CreateInstance();

        return Ok();
    }

    public class ExtendLifeTimeParameter
    {
        public UInt64 Seconds { get; set; }
    }
}
