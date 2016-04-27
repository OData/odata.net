//---------------------------------------------------------------------
// <copyright file="ObjectContextServiceProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
#if EF6Provider
    using System.Data.Entity;
    using System.Data.Entity.Core;
    using System.Data.Entity.Core.EntityClient;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Core.Objects.DataClasses;
#else
    using System.Data.EntityClient;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
#endif
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Xml.Linq;
#if EF6Provider
    using Microsoft.OData.Service;
#endif
    using Microsoft.OData.Edm;
    using Microsoft.OData.Service.Caching;
    using Strings = Microsoft.OData.Service.Strings;

    #endregion Namespaces

    /// <summary>
    /// Provides a reflection-based provider implementation.
    /// </summary>
    [DebuggerDisplay("ObjectContextServiceProvider: {DataSourceType}")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Pending")]
    internal partial class ObjectContextServiceProvider : BaseServiceProvider, IDataServiceUpdateProvider2, IDataServiceEntityFrameworkProvider
    {
        #region Private fields

        /// <summary>
        /// EF provider behavior
        /// </summary>
        private static readonly ProviderBehavior EFProviderBehavior = new ProviderBehavior(ProviderQueryBehaviorKind.EntityFrameworkProviderQueryBehavior);

        /// <summary>
        /// List of objects that we need to be replaced. The key value indicates the current instance
        /// that will be replaced during SaveChanges. All the property changes are expected to happen
        /// on the value instance. At the time of SaveChanges, all the changes applied to the Value 
        /// instance are then applied to the instance present in Key and then it is saved.
        /// Since EF will always return the same reference for same key value by looking up the first
        /// level cache, we can assume reference equality for the objects thus obtained.
        /// </summary>
        private readonly Dictionary<object, object> objectsToBeReplaced = new Dictionary<object, object>(ReferenceEqualityComparer<object>.Instance);

        /// <summary>
        /// List of service actions to be invoked during SaveChanges().
        /// </summary>
        private readonly List<IDataServiceInvokable> actionsToInvoke = new List<IDataServiceInvokable>();

        /// <summary>List of cspace types for which ospace metadata couldn't be found.</summary>
        private List<StructuralType> typesWithoutOSpaceMetadata;

        /// <summary>Reference to the ObjectContext to use for operations</summary>
        private ObjectContext objectContext = null;

        /// <summary>Method to use to save changes to the context</summary>
        private Func<int> saveChangesMethod = null;

        #endregion Private fields

        /// <summary>
        /// Initializes a new Microsoft.OData.Service.ObjectContextServiceProvider instance.
        /// </summary>
        /// <param name="dataServiceInstance">instance of the data service.</param>
        /// <param name="dataSourceInstance">data source instance.</param>
        internal ObjectContextServiceProvider(object dataServiceInstance, object dataSourceInstance)
            : base(dataServiceInstance, dataSourceInstance)
        {
            this.typesWithoutOSpaceMetadata = new List<StructuralType>();
        }

        #region Properties

        /// <summary>Gets a value indicating whether null propagation is required in expression trees.</summary>
        public override bool IsNullPropagationRequired
        {
            get { return false; }
        }

        /// <summary>Namespace name for the EDM container.</summary>
        public override string ContainerNamespace
        {
            get { return this.DataSourceType.Namespace; }
        }

        /// <summary>Name of the EDM container</summary>
        public override string ContainerName
        {
            get
            {
                string defaultContainerName = this.ObjectContext.DefaultContainerName;
#if !EF6Provider
                // If the default container name is null, we use the container name of the first resource set that has a container name.
                if (string.IsNullOrEmpty(defaultContainerName))
                {
                    var firstResourceSet = this.ResourceSets.FirstOrDefault(set => !string.IsNullOrEmpty(set.EntityContainerName));
                    if (firstResourceSet != null)
                    {
                        defaultContainerName = firstResourceSet.EntityContainerName;
                    }
                }
#endif
                return defaultContainerName;
            }
        }

        #region IDataServiceEntityFrameworkProvider

        /// <summary>Return the schema version for the EF provider.</summary>
        public MetadataEdmSchemaVersion EdmSchemaVersion
        {
            get
            {
                return MetadataEdmSchemaVersion.Version4Dot0;
            }
        }

        #endregion IDataServiceEntityFrameworkProvider

        #region IDataServiceProviderBehavior

        /// <summary>
        /// Instance of provider behavior that defines the assumptions service should make
        /// about the provider.
        /// </summary>
        public override ProviderBehavior ProviderBehavior
        {
            get { return EFProviderBehavior; }
        }

        #endregion IDataServiceProviderBehavior

        /// <summary>Strongly-types instance being reflected upon.</summary>
        private ObjectContext ObjectContext
        {
            get
            {
                if (this.objectContext == null)
                {
                    DbContextHelper.GetObjectContext(this.CurrentDataSource, out this.objectContext, out this.saveChangesMethod);
                }

                Debug.Assert(this.objectContext != null, "Data source is neither an ObjectContext nor a DbContext");
                return this.objectContext;
            }
        }

        /// <summary>Target type for the data provider </summary>
        private Type DataSourceType
        {
            [DebuggerStepThrough]
            get { return this.CurrentDataSource.GetType(); }
        }

        #endregion Properties

        /// <summary>
        /// Gets the ResourceAssociationSet instance when given the source association end.
        /// </summary>
        /// <param name="resourceSet">Resource set of the source association end.</param>
        /// <param name="resourceType">Resource type of the source association end.</param>
        /// <param name="resourceProperty">Resource property of the source association end.</param>
        /// <returns>ResourceAssociationSet instance.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1506", Justification = "Fix it")]
        public override ResourceAssociationSet GetResourceAssociationSet(ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            WebUtil.CheckArgumentNull(resourceSet, "resourceSet");
            WebUtil.CheckArgumentNull(resourceType, "resourceType");
            WebUtil.CheckArgumentNull(resourceProperty, "resourceProperty");

            // Get source set
            EntitySet sourceEntitySet = this.GetEntitySet(resourceSet.Name);
            Debug.Assert(sourceEntitySet != null, "entitySet != null -- GetEntitySet should never return null");

            // Get the source type
            EntityType sourceEntityType = this.ObjectContext.MetadataWorkspace.GetItem<EntityType>(resourceType.FullName, DataSpace.CSpace);
            Debug.Assert(sourceEntityType != null, "entityType != null");

            // Get source navigation property
            NavigationProperty sourceNavigationProperty;
            sourceEntityType.NavigationProperties.TryGetValue(resourceProperty.Name, false /*ignoreCase*/, out sourceNavigationProperty);
            if (sourceNavigationProperty == null)
            {
                throw new InvalidOperationException(Strings.BadProvider_PropertyMustBeNavigationPropertyOnType(resourceProperty.Name, resourceType.FullName));
            }

            if (sourceEntityType != (EntityType)sourceNavigationProperty.DeclaringType || resourceType != resourceType.GetDeclaringTypeForProperty(resourceProperty))
            {
                throw new InvalidOperationException(Strings.BadProvider_ResourceTypeMustBeDeclaringTypeForProperty(resourceType.FullName, resourceProperty.Name));
            }

            ResourceAssociationSet result = null;
            foreach (AssociationSet associationSet in sourceEntitySet.EntityContainer.BaseEntitySets.OfType<AssociationSet>())
            {
                if (associationSet.ElementType == sourceNavigationProperty.RelationshipType)
                {
                    // from AssociationSetEnd
                    AssociationSetEnd setEnd = associationSet.AssociationSetEnds[sourceNavigationProperty.FromEndMember.Name];
                    if (setEnd.EntitySet == sourceEntitySet)
                    {
                        // from ResourceAssociationSetEnd
                        ResourceAssociationSetEnd thisAssociationSetEnd = ObjectContextServiceProvider.PopulateResourceAssociationSetEnd(setEnd, resourceSet, resourceType, resourceProperty);
#if !EF6Provider
                        // from ResourceAssociationTypeEnd
                        ResourceAssociationTypeEnd thisAssociationTypeEnd = ObjectContextServiceProvider.PopulateResourceAssociationTypeEnd(
                            setEnd.CorrespondingAssociationEndMember,
                            resourceType,
                            resourceProperty);
#endif

                        // to AssociationSetEnd
                        setEnd = associationSet.AssociationSetEnds[sourceNavigationProperty.ToEndMember.Name];

                        // Get the target resource set
                        EntitySet targetEntitySet = setEnd.EntitySet;
                        string targetEntitySetName = GetEntitySetName(targetEntitySet.Name, targetEntitySet.EntityContainer.Name, this.ObjectContext.DefaultContainerName == targetEntitySet.EntityContainer.Name);
                        ResourceSet targetResourceSet;
                        ((IDataServiceMetadataProvider)this).TryResolveResourceSet(targetEntitySetName, out targetResourceSet);
                        Debug.Assert(targetResourceSet != null, "targetResourceSet != null");

                        // Get the target resource type
                        EntityType targetEntityType = (EntityType)((RefType)sourceNavigationProperty.ToEndMember.TypeUsage.EdmType).ElementType;
                        ResourceType targetResourceType;
                        ((IDataServiceMetadataProvider)this).TryResolveResourceType(targetEntityType.FullName, out targetResourceType);
                        Debug.Assert(targetResourceType != null, "targetResourceType != null");

                        // Get the target resource property
                        ResourceProperty targetResourceProperty = null;
                        foreach (NavigationProperty navProperty in targetEntityType.NavigationProperties)
                        {
                            if (navProperty.ToEndMember == sourceNavigationProperty.FromEndMember)
                            {
                                targetResourceProperty = targetResourceType.TryResolvePropertyName(navProperty.Name, exceptKind: ResourcePropertyKind.Stream);
                                break;
                            }
                        }

                        // to ResourceAssociationSetEnd
                        ResourceAssociationSetEnd relatedAssociationSetEnd = ObjectContextServiceProvider.PopulateResourceAssociationSetEnd(
                            setEnd,
                            targetResourceSet,
                            targetResourceType,
                            (resourceType == targetResourceType && resourceProperty == targetResourceProperty) ? null : targetResourceProperty);
#if !EF6Provider
                        // to ResourceAssociationTypeEnd
                        ResourceAssociationTypeEnd relatedAssociationTypeEnd = ObjectContextServiceProvider.PopulateResourceAssociationTypeEnd(
                            setEnd.CorrespondingAssociationEndMember,
                            targetResourceType,
                            relatedAssociationSetEnd.ResourceProperty);
#endif
                        result = new ResourceAssociationSet(associationSet.Name, thisAssociationSetEnd, relatedAssociationSetEnd);

#if !EF6Provider
                        ObjectContextServiceProvider.PopulateAnnotations(associationSet.MetadataProperties, result.AddCustomAnnotation);

                        result.ResourceAssociationType = ObjectContextServiceProvider.PopulateResourceAssociationType(
                            associationSet.ElementType,
                            thisAssociationTypeEnd,
                            relatedAssociationTypeEnd);
#endif
                        break;
                    }
                }
            }

            return result;
        }

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
            ObjectQuery query = (ObjectQuery)queryRootDelegate(this.ObjectContext);
            query.MergeOption = MergeOption.NoTracking;
            return query;
        }
        
        /// <summary>
        /// Returns the collection of open properties name and value for the given resource instance.
        /// </summary>
        /// <param name="target">instance of the resource.</param>
        /// <returns>Returns the collection of open properties name and value for the given resource instance. Currently not supported for ObjectContext provider.</returns>
        public override IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the value of the open property.
        /// </summary>
        /// <param name="target">instance of the resource type.</param>
        /// <param name="propertyName">name of the property.</param>
        /// <returns>the value of the open property. Currently this is not supported for ObjectContext providers.</returns>
        public override object GetOpenPropertyValue(object target, string propertyName)
        {
            throw new NotImplementedException();
        }

        #region IUpdatable Members

        /// <summary>
        /// Creates the resource of the given type and belonging to the given container
        /// </summary>
        /// <param name="containerName">container name to which the resource needs to be added</param>
        /// <param name="fullTypeName">full type name i.e. Namespace qualified type name of the resource</param>
        /// <returns>object representing a resource of given type and belonging to the given container</returns>
        public virtual object CreateResource(string containerName, string fullTypeName)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(fullTypeName, "fullTypeName");
            ResourceType resourceType;
            if (!((IDataServiceMetadataProvider)this).TryResolveResourceType(fullTypeName, out resourceType))
            {
                throw new InvalidOperationException(Strings.ObjectContext_ResourceTypeNameNotExist(fullTypeName));
            }

            Debug.Assert(resourceType != null, "resourceType != null");
            if (resourceType.InstanceType.IsAbstract)
            {
                throw ObjectContextServiceProvider.CreateBadRequestError(Strings.CannotCreateInstancesOfAbstractType(resourceType.FullName));
            }

            object resource;
            if (containerName != null)
            {
                if (resourceType.ResourceTypeKind != ResourceTypeKind.EntityType)
                {
                    throw new InvalidOperationException(Strings.ObjectContext_EntityTypeExpected(resourceType.FullName, resourceType.ResourceTypeKind));
                }

                resource = CreateObject(this.ObjectContext, resourceType.InstanceType);
                this.ObjectContext.AddObject(containerName, resource);
            }
            else
            {
                // When the container name is null, it means we are trying to create a instance of complex types.
                if (resourceType.ResourceTypeKind != ResourceTypeKind.ComplexType)
                {
                    throw new InvalidOperationException(Strings.ObjectContext_ComplexTypeExpected(resourceType.FullName, resourceType.ResourceTypeKind));
                }

                resource = this.GetConstructorDelegate(resourceType)();
            }

            return resource;
        }

        /// <summary>
        /// Gets the resource of the given type that the query points to
        /// </summary>
        /// <param name="query">query pointing to a particular resource</param>
        /// <param name="fullTypeName">full type name i.e. Namespace qualified type name of the resource</param>
        /// <returns>object representing a resource of given type and as referenced by the query</returns>
        public virtual object GetResource(IQueryable query, string fullTypeName)
        {
            WebUtil.CheckArgumentNull(query, "query");
            ObjectQuery objectQuery = (ObjectQuery)query;

            objectQuery.MergeOption = MergeOption.AppendOnly;
            object result = null;
            foreach (object resource in objectQuery)
            {
                if (result != null)
                {
                    throw new InvalidOperationException(Strings.SingleResourceExpected);
                }

                result = resource;
            }

            if (result != null)
            {
                ResourceType resourceType = this.GetResourceType(result);
                if (resourceType == null)
                {
                    throw new InvalidOperationException(Strings.ObjectContext_UnknownResourceTypeForClrType(result.GetType().FullName));
                }

                if (fullTypeName != null && resourceType.FullName != fullTypeName)
                {
                    throw ObjectContextServiceProvider.CreateBadRequestError(Strings.TargetElementTypeOfTheUriSpecifiedDoesNotMatchWithTheExpectedType(resourceType.FullName, fullTypeName));
                }
            }

            return result;
        }

        /// <summary>
        /// Resets the value of the given resource to its default value
        /// </summary>
        /// <param name="resource">resource whose value needs to be reset</param>
        /// <returns>same resource with its value reset</returns>
        public virtual object ResetResource(object resource)
        {
            WebUtil.CheckArgumentNull(resource, "resource");
            var resourceTypeCacheItem = this.ResolveNonPrimitiveTypeCacheItem(resource.GetType());
            if (resourceTypeCacheItem == null)
            {
                throw new InvalidOperationException(Strings.ObjectContext_UnknownResourceTypeForClrType(resource.GetType().FullName));
            }

            var resourceType = resourceTypeCacheItem.ResourceType;
            if (resourceType.ResourceTypeKind == ResourceTypeKind.EntityType)
            {
                // For entity types, do the following:
                // create a new instance of the same type and set the key values on it
                object newInstance = CreateObject(this.ObjectContext, resourceType.InstanceType);

                // set the key value on the new instance
                foreach (ResourceProperty property in resourceType.KeyProperties)
                {
                    object propertyValue = ((IDataServiceQueryProvider)this).GetPropertyValue(resource, property);
                    this.SetValue(newInstance, resourceTypeCacheItem, propertyValue, property);
                }

                // When reset resource, we return the old instance since it's the one
                // EF actually attached to.
                // but all property modification will be done on the newInstance
                // upon save changes, the modifications on newInstance will be merged with
                // the old instance (property by property).
                this.objectsToBeReplaced.Add(resource, newInstance);

                ObjectStateEntry objectStateEntry = this.ObjectContext.ObjectStateManager.GetObjectStateEntry(resource);
                if (objectStateEntry.State == EntityState.Added)
                {
                    // in case when the resource is been added, and we PUT to that resource
                    // we'll actually insert the newInstance and detach the old one. 
                    // So we need to return the newInstance instead.
                    this.ObjectContext.AddObject(ObjectContextServiceProvider.GetEntitySetName(objectStateEntry, this.ObjectContext.DefaultContainerName), newInstance);
                    return newInstance;
                }
            }
            else if (resourceType.ResourceTypeKind == ResourceTypeKind.ComplexType)
            {
                // For complex types, just return a brand new instance.
                return this.GetConstructorDelegate(resourceType)();
            }

            return resource;
        }

        /// <summary>
        /// Sets the value of the given property on the target object
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="propertyValue">value of the property</param>
        public virtual void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            WebUtil.CheckArgumentNull(targetResource, "targetResource");
            WebUtil.CheckStringArgumentNullOrEmpty(propertyName, "propertyName");
            ResourceTypeCacheItem resourceTypeCacheItem = this.ResolveNonPrimitiveTypeCacheItem(targetResource.GetType());
            if (resourceTypeCacheItem == null)
            {
                throw new InvalidOperationException(Strings.ObjectContext_UnknownResourceTypeForClrType(targetResource.GetType().FullName));
            }

            var resourceType = resourceTypeCacheItem.ResourceType;
            ResourceProperty resourceProperty = resourceType.TryResolvePropertyName(propertyName, exceptKind: ResourcePropertyKind.Stream);
            if (resourceProperty == null)
            {
                throw new InvalidOperationException(Strings.ObjectContext_PropertyNotDefinedOnType(resourceType.FullName, propertyName));
            }

            // is target Resource going to be replaced?
            // See comment in ResetResources
            object replacedTarget;
            if (this.objectsToBeReplaced.TryGetValue(targetResource, out replacedTarget))
            {
                this.SetValue(replacedTarget, resourceTypeCacheItem, propertyValue, resourceProperty);
            }
            else if (resourceType.ResourceTypeKind == ResourceTypeKind.EntityType && resourceProperty.IsOfKind(ResourcePropertyKind.Primitive))
            {
                try
                {
                    // Dev11 The reason for using EF current values api is that in case of POCO, EF will know 
                    // which properties are getting updated and can do some fixups, when we are changing both FK and navigation
                    // properties. If we use reflection, EF has no clue about if the property is changed or not.
                    ObjectStateEntry entry = this.ObjectContext.ObjectStateManager.GetObjectStateEntry(targetResource);
                    int propertyOrdinal = entry.CurrentValues.GetOrdinal(propertyName);
                    entry.CurrentValues.SetValue(propertyOrdinal, propertyValue);
                }
                catch (InvalidOperationException exception)
                {
                    throw ObjectContextServiceProvider.CreateBadRequestError(Strings.BadRequest_ErrorInSettingPropertyValue(resourceProperty.Name), exception);
                }

                // Ideally we should have caught the ConstraintException and all other types of exception here, and then throw DataServiceException
                // Since we did not do this in V1/V2, this might be a breaking change now.
            }
            else
            {
                this.SetValue(targetResource, resourceTypeCacheItem, propertyValue, resourceProperty);
            }
        }

        /// <summary>
        /// Gets the value of the given property on the target object
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <returns>the value of the property for the given target resource</returns>
        public virtual object GetValue(object targetResource, string propertyName)
        {
            WebUtil.CheckArgumentNull(targetResource, "targetResource");
            WebUtil.CheckStringArgumentNullOrEmpty(propertyName, "propertyName");
            ResourceType resourceType = this.GetResourceType(targetResource);
            if (resourceType == null)
            {
                throw new InvalidOperationException(Strings.ObjectContext_UnknownResourceTypeForClrType(targetResource.GetType().FullName));
            }

            ResourceProperty resourceProperty = resourceType.TryResolvePropertyName(propertyName, exceptKind: ResourcePropertyKind.Stream);
            if (resourceProperty == null)
            {
                throw new InvalidOperationException(Strings.ObjectContext_PropertyNotDefinedOnType(resourceType.FullName, propertyName));
            }

            // For setting the values of the complex type, we always create a new instance of the complex type
            // since we treat them as atomic. Hence we need to do reflection to get/set complex type values.
            // EF API returns an instance of ObjectStateEntryDbUpdatableDataRecord for complex type properties -
            // hence sometimes we are working with instances, and sometimes not. Hence make sure we use the API
            // for primitive properties only
            if (resourceProperty.IsOfKind(ResourcePropertyKind.Primitive))
            {
                ObjectStateEntry entry = this.ObjectContext.ObjectStateManager.GetObjectStateEntry(targetResource);
                return entry.CurrentValues[propertyName];
            }

            // When target is in the list of modified resources, we should return the new instance.
            // See comments in ResetResources
            object replacedTarget;
            if (this.objectsToBeReplaced.TryGetValue(targetResource, out replacedTarget))
            {
                targetResource = replacedTarget;
            }

            object resource = ((IDataServiceQueryProvider)this).GetPropertyValue(targetResource, resourceProperty);
            return resource;
        }

        /// <summary>
        /// Sets the value of the given reference property on the target object
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="propertyValue">value of the property</param>
        public virtual void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            this.UpdateRelationship(targetResource, propertyName, propertyValue, null);
        }

        /// <summary>
        /// Adds the given value to the collection
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="resourceToBeAdded">value of the property which needs to be added</param>
        public virtual void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            this.UpdateRelationship(targetResource, propertyName, resourceToBeAdded, true /*addRelationship*/);
        }

        /// <summary>
        /// Removes the given value from the collection
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="resourceToBeRemoved">value of the property which needs to be removed</param>
        public virtual void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            this.UpdateRelationship(targetResource, propertyName, resourceToBeRemoved, false /*addRelationship*/);
        }

        /// <summary>
        /// Delete the given resource
        /// </summary>
        /// <param name="resource">resource that needs to be deleted</param>
        public virtual void DeleteResource(object resource)
        {
            WebUtil.CheckArgumentNull(resource, "resource");
            this.ObjectContext.DeleteObject(resource);
        }

        /// <summary>
        /// Saves all the pending changes made till now
        /// </summary>
        [SuppressMessage("DataWeb.Usage", "AC0014", Justification = "Throws every time, just wraps some types")]
        public virtual void SaveChanges()
        {
            foreach (IDataServiceInvokable action in this.actionsToInvoke)
            {
                action.Invoke();
            }

            // handle the resource which need to be replaced.
            foreach (KeyValuePair<object, object> objectToBeReplaced in this.objectsToBeReplaced)
            {
                ObjectContextServiceProvider.ApplyChangesToEntity(this.ObjectContext, objectToBeReplaced.Key, objectToBeReplaced.Value);
            }

            // clear all these once we have processed all the entities that need to be replaced.
            this.objectsToBeReplaced.Clear();
            this.actionsToInvoke.Clear();

            try
            {
                // Save Changes
                this.SaveContextChanges();
            }
            catch (OptimisticConcurrencyException e)
            {
                throw new DataServiceException((int)HttpStatusCode.PreconditionFailed, null, Strings.Serializer_ETagValueDoesNotMatch, null, e);
            }
            catch (Exception e)
            {
                if (DbContextHelper.IsDbEntityValidationException(e))
                {
                    throw DbContextHelper.WrapDbEntityValidationException(e);
                }

                throw;
            }
        }

        /// <summary>
        /// Returns the actual instance of the resource represented by the given resource object
        /// </summary>
        /// <param name="resource">object representing the resource whose instance needs to be fetched</param>
        /// <returns>The actual instance of the resource represented by the given resource object</returns>
        public virtual object ResolveResource(object resource)
        {
            WebUtil.CheckArgumentNull(resource, "resource");
            object newEntity;

            // If the current resource is getting replaced via the PUT method,
            // all the changes are present in the newEntity. Hence we need to 
            // call apply changes on the context so that the context knows about
            // the changes before the change interceptor is fired.
            if (this.objectsToBeReplaced.TryGetValue(resource, out newEntity))
            {
                ObjectContextServiceProvider.ApplyChangesToEntity(this.ObjectContext, resource, newEntity);

                // remove this entity from the dictionary so that we do not apply the changes
                // again during save changes.
                this.objectsToBeReplaced.Remove(resource);
            }

            return resource;
        }

        /// <summary>
        /// Revert all the pending changes.
        /// </summary>
        public virtual void ClearChanges()
        {
            // Detach all the existing entries in the object context
            foreach (ObjectStateEntry entry in this.ObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified | EntityState.Unchanged))
            {
                // entry.State != Entry.Detached: Also, if they are stub entries (no entity object
                // associated with the entry yet - these get automatically populated when we query the related entity),
                // they also get automatically detached once we detach the associated entity).
                // Both Entity and IsRelationship property throws if the entry is detached.
                // !entry.IsRelationship: We just need to remove the key entries.
                // While detaching the key entries, the relationship entries associated 
                // with the key entries also get detached. 
                // entry.Entity != null:  Since they are no ordering gaurantees, the stub key entries 
                // can come first, we need to skip these.
                if (entry.State != EntityState.Detached && !entry.IsRelationship && entry.Entity != null)
                {
                    this.ObjectContext.Detach(entry.Entity);
                }
            }

            // clear the list of objects that need to be special handled during save changes for replace semantics
            this.objectsToBeReplaced.Clear();
            this.actionsToInvoke.Clear();
        }

        #endregion IUpdatable Members

        #region IDataServiceEntityFrameworkProvider

        /// <summary>
        /// Get the list of etag property names given the entity set name and the instance of the resource
        /// </summary>
        /// <param name="containerName">name of the entity set</param>
        /// <param name="resourceType">Type of the resource whose etag properties need to be fetched</param>
        /// <returns>list of etag property names</returns>
        public virtual IList<ResourceProperty> GetETagProperties(string containerName, ResourceType resourceType)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(containerName, "containerName");
            WebUtil.CheckArgumentNull(resourceType, "resourceType");

            EntitySetBase entitySet = this.GetEntitySet(containerName);
            EntityType entityType = this.ObjectContext.MetadataWorkspace.GetItem<EntityType>(resourceType.FullName, DataSpace.CSpace);
            Debug.Assert(entityType != null, "entityType != null");
            List<ResourceProperty> etagProperties = new List<ResourceProperty>();

            // Workspace associated directly with the ObjectContext has metadata only about OSpace, CSpace and OCSpace.
            // Since GetRequiredOriginalValueMembers depends on mapping information (CSSpace metadata),
            // we need to make sure we call this API on a workspace which has information about the CS Mapping.
            // Hence getting workspace from the underlying Entity connection.
            MetadataWorkspace workspace = ((EntityConnection)this.ObjectContext.Connection).GetMetadataWorkspace();
#pragma warning disable 618
            foreach (EdmMember member in workspace.GetRequiredOriginalValueMembers(entitySet, entityType))
#pragma warning restore 618
            {
                ResourceProperty property = resourceType.TryResolvePropertyName(member.Name, exceptKind: ResourcePropertyKind.Stream);
                Debug.Assert(property != null, "property != null");
                Debug.Assert(property.ResourceType.ResourceTypeKind == ResourceTypeKind.Primitive, "property.ResourceType.TypeKind == ResourceTypeKind.Primitive");

                // Ignore key properties if they are part of etag, since the uri already has the key information
                // and it makes no sense to duplicate them in etag
                if (!property.IsOfKind(ResourcePropertyKind.Key))
                {
                    etagProperties.Add(property);
                }
            }

            return etagProperties;
        }

        #endregion IDataServiceEntityFrameworkProvider

        /// <summary>
        /// Return the list of custom annotation for the entity container with the given name.
        /// </summary>
        /// <param name="entityContainerName">Name of the EntityContainer.</param>
        /// <returns>Return the list of custom annotation for the entity container with the given name.</returns>
        public override IEnumerable<KeyValuePair<string, object>> GetEntityContainerAnnotations(string entityContainerName)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(entityContainerName, "entityContainerName");

            EntityContainer container = this.ObjectContext.MetadataWorkspace.GetItem<EntityContainer>(entityContainerName, DataSpace.CSpace);
            Debug.Assert(container != null, "container != null");

            Dictionary<string, object> customAnnotations = null;
#if !EF6Provider
            ObjectContextServiceProvider.PopulateAnnotations(
                container.MetadataProperties,
                (namespaceName, name, annotation) =>
                {
                    Debug.Assert(!String.IsNullOrEmpty(namespaceName), "!String.IsNullOrEmpty(namespaceName)");
                    Debug.Assert(!String.IsNullOrEmpty(name), "!String.IsNullOrEmpty(name)");
                    Debug.Assert(annotation != null, "annotation != null");

                    if (customAnnotations == null)
                    {
                        customAnnotations = new Dictionary<string, object>(StringComparer.Ordinal);
                    }

                    // custom annotations can be only of string or XElement type
                    Debug.Assert(annotation.GetType() == typeof(string) || annotation.GetType() == typeof(XElement), "only string and xelement annotations are supported");
                    customAnnotations.Add(namespaceName + ":" + name, annotation);
                });
#endif
            return customAnnotations ?? WebUtil.EmptyKeyValuePairStringObject;
        }

        #region IConcurrencyProvider Methods

        /// <summary>
        /// Set the etag values for the given resource.
        /// </summary>
        /// <param name="resource">resource for which etag values need to be set.</param>
        /// <param name="checkForEquality">true if we need to compare the property values for equality. If false, then we need to compare values for non-equality.</param>
        /// <param name="concurrencyValues">list of the etag property names, along with their values.</param>
        public virtual void SetConcurrencyValues(object resource, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
        {
            WebUtil.CheckArgumentNull(resource, "resource");
            WebUtil.CheckArgumentNull(concurrencyValues, "concurrencyValues");

            // Now this method will need to check for cases when etag are specified
            if (checkForEquality == null)
            {
                ResourceType resourceType = this.GetResourceType(resource);
                throw ObjectContextServiceProvider.CreateBadRequestError(Strings.DataService_CannotPerformOperationWithoutETag(resourceType == null ? resource.GetType().FullName : resourceType.FullName));
            }

            if (!checkForEquality.Value)
            {
                throw ObjectContextServiceProvider.CreateBadRequestError(Strings.ObjectContext_IfNoneMatchHeaderNotSupportedInUpdateAndDelete);
            }

            ObjectStateEntry objectStateEntry = this.ObjectContext.ObjectStateManager.GetObjectStateEntry(resource);
            Debug.Assert(objectStateEntry != null, "ObjectStateEntry must be found");

            OriginalValueRecord originalValues = objectStateEntry.GetUpdatableOriginalValues();
            foreach (KeyValuePair<string, object> etag in concurrencyValues)
            {
                int propertyOrdinal = originalValues.GetOrdinal(etag.Key);
                originalValues.SetValue(propertyOrdinal, etag.Value);
            }
        }

        #endregion IConcurrencyProvider Methods

        #region IDataServiceUpdateProvider2 Methods

        /// <summary>
        /// Queues up the <paramref name="invokable"/> to be invoked during IUpdatable.SaveChanges().
        /// </summary>
        /// <param name="invokable">The invokable instance whose Invoke() method will be called during IUpdatable.SaveChanges().</param>
        [SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "Invokable", Justification = "Invokable is the correct spelling")]
        [SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "invokable", Justification = "Invokable is the correct spelling")]
        public virtual void ScheduleInvokable(IDataServiceInvokable invokable)
        {
            WebUtil.CheckArgumentNull(invokable, "invokable");
            this.actionsToInvoke.Add(invokable);
        }

        #endregion IDataServiceUpdateProvider2 Methods

        /// <summary>
        /// Populates the member metadata for the given type
        /// </summary>
        /// <param name="resourceTypeCacheItem">Instance of ResourceTypeCacheItem containing the ResourceType and its metadata.</param>
        /// <param name="workspace">workspace containing the metadata information</param>
        /// <param name="metadataCacheItem">Instance of ProviderMetadataCacheItem.</param>
        /// <param name="primitiveResourceTypeMap">Map of primitive types to use when building member metadata.</param>
        internal static void PopulateMemberMetadata(
            ResourceTypeCacheItem resourceTypeCacheItem,
            IProviderMetadata workspace,
            ProviderMetadataCacheItem metadataCacheItem,
            PrimitiveResourceTypeMap primitiveResourceTypeMap)
        {
            Debug.Assert(resourceTypeCacheItem != null, "resourceTypeCacheItem != null");
            Debug.Assert(workspace != null, "workspace != null");

            var resourceType = resourceTypeCacheItem.ResourceType;

            // Find the type from the OSpace
            IProviderType edmType = workspace.GetProviderType(resourceType.FullName);
            foreach (IProviderMember member in edmType.Members)
            {
                ResourcePropertyKind kind = (ResourcePropertyKind)(-1);

                // ObjectContextServiceProvider fails with NullReferenceException when an entity property is not public.
                // If the property on the CLR type which is representing the EDM type has non-public properties but those properties are part of the
                // conceptual model, the server will try to load CLR metadata for these properties.
                // The Type.GetProperty(propertyName) method used BindingFlags.Instance | BindingFlags.Public by default if no binding flags are specified.
                // Since the property was not found with these binding flags, the GetProperty method returns null, which we didn't check for in v1 and v2 and threw an NRE.
                // We now check for null return values from this function and throw if we find that the model property declared on the CLR type is not public.
                PropertyInfo propertyInfo = resourceType.InstanceType.GetProperty(member.Name, BindingFlags.Instance | BindingFlags.Public);
                if (propertyInfo == null)
                {
                    throw new DataServiceException((int)HttpStatusCode.InternalServerError, Strings.ObjectContext_PublicPropertyNotDefinedOnType(edmType.Name, member.Name));
                }

                ResourceType propertyType = null;
                switch (member.EdmTypeKind)
                {
                    case BuiltInTypeKind.PrimitiveType:

                        Type propertyClrType = propertyInfo.PropertyType;
                        propertyType = primitiveResourceTypeMap.GetPrimitive(propertyClrType);

                        if (propertyType == null)
                        {
                            throw new NotSupportedException(Strings.ObjectContext_PrimitiveTypeNotSupported(member.Name, edmType.Name, member.EdmTypeName));
                        }

                        if (member.IsKey)
                        {
                            kind = ResourcePropertyKind.Key | ResourcePropertyKind.Primitive;
                        }
                        else
                        {
                            kind = ResourcePropertyKind.Primitive;
                        }

                        break;
                    case BuiltInTypeKind.ComplexType:
                        kind = ResourcePropertyKind.ComplexType;
                        propertyType = metadataCacheItem.TryGetResourceType(propertyInfo.PropertyType);
                        break;
                    case BuiltInTypeKind.EntityType:
                        kind = ResourcePropertyKind.ResourceReference;
                        propertyType = metadataCacheItem.TryGetResourceType(propertyInfo.PropertyType);
                        break;
                    case BuiltInTypeKind.CollectionType:
                        kind = ResourcePropertyKind.ResourceSetReference;
                        Type collectionItemClrType = workspace.GetClrType(member.CollectionItemType);
                        Debug.Assert(!WebUtil.IsPrimitiveType(collectionItemClrType), "We don't support collections of primitives, we shouldn't see one here");
                        propertyType = metadataCacheItem.TryGetResourceType(collectionItemClrType);
                        break;
                    default:
                        throw new NotSupportedException(Strings.ObjectContext_PrimitiveTypeNotSupported(member.Name, edmType.Name, member.EdmTypeName));
                }

                Debug.Assert(propertyType != null, "propertyType != null");
                ResourceProperty resourceProperty = new ResourceProperty(propertyInfo.Name, kind, propertyType);
                SetMimeTypeForMappedMember(resourceProperty, member);
                resourceType.AddProperty(resourceProperty);
#if !EF6Provider
                ObjectContextServiceProvider.PopulateFacets(resourceProperty, member.Facets, resourceProperty.ResourceType.ResourceTypeKind == ResourceTypeKind.EntityType /*ignoreNullableAnnotation*/);
                ObjectContextServiceProvider.PopulateAnnotations(member.MetadataProperties, resourceProperty.AddCustomAnnotation);
#endif
                resourceTypeCacheItem.AddResourcePropertyCacheItem(resourceProperty, new ObjectContextResourcePropertyCacheItem(propertyInfo, member));
            }
        }

        /// <summary>Gets the CLR type mapped to the specified C-Space type.</summary>
        /// <param name="workspace">Workspace in which the type is defined.</param>
        /// <param name="edmType">C-Space type whose matching clr type needs to be looked up.</param>
        /// <returns>The resolved <see cref="Type"/> for the given <paramref name="edmType"/>.</returns>
        internal static Type GetClrTypeForCSpaceType(MetadataWorkspace workspace, StructuralType edmType)
        {
            Debug.Assert(workspace != null, "workspace != null");
            Debug.Assert(edmType != null, "edmType != null");
            Debug.Assert(
               edmType.BuiltInTypeKind == BuiltInTypeKind.EntityType || edmType.BuiltInTypeKind == BuiltInTypeKind.ComplexType,
               "Must be entityType or complexType");

            StructuralType ospaceType;
            if (workspace.TryGetObjectSpaceType(edmType, out ospaceType))
            {
                ObjectItemCollection objectItemCollection = (ObjectItemCollection)workspace.GetItemCollection(DataSpace.OSpace);
                return objectItemCollection.GetClrType(ospaceType);
            }

            return null;
        }

        /// <summary>Checks that the metadata model is consistent.</summary>
        protected sealed override void CheckModelConsistency()
        {
            CheckNavigationPropertiesBound(this.CurrentDataSource);
        }

        /// <summary>
        /// Populates metadata from the given object context
        /// </summary>
        /// <param name="metadataCacheItem">Instance of ProviderMetadataCacheItem in which metadata needs to be populated.</param>
        protected sealed override void PopulateMetadata(ProviderMetadataCacheItem metadataCacheItem)
        {
            Debug.Assert(metadataCacheItem != null, "metadataCacheItem != null");
            Debug.Assert(this.ObjectContext != null, "this.ObjectContext != null");

            InitializeObjectItemCollection(this.ObjectContext, this.DataSourceType.Assembly);
            MetadataWorkspace metadataWorkspace = this.ObjectContext.MetadataWorkspace;

            // Create Resource types for all the top level entity types and complexTypes
            foreach (StructuralType edmType in metadataWorkspace.GetItems<StructuralType>(DataSpace.CSpace))
            {
                if (edmType.BuiltInTypeKind == BuiltInTypeKind.EntityType ||
                    edmType.BuiltInTypeKind == BuiltInTypeKind.ComplexType)
                {
                    // Populates metadata for the given types and all its base types
                    if (PopulateTypeMetadata(metadataWorkspace, edmType, metadataCacheItem) == null)
                    {
                        this.typesWithoutOSpaceMetadata.Add(edmType);
                    }
                }
            }

            foreach (EntityContainer entityContainer in metadataWorkspace.GetItems<EntityContainer>(DataSpace.CSpace))
            {
                bool defaultEntityContainer = entityContainer.Name == this.ObjectContext.DefaultContainerName;

                // Get the list of entity sets (Ignore the relationship sets, since we won't allow that to be queried directly
                foreach (EntitySetBase entitySetBase in entityContainer.BaseEntitySets)
                {
                    // Ignore all the association sets for the type being, since we are caching only entity sets
                    if (entitySetBase.BuiltInTypeKind != BuiltInTypeKind.EntitySet)
                    {
                        continue;
                    }

                    EntitySet entitySet = (EntitySet)entitySetBase;
                    Type elementType = GetClrTypeForCSpaceType(metadataWorkspace, entitySet.ElementType);
                    ResourceType resourceType;
                    if (elementType == null || (resourceType = metadataCacheItem.TryGetResourceType(elementType)) == null)
                    {
                        throw new InvalidOperationException(Strings.ObjectContextServiceProvider_OSpaceTypeNotFound(entitySet.ElementType.FullName));
                    }

                    string entitySetName = GetEntitySetName(entitySet.Name, entitySet.EntityContainer.Name, defaultEntityContainer);
                    ResourceSet resourceSet = new ResourceSet(entitySetName, resourceType);
                    metadataCacheItem.EntitySets.Add(entitySetName, resourceSet);
#if !EF6Provider
                    resourceSet.EntityContainerName = entityContainer.Name;
                    ObjectContextServiceProvider.PopulateAnnotations(entitySetBase.MetadataProperties, resourceSet.AddCustomAnnotation);
#endif
                    metadataCacheItem.QueryRootCache.Add(resourceSet, this.BuildQueryRootDelegate(resourceSet));
                }
            }

            // Now go and populate the member information for each resource type
            foreach (var resourceTypeCacheItem in metadataCacheItem.ResourceTypeCacheItems)
            {
                var resourceType = resourceTypeCacheItem.ResourceType;

                if (resourceType.ResourceTypeKind == ResourceTypeKind.Primitive)
                {
                    continue;
                }

                PopulateMemberMetadata(resourceTypeCacheItem, new ObjectContextMetadata(metadataWorkspace), metadataCacheItem, PrimitiveResourceTypeMap.TypeMap);
            }
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
            foreach (Type type in userSpecifiedTypes)
            {
                if (this.PopulateMetadataForType(type, metadataCacheItem) == null)
                {
                    throw new InvalidOperationException(Strings.BadProvider_InvalidTypeSpecified(type.FullName));
                }
            }

            // If there is a type in the model, for which we couldn't load the metadata, we should throw.
            if (this.typesWithoutOSpaceMetadata.Count != 0)
            {
                throw new InvalidOperationException(Strings.ObjectContext_UnableToLoadMetadataForType(this.typesWithoutOSpaceMetadata[0].FullName));
            }

            this.typesWithoutOSpaceMetadata = null;
        }

        /// <summary>
        /// Populate metadata for the given clr type.
        /// </summary>
        /// <param name="type">type whose metadata needs to be loaded.</param>
        /// <param name="metadataCacheItem">Instance of ProviderMetadataCacheItem.</param>
        /// <returns>resource type containing metadata for the given clr type.</returns>
        protected sealed override ResourceType PopulateMetadataForType(Type type, ProviderMetadataCacheItem metadataCacheItem)
        {
            Debug.Assert(!WebUtil.IsPrimitiveType(type), "Why are we trying to load metadata for a primitive type?");

            ResourceType resourceType = metadataCacheItem.TryGetResourceType(type);
            if (resourceType == null)
            {
                InitializeObjectItemCollection(this.ObjectContext, type.Assembly);
                ObjectItemCollection objectItemCollection = (ObjectItemCollection)this.ObjectContext.MetadataWorkspace.GetItemCollection(DataSpace.OSpace);
                StructuralType ospaceType, cspaceType;
                if (objectItemCollection.TryGetItem<StructuralType>(type.FullName, out ospaceType))
                {
                    if (this.ObjectContext.MetadataWorkspace.TryGetEdmSpaceType(ospaceType, out cspaceType))
                    {
                        ResourceType baseType = null;
                        if (cspaceType.BaseType != null)
                        {
                            baseType = this.PopulateMetadataForType(type.BaseType, metadataCacheItem);
                        }

                        resourceType = CreateResourceType(cspaceType, type, baseType, metadataCacheItem);
                        this.typesWithoutOSpaceMetadata.Remove(cspaceType);
                    }
                }
            }

            return resourceType;
        }

        /// <summary>
        /// Returns the resource type for the corresponding clr type.
        /// </summary>
        /// <param name="type">clrType whose corresponding resource type needs to be returned</param>
        /// <returns>Returns the resource type</returns>
        protected sealed override ResourceTypeCacheItem ResolveNonPrimitiveTypeCacheItem(System.Type type)
        {
            Type actualType = ObjectContext.GetObjectType(type);
            return base.ResolveNonPrimitiveTypeCacheItem(actualType);
        }

        /// <summary>
        /// Checks that all navigation properties are bound to some association set for every entity set.
        /// </summary>
        /// <param name='dataSourceInstance'>Instance of the data source for the provider.</param>
        private static void CheckNavigationPropertiesBound(object dataSourceInstance)
        {
            // For every navigation property, ensure that all of the EntitySets that can
            // take their EntityType have an AssociationSet of the appropriate Association type.
            Debug.Assert(dataSourceInstance != null, "dataSourceInstance != null");

            ObjectContext context = DbContextHelper.GetObjectContext(dataSourceInstance);

            MetadataWorkspace workspace = context.MetadataWorkspace;
            foreach (EntityType type in workspace.GetItems<EntityType>(DataSpace.CSpace))
            {
                foreach (NavigationProperty navigationProperty in type.NavigationProperties)
                {
                    foreach (EntitySet entitySet in GetEntitySetsForType(workspace, type))
                    {
                        IEnumerable<EntitySet> entitySetsWithAssocation = GetEntitySetsWithAssociationSets(
                            workspace,
                            navigationProperty.RelationshipType,
                            navigationProperty.FromEndMember);
                        if (!entitySetsWithAssocation.Contains(entitySet))
                        {
                            throw new InvalidOperationException(Strings.ObjectContext_NavigationPropertyUnbound(
                                navigationProperty.Name,
                                type.FullName,
                                entitySet.Name));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets all <see cref="EntitySet"/> instance that may hold an entity of type <paramref name="type"/>.
        /// </summary>
        /// <param name="workspace">Workspace with metadata.</param>
        /// <param name="type">Entity type to get entity sets for.</param>
        /// <returns>An enumeration of <see cref="EntitySet"/> instances that can hold <paramref name="type"/>.</returns>
        private static IEnumerable<EntitySet> GetEntitySetsForType(MetadataWorkspace workspace, EntityType type)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(workspace != null, "workspace != null");
            foreach (EntityContainer container in workspace.GetItems<EntityContainer>(DataSpace.CSpace))
            {
                foreach (EntitySet entitySet in container.BaseEntitySets.OfType<EntitySet>())
                {
                    if (IsAssignableFrom(entitySet.ElementType, type))
                    {
                        yield return entitySet;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all entity sets that participate as members for the specified <paramref name="associationType"/>.
        /// </summary>
        /// <param name="workspace">Workspace with metadata.</param>
        /// <param name="associationType">Type of assocation to check.</param>
        /// <param name="member">Member of association to check.</param>
        /// <returns>
        /// All <see cref="EntitySet"/> instances that are are on the <paramref name="member"/> role for 
        /// some association of <paramref name="associationType"/>.
        /// </returns>
        private static IEnumerable<EntitySet> GetEntitySetsWithAssociationSets(
            MetadataWorkspace workspace,
            RelationshipType associationType,
            RelationshipEndMember member)
        {
            Debug.Assert(workspace != null, "workspace != null");
            Debug.Assert(associationType != null, "associationType != null");
            Debug.Assert(member != null, "member != null");
            foreach (EntityContainer container in workspace.GetItems<EntityContainer>(DataSpace.CSpace))
            {
                foreach (AssociationSet associationSet in container.BaseEntitySets.OfType<AssociationSet>())
                {
                    if (associationSet.ElementType == associationType)
                    {
                        foreach (AssociationSetEnd end in associationSet.AssociationSetEnds)
                        {
                            if (end.CorrespondingAssociationEndMember == member)
                            {
                                yield return end.EntitySet;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Reads the HasStream attribute from the specified <paramref name="type"/>.</summary>
        /// <param name="type">Type to read attribute from.</param>
        /// <returns>True if HasStream="true" stream property is defined for the entity type.</returns>
        private static bool GetEntityTypeDefaultStreamProperty(StructuralType type)
        {
            Debug.Assert(type != null, "type != null");

            const string HasStreamAttribute = XmlConstants.DataWebMetadataNamespace + ":" + XmlConstants.DataWebAccessHasStreamAttribute;
            bool result = false;
            MetadataProperty property;
            if (type.MetadataProperties.TryGetValue(HasStreamAttribute, false /* ignoreCase */, out property))
            {
                string text = (string)property.Value;
                if (String.IsNullOrEmpty(text))
                {
                    throw new InvalidOperationException(Strings.ObjectContext_HasStreamAttributeEmpty(type.Name));
                }

                if (text != XmlConstants.DataWebAccessDefaultStreamPropertyValue)
                {
                    // In the future we might support multiple stream properties. For now we only support $default.
                    throw new NotSupportedException(Strings.ObjectContext_UnsupportedStreamProperty(text, type.Name));
                }

                result = true;
            }

            return result;
        }

        /// <summary>Sets the MIME type, if specified for the specified member.</summary>
        /// <param name="resourceProperty">resource property whose mime type needs to be updated.</param>
        /// <param name="csdlMember">C-Space member for which we need to find the C-Space mime type attribute.</param>
        private static void SetMimeTypeForMappedMember(ResourceProperty resourceProperty, IProviderMember csdlMember)
        {
            string mimeType = csdlMember.MimeType;
            if (mimeType != null)
            {
                resourceProperty.MimeType = mimeType;
            }
        }

        /// <summary>Checks whether <paramref name="derivedType"/> may be assigned to <paramref name="baseType"/>.</summary>
        /// <param name="baseType">Type to check assignment to.</param>
        /// <param name="derivedType">Type to check assignment from.</param>
        /// <returns>
        /// true if an instance of <paramref name="derivedType" /> can be assigned to a variable of 
        /// <paramref name="baseType"/>; false otherwise.
        /// </returns>
        private static bool IsAssignableFrom(EntityType baseType, EntityType derivedType)
        {
            while (derivedType != null)
            {
                if (derivedType == baseType)
                {
                    return true;
                }

                derivedType = (EntityType)derivedType.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Populates the metadata for the given type and its base type
        /// </summary>
        /// <param name="workspace">metadata workspace containing all the metadata information</param>
        /// <param name="edmType"> type whose metadata needs to be populated </param>
        /// <param name="metadataCacheItem">Instance of ProviderMetadataCacheItem.</param>
        /// <returns>returns the resource type corresponding to the given edmType</returns>
        private static ResourceType PopulateTypeMetadata(
            MetadataWorkspace workspace,
            StructuralType edmType,
            ProviderMetadataCacheItem metadataCacheItem)
        {
            Debug.Assert(
                edmType.BuiltInTypeKind == BuiltInTypeKind.EntityType ||
                edmType.BuiltInTypeKind == BuiltInTypeKind.ComplexType,
                "type must be entity or complex type");

            ResourceType resourceType = null;
            Type clrType = GetClrTypeForCSpaceType(workspace, edmType);
            if (clrType != null)
            {
                resourceType = metadataCacheItem.TryGetResourceType(clrType);
                if (resourceType == null)
                {
                    ResourceType baseType = null;
                    if (edmType.BaseType != null)
                    {
                        baseType = PopulateTypeMetadata(workspace, (StructuralType)edmType.BaseType, metadataCacheItem);
                    }

                    resourceType = CreateResourceType(edmType, clrType, baseType, metadataCacheItem);
                }
            }

            return resourceType;
        }

        /// <summary>
        /// Creates a new instance of resource type given the cspace structural type and mapping clr type.
        /// </summary>
        /// <param name="cspaceType">cspace structural type.</param>
        /// <param name="clrType">mapping clr type for the given structural type.</param>
        /// <param name="baseResourceType">the base resource type for the given resource type.</param>
        /// <param name="metadataCacheItem">Instance of ProviderMetadataCacheItem.</param>
        /// <returns>the new resource type instance created for the given cspace type.</returns>
        private static ResourceType CreateResourceType(
            StructuralType cspaceType,
            Type clrType,
            ResourceType baseResourceType,
            ProviderMetadataCacheItem metadataCacheItem)
        {
            ResourceTypeKind resourceTypeKind = cspaceType.BuiltInTypeKind == BuiltInTypeKind.EntityType ? ResourceTypeKind.EntityType : ResourceTypeKind.ComplexType;

            // We do not support open types in Object Context provider yet.
            ResourceType resourceType = new ResourceType(clrType, resourceTypeKind, baseResourceType, cspaceType.NamespaceName, cspaceType.Name, clrType.IsAbstract);
            if (GetEntityTypeDefaultStreamProperty(cspaceType))
            {
                resourceType.IsMediaLinkEntry = true;
            }

            // Add stream properties that are marked NamedStream in clrType to resourceType.
            AddStreamProperties(resourceType, clrType, baseResourceType == null);

            metadataCacheItem.AddResourceType(clrType, resourceType);
            var childTypes = metadataCacheItem.ChildTypesCache;
            childTypes.Add(resourceType, null);
            if (baseResourceType != null)
            {
                Debug.Assert(childTypes.ContainsKey(baseResourceType), "childTypes.ContainsKey(baseResourceType)");
                if (childTypes[baseResourceType] == null)
                {
                    childTypes[baseResourceType] = new List<ResourceType>();
                }

                childTypes[baseResourceType].Add(resourceType);
            }

#if !EF6Provider
            ObjectContextServiceProvider.PopulateAnnotations(cspaceType.MetadataProperties, resourceType.AddCustomAnnotation);
#endif
            return resourceType;
        }

        /// <summary>
        /// Returns the entity set name for the given entity set. If this entity set belongs to the default container name,
        /// then it returns the entity set name, otherwise qualifies it with the entitycontainer name
        /// </summary>
        /// <param name="entitySetName">entity set name</param>
        /// <param name="entityContainerName">entity container name</param>
        /// <param name="containedInDefaultEntityContainer">true if the given entity set belongs to the default entity container</param>
        /// <returns>returns the entity set name</returns>
        private static string GetEntitySetName(string entitySetName, string entityContainerName, bool containedInDefaultEntityContainer)
        {
            if (containedInDefaultEntityContainer)
            {
                return entitySetName;
            }
            else
            {
                return entityContainerName + "." + entitySetName;
            }
        }

        /// <summary>
        /// Returns the escaped entity set name for the given entity set. If this entity set belongs to the default container name,
        /// then it returns the escaped entity set name, otherwise it escapes both the container and set name
        /// </summary>
        /// <param name="qualifiedEntitySetName">qualified entity set name whose name needs to be escaped</param>
        /// <returns>returns the escaped entityset name</returns>
        private static string GetEscapedEntitySetName(string qualifiedEntitySetName)
        {
            int indexOfLastPeriod = qualifiedEntitySetName.LastIndexOf('.');

            if (-1 == indexOfLastPeriod)
            {
                return "[" + qualifiedEntitySetName + "]";
            }
            else
            {
                return "[" + qualifiedEntitySetName.Substring(0, indexOfLastPeriod) + "].[" + qualifiedEntitySetName.Substring(indexOfLastPeriod + 1) + "]";
            }
        }

        /// <summary>Initializes metadata for the given object context.</summary>
        /// <param name="objectContext">Instance of data source to use if pure static analysis isn't possible.</param>
        /// <param name="assembly">assembly whose metadata needs to be loaded.</param>
        private static void InitializeObjectItemCollection(ObjectContext objectContext, Assembly assembly)
        {
            objectContext.MetadataWorkspace.LoadFromAssembly(assembly);
        }

        /// <summary>
        /// Create a new instance of the given clrtype using ObjectContext.CreateObject method
        /// </summary>
        /// <param name="context">current object context instance.</param>
        /// <param name="clrType">clrType whose instance needs to be created.</param>
        /// <returns>the instance returned by ObjectContext.</returns>
        private static object CreateObject(ObjectContext context, Type clrType)
        {
            // this.ObjectContext.CreateObject<T>()
            MethodInfo createObjectMethod = context.GetType().GetMethod("CreateObject", BindingFlags.Public | BindingFlags.Instance);
            return createObjectMethod.MakeGenericMethod(clrType).Invoke(context, null);
        }

        /// <summary>
        /// Checks if the given association is a FK association with cardinality 1 to 1 or 0..1 to 1
        /// </summary>
        /// <param name="association">metadata for the association.</param>
        /// <returns>Returns true if the given association is a FK association with cardinality 1 to 1 or 0..1 to 1.</returns>
        private static bool IsOneToOneFKAssocation(AssociationType association)
        {
            // first check if the relationship type is a FK relationship. If no, then return , since updates are not supported only for FK relationships
            if (!association.IsForeignKey)
            {
                return false;
            }

            // check if the relationship is 1 to 1 or 1 to 0..1 relationship (FK relationships are not supported for 0..1 to 0..1 cardinality)
            // if its neither, we do support updates and there is no need to throw.
            if (association.RelationshipEndMembers[0].RelationshipMultiplicity == RelationshipMultiplicity.Many ||
                association.RelationshipEndMembers[1].RelationshipMultiplicity == RelationshipMultiplicity.Many)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the entity set name for the given object state entry.
        /// </summary>
        /// <param name="entry">object state entry for the object whose entity set name needs to be retreived.</param>
        /// <param name="defaultContainerName">default container name.</param>
        /// <returns>entity set name for the given entity entry.</returns>
        private static string GetEntitySetName(ObjectStateEntry entry, string defaultContainerName)
        {
            return ObjectContextServiceProvider.GetEntitySetName(
                entry.EntitySet.Name,
                entry.EntitySet.EntityContainer.Name,
                entry.EntitySet.EntityContainer.Name == defaultContainerName);
        }

        /// <summary>
        /// Apply changes from the newEntity to the original entity
        /// </summary>
        /// <param name="objectContext">ObjectContext instance.</param>
        /// <param name="originalTrackedEntity">original entity which is tracked by the context.</param>
        /// <param name="newEntity">newEntity which contains all the changed values.</param>
        private static void ApplyChangesToEntity(ObjectContext objectContext, object originalTrackedEntity, object newEntity)
        {
            ObjectStateEntry objectStateEntry = objectContext.ObjectStateManager.GetObjectStateEntry(originalTrackedEntity);
            string entitySetName = ObjectContextServiceProvider.GetEntitySetName(objectStateEntry, objectContext.DefaultContainerName);
            if (objectStateEntry.State == EntityState.Added)
            {
                // In the case of batching if we do a PUT after POST in the same changeset, we need to detach and re-add.
                objectContext.Detach(originalTrackedEntity);
                objectContext.AddObject(entitySetName, newEntity);
            }
            else
            {
#pragma warning disable 618
                // Since we use to do this in V1, keeping the same code there.
                // Apply property changes as specified in the new object.
                objectContext.ApplyPropertyChanges(entitySetName, newEntity);
#pragma warning restore 618
            }
        }

#if !EF6Provider
        /// <summary>
        /// Populate annotations from the list of the metadata properties.
        /// </summary>
        /// <param name="metadataProperties">list of metadata properties.</param>
        /// <param name="addAnnotationMethod">method for adding the annotation.</param>
        private static void PopulateAnnotations(IEnumerable<MetadataProperty> metadataProperties, Action<string, string, object> addAnnotationMethod)
        {
            foreach (MetadataProperty metadataProperty in metadataProperties)
            {
                if (metadataProperty.PropertyKind != PropertyKind.System)
                {
                    int lastColonIndex = metadataProperty.Name.LastIndexOf(':');
                    Debug.Assert(lastColonIndex != -1, "In EF, every extended metadata property should live in some custom namespace");
                    string annotationName = metadataProperty.Name.Substring(lastColonIndex + 1);
                    string annotationNamespace = metadataProperty.Name.Substring(0, lastColonIndex);

                    // Drop all the annotations in the astoria namespace, since we should have read it and applied it to the metadata.
                    if (annotationNamespace != XmlConstants.DataWebMetadataNamespace)
                    {
                        addAnnotationMethod(annotationNamespace, annotationName, metadataProperty.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Populate the annotations from the list of facets.
        /// </summary>
        /// <param name="property">ResourceProperty instance whose annotation needs to be populated.</param>
        /// <param name="facets">List of facets.</param>
        /// <param name="ignoreNullableFacet">True if the nullable annotation needs to be ignored.</param>
        private static void PopulateFacets(ResourceProperty property, IEnumerable<Facet> facets, bool ignoreNullableFacet)
        {
            foreach (Facet facet in facets)
            {
                if (facet.Name == XmlConstants.CsdlNullableAttributeName)
                {
                    // In EF, the metadata API's return the nullable annotation with false value for navigation property.
                    // But writing nullable attribute in the csdl for navigation properties is not allowed.
                    if (!ignoreNullableFacet && (bool)facet.Value != true)
                    {
                        // For the nullable facet, the default value is true.
                        // Hence not writing the nullable facet when the value is true.
                        property.AddCustomAnnotation(String.Empty, facet.Name, facet.Value);
                    }
                }
                else if (facet.Value != null)
                {
                    property.AddCustomAnnotation(String.Empty, facet.Name, facet.Value);
                }
            }
        }

        /// <summary>
        /// Return the string for the given enum RelationshipMultiplicity value.
        /// </summary>
        /// <param name="multiplicity">RelationshipMultiplicity value.</param>
        /// <returns>Return the string for the given enum RelationshipMultiplicity value.</returns>
        private static string GetMultiplicity(RelationshipMultiplicity multiplicity)
        {
            switch (multiplicity)
            {
                case RelationshipMultiplicity.Many:
                    return XmlConstants.Many;

                case RelationshipMultiplicity.One:
                    return XmlConstants.One;

                default:
                    Debug.Assert(multiplicity == RelationshipMultiplicity.ZeroOrOne, "multiplicity == RelationshipMultiplicity.ZeroOrOne");
                    return XmlConstants.ZeroOrOne;
            }
        }
#endif
        /// <summary>
        /// Populate ResourceAssociationSetEnd from the AssociationSetEnd instance.
        /// </summary>
        /// <param name="setEnd">Instance of AssociationSetEnd.</param>
        /// <param name="resourceSet">ResourceSet to which the type referred by the end belongs to.</param>
        /// <param name="resourceType">ResourceType referred by the end.</param>
        /// <param name="resourceProperty">ResourceProperty that takes part in the association.</param>
        /// <returns>An instance of ResourceAssociationSetEnd.</returns>
        private static ResourceAssociationSetEnd PopulateResourceAssociationSetEnd(AssociationSetEnd setEnd, ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            ResourceAssociationSetEnd resourceAssociationSetEnd = new ResourceAssociationSetEnd(resourceSet, resourceType, resourceProperty);
#if !EF6Provider
            resourceAssociationSetEnd.Name = setEnd.Name;

            // Populate Annotations
            ObjectContextServiceProvider.PopulateAnnotations(setEnd.MetadataProperties, resourceAssociationSetEnd.AddCustomAnnotation);
#endif
            return resourceAssociationSetEnd;
        }

#if !EF6Provider
        /// <summary>
        /// Populate the ResourceAssociationTypeEnd from the AssociationEndMember instance.
        /// </summary>
        /// <param name="end">Instance of AssociationEndMember.</param>
        /// <param name="resourceType">ResourceType instance which the end refers to.</param>
        /// <param name="resourceProperty">ResourceProperty instance.</param>
        /// <returns>An instance of ResourceAssociationTypeEnd.</returns>
        private static ResourceAssociationTypeEnd PopulateResourceAssociationTypeEnd(AssociationEndMember end, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            ResourceAssociationTypeEnd resourceAssociationTypeEnd = new ResourceAssociationTypeEnd(end.Name, resourceType, resourceProperty, ObjectContextServiceProvider.GetMultiplicity(end.RelationshipMultiplicity), (EdmOnDeleteAction)end.DeleteBehavior);
            ObjectContextServiceProvider.PopulateAnnotations(end.MetadataProperties, resourceAssociationTypeEnd.AddCustomAnnotation);
            return resourceAssociationTypeEnd;
        }

        /// <summary>
        /// Populate the ResourceAssociationType from the AssociationType instance.
        /// </summary>
        /// <param name="associationType">AssociationType instance.</param>
        /// <param name="end1">ResourceAssociationTypeEnd instance.</param>
        /// <param name="end2">ResourceAssociationTypeEnd instance.</param>
        /// <returns>An instance of ResourceAssociationType.</returns>
        private static ResourceAssociationType PopulateResourceAssociationType(AssociationType associationType, ResourceAssociationTypeEnd end1, ResourceAssociationTypeEnd end2)
        {
            ResourceAssociationType resourceAssociationType = new ResourceAssociationType(
                            associationType.Name,
                            associationType.NamespaceName,
                            end1,
                            end2);

            ObjectContextServiceProvider.PopulateAnnotations(associationType.MetadataProperties, resourceAssociationType.AddCustomAnnotation);

            if (associationType.ReferentialConstraints != null && associationType.ReferentialConstraints.Count != 0)
            {
                PopulateReferentialConstraint(resourceAssociationType, associationType.ReferentialConstraints[0]);
            }

            return resourceAssociationType;
        }

        /// <summary>
        /// Populate the ResourceReferentialConstraint instance from the ReferentialConstraint instance.
        /// </summary>
        /// <param name="resourceAssociationType">ResourceAssociationType instance.</param>
        /// <param name="referentialConstraint">ReferentialConstraint instance.</param>
        private static void PopulateReferentialConstraint(ResourceAssociationType resourceAssociationType, ReferentialConstraint referentialConstraint)
        {
            ResourceAssociationTypeEnd principalEnd = resourceAssociationType.GetEnd(referentialConstraint.FromRole.Name);
            ResourceAssociationTypeEnd dependentEnd = resourceAssociationType.GetEnd(referentialConstraint.ToRole.Name);

            List<ResourceProperty> dependentProperties = new List<ResourceProperty>();
            foreach (System.Data.Metadata.Edm.EdmProperty edmProperty in referentialConstraint.ToProperties)
            {
                dependentProperties.Add(dependentEnd.ResourceType.TryResolvePropertyName(edmProperty.Name));
            }

            resourceAssociationType.ReferentialConstraint = new ResourceReferentialConstraint(principalEnd, dependentProperties);
            ObjectContextServiceProvider.PopulateAnnotations(referentialConstraint.MetadataProperties, resourceAssociationType.ReferentialConstraint.AddCustomAnnotation);
        }

#endif
        /// <summary>
        /// Creates a new exception to indicate BadRequest error.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <param name="innerException">Inner Exception.</param>
        /// <returns>A new exception to indicate a bad request error.</returns>
        private static DataServiceException CreateBadRequestError(string message, Exception innerException = null)
        {
            // 400 - Bad Request
            return new DataServiceException(400, null, message, null, innerException);
        }

        /// <summary>
        /// Get the entity set metadata object given the qualified entity set name
        /// </summary>
        /// <param name="qualifiedEntitySetName">qualified entity set name i.e. if the entity set
        /// belongs to entity container other than the default one, then the entity container name should
        /// be part of the qualified name</param>
        /// <returns>the entity set metadata object</returns>
        private EntitySet GetEntitySet(string qualifiedEntitySetName)
        {
            Debug.Assert(
                !String.IsNullOrEmpty(qualifiedEntitySetName),
                "!String.IsNullOrEmpty(qualifiedEntitySetName) -- otherwise qualifiedEntitySetName didn't come from internal metadata");

            string entityContainerName;
            string entitySetName;

            // entity set name is fully qualified
            int index = qualifiedEntitySetName.LastIndexOf('.');
            if (index != -1)
            {
                entityContainerName = qualifiedEntitySetName.Substring(0, index);
                entitySetName = qualifiedEntitySetName.Substring(index + 1);
            }
            else
            {
                entityContainerName = this.ObjectContext.DefaultContainerName;
                entitySetName = qualifiedEntitySetName;
            }

            EntityContainer entityContainer = this.ObjectContext.MetadataWorkspace.GetEntityContainer(entityContainerName, DataSpace.CSpace);
            Debug.Assert(
                entityContainer != null,
                "entityContainer != null -- otherwise entityContainerName '" + entityContainerName + "' didn't come from metadata");

            EntitySet entitySet = entityContainer.GetEntitySetByName(entitySetName, false /*ignoreCase*/);
            Debug.Assert(
                entitySet != null,
                "entitySet != null -- otherwise entitySetName '" + entitySetName + "' didn't come from metadata");

            return entitySet;
        }

        /// <summary>
        /// Return the set of IL instructions for getting the IQueryable instance for the given ResourceSet.
        /// </summary>
        /// <param name="resourceSet">ResourceSet instance.</param>
        /// <returns>Func to invoke to get IQueryable for the given ResourceSet.</returns>
        private Func<object, IQueryable> BuildQueryRootDelegate(ResourceSet resourceSet)
        {
            string escapedEntitySetName = GetEscapedEntitySetName(resourceSet.Name);

            // ((ObjectContext)arg0).CreateQuery<resourceSet.ResourceType.InstanceType>("escapedEntitySetName", new ObjectParameter[0]);
            MethodInfo genericMethod = typeof(ObjectContext).GetMethod("CreateQuery", WebUtil.PublicInstanceBindingFlags).MakeGenericMethod(resourceSet.ResourceType.InstanceType);
            return context => (IQueryable)genericMethod.Invoke(context, new Object[] { escapedEntitySetName, new ObjectParameter[0] });
        }

        /// <summary>
        /// Update the relationship for the given entity.
        /// </summary>
        /// <param name="targetResource">source entity.</param>
        /// <param name="propertyName">navigation property which needs to get updated.</param>
        /// <param name="propertyValue">target entity - the other end of the relationship.</param>
        /// <param name="addRelationship">null for reference properties, true if relationship needs to be added for collection properties, else false.</param>
        private void UpdateRelationship(object targetResource, string propertyName, object propertyValue, bool? addRelationship)
        {
            WebUtil.CheckArgumentNull(targetResource, "targetResource");
            WebUtil.CheckStringArgumentNullOrEmpty(propertyName, "propertyName");

            ResourceType resourceType = this.GetResourceType(targetResource);
            if (resourceType == null)
            {
                throw new InvalidOperationException(Strings.ObjectContext_UnknownResourceTypeForClrType(targetResource.GetType().FullName));
            }

            // Get the resource type and resource property to find the type of the property
            ResourceProperty resourceProperty = resourceType.TryResolvePropertyName(propertyName, exceptKind: ResourcePropertyKind.Stream);
            if (resourceProperty == null)
            {
                throw new InvalidOperationException(Strings.ObjectContext_PropertyNotDefinedOnType(resourceType.FullName, propertyName));
            }

            if (resourceProperty.ResourceType.ResourceTypeKind != ResourceTypeKind.EntityType)
            {
                throw new InvalidOperationException(Strings.ObjectContext_PropertyMustBeNavigationProperty(propertyName, resourceType.FullName));
            }

            // Get the NavigationProperty metadata to get the association and ToRole metadata
            EntityType cspaceEntityType = this.ObjectContext.MetadataWorkspace.GetItem<EntityType>(resourceType.FullName, DataSpace.CSpace);
            NavigationProperty navProperty = cspaceEntityType.NavigationProperties[propertyName];

            // Get the relation end 
            var stateEntry = this.ObjectContext.ObjectStateManager.GetObjectStateEntry(targetResource);
            IRelatedEnd relatedEnd = stateEntry.RelationshipManager.GetRelatedEnd(navProperty.RelationshipType.Name, navProperty.ToEndMember.Name);

            try
            {
                if (resourceProperty.Kind == ResourcePropertyKind.ResourceReference)
                {
                    Debug.Assert(addRelationship == null, "addRelationship == null for reference properties");

                    // For FK relationships with 1 to 1 or 1 to 0..1 cardinalities, we need to handle it in a different manner
                    if (IsOneToOneFKAssocation((AssociationType)navProperty.RelationshipType))
                    {
                        // If the parent entity is not in the added state and the other end is not yet loaded, then load it
                        if (stateEntry.State != EntityState.Added && !relatedEnd.IsLoaded)
                        {
                            relatedEnd.Load();
                        }
                    }

                    EntityReference entityReference = relatedEnd as EntityReference;
                    Debug.Assert(entityReference != null, "relatedEnd was not an EntityReference");

                    // if the property value is null, then just set the entity key to null. 
                    // if the key is null, then this will be a no-op, but if the key is not null, then it will
                    // delete the relationship
                    if (propertyValue == null)
                    {
                        entityReference.EntityKey = null;
                    }
                    else
                    {
                        EntityKey key = entityReference.EntityKey;

                        // If the key is null, that means there was no relationship and user is trying to set the relationship for the first time
                        // if there is a key, make sure the relationship is set to something new.
                        // The reason why we need to do a equality check is because when we set the EntityKey to null, it will put the dependent
                        // in deleted state, and setting the same entity key again, will try to add it. Setting the key to the same value is
                        // a very common pattern in the astoria client and hence we need to make sure that scenario works.
                        if (key == null ||
                            !key.Equals(this.ObjectContext.ObjectStateManager.GetObjectStateEntry(propertyValue).EntityKey))
                        {
                            // Get the 'Value' property in the EntityReference<> type and try setting the value directly.
                            PropertyInfo valueProperty = entityReference.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
                            valueProperty.GetSetMethod().Invoke(entityReference, new object[] { propertyValue });
                        }
                    }
                }
                else if (addRelationship == true)
                {
                    Debug.Assert(propertyValue != null, "propertyValue != null");
                    relatedEnd.Add(propertyValue);
                }
                else
                {
                    // The reason why we need to attach first, before removing is ObjectContext might not know about the relationship yet.
                    // Hence attaching establishes the relationship, and then removing marks it as delete.
                    Debug.Assert(propertyValue != null, "propertyValue != null");
                    relatedEnd.Attach(propertyValue);
                    relatedEnd.Remove(propertyValue);
                }
            }
            catch (InvalidOperationException exception)
            {
                throw ObjectContextServiceProvider.CreateBadRequestError(Strings.BadRequest_ErrorInSettingPropertyValue(propertyName), exception);
            }
            catch (ArgumentException exception)
            {
                // In V2, we didn't use to do reflection, and hence we now need to catch both ArgumentException and TargetInvocationException
                // and wrap it with DataServiceException, so that the status code does not get changed.
                throw ObjectContextServiceProvider.CreateBadRequestError(Strings.BadRequest_ErrorInSettingPropertyValue(propertyName), exception);
            }
            catch (TargetInvocationException exception)
            {
                // In V2, we didn't use to do reflection, and hence we now need to catch both ArgumentException and TargetInvocationException
                // and wrap it with DataServiceException, so that the status code does not get changed.
                throw ObjectContextServiceProvider.CreateBadRequestError(Strings.BadRequest_ErrorInSettingPropertyValue(propertyName), exception.InnerException ?? exception);
            }
        }

        /// <summary>
        /// Saves changes on the ObjectContext using the provider's Func
        /// </summary>
        private void SaveContextChanges()
        {
            if (this.saveChangesMethod == null)
            {
                DbContextHelper.GetObjectContext(this.CurrentDataSource, out this.objectContext, out this.saveChangesMethod);
            }

            this.saveChangesMethod();
        }

        /// <summary>
        /// Gets the constructor delegate for the given ResourceType from the cache.
        /// </summary>
        /// <param name="resourceType">ResourceType instance.</param>
        /// <returns>the constructor delegate for the given ResourceType from the cache.</returns>
        private Func<object> GetConstructorDelegate(ResourceType resourceType)
        {
            var cacheEntry = this.MetadataCacheItem.TryGetResourceTypeCacheItem(resourceType.InstanceType);
            return cacheEntry.ConstructorDelegate;
        }

        /// <summary>Sets the value of the property.</summary>
        /// <param name="instance">The object whose property needs to be set.</param>
        /// <param name="resourceTypeCacheItem">ResourceTypeCacheItem containing the ResourceType and its metadata representing the instance parameter.</param>
        /// <param name="propertyValue">new value for the property.</param>
        /// <param name="resourceProperty">metadata for the property to be set.</param>
        private void SetValue(object instance, ResourceTypeCacheItem resourceTypeCacheItem, object propertyValue, ResourceProperty resourceProperty)
        {
            Debug.Assert(instance != null, "instance != null");
            Debug.Assert(resourceTypeCacheItem != null, "resourceTypeCacheItem != null");
            Debug.Assert(resourceProperty != null, "resourceProperty != null");

            MethodInfo setMethod = this.GetResourcePropertyCacheItem(resourceTypeCacheItem, resourceProperty).PropertyInfo.GetSetMethod();
            if (setMethod == null)
            {
                throw ObjectContextServiceProvider.CreateBadRequestError(Strings.BadRequest_PropertyValueCannotBeSet(resourceProperty.Name));
            }

            try
            {
                setMethod.Invoke(instance, new object[] { propertyValue });
            }
            catch (TargetInvocationException exception)
            {
                ErrorHandler.HandleTargetInvocationException(exception);
                throw;
            }
            catch (ArgumentException exception)
            {
                throw ObjectContextServiceProvider.CreateBadRequestError(Strings.BadRequest_ErrorInSettingPropertyValue(resourceProperty.Name), exception);
            }
        }

        /// <summary>
        /// Class for storing metadata for a given ResourceProperty.
        /// </summary>
        private class ObjectContextResourcePropertyCacheItem : ResourcePropertyCacheItem
        {
            /// <summary>>Whether the property value can be null or not.</summary>
            private readonly bool isNullable;

            /// <summary>
            /// Creates a new instance of ResourcePropertyCacheItem.
            /// </summary>
            /// <param name="propertyInfo">PropertyInfo instance for the given ResourceProperty.</param>
            /// <param name="providerMember">IProviderMember which wraps the IEdmMember instance.</param>
            internal ObjectContextResourcePropertyCacheItem(PropertyInfo propertyInfo, IProviderMember providerMember)
                : base(propertyInfo)
            {
                if (providerMember.EdmTypeKind == BuiltInTypeKind.PrimitiveType)
                {
                    this.isNullable = PropertyIsNullable(providerMember.Facets);
                }
                else
                {
                    this.isNullable = false;
                }
            }

            /// <summary>
            /// Returns true if the property value can be null, otherwise false.
            /// </summary>
            public bool IsNullable
            {
                get { return this.isNullable; }
            }

            /// <summary>
            /// Returns whether or not the property's custom annotations indicate that it is nullable
            /// </summary>
            /// <param name="facets">List of facets.</param>
            /// <returns>True if the 'Nullable' annotation is absent or present with the value 'true'. Otherwise returns false.</returns>
            private static bool PropertyIsNullable(IEnumerable<Facet> facets)
            {
                foreach (Facet facet in facets)
                {
                    if (facet.Name == XmlConstants.CsdlNullableAttributeName)
                    {
                        return (bool)facet.Value;
                    }
                }

                // default for CSDL is nullable=true, so if we don't find it we can safely assume it is nullable
                return true;
            }
        }
    }
}
