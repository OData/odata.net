//---------------------------------------------------------------------
// <copyright file="EdmNavigationPropertyBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a mapping from an EDM navigation property to a navigation source.
    /// </summary>
    public class EdmNavigationPropertyBinding : IEdmNavigationPropertyBinding
    {
        private IEdmNavigationProperty navigationProperty;
        private IEdmNavigationSource target;

        /// <summary>
        /// Creates a new navigation property binding.
        /// </summary>
        /// <param name="navigationProperty">The navigation property.</param>
        /// <param name="target">The navigation source that the navigation property targets.</param>
        public EdmNavigationPropertyBinding(IEdmNavigationProperty navigationProperty, IEdmNavigationSource target)
        {
            this.navigationProperty = navigationProperty;
            this.target = target;
        }

        /// <summary>
        /// Gets the navigation property.
        /// </summary>
        public IEdmNavigationProperty NavigationProperty
        {
            get { return this.navigationProperty; }
        }

        /// <summary>
        /// Gets the target navigation source.
        /// </summary>
        public IEdmNavigationSource Target
        {
            get { return this.target; }
        }
    }
}
