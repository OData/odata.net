//---------------------------------------------------------------------
// <copyright file="ErrorHandlerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Caching;
    using Microsoft.OData.Service.Providers;
    using System.Net;
    using AstoriaUnitTests.TDD.Tests.Server.Simulators;
    using AstoriaUnitTests.Tests.Server.Simulators;
    using FluentAssertions;

    [TestClass]
    public class ErrorHandlerTests
    {
        private const string MimeApplicationAtom = "application/atom+xml";
        private const string MimeApplicationJsonODataMinimalMetadata = "application/json;odata.metadata=minimal";
        private const string MimeApplicationJsonODataFullMetadata = "application/json;odata.metadata=full";
        private const string MimeApplicationJsonODataNoMetadata = "application/json;odata.metadata=none";
        private const string MimeApplicationJson = "application/json";
        private const string MimeApplicationXml = "application/xml";
        private const string MimeTextPlain = "text/plain";
        private const string MimeTextXml = "text/xml";
        private const string Utf8Charset = ";charset=utf-8";

        private DataServiceHost2Simulator host;
        private DataServiceSimulator service;

        [TestInitialize]
        public void InitTest()
        {
            this.host = new DataServiceHost2Simulator();
            this.host.AbsoluteServiceUri = new Uri("http://service/");
            this.host.AbsoluteServiceUri = new Uri("http://service/set1");

            var providerSimulator = new DataServiceProviderSimulator();
            this.service = new DataServiceSimulator
            {
                OperationContext = new DataServiceOperationContext(false, this.host),
                Configuration = new DataServiceConfiguration(providerSimulator)
            };

            DataServiceStaticConfiguration staticConfiguration = new DataServiceStaticConfiguration(this.service.Instance.GetType(), providerSimulator);
            IDataServiceProviderBehavior providerBehavior = DataServiceProviderBehavior.CustomDataServiceProviderBehavior;

            this.service.ProcessingPipeline = new DataServiceProcessingPipeline();
            this.service.Provider = new DataServiceProviderWrapper(
                new DataServiceCacheItem(
                    this.service.Configuration, 
                    staticConfiguration), 
                providerSimulator, 
                providerSimulator, 
                this.service,
                false);
            this.service.Provider.ProviderBehavior = providerBehavior;
            this.service.OperationContext.InitializeAndCacheHeaders(this.service);
        }

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/885
        [Ignore] // Remove Atom
        // [TestMethod]
        public void HandleExceptionArgShouldContainCorrectResponseContentTypeWhenMpvIs30()
        {
            var testCases = new Dictionary<string, string>
            {
                {MimeApplicationAtom, MimeApplicationXml + Utf8Charset},
                {MimeApplicationJsonODataMinimalMetadata, MimeApplicationJsonODataMinimalMetadata + Utf8Charset},
                {MimeApplicationJsonODataFullMetadata, MimeApplicationJsonODataFullMetadata + Utf8Charset},
                {MimeApplicationJsonODataNoMetadata, MimeApplicationJsonODataNoMetadata + Utf8Charset},
                {MimeApplicationJson, MimeApplicationJsonODataMinimalMetadata + Utf8Charset},
                {MimeApplicationXml, MimeApplicationXml + Utf8Charset},
                {MimeTextPlain, MimeApplicationXml + Utf8Charset},

                // TODO: Server/ODL -- Writing an error payload when the response content-type is text/xml fails.
                //{MimeTextXml, MimeTextXml + ";charset=utf-8"},
            };

            this.service.Configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            foreach (var testCase in testCases)
            {
                this.host.RequestAccept = testCase.Key;
                this.service.OperationContext.InitializeAndCacheHeaders(this.service);

                this.host.ProcessExceptionCallBack = args => args.ResponseContentType.Should().Be(testCase.Value);
                ErrorHandler.HandleBeforeWritingException(new DataServiceException(500, "foo"), this.service);
            }
        }

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/885
        [Ignore] // Remove Atom
        // [TestMethod]
        public void HandleExceptionArgShouldContainCorrectResponseContentTypeWhenMpvIs20()
        {
            var testCases = new Dictionary<string, string>
            {
                {MimeApplicationAtom, MimeApplicationXml + Utf8Charset},
                {MimeApplicationJsonODataMinimalMetadata, MimeApplicationJsonODataMinimalMetadata + Utf8Charset},
                {MimeApplicationJsonODataFullMetadata, MimeApplicationJsonODataFullMetadata + Utf8Charset},
                {MimeApplicationJsonODataNoMetadata, MimeApplicationJsonODataNoMetadata + Utf8Charset},
                {MimeApplicationJson, MimeApplicationJsonODataMinimalMetadata + Utf8Charset},
                {MimeApplicationXml, MimeApplicationXml + Utf8Charset},
                {MimeTextPlain, MimeApplicationXml + Utf8Charset},

                // TODO: Server/ODL -- Writing an error payload when the response content-type is text/xml fails.
                // {MimeTextXml, MimeTextXml + ";charset=utf-8"},
            };

            this.service.Configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            foreach (var testCase in testCases)
            {
                this.host.RequestAccept = testCase.Key;
                this.service.OperationContext.InitializeAndCacheHeaders(this.service);

                this.host.ProcessExceptionCallBack = args => args.ResponseContentType.Should().Be(testCase.Value);
                ErrorHandler.HandleBeforeWritingException(new DataServiceException(500, "foo"), this.service);
            }
        }
    }
}
