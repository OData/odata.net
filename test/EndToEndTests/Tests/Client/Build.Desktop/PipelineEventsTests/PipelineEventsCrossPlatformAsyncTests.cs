//---------------------------------------------------------------------
// <copyright file="PipelineEventsCrossPlatformAsyncTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.PipelineEventsTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Microsoft.OData.Client;
    using Microsoft.OData;
    using Microsoft.Test.DataDriven;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReferenceModifiedClientTypes;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Client processing pipeline async test cases.
    /// </summary>
    [TestClass]
    public class PipelineEventsCrossPlatformAsyncTests : EndToEndTestBase
    {
        public PipelineEventsCrossPlatformAsyncTests()
            : base(ServiceDescriptors.AstoriaDefaultServiceModifiedClientTypes)
        {
        }

        /// <summary>
        /// This test covers modifying property values and modifying entry id/links/actions.
        /// </summary>
        [TestMethod]
        public void QueryEntitySetLinqAsync()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, QueryEntitySetLinqAsync);
        }

        private static void QueryEntitySetLinqAsync(DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            contextWrapper.Configurations.ResponsePipeline
                .OnEntryStarted(PipelineEventsTestsHelper.ModifyEntryId_Reading)
                .OnEntryStarted(PipelineEventsTestsHelper.ModifyEntryEditLink_ReadingStart)
                .OnEntryEnded(PipelineEventsTestsHelper.ModifyEntryEditLink_ReadingEnd)
                .OnEntryEnded(PipelineEventsTestsHelper.ModifyEntryAction_Reading)
                .OnNestedResourceInfoEnded(PipelineEventsTestsHelper.ModifyAssociationLinkUrl_ReadingNavigationLink)
                .OnEntityMaterialized(PipelineEventsTestsHelper.ModifyPropertyValueCustomer_Materialized);

            var query = contextWrapper.CreateQuery<Customer>("Customer");

            var customers = Enumerable.Empty<Customer>();
            var r = query.BeginExecute(
                result =>
                {
                    customers = query.EndExecute(result);
                }, null);

            while (!r.IsCompleted)
            {
                Thread.Sleep(1000);
            }

            foreach (Customer customer in customers)
            {
                PipelineEventsTestsHelper.VerifyModfiedCustomerEntry(contextWrapper, customer);
            }
        }

        [TestMethod]
        public void QueryEntitySetExecuteAsync()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, QueryEntitySetExecuteAsync);
        }

        private static void QueryEntitySetExecuteAsync(DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            contextWrapper.Configurations.ResponsePipeline
                .OnEntryStarted(PipelineEventsTestsHelper.ModifyEntryId_Reading)
                .OnEntryStarted(PipelineEventsTestsHelper.ModifyEntryEditLink_ReadingStart)
                .OnEntryEnded(PipelineEventsTestsHelper.ModifyEntryEditLink_ReadingEnd)
                .OnEntryEnded(PipelineEventsTestsHelper.ModifyEntryAction_Reading)
                .OnNestedResourceInfoEnded(PipelineEventsTestsHelper.ModifyAssociationLinkUrl_ReadingNavigationLink)
                .OnEntityMaterialized(PipelineEventsTestsHelper.ModifyPropertyValueCustomer_Materialized);

            IEnumerable<Customer> customers = null;
            IAsyncResult r = contextWrapper.BeginExecute<Customer>(
                new Uri("Customer", UriKind.Relative),
                result => { customers = contextWrapper.EndExecute<Customer>(result); },
                null);

            while (!r.IsCompleted)
            {
                Thread.Sleep(1000);
            }

            foreach (Customer customer in customers)
            {
                PipelineEventsTestsHelper.VerifyModfiedCustomerEntry(contextWrapper, customer);
            }
        }

        /// <summary>
        /// This test covers adding/removing property values and modifying primitive/complex/collection property values.
        /// </summary>
        [TestMethod]
        public void QueryEntityInstanceExecuteAsync()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, QueryEntityInstanceExecuteAsync);
        }

        private static void QueryEntityInstanceExecuteAsync(DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            // contextWrapper.Context.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
            contextWrapper.Configurations.ResponsePipeline
                .OnEntryEnded(PipelineEventsTestsHelper.AddRemovePropertySpecialEmployeeEntry_Reading)
                .OnEntityMaterialized(PipelineEventsTestsHelper.AddEnumPropertySpecialEmployeeEntity_Materialized)
                .OnEntityMaterialized(PipelineEventsTestsHelper.ModifyPropertyValueCustomerEntity_Materialized);

            IEnumerable<SpecialEmployee> specialEmployees = null;
            IAsyncResult r = contextWrapper.BeginExecute<SpecialEmployee>(
                new Uri("Person(-10)", UriKind.Relative),
                result => { specialEmployees = contextWrapper.EndExecute<SpecialEmployee>(result); },
                null);

            while (!r.IsCompleted)
            {
                Thread.Sleep(1000);
            }

            SpecialEmployee specialEmployee = specialEmployees.Single();
            Assert.AreEqual("AddRemovePropertySpecialEmployeeEntry_Reading", specialEmployee.CarsLicensePlate,
                "Unexpected CarsLicensePlate");
            Assert.AreEqual(1, specialEmployee.BonusLevel, "Unexpected BonusLevel");
        }

        [TestMethod]
        public void QueryEntityInstanceBatchAsync()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, QueryEntityInstanceBatchAsync);
        }

        private static void QueryEntityInstanceBatchAsync(DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            // contextWrapper.Context.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
            contextWrapper.Configurations.ResponsePipeline
                .OnEntryEnded(PipelineEventsTestsHelper.AddRemovePropertySpecialEmployeeEntry_Reading)
                .OnEntityMaterialized(PipelineEventsTestsHelper.AddEnumPropertySpecialEmployeeEntity_Materialized)
                .OnEntityMaterialized(PipelineEventsTestsHelper.ModifyPropertyValueCustomerEntity_Materialized);

            DataServiceRequest[] requests = new DataServiceRequest[]
            {
                contextWrapper.CreateQuery<Person>("Person(-10)"),
                contextWrapper.CreateQuery<Customer>("Customer"),
            };

            DataServiceResponse responses = null;
            IAsyncResult r = contextWrapper.BeginExecuteBatch(
                result => { responses = contextWrapper.EndExecuteBatch(result); },
                null,
                requests);

            while (!r.IsCompleted)
            {
                Thread.Sleep(1000);
            }

            bool personVerified = false;
            bool customerVerified = false;
            foreach (QueryOperationResponse response in responses)
            {
                foreach (object p in response)
                {
                    SpecialEmployee se1 = p as SpecialEmployee;
                    Customer c = p as Customer;
                    if (se1 != null)
                    {
                        Assert.AreEqual("AddRemovePropertySpecialEmployeeEntry_Reading", se1.CarsLicensePlate,
                            "Unexpected CarsLicensePlate");
                        Assert.AreEqual(1, se1.BonusLevel, "Unexpected BonusLevel");
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
        /// This test verifies that user can modify entry in a delegate in the case of LoadProperty.
        /// </summary>
        [TestMethod]
        public void LoadPropertyTestAsync()
        {
            for (int i = 0; i < 1; i++)
            {
                var context = this.CreateWrappedContext<DefaultContainer>().Context;
                ///context.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
                string mimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata;
                context.SendingRequest2 += (sender, eventArgs) => ((Microsoft.OData.Client.HttpWebRequestMessage)eventArgs.RequestMessage).SetHeader("Accept", mimeType);
                context.Configurations.ResponsePipeline.OnEntryEnded(PipelineEventsTestsHelper.ModifyEntryId_Reading);

                if (i == 1)
                {
                    //context.Format.UseAtom();
                }

                var ar1 = context.BeginExecute<SpecialEmployee>(new Uri("Person(-10)", UriKind.Relative), null, null).EnqueueWait(this);
                var specialEmployee = context.EndExecute<SpecialEmployee>(ar1).SingleOrDefault();

                var ar11 = context.BeginLoadProperty(specialEmployee, "Car", null, null).EnqueueWait(this);
                var result = context.EndLoadProperty(ar11);
                foreach (Car car in result)
                {
                    EntityDescriptor descriptor = context.GetEntityDescriptor(car);
                    Assert.IsTrue(descriptor.Identity.OriginalString.Contains("ModifyEntryId"), "Wrong Id");
                }

                this.EnqueueTestComplete();
            }
        }

        /// <summary>
        /// Verify that user can modify primitive/complex/collection property values in writing pipeline delegates.
        /// </summary>
        [TestMethod]
        public void AddObjectTestAsync()
        {
            this.Invoke(
                this.AddObjectTestAction,
                CreateData(/*ODataFormat.Atom,*/ ODataFormat.Json),
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

            Customer customer = PipelineEventsTestsHelper.CreateNewCustomer(1100);
            contextWrapper.AddObject("Customer", customer);

            IAsyncResult r1 = contextWrapper.BeginSaveChanges(
                saveChangesOption,
                result =>
                {
                    contextWrapper.EndSaveChanges(result);
                },
                null);

            while (!r1.IsCompleted)
            {
                Thread.Sleep(1000);
            }

            Assert.IsTrue(customer.Name.EndsWith("UpdatedODataEntryPropertyValue"), "Unexpected primitive property");
            Assert.IsTrue(customer.Auditing.ModifiedBy.Equals("UpdatedODataEntryPropertyValue"), "Unexpected complex property");
            Assert.IsTrue(customer.PrimaryContactInfo.EmailBag.Contains("UpdatedODataEntryPropertyValue"));

            contextWrapper.DeleteObject(customer);
            IAsyncResult r2 = contextWrapper.BeginSaveChanges(
                saveChangesOption,
                result =>
                {
                    contextWrapper.EndSaveChanges(result);
                },
                null);

            while (!r2.IsCompleted)
            {
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Verify that user can modify primitive/complex/collection property values in writing pipeline delegates.
        /// </summary>
        [TestMethod]
        public void UpdateObjectTestAsync()
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

            Customer customer = PipelineEventsTestsHelper.CreateNewCustomer(1200);
            contextWrapper.AddObject("Customer", customer);
            IAsyncResult r1 = contextWrapper.BeginSaveChanges(
                result =>
                {
                    contextWrapper.EndSaveChanges(result);
                },
                null);

            while (!r1.IsCompleted)
            {
                Thread.Sleep(1000);
            }

            contextWrapper.Configurations.RequestPipeline
                .OnEntryStarting(PipelineEventsTestsHelper.ModifyPropertyValueCustomerEntity_Writing)
                .OnEntryEnding(PipelineEventsTestsHelper.ModifyPropertyValueCustomerEntry_Writing);

            customer.Name = "update";
            contextWrapper.UpdateObject(customer);
            IAsyncResult r2 = contextWrapper.BeginSaveChanges(
                result =>
                {
                    contextWrapper.EndSaveChanges(result);
                },
                null);

            while (!r2.IsCompleted)
            {
                Thread.Sleep(1000);
            }


            Assert.IsTrue(customer.Name.EndsWith("UpdatedODataEntryPropertyValue"), "Unexpected primitive property");
            Assert.IsTrue(customer.Auditing.ModifiedBy.Equals("UpdatedODataEntryPropertyValue"), "Unexpected complex property");
            Assert.IsTrue(customer.PrimaryContactInfo.EmailBag.Contains("UpdatedODataEntryPropertyValue"));

            contextWrapper.DeleteObject(customer);
            IAsyncResult r3 = contextWrapper.BeginSaveChanges(
                result =>
                {
                    contextWrapper.EndSaveChanges(result);
                },
                null);

            while (!r3.IsCompleted)
            {
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Verify that user can modify entity property and association link through pipeline delegates in AddObject+SetLink scenario.
        /// </summary>
        [TestMethod]
        public void AddObjectSetLinkTestAsync()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, AddObjectSetLinkTestAsync);
        }

        private static void AddObjectSetLinkTestAsync(DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            // These delegates are invoked when the client sends a single request for AddObject+SetLink
            contextWrapper.Configurations.RequestPipeline
                .OnNestedResourceInfoStarting(PipelineEventsTestsHelper.ModifyNavigationLink_WritingStart)
                .OnNestedResourceInfoEnding(PipelineEventsTestsHelper.ModifyNavigationLink_WritingEnd)
                .OnEntityReferenceLink(PipelineEventsTestsHelper.ModifyReferenceLink);

            Customer customer = PipelineEventsTestsHelper.CreateNewCustomer(1300);
            Customer customer2 = PipelineEventsTestsHelper.CreateNewCustomer(1301);
            Customer customer3 = PipelineEventsTestsHelper.CreateNewCustomer(1302);
            Customer customer4 = PipelineEventsTestsHelper.CreateNewCustomer(1303);
            contextWrapper.AddObject("Customer", customer);
            contextWrapper.AddObject("Customer", customer2);
            contextWrapper.AddObject("Customer", customer3);
            contextWrapper.AddObject("Customer", customer4);

            IAsyncResult r1 = contextWrapper.BeginSaveChanges(
                result => { contextWrapper.EndSaveChanges(result); },
                null);

            while (!r1.IsCompleted)
            {
                Thread.Sleep(1000);
            }

            Order order = PipelineEventsTestsHelper.CreateNewOrder(1300);
            contextWrapper.AddObject("Order", order);
            contextWrapper.SetLink(order, "Customer", customer);

            IAsyncResult r2 = contextWrapper.BeginSaveChanges(
                result => { contextWrapper.EndSaveChanges(result); },
                null);

            while (!r2.IsCompleted)
            {
                Thread.Sleep(1000);
            }

            Assert.AreEqual(1300, order.OrderId, "OrderId should not be altered in the pipeline delegates");

            Customer relatedCustomer = null;
            IAsyncResult r3 = contextWrapper.BeginExecute<Customer>(
                new Uri("Order(1300)/Customer", UriKind.Relative),
                result => { relatedCustomer = contextWrapper.EndExecute<Customer>(result).Single(); },
                null);

            while (!r3.IsCompleted)
            {
                Thread.Sleep(1000);
            }

            Assert.AreEqual(1302, relatedCustomer.CustomerId,
                "Associated CustomerId should be altered in the pipeline delegates");

            contextWrapper.DeleteObject(customer);
            contextWrapper.DeleteObject(customer2);
            contextWrapper.DeleteObject(customer3);
            contextWrapper.DeleteObject(customer4);
            contextWrapper.DeleteObject(order);
            IAsyncResult r4 = contextWrapper.BeginSaveChanges(
                result => { contextWrapper.EndSaveChanges(result); },
                null);

            while (!r4.IsCompleted)
            {
                Thread.Sleep(1000);
            }
        }

        [TestMethod]
        public void CancelRequestTest()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, CancelRequestTest);
        }
        private static void CancelRequestTest(DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            contextWrapper.Configurations.RequestPipeline
                .OnEntryStarting(PipelineEventsTestsHelper.ModifyPropertyValueCustomerEntity_Writing)
                .OnEntryEnding(PipelineEventsTestsHelper.ModifyPropertyValueCustomerEntry_Writing);

            Customer customer = PipelineEventsTestsHelper.CreateNewCustomer();
            contextWrapper.AddObject("Customer", customer);

            IAsyncResult result = contextWrapper.BeginSaveChanges(null, null);
            contextWrapper.CancelRequest(result);

            Assert.IsTrue(customer.Name.EndsWith("ModifyPropertyValueCustomerEntity_Writing"), "Unexpected primitive property");
        }

        private DataServiceContextWrapper<DefaultContainer> CreateContext()
        {
            return this.CreateWrappedContext<DefaultContainer>();
        }
    }
}

