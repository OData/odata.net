//---------------------------------------------------------------------
// <copyright file="ReflectionDataContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Reflection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.OData.Service.Providers;
    using Microsoft.Test.OData.Framework.TestProviders.Common;
    using Microsoft.Test.OData.Framework.TestProviders.Contracts;

    /// <summary>
    /// ReflectionDataContext class tracks data in-memory
    /// </summary>
    public abstract class ReflectionDataContext
    {
        private static Dictionary<Type, Dictionary<string, IList>> resourceSetsByContextTypeStorage = new Dictionary<Type, Dictionary<string, IList>>();

        /// <summary>List of pending changes to apply once the <see cref="SaveChanges"/> is called.</summary>
        /// <remarks>This is a list of actions which will be called to apply the changes. Discarding the changes is done
        /// simply by clearing this list.</remarks>
        private List<Action> pendingChanges;
        private List<object> deletedObjects = new List<object>();

        /// <summary>
        /// Initializes a new instance of the ReflectionDataContext class
        /// </summary>
        protected ReflectionDataContext()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ReflectionDataContext class
        /// </summary>
        /// <param name="skipDataInitialization">if set to <c>true</c>, then skip data initialization.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Initialization must happen at construction time")]
        protected ReflectionDataContext(bool skipDataInitialization)
        {
            this.ReflectionMetadataHelper = new ReflectionMetadataHelper(this.GetType());
            this.pendingChanges = new List<Action>();

            if (!resourceSetsByContextTypeStorage.ContainsKey(this.GetType()))
            {
                resourceSetsByContextTypeStorage.Add(this.GetType(), new Dictionary<string, IList>());

                foreach (string resourceSetName in this.ReflectionMetadataHelper.GetResourceSetNames())
                {
                    Type resourceType = this.ReflectionMetadataHelper.GetResourceTypeOfSet(resourceSetName);

                    Type listOfResourceType = typeof(List<>).MakeGenericType(new Type[] { resourceType });

                    var listOfTInstance = Activator.CreateInstance(listOfResourceType) as IList;

                    this.ResourceSetsStorage.Add(resourceSetName, listOfTInstance);
                }
            }

            if (!skipDataInitialization)
            {
                this.EnsureDataIsInitialized();
            }
        }

        /// <summary>
        /// Gets a ReflectionMetadataHelper
        /// </summary>
        internal ReflectionMetadataHelper ReflectionMetadataHelper { get; private set; }

        internal Dictionary<string, IList> ResourceSetsStorage
        {
            get
            {
                Dictionary<string, IList> resourceSetsLookup = null;

                Type currentContextType = this.GetType();
                bool found = resourceSetsByContextTypeStorage.TryGetValue(currentContextType, out resourceSetsLookup);

                ExceptionUtilities.Assert(found, "Cannot find resource sets by the context type '{0}'", currentContextType);

                return resourceSetsLookup;
            }
        }

        /// <summary>
        /// Gets the full name of the resource type representing the given CLR type
        /// </summary>
        /// <param name="type">The CLR type</param>
        /// <returns>The resource type full name</returns>
        public static string GetResourceTypeFullName(Type type)
        {
            return type.FullName.Replace('+', '_');
        }

        /// <summary>
        /// Gets a List of T based on the resourceSet name provided
        /// </summary>
        /// <typeparam name="T">Type of the ResourceSet</typeparam>
        /// <param name="resourceSetName">Name of the ResourceSet</param>
        /// <returns>A List of the items in a particular resourceSet</returns>
        public IList<T> GetResourceSetEntities<T>(string resourceSetName)
        {
            return (IList<T>)this.GetResourceSetEntities(resourceSetName);
        }

        /// <summary>
        /// Clears Data in static lists
        /// </summary>
        public void ClearData()
        {
            this.ResourceSetsStorage.Clear();
        }
        
        /// <summary>
        /// Adds the given value to the collection
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="resourceToBeAdded">value of the property which needs to be added</param>
        public virtual void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(resourceToBeAdded, "resourceToBeAdded");
            
            // resolve the target token
            var targetToken = UpdatableToken.AssertIsToken(targetResource, "targetResource");
            targetResource = targetToken.Resource;
            
            // resolve the added token
            resourceToBeAdded = UpdatableToken.AssertIsTokenAndResolve(resourceToBeAdded, "resourceToBeAdded");
            
            // All resource set reference properties must be of type IList<T> where T is the type of an entitySet
            // Note that we don't support bi-directional relationships so we only handle the one resource set reference property in isolation.
            IList list = this.GetValue(targetToken, propertyName) as IList;
            ExceptionUtilities.CheckObjectNotNull(list, "Property '{0}' on type '{1}' was not a list", propertyName, targetResource.GetType().Name);

            this.pendingChanges.Add(() =>
            {
                list.Add(resourceToBeAdded);
            });
        }

        /// <summary>
        /// Revert all the pending changes.
        /// </summary>
        public virtual void ClearChanges()
        {
            // Simply clear the list of pending changes
            this.pendingChanges.Clear();
        }

        /// <summary>
        /// Creates a new Resource using the set and type name
        /// </summary>
        /// <param name="containerName">Name of the ResourceSet</param>
        /// <param name="fullTypeName">Name of the Type</param>
        /// <returns>Returns a created resource</returns>
        public virtual object CreateResource(string containerName, string fullTypeName)
        {
            ExceptionUtilities.CheckArgumentNotNull(fullTypeName, "fullTypeName");

            var token = this.InstantiateResourceType(fullTypeName);

            if (containerName != null)
            {
                // And register pending change to add the resource to the resource set list
                this.pendingChanges.Add(() =>
                {
                    IList resourceSetList = this.GetResourceSetEntities(containerName);
                    resourceSetList.Add(token.Resource);
                });
            }
       
            // return a token representing the new resource
            return token;
        }

        /// <summary>
        /// Delete the given resource
        /// </summary>
        /// <param name="targetResource">resource that needs to be deleted</param>
        public virtual void DeleteResource(object targetResource)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");

            targetResource = UpdatableToken.AssertIsTokenAndResolve(targetResource, "targetResource");

            string resourceSetName = this.GetResourceSetOfTargetResource(targetResource);
            ExceptionUtilities.CheckObjectNotNull(resourceSetName, "Unable to find set of the resource to delete");

            this.deletedObjects.Add(targetResource);

            IList resourceSetList = this.GetResourceSetEntities(resourceSetName);

            // Remove any any references this target resource has
            this.DeleteAllReferences(targetResource);

            // Add a pending change to remove the resource from the resource set
            this.pendingChanges.Add(() =>
            {
                resourceSetList.Remove(targetResource);
            });
        }
        
        /// <summary>
        /// Gets the resource of the given type that the query points to
        /// </summary>
        /// <param name="query">query pointing to a particular resource</param>
        /// <param name="fullTypeName">full type name i.e. Namespace qualified type name of the resource</param>
        /// <returns>object representing a resource of given type and as referenced by the query</returns>
        public virtual object GetResource(IQueryable query, string fullTypeName)
        {
            ExceptionUtilities.CheckArgumentNotNull(query, "query");

            // Since we're not using resource handles we're going to return the resource itself.
            object resource = null;
            foreach (object r in query)
            {
                ExceptionUtilities.Assert(resource == null, "Invalid Uri specified. The query '{0}' must refer to a single resource", query.ToString());
                resource = r;
            }

            if (resource != null)
            {
                if (fullTypeName != null)
                {
                    this.ValidateResourceType(resource, fullTypeName);
                }

                // return a token for this resource
                return new UpdatableToken(resource);
            }

            return null;
        }

        /// <summary>
        /// Get resources
        /// </summary>
        /// <param name="queryable">Queryable to get data from</param>
        /// <returns>Object to return</returns>
        public virtual object GetResources(IQueryable queryable)
        {
            ExceptionUtilities.CheckArgumentNotNull(queryable, "queryable");

            if (DataServiceOverrides.UpdateProvider2.GetResourcesFunc != null)
            {
                return DataServiceOverrides.UpdateProvider2.GetResourcesFunc(queryable);
            }

            return queryable;
        }

        /// <summary>
        /// Gets the value of the given property on the target object
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <returns>the value of the property for the given target resource</returns>
        public virtual object GetValue(object targetResource, string propertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");

            // resolve the token, and return a pending value if there is one
            // NOTE: this code is specifically to handle cases of mapped complex-type values, because the product does not cache the
            // value returned by CreateResource so we need to take into account any pending updates, or we risk returning stale data
            var token = UpdatableToken.AssertIsToken(targetResource, "targetResource");
            if (token.PendingPropertyUpdates.ContainsKey(propertyName))
            {
                return token.PendingPropertyUpdates[propertyName];
            }

            targetResource = token.Resource;
            PropertyInfo pi = targetResource.GetType().GetProperty(propertyName);
            ExceptionUtilities.CheckObjectNotNull(pi, "Cannot find the property '{0}' on type '{1}'", propertyName, targetResource.GetType().Name);

            var value = pi.GetValue(targetResource, null);

            // NOTE: we need to token-ize any complex values before returning them
            // we should have a better way of telling a type is complex, but this works for now
            if (value != null && pi.PropertyType.Assembly == this.GetType().Assembly)
            {
                ExceptionUtilities.Assert(!this.ReflectionMetadataHelper.IsTypeAnEntityType(pi.PropertyType), "GetValue should never be called for reference properties. Type was '{0}', property was '{1}'", pi.PropertyType.FullName, propertyName);
                value = new UpdatableToken(value);
            }

            return value;
        }

        /// <summary>
        /// Removes the given value from the collection
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="resourceToBeRemoved">value of the property which needs to be removed</param>
        public virtual void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(resourceToBeRemoved, "resourceToBeRemoved");

            UpdatableToken.AssertIsToken(targetResource, "targetResource");
            resourceToBeRemoved = UpdatableToken.AssertIsTokenAndResolve(resourceToBeRemoved, "resourceToBeRemoved");

            // We will use the GetValue we already implement to get the IList
            IList list = this.GetValue(targetResource, propertyName) as IList;
            ExceptionUtilities.CheckObjectNotNull(list, "Property '{0}' on type '{1}' was not a list", propertyName, targetResource.GetType().Name);

            this.pendingChanges.Add(() =>
            {
                list.Remove(resourceToBeRemoved);
            });
        }

        /// <summary>
        /// Resets the value of the given resource to its default value
        /// </summary>
        /// <param name="resource">resource whose value needs to be reset</param>
        /// <returns>same resource with its value reset</returns>
        public virtual object ResetResource(object resource)
        {
            ExceptionUtilities.CheckArgumentNotNull(resource, "resource");
            var token = UpdatableToken.AssertIsToken(resource, "resource");
            resource = token.Resource;

            // create a new token to return
            token = new UpdatableToken(resource);

            object newInstance = Activator.CreateInstance(resource.GetType());
            ExceptionUtilities.CheckObjectNotNull(newInstance, "Cannot reset resource because unable to creating new instance of type '{0}' returns null", resource.GetType().Name);
            
            string[] propertiesToReset = this.ReflectionMetadataHelper.GetPropertiesToReset(GetResourceTypeFullName(resource.GetType()));

            // We must only reset values of scalar, scalar Collection, complex, and complexCollection properties, the key and navigations must stay the same   
            foreach (string propertyToReset in propertiesToReset)
            {
                PropertyInfo pi = newInstance.GetType().GetProperty(propertyToReset);
                ExceptionUtilities.CheckObjectNotNull(pi, "Cannot reset resource because unable to find property '{0}'", propertyToReset);
                object newValue = pi.GetValue(newInstance, null);
                this.pendingChanges.Add(() => pi.SetValue(resource, newValue, null));
                token.PendingPropertyUpdates[propertyToReset] = newValue;
            }

            return token;
        }

        /// <summary>
        /// Returns the actual instance of the resource represented by the given resource object
        /// </summary>
        /// <param name="resource">object representing the resource whose instance needs to be fetched</param>
        /// <returns>The actual instance of the resource represented by the given resource object</returns>
        public virtual object ResolveResource(object resource)
        {
            ExceptionUtilities.CheckArgumentNotNull(resource, "resource");
            return UpdatableToken.AssertIsTokenAndResolve(resource, "resource");
        }

        /// <summary>
        /// Saves all the pending changes made till now
        /// </summary>
        public virtual void SaveChanges()
        {
            // Just run all the pending changes we gathered so far
            foreach (var pendingChange in this.pendingChanges)
            {
                pendingChange();
            }

            this.pendingChanges.Clear();

            foreach (var deleted in this.deletedObjects)
            {
                foreach (var entity in this.ResourceSetsStorage.SelectMany(p => p.Value.Cast<object>()))
                {
                    ExceptionUtilities.Assert(!object.ReferenceEquals(deleted, entity), "Found deleted entity!");
                    foreach (var propertyInfo in entity.GetType().GetProperties())
                    {
                        var value = propertyInfo.GetValue(entity, null);
                        ExceptionUtilities.Assert(!object.ReferenceEquals(deleted, value), "Found deleted entity!");

                        var enumerable = value as IEnumerable;
                        if (enumerable != null)
                        {
                            foreach (var valueElement in enumerable.Cast<object>())
                            {
                                ExceptionUtilities.Assert(!object.ReferenceEquals(deleted, valueElement), "Found deleted entity!");
                            }
                        }
                    }
                }
            }

            this.deletedObjects.Clear();
        }

        /// <summary>
        /// Sets the value of the given reference property on the target object
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="propertyValue">value of the property</param>
        public virtual void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            if (propertyValue != null)
            {
                UpdatableToken.AssertIsToken(propertyValue, "propertyValue");
            }
            
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
        public virtual void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");

            var token = UpdatableToken.AssertIsToken(targetResource, "targetResource");
            targetResource = token.Resource;

            // note that propertyValue might be a token itself, but GetValue will simply return whatever is stored in the token
            token.PendingPropertyUpdates[propertyName] = propertyValue;

            // Add a pending change to modify the value of the property
            this.pendingChanges.Add(() =>
            {
                Type t = targetResource.GetType();
                PropertyInfo pi = t.GetProperty(propertyName);
                ExceptionUtilities.CheckObjectNotNull(pi, "Unable to find property '{0}' on type '{1}'", propertyName, targetResource.GetType().Name);

                string entitySetName = this.ReflectionMetadataHelper.FindSetNameForType(t);
                object generatedValue;
                if (this.TryGetStoreGeneratedValue(entitySetName, GetResourceTypeFullName(t), propertyName, out generatedValue))
                {
                    propertyValue = generatedValue;
                }
                
                if (this.IsCollectionProperty(pi))
                {
                    ExceptionUtilities.CheckObjectNotNull(propertyValue, "Collection property value was null");

                    var enumerable = propertyValue as IEnumerable;
                    ExceptionUtilities.CheckObjectNotNull(enumerable, "Collection property value was not an enumerable");

                    this.SetCollectionPropertyValue(targetResource, pi, enumerable);
                }
                else
                {
                    propertyValue = UpdatableToken.ResolveIfToken(propertyValue);
                    pi.SetValue(targetResource, propertyValue, null);
                }
            });
        }

        /// <summary>
        /// Sets the concurrencyValues
        /// </summary>
        /// <param name="resourceCookie">The resource to be evaluated</param>
        /// <param name="checkForEquality">Determines whether to apply the equality check or not</param>
        /// <param name="concurrencyValues">The concurrency values to compare against</param>
        public virtual void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
        {
            ExceptionUtilities.CheckArgumentNotNull(resourceCookie, "resourceCookie");
            ExceptionUtilities.ThrowDataServiceExceptionIfFalse(checkForEquality.HasValue, 417, "Missing concurrency token for update operation");
            ExceptionUtilities.Assert(checkForEquality.Value, "Should not be called with check for equality parameter equal to false");
            ExceptionUtilities.CheckArgumentNotNull(concurrencyValues, "concurrencyValues");

            // If-Match: *
            if (!concurrencyValues.Any())
            {
                return;
            }

            resourceCookie = UpdatableToken.AssertIsTokenAndResolve(resourceCookie, "resourceCookie");
            
            Dictionary<string, object> etags = this.GetConcurrencyValues(resourceCookie);

            bool matches = CompareETagValues(etags, concurrencyValues);
            ExceptionUtilities.ThrowDataServiceExceptionIfFalse(matches, 412, "Concurrency tokens do not match");
        }

        /// <summary>
        /// Adds an invokable to list of operations occuring in savechanges
        /// </summary>
        /// <param name="invokable">Action to invoke at save changes</param>
        public virtual void ScheduleInvokable(IDataServiceInvokable invokable)
        {
            if (DataServiceOverrides.UpdateProvider2.ImmediateCreateInvokableFunc != null)
            {
                DataServiceOverrides.UpdateProvider2.ImmediateCreateInvokableFunc(invokable);
            }
            else if (DataServiceOverrides.UpdateProvider2.AddPendingActionsCreateInvokableFunc != null)
            {
                this.pendingChanges.Add(
                    () =>
                    {
                        DataServiceOverrides.UpdateProvider2.AddPendingActionsCreateInvokableFunc(invokable);
                    });
            }
            else
            {
                this.pendingChanges.Add(
                    () =>
                    {
                        invokable.Invoke();
                    });
            }
        }

        internal IList GetResourceSetEntities(string resourceSetName)
        {
            IList entities;
            if (!this.ResourceSetsStorage.TryGetValue(resourceSetName, out entities))
            {
                Type elementType = this.ReflectionMetadataHelper.GetResourceTypeOfSet(resourceSetName);
                entities = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                this.ResourceSetsStorage[resourceSetName] = entities;
            }

            return entities;
        }

        /// <summary>
        /// Returns whether or not the given property is a collection property
        /// </summary>
        /// <param name="propertyInfo">The property to check</param>
        /// <returns>True if the property type is IEnumerable and not a string or byte array</returns>
        protected virtual bool IsCollectionProperty(PropertyInfo propertyInfo)
        {
            return typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) && propertyInfo.PropertyType != typeof(string) && propertyInfo.PropertyType != typeof(byte[]);
        }

        /// <summary>
        /// Gets the instance type of a particular collection property
        /// </summary>
        /// <param name="fullTypeName">The full type name</param>
        /// <param name="propertyName">The property name</param>
        /// <returns>The instance type or null if the type is unknown</returns>
        protected virtual Type GetCollectionPropertyType(string fullTypeName, string propertyName)
        {
            var type = this.ReflectionMetadataHelper.FindClrTypeByFullName(fullTypeName);
            Type collectionType = null;
            if (type != null)
            {
                var property = type.GetProperty(propertyName);
                if (property != null)
                {
                    collectionType = property.PropertyType;
                }
            }

            return collectionType;
        }

        /// <summary>
        /// Sets the value of a collection property
        /// </summary>
        /// <param name="targetResource">The resource to set the value on</param>
        /// <param name="propertyInfo">The property to set</param>
        /// <param name="propertyValue">The collection value</param>
        protected virtual void SetCollectionPropertyValue(object targetResource, PropertyInfo propertyInfo, IEnumerable propertyValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");
            ExceptionUtilities.CheckArgumentNotNull(propertyInfo, "propertyInfo");
            ExceptionUtilities.CheckArgumentNotNull(propertyValue, "propertyValue");
            
            Type collectionType = this.GetCollectionPropertyType(GetResourceTypeFullName(propertyInfo.ReflectedType), propertyInfo.Name);
            ExceptionUtilities.CheckObjectNotNull(collectionType, "Could not infer collection type for property");
            object collection;

            // need to go through the enumerable and resolve any tokens
            propertyValue = propertyValue.Cast<object>().Select(o => UpdatableToken.ResolveIfToken(o));

            // Algorithm for setting a collection value:
            // - Get a collection of the correct instance type
            //   - Look for a constructor that takes a non-generic IEnumerable, if one is found invoke it
            //   - If the type is generic
            //     - look for a constuctor taking IEnumerable<T>, if one is found invoke it
            //     - look for an Add method taking T, if one is found use default constructor and add each item
            //   - look for an untyped Add method, if one is found use default constructor and add each item
            // - Set the property normally, using the new collection instance
            var enumerableConstructor = collectionType.GetConstructor(new Type[] { typeof(IEnumerable) });
            if (enumerableConstructor != null)
            {
                // invoke the IEnumerable constructor with the property value
                collection = enumerableConstructor.Invoke(new object[] { propertyValue });
            }
            else if (collectionType.IsGenericType && collectionType.GetGenericArguments().Count() == 1)
            {
                // determine the element type
                var typeArgument = collectionType.GetGenericArguments().Single();

                // look for a constructor taking IEnumerable<T>
                var typedEnumerableConstructor = collectionType.GetConstructor(new Type[] { typeof(IEnumerable<>).MakeGenericType(typeArgument) });

                if (typedEnumerableConstructor != null)
                {
                    // convert the IEnumerable into IEnumerable<T> using Enumerable.Cast
                    var typedEnumerable = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(typeArgument).Invoke(null, new object[] { propertyValue });

                    // invoke the constructor
                    collection = typedEnumerableConstructor.Invoke(new object[] { typedEnumerable });
                }
                else
                {
                    // look for an Add method accepting T
                    var typedAddMethod = collectionType.GetMethod("Add", new Type[] { typeArgument });

                    // for generic types, we must find either a constructor or an Add method
                    ExceptionUtilities.CheckObjectNotNull(typedAddMethod, "Could not find constructor or add method for type: " + collectionType.FullName);

                    // create a new instance, and add each item
                    collection = Activator.CreateInstance(collectionType);
                    foreach (var element in propertyValue)
                    {
                        typedAddMethod.Invoke(collection, new object[] { element });
                    }
                }
            }
            else
            {
                // look for an Add method
                var addMethod = collectionType.GetMethod("Add");

                // fail if no method is found
                ExceptionUtilities.CheckObjectNotNull(addMethod, "Could not find constructor or add method for type: " + collectionType.FullName);

                // create a new instance and add each item
                collection = Activator.CreateInstance(collectionType);
                foreach (var element in propertyValue)
                {
                    addMethod.Invoke(collection, new object[] { element });
                }
            }

            // set the new collection instance as the value
            propertyInfo.SetValue(targetResource, collection, null);
        }

        /// <summary>
        /// Initializes data the first time it is called, then does nothing on subsequent calls
        /// </summary>
        protected abstract void EnsureDataIsInitialized();

        /// <summary>
        /// Attempts to get a store-generated value for the given property of an entity
        /// </summary>
        /// <param name="entitySetName">The entity set name</param>
        /// <param name="fullTypeName">The full type name of the entity</param>
        /// <param name="propertyName">The property name</param>
        /// <param name="propertyValue">The generated property value</param>
        /// <returns>Whether or not a value could (or should) be generated for the given property</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Type cannot be inferred statically")]
        protected virtual bool TryGetStoreGeneratedValue(string entitySetName, string fullTypeName, string propertyName, out object propertyValue)
        {
            propertyValue = null;
            return false;
        }

        private static bool CompareETagValues(Dictionary<string, object> resourceCookieValues, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
        {
            if (concurrencyValues.Count() != resourceCookieValues.Count)
            {
                return false;
            }

            foreach (KeyValuePair<string, object> keyValuePair in concurrencyValues)
            {
                if (!resourceCookieValues.ContainsKey(keyValuePair.Key))
                {
                    return false;
                }
                else
                {
                    if (keyValuePair.Value == null)
                    {
                        return resourceCookieValues[keyValuePair.Key] == null;
                    }
                    else
                    {
                        if (!keyValuePair.Value.Equals(resourceCookieValues[keyValuePair.Key]))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private string GetResourceSetOfTargetResource(object targetResource)
        {
            string resourceSetName = null;
            foreach (string currentResourceSetName in this.ReflectionMetadataHelper.GetResourceSetNames())
            {
                if (this.GetResourceSetEntities(currentResourceSetName).Contains(targetResource))
                {
                    resourceSetName = currentResourceSetName;
                    break;
                }
            }

            return resourceSetName;
        }

        private void ValidateResourceType(object targetResource, string fullTypeName)
        {
            Type expectedType = this.ReflectionMetadataHelper.FindClrTypeByFullName(fullTypeName);
            ExceptionUtilities.Assert(
                expectedType.IsAssignableFrom(targetResource.GetType()),
                "Invalid uri specified. expected type: '{0}', actual type: '{1}'",
                fullTypeName,
                targetResource.GetType().FullName);
        }

        private Dictionary<string, object> GetConcurrencyValues(object targetResource)
        {
            Dictionary<string, object> etagValues = new Dictionary<string, object>();
            string[] etagProperties = this.ReflectionMetadataHelper.GetETagPropertiesOfType(GetResourceTypeFullName(targetResource.GetType()));

            foreach (string etagProperty in etagProperties)
            {
                etagValues.Add(etagProperty, targetResource.GetType().GetProperty(etagProperty).GetValue(targetResource, null));
            }

            return etagValues;
        }

        private UpdatableToken InstantiateResourceType(string fullTypeName)
        {
            Type t = this.ReflectionMetadataHelper.FindClrTypeByFullName(fullTypeName);
            ExceptionUtilities.ThrowDataServiceExceptionIfFalse(!t.IsAbstract, 400, "Cannot create resource because type \"{0}\" is abstract", t.FullName);

            var instance = Activator.CreateInstance(t);
            var token = new UpdatableToken(instance);
            foreach (var p in t.GetProperties().Where(p => p.CanWrite))
            {
                // make local variable so that lambdas below work
                var property = p;

                if (this.IsCollectionProperty(property))
                {
                    Type collectionType = this.GetCollectionPropertyType(GetResourceTypeFullName(t), property.Name);
                    if (collectionType != null)
                    {
                        var newCollection = Activator.CreateInstance(collectionType);
                        token.PendingPropertyUpdates[property.Name] = newCollection;
                        this.pendingChanges.Add(() => property.SetValue(instance, newCollection, null));
                    }
                }

                string entitySetName = this.ReflectionMetadataHelper.FindSetNameForType(t);
                object generatedValue;
                if (this.TryGetStoreGeneratedValue(entitySetName, fullTypeName, property.Name, out generatedValue))
                {
                    token.PendingPropertyUpdates[property.Name] = generatedValue;
                    this.pendingChanges.Add(() => property.SetValue(instance, generatedValue, null));
                }
            }

            return token;
        }

        /// <summary>
        /// ReflectionProvider was leaving around left over references after it a resource was being deleted. This will ensure the reference is removed from collections
        /// and reference properties
        /// </summary>
        /// <param name="targetResource">TargetResource To remove</param>
        private void DeleteAllReferences(object targetResource)
        {
            foreach (string currentSetName in this.ReflectionMetadataHelper.GetResourceSetNames())
            {
                IList entitySetList = this.GetResourceSetEntities(currentSetName);

                foreach (object currentEntityInstance in entitySetList)
                {
                    Type currentEntityType = currentEntityInstance.GetType();

                    foreach (var navigationProperty in this.ReflectionMetadataHelper.GetNavigationProperties(GetResourceTypeFullName(currentEntityType)))
                    {
                        if (navigationProperty.CollectionElementType != null)
                        {
                            this.RemoveResourceFromCollectionOnTargetResourceMatch(targetResource, navigationProperty, currentEntityInstance);
                        }
                        else
                        {
                            ExceptionUtilities.CheckObjectNotNull(navigationProperty.PropertyInfo, "Invalid navigation property info");
                            this.SetEntityReferenceToNullOnTargetResourceMatch(targetResource, navigationProperty, currentEntityInstance);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method is done as a cleanup, when a resource is deleted, this method will remove the deleted object from an entityInstance collection property
        /// </summary>
        /// <param name="targetResource">Resource to look for in the currentEntityInstance</param>
        /// <param name="navigationPropertyInfo">NavigationProperty to look in on the currentEntityInstance</param>
        /// <param name="currentEntityInstance">currentEntityInstance that may contain the targetResource</param>
        private void RemoveResourceFromCollectionOnTargetResourceMatch(object targetResource, NavigationPropertyInfo navigationPropertyInfo, object currentEntityInstance)
        {
            IEnumerable childCollectionObject = navigationPropertyInfo.PropertyInfo.GetValue(currentEntityInstance, null) as IEnumerable;
            if (childCollectionObject.Cast<object>().Any(o => o == targetResource))
            {
                MethodInfo removeMethod = navigationPropertyInfo.PropertyInfo.PropertyType.GetMethod("Remove");
                this.pendingChanges.Add(() => removeMethod.Invoke(childCollectionObject, new object[] { targetResource }));
            }
        }

        /// <summary>
        /// Method is done as a cleanup, when a resource is deleted, this method will remove the deleted object from an entityInstance reference property
        /// </summary>
        /// <param name="targetResource">Resource to look for in the currentEntityInstance</param>
        /// <param name="navigationPropertyInfo">NavigationProperty to look in on the currentEntityInstance</param>
        /// <param name="currentEntityInstance">currentEntityInstance that may contain the targetResource</param>
        private void SetEntityReferenceToNullOnTargetResourceMatch(object targetResource, NavigationPropertyInfo navigationPropertyInfo, object currentEntityInstance)
        {
            object childReferenceObject = navigationPropertyInfo.PropertyInfo.GetValue(currentEntityInstance, null);
            if (childReferenceObject == targetResource)
            {
                this.pendingChanges.Add(() => navigationPropertyInfo.PropertyInfo.SetValue(currentEntityInstance, null, null));
            }
        }
    }
}
