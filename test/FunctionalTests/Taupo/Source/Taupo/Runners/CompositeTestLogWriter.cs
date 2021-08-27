//---------------------------------------------------------------------
// <copyright file="CompositeTestLogWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Security;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// Implementation of <see cref="ITestLogWriter"/> which delegates all notifications
    /// to multiple writers (Composite pattern).
    /// </summary>
    public sealed class CompositeTestLogWriter : MarshalByRefObject, ITestLogWriter
    {
        private ReadOnlyCollection<ITestLogWriter> writers;

        /// <summary>
        /// Initializes a new instance of the CompositeTestLogWriter class.
        /// </summary>
        /// <param name="writers">The list of writers to write to.</param>
        public CompositeTestLogWriter(params ITestLogWriter[] writers)
        {
            this.writers = writers.ToList().AsReadOnly();
        }

        /// <summary>
        /// Begins the test suite.
        /// </summary>
        public void BeginTestSuite()
        {
            this.ForAll(i => i.BeginTestSuite());
        }

        /// <summary>
        /// Begins the test case.
        /// </summary>
        /// <param name="testCase">The test case.</param>
        public void BeginTestCase(TestItemData testCase)
        {
            this.ForAll(i => i.BeginTestCase(testCase));
        }

        /// <summary>
        /// Begins the test module.
        /// </summary>
        /// <param name="testModule">The test module.</param>
        public void BeginTestModule(TestModuleData testModule)
        {
            this.ForAll(i => i.BeginTestModule(testModule));
        }

        /// <summary>
        /// Begins the variation.
        /// </summary>
        /// <param name="variation">The variation.</param>
        public void BeginVariation(TestItemData variation)
        {
            this.ForAll(i => i.BeginVariation(variation));
        }

        /// <summary>
        /// Ends the test suite.
        /// </summary>
        public void EndTestSuite()
        {
            this.ForAll(i => i.EndTestSuite());
        }

        /// <summary>
        /// Ends the test case.
        /// </summary>
        /// <param name="testCase">The test case.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        public void EndTestCase(TestItemData testCase, TestResult result, ExceptionDetails exception)
        {
            this.ForAll(i => i.EndTestCase(testCase, result, exception));
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
            this.ForAll(i => i.EndTestModule(testModule, result, exception, statisticsByPriority, globalStatistics));
        }

        /// <summary>
        /// Ends the variation.
        /// </summary>
        /// <param name="variation">The variation.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        public void EndVariation(TestItemData variation, TestResult result, ExceptionDetails exception)
        {
            this.ForAll(i => i.EndVariation(variation, result, exception));
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease"/> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"/> property.
        /// </returns>
        [SecurityCritical]
        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Writes the line to the test log.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message.</param>
        public void WriteLine(LogLevel logLevel, string message)
        {
            this.ForAll(i => i.WriteLine(logLevel, message));
        }

        private void ForAll(Action<ITestLogWriter> action)
        {
            foreach (var item in this.writers)
            {
                action(item);
            }
        }
    }
}
