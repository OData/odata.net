//---------------------------------------------------------------------
// <copyright file="TestConfigurationMatrix.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.TestConfiguration
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Test Matrix defines the full set of tests, platforms, dimensions and the exploration strategies that will be run
    /// either on a machine or in a lab
    /// </summary>
    public class TestConfigurationMatrix : IAnnotatable<TestConfigurationMatrixAnnotation>
    {
        /// <summary>
        /// Initializes a new instance of the TestConfigurationMatrix class.
        /// </summary>
        /// <param name="name">Name of the test matrix</param>
        /// <param name="owner">Owner of the test configuration</param>
        /// <param name="backupOwner">backup owner of the test configuration</param>
        public TestConfigurationMatrix(string name, string owner, string backupOwner)
            : this(name, owner, backupOwner, new List<TestAssembly>(), new List<TestRunStrategy>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the TestConfigurationMatrix class.
        /// </summary>
        /// <param name="name">Name of the test matrix</param>
        /// <param name="owner">Owner of the test configuration</param>
        /// <param name="backupOwner">backup owner of the test configuration</param>
        /// <param name="testAssemblies">test assemblies in the configuration</param>
        /// <param name="testRunStrategies">run strategies in the configuration</param>
        public TestConfigurationMatrix(string name, string owner, string backupOwner, IEnumerable<TestAssembly> testAssemblies, IEnumerable<TestRunStrategy> testRunStrategies)
        {
            ExceptionUtilities.CheckArgumentNotNull(name, "name");
            ExceptionUtilities.CheckArgumentNotNull(owner, "owner");
            ExceptionUtilities.CheckArgumentNotNull(backupOwner, "backupOwner");
            ExceptionUtilities.CheckArgumentNotNull(testAssemblies, "testAssemblies");
            ExceptionUtilities.CheckArgumentNotNull(testRunStrategies, "testRunStrategies");

            this.Name = name;
            this.PrimaryOwner = owner;
            this.BackupOwner = backupOwner;
            this.TestPaths = new List<TestPath>();
            this.Dimensions = new List<TestDimension>();
            this.TestRunStrategies = new List<TestRunStrategy>(testRunStrategies);
            this.TestAssemblies = new List<TestAssembly>(testAssemblies);
            this.Annotations = new List<TestConfigurationMatrixAnnotation>();
        }

        /// <summary>
        /// Gets the test specific annotations, for example
        /// </summary>
        public IList<TestConfigurationMatrixAnnotation> Annotations { get; private set; }

        /// <summary>
        /// Gets the test assemblies that the tests live in, no more than one per PlatformType
        /// </summary>
        public IList<TestAssembly> TestAssemblies { get; private set; }

        /// <summary>
        /// Gets the Dimensions to run the tests against
        /// </summary>
        public IList<TestDimension> Dimensions { get; private set; }

        /// <summary>
        /// Gets the Paths that represent the set of tests to run
        /// </summary>
        public IList<TestPath> TestPaths { get; private set; }

        /// <summary>
        /// Gets the Exploration stategies of how tests will run based on the priority level
        /// </summary>
        public IList<TestRunStrategy> TestRunStrategies { get; private set; }
        
        /// <summary>
        /// Gets the name of the test matrix
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the Owner of the Test Configuration
        /// </summary>
        public string PrimaryOwner { get; private set; }

        /// <summary>
        /// Gets the Backup owner of the Test Configuration
        /// </summary>
        public string BackupOwner { get; private set; }
    }
}
