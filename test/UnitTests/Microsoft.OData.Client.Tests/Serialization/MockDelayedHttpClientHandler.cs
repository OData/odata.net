//---------------------------------------------------------------------
// <copyright file="MockDelayedHttpClientHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OData.Client.Tests.Serialization
{
    /// <summary>
    /// A mock HttpClientHandler that responds to HTTP requests with a mocked
    /// response after a specified time delay.
    /// </summary>
    internal sealed class MockDelayedHttpClientHandler : MockHttpClientHandler
    {
        private readonly int _delay;
        public MockDelayedHttpClientHandler(string expectedResponse, int delayMilliseconds) : base(expectedResponse)
        {
            _delay = delayMilliseconds;
        }

        /// <summary>
        /// Callback that is triggered when a request has started.
        /// This can be used to coordinate with a task on a separate
        /// thread that should run only after a request has started.
        /// For example, we may want to test aborting a request after
        /// the request has started.
        /// </summary>
        public Action<HttpRequestMessage> OnRequestStarted { get; set; }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Notify start so tests (Abort / external cancellation) can act after "start"
            OnRequestStarted?.Invoke(request);

            if (_delay > 0)
            {
                // WaitOne returns true if the handle was signaled (cancellation requested)
                // before timeout elapses; false if the timeout elapsed first
                // This avoids a busy spin while still allowing prompt cancellation.
                bool cancelled = cancellationToken.WaitHandle.WaitOne(_delay);
                if (cancelled)
                {
                    // Ensure that we surface the OperationCanceledException consistently
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }

            // Delegate to base to create the standard mocked response
            return base.Send(request, cancellationToken);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Notify start so tests (Abort / external cancellation) can act after "start"
            OnRequestStarted?.Invoke(request);

            if (_delay > 0)
            {
                // Asynchronous cancellable delay - if per-request timeout or abort fires,
                // Task.Delay will throw OperationCanceledException promptly
                await Task.Delay(_delay, cancellationToken).ConfigureAwait(false);
            }

            // Delegate to base to create the standard mocked response
            return base.Send(request, cancellationToken);
        }
    }
}
