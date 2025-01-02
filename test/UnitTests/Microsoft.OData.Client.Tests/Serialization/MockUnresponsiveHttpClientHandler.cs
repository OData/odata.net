//---------------------------------------------------------------------
// <copyright file="MockUnresponsiveHttpClientHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OData.Client.Tests.Serialization
{
    /// <summary>
    /// A mock implementation of <see cref="HttpClientHandler"/> that
    /// is used for testing cancellation of requests. It spins until cancelled.
    /// If the requests stays too long without being cancelled, an exception
    /// is thrown.
    /// </summary>
    internal class MockUnresponsiveHttpClientHandler : HttpClientHandler
    {
        private const int MaxWaitTimeMillis = 5000;

        /// <summary>
        /// Callback that is triggered when a request has started.
        /// This can be used to coordinate with a task on a separate
        /// thread that should run only after a request has started.
        /// For example, we may want to test aborting a request after
        /// the request has started.
        /// </summary>
        public Action OnRequestStarted { get; set; }

        public HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            NotifyRequestStart();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // If it's been running for this long, then there's probably something
                // wrong with the test
                long elapsed = stopwatch.ElapsedMilliseconds;
                if (stopwatch.ElapsedMilliseconds > MaxWaitTimeMillis)
                {
                    throw new Exception(
                        $"{nameof(MockUnresponsiveHttpClientHandler)} has exceeded maximum {MaxWaitTimeMillis}ms wait time in request."
                        + "Review your logic to ensure the request is cancelled in time.");
                }
            }

#pragma warning disable CS0162 // Unreachable code detected
            return null;
#pragma warning restore CS0162 // Unreachable code detected
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = Send(request, cancellationToken);
            return Task.FromResult(response);
        }

        private void NotifyRequestStart()
        {
            OnRequestStarted?.Invoke();
        }
    }
}
