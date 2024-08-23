//-----------------------------------------------------------------------------
// <copyright file="JsonWithoutTypeResolverTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.ClientWithoutTypeResolverTests.Server
{
    public class JsonWithoutTypeResolverTestsController : ODataController
    {
        private static CommonEndToEndDataSource _dataSource;

        [EnableQuery]
        [HttpGet("odata/Customers")]
        public IActionResult GetCustomers()
        {
            var customers = _dataSource.Customers;
            return Ok(customers);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})")]
        public IActionResult GetCustomer([FromRoute] int key)
        {
            var customer = _dataSource.Customers.SingleOrDefault(a => a.CustomerId == key);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})/PrimaryContactInfo")]
        public IActionResult GetCustomerPrimaryContactInfo([FromRoute] int key)
        {
            var customerPrimaryContactInfo = _dataSource.Customers.SingleOrDefault(a => a.CustomerId == key);

            if (customerPrimaryContactInfo == null)
            {
                return NotFound();
            }

            return Ok(customerPrimaryContactInfo.PrimaryContactInfo);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})/PrimaryContactInfo/MobilePhoneBag")]
        public IActionResult GetCustomerPrimaryContactInfoPhoneBag([FromRoute] int key)
        {
            var customerPrimaryContactInfoPhoneBag = _dataSource.Customers.SingleOrDefault(a => a.CustomerId == key);

            if (customerPrimaryContactInfoPhoneBag == null)
            {
                return NotFound();
            }

            return Ok(customerPrimaryContactInfoPhoneBag.PrimaryContactInfo.MobilePhoneBag);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})/PrimaryContactInfo/ContactAlias")]
        public IActionResult GetCustomerPrimaryContactInfoContactAlias([FromRoute] int key)
        {
            var customerPrimaryContactInfoContactAlias = _dataSource.Customers.SingleOrDefault(a => a.CustomerId == key);

            if (customerPrimaryContactInfoContactAlias == null)
            {
                return NotFound();
            }

            return Ok(customerPrimaryContactInfoContactAlias.PrimaryContactInfo.ContactAlias);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})/PrimaryContactInfo/ContactAlias/AlternativeNames")]
        public IActionResult GetContactAliasAlternativeNames([FromRoute] int key)
        {
            var contactAliasAlternativeNames = _dataSource.Customers.SingleOrDefault(a => a.CustomerId == key);
            
            if (contactAliasAlternativeNames == null)
            {
                return NotFound();
            }

            return Ok(contactAliasAlternativeNames.PrimaryContactInfo.ContactAlias.AlternativeNames);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Customer/PrimaryContactInfo/ContactAlias/AlternativeNames")]
        public IActionResult GetDerivedCustomerAliasAlternativeNames([FromRoute] int key)
        {
            var contactAliasAlternativeNames = _dataSource.Customers.SingleOrDefault(a => a.CustomerId == key);

            if (contactAliasAlternativeNames == null)
            {
                return NotFound();
            }

            return Ok(contactAliasAlternativeNames.PrimaryContactInfo.ContactAlias.AlternativeNames);
        }

        [EnableQuery]
        [HttpGet("odata/Default.GetSpecificCustomer")]
        public IEnumerable<Customer> GetSpecificCustomer([FromODataUri] string name)
        {
            var customers = _dataSource.Customers.Where(a => a.Name == name);

            return customers;
        }

        [EnableQuery]
        [HttpGet("odata/PageViews")]
        public IActionResult Get()
        {
            var pageViews = _dataSource.PageViews;
            return Ok(pageViews);
        }

        [EnableQuery]
        [HttpGet("odata/PageViews/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.ProductPageView")]
        public IActionResult GetFromProductPageViews()
        {
            var productPageViews = _dataSource.PageViews.OfType<ProductPageView>().ToList();
            return Ok(productPageViews);
        }

        [EnableQuery]
        [HttpGet("odata/Products/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct")]
        public IActionResult GetFromProductDiscontinuedProducts()
        {
            var discontinuedProducts = _dataSource.Products.OfType<DiscontinuedProduct>().ToList();
            return Ok(discontinuedProducts);
        }

        [EnableQuery]
        [HttpGet("odata/Products({key})/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct/ProductId")]
        public IActionResult GetFromProductDiscontinuedProductId([FromRoute] int key)
        {
            var discontinuedProduct = _dataSource.Products.OfType<DiscontinuedProduct>().SingleOrDefault(a => a.ProductId == key);

            if (discontinuedProduct == null)
            {
                return NotFound();
            }

            return Ok(discontinuedProduct.ProductId);
        }

        [EnableQuery]
        [HttpGet("odata/MappedEntityTypes")]
        public IActionResult GetMappedEntityTypes()
        {
            var mappedEntityTypes = _dataSource.MappedEntityTypes;
            return Ok(mappedEntityTypes);
        }

        [EnableQuery]
        [HttpGet("odata/MappedEntityTypes({key})/BagOfDecimals")]
        public IActionResult GetMappedEntityTypeBagOfDecimals([FromRoute] int key)
        {
            var mappedEntityType = _dataSource.MappedEntityTypes.SingleOrDefault(a => a.Id == key);

            if (mappedEntityType == null)
            {
                return NotFound();
            }

            return Ok(mappedEntityType.BagOfDecimals);
        }

        [HttpPost("odata/jsonwithouttyperesolver/Default.ResetDataSource")]
        public IActionResult ResetDataSource()
        {
            _dataSource = CommonEndToEndDataSource.CreateInstance();

            return Ok();
        }
    }
}
