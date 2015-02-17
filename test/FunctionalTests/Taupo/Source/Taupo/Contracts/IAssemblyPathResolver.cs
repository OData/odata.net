//---------------------------------------------------------------------
// <copyright file="IAssemblyPathResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Helps resolve where assemblies are located
    /// </summary>
    [ImplementationSelector("AssemblyPathResolver", DefaultImplementation = "Default", HelpText = "Resolver that understands how to resolve assemblies to their locations")]
    public interface IAssemblyPathResolver
    {
        /// <summary>
        /// Resolves an assembly Path from the assemblyName
        /// </summary>
        /// <param name="assemblyName">Assembly Name</param>
        /// <returns>Original AssemblyName value or a fully qualified path to the Assembly on the local machine</returns>
        string ResolveAssemblyLocation(string assemblyName);
    }
}
