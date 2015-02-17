//---------------------------------------------------------------------
// <copyright file="FeedWithoutExpectedTypeValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Helper class to verify that all items of a collection are of the same kind and type.
    /// </summary>
    /// <remarks>This class is only used if no expected item type is specified for the collection; 
    /// otherwise all items are already validated against the expected item type.</remarks>
    internal sealed class FeedWithoutExpectedTypeValidator
    {
        /// <summary>
        /// The base type for all entries in the feed.
        /// </summary>
        private IEdmEntityType itemType;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal FeedWithoutExpectedTypeValidator()
        {
        }

        /// <summary>
        /// Validates the type of an entry in a top-level feed.
        /// </summary>
        /// <param name="entityType">The type of the entry.</param>
        internal void ValidateEntry(IEdmEntityType entityType)
        {
            Debug.Assert(entityType != null, "entityType != null");

            // If we don't have a type, store the type of the first item.
            if (this.itemType == null)
            {
                this.itemType = entityType;
            }

            // Validate the expected and actual types.
            if (this.itemType.IsEquivalentTo(entityType))
            {
                return;
            }

            // If the types are not equivalent, make sure they have a common base type.
            IEdmType commonBaseType = EdmLibraryExtensions.GetCommonBaseType(this.itemType, entityType);
            if (commonBaseType == null)
            {
                throw new ODataException(Strings.FeedWithoutExpectedTypeValidator_IncompatibleTypes(entityType.ODataFullName(), this.itemType.ODataFullName()));
            }

            this.itemType = (IEdmEntityType)commonBaseType;
        }
    }
}
