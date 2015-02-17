//---------------------------------------------------------------------
// <copyright file="ITestModuleLoader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Handles the loading of test module information from assemblies.
    /// </summary>
    public interface ITestModuleLoader
    {
        /// <summary>
        /// Determines whether this loader can load the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to consider.</param>
        /// <returns>
        /// <c>true</c> if this loader can load the specified assemblies; otherwise, <c>false</c>.
        /// </returns>
        bool CanLoad(IEnumerable<Assembly> assemblies);

        /// <summary>
        /// Loads test module information from the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to consider.</param>
        /// <param name="explorationSeed">The exploration seed to use when loading test items with a <see cref="TestMatrixAttribute"/>.</param>
        /// <param name="explorationKind">The exploraiton kind to use when loading test items with a <see cref="TestMatrixAttribute"/>.</param>
        /// <returns>A <see cref="TestModuleData"/> instance representing the
        /// module information found in the specified assemblies.</returns>
        TestModuleData Load(IEnumerable<Assembly> assemblies, int explorationSeed, TestMatrixExplorationKind? explorationKind);
    }
}
