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

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.OData;

namespace Microsoft.Data.OData.Samples.Messages
{

    public class FileResponseMessage : IODataResponseMessageAsync
    {
        private readonly Stream stream;
        private IDictionary<string, string> headers;
        private int statusCode;
        private bool lockedHeaders = false;

        public FileResponseMessage(Stream stream)
        {
            this.stream = stream;
            this.headers = new Dictionary<string, string>();
        }

        public string GetHeader(string headerName)
        {
            string value;
            headers.TryGetValue(headerName, out value);
            return value;
        }

        public Task<Stream> GetStreamAsync()
        {
            StreamWriter sw = new StreamWriter(stream);
            sw.WriteLine("HTTP/1.1 " + this.StatusCode.ToString());
            lockedHeaders = true;
            foreach (var h in headers)
            {
                sw.WriteLine(h.Key + ": " + h.Value);
            }
            sw.WriteLine();
            sw.Flush();


            TaskCompletionSource<Stream> completionSource = new TaskCompletionSource<Stream>();
            completionSource.SetResult(this.stream);
            return completionSource.Task;
        }

        #region IODataResponseMessage Members


        public void SetHeader(string headerName, string headerValue)
        {
            if (lockedHeaders)
                throw new ODataException("Cannot set headers they have already been written to the stream");

            this.headers[headerName] = headerValue;
        }


        #endregion


        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                return this.headers;
            }
        }

        public int StatusCode
        {
            get
            {
                return statusCode;
            }
            set
            {
                statusCode = value;
            }
        }


        public Stream GetStream()
        {
            return stream;
        }
    }

}
