//---------------------------------------------------------------------
// <copyright file="EmptyOpenTypesContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service.Providers;
    using System.ServiceModel.Web;
    using System.ServiceModel.Activation;
    using System.IO;

    public class StubTypeWithId
    {
        public int Id { get; set; }
    }

    public class EmptyOpenTypesContext : IDataServiceMetadataProvider, IDataServiceQueryProvider
    {
        private readonly ResourceType resourceType;
        private readonly ResourceSet resourceSet;

        public EmptyOpenTypesContext()
        {
            this.resourceType = new ResourceType(typeof(StubTypeWithId), ResourceTypeKind.EntityType, null, "OpenTypes", "Entity", false);
            this.resourceType.CanReflectOnInstanceType = true;
            this.resourceType.IsOpenType = true;
            var idProperty = new ResourceProperty("Id", ResourcePropertyKind.Primitive | ResourcePropertyKind.Key, ResourceType.GetPrimitiveResourceType(typeof(int)));
            idProperty.CanReflectOnInstanceTypeProperty = true;
            this.resourceType.AddProperty(idProperty);
            this.resourceSet = new ResourceSet("Entities", this.resourceType);
            this.resourceSet.SetReadOnly();
        }

        public string ContainerName
        {
            get { return "OpenTypesContext"; }
        }
        public string ContainerNamespace
        {
            get { return "OpenTypes"; }
        }
        public IEnumerable<ResourceType> GetDerivedTypes(ResourceType resourceType)
        {
            return Enumerable.Empty<ResourceType>();
        }
        public ResourceAssociationSet GetResourceAssociationSet(ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            return null;
        }
        public bool HasDerivedTypes(ResourceType resourceType)
        {
            return false;
        }
        public IEnumerable<ResourceSet> ResourceSets
        {
            get { yield return this.resourceSet; }
        }
        public IEnumerable<ServiceOperation> ServiceOperations
        {
            get { return Enumerable.Empty<ServiceOperation>(); }
        }
        public bool TryResolveResourceSet(string name, out ResourceSet resourceSet)
        {
            resourceSet = this.resourceSet;
            return name == this.resourceSet.Name;
        }
        public bool TryResolveResourceType(string name, out ResourceType resourceType)
        {
            resourceType = this.resourceType;
            return name == this.resourceType.FullName;
        }
        public bool TryResolveServiceOperation(string name, out ServiceOperation serviceOperation)
        {
            serviceOperation = null;
            return false;
        }
        public IEnumerable<ResourceType> Types
        {
            get { yield return this.resourceType; }
        }
        public object CurrentDataSource { get; set; }
        public object GetOpenPropertyValue(object target, string propertyName)
        {
            return null;
        }
        public IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target)
        {
            return Enumerable.Empty<KeyValuePair<string, object>>();
        }
        public object GetPropertyValue(object target, ResourceProperty resourceProperty)
        {
            return 5;
        }
        public IQueryable GetQueryRootForResourceSet(ResourceSet resourceSet)
        {
            return new List<StubTypeWithId>() { new StubTypeWithId { Id = 5 } }.AsQueryable();
        }
        public ResourceType GetResourceType(object target)
        {
            return this.resourceType;
        }
        public object InvokeServiceOperation(ServiceOperation serviceOperation, object[] parameters)
        {
            throw new NotImplementedException();
        }
        public bool IsNullPropagationRequired
        {
            get { return true; }
        }
    }
}
