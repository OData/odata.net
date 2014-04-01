//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods for reading OData content.
    /// </summary>
    internal static class ReaderUtils
    {
        /// <summary>
        /// Creates a new <see cref="ODataEntry"/> instance to return to the user.
        /// </summary>
        /// <returns>The newly created entry.</returns>
        /// <remarks>The method populates the Properties property with an empty read only enumeration.</remarks>
        internal static ODataEntry CreateNewEntry()
        {
            return new ODataEntry
            {
                Properties = new ReadOnlyEnumerable<ODataProperty>()
            };
        }

        /// <summary>Checks for duplicate navigation links and if there already is an association link with the same name
        /// sets the association link URL on the navigation link.</summary>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker for the current scope.</param>
        /// <param name="navigationLink">The navigation link to be checked.</param>
        /// <param name="isExpanded">true if the link is expanded, false otherwise.</param>
        /// <param name="isCollection">true if the navigation link is a collection, false if it's a singleton or null if we don't know.</param>
        internal static void CheckForDuplicateNavigationLinkNameAndSetAssociationLink(
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            ODataNavigationLink navigationLink,
            bool isExpanded,
            bool? isCollection)
        {
            Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");
            Debug.Assert(navigationLink != null, "navigationLink != null");
            
            Uri associationLinkUrl = duplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(navigationLink, isExpanded, isCollection);

            // We must not set the AssociationLinkUrl to null since that would disable templating on it, but we want templating to work if the association link was not in the payload.
            if (associationLinkUrl != null && navigationLink.AssociationLinkUrl == null)
            {
                navigationLink.AssociationLinkUrl = associationLinkUrl;
            }
        }

        /// <summary>Checks that for duplicate association links and if there already is a navigation link with the same name
        /// sets the association link URL on that navigation link.</summary>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker for the current scope.</param>
        /// <param name="associationLinkName">The name of association link to be checked.</param>
        /// <param name="associationLinkUrl">The url of association link to be checked.</param>
        internal static void CheckForDuplicateAssociationLinkAndUpdateNavigationLink(
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            string associationLinkName,
            Uri associationLinkUrl)
        {
            Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");
            Debug.Assert(associationLinkName != null, "associationLinkName != null");

            ODataNavigationLink navigationLink = duplicatePropertyNamesChecker.CheckForDuplicateAssociationLinkNames(associationLinkName, associationLinkUrl);

            // We must not set the AssociationLinkUrl to null since that would disable templating on it, but we want templating to work if the association link was not in the payload.
            if (navigationLink != null && navigationLink.AssociationLinkUrl == null && associationLinkUrl != null)
            {
                navigationLink.AssociationLinkUrl = associationLinkUrl;
            }
        }

        /// <summary>
        /// Returns true if the specified <paramref name="flag"/> is set in the <paramref name="undeclaredPropertyBehaviorKinds"/>.
        /// </summary>
        /// <param name="undeclaredPropertyBehaviorKinds">The value of the setting to test.</param>
        /// <param name="flag">The flag to test.</param>
        /// <returns>true if the flas is present, flase otherwise.</returns>
        internal static bool HasFlag(this ODataUndeclaredPropertyBehaviorKinds undeclaredPropertyBehaviorKinds, ODataUndeclaredPropertyBehaviorKinds flag)
        {
            Debug.Assert(((int)flag | ((int)flag - 1)) + 1 == (int)flag * 2, "Only one flag must be set.");

            return (undeclaredPropertyBehaviorKinds & flag) == flag;
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
