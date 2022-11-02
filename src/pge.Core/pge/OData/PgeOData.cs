namespace pge.OData
{
    using System;
    using System.Net.Http;

    public sealed class PgeOData : IOData
    {
        public HttpPayload ServeRequest(Uri requestUri, HttpMethod httpMethod, HttpPayload requestPayload)
        {
            throw new NotImplementedException();
        }
    }
}
