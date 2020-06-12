//---------------------------------------------------------------------
// <copyright file="ODataMetadataSelector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Evaluation
{
    /// <summary>
    /// The role of this class is to control what metadata information gets written for full metadata.
    /// Navigation links, association links, operations and stream properties can be modified for a full metadata response.
    /// </summary>
    public abstract class ODataMetadataSelector
    {
        /// <summary>
        /// Alters the selected navigation properties for which the metadata needs to be written.
        /// </summary>
        /// <param name="type">Edm type of the resource on which the navigation property exists.</param>
        /// <param name="navigationProperties">Enumerable of selected navigation properties.</param>
        /// <returns>The list of navigation properties to be written.</returns>
        public virtual IEnumerable<IEdmNavigationProperty> SelectNavigationProperties(IEdmStructuredType type, IEnumerable<IEdmNavigationProperty> navigationProperties)
        {
            return navigationProperties;
        }

        /// <summary>
        /// Alters the bindable operations for the type which may get written with full metadata response.
        /// </summary>
        /// <param name="type">Edm type of the resource on which the navigation property exists.</param>
        /// <param name="bindableOperations">Enumerable of the operations whose metadata gets written out by convention.</param>
        /// <returns>The list of bound operations to be written.</returns>
        public virtual IEnumerable<IEdmOperation> SelectBindableOperations(IEdmStructuredType type, IEnumerable<IEdmOperation> bindableOperations)
        {
            return bindableOperations;
        }

        /// <summary>
        /// Alters the selected stream properties for the type which may get written with full metadata response.
        /// </summary>
        /// <param name="type">Edm type of the resource on which the navigation property exists.</param>
        /// <param name="selectedStreamProperties">enumerable of the selected stream properties.</param>
        /// <returns>The list of stream properties to be written.</returns>
        public virtual IEnumerable<IEdmStructuralProperty> SelectStreamProperties(IEdmStructuredType type, IEnumerable<IEdmStructuralProperty> selectedStreamProperties)
        {
            return selectedStreamProperties;
        }
    }
}
