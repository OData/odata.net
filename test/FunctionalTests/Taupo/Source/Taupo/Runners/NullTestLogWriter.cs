//---------------------------------------------------------------------
// <copyright file="NullTestLogWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// Null implementation of <see cref="ITestLogWriter"/>.
    /// </summary>
    public sealed class NullTestLogWriter : ITestLogWriter
    {
        /// <summary>
        /// Begins the test suite.
        /// </summary>
        public void BeginTestSuite()
        {
        }

        /// <summary>
        /// Begins the test case.
        /// </summary>
        /// <param name="testCase">The test case.</param>
        public void BeginTestCase(TestItemData testCase)
        {
        }

        /// <summary>
        /// Begins the test module.
        /// </summary>
        /// <param name="testModule">The test module.</param>
        public void BeginTestModule(TestModuleData testModule)
        {
        }

        /// <summary>
        /// Begins the variation.
        /// </summary>
        /// <param name="variation">The variation.</param>
        public void BeginVariation(TestItemData variation)
        {
        }

        /// <summary>
        /// Ends the test suite.
        /// </summary>
        public void EndTestSuite()
        {
        }

        /// <summary>
        /// Ends the test case.
        /// </summary>
        /// <param name="testCase">The test case.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        public void EndTestCase(TestItemData testCase, TestResult result, ExceptionDetails exception)
        {
        }

        /// <summary>
        /// Ends the test module.
        /// </summary>
        /// <param name="testModule">The test module.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="statisticsByPriority">The statistics for the module, broken down by priority.</param>
        /// <param name="globalStatistics">The global statistics for the module.</param>
        public void EndTestModule(TestModuleData testModule, TestResult result, ExceptionDetails exception, IDictionary<int, RunStatistics> statisticsByPriority, RunStatistics globalStatistics)
        {
        }

        /// <summary>
        /// Ends the variation.
        /// </summary>
        /// <param name="variation">The variation.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        public void EndVariation(TestItemData variation, TestResult result, ExceptionDetails exception)
        {
        }

        /// <summary>
        /// Writes the line to the test log.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message.</param>
        public void WriteLine(LogLevel logLevel, string message)
        {
        }
    }
}
