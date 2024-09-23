//-----------------------------------------------------------------------------
// <copyright file="TestHttpClientRequestMessage.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.Client.E2E.TestCommon.Common
{
    /// <summary>
    /// An implementation of <see cref="IODataRequestMessageAsync"/> that uses an <see cref="HttpRequestMessage"/> under the covers.
    /// In OData library, a message is an abstraction which consists of stream and header interfaces that hides the details of stream-reading/writing.
    /// </summary>
    public class TestHttpClientRequestMessage : IODataRequestMessageAsync, IServiceCollectionProvider, IDisposable, IAsyncDisposable
    {
        private readonly HttpRequestMessage _request;
        private readonly HttpClient _httpClient;
        private HttpContent _content;
        private Stream _stream;
        private bool _disposed;

        public TestHttpClientRequestMessage(Uri uri, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _stream = new MemoryStream();

            _request = new HttpRequestMessage
            {
                RequestUri = uri
            };
        }

        public string? GetHeader(string headerName)
        {
            if (_request.Headers.TryGetValues(headerName, out var values))
            {
                return string.Join(",", values);
            }

            if (_request.Content.Headers.TryGetValues(headerName, out values))
            {
                return string.Join(",", values);
            }

            return null;
        }

        public async Task<Stream> GetStreamAsync()
        {
            if (_stream == null)
            {
                _stream = new MemoryStream();
                _content = new StreamContent(_stream);
                _request.Content = _content;
            }
            else
            {
                _stream.Position = _stream.Length;
            }

            return await Task.FromResult<Stream>(_stream);
        }

        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                if (_content != null)
                {
                    foreach (var contentHeader in _content.Headers)
                    {
                        yield return new KeyValuePair<string, string>(contentHeader.Key, string.Join(",", contentHeader.Value));
                    }
                }
            }
        }

        public void SetHeader(string headerName, string headerValue)
        {
            if (!_request.Headers.TryAddWithoutValidation(headerName, headerValue))
            {
                if (_content == null)
                {
                    _content = new StreamContent(_stream);
                    _request.Content = _content;
                }
                _content.Headers.TryAddWithoutValidation(headerName, headerValue);
            }
        }

        public async Task<IODataResponseMessageAsync> GetResponseAsync()
        {
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.SendAsync(_request);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Request to {_request.RequestUri} failed: {ex.Message}", ex);
            }

            return new TestHttpClientResponseMessage(response)
            {
                ServiceProvider = ServiceProvider
            };
        }

        public Stream GetStream()
        {
            if (_stream == null)
            {
                _stream = new MemoryStream();
                _content = new StreamContent(_stream);
                _request.Content = _content;
            }
            else
            {
                _stream.Position = _stream.Length;
            }

            return _stream;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _stream?.Dispose();
                _content?.Dispose();
                _request?.Dispose();
            }

            _disposed = true;
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_disposed)
            {
                return;
            }

            if (_stream != null)
            {
                await _stream.DisposeAsync();
            }

            _content?.Dispose();
            _request?.Dispose();
            _disposed = true;
        }

        public Uri Url
        {
            get => _request.RequestUri;
            set => throw new InvalidOperationException("Request Uri cannot be changed");
        }

        public string Method
        {
            get => _request.Method.Method;
            set => _request.Method = new HttpMethod(value);
        }

        public IServiceProvider ServiceProvider { get; set; }
    }
}