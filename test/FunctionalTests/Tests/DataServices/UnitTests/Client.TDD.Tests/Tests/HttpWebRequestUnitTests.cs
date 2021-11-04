//---------------------------------------------------------------------
// <copyright file="HttpWebRequestUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using Microsoft.OData.Client;
    using System.Net;
    using FluentAssertions;
    using Xunit;
    using System.Net.Http;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Unit tests for the HttpWebRequest class. Primarily used for testing differences accoss all the platforms
    /// </summary>
    public class HttpWebRequestMessageUnitTests
    {
        [Fact]
        public void SetUserAgentShouldSucceed()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "http://www.svc");
            IDictionary<string, string> Headers = new Dictionary<string, string>();
            DataServiceClientRequestMessageArgs args = new DataServiceClientRequestMessageArgs(request.Method.ToString(), request.RequestUri, true, false, Headers);
            new HttpClientRequestMessage(args).SetHeader(XmlConstants.HttpUserAgent, "MyUserAgent");
            new HttpWebRequestMessage(args).SetHeader(XmlConstants.HttpUserAgent, "MyUserAgent");
        }

        [Fact]
        public void SetAcceptCharsetShouldNotBeSetOnSilverlightAndSetOnOtherPlatforms()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "http://www.svc");
            IDictionary<string, string> Headers = new Dictionary<string, string>();
            DataServiceClientRequestMessageArgs args = new DataServiceClientRequestMessageArgs(request.Method.ToString(), request.RequestUri, true, false, Headers);
            new HttpClientRequestMessage(args).SetHeader(XmlConstants.HttpAcceptCharset, "utf8");
            new HttpWebRequestMessage(args).SetHeader(XmlConstants.HttpAcceptCharset, "utf8");
        }

        [Fact]
        public void SetContentLengthShouldSucceed()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "http://www.svc");
            IDictionary<string, string> Headers = new Dictionary<string, string>();
            DataServiceClientRequestMessageArgs args = new DataServiceClientRequestMessageArgs(request.Method.ToString(), request.RequestUri, true, false, Headers);
            new HttpClientRequestMessage(args).SetHeader(XmlConstants.HttpContentLength, 1.ToString(CultureInfo.InvariantCulture.NumberFormat));
            new HttpWebRequestMessage(args).SetHeader(XmlConstants.HttpContentLength, 1.ToString(CultureInfo.InvariantCulture.NumberFormat));
        }
    }
}
