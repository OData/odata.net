//---------------------------------------------------------------------
// <copyright file="ClientTypeCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Metadata
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using c = Microsoft.OData.Client;

    #endregion Namespaces.

    /// <summary>
    /// Caches wire type names and their mapped client CLR types.
    /// </summary>
    [DebuggerDisplay("{PropertyName}")]
    internal static class ClientTypeCache
    {
        /// <summary>cache &lt;T&gt; and wireName to mapped type</summary>
        private static readonly Dictionary<TypeName, Type> namedTypes = new Dictionary<TypeName, Type>(new TypeNameEqualityComparer());

        /// <summary>
        /// resolve the wireName/userType pair to a CLR type
        /// </summary>
        /// <param name="wireName">type name sent by server</param>
        /// <param name="userType">type passed by user or on propertyType from a class</param>
        /// <returns>mapped clr type</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        internal static Type ResolveFromName(string wireName, Type userType)
        {
            Type foundType;

            TypeName typename;
            typename.Type = userType;
            typename.Name = wireName;

            // search the "wirename"-userType key in type cache
            bool foundInCache;
            lock (ClientTypeCache.namedTypes)
            {
                foundInCache = ClientTypeCache.namedTypes.TryGetValue(typename, out foundType);
            }

            // at this point, if we have seen this type before, we either have the resolved type "foundType",
            // or we have tried to resolve it before but did not success, in which case foundType will be null.
            // Either way we should return what's in the cache since the result is unlikely to change.
            // We only need to keep on searching if there isn't an entry in the cache.
            if (!foundInCache)
            {
                string name = wireName;
                int index = wireName.LastIndexOf('.');
                if ((index >= 0) && (index < wireName.Length - 1))
                {
                    name = wireName.Substring(index + 1);
                }

                if (userType.Name == name)
                {
                    foundType = userType;
                }
                else
                {
                    // searching only loaded assemblies, not referenced assemblies
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        Type found = assembly.GetType(wireName, false);
                        ResolveSubclass(name, userType, found, ref foundType);

                        if (found == null)
                        {
                            IEnumerable<Type> types = null;
                            try
                            {
                                types = assembly.GetTypes();
                            }
                            catch (ReflectionTypeLoadException)
                            {
                            }

                            if (types != null)
                            {
                                foreach (Type t in types)
                                {
                                    ResolveSubclass(name, userType, t, ref foundType);
                                }
                            }
                        }
                    }
                }

                // The above search can all fail and leave "foundType" to be null
                // we should cache this result too so we won't waste time searching again.
                lock (ClientTypeCache.namedTypes)
                {
                    ClientTypeCache.namedTypes[typename] = foundType;
                }
            }

            return foundType;
        }

        /// <summary>
        /// is the type a visible subclass with correct name
        /// </summary>
        /// <param name="wireClassName">type name from server</param>
        /// <param name="userType">the type from user for materialization or property type</param>
        /// <param name="type">type being tested</param>
        /// <param name="existing">the previously discovered matching type</param>
        /// <exception cref="InvalidOperationException">if the mapping is ambiguous</exception>
        private static void ResolveSubclass(string wireClassName, Type userType, Type type, ref Type existing)
        {
            if ((type != null) && c.PlatformHelper.IsVisible(type) && (wireClassName == type.Name) && userType.IsAssignableFrom(type))
            {
                if (existing != null)
                {
                    throw c.Error.InvalidOperation(c.Strings.ClientType_Ambiguous(wireClassName, userType));
                }

                existing = type;
            }
        }

        /// <summary>type + wireName combination</summary>
        private struct TypeName
        {
            /// <summary>type</summary>
            internal Type Type;

            /// <summary>type name from server</summary>
            internal string Name;
        }

        /// <summary>equality comparer for TypeName</summary>
        private sealed class TypeNameEqualityComparer : IEqualityComparer<TypeName>
        {
            /// <summary>equality comparer for TypeName</summary>
            /// <param name="x">left type</param>
            /// <param name="y">right type</param>
            /// <returns>true if x and y are equal</returns>
            public bool Equals(TypeName x, TypeName y)
            {
                return (x.Type == y.Type && x.Name == y.Name);
            }

            /// <summary>compute hashcode for TypeName</summary>
            /// <param name="obj">object to compute hashcode for</param>
            /// <returns>computed hashcode</returns>
            public int GetHashCode(TypeName obj)
            {
                return obj.Type.GetHashCode() ^ obj.Name.GetHashCode();
            }
        }
    }
}
