namespace pge.OData
{
    using System;
    using System.Net.Http;

    public sealed class HttpStart
    {
        public HttpMethod Method { get; }

        public Uri Target { get; }

        public Version Version { get; }
    }
}
