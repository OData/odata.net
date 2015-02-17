//---------------------------------------------------------------------
// <copyright file="TestRunStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.TestConfiguration
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// Strategy defines the priority and exploration information used to determine which tests to run
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable exists for easy creation of objects")]
    public class TestRunStrategy : IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the TestRunStrategy class.
        /// </summary>
        /// <param name="priority">Priority of the Strategy</param>
        /// <param name="explorationKind">Exploration kind</param>
        public TestRunStrategy(int priority, TestMatrixExplorationKind explorationKind)
        {
            this.Priority = priority;
            this.ExplorationKind = explorationKind;
            this.OverrideDimensions = new List<TestDimension>();
        }

        /// <summary>
        /// Gets the Priority for a particular Run Strategy
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// Gets the Exploration king
        /// </summary>
        public TestMatrixExplorationKind ExplorationKind { get; private set; }

        /// <summary>
        /// Gets a list of dimensions to override for that priority for exploring
        /// </summary>
        public IList<TestDimension> OverrideDimensions { get; private set; }

        /// <summary>
        /// Dimension to add to the list of overriding ones
        /// </summary>
        /// <param name="dimension">Dimension to add</param>
        public void Add(TestDimension dimension)
        {
            this.OverrideDimensions.Add(dimension);
        }

        /// <summary>
        /// This method is not supported and throws <see cref="TaupoNotSupportedException"/>
        /// </summary>
        /// <returns>This method never returns a result.</returns>
        public IEnumerator GetEnumerator()
        {
            throw new TaupoNotSupportedException(ExceptionUtilities.EnumerableNotImplementedExceptionMessage);
        }
    }
}
