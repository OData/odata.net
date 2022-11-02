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

        public HttpResponse ServeRequest(HttpRequest request)
        {
            throw new NotImplementedException();
        }

        public string ServeRequest(string request)
        {
            throw new NotImplementedException();
        }
    }
}
