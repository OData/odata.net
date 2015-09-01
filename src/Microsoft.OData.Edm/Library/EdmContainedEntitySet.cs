﻿//---------------------------------------------------------------------
// <copyright file="EdmContainedEntitySet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Library
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Edm.Expressions;
    using Microsoft.OData.Edm.Library.Expressions;

    /// <summary>
    /// Represents an EDM contained entity set.
    /// </summary>
    internal class EdmContainedEntitySet : EdmEntitySetBase, IEdmContainedEntitySet
    {
        private readonly IEdmNavigationSource parentNavigationSource;
        private readonly IEdmNavigationProperty navigationProperty;
        private IEdmPathExpression path;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmContainedEntitySet"/> class.
        /// </summary>
        /// <param name="parentNavigationSource">The <see cref="IEdmNavigationSource"/> that container element belongs to</param>
        /// <param name="navigationProperty">An <see cref="IEdmNavigationProperty"/> containing the navigation property definition of the contained element</param>
        public EdmContainedEntitySet(IEdmNavigationSource parentNavigationSource, IEdmNavigationProperty navigationProperty)
            : base(navigationProperty.Name, navigationProperty.ToEntityType())
        {
            EdmUtil.CheckArgumentNull(parentNavigationSource, "parentNavigationSource");
            EdmUtil.CheckArgumentNull(navigationProperty, "navigationProperty");

            this.parentNavigationSource = parentNavigationSource;
            this.navigationProperty = navigationProperty;
        }

        /// <summary>
        /// Gets the path that a navigation property targets. This property is not thread safe.
        /// </summary>
        public override IEdmPathExpression Path
        {
            get { return this.path ?? (this.path = ComputePath()); }
        }

        /// <summary>The parent navigation source of this contained entity set.</summary>
        /// <returns>The parent navigation source of this contained entity set.</returns>
        public IEdmNavigationSource ParentNavigationSource
        {
            get { return this.parentNavigationSource; }
        }

        /// <summary>The navigation property of this contained entity set.</summary>
        /// <returns>The navigation property of this contained entity set.</returns>
        public IEdmNavigationProperty NavigationProperty
        {
            get { return this.navigationProperty; }
        }

        /// <summary>
        /// Finds the entity set that a navigation property targets.
        /// </summary>
        /// <param name="property">The navigation property.</param>
        /// <returns>The entity set that the navigation property targets</returns>
        public override IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty property)
        {
            IEdmNavigationSource navigationTarget = base.FindNavigationTarget(property);
            if (navigationTarget is IEdmUnknownEntitySet)
            {
                return this.parentNavigationSource.FindNavigationTarget(property);
            }

            return navigationTarget;
        }

        private IEdmPathExpression ComputePath()
        {
            List<string> newPath = new List<string>(this.parentNavigationSource.Path.Path);
            newPath.Add(this.navigationProperty.Name);
            return new EdmPathExpression(newPath.ToArray());
        }
    }
}
