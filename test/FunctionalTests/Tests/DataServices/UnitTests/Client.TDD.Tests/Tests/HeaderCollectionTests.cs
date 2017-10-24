//---------------------------------------------------------------------
// <copyright file="HeaderCollectionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using FluentAssertions;
    using Microsoft.OData.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HeaderCollectionTests
    {
        [TestMethod]
        public void HeaderDictionaryKeysShouldBeUnchanged()
        {
            var expectedKeys = GetInterestingHeaderNames().Select(InvertCase).ToList();

            var headers = new WebHeaderCollection();
            foreach (var header in expectedKeys)
            {
                headers[header] = "some value";
            }

            var dictionary = new HeaderCollection(headers);
#if (NETCOREAPP1_0 || NETCOREAPP2_0)
            // Implementation of BeEquivalentTo changed in newer version of FluentAssertions so use Contain
            dictionary.HeaderNames.Should().Contain(expectedKeys);
#else
            dictionary.HeaderNames.Should().BeEquivalentTo(expectedKeys);
#endif
        }

        [TestMethod]
        public void HeaderDictionaryLookupsShouldBeCaseInsensitive()
        {
            WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
            webHeaderCollection["CONTENT-TYPE"] = "something";
            var dictionary = new HeaderCollection(webHeaderCollection);
            dictionary.GetHeader("Content-Type").Should().Be("something");
            dictionary.GetHeader("content-type").Should().Be("something");
        }

        [TestMethod]
        public void ResponseMessageShouldBeCaseInsensitive()
        {
            var dictionary = new Dictionary<string, string> { { "content-type", "some value" } };
            var message = new HttpWebResponseMessage(dictionary, 201, () => { throw new Exception(); });
            message.GetHeader("Content-Type").Should().Be("some value");
            message.GetHeader("CONTENT-TYPE").Should().Be("some value");
        }

        [TestMethod]
        public void ResponseMessageShouldSupportSettingHeaders()
        {
            var message = new HttpWebResponseMessage(new Dictionary<string, string>(), 201, () => { throw new Exception(); });
            Action setHeader = () => message.SetHeader("Content-Type", "some value");
            setHeader.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void RequestMessageShouldBeCaseInsensitiveForContentType()
        {
            TestRequestHeaderRoundTrip("cOnTenT-TyPe", "CoNtEnT-tYpE", "some value");
        }

        [TestMethod]
        public void RequestMessageShouldBeCaseInsensitiveForAccept()
        {
            TestRequestHeaderRoundTrip("accept", "ACCEPT", "some value");
        }

        [TestMethod]
        public void RequestMessageShouldBeCaseInsensitiveForContentLength()
        {
            TestRequestHeaderRoundTrip("conTenT-LENGTH", "CONTENT-lEngTh", "1234");
        }

        [TestMethod]
        public void RequestMessageShouldBeCaseInsensitiveForODataVersion()
        {
            TestRequestHeaderRoundTrip("odata-VERSION", "ODATA-version", "1.0");
        }

        [TestMethod]
        public void ChangingCopyOfHeaderCollectionShouldNotChangeOriginal()
        {
            var original = new HeaderCollection();
            var copy = original.Copy();
            copy.SetHeader("Fake", "Value");
            copy.HeaderNames.Should().Contain("Fake");
            original.HeaderNames.Should().BeEmpty();
        }

        [TestMethod]
        public void SetDefaultHeadersShouldSetUserAgent()
        {
            var headers = new HeaderCollection();
            headers.SetDefaultHeaders();
            headers.HeaderNames.Should().Contain("User-Agent");
        }

        [TestMethod]
        public void SetUseAgentShouldSetCorrectValue()
        {
            var headers = new HeaderCollection();
            headers.SetUserAgent();
            headers.HeaderNames.Count().Should().Be(1);

            Version assemblyVersion = typeof(HeaderCollection).GetAssembly().GetName().Version;
            headers.GetHeader("User-Agent").Should().Be(string.Format(CultureInfo.InvariantCulture, "Microsoft.OData.Client/{0}.{1}.{2}", assemblyVersion.Major, assemblyVersion.Minor, assemblyVersion.Build));
        }

        [TestMethod]
        public void SetRequestVersionShouldSetCorrectValue()
        {
            var headers = new HeaderCollection();
            headers.SetRequestVersion(new Version(4, 0, 0, 1), new Version(5, 0, 0, 1));
            headers.GetHeader("OData-Version").Should().Be("4.0");
            headers.GetHeader("OData-MaxVersion").Should().Be("5.0");
        }

        [TestMethod]
        public void SetRequestVersionShouldSetCorrectValueWhenHeaderCollectionContainsODataVersionLessThan40()
        {
            var headers = new HeaderCollection();
            headers.SetHeader("OData-Version", "3.0.0");
            headers.SetRequestVersion(new Version(4, 0, 0, 1), new Version(5, 0, 0, 1));
            headers.GetHeader("OData-Version").Should().Be("4.0");
            headers.GetHeader("OData-MaxVersion").Should().Be("5.0");
        }

        [TestMethod]
        public void SetRequestVersionShouldSetCorrectValueWhenHeaderCollectionContainsODataVersionGreaterThan40()
        {
            var headers = new HeaderCollection();
            headers.SetHeader("OData-Version", "5.0.0");
            headers.SetHeader("OData-MaxVersion", "5.0.0");
            headers.SetRequestVersion(new Version(4, 0, 0, 1), new Version(5, 0, 0, 1));
            headers.GetHeader("OData-Version").Should().Be("5.0.0");
            headers.GetHeader("OData-MaxVersion").Should().Be("5.0");
        }

        private static void TestRequestHeaderRoundTrip(string headerToSet, string headerToGet, string value)
        {
            var requestMessage = CreateRequestMessage();
            requestMessage.SetHeader(headerToSet, value);
            requestMessage.GetHeader(headerToGet).Should().Be(value);
        }

        private static HttpWebRequestMessage CreateRequestMessage()
        {
            return new ExtendedHttpWebRequestMessage(new DataServiceClientRequestMessageArgs("GET", new Uri("http://temp.org"), false, false, new Dictionary<string, string>()));
        }

        private static IEnumerable<string> GetInterestingHeaderNames()
        {
            var interestingHeaders = new List<string>();
            foreach (var enumHeader in Enum.GetValues(typeof(HttpResponseHeader)).Cast<HttpResponseHeader>())
            {
                var temporaryCollection = new WebHeaderCollection();
                temporaryCollection[enumHeader] = "some value";
                interestingHeaders.Add(temporaryCollection.AllKeys.Single());
            }

            interestingHeaders.Add("ETag");
            interestingHeaders.Add("OData-Version");
            return interestingHeaders;
        }

        private static string InvertCase(string s)
        {
            return new string(s.ToCharArray().Select(InvertCase).ToArray());
        }

        private static char InvertCase(char c)
        {
            return char.IsUpper(c) ? char.ToLower(c) : char.ToUpper(c);
        }

        public class ExtendedHttpWebRequestMessage : HttpWebRequestMessage
        {
            public ExtendedHttpWebRequestMessage(DataServiceClientRequestMessageArgs args) : base(args)
            {
            }
        }
    }
}