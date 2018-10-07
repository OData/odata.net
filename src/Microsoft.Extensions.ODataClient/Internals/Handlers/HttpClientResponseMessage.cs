using Microsoft.Extensions.ODataClient.Internals.Handlers;
using Microsoft.OData;
using Microsoft.OData.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Microsoft.Extensions.ODataClient
{
    internal class HttpClientResponseMessage : HttpWebResponseMessage, IODataResponseMessage
    {
        public HttpClientResponseMessage(HttpResponseMessage httpResponse, DataServiceClientConfigurations config)
            : base(httpResponse.ToStringDictionary(),
                  (int)httpResponse.StatusCode,
                  () => { var task = httpResponse.Content.ReadAsStreamAsync(); task.Wait(); return task.Result; })
        {
            // merge properties back if populated
            var properties = httpResponse.RequestMessage?.Properties;
            if (properties != null)
            {
                foreach (var item in properties)
                {
                    config.Properties[item.Key] = item.Value;
                }
            }
        }
    }
}
