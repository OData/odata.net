//---------------------------------------------------------------------
// <copyright file="DataServiceProviderSimulator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.OData.Service;
using Microsoft.OData.Service.Providers;
using System.Diagnostics;

namespace AstoriaUnitTests.Tests.Server.Simulators
{
    internal class DataServiceProviderSimulator : IDataServiceQueryProvider, IDataServiceMetadataProvider
    {
        static IEnumerable<ResourceType> emptyResourceTypes = Enumerable.Empty<ResourceType>();

        List<ResourceSet> resourceSets;
        List<ResourceType> resourceTypes;
        List<ServiceOperation> serviceOps;
        List<ResourceAssociationSet> resourceAssociationSets;
        IDataSourceSimulator dataSource;

        public DataServiceProviderSimulator()
        {
            this.resourceSets = new List<ResourceSet>();
            this.resourceTypes = new List<ResourceType>();
            this.serviceOps = new List<ServiceOperation>();
            this.resourceAssociationSets = new List<ResourceAssociationSet>();
            this.ContainerName = "TestContainer";
            this.ContainerNamespace = "TestNamespace";
        }

        public void AddResourceSet(ResourceSet set)
        {
            this.resourceSets.Add(set);
        }

        public void AddResourceType(ResourceType type)
        {
            this.resourceTypes.Add(type);
        }

        public void AddServiceOp(ServiceOperation sop)
        {
            this.serviceOps.Add(sop);
        }

        public void AddResourceAssociationSet(ResourceAssociationSet associationSet)
        {
            this.resourceAssociationSets.Add(associationSet);
        }

        public string ContainerNamespace
        {
            get;
            set;
        }

        public string ContainerName
        {
            get;
            set;
        }

        public IEnumerable<ResourceSet> ResourceSets
        {
            get { return this.resourceSets.AsEnumerable(); }
        }

        public IEnumerable<ResourceType> Types
        {
            get { return this.resourceTypes.AsEnumerable(); }
        }

        public IEnumerable<ServiceOperation> ServiceOperations
        {
            get { return this.serviceOps.AsEnumerable(); }
        }

        public bool TryResolveResourceSet(string name, out ResourceSet resourceSet)
        {
            resourceSet = this.resourceSets.Where(r => r.Name == name).FirstOrDefault();
            return resourceSets != null;
        }

        public ResourceAssociationSet GetResourceAssociationSet(ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            string key = resourceSet.Name + resourceType.FullName + resourceProperty == null ? string.Empty : resourceProperty.Name;
            foreach (var associationSet in this.resourceAssociationSets)
            {
                foreach (var end in new[] { associationSet.End1, associationSet.End2 })
                {
                    string endName = end.ResourceSet.Name + end.ResourceType.FullName + end.ResourceProperty == null ? string.Empty : end.ResourceProperty.Name;
                    if (endName == key)
                    {
                        return associationSet;
                    }
                }
            }

            return null;
        }

        public bool TryResolveResourceType(string fullName, out ResourceType resourceType)
        {
            resourceType = this.resourceTypes.FirstOrDefault(r => r.FullName == fullName);
            return resourceType != null;
        }

        public IEnumerable<ResourceType> GetDerivedTypes(ResourceType resourceType)
        {
            return this.resourceTypes.Where(type => resourceType.IsAssignableFrom(type) && type != resourceType);
        }

        public bool HasDerivedTypes(ResourceType resourceType)
        {
            return this.resourceTypes.Any(t => t.BaseType == resourceType);
        }

        public bool TryResolveServiceOperation(string name, out ServiceOperation serviceOperation)
        {
            serviceOperation = this.serviceOps.FirstOrDefault(r => r.Name == name);
            return serviceOperation != null;
        }

        public object CurrentDataSource
        {
            get { return this.dataSource; }
            set { this.dataSource = (IDataSourceSimulator)value; }
        }

        public bool IsNullPropagationRequired
        {
            get;
            set;
        }

        public IQueryable GetQueryRootForResourceSet(ResourceSet resourceSet)
        {
            Debug.Assert(this.dataSource != null, "Make sure CurrentDataSource is set before querying.");
            return this.dataSource.GetQueryableRoot(resourceSet.Name);
        }

        public ResourceType GetResourceType(object target)
        {
            foreach (var t in this.resourceTypes)
            {
                if (t.InstanceType.Equals(target.GetType()))
                {
                    return t;
                }
            }

            return null;
        }

        public object GetPropertyValue(object target, ResourceProperty resourceProperty)
        {
            Debug.Assert(this.dataSource != null, "Make sure CurrentDataSource is set before querying.");
            return this.dataSource.GetPropertyValue(target, resourceProperty.Name);
        }

        public object GetOpenPropertyValue(object target, string propertyName)
        {
            Debug.Assert(this.dataSource != null, "Make sure CurrentDataSource is set before querying.");
            return this.dataSource.GetPropertyValue(target, propertyName);            
        }

        public IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target)
        {
            return null;
        }

        public object InvokeServiceOperation(ServiceOperation serviceOperation, object[] parameters)
        {
            return null;
        }
    }

    internal interface IDataSourceSimulator
    {
        IQueryable GetQueryableRoot(string resourceSet);
        object GetPropertyValue(object target, string propertyName);
    }
}
