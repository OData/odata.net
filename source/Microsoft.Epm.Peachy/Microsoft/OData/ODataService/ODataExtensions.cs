namespace Microsoft.OData.ODataService
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.HttpServer;

    public static class ODataExtensions
    {
        public static async Task<HttpServerResponse> HandleRequestAsync(this IODataService source, HttpServerRequest request)
        {
            if (string.Equals(request.HttpMethod, "GET", System.StringComparison.OrdinalIgnoreCase))
            {
                using (var streamReader = new StreamReader(request.Body))
                {
                    var odataResponse = await source.GetAsync(request.Url, request.Headers, await streamReader.ReadToEndAsync());
                    return new HttpServerResponse()
                    {
                        StatusCode = odataResponse.StatusCode,
                        Headers = odataResponse.Headers,
                        Body = odataResponse.Body,
                    };
                }
            }

            throw new InvalidOperationException("TODO");
        }
    }
}
