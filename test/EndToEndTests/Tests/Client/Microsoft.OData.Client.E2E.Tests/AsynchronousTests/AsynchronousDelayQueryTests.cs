//---------------------------------------------------------------------
// <copyright file="AsynchronousDelayQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Asynchronous;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Xunit;
using AccessLevel = Microsoft.OData.E2E.TestCommon.Common.Client.Default.AccessLevel;
using Asset = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Asset;
using Color = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Color;
using Company = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Company;
using Employee = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Employee;
using Product = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Product;
using PublicCompany = Microsoft.OData.E2E.TestCommon.Common.Client.Default.PublicCompany;

namespace Microsoft.OData.Client.E2E.Tests.AsynchronousTests;

public class AsynchronousDelayQueryTests : AsynchronousEndToEndTestBase<AsynchronousDelayQueryTests.TestsStartup>
{
    private readonly Uri _baseUri;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(MetadataController), typeof(AsynchronousDelayQueryTestsController));
            
            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel(), new DefaultODataBatchHandler()));
        }
    }

    public AsynchronousDelayQueryTests(TestWebApplicationFactory<TestsStartup> fixture)
        : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");
    }

    [Fact]
    public async Task DelayQuery_OnEntitySet_Test()
    {
        // Arrange
        var context = CreateWrappedContext();
        context.MergeOption = MergeOption.OverwriteChanges;

        // Act & Assert
        //Post a Product
        var product = Product.CreateProduct(10001, 2.0f, 2, true, DateTimeOffset.Now);
        context.AddToProducts(product);
        await context.SaveChangesAsync();

        //Action Bound on EntitySet and return an EntitySet
        var discountAction = context.Products.Discount(50);
        Assert.EndsWith("/Products/Default.Discount", discountAction.RequestUri.OriginalString);
        var ar = discountAction.BeginExecute(null, null).EnqueueWait(this);
        discountAction.EndExecute(ar);

        //Query an Entity
        var queryProduct = context.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } });
        Assert.EndsWith("/Products(10001)", queryProduct.RequestUri.OriginalString);
        queryProduct.BeginGetValue(
            (ar1) =>
            {
                product = queryProduct.EndGetValue(ar1);
                Assert.Equal(1, product.UnitPrice);
            }, queryProduct).EnqueueWait(this);

        //Action Bound on Entity and return an Enum
        var expectedAccessLevel = AccessLevel.ReadWrite | AccessLevel.Execute;
        var accessLevelAction = context.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } }).AddAccessRight(expectedAccessLevel);
        Assert.EndsWith("/Products(10001)/Default.AddAccessRight", accessLevelAction.RequestUri.OriginalString);
        var ar2 = accessLevelAction.BeginGetValue(null, null).EnqueueWait(this);
        var accessLevel = accessLevelAction.EndGetValue(ar2);
        Assert.Equal(expectedAccessLevel, accessLevel);

        //Function Bound on Entity and return Collection of Entity
        var getProductDetailsFunction = context.Products.ByKey(new Dictionary<string, object> { { "ProductID", 10001 } }).GetProductDetails(1);
        var getRelatedProductAction = getProductDetailsFunction.ByKey(new Dictionary<string, object> { { "ProductID", 10001 }, { "ProductDetailID", 10001 } }).GetRelatedProduct();
        Assert.EndsWith("/Products(10001)/Default.GetProductDetails(count=1)", getProductDetailsFunction.RequestUri.OriginalString);
        Assert.EndsWith("/Products(10001)/Default.GetProductDetails(count=1)(ProductID=10001,ProductDetailID=10001)/Default.GetRelatedProduct()", getRelatedProductAction.RequestUri.OriginalString);

        var ar3 = getProductDetailsFunction.BeginExecute(null, null).EnqueueWait(this);
        var productDetails = getProductDetailsFunction.EndExecute(ar3);

        Assert.All(productDetails, pd => Assert.Equal(5, pd.ProductID));
    }

    [Fact]
    public async Task DelayQuery_OnSingleton_Test()
    {
        // Arrange
        var context = CreateWrappedContext();
        context.MergeOption = MergeOption.OverwriteChanges;

        // Act & Assert
        var company = await context.Company.GetValueAsync();
        Assert.NotNull(company);

        //LoadProperty
        await context.LoadPropertyAsync(company, "Departments");
        Assert.NotNull(company.Departments);

        //Query Property
        var name = await context.Company.Select(c => c.Name).GetValueAsync();
        Assert.Equal(company.Name, name);

        //Projection
        var companyQuery = context.Company.Select(c => new Company() { Name = c.Name, Address = c.Address });
        var company2 = await companyQuery.GetValueAsync();
        Assert.EndsWith("/Company?$select=Name,Address", companyQuery.RequestUri.OriginalString);
        Assert.NotNull(company2.Address);

        //Expand
        companyQuery = context.Company.Expand(c => c.VipCustomer.Company);
        var company3 = await companyQuery.GetValueAsync();
        Assert.EndsWith("/Company?$expand=VipCustomer($expand=Company)", companyQuery.RequestUri.OriginalString);
        Assert.NotNull(company);

        //Projection with Navigation
        companyQuery = context.Company.Select(c => new Company() { Name = c.Name, Address = c.Address, Departments = c.Departments });
        await companyQuery.GetValueAsync();
        Assert.EndsWith("/Company?$expand=Departments&$select=Name,Address", companyQuery.RequestUri.OriginalString);
        Assert.NotNull(company2.Address);

        //Query navigation property which is an collection
        var employeesQuery = context.Company.Employees;
        var employees = await employeesQuery.ExecuteAsync();
        Assert.EndsWith("/Company/Employees", employeesQuery.RequestUri.OriginalString);
        Assert.NotNull(employees);

        //Query navigation property which is from a singleton
        var coreDepartmentQuery = context.Company.CoreDepartment;
        var coreDepartment = await coreDepartmentQuery.GetValueAsync();
        Assert.EndsWith("/Company/CoreDepartment", coreDepartmentQuery.RequestUri.OriginalString);
        Assert.NotNull(coreDepartment);

        //QueryOption on navigation property
        employeesQuery = context.Company.Employees.Where(e => e.PersonID > 0) as DataServiceQuery<Employee>;
        employees = await employeesQuery.ExecuteAsync();
        Assert.EndsWith("/Company/Employees?$filter=PersonID gt 0", employeesQuery.RequestUri.OriginalString);
        Assert.NotNull(employees);

        //Function Bound on Singleton
        var getEmployeesCountQuery = context.Company.GetEmployeesCount();
        var count = await getEmployeesCountQuery.GetValueAsync();
        Assert.EndsWith("/Company/Default.GetEmployeesCount()", getEmployeesCountQuery.RequestUri.OriginalString);
        Assert.True(count > 0);
    }

    [Fact]
    public async Task FunctionBound_OnContainedSingleNavigation_Test()
    {
        // Arrange
        var context = CreateWrappedContext();

        // Act & Assert
        var getActualAmountFunction = context.Accounts.ByKey(new Dictionary<string, object> { { "AccountID", 101 } }).MyGiftCard.GetActualAmount(1);
        var amount = await getActualAmountFunction.GetValueAsync();
        Assert.EndsWith("/Accounts(101)/MyGiftCard/Default.GetActualAmount(bonusRate=1.0)", getActualAmountFunction.RequestUri.OriginalString);
    }

    [Fact]
    public async Task DelayQuery_OnDerivedSingleton_Test()
    {
        // Arrange
        var context = CreateWrappedContext();
        context.MergeOption = MergeOption.OverwriteChanges;

        // Act & Assert
        //Query Singleton
        var publicCompanyQuery = context.PublicCompany;
        var publicCompany = await publicCompanyQuery.GetValueAsync();
        Assert.EndsWith("/PublicCompany", publicCompanyQuery.RequestUri.OriginalString);
        Assert.NotNull(publicCompany);

        //Query Property On Derived Singleton
        var stockExchangeQuery = context.PublicCompany.Select(p => (p as PublicCompany).StockExchange);
        var stockExchange = await stockExchangeQuery.GetValueAsync();
        Assert.EndsWith("/PublicCompany/Microsoft.OData.E2E.TestCommon.Common.Server.Default.PublicCompany/StockExchange", stockExchangeQuery.RequestUri.OriginalString);
        Assert.NotNull(stockExchange);

        //Query Navigation Property on Derived Singleton
        var assetsQuery = context.PublicCompany.CastToPublicCompany().Assets;
        var assets = await assetsQuery.ExecuteAsync();
        Assert.EndsWith("/PublicCompany/Microsoft.OData.E2E.TestCommon.Common.Server.Default.PublicCompany/Assets", assetsQuery.RequestUri.OriginalString);
        Assert.NotNull(assets);

        //Filter Navigation Property on Derived Singleton
        assetsQuery = context.PublicCompany.CastToPublicCompany().Assets.Where(a => a.Name != "Temp") as DataServiceQuery<Asset>;
        assets = await assetsQuery.ExecuteAsync();
        Assert.EndsWith("/PublicCompany/Microsoft.OData.E2E.TestCommon.Common.Server.Default.PublicCompany/Assets?$filter=Name ne 'Temp'", assetsQuery.RequestUri.OriginalString);
        Assert.NotNull(assets);

        //Projection
        var publicCompany2Query = context.PublicCompany.Select(
            p =>
                new PublicCompany()
                {
                    Address = p.Address,
                    LabourUnion = (p as PublicCompany).LabourUnion,
                    StockExchange = (p as PublicCompany).StockExchange
                });
        var publicCompany2 = await publicCompany2Query.GetValueAsync();
        Assert.EndsWith("/PublicCompany?$expand=Microsoft.OData.E2E.TestCommon.Common.Server.Default.PublicCompany/LabourUnion&$select=Address,Microsoft.OData.E2E.TestCommon.Common.Server.Default.PublicCompany/StockExchange", publicCompany2Query.RequestUri.OriginalString);
        Assert.NotNull(publicCompany2.Address);
        Assert.NotNull(publicCompany2.LabourUnion);
        Assert.NotNull(publicCompany2.StockExchange);

        //Expand Navigation Property on Derived Singleton
        var publicCompany3Query = context.PublicCompany.Expand(p => (p as PublicCompany).LabourUnion);
        publicCompany = await publicCompany3Query.GetValueAsync();
        Assert.EndsWith("/PublicCompany?$expand=Microsoft.OData.E2E.TestCommon.Common.Server.Default.PublicCompany/LabourUnion", publicCompany3Query.RequestUri.OriginalString);
        Assert.NotNull((publicCompany as PublicCompany).LabourUnion);

        //Action Bound on Navigation Property on Derived Singleton, return void
        DataServiceActionQuery changeLabourUnionAction = context.PublicCompany.CastToPublicCompany().LabourUnion.ChangeLabourUnionName("changedLabourUnion");
        await changeLabourUnionAction.ExecuteAsync();
        Assert.EndsWith("/PublicCompany/Microsoft.OData.E2E.TestCommon.Common.Server.Default.PublicCompany/LabourUnion/Default.ChangeLabourUnionName", changeLabourUnionAction.RequestUri.OriginalString);
        var labourUnion = (await context.PublicCompany.Select(p => (p as PublicCompany).LabourUnion).GetValueAsync()).Name;
        Assert.Equal("changedLabourUnion", labourUnion);
    }

    [Fact]
    public async Task DelayQuery_OnFunctionImport_Test()
    {
        // Arrange
        var context = CreateWrappedContext();
        context.MergeOption = MergeOption.OverwriteChanges;

        // Act & Assert
        //Post an Product
        var product = Product.CreateProduct(10001, 2.0f, 2, true, DateTimeOffset.Now);
        product.SkinColor = Color.Red;
        product.UserAccess = AccessLevel.Execute | AccessLevel.Read;
        context.AddToProducts(product);
        await context.SaveChangesAsync();

        //FunctionImport, return collection of primitivetype
        var getBossEmailsFunction = context.GetBossEmails(0, 10);
        var emails = await getBossEmailsFunction.ExecuteAsync();
        Assert.EndsWith("/GetBossEmails(start=0,count=10)", getBossEmailsFunction.RequestUri.OriginalString);
        Assert.Empty(emails);

        //FunctionImport, return Collection of primitivetype with enum parameter
        var productsQuery = context.GetProductsByAccessLevel(AccessLevel.Read | AccessLevel.Execute);
        var productsCount = await productsQuery.GetValueAsync();
        Assert.EndsWith("/GetProductsByAccessLevel(accessLevel=Microsoft.OData.E2E.TestCommon.Common.Server.Default.AccessLevel'Read%2CExecute')", productsQuery.RequestUri.AbsoluteUri);
        Assert.Equal(1, productsCount);

        productsQuery = context.GetProductsByAccessLevel(AccessLevel.Execute | AccessLevel.Read);
        productsCount = await productsQuery.GetValueAsync();
        Assert.EndsWith("/GetProductsByAccessLevel(accessLevel=Microsoft.OData.E2E.TestCommon.Common.Server.Default.AccessLevel'Read%2CExecute')", productsQuery.RequestUri.AbsoluteUri);
        Assert.Equal(1, productsCount);
    }

    [Fact]
    public async Task DelayQuery_OnActionImport_Test()
    {
        // Arrange
        var context = CreateWrappedContext();
        context.MergeOption = MergeOption.OverwriteChanges;

        // Act & Assert
        var product = (await (context.Products.Take(1) as DataServiceQuery<Product>).ExecuteAsync()).First();
        double originalPrice = product.UnitPrice;

        //ActionImport return void
        var discountAction = context.Discount(50);
        await discountAction.ExecuteAsync();
        Assert.EndsWith("/Discount", discountAction.RequestUri.OriginalString);

        product = (await (context.Products.Take(1) as DataServiceQuery<Product>).ExecuteAsync()).First();
        Assert.Equal(originalPrice * 50 / 100, product.UnitPrice);
    }

    #region Private

    private Container CreateWrappedContext()
    {
        var context = new Container(_baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        ResetDefaultDataSource(context);

        return context;
    }

    private void ResetDefaultDataSource(Container context)
    {
        var actionUri = new Uri(_baseUri + "asynchronousdelayquerytests/Default.ResetDefaultDataSource", UriKind.Absolute);
        context.Execute(actionUri, "POST");
    }

    #endregion
}
