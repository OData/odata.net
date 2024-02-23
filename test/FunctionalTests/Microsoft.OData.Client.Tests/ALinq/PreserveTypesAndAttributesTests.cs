//---------------------------------------------------------------------
// <copyright file="PreserveTypesAndAttributesTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Microsoft.OData.Client.Tests.ALinq
{
    /// <summary>
    /// Tests to check original resource type is used when retrieving attributes like OriginalNameAttribute
    /// even if the IQueryable<T> object is casted to another type.
    /// </summary>
    public class PreserveTypesAndAttributesTests
    {
        private readonly TestContext ctx;

        public PreserveTypesAndAttributesTests()
        {
            string uri = "https://services.odata.org/V4/Northwind/Northwind.svc";
            this.ctx = new TestContext(new Uri(uri));
        }

        [Fact]
        public void OneLevelCase1()
        {
            IQueryable<ICustomer> q1 = ctx.OneLevelCustomers1;
            var r1 = q1.Where(c => c.CorporationName != "").OrderBy(c => c.Id).Take(5).ToList();
                        
            IQueryable<OneLevelCustomer1> q2 = ctx.OneLevelCustomers1;
            var r2 = q2.Where(c => c.CorporationName != "").OrderBy(c => c.Id).Take(5).ToList();
            
            Assert.Equal(r1.Count, r2.Count);
            for (var i = 0; i < r1.Count; i++) {
                Assert.Equal(r1[i].City, r2[i].City);
                Assert.Equal(r1[i].CorporationName, r2[i].CorporationName);
                Assert.Equal(r1[i].Name, r2[i].Name);
                Assert.Equal(r1[i].Id, r2[i].Id);
            }
        }
        
        [Fact]
        public void OneLevelCase2()
        {
            IQueryable<BaseCustomer> q1 = ctx.OneLevelCustomers2;
            var r1 = q1.Where(c => c.Id != "").OrderBy(c => c.Id).Select(c => new { c.Id }).Take(5).ToList();
                        
            IQueryable<OneLevelCustomer2> q2 = ctx.OneLevelCustomers2;
            var r2 = q2.Where(c => c.Id != "").OrderBy(c => c.Id).Select(c => new { c.Id }).Take(5).ToList();
            
            Assert.Equal(r1.Count, r2.Count);
            for (var i = 0; i < r1.Count; i++) {
                Assert.Equal(r1[i].Id, r2[i].Id);
            }
        }

        [Fact]
        public void MoreThanOneLevelLevelCase1()
        {
            IQueryable<ICustomer> q1 = ctx.TwoLevelCustomers1;
            q1 = q1.Where(c => c.Name.Contains("a"));
            var r1 = q1.Where(c => c.Id != "").OrderBy(c => c.Id).Take(5).ToList();

            IQueryable<ACustomer> q2 = ctx.TwoLevelCustomers1;
            q2 = q2.Where(c => c.Name.Contains("a"));
            var r2 = q2.Where(c => c.Id != "").OrderBy(c => c.Id).Take(5).ToList();

            IQueryable<TwoLevelCustomer1> q3 = ctx.TwoLevelCustomers1;
            q3 = q3.Where(c => c.Name.Contains("a"));
            var r3 = q3.Where(c => c.Id != "").OrderBy(c => c.Id).Take(5).ToList();
            
            Assert.Equal(r1.Count, r2.Count);
            for (var i = 0; i < r1.Count; i++) {
                Assert.Equal(r1[i].Id, r2[i].Id);
                Assert.Equal(r1[i].CorporationName, r2[i].CorporationName);
                Assert.Equal(r1[i].City, r2[i].City);
                Assert.Equal(r1[i].Name, r2[i].Name);
            }
            Assert.Equal(r3.Count, r2.Count);
            for (var i = 0; i < r1.Count; i++) {
                Assert.Equal(r3[i].Id, r2[i].Id);
                Assert.Equal(r3[i].CorporationName, r2[i].CorporationName);
                Assert.Equal(r3[i].City, r2[i].City);
                Assert.Equal(r3[i].Name, r2[i].Name);
            }
        }

        [Fact]
        public void MoreThanOneLevelLevelCase2()
        {
            IQueryable<ICustomer> q1 = ctx.TwoLevelCustomers2.Expand(c => c.OrdersList);
            q1 = q1.Where(c => c.Name.Contains("a"));
            var r1 = q1.Where(c => c.Id != "").OrderBy(c => c.Id).Take(5).ToList();

            IQueryable<ICustomer2> q2 = ctx.TwoLevelCustomers2.Expand(c => c.OrdersList);
            q2 = q2.Where(c => c.Name.Contains("a"));
            var r2 = q2.Where(c => c.Id != "").OrderBy(c => c.Id).Take(5).ToList();

            IQueryable<TwoLevelCustomer2> q3 = ctx.TwoLevelCustomers2.Expand(c => c.OrdersList);
            q3 = q3.Where(c => c.Name.Contains("a"));
            var r3 = q3.Where(c => c.Id != "").OrderBy(c => c.Id).Take(5).ToList();

            Assert.Equal(r1.Count, r2.Count);
            for (var i = 0; i < r1.Count; i++) {
                Assert.Equal(r1[i].Id, r2[i].Id);
                Assert.Equal(r1[i].CorporationName, r2[i].CorporationName);
                Assert.Equal(r1[i].City, r2[i].City);
                Assert.Equal(r1[i].Name, r2[i].Name);
            }
            Assert.Equal(r3.Count, r2.Count);
            for (var i = 0; i < r1.Count; i++) {
                Assert.Equal(r3[i].Id, r2[i].Id);
                Assert.Equal(r3[i].CorporationName, r2[i].CorporationName);
                Assert.Equal(r3[i].City, r2[i].City);
                Assert.Equal(r3[i].Name, r2[i].Name);
            }
        }
    }


    // One level of inheritance classes

    public interface ICustomer
    {
        string Id { get; set; }
        string City { get; set; }
        string CorporationName { get; set; }
        string Name { get; set; }
    }
    
    [Key("CustomerID")]
    public class OneLevelCustomer1 : ICustomer
    {
        [OriginalName("CustomerID")]
        public string Id { get; set; }

        public string City { get; set; }

        [OriginalName("CompanyName")]
        public string CorporationName { get; set; }

        [OriginalName("ContactName")]
        public string Name { get; set; }
    }

    [Key("CustomerID")]
    public class BaseCustomer
    {
        [OriginalName("CustomerID")]
        public string Id { get; set; }
    }

    public class OneLevelCustomer2 : BaseCustomer
    {
        public string City { get; set; }

        [OriginalName("CompanyName")]
        public string CorporationName { get; set; }

        [OriginalName("ContactName")]
        public string Name { get; set; }
    }


    // More than one level of inheritance classes

    public abstract class ACustomer : ICustomer
    {
        public virtual string Id { get; set; }
        public virtual string City { get; set; }
        public virtual string CorporationName { get; set; }
        public virtual string Name { get; set; }
    }

    [Key("CustomerID")]
    public class TwoLevelCustomer1 : ACustomer
    {
        [OriginalName("CustomerID")]
        public override string Id { get; set; }

        public override string City { get; set; }

        [OriginalName("CompanyName")]
        public override string CorporationName { get; set; }

        [OriginalName("ContactName")]
        public override string Name { get; set; }
    }

    public interface ICustomer2 : ICustomer
    {
        List<Order> OrdersList { get; set; }
    }

    [Key("CustomerID")]
    public class TwoLevelCustomer2 : ICustomer2
    {
        [OriginalName("CustomerID")]
        public string Id { get; set; }

        public string City { get; set; }

        [OriginalName("CompanyName")]
        public string CorporationName { get; set; }

        [OriginalName("ContactName")]
        public string Name { get; set; }

    	[OriginalName("Orders")]
        public List<Order> OrdersList { get; set; }
    }

    [Key("OrderID")]
    public class Order
    {
        public int OrderID { get; set; }
        public decimal Freight { get; set; }
    }


    public class TestContext
    {
        public DataServiceQuery<OneLevelCustomer1> OneLevelCustomers1;
        public DataServiceQuery<TwoLevelCustomer1> TwoLevelCustomers1;
        public DataServiceQuery<OneLevelCustomer2> OneLevelCustomers2;
        public DataServiceQuery<TwoLevelCustomer2> TwoLevelCustomers2;

        // We have to create multiple DataServiceContext because the entitySetName is the same for every DataServiceQuery
        DataServiceContext ctx1;
        DataServiceContext ctx2;
        DataServiceContext ctx3;
        DataServiceContext ctx4;

        public TestContext(Uri serviceRoot)
        {
            ctx1 = new DataServiceContext(serviceRoot);
            ctx2 = new DataServiceContext(serviceRoot);
            ctx3 = new DataServiceContext(serviceRoot);
            ctx4 = new DataServiceContext(serviceRoot);
            OneLevelCustomers1 = ctx1.CreateQuery<OneLevelCustomer1>("Customers");
            TwoLevelCustomers1 = ctx2.CreateQuery<TwoLevelCustomer1>("Customers");
            OneLevelCustomers2 = ctx3.CreateQuery<OneLevelCustomer2>("Customers");
            TwoLevelCustomers2 = ctx4.CreateQuery<TwoLevelCustomer2>("Customers");
        }
    }
}
