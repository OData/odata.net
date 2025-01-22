//---------------------------------------------------------------------
// <copyright file="KeyAsSegmentTestsController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
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
    [HttpGet("odata/People/{key}/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee/$/Salary")]
    public IActionResult GetEmployeeSalary([FromRoute] int key)
    {
        var employee = _dataSource.People?.OfType<Employee>().SingleOrDefault(a => a.PersonId == key);
        if (employee == null)
        {
            return NotFound();
        }
        return Ok(employee.Salary);
    }

    [EnableQuery]
    [HttpGet("odata/Orders")]
    public IActionResult GetOrders()
    {
        var orders = _dataSource.Orders;
        return Ok(orders);
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
    [HttpGet("odata/Products/$/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct")]
    public IActionResult GetDiscontinuedProducts()
    {
        var discontinuedProducts = _dataSource.Products?.OfType<DiscontinuedProduct>();

        return Ok(discontinuedProducts);
    }

    [EnableQuery]
    [HttpGet("odata/Products/$/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct/$")]
    public IActionResult GetAnotherDiscontinuedProducts()
    {
        var discontinuedProducts = _dataSource.Products?.OfType<DiscontinuedProduct>();

        return Ok(discontinuedProducts);
    }

    [EnableQuery]
    [HttpGet("odata/Products/$/$/$/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct")]
    public IActionResult GetDiscontinuedProductsMultipleDollarSegments()
    {
        var discontinuedProducts = _dataSource.Products?.OfType<DiscontinuedProduct>();

        return Ok(discontinuedProducts);
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
    [HttpGet("odata/Login")]
    public IActionResult GetLogin()
    {
        var login = _dataSource.Logins;
        return Ok(login);
    }

    [EnableQuery]
    [HttpGet("odata/Login/{key}/Customer")]
    public IActionResult GetLoginCustomers([FromODataUri] string key)
    {
        var login = _dataSource.Logins?.SingleOrDefault(l => l.Username == key);
        if (login == null)
        {
            return NotFound();
        }

        return Ok(login.Customer);
    }

    [EnableQuery]
    [HttpGet("odata/CustomerInfos")]
    public IActionResult GetCustomerInfos()
    {
        var customerInfos = _dataSource.CustomerInfos;
        return Ok(customerInfos);
    }

    [EnableQuery]
    [HttpGet("odata/Cars")]
    public IActionResult GetCars()
    {
        var cars = _dataSource.Cars;
        return Ok(cars);
    }

    [HttpPost("odata/People/$/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee/$/Default.IncreaseSalaries")]
    public IActionResult IncreaseSalaries([FromODataBody] int n)
    {
        _dataSource.People?.OfType<Employee>().ToList().ForEach(e => e.Salary += n);
        return Ok();
    }

    [HttpPost("odata/People/$/$/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee/$/$/Default.IncreaseSalaries")]
    public IActionResult IncreaseSalariesMultipleDollarSegments([FromODataBody] int n)
    {
        _dataSource.People?.OfType<Employee>().ToList().ForEach(e => e.Salary += n);
        return Ok();
    }

    [HttpPost("odata/People/$/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee/$/Default.IncreaseSalaries/$")]
    public IActionResult IncreaseSalariesDollarSegmentAtUriEnd([FromODataBody] int n)
    {
        _dataSource.People?.OfType<Employee>().ToList().ForEach(e => e.Salary += n);
        return Ok();
    }

    [HttpPost("odata/People/$/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee/{key}/$/Default.Sack")]
    public IActionResult Sack([FromODataUri] int key)
    {
        var employee = _dataSource.People?.OfType<Employee>().SingleOrDefault(a => a.PersonId == key);
        if (employee == null)
        {
            return NotFound();
        }

        _dataSource.People?.Remove(employee);
        return NoContent();
    }

    [HttpPost("odata/People")]
    public IActionResult AddPerson([FromBody] Person person)
    {
        _dataSource.People?.Add(person);
        return Created(person);
    }

    [HttpPost("odata/Customers")]
    public IActionResult AddCustomer([FromBody] Customer customer)
    {
        _dataSource.Customers?.Add(customer);
        return Created(customer);
    }

    [HttpPost("odata/Customers({key})/Orders")]
    public IActionResult AddOrderToCustomer([FromODataUri] int key, [FromBody] Order order)
    {
        var customer = _dataSource.Customers?.SingleOrDefault(c => c.CustomerId == key);
        if (customer == null)
        {
            return NotFound();
        }

        customer.Orders ??= [];
        customer.Orders.Add(order);
        return Created(order);
    }

    [HttpPost("odata/Login")]
    public IActionResult PostLogin([FromBody] Login login)
    {
        _dataSource.Logins?.Add(login);
        return Created(login);
    }

    [HttpPost("odata/Customers({key})/Logins/$ref")]
    public IActionResult AddLoginsRefToCustomer([FromODataUri] int key, [FromBody] Uri loginUri)
    {
        if (loginUri == null)
        {
            return BadRequest();
        }

        // Extract the login ID from the URI
        var lastSegment = loginUri.Segments.Last();
        int startIndex = lastSegment.IndexOf("('") + 2; // +2 to skip the opening parenthesis and the single quote
        int endIndex = lastSegment.IndexOf("')") - 1; // -1 to skip the closing single quote

        var loginUserName = Uri.UnescapeDataString(lastSegment.Substring(startIndex, endIndex - startIndex + 1));

        // Find the login by ID
        var login = _dataSource.Logins?.SingleOrDefault(d => d.Username == loginUserName);
        if (login == null)
        {
            return NotFound();
        }

        var customer = _dataSource.Customers?.SingleOrDefault(c => c.CustomerId == key);
        if (customer == null)
        {
            return NotFound();
        }

        customer.Logins ??= [];
        customer.Logins.Add(login); 

        return Ok(login);
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

    [HttpPatch("odata/People/{key}")]
    public IActionResult PatchPerson([FromRoute] int key, [FromBody] Delta<Person> patch)
    {
        var person = _dataSource.People?.SingleOrDefault(a => a.PersonId == key);
        if (person == null)
        {
            return NotFound();
        }
        patch.Patch(person);
        return Updated(person);
    }

    [HttpDelete("odata/People({key})/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.SpecialEmployee")]
    public IActionResult DeleteSpecialEmployee([FromRoute] int key)
    {
        var person = _dataSource.People?.SingleOrDefault(a => a.PersonId == key);
        if (person == null)
        {
            return NotFound();
        }

        _dataSource.People?.Remove(person);
        return NoContent();
    }

    [HttpPost("odata/keyassegmenttests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = CommonEndToEndDataSource.CreateInstance();

        return Ok();
    }
}
