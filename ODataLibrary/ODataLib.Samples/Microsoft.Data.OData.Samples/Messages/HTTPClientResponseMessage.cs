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
    class HTTPClientResponseMessage : IODataResponseMessageAsync
    {
        private HttpWebResponse response;
        private bool lockedHeaders = false;

        public HTTPClientResponseMessage(HttpWebResponse res)
        {
            response = res;

        }


        public string GetHeader(string headerName)
        {
            return response.Headers[headerName];
        }

        public System.Threading.Tasks.Task<System.IO.Stream> GetStreamAsync()
        {
            lockedHeaders = true;
            TaskCompletionSource<Stream> completionSource = new TaskCompletionSource<Stream>();
            completionSource.SetResult(response.GetResponseStream());
            return completionSource.Task;
        }

        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                foreach (string headerName in this.response.Headers.Keys)
                {
                    yield return new KeyValuePair<string, string>(headerName, this.response.Headers[headerName]);
                }
            }
        }

        public void SetHeader(string headerName, string headerValue)
        {
            if (lockedHeaders)
                throw new ODataException("Cannot set headers they have already been written to the stream");

            this.response.Headers[headerName] = headerValue;
        }

        public int StatusCode
        {
            get
            {
                return (int)response.StatusCode;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string StatusDescription
        {
            get
            {
                return response.StatusDescription;
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public Stream GetStream()
        {
            return response.GetResponseStream();
        }
    }
}
