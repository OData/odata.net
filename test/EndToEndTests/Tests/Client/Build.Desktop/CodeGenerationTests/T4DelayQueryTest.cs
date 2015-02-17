//---------------------------------------------------------------------
// <copyright file="T4DelayQueryTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.CodeGenerationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReferencePlus;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class T4DelayQueryTest : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        public T4DelayQueryTest()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        [TestMethod]
        public void DelayQueryOnEntitySet()
        {
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;
            
            //Post an Product
            var product = Product.CreateProduct(10001, "10001", "2", 2.0f, 2, true);
            TestClientContext.AddToProducts(product);
            TestClientContext.SaveChanges();

            //Function Bound on EntitySet
            //TODO: Enable this support on server side
            var querySeniorEmployee = (TestClientContext.People.OfType<Employee>() as DataServiceQuery<Employee>).GetSeniorEmployees();
            Assert.IsTrue(querySeniorEmployee.RequestUri.OriginalString.EndsWith(
                "People/Microsoft.Test.OData.Services.ODataWCFService.Employee/Microsoft.Test.OData.Services.ODataWCFService.GetSeniorEmployees()"));
            var queryEmployeeAddress = querySeniorEmployee.GetHomeAddress();
            Assert.IsTrue(queryEmployeeAddress.RequestUri.OriginalString.EndsWith(
                "People/Microsoft.Test.OData.Services.ODataWCFService.Employee/Microsoft.Test.OData.Services.ODataWCFService.GetSeniorEmployees()/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()"));
            //var employee = querySeniorEmployee.GetValue();
            //Assert.IsNotNull(employee);

            //Action Bound on EntitySet
            var discountAction = TestClientContext.Products.Discount(50);
            Assert.IsTrue(discountAction.RequestUri.OriginalString.EndsWith("/Products/Microsoft.Test.OData.Services.ODataWCFService.Discount"));
            var products = discountAction.Execute();
            Assert.IsTrue(products.Count() > 0);

            //ByKey
            var queryProduct = TestClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } });
            product = queryProduct.GetValue();
            Assert.AreEqual(1, product.UnitPrice);
            Assert.IsTrue(queryProduct.RequestUri.OriginalString.EndsWith("/Products(10001)"));

            //Action Bound on Entity
            var expectedAccessLevel = AccessLevel.ReadWrite | AccessLevel.Execute;
            var accessLevelAction = TestClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } }).AddAccessRight(expectedAccessLevel);
            Assert.IsTrue(accessLevelAction.RequestUri.OriginalString.EndsWith("/Products(10001)/Microsoft.Test.OData.Services.ODataWCFService.AddAccessRight"));
            var accessLevel = accessLevelAction.GetValue();
            Assert.IsTrue(accessLevel.Value.HasFlag(expectedAccessLevel));

            //Function Bound on Entity and return Collection of Entity
            var getProductDetailsAction = TestClientContext.Products.ByKey(new Dictionary<string, object> { { "ProductID", 5 } }).GetProductDetails(1);
            Assert.IsTrue(getProductDetailsAction.RequestUri.OriginalString.EndsWith("/Products(5)/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=1)"));
            foreach (var pd in getProductDetailsAction)
            {
                //Check whether GetEnumerator works
                Assert.AreEqual(5, pd.ProductID);
            }

            //Composable Function
            //Won't execute since ODL doesn't support it now.
            var getRelatedProductAction = getProductDetailsAction.ByKey(new Dictionary<string, object> { { "ProductID", 5 }, { "ProductDetailID", 2 } }).GetRelatedProduct();
            Assert.IsTrue(getRelatedProductAction.RequestUri.OriginalString.EndsWith("/Products(5)/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=1)(ProductID=5,ProductDetailID=2)/Microsoft.Test.OData.Services.ODataWCFService.GetRelatedProduct()"));
            //getRelatedProductAction.GetValue();

            //Complex query option
            var queryProducts = TestClientContext.Products.Where(p => p.ProductID > 3)
                .Select(p => new Product() { Details = p.Details, Name = p.Name, ProductID = p.ProductID })
                .Skip(3).Take(2) as DataServiceQuery<Product>;
            Assert.IsTrue(queryProducts.RequestUri.OriginalString.EndsWith("/Products?$filter=ProductID gt 3&$skip=3&$top=2&$expand=Details&$select=Name,ProductID"));
            foreach (var p in queryProducts)
            {
                Assert.IsTrue(p.ProductID > 3);
            }
        }

        [TestMethod]
        public void DelayQueryOnSingleton()
        {
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            var company = TestClientContext.Company.GetValue();
            Assert.IsNotNull(company);

            //LoadProperty
            TestClientContext.LoadProperty(company, "Departments");
            Assert.IsNotNull(company.Departments);

            //Query Property
            var name = TestClientContext.Company.Select(c => c.Name).GetValue();
            Assert.AreEqual(company.Name, name);

            //Projection
            var companyQuery = TestClientContext.Company.Select(c => new Company() { Name = c.Name, Address = c.Address });
            var company2 = companyQuery.GetValue();
            Assert.IsTrue(companyQuery.RequestUri.OriginalString.EndsWith("/Company?$select=Name,Address"));
            Assert.IsNotNull(company2.Address);

            //Expand
            companyQuery = TestClientContext.Company.Expand(c => c.VipCustomer.Company);
            var company3 = companyQuery.GetValue();
            Assert.IsTrue(companyQuery.RequestUri.OriginalString.EndsWith("/Company?$expand=VipCustomer($expand=Company)"));
            Assert.IsNotNull(company);

            //Projection with Navigation
            companyQuery = TestClientContext.Company.Select(c => new Company() { Name = c.Name, Address = c.Address, Departments = c.Departments });
            companyQuery.GetValue();
            Assert.IsTrue(companyQuery.RequestUri.OriginalString.EndsWith("/Company?$expand=Departments&$select=Name,Address"));
            Assert.IsNotNull(company2.Address);

            //Query navigation property which is an collection
            var employeesQuery = TestClientContext.Company.Employees;
            var employees = employeesQuery.Execute();
            Assert.IsTrue(employeesQuery.RequestUri.OriginalString.EndsWith("/Company/Employees"));
            Assert.IsNotNull(employees);

            //Query Navigation Property which is an single entity
            var company4Query = TestClientContext.Company.VipCustomer.Company;
            var company4 = company4Query.GetValue();
            Assert.IsTrue(company4Query.RequestUri.OriginalString.EndsWith("/Company/VipCustomer/Company"));
            Assert.IsNotNull(company4Query);

            //Query navigation property which is from a singleton
            var coreDepartmentQuery = TestClientContext.Company.CoreDepartment;
            var coreDepartment = coreDepartmentQuery.GetValue();
            Assert.IsTrue(coreDepartmentQuery.RequestUri.OriginalString.EndsWith("/Company/CoreDepartment"));
            Assert.IsNotNull(coreDepartment);

            //QueryOption on navigation property
            employeesQuery = TestClientContext.Company.Employees.Where(e => e.PersonID > 0) as DataServiceQuery<Employee>;
            employees = employeesQuery.Execute();
            Assert.IsTrue(employeesQuery.RequestUri.OriginalString.EndsWith("/Company/Employees?$filter=PersonID gt 0"));
            Assert.IsNotNull(employees);

            //Function Bound on Singleton
            var getEmployeesCountQuery = TestClientContext.Company.GetEmployeesCount();
            var count = getEmployeesCountQuery.GetValue();
            Assert.IsTrue(getEmployeesCountQuery.RequestUri.OriginalString.EndsWith("/Company/Microsoft.Test.OData.Services.ODataWCFService.GetEmployeesCount()"));
            Assert.IsTrue(count > 0);

            //Function Bound on Singleton
            var company5 = TestClientContext.Company.GetValue();
            getEmployeesCountQuery = company.GetEmployeesCount();
            count = getEmployeesCountQuery.GetValue();
            Assert.IsTrue(getEmployeesCountQuery.RequestUri.OriginalString.EndsWith("/Company/Microsoft.Test.OData.Services.ODataWCFService.GetEmployeesCount()"));
            Assert.IsTrue(count > 0);

            //Query Action bound on Navigation Property
            var getHomeAddressQuery = TestClientContext.Company.VipCustomer.GetHomeAddress();
            var address = getHomeAddressQuery.GetValue();
            Assert.IsTrue(getHomeAddressQuery.RequestUri.OriginalString.EndsWith("/Company/VipCustomer/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()"));
            Assert.IsNotNull(address);
        }

        [TestMethod]
        public void FunctionBoundOnContainedSingleNavigation()
        {
            var getActualAmountFunction = TestClientContext.Accounts.ByKey(new Dictionary<string, object> { { "AccountID", 101 } }).MyGiftCard.GetActualAmount(1);
            getActualAmountFunction.GetValue();
            Assert.IsTrue(getActualAmountFunction.RequestUri.OriginalString.EndsWith("/Accounts(101)/MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=1.0)"));

            //Not supported by service
            getActualAmountFunction = TestClientContext.Accounts.ByKey(new Dictionary<string, object> { { "AccountID", 101 } }).MyGiftCard.GetActualAmount(null);
            Assert.IsTrue(getActualAmountFunction.RequestUri.OriginalString.EndsWith("/Accounts(101)/MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=null)"));
            //getActualAmountFunction.GetValue();            
        }
    }

    [TestClass]
    public class T4DelayQueryTest2 : ODataWCFServiceTestsBase<InMemoryEntitiesPlus>
    {
        public T4DelayQueryTest2()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        [TestMethod]
        public void DelayQueryOnDerivedSingleton()
        {
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            //Query Singleton
            var publicCompanyQuery = TestClientContext.PublicCompanyPlus;
            var publicCompany = publicCompanyQuery.GetValue();
            Assert.IsTrue(publicCompanyQuery.RequestUri.OriginalString.EndsWith("/PublicCompany"));
            Assert.IsNotNull(publicCompany);

            //Query Property On Derived Singleton
            var stockExchangeQuery = TestClientContext.PublicCompanyPlus.Select(p => (p as PublicCompanyPlus).StockExchangePlus);
            var stockExchange = stockExchangeQuery.GetValue();
            Assert.IsTrue(stockExchangeQuery.RequestUri.OriginalString.EndsWith("/PublicCompany/Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/StockExchange"));
            Assert.IsNotNull(stockExchange);

            //Query Navigation Property on Dervied Singleton
            var assetsQuery = TestClientContext.PublicCompanyPlus.CastToPublicCompanyPlus().AssetsPlus;
            var assets = assetsQuery.Execute();
            Assert.IsTrue(assetsQuery.RequestUri.OriginalString.EndsWith("/PublicCompany/Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/Assets"));
            Assert.IsNotNull(assets);

            //Filter Navigation Property on Dervied Singleton
            assetsQuery = TestClientContext.PublicCompanyPlus.CastToPublicCompanyPlus().AssetsPlus.Where(a => a.NamePlus != "Temp") as DataServiceQuery<AssetPlus>;
            assets = assetsQuery.Execute();
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
            var publicCompany2 = publicCompany2Query.GetValue();
            Assert.IsTrue(publicCompany2Query.RequestUri.OriginalString.EndsWith(
                "/PublicCompany?$expand=Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/LabourUnion&$select=Address,Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/StockExchange"));
            Assert.IsNotNull(publicCompany2.AddressPlus);
            Assert.IsNotNull(publicCompany2.LabourUnionPlus);
            Assert.IsNotNull(publicCompany2.StockExchangePlus);

            //Expand Navigation Property on Derived Singleton
            var publicCompany3Query = TestClientContext.PublicCompanyPlus.Expand(p => (p as PublicCompanyPlus).LabourUnionPlus);
            publicCompany = publicCompany3Query.GetValue();
            Assert.IsTrue(publicCompany3Query.RequestUri.OriginalString.EndsWith("/PublicCompany?$expand=Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/LabourUnion"));
            Assert.IsNotNull((publicCompany as PublicCompanyPlus).LabourUnionPlus);

            //Action Bound on Navigation Property on Derived Singleton
            DataServiceActionQuery changeLabourUnionAction = TestClientContext.PublicCompanyPlus.CastToPublicCompanyPlus().LabourUnionPlus.ChangeLabourUnionNamePlus("changedLabourUnion");
            changeLabourUnionAction.Execute();
            Assert.IsTrue(changeLabourUnionAction.RequestUri.OriginalString.EndsWith("/PublicCompany/Microsoft.Test.OData.Services.ODataWCFService.PublicCompany/LabourUnion/Microsoft.Test.OData.Services.ODataWCFService.ChangeLabourUnionName"));
            var labourUnion = TestClientContext.PublicCompanyPlus.Select(p => (p as PublicCompanyPlus).LabourUnionPlus).GetValue().NamePlus;
            Assert.AreEqual("changedLabourUnion", labourUnion);
        }

        [TestMethod]
        public void DelayQueryOnFunctionImport()
        {
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            //Post an Product
            var product = ProductPlus.CreateProductPlus(10001, "10001", "2", 2.0f, 2, true);
            product.SkinColorPlus = ColorPlus.RedPlus;
            product.UserAccessPlus = AccessLevelPlus.ExecutePlus | AccessLevelPlus.ReadPlus;
            TestClientContext.AddToProductsPlus(product);
            TestClientContext.SaveChanges();

            //FunctionImport
            var getAllProductsFunction = TestClientContext.GetAllProductsPlus();
            var products = getAllProductsFunction.ToList();
            Assert.IsTrue(products.Count() > 0);
            Assert.IsTrue(getAllProductsFunction.RequestUri.OriginalString.EndsWith("/GetAllProducts()"));
            double unitPrice = products.First().UnitPricePlus;

            //GetEnumerator
            var discountAction = getAllProductsFunction.DiscountPlus(50);
            foreach (var p in discountAction)
            {
                Assert.AreEqual(unitPrice * 50 / 100, p.UnitPricePlus);
                break;
            }
            Assert.IsTrue(discountAction.RequestUri.OriginalString.EndsWith("/GetAllProducts()/Microsoft.Test.OData.Services.ODataWCFService.Discount"));

            //Filter after FunctionImport
            var filterAllProducts = getAllProductsFunction.Where(p => p.SkinColorPlus == ColorPlus.RedPlus) as DataServiceQuery<ProductPlus>;

            foreach (var p in filterAllProducts)
            {
                Assert.AreEqual(ColorPlus.RedPlus, p.SkinColorPlus);
            }
            Assert.IsTrue(filterAllProducts.RequestUri.OriginalString.EndsWith("/GetAllProducts()?$filter=SkinColor eq Microsoft.Test.OData.Services.ODataWCFService.Color'Red'"));

            //FunctionImport, return collection of primitivetype
            var getBossEmailsFunction = TestClientContext.GetBossEmailsPlus(0, 10);
            var emails = getBossEmailsFunction.Execute();
            Assert.IsTrue(getBossEmailsFunction.RequestUri.OriginalString.EndsWith("/GetBossEmails(start=0,count=10)"));

            //FunctionImport, return Collection of primitivetype with enum parameter
            var productsNameQuery = TestClientContext.GetProductsByAccessLevelPlus(AccessLevelPlus.ReadPlus | AccessLevelPlus.ExecutePlus);
            var productsName = productsNameQuery.Execute();
            Assert.IsTrue(Uri.UnescapeDataString(productsNameQuery.RequestUri.OriginalString).EndsWith("/GetProductsByAccessLevel(accessLevel=Microsoft.Test.OData.Services.ODataWCFService.AccessLevel'Read,Execute')"));
            Assert.IsTrue(productsName.Count() > 0);
        }

        [TestMethod]
        public void DelayQueryOnActionImport()
        {
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            var product = (TestClientContext.ProductsPlus.Take(1) as DataServiceQuery<ProductPlus>).Execute().Single();
            double originalPrice = product.UnitPricePlus;

            //ActionImport return void
            var discountAction = TestClientContext.DiscountPlus(50);
            discountAction.Execute();
            Assert.IsTrue(discountAction.RequestUri.OriginalString.EndsWith("/Discount"));

            product = (TestClientContext.ProductsPlus.Take(1) as DataServiceQuery<ProductPlus>).Execute().Single();
            Assert.AreEqual(originalPrice * 50 / 100, product.UnitPricePlus);
        }

        [TestMethod]
        public void ByMultipleKey()
        {
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            var queryProductDetails = TestClientContext.ProductDetailsPlus.ByKey(new Dictionary<string, object> { { "ProductID", 6 }, { "ProductDetailID", 1 } });
            var productDetails = queryProductDetails.GetValue();
            Assert.IsTrue(queryProductDetails.RequestUri.OriginalString.EndsWith("/ProductDetails(ProductID=6,ProductDetailID=1)"));
            Assert.IsNotNull(productDetails);

            var queryRelatedProductDetails = TestClientContext.ProductDetailsPlus.ByKey(new Dictionary<string, object> { { "ProductID", 6 }, { "ProductDetailID", 1 } }).GetRelatedProductPlus();
            var relatedProduct = queryRelatedProductDetails.GetValue();
            Assert.IsTrue(queryRelatedProductDetails.RequestUri.OriginalString.EndsWith("/ProductDetails(ProductID=6,ProductDetailID=1)/Microsoft.Test.OData.Services.ODataWCFService.GetRelatedProduct()"));
            Assert.IsNotNull(relatedProduct);
        }

        [TestMethod]
        public void FunctionAndFunctionImportInFilterAndOrderBy()
        {
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            //FunctionImport in query option
            var queryAccount = TestClientContext.AccountsPlus.Where(a => a.MyGiftCardPlus.GetActualAmountPlus(0.5).GetValue() > 1) as DataServiceQuery<AccountPlus>;
            Assert.IsTrue(queryAccount.RequestUri.OriginalString.EndsWith("/Accounts?$filter=MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.5) gt 1.0"));
            //queryAccount.Execute();

            queryAccount = TestClientContext.AccountsPlus.Where(a => 1 != a.MyGiftCardPlus.GetActualAmountPlus(0.5).GetValue()) as DataServiceQuery<AccountPlus>;
            Assert.IsTrue(queryAccount.RequestUri.OriginalString.EndsWith("/Accounts?$filter=1.0 ne MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.5)"));

            var queryPeople = TestClientContext.PeoplePlus.Where(p => p.GetHomeAddressPlus().GetValue().FamilyNamePlus != "name") as DataServiceQuery<PersonPlus>;
            Assert.IsTrue(queryPeople.RequestUri.OriginalString.EndsWith("/People?$filter=$it/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()/FamilyName ne 'name'"));
            //queryPeople.Execute();

            queryPeople = TestClientContext.PeoplePlus.Where(p => "name" != p.GetHomeAddressPlus().GetValue().FamilyNamePlus) as DataServiceQuery<PersonPlus>;
            Assert.IsTrue(queryPeople.RequestUri.OriginalString.EndsWith("/People?$filter='name' ne $it/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()/FamilyName"));

            var queryProducts = TestClientContext.ProductsPlus.Where(p => p.GetProductDetailsPlus(2).Count() > 1) as DataServiceQuery<ProductPlus>;
            Assert.IsTrue(queryProducts.RequestUri.OriginalString.EndsWith("/Products?$filter=$it/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=2)/$count gt 1"));
            //queryProducts.Execute();

            queryProducts = TestClientContext.ProductsPlus.Where(p => 1 <= p.GetProductDetailsPlus(2).Count()) as DataServiceQuery<ProductPlus>;
            Assert.IsTrue(queryProducts.RequestUri.OriginalString.EndsWith("/Products?$filter=1 le $it/Microsoft.Test.OData.Services.ODataWCFService.GetProductDetails(count=2)/$count"));

            //FunctionImport in query option
            queryAccount = TestClientContext.AccountsPlus.OrderBy(a => a.MyGiftCardPlus.GetActualAmountPlus(0.5).GetValue()) as DataServiceQuery<AccountPlus>;
            Assert.IsTrue(queryAccount.RequestUri.OriginalString.EndsWith("/Accounts?$orderby=MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.5)"));
            //queryAccount.Execute();

            queryAccount = TestClientContext.AccountsPlus.OrderByDescending(a => a.MyGiftCardPlus.GetActualAmountPlus(0.5).GetValue()) as DataServiceQuery<AccountPlus>;
            Assert.IsTrue(queryAccount.RequestUri.OriginalString.EndsWith("/Accounts?$orderby=MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.5) desc"));

            queryPeople = TestClientContext.PeoplePlus.OrderBy(p => p.GetHomeAddressPlus().GetValue().FamilyNamePlus) as DataServiceQuery<PersonPlus>;
            Assert.IsTrue(queryPeople.RequestUri.OriginalString.EndsWith("/People?$orderby=$it/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()/FamilyName"));
            //queryPeople.Execute();

            queryPeople = TestClientContext.PeoplePlus.OrderByDescending(p => p.GetHomeAddressPlus().GetValue().FamilyNamePlus) as DataServiceQuery<PersonPlus>;
            Assert.IsTrue(queryPeople.RequestUri.OriginalString.EndsWith("/People?$orderby=$it/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress()/FamilyName desc"));
        }
    }
}
