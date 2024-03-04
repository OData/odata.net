using Microsoft.OData.Client;
using System.Net;
using System.Net.Http;

namespace AstoriaTestFramework.Client.CLWrappers
{
    internal class TestHttpClientProvider : IHttpClientProvider
    {
        readonly HttpClient _client;
        public TestHttpClientProvider(TestHttpClientProviderOptions options)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.Credentials = options.Credentials;
            _client = new HttpClient(handler);
        }

        public HttpClient GetHttpClient()
        {
            return _client;
        }
    }

    internal class TestHttpClientProviderOptions
    {
        public ICredentials Credentials { get; set; }
    }
}
