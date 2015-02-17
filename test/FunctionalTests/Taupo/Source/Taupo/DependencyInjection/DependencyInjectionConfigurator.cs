//---------------------------------------------------------------------
// <copyright file="DependencyInjectionConfigurator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.DependencyInjection;
    using Microsoft.Test.Taupo.Utilities;

    /// <summary>
    /// Configures dependency injection container based on test parameters and default implementation selector.
    /// </summary>
    public class DependencyInjectionConfigurator
    {
        private List<ImplementationInfo> implementations;
        private ImplementationSelector implementationSelector;
        private LogLevelResolver loggerResolver;

        /// <summary>
        /// Initializes a new instance of the DependencyInjectionConfigurator class.
        /// </summary>
        /// <param name="implementationSelector">The implementation selector.</param>
        /// <param name="testParameters">The test parameters.</param>
        public DependencyInjectionConfigurator(ImplementationSelector implementationSelector, IDictionary<string, string> testParameters)
        {
            ExceptionUtilities.CheckArgumentNotNull(implementationSelector, "implementationSelector");
            ExceptionUtilities.CheckArgumentNotNull(testParameters, "testParameters");

            this.TestParameters = new Dictionary<string, string>(testParameters);
            this.RootLogger = Logger.Null;
            this.implementationSelector = implementationSelector;
            this.loggerResolver = new LogLevelResolver(this.TestParameters);
        }

        /// <summary>
        /// Gets or sets the root logger.
        /// </summary>
        /// <value>The root logger.</value>
        public Logger RootLogger { get; set; }

        /// <summary>
        /// Gets the test parameters.
        /// </summary>
        /// <value>The test parameters.</value>
        public IDictionary<string, string> TestParameters { get; private set; }

        /// <summary>
        /// Configures the default dependencies.
        /// </summary>
        /// <param name="dependencyInjectionContainer">The dependency injection container.</param>
        public void ConfigureDefaultDependencies(DependencyInjectionContainer dependencyInjectionContainer)
        {
            ExceptionUtilities.CheckArgumentNotNull(dependencyInjectionContainer, "dependencyInjectionContainer");

            foreach (var param in this.TestParameters)
            {
                dependencyInjectionContainer.TestParameters[param.Key] = param.Value;
            }

            // set up dependency injector
            dependencyInjectionContainer.RegisterCustomResolver(typeof(Logger), this.GetLoggerForType).Transient();
            dependencyInjectionContainer.RegisterInstance<IDependencyInjector>(dependencyInjectionContainer);
            dependencyInjectionContainer.InjectDependenciesInto(dependencyInjectionContainer);
            dependencyInjectionContainer.InjectDependenciesInto(this.implementationSelector);

            this.implementations = this.implementationSelector.GetImplementations(this.TestParameters).ToList();

            // only register dependencies that cannot already be resolved (unless the dependency was specified as a
            // test parameter, in which case override the default)
            foreach (var implInfo in this.implementations.Where(i => i.IsTestParameterSpecified || !dependencyInjectionContainer.CanResolve(i.ContractType)))
            {
                var options = dependencyInjectionContainer.Register(implInfo.ContractType, implInfo.ImplementationType);
                options.IsTransient = implInfo.IsTransient;
            }
        }

        /// <summary>
        /// Initializes the global objects.
        /// </summary>
        /// <param name="dependencyInjectionContainer">The dependency injection container.</param>
        public void InitializeGlobalObjects(DependencyInjectionContainer dependencyInjectionContainer)
        {
            ExceptionUtilities.CheckArgumentNotNull(dependencyInjectionContainer, "dependencyInjectionContainer");

            // pre-resolve global objects
            foreach (var globalObject in this.implementations.Where(c => c.IsGlobal))
            {
                var result = dependencyInjectionContainer.Resolve(globalObject.ContractType);
                IInitializable init = result as IInitializable;
                if (init != null)
                {
                    init.Initialize();
                }
            }
        }

        /// <summary>
        /// Creates Logger object to be injected into the target object based on its type.
        /// </summary>
        /// <param name="targetType">Type of the object whose logger is to be created</param>
        /// <returns>Instance of <see cref="FilteringLogger"/> for the target object</returns>
        /// <remarks>
        /// This method returns instances of loggers which can be easily controlled on LTM command line.
        /// For any class that has a Logger dependency (say ClassName) you can use "LogLevelClassName" parameter
        /// to configure log level for that class.
        /// For example by passing LogLevelCsdlContentGenerator=Verbose you will get verbose logs from 
        /// CSDL Content Generator and so on. The default level for each component is 'Info'.
        /// </remarks>
        private object GetLoggerForType(Type targetType)
        {
            return new FilteringLogger(this.RootLogger, this.loggerResolver.GetLogLevelForType(targetType));
        }
    }
}
