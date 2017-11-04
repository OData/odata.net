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
#if SILVERLIGHT
    using Microsoft.Silverlight.Testing;
#endif
using Microsoft.Test.OData.Services.TestServices;
#if WIN8 || WINDOWSPHONE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Test.OData.Tests.Client.AsynchronousTests
{
    [TestClass]
    public class AsynchronousDelayQueryTests : EndToEndTestBase
    {
        InMemoryEntities TestClientContext;

        public AsynchronousDelayQueryTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        [TestMethod, Asynchronous]
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
            Assert.IsTrue(discountAction.RequestUri.OriginalString.EndsWith("/Products/Microsoft.Test.OData.Services.ODataWCFService.Discount"));
            var ar = discountAction.BeginExecute(null, null).EnqueueWait(this);
            discountAction.EndExecute(ar);

            //Query an Entity
            var queryProduct = TestClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } });
            Assert.IsTrue(queryProduct.RequestUri.OriginalString.EndsWith("/Products(10001)"));
            queryProduct.BeginGetValue(
                (ar1) =>
                {
                    product = queryProduct.EndGetValue(ar1);
                    Assert.AreEqual(1, product.UnitPrice);
                }, queryProduct).EnqueueWait(this);

            //Action Bound on Entity and return an Enum
            var expectedAccessLevel = AccessLevel.ReadWrite | AccessLevel.Execute;
            var accessLevelAction = TestClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } }).AddAccessRight(expectedAccessLevel);
            Assert.IsTrue(accessLevelAction.RequestUri.OriginalString.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.AddAccessRight"));
            var ar2 = accessLevelAction.BeginGetValue(null, null).EnqueueWait(this);
            var accessLevel = accessLevelAction.EndGetValue(ar2);
            Assert.IsTrue(accessLevel.Value.HasFlag(expectedAccessLevel));

            //Function Bound on Entity and return Collection of Entity
            //Won't execute since ODL doesn't support it now.
            var getProductDetailsFunction = TestClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } }).GetProductDetails(1);
            var getRelatedProductAction = getProductDetailsFunction.ByKey(new Dictionary<string, object> { { "ProductID", 10001 }, { "ProductDetailID", 10001 } }).GetRelatedProduct();
            Assert.IsTrue(getProductDetailsFunction.RequestUri.OriginalString.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=1)"));
            Assert.IsTrue(getRelatedProductAction.RequestUri.OriginalString.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=1)(ProductID=10001,ProductDetailID=10001)/Microsoft.Test.OData.Services.ODataWCFService.GetRelatedProduct()"));

            var ar3 = getProductDetailsFunction.BeginExecute(null, null).EnqueueWait(this);
            var productDetails = getProductDetailsFunction.EndExecute(ar3);
            foreach (var pd in productDetails)
            {
                //Check whether GetEnumerator works
                Assert.AreEqual(5, pd.ProductID);
            }
        }

        [TestMethod, Asynchronous]
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
            Assert.IsTrue(discountAction.RequestUri.OriginalString.EndsWith("/Products/Microsoft.Test.OData.Services.ODataWCFService.Discount"));
            await discountAction.ExecuteAsync();

            //ByKey
            var queryProduct = TestClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } });
            Assert.IsTrue(queryProduct.RequestUri.OriginalString.EndsWith("/Products(10001)"));
            product = await queryProduct.GetValueAsync();
            Assert.AreEqual(1, product.UnitPrice);

            //Action Bound on Entity
            var expectedAccessLevel = AccessLevel.ReadWrite | AccessLevel.Execute;
            var accessLevelAction = TestClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } }).AddAccessRight(expectedAccessLevel);
            Assert.IsTrue(accessLevelAction.RequestUri.OriginalString.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.AddAccessRight"));
            var accessLevel = await accessLevelAction.GetValueAsync();
            Assert.IsTrue(accessLevel.Value.HasFlag(expectedAccessLevel));

            //Function Bound on Entity and return Collection of Entity
            //Won't execute since ODL doesn't support it now.
            var getProductDetailsAction = TestClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } }).GetProductDetails(1);
            var getRelatedProductAction = getProductDetailsAction.ByKey(new Dictionary<string, object> { { "ProductID", 10001 }, { "ProductDetailID", 10001 } }).GetRelatedProduct();
            Assert.IsTrue(getProductDetailsAction.RequestUri.OriginalString.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=1)"));
            Assert.IsTrue(getRelatedProductAction.RequestUri.OriginalString.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=1)(ProductID=10001,ProductDetailID=10001)/Microsoft.Test.OData.Services.ODataWCFService.GetRelatedProduct()"));

            foreach (var pd in await getProductDetailsAction.ExecuteAsync())
            {
                //Check whether GetEnumerator works
                Assert.AreEqual(5, pd.ProductID);
            }
        }


        [TestMethod, Asynchronous,]
        public void DelayQueryOnSingletonNet35()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            //Query Singleton
            var queryCompany1 = TestClientContext.Company;
            var ar = queryCompany1.BeginGetValue(null, null).EnqueueWait(this);
            var company = queryCompany1.EndGetValue(ar);
            Assert.IsNotNull(company);

            //LoadProperty
            var ar2 = TestClientContext.BeginLoadProperty(company, "Departments", null, null);
            TestClientContext.EndLoadProperty(ar2);
            Assert.IsNotNull(company.Departments);

            //Query Property
            var queryName = TestClientContext.Company.Select(c => c.Name);
            var ar3 = queryName.BeginGetValue(null, null).EnqueueWait(this);
            var name = queryName.EndGetValue(ar3);
            Assert.AreEqual(company.Name, name);

            //Projection
            var companyQuery = TestClientContext.Company.Select(c => new Company() { Name = c.Name, Address = c.Address });
            var ar4 = companyQuery.BeginGetValue(null, null).EnqueueWait(this);
            var company2 = companyQuery.EndGetValue(ar4);
            Assert.IsTrue(companyQuery.RequestUri.OriginalString.EndsWith("/Company?$select=Name,Address"));
            Assert.IsNotNull(company2.Address);

            //Expand
            companyQuery = TestClientContext.Company.Expand(c => c.VipCustomer.Company);
            var ar5 = companyQuery.BeginGetValue(null, null).EnqueueWait(this);
            var company3 = companyQuery.EndGetValue(ar5);
            Assert.IsTrue(companyQuery.RequestUri.OriginalString.EndsWith("/Company?$expand=VipCustomer($expand=Company)"));
            Assert.IsNotNull(company);

            //Projection with Navigation
            companyQuery = TestClientContext.Company.Select(c => new Company() { Name = c.Name, Address = c.Address, Departments = c.Departments });
            var ar6 = companyQuery.BeginGetValue(null, null).EnqueueWait(this);
            var company4 = companyQuery.EndGetValue(ar6);
            Assert.IsTrue(companyQuery.RequestUri.OriginalString.EndsWith("/Company?$expand=Departments&$select=Name,Address"));
            Assert.IsNotNull(company4.Address);

            //Query navigation property which is an collection
            var employeesQuery = TestClientContext.Company.Employees;
            var ar7 = employeesQuery.BeginExecute(null, null).EnqueueWait(this);
            var employees = employeesQuery.EndExecute(ar7);
            Assert.IsTrue(employeesQuery.RequestUri.OriginalString.EndsWith("/Company/Employees"));
            Assert.IsNotNull(employees);

            //Query Navigation Property which is an single entity
            companyQuery = TestClientContext.Company.VipCustomer.Company;
            var ar8 = companyQuery.BeginGetValue(null, null).EnqueueWait(this);
            var company5 = companyQuery.EndGetValue(ar8);
            Assert.IsTrue(companyQuery.RequestUri.OriginalString.EndsWith("/Company/VipCustomer/Company"));
            Assert.IsNotNull(company5);

            //Query navigation property which is from a singleton
            var coreDepartmentQuery = TestClientContext.Company.CoreDepartment;
            var ar9 = coreDepartmentQuery.BeginGetValue(null, null).EnqueueWait(this);
            var coreDepartment = coreDepartmentQuery.EndGetValue(ar9);
            Assert.IsTrue(coreDepartmentQuery.RequestUri.OriginalString.EndsWith("/Company/CoreDepartment"));
            Assert.IsNotNull(coreDepartment);

            //QueryOption on navigation property
            employeesQuery = TestClientContext.Company.Employees.Where(e => e.PersonID > 0) as DataServiceQuery<Employee>;
            var ar10 = employeesQuery.BeginExecute(null, null).EnqueueWait(this);
            employees = employeesQuery.EndExecute(ar10);
            Assert.IsTrue(employeesQuery.RequestUri.OriginalString.EndsWith("/Company/Employees?$filter=PersonID gt 0"));
            Assert.IsNotNull(employees);

            //Function Bound on Singleton
            var getEmployeesCountQuery = TestClientContext.Company.GetEmployeesCount();
            var ar11 = getEmployeesCountQuery.BeginGetValue(null, null).EnqueueWait(this);
            var count = getEmployeesCountQuery.EndGetValue(ar11);
            Assert.IsTrue(getEmployeesCountQuery.RequestUri.OriginalString.EndsWith("/Company/Microsoft.Test.OData.Services.ODataWCFService.GetEmployeesCount()"));
            Assert.IsTrue(count > 0);

            //Function Bound on Singleton
            queryCompany1 = TestClientContext.Company;
            var ar12 = queryCompany1.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany1.EndGetValue(ar12);

            getEmployeesCountQuery = company.GetEmployeesCount();
            var ar13 = getEmployeesCountQuery.BeginGetValue(null, null).EnqueueWait(this);
            count = getEmployeesCountQuery.EndGetValue(ar13);
            Assert.IsTrue(getEmployeesCountQuery.RequestUri.OriginalString.EndsWith("/Company/Microsoft.Test.OData.Services.ODataWCFService.GetEmployeesCount()"));
            Assert.IsTrue(count > 0);

            //Query Action bound on Navigation Property
            var getHomeAddressQuery = TestClientContext.Company.VipCustomer.GetHomeAddress();
            var ar14 = getHomeAddressQuery.BeginGetValue(null, null).EnqueueWait(this);
            getHomeAddressQuery.EndGetValue(ar14);
            Assert.IsTrue(getHomeAddressQuery.RequestUri.OriginalString.EndsWith("/Company/VipCustomer/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()"));
        }

        [TestMethod, Asynchronous]
        public async Task DelayQueryOnSingletonNet45()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            var company = await TestClientContext.Company.GetValueAsync();
            Assert.IsNotNull(company);

            //LoadProperty
            await TestClientContext.LoadPropertyAsync(company, "Departments");
            Assert.IsNotNull(company.Departments);

            //Query Property
            var name = await TestClientContext.Company.Select(c => c.Name).GetValueAsync();
            Assert.AreEqual(company.Name, name);

            //Projection
            var companyQuery = TestClientContext.Company.Select(c => new Company() { Name = c.Name, Address = c.Address });
            var company2 = await companyQuery.GetValueAsync();
            Assert.IsTrue(companyQuery.RequestUri.OriginalString.EndsWith("/Company?$select=Name,Address"));
            Assert.IsNotNull(company2.Address);

            //Expand
            companyQuery = TestClientContext.Company.Expand(c => c.VipCustomer.Company);
            var company3 = await companyQuery.GetValueAsync();
            Assert.IsTrue(companyQuery.RequestUri.OriginalString.EndsWith("/Company?$expand=VipCustomer($expand=Company)"));
            Assert.IsNotNull(company);

            //Projection with Navigation
            companyQuery = TestClientContext.Company.Select(c => new Company() { Name = c.Name, Address = c.Address, Departments = c.Departments });
            await companyQuery.GetValueAsync();
            Assert.IsTrue(companyQuery.RequestUri.OriginalString.EndsWith("/Company?$expand=Departments&$select=Name,Address"));
            Assert.IsNotNull(company2.Address);

            //Query navigation property which is an collection
            var employeesQuery = TestClientContext.Company.Employees;
            var employees = await employeesQuery.ExecuteAsync();
            Assert.IsTrue(employeesQuery.RequestUri.OriginalString.EndsWith("/Company/Employees"));
            Assert.IsNotNull(employees);

            //Query Navigation Property which is an single entity
            var company4Query = TestClientContext.Company.VipCustomer.Company;
            var company4 = await company4Query.GetValueAsync();
            Assert.IsTrue(company4Query.RequestUri.OriginalString.EndsWith("/Company/VipCustomer/Company"));
            Assert.IsNotNull(company4Query);

            //Query navigation property which is from a singleton
            var coreDepartmentQuery = TestClientContext.Company.CoreDepartment;
            var coreDepartment = await coreDepartmentQuery.GetValueAsync();
            Assert.IsTrue(coreDepartmentQuery.RequestUri.OriginalString.EndsWith("/Company/CoreDepartment"));
            Assert.IsNotNull(coreDepartment);

            //QueryOption on navigation property
            employeesQuery = TestClientContext.Company.Employees.Where(e => e.PersonID > 0) as DataServiceQuery<Employee>;
            employees = await employeesQuery.ExecuteAsync();
            Assert.IsTrue(employeesQuery.RequestUri.OriginalString.EndsWith("/Company/Employees?$filter=PersonID gt 0"));
            Assert.IsNotNull(employees);

            //Function Bound on Singleton
            var getEmployeesCountQuery = TestClientContext.Company.GetEmployeesCount();
            var count = await getEmployeesCountQuery.GetValueAsync();
            Assert.IsTrue(getEmployeesCountQuery.RequestUri.OriginalString.EndsWith("/Company/Microsoft.Test.OData.Services.ODataWCFService.GetEmployeesCount()"));
            Assert.IsTrue(count > 0);

            //Function Bound on Singleton
            var company5 = await TestClientContext.Company.GetValueAsync();
            getEmployeesCountQuery = company5.GetEmployeesCount();
            count = await getEmployeesCountQuery.GetValueAsync();
            Assert.IsTrue(getEmployeesCountQuery.RequestUri.OriginalString.EndsWith("/Company/Microsoft.Test.OData.Services.ODataWCFService.GetEmployeesCount()"));
            Assert.IsTrue(count > 0);

            //Query Action bound on Navigation Property, also on baseType
            var getHomeAddressQuery = TestClientContext.Company.VipCustomer.GetHomeAddress();
            var homeAddress = await getHomeAddressQuery.GetValueAsync();
            Assert.IsTrue(getHomeAddressQuery.RequestUri.OriginalString.EndsWith("/Company/VipCustomer/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()"));
            Assert.IsNotNull(homeAddress);
        }

        [TestMethod, Asynchronous]
        public async Task FunctionBoundOnContainedSingleNavigation()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            var getActualAmountFunction = TestClientContext.Accounts.ByKey(new Dictionary<string, object> { { "AccountID", 101 } }).MyGiftCard.GetActualAmount(1);
            var amount = await getActualAmountFunction.GetValueAsync();
            Assert.IsTrue(getActualAmountFunction.RequestUri.OriginalString.EndsWith("/Accounts(101)/MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=1.0)"));
        }
    }

    [TestClass]
    public class AsynchronousDelayQueryTests2 : EndToEndTestBase
    {
        InMemoryEntitiesPlus TestClientContext;

        public AsynchronousDelayQueryTests2()
            : base(ServiceDescriptors.ODataWCFServicePlusDescriptor)
        {
        }

        [TestMethod, Asynchronous]
        public async Task DelayQueryOnDerivedSingleton()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntitiesPlus>().Context;
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            //Query Singleton
            var publicCompanyQuery = TestClientContext.PublicCompanyPlus;
            var publicCompany = await publicCompanyQuery.GetValueAsync();
            Assert.IsTrue(publicCompanyQuery.RequestUri.OriginalString.EndsWith("/PublicCompany"));
            Assert.IsNotNull(publicCompany);

            //Query Property On Derived Singleton
            var stockExchangeQuery = TestClientContext.PublicCompanyPlus.Select(p => (p as PublicCompanyPlus).StockExchangePlus);
            var stockExchange = await stockExchangeQuery.GetValueAsync();
            Assert.IsTrue(stockExchangeQuery.RequestUri.OriginalString.EndsWith("/PublicCompany/Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/StockExchange"));
            Assert.IsNotNull(stockExchange);

            //Query Navigation Property on Dervied Singleton
            var assetsQuery = TestClientContext.PublicCompanyPlus.CastToPublicCompanyPlus().AssetsPlus;
            var assets = await assetsQuery.ExecuteAsync();
            Assert.IsTrue(assetsQuery.RequestUri.OriginalString.EndsWith("/PublicCompany/Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/Assets"));
            Assert.IsNotNull(assets);

            //Filter Navigation Property on Dervied Singleton
            assetsQuery = TestClientContext.PublicCompanyPlus.CastToPublicCompanyPlus().AssetsPlus.Where(a => a.NamePlus != "Temp") as DataServiceQuery<AssetPlus>;
            assets = await assetsQuery.ExecuteAsync();
            Assert.IsTrue(assetsQuery.RequestUri.OriginalString.EndsWith("/PublicCompany/Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/Assets?$filter=Name ne 'Temp'"));
            Assert.IsNotNull(assets);

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
            Assert.IsTrue(publicCompany2Query.RequestUri.OriginalString.EndsWith(
                "/PublicCompany?$expand=Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/LabourUnion&$select=Address,Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/StockExchange"));
            Assert.IsNotNull(publicCompany2.AddressPlus);
            Assert.IsNotNull(publicCompany2.LabourUnionPlus);
            Assert.IsNotNull(publicCompany2.StockExchangePlus);

            //Expand Navigation Property on Derived Singleton
            var publicCompany3Query = TestClientContext.PublicCompanyPlus.Expand(p => (p as PublicCompanyPlus).LabourUnionPlus);
            publicCompany = await publicCompany3Query.GetValueAsync();
            Assert.IsTrue(publicCompany3Query.RequestUri.OriginalString.EndsWith("/PublicCompany?$expand=Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/LabourUnion"));
            Assert.IsNotNull((publicCompany as PublicCompanyPlus).LabourUnionPlus);

            //Action Bound on Navigation Property on Derived Singleton, return void
            DataServiceActionQuery changeLabourUnionAction = TestClientContext.PublicCompanyPlus.CastToPublicCompanyPlus().LabourUnionPlus.ChangeLabourUnionNamePlus("changedLabourUnion");
            await changeLabourUnionAction.ExecuteAsync();
            Assert.IsTrue(changeLabourUnionAction.RequestUri.OriginalString.EndsWith("/PublicCompany/Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/LabourUnion/Microsoft.Test.OData.Services.ODataWCFService.ChangeLabourUnionName"));
            var labourUnion = (await TestClientContext.PublicCompanyPlus.Select(p => (p as PublicCompanyPlus).LabourUnionPlus).GetValueAsync()).NamePlus;
            Assert.AreEqual("changedLabourUnion", labourUnion);
        }

        [TestMethod, Asynchronous]
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
            Assert.IsTrue(products.Count() > 0);
            Assert.IsTrue(getAllProductsFunction.RequestUri.OriginalString.EndsWith("/GetAllProducts()"));
            double unitPrice = products.First().UnitPricePlus;

            //GetEnumerator
            var discountAction = getAllProductsFunction.DiscountPlus(50);
            foreach (var p in await discountAction.ExecuteAsync())
            {
                Assert.AreEqual(unitPrice * 50 / 100, p.UnitPricePlus);
                break;
            }
            Assert.IsTrue(discountAction.RequestUri.OriginalString.EndsWith("/GetAllProducts()/Microsoft.Test.OData.Services.ODataWCFService.Discount"));

            //Filter after Bound FunctionImport
            var filterAllProducts = getAllProductsFunction.Where(p => p.SkinColorPlus == ColorPlus.RedPlus) as DataServiceQuery<ProductPlus>;
            foreach (var p in await filterAllProducts.ExecuteAsync())
            {
                Assert.AreEqual(ColorPlus.RedPlus, p.SkinColorPlus);
            }
            Assert.IsTrue(filterAllProducts.RequestUri.OriginalString.EndsWith("/GetAllProducts()?$filter=SkinColor eq Microsoft.Test.OData.Services.ODataWCFService.Color'Red'"));

            //FunctionImport, return collection of primitivetype
            var getBossEmailsFunction = TestClientContext.GetBossEmailsPlus(0, 10);
            var emails = await getBossEmailsFunction.ExecuteAsync();
            Assert.IsTrue(getBossEmailsFunction.RequestUri.OriginalString.EndsWith("/GetBossEmails(start=0,count=10)"));

            //FunctionImport, return Collection of primitivetype with enum parameter
            //Fail now
            var productsNameQuery = TestClientContext.GetProductsByAccessLevelPlus(AccessLevelPlus.ReadPlus | AccessLevelPlus.ExecutePlus);
            var productsName = await productsNameQuery.ExecuteAsync();

            Assert.IsTrue(Uri.UnescapeDataString(productsNameQuery.RequestUri.OriginalString).EndsWith("/GetProductsByAccessLevel(accessLevel=Microsoft.Test.OData.Services.ODataWCFService.AccessLevel'Read,Execute')"));
            Assert.IsTrue(productsName.Count() > 0);
        }

        [TestMethod, Asynchronous]
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
            Assert.IsTrue(discountAction.RequestUri.OriginalString.EndsWith("/Discount"));

            var ar3 = queryProduct.BeginExecute(null, null).EnqueueWait(this);
            product = queryProduct.EndExecute(ar3).Single();
            Assert.AreEqual(originalPrice * 50 / 100, product.UnitPricePlus);
        }

        [TestMethod, Asynchronous]
        public async Task DelayQueryOnActionImport45()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntitiesPlus>().Context;
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            var product = (await (TestClientContext.ProductsPlus.Take(1) as DataServiceQuery<ProductPlus>).ExecuteAsync()).Single();
            double originalPrice = product.UnitPricePlus;

            //ActionImport return void
            var discountAction = TestClientContext.DiscountPlus(50);
            await discountAction.ExecuteAsync();
            Assert.IsTrue(discountAction.RequestUri.OriginalString.EndsWith("/Discount"));

            product = (await (TestClientContext.ProductsPlus.Take(1) as DataServiceQuery<ProductPlus>).ExecuteAsync()).Single();
            Assert.AreEqual(originalPrice * 50 / 100, product.UnitPricePlus);
        }

        [TestMethod]
        public void FunctionAndFunctionImportInFilterAndOrderBy()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntitiesPlus>().Context;
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            //FunctionImport in query option
            var queryAccount = TestClientContext.AccountsPlus.Where(a => a.MyGiftCardPlus.GetActualAmountPlus(0.5).GetValueAsync().Result > 1) as DataServiceQuery<AccountPlus>;
            Assert.IsTrue(queryAccount.RequestUri.OriginalString.EndsWith("/Accounts?$filter=MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.5) gt 1.0"));
            //queryAccount.Execute();

            queryAccount = TestClientContext.AccountsPlus.Where(a => 1 != a.MyGiftCardPlus.GetActualAmountPlus(0.5).GetValueAsync().Result) as DataServiceQuery<AccountPlus>;
            Assert.IsTrue(queryAccount.RequestUri.OriginalString.EndsWith("/Accounts?$filter=1.0 ne MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.5)"));
            //queryAccount.Execute();

            var queryPeople = TestClientContext.PeoplePlus.Where(p => p.GetHomeAddressPlus().GetValueAsync().Result.FamilyNamePlus != "name") as DataServiceQuery<PersonPlus>;
            Assert.IsTrue(queryPeople.RequestUri.OriginalString.EndsWith("/People?$filter=$it/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()/FamilyName ne 'name'"));
            //queryPeople.Execute();

            queryPeople = TestClientContext.PeoplePlus.Where(p => "name" != p.GetHomeAddressPlus().GetValueAsync().Result.FamilyNamePlus) as DataServiceQuery<PersonPlus>;
            Assert.IsTrue(queryPeople.RequestUri.OriginalString.EndsWith("/People?$filter='name' ne $it/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()/FamilyName"));
            //queryPeople.Execute();

            var queryProducts = TestClientContext.ProductsPlus.Where(p => p.GetProductDetailsPlus(2).Count() > 1) as DataServiceQuery<ProductPlus>;
            Assert.IsTrue(queryProducts.RequestUri.OriginalString.EndsWith("/Products?$filter=$it/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=2)/$count gt 1"));
            //queryProducts.Execute();

            queryProducts = TestClientContext.ProductsPlus.Where(p => 1 <= p.GetProductDetailsPlus(2).Count()) as DataServiceQuery<ProductPlus>;
            Assert.IsTrue(queryProducts.RequestUri.OriginalString.EndsWith("/Products?$filter=1 le $it/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=2)/$count"));
            //queryAccount.Execute();

            queryAccount = TestClientContext.AccountsPlus.OrderBy(a => a.MyGiftCardPlus.GetActualAmountPlus(0.5).GetValueAsync().Result) as DataServiceQuery<AccountPlus>;
            Assert.IsTrue(queryAccount.RequestUri.OriginalString.EndsWith("/Accounts?$orderby=MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.5)"));
            //queryAccount.Execute();

            queryAccount = TestClientContext.AccountsPlus.OrderByDescending(a => a.MyGiftCardPlus.GetActualAmountPlus(0.5).GetValueAsync().Result) as DataServiceQuery<AccountPlus>;
            Assert.IsTrue(queryAccount.RequestUri.OriginalString.EndsWith("/Accounts?$orderby=MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.5) desc"));
            //queryAccount.Execute();

            queryPeople = TestClientContext.PeoplePlus.OrderBy(p => p.GetHomeAddressPlus().GetValueAsync().Result.FamilyNamePlus) as DataServiceQuery<PersonPlus>;
            Assert.IsTrue(queryPeople.RequestUri.OriginalString.EndsWith("/People?$orderby=$it/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()/FamilyName"));
            //queryPeople.Execute();

            queryPeople = TestClientContext.PeoplePlus.OrderByDescending(p => p.GetHomeAddressPlus().GetValueAsync().Result.FamilyNamePlus) as DataServiceQuery<PersonPlus>;
            Assert.IsTrue(queryPeople.RequestUri.OriginalString.EndsWith("/People?$orderby=$it/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()/FamilyName desc"));
            //queryPeople.Execute();
        }
    }
}
