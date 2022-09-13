//---------------------------------------------------------------------
// <copyright file="SyncHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;
    using System.Globalization;
    using System.Threading;

    /// <summary>
    /// Helper functions for executing and waiting for async operations to complete
    /// </summary>
    public static class SyncHelpers
    {
        private const int MaxSecondsDefaultTimeout = 300;
        private const int MaxMillisecondsDefaultTimeout = MaxSecondsDefaultTimeout * 1000;

        /// <summary>
        /// Initializes static members of the SyncHelpers class
        /// </summary>
        static SyncHelpers()
        {
            MaxMillisecondsTimeout = MaxMillisecondsDefaultTimeout;
        }

        /// <summary>
        /// Gets or sets the maximum timeout in milliseconds. Defaults to 5 minutes.
        /// </summary>
        internal static int MaxMillisecondsTimeout { get; set; }

        /// <summary>
        /// Executes a specified method and waits until the action is completed
        /// </summary>
        /// <param name="action">The action.</param>
        public static void ExecuteActionAndWait(Action<IAsyncContinuation> action)
        {
            var waitHandle = new ManualResetEvent(false);
            Exception exception = null;
            var continuation = AsyncHelpers.CreateContinuation(
                () => waitHandle.Set(),
                exc =>
                {
                    exception = exc;
                    waitHandle.Set();
                });

            // try it synchronously, otherwise wait
            action(continuation);

            bool signalOccurred = WaitHandle.WaitAll(new WaitHandle[] { waitHandle }, MaxMillisecondsTimeout);

            if (!signalOccurred)
            {
                throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "The action has not completed within {0} milliseconds", MaxMillisecondsTimeout));
            }

            if (exception != null)
            {
                // wrap the exception to preserve the original call-stack
                throw new TaupoInfrastructureException("An exception occurred during asynchronous execution", exception);
            }
        }
    }
}
