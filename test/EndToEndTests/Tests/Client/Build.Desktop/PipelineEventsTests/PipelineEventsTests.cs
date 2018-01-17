//---------------------------------------------------------------------
// <copyright file="PipelineEventsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.PipelineEventsTests
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.OData;
    using Microsoft.Test.DataDriven;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReferenceModifiedClientTypes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Client processing pipeline test cases.
    /// </summary>
    [TestClass]
    public class PipelineEventsTests : EndToEndTestBase
    {
        public PipelineEventsTests()
            : base(ServiceDescriptors.AstoriaDefaultServiceModifiedClientTypes)
        {
        }

        /// <summary>
        /// This test covers modifying property values and modifying entry id/links/actions.
        /// </summary>
        [TestMethod]
        public void QueryEntitySet()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, QueryEntitySet);
        }

        private static void QueryEntitySet(DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            contextWrapper.Configurations.ResponsePipeline
                .OnEntryStarted(PipelineEventsTestsHelper.ModifyEntryId_Reading)
                .OnEntryStarted(PipelineEventsTestsHelper.ModifyEntryEditLink_ReadingStart)
                .OnEntryEnded(PipelineEventsTestsHelper.ModifyEntryEditLink_ReadingEnd)
                .OnEntryEnded(PipelineEventsTestsHelper.ModifyEntryAction_Reading)
                .OnNestedResourceInfoEnded(PipelineEventsTestsHelper.ModifyAssociationLinkUrl_ReadingNavigationLink)
                .OnEntityMaterialized(PipelineEventsTestsHelper.ModifyPropertyValueCustomer_Materialized);

            var entryResultsLinq = contextWrapper.CreateQuery<Customer>("Customer").ToArray();
            foreach (var customer in entryResultsLinq)
            {
                PipelineEventsTestsHelper.VerifyModfiedCustomerEntry(contextWrapper, customer);
            }

            var entryResultsExecute = contextWrapper.Execute<Customer>(new Uri("Customer", UriKind.Relative));
            foreach (Customer customer in entryResultsExecute)
            {
                PipelineEventsTestsHelper.VerifyModfiedCustomerEntry(contextWrapper, customer);
            }

            var customerQuery = contextWrapper.CreateQuery<Customer>("Customer");
            DataServiceCollection<Customer> entryResultsCollection = new DataServiceCollection<Customer>(customerQuery);
            foreach (Customer customer in entryResultsCollection)
            {
                PipelineEventsTestsHelper.VerifyModfiedCustomerEntry(contextWrapper, customer);
            }
        }

        /// <summary>
        /// This test covers modifying ODataEntry to have null complex property
        /// </summary>
        // [TestMethod] // github issuse: #896
        public void QueryEntitySetNull()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, QueryEntitySetNull);
        }

        private static void QueryEntitySetNull(DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            contextWrapper.Configurations.ResponsePipeline
                .OnEntryEnded(PipelineEventsTestsHelper.ChangeEntryPropertyToNull_Reading);

            var entryResultsExecute = contextWrapper.Execute<Customer>(new Uri("Customer", UriKind.Relative));
            foreach (Customer customer in entryResultsExecute)
            {
                Assert.IsNull(customer.Auditing, "Unexpected property value");
            }
        }

        /// <summary>
        /// This test covers adding/removing property values and modifying primitive/complex/collection property values.
        /// </summary>
        [TestMethod]
        public void QueryEntityInstance()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, QueryEntityInstance);
        }

        private static void QueryEntityInstance(DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            // contextWrapper.Context.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
            contextWrapper.Configurations.ResponsePipeline
                .OnEntryEnded(PipelineEventsTestsHelper.AddRemovePropertySpecialEmployeeEntry_Reading)
                .OnEntityMaterialized(PipelineEventsTestsHelper.AddEnumPropertySpecialEmployeeEntity_Materialized)
                .OnEntityMaterialized(PipelineEventsTestsHelper.ModifyPropertyValueCustomerEntity_Materialized);

            var specialEmployee =
                contextWrapper.CreateQuery<Person>("Person").Where(p => p.PersonId == -10).Single() as SpecialEmployee;
            EntityDescriptor descriptor = contextWrapper.GetEntityDescriptor(specialEmployee);
            Assert.AreEqual("AddRemovePropertySpecialEmployeeEntry_Reading", specialEmployee.CarsLicensePlate,
                "Unexpected CarsLicensePlate");
            Assert.AreEqual(1, specialEmployee.BonusLevel, "Unexpected BonusLevel");

            specialEmployee = contextWrapper.Execute<SpecialEmployee>(new Uri("Person(-10)", UriKind.Relative)).Single();
            Assert.AreEqual("AddRemovePropertySpecialEmployeeEntry_Reading", specialEmployee.CarsLicensePlate,
                "Unexpected CarsLicensePlate");
            Assert.AreEqual(1, specialEmployee.BonusLevel, "Unexpected BonusLevel");

            DataServiceRequest[] requests = new DataServiceRequest[]
            {
                contextWrapper.CreateQuery<Person>("Person"),
                contextWrapper.CreateQuery<Customer>("Customer"),
            };

            DataServiceResponse responses = contextWrapper.ExecuteBatch(requests);
            bool personVerified = false;
            bool customerVerified = false;
            foreach (QueryOperationResponse response in responses)
            {
                foreach (object p in response)
                {
                    var specialEmployee1 = p as SpecialEmployee;
                    Customer c = p as Customer;
                    if (specialEmployee1 != null)
                    {
                        Assert.AreEqual("AddRemovePropertySpecialEmployeeEntry_Reading", specialEmployee1.CarsLicensePlate,
                            "Unexpected CarsLicensePlate");
                        Assert.AreEqual(1, specialEmployee1.BonusLevel, "Unexpected BonusLevel");
                        personVerified = true;
                    }

                    if (c != null)
                    {
                        Assert.IsTrue(c.Name.EndsWith("ModifyPropertyValueCustomerEntity_Materialized"),
                            "Unexpected primitive property");
                        Assert.IsTrue(c.Auditing.ModifiedBy.Equals("ModifyPropertyValueCustomerEntity_Materialized"),
                            "Unexpected complex property");
                        Assert.IsTrue(c.PrimaryContactInfo.EmailBag.Contains("ModifyPropertyValueCustomerEntity_Materialized"),
                            "Unexpected collection property");
                        customerVerified = true;
                    }
                }
            }

            Assert.IsTrue(personVerified && customerVerified, "Some inner request does not completed correctly");
        }

        /// <summary>
        /// Verifies that user can change entry type name in pipeline events
        /// </summary>
        [TestMethod]
        public void QueryEntityInstanceChangeTypeName()
        {
            this.RunOnAtomAndJsonFormats(
                this.CreateContext,
                (contextWrapper) =>
                {
                    // contextWrapper.Context.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
                    contextWrapper.Configurations.ResponsePipeline.OnEntryEnded(PipelineEventsTestsHelper.ModifyTypeName_Reading);
                    contextWrapper.Configurations.RequestPipeline.OnEntryEnding(PipelineEventsTestsHelper.ModifyTypeName_Writing);
                    contextWrapper.Context.ResolveType = new Func<string, Type>(this.ResolveTypeFromTypeName);
                    contextWrapper.Context.ResolveName = new Func<Type, string>(this.ResolveTypeNameFromType);

                    var entryResultsLinq = contextWrapper.CreateQuery<Machine>("Computer").ToArray();
                    foreach (var machine in entryResultsLinq)
                    {
                        Assert.IsTrue(machine.Name.EndsWith("ModifyTypeName_Reading"), "Unexpected machine name");
                    }

                    Machine newMachine = new Machine() { ComputerId = 12345, Name = "new machine" };
                    contextWrapper.AddObject("Computer", newMachine);
                    contextWrapper.SaveChanges();

                    Assert.IsTrue(newMachine.Name.EndsWith("new machineModifyTypeName_Reading"), "Unexpected machine name");

                    contextWrapper.DeleteObject(newMachine);
                    contextWrapper.SaveChanges();
                });
        }

        /// <summary>
        /// Helper type resolvers so client can use different type than the server response
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private Type ResolveTypeFromTypeName(string typeName)
        {
            if (typeName.EndsWith("Computer"))
            {
                typeName = "Machine";
            }

            if (typeName.StartsWith("Microsoft.Test.OData.Services.AstoriaDefaultService", global::System.StringComparison.Ordinal))
            {
                return this.GetType().Assembly.GetType(string.Concat("Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReferenceModified" +
                            "ClientTypes", typeName.Substring(51)), false);
            }
            return null;
        }

        protected string ResolveTypeNameFromType(Type clientType)
        {
            if (clientType.Name.EndsWith("Machine"))
            {
                clientType = typeof(Computer);
            }

            if (clientType.Namespace.Equals("Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReferenceModified" +
                    "ClientTypes", global::System.StringComparison.Ordinal))
            {
                return string.Concat("Microsoft.Test.OData.Services.AstoriaDefaultService.", clientType.Name);
            }
            return clientType.FullName;
        }

        /// <summary>
        /// This test verifies that user can modify entry in a delegate in the case of LoadProperty.
        /// </summary>
        [TestMethod]
        public void LoadPropertyTest()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, LoadPropertyTest);
        }

        private static void LoadPropertyTest(DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            // contextWrapper.Context.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
            SpecialEmployee specialEmployee =
                contextWrapper.Execute<SpecialEmployee>(new Uri("Person(-10)", UriKind.Relative)).Single();

            contextWrapper.Configurations.ResponsePipeline.OnEntryEnded(PipelineEventsTestsHelper.ModifyEntryId_Reading);
            QueryOperationResponse<Car> cars =
                contextWrapper.LoadProperty(specialEmployee, "Car") as QueryOperationResponse<Car>;
            foreach (Car car in cars)
            {
                EntityDescriptor descriptor = contextWrapper.GetEntityDescriptor(car);
                Assert.IsTrue(descriptor.Identity.OriginalString.Contains("ModifyEntryId"), "Wrong Id");
            }
        }

        /// <summary>
        /// Verify that user can modify entry property and expanded navigation entry property in a delegate.
        /// </summary>
        [TestMethod]
        public void ExpandQueryTest()
        {
            this.RunOnAtomAndJsonFormats(
                this.CreateContext,
                (contextWrapper) =>
                {
                    contextWrapper.Configurations.ResponsePipeline.OnEntryEnded(PipelineEventsTestsHelper.ModifyEntryId_Reading);
                    Customer customer = contextWrapper.Execute<Customer>(new Uri("Customer(-10)?$expand=Orders", UriKind.Relative)).Single();

                    EntityDescriptor descriptor = contextWrapper.GetEntityDescriptor(customer);
                    Assert.IsTrue(descriptor.Identity.OriginalString.Contains("ModifyEntryId"), "Wrong Id");

                    foreach (Order order in customer.Orders)
                    {
                        EntityDescriptor orderDescriptor = contextWrapper.GetEntityDescriptor(order);
                        Assert.IsTrue(orderDescriptor.Identity.OriginalString.Contains("ModifyEntryId"), "Wrong Id");
                    }

                    // verify that the events are called even if the customer properties are not selected
                    contextWrapper.Configurations.ResponsePipeline
                        .OnEntityMaterialized(PipelineEventsTestsHelper.ModifyPropertyValueCustomerEntity_Materialized)
                        .OnEntryStarted(this.SetOnCustomerEntryStartedCalled);

                    this.ResetDelegateFlags();
                    Customer customer2 = contextWrapper.Execute<Customer>(new Uri("Customer(-10)?$expand=Orders&$select=Orders", UriKind.Relative)).Single();

                    Assert.IsTrue(this.OnCustomerEntryStartedCalled, "SetOnCustomerEntryStartedCalled should be called");
                    Assert.IsTrue(customer2.Name.EndsWith("ModifyPropertyValueCustomerEntity_Materialized"), "Unexpected name");
                    EntityDescriptor descriptor2 = contextWrapper.GetEntityDescriptor(customer2);
                    Assert.IsTrue(descriptor2.Identity.OriginalString.Contains("ModifyEntryId"), "Wrong Id");

                    foreach (Order order in customer2.Orders)
                    {
                        EntityDescriptor orderDescriptor = contextWrapper.GetEntityDescriptor(order);
                        Assert.IsTrue(orderDescriptor.Identity.OriginalString.Contains("ModifyEntryId"), "Wrong Id");
                    }
                });
        }

        /// <summary>
        /// Verify that user can modify feed next link in a delegate.
        /// </summary>
        [TestMethod]
        public void PagingQueryTest()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, PagingQueryTest);
        }

        private static void PagingQueryTest(DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            contextWrapper.Configurations.ResponsePipeline
                .OnFeedStarted(PipelineEventsTestsHelper.ModifyFeedId_ReadingFeed)
                .OnFeedEnded(PipelineEventsTestsHelper.ModifyNextlink_ReadingFeed);

            var entryResultsLinq =
                contextWrapper.CreateQuery<Customer>("Customer").Execute() as QueryOperationResponse<Customer>;
            entryResultsLinq.ToArray();
            Assert.AreEqual("http://modifyfeedidmodifynextlink/",
                entryResultsLinq.GetContinuation().NextLinkUri.AbsoluteUri, "Unexpected next link");
        }

        /// <summary>
        /// This test covers adding/removing selected property and adding navigation that are not selected.
        /// </summary>
        [TestMethod]
        public void ProjectionQueryTest()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, ProjectionQueryTest);
        }

        private static void ProjectionQueryTest(DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            contextWrapper.Configurations.ResponsePipeline
                .OnEntryEnded(PipelineEventsTestsHelper.ModifyMessageEntry_Reading)
                .OnEntityMaterialized(PipelineEventsTestsHelper.ModifyMessageEntry_Materialized);
            var entryResultsExecute =
                contextWrapper.Execute<Message>(
                    new Uri("Message(FromUsername='1',MessageId=-10)?$select=MessageId,ToUsername,Sender",
                        UriKind.Relative))
                    .Single();

            Assert.IsNull(entryResultsExecute.ToUsername, "Unexpected ToUsername");
            Assert.AreEqual("ModifyMessageEntry_ReadingModifyMessageEntry_Materialized", entryResultsExecute.Body,
                "Unexpected Body");
        }

        /// <summary>
        /// Verify that user can modify primitive/complex/collection property values in writing pipeline delegates.
        /// </summary>
        [TestMethod]
        public void AddObjectTest()
        {
            this.Invoke(
                this.AddObjectTestAction,
                CreateData(ODataFormat.Json),
                CreateData(MergeOption.AppendOnly, MergeOption.OverwriteChanges, MergeOption.PreserveChanges),
                CreateData(SaveChangesOptions.None, SaveChangesOptions.BatchWithSingleChangeset, SaveChangesOptions.ContinueOnError, SaveChangesOptions.ReplaceOnUpdate),
                new Constraint[] { });
        }

        public void AddObjectTestAction(ODataFormat format, MergeOption mergeOption, SaveChangesOptions saveChangesOption)
        {
            DataServiceContextWrapper<DefaultContainer> contextWrapper = this.CreateContext();
            contextWrapper.Context.MergeOption = mergeOption;
            if (format == ODataFormat.Json)
            {
                contextWrapper.Format.UseJson();
            }

            contextWrapper.Configurations.RequestPipeline
                .OnEntryStarting(PipelineEventsTestsHelper.ModifyPropertyValueCustomerEntity_Writing)
                .OnEntryEnding(PipelineEventsTestsHelper.ModifyPropertyValueCustomerEntry_Writing);

            Customer customer = PipelineEventsTestsHelper.CreateNewCustomer(100);
            contextWrapper.AddObject("Customer", customer);
            contextWrapper.SaveChanges(saveChangesOption);

            Assert.IsTrue(customer.Name.EndsWith("UpdatedODataEntryPropertyValue"), "Unexpected primitive property");
            Assert.IsTrue(customer.Auditing.ModifiedBy.Equals("UpdatedODataEntryPropertyValue"), "Unexpected complex property");
            Assert.IsTrue(customer.PrimaryContactInfo.EmailBag.Contains("UpdatedODataEntryPropertyValue"));

            contextWrapper.DeleteObject(customer);
            contextWrapper.SaveChanges();
        }

        /// <summary>
        /// Verify that user can modify primitive/complex/collection property values in writing pipeline delegates.
        /// </summary>
        [TestMethod]
        public void UpdateObjectTest()
        {
            this.Invoke(
                this.UpdateObjectTestAction,
                CreateData(/*ODataFormat.Atom,*/ ODataFormat.Json),
                CreateData(MergeOption.AppendOnly, MergeOption.OverwriteChanges, MergeOption.PreserveChanges),
                CreateData(SaveChangesOptions.None, SaveChangesOptions.BatchWithSingleChangeset, SaveChangesOptions.ContinueOnError, SaveChangesOptions.ReplaceOnUpdate),
                new Constraint[] { });
        }

        public void UpdateObjectTestAction(ODataFormat format, MergeOption mergeOption, SaveChangesOptions saveChangesOption)
        {
            DataServiceContextWrapper<DefaultContainer> contextWrapper = this.CreateContext();
            contextWrapper.Context.MergeOption = mergeOption;
            contextWrapper.Context.AddAndUpdateResponsePreference = DataServiceResponsePreference.IncludeContent;
            if (format == ODataFormat.Json)
            {
                contextWrapper.Format.UseJson();
            }

            Customer customer = PipelineEventsTestsHelper.CreateNewCustomer(200);
            contextWrapper.AddObject("Customer", customer);
            contextWrapper.SaveChanges();

            contextWrapper.Configurations.RequestPipeline
                .OnEntryStarting(PipelineEventsTestsHelper.ModifyPropertyValueCustomerEntity_Writing)
                .OnEntryEnding(PipelineEventsTestsHelper.ModifyPropertyValueCustomerEntry_Writing);

            customer.Name = "update";
            contextWrapper.UpdateObject(customer);
            contextWrapper.SaveChanges(saveChangesOption);

            Assert.IsTrue(customer.Name.EndsWith("UpdatedODataEntryPropertyValue"), "Unexpected primitive property");
            Assert.IsTrue(customer.Auditing.ModifiedBy.Equals("UpdatedODataEntryPropertyValue"), "Unexpected complex property");
            Assert.IsTrue(customer.PrimaryContactInfo.EmailBag.Contains("UpdatedODataEntryPropertyValue"));

            contextWrapper.DeleteObject(customer);
            contextWrapper.SaveChanges();
        }

        [TestMethod]
        public void AddUpdateObjectStreamTest()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, AddUpdateObjectStreamTest);
        }

        private static void AddUpdateObjectStreamTest(DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            contextWrapper.Configurations.RequestPipeline
                .OnEntryStarting(PipelineEventsTestsHelper.ModifyPropertyValueCarEntity_Writing)
                .OnEntryEnding(PipelineEventsTestsHelper.ModifyPropertyValueCarEntry_Writing);

            Car car = PipelineEventsTestsHelper.CreateNewCar();
            contextWrapper.AddObject("Car", car);
            contextWrapper.SetSaveStream(car, new MemoryStream(new byte[] { 66, 67 }), true, "text/plain", "slug");
            contextWrapper.SetSaveStream(car, "Photo", new MemoryStream(new byte[] { 66 }), true,
                new DataServiceRequestArgs() { ContentType = "text/plain" });
            contextWrapper.SaveChanges();

            // when DataServiceResponsePreference.IncludeContent is not set, property modified in OnEntryEnding will not be updated in client
            Assert.IsTrue(car.Description.EndsWith("ModifyPropertyValueCarEntity_Writing"), "Unexpected primitive property");

            contextWrapper.SetSaveStream(car, new MemoryStream(new byte[] { 68, 69 }), true, "text/plain", "slug");
            contextWrapper.SetSaveStream(car, "Video", new MemoryStream(new byte[] { 66 }), true,
                new DataServiceRequestArgs() { ContentType = "text/plain" });
            car.Description = "update";
            contextWrapper.UpdateObject(car);
            contextWrapper.SaveChanges();

            // when DataServiceResponsePreference.IncludeContent is not set, property modified in OnEntryEnding will not be updated in client
            Assert.IsTrue(car.Description.EndsWith("ModifyPropertyValueCarEntity_Writing"), "Unexpected primitive property");

            contextWrapper.DeleteObject(car);
            contextWrapper.SaveChanges();
        }

        [TestMethod]
        public void AddUpdateBatchTest()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, AddUpdateBatchTest);
        }

        private static void AddUpdateBatchTest(DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            contextWrapper.Configurations.RequestPipeline
                .OnEntryStarting(PipelineEventsTestsHelper.ModifyPropertyValueCustomerEntity_Writing)
                .OnEntryEnding(PipelineEventsTestsHelper.ModifyPropertyValueCustomerEntry_Writing);

            Customer customer = PipelineEventsTestsHelper.CreateNewCustomer(300);
            contextWrapper.AddObject("Customer", customer);

            Customer customer2 = PipelineEventsTestsHelper.CreateNewCustomer(301);
            contextWrapper.AddObject("Customer", customer2);

            Order order = PipelineEventsTestsHelper.CreateNewOrder(300);
            contextWrapper.AddRelatedObject(customer, "Orders", order);

            contextWrapper.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);

            Assert.IsTrue(customer.Name.EndsWith("UpdatedODataEntryPropertyValue"), "Unexpected primitive property");
            Assert.IsTrue(customer2.Name.EndsWith("UpdatedODataEntryPropertyValue"), "Unexpected primitive property");

            contextWrapper.DeleteObject(customer);
            contextWrapper.DeleteObject(customer2);
            contextWrapper.DeleteObject(order);
            contextWrapper.SaveChanges();
        }

        /// <summary>
        /// Verify that user can modify entity property and association link through pipeline delegates in AddObject+SetLink scenario.
        /// </summary>
        [TestMethod]
        public void AddObjectSetLinkTest()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, AddObjectSetLinkTest);
        }

        private static void AddObjectSetLinkTest(DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            // These delegates are invoked when the client sends a single request for AddObject+SetLink
            contextWrapper.Configurations.RequestPipeline
                .OnNestedResourceInfoStarting(PipelineEventsTestsHelper.ModifyNavigationLink_WritingStart)
                .OnNestedResourceInfoEnding(PipelineEventsTestsHelper.ModifyNavigationLink_WritingEnd)
                .OnEntityReferenceLink(PipelineEventsTestsHelper.ModifyReferenceLink);

            Customer customer = PipelineEventsTestsHelper.CreateNewCustomer(400);
            Customer customer2 = PipelineEventsTestsHelper.CreateNewCustomer(401);
            Customer customer3 = PipelineEventsTestsHelper.CreateNewCustomer(402);
            Customer customer4 = PipelineEventsTestsHelper.CreateNewCustomer(403);
            contextWrapper.AddObject("Customer", customer);
            contextWrapper.AddObject("Customer", customer2);
            contextWrapper.AddObject("Customer", customer3);
            contextWrapper.AddObject("Customer", customer4);
            contextWrapper.SaveChanges();

            Order order = PipelineEventsTestsHelper.CreateNewOrder(400);
            contextWrapper.AddObject("Order", order);
            contextWrapper.SetLink(order, "Customer", customer);
            contextWrapper.SaveChanges();

            Assert.AreEqual(400, order.OrderId, "OrderId should not be altered in the pipeline delegates");
            Customer relatedCustomer = contextWrapper.Execute<Customer>(new Uri("Order(400)/Customer", UriKind.Relative)).Single();
            Assert.AreEqual(402, relatedCustomer.CustomerId, "Associated CustomerId should be altered in the pipeline delegates");

            contextWrapper.DeleteObject(customer);
            contextWrapper.DeleteObject(customer2);
            contextWrapper.DeleteObject(customer3);
            contextWrapper.DeleteObject(customer4);
            contextWrapper.DeleteObject(order);
            contextWrapper.SaveChanges();
        }

        [TestMethod]
        public void ThrowExceptionInPipelineDelegateTest()
        {
            this.RunOnAtomAndJsonFormats(
                this.CreateContext,
                (contextWrapper) =>
                {
                    contextWrapper.Configurations.ResponsePipeline.OnEntryEnded(PipelineEventsTestsHelper.ThrowException_Reading);
                    contextWrapper.Configurations.RequestPipeline.OnEntryEnding(PipelineEventsTestsHelper.ThrowException_Writing);

                    Customer customer = PipelineEventsTestsHelper.CreateNewCustomer();
                    contextWrapper.AddObject("Customer", customer);

                    var e1 = this.Throws<Exception>(() => contextWrapper.SaveChanges());
                    Assert.AreEqual("ThrowException_Writing", e1.Message);

                    var e2 = this.Throws<Exception>(() => contextWrapper.Execute<Customer>(new Uri("Customer", UriKind.Relative)).First());
                    Assert.AreEqual("ThrowException_Reading", e2.Message);
                });
        }

        /// <summary>
        /// Verify delegate behavior of error response, inner error in batch response, in-stream error in response
        /// </summary>
        // [TestMethod] // github issuse: #896
        // there is not feed id when using json format.
        public void ErrorResponseTest()
        {
            DataServiceContextWrapper<DefaultContainer> contextWrapper = this.CreateWrappedContext<DefaultContainer>();
            //contextWrapper.Format.UseAtom();
            contextWrapper.Configurations.ResponsePipeline
                .OnFeedStarted(this.SetOnCustomerFeedStartedCalled)
                .OnEntryStarted(this.SetOnCustomerEntryStartedCalled);

            // regular error response
            this.ResetDelegateFlags();
            this.Throws<Exception>(() => contextWrapper.Execute<Customer>(new Uri("Customer(1234)", UriKind.Relative)).Single());
            Assert.IsFalse(OnCustomerEntryStartedCalled, "Unexpected OnEntryEndedCalled");

            // inner response error in a batch
            DataServiceRequest[] requests = new DataServiceRequest[] {
                        contextWrapper.CreateQuery<Order>("Order"),
                        contextWrapper.CreateQuery<Customer>("Customer(-1234)"),
            };
            this.ResetDelegateFlags();
            this.Throws<Exception>(() =>
            {
                DataServiceResponse responses = contextWrapper.ExecuteBatch(requests);
                foreach (QueryOperationResponse response in responses)
                {
                    foreach (object p in response) { }
                }
            });
            Assert.IsFalse(OnCustomerFeedStartedCalled, "Unexpected OnCustomerFeedStartedCalled");
            Assert.IsFalse(OnCustomerEntryStartedCalled, "Unexpected OnEntryEndedCalled");
            Assert.IsTrue(OnOrderFeedStartedCalled, "Unexpected OnOrderFeedStartedCalled");
            Assert.IsTrue(OnOrderEntryStartedCalled, "Unexpected OnOrderEntryStartedCalled");

            // in-stream error in response
            this.ResetDelegateFlags();
            this.Throws<Exception>(() => contextWrapper.Execute<Customer>(new Uri("InStreamErrorGetCustomer", UriKind.Relative)).ToArray());
            Assert.IsTrue(OnCustomerFeedStartedCalled, "Unexpected OnCustomerFeedStartedCalled");
            Assert.IsTrue(OnCustomerEntryStartedCalled, "Unexpected OnEntryEndedCalled");
        }

        /// <summary>
        /// This test covers the handle of null entry in pipeline.
        /// </summary>
        [TestMethod]
        public void ExpandNullEntry()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, ExpandNullEntry);
        }

        private static void ExpandNullEntry(DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            contextWrapper.Configurations.ResponsePipeline
                .OnEntryStarted(PipelineEventsTestsHelper.ModifyNullableEntryId_Reading)
                .OnEntryEnded(PipelineEventsTestsHelper.ModifyNullalbeEntryEditLink_ReadingEnd);

            var entries = contextWrapper.CreateQuery<License>("License").Expand("Driver").Where(c => c.Name == "3").ToArray();
            Assert.IsTrue(entries.Count() == 1, "Wrong count");
            var license = entries[0];
            Assert.IsNull(license.Driver, "Driver is not null");

            EntityDescriptor descriptor = contextWrapper.GetEntityDescriptor(license);
            Assert.IsTrue(descriptor.Identity.OriginalString.Contains("ModifyEntryId"), "Wrong Id");
            Assert.IsTrue(descriptor.EditLink.AbsoluteUri.Contains("ModifyEntryEditLink"), "Wrong EditLink");
        }

        private bool OnCustomerFeedStartedCalled = false;
        private bool OnCustomerEntryStartedCalled = false;
        private bool OnOrderFeedStartedCalled = false;
        private bool OnOrderEntryStartedCalled = false;

        private void ResetDelegateFlags()
        {
            this.OnCustomerFeedStartedCalled = false;
            this.OnCustomerEntryStartedCalled = false;
            this.OnOrderFeedStartedCalled = false;
            this.OnOrderEntryStartedCalled = false;
        }

        /// <summary>
        /// Set flags indicating OnEntryStarted is called
        /// </summary>
        private Action<ReadingEntryArgs> SetOnCustomerEntryStartedCalled
        {
            get
            {
                return args =>
                {
                    if (args.Entry != null)
                    {
                        if (args.Entry.TypeName.EndsWith("Customer"))
                        {
                            this.OnCustomerEntryStartedCalled = true;
                        }
                        if (args.Entry.TypeName.EndsWith("Order"))
                        {
                            this.OnOrderEntryStartedCalled = true;
                        }
                    }
                };
            }
        }

        /// <summary>
        /// Set flags indicating OnFeedStarted is called
        /// </summary>
        private Action<ReadingFeedArgs> SetOnCustomerFeedStartedCalled
        {
            get
            {
                return args =>
                {
                    if (args.Feed.Id.OriginalString.EndsWith("Customer"))
                    {
                        this.OnCustomerFeedStartedCalled = true;
                    }
                    if (args.Feed.Id.OriginalString.EndsWith("Order"))
                    {
                        this.OnOrderFeedStartedCalled = true;
                    }
                };
            }
        }

        private T Throws<T>(Action action) where T : Exception
        {
            try
            {
                action();
            }
            catch (T e)
            {
                return e;
            }

            Assert.Fail("Expected exception not thrown.");
            return null;
        }

        private DataServiceContextWrapper<DefaultContainer> CreateContext()
        {
            return this.CreateWrappedContext<DefaultContainer>();
        }
    }
}

