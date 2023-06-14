namespace Microsoft.HttpServer
{
    using System.Threading.Tasks;

    public delegate Task<HttpServerResponse> HttpRequestHandler(HttpServerRequest request);

    public interface IHttpServer
    {
        Task ListenAsync();
    }
}
