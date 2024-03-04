using Microsoft.OData.Client;
using System.Net;
using System.Net.Http;

namespace Microsoft.Test.Taupo.Astoria.Client
{
    internal class TestHttpClientProvider : IHttpClientProvider
    {
        private readonly HttpClient _client;
        public TestHttpClientProvider(TestHttpClientProviderOptions options)
        {
            var handler = new HttpClientHandler();
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
