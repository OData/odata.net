using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Microsoft.Extensions.ODataClient.Internals.Handlers
{
    internal static class HttpHeadersExtensions
    {
        public static IDictionary<string, string> ToStringDictionary(this HttpHeaders headers)
        {
            return headers.ToDictionary((h1) => h1.Key, (h2) => string.Join(",", h2.Value));
        }

        public static IDictionary<string, string> ToStringDictionary(this HttpResponseMessage message)
        {
            if (message.Content != null)
            {
                var dic = message.Headers.ToStringDictionary();
                foreach (var item in message.Content.Headers.ToStringDictionary())
                {
                    dic[item.Key] = item.Value;
                }

                return dic;
            }

            return message.Headers.ToStringDictionary();
        }
        
    }
}
