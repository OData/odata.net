//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Data.OData;

namespace Microsoft.Data.OData.Samples.Messages
{
    class HTTPClientRequestMessage : IODataRequestMessageAsync
    {
        private readonly HttpWebRequest request;
        private bool lockedHeaders = false;

        public HTTPClientRequestMessage(string uri)
        {
            request = (HttpWebRequest)WebRequest.Create(new Uri(uri));
        }

        public string GetHeader(string headerName)
        {
            return request.Headers.Get(headerName);
        }

        public System.Threading.Tasks.Task<System.IO.Stream> GetStreamAsync()
        {
            lockedHeaders = true;

            TaskCompletionSource<Stream> completionSource = new TaskCompletionSource<Stream>();
            completionSource.SetResult(request.GetRequestStream());
            return completionSource.Task;
        }

        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get {
                throw new NotSupportedException();
            }
        }

        public void SetHeader(string headerName, string headerValue)
        {
            if (lockedHeaders)
                throw new ODataException("Cannot set headers they have already been written to the stream");

            if (headerName == "Content-Type")
                request.ContentType = headerValue;
            else if (headerName == "Accept")
                request.Accept = headerValue;
            else
                request.Headers.Add(headerName, headerValue);
        }

        public IODataResponseMessage GetResponse()
        {
            WebResponse res;
            try
            {
                res = request.GetResponse();
            }
            catch (WebException ex)
            {
                res = ex.Response;
            }

            return new HTTPClientResponseMessage((HttpWebResponse)res);
        }
        
        public Stream GetStream()
        {
            return request.GetRequestStream();
        }

        public Uri Url
        {
            get
            {
                return request.RequestUri;
            }
            set
            {
                throw new ArgumentException("Request Uri cannot be changed");
            }
        }


        public HttpMethod Method
        {
            get
            {
                switch (request.Method)
                {
                    case "GET":
                        return HttpMethod.Get;
                    case "PUT":
                        return HttpMethod.Put;
                    case "PATCH":
                        return HttpMethod.Patch;
                    case "POST":
                        return HttpMethod.Post;
                    case "DELETE":
                        return HttpMethod.Delete;
                    case "MERGE":
                        return HttpMethod.Merge;
                    default:
                        return HttpMethod.Get;
                }
            }
            set
            {
                switch (value)
                {
                    case HttpMethod.Get:
                        request.Method = "GET";
                        break;
                    case HttpMethod.Delete:
                        request.Method = "DELETE";
                        break;
                    case HttpMethod.Merge:
                        request.Method = "MERGE";
                        break;
                    case HttpMethod.Patch:
                        request.Method = "PATCH";
                        break;
                    case HttpMethod.Post:
                        request.Method = "POST";
                        break;
                    case HttpMethod.Put:
                        request.Method = "PUT";
                        break;
                }
            }
        }
    }
}
