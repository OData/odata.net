namespace pge.OData
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    public interface IOData
    {
        //// TODO remove this? and all of the now-dead code?
        //// TODO maybe this is help for doing http/2? factory might need a parameter for http version if this is the case
        HttpResponse ServeRequest(HttpRequest request);

        string ServeRequest(string request);
    }
}
