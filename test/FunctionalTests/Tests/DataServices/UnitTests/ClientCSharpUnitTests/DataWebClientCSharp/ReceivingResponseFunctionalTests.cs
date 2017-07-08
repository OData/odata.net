//---------------------------------------------------------------------
// <copyright file="ReceivingResponseFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.DataWebClientCSharp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using Microsoft.OData.Service;
    using Microsoft.OData.Client;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.ServiceModel.Web;
    using System.Threading.Tasks;
    using AstoriaUnitTests.Stubs;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Functional tests for the ReceivingResponse event on DataServiceContext.
    /// We set up a server that always adds a custom header to the responses, and then try to read it
    /// in ReceivingResponse on the client in each of the various code paths.
    /// </summary>
    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/880
    [Ignore] // Remove Atom
    // [TestClass]
    public class ReceivingResponseFunctionalTests
    {
        private int receivingResponseHitCount = 0;
        private const string ResponseHeaderName = "CustomResponseHeader";
        private const string ResponseHeaderValue = "SimpleValue";

        [Ignore] // Remove Atom
        [TestMethod]
        public void QueryResponseShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();
                AddReceivingResponseValidationListener(ctx, AssertDescriptorIsNull);

                var query = ctx.CreateQuery<Product>("Products");
                query = query.Where(p => p.ID == 1) as DataServiceQuery<Product>;

                var result = query.Execute();
                result.ToList();

                this.receivingResponseHitCount.Should().Be(1);
            }
        }

        [TestMethod]
        public void InsertShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();

                Product product = new Product(77, "bottle", false);
                ctx.AddObject("Products", product);

                AddReceivingResponseValidationListener(ctx, args =>
                {
                    args.Descriptor.Should().BeSameAs(ctx.GetEntityDescriptor(product));
                    var entityDescriptor = args.Descriptor as EntityDescriptor;
                    entityDescriptor.Should().NotBeNull();
                    entityDescriptor.State.Should().Be(EntityStates.Added);
                });

                var result = ctx.SaveChanges();
                result.ToList();

                this.receivingResponseHitCount.Should().Be(1);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void UpdateShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();

                ctx.ResolveName = T => T.FullName;
                Customer customer = new CustomerWithBirthday() { ID = 1, Name = "NewSampleTestNameAfterUpdate" };
                ctx.AttachTo("Customers", customer, "*");
                ctx.UpdateObject(customer);
                ctx.AddAndUpdateResponsePreference = DataServiceResponsePreference.IncludeContent;

                AddReceivingResponseValidationListener(ctx, args =>
                {
                    args.Descriptor.Should().BeSameAs(ctx.GetEntityDescriptor(customer));
                    var entityDescriptor = args.Descriptor as EntityDescriptor;
                    entityDescriptor.Should().NotBeNull();
                    entityDescriptor.State.Should().Be(EntityStates.Modified);
                });

                var response = ctx.SaveChanges();
                foreach (var theResponse in response)
                {
                    Assert.AreEqual(200, theResponse.StatusCode);
                }

                var customers = ctx.Entities.Where(d => d.Entity is Customer);
                customers.Select(d => d.Entity).Should().Contain(e => ((Customer)e).Name == "NewSampleTestNameAfterUpdate");

                this.receivingResponseHitCount.Should().Be(1);
            }
        }

        [TestMethod]
        public void InsertUpdateNoContentShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();

                ctx.AddAndUpdateResponsePreference = DataServiceResponsePreference.NoContent;
                ctx.ResolveName = T => T.FullName;

                EntityDescriptor expectedDescriptor = null;
                EntityStates expectedStates = EntityStates.Unchanged;

                AddReceivingResponseValidationListener(ctx, args =>
                {
                    args.Descriptor.Should().BeSameAs(expectedDescriptor);
                    var entityDescriptor = args.Descriptor as EntityDescriptor;
                    entityDescriptor.Should().NotBeNull();
                    entityDescriptor.State.Should().Be(expectedStates);
                    args.ResponseMessage.StatusCode.Should().Be(204);
                });

                Product product = new Product(77, "bottle", false);
                ctx.AddObject("Products", product);
                expectedDescriptor = ctx.GetEntityDescriptor(product);
                expectedStates = EntityStates.Added;
                var result = ctx.SaveChanges();
                this.receivingResponseHitCount.Should().Be(1);
                this.receivingResponseHitCount = 0;

                Customer customer = new Customer() { ID = 2, Name = "NewSampleTestName" };
                ctx.AttachTo("Customers", customer, "*");
                ctx.UpdateObject(customer);
                expectedDescriptor = ctx.GetEntityDescriptor(customer);
                expectedStates = EntityStates.Modified;
                ctx.SaveChanges();
                this.receivingResponseHitCount.Should().Be(1);
            }
        }

        [TestMethod]
        public void DeleteShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.Format.UseAtom();

                ctx.ResolveName = T => T.FullName;
                Customer customer = new Customer() { ID = 1 };
                ctx.AttachTo("Customers", customer, "*");
                ctx.DeleteObject(customer);

                AddReceivingResponseValidationListener(ctx, args =>
                {
                    args.Descriptor.Should().BeSameAs(ctx.GetEntityDescriptor(customer));
                    var entityDescriptor = args.Descriptor as EntityDescriptor;
                    entityDescriptor.Should().NotBeNull();
                    entityDescriptor.State.Should().Be(EntityStates.Deleted);
                });

                var response = ctx.SaveChanges();
                foreach (var theResponse in response)
                {
                    Assert.AreEqual(204, theResponse.StatusCode);
                }

                this.receivingResponseHitCount.Should().Be(1);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void BatchQueryResponsesShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.Format.UseAtom();
                AddReceivingResponseValidationListener(ctx, AssertDescriptorIsNull);

                var query = ctx.CreateQuery<Customer>("Customers");
                query = query.Where(p => p.ID == 1) as DataServiceQuery<Customer>;

                var result = ctx.ExecuteBatch(query, query);
                result.ToList();

                // We expect ReceivingResponse to be called 3 times - one for the top level batch, and once for each of the two queries.
                this.receivingResponseHitCount.Should().Be(3);
            }
        }

        [TestMethod]
        public void BatchWithIndependentOperationsShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();

                Product product1 = new Product(77, "bottle", false);
                Product product2 = new Product(177, "can", false);
                ctx.AddObject("Products", product1);
                ctx.AddObject("Products", product2);

                var expectedDescriptors = new Queue<Descriptor>(new[] { null, ctx.GetEntityDescriptor(product1), ctx.GetEntityDescriptor(product2) });

                // For $batch top level, no descriptor is expected. For the inner two inserts, we expect the appropriate EntityDescriptor.
                AddReceivingResponseValidationListener(ctx, args =>
                {
                    var expected = expectedDescriptors.Dequeue();
                    args.Descriptor.Should().BeSameAs(expected);
                    if (expected != null) args.Descriptor.State.Should().Be(EntityStates.Added);
                });

                var result = ctx.SaveChanges(SaveChangesOptions.BatchWithIndependentOperations);
                result.ToList();

                // We expect ReceivingResponse to be called 3 times - one for the top level batch, and once for each of the two insert operations.
                this.receivingResponseHitCount.Should().Be(3);
            }
        }

        [TestMethod]
        public void BatchWithIndependentOperationsAsyncInsertsShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();

                Product product1 = new Product(77, "bottle", false);
                Product product2 = new Product(177, "can", false);
                ctx.AddObject("Products", product1);
                ctx.AddObject("Products", product2);

                var expectedDescriptors = new Queue<Descriptor>(new[] { null, ctx.GetEntityDescriptor(product1), ctx.GetEntityDescriptor(product2) });

                // For $batch top level, no descriptor is expected. For the inner two inserts, we expect the appropriate EntityDescriptor.
                AddReceivingResponseValidationListener(ctx, args =>
                {
                    var expected = expectedDescriptors.Dequeue();
                    args.Descriptor.Should().BeSameAs(expected);
                    if (expected != null) args.Descriptor.State.Should().Be(EntityStates.Added);
                });


                object objb = new object();
                IAsyncResult asyncResultsB = ctx.BeginSaveChanges(SaveChangesOptions.BatchWithIndependentOperations, null, objb);
                if (!asyncResultsB.IsCompleted)
                {
                    if (!asyncResultsB.AsyncWaitHandle.WaitOne(new TimeSpan(0, 5, 0), false))
                        throw new TimeoutException("BeginSaveChanges didn't complete in expected time");
                }
                var result = ctx.EndSaveChanges(asyncResultsB);
                result.ToList();
                // We expect ReceivingResponse to be called 3 times - one for the top level batch, and once for each of the two insert operations.
                this.receivingResponseHitCount.Should().Be(3);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void BatchWithIndependentOperationsInsertsLinkShouldCallErrorResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();

                Product product1 = new Product(77, "bottle", false);
                Product product2 = new Product(177, "can", false);
                ctx.AddObject("Products", product1);
                ctx.AddObject("Products", product2);

                ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();

                ctx.ResolveName = T => T.FullName;
                Customer existingCustomer = ctx.Execute<Customer>(new Uri(request.BaseUri + "/Customers")).First();

                // Create a new customer using the same ID
                Customer newCustomer = new Customer() { ID = existingCustomer.ID };
                Order newOrder = new Order() { ID = 499, DollarAmount = 4.99 };

                ctx.AddObject("Customers", newCustomer);
                ctx.AddObject("Orders", newOrder);

                ctx.AddLink(newCustomer, "Orders", newOrder);
                ctx.SetLink(newOrder, "Customer", newCustomer);

                Action test = () => ctx.SaveChanges(SaveChangesOptions.BatchWithIndependentOperations);

                test.ShouldThrow<InvalidOperationException>().WithMessage("One of the link's resources failed to insert.");

            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void BatchWithIndependentOperationsInsertsErrorResponseShouldCallReceiveResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();

                ctx.ResolveName = T => T.FullName;
                Customer existingCustomer = ctx.Execute<Customer>(new Uri(request.BaseUri + "/Customers")).First();

                // Create a new customer using the same ID
                Customer newCustomer = new Customer() { ID = existingCustomer.ID };
                Order newOrder = new Order() { ID = 499, DollarAmount = 4.99 };

                ctx.AddObject("Customers", newCustomer);
                ctx.AddObject("Orders", newOrder);

                var expectedStatusCode = new Queue<int>(new int[] { 400, 201 });


                var response = ctx.SaveChanges(SaveChangesOptions.BatchWithIndependentOperations);

                foreach (var theResponse in response)
                {
                    Assert.AreEqual(expectedStatusCode.Dequeue(), theResponse.StatusCode);
                }

                Assert.AreEqual(2, response.Count());

                Assert.AreEqual(202, response.BatchStatusCode);

            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void BatchInsertsShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();

                Product product1 = new Product(77, "bottle", false);
                Product product2 = new Product(177, "can", false);
                ctx.AddObject("Products", product1);
                ctx.AddObject("Products", product2);

                var expectedDescriptors = new Queue<Descriptor>(new[] { null, ctx.GetEntityDescriptor(product1), ctx.GetEntityDescriptor(product2) });

                // For $batch top level, no descriptor is expected. For the inner two inserts, we expect the appropriate EntityDescriptor.
                AddReceivingResponseValidationListener(ctx, args =>
                {
                    var expected = expectedDescriptors.Dequeue();
                    args.Descriptor.Should().BeSameAs(expected);
                    if (expected != null) args.Descriptor.State.Should().Be(EntityStates.Added);
                });

                var result = ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
                result.ToList();

                // We expect ReceivingResponse to be called 3 times - one for the top level batch, and once for each of the two insert operations.
                this.receivingResponseHitCount.Should().Be(3);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void BatchUpdatesShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();

                ctx.AddAndUpdateResponsePreference = DataServiceResponsePreference.IncludeContent;

                ctx.ResolveName = T => T.FullName;
                Customer customer = new CustomerWithBirthday() { ID = 1, Name = "NewSampleTestNameAfterUpdate" };
                ctx.AttachTo("Customers", customer, "*");
                ctx.UpdateObject(customer);

                ctx.ResolveName = T => T.FullName;
                Customer customer2 = new Customer() { ID = 2, Name = "NewSampleTestNameAfterUpdate2" };
                ctx.AttachTo("Customers", customer2, "*");
                ctx.UpdateObject(customer2);

                var expectedDescriptors = new Queue<Descriptor>(new[] { null, ctx.GetEntityDescriptor(customer), ctx.GetEntityDescriptor(customer2) });

                // For $batch top level, no descriptor is expected. For the inner two updates, we expect the appropriate EntityDescriptor.
                AddReceivingResponseValidationListener(ctx, args =>
                {
                    var expected = expectedDescriptors.Dequeue();
                    args.Descriptor.Should().BeSameAs(expected);
                    if (expected != null) args.Descriptor.State.Should().Be(EntityStates.Modified);
                });

                var response = ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
                foreach (var theResponse in response)
                {
                    Assert.AreEqual(200, theResponse.StatusCode);
                }

                var customers = ctx.Entities.Where(d => d.Entity is Customer);
                customers.Select(d => d.Entity).Should().Contain(e => ((Customer)e).Name == "NewSampleTestNameAfterUpdate").And.Contain(e => ((Customer)e).Name == "NewSampleTestNameAfterUpdate2");

                // We expect ReceivingResponse to be called 3 times - one for the top level batch, and once for each of the two update operations.
                this.receivingResponseHitCount.Should().Be(3);
            }
        }


        [TestMethod]
        public void BatchWithIndependentOperationsUpdatesShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();

                ctx.AddAndUpdateResponsePreference = DataServiceResponsePreference.IncludeContent;

                ctx.ResolveName = T => T.FullName;
                Customer customer = new CustomerWithBirthday() { ID = 1, Name = "NewSampleTestNameAfterUpdate" };
                ctx.AttachTo("Customers", customer, "*");
                ctx.UpdateObject(customer);

                ctx.ResolveName = T => T.FullName;
                Customer customer2 = new Customer() { ID = 2, Name = "NewSampleTestNameAfterUpdate2" };
                ctx.AttachTo("Customers", customer2, "*");
                ctx.UpdateObject(customer2);

                var expectedDescriptors = new Queue<Descriptor>(new[] { null, ctx.GetEntityDescriptor(customer), ctx.GetEntityDescriptor(customer2) });

                // For $batch top level, no descriptor is expected. For the inner two updates, we expect the appropriate EntityDescriptor.
                AddReceivingResponseValidationListener(ctx, args =>
                {
                    var expected = expectedDescriptors.Dequeue();
                    args.Descriptor.Should().BeSameAs(expected);
                    if (expected != null) args.Descriptor.State.Should().Be(EntityStates.Modified);
                });

                var response = ctx.SaveChanges(SaveChangesOptions.BatchWithIndependentOperations);
                foreach (var theResponse in response)
                {
                    Assert.AreEqual(200, theResponse.StatusCode);
                }

                var customers = ctx.Entities.Where(d => d.Entity is Customer);
                customers.Select(d => d.Entity).Should().Contain(e => ((Customer)e).Name == "NewSampleTestNameAfterUpdate").And.Contain(e => ((Customer)e).Name == "NewSampleTestNameAfterUpdate2");

                // We expect ReceivingResponse to be called 3 times - one for the top level batch, and once for each of the two update operations.
                this.receivingResponseHitCount.Should().Be(3);
            }
        }

        [TestMethod]
        public void BatchWithIndependentOperationsAsyncUpdatesShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();

                ctx.AddAndUpdateResponsePreference = DataServiceResponsePreference.IncludeContent;

                ctx.ResolveName = T => T.FullName;
                Customer customer = new CustomerWithBirthday() { ID = 1, Name = "NewSampleTestNameAfterUpdate" };
                ctx.AttachTo("Customers", customer, "*");
                ctx.UpdateObject(customer);

                ctx.ResolveName = T => T.FullName;
                Customer customer2 = new Customer() { ID = 2, Name = "NewSampleTestNameAfterUpdate2" };
                ctx.AttachTo("Customers", customer2, "*");
                ctx.UpdateObject(customer2);

                var expectedDescriptors = new Queue<Descriptor>(new[] { null, ctx.GetEntityDescriptor(customer), ctx.GetEntityDescriptor(customer2) });

                // For $batch top level, no descriptor is expected. For the inner two updates, we expect the appropriate EntityDescriptor.
                AddReceivingResponseValidationListener(ctx, args =>
                {
                    var expected = expectedDescriptors.Dequeue();
                    args.Descriptor.Should().BeSameAs(expected);
                    if (expected != null) args.Descriptor.State.Should().Be(EntityStates.Modified);
                });

                object objb = new object();
                IAsyncResult asyncResultsB = ctx.BeginSaveChanges(SaveChangesOptions.BatchWithIndependentOperations, null, objb);
                if (!asyncResultsB.IsCompleted)
                {
                    if (!asyncResultsB.AsyncWaitHandle.WaitOne(new TimeSpan(0, 5, 0), false))
                        throw new TimeoutException("BeginSaveChanges didn't complete in expected time");
                }
                var response = ctx.EndSaveChanges(asyncResultsB);
                response.ToList();
                foreach (var theResponse in response)
                {
                    Assert.AreEqual(200, theResponse.StatusCode);
                }

                var customers = ctx.Entities.Where(d => d.Entity is Customer);
                customers.Select(d => d.Entity).Should().Contain(e => ((Customer)e).Name == "NewSampleTestNameAfterUpdate").And.Contain(e => ((Customer)e).Name == "NewSampleTestNameAfterUpdate2");

                // We expect ReceivingResponse to be called 3 times - one for the top level batch, and once for each of the two update operations.
                this.receivingResponseHitCount.Should().Be(3);
            }
        }

        [TestMethod]
        public void BatchDeletesShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.Format.UseAtom();

                ctx.ResolveName = T => T.FullName;
                Customer customer = new Customer() { ID = 1 };
                ctx.AttachTo("Customers", customer, "*");
                ctx.DeleteObject(customer);

                ctx.ResolveName = T => T.FullName;
                Customer customer2 = new Customer() { ID = 2 };
                ctx.AttachTo("Customers", customer2, "*");
                ctx.DeleteObject(customer2);

                var expectedDescriptors = new Queue<Descriptor>(new[] { null, ctx.GetEntityDescriptor(customer), ctx.GetEntityDescriptor(customer2) });

                // For $batch top level, no descriptor is expected. For the inner two deletes, we expect the appropriate EntityDescriptor.
                AddReceivingResponseValidationListener(ctx, args =>
                {
                    var expected = expectedDescriptors.Dequeue();
                    args.Descriptor.Should().BeSameAs(expected);
                    if (expected != null) args.Descriptor.State.Should().Be(EntityStates.Deleted);
                });

                var response = ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
                foreach (var theResponse in response)
                {
                    Assert.AreEqual(204, theResponse.StatusCode);
                }

                // We expect ReceivingResponse to be called 3 times - one for the top level batch, and once for each of the two delete operations.
                this.receivingResponseHitCount.Should().Be(3);
            }
        }

        [TestMethod]
        public void BatchWithIndependentOperationsDeletesShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.Format.UseAtom();

                ctx.ResolveName = T => T.FullName;
                Customer customer = new Customer() { ID = 1 };
                ctx.AttachTo("Customers", customer, "*");
                ctx.DeleteObject(customer);

                ctx.ResolveName = T => T.FullName;
                Customer customer2 = new Customer() { ID = 2 };
                ctx.AttachTo("Customers", customer2, "*");
                ctx.DeleteObject(customer2);

                var expectedDescriptors = new Queue<Descriptor>(new[] { null, ctx.GetEntityDescriptor(customer), ctx.GetEntityDescriptor(customer2) });

                // For $batch top level, no descriptor is expected. For the inner two deletes, we expect the appropriate EntityDescriptor.
                AddReceivingResponseValidationListener(ctx, args =>
                {
                    var expected = expectedDescriptors.Dequeue();
                    args.Descriptor.Should().BeSameAs(expected);
                    if (expected != null) args.Descriptor.State.Should().Be(EntityStates.Deleted);
                });

                var response = ctx.SaveChanges(SaveChangesOptions.BatchWithIndependentOperations);
                foreach (var theResponse in response)
                {
                    Assert.AreEqual(204, theResponse.StatusCode);
                }

                // We expect ReceivingResponse to be called 3 times - one for the top level batch, and once for each of the two delete operations.
                this.receivingResponseHitCount.Should().Be(3);
            }
        }

        [TestMethod]
        public void BatchWithIndependentOperationsAsyncDeletesShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.Format.UseAtom();

                ctx.ResolveName = T => T.FullName;
                Customer customer = new Customer() { ID = 1 };
                ctx.AttachTo("Customers", customer, "*");
                ctx.DeleteObject(customer);

                ctx.ResolveName = T => T.FullName;
                Customer customer2 = new Customer() { ID = 2 };
                ctx.AttachTo("Customers", customer2, "*");
                ctx.DeleteObject(customer2);

                var expectedDescriptors = new Queue<Descriptor>(new[] { null, ctx.GetEntityDescriptor(customer), ctx.GetEntityDescriptor(customer2) });

                // For $batch top level, no descriptor is expected. For the inner two deletes, we expect the appropriate EntityDescriptor.
                AddReceivingResponseValidationListener(ctx, args =>
                {
                    var expected = expectedDescriptors.Dequeue();
                    args.Descriptor.Should().BeSameAs(expected);
                    if (expected != null) args.Descriptor.State.Should().Be(EntityStates.Deleted);
                });

                object objb = new object();
                IAsyncResult asyncResultsB = ctx.BeginSaveChanges(SaveChangesOptions.BatchWithIndependentOperations, null, objb);
                if (!asyncResultsB.IsCompleted)
                {
                    if (!asyncResultsB.AsyncWaitHandle.WaitOne(new TimeSpan(0, 5, 0), false))
                        throw new TimeoutException("BeginSaveChanges didn't complete in expected time");
                }
                var response = ctx.EndSaveChanges(asyncResultsB);

                foreach (var theResponse in response)
                {
                    Assert.AreEqual(204, theResponse.StatusCode);
                }

                // We expect ReceivingResponse to be called 3 times - one for the top level batch, and once for each of the two delete operations.
                this.receivingResponseHitCount.Should().Be(3);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void GetReadStreamShouldCallReceivingResponse()
        {
            PlaybackServiceDefinition playbackService = new PlaybackServiceDefinition();
            using (TestWebRequest web = playbackService.CreateForInProcessWcf())
            {
                web.ServiceType = typeof(PlaybackService);
                web.StartService();

                playbackService.OverridingPlayback = GetBlobResponse(web.BaseUri);

                var ctx = GetContextForCustomHeaderResponseService(web);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();

                var query = ctx.CreateQuery<BaseEntity>("Categories(1)");
                var result = query.Execute();

                AddReceivingResponseValidationListener(ctx, args =>
                {
                    var streamDescriptor = args.Descriptor as StreamDescriptor;
                    streamDescriptor.Should().NotBeNull();
                    streamDescriptor.State.Should().Be(EntityStates.Unchanged);
                });

                var readStream = ctx.GetReadStream(result.First(), new DataServiceRequestArgs());
                Assert.IsNotNull(readStream);

                this.receivingResponseHitCount.Should().Be(1);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void SetSaveStreamShouldCallReceivingResponse()
        {
            PlaybackServiceDefinition playbackService = new PlaybackServiceDefinition();
            using (TestWebRequest web = playbackService.CreateForInProcessWcf())
            {
                web.ServiceType = typeof(PlaybackService);
                web.StartService();

                playbackService.OverridingPlayback = GetBlobResponse(web.BaseUri);

                var ctx = GetContextForCustomHeaderResponseService(web);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();

                var query = ctx.CreateQuery<BaseEntity>("Categories(1)");
                var result = query.Execute();

                AddReceivingResponseValidationListener(ctx, args =>
                {
                    var streamDescriptor = args.Descriptor as StreamDescriptor;
                    streamDescriptor.Should().NotBeNull();
                    streamDescriptor.State.Should().Be(EntityStates.Modified);
                });

                Stream newStream = new MemoryStream();
                ctx.SetSaveStream(result.First(), newStream, false, new DataServiceRequestArgs() { ContentType = "application/jpeg" });
                ctx.SaveChanges();

                this.receivingResponseHitCount.Should().Be(1);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void LoadPropertyShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();

                this.AddReceivingResponseValidationListener(ctx, AssertDescriptorIsNull);

                ctx.ResolveName = T => T.FullName;
                Customer customer = new Customer() { ID = 1 };
                ctx.AttachTo("Customers", customer, "*");

                var orders = ctx.LoadProperty(customer, "Orders");
                orders.Cast<object>().ToList<object>();
                Assert.AreEqual(200, orders.StatusCode);

                this.receivingResponseHitCount.Should().Be(1);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void LinqQueryDataServiceCollectionShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();

                this.AddReceivingResponseValidationListener(ctx, AssertDescriptorIsNull);
                var clientType = typeof(OrderWithBinding);
                var serverType = typeof(Order);

                ctx.ResolveType = name => clientType;
                ctx.ResolveName = type => serverType.FullName;

                var orderQuery = ctx.CreateQuery<OrderWithBinding>("Orders");
                DataServiceCollection<OrderWithBinding> customers = new DataServiceCollection<OrderWithBinding>(orderQuery);

                this.receivingResponseHitCount.Should().Be(1);
            }
        }

        private class OrderWithBinding : Order, System.ComponentModel.INotifyPropertyChanged
        {
            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

            public void FirePropertyChanged()
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs("property"));
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void DifferntResponseTypeShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();

                AddReceivingResponseValidationListener(ctx, AssertDescriptorIsNull);

                // Null, Entry, Feed are already covered by other tests

                // primitive type
                this.receivingResponseHitCount = 0;
                ctx.Execute<int>(new Uri(request.ServiceRoot + "/Customers(1)/ID"), "GET", true);
                this.receivingResponseHitCount.Should().Be(1);

                // complex type
                this.receivingResponseHitCount = 0;
                ctx.Execute<Address>(new Uri(request.ServiceRoot + "/Customers(1)/Address"), "GET", true);
                this.receivingResponseHitCount.Should().Be(1);

                // collection type
                this.receivingResponseHitCount = 0;
                ctx.Execute<int>(new Uri(request.ServiceRoot + "/ReturnSimpleCollectionServiceOperation"), "GET", true);
                this.receivingResponseHitCount.Should().Be(1);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void AsyncQueryShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();
                AddReceivingResponseValidationListener(ctx, AssertDescriptorIsNull);

                var query = (DataServiceQuery<Customer>)(ctx.CreateQuery<Customer>("Customers").Where(c => c.Name.Contains("1")));

                var taskFactory = new TaskFactory<IEnumerable<Customer>>();
                var result = taskFactory.FromAsync(query.BeginExecute, query.EndExecute, query);

                result.Wait(10000).Should().BeTrue();

                this.receivingResponseHitCount.Should().Be(1);
            }
        }

        [TestMethod]
        public void ErrorQueryResponseShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.Format.UseAtom();

                AddReceivingResponseValidationListener(ctx, AssertDescriptorIsNull);

                try
                {
                    ctx.Execute<Customer>(new Uri(request.ServiceRoot + "/NonExistingAddress"));
                }
                catch (DataServiceQueryException e)
                {
                    this.receivingResponseHitCount.Should().Be(1);
                    Assert.AreEqual(404, e.Response.StatusCode);
                }

                try
                {
                    receivingResponseHitCount = 0;
                    IAsyncResult result = ctx.BeginExecute<Customer>(new Uri(request.ServiceRoot + "/NonExistingAddress"), null, null);
                    ctx.EndExecute<Customer>(result);
                }
                catch (DataServiceQueryException e)
                {
                    this.receivingResponseHitCount.Should().Be(1);
                    Assert.AreEqual(404, e.Response.StatusCode);
                }
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void BatchQueryErrorResponsesShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.Format.UseAtom();

                var query1 = ctx.CreateQuery<Customer>("Customers");
                var query2 = ctx.CreateQuery<Customer>("NonExistingAddress");

                var expectedStatusCode = new Queue<int>(new int[] { 202, 200, 404 });
                AddReceivingResponseValidationListener(ctx, args =>
                {
                    args.Descriptor.Should().BeNull();
                    args.ResponseMessage.StatusCode.Should().Be(expectedStatusCode.Dequeue());
                });

                // We expect ReceivingResponse called for the top level batch
                var result = ctx.ExecuteBatch(query1, query2);
                this.receivingResponseHitCount.Should().Be(1);

                // We expect ReceivingResponse also called for each of the two queries.
                result.ToList();
                this.receivingResponseHitCount.Should().Be(3);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void BatchInsertErrorResponseShouldCallReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();
                ctx.ResolveName = T => T.FullName;
                Customer existingCustomer = ctx.Execute<Customer>(new Uri(request.BaseUri + "/Customers")).First();

                // Create a new customer using the same ID
                Customer newCustomer = new Customer() { ID = existingCustomer.ID };
                Order newOrder = new Order() { ID = 499, DollarAmount = 4.99 };

                ctx.AddObject("Customers", newCustomer);
                ctx.AddObject("Orders", newOrder);

                ctx.AddLink(newCustomer, "Orders", newOrder);
                ctx.SetLink(newOrder, "Customer", newCustomer);

                var expectedStatusCode = new Queue<int>(new int[] { 202, 400 });

                AddReceivingResponseValidationListener(ctx, args =>
                {
                    args.Descriptor.Should().BeNull();
                    args.ResponseMessage.StatusCode.Should().Be(expectedStatusCode.Dequeue());
                });

                try
                {
                    var result = ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
                }
                catch (Exception e)
                {
                    e.InnerException.Should().NotBeNull("because it should be a message from the server. Actual message: " + e.Message);
                    e.InnerException.Message.Should().Contain("Entity with the same key already present.");
                }

                // We expect ReceivingResponse called for the top level batch and the error inner operation
                this.receivingResponseHitCount.Should().Be(2);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void ModifyQueryResponseInReceivingResponse()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();

                ctx.ReceivingResponse += new EventHandler<ReceivingResponseEventArgs>((sender, arg) =>
                {
                    receivingResponseHitCount++;

                    // modify the response message headers and add cookies
                    HttpWebResponse response = (arg.ResponseMessage as HttpWebResponseMessage).Response;
                    response.Headers["Content-Type"] = "application/json";
                    response.Headers["Content-Length"] = "0";

                    Assert.AreEqual("SimpleValue", response.Headers["CustomResponseHeader"]);
                    response.Cookies.Add(new Cookie("CustomCookie", "CustomCookieValue"));
                    response.Headers["OData-MaxVersion"] = "4.0;";
                });

                var query = ctx.CreateQuery<Customer>("Customers").Where(c => c.ID == 1) as DataServiceQuery<Customer>;

                var result = query.Execute();
                this.receivingResponseHitCount.Should().Be(1);
                Assert.IsTrue(result.Single().ID == 1);
            }
        }

        #region exception test

        [TestMethod]
        public void BatchExceptionEmptyResponseEnumerableTest()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                var ctx = GetContextForCustomHeaderResponseService(request);    // start service
                ctx = new DataServiceContext(                                   // set wrong url expecting error
                    new Uri(request.BaseUri + "/expect_error/"), ODataProtocolVersion.V4);

                Product product = new Product(77, "bottle", false);
                ctx.AddObject("Products_not_existing", product);
                bool gotEnumeratorSuccessfully = false;
                AutoResetEvent waitHandle = new AutoResetEvent(false);
                ctx.BeginSaveChanges(SaveChangesOptions.BatchWithIndependentOperations, (ar) =>
                {
                    try
                    {
                        ctx.EndSaveChanges(ar);               // expect http error status code exception
                    }
                    catch (DataServiceRequestException ex)
                    {
                        foreach (var tmp in ex.Response)      // here verify that we can iterate ex.Response's inner IEnumerable<OperationResponse>
                        {
                            Console.WriteLine(tmp);
                        }

                        gotEnumeratorSuccessfully = true;
                        gotEnumeratorSuccessfully = (ex.Response.Count() == 0);
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                }, null);

                waitHandle.WaitOne(TimeSpan.FromSeconds(30));
                Assert.IsTrue(gotEnumeratorSuccessfully);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Adds an even to RecevingResponse which confirms that we can read a custom header, cannot call SetHeader,
        /// and (if not an inner batch operation) cast to HttpWebResponseMessage and get to the underlying System.Net.HttpWebResponse.
        /// </summary>
        private void AddReceivingResponseValidationListener(DataServiceContext ctx, Action<ReceivingResponseEventArgs> additionalValidation)
        {
            ctx.ReceivingResponse += (sender, eventArgs) =>
            {
                Assert.AreEqual(ResponseHeaderValue, eventArgs.ResponseMessage.GetHeader(ResponseHeaderName));

                Action setHeader = () => eventArgs.ResponseMessage.SetHeader("AddedOnClient", "Success");
                setHeader.ShouldThrow<Exception>(); // TODO: make better

                this.receivingResponseHitCount++;

                if (!eventArgs.IsBatchPart)
                {
                    var responseMessage = eventArgs.ResponseMessage as HttpWebResponseMessage;
                    responseMessage.Should().NotBeNull();
                    responseMessage.Response.Should().NotBeNull();
                }

                if (additionalValidation != null)
                {
                    additionalValidation(eventArgs);
                }
            };
        }

        private void AssertDescriptorIsNull(ReceivingResponseEventArgs args)
        {
            args.Descriptor.Should().BeNull();
        }

        /// <summary>
        /// Gets a DataServiceContext for the <see cref="AddCustomHeaderToResponseService"/> service.
        /// </summary>
        private static DataServiceContext GetContextForCustomHeaderResponseService(TestWebRequest request)
        {
            request.DataServiceType = typeof(AddCustomHeaderToResponseService);
            request.StartService();

            var ctx = new DataServiceContext(new Uri(request.BaseUri), ODataProtocolVersion.V4);
            return ctx;
        }

        /// <summary>
        /// Gets a response payload for a BLOB response.
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns></returns>
        private string GetBlobResponse(string resourcePath)
        {
            if (!resourcePath.EndsWith("\\"))
            {
                resourcePath += "\\";
            }

            return @"HTTP/1.1 200 OK
Proxy-Connection: Keep-Alive
Connection: Keep-Alive
Content-Length: 14546
Via: 1.1 TK5-PRXY-08
Expires: Tue, 17 Jul 2012 00:44:53 GMT
Date: Tue, 17 Jul 2012 00:43:53 GMT
Content-Type: application/atom+xml;charset=utf-8
Server: Microsoft-IIS/7.5
Cache-Control: private
Vary: *
X-Content-Type-Options: nosniff
OData-Version: 4.0;
X-AspNet-Version: 4.0.30319
X-Powered-By: ASP.NET
" + ResponseHeaderName + ": " + ResponseHeaderValue + @"

<?xml version=""1.0"" encoding=""utf-8"" ?> 
<entry xml:base=""http://services.odata.org/V3/Northwind/Northwind.svc/"" xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
<id>http://services.odata.org/V3/Northwind/Northwind.svc/Categories(1)</id> 
<category term=""#NorthwindModel.Category"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" /> 
<link rel=""edit"" title=""Category"" href=""Categories(1)"" /> 
<link rel=""edit-media"" href=""" + resourcePath + @"Categories(1)/$value"" />
<title /> 
<updated>2012-07-17T00:48:10Z</updated> 
<author>
<name /> 
</author>
<content type=""application/jpeg"" src=""" + resourcePath + @"""/>
<m:properties>
</m:properties>
</entry>";
        }

        #endregion

        #region Server

        /// <summary>
        /// Service that adds a custom header to responses it hands out.
        /// </summary>
        private class AddCustomHeaderToResponseService : DataService<CustomDataContext>
        {
            public AddCustomHeaderToResponseService()
            {
                ProcessingPipeline.ProcessedRequest += ProcessedRequestHandler;
            }

            public static void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                config.UseVerboseErrors = true;
            }

            protected override void OnStartProcessingRequest(ProcessRequestArgs args)
            {
                base.OnStartProcessingRequest(args);
                args.OperationContext.ResponseHeaders.Set(ResponseHeaderName, ResponseHeaderValue);
            }

            private void ProcessedRequestHandler(object sender, DataServiceProcessingPipelineEventArgs args)
            {
            }

            [WebInvoke]
            public void VoidPostServiceOperation()
            {
            }

            [WebGet]
            public IEnumerable<int> ReturnSimpleCollectionServiceOperation()
            {
                return new List<int>() { 55, 66, 77, };
            }
        }

        #endregion
    }
}
