//---------------------------------------------------------------------
// <copyright file="DataServiceClientFormatTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using AstoriaUnitTests.TDD.Common;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Client.TDDUnitTests;
    using Microsoft.OData.Client.TDDUnitTests.Tests;
    using ClientStrings = Microsoft.OData.Client.Strings;
    using Xunit;

    public class DataServiceClientFormatTests
    {
        private readonly IEdmModel serviceModel = new EdmModel();
        private DataServiceClientFormat v3TestSubject;
        private DataServiceContext v3Context;


#if (NETCOREAPP1_0 || NETCOREAPP2_0)
        private readonly QueryComponents queryComponentsWithSelect = new QueryComponents(new Uri("http://temp.org/?$select=foo"), new Version(1, 1, 1, 1), typeof(object), null, null);
        private readonly QueryComponents queryComponentsWithoutSelect = new QueryComponents(new Uri("http://temp.org/"), new Version(1, 1, 1, 1), typeof(object), null, null);
#else
        private readonly QueryComponents queryComponentsWithSelect = new QueryComponents(new Uri("http://temp.org/?$select=foo"), new Version(), typeof(object), null, null);
        private readonly QueryComponents queryComponentsWithoutSelect = new QueryComponents(new Uri("http://temp.org/"), new Version(), typeof(object), null, null);
#endif
        private static IODataRequestMessage RequestWithApplicationJson;
        private static IODataResponseMessage ResponseWithApplicationJson;

        public DataServiceClientFormatTests()
        {
            RequestWithApplicationJson = new ODataRequestMessageSimulator();
            RequestWithApplicationJson.SetHeader(XmlConstants.HttpContentType, "aPPlicaTion/jSoN");

            ResponseWithApplicationJson = new ODataResponseMessageSimulator();
            ResponseWithApplicationJson.SetHeader(XmlConstants.HttpContentType, "aPPlicaTion/jSoN");

            this.v3Context = new DataServiceContext(new Uri("http://temp.org/"), ODataProtocolVersion.V4).ReConfigureForNetworkLoadingTests();
            this.v3TestSubject = this.v3Context.Format;
        }

        [Fact]
        public void ApiSample()
        {
            this.v3Context.Format.UseJson(this.serviceModel);
        }

        [Fact]
        public void ContextShouldTrackFormatting()
        {
            var format = this.v3Context.Format;
            format.Should().NotBeNull();
            this.v3Context.Format.Should().BeSameAs(format);
        }

        [Fact]
        public void JsonFormatShouldRequireModelResolver()
        {
            Action passNullResolver = () => this.v3TestSubject.UseJson(null);
            passNullResolver.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void JsonShouldStoreResolverAndFormat()
        {
            this.v3TestSubject.UseJson(this.serviceModel);
            this.v3TestSubject.ODataFormat.Should().BeSameAs(ODataFormat.Json);
            this.v3TestSubject.ServiceModel.Should().BeSameAs(this.serviceModel);
        }



        [Fact]
        public void AtomShouldBeTheDefault()
        {
            this.v3TestSubject.ODataFormat.Should().BeSameAs(ODataFormat.Json);
        }

        [Fact]
        public void TestNetworkLoading()
        {

            // forces metadata to be loaded
            this.v3Context.Format.UseJson();
            this.v3Context.Format.ServiceModel.Should().NotBeNull();

        }

        [Fact]
        public void SetRequestAcceptHeaderShouldSetAcceptHeaderToJsonLightWhenUsingJson()
        {
            this.TestSetRequestAcceptHeader(f => f.UseJson(this.serviceModel), null, TestConstants.MimeApplicationJsonODataMinimalMetadata);
        }

        [Fact]
        public void SetRequestAcceptHeaderShouldNotSetAcceptHeaderIfAlreadySetWhenUsingJson()
        {
            const string initialHeaderValue = "InitialHeaderValue";
            this.TestSetRequestAcceptHeader(f => f.UseJson(this.serviceModel), initialHeaderValue, initialHeaderValue);
        }

        [Fact]
        public void SetRequestAcceptHeaderForStream()
        {
            this.TestSetRequestHeader(f => f.UseJson(this.serviceModel), (f, r) => f.SetRequestAcceptHeaderForStream(r), "Accept", null, "*/*");
        }

        [Fact]
        public void SetRequestAcceptHeaderForCount()
        {
            this.TestSetRequestHeader(f => f.UseJson(this.serviceModel), (f, r) => f.SetRequestAcceptHeaderForCount(r), "Accept", null, "text/plain");
        }

        [Fact]
        public void SetRequestAcceptHeaderForMultipartBatch()
        {
            var headers = new HeaderCollection();
            headers.SetHeader("Content-Type", "multipart/mixed;boundary=xyz_ewquwdu");
            this.TestSetRequestHeaderForBatch(
                f => f.UseJson(this.serviceModel), // Configure DataServiceClientFormat
                headers, // Header Collection
                (f, r) => f.SetRequestAcceptHeaderForBatch(r), // set request header
                "Accept", // Header to set
                null, // Initial header value
                "multipart/mixed"); // Expected header value
        }

        [Fact]
        public void SetRequestAcceptHeaderForJsonBatch()
        {
            var headers = new HeaderCollection();
            headers.SetHeader("Content-Type", "application/json");
            this.TestSetRequestHeaderForBatch(
                f => f.UseJson(this.serviceModel), // Configure DataServiceClientFormat
                headers, // Header Collection
                (f, r) => f.SetRequestAcceptHeaderForBatch(r), // set request header
                "Accept", // Header to set
                null, // Initial header value
                "application/json"); // Expected header value
        }

        [Fact]
        public void SetRequestAcceptHeaderForQueryInJsonWithSelect()
        {
            this.TestSetRequestHeader(
                f => f.UseJson(this.serviceModel),
                (f, r) => f.SetRequestAcceptHeaderForQuery(r, this.queryComponentsWithSelect),
                "Accept",
                null,
                TestConstants.MimeApplicationJsonODataFullMetadata);
        }

        [Fact]
        public void SetRequestAcceptHeaderForQueryInJsonWithoutSelect()
        {
            this.TestSetRequestHeader(
                f => f.UseJson(this.serviceModel),
                (f, r) => f.SetRequestAcceptHeaderForQuery(r, this.queryComponentsWithoutSelect),
                "Accept",
                null,
                TestConstants.MimeApplicationJsonODataMinimalMetadata);
        }


        private void TestSetRequestAcceptHeader(Action<DataServiceClientFormat> configureFormat, string initialHeaderValue, string expectedValueAfterSet)
        {
            this.TestSetRequestHeader(configureFormat, (f, r) => f.SetRequestAcceptHeader(r), TestConstants.HttpAccept, initialHeaderValue, expectedValueAfterSet);
        }

        [Fact]
        public void SetRequestContentTypeHeaderShouldSetContentTypeHeaderToJsonLightWhenUsingJson()
        {
            this.TestSetRequestContentTypeHeaderForEntry(f => f.UseJson(this.serviceModel), null, TestConstants.MimeApplicationJsonODataMinimalMetadata);
        }

        [Fact]
        public void SetRequestContentTypeHeaderShouldSetContentTypeHeaderToJsonLightWhenUsingJsonForLinks()
        {
            this.TestSetRequestContentTypeHeaderForLinks(f => f.UseJson(this.serviceModel), null, TestConstants.MimeApplicationJsonODataMinimalMetadata);
        }

        [Fact]
        public void SetRequestContentTypeHeaderShouldSetContentTypeHeaderToJsonLightWhenUsingJsonForActions()
        {
            this.TestSetRequestContentTypeHeaderForAction(f => f.UseJson(this.serviceModel), null, TestConstants.MimeApplicationJsonODataMinimalMetadata);
        }

        // github: https://github.com/OData/odata.net/issues/879: Need to support instance annotations on feed or nestedResourceInfo.
        [Fact(Skip="Remove Atom")] // Remove Atom
        // [Fact]
        public void SetRequestContentTypeHeaderShouldNotSetContentTypeHeaderIfAlreadySetWhenUsingJson()
        {
            const string initialHeaderValue = "InitialHeaderValue";
            this.TestSetRequestContentTypeHeaderForEntry(f => f.UseJson(this.serviceModel), initialHeaderValue, initialHeaderValue);
        }

        [Fact]
        public void UseJsonOverloadWithNoParameterShouldInvokeDelegate()
        {
            var otherModel = new EdmModel();
            this.v3Context.Format.LoadServiceModel = () => otherModel;
            this.v3Context.Format.UseJson();
            this.v3TestSubject.ServiceModel.Should().BeSameAs(otherModel);
        }

        [Fact]
        public void UseJsonOverloadWithNoParameterShouldPassIfNoDelegateProvided()
        {
             this.v3Context.Format.UseJson();
             this.v3TestSubject.ServiceModel.Should().NotBeNull();
        }

        [Fact]
        public void UseJsonOverloadWithNoParameterShouldFailIfDelegateReturnsNull()
        {
            Action callOverload = () => this.v3Context.Format.UseJson();
            this.v3Context.Format.LoadServiceModel = () => null;
            callOverload.ShouldThrow<InvalidOperationException>().WithMessage(ClientStrings.DataServiceClientFormat_LoadServiceModelRequired);
        }

        [Fact]
        public void ValidateCanWriteRequestMessageShouldNotThrowForV3AndJsonLightWithModel()
        {
            this.v3TestSubject.UseJson(this.serviceModel);
            DataServiceClientFormat.ValidateCanWriteRequestFormat(RequestWithApplicationJson);
        }

        [Fact]
        public void ValidateCanReadResponseMessageShouldNotThrowForV3AndJsonLightWithModel()
        {
            this.v3TestSubject.UseJson(this.serviceModel);
            DataServiceClientFormat.ValidateCanReadResponseFormat(ResponseWithApplicationJson);
        }

        private void TestSetRequestContentTypeHeaderForEntry(Action<DataServiceClientFormat> configureFormat, string initialHeaderValue, string expectedValueAfterSet)
        {
            this.TestSetRequestHeader(configureFormat, (f, h) => f.SetRequestContentTypeForEntry(h), TestConstants.HttpContentType, initialHeaderValue, expectedValueAfterSet);
        }

        private void TestSetRequestContentTypeHeaderForLinks(Action<DataServiceClientFormat> configureFormat, string initialHeaderValue, string expectedValueAfterSet)
        {
            this.TestSetRequestHeader(configureFormat, (f, h) => f.SetRequestContentTypeForLinks(h), TestConstants.HttpContentType, initialHeaderValue, expectedValueAfterSet);
        }

        private void TestSetRequestContentTypeHeaderForAction(Action<DataServiceClientFormat> configureFormat, string initialHeaderValue, string expectedValueAfterSet)
        {
            this.TestSetRequestHeader(configureFormat, (f, h) => f.SetRequestContentTypeForOperationParameters(h), TestConstants.HttpContentType, initialHeaderValue, expectedValueAfterSet);
        }

        private void TestSetRequestHeader(Action<DataServiceClientFormat> configureFormat, Action<DataServiceClientFormat, HeaderCollection> setRequestHeader, string expectedHeaderToSet, string initialHeaderValue, string expectedValueAfterSet)
        {
            var headers = new HeaderCollection();

            configureFormat(this.v3TestSubject);

            headers.SetHeader(expectedHeaderToSet, initialHeaderValue);

            // Verify header has the expected initial value. This ensures that SetHeader above actually what we expect, and didn't use a default value or ignore the set request.
            headers.GetHeader(expectedHeaderToSet).Should().Be(initialHeaderValue);

            // Try to set header to new value and verify
            setRequestHeader(this.v3TestSubject, headers);
            headers.GetHeader(expectedHeaderToSet).Should().Be(expectedValueAfterSet);

            if (expectedHeaderToSet == "Content-Type")
            {
                if (expectedValueAfterSet == TestConstants.MimeApplicationJsonODataMinimalMetadata)
                {
                    headers.GetHeader("OData-Version").Should().Be("4.0");
                }
                else
                {
                    headers.GetHeader("OData-Version").Should().BeNull();
                }
            }

            if (expectedHeaderToSet == "Accept")
            {
                headers.GetHeader("Accept-Charset").Should().Be("UTF-8");
            }
        }

        private void TestSetRequestHeaderForBatch(Action<DataServiceClientFormat> configureFormat, HeaderCollection headerCollection, Action<DataServiceClientFormat, HeaderCollection> setRequestHeader, string expectedHeaderToSet, string initialHeaderValue, string expectedValueAfterSet)
        {
            var headers = headerCollection;
            configureFormat(this.v3TestSubject);

            headers.SetHeader(expectedHeaderToSet, initialHeaderValue);

            // Verify header has the expected initial value. This ensures that SetHeader above actually what we expect, and didn't use a default value or ignore the set request.
            headers.GetHeader(expectedHeaderToSet).Should().Be(initialHeaderValue);

            // Try to set header to new value and verify
            setRequestHeader(this.v3TestSubject, headers);
            headers.GetHeader(expectedHeaderToSet).Should().Be(expectedValueAfterSet);

            if (expectedHeaderToSet == "Accept")
            {
                headers.GetHeader("Accept-Charset").Should().Be("UTF-8");
            }
        }
    }
}
