//---------------------------------------------------------------------
// <copyright file="DataServiceOperationContextTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using Microsoft.OData.Service;
    using System.IO;
    using AstoriaUnitTests.TDD.Tests.Server.Simulators;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataServiceOperationContextTests
    {
        [TestMethod]
        public void GetQueryStringItemAsksHost()
        {
            var host = new DataServiceHost2Simulator();
            host.SetQueryStringItem("some-key", "test-value");
            DataServiceOperationContext context = new DataServiceOperationContext(host);
            context.GetQueryStringValue("some-key").Should().Be("test-value");
        }

        [TestMethod]
        public void GetMissingQueryStringItemReturnsNullIfHostReturnsNull()
        {
            var host = new DataServiceHost2Simulator();
            DataServiceOperationContext context = new DataServiceOperationContext(host);
            context.GetQueryStringValue("some-key").Should().Be(null);
        }

        [TestMethod]
        public void GetQueryStringItemNullParameterReturnsNullIndependentOfHost()
        {
            var host = new StupidHostSimulator();
            DataServiceOperationContext context = new DataServiceOperationContext(host);
            context.GetQueryStringValue(null).Should().Be(null);
        }

        [TestMethod]
        public void GetQueryStringItemEmptyParameterReturnsNullIndependentOfHost()
        {
            var host = new StupidHostSimulator();
            DataServiceOperationContext context = new DataServiceOperationContext(host);
            context.GetQueryStringValue("").Should().Be(null);
        }
        
        [TestMethod]
        public void AbsoluteServiceUriShouldNotBeSettableInsideBatchOperations()
        {
            var batchOperationContext = new DataServiceOperationContext(true, new DataServiceHost2Simulator());
            Action setAbsoluteServiceUri = () => batchOperationContext.AbsoluteServiceUri = new Uri("http://temp.org");
            setAbsoluteServiceUri.ShouldThrow<InvalidOperationException>().WithMessage(Strings.DataServiceOperationContext_CannotModifyServiceUriInsideBatch);
        }

        public class StupidHostSimulator : IDataServiceHost
        {
            public Uri AbsoluteRequestUri { get; private set; }
            public Uri AbsoluteServiceUri { get; private set; }
            public string RequestAccept { get; private set; }
            public string RequestAcceptCharSet { get; private set; }
            public string RequestContentType { get; private set; }
            public string RequestHttpMethod { get; private set; }
            public string RequestIfMatch { get; private set; }
            public string RequestIfNoneMatch { get; private set; }
            public string RequestMaxVersion { get; private set; }
            public Stream RequestStream { get; private set; }
            public string RequestVersion { get; private set; }
            public string ResponseCacheControl { get; set; }
            public string ResponseContentType { get; set; }
            public string ResponseETag { get; set; }
            public string ResponseLocation { get; set; }
            public int ResponseStatusCode { get; set; }
            public Stream ResponseStream { get; private set; }
            public string ResponseVersion { get; set; }

            public string GetQueryStringItem(string item)
            {
                throw new NotImplementedException();
            }

            public void ProcessException(HandleExceptionArgs args)
            {
                throw new NotImplementedException();
            }
        }
    }
}
