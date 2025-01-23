//---------------------------------------------------------------------
// <copyright file="KeyAsSegmentTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Server;

public class KeyAsSegmentTestsController : ODataController
{
    private static CommonEndToEndDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/People")]
    public IActionResult GetPeople()
    {
        var people = _dataSource.People;
        return Ok(people);
    }

    [EnableQuery]
    [HttpGet("odata/People/$/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee")]
    public IActionResult GetPeopleOfTypeEmployee()
    {
        var people = _dataSource.People?.OfType<Employee>();
        return Ok(people);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})")]
    public IActionResult GetPerson([FromRoute] int key)
    {
        var person = _dataSource.People?.SingleOrDefault(a => a.PersonId == key);
        if (person == null)
        {
            return NotFound();
        }
        return Ok(person);
    }

    [EnableQuery]
    [HttpGet("odata/People/{key}")]
    public IActionResult GetPersonById([FromRoute] int key)
    {
        var person = _dataSource.People?.SingleOrDefault(a => a.PersonId == key);
        if (person == null)
        {
            return NotFound();
        }
        return Ok(person);
    }

    [EnableQuery]
    [HttpGet("odata/People/{key}/PersonMetadata")]
    public IActionResult GetPersonMetadatada([FromRoute] int key)
    {
        var person = _dataSource.People?.SingleOrDefault(a => a.PersonId == key);
        if (person == null)
        {
            return NotFound();
        }
        return Ok(person.PersonMetadata);
    }

    [EnableQuery]
    [HttpGet("odata/Products/$/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct/{key}/RelatedProducts/$/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct/{relatedKey}/Photos")]
    public IActionResult GetProductRelatedPhotos([FromRoute] int key, [FromRoute] int relatedKey)
    {
        var product = _dataSource.Products?.SingleOrDefault(a => a.ProductId == key);
        if (product == null)
        {
            return NotFound();
        }

        var relatedProduct = product.RelatedProducts?.SingleOrDefault(a => a.ProductId == relatedKey);
        if (relatedProduct == null)
        {
            return NotFound();
        }

        return Ok(relatedProduct.Photos);
    }

    [EnableQuery]
    [HttpGet("odata/Orders({key})")]
    public IActionResult GetProduct([FromRoute] int key)
    {
        var order = _dataSource.Orders?.SingleOrDefault(a => a.OrderId == key);
        if (order == null)
        {
            return NotFound();
        }
        return Ok(order);
    }

    [EnableQuery]
    [HttpGet("odata/Customers")]
    public IActionResult GetCustomers()
    {
        var customers = _dataSource.Customers;
        return Ok(customers);
    }

    [EnableQuery]
    [HttpGet("odata/Customers/{key}")]
    public IActionResult GetCustomer([FromODataUri] int key)
    {
        var customer = _dataSource.Customers?.SingleOrDefault(c => c.CustomerId == key);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [EnableQuery]
    [HttpGet("odata/CustomerInfos")]
    public IActionResult GetCustomerInfos()
    {
        var customerInfos = _dataSource.CustomerInfos;
        return Ok(customerInfos);
    }

    [EnableQuery]
    [HttpGet("odata/CustomerInfos({key})/$value")]
    public IActionResult GetCustomerInfosValue([FromODataUri] int key)
    {
        var customerInfo = _dataSource.CustomerInfos?.SingleOrDefault(c => c.CustomerInfoId == key);
        if (customerInfo == null)
        {
            return NotFound();
        }

        return Ok(customerInfo);
    }

    [EnableQuery]
    [HttpGet("odata/Cars")]
    public IActionResult GetCars()
    {
        var cars = _dataSource.Cars;
        return Ok(cars);
    }

    [EnableQuery]
    [HttpGet("odata/Cars({key})/Photo")]
    public IActionResult GetCarPhoto([FromODataUri] int key)
    {
        var car = _dataSource.Cars?.SingleOrDefault(c => c.VIN == key);
        if (car == null)
        {
            return NotFound();
        }

        car.Photo = new MemoryStream([0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f]);

        return Ok(car.Photo);
    }

    [HttpPut("odata/Cars({key})/Video")]
    public IActionResult AddVideoToCar([FromODataUri] int key)
    {
        var car = _dataSource.Cars?.SingleOrDefault(c => c.VIN == key);
        if (car == null)
        {
            return NotFound();
        }

        var video = Request.Body;
        car.Video = video;
        return Updated(car);
    }

    [EnableQuery]
    [HttpPut("odata/CustomerInfos({key})/$value")]
    public IActionResult AddStreamValueToCustomerInfo([FromODataUri] int key)
    {
        var customerInfo = _dataSource.CustomerInfos?.SingleOrDefault(c => c.CustomerInfoId == key);
        if (customerInfo == null)
        {
            return NotFound();
        }

        var stream = Request.Body;

        return Updated(customerInfo);
    }

    [HttpPost("odata/keyassegmenttests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = CommonEndToEndDataSource.CreateInstance();

        return Ok();
    }
}
