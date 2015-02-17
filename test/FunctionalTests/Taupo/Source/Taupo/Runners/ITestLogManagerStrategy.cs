//---------------------------------------------------------------------
// <copyright file="ITestLogManagerStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using Microsoft.Test.Taupo.Runners;

    /// <summary>
    /// This interface provides the logging strategy for the Taupo test runners
    /// </summary>
    public interface ITestLogManagerStrategy
    {
        /// <summary>
        /// Initializes the runner strategy and returns a test logger
        /// </summary>
        /// <param name="runnerStrategy">The Runner strategy for the corresponding Taupo test runner</param>
        /// <param name="writers"> Any additional writers to add to the loggers</param>
        /// <returns>A test log writer to log results from this test execution</returns>
        ITestLogWriter Initialize(RunnerStrategy runnerStrategy, params ITestLogWriter[] writers);

        /// <summary>
        /// Ends logging and cleans up any resources
        /// </summary>
        /// <param name="testLogWriter">The test log writer used for this test run</param>
        void Terminate(ITestLogWriter testLogWriter);
    }
}
