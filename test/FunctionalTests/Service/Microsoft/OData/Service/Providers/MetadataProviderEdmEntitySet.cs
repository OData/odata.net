//---------------------------------------------------------------------
// <copyright file="MetadataProviderEdmEntitySet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using Strings = Microsoft.OData.Service.Strings;
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

        /// <summary>Mapping of navigation property to its bindings.</summary>
        private Dictionary<IEdmNavigationProperty, Dictionary<string, IEdmNavigationPropertyBinding>> navigationBindingMappings = new Dictionary<IEdmNavigationProperty, Dictionary<string, IEdmNavigationPropertyBinding>>(EqualityComparer<IEdmNavigationProperty>.Default);

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
        /// Gets the type of this navigation source.
        /// </summary>
        public IEdmType Type
        {
            get { return new EdmCollectionType(new EdmEntityTypeReference((EdmEntityType)this.model.EnsureSchemaType(this.resourceSet.ResourceType), false)); }
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
        public IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings
        {
            get
            {
                // This should be called only in $metadata scenarios
                this.model.AssertCacheState(MetadataProviderState.Full);
                List<IEdmNavigationPropertyBinding> result = new List<IEdmNavigationPropertyBinding>();
                foreach (KeyValuePair<IEdmNavigationProperty, Dictionary<string, IEdmNavigationPropertyBinding>> mapping in this.navigationBindingMappings)
                {
                    foreach (KeyValuePair<string, IEdmNavigationPropertyBinding> kv in mapping.Value)
                    {
                        result.Add(kv.Value);
                    }
                }

                return result;
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
        /// Gets the path that represents current path of the navigation source.
        /// </summary>
        public IEdmPathExpression Path
        {
            // TODO: implement path and type to support MetadataProviderEdmEntitySet.cs
            get { return null; }
        }

        public bool IncludeInServiceDocument
        {
            get; internal set;
        }

        /// <summary>
        /// Finds the entity set that this navigation property refers to.
        /// </summary>
        /// <param name="navigationProperty">Instance of navigation property.</param>
        /// <returns>an instance of IEdmEntitySet that this navigation property refers to.</returns>
        public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty)
        {
            WebUtil.CheckArgumentNull(navigationProperty, "navigationProperty");
            Dictionary<string, IEdmNavigationPropertyBinding> result = null;
            if (this.navigationBindingMappings == null || !this.navigationBindingMappings.TryGetValue(navigationProperty, out result))
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
                    if (this.navigationBindingMappings != null)
                    {
                        this.navigationBindingMappings.TryGetValue(navigationProperty, out result);
                    }
                }
            }

            if (this.model.Mode == MetadataProviderEdmModelMode.SelectAndExpandParsing)
            {
                // When parsing $select/$expand, the URI parser has no way of knowing which sets are visible. So, if a navigation property is found
                // that does not have a target entity set, then it means that the target set is hidden. To avoid disclosing the existence of the set, act
                // as if the property does not exist.
                if (result == null)
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
                if (result == null)
                {
                    // Same fragility as above.
                    throw DataServiceException.CreateResourceNotFound(navigationProperty.Name);
                }
            }

            return result == null ? null : result.Select(item => item.Value).SingleOrDefault().Target;
        }

        public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty, IEdmPathExpression bindingPath)
        {
            return FindNavigationTarget(navigationProperty);
        }

        public IEnumerable<IEdmNavigationPropertyBinding> FindNavigationPropertyBindings(IEdmNavigationProperty navigationProperty)
        {
            FindNavigationTarget(navigationProperty);
            Dictionary<string, IEdmNavigationPropertyBinding> result;
            if (this.navigationBindingMappings.TryGetValue(navigationProperty, out result))
            {
                return result.Select(item => item.Value);
            }

            return null;
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

            string path = navigationProperty.Name;

            if (!this.Type.AsElementType().IsOrInheritsFrom(navigationProperty.DeclaringType))
            {
                path = navigationProperty.DeclaringType.FullTypeName() + '/' + path;
            }

            if (!this.navigationBindingMappings.ContainsKey(navigationProperty))
            {
                this.navigationBindingMappings[navigationProperty] = new Dictionary<string, IEdmNavigationPropertyBinding>();
            }

            this.navigationBindingMappings[navigationProperty][path] = new EdmNavigationPropertyBinding(navigationProperty, entitySet, new EdmPathExpression(path));
        }
    }
}
