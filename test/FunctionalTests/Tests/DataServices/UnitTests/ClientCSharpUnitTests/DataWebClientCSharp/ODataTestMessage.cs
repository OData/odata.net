//---------------------------------------------------------------------
// <copyright file="ODataTestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.OData;

    /// <summary>
    /// Used for short integration client tests
    /// </summary>
    public class ODataTestMessage : IODataRequestMessage, IODataResponseMessage
    {
        private readonly Dictionary<string, string> headers;
        public ODataTestMessage()
        {
            // intentionally use a case-sensitive comparer here so that we know that the client consistently ASKS for headers with canonical casing.
            this.headers = new Dictionary<string, string>(StringComparer.Ordinal);
            this.MemoryStream = new NonDisposableStream();
        }

        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { return headers.Select(pair => new KeyValuePair<string, string>(char.IsUpper(pair.Key[0]) ? pair.Key.ToLowerInvariant() : pair.Key.ToUpperInvariant(), pair.Value)); }
        }

        public Uri Url { get; set; }

        public string Method { get; set; }

        public int StatusCode { get; set; }

        public MemoryStream MemoryStream { get; set; }

        public string GetHeader(string headerName)
        {
            string headerValue;
            this.headers.TryGetValue(headerName, out headerValue);
            return headerValue;
        }

        public void SetHeader(string headerName, string headerValue)
        {
            headers[headerName] = headerValue;
        }

        public Stream GetStream()
        {
            return this.MemoryStream;
        }

        public void WriteToStream(string payload)
        {
            StreamWriter writer = new StreamWriter(this.MemoryStream);
            writer.Write(payload);
            writer.Flush();
            this.MemoryStream.Position = 0;
        }

        public string GetRequestStreamAsText()
        {
            this.MemoryStream.Seek(0, SeekOrigin.Begin);
            return new StreamReader(this.MemoryStream).ReadToEnd();
        }

        private class NonDisposableStream : MemoryStream
        {
            internal NonDisposableStream()
                : base()
            {
            }

            protected override void Dispose(bool disposing)
            {
                // Do not call the base on this call, since we do not want to dispose the stream.
            }
        }
    }
}