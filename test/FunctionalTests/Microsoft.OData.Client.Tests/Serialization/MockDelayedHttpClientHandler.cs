//---------------------------------------------------------------------
// <copyright file="MockDelayedHttpClientHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;

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
            NotifyRequestStart(request);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (stopwatch.ElapsedMilliseconds > _delay)
                {
                    break;
                }
            }

            stopwatch.Stop();
            return base.Send(request, cancellationToken);
        }

        private void NotifyRequestStart(HttpRequestMessage request)
        {
            OnRequestStarted?.Invoke(request);
        }
    }
}
