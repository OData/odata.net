//---------------------------------------------------------------------
// <copyright file="HostScenarioTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using Microsoft.OData.Service;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using AstoriaUnitTests.TDD.Tests.Server.Simulators;
    using AstoriaUnitTests.TDD.Tests.Server.Util;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HostScenarioTests
    {
        /// <summary>
        /// Tests multiple calls to ProcessRequest() without changing the instance of the Host that is attached to the server.
        /// </summary>
        [TestMethod]
        public void ProcessRequestCalledSecondTimeWithoutAttachHostShouldThrow()
        {
            var host = new DataServiceHostSimulator {AbsoluteRequestUri = new Uri("http://example.com/Customers(1)"), AbsoluteServiceUri = new Uri("http://example.com/"), RequestHttpMethod = "GET", ResponseStream = new MemoryStream(), ProcessExceptionCallBack = (args) => { }};
            var svc = new TestService();
            svc.AttachHost(host);
            svc.ProcessRequest();
            host.ResponseStatusCode.Should().Be(200);
            host.ResponseStream.Position = 0;
            var customerResponse = new StreamReader(host.ResponseStream).ReadToEnd();
            customerResponse.Should().Contain("Customer");
            customerResponse.Should().Contain("Redmond Way");
            customerResponse.Should().Contain("bob@live.com");

            host.ResponseStream = new MemoryStream();
            host.AbsoluteRequestUri = new Uri("http://example.com/Customers(1)/Address");
            Action secondRequest = () => svc.ProcessRequest();
            secondRequest.ShouldThrow<InvalidOperationException>();
        }

        /// <summary>
        /// Tests multiple calls to ProcessRequest() by attaching the Host everytime.
        /// </summary>
        [TestMethod]
        public void ProcessRequestCalledTwiceWithSameHostShouldWork()
        {
            var host = new DataServiceHostSimulator {AbsoluteRequestUri = new Uri("http://example.com/Customers(1)"), AbsoluteServiceUri = new Uri("http://example.com/"), RequestHttpMethod = "GET", ResponseStream = new MemoryStream(), ProcessExceptionCallBack = (args) => { }};
            var svc = new TestService();
            svc.AttachHost(host);
            svc.ProcessRequest();
            host.ResponseStatusCode.Should().Be(200);
            host.ResponseStream.Position = 0;
            var customerResponse = new StreamReader(host.ResponseStream).ReadToEnd();
            customerResponse.Should().Contain("Customer");
            customerResponse.Should().Contain("Redmond Way");
            customerResponse.Should().Contain("bob@live.com");

            host.ResponseStream = new MemoryStream();
            host.AbsoluteRequestUri = new Uri("http://example.com/Customers(1)/Address");
            // re-attach the host before calling ProcessRequest
            svc.AttachHost(host);
            svc.ProcessRequest();
            host.ResponseStatusCode.Should().Be(200);
            host.ResponseStream.Position = 0;
            var addressResponse = new StreamReader(host.ResponseStream).ReadToEnd();
            addressResponse.Should().Contain("Redmond Way");
            addressResponse.Should().NotContain("bob@live.com");
            addressResponse.Should().NotMatch(customerResponse);
        }

        /// <summary>
        /// Tests that the ResponseStatusCode Is not set on the host after ProcessRequest() which results in a 404.
        /// </summary>
        [TestMethod]
        public void ResponseStatusCodeIsNotSetOnHostAfterProcessRequestWith404Error()
        {
            HandleExceptionArgs exceptionArgs = null;
            var host = new DataServiceHostSimulator { AbsoluteRequestUri = new Uri("http://example.com/invalid"), AbsoluteServiceUri = new Uri("http://example.com/"), RequestHttpMethod = "GET", ResponseStream = new MemoryStream(), ProcessExceptionCallBack = (args) => { exceptionArgs = args; } };
            var svc = new TestService();
            svc.AttachHost(host);
            svc.ProcessRequest();
            //// Astoria Server fails to set IDSH.ResponseStatusCode for custom hosts in an error scenario
            //// The behavior has been this way from V2 of WCF Data Services and on, when this is changed
            //// it will be a breaking change
            host.ResponseStatusCode.Should().Be(0);
            host.ResponseStream.Position = 0;
            var customerResponse = new StreamReader(host.ResponseStream).ReadToEnd();
            customerResponse.Should().Contain("Resource not found for the segment 'invalid'.");
            exceptionArgs.ResponseStatusCode.Should().Be(404);
        }
    }
}
