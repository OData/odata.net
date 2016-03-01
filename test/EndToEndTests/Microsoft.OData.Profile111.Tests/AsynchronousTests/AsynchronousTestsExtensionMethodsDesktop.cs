//---------------------------------------------------------------------
// <copyright file="AsynchronousTestsExtensionMethodsDesktop.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Microsoft.OData.Profile111.Tests.AsynchronousTests
{
    /// <summary>
    /// Extension methods for running asynchronous API tests using the desktop client
    /// </summary>
    public static class AsynchronousTestsExtensionMethods
    {
        private const int DeltaMilliseconds = 100;

        /// <summary>
        /// Mark the test as completed. Test should wait before exiting until this method is called.
        /// </summary>
        /// <param name="test">The asynchronous end-to-end test</param>
        public static void EnqueueTestComplete(this EndToEndTestBase test)
        {
            //If someday all the test cases are changed to the way that Linq_OrderByDescendingThenByDescendingTest is written, then this method should do nothing, just to be able to write test cases in the same way on all 4 platforms.This method is specific to SL.
            test.TestCompleted = true;
        }

        /// <summary>
        /// Suspends the current thread until the asynchronous operation completes.
        /// </summary>
        /// <param name="test">The caller test</param>
        public static void WaitForTestToComplete(this EndToEndTestBase test)
        {
            while (!test.TestCompleted)
            {
                Sleep(DeltaMilliseconds);
            }
        }

        public static void EnqueueConditional(this EndToEndTestBase test, Func<bool> predicate)
        {
            while (!predicate())
            {
                Sleep(DeltaMilliseconds);
            }
        }

        public static void EnqueueCallback(this EndToEndTestBase test, Action action)
        {
            action();
        }

        /// <summary>
        /// Blocks the current thread until the current async result is completed.
        /// </summary>
        /// <param name="asyncResult">The async result to wait for completions</param>
        /// <param name="test">The current test.</param>
        /// <returns></returns>
        public static IAsyncResult EnqueueWait(this IAsyncResult asyncResult, EndToEndTestBase test)
        {
            // "test" parameter is never used, but is needed to maintain the same interface across platforms
            asyncResult.AsyncWaitHandle.WaitOne();
            return asyncResult;
        }

        /// <summary>
        /// Blocks the current thread for the specified milliseconds
        /// </summary>
        /// <param name="milliseconds">The milliseconds to sleep</param>
        private static void Sleep(int milliseconds)
        {
            Task.Delay(milliseconds).Wait();
        }
    }
}