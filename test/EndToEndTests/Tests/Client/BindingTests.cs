﻿//---------------------------------------------------------------------
// <copyright file="BindingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using Microsoft.OData.Client;
    using System.Linq;
    using System.Net;
    using System;
    
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReference;
#if WIN8 || WINDOWSPHONE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif
#if !PORTABLELIB && !SILVERLIGHT
    using Microsoft.Test.OData.Framework.Verification;

#endif

    [TestClass]
    public class BindingTests : EndToEndTestBase
    {
        public BindingTests()
            : base(ServiceDescriptors.AstoriaDefaultService)
        {
        }

        #region Collections
        [TestMethod, Asynchronous]
        public void LoadPropertyCollection()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            var querry = (from c in context.Customer
                          where c.CustomerId == -10
                          select c) as DataServiceQuery<Customer>;

            var ar = querry.BeginExecute(null, null).EnqueueWait(this);
            var cus = querry.EndExecute(ar).First();
            DataServiceCollection<Customer> DSC = new DataServiceCollection<Customer>(context);
            DSC.Add(cus);
            var ar02 = context.BeginLoadProperty(cus, "Orders", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar02);
            foreach (Order o in cus.Orders)
            {
                o.OrderId = (134);
            }

            int fristCount = context.Entities.Count();
            foreach (var ed in context.Entities)
            {
                if (ed.Entity.GetType().Equals(cus.Orders.First().GetType()))
                {
                    Assert.AreEqual(EntityStates.Modified, ed.State);
                }
            }

            var o1 = new Order { OrderId = 1220 };
            cus.Orders.Add(o1);
            Assert.AreEqual(fristCount + 1, context.Entities.Count());
            cus.Orders.Remove(o1);
            Assert.AreEqual(fristCount, context.Entities.Count());

            this.EnqueueTestComplete();
        }

        #if !PORTABLELIB && !SILVERLIGHT
        [TestMethod]
        public void LoadCollectionExceptionShouldNotRuinEntityTracking()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            int[] customerIds = { /*existing*/-9, /*non-existing*/0, /*existing*/-10 };
            foreach (var customerId in customerIds)
            {
                Customer customer = null;

                try
                {
                    var query = context.Customer.Where(c => c.CustomerId == customerId);
                    var collection = new DataServiceCollection<Customer>(query);
                    customer = collection.Single();
                }
                catch (DataServiceQueryException e)
                {
                    var inner = e.InnerException as DataServiceClientException;
                    if (inner != null && inner.StatusCode == (int)HttpStatusCode.NotFound)
                    {
                        continue;
                    }

                    throw;
                }

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void CanContinueLoadEntityAfterLoadCollectionException()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> collection = null;
            int[] customerIds = { /*non-existing*/0, /*existing*/-10, /*existing*/-9 };
            foreach (var customerId in customerIds)
            {
                Customer customer = null;

                try
                {
                    var query = context.Customer.Where(c => c.CustomerId == customerId);

                    if (collection == null)
                    {
                        collection = new DataServiceCollection<Customer>(query);
                    }
                    else
                    {
                        collection.Load(query);
                    }

                    customer = collection.Single();
                }
                catch (DataServiceQueryException e)
                {
                    var inner = e.InnerException as DataServiceClientException;
                    if (inner != null && inner.StatusCode == (int)HttpStatusCode.NotFound)
                    {
                        continue;
                    }

                    throw;
                }

                // TODO: why DeleteObject won't trigger the callback in the observer to remove the entity in DataServiceCollection.
                // context.DeleteObject(customer);

                collection.Remove(customer);
                context.SaveChanges();
            }
        }
        #endif
        #endregion

        #region Remove tests
        //Remove added
        [TestMethod, Asynchronous]
        public void AddDeleteEntitySave()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> DSC = new DataServiceCollection<Customer>(context);
            Customer c = new Customer { CustomerId = 1002 };
            DSC.Add(c);
            DSC.Remove(c);
            Assert.IsTrue(VerifyCtxCount(context, 0, 0));
            this.SaveChanges(context);
            Assert.IsTrue(VerifyCtxCount(context, 0, 0));

            this.EnqueueTestComplete();
        }

        //Remove null
        [TestMethod, Asynchronous]
        public void AddRemoveNullEntity()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> DSC = new DataServiceCollection<Customer>(context);
            try
            {
                DSC.Add(null);
                Assert.Fail("Expected error not thrown");
            }
            catch (InvalidOperationException e)
            {
#if !PORTABLELIB && !SILVERLIGHT
                 StringResourceUtil.VerifyDataServicesClientString(e.Message, "DataBinding_BindingOperation_ArrayItemNull", "Add" );
#else
                Assert.IsNotNull(e);
#endif

            }

            try
            {
                DSC.Remove(null);
                Assert.Fail("Expected error not thrown");
            }
            catch (InvalidOperationException e)
            {
#if !PORTABLELIB && !SILVERLIGHT
                StringResourceUtil.VerifyDataServicesClientString(e.Message, "DataBinding_BindingOperation_ArrayItemNull", "Remove");
#else
                Assert.IsNotNull(e);
#endif
                
                }

            this.EnqueueTestComplete();
        }

        //Remove same entity twice
        [TestMethod, Asynchronous]
        public void RemoveEntityTwice()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> DSC = new DataServiceCollection<Customer>(context);
            Customer c = new Customer { CustomerId = 1002 };
            Customer c2 = new Customer { CustomerId = 1003 };
            DSC.Add(c);
            DSC.Add(c2);
            DSC.Remove(c);
            this.CheckState(context, EntityStates.Deleted, c);
            Assert.IsTrue(VerifyCtxCount(context, 1, 0));
            DSC.Remove(c);
            this.CheckState(context, EntityStates.Deleted, c);
            Assert.IsTrue(VerifyCtxCount(context, 1, 0));

            this.EnqueueTestComplete();
        }

        //Remove entity with links
        [TestMethod, Asynchronous]
        public void RemoveParentEntityWithLinks()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> DSC = new DataServiceCollection<Customer>(context);
            Customer c = new Customer { CustomerId = 1002 };
            DSC.Add(c);
            Order o = new Order { OrderId = 2001, Customer = c, CustomerId = 1002 };
            c.Orders.Add(o);
            VerifyCtxCount(context, 2, 1);
            DSC.Remove(c);
            VerifyCtxCount(context, 0, 0);
            SaveChanges(context);
            VerifyCtxCount(context, 0, 0);

            this.EnqueueTestComplete();
        }

        [TestMethod, Asynchronous]
        public void RemoveChildEntityWithLinks()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> DSC = new DataServiceCollection<Customer>(context);
            DataServiceCollection<Order> DSCorder = new DataServiceCollection<Order>(context);
            Customer c = new Customer { CustomerId = 1002 };
            DSC.Add(c);
            Order o = new Order { OrderId = 2001, Customer = c, CustomerId = 1002 };
            c.Orders.Add(o);
            VerifyCtxCount(context, 2, 1);
            DSCorder.Remove(o);
            VerifyCtxCount(context, 1, 0);
            SaveChanges(context);
            VerifyCtxCount(context, 1, 0);

            this.EnqueueTestComplete();
        }

        //Clear list  
        [TestMethod, Asynchronous]
        public void ClearListTest()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> DSC = new DataServiceCollection<Customer>(context);
            Customer c = new Customer { CustomerId = 1002 };
            Customer c2 = new Customer { CustomerId = 1003 };
            Customer c3 = new Customer { CustomerId = 1004 };
            DSC.Add(c);
            DSC.Add(c2);
            DSC.Add(c3);
            Order o = new Order { OrderId = 2001, Customer = c, CustomerId = 1002 };
            c.Orders.Add(o);
            VerifyCtxCount(context, 4, 1);
            DSC.Clear();
            VerifyCtxCount(context, 0, 0);

            this.EnqueueTestComplete();
        }

        #region Remove entity in modified state, detached state, deleted state, unchanged state
        [TestMethod, Asynchronous]
        public void DeletingInModifiedState()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> DSC = new DataServiceCollection<Customer>(context);
            Customer modified = new Customer { CustomerId = 1002 };
            DSC.Add(modified);
            VerifyCtxCount(context, 4, 0);
            this.SaveChanges(context);
            modified.CustomerId = 100002;
            this.CheckState(context, EntityStates.Modified, modified);
            VerifyCtxCount(context, 1, 0);
            DSC.Remove(modified);
            VerifyCtxCount(context, 0, 0);

            this.EnqueueTestComplete();
        }

        [TestMethod, Asynchronous]
        public void DeletingInDetachedState()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> DSC = new DataServiceCollection<Customer>(context);
            Customer detached = new Customer { CustomerId = 1003 };
            DSC.Add(detached);
            VerifyCtxCount(context, 1, 0);
            this.SaveChanges(context);
            context.Detach(detached);
            this.CheckState(context, EntityStates.Detached, detached);
            VerifyCtxCount(context, 1, 0);
            DSC.Remove(detached);
            VerifyCtxCount(context, 0, 0);

            this.EnqueueTestComplete();
        }

        [TestMethod, Asynchronous]
        public void DeletingInDeletedState()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> DSC = new DataServiceCollection<Customer>(context);
            Customer deleted = new Customer { CustomerId = 1004 };
            DSC.Add(deleted);
            VerifyCtxCount(context, 1, 0);
            this.SaveChanges(context);
            context.DeleteObject(deleted);
            this.CheckState(context, EntityStates.Deleted, deleted);
            VerifyCtxCount(context, 1, 0);
            DSC.Remove(deleted);
            VerifyCtxCount(context, 0, 0);

            this.EnqueueTestComplete();
        }

        [TestMethod, Asynchronous]
        public void DeletingInUnchangedState()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> DSC = new DataServiceCollection<Customer>(context);
            Customer unchanged = new Customer { CustomerId = 1002 };
            DSC.Add(unchanged);
            VerifyCtxCount(context, 1, 0);
            this.SaveChanges(context);
            this.CheckState(context, EntityStates.Unchanged, unchanged);
            VerifyCtxCount(context, 1, 0);
            DSC.Remove(unchanged);
            VerifyCtxCount(context, 0, 0);

            this.EnqueueTestComplete();
        }
        #endregion


        #endregion

        #region Check States

        [TestMethod, Asynchronous]
        public void AddSaveRemoveSaveEntity()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> DSC = new DataServiceCollection<Customer>(context);
            Customer c = new Customer { CustomerId = 1002 };
            DSC.Add(c);
            this.CheckState(context, EntityStates.Added, c);
            Assert.IsTrue(VerifyCtxCount(context, 1, 0));
            this.SaveChanges(context);
            Assert.IsTrue(VerifyCtxCount(context, 1, 0));
            DSC.Remove(c);
            this.CheckState(context, EntityStates.Deleted, c);
            Assert.IsTrue(VerifyCtxCount(context, 1, 0));
            this.SaveChanges(context);
            Assert.IsTrue(VerifyCtxCount(context, 0, 0));

            this.EnqueueTestComplete();
        }

        [TestMethod, Asynchronous]
        public void AddSaveUpdateSaveEntity()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> DSC = new DataServiceCollection<Customer>(context);
            Customer c = new Customer { CustomerId = 1002 };
            DSC.Add(c);
            this.CheckState(context, EntityStates.Added, c);
            Assert.IsTrue(VerifyCtxCount(context, 1, 0));
            this.SaveChanges(context);
            Assert.IsTrue(VerifyCtxCount(context, 1, 0));
            c.CustomerId = 1003;
            this.CheckState(context, EntityStates.Modified, c);
            Assert.IsTrue(VerifyCtxCount(context, 1, 0));
            this.SaveChanges(context);
            Assert.IsTrue(VerifyCtxCount(context, 1, 0));

            this.EnqueueTestComplete();
        }

        [TestMethod, Asynchronous]
        public void AddSaveUnchangedSaveEntity()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> DSC = new DataServiceCollection<Customer>(context);
            Customer c = new Customer { CustomerId = 1002 };
            DSC.Add(c);
            this.CheckState(context, EntityStates.Added, c);
            Assert.IsTrue(VerifyCtxCount(context, 1, 0));
            this.SaveChanges(context);
            Assert.IsTrue(VerifyCtxCount(context, 1, 0));

            // Unchanged c
            this.CheckState(context, EntityStates.Unchanged, c);
            Assert.IsTrue(VerifyCtxCount(context, 1, 0));
            this.SaveChanges(context);
            Assert.IsTrue(VerifyCtxCount(context, 1, 0));

            this.EnqueueTestComplete();
        }

        [TestMethod, Asynchronous]
        public void AddSaveDetachedSaveEntity()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> DSC = new DataServiceCollection<Customer>(context);
            Customer c = new Customer { CustomerId = 1002 };
            DSC.Add(c);
            this.CheckState(context, EntityStates.Added, c);
            Assert.IsTrue(VerifyCtxCount(context, 1, 0));
            this.SaveChanges(context);
            Assert.IsTrue(VerifyCtxCount(context, 1, 0));
            context.Detach(c);
            this.CheckState(context, EntityStates.Detached, DSC);
            Assert.IsTrue(VerifyCtxCount(context, 0, 0));
            this.SaveChanges(context);
            Assert.IsTrue(VerifyCtxCount(context, 0, 0));

            this.EnqueueTestComplete();
        }
        #endregion

        #region helpers
        internal bool VerifyCtxCount(DataServiceContext ctx, int expectedEntities, int expectedLinks)
        {
            return (ctx.Entities.Count() == expectedEntities && ctx.Links.Count == expectedLinks);
        }

        internal void CheckState(DataServiceContext ctx, EntityStates state,Object o)
        {
            var type = o.GetType();
             foreach (var ed in ctx.Entities)
            {
                if (ed.Entity.GetType().Equals(type) && (o).Equals(ed.Entity))
                {
                    Assert.AreEqual(state, ed.State);
                }
            }
        }

        internal void SaveChanges(DataServiceContext ctx)
        {
                 var ar = ctx.BeginSaveChanges(null, null).EnqueueWait(this);
                ctx.EndSaveChanges(ar);
        }
        #endregion
    }
}
