//---------------------------------------------------------------------
// <copyright file="DimensionHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.TestConfiguration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// DimensionHelper has methods that are used to quickly construct Dimensions
    /// that contain a TestShell Dimension and an exploration strategy to get its values
    /// </summary>
    public static class DimensionHelper
    {
        /// <summary>
        /// Gets an Implementation by its tag and selector name
        /// </summary>
        /// <param name="selectorName">Selector name</param>
        /// <param name="assemblies">assemblies to look in</param>
        /// <param name="tagNames">tag names to look for</param>
        /// <returns>A selector name followed by its values</returns>
        public static TestDimension GetImplementationsByNameAndTag(string selectorName, IEnumerable<Assembly> assemblies, params string[] tagNames)
        {
            ExceptionUtilities.CheckArgumentNotNull(selectorName, "selectorName");
            ExceptionUtilities.CheckArgumentNotNull(assemblies, "assemblies");
            ExceptionUtilities.CheckCollectionNotEmpty(assemblies, "assemblies");

            var implementations = GetImplementationsBySelectorName(selectorName, assemblies.SelectMany(s => s.GetTypes()));

            return new TestDimension(selectorName, implementations.Where(s1 => tagNames.Any(t => s1.Tags.Contains(t))).Select(s => s.ImplementationName).ToArray());
        }

        /// <summary>
        /// Gets an implementation by its name
        /// </summary>
        /// <param name="selectorName">Name of selector to find</param>
        /// <param name="assemblies">Assemblies to look in</param>
        /// <returns>selector name and its values</returns>
        public static TestDimension GetImplementationsByName(string selectorName, IEnumerable<Assembly> assemblies)
        {
            ExceptionUtilities.CheckArgumentNotNull(selectorName, "selectorName");
            ExceptionUtilities.CheckArgumentNotNull(assemblies, "assemblies");
            ExceptionUtilities.CheckCollectionNotEmpty(assemblies, "assemblies");

            var implementations = GetImplementationsBySelectorName(selectorName, assemblies.SelectMany(s => s.GetTypes()));

            return new TestDimension(selectorName, implementations.Select(s => s.ImplementationName).ToArray());
        }

        private static IList<ImplementationNameAttribute> GetImplementationsBySelectorName(string selectorName, IEnumerable<Type> typeClosure)
        {
            List<ImplementationNameAttribute> implementations = new List<ImplementationNameAttribute>();
            Type foundContractType = null;

            foreach (Type type in typeClosure)
            {
                var isa = PlatformHelper.GetCustomAttribute<ImplementationSelectorAttribute>(type, false);
                if (isa != null && isa.TestArgumentName == selectorName)
                {
                    foundContractType = type;
                    break;
                }
            }

            if (foundContractType == null)
            {
                throw new TaupoInfrastructureException(string.Format(CultureInfo.InvariantCulture, "Unable to find selector with name '{0}'", selectorName));
            }

            foreach (Type type in typeClosure)
            {
                var isva = PlatformHelper.GetCustomAttribute<ImplementationNameAttribute>(type, false);
                if (isva != null)
                {
                    if (isva.ContractType == foundContractType)
                    {
                        ExceptionUtilities.CheckObjectNotNull(isva.ImplementationName, "Implementation Name of ImplementationType '{0}' must not be null", type.Name);
                        implementations.Add(isva);
                    }
                }
            }

            ExceptionUtilities.CheckCollectionNotEmpty(implementations, "implementations");
            return implementations;
        }
    }
}
