//---------------------------------------------------------------------
// <copyright file="TestConfigurationMatrixExplorer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.TestConfiguration
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.TestConfiguration;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// Test Path type
    /// </summary>
    public class TestConfigurationMatrixExplorer : ITestConfigurationExplorer
    {
        /// <summary>
        /// Gets or sets a RandomNumberGenerator
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IRandomNumberGenerator RandomNumberGenerator { get; set; }

        /// <summary>
        /// Explores the Test Configuration matrix
        /// </summary>
        /// <param name="configurationMatrix">Matrix to explore</param>
        /// <param name="explorationKind">Exploration kind, pairwise or exhaustive</param>
        /// <returns>Rows of the explored matrix</returns>
        public IEnumerable<TestRow> Explore(TestConfigurationMatrix configurationMatrix, TestMatrixExplorationKind explorationKind)
        {
            ExceptionUtilities.CheckArgumentNotNull(configurationMatrix, "configurationMatrix");

            return this.Explore(configurationMatrix, explorationKind, null);
        }

        /// <summary>
        /// Explores the Test Configuration matrix and run strategy
        /// </summary>
        /// <param name="configurationMatrix">Configuration strategy to explore</param>
        /// <param name="testRunStrategyPriority">priority to override matrix dimensions with</param>
        /// <returns>Rows of the explored matrix</returns>
        public IEnumerable<TestRow> Explore(TestConfigurationMatrix configurationMatrix, int testRunStrategyPriority)
        {
            ExceptionUtilities.CheckArgumentNotNull(configurationMatrix, "configurationMatrix");

            var runStrategy = configurationMatrix.TestRunStrategies.Where(s => s.Priority == testRunStrategyPriority).SingleOrDefault();
            ExceptionUtilities.CheckObjectNotNull(runStrategy, "Expected to find a run Strategy with priority:{0}", testRunStrategyPriority);

            return this.Explore(configurationMatrix, runStrategy.ExplorationKind, runStrategy);
        }

        private IEnumerable<TestRow> Explore(TestConfigurationMatrix matrix, TestMatrixExplorationKind explorationKind, TestRunStrategy testRunStrategy)
        {
            List<KeyValuePair<Dimension, IExplorationStrategy>> testShellDimensionsWithStrategies = new List<KeyValuePair<Dimension, IExplorationStrategy>>();
            List<IConstraint> testShellContraints = new List<IConstraint>();

            Dictionary<string, TestDimension> testDimensionsToExpand = new Dictionary<string, TestDimension>();
            foreach (var testDimension in matrix.Dimensions)
            {
                testDimensionsToExpand.Add(testDimension.Name, testDimension);
            }

            // Now override any dimensions for the run Strategy
            if (testRunStrategy != null)
            {
                foreach (var testDimension in testRunStrategy.OverrideDimensions)
                {
                    testDimensionsToExpand[testDimension.Name] = testDimension;
                }
            }
            
            // Add test matrix combinations
            this.GetFlattenDimensionsWithConstraints(testDimensionsToExpand.Values, testShellDimensionsWithStrategies, testShellContraints);
            
            // Create test dimension for the platforms
            var platformDimension = new Dimension<string>("Platform");
            var platformExplorationStrategy = new ExhaustiveIEnumerableStrategy<string>(matrix.TestAssemblies.Select(ta => ta.PlatformType.ToString()));
            testShellDimensionsWithStrategies.Add(new KeyValuePair<Dimension, IExplorationStrategy>(platformDimension, platformExplorationStrategy));

            CombinatorialStrategy testConfigurationExplorationStrategy = null;
            Matrix testShellMatrix = new Matrix(matrix.Name, testShellDimensionsWithStrategies.Select(pair => pair.Key).ToArray());

            if (explorationKind == TestMatrixExplorationKind.Exhaustive)
            {
                testConfigurationExplorationStrategy = new ExhaustiveCombinatorialStrategy(testShellMatrix, testShellContraints);
            }
            else
            {
                testConfigurationExplorationStrategy = new PairwiseStrategy(testShellMatrix, this.RandomNumberGenerator.Next, testShellContraints);
            }
            
            foreach (var ds in testShellDimensionsWithStrategies)
            {
                testConfigurationExplorationStrategy.SetDimensionStrategy(ds.Key, ds.Value);
            }

            List<TestRow> testRows = new List<TestRow>();
            foreach (var vector in testConfigurationExplorationStrategy.Explore())
            {
                Dictionary<string, string> rowValues = new Dictionary<string, string>();
                foreach (var testShellDimensionWithStrategy in testShellDimensionsWithStrategies)
                {
                    rowValues.Add(testShellDimensionWithStrategy.Key.Name, (string)vector.GetValue(testShellDimensionWithStrategy.Key));
                }

                testRows.Add(new TestRow(rowValues));
            }

            return testRows;
        }

        private void GetFlattenDimensionsWithConstraints(IEnumerable<TestDimension> testDimensions, List<KeyValuePair<Dimension, IExplorationStrategy>> flattenedDimensions, List<IConstraint> contraints)
        {
            foreach (TestDimension testDimension in testDimensions)
            {
                var mainDimension = new Dimension<string>(testDimension.Name);
                flattenedDimensions.Add(new KeyValuePair<Dimension, IExplorationStrategy>(mainDimension, new ExhaustiveIEnumerableStrategy<string>(testDimension.Values)));

                this.GetFlattenDimensionsWithConstraints(testDimension.RequiredTestDimensionIfValueEqual.Values, flattenedDimensions, contraints);

                foreach (var dependentValueDimensionPair in testDimension.RequiredTestDimensionIfValueEqual)
                {
                    var dependentDimension = flattenedDimensions.Where(r => r.Key.Name == dependentValueDimensionPair.Value.Name).Select(r2 => r2.Key).SingleOrDefault();
                    ExceptionUtilities.CheckObjectNotNull(dependentDimension, "Should have a dependent dimension");
                    contraints.Add(new ExcludeVectorContraint(mainDimension, dependentDimension, dependentValueDimensionPair.Key, dependentValueDimensionPair.Value.UnspecifiedValue));
                }
            }
        }

        /// <summary>
        /// Represents a class that will exclude particular vectors based on the whether it is specified or not
        /// </summary>
        private class ExcludeVectorContraint : IConstraint
        {
            private List<QualifiedDimension> requiredDimensions;
            private Dimension mainDimension;
            private Dimension dependentDimension;
            private string unspecifiedValue = null;
            private string mainDimensionRequiredValue = null;

            public ExcludeVectorContraint(Dimension mainDimension, Dimension dependentDimension, string mainDimensionRequiredValue, string unspecifiedValue)
            {
                this.mainDimensionRequiredValue = mainDimensionRequiredValue;
                this.unspecifiedValue = unspecifiedValue;
                this.mainDimension = mainDimension;
                this.dependentDimension = dependentDimension;
                this.requiredDimensions = new List<QualifiedDimension>();
                this.requiredDimensions.Add(this.mainDimension);
                this.requiredDimensions.Add(this.dependentDimension);
            }

            public ReadOnlyCollection<QualifiedDimension> RequiredDimensions
            {
                get { return this.requiredDimensions.AsReadOnly(); }
            }

            public bool IsValid(Vector target)
            {
                foreach (QualifiedDimension d in this.RequiredDimensions)
                {
                    ExceptionUtilities.Assert(target.HasValue(d), "vector doesn't have a value for dimension {0}", d.FullyQualifiedName);
                }

                string mainDimensionValue = target.GetValueString(this.mainDimension);
                string dependentDimensionValue = target.GetValueString(this.dependentDimension);

                if (mainDimensionValue.Equals(this.mainDimensionRequiredValue))
                {
                    if (dependentDimensionValue.Equals(this.unspecifiedValue))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!dependentDimensionValue.Equals(this.unspecifiedValue))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
