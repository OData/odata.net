using Microsoft.OData;
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
    internal class HttpClientResponseMessage : IODataResponseMessage
    {
        private readonly IDictionary<string, string> headers;
        private readonly Func<Stream> getResponseStream;
        private readonly int statusCode;

#if DEBUG
        /// <summary>set to true once the GetStream was called.</summary>
        private bool streamReturned;
#endif

        public HttpClientResponseMessage(HttpResponseMessage httpResponse)
        {
            this.headers = HttpHeadersToStringDictionary(httpResponse.Headers);
            this.statusCode = (int)httpResponse.StatusCode;
            this.getResponseStream = () => { var task = httpResponse.Content.ReadAsStreamAsync(); task.Wait(); return task.Result; };
        }

        private static IDictionary<string, string> HttpHeadersToStringDictionary(HttpHeaders headers)
        {
            return headers.ToDictionary((h1) => h1.Key, (h2) => string.Join(",", h2.Value));
        }

        /// <summary>
        /// Returns the collection of response headers.
        /// </summary>
        public virtual IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { return this.headers; }
        }

        /// <summary>
        /// The response status code.
        /// </summary>
        public virtual int StatusCode
        {
            get { return this.statusCode; }

            set { throw new NotSupportedException(); }
        }

        public virtual string GetHeader(string headerName)
        {
            string result;
            if (this.headers.TryGetValue(headerName, out result))
            {
                return result;
            }

            // Since the unintialized value of ContentLength header is -1, we need to return
            // -1 if the content length header is not present
            if (string.Equals(headerName, "Content-Length", StringComparison.Ordinal))
            {
                return "-1";
            }

            return null;
        }

        public virtual void SetHeader(string headerName, string headerValue)
        {
            if (String.IsNullOrEmpty(headerValue))
            {
                return;
            }
            if (this.headers.ContainsKey(headerName))
            {
                this.headers[headerName] = headerValue;
            }
            else
            {
                this.headers.Add(headerName, headerValue);
            }
        }

        public virtual Stream GetStream()
        {
#if DEBUG
            Debug.Assert(!this.streamReturned, "The GetStream can only be called once.");
            this.streamReturned = true;
#endif

            return this.getResponseStream();
        }
    }
}
