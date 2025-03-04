//-----------------------------------------------------------------------------
// <copyright file="ChangeTrackingOptionsTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.PropertyTrackingTests;

public class ChangeTrackingOptionsTestsController : ODataController
{
    private static DefaultDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/People")]
    public IActionResult GetPeople()
    {
        var people = _dataSource.People;
        return Ok(people);
    }

    [EnableQuery]
    [HttpGet("odata/People({key})")]
    public IActionResult GetPerson([FromRoute] int key)
    {
        var person = _dataSource.People?.SingleOrDefault(a => a.PersonID == key);
        if (person == null)
        {
            return NotFound();
        }

        return Ok(person);
    }

    [EnableQuery]
    [HttpGet("odata/Boss")]
    public IActionResult GetBoss()
    {
        var boss = _dataSource.Boss;
        return Ok(boss);
    }

    [EnableQuery]
    [HttpGet("odata/Customers")]
    public IActionResult GetCustomers()
    {
        var customers = _dataSource.Customers;
        return Ok(customers);
    }

    [EnableQuery]
    [HttpGet("odata/Accounts")]
    public IActionResult GetAccounts()
    {
        var accounts = _dataSource.Accounts;
        return Ok(accounts);
    }

    [EnableQuery]
    [HttpGet("odata/Orders")]
    public IActionResult GetOrders()
    {
        var orders = _dataSource.Orders;
        return Ok(orders);
    }

    [EnableQuery]
    [HttpGet("odata/Orders({key})")]
    public IActionResult GetOrder([FromRoute] int key)
    {
        var order = _dataSource.Orders?.SingleOrDefault(a => a.OrderID == key);
        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [EnableQuery]
    [HttpGet("odata/OrderDetails")]
    public IActionResult GetOrderDetails()
    {
        var orderDetails = _dataSource.OrderDetails;
        return Ok(orderDetails);
    }

    [EnableQuery]
    [HttpGet("odata/PublicCompany")]
    public IActionResult GetPublicCompany()
    {
        var company = _dataSource.PublicCompany;
        return Ok(company);
    }

    [EnableQuery]
    [HttpGet("odata/Products")]
    public IActionResult GetProducts()
    {
        var products = _dataSource.Products;
        return Ok(products);
    }

    [EnableQuery]
    [HttpGet("odata/Customers({key})/Orders")]
    public IActionResult GetCustomerOrders([FromRoute] int key)
    {
        var customer = _dataSource.Customers?.SingleOrDefault(c => c.PersonID == key);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer.Orders);
    }

    [EnableQuery]
    [HttpGet("odata/Accounts({key})/MyGiftCard")]
    public IActionResult GetAccountMyGiftCard([FromRoute] int key)
    {
        var account = _dataSource.Accounts?.SingleOrDefault(a => a.AccountID == key);
        if (account == null)
        {
            return NotFound();
        }

        return Ok(account.MyGiftCard);
    }

    [HttpPost("odata/People")]
    public IActionResult AddPerson([FromBody] Person person)
    {
        _dataSource.People?.Add(person);
        return Created(person);
    }

    [HttpPost("odata/Customers")]
    public IActionResult AddCustomer([FromBody] Customer person)
    {
        _dataSource.Customers?.Add(person);
        return Created(person);
    }

    [HttpPost("odata/Orders")]
    public IActionResult AddOrder([FromBody] Order order)
    {
        _dataSource.Orders?.Add(order);
        return Created(order);
    }

    [HttpPost("odata/PublicCompany")]
    public IActionResult AddPublicCompany([FromBody] PublicCompany publicCompany)
    {
        return Created(publicCompany);
    }

    [HttpPost("odata/Accounts")]
    public IActionResult AddAccount([FromBody] Account account)
    {
        _dataSource.Accounts?.Add(account);
        return Created(account);
    }

    [HttpPost("odata/Products")]
    public IActionResult AddProduct([FromBody] Product product)
    {
        _dataSource.Products?.Add(product);
        return Created(product);
    }

    [HttpPost("odata/Customers({key})/Orders/$ref")]
    public IActionResult AddOrderRefToCustomer([FromRoute] int key, [FromBody] Uri orderUri)
    {
        if (orderUri == null)
        {
            return BadRequest();
        }

        // Extract the ID from the URI
        var lastSegment = orderUri.Segments.Last();
        int startIndex = lastSegment.IndexOf('(') + 1;
        int endIndex = lastSegment.IndexOf(')') - 1;
        var orderId = int.Parse(Uri.UnescapeDataString(lastSegment.Substring(startIndex, endIndex - startIndex + 1)));

        // Find the order by ID
        var order = _dataSource.Orders?.SingleOrDefault(d => d.OrderID == orderId);
        if (order == null)
        {
            return NotFound();
        }

        var customer = _dataSource.Customers?.SingleOrDefault(c => c.PersonID == key);
        if (customer == null)
        {
            return NotFound();
        }

        customer.Orders ??= new List<Order>();
        customer.Orders.Add(order);

        return Updated(order);
    }

    [HttpPut("odata/Orders({key})")]
    public IActionResult UpdateOrder([FromRoute] int key, [FromBody] Order order)
    {
        var orderToUpdate = _dataSource.Orders?.SingleOrDefault(a => a.OrderID == key);
        if (orderToUpdate == null)
        {
            return NotFound();
        }

        orderToUpdate.LoggedInEmployee = order.LoggedInEmployee ?? orderToUpdate.LoggedInEmployee;
        orderToUpdate.CustomerForOrder = order.CustomerForOrder ?? orderToUpdate.CustomerForOrder;
        orderToUpdate.ShipDate = order.ShipDate;
        orderToUpdate.ShipTime = order.ShipTime;
        orderToUpdate.InfoFromCustomer = order.InfoFromCustomer ?? orderToUpdate.InfoFromCustomer;
        orderToUpdate.OrderDetails = order.OrderDetails ?? orderToUpdate.OrderDetails;
        orderToUpdate.ShelfLife = order.ShelfLife ?? orderToUpdate.ShelfLife;
        orderToUpdate.OrderDate = order.OrderDate;
        orderToUpdate.OrderShelfLifes = order.OrderShelfLifes ?? orderToUpdate.OrderShelfLifes;
        orderToUpdate.UpdatedTime = DateTime.UtcNow;

        return Updated(orderToUpdate);
    }

    [HttpPut("odata/Boss")]
    public IActionResult PutBoss([FromBody] Person person)
    {
        var boss = _dataSource.Boss;
        if (boss == null)
        {
            return NotFound();
        }

        boss.FirstName = person.FirstName ?? boss.FirstName;
        boss.LastName = person.LastName ?? boss.FirstName;
        boss.MiddleName = person.MiddleName ?? boss.MiddleName;
        boss.HomeAddress = person.HomeAddress ?? boss.HomeAddress;
        boss.Emails = person.Emails ?? boss.Emails;
        boss.Numbers = person.Numbers ?? boss.Numbers;
        boss.Parent = person.Parent ?? boss.Parent;
        boss.Home = person.Home ?? boss.Home;
        boss.UpdatedTime = DateTime.UtcNow;

        return Updated(boss);
    }

    [HttpPut("odata/Boss/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer")]
    public IActionResult PutBossCustomer([FromBody] Customer customer)
    {
        var boss = _dataSource.Boss as Customer;
        if (boss == null)
        {
            return NotFound();
        }

        boss.FirstName = customer.FirstName ?? boss.FirstName;
        boss.LastName = customer.LastName ?? boss.FirstName;
        boss.MiddleName = customer.MiddleName ?? boss.MiddleName;
        boss.HomeAddress = customer.HomeAddress ?? boss.HomeAddress;
        boss.Emails = customer.Emails ?? boss.Emails;
        boss.Numbers = customer.Numbers ?? boss.Numbers;
        boss.Parent = customer.Parent ?? boss.Parent;
        boss.Home = customer.Home ?? boss.Home;
        boss.UpdatedTime = DateTime.UtcNow;

        return Updated(boss);
    }

    [HttpPut("odata/People({key})/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer")]
    public IActionResult PutPerson([FromRoute] int key, [FromBody] Customer customer)
    {
        var personToUpdate = _dataSource.People?.SingleOrDefault(a => a.PersonID == key) as Customer;
        if (personToUpdate == null)
        {
            return NotFound();
        }

        personToUpdate.FirstName = customer.FirstName ?? personToUpdate.FirstName;
        personToUpdate.LastName = customer.LastName ?? personToUpdate.FirstName;
        personToUpdate.MiddleName = customer.MiddleName ?? personToUpdate.MiddleName;
        personToUpdate.HomeAddress = customer.HomeAddress ?? personToUpdate.HomeAddress;
        personToUpdate.Emails = customer.Emails ?? personToUpdate.Emails;
        personToUpdate.Numbers = customer.Numbers ?? personToUpdate.Numbers;
        personToUpdate.Parent = customer.Parent ?? personToUpdate.Parent;
        personToUpdate.Home = customer.Home ?? personToUpdate.Home;
        personToUpdate.City = customer.City ?? personToUpdate.City; 
        personToUpdate.Company = customer.Company ?? personToUpdate.Company;
        personToUpdate.UpdatedTime = DateTime.UtcNow;

        return Updated(personToUpdate);
    }

    [HttpPut("odata/OrderDetails(OrderID={orderID},ProductID={productID})")]
    public IActionResult PatchOrderDetail([FromODataUri] int orderID, [FromODataUri] int productID, [FromBody] OrderDetail OrderDetail)
    {
        var orderDetailToUpdate = _dataSource.OrderDetails?.SingleOrDefault(a => a.OrderID == orderID && a.ProductID == productID);
        if (orderDetailToUpdate == null)
        {
            return NotFound();
        }

        orderDetailToUpdate.Quantity = OrderDetail.Quantity > 0 ? OrderDetail.Quantity : orderDetailToUpdate.Quantity;
        orderDetailToUpdate.UnitPrice = OrderDetail.UnitPrice > 0 ? OrderDetail.UnitPrice : orderDetailToUpdate.UnitPrice;
        orderDetailToUpdate.OrderPlaced = OrderDetail.OrderPlaced;
        orderDetailToUpdate.UpdatedTime = DateTime.UtcNow;

        return Updated(orderDetailToUpdate);
    }

    [HttpPatch("odata/OrderDetails(OrderID={orderID},ProductID={productID})")]
    public IActionResult PatchOrderDetail([FromODataUri] int orderID, [FromODataUri] int productID, [FromBody] Delta<OrderDetail> delta)
    {
        var orderDetail = _dataSource.OrderDetails?.SingleOrDefault(a => a.OrderID == orderID && a.ProductID == productID);
        if (orderDetail == null)
        {
            return NotFound();
        }

        var updatedOrderDetail = delta.Patch(orderDetail);
        return Updated(updatedOrderDetail);
    }

    [HttpPatch("odata/Orders({key})")]
    public IActionResult PatchOrder([FromRoute] int key, [FromBody] Delta<Order> delta)
    {
        var order = _dataSource.Orders?.SingleOrDefault(a => a.OrderID == key);
        if (order == null)
        {
            return NotFound();
        }

        var updatedOrder = delta.Patch(order);
        return Updated(updatedOrder);
    }

    [HttpPatch("odata/Products({key})")]
    public IActionResult PatchProduct([FromRoute] int key, [FromBody] Delta<Product> delta)
    {
        var product = _dataSource.Products?.SingleOrDefault(a => a.ProductID == key);
        if (product == null)
        {
            return NotFound();
        }

        var updatedProduct = delta.Patch(product);
        return Updated(updatedProduct);
    }

    [HttpPatch("odata/Boss")]
    public IActionResult PatchBoss([FromBody] Delta<Person> delta)
    {
        var boss = _dataSource.Boss;
        if (boss == null)
        {
            return NotFound();
        }

        var updatedBoss = delta.Patch(boss);
        return Updated(updatedBoss);
    }

    [HttpPatch("odata/Boss/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer")]
    public IActionResult PatchBossCustomer([FromBody] Delta<Customer> delta)
    {
        var boss = _dataSource.Boss as Customer;
        if (boss == null)
        {
            return NotFound();
        }

        var updatedBoss = delta.Patch(boss);
        return Updated(updatedBoss);
    }

    [HttpPatch("odata/Accounts({key})/MyGiftCard")]
    public IActionResult PatchAccountGiftCard([FromRoute] int key, [FromBody] Delta<GiftCard> delta)
    {
        var account = _dataSource.Accounts?.SingleOrDefault(a => a.AccountID == key);
        if (account == null)
        {
            return NotFound();
        }

        if(account.MyGiftCard == null)
        {
            account.MyGiftCard = new GiftCard();
        }

        var updatedGiftCard = delta.Patch(account.MyGiftCard);
        return Updated(updatedGiftCard);
    }

    [HttpPatch("odata/People({key})/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer")]
    public IActionResult PatchPerson([FromRoute] int key, [FromBody] Delta<Customer> delta)
    {
        var person = _dataSource.People?.SingleOrDefault(a => a.PersonID == key) as Customer;
        if (person == null)
        {
            return NotFound();
        }

        var updatedCustomer = delta.Patch(person);
        return Updated(updatedCustomer);
    }

    [HttpPatch("odata/PublicCompany/Microsoft.OData.E2E.TestCommon.Common.Server.Default.PublicCompany")]
    public IActionResult PatchPublicCompany([FromBody] Delta<PublicCompany> delta)
    {
        var company = _dataSource.PublicCompany as PublicCompany;
        if (company == null)
        {
            return NotFound();
        }

        var updatedCompany = delta.Patch(company);
        return Updated(updatedCompany);
    }

    [HttpPatch("odata/Accounts({key})")]
    public IActionResult PatchAccounts([FromRoute] int key, [FromBody] Delta<Account> delta)
    {
        var account = _dataSource.Accounts?.SingleOrDefault(a => a.AccountID == key);
        if (account == null)
        {
            return NotFound();
        }

        var updatedAccount = delta.Patch(account);
        return Updated(updatedAccount);
    }

    [HttpPatch("odata/Customers({key})")]
    public IActionResult PatchCustomers([FromRoute] int key, [FromBody] Delta<Customer> delta)
    {
        var customer = _dataSource.Customers?.SingleOrDefault(a => a.PersonID == key);
        if (customer == null)
        {
            return NotFound();
        }

        var updatedCustomer = delta.Patch(customer);
        return Updated(updatedCustomer);
    }

    [HttpPost("odata/changetrackingoptionstests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = DefaultDataSource.CreateInstance();

        return Ok();
    }
}
