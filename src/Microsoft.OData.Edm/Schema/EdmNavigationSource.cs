//---------------------------------------------------------------------
// <copyright file="EdmNavigationSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an abstract EDM navigation source.
    /// </summary>
    public abstract class EdmNavigationSource : EdmNamedElement, IEdmNavigationSource
    {
        private readonly Dictionary<IEdmNavigationProperty, Dictionary<string, IEdmNavigationPropertyBinding>> navigationPropertyMappings = new Dictionary<IEdmNavigationProperty, Dictionary<string, IEdmNavigationPropertyBinding>>();

        private readonly Dictionary<IEdmNavigationProperty, IEdmContainedEntitySet> containedNavigationPropertyCache = new Dictionary<IEdmNavigationProperty, IEdmContainedEntitySet>();

        private readonly Dictionary<IEdmNavigationProperty, IEdmUnknownEntitySet> unknownNavigationPropertyCache = new Dictionary<IEdmNavigationProperty, IEdmUnknownEntitySet>();

        private readonly Cache<EdmNavigationSource, IEnumerable<IEdmNavigationPropertyBinding>> navigationTargetsCache = new Cache<EdmNavigationSource, IEnumerable<IEdmNavigationPropertyBinding>>();
        private static readonly Func<EdmNavigationSource, IEnumerable<IEdmNavigationPropertyBinding>> ComputeNavigationTargetsFunc = (me) => me.ComputeNavigationTargets();

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmNavigationSource"/> class.
        /// </summary>
        /// <param name="name">Name of the navigation source.</param>
        protected EdmNavigationSource(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the navigation targets of this navigation source.
        /// </summary>
        public IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings
        {
            get { return this.navigationTargetsCache.GetValue(this, ComputeNavigationTargetsFunc, null); }
        }

        /// <summary>
        /// Gets the type of this navigation source.
        /// </summary>
        public abstract IEdmType Type { get; }

        /// <summary>
        /// Gets the path that a navigation property targets.
        /// </summary>
        public abstract IEdmPathExpression Path { get; }

        /// <summary>
        /// Adds a navigation target, specifying the destination entity set of a navigation property of an entity in this navigation source.
        /// </summary>
        /// <param name="navigationProperty">The navigation property the target is being set for.</param>
        /// <param name="target">The destination navigation source of the specified navigation property.</param>
        public void AddNavigationTarget(IEdmNavigationProperty navigationProperty, IEdmNavigationSource target)
        {
            EdmUtil.CheckArgumentNull(navigationProperty, "navigationProperty");
            EdmUtil.CheckArgumentNull(target, "navigation target");

            string path = navigationProperty.Name;

            if (!this.Type.AsElementType().IsOrInheritsFrom(navigationProperty.DeclaringType))
            {
                path = navigationProperty.DeclaringType.FullTypeName() + '/' + path;
            }

            if (!this.navigationPropertyMappings.ContainsKey(navigationProperty))
            {
                this.navigationPropertyMappings[navigationProperty] = new Dictionary<string, IEdmNavigationPropertyBinding>();
            }

            this.navigationPropertyMappings[navigationProperty][path] = new EdmNavigationPropertyBinding(navigationProperty, target, new EdmPathExpression(path));
            this.navigationTargetsCache.Clear(null);
        }

        /// <summary>
        /// Adds a navigation target, specifying the destination entity set of a navigation property of an entity in this navigation source.
        /// </summary>
        /// <param name="navigationProperty">The navigation property the target is being set for.</param>
        /// <param name="target">The destination navigation source of the specified navigation property.</param>
        /// <param name="bindingPath">Sets the path that the navigation property targets.</param>
        public void AddNavigationTarget(IEdmNavigationProperty navigationProperty, IEdmNavigationSource target, IEdmPathExpression bindingPath)
        {
            EdmUtil.CheckArgumentNull(navigationProperty, "navigationProperty");
            EdmUtil.CheckArgumentNull(target, "navigation target");
            EdmUtil.CheckArgumentNull(bindingPath, "binding path");

            if (navigationProperty.Name != bindingPath.PathSegments.Last())
            {
                throw new ArgumentException(Strings.NavigationPropertyBinding_PathIsNotValid);
            }

            if (!this.navigationPropertyMappings.ContainsKey(navigationProperty))
            {
                this.navigationPropertyMappings[navigationProperty] = new Dictionary<string, IEdmNavigationPropertyBinding>();
            }

            this.navigationPropertyMappings[navigationProperty][bindingPath.Path] = new EdmNavigationPropertyBinding(navigationProperty, target, bindingPath);
            this.navigationTargetsCache.Clear(null);
        }

        /// <summary>
        /// Finds the bindings of the navigation property.
        /// </summary>
        /// <param name="navigationProperty">The navigation property.</param>
        /// <returns>The list of bindings for current navigation property.</returns>
        public virtual IEnumerable<IEdmNavigationPropertyBinding> FindNavigationPropertyBindings(IEdmNavigationProperty navigationProperty)
        {
            EdmUtil.CheckArgumentNull(navigationProperty, "navigationProperty");

            Dictionary<string, IEdmNavigationPropertyBinding> result;
            if (this.navigationPropertyMappings.TryGetValue(navigationProperty, out result))
            {
                return result.Select(item => item.Value);
            }

            return null;
        }

        /// <summary>
        /// Finds the navigation source that a navigation property targets.
        /// </summary>
        /// <param name="navigationProperty">The navigation property.</param>
        /// <returns>The navigation source that the navigation proportion targets, or null if no such navigation source exists.</returns>
        public virtual IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty)
        {
            EdmUtil.CheckArgumentNull(navigationProperty, "property");

            bool isDerived = !this.Type.AsElementType().IsOrInheritsFrom(navigationProperty.DeclaringType);

            IEdmPathExpression bindingPath = isDerived
                ? new EdmPathExpression(navigationProperty.DeclaringType.FullTypeName(), navigationProperty.Name)
                : new EdmPathExpression(navigationProperty.Name);

            return FindNavigationTarget(navigationProperty, bindingPath);
        }

        /// <summary>
        /// Finds the navigation source that a navigation property targets.
        /// </summary>
        /// <param name="navigationProperty">The navigation property.</param>
        /// <param name="bindingPath">The binding path of the navigation property.</param>
        /// <returns>The navigation source that the navigation property targets</returns>
        public virtual IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty, IEdmPathExpression bindingPath)
        {
            EdmUtil.CheckArgumentNull(navigationProperty, "navigationProperty");

            if (!navigationProperty.ContainsTarget && bindingPath != null)
            {
                Dictionary<string, IEdmNavigationPropertyBinding> result;
                IEdmNavigationPropertyBinding binding;
                if (this.navigationPropertyMappings.TryGetValue(navigationProperty, out result) && result.TryGetValue(bindingPath.Path, out binding))
                {
                    return binding.Target;
                }
            }
            else if (navigationProperty.ContainsTarget)
            {
                return EdmUtil.DictionaryGetOrUpdate(
                    this.containedNavigationPropertyCache,
                    navigationProperty,
                    navProperty => new EdmContainedEntitySet(this, navProperty));
            }

            return EdmUtil.DictionaryGetOrUpdate(
                this.unknownNavigationPropertyCache,
                navigationProperty,
                navProperty => new EdmUnknownEntitySet(this, navProperty));
        }

        private IEnumerable<IEdmNavigationPropertyBinding> ComputeNavigationTargets()
        {
            List<IEdmNavigationPropertyBinding> result = new List<IEdmNavigationPropertyBinding>();
            foreach (KeyValuePair<IEdmNavigationProperty, Dictionary<string, IEdmNavigationPropertyBinding>> mapping in this.navigationPropertyMappings)
            {
                foreach (KeyValuePair<string, IEdmNavigationPropertyBinding> kv in mapping.Value)
                {
                    result.Add(kv.Value);
                }
            }

            return result;
        }
    }
}
