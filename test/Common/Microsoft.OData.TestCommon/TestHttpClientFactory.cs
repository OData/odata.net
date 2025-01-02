//---------------------------------------------------------------------
// <copyright file="TestHttpClientFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Net.Http;
using System.Net;

namespace Microsoft.OData.TestCommon
{
    public class TestHttpClientFactory : IHttpClientFactory
    {
        readonly HttpClient _client;
        public TestHttpClientFactory(TestHttpClientFactoryOptions options)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.Credentials = options.Credentials;
            _client = new HttpClient(handler);
        }

        public HttpClient CreateClient(string name)
        {
            return _client;
        }
    }

    public class TestHttpClientFactoryOptions
    {
        public ICredentials Credentials { get; set; }
    }
}
