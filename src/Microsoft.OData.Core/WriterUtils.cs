//---------------------------------------------------------------------
// <copyright file="WriterUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Class with utility methods for writing OData content.
    /// </summary>
    internal static class WriterUtils
    {
        /// <summary>
        /// Prepare the type name for writing.
        /// 1) If it is primitive type, remove the Edm. prefix.
        /// 2) If it is a non-primitive type or 4.0, prefix with #.
        /// </summary>
        /// <param name="typeName">The type name to write</param>
        /// <param name="version">OData Version of payload being written</param>
        /// <returns>The type name for writing</returns>
        internal static string PrefixTypeNameForWriting(string typeName, ODataVersion version)
        {
            if (!string.IsNullOrEmpty(typeName))
            {
                string itemTypeName = EdmLibraryExtensions.GetCollectionItemTypeName(typeName);
                if (itemTypeName == null)
                {
                    IEdmSchemaType primitiveType = EdmLibraryExtensions.ResolvePrimitiveTypeName(typeName);
                    if (primitiveType != null)
                    {
                        typeName = primitiveType.ShortQualifiedName();
                        return version < ODataVersion.V401 ? PrefixTypeName(typeName) : typeName;
                    }
                }
                else
                {
                    IEdmSchemaType primitiveType = EdmLibraryExtensions.ResolvePrimitiveTypeName(itemTypeName);
                    if (primitiveType != null)
                    {
                        typeName = EdmLibraryExtensions.GetCollectionTypeName(primitiveType.ShortQualifiedName());
                        return version < ODataVersion.V401 ? PrefixTypeName(typeName) : typeName;
                    }
                }
            }

            return PrefixTypeName(typeName);
        }

        /// <summary>
        /// For JsonLight writer, always prefix the type name with # for payload writting.
        /// </summary>
        /// <param name="typeName">The type name to prefix</param>
        /// <returns>The (#) prefixed type name.</returns>
        private static string PrefixTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return typeName;
            }

            Debug.Assert(!typeName.StartsWith(ODataConstants.TypeNamePrefix, StringComparison.Ordinal), "The type name not start with " + ODataConstants.TypeNamePrefix + "before prefix");

            return ODataConstants.TypeNamePrefix + typeName;
        }
    }
}