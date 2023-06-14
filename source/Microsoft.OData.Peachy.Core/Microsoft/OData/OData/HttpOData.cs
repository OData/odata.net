namespace Microsoft.OData.OData
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// An <see cref="IOData"/> implementation that leverages HTTP to make requests to the backing service
    /// </summary>
    public sealed class HttpOData : IOData
    {
        private readonly Func<HttpClient> httpClientFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <exception cref="ArgumentNullException">httpclientfactory</exception>
        public HttpOData(Func<HttpClient> httpClientFactory)
        {
            if (httpClientFactory == null)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }

            this.httpClientFactory = httpClientFactory;
        }

        public async Task<Stream> GetAsync(string url, Stream request)
        {
            using (var httpClient = this.httpClientFactory())
            {
                using (var streamReader = new StreamReader(request))
                {
                    string? line;
                    while (!string.IsNullOrEmpty(line = await streamReader.ReadLineAsync()))
                    {

                    }
                }
            }
        }
    }
}
