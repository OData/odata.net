namespace Microsoft.HttpServer
{
    using System.Collections.Generic;
    using System.IO;

    public sealed class HttpServerResponse
    {
        public int StatusCode { get; set; }

        public IEnumerable<string> Headers { get; set; } //// TODO no setters

        public Stream Body { get; set; }
    }
}
