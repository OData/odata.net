//---------------------------------------------------------------------
// <copyright file="SingletonClientTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.SingletonTests
{
    using System;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.OData;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SingletonClientTests : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        public SingletonClientTests() : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {

        }

        [TestMethod]
        public void SingletonClientTest()
        {
            Random rand = new Random();
            ODataFormat[] formats = { ODataFormat.Json };
            foreach (var format in formats)
            {
                //Query Singleton
                TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
                var company = TestClientContext.Company.GetValue();
                Assert.IsTrue(company != null);

                //Update Singleton Property and Verify
                company.CompanyCategory = CompanyCategory.Communication;
                company.Name = "UpdatedName";
                company.Address.City = "UpdatedCity";
                TestClientContext.UpdateObject(company);
                TestClientContext.SaveChanges(Microsoft.OData.Client.SaveChangesOptions.ReplaceOnUpdate);

                //Query Singleton Property - Select
                var companyCategory = TestClientContext.Company.Select(c => c.CompanyCategory).GetValue();
                Assert.IsTrue(companyCategory == CompanyCategory.Communication);
                var city = TestClientContext.CreateSingletonQuery<string>("Company/Address/City").Execute().Single();
                Assert.IsTrue(city == "UpdatedCity");
                var name = TestClientContext.Execute<string>(new Uri("Company/Name", UriKind.Relative)).Single();
                Assert.IsTrue(name == "UpdatedName");

                //Projection with properties - Select
                company = TestClientContext.Company.Select(c => new Company { CompanyID = c.CompanyID, Address = c.Address, Name = c.Name }).GetValue();
                Assert.IsTrue(company != null);
                Assert.IsTrue(company.Name == "UpdatedName");

                //Load Navigation Property
                //Singleton
                TestClientContext.LoadProperty(company, "VipCustomer");
                Assert.IsTrue(company.VipCustomer != null);

                //Collection
                TestClientContext.LoadProperty(company, "Departments");
                Assert.IsTrue(company.Departments != null);
                Assert.IsTrue(company.Departments.Count > 0);

                //Single Entity
                TestClientContext.LoadProperty(company, "CoreDepartment");
                Assert.IsTrue(company.CoreDepartment != null);

                //Add Navigation Property - Collection
                company = TestClientContext.Company.GetValue();
                int tmpDepartmentID = rand.Next();
                int tmpCoreDepartmentID = rand.Next();
                Department department = new Department()
                {
                    DepartmentID = tmpDepartmentID,
                    Name = "ID" + tmpDepartmentID,
                };
                Department coreDepartment = new Department()
                {
                    DepartmentID = tmpCoreDepartmentID,
                    Name = "ID" + tmpCoreDepartmentID,
                };
                TestClientContext.AddToDepartments(department);
                TestClientContext.AddLink(company, "Departments", department);
                TestClientContext.SaveChanges();

                TestClientContext.AddToDepartments(coreDepartment);
                TestClientContext.AddLink(company, "Departments", coreDepartment);
                TestClientContext.SaveChanges();

                TestClientContext.Departments.ToList();
                //Projection with Navigation properties - Select
                company = TestClientContext.Company.Select(c => new Company { CompanyID = c.CompanyID, Departments = c.Departments }).GetValue();
                Assert.IsTrue(company != null);
                Assert.IsTrue(company.Departments.Any(c => c.DepartmentID == tmpDepartmentID));
                Assert.IsTrue(company.Departments.Any(c => c.DepartmentID == tmpCoreDepartmentID));

                //Update Navigation Property - Single Entity
                TestClientContext.SetLink(company, "CoreDepartment", coreDepartment);
                TestClientContext.SaveChanges();

                //Projection with Navigation properties - Select
                company = TestClientContext.Company.Select(c => new Company { CompanyID = c.CompanyID, CoreDepartment = c.CoreDepartment }).GetValue();
                Assert.IsTrue(company != null);
                Assert.IsTrue(company.CoreDepartment.DepartmentID == tmpCoreDepartmentID);

                //Update EntitySet's Navigation Property - Singleton 
                TestClientContext.SetLink(department, "Company", company);
                TestClientContext.SaveChanges(Microsoft.OData.Client.SaveChangesOptions.ReplaceOnUpdate);

                //Query(Expand) EntitySet's Navigation Property - Singleton
                department = TestClientContext.Departments.Expand(d => d.Company).Where(d => d.DepartmentID == tmpDepartmentID).Single();
                Assert.IsTrue(department != null);
                Assert.IsTrue(department.Company.CompanyID == company.CompanyID);

                //Delete Navigation Property - EntitySet
                TestClientContext.DeleteLink(company, "Departments", department);
                TestClientContext.SaveChanges();

                //Projection with Navigation Property - EntitySet
                company.Departments = null;
                company = TestClientContext.Company.Select(c => new Company { CompanyID = c.CompanyID, Departments = c.Departments }).GetValue();
                Assert.IsTrue(company != null);
                Assert.IsTrue(!company.Departments.Any(c => c.DepartmentID == tmpDepartmentID));

                //Query Singleton's Navigation Property - Singleton
                company = TestClientContext.Company.Select(c => new Company { CompanyID = c.CompanyID, VipCustomer = c.VipCustomer }).GetValue();
                Assert.IsTrue(company != null);
                Assert.IsTrue(company.VipCustomer != null);

                //Query Singleton again with Execute
                var vipCustomer = TestClientContext.Execute<Customer>(new Uri("VipCustomer", UriKind.Relative)).Single();

                //Update Singleton's Navigation property - Singleton
                vipCustomer.LastName = "UpdatedLastName";
                TestClientContext.UpdateRelatedObject(company, "VipCustomer", vipCustomer);
                TestClientContext.SaveChanges();

                company.VipCustomer = null;
                company = TestClientContext.Company.Expand(c => c.VipCustomer).GetValue();
                Assert.IsTrue(company != null);
                Assert.IsTrue(company.VipCustomer != null);
                Assert.IsTrue(company.VipCustomer.LastName == "UpdatedLastName");

                //Update Navigation Property - Delete the Singleton navigation
                TestClientContext.SetLink(company, "VipCustomer", null);
                TestClientContext.SaveChanges();

                //Expand Navigation Property - Singleton
                company.VipCustomer = null;
                company = TestClientContext.Company.Expand(c => c.VipCustomer).GetValue();
                Assert.IsTrue(company != null);
                Assert.IsTrue(company.VipCustomer == null);

                //TODO: AttachTo doesn't support singleton.
                //TestClientContext = new InMemoryEntities(ServiceBaseUri);
                //TestClientContext.AttachTo("Company", company);
                //TestClientContext.AttachTo("VipCustomer", vipCustomer);
                //TestClientContext.SetLink(company, "VipCustomer", vipCustomer);
                //TestClientContext.SaveChanges();

                //Update Navigation Property - Singleton
                vipCustomer = TestClientContext.VipCustomer.GetValue();
                TestClientContext.SetLink(company, "VipCustomer", vipCustomer);
                TestClientContext.SaveChanges();
                company = TestClientContext.Company.Select(c => new Company { CompanyID = c.CompanyID, VipCustomer = c.VipCustomer }).GetValue();
                Assert.IsTrue(company != null);
                Assert.IsTrue(company.VipCustomer != null);
            }
        }

        [TestMethod]
        public void DerivedTypeSingletonClientTest()
        {
            //Query Singleton
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            var company = TestClientContext.PublicCompany.GetValue();
            Assert.IsTrue(company != null);

            //Update DerivedType Property and Verify
            PublicCompany publicCompany = company as PublicCompany;
            publicCompany.Name = "UpdatedName";
            publicCompany.StockExchange = "Updated StockExchange";
            TestClientContext.UpdateObject(publicCompany);
            TestClientContext.SaveChanges(Microsoft.OData.Client.SaveChangesOptions.ReplaceOnUpdate);

            //Query Singleton Property - Select            
            var name = TestClientContext.PublicCompany.Select(c => c.Name).GetValue();
            Assert.IsTrue(name == "UpdatedName");
            company = TestClientContext.CreateSingletonQuery<Company>("PublicCompany").Single();
            Assert.IsTrue((company as PublicCompany).StockExchange == "Updated StockExchange");

            //Query properties of DerivedType
            var stockExchange = TestClientContext.PublicCompany.Select(c => (c as PublicCompany).StockExchange).GetValue();
            Assert.IsTrue(stockExchange == "Updated StockExchange");

            //Projection with properties - Select
            publicCompany = TestClientContext.PublicCompany.Select(c =>
                new PublicCompany { CompanyID = c.CompanyID, Name = c.Name, StockExchange = (c as PublicCompany).StockExchange }).GetValue();
            Assert.IsTrue(publicCompany != null);
            Assert.IsTrue(publicCompany.Name == "UpdatedName");
            Assert.IsTrue(publicCompany.StockExchange == "Updated StockExchange");

            company = TestClientContext.CreateSingletonQuery<Company>("PublicCompany").Single();

            //Load Navigation Property
            //Collection            
            TestClientContext.LoadProperty(company, "Assets");
            Assert.IsTrue((company as PublicCompany).Assets != null);
            Assert.IsTrue((company as PublicCompany).Assets.Count == 2);

            ////Single Enity
            TestClientContext.LoadProperty(company, "Club");
            Assert.IsTrue((company as PublicCompany).Club != null);

            //Singleton
            TestClientContext.LoadProperty(publicCompany, "LabourUnion");
            Assert.IsTrue((company as PublicCompany).LabourUnion != null);

            //Add Contained Navigation Property - Collection of derived type
            Random rand = new Random();
            int tmpAssertId = rand.Next();
            Asset tmpAssert = new Asset()
            {
                AssetID = tmpAssertId,
                Name = tmpAssertId + "Name",
                Number = tmpAssertId
            };

            TestClientContext.AddRelatedObject(publicCompany, "Assets", tmpAssert);
            TestClientContext.SaveChanges();

            //Query contained Navigation Property - Collection of derived type
            company = TestClientContext.PublicCompany.Expand(c => (c as PublicCompany).Assets).GetValue();
            Assert.IsTrue(company != null);
            Assert.IsTrue((company as PublicCompany).Assets.Any(a => a.AssetID == tmpAssertId));

            TestClientContext.DeleteObject(tmpAssert);
            TestClientContext.SaveChanges();

            company = TestClientContext.PublicCompany.Expand(c => (c as PublicCompany).Assets).GetValue();
            Assert.IsTrue(company != null);
            Assert.IsTrue(!(company as PublicCompany).Assets.Any(a => a.AssetID == tmpAssertId));

            company = TestClientContext.PublicCompany.Expand(c => (c as PublicCompany).Club).GetValue();
            Assert.IsTrue(company != null);
            Assert.IsTrue((company as PublicCompany).Club != null);

            //Updated Conatined Navigation Property - SingleEntity of derived type
            var club = (company as PublicCompany).Club;
            club.Name = "UpdatedClubName";
            TestClientContext.UpdateRelatedObject(company, "Club", club);
            TestClientContext.SaveChanges();

            //Query Contained Navigation Property - Single Entity of derived type
            publicCompany = TestClientContext.PublicCompany.Select(c => new PublicCompany { CompanyID = c.CompanyID, Club = (c as PublicCompany).Club }).GetValue();
            Assert.IsTrue(publicCompany != null);
            Assert.IsTrue(publicCompany.Club != null);
            Assert.IsTrue(publicCompany.Club.Name == "UpdatedClubName");

            //Projection with Navigation property of derived type - Singleton
            company = TestClientContext.PublicCompany.Expand(c => (c as PublicCompany).LabourUnion).GetValue();

            //Update Navigation property of derived Type - Singleton
            var labourUnion = (company as PublicCompany).LabourUnion;
            labourUnion.Name = "UpdatedLabourUnionName";
            TestClientContext.UpdateRelatedObject(publicCompany, "LabourUnion", labourUnion);
            TestClientContext.SaveChanges();

            //Query singleton of derived type.            
            publicCompany = TestClientContext.PublicCompany.Select(c => new PublicCompany { CompanyID = c.CompanyID, LabourUnion = (c as PublicCompany).LabourUnion }).GetValue();
            Assert.IsTrue(publicCompany != null);
            Assert.IsTrue(publicCompany.LabourUnion != null);
        }
        #region Action/Function
        [TestMethod]
        public void InvokeFunctionBoundedToSingleton()
        {
            var employeeCount = TestClientContext.Execute<int>(new Uri(ServiceBaseUri.AbsoluteUri + "Company/Microsoft.Test.OData.Services.ODataWCFService.GetEmployeesCount()", UriKind.Absolute)).Single();
            Assert.AreEqual(2, employeeCount);

            Company company = TestClientContext.Company.GetValue();
            OperationDescriptor descriptor = TestClientContext.GetEntityDescriptor(company).OperationDescriptors.Single(e => e.Title == "Microsoft.Test.OData.Services.ODataWCFService.GetEmployeesCount");
            employeeCount = TestClientContext.Execute<int>(descriptor.Target, "GET", true).Single();
            Assert.AreEqual(2, employeeCount);
        }

        [TestMethod]
        public void InvokeActionBoundedToSingleton()
        {
            Company company = TestClientContext.Company.GetValue();
            TestClientContext.LoadProperty(company, "Revenue");

            var newValue = TestClientContext.Execute<Int64>(new Uri(ServiceBaseUri.AbsoluteUri + "Company/Microsoft.Test.OData.Services.ODataWCFService.IncreaseRevenue"), "POST", true, new BodyOperationParameter("IncreaseValue", 20000));
            Assert.AreEqual(newValue.Single(), company.Revenue + 20000);

            OperationDescriptor descriptor = TestClientContext.GetEntityDescriptor(company).OperationDescriptors.Single(e => e.Title == "Microsoft.Test.OData.Services.ODataWCFService.IncreaseRevenue");
            newValue = TestClientContext.Execute<Int64>(descriptor.Target, "POST", new BodyOperationParameter("IncreaseValue", 40000));
            Assert.AreEqual(newValue.Single(), company.Revenue + 60000);
        }

        #endregion
    }
}
