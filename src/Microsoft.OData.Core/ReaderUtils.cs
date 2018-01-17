//---------------------------------------------------------------------
// <copyright file="ReaderUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods for reading OData content.
    /// </summary>
    internal static class ReaderUtils
    {
        /// <summary>
        /// Gets the expected type kind based on the given <see cref="IEdmTypeReference"/>, or EdmTypeKind.None if no specific type should be expected.
        /// </summary>
        /// <param name="expectedTypeReference">The expected type reference.</param>
        /// <param name="enablePrimitiveTypeConversion">Whether primitive type conversion is enabled.</param>
        /// <returns>The expected type kind based on the settings and type reference, or EdmTypeKind.None if no specific type should be expected.</returns>
        internal static EdmTypeKind GetExpectedTypeKind(IEdmTypeReference expectedTypeReference,
                                                       bool enablePrimitiveTypeConversion)
        {
            IEdmType expectedTypeDefinition;
            if (expectedTypeReference == null || (expectedTypeDefinition = expectedTypeReference.Definition) == null)
            {
                return EdmTypeKind.None;
            }

            // If the EnablePrimitiveTypeConversion is off, we must not infer the type kind from the expected type
            // but instead we need to read it from the payload.
            // This is necessary to correctly fail on complex/collection as well as to correctly read spatial values.
            EdmTypeKind expectedTypeKind = expectedTypeDefinition.TypeKind;
            if (!enablePrimitiveTypeConversion
                && (expectedTypeKind == EdmTypeKind.Primitive && !expectedTypeDefinition.IsStream()))
            {
                return EdmTypeKind.None;
            }

            // Otherwise, if we have an expected type, use that.
            return expectedTypeKind;
        }

        /// <summary>
        /// Creates a new <see cref="ODataResource"/> instance to return to the user.
        /// </summary>
        /// <returns>The newly created resource.</returns>
        /// <remarks>The method populates the Properties property with an empty read only enumeration.</remarks>
        internal static ODataResource CreateNewResource()
        {
            return new ODataResource
            {
                Properties = new ReadOnlyEnumerable<ODataProperty>()
            };
        }

        /// <summary>
        /// Creates a new <see cref="ODataDeletedResource"/> instance to return to the user.
        /// </summary>
        /// <param name="id">The id of the deleted resource, or null if not yet known.</param>
        /// <param name="reason">The <see cref="DeltaDeletedEntryReason"/> for the deleted resource.</param>
        /// <returns>The newly created deleted resource.</returns>
        /// <remarks>The method populates the Properties property with an empty read only enumeration.</remarks>
        internal static ODataDeletedResource CreateDeletedResource(Uri id, DeltaDeletedEntryReason reason)
        {
            return new ODataDeletedResource(id, reason)
            {
                Properties = new ReadOnlyEnumerable<ODataProperty>()
            };
        }

        /// <summary>Checks for duplicate navigation links and if there already is an association link with the same name
        /// sets the association link URL on the nested resource info.</summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker for the current scope.</param>
        /// <param name="nestedResourceInfo">The nested resource info to be checked.</param>
        internal static void CheckForDuplicateNestedResourceInfoNameAndSetAssociationLink(
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            ODataNestedResourceInfo nestedResourceInfo)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");

            Uri associationLinkUrl = propertyAndAnnotationCollector.ValidatePropertyUniquenessAndGetAssociationLink(nestedResourceInfo);

            // We must not set the AssociationLinkUrl to null since that would disable templating on it, but we want templating to work if the association link was not in the payload.
            if (associationLinkUrl != null && nestedResourceInfo.AssociationLinkUrl == null)
            {
                nestedResourceInfo.AssociationLinkUrl = associationLinkUrl;
            }
        }

        /// <summary>Checks that for duplicate association links and if there already is a nested resource info with the same name
        /// sets the association link URL on that nested resource info.</summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker for the current scope.</param>
        /// <param name="associationLinkName">The name of association link to be checked.</param>
        /// <param name="associationLinkUrl">The url of association link to be checked.</param>
        internal static void CheckForDuplicateAssociationLinkAndUpdateNestedResourceInfo(
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            string associationLinkName,
            Uri associationLinkUrl)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");
            Debug.Assert(associationLinkName != null, "associationLinkName != null");

            ODataNestedResourceInfo nestedResourceInfo = propertyAndAnnotationCollector.ValidatePropertyOpenForAssociationLinkAndGetNestedResourceInfo(associationLinkName, associationLinkUrl);

            // We must not set the AssociationLinkUrl to null since that would disable templating on it, but we want templating to work if the association link was not in the payload.
            if (nestedResourceInfo != null && nestedResourceInfo.AssociationLinkUrl == null && associationLinkUrl != null)
            {
                nestedResourceInfo.AssociationLinkUrl = associationLinkUrl;
            }
        }

        /// <summary>
        /// Gets the expected property name from the specified property or operation import.
        /// </summary>
        /// <param name="expectedProperty">The <see cref="IEdmProperty"/> to get the expected property name for (or null if none is specified).</param>
        /// <returns>The expected name of the property to be read from the payload.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Ignoring violation because of Debug.Assert.")]
        internal static string GetExpectedPropertyName(IEdmStructuralProperty expectedProperty)
        {
            if (expectedProperty == null)
            {
                return null;
            }

            return expectedProperty.Name;
        }

        /// <summary>
        /// Remove the prefix (#) from type name if there is.
        /// </summary>
        /// <param name="typeName">The type name which may be prefixed (#).</param>
        /// <returns>The type name with prefix removed, if there is.</returns>
        internal static string RemovePrefixOfTypeName(string typeName)
        {
            string prefixRemovedTypeName = typeName;
            if (!string.IsNullOrEmpty(typeName) && typeName.StartsWith(ODataConstants.TypeNamePrefix, StringComparison.Ordinal))
            {
                prefixRemovedTypeName = typeName.Substring(ODataConstants.TypeNamePrefix.Length);

                Debug.Assert(!prefixRemovedTypeName.StartsWith(ODataConstants.TypeNamePrefix, StringComparison.Ordinal), "The type name not start with " + ODataConstants.TypeNamePrefix + "after removing prefix");
            }

            return prefixRemovedTypeName;
        }

        /// <summary>
        /// Add the Edm. prefix to the primitive type if there isn't.
        /// </summary>
        /// <param name="typeName">The type name which may be not prefixed (Edm.).</param>
        /// <returns>The type name with Edm. prefix</returns>
        internal static string AddEdmPrefixOfTypeName(string typeName)
        {
            if (!string.IsNullOrEmpty(typeName))
            {
                string itemTypeName = EdmLibraryExtensions.GetCollectionItemTypeName(typeName);
                if (itemTypeName == null)
                {
                    // This is the primitive type
                    IEdmSchemaType primitiveType = EdmLibraryExtensions.ResolvePrimitiveTypeName(typeName);
                    if (primitiveType != null)
                    {
                        return primitiveType.FullName();
                    }
                }
                else
                {
                    // This is the collection type
                    IEdmSchemaType primitiveType = EdmLibraryExtensions.ResolvePrimitiveTypeName(itemTypeName);
                    if (primitiveType != null)
                    {
                        return EdmLibraryExtensions.GetCollectionTypeName(primitiveType.FullName());
                    }
                }
            }

            // Return the origin type name
            return typeName;
        }
    }
}
