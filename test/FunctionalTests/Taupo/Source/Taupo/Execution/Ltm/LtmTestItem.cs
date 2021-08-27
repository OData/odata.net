//---------------------------------------------------------------------
// <copyright file="LtmTestItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Security;
using System.Threading;
using Microsoft.Test.Taupo.Common;

namespace Microsoft.Test.Taupo.Execution.Ltm
{
    [SecurityCritical]
    public abstract class LtmTestItem : ITestItem
    {
        protected LtmTestItem(TestItem item)
        {
            TestItem = item;
        }

        public abstract TestType ItemType { get; }

        public TestItem TestItem { get; private set; }

        public string Guid
        {
            [SecurityCritical]
            get { return this.TestItem.GetType().FullName; }
        }

        public int Id
        {
            [SecurityCritical]
            get { return this.TestItem.Metadata.Id; }
        }
        
        public string Name
        {
            [SecurityCritical]
            get { return this.TestItem.Metadata.Name; }
        }
        
        public string Desc
        {
            [SecurityCritical]
            get { return this.TestItem.Metadata.Description; }
        }

        public string Owner
        {
            [SecurityCritical]
            get { return this.TestItem.Metadata.Owner; }
        }

        public string Version
        {
            [SecurityCritical]
            get { return this.TestItem.Metadata.Version; }
        }

        public int Priority
        {
            [SecurityCritical]
            get { return this.TestItem.Metadata.Priority; }
        }

        TestType ITestItem.Type
        {
            [SecurityCritical]
            get { return this.ItemType; }
        }
       
        TestFlags ITestItem.Flags
        {
            [SecurityCritical]
            get { throw new NotImplementedException(); }
        }

        ITestProperties ITestItem.Metadata
        {
            [SecurityCritical]
            get { return null; }
        }

        ITestItems ITestItem.Children
        {
            [SecurityCritical]
            get
            {
                return new LtmTestItems(this.GetChildren());
            }
        }

        public abstract IEnumerable<LtmTestItem> GetChildren();

        [SecurityCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all types exceptions.")]
        TestResult ITestItem.Init()
        {
            try
            {
                RunInAsyncContext(() => TestItem.Init());
                return TestResult.Passed;
            }
            catch (TestSkippedException ex)
            {
                LtmLogger.Instance.HandleException(ex.InnerException ?? ex, TestResult.Skipped);
                return TestResult.Skipped;
            }
            catch (TestFailedException ex)
            {
                this.OutputBugInformation();
                LtmLogger.Instance.HandleException(ex.InnerException, TestResult.Failed);
                return TestResult.Failed;
            }
            catch (Exception ex)
            {
                this.OutputBugInformation();
                LtmLogger.Instance.HandleException(ex, TestResult.Failed);
                return TestResult.Failed;
            }
        }

        [SecurityCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all types exceptions.")]
        TestResult ITestItem.Execute()
        {
            try
            {
                RunInAsyncContext(() => TestItem.Execute());
                return TestResult.Passed;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException is TestSkippedException)
                {
                    LtmLogger.Instance.HandleException(ex.InnerException, TestResult.Skipped);
                    return TestResult.Skipped;
                }
                else if (ex.InnerException is TestFailedException)
                {
                    this.OutputBugInformation();
                    LtmLogger.Instance.HandleException(ex.InnerException, TestResult.Failed);
                    return TestResult.Failed;
                }
                else
                {
                    this.OutputBugInformation();
                    LtmLogger.Instance.HandleException(ex.InnerException ?? ex, TestResult.Failed);
                    return TestResult.Failed;
                }
            }
            catch (Exception ex)
            {
                this.OutputBugInformation();
                LtmLogger.Instance.HandleException(ex, TestResult.Failed);
                return TestResult.Failed;
            }
        }

        [SecurityCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all types exceptions.")]
        TestResult ITestItem.Terminate()
        {
            try
            {
                RunInAsyncContext(() => TestItem.Terminate());
                return TestResult.Passed;
            }
            catch (TestFailedException ex)
            {
                this.OutputBugInformation();
                LtmLogger.Instance.HandleException(ex.InnerException, TestResult.Failed);
                return TestResult.Failed;
            }
            catch (Exception ex)
            {
                this.OutputBugInformation();
                LtmLogger.Instance.HandleException(ex, TestResult.Failed);
                return TestResult.Failed;
            }
        }

        private static void RunInAsyncContext(Action action)
        {
            ReadOnlyCollection<Action<IAsyncContinuation>> actions;

            using (var context = AsyncExecutionContext.Begin())
            {
                action();
                actions = context.GetQueuedActions();
            }

            ManualResetEvent finishedEvent = new ManualResetEvent(false);
            Exception lastException = null;

            var continuation = AsyncHelpers.CreateContinuation(
                () => finishedEvent.Set(),
                ex => { lastException = ex; finishedEvent.Set(); 
                });

            AsyncHelpers.RunActionSequence(continuation, actions);
            finishedEvent.WaitOne();

            if (lastException != null)
            {
                throw lastException;
            }
        }

        private void OutputBugInformation()
        {
            foreach (BugAttribute bug in this.TestItem.Bugs)
            {
                LtmLogger.Instance.WriteLine(LogLevel.Info, bug.ToString());
            }
        }
    }
}
