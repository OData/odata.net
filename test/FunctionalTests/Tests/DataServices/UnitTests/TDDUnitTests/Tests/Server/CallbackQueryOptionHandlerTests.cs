//---------------------------------------------------------------------
// <copyright file="CallbackQueryOptionHandlerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using Microsoft.OData.Service;
    using AstoriaUnitTests.TDD.Tests.Server.Simulators;
    using FluentAssertions;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ds = Microsoft.OData.Service;

    [TestClass]
    public class CallbackQueryOptionHandlerTests
    {
        [TestMethod]
        public void NoCallbackQueryOptionShouldDoNothingSpecial()
        {
            var host = new DataServiceHostSimulator { RequestHttpMethod = "GET" };
            var result = CallbackQueryOptionHandler.HandleCallbackQueryOption(new AstoriaRequestMessage(host), new ODataFormatWithParameters(ODataFormat.Json));
            result.Should().BeNull();
        }

        [TestMethod]
        public void CallbackQueryOptionShouldWorkIfJson()
        {
            var host = new DataServiceHostSimulator { RequestHttpMethod = "GET" };
            host.SetQueryStringItem("$callback", "foo");
            var result = CallbackQueryOptionHandler.HandleCallbackQueryOption(new AstoriaRequestMessage(host), new ODataFormatWithParameters(ODataFormat.Json));
            result.Should().Be("foo");
        }

        [TestMethod]
        public void CallbackQueryOptionShouldWorkIfRaw()
        {
            var host = new DataServiceHostSimulator { RequestHttpMethod = "GET" };
            host.SetQueryStringItem("$callback", "foo");
            var result = CallbackQueryOptionHandler.HandleCallbackQueryOption(new AstoriaRequestMessage(host), new ODataFormatWithParameters(ODataFormat.RawValue));
            result.Should().Be("foo");
        }

        [TestMethod]
        public void IgnoresOtherStuffInHost()
        {
            var host = new DataServiceHostSimulator { RequestHttpMethod = "GET" };
            host.SetQueryStringItem("$callback", "foo");
            host.SetQueryStringItem("$format", "atom");
            var result = CallbackQueryOptionHandler.HandleCallbackQueryOption(new AstoriaRequestMessage(host), new ODataFormatWithParameters(ODataFormat.Json));
            result.Should().Be("foo");
        }

        [TestMethod]
        public void FailIfMethodIsNotGet()
        {
            var host = new DataServiceHostSimulator { RequestHttpMethod = "POST" };
            host.SetQueryStringItem("$callback", "foo");
            Action method = () => CallbackQueryOptionHandler.HandleCallbackQueryOption(new AstoriaRequestMessage(host), new ODataFormatWithParameters(ODataFormat.Json));
            method.ShouldThrow<DataServiceException>().WithMessage(ds.Strings.CallbackQueryOptionHandler_GetRequestsOnly);
        }

        [TestMethod]
        public void FailIfContentTypeIsBatch()
        {
            var host = new DataServiceHostSimulator { RequestHttpMethod = "GET" };
            host.SetQueryStringItem("$callback", "foo");
            Action method = () => CallbackQueryOptionHandler.HandleCallbackQueryOption(new AstoriaRequestMessage(host), new ODataFormatWithParameters(ODataFormat.Batch));
            method.ShouldThrow<DataServiceException>().WithMessage(ds.Strings.CallbackQueryOptionHandler_UnsupportedContentType(ODataFormat.Batch));
        }

        [TestMethod]
        public void FailIfContentTypeIsMetadata()
        {
            var host = new DataServiceHostSimulator { RequestHttpMethod = "GET" };
            host.SetQueryStringItem("$callback", "foo");
            Action method = () => CallbackQueryOptionHandler.HandleCallbackQueryOption(new AstoriaRequestMessage(host), new ODataFormatWithParameters(ODataFormat.Metadata));
            method.ShouldThrow<DataServiceException>().WithMessage(ds.Strings.CallbackQueryOptionHandler_UnsupportedContentType(ODataFormat.Metadata));
        }
    }
}
