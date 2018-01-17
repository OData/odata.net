//---------------------------------------------------------------------
// <copyright file="DataServiceContextTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using Microsoft.OData.Client;
    using System.IO;
    using System.Linq;
#if !PORTABLELIB
    using AstoriaUnitTests.ClientExtensions;
#endif
    using FluentAssertions;
#if !PORTABLELIB
    using Microsoft.OData.Tests;
#endif
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#if !PORTABLELIB
    using ClientStrings = Microsoft.OData.Client.Strings;
#endif

    [TestClass]
    public class DataServiceContextTests
    {
        [TestMethod]
        public void UsePostTunnelingDefaultShouldBeFalseExceptOnSilverlightOrOnPortableLibraryRunningOnSilverlight()
        {
            bool usePostTunnelingDefault = false;
            // If the portable library is running on 
#if SILVERLIGHT
            usePostTunnelingDefault = true;
#endif
            var context = new DataServiceContext();
            context.UsePostTunneling.Should().Be(usePostTunnelingDefault);
        }

#if !PORTABLELIB
        private DataServiceContext testSubject;
        private EventHandler<SendingRequestEventArgs> sendingRequestHandler1 = (sender, args) => { };

        [TestInitialize]
        public void Init()
        {
            this.testSubject = new DataServiceContext(new Uri("http://base.org/"));
        }

        [TestMethod]
        public void EntitySetNameGivenInAddObjectShouldBeRecorded()
        {
            const string entitySetName = "ThisIsMyEntitySetName";
            this.testSubject.AddObject(entitySetName, new Customer());
            this.testSubject.Entities.Single().EntitySetName.Should().Be(entitySetName);
        }

        [TestMethod]
        public void EntitySetNameGivenInAttachShouldBeRecorded()
        {
            const string entitySetName = "ThisIsMyEntitySetName";
            this.testSubject.AttachTo(entitySetName, new Customer());
            this.testSubject.Entities.Single().EntitySetName.Should().Be(entitySetName);
        }

        [TestMethod]
        public void ChangeStateShouldFailWhenSettingToAdded()
        {
            Action changeToAdded = () => this.testSubject.ChangeState(new object(), EntityStates.Added);
            changeToAdded.ShouldThrow<NotSupportedException>().WithMessage(ClientStrings.Context_CannotChangeStateToAdded);
        }

        [TestMethod]
        public void ChangeStateShouldFailWhenChangingAddedToDeleted()
        {
            var entity = new TestEntityType();
            this.testSubject.AddObject("Things", entity);
            Action changeToDeleted = () => this.testSubject.ChangeState(entity, EntityStates.Deleted);
            changeToDeleted.ShouldThrow<InvalidOperationException>().WithMessage(ClientStrings.Context_CannotChangeStateIfAdded(EntityStates.Deleted));
        }

        [TestMethod]
        public void ChangeStateShouldFailWhenChangingAddedToUnchanged()
        {
            var entity = new TestEntityType();
            this.testSubject.AddObject("Things", entity);
            Action changeToUnchanged = () => this.testSubject.ChangeState(entity, EntityStates.Unchanged);
            changeToUnchanged.ShouldThrow<InvalidOperationException>().WithMessage(ClientStrings.Context_CannotChangeStateIfAdded(EntityStates.Unchanged));
        }

        [TestMethod]
        public void ChangeStateShouldFailWhenChangingAddedToModified()
        {
            var entity = new TestEntityType();
            this.testSubject.AddObject("Things", entity);
            Action changeToModified = () => this.testSubject.ChangeState(entity, EntityStates.Modified);
            changeToModified.ShouldThrow<InvalidOperationException>().WithMessage(ClientStrings.Context_CannotChangeStateToModifiedIfNotUnchanged);
        }

        [TestMethod]
        public void ChangeStateShouldFailWhenChangingDeletedToModified()
        {
            var entity = new TestEntityType();
            this.AttachAndGetDescriptor(entity);
            this.testSubject.DeleteObject(entity);
            Action changeToModified = () => this.testSubject.ChangeState(entity, EntityStates.Modified);
            changeToModified.ShouldThrow<InvalidOperationException>().WithMessage(ClientStrings.Context_CannotChangeStateToModifiedIfNotUnchanged);
        }

        [TestMethod]
        public void ChangeStateShouldAllowSettingModifiedToModified()
        {
            var entity = new TestEntityType();
            var descriptor = this.AttachAndGetDescriptor(entity);
            this.testSubject.UpdateObject(entity);
            descriptor.State.Should().Be(EntityStates.Modified);

            this.testSubject.ChangeState(entity, EntityStates.Modified);
            descriptor.State.Should().Be(EntityStates.Modified);
        }

        [TestMethod]
        public void ChangeStateShouldSetToUnchangedAndNotModifyChangeOrder()
        {
            var entity = new TestEntityType();
            var descriptor = this.AttachAndGetDescriptor(entity);
            this.testSubject.UpdateObject(entity);
            descriptor.Should().NotBeNull();
            descriptor.State.Should().Be(EntityStates.Modified);
            descriptor.ChangeOrder.Should().Be(2);

            this.testSubject.ChangeState(entity, EntityStates.Unchanged);
            descriptor.State.Should().Be(EntityStates.Unchanged);
            descriptor.ChangeOrder.Should().Be(2);
        }

        [TestMethod]
        public void ChangeStateShouldSetToModifiedAndSetChangeOrder()
        {
            var entity = new TestEntityType();
            var descriptor = this.AttachAndGetDescriptor(entity);

            this.testSubject.ChangeState(entity, EntityStates.Modified);
            descriptor.State.Should().Be(EntityStates.Modified);
            descriptor.ChangeOrder.Should().Be(2);
        }

        [TestMethod]
        public void ChangeStateShouldSetToDeletedAndSetChangeOrder()
        {
            var entity = new TestEntityType();
            var descriptor = this.AttachAndGetDescriptor(entity);

            this.testSubject.ChangeState(entity, EntityStates.Deleted);
            descriptor.State.Should().Be(EntityStates.Deleted);
            descriptor.ChangeOrder.Should().Be(2);
        }

        private EntityDescriptor AttachAndGetDescriptor(object entity)
        {
            this.testSubject.AttachTo("Things", entity);
            var descriptor = this.testSubject.GetEntityDescriptor(entity);
            descriptor.Should().NotBeNull();
            descriptor.State.Should().Be(EntityStates.Unchanged);
            descriptor.ChangeOrder.Should().Be(1);
            return descriptor;
        }

        [TestMethod]
        public void ChangeStateShouldSetToDetachedAndActuallyRemoveFromTracking()
        {
            var entity = new TestEntityType();
            var descriptor = this.AttachAndGetDescriptor(entity);

            this.testSubject.ChangeState(entity, EntityStates.Detached);
            descriptor.State.Should().Be(EntityStates.Detached);
            this.testSubject.GetEntityDescriptor(entity).Should().BeNull();
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [TestMethod]
        public void ChangingStateToUnchangedShouldPreventRequestFromBeingSent()
        {
            // regression coverage for Light switch issue: due to attach detach workaround identity strings don't match and duplicates occur with doubles,
            // which was really due to: DataServiceContext should support changing EntityState and ETag values for entity instances 
            var entity = new TestEntityType();
            this.testSubject.AttachTo("Things", entity);
            this.testSubject.UpdateObject(entity);
            this.testSubject.ChangeState(entity, EntityStates.Unchanged);
            this.testSubject.Configurations.RequestPipeline.OnMessageCreating = (args) => { throw new NotImplementedException(); };
            Action saveChanges = () => this.testSubject.SaveChanges();
            saveChanges.ShouldNotThrow();
        }

        [TestMethod]
        public void SettingETagOnDescriptorShouldOverrideWhatIsSent()
        {
            // regression coverage for Light switch issue: due to attach detach workaround identity strings don't match and duplicates occur with doubles,
            // which was really due to: DataServiceContext should support changing EntityState and ETag values for entity instances 
            var entity = new TestEntityType();
            var transportLayer = new TransportLayerThatRemembersIfMatchAndAlwaysSendsBack204(this.testSubject);

            this.testSubject.AttachTo("Things", entity, "ETagSetInAttach");
            this.testSubject.ChangeState(entity, EntityStates.Modified);
            this.testSubject.GetEntityDescriptor(entity).ETag = "ETagSetInProperty";

            this.testSubject.SaveChanges().Should().HaveCount(1);
            transportLayer.LastIfMatchHeaderValue.Should().Be("ETagSetInProperty");
        }

        [TestMethod]
        public void HeadersInArgsShouldNotBeModifiedDuringGetReadStream()
        {
            // Regression coverage for: SendingRequest cannot set Accept-Charset header for SetSaveStream if reusing request arg
            new TransportLayerThatAlwaysSendsBack204(this.testSubject);
            var entity = new TestEntityType();
            var descriptor = this.AttachAndGetDescriptor(entity);
            descriptor.ReadStreamUri = new Uri("http://fakeReadStreamUri.org/");

            var args = new DataServiceRequestArgs();
            this.testSubject.GetReadStream(entity, args);
            args.Headers.Should().BeEmpty();
        }
#endif

        [TestMethod]
        public void DefaultResolveTypeWithNullTypeNameShouldReturnNullType()
        {
            Assert.IsNull(new DefaultResolveTypeContext().TestDefaultResolveType(null, null, null));
        }

        [TestMethod]
        public void DefaultResolveTypeShouldReturnType()
        {
            const string typeNamespace = "AstoriaUnitTests.TDD.Tests.Client";
            new DefaultResolveTypeContext().TestDefaultResolveType(typeof(ResolveTypeEntityType).FullName, typeNamespace, typeNamespace).Should().Be(typeof(ResolveTypeEntityType));
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [TestMethod]
        public void DisposeShouldBeCalledOnResponseMessageForExecuteWithNoContent()
        {
            DataServiceContext context = new DataServiceContext();
            bool responseMessageDisposed = false;
            context.Configurations.RequestPipeline.OnMessageCreating = args =>
            {
                var requestMessage = new InMemoryMessage { Url = args.RequestUri, Method = args.Method, Stream = new MemoryStream() };
                var responseMessage = new InMemoryMessage { StatusCode = 204, Stream = new MemoryStream() };
                responseMessage.DisposeAction = () => responseMessageDisposed = true;
                return new TestDataServiceClientRequestMessage(requestMessage, () => responseMessage);
            };

            context.Execute(new Uri("http://host/voidAction", UriKind.Absolute), "POST").StatusCode.Should().Be(204);
            responseMessageDisposed.Should().BeTrue();
        }

        [TestMethod]
        public void SaveChangesWithBatchAndBatchWithIndependentOperationsShouldThrow()
        {
            Action test = () => this.testSubject.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.BatchWithIndependentOperations);
            test.ShouldThrow<ArgumentOutOfRangeException>().WithMessage("options", ComparisonMode.Substring);
        }

        [TestMethod]
        public void SaveChangesWithBatchAndContinueOnErrorShouldThrow()
        {
            Action test = () => this.testSubject.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.ContinueOnError);
            test.ShouldThrow<ArgumentOutOfRangeException>().WithMessage("options", ComparisonMode.Substring);
        }

        [TestMethod]
        public void SaveChangesWithBatchWithIndependentOperationsAndContinueOnErrorShouldThrow()
        {
            Action test = () => this.testSubject.SaveChanges(SaveChangesOptions.BatchWithIndependentOperations | SaveChangesOptions.ContinueOnError);
            test.ShouldThrow<ArgumentOutOfRangeException>().WithMessage("options", ComparisonMode.Substring);
        }
#endif

        [TestMethod]
        public void DataServiceContextDoesNotRestrictBaseUriScheme()
        {
            Uri udpBaseUri = new Uri("udp://base.org/");
            this.TestBaseUriScheme(udpBaseUri);

            Uri fileBaseUri = new Uri("file://base.org/");
            this.TestBaseUriScheme(fileBaseUri);

            Uri httpBaseUri = new Uri("http://base.org/");
            this.TestBaseUriScheme(httpBaseUri);

            Uri httpsBaseUri = new Uri("https://base.org/");
            this.TestBaseUriScheme(httpsBaseUri);

            Uri ftpBaseUri = new Uri("ftp://base.org/");
            this.TestBaseUriScheme(ftpBaseUri);
        }

        private void TestBaseUriScheme(Uri baseUri)
        {
            DataServiceContext dsContext = null;
            Action test = () => dsContext = new DataServiceContext(baseUri);
            test.ShouldNotThrow();
            dsContext.BaseUri.ShouldBeEquivalentTo(baseUri);
        }

        private class TransportLayerThatRemembersIfMatchAndAlwaysSendsBack204 : TransportLayerThatAlwaysSendsBack204
        {
            private InMemoryMessage lastMessageCreated;
            internal string LastIfMatchHeaderValue
            {
                get { return this.lastMessageCreated.GetHeader("If-Match"); }
            }

            public TransportLayerThatRemembersIfMatchAndAlwaysSendsBack204(DataServiceContext context)
                : base(context)
            {
                // Do nothing
            }

            protected override InMemoryMessage CreateRequestMessage(DataServiceClientRequestMessageArgs requestMessageArgs)
            {
                this.lastMessageCreated = base.CreateRequestMessage(requestMessageArgs);
                return this.lastMessageCreated;
            }
        }

        private class TransportLayerThatAlwaysSendsBack204
        {
            public TransportLayerThatAlwaysSendsBack204(DataServiceContext context)
            {
                context.Configurations.RequestPipeline.OnMessageCreating = (args) =>
                {
                    var requestMessage = this.CreateRequestMessage(args);
                    return new TestDataServiceClientRequestMessage(requestMessage, () => new InMemoryMessage { StatusCode = 204, Stream = new MemoryStream() });
                };
            }

            protected virtual InMemoryMessage CreateRequestMessage(DataServiceClientRequestMessageArgs requestMessageArgs)
            {
                var requestMessage = new InMemoryMessage { Url = requestMessageArgs.RequestUri, Method = requestMessageArgs.Method, Stream = new MemoryStream() };

                foreach (var header in requestMessageArgs.Headers)
                {
                    requestMessage.SetHeader(header.Key, header.Value);
                }

                return requestMessage;
            }
        }

        private class TestEntityType
        {
            public int ID { get; set; }
        }
#endif
    }

    internal class DefaultResolveTypeContext : DataServiceContext
    {
        internal Type TestDefaultResolveType(string typeName, string fullNamespace, string languageDependentNamespace)
        {
            return this.DefaultResolveType(typeName, fullNamespace, languageDependentNamespace);
        }
    }

    public class ResolveTypeEntityType
    {
        public int ID { get; set; }
    }
}