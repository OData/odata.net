//---------------------------------------------------------------------
// <copyright file="ITestConfigurationExplorer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.TestConfiguration
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// Represents a test configuration row
    /// </summary>
    public interface ITestConfigurationExplorer
    {
        /// <summary>
        /// Explores the Test Configuration matrix
        /// </summary>
        /// <param name="configurationMatrix">Matrix to explore</param>
        /// <param name="explorationKind">Exploration kind, exhaustive or pairwise</param>
        /// <returns>Rows of the explored matrix</returns>
        IEnumerable<TestRow> Explore(TestConfigurationMatrix configurationMatrix, TestMatrixExplorationKind explorationKind);

        /// <summary>
        /// Explores the Test Configuration matrix and run strategy
        /// </summary>
        /// <param name="configurationMatrix">Configuration strategy to explore</param>
        /// <param name="testRunStrategyPriority">priority to override matrix dimensions with</param>
        /// <returns>Rows of the explored matrix</returns>
        IEnumerable<TestRow> Explore(TestConfigurationMatrix configurationMatrix, int testRunStrategyPriority);
    }
}
