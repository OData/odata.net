//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library
{
    using Microsoft.OData.Edm.Library.Expressions;

    /// <summary>
    /// Represents an abstract EDM navigation source.
    /// </summary>
    public abstract class EdmNavigationSource : EdmNamedElement, IEdmNavigationSource
    {
        private readonly Dictionary<IEdmNavigationProperty, IEdmNavigationSource> navigationPropertyMappings = new Dictionary<IEdmNavigationProperty, IEdmNavigationSource>();

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
            this.navigationPropertyMappings[property] = target;
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
                IEdmNavigationSource result;
                if (this.navigationPropertyMappings.TryGetValue(property, out result))
                {
                    return result;
                }
            }
            else
            {
                IEdmContainedEntitySet containedEntitySet;
                if (!this.containedNavigationPropertyCache.TryGetValue(property, out containedEntitySet))
                {
                    containedEntitySet = new EdmContainedEntitySet(this, property);
                    this.containedNavigationPropertyCache.Add(property, containedEntitySet);
                }

                return containedEntitySet;
            }

            IEdmUnknownEntitySet unknownEntitySet;
            if (!this.unknownNavigationPropertyCache.TryGetValue(property, out unknownEntitySet))
            {
                unknownEntitySet = new EdmUnknownEntitySet(this, property);
                this.unknownNavigationPropertyCache.Add(property, unknownEntitySet);
            }

            return unknownEntitySet;
        }

        private IEnumerable<IEdmNavigationPropertyBinding> ComputeNavigationTargets()
        {
            List<IEdmNavigationPropertyBinding> result = new List<IEdmNavigationPropertyBinding>();
            foreach (KeyValuePair<IEdmNavigationProperty, IEdmNavigationSource> mapping in this.navigationPropertyMappings)
            {
                result.Add(new EdmNavigationPropertyBinding(mapping.Key, mapping.Value));
            }

            return result;
        }
    }
}
