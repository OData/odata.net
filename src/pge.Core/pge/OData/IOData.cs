namespace pge.OData
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    public interface IOData
    {
        //// TODO remove this? and all of the now-dead code?
        HttpResponse ServeRequest(HttpRequest request);

        string ServeRequest(string request);
    }
}
