//---------------------------------------------------------------------
// <copyright file="MockHttpClientHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OData.Client.Tests.Serialization
{
    /// <summary>
    /// A mock implementation of <see cref="HttpClientHandler"/>
    /// for testing purposes.
    /// </summary>
    internal class MockHttpClientHandler : HttpClientHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _requestHandler;
        private readonly List<string> _requests = new List<string>();

        /// <summary>
        /// Creates an instance of <see cref="MockHttpClientHandler"/> that
        /// returns the specified response contents for any request.
        /// </summary>
        /// <param name="expectedResponse">The contents to return in each response.</param>
        public MockHttpClientHandler(string expectedResponse)
            : this((_) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(expectedResponse);
                return response;
            })
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="MockHttpClientHandler"/> with the
        /// specified request handler.
        /// </summary>
        /// <param name="requestHandler"></param>
        public MockHttpClientHandler(Func<HttpRequestMessage, HttpResponseMessage> requestHandler)
        {
            _requestHandler = requestHandler;
        }

        /// <summary>
        /// List of requests handled by this handler.
        /// Each item is of the form: "[METHOD] [url]"
        /// e.g.: { "GET http://service/", "GET http://service/path" }.
        /// </summary>
        public IReadOnlyList<string> Requests => _requests;

        /// <summary>
        /// Whether the handler has been disposed;
        /// </summary>
        public bool Disposed { get; private set; } = false;

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _requests.Add($"{request.Method} {request.RequestUri.AbsoluteUri}");
            HttpResponseMessage response = _requestHandler(request);
            return response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = Send(request, cancellationToken);
            return Task.FromResult(response);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.Disposed)
            {
                return;
            }

            this.Disposed = true;
            base.Dispose(disposing);
        }
    }
}
