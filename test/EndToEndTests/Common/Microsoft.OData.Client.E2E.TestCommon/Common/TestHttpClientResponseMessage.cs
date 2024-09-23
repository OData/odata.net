//-----------------------------------------------------------------------------
// <copyright file="TestHttpClientResponseMessage.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.Client.E2E.TestCommon.Common
{
    /// <summary>
    /// An implementation of <see cref="IODataResponseMessageAsync"/> that uses an <see cref="HttpResponseMessage"/> under the covers.
    /// In ODataLibrary, a message is an abstraction which consists of stream and header interfaces that hides the details of stream-reading/writing.
    /// </summary>
    public class TestHttpClientResponseMessage : IODataResponseMessageAsync, IServiceCollectionProvider
    {
        private readonly HttpResponseMessage _response;
        private bool _disposed;

        public TestHttpClientResponseMessage(HttpResponseMessage response)
        {
            _response = response;
        }

        public string? GetHeader(string headerName)
        {
            if (_response.Headers.TryGetValues(headerName, out var values))
            {
                return string.Join(",", values);
            }

            if (_response.Content.Headers.TryGetValues(headerName, out values))
            {
                return string.Join(",", values);
            }

            return null;
        }

        public async Task<Stream> GetStreamAsync()
        {
            return await _response.Content.ReadAsStreamAsync();
        }

        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                foreach (var header in _response.Headers)
                {
                    yield return new KeyValuePair<string, string>(header.Key, string.Join(",", header.Value));
                }

                foreach (var contentHeader in _response.Content.Headers)
                {
                    yield return new KeyValuePair<string, string>(contentHeader.Key, string.Join(",", contentHeader.Value));
                }
            }
        }

        public void SetHeader(string headerName, string headerValue)
        {
            // HttpResponseMessage doesn't allow directly setting headers on the response, 
            throw new NotSupportedException("Setting headers on HttpResponseMessage is not supported.");
        }

        public int StatusCode
        {
            get
            {
                return (int)_response.StatusCode;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public string StatusDescription
        {
            get => _response.ReasonPhrase;
            set
            {
                throw new NotSupportedException();
            }
        }

        public Stream GetStream()
        {
            return _response.Content.ReadAsStream();
        }

        public IServiceProvider ServiceProvider { get; set; }
    }
}