//---------------------------------------------------------------------
// <copyright file="AsynchronousDelayQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Client;
using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReferencePlus;
using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
using Microsoft.Test.OData.Services.TestServices;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Test.OData.Tests.Client.AsynchronousTests
{
    public class AsynchronousDelayQueryTests : EndToEndTestBase
    {
        InMemoryEntities TestClientContext;
 
        public AsynchronousDelayQueryTests(ITestOutputHelper helper)
            : base(ServiceDescriptors.ODataWCFServiceDescriptor, helper)
        {
        }

        [Fact, Asynchronous]
        public async Task DelayQueryOnEntitySetNet35()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            //Post a Product
            var product = Product.CreateProduct(10001, "10001", "2", 2.0f, 2, true);
            TestClientContext.AddToProducts(product);
            await TestClientContext.SaveChangesAsync();

            //Action Bound on EntitySet and return an EntitySet
            var discountAction = TestClientContext.Products.Discount(50);
            Assert.EndsWith("/Products/Microsoft.Test.OData.Services.ODataWCFService.Discount", discountAction.RequestUri.OriginalString);
            var ar = discountAction.BeginExecute(null, null).EnqueueWait(this);
            discountAction.EndExecute(ar);

            //Query an Entity
            var queryProduct = TestClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } });
            Assert.EndsWith("/Products(10001)", queryProduct.RequestUri.OriginalString);
            queryProduct.BeginGetValue(
                (ar1) =>
                {
                    product = queryProduct.EndGetValue(ar1);
                    Assert.Equal(1, product.UnitPrice);
                }, queryProduct).EnqueueWait(this);

            //Action Bound on Entity and return an Enum
            var expectedAccessLevel = AccessLevel.ReadWrite | AccessLevel.Execute;
            var accessLevelAction = TestClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } }).AddAccessRight(expectedAccessLevel);
            Assert.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.AddAccessRight", accessLevelAction.RequestUri.OriginalString);
            var ar2 = accessLevelAction.BeginGetValue(null, null).EnqueueWait(this);
            var accessLevel = accessLevelAction.EndGetValue(ar2);
            Assert.True(accessLevel.Value.HasFlag(expectedAccessLevel));

            //Function Bound on Entity and return Collection of Entity
            //Won't execute since ODL doesn't support it now.
            var getProductDetailsFunction = TestClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } }).GetProductDetails(1);
            var getRelatedProductAction = getProductDetailsFunction.ByKey(new Dictionary<string, object> { { "ProductID", 10001 }, { "ProductDetailID", 10001 } }).GetRelatedProduct();
            Assert.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=1)", getProductDetailsFunction.RequestUri.OriginalString);
            Assert.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=1)(ProductID=10001,ProductDetailID=10001)/Microsoft.Test.OData.Services.ODataWCFService.GetRelatedProduct()", getRelatedProductAction.RequestUri.OriginalString);

            var ar3 = getProductDetailsFunction.BeginExecute(null, null).EnqueueWait(this);
            var productDetails = getProductDetailsFunction.EndExecute(ar3);
            foreach (var pd in productDetails)
            {
                //Check whether GetEnumerator works
                Assert.Equal(5, pd.ProductID);
            }
        }

        [Fact, Asynchronous]
        public async Task DelayQueryOnEntitySetNet45()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;
            
            //Post an Product
            var product = Product.CreateProduct(10001, "10001", "2", 2.0f, 2, true);
            TestClientContext.AddToProducts(product);
            await TestClientContext.SaveChangesAsync();

            //Action Bound on EntitySet
            var discountAction = TestClientContext.Products.Discount(50);
            Assert.EndsWith("/Products/Microsoft.Test.OData.Services.ODataWCFService.Discount", discountAction.RequestUri.OriginalString);
            await discountAction.ExecuteAsync();

            //ByKey
            var queryProduct = TestClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } });
            Assert.EndsWith("/Products(10001)", queryProduct.RequestUri.OriginalString);
            product = await queryProduct.GetValueAsync();
            Assert.Equal(1, product.UnitPrice);

            //Action Bound on Entity
            var expectedAccessLevel = AccessLevel.ReadWrite | AccessLevel.Execute;
            var accessLevelAction = TestClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } }).AddAccessRight(expectedAccessLevel);
            Assert.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.AddAccessRight", accessLevelAction.RequestUri.OriginalString);
            var accessLevel = await accessLevelAction.GetValueAsync();
            Assert.True(accessLevel.Value.HasFlag(expectedAccessLevel));

            //Function Bound on Entity and return Collection of Entity
            //Won't execute since ODL doesn't support it now.
            var getProductDetailsAction = TestClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } }).GetProductDetails(1);
            var getRelatedProductAction = getProductDetailsAction.ByKey(new Dictionary<string, object> { { "ProductID", 10001 }, { "ProductDetailID", 10001 } }).GetRelatedProduct();
            Assert.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=1)", getProductDetailsAction.RequestUri.OriginalString);
            Assert.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=1)(ProductID=10001,ProductDetailID=10001)/Microsoft.Test.OData.Services.ODataWCFService.GetRelatedProduct()", getRelatedProductAction.RequestUri.OriginalString);

            foreach (var pd in await getProductDetailsAction.ExecuteAsync())
            {
                //Check whether GetEnumerator works
                Assert.Equal(5, pd.ProductID);
            }
        }


        [Fact, Asynchronous,]
        public void DelayQueryOnSingletonNet35()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            //Query Singleton
            var queryCompany1 = TestClientContext.Company;
            var ar = queryCompany1.BeginGetValue(null, null).EnqueueWait(this);
            var company = queryCompany1.EndGetValue(ar);
            Assert.NotNull(company);

            //LoadProperty
            var ar2 = TestClientContext.BeginLoadProperty(company, "Departments", null, null);
            TestClientContext.EndLoadProperty(ar2);
            Assert.NotNull(company.Departments);

            //Query Property
            var queryName = TestClientContext.Company.Select(c => c.Name);
            var ar3 = queryName.BeginGetValue(null, null).EnqueueWait(this);
            var name = queryName.EndGetValue(ar3);
            Assert.Equal(company.Name, name);

            //Projection
            var companyQuery = TestClientContext.Company.Select(c => new Company() { Name = c.Name, Address = c.Address });
            var ar4 = companyQuery.BeginGetValue(null, null).EnqueueWait(this);
            var company2 = companyQuery.EndGetValue(ar4);
            Assert.EndsWith("/Company?$select=Name,Address", companyQuery.RequestUri.OriginalString);
            Assert.NotNull(company2.Address);

            //Expand
            companyQuery = TestClientContext.Company.Expand(c => c.VipCustomer.Company);
            var ar5 = companyQuery.BeginGetValue(null, null).EnqueueWait(this);
            var company3 = companyQuery.EndGetValue(ar5);
            Assert.EndsWith("/Company?$expand=VipCustomer($expand=Company)", companyQuery.RequestUri.OriginalString);
            Assert.NotNull(company);

            //Projection with Navigation
            companyQuery = TestClientContext.Company.Select(c => new Company() { Name = c.Name, Address = c.Address, Departments = c.Departments });
            var ar6 = companyQuery.BeginGetValue(null, null).EnqueueWait(this);
            var company4 = companyQuery.EndGetValue(ar6);
            Assert.EndsWith("/Company?$expand=Departments&$select=Name,Address", companyQuery.RequestUri.OriginalString);
            Assert.NotNull(company4.Address);

            //Query navigation property which is an collection
            var employeesQuery = TestClientContext.Company.Employees;
            var ar7 = employeesQuery.BeginExecute(null, null).EnqueueWait(this);
            var employees = employeesQuery.EndExecute(ar7);
            Assert.EndsWith("/Company/Employees", employeesQuery.RequestUri.OriginalString);
            Assert.NotNull(employees);

            //Query Navigation Property which is an single entity
            companyQuery = TestClientContext.Company.VipCustomer.Company;
            var ar8 = companyQuery.BeginGetValue(null, null).EnqueueWait(this);
            var company5 = companyQuery.EndGetValue(ar8);
            Assert.EndsWith("/Company/VipCustomer/Company", companyQuery.RequestUri.OriginalString);
            Assert.NotNull(company5);

            //Query navigation property which is from a singleton
            var coreDepartmentQuery = TestClientContext.Company.CoreDepartment;
            var ar9 = coreDepartmentQuery.BeginGetValue(null, null).EnqueueWait(this);
            var coreDepartment = coreDepartmentQuery.EndGetValue(ar9);
            Assert.EndsWith("/Company/CoreDepartment", coreDepartmentQuery.RequestUri.OriginalString);
            Assert.NotNull(coreDepartment);

            //QueryOption on navigation property
            employeesQuery = TestClientContext.Company.Employees.Where(e => e.PersonID > 0) as DataServiceQuery<Employee>;
            var ar10 = employeesQuery.BeginExecute(null, null).EnqueueWait(this);
            employees = employeesQuery.EndExecute(ar10);
            Assert.EndsWith("/Company/Employees?$filter=PersonID gt 0", employeesQuery.RequestUri.OriginalString);
            Assert.NotNull(employees);

            //Function Bound on Singleton
            var getEmployeesCountQuery = TestClientContext.Company.GetEmployeesCount();
            var ar11 = getEmployeesCountQuery.BeginGetValue(null, null).EnqueueWait(this);
            var count = getEmployeesCountQuery.EndGetValue(ar11);
            Assert.EndsWith("/Company/Microsoft.Test.OData.Services.ODataWCFService.GetEmployeesCount()", getEmployeesCountQuery.RequestUri.OriginalString);
            Assert.True(count > 0);

            //Function Bound on Singleton
            queryCompany1 = TestClientContext.Company;
            var ar12 = queryCompany1.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany1.EndGetValue(ar12);

            getEmployeesCountQuery = company.GetEmployeesCount();
            var ar13 = getEmployeesCountQuery.BeginGetValue(null, null).EnqueueWait(this);
            count = getEmployeesCountQuery.EndGetValue(ar13);
            Assert.EndsWith("/Company/Microsoft.Test.OData.Services.ODataWCFService.GetEmployeesCount()", getEmployeesCountQuery.RequestUri.OriginalString);
            Assert.True(count > 0);

            //Query Action bound on Navigation Property
            var getHomeAddressQuery = TestClientContext.Company.VipCustomer.GetHomeAddress();
            var ar14 = getHomeAddressQuery.BeginGetValue(null, null).EnqueueWait(this);
            getHomeAddressQuery.EndGetValue(ar14);
            Assert.EndsWith("/Company/VipCustomer/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()", getHomeAddressQuery.RequestUri.OriginalString);
        }

        [Fact, Asynchronous]
        public async Task DelayQueryOnSingletonNet45()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            var company = await TestClientContext.Company.GetValueAsync();
            Assert.NotNull(company);

            //LoadProperty
            await TestClientContext.LoadPropertyAsync(company, "Departments");
            Assert.NotNull(company.Departments);

            //Query Property
            var name = await TestClientContext.Company.Select(c => c.Name).GetValueAsync();
            Assert.Equal(company.Name, name);

            //Projection
            var companyQuery = TestClientContext.Company.Select(c => new Company() { Name = c.Name, Address = c.Address });
            var company2 = await companyQuery.GetValueAsync();
            Assert.EndsWith("/Company?$select=Name,Address", companyQuery.RequestUri.OriginalString);
            Assert.NotNull(company2.Address);

            //Expand
            companyQuery = TestClientContext.Company.Expand(c => c.VipCustomer.Company);
            var company3 = await companyQuery.GetValueAsync();
            Assert.EndsWith("/Company?$expand=VipCustomer($expand=Company)", companyQuery.RequestUri.OriginalString);
            Assert.NotNull(company);

            //Projection with Navigation
            companyQuery = TestClientContext.Company.Select(c => new Company() { Name = c.Name, Address = c.Address, Departments = c.Departments });
            await companyQuery.GetValueAsync();
            Assert.EndsWith("/Company?$expand=Departments&$select=Name,Address", companyQuery.RequestUri.OriginalString);
            Assert.NotNull(company2.Address);

            //Query navigation property which is an collection
            var employeesQuery = TestClientContext.Company.Employees;
            var employees = await employeesQuery.ExecuteAsync();
            Assert.EndsWith("/Company/Employees", employeesQuery.RequestUri.OriginalString);
            Assert.NotNull(employees);

            //Query Navigation Property which is an single entity
            var company4Query = TestClientContext.Company.VipCustomer.Company;
            var company4 = await company4Query.GetValueAsync();
            Assert.EndsWith("/Company/VipCustomer/Company", company4Query.RequestUri.OriginalString);
            Assert.NotNull(company4Query);

            //Query navigation property which is from a singleton
            var coreDepartmentQuery = TestClientContext.Company.CoreDepartment;
            var coreDepartment = await coreDepartmentQuery.GetValueAsync();
            Assert.EndsWith("/Company/CoreDepartment", coreDepartmentQuery.RequestUri.OriginalString);
            Assert.NotNull(coreDepartment);

            //QueryOption on navigation property
            employeesQuery = TestClientContext.Company.Employees.Where(e => e.PersonID > 0) as DataServiceQuery<Employee>;
            employees = await employeesQuery.ExecuteAsync();
            Assert.EndsWith("/Company/Employees?$filter=PersonID gt 0", employeesQuery.RequestUri.OriginalString);
            Assert.NotNull(employees);

            //Function Bound on Singleton
            var getEmployeesCountQuery = TestClientContext.Company.GetEmployeesCount();
            var count = await getEmployeesCountQuery.GetValueAsync();
            Assert.EndsWith("/Company/Microsoft.Test.OData.Services.ODataWCFService.GetEmployeesCount()", getEmployeesCountQuery.RequestUri.OriginalString);
            Assert.True(count > 0);

            //Function Bound on Singleton
            var company5 = await TestClientContext.Company.GetValueAsync();
            getEmployeesCountQuery = company5.GetEmployeesCount();
            count = await getEmployeesCountQuery.GetValueAsync();
            Assert.EndsWith("/Company/Microsoft.Test.OData.Services.ODataWCFService.GetEmployeesCount()", getEmployeesCountQuery.RequestUri.OriginalString);
            Assert.True(count > 0);

            //Query Action bound on Navigation Property, also on baseType
            var getHomeAddressQuery = TestClientContext.Company.VipCustomer.GetHomeAddress();
            var homeAddress = await getHomeAddressQuery.GetValueAsync();
            Assert.EndsWith("/Company/VipCustomer/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()", getHomeAddressQuery.RequestUri.OriginalString);
            Assert.NotNull(homeAddress);
        }

        [Fact, Asynchronous]
        public async Task FunctionBoundOnContainedSingleNavigation()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            var getActualAmountFunction = TestClientContext.Accounts.ByKey(new Dictionary<string, object> { { "AccountID", 101 } }).MyGiftCard.GetActualAmount(1);
            var amount = await getActualAmountFunction.GetValueAsync();
            Assert.EndsWith("/Accounts(101)/MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=1.0)", getActualAmountFunction.RequestUri.OriginalString);
        }
    }

    public class AsynchronousDelayQueryTests2 : EndToEndTestBase
    {
        InMemoryEntitiesPlus TestClientContext;

        public AsynchronousDelayQueryTests2(ITestOutputHelper helper)
            : base(ServiceDescriptors.ODataWCFServicePlusDescriptor, helper)
        {
        }

        [Fact, Asynchronous]
        public async Task DelayQueryOnDerivedSingleton()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntitiesPlus>().Context;
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            //Query Singleton
            var publicCompanyQuery = TestClientContext.PublicCompanyPlus;
            var publicCompany = await publicCompanyQuery.GetValueAsync();
            Assert.EndsWith("/PublicCompany", publicCompanyQuery.RequestUri.OriginalString);
            Assert.NotNull(publicCompany);

            //Query Property On Derived Singleton
            var stockExchangeQuery = TestClientContext.PublicCompanyPlus.Select(p => (p as PublicCompanyPlus).StockExchangePlus);
            var stockExchange = await stockExchangeQuery.GetValueAsync();
            Assert.EndsWith("/PublicCompany/Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/StockExchange", stockExchangeQuery.RequestUri.OriginalString);
            Assert.NotNull(stockExchange);

            //Query Navigation Property on Dervied Singleton
            var assetsQuery = TestClientContext.PublicCompanyPlus.CastToPublicCompanyPlus().AssetsPlus;
            var assets = await assetsQuery.ExecuteAsync();
            Assert.EndsWith("/PublicCompany/Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/Assets", assetsQuery.RequestUri.OriginalString);
            Assert.NotNull(assets);

            //Filter Navigation Property on Dervied Singleton
            assetsQuery = TestClientContext.PublicCompanyPlus.CastToPublicCompanyPlus().AssetsPlus.Where(a => a.NamePlus != "Temp") as DataServiceQuery<AssetPlus>;
            assets = await assetsQuery.ExecuteAsync();
            Assert.EndsWith("/PublicCompany/Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/Assets?$filter=Name ne 'Temp'", assetsQuery.RequestUri.OriginalString);
            Assert.NotNull(assets);

            //Projection
            var publicCompany2Query = TestClientContext.PublicCompanyPlus.Select(
                p =>
                    new PublicCompanyPlus()
                    {
                        AddressPlus = p.AddressPlus,
                        LabourUnionPlus = (p as PublicCompanyPlus).LabourUnionPlus,
                        StockExchangePlus = (p as PublicCompanyPlus).StockExchangePlus
                    });
            var publicCompany2 = await publicCompany2Query.GetValueAsync();
            Assert.EndsWith("/PublicCompany?$expand=Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/LabourUnion&$select=Address,Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/StockExchange", publicCompany2Query.RequestUri.OriginalString);
            Assert.NotNull(publicCompany2.AddressPlus);
            Assert.NotNull(publicCompany2.LabourUnionPlus);
            Assert.NotNull(publicCompany2.StockExchangePlus);

            //Expand Navigation Property on Derived Singleton
            var publicCompany3Query = TestClientContext.PublicCompanyPlus.Expand(p => (p as PublicCompanyPlus).LabourUnionPlus);
            publicCompany = await publicCompany3Query.GetValueAsync();
            Assert.EndsWith("/PublicCompany?$expand=Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/LabourUnion", publicCompany3Query.RequestUri.OriginalString);
            Assert.NotNull((publicCompany as PublicCompanyPlus).LabourUnionPlus);

            //Action Bound on Navigation Property on Derived Singleton, return void
            DataServiceActionQuery changeLabourUnionAction = TestClientContext.PublicCompanyPlus.CastToPublicCompanyPlus().LabourUnionPlus.ChangeLabourUnionNamePlus("changedLabourUnion");
            await changeLabourUnionAction.ExecuteAsync();
            Assert.EndsWith("/PublicCompany/Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/LabourUnion/Microsoft.Test.OData.Services.ODataWCFService.ChangeLabourUnionName", changeLabourUnionAction.RequestUri.OriginalString);
            var labourUnion = (await TestClientContext.PublicCompanyPlus.Select(p => (p as PublicCompanyPlus).LabourUnionPlus).GetValueAsync()).NamePlus;
            Assert.Equal("changedLabourUnion", labourUnion);
        }

        [Fact, Asynchronous]
        public async Task DelayQueryOnFunctionImport()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntitiesPlus>().Context;
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            //Post an Product
            var product = ProductPlus.CreateProductPlus(10001, "10001", "2", 2.0f, 2, true);
            product.SkinColorPlus = ColorPlus.RedPlus;
            product.UserAccessPlus = AccessLevelPlus.ExecutePlus | AccessLevelPlus.ReadPlus;
            TestClientContext.AddToProductsPlus(product);
            await TestClientContext.SaveChangesAsync();

            //Composable FunctionImport, return collection of entitySet
            var getAllProductsFunction = TestClientContext.GetAllProductsPlus();
            var products = (await getAllProductsFunction.ExecuteAsync()).ToList();
            Assert.True(products.Count() > 0);
            Assert.EndsWith("/GetAllProducts()", getAllProductsFunction.RequestUri.OriginalString);
            double unitPrice = products.First().UnitPricePlus;

            //GetEnumerator
            var discountAction = getAllProductsFunction.DiscountPlus(50);
            foreach (var p in await discountAction.ExecuteAsync())
            {
                Assert.Equal(unitPrice * 50 / 100, p.UnitPricePlus);
                break;
            }
            Assert.EndsWith("/GetAllProducts()/Microsoft.Test.OData.Services.ODataWCFService.Discount", discountAction.RequestUri.OriginalString);

            //Filter after Bound FunctionImport
            var filterAllProducts = getAllProductsFunction.Where(p => p.SkinColorPlus == ColorPlus.RedPlus) as DataServiceQuery<ProductPlus>;
            foreach (var p in await filterAllProducts.ExecuteAsync())
            {
                Assert.Equal(ColorPlus.RedPlus, p.SkinColorPlus);
            }
            Assert.EndsWith("/GetAllProducts()?$filter=SkinColor eq Microsoft.Test.OData.Services.ODataWCFService.Color'Red'", filterAllProducts.RequestUri.OriginalString);

            //FunctionImport, return collection of primitivetype
            var getBossEmailsFunction = TestClientContext.GetBossEmailsPlus(0, 10);
            var emails = await getBossEmailsFunction.ExecuteAsync();
            Assert.EndsWith("/GetBossEmails(start=0,count=10)", getBossEmailsFunction.RequestUri.OriginalString);

            //FunctionImport, return Collection of primitivetype with enum parameter
            //Fail now
            var productsNameQuery = TestClientContext.GetProductsByAccessLevelPlus(AccessLevelPlus.ReadPlus | AccessLevelPlus.ExecutePlus);
            var productsName = await productsNameQuery.ExecuteAsync();

            Assert.EndsWith("/GetProductsByAccessLevel(accessLevel=Microsoft.Test.OData.Services.ODataWCFService.AccessLevel'Read,Execute')", Uri.UnescapeDataString(productsNameQuery.RequestUri.OriginalString));
            Assert.True(productsName.Count() > 0);
        }

        [Fact, Asynchronous]
        public void DelayQueryOnActionImport35()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntitiesPlus>().Context;
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            var queryProduct = TestClientContext.ProductsPlus.Take(1) as DataServiceQuery<ProductPlus>;
            var ar = queryProduct.BeginExecute(null, null).EnqueueWait(this);
            var product = queryProduct.EndExecute(ar).Single();
            double originalPrice = product.UnitPricePlus;

            //ActionImport return void
            var discountAction = TestClientContext.DiscountPlus(50);
            var ar2 = discountAction.BeginExecute(null, null).EnqueueWait(this);
            discountAction.EndExecute(ar2);
            Assert.EndsWith("/Discount", discountAction.RequestUri.OriginalString);

            var ar3 = queryProduct.BeginExecute(null, null).EnqueueWait(this);
            product = queryProduct.EndExecute(ar3).Single();
            Assert.Equal(originalPrice * 50 / 100, product.UnitPricePlus);
        }

        [Fact, Asynchronous]
        public async Task DelayQueryOnActionImport45()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntitiesPlus>().Context;
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            var product = (await (TestClientContext.ProductsPlus.Take(1) as DataServiceQuery<ProductPlus>).ExecuteAsync()).Single();
            double originalPrice = product.UnitPricePlus;

            //ActionImport return void
            var discountAction = TestClientContext.DiscountPlus(50);
            await discountAction.ExecuteAsync();
            Assert.EndsWith("/Discount", discountAction.RequestUri.OriginalString);

            product = (await (TestClientContext.ProductsPlus.Take(1) as DataServiceQuery<ProductPlus>).ExecuteAsync()).Single();
            Assert.Equal(originalPrice * 50 / 100, product.UnitPricePlus);
        }

        [Fact]
        public void FunctionAndFunctionImportInFilterAndOrderBy()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntitiesPlus>().Context;
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            //FunctionImport in query option
            var queryAccount = TestClientContext.AccountsPlus.Where(a => a.MyGiftCardPlus.GetActualAmountPlus(0.5).GetValueAsync().Result > 1) as DataServiceQuery<AccountPlus>;
            Assert.EndsWith("/Accounts?$filter=MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.5) gt 1.0", queryAccount.RequestUri.OriginalString);
            //queryAccount.Execute();

            queryAccount = TestClientContext.AccountsPlus.Where(a => 1 != a.MyGiftCardPlus.GetActualAmountPlus(0.5).GetValueAsync().Result) as DataServiceQuery<AccountPlus>;
            Assert.EndsWith("/Accounts?$filter=1.0 ne MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.5)", queryAccount.RequestUri.OriginalString);
            //queryAccount.Execute();

            var queryPeople = TestClientContext.PeoplePlus.Where(p => p.GetHomeAddressPlus().GetValueAsync().Result.FamilyNamePlus != "name") as DataServiceQuery<PersonPlus>;
            Assert.EndsWith("/People?$filter=$it/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()/FamilyName ne 'name'", queryPeople.RequestUri.OriginalString);
            //queryPeople.Execute();

            queryPeople = TestClientContext.PeoplePlus.Where(p => "name" != p.GetHomeAddressPlus().GetValueAsync().Result.FamilyNamePlus) as DataServiceQuery<PersonPlus>;
            Assert.EndsWith("/People?$filter='name' ne $it/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()/FamilyName", queryPeople.RequestUri.OriginalString);
            //queryPeople.Execute();

            var queryProducts = TestClientContext.ProductsPlus.Where(p => p.GetProductDetailsPlus(2).Count() > 1) as DataServiceQuery<ProductPlus>;
            Assert.EndsWith("/Products?$filter=$it/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=2)/$count gt 1", queryProducts.RequestUri.OriginalString);
            //queryProducts.Execute();

            queryProducts = TestClientContext.ProductsPlus.Where(p => 1 <= p.GetProductDetailsPlus(2).Count()) as DataServiceQuery<ProductPlus>;
            Assert.EndsWith("/Products?$filter=1 le $it/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=2)/$count", queryProducts.RequestUri.OriginalString);
            //queryAccount.Execute();

            queryAccount = TestClientContext.AccountsPlus.OrderBy(a => a.MyGiftCardPlus.GetActualAmountPlus(0.5).GetValueAsync().Result) as DataServiceQuery<AccountPlus>;
            Assert.EndsWith("/Accounts?$orderby=MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.5)", queryAccount.RequestUri.OriginalString);
            //queryAccount.Execute();

            queryAccount = TestClientContext.AccountsPlus.OrderByDescending(a => a.MyGiftCardPlus.GetActualAmountPlus(0.5).GetValueAsync().Result) as DataServiceQuery<AccountPlus>;
            Assert.EndsWith("/Accounts?$orderby=MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.5) desc", queryAccount.RequestUri.OriginalString);
            //queryAccount.Execute();

            queryPeople = TestClientContext.PeoplePlus.OrderBy(p => p.GetHomeAddressPlus().GetValueAsync().Result.FamilyNamePlus) as DataServiceQuery<PersonPlus>;
            Assert.EndsWith("/People?$orderby=$it/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()/FamilyName", queryPeople.RequestUri.OriginalString);
            //queryPeople.Execute();

            queryPeople = TestClientContext.PeoplePlus.OrderByDescending(p => p.GetHomeAddressPlus().GetValueAsync().Result.FamilyNamePlus) as DataServiceQuery<PersonPlus>;
            Assert.EndsWith("/People?$orderby=$it/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()/FamilyName desc", queryPeople.RequestUri.OriginalString);
            //queryPeople.Execute();
        }
    }
}
