//---------------------------------------------------------------------
// <copyright file="BindingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Asynchronous;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;
using Xunit;
using Customer = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Customer;
using Order = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Order;

namespace Microsoft.OData.Client.E2E.Tests.AsynchronousTests;

/// <summary>
/// The BindingTests class validates the behavior of the OData client when working with entity binding scenarios.
/// It tests operations such as loading properties, adding, removing, and saving entities, handling entity states (e.g., modified, detached, deleted, unchanged),
/// and managing relationships between parent and child entities.
/// </summary>
public class BindingTests : AsynchronousEndToEndTestBase<BindingTests.TestsStartup>
{
    private readonly Uri _baseUri;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(BindingTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
            {
                opt.EnableQueryFeatures().AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel(), new DefaultODataBatchHandler());
            });
        }
    }

    public BindingTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");
    }

    #region Collections
    [Fact]
    public void LoadProperty_Collection()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        
        var query = (from c in context.Customers
                      where c.CustomerId == -10
                      select c) as DataServiceQuery<Customer>;

        // Act
        var asyncResult = query.BeginExecute(null, null).EnqueueWait(this);
        var cus = query.EndExecute(asyncResult).First();

        var dataServiceCollection = new DataServiceCollection<Customer>(context);
        dataServiceCollection.Add(cus);

        var asyncResult2 = context.BeginLoadProperty(cus, "Orders", null, null).EnqueueWait(this);
        context.EndLoadProperty(asyncResult2);
        foreach (Order o in cus.Orders)
        {
            o.OrderId = 134;
        }

        // Assert
        Assert.Equal(3, context.Entities.Where(ed => ed.Entity.GetType().Equals(cus.Orders.First().GetType()) && ed.State == EntityStates.Modified).Count());

        var o1 = new Order { OrderId = 1220 };
        cus.Orders.Add(o1);

        // They are not added to the context yet
        Assert.Equal(5, context.Entities.Count);

        cus.Orders.Remove(o1);
        Assert.Equal(4, context.Entities.Count);

        this.EnqueueTestComplete();
    }
    #endregion

    #region Remove tests
    //Remove added
    [Fact]
    public void AddDeleteEntity_AndSave()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        
        var dataServiceCollection = new DataServiceCollection<Customer>(context, "Customers", null, null);
        var customer = new Customer { CustomerId = 1002 };

        // Act & Assert
        dataServiceCollection.Add(customer);
        dataServiceCollection.Remove(customer);
        
        Assert.Empty(context.Entities);
        Assert.Empty(this.GetLinks(context));

        var asyncResult = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(asyncResult);
        
        Assert.Empty(context.Entities);
        Assert.Empty(this.GetLinks(context));

        this.EnqueueTestComplete();
    }

    //Remove same entity twice
    [Fact]
    public void RemoveEntity_Twice()
    {
        // Arrange
        var context = this.CreateWrappedContext();

        var dataServiceCollection = new DataServiceCollection<Customer>(context, "Customers", null, null);
        var customer1 = new Customer { CustomerId = 1002 };
        var customer2 = new Customer { CustomerId = 1003 };
        dataServiceCollection.Add(customer1);
        dataServiceCollection.Add(customer2);

        // Act & Assert
        dataServiceCollection.Remove(customer1);

        // "customer1" is removed
        Assert.DoesNotContain(context.Entities, e => e.Entity.GetType().Equals(customer1.GetType()) && (customer1).Equals(e.Entity));

        Assert.Single(context.Entities);
        Assert.Empty(this.GetLinks(context));

        dataServiceCollection.Remove(customer1);

        // "customer1" is removed
        Assert.DoesNotContain(context.Entities, e => e.Entity.GetType().Equals(customer1.GetType()) && (customer1).Equals(e.Entity));

        this.GetLinks(context);
        Assert.Single(context.Entities);
        Assert.Empty(this.GetLinks(context));

        this.EnqueueTestComplete();
    }

    //Remove entity with links
    [Fact]
    public void RemoveParentEntity_WithLinks()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        
        var dataServiceCollection = new DataServiceCollection<Customer>(context, "Customers", null, null);
        var customer = new Customer { CustomerId = 1002 };
        dataServiceCollection.Add(customer);

        var order = new Order { OrderId = 2001, Customer = customer, CustomerId = 1002 };
        customer.Orders.Add(order);
        
        Assert.Equal(2, context.Entities.Count);
        Assert.Single(this.GetLinks(context));

        dataServiceCollection.Remove(customer);
        
        Assert.Empty(context.Entities);
        Assert.Empty(this.GetLinks(context));

        var ar = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar);
        
        Assert.Empty(context.Entities);
        Assert.Empty(this.GetLinks(context));

        this.EnqueueTestComplete();
    }

    [Fact]
    public void RemoveChildEntity_WithLinks()
    {
        // Arrange
        var context = this.CreateWrappedContext();

        var dataServiceCollection = new DataServiceCollection<Customer>(context, "Customers", null, null);
        var dataServiceCollectionOrder = new DataServiceCollection<Order>(context, "Orders", null, null);
        var customer = new Customer { CustomerId = 1002 };
        dataServiceCollection.Add(customer);

        var order = new Order { OrderId = 2001, Customer = customer, CustomerId = 1002 };

        // Act & Assert
        dataServiceCollectionOrder.Add(order);
        customer.Orders.Add(order);
        
        Assert.Equal(2, context.Entities.Count);
        Assert.Single(this.GetLinks(context));

        dataServiceCollectionOrder.Remove(order);
        
        Assert.Single(context.Entities);
        Assert.Empty(this.GetLinks(context));

        var ar = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar);
        
        Assert.Single(context.Entities);
        Assert.Empty(this.GetLinks(context));

        this.EnqueueTestComplete();
    }

    //Clear list  
    [Fact]
    public void ClearList_WithoutAffectingContext()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        
        var dataServiceCollection = new DataServiceCollection<Customer>(context, "Customers", null, null);
        var customer = new Customer { CustomerId = 1002 };
        var customer2 = new Customer { CustomerId = 1003 };
        var customer3 = new Customer { CustomerId = 1004 };
        dataServiceCollection.Add(customer);
        dataServiceCollection.Add(customer2);
        dataServiceCollection.Add(customer3);

        var order = new Order { OrderId = 2001, Customer = customer, CustomerId = 1002 };

        // Act & Assert
        customer.Orders.Add(order);
        
        Assert.Equal(4, context.Entities.Count);
        Assert.Single(this.GetLinks(context));

        dataServiceCollection.Clear();

        Assert.Equal(4, context.Entities.Count);
        Assert.Single(this.GetLinks(context));

        this.EnqueueTestComplete();
    }

    #region Remove entity in modified state, detached state, deleted state, unchanged state
    [Fact]
    public void DeleteEntity_InModifiedState()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        
        var dataServiceCollection = new DataServiceCollection<Customer>(context, "Customers", null, null);
        var modified = new Customer { CustomerId = 1002 };

        // Act & Assert
        dataServiceCollection.Add(modified);
        
        Assert.Single(context.Entities);
        Assert.Empty(this.GetLinks(context));

        var ar = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar);

        modified.CustomerId = 100002;
        // modified is in modified state
        Assert.Equal(EntityStates.Modified, context.Entities.First(e => e.Entity.GetType().Equals(modified.GetType()) && (modified).Equals(e.Entity)).State);
        
        Assert.Single(context.Entities);
        Assert.Empty(this.GetLinks(context));

        dataServiceCollection.Remove(modified);
        
        Assert.Empty(context.Entities.Where(e => e.State == EntityStates.Modified).ToList());
        Assert.Empty(this.GetLinks(context));

        this.EnqueueTestComplete();
    }

    [Fact]
    public void DeleteEntity_InDetachedState()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        
        var dataServiceCollection = new DataServiceCollection<Customer>(context, "Customers", null, null);
        Customer detached = new Customer { CustomerId = 1003 };

        // Act & Assert
        dataServiceCollection.Add(detached);
        
        Assert.Single(context.Entities);
        Assert.Empty(this.GetLinks(context));

        var ar = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar);

        context.Detach(detached);

        Assert.Empty(context.Entities);
        Assert.Empty(this.GetLinks(context));

        dataServiceCollection.Remove(detached);

        
        Assert.Empty(context.Entities);
        Assert.Empty(this.GetLinks(context));

        this.EnqueueTestComplete();
    }

    [Fact]
    public void DeleteEntity_InDeletedState()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        
        var dataServiceCollection = new DataServiceCollection<Customer>(context, "Customers", null, null);
        Customer deleted = new Customer { CustomerId = 1004 };

        // Act & Assert 
        dataServiceCollection.Add(deleted);
        
        Assert.Single(context.Entities);
        Assert.Empty(this.GetLinks(context));

        var ar = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar);

        context.DeleteObject(deleted);

        Assert.Single(context.Entities);
        Assert.Equal(EntityStates.Deleted, context.Entities.First(e => e.Entity.GetType().Equals(deleted.GetType()) && (deleted).Equals(e.Entity)).State);
        Assert.Empty(this.GetLinks(context));

        dataServiceCollection.Remove(deleted);
        
        Assert.Single(context.Entities);
        Assert.Empty(this.GetLinks(context));

        this.EnqueueTestComplete();
    }

    [Fact]
    public void DeleteEntity_InUnchangedState()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        
        var dataServiceCollection = new DataServiceCollection<Customer>(context, "Customers", null, null);
        Customer unchanged = new Customer { CustomerId = 1002 };

        // Act & Assert
        dataServiceCollection.Add(unchanged);
        
        Assert.Single(context.Entities);
        Assert.Empty(this.GetLinks(context));

        var ar = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar);

        Assert.Single(context.Entities);
        Assert.Equal(EntityStates.Unchanged, context.Entities.First(e => e.Entity.GetType().Equals(unchanged.GetType()) && (unchanged).Equals(e.Entity)).State);
        Assert.Empty(this.GetLinks(context));

        dataServiceCollection.Remove(unchanged);

        this.GetLinks(context);
        Assert.Empty(context.Entities.Where(e => e.State == EntityStates.Unchanged).ToList());
        Assert.Empty(this.GetLinks(context));

        this.EnqueueTestComplete();
    }
    #endregion


    #endregion

    #region Check States

    [Fact]
    public void AddSaveRemoveAndThenSave_Entity()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        
        var dataServiceCollection = new DataServiceCollection<Customer>(context, "Customers", null, null);
        Customer c = new Customer { CustomerId = 1002 };

        // Act & Assert
        dataServiceCollection.Add(c);

        Assert.Single(context.Entities);
        Assert.Equal(EntityStates.Added, context.Entities.First(e => e.Entity.GetType().Equals(c.GetType()) && (c).Equals(e.Entity)).State);
        Assert.Empty(this.GetLinks(context));

        // Save to add
        var ar = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar);
        
        Assert.Single(context.Entities);
        Assert.Empty(this.GetLinks(context));

        dataServiceCollection.Remove(c);

        Assert.Single(context.Entities);
        Assert.Equal(EntityStates.Deleted, context.Entities.First(e => e.Entity.GetType().Equals(c.GetType()) && (c).Equals(e.Entity)).State);
        Assert.Empty(this.GetLinks(context));

        // Save to delete
        ar = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar);
        
        Assert.Empty(context.Entities);
        Assert.Empty(this.GetLinks(context));

        this.EnqueueTestComplete();
    }

    [Fact]
    public void AddSaveUpdateAndThenSave_Entity()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        
        var dataServiceCollection = new DataServiceCollection<Customer>(context, "Customers", null, null);
        Customer c = new Customer { CustomerId = 1002 };

        // Act & Assert
        dataServiceCollection.Add(c);

        Assert.Single(context.Entities);
        Assert.Equal(EntityStates.Added, context.Entities.First(e => e.Entity.GetType().Equals(c.GetType()) && (c).Equals(e.Entity)).State);
        Assert.Empty(this.GetLinks(context));

        // Save to add
        var ar = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar);
        
        Assert.Single(context.Entities);
        Assert.Empty(this.GetLinks(context));

        c.CustomerId = 1003;

        Assert.Single(context.Entities);
        Assert.Equal(EntityStates.Modified, context.Entities.First(e => e.Entity.GetType().Equals(c.GetType()) && (c).Equals(e.Entity)).State);
        Assert.Empty(this.GetLinks(context));

        // Save to update
        ar = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar);
        
        Assert.Single(context.Entities);
        Assert.Empty(this.GetLinks(context));

        this.EnqueueTestComplete();
    }

    [Fact]
    public void AddSaveUnchangedAndThenSave_Entity()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        
        var dataServiceCollection = new DataServiceCollection<Customer>(context, "Customers", null, null);
        Customer c = new Customer { CustomerId = 1002 };

        // Act & Assert
        dataServiceCollection.Add(c);

        Assert.Single(context.Entities);
        Assert.Equal(EntityStates.Added, context.Entities.First(e => e.Entity.GetType().Equals(c.GetType()) && (c).Equals(e.Entity)).State);
        Assert.Empty(this.GetLinks(context));

        // Save to add
        var ar = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar);

        Assert.Single(context.Entities);
        Assert.Empty(this.GetLinks(context));
        Assert.Equal(EntityStates.Unchanged, context.Entities.First(e => e.Entity.GetType().Equals(c.GetType()) && (c).Equals(e.Entity)).State);

        // Save to update Un-changed
        ar = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar);
        
        Assert.Single(context.Entities);
        Assert.Empty(this.GetLinks(context));

        this.EnqueueTestComplete();
    }

    [Fact]
    public void AddSaveDetachedAndThenSave_Entity()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        
        var dataServiceCollection = new DataServiceCollection<Customer>(context, "Customers", null, null);
        Customer c = new Customer { CustomerId = 1002 };

        // Act & Assert
        dataServiceCollection.Add(c);

        Assert.Single(context.Entities);
        Assert.Equal(EntityStates.Added, context.Entities.First(e => e.Entity.GetType().Equals(c.GetType()) && (c).Equals(e.Entity)).State);
        Assert.Empty(this.GetLinks(context));

        // Save to add
        var ar = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar);
        
        Assert.Single(context.Entities);
        Assert.Empty(this.GetLinks(context));

        context.Detach(c);

        Assert.Empty(context.Entities);
        Assert.Empty(this.GetLinks(context));

        // Save to update Detached
        ar = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar);
        
        Assert.Empty(context.Entities);
        Assert.Empty(this.GetLinks(context));

        this.EnqueueTestComplete();
    }
    #endregion

    #region Private

    private Container CreateWrappedContext()
    {
        var context = new Container(_baseUri)
        {
            HttpClientFactory = HttpClientFactory,
            UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses
        };

        ResetDefaultDataSource(context);

        return context;
    }

    private IEnumerable<LinkDescriptor?> GetLinks(Container context)
    {
        return context.Links.Where(l =>  l.Source.GetType().GetProperty(l.SourceProperty).PropertyType.IsSubclassOf(typeof(BaseEntityType)));
    }

    private void ResetDefaultDataSource(Container context)
    {
        var actionUri = new Uri(_baseUri + "bindingtests/Default.ResetDefaultDataSource", UriKind.Absolute);
        context.Execute(actionUri, "POST");
    }

    #endregion
}
