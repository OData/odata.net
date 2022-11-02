namespace pge.OData
{
    using System;
    using System.Net;

    public sealed class HttpStatus
    {
        public Version Version { get; }

        public HttpStatusCode StatusCode { get; }

        public string StatusText { get; }
    }
}
