namespace Microsoft.OData.OData
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.HttpServer;

    public static class ODataExtensions
    {
        public static async Task<HttpServerResponse> HandleRequest(this IOData source, HttpServerRequest request)
        {
            if (string.Equals(request.HttpMethod, "GET", System.StringComparison.OrdinalIgnoreCase))
            {
                using (var streamReader = new StreamReader(request.Body))
                {
                    var odataResponse = await source.GetAsync(request.Url, request.Headers, await streamReader.ReadToEndAsync());
                    return new HttpServerResponse()
                    {
                        StatusCode = 208, //// TODO
                        Headers = new[] { "gdebruin: did this also work?" }, //// TODO
                        Body = odataResponse,
                    };
                }
            }

            throw new InvalidOperationException("TODO");
        }
    }
}
