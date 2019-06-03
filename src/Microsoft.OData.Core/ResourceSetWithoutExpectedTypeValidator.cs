//---------------------------------------------------------------------
// <copyright file="ResourceSetWithoutExpectedTypeValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Helper class to verify that all items of a collection are of the same kind and type.
    /// </summary>
    /// <remarks>This class is only used if no expected item type is specified for the collection;
    /// otherwise all items are already validated against the expected item type.</remarks>
    internal sealed class ResourceSetWithoutExpectedTypeValidator
    {
        /// <summary>
        /// The base type for all items in the resource set.
        /// </summary>
        private IEdmType itemType;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="memberType">The type of the resource set, or null.</param>
        public ResourceSetWithoutExpectedTypeValidator(IEdmType memberType)
        {
            this.itemType = memberType;
        }

        /// <summary>
        /// Validates the type of a resource in a top-level resource set.
        /// </summary>
        /// <param name="itemType">The type of the resource.</param>
        internal void ValidateResource(IEdmType itemType)
        {
            if (this.itemType == null || this.itemType.TypeKind == EdmTypeKind.Untyped)
            {
                return;
            }

            // Validate the expected and actual types.
            if (this.itemType.IsEquivalentTo(itemType))
            {
                return;
            }

            IEdmStructuredType structuredType = itemType as IEdmStructuredType;
            IEdmStructuredType thisStructuredType = this.itemType as IEdmStructuredType;

            if (structuredType == null || thisStructuredType == null)
            {
                throw new ODataException(Strings.ResourceSetWithoutExpectedTypeValidator_IncompatibleTypes(itemType.FullTypeName(), this.itemType.FullTypeName()));
            }

            // Make sure the resource types is same or derived type of expected type
            if (!this.itemType.IsAssignableFrom(itemType))
            {
                throw new ODataException(Strings.ResourceSetWithoutExpectedTypeValidator_IncompatibleTypes(itemType.FullTypeName(), this.itemType.FullTypeName()));
            }
        }
    }
}
