//-----------------------------------------------------------------------------
// <copyright file="StreamResponseMessage.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.Client.E2E.TestCommon.Common
{
    /// An implementation of IODataResponseMessage.
    /// In ODataLibrary, a message is an abstraction which consists of stream and header interfaces that hides the details of stream-reading/writing.
    public class StreamResponseMessage : IODataResponseMessage, IServiceCollectionProvider
    {
        private readonly Stream stream;
        private readonly Dictionary<string, string> headers = new Dictionary<string, string>();

        public StreamResponseMessage(Stream stream)
        {
            this.stream = stream;
        }

        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { return this.headers; }
        }

        public int StatusCode { get; set; }

        public string GetHeader(string headerName)
        {
            string value;

            return this.headers.TryGetValue(headerName, out value) ? value : null;
        }

        public void SetHeader(string headerName, string headerValue)
        {
            this.headers.Add(headerName, headerValue);
        }

        public Stream GetStream()
        {
            return this.stream;
        }

        public IServiceProvider ServiceProvider { get; set; }
    }
}
