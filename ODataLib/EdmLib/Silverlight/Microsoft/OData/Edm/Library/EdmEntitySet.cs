//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Internal;

namespace Microsoft.OData.Edm.Library
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
