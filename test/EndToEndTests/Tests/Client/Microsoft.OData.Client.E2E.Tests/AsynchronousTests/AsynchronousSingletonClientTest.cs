//---------------------------------------------------------------------
// <copyright file="AsynchronousSingletonClientTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.AsyncRequestTests;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Xunit;
using Company = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Company;
using CompanyCategory = Microsoft.OData.E2E.TestCommon.Common.Client.Default.CompanyCategory;
using Department = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Department;
using PublicCompany = Microsoft.OData.E2E.TestCommon.Common.Client.Default.PublicCompany;
using Customer = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Customer;
using Asset = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Asset;

namespace Microsoft.OData.Client.E2E.Tests.AsynchronousTests;


public class AsynchronousSingletonClientTest : AsynchronousEndToEndTestBase<AsynchronousSingletonClientTest.TestsStartup>
{
    private readonly Uri _baseUri;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(MetadataController), typeof(AsynchronousSingletonClientTestsController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel(), new DefaultODataBatchHandler()));
        }
    }

    public AsynchronousSingletonClientTest(TestWebApplicationFactory<TestsStartup> fixture)
        : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");
    }

    [Fact]
    public void SingletonQueryUpdatePropertyClientTest()
    {
        var context = CreateWrappedContext();
        context.MergeOption = MergeOption.OverwriteChanges;

        //Query Singleton            
        var queryCompany = context.Company as DataServiceQuerySingle<Company>;
        var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
        var company = queryCompany.EndGetValue(queryCompanyAr);
        Assert.True(company != null);

        //Update Singleton Property and Verify
        company.CompanyCategory = CompanyCategory.Communication;
        company.Name = "UpdatedName";
        company.Address.City = "UpdatedCity";
        context.UpdateObject(company);
        var updateCompanyAr = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(updateCompanyAr);

        // Query Singleton Property - Select
        var queryCompanyCategory = context.Company.Select(c => c.CompanyCategory) as DataServiceQuerySingle<CompanyCategory>;
        CompanyCategory? companyCategory = CompanyCategory.Others;
        var queryCompanyCategoryAr = queryCompanyCategory.BeginGetValue(null, null).EnqueueWait(this);
        companyCategory = queryCompanyCategory.EndGetValue(queryCompanyCategoryAr);
        Assert.True(companyCategory == CompanyCategory.Communication);

        var queryCity = context.CreateQuery<string>("Company/Address/City");
        var queryCityAr = queryCity.BeginExecute(null, null);
        var city = queryCity.EndExecute(queryCityAr).Single();
        this.EnqueueCallback(() => Assert.True(city == "UpdatedCity"));

        var queryNameAr = context.BeginExecute<string>(new Uri("Company/Name", UriKind.Relative), null, null).EnqueueWait(this);
        var name = context.EndExecute<string>(queryNameAr).Single();
        Assert.True(name == "UpdatedName");

        //Projection with properties - Select
        this.TestCompleted = false;
        queryCompany = context.Company.Select(c => new Company { CompanyID = c.CompanyID, Address = c.Address, Name = c.Name }) as DataServiceQuerySingle<Company>;
        queryCompanyAr = queryCompany.BeginGetValue(null, null);
        company = queryCompany.EndGetValue(queryCompanyAr);
        Assert.True(company.Name == "UpdatedName");
        Assert.True(company.Departments.Count == 0);
        this.EnqueueTestComplete();

        CreateWrappedContext();
    }

    [Fact]
    public void SingletonQueryUpdateNavigationCollectionPropertyClientTest()
    {
        var context = CreateWrappedContext();
        context.MergeOption = MergeOption.OverwriteChanges;

        var queryCompany = context.Company as DataServiceQuerySingle<Company>;
        var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
        var company = queryCompany.EndGetValue(queryCompanyAr);

        //Load Navigation Property
        var loadDeparments = context.BeginLoadProperty(company, "Departments", null, null).EnqueueWait(this);
        context.EndLoadProperty(loadDeparments);
        Assert.True(company.Departments.Count > 0);

        //Add Navigation Property - Collection
        Random rand = new Random();
        int tmpDepartmentID = rand.Next();
        Department? department = new Department()
        {
            DepartmentID = tmpDepartmentID,
            Name = "ID" + tmpDepartmentID
        };
        context.AddToDepartments(department);
        context.AddLink(company, "Departments", department);
        var addDepartmentsLink = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(addDepartmentsLink);

        //Projection with Navigation properties - Select
        var selectCompany = context.Company.Select(c => new Company { CompanyID = c.CompanyID, Departments = c.Departments }) as DataServiceQuerySingle<Company>;
        var selectCompanyAr = selectCompany.BeginGetValue(null, null).EnqueueWait(this);
        var projectedCompany = selectCompany.EndGetValue(selectCompanyAr);
        Assert.True(projectedCompany.Departments.Any(c => c.DepartmentID == tmpDepartmentID));

        //Update EntitySet's Navigation Property - Singleton 
        context.SetLink(department, "Company", company);
        var ar4 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar4);

        //Query(Expand) EntitySet's Navigation Property - Singleton
        var queryDepartment = context.Departments.Expand(d => d.Company).Where(d => d.DepartmentID == tmpDepartmentID) as DataServiceQuery<Department>;
        var queryDepartmentAr = queryDepartment.BeginExecute(null, null).EnqueueWait(this);
        department = queryDepartment.EndExecute(queryDepartmentAr).SingleOrDefault();
        Assert.True(department.Company.CompanyID == company.CompanyID);

        //Delete Navigation Property - EntitySet
        context.DeleteLink(company, "Departments", department);
        var deleteLinkAr = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(deleteLinkAr);

        //Expand Navigation Property - EntitySet
        queryCompany = context.Company.Expand("Departments");
        queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
        company = queryCompany.EndGetValue(queryCompanyAr);
        Assert.True(!company.Departments.Any(c => c.DepartmentID == tmpDepartmentID));

        this.EnqueueTestComplete();

        CreateWrappedContext();
    }

    [Fact]
    public void SingletonQueryUpdateNavigationSingletonPropertyClientTest()
    {
        var context = CreateWrappedContext();
        context.MergeOption = MergeOption.OverwriteChanges;

        var queryCompany = context.Company as DataServiceQuerySingle<Company>;
        var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
        var company = queryCompany.EndGetValue(queryCompanyAr);

        //Query Singleton again with Execute
        var queryVipCustomerAr = context.BeginExecute<Customer>(new Uri("VipCustomer", UriKind.Relative), null, null).EnqueueWait(this);
        var vipCustomer = context.EndExecute<Customer>(queryVipCustomerAr).Single();
        Assert.True(vipCustomer != null);

        //Update Singleton's Navigation property - Singleton
        vipCustomer.City = "UpdatedCity";
        context.UpdateRelatedObject(company, "VipCustomer", vipCustomer);
        var ar6 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar6);

        //Expand Navigation Property - Singleton
        company.VipCustomer = null;
        queryCompany = context.Company.Expand(c => c.VipCustomer);
        queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
        company = queryCompany.EndGetValue(queryCompanyAr);
        Assert.True(company.VipCustomer.City == "UpdatedCity");

        //Update Navigation Property - Delete the Singleton navigation
        context.SetLink(company, "VipCustomer", null);
        var ar7 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar7);

        //Expand Navigation Property using name- Singleton
        company.VipCustomer = null;
        queryCompany = context.Company.Expand("VipCustomer");
        queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
        company = queryCompany.EndGetValue(queryCompanyAr);
        Assert.True(company.VipCustomer == null);

        //Update Navigation Property - Singleton
        context.SetLink(company, "VipCustomer", vipCustomer);
        var ar8 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar8);

        //LoadProperty Navigation Property - Singleton
        company.VipCustomer = null;
        var ar13 = context.BeginLoadProperty(company, "VipCustomer", null, null).EnqueueWait(this);
        context.EndLoadProperty(ar13);
        Assert.True(company.VipCustomer != null);

        //Expand Navigation Property - Singleton
        company.VipCustomer = null;
        queryCompany = context.Company.Expand(c => c.VipCustomer);
        queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
        company = queryCompany.EndGetValue(queryCompanyAr);
        Assert.True(company.VipCustomer != null);

        //Query Singleton's Navigation Property - Singleton
        queryCompany = context.Company.Select(c => new Company { CompanyID = c.CompanyID, VipCustomer = c.VipCustomer }) as DataServiceQuerySingle<Company>;
        queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
        company = queryCompany.EndGetValue(queryCompanyAr);
        Assert.True(company.VipCustomer != null);

        this.EnqueueTestComplete();

        CreateWrappedContext();
    }

    [Fact]
    public void SingletonQueryUpdateNavigationSingleEntityPropertyClientTest()
    {
        var context = CreateWrappedContext();
        context.MergeOption = MergeOption.OverwriteChanges;

        Company? company = null;
        var queryCompany = context.Company.Expand("CoreDepartment");
        var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
        company = queryCompany.EndGetValue(queryCompanyAr);
        Assert.True(company.CoreDepartment != null);

        //Single Entity
        company.CoreDepartment = null;
        var ar15 = context.BeginLoadProperty(company, "CoreDepartment", null, null).EnqueueWait(this);
        context.EndLoadProperty(ar15);
        Assert.True(company.CoreDepartment != null);

        Random rand = new Random();
        int tmpCoreDepartmentID = rand.Next();
        Department coreDepartment = new Department()
        {
            DepartmentID = tmpCoreDepartmentID,
            Name = "ID" + tmpCoreDepartmentID
        };

        context.AddToDepartments(coreDepartment);
        context.AddLink(company, "Departments", coreDepartment);
        var ar1 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar1);

        //Update Navigation Property - Single Entity
        context.SetLink(company, "CoreDepartment", coreDepartment);
        var ar3 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar3);

        //Projection with Navigation properties - Select
        queryCompany = context.Company.Select(c => new Company { CompanyID = c.CompanyID, CoreDepartment = c.CoreDepartment }) as DataServiceQuerySingle<Company>;
        queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
        company = queryCompany.EndGetValue(queryCompanyAr);
        Assert.True(company.CoreDepartment.DepartmentID == tmpCoreDepartmentID);
        this.EnqueueTestComplete();

        CreateWrappedContext();
    }

    [Fact]
    public void UpdateDerivedTypePropertyClientTest()
    {
        var context = CreateWrappedContext();
        context.MergeOption = MergeOption.OverwriteChanges;

        //Query Singleton
        var queryCompany = context.PublicCompany;
        var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
        var company = queryCompany.EndGetValue(queryCompanyAr);

        //Update DerivedType Property and Verify
        var publicCompany = company as PublicCompany;
        Assert.NotNull(publicCompany);
        publicCompany.Name = "UpdatedName";
        publicCompany.StockExchange = "Updated StockExchange";
        context.UpdateObject(publicCompany);
        var updateCompanyAr = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(updateCompanyAr);

        //Query Singleton Property - Select            
        var queryName = context.PublicCompany.Select(c => c.Name) as DataServiceQuerySingle<string>;
        var queryNameAr = queryName.BeginGetValue(null, null).EnqueueWait(this);
        var name = queryName.EndGetValue(queryNameAr);
        Assert.True(name == "UpdatedName");

        //Projection with properties of DerivedType
        var queryStockExchange = context.PublicCompany.Select(c => (c as PublicCompany).StockExchange) as DataServiceQuerySingle<string>;
        var queryStockExchangeAr = queryStockExchange.BeginGetValue(null, null).EnqueueWait(this);
        var stockExchange = queryStockExchange.EndGetValue(queryStockExchangeAr);
        Assert.True(stockExchange == "Updated StockExchange");

        //Projection with properties - Select
        var queryPublicCompany = context.PublicCompany.Select(c =>
            new PublicCompany
            {
                CompanyID = c.CompanyID,
                Name = c.Name,
                StockExchange = (c as PublicCompany).StockExchange
            }) as DataServiceQuerySingle<PublicCompany>;

        var queryPublicCompanyAr = queryPublicCompany.BeginGetValue(null, null).EnqueueWait(this);
        publicCompany = queryPublicCompany.EndGetValue(queryPublicCompanyAr);
        Assert.True(publicCompany.Name == "UpdatedName");
        Assert.True(publicCompany.StockExchange == "Updated StockExchange");

        this.EnqueueTestComplete();

        CreateWrappedContext();
    }

    [Fact]
    public void UpdateDerivedTypeNavigationOfContainedCollection()
    {
        var context = CreateWrappedContext();
        context.MergeOption = MergeOption.OverwriteChanges;

        //Query Singleton
        var queryCompany = context.PublicCompany as DataServiceQuerySingle<Company>;
        var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
        var company = queryCompany.EndGetValue(queryCompanyAr);

        //Load Navigation Property
        //Collection            
        var loadAssetsAr = context.BeginLoadProperty(company, "Assets", null, null).EnqueueWait(this);
        context.EndLoadProperty(loadAssetsAr);
        Assert.True((company as PublicCompany)?.Assets != null);

        //Add Contained Navigation Property - Collection of derived type
        Random rand = new Random();
        int tmpAssertId = rand.Next();
        Asset tmpAssert = new Asset()
        {
            AssetID = tmpAssertId,
            Name = tmpAssertId + "Name",
            Number = tmpAssertId
        };

        context.AddRelatedObject(company, "Assets", tmpAssert);
        var addRelatedObjectAr = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(addRelatedObjectAr);

        //Query contained Navigation Property - Collection of derived type
        queryCompany = context.PublicCompany.Expand(c => (c as PublicCompany).Assets) as DataServiceQuerySingle<Company>;
        var queryCompanyAr1 = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
        company = queryCompany.EndGetValue(queryCompanyAr1);
        Assert.True((company as PublicCompany)?.Assets.Any(a => a.AssetID == tmpAssertId));

        //Delete contained Navigation Property - Collection of derived type
        context.DeleteObject(tmpAssert);
        var deleteObjectAr = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(deleteObjectAr);

        //Expand contained Navigation property - Collection of derived type
        queryCompany = context.PublicCompany.Expand(c => (c as PublicCompany).Assets) as DataServiceQuerySingle<Company>;
        queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
        company = queryCompany.EndGetValue(queryCompanyAr);
        Assert.True(!(company as PublicCompany)?.Assets.Any(a => a.AssetID == tmpAssertId));

        this.EnqueueTestComplete();

        CreateWrappedContext();
    }

    [Fact]
    public void UpdateDerivedTypeNavigationOfContainedSingleEntity()
    {
        var context = CreateWrappedContext();
        context.MergeOption = MergeOption.OverwriteChanges;

        //Query Singleton
        var queryCompany = context.PublicCompany;
        var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
        var company = queryCompany.EndGetValue(queryCompanyAr);

        //Load Navigation Property 
        //Single Entity
        var loadClubAr = context.BeginLoadProperty(company, "Club", null, null).EnqueueWait(this);
        context.EndLoadProperty(loadClubAr);
        Assert.True((company as PublicCompany)?.Club != null);

        //Updated Contained Navigation Property - SingleEntity of derived type
        var club = (company as PublicCompany)?.Club;
        Assert.NotNull(club);
        club.Name = "UpdatedClubName";
        context.UpdateRelatedObject(company, "Club", club);
        var updateRelatedObjectAr = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(updateRelatedObjectAr);

        //Projection with Contained Navigation Property - Single Entity of derived type
        var queryPublicCompany = context.PublicCompany.Select(c =>
            new PublicCompany { CompanyID = c.CompanyID, Club = (c as PublicCompany).Club }) as DataServiceQuerySingle<PublicCompany>;
        var queryPublicCompany2Ar = queryPublicCompany.BeginGetValue(null, null).EnqueueWait(this);
        var publicCompany = queryPublicCompany.EndGetValue(queryPublicCompany2Ar);
        Assert.True(publicCompany.Club.Name == "UpdatedClubName");

        this.EnqueueTestComplete();

        CreateWrappedContext();
    }

    [Fact]
    public void DerivedTypeSingletonClientTest()
    {
        var context = CreateWrappedContext();
        context.MergeOption = MergeOption.OverwriteChanges;

        //Query Singleton
        var queryCompany = context.PublicCompany as DataServiceQuerySingle<Company>;
        var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
        var company = queryCompany.EndGetValue(queryCompanyAr);

        //Singleton
        var loadLabourUnionAr = context.BeginLoadProperty(company, "LabourUnion", null, null).EnqueueWait(this);
        context.EndLoadProperty(loadLabourUnionAr);
        Assert.True((company as PublicCompany)?.LabourUnion != null);

        //Expand Navigation Property - Singleton
        queryCompany = context.PublicCompany.Expand(c => (c as PublicCompany).Club) as DataServiceQuerySingle<Company>;
        queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
        company = queryCompany.EndGetValue(queryCompanyAr);
        Assert.True((company as PublicCompany)?.Club != null);

        //Update Navigation property of derived Type - Singleton
        var labourUnion = (company as PublicCompany)?.LabourUnion;
        labourUnion.Name = "UpdatedLabourUnionName";
        context.UpdateRelatedObject(company, "LabourUnion", labourUnion);
        var updateRelatedObjectAr = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(updateRelatedObjectAr);

        //Projecton with Navigation property - singleton of derived type.            
        var queryPublicCompany = context.PublicCompany.Select(c =>
            new PublicCompany { CompanyID = c.CompanyID, LabourUnion = (c as PublicCompany).LabourUnion }) as DataServiceQuerySingle<PublicCompany>;
        var queryPublicCompanyAr = queryPublicCompany.BeginGetValue(null, null).EnqueueWait(this);
        var publicCompany = queryPublicCompany.EndGetValue(queryPublicCompanyAr);
        Assert.True(publicCompany.LabourUnion != null);

        this.EnqueueTestComplete();
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
        var actionUri = new Uri(_baseUri + "asynchronoussingletonclienttests/Default.ResetDefaultDataSource", UriKind.Absolute);
        context.Execute(actionUri, "POST");
    }

    #endregion
}
