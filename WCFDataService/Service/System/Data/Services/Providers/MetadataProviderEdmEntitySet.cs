//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace System.Data.Services.Providers
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Strings = System.Data.Services.Strings;
    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmEntitySet"/> implementation backed by an IDSMP metadata provider.
    /// </summary>
    internal sealed class MetadataProviderEdmEntitySet : IResourceSetBasedEdmEntitySet
    {
        /// <summary>The edm model to which this instance belongs to.</summary>
        private readonly MetadataProviderEdmModel model;

        /// <summary>EntityContainer instance that this set belongs to.</summary>
        private readonly MetadataProviderEdmEntityContainer entityContainer;

        /// <summary>The resource set underlying this entity set.</summary>
        private readonly ResourceSetWrapper resourceSet;

        /// <summary>Mapping of navigation property to entity set.</summary>
        private Dictionary<IEdmNavigationProperty, MetadataProviderEdmEntitySet> navigationTargetMapping;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model">The edm model this instance belongs to.</param>
        /// <param name="entityContainer">Entity container instance that this set belongs to.</param>
        /// <param name="resourceSet">ResourceSet that this entity set represents.</param>
        internal MetadataProviderEdmEntitySet(MetadataProviderEdmModel model, MetadataProviderEdmEntityContainer entityContainer, ResourceSetWrapper resourceSet)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(entityContainer != null, "entityContainer != null");
            Debug.Assert(resourceSet != null, "resourceSet != null");

            this.model = model;
            this.entityContainer = entityContainer;
            this.resourceSet = resourceSet;
        }

        /// <summary>
        /// The resource-set wrapper that this entity-set was created from.
        /// </summary>
        public ResourceSetWrapper ResourceSet
        {
            get { return this.resourceSet; }
        }

        /// <summary>
        /// The element type of this entity set.
        /// </summary>
        public IEdmEntityType ElementType
        {
            get { return (IEdmEntityType)this.model.EnsureSchemaType(this.resourceSet.ResourceType); }
        }

        /// <summary>
        /// The entity container kind of the entity set; returns EdmContainerElementKind.EntitySet.
        /// </summary>
        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.EntitySet; }
        }

        /// <summary>
        /// Gets the name of this element.
        /// </summary>
        public string Name
        {
            get { return MetadataProviderUtils.GetEntitySetName(this.resourceSet.ResourceSet); }
        }

        /// <summary>
        /// Gets the list of navigation targets for this set.
        /// </summary>
        public Collections.Generic.IEnumerable<IEdmNavigationTargetMapping> NavigationTargets
        {
            get
            {
                // This should be called only in $metadata scenarios
                this.model.AssertCacheState(MetadataProviderState.Full);
                if (this.navigationTargetMapping != null)
                {
                    foreach (var mapping in this.navigationTargetMapping)
                    {
                        yield return new EdmNavigationTargetMapping(mapping.Key, mapping.Value);
                    }
                }
                else
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// EntityContainer instance that this set belongs to.
        /// </summary>
        public IEdmEntityContainer Container
        {
            get { return this.entityContainer; }
        }

        /// <summary>
        /// Finds the entity set that this navigation property refers to.
        /// </summary>
        /// <param name="navigationProperty">Instance of navigation property.</param>
        /// <returns>an instance of IEdmEntitySet that this navigation property refers to.</returns>
        public IEdmEntitySet FindNavigationTarget(IEdmNavigationProperty navigationProperty)
        {
            WebUtil.CheckArgumentNull(navigationProperty, "navigationProperty");
            MetadataProviderEdmEntitySet targetEntitySet = null;
            if (this.navigationTargetMapping == null || !this.navigationTargetMapping.TryGetValue(navigationProperty, out targetEntitySet))
            {
                string declaringTypeName = navigationProperty.DeclaringEntityType().FullName();
                ResourceType declaringResourceType = this.model.MetadataProvider.TryResolveResourceType(declaringTypeName);
                ResourceProperty navigationResourceProperty = declaringResourceType.TryResolvePropertiesDeclaredOnThisTypeByName(navigationProperty.Name);

                if (navigationResourceProperty != null && navigationResourceProperty.TypeKind == ResourceTypeKind.EntityType)
                {
                    // Calling this method causes the model to load all the metadata for the given association.
                    // Hence we do not need to add this to the target mapping explicitly
                    this.model.PairUpNavigationProperty(this.resourceSet, declaringResourceType, navigationResourceProperty);

                    // Since the entity set or target entity set might be hidden, the navigation target might not get added
                    // from the previous call
                    if (this.navigationTargetMapping != null)
                    {
                        this.navigationTargetMapping.TryGetValue(navigationProperty, out targetEntitySet);
                    }
                }
            }

            if (this.model.Mode == MetadataProviderEdmModelMode.SelectAndExpandParsing)
            {
                // When parsing $select/$expand, the URI parser has no way of knowing which sets are visible. So, if a navigation property is found
                // that does not have a target entity set, then it means that the target set is hidden. To avoid disclosing the existence of the set, act
                // as if the property does not exist.
                if (targetEntitySet == null)
                {
                    // We're playing a dangerous game here. We are trying to throw the SAME ERROR MESSAGE that the Uri Parser would throw
                    // for some property that is not found. If it looks any different, a client could detect the difference and learn about
                    // the existance of a navigation property that should have been hidden.
                    throw DataServiceException.CreateBadRequestError(Strings.BadRequest_InvalidPropertyNameSpecified(navigationProperty.Name, navigationProperty.DeclaringEntityType().FullName()));
                }
            }

            if (this.model.Mode == MetadataProviderEdmModelMode.UriPathParsing)
            {
                // As with select and expand, when parsing the path, the URI parser has no way of knowing which sets are visible. See above.
                if (targetEntitySet == null)
                {
                    // Same fragility as above.
                    throw DataServiceException.CreateResourceNotFound(navigationProperty.Name);
                }
            }

            return targetEntitySet;
        }

        /// <summary>
        /// Cache the entity set that the given navigation property refers to.
        /// </summary>
        /// <param name="navigationProperty">IEdmNavigationProperty that contains the navigation property metadata.</param>
        /// <param name="entitySet">Entityset that <paramref name="navigationProperty"/> refers to.</param>
        internal void EnsureNavigationTarget(IEdmNavigationProperty navigationProperty, MetadataProviderEdmEntitySet entitySet)
        {
            Debug.Assert(navigationProperty != null, "navigationProperty != null");
            Debug.Assert(entitySet != null, "entitySet != null");

            MetadataProviderEdmEntitySet targetEntitySet;
            if (this.navigationTargetMapping == null)
            {
                this.navigationTargetMapping = new Dictionary<IEdmNavigationProperty, MetadataProviderEdmEntitySet>(EqualityComparer<IEdmNavigationProperty>.Default);
            }

            if (!this.navigationTargetMapping.TryGetValue(navigationProperty, out targetEntitySet))
            {
                // Looks like this will get called multiple types for the same navigation property in MEST scenarios.
                // Looking at the EdmEntitySet, the last one wins. So keeping the same behavior here.
                this.navigationTargetMapping.Add(navigationProperty, entitySet);
            }
            else
            {
                Debug.Assert(targetEntitySet == entitySet, "targetEntitySet == entitySet");
            }
        }
    }
}
