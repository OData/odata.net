//---------------------------------------------------------------------
// <copyright file="AsynchronousSingletonClientTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Test.OData.Tests.Client
{
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.Linq;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Xunit.Abstractions;
    using Xunit;

    public class AsynchronousSingletonClientTest : EndToEndTestBase
    {
        InMemoryEntities TestClientContext;
        public AsynchronousSingletonClientTest(ITestOutputHelper helper)
            : base(ServiceDescriptors.ODataWCFServiceDescriptor, helper)
        {
        }
        [Fact, Asynchronous]
        public void SingletonQueryUpdatePropertyClientTest()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;

            //Query Singleton            
            var queryCompany = TestClientContext.Company as DataServiceQuerySingle<Company>;
            var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            var company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(company != null);

            //Update Singleton Property and Verify
            company.CompanyCategory = CompanyCategory.Communication;
            company.Name = "UpdatedName";
            company.Address.City = "UpdatedCity";
            TestClientContext.UpdateObject(company);
            var updateCompanyAr = TestClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            TestClientContext.EndSaveChanges(updateCompanyAr);

            //Query Singleton Property - Select
            var queryCompanyCategory = TestClientContext.Company.Select(c => c.CompanyCategory) as DataServiceQuerySingle<CompanyCategory?>;
            CompanyCategory? companyCategory = CompanyCategory.Others;
            var queryCompanyCategoryAr = queryCompanyCategory.BeginGetValue(null, null).EnqueueWait(this);
            companyCategory = queryCompanyCategory.EndGetValue(queryCompanyCategoryAr);
            Assert.True(companyCategory == CompanyCategory.Communication);

            var queryCity = TestClientContext.CreateQuery<string>("Company/Address/City");
            var queryCityAr = queryCity.BeginExecute(null, null);
            var city = queryCity.EndExecute(queryCityAr).Single();
            this.EnqueueCallback(() => Assert.True(city == "UpdatedCity"));

            var queryNameAr = TestClientContext.BeginExecute<string>(new Uri("Company/Name", UriKind.Relative), null, null).EnqueueWait(this);
            var name = TestClientContext.EndExecute<string>(queryNameAr).Single();
            Assert.True(name == "UpdatedName");

            //Projection with properties - Select
            this.TestCompleted = false;
            queryCompany = TestClientContext.Company.Select(c => new Company { CompanyID = c.CompanyID, Address = c.Address, Name = c.Name }) as DataServiceQuerySingle<Company>;
            queryCompanyAr = queryCompany.BeginGetValue(null, null);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(company.Name == "UpdatedName");
            Assert.True(company.Departments.Count == 0);
            this.EnqueueTestComplete();
        }

        [Fact, Asynchronous]
        public void SingletonQueryUpdateNavigationCollectionPropertyClientTest()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;

            var queryCompany = TestClientContext.Company as DataServiceQuerySingle<Company>;
            var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            var company = queryCompany.EndGetValue(queryCompanyAr);

            //Load Navigation Property
            var loadDeparments = TestClientContext.BeginLoadProperty(company, "Departments", null, null).EnqueueWait(this);
            TestClientContext.EndLoadProperty(loadDeparments);
            Assert.True(company.Departments.Count > 0);

            //Add Navigation Property - Collection
            Random rand = new Random();
            int tmpDepartmentID = rand.Next();
            Department department = new Department()
            {
                DepartmentID = tmpDepartmentID,
                Name = "ID" + tmpDepartmentID
            };
            TestClientContext.AddToDepartments(department);
            TestClientContext.AddLink(company, "Departments", department);
            var addDepartmentsLink = TestClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            TestClientContext.EndSaveChanges(addDepartmentsLink);

            //Projection with Navigation properties - Select
            var selectCompany = TestClientContext.Company.Select(c => new Company { CompanyID = c.CompanyID, Departments = c.Departments }) as DataServiceQuerySingle<Company>;
            var selectCompanyAr = selectCompany.BeginGetValue(null, null).EnqueueWait(this);
            var projectedCompany = selectCompany.EndGetValue(selectCompanyAr);
            Assert.True(projectedCompany.Departments.Any(c => c.DepartmentID == tmpDepartmentID));

            //Update EntitySet's Navigation Property - Singleton 
            TestClientContext.SetLink(department, "Company", company);
            var ar4 = TestClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            TestClientContext.EndSaveChanges(ar4);

            //Query(Expand) EntitySet's Navigation Property - Singleton
            var queryDepartment = TestClientContext.Departments.Expand(d => d.Company).Where(d => d.DepartmentID == tmpDepartmentID) as DataServiceQuery<Department>;
            var queryDepartmentAr = queryDepartment.BeginExecute(null, null).EnqueueWait(this);
            department = queryDepartment.EndExecute(queryDepartmentAr).SingleOrDefault();
            Assert.True(department.Company.CompanyID == company.CompanyID);

            //Delete Navigation Property - EntitySet
            TestClientContext.DeleteLink(company, "Departments", department);
            var deleteLinkAr = TestClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            TestClientContext.EndSaveChanges(deleteLinkAr);

            //Expand Navigation Property - EntitySet
            queryCompany = TestClientContext.Company.Expand("Departments");
            queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(!company.Departments.Any(c => c.DepartmentID == tmpDepartmentID));

            this.EnqueueTestComplete();
        }

        [Fact, Asynchronous]
        public void SingletonQueryUpdateNavigationSingletonPropertyClientTest()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;

            var queryCompany = TestClientContext.Company as DataServiceQuerySingle<Company>;
            var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            var company = queryCompany.EndGetValue(queryCompanyAr);

            //Query Singleton again with Execute
            var queryVipCustomerAr = TestClientContext.BeginExecute<Customer>(new Uri("VipCustomer", UriKind.Relative), null, null).EnqueueWait(this);
            var vipCustomer = TestClientContext.EndExecute<Customer>(queryVipCustomerAr).Single();
            Assert.True(vipCustomer != null);

            //Update Singleton's Navigation property - Singleton
            vipCustomer.City = "UpdatedCity";
            TestClientContext.UpdateRelatedObject(company, "VipCustomer", vipCustomer);
            var ar6 = TestClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            TestClientContext.EndSaveChanges(ar6);

            //Expand Navigation Property - Singleton
            company.VipCustomer = null;
            queryCompany = TestClientContext.Company.Expand(c => c.VipCustomer);
            queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(company.VipCustomer.City == "UpdatedCity");

            //Update Navigation Property - Delete the Singleton navigation
            TestClientContext.SetLink(company, "VipCustomer", null);
            var ar7 = TestClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            TestClientContext.EndSaveChanges(ar7);

            //Expand Navigation Property using name- Singleton
            company.VipCustomer = null;
            queryCompany = TestClientContext.Company.Expand("VipCustomer");
            queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(company.VipCustomer == null);

            //Update Navigation Property - Singleton
            TestClientContext.SetLink(company, "VipCustomer", vipCustomer);
            var ar8 = TestClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            TestClientContext.EndSaveChanges(ar8);

            //LoadProperty Navigation Property - Singleton
            company.VipCustomer = null;
            var ar13 = TestClientContext.BeginLoadProperty(company, "VipCustomer", null, null).EnqueueWait(this);
            TestClientContext.EndLoadProperty(ar13);
            Assert.True(company.VipCustomer != null);

            //Expand Navigation Property - Singleton
            company.VipCustomer = null;
            queryCompany = TestClientContext.Company.Expand(c => c.VipCustomer);
            queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(company.VipCustomer != null);

            //Query Singleton's Navigation Property - Singleton
            queryCompany = TestClientContext.Company.Select(c => new Company { CompanyID = c.CompanyID, VipCustomer = c.VipCustomer }) as DataServiceQuerySingle<Company>;
            queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(company.VipCustomer != null);

            this.EnqueueTestComplete();
        }

        [Fact, Asynchronous]
        public void SingletonQueryUpdateNavigationSingleEntityPropertyClientTest()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;

            Company company = null;
            var queryCompany = TestClientContext.Company.Expand("CoreDepartment");
            var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(company.CoreDepartment != null);

            //Single Entity
            company.CoreDepartment = null;
            var ar15 = TestClientContext.BeginLoadProperty(company, "CoreDepartment", null, null).EnqueueWait(this);
            TestClientContext.EndLoadProperty(ar15);
            Assert.True(company.CoreDepartment != null);

            Random rand = new Random();
            int tmpCoreDepartmentID = rand.Next();
            Department coreDepartment = new Department()
            {
                DepartmentID = tmpCoreDepartmentID,
                Name = "ID" + tmpCoreDepartmentID
            };

            TestClientContext.AddToDepartments(coreDepartment);
            TestClientContext.AddLink(company, "Departments", coreDepartment);
            var ar1 = TestClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            TestClientContext.EndSaveChanges(ar1);

            //Update Navigation Property - Single Entity
            TestClientContext.SetLink(company, "CoreDepartment", coreDepartment);
            var ar3 = TestClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            TestClientContext.EndSaveChanges(ar3);

            //Projection with Navigation properties - Select
            queryCompany = TestClientContext.Company.Select(c => new Company { CompanyID = c.CompanyID, CoreDepartment = c.CoreDepartment }) as DataServiceQuerySingle<Company>;
            queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(company.CoreDepartment.DepartmentID == tmpCoreDepartmentID);
            this.EnqueueTestComplete();
        }

        [Fact, Asynchronous]
        public void UpdateDerivedTypePropertyClientTest()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;

            //Query Singleton
            var queryCompany = TestClientContext.PublicCompany;
            var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            var company = queryCompany.EndGetValue(queryCompanyAr);

            //Update DerivedType Property and Verify
            PublicCompany publicCompany = company as PublicCompany;
            publicCompany.Name = "UpdatedName";
            publicCompany.StockExchange = "Updated StockExchange";
            TestClientContext.UpdateObject(publicCompany);
            var updateCompanyAr = TestClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            TestClientContext.EndSaveChanges(updateCompanyAr);

            //Query Singleton Property - Select            
            var queryName = TestClientContext.PublicCompany.Select(c => c.Name) as DataServiceQuerySingle<string>;
            var queryNameAr = queryName.BeginGetValue(null, null).EnqueueWait(this);
            var name = queryName.EndGetValue(queryNameAr);
            Assert.True(name == "UpdatedName");

            //Projection with properties of DerivedType
            var queryStockExchange = TestClientContext.PublicCompany.Select(c => (c as PublicCompany).StockExchange) as DataServiceQuerySingle<string>;
            var queryStockExchangeAr = queryStockExchange.BeginGetValue(null, null).EnqueueWait(this);
            var stockExchange = queryStockExchange.EndGetValue(queryStockExchangeAr);
            Assert.True(stockExchange == "Updated StockExchange");

            //Projection with properties - Select
            var queryPublicCompany = TestClientContext.PublicCompany.Select(c =>
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
        }

        [Fact, Asynchronous]
        public void UpdateDerivedTypeNavigationOfContainedCollection()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;

            //Query Singleton
            var queryCompany = TestClientContext.PublicCompany as DataServiceQuerySingle<Company>;
            var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            var company = queryCompany.EndGetValue(queryCompanyAr);

            //Load Navigation Property
            //Collection            
            var loadAssetsAr = TestClientContext.BeginLoadProperty(company, "Assets", null, null).EnqueueWait(this);
            TestClientContext.EndLoadProperty(loadAssetsAr);
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
            TestClientContext.AddRelatedObject(company, "Assets", tmpAssert);
            var addRelatedObjectAr = TestClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            TestClientContext.EndSaveChanges(addRelatedObjectAr);

            //Query contained Navigation Property - Collection of derived type
            queryCompany = TestClientContext.PublicCompany.Expand(c => (c as PublicCompany).Assets) as DataServiceQuerySingle<Company>;
            var queryCompanyAr1 = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr1);
            Assert.True((company as PublicCompany).Assets.Any(a => a.AssetID == tmpAssertId));

            //Delete contained Navigation Property - Collection of derived type
            TestClientContext.DeleteObject(tmpAssert);
            var deleteObjectAr = TestClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            TestClientContext.EndSaveChanges(deleteObjectAr);

            //Expand contained Navigation property - Collection of derived type
            queryCompany = TestClientContext.PublicCompany.Expand(c => (c as PublicCompany).Assets) as DataServiceQuerySingle<Company>;
            queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True(!(company as PublicCompany).Assets.Any(a => a.AssetID == tmpAssertId));

            this.EnqueueTestComplete();
        }

        [Fact, Asynchronous]
        public void UpdateDerivedTypeNavigationOfContainedSingleEntity()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;

            //Query Singleton
            var queryCompany = TestClientContext.PublicCompany;
            var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            var company = queryCompany.EndGetValue(queryCompanyAr);

            //Load Navigation Property 
            //Single Enity
            var loadClubAr = TestClientContext.BeginLoadProperty(company, "Club", null, null).EnqueueWait(this);
            TestClientContext.EndLoadProperty(loadClubAr);
            Assert.True((company as PublicCompany).Club != null);

            //Updated Conatined Navigation Property - SingleEntity of derived type
            var club = (company as PublicCompany).Club;
            club.Name = "UpdatedClubName";
            TestClientContext.UpdateRelatedObject(company, "Club", club);
            var updateRelatedObjectAr = TestClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            TestClientContext.EndSaveChanges(updateRelatedObjectAr);

            //Projecton with Contained Navigation Property - Single Entity of derived type
            var queryPublicCompany = TestClientContext.PublicCompany.Select(c =>
                new PublicCompany { CompanyID = c.CompanyID, Club = (c as PublicCompany).Club }) as DataServiceQuerySingle<PublicCompany>;
            var queryPublicCompany2Ar = queryPublicCompany.BeginGetValue(null, null).EnqueueWait(this);
            var publicCompany = queryPublicCompany.EndGetValue(queryPublicCompany2Ar);
            Assert.True(publicCompany.Club.Name == "UpdatedClubName");

            this.EnqueueTestComplete();
        }
        [Fact, Asynchronous]
        public void DerivedTypeSingletonClientTest()
        {
            TestClientContext = this.CreateWrappedContext<InMemoryEntities>().Context;
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;

            //Query Singleton
            var queryCompany = TestClientContext.PublicCompany as DataServiceQuerySingle<Company>;
            var queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            var company = queryCompany.EndGetValue(queryCompanyAr);

            //Singleton
            var loadLabourUnionAr = TestClientContext.BeginLoadProperty(company, "LabourUnion", null, null).EnqueueWait(this);
            TestClientContext.EndLoadProperty(loadLabourUnionAr);
            Assert.True((company as PublicCompany).LabourUnion != null);

            //Expand Navigation Property - Singleton
            queryCompany = TestClientContext.PublicCompany.Expand(c => (c as PublicCompany).Club) as DataServiceQuerySingle<Company>;
            queryCompanyAr = queryCompany.BeginGetValue(null, null).EnqueueWait(this);
            company = queryCompany.EndGetValue(queryCompanyAr);
            Assert.True((company as PublicCompany).Club != null);

            //Update Navigation property of derived Type - Singleton
            var labourUnion = (company as PublicCompany).LabourUnion;
            labourUnion.Name = "UpdatedLabourUnionName";
            TestClientContext.UpdateRelatedObject(company, "LabourUnion", labourUnion);
            var updateRelatedObjectAr = TestClientContext.BeginSaveChanges(null, null).EnqueueWait(this);
            TestClientContext.EndSaveChanges(updateRelatedObjectAr);

            //Projecton with Navigation property - singletonof derived type.            
            var queryPublicCompany = TestClientContext.PublicCompany.Select(c =>
                new PublicCompany { CompanyID = c.CompanyID, LabourUnion = (c as PublicCompany).LabourUnion }) as DataServiceQuerySingle<PublicCompany>;
            var queryPublicCompanyAr = queryPublicCompany.BeginGetValue(null, null).EnqueueWait(this);
            var publicCompany = queryPublicCompany.EndGetValue(queryPublicCompanyAr);
            Assert.True(publicCompany.LabourUnion != null);

            this.EnqueueTestComplete();
        }
    }
}