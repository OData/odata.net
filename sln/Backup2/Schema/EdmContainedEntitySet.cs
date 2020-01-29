//---------------------------------------------------------------------
// <copyright file="EdmContainedEntitySet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents an EDM contained entity set.
    /// </summary>
    internal class EdmContainedEntitySet : EdmEntitySetBase, IEdmContainedEntitySet
    {
        private readonly IEdmPathExpression navigationPath;
        private readonly IEdmNavigationSource parentNavigationSource;
        private readonly IEdmNavigationProperty navigationProperty;
        private IEdmPathExpression path;
        private string fullPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmContainedEntitySet"/> class.
        /// </summary>
        /// <param name="parentNavigationSource">The <see cref="IEdmNavigationSource"/> that container element belongs to</param>
        /// <param name="navigationProperty">An <see cref="IEdmNavigationProperty"/> containing the navigation property definition of the contained element</param>
        public EdmContainedEntitySet(IEdmNavigationSource parentNavigationSource, IEdmNavigationProperty navigationProperty)
            : this(parentNavigationSource, navigationProperty, new EdmPathExpression(navigationProperty.Name))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmContainedEntitySet"/> class.
        /// </summary>
        /// <param name="parentNavigationSource">The <see cref="IEdmNavigationSource"/> that container element belongs to</param>
        /// <param name="navigationProperty">An <see cref="IEdmNavigationProperty"/> containing the navigation property definition of the contained element</param>
        /// <param name="navigationPath">The path from the parentNavigationSource to the related resource, concluding with the navigation property name. May traverse complex types and cast segments</param>
        public EdmContainedEntitySet(IEdmNavigationSource parentNavigationSource, IEdmNavigationProperty navigationProperty, IEdmPathExpression navigationPath)
            : base(navigationProperty.Name, navigationProperty.ToEntityType())
        {
            EdmUtil.CheckArgumentNull(parentNavigationSource, "parentNavigationSource");
            EdmUtil.CheckArgumentNull(navigationProperty, "navigationProperty");

            this.parentNavigationSource = parentNavigationSource;
            this.navigationProperty = navigationProperty;
            this.navigationPath = navigationPath;
        }

        /// <summary>
        /// Gets the path that a navigation property targets. This property is not thread safe.
        /// </summary>
        public override IEdmPathExpression Path
        {
            get { return this.path ?? (this.path = this.ComputePath()); }
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

        internal IEdmPathExpression NavigationPath
        {
            get
            {
                return this.navigationPath;
            }
        }

        private string FullNavigationPath
        {
            get
            {
                if (this.fullPath == null)
                {
                    List<string> fullPath = new List<string>();
                    EdmContainedEntitySet currentSource = this;
                    while (currentSource != null)
                    {
                        fullPath.AddRange(currentSource.NavigationPath.PathSegments);
                        currentSource = currentSource.ParentNavigationSource as EdmContainedEntitySet;
                    }

                    fullPath.Reverse();
                    this.fullPath = new EdmPathExpression(fullPath).Path;
                }

                return this.fullPath;
            }
        }

        /// <summary>
        /// Finds the bindings of the navigation property.
        /// </summary>
        /// <param name="navigationProperty">The navigation property.</param>
        /// <returns>The list of bindings for current navigation property.</returns>
        public override IEnumerable<IEdmNavigationPropertyBinding> FindNavigationPropertyBindings(IEdmNavigationProperty navigationProperty)
        {
            IEnumerable<IEdmNavigationPropertyBinding> bindings = base.FindNavigationPropertyBindings(navigationProperty);
            IEdmNavigationSource parent;
            IEdmContainedEntitySet contained = this;
            IEnumerable<IEdmNavigationPropertyBinding> parentBindings;

            while (contained != null)
            {
                parent = contained.ParentNavigationSource;
                parentBindings = parent.FindNavigationPropertyBindings(navigationProperty);
                if (parentBindings != null)
                {
                    bindings = bindings == null ? parentBindings : bindings.Concat(parentBindings);
                }

                contained = parent as IEdmContainedEntitySet;
            }

            return bindings;
        }

        /// <summary>
        /// Finds the entity set that a navigation property targets.
        /// </summary>
        /// <param name="navigationProperty">The navigation property.</param>
        /// <returns>The entity set that the navigation property targets</returns>
        public override IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty)
        {
            return this.FindNavigationTarget(navigationProperty, new EdmPathExpression(navigationProperty.Name));
        }

        /// <summary>
        /// Finds the entity set that a navigation property targets.
        /// </summary>
        /// <param name="navigationProperty">The navigation property.</param>
        /// <param name="bindingPath">The binding path of the navigation property</param>
        /// <returns>The entity set that the navigation property targets</returns>
        public override IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty, IEdmPathExpression bindingPath)
        {
            // 7.4.1 expected the path to be prefixed with the path to the contained navigation source.
            // For backward compatibility, if the binding path received starts with the path to this contained resource,
            // we trim it off and then treat the remainder as the path to the target. This logic should be removed in
            // the next breaking change as it could be ambiguous in the case that the prefix of the path to the contained
            // source matches a valid path to the target of the contained source.
            if (bindingPath != null)
            {
                if (bindingPath.Path.Length > this.FullNavigationPath.Length && bindingPath.Path.StartsWith(this.FullNavigationPath, System.StringComparison.Ordinal))
                {
                    bindingPath = new EdmPathExpression(bindingPath.Path.Substring(this.FullNavigationPath.Length + 1));
                }
            }

            IEdmNavigationSource navigationTarget = base.FindNavigationTarget(navigationProperty, bindingPath);

            if (navigationTarget is IEdmUnknownEntitySet)
            {
                IEnumerable<string> segments = (bindingPath == null || string.IsNullOrEmpty(bindingPath.Path)) ? new string[] { navigationProperty.Name } : bindingPath.PathSegments;
                bindingPath = new EdmPathExpression(this.NavigationPath.PathSegments.Concat(segments));

                return this.parentNavigationSource.FindNavigationTarget(navigationProperty, bindingPath);
            }

            return navigationTarget;
        }

        private IEdmPathExpression ComputePath()
        {
            IEdmType targetType = this.navigationProperty.DeclaringType.AsElementType();
            List<string> newPath = new List<string>(this.parentNavigationSource.Path.PathSegments);
            if (!(targetType is IEdmComplexType) && !this.parentNavigationSource.Type.AsElementType().IsOrInheritsFrom(targetType))
            {
                newPath.Add(targetType.FullTypeName());
            }

            newPath.AddRange(this.NavigationPath.PathSegments);
            return new EdmPathExpression(newPath.ToArray());
        }
    }
}
