//---------------------------------------------------------------------
// <copyright file="AsynchronousTestsExtensionMethodsSilverlight.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if SILVERLIGHT
namespace Microsoft.Test.OData.Tests.Client
{
    using System;

    /// <summary>
    /// Extension methods for running asynchronous API tests using the Silverlight client
    /// </summary>
    public static class AsynchronousTestsExtensionMethodsSilverlight
    {
        /// <summary>
        /// Shim to match desktop tests. Silverlight tests with the Asynchronous attribute wait unti they call EnqueueTestComplete
        /// </summary>
        /// <param name="test">The caller test</param>
        public static void WaitForTestToComplete(this EndToEndTestBase test)
        {
        }

        /// <summary>
        /// Blocks the current thread until the current async result is completed.
        /// </summary>
        /// <param name="asyncResult">The async result to wait for completions</param>
        /// <param name="test">The current test</param>
        /// <returns></returns>
        public static IAsyncResult EnqueueWait(this IAsyncResult asyncResult, EndToEndTestBase test)
        {
            test.EnqueueConditional(() => asyncResult.IsCompleted);
            return asyncResult;
        }
    }
}
#endif