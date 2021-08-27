//---------------------------------------------------------------------
// <copyright file="RunnerBridge.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Security;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// A bridge between the UI for a runner and the actual
    /// <see cref="ITestModuleRunner"/> used to run the <see cref="TestModule"/>.
    /// Useful for specifying an execution context for the
    /// <see cref="ITestModuleRunner"/> to which this class delegates.
    /// </summary>
    public class RunnerBridge : MarshalByRefObject, ITestModuleRunner
    {
        private ITestModuleRunner runner;
        private Type runnerType;

        /// <summary>
        /// Initializes a new instance of the <see cref="RunnerBridge"/> class.
        /// </summary>
        /// <param name="runnerType">The type of the <see cref="ITestModuleRunner"/> to which to delegate.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "runnerType", Justification = "runnerType is a parameter name.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ITestModuleRunner", Justification = "davidewi: ITestModuleRunner is the name of an interface.")]
        public RunnerBridge(Type runnerType)
        {
            if (!typeof(ITestModuleRunner).IsAssignableFrom(runnerType))
            {
                throw new TaupoArgumentException("The runnerType parameter must be a System.Type that is assignable to Microsoft.Test.Taupo.Runners.ITestModuleRunner.");
            }

            this.runnerType = runnerType;
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
        /// Occurs when the test run has completed.
        /// </summary>
        public event EventHandler<RunStatisticsUpdatedEventArgs> RunStatisticsUpdated;

        /// <summary>
        /// Gets or sets the factory for creating objects in a sandbox.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ISandboxObjectFactory SandboxObjectFactory { get; set; }

        private ITestModuleRunner Runner
        {
            get
            {
                if (this.runner == null)
                {
                    this.runner = (ITestModuleRunner)this.SandboxObjectFactory.CreateInstance(this.runnerType);
                    this.runner.RunCompleted += this.OnRunCompleted;
                    this.runner.RunStatisticsUpdated += this.OnRunStatisticsUpdated;
                    this.runner.TestItemCompleted += this.OnTestItemCompleted;
                    this.runner.TestItemStarting += this.OnTestItemStarting;
                }

                return this.runner;
            }
        }

        /// <summary>
        /// Attempts to abort execution of the test module.
        /// </summary>
        public void Abort()
        {
            this.Runner.Abort();
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
        /// <param name="parameters">The test parameters with which to invoke the test module.</param>
        public void Run(TestModuleData testModuleData, ITestLogWriter logWriter, IEnumerable<TestItemData> variationsToRun, IDictionary<string, string> parameters)
        {
            this.Runner.Run(testModuleData, logWriter, variationsToRun, parameters);
        }

        /// <summary>
        /// Raises the <see cref="RunCompleted"/> event. Not intended to be used publically.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This must be public in order to handle an event raised from a
        /// partial trust <see cref="AppDomain"/>.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnRunCompleted(object sender, EventArgs e)
        {
            var handler = this.RunCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RunStatisticsUpdated"/> event. Not intended to be used publically.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Microsoft.Test.Taupo.Runners.RunStatisticsUpdatedEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This must be public in order to handle an event raised from a
        /// partial trust <see cref="AppDomain"/>.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnRunStatisticsUpdated(object sender, RunStatisticsUpdatedEventArgs e)
        {
            var handler = this.RunStatisticsUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="TestItemCompleted"/> event. Not intended to be used publically.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Microsoft.Test.Taupo.Runners.TestItemStatusChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This must be public in order to handle an event raised from a
        /// partial trust <see cref="AppDomain"/>.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnTestItemCompleted(object sender, TestItemStatusChangedEventArgs e)
        {
            var handler = this.TestItemCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="TestItemStarting"/> event. Not intended to be used publically.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Microsoft.Test.Taupo.Runners.TestItemStatusChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This must be public in order to handle an event raised from a
        /// partial trust <see cref="AppDomain"/>.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnTestItemStarting(object sender, TestItemStatusChangedEventArgs e)
        {
            var handler = this.TestItemStarting;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
