//---------------------------------------------------------------------
// <copyright file="WriterUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    #region Namespaces

    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Class with utility methods for writing OData content.
    /// </summary>
    internal static class WriterUtils
    {
        /// <summary>
        /// Determines if a property should be written or skipped.
        /// </summary>
        /// <param name="projectedProperties">The projected properties annotation to use (can be null).</param>
        /// <param name="propertyName">The name of the property to check.</param>
        /// <returns>true if the property should be skipped, false to write the property.</returns>
        internal static bool ShouldSkipProperty(this ProjectedPropertiesAnnotation projectedProperties, string propertyName)
        {
            if (projectedProperties == null)
            {
                return false;
            }
            else if (object.ReferenceEquals(ProjectedPropertiesAnnotation.EmptyProjectedPropertiesInstance, projectedProperties))
            {
                return true;
            }
            else if (object.ReferenceEquals(ProjectedPropertiesAnnotation.AllProjectedPropertiesInstance, projectedProperties))
            {
                return false;
            }

            return !projectedProperties.IsPropertyProjected(propertyName);
        }

        /// <summary>
        /// Remove the Edm. prefix from the type name if it is primitive type.
        /// </summary>
        /// <param name="typeName">The type name to remove the Edm. prefix</param>
        /// <returns>The type name without the Edm. Prefix</returns>
        internal static string RemoveEdmPrefixFromTypeName(string typeName)
        {
            if (!string.IsNullOrEmpty(typeName))
            {
                string itemTypeName = EdmLibraryExtensions.GetCollectionItemTypeName(typeName);
                if (itemTypeName == null)
                {
                    IEdmSchemaType primitiveType = EdmLibraryExtensions.ResolvePrimitiveTypeName(typeName);
                    if (primitiveType != null)
                    {
                        return primitiveType.ShortQualifiedName();
                    }
                }
                else
                {
                    IEdmSchemaType primitiveType = EdmLibraryExtensions.ResolvePrimitiveTypeName(itemTypeName);
                    if (primitiveType != null)
                    {
                        return EdmLibraryExtensions.GetCollectionTypeName(primitiveType.ShortQualifiedName());
                    }
                }
            }

            return typeName;
        }
    }
}
