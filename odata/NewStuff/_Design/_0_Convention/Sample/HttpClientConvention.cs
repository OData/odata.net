namespace NewStuff._Design._0_Convention.Sample
{
    using System.Net.Http;

    public sealed class HttpClientConvention : IConvention
    {
        private readonly HttpClient httpClient;

        public HttpClientConvention(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public IGetRequestWriter Get()
        {
            throw new System.NotImplementedException();
        }

        public IPatchRequestWriter Patch()
        {
            throw new System.NotImplementedException();
        }

        public IPatchRequestWriter Post()
        {
            throw new System.NotImplementedException();
        }
    }
}
