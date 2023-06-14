namespace Microsoft.OData.ODataService
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// An <see cref="IODataService"/> implementation that leverages HTTP to make requests to the backing service
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    public sealed class HttpOData : IODataService
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
                    while (!string.IsNullOrEmpty(line = await streamReader.ReadLineAsync().ConfigureAwait(false)))
                    {
                        var header = line.Split(':', 2);
                        if (header.Length < 1)
                        {
                            throw new InvalidOperationException("TODO document this in the interface");
                        }

                        httpClient.DefaultRequestHeaders.Add(header[0], header.ElementAtOrDefault(1, string.Empty));
                    }

                    //// TODO need to return response code
                    using (var httpResponse = await httpClient.GetAsync(url).ConfigureAwait(false))
                    {
                        return await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
