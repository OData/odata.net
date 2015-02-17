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
#if WINDOWS_PHONE
            Assembly assemblyWithTypes = null;
            if (String.Equals(assemblyName, "Microsoft.OData.Client", StringComparison.OrdinalIgnoreCase))
            {
                assemblyWithTypes = typeof(DataServiceContext).GetAssembly();
            }

            Type type = assemblyWithTypes.GetType(typeName, false);
            if (type == null)
            {
                type = WrapperUtilities.GetTypeFromAssembly(typeName, assemblyName);
            }

            return type;
#else
#if WIN8
            Assembly assemblyWithTypes = null;
            if (String.Equals(assemblyName, "Microsoft.OData.Client", StringComparison.OrdinalIgnoreCase))
            {
                assemblyWithTypes = typeof(DataServiceContext).GetAssembly();
            }

            Type type = assemblyWithTypes.GetType(typeName, false);
            if (type == null)
            {
                type = WrapperUtilities.GetTypeFromAssembly(typeName, assemblyName);
            }

            return type;
#else
            return WrapperUtilities.GetTypeFromAssembly(typeName, assemblyName);
#endif
#endif
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
#if !WINDOWS_PHONE
            if (PlatformMethodMap.ContainsKey(signature))
            {
                signature = PlatformMethodMap[signature];
            }
#endif

            return WrapperUtilities.GetMethodInfo(type, methodInfoCache, signature);
        }

        private static void InitializePlatformSpecificMethodMap()
        {
            internalMethodMap = new Dictionary<string, string>();
#if WINDOWS_PHONE
            InitializeWindowsPhonePlatformMethodMap(internalMethodMap);
#endif
        }

#if WINDOWS_PHONE
        private static void InitializeWindowsPhonePlatformMethodMap(Dictionary<string, string> methodMap)
        {
            methodMap.Add("System.Collections.ObjectModel.ReadOnlyCollection`1[Microsoft.OData.Client.EntityDescriptor] get_Entities()", "System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.OData.Client.EntityDescriptor, Microsoft.OData.Client, Version=7.1.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35]] get_Entities()");
            methodMap.Add("System.Collections.ObjectModel.ReadOnlyCollection`1[Microsoft.OData.Client.LinkDescriptor] get_Links()", "System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.OData.Client.LinkDescriptor, Microsoft.OData.Client, Version=7.1.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35]] get_Links()");
            methodMap.Add("System.IAsyncResult BeginExecute[T](Microsoft.OData.Client.DataServiceQueryContinuation`1[T], System.AsyncCallback, System.Object)", "System.IAsyncResult BeginExecute[](Microsoft.OData.Client.DataServiceQueryContinuation`1, System.AsyncCallback, System.Object)");
            methodMap.Add("System.IAsyncResult BeginExecute[TElement](System.Uri, System.AsyncCallback, System.Object)", "System.IAsyncResult BeginExecute[](System.Uri, System.AsyncCallback, System.Object)");
            methodMap.Add("Microsoft.OData.Client.DataServiceQuery`1[T] CreateQuery[T](System.String)", "Microsoft.OData.Client.DataServiceQuery`1 CreateQuery[](System.String)");
            methodMap.Add("System.Collections.Generic.IEnumerable`1[TElement] EndExecute[TElement](System.IAsyncResult)", "System.Collections.Generic.IEnumerable`1 EndExecute[](System.IAsyncResult)");
            methodMap.Add("Boolean TryGetEntity[TEntity](System.Uri, TEntity ByRef)", "Boolean TryGetEntity[](System.Uri,  ByRef)");
            methodMap.Add("System.Collections.Generic.IEnumerable`1[TElement] EndExecute(System.IAsyncResult)", "System.Collections.Generic.IEnumerable`1 EndExecute(System.IAsyncResult)");
            methodMap.Add("Microsoft.OData.Client.DataServiceQuery`1[TElement] Expand(System.String)", "Microsoft.OData.Client.DataServiceQuery`1 Expand(System.String)");
        }
#endif
    }
}