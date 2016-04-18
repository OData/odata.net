//---------------------------------------------------------------------
// <copyright file="AsynchronousSingletonClientTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.OData.Client;
using Microsoft.Test.OData.Services.TestServices;
using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
using Xunit;

namespace Microsoft.OData.Profile111.Tests.AsynchronousTests
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class AsynchronousSingletonClientTest : EndToEndTestBase
    {
        InMemoryEntities testClientContext;

        public AsynchronousSingletonClientTest()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        [Fact]
        public void SingletonQueryUpdatePropertyClientTest()
        {
            this.testClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            this.testClientContext.MergeOption = MergeOption.OverwriteChanges;

            //Query Singleton            
            var queryCompany = this.testClientContext.Company as DataServiceQuerySingle<Company>;
            var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            var company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(company != null);

            //Update Singleton Property and Verify
            company.CompanyCategory = CompanyCategory.Communication;
            company.Name = "UpdatedName";
            company.Address.City = "UpdatedCity";
            this.testClientContext.UpdateObject(company);
            var updateCompanyAr = this.testClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            this.testClientContext.EndSaveChanges(updateCompanyAr);

            //Query Singleton Property - Select
            var queryCompanyCategory = this.testClientContext.Company.Select(c => c.CompanyCategory);
            var queryCompanyCategoryAr = queryCompanyCategory.BeginGetValue(null, null).EnqueueWait(this);
            CompanyCategory? companyCategory = queryCompanyCategory.EndGetValue(queryCompanyCategoryAr);
            Assert.True(companyCategory == CompanyCategory.Communication);

            var queryCity = this.testClientContext.CreateQuery<string>("Company/Address/City");
            var queryCityAr = queryCity.BeginExecute(null, null);
            var city = queryCity.EndExecute(queryCityAr).Single();
            this.EnqueueCallback(() => Assert.True(city == "UpdatedCity"));

            var queryNameAr = this.testClientContext.BeginExecute<string>(new Uri("Company/Name", UriKind.Relative), null, null).EnqueueWait(this);
            var name = this.testClientContext.EndExecute<string>(queryNameAr).Single();
            Assert.True(name == "UpdatedName");

            //Projection with properties - Select
            this.TestCompleted = false;
            queryCompany = this.testClientContext.Company.Select(c => new Company { CompanyID = c.CompanyID, Address = c.Address, Name = c.Name });
            queryCompanyAr = queryCompany.BeginGetValue(null, null);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(company.Name == "UpdatedName");
            Assert.True(company.Departments.Count == 0);
            this.EnqueueTestComplete();
        }

        [Fact]
        public void SingletonQueryUpdateNavigationCollectionPropertyClientTest()
        {
            this.testClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            this.testClientContext.MergeOption = MergeOption.OverwriteChanges;

            var queryCompany = this.testClientContext.Company as DataServiceQuerySingle<Company>;
            var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            var company = queryCompany.EndGetValue(queryCompanyAr);

            //Load Navigation Property
            var loadDeparments = this.testClientContext.BeginLoadProperty(company, "Departments", null, null).EnqueueWait(this);
            this.testClientContext.EndLoadProperty(loadDeparments);
            Assert.True(company.Departments.Count > 0);

            //Add Navigation Property - Collection
            Random rand = new Random();
            int tmpDepartmentId = rand.Next();
            Department department = new Department()
            {
                DepartmentID = tmpDepartmentId,
                Name = "ID" + tmpDepartmentId
            };
            this.testClientContext.AddToDepartments(department);
            this.testClientContext.AddLink(company, "Departments", department);
            var addDepartmentsLink = this.testClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            this.testClientContext.EndSaveChanges(addDepartmentsLink);

            //Projection with Navigation properties - Select
            var selectCompany = this.testClientContext.Company.Select(c => new Company { CompanyID = c.CompanyID, Departments = c.Departments });
            var selectCompanyAr = selectCompany.BeginGetValue(null, null).EnqueueWait(this);
            var projectedCompany = selectCompany.EndGetValue(selectCompanyAr);
            Assert.True(projectedCompany.Departments.Any(c => c.DepartmentID == tmpDepartmentId));

            //Update EntitySet's Navigation Property - Singleton 
            this.testClientContext.SetLink(department, "Company", company);
            var ar4 = this.testClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            this.testClientContext.EndSaveChanges(ar4);

            //Query(Expand) EntitySet's Navigation Property - Singleton
            var queryDepartment = this.testClientContext.Departments.Expand(d => d.Company).Where(d => d.DepartmentID == tmpDepartmentId) as DataServiceQuery<Department>;
            var queryDepartmentAr = queryDepartment.BeginExecute(null, null).EnqueueWait(this);
            department = queryDepartment.EndExecute(queryDepartmentAr).SingleOrDefault();
            Assert.True(department.Company.CompanyID == company.CompanyID);

            //Delete Navigation Property - EntitySet
            this.testClientContext.DeleteLink(company, "Departments", department);
            var deleteLinkAr = this.testClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            this.testClientContext.EndSaveChanges(deleteLinkAr);

            //Expand Navigation Property - EntitySet
            queryCompany = this.testClientContext.Company.Expand("Departments");
            queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(company.Departments.All(c => c.DepartmentID != tmpDepartmentId));

            this.EnqueueTestComplete();
        }

        [Fact]
        public void SingletonQueryUpdateNavigationSingletonPropertyClientTest()
        {
            this.testClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            this.testClientContext.MergeOption = MergeOption.OverwriteChanges;

            var queryCompany = this.testClientContext.Company as DataServiceQuerySingle<Company>;
            var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            var company = queryCompany.EndGetValue(queryCompanyAr);

            //Query Singleton again with Execute
            var queryVipCustomerAr = this.testClientContext.BeginExecute<Customer>(new Uri("VipCustomer", UriKind.Relative), null, null).EnqueueWait(this);
            var vipCustomer = this.testClientContext.EndExecute<Customer>(queryVipCustomerAr).Single();
            Assert.True(vipCustomer != null);

            //Update Singleton's Navigation property - Singleton
            vipCustomer.City = "UpdatedCity";
            this.testClientContext.UpdateRelatedObject(company, "VipCustomer", vipCustomer);
            var ar6 = this.testClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            this.testClientContext.EndSaveChanges(ar6);

            //Expand Navigation Property - Singleton
            company.VipCustomer = null;
            queryCompany = this.testClientContext.Company.Expand(c => c.VipCustomer);
            queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(company.VipCustomer.City == "UpdatedCity");

            //Update Navigation Property - Delete the Singleton navigation
            this.testClientContext.SetLink(company, "VipCustomer", null);
            var ar7 = this.testClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            this.testClientContext.EndSaveChanges(ar7);

            //Expand Navigation Property using name- Singleton
            company.VipCustomer = null;
            queryCompany = this.testClientContext.Company.Expand("VipCustomer");
            queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(company.VipCustomer == null);

            //Update Navigation Property - Singleton
            this.testClientContext.SetLink(company, "VipCustomer", vipCustomer);
            var ar8 = this.testClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            this.testClientContext.EndSaveChanges(ar8);

            //LoadProperty Navigation Property - Singleton
            company.VipCustomer = null;
            var ar13 = this.testClientContext.BeginLoadProperty(company, "VipCustomer", null, null).EnqueueWait(this);
            this.testClientContext.EndLoadProperty(ar13);
            Assert.True(company.VipCustomer != null);

            //Expand Navigation Property - Singleton
            company.VipCustomer = null;
            queryCompany = this.testClientContext.Company.Expand(c => c.VipCustomer);
            queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(company.VipCustomer != null);

            //Query Singleton's Navigation Property - Singleton
            queryCompany = this.testClientContext.Company.Select(c => new Company { CompanyID = c.CompanyID, VipCustomer = c.VipCustomer });
            queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(company.VipCustomer != null);

            this.EnqueueTestComplete();
        }

        [Fact]
        public void SingletonQueryUpdateNavigationSingleEntityPropertyClientTest()
        {
            this.testClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            this.testClientContext.MergeOption = MergeOption.OverwriteChanges;

            var queryCompany = this.testClientContext.Company.Expand("CoreDepartment");
            var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            Company company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(company.CoreDepartment != null);

            //Single Entity
            company.CoreDepartment = null;
            var ar15 = this.testClientContext.BeginLoadProperty(company, "CoreDepartment", null, null).EnqueueWait(this);
            this.testClientContext.EndLoadProperty(ar15);
            Assert.True(company.CoreDepartment != null);

            Random rand = new Random();
            int tmpCoreDepartmentId = rand.Next();
            Department coreDepartment = new Department()
            {
                DepartmentID = tmpCoreDepartmentId,
                Name = "ID" + tmpCoreDepartmentId
            };

            this.testClientContext.AddToDepartments(coreDepartment);
            this.testClientContext.AddLink(company, "Departments", coreDepartment);
            var ar1 = this.testClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            this.testClientContext.EndSaveChanges(ar1);

            //Update Navigation Property - Single Entity
            this.testClientContext.SetLink(company, "CoreDepartment", coreDepartment);
            var ar3 = this.testClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            this.testClientContext.EndSaveChanges(ar3);

            //Projection with Navigation properties - Select
            queryCompany = this.testClientContext.Company.Select(c => new Company { CompanyID = c.CompanyID, CoreDepartment = c.CoreDepartment });
            queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(company.CoreDepartment.DepartmentID == tmpCoreDepartmentId);
            this.EnqueueTestComplete();
        }

        [Fact]
        public void UpdateDerivedTypePropertyClientTest()
        {
            this.testClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            this.testClientContext.MergeOption = MergeOption.OverwriteChanges;

            //Query Singleton
            var queryCompany = this.testClientContext.PublicCompany;
            var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            var company = queryCompany.EndGetValue(queryCompanyAr);

            //Update DerivedType Property and Verify
            PublicCompany publicCompany = company as PublicCompany;
            publicCompany.Name = "UpdatedName";
            publicCompany.StockExchange = "Updated StockExchange";
            this.testClientContext.UpdateObject(publicCompany);
            var updateCompanyAr = this.testClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            this.testClientContext.EndSaveChanges(updateCompanyAr);

            //Query Singleton Property - Select            
            var queryName = this.testClientContext.PublicCompany.Select(c => c.Name);
            var queryNameAr = queryName.BeginGetValue(null, null).EnqueueWait(this);
            var name = queryName.EndGetValue(queryNameAr);
            Assert.True(name == "UpdatedName");

            //Projection with properties of DerivedType
            var queryStockExchange = this.testClientContext.PublicCompany.Select(c => (c as PublicCompany).StockExchange);
            var queryStockExchangeAr = queryStockExchange.BeginGetValue(null, null).EnqueueWait(this);
            var stockExchange = queryStockExchange.EndGetValue(queryStockExchangeAr);
            Assert.True(stockExchange == "Updated StockExchange");

            //Projection with properties - Select
            var queryPublicCompany = this.testClientContext.PublicCompany.Select(c =>
                new PublicCompany
                {
                    CompanyID = c.CompanyID,
                    Name = c.Name,
                    StockExchange = (c as PublicCompany).StockExchange
                });
            var queryPublicCompanyAr = queryPublicCompany.BeginGetValue(null, null).EnqueueWait(this);
            publicCompany = queryPublicCompany.EndGetValue(queryPublicCompanyAr);
            Assert.True(publicCompany.Name == "UpdatedName");
            Assert.True(publicCompany.StockExchange == "Updated StockExchange");

            this.EnqueueTestComplete();
        }

        [Fact]
        public void UpdateDerivedTypeNavigationOfContainedCollection()
        {
            this.testClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            this.testClientContext.MergeOption = MergeOption.OverwriteChanges;

            //Query Singleton
            var queryCompany = this.testClientContext.PublicCompany as DataServiceQuerySingle<Company>;
            var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            var company = queryCompany.EndGetValue(queryCompanyAr);

            //Load Navigation Property
            //Collection            
            var loadAssetsAr = this.testClientContext.BeginLoadProperty(company, "Assets", null, null).EnqueueWait(this);
            this.testClientContext.EndLoadProperty(loadAssetsAr);
            Assert.True((company as PublicCompany).Assets != null);

            //Add Contained Navigation Property - Collection of derived type
            Random rand = new Random();
            int tmpAssertId = rand.Next();
            Asset tmpAssert = new Asset()
            {
                AssetID = tmpAssertId,
                Name = tmpAssertId + "Name",
                Number = tmpAssertId
            };
            this.testClientContext.AddRelatedObject(company, "Assets", tmpAssert);
            var addRelatedObjectAr = this.testClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            this.testClientContext.EndSaveChanges(addRelatedObjectAr);

            //Query contained Navigation Property - Collection of derived type
            queryCompany = this.testClientContext.PublicCompany.Expand(c => (c as PublicCompany).Assets);
            var queryCompanyAr1 = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr1);
            Assert.True((company as PublicCompany).Assets.Any(a => a.AssetID == tmpAssertId));

            //Delete contained Navigation Property - Collection of derived type
            this.testClientContext.DeleteObject(tmpAssert);
            var deleteObjectAr = this.testClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            this.testClientContext.EndSaveChanges(deleteObjectAr);

            //Expand contained Navigation property - Collection of derived type
            queryCompany = this.testClientContext.PublicCompany.Expand(c => (c as PublicCompany).Assets);
            queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True((company as PublicCompany).Assets.All(a => a.AssetID != tmpAssertId));

            this.EnqueueTestComplete();
        }

        [Fact]
        public void UpdateDerivedTypeNavigationOfContainedSingleEntity()
        {
            this.testClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            this.testClientContext.MergeOption = MergeOption.OverwriteChanges;

            //Query Singleton
            var queryCompany = this.testClientContext.PublicCompany;
            var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            var company = queryCompany.EndGetValue(queryCompanyAr);

            //Load Navigation Property 
            //Single Enity
            var loadClubAr = this.testClientContext.BeginLoadProperty(company, "Club", null, null).EnqueueWait(this);
            this.testClientContext.EndLoadProperty(loadClubAr);
            Assert.True((company as PublicCompany).Club != null);

            //Updated Conatined Navigation Property - SingleEntity of derived type
            var club = (company as PublicCompany).Club;
            club.Name = "UpdatedClubName";
            this.testClientContext.UpdateRelatedObject(company, "Club", club);
            var updateRelatedObjectAr = this.testClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            this.testClientContext.EndSaveChanges(updateRelatedObjectAr);

            //Projecton with Contained Navigation Property - Single Entity of derived type
            var queryPublicCompany = this.testClientContext.PublicCompany.Select(c =>
                new PublicCompany { CompanyID = c.CompanyID, Club = (c as PublicCompany).Club });
            var queryPublicCompany2Ar = queryPublicCompany.BeginGetValue(null, null).EnqueueWait(this);
            var publicCompany = queryPublicCompany.EndGetValue(queryPublicCompany2Ar);
            Assert.True(publicCompany.Club.Name == "UpdatedClubName");

            this.EnqueueTestComplete();
        }
        [Fact]
        public void DerivedTypeSingletonClientTest()
        {
            this.testClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            this.testClientContext.MergeOption = MergeOption.OverwriteChanges;

            //Query Singleton
            var queryCompany = this.testClientContext.PublicCompany as DataServiceQuerySingle<Company>;
            var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            var company = queryCompany.EndGetValue(queryCompanyAr);

            //Singleton
            var loadLabourUnionAr = this.testClientContext.BeginLoadProperty(company, "LabourUnion", null, null).EnqueueWait(this);
            this.testClientContext.EndLoadProperty(loadLabourUnionAr);
            Assert.True((company as PublicCompany).LabourUnion != null);

            //Expand Navigation Property - Singleton
            queryCompany = this.testClientContext.PublicCompany.Expand(c => (c as PublicCompany).Club);
            queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True((company as PublicCompany).Club != null);

            //Update Navigation property of derived Type - Singleton
            var labourUnion = (company as PublicCompany).LabourUnion;
            labourUnion.Name = "UpdatedLabourUnionName";
            this.testClientContext.UpdateRelatedObject(company, "LabourUnion", labourUnion);
            var updateRelatedObjectAr = this.testClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            this.testClientContext.EndSaveChanges(updateRelatedObjectAr);

            //Projecton with Navigation property - singletonof derived type.            
            var queryPublicCompany = this.testClientContext.PublicCompany.Select(c =>
                new PublicCompany { CompanyID = c.CompanyID, LabourUnion = (c as PublicCompany).LabourUnion });
            var queryPublicCompanyAr = queryPublicCompany.BeginGetValue(null, null).EnqueueWait(this);
            var publicCompany = queryPublicCompany.EndGetValue(queryPublicCompanyAr);
            Assert.True(publicCompany.LabourUnion != null);

            this.EnqueueTestComplete();
        }
    }
}