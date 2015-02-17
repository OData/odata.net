//---------------------------------------------------------------------
// <copyright file="DependencyImplementationAssemblies.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// Maintains a list of assemblies that contain implementations used for dependency injection
    /// </summary>
    public static class DependencyImplementationAssemblies
    {
        /// <summary>
        /// List of assemblies that contain implementations used for dependency injection
        /// </summary>
        private static List<Assembly> assemblies;

        /// <summary>
        /// Initializes static members of the DependencyImplementationAssemblies class 
        /// </summary>
        static DependencyImplementationAssemblies()
        {
            assemblies = new List<Assembly>();
            assemblies.Add(typeof(DependencyImplementationAssemblies).Assembly); // Microsoft.Test.Taupo.OData.Tests.dll
            assemblies.Add(typeof(Logger).Assembly); // Microsoft.Test.Taupo.dll
            assemblies.Add(typeof(ODataConstants).Assembly); // Microsoft.Test.Taupo.Astoria.dll
            assemblies.Add(typeof(QueryExpression).Assembly); // Microsoft.Test.Taupo.Query.dll
            assemblies.Add(typeof(IWellKnownTextSpatialFormatter).Assembly); // Microsoft.Test.Taupo.Spatial.dll
        }

        /// <summary>
        /// returns assemblies containing implementations used for dependency injection 
        /// </summary>
        /// <returns>assemblies that contain implementations used for dependency injection</returns>
        public static IEnumerable<Assembly> GetAssemblies()
        {
            return assemblies;
        }
    }
}
