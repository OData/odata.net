//---------------------------------------------------------------------
// <copyright file="EdmNavigationTargetMapping.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a mapping from an EDM navigation property to an entity set.
    /// </summary>
    public class EdmNavigationTargetMapping : IEdmNavigationTargetMapping
    {
        private IEdmNavigationProperty navigationProperty;
        private IEdmEntitySet targetEntitySet;

        /// <summary>
        /// Creates a new navigation target mapping.
        /// </summary>
        /// <param name="navigationProperty">The navigation property.</param>
        /// <param name="targetEntitySet">The entity set that the navigation propertion targets.</param>
        public EdmNavigationTargetMapping(IEdmNavigationProperty navigationProperty, IEdmEntitySet targetEntitySet)
        {
            this.navigationProperty = navigationProperty;
            this.targetEntitySet = targetEntitySet;
        }

        /// <summary>
        /// Gets the navigation property.
        /// </summary>
        public IEdmNavigationProperty NavigationProperty
        {
            get { return this.navigationProperty; }
        }

        /// <summary>
        /// Gets the target entity set.
        /// </summary>
        public IEdmEntitySet TargetEntitySet
        {
            get { return this.targetEntitySet; }
        }
    }
}
