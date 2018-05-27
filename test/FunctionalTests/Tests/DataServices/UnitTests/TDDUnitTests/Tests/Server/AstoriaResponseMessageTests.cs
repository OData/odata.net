//---------------------------------------------------------------------
// <copyright file="AstoriaResponseMessageTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using System.IO;
    using System.Net;
    using AstoriaUnitTests.TDD.Tests.Server.Simulators;
    using AstoriaUnitTests.Tests.Server.Simulators;
    using FluentAssertions;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the AstoriaResponseMessage class.
    /// </summary>
    [TestClass]
    public class AstoriaResponseMessageTests
    {
        private static Version V4 = new Version(4, 0);
        [TestMethod]
        public void SettingResponseCacheControlHeaderShouldPassThroughToHost()
        {
            var host = new DataServiceHostSimulator { };
            IODataResponseMessage message = new AstoriaResponseMessage(host);
            const string header = "Cache-Control";
            const string value = "value1";

            message.SetHeader(header, value);
            VerifyHeader(message, header, value);
            host.ResponseCacheControl.Should().Be(value);
        }        
        
        [TestMethod]
        public void SettingResponseETagHeaderShouldPassThroughToHost()
        {
            var host = new DataServiceHostSimulator { };
            IODataResponseMessage message = new AstoriaResponseMessage(host);
            const string header = "ETag";
            const string value = "value2";                               

            message.SetHeader(header, value);
            VerifyHeader(message, header, value);
            host.ResponseETag.Should().Be(value);
        }
        
        [TestMethod]
        public void SettingResponseLocationHeaderShouldPassThroughToHost()
        {
            var host = new DataServiceHostSimulator { };
            IODataResponseMessage message = new AstoriaResponseMessage(host);
            const string header = "Location";
            const string value = "value3";

            message.SetHeader(header, value);
            VerifyHeader(message, header, value);
            host.ResponseLocation.Should().Be(value);
        }    

        [TestMethod]
        public void SettingResponseDataServiceVersionHeaderShouldPassThroughToHost()
        {
            var host = new DataServiceHostSimulator { };
            IODataResponseMessage message = new AstoriaResponseMessage(host);
            const string header = "OData-Version";
            const string value = "value4";

            message.SetHeader(header, value);
            VerifyHeader(message, header, value);
            host.ResponseVersion.Should().Be(value);
        }    

        [TestMethod]
        public void SettingResponseContentTypeHeaderShouldPassThroughToHost()
        {
            var host = new DataServiceHostSimulator { };
            IODataResponseMessage message = new AstoriaResponseMessage(host);
            const string header = "Content-Type";
            const string value = "value5";

            message.SetHeader(header, value);
            VerifyHeader(message, header, value);
            host.ResponseContentType.Should().Be(value);
        }    

        /// <summary>
        /// Verifies that the stream is set from the SetStream call, and does not have to be the Host's ResponseStream.
        /// </summary>
        [TestMethod]
        public void StreamIsSetFromMethodNotHost()
        {
            var streamInHost = new MemoryStream();
            var host = new DataServiceHostSimulator { ResponseStream = streamInHost };
            IODataResponseMessage message = new AstoriaResponseMessage(host);
            var streamInResponse = new MemoryStream();
            message.SetStream(streamInResponse);
            message.GetStream().Should().Be(streamInResponse);
        }

        /// <summary>
        /// Verifies that the StatusCode is obtained from the host and when it is set, passes the value through.
        /// </summary>
        [TestMethod]
        public void StatusCodeIsTiedToHost()
        {
            var host = new DataServiceHostSimulator { ResponseStatusCode = 432 };
            IODataResponseMessage message = new AstoriaResponseMessage(host);
            message.StatusCode.Should().Be(432);

            const int code = 123;
            message.StatusCode = code;
            message.StatusCode.Should().Be(code);
            host.ResponseStatusCode.Should().Be(code);
        }

        [TestMethod]
        public void ShouldNotSetODataAnnotationsPreferenceAppliedWhenShouldWriteResponseBodyIsFalse()
        {
            var service = new DataServiceSimulator();
            var descrption = CreateRequestDescription(service, /*shouldWriteResponseBody*/ false);
            descrption.ShouldWriteResponseBody.Should().Be(false);

            AstoriaResponseMessage responseMessage = (AstoriaResponseMessage)service.OperationContext.ResponseMessage;
            responseMessage.PreferenceAppliedHeader().AnnotationFilter.Should().BeNull();
            responseMessage.SetResponseHeaders(descrption, 200);
            responseMessage.PreferenceAppliedHeader().AnnotationFilter.Should().BeNull();
        }

        [TestMethod]
        public void ShouldSetODataAnnotationsPreferenceAppliedWhenShouldWriteResponseBodyIsTrue()
        {
            var service = new DataServiceSimulator();
            var descrption = CreateRequestDescription(service, /*shouldWriteResponseBody*/ true);
            descrption.ShouldWriteResponseBody.Should().Be(true);

            AstoriaResponseMessage responseMessage = (AstoriaResponseMessage)service.OperationContext.ResponseMessage;
            responseMessage.PreferenceAppliedHeader().AnnotationFilter.Should().BeNull();
            responseMessage.SetResponseHeaders(descrption, 200);
            responseMessage.PreferenceAppliedHeader().AnnotationFilter.Should().Be("*");
        }

        private static RequestDescription CreateRequestDescription(DataServiceSimulator service, bool shouldWriteResponseBody)
        {
            service.Configuration = new DataServiceConfiguration(new DataServiceProviderSimulator());
            service.Configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            var host = new DataServiceHost2Simulator();
            host.RequestHeaders["Prefer"] = "odata.include-annotations=\"*\"";
            host.RequestVersion = "4.0;";
            host.RequestMaxVersion = "4.0;";
            host.RequestHttpMethod = shouldWriteResponseBody ? "GET" : "DELETE";
            service.OperationContext = new DataServiceOperationContext(false, host);
            service.OperationContext.InitializeAndCacheHeaders(service);
            service.OperationContext.RequestMessage.InitializeRequestVersionHeaders(V4);

            RequestDescription descrption = new RequestDescription(RequestTargetKind.Resource, RequestTargetSource.EntitySet, new Uri("http://service/set"));
            descrption.AnalyzeClientPreference(service);
            descrption.Preference.AnnotationFilter.Should().Be("*");

            HttpVerbs verb = shouldWriteResponseBody ? HttpVerbs.GET : HttpVerbs.PATCH;
            descrption.DetermineWhetherResponseBodyOrETagShouldBeWritten(verb);
            descrption.DetermineWhetherResponseBodyShouldBeWritten(verb);
            return descrption;
        }

        private static void VerifyHeader(IODataResponseMessage message, string headerName, string expectedValue)
        {
            message.GetHeader(headerName).Should().Be(expectedValue);
        }
    }
}
