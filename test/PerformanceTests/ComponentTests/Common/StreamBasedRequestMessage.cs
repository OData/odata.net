//---------------------------------------------------------------------
// <copyright file="StreamBasedRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.OData.Core;

    /// <summary>
    /// An OData Request Message backed by a Stream.
    /// </summary>
    public class StreamBasedRequestMessage : IODataRequestMessageAsync
    {
        private readonly Stream _stream;
        private readonly IDictionary<string, string> _headers;
        private const bool LockedHeaders = false;

        public StreamBasedRequestMessage(Stream stream)
        {
            _stream = stream;
            _headers = new Dictionary<string, string>();
        }

        public Task<Stream> GetStreamAsync()
        {
            var completionSource = new TaskCompletionSource<Stream>();
            completionSource.SetResult(_stream);
            return completionSource.Task;
        }

        public Stream GetStream()
        {
            return _stream;
        }

        public void SetHeader(string headerName, string headerValue)
        {
            if (LockedHeaders)
            {
                throw new ODataException("Cannot set headers they have already been written to the stream");
            }

            _headers[headerName] = headerValue;
        }

        public string GetHeader(string headerName)
        {
            string value;
            _headers.TryGetValue(headerName, out value);
            return value;
        }

        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                return this._headers;
            }
        }

        public string Method
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Uri Url
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
