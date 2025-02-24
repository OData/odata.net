//---------------------------------------------------------------------
// <copyright file="SingletonClientTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.Tests.SingletonTests.Server;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Xunit;
using Asset = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Asset;
using Company = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Company;
using CompanyCategory = Microsoft.OData.E2E.TestCommon.Common.Client.Default.CompanyCategory;
using Customer = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Customer;
using Department = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Department;
using PublicCompany = Microsoft.OData.E2E.TestCommon.Common.Client.Default.PublicCompany;

namespace Microsoft.OData.Client.E2E.Tests.SingletonTests.Tests;

public class SingletonClientTests : EndToEndTestBase<SingletonClientTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(SingletonClientTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
            {
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel());
                opt.RouteOptions.EnableNonParenthesisForEmptyParameterFunction = true;
            });
        }
    }

    public SingletonClientTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");

        _context = new Container(_baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        ResetDefaultDataSource();
    }

    #region Singleton Client Tests

    [Fact]
    public async Task SingletonClientTestAsync()
    {
        // Arrange
        var rand = new Random();
        var format = ODataFormat.Json;

        //Query Singleton
        _context.MergeOption = MergeOption.OverwriteChanges;
        var company = await _context.Company.GetValueAsync();
        Assert.NotNull(company);

        //Update Singleton Property and Verify
        company.CompanyCategory = CompanyCategory.Communication;
        company.Name = "UpdatedName";
        company.Address.City = "UpdatedCity";
        _context.UpdateObject(company);
        await _context.SaveChangesAsync(SaveChangesOptions.ReplaceOnUpdate);

        //Query Singleton Property - Select
        var companyCategory = await _context.Company.Select(c => c.CompanyCategory).GetValueAsync();
        Assert.Equal(CompanyCategory.Communication, companyCategory);

        var cities = await _context.CreateSingletonQuery<string>("Company/Address/City").ExecuteAsync();
        var city = cities.Single();
        Assert.Equal("UpdatedCity", city);

        var names = await _context.ExecuteAsync<string>(new Uri("Company/Name", UriKind.Relative));
        var name = names.Single();
        Assert.Equal("UpdatedName", name);

        //Projection with properties - Select
        company = await _context.Company.Select(c => new Company { CompanyID = c.CompanyID, Address = c.Address, Name = c.Name }).GetValueAsync();
        Assert.NotNull(company);
        Assert.Equal("UpdatedName", company.Name);

        //Load Navigation Property
        //Singleton
        _context.LoadProperty(company, "VipCustomer");
        Assert.NotNull(company.VipCustomer);

        //Collection
        await _context.LoadPropertyAsync(company, "Departments");
        Assert.NotNull(company.Departments);
        Assert.True(company.Departments.Count > 0);

        //Single Entity
        await _context.LoadPropertyAsync(company, "CoreDepartment");
        Assert.NotNull(company.CoreDepartment);

        //Add Navigation Property - Collection
        company = await _context.Company.GetValueAsync();
        int tmpDepartmentID = rand.Next();
        int tmpCoreDepartmentID = rand.Next();
        var department = new Department()
        {
            DepartmentID = tmpDepartmentID,
            Name = "ID" + tmpDepartmentID,
        };
        var coreDepartment = new Department()
        {
            DepartmentID = tmpCoreDepartmentID,
            Name = "ID" + tmpCoreDepartmentID,
        };
        _context.AddToDepartments(department);
        _context.AddLink(company, "Departments", department);
        await _context.SaveChangesAsync();

        _context.AddToDepartments(coreDepartment);
        _context.AddLink(company, "Departments", coreDepartment);
        await _context.SaveChangesAsync();

        _context.Departments.ToList();

        //Projection with Navigation properties - Select
        company = await _context.Company.Select(c => new Company { CompanyID = c.CompanyID, Departments = c.Departments }).GetValueAsync();
        Assert.NotNull(company);
        Assert.Contains(company.Departments, c => c.DepartmentID == tmpDepartmentID);
        Assert.Contains(company.Departments, c => c.DepartmentID == tmpCoreDepartmentID);

        //Update Navigation Property - Single Entity
        _context.SetLink(company, "CoreDepartment", coreDepartment);
        await _context.SaveChangesAsync();

        //Projection with Navigation properties - Select
        company = await _context.Company.Select(c => new Company { CompanyID = c.CompanyID, CoreDepartment = c.CoreDepartment }).GetValueAsync();
        Assert.NotNull(company);
        Assert.Equal(company.CoreDepartment.DepartmentID, tmpCoreDepartmentID);

        //Update EntitySet's Navigation Property - Singleton 
        _context.SetLink(department, "Company", company);
        await _context.SaveChangesAsync(SaveChangesOptions.ReplaceOnUpdate);

        //Query(Expand) EntitySet's Navigation Property - Singleton
        department = _context.Departments.Expand(d => d.Company).Where(d => d.DepartmentID == tmpDepartmentID).Single();
        Assert.NotNull(department);
        Assert.Equal(department.Company.CompanyID, company.CompanyID);

        //Delete Navigation Property - EntitySet
        _context.DeleteLink(company, "Departments", department);
        await _context.SaveChangesAsync();

        //Projection with Navigation Property - EntitySet
        company.Departments = null;
        company = await _context.Company.Select(c => new Company { CompanyID = c.CompanyID, Departments = c.Departments }).GetValueAsync();
        Assert.NotNull(company);
        Assert.DoesNotContain(company.Departments, c => c.DepartmentID == tmpDepartmentID);

        //Query Singleton's Navigation Property - Singleton
        company = await _context.Company.Select(c => new Company { CompanyID = c.CompanyID, VipCustomer = c.VipCustomer }).GetValueAsync();
        Assert.NotNull(company);
        Assert.NotNull(company.VipCustomer);

        //Query Singleton again with Execute
        var vipCustomers = await _context.ExecuteAsync<Customer>(new Uri("VipCustomer", UriKind.Relative));
        var vipCustomer = vipCustomers.Single();

        //Update Singleton's Navigation property - Singleton
        vipCustomer.LastName = "UpdatedLastName";
        _context.UpdateRelatedObject(company, "VipCustomer", vipCustomer);
        await _context.SaveChangesAsync();

        company.VipCustomer = null;
        company = await _context.Company.Expand(c => c.VipCustomer).GetValueAsync();
        Assert.NotNull(company);
        Assert.NotNull(company.VipCustomer);
        Assert.Equal("UpdatedLastName", company.VipCustomer.LastName);

        //Update Navigation Property - Delete the Singleton navigation
        _context.SetLink(company, "VipCustomer", null);
        await _context.SaveChangesAsync();

        //Expand Navigation Property - Singleton
        company.VipCustomer = null;
        company = await _context.Company.Expand(c => c.VipCustomer).GetValueAsync();
        Assert.NotNull(company);
        Assert.Null(company.VipCustomer);

        //Update Navigation Property - Singleton
        var anotherVipCustomer = await _context.VipCustomer.GetValueAsync();
        _context.SetLink(company, "VipCustomer", anotherVipCustomer);
        await _context.SaveChangesAsync();
        company = await _context.Company.Select(c => new Company { CompanyID = c.CompanyID, VipCustomer = c.VipCustomer }).GetValueAsync();
        Assert.NotNull(company);
        Assert.NotNull(company.VipCustomer);

        ResetDefaultDataSource();
    }

    [Fact]
    public void SingletonClientTest()
    {
        // Arrange
        var rand = new Random();
        var format = ODataFormat.Json;

        //Query Singleton
        _context.MergeOption = MergeOption.OverwriteChanges;
        var company = _context.Company.GetValue();
        Assert.NotNull(company);

        //Update Singleton Property and Verify
        company.CompanyCategory = CompanyCategory.Communication;
        company.Name = "UpdatedName";
        company.Address.City = "UpdatedCity";
        _context.UpdateObject(company);
        var result = _context.SaveChanges(SaveChangesOptions.ReplaceOnUpdate);

        //Query Singleton Property - Select
        var companyCategory = _context.Company.Select(c => c.CompanyCategory).GetValue();
        Assert.Equal(CompanyCategory.Communication, companyCategory);

        var cities = _context.CreateSingletonQuery<string>("Company/Address/City").Execute();
        var city = cities.Single();
        Assert.Equal("UpdatedCity", city);

        var name = _context.Execute<string>(new Uri("Company/Name", UriKind.Relative)).Single();
        Assert.Equal("UpdatedName", name);

        //Projection with properties - Select
        company = _context.Company.Select(c => new Company { CompanyID = c.CompanyID, Address = c.Address, Name = c.Name }).GetValue();
        Assert.NotNull(company);
        Assert.Equal("UpdatedName", company.Name);

        //Load Navigation Property
        //Singleton
        _context.LoadProperty(company, "VipCustomer");
        Assert.NotNull(company.VipCustomer);

        //Collection
        _context.LoadProperty(company, "Departments");
        Assert.NotNull(company.Departments);
        Assert.True(company.Departments.Count > 0);

        //Single Entity
        _context.LoadProperty(company, "CoreDepartment");
        Assert.NotNull(company.CoreDepartment);

        //Add Navigation Property - Collection
        company = _context.Company.GetValue();
        int tmpDepartmentID = rand.Next();
        int tmpCoreDepartmentID = rand.Next();
        var department = new Department()
        {
            DepartmentID = tmpDepartmentID,
            Name = "ID" + tmpDepartmentID,
        };
        var coreDepartment = new Department()
        {
            DepartmentID = tmpCoreDepartmentID,
            Name = "ID" + tmpCoreDepartmentID,
        };
        _context.AddToDepartments(department);
        _context.AddLink(company, "Departments", department);
        result = _context.SaveChanges();

        _context.AddToDepartments(coreDepartment);
        _context.AddLink(company, "Departments", coreDepartment);
        result = _context.SaveChanges();

        _context.Departments.ToList();

        //Projection with Navigation properties - Select
        company = _context.Company.Select(c => new Company { CompanyID = c.CompanyID, Departments = c.Departments }).GetValue();
        Assert.NotNull(company);
        Assert.Contains(company.Departments, c => c.DepartmentID == tmpDepartmentID);
        Assert.Contains(company.Departments, c => c.DepartmentID == tmpCoreDepartmentID);

        //Update Navigation Property - Single Entity
        _context.SetLink(company, "CoreDepartment", coreDepartment);
        result = _context.SaveChanges();

        //Projection with Navigation properties - Select
        company = _context.Company.Select(c => new Company { CompanyID = c.CompanyID, CoreDepartment = c.CoreDepartment }).GetValue();
        Assert.NotNull(company);
        Assert.Equal(company.CoreDepartment.DepartmentID, tmpCoreDepartmentID);

        //Update EntitySet's Navigation Property - Singleton 
        _context.SetLink(department, "Company", company);
        result = _context.SaveChanges(SaveChangesOptions.ReplaceOnUpdate);

        //Query(Expand) EntitySet's Navigation Property - Singleton
        department = _context.Departments.Expand(d => d.Company).Where(d => d.DepartmentID == tmpDepartmentID).Single();
        Assert.NotNull(department);
        Assert.Equal(department.Company.CompanyID, company.CompanyID);

        //Delete Navigation Property - EntitySet
        _context.DeleteLink(company, "Departments", department);
        result = _context.SaveChanges();

        //Projection with Navigation Property - EntitySet
        company.Departments = null;
        company = _context.Company.Select(c => new Company { CompanyID = c.CompanyID, Departments = c.Departments }).GetValue();
        Assert.NotNull(company);
        Assert.DoesNotContain(company.Departments, c => c.DepartmentID == tmpDepartmentID);

        //Query Singleton's Navigation Property - Singleton
        company = _context.Company.Select(c => new Company { CompanyID = c.CompanyID, VipCustomer = c.VipCustomer }).GetValue();
        Assert.NotNull(company);
        Assert.NotNull(company.VipCustomer);

        //Query Singleton again with Execute
        var vipCustomer = _context.Execute<Customer>(new Uri("VipCustomer", UriKind.Relative)).Single();

        //Update Singleton's Navigation property - Singleton
        vipCustomer.LastName = "UpdatedLastName";
        _context.UpdateRelatedObject(company, "VipCustomer", vipCustomer);
        result = _context.SaveChanges();

        company.VipCustomer = null;
        company = _context.Company.Expand(c => c.VipCustomer).GetValue();
        Assert.NotNull(company);
        Assert.NotNull(company.VipCustomer);
        Assert.Equal("UpdatedLastName", company.VipCustomer.LastName);

        //Update Navigation Property - Delete the Singleton navigation
        _context.SetLink(company, "VipCustomer", null);
        result = _context.SaveChanges();

        //Expand Navigation Property - Singleton
        company.VipCustomer = null;
        company = _context.Company.Expand(c => c.VipCustomer).GetValue();
        Assert.NotNull(company);
        Assert.Null(company.VipCustomer);

        //Update Navigation Property - Singleton
        var anotherVipCustomer = _context.VipCustomer.GetValue();
        _context.SetLink(company, "VipCustomer", anotherVipCustomer);
        result = _context.SaveChanges();
        company = _context.Company.Select(c => new Company { CompanyID = c.CompanyID, VipCustomer = c.VipCustomer }).GetValue();
        Assert.NotNull(company);
        Assert.NotNull(company.VipCustomer);

        ResetDefaultDataSource();
    }

    #endregion

    #region DerivedType Singleton Tests

    [Fact]
    public void DerivedTypeSingletonClientTest()
    {
        // Query Singleton
        _context.MergeOption = MergeOption.OverwriteChanges;
        var company = _context.PublicCompany.GetValue();
        Assert.NotNull(company);

        // Update DerivedType Property and Verify
        var publicCompany = company as PublicCompany;
        Assert.NotNull(publicCompany);
        publicCompany.Name = "UpdatedName";
        publicCompany.StockExchange = "Updated StockExchange";
        _context.UpdateObject(publicCompany);
        _context.SaveChanges(SaveChangesOptions.ReplaceOnUpdate);

        // Query Singleton Property - Select            
        var name = _context.PublicCompany.Select(c => c.Name).GetValue();
        Assert.Equal("UpdatedName", name);
        company = _context.CreateSingletonQuery<Company>("PublicCompany").Single();
        Assert.Equal("Updated StockExchange", (company as PublicCompany).StockExchange);

        // Query properties of DerivedType
        var stockExchange = _context.PublicCompany.Select(c => (c as PublicCompany).StockExchange).GetValue();
        Assert.Equal("Updated StockExchange", stockExchange);

        // Projection with properties - Select
        publicCompany = _context.PublicCompany.Select(c =>
            new PublicCompany { CompanyID = c.CompanyID, Name = c.Name, StockExchange = (c as PublicCompany).StockExchange }).GetValue();
        Assert.NotNull(publicCompany);
        Assert.Equal("UpdatedName", publicCompany.Name);
        Assert.Equal("Updated StockExchange", publicCompany.StockExchange);

        company = _context.CreateSingletonQuery<Company>("PublicCompany").Single();

        // Load Navigation Property
        // Collection            
        _context.LoadProperty(company, "Assets");
        Assert.NotNull(((PublicCompany)company).Assets);
        Assert.Equal(2, ((PublicCompany)company).Assets.Count);

        // Single Entity
        _context.LoadProperty(company, "Club");
        Assert.NotNull(((PublicCompany)company).Club);

        // Singleton
        _context.LoadProperty(publicCompany, "LabourUnion");
        Assert.NotNull(((PublicCompany)company).LabourUnion);

        //Add Contained Navigation Property - Collection of derived type
        var random = new Random();
        int tmpAssertId = random.Next();
        Asset tmpAssert = new()
        {
            AssetID = tmpAssertId,
            Name = tmpAssertId + "Name",
            Number = tmpAssertId
        };

        _context.AddRelatedObject(publicCompany, "Assets", tmpAssert);
        _context.SaveChanges();

        // Query contained Navigation Property - Collection of derived type
        company = _context.PublicCompany.Expand(c => (c as PublicCompany).Assets).GetValue();
        Assert.NotNull(company);
        Assert.Contains(((PublicCompany)company).Assets, a => a.AssetID == tmpAssertId);

        _context.DeleteObject(tmpAssert);
        _context.SaveChanges();

        company = _context.PublicCompany.Expand(c => (c as PublicCompany).Assets).GetValue();
        Assert.NotNull(company);
        Assert.DoesNotContain(((PublicCompany)company).Assets, a => a.AssetID == tmpAssertId);

        // Updated contained Navigation Property - SingleEntity of derived type
        var club = ((PublicCompany)company).Club;
        club.Name = "UpdatedClubName";
        _context.UpdateRelatedObject(company, "Club", club);
        _context.SaveChanges();

        // Query Contained Navigation Property - Single Entity of derived type
        publicCompany = _context.PublicCompany.Select(c => new PublicCompany { CompanyID = c.CompanyID, Club = (c as PublicCompany).Club }).GetValue();
        Assert.NotNull(publicCompany);
        Assert.NotNull(publicCompany.Club);
        Assert.Equal("UpdatedClubName", publicCompany.Club.Name);

        company = _context.PublicCompany.Expand(c => (c as PublicCompany).Club).GetValue();
        Assert.NotNull(company);
        Assert.NotNull(((PublicCompany)company).Club);

        // Projection with Navigation property of derived type - Singleton
        company = _context.PublicCompany.Expand(c => (c as PublicCompany).LabourUnion).GetValue();

        // Update Navigation property of derived Type - Singleton
        var labourUnion = ((PublicCompany)company).LabourUnion;
        labourUnion.Name = "UpdatedLabourUnionName";
        _context.UpdateRelatedObject(publicCompany, "LabourUnion", labourUnion);
        _context.SaveChanges();

        //Query singleton of derived type.            
        publicCompany = _context.PublicCompany.Select(c => new PublicCompany { CompanyID = c.CompanyID, LabourUnion = (c as PublicCompany).LabourUnion }).GetValue();
        Assert.NotNull(publicCompany);
        Assert.NotNull(publicCompany.LabourUnion);

        ResetDefaultDataSource();
    }

    #endregion

    #region Action/Function

    [Fact]
    public void InvokeFunctionBoundedToSingleton()
    {
        // Arrange & Act & Assert
        var employeeCount = _context.Execute<int>(new Uri(_baseUri.AbsoluteUri + "Company/Default.GetEmployeesCount", UriKind.Absolute)).Single();
        Assert.Equal(2, employeeCount);

        var company = _context.Company.GetValue();
        var descriptor = _context.GetEntityDescriptor(company).OperationDescriptors.Single(e => e.Title == "Default.GetEmployeesCount");
        employeeCount = _context.Execute<int>(descriptor.Target, "GET", true).Single();
        Assert.Equal(2, employeeCount);
    }

    [Fact]
    public void InvokeActionBoundedToSingleton()
    {
        var company = _context.Company.GetValue();
        _context.LoadProperty(company, "Revenue");

        var newValue = _context.Execute<long>(
            new Uri(_baseUri.AbsoluteUri + "Company/Default.IncreaseRevenue"), "POST", true, new BodyOperationParameter("IncreaseValue", 20000));
        Assert.Equal(newValue.Single(), company.Revenue + 20000);

        OperationDescriptor descriptor = _context.GetEntityDescriptor(company).OperationDescriptors.Single(e => e.Title == "Default.IncreaseRevenue");
        newValue = _context.Execute<long>(descriptor.Target, "POST", new BodyOperationParameter("IncreaseValue", 40000));
        Assert.Equal(newValue.Single(), company.Revenue + 60000);
    }

    #endregion

    #region Private methods

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "singletonclienttests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
