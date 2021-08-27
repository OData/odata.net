//---------------------------------------------------------------------
// <copyright file="AstoriaWrapperUtilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Intermediate Wrapper for resolving references to Product assemblies on the Windows Phone platform,
    /// delegates to the WrapperUtilities for all other platforms
    /// </summary>
    public static class AstoriaWrapperUtilities
    {
        private static Dictionary<string, string> internalMethodMap;

        /// <summary>
        /// Gets an internal method map which maps the desktop method signature to its platform specific signature
        /// </summary>
        private static Dictionary<string, string> PlatformMethodMap
        {
            get
            {
                if (internalMethodMap == null)
                {
                    InitializePlatformSpecificMethodMap();
                }

                return internalMethodMap;
            }
        }

        /// <summary>
        /// Gets the type for a given name of the type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>Requested type.</returns>
        public static Type GetTypeFromAssembly(string typeName, string assemblyName)
        {
            return WrapperUtilities.GetTypeFromAssembly(typeName, assemblyName);
        }

        /// <summary>
        /// Gets the MethodHandle based on the given signature
        /// </summary>
        /// <param name="type">The type where the method is</param>
        /// <param name="methodInfoCache">The cache of method infos</param>
        /// <param name="signature">The signature of the method to look up</param>
        /// <returns>Runtime method handle.</returns>
        public static MethodInfo GetMethodInfo(Type type, IDictionary<string, MethodInfo> methodInfoCache, string signature)
        {
            if (PlatformMethodMap.ContainsKey(signature))
            {
                signature = PlatformMethodMap[signature];
            }

            return WrapperUtilities.GetMethodInfo(type, methodInfoCache, signature);
        }

        private static void InitializePlatformSpecificMethodMap()
        {
            internalMethodMap = new Dictionary<string, string>();
        }
    }
}