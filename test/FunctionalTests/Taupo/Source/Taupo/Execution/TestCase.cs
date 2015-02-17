//---------------------------------------------------------------------
// <copyright file="TestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.DependencyInjection;

    /// <summary>
    /// Base class for all test cases.
    /// </summary>
    public abstract class TestCase : TestItem
    {
        private DependencyInjectionContainer testCaseDependencyInjectionContainer;

        /// <summary>
        /// Initializes a new instance of the TestCase class and reads metadata from <see cref="TestCaseAttribute"/>
        /// </summary>
        protected TestCase()
        {
            this.Log = Logger.Null;
            this.ReadMetadataFromAttribute();
        }

        /// <summary>
        /// Gets or sets logger to be used for printing diagnostics messages.
        /// </summary>
        [InjectDependency]
        public Logger Log { get; set; }

        /// <summary>
        /// Gets or sets assertion class to be used.
        /// </summary>
        [InjectDependency]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Gets or sets random number generator to be used by tests.
        /// </summary>
        [InjectDependency]
        public IRandomNumberGenerator Random { get; set; }

        /// <summary>
        /// Initializes test item. Will be invoked before test item is executed.
        /// </summary>
        public override void Init()
        {
            base.Init();

            this.ConfigureDependencies(this.testCaseDependencyInjectionContainer);
            this.ConfigureChildTestItems();
        }

        /// <summary>
        /// Cleans up after test item execution.
        /// </summary>
        public override void Terminate()
        {
            if (this.testCaseDependencyInjectionContainer != null)
            {
                this.testCaseDependencyInjectionContainer.Dispose();
                this.testCaseDependencyInjectionContainer = null;
            }

            this.Log = Logger.Null;

            base.Terminate();
        }

        internal void InjectDependencies(DependencyInjectionContainer parentContainer)
        {
            this.testCaseDependencyInjectionContainer = parentContainer.CreateInheritedContainer();
        }

        /// <summary>
        /// Configures the dependencies for the test case.
        /// </summary>
        /// <param name="container">The container (private to the test case).</param>
        /// <remarks>By default dependencies are inherited from the parent test case or module.</remarks>
        protected virtual void ConfigureDependencies(DependencyInjectionContainer container)
        {
        }

        /// <summary>
        /// Configures the child test items in the test case
        /// </summary>
        protected virtual void ConfigureChildTestItems()
        {
            this.testCaseDependencyInjectionContainer.RegisterInstance<IDependencyInjector>(this.testCaseDependencyInjectionContainer);
            this.testCaseDependencyInjectionContainer.InjectDependenciesInto(this);
            this.SetupTestCaseSpecificWorkspace();
            AsyncExecutionContext.EnqueueSynchronousAction(
                () =>
                {
                    this.testCaseDependencyInjectionContainer.InjectDependenciesInto(this);
                    this.InjectDependenciesIntoChildren();
                });
        }

        /// <summary>
        /// Gets the workspace builder which can override the workspace for the test case.
        /// </summary>
        /// <returns>Instance of <see cref="IWorkspaceBuilder" /> or null if no custom workspace is needed.</returns>
        protected virtual IWorkspaceBuilder GetWorkspaceBuilder()
        {
            return null;
        }

        /// <summary>
        /// Injects dependencies into the specified object using currently configured dependency injector.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="targetObject">The object to inject dependencies into.</param>
        /// <returns>Input object</returns>
        protected T InjectDependenciesInto<T>(T targetObject)
            where T : class
        {
            return this.testCaseDependencyInjectionContainer.InjectDependenciesInto(targetObject);
        }

        private void InjectDependenciesIntoChildren()
        {
            foreach (TestCase child in this.Children.OfType<TestCase>())
            {
                child.InjectDependencies(this.testCaseDependencyInjectionContainer);
            }
        }

        private void ReadMetadataFromAttribute()
        {
            // Apply the metadata from the first attribute. If the test case has multiple attributes specified, the metadata can be rebuilt from the specific attribute.
            var tca = (TestCaseAttribute)this.GetType().GetCustomAttributes(typeof(TestCaseAttribute), true).FirstOrDefault();
            this.Metadata.Name = this.Metadata.Description = this.GetType().Name;

            if (tca != null)
            {
                this.ReadMetadataFromAttribute(tca);
            }
        }

        private void SetupTestCaseSpecificWorkspace()
        {
            IWorkspaceBuilder workspaceBuilder = this.GetWorkspaceBuilder();
            if (workspaceBuilder != null)
            {
                this.testCaseDependencyInjectionContainer.InjectDependenciesInto(workspaceBuilder);
                Workspace workspace = workspaceBuilder.BuildWorkspace(null);

                this.testCaseDependencyInjectionContainer.RegisterInstance(workspace);
            }
        }
    }
}
