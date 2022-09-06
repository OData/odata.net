//---------------------------------------------------------------------
// <copyright file="AsynchronousTestModuleRunnerBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// Asynchronous test module runner.
    /// </summary>
    public abstract class AsynchronousTestModuleRunnerBase : MarshalByRefObject, ITestModuleRunner
    {
        private ITestLogWriter currentLogWriter;
        private ExecutionContext currentContext;
        private Stack<ExecutionContext> executionContextStack = new Stack<ExecutionContext>();
        private Dictionary<TestItemData, TestResult> completedItems = new Dictionary<TestItemData, TestResult>();
        private int asyncCounter;
        private Dictionary<TestItemData, TestItemData> scheduledVariations;
        private object lockObject = new object();

        private bool runCompleted;

        private Thread executionThread;
        private Action actionToExecute;
        private object executionThreadMonitor = new object();
        private bool executionThreadAborted;
        private Dictionary<int, RunStatistics> statisticsByPriority = new Dictionary<int, RunStatistics>();
        private RunStatistics globalStatistics = new RunStatistics();

        /// <summary>
        /// Initializes a new instance of the AsynchronousTestModuleRunnerBase class.
        /// </summary>
        protected AsynchronousTestModuleRunnerBase()
        {
        }

        /// <summary>
        /// Occurs when test item is starting execution.
        /// </summary>
        public event EventHandler<TestItemStatusChangedEventArgs> TestItemStarting;

        /// <summary>
        /// Occurs when test item has completed execution.
        /// </summary>
        public event EventHandler<TestItemStatusChangedEventArgs> TestItemCompleted;

        /// <summary>
        /// Occurs when the test run has completed.
        /// </summary>
        public event EventHandler<EventArgs> RunCompleted;

        /// <summary>
        /// Occurs when the statistics for this particular run have changed.
        /// </summary>
        public event EventHandler<RunStatisticsUpdatedEventArgs> RunStatisticsUpdated;

        internal enum CurrentState
        {
            Initializing,
            Executing,
            Terminating,
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
        /// Runs the specified module.
        /// </summary>
        /// <param name="testModuleData">Data about the test module to run.</param>
        /// <param name="logWriter">The log writer.</param>
        /// <param name="variationsToRun">The variations to run.</param>
        /// <param name="parameters">The parameters.</param>
        public void Run(TestModuleData testModuleData, ITestLogWriter logWriter, IEnumerable<TestItemData> variationsToRun, IDictionary<string, string> parameters)
        {
            ExceptionUtilities.CheckArgumentNotNull(testModuleData, "testModuleData");
            ExceptionUtilities.CheckArgumentNotNull(logWriter, "logWriter");

            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }

            var testModule = (TestModule)Activator.CreateInstance(testModuleData.TestItemType);
            testModule.ExplorationSeed = testModuleData.ExplorationSeed;
            testModule.ExplorationKind = testModuleData.ExplorationKind;

            foreach (var param in parameters)
            {
                testModule.TestParameters[param.Key] = param.Value;
            }

            if (variationsToRun == null)
            {
                variationsToRun = testModuleData.GetAllChildrenRecursive().Where(c => c.IsVariation);
            }

            lock (this.lockObject)
            {
                var logger = new TestLogWriterLogger(logWriter);
                logger.OnWarning += this.OnWarning;
                testModule.Log = logger;

                this.executionThreadAborted = false;
                this.runCompleted = false;
                this.currentLogWriter = logWriter;
                this.scheduledVariations = variationsToRun.Distinct().ToDictionary<TestItemData, TestItemData>(iteration => iteration);
                this.completedItems.Clear();
                this.statisticsByPriority.Clear();
                this.globalStatistics = new RunStatistics();
                this.currentLogWriter.BeginTestSuite();
                this.executionThread = new Thread(this.ExecutionThreadStart)
                {
                    IsBackground = true,
                };

                this.executionThread.Start();
                this.BeginExecution(testModule, testModuleData);
            }
        }

        /// <summary>
        /// Attempts to abort execution of the test module.
        /// </summary>
        public void Abort()
        {
            var thread = this.executionThread;
            this.executionThread = null;

            if (thread != null)
            {
                // kill the execution thread - when it completes it will call 
                thread.Abort();
                thread.Join();
            }
        }

        /// <summary>
        /// Dispatches the specified action asynchronously on the user interface thread.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <remarks>The function exits immediately, execution of the action continues 
        /// later on a different thread.</remarks>
        protected abstract void ExecuteOnUIThread(Action action);

        /// <summary>
        /// Raises the <see cref="RunStatisticsUpdated"/> event. Not intended to be used publically.
        /// </summary>
        /// <param name="e">The <see cref="Microsoft.Test.Taupo.Runners.RunStatisticsUpdatedEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This must be public in order to handle an event raised from a
        /// partial trust <see cref="AppDomain"/>.
        /// </remarks>
        protected virtual void OnRunStatisticsUpdated(RunStatisticsUpdatedEventArgs e)
        {
            var handler = this.RunStatisticsUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void ExecutionThreadStart()
        {
            try
            {
                while (!this.runCompleted)
                {
                    lock (this.executionThreadMonitor)
                    {
                        while (this.actionToExecute == null && !this.runCompleted)
                        {
                            Monitor.Wait(this.executionThreadMonitor);
                        }

                        if (!this.runCompleted)
                        {
                            var action = this.actionToExecute;
                            this.actionToExecute = null;
                            action();
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
                this.ExecuteOnUIThread(this.ExecutionThreadAborted);
            }
        }

        private void OnWarning(object sender, EventArgs e)
        {
            var context = this.currentContext;
            if (context != null)
            {
                if (context.CurrentResult < TestResult.Warning)
                {
                    context.CurrentResult = TestResult.Warning;
                }
            }
        }

        private void LogBeginTestItem(TestItemData itemData)
        {
            var testModuleData = itemData as TestModuleData;
            if (testModuleData != null)
            {
                this.currentLogWriter.BeginTestModule(testModuleData);
                return;
            }

            if (itemData.IsTestCase)
            {
                this.currentLogWriter.BeginTestCase(itemData);
                return;
            }

            if (itemData.IsVariation)
            {
                this.currentLogWriter.BeginVariation(itemData);
                return;
            }
        }

        private void BeginExecution(TestItem testItem, TestItemData itemData)
        {
            if (this.currentContext != null)
            {
                this.executionContextStack.Push(this.currentContext);
            }

            this.currentContext = new ExecutionContext()
            {
                TestItem = testItem,
                TestItemData = itemData,
                CurrentChildIndex = -1,
                CurrentState = CurrentState.Initializing,
                CurrentResult = TestResult.InProgress,
            };

            this.ReportItemStarting(TestResult.InProgress);
            this.LogBeginTestItem(itemData);

            var callback = this.CreateCallback();
            this.RunOnExecutionThread(() => testItem.InitAsync(callback));
        }

        private void EnterState(CurrentState currentState)
        {
            lock (this.lockObject)
            {
                switch (currentState)
                {
                    case CurrentState.Executing:
                        this.currentContext.CurrentState = currentState;

                        VariationTestItem vti = this.currentContext.TestItem as VariationTestItem;
                        if (vti != null)
                        {
                            var callback = this.CreateCallback();
                            if (vti.Timeout > 0)
                            {
                                callback.SetTimeout(vti.Timeout);
                            }

                            this.RunOnExecutionThread(() => vti.ExecuteAsync(callback));
                        }
                        else
                        {
                            this.ExecuteOnUIThread(this.OnSuccess);
                        }

                        break;

                    case CurrentState.Terminating:
                        {
                            this.currentContext.CurrentState = currentState;
                            var callback = this.CreateCallback();
                            this.currentContext.TestItem.TerminateAsync(callback);
                        }

                        break;
                }
            }
        }

        private AsynchronousCallback CreateCallback()
        {
            int counter = Interlocked.Increment(ref this.asyncCounter);
            return new AsynchronousCallback(this, counter);
        }

        private void ReportItemStarting(TestResult result)
        {
            var testItem = this.currentContext.TestItemData;

            if (this.TestItemStarting != null)
            {
                this.TestItemStarting(this, new TestItemStatusChangedEventArgs(testItem, result));
            }
        }

        private void ReportItemCompleted(TestResult result)
        {
            var testItem = this.currentContext.TestItemData;

            if (this.TestItemCompleted != null)
            {
                this.TestItemCompleted(this, new TestItemStatusChangedEventArgs(testItem, result));
            }
        }

        private void OnSuccess()
        {
            lock (this.lockObject)
            {
                Interlocked.Increment(ref this.asyncCounter);
                var result = TestResult.Passed;
                this.ProcessResult(result);
            }
        }

        private void OnFailure(Exception ex)
        {
            // bump the async counter to prevent any other event from interfering
            Interlocked.Increment(ref this.asyncCounter);

            lock (this.lockObject)
            {
                var result = ex is TestSkippedException ? TestResult.Skipped : TestResult.Failed;
                this.currentContext.Exception = ex;
                this.ProcessResult(result);
            }
        }

        private void OnTimeout()
        {
            // bump the async counter to prevent any other event from interfering
            Interlocked.Increment(ref this.asyncCounter);

            lock (this.lockObject)
            {
                var result = TestResult.Timeout;
                this.currentContext.Exception = new TaupoInfrastructureException("Timeout.");
                this.ProcessResult(result);
            }
        }

        private void ExecutionThreadAborted()
        {
            // bump the async counter to prevent any other event from interfering
            Interlocked.Increment(ref this.asyncCounter);

            // this is so future execution requests will be short-circuited
            this.executionThreadAborted = true;

            lock (this.lockObject)
            {
                var result = TestResult.Aborted;
                this.currentContext.Exception = new TaupoInfrastructureException("Aborted.");
                this.ProcessResult(result);
            }
        }

        private void ProcessResult(TestResult result)
        {
            if (result > this.currentContext.CurrentResult)
            {
                this.currentContext.CurrentResult = result;
            }

            switch (this.currentContext.CurrentState)
            {
                case CurrentState.Initializing:
                    if (result == TestResult.Passed)
                    {
                        this.EnterState(CurrentState.Executing);
                    }
                    else
                    {
                        // if init failed or was skipped, don't execute, go to terminate
                        this.EnterState(CurrentState.Terminating);
                    }

                    break;

                case CurrentState.Executing:
                    this.ExecuteNextChild();
                    break;

                case CurrentState.Terminating:
                    this.EndTestItem();
                    break;
            }
        }

        private void EndTestItem()
        {
            var testItemData = this.currentContext.TestItemData;
            var result = this.currentContext.CurrentResult;
            var exceptionDetails = (this.currentContext.Exception == null) ? (ExceptionDetails)null : new ExceptionDetails(this.currentContext.Exception);

            this.completedItems[testItemData] = result;

            if (result == TestResult.Failed || result == TestResult.Timeout)
            {
                if (exceptionDetails != null)
                {
                    foreach (var bug in testItemData.Bugs)
                    {
                        this.currentLogWriter.WriteLine(LogLevel.Verbose, bug.ToString());
                    }
                }
            }

            bool updatedCounters = false;
            if (testItemData.IsVariation)
            {
                this.IncrementResultCounter(testItemData, result);
                updatedCounters = true;

                this.currentLogWriter.EndVariation(testItemData, result, exceptionDetails);
            }
            else if (result == TestResult.Failed || result == TestResult.Skipped || result == TestResult.Timeout)
            {
                // If this is a failure/skip/timeout for a test item that's not a variation,
                // mark all its variations that were scheduled to run but have
                // not already run as that result.
                foreach (var tid in testItemData.GetAllChildrenRecursive().Where(
                    t => t.IsVariation && !this.completedItems.ContainsKey(t) && this.scheduledVariations.ContainsKey(t)))
                {
                    this.IncrementResultCounter(tid, result);
                    updatedCounters = true;
                }
            }

            if (updatedCounters)
            {
                this.OnRunStatisticsUpdated(new RunStatisticsUpdatedEventArgs(this.globalStatistics));
            }

            if (testItemData.IsTestCase)
            {
                this.currentLogWriter.EndTestCase(testItemData, result, exceptionDetails);
            }

            TestModuleData testModuleData = testItemData as TestModuleData;
            if (testModuleData != null)
            {
                this.currentLogWriter.EndTestModule(testModuleData, result, exceptionDetails, this.statisticsByPriority, this.globalStatistics);
            }

            this.ReportItemCompleted(result);
            this.GoToNextParentItem();
        }

        private void GoToNextParentItem()
        {
            if (this.executionContextStack.Count > 0)
            {
                this.currentContext = this.executionContextStack.Pop();
                this.ExecuteNextChild();
            }
            else
            {
                this.RunFinished();
            }
        }

        private void RunFinished()
        {
            this.currentLogWriter.EndTestSuite();

            // this causes execution thread to stop gracefully
            lock (this.executionThreadMonitor)
            {
                this.runCompleted = true;
                Monitor.Pulse(this.executionThreadMonitor);
            }

            this.executionThread = null;

            var handler = this.RunCompleted;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        private void ExecuteNextChild()
        {
            do
            {
                this.currentContext.CurrentChildIndex++;
            }
            while (this.currentContext.CurrentChildIndex < this.currentContext.TestItem.Children.Count
                && !this.ShouldRun(this.currentContext.TestItemData.Children[this.currentContext.CurrentChildIndex]));

            if (this.currentContext.CurrentChildIndex < this.currentContext.TestItem.Children.Count)
            {
                var testItem = this.currentContext.TestItem.Children[this.currentContext.CurrentChildIndex];
                var testItemData = this.currentContext.TestItemData.Children[this.currentContext.CurrentChildIndex];
                ExceptionUtilities.Assert(TestItemData.Matches(testItemData, testItem), "TestItemData and TestItem hierarchies should match exactly.");

                this.BeginExecution(
                    testItem,
                    testItemData);
            }
            else
            {
                this.EnterState(CurrentState.Terminating);
            }
        }

        private bool ShouldRun(TestItemData testItemData)
        {
            if (this.executionThreadAborted)
            {
                return false;
            }

            if (testItemData.IsVariation)
            {
                return this.scheduledVariations.ContainsKey(testItemData);
            }
            else
            {
                return testItemData.Children.Any(i => this.ShouldRun(i));
            }
        }

        /// <summary>
        /// Run action on a separate thread which can be aborted.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        private void RunOnExecutionThread(Action action)
        {
            ExceptionUtilities.Assert(this.executionThread != null, "executionThread != null");
            ExceptionUtilities.Assert(!this.executionThreadAborted, "!this.executionThreadAborted");
            ExceptionUtilities.Assert(!this.runCompleted, "!this.runCompleted");

            lock (this.executionThreadMonitor)
            {
                this.actionToExecute = action;
                Monitor.Pulse(this.executionThreadMonitor);
            }
        }

        private void IncrementResultCounter(TestItemData testItemData, TestResult result)
        {
            int priority = testItemData.Metadata.Priority;

            RunStatistics statistics;
            if (!this.statisticsByPriority.TryGetValue(priority, out statistics))
            {
                this.statisticsByPriority[priority] = statistics = new RunStatistics();
            }

            switch (result)
            {
                case TestResult.Timeout:
                    ++statistics.Timeouts;
                    ++this.globalStatistics.Timeouts;
                    break;
                case TestResult.Skipped:
                    ++statistics.Skipped;
                    ++this.globalStatistics.Skipped;
                    break;
                case TestResult.Failed:
                    ++statistics.Failures;
                    ++this.globalStatistics.Failures;
                    break;
                case TestResult.Passed:
                    ++statistics.Passed;
                    ++this.globalStatistics.Passed;
                    break;
                case TestResult.Warning:
                    ++statistics.Warnings;
                    ++this.globalStatistics.Warnings;
                    break;
            }
        }

        /// <summary>
        /// Implementation of <see cref="IAsyncContinuation"/> which reports status back to the runner.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Timer is always automatically disposed.")]
        private class AsynchronousCallback : IAsyncContinuation
        {
            private AsynchronousTestModuleRunnerBase runner;
            private int expectedAsyncCounter;
            private Timer timer;
            private object lockObject = new object();

            /// <summary>
            /// Initializes a new instance of the AsynchronousCallback class.
            /// </summary>
            /// <param name="runner">The runner.</param>
            /// <param name="expectedAsyncCounter">The expected asynchronous call counter.</param>
            /// <remarks>The runner maintains the asynchronous call counter and each callback gets created
            /// with the expected value for the counter. If the current value of the counter is different
            /// than expected, the result is not reported to the runner. This is to prevent out-of-order
            /// results from destroying the log file.</remarks>
            public AsynchronousCallback(AsynchronousTestModuleRunnerBase runner, int expectedAsyncCounter)
            {
                this.runner = runner;
                this.expectedAsyncCounter = expectedAsyncCounter;
            }

            /// <summary>
            /// Reports success of asynchronous method invocation.
            /// </summary>
            public void Continue()
            {
                lock (this.lockObject)
                {
                    this.StopTimer();
                    if (this.expectedAsyncCounter == this.runner.asyncCounter)
                    {
                        this.runner.ExecuteOnUIThread(() => this.runner.OnSuccess());
                    }
                }
            }

            /// <summary>
            /// Reports the failure of asynchronous method invocation.
            /// </summary>
            /// <param name="exception">The exception.</param>
            public void Fail(Exception exception)
            {
                lock (this.lockObject)
                {
                    this.StopTimer();
                    if (this.expectedAsyncCounter == this.runner.asyncCounter)
                    {
                        this.runner.ExecuteOnUIThread(() => this.runner.OnFailure(exception));
                    }
                }
            }

            /// <summary>
            /// Sets the timeout for the execution.
            /// </summary>
            /// <param name="timeout">The timeout in milliseconds.</param>
            public void SetTimeout(int timeout)
            {
                lock (this.lockObject)
                {
                    this.timer = new Timer(state => this.runner.ExecuteOnUIThread(this.OnTimerElapsed), null, timeout, Timeout.Infinite);
                }
            }

            private void OnTimerElapsed()
            {
                lock (this.lockObject)
                {
                    this.StopTimer();
                    if (this.expectedAsyncCounter == this.runner.asyncCounter)
                    {
                        this.runner.OnTimeout();
                    }
                }
            }

            private void StopTimer()
            {
                if (this.timer != null)
                {
                    this.timer.Dispose();
                    this.timer = null;
                }
            }
        }

        /// <summary>
        /// Execution context.
        /// </summary>
        private class ExecutionContext
        {
            /// <summary>
            /// Gets or sets the test item.
            /// </summary>
            internal TestItem TestItem { get; set; }

            /// <summary>
            /// Gets or sets the test item data.
            /// </summary>
            internal TestItemData TestItemData { get; set; }

            /// <summary>
            /// Gets or sets the index of the current child.
            /// </summary>
            internal int CurrentChildIndex { get; set; }

            /// <summary>
            /// Gets or sets the current state.
            /// </summary>
            internal CurrentState CurrentState { get; set; }

            /// <summary>
            /// Gets or sets the current result.
            /// </summary>
            /// <value>The current result.</value>
            internal TestResult CurrentResult { get; set; }

            /// <summary>
            /// Gets or sets the current exception.
            /// </summary>
            /// <value>The exception.</value>
            internal Exception Exception { get; set; }
        }

        /// <summary>
        /// Implementation of <see cref="Logger"/> which writes to <see cref="ITestLogWriter"/>.
        /// </summary>
        private class TestLogWriterLogger : Logger
        {
            private ITestLogWriter writer;

            /// <summary>
            /// Initializes a new instance of the TestLogWriterLogger class.
            /// </summary>
            /// <param name="writer">The writer to forward log messages to.</param>
            public TestLogWriterLogger(ITestLogWriter writer)
            {
                this.writer = writer;
            }

            /// <summary>
            /// Occurs when the warning message has been written.
            /// </summary>
            public event EventHandler<EventArgs> OnWarning;

            /// <summary>
            /// Writes formatted text to the log output.
            /// </summary>
            /// <param name="logLevel">Log level.</param>
            /// <param name="text">Formatted text</param>
            protected override void WriteToOutput(LogLevel logLevel, string text)
            {
                if (logLevel == LogLevel.Warning)
                {
                    var handler = this.OnWarning;
                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                }

                this.writer.WriteLine(logLevel, text);
            }
        }
    }
}
