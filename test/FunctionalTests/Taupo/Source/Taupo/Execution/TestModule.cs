//---------------------------------------------------------------------
// <copyright file="TestModule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.DependencyInjection;

    /// <summary>
    /// Base class for all test modules.
    /// </summary>
    public abstract class TestModule : TestItem
    {
        private ILogLevelResolver loggerResolver;
        private Logger rootLogger;
        private IDependencyInjector dependencyInjector;
        private DependencyInjectionContainer moduleDependencyInjectionContainer;

        /// <summary>
        /// Initializes a new instance of the TestModule class, initializes default dependencies
        /// and reads metadata from <see cref="TestModuleAttribute"/>.
        /// </summary>
        protected TestModule()
        {
            // default dependencies
            this.Log = Logger.Null;
            this.TestParameters = new Dictionary<string, string>();
            this.Assert = new DefaultAssertionHandler(this.Log);
            this.loggerResolver = new LogLevelResolver(this.TestParameters);
            this.ReadMetadataFromAttribute();
            this.rootLogger = Logger.Null;
        }

        /// <summary>
        /// Occurs when the test module is initialized.
        /// </summary>
        public event EventHandler OnInitialize;

        /// <summary>
        /// Gets or sets logger to be used by the test module.
        /// </summary>
        [InjectDependency]
        public Logger Log { get; set; }

        /// <summary>
        /// Gets or sets assertion handler used by test module and all child test items.
        /// </summary>
        [InjectDependency]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Gets the test parameters.
        /// </summary>
        /// <value>The test parameters.</value>
        public IDictionary<string, string> TestParameters { get; private set; }

        /// <summary>
        /// Initializes the test module.
        /// </summary>
        /// <remarks>
        /// Can be overridden by derived classes to provide extra initialization.
        /// </remarks>
        public override void Init()
        {
            this.rootLogger = this.Log;

            // this is to give LTM a chance to set up our TestParameters (which are not available until just before
            // Init()
            var onInit = this.OnInitialize;
            if (onInit != null)
            {
                onInit(this, new EventArgs());
            }

            if (this.TestParameters.ContainsKey("Help"))
            {
                this.ShowHelp();
                throw new TestSkippedException("Help");
            }

            this.Log.WriteLine(LogLevel.Verbose, typeof(TestModule).Name + " is initializing. Pass Help=true parameter to display list of available options.");
            this.Log.WriteLine(LogLevel.Verbose, "Module-level exploration seed was: {0}", this.ExplorationSeed);
            this.ConfigureDependencyInjectionContainer();
            this.SetupDefaultWorkspace();
            AsyncExecutionContext.EnqueueSynchronousAction(() => this.InjectDependenciesIntoChildren(this));
        }

        /// <summary>
        /// Cleans up after test item execution.
        /// </summary>
        public override void Terminate()
        {
            if (this.moduleDependencyInjectionContainer != null)
            {
                this.moduleDependencyInjectionContainer.Dispose();
                this.moduleDependencyInjectionContainer = null;
            }

            this.rootLogger = null;

            base.Terminate();
        }

        /// <summary>
        /// Gets the available parameters for this test module.
        /// </summary>
        /// <returns>Array of <see cref="TestParameterInfo"/> that describes allowed test parameters for the test module.</returns>
        /// <remarks>The parameters are determined by scanning all Taupo and test assemblies
        /// and examining <see cref="InjectTestParameterAttribute"/>, <see cref="ImplementationSelectorAttribute"/>.
        /// </remarks>
        public TestParameterInfo[] GetAvailableParameters()
        {
            ImplementationSelector dis = new ImplementationSelector();
            this.ConfigureImplementationSelector(dis);

            HelpGenerator hg = new HelpGenerator(this.loggerResolver);
            return hg.GetAvailableParameters(dis.Types);
        }

        /// <summary>
        /// Gets the workspace builder for the test module.
        /// </summary>
        /// <returns>Instance of <see cref="IWorkspaceBuilder" /> or null if no workspace is needed.</returns>
        protected virtual IWorkspaceBuilder GetWorkspaceBuilder()
        {
            return null;
        }

        /// <summary>
        /// Sets up dependecies for this <see cref="TestModule"/>
        /// </summary>
        /// <param name="container">Dependency injection container.</param>
        protected virtual void ConfigureDependencies(DependencyInjectionContainer container)
        {
        }

        /// <summary>
        /// Adds required assemblies to the <see cref="ImplementationSelector"/>
        /// </summary>
        /// <param name="implementationSelector">Selector to be configured.</param>
        protected virtual void ConfigureImplementationSelector(IImplementationSelector implementationSelector)
        {
            // add this assembly and all satellite assemblies to DefaultImplementationSelector
            implementationSelector.AddAssembly(typeof(TestModule).GetAssembly());
        }

        private void ShowHelp()
        {
            ImplementationSelector dis = new ImplementationSelector();
            this.ConfigureImplementationSelector(dis);

            HelpGenerator hg = new HelpGenerator(this.loggerResolver);
            string helpText = hg.GetHelpText(dis.Types);
            this.rootLogger.WriteLine(LogLevel.Info, helpText);
        }

        private void ReadMetadataFromAttribute()
        {
            var tma = PlatformHelper.GetCustomAttribute<TestModuleAttribute>(this.GetType());
            var metadata = this.Metadata;

            metadata.Name = metadata.Description = this.GetType().Name;

            if (tma != null)
            {
                this.ReadMetadataFromAttribute(tma);
            }
        }

        private void InjectDependenciesIntoChildren(TestItem item)
        {
            this.dependencyInjector.InjectDependenciesInto(item);
            foreach (TestCase child in item.Children.OfType<TestCase>())
            {
                child.InjectDependencies(this.moduleDependencyInjectionContainer);
            }
        }

        private void ConfigureDependencyInjectionContainer()
        {
            this.dependencyInjector = this.moduleDependencyInjectionContainer = new LightweightDependencyInjectionContainer();
            var implementationSelector = new ImplementationSelector();
            this.ConfigureImplementationSelector(implementationSelector);
            var configurator = new DependencyInjectionConfigurator(implementationSelector, this.TestParameters);
            configurator.RootLogger = this.rootLogger;
            configurator.ConfigureDefaultDependencies(this.moduleDependencyInjectionContainer);
            this.ConfigureDependencies(this.moduleDependencyInjectionContainer);
            this.dependencyInjector.InjectDependenciesInto(this);
            configurator.InitializeGlobalObjects(this.moduleDependencyInjectionContainer);
        }

        private void SetupDefaultWorkspace()
        {
            IWorkspaceBuilder workspaceBuilder = this.GetWorkspaceBuilder();
            if (workspaceBuilder != null)
            {
                this.dependencyInjector.InjectDependenciesInto(workspaceBuilder);
                Workspace workspace = workspaceBuilder.BuildWorkspace(null);
                for (Type t = workspace.GetType(); t != typeof(object); t = t.GetBaseType())
                {
                    this.moduleDependencyInjectionContainer.RegisterInstance(t, workspace);
                }
            }
        }
    }
}
