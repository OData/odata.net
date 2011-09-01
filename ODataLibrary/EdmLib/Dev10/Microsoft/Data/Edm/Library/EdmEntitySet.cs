//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM entity set.
    /// </summary>
    public class EdmEntitySet : EdmElement, IEdmEntitySet, IDependencyTrigger, IDependent
    {
        private string name;
        private IEdmEntityType elementType;

        private readonly HashSetInternal<IDependencyTrigger> dependsOn = new HashSetInternal<IDependencyTrigger>();
        private readonly HashSetInternal<IDependent> dependents = new HashSetInternal<IDependent>();
        private readonly Dictionary<EdmNavigationProperty, IEdmEntitySet> navigationPropertyMappings = new Dictionary<EdmNavigationProperty, IEdmEntitySet>();

        // AssociationSets cache.
        private readonly Cache<EdmEntitySet, IEnumerable<IEdmAssociationSet>> associationSetsCache = new Cache<EdmEntitySet, IEnumerable<IEdmAssociationSet>>();
        private readonly static Func<EdmEntitySet, IEnumerable<IEdmAssociationSet>> s_computeAssociationSets = (me) => me.ComputeAssociationSets();

        /// <summary>
        /// Initializes a new instance of the EdmEntitySet class.
        /// </summary>
        /// <param name="name">Name of the entity set.</param>
        /// <param name="elementType">The entity type of the elements in this entity set.</param>
        public EdmEntitySet(string name, IEdmEntityType elementType)
        {
            this.name = name ?? string.Empty;
            this.ElementType = elementType;
        }

        /// <summary>
        /// Initializes a new instance of the EdmEntitySet class.
        /// </summary>
        public EdmEntitySet()
        {
            this.name = string.Empty;
        }

        /// <summary>
        /// Gets or sets the name of the entity set.
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.SetField(ref this.name, value ?? string.Empty); }
        }

        /// <summary>
        /// Gets or sets the entity type contained in this entity set.
        /// </summary>
        public IEdmEntityType ElementType
        {
            get
            {
                return this.elementType;
            }

            set
            {
                this.SetField(ref this.elementType, value);
                this.navigationPropertyMappings.Clear();
            }
        }

        /// <summary>
        /// Gets the kind of element of this container element.
        /// </summary>
        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.EntitySet; }
        }

        /// <summary>
        /// Adds a navigation target, specifying the destination entity set of a navigation property of an entity in this entity set.
        /// </summary>
        /// <param name="property">The navigation property the target is being set for.</param>
        /// <param name="target">The destination entity set of the specified navigation property.</param>
        public void AddNavigationTarget(EdmNavigationProperty property, IEdmEntitySet target)
        {
            this.navigationPropertyMappings[property] = target;
            this.associationSetsCache.Clear();
        }

        /// <summary>
        /// Removes a navigation target which specified the destination entity set of a navigation property of an entity in this entity set.
        /// </summary>
        /// <param name="property">The navigation property the target is being removed from</param>
        public void RemoveNavigationTarget(EdmNavigationProperty property)
        {
            this.navigationPropertyMappings.Remove(property);
            this.associationSetsCache.Clear();
        }

        /// <summary>
        /// Gets the collection of entity sets related to this entity set.
        /// </summary>
        public IEnumerable<IEdmAssociationSet> AssociationSets
        {
            get { return this.associationSetsCache.GetValue(this, s_computeAssociationSets, null); }
        }

        HashSetInternal<IDependent> IDependencyTrigger.Dependents
        {
            get { return this.dependents; }
        }

        HashSetInternal<IDependencyTrigger> IDependent.DependsOn
        {
            get { return this.dependsOn; }
        }

        void IFlushCaches.FlushCaches()
        {
            this.associationSetsCache.Clear();
        }

        private IEnumerable<IEdmAssociationSet> ComputeAssociationSets()
        {
            List<IEdmAssociationSet> associationSets = new List<IEdmAssociationSet>();

            foreach (IEdmNavigationProperty navigationProperty in this.ElementType.DeclaredNavigationProperties())
            {
                EdmNavigationProperty constructedProperty = navigationProperty as EdmNavigationProperty;
                if (constructedProperty != null && constructedProperty.IsFromPrincipal)
                {
                    IEdmEntitySet dependentEntitySet;
                    if (this.navigationPropertyMappings.TryGetValue(constructedProperty, out dependentEntitySet))
                    {
                        associationSets.Add(new AssociationSet(navigationProperty.To.DeclaringAssociation, dependentEntitySet, this));
                    } // ToDo: if the dependent entity set is null, see if there is a single entity set we could default to by the fact that it is the only set with the required entity type.
                }
            }

            return associationSets;
        }

        private class AssociationSet : EdmElement, IEdmAssociationSet
        {
            private readonly IEdmAssociation association;
            private readonly IEdmAssociationSetEnd end1;
            private readonly IEdmAssociationSetEnd end2;

            public AssociationSet(IEdmAssociation association, IEdmEntitySet fromPrincipalEntitySet, IEdmEntitySet toPrincipalEntitySet)
            {
                this.association = association;
                this.end1 = new AssociationSetEnd(association.End1, fromPrincipalEntitySet);
                this.end2 = new AssociationSetEnd(association.End2, toPrincipalEntitySet);
            }

            public IEdmAssociation Association
            {
                get { return this.association; }
            }

            public IEdmAssociationSetEnd End1
            {
                get { return this.end1; }
            }

            public IEdmAssociationSetEnd End2
            {
                get { return this.end2; }
            }

            public EdmContainerElementKind ContainerElementKind
            {
                get { return EdmContainerElementKind.AssociationSet; }
            }

            public string Name
            {
                get { return this.association.Name + "Set"; }
            }

            private class AssociationSetEnd : EdmElement, IEdmAssociationSetEnd
            {
                private readonly IEdmAssociationEnd role;
                private readonly IEdmEntitySet entitySet;

                public AssociationSetEnd(IEdmAssociationEnd role, IEdmEntitySet entitySet)
                {
                    this.role = role;
                    this.entitySet = entitySet;
                }

                public IEdmAssociationEnd Role
                {
                    get { return this.role; }
                }

                public IEdmEntitySet EntitySet
                {
                    get { return this.entitySet; }
                }
            }
        }
    }
}
