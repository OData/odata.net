//---------------------------------------------------------------------
// <copyright file="MessageWriterBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.UriParser;

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using System.Linq;
    using AstoriaUnitTests.TDD.Tests.Server.Simulators;
    using AstoriaUnitTests.Tests.Server.Simulators;
    using FluentAssertions;
    using Microsoft.OData;
    using Microsoft.OData.Client;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Caching;
    using Microsoft.OData.Service.Providers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit and short-span integration tests for the MessageWriterBuilder class.
    /// TODO: add tests for the CreateMessageWriter method.
    /// </summary>
    [TestClass]
    public class MessageWriterBuilderTests
    {
        private DataServiceSimulator dataServiceSimulator;
        private DataServiceHost2Simulator host;
        private ODataResponseMessageSimulator responseMessageSimulator;

        [TestInitialize]
        public void Initialize()
        {
            this.host = new DataServiceHost2Simulator();

            var context = new DataServiceOperationContext(this.host);
            this.dataServiceSimulator = new DataServiceSimulator { OperationContext = context };

            var providerSimulator = new DataServiceProviderSimulator();

            DataServiceStaticConfiguration staticConfiguration = new DataServiceStaticConfiguration(this.dataServiceSimulator.Instance.GetType(), providerSimulator);
            IDataServiceProviderBehavior providerBehavior = DataServiceProviderBehavior.CustomDataServiceProviderBehavior;

            var resourceType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "SelectTestNamespace", "Fake", false) { CanReflectOnInstanceType = false, IsOpenType = true };
            resourceType.AddProperty(new ResourceProperty("Id", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int))) { CanReflectOnInstanceTypeProperty = false });
            var resourceSet = new ResourceSet("FakeSet", resourceType);
            resourceSet.SetReadOnly();

            providerSimulator.AddResourceSet(resourceSet);

            var configuration = new DataServiceConfiguration(providerSimulator);
            configuration.SetEntitySetAccessRule("*", EntitySetRights.All);

            var provider = new DataServiceProviderWrapper(
                new DataServiceCacheItem(
                    configuration,
                    staticConfiguration),
                providerSimulator,
                providerSimulator,
                this.dataServiceSimulator,
                false);

            this.dataServiceSimulator.ProcessingPipeline = new DataServiceProcessingPipeline();
            this.dataServiceSimulator.Provider = provider;
            provider.ProviderBehavior = providerBehavior;
            this.dataServiceSimulator.Configuration = new DataServiceConfiguration(providerSimulator);
            this.dataServiceSimulator.Configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;

            this.responseMessageSimulator = new ODataResponseMessageSimulator();
        }

        [TestMethod]
        public void CreatedSettingsShouldHaveCheckCharactersTurnedOff()
        {
            var settings = MessageWriterBuilder.CreateMessageWriterSettings();
            settings.EnableCharactersCheck.Should().BeFalse();
        }

        [TestMethod]
        public void CreatedSettingsUseDefaultQuotas()
        {
            var settings = MessageWriterBuilder.CreateMessageWriterSettings();
            settings.MessageQuotas.MaxReceivedMessageSize.Should().Be(long.MaxValue);
            settings.MessageQuotas.MaxPartsPerBatch = int.MaxValue;
            settings.MessageQuotas.MaxOperationsPerChangeset = int.MaxValue;
            settings.MessageQuotas.MaxNestingDepth = int.MaxValue;
        }

        [TestMethod]
        public void ServiceUriShouldBeSetAsBase()
        {
            Uri serviceUri = new Uri("http://www.example.com");
            var testSubject = new ODataMessageWriterSettings { BaseUri = null };
            MessageWriterBuilder.ApplyCommonSettings(testSubject, serviceUri, VersionUtil.Version4Dot0, this.dataServiceSimulator, this.responseMessageSimulator);
            testSubject.BaseUri.Should().BeSameAs(serviceUri);
        }

        [TestMethod]
        public void VersionShouldBeConvertedAndSet()
        {
            var testSubject = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            MessageWriterBuilder.ApplyCommonSettings(testSubject, new Uri("http://www.example.com"), VersionUtil.Version4Dot0, this.dataServiceSimulator, this.responseMessageSimulator);
            testSubject.Version.Should().Be(ODataVersion.V4);
        }

        [TestMethod]
        public void ApplyCommonSettingsShouldDisableMessageStreamDisposalForTopLevel()
        {
            // AstoriaResponseMessage is only used for representing top-level responses, not individual parts of a batched response.
            var topLevelResponseMessage = new AstoriaResponseMessage(this.host);

            var testSubject = new ODataMessageWriterSettings { EnableMessageStreamDisposal = true };
            MessageWriterBuilder.ApplyCommonSettings(testSubject, new Uri("http://www.example.com"), VersionUtil.Version4Dot0, this.dataServiceSimulator, topLevelResponseMessage);
            testSubject.EnableMessageStreamDisposal.Should().BeFalse();
        }

        [TestMethod]
        public void ApplyCommonSettingsShouldNotDisableMessageStreamDisposalForNonTopLevel()
        {
            // AstoriaResponseMessage is only used for representing top-level responses, not individual parts of a batched response.
            // For this test, its enough to just pass the test implementation of the response contract.
            var nonTopLevelResponseMessage = this.responseMessageSimulator;

            var testSubject = new ODataMessageWriterSettings { EnableMessageStreamDisposal = false };
            MessageWriterBuilder.ApplyCommonSettings(testSubject, new Uri("http://www.example.com"), VersionUtil.Version4Dot0, this.dataServiceSimulator, nonTopLevelResponseMessage);
            testSubject.EnableMessageStreamDisposal.Should().BeTrue();
        }

        [TestMethod]
        public void NormalRequestShouldSetMetadataUri()
        {
            this.host.AbsoluteServiceUri = new Uri("http://myservice.org/");
            this.host.AbsoluteRequestUri = new Uri("http://myservice.org/FakeSet");

            var testSubject = this.ForNormalRequest();

            testSubject.WriterSettings.MetadataDocumentUri.Should().Be("http://myservice.org/$metadata");
            testSubject.WriterSettings.SelectExpandClause.Should().BeNull();
        }

        [TestMethod]
        public void NormalRequestShouldSetMetadataUriWithSelect()
        {
            this.host.AbsoluteServiceUri = new Uri("http://myservice.org/");
            this.host.AbsoluteRequestUri = new Uri("http://myservice.org/FakeSet?$select=Fake");

            var testSubject = this.ForNormalRequest();

            testSubject.WriterSettings.MetadataDocumentUri.Should().Be("http://myservice.org/$metadata");

            var selected = testSubject.WriterSettings.SelectExpandClause.SelectedItems.SingleOrDefault() as PathSelectItem;
            selected.Should().NotBeNull();
        }

        [TestMethod]
        public void RawValueWithMimeTypeShouldUseFormat()
        {
            this.host.AbsoluteServiceUri = new Uri("http://myservice.org/");
            this.host.AbsoluteRequestUri = new Uri("http://myservice.org/FakeSet");

            SegmentInfo segment = new SegmentInfo
            {
                TargetKind = RequestTargetKind.PrimitiveValue,
                TargetSource = RequestTargetSource.Property,
                ProjectedProperty = new ResourceProperty("Fake", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))) { MimeType = "fake/things" }
            };

            var requestDescription = new RequestDescription(new[] { segment }, new Uri("http://temp.org"));

            var testSubject = this.ForNormalRequest(requestDescription);
            testSubject.WriterSettings.UseFormat.Should().BeTrue();
            testSubject.WriterSettings.Format.Should().BeSameAs(ODataFormat.RawValue);
        }

        [TestMethod]
        public void NormalRequestShouldUseAcceptHeader()
        {
            const string accept = "application/json;q=0.1";
            const string acceptCharset = "something";
            this.host.RequestAccept = accept;
            this.host.RequestAcceptCharSet = acceptCharset;
            this.host.AbsoluteServiceUri = new Uri("http://myservice.org/");
            this.host.AbsoluteRequestUri = new Uri("http://myservice.org/FakeSet");

            var testSubject = this.ForNormalRequest();

            testSubject.WriterSettings.UseFormat.Should().BeFalse();
            testSubject.WriterSettings.AcceptableMediaTypes.Should().Be(accept);
            testSubject.WriterSettings.AcceptableCharsets.Should().Be(acceptCharset);
        }

        private MessageWriterBuilder ForNormalRequest()
        {
            var requestDescription = new RequestDescription(RequestTargetKind.Resource, RequestTargetSource.EntitySet, new Uri("http://temp.org/"));

            var resourceType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Fake", "Type", false) { CanReflectOnInstanceType = false, IsOpenType = true };
            resourceType.AddProperty(new ResourceProperty("Id", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int))) { CanReflectOnInstanceTypeProperty = false });
            var resourceSet = new ResourceSet("FakeSet", resourceType);
            resourceSet.SetReadOnly();

            requestDescription.LastSegmentInfo.TargetResourceType = resourceType;
            requestDescription.LastSegmentInfo.TargetResourceSet = ResourceSetWrapper.CreateForTests(resourceSet);

            return this.ForNormalRequest(requestDescription);
        }

        private MessageWriterBuilder ForNormalRequest(RequestDescription requestDescription)
        {
            this.dataServiceSimulator.OperationContext.InitializeAndCacheHeaders(this.dataServiceSimulator);
            this.dataServiceSimulator.OperationContext.RequestMessage.CacheHeaders();
            this.dataServiceSimulator.OperationContext.RequestMessage.InitializeRequestVersionHeaders(VersionUtil.Version4Dot0);

            requestDescription.ParseExpandAndSelect(this.dataServiceSimulator);
            requestDescription.DetermineWhetherResponseBodyOrETagShouldBeWritten(HttpVerbs.GET);
            requestDescription.DetermineWhetherResponseBodyShouldBeWritten(HttpVerbs.GET);
            requestDescription.DetermineResponseFormat(this.dataServiceSimulator);
            var testSubject = MessageWriterBuilder.ForNormalRequest(this.dataServiceSimulator, requestDescription, this.responseMessageSimulator, new EdmModel());
            return testSubject;
        }

        private class FakeReflectionProviderBehavior : IDataServiceProviderBehavior
        {
            public ProviderBehavior ProviderBehavior { get { return new ProviderBehavior(ProviderQueryBehaviorKind.ReflectionProviderQueryBehavior); } }
        }
    }
}
