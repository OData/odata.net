//---------------------------------------------------------------------
// <copyright file="ITestModuleRunner.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// Exposes a contract for processes to run tests in a module.
    /// </summary>
    public interface ITestModuleRunner
    {
        /// <summary>
        /// Occurs when the test run has completed.
        /// </summary>
        event EventHandler<EventArgs> RunCompleted;

        /// <summary>
        /// Occurs when the statistics for this particular run have changed.
        /// </summary>
        event EventHandler<RunStatisticsUpdatedEventArgs> RunStatisticsUpdated;

        /// <summary>
        /// Occurs when test item is starting execution.
        /// </summary>
        event EventHandler<TestItemStatusChangedEventArgs> TestItemStarting;

        /// <summary>
        /// Occurs when test item has completed execution.
        /// </summary>
        event EventHandler<TestItemStatusChangedEventArgs> TestItemCompleted;

        /// <summary>
        /// Attempts to abort execution of the test module.
        /// </summary>
        void Abort();

        /// <summary>
        /// Runs the specified module.
        /// </summary>
        /// <param name="testModuleData">Data about the test module to run.</param>
        /// <param name="logWriter">The log writer.</param>
        /// <param name="variationsToRun">The variations to run.</param>
        /// <param name="parameters">The test parameters with which to invoke the test module.</param>
        void Run(TestModuleData testModuleData, ITestLogWriter logWriter, IEnumerable<TestItemData> variationsToRun, IDictionary<string, string> parameters);
    }
}
