//---------------------------------------------------------------------
// <copyright file="MockHttpClientHandlerProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Net.Http;
using System.Threading;

namespace Microsoft.OData.Client.Tests.Serialization
{
    /// <summary>
    /// A mock implementation of <see cref="IHttpClientProvider"/>
    /// for testing purposes.
    /// </summary>
    internal sealed class MockHttpClientHandlerProvider: IHttpClientProvider
    {
        private readonly HttpClientHandler _handler;
        private int _numCalls = 0;

        /// <summary>
        /// Creates a new instance of <see cref="MockHttpClientHandlerProvider"/>.
        /// </summary>
        /// <param name="handler">The <see cref="HttpClientHandler"/> to return.</param>
        public MockHttpClientHandlerProvider(HttpClientHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Number of times <see cref="GetHttpClient()"/> was
        /// called.
        /// </summary>
        public int NumCalls => _numCalls;

        public HttpClientHandler GetHttpClient()
        {
            Interlocked.Increment(ref _numCalls);
            return _handler;
        }
    }
}
