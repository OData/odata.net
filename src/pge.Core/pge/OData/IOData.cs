namespace pge.OData
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    public interface IOData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="httpMethod"></param>
        /// <param name="requestPayload"></param>
        /// <returns>The response <see cref="HttpPayload"/></returns>
        HttpPayload ServeRequest(Uri requestUri, HttpMethod httpMethod, HttpPayload requestPayload);
    }
}
