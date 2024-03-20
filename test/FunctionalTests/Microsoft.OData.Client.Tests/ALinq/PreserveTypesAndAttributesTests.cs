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
            IQueryable<ICustomer> query1 = ctx.OneLevelCustomers1;
            var result1 = query1.Where(c => c.CorporationName != "").OrderBy(c => c.Id).Take(5).ToList();
                        
            IQueryable<OneLevelCustomer1> query2 = ctx.OneLevelCustomers1;
            var result2 = query2.Where(c => c.CorporationName != "").OrderBy(c => c.Id).Take(5).ToList();
            
            Assert.Equal(result1.Count, result2.Count);
            for (var i = 0; i < result1.Count; i++) {
                Assert.Equal(result1[i].City, result2[i].City);
                Assert.Equal(result1[i].CorporationName, result2[i].CorporationName);
                Assert.Equal(result1[i].Name, result2[i].Name);
                Assert.Equal(result1[i].Id, result2[i].Id);
            }
        }
        
        [Fact]
        public void OneLevelCase2()
        {
            IQueryable<BaseCustomer> query1 = ctx.OneLevelCustomers2;
            var result1 = query1.Where(c => c.Id != "").OrderBy(c => c.Id).Select(c => new { c.Id }).Take(5).ToList();
                        
            IQueryable<OneLevelCustomer2> query2 = ctx.OneLevelCustomers2;
            var result2 = query2.Where(c => c.Id != "").OrderBy(c => c.Id).Select(c => new { c.Id }).Take(5).ToList();
            
            Assert.Equal(result1.Count, result2.Count);
            for (var i = 0; i < result1.Count; i++) {
                Assert.Equal(result1[i].Id, result2[i].Id);
            }
        }

        [Fact]
        public void MoreThanOneLevelLevelCase1()
        {
            IQueryable<ICustomer> query1 = ctx.TwoLevelCustomers1;
            query1 = query1.Where(c => c.Name.Contains("a"));
            var result1 = query1.Where(c => c.Id != "").OrderBy(c => c.Id).Take(5).ToList();

            IQueryable<ACustomer> query2 = ctx.TwoLevelCustomers1;
            query2 = query2.Where(c => c.Name.Contains("a"));
            var result2 = query2.Where(c => c.Id != "").OrderBy(c => c.Id).Take(5).ToList();

            IQueryable<TwoLevelCustomer1> query3 = ctx.TwoLevelCustomers1;
            query3 = query3.Where(c => c.Name.Contains("a"));
            var result3 = query3.Where(c => c.Id != "").OrderBy(c => c.Id).Take(5).ToList();
            
            Assert.Equal(result1.Count, result2.Count);
            for (var i = 0; i < result1.Count; i++) {
                Assert.Equal(result1[i].Id, result2[i].Id);
                Assert.Equal(result1[i].CorporationName, result2[i].CorporationName);
                Assert.Equal(result1[i].City, result2[i].City);
                Assert.Equal(result1[i].Name, result2[i].Name);
            }
            Assert.Equal(result3.Count, result2.Count);
            for (var i = 0; i < result1.Count; i++) {
                Assert.Equal(result3[i].Id, result2[i].Id);
                Assert.Equal(result3[i].CorporationName, result2[i].CorporationName);
                Assert.Equal(result3[i].City, result2[i].City);
                Assert.Equal(result3[i].Name, result2[i].Name);
            }
        }

        [Fact]
        public void MoreThanOneLevelLevelCase2()
        {
            IQueryable<ICustomer> query1 = ctx.TwoLevelCustomers2.Expand(c => c.OrdersList);
            query1 = query1.Where(c => c.Name.Contains("a"));
            var result1 = query1.Where(c => c.Id != "").OrderBy(c => c.Id).Take(5).ToList();

            IQueryable<ICustomer2> query2 = ctx.TwoLevelCustomers2.Expand(c => c.OrdersList);
            query2 = query2.Where(c => c.Name.Contains("a"));
            var result2 = query2.Where(c => c.Id != "").OrderBy(c => c.Id).Take(5).ToList();

            IQueryable<TwoLevelCustomer2> query3 = ctx.TwoLevelCustomers2.Expand(c => c.OrdersList);
            query3 = query3.Where(c => c.Name.Contains("a"));
            var result3 = query3.Where(c => c.Id != "").OrderBy(c => c.Id).Take(5).ToList();

            Assert.Equal(result1.Count, result2.Count);
            for (var i = 0; i < result1.Count; i++) {
                Assert.Equal(result1[i].Id, result2[i].Id);
                Assert.Equal(result1[i].CorporationName, result2[i].CorporationName);
                Assert.Equal(result1[i].City, result2[i].City);
                Assert.Equal(result1[i].Name, result2[i].Name);
            }
            Assert.Equal(result3.Count, result2.Count);
            for (var i = 0; i < result1.Count; i++) {
                Assert.Equal(result3[i].Id, result2[i].Id);
                Assert.Equal(result3[i].CorporationName, result2[i].CorporationName);
                Assert.Equal(result3[i].City, result2[i].City);
                Assert.Equal(result3[i].Name, result2[i].Name);
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
