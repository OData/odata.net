//---------------------------------------------------------------------
// <copyright file="ITestLogWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// Writes test log file.
    /// </summary>
    /// <remarks>Methods of this interface are invoked by <see cref="AsynchronousTestModuleRunnerBase"/>
    /// during test execution and should result in corresponding information written
    /// to the test log.</remarks>
    public interface ITestLogWriter
    {
        /// <summary>
        /// Begins the test suite.
        /// </summary>
        void BeginTestSuite();

        /// <summary>
        /// Ends the test suite.
        /// </summary>
        void EndTestSuite();

        /// <summary>
        /// Begins the test case.
        /// </summary>
        /// <param name="testCase">The test case.</param>
        void BeginTestCase(TestItemData testCase);

        /// <summary>
        /// Ends the test case.
        /// </summary>
        /// <param name="testCase">The test case.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        void EndTestCase(TestItemData testCase, TestResult result, ExceptionDetails exception);

        /// <summary>
        /// Begins the test module.
        /// </summary>
        /// <param name="testModule">The test module.</param>
        void BeginTestModule(TestModuleData testModule);

        /// <summary>
        /// Ends the test module.
        /// </summary>
        /// <param name="testModule">The test module.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="statisticsByPriority">The statistics for the module, broken down by priority.</param>
        /// <param name="globalStatistics">The global statistics for the module.</param>
        void EndTestModule(TestModuleData testModule, TestResult result, ExceptionDetails exception, IDictionary<int, RunStatistics> statisticsByPriority, RunStatistics globalStatistics);

        /// <summary>
        /// Begins the variation.
        /// </summary>
        /// <param name="variation">The variation.</param>
        void BeginVariation(TestItemData variation);

        /// <summary>
        /// Ends the variation.
        /// </summary>
        /// <param name="variation">The variation.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        void EndVariation(TestItemData variation, TestResult result, ExceptionDetails exception);

        /// <summary>
        /// Writes the line to the test log.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message.</param>
        void WriteLine(LogLevel logLevel, string message);
    }
}
