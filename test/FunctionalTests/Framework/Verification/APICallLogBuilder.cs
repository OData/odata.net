//---------------------------------------------------------------------
// <copyright file="APICallLogBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using Microsoft.OData.Service;
using Microsoft.OData.Service.Providers;
using System.Data.Test.Astoria.CallOrder;
using System.Data.Test.Astoria.Util;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml;
using AstoriaUnitTests.Data;

namespace System.Data.Test.Astoria
{
    public class APICallLogBuilder
    {
        public APICallLogBuilder(Workspace workspace)
        {
            this.Workspace = workspace;

            this.Updatable = new IUpdatableCallLogBuilder(this);
            this.ServiceProvider = new IServiceProviderCallLogBuilder(this);
            this.UpdateProvider = new IDataServiceUpdateProviderCallLogBuilder(this);
            this.MetadataProvider = new IDataServiceMetadataProviderCallLogBuilder(this);
            this.QueryProvider = new IDataServiceQueryProviderCallLogBuilder(this);
            this.StreamProvider = new IDataServiceStreamProviderCallLogBuilder(this);
            this.StreamProvider2 = new IDataServiceStreamProvider2CallLogBuilder(this);
            this.Stream = new StreamCallLogBuilder(this);
            this.DataService = new DataServiceAPICallLogBuilder(this);
            this.ProcessingPipeline = new DataServiceProcessingPipelineCallLogBuilder(this);
            this.PagingProvider = new IDataServicePagingProviderCallLogBuilder(this);
            this.ExpandProvider = new IExpandProviderCallLogBuilder(this);

            this.Entries = new List<APICallLogEntry>();
            this.CurrentFormat = null;
        }

        public List<APICallLogEntry> Entries
        {
            get;
            private set;
        }

        internal Workspace Workspace
        {
            get;
            private set;
        }

        public SerializationFormatKind? CurrentFormat
        {
            get;
            set;
        }

        #region interfaces
        public IUpdatableCallLogBuilder Updatable
        {
            get;
            private set;
        }

        public IExpandProviderCallLogBuilder ExpandProvider
        {
            get;
            private set;
        }

        public IServiceProviderCallLogBuilder ServiceProvider
        {
            get;
            private set;
        }

        public IDataServiceUpdateProviderCallLogBuilder UpdateProvider
        {
            get;
            private set;
        }

        public IDataServiceMetadataProviderCallLogBuilder MetadataProvider
        {
            get;
            private set;
        }

        public IDataServiceQueryProviderCallLogBuilder QueryProvider
        {
            get;
            private set;
        }

        public IDataServicePagingProviderCallLogBuilder PagingProvider
        {
            get;
            private set;
        }

        public IDataServiceStreamProviderCallLogBuilder StreamProvider
        {
            get;
            private set;
        }

        public IDataServiceStreamProvider2CallLogBuilder StreamProvider2
        {
            get;
            private set;
        }

        public StreamCallLogBuilder Stream
        {
            get;
            private set;
        }

        public DataServiceAPICallLogBuilder DataService
        {
            get;
            private set;
        }

        public DataServiceProcessingPipelineCallLogBuilder ProcessingPipeline
        {
            get;
            private set;
        }
        #endregion

        internal void Add(APICallLogEntry entry)
        {
            Entries.Add(entry);
        }

        public void Add(string methodName, params string[] arguments)
        {
            Add(new APICallLogEntry(methodName, arguments));
        }

        internal void Add(MethodInfo method, params string[] argumentValues)
        {
            List<KeyValuePair<string, string>> arguments = new List<KeyValuePair<string, string>>();
            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length < argumentValues.Length)
                throw new ArgumentException("Too many argument values specified");

            for (int i = 0; i < argumentValues.Length; i++)
            {
                arguments.Add(new KeyValuePair<string, string>(parameters[i].Name, argumentValues[i]));
            }

            Add(new APICallLogEntry(method.DeclaringType.Name + "." + method.Name, arguments));
        }

        public void InitializeProvider()
        {
            MetadataProvider.GetService();
            if (MetadataProvider.ServiceHasInterface)
            {
                QueryProvider.GetService();
            }

            if (QueryProvider.ServiceHasInterface)
            {
                QueryProvider.GetCurrentDataSource();
                DataService.CreateDataSource();
                QueryProvider.SetCurrentDataSource(Workspace.ContextNamespace + "." + Workspace.ContextTypeName);
            }
            else
            {
                DataService.CreateDataSource();
            }
        }

        public void Insert(ComplexResourceInstance instance)
        {
            if (instance is KeyedResourceInstance)
            {
                this.Updatable.CreateResource(instance as KeyedResourceInstance);
            }
            else
            {
                this.Updatable.CreateResource(instance);
            }

            Update(instance);
        }

        public void Update(ComplexResourceInstance instance)
        {
            ComplexType type;
            IEnumerable<ResourceInstanceProperty> properties;
            if (instance is KeyedResourceInstance)
            {
                type = Workspace.ServiceContainer.ResourceTypes.Single(rt => rt.Name == instance.TypeName);
                KeyedResourceInstance keyedInstance = instance as KeyedResourceInstance;
                properties = keyedInstance.KeyProperties.Union(keyedInstance.Properties);
            }
            else
            {
                type = Workspace.ServiceContainer.ComplexTypes.Single(rt => rt.Name == instance.TypeName);
                properties = instance.Properties;
            }

            if (instance is KeyedResourceInstance)
            {
                var typeCache = new HashSet<string>();

                // Logic below will not work when entities are being inserted that have more than one type of complex type
                // There is multiple caches, one at the Data Service Metadata Provider and another in EdmLib
                foreach (var property in properties.OfType<ResourceInstanceComplexProperty>())
                {
                    var subInstance = property.ComplexResourceInstance;
                    if (subInstance != null)
                    {
                        // if its a dynamic property, resolve
                        if (!type.Properties.Any(p => p.Name == property.Name && p.Facets.IsDeclaredProperty))
                        {
                            var typeName = Workspace.ContextNamespace + "." + subInstance.TypeName;
                            this.MetadataProvider.TryResolveResourceType(typeName);
                        }
                    }
                }
            }

            // all properties from content section
            foreach (ResourceInstanceProperty property in properties.Where(p => !(p is ResourceInstanceNavProperty)))
            {
                Update(type, instance, property);
            }

            foreach (ResourceInstanceProperty property in properties.OfType<ResourceInstanceNavProperty>())
            {
                Update(type, instance, property);
            }
        }

        public void ResolveLink(AssociationResourceInstance assoc)
        {
            ResourceContainer container = this.Workspace.ServiceContainer.ResourceContainers[assoc.ResourceSetName];

            if (this.Workspace.IsDebugProductBuild())
            {
                this.CheckSetForVersioning(container);
            }
            
            this.MetadataProvider.TryResolveServiceOperation(assoc.ResourceSetName);
            this.MetadataProvider.TryResolveResourceSet(assoc.ResourceSetName);
                        
            this.QueryProvider.GetQueryRootForResourceSet(assoc.ResourceSetName);

            this.DataService.DefaultQueryInterceptor(container);

            this.Updatable.GetResource(container, null);
        }
        
        public void Update(ComplexType type, ComplexResourceInstance instance, ResourceInstanceProperty property)
        {
            if (property is ResourceInstanceSimpleProperty)
            {
                this.Updatable.SetValue(instance, property);
            }
            else if (property is ResourceInstanceComplexProperty)
            {
                ComplexResourceInstance subInstance = ((ResourceInstanceComplexProperty)property).ComplexResourceInstance;
                if (subInstance != null)
                {
                    if (!type.Properties.Any(p => p.Name == property.Name && p.Facets.IsDeclaredProperty))
                    {
                        // if its a dynamic property, resolve
                        this.MetadataProvider.TryResolveResourceType(Workspace.ContextNamespace + "." + subInstance.TypeName);
                    }

                    Insert(subInstance);
                }
                
                this.Updatable.SetValue(instance, property);
            }
            else if (property is ResourceInstanceNavColProperty)
            {
                this.MetadataProvider.GetResourceAssociationSet((KeyedResourceInstance)instance, property);

                ResourceInstanceNavColProperty collection = property as ResourceInstanceNavColProperty;
                foreach (ResourceBodyTree thing in collection.Collection.NodeList)
                {
                    if (thing is AssociationResourceInstance)
                    {                    
                        AssociationResourceInstance assoc = thing as AssociationResourceInstance;
                        ResolveLink(assoc);

                        if (assoc.Operation == AssociationOperation.Add)
                        {
                            this.Updatable.AddReferenceToCollection(instance, property.Name, assoc);
                        }
                        else
                        {
                            this.Updatable.RemoveReferenceFromCollection(instance, property.Name, assoc);
                        }
                    }
                    else if (thing is KeyedResourceInstance)
                    {
                        KeyedResourceInstance deepInsert = thing as KeyedResourceInstance;
                        ResourceContainer deepContainer = this.Workspace.ServiceContainer.ResourceContainers[deepInsert.ResourceSetName];
                        if (this.Workspace.IsDebugProductBuild())
                        {
                            this.CheckSetForVersioning(deepContainer);
                        }

                        Insert(deepInsert);

                        this.Updatable.AddReferenceToCollection(instance, property.Name, deepInsert);
                    }
                }
            }
            else if (property is ResourceInstanceNavRefProperty)
            {
                this.MetadataProvider.GetResourceAssociationSet((KeyedResourceInstance)instance, property);

                ResourceInstanceNavRefProperty reference = property as ResourceInstanceNavRefProperty;
                if (reference.TreeNode is AssociationResourceInstance)
                {
                    AssociationResourceInstance assoc = reference.TreeNode as AssociationResourceInstance;
                    ResolveLink(assoc);

                    if (assoc.Operation == AssociationOperation.Add)
                    {
                        this.Updatable.SetReference(instance, property.Name, assoc);
                    }
                    else
                    {
                        this.Updatable.SetReferenceToNull(instance, property.Name);
                    }
                }
                else if (reference.TreeNode is KeyedResourceInstance)
                {
                    KeyedResourceInstance deepInsert = reference.TreeNode as KeyedResourceInstance;
                    ResourceContainer deepContainer = this.Workspace.ServiceContainer.ResourceContainers[deepInsert.ResourceSetName];
                    if (this.Workspace.IsDebugProductBuild())
                    {
                        this.CheckSetForVersioning(deepContainer);
                    }

                    Insert(deepInsert);

                    this.Updatable.SetReference(instance, property.Name, deepInsert);
                }
            }
        }

        public void SerializeProperty(ComplexType type, NodeProperty property)
        {
            this.QueryProvider.GetPropertyValue(type, property);

            if (property.Type is ComplexType)
                SerializeComplexType(property.Type as ComplexType);
        }

        public void SerializeComplexType(ComplexType type)
        {
            this.QueryProvider.GetResourceType(type);

            foreach (NodeProperty property in type.Properties)
            {
                SerializeProperty(type, property);
            }
        }

        public void SerializeEntity(ResourceContainer container, PayloadObject entity, bool inSet, AstoriaRequest request)
        {
            SerializeEntity(container, entity, inSet, false, request, false);
        }

        public void SerializeEntity(ResourceContainer container, PayloadObject entity, bool inSet, bool inExpand, AstoriaRequest request, bool scopedProjection)
        {
            ResourceType type = container.ResourceTypes.SingleOrDefault(rt => rt.FullName == entity.Type);
            if (type == null)
            {
                if (container.ResourceTypes.Count() == 1)
                    type = container.BaseType;
                else
                    AstoriaTestLog.FailAndThrow("Could not infer resource type for entity from set '" + container.Name + "' with type '" + entity.Type + "'");
            }

            if (Versioning.Server.SupportsLiveFeatures)
            {
                this.SerializeEntity(container, type, entity, inSet, inExpand, request, scopedProjection);
            }
            else
            {
                this.SerializeEntityV2(container, type, entity, inSet, inExpand, request, scopedProjection);
            }
        }

        private void SerializeEntityV2(ResourceContainer container, ResourceType type, PayloadObject entity, bool inSet, bool inExpand, AstoriaRequest request, bool scopedProjection)
        {
            if (request.Format == SerializationFormatKind.JSON && (!inExpand || inSet))
            {
                // json does this an extra time
                this.QueryProvider.GetResourceType(type);
            }

            this.QueryProvider.GetResourceType(type);
            foreach (ResourceProperty keyProperty in type.Properties.Where(p => p.PrimaryKey != null).OrderBy(p => p.Name))
            {
                this.QueryProvider.GetPropertyValue(type, keyProperty);
            }

            this.QueryProvider.GetResourceType(type);

            if (request.Format != SerializationFormatKind.JSON)
            {
                SerializeBlobMetadata(type, request);
            }

            if (inSet || inExpand)
            {
                this.QueryProvider.GetResourceType(type);
                foreach (NodeProperty p in type.Properties.Where(p => p.Facets.ConcurrencyModeFixed))
                {
                    this.QueryProvider.GetPropertyValue(type, p);
                }
            }
            else
            {
                // if not in a set, then the etag was already built for the header for types with etags
                // however, an extra get type will occur here for types that don't have etags
                if (!type.Properties.Any(p => p.Facets.ConcurrencyModeFixed))
                    this.QueryProvider.GetResourceType(type);
            }

            // TODO: put back the V2-style serialization?
            //SerializeEntity(container, entity, request, scopedProjection, type);
        }

        private void SerializeBlobMetadata(ResourceType type, AstoriaRequest request)
        {
            if (type.Facets.HasStream)
            {
                this.StreamProvider.GetService();
                this.StreamProvider.GetStreamETag(type, request);
                this.StreamProvider.GetReadStreamUri(type, request);
                this.StreamProvider.GetStreamContentType(type, request);
            }
        }

        private void SerializeEntity(ResourceContainer container, ResourceType type, PayloadObject entity, bool inSet, bool inExpand, AstoriaRequest request, bool scopedProjection)
        {
            this.QueryProvider.GetResourceType(type);

            if (container.Workspace.IsDebugProductBuild())
            {
                this.QueryProvider.GetResourceType(type);
            }

            foreach (ResourceProperty keyProperty in type.Properties.Where(p => p.PrimaryKey != null).OrderBy(p => p.Name))
            {
                this.QueryProvider.GetPropertyValue(type, keyProperty);
            }

            this.CheckSetForVersioning(container);

            SerializeBlobMetadata(type, request);

            if (inSet || inExpand)
            {
                foreach (NodeProperty p in type.Properties.Where(p => p.Facets.ConcurrencyModeFixed))
                {
                    this.QueryProvider.GetPropertyValue(type, p);
                }
            }

            if (!scopedProjection)
            {
                // product will try to get an action provider, but none will be returned by the test-framework-generated service
                if (this.ServiceProvider.GetService(typeof(IDataServiceActionProvider)))
                {
                    // as always, this is to check for whether the data source implements the interface
                    // but will only happen the first time the interface is looked up
                    this.QueryProvider.GetCurrentDataSource();
                }
            }

            SerializeEntityProperties(container, entity, request, scopedProjection, type);
        }

        private void SerializeEntityProperties(ResourceContainer container, PayloadObject entity, AstoriaRequest request, bool scopedProjection, ResourceType type)
        {
            foreach (ResourceProperty property in type.Properties.OfType<ResourceProperty>().Where(p => p.IsNavigation && p.Facets.IsDeclaredProperty))
            {
                this.MetadataProvider.GetResourceAssociationSet(container, container.BaseType, property);

                PayloadObject e = entity.PayloadObjects.SingleOrDefault(o => o.Name == property.Name);
                bool wasNull = e == null;

                if (request.Format == SerializationFormatKind.JSON)
                {
                    if (wasNull && property.OtherAssociationEnd.Multiplicity != Multiplicity.Many)
                    {
                        if (entity.PayloadProperties.Any(p => p.Name == property.Name && p.IsNull))
                        {
                            e = new PayloadObject(entity.Payload) { Reference = true };
                        }
                    }
                }

                if (e == null || e.Deferred)
                {
                    continue;
                }

                if (this.ExpandProvider.ServiceHasInterface)
                {
                    if (property.Facets.IsClrProperty)
                    {
                        this.QueryProvider.GetResourceType(type);
                    }
                    else
                    {
                        this.QueryProvider.GetPropertyValue(type, property);
                    }
                }

                ResourceContainer otherContainer = container.FindDefaultRelatedContainer(property);

                if (e.Reference)
                {
                    if (e.Type == null)
                    {
                        continue;
                    }

                    this.SerializeEntity(otherContainer, e, false, true, request, false);
                }
                else
                {
                    foreach (PayloadObject expandedEntity in e.PayloadObjects)
                    {
                        this.SerializeEntity(otherContainer, expandedEntity, true, true, request, false);
                    }

                    this.PagingProvider.GetContinuationToken(otherContainer);
                }
            }

            // this is partially payload driven to make dealing with projections easier
            if (type.Facets.NamedStreams.Any() && (!request.URI.Contains("$select") || entity.NamedStreams.Any()))
            {
                this.StreamProvider2.GetService();
                foreach (string streamName in entity.NamedStreams.Select(s => s.Name))
                {
                    this.StreamProvider2.GetStreamETag(type, streamName, request);
                    this.StreamProvider2.GetReadStreamUri(type, streamName, request);
                    this.StreamProvider2.GetStreamContentType(type, streamName, request);
                }
            }

            Queue<ComplexType> complexTypesToSerialize = new Queue<ComplexType>();
            foreach (ResourceProperty property in type.Properties.OfType<ResourceProperty>().Where(p => !p.IsNavigation && p.Facets.IsDeclaredProperty))
            {
                bool included = true;
                if (scopedProjection)
                {
                    included = entity.PayloadProperties.Any(p => p.Name == property.Name);
                }

                if (included)
                {
                    this.SerializeProperty(type, property);
                }
            }

            if (type.Facets.IsOpenType && !scopedProjection)
            {
                this.QueryProvider.GetOpenPropertyValues(type);
            }

            if (type.Facets.IsOpenType && !(Workspace is OpenTypesWorkspace))
            {
                // assume complex non-declared properties have values
                foreach (ResourceProperty dynamicComplex in type.Properties.OfType<ResourceProperty>().Where(p => !p.Facets.IsDeclaredProperty && p.IsComplexType))
                {
                    if (entity.PayloadProperties.Any(p => p.Name == dynamicComplex.Name))
                    {
                        ComplexType complexType = dynamicComplex.Type as ComplexType;
                        QueryProvider.GetResourceType(complexType);
                        this.SerializeComplexType(complexType);
                    }
                }
            }
        }

        public void CheckETag(KeyExpression key)
        {
            if (key.ResourceType.Properties.Any(p => p.Facets.ConcurrencyModeFixed))
            {
                if (Workspace.Settings.UpdatableImplementation == UpdatableImplementation.DataServiceUpdateProvider)
                {
                    UpdateProvider.SetConcurrencyValues(key.ResourceType, true,
                        key.ResourceType.Properties
                        .Where(p => p.Facets.ConcurrencyModeFixed)
                        .Select(p => new KeyValuePair<string, object>(p.Name, key.KnownPropertyValues[p.Name])));
                }
                else if (Workspace.Settings.UpdatableImplementation == UpdatableImplementation.IUpdatable)
                {
                    foreach (NodeProperty p in key.ResourceType.Properties.Where(p => p.Facets.ConcurrencyModeFixed))
                        Updatable.GetValue(key.ResourceType, p.Name);
                }
            }
        }

        private HashSet<ResourceContainer> setsCheckedForVersioning = new HashSet<ResourceContainer>();
        public void CheckSetForVersioning(ResourceContainer entitySet)
        {
            if (this.setsCheckedForVersioning.Add(entitySet))
            {
                // Go through all types contained in the set.
                //  If any one type has collection properties, the whole set has them.
                //  If any one type has named streams, the whole set has them.
                ResourceType baseType = entitySet.BaseType;

                foreach (var navProp in entitySet.BaseType.Properties.OfType<ResourceProperty>().Where(p => p.IsNavigation))
                {
                    this.MetadataProvider.GetResourceAssociationSet(entitySet, entitySet.BaseType, navProp);
                }

                // If the base type has named streams or it has no derived type, we need not look any further.
                if (this.MetadataProvider.HasDerivedTypes(baseType))
                {
                    foreach (ResourceType derivedType in this.MetadataProvider.GetDelayedDerivedTypes(baseType))
                    {
                        foreach (var navProp in derivedType.Properties.OfType<ResourceProperty>().Where(p => p.IsNavigation))
                        {
                            // get the declaring type for the property
                            var declaringType = derivedType;
                            while(declaringType.BaseType != null && declaringType.BaseType.Properties.Any(p => p.Name == navProp.Name))
                            {
                                declaringType = (ResourceType)declaringType.BaseType;
                            }

                            this.MetadataProvider.GetResourceAssociationSet(entitySet, declaringType, navProp);
                        }
                    }
                }
            }
        }
    }

    #region Interfaces
    public abstract class InterfaceCallLogBuilder<T>
    {
        internal InterfaceCallLogBuilder(APICallLogBuilder parent)
        {
            Parent = parent;
        }

        protected APICallLogBuilder Parent
        {
            get;
            private set;
        }

        protected Type Interface 
        { 
            get { return typeof(T); } 
        }

        public abstract bool ServiceHasInterface
        {
            get;
        }
        
        public virtual void GetService()
        {
            this.Parent.ServiceProvider.GetService(this.Interface);
        }

        protected void Add(string methodName, params string[] argumentValues)
        {
            if (!ServiceHasInterface)
                return;

            MethodInfo method = typeof(T).GetMethod(methodName);
            if (method == null)
                throw new ArgumentException("Could not find method '" + methodName + "' on interface '" + typeof(T).ToString() + "'");

            Parent.Add(method, argumentValues);
        }

        protected void AddGetProperty(string propertyName)
        {
            if (!ServiceHasInterface)
                return;

            PropertyInfo property = typeof(T).GetProperty(propertyName);
            if (property == null)
                throw new ArgumentException("Could not find property '" + propertyName + "' on interface '" + typeof(T).ToString() + "'");

            Parent.Add(property.GetGetMethod());
        }

        protected void AddSetProperty(string propertyName, string valueType)
        {
            if (!ServiceHasInterface)
                return;

            PropertyInfo property = typeof(T).GetProperty(propertyName);
            if (property == null)
                throw new ArgumentException("Could not find property '" + propertyName + "' on interface '" + typeof(T).ToString() + "'");

            Parent.Add(property.GetSetMethod(), valueType == null ? "null" : valueType);
        }

        protected string Serialize(object value)
        {
            if (value == null)
                return "null";

            Type t = value.GetType();

            if (t == typeof(byte[]))
                return Convert.ToBase64String((byte[])value);

            if(t == typeof(DateTime))
                return ((DateTime)value).ToString("o", Globalization.CultureInfo.InvariantCulture.NumberFormat);

            if (t.IsPrimitive || t == typeof(string) || t.IsValueType)
                return value.ToString();

            return t.ToString();
        }

        protected string Serialize(Type t)
        {
            return t.ToString();
        }

        public virtual void Dispose()
        {
            if (!Versioning.Server.SupportsV2Features)
                return;

            if (!ServiceHasInterface)
                return;

            Parent.Add(typeof(T).FullName + ".Dispose");
        }
    }

    #region IUpdatable
    public class IUpdatableCallLogBuilder : InterfaceCallLogBuilder<IUpdatable>
    {
        internal IUpdatableCallLogBuilder(APICallLogBuilder parent)
            : base(parent)
        {
        }

        public override bool ServiceHasInterface
        {
            get 
            {
                return Parent.Workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr 
                    || Parent.Workspace.DataLayerProviderKind == DataLayerProviderKind.InMemoryLinq;
            }
        }
          
        public override void GetService()
        {
            // for EF, we will never ask for either update interface
            if (Parent.Workspace.DataLayerProviderKind == DataLayerProviderKind.Edm)
                return;

            // we will always ask for the most-derived version of an interface first
            if (this.Parent.ServiceProvider.GetService(typeof(IDataServiceUpdateProvider2)))
            {
                // because we never return an UpdateProvider2, the product will always check the current data source
                // this goes inside an if-statement because it will only happen the first time we try to find the provider
                this.Parent.QueryProvider.GetCurrentDataSource();
            }
            
            // for custom provider, we will never ask for IUpdatable, otherwise we always will
            if (Parent.Workspace.DataLayerProviderKind != DataLayerProviderKind.NonClr)
                base.GetService();

            // if we did not find the iupdatable...
            if (Parent.Workspace.Settings.UpdatableImplementation != UpdatableImplementation.IUpdatable)
                Parent.UpdateProvider.GetService();
        }

        public string GetUpdatableTokenTypeName(ComplexType type)
        {
            if (Parent.Workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr)
            {
                if (type.Facets.IsClrType)
                    return type.FullName;
                else if (type is ResourceType)
                    return typeof(NonClr.RowEntityType).FullName;
                else
                    return typeof(NonClr.RowComplexType).FullName;
            }
            else
            {
                return GetUpdatableTokenTypeName(type.Name);
            }
        }

        public string GetUpdatableTokenTypeName(string typeName)
        {
            if (Parent.Workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr)
            {
                ComplexType type = Parent.Workspace.ServiceContainer.AllTypes.Single(rt => rt.Name == typeName);
                return GetUpdatableTokenTypeName(type);
            }
            else if (Parent.Workspace.DataLayerProviderKind == DataLayerProviderKind.InMemoryLinq)
            {
                return typeof(InMemoryLinq.InMemoryContext.ResourceToken).FullName;
            }
            else
            {
                return Parent.Workspace.ContextNamespace + "." + typeName;
            }
        }

        public void AddReferenceToCollection(ComplexResourceInstance instance, string propertyName, ComplexResourceInstance value)
        {
            Add("AddReferenceToCollection", GetUpdatableTokenTypeName(instance.TypeName), propertyName, GetUpdatableTokenTypeName(value.TypeName));
        }

        public void AddReferenceToCollection(ResourceType instance, string propertyName, ResourceType value)
        {
            Add("AddReferenceToCollection", GetUpdatableTokenTypeName(instance), propertyName, GetUpdatableTokenTypeName(value));
        }

        public void ClearChanges()
        {
            Add("ClearChanges");
        }

        public void CreateResource(ResourceContainer container, ResourceType type)
        {
            Add("CreateResource", container.Name, type.FullName);
        }

        public void CreateResource(ComplexType type)
        {
            Add("CreateResource", "null", type.FullName);
        }

        public void CreateResource(KeyedResourceInstance instance)
        {
            Add("CreateResource", instance.ResourceSetName, Parent.Workspace.ContextNamespace + "." + instance.TypeName);
        }

        public void CreateResource(ComplexResourceInstance instance)
        {
            Add("CreateResource", "null", Parent.Workspace.ContextNamespace + "." + instance.TypeName);
        }

        public void DeleteResource(string typeName)
        {
            Add("DeleteResource", typeName);
        }

        public void DeleteResource(ComplexType type)
        {
            Add("DeleteResource", GetUpdatableTokenTypeName(type));
        }

        public void GetResource(string queryableType, string fullTypeName)
        {
            if (fullTypeName == null)
                fullTypeName = "null";
            Add("GetResource", queryableType, fullTypeName);
        }

        public void GetResource(ResourceContainer container, string fullTypeName)
        {
            GetResource(Parent.QueryProvider.GetQueryableTypeName(container), fullTypeName);
        }

        public void GetValue(ResourceType targetResourceType, string propertyName)
        {
            Add("GetValue", GetUpdatableTokenTypeName(targetResourceType), propertyName);
        }

        public void RemoveReferenceFromCollection(ComplexType instanceType, string propertyName, ComplexType valueType)
        {
            Add("RemoveReferenceFromCollection", GetUpdatableTokenTypeName(instanceType), propertyName, GetUpdatableTokenTypeName(valueType));
        }

        public void RemoveReferenceFromCollection(ComplexResourceInstance instance, string propertyName, ComplexResourceInstance value)
        {
            Add("RemoveReferenceFromCollection", GetUpdatableTokenTypeName(instance.TypeName), propertyName, GetUpdatableTokenTypeName(value.TypeName));
        }

        public void ResetResource(string typeName)
        {
            Add("ResetResource", typeName);
        }

        public void ResetResource(ResourceType type)
        {
            ResetResource(GetUpdatableTokenTypeName(type));
        }

        public void ResolveResource(ResourceType type)
        {
            Add("ResolveResource", GetUpdatableTokenTypeName(type));
        }

        public void ResolveResource(string tokenTypeName)
        {
            Add("ResolveResource", tokenTypeName);
        }

        public void SaveChanges()
        {
            Add("SaveChanges");
        }

        public void SetReference(ComplexType instanceType, string propertyName, ComplexType valueType)
        {
            Add("SetReference", GetUpdatableTokenTypeName(instanceType), propertyName, GetUpdatableTokenTypeName(valueType));
        }

        public void SetReference(ComplexResourceInstance instance, string propertyName, ComplexResourceInstance value)
        {
            Add("SetReference", GetUpdatableTokenTypeName(instance.TypeName), propertyName, GetUpdatableTokenTypeName(value.TypeName));
        }

        public void SetReferenceToNull(ComplexType instanceType, string propertyName)
        {
            Add("SetReference", GetUpdatableTokenTypeName(instanceType), propertyName, "null");
        }

        public void SetReferenceToNull(ComplexResourceInstance instance, string propertyName)
        {
            Add("SetReference", GetUpdatableTokenTypeName(instance.TypeName), propertyName, "null");
        }

        public void SetValue(ComplexResourceInstance instance, ResourceInstanceProperty property)
        {
            SetValue(Parent.Workspace.ServiceContainer.AllTypes.Single(ct => ct.Name == instance.TypeName), property);
        }

        public void SetValue(ComplexType type, ResourceInstanceProperty property)
        {
            SetValue(type, property, false);
        }

        public void SetValue(ComplexType type, ResourceInstanceProperty property, bool propertyOnly)
        {
            Globalization.CultureInfo realCulture = Globalization.CultureInfo.CurrentCulture;
            Threading.Thread.CurrentThread.CurrentCulture = Globalization.CultureInfo.InvariantCulture;
            try
            {
                string typeName = GetUpdatableTokenTypeName(type);
                if (property is ResourceInstanceSimpleProperty)
                {
                    ResourceInstanceSimpleProperty simple = property as ResourceInstanceSimpleProperty;
                    if (simple.PropertyValue == null)
                    {
                        Add("SetValue", typeName, simple.Name, "null");
                    }
                    else
                    {
                        NodeProperty propertyInfo = type.Properties.SingleOrDefault(p => p.Name == property.Name);
                        bool isOpenProperty = propertyInfo == null || !propertyInfo.Facets.IsDeclaredProperty;

                        object value = simple.PropertyValue;

                        Type t = value.GetType();

                        string valueAsString;
                        if (t == typeof(byte[]))
                        {
                            valueAsString = Convert.ToBase64String((byte[])value);
                        }
                        else if (t == typeof(DateTime))
                        {
                            DateTime dt = (DateTime)value;
                            if (Parent.CurrentFormat == SerializationFormatKind.JSON)
                            {
                                if (simple.UseTickCountForJsonDateTime)
                                {
                                    dt = new DateTime(dt.Ticks - dt.Ticks % 10000, DateTimeKind.Utc);
                                    valueAsString = dt.ToString("o", Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                }
                                else if (!isOpenProperty)
                                {
                                    dt = (DateTime)Convert.ChangeType(XmlConvert.ToString(dt, XmlDateTimeSerializationMode.RoundtripKind), typeof(DateTime), Globalization.CultureInfo.InvariantCulture);
                                    valueAsString = dt.ToString("o", Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                }
                                else
                                {
                                    valueAsString = XmlConvert.ToString(dt, XmlDateTimeSerializationMode.RoundtripKind);
                                }
                            }
                            else
                            {
                                if (!isOpenProperty || (property.IncludeTypeMetadataHint))
                                    valueAsString = dt.ToString("o", Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                else
                                    valueAsString = XmlConvert.ToString(dt, XmlDateTimeSerializationMode.RoundtripKind);
                            }
                        }
                        else if (t.IsPrimitive || t == typeof(string) || t.IsValueType)
                        {
                            if (!isOpenProperty)
                            {
                                valueAsString = value.ToString();
                            }
                            else
                            {
                                if (Parent.CurrentFormat == SerializationFormatKind.JSON)
                                {
                                    var jsonType = JsonPrimitiveTypesUtil.JsonPrimitiveTypeMapping(value);
                                    value = JsonPrimitiveTypesUtil.StringToPrimitive(JsonPrimitiveTypesUtil.PrimitiveToStringUnquoted(value, t), jsonType);
                                    valueAsString = value.ToString();
                                }
                                else
                                {
                                    if (property.IncludeTypeMetadataHint)
                                    {
                                        valueAsString = value.ToString();
                                    }
                                    else // type will be lost, will just be a string
                                    {
                                        valueAsString = TypeData.XmlValueFromObject(value);
                                    }
                                }
                            }
                        }
                        else
                        {
                            valueAsString = t.ToString();
                        }

                        Add("SetValue", typeName, simple.Name, valueAsString);
                    }
                }
                else if (property is ResourceInstanceComplexProperty)
                {
                    ResourceInstanceComplexProperty complex = property as ResourceInstanceComplexProperty;
                    if (complex.ComplexResourceInstance == null)
                    {
                        Add("SetValue", typeName, complex.Name, "null");
                    }
                    else
                    {
                        Add("SetValue", typeName, complex.Name, GetUpdatableTokenTypeName(complex.ComplexResourceInstance.TypeName));
                    }
                }
            }
            finally
            {
                Threading.Thread.CurrentThread.CurrentCulture = realCulture;
            }
        }

        public void SetValue(string typeName, string propertyName, string propertyValue)
        {
            Add("SetValue", typeName, propertyName, propertyValue);
        }
    }
    #endregion

    #region IExpandProvider
    public class IExpandProviderCallLogBuilder : InterfaceCallLogBuilder<IExpandProvider>
    {
        internal IExpandProviderCallLogBuilder(APICallLogBuilder parent)
            : base(parent)
        {
        }

        public override bool ServiceHasInterface
        {
            get 
            {
                return Parent.Workspace.ServiceModifications.Interfaces.IServiceProvider.Services.ContainsKey(typeof(IExpandProvider));
            }
        }

        public override void GetService()
        {
            if (this.Parent.ServiceProvider.GetService(this.Interface) && !this.ServiceHasInterface)
            {
                this.Parent.QueryProvider.GetCurrentDataSource();
            }
        }

        public void ApplyExpansions(ResourceContainer container, string expandedProperties)
        {
            Add("ApplyExpansions", Parent.QueryProvider.GetQueryableTypeName(container), expandedProperties);
        }
    }
    #endregion

    #region IDataServiceUpdateProvider
    public class IDataServiceUpdateProviderCallLogBuilder : InterfaceCallLogBuilder<IDataServiceUpdateProvider>
    {
        internal IDataServiceUpdateProviderCallLogBuilder(APICallLogBuilder parent)
            : base (parent)
        {
        }

        public override bool ServiceHasInterface
        {
            get
            {
                if (Parent.Workspace.DataLayerProviderKind == DataLayerProviderKind.InMemoryLinq || Parent.Workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr)
                    return Parent.Workspace.Settings.UpdatableImplementation == UpdatableImplementation.DataServiceUpdateProvider;
                return false;
            }
        }

        public void SetConcurrencyValues(ResourceType type, bool? checkForEquality, params KeyValuePair<string, object>[] concurrencyValues)
        {
            SetConcurrencyValues(type, checkForEquality, concurrencyValues.AsEnumerable());
        }

        public void SetConcurrencyValues(ResourceType type, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
        {
            if (!ServiceHasInterface)
                return;

            string tokenTypeName = Parent.Updatable.GetUpdatableTokenTypeName(type);
            string check = (checkForEquality.HasValue) ? checkForEquality.Value.ToString() : "null";

            if (concurrencyValues != null)
            {
                List<string> arguments = new List<string>() 
                { 
                    "resourceCookie", tokenTypeName, 
                    "checkForEquality", check 
                };
                arguments.AddRange(concurrencyValues.SelectMany(p => new string[] { p.Key, p.Value == null ? "null" : p.Value.ToString() }));
                Parent.Add(new APICallLogEntry("IDataServiceUpdateProvider.SetConcurrencyValues", arguments.ToArray()));
            }
            else
            {
                Add("SetConcurrencyValues", tokenTypeName, check, "null");
            }
        }
    }
    #endregion

    #region IServiceProvider
    public class IServiceProviderCallLogBuilder : InterfaceCallLogBuilder<IServiceProvider>
    {
        internal IServiceProviderCallLogBuilder(APICallLogBuilder parent)
            : base(parent)
        {
        }

        private HashSet<Type> typeCache = new HashSet<Type>();

        public override bool ServiceHasInterface
        {
            get 
            {
                return Versioning.Server.SupportsV2Features && Parent.Workspace.ServiceModifications.Interfaces.IServiceProvider.Services.Any();
            }
        }

        public bool GetService(Type serviceType)
        {
            if (typeCache.Add(serviceType))
            {
                Add("GetService", serviceType.ToString());
                return true;
            }

            return false;
        }

        public bool GetServiceCalled(Type type)
        {
            return typeCache.Contains(type);
        }
    }
    #endregion

    #region IDataServiceMetadataProvider
    public class IDataServiceMetadataProviderCallLogBuilder : InterfaceCallLogBuilder<Microsoft.OData.Service.Providers.IDataServiceMetadataProvider>
    {
        internal IDataServiceMetadataProviderCallLogBuilder(APICallLogBuilder parent)
            : base(parent)
        {
            CacheEntitySets = true;
            CacheEntityTypes = true;
        }

        public override bool ServiceHasInterface
        {
            get 
            {
                return Parent.Workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr;
            }
        }

        public bool CacheEntitySets
        {
            get;
            set;
        }

        public bool CacheEntityTypes
        {
            get;
            set;
        }

        private readonly HashSet<string> associationSetCache = new HashSet<string>();
        private readonly HashSet<string> resourceSetCache = new HashSet<string>();
        private readonly HashSet<string> resourceTypeCache = new HashSet<string>();
        private bool containerNameCached = false;
        private bool containerNamespaceCached = false;
        private bool resourceSetsCached = false;

        private string Serialize(ResourceContainer set)
        {
            return set.Name;
        }

        private string Serialize(ResourceType type)
        {
            return type.FullName;
        }

        private string Serialize(ResourceProperty property)
        {
            return property.Name;
        }

        private string Serialize(ServiceOperation serviceOp)
        {
            return serviceOp.Name;
        }

        #region IDataServiceMetadataProvider Members

        public void ContainerName()
        {
            if (!this.containerNameCached)
            {
                this.containerNameCached = true;
                AddGetProperty("ContainerName");
            }
        }

        public void ContainerNamespace()
        {
            if (!this.containerNamespaceCached)
            {
                this.containerNamespaceCached = true;
                AddGetProperty("ContainerNamespace");
            }
        }

        public IEnumerable<ComplexType> GetDelayedDerivedTypes(ResourceType resourceType)
        {
            Add("GetDerivedTypes", Serialize(resourceType));

            foreach (ResourceType type in resourceType.DerivedTypes)
            {
                resourceTypeCache.Add(type.FullName);
                yield return type;
            }
        }

        public void GetDerivedTypes(ResourceType resourceType)
        {
            Add("GetDerivedTypes", Serialize(resourceType));

            foreach (ResourceType type in resourceType.DerivedTypes)
            {
                resourceTypeCache.Add(type.FullName);
            }
        }

        public void GetResourceAssociationSet(string setName, string fullTypeName, string propertyName)
        {
            string key = setName + "_" + fullTypeName + "_" + propertyName;
            if (associationSetCache.Add(key))
            {
                Add("GetResourceAssociationSet", setName, fullTypeName, propertyName);
                ResourceContainer container = Parent.Workspace.ServiceContainer.ResourceContainers[setName];
                ResourceProperty property = container.BaseType.Properties[propertyName] as ResourceProperty;
                container = container.FindDefaultRelatedContainer(property);
                resourceSetCache.Add(container.Name);
            }
        }

        public void GetResourceAssociationSet(ResourceContainer resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            GetResourceAssociationSet(Serialize(resourceSet), Serialize(resourceType), Serialize(resourceProperty));
        }

        public void GetResourceAssociationSet(KeyedResourceInstance instance, ResourceInstanceProperty property)
        {
            ResourceContainer container = Parent.Workspace.ServiceContainer.ResourceContainers[instance.ResourceSetName];

            GetResourceAssociationSet(instance.ResourceSetName, container.BaseType.FullName, property.Name);
        }

        public bool HasDerivedTypes(ResourceType resourceType)
        {
            Add("HasDerivedTypes", Serialize(resourceType));
            return resourceType.HasDerivedTypes;
        }

        public void ResourceSets()
        {
            if (!this.resourceSetsCached)
            {
                this.resourceSetsCached = true;
                AddGetProperty("ResourceSets");
            }
        }

        public void ServiceOperations()
        {
            AddGetProperty("ServiceOperations");
        }

        public void TryResolveResourceSet(string name)
        {
            if (!CacheEntitySets || resourceSetCache.Add(name))
            {
                Add("TryResolveResourceSet", name);
                
            }
            ResourceContainer container = Parent.Workspace.ServiceContainer.ResourceContainers[name];
            if (container != null && !(container is ServiceOperation))
                resourceTypeCache.Add(container.BaseType.FullName);
        }

        public void TryResolveResourceType(string name)
        {
            if(!CacheEntityTypes || resourceTypeCache.Add(name))
                Add("TryResolveResourceType", name);
        }

        public void TryResolveServiceOperation(string name)
        {
            Add("TryResolveServiceOperation", name);
        }

        public void Types()
        {
            AddGetProperty("Types");
        }

        #endregion
    }
    #endregion

    #region IDataServiceQueryProvider
    public class IDataServiceQueryProviderCallLogBuilder : InterfaceCallLogBuilder<Microsoft.OData.Service.Providers.IDataServiceQueryProvider>
    {
        internal IDataServiceQueryProviderCallLogBuilder(APICallLogBuilder parent)
            : base(parent)
        {
        }

        public override bool ServiceHasInterface
        {
            get
            {
                return Parent.Workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr;
            }
        }

        public string GetQueryableTypeName(ResourceContainer container)
        {
            string elementTypeName = container.BaseType.FullName;

            string queryableType = null;
            switch (Parent.Workspace.DataLayerProviderKind)
            {
                case DataLayerProviderKind.InMemoryLinq:
                    queryableType = "System.Linq.EnumerableQuery";
                    break;

                case DataLayerProviderKind.LinqToSql:
                    queryableType = "System.Data.Linq.DataQuery";
                    break;

                case DataLayerProviderKind.Edm:
                    queryableType = "System.Data.Objects.ObjectQuery";
                    break;

                case DataLayerProviderKind.NonClr:
                    queryableType = typeof(NonClr.NonClrQueryable).FullName;
                    if (!container.BaseType.Facets.IsClrType)
                        elementTypeName = typeof(NonClr.RowEntityType).FullName;
                    break;

                default:
                    AstoriaTestLog.FailAndThrow("Unexpected request for query type update on provider: " + Parent.Workspace.DataLayerProviderKind + ", cannot infer queryable type");
                    break;
            }

            return queryableType + "`1[" + elementTypeName + "]";
        }

        private string Serialize(ResourceContainer set)
        {
            return set.Name;
        }

        private string Serialize(ResourceType type)
        {
            return type.FullName;
        }

        private string Serialize(ResourceProperty property)
        {
            return property.Name;
        }

        private string Serialize(ServiceOperation serviceOp)
        {
            return serviceOp.Name;
        }

        public string GetBackingTypeName(ComplexType type)
        {
            if (Parent.Workspace.DataLayerProviderKind != DataLayerProviderKind.NonClr || type.Facets.IsClrType)
                return type.FullName;
            if (type is ResourceType)
                return typeof(NonClr.RowEntityType).FullName;
            return typeof(NonClr.RowComplexType).FullName;
        }

        #region IDataServiceQueryProvider Members

        public void GetResourceType(ComplexType type)
        {
            GetResourceType(GetBackingTypeName(type));
        }

        public void GetResourceType(string typeName)
        {
            Add("GetResourceType", typeName);
        }
        
        public void GetCurrentDataSource()
        {
            AddGetProperty("CurrentDataSource");
        }

        public void SetCurrentDataSource(string value)
        {
            AddSetProperty("CurrentDataSource", value);
        }

        public void GetOpenPropertyValue(ResourceType type, string propertyName)
        {
            Add("GetOpenPropertyValue", GetBackingTypeName(type), propertyName);
        }

        public void GetOpenPropertyValues(ResourceType type)
        {
            Add("GetOpenPropertyValues", GetBackingTypeName(type));
        }

        public void GetPropertyValue(ComplexType type, NodeProperty property)
        {
            if(!property.Facets.IsClrProperty)
                Add("GetPropertyValue", GetBackingTypeName(type), property.Name);
        }

        public void GetQueryRootForResourceSet(ResourceContainer resourceSet)
        {
            Add("GetQueryRootForResourceSet", Serialize(resourceSet));
        }

        public void GetQueryRootForResourceSet(string setName)
        {
            Add("GetQueryRootForResourceSet", setName);
        }

        public void InvokeServiceOperation(ServiceOperation serviceOperation, object[] parameters)
        {
            Add("InvokeServiceOperation", Serialize(serviceOperation), Serialize(parameters));
        }

        public void InvokeServiceOperation(string serviceOperationName, params object[] parameters)
        {
            Add("InvokeServiceOperation", serviceOperationName, Serialize(parameters));
        }

        public void IsNullPropagationRequired()
        {
            AddGetProperty("IsNullPropagationRequired");
        }
        #endregion
    }
    #endregion

    #region IDataServiceStreamProvider
    public class IDataServiceStreamProviderCallLogBuilder : InterfaceCallLogBuilder<Microsoft.OData.Service.Providers.IDataServiceStreamProvider>
    {
        public IDataServiceStreamProviderCallLogBuilder(APICallLogBuilder parent)
            : base(parent) 
        { }

        public override bool ServiceHasInterface
        {
            get
            {
                return Parent.Workspace.Settings.SupportsMediaLinkEntries;
            }
        }

        public override void GetService()
        {
            Parent.StreamProvider2.GetService();
            if (!Parent.StreamProvider2.ServiceHasInterface && !Parent.ServiceProvider.GetServiceCalled(this.Interface))
            {
                // will get the current data source to see if it implements the interface
                Parent.QueryProvider.GetCurrentDataSource();
                base.GetService();
            }
        }

        public void DeleteStream(ComplexType type, AstoriaRequest request)
        {
            if (!ServiceHasInterface)
                return;

            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider.DeleteStream",
                "entity", Parent.QueryProvider.GetBackingTypeName(type),
                "AbsoluteRequestUri", Uri.UnescapeDataString(request.URI),
                "AbsoluteServiceUri", request.Workspace.ServiceUri + "/",
                "IsBatchRequest", (request is BatchRequest || request.Batched).ToString(),
                "RequestMethod", request.EffectiveVerb.ToHttpMethod()));
        }

        public void GetReadStream(ComplexType type, string etag, bool? checkETagForEquality, AstoriaRequest request)
        {
            if (!ServiceHasInterface)
                return;

            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider.GetReadStream",
                "entity", Parent.QueryProvider.GetBackingTypeName(type),
                "etag", (etag == null ? "null" : etag),
                "checkETagForEquality", (checkETagForEquality.HasValue ? checkETagForEquality.Value.ToString() : "null"),
                "AbsoluteRequestUri", Uri.UnescapeDataString(request.URI),
                "AbsoluteServiceUri", request.Workspace.ServiceUri + "/",
                "IsBatchRequest", (request is BatchRequest || request.Batched).ToString(),
                "RequestMethod", request.EffectiveVerb.ToHttpMethod()));
        }

        public void GetReadStreamUri(ComplexType type, AstoriaRequest request)
        {
            if (!ServiceHasInterface)
                return;

            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider.GetReadStreamUri",
                "entity", Parent.QueryProvider.GetBackingTypeName(type),
                "AbsoluteRequestUri", Uri.UnescapeDataString(request.URI),
                "AbsoluteServiceUri", request.Workspace.ServiceUri + "/",
                "IsBatchRequest", (request is BatchRequest || request.Batched).ToString(),
                "RequestMethod", request.EffectiveVerb.ToHttpMethod()));
        }

        public void GetStreamContentType(ComplexType type, AstoriaRequest request)
        {
            if (!ServiceHasInterface)
                return;

            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider.GetStreamContentType",
                "entity", Parent.QueryProvider.GetBackingTypeName(type),
                "AbsoluteRequestUri", Uri.UnescapeDataString(request.URI),
                "AbsoluteServiceUri", request.Workspace.ServiceUri + "/",
                "IsBatchRequest", (request is BatchRequest || request.Batched).ToString(),
                "RequestMethod", request.EffectiveVerb.ToHttpMethod()));
        }

        public void GetStreamETag(ComplexType type, AstoriaRequest request)
        {
            if (!ServiceHasInterface)
                return;

            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider.GetStreamETag",
                "entity", Parent.QueryProvider.GetBackingTypeName(type),
                "AbsoluteRequestUri", Uri.UnescapeDataString(request.URI),
                "AbsoluteServiceUri", request.Workspace.ServiceUri + "/",
                "IsBatchRequest", (request is BatchRequest || request.Batched).ToString(),
                "RequestMethod", request.EffectiveVerb.ToHttpMethod()));
        }

        public void GetWriteStream(ComplexType type, string etag, bool? checkETagForEquality, AstoriaRequest request)
        {
            if (!ServiceHasInterface)
                return;

            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider.GetWriteStream",
                "entity", Parent.QueryProvider.GetBackingTypeName(type),
                "etag", (etag == null ? "null" : etag),
                "checkETagForEquality", (checkETagForEquality.HasValue ? checkETagForEquality.Value.ToString() : "null"),
                "AbsoluteRequestUri", Uri.UnescapeDataString(request.URI),
                "AbsoluteServiceUri", request.Workspace.ServiceUri + "/",
                "IsBatchRequest", (request is BatchRequest || request.Batched).ToString(),
                "RequestMethod", request.EffectiveVerb.ToHttpMethod()));
        }

        public void ResolveType(string entitySetName, AstoriaRequest request)
        {
            if (!ServiceHasInterface)
                return;

            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider.ResolveType",
                "entitySetName", (entitySetName == null ? "null" : entitySetName),
                "AbsoluteRequestUri", Uri.UnescapeDataString(request.URI),
                "AbsoluteServiceUri", request.Workspace.ServiceUri + "/",
                "IsBatchRequest", (request is BatchRequest || request.Batched).ToString(),
                "RequestMethod", request.EffectiveVerb.ToHttpMethod()));
        }

        public void StreamBufferSize()
        {
            AddGetProperty("StreamBufferSize");
        }
    }
    #endregion

    #region IDataServiceStreamProvider2
    public class IDataServiceStreamProvider2CallLogBuilder : InterfaceCallLogBuilder<Microsoft.OData.Service.Providers.IDataServiceStreamProvider2>
    {
        public IDataServiceStreamProvider2CallLogBuilder(APICallLogBuilder parent)
            : base(parent)
        { }

        public override bool ServiceHasInterface
        {
            get
            {
                return Parent.Workspace.Settings.SupportsNamedStreams;
            }
        }
        
        public void GetReadStream(ComplexType type, string streamName, string etag, bool? checkETagForEquality, AstoriaRequest request)
        {
            if (!ServiceHasInterface)
                return;

            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider2.GetReadStream",
                "entity", Parent.QueryProvider.GetBackingTypeName(type),
                "streamProperty", streamName,
                "etag", (etag == null ? "null" : etag),
                "checkETagForEquality", (checkETagForEquality.HasValue ? checkETagForEquality.Value.ToString() : "null"),
                "AbsoluteRequestUri", Uri.UnescapeDataString(request.URI),
                "AbsoluteServiceUri", request.Workspace.ServiceUri + "/",
                "IsBatchRequest", (request is BatchRequest || request.Batched).ToString(),
                "RequestMethod", request.EffectiveVerb.ToHttpMethod()));
        }

        public void GetReadStreamUri(ComplexType type, string streamName, AstoriaRequest request)
        {
            if (!ServiceHasInterface)
                return;

            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider2.GetReadStreamUri",
                "entity", Parent.QueryProvider.GetBackingTypeName(type),
                "streamProperty", streamName,
                "AbsoluteRequestUri", Uri.UnescapeDataString(request.URI),
                "AbsoluteServiceUri", request.Workspace.ServiceUri + "/",
                "IsBatchRequest", (request is BatchRequest || request.Batched).ToString(),
                "RequestMethod", request.EffectiveVerb.ToHttpMethod()));
        }

        public void GetStreamContentType(ComplexType type, string streamName, AstoriaRequest request)
        {
            if (!ServiceHasInterface)
                return;

            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider2.GetStreamContentType",
                "entity", Parent.QueryProvider.GetBackingTypeName(type),
                "streamProperty", streamName,
                "AbsoluteRequestUri", Uri.UnescapeDataString(request.URI),
                "AbsoluteServiceUri", request.Workspace.ServiceUri + "/",
                "IsBatchRequest", (request is BatchRequest || request.Batched).ToString(),
                "RequestMethod", request.EffectiveVerb.ToHttpMethod()));
        }

        public void GetStreamETag(ComplexType type, string streamName, AstoriaRequest request)
        {
            if (!ServiceHasInterface)
                return;

            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider2.GetStreamETag",
                "entity", Parent.QueryProvider.GetBackingTypeName(type),
                "streamProperty", streamName,
                "AbsoluteRequestUri", Uri.UnescapeDataString(request.URI),
                "AbsoluteServiceUri", request.Workspace.ServiceUri + "/",
                "IsBatchRequest", (request is BatchRequest || request.Batched).ToString(),
                "RequestMethod", request.EffectiveVerb.ToHttpMethod()));
        }

        public void GetWriteStream(ComplexType type, string streamName, string etag, bool? checkETagForEquality, AstoriaRequest request)
        {
            if (!ServiceHasInterface)
                return;

            Parent.Add(new APICallLogEntry("IDataServiceStreamProvider2.GetWriteStream",
                "entity", Parent.QueryProvider.GetBackingTypeName(type),
                "streamProperty", streamName,
                "etag", (etag == null ? "null" : etag),
                "checkETagForEquality", (checkETagForEquality.HasValue ? checkETagForEquality.Value.ToString() : "null"),
                "AbsoluteRequestUri", Uri.UnescapeDataString(request.URI),
                "AbsoluteServiceUri", request.Workspace.ServiceUri + "/",
                "IsBatchRequest", (request is BatchRequest || request.Batched).ToString(),
                "RequestMethod", request.EffectiveVerb.ToHttpMethod()));
        }
    }
    #endregion

    #region IDataServicePagingProvider
    public class IDataServicePagingProviderCallLogBuilder : InterfaceCallLogBuilder<Microsoft.OData.Service.Providers.IDataServicePagingProvider>
    {
        internal IDataServicePagingProviderCallLogBuilder(APICallLogBuilder parent)
            : base(parent)
        {}

        public override bool ServiceHasInterface
        {
            get
            {
                return Parent.Workspace.ServiceModifications.Interfaces.IServiceProvider.Services
                    .ContainsKey(typeof(Microsoft.OData.Service.Providers.IDataServicePagingProvider));
            }
        }

        public void GetContinuationToken(ResourceContainer container)
        {
            // Enumerator's type is too hard to predict right now, so leave it off
            if(ServiceHasInterface)
                Parent.Add("GetContinuationToken");
        }

        public void SetContinuationToken(string queryableType, ResourceType type, object[] continuationToken)
        {
            if (continuationToken == null)
                Add("SetContinuationToken", queryableType, type.FullName, "null");
            else
            {
                Add("SetContinuationToken", queryableType, type.FullName,
                    string.Join(", ", continuationToken.Select(o => Serialize(o)).ToArray()));
            }
        }

        public void SetContinuationToken(ResourceContainer container, ResourceType type, object[] continuationToken)
        {
            SetContinuationToken(Parent.QueryProvider.GetQueryableTypeName(container), type, continuationToken);
        }

        public override void GetService()
        {
            // will never ask for paging provider if its EDM
            if (this.Parent.Workspace.DataLayerProviderKind != DataLayerProviderKind.Edm)
            {
                if (this.Parent.ServiceProvider.GetService(this.Interface) && !this.ServiceHasInterface)
                {
                    this.Parent.QueryProvider.GetCurrentDataSource();
                }
            }
        }
    }
    #endregion

    #region DataService<T>
    public class DataServiceAPICallLogBuilder
    {
        private APICallLogBuilder Parent;

        internal DataServiceAPICallLogBuilder(APICallLogBuilder parent)
        {
            Parent = parent;
        }

        private static string GetServiceTypeFromWorkspace(Workspace w)
        {
            return w.ServiceClassName;
        }

        private string DefaultServiceTypeName
        {
            get
            {
                return GetServiceTypeFromWorkspace(Parent.Workspace);
            }
        }

        public void CreateDataSource()
        {
            CreateDataSource(DefaultServiceTypeName);
        }

        public void CreateDataSource(string serviceTypeName)
        {
            Parent.Add(new APICallLogEntry(serviceTypeName + ".CreateDataSource", new KeyValuePair<string, string>[0]));
        }

        public void HandleException(AstoriaResponse response, string exceptionType, bool responseWritten)
        {
            string message = response.Workspace.GetLocalizedResourceString(response.Request.ExpectedErrorIdentifier.Id, response.Request.ExpectedErrorArguments);

            HandleException(response, exceptionType, message, responseWritten);
        }

        public void HandleException(AstoriaResponse response, bool responseWritten)
        {
            HandleException(response, "Microsoft.OData.Service.DataServiceException", responseWritten);
        }

        public void HandleException(AstoriaResponse response, string exceptionType, string message, bool responseWritten)
        {
            HandleException(DefaultServiceTypeName, exceptionType, message, response.ContentType, response.ActualStatusCode, responseWritten, true);
        }

        public void HandleException(string exceptionType, string message, string contentType, HttpStatusCode statusCode, bool responseWritten)
        {
            HandleException(DefaultServiceTypeName, exceptionType, message, contentType, statusCode, responseWritten, true);
        }

        public void HandleException(string serviceTypeName, string exceptionType, string message, 
            string contentType, HttpStatusCode statusCode, bool responseWritten, bool verbose)
        {
            Parent.Add(new APICallLogEntry(serviceTypeName + "." + "HandleException",
                new Dictionary<string, string>()
                {
                    { "ExceptionType", exceptionType },
                    { "ExceptionMessage", message },
                    { "ResponseContentType", contentType },
                    { "ResponseStatusCode", ((int)statusCode).ToString() },
                    { "ResponseWritten", responseWritten.ToString() },
                    { "UseVerboseErrors", verbose.ToString() }
                }));
        }

        public void OnStartProcessingRequest(AstoriaRequest request)
        {
            string uri = request.URI;
            if (!uri.StartsWith(request.Workspace.ServiceUri))
            {
                if (!uri.StartsWith("/"))
                    uri = "/" + uri;
                uri = request.Workspace.ServiceUri + uri;
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "IsBatchOperation", request.Batched.ToString() },
                { "RequestUri", Uri.UnescapeDataString(uri) }
            };
            if(Versioning.Server.SupportsV2Features)
            {
                parameters.Add("AbsoluteRequestUri", Uri.UnescapeDataString(uri));
                parameters.Add("AbsoluteServiceUri", request.Workspace.ServiceUri + "/");
                parameters.Add("IsBatchRequest", (request is BatchRequest || request.Batched).ToString());
                parameters.Add("RequestMethod", request.EffectiveVerb.ToString().ToUpperInvariant());
                // TODO: headers?
            }

            Parent.Add(new APICallLogEntry(GetServiceTypeFromWorkspace(request.Workspace) + "." + "OnStartProcessingRequest", parameters.ToArray()));
        }

        public void InitializeService()
        {
            InitializeService("TestDataWebService");
        }

        public void InitializeService(string typeName)
        {
            Parent.Add(new APICallLogEntry(typeName + ".InitializeService", new KeyValuePair<string, string>[0]));
        }

        public void QueryInterceptor(string methodName)
        {
            QueryInterceptor(DefaultServiceTypeName, methodName);
        }

        public void QueryInterceptor(string serviceTypeName, string methodName)
        {
            if (Parent.Workspace.GenerateCallOrderInterceptors)
            {
                Parent.Add(new APICallLogEntry(serviceTypeName + "." + methodName, new KeyValuePair<string, string>[0]));
            }
        }

        public void DefaultQueryInterceptor(ResourceContainer container)
        {
            QueryInterceptor("QueryInterceptor_" + container.Name);
        }

        public void ChangeInterceptor(string methodName, KeyedResourceInstance entity, UpdateOperations action)
        {
            ChangeInterceptor(DefaultServiceTypeName, methodName, Parent.Workspace.ContextNamespace + "." + entity.TypeName, action);
        }

        public void ChangeInterceptor(string methodName, string entityTypeName, UpdateOperations action)
        {
            ChangeInterceptor(DefaultServiceTypeName, methodName, entityTypeName, action);
        }

        public void ChangeInterceptor(string serviceTypeName, string methodName, string entityTypeName, UpdateOperations action)
        {
            if (Parent.Workspace.GenerateCallOrderInterceptors)
            {
                Parent.Add(new APICallLogEntry(serviceTypeName + "." + methodName,
                    "entity", entityTypeName,
                    "action", action.ToString()));
            }
        }

        public void DefaultChangeInterceptor(ResourceContainer container, ResourceType type, UpdateOperations action)
        {
            ChangeInterceptor("ChangeInterceptor_" + container.Name, Parent.QueryProvider.GetBackingTypeName(type), action);
        }
    }
    #endregion

    #region DataServiceProcessingPipeline
    public class DataServiceProcessingPipelineCallLogBuilder
    {
        private APICallLogBuilder Parent;

        public DataServiceProcessingPipelineCallLogBuilder(APICallLogBuilder parent)
        {
            Parent = parent;
        }

        public void ProcessingRequest(AstoriaRequest request)
        {
            if (!Versioning.Server.SupportsV2Features)
                return;

            this.Parent.Add("DataServiceProcessingPipeline.ProcessingRequest",
                "sender", Parent.Workspace.ServiceClassName,
                "AbsoluteRequestUri", Uri.UnescapeDataString(request.URI),
                "AbsoluteServiceUri", request.Workspace.ServiceUri + "/",
                "IsBatchRequest", (request is BatchRequest || request.Batched).ToString(),
                "RequestMethod", request.EffectiveVerb.ToString().ToUpperInvariant());
            // TODO: headers?
        }

        public void ProcessingChangeset()
        {
            if (!Versioning.Server.SupportsV2Features)
                return;

            this.Parent.Add("DataServiceProcessingPipeline.ProcessingChangeset",
                "sender", Parent.Workspace.ServiceClassName,
                "e", typeof(EventArgs).ToString());
        }

        public void ProcessedRequest(AstoriaRequest request)
        {
            if (!Versioning.Server.SupportsV2Features)
                return;

            this.Parent.Add("DataServiceProcessingPipeline.ProcessedRequest",
                "sender", Parent.Workspace.ServiceClassName,
                "AbsoluteRequestUri", Uri.UnescapeDataString(request.URI),
                "AbsoluteServiceUri", request.Workspace.ServiceUri + "/",
                "IsBatchRequest", (request is BatchRequest || request.Batched).ToString(),
                "RequestMethod", request.EffectiveVerb.ToString().ToUpperInvariant());
            // TODO: headers?
        }

        public void ProcessedChangeset()
        {
            if (!Versioning.Server.SupportsV2Features)
                return;

            this.Parent.Add("DataServiceProcessingPipeline.ProcessedChangeset",
                "sender", Parent.Workspace.ServiceClassName,
                "e", typeof(EventArgs).ToString());
        }
    }
    #endregion
    #endregion

    #region Stream
    public class StreamCallLogBuilder
    {
        private APICallLogBuilder parent;

        internal StreamCallLogBuilder(APICallLogBuilder parent)
        {
            this.parent = parent;
        }

        public void CanWrite()
        {
            this.parent.Add("System.IO.Stream.CanWrite");
        }

        public void CanRead()
        {
            this.parent.Add("System.IO.Stream.CanRead");
        }

        public void WriteBlocks(byte[] buffer, int blockSize)
        {
            byte[] block = new byte[blockSize];

            using (MemoryStream input = new MemoryStream(buffer))
            {
                long total = 0;
                int count = 0;
                while (0 < (count = input.Read(block, 0, block.Length)))
                {
                    this.Write(block, 0, count);
                    total += count;
                }
            }
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            this.parent.Add("System.IO.Stream.Write",
                "buffer", Convert.ToBase64String(buffer, offset, count),
                "offset", offset.ToString(),
                "count", count.ToString());
        }

        public void ReadBlocks(byte[] buffer, int blockSize)
        {
            byte[] block = new byte[blockSize];
            var stream = new MemoryStream(buffer);
            bool read = true;
            while(read && stream.CanRead)
            {
                var result = stream.Read(block, 0, blockSize);
                this.Read(block, 0, blockSize);
                read = result > 0;
            }
        }

        public void Read(byte[] buffer, int offset, int count)
        {
            this.parent.Add("System.IO.Stream.Read",
                "buffer", "byte[" + buffer.Length + "]",
                "offset", offset.ToString(),
                "count", count.ToString());
        }

        public void Close()
        {
            this.parent.Add("System.IO.Stream.Close");
        }

        public void Dispose()
        {
            this.parent.Add("System.IO.Stream.Dispose");
        }
    }
    #endregion
}
