//---------------------------------------------------------------------
// <copyright file="IEdmNavigationPropertyBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a binding from an EDM navigation property to a navigation source.
    /// </summary>
    public interface IEdmNavigationPropertyBinding
    {
        /// <summary>
        /// Gets the navigation property.
        /// </summary>
        IEdmNavigationProperty NavigationProperty { get; }

        /// <summary>
        /// Gets the target navigation source.
        /// </summary>
        IEdmNavigationSource Target { get; }

        /// <summary>
        /// Get the path that a navigation property targets.
        /// </summary>
        IEdmPathExpression Path { get; }
    }
}
