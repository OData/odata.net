//---------------------------------------------------------------------
// <copyright file="OpenReflectionDataContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
#region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endregion

    public class OpenReflectionDataContext<T> : IDataServiceMetadataProvider, IDataServiceQueryProvider, IDataServiceUpdateProvider, IServiceProvider, IDataServicePagingProvider
    {
        private static readonly string ContainerNameValue = "OpenTypesContainer";
        private static readonly string NamespaceNameValue = "OpenTypesNamespace";
        private static readonly string InstancePropertyName = "InstanceType";

        public static List<string> ResourceTypeNames = new List<string>();
        public static string ComplexTypeName;
        public static EventHandler ValuesRequested;
       
        public static List<T> Values
        {
            get;
            set;
        }

        private List<ResourceType> rts = new List<ResourceType>();
        private ResourceSet rs;

        private string openTypesPropertyName;
        private int currentToken;
        private Dictionary<int, T> tokens;
        private List<KeyValuePair<object, EntityState>> pendingChanges;

        public static void Clear()
        {
            OpenReflectionDataContext<T>.ResourceTypeNames.Clear();
            OpenReflectionDataContext<T>.ComplexTypeName = null;
            OpenReflectionDataContext<T>.Values = null;
            OpenReflectionDataContext<T>.ValuesRequested = null;
        }

        #region Constructor

        public OpenReflectionDataContext()
        {
            ResourceType ct = null;
            if (OpenReflectionDataContext<T>.ComplexTypeName != null)
            {
                ct = new ResourceType(
                    typeof(T),
                    ResourceTypeKind.ComplexType,
                    null,
                    OpenReflectionDataContext<T>.NamespaceNameValue,
                    ComplexTypeName,
                    false);
                AddResourceTypeProperties(ct, ct);
                ct.CanReflectOnInstanceType = false;

                this.rts.Add(ct);
            }

            ResourceType rt = new ResourceType(
                                typeof(T),
                                ResourceTypeKind.EntityType,
                                null,
                                OpenReflectionDataContext<T>.NamespaceNameValue,
                                ResourceTypeNames[0],
                                false);
            AddResourceTypeProperties(rt, ct);
            rt.IsOpenType = true;
            rt.CanReflectOnInstanceType = false;

            this.rts.Add(rt);

            rs = new ResourceSet("Values", rt);

            foreach (string rtName in ResourceTypeNames.Skip(1))
            {
                ResourceType derivedRT = new ResourceType(
                            typeof(T),
                            ResourceTypeKind.EntityType,
                            rt,
                            OpenReflectionDataContext<T>.NamespaceNameValue,
                            rtName,
                            false);
                this.rts.Add(derivedRT);
            }

            this.rts.Act(x => x.SetReadOnly());
            rs.SetReadOnly();

            this.currentToken = 0;
            this.tokens = new Dictionary<int, T>();
            this.pendingChanges = new List<KeyValuePair<object, EntityState>>();
        }

        #endregion

        #region IDataServiceProvider Members

        public string ContainerName
        {
            get { return OpenReflectionDataContext<T>.ContainerNameValue; }
        }

        public string ContainerNamespace
        {
            get { return OpenReflectionDataContext<T>.NamespaceNameValue; }
        }

        public object CurrentDataSource
        {
            get
            {
                return this;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public void DisposeDataSource()
        {
        }

        public IEnumerable<ResourceType> GetDerivedTypes(ResourceType resourceType)
        {
            if (resourceType.Name == this.rts[0].Name)
            {
                return this.rts.Skip(1);
            }
            else
            {
                return Enumerable.Empty<ResourceType>();
            }
        }

        public object GetOpenPropertyValue(object target, string propertyName)
        {
            return this.GetPropertyValue(target, propertyName);
        }

        public IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target)
        {
            return (IDictionary<string, object>)target
                            .GetType()
                            .GetProperty(openTypesPropertyName, BindingFlags.Public | BindingFlags.Instance)
                            .GetValue(target, null);
        }

        public object GetPropertyValue(object target, ResourceProperty resourceProperty)
        {
            Assert.IsTrue(resourceProperty.CanReflectOnInstanceTypeProperty == false, "We should never call GetPropertyValue on reflectable properties");
            return this.GetPropertyValue(target, resourceProperty.Name);
        }

        public IQueryable GetQueryRootForResourceSet(ResourceSet resourceSet)
        {
            if (Values == null)
            {
                ValuesRequested(this, EventArgs.Empty);
            }

            IQueryable<T> queryable = Values.AsQueryable();

            return new OpenTypeQueryable<T>(queryable, new OpenTypeQueryProvider(queryable.Provider, this, InstancePropertyName));
        }

        public ResourceAssociationSet GetResourceAssociationSet(ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            throw new NotImplementedException();
        }

        public ResourceType GetResourceType(object target)
        {
            return this.rts.Single(rt => rt.FullName == (string)(typeof(T).GetProperty(InstancePropertyName).GetValue(target, new object[0])));
        }

        public bool HasDerivedTypes(ResourceType resourceType)
        {
            return (resourceType.FullName == this.rts[0].FullName);
        }

        public object InvokeServiceOperation(ServiceOperation serviceOperation, object[] parameters)
        {
            throw new NotImplementedException();
        }

        public bool IsNullPropagationRequired
        {
            get { return true; }
        }

        public IEnumerable<ResourceSet> ResourceSets
        {
            get { return new[] { this.rs }; }
        }

        public IEnumerable<ServiceOperation> ServiceOperations
        {
            get { return Enumerable.Empty<ServiceOperation>(); }
        }

        public bool TryResolveResourceSet(string name, out ResourceSet resourceSet)
        {
            resourceSet = this.rs;
            return true;
        }

        public bool TryResolveResourceType(string name, out ResourceType resourceType)
        {
            resourceType = this.rts.SingleOrDefault(rt => rt.FullName == name);
            return resourceType != null;
        }

        public bool TryResolveServiceOperation(string name, out ServiceOperation serviceOperation)
        {
            serviceOperation = null;
            return false;
        }

        public IEnumerable<ResourceType> Types
        {
            get { return this.rts; }
        }

        #endregion

        #region IUpdatable Members

        public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            throw new NotImplementedException();
        }

        public void ClearChanges()
        {
            this.pendingChanges.Clear();
        }

        public object CreateResource(string containerName, string fullTypeName)
        {
            ResourceType instanceType = this.rts.Single(rt => rt.FullName == fullTypeName);
            T instance = (T)Activator.CreateInstance(typeof(T));
            typeof(T).GetProperty(InstancePropertyName).SetValue(instance, instanceType.FullName, new object [0]);
            
            int token = this.currentToken;
            this.tokens.Add(this.currentToken++, instance);
            if (instanceType.ResourceTypeKind == ResourceTypeKind.EntityType)
            {
                this.pendingChanges.Add(new KeyValuePair<object, EntityState>(instance, EntityState.Added));
            }

            return token;
        }

        public void DeleteResource(object targetResource)
        {
            object objectToDelete = this.tokens[(int)targetResource];
            this.pendingChanges.Add(new KeyValuePair<object, EntityState>(objectToDelete, EntityState.Deleted));
        }

        public object GetResource(IQueryable query, string fullTypeName)
        {
            foreach (object r in query)
            {
                int token = this.currentToken;
                this.tokens.Add(this.currentToken++, (T)r);
                return token;
            }

            return null;
        }

        public object GetValue(object targetResource, string propertyName)
        {
            return this.GetPropertyValue(this.tokens[(int)targetResource], propertyName);
        }

        public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            throw new NotImplementedException();
        }

        public object ResetResource(object resource)
        {
            //T instance = this.tokens[(int)resource];
            //string instanceType = (string)typeof(T).GetProperty(InstancePropertyName).GetValue(instance, new object[0]);
            //T newInstance = (T)Activator.CreateInstance(typeof(T));
            //typeof(T).GetProperty(InstancePropertyName).SetValue(newInstance, instanceType, new object[0]);
            throw new NotImplementedException();
        }

        public object ResolveResource(object resource)
        {
            return this.tokens[(int)resource];
        }

        public void SaveChanges()
        {
            if (Values == null)
            {
                ValuesRequested(this, EventArgs.Empty);
            }

            foreach (KeyValuePair<object, EntityState> kvp in this.pendingChanges)
            {
                switch (kvp.Value)
                {
                    case EntityState.Added:
                        Values.Add((T)kvp.Key);
                        break;

                    case EntityState.Deleted:
                        Values.Remove((T)kvp.Key);
                        break;
                }
            }

            this.pendingChanges.Clear();
        }

        public void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            throw new NotImplementedException();
        }

        public void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            T instance = this.tokens[(int)targetResource];
            ResourceType resourceType = this.GetResourceType(instance);
            if (resourceType != null)
            {
                ResourceProperty resourceProperty = resourceType.Properties.SingleOrDefault(rp => rp.Name == propertyName);
                if (resourceProperty != null && resourceProperty.Kind == ResourcePropertyKind.Collection)
                {
                    CollectionResourceType collectionType = resourceProperty.ResourceType as CollectionResourceType;
                    IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(collectionType.ItemType.InstanceType));
                    foreach (var i in (IEnumerable)propertyValue)
                    {
                        object itemValue = i;
                        if (collectionType.ItemType.ResourceTypeKind == ResourceTypeKind.ComplexType)
                        {
                            itemValue = this.tokens[(int)itemValue];
                        }

                        list.Add(itemValue);
                    }
                    propertyValue = list;
                }
            }
            this.SetPropertyValue(instance, propertyName, propertyValue);
        }

        public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
        {
            throw new NotImplementedException();
        }

        #endregion

        private void AddResourceTypeProperties(ResourceType rt, ResourceType complexType)
        {
            foreach (PropertyInfo pi in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (typeof(IDictionary<string, object>).IsAssignableFrom(pi.PropertyType) && rt.ResourceTypeKind == ResourceTypeKind.EntityType)
                {
                    this.openTypesPropertyName = pi.Name;
                    continue;
                }

                // This property is only useful for type information.
                if (pi.Name == InstancePropertyName)
                {
                    continue;
                }

                Type elementType = TypeUtils.GetElementType(pi.PropertyType);
                if (elementType != null && pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    ResourceType itemType = null;
                    if (elementType == typeof(T))
                    {
                        itemType = complexType;
                    }
                    else
                    {
                        itemType = ResourceType.GetPrimitiveResourceType(elementType);
                    }

                    // Ignore properties we can't declare due to unknown types
                    if (itemType == null)
                    {
                        continue;
                    }

                    ResourceProperty rp = new ResourceProperty(
                        pi.Name,
                        ResourcePropertyKind.Collection,
                        ResourceType.GetCollectionResourceType(itemType));
                    rp.CanReflectOnInstanceTypeProperty = true;
                    rt.AddProperty(rp);
                }

                ResourcePropertyKind rpk = ResourcePropertyKind.Primitive;

                if (pi.Name.EndsWith("ID") && rt.ResourceTypeKind == ResourceTypeKind.EntityType)
                {
                    rpk |= ResourcePropertyKind.Key;
                }

                ResourceType primitiveType = ResourceType.GetPrimitiveResourceType(pi.PropertyType);

                if (primitiveType != null)
                {
                    ResourceProperty rp = new ResourceProperty(
                            pi.Name,
                            rpk,
                            ResourceType.GetPrimitiveResourceType(pi.PropertyType));

                    rp.CanReflectOnInstanceTypeProperty = true;
                    rt.AddProperty(rp);
                }
            }
        }

        private object GetPropertyValue(object targetResource, string propertyName)
        {
            PropertyInfo property = targetResource.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
            {
                IDictionary<string, object> openProperties = (IDictionary<string, object>)targetResource
                                .GetType()
                                .GetProperty(openTypesPropertyName, BindingFlags.Public | BindingFlags.Instance)
                                .GetValue(targetResource, null);

                object result;
                if (openProperties.TryGetValue(propertyName, out result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return property.GetValue(targetResource, null);
            }
        }

        private void SetPropertyValue(object targetResource, string propertyName, object propertyValue)
        {
            PropertyInfo property = targetResource.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
            {
                IDictionary<string, object> openProperties = (IDictionary<string, object>)targetResource
                                .GetType()
                                .GetProperty(openTypesPropertyName, BindingFlags.Public | BindingFlags.Instance)
                                .GetValue(targetResource, null);

                object result;
                if (openProperties.TryGetValue(propertyName, out result))
                {
                    openProperties.Remove(propertyName);
                }

                openProperties.Add(propertyName, propertyValue);
            }
            else
            {
                property.SetValue(targetResource, propertyValue, new object[0]);
            }
        }

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IDataServiceQueryProvider) ||
                serviceType == typeof(IDataServiceMetadataProvider) ||
                serviceType == typeof(IUpdatable))
            {
                return this;
            }
            else if (serviceType == typeof(IDataServicePagingProvider))
            { 
                if (OpenTypeQueryProvider.PagingStrategy != PagingStrategy.None)
                {
                    return this;
                }
            }        
            
            return null;
        }

        #endregion

        #region IDataServicePagingProvider Members

        public object[] GetContinuationToken(IEnumerator e)
        {
            if (OpenTypeQueryProvider.PagingStrategy == PagingStrategy.FixedPageSize)
            {
                object lastObject = (e as BaseEnumerator).LastObject;
                if (lastObject == null)
                {
                    return null;
                }

                List<object> skipToken = new List<object>();
                foreach (ResourceProperty rp in GetResourceType(lastObject).KeyProperties)
                {
                    skipToken.Add(GetPropertyValue(lastObject, rp.Name));
                }
                
                return skipToken.ToArray();
            }
            else
            {
                return null;
            }
        }

        public void SetContinuationToken(IQueryable query, ResourceType resourceType, object[] continuationToken)
        {
            OpenTypeQueryProvider openQueryProvider = query.Provider as OpenTypeQueryProvider;
            openQueryProvider.ApplyContinuationToken(query, resourceType, continuationToken);
        }

        #endregion
    }

    public static class Extensions
    {
        public static void Act<T>(this IEnumerable<T> seq, Action<T> act)
        {
            foreach (T item in seq)
            {
                act(item);
            }
        }
    }
}
