//---------------------------------------------------------------------
// <copyright file="IEdmNavigationTargetMapping.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a mapping from an EDM navigation property to an entity set.
    /// </summary>
    public interface IEdmNavigationTargetMapping
    {
        /// <summary>
        /// Gets the navigation property.
        /// </summary>
        IEdmNavigationProperty NavigationProperty { get; }

        /// <summary>
        /// Gets the target entity set.
        /// </summary>
        IEdmEntitySet TargetEntitySet { get; }
    }
}
