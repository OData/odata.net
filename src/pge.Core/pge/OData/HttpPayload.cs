namespace pge.OData
{
    using System.Collections.Generic;

    public sealed class HttpPayload
    {
        public IEnumerable<string> Headers { get; }

        public string Body { get; }
    }
}
