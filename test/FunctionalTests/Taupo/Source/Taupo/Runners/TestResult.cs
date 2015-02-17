//---------------------------------------------------------------------
// <copyright file="TestResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    /// <summary>
    /// Result of test case, module or variation execution.
    /// </summary>
    public enum TestResult
    {
        /// <summary>
        /// Test is currently running.
        /// </summary>
        InProgress,

        /// <summary>
        /// Test has passed.
        /// </summary>
        Passed,

        /// <summary>
        /// Test has passed with warnings.
        /// </summary>
        Warning,

        /// <summary>
        /// Test has been skipped.
        /// </summary>
        Skipped,

        /// <summary>
        /// Test has failed.
        /// </summary>
        Failed,

        /// <summary>
        /// Test has timed out.
        /// </summary>
        Timeout,

        /// <summary>
        /// Test has been aborted.
        /// </summary>
        Aborted,
    }
}