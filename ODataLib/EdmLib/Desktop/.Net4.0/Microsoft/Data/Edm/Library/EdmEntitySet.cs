//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM entity set.
    /// </summary>
    public class EdmEntitySet : EdmNamedElement, IEdmEntitySet
    {
        private readonly IEdmEntityContainer container;
        private readonly IEdmEntityType elementType;
        private readonly Dictionary<IEdmNavigationProperty, IEdmEntitySet> navigationPropertyMappings = new Dictionary<IEdmNavigationProperty, IEdmEntitySet>();

        private readonly Cache<EdmEntitySet, IEnumerable<IEdmNavigationTargetMapping>> navigationTargetsCache = new Cache<EdmEntitySet, IEnumerable<IEdmNavigationTargetMapping>>();
        private static readonly Func<EdmEntitySet, IEnumerable<IEdmNavigationTargetMapping>> ComputeNavigationTargetsFunc = (me) => me.ComputeNavigationTargets();

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEntitySet"/> class.
        /// </summary>
        /// <param name="container">An <see cref="IEdmEntityContainer"/> containing this entity set.</param>
        /// <param name="name">Name of the entity set.</param>
        /// <param name="elementType">The entity type of the elements in this entity set.</param>
        public EdmEntitySet(IEdmEntityContainer container, string name, IEdmEntityType elementType)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(container, "container");
            EdmUtil.CheckArgumentNull(elementType, "elementType");

            this.container = container;
            this.elementType = elementType;
        }

        /// <summary>
        /// Gets the entity type contained in this entity set.
        /// </summary>
        public IEdmEntityType ElementType
        {
            get { return this.elementType; }
        }

        /// <summary>
        /// Gets the kind of element of this container element.
        /// </summary>
        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.EntitySet; }
        }

        /// <summary>
        /// Gets the container of this entity set.
        /// </summary>
        public IEdmEntityContainer Container
        {
            get { return this.container; }
        }

        /// <summary>
        /// Gets the navigation targets of this entity set.
        /// </summary>
        public IEnumerable<IEdmNavigationTargetMapping> NavigationTargets
        {
            get { return this.navigationTargetsCache.GetValue(this, ComputeNavigationTargetsFunc, null); }
        }

        /// <summary>
        /// Adds a navigation target, specifying the destination entity set of a navigation property of an entity in this entity set.
        /// </summary>
        /// <param name="property">The navigation property the target is being set for.</param>
        /// <param name="target">The destination entity set of the specified navigation property.</param>
        public void AddNavigationTarget(IEdmNavigationProperty property, IEdmEntitySet target)
        {
            this.navigationPropertyMappings[property] = target;
            this.navigationTargetsCache.Clear(null);
        }

        /// <summary>
        /// Finds the entity set that a navigation property targets.
        /// </summary>
        /// <param name="property">The navigation property.</param>
        /// /// <returns>The entity set that the navigation propertion targets, or null if no such entity set exists.</returns>
        public IEdmEntitySet FindNavigationTarget(IEdmNavigationProperty property)
        {
            IEdmNavigationProperty navigationProperty = property as IEdmNavigationProperty;
            IEdmEntitySet result;
            if (navigationProperty != null && this.navigationPropertyMappings.TryGetValue(navigationProperty, out result))
            {
                return result;
            }

            return null;
        }

        private IEnumerable<IEdmNavigationTargetMapping> ComputeNavigationTargets()
        {
            List<IEdmNavigationTargetMapping> result = new List<IEdmNavigationTargetMapping>();
            foreach (KeyValuePair<IEdmNavigationProperty, IEdmEntitySet> mapping in this.navigationPropertyMappings)
            {
                result.Add(new EdmNavigationTargetMapping(mapping.Key, mapping.Value));
            }

            return result;
        }
    }
}
