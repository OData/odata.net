//---------------------------------------------------------------------
// <copyright file="BindingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.OData.Client;
using Microsoft.OData.Profile111.Tests.AsynchronousTests;
using Microsoft.Test.OData.Services.TestServices;
using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReference;
using Xunit;

namespace Microsoft.OData.Profile111.Tests
{
    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    public class BindingTests : EndToEndTestBase
    {
        public BindingTests()
            : base(ServiceDescriptors.AstoriaDefaultService)
        {
        }

        #region Collections

        [Fact]
        public void LoadPropertyCollection()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            var querry = (from c in context.Customer
                          where c.CustomerId == -10
                          select c) as DataServiceQuery<Customer>;

            var ar = querry.BeginExecute(null, null).EnqueueWait(this);
            var cus = querry.EndExecute(ar).First();
            DataServiceCollection<Customer> dsc = new DataServiceCollection<Customer>(context) {cus};
            var ar02 = context.BeginLoadProperty(cus, "Orders", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar02);
            foreach (Order o in cus.Orders)
            {
                o.OrderId = (134);
            }

            int fristCount = context.Entities.Count;
            foreach (var ed in context.Entities)
            {
                if (ed.Entity.GetType() == cus.Orders.First().GetType())
                {
                    Assert.Equal(EntityStates.Modified, ed.State);
                }
            }

            var o1 = new Order { OrderId = 1220 };
            cus.Orders.Add(o1);
            Assert.Equal(fristCount + 1, context.Entities.Count);
            cus.Orders.Remove(o1);
            Assert.Equal(fristCount, context.Entities.Count);

            this.EnqueueTestComplete();
        }

        #endregion

        #region Remove tests

        //Remove added
        [Fact]
        public void AddDeleteEntitySave()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> dsc = new DataServiceCollection<Customer>(context);
            Customer c = new Customer { CustomerId = 1002 };
            dsc.Add(c);
            dsc.Remove(c);
            Assert.True(VerifyCtxCount(context, 0, 0));
            this.SaveChanges(context);
            Assert.True(VerifyCtxCount(context, 0, 0));

            this.EnqueueTestComplete();
        }

        //Remove null
        [Fact]
        public void AddRemoveNullEntity()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> dsc = new DataServiceCollection<Customer>(context);
            try
            {
                dsc.Add(null);
                Assert.True(false, "Expected error not thrown");
            }
            catch (InvalidOperationException e)
            {
                Assert.NotNull(e);
            }

            try
            {
                dsc.Remove(null);
                Assert.True(false, "Expected error not thrown");
            }
            catch (InvalidOperationException e)
            {
                Assert.NotNull(e);
            }

            this.EnqueueTestComplete();
        }

        //Remove same entity twice
        [Fact]
        public void RemoveEntityTwice()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> dsc = new DataServiceCollection<Customer>(context);
            Customer c = new Customer { CustomerId = 1002 };
            Customer c2 = new Customer { CustomerId = 1003 };
            dsc.Add(c);
            dsc.Add(c2);
            dsc.Remove(c);
            this.CheckState(context, EntityStates.Deleted, c);
            Assert.True(VerifyCtxCount(context, 1, 0));
            dsc.Remove(c);
            this.CheckState(context, EntityStates.Deleted, c);
            Assert.True(VerifyCtxCount(context, 1, 0));

            this.EnqueueTestComplete();
        }

        //Remove entity with links
        [Fact]
        public void RemoveParentEntityWithLinks()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> dsc = new DataServiceCollection<Customer>(context);
            Customer c = new Customer { CustomerId = 1002 };
            dsc.Add(c);
            Order o = new Order { OrderId = 2001, Customer = c, CustomerId = 1002 };
            c.Orders.Add(o);
            VerifyCtxCount(context, 2, 1);
            dsc.Remove(c);
            VerifyCtxCount(context, 0, 0);
            SaveChanges(context);
            VerifyCtxCount(context, 0, 0);

            this.EnqueueTestComplete();
        }

        [Fact]
        public void RemoveChildEntityWithLinks()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> dsc = new DataServiceCollection<Customer>(context);
            DataServiceCollection<Order> dscOrder = new DataServiceCollection<Order>(context);
            Customer c = new Customer { CustomerId = 1002 };
            dsc.Add(c);
            Order o = new Order { OrderId = 2001, Customer = c, CustomerId = 1002 };
            c.Orders.Add(o);
            VerifyCtxCount(context, 2, 1);
            dscOrder.Remove(o);
            VerifyCtxCount(context, 1, 0);
            SaveChanges(context);
            VerifyCtxCount(context, 1, 0);

            this.EnqueueTestComplete();
        }

        //Clear list  
        [Fact]
        public void ClearListTest()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> dsc = new DataServiceCollection<Customer>(context);
            Customer c = new Customer { CustomerId = 1002 };
            Customer c2 = new Customer { CustomerId = 1003 };
            Customer c3 = new Customer { CustomerId = 1004 };
            dsc.Add(c);
            dsc.Add(c2);
            dsc.Add(c3);
            Order o = new Order { OrderId = 2001, Customer = c, CustomerId = 1002 };
            c.Orders.Add(o);
            VerifyCtxCount(context, 4, 1);
            dsc.Clear();
            VerifyCtxCount(context, 0, 0);

            this.EnqueueTestComplete();
        }

        #region Remove entity in modified state, detached state, deleted state, unchanged state
        
        [Fact]
        public void DeletingInModifiedState()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> dsc = new DataServiceCollection<Customer>(context);
            Customer modified = new Customer { CustomerId = 1002 };
            dsc.Add(modified);
            VerifyCtxCount(context, 4, 0);
            this.SaveChanges(context);
            modified.CustomerId = 100002;
            this.CheckState(context, EntityStates.Modified, modified);
            VerifyCtxCount(context, 1, 0);
            dsc.Remove(modified);
            VerifyCtxCount(context, 0, 0);

            this.EnqueueTestComplete();
        }

        [Fact]
        public void DeletingInDetachedState()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> dsc = new DataServiceCollection<Customer>(context);
            Customer detached = new Customer { CustomerId = 1003 };
            dsc.Add(detached);
            VerifyCtxCount(context, 1, 0);
            this.SaveChanges(context);
            context.Detach(detached);
            this.CheckState(context, EntityStates.Detached, detached);
            VerifyCtxCount(context, 1, 0);
            dsc.Remove(detached);
            VerifyCtxCount(context, 0, 0);

            this.EnqueueTestComplete();
        }

        [Fact]
        public void DeletingInDeletedState()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> dsc = new DataServiceCollection<Customer>(context);
            Customer deleted = new Customer { CustomerId = 1004 };
            dsc.Add(deleted);
            VerifyCtxCount(context, 1, 0);
            this.SaveChanges(context);
            context.DeleteObject(deleted);
            this.CheckState(context, EntityStates.Deleted, deleted);
            VerifyCtxCount(context, 1, 0);
            dsc.Remove(deleted);
            VerifyCtxCount(context, 0, 0);

            this.EnqueueTestComplete();
        }

        [Fact]
        public void DeletingInUnchangedState()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> dsc = new DataServiceCollection<Customer>(context);
            Customer unchanged = new Customer { CustomerId = 1002 };
            dsc.Add(unchanged);
            VerifyCtxCount(context, 1, 0);
            this.SaveChanges(context);
            this.CheckState(context, EntityStates.Unchanged, unchanged);
            VerifyCtxCount(context, 1, 0);
            dsc.Remove(unchanged);
            VerifyCtxCount(context, 0, 0);

            this.EnqueueTestComplete();
        }
        #endregion

        #endregion

        #region Check States

        [Fact]
        public void AddSaveRemoveSaveEntity()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> dsc = new DataServiceCollection<Customer>(context);
            Customer c = new Customer { CustomerId = 1002 };
            dsc.Add(c);
            this.CheckState(context, EntityStates.Added, c);
            Assert.True(VerifyCtxCount(context, 1, 0));
            this.SaveChanges(context);
            Assert.True(VerifyCtxCount(context, 1, 0));
            dsc.Remove(c);
            this.CheckState(context, EntityStates.Deleted, c);
            Assert.True(VerifyCtxCount(context, 1, 0));
            this.SaveChanges(context);
            Assert.True(VerifyCtxCount(context, 0, 0));

            this.EnqueueTestComplete();
        }

        [Fact]
        public void AddSaveUpdateSaveEntity()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> dsc = new DataServiceCollection<Customer>(context);
            Customer c = new Customer { CustomerId = 1002 };
            dsc.Add(c);
            this.CheckState(context, EntityStates.Added, c);
            Assert.True(VerifyCtxCount(context, 1, 0));
            this.SaveChanges(context);
            Assert.True(VerifyCtxCount(context, 1, 0));
            c.CustomerId = 1003;
            this.CheckState(context, EntityStates.Modified, c);
            Assert.True(VerifyCtxCount(context, 1, 0));
            this.SaveChanges(context);
            Assert.True(VerifyCtxCount(context, 1, 0));

            this.EnqueueTestComplete();
        }

        [Fact]
        public void AddSaveUnchangedSaveEntity()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> dsc = new DataServiceCollection<Customer>(context);
            Customer c = new Customer { CustomerId = 1002 };
            dsc.Add(c);
            this.CheckState(context, EntityStates.Added, c);
            Assert.True(VerifyCtxCount(context, 1, 0));
            this.SaveChanges(context);
            Assert.True(VerifyCtxCount(context, 1, 0));

            // Unchanged c
            this.CheckState(context, EntityStates.Unchanged, c);
            Assert.True(VerifyCtxCount(context, 1, 0));
            this.SaveChanges(context);
            Assert.True(VerifyCtxCount(context, 1, 0));

            this.EnqueueTestComplete();
        }

        [Fact]
        public void AddSaveDetachedSaveEntity()
        {
            var ctxwrap = this.CreateWrappedContext<DefaultContainer>();
            var context = ctxwrap.Context;
            DataServiceCollection<Customer> dsc = new DataServiceCollection<Customer>(context);
            Customer c = new Customer { CustomerId = 1002 };
            dsc.Add(c);
            this.CheckState(context, EntityStates.Added, c);
            Assert.True(VerifyCtxCount(context, 1, 0));
            this.SaveChanges(context);
            Assert.True(VerifyCtxCount(context, 1, 0));
            context.Detach(c);
            this.CheckState(context, EntityStates.Detached, dsc);
            Assert.True(VerifyCtxCount(context, 0, 0));
            this.SaveChanges(context);
            Assert.True(VerifyCtxCount(context, 0, 0));

            this.EnqueueTestComplete();
        }
        #endregion

        #region helpers

        internal bool VerifyCtxCount(DataServiceContext ctx, int expectedEntities, int expectedLinks)
        {
            return (ctx.Entities.Count == expectedEntities && ctx.Links.Count == expectedLinks);
        }

        internal void CheckState(DataServiceContext ctx, EntityStates state,Object o)
        {
            var type = o.GetType();
             foreach (var ed in ctx.Entities)
            {
                if (ed.Entity.GetType() == type && (o).Equals(ed.Entity))
                {
                    Assert.Equal(state, ed.State);
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
