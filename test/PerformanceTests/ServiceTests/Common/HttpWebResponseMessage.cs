//---------------------------------------------------------------------
// <copyright file="HttpWebResponseMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.OData.Core;

    public class HttpWebResponseMessage : IODataResponseMessageAsync
    {
        private HttpWebResponse response;
        private bool lockedHeaders = false;

        public HttpWebResponseMessage(HttpWebResponse response)
        {
            this.response = response;
        }

        public string GetHeader(string headerName)
        {
            return response.Headers[headerName];
        }

        public Task<Stream> GetStreamAsync()
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
            {
                throw new InvalidOperationException("Cannot set headers they have already been written to the stream");
            }

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
