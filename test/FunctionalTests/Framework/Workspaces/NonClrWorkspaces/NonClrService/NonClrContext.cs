//---------------------------------------------------------------------
// <copyright file="NonClrContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OData.Service;
using System.Data.Common;
using DSP = Microsoft.OData.Service.Providers;
using System.ServiceModel.Web;
using System.Globalization;
using System.Data.Test.Astoria.CallOrder;

namespace System.Data.Test.Astoria.NonClr
{
    #region Backing Types

    public class RowComplexType
    {
        private readonly string typeName;
        public IDictionary<string, object> Properties { get; set; }
        public string TypeName { get { return this.typeName; } }

        public RowComplexType(string typeName)
        {
            if (String.IsNullOrEmpty(typeName)) throw new ArgumentNullException("typeName");
            this.typeName = typeName;
            this.Properties = new Dictionary<string, object>();
        }

        public object GetValue(string propertyName)
        {
            object propertyValue;
            if (this.Properties.TryGetValue(propertyName, out propertyValue))
                return propertyValue;
            else
                return null;
        }

        public T GetDefaultValue<T>()
        {
            return default(T);
        }
    }

    public class RowEntityType : RowComplexType
    {
        private string _entitySet;
        public RowEntityType(string entitySet, string typeName)
            : base(typeName)
        {
            _entitySet = entitySet;
        }
        internal string EntitySet
        {
            get { return _entitySet; }
        }
    }

    class SimpleBaseType : RowEntityType
    {
        public SimpleBaseType(string entitySet)
            : base(entitySet, "Aruba.NonClr.SimpleBaseType")
        { }

        public SimpleBaseType(string entitySet, string typeName) : base(entitySet, typeName)
        { }

        public int ID { get; set; }
        public string Name { get; set; }
    }

    class IntermediateType<T> : SimpleBaseType
    {
        public IntermediateType(string entitySet)
            : base(entitySet, "Aruba.NonClr.IntermediateType")
        { }

        public IntermediateType(string entitySet, string typeName) : base(entitySet, typeName)
        { }
    }

    class DerivedType : IntermediateType<DerivedType>
    {
        public DerivedType(string entitySet)
            : base(entitySet, "Aruba.NonClr.DerivedType")
        { }
    }

    #endregion

    public abstract class NonClrContext : DSP.IDataServiceUpdateProvider, DSP.IDataServiceMetadataProvider, DSP.IDataServiceQueryProvider
    {
        public const string ServerGeneratedCustomState = "ServerGenerated";

        internal static NonClrEntitySetDictionary _entitySetDictionary = new NonClrEntitySetDictionary();

        public List<DSP.ResourceSet> containers = null;
        public List<DSP.ResourceType> types = null;
        public List<DSP.ServiceOperation> operations = null;

        internal static List<DSP.ResourceType> underConstructionTypes = new List<Microsoft.OData.Service.Providers.ResourceType>();

        internal static List<DSP.ResourceSet> addedContainers = new List<DSP.ResourceSet>();
        internal static List<DSP.ResourceType> addedTypes = new List<DSP.ResourceType>();
        
        private Dictionary<string, DSP.ResourceAssociationSet> associations;

        public static List<ServiceOperationCreationParams> serviceOpCreateParams = null;

        public abstract void PopulateMetadata();
        private object _service;

        public NonClrContext(object service)
        {
            _service = service;
        }

        public NonClrEntitySetDictionary EntitySetDictionary
        {
            get
            {
                return _entitySetDictionary;
            }
        }

        internal object Service
        {
            get { return _service; }
        }

        private List<KeyValuePair<object, EntityState>> pendingChanges;
        internal List<KeyValuePair<object, EntityState>> PendingChanges
        {
            get
            {
                if (pendingChanges == null)
                {
                    pendingChanges = new List<KeyValuePair<object, EntityState>>();
                }

                return pendingChanges;
            }
        }

        public void RestoreData()
        {
            foreach (var list in EntitySetDictionary.Values)
                list.Clear();

            AddData(this);
        }

        #region IDataServiceMetadataProvider Members

        public string ContainerName
        {
            get
            {
                return typeof(NonClrContext).Name;
            }
        } // ContainerName

        public string ContainerNamespace
        {
            get
            {
                return typeof(NonClrContext).Namespace;
            }
        }// ContainerNamespace

        public IEnumerable<DSP.ResourceType> Types
        {
            get
            {
                PopulateMetadata();
                return types;
            }
        } // Types

        public IEnumerable<DSP.ResourceSet> ResourceSets
        {
            get
            {
                PopulateMetadata();
                return containers;
            }
        } // ResourceSets

        public IEnumerable<DSP.ServiceOperation> ServiceOperations
        {
            get
            {
                PopulateMetadata();
                return operations;
            }
        } // ServiceOperations

        public DSP.ResourceAssociationSet GetResourceAssociationSet(DSP.ResourceSet resourceSet, DSP.ResourceType resourceType, DSP.ResourceProperty resourceProperty)
        {
            if (associations == null)
                associations = new Dictionary<string, DSP.ResourceAssociationSet>();

            DSP.ResourceType targetType = resourceProperty.ResourceType;
            DSP.ResourceSet targetSet = null;

            var targetContainers = this.containers.Where(c => IsAssignableFrom(c.ResourceType, targetType));
            if (targetContainers.Count() == 1)
            {
                targetSet = targetContainers.First();
            }
            else if (targetContainers.Count() > 1)
            {
                // MEST (uses naming convention)
                if (resourceSet.CustomState != null)
                    targetSet = targetContainers.Single(c => (string)c.CustomState == (string)resourceSet.CustomState);
                else if (resourceProperty.CustomState != null)
                    targetSet = targetContainers.Single(c => (string)c.CustomState == (string)resourceProperty.CustomState);
                else
                    throw new DataServiceException(500, "Cannot infer association set for MEST scenario");
            }

            string associationName = resourceSet.Name + "_" + resourceType.Name + '_' + resourceProperty.Name;

            if (associations.Keys.Contains(associationName))
                return associations[associationName];
            else
            {
                DSP.ResourceProperty targetProperty = null;

                // Self links must be one-way
                if (resourceProperty.CustomState != null && (resourceSet != targetSet && resourceType != targetType))
                    targetProperty = targetType.Properties.SingleOrDefault(p => p.CustomState != null && (string)p.CustomState == (string)resourceProperty.CustomState);

                DSP.ResourceAssociationSetEnd sourceEnd = new DSP.ResourceAssociationSetEnd(resourceSet, resourceType, resourceProperty);
                DSP.ResourceAssociationSetEnd targetEnd = new DSP.ResourceAssociationSetEnd(targetSet, targetType, targetProperty);

                DSP.ResourceAssociationSet associationSet = new DSP.ResourceAssociationSet(associationName, sourceEnd, targetEnd);
                associations.Add(associationName, associationSet);

                // add to hash for target side
                if (targetProperty != null)
                {
                    associationName = targetSet.Name + "_" + targetType.Name + '_' + targetProperty.Name;
                    associations.Add(associationName, associationSet);
                }

                return associationSet;
            }
        } // GetResourceAssociationSet

        public bool TryResolveResourceSet(string name, out DSP.ResourceSet resourceSet)
        {
            if (this.ResourceSets != null)
            {
                resourceSet = this.ResourceSets.Where<DSP.ResourceSet>(c => c.Name == name).FirstOrDefault<DSP.ResourceSet>();
                return resourceSet != null;
            }

            resourceSet = null;
            return false;

        } // TryResolveResourceSet

        public bool TryResolveServiceOperation(string name, out DSP.ServiceOperation serviceOperation)
        {
            if (this.ServiceOperations != null)
            {
                serviceOperation = this.ServiceOperations.Where<DSP.ServiceOperation>(s => s.Name == name).FirstOrDefault<DSP.ServiceOperation>();
                return serviceOperation != null;
            }

            serviceOperation = null;
            return false;

        } // TryResolveServiceOperation

        public bool TryResolveResourceType(string name, out DSP.ResourceType resourceType)
        {
            if (types != null)
            {
                resourceType = this.Types.Where<DSP.ResourceType>(t => t.FullName == name).FirstOrDefault<DSP.ResourceType>();

                if (resourceType == null)
                {
                    int lastIndexOfPos = name.LastIndexOf(".");
                    if (lastIndexOfPos > -1)
                    {
                        string newTypeName = name.Substring(lastIndexOfPos, name.Length - lastIndexOfPos).Replace(".", "");
                        resourceType = this.Types.Where<DSP.ResourceType>(t => t.FullName == newTypeName).FirstOrDefault<DSP.ResourceType>();
                    }
                }
                return resourceType != null;
            }

            resourceType = null;
            return false;

        } // TryResolveResourceType

        public bool HasDerivedTypes(DSP.ResourceType resourceType)
        {
            var e = this.GetDerivedTypes(resourceType);
            return e != null && e.Count() > 0;
        } // HasDerivedTypes

        public IEnumerable<DSP.ResourceType> GetDerivedTypes(DSP.ResourceType resourceType)
        {
            foreach (DSP.ResourceType type in Types)
            {
                if (type.BaseType != null && IsDerivedType(resourceType, type))
                {
                    yield return type;
                }
            }
        } // GetDerivedTypes

        private static bool IsDerivedType(DSP.ResourceType baseType, DSP.ResourceType derivedType)
        {
            while (derivedType.BaseType != null)
            {
                if (derivedType.BaseType == baseType)
                {
                    return true;
                }
                derivedType = derivedType.BaseType;
            }

            return false;
        } // IsDerivedType

        #endregion

        #region IDataServiceQueryProvider Members

        public object CurrentDataSource
        {
            get;
            set;
        } // CurrentDataSource

        public IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target)
        {
            if (target == null)
                throw new DataServiceException(500, "GetOpenPropertyValues called on null target");

            DSP.ResourceType resourceType = null;
            RowComplexType complexType = target as RowComplexType;

            if (complexType == null)
                throw new DataServiceException(500, "GetOpenPropertyValues: Target is not of type RowComplexType");

            resourceType = this.Types.Where(r => r.FullName == complexType.TypeName).SingleOrDefault();

            if (resourceType == null)
                throw new DataServiceException(500, "Could not find resource type '" + complexType.TypeName + "'");

            // if its a closed type, this method should not be called
            if (!resourceType.IsOpenType)
                throw new DataServiceException(500, "GetOpenPropertyValues called on non open type '" + resourceType.FullName + "'");

            return complexType.Properties.Where(pair => !resourceType.Properties.Any(p => p.Name == pair.Key));
        } // GetOpenPropertyValues

        public object GetPropertyValue(object targetResource, DSP.ResourceProperty resourceProperty)
        {
            if (resourceProperty.Kind == DSP.ResourcePropertyKind.ResourceSetReference)
            {
                // there are 4 possibilites
                // 1) both the property and things in the list are strongly typed
                // 2) the property is strong, but the list is not
                // 3) the property is weak, but the things inside the list are strong
                // 4) neither is defined in clr terms

                // GetValue will automatically take care of #1 and #2, but we need to handle #3 here

                RowEntityType target = targetResource as RowEntityType;
                IEnumerable list = (IEnumerable)this.GetValue(targetResource, resourceProperty.Name);

                Type genericList = typeof(IEnumerable<>).MakeGenericType(resourceProperty.ResourceType.InstanceType);
                if (genericList.IsAssignableFrom(list.GetType()))
                    return list;

                MethodInfo castMethod = typeof(Enumerable).GetMethod("Cast", BindingFlags.Static | BindingFlags.Public);
                castMethod = castMethod.MakeGenericMethod(resourceProperty.ResourceType.InstanceType);
                return castMethod.Invoke(null, new object[] { list });
            }
            else
            {
                return this.GetValue(targetResource, resourceProperty.Name);
            }
        } // GetPropertyValue

        public object GetOpenPropertyValue(object target, string propertyName)
        {
            if (target == null)
                throw new DataServiceException(500, "GetOpenPropertyValue called on null target");

            DSP.ResourceType resourceType = null;
            RowComplexType complexType = target as RowComplexType;

            if (complexType == null)
                throw new DataServiceException(500, "GetOpenPropertyValue: Target is not of type RowComplexType");

            resourceType = this.Types.Where(r => r.FullName == complexType.TypeName).SingleOrDefault();

            if (resourceType == null)
                throw new DataServiceException(500, "Could not find resource type '" + complexType.TypeName + "'");

            // if its a closed type, this method should not be called
            if (!resourceType.IsOpenType)
                throw new DataServiceException(500, "GetOpenPropertyValue called on non open type '" + resourceType.FullName + "'");

            // if its a declared property, this method should not be called
            if (resourceType.Properties.Any(p => p.Name == propertyName))
                throw new DataServiceException(500, "GetOpenPropertyValue called for declared property '" + propertyName + "'");

            object value;
            if (complexType.Properties.TryGetValue(propertyName, out value))
                return value;

            return null;
        } // GetOpenPropertyValue

        public IQueryable GetQueryRootForResourceSet(DSP.ResourceSet container)
        {
            List<RowEntityType> list = EntitySetDictionary[container.Name];
            IQueryable<RowEntityType> realQueryable = list.AsQueryable();
            NonClrQueryProvider provider = new NonClrQueryProvider(realQueryable.Provider, this);
            NonClrQueryable<RowEntityType> queryable = new NonClrQueryable<RowEntityType>(realQueryable, provider);

            if (container.ResourceType.InstanceType == typeof(RowEntityType))
                return queryable;
            else
            {
                MethodInfo castMethod = typeof(Queryable).GetMethod("Cast", BindingFlags.Static | BindingFlags.Public);
                castMethod = castMethod.MakeGenericMethod(container.ResourceType.InstanceType);

                object newQueryable = castMethod.Invoke(null, new object[] { queryable });
                return newQueryable as IQueryable;
            }
        } // GetQueryRootForResourceSet

        public DSP.ResourceType GetResourceType(object instance)
        {
            if (instance == null)
                return null;

            DSP.ResourceType resourceType = null;
            RowComplexType complexType = instance as RowComplexType;

            if (complexType != null)
            {
                string typeName = complexType.TypeName;
                resourceType = this.Types.FirstOrDefault(r => r.FullName == typeName);
            }

            return resourceType;
        } // GetResourceType

        public bool IsNullPropagationRequired
        {
            get
            {
                return true;
            }
        } // IsNullPropagationRequired

        public object InvokeServiceOperation(DSP.ServiceOperation serviceOperation, object[] parameters)
        {
            MethodInfo methodInfo = _service.GetType().GetMethod(serviceOperation.Name);
            if (methodInfo == null)
            {
                throw new Exception(String.Format("Method with name '{0}' not found on the context instance", serviceOperation.Name));
            }

            return methodInfo.Invoke(_service, parameters);
        } // InvokeServiceOperation

        #endregion

        #region IDataServiceMetadataProvider Helper methods

        internal static bool IsAssignableFrom(DSP.ResourceType baseType, DSP.ResourceType derivedType)
        {
            while (derivedType != null)
            {
                if (derivedType == baseType)
                {
                    return true;
                }

                derivedType = derivedType.BaseType;
            }

            return false;
        }

        private static bool IEnumerableTypeFilter(Type m, object filterCriteria)
        {
            return m.IsGenericType && m.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        internal static Type GetGenericInterfaceElementType(Type type, TypeFilter typeFilter)
        {
            if (typeFilter(type, null))
            {
                return type.GetGenericArguments()[0];
            }

            Type[] queriables = type.FindInterfaces(typeFilter, null);
            if (queriables != null && queriables.Length == 1)
            {
                return queriables[0].GetGenericArguments()[0];
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region IUpdatable Members

        public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            AddReferenceToCollection_Internal(targetResource, propertyName, resourceToBeAdded, true);
        } // AddReferenceToCollection

        internal void AddReferenceToCollection_Internal(object targetResource, string propertyName, object resourceToBeAdded, bool addBackReference)
        {
            RowEntityType targetEntity = (RowEntityType)targetResource;
            RowEntityType entityResourceToBeAdded = (RowEntityType)resourceToBeAdded;

            IDictionary<string,object> targetProperties = targetEntity.Properties;

            object propertyValue;

            // if this is the first item that is getting added
            if (!targetProperties.TryGetValue(propertyName, out propertyValue))
            {
                targetProperties[propertyName] = new List<RowEntityType>() { entityResourceToBeAdded };
            }
            else
            {
                // Check to see if item already exists
                List<RowEntityType> collection = (List<RowEntityType>)targetProperties[propertyName];
                foreach (RowEntityType existingEntity in collection)
                    if (EqualKeys(existingEntity, entityResourceToBeAdded))
                        return;

                collection.Add(entityResourceToBeAdded);
            }

            if (addBackReference)
                SetReverseNavigation(targetEntity, entityResourceToBeAdded, propertyName, false);
        }

        // let properties with the same name collide... it won't hurt
        private static Dictionary<string, int> autoIncremementingProperties =
            new Dictionary<string, int>();

        public object CreateResource(string containerName, string fullTypeName)
        {
            object resource = null;

            DSP.ResourceType type = this.Types.FirstOrDefault(t => t.FullName == fullTypeName);
            if (type == null)
                throw new DataServiceException((int)Net.HttpStatusCode.BadRequest, "No type of name '" + fullTypeName + "' exists");

            if (type.IsAbstract)
                throw new DataServiceException((int)Net.HttpStatusCode.BadRequest, "Cannot create an instance of abstract type '" + fullTypeName + "'");

            if (containerName == null)
            {
                return new RowComplexType(fullTypeName);
            }
            else
            {
                DSP.ResourceSet container = containers.FirstOrDefault(rc => rc.Name == containerName);
                if (container == null)
                    throw new DataServiceException((int)Net.HttpStatusCode.BadRequest, "No container of name '" + containerName + "' exists");

                bool belongsInContainer = false;
                DSP.ResourceType typeForContainerCheck = type;
                while (typeForContainerCheck != null && !belongsInContainer)
                {
                    if (typeForContainerCheck == container.ResourceType)
                        belongsInContainer = true;
                    else
                        typeForContainerCheck = typeForContainerCheck.BaseType;
                }
                if (!belongsInContainer)
                    throw new DataServiceException((int)Net.HttpStatusCode.BadRequest,
                        "An entity of type '" + fullTypeName + "' cannot be added to '" + containerName + "'");

                if (type.InstanceType == typeof(RowEntityType))
                    resource = new RowEntityType(containerName, fullTypeName);
                else
                {
                    ConstructorInfo constructor = type.InstanceType.GetConstructor(new Type[] { typeof(string) });
                    if (constructor != null)
                        resource = constructor.Invoke(new object[] { containerName });
                    else
                    {
                        constructor = type.InstanceType.GetConstructor(new Type[] { });
                        resource = constructor.Invoke(new object[] { });
                    }
                }

                foreach (DSP.ResourceProperty property in type.Properties.Where(p => p.Kind == DSP.ResourcePropertyKind.ResourceSetReference))
                {
                    this.SetValue(resource, property.Name, new List<RowEntityType>());
                }

                foreach (DSP.ResourceProperty property in type.Properties.Where(p => ServerGeneratedCustomState.Equals(p.CustomState)))
                {
                    if (property.ResourceType.InstanceType != typeof(int))
                        throw new DataServiceException(500, "Auto incrementing is not supported for non-int properties");
                    int value;
                    if (!autoIncremementingProperties.TryGetValue(property.Name, out value))
                        value = 0;
                    this.SetValue(resource, property.Name, value);
                    autoIncremementingProperties[property.Name] = value + 1;
                }

                this.PendingChanges.Add(new KeyValuePair<object, EntityState>(resource, EntityState.Added));
                return resource;
            }
        } // CreateResource

        public void DeleteResource(object targetResource)
        {
            this.PendingChanges.Add(new KeyValuePair<object, EntityState>(targetResource, EntityState.Deleted));
        } // DeleteResource

        public object GetResource(IQueryable query, string fullTypeName)
        {
            object resource = null;
            foreach (object r in query)
            {
                if (resource != null)
                    throw new Exception("query returning more than one type");

                resource = r;
            }

            return resource;
        } // GetResource

        public object GetValue(object targetResource, string propertyName)
        {
            // Check for strongly typed properties
            PropertyInfo property = targetResource.GetType().GetProperty(propertyName);

            if (property != null)
                return property.GetValue(targetResource, null);

            RowComplexType complexType = (RowComplexType)targetResource;

            object propertyValue;
            if (complexType.Properties.TryGetValue(propertyName, out propertyValue))
                return propertyValue;
            else
                return null;
        } // GetValue

        public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            RemoveReferenceFromCollection_Internal(targetResource, propertyName, resourceToBeRemoved, true);
        } //RemoveReferenceFromCollection

        internal void RemoveReferenceFromCollection_Internal(object sourceResource, string propertyName, object resourceToBeRemoved, bool removeBackReference)
        {
            RowEntityType sourceEntity = sourceResource as RowEntityType;
            RowEntityType removedEntity = resourceToBeRemoved as RowEntityType;

            IList collection = (IList)sourceEntity.Properties[propertyName];
            collection.Remove(removedEntity);

            if (removeBackReference)
                SetReverseNavigation(sourceEntity, removedEntity, propertyName, true);
        }

        /// <summary>
        /// For the given association from source to target, perform the appropriate reversed action
        /// </summary>
        /// <param name="source">the source of the association to reverse</param>
        /// <param name="target">the target of the association to reverse</param>
        /// <param name="forwardPropertyName">the name of the property from source to target</param>
        /// <param name="remove">whether or not to remove the reversed association</param>
        private void SetReverseNavigation(RowEntityType source, RowEntityType target, string forwardPropertyName, bool remove)
        {
            DSP.ResourceType targetType;
            DSP.ResourceType sourceType = this.GetResourceType(source);

            if (target == null)
            {
                targetType = sourceType.Properties.Single(p => p.Name == forwardPropertyName).ResourceType;
            }
            else
            {
                targetType = this.GetResourceType(target);
            }

            DSP.ResourceProperty forwardProperty = sourceType.Properties.SingleOrDefault(p => p.Name == forwardPropertyName);

            if (forwardProperty != null && forwardProperty.CustomState != null)
            {
                // because sourceType could match targetType, we need to make sure we filter out the target property
                DSP.ResourceProperty reverseProperty = targetType.Properties
                    .SingleOrDefault(p => p != forwardProperty && p.CustomState != null && (string)p.CustomState == (string)forwardProperty.CustomState);

                if (reverseProperty != null)
                {
                    bool reference = (reverseProperty.Kind & DSP.ResourcePropertyKind.ResourceReference) != 0;
                    if(remove)
                    {
                        if(reference)
                            this.SetReference_Internal(target, reverseProperty.Name, null, false);
                        else
                            this.RemoveReferenceFromCollection_Internal(target, reverseProperty.Name, source, false);
                    }
                    else
                    {
                        if(reference)
                            this.SetReference_Internal(target, reverseProperty.Name, source, false);
                        else
                            this.AddReferenceToCollection_Internal(target, reverseProperty.Name, source, false);
                    }
                }
            }
        }

        public object ResetResource(object resource)
        {
            if (resource is RowEntityType)
            {
                RowEntityType entityResource = resource as RowEntityType;

                DSP.ResourceType type = Types.Where(t => t.Namespace + "." + t.Name == entityResource.TypeName).FirstOrDefault();

                foreach (DSP.ResourceProperty property in type.Properties)
                {
                    if ((property.Kind & DSP.ResourcePropertyKind.Key) == 0 &&
                        (property.Kind & DSP.ResourcePropertyKind.ResourceReference) == 0 &&
                        (property.Kind & DSP.ResourcePropertyKind.ResourceSetReference) == 0)
                    {
                        entityResource.Properties.Remove(property.Name);
                    }
                }

                // reset dynamic properties
                foreach (string key in entityResource.Properties.Keys.Except(type.Properties.Select(p => p.Name)).ToList())
                {
                    //the ToList is so we can modify the collection inside the loop
                    entityResource.Properties.Remove(key);
                }

                return entityResource;
            }
            else
            {
                return new RowComplexType(((RowComplexType)resource).TypeName);
            }
        } // ResetResource

        public object ResolveResource(object resource)
        {
            return resource;
        } // ResolveResource

        public void SaveChanges()
        {
            if (this.pendingChanges == null)
            {
                return;
            }

            foreach (KeyValuePair<object, EntityState> pendingChange in this.pendingChanges)
            {
                RowEntityType entity = pendingChange.Key as RowEntityType;
                List<RowEntityType> resourceList = EntitySetDictionary[entity.EntitySet];

                switch (pendingChange.Value)
                {
                    case EntityState.Added:
                        // Check to see if item already exists
                        foreach (RowEntityType existingEntity in resourceList)
                        {
                            if (EqualKeys(existingEntity, entity))
                                throw new DataServiceException(500, "An entity with the given key already exists");
                        }

                        resourceList.Add(entity);
                        break;

                    case EntityState.Deleted:
                        resourceList.Remove(entity);
                        break;

                    default:
                        throw new Exception("Unsupported State");
                }
            }

            this.pendingChanges.Clear();
        } // SaveChanges

        public void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            SetReference_Internal(targetResource, propertyName, propertyValue, true);
        } // SetReference

        internal void SetReference_Internal(object targetResource, string propertyName, object propertyValue, bool setBackReference)
        {
            RowEntityType targetEntity = (RowEntityType)targetResource;
            RowEntityType propertyEntity = (RowEntityType)propertyValue;

            object oldValue = null;
            if (!targetEntity.Properties.TryGetValue(propertyName, out oldValue))
                targetEntity.Properties.Add(propertyName, propertyEntity);
            else
                targetEntity.Properties[propertyName] = propertyEntity;

            if (setBackReference)
            {
                if (propertyEntity == null)
                    propertyEntity = (RowEntityType)oldValue;

                if(propertyEntity != null)
                    this.SetReverseNavigation(targetEntity, propertyEntity, propertyName, (propertyValue == null));
            }
        }

        public void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            RowComplexType targetComplex = targetResource as RowComplexType;
            targetComplex.Properties[propertyName] = propertyValue;
        } // SetValue

        public void ClearChanges()
        {
            if (null != this.pendingChanges)
            {
                this.pendingChanges.Clear();
            }
        } // ClearChanges

        #endregion

        #region IDataServiceUpdateProvider methods
        public void SetConcurrencyValues(object resource, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
        {
            if (!checkForEquality.HasValue)
            {
                throw new DataServiceException(417 /*ExpectationFailed*/, "IDataServiceUpdateProvider: Missing ETag for update operation");
            }

            if (!checkForEquality.Value)
                throw new DataServiceException(500, "checkForEquality must be true");

            foreach (var etagProperty in concurrencyValues)
            {
                object propertyValue = ((IUpdatable)this).GetValue(resource, etagProperty.Key);
                if ((propertyValue == null && etagProperty.Value == null) ||
                    propertyValue != null && ComparePropertyValue(propertyValue, etagProperty.Value))
                {
                    continue;
                }

                //throw new DataServiceException(412, String.Format("'{0}' property value did not match. Actual: '{1}', Expected: '{2}'", etagProperty.Key, etagProperty.Value, propertyValue));
                throw new DataServiceException(412, "The etag value in the request header does not match with the current etag value of the object.");
            }
        } // SetConcurrencyValues
        #endregion

        private bool ComparePropertyValue(object value1, object value2)
        {
            if (value1.GetType() == typeof(byte[]))
            {
                byte[] v1 = (byte[])value1;
                byte[] v2 = (byte[])value2;

                if (v1.Length != v2.Length)
                    return false;

                for (int i = 0; i < v1.Length; i++)
                {
                    if (!v1[i].Equals(v2[i]))
                    {
                        return false;
                    }
                }
            }
            else if (!value1.Equals(value2))
            {
                return false;
            }

            return true;
        }

        public void ResetMetadata()
        {
            addedTypes.Clear();
            addedContainers.Clear();
            underConstructionTypes.Clear();
            serviceOpCreateParams = null;
            _entitySetDictionary.Clear();
        }

        public void ResourceTypeComplete(string name)
        {
            addedTypes.Add(underConstructionTypes.Single(t => t.Name == name));
            underConstructionTypes.RemoveAll(t => t.Name == name);
        }

        public void AddResourceType(string name, string typeName, string resourceTypeKind, bool isOpenType, string baseType, string namespaceName, bool isAbstract)
        {
            DSP.ResourceType baseResourceType = null;

            if (baseType != null)
                baseResourceType = GetResourceType(baseType);

            Type backingType = Type.GetType("System.Data.Test.Astoria.NonClr." + typeName);

            DSP.ResourceType newType = new DSP.ResourceType(
                        backingType,
                        ConvertResourceTypeKind(resourceTypeKind),
                        baseResourceType,
                        namespaceName,
                        name,
                        isAbstract);
            newType.IsOpenType = isOpenType;

            underConstructionTypes.Add(newType);
        }

        public void RemoveResourceType(string name)
        {
            DSP.ResourceType rt = GetResourceType(name);

            if (rt != null)
                types.Remove(rt);
        }

        public void AddResourceSet(string name, string typeName)
        {
            DSP.ResourceType resourceType = GetResourceType(typeName);

            if (resourceType != null)
            {
                resourceType.SetReadOnly();
                SetDerivedReadOnly(resourceType);

                DSP.ResourceSet newResourceSet = new DSP.ResourceSet(name, resourceType);
                newResourceSet.SetReadOnly();
                addedContainers.Add(newResourceSet);
            }
        }

        private void SetDerivedReadOnly(DSP.ResourceType resourceType)
        {
            foreach (DSP.ResourceType rt in types)
            {
                if (rt.BaseType != null && rt.BaseType.Equals(resourceType))
                {
                    rt.SetReadOnly();
                    SetDerivedReadOnly(rt);
                }
            }
        }

        public void RemoveResourceSet(string name)
        {
            DSP.ResourceSet rc = GetResourceSet(name);

            if (rc != null)
                containers.Remove(rc);
        }

        public void AddResourceProperty(string propertyName, string addToResourceType, string resourcePropertyKind,
                                        string propertyType, string containerName, bool isClrProperty)
        {
            DSP.ResourceProperty newProperty = null;
            DSP.ResourceType resourceType = GetResourceType(addToResourceType);
            DSP.ResourceSet assocResourceSet = GetResourceSet(containerName);

            if (assocResourceSet != null)
            {
                DSP.ResourceType assocResourceType = GetResourceType(propertyType);

                newProperty = new DSP.ResourceProperty(
                                propertyName,
                                ConvertResourcePropertyKind(resourcePropertyKind),
                                assocResourceType);
            }
            else
            {
                DSP.ResourceType assocResourceType = GetResourceType(propertyType);
                if (assocResourceType == null)
                {
                    Type t = Type.GetType(propertyType);
                    assocResourceType = DSP.ResourceType.GetPrimitiveResourceType(t);
                }

                newProperty = new DSP.ResourceProperty(
                                propertyName,
                                ConvertResourcePropertyKind(resourcePropertyKind),
                                assocResourceType);
            }

            newProperty.CanReflectOnInstanceTypeProperty = isClrProperty;
            resourceType.AddProperty(newProperty);
        }

        public abstract void AddServiceOperation(string name, DSP.ServiceOperationResultKind kind, string typeName, string method);

        public void RemoveServiceOperation(string name)
        {
            foreach (DSP.ServiceOperation operation in operations)
            {
                if (operation.Name.ToLowerInvariant() == name.ToLowerInvariant())
                {
                    operations.Remove(operation);
                    return;
                }
            }
        }

        private DSP.ResourceType GetResourceType(string name)
        {
            foreach (DSP.ResourceType rt in types.Union(underConstructionTypes))
            {
                if (rt.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return rt;
            }

            return null;
        }

        private DSP.ResourceSet GetResourceSet(string name)
        {
            if (name == null)
                return null;

            foreach (DSP.ResourceSet rc in containers)
            {
                if (rc.Name.ToLowerInvariant() == name.ToLowerInvariant())
                    return rc;
            }

            return null;
        }

        private DSP.ResourceTypeKind ConvertResourceTypeKind(string resourceTypeKind)
        {
            switch (resourceTypeKind)
            {
                case "ComplexType": return DSP.ResourceTypeKind.ComplexType;
                case "EntityType": return DSP.ResourceTypeKind.EntityType;
                case "Primitive": return DSP.ResourceTypeKind.Primitive;
                default:
                    return DSP.ResourceTypeKind.EntityType;
            }
        }

        private DSP.ResourcePropertyKind ConvertResourcePropertyKind(string resourcePropertyKind)
        {
            switch (resourcePropertyKind)
            {
                case "ComplexType": return DSP.ResourcePropertyKind.ComplexType;
                case "Key": return DSP.ResourcePropertyKind.Key | DSP.ResourcePropertyKind.Primitive;
                case "Primitive": return DSP.ResourcePropertyKind.Primitive;
                case "ResourceReference": return DSP.ResourcePropertyKind.ResourceReference;
                case "ResourceSetReference": return DSP.ResourcePropertyKind.ResourceSetReference;
                default:
                    return DSP.ResourcePropertyKind.Primitive;
            }
        }

        public DSP.ServiceOperationResultKind ConvertServiceOperationResultKind(string serviceOperationResultKind)
        {
            switch (serviceOperationResultKind)
            {
                case "DirectValue": return DSP.ServiceOperationResultKind.DirectValue;
                case "Enumeration": return DSP.ServiceOperationResultKind.Enumeration;
                case "Nothing": return DSP.ServiceOperationResultKind.Void;
                case "QueryWithMultipleResults": return DSP.ServiceOperationResultKind.QueryWithMultipleResults;
                case "QueryWithSingleResult": return DSP.ServiceOperationResultKind.QueryWithSingleResult;
                default:
                    return DSP.ServiceOperationResultKind.Void;
            }
        }

        public bool EqualKeys(RowEntityType o, RowEntityType o2)
        {
            string[] keys = GetKeys(o);
            string[] keys2 = GetKeys(o2);
            if (keys.Count() == keys2.Count())
            {
                bool keysEqual = true;
                for (int i = 0; i < keys.Count(); i++)
                {
                    if (!keys[i].Equals(keys2[i]))
                    {
                        keysEqual = false;
                        break;
                    }
                }
                if (keysEqual)
                {
                    object[] oValues = GetPropertyValues(keys, o);
                    object[] o2Values = GetPropertyValues(keys, o2);
                    bool valuesEqual = true;
                    for (int i = 0; i < oValues.Length; i++)
                    {
                        if (!oValues[i].Equals(o2Values[i]))
                        {
                            valuesEqual = false;
                            break;
                        }
                    }
                    return valuesEqual;
                }
            }
            return false;
        }

        private string[] GetKeys(RowEntityType t)
        {
            List<string> keys = new List<string>();
            string simpleTypeName = t.TypeName.Substring(t.TypeName.LastIndexOf('.') + 1);
            DSP.ResourceType resourceType = types.Where(rt => rt.Name.Equals(simpleTypeName)).FirstOrDefault();
            keys.AddRange(resourceType.KeyProperties.OfType<DSP.ResourceProperty>().Select<DSP.ResourceProperty, string>(rp2 => rp2.Name));
            return keys.ToArray();
        }

        internal static void SetPropertyValues(string[] propertyNames, object[] values, object o)
        {
            for (int i = 0; i < propertyNames.Count(); i++)
            {
                o.GetType().InvokeMember(propertyNames[i], BindingFlags.SetProperty, null, o, new object[] { values[i] });
            }
        }

        public static object[] GetPropertyValues(string[] propertyNames, RowEntityType o)
        {
            List<object> propertyValues = new List<object>();
            foreach (string propertyName in propertyNames)
            {
                propertyValues.Add(o.Properties[propertyName]);
            }
            return propertyValues.ToArray();
        }

        private static void AddData(IUpdatable updatable)
        {
            // Do nothing, will be replaced
        } // AddData
    }

    public class LazyResourceType : DSP.ResourceType
    {
        private List<DSP.ResourceProperty> lazyProperties = new List<DSP.ResourceProperty>();

        public LazyResourceType(Type instanceType, DSP.ResourceTypeKind resourceTypeKind, DSP.ResourceType baseType, string namespaceName, string name, bool isAbstract)
            : base(instanceType, resourceTypeKind, baseType, namespaceName, name, isAbstract)
        {
        }
        
        public void AddLazyProperty(DSP.ResourceProperty property)
        {
            this.lazyProperties.Add(property);
        }

        protected override IEnumerable<DSP.ResourceProperty> LoadPropertiesDeclaredOnThisType()
        {
            APICallLog.Current.Add("LoadPropertiesDeclaredOnThisType", "Name", this.Name);
            try
            {
                return this.lazyProperties.AsEnumerable();
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }
    }

    public class ServiceOperationCreationParams
    {
        public ServiceOperationCreationParams(string name, DSP.ServiceOperationResultKind kind, string typeName, string method)
        {
            Name = name;
            Kind = kind;
            TypeName = typeName;
            Method = method;
        }

        public string Name
        {
            get;
            private set;
        }

        public string TypeName
        {
            get;
            private set;
        }

        public DSP.ServiceOperationResultKind Kind
        {
            get;
            private set;
        }

        public string Method
        {
            get;
            private set;
        }
    }
}
