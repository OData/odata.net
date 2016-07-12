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
        private readonly Dictionary<IEdmNavigationProperty, Dictionary<string, IEdmNavigationSource>> navigationPropertyMappings = new Dictionary<IEdmNavigationProperty, Dictionary<string, IEdmNavigationSource>>();

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
        /// <param name="property">The navigation property the target is being set for.</param>
        /// <param name="target">The destination navigation source of the specified navigation property.</param>
        public void AddNavigationTarget(IEdmNavigationProperty property, IEdmNavigationSource target)
        {
            string path = property.Name;

            if (!this.Type.AsElementType().IsOrInheritsFrom(property.DeclaringType))
            {
                path = property.DeclaringType.FullTypeName() + '/' + path;
            }

            if (!this.navigationPropertyMappings.ContainsKey(property))
            {
                this.navigationPropertyMappings[property] = new Dictionary<string, IEdmNavigationSource>();
            }

            this.navigationPropertyMappings[property][path] = target;
            this.navigationTargetsCache.Clear(null);
        }

        /// <summary>
        /// Adds a navigation target, specifying the destination entity set of a navigation property of an entity in this navigation source.
        /// </summary>
        /// <param name="property">The navigation property the target is being set for.</param>
        /// <param name="target">The destination navigation source of the specified navigation property.</param>
        /// <param name="path">Sets the path that the navigation property targets.</param>
        public void AddNavigationTarget(IEdmNavigationProperty property, IEdmNavigationSource target, string path)
        {
            if (property.Name != path.Split('/').Last())
            {
                throw new ArgumentException(Strings.NavigationPropertyBinding_PathIsNotValid);
            }

            if (!this.navigationPropertyMappings.ContainsKey(property))
            {
                this.navigationPropertyMappings[property] = new Dictionary<string, IEdmNavigationSource>();
            }

            this.navigationPropertyMappings[property][path] = target;
            this.navigationTargetsCache.Clear(null);
        }

        /// <summary>
        /// Finds the navigation source that a navigation property targets.
        /// </summary>
        /// <param name="property">The navigation property.</param>
        /// <returns>The navigation source that the navigation propertion targets, or null if no such navigation source exists.</returns>
        public virtual IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty property)
        {
            EdmUtil.CheckArgumentNull(property, "property");

            if (!property.ContainsTarget)
            {
                Dictionary<string, IEdmNavigationSource> result;
                if (this.navigationPropertyMappings.TryGetValue(property, out result))
                {
                    return result.Values.FirstOrDefault();
                }
            }
            else
            {
                return EdmUtil.DictionaryGetOrUpdate(
                    this.containedNavigationPropertyCache,
                    property,
                    navProperty => new EdmContainedEntitySet(this, navProperty));
            }

            return EdmUtil.DictionaryGetOrUpdate(
                this.unknownNavigationPropertyCache,
                property,
                navProperty => new EdmUnknownEntitySet(this, navProperty));
        }

        private IEnumerable<IEdmNavigationPropertyBinding> ComputeNavigationTargets()
        {
            List<IEdmNavigationPropertyBinding> result = new List<IEdmNavigationPropertyBinding>();
            foreach (KeyValuePair<IEdmNavigationProperty, Dictionary<string, IEdmNavigationSource>> mapping in this.navigationPropertyMappings)
            {
                foreach (KeyValuePair<string, IEdmNavigationSource> kv in mapping.Value)
                {
                    result.Add(new EdmNavigationPropertyBinding(mapping.Key, kv.Value, kv.Key));
                }
            }

            return result;
        }
    }
}
