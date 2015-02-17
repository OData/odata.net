//---------------------------------------------------------------------
// <copyright file="WcfQueryStringHandlerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using System.Collections.Specialized;
    using Microsoft.OData.Service;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the WCF host implementation. Note that there are some tests that may be appropriate to move here in the
    /// old HttpContextServiceHostTest.cs file, which is bound to the old AstoriaTestFramework.
    /// </summary>
    [TestClass]
    public class WcfQueryStringHandlerTests
    {
        [TestMethod]
        public void GetMissingKeyShouldReturnNull()
        {
            HttpContextServiceHost.GetQueryStringItem("missing_item", new NameValueCollection()).Should().Be(null);
        }

        [TestMethod]
        public void GetPresentKeyShouldReturnItsValue()
        {
            var collection = new NameValueCollection();
            collection.Add("somekey", "thevalue");
            HttpContextServiceHost.GetQueryStringItem("somekey", collection).Should().Be("thevalue");
        }

        [TestMethod]
        public void GetWillTrimExcessWhiteSpaceOffKey()
        {
            var collection = new NameValueCollection();
            collection.Add("\tsomekey  \n\r", "thevalue");
            HttpContextServiceHost.GetQueryStringItem("somekey", collection).Should().Be("thevalue");
        }

        [TestMethod]
        public void GetDoesNotCareAboutSpecialDollarCharacter()
        {
            var collection = new NameValueCollection();
            collection.Add("$isreserved$butwedontcarehere", "thevalue");
            HttpContextServiceHost.GetQueryStringItem("$isreserved$butwedontcarehere", collection).Should().Be("thevalue");
        }

        [TestMethod]
        public void GetWorksForValuesWithWhitespace()
        {
            var collection = new NameValueCollection();
            collection.Add("somekey", "the value\t with WHITESPACE");
            HttpContextServiceHost.GetQueryStringItem("somekey", collection).Should().Be("the value\t with WHITESPACE");
        }

        [TestMethod]
        public void VerifyAllowsWellKnownDollarQueryOptions()
        {
            var collection = new NameValueCollection();
            collection.Add("$select", "value");
            collection.Add("$filter", "value");
            collection.Add("$top", "value");
            collection.Add("$skip", "value");
            collection.Add("$expand", "value");
            collection.Add("$orderby", "value");
            collection.Add("$count", "value");
            collection.Add("$format", "value");
            collection.Add("$callback", "value");
            Action action = () => HttpContextServiceHost.VerifyQueryParameters(collection);
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void VerifyDoesNotAllowControlInfo()
        {
            // $controlinfo was briefly used for controlling how much metadata a client wanted
            // in JSON-Light payloads. It was removed and replaced with a parameter in the media type.
            var collection = new NameValueCollection();
            collection.Add("$controlinfo", "value");
            Action action = () => HttpContextServiceHost.VerifyQueryParameters(collection);
            action.ShouldThrow<DataServiceException>().WithMessage("The query parameter '$controlinfo' begins with a system-reserved '$' character but is not recognized.");
        }

        [TestMethod]
        public void VerifyDoesNotAllowDollarLinks()
        {
            var collection = new NameValueCollection();
            collection.Add("$ref", "value");
            Action action = () => HttpContextServiceHost.VerifyQueryParameters(collection);
            action.ShouldThrow<DataServiceException>().WithMessage("The query parameter '$ref' begins with a system-reserved '$' character but is not recognized.");
        }

        [TestMethod]
        public void VerifyDoesNotAllowDollarCount()
        {
            var collection = new NameValueCollection();
            collection.Add("$inlinecount", "value");
            Action action = () => HttpContextServiceHost.VerifyQueryParameters(collection);
            action.ShouldThrow<DataServiceException>().WithMessage("The query parameter '$inlinecount' begins with a system-reserved '$' character but is not recognized.");
        }

        [TestMethod]
        public void VerifyDoesNotAllowDollarValue()
        {
            var collection = new NameValueCollection();
            collection.Add("$value", "value");
            Action action = () => HttpContextServiceHost.VerifyQueryParameters(collection);
            action.ShouldThrow<DataServiceException>().WithMessage("The query parameter '$value' begins with a system-reserved '$' character but is not recognized.");
        }

        [TestMethod]
        public void VerifyDoesNotAllowDollarUnknown()
        {
            var collection = new NameValueCollection();
            collection.Add("$unknown", "value");
            Action action = () => HttpContextServiceHost.VerifyQueryParameters(collection);
            action.ShouldThrow<DataServiceException>().WithMessage("The query parameter '$unknown' begins with a system-reserved '$' character but is not recognized.");
        }

        [TestMethod]
        public void VerifyDoesNotAllowRepeatSelect()
        {
            var collection = new NameValueCollection();
            collection.Add("$select", "value");
            collection.Add("$select", "value");
            Action action = () => HttpContextServiceHost.VerifyQueryParameters(collection);
            action.ShouldThrow<DataServiceException>().WithMessage("Query parameter '$select' is specified, but it should be specified exactly once.");
        }

        [TestMethod]
        public void VerifyDoesNotAllowRepeatFormat()
        {
            var collection = new NameValueCollection();
            collection.Add("$format", "json");
            collection.Add("$format", "atom");
            Action action = () => HttpContextServiceHost.VerifyQueryParameters(collection);
            action.ShouldThrow<DataServiceException>().WithMessage("Query parameter '$format' is specified, but it should be specified exactly once.");
        }
    }
}
