namespace Microsoft.Epm
{
    using System.Collections.Generic;
    using System.IO;

    public sealed class HttpServerRequest
    {
        public string HttpMethod { get; set; } //// TODO no setters

        public IEnumerable<string> Headers { get; set; }

        public Stream Body { get; set; }
    }
}
