//---------------------------------------------------------------------
// <copyright file="MockHttpClientProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading;

namespace Microsoft.OData.Client.Tests.Serialization
{
    /// <summary>
    /// A mock implementation of <see cref="IHttpClientProvider"/>
    /// for testing purposes.
    /// </summary>
    internal sealed class MockHttpClientProvider

        : IHttpClientProvider
    {
        private readonly HttpClient _client;
        private int _numCalls = 0;

        /// <summary>
        /// Creates a new instance of <see cref="MockHttpClientProvider"/>.
        /// </summary>
        /// <param name="handler">The <see cref="HttpClientHandler"/> based on which to create HttpClient.</param>
        public MockHttpClientProvider(HttpClientHandler handler, MockHttpClientProviderOptions options = default)
        {
            _client = new HttpClient(handler);

            options = options == null ? new MockHttpClientProviderOptions() : options;
            if (options.Timeout.HasValue)
            {
                _client.Timeout = TimeSpan.FromSeconds(options.Timeout.Value);
            }
        }

        /// <summary>
        /// Number of times <see cref="GetHttpClient()"/> was
        /// called.
        /// </summary>
        public int NumCalls => _numCalls;

        public HttpClient GetHttpClient()
        {
            Interlocked.Increment(ref _numCalls);
            return _client;
        }
    }

    internal sealed class  MockHttpClientProviderOptions
    {
        public int? Timeout { get; set; }
    }
}
