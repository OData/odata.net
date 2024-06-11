//---------------------------------------------------------------------
// <copyright file="ODataMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OData;

namespace ExperimentsLib
{
    /// <summary>
    /// Implementation of <see cref="IODataResponseMessageAsync"/> used to pass
    /// the response message to the <see cref="ODataMessageWriter"/>.
    /// </summary>
    public class ODataMessage : IODataResponseMessageAsync, IServiceCollectionProvider, IDisposable
    {
        private readonly Dictionary<string, string> headers;

        public ODataMessage()
        {
            this.headers = new Dictionary<string, string>();
        }

        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { return this.headers; }
        }

        public int StatusCode { get; set; }

        public Uri Url { get; set; }

        public string Method { get; set; }

        public Stream Stream { get; set; }

        public IServiceProvider ServiceProvider { get; set; }

        public string GetHeader(string headerName)
        {
            return this.headers.TryGetValue(headerName, out string headerValue) ? headerValue : null;
        }

        public void SetHeader(string headerName, string headerValue)
        {
            this.headers[headerName] = headerValue;
        }

        public Stream GetStream()
        {
            return this.Stream;
        }

        public Action DisposeAction { get; set; }

        void IDisposable.Dispose()
        {
            if (this.DisposeAction != null)
            {
                this.DisposeAction();
            }
        }

        public Task<Stream> GetStreamAsync()
        {
            return Task.FromResult(Stream);
        }
    }
}
