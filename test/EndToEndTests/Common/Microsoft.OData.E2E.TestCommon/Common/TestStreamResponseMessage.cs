//-----------------------------------------------------------------------------
// <copyright file="TestStreamResponseMessage.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.E2E.TestCommon.Common
{
    /// <summary>
    /// Represents an OData response message that encapsulates an underlying <see cref="Stream"/>.
    /// </summary>
    public class TestStreamResponseMessage : IODataResponseMessageAsync, IServiceCollectionProvider
    {
        private readonly Stream stream;
        private readonly Dictionary<string, string> headers = new Dictionary<string, string>();

        public TestStreamResponseMessage(Stream stream)
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
            this.headers[headerName] = headerValue;
        }

        public Stream GetStream()
        {
            return this.stream;
        }

        public Task<Stream> GetStreamAsync()
        {
            return Task.FromResult(this.stream);
        }

        public IServiceProvider ServiceProvider { get; set; }
    }
}
