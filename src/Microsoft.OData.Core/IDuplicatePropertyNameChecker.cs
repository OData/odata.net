//---------------------------------------------------------------------
// <copyright file="IDuplicatePropertyNameChecker.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Validates that
    ///   1) No duplicate property.
    ///   2) No duplicate "@odata.associationLink" on a property.
    ///   3) "@odata.associationLink"s are put on navigation properties.
    /// </summary>
    internal interface IDuplicatePropertyNameChecker
    {
        /// <summary>
        /// Validates property uniqueness.
        /// </summary>
        /// <param name="property">The property.</param>
        void ValidatePropertyUniqueness(ODataProperty property);

        /// <summary>
        /// Validates property uniqueness.
        /// </summary>
        /// <param name="property">The property.</param>
        void ValidatePropertyUniqueness(ODataNestedResourceInfo property);

        /// <summary>
        /// Validates that "@odata.associationLink" is put on a navigation property,
        /// and that no duplicate exists.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        void ValidatePropertyOpenForAssociationLink(string propertyName);

        /// <summary>
        /// Resets to initial state for reuse.
        /// </summary>
        void Reset();
    }
}
