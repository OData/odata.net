//---------------------------------------------------------------------
// <copyright file="TestMatrixAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Attribute for marking a class or variation as being part of a test matrix
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TestMatrixAttribute : TestItemBaseAttribute
    {
        /// <summary>
        /// Initializes a new instance of the TestMatrixAttribute class.
        /// </summary>
        public TestMatrixAttribute()
        {
            this.ExplorationKind = TestMatrixExplorationKind.Pairwise;
        }

        /// <summary>
        /// Gets or sets the kind of exploration to use for the test matrix.
        /// </summary>
        public TestMatrixExplorationKind ExplorationKind { get; set; }
    }
}
