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
        [EnableQuery]
        [HttpGet("odata/Customers")]
        public IActionResult GetCustomers()
        {
            var customers = CommonEndToEndDataSource.Customers;
            return Ok(customers);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})")]
        public IActionResult GetCustomer([FromRoute]int key)
        {
            var customer = CommonEndToEndDataSource.Customers.SingleOrDefault(a=>a.CustomerId == key);
            return Ok(customer);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})/PrimaryContactInfo")]
        public IActionResult GetCustomerPrimaryContactInfo([FromRoute] int key)
        {
            var customerPrimaryContactInfo = CommonEndToEndDataSource.Customers.SingleOrDefault(a => a.CustomerId == key).PrimaryContactInfo;
            return Ok(customerPrimaryContactInfo);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})/PrimaryContactInfo/MobilePhoneBag")]
        public IActionResult GetCustomerPrimaryContactInfoPhoneBag([FromRoute] int key)
        {
            var customerPrimaryContactInfoPhoneBag = CommonEndToEndDataSource.Customers.SingleOrDefault(a => a.CustomerId == key).PrimaryContactInfo.MobilePhoneBag;
            return Ok(customerPrimaryContactInfoPhoneBag);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})/PrimaryContactInfo/ContactAlias")]
        public IActionResult GetCustomerPrimaryContactInfoContactAlias([FromRoute] int key)
        {
            var customerPrimaryContactInfoContactAlias = CommonEndToEndDataSource.Customers.SingleOrDefault(a => a.CustomerId == key).PrimaryContactInfo.ContactAlias;
            return Ok(customerPrimaryContactInfoContactAlias);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})/PrimaryContactInfo/ContactAlias/AlternativeNames")]
        public IActionResult GetContactAliasAlternativeNames([FromRoute] int key)
        {
            var contactAliasAlternativeNames = CommonEndToEndDataSource.Customers.SingleOrDefault(a => a.CustomerId == key).PrimaryContactInfo.ContactAlias.AlternativeNames;
            return Ok(contactAliasAlternativeNames);
        }

        [EnableQuery]
        [HttpGet("odata/Customers({key})/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Customer/PrimaryContactInfo/ContactAlias/AlternativeNames")]
        public IActionResult GetDerivedCustomerAliasAlternativeNames([FromRoute] int key)
        {
            var contactAliasAlternativeNames = CommonEndToEndDataSource.Customers.SingleOrDefault(a => a.CustomerId == key).PrimaryContactInfo.ContactAlias.AlternativeNames;
            return Ok(contactAliasAlternativeNames);
        }

        [EnableQuery]
        [HttpGet("odata/Default.GetSpecificCustomer()")]
        public IActionResult GetSpecificCustomer([FromODataUri] string name)
        {
            //string name = (string)parameters["Name"];
            var customer = CommonEndToEndDataSource.Customers.SingleOrDefault(a => a.Name == name);
            return Ok(customer);
        }

        [EnableQuery]
        [HttpGet("odata/PageViews")]
        public IActionResult Get()
        {
            var pageViews = CommonEndToEndDataSource.PageViews;
            return Ok(pageViews);
        }

        [EnableQuery]
        [HttpGet("odata/PageViews/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.ProductPageView")]
        public IActionResult GetFromProductPageViews()
        {
            var productPageViews = CommonEndToEndDataSource.PageViews.OfType<ProductPageView>().ToList();
            return Ok(productPageViews);
        }

        [EnableQuery]
        [HttpGet("odata/Products/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct")]
        public IActionResult GetFromProductDiscontinuedProducts()
        {
            var discontinuedProducts = CommonEndToEndDataSource.Products.OfType<DiscontinuedProduct>().ToList();
            return Ok(discontinuedProducts);
        }

        [EnableQuery]
        [HttpGet("odata/Products({key})/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct/ProductId")]
        public IActionResult GetFromProductDiscontinuedProductId([FromRoute] int key)
        {
            var discontinuedProductId = CommonEndToEndDataSource.Products.OfType<DiscontinuedProduct>().SingleOrDefault(a=>a.ProductId==key).ProductId;
            return Ok(discontinuedProductId);
        }

        [EnableQuery]
        [HttpGet("odata/MappedEntityTypes")]
        public IActionResult GetMappedEntityTypes()
        {
            var mappedEntityTypes = CommonEndToEndDataSource.MappedEntityTypes;
            return Ok(mappedEntityTypes);
        }

        [EnableQuery]
        [HttpGet("odata/MappedEntityTypes({key})/BagOfDecimals")]
        public IActionResult GetMappedEntityTypeBagOfDecimals([FromRoute] int key)
        {
            var bagOfDecimals = CommonEndToEndDataSource.MappedEntityTypes.SingleOrDefault(a=>a.Id == key).BagOfDecimals;
            return Ok(bagOfDecimals);
        }
    }
}
