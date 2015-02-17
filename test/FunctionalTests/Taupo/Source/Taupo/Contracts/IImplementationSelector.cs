//---------------------------------------------------------------------
// <copyright file="IImplementationSelector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Provides methods to configure implementations based on set of loaded assemblies.
    /// </summary>
    public interface IImplementationSelector
    {
        /// <summary>
        /// Adds the specified types to the list of types which should be taken into account when computing implementation.
        /// </summary>
        /// <param name="types">Types to add.</param>
        void AddTypes(IEnumerable<Type> types);

        /// <summary>
        /// Adds all types from the assembly to the list of types which should be taken into account when computing implementation.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        void AddAssembly(Assembly assembly);
    }
}
