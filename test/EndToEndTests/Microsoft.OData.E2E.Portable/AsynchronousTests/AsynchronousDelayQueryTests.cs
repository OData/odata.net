//---------------------------------------------------------------------
// <copyright file="AsynchronousDelayQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Client;
using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReferencePlus;
using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
using Microsoft.Test.OData.Services.TestServices;
using Xunit;

namespace Microsoft.OData.Profile111.Tests.AsynchronousTests
{
    public class AsynchronousDelayQueryTests : EndToEndTestBase
    {
        InMemoryEntities testClientContext;

        public AsynchronousDelayQueryTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        [Fact]
        public async Task DelayQueryOnEntitySetNet35()
        {
            this.testClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            this.testClientContext.MergeOption = MergeOption.OverwriteChanges;

            //Post a Product
            var product = Product.CreateProduct(10001, "10001", "2", 2.0f, 2, true);
            this.testClientContext.AddToProducts(product);
            await this.testClientContext.SaveChangesAsync();

            //Action Bound on EntitySet and return an EntitySet
            var discountAction = this.testClientContext.Products.Discount(50);
            Assert.True(discountAction.RequestUri.OriginalString.EndsWith("/Products/Microsoft.Test.OData.Services.ODataWCFService.Discount"));
            var ar = discountAction.BeginExecute(null, null).EnqueueWait(this);
            discountAction.EndExecute(ar);

            //Query an Entity
            var queryProduct = this.testClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } });
            Assert.True(queryProduct.RequestUri.OriginalString.EndsWith("/Products(10001)"));
            queryProduct.BeginGetValue(
                ar1 =>
                {
                    product = queryProduct.EndGetValue(ar1);
                    Assert.Equal(1, product.UnitPrice);
                }, queryProduct).EnqueueWait(this);

            //Action Bound on Entity and return an Enum
            var expectedAccessLevel = AccessLevel.ReadWrite | AccessLevel.Execute;
            var accessLevelAction = this.testClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } }).AddAccessRight(expectedAccessLevel);
            Assert.True(accessLevelAction.RequestUri.OriginalString.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.AddAccessRight"));
            var ar2 = accessLevelAction.BeginGetValue(null, null).EnqueueWait(this);
            var accessLevel = accessLevelAction.EndGetValue(ar2);
            Assert.True(accessLevel.Value.HasFlag(expectedAccessLevel));

            //Function Bound on Entity and return Collection of Entity
            //Won't execute since ODL doesn't support it now.
            var getProductDetailsFunction = this.testClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } }).GetProductDetails(1);
            var getRelatedProductAction = getProductDetailsFunction.ByKey(new Dictionary<string, object> { { "ProductID", 10001 }, { "ProductDetailID", 10001 } }).GetRelatedProduct();
            Assert.True(getProductDetailsFunction.RequestUri.OriginalString.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=1)"));
            Assert.True(getRelatedProductAction.RequestUri.OriginalString.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=1)(ProductID=10001,ProductDetailID=10001)/Microsoft.Test.OData.Services.ODataWCFService.GetRelatedProduct()"));

            var ar3 = getProductDetailsFunction.BeginExecute(null, null).EnqueueWait(this);
            var productDetails = getProductDetailsFunction.EndExecute(ar3);
            foreach (var pd in productDetails)
            {
                //Check whether GetEnumerator works
                Assert.Equal(5, pd.ProductID);
            }
        }

        [Fact]
        public async Task DelayQueryOnEntitySetNet45()
        {
            this.testClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            this.testClientContext.MergeOption = MergeOption.OverwriteChanges;
            
            //Post an Product
            var product = Product.CreateProduct(10001, "10001", "2", 2.0f, 2, true);
            this.testClientContext.AddToProducts(product);
            await this.testClientContext.SaveChangesAsync();

            //Action Bound on EntitySet
            var discountAction = this.testClientContext.Products.Discount(50);
            Assert.True(discountAction.RequestUri.OriginalString.EndsWith("/Products/Microsoft.Test.OData.Services.ODataWCFService.Discount"));
            await discountAction.ExecuteAsync();

            //ByKey
            var queryProduct = this.testClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } });
            Assert.True(queryProduct.RequestUri.OriginalString.EndsWith("/Products(10001)"));
            product = await queryProduct.GetValueAsync();
            Assert.Equal(1, product.UnitPrice);

            //Action Bound on Entity
            var expectedAccessLevel = AccessLevel.ReadWrite | AccessLevel.Execute;
            var accessLevelAction = this.testClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } }).AddAccessRight(expectedAccessLevel);
            Assert.True(accessLevelAction.RequestUri.OriginalString.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.AddAccessRight"));
            var accessLevel = await accessLevelAction.GetValueAsync();
            Assert.True(accessLevel.Value.HasFlag(expectedAccessLevel));

            //Function Bound on Entity and return Collection of Entity
            //Won't execute since ODL doesn't support it now.
            var getProductDetailsAction = this.testClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } }).GetProductDetails(1);
            var getRelatedProductAction = getProductDetailsAction.ByKey(new Dictionary<string, object> { { "ProductID", 10001 }, { "ProductDetailID", 10001 } }).GetRelatedProduct();
            Assert.True(getProductDetailsAction.RequestUri.OriginalString.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=1)"));
            Assert.True(getRelatedProductAction.RequestUri.OriginalString.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=1)(ProductID=10001,ProductDetailID=10001)/Microsoft.Test.OData.Services.ODataWCFService.GetRelatedProduct()"));

            foreach (var pd in await getProductDetailsAction.ExecuteAsync())
            {
                //Check whether GetEnumerator works
                Assert.Equal(5, pd.ProductID);
            }
        }


        [Fact]
        public void DelayQueryOnSingletonNet35()
        {
            this.testClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            this.testClientContext.MergeOption = MergeOption.OverwriteChanges;

            //Query Singleton
            var queryCompany1 = this.testClientContext.Company;
            var ar = queryCompany1.BeginGetValue(null, null).EnqueueWait(this);
            var company = queryCompany1.EndGetValue(ar);
            Assert.NotNull(company);

            //LoadProperty
            var ar2 = this.testClientContext.BeginLoadProperty(company, "Departments", null, null);
            this.testClientContext.EndLoadProperty(ar2);
            Assert.NotNull(company.Departments);

            //Query Property
            var queryName = this.testClientContext.Company.Select(c => c.Name);
            var ar3 = queryName.BeginGetValue(null, null).EnqueueWait(this);
            var name = queryName.EndGetValue(ar3);
            Assert.Equal(company.Name, name);

            //Projection
            var companyQuery = this.testClientContext.Company.Select(c => new Company() { Name = c.Name, Address = c.Address });
            var ar4 = companyQuery.BeginGetValue(null, null).EnqueueWait(this);
            var company2 = companyQuery.EndGetValue(ar4);
            Assert.True(companyQuery.RequestUri.OriginalString.EndsWith("/Company?$select=Name,Address"));
            Assert.NotNull(company2.Address);

            //Expand
            companyQuery = this.testClientContext.Company.Expand(c => c.VipCustomer.Company);
            var ar5 = companyQuery.BeginGetValue(null, null).EnqueueWait(this);
            var company3 = companyQuery.EndGetValue(ar5);
            Assert.True(companyQuery.RequestUri.OriginalString.EndsWith("/Company?$expand=VipCustomer($expand=Company)"));
            Assert.NotNull(company);

            //Projection with Navigation
            companyQuery = this.testClientContext.Company.Select(c => new Company() { Name = c.Name, Address = c.Address, Departments = c.Departments });
            var ar6 = companyQuery.BeginGetValue(null, null).EnqueueWait(this);
            var company4 = companyQuery.EndGetValue(ar6);
            Assert.True(companyQuery.RequestUri.OriginalString.EndsWith("/Company?$expand=Departments&$select=Name,Address"));
            Assert.NotNull(company4.Address);

            //Query navigation property which is an collection
            var employeesQuery = this.testClientContext.Company.Employees;
            var ar7 = employeesQuery.BeginExecute(null, null).EnqueueWait(this);
            var employees = employeesQuery.EndExecute(ar7);
            Assert.True(employeesQuery.RequestUri.OriginalString.EndsWith("/Company/Employees"));
            Assert.NotNull(employees);

            //Query Navigation Property which is an single entity
            companyQuery = this.testClientContext.Company.VipCustomer.Company;
            var ar8 = companyQuery.BeginGetValue(null, null).EnqueueWait(this);
            var company5 = companyQuery.EndGetValue(ar8);
            Assert.True(companyQuery.RequestUri.OriginalString.EndsWith("/Company/VipCustomer/Company"));
            Assert.NotNull(company5);

            //Query navigation property which is from a singleton
            var coreDepartmentQuery = this.testClientContext.Company.CoreDepartment;
            var ar9 = coreDepartmentQuery.BeginGetValue(null, null).EnqueueWait(this);
            var coreDepartment = coreDepartmentQuery.EndGetValue(ar9);
            Assert.True(coreDepartmentQuery.RequestUri.OriginalString.EndsWith("/Company/CoreDepartment"));
            Assert.NotNull(coreDepartment);

            //QueryOption on navigation property
            employeesQuery = this.testClientContext.Company.Employees.Where(e => e.PersonID > 0) as DataServiceQuery<Employee>;
            var ar10 = employeesQuery.BeginExecute(null, null).EnqueueWait(this);
            employees = employeesQuery.EndExecute(ar10);
            Assert.True(employeesQuery.RequestUri.OriginalString.EndsWith("/Company/Employees?$filter=PersonID gt 0"));
            Assert.NotNull(employees);

            //Function Bound on Singleton
            var getEmployeesCountQuery = this.testClientContext.Company.GetEmployeesCount();
            var ar11 = getEmployeesCountQuery.BeginGetValue(null, null).EnqueueWait(this);
            var count = getEmployeesCountQuery.EndGetValue(ar11);
            Assert.True(getEmployeesCountQuery.RequestUri.OriginalString.EndsWith("/Company/Microsoft.Test.OData.Services.ODataWCFService.GetEmployeesCount()"));
            Assert.True(count > 0);

            //Function Bound on Singleton
            queryCompany1 = this.testClientContext.Company;
            var ar12 = queryCompany1.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany1.EndGetValue(ar12);

            getEmployeesCountQuery = company.GetEmployeesCount();
            var ar13 = getEmployeesCountQuery.BeginGetValue(null, null).EnqueueWait(this);
            count = getEmployeesCountQuery.EndGetValue(ar13);
            Assert.True(getEmployeesCountQuery.RequestUri.OriginalString.EndsWith("/Company/Microsoft.Test.OData.Services.ODataWCFService.GetEmployeesCount()"));
            Assert.True(count > 0);

            //Query Action bound on Navigation Property
            var getHomeAddressQuery = this.testClientContext.Company.VipCustomer.GetHomeAddress();
            var ar14 = getHomeAddressQuery.BeginGetValue(null, null).EnqueueWait(this);
            getHomeAddressQuery.EndGetValue(ar14);
            Assert.True(getHomeAddressQuery.RequestUri.OriginalString.EndsWith("/Company/VipCustomer/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()"));
        }

        [Fact]
        public async Task DelayQueryOnSingletonNet45()
        {
            this.testClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            this.testClientContext.MergeOption = MergeOption.OverwriteChanges;

            var company = await this.testClientContext.Company.GetValueAsync();
            Assert.NotNull(company);

            //LoadProperty
            await this.testClientContext.LoadPropertyAsync(company, "Departments");
            Assert.NotNull(company.Departments);

            //Query Property
            var name = await this.testClientContext.Company.Select(c => c.Name).GetValueAsync();
            Assert.Equal(company.Name, name);

            //Projection
            var companyQuery = this.testClientContext.Company.Select(c => new Company() { Name = c.Name, Address = c.Address });
            var company2 = await companyQuery.GetValueAsync();
            Assert.True(companyQuery.RequestUri.OriginalString.EndsWith("/Company?$select=Name,Address"));
            Assert.NotNull(company2.Address);

            //Expand
            companyQuery = this.testClientContext.Company.Expand(c => c.VipCustomer.Company);
            var company3 = await companyQuery.GetValueAsync();
            Assert.True(companyQuery.RequestUri.OriginalString.EndsWith("/Company?$expand=VipCustomer($expand=Company)"));
            Assert.NotNull(company);

            //Projection with Navigation
            companyQuery = this.testClientContext.Company.Select(c => new Company() { Name = c.Name, Address = c.Address, Departments = c.Departments });
            await companyQuery.GetValueAsync();
            Assert.True(companyQuery.RequestUri.OriginalString.EndsWith("/Company?$expand=Departments&$select=Name,Address"));
            Assert.NotNull(company2.Address);

            //Query navigation property which is an collection
            var employeesQuery = this.testClientContext.Company.Employees;
            var employees = await employeesQuery.ExecuteAsync();
            Assert.True(employeesQuery.RequestUri.OriginalString.EndsWith("/Company/Employees"));
            Assert.NotNull(employees);

            //Query Navigation Property which is an single entity
            var company4Query = this.testClientContext.Company.VipCustomer.Company;
            var company4 = await company4Query.GetValueAsync();
            Assert.True(company4Query.RequestUri.OriginalString.EndsWith("/Company/VipCustomer/Company"));
            Assert.NotNull(company4Query);

            //Query navigation property which is from a singleton
            var coreDepartmentQuery = this.testClientContext.Company.CoreDepartment;
            var coreDepartment = await coreDepartmentQuery.GetValueAsync();
            Assert.True(coreDepartmentQuery.RequestUri.OriginalString.EndsWith("/Company/CoreDepartment"));
            Assert.NotNull(coreDepartment);

            //QueryOption on navigation property
            employeesQuery = this.testClientContext.Company.Employees.Where(e => e.PersonID > 0) as DataServiceQuery<Employee>;
            employees = await employeesQuery.ExecuteAsync();
            Assert.True(employeesQuery.RequestUri.OriginalString.EndsWith("/Company/Employees?$filter=PersonID gt 0"));
            Assert.NotNull(employees);

            //Function Bound on Singleton
            var getEmployeesCountQuery = this.testClientContext.Company.GetEmployeesCount();
            var count = await getEmployeesCountQuery.GetValueAsync();
            Assert.True(getEmployeesCountQuery.RequestUri.OriginalString.EndsWith("/Company/Microsoft.Test.OData.Services.ODataWCFService.GetEmployeesCount()"));
            Assert.True(count > 0);

            //Function Bound on Singleton
            var company5 = await this.testClientContext.Company.GetValueAsync();
            getEmployeesCountQuery = company5.GetEmployeesCount();
            count = await getEmployeesCountQuery.GetValueAsync();
            Assert.True(getEmployeesCountQuery.RequestUri.OriginalString.EndsWith("/Company/Microsoft.Test.OData.Services.ODataWCFService.GetEmployeesCount()"));
            Assert.True(count > 0);

            //Query Action bound on Navigation Property, also on baseType
            var getHomeAddressQuery = this.testClientContext.Company.VipCustomer.GetHomeAddress();
            var homeAddress = await getHomeAddressQuery.GetValueAsync();
            Assert.True(getHomeAddressQuery.RequestUri.OriginalString.EndsWith("/Company/VipCustomer/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()"));
            Assert.NotNull(homeAddress);
        }

        [Fact]
        public async Task FunctionBoundOnContainedSingleNavigation()
        {
            this.testClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            var getActualAmountFunction = this.testClientContext.Accounts.ByKey(new Dictionary<string, object> { { "AccountID", 101 } }).MyGiftCard.GetActualAmount(1);
            var amount = await getActualAmountFunction.GetValueAsync();
            Assert.True(getActualAmountFunction.RequestUri.OriginalString.EndsWith("/Accounts(101)/MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=1.0)"));
        }
    }

    public class AsynchronousDelayQueryTests2 : EndToEndTestBase
    {
        InMemoryEntitiesPlus testClientContext;

        public AsynchronousDelayQueryTests2()
            : base(ServiceDescriptors.ODataWCFServicePlusDescriptor)
        {
        }

        [Fact]
        public async Task DelayQueryOnDerivedSingleton()
        {
            this.testClientContext = this.CreateWrappedContext<InMemoryEntitiesPlus>().Context;
            this.testClientContext.MergeOption = MergeOption.OverwriteChanges;

            //Query Singleton
            var publicCompanyQuery = this.testClientContext.PublicCompanyPlus;
            var publicCompany = await publicCompanyQuery.GetValueAsync();
            Assert.True(publicCompanyQuery.RequestUri.OriginalString.EndsWith("/PublicCompany"));
            Assert.NotNull(publicCompany);

            //Query Property On Derived Singleton
            var stockExchangeQuery = this.testClientContext.PublicCompanyPlus.Select(p => (p as PublicCompanyPlus).StockExchangePlus);
            var stockExchange = await stockExchangeQuery.GetValueAsync();
            Assert.True(stockExchangeQuery.RequestUri.OriginalString.EndsWith("/PublicCompany/Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/StockExchange"));
            Assert.NotNull(stockExchange);

            //Query Navigation Property on Dervied Singleton
            var assetsQuery = this.testClientContext.PublicCompanyPlus.CastToPublicCompanyPlus().AssetsPlus;
            var assets = await assetsQuery.ExecuteAsync();
            Assert.True(assetsQuery.RequestUri.OriginalString.EndsWith("/PublicCompany/Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/Assets"));
            Assert.NotNull(assets);

            //Filter Navigation Property on Dervied Singleton
            assetsQuery = this.testClientContext.PublicCompanyPlus.CastToPublicCompanyPlus().AssetsPlus.Where(a => a.NamePlus != "Temp") as DataServiceQuery<AssetPlus>;
            assets = await assetsQuery.ExecuteAsync();
            Assert.True(assetsQuery.RequestUri.OriginalString.EndsWith("/PublicCompany/Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/Assets?$filter=Name ne 'Temp'"));
            Assert.NotNull(assets);

            //Projection
            var publicCompany2Query = this.testClientContext.PublicCompanyPlus.Select(
                p =>
                    new PublicCompanyPlus()
                    {
                        AddressPlus = p.AddressPlus,
                        LabourUnionPlus = (p as PublicCompanyPlus).LabourUnionPlus,
                        StockExchangePlus = (p as PublicCompanyPlus).StockExchangePlus
                    });
            var publicCompany2 = await publicCompany2Query.GetValueAsync();
            Assert.True(publicCompany2Query.RequestUri.OriginalString.EndsWith(
                "/PublicCompany?$expand=Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/LabourUnion&$select=Address,Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/StockExchange"));
            Assert.NotNull(publicCompany2.AddressPlus);
            Assert.NotNull(publicCompany2.LabourUnionPlus);
            Assert.NotNull(publicCompany2.StockExchangePlus);

            //Expand Navigation Property on Derived Singleton
            var publicCompany3Query = this.testClientContext.PublicCompanyPlus.Expand(p => (p as PublicCompanyPlus).LabourUnionPlus);
            publicCompany = await publicCompany3Query.GetValueAsync();
            Assert.True(publicCompany3Query.RequestUri.OriginalString.EndsWith("/PublicCompany?$expand=Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/LabourUnion"));
            Assert.NotNull(((PublicCompanyPlus)publicCompany).LabourUnionPlus);

            //Action Bound on Navigation Property on Derived Singleton, return void
            DataServiceActionQuery changeLabourUnionAction = this.testClientContext.PublicCompanyPlus.CastToPublicCompanyPlus().LabourUnionPlus.ChangeLabourUnionNamePlus("changedLabourUnion");
            await changeLabourUnionAction.ExecuteAsync();
            Assert.True(changeLabourUnionAction.RequestUri.OriginalString.EndsWith("/PublicCompany/Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/LabourUnion/Microsoft.Test.OData.Services.ODataWCFService.ChangeLabourUnionName"));
            var labourUnion = (await this.testClientContext.PublicCompanyPlus.Select(p => (p as PublicCompanyPlus).LabourUnionPlus).GetValueAsync()).NamePlus;
            Assert.Equal("changedLabourUnion", labourUnion);
        }

        [Fact]
        public async Task DelayQueryOnFunctionImport()
        {
            this.testClientContext = this.CreateWrappedContext<InMemoryEntitiesPlus>().Context;
            this.testClientContext.MergeOption = MergeOption.OverwriteChanges;

            //Post an Product
            var product = ProductPlus.CreateProductPlus(10001, "10001", "2", 2.0f, 2, true);
            product.SkinColorPlus = ColorPlus.RedPlus;
            product.UserAccessPlus = AccessLevelPlus.ExecutePlus | AccessLevelPlus.ReadPlus;
            this.testClientContext.AddToProductsPlus(product);
            await this.testClientContext.SaveChangesAsync();

            //Composable FunctionImport, return collection of entitySet
            var getAllProductsFunction = this.testClientContext.GetAllProductsPlus();
            var products = (await getAllProductsFunction.ExecuteAsync()).ToList();
            Assert.True(products.Any());
            Assert.True(getAllProductsFunction.RequestUri.OriginalString.EndsWith("/GetAllProducts()"));
            double unitPrice = products.First().UnitPricePlus;

            //GetEnumerator
            var discountAction = getAllProductsFunction.DiscountPlus(50);
            foreach (var p in await discountAction.ExecuteAsync())
            {
                Assert.Equal(unitPrice * 50 / 100, p.UnitPricePlus);
                break;
            }
            Assert.True(discountAction.RequestUri.OriginalString.EndsWith("/GetAllProducts()/Microsoft.Test.OData.Services.ODataWCFService.Discount"));

            //Filter after Bound FunctionImport
            var filterAllProducts = getAllProductsFunction.Where(p => p.SkinColorPlus == ColorPlus.RedPlus) as DataServiceQuery<ProductPlus>;
            foreach (var p in await filterAllProducts.ExecuteAsync())
            {
                Assert.Equal(ColorPlus.RedPlus, p.SkinColorPlus);
            }
            Assert.True(filterAllProducts.RequestUri.OriginalString.EndsWith("/GetAllProducts()?$filter=SkinColor eq Microsoft.Test.OData.Services.ODataWCFService.Color'Red'"));

            //FunctionImport, return collection of primitivetype
            var getBossEmailsFunction = this.testClientContext.GetBossEmailsPlus(0, 10);
            var emails = await getBossEmailsFunction.ExecuteAsync();
            Assert.True(getBossEmailsFunction.RequestUri.OriginalString.EndsWith("/GetBossEmails(start=0,count=10)"));

            //FunctionImport, return Collection of primitivetype with enum parameter
            //Fail now
            var productsNameQuery = this.testClientContext.GetProductsByAccessLevelPlus(AccessLevelPlus.ReadPlus | AccessLevelPlus.ExecutePlus);
            var productsName = await productsNameQuery.ExecuteAsync();

            Assert.True(Uri.UnescapeDataString(productsNameQuery.RequestUri.OriginalString).EndsWith("/GetProductsByAccessLevel(accessLevel=Microsoft.Test.OData.Services.ODataWCFService.AccessLevel'Read,Execute')"));
            Assert.True(productsName.Any());
        }

        [Fact]
        public void DelayQueryOnActionImport35()
        {
            this.testClientContext = this.CreateWrappedContext<InMemoryEntitiesPlus>().Context;
            this.testClientContext.MergeOption = MergeOption.OverwriteChanges;

            var queryProduct = this.testClientContext.ProductsPlus.Take(1) as DataServiceQuery<ProductPlus>;
            var ar = queryProduct.BeginExecute(null, null).EnqueueWait(this);
            var product = queryProduct.EndExecute(ar).Single();
            double originalPrice = product.UnitPricePlus;

            //ActionImport return void
            var discountAction = this.testClientContext.DiscountPlus(50);
            var ar2 = discountAction.BeginExecute(null, null).EnqueueWait(this);
            discountAction.EndExecute(ar2);
            Assert.True(discountAction.RequestUri.OriginalString.EndsWith("/Discount"));

            var ar3 = queryProduct.BeginExecute(null, null).EnqueueWait(this);
            product = queryProduct.EndExecute(ar3).Single();
            Assert.Equal(originalPrice * 50 / 100, product.UnitPricePlus);
        }

        [Fact]
        public async Task DelayQueryOnActionImport45()
        {
            this.testClientContext = this.CreateWrappedContext<InMemoryEntitiesPlus>().Context;
            this.testClientContext.MergeOption = MergeOption.OverwriteChanges;

            var product = (await (this.testClientContext.ProductsPlus.Take(1) as DataServiceQuery<ProductPlus>).ExecuteAsync()).Single();
            double originalPrice = product.UnitPricePlus;

            //ActionImport return void
            var discountAction = this.testClientContext.DiscountPlus(50);
            await discountAction.ExecuteAsync();
            Assert.True(discountAction.RequestUri.OriginalString.EndsWith("/Discount"));

            product = (await (this.testClientContext.ProductsPlus.Take(1) as DataServiceQuery<ProductPlus>).ExecuteAsync()).Single();
            Assert.Equal(originalPrice * 50 / 100, product.UnitPricePlus);
        }

        [Fact]
        public void FunctionAndFunctionImportInFilterAndOrderBy()
        {
            this.testClientContext = this.CreateWrappedContext<InMemoryEntitiesPlus>().Context;
            this.testClientContext.MergeOption = MergeOption.OverwriteChanges;

            //FunctionImport in query option
            var queryAccount = this.testClientContext.AccountsPlus.Where(a => a.MyGiftCardPlus.GetActualAmountPlus(0.5).GetValueAsync().Result > 1) as DataServiceQuery<AccountPlus>;
            Assert.True(queryAccount.RequestUri.OriginalString.EndsWith("/Accounts?$filter=MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.5) gt 1.0"));
            //queryAccount.Execute();

            queryAccount = this.testClientContext.AccountsPlus.Where(a => 1 != a.MyGiftCardPlus.GetActualAmountPlus(0.5).GetValueAsync().Result) as DataServiceQuery<AccountPlus>;
            Assert.True(queryAccount.RequestUri.OriginalString.EndsWith("/Accounts?$filter=1.0 ne MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.5)"));
            //queryAccount.Execute();

            var queryPeople = this.testClientContext.PeoplePlus.Where(p => p.GetHomeAddressPlus().GetValueAsync().Result.FamilyNamePlus != "name") as DataServiceQuery<PersonPlus>;
            Assert.True(queryPeople.RequestUri.OriginalString.EndsWith("/People?$filter=$it/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()/FamilyName ne 'name'"));
            //queryPeople.Execute();

            queryPeople = this.testClientContext.PeoplePlus.Where(p => "name" != p.GetHomeAddressPlus().GetValueAsync().Result.FamilyNamePlus) as DataServiceQuery<PersonPlus>;
            Assert.True(queryPeople.RequestUri.OriginalString.EndsWith("/People?$filter='name' ne $it/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()/FamilyName"));
            //queryPeople.Execute();

            var queryProducts = this.testClientContext.ProductsPlus.Where(p => p.GetProductDetailsPlus(2).Count() > 1) as DataServiceQuery<ProductPlus>;
            Assert.True(queryProducts.RequestUri.OriginalString.EndsWith("/Products?$filter=$it/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=2)/$count gt 1"));
            //queryProducts.Execute();

            queryProducts = this.testClientContext.ProductsPlus.Where(p => 1 <= p.GetProductDetailsPlus(2).Count()) as DataServiceQuery<ProductPlus>;
            Assert.True(queryProducts.RequestUri.OriginalString.EndsWith("/Products?$filter=1 le $it/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=2)/$count"));
            //queryAccount.Execute();

            queryAccount = this.testClientContext.AccountsPlus.OrderBy(a => a.MyGiftCardPlus.GetActualAmountPlus(0.5).GetValueAsync().Result) as DataServiceQuery<AccountPlus>;
            Assert.True(queryAccount.RequestUri.OriginalString.EndsWith("/Accounts?$orderby=MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.5)"));
            //queryAccount.Execute();

            queryAccount = this.testClientContext.AccountsPlus.OrderByDescending(a => a.MyGiftCardPlus.GetActualAmountPlus(0.5).GetValueAsync().Result) as DataServiceQuery<AccountPlus>;
            Assert.True(queryAccount.RequestUri.OriginalString.EndsWith("/Accounts?$orderby=MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.5) desc"));
            //queryAccount.Execute();

            queryPeople = this.testClientContext.PeoplePlus.OrderBy(p => p.GetHomeAddressPlus().GetValueAsync().Result.FamilyNamePlus) as DataServiceQuery<PersonPlus>;
            Assert.True(queryPeople.RequestUri.OriginalString.EndsWith("/People?$orderby=$it/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()/FamilyName"));
            //queryPeople.Execute();

            queryPeople = this.testClientContext.PeoplePlus.OrderByDescending(p => p.GetHomeAddressPlus().GetValueAsync().Result.FamilyNamePlus) as DataServiceQuery<PersonPlus>;
            Assert.True(queryPeople.RequestUri.OriginalString.EndsWith("/People?$orderby=$it/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()/FamilyName desc"));
            //queryPeople.Execute();
        }
    }
}
