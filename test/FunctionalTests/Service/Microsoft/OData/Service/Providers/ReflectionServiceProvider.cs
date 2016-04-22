//---------------------------------------------------------------------
// <copyright file="ReflectionServiceProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Xml;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service.Caching;
    using CommonUtil = Microsoft.OData.Service.CommonUtil;

    #endregion Namespaces

    /// <summary>
    /// Provides a reflection-based provider implementation.
    /// </summary>
    [DebuggerDisplay("ReflectionServiceProvider: {Type}")]
    internal class ReflectionServiceProvider : BaseServiceProvider
    {
        /// <summary>
        /// EF provider behavior
        /// </summary>
        private static ProviderBehavior ReflectionProviderBehavior = new ProviderBehavior(ProviderQueryBehaviorKind.ReflectionProviderQueryBehavior);

        /// <summary>
        /// Initializes a new Microsoft.OData.Service.ReflectionServiceProvider instance.
        /// </summary>
        /// <param name="dataServiceInstance">data service instance.</param>
        /// <param name="dataSourceInstance">data source instance.</param>
        internal ReflectionServiceProvider(object dataServiceInstance, object dataSourceInstance)
            : base(dataServiceInstance, dataSourceInstance)
        {
        }

        /// <summary>Gets a value indicating whether null propagation is required in expression trees.</summary>
        public override bool IsNullPropagationRequired
        {
            get { return true; }
        }

        /// <summary>Namespace name for the EDM container.</summary>
        public override string ContainerNamespace
        {
            get { return this.DataSourceType.Namespace; }
        }

        /// <summary>Name of the EDM container</summary>
        public override string ContainerName
        {
            get { return this.DataSourceType.Name; }
        }

        #region IDataServiceProviderBehavior

        /// <summary>
        /// Instance of provider behavior that defines the assumptions service should make
        /// about the provider.
        /// </summary>
        public override ProviderBehavior ProviderBehavior
        {
            get { return ReflectionProviderBehavior; }
        }

        #endregion IDataServiceProviderBehavior

        /// <summary>Target type for the data provider </summary>
        private Type DataSourceType
        {
            [DebuggerStepThrough]
            get { return this.CurrentDataSource.GetType(); }
        }

        /// <summary>
        /// Gets the ResourceAssociationSet instance when given the source association end.
        /// </summary>
        /// <param name="resourceSet">Resource set of the source association end.</param>
        /// <param name="resourceType">Resource type of the source association end.</param>
        /// <param name="resourceProperty">Resource property of the source association end.</param>
        /// <returns>ResourceAssociationSet instance.</returns>
        public override ResourceAssociationSet GetResourceAssociationSet(ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            WebUtil.CheckArgumentNull(resourceSet, "resourceSet");
            WebUtil.CheckArgumentNull(resourceType, "resourceType");
            WebUtil.CheckArgumentNull(resourceProperty, "resourceProperty");

            ResourceSet knownSet;
            if (!this.TryResolveResourceSet(resourceSet.Name, out knownSet) || knownSet != resourceSet)
            {
                throw new InvalidOperationException(Strings.BadProvider_UnknownResourceSet(resourceSet.Name));
            }

            ResourceType knownType;
            if (!this.TryResolveResourceType(resourceType.FullName, out knownType) || knownType != resourceType)
            {
                throw new InvalidOperationException(Strings.BadProvider_UnknownResourceType(resourceType.FullName));
            }

            if (resourceType != resourceType.GetDeclaringTypeForProperty(resourceProperty))
            {
                throw new InvalidOperationException(Strings.BadProvider_ResourceTypeMustBeDeclaringTypeForProperty(resourceType.FullName, resourceProperty.Name));
            }

            ResourceType targetType = resourceProperty.ResourceType;
            if (targetType.ResourceTypeKind == ResourceTypeKind.ComplexType)
            {
                return null;
            }
            Debug.Assert(targetType != null, "targetType != null");
            if (targetType.ResourceTypeKind != ResourceTypeKind.EntityType)
            {
                throw new InvalidOperationException(Strings.BadProvider_PropertyMustBeNavigationPropertyOnType(resourceProperty.Name, resourceType.FullName));
            }

            ResourceSet targetSet = InternalGetContainerForResourceType(targetType.InstanceType, this.ResourceSets);
            Debug.Assert(targetSet != null, "targetSet != null");

            // In order to prevent this from becoming a breaking change, we use the name to build up the association whenever possible.
            // If there exists an entity with the same name, then we try and use the full name.
            string associationSetName;
            if (this.MetadataCacheItem.ResourceTypeCacheItems.Count(rt => rt.ResourceType.Name == resourceType.Name) > 1)
            {
                associationSetName = resourceType.FullName.Replace('.', '_') + '_' + resourceProperty.Name;
            }
            else
            {
                associationSetName = resourceType.Name + '_' + resourceProperty.Name;
            }

            // We changed the association logic on the server to create the correct navigation property for V4 to run the tests.
            // The association logic on server side needs to be removed.
            // Calculating targetResourceProperty value here is for V4 and it is not the correct code for V3.
            ResourceProperty targetResourceProperty = null;

            if (targetType != resourceType)
            {
                targetResourceProperty = GetResourcePropertyByResourcePropertyTypeInResourceSet(resourceType, targetSet);
            }
            else
            {
                targetResourceProperty = resourceProperty;
            }

            if (targetResourceProperty != null && !this.MetadataCacheItem.TargetResourcePropertiesCacheItems.ContainsKey(targetSet.Name + "_" + targetResourceProperty.Name))
            {
                this.MetadataCacheItem.TargetResourcePropertiesCacheItems.Add(targetSet.Name + "_" + targetResourceProperty.Name, resourceProperty);
            }

            ResourceAssociationSetEnd sourceEnd = new ResourceAssociationSetEnd(resourceSet, resourceType, resourceProperty);
            ResourceAssociationSetEnd targetEnd = new ResourceAssociationSetEnd(targetSet, targetType, targetResourceProperty);
            return new ResourceAssociationSet(associationSetName, sourceEnd, targetEnd);
        }

        #region IDataServiceQueryProvider Methods

        /// <summary>
        /// Returns the IQueryable that represents the container.
        /// </summary>
        /// <param name="container">resource set representing the entity set.</param>
        /// <returns>
        /// An IQueryable that represents the container; null if there is 
        /// no container for the specified name.
        /// </returns>
        public override IQueryable GetQueryRootForResourceSet(ResourceSet container)
        {
            WebUtil.CheckArgumentNull(container, "container");
            Func<object, IQueryable> queryRootDelegate = this.GetQueryRootDelegate(container);
            return queryRootDelegate(this.CurrentDataSource);
        }

        /// <summary>
        /// Returns the collection of open properties name and value for the given resource instance.
        /// </summary>
        /// <param name="target">instance of the resource.</param>
        /// <returns>Returns the collection of open properties name and value for the given resource instance. Currently not supported for Reflection provider.</returns>
        public override IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the value of the open property.
        /// </summary>
        /// <param name="target">instance of the resource type.</param>
        /// <param name="propertyName">name of the property.</param>
        /// <returns>the value of the open property. Currently this is not supported for Reflection provider.</returns>
        public override object GetOpenPropertyValue(object target, string propertyName)
        {
            throw new NotImplementedException();
        }

        #endregion IDataServiceQueryProvider Methods

        /// <summary>Checks whether the given property is a key property.</summary>
        /// <param name="property">property to check</param>
        /// <param name="keyKind">returns the key kind of the property, based on the heuristic it matches</param>
        /// <returns>true if this is a key property, else returns false</returns>
        internal static bool IsPropertyKeyProperty(PropertyInfo property, out ResourceKeyKind keyKind)
        {
            keyKind = (ResourceKeyKind)(-1);

            // Only primitive types are allowed to be keys.
            // Checks for generic to exclude Nullable<> value-type primitives, since we don't allows keys to be null.
            if (WebUtil.IsPrimitiveType(property.PropertyType) &&
                !property.PropertyType.IsGenericType)
            {
                KeyAttribute keyAttribute = property.ReflectedType.GetCustomAttributes(true).OfType<KeyAttribute>().FirstOrDefault();
                if (keyAttribute != null && keyAttribute.KeyNames.Contains(property.Name))
                {
                    keyKind = ResourceKeyKind.AttributedKey;
                    return true;
                }

                // For now, the key property must be {TypeName}Id or Id and the property
                // type must be primitive, since we do not support non-primitive types
                // as keys
                if (property.Name == property.DeclaringType.Name + "ID")
                {
                    keyKind = ResourceKeyKind.TypeNameId;
                    return true;
                }

                if (property.Name == "ID")
                {
                    keyKind = ResourceKeyKind.Id;
                    return true;
                }
            }

            return false;
        }

        /// <summary>Populates the metadata for this provider.</summary>
        /// <param name="metadataCacheItem">Instance of ProviderMetadataCacheItem in which metadata needs to be populated.</param>
        protected sealed override void PopulateMetadata(ProviderMetadataCacheItem metadataCacheItem)
        {
            Queue<ResourceType> unvisitedTypes = new Queue<ResourceType>();

            // Get the list of properties to be ignored.
            List<string> propertiesToBeIgnored = new List<string>(
                IgnorePropertiesAttribute.GetProperties(this.DataSourceType, true /*inherit*/, WebUtil.PublicInstanceBindingFlags));
            PropertyInfo[] properties = this.DataSourceType.GetProperties(WebUtil.PublicInstanceBindingFlags);
            foreach (PropertyInfo property in properties)
            {
                if (!propertiesToBeIgnored.Contains(property.Name) && property.CanRead && property.GetIndexParameters().Length == 0)
                {
                    Type elementType = BaseServiceProvider.GetIQueryableElement(property.PropertyType);
                    if (elementType != null)
                    {
                        // If the element type has key defined (in itself or one of its ancestors)
                        ResourceType resourceType = BuildHierarchyForEntityType(elementType, metadataCacheItem, unvisitedTypes, true /* entity type candidate */);
                        if (resourceType != null)
                        {
                            // We do not allow MEST scenario for reflection provider
                            foreach (KeyValuePair<string, ResourceSet> entitySetInfo in metadataCacheItem.EntitySets)
                            {
                                Type entitySetType = entitySetInfo.Value.ResourceType.InstanceType;
                                if (entitySetType.IsAssignableFrom(elementType))
                                {
                                    throw new InvalidOperationException(Strings.ReflectionProvider_MultipleEntitySetsForSameType(entitySetInfo.Value.Name, property.Name, entitySetType.FullName, resourceType.FullName));
                                }

                                if (elementType.IsAssignableFrom(entitySetType))
                                {
                                    throw new InvalidOperationException(Strings.ReflectionProvider_MultipleEntitySetsForSameType(property.Name, entitySetInfo.Value.Name, resourceType.FullName, entitySetType.FullName));
                                }
                            }

                            // Add the entity set to the list of entity sets.
                            ResourceSet resourceContainer = new ResourceSet(property.Name, resourceType);
                            metadataCacheItem.EntitySets.Add(property.Name, resourceContainer);
                            metadataCacheItem.QueryRootCache.Add(resourceContainer, this.BuildQueryRootDelegate(resourceContainer));
                        }
                        else
                        {
                            throw new InvalidOperationException(Strings.ReflectionProvider_InvalidEntitySetProperty(property.Name, XmlConvert.EncodeName(((IDataServiceMetadataProvider)this).ContainerName)));
                        }
                    }
                }
            }

            // Populate the metadata for all the types in unvisited types 
            // and also their properties and populates metadata about property types
            PopulateMetadataForTypes(metadataCacheItem, unvisitedTypes);

            // At this point, we should have all the top level entity types and the complex types
            PopulateMetadataForDerivedTypes(metadataCacheItem, unvisitedTypes);
        }

        /// <summary>
        /// Populate types for metadata specified by the provider
        /// </summary>
        /// <param name="userSpecifiedTypes">list of types specified by the provider</param>
        /// <param name="metadataCacheItem">Instance of ProviderMetadataCacheItem.</param>
        protected sealed override void PopulateMetadataForUserSpecifiedTypes(
            IEnumerable<Type> userSpecifiedTypes,
            ProviderMetadataCacheItem metadataCacheItem)
        {
            Queue<ResourceType> unvisitedTypes = new Queue<ResourceType>();
            foreach (Type type in userSpecifiedTypes)
            {
                ResourceType resourceType;
                if (TryGetType(metadataCacheItem, type, out resourceType))
                {
                    continue;
                }

                if (IsEntityOrComplexType(type, metadataCacheItem, unvisitedTypes) == null)
                {
                    throw new InvalidOperationException(Strings.BadProvider_InvalidTypeSpecified(type.FullName));
                }
            }

            PopulateMetadataForTypes(metadataCacheItem, unvisitedTypes);
        }

        /// <summary>
        /// Populate metadata for the given clr type.
        /// </summary>
        /// <param name="type">type whose metadata needs to be loaded.</param>
        /// <param name="metadataCacheItem">Instance of ProviderMetadataCacheItem.</param>
        /// <returns>resource type containing metadata for the given clr type.</returns>
        protected sealed override ResourceType PopulateMetadataForType(
            Type type,
            ProviderMetadataCacheItem metadataCacheItem)
        {
            Queue<ResourceType> unvisitedTypes = new Queue<ResourceType>();
            ResourceType resourceType;
            if (!TryGetType(metadataCacheItem, type, out resourceType))
            {
                resourceType = IsEntityOrComplexType(type, metadataCacheItem, unvisitedTypes);
                if (resourceType != null)
                {
                    PopulateMetadataForTypes(metadataCacheItem, unvisitedTypes);
                }
            }

            return resourceType;
        }

        /// <summary>Checks whether the specified type is a complex type.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>
        /// true if the specified type is a complex type; false otherwise. Note
        /// that resources are not distinguished from complex types.
        /// </returns>
        private static bool IsComplexType(Type type)
        {
            Debug.Assert(type != null, "type != null");

            // Complex types are all types that contain public properties of primitive
            // types.
            //
            // We purposefully ignore certain known classes which fit this description
            // but we know are not meaningful for Astoria:
            // - System.Array:  what would get serialized would be Length, IsFixed, etc.
            // - Pointers:      we would otherwise serialize the pointer size
            // - COM object wrappers
            // - interface: since we will never know what the exact type of the instance will be.
            if (!type.IsVisible || type.IsArray || type.IsPointer || type.IsCOMObject || type.IsInterface ||
                type == typeof(IntPtr) || type == typeof(UIntPtr) || type == typeof(char) ||
                type == typeof(TimeSpan) || type == typeof(DateTimeOffset) || type == typeof(Uri) ||
                type.IsEnum)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks whether there is a key defined for the given type.
        /// </summary>
        /// <param name="type">type to check </param>
        /// <param name="entityTypeCandidate">
        /// Whether <paramref name="type"/> is being considered as a possible 
        /// entity type.
        /// </param>
        /// <returns>returns true if there are one or key properties present else returns false</returns>
        private static bool DoesTypeHaveKeyProperties(Type type, bool entityTypeCandidate)
        {
            Debug.Assert(type != null, "type != null");

            // Check for properties declared on this element only
            foreach (PropertyInfo property in type.GetProperties(WebUtil.PublicInstanceBindingFlags | BindingFlags.DeclaredOnly))
            {
                ResourceKeyKind keyKind;
                if (IsPropertyKeyProperty(property, out keyKind))
                {
                    if (keyKind == ResourceKeyKind.AttributedKey && !entityTypeCandidate)
                    {
                        throw new InvalidOperationException(Strings.ReflectionProvider_EntityTypeHasKeyButNoEntitySet(type.FullName));
                    }

                    if (!entityTypeCandidate)
                    {
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Populates the metadata for the given unvisited types and all the associated types with this type
        /// </summary>
        /// <param name="metadataCacheItem">Instance of ProviderMetadataCacheItem.</param>
        /// <param name="unvisitedTypes">list of unvisited type</param>
        private static void PopulateMetadataForTypes(
            ProviderMetadataCacheItem metadataCacheItem,
            Queue<ResourceType> unvisitedTypes)
        {
            Debug.Assert(metadataCacheItem != null, "metadataCacheItem != null");
            Debug.Assert(unvisitedTypes != null, "unvisitedTypes != null");

            // Start walking down all the types
            while (unvisitedTypes.Count != 0)
            {
                // get the unvisited element
                ResourceType type = unvisitedTypes.Dequeue();

                // Go through all the properties and find out one or more complex types
                BuildTypeProperties(type, metadataCacheItem, unvisitedTypes);
            }
        }

        /// <summary>
        /// Walks through the list of ancestors and finds the root base type and collects metadata for the entire chain of ancestors
        /// </summary>
        /// <param name="type">type whose ancestors metadata needs to be populated</param>
        /// <param name="metadataCacheItem">Instance of ProviderMetadataCacheItem.</param>
        /// <param name="unvisitedTypes">list of unvisited types</param>
        /// <param name="entityTypeCandidate">Whether <paramref name="type"/> is a candidate to be an entity type.</param>
        /// <returns>return true if this given type is a entity type, otherwise returns false</returns>
        private static ResourceType BuildHierarchyForEntityType(
            Type type,
            ProviderMetadataCacheItem metadataCacheItem,
            Queue<ResourceType> unvisitedTypes,
            bool entityTypeCandidate)
        {
            List<Type> ancestors = new List<Type>();

            if (!type.IsVisible)
            {
                return null;
            }

            if (CommonUtil.IsUnsupportedType(type))
            {
                // deriving from an unsupported type is not allowed
                throw new InvalidOperationException(Strings.BadProvider_UnsupportedType(type.FullName));
            }

            Type baseType = type;
            ResourceType baseResourceType = null;

            // Since this method is also used on property types, which can be interfaces,
            // Base types can be null
            while (baseType != null)
            {
                // Try and check if the base type is already loaded
                if (TryGetType(metadataCacheItem, baseType, out baseResourceType))
                {
                    break;
                }

                ancestors.Add(baseType);
                baseType = baseType.BaseType;
            }

            if (baseResourceType == null)
            {
                // If entityTypeCandidate is false, then it means that the current type can't
                // be a entity type with keys. In other words, it must derive from an existing
                // type. Otherwise, its not an entity type
                if (entityTypeCandidate == false)
                {
                    return null;
                }

                // Find the last ancestor which has key defined
                for (int i = ancestors.Count - 1; i >= 0; i--)
                {
                    if (CommonUtil.IsUnsupportedType(ancestors[i]))
                    {
                        // deriving from an unsupported type is not allowed
                        throw new InvalidOperationException(Strings.BadProvider_UnsupportedAncestorType(type.FullName, ancestors[i].FullName));
                    }

                    if (DoesTypeHaveKeyProperties(ancestors[i], entityTypeCandidate))
                    {
                        break;
                    }

                    // Else this type is not interesting. Remove it from the ancestors list
                    ancestors.RemoveAt(i);
                }
            }
            else if (baseResourceType.ResourceTypeKind != ResourceTypeKind.EntityType)
            {
                return null;
            }
            else if (ancestors.Count == 0)
            {
                // we might have found the top level element.So just return
                return baseResourceType;
            }

            // For all the valid ancestors, add the type to the list of types encountered 
            // and unvisited types
            // its important that we enqueue the ancestors first, since when we populate member metadata
            // we can make sure that the base type is fully populated
            for (int i = ancestors.Count - 1; i >= 0; i--)
            {
                ResourceType entityType = ReflectionServiceProvider.CreateResourceType(ancestors[i], ResourceTypeKind.EntityType, baseResourceType, metadataCacheItem);
                unvisitedTypes.Enqueue(entityType);
                baseResourceType = entityType;
            }

            return baseResourceType;
        }

        /// <summary>
        /// Populates the metadata for the properties of the given resource type
        /// </summary>
        /// <param name="parentResourceType">resource type whose properties metadata needs to be populated</param>
        /// <param name="metadataCacheItem">Instance of ProviderMetadataCacheItem.</param>
        /// <param name="unvisitedTypes">list of unvisited type</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Pending")]
        private static void BuildTypeProperties(
            ResourceType parentResourceType,
            ProviderMetadataCacheItem metadataCacheItem,
            Queue<ResourceType> unvisitedTypes)
        {
            Debug.Assert(parentResourceType != null, "parentResourceType != null");
            Debug.Assert(metadataCacheItem != null, "metadataCacheItem != null");
            Debug.Assert(unvisitedTypes != null, "unvisitedTypes != null");

            BindingFlags bindingFlags = WebUtil.PublicInstanceBindingFlags;

            // For non root types, we should only look for properties that are declared for this type
            if (parentResourceType.BaseType != null)
            {
                bindingFlags = bindingFlags | BindingFlags.DeclaredOnly;
            }

            HashSet<string> propertiesToBeIgnored = new HashSet<string>(IgnorePropertiesAttribute.GetProperties(parentResourceType.InstanceType, false /*inherit*/, bindingFlags), StringComparer.Ordinal);
            Debug.Assert(parentResourceType.IsOpenType == false, "ReflectionServiceProvider does not support Open types.");

            HashSet<string> etagPropertyNames = new HashSet<string>(LoadETagProperties(parentResourceType), StringComparer.Ordinal);

            ResourceKeyKind keyKind = (ResourceKeyKind)Int32.MaxValue;
            PropertyInfo[] properties = parentResourceType.InstanceType.GetProperties(bindingFlags);

            // Should not allow System.Object on server
            // The general fix for this bug is to not support any resource type that doesn't have
            // any publically visible properties, including System.object and also custom types which don't have any properties.
            if (!properties.Any() && parentResourceType.BaseType == null)
            {
                throw new NotSupportedException(Strings.ReflectionProvider_ResourceTypeHasNoPublicallyVisibleProperties(parentResourceType.FullName));
            }

            foreach (PropertyInfo property in properties)
            {
                // Ignore the properties which are specified in the IgnoreProperties attribute
                if (propertiesToBeIgnored.Contains(property.Name))
                {
                    continue;
                }

                if (property.CanRead && property.GetIndexParameters().Length == 0)
                {
                    ResourcePropertyKind kind = (ResourcePropertyKind)(-1);
                    ResourceType resourceType;
                    Type resourcePropertyType = property.PropertyType;
                    bool collection = false;

                    if (!TryGetType(metadataCacheItem, resourcePropertyType, out resourceType))
                    {
                        Type collectionType = GetIEnumerableElement(property.PropertyType);
                        if (collectionType != null)
                        {
                            TryGetType(metadataCacheItem, collectionType, out resourceType);

                            // Even if the above method returns false, we should set the
                            // following variable appropriately, so that we can use them below
                            collection = true;
                            resourcePropertyType = collectionType;
                        }
                    }

                    if (resourceType != null)
                    {
                        #region Already Known Type
                        if (resourceType.ResourceTypeKind == ResourceTypeKind.Primitive)
                        {
                            if (collection)
                            {
                                // If it's a collection it can't be a key property (we don't allow collection properties as key)
                                kind = ResourcePropertyKind.Collection;
                            }
                            else
                            {
                                ResourceKeyKind currentKeyKind;
                                if (parentResourceType.BaseType == null && parentResourceType.ResourceTypeKind == ResourceTypeKind.EntityType && IsPropertyKeyProperty(property, out currentKeyKind))
                                {
                                    // Check for key property only on root types, since keys must be defined on the root types
                                    if ((int)currentKeyKind < (int)keyKind)
                                    {
                                        if (parentResourceType.KeyProperties.Count != 0)
                                        {
                                            // Remove the existing property as key property - mark it as non key property
                                            parentResourceType.RemoveKeyProperties();
                                        }

                                        keyKind = currentKeyKind;
                                        kind = ResourcePropertyKind.Key | ResourcePropertyKind.Primitive;
                                    }
                                    else if ((int)currentKeyKind == (int)keyKind)
                                    {
                                        Debug.Assert(currentKeyKind == ResourceKeyKind.AttributedKey, "This is the only way of specifying composite keys");
                                        kind = ResourcePropertyKind.Key | ResourcePropertyKind.Primitive;
                                    }
                                    else
                                    {
                                        kind = ResourcePropertyKind.Primitive;
                                    }
                                }
                                else
                                {
                                    kind = ResourcePropertyKind.Primitive;
                                }
                            }
                        }
                        else if (resourceType.ResourceTypeKind == ResourceTypeKind.ComplexType)
                        {
                            kind = collection ? ResourcePropertyKind.Collection : ResourcePropertyKind.ComplexType;
                        }
                        else if (resourceType.ResourceTypeKind == ResourceTypeKind.EntityType)
                        {
                            kind = collection ? ResourcePropertyKind.ResourceSetReference : ResourcePropertyKind.ResourceReference;
                        }
                        #endregion // Already Known Type
                    }
                    else
                    {
                        resourceType = IsEntityOrComplexType(resourcePropertyType, metadataCacheItem, unvisitedTypes);
                        if (resourceType != null)
                        {
                            if (resourceType.ResourceTypeKind == ResourceTypeKind.ComplexType)
                            {
                                if (collection)
                                {
                                    // For backward compat reasons only look closely on the item type if the property is a collection (IEnumerable<T>) 
                                    // and the T looks like a complex type.
                                    // In that case we used to only allow entity types as T, now with collection properties we also allow primitive and complex types,
                                    //   but we must explicitely disallow collection of collection, that is any T which implements IEnumerable<T>.
                                    // Note that we can't just make IEnumerable<T> not be a complex types since in certain cases we used to recognize it as such
                                    //   and we would break those cases. One example is user registered known types.
                                    Type collectionType = GetIEnumerableElement(resourcePropertyType);
                                    if (collectionType != null)
                                    {
                                        throw new InvalidOperationException(Strings.ReflectionProvider_CollectionOfCollectionProperty(property.Name, parentResourceType.FullName));
                                    }

                                    kind = ResourcePropertyKind.Collection;
                                }
                                else
                                {
                                    kind = ResourcePropertyKind.ComplexType;
                                }
                            }
                            else
                            {
                                Debug.Assert(resourceType.ResourceTypeKind == ResourceTypeKind.EntityType, "Must be an entity type");
                                kind = collection ? ResourcePropertyKind.ResourceSetReference : ResourcePropertyKind.ResourceReference;
                            }
                        }
                    }

                    // if resource type is null OR
                    // if complex type has a property of entity type
                    if (resourceType == null ||
                        (resourceType.ResourceTypeKind == ResourceTypeKind.EntityType && parentResourceType.ResourceTypeKind == ResourceTypeKind.ComplexType))
                    {
                        if (resourceType == null)
                        {
                            // Provide a better error message for collection of collection
                            if (collection && GetIEnumerableElement(resourcePropertyType) != null)
                            {
                                throw new InvalidOperationException(Strings.ReflectionProvider_CollectionOfCollectionProperty(property.Name, parentResourceType.FullName));
                            }

                            // Provide a better error message for collection of wrong types
                            if (collection)
                            {
                                throw new InvalidOperationException(Strings.ReflectionProvider_CollectionOfUnsupportedTypeProperty(property.Name, parentResourceType.FullName, resourcePropertyType));
                            }

                            if (CommonUtil.IsUnsupportedType(resourcePropertyType))
                            {
                                throw new InvalidOperationException(Strings.BadProvider_UnsupportedPropertyType(property.Name, parentResourceType.FullName));
                            }

                            throw new InvalidOperationException(Strings.ReflectionProvider_InvalidProperty(property.Name, parentResourceType.FullName));
                        }

                        // Navigation property on a complex type is not supported
                        throw new InvalidOperationException(Strings.ReflectionProvider_ComplexTypeWithNavigationProperty(property.Name, parentResourceType.FullName));
                    }

                    if (resourceType.ResourceTypeKind == ResourceTypeKind.EntityType)
                    {
                        ResourceSet container = InternalGetContainerForResourceType(resourcePropertyType, metadataCacheItem.EntitySets.Values);
                        if (container == null)
                        {
                            throw new InvalidOperationException(Strings.ReflectionProvider_EntityPropertyWithNoEntitySet(parentResourceType.FullName, property.Name));
                        }
                    }

                    if (kind == ResourcePropertyKind.Collection)
                    {
                        // Collection properties need the collection type (representing the IEnumerable<> part).
                        resourceType = ResourceType.GetCollectionResourceType(resourceType);
                    }

                    if (etagPropertyNames.Remove(property.Name))
                    {
                        kind |= ResourcePropertyKind.ETag;
                    }

                    ResourceProperty resourceProperty = new ResourceProperty(property.Name, kind, resourceType);
                    MimeTypeAttribute attribute = BaseServiceProvider.GetMimeTypeAttribute(property);
                    if (attribute != null)
                    {
                        resourceProperty.MimeType = attribute.MimeType;
                    }

                    parentResourceType.AddProperty(resourceProperty);
                }
                else
                {
                    throw new InvalidOperationException(Strings.ReflectionProvider_InvalidProperty(property.Name, parentResourceType.FullName));
                }
            }

            if (parentResourceType.ResourceTypeKind == ResourceTypeKind.EntityType &&
                (parentResourceType.KeyProperties == null || parentResourceType.KeyProperties.Count == 0))
            {
                throw new InvalidOperationException(Strings.ReflectionProvider_KeyPropertiesCannotBeIgnored(parentResourceType.FullName));
            }

            if (etagPropertyNames.Count != 0)
            {
                throw new InvalidOperationException(Strings.ReflectionProvider_ETagPropertyNameNotValid(etagPropertyNames.ElementAt(0), parentResourceType.FullName));
            }
        }

        /// <summary>
        /// If the given type is a entity or complex type, it returns the resource type corresponding to the given type
        /// </summary>
        /// <param name="type">clr type</param>
        /// <param name="metadataCacheItem">Instance of ProviderMetadataCacheItem.</param>
        /// <param name="unvisitedTypes">list of unvisited types</param>
        /// <returns>resource type corresponding to the given clr type, if the clr type is entity or complex</returns>
        private static ResourceType IsEntityOrComplexType(
            Type type,
            ProviderMetadataCacheItem metadataCacheItem,
            Queue<ResourceType> unvisitedTypes)
        {
            // Ignore values types here. We do not support resources of values type (entity or complex)
            if (type.IsValueType || CommonUtil.IsUnsupportedType(type))
            {
                return null;
            }

            ResourceType resourceType = BuildHierarchyForEntityType(type, metadataCacheItem, unvisitedTypes, false /* entityTypeCandidate */);
            if (resourceType == null && IsComplexType(type))
            {
                resourceType = ReflectionServiceProvider.CreateResourceType(type, ResourceTypeKind.ComplexType, null, metadataCacheItem);
                unvisitedTypes.Enqueue(resourceType);
            }

            return resourceType;
        }

        /// <summary>Get the resource set for the given clr type.</summary>
        /// <param name="type">clr type for which resource set name needs to be returned</param>
        /// <param name="entitySets">Available entity sets to consider.</param>
        /// <returns>The container for its type, null if not found.</returns>
        private static ResourceSet InternalGetContainerForResourceType(Type type, IEnumerable<ResourceSet> entitySets)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(entitySets != null, "entitySets != null");

            // For each entity set, find out which one matches the type of this resource
            foreach (ResourceSet entitySetInfo in entitySets)
            {
                if (entitySetInfo.ResourceType.InstanceType.IsAssignableFrom(type))
                {
                    return entitySetInfo;
                }
            }

            return null;
        }

        /// <summary>
        /// Find out all the derived types in the list of assemblies and then populate metadata for those types
        /// </summary>
        /// <param name="metadataCacheItem">Instance of ProviderMetadataCacheItem.</param>
        /// <param name="unvisitedTypes">list of unvisited types</param>
        private static void PopulateMetadataForDerivedTypes(
            ProviderMetadataCacheItem metadataCacheItem,
            Queue<ResourceType> unvisitedTypes)
        {
            Debug.Assert(metadataCacheItem != null, "metadataCacheItem != null");
            Debug.Assert(unvisitedTypes != null, "unvisitedTypes != null");

            // Find all the root resource entity types
            List<ResourceType> rootTypes = new List<ResourceType>();

            // To find the list of derived types, we should use the types
            // referred by the sets as the base type, to make sure we are
            // loading metadata only for types that can be referenced by
            // the given sets.
            foreach (ResourceSet resourceSet in metadataCacheItem.EntitySets.Values)
            {
                rootTypes.Add(resourceSet.ResourceType);
            }

            // Use the default comparer, which calls Assembly.Equals (not a simple reference comparison).
            HashSet<Assembly> assemblies = new HashSet<Assembly>(EqualityComparer<Assembly>.Default);
            List<Type> derivedTypes = new List<Type>();

            // Walk through all the types in the assemblies and find all the derived types
            foreach (var resourceTypeCacheItem in metadataCacheItem.ResourceTypeCacheItems)
            {
                var resourceType = resourceTypeCacheItem.ResourceType;

                // No need to look into primitive types, as these live in system assemblies.
                if (resourceType.ResourceTypeKind == ResourceTypeKind.Primitive)
                {
                    continue;
                }

                Assembly assembly = resourceType.InstanceType.Assembly;
                //// ignore if the assembly has already been scanned
                if (assemblies.Contains(assembly))
                {
                    continue;
                }

                // Walk all the types in that assembly
                foreach (Type type in assembly.GetTypes())
                {
                    // skip all the non visible types or types which have generic parameters
                    if (!type.IsVisible || HasGenericParameters(type))
                    {
                        continue;
                    }

                    // Skip the type if its already loaded
                    if (metadataCacheItem.TryGetResourceType(type) != null)
                    {
                        continue;
                    }

                    // Check if this type dervies from any one of the root types
                    for (int i = 0; i < rootTypes.Count; i++)
                    {
                        if (rootTypes[i].InstanceType.IsAssignableFrom(type))
                        {
                            derivedTypes.Add(type);
                        }
                    }
                }

                assemblies.Add(assembly);
            }

            foreach (Type type in derivedTypes)
            {
                BuildHierarchyForEntityType(type, metadataCacheItem, unvisitedTypes, false /* entityTypeCandidate */);
                PopulateMetadataForTypes(metadataCacheItem, unvisitedTypes);
            }
        }

        /// <summary>
        /// Loads the etag properties for the given resource type
        /// </summary>
        /// <param name="resourceType">resource type whose etag property names need to be loaded.</param>
        /// <returns>the list of properties that form the etag for the given resource type.</returns>
        private static IEnumerable<string> LoadETagProperties(ResourceType resourceType)
        {
            // if it is the root type, then we need to inherit the attribute from the base type.
            // otherwise, we need not, since if the base type already has it, the appropriate properties
            // must already have been marked as concurrency properties.
            bool inherit = resourceType.BaseType == null;

            // Read the etag attribute from the type and return it
            ETagAttribute[] attributes = (ETagAttribute[])resourceType.InstanceType.GetCustomAttributes(typeof(ETagAttribute), inherit);
            Debug.Assert(attributes.Length <= 1, "Only one attribute can be specified per type");

            if (attributes.Length == 1)
            {
                // Validate the property names
                // we may need to cache them instead of reading them everytime
                return attributes[0].PropertyNames;
            }

            return WebUtil.EmptyStringArray;
        }

        /// <summary>
        /// returns the new resource type instance
        /// </summary>
        /// <param name="type">backing clr type for the resource.</param>
        /// <param name="kind">kind of the resource.</param>
        /// <param name="baseType">base type of the resource.</param>
        /// <param name="metadataCacheItem">Instance of ProviderMetadataCacheItem.</param>
        /// <returns>returns a new instance of the resource type containing all the metadata.</returns>
        private static ResourceType CreateResourceType(
            Type type,
            ResourceTypeKind kind,
            ResourceType baseType,
            ProviderMetadataCacheItem metadataCacheItem)
        {
            ResourceType resourceType = new ResourceType(type, kind, baseType, type.Namespace, CommonUtil.GetModelTypeName(type), type.IsAbstract);
            resourceType.IsOpenType = false;

            // We need to look at inherited attributes as well so we pass true for inherit argument. 
            if (type.GetCustomAttributes(typeof(HasStreamAttribute), true /* inherit */).Length == 1)
            {
                resourceType.IsMediaLinkEntry = true;
            }

            // Add stream properties that are marked NamedStream in clrType to resourceType.
            AddStreamProperties(resourceType, type, baseType == null);

            metadataCacheItem.AddResourceType(type, resourceType);
            var childTypes = metadataCacheItem.ChildTypesCache;
            childTypes.Add(resourceType, null);
            if (baseType != null)
            {
                Debug.Assert(childTypes.ContainsKey(baseType), "childTypes.ContainsKey(baseType)");
                if (childTypes[baseType] == null)
                {
                    childTypes[baseType] = new List<ResourceType>();
                }

                childTypes[baseType].Add(resourceType);
            }

            return resourceType;
        }

        /// <summary>
        /// Checks whether the given type is a generic type with a generic parameter.
        /// </summary>
        /// <param name="type">type which needs to be checked.</param>
        /// <returns>Returns true, if the <paramref name="type"/> is generic and has generic parameters. Otherwise returns false.</returns>
        private static bool HasGenericParameters(Type type)
        {
            if (type.IsGenericType)
            {
                foreach (Type arg in type.GetGenericArguments())
                {
                    if (arg.IsGenericParameter)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Return the set of IL instructions for getting the IQueryable instance for the given ResourceSet.
        /// </summary>
        /// <param name="resourceSet">ResourceSet instance.</param>
        /// <returns>Func to invoke to get IQueryable for the given ResourceSet.</returns>
        private Func<object, IQueryable> BuildQueryRootDelegate(ResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            PropertyInfo propertyInfo = this.DataSourceType.GetProperty(resourceSet.Name, WebUtil.PublicInstanceBindingFlags);
            MethodInfo getValueMethod = propertyInfo.GetGetMethod();

            return new Func<object, IQueryable>(o => (IQueryable)getValueMethod.Invoke(o, null));
        }

        /// <summary>
        /// Get resource property by resource property type in resource set
        /// </summary>
        /// <param name="type">resource type</param>
        /// <param name="resourceSet">resource set</param>
        /// <returns>resource property</returns>
        private ResourceProperty GetResourcePropertyByResourcePropertyTypeInResourceSet(ResourceType type, ResourceSet resourceSet)
        {
            Debug.Assert(type != null, "resourceType != null");
            Debug.Assert(resourceSet != null, "resourceSet != null");

            foreach (ResourceProperty p in resourceSet.ResourceType.Properties)
            {
                if (p.ResourceType == type && !this.MetadataCacheItem.TargetResourcePropertiesCacheItems.ContainsKey(resourceSet.Name + "_" + p.Name))
                {
                    return p;
                }
            }

            return null;
        }
    }
}
