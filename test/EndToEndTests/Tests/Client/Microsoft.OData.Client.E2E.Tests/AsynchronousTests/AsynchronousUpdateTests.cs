//---------------------------------------------------------------------
// <copyright file="AsynchronousUpdateTests.cs" company="Microsoft">
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
using Employee = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Employee;
using Order = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Order;
using Person = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Person;
using AuditInfo = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.AuditInfo;
using ConcurrencyInfo = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.ConcurrencyInfo;

namespace Microsoft.OData.Client.E2E.Tests.AsynchronousTests;

public class AsynchronousUpdateTests : AsynchronousEndToEndTestBase<AsynchronousUpdateTests.TestsStartup>
{
    private readonly Uri _baseUri;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(AsynchronousUpdateTestsController), typeof(MetadataController));
            
            services.AddControllers().AddOData(opt =>
            {
                opt.EnableQueryFeatures().AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel(), new DefaultODataBatchHandler());
            });
        }
    }

    public AsynchronousUpdateTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");
    }

    /// <summary>
    /// Prefer header for include(notInclude)Content
    /// </summary>
    [Fact]
    public void PreferHeader_ShouldHandleIncludeAndNoContentCorrectly()
    {
        // Arrange
        var context = this.CreateWrappedContext();

        var c1 = new Customer { CustomerId = 1, Name = "testName" };

        context.AddAndUpdateResponsePreference = DataServiceResponsePreference.IncludeContent;
        c1.Name = "changedName";
        context.AddToCustomers(c1);

        var ar0 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        var returnedValue = context.EndSaveChanges(ar0).SingleOrDefault() as ChangeOperationResponse;
        Assert.Equal(201, returnedValue?.StatusCode);

        context.AddAndUpdateResponsePreference = DataServiceResponsePreference.NoContent;
        c1.Auditing = new AuditInfo
        {
            ModifiedBy = "Me",
            ModifiedDate = DateTimeOffset.Now,
            Concurrency = new ConcurrencyInfo
            {
                Token = Guid.NewGuid().ToString(),
                QueriedDateTime = DateTimeOffset.Now
            },
        };

        context.UpdateObject(c1);

        var ar1 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        returnedValue = context.EndSaveChanges(ar1).SingleOrDefault() as ChangeOperationResponse;
        Assert.Equal(204, returnedValue?.StatusCode);

        context.AddAndUpdateResponsePreference = DataServiceResponsePreference.IncludeContent;
        c1.Name = "changedName2";
        context.UpdateObject(c1);

        var ar2 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        returnedValue = context.EndSaveChanges(ar2).SingleOrDefault() as ChangeOperationResponse;
        Assert.Equal(200, returnedValue?.StatusCode);

        var response = context.Customers.Where(p => p.CustomerId == 1) as DataServiceQuery<Customer>;
        var ar3 = response.BeginExecute(null, null).EnqueueWait(this);
        var people = response.EndExecute(ar3);
        var person = people.SingleOrDefault();
        Assert.Equal("changedName2", person.Name);
        Assert.Equal("Me", person.Auditing.ModifiedBy);
        Assert.Equal(c1.Auditing.Concurrency.Token, person.Auditing.Concurrency.Token);
        Assert.Equal(c1.Auditing.Concurrency.QueriedDateTime, person.Auditing.Concurrency.QueriedDateTime);

        this.EnqueueTestComplete();
    }

    /// <summary>
    /// service operations tests only ONE operation being tested (getCustomerCount)
    /// </summary>
    [Fact]
    public void ServiceOperationTests_ShouldReturnCorrectCustomerCount()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        var ar200 = context.BeginExecute<int>(new Uri("GetCustomerCount/", UriKind.Relative), null, null, "GET").EnqueueWait(this);
        var count = context.EndExecute<int>(ar200).SingleOrDefault();

        Assert.Equal(10, count);

        this.EnqueueTestComplete();
    }

    /// <summary>
    /// Execute actions with no parameter  return and no return;
    /// </summary>
    [Fact]
    public void ActionTestsNoParams_ShouldExecuteActionsWithoutParameters()
    {
        var context = this.CreateWrappedContext();

        var e1 = new Employee { Name = "tim", Salary = 300, Title = "bill", PersonId = 1006 };
        context.AddToPeople(e1);
        var ar0 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar0);

        var ar2 = context.BeginExecute(new Uri("People(1006)/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee/Sack", UriKind.Relative), null, null, "POST").EnqueueWait(this);
        context.EndExecute(ar2);
        var ar21 = context.BeginLoadProperty(e1, "Title", null, null).EnqueueWait(this);
        context.EndLoadProperty(ar21);
        Assert.Equal("bill[Sacked]", e1.Title);

        this.EnqueueTestComplete();
    }

    /// <summary>
    /// Add update delete a  link
    /// </summary>
    [Fact]
    public void AddUpdateDeleteAssociationLinkSetLinkTest_ShouldManageLinksCorrectly()
    {
        var context = this.CreateWrappedContext();
        context.AddAndUpdateResponsePreference = DataServiceResponsePreference.None;
        context.IgnoreResourceNotFoundException = true;
        context.MergeOption = MergeOption.OverwriteChanges;
        var c1 = new Customer { CustomerId = 1004 };
        var c2 = new Customer { CustomerId = 1006 };
        var o1 = new Order { OrderId = 999 };
        context.AddToOrders(o1);
        context.AddToCustomers(c1);
        context.AddToCustomers(c2);

        var ar0 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar0);
        var ar01 = context.BeginLoadProperty(c1, "Orders", null, null).EnqueueWait(this);
        context.EndLoadProperty(ar01);
        var ar02 = context.BeginLoadProperty(o1, "Customer", null, null).EnqueueWait(this);
        context.EndLoadProperty(ar02);
        Assert.Empty(c1.Orders);
        Assert.Null(o1.Customer);

        context.AddLink(c1, "Orders", o1);
        var ar1 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar1);
        var ar11 = context.BeginLoadProperty(c1, "Orders", null, null).EnqueueWait(this);
        context.EndLoadProperty(ar11);
        var ar12 = context.BeginLoadProperty(o1, "Customer", null, null).EnqueueWait(this);
        context.EndLoadProperty(ar12);
        Assert.Single(c1.Orders);
        Assert.Null(o1.Customer);

        context.SetLink(o1, "Customer", c1);
        var ar2 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar2);
        var ar21 = context.BeginLoadProperty(c1, "Orders", null, null).EnqueueWait(this);
        context.EndLoadProperty(ar21);
        var ar22 = context.BeginLoadProperty(o1, "Customer", null, null).EnqueueWait(this);
        context.EndLoadProperty(ar22);
        Assert.Single(c1.Orders);
        Assert.Equal(c1, o1.Customer);

        context.SetLink(o1, "Customer", c2);
        var ar3 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar3);
        var ar31 = context.BeginLoadProperty(c1, "Orders", null, null).EnqueueWait(this);
        context.EndLoadProperty(ar31);
        var ar32 = context.BeginLoadProperty(o1, "Customer", null, null).EnqueueWait(this);
        context.EndLoadProperty(ar32);
        Assert.Single(c1.Orders);
        Assert.Equal(c2, o1.Customer);

        context.DeleteLink(c1, "Orders", o1);
        var ar4 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar4);
        var ar41 = context.BeginLoadProperty(c1, "Orders", null, null).EnqueueWait(this);
        context.EndLoadProperty(ar41);
        var ar42 = context.BeginLoadProperty(o1, "Customer", null, null).EnqueueWait(this);
        context.EndLoadProperty(ar42);
        Assert.Equal(c2, o1.Customer);
        Assert.Empty(c1.Orders);

        context.SetLink(o1, "Customer", null);
        var ar5 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar5);
        var ar51 = context.BeginLoadProperty(c1, "Orders", null, null).EnqueueWait(this);
        context.EndLoadProperty(ar51);
        var ar52 = context.BeginLoadProperty(o1, "Customer", null, null).EnqueueWait(this);
        context.EndLoadProperty(ar52);
        Assert.Null(o1.Customer);
        Assert.Empty(c1.Orders);

        this.EnqueueTestComplete();
    }

    /// <summary>
    /// Create, update, delete an entity
    /// </summary>
    [Fact]
    public void AddUpdateDeleteTest_ShouldPerformCRUDOperationsOnEntity()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        var peopleQuery = context.People;
        var personQuery = context.People.Where(p => p.PersonId == 1000) as DataServiceQuery<Person>;

        // Verify PersonId == 1000 not exists
        var ar1 = peopleQuery.BeginExecute(null, null).EnqueueWait(this);
        var people = peopleQuery.EndExecute(ar1);
        var person = people.Where(p => p.PersonId == 1000).SingleOrDefault();
        Assert.Null(person);

        // Add
        person = Person.CreatePerson(1000);
        person.Name = "Name1";
        context.AddToPeople(person);

        var ar2 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar2);
        context.Detach(person);

        // Verify add
        person = null;
        Assert.NotNull(personQuery);
        var ar3 = personQuery.BeginExecute(null, null).EnqueueWait(this);
        person = personQuery.EndExecute(ar3).Single();
        Assert.NotNull(person);

        // Update
        person.Name = "Name2";
        context.UpdateObject(person);
        var ar4 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar4);
        context.Detach(person);

        // Verify update
        person = null;
        var ar5 = personQuery.BeginExecute(null, null).EnqueueWait(this);
        person = personQuery.EndExecute(ar5).Single();
        Assert.Equal("Name2", person.Name);
        context.Detach(person);

        // Delete
        context.AttachTo("People", person);
        context.DeleteObject(person);
        var ar6 = context.BeginSaveChanges(null, null).EnqueueWait(this);
        context.EndSaveChanges(ar6);

        // Verify Delete
        context.Detach(person);
        var ar7 = peopleQuery.BeginExecute(null, null).EnqueueWait(this);
        people = peopleQuery.EndExecute(ar7);
        person = people.Where(p => p.PersonId == 1000).SingleOrDefault();
        Assert.Null(person);

        this.EnqueueTestComplete();
    }

    /// <summary>
    /// Add update delete an entity using Batch SaveChangesOptions
    /// </summary>
    [Fact]
    public void AddUpdateDeleteBatchTest_ShouldPerformBatchCRUDOperations()
    {
        var context = this.CreateWrappedContext();
        int numberOfPeople = 10;
        for (int i = 0; i < numberOfPeople; i++)
        {
            // Create
            context.AddToPeople(new Person { PersonId = 1000 + i });
        }

        var ar1 = context.BeginSaveChanges(SaveChangesOptions.BatchWithSingleChangeset, null, null).EnqueueWait(this);
        context.EndSaveChanges(ar1);
        var query = context.People.Where(p => p.PersonId >= 1000) as DataServiceQuery<Person>;
        var ar2 = query.BeginExecute(null, null).EnqueueWait(this);
        var people = query.EndExecute(ar2).ToList();
        Assert.Equal(numberOfPeople, people.Count());

        // Update
        foreach (var person in people)
        {
            person.Name = person.PersonId.ToString();
            context.UpdateObject(person);
        }

        var ar3 = context.BeginSaveChanges(SaveChangesOptions.BatchWithSingleChangeset, null, null).EnqueueWait(this);
        context.EndSaveChanges(ar3);

        foreach (var person in people)
        {
            context.Detach(person);
        }
        people = null;

        var ar4 = query.BeginExecute(null, null).EnqueueWait(this);
        people = query.EndExecute(ar4).ToList();
        foreach (var person in people)
        {
            Assert.Equal(person.PersonId.ToString(), person.Name);
        }

        // Delete
        foreach (var person in people)
        {
            context.DeleteObject(person);
        }

        var ar5 = context.BeginSaveChanges(SaveChangesOptions.BatchWithSingleChangeset, null, null).EnqueueWait(this);
        context.EndSaveChanges(ar5);

        var ar6 = query.BeginExecute(null, null).EnqueueWait(this);
        people = query.EndExecute(ar6).ToList();
        Assert.False(people.Any());

        this.EnqueueTestComplete();
    }

    #region Private methods

    private Container CreateWrappedContext()
    {
        var context = new Container(_baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        ResetDefaultDataSource(context);

        return context;
    }

    private void ResetDefaultDataSource(Container context)
    {
        var actionUri = new Uri(_baseUri + "asynchronousupdatetests/Default.ResetDefaultDataSource", UriKind.Absolute);
        context.Execute(actionUri, "POST");
    }

    #endregion
}
