//---------------------------------------------------------------------
// <copyright file="Dev10EF4FKTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.EFFK.Client
{
    using System;
    using System.Collections.Generic;

    public class Customer
    {
        public Customer()
        {
            this.Orders = new List<Order>();
            this.Address = new Address()
            {
                StreetAddress = "Line1",
                City = "Redmond",
                State = "WA",
                PostalCode = "98052"
            };
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Concurrency { get; set; }
        public byte[] EditTimeStamp { get; set; }
        public Guid? GuidValue { get; set; }
        public Address Address { get; set; }
        public List<Order> Orders { get; set; }
        public Customer BestFriend { get; set; }
    }

    public class Order
    {
        public int ID { get; set; }
        public double DollarAmount { get; set; }
        public Customer Customers { get; set; }
        public int? CustomerId { get; set; }
    }

    public class NarrowCustomerWithNavOrder
    {
        public NarrowCustomerWithNavOrder()
        {
            this.Orders = new List<NarrowOrderNavOnly>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public List<NarrowOrderNavOnly> Orders { get; set; }
    }

    public class NarrowOrderNavOnly
    {
        public int ID { get; set; }
        public NarrowCustomerWithNavOrder Customers { get; set; }
    }

    public class NarrowCustomerWithFKOrder
    {
        public NarrowCustomerWithFKOrder()
        {
            this.Orders = new List<NarrowOrderFKOnly>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public List<NarrowOrderFKOnly> Orders { get; set; }
    }

    public class NarrowOrderFKOnly
    {
        public int ID { get; set; }
        public int? CustomerId { get; set; }
    }

    public class CustomerWithBirthday : Customer
    {
        public CustomerWithBirthday()
            : base()
        {
            // This is what EF generated code does to make sure the default datetime is a valid value in SQL
            this.Birthday = new DateTime(624235248000000000, DateTimeKind.Unspecified);
        }

        public DateTimeOffset Birthday { get; set; }
    }

    public class Address
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }

    public class Worker
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public Office Office { get; set; }
    }

    public class Office
    {
        public int ID { get; set; }
        public int OfficeNumber { get; set; }
        public Int16 FloorNumber { get; set; }
        public string BuildingName { get; set; }
        public Worker Worker { get; set; }
    }
}

namespace AstoriaUnitTests.Tests
{
    using System;
    using Microsoft.OData.Client;
    using System.Linq;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using EFFK = AstoriaUnitTests.EFFK;
    using EFFKClient = AstoriaUnitTests.EFFK.Client;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/880
    [TestClass]
    public class EFFKTests
    {
        static TestWebRequest web;
        static IDisposable changeScope;

        DataServiceContext ctx;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            changeScope = EFFK.CustomObjectContextPOCOProxy.CreateChangeScope();
            web = TestWebRequest.CreateForInProcessWcf();
            web.DataServiceType = typeof(EFFK.CustomObjectContextPOCOProxy);
            OpenWebDataServiceHelper.ForceVerboseErrors = true;
            web.StartService();
        }

        [ClassCleanup]
        public static void PerClassCleanUp()
        {
            OpenWebDataServiceHelper.ForceVerboseErrors = false;
            if (web != null)
            {
                web.StopService();
            }

            if (changeScope != null)
            {
                changeScope.Dispose();
            }
        }

        [TestInitialize]
        public void PerTestSetup()
        {
            this.ctx = new DataServiceContext(web.ServiceRoot);
            //this.ctx.EnableAtom = true;
            //this.ctx.Format.UseAtom();
            this.ctx.ResolveName = (t) =>
            {
                string clientName = t.FullName;

                if (clientName.Contains("Narrow"))
                {
                    return null;
                }

                return clientName.Replace("AstoriaUnitTests.EFFK.Client", "AstoriaUnitTests.ObjectContextStubs.Types");
            };

            this.ctx.ResolveType = (name) =>
            {
                string clientTypeName = name.Replace("AstoriaUnitTests.ObjectContextStubs.Types", "AstoriaUnitTests.EFFK.Client");
                return Type.GetType(clientTypeName);
            };
        }

        [TestCleanup]
        public void PerTestCleanUp()
        {
            this.ctx = null;
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void EFFK_Get_SimpleGet()
        {
            var q = ctx.CreateQuery<EFFKClient.Customer>("CustomObjectContext.Customers").Expand("Orders($expand=Customers)");
            foreach (var c in q)
            {
                foreach (var o in c.Orders)
                {
                    Assert.AreEqual(c.ID, o.Customers.ID);
                    Assert.AreEqual(c.ID, o.CustomerId);
                }
            }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void EFFK_Get_ProjectNavOnly()
        {
            var q = ctx.CreateQuery<EFFKClient.Customer>("CustomObjectContext.Customers").Select(c => new EFFKClient.NarrowCustomerWithNavOrder()
            {
                ID = c.ID,
                Name = c.Name,
                Orders = c.Orders.Select(o => new EFFKClient.NarrowOrderNavOnly()
                {
                    ID = o.ID,
                    Customers = new EFFKClient.NarrowCustomerWithNavOrder() { ID = o.Customers.ID }
                }).ToList()
            });

            foreach (var c in q)
            {
                foreach (var o in c.Orders)
                {
                    Assert.AreEqual(c.ID, o.Customers.ID);
                }
            }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void EFFK_Get_ProjectFKOnly()
        {
            var q = ctx.CreateQuery<EFFKClient.Customer>("CustomObjectContext.Customers").Select(c => new EFFKClient.NarrowCustomerWithFKOrder()
            {
                ID = c.ID,
                Name = c.Name,
                Orders = c.Orders.Select(o => new EFFKClient.NarrowOrderFKOnly()
                {
                    ID = o.ID,
                    CustomerId = o.CustomerId
                }).ToList()
            });

            foreach (var c in q)
            {
                foreach (var o in c.Orders)
                {
                    Assert.AreEqual(c.ID, o.CustomerId);
                }
            }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void EFFK_Update_SimpleUpdate()
        {
            var cust = ctx.CreateQuery<EFFKClient.Customer>("CustomObjectContext.Customers").Expand("Orders($expand=Customers)").FirstOrDefault();
            var order = cust.Orders.FirstOrDefault();

            // set FK
            order.CustomerId = 1;
            ctx.UpdateObject(order);
            ctx.SaveChanges();
            VerifyServerOrderId(order.ID, 1);

            // set Link
            ctx.SetLink(order, "Customers", cust);
            ctx.SaveChanges();
            VerifyServerOrderId(order.ID, cust.ID);

            // set both FK and Link
            order.CustomerId = 2;
            ctx.UpdateObject(order);
            ctx.SetLink(order, "Customers", cust);
            ctx.SaveChanges();
            VerifyServerOrderId(order.ID, cust.ID);
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void EFFK_Update_ProjectedUpdate()
        {
            var cust = ctx.CreateQuery<EFFKClient.Customer>("CustomObjectContext.Customers").Select(c => new EFFKClient.Customer()
            {
                ID = c.ID,
                Orders = c.Orders.Select(o => new EFFKClient.Order()
                {
                    ID = o.ID
                }).ToList()
            }).FirstOrDefault();

            var order = cust.Orders.FirstOrDefault();

            // set FK
            order.CustomerId = 1;
            ctx.UpdateObject(order);
            ctx.SaveChanges();
            VerifyServerOrderId(order.ID, 1);

            // set Link
            ctx.SetLink(order, "Customers", cust);
            ctx.SaveChanges();
            VerifyServerOrderId(order.ID, cust.ID);

            // set both FK and Link
            order.CustomerId = 2;
            ctx.UpdateObject(order);
            ctx.SetLink(order, "Customers", cust);
            ctx.SaveChanges();
            VerifyServerOrderId(order.ID, cust.ID);
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void EFFK_Update_NarrowingUpdate1()
        {
            var cust = ctx.CreateQuery<EFFKClient.Customer>("CustomObjectContext.Customers").Select(c => new EFFKClient.NarrowCustomerWithFKOrder()
            {
                ID = c.ID,
                Orders = c.Orders.Select(o => new EFFKClient.NarrowOrderFKOnly()
                {
                    ID = o.ID,
                    CustomerId = o.CustomerId
                }).ToList()
            }).FirstOrDefault();

            var order = cust.Orders.FirstOrDefault();

            // set FK
            order.CustomerId = 1;
            ctx.UpdateObject(order);
            ctx.SaveChanges();
            VerifyServerOrderId(order.ID, 1);

            // Add Link
            ctx.DetachLink(cust, "Orders", order);
            ctx.AddLink(cust, "Orders", order);
            ctx.SaveChanges();
            VerifyServerOrderId(order.ID, cust.ID);

            // Add Link + Set FK
            order.CustomerId = 2;
            ctx.DetachLink(cust, "Orders", order);
            ctx.AddLink(cust, "Orders", order);
            ctx.SaveChanges();
            VerifyServerOrderId(order.ID, cust.ID);
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void EFFK_Update_NarrowingUpdate2()
        {
            var cust = ctx.CreateQuery<EFFKClient.Customer>("CustomObjectContext.Customers").Select(c => new EFFKClient.NarrowCustomerWithNavOrder()
            {
                ID = c.ID,
                Orders = c.Orders.Select(o => new EFFKClient.NarrowOrderNavOnly()
                {
                    ID = o.ID,
                    Customers = new EFFKClient.NarrowCustomerWithNavOrder()
                    {
                        ID = o.Customers.ID
                    }
                }).ToList()
            }).Where(c => c.ID == 1).FirstOrDefault();

            var cust2 = ctx.CreateQuery<EFFKClient.Customer>("CustomObjectContext.Customers").Select(c => new EFFKClient.NarrowCustomerWithNavOrder()
            {
                ID = c.ID
            }).Where(c => c.ID == 2).FirstOrDefault();

            var order = cust.Orders.FirstOrDefault();

            // Set Link
            ctx.SetLink(order, "Customers", cust2);
            ctx.SaveChanges();
            VerifyServerOrderId(order.ID, 2);

            // Add link
            ctx.DetachLink(cust, "Orders", order);
            ctx.AddLink(cust, "Orders", order);
            ctx.SaveChanges();
            VerifyServerOrderId(order.ID, 1);
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void EFFK_InsertTest()
        {
            var newCust = new EFFKClient.Customer() { ID = 10, Name = "New Cust", EditTimeStamp = new byte[] { 0, 1, 2, 3 } };
            var cust1 = ctx.CreateQuery<EFFKClient.Customer>("CustomObjectContext.Customers").Where(c => c.ID == 1).FirstOrDefault();

            var orders = new Tuple<EFFKClient.Order, int>[]
            {
                Tuple.Create(new EFFKClient.Order(){ ID=100000, Customers=null },  0),                          // Set None
                Tuple.Create(new EFFKClient.Order(){ ID=100007, CustomerId =10 }, 10),                          // Set FK only, new
                Tuple.Create(new EFFKClient.Order(){ ID=100008, Customers = newCust }, 10),                     // Set Nav only, new
                Tuple.Create(new EFFKClient.Order(){ ID=100009, CustomerId =10, Customers=newCust }, 10),       // Set FK + Nav to same, new
                Tuple.Create(new EFFKClient.Order(){ ID=100010, CustomerId =1, Customers=newCust }, 10),        // Set FK + Nav to diff, new

                // Order Object | Expected Customer ID
                Tuple.Create(new EFFKClient.Order(){ ID=100001, CustomerId =1, Customers=null },  1),           // Set FK only
                Tuple.Create(new EFFKClient.Order(){ ID=100002, Customers=cust1 },  1),                         // Set Nav only
                Tuple.Create(new EFFKClient.Order(){ ID=100003, CustomerId =1, Customers=cust1 }, 1),           // Set FK + Nav to same
                Tuple.Create(new EFFKClient.Order(){ ID=100004, CustomerId =2, Customers=null }, 1),            // Set FK + Nav to diff
                Tuple.Create(new EFFKClient.Order(){ ID=100005, CustomerId =2, Customers=cust1 }, 1),           // Set FK + Nav to diff
                Tuple.Create(new EFFKClient.Order(){ ID=100006, CustomerId =2, Customers=newCust }, 1),         // Set FK + Nav to diff
            };

            ctx.AddObject("CustomObjectContext.Customers", newCust);
            foreach (var t in orders)
            {
                ctx.AddObject("CustomObjectContext.Orders", t.Item1);
                if (t.Item1.CustomerId == 2)
                {
                    ctx.SetLink(t.Item1, "Customers", cust1);
                }
                else if (t.Item1.Customers != null)
                {
                    ctx.SetLink(t.Item1, "Customers", t.Item1.Customers);
                }
                ctx.SaveChanges();

                if (t.Item2 == 0)
                {
                    VerifyServerOrderId(t.Item1.ID, null);
                }
                else
                {
                    VerifyServerOrderId(t.Item1.ID, t.Item2);
                }
            }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void EFFK_InsertAddRelated()
        {
            var newCust = new EFFKClient.Customer() { ID = 11, EditTimeStamp = new byte[] { 0 } };
            var newOrder1 = new EFFKClient.Order() { ID = 100020, CustomerId = 11 };
            var newOrder2 = new EFFKClient.Order() { ID = 100021, CustomerId = 1 };
            var newOrder3 = new EFFKClient.Order() { ID = 100022 };

            ctx.AddObject("CustomObjectContext.Customers", newCust);
            ctx.AddRelatedObject(newCust, "Orders", newOrder1);
            ctx.AddRelatedObject(newCust, "Orders", newOrder2);
            ctx.AddRelatedObject(newCust, "Orders", newOrder3);

            ctx.SaveChanges();

            VerifyServerOrderId(100020, 11);
            VerifyServerOrderId(100021, 11);
            VerifyServerOrderId(100022, 11);
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void EFFK_DeleteLinks()
        {
            var cust = ctx.CreateQuery<EFFKClient.Customer>("CustomObjectContext.Customers").Expand("Orders($expand=Customers)").FirstOrDefault();
            var order = cust.Orders.FirstOrDefault();

            // delete link via FK
            order.CustomerId = null;
            ctx.UpdateObject(order);
            ctx.SaveChanges();
            VerifyServerOrderId(order.ID, null);

            order.CustomerId = cust.ID;
            ctx.UpdateObject(order);
            ctx.SaveChanges();
            VerifyServerOrderId(order.ID, cust.ID);

            // delete link via Set Link
            ctx.SetLink(order, "Customers", null);
            ctx.SaveChanges();
            VerifyServerOrderId(order.ID, null);

            order.CustomerId = cust.ID;
            ctx.UpdateObject(order);
            ctx.SaveChanges();
            VerifyServerOrderId(order.ID, cust.ID);

            // delete link via DeleteLink
            ctx.DeleteLink(cust, "Orders", order);
            ctx.SaveChanges();
            VerifyServerOrderId(order.ID, null);
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void EFFK_SettingTheSameRelationshipAgain()
        {
            var cust = ctx.CreateQuery<EFFKClient.Customer>("CustomObjectContext.Customers").Expand("Orders($expand=Customers)").FirstOrDefault();
            var order = cust.Orders.FirstOrDefault();

            ctx.SetLink(order, "Customers", cust);
            ctx.SaveChanges();
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void EFFK_1To1_BasicInsert_Bind_Delete()
        {
            // Create new office type
            EFFKClient.Office o = new EFFKClient.Office() { ID = 1, BuildingName = "Building 35", FloorNumber = 2, OfficeNumber = 2173 };
            ctx.AddObject("CustomObjectContext.Offices", o);

            // create new employee type
            EFFKClient.Worker e = new EFFKClient.Worker() { ID = 1, FirstName = "Pratik", LastName = "Patel" };
            ctx.AddObject("CustomObjectContext.Workers", e);
            ctx.SaveChanges();

            // Establish relationship between employee and office again. This operation should be no-op
            ctx.SetLink(o, "Worker", e);
            ctx.SaveChanges();

            ctx.SetLink(e, "Office", o);
            ctx.SaveChanges();

            // clean the tests by deleting the office instance created by this test
            ctx.DeleteObject(e);
            ctx.DeleteObject(o);
            ctx.SaveChanges();

            Assert.AreEqual(ctx.CreateQuery<EFFKClient.Worker>("CustomObjectContext.Workers").Count(), 0, "There should be no workers left");
            Assert.AreEqual(ctx.CreateQuery<EFFKClient.Office>("CustomObjectContext.Offices").Count(), 0, "There should be no offices left");
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void EFFK_1To1_CascadeDelete_PrincipalToDependent()
        {
            // Create new office type
            EFFKClient.Office o = new EFFKClient.Office() { ID = 1, BuildingName = "Building 35", FloorNumber = 2, OfficeNumber = 2173 };
            ctx.AddObject("CustomObjectContext.Offices", o);

            // create new employee type
            EFFKClient.Worker w = new EFFKClient.Worker() { ID = 1, FirstName = "Pratik", LastName = "Patel" };
            ctx.AddObject("CustomObjectContext.Workers", w);
            ctx.SaveChanges();

            // setting the link from principal to dependent should delete the dependent
            ctx.SetLink(o, "Worker", null);
            ctx.SaveChanges();
            Assert.AreEqual(ctx.CreateQuery<EFFKClient.Worker>("CustomObjectContext.Workers").Count(), 0, "There should be no workers left");

            ctx.DeleteObject(o);
            ctx.SaveChanges();

            Assert.AreEqual(ctx.CreateQuery<EFFKClient.Office>("CustomObjectContext.Offices").Count(), 0, "There should be no offices left");
            Assert.AreEqual(ctx.CreateQuery<EFFKClient.Worker>("CustomObjectContext.Workers").Count(), 0, "There should be no workers left");
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void EFFK_1To1_CascadeDelete_DependentToPrincipal()
        {
            // Create new office type
            EFFKClient.Office o = new EFFKClient.Office() { ID = 1, BuildingName = "Building 35", FloorNumber = 2, OfficeNumber = 2173 };
            ctx.AddObject("CustomObjectContext.Offices", o);

            // create new employee type
            EFFKClient.Worker w = new EFFKClient.Worker() { ID = 1, FirstName = "Pratik", LastName = "Patel" };
            ctx.AddObject("CustomObjectContext.Workers", w);
            ctx.SaveChanges();

            // setting the link from dependent to principal should delete the dependent
            ctx.SetLink(w, "Office", null);
            ctx.SaveChanges();
            Assert.AreEqual(ctx.CreateQuery<EFFKClient.Worker>("CustomObjectContext.Workers").Count(), 0, "There should be no workers left");

            ctx.DeleteObject(o);
            ctx.SaveChanges();

            Assert.AreEqual(ctx.CreateQuery<EFFKClient.Office>("CustomObjectContext.Offices").Count(), 0, "There should be no offices left");
            Assert.AreEqual(ctx.CreateQuery<EFFKClient.Worker>("CustomObjectContext.Workers").Count(), 0, "There should be no workers left");
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void EFFK_1To1_DeleteDependent()
        {
            // Create new office type
            EFFKClient.Office o = new EFFKClient.Office() { ID = 1, BuildingName = "Building 35", FloorNumber = 2, OfficeNumber = 2173 };
            ctx.AddObject("CustomObjectContext.Offices", o);

            // create new employee type
            EFFKClient.Worker w = new EFFKClient.Worker() { ID = 1, FirstName = "Pratik", LastName = "Patel" };
            ctx.AddObject("CustomObjectContext.Workers", w);
            ctx.SaveChanges();

            // Deleting the dependent should work
            ctx.DeleteObject(w);
            ctx.SaveChanges();

            Assert.AreEqual(ctx.CreateQuery<EFFKClient.Office>("CustomObjectContext.Offices").Count(), 1, "There should be exactly one office instance");
            Assert.AreEqual(ctx.CreateQuery<EFFKClient.Worker>("CustomObjectContext.Workers").Count(), 0, "There should be no workers left");

            // clean the context
            ctx.DeleteObject(o);
            ctx.SaveChanges();

            Assert.AreEqual(ctx.CreateQuery<EFFKClient.Office>("CustomObjectContext.Offices").Count(), 0, "There should be no offices left");
            Assert.AreEqual(ctx.CreateQuery<EFFKClient.Worker>("CustomObjectContext.Workers").Count(), 0, "There should be no workers left");
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void EFFK_1To1_BasicInsertAndBind_Batch()
        {
            // Create new office type
            EFFKClient.Office o = new EFFKClient.Office() { ID = 1, BuildingName = "Building 35", FloorNumber = 2, OfficeNumber = 2173 };
            ctx.AddObject("CustomObjectContext.Offices", o);

            // create new employee type
            EFFKClient.Worker e = new EFFKClient.Worker() { ID = 1, FirstName = "Pratik", LastName = "Patel" };
            ctx.AddObject("CustomObjectContext.Workers", e);

            // Establish relationship between employee and office
            ctx.SetLink(o, "Worker", e);
            ctx.SetLink(e, "Office", o);
            ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);

            // clean the context
            ctx.DeleteObject(e);
            ctx.DeleteObject(o);
            ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void EFFK_1To1_BasicInsertAndBind_Batch_ChangedUriCompositionRulesOnServer()
        {
            // Fix URI composition in Astoria for V3 payloads
            ctx = new DataServiceContext(web.ServiceRoot, Microsoft.OData.Client.ODataProtocolVersion.V4);
            //ctx.EnableAtom = true;
            //ctx.Format.UseAtom();
            // Create new office type
            EFFKClient.Office o = new EFFKClient.Office() { ID = 1, BuildingName = "Building 35", FloorNumber = 2, OfficeNumber = 2173 };
            ctx.AddObject("CustomObjectContext.Offices", o);

            // create new employee type
            EFFKClient.Worker e = new EFFKClient.Worker() { ID = 1, FirstName = "Pratik", LastName = "Patel" };
            ctx.AddObject("CustomObjectContext.Workers", e);

            // Establish relationship between employee and office
            ctx.SetLink(o, "Worker", e);
            ctx.SetLink(e, "Office", o);
            ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);

            // clean the context
            ctx.DeleteObject(e);
            ctx.DeleteObject(o);
            ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void EFFK_1To1_UpdateRelationship()
        {
            // Create new office type
            EFFKClient.Office o = new EFFKClient.Office() { ID = 1, BuildingName = "Building 35", FloorNumber = 2, OfficeNumber = 2173 };
            ctx.AddObject("CustomObjectContext.Offices", o);

            // create new employee type
            EFFKClient.Worker w = new EFFKClient.Worker() { ID = 1, FirstName = "Pratik", LastName = "Patel" };
            ctx.AddObject("CustomObjectContext.Workers", w);
            ctx.SaveChanges();

            EFFKClient.Office o1 = new EFFKClient.Office() { ID = 2, BuildingName = "Building 35", FloorNumber = 2, OfficeNumber = 2174 };
            ctx.AddObject("CustomObjectContext.Offices", o1);
            ctx.SaveChanges();

            try
            {
                ctx.SetLink(o1, "Worker", w);
                ctx.SaveChanges();
            }
            catch (DataServiceRequestException ex)
            {
                Assert.AreEqual(ex.Response.First().StatusCode, 400, "Expecting bad request");
                Assert.IsTrue(ex.Response.First().Error.Message.Contains("A referential integrity constraint violation occurred"), "Making sure appropriate EF exception is thrown");
                ctx.DetachLink(o1, "Worker", w); // To clear this operation from the context so that next test doesn't hit the same issue
            }

            try
            {
                ctx.SetLink(w, "Office", o1);
                ctx.SaveChanges();
            }
            catch (DataServiceRequestException ex1)
            {
                Assert.AreEqual(ex1.Response.First().StatusCode, 400, "Expecting bad request");
                Assert.IsTrue(ex1.Response.First().Error.Message.Contains("The principal object must be tracked and not marked for deletion."), "Making sure appropriate EF exception is thrown");
                ctx.DetachLink(w, "Office", o1); // To clear this operation from the context so that next test doesn't hit the same issue
            }

            Assert.AreEqual(ctx.CreateQuery<EFFKClient.Office>("CustomObjectContext.Offices").Count(), 2, "Unexpected number of office instances encountered");
            Assert.AreEqual(ctx.CreateQuery<EFFKClient.Worker>("CustomObjectContext.Workers").Count(), 1, "Unexpected number of worker instances encountered");

            ctx.DeleteObject(o);
            ctx.DeleteObject(o1);
            ctx.DeleteObject(w);
            ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
            Assert.AreEqual(ctx.CreateQuery<EFFKClient.Office>("CustomObjectContext.Offices").Count(), 0, "There should be no offices left");
            Assert.AreEqual(ctx.CreateQuery<EFFKClient.Worker>("CustomObjectContext.Workers").Count(), 0, "There should be no workers left");
        }

        private void VerifyServerOrderId(int orderId, int? expectedCustomerId, String message = null)
        {
            var _ctx = new DataServiceContext(web.ServiceRoot);
            //_ctx.EnableAtom = true;
            //_ctx.Format.UseAtom();

            var order = _ctx.CreateQuery<EFFKClient.Order>("CustomObjectContext.Orders").Expand("Customers").Where(o => o.ID == orderId).FirstOrDefault();

            if (message == null)
            {
                message = String.Format("Order {0} expecting Customer {1}", orderId, expectedCustomerId);
            }

            Assert.IsNotNull(order, message);
            if (expectedCustomerId.HasValue)
            {
                Assert.AreEqual(expectedCustomerId, order.CustomerId, message);
                Assert.AreEqual(expectedCustomerId, order.Customers.ID, message);
            }
            else
            {
                Assert.IsNull(order.Customers);
            }
        }
    }

}