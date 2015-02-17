//---------------------------------------------------------------------
// <copyright file="DefaultTestModuleLoader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// The default module loader. Recognizes assemblies that
    /// have classes that derive from <see cref="TestModule"/>
    /// and are decorated with the <see cref="TestModuleAttribute"/>.
    /// </summary>
    public class DefaultTestModuleLoader : ITestModuleLoader
    {
        /// <summary>
        /// Determines whether this instance can load the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>
        /// <c>true</c> if this instance can load the specified assemblies; otherwise, <c>false</c>.
        /// </returns>
        public bool CanLoad(IEnumerable<Assembly> assemblies)
        {
            ExceptionUtilities.CheckArgumentNotNull(assemblies, "assemblies");

            return assemblies.Any(
                asm => asm.GetExportedTypes().Any(
                    t => typeof(TestModule).IsAssignableFrom(t) && t.IsDefined(typeof(TestModuleAttribute), false)));
        }

        /// <summary>
        /// Loads test module information from the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to consider.</param>
        /// <param name="explorationSeed">The exploration seed to use when loading test items with a <see cref="TestMatrixAttribute"/>.</param>
        /// <param name="explorationKind">The exploraiton kind to use when loading test items with a <see cref="TestMatrixAttribute"/>.</param>
        /// <returns>
        /// A <see cref="TestModuleData"/> instance representing the
        /// module information found in the specified assemblies.
        /// </returns>
        public TestModuleData Load(IEnumerable<Assembly> assemblies, int explorationSeed, TestMatrixExplorationKind? explorationKind)
        {
            ExceptionUtilities.CheckArgumentNotNull(assemblies, "assemblies");

            List<Type> testModuleTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                List<Type> types = assembly.GetTypes().Where(t => typeof(TestModule).IsAssignableFrom(t)).ToList();
                List<Type> filteredTypes = types.Where(t => t.IsDefined(typeof(TestModuleAttribute), false)).ToList();
                testModuleTypes.AddRange(filteredTypes);
            }

            Type testModuleType = testModuleTypes.FirstOrDefault();

            if (testModuleType == null)
            {
                throw new TaupoArgumentException("The specified assembly does not contain any test modules.");
            }

            var testModule = (TestModule)Activator.CreateInstance(testModuleType);
            testModule.ExplorationSeed = explorationSeed;
            testModule.ExplorationKind = explorationKind;

            return new TestModuleData(testModule);
        }
    }
}
