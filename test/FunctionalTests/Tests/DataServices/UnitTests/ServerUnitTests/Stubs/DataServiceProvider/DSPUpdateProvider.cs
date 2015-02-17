//---------------------------------------------------------------------
// <copyright file="DSPUpdateProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using System.Reflection;

    /// <summary>Implements the <see cref="IDataServiceUpdateProvider"/>.</summary>
    /// <remarks>All the changes requested by calling method on this class are just remembered in a list of pending actions
    /// which are only applied once the SaveChanges method is called.
    /// Note that this class implements support for updating resource reference and resource reference set properties
    /// but it treats each such property on its own. We don't support bi-directional links or relationships.
    /// So for example if there's a resource reference from Product to its Category and a resource reference set from Category to its products
    /// and a Product is modified to reference a certain Category, that Category will not be automatically modifies to include the Product in its
    /// list of products.</remarks>
    public class DSPUpdateProvider : IDataServiceUpdateProvider2
    {
        /// <summary>The data context to apply the change to.</summary>
        private DSPContext dataContext;

        /// <summary>The metadata describing the types to work with.</summary>
        private DSPMetadata metadata;

        /// <summary>List of pending changes to apply once the <see cref="SaveChanges"/> is called.</summary>
        /// <remarks>This is a list of actions which will be called to apply the changes. Discarding the changes is done
        /// simply by clearing this list.</remarks>
        private List<Action> pendingChanges;

        /// <summary>List of properties modified, used to correctly answer GetValue calls.
        /// This is a dictionary where the key is the resource in question and the value is a dictionary which stores property name - value pairs.</summary>
        private Dictionary<object, Dictionary<string, object>> propertyValuesModified;

        /// <summary>Constructor.</summary>
        /// <param name="dataContext">The data context to apply the changes to.</param>
        /// <param name="metadata">The metadata describing the types to work with.</param>
        public DSPUpdateProvider(DSPContext dataContext, DSPMetadata metadata)
        {
            this.dataContext = dataContext;
            this.metadata = metadata;
            this.pendingChanges = new List<Action>();
            this.propertyValuesModified = new Dictionary<object, Dictionary<string, object>>();
        }

        /// <summary>The data context to apply the change to.</summary>
        protected DSPContext DataContext
        {
            get { return this.dataContext; }
        }

        /// <summary>The metadata describing the types to work with.</summary>
        protected DSPMetadata Metadata
        {
            get { return this.metadata; }
        }

        #region IUpdatable Members

        /// <summary>
        /// Adds the given value to the collection
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="resourceToBeAdded">value of the property which needs to be added</param>
        /// <remarks>Adds resource instance <paramref name="resourceToBeAdded"/> into a resource reference set
        /// <paramref name="propertyName"/> on resource <paramref name="targetResource"/>.
        /// Both resources, that is <paramref name="targetResource"/> and <paramref name="resourceToBeAdded"/>, are specified 
        /// in the parameters as the resource "handle".
        /// All changes made by this method should be creates as pending until SaveChanges is called which will commit them (or if it's not called and ClearChanges
        /// is called instead they should be discarded).</remarks>
        public virtual void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            // We don't use resource "handles" so both resources passed in as parameters are the real resource instances.
            DSPResource dspTargetResource = ValidateDSPResource(targetResource);
            DSPResource dspResourceToBeAdded = ValidateDSPResource(resourceToBeAdded);

            // All resource set reference properties must be of type IList<DSPResource> (assumption of this provider)
            // Note that we don't support bi-directional relationships so we only handle the one resource set reference property in isolation.

            IList<DSPResource> list = dspTargetResource.GetValue(propertyName) as IList<DSPResource>;
            if (list == null)
            {
                throw new ArgumentException("The value of the property '" + propertyName + "' does not implement IList<DSPResource>, which is a requirement for resource set reference property.");
            }

            this.pendingChanges.Add(() =>
            {
                list.Add(dspResourceToBeAdded);
            });
        }

        /// <summary>
        /// Revert all the pending changes.
        /// </summary>
        /// <remarks>This method gets called if there was some problem applying changes specified by the request and the changes need to be reverted.
        /// All changes made by the methods on this class should be reverted when this method returns. Note that the class might get used to perform some more
        /// changes after this call.</remarks>
        public void ClearChanges()
        {
            // Simply clear the list of pending changes
            this.pendingChanges.Clear();
            this.propertyValuesModified.Clear();
        }

        /// <summary>
        /// Creates the resource of the given type and belonging to the given container
        /// </summary>
        /// <param name="containerName">container name to which the resource needs to be added</param>
        /// <param name="fullTypeName">full type name i.e. Namespace qualified type name of the resource</param>
        /// <returns>object representing a resource of given type and belonging to the given container</returns>
        /// <remarks>The method should create a new instance of the resource type specified by the <paramref name="fullTypeName"/> 
        /// and add it to the resource set specified by the <paramref name="containerName"/>.
        /// The method should then return the "handle" to the resource just created.
        /// All properties of the new resource should have their default values.
        /// This method is called in two slightly different cases:
        ///   - entity resource creation - in this case the <paramref name="containerName"/> specifies the name of the resource set
        ///       the newly created entity should be added to and the <paramref name="fullTypeName"/> is the FullName of the resource type representing
        ///       entity type. The method should create new instance of the type and add it to the resource set.
        ///   - complex resource creation - in this case the <paramref name="containerName"/> is null and the <paramref name="fullTypeName"/>
        ///       specified the FullName of the resource type representing a complex type. The method should just create new instance of the type
        ///       and return it. Later the <see cref="SetValue"/> will be called to set the complex type instance returned as a value of some
        ///       complex property.
        /// All changes made by this method should be creates as pending until SaveChanges is called which will commit them (or if it's not called and ClearChanges
        /// is called instead they should be discarded).</remarks>
        public virtual object CreateResource(string containerName, string fullTypeName)
        {
            ResourceType resourceType;
            if (!this.metadata.TryResolveResourceType(fullTypeName, out resourceType))
            {
                throw new ArgumentException("Unknown resource type '" + fullTypeName + "'.");
            }

            // Create new instance of the DSPResource (this will create empty property bag, which is the same as all properties having default values)
            DSPResource newResource = (DSPResource)Activator.CreateInstance(resourceType.InstanceType, resourceType);

            if (containerName != null)
            {
                // We're creating an entity and should add it to the resource set
                // This check here is just for documentation, the method should never be called with non-entity type in this case.
                if (resourceType.ResourceTypeKind != ResourceTypeKind.EntityType)
                {
                    throw new ArgumentException("The specified resource type '" + fullTypeName + "' is not an entity type, but resource set was specified.");
                }

                IList<object> resourceSetList = this.dataContext.GetResourceSetEntities(containerName);

                // And register pending change to add the resource to the resource set list
                this.pendingChanges.Add(() =>
                {
                    resourceSetList.Add(newResource);
                });
            }
            else
            {
                // We're creating a complex type instance, so no additional operation is needed.
                // This check here is just for documentation the method should never be called with non-complex type in this case.
                if (resourceType.ResourceTypeKind != ResourceTypeKind.ComplexType)
                {
                    throw new ArgumentException("The specified resource type '" + fullTypeName + "' is not a complex type.");
                }
            }

            // The method should return the resource "handle", we don't have handles so we return the resource itself directly.
            return newResource;
        }

        /// <summary>
        /// Delete the given resource
        /// </summary>
        /// <param name="targetResource">resource that needs to be deleted</param>
        /// <remarks>This method gets a "handle" to a resource in the <paramref name="targetResource"/> and should pend a change which
        /// deletes that resource.
        /// That includes removing the resource from its resource set and freeing up all the resources associated with that resource.
        /// Note that this method is not called for complex type instances, only entity resurces are deleted in this way. Complex type instances 
        /// should be deleted when the entity type which points to them is deleted.
        /// All changes made by this method should be creates as pending until SaveChanges is called which will commit them (or if it's not called and ClearChanges
        /// is called instead they should be discarded).</remarks>
        public virtual void DeleteResource(object targetResource)
        {
            DSPResource dspTargetResource = ValidateDSPResource(targetResource);

            ResourceSet resourceSet = null;
            ResourceType rt = dspTargetResource.ResourceType;

            while (rt != null)
            {
                resourceSet = rt.GetAnnotation().ResourceSet;
                if (resourceSet != null) break;
                rt = rt.BaseType;
            }
            
            IList<object> resourceSetList = this.dataContext.GetResourceSetEntities(resourceSet.Name);

            // Add a pending change to remove the resource from the resource set
            this.pendingChanges.Add(() =>
            {
                resourceSetList.Remove(dspTargetResource);
            });
        }

        /// <summary>
        /// Gets the resource of the given type that the query points to
        /// </summary>
        /// <param name="query">query pointing to a particular resource</param>
        /// <param name="fullTypeName">full type name i.e. Namespace qualified type name of the resource</param>
        /// <returns>object representing a resource of given type and as referenced by the query</returns>
        /// <remarks>This method should obtain a single result from the specified <paramref name="query"/>. It should fail if no or more than one result
        /// can be obtain by evaluating such query.
        /// The result should then be converted to its resource "handle" and that handle should be returned from the method.
        /// The <paramref name="fullTypeName"/> is the expected FullName of the resource type of the resource to be retrieved. If this parameter is null
        /// the method should ignore it. If it's not null, the method should check that the resource returned by the query is of this resource type
        /// and fail if that's not the case.</remarks>
        public virtual object GetResource(System.Linq.IQueryable query, string fullTypeName)
        {
            // Since we're not using resource handles we're going to return the resource itself.
            object resource = null;
            foreach (object r in query)
            {
                if (resource != null)
                {
                    throw new ArgumentException(String.Format("Invalid Uri specified. The query '{0}' must refer to a single resource", query.ToString()));
                }

                resource = r;
            }

            if (resource != null)
            {
                if (fullTypeName != null)
                {
                    ResourceType resourceType;
                    if (!this.metadata.TryResolveResourceType(fullTypeName, out resourceType))
                    {
                        throw new ArgumentException("Unknown resource type '" + fullTypeName + "'.");
                    }

                    if (!resourceType.InstanceType.IsAssignableFrom(resource.GetType()))
                    {
                        throw new System.ArgumentException(String.Format("Invalid uri specified. Expected instance type: '{0}', Actual instance type: '{1}'", resourceType.InstanceType.FullName, resource.GetType().FullName));
                    }
                }

                return resource;
            }

            return null;
        }

        /// <summary>
        /// Gets the value of the given property on the target object
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <returns>the value of the property for the given target resource</returns>
        /// <remarks>The method gets a resource "handle" in the <paramref name="targetResource"/> and the name of a resource property
        /// defined on it and should return the value of that property.</remarks>
        public virtual object GetValue(object targetResource, string propertyName)
        {
            DSPResource dspTargetResource = ValidateDSPResource(targetResource);

            // First try to get the value from the modified properties
            Dictionary<string, object> propertiesModified;
            if (this.propertyValuesModified.TryGetValue(dspTargetResource, out propertiesModified))
            {
                object propertyValue;
                if (propertiesModified.TryGetValue(propertyName, out propertyValue))
                {
                    return propertyValue;
                }
            }

            // Only if the property has no been modified yet, return the value from the actual resource.
            return dspTargetResource.GetValue(propertyName);
        }

        /// <summary>
        /// Removes the given value from the collection
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="resourceToBeRemoved">value of the property which needs to be removed</param>
        /// <remarks>Removes resource instance <paramref name="resourceToBeRemoved"/> from a resource reference set
        /// <paramref name="propertyName"/> on resource <paramref name="targetResource"/>.
        /// Both resources, that is <paramref name="targetResource"/> and <paramref name="resourceToBeRemoved"/>, are specified 
        /// in the parameters as the resource "handle".
        /// All changes made by this method should be creates as pending until SaveChanges is called which will commit them (or if it's not called and ClearChanges
        /// is called instead they should be discarded).</remarks>
        public virtual void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            // We don't use resource "handles" so both resources passed in as parameters are the real resource instances.
            DSPResource dspTargetResource = ValidateDSPResource(targetResource);
            DSPResource dspResourceToBeRemoved = ValidateDSPResource(resourceToBeRemoved);

            // All resource set reference properties must be of type IList<DSPResource> (assumption of this provider)
            // We don't use resource "handles" so both resources passed in as parameters are the real resource instances.
            // Note that we don't support bi-directional relationships so we only handle the one resource set reference property in isolation.

            // We will use the GetValue we already implement to get the IList
            IList<DSPResource> list = this.GetValue(targetResource, propertyName) as IList<DSPResource>;
            if (list == null)
            {
                throw new ArgumentException("The value of the property '" + propertyName + "' does not implement IList<DSPResource>, which is a requirement for resource set reference property.");
            }

            this.pendingChanges.Add(() =>
            {
                list.Remove(dspResourceToBeRemoved);
            });
        }

        /// <summary>
        /// Resets the value of the given resource to its default value
        /// </summary>
        /// <param name="resource">resource whose value needs to be reset</param>
        /// <returns>same resource with its value reset</returns>
        /// <remarks>This method should reset resource properties to their default values.
        /// The resource is specfied by its resource "handle" by the <paramref name="resource"/>.
        /// The method can choose to modify the existing resource or create a new one and it should return a resource "handle"
        /// to the resource which has its properties with default values. If it chooses to return a new resource it must also
        /// replace that old resource with the new one in its resource set and all the references which may point to it.
        /// The returned resource must have the same identity as the one on the input. That means all its key properties must have the same value.
        /// All changes made by this method should be creates as pending until SaveChanges is called which will commit them (or if it's not called and ClearChanges
        /// is called instead they should be discarded).</remarks>
        public virtual object ResetResource(object resource)
        {
            DSPResource dspResource = ValidateDSPResource(resource);

            this.pendingChanges.Add(() =>
            {
                foreach (var resourceProperty in dspResource.ResourceType.Properties)
                {
                    // We must only reset values of non-key properties, the key properties must be persited (to maintain the identity of the resource instance)
                    if ((resourceProperty.Kind & ResourcePropertyKind.Key) != ResourcePropertyKind.Key)
                    {
                        dspResource.SetValue(resourceProperty.Name, null);
                    }
                }
            });

            return resource;
        }

        /// <summary>
        /// Returns the actual instance of the resource represented by the given resource object
        /// </summary>
        /// <param name="resource">object representing the resource whose instance needs to be fetched</param>
        /// <returns>The actual instance of the resource represented by the given resource object</returns>
        public object ResolveResource(object resource)
        {
            // We're not using resource handles, so the resource is also the handle itself
            // It is possible to represent resources with "handles" here instead. This method is meant to translate the resource handle 
            // passed in the parameter "resource" to the underlying resource instance.
            return resource;
        }

        /// <summary>
        /// Saves all the pending changes made till now
        /// </summary>
        /// <remarks>All changes made by methods on this class should not be persisted until this SaveChanges method gets called.
        /// After this method returns the changes should be persited in the underlying data storage.
        /// Note that this class might be used to perform additional update operations after this method is called.</remarks>
        public void SaveChanges()
        {
            // Just run all the pending changes we gathered so far
            foreach (var pendingChange in this.pendingChanges)
            {
                pendingChange();
            }

            this.pendingChanges.Clear();
            this.propertyValuesModified.Clear();
        }

        /// <summary>
        /// Sets the value of the given reference property on the target object
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="propertyValue">value of the property</param>
        /// <remarks>Sets a new value for a resource reference property.
        /// - Create new reference if the old value of the reference property was null and the new value <paramref name="propertyValue"/> is not-null.
        /// - Update the reference if the old value of the reference property was non-null and the new value <paramref name="propertyValue"/> is not-null.
        /// - Remove the reference if the old value of the reference property was non-null and the new value <paramref name="propertyValue"/> is null.
        /// All changes made by this method should be creates as pending until SaveChanges is called which will commit them (or if it's not called and ClearChanges
        /// is called instead they should be discarded).</remarks>
        public virtual void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            // Note that we don't support bi-directional relationships so we only handle the one resource reference property in isolation.

            // Our reference properties are just like normal properties we just set the property value to the new value
            //   we don't perform any special actions for references.
            // So just call the SetValue which will do exactly that.
            this.SetValue(targetResource, propertyName, propertyValue);
        }

        /// <summary>
        /// Sets the value of the given property on the target object
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="propertyValue">value of the property</param>
        /// <remarks>This method should pend a change which will set a resource property with name <paramref name="propertyName"/>
        /// to value specified by <paramref name="propertyValue"/> on the resource specified by the resource "handle" <paramref name="targetResource"/>.
        /// All changes made by this method should be creates as pending until SaveChanges is called which will commit them (or if it's not called and ClearChanges
        /// is called instead they should be discarded).</remarks>
        public virtual void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            DSPResource dspTargetResource = ValidateDSPResource(targetResource);

            // Note that we rely on the DSPResource.SetValue implementation to correctly deal with collections.

            // Add a pending change to modify the value of the property
            this.pendingChanges.Add(() =>
            {
                dspTargetResource.SetValue(propertyName, propertyValue);
            });

            // And remember the new value
            Dictionary<string, object> propertiesModified;
            if (!this.propertyValuesModified.TryGetValue(dspTargetResource, out propertiesModified))
            {
                propertiesModified = new Dictionary<string, object>();
                this.propertyValuesModified[dspTargetResource] = propertiesModified;
            }
            propertiesModified[propertyName] = propertyValue;
        }

        #endregion

        #region IDataServiceUpdateProvider Members

        /// <summary>
        /// Passes the etag value for the given resource.
        /// </summary>
        /// <param name="resourceCookie">cookie representing the resource.</param>
        /// <param name="checkForEquality">true if we need to compare the property values for equality. If false, then we need to compare values for non-equality.</param>
        /// <param name="concurrencyValues">list of the etag property names and its corresponding values.</param>
        public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
        {
            DSPResource dspTargetResource = ValidateDSPResource(resourceCookie);

            // This implementation does pretty much exactly the same thing as the product if no IDataServiceUpdateProvider is implemented
            // Especially this throws the same exceptions/status codes as the product.
            if (checkForEquality == null)
            {
                throw new DataServiceException(400, "Update operation to resource of type '" + dspTargetResource.ResourceType.FullName + "' is missing an If-Match header even though the resource has an ETag.");
            }

            foreach (var concurrencyValue in concurrencyValues)
            {
                if (!concurrencyValue.Value.Equals(dspTargetResource.GetValue(concurrencyValue.Key)))
                {
                    throw new DataServiceException(412, "Wrong If-Match header value for property '" + concurrencyValue.Key + "'. The header value is '" + concurrencyValue.Value.ToString() + "', but the expected value is '" + (dspTargetResource.GetValue(concurrencyValue.Key) ?? "null").ToString() + "'.");
                }
            }
        }

        #endregion

        #region IDataServiceUpdateProvider2 Members

        /// <summary>
        /// Queues up the <paramref name="invokable"/> to be invoked during IUpdatable.SaveChanges().
        /// </summary>
        /// <param name="invokable">The invokable instance whose Invoke() method will be called during IUpdatable.SaveChanges().</param>
        public void ScheduleInvokable(IDataServiceInvokable invokable)
        {
            this.pendingChanges.Add(invokable.Invoke);
        }

        #endregion IDataServiceUpdateProvider2 Members

        /// <summary>Validates that the <paramref name="resource"/> is a <see cref="DSPResource"/>.</summary>
        /// <param name="resource">The resource instance to validate.</param>
        /// <returns>The resource instance casted to <see cref="DSPResource"/>.</returns>
        public static DSPResource ValidateDSPResource(object resource)
        {
            DSPResource dspResource = resource as DSPResource;
            if (resource != null && dspResource == null)
            {
                throw new ArgumentException("The specified resource is not a DSPResource. That is not supported by this provider.");
            }

            return dspResource;
        }
    }
}
