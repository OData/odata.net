//---------------------------------------------------------------------
// <copyright file="AsynchronousUpdateTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.AsynchronousTests
{
    using System.Collections.ObjectModel;
    using Microsoft.OData.Client;
    using System.Linq;
    using System.IO;
    using System;
#if SILVERLIGHT
    using Microsoft.Silverlight.Testing;
#endif   
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReference;
#if WIN8 || WINDOWSPHONE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    /// <summary>
    /// Client update tests using asynchronous APIs
    /// </summary>
    [TestClass]
    public class AsynchronousUpdateTests : EndToEndTestBase
    {
        public AsynchronousUpdateTests()
            : base(ServiceDescriptors.AstoriaDefaultService)
        {
        }

        /// <summary>
        /// refer header for include(notInclude)Content
        /// </summary>
        [TestMethod, Asynchronous]
        public void PreferHeader()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            Customer c1 = new Customer {CustomerId = 1, Name = "testName"};
            
            context.AddAndUpdateResponsePreference = DataServiceResponsePreference.IncludeContent;
            c1.Name = "changedName";
            context.AddToCustomer(c1);
            var ar0 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            var returnedValue = context.EndSaveChanges(ar0).SingleOrDefault() as ChangeOperationResponse;
            Assert.AreEqual(201,returnedValue.StatusCode,  "StatusCode == 201");

            context.AddAndUpdateResponsePreference = DataServiceResponsePreference.NoContent;
            c1.CustomerId = 2;
            context.UpdateObject(c1);
            var ar1 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            returnedValue = context.EndSaveChanges(ar1).SingleOrDefault() as ChangeOperationResponse;
            Assert.AreEqual(204,returnedValue.StatusCode,  "StatusCode == 204");

            context.AddAndUpdateResponsePreference = DataServiceResponsePreference.IncludeContent;
            c1.Name = "changedName2";
            context.UpdateObject(c1);
            var ar2 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            returnedValue = context.EndSaveChanges(ar2).SingleOrDefault() as ChangeOperationResponse;
            Assert.AreEqual(200,returnedValue.StatusCode,  "StatusCode == 200");
           
            this.EnqueueTestComplete();
        }

        /// <summary>
        /// service operations tests only ONE operation being tested (getCustomerCount)
        /// </summary>
        [TestMethod, Asynchronous]
        public void ServiceOperationTests()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            var ar200 = context.BeginExecute<int>(new Uri("GetCustomerCount/", UriKind.Relative), null, null, "GET").EnqueueWait(this);
            var count = context.EndExecute<int>(ar200).SingleOrDefault();
            Assert.AreEqual(10, count);

            this.EnqueueTestComplete();
         }

        /// <summary>
        /// Execute actions with parameter ( Primitive, complex, collection, multiple ) Parms
        /// </summary>
        // github issuse: #896
        // [TestMethod, Asynchronous]
        public void ActionTestsParams()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            Employee e1 = new Employee {Salary = 300, Name = "bill", PersonId = 1005};
            Collection<string> specifications = new Collection<string> {"A", "B", "C"};
            DateTimeOffset purchaseTime = DateTimeOffset.Now;
            ComputerDetail cd1 = new ComputerDetail {ComputerDetailId = 101, SpecificationsBag = new ObservableCollection<string>()};
            Customer c1 = new Customer { Name = "nill", CustomerId = 1007, Auditing = new AuditInfo { ModifiedBy = "No-one", ModifiedDate = DateTimeOffset.Now, Concurrency = new ConcurrencyInfo { Token = "Test", QueriedDateTime = DateTimeOffset.MinValue } } };
            AuditInfo a1 = new AuditInfo { ModifiedBy = "some-one", ModifiedDate =  DateTimeOffset.MinValue,Concurrency = new ConcurrencyInfo { Token = "Test", QueriedDateTime = DateTimeOffset.MinValue} };
            context.AddToCustomer(c1);
            context.AddToPerson(e1);
            context.AddToComputerDetail(cd1);
            var ar0 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar0);

            var ar1 = context.BeginExecute(new Uri("Person/Microsoft.Test.OData.Services.AstoriaDefaultService.Employee" + "/Microsoft.Test.OData.Services.AstoriaDefaultService.IncreaseSalaries", UriKind.Relative), null, null, "POST", new BodyOperationParameter("n", 100)).EnqueueWait(this);
            context.EndExecute(ar1);
            var ar11 = context.BeginLoadProperty(e1, "Salary", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar11);
            Assert.AreEqual(400, e1.Salary);

            var ar2 = context.BeginExecute(new Uri("ComputerDetail(" + cd1.ComputerDetailId + ")" + "/Microsoft.Test.OData.Services.AstoriaDefaultService.ResetComputerDetailsSpecifications", UriKind.Relative), null, null, "POST", new BodyOperationParameter("specifications", specifications), new BodyOperationParameter("purchaseTime", purchaseTime)).EnqueueWait(this);
            context.EndExecute(ar2);
            var ar21 = context.BeginLoadProperty(cd1, "PurchaseDate", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar21);
            var ar22 = context.BeginLoadProperty(cd1, "SpecificationsBag", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar22);
            Assert.AreEqual(purchaseTime, cd1.PurchaseDate);
            Assert.AreEqual(specifications.Aggregate("", (current, item) => current + item),cd1.SpecificationsBag.Aggregate("", (current, item) => current + item));

            var ar3 = context.BeginExecute(new Uri("Customer(1007)/Microsoft.Test.OData.Services.AstoriaDefaultService.ChangeCustomerAuditInfo", UriKind.Relative), null, null, "POST", new BodyOperationParameter("auditInfo", a1)).EnqueueWait(this);
            context.EndExecute(ar3);
            var query = (from c in context.Customer where c.CustomerId == c1.CustomerId select c.Auditing ) as DataServiceQuery<AuditInfo>;
            var ar2222 = query.BeginExecute(null, null).EnqueueWait(this);
            var temp = (query.EndExecute(ar2222) as QueryOperationResponse<AuditInfo>);
            c1.Auditing = temp.SingleOrDefault();
            Assert.AreEqual(c1.Auditing.ModifiedBy , "some-one");
            Assert.AreEqual(c1.Auditing.ModifiedDate, DateTimeOffset.MinValue);
            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Execute actions with no parameter  return and no return;
        /// </summary>
        [TestMethod, Asynchronous]
        public void ActionTestsNoParams()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
          
            Employee e1 = new Employee {Name = "tim", Salary = 300, Title = "bill",PersonId = 1006 };
            context.AddToPerson(e1);
            var ar0 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar0);

            var ar2 = context.BeginExecute(new Uri("Person(1006)/Microsoft.Test.OData.Services.AstoriaDefaultService.Employee" + "/Microsoft.Test.OData.Services.AstoriaDefaultService.Sack", UriKind.Relative), null, null, "POST").EnqueueWait(this);
            context.EndExecute(ar2);
            var ar21 = context.BeginLoadProperty(e1, "Title", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar21);
            Assert.AreEqual("bill[Sacked]",e1.Title );

            var ar1 = context.BeginExecute<Computer>(new Uri("Computer(-10)" + "/Microsoft.Test.OData.Services.AstoriaDefaultService.GetComputer", UriKind.Relative), null, null, "POST").EnqueueWait(this);
            var comp = context.EndExecute<Computer>(ar1).SingleOrDefault();
            Assert.AreEqual(-10, comp.ComputerId);

            this.EnqueueTestComplete();        
        }

        /// <summary>
        /// Test Setlink when only attachTo function is called for derived Type
        /// </summary>
        [TestMethod, Asynchronous]
        public void SetDerivedTypeNavigationLinkTest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            context.Format.UseJson();
            const int personId = -6;
            var query0 = context.Person.Where(p => p.PersonId == personId)  as DataServiceQuery<Person>;
            var ar00 = query0.BeginExecute(null, null).EnqueueWait(this);
            var person = query0.EndExecute(ar00).First();
            var employee = new Employee() { PersonId = 122222, ManagersPersonId = 2222, Salary = 544444444 };
            context.AddToPerson(employee);
            var ar0 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar0);

            context.SetLink(person, "Manager", employee);
            var ar1 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar1);

            var query = context.CreateQuery<Person>("Person").Expand(p => (p as Employee).Manager).Where(p => p.PersonId == personId) as DataServiceQuery<Person>;
            var ar11 = query.BeginExecute(null, null).EnqueueWait(this);
            Person resultPerson = query.EndExecute(ar11).Single();
            Assert.AreEqual(person.PersonId, resultPerson.PersonId);
            Assert.AreEqual(person, resultPerson);
            Assert.IsNotNull(context.Links.SingleOrDefault(l => l.SourceProperty == "Manager" && l.Source == person && l.Target == employee));

            context.SetLink(person, "Manager", null);
            var ar2 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar2);
            var ar21 = context.BeginLoadProperty(person, "Manager", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar21);
            Assert.IsNull((person as Employee).Manager);
            this.EnqueueTestComplete();     
        }

        /// <summary>
        /// Test Addlink when only attachTo function is called for base Type
        /// </summary>
        [TestMethod, Asynchronous]
        public void AddBaseTypeNavigationLinkTest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            context.Format.UseJson();
            const int personId = -4;

            var person = new Person() {PersonId = personId};
            context.AttachTo("Person", person);
            var personMetadata = new PersonMetadata {PersonId = 12432};
            context.AddToPersonMetadata(personMetadata);
            var ar0 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar0);

            context.AddLink(person, "PersonMetadata", personMetadata);
            var ar1 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar1);

            var query = context.CreateQuery<Person>("Person").Where(p => p.PersonId == personId) as DataServiceQuery<Person>;
            var arTemp = query.BeginExecute(null, null).EnqueueWait(this);
            Person resultPerson = query.EndExecute(arTemp).Single();
            Assert.AreEqual(person.PersonId, resultPerson.PersonId);
            Assert.AreEqual(person, resultPerson);
            Assert.IsNotNull(context.Links.SingleOrDefault(l => l.SourceProperty == "PersonMetadata" && l.Source == person && l.Target == personMetadata));
            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Add update delete a  link
        /// </summary>
        [TestMethod, Asynchronous]
        public void AddUpdateDeleteAssociationLinkSetLinkTest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            context.BaseUri = this.ServiceUri;
            context.AddAndUpdateResponsePreference = DataServiceResponsePreference.None;
            ///context.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
            context.IgnoreResourceNotFoundException = true;
            context.MergeOption = MergeOption.OverwriteChanges;
            Customer c1 = new Customer { CustomerId = 1004 };
            Customer c2 = new Customer { CustomerId = 1006 };
            Order o1 = new Order { OrderId = 999 };
            context.AddToOrder(o1);
            context.AddToCustomer(c1);
            context.AddToCustomer(c2);

            var ar0 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar0);
            var ar01 = context.BeginLoadProperty(c1, "Orders", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar01);
            var ar02 = context.BeginLoadProperty(o1, "Customer", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar02);
            Assert.AreEqual(0, c1.Orders.Count, "IsTrue c1.Orders.Count == 0");
            Assert.IsNull(o1.Customer, "IsNull o1.Customer");

            context.AddLink(c1, "Orders", o1);
            var ar1 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar1);
            var ar11 = context.BeginLoadProperty(c1, "Orders", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar11);
            var ar12 = context.BeginLoadProperty(o1, "Customer", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar12);
            Assert.AreEqual(1, c1.Orders.Count, "IsTrue c1.Orders.Count == 1");
            Assert.IsNull(o1.Customer, "IsNull o1.Customer");

            context.SetLink(o1, "Customer", c1);
            var ar2 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar2);
            var ar21 = context.BeginLoadProperty(c1, "Orders", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar21);
            var ar22 = context.BeginLoadProperty(o1, "Customer", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar22);
            Assert.AreEqual(1, c1.Orders.Count, "IsTrue c1.Orders.Count == 1");
            Assert.AreEqual(c1, o1.Customer, "IsTrue o1.Customer == c1");

            context.SetLink(o1, "Customer", c2);
            var ar3 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar3);
            var ar31 = context.BeginLoadProperty(c1, "Orders", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar31);
            var ar32 = context.BeginLoadProperty(o1, "Customer", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar32);
            Assert.AreEqual(1, c1.Orders.Count, "IsTrue c1.Orders.Count == 1");
            Assert.AreEqual(c2, o1.Customer, "IsTrue o1.Customer == c2");

            context.DeleteLink(c1, "Orders", o1);
            var ar4 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar4);
            var ar41 = context.BeginLoadProperty(c1, "Orders", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar41);
            var ar42 = context.BeginLoadProperty(o1, "Customer", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar42);
            Assert.AreEqual(c2, o1.Customer, "IsTrue o1.Customer == c2");
            Assert.AreEqual(0, c1.Orders.Count, "IsTrue c1.Orders.Count == 0");

            context.SetLink(o1, "Customer", null);
            var ar5 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar5);
            var ar51 = context.BeginLoadProperty(c1, "Orders", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar51);
            var ar52 = context.BeginLoadProperty(o1, "Customer", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar52);
            Assert.IsNull(o1.Customer, "IsNull o1.Customer");
            Assert.AreEqual(0, c1.Orders.Count, "IsTrue c1.Orders.Count == 0");

            this.EnqueueTestComplete();
        }

        [TestMethod, Asynchronous]
        public void AddUpdateDeleteAssociationLinkAddRelatedObjectTestDerivedTypes()
        {
            //context setup
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            context.BaseUri = this.ServiceUri;
            context.AddAndUpdateResponsePreference = DataServiceResponsePreference.None;
            ///context.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
            context.IgnoreResourceNotFoundException = true;
            context.MergeOption = MergeOption.OverwriteChanges;
            Employee e1 = new Employee {PersonId = 3000};
            Employee e2 = new Employee {PersonId = 3001};
            context.AddToPerson(e1);
            context.AddToPerson(e2);
           
            context.SetLink(e1, "Manager", e2);
            var ar0 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar0);
            var ar01 = context.BeginLoadProperty(e1, "Manager", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar01);
            var ar02 = context.BeginLoadProperty(e2, "Manager", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar02);
            Assert.IsNotNull(e1.Manager, "IsNotNull e1.Manager");
            Assert.IsNull(e2.Manager, "IsNull e2.Manager");
           
            context.SetLink(e1, "Manager", null);
            var ar1 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar1);
            var ar11 = context.BeginLoadProperty(e1, "Manager", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar11);
            var ar12 = context.BeginLoadProperty(e2, "Manager", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar12);
            // TODO: LoadProperty does not remove element on client side from one-one link after it is set to null
            // Assert.IsNull(e1.Manager, "IsNull e1.Manager");
            Assert.IsNull(e2.Manager, "IsNull e2.Manager");

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Add update delete a relationship link
        /// </summary>
        [TestMethod, Asynchronous]
        public void AddUpdateDeleteAssociationLinkAddRelatedObjectTest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            context.BaseUri = this.ServiceUri;
            context.AddAndUpdateResponsePreference = DataServiceResponsePreference.None;
            ///context.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
            context.IgnoreResourceNotFoundException = true;
            context.MergeOption = MergeOption.OverwriteChanges;

            Customer c1 = new Customer {CustomerId = 1000};
            Order o1 = new Order {OrderId = 1001};
            Order o2 = new Order {OrderId = 1002};

            context.AddToCustomer(c1);
            var ar0 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar0);
            var ar01 = context.BeginLoadProperty(c1, "Orders", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar01);
            Assert.AreEqual(0, c1.Orders.Count , "IsTrue c1.Orders.Count == 0");

            context.AddRelatedObject(c1, "Orders", o2);
            var ar1 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar1);
            var ar11 = context.BeginLoadProperty(c1, "Orders", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar11);
            Assert.AreEqual(1, c1.Orders.Count , "IsTrue c1.Orders.Count == 1");

            context.AddRelatedObject(c1, "Orders", o1);
            var ar2 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar2);
            var ar21 = context.BeginLoadProperty(c1, "Orders", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar21);
            Assert.AreEqual(2, c1.Orders.Count , "IsTrue c1.Orders.Count == 2");

            context.DeleteLink(c1, "Orders", o1);
            var ar3 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar3);
            var ar31 = context.BeginLoadProperty(c1, "Orders", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar31);
            Assert.AreEqual(1,c1.Orders.Count ,  "IsTrue c1.Orders.Count == 1");

            context.DeleteLink(c1, "Orders", o2);
            var ar4 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar4);
            var ar41 = context.BeginLoadProperty(c1, "Orders", null, null).EnqueueWait(this);
            context.EndLoadProperty(ar41);
            Assert.AreEqual(0,c1.Orders.Count,  "IsTrue c1.Orders.Count == 0");

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Create, update, delete an entity
        /// </summary>
        [TestMethod, Asynchronous]
        public void AddUpdateDeleteTest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            var peopleQuery = context.Person;
            var personQuery = context.Person.Where(p => p.PersonId == 1000) as DataServiceQuery<Person>;

            // Verify PersonId == 1000 not exists
            var ar1 = peopleQuery.BeginExecute(null, null).EnqueueWait(this);
            var people = peopleQuery.EndExecute(ar1);
            var person = people.Where(p => p.PersonId == 1000).SingleOrDefault();
            Assert.IsNull(person);
            
            // Add
            person = Person.CreatePerson(1000);
            person.Name = "Name1";
            context.AddToPerson(person);

            var ar2 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar2);
            context.Detach(person);
            
            // Verify add
            person = null;
            var ar3 = personQuery.BeginExecute(null, null).EnqueueWait(this);
            person = personQuery.EndExecute(ar3).Single();
            Assert.IsNotNull(person);
            
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
            Assert.AreEqual("Name2", person.Name);
            context.Detach(person);
            
            // Delete
            context.AttachTo("Person", person);
            context.DeleteObject(person);
            var ar6 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar6);
            
            // Verify Delete
            context.Detach(person);
            var ar7 = peopleQuery.BeginExecute(null, null).EnqueueWait(this);
            people = peopleQuery.EndExecute(ar7);
            person = people.Where(p => p.PersonId == 1000).SingleOrDefault();
            Assert.IsNull(person);

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Add update delete an entity using Batch SaveChangesOptions
        /// </summary>
        [TestMethod, Asynchronous]
        public void AddUpdateDeleteBatchTest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            int numberOfPeople = 10;
            for (int i = 0; i < numberOfPeople; i++)
            {
                // Create
                context.AddToPerson(new Person { PersonId = 1000 + i });
            }

            var ar1 = context.BeginSaveChanges(SaveChangesOptions.BatchWithSingleChangeset, null, null).EnqueueWait(this);
            context.EndSaveChanges(ar1);
            var query = context.Person.Where(p => p.PersonId >= 1000) as DataServiceQuery<Person>;
            var ar2 = query.BeginExecute(null, null).EnqueueWait(this);
            var people = query.EndExecute(ar2).ToList();
            Assert.AreEqual(numberOfPeople, people.Count());
            
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
                Assert.AreEqual(person.PersonId.ToString(), person.Name);
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
            Assert.IsFalse(people.Any());
            
            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Add update delete an entity with Media Link
        /// </summary>
        [TestMethod, Asynchronous]
        public void AddUpdateDeleteMediaLinkEntryTest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            var car = new Car { VIN = 1000 };
            context.AddToCar(car);

            // Get an image to set at the media entry
            var mediaEntry = this.GetStream();

            context.SetSaveStream(car, mediaEntry, true,"image/png","UnitTestLogo.png");
            var ar2 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            var changeOperationResponse = context.EndSaveChanges(ar2).FirstOrDefault() as ChangeOperationResponse;
           
            //gets the stream from the car in context and compares the values to what is in mediaEntry
            var ar3 = context.BeginGetReadStream(car, new DataServiceRequestArgs(), null, null).EnqueueWait(this);
            var receiveStream = context.EndGetReadStream(ar3).Stream;
            mediaEntry = this.GetStream();
            var sr2 = new StreamReader(receiveStream).ReadToEnd();
            var sr1 = new StreamReader(mediaEntry).ReadToEnd();
            Assert.AreEqual(sr1, sr2);

            // When we issue a POST request, the ID and edit-media link are not updated on the client, so we need to get the server values. 
            var carDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            
            // Cache the current merge option (we reset to the cached value in the finally block). 
            var cachedMergeOption = context.MergeOption;
            context.MergeOption = MergeOption.OverwriteChanges;
            
            // Get the updated entity from the service. 
            var ar4 = context.BeginExecute<Car>(carDescriptor.EditLink, null, null).EnqueueWait(this);
            car = context.EndExecute<Car>(ar4).First();
            
            // Delete
            context.MergeOption = cachedMergeOption;
            context.DeleteObject(car);
            var ar5 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar5);
            
            // Verify Delete
            context.Detach(car);
            var query = context.Car;
            var ar6 = query.BeginExecute(null, null).EnqueueWait(this);
            var cars = query.EndExecute(ar6);
            car = cars.Where(c => c.VIN == 1000).SingleOrDefault();
            Assert.IsNull(car);

           this.EnqueueTestComplete();
        }

        /// <summary>
        /// Add update delete an entity with named streams
        /// </summary>
        [TestMethod, Asynchronous]
        public void AddUpdateDeleteNamedStreamTest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            var car = new Car { VIN = 1000 };
            context.AddToCar(car);

            // Set the Media Entry
            var stream1 = this.GetStream();
            context.SetSaveStream(car, stream1, true, "image/png", "odata.png");

            // Set the Named Stream
            var stream2 = this.GetStream();
            context.SetSaveStream(car, "Photo", stream2, true, "image/png");
            
            // Save changes
            var ar3 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            var changeOperationResponse = context.EndSaveChanges(ar3).FirstOrDefault() as ChangeOperationResponse;
            
            // When we issue a POST request, the ID and edit-media link are not updated on the client, so we need to get the server values. 
            var carDescriptor = changeOperationResponse.Descriptor as EntityDescriptor;
            var cachedMergeOption = context.MergeOption;
            context.MergeOption = MergeOption.OverwriteChanges;
            
            // Get the entity 
            var ar4 = context.BeginExecute<Car>(carDescriptor.EditLink, null, null).EnqueueWait(this);
            car = context.EndExecute<Car>(ar4).First();
            
            // Delete
            context.MergeOption = cachedMergeOption;
            context.DeleteObject(car);

            // Save changes
            var ar5 = context.BeginSaveChanges(null, null).EnqueueWait(this);
            context.EndSaveChanges(ar5);
            
            // Get all cars
            context.Detach(car);
            var query = context.Car;
            var ar6 = query.BeginExecute(null, null).EnqueueWait(this);
            var cars = query.EndExecute(ar6);

            // Verify Delete
            car = cars.Where(c => c.VIN == 1000).SingleOrDefault();
            Assert.IsNull(car);
            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Gets a dummy stream to use on MLE and Named Streams
        /// </summary>
        /// <returns>The stream</returns>
        private Stream GetStream()
        {
            return  new MemoryStream(new byte[] {64, 65, 66});
        }
    }
}
