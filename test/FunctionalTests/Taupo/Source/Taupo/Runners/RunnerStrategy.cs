//---------------------------------------------------------------------
// <copyright file="RunnerStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.DependencyInjection;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// This type describes a strategy to create a test runner
    /// </summary>
    public abstract class RunnerStrategy
    {
        private const string ExplorationSeedParameterName = "TestExplorationSeed";
        private const string ExplorationKindParameterName = "TestExplorationKind";

        private ICollection<ITestModuleLoader> loaders = new List<ITestModuleLoader>
        {
            new DefaultTestModuleLoader()
        };

        private IDictionary<string, string> parameters;

        /// <summary>
        /// Initializes a new instance of the RunnerStrategy class
        /// </summary>
        protected RunnerStrategy()
        {
        }

        /// <summary>
        /// Initializes a new instance of the RunnerStrategy class using a collection of <see cref="ITestModuleLoader"/>s
        /// </summary>
        /// <param name="loaders"><see cref="ITestModuleLoader"/>s to use.</param>
        protected RunnerStrategy(params ITestModuleLoader[] loaders)
        {
            this.loaders = loaders;
        }

        /// <summary>
        /// Gets or sets the Logger to use for test execution
        /// </summary>
        public ITestLogWriter TestLogger { get; set; }

        /// <summary>
        /// Gets the test parameters injected into the tests
        /// </summary>
        public IDictionary<string, string> Parameters
        {
            get
            {
                if (this.parameters == null)
                {
                    this.parameters = new Dictionary<string, string>();
                }

                return this.parameters;
            }
        }

        /// <summary>
        /// Gets the test suites to load as part of this test run
        /// </summary>
        protected Collection<string> SuitesToLoad { get; private set; }

        /// <summary>
        /// Creates an instance of the ITestModuleRunner 
        /// </summary>
        /// <param name="runnerType">The type of the runner to create</param>
        /// <returns>An implementation of the ITestModuleRunner </returns>
        public ITestModuleRunner CreateRunner(Type runnerType)
        {
            LightweightDependencyInjectionContainer dependencyInjectionContainer = null;
            ITestModuleRunner runner = null;
            try
            {
                runner = new RunnerBridge(runnerType);
                dependencyInjectionContainer = new LightweightDependencyInjectionContainer();
                var implementationSelector = new ImplementationSelector();
                implementationSelector.AddAssembly(typeof(TestItem).GetAssembly());

                var helpGenerator = new HelpGenerator(new LogLevelResolver(this.Parameters));
                var availableParameters = helpGenerator.GetAvailableParameters(implementationSelector.Types);
                Dictionary<string, string> availableParametersLookup = this.Parameters.Where(kvp => availableParameters.Any(p => p.ParameterName == kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                // Any TestParameters passed in are not used for injecting dependencies into RunnerBridge
                var configurator = new DependencyInjectionConfigurator(
                    implementationSelector,
                    availableParametersLookup);

                configurator.RootLogger = Logger.Null;
                configurator.ConfigureDefaultDependencies(dependencyInjectionContainer);

                dependencyInjectionContainer.InjectDependenciesInto(runner);
            }
            finally
            {
                if (dependencyInjectionContainer != null)
                {
                    dependencyInjectionContainer.Dispose();
                }
            }

            return runner;
        }

        /// <summary>
        /// Loads the default test suite for this run.
        /// </summary>
        /// <param name="continuation">The function to call after the suite is loaded</param>
        public virtual void LoadDefaultTestSuite(Action<TestSuite, TestModuleData> continuation)
        {
            if (this.SuitesToLoad != null)
            {
                string suiteName = this.SuitesToLoad.FirstOrDefault();
                this.LoadTestSuite(suiteName, continuation);
            }
        }

        /// <summary>
        /// Loads the test suite for this run.
        /// </summary>
        /// <param name="suiteName"> The name of the suite to load</param>
        /// <param name="continuation">The function to call after the suite is loaded</param>
        public virtual void LoadTestSuite(string suiteName, Action<TestSuite, TestModuleData> continuation)
        {
            string suiteFileName = Path.GetFullPath(suiteName);

            TestSuite suite;
            if (Path.GetExtension(suiteName).Equals(".dll", StringComparison.OrdinalIgnoreCase))
            {
                suite = new TestSuite();
                suite.Assemblies.Add(Path.GetFileNameWithoutExtension(suiteName));
            }
            else
            {
                using (XmlReader reader = XmlReader.Create(suiteFileName))
                {
                    suite = TestSuiteUtilities.LoadFrom(reader);
                }
            }

            string assemblyDirectory = Path.GetDirectoryName(suiteFileName);
            LoadTestSuite(suite, continuation, assemblyDirectory);
        }

        /// <summary>
        /// Loads the test suite for this run from a pre-constructed TestSuite.
        /// </summary>
        /// <param name="suite">The test suite to finish loading</param>
        /// <param name="continuation">The function to call after the suite is loaded</param>
        /// <param name="assemblyDirectory">The folder containing the test assemblies</param>
        public virtual void LoadTestSuite(TestSuite suite, Action<TestSuite, TestModuleData> continuation, string assemblyDirectory)
        {
            // Copy the parameters from the suite into our current cache of parameters.
            // This is useful for parameters that are used much earlier than normal
            // in the pipeline, like TestExplorationSeed.
            foreach (var kvp in suite.Parameters)
            {
                this.Parameters[kvp.Key] = kvp.Value;
            }

            List<Assembly> assemblies = new List<Assembly>();

            foreach (string asm in suite.Assemblies)
            {
                string fullPath = Path.Combine(assemblyDirectory, asm + ".dll");
                var assembly = AssemblyHelpers.LoadAssembly(fullPath, assemblyDirectory);
                assemblies.Add(assembly);
            }

            var module = this.FindTestModule(assemblies);
            continuation(suite, module);
        }

        /// <summary>
        /// Creates a textwriter to log to the log file
        /// </summary>
        /// <param name="logName">The name of log file to create</param>
        /// <returns>A textwriter to log to the log file</returns>
        public virtual TextWriter CreateLogTextWriter(string logName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(logName, "logName");
            return File.CreateText(logName);
        }

        /// <summary>
        /// Parses the command-line switches into test parameters
        /// </summary>
        /// <param name="args">The string array representing test parameters</param>
        /// <returns>A Key value pair of test parameters</returns>
        protected IDictionary<string, string> ParseParameters(string[] args)
        {
            var testParameters = new Dictionary<string, string>();
            if (args.Length > 0)
            {
                string suiteParameter = args[0].ToUpperInvariant().Replace("/SUITE:", string.Empty);
                this.SetSuitesToLoad(suiteParameter.Split(','));
            }

            for (int i = 1; i < args.Length; ++i)
            {
                string arg = args[i];
                if (arg.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    int p = arg.IndexOf(':');
                    string key;
                    string value;
                    if (p < 0)
                    {
                        key = arg.Substring(1);
                        value = args[++i];
                    }
                    else
                    {
                        key = arg.Substring(1, p - 1);
                        value = arg.Substring(p + 1);
                    }

                    testParameters.Add(key, value);
                }
            }

            return testParameters;
        }

        /// <summary>
        /// Writes a plain text message to the test loggers
        /// </summary>
        /// <param name="statusText">A plain text message</param>
        protected abstract void WriteStatusText(string statusText);

        /// <summary>
        /// Finds the test module to execute in the loaded assemblies
        /// </summary>
        /// <param name="loadedAssemblies">The assemblies loaded in the current application</param>
        /// <returns>The test module to execute</returns>
        protected TestModuleData FindTestModule(IEnumerable<Assembly> loadedAssemblies)
        {
            ExceptionUtilities.CheckArgumentNotNull(loadedAssemblies, "loadedAssemblies");
            ExceptionUtilities.Assert(loadedAssemblies.Any(), "No loaded assemblies to run tests from");
            TestModuleData testModuleData = null;

            ITestModuleLoader testModuleLoader = this.loaders.FirstOrDefault(testLoader => testLoader.CanLoad(loadedAssemblies));
            ExceptionUtilities.CheckObjectNotNull(testModuleLoader, "could not find a suitable test loader");

            int explorationSeed = this.GetExplorationSeed();
            var explorationKind = this.GetExplorationKind();

            this.WriteStatusText(string.Format(CultureInfo.InvariantCulture, "{0}={1}", ExplorationSeedParameterName, explorationSeed));
            if (explorationKind.HasValue)
            {
                this.WriteStatusText(string.Format(CultureInfo.InvariantCulture, "{0}={1}", ExplorationKindParameterName, explorationKind));
            }

            testModuleData = testModuleLoader.Load(loadedAssemblies, explorationSeed, explorationKind);

            return testModuleData;
        }

        /// <summary>
        /// Sets the test suites to load in this test run
        /// </summary>
        /// <param name="suitesToLoad">The test suites to load in this test run</param>
        protected void SetSuitesToLoad(string[] suitesToLoad)
        {
            this.SuitesToLoad = new Collection<string>(suitesToLoad);
        }

        private int GetExplorationSeed()
        {
            int explorationSeed = (int)DateTime.Now.Ticks;

            string explorationSeedString = null;
            if (this.Parameters.TryGetValue(ExplorationSeedParameterName, out explorationSeedString))
            {
                ExceptionUtilities.Assert(
                    int.TryParse(explorationSeedString, out explorationSeed),
                    "Failed to parse value '{0}' for the '{1}' parameter. Make sure it's a valid integer.",
                    explorationSeedString,
                    ExplorationSeedParameterName);
            }

            return explorationSeed;
        }

        private TestMatrixExplorationKind? GetExplorationKind()
        {
            string explorationKindString = null;
            if (this.Parameters.TryGetValue(ExplorationKindParameterName, out explorationKindString))
            {
                TestMatrixExplorationKind explorationKind;
                ExceptionUtilities.Assert(
                    this.TryParseEnumValue(explorationKindString, out explorationKind),
                    "Failed to parse value '{0}' for the '{1}' parameter. Valid values are: {2}.",
                    explorationKindString,
                    ExplorationKindParameterName,
                    string.Join(", ", DataUtilities.GetEnumValues<TestMatrixExplorationKind>().Select(v => v.ToString()).OrderBy(s => s)));

                return explorationKind;
            }

            return null;
        }

        private bool TryParseEnumValue<TEnum>(string value, out TEnum result) where TEnum : struct
        {
            return Enum.TryParse<TEnum>(value, true, out result);
        }
    }
}
